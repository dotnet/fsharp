// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.CheckExprs

open System
open FSharp.Compiler 
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.Internal
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.AccessibilityLogic
open FSharp.Compiler.CompilerGlobalState
open FSharp.Compiler.ConstraintSolver
open FSharp.Compiler.Import
open FSharp.Compiler.InfoReader
open FSharp.Compiler.Infos
open FSharp.Compiler.MethodOverrides
open FSharp.Compiler.NameResolution
open FSharp.Compiler.Range
open FSharp.Compiler.SyntaxTree
open FSharp.Compiler.SyntaxTreeOps
open FSharp.Compiler.XmlDoc
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps

#if !NO_EXTENSIONTYPING
open FSharp.Compiler.ExtensionTyping
#endif

/// Represents information about the initialization field used to check that object constructors
/// have completed before fields are accessed.
type SafeInitData = 
    | SafeInitField of RecdFieldRef * RecdField
    | NoSafeInitInfo 

/// Represents information about object constructors
[<Sealed>]
type CtorInfo

val InitialImplicitCtorInfo: unit -> CtorInfo

/// Represents an item in the environment that may restrict the automatic generalization of later
/// declarations because it refers to type inference variables. As type inference progresses
/// these type inference variables may get solved.
[<NoEquality; NoComparison>]
type UngeneralizableItem

[<NoEquality; NoComparison>]
/// Represents the type environment at a particular scope. Includes the name
/// resolution environment, the ungeneralizable items from earlier in the scope
/// and other information about the scope.
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
    } 

    member DisplayEnv : DisplayEnv
    member NameEnv : NameResolution.NameResolutionEnv
    member AccessRights : AccessorDomain

//-------------------------------------------------------------------------
// Some of the exceptions arising from type checking. These should be moved to 
// use ErrorLogger.
//------------------------------------------------------------------------- 

exception BakedInMemberConstraintName of string * range
exception FunctionExpected of DisplayEnv * TType * range
exception NotAFunction of DisplayEnv * TType * range * range
exception NotAFunctionButIndexer of DisplayEnv * TType * string option * range * range
exception Recursion of DisplayEnv * Ident * TType * TType * range
exception RecursiveUseCheckedAtRuntime of DisplayEnv * ValRef * range
exception LetRecEvaluatedOutOfOrder of DisplayEnv * ValRef * ValRef * range
exception LetRecCheckedAtRuntime of range
exception LetRecUnsound of DisplayEnv * ValRef list * range
exception TyconBadArgs of DisplayEnv * TyconRef * int * range
exception UnionCaseWrongArguments of DisplayEnv * int * int * range
exception UnionCaseWrongNumberOfArgs of DisplayEnv * int * int * range
exception FieldsFromDifferentTypes of DisplayEnv * RecdFieldRef * RecdFieldRef * range
exception FieldGivenTwice of DisplayEnv * RecdFieldRef * range
exception MissingFields of string list * range
exception UnitTypeExpected of DisplayEnv * TType * range
exception UnitTypeExpectedWithEquality of DisplayEnv * TType * range
exception UnitTypeExpectedWithPossiblePropertySetter of DisplayEnv * TType * string * string * range
exception UnitTypeExpectedWithPossibleAssignment of DisplayEnv * TType * bool * string * range
exception FunctionValueUnexpected of DisplayEnv * TType * range
exception UnionPatternsBindDifferentNames of range
exception VarBoundTwice of Ident
exception ValueRestriction of DisplayEnv * bool * Val * Typar * range
exception ValNotMutable of DisplayEnv * ValRef * range
exception ValNotLocal of DisplayEnv * ValRef * range
exception InvalidRuntimeCoercion of DisplayEnv * TType * TType * range
exception IndeterminateRuntimeCoercion of DisplayEnv * TType * TType * range
exception IndeterminateStaticCoercion of DisplayEnv * TType * TType * range
exception StaticCoercionShouldUseBox of DisplayEnv * TType * TType * range
exception RuntimeCoercionSourceSealed of DisplayEnv * TType * range
exception CoercionTargetSealed of DisplayEnv * TType * range
exception UpcastUnnecessary of range
exception TypeTestUnnecessary of range
exception SelfRefObjCtor of bool * range
exception VirtualAugmentationOnNullValuedType of range
exception NonVirtualAugmentationOnNullValuedType of range
exception UseOfAddressOfOperator of range
exception DeprecatedThreadStaticBindingWarning of range
exception IntfImplInIntrinsicAugmentation of range
exception IntfImplInExtrinsicAugmentation of range
exception OverrideInIntrinsicAugmentation of range
exception OverrideInExtrinsicAugmentation of range
exception NonUniqueInferredAbstractSlot of TcGlobals * DisplayEnv * string * MethInfo * MethInfo * range
exception StandardOperatorRedefinitionWarning of string * range
exception InvalidInternalsVisibleToAssemblyName of (*badName*)string * (*fileName option*) string option

