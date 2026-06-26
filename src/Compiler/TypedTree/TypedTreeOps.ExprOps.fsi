// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

/// TypedTreeOps.ExprOps: address-of operations, expression folding, intrinsic call wrappers, and higher-level expression helpers.
namespace FSharp.Compiler.TypedTreeOps

open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.CompilerGlobalState
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.Syntax

[<AutoOpen>]
module internal AddressOps =

    /// An exception representing a warning for a defensive copy of an immutable struct
    exception DefensiveCopyWarning of string * range

    type Mutates =
        | AddressOfOp
        | DefinitelyMutates
        | PossiblyMutates
        | NeverMutates

    val isRecdOrStructTyconRefAssumedImmutable: TcGlobals -> TyconRef -> bool

    val isTyconRefReadOnly: TcGlobals -> range -> TyconRef -> bool

    val isRecdOrStructTyconRefReadOnly: TcGlobals -> range -> TyconRef -> bool

    val isRecdOrStructTyReadOnly: TcGlobals -> range -> TType -> bool

    val CanTakeAddressOf: TcGlobals -> range -> bool -> TType -> Mutates -> bool

    val CanTakeAddressOfImmutableVal: TcGlobals -> range -> ValRef -> Mutates -> bool

    val MustTakeAddressOfVal: TcGlobals -> ValRef -> bool

    val MustTakeAddressOfByrefGet: TcGlobals -> ValRef -> bool

    val CanTakeAddressOfByrefGet: TcGlobals -> ValRef -> Mutates -> bool

    val MustTakeAddressOfRecdFieldRef: RecdFieldRef -> bool

    val CanTakeAddressOfRecdFieldRef: TcGlobals -> range -> RecdFieldRef -> TypeInst -> Mutates -> bool

    val CanTakeAddressOfUnionFieldRef: TcGlobals -> range -> UnionCaseRef -> int -> TypeInst -> Mutates -> bool

    /// Helper to create an expression that dereferences an address.
    val mkDerefAddrExpr: mAddrGet: range -> expr: Expr -> mExpr: range -> exprTy: TType -> Expr

    /// Helper to take the address of an expression
    val mkExprAddrOfExprAux:
        TcGlobals ->
        bool ->
        bool ->
        Mutates ->
        Expr ->
        ValRef option ->
        range ->
            (Val * Expr) option * Expr * bool * bool

    /// Take the address of an expression, or force it into a mutable local. Any allocated
    /// mutable local may need to be kept alive over a larger expression, hence we return
    /// a wrapping function that wraps "let mutable loc = Expr in ..." around a larger
    /// expression.
    val mkExprAddrOfExpr:
        TcGlobals -> bool -> bool -> Mutates -> Expr -> ValRef option -> range -> (Expr -> Expr) * Expr * bool * bool

    /// Make an expression that gets an item from a tuple
    val mkTupleFieldGet: TcGlobals -> TupInfo * Expr * TypeInst * int * range -> Expr

    /// Make an expression that gets an item from an anonymous record
    val mkAnonRecdFieldGet: TcGlobals -> AnonRecdTypeInfo * Expr * TypeInst * int * range -> Expr

    /// Build an expression representing the read of an instance class or record field.
    /// First take the address of the record expression if it is a struct.
    val mkRecdFieldGet: TcGlobals -> Expr * RecdFieldRef * TypeInst * range -> Expr

    /// Like mkUnionCaseFieldGetUnprovenViaExprAddr, but for struct-unions, the input should be a copy of the expression.
    val mkUnionCaseFieldGetUnproven: TcGlobals -> Expr * UnionCaseRef * TypeInst * int * range -> Expr

