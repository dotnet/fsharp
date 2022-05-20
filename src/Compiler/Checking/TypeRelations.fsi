// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Primary relations on types and signatures, with the exception of
/// constraint solving and method overload resolution.
module internal FSharp.Compiler.TypeRelations

open FSharp.Compiler.Import
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree

type CanCoerce =
    | CanCoerce
    | NoCoerce

/// Implements a :> b without coercion based on finalized (no type variable) types
val TypeDefinitelySubsumesTypeNoCoercion:
    ndeep: int -> g: TcGlobals -> amap: ImportMap -> m: range -> ty1: TType -> ty2: TType -> bool

/// The feasible equivalence relation. Part of the language spec.
val TypesFeasiblyEquivalent:
    stripMeasures: bool -> ndeep: int -> g: TcGlobals -> amap: 'a -> m: range -> ty1: TType -> ty2: TType -> bool

/// The feasible equivalence relation. Part of the language spec.
val TypesFeasiblyEquiv: ndeep: int -> g: TcGlobals -> amap: 'a -> m: range -> ty1: TType -> ty2: TType -> bool

/// The feasible equivalence relation after stripping Measures.
val TypesFeasiblyEquivStripMeasures: g: TcGlobals -> amap: 'a -> m: range -> ty1: TType -> ty2: TType -> bool

/// The feasible coercion relation. Part of the language spec.
val TypeFeasiblySubsumesType:
    ndeep: int ->
    g: TcGlobals ->
    amap: ImportMap ->
    m: range ->
    ty1: TType ->
    canCoerce: CanCoerce ->
    ty2: TType ->
        bool

/// Choose solutions for Expr.TyChoose type "hidden" variables introduced
/// by letrec nodes. Also used by the pattern match compiler to choose type
/// variables when compiling patterns at generalized bindings.
///     e.g. let ([], x) = ([], [])
/// Here x gets a generalized type "list<'T>".
val ChooseTyparSolutionAndRange: g: TcGlobals -> amap: ImportMap -> tp: Typar -> TType * range

val ChooseTyparSolution: g: TcGlobals -> amap: ImportMap -> tp: Typar -> TType

val IterativelySubstituteTyparSolutions: g: TcGlobals -> tps: Typars -> solutions: TTypes -> TypeInst

val ChooseTyparSolutionsForFreeChoiceTypars: g: TcGlobals -> amap: ImportMap -> e: Expr -> Expr

/// Break apart lambdas. Needs ChooseTyparSolutionsForFreeChoiceTypars because it's used in
/// PostTypeCheckSemanticChecks before we've eliminated these nodes.
val tryDestTopLambda:
    g: TcGlobals ->
    amap: ImportMap ->
    ValReprInfo ->
    e: Expr * ty: TType ->
        (Typars * Val option * Val option * Val list list * Expr * TType) option

val destTopLambda:
    g: TcGlobals ->
    amap: ImportMap ->
    topValInfo: ValReprInfo ->
    e: Expr * ty: TType ->
        Typars * Val option * Val option * Val list list * Expr * TType

/// Do AdjustArityOfLambdaBody for a series of iterated lambdas, producing one method.
/// The required iterated function arity (List.length topValInfo) must be identical
/// to the iterated function arity of the input lambda (List.length vsl)
val IteratedAdjustArityOfLambda:
    g: TcGlobals ->
    amap: ImportMap ->
    topValInfo: ValReprInfo ->
    e: Expr ->
        Typars * Val option * Val option * Val list list * Expr * TType

/// "Single Feasible Type" inference
/// Look for the unique supertype of ty2 for which ty2 :> ty1 might feasibly hold
val FindUniqueFeasibleSupertype: g: TcGlobals -> amap: ImportMap -> m: range -> ty1: TType -> ty2: TType -> TType option

/// Returns true if ty is feasibly accepted as one of the parameters of activePatternTy
val ActivePatternFeasiblyAcceptsTypeAsInput:
    g: TcGlobals ->
    amap: ImportMap ->
    m: range ->
    ty: TType ->
    activePatternTy: TType ->
        bool