val TcFieldInit : range -> ILFieldInit -> Const

val LightweightTcValForUsingInBuildMethodCall : g : TcGlobals -> vref:ValRef -> vrefFlags : ValUseFlag -> vrefTypeInst : TTypes -> m : range -> Expr * TType

//-------------------------------------------------------------------------
// The rest are all helpers needed for declaration checking (CheckDecls.fs)
//------------------------------------------------------------------------- 

/// Represents the compilation environment for typechecking a single file in an assembly. 
[<NoEquality; NoComparison>]
type TcFileState = 
    { g: TcGlobals

      /// Push an entry every time a recursive value binding is used, 
      /// in order to be able to fix up recursive type applications as 
      /// we infer type parameters 
      mutable recUses: ValMultiMap<(Expr ref * range * bool)>
      
      /// Checks to run after all inference is complete. 
      mutable postInferenceChecks: ResizeArray<unit -> unit>

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
      topCcu: CcuThunk 
      
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
            
      isInternalTestSpanStackReferring: bool
    } 
    static member Create: 
        g: TcGlobals *
        isScript: bool *
        niceNameGen: NiceNameGenerator *
        amap: ImportMap *
        topCcu: CcuThunk *
        isSig: bool *
        haveSig: bool *
        conditionalDefines: string list option * 
        tcSink: TcResultsSink *
        tcVal: TcValF *
        isInternalTestSpanStackReferring: bool
         -> TcFileState

type MemberOrValContainerInfo = 
    | MemberOrValContainerInfo of
        TyconRef *
        (TType * SlotImplSet) option *
        Val option *
        SafeInitData *
        Typars

/// Provides information about the context for a value or member definition 
type ContainerInfo = 
    | ContainerInfo of 
        ParentRef *  
        MemberOrValContainerInfo option
    member ParentRef : ParentRef

val ExprContainerInfo: ContainerInfo

type NewSlotsOK = 
    | NewSlotsOK 
    | NoNewSlots

type OverridesOK = 
    | OverridesOK 
    | WarnOnOverrides
    | ErrorOnOverrides

/// A flag to represent the sort of bindings are we processing.
type DeclKind = 
    | ModuleOrMemberBinding 

    /// Extensions to a type within the same assembly
    | IntrinsicExtensionBinding 

    /// Extensions to a type in a different assembly
    | ExtrinsicExtensionBinding 

    | ClassLetBinding of isStatic: bool

    | ObjectExpressionOverrideBinding

    | ExpressionBinding 

    static member IsModuleOrMemberOrExtensionBinding: DeclKind -> bool

    static member MustHaveArity: DeclKind -> bool

    member CanBeDllImport: bool

    static member IsAccessModifierPermitted: DeclKind -> bool

    static member ImplicitlyStatic: DeclKind -> bool

    static member AllowedAttribTargets: MemberFlags option -> DeclKind -> AttributeTargets

    // Note: now always true
    static member CanGeneralizeConstrainedTypars: DeclKind -> bool
        
    static member ConvertToLinearBindings: DeclKind -> bool

    static member CanOverrideOrImplement: DeclKind -> OverridesOK

type ImplicitlyBoundTyparsAllowed = 
    | NewTyparsOKButWarnIfNotRigid 
    | NewTyparsOK 
    | NoNewTypars

type CheckConstraints = 
    | CheckCxs 
    | NoCheckCxs

type IsObjExprBinding = 
    | ObjExprBinding 
    | ValOrMemberBinding

type RecDefnBindingInfo =
    | RecDefnBindingInfo of
        ContainerInfo *
        NewSlotsOK *
        DeclKind *
        SynBinding

/// The ValReprInfo for a value, except the number of typars is not yet inferred 
type PartialValReprInfo = PartialValReprInfo of ArgReprInfo list list * ArgReprInfo 

