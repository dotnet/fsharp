// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Collections.Generic
open System.Collections.Immutable
open System.Linq
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Editor.Host
open Microsoft.CodeAnalysis.Navigation
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.Text

open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.VisualStudio.FSharp.Editor.Logging
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop

type internal FSharpNavigableItem(document: Document, textSpan: TextSpan) =

    interface INavigableItem with
        member this.Glyph = Glyph.BasicFile
        member this.DisplayFileLocation = true
        member this.IsImplicitlyDeclared = false
        member this.Document = document
        member this.SourceSpan = textSpan
        member this.DisplayTaggedParts = ImmutableArray<TaggedText>.Empty
        member this.ChildItems = ImmutableArray<INavigableItem>.Empty

[<Shared>]
[<ExportLanguageService(typeof<IGoToDefinitionService>, FSharpCommonConstants.FSharpLanguageName)>]
[<Export(typeof<FSharpGoToDefinitionService>)>]
type internal FSharpGoToDefinitionService 
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider,
        projectInfoManager: ProjectInfoManager,
        [<ImportMany>]presenters: IEnumerable<INavigableItemsPresenter>
    ) =
    let serviceProvider =  ServiceProvider.GlobalProvider  
    let statusBar = serviceProvider.GetService<SVsStatusbar,IVsStatusbar>()

    static member FindDefinition
        (checker: FSharpChecker, documentKey: DocumentId, sourceText: SourceText, filePath: string, position: int,
         defines: string list, options: FSharpProjectOptions, textVersionHash: int) : Option<range> = maybe {
            let textLine = sourceText.Lines.GetLineFromPosition position
            let textLinePos = sourceText.Lines.GetLinePosition position
            let fcsTextLineNumber = Line.fromZ textLinePos.Line
            let! symbol = CommonHelpers.getSymbolAtPosition(documentKey, sourceText, position, filePath, defines, SymbolLookupKind.Greedy)
            let! _, _, checkFileResults = 
                checker.ParseAndCheckDocument 
                    (filePath, textVersionHash, sourceText.ToString(), options, allowStaleResults = true)  |> Async.RunSynchronously

            let declarations = 
                checkFileResults.GetDeclarationLocationAlternate 
                    (fcsTextLineNumber, symbol.Ident.idRange.EndColumn, textLine.ToString(), symbol.FullIsland, false) |> Async.RunSynchronously
            
            match declarations with
            | FSharpFindDeclResult.DeclFound range -> return range
            | _ -> return! None
    }
    
    // FSROSLYNTODO: Since we are not integrated with the Roslyn project system yet, the below call
    // document.Project.Solution.GetDocumentIdsWithFilePath() will only access files in the same project.
    // Either Roslyn INavigableItem needs to be extended to allow arbitary full paths, or we need to
    // fully integrate with their project system.
    member this.FindDefinitionsTask(document: Document, position: int, cancellationToken: CancellationToken) =
        let findDefinition () =
            try maybe {
                let results = List<INavigableItem>()
                let! options = projectInfoManager.TryGetOptionsForEditingDocumentOrProject document
                let sourceText = document.GetTextAsync cancellationToken |> Async.RunTaskSynchronously
                let textVersion = document.GetTextVersionAsync cancellationToken |> Async.RunTaskSynchronously
                let defines = CompilerEnvironment.GetCompilationDefinesForEditing (document.Name, options.OtherOptions |> Seq.toList)
                let! range = FSharpGoToDefinitionService.FindDefinition 
                                (checkerProvider.Checker, document.Id, sourceText, document.FilePath, position, defines, options, textVersion.GetHashCode())
                // REVIEW: 
                let fileName = try System.IO.Path.GetFullPath range.FileName with _ -> range.FileName
                let refDocumentIds = document.Project.Solution.GetDocumentIdsWithFilePath fileName
                if not refDocumentIds.IsEmpty then 
                    let refDocumentId = refDocumentIds.First()
                    let refDocument = document.Project.Solution.GetDocument refDocumentId
                    let refSourceText = refDocument.GetTextAsync cancellationToken |> Async.RunTaskSynchronously
                    let refTextSpan = CommonRoslynHelpers.FSharpRangeToTextSpan (refSourceText, range)
                    results.Add (FSharpNavigableItem (refDocument, refTextSpan))
                return results.AsEnumerable()
                } |> Option.defaultValue Seq.empty
            with e ->
                debug "\n%s\n%s\n%s\n" e.Message (e.TargetSite.ToString()) e.StackTrace
                Seq.empty
        let definitionTask = new Task<_>(findDefinition, cancellationToken) : INavigableItem seq Task
        definitionTask

    member this.TryGoToDefinition(document: Document, position: int, cancellationToken: CancellationToken) =
        let definitionTask = this.FindDefinitionsTask (document, position, cancellationToken)
            
        // Running the task synchronously improves the speed significantly
        // UI locks are not an issue thanks to the cancellation token
        definitionTask.RunSynchronously ()

        if definitionTask.Status = TaskStatus.RanToCompletion && definitionTask.Result.Any() then
            let navigableItem = definitionTask.Result.First() // F# API provides only one INavigableItem
            let workspace = document.Project.Solution.Workspace
            let navigationService = workspace.Services.GetService<IDocumentNavigationService>()
            ignore presenters
            // prefer open documents in the preview tab
            let options = workspace.Options.WithChangedOption (NavigationOptions.PreferProvisionalTab, true)
            navigationService.TryNavigateToSpan (workspace, navigableItem.Document.Id, navigableItem.SourceSpan, options)

            // FSROSLYNTODO: potentially display multiple results here
            // If GotoDef returns one result then it should try to jump to a discovered location. If it returns multiple results then it should use 
            // presenters to render items so user can choose whatever he needs. Given that per comment F# API always returns only one item then we 
            // should always navigate to definition and get rid of presenters.
            //
            //let refDisplayString = refSourceText.GetSubText(refTextSpan).ToString()
            //for presenter in presenters do
            //    presenter.DisplayResult(navigableItem.DisplayString, definitionTask.Result)
            //true

        else 
            statusBar.SetText "Could Not Navigate to Definition of Symbol Under Caret" |> ignore
            true
        
    interface IGoToDefinitionService with
        member this.FindDefinitionsAsync(document: Document, position: int, cancellationToken: CancellationToken) =
            this.FindDefinitionsTask (document, position, cancellationToken)
        
        member this.TryGoToDefinition(document: Document, position: int, cancellationToken: CancellationToken) =
            this.TryGoToDefinition (document,position,cancellationToken)
