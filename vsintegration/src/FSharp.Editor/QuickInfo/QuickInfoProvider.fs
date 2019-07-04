// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
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

open FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.Range
open FSharp.Compiler

open Internal.Utilities.StructuredFormat

type internal QuickInfo =
    { StructuredText: FSharpStructuredToolTipText
      Span: TextSpan
      Symbol: FSharpSymbol
      SymbolKind: LexerSymbolKind }

module internal FSharpQuickInfo =

    let userOpName = "QuickInfo"

    // when a construct has been declared in a signature file the documentation comments that are
    // written in that file are the ones that go into the generated xml when the project is compiled
    // therefore we should include these doccoms in our design time quick info
    let getQuickInfoFromRange
        (
            checker: FSharpChecker,
            projectInfoManager: FSharpProjectOptionsManager,
            document: Document,
            declRange: range,
            cancellationToken: CancellationToken
        )
        : Async<QuickInfo option> =

        asyncMaybe {
            let solution = document.Project.Solution
            // ascertain the location of the target declaration in the signature file
            let! extDocId = solution.GetDocumentIdsWithFilePath declRange.FileName |> Seq.tryHead
            let extDocument = solution.GetProject(extDocId.ProjectId).GetDocument extDocId
            let! extSourceText = extDocument.GetTextAsync cancellationToken
            let! extSpan = RoslynHelpers.TryFSharpRangeToTextSpan (extSourceText, declRange)
            let extLineText = (extSourceText.Lines.GetLineFromPosition extSpan.Start).ToString()

            // project options need to be retrieved because the signature file could be in another project
            let! extParsingOptions, extProjectOptions = projectInfoManager.TryGetOptionsByProject(document.Project, cancellationToken)
            let extDefines = CompilerEnvironment.GetCompilationDefinesForEditing extParsingOptions
            let! extLexerSymbol = Tokenizer.getSymbolAtPosition(extDocId, extSourceText, extSpan.Start, declRange.FileName, extDefines, SymbolLookupKind.Greedy, true)
            let! _, _, extCheckFileResults = checker.ParseAndCheckDocument(extDocument, extProjectOptions, allowStaleResults=true, sourceText=extSourceText, userOpName = userOpName)

            let! extQuickInfoText = 
                extCheckFileResults.GetStructuredToolTipText
                    (declRange.StartLine, extLexerSymbol.Ident.idRange.EndColumn, extLineText, extLexerSymbol.FullIsland, FSharpTokenTag.IDENT, userOpName=userOpName) |> liftAsync

            match extQuickInfoText with
            | FSharpToolTipText []
            | FSharpToolTipText [FSharpStructuredToolTipElement.None] -> return! None
            | extQuickInfoText  ->
                let! extSymbolUse =
                    extCheckFileResults.GetSymbolUseAtLocation(declRange.StartLine, extLexerSymbol.Ident.idRange.EndColumn, extLineText, extLexerSymbol.FullIsland, userOpName=userOpName)
                let! span = RoslynHelpers.TryFSharpRangeToTextSpan (extSourceText, extLexerSymbol.Range)

                return { StructuredText = extQuickInfoText
                         Span = span
                         Symbol = extSymbolUse.Symbol
                         SymbolKind = extLexerSymbol.Kind }
        }

    /// Get QuickInfo combined from doccom of Signature and definition
    let getQuickInfo
        (
            checker: FSharpChecker,
            projectInfoManager: FSharpProjectOptionsManager,
            document: Document,
            position: int,
            cancellationToken: CancellationToken
        )
        : Async<(FSharpSymbolUse * QuickInfo option * QuickInfo option) option> =

        asyncMaybe {
            let! sourceText = document.GetTextAsync cancellationToken
            let! parsingOptions, projectOptions = projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document, cancellationToken)
            let defines = CompilerEnvironment.GetCompilationDefinesForEditing parsingOptions
            let! lexerSymbol = Tokenizer.getSymbolAtPosition(document.Id, sourceText, position, document.FilePath, defines, SymbolLookupKind.Greedy, true)
            let idRange = lexerSymbol.Ident.idRange  
            let! _, _, checkFileResults = checker.ParseAndCheckDocument(document, projectOptions, allowStaleResults = true, sourceText=sourceText, userOpName = userOpName)
            let textLinePos = sourceText.Lines.GetLinePosition position
            let fcsTextLineNumber = Line.fromZ textLinePos.Line
            let lineText = (sourceText.Lines.GetLineFromPosition position).ToString()
            let! symbolUse = checkFileResults.GetSymbolUseAtLocation (fcsTextLineNumber, idRange.EndColumn, lineText, lexerSymbol.FullIsland, userOpName=userOpName)

            /// Gets the QuickInfo information for the orignal target
            let getTargetSymbolQuickInfo () =
                asyncMaybe {
                    let! targetQuickInfo =
                        checkFileResults.GetStructuredToolTipText
                            (fcsTextLineNumber, idRange.EndColumn, lineText, lexerSymbol.FullIsland, FSharpTokenTag.IDENT, userOpName=userOpName) |> liftAsync

                    match targetQuickInfo with
                    | FSharpToolTipText []
                    | FSharpToolTipText [FSharpStructuredToolTipElement.None] -> return! None
                    | _ ->
                        let! targetTextSpan = RoslynHelpers.TryFSharpRangeToTextSpan (sourceText, lexerSymbol.Range)
                        return { StructuredText = targetQuickInfo
                                 Span = targetTextSpan
                                 Symbol = symbolUse.Symbol
                                 SymbolKind = lexerSymbol.Kind }
                }

            // if the target is in a signature file, adjusting the quick info is unnecessary
            if isSignatureFile document.FilePath then
                let! targetQuickInfo = getTargetSymbolQuickInfo()
                return symbolUse, None, Some targetQuickInfo
            else
                // find the declaration location of the target symbol, with a preference for signature files
                let! findSigDeclarationResult = checkFileResults.GetDeclarationLocation (idRange.StartLine, idRange.EndColumn, lineText, lexerSymbol.FullIsland, preferFlag=true, userOpName=userOpName) |> liftAsync

                // it is necessary to retrieve the backup quick info because this acquires
                // the textSpan designating where we want the quick info to appear.
                let! targetQuickInfo = getTargetSymbolQuickInfo()

                let! result =
                    match findSigDeclarationResult with 
                    | FSharpFindDeclResult.DeclFound declRange when isSignatureFile declRange.FileName ->
                        asyncMaybe {
                            let! sigQuickInfo = getQuickInfoFromRange(checker, projectInfoManager, document, declRange, cancellationToken)

                            // if the target was declared in a signature file, and the current file
                            // is not the corresponding module implementation file for that signature,
                            // the doccoms from the signature will overwrite any doccoms that might be
                            // present on the definition/implementation
                            let! findImplDefinitionResult = checkFileResults.GetDeclarationLocation (idRange.StartLine, idRange.EndColumn, lineText, lexerSymbol.FullIsland, preferFlag=false, userOpName=userOpName) |> liftAsync

                            match findImplDefinitionResult  with
                            | FSharpFindDeclResult.DeclNotFound _
                            | FSharpFindDeclResult.ExternalDecl _ -> return symbolUse, Some sigQuickInfo, None
                            | FSharpFindDeclResult.DeclFound declRange ->
                                let! implQuickInfo = getQuickInfoFromRange(checker, projectInfoManager, document, declRange, cancellationToken)
                                return symbolUse, Some sigQuickInfo, Some { implQuickInfo with Span = targetQuickInfo.Span }
                        }
                    | _ -> async.Return None
                    |> liftAsync

                return result |> Option.defaultValue (symbolUse, None, Some targetQuickInfo)
        }

