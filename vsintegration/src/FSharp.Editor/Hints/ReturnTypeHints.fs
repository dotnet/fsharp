// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor.Hints

open Microsoft.VisualStudio.FSharp.Editor
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Symbols
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Position
open Hints
open FSharp.Compiler.Syntax


type ReturnTypeHints(parseFileResults: FSharpParseFileResults, symbol: FSharpMemberOrFunctionOrValue) =


    let getHintParts (symbolUse: FSharpSymbolUse) =

        match symbol.GetReturnTypeLayout symbolUse.DisplayContext with
        | Some typeInfo ->
            let colon = TaggedText(TextTag.Text, ": ")
            colon :: (typeInfo |> Array.toList)

        // not sure when this can happen
        | None -> []


    let getHint (symbolUse: FSharpSymbolUse) =
        {
            Kind = HintKind.TypeHint
            Range = symbolUse.Range.EndRange
            Parts = getHintParts symbolUse
        }

    let isSolved =
        if symbol.GenericParameters.Count > 0 then
            symbol.GenericParameters |> Seq.forall (fun p -> p.IsSolveAtCompileTime)

        elif symbol.FullType.IsGenericParameter then
            symbol.FullType.GenericParameter.DisplayNameCore <> "?"

        else
            true

    let findBindingEqualsPosition (symbolUse: FSharpSymbolUse) =
        let visitor =
            { new SyntaxVisitorBase<_>() with

                override _.VisitExpr(_, _, defaultTraverse, expr) = defaultTraverse expr

                override _.VisitBinding(_path, defaultTraverse, binding) =
                    match binding with
                    | SynBinding (trivia = trivia; range = range) when range = symbolUse.Range ->
                        trivia.EqualsRange
                    | _ -> defaultTraverse binding
            }
        SyntaxTraversal.Traverse(symbolUse.Range.End, parseFileResults.ParseTree, visitor)


    member _.isValidForReturnTypeHint (symbolUse: FSharpSymbolUse) =


        let adjustedRangeStart =

                symbolUse.Range.Start

        let isNotAnnotatedManually =
            not (parseFileResults.IsTypeAnnotationGivenAtPosition adjustedRangeStart)

        let isNotAfterDot = symbolUse.IsFromDefinition && not symbol.IsMemberThisValue

        let isNotTypeAlias = not symbol.IsConstructorThisValue

        symbol.IsValue // we'll be adding other stuff gradually here
        && isSolved
        && isNotAnnotatedManually
        && isNotAfterDot
        && isNotTypeAlias

    member _.getHints (symbolUse: FSharpSymbolUse) = [
        if symbol.IsFunction && isSolved then

            match findBindingEqualsPosition symbolUse with
            | Some equalsRange ->

                let _adjustedRangeStart = equalsRange.Start

                getHint symbolUse
            | None -> ()

    ]