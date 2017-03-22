// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Threading
open System.Threading.Tasks
open System.Windows
open System.Windows.Controls
open System.ComponentModel.Composition

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Editor.Shared.Utilities
open Microsoft.CodeAnalysis.Editor.Shared.Extensions
open Microsoft.CodeAnalysis.Editor.Implementation.IntelliSense.QuickInfo
open Microsoft.CodeAnalysis.Text

open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.Utilities
open Microsoft.VisualStudio.Language.Intellisense

open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.CompileOps
open  Microsoft.VisualStudio.FSharp.Editor.Logging
open CommonRoslynHelpers


module private SessionHandling =
    let mutable currentSession = None
    
    [<Export(typeof<IQuickInfoSourceProvider>)>]
    [<Name("Session Capturing Quick Info Source Provider")>]
    [<Order(After = PredefinedQuickInfoProviderNames.Semantic)>]
    [<ContentType(FSharpCommonConstants.FSharpContentTypeName)>]
    type SourceProviderForCapturingSession() =
            interface IQuickInfoSourceProvider with 
                member x.TryCreateQuickInfoSource _ =
                  { new IQuickInfoSource with
                    member __.AugmentQuickInfoSession(session,_,_) = currentSession <- Some session
                    member __.Dispose() = () }
    
[<ExportQuickInfoProvider(PredefinedQuickInfoProviderNames.Semantic, FSharpCommonConstants.FSharpLanguageName)>]
type internal FSharpQuickInfoProvider 
    [<System.ComponentModel.Composition.ImportingConstructor>] 
    (
        [<System.ComponentModel.Composition.Import(typeof<SVsServiceProvider>)>] serviceProvider: IServiceProvider,
        checkerProvider: FSharpCheckerProvider,
        projectInfoManager: ProjectInfoManager,
        typeMap: Shared.Utilities.ClassificationTypeMap,
        gotoDefinitionService:FSharpGoToDefinitionService,
        glyphService: IGlyphService
    ) =

    let fragment (content: Layout.TaggedText seq, typemap: ClassificationTypeMap, initialDoc: Document) : IDeferredQuickInfoContent =

        let workspace = initialDoc.Project.Solution.Workspace
        let solution = workspace.CurrentSolution


        /// Retrieve the DocumentId from the workspace for the range's file
        let docIdOfRange (range: range) =
            let filePath = System.IO.Path.GetFullPathSafe range.FileName

            //The same file may be present in many projects. We choose one from current or referenced project.
            let rec matchingDoc = function
            | [] -> None
            | (docId:DocumentId)::_ when docId.ProjectId = initialDoc.Id.ProjectId || IsScript initialDoc.FilePath -> Some docId
            | docId::tail -> 
                if solution.GetDependentProjectIds(docId.ProjectId).Contains initialDoc.Id.ProjectId then Some docId
                else matchingDoc tail
            solution.GetDocumentIdsWithFilePath filePath |> List.ofSeq |> matchingDoc


        let canGoTo range =
            range <> rangeStartup && docIdOfRange range |> Option.isSome

        // to ensure proper navigation decsions we need to check the type of document the navigation call
        // is originating from and the target we're provided by default
        //  - signature files (.fsi) should navigate to other signature files 
        //  - implementation files (.fs) should navigate to other implementation files
        //let adjustTarget (range:range) =

        let navigateTo (range:range) = 
            maybe { 
                let targetPath = range.FileName 
                let! targetId = docIdOfRange range
                let targetDoc = solution.GetDocument targetId 
                let targetSource = targetDoc.GetTextAsync() |> Async.RunTaskSynchronously
                let! targetTextSpan = CommonRoslynHelpers.TryFSharpRangeToTextSpan (targetSource, range)

                let navigateWith navfn args = 
                    if navfn args then 
                        SessionHandling.currentSession
                        |> Option.iter(fun session -> session.Dismiss())

                match isSignatureFile initialDoc.FilePath, isSignatureFile targetPath with 
                | true, true 
                | false, false ->
                    navigateWith gotoDefinitionService.TryNavigateToTextSpan (targetDoc, targetTextSpan) 
                // adjust the target from signature to implementation
                | false, true ->
                    navigateWith gotoDefinitionService.TryNavigateToSymbolDefinition 
                                (targetDoc, targetSource, range, Async.DefaultCancellationToken)
                // adjust the target from implmentation to signature
                | true, false -> 
                    navigateWith gotoDefinitionService.TryNavigateToSymbolDeclaration
                                (targetDoc, targetSource, range, Async.DefaultCancellationToken)
            } |> ignore

        let formatMap = typemap.ClassificationFormatMapService.GetClassificationFormatMap "tooltip"

        let layoutTagToFormatting =
            roslynTag
            >> ClassificationTags.GetClassificationTypeName
            >> typemap.GetClassificationType
            >> formatMap.GetTextProperties

        let inlines = seq { 
            for taggedText in content do
                let run = Documents.Run(taggedText.Text, ToolTip = taggedText.Tag ) :> Documents.Inline
                let inl =
                    match taggedText with
                    | :? Layout.NavigableTaggedText as nav when canGoTo nav.Range ->
                        let h = Documents.Hyperlink run
                        //h.ToolTip <- nav.FullName + "\n" + nav.Range.FileName
                        h.Click.Add <| fun _ -> navigateTo nav.Range
                        h :> Documents.Inline
                    //| :? Layout.NavigableTaggedText as nav ->
                    //    run.ToolTip <- nav.FullName
                    //    run :> Documents.Inline
                    | _ -> run
                DependencyObjectExtensions.SetTextProperties(inl, layoutTagToFormatting taggedText.Tag)
                yield inl
        }

        let createTextLinks () =
            let tb = TextBlock(TextWrapping = TextWrapping.Wrap, TextTrimming = TextTrimming.None)
            DependencyObjectExtensions.SetDefaultTextProperties(tb, formatMap)
            tb.Inlines.AddRange(inlines)
            if tb.Inlines.Count = 0 then tb.Visibility <- Visibility.Collapsed
            tb :> FrameworkElement
            
        { new IDeferredQuickInfoContent with member x.Create() = createTextLinks() }

    let tooltip(symbolGlyph, mainDescription, documentation) =

        let empty = 
          { new IDeferredQuickInfoContent with 
            member x.Create() = TextBlock(Visibility = Visibility.Collapsed) :> FrameworkElement }

        let roslynQuickInfo = QuickInfoDisplayDeferredContent(symbolGlyph, null, mainDescription, documentation, empty, empty, empty, empty)

        let create() =
            let qi = roslynQuickInfo.Create()
            let style = Style(typeof<Documents.Hyperlink>)
            style.Setters.Add(Setter(Documents.Inline.TextDecorationsProperty, null))
            let trigger = DataTrigger(Binding = Data.Binding("IsMouseOver", Source = qi), Value = true)
            trigger.Setters.Add(Setter(Documents.Inline.TextDecorationsProperty, TextDecorations.Underline))
            style.Triggers.Add(trigger)
            qi.Resources.Add(typeof<Documents.Hyperlink>, style)
            qi

        { new IDeferredQuickInfoContent with member x.Create() = create() }

    let xmlMemberIndexService = serviceProvider.GetService(typeof<SVsXMLMemberIndexService>) :?> IVsXMLMemberIndexService
    let documentationBuilder = XmlDocumentation.CreateDocumentationBuilder(xmlMemberIndexService, serviceProvider.DTE)
    
    static member ProvideQuickInfo(checker: FSharpChecker, documentId: DocumentId, sourceText: SourceText, filePath: string, position: int, options: FSharpProjectOptions, textVersionHash: int) =
        asyncMaybe {
            let! _, _, checkFileResults = checker.ParseAndCheckDocument(filePath, textVersionHash, sourceText.ToString(), options, allowStaleResults = true)
            let textLine = sourceText.Lines.GetLineFromPosition(position)
            let textLineNumber = textLine.LineNumber + 1 // Roslyn line numbers are zero-based
            let defines = CompilerEnvironment.GetCompilationDefinesForEditing(filePath, options.OtherOptions |> Seq.toList)
            let! symbol = CommonHelpers.getSymbolAtPosition(documentId, sourceText, position, filePath, defines, SymbolLookupKind.Precise)
            let! res = checkFileResults.GetStructuredToolTipTextAlternate(textLineNumber, symbol.Ident.idRange.EndColumn, textLine.ToString(), symbol.FullIsland, FSharpTokenTag.IDENT) |> liftAsync
            match res with
            | FSharpToolTipText [] 
            | FSharpToolTipText [FSharpStructuredToolTipElement.None] -> return! None
            | _ -> 
                let! symbolUse = checkFileResults.GetSymbolUseAtLocation(textLineNumber, symbol.Ident.idRange.EndColumn, textLine.ToString(), symbol.FullIsland)
                return! Some(res, CommonRoslynHelpers.FSharpRangeToTextSpan(sourceText, symbol.Range), symbolUse.Symbol, symbol.Kind)
        }
    
    interface IQuickInfoProvider with
        override this.GetItemAsync(document: Document, position: int, cancellationToken: CancellationToken): Task<QuickInfoItem> =
            asyncMaybe {
                let! sourceText = document.GetTextAsync(cancellationToken)
                let defines = projectInfoManager.GetCompilationDefinesForEditingDocument(document)  
                let! _ = CommonHelpers.getSymbolAtPosition(document.Id, sourceText, position, document.FilePath, defines, SymbolLookupKind.Precise)
                let! options = projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document)
                let! textVersion = document.GetTextVersionAsync(cancellationToken)
                let! toolTipElement, textSpan, symbol, symbolKind = 
                    FSharpQuickInfoProvider.ProvideQuickInfo(checkerProvider.Checker, document.Id, sourceText, document.FilePath, position, options, textVersion.GetHashCode())
                let mainDescription = Collections.Generic.List()
                let documentation = Collections.Generic.List()
                XmlDocumentation.BuildDataTipText(
                    documentationBuilder, 
                    mainDescription.Add, 
                    documentation.Add, 
                    toolTipElement)
                let content = 
                    tooltip
                        (
                            SymbolGlyphDeferredContent(CommonRoslynHelpers.GetGlyphForSymbol(symbol, symbolKind), glyphService),
                            fragment(mainDescription, typeMap, document),
                            fragment(documentation, typeMap, document)
                        )
                return QuickInfoItem(textSpan, content)
            } 
            |> Async.map Option.toObj
            |> CommonRoslynHelpers.StartAsyncAsTask(cancellationToken)
