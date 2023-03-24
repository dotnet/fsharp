// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor.QuickInfo

open System
open System.IO
open System.Threading
open System.Threading.Tasks
open System.ComponentModel.Composition
open System.Text

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

type internal FSharpAsyncQuickInfoSource
    (
        statusBar: StatusBar,
        xmlMemberIndexService: IVsXMLMemberIndexService,
        metadataAsSource: FSharpMetadataAsSourceService,
        textBuffer: ITextBuffer
    ) =

    // test helper
    static member ProvideQuickInfo(document: Document, position: int) =
        asyncMaybe {
            let! _, sigQuickInfo, targetQuickInfo = FSharpQuickInfo.getQuickInfo (document, position, CancellationToken.None)
            return! sigQuickInfo |> Option.orElse targetQuickInfo
        }

    static member BuildSingleQuickInfoItem (documentationBuilder: IDocumentationBuilder) (quickInfo: FSharpQuickInfo) =
        let mainDescription, documentation, typeParameterMap, usage, exceptions =
            ResizeArray(), ResizeArray(), ResizeArray(), ResizeArray(), ResizeArray()

        XmlDocumentation.BuildDataTipText(
            documentationBuilder,
            mainDescription.Add,
            documentation.Add,
            typeParameterMap.Add,
            usage.Add,
            exceptions.Add,
            quickInfo.StructuredText
        )

        let docs =
            RoslynHelpers.joinWithLineBreaks [ documentation; typeParameterMap; usage; exceptions ]

        (mainDescription, docs)

    interface IAsyncQuickInfoSource with
        override _.Dispose() = () // no cleanup necessary

        // This method can be called from the background thread.
        // Do not call IServiceProvider.GetService here.
        override _.GetQuickInfoItemAsync(session: IAsyncQuickInfoSession, cancellationToken: CancellationToken) : Task<QuickInfoItem> =
            let triggerPoint = session.GetTriggerPoint(textBuffer.CurrentSnapshot)

            match triggerPoint.HasValue with
            | false -> Task.FromResult<QuickInfoItem>(null)
            | true ->
                let triggerPoint = triggerPoint.GetValueOrDefault()

                asyncMaybe {
                    let document =
                        textBuffer.CurrentSnapshot.GetOpenDocumentInCurrentContextWithChanges()

                    let! symbolUseRange, sigQuickInfo, targetQuickInfo =
                        FSharpQuickInfo.getQuickInfo (document, triggerPoint.Position, cancellationToken)

                    let getTooltip filePath =
                        let solutionDir = Path.GetDirectoryName(document.Project.Solution.FilePath)
                        let projectDir = Path.GetDirectoryName(document.Project.FilePath)

                        [
                            Path.GetRelativePath(projectDir, filePath)
                            Path.GetRelativePath(solutionDir, filePath)
                        ]
                        |> List.minBy String.length

                    let getTrackingSpan (span: TextSpan) =
                        textBuffer.CurrentSnapshot.CreateTrackingSpan(span.Start, span.Length, SpanTrackingMode.EdgeInclusive)

                    let documentationBuilder =
                        XmlDocumentation.CreateDocumentationBuilder(xmlMemberIndexService)

                    match sigQuickInfo, targetQuickInfo with
                    | None, None -> return null
                    | Some quickInfo, None
                    | None, Some quickInfo ->
                        let mainDescription, docs =
                            FSharpAsyncQuickInfoSource.BuildSingleQuickInfoItem documentationBuilder quickInfo

                        let imageId = Tokenizer.GetImageIdForSymbol(quickInfo.Symbol, quickInfo.SymbolKind)

                        let navigation =
                            FSharpNavigation(statusBar, metadataAsSource, document, symbolUseRange)

                        let content =
                            QuickInfoViewProvider.provideContent (
                                imageId,
                                mainDescription |> List.ofSeq,
                                [ docs |> List.ofSeq ],
                                navigation,
                                getTooltip
                            )

                        let span = getTrackingSpan quickInfo.Span
                        return QuickInfoItem(span, content)

                    | Some sigQuickInfo, Some targetQuickInfo ->
                        let mainDescription, targetDocumentation, sigDocumentation, typeParameterMap, exceptions, usage =
                            ResizeArray(), ResizeArray(), ResizeArray(), ResizeArray(), ResizeArray(), ResizeArray()

                        XmlDocumentation.BuildDataTipText(
                            documentationBuilder,
                            ignore,
                            sigDocumentation.Add,
                            ignore,
                            ignore,
                            ignore,
                            sigQuickInfo.StructuredText
                        )

                        XmlDocumentation.BuildDataTipText(
                            documentationBuilder,
                            mainDescription.Add,
                            targetDocumentation.Add,
                            typeParameterMap.Add,
                            exceptions.Add,
                            usage.Add,
                            targetQuickInfo.StructuredText
                        )
                        // get whitespace nomalized documentation text
                        let getText (tts: seq<TaggedText>) =
                            let text =
                                (StringBuilder(), tts)
                                ||> Seq.fold (fun sb tt ->
                                    if String.IsNullOrWhiteSpace tt.Text then
                                        sb
                                    else
                                        sb.Append tt.Text)
                                |> string

                            if String.IsNullOrWhiteSpace text then None else Some text

                        let documentationParts: TaggedText list list =
                            [
                                match getText targetDocumentation, getText sigDocumentation with
                                | None, None -> ()
                                | None, Some _ -> sigDocumentation |> List.ofSeq
                                | Some _, None -> targetDocumentation |> List.ofSeq
                                | Some implText, Some sigText when implText.Equals(sigText, StringComparison.OrdinalIgnoreCase) ->
                                    sigDocumentation |> List.ofSeq
                                | Some _, Some _ ->
                                    sigDocumentation |> List.ofSeq
                                    targetDocumentation |> List.ofSeq
                                RoslynHelpers.joinWithLineBreaks [ typeParameterMap; usage; exceptions ]
                                |> List.ofSeq
                            ]

                        let imageId =
                            Tokenizer.GetImageIdForSymbol(targetQuickInfo.Symbol, targetQuickInfo.SymbolKind)

                        let navigation =
                            FSharpNavigation(statusBar, metadataAsSource, document, symbolUseRange)

                        let content =
                            QuickInfoViewProvider.provideContent (
                                imageId,
                                mainDescription |> List.ofSeq,
                                documentationParts,
                                navigation,
                                getTooltip
                            )

                        let span = getTrackingSpan targetQuickInfo.Span
                        return QuickInfoItem(span, content)
                }
                |> Async.map Option.toObj
                |> RoslynHelpers.StartAsyncAsTask cancellationToken

[<Export(typeof<IAsyncQuickInfoSourceProvider>)>]
[<Name("F# Quick Info Provider")>]
[<ContentType(FSharpConstants.FSharpLanguageName)>]
[<Order>]
type internal FSharpAsyncQuickInfoSourceProvider [<ImportingConstructor>]
    (
        [<Import(typeof<SVsServiceProvider>)>] serviceProvider: IServiceProvider,
        metadataAsSource: FSharpMetadataAsSourceService
    ) =

    interface IAsyncQuickInfoSourceProvider with
        override _.TryCreateQuickInfoSource(textBuffer: ITextBuffer) : IAsyncQuickInfoSource =
            // GetService calls must be made on the UI thread
            // It is safe to do it here (see #4713)
            let statusBar = StatusBar(serviceProvider.GetService<SVsStatusbar, IVsStatusbar>())
            let xmlMemberIndexService = serviceProvider.XMLMemberIndexService
            new FSharpAsyncQuickInfoSource(statusBar, xmlMemberIndexService, metadataAsSource, textBuffer) :> IAsyncQuickInfoSource
