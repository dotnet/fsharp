// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor.QuickInfo

open System
open System.Threading
open System.Threading.Tasks
open System.ComponentModel.Composition

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text

open Microsoft.VisualStudio.Language.Intellisense
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Utilities
open Microsoft.VisualStudio.FSharp.Editor

open FSharp.Compiler.Text
open Microsoft.IO
open FSharp.Compiler.EditorServices

type internal FSharpAsyncQuickInfoSource
    (
        statusBar: StatusBar,
        xmlMemberIndexService: IVsXMLMemberIndexService,
        metadataAsSource: FSharpMetadataAsSourceService,
        textBuffer: ITextBuffer,
        editorOptions: EditorOptions
    ) =

    let tryGetQuickInfoItem (session: IAsyncQuickInfoSession) =
        asyncMaybe {
            let userOpName = "getQuickInfo"

            let! triggerPoint = session.GetTriggerPoint(textBuffer.CurrentSnapshot) |> Option.ofNullable
            let position = triggerPoint.Position

            let document =
                textBuffer.CurrentSnapshot.GetOpenDocumentInCurrentContextWithChanges()

            let! lexerSymbol = document.TryFindFSharpLexerSymbolAsync(position, SymbolLookupKind.Greedy, true, true, userOpName)
            let! _, checkFileResults = document.GetFSharpParseAndCheckResultsAsync(userOpName) |> liftAsync
            let! cancellationToken = Async.CancellationToken |> liftAsync
            let! sourceText = document.GetTextAsync cancellationToken
            let idRange = lexerSymbol.Ident.idRange
            let textLinePos = sourceText.Lines.GetLinePosition position
            let fcsTextLineNumber = Line.fromZ textLinePos.Line
            let lineText = (sourceText.Lines.GetLineFromPosition position).ToString()

            let getLinkTooltip filePath =
                let solutionDir = Path.GetDirectoryName(document.Project.Solution.FilePath)
                let projectDir = Path.GetDirectoryName(document.Project.FilePath)

                [
                    Path.GetRelativePath(projectDir, filePath)
                    Path.GetRelativePath(solutionDir, filePath)
                ]
                |> List.minBy String.length

            let symbolUseRange =
                maybe {
                    let! symbolUse =
                        checkFileResults.GetSymbolUseAtLocation(fcsTextLineNumber, idRange.EndColumn, lineText, lexerSymbol.FullIsland)

                    return symbolUse.Range
                }

            let tryGetSingleContent (data: ToolTipElement) =
                maybe {
                    let documentationBuilder =
                        XmlDocumentation.CreateDocumentationBuilder(xmlMemberIndexService)

                    let symbol, description, documentation =
                        XmlDocumentation.BuildSingleTipText(documentationBuilder, data)

                    return
                        QuickInfoViewProvider.provideContent (
                            Tokenizer.GetImageIdForSymbol(symbol, lexerSymbol.Kind),
                            description,
                            documentation,
                            FSharpNavigation(statusBar, metadataAsSource, document, defaultArg symbolUseRange range.Zero),
                            getLinkTooltip
                        )
                }

            let (ToolTipText elements) =
                match lexerSymbol.Kind with
                | LexerSymbolKind.Keyword -> checkFileResults.GetKeywordTooltip(lexerSymbol.FullIsland)
                | LexerSymbolKind.String ->
                    checkFileResults.GetToolTip(
                        fcsTextLineNumber,
                        idRange.EndColumn,
                        lineText,
                        lexerSymbol.FullIsland,
                        FSharp.Compiler.Tokenization.FSharpTokenTag.String,
                        ?width = editorOptions.QuickInfo.DescriptionWidth
                    )
                | _ ->
                    checkFileResults.GetToolTip(
                        fcsTextLineNumber,
                        idRange.EndColumn,
                        lineText,
                        lexerSymbol.FullIsland,
                        FSharp.Compiler.Tokenization.FSharpTokenTag.IDENT,
                        ?width = editorOptions.QuickInfo.DescriptionWidth
                    )

            let content = elements |> List.choose tryGetSingleContent
            do! Option.guard (not content.IsEmpty)

            let! textSpan = RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, lexerSymbol.Range)

            let trackingSpan =
                textBuffer.CurrentSnapshot.CreateTrackingSpan(textSpan.Start, textSpan.Length, SpanTrackingMode.EdgeInclusive)

            return QuickInfoItem(trackingSpan, QuickInfoViewProvider.stackWithSeparators content)
        }

    interface IAsyncQuickInfoSource with
        override _.Dispose() = () // no cleanup necessary

        override _.GetQuickInfoItemAsync(session: IAsyncQuickInfoSession, cancellationToken: CancellationToken) : Task<QuickInfoItem> =
            tryGetQuickInfoItem session
            |> Async.map Option.toObj
            |> RoslynHelpers.StartAsyncAsTask cancellationToken

[<Export(typeof<IAsyncQuickInfoSourceProvider>)>]
[<Name("F# Quick Info Provider")>]
[<ContentType(FSharpConstants.FSharpLanguageName)>]
[<Order>]
type internal FSharpAsyncQuickInfoSourceProvider [<ImportingConstructor>]
    (
        [<Import(typeof<SVsServiceProvider>)>] serviceProvider: IServiceProvider,
        metadataAsSource: FSharpMetadataAsSourceService,
        editorOptions: EditorOptions
    ) =

    interface IAsyncQuickInfoSourceProvider with
        override _.TryCreateQuickInfoSource(textBuffer: ITextBuffer) : IAsyncQuickInfoSource =
            // GetService calls must be made on the UI thread
            // It is safe to do it here (see #4713)
            let statusBar = StatusBar(serviceProvider.GetService<SVsStatusbar, IVsStatusbar>())
            let xmlMemberIndexService = serviceProvider.XMLMemberIndexService

            new FSharpAsyncQuickInfoSource(statusBar, xmlMemberIndexService, metadataAsSource, textBuffer, editorOptions)