type ValMemberInfoTransient =
    | ValMemberInfoTransient of
        memberInfo: ValMemberInfo *
        logicalName: string *
        compiledName: string 

type ValSpecResult =
    | ValSpecResult of
        ParentRef *
        ValMemberInfoTransient option
        * Ident
        * Typars
        * Typars
        * TType
        * PartialValReprInfo
        * DeclKind 

type SyntacticUnscopedTyparEnv = UnscopedTyparEnv of NameMap<Typar>

/// A type to represent information associated with values to indicate what explicit (declared) type parameters
/// are given and what additional type parameters can be inferred, if any.
///
/// The declared type parameters, e.g. let f<'a> (x:'a) = x, plus an indication 
/// of whether additional polymorphism may be inferred, e.g. let f<'a, ..> (x:'a) y = x 
type ExplicitTyparInfo =
    | ExplicitTyparInfo of
        Typars *
        Typars *
        bool 

/// NormalizedBindingRhs records the r.h.s. of a binding after some munging just before type checking.
type NormalizedBindingRhs = 
    | NormalizedBindingRhs of
        SynSimplePats list *
        SynBindingReturnInfo option *
        SynExpr 

/// Represents a syntactic, unchecked binding after the resolution of the name resolution status of pattern
/// constructors and after "pushing" all complex patterns to the right hand side.
type NormalizedBinding = 
    | NormalizedBinding of 
        SynAccess option *
        SynBindingKind *
        mustInline: bool *
        isMutable: bool *
        SynAttribute list * 
        XmlDoc *
        SynValTyparDecls * 
        SynValData * 
        SynPat * 
        NormalizedBindingRhs *
        range *
        DebugPointForBinding

/// RecursiveBindingInfo - flows through initial steps of TcLetrec 
type RecursiveBindingInfo =
    | RBInfo of
        int * // index of the binding in the recursive group
        ContainerInfo * 
        Typars * 
        ValInline * 
        Val * 
        ExplicitTyparInfo * 
        PartialValReprInfo * 
        ValMemberInfoTransient option * 
        Val option * 
        Val option * 
        SafeInitData * 
        SynAccess option * 
        TType * 
        DeclKind
    member Val: Val
    member EnclosingDeclaredTypars: Typar list
    member Index: int

[<Sealed>]
type CheckedBindingInfo

type PreGeneralizationRecursiveBinding = 
    { ExtraGeneralizableTypars: Typars
      CheckedBinding: CheckedBindingInfo
      RecBindingInfo: RecursiveBindingInfo }

type PreCheckingRecursiveBinding = 
    { SyntacticBinding: NormalizedBinding 
      RecBindingInfo: RecursiveBindingInfo }

type NormalizedRecBindingDefn =
    | NormalizedRecBindingDefn of
        ContainerInfo *
        NewSlotsOK *
        DeclKind *
        NormalizedBinding

[<Sealed>]
type PrelimValScheme1 =
    member Ident: Ident
    member Type: TType

/// The results of applying arity inference to PrelimValScheme2 
type ValScheme = 
    | ValScheme of 
        Ident * 
        TypeScheme * 
        ValReprInfo option * 
        ValMemberInfoTransient option * 
        isMutable: bool *
        ValInline * 
        ValBaseOrThisInfo * 
        SynAccess option * 
        compgen: bool *
        isIncrClass: bool *
        isTyFunc: bool *
        hasDeclaredTypars: bool

[<Sealed>]
type RecursiveUseFixupPoints

type PreInitializationGraphEliminationBinding = 
    { FixupPoints: RecursiveUseFixupPoints
      Binding: Binding }

type PostGeneralizationRecursiveBinding = 
    { ValScheme: ValScheme
      CheckedBinding: CheckedBindingInfo
      RecBindingInfo: RecursiveBindingInfo }
    member GeneralizedTypars: Typar list

type PostBindCtorThisVarRefCellRecursiveBinding = 
    { ValScheme: ValScheme
      Binding: Binding }

val emptyUnscopedTyparEnv: SyntacticUnscopedTyparEnv

val addFreeItemOfModuleTy: ModuleOrNamespaceType -> UngeneralizableItem list -> UngeneralizableItem list

val unionGeneralizedTypars: typarSets:Typar list list -> Typar list    

val AddDeclaredTypars: check: CheckForDuplicateTyparFlag -> typars: Typar list -> env: TcEnv -> TcEnv

