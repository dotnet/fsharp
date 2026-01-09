// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.CheckBasics

open System.Collections.Concurrent
open System.Collections.Generic

open FSharp.Compiler.Diagnostics
open Internal.Utilities.Library
open Internal.Utilities.Collections
open FSharp.Compiler
open FSharp.Compiler.AccessibilityLogic
open FSharp.Compiler.CompilerGlobalState
open FSharp.Compiler.ConstraintSolver
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.InfoReader
open FSharp.Compiler.NameResolution
open FSharp.Compiler.PatternMatchCompilation
open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTreeOps
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps

#if !NO_TYPEPROVIDERS
#endif

/// The ValReprInfo for a value, except the number of typars is not yet inferred
type PrelimValReprInfo =
    | PrelimValReprInfo of
        curriedArgInfos: ArgReprInfo list list *
        returnInfo: ArgReprInfo

//-------------------------------------------------------------------------
// Data structures that track the gradual accumulation of information
// about values and members during inference.
//-------------------------------------------------------------------------

type PrelimMemberInfo =
    | PrelimMemberInfo of
        memberInfo: ValMemberInfo *
        logicalName: string *
        compiledName: string

/// Indicates whether constraints should be checked when checking syntactic types
type CheckConstraints =
    | CheckCxs
    | NoCheckCxs

/// A type to represent information associated with values to indicate what explicit (declared) type parameters
/// are given and what additional type parameters can be inferred, if any.
///
/// The declared type parameters, e.g. let f<'a> (x:'a) = x, plus an indication
/// of whether additional polymorphism may be inferred, e.g. let f<'a, ..> (x:'a) y = x
type ExplicitTyparInfo =
    | ExplicitTyparInfo of
        rigidCopyOfDeclaredTypars: Typars *
        declaredTypars: Typars *
        infer: bool

type ArgAndRetAttribs = ArgAndRetAttribs of Attribs list list * Attribs

/// The results of preliminary pass over patterns to extract variables being declared.
// We should make this a record for cleaner code
type PrelimVal1 =
    | PrelimVal1 of
        id: Ident *
        explicitTyparInfo: ExplicitTyparInfo *
        prelimType: TType *
        prelimValReprInfo: PrelimValReprInfo option *
        memberInfoOpt: PrelimMemberInfo option *
        isMutable: bool *
        inlineFlag: ValInline *
        baseOrThisInfo: ValBaseOrThisInfo *
        argAttribs: ArgAndRetAttribs *
        visibility: SynAccess option *
        isCompGen: bool

    member x.Type = let (PrelimVal1(prelimType=ty)) = x in ty

    member x.Ident = let (PrelimVal1(id=id)) = x in id

type UnscopedTyparEnv = UnscopedTyparEnv of NameMap<Typar>

type TcPatLinearEnv = TcPatLinearEnv of tpenv: UnscopedTyparEnv * names: NameMap<PrelimVal1> * takenNames: Set<string>

/// Translation of patterns is split into three phases. The first collects names.
/// The second is run after val_specs have been created for those names and inference
/// has been resolved. The second phase is run by applying a function returned by the
/// first phase. The input to the second phase is a List.map that gives the Val and type scheme
/// for each value bound by the pattern.
type TcPatPhase2Input =
    | TcPatPhase2Input of NameMap<Val * GeneralizedType> * bool

    // Get an input indicating we are no longer on the left-most path through a disjunctive "or" pattern
    member x.WithRightPath() = (let (TcPatPhase2Input(a, _)) = x in TcPatPhase2Input(a, false))

/// Represents information about the initialization field used to check that object constructors
/// have completed before fields are accessed.
type SafeInitData =
    | SafeInitField of RecdFieldRef * RecdField
    | NoSafeInitInfo

type TcPatValFlags = 
    | TcPatValFlags of 
        inlineFlag: ValInline * 
        explicitTyparInfo: ExplicitTyparInfo * 
        argAndRetAttribs: ArgAndRetAttribs * 
        isMutable: bool * 
        visibility: SynAccess option * 
        isCompilerGenerated: bool

