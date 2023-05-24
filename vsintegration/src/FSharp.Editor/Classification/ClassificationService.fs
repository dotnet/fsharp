// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Generic
open System.Collections.Immutable
open System.Threading
open System.Runtime.Caching

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Classification

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
type internal FSharpClassificationService [<ImportingConstructor>] () =

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
                | Some span -> result.Add(ClassifiedSpan(TextSpan(textSpan.Start + span.Start, span.Length), spanKind))
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
            | None -> ()
            | Some span ->
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
                    lookup.[dataItem.Range.StartLine] <- items
                    items

            items.Add dataItem

        d.ForEach(f)

        Collections.ObjectModel.ReadOnlyDictionary lookup :> IReadOnlyDictionary<_, _>

    let semanticClassificationCache = new DocumentCache<SemanticClassificationLookup>("fsharp-semantic-classification-cache")

    interface IFSharpClassificationService with
        // Do not perform classification if we don't have project options (#defines matter)
        member _.AddLexicalClassifications(_: SourceText, _: TextSpan, _: List<ClassifiedSpan>, _: CancellationToken) = ()

        member _.AddSyntacticClassificationsAsync
            (
                document: Document,
                textSpan: TextSpan,
                result: List<ClassifiedSpan>,
                cancellationToken: CancellationToken
            ) =
            cancellableTask {
                use _logBlock = Logger.LogBlock(LogEditorFunctionId.Classification_Syntactic)

                let! cancellationToken = CancellableTask.getCurrentCancellationToken ()

                let defines, langVersion = document.GetFSharpQuickDefinesAndLangVersion()
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
                    TelemetryReporter.ReportSingleEventWithDuration(TelemetryEvents.AddSyntacticCalssifications, eventProps)

                if not isOpenDocument then
                    let classifiedSpans =
                        getLexicalClassifications (document.FilePath, defines, sourceText, textSpan, cancellationToken)

                    result.AddRange(classifiedSpans)
                else
                    result.AddRange(
                        Tokenizer.getClassifiedSpans (
                            document.Id,
                            sourceText,
                            textSpan,
                            Some(document.FilePath),
                            defines,
                            Some langVersion,
                            cancellationToken
                        )
                    )
            }
            |> CancellableTask.startAsTask cancellationToken

        member _.AddSemanticClassificationsAsync
            (
                document: Document,
                textSpan: TextSpan,
                result: List<ClassifiedSpan>,
                cancellationToken: CancellationToken
            ) =
            cancellableTask {
                use _logBlock = Logger.LogBlock(LogEditorFunctionId.Classification_Semantic)

                let! sourceText = document.GetTextAsync(cancellationToken)

                // If we are trying to get semantic classification for a document that is not open, get the results from the background and cache it.
                // We do this for find all references when it is populating results.
                // We cache it temporarily so we do not have to continously call into the checker and perform a background operation.
                let isOpenDocument = document.Project.Solution.Workspace.IsDocumentOpen document.Id

                if not isOpenDocument then
                    match! semanticClassificationCache.TryGetValueAsync document with
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
                            TelemetryReporter.ReportSingleEventWithDuration(TelemetryEvents.AddSemanticCalssifications, eventProps)

                        addSemanticClassificationByLookup sourceText textSpan classificationDataLookup result
                    | _ ->
                        let eventProps : (string * obj) array =
                            [|
                                "context.document.project.id", document.Project.Id.Id.ToString()
                                "context.document.id", document.Id.Id.ToString()
                                "isOpenDocument", isOpenDocument
                                "textSpanLength", textSpan.Length
                                "cacheHit", false
                            |]

                        use _eventDuration =
                            TelemetryReporter.ReportSingleEventWithDuration(TelemetryEvents.AddSemanticCalssifications, eventProps)

                        let! classificationData = document.GetFSharpSemanticClassificationAsync(nameof (FSharpClassificationService))
                        let classificationDataLookup = toSemanticClassificationLookup classificationData
                        do! semanticClassificationCache.SetAsync(document, classificationDataLookup)
                        addSemanticClassificationByLookup sourceText textSpan classificationDataLookup result
                else
                    let eventProps: (string * obj) array =
                        [|
                            "context.document.project.id", document.Project.Id.Id.ToString()
                            "context.document.id", document.Id.Id.ToString()
                            "isOpenDocument", isOpenDocument
                            "textSpanLength", textSpan.Length
                            "cacheHit", false
                        |]

                    use _eventDuration =
                        TelemetryReporter.ReportSingleEventWithDuration(TelemetryEvents.AddSemanticCalssifications, eventProps)

                    let! _, checkResults = document.GetFSharpParseAndCheckResultsAsync(nameof (IFSharpClassificationService))

                    let targetRange =
                        RoslynHelpers.TextSpanToFSharpRange(document.FilePath, textSpan, sourceText)

                    let classificationData = checkResults.GetSemanticClassification(Some targetRange)
                    addSemanticClassification sourceText textSpan classificationData result
            }
            |> CancellableTask.startAsTask cancellationToken

        // Do not perform classification if we don't have project options (#defines matter)
        member _.AdjustStaleClassification(_: SourceText, classifiedSpan: ClassifiedSpan) : ClassifiedSpan = classifiedSpan
