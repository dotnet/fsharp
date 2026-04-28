// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

/// Defines derived expression manipulation and construction functions.
namespace FSharp.Compiler.TypedTreeOps

open System.Collections.Immutable
open Internal.Utilities.Library
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.CompilerGlobalState
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Syntax
open FSharp.Compiler.Syntax.PrettyNaming
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics

[<AutoOpen>]
module internal XmlDocSignatures =

    /// XmlDoc signature helpers
    val commaEncs: string seq -> string

    val angleEnc: string -> string

    val ticksAndArgCountTextOfTyconRef: TyconRef -> string

    val typarEnc: TcGlobals -> Typars * Typars -> Typar -> string

    val buildAccessPath: CompilationPath option -> string

    val XmlDocArgsEnc: TcGlobals -> Typars * Typars -> TType list -> string

    val XmlDocSigOfVal: TcGlobals -> full: bool -> string -> Val -> string

    val XmlDocSigOfUnionCase: path: string list -> string

    val XmlDocSigOfField: path: string list -> string

    val XmlDocSigOfProperty: path: string list -> string

    val XmlDocSigOfTycon: path: string list -> string

    val XmlDocSigOfSubModul: path: string list -> string

    val XmlDocSigOfEntity: eref: EntityRef -> string

    type ActivePatternElemRef with

        member LogicalName: string

        member DisplayNameCore: string

        member DisplayName: string

    val TryGetActivePatternInfo: ValRef -> PrettyNaming.ActivePatternInfo option

    val mkChoiceCaseRef: g: TcGlobals -> m: range -> n: int -> i: int -> UnionCaseRef

    type PrettyNaming.ActivePatternInfo with

        /// Get the core of the display name for one of the cases of the active pattern, by index
        member DisplayNameCoreByIdx: idx: int -> string

        /// Get the display name for one of the cases of the active pattern, by index
        member DisplayNameByIdx: idx: int -> string

        /// Get the result type for the active pattern
        member ResultType: g: TcGlobals -> range -> TType list -> ActivePatternReturnKind -> TType

        /// Get the overall type for a function that implements the active pattern
        member OverallType:
            g: TcGlobals -> m: range -> argTy: TType -> retTys: TType list -> retKind: ActivePatternReturnKind -> TType

    val doesActivePatternHaveFreeTypars: TcGlobals -> ValRef -> bool

[<AutoOpen>]
module internal NullnessAnalysis =

    val nullnessOfTy: TcGlobals -> TType -> Nullness

    val changeWithNullReqTyToVariable: TcGlobals -> reqTy: TType -> TType

    val reqTyForArgumentNullnessInference: TcGlobals -> actualTy: TType -> reqTy: TType -> TType

    val IsNonNullableStructTyparTy: TcGlobals -> TType -> bool

    val inline HasConstraint: [<InlineIfLambda>] predicate: (TyparConstraint -> bool) -> Typar -> bool

    val inline IsTyparTyWithConstraint:
        TcGlobals -> [<InlineIfLambda>] predicate: (TyparConstraint -> bool) -> TType -> bool

    /// Determine if a type is a variable type with the ': not struct' constraint.
    ///
    /// Note, isRefTy does not include type parameters with the ': not struct' constraint
    /// This predicate is used to detect those type parameters.
    val IsReferenceTyparTy: TcGlobals -> TType -> bool

    val TypeNullIsTrueValue: TcGlobals -> TType -> bool

    val TypeNullIsExtraValue: TcGlobals -> range -> TType -> bool

    /// A type coming via interop from C# can be holding a nullness combination not supported in F#.
    /// Prime example are APIs marked as T|null applied to structs, tuples and anons.
    /// Unsupported values can also be nested within generic type arguments, e.g. a List<Tuple<string,T|null>> applied to an anon.
    val GetDisallowedNullness: TcGlobals -> TType -> TType list

    val TypeHasAllowNull: TyconRef -> TcGlobals -> range -> bool

    val TypeNullIsExtraValueNew: TcGlobals -> range -> TType -> bool

    val GetTyparTyIfSupportsNull: TcGlobals -> TType -> Typar voption

    val TypeNullNever: TcGlobals -> TType -> bool

    val TypeHasDefaultValue: TcGlobals -> range -> TType -> bool

    val TypeHasDefaultValueNew: TcGlobals -> range -> TType -> bool

    val (|TyparTy|NullableTypar|StructTy|NullTrueValue|NullableRefType|WithoutNullRefType|UnresolvedRefType|):
        TType * TcGlobals -> Choice<unit, unit, unit, unit, unit, unit, unit>

