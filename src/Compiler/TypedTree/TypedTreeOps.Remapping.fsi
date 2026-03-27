// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

/// TypedTreeOps.Remapping: signature operations, expression free variables, expression remapping, and expression shape queries.
namespace FSharp.Compiler.TypedTreeOps

open Internal.Utilities.Library
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics

[<AutoOpen>]
module internal SignatureOps =

    /// Wrap one module or namespace definition in a 'module M = ..' outer wrapper
    val wrapModuleOrNamespaceType: Ident -> CompilationPath -> ModuleOrNamespaceType -> ModuleOrNamespace

    /// Wrap one module or namespace definition in a 'namespace N' outer wrapper
    val wrapModuleOrNamespaceTypeInNamespace:
        Ident -> CompilationPath -> ModuleOrNamespaceType -> ModuleOrNamespaceType * ModuleOrNamespace

    /// Wrap one module or namespace implementation in a 'namespace N' outer wrapper
    val wrapModuleOrNamespaceContentsInNamespace:
        isModule: bool ->
        id: Ident ->
        cpath: CompilationPath ->
        mexpr: ModuleOrNamespaceContents ->
            ModuleOrNamespaceContents

    /// The remapping that corresponds to a module meeting its signature
    /// and also report the set of tycons, tycon representations and values hidden in the process.
    type SignatureRepackageInfo =
        {
            /// The list of corresponding values
            RepackagedVals: (ValRef * ValRef) list

            /// The list of corresponding modules, namespaces and type definitions
            RepackagedEntities: (TyconRef * TyconRef) list
        }

        /// The empty table
        static member Empty: SignatureRepackageInfo

    /// A set of tables summarizing the items hidden by a signature
    type SignatureHidingInfo =
        { HiddenTycons: Zset<Tycon>
          HiddenTyconReprs: Zset<Tycon>
          HiddenVals: Zset<Val>
          HiddenRecdFields: Zset<RecdFieldRef>
          HiddenUnionCases: Zset<UnionCaseRef> }

        /// The empty table representing no hiding
        static member Empty: SignatureHidingInfo

    /// Compute the remapping information implied by a signature being inferred for a particular implementation
    val ComputeRemappingFromImplementationToSignature:
        TcGlobals -> ModuleOrNamespaceContents -> ModuleOrNamespaceType -> SignatureRepackageInfo * SignatureHidingInfo

    /// Compute the remapping information implied by an explicit signature being given for an inferred signature
    val ComputeRemappingFromInferredSignatureToExplicitSignature:
        TcGlobals -> ModuleOrNamespaceType -> ModuleOrNamespaceType -> SignatureRepackageInfo * SignatureHidingInfo

    /// Compute the hiding information that corresponds to the hiding applied at an assembly boundary
    val ComputeSignatureHidingInfoAtAssemblyBoundary: ModuleOrNamespaceType -> SignatureHidingInfo -> SignatureHidingInfo

    /// Compute the hiding information that corresponds to the hiding applied at an assembly boundary
    val ComputeImplementationHidingInfoAtAssemblyBoundary:
        ModuleOrNamespaceContents -> SignatureHidingInfo -> SignatureHidingInfo

    val mkRepackageRemapping: SignatureRepackageInfo -> Remap

    /// Get the value including fsi remapping
    val DoRemapTycon: (Remap * SignatureHidingInfo) list -> Tycon -> Tycon

    /// Get the value including fsi remapping
    val DoRemapVal: (Remap * SignatureHidingInfo) list -> Val -> Val

    /// Determine if a type definition is hidden by a signature
    val IsHiddenTycon: (Remap * SignatureHidingInfo) list -> Tycon -> bool

    /// Determine if the representation of a type definition is hidden by a signature
    val IsHiddenTyconRepr: (Remap * SignatureHidingInfo) list -> Tycon -> bool

    /// Determine if a member, function or value is hidden by a signature
    val IsHiddenVal: (Remap * SignatureHidingInfo) list -> Val -> bool

    /// Determine if a record field is hidden by a signature
    val IsHiddenRecdField: (Remap * SignatureHidingInfo) list -> RecdFieldRef -> bool

    /// Fold over all the value and member definitions in a module or namespace type
    val foldModuleOrNamespaceTy: (Val -> 'T -> 'T) -> (Val -> 'T -> 'T) -> ModuleOrNamespaceType -> 'T -> 'T

    /// Collect all the values and member definitions in a module or namespace type
    val allValsOfModuleOrNamespaceTy: ModuleOrNamespaceType -> seq<Val>

    /// Collect all the entities in a module or namespace type
    val allEntitiesOfModuleOrNamespaceTy: ModuleOrNamespaceType -> seq<Entity>

    /// Check if a set of free type variables are all public
    val freeTyvarsAllPublic: FreeTyvars -> bool

    /// Check if a set of free variables are all public
    val freeVarsAllPublic: FreeVars -> bool

    [<return: Struct>]
    val (|LinearMatchExpr|_|):
        Expr -> (DebugPointAtBinding * range * DecisionTree * DecisionTreeTarget * Expr * range * TType) voption

    val rebuildLinearMatchExpr:
        DebugPointAtBinding * range * DecisionTree * DecisionTreeTarget * Expr * range * TType -> Expr

    [<return: Struct>]
    val (|LinearOpExpr|_|): Expr -> (TOp * TypeInst * Expr list * Expr * range) voption

    val rebuildLinearOpExpr: TOp * TypeInst * Expr list * Expr * range -> Expr

