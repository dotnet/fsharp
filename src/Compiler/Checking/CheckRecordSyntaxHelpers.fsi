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

val TransformAstForNestedUpdates<'a> :
    cenv: TcFileState ->
    env: TcEnv ->
    overallTy: TType ->
    lid: LongIdent ->
    exprBeingAssigned: SynExpr ->
    withExpr: SynExpr * (range * 'a) ->
        (Ident list * Ident) * SynExpr option

val BindIdText: string

val inline (|IsSimpleOrBoundExpr|_|): withExprOpt: (SynExpr * BlockSeparator) option -> bool

val BindOriginalRecdExpr:
    withExpr: SynExpr * BlockSeparator -> mkRecdExpr: ((SynExpr * BlockSeparator) option -> SynExpr) -> SynExpr
