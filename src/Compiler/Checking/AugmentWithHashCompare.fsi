// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Generate the hash/compare functions we add to user-defined types by default.
module internal FSharp.Compiler.AugmentTypeDefinitions

open FSharp.Compiler
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TcGlobals

[<NoEquality; NoComparison; StructuredFormatDisplay("{DebugText}")>]
type EqualityWithComparerAugmentation =
    { GetHashCode: Val
      GetHashCodeWithComparer: Val
      EqualsWithComparer: Val
      EqualsExactWithComparer: Val }

val mkBindNullComparison: TcGlobals -> Text.Range -> thise: Expr -> thate: Expr -> expr: Expr -> Expr

val mkBindThisNullEquals: TcGlobals -> Text.Range -> thise: Expr -> thate: Expr -> expr: Expr -> Expr

val mkBindNullHash: TcGlobals -> Text.Range -> thise: Expr -> expr: Expr -> Expr

val CheckAugmentationAttribs: bool -> TcGlobals -> Import.ImportMap -> Tycon -> unit

val TyconIsCandidateForAugmentationWithCompare: TcGlobals -> Tycon -> bool

val TyconIsCandidateForAugmentationWithEquals: TcGlobals -> Tycon -> bool

val TyconIsCandidateForAugmentationWithHash: TcGlobals -> Tycon -> bool

val MakeValsForCompareAugmentation: TcGlobals -> TyconRef -> Val * Val

val MakeValsForCompareWithComparerAugmentation: TcGlobals -> TyconRef -> Val

val MakeValsForEqualsAugmentation: TcGlobals -> TyconRef -> Val * Val

val MakeValsForEqualityWithComparerAugmentation: TcGlobals -> TyconRef -> EqualityWithComparerAugmentation

val MakeBindingsForCompareAugmentation: TcGlobals -> Tycon -> Binding list

val MakeBindingsForCompareWithComparerAugmentation: TcGlobals -> Tycon -> Binding list

val MakeBindingsForEqualsAugmentation: TcGlobals -> Tycon -> Binding list

val MakeBindingsForEqualityWithComparerAugmentation: TcGlobals -> Tycon -> Binding list

/// This predicate can be used once type inference is complete, before then it is an approximation
/// that doesn't assert any new constraints
val TypeDefinitelyHasEquality: TcGlobals -> TType -> bool

val MakeValsForUnionAugmentation: TcGlobals -> TyconRef -> Val list

val MakeBindingsForUnionAugmentation: TcGlobals -> Tycon -> ValRef list -> Binding list

/// Build a record's single-line reflection-free ToString body; returns the 'this' value and the body expression.
val mkRecdToString: g: TcGlobals * tcref: TyconRef * tycon: Tycon * openBrace: string * closeBrace: string -> Val * Expr

/// Whether a reflection-free structural ToString should be generated for this type.
val TyconIsCandidateForAugmentationWithToString: g: TcGlobals * tycon: Tycon -> bool

/// Make the ToString override slot for a reflection-free record or union.
val MakeValsForToStringAugmentation: g: TcGlobals * tcref: TyconRef -> Val

/// Build the body binding for a reflection-free record or union ToString override.
val MakeBindingsForToStringAugmentation: g: TcGlobals * tycon: Tycon * toStringVal: Val -> Binding list