/// Represents information about object constructors
type CtorInfo =
    { /// Object model constructors have a very specific form to satisfy .NET limitations.
      /// For "new = \arg. { new C with ... }"
      ///     ctor = 3 indicates about to type check "\arg. (body)",
      ///     ctor = 2 indicates about to type check "body"
      ///     ctor = 1 indicates actually type checking the body expression
      /// 0 indicates everywhere else, including auxiliary expressions such expr1 in "let x = expr1 in { new ... }"
      /// REVIEW: clean up this rather odd approach ...
      ctorShapeCounter: int

      /// A handle to the ref cell to hold results of 'this' for 'type X() as x = ...' and 'new() as x = ...' constructs
      /// in case 'x' is used in the arguments to the 'inherits' call.
      safeThisValOpt: Val option

      /// A handle to the boolean ref cell to hold success of initialized 'this' for 'type X() as x = ...' constructs
      safeInitInfo: SafeInitData

      /// Is there an implicit constructor or an explicit one?
      ctorIsImplicit: bool
    }

    static member InitialExplicit (safeThisValOpt, safeInitInfo) =
        { ctorShapeCounter = 3
          safeThisValOpt = safeThisValOpt
          safeInitInfo = safeInitInfo
          ctorIsImplicit = false}

    static member InitialImplicit () =
        { ctorShapeCounter = 0
          safeThisValOpt = None
          safeInitInfo = NoSafeInitInfo
          ctorIsImplicit = true }


/// Represents an item in the environment that may restrict the automatic generalization of later
/// declarations because it refers to type inference variables. As type inference progresses
/// these type inference variables may get solved.
[<NoEquality; NoComparison; Sealed>]
type UngeneralizableItem(computeFreeTyvars: unit -> FreeTyvars) =

    // Flag is for: have we determined that this item definitely has
    // no free type inference variables? This implies that
    //   (a) it will _never_ have any free type inference variables as further constraints are added to the system.
    //   (b) its set of FreeTycons will not change as further constraints are added to the system
    let mutable willNeverHaveFreeTypars = false

    // If WillNeverHaveFreeTypars then we can cache the computation of FreeTycons, since they are invariant.
    let mutable cachedFreeLocalTycons = emptyFreeTycons

    // If WillNeverHaveFreeTypars then we can cache the computation of FreeTraitSolutions, since they are invariant.
    let mutable cachedFreeTraitSolutions = emptyFreeLocals

    member _.GetFreeTyvars() =
        let fvs = computeFreeTyvars()
        if fvs.FreeTypars.IsEmpty then
            willNeverHaveFreeTypars <- true
            cachedFreeLocalTycons <- fvs.FreeTycons
            cachedFreeTraitSolutions <- fvs.FreeTraitSolutions
        fvs

    member _.WillNeverHaveFreeTypars = willNeverHaveFreeTypars

    member _.CachedFreeLocalTycons = cachedFreeLocalTycons

    member _.CachedFreeTraitSolutions = cachedFreeTraitSolutions