[<AutoOpen>]
module internal ExprFolding =

    ///  Work out what things on the right-hand-side of a 'let rec' recursive binding need to be fixed up
    val IterateRecursiveFixups:
        TcGlobals ->
        Val option ->
        (Val option -> Expr -> (Expr -> Expr) -> Expr -> unit) ->
        Expr * (Expr -> Expr) ->
            Expr ->
                unit

    /// Combine two static-resolution requirements on a type parameter
    val JoinTyparStaticReq: TyparStaticReq -> TyparStaticReq -> TyparStaticReq

    /// A set of function parameters (visitor) for folding over expressions
    type ExprFolder<'State> =
        { exprIntercept: ('State -> Expr -> 'State) -> ('State -> Expr -> 'State) -> 'State -> Expr -> 'State
          valBindingSiteIntercept: 'State -> bool * Val -> 'State
          nonRecBindingsIntercept: 'State -> Binding -> 'State
          recBindingsIntercept: 'State -> Bindings -> 'State
          dtreeIntercept: 'State -> DecisionTree -> 'State
          targetIntercept: ('State -> Expr -> 'State) -> 'State -> DecisionTreeTarget -> 'State option
          tmethodIntercept: ('State -> Expr -> 'State) -> 'State -> ObjExprMethod -> 'State option }

    /// The empty set of actions for folding over expressions
    val ExprFolder0: ExprFolder<'State>

    /// Fold over all the expressions in an implementation file
    val FoldImplFile: ExprFolder<'State> -> 'State -> CheckedImplFile -> 'State

    /// Fold over all the expressions in an expression
    val FoldExpr: ExprFolder<'State> -> 'State -> Expr -> 'State

#if DEBUG
    /// Extract some statistics from an expression
    val ExprStats: Expr -> string
#endif

