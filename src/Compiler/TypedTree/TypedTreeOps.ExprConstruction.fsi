// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

namespace FSharp.Compiler.TypedTreeOps

open System.Collections.Generic
open System.Collections.Immutable
open Internal.Utilities.Collections
open Internal.Utilities.Library
open Internal.Utilities.Rational
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.CompilerGlobalState
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open FSharp.Compiler.Xml
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TcGlobals

[<AutoOpen>]
module internal ExprConstruction =

    /// An ordering for value definitions, based on stamp
    val valOrder: IComparer<Val>

    /// An ordering for type definitions, based on stamp
    val tyconOrder: IComparer<Tycon>

    val recdFieldRefOrder: IComparer<RecdFieldRef>

    val unionCaseRefOrder: IComparer<UnionCaseRef>

    val mkLambdaTy: TcGlobals -> Typars -> TTypes -> TType -> TType

    val mkLambdaArgTy: range -> TTypes -> TType

    /// Get the natural type of a single argument amongst a set of curried arguments
    val typeOfLambdaArg: range -> Val list -> TType

    /// Get the curried type corresponding to a lambda
    val mkMultiLambdaTy: TcGlobals -> range -> Val list -> TType -> TType

    /// Module publication, used while compiling fslib.
    val ensureCcuHasModuleOrNamespaceAtPath: CcuThunk -> Ident list -> CompilationPath -> XmlDoc -> unit

    /// Ignore 'Expr.Link' in an expression
    val stripExpr: Expr -> Expr

    /// Ignore 'Expr.Link' and 'Expr.DebugPoint' in an expression
    val stripDebugPoints: Expr -> Expr

    /// Match any 'Expr.Link' and 'Expr.DebugPoint' in an expression, providing the inner expression and a function to rebuild debug points
    val (|DebugPoints|): Expr -> Expr * (Expr -> Expr)

    val mkCase: DecisionTreeTest * DecisionTree -> DecisionTreeCase

    val isRefTupleExpr: Expr -> bool

    val tryDestRefTupleExpr: Expr -> Exprs

    val primMkMatch: DebugPointAtBinding * range * DecisionTree * DecisionTreeTarget array * range * TType -> Expr

    /// Build decision trees imperatively
    type MatchBuilder =

        /// Create a new builder
        new: DebugPointAtBinding * range -> MatchBuilder

        /// Add a new destination target
        member AddTarget: DecisionTreeTarget -> int

        /// Add a new destination target that is an expression result
        member AddResultTarget: Expr -> DecisionTree

        /// Finish the targets
        member CloseTargets: unit -> DecisionTreeTarget list

        /// Build the overall expression
        member Close: DecisionTree * range * TType -> Expr

    /// Add an if-then-else boolean conditional node into a decision tree
    val mkBoolSwitch: range -> Expr -> DecisionTree -> DecisionTree -> DecisionTree

    /// Build a conditional expression
    val primMkCond: DebugPointAtBinding -> range -> TType -> Expr -> Expr -> Expr -> Expr

    /// Build a conditional expression
    val mkCond: DebugPointAtBinding -> range -> TType -> Expr -> Expr -> Expr -> Expr

    /// Build an expression corresponding to the use of a reference to a value
    val exprForValRef: range -> ValRef -> Expr

    /// Build an expression corresponding to the use of a value
    /// Note: try to use exprForValRef or the expression returned from mkLocal instead of this.
    val exprForVal: range -> Val -> Expr

    val mkLocalAux: range -> string -> TType -> ValMutability -> bool -> Val * Expr

    /// Make a new local value and build an expression to reference it
    val mkLocal: range -> string -> TType -> Val * Expr

    /// Make a new compiler-generated local value and build an expression to reference it
    val mkCompGenLocal: range -> string -> TType -> Val * Expr

    /// Make a new mutable compiler-generated local value and build an expression to reference it
    val mkMutableCompGenLocal: range -> string -> TType -> Val * Expr

    /// Build a lambda expression taking multiple values
    val mkMultiLambda: range -> Val list -> Expr * TType -> Expr

    /// Rebuild a lambda during an expression tree traversal
    val rebuildLambda: range -> Val option -> Val option -> Val list -> Expr * TType -> Expr

    /// Build a lambda expression taking a single value
    val mkLambda: range -> Val -> Expr * TType -> Expr

    /// Build a generic lambda expression (type abstraction)
    val mkTypeLambda: range -> Typars -> Expr * TType -> Expr

    /// Build an type-chose expression, indicating that a local free choice of a type variable
    val mkTypeChoose: range -> Typars -> Expr -> Expr

    /// Build an object expression
    val mkObjExpr: TType * Val option * Expr * ObjExprMethod list * (TType * ObjExprMethod list) list * range -> Expr

    /// Build an iterated (curried) lambda expression
    val mkLambdas: TcGlobals -> range -> Typars -> Val list -> Expr * TType -> Expr

    /// Build an iterated (tupled+curried) lambda expression
    val mkMultiLambdasCore: TcGlobals -> range -> Val list list -> Expr * TType -> Expr * TType

    /// Build an iterated generic (type abstraction + tupled+curried) lambda expression
    val mkMultiLambdas: TcGlobals -> range -> Typars -> Val list list -> Expr * TType -> Expr

    /// Build a lambda expression that corresponds to the implementation of a member
    val mkMemberLambdas:
        TcGlobals -> range -> Typars -> Val option -> Val option -> Val list list -> Expr * TType -> Expr

    /// Make a binding that binds a function value to a lambda taking multiple arguments
    val mkMultiLambdaBind:
        TcGlobals -> Val -> DebugPointAtBinding -> range -> Typars -> Val list list -> Expr * TType -> Binding

    /// Build a user-level value binding
    val mkBind: DebugPointAtBinding -> Val -> Expr -> Binding

    /// Build a user-level let-binding
    val mkLetBind: range -> Binding -> Expr -> Expr

    /// Build a user-level value sequence of let bindings
    val mkLetsBind: range -> Binding list -> Expr -> Expr

    /// Build a user-level value sequence of let bindings
    val mkLetsFromBindings: range -> Bindings -> Expr -> Expr

    /// Build a user-level let expression
    val mkLet: DebugPointAtBinding -> range -> Val -> Expr -> Expr -> Expr

    // Compiler generated bindings may involve a user variable.
    // Compiler generated bindings may give rise to a sequence point if they are part of
    // an SPAlways expression. Compiler generated bindings can arise from for example, inlining.
    val mkCompGenBind: Val -> Expr -> Binding

    /// Make a set of bindings that bind compiler generated values to corresponding expressions.
    /// Compiler-generated bindings do not give rise to a sequence point in debugging.
    val mkCompGenBinds: Val list -> Exprs -> Bindings

    /// Make a let-expression that locally binds a compiler-generated value to an expression.
    /// Compiler-generated bindings do not give rise to a sequence point in debugging.
    val mkCompGenLet: range -> Val -> Expr -> Expr -> Expr

    /// Make a binding that binds a value to an expression in an "invisible" way.
    /// Invisible bindings are not given a sequence point and should not have side effects.
    val mkInvisibleBind: Val -> Expr -> Binding

    /// Make a set of bindings that bind values to expressions in an "invisible" way.
    /// Invisible bindings are not given a sequence point and should not have side effects.
    val mkInvisibleBinds: Vals -> Exprs -> Bindings

    /// Make a let-expression that locally binds a value to an expression in an "invisible" way.
    /// Invisible bindings are not given a sequence point and should not have side effects.
    val mkInvisibleLet: range -> Val -> Expr -> Expr -> Expr

    val mkInvisibleLets: range -> Vals -> Exprs -> Expr -> Expr

    val mkInvisibleLetsFromBindings: range -> Vals -> Exprs -> Expr -> Expr

    /// Make a let-rec expression that locally binds values to expressions where self-reference back to the values is possible.
    val mkLetRecBinds: range -> Bindings -> Expr -> Expr

    val NormalizeDeclaredTyparsForEquiRecursiveInference: TcGlobals -> Typars -> Typars

    /// GeneralizedType (generalizedTypars, tauTy)
    ///
    ///    generalizedTypars -- the truly generalized type parameters
    ///    tauTy  --  the body of the generalized type. A 'tau' type is one with its type parameters stripped off.
    type GeneralizedType = GeneralizedType of Typars * TType

    /// Make the right-hand side of a generalized binding, incorporating the generalized generic parameters from the type
    /// scheme into the right-hand side as type generalizations.
    val mkGenericBindRhs: TcGlobals -> range -> Typars -> GeneralizedType -> Expr -> Expr

    /// Test if the type parameter is one of those being generalized by a type scheme.
    val isBeingGeneralized: Typar -> GeneralizedType -> bool

    val mkBool: TcGlobals -> range -> bool -> Expr

    val mkTrue: TcGlobals -> range -> Expr

    val mkFalse: TcGlobals -> range -> Expr

    /// Make the expression corresponding to 'expr1 || expr2'
    val mkLazyOr: TcGlobals -> range -> Expr -> Expr -> Expr

    /// Make the expression corresponding to 'expr1 && expr2'
    val mkLazyAnd: TcGlobals -> range -> Expr -> Expr -> Expr

    val mkCoerceExpr: Expr * TType * range * TType -> Expr

    /// Make an expression that is IL assembly code
    val mkAsmExpr: ILInstr list * TypeInst * Exprs * TTypes * range -> Expr

    /// Make an expression that constructs a union case, e.g. 'Some(expr)'
    val mkUnionCaseExpr: UnionCaseRef * TypeInst * Exprs * range -> Expr

    /// Make an expression that constructs an exception value
    val mkExnExpr: TyconRef * Exprs * range -> Expr

    val mkTupleFieldGetViaExprAddr: TupInfo * Expr * TypeInst * int * range -> Expr

    /// Make an expression that gets an item from an anonymous record (via the address of the value if it is a struct)
    val mkAnonRecdFieldGetViaExprAddr: AnonRecdTypeInfo * Expr * TypeInst * int * range -> Expr

    /// Make an expression that gets an instance field from a record or class (via the address of the value if it is a struct)
    val mkRecdFieldGetViaExprAddr: Expr * RecdFieldRef * TypeInst * range -> Expr

    /// Make an expression that gets the address of an instance field from a record or class (via the address of the value if it is a struct)
    val mkRecdFieldGetAddrViaExprAddr: readonly: bool * Expr * RecdFieldRef * TypeInst * range -> Expr

    /// Make an expression that gets the address of a static field in a record or class
    val mkStaticRecdFieldGetAddr: readonly: bool * RecdFieldRef * TypeInst * range -> Expr

    /// Make an expression that gets a static field from a record or class
    val mkStaticRecdFieldGet: RecdFieldRef * TypeInst * range -> Expr

    /// Make an expression that sets a static field in a record or class
    val mkStaticRecdFieldSet: RecdFieldRef * TypeInst * Expr * range -> Expr

    /// Make an expression that gets the address of an element in an array
    val mkArrayElemAddress:
        TcGlobals -> readonly: bool * ILReadonly * bool * ILArrayShape * TType * Expr list * range -> Expr

    /// Make an expression that sets an instance the field of a record or class (via the address of the value if it is a struct)
    val mkRecdFieldSetViaExprAddr: Expr * RecdFieldRef * TypeInst * Expr * range -> Expr

    /// Make an expression that gets the tag of a union value (via the address of the value if it is a struct)
    val mkUnionCaseTagGetViaExprAddr: Expr * TyconRef * TypeInst * range -> Expr

    /// Make a 'TOp.UnionCaseProof' expression, which proves a union value is over a particular case (used only for ref-unions, not struct-unions)
    val mkUnionCaseProof: Expr * UnionCaseRef * TypeInst * range -> Expr

    /// Build a 'TOp.UnionCaseFieldGet' expression for something we've already determined to be a particular union case. For ref-unions,
    /// the input expression has 'TType_ucase', which is an F# compiler internal "type" corresponding to the union case. For struct-unions,
    /// the input should be the address of the expression.
    val mkUnionCaseFieldGetProvenViaExprAddr: Expr * UnionCaseRef * TypeInst * int * range -> Expr

    /// Build a 'TOp.UnionCaseFieldGetAddr' expression for a field of a union when we've already determined the value to be a particular union case. For ref-unions,
    /// the input expression has 'TType_ucase', which is an F# compiler internal "type" corresponding to the union case. For struct-unions,
    /// the input should be the address of the expression.
    val mkUnionCaseFieldGetAddrProvenViaExprAddr: readonly: bool * Expr * UnionCaseRef * TypeInst * int * range -> Expr

    /// Build a 'get' expression for something we've already determined to be a particular union case, but where
    /// the static type of the input is not yet proven to be that particular union case. This requires a type
    /// cast to 'prove' the condition.
    val mkUnionCaseFieldGetUnprovenViaExprAddr: Expr * UnionCaseRef * TypeInst * int * range -> Expr

    val mkUnionCaseFieldSet: Expr * UnionCaseRef * TypeInst * int * Expr * range -> Expr

    /// Make an expression that gets an instance field from an F# exception value
    val mkExnCaseFieldGet: Expr * TyconRef * int * range -> Expr

    /// Make an expression that sets an instance field in an F# exception value
    val mkExnCaseFieldSet: Expr * TyconRef * int * Expr * range -> Expr

    val mkDummyLambda: TcGlobals -> Expr * TType -> Expr

    /// Build a 'while' loop expression
    val mkWhile: TcGlobals -> DebugPointAtWhile * SpecialWhileLoopMarker * Expr * Expr * range -> Expr

    /// Build a 'for' loop expression
    val mkIntegerForLoop:
        TcGlobals -> DebugPointAtFor * DebugPointAtInOrTo * Val * Expr * ForLoopStyle * Expr * Expr * range -> Expr

    /// Build a 'try/with' expression
    val mkTryWith:
        TcGlobals ->
        Expr (* filter val *) *
        Val (* filter expr *) *
        Expr (* handler val *) *
        Val (* handler expr *) *
        Expr *
        range *
        TType *
        DebugPointAtTry *
        DebugPointAtWith ->
            Expr

    /// Build a 'try/finally' expression
    val mkTryFinally: TcGlobals -> Expr * Expr * range * TType * DebugPointAtTry * DebugPointAtFinally -> Expr

    val mkDefault: range * TType -> Expr

    /// Build an expression to mutate a local
    ///   localv <- e
    val mkValSet: range -> ValRef -> Expr -> Expr

    /// Build an expression to mutate the contents of a local pointer
    ///  *localv_ptr = e
    val mkAddrSet: range -> ValRef -> Expr -> Expr

    /// Build an expression to dereference a local pointer
    /// *localv_ptr
    val mkAddrGet: range -> ValRef -> Expr

    /// Build an expression to take the address of a local
    /// &localv
    val mkValAddr: range -> readonly: bool -> ValRef -> Expr

    val valOfBind: Binding -> Val

    /// Get the values for a set of bindings
    val valsOfBinds: Bindings -> Vals

    val mkDebugPoint: m: range -> expr: Expr -> Expr

    [<return: Struct>]
    val (|InnerExprPat|): Expr -> Expr