/// Represents the type environment at a particular scope. Includes the name
/// resolution environment, the ungeneralizable items from earlier in the scope
/// and other information about the scope.
[<NoEquality; NoComparison>]
type TcEnv =
    { /// Name resolution information
      eNameResEnv: NameResolutionEnv

      /// The list of items in the environment that may contain free inference
      /// variables (which may not be generalized). The relevant types may
      /// change as a result of inference equations being asserted, hence may need to
      /// be recomputed.
      eUngeneralizableItems: UngeneralizableItem list

      // Two (!) versions of the current module path
      // These are used to:
      //    - Look up the appropriate point in the corresponding signature
      //      see if an item is public or not
      //    - Change fslib canonical module type to allow compiler references to these items
      //    - Record the cpath for concrete modul_specs, tycon_specs and excon_specs so they can cache their generated IL representation where necessary
      //    - Record the pubpath of public, concrete {val, tycon, modul, excon}_specs.
      //      This information is used mainly when building non-local references
      //      to public items.
      //
      // Of the two, 'ePath' is the one that's barely used. It's only
      // used by UpdateAccModuleOrNamespaceType to modify the CCU while compiling FSharp.Core
      ePath: Ident list

      eCompPath: CompilationPath

      eAccessPath: CompilationPath

      /// This field is computed from other fields, but we amortize the cost of computing it.
      eAccessRights: AccessorDomain

      /// Internals under these should be accessible
      eInternalsVisibleCompPaths: CompilationPath list

      /// Mutable accumulator for the current module type
      eModuleOrNamespaceTypeAccumulator: ModuleOrNamespaceType ref

      /// Context information for type checker
      eContextInfo: ContextInfo

      /// Here Some tcref indicates we can access protected members in all super types
      eFamilyType: TyconRef option

      // Information to enforce special restrictions on valid expressions
      // for .NET constructors.
      eCtorInfo: CtorInfo option

      eCallerMemberName: string option

      // Active arg infos in iterated lambdas , allowing us to determine the attributes of arguments
      eLambdaArgInfos: ArgReprInfo list list

      // Do we lay down an implicit debug point?
      eIsControlFlow: bool
      
      // In order to avoid checking implicit-yield expressions multiple times, we cache the resulting checked expressions.
      // This avoids exponential behavior in the type checker when nesting implicit-yield expressions.
      eCachedImplicitYieldExpressions : HashMultiMap<range, SynExpr * TType * Expr>
    }

    member tenv.DisplayEnv = tenv.eNameResEnv.DisplayEnv

    member tenv.NameEnv = tenv.eNameResEnv

    member tenv.AccessRights = tenv.eAccessRights

    override tenv.ToString() = "TcEnv(...)"