val AddLocalVal: NameResolution.TcResultsSink -> scopem: range -> v: Val -> TcEnv -> TcEnv

val AddLocalValPrimitive: v: Val -> TcEnv -> TcEnv

val AddLocalVals: tcSink: TcResultsSink -> scopem: range -> vals: Val list -> env: TcEnv -> TcEnv

val AdjustRecType: vspec: Val -> vscheme: ValScheme -> unit

val AnalyzeAndMakeAndPublishRecursiveValue: overridesOK:OverridesOK -> isGeneratedEventVal:bool -> cenv:TcFileState -> env:TcEnv -> tpenv:SyntacticUnscopedTyparEnv * recBindIdx:int -> NormalizedRecBindingDefn -> (PreCheckingRecursiveBinding list * Val list) * (SyntacticUnscopedTyparEnv * int)    

val CheckForNonAbstractInterface: declKind:DeclKind -> tcref:TyconRef -> memberFlags:MemberFlags -> m:range -> unit    

val CheckMemberFlags: optIntfSlotTy:'a option -> newslotsOK:NewSlotsOK -> overridesOK:OverridesOK -> memberFlags:MemberFlags -> m:range -> unit

val CheckSuperType: cenv:TcFileState -> ty:TType -> m:range -> unit    

val ChooseCanonicalDeclaredTyparsAfterInference: g: TcGlobals -> denv: DisplayEnv -> declaredTypars: Typar list -> m: range -> Typar list

val ChooseCanonicalValSchemeAfterInference: g: TcGlobals -> denv: DisplayEnv -> vscheme: ValScheme -> m: range -> ValScheme

val ComputeIsComplete: enclosingDeclaredTypars:Typar list -> declaredTypars:Typar list -> ty:TType -> bool    

val ComputeAccessRights: eAccessPath: CompilationPath -> eInternalsVisibleCompPaths: CompilationPath list -> eFamilyType: TyconRef option -> AccessorDomain

val ComputeAccessAndCompPath: env: TcEnv -> declKindOpt:DeclKind option -> m: range -> vis: SynAccess option -> overrideVis: Accessibility option -> actualParent: ParentRef -> Accessibility * CompilationPath option

