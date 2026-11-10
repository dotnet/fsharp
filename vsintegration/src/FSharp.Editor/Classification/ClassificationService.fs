// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Generic
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks
open System.Runtime.Caching
open System.Diagnostics
open System.Runtime.ExceptionServices

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Classification
open Microsoft.Internal.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.Settings
open Microsoft.VisualStudio.Shell

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Tokenization
open CancellableTasks
open Microsoft.VisualStudio.FSharp.Editor.Telemetry

// IEditorClassificationService is marked as Obsolete, but is still supported. The replacement (IClassificationService)
// is internal to Microsoft.CodeAnalysis.Workspaces which we don't have internals visible to. Rather than add yet another
// IVT, we'll maintain the status quo.
#nowarn "44"

#nowarn "57"

type SemanticClassificationData = SemanticClassificationView
type SemanticClassificationLookup = IReadOnlyDictionary<int, ResizeArray<SemanticClassificationItem>>

[<Export(typeof<IFSharpClassificationService>)>]
type internal FSharpClassificationService [<ImportingConstructor>] (serviceProvider: SVsServiceProvider, options: EditorOptions) =

    let struct (_, provider) = HighlightingTelemetry.readProvider serviceProvider
    let produceSemanticClassification = provider <> "lsp"

    do
        HighlightingTelemetry.instance.ReportState(
            "old",
            (if produceSemanticClassification then
                 "enabled"
             else
                 "disabled"),
            [| "resolved_provider", box provider |],
            (fun () -> options.Advanced.SendAdditionalTelemetry)
        )

    static let getLexicalClassifications (filePath: string, defines, text: SourceText, textSpan: TextSpan, ct: CancellationToken) =

        let text = text.GetSubText(textSpan)
        let result = ImmutableArray.CreateBuilder()

        let tokenCallback =
            fun (tok: FSharpToken) ->
                let spanKind =
                    if tok.IsKeyword then
                        ClassificationTypeNames.Keyword
                    elif tok.IsNumericLiteral then
                        ClassificationTypeNames.NumericLiteral
                    elif tok.IsCommentTrivia then
                        ClassificationTypeNames.Comment
                    elif tok.IsStringLiteral then
                        ClassificationTypeNames.StringLiteral
                    else
                        ClassificationTypeNames.Text

                match RoslynHelpers.TryFSharpRangeToTextSpan(text, tok.Range) with
                | ValueSome span -> result.Add(ClassifiedSpan(TextSpan(textSpan.Start + span.Start, span.Length), spanKind))
                | _ -> ()

        let flags =
            FSharpLexerFlags.Default
            &&& ~~~FSharpLexerFlags.Compiling
            &&& ~~~FSharpLexerFlags.UseLexFilter

        FSharpLexer.Tokenize(
            text.ToFSharpSourceText(),
            tokenCallback,
            filePath = filePath,
            conditionalDefines = defines,
            flags = flags,
            ct = ct
        )

        result.ToImmutable()

    static let addSemanticClassification
        sourceText
        (targetSpan: TextSpan)
        (items: seq<SemanticClassificationItem>)
        (outputResult: List<ClassifiedSpan>)
        =
        for item in items do
            match RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, item.Range) with
            | ValueNone -> ()
            | ValueSome span ->
                // Use fixupSpan (not tryFixupSpan) for syntax coloring
                let span =
                    match item.Type with
                    | SemanticClassificationType.Printf -> span
                    | _ -> Tokenizer.fixupSpan (sourceText, span)

                if targetSpan.Contains span then
                    outputResult.Add(ClassifiedSpan(span, FSharpClassificationTypes.getClassificationTypeName (item.Type)))

    static let addSemanticClassificationByLookup
        sourceText
        (targetSpan: TextSpan)
        (lookup: SemanticClassificationLookup)
        (outputResult: List<ClassifiedSpan>)
        =
        let r = RoslynHelpers.TextSpanToFSharpRange("", targetSpan, sourceText)

        for i = r.StartLine to r.EndLine do
            match lookup.TryGetValue i with
            | true, items -> addSemanticClassification sourceText targetSpan items outputResult
            | _ -> ()

    static let toSemanticClassificationLookup (d: SemanticClassificationData) =
        let lookup = Dictionary<int, ResizeArray<SemanticClassificationItem>>()

        let f (dataItem: SemanticClassificationItem) =
            let items =
                match lookup.TryGetValue dataItem.Range.StartLine with
                | true, items -> items
                | _ ->
                    let items = ResizeArray()
                    lookup[dataItem.Range.StartLine] <- items
                    items

            items.Add dataItem

        d.ForEach(f)

        lookup :> IReadOnlyDictionary<_, _>

    static let itemToSemanticClassificationLookup (d: SemanticClassificationItem array) =
        let lookup = Dictionary<int, ResizeArray<SemanticClassificationItem>>()

        for item in d do
            let items =
                let startLine = item.Range.StartLine

                match lookup.TryGetValue startLine with
                | true, items -> items
                | _ ->
                    let items = ResizeArray()
                    lookup[startLine] <- items
                    items

            items.Add item

        lookup :> IReadOnlyDictionary<_, _>

    static let unopenedDocumentsSemanticClassificationCache =
        new DocumentCache<SemanticClassificationLookup>("fsharp-unopened-documents-semantic-classification-cache", 5.)

    static let openedDocumentsSemanticClassificationCache =
        new DocumentCache<SemanticClassificationLookup>("fsharp-opened-documents-semantic-classification-cache", 2.)

    interface IFSharpClassificationService with
        // Do not perform classification if we don't have project options (#defines matter)
        member _.AddLexicalClassifications(_: SourceText, _: TextSpan, _: List<ClassifiedSpan>, _: CancellationToken) = ()

        member _.AddSyntacticClassificationsAsync
            (document: Document, textSpan: TextSpan, result: List<ClassifiedSpan>, cancellationToken: CancellationToken)
            =

            cancellableTask {
                use _logBlock = Logger.LogBlock(LogEditorFunctionId.Classification_Syntactic)

                let! cancellationToken = CancellableTask.getCancellationToken ()

                let defines, langVersion = document.GetFsharpParsingOptions()

                let! sourceText = document.GetTextAsync(cancellationToken)

                // For closed documents, only get classification for the text within the span.
                // This may be inaccurate for multi-line tokens such as string literals, but this is ok for now
                //     as it's better than having to tokenize a big part of a file which in return will allocate a lot and hurt find all references performance.
                let isOpenDocument = document.Project.Solution.Workspace.IsDocumentOpen document.Id

                let eventProps: (string * obj) array =
                    [|
                        "context.document.project.id", document.Project.Id.Id.ToString()
                        "context.document.id", document.Id.Id.ToString()
                        "isOpenDocument", isOpenDocument
                        "textSpanLength", textSpan.Length
                    |]

                use _eventDuration =
                    TelemetryReporter.ReportSingleEventWithDuration(TelemetryEvents.AddSyntacticClassifications, eventProps)

                if not isOpenDocument then
                    let classifiedSpans =
                        getLexicalClassifications (document.FilePath, defines, sourceText, textSpan, cancellationToken)

                    result.AddRange(classifiedSpans)
                else
                    Tokenizer.classifySpans (
                        document.Id,
                        sourceText,
                        textSpan,
                        Some(document.FilePath),
                        defines,
                        Some langVersion,
                        result,
                        cancellationToken
                    )
            }
            |> CancellableTask.startAsTask cancellationToken

        member _.AddSemanticClassificationsAsync
            (document: Document, textSpan: TextSpan, result: List<ClassifiedSpan>, cancellationToken: CancellationToken)
            =

            if not produceSemanticClassification then
                Task.CompletedTask
            else
                cancellableTask {
                    use _logBlock = Logger.LogBlock(LogEditorFunctionId.Classification_Semantic)
                    let stopwatch = Stopwatch.StartNew()
                    let branchStopwatch = Stopwatch()
                    let mutable outcome = "faulted"
                    let mutable cacheHit = false
                    let mutable documentLength = -1
                    let isOpenDocument = document.Project.Solution.Workspace.IsDocumentOpen document.Id

                    try
                        try
                            let! sourceText = document.GetTextAsync(cancellationToken)
                            documentLength <- sourceText.Length

                            let cache =
                                if isOpenDocument then
                                    openedDocumentsSemanticClassificationCache
                                else
                                    unopenedDocumentsSemanticClassificationCache

                            let! cached = cache.TryGetValueAsync document
                            branchStopwatch.Start()

                            match cached with
                            | ValueSome classificationDataLookup ->
                                cacheHit <- true
                                addSemanticClassificationByLookup sourceText textSpan classificationDataLookup result
                            | ValueNone when not isOpenDocument ->
                                let! classificationData =
                                    document.GetFSharpSemanticClassificationAsync(nameof (FSharpClassificationService))

                                let classificationDataLookup = toSemanticClassificationLookup classificationData
                                do! unopenedDocumentsSemanticClassificationCache.SetAsync(document, classificationDataLookup)
                                addSemanticClassificationByLookup sourceText textSpan classificationDataLookup result
                            | ValueNone ->
                                let! _, checkResults = document.GetFSharpParseAndCheckResultsAsync(nameof (IFSharpClassificationService))

                                let targetRange =
                                    RoslynHelpers.TextSpanToFSharpRange(document.FilePath, textSpan, sourceText)

                                let classificationData =
                                    checkResults.GetSemanticClassification(Some targetRange, RelatedSymbolUseKind.All)

                                if classificationData.Length > 0 then
                                    let classificationDataLookup = itemToSemanticClassificationLookup classificationData
                                    do! unopenedDocumentsSemanticClassificationCache.SetAsync(document, classificationDataLookup)
                                    addSemanticClassification sourceText textSpan classificationData result

                            outcome <- "completed"
                        with :? OperationCanceledException as error ->
                            outcome <- "cancelled"
                            ExceptionDispatchInfo.Capture(error).Throw()
                    finally
                        HighlightingTelemetry.instance.ReportDetails(
                            (fun () ->
                                document.Project.Solution.Workspace.Services.GetService<EditorOptions>().Advanced.SendAdditionalTelemetry),
                            [|
                                "context.document.project.id", box (document.Project.Id.Id.ToString())
                                "context.document.id", box (document.Id.Id.ToString())
                                "isOpenDocument", box isOpenDocument
                                "textSpanLength", box textSpan.Length
                                "outcome", box outcome
                                "provider_duration_ms", box stopwatch.ElapsedMilliseconds
                                if documentLength >= 0 then
                                    "document_length", box documentLength
                                if branchStopwatch.IsRunning then
                                    "cacheHit", box cacheHit
                                    "vs_event_duration_ms", box branchStopwatch.ElapsedMilliseconds
                            |]
                        )
                }
                |> CancellableTask.ifCanceledReturn ()
                |> CancellableTask.startAsTask cancellationToken

        // Do not perform classification if we don't have project options (#defines matter)
        member _.AdjustStaleClassification(_: SourceText, classifiedSpan: ClassifiedSpan) : ClassifiedSpan = classifiedSpan