[<AutoOpen>]
module internal ExprFreeVars =

    val emptyFreeVars: FreeVars

    val unionFreeVars: FreeVars -> FreeVars -> FreeVars

    val accFreeInTargets: FreeVarOptions -> DecisionTreeTarget array -> FreeVars -> FreeVars

    val accFreeInExprs: FreeVarOptions -> Exprs -> FreeVars -> FreeVars

    val accFreeInSwitchCases: FreeVarOptions -> DecisionTreeCase list -> DecisionTree option -> FreeVars -> FreeVars

    val accFreeInDecisionTree: FreeVarOptions -> DecisionTree -> FreeVars -> FreeVars

    /// Get the free variables in a module definition.
    val freeInModuleOrNamespace: FreeVarOptions -> ModuleOrNamespaceContents -> FreeVars

    /// Get the free variables in an expression with accumulator
    val accFreeInExpr: FreeVarOptions -> Expr -> FreeVars -> FreeVars

    /// Get the free variables in an expression.
    val freeInExpr: FreeVarOptions -> Expr -> FreeVars

    /// Get the free variables in the right hand side of a binding.
    val freeInBindingRhs: FreeVarOptions -> Binding -> FreeVars

[<AutoOpen>]
module internal ExprRemapping =

    /// Given a (curried) lambda expression, pull off its arguments
    val stripTopLambda: Expr * TType -> Typars * Val list list * Expr * TType

    /// A flag to govern whether ValReprInfo inference should be type-directed or syntax-directed when
    /// inferring from a lambda expression.
    [<RequireQualifiedAccess>]
    type AllowTypeDirectedDetupling =
        | Yes
        | No

    /// Given a lambda expression, extract the ValReprInfo for its arguments and other details
    val InferValReprInfoOfExpr:
        TcGlobals -> AllowTypeDirectedDetupling -> TType -> Attribs list list -> Attribs -> Expr -> ValReprInfo

    /// Given a lambda binding, extract the ValReprInfo for its arguments and other details
    val InferValReprInfoOfBinding: TcGlobals -> AllowTypeDirectedDetupling -> Val -> Expr -> ValReprInfo

    /// Mutate a value to indicate it should be considered a local rather than a module-bound definition
    // REVIEW: this mutation should not be needed
    val ClearValReprInfo: Val -> Val

    /// Determine the underlying type of an enum type (normally int32)
    val underlyingTypeOfEnumTy: TcGlobals -> TType -> TType

    /// If the input type is an enum type, then convert to its underlying type, otherwise return the input type
    val normalizeEnumTy: TcGlobals -> TType -> TType

    //---------------------------------------------------------------------------
    // Resolve static optimizations
    //-------------------------------------------------------------------------

    type StaticOptimizationAnswer =
        | Yes = 1y
        | No = -1y
        | Unknown = 0y

    val DecideStaticOptimizations:
        TcGlobals -> StaticOptimization list -> canDecideTyparEqn: bool -> StaticOptimizationAnswer

    /// Indicate what should happen to value definitions when copying expressions
    type ValCopyFlag =
        | CloneAll
        | CloneAllAndMarkExprValsAsCompilerGenerated

        /// OnlyCloneExprVals is a nasty setting to reuse the cloning logic in a mode where all
        /// Tycon and "module/member" Val objects keep their identity, but the Val objects for all Expr bindings
        /// are cloned. This is used to 'fixup' the TAST created by tlr.fs
        ///
        /// This is a fragile mode of use. It's not really clear why TLR needs to create a "bad" expression tree that
        /// reuses Val objects as multiple value bindings, and its been the cause of several subtle bugs.
        | OnlyCloneExprVals

    /// Remap an expression using the given remapping substitution
    val remapExpr: TcGlobals -> ValCopyFlag -> Remap -> Expr -> Expr

    /// Remap an attribute using the given remapping substitution
    val remapAttrib: TcGlobals -> Remap -> Attrib -> Attrib

    /// Remap a (possible generic) type using the given remapping substitution
    val remapPossibleForallTy: TcGlobals -> Remap -> TType -> TType

    /// Copy an entire module or namespace type using the given copying flags
    val copyModuleOrNamespaceType: TcGlobals -> ValCopyFlag -> ModuleOrNamespaceType -> ModuleOrNamespaceType

    /// Copy an entire expression using the given copying flags
    val copyExpr: TcGlobals -> ValCopyFlag -> Expr -> Expr

    /// Copy an entire implementation file using the given copying flags
    val copyImplFile: TcGlobals -> ValCopyFlag -> CheckedImplFile -> CheckedImplFile

    /// Instantiate the generic type parameters in an expression, building a new one
    val instExpr: TcGlobals -> TyparInstantiation -> Expr -> Expr

    val allValsOfModDef: ModuleOrNamespaceContents -> seq<Val>

    val allTopLevelValsOfModDef: ModuleOrNamespaceContents -> seq<Val>

