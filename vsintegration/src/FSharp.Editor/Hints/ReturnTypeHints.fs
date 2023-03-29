// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor.Hints

open Microsoft.VisualStudio.FSharp.Editor
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Symbols
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open Hints

type ReturnTypeHints(parseFileResults: FSharpParseFileResults, symbol: FSharpMemberOrFunctionOrValue) =

    let getHintParts (symbolUse: FSharpSymbolUse) =
        symbol.GetReturnTypeLayout symbolUse.DisplayContext
        |> Option.map (fun typeInfo ->
            [
                TaggedText(TextTag.Text, ": ")
                yield! typeInfo |> Array.toList
                TaggedText(TextTag.Space, " ")
            ])

    let getHint symbolUse range =
        getHintParts symbolUse
        |> Option.map (fun parts ->
            {
                Kind = HintKind.ReturnTypeHint
                Range = range
                Parts = parts
            })

    let findEqualsPositionIfValid (symbolUse: FSharpSymbolUse) =
        SyntaxTraversal.Traverse(
            symbolUse.Range.End,
            parseFileResults.ParseTree,
            { new SyntaxVisitorBase<_>() with
                override _.VisitBinding(_path, defaultTraverse, binding) =
                    match binding with
                    // Skip lambdas
                    | SynBinding(expr = SynExpr.Lambda _) -> defaultTraverse binding

                    // Let binding
                    | SynBinding (trivia = { EqualsRange = Some equalsRange }; range = range; returnInfo = None) when
                        range.Start = symbolUse.Range.Start
                        ->
                        Some equalsRange.StartRange

                    // Member binding
                    | SynBinding (headPat = SynPat.LongIdent(longDotId = SynLongIdent(id = _ :: ident :: _))
                                  trivia = { EqualsRange = Some equalsRange }
                                  returnInfo = None) when

                        ident.idRange.Start = symbolUse.Range.Start
                        ->
                        Some equalsRange.StartRange

                    | _ -> defaultTraverse binding
            }
        )

    member _.getHints symbolUse =
        [
            if symbol.IsFunction then
                yield!
                    findEqualsPositionIfValid symbolUse
                    |> Option.bind (getHint symbolUse)
                    |> Option.toList
        ]
