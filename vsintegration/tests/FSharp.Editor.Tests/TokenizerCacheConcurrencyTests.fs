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

/// Every editor operation on a document shares one token cache, keyed by document id and defines, so
/// classification and symbol lookup read and write the same entries from whatever thread they run on.
type TokenizerCacheConcurrencyTests() =

    let fileName = "C:\\test.fs"
    let defines = []
    let langVersion = Some "preview"

    // Block comments and triple-quoted strings make a line's classification depend on the lex state
    // threaded out of the line before it, so a torn cache misclassifies lines instead of losing them.
    let source =
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

    // Dropping every `*)` leaves the first block comment open to the end of the file: the lines keep
    // their text and hash but change lex state, which is what drives cached entries out of the cache.
    let sourceText = SourceText.From source
    let commentedText = SourceText.From(source.Replace(" *)", ""))

    let newDocumentId () =
        DocumentId.CreateNewId(ProjectId.CreateNewId())

    let classify (documentId: DocumentId) (text: SourceText) (span: TextSpan) =
        let spans = ResizeArray<ClassifiedSpan>()

        Tokenizer.classifySpans (documentId, text, span, Some fileName, defines, langVersion, spans, CancellationToken.None)

        spans |> Seq.map (fun s -> s.ClassificationType, s.TextSpan) |> Array.ofSeq

    let lineSpan (text: SourceText) startLine endLine =
        TextSpan.FromBounds(text.Lines.[startLine].Start, text.Lines.[endLine].End)

    /// What an uncontended scan of the document produces, one line at a time.
    let scanEveryLine (text: SourceText) =
        let documentId = newDocumentId ()

        Array.init text.Lines.Count (fun i -> classify documentId text (lineSpan text i i))

    let expectedRange (perLine: (string * TextSpan)[][]) startLine endLine =
        Array.concat perLine.[startLine..endLine]

    let describe (spans: (string * TextSpan)[]) =
        spans |> Seq.map (fun (kind, span) -> $"{kind}{span}") |> String.concat " "

    let runConcurrently workerCount (work: int -> unit) =
        let workers =
            Array.init workerCount (fun worker -> Task.Run(Action(fun () -> work worker)))

        Task.WaitAll workers

    let workerCount = max 8 Environment.ProcessorCount

    [<Fact>]
    member _.``Overlapping concurrent reads of one document's token cache agree with a single-threaded scan``() =
        let perLine = scanEveryLine sourceText
        let lineCount = sourceText.Lines.Count
        let documentId = newDocumentId ()
        let failures = ConcurrentQueue<string>()

        runConcurrently workerCount (fun worker ->
            let random = Random(worker)

            for _ in 1..60 do
                let startLine, endLine =
                    let a = random.Next lineCount
                    let b = random.Next lineCount
                    min a b, max a b

                let actual = classify documentId sourceText (lineSpan sourceText startLine endLine)
                let expected = expectedRange perLine startLine endLine

                if actual <> expected then
                    failures.Enqueue $"lines {startLine}..{endLine}\n  expected {describe expected}\n  actual   {describe actual}"

                // Symbol lookup reads the same cache, and does so a line at a time.
                Tokenizer.getSymbolAtPosition (
                    documentId,
                    sourceText,
                    random.Next sourceText.Length,
                    fileName,
                    defines,
                    SymbolLookupKind.Greedy,
                    false,
                    false,
                    langVersion,
                    CancellationToken.None
                )
                |> ignore)

        let diverged = String.concat Environment.NewLine failures
        Assert.True(failures.IsEmpty, $"Concurrent classification diverged:{Environment.NewLine}{diverged}")

    [<Fact>]
    member _.``Concurrent reads of two versions of a document keep invalidating the cache correctly``() =
        let perLine = scanEveryLine sourceText
        let commentedPerLine = scanEveryLine commentedText
        let lineCount = sourceText.Lines.Count

        // Without this the test would pass on a cache that never invalidates anything.
        Assert.NotEqual<(string * TextSpan)[]>(perLine.[lineCount - 1], commentedPerLine.[lineCount - 1])

        let documentId = newDocumentId ()
        let failures = ConcurrentQueue<string>()

        runConcurrently workerCount (fun worker ->
            let random = Random(worker)

            for iteration in 1..60 do
                let text, expectedPerLine =
                    if (worker + iteration) % 2 = 0 then
                        sourceText, perLine
                    else
                        commentedText, commentedPerLine

                let startLine, endLine =
                    let a = random.Next lineCount
                    let b = random.Next lineCount
                    min a b, max a b

                let actual = classify documentId text (lineSpan text startLine endLine)
                let expected = expectedRange expectedPerLine startLine endLine

                if actual <> expected then
                    failures.Enqueue $"lines {startLine}..{endLine}\n  expected {describe expected}\n  actual   {describe actual}")

        let diverged = String.concat Environment.NewLine failures
        Assert.True(failures.IsEmpty, $"Concurrent classification diverged:{Environment.NewLine}{diverged}")
