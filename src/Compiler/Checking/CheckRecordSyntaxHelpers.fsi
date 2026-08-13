// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.CheckRecordSyntaxHelpers

open FSharp.Compiler.CheckBasics
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree

val TransformAstForNestedUpdates<'a> :
    cenv: TcFileState ->
    env: TcEnv ->
    overallTy: TType ->
    lid: LongIdent ->
    exprBeingAssigned: SynExpr ->
    withExpr: SynExpr * (range * 'a) ->
        (Ident list * Ident) * SynExpr

val BindIdText: string

val inline (|IsSimpleOrBoundExpr|_|): withExpr: SynExpr -> bool

val BindOriginalRecdExpr:
    withExpr: SynExpr * BlockSeparator -> mkRecdExpr: ((SynExpr * BlockSeparator) option -> SynExpr) -> SynExpr

val bindSrcIn: spreadSrcExpr: SynExpr -> ((SynExpr -> SynExpr) -> SynExpr)
