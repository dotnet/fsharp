// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace rec Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Immutable
open System.Diagnostics
open System.Threading

open Microsoft.CodeAnalysis
open System.Runtime.Caching
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Diagnostics
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Text
open CancellableTasks

type private PerDocumentSavedData =
    {
        Hash: int
        Diagnostics: ImmutableArray<Diagnostic>
    }

[<Export(typeof<IFSharpSimplifyNameDiagnosticAnalyzer>)>]
type internal SimplifyNameDiagnosticAnalyzer [<ImportingConstructor>] () =

    static let userOpName = "SimplifyNameDiagnosticAnalyzer"
    static let cache = new MemoryCache("FSharp.Editor." + userOpName)
    // Make sure only a few  documents are being analyzed at a time, to be nice
    static let guard = new SemaphoreSlim(3)

    static member LongIdentPropertyKey = "FullName"

    interface IFSharpSimplifyNameDiagnosticAnalyzer with

        member _.AnalyzeSemanticsAsync(descriptor, document: Document, cancellationToken: CancellationToken) =

            asyncMaybe {
                do! Option.guard document.Project.IsFSharpCodeFixesSimplifyNameEnabled
                do Trace.TraceInformation("{0:n3} (start) SimplifyName", DateTime.Now.TimeOfDay.TotalSeconds)
                let! textVersion = document.GetTextVersionAsync(cancellationToken)
                let textVersionHash = textVersion.GetHashCode()

                let! lockObtained =
                    guard.WaitAsync(DefaultTuning.PerDocumentSavedDataSlidingWindow, cancellationToken)
                    |> Async.AwaitTask
                    |> liftAsync

                do! Option.guard lockObtained

                try
                    let key = document.Id.ToString()

                    match cache.Get(key) with
                    | :? PerDocumentSavedData as data when data.Hash = textVersionHash -> return data.Diagnostics
                    | _ ->
                        let! sourceText = document.GetTextAsync()

                        let! _, checkResults =
                            document.GetFSharpParseAndCheckResultsAsync(nameof (SimplifyNameDiagnosticAnalyzer))
                            |> CancellableTask.start cancellationToken
                            |> Async.AwaitTask
                            |> liftAsync

                        let! result =
                            SimplifyNames.getSimplifiableNames (
                                checkResults,
                                fun lineNumber -> sourceText.Lines.[Line.toZ lineNumber].ToString()
                            )
                            |> liftAsync

                        let mutable diag = ResizeArray()

                        for r in result do
                            diag.Add(
                                Diagnostic.Create(
                                    descriptor,
                                    RoslynHelpers.RangeToLocation(r.Range, sourceText, document.FilePath),
                                    properties =
                                        (dict [ SimplifyNameDiagnosticAnalyzer.LongIdentPropertyKey, r.RelativeName ])
                                            .ToImmutableDictionary()
                                )
                            )

                        let diagnostics = diag.ToImmutableArray()
                        cache.Remove(key) |> ignore

                        let data =
                            {
                                Hash = textVersionHash
                                Diagnostics = diagnostics
                            }

                        let cacheItem = CacheItem(key, data)

                        let policy =
                            CacheItemPolicy(SlidingExpiration = DefaultTuning.PerDocumentSavedDataSlidingWindow)

                        cache.Set(cacheItem, policy)
                        return diagnostics
                finally
                    guard.Release() |> ignore
            }
            |> Async.map (Option.defaultValue ImmutableArray.Empty)
            |> RoslynHelpers.StartAsyncAsTask cancellationToken
