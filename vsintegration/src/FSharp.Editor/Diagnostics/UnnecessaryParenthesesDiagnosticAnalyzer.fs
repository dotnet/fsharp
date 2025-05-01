// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Collections.Generic
open System.Collections.Immutable
open System.Runtime.Caching
open System.Threading
open System.Threading.Tasks
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Diagnostics
open CancellableTasks

// This interface is not defined in Microsoft.CodeAnalysis.ExternalAccess.FSharp.Diagnostics
// and so we are not currently exporting the type below as an implementation of it
// using [<Export(typeof<IFSharpUnnecessaryParenthesesDiagnosticAnalyzer>)>], since it would not be recognized.
type IFSharpUnnecessaryParenthesesDiagnosticAnalyzer =
    inherit IFSharpDocumentDiagnosticAnalyzer

[<NoEquality; NoComparison>]
type private DocumentData =
    {
        Hash: int
        Diagnostics: ImmutableArray<Diagnostic>
    }

[<Sealed>]
type internal UnnecessaryParenthesesDiagnosticAnalyzer [<ImportingConstructor>] () =
    static let completedTask = Task.FromResult ImmutableArray.Empty

    static let descriptor =
        let title = "Parentheses can be removed."

        DiagnosticDescriptor(
            "FS3583",
            title,
            title,
            "Style",
            DiagnosticSeverity.Hidden,
            isEnabledByDefault = true,
            description = null,
            helpLinkUri = null
        )

    static let cache =
        new MemoryCache $"FSharp.Editor.{nameof UnnecessaryParenthesesDiagnosticAnalyzer}"

    static let semaphore = new SemaphoreSlim 3

    static member GetDiagnostics(document: Document) =
        cancellableTask {
            let! cancellationToken = CancellableTask.getCancellationToken ()
            let! textVersion = document.GetTextVersionAsync cancellationToken
            let textVersionHash = textVersion.GetHashCode()

            match! semaphore.WaitAsync(DefaultTuning.PerDocumentSavedDataSlidingWindow, cancellationToken) with
            | false -> return ImmutableArray.Empty
            | true ->
                try
                    let key = string document.Id

                    match cache.Get key with
                    | :? DocumentData as data when data.Hash = textVersionHash -> return data.Diagnostics
                    | _ ->
                        let! parseResults = document.GetFSharpParseResultsAsync(nameof UnnecessaryParenthesesDiagnosticAnalyzer)
                        let! sourceText = document.GetTextAsync cancellationToken

                        let getLineString line =
                            sourceText.Lines[Line.toZ line].ToString()

                        let unnecessaryParentheses =
                            (HashSet Range.comparer, parseResults.ParseTree)
                            ||> ParsedInput.fold (fun ranges path node ->
                                match node with
                                | SyntaxNode.SynExpr(SynExpr.Paren(expr = inner; rightParenRange = Some _; range = range)) when
                                    not (SynExpr.shouldBeParenthesizedInContext getLineString path inner)
                                    ->
                                    ignore (ranges.Add range)
                                    ranges

                                | SyntaxNode.SynPat(SynPat.Paren(inner, range)) when not (SynPat.shouldBeParenthesizedInContext path inner) ->
                                    ignore (ranges.Add range)
                                    ranges

                                | _ -> ranges)

                        let diagnostics =
                            let builder = ImmutableArray.CreateBuilder unnecessaryParentheses.Count

                            for range in unnecessaryParentheses do
                                builder.Add(
                                    Diagnostic.Create(descriptor, RoslynHelpers.RangeToLocation(range, sourceText, document.FilePath))
                                )

                            builder.MoveToImmutable()

                        ignore (cache.Remove key)

                        cache.Set(
                            CacheItem(
                                key,
                                {
                                    Hash = textVersionHash
                                    Diagnostics = diagnostics
                                }
                            ),
                            CacheItemPolicy(SlidingExpiration = DefaultTuning.PerDocumentSavedDataSlidingWindow)
                        )

                        return diagnostics
                finally
                    ignore (semaphore.Release())
        }

    interface IFSharpUnnecessaryParenthesesDiagnosticAnalyzer with
        member _.AnalyzeSemanticsAsync(document: Document, cancellationToken: CancellationToken) =
            ignore (document, cancellationToken)
            completedTask

        member _.AnalyzeSyntaxAsync(document: Document, cancellationToken: CancellationToken) =
            UnnecessaryParenthesesDiagnosticAnalyzer.GetDiagnostics document
            |> CancellableTask.start cancellationToken
