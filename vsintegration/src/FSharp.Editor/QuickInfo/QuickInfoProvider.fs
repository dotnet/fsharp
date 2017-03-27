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
open Microsoft.CodeAnalysis.Navigation
open Microsoft.CodeAnalysis.Text

open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.Utilities
open Microsoft.VisualStudio.Language.Intellisense

open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.CompileOps

open CommonRoslynHelpers

module internal FSharpQuickInfo =
    
    [<Literal>]
    let SessionCapturingProviderName = "Session Capturing Quick Info Source Provider"

    let mutable currentSession = None

    [<Export(typeof<IQuickInfoSourceProvider>)>]
    [<Name(SessionCapturingProviderName)>]
    [<Order(After = PredefinedQuickInfoProviderNames.Semantic)>]
    [<ContentType(FSharpCommonConstants.FSharpContentTypeName)>]
    type SourceProviderForCapturingSession() =
        interface IQuickInfoSourceProvider with 
            member __.TryCreateQuickInfoSource _ =
              { new IQuickInfoSource with
                member __.AugmentQuickInfoSession(session,_,_) = currentSession <- Some session
                member __.Dispose() = () }

    let fragment(content, typemap: ClassificationTypeMap, thisDoc: Document) =

        let workspace = thisDoc.Project.Solution.Workspace
        let documentNavigationService = workspace.Services.GetService<IDocumentNavigationService>()
        let solution = workspace.CurrentSolution

        let documentId (range: range) =
            let filePath = System.IO.Path.GetFullPathSafe range.FileName
            let projectOf (id : DocumentId) = solution.GetDocument(id).Project

            //The same file may be present in many projects. We choose one from current or referenced project.
            let rec matchingDoc = function
            | [] -> None
            | id::_ when projectOf id = thisDoc.Project || IsScript thisDoc.FilePath -> Some id
            | id::tail -> 
                if (projectOf id).GetDependentProjects() |> Seq.contains thisDoc.Project then Some id
                else matchingDoc tail
            solution.GetDocumentIdsWithFilePath(filePath) |> List.ofSeq |> matchingDoc

        let canGoTo range =
            range <> rangeStartup && documentId range |> Option.isSome

        let goTo range = 
            asyncMaybe { 
                let! id = documentId range
                let! src = solution.GetDocument(id).GetTextAsync()
                let! span = CommonRoslynHelpers.TryFSharpRangeToTextSpan(src, range)
                if documentNavigationService.TryNavigateToSpan(workspace, id, span) then
                   let! session = currentSession
                   session.Dismiss()   
            } |> Async.Ignore |> Async.StartImmediate 

        let formatMap = typemap.ClassificationFormatMapService.GetClassificationFormatMap("tooltip")

        let props = 
            ClassificationTags.GetClassificationTypeName
            >> typemap.GetClassificationType
            >> formatMap.GetTextProperties

        let inlines = seq { 
            for (tag, text, rangeOpt) in content do
                let run =
                    match rangeOpt with
                    | Some(range) when canGoTo range ->
                        let h = Documents.Hyperlink(Documents.Run(text), ToolTip = range.FileName)
                        h.Click.Add <| fun _ -> goTo range          
                        h :> Documents.Inline
                    | _ -> 
                        Documents.Run(text) :> Documents.Inline
                DependencyObjectExtensions.SetTextProperties(run, props tag)
                yield run
        }

        let create() =
            let tb = TextBlock(TextWrapping = TextWrapping.Wrap, TextTrimming = TextTrimming.None)
            DependencyObjectExtensions.SetDefaultTextProperties(tb, formatMap)
            tb.Inlines.AddRange(inlines)
            if tb.Inlines.Count = 0 then tb.Visibility <- Visibility.Collapsed
            tb :> FrameworkElement
            
        { new IDeferredQuickInfoContent with member x.Create() = create() }

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

[<ExportQuickInfoProvider(PredefinedQuickInfoProviderNames.Semantic, FSharpCommonConstants.FSharpLanguageName)>]
type internal FSharpQuickInfoProvider 
    [<System.ComponentModel.Composition.ImportingConstructor>] 
    (
        [<System.ComponentModel.Composition.Import(typeof<SVsServiceProvider>)>] serviceProvider: IServiceProvider,
        checkerProvider: FSharpCheckerProvider,
        projectInfoManager: ProjectInfoManager,
        typeMap: Shared.Utilities.ClassificationTypeMap,
        glyphService: IGlyphService
    ) =

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
                    CommonRoslynHelpers.CollectNavigableText mainDescription, 
                    CommonRoslynHelpers.CollectNavigableText documentation, 
                    toolTipElement)
                let content = 
                    FSharpQuickInfo.tooltip
                        (
                            SymbolGlyphDeferredContent(CommonRoslynHelpers.GetGlyphForSymbol(symbol, symbolKind), glyphService),
                            FSharpQuickInfo.fragment(mainDescription, typeMap, document),
                            FSharpQuickInfo.fragment(documentation, typeMap, document)
                        )
                return QuickInfoItem(textSpan, content)
            } 
            |> Async.map Option.toObj
            |> CommonRoslynHelpers.StartAsyncAsTask(cancellationToken)