[<AutoOpen>]
module internal ExprShapeQueries =

    /// Adjust marks in expressions, replacing all marks by the given mark.
    /// Used when inlining.
    val remarkExpr: range -> Expr -> Expr

    val isRecdOrUnionOrStructTyconRefDefinitelyMutable: TyconRef -> bool

    //-------------------------------------------------------------------------
    // Primitives associated with quotations
    //-------------------------------------------------------------------------

    val isQuotedExprTy: TcGlobals -> TType -> bool

    val destQuotedExprTy: TcGlobals -> TType -> TType

    val mkQuotedExprTy: TcGlobals -> TType -> TType

    val mkAnyTupledTy: TcGlobals -> TupInfo -> TType list -> TType

    val mkRefTupledTy: TcGlobals -> TType list -> TType

    val mkMethodTy: TcGlobals -> TType list list -> TType -> TType

    /// Build a single-dimensional array type
    val mkArrayType: TcGlobals -> TType -> TType

    val GenWitnessArgTys: TcGlobals -> TraitWitnessInfo -> TType list list

    val GenWitnessTys: TcGlobals -> TraitWitnessInfos -> TType list

    val GenWitnessTy: TcGlobals -> TraitWitnessInfo -> TType

    /// Compute the type of an expression from the expression itself
    val tyOfExpr: TcGlobals -> Expr -> TType

    /// Build the application of a (possibly generic, possibly curried) function value to a set of type and expression arguments
    val primMkApp: Expr * TType -> TypeInst -> Exprs -> range -> Expr

    /// Build the application of a (possibly generic, possibly curried) function value to a set of type and expression arguments.
    /// Reduce the application via let-bindings if the function value is a lambda expression.
    val mkApps: TcGlobals -> (Expr * TType) * TType list list * Exprs * range -> Expr

    /// Build the application of a generic construct to a set of type arguments.
    /// Reduce the application via substitution if the function value is a typed lambda expression.
    val mkTyAppExpr: range -> Expr * TType -> TType list -> Expr

    ///  Accumulate the targets actually used in a decision graph (for reporting warnings)
    val accTargetsOfDecisionTree: DecisionTree -> int list -> int list

    /// Make a 'match' expression applying some peep-hole optimizations along the way, e.g to
    /// pre-decide the branch taken at compile-time.
    val mkAndSimplifyMatch:
        DebugPointAtBinding -> range -> range -> TType -> DecisionTree -> DecisionTreeTarget list -> Expr

