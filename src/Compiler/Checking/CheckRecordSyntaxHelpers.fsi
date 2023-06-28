// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.CheckRecordSyntaxHelpers

open FSharp.Compiler.CheckBasics
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree

val GroupUpdatesToNestedFields:
    fields: ((Ident list * Ident) * SynExpr option) list -> ((Ident list * Ident) * SynExpr option) list

val TransformAstForNestedUpdates<'a> :
    cenv: TcFileState ->
    env: TcEnv ->
    overallTy: TType ->
    lid: LongIdent ->
    exprBeingAssigned: SynExpr ->
    withExpr: SynExpr * (range * 'a) ->
        (Ident list * Ident) * SynExpr option
