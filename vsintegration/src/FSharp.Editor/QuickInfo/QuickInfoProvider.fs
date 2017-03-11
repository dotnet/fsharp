// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Editor.Implementation.IntelliSense.QuickInfo
open Microsoft.CodeAnalysis.Navigation
open Microsoft.CodeAnalysis.Text

open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.Language.Intellisense

open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.Range

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

    let navigationFrom (thisDoc: Document) =
        let workspace = thisDoc.Project.Solution.Workspace
        let documentNavigationService = workspace.Services.GetService<IDocumentNavigationService>()
        let solution = workspace.CurrentSolution

        let documentId (range: range) =
            let filePath = System.IO.Path.GetFullPathSafe range.FileName
            let projectOf (id : DocumentId) = solution.GetDocument(id).Project

            //The same file may be present in many projects. We choose one from current or referenced project.
            let rec matchingDoc = function
            | [] -> None
            | id::_ when projectOf id = thisDoc.Project -> Some id
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
                return documentNavigationService.TryNavigateToSpan(workspace, id, span)
            } |> Async.Ignore

        canGoTo, goTo
    
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
                let empty = EmptyQuickInfoContent()
                let canGoTo, goTo = navigationFrom document
                let content = 
                    QuickInfoDisplayDeferredContent
                        (
                            symbolGlyph = SymbolGlyphDeferredContent(CommonRoslynHelpers.GetGlyphForSymbol(symbol, symbolKind), glyphService),
                            warningGlyph = null,
                            mainDescription = FSharpDeferredContent(mainDescription, typeMap, canGoTo, goTo),
                            documentation = FSharpDeferredContent(documentation, typeMap, canGoTo, goTo),
                            typeParameterMap = empty,
                            anonymousTypes = empty,
                            usageText = empty,
                            exceptionText = empty
                        )
                return QuickInfoItem(textSpan, content)
            } 
            |> Async.map Option.toObj
            |> CommonRoslynHelpers.StartAsyncAsTask(cancellationToken)
