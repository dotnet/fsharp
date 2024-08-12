// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor.QuickInfo

open System.Threading
open System.Threading.Tasks
open System.ComponentModel.Composition

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text

open Microsoft.VisualStudio.Language.Intellisense
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Utilities
open Microsoft.VisualStudio.FSharp.Editor

open FSharp.Compiler.Text
open Microsoft.IO
open FSharp.Compiler.EditorServices
open CancellableTasks

type internal FSharpAsyncQuickInfoSource
    (xmlMemberIndexService, metadataAsSource: FSharpMetadataAsSourceService, textBuffer: ITextBuffer, editorOptions: EditorOptions) =

    let getQuickInfoItem (sourceText, (document: Document), (lexerSymbol: LexerSymbol), (ToolTipText elements)) =
        cancellableTask {
            let documentationBuilder =
                XmlDocumentation.CreateDocumentationBuilder(xmlMemberIndexService)

            let getSingleContent (data: ToolTipElement) =

                let symbol, description, documentation =
                    XmlDocumentation.BuildSingleTipText(
                        documentationBuilder,
                        data,
                        XmlDocumentation.DefaultLineLimits,
                        editorOptions.QuickInfo.ShowRemarks
                    )

                let getLinkTooltip filePath =
                    let solutionDir = Path.GetDirectoryName(document.Project.Solution.FilePath)
                    let projectDir = Path.GetDirectoryName(document.Project.FilePath)

                    [
                        Path.GetRelativePath(projectDir, filePath)
                        if not (isNull solutionDir) then
                            Path.GetRelativePath(solutionDir, filePath)
                    ]
                    |> List.minBy String.length

                QuickInfoViewProvider.provideContent (
                    Tokenizer.GetImageIdForSymbol(symbol, lexerSymbol.Kind),
                    description,
                    documentation,
                    FSharpNavigation(metadataAsSource, document, lexerSymbol.Range),
                    getLinkTooltip
                )

            let content = elements |> List.map getSingleContent

            if content.IsEmpty then
                return None
            else
                let textSpan = RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, lexerSymbol.Range)

                match textSpan with
                | ValueNone -> return None
                | ValueSome textSpan ->
                    let trackingSpan =
                        textBuffer.CurrentSnapshot.CreateTrackingSpan(textSpan.Start, textSpan.Length, SpanTrackingMode.EdgeInclusive)

                    return Some(QuickInfoItem(trackingSpan, QuickInfoViewProvider.stackWithSeparators content))
        }

    static member TryGetToolTip(document: Document, position, ?width) =
        cancellableTask {
            let userOpName = "getQuickInfo"
            let! cancellationToken = CancellableTask.getCancellationToken ()
            let! lexerSymbol = document.TryFindFSharpLexerSymbolAsync(position, SymbolLookupKind.Greedy, true, true, userOpName)

            match lexerSymbol with
            | None -> return None
            | Some lexerSymbol ->
                let! _, checkFileResults = document.GetFSharpParseAndCheckResultsAsync(userOpName)
                let! sourceText = document.GetTextAsync cancellationToken
                let range = lexerSymbol.Range
                let textLinePos = sourceText.Lines.GetLinePosition position
                let fcsTextLineNumber = Line.fromZ textLinePos.Line
                let lineText = (sourceText.Lines.GetLineFromPosition position).ToString()

                let tooltip =
                    match lexerSymbol.Kind with
                    | LexerSymbolKind.Keyword -> checkFileResults.GetKeywordTooltip(lexerSymbol.FullIsland)
                    | LexerSymbolKind.String ->
                        checkFileResults.GetToolTip(
                            fcsTextLineNumber,
                            range.EndColumn,
                            lineText,
                            lexerSymbol.FullIsland,
                            FSharp.Compiler.Tokenization.FSharpTokenTag.String,
                            ?width = width
                        )
                    | _ ->
                        checkFileResults.GetToolTip(
                            fcsTextLineNumber,
                            range.EndColumn,
                            lineText,
                            lexerSymbol.FullIsland,
                            FSharp.Compiler.Tokenization.FSharpTokenTag.IDENT,
                            ?width = width
                        )

                return Some(sourceText, document, lexerSymbol, tooltip)
        }

    interface IAsyncQuickInfoSource with
        override _.Dispose() = () // no cleanup necessary

        override _.GetQuickInfoItemAsync(session: IAsyncQuickInfoSession, cancellationToken: CancellationToken) : Task<QuickInfoItem> =
            cancellableTask {
                let document =
                    textBuffer.CurrentSnapshot.GetOpenDocumentInCurrentContextWithChanges()

                let triggerPoint = session.GetTriggerPoint(textBuffer.CurrentSnapshot)

                if not triggerPoint.HasValue then
                    return Unchecked.defaultof<_>
                else
                    let position = triggerPoint.Value.Position

                    let! tipdata =
                        FSharpAsyncQuickInfoSource.TryGetToolTip(document, position, ?width = editorOptions.QuickInfo.DescriptionWidth)

                    match tipdata with
                    | Some tipdata ->
                        let! tipdata = getQuickInfoItem tipdata
                        return Option.toObj tipdata
                    | None -> return Unchecked.defaultof<_>

            }
            |> CancellableTask.start cancellationToken

[<Export(typeof<IAsyncQuickInfoSourceProvider>)>]
[<Name("F# Quick Info Provider")>]
[<ContentType(FSharpConstants.FSharpLanguageName)>]
[<Order>]
type internal FSharpAsyncQuickInfoSourceProvider
    [<ImportingConstructor>]
    (
        [<Import(typeof<SVsServiceProvider>)>] serviceProvider: System.IServiceProvider,
        metadataAsSource: FSharpMetadataAsSourceService,
        editorOptions: EditorOptions
    ) =

    interface IAsyncQuickInfoSourceProvider with
        override _.TryCreateQuickInfoSource(textBuffer: ITextBuffer) : IAsyncQuickInfoSource =
            let xmlMemberIndexService = serviceProvider.XMLMemberIndexService
            new FSharpAsyncQuickInfoSource(xmlMemberIndexService, metadataAsSource, textBuffer, editorOptions)