/// Represents the compilation environment for typechecking a single file in an assembly.
[<NoEquality; NoComparison>]
type TcFileState =
    { g: TcGlobals

      /// Push an entry every time a recursive value binding is used,
      /// in order to be able to fix up recursive type applications as
      /// we infer type parameters
      mutable recUses: ValMultiMap<Expr ref * range * bool>

      /// Guard against depth of expression nesting, by moving to new stack when a maximum depth is reached
      stackGuard: StackGuard

      /// Set to true if this file causes the creation of generated provided types.
      mutable createsGeneratedProvidedTypes: bool

      /// Are we in a script? if so relax the reporting of discarded-expression warnings at the top level
      isScript: bool

      /// Environment needed to convert IL types to F# types in the importer.
      amap: Import.ImportMap

      /// Used to generate new syntactic argument names in post-parse syntactic processing
      synArgNameGenerator: SynArgNameGenerator

      tcSink: TcResultsSink

      /// Holds a reference to the component being compiled.
      /// This field is very rarely used (mainly when fixing up forward references to fslib.
      thisCcu: CcuThunk

      /// Holds the current inference constraints
      css: ConstraintSolverState

      /// Are we compiling the signature of a module from fslib?
      compilingCanonicalFslibModuleType: bool

      /// Is this a .fsi file?
      isSig: bool

      /// Does this .fs file have a .fsi file?
      haveSig: bool

      /// Used to generate names
      niceNameGen: NiceNameGenerator

      /// Used to read and cache information about types and members
      infoReader: InfoReader

      /// Used to resolve names
      nameResolver: NameResolver

      /// The set of active conditional defines. The value is None when conditional erasure is disabled in tooling.
      conditionalDefines: string list option

      namedDebugPointsForInlinedCode: Dictionary<NamedDebugPointKey, range>

      isInternalTestSpanStackReferring: bool

      diagnosticOptions: FSharpDiagnosticOptions

      argInfoCache: ConcurrentDictionary<string * range, ArgReprInfo>

      // forward call
      TcPat: WarnOnUpperFlag -> TcFileState -> TcEnv -> PrelimValReprInfo option -> TcPatValFlags -> TcPatLinearEnv -> TType -> SynPat -> (TcPatPhase2Input -> Pattern) * TcPatLinearEnv

      // forward call
      TcSimplePats: TcFileState -> bool -> CheckConstraints -> TType -> TcEnv -> TcPatLinearEnv -> SynSimplePats -> SynPat list * bool -> string list * TcPatLinearEnv

      // forward call
      TcSequenceExpressionEntry: TcFileState -> TcEnv -> OverallTy -> UnscopedTyparEnv -> bool * SynExpr -> range -> Expr * UnscopedTyparEnv

      // forward call
      TcArrayOrListComputedExpression: TcFileState -> TcEnv -> OverallTy -> UnscopedTyparEnv -> bool * SynExpr -> range -> Expr * UnscopedTyparEnv

      // forward call
      TcComputationExpression: TcFileState -> TcEnv -> OverallTy -> UnscopedTyparEnv -> range * Expr * TType * SynExpr -> Expr * UnscopedTyparEnv
    }

    /// Create a new compilation environment
    static member Create
         (g, isScript, amap, thisCcu, isSig, haveSig, conditionalDefines, tcSink, tcVal, isInternalTestSpanStackReferring, diagnosticOptions,
          tcPat,
          tcSimplePats,
          tcSequenceExpressionEntry,
          tcArrayOrListSequenceExpression,
          tcComputationExpression) =

        let niceNameGen = NiceNameGenerator()
        let infoReader = InfoReader(g, amap)
        let instantiationGenerator m tpsorig = FreshenTypars g m tpsorig
        let nameResolver = NameResolver(g, amap, infoReader, instantiationGenerator)
        { g = g
          amap = amap
          recUses = ValMultiMap<_>.Empty
          stackGuard = StackGuard("TcFileState")
          createsGeneratedProvidedTypes = false
          thisCcu = thisCcu
          isScript = isScript
          css = ConstraintSolverState.New(g, amap, infoReader, tcVal)
          infoReader = infoReader
          tcSink = tcSink
          nameResolver = nameResolver
          niceNameGen = niceNameGen
          synArgNameGenerator = SynArgNameGenerator()
          isSig = isSig
          haveSig = haveSig
          namedDebugPointsForInlinedCode = Dictionary()
          compilingCanonicalFslibModuleType = (isSig || not haveSig) && g.compilingFSharpCore
          conditionalDefines = conditionalDefines
          isInternalTestSpanStackReferring = isInternalTestSpanStackReferring
          diagnosticOptions = diagnosticOptions
          argInfoCache = ConcurrentDictionary()
          TcPat = tcPat
          TcSimplePats = tcSimplePats
          TcSequenceExpressionEntry = tcSequenceExpressionEntry
          TcArrayOrListComputedExpression = tcArrayOrListSequenceExpression
          TcComputationExpression = tcComputationExpression
        }

    override _.ToString() = "<cenv>"

open FSharp.Compiler.AttributeChecking
open FSharp.Compiler.Syntax.PrettyNaming
open FSharp.Compiler.Text.Range
open FSharp.Compiler.TypedTreeBasics

let CheckNamespaceModuleOrTypeName (g: TcGlobals) (id: Ident) = 
    // type names '[]' etc. are used in fslib
    if not g.compilingFSharpCore && id.idText.IndexOfAny IllegalCharactersInTypeAndNamespaceNames <> -1 then 
        errorR(Error(FSComp.SR.tcInvalidNamespaceModuleTypeUnionName(), id.idRange))

/// Adjust the TcEnv to account for opening the set of modules or namespaces implied by an `open` declaration
let OpenModuleOrNamespaceRefs tcSink g amap scopem root env mvvs openDeclaration =
    let env =
        if isNil mvvs then env else
        { env with eNameResEnv = AddModuleOrNamespaceRefsContentsToNameEnv g amap env.eAccessRights scopem root env.eNameResEnv mvvs }
    CallEnvSink tcSink (scopem, env.NameEnv, env.eAccessRights)
    CallOpenDeclarationSink tcSink openDeclaration
    env

