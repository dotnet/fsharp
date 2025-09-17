// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.CheckRecordSyntaxHelpers

open FSharp.Compiler.CheckBasics
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree

[<RequireQualifiedAccess; NoEquality; NoComparison>]
type SynExprOrSpreadValue =
    /// A syntactic expression being assigned to a record field.
    | SynExpr of SynExpr

    /// A typechecked record field `get` from a spread expression.
    | SpreadValue of TType * Expr

val GroupUpdatesToNestedFields:
    fields: ((Ident list * Ident) * SynExprOrSpreadValue option) list ->
        ((Ident list * Ident) * SynExprOrSpreadValue option) list

val TransformAstForNestedUpdates:
    cenv: TcFileState ->
    env: TcEnv ->
    overallTy: TType ->
    lid: LongIdent ->
    exprBeingAssigned: SynExprOrSpreadValue ->
    withExpr: SynExpr * BlockSeparator ->
        (Ident list * Ident) * SynExprOrSpreadValue option

val BindOriginalRecdExpr:
    withExpr: SynExpr * BlockSeparator -> mkRecdExpr: ((SynExpr * BlockSeparator) option -> SynExpr) -> SynExpr
