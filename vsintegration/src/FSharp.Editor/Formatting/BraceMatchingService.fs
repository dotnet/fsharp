// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.ComponentModel.Composition
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open FSharp.Compiler.CodeAnalysis
open System.Runtime.InteropServices
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Editor

[<Export(typeof<IFSharpBraceMatcher>)>]
type internal FSharpBraceMatchingService [<ImportingConstructor>] () =

    static member GetBraceMatchingResult
        (
            document: Document,
            position: int,
            userOpName: string,
            [<Optional; DefaultParameterValue(false)>] forFormatting: bool
        ) =
        async {
            let! checker, _, parsingOptions, _ =
                document.GetFSharpCompilationOptionsAsync(nameof (FSharpBraceMatchingService))

            let! text = document.GetFSharpSourceText()

            let! matchedBraces = checker.MatchBraces(document.FilePath, text, parsingOptions, userOpName)

            let! sourceText = document.GetTextAsync() |> Async.AwaitTask

            let isPositionInRange range =
                match RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, range) with
                | None -> false
                | Some span ->
                    if forFormatting then
                        let length = position - span.Start
                        length >= 0 && length <= span.Length
                    else
                        span.Contains position

            return
                matchedBraces
                |> Array.tryFind (fun (left, right) -> isPositionInRange left || isPositionInRange right)

        }

    interface IFSharpBraceMatcher with
        member this.FindBracesAsync(document, position, cancellationToken) =
            asyncMaybe {
                let! left, right =  FSharpBraceMatchingService.GetBraceMatchingResult(document, position, nameof FSharpBraceMatchingService)
                let! sourceText = document.GetTextAsync(cancellationToken)
                let! leftSpan = RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, left)
                let! rightSpan = RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, right)
                return FSharpBraceMatchingResult(leftSpan, rightSpan)
            }
            |> Async.map Option.toNullable
            |> RoslynHelpers.StartAsyncAsTask cancellationToken