[<AutoOpen>]
module internal TypeTestsAndPatterns =

    /// Determine if a type is a ComInterop type
    val isComInteropTy: TcGlobals -> TType -> bool

    val mkIsInstConditional: TcGlobals -> range -> TType -> Expr -> Val -> Expr -> Expr -> Expr

    val canUseUnboxFast: TcGlobals -> range -> TType -> bool

    val canUseTypeTestFast: TcGlobals -> TType -> bool

    /// Determines types that are potentially known to satisfy the 'comparable' constraint and returns
    /// a set of residual types that must also satisfy the constraint
    [<return: Struct>]
    val (|SpecialComparableHeadType|_|): TcGlobals -> TType -> TType list voption

    [<return: Struct>]
    val (|SpecialEquatableHeadType|_|): TcGlobals -> TType -> TType list voption

    [<return: Struct>]
    val (|SpecialNotEquatableHeadType|_|): TcGlobals -> TType -> unit voption

    val GetMemberCallInfo: TcGlobals -> ValRef * ValUseFlag -> int * bool * bool * bool * bool * bool * bool * bool

[<AutoOpen>]
module internal Rewriting =

    type ExprRewritingEnv =
        { PreIntercept: ((Expr -> Expr) -> Expr -> Expr option) option
          PostTransform: Expr -> Expr option
          PreInterceptBinding: ((Expr -> Expr) -> Binding -> Binding option) option
          RewriteQuotations: bool
          StackGuard: StackGuard }

    val RewriteDecisionTree: ExprRewritingEnv -> DecisionTree -> DecisionTree

    val RewriteExpr: ExprRewritingEnv -> Expr -> Expr

    val RewriteImplFile: ExprRewritingEnv -> CheckedImplFile -> CheckedImplFile

    val IsGenericValWithGenericConstraints: TcGlobals -> Val -> bool

    type Entity with

        member HasInterface: TcGlobals -> TType -> bool

        member HasOverride: TcGlobals -> string -> TType list -> bool

        member HasMember: TcGlobals -> string -> TType list -> bool

        member internal TryGetMember: TcGlobals -> string -> TType list -> ValRef option

    type EntityRef with

        member HasInterface: TcGlobals -> TType -> bool

    /// Make a remapping table for viewing a module or namespace 'from the outside'
    val ApplyExportRemappingToEntity: TcGlobals -> Remap -> ModuleOrNamespace -> ModuleOrNamespace

[<AutoOpen>]
module internal TupleCompilation =

    val mkFastForLoop:
        TcGlobals -> DebugPointAtFor * DebugPointAtInOrTo * range * Val * Expr * bool * Expr * Expr -> Expr

    val mkCompiledTuple: TcGlobals -> bool -> TTypes * Exprs * range -> TyconRef * TTypes * Exprs * range

    /// Make a TAST expression representing getting an item from a tuple
    val mkGetTupleItemN: TcGlobals -> range -> int -> ILType -> bool -> Expr -> TType -> Expr

    [<RequireQualifiedAccess>]
    module IntegralConst =
        /// Constant 0.
        [<return: Struct>]
        val (|Zero|_|): c: Const -> unit voption

    /// An expression holding the loop's iteration count.
    type Count = Expr

    /// An expression representing the loop's current iteration index.
    type Idx = Expr

    /// An expression representing the current loop element.
    type Elem = Expr

    /// An expression representing the loop body.
    type Body = Expr

    /// An expression representing the overall loop.
    type Loop = Expr

    /// Makes an optimized while-loop for a range expression with the given integral start, step, and finish:
    ///
    /// start..step..finish
    ///
    /// The buildLoop function enables using the precomputed iteration count in an optional initialization step before the loop is executed.
    val mkOptimizedRangeLoop:
        g: TcGlobals ->
        mBody: range * mFor: range * mIn: range * spInWhile: DebugPointAtWhile ->
            rangeTy: TType * rangeExpr: Expr ->
                start: Expr * step: Expr * finish: Expr ->
                    buildLoop: (Count -> ((Idx -> Elem -> Body) -> Loop) -> Expr) ->
                        Expr

    type OptimizeForExpressionOptions =
        | OptimizeIntRangesOnly
        | OptimizeAllForExpressions

    val DetectAndOptimizeForEachExpression: TcGlobals -> OptimizeForExpressionOptions -> Expr -> Expr

    val BindUnitVars: TcGlobals -> Val list * ArgReprInfo list * Expr -> Val list * Expr

    val mkUnitDelayLambda: TcGlobals -> range -> Expr -> Expr

    /// Match expressions that are an application of a particular F# function value
    [<return: Struct>]
    val (|ValApp|_|): TcGlobals -> ValRef -> Expr -> (TypeInst * Exprs * range) voption

    val GetTypeOfIntrinsicMemberInCompiledForm:
        TcGlobals -> ValRef -> Typars * TraitWitnessInfos * CurriedArgInfos * TType option * ArgReprInfo

    /// Match an if...then...else expression or the result of "a && b" or "a || b"
    [<return: Struct>]
    val (|IfThenElseExpr|_|): expr: Expr -> (Expr * Expr * Expr) voption

    /// Match 'if __useResumableCode then ... else ...' expressions
    [<return: Struct>]
    val (|IfUseResumableStateMachinesExpr|_|): TcGlobals -> Expr -> (Expr * Expr) voption

[<AutoOpen>]
module internal ConstantEvaluation =

    val IsSimpleSyntacticConstantExpr: TcGlobals -> Expr -> bool

    [<return: Struct>]
    val (|ConstToILFieldInit|_|): Const -> ILFieldInit voption

    val EvalLiteralExprOrAttribArg: TcGlobals -> Expr -> Expr

    val EvaledAttribExprEquality: TcGlobals -> Expr -> Expr -> bool

    [<return: Struct>]
    val (|Int32Expr|_|): Expr -> int32 voption

    /// Matches if the given expression is an application
    /// of the range or range-step operator on an integral type
    /// and returns the type, start, step, and finish if so.
    ///
    /// start..finish
    ///
    /// start..step..finish
    [<return: Struct>]
    val (|IntegralRange|_|): g: TcGlobals -> expr: Expr -> (TType * (Expr * Expr * Expr)) voption

[<AutoOpen>]
module internal ResumableCodePatterns =

    /// Recognise a 'match __resumableEntry() with ...' expression
    [<return: Struct>]
    val (|ResumableEntryMatchExpr|_|): g: TcGlobals -> Expr -> (Expr * Val * Expr * (Expr * Expr -> Expr)) voption

    /// Recognise a '__stateMachine' expression
    [<return: Struct>]
    val (|StructStateMachineExpr|_|):
        g: TcGlobals -> expr: Expr -> (TType * (Val * Expr) * (Val * Val * Expr) * (Val * Expr)) voption

    /// Recognise a sequential or binding construct in a resumable code
    [<return: Struct>]
    val (|SequentialResumableCode|_|): g: TcGlobals -> Expr -> (Expr * Expr * range * (Expr -> Expr -> Expr)) voption

    /// Recognise a '__debugPoint' expression
    [<return: Struct>]
    val (|DebugPointExpr|_|): g: TcGlobals -> Expr -> string voption

    /// Recognise a '__resumeAt' expression
    [<return: Struct>]
    val (|ResumeAtExpr|_|): g: TcGlobals -> Expr -> Expr voption

    [<return: Struct>]
    val (|ResumableCodeInvoke|_|):
        g: TcGlobals -> expr: Expr -> (Expr * Expr * Expr list * range * (Expr * Expr list -> Expr)) voption

[<AutoOpen>]
module internal SeqExprPatterns =

    /// Detect the de-sugared form of a 'yield x' within a 'seq { ... }'
    [<return: Struct>]
    val (|SeqYield|_|): TcGlobals -> Expr -> (Expr * range) voption

    /// Detect the de-sugared form of a 'expr; expr' within a 'seq { ... }'
    [<return: Struct>]
    val (|SeqAppend|_|): TcGlobals -> Expr -> (Expr * Expr * range) voption

    /// Detect the de-sugared form of a 'while gd do expr' within a 'seq { ... }'
    [<return: Struct>]
    val (|SeqWhile|_|): TcGlobals -> Expr -> (Expr * Expr * DebugPointAtWhile * range) voption

    /// Detect the de-sugared form of a 'try .. finally .. ' within a 'seq { ... }'
    [<return: Struct>]
    val (|SeqTryFinally|_|): TcGlobals -> Expr -> (Expr * Expr * DebugPointAtTry * DebugPointAtFinally * range) voption

    /// Detect the de-sugared form of a 'use x = ..' within a 'seq { ... }'
    [<return: Struct>]
    val (|SeqUsing|_|): TcGlobals -> Expr -> (Expr * Val * Expr * TType * DebugPointAtBinding * range) voption

    /// Detect the de-sugared form of a 'for x in collection do ..' within a 'seq { ... }'
    [<return: Struct>]
    val (|SeqForEach|_|): TcGlobals -> Expr -> (Expr * Val * Expr * TType * range * range * DebugPointAtInOrTo) voption

    /// Detect the outer 'Seq.delay' added for a construct 'seq { ... }'
    [<return: Struct>]
    val (|SeqDelay|_|): TcGlobals -> Expr -> (Expr * TType) voption

    /// Detect a 'Seq.empty' implicit in the implied 'else' branch of an 'if .. then' in a seq { ... }
    [<return: Struct>]
    val (|SeqEmpty|_|): TcGlobals -> Expr -> range voption

    /// Detect a 'seq { ... }' expression
    [<return: Struct>]
    val (|Seq|_|): TcGlobals -> Expr -> (Expr * TType) voption
