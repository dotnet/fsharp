// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Generic
open System.Collections.Immutable
open System.Threading
open System.Runtime.Caching
open System.Runtime.CompilerServices

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Classification

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

type internal LastGoodSemanticClassification =
    {
        Text: SourceText
        Lookup: SemanticClassificationLookup
    }

[<Export(typeof<IFSharpClassificationService>)>]
type internal FSharpClassificationService [<ImportingConstructor>] () =

    static let shouldProduceClassification (document: Document) =
        document.Project.Solution.GetFSharpExtensionConfig().ShouldProduceSemanticHighlighting()

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

    // Roslyn replaces a span's semantic tags with whatever this service returns, so answering "nothing"
    // while the checker is unavailable (project loading or reloading, a superseded check) strips the
    // colours the user already sees. Keep the last whole-file lookup per document, outliving the
    // version-keyed cache above, and re-emit it for those requests.
    static let lastGoodSemanticClassification =
        ConditionalWeakTable<DocumentId, LastGoodSemanticClassification>()

    static let rememberLastGood (documentId: DocumentId) (text: SourceText) (lookup: SemanticClassificationLookup) =
        let lastGood = { Text = text; Lookup = lookup }
        // net472 has no ConditionalWeakTable.AddOrUpdate.
        lock lastGoodSemanticClassification (fun () ->
            lastGoodSemanticClassification.Remove documentId |> ignore
            lastGoodSemanticClassification.Add(documentId, lastGood))

    // Only for the text it was computed from: the lookup names positions, so against edited text it
    // would colour the wrong characters.
    static let addLastGood (documentId: DocumentId) (sourceText: SourceText) (targetSpan: TextSpan) (result: List<ClassifiedSpan>) =
        match lastGoodSemanticClassification.TryGetValue documentId with
        | true, lastGood when lastGood.Text.ContentEquals sourceText ->
            addSemanticClassificationByLookup sourceText targetSpan lastGood.Lookup result
        | _ -> ()

    static let addLastGoodForCurrentText (document: Document) (targetSpan: TextSpan) (result: List<ClassifiedSpan>) =
        match document.TryGetText() with
        | true, sourceText -> addLastGood document.Id sourceText targetSpan result
        | _ -> ()

    // Which of the two caches a document lands in is not observable from its classifications - a miss
    // only costs a recheck - so tests reach the caches directly to tell the branches apart.
    static member internal OpenedDocumentsSemanticClassificationCache =
        openedDocumentsSemanticClassificationCache

    static member internal UnopenedDocumentsSemanticClassificationCache =
        unopenedDocumentsSemanticClassificationCache

    interface IFSharpClassificationService with
        // Do not perform classification if we don't have project options (#defines matter)
        member _.AddLexicalClassifications(_: SourceText, _: TextSpan, _: List<ClassifiedSpan>, _: CancellationToken) = ()

        member _.AddSyntacticClassificationsAsync
            (document: Document, textSpan: TextSpan, result: List<ClassifiedSpan>, cancellationToken: CancellationToken)
            =

            if not (document |> shouldProduceClassification) then
                System.Threading.Tasks.Task.CompletedTask
            else

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

            if not (document |> shouldProduceClassification) then
                System.Threading.Tasks.Task.CompletedTask
            else

                cancellableTask {
                    use _logBlock = Logger.LogBlock(LogEditorFunctionId.Classification_Semantic)

                    let! sourceText = document.GetTextAsync(cancellationToken)

                    // If we are trying to get semantic classification for a document that is not open, get the results from the background and cache it.
                    // We do this for find all references when it is populating results.
                    // We cache it temporarily so we do not have to continuously call into the checker and perform a background operation.
                    let isOpenDocument = document.Project.Solution.Workspace.IsDocumentOpen document.Id

                    if not isOpenDocument then
                        match! unopenedDocumentsSemanticClassificationCache.TryGetValueAsync document with
                        | ValueSome classificationDataLookup ->
                            let eventProps: (string * obj) array =
                                [|
                                    "context.document.project.id", document.Project.Id.Id.ToString()
                                    "context.document.id", document.Id.Id.ToString()
                                    "isOpenDocument", isOpenDocument
                                    "textSpanLength", textSpan.Length
                                    "cacheHit", true
                                |]

                            use _eventDuration =
                                TelemetryReporter.ReportSingleEventWithDuration(TelemetryEvents.AddSemanticClassifications, eventProps)

                            addSemanticClassificationByLookup sourceText textSpan classificationDataLookup result
                        | ValueNone ->
                            let eventProps: (string * obj) array =
                                [|
                                    "context.document.project.id", document.Project.Id.Id.ToString()
                                    "context.document.id", document.Id.Id.ToString()
                                    "isOpenDocument", isOpenDocument
                                    "textSpanLength", textSpan.Length
                                    "cacheHit", false
                                |]

                            use _eventDuration =
                                TelemetryReporter.ReportSingleEventWithDuration(TelemetryEvents.AddSemanticClassifications, eventProps)

                            match! document.TryGetFSharpSemanticClassificationAsync(nameof (FSharpClassificationService)) with
                            | ValueNone -> ()
                            | ValueSome classificationData ->
                                let classificationDataLookup = toSemanticClassificationLookup classificationData
                                do! unopenedDocumentsSemanticClassificationCache.SetAsync(document, classificationDataLookup)
                                addSemanticClassificationByLookup sourceText textSpan classificationDataLookup result
                    else

                        match! openedDocumentsSemanticClassificationCache.TryGetValueAsync document with
                        | ValueSome classificationDataLookup ->
                            let eventProps: (string * obj) array =
                                [|
                                    "context.document.project.id", document.Project.Id.Id.ToString()
                                    "context.document.id", document.Id.Id.ToString()
                                    "isOpenDocument", isOpenDocument
                                    "textSpanLength", textSpan.Length
                                    "cacheHit", true
                                |]

                            use _eventDuration =
                                TelemetryReporter.ReportSingleEventWithDuration(TelemetryEvents.AddSemanticClassifications, eventProps)

                            addSemanticClassificationByLookup sourceText textSpan classificationDataLookup result
                        | ValueNone ->

                            let eventProps: (string * obj) array =
                                [|
                                    "context.document.project.id", document.Project.Id.Id.ToString()
                                    "context.document.id", document.Id.Id.ToString()
                                    "isOpenDocument", isOpenDocument
                                    "textSpanLength", textSpan.Length
                                    "cacheHit", false
                                |]

                            use _eventDuration =
                                TelemetryReporter.ReportSingleEventWithDuration(TelemetryEvents.AddSemanticClassifications, eventProps)

                            match! document.TryGetFSharpParseAndCheckResultsAsync(nameof (IFSharpClassificationService)) with
                            | ValueNone -> addLastGood document.Id sourceText textSpan result
                            | ValueSome(struct (_, checkResults)) ->
                                // The cache is keyed by text version only, so it has to hold the whole file:
                                // the next request at this version is usually for a different span.
                                let classificationData =
                                    checkResults.GetSemanticClassification(None, RelatedSymbolUseKind.All)

                                // Every checked file resolves at least its enclosing module, so nothing here means
                                // the classification itself failed (SemanticClassification.fs recovers with an empty
                                // array). Caching that would pin the version to no colours.
                                if classificationData.Length = 0 then
                                    addLastGood document.Id sourceText textSpan result
                                else
                                    let classificationDataLookup = itemToSemanticClassificationLookup classificationData
                                    do! openedDocumentsSemanticClassificationCache.SetAsync(document, classificationDataLookup)
                                    rememberLastGood document.Id sourceText classificationDataLookup
                                    addSemanticClassificationByLookup sourceText textSpan classificationDataLookup result
                }
                // A cancellation that is not Roslyn's own (a superseded or aborted check surfaces as one)
                // must not turn into an empty answer, which Roslyn would paint as "no colours".
                |> CancellableTask.ifCanceledThen (fun () -> addLastGoodForCurrentText document textSpan result)
                |> CancellableTask.startAsTask cancellationToken

        // Do not perform classification if we don't have project options (#defines matter)
        member _.AdjustStaleClassification(_: SourceText, classifiedSpan: ClassifiedSpan) : ClassifiedSpan = classifiedSpan