val EliminateInitializationGraphs:
    g: TcGlobals 
    -> mustHaveArity: bool 
    -> denv: DisplayEnv 
    -> bindings: 'Binding list 
    -> iterBindings:((PreInitializationGraphEliminationBinding list -> unit) -> 'Binding list -> unit)
    -> buildLets: (Binding list -> 'Result)
    -> mapBindings: ((PreInitializationGraphEliminationBinding list -> Binding list) -> 'Binding list -> 'Result list)
    -> bindsm: range
    -> 'Result list

val FixupLetrecBind: cenv:TcFileState -> denv:DisplayEnv -> generalizedTyparsForRecursiveBlock:Typars -> bind:PostBindCtorThisVarRefCellRecursiveBinding -> PreInitializationGraphEliminationBinding    

val FreshenObjectArgType: cenv: TcFileState -> m: range -> rigid: TyparRigidity -> tcref: TyconRef -> isExtrinsic: bool -> declaredTyconTypars: Typar list -> TType * Typar list * TyparInst * TType * TType

val GetCurrAccumulatedModuleOrNamespaceType: env: TcEnv -> ModuleOrNamespaceType

val GetInstanceMemberThisVariable: vspec: Val * expr: Expr -> Val option

val InferGenericArityFromTyScheme: TypeScheme -> partialValReprInfo: PartialValReprInfo -> ValReprInfo

val LocateEnv: ccu: CcuThunk -> env: TcEnv -> enclosingNamespacePath: Ident list -> TcEnv

val MakeCheckSafeInit: g: TcGlobals -> tinst: TypeInst -> safeInitInfo: SafeInitData -> reqExpr: Expr -> expr: Expr -> Expr

val MakeAndPublishVal: cenv: TcFileState -> env: TcEnv -> altActualParent: ParentRef * inSig: bool * declKind: DeclKind * vrec: ValRecursiveScopeInfo * vscheme: ValScheme * attrs: Attribs * doc: XmlDoc * konst: Const option * isGeneratedEventVal: bool -> Val

val MakeAndPublishBaseVal: cenv: TcFileState -> env: TcEnv -> Ident option -> TType -> Val option

val MakeAndPublishSimpleVals: cenv: TcFileState -> env: TcEnv -> names: NameMap<PrelimValScheme1> -> NameMap<(Val * TypeScheme)> * NameMap<Val>

val MakeAndPublishSafeThisVal: cenv: TcFileState -> env: TcEnv -> thisIdOpt: Ident option -> thisTy: TType -> Val option

val MakeMemberDataAndMangledNameForMemberVal: g: TcGlobals * tcref: TyconRef * isExtrinsic: bool * attrs: Attribs * optImplSlotTys: TType list * memberFlags: MemberFlags * valSynData: SynValInfo * id: Ident * isCompGen: bool -> ValMemberInfoTransient

val MakeInnerEnvForTyconRef: env: TcEnv -> tcref: TyconRef -> isExtrinsicExtension: bool -> TcEnv

val MakeInnerEnvWithAcc: env: TcEnv -> nm: Ident -> mtypeAcc: ModuleOrNamespaceType ref -> modKind: ModuleOrNamespaceKind -> TcEnv

val MakeInnerEnv: env: TcEnv -> nm: Ident -> modKind: ModuleOrNamespaceKind -> TcEnv * ModuleOrNamespaceType ref

val NonGenericTypeScheme: ty: TType -> TypeScheme

val PublishModuleDefn: cenv: TcFileState -> env: TcEnv -> mspec: ModuleOrNamespace -> unit

val PublishTypeDefn: cenv: TcFileState -> env: TcEnv -> mspec: Tycon -> unit

val PublishValueDefn: cenv: TcFileState -> env: TcEnv -> declKind: DeclKind -> vspec: Val -> unit

/// Mark a typar as no longer being an inference type variable
val SetTyparRigid: DisplayEnv -> range -> Typar -> unit

val TcAndPublishValSpec: cenv: TcFileState * env: TcEnv * containerInfo: ContainerInfo * declKind: DeclKind * memFlagsOpt: MemberFlags option * tpenv: SyntacticUnscopedTyparEnv * valSpfn: SynValSig -> Val list * SyntacticUnscopedTyparEnv

val TcAttributes: cenv: TcFileState -> env: TcEnv -> attrTgt: AttributeTargets -> synAttribs: SynAttribute list -> Attrib list

val TcAttributesCanFail: cenv:TcFileState -> env:TcEnv -> attrTgt:AttributeTargets -> synAttribs:SynAttribute list -> Attrib list * (unit -> Attribs)    

val TcAttributesWithPossibleTargets: canFail: bool -> cenv: TcFileState -> env: TcEnv -> attrTgt: AttributeTargets -> synAttribs: SynAttribute list -> (AttributeTargets * Attrib) list * bool

val TcConst: cenv: TcFileState -> ty: TType -> m: range -> env: TcEnv -> c: SynConst -> Const

val TcLetBindings: cenv:TcFileState -> env:TcEnv -> containerInfo:ContainerInfo -> declKind:DeclKind -> tpenv:SyntacticUnscopedTyparEnv -> binds:SynBinding list * bindsm:range * scopem:range -> ModuleOrNamespaceExpr list * TcEnv * SyntacticUnscopedTyparEnv

val TcLetrecBinding: cenv:TcFileState * envRec:TcEnv * scopem:range * extraGeneralizableTypars:Typars * reqdThisValTyOpt:TType option -> envNonRec:TcEnv * generalizedRecBinds:PostGeneralizationRecursiveBinding list * preGeneralizationRecBinds:PreGeneralizationRecursiveBinding list * tpenv:SyntacticUnscopedTyparEnv * uncheckedRecBindsTable:Map<Stamp,PreCheckingRecursiveBinding> -> rbind:PreCheckingRecursiveBinding -> TcEnv * PostGeneralizationRecursiveBinding list * PreGeneralizationRecursiveBinding list * SyntacticUnscopedTyparEnv * Map<Stamp,PreCheckingRecursiveBinding>    

val TcLetrecComputeCtorSafeThisValBind: cenv:TcFileState -> safeThisValOpt:Val option -> Binding option    

val TcLetrec: overridesOK:OverridesOK -> cenv:TcFileState -> env:TcEnv -> tpenv:SyntacticUnscopedTyparEnv -> binds:RecDefnBindingInfo list * bindsm:range * scopem:range -> Bindings * TcEnv * SyntacticUnscopedTyparEnv    

val TcLetrecAdjustMemberForSpecialVals: cenv: TcFileState -> pgrbind: PostGeneralizationRecursiveBinding -> PostBindCtorThisVarRefCellRecursiveBinding

val TcNewExpr: cenv:TcFileState -> env:TcEnv -> tpenv:SyntacticUnscopedTyparEnv -> objTy:TType -> mObjTyOpt:range option -> superInit:bool -> arg:SynExpr -> mWholeExprOrObjTy:range -> Expr * SyntacticUnscopedTyparEnv    

#if !NO_EXTENSIONTYPING
val TcProvidedTypeAppToStaticConstantArgs: cenv:TcFileState -> env:TcEnv -> optGeneratedTypePath:string list option -> tpenv:SyntacticUnscopedTyparEnv -> tcref:TyconRef -> args:SynType list -> m:range -> bool * Tainted<ProvidedType> * (unit -> unit)
#endif

val TcSimplePatsOfUnknownType: cenv: TcFileState -> optArgsOK: bool -> checkCxs: CheckConstraints -> env: TcEnv -> tpenv: SyntacticUnscopedTyparEnv -> spats: SynSimplePats -> string list * (SyntacticUnscopedTyparEnv * NameMap<PrelimValScheme1> * Set<string>)

val TcTyparConstraints: cenv: TcFileState -> newOk: ImplicitlyBoundTyparsAllowed -> checkCxs: CheckConstraints -> occ: ItemOccurence -> env: TcEnv -> tpenv: SyntacticUnscopedTyparEnv -> synConstraints: SynTypeConstraint list -> SyntacticUnscopedTyparEnv

val TcTyparDecls: cenv: TcFileState -> env: TcEnv -> synTypars: SynTyparDecl list -> Typar list

val TcType: cenv: TcFileState -> newOk: ImplicitlyBoundTyparsAllowed -> checkCxs: CheckConstraints -> occ: ItemOccurence -> env: TcEnv -> tpenv: SyntacticUnscopedTyparEnv -> ty: SynType -> TType * SyntacticUnscopedTyparEnv

val TcTypeOrMeasureAndRecover: optKind: TyparKind option -> cenv: TcFileState -> newOk: ImplicitlyBoundTyparsAllowed -> checkCxs: CheckConstraints -> occ: ItemOccurence -> env: TcEnv -> tpenv: SyntacticUnscopedTyparEnv -> ty: SynType -> TType * SyntacticUnscopedTyparEnv

val TcTypeAndRecover: cenv: TcFileState -> newOk: ImplicitlyBoundTyparsAllowed -> checkCxs: CheckConstraints -> occ: ItemOccurence -> env: TcEnv -> tpenv: SyntacticUnscopedTyparEnv -> ty: SynType -> TType * SyntacticUnscopedTyparEnv

val TcValSpec: cenv: TcFileState -> TcEnv -> DeclKind -> ImplicitlyBoundTyparsAllowed -> ContainerInfo -> MemberFlags option -> thisTyOpt: TType option -> SyntacticUnscopedTyparEnv -> SynValSig -> Attrib list -> ValSpecResult list * SyntacticUnscopedTyparEnv

val TranslateTopValSynInfo: range -> tcAttributes: (AttributeTargets -> SynAttribute list -> Attrib list) -> synValInfo: SynValInfo -> PartialValReprInfo

val TranslatePartialArity: tps: Typar list -> PartialValReprInfo -> ValReprInfo

module GeneralizationHelpers =
    val ComputeUngeneralizableTypars: env: TcEnv -> Zset<Typar>
    val ComputeUnabstractableTraitSolutions: env: TcEnv -> FreeLocals
    val ComputeUnabstractableTycons: env: TcEnv -> FreeTycons

// Logically extends System.AttributeTargets for F# constructs
module AttributeTargets =
    val FieldDecl: AttributeTargets
    val FieldDeclRestricted: AttributeTargets
    val UnionCaseDecl: AttributeTargets
    val TyconDecl: AttributeTargets
    val ExnDecl: AttributeTargets
    val ModuleDecl: AttributeTargets
    val Top: AttributeTargets

module BindingNormalization =
    val NormalizeBinding: isObjExprBinding: IsObjExprBinding -> cenv: TcFileState -> env: TcEnv -> binding: SynBinding -> NormalizedBinding