type internal FSharpAsyncQuickInfoSource
    (
        statusBar: StatusBar,
        xmlMemberIndexService: IVsXMLMemberIndexService,
        checkerProvider:FSharpCheckerProvider,
        projectInfoManager:FSharpProjectOptionsManager,
        textBuffer:ITextBuffer,
        settings: EditorOptions
    ) =

    static let joinWithLineBreaks segments =
        let lineBreak = TaggedTextOps.Literals.lineBreak
        match segments |> List.filter (Seq.isEmpty >> not) with
        | [] -> Seq.empty
        | xs -> xs |> List.reduce (fun acc elem -> seq { yield! acc; yield lineBreak; yield! elem })

    // test helper
    static member ProvideQuickInfo(checker:FSharpChecker, documentId:DocumentId, sourceText:SourceText, filePath:string, position:int, parsingOptions:FSharpParsingOptions, options:FSharpProjectOptions, textVersionHash:int, languageServicePerformanceOptions: LanguageServicePerformanceOptions) =
        asyncMaybe {
            let! _, _, checkFileResults = checker.ParseAndCheckDocument(filePath, textVersionHash, sourceText, options, languageServicePerformanceOptions, userOpName=FSharpQuickInfo.userOpName)
            let textLine = sourceText.Lines.GetLineFromPosition position
            let textLineNumber = textLine.LineNumber + 1 // Roslyn line numbers are zero-based
            let defines = CompilerEnvironment.GetCompilationDefinesForEditing parsingOptions
            let! symbol = Tokenizer.getSymbolAtPosition (documentId, sourceText, position, filePath, defines, SymbolLookupKind.Precise, true)
            let! res = checkFileResults.GetStructuredToolTipText (textLineNumber, symbol.Ident.idRange.EndColumn, textLine.ToString(), symbol.FullIsland, FSharpTokenTag.IDENT, userOpName=FSharpQuickInfo.userOpName) |> liftAsync
            match res with
            | FSharpToolTipText []
            | FSharpToolTipText [FSharpStructuredToolTipElement.None] -> return! None
            | _ ->
                let! symbolUse = checkFileResults.GetSymbolUseAtLocation (textLineNumber, symbol.Ident.idRange.EndColumn, textLine.ToString(), symbol.FullIsland, userOpName=FSharpQuickInfo.userOpName)
                let! symbolSpan = RoslynHelpers.TryFSharpRangeToTextSpan (sourceText, symbol.Range)
                return { StructuredText = res
                         Span = symbolSpan
                         Symbol = symbolUse.Symbol
                         SymbolKind = symbol.Kind }
        }

    static member BuildSingleQuickInfoItem (documentationBuilder:IDocumentationBuilder) (quickInfo:QuickInfo) =
        let mainDescription, documentation, typeParameterMap, usage, exceptions = ResizeArray(), ResizeArray(), ResizeArray(), ResizeArray(), ResizeArray()
        XmlDocumentation.BuildDataTipText(documentationBuilder, mainDescription.Add, documentation.Add, typeParameterMap.Add, usage.Add, exceptions.Add, quickInfo.StructuredText)
        let docs = joinWithLineBreaks [documentation; typeParameterMap; usage; exceptions]
        (mainDescription, docs)

    interface IAsyncQuickInfoSource with
        override __.Dispose() = () // no cleanup necessary

        // This method can be called from the background thread.
        // Do not call IServiceProvider.GetService here.
        override __.GetQuickInfoItemAsync(session:IAsyncQuickInfoSession, cancellationToken:CancellationToken) : Task<QuickInfoItem> =
            // if using LSP, just bail early
            if settings.Advanced.UsePreviewTextHover then Task.FromResult<QuickInfoItem>(null)
            else
            let triggerPoint = session.GetTriggerPoint(textBuffer.CurrentSnapshot)
            match triggerPoint.HasValue with
            | false -> Task.FromResult<QuickInfoItem>(null)
            | true ->
                let triggerPoint = triggerPoint.GetValueOrDefault()
                let documentationBuilder = XmlDocumentation.CreateDocumentationBuilder(xmlMemberIndexService)
                asyncMaybe {
                    let document = textBuffer.CurrentSnapshot.GetOpenDocumentInCurrentContextWithChanges()
                    let! symbolUse, sigQuickInfo, targetQuickInfo = FSharpQuickInfo.getQuickInfo(checkerProvider.Checker, projectInfoManager, document, triggerPoint.Position, cancellationToken)
                    let getTrackingSpan (span:TextSpan) =
                        textBuffer.CurrentSnapshot.CreateTrackingSpan(span.Start, span.Length, SpanTrackingMode.EdgeInclusive)

                    match sigQuickInfo, targetQuickInfo with
                    | None, None -> return null
                    | Some quickInfo, None
                    | None, Some quickInfo->
                        let mainDescription, docs = FSharpAsyncQuickInfoSource.BuildSingleQuickInfoItem documentationBuilder quickInfo
                        let imageId = Tokenizer.GetImageIdForSymbol(quickInfo.Symbol, quickInfo.SymbolKind)
                        let navigation = QuickInfoNavigation(statusBar, checkerProvider.Checker, projectInfoManager, document, symbolUse.RangeAlternate)
                        let content = QuickInfoViewProvider.provideContent(imageId, mainDescription, docs, navigation)
                        let span = getTrackingSpan quickInfo.Span
                        return QuickInfoItem(span, content)

                    | Some sigQuickInfo, Some targetQuickInfo ->
                        let mainDescription, targetDocumentation, sigDocumentation, typeParameterMap, exceptions, usage = ResizeArray(), ResizeArray(), ResizeArray(), ResizeArray(), ResizeArray(), ResizeArray()
                        XmlDocumentation.BuildDataTipText(documentationBuilder, ignore, sigDocumentation.Add, ignore, ignore, ignore, sigQuickInfo.StructuredText)
                        XmlDocumentation.BuildDataTipText(documentationBuilder, mainDescription.Add, targetDocumentation.Add, typeParameterMap.Add, exceptions.Add, usage.Add, targetQuickInfo.StructuredText)
                        // get whitespace nomalized documentation text
                        let getText (tts: seq<Layout.TaggedText>) =
                            let text =
                                (StringBuilder(), tts)
                                ||> Seq.fold (fun sb tt ->
                                    if String.IsNullOrWhiteSpace tt.Text then sb else sb.Append tt.Text)
                                |> string
                            if String.IsNullOrWhiteSpace text then None else Some text

                        let documentation =
                            [ match getText targetDocumentation, getText sigDocumentation with
                              | None, None -> ()
                              | None, Some _ -> yield! sigDocumentation
                              | Some _, None -> yield! targetDocumentation
                              | Some implText, Some sigText when implText.Equals (sigText, StringComparison.OrdinalIgnoreCase) ->
                                    yield! sigDocumentation
                              | Some _  , Some _ ->
                                    yield! joinWithLineBreaks [ sigDocumentation; [ TaggedTextOps.tagText "-------------" ]; targetDocumentation ]
                            ] |> ResizeArray
                        let docs = joinWithLineBreaks [documentation; typeParameterMap; usage; exceptions]
                        let imageId = Tokenizer.GetImageIdForSymbol(targetQuickInfo.Symbol, targetQuickInfo.SymbolKind)
                        let navigation = QuickInfoNavigation(statusBar, checkerProvider.Checker, projectInfoManager, document, symbolUse.RangeAlternate)
                        let content = QuickInfoViewProvider.provideContent(imageId, mainDescription, docs, navigation)
                        let span = getTrackingSpan targetQuickInfo.Span
                        return QuickInfoItem(span, content)
                }   |> Async.map Option.toObj
                    |> RoslynHelpers.StartAsyncAsTask cancellationToken

[<Export(typeof<IAsyncQuickInfoSourceProvider>)>]
[<Name("F# Quick Info Provider")>]
[<ContentType(FSharpConstants.FSharpLanguageName)>]
[<Order>]
type internal FSharpAsyncQuickInfoSourceProvider
    [<ImportingConstructor>]
    (
        [<Import(typeof<SVsServiceProvider>)>] serviceProvider: IServiceProvider,
        checkerProvider:FSharpCheckerProvider,
        projectInfoManager:FSharpProjectOptionsManager,
        settings: EditorOptions
    ) =

    interface IAsyncQuickInfoSourceProvider with
        override __.TryCreateQuickInfoSource(textBuffer:ITextBuffer) : IAsyncQuickInfoSource =
            // GetService calls must be made on the UI thread
            // It is safe to do it here (see #4713)
            let statusBar = StatusBar(serviceProvider.GetService<SVsStatusbar,IVsStatusbar>())
            let xmlMemberIndexService = serviceProvider.XMLMemberIndexService
            new FSharpAsyncQuickInfoSource(statusBar, xmlMemberIndexService, checkerProvider, projectInfoManager, textBuffer, settings) :> IAsyncQuickInfoSource