[<AutoOpen>]
module internal Makers =

    val mkString: TcGlobals -> range -> string -> Expr

    val mkByte: TcGlobals -> range -> byte -> Expr

    val mkUInt16: TcGlobals -> range -> uint16 -> Expr

    val mkUnit: TcGlobals -> range -> Expr

    val mkInt32: TcGlobals -> range -> int32 -> Expr

    val mkInt: TcGlobals -> range -> int -> Expr

    val mkZero: TcGlobals -> range -> Expr

    val mkOne: TcGlobals -> range -> Expr

    val mkTwo: TcGlobals -> range -> Expr

    val mkMinusOne: TcGlobals -> range -> Expr

    /// Makes an expression holding a constant 0 value of the given numeric type.
    val mkTypedZero: g: TcGlobals -> m: range -> ty: TType -> Expr

    /// Makes an expression holding a constant 1 value of the given numeric type.
    val mkTypedOne: g: TcGlobals -> m: range -> ty: TType -> Expr

    val mkRefCellContentsRef: TcGlobals -> RecdFieldRef

    val mkSequential: range -> Expr -> Expr -> Expr

    val mkThenDoSequential: range -> expr: Expr -> stmt: Expr -> Expr

    val mkCompGenSequential: range -> stmt: Expr -> expr: Expr -> Expr

    val mkCompGenThenDoSequential: range -> expr: Expr -> stmt: Expr -> Expr

    val mkSequentials: TcGlobals -> range -> Exprs -> Expr

    val mkGetArg0: range -> TType -> Expr

    val mkAnyTupled: TcGlobals -> range -> TupInfo -> Exprs -> TType list -> Expr

    val mkRefTupled: TcGlobals -> range -> Exprs -> TType list -> Expr

    val mkRefTupledNoTypes: TcGlobals -> range -> Exprs -> Expr

    val mkRefTupledVars: TcGlobals -> range -> Val list -> Expr

    val mkRecordExpr:
        TcGlobals -> RecordConstructionInfo * TyconRef * TypeInst * RecdFieldRef list * Exprs * range -> Expr

    val mkAnonRecd: TcGlobals -> range -> AnonRecdTypeInfo -> Ident[] -> Exprs -> TType list -> Expr

    val mkRefCell: TcGlobals -> range -> TType -> Expr -> Expr

    val mkRefCellGet: TcGlobals -> range -> TType -> Expr -> Expr

    val mkRefCellSet: TcGlobals -> range -> TType -> Expr -> Expr -> Expr

    val mkNil: TcGlobals -> range -> TType -> Expr

    val mkCons: TcGlobals -> TType -> Expr -> Expr -> Expr

    val mkArray: TType * Exprs * range -> Expr

    val mkCompGenLocalAndInvisibleBind: TcGlobals -> string -> range -> Expr -> Val * Expr * Binding

    val mkUnbox: TType -> Expr -> range -> Expr

    val mkBox: TType -> Expr -> range -> Expr

    val mkIsInst: TType -> Expr -> range -> Expr

    val mspec_Type_GetTypeFromHandle: TcGlobals -> ILMethodSpec

    val fspec_Missing_Value: TcGlobals -> ILFieldSpec

    val mkInitializeArrayMethSpec: TcGlobals -> ILMethodSpec

    val mkInvalidCastExnNewobj: TcGlobals -> ILInstr

    val mkCallNewFormat:
        TcGlobals -> range -> TType -> TType -> TType -> TType -> TType -> formatStringExpr: Expr -> Expr

    val mkCallGetGenericComparer: TcGlobals -> range -> Expr

    val mkCallGetGenericEREqualityComparer: TcGlobals -> range -> Expr

    val mkCallGetGenericPEREqualityComparer: TcGlobals -> range -> Expr

    val mkCallUnbox: TcGlobals -> range -> TType -> Expr -> Expr

    val mkCallUnboxFast: TcGlobals -> range -> TType -> Expr -> Expr

    val mkCallTypeTest: TcGlobals -> range -> TType -> Expr -> Expr

    val mkCallTypeOf: TcGlobals -> range -> TType -> Expr

    val mkCallTypeDefOf: TcGlobals -> range -> TType -> Expr

    val mkCallDispose: TcGlobals -> range -> TType -> Expr -> Expr

    val mkCallSeq: TcGlobals -> range -> TType -> Expr -> Expr

    val mkCallCreateInstance: TcGlobals -> range -> TType -> Expr

    val mkCallGetQuerySourceAsEnumerable: TcGlobals -> range -> TType -> TType -> Expr -> Expr

    val mkCallNewQuerySource: TcGlobals -> range -> TType -> TType -> Expr -> Expr

    val mkCallCreateEvent: TcGlobals -> range -> TType -> TType -> Expr -> Expr -> Expr -> Expr

    val mkCallGenericComparisonWithComparerOuter: TcGlobals -> range -> TType -> Expr -> Expr -> Expr -> Expr

    val mkCallGenericEqualityEROuter: TcGlobals -> range -> TType -> Expr -> Expr -> Expr

    val mkCallGenericEqualityWithComparerOuter: TcGlobals -> range -> TType -> Expr -> Expr -> Expr -> Expr

    val mkCallGenericHashWithComparerOuter: TcGlobals -> range -> TType -> Expr -> Expr -> Expr

    val mkCallEqualsOperator: TcGlobals -> range -> TType -> Expr -> Expr -> Expr

    val mkCallNotEqualsOperator: TcGlobals -> range -> TType -> Expr -> Expr -> Expr

    val mkCallLessThanOperator: TcGlobals -> range -> TType -> Expr -> Expr -> Expr

    val mkCallLessThanOrEqualsOperator: TcGlobals -> range -> TType -> Expr -> Expr -> Expr

    val mkCallGreaterThanOperator: TcGlobals -> range -> TType -> Expr -> Expr -> Expr

    val mkCallGreaterThanOrEqualsOperator: TcGlobals -> range -> TType -> Expr -> Expr -> Expr

    val mkCallAdditionOperator: TcGlobals -> range -> TType -> Expr -> Expr -> Expr

    val mkCallSubtractionOperator: TcGlobals -> range -> TType -> Expr -> Expr -> Expr

    val mkCallMultiplyOperator: TcGlobals -> range -> ty1: TType -> ty2: TType -> retTy: TType -> Expr -> Expr -> Expr

    val mkCallDivisionOperator: TcGlobals -> range -> ty1: TType -> ty2: TType -> retTy: TType -> Expr -> Expr -> Expr

    val mkCallModulusOperator: TcGlobals -> range -> TType -> Expr -> Expr -> Expr

    val mkCallDefaultOf: TcGlobals -> range -> TType -> Expr

    val mkCallBitwiseAndOperator: TcGlobals -> range -> TType -> Expr -> Expr -> Expr

    val mkCallBitwiseOrOperator: TcGlobals -> range -> TType -> Expr -> Expr -> Expr

    val mkCallBitwiseXorOperator: TcGlobals -> range -> TType -> Expr -> Expr -> Expr

    val mkCallShiftLeftOperator: TcGlobals -> range -> TType -> Expr -> Expr -> Expr

    val mkCallShiftRightOperator: TcGlobals -> range -> TType -> Expr -> Expr -> Expr

    val mkCallUnaryNegOperator: TcGlobals -> range -> TType -> Expr -> Expr

    val mkCallUnaryNotOperator: TcGlobals -> range -> TType -> Expr -> Expr

    val mkCallAdditionChecked: TcGlobals -> range -> TType -> Expr -> Expr -> Expr

    val mkCallSubtractionChecked: TcGlobals -> range -> TType -> Expr -> Expr -> Expr

    val mkCallMultiplyChecked: TcGlobals -> range -> ty1: TType -> ty2: TType -> retTy: TType -> Expr -> Expr -> Expr

    val mkCallUnaryNegChecked: TcGlobals -> range -> TType -> Expr -> Expr

    val mkCallToByteChecked: TcGlobals -> range -> TType -> Expr -> Expr

    val mkCallToSByteChecked: TcGlobals -> range -> TType -> Expr -> Expr

    val mkCallToInt16Checked: TcGlobals -> range -> TType -> Expr -> Expr

    val mkCallToUInt16Checked: TcGlobals -> range -> TType -> Expr -> Expr

    val mkCallToIntChecked: TcGlobals -> range -> TType -> Expr -> Expr

    val mkCallToInt32Checked: TcGlobals -> range -> TType -> Expr -> Expr

    val mkCallToUInt32Checked: TcGlobals -> range -> TType -> Expr -> Expr

    val mkCallToInt64Checked: TcGlobals -> range -> TType -> Expr -> Expr

    val mkCallToUInt64Checked: TcGlobals -> range -> TType -> Expr -> Expr

    val mkCallToIntPtrChecked: TcGlobals -> range -> TType -> Expr -> Expr

    val mkCallToUIntPtrChecked: TcGlobals -> range -> TType -> Expr -> Expr

    val mkCallToByteOperator: TcGlobals -> range -> TType -> Expr -> Expr

    val mkCallToSByteOperator: TcGlobals -> range -> TType -> Expr -> Expr

    val mkCallToInt16Operator: TcGlobals -> range -> TType -> Expr -> Expr

    val mkCallToUInt16Operator: TcGlobals -> range -> TType -> Expr -> Expr

    val mkCallToInt32Operator: TcGlobals -> range -> TType -> Expr -> Expr

    val mkCallToUInt32Operator: TcGlobals -> range -> TType -> Expr -> Expr

    val mkCallToInt64Operator: TcGlobals -> range -> TType -> Expr -> Expr

    val mkCallToUInt64Operator: TcGlobals -> range -> TType -> Expr -> Expr

    val mkCallToSingleOperator: TcGlobals -> range -> TType -> Expr -> Expr

    val mkCallToDoubleOperator: TcGlobals -> range -> TType -> Expr -> Expr

    val mkCallToIntPtrOperator: TcGlobals -> range -> TType -> Expr -> Expr

    val mkCallToUIntPtrOperator: TcGlobals -> range -> TType -> Expr -> Expr

    val mkCallToCharOperator: TcGlobals -> range -> TType -> Expr -> Expr

    val mkCallToEnumOperator: TcGlobals -> range -> TType -> Expr -> Expr

    val mkCallArrayLength: TcGlobals -> range -> TType -> Expr -> Expr

    val mkCallArrayGet: TcGlobals -> range -> TType -> Expr -> Expr -> Expr

    val mkCallArray2DGet: TcGlobals -> range -> TType -> Expr -> Expr -> Expr -> Expr

    val mkCallArray3DGet: TcGlobals -> range -> TType -> Expr -> Expr -> Expr -> Expr -> Expr

    val mkCallArray4DGet: TcGlobals -> range -> TType -> Expr -> Expr -> Expr -> Expr -> Expr -> Expr

    val mkCallArraySet: TcGlobals -> range -> TType -> Expr -> Expr -> Expr -> Expr

    val mkCallArray2DSet: TcGlobals -> range -> TType -> Expr -> Expr -> Expr -> Expr -> Expr

    val mkCallArray3DSet: TcGlobals -> range -> TType -> Expr -> Expr -> Expr -> Expr -> Expr -> Expr

    val mkCallArray4DSet: TcGlobals -> range -> TType -> Expr -> Expr -> Expr -> Expr -> Expr -> Expr -> Expr

    val mkCallHash: TcGlobals -> range -> TType -> Expr -> Expr

    val mkCallBox: TcGlobals -> range -> TType -> Expr -> Expr

    val mkCallIsNull: TcGlobals -> range -> TType -> Expr -> Expr

    val mkCallRaise: TcGlobals -> range -> TType -> Expr -> Expr

    val mkCallNewDecimal: TcGlobals -> range -> Expr * Expr * Expr * Expr * Expr -> Expr

    val tryMkCallBuiltInWitness: TcGlobals -> TraitConstraintInfo -> Expr list -> range -> Expr option

    val tryMkCallCoreFunctionAsBuiltInWitness:
        TcGlobals -> IntrinsicValRef -> TType list -> Expr list -> range -> Expr option

    val TryEliminateDesugaredConstants: TcGlobals -> range -> Const -> Expr option

    val mkCallSeqCollect: TcGlobals -> range -> TType -> TType -> Expr -> Expr -> Expr

    val mkCallSeqUsing: TcGlobals -> range -> TType -> TType -> Expr -> Expr -> Expr

    val mkCallSeqDelay: TcGlobals -> range -> TType -> Expr -> Expr

    val mkCallSeqAppend: TcGlobals -> range -> TType -> Expr -> Expr -> Expr

    val mkCallSeqGenerated: TcGlobals -> range -> TType -> Expr -> Expr -> Expr

    val mkCallSeqFinally: TcGlobals -> range -> TType -> Expr -> Expr -> Expr

    val mkCallSeqTryWith: TcGlobals -> range -> TType -> Expr -> Expr -> Expr -> Expr

    val mkCallSeqOfFunctions: TcGlobals -> range -> TType -> TType -> Expr -> Expr -> Expr -> Expr

    val mkCallSeqToArray: TcGlobals -> range -> TType -> Expr -> Expr

    val mkCallSeqToList: TcGlobals -> range -> TType -> Expr -> Expr

    val mkCallSeqMap: TcGlobals -> range -> TType -> TType -> Expr -> Expr -> Expr

    val mkCallSeqSingleton: TcGlobals -> range -> TType -> Expr -> Expr

    val mkCallSeqEmpty: TcGlobals -> range -> TType -> Expr

    /// Make a call to the 'isprintf' function for string interpolation
    val mkCall_sprintf: g: TcGlobals -> m: range -> funcTy: TType -> fmtExpr: Expr -> fillExprs: Expr list -> Expr

    val mkCallDeserializeQuotationFSharp20Plus: TcGlobals -> range -> Expr -> Expr -> Expr -> Expr -> Expr

    val mkCallDeserializeQuotationFSharp40Plus: TcGlobals -> range -> Expr -> Expr -> Expr -> Expr -> Expr -> Expr

    val mkCallCastQuotation: TcGlobals -> range -> TType -> Expr -> Expr

    val mkCallLiftValue: TcGlobals -> range -> TType -> Expr -> Expr

    val mkCallLiftValueWithName: TcGlobals -> range -> TType -> string -> Expr -> Expr

    val mkCallLiftValueWithDefn: TcGlobals -> range -> TType -> Expr -> Expr

    val mkCallCheckThis: TcGlobals -> range -> TType -> Expr -> Expr

    val mkCallFailInit: TcGlobals -> range -> Expr

    val mkCallFailStaticInit: TcGlobals -> range -> Expr

    val mkCallQuoteToLinqLambdaExpression: TcGlobals -> range -> TType -> Expr -> Expr

    val mkOptionToNullable: TcGlobals -> range -> TType -> Expr -> Expr

    val mkOptionDefaultValue: TcGlobals -> range -> TType -> Expr -> Expr -> Expr

    val mkLazyDelayed: TcGlobals -> range -> TType -> Expr -> Expr

    val mkLazyForce: TcGlobals -> range -> TType -> Expr -> Expr

    val mkGetString: TcGlobals -> range -> Expr -> Expr -> Expr

    val mkGetStringChar: (TcGlobals -> range -> Expr -> Expr -> Expr)

    val mkGetStringLength: TcGlobals -> range -> Expr -> Expr

    val mkStaticCall_String_Concat2: TcGlobals -> range -> Expr -> Expr -> Expr

    val mkStaticCall_String_Concat3: TcGlobals -> range -> Expr -> Expr -> Expr -> Expr

    val mkStaticCall_String_Concat4: TcGlobals -> range -> Expr -> Expr -> Expr -> Expr -> Expr

    val mkStaticCall_String_Concat_Array: TcGlobals -> range -> Expr -> Expr

    val mkDecr: TcGlobals -> range -> Expr -> Expr

    val mkIncr: TcGlobals -> range -> Expr -> Expr

    val mkLdlen: TcGlobals -> range -> Expr -> Expr

    val mkLdelem: TcGlobals -> range -> TType -> Expr -> Expr -> Expr

    val mkILAsmCeq: TcGlobals -> range -> Expr -> Expr -> Expr

    val mkILAsmClt: TcGlobals -> range -> Expr -> Expr -> Expr

    val mkNull: range -> TType -> Expr

    val mkThrow: range -> TType -> Expr -> Expr

    val mkReraiseLibCall: TcGlobals -> TType -> range -> Expr

    val mkReraise: range -> TType -> Expr

    /// Add a label to use as the target for a goto
    val mkLabelled: range -> ILCodeLabel -> Expr -> Expr

    val mkNullTest: TcGlobals -> range -> Expr -> Expr -> Expr -> Expr

    val mkNonNullTest: TcGlobals -> range -> Expr -> Expr

    val mkNonNullCond: TcGlobals -> range -> TType -> Expr -> Expr -> Expr -> Expr

    /// Build an if-then statement
    val mkIfThen: TcGlobals -> range -> Expr -> Expr -> Expr

    /// Build the application of a (possibly generic, possibly curried) function value to a set of type and expression arguments
    val primMkApp: Expr * TType -> TypeInst -> Exprs -> range -> Expr

    /// Build the application of a (possibly generic, possibly curried) function value to a set of type and expression arguments.
    /// Reduce the application via let-bindings if the function value is a lambda expression.
    val mkApps: TcGlobals -> (Expr * TType) * TType list list * Exprs * range -> Expr

    val mkExprAppAux: TcGlobals -> Expr -> TType -> Exprs -> range -> Expr

    val mkAppsAux: TcGlobals -> Expr -> TType -> TType list list -> Exprs -> range -> Expr

    /// Build the application of a generic construct to a set of type arguments.
    /// Reduce the application via substitution if the function value is a typed lambda expression.
    val mkTyAppExpr: range -> Expr * TType -> TType list -> Expr

    val mkUnionCaseTest: TcGlobals -> Expr * UnionCaseRef * TypeInst * range -> Expr

