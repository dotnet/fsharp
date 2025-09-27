// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.CheckRecordSyntaxHelpers

open FSharp.Compiler.CheckBasics
open FSharp.Compiler.NameResolution
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree

val GroupUpdatesToNestedFields:
    fields: (ExplicitOrSpread<(Ident list * Ident) * SynExpr option, (Ident list * Ident) * 'Spread>) list ->
        (ExplicitOrSpread<(Ident list * Ident) * SynExpr option, (Ident list * Ident) * 'Spread>) list

val TransformAstForNestedUpdates:
    cenv: TcFileState ->
    env: TcEnv ->
    overallTy: TType ->
    lid: LongIdent ->
    exprBeingAssigned: SynExpr ->
    withExpr: SynExpr * BlockSeparator ->
        (Ident list * Ident) * SynExpr option

val BindOriginalRecdExpr:
    withExpr: SynExpr * BlockSeparator -> mkRecdExpr: ((SynExpr * BlockSeparator) option -> SynExpr) -> SynExpr
