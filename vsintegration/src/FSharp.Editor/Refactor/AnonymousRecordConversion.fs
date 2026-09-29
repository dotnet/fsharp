// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal Microsoft.VisualStudio.FSharp.Editor.AnonymousRecordConversion

open Microsoft.CodeAnalysis.Text

open FSharp.Compiler.Syntax
open FSharp.Compiler.Text

open StructConversion

/// Both forms differ only by `struct` in front of `{|`, which the node's range starts with.
let private keywordChanges (sourceText: SourceText) (toStruct: bool) (isStruct: bool) (m: range) =
    match isStruct, toStruct with
    | true, true
    | false, false -> []
    | false, true ->
        let start = (spanOf sourceText m).Start

        let keyword =
            if needsSpaceBefore sourceText start then
                " struct "
            else
                "struct "

        [ TextChange(TextSpan(start, 0), keyword) ]
    | true, false -> [ TextChange(structKeyword sourceText m, "") ]

let kind: StructKind =
    {
        IsExpr =
            fun expr _ ->
                match expr with
                | SynExpr.AnonRecd _ -> true
                | _ -> false
        IsPat = fun _ _ -> false
        IsType =
            function
            | SynType.AnonRecd _ -> true
            | _ -> false
        IsStruct =
            function
            | CaretNode.Expr(node = SynExpr.AnonRecd(isStruct = isStruct))
            | CaretNode.Type(node = SynType.AnonRecd(isStruct = isStruct)) -> isStruct
            | _ -> false
        ExprChanges =
            fun sourceText toStruct expr _ ->
                match expr with
                | SynExpr.AnonRecd(isStruct = isStruct; range = m) -> ValueSome(keywordChanges sourceText toStruct isStruct m)
                | _ -> ValueNone
        PatChanges = fun _ _ _ _ -> ValueNone
        TypeChanges =
            fun sourceText toStruct _ ty ->
                match ty with
                | SynType.AnonRecd(isStruct = isStruct; range = m) -> keywordChanges sourceText toStruct isStruct m
                | _ -> []
    }
