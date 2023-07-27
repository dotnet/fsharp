// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.CheckRecordSyntaxHelpers

open FSharp.Compiler.CheckBasics
open FSharp.Compiler.NameResolution
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree

val GroupUpdatesToNestedFields:
    sink: TcResultsSink ->
    nenv: NameResolutionEnv ->
    ad: AccessibilityLogic.AccessorDomain ->
    fields: ((Ident list * Ident) * Item option * SynExpr option) list ->
        ((Ident list * Ident) * Item option * SynExpr option) list

val TransformAstForNestedUpdates<'a> :
    cenv: TcFileState ->
    env: TcEnv ->
    overallTy: TType ->
    lid: LongIdent ->
    exprBeingAssigned: SynExpr ->
    withExpr: SynExpr * (range * 'a) ->
        (Ident list * Ident) * Item option * SynExpr option
