// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.CheckBasics

open System.Collections.Generic
open FSharp.Compiler.Diagnostics
open Internal.Utilities.Library
open FSharp.Compiler.AccessibilityLogic
open FSharp.Compiler.CompilerGlobalState
open FSharp.Compiler.ConstraintSolver
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Import
open FSharp.Compiler.InfoReader
open FSharp.Compiler.NameResolution
open FSharp.Compiler.PatternMatchCompilation
open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTreeOps
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps

/// Represents information about the initialization field used to check that object constructors
/// have completed before fields are accessed.
type SafeInitData =
    | SafeInitField of RecdFieldRef * RecdField
    | NoSafeInitInfo

/// Represents information about object constructors
type CtorInfo =
    {
        /// Object model constructors have a very specific form to satisfy .NET limitations.
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

        /// Is the an implicit constructor or an explicit one?
        ctorIsImplicit: bool
    }

    static member InitialExplicit: safeThisValOpt: Val option * safeInitInfo: SafeInitData -> CtorInfo

    static member InitialImplicit: unit -> CtorInfo

/// Represents an item in the environment that may restrict the automatic generalization of later
/// declarations because it refers to type inference variables. As type inference progresses
/// these type inference variables may get solved.
[<NoEquality; NoComparison; Sealed>]
type UngeneralizableItem =

    new: (unit -> FreeTyvars) -> UngeneralizableItem

    member internal GetFreeTyvars: unit -> FreeTyvars

    member internal WillNeverHaveFreeTypars: bool

    member internal CachedFreeLocalTycons: FreeTycons

    member internal CachedFreeTraitSolutions: FreeLocals

/// Represents the type environment at a particular scope. Includes the name
/// resolution environment, the ungeneralizable items from earlier in the scope
/// and other information about the scope.
[<NoEquality; NoComparison>]
type TcEnv =
    {
        /// Name resolution information
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

        eIsControlFlow: bool
    }

    member DisplayEnv: DisplayEnv

    member NameEnv: NameResolutionEnv

    member AccessRights: AccessorDomain

/// Represents the current environment of type variables that have implicit scope
/// (i.e. are without explicit declaration).
type UnscopedTyparEnv = UnscopedTyparEnv of NameMap<Typar>

/// A type to represent information associated with values to indicate what explicit (declared) type parameters
/// are given and what additional type parameters can be inferred, if any.
///
/// The declared type parameters, e.g. let f<'a> (x:'a) = x, plus an indication
/// of whether additional polymorphism may be inferred, e.g. let f<'a, ..> (x:'a) y = x
type ExplicitTyparInfo = ExplicitTyparInfo of rigidCopyOfDeclaredTypars: Typars * declaredTypars: Typars * infer: bool

type ArgAndRetAttribs = ArgAndRetAttribs of Attribs list list * Attribs

/// Indicates whether constraints should be checked when checking syntactic types
type CheckConstraints =
    | CheckCxs
    | NoCheckCxs

/// Represents the ValReprInfo for a value, before the typars are fully inferred
type PrelimValReprInfo = PrelimValReprInfo of curriedArgInfos: ArgReprInfo list list * returnInfo: ArgReprInfo

/// Holds the initial ValMemberInfo and other information before it is fully completed
type PrelimMemberInfo = PrelimMemberInfo of memberInfo: ValMemberInfo * logicalName: string * compiledName: string

/// Represents the results of the first phase of preparing simple values from a pattern
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

    member Type: TType

    member Ident: Ident

/// Translation of patterns is split into three phases. The first collects names.
/// The second is run after val_specs have been created for those names and inference
/// has been resolved. The second phase is run by applying a function returned by the
/// first phase. The input to the second phase is a List.map that gives the Val and type scheme
/// for each value bound by the pattern.
type TcPatPhase2Input =
    | TcPatPhase2Input of NameMap<Val * GeneralizedType> * bool

    member WithRightPath: unit -> TcPatPhase2Input

/// Represents the context flowed left-to-right through pattern checking
type TcPatLinearEnv = TcPatLinearEnv of tpenv: UnscopedTyparEnv * names: NameMap<PrelimVal1> * takenNames: Set<string>

/// Represents the flags passsed to TcPat regarding the binding location
type TcPatValFlags =
    | TcPatValFlags of
        inlineFlag: ValInline *
        explicitTyparInfo: ExplicitTyparInfo *
        argAndRetAttribs: ArgAndRetAttribs *
        isMutable: bool *
        visibility: SynAccess option *
        isCompilerGenerated: bool

/// Represents the compilation environment for typechecking a single file in an assembly.
[<NoEquality; NoComparison>]
type TcFileState =
    {
        g: TcGlobals

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
        amap: ImportMap

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

        // forward call
        TcPat: WarnOnUpperFlag
            -> TcFileState
            -> TcEnv
            -> PrelimValReprInfo option
            -> TcPatValFlags
            -> TcPatLinearEnv
            -> TType
            -> SynPat
            -> (TcPatPhase2Input -> Pattern) * TcPatLinearEnv

        // forward call
        TcSimplePats: TcFileState
            -> bool
            -> CheckConstraints
            -> TType
            -> TcEnv
            -> TcPatLinearEnv
            -> SynSimplePats
            -> string list * TcPatLinearEnv

        // forward call
        TcSequenceExpressionEntry: TcFileState
            -> TcEnv
            -> OverallTy
            -> UnscopedTyparEnv
            -> bool * SynExpr
            -> range
            -> Expr * UnscopedTyparEnv

        // forward call
        TcArrayOrListComputedExpression: TcFileState
            -> TcEnv
            -> OverallTy
            -> UnscopedTyparEnv
            -> bool * SynExpr
            -> range
            -> Expr * UnscopedTyparEnv

        // forward call
        TcComputationExpression: TcFileState
            -> TcEnv
            -> OverallTy
            -> UnscopedTyparEnv
            -> range * Expr * TType * SynExpr
            -> Expr * UnscopedTyparEnv
    }

    static member Create:
        g: TcGlobals *
        isScript: bool *
        amap: ImportMap *
        thisCcu: CcuThunk *
        isSig: bool *
        haveSig: bool *
        conditionalDefines: string list option *
        tcSink: TcResultsSink *
        tcVal: TcValF *
        isInternalTestSpanStackReferring: bool *
        diagnosticOptions: FSharpDiagnosticOptions *
        tcPat: (WarnOnUpperFlag -> TcFileState -> TcEnv -> PrelimValReprInfo option -> TcPatValFlags -> TcPatLinearEnv -> TType -> SynPat -> (TcPatPhase2Input -> Pattern) * TcPatLinearEnv) *
        tcSimplePats: (TcFileState -> bool -> CheckConstraints -> TType -> TcEnv -> TcPatLinearEnv -> SynSimplePats -> string list * TcPatLinearEnv) *
        tcSequenceExpressionEntry: (TcFileState -> TcEnv -> OverallTy -> UnscopedTyparEnv -> bool * SynExpr -> range -> Expr * UnscopedTyparEnv) *
        tcArrayOrListSequenceExpression: (TcFileState -> TcEnv -> OverallTy -> UnscopedTyparEnv -> bool * SynExpr -> range -> Expr * UnscopedTyparEnv) *
        tcComputationExpression: (TcFileState -> TcEnv -> OverallTy -> UnscopedTyparEnv -> range * Expr * TType * SynExpr -> Expr * UnscopedTyparEnv) ->
            TcFileState