[<AutoOpen>]
module internal ExprTransforms =

    /// Given a lambda expression taking multiple variables, build a corresponding lambda taking a tuple
    val MultiLambdaToTupledLambda: TcGlobals -> Val list -> Expr -> Val * Expr

    /// Given a lambda expression, adjust it to have be one or two lambda expressions (fun a -> (fun b -> ...))
    /// where the first has the given arguments.
    val AdjustArityOfLambdaBody: TcGlobals -> int -> Val list -> Expr -> Val list * Expr

    /// Make an application expression, doing beta reduction by introducing let-bindings
    /// if the function expression is a construction of a lambda
    val MakeApplicationAndBetaReduce: TcGlobals -> Expr * TType * TypeInst list * Exprs * range -> Expr

    /// Make a delegate invoke expression for an F# delegate type, doing beta reduction by introducing let-bindings
    /// if the delegate expression is a construction of a delegate.
    val MakeFSharpDelegateInvokeAndTryBetaReduce:
        TcGlobals ->
        delInvokeRef: Expr * delExpr: Expr * delInvokeTy: TType * tyargs: TypeInst * delInvokeArg: Expr * m: range ->
            Expr

    val MakeArgsForTopArgs: TcGlobals -> range -> (TType * ArgReprInfo) list list -> TyparInstantiation -> Val list list

    val AdjustValForExpectedValReprInfo: TcGlobals -> range -> ValRef -> ValUseFlag -> ValReprInfo -> Expr * TType

    val AdjustValToHaveValReprInfo: Val -> ParentRef -> ValReprInfo -> unit

    val stripTupledFunTy: TcGlobals -> TType -> TType list list * TType

    [<return: Struct>]
    val (|ExprValWithPossibleTypeInst|_|): Expr -> (ValRef * ValUseFlag * TypeInst * range) voption

    val mkCoerceIfNeeded: TcGlobals -> TType -> TType -> Expr -> Expr

    val mkCompGenLetIn: range -> string -> TType -> Expr -> (Val * Expr -> Expr) -> Expr

    val mkCompGenLetMutableIn: range -> string -> TType -> Expr -> (Val * Expr -> Expr) -> Expr

    val AdjustPossibleSubsumptionExpr: TcGlobals -> Expr -> Exprs -> (Expr * Exprs) option

    val NormalizeAndAdjustPossibleSubsumptionExprs: TcGlobals -> Expr -> Expr

    val LinearizeTopMatch: TcGlobals -> ParentRef -> Expr -> Expr

    val etaExpandTypeLambda: TcGlobals -> range -> Typars -> Expr * TType -> Expr

    [<return: Struct>]
    val (|NewDelegateExpr|_|): TcGlobals -> Expr -> (Unique * Val list * Expr * range * (Expr -> Expr)) voption

    [<return: Struct>]
    val (|DelegateInvokeExpr|_|): TcGlobals -> Expr -> (Expr * TType * TypeInst * Expr * Expr * range) voption

    [<return: Struct>]
    val (|OpPipeRight|_|): TcGlobals -> Expr -> (TType * Expr * Expr * range) voption

    [<return: Struct>]
    val (|OpPipeRight2|_|): TcGlobals -> Expr -> (TType * Expr * Expr * Expr * range) voption

    [<return: Struct>]
    val (|OpPipeRight3|_|): TcGlobals -> Expr -> (TType * Expr * Expr * Expr * Expr * range) voption

    /// Mutate a value to indicate it should be considered a local rather than a module-bound definition
    // REVIEW: this mutation should not be needed
    val ClearValReprInfo: Val -> Val

    val destInt32: Expr -> int32 option

    val destThrow: Expr -> (range * TType * Expr) option

    val isThrow: Expr -> bool

    val isIDelegateEventType: TcGlobals -> TType -> bool

    val destIDelegateEventType: TcGlobals -> TType -> TType
