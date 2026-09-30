// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Concurrent
open System.Collections.Generic
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks
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

/// What a classification answers for: the text it colours and the semantics it was checked against.
/// An edit elsewhere in the project reclassifies this document without touching its text.
[<Struct>]
type internal ClassificationVersion =
    {
        TextVersion: VersionStamp
        SemanticVersion: VersionStamp
    }

/// The whole-file semantic classification of one version of an open document.
type internal OpenDocumentClassification =
    {
        Version: ClassificationVersion
        Text: SourceText
        Lookup: SemanticClassificationLookup
    }

type internal ClassifyWholeFile = CancellationToken -> Task<OpenDocumentClassification voption>

/// One classification of a document version, shared by every request that arrives while it runs.
/// The computation starts with the first waiter and is cancelled only when the last one leaves; once
/// it is, nobody can join it any more.
[<Sealed>]
type internal InFlightClassification(version: ClassificationVersion, compute: ClassifyWholeFile) =
    let cts = new CancellationTokenSource()
    let job = lazy (compute cts.Token)
    let gate = obj ()
    let mutable waiters = 0
    let mutable closed = false

    member _.Version = version

    member _.IsCancelled = lock gate (fun () -> closed)

    member _.IsCompleted = job.IsValueCreated && job.Value.IsCompleted

    /// ValueNone once the last waiter has left and the work is being cancelled: start a new one.
    member _.TryJoin(cancellationToken: CancellationToken) : Task<OpenDocumentClassification voption> voption =
        let joined =
            lock gate (fun () ->
                if not closed then
                    waiters <- waiters + 1

                not closed)

        if not joined then
            ValueNone
        else
            let job = job.Value
            let left = ref 0

            let leave () =
                if Interlocked.Exchange(left, 1) = 0 then
                    let cancel =
                        lock gate (fun () ->
                            waiters <- waiters - 1
                            closed <- waiters = 0 && not job.IsCompleted
                            closed)

                    // Outside the lock: cancellation callbacks run synchronously.
                    if cancel then
                        cts.Cancel()

            ValueSome(
                task {
                    use _ = cancellationToken.Register(fun () -> leave ())

                    try
                        let! _ = Task.WhenAny(job, Task.Delay(Timeout.Infinite, cancellationToken))
                        cancellationToken.ThrowIfCancellationRequested()
                        return! job
                    finally
                        leave ()
                }
            )

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

    // The classification of an open document's latest checked version. Roslyn asks for it span by span
    // for as long as that version is on screen, and replaces a span's semantic tags with whatever comes
    // back - so it is kept for the life of the document rather than expiring, and when the checker
    // cannot answer (project loading or reloading, a superseded check) it is re-emitted rather than
    // answering "nothing", which would strip the colours the user already sees.
    static let openDocumentClassifications =
        ConditionalWeakTable<DocumentId, OpenDocumentClassification>()

    static let inFlightClassifications =
        ConcurrentDictionary<DocumentId, InFlightClassification>()

    static let remember (documentId: DocumentId) (classification: OpenDocumentClassification) =
        // net472 has no ConditionalWeakTable.AddOrUpdate.
        lock openDocumentClassifications (fun () ->
            openDocumentClassifications.Remove documentId |> ignore
            openDocumentClassifications.Add(documentId, classification))

    // Only for the text it was computed from: the lookup names positions, so against edited text it
    // would colour the wrong characters.
    static let addLastGood (documentId: DocumentId) (sourceText: SourceText) (targetSpan: TextSpan) (result: List<ClassifiedSpan>) =
        match openDocumentClassifications.TryGetValue documentId with
        | true, classification when classification.Text.ContentEquals sourceText ->
            addSemanticClassificationByLookup sourceText targetSpan classification.Lookup result
        | _ -> ()

    static let addLastGoodForCurrentText (document: Document) (targetSpan: TextSpan) (result: List<ClassifiedSpan>) =
        match document.TryGetText() with
        | true, sourceText -> addLastGood document.Id sourceText targetSpan result
        | _ -> ()

    static let classifyWholeFile (document: Document) (version: ClassificationVersion) (sourceText: SourceText) =
        cancellableTask {
            match! document.TryGetFSharpParseAndCheckResultsAsync(nameof (IFSharpClassificationService)) with
            | ValueNone -> return ValueNone
            | ValueSome(struct (_, checkResults)) ->
                let classificationData =
                    checkResults.GetSemanticClassification(None, RelatedSymbolUseKind.All)

                // Every checked file resolves at least its enclosing module, so nothing here means
                // the classification itself failed (SemanticClassification.fs recovers with an empty
                // array). Remembering that would pin the version to no colours.
                if classificationData.Length = 0 then
                    return ValueNone
                else
                    let classification =
                        {
                            Version = version
                            Text = sourceText
                            Lookup = itemToSemanticClassificationLookup classificationData
                        }

                    remember document.Id classification
                    return ValueSome classification
        }

    // Requests for the same version that overlap - split views, the taggers above and below the
    // viewport - share one classification instead of each walking the whole file.
    static let classifyOpenDocument (document: Document) (version: ClassificationVersion) (sourceText: SourceText) =
        cancellableTask {
            let! cancellationToken = CancellableTask.getCancellationToken ()

            let start () =
                InFlightClassification(version, classifyWholeFile document version sourceText)

            // An instance its last waiter has just abandoned refuses to be joined; the next pass
            // replaces it.
            let rec join () =
                let inFlight =
                    inFlightClassifications.AddOrUpdate(
                        document.Id,
                        (fun _ -> start ()),
                        fun _ running ->
                            if running.Version = version && not running.IsCancelled then
                                running
                            else
                                start ()
                    )

                match inFlight.TryJoin cancellationToken with
                | ValueSome work -> struct (inFlight, work)
                | ValueNone -> join ()

            let struct (inFlight, work) = join ()

            try
                return! work
            finally
                // A waiter that leaves early keeps the entry for those still waiting.
                if inFlight.IsCompleted || inFlight.IsCancelled then
                    (inFlightClassifications :> ICollection<KeyValuePair<_, _>>).Remove(KeyValuePair(document.Id, inFlight))
                    |> ignore
        }

    // Which store a document lands in is not observable from its classifications - a miss only costs
    // a recheck - so tests reach them directly to tell the branches apart.
    static member internal OpenDocumentClassifications = openDocumentClassifications

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

                        let! textVersion = document.GetTextVersionAsync(cancellationToken)
                        let! semanticVersion = document.Project.GetDependentSemanticVersionAsync(cancellationToken)

                        let version =
                            {
                                TextVersion = textVersion
                                SemanticVersion = semanticVersion
                            }

                        match openDocumentClassifications.TryGetValue document.Id with
                        | true, classification when classification.Version = version ->
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

                            addSemanticClassificationByLookup sourceText textSpan classification.Lookup result
                        | _ ->

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

                            match! classifyOpenDocument document version sourceText with
                            | ValueSome classification -> addSemanticClassificationByLookup sourceText textSpan classification.Lookup result
                            | ValueNone -> addLastGood document.Id sourceText textSpan result
                }
                // A cancellation that is not Roslyn's own (a superseded or aborted check surfaces as one)
                // must not turn into an empty answer, which Roslyn would paint as "no colours".
                |> CancellableTask.ifCanceledThen (fun () -> addLastGoodForCurrentText document textSpan result)
                |> CancellableTask.startAsTask cancellationToken

        // Do not perform classification if we don't have project options (#defines matter)
        member _.AdjustStaleClassification(_: SourceText, classifiedSpan: ClassifiedSpan) : ClassifiedSpan = classifiedSpan
