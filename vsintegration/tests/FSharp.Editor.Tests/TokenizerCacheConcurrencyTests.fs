// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests

open System
open System.Collections.Concurrent
open System.Threading
open System.Threading.Tasks
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor
open Xunit

type private DocumentVersion =
    {
        Text: SourceText
        SingleThreadedClassificationPerLine: (string * TextSpan)[][]
    }

type TokenizerCacheConcurrencyTests() =

    let fileName = "C:\\test.fs"
    let defines = []
    let langVersion = Some "preview"
    let workerCount = max 8 Environment.ProcessorCount

    let sourceWithMultiLineTokens =
        String.Join(
            "\n",
            [|
                for i in 0..11 do
                    $"let value{i} = {i}"
                    $"// comment {i}"
                    "let block ="
                    "    (* opening"
                    $"       still inside {i} *)"
                    "    42"
                    $"let text{i} = \"\"\"triple"
                    $"   quoted {i}\"\"\""
                    $"let plain{i} = \"single line\""
                    "type T() = class end"
            |]
        )

    let newDocumentId () =
        DocumentId.CreateNewId(ProjectId.CreateNewId())

    let classify (documentId: DocumentId) (text: SourceText) (span: TextSpan) =
        let spans = ResizeArray<ClassifiedSpan>()

        Tokenizer.classifySpans (documentId, text, span, Some fileName, defines, langVersion, spans, CancellationToken.None)

        spans |> Seq.map (fun s -> s.ClassificationType, s.TextSpan) |> Array.ofSeq

    let lineSpan (text: SourceText) startLine endLine =
        TextSpan.FromBounds(text.Lines.[startLine].Start, text.Lines.[endLine].End)

    let versionOf (text: SourceText) =
        let documentId = newDocumentId ()

        {
            Text = text
            SingleThreadedClassificationPerLine = Array.init text.Lines.Count (fun i -> classify documentId text (lineSpan text i i))
        }

    let closedComments = versionOf (SourceText.From sourceWithMultiLineTokens)

    let unclosedComments =
        versionOf (SourceText.From(sourceWithMultiLineTokens.Replace(" *)", "")))

    let bothVersions = [| closedComments; unclosedComments |]

    let alternatingVersion worker iteration = bothVersions.[(worker + iteration) % 2]

    let lineCount = closedComments.Text.Lines.Count

    let anyLines (random: Random) =
        let a, b = random.Next lineCount, random.Next lineCount
        min a b, max a b

    let documentIdWithCacheOfOneLine () =
        let documentId = newDocumentId ()
        let oneLine = SourceText.From "let x = 1"
        classify documentId oneLine (lineSpan oneLine 0 0) |> ignore
        documentId

    let describe (spans: (string * TextSpan)[]) =
        spans |> Seq.map (fun (kind, span) -> $"{kind}{span}") |> String.concat " "

    let readLines (failures: ConcurrentQueue<string>) documentId (version: DocumentVersion) (startLine, endLine) =
        let actual =
            classify documentId version.Text (lineSpan version.Text startLine endLine)

        let expected =
            Array.concat version.SingleThreadedClassificationPerLine.[startLine..endLine]

        if actual <> expected then
            failures.Enqueue $"lines {startLine}..{endLine}\n  expected {describe expected}\n  actual   {describe actual}"

    let expectNoDivergence (failures: ConcurrentQueue<string>) =
        let diverged = String.concat Environment.NewLine failures
        Assert.True(failures.IsEmpty, $"Concurrent classification diverged:{Environment.NewLine}{diverged}")

    let runConcurrently (work: int -> unit) =
        let workers =
            Array.init workerCount (fun worker -> Task.Run(Action(fun () -> work worker)))

        Task.WaitAll workers

    [<Fact>]
    member _.``Overlapping concurrent reads of one document's token cache agree with a single-threaded scan``() =
        let documentId = newDocumentId ()
        let failures = ConcurrentQueue<string>()
        let text = closedComments.Text

        runConcurrently (fun worker ->
            let random = Random(worker)

            for _ in 1..60 do
                readLines failures documentId closedComments (anyLines random)

                Tokenizer.getSymbolAtPosition (
                    documentId,
                    text,
                    random.Next text.Length,
                    fileName,
                    defines,
                    SymbolLookupKind.Greedy,
                    false,
                    false,
                    langVersion,
                    CancellationToken.None
                )
                |> ignore)

        expectNoDivergence failures

    [<Fact>]
    member _.``Concurrent reads of two versions of a document keep invalidating the cache correctly``() =
        let lastLine = lineCount - 1

        Assert.True(
            closedComments.SingleThreadedClassificationPerLine.[lastLine]
            <> unclosedComments.SingleThreadedClassificationPerLine.[lastLine],
            "The versions must classify the last line differently, or no cached entry is ever invalidated"
        )

        let documentId = newDocumentId ()
        let failures = ConcurrentQueue<string>()

        runConcurrently (fun worker ->
            let random = Random(worker)

            for iteration in 1..60 do
                readLines failures documentId (alternatingVersion worker iteration) (anyLines random))

        expectNoDivergence failures

    [<Fact>]
    member _.``Concurrent scans that grow the cache of a document first seen shorter agree with a single-threaded scan``() =
        let failures = ConcurrentQueue<string>()

        for round in 1..20 do
            let documentId = documentIdWithCacheOfOneLine ()

            runConcurrently (fun worker ->
                let random = Random(round * workerCount + worker)

                for iteration in 1..10 do
                    let endLine = random.Next lineCount
                    let startLine = if iteration % 2 = 0 then 0 else random.Next(endLine + 1)
                    readLines failures documentId (alternatingVersion worker iteration) (startLine, endLine))

        expectNoDivergence failures