[<AutoOpen>]
module internal TypedTreeCollections =

    /// Mutable data structure mapping Val's to T based on stamp keys
    [<Sealed; NoEquality; NoComparison>]
    type ValHash<'T> =

        member Values: seq<'T>

        member TryFind: Val -> 'T option

        member Add: Val * 'T -> unit

        static member Create: unit -> ValHash<'T>

    /// Maps Val's to list of T based on stamp keys
    [<Struct; NoEquality; NoComparison>]
    type ValMultiMap<'T> =

        member ContainsKey: Val -> bool

        member Find: Val -> 'T list

        member Add: Val * 'T -> ValMultiMap<'T>

        member Remove: Val -> ValMultiMap<'T>

        member Contents: StampMap<'T list>

        static member Empty: ValMultiMap<'T>

    /// Maps TyconRef to list of T based on stamp keys
    [<Struct; NoEquality; NoComparison>]
    type TyconRefMultiMap<'T> =

        /// Fetch the entries for the given type definition
        member Find: TyconRef -> 'T list

        /// Make a new map, containing a new entry for the given type definition
        member Add: TyconRef * 'T -> TyconRefMultiMap<'T>

        /// The empty map
        static member Empty: TyconRefMultiMap<'T>

        /// Make a new map, containing a entries for the given type definitions
        static member OfList: (TyconRef * 'T) list -> TyconRefMultiMap<'T>

[<AutoOpen>]
module internal TypeTesters =

    /// Try to create a EntityRef suitable for accessing the given Entity from another assembly
    val tryRescopeEntity: CcuThunk -> Entity -> EntityRef voption

    /// Try to create a ValRef suitable for accessing the given Val from another assembly
    val tryRescopeVal: CcuThunk -> Remap -> Val -> ValRef voption

    val actualTyOfRecdField: TyparInstantiation -> RecdField -> TType

    val actualTysOfRecdFields: TyparInstantiation -> RecdField list -> TType list

    val actualTysOfInstanceRecdFields: TyparInstantiation -> TyconRef -> TType list

    val actualTysOfUnionCaseFields: TyparInstantiation -> UnionCaseRef -> TType list

    val actualResultTyOfUnionCase: TypeInst -> UnionCaseRef -> TType

    val recdFieldsOfExnDefRef: TyconRef -> RecdField list

    val recdFieldOfExnDefRefByIdx: TyconRef -> int -> RecdField

    val recdFieldTysOfExnDefRef: TyconRef -> TType list

    val recdFieldTyOfExnDefRefByIdx: TyconRef -> int -> TType

    val actualTyOfRecdFieldForTycon: Tycon -> TypeInst -> RecdField -> TType

    val actualTyOfRecdFieldRef: RecdFieldRef -> TypeInst -> TType

    val actualTyOfUnionFieldRef: UnionCaseRef -> int -> TypeInst -> TType

    val destForallTy: TcGlobals -> TType -> Typars * TType

    val tryDestForallTy: TcGlobals -> TType -> Typars * TType

    val stripFunTy: TcGlobals -> TType -> TType list * TType

    val applyForallTy: TcGlobals -> TType -> TypeInst -> TType

    val reduceIteratedFunTy: TcGlobals -> TType -> 'T list -> TType

    val applyTyArgs: TcGlobals -> TType -> TType list -> TType

    val applyTys: TcGlobals -> TType -> TType list * 'T list -> TType

    val formalApplyTys: TcGlobals -> TType -> 'a list * 'b list -> TType

    val stripFunTyN: TcGlobals -> int -> TType -> TType list * TType

    val tryDestAnyTupleTy: TcGlobals -> TType -> TupInfo * TType list

    val tryDestRefTupleTy: TcGlobals -> TType -> TType list

    type UncurriedArgInfos = (TType * ArgReprInfo) list

    type CurriedArgInfos = (TType * ArgReprInfo) list list

    type TraitWitnessInfos = TraitWitnessInfo list

    val GetTopTauTypeInFSharpForm: TcGlobals -> ArgReprInfo list list -> TType -> range -> CurriedArgInfos * TType

    val destTopForallTy: TcGlobals -> ValReprInfo -> TType -> Typars * TType

    val GetValReprTypeInFSharpForm:
        TcGlobals -> ValReprInfo -> TType -> range -> Typars * CurriedArgInfos * TType * ArgReprInfo

    val IsCompiledAsStaticProperty: TcGlobals -> Val -> bool

    val IsCompiledAsStaticPropertyWithField: TcGlobals -> Val -> bool

    /// Check if a type definition is one of the artificial type definitions used for array types of different ranks
    val isArrayTyconRef: TcGlobals -> TyconRef -> bool

    /// Determine the rank of one of the artificial type definitions used for array types
    val rankOfArrayTyconRef: TcGlobals -> TyconRef -> int

    /// Get the element type of an array type
    val destArrayTy: TcGlobals -> TType -> TType

    /// Get the element type of an F# list type
    val destListTy: TcGlobals -> TType -> TType

    val tyconRefEqOpt: TcGlobals -> TyconRef option -> TyconRef -> bool

    /// Determine if a type is the System.String type
    val isStringTy: TcGlobals -> TType -> bool

    /// Determine if a type is an F# list type
    val isListTy: TcGlobals -> TType -> bool

    /// Determine if a type is any kind of array type
    val isArrayTy: TcGlobals -> TType -> bool

    /// Determine if a type is a single-dimensional array type
    val isArray1DTy: TcGlobals -> TType -> bool

    /// Determine if a type is the F# unit type
    val isUnitTy: TcGlobals -> TType -> bool

    /// Determine if a type is the System.Object type with any nullness qualifier
    val isObjTyAnyNullness: TcGlobals -> TType -> bool

    /// Determine if a type is the (System.Object | null) type. Allows either nullness if null checking is disabled.
    val isObjNullTy: TcGlobals -> TType -> bool

    /// Determine if a type is a strictly non-nullable System.Object type. If nullness checking is disabled, this returns false.
    val isObjTyWithoutNull: TcGlobals -> TType -> bool

    /// Determine if a type is the System.ValueType type
    val isValueTypeTy: TcGlobals -> TType -> bool

    /// Determine if a type is the System.Void type
    val isVoidTy: TcGlobals -> TType -> bool

    /// Determine if a type is a nominal .NET type
    val isILAppTy: TcGlobals -> TType -> bool

    val isNativePtrTy: TcGlobals -> TType -> bool

    val isByrefTy: TcGlobals -> TType -> bool

    val isInByrefTag: TcGlobals -> TType -> bool

    val isInByrefTy: TcGlobals -> TType -> bool

    val isOutByrefTag: TcGlobals -> TType -> bool

    val isOutByrefTy: TcGlobals -> TType -> bool

#if !NO_TYPEPROVIDERS
    val extensionInfoOfTy: TcGlobals -> TType -> TyconRepresentation
#endif

    /// Represents metadata extracted from a nominal type
    type TypeDefMetadata =
        | ILTypeMetadata of TILObjectReprData
        | FSharpOrArrayOrByrefOrTupleOrExnTypeMetadata
#if !NO_TYPEPROVIDERS
        | ProvidedTypeMetadata of TProvidedTypeInfo
#endif

    /// Extract metadata from a type definition
    val metadataOfTycon: Tycon -> TypeDefMetadata

    /// Extract metadata from a type
    val metadataOfTy: TcGlobals -> TType -> TypeDefMetadata

    val isILReferenceTy: TcGlobals -> TType -> bool

    val isILInterfaceTycon: Tycon -> bool

    /// Get the rank of an array type
    val rankOfArrayTy: TcGlobals -> TType -> int

    val isFSharpObjModelRefTy: TcGlobals -> TType -> bool

    val isFSharpClassTy: TcGlobals -> TType -> bool

    val isFSharpStructTy: TcGlobals -> TType -> bool

    val isFSharpInterfaceTy: TcGlobals -> TType -> bool

    /// Determine if a type is a delegate type
    val isDelegateTy: TcGlobals -> TType -> bool

    /// Determine if a type is an interface type
    val isInterfaceTy: TcGlobals -> TType -> bool

    /// Determine if a type is a delegate type defined in F#
    val isFSharpDelegateTy: TcGlobals -> TType -> bool

    /// Determine if a type is a class type
    val isClassTy: TcGlobals -> TType -> bool

    val isStructOrEnumTyconTy: TcGlobals -> TType -> bool

    /// Determine if a type is a struct, record or union type
    val isStructRecordOrUnionTyconTy: TcGlobals -> TType -> bool

    /// Determine if TyconRef is to a struct type
    val isStructTyconRef: TyconRef -> bool

    /// Determine if a type is a struct type
    val isStructTy: TcGlobals -> TType -> bool

    /// Check if a type is a measureable type (like int<kg>) whose underlying type is a value type.
    val isMeasureableValueType: TcGlobals -> TType -> bool

    /// Determine if a type is a reference type
    val isRefTy: TcGlobals -> TType -> bool

    /// Determine if a type is a function (including generic). Not the same as isFunTy.
    val isForallFunctionTy: TcGlobals -> TType -> bool

    /// Determine if a type is an unmanaged type
    val isUnmanagedTy: TcGlobals -> TType -> bool

    val isInterfaceTycon: Tycon -> bool

    /// Determine if a reference to a type definition is an interface type
    val isInterfaceTyconRef: TyconRef -> bool

    /// Determine if a type is an enum type
    val isEnumTy: TcGlobals -> TType -> bool

    /// Determine if a type is a signed integer type
    val isSignedIntegerTy: TcGlobals -> TType -> bool

    /// Determine if a type is an unsigned integer type
    val isUnsignedIntegerTy: TcGlobals -> TType -> bool

    /// Determine if a type is an integer type
    val isIntegerTy: TcGlobals -> TType -> bool

    /// Determine if a type is a floating point type
    val isFpTy: TcGlobals -> TType -> bool

    /// Determine if a type is a decimal type
    val isDecimalTy: TcGlobals -> TType -> bool

    /// Determine if a type is a non-decimal numeric type type
    val isNonDecimalNumericType: TcGlobals -> TType -> bool

    /// Determine if a type is a numeric type type
    val isNumericType: TcGlobals -> TType -> bool

    val actualReturnTyOfSlotSig: TypeInst -> TypeInst -> SlotSig -> TType option

    val slotSigHasVoidReturnTy: SlotSig -> bool

    val returnTyOfMethod: TcGlobals -> ObjExprMethod -> TType option

    /// Is the type 'abstract' in C#-speak
    val isAbstractTycon: Tycon -> bool

    val MemberIsExplicitImpl: TcGlobals -> ValMemberInfo -> bool

    val ValIsExplicitImpl: TcGlobals -> Val -> bool

    val ValRefIsExplicitImpl: TcGlobals -> ValRef -> bool

    /// Get the unit of measure for an annotated type
    val getMeasureOfType: TcGlobals -> TType -> (TyconRef * Measure) option

    // Return true if this type is a nominal type that is an erased provided type
    val isErasedType: TcGlobals -> TType -> bool

    // Return all components of this type expression that cannot be tested at runtime
    val getErasedTypes: TcGlobals -> TType -> checkForNullness: bool -> TType list

    /// Determine the underlying type of an enum type (normally int32)
    val underlyingTypeOfEnumTy: TcGlobals -> TType -> TType

    /// If the input type is an enum type, then convert to its underlying type, otherwise return the input type
    val normalizeEnumTy: TcGlobals -> TType -> TType

    /// Any delegate type with ResumableCode attribute, or any function returning such a delegate type
    val isResumableCodeTy: TcGlobals -> TType -> bool

    /// The delegate type ResumableCode, or any function returning this a delegate type
    val isReturnsResumableCodeTy: TcGlobals -> TType -> bool

    /// Determine if a value is a method implementing an interface dispatch slot using a private method impl
    val ComputeUseMethodImpl: g: TcGlobals -> v: Val -> bool

    val useGenuineField: Tycon -> RecdField -> bool

    val ComputeFieldName: Tycon -> RecdField -> string

[<AutoOpen>]
module internal CommonContainers =

    //-------------------------------------------------------------------------
    // More common type construction
    //-------------------------------------------------------------------------

    val destByrefTy: TcGlobals -> TType -> TType

    val destNativePtrTy: TcGlobals -> TType -> TType

    val isByrefTyconRef: TcGlobals -> TyconRef -> bool

    val isRefCellTy: TcGlobals -> TType -> bool

    /// Get the element type of an FSharpRef type
    val destRefCellTy: TcGlobals -> TType -> TType

    /// Create the FSharpRef type for a given element type
    val mkRefCellTy: TcGlobals -> TType -> TType

    val StripSelfRefCell: TcGlobals * ValBaseOrThisInfo * TType -> TType

    val isBoolTy: TcGlobals -> TType -> bool

    /// Determine if a type is a value option type
    val isValueOptionTy: TcGlobals -> TType -> bool

    /// Determine if a type is an option type
    val isOptionTy: TcGlobals -> TType -> bool

    /// Determine if a type is an Choice type
    val isChoiceTy: TcGlobals -> TType -> bool

    /// Take apart an option type
    val destOptionTy: TcGlobals -> TType -> TType

    /// Try to take apart an option type
    val tryDestOptionTy: TcGlobals -> TType -> TType voption

    /// Try to take apart an option type
    val destValueOptionTy: TcGlobals -> TType -> TType

    /// Take apart an Choice type
    val tryDestChoiceTy: TcGlobals -> TType -> int -> TType voption

    /// Try to take apart an Choice type
    val destChoiceTy: TcGlobals -> TType -> int -> TType

    /// Determine is a type is a System.Nullable type
    val isNullableTy: TcGlobals -> TType -> bool

    /// Try to take apart a System.Nullable type
    val tryDestNullableTy: TcGlobals -> TType -> TType voption

    /// Take apart a System.Nullable type
    val destNullableTy: TcGlobals -> TType -> TType

    /// Determine if a type is a System.Linq.Expression type
    val isLinqExpressionTy: TcGlobals -> TType -> bool

    /// Take apart a System.Linq.Expression type
    val destLinqExpressionTy: TcGlobals -> TType -> TType

    /// Try to take apart a System.Linq.Expression type
    val tryDestLinqExpressionTy: TcGlobals -> TType -> TType option

    val mkLazyTy: TcGlobals -> TType -> TType

    /// Build an PrintFormat type
    val mkPrintfFormatTy: TcGlobals -> TType -> TType -> TType -> TType -> TType -> TType

    val (|NullableTy|_|): TcGlobals -> TType -> TType voption

    /// An active pattern to transform System.Nullable types to their input, otherwise leave the input unchanged
    [<return: Struct>]
    val (|StripNullableTy|): TcGlobals -> TType -> TType

    /// Matches any byref type, yielding the target type
    [<return: Struct>]
    val (|ByrefTy|_|): TcGlobals -> TType -> TType voption

    val mkListTy: TcGlobals -> TType -> TType

    /// Create the option type for a given element type
    val mkOptionTy: TcGlobals -> TType -> TType

    /// Create the voption type for a given element type
    val mkValueOptionTy: TcGlobals -> TType -> TType

    /// Create the Nullable type for a given element type
    val mkNullableTy: TcGlobals -> TType -> TType

    /// Create the union case 'None' for an option type
    val mkNoneCase: TcGlobals -> UnionCaseRef

    /// Create the union case 'Some(expr)' for an option type
    val mkSomeCase: TcGlobals -> UnionCaseRef

    /// Create the struct union case 'ValueNone' for a voption type
    val mkValueNoneCase: TcGlobals -> UnionCaseRef

    /// Create the struct union case 'ValueSome(expr)' for a voption type
    val mkValueSomeCase: TcGlobals -> UnionCaseRef

    /// Create the struct union case 'Some' or 'ValueSome(expr)' for a voption type
    val mkAnySomeCase: TcGlobals -> isStruct: bool -> UnionCaseRef

    val mkSome: TcGlobals -> TType -> Expr -> range -> Expr

    val mkNone: TcGlobals -> TType -> range -> Expr

    /// Create the expression 'ValueSome(expr)'
    val mkValueSome: TcGlobals -> TType -> Expr -> range -> Expr

    /// Create the struct expression 'ValueNone' for an voption type
    val mkValueNone: TcGlobals -> TType -> range -> Expr

    /// Indicates if an F# type is the type associated with an F# exception declaration
    val isFSharpExceptionTy: g: TcGlobals -> ty: TType -> bool