//-------------------------------------------------------------------------
// Bind 'open' declarations
//------------------------------------------------------------------------- 

let TcOpenLidAndPermitAutoResolve tcSink (env: TcEnv) amap (longId : Ident list) =
    let ad = env.AccessRights
    match longId with
    | [] -> []
    | id :: rest ->
        let m = longId |> List.map (fun id -> id.idRange) |> List.reduce unionRanges
        match ResolveLongIdentAsModuleOrNamespace tcSink amap m true OpenQualified env.NameEnv ad id rest true ShouldNotifySink.Yes with 
        | Result res -> res
        | Exception err ->
            errorR(err); []

let TcOpenModuleOrNamespaceDecl tcSink g amap scopem env (longId, m) = 
    match TcOpenLidAndPermitAutoResolve tcSink env amap longId with
    | [] -> env, []
    | modrefs ->

    // validate opened namespace names
    for id in longId do
        if id.idText <> MangledGlobalName then
            CheckNamespaceModuleOrTypeName g id

    let IsPartiallyQualifiedNamespace (modref: ModuleOrNamespaceRef) = 
        let (CompPath(_, _, p)) = modref.CompilationPath 
        // Bug FSharp 1.0 3274: FSI paths don't count when determining this warning
        let p = 
            match p with 
            | [] -> []
            | (h, _) :: t -> if h.StartsWithOrdinal FsiDynamicModulePrefix then t else p

        // See https://fslang.uservoice.com/forums/245727-f-language/suggestions/6107641-make-microsoft-prefix-optional-when-using-core-f
        let isFSharpCoreSpecialCase =
            match ccuOfTyconRef modref with 
            | None -> false
            | Some ccu -> 
                ccuEq ccu g.fslibCcu &&
                // Check if we're using a reference one string shorter than what we expect.
                //
                // "p" is the fully qualified path _containing_ the thing we're opening, e.g. "Microsoft.FSharp" when opening "Microsoft.FSharp.Data"
                // "longId" is the text being used, e.g. "FSharp.Data"
                //    Length of thing being opened = p.Length + 1
                //    Length of reference = longId.Length
                // So the reference is a "shortened" reference if (p.Length + 1) - 1 = longId.Length
                (p.Length + 1) - 1 = longId.Length && 
                fst p[0] = "Microsoft" 

        modref.IsNamespace && 
        p.Length >= longId.Length &&
        not isFSharpCoreSpecialCase
        // Allow "open Foo" for "Microsoft.Foo" from FSharp.Core

    modrefs |> List.iter (fun (_, modref, _) ->
       if modref.IsModule && HasFSharpAttribute g g.attrib_RequireQualifiedAccessAttribute modref.Attribs then 
           errorR(Error(FSComp.SR.tcModuleRequiresQualifiedAccess(fullDisplayTextOfModRef modref), m)))

    // Bug FSharp 1.0 3133: 'open Lexing'. Skip this warning if we successfully resolved to at least a module name
    if not (modrefs |> List.exists (fun (_, modref, _) -> modref.IsModule && not (HasFSharpAttribute g g.attrib_RequireQualifiedAccessAttribute modref.Attribs))) then
        modrefs |> List.iter (fun (_, modref, _) ->
            if IsPartiallyQualifiedNamespace modref then 
                 errorR(Error(FSComp.SR.tcOpenUsedWithPartiallyQualifiedPath(fullDisplayTextOfModRef modref), m)))
        
    let modrefs = List.map p23 modrefs
    modrefs |> List.iter (fun modref -> CheckEntityAttributes g modref m |> CommitOperationResult)        

    let openDecl = OpenDeclaration.Create (SynOpenDeclTarget.ModuleOrNamespace (SynLongIdent(longId, [], []), m), modrefs, [], scopem, false)
    let env = OpenModuleOrNamespaceRefs tcSink g amap scopem false env modrefs openDecl
    env, [openDecl]
