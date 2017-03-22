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


module internal FSharpGoToDefinition =
  
    let checkAndFindDefinition
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


    let rangeToNavigableItem (range:range, document:Document, cancellationToken:CancellationToken) =
        let fileName = try System.IO.Path.GetFullPath range.FileName with _ -> range.FileName
        let refDocumentIds = document.Project.Solution.GetDocumentIdsWithFilePath fileName
        if not refDocumentIds.IsEmpty then 
            let refDocumentId = refDocumentIds.First()
            let refDocument = document.Project.Solution.GetDocument refDocumentId
            let refSourceText = refDocument.GetTextAsync cancellationToken |> Async.RunTaskSynchronously
            let refTextSpan = CommonRoslynHelpers.FSharpRangeToTextSpan (refSourceText, range)
            Some (FSharpNavigableItem (refDocument, refTextSpan))
        else None


    let findDefinition (document: Document, position: int, checker:FSharpChecker, projectInfoManager:ProjectInfoManager, cancellationToken: CancellationToken) : FSharpNavigableItem option =
        maybe {
            let! options = projectInfoManager.TryGetOptionsForEditingDocumentOrProject document
            let sourceText = document.GetTextAsync cancellationToken |> Async.RunTaskSynchronously
            let textVersion = document.GetTextVersionAsync cancellationToken |> Async.RunTaskSynchronously
            let defines = CompilerEnvironment.GetCompilationDefinesForEditing (document.Name, options.OtherOptions |> Seq.toList)
            let! range = checkAndFindDefinition
                            (checker, document.Id, sourceText, document.FilePath, position, defines, options, textVersion.GetHashCode())
            return! rangeToNavigableItem (range, document, cancellationToken)
        }
    

    let private findSymbolHelper 
        (document:Document, range:range, sourceText:SourceText, preferSignature:bool, checker: FSharpChecker, projectInfoManager: ProjectInfoManager,cancellationToken) =
        asyncMaybe {
            let! projectOptions = projectInfoManager.TryGetOptionsForEditingDocumentOrProject document
            let defines = CompilerEnvironment.GetCompilationDefinesForEditing (document.FilePath, projectOptions.OtherOptions |> Seq.toList)

            let originTextSpan = CommonRoslynHelpers.FSharpRangeToTextSpan (sourceText, range)
            let position = originTextSpan.Start
            let! lexerSymbol = 
                CommonHelpers.getSymbolAtPosition 
                    (document.Id, sourceText, position, document.FilePath, defines, SymbolLookupKind.Greedy)

            let lineText = (sourceText.Lines.GetLineFromPosition position).ToString() 

            let! _, _, checkFileResults = 
                checker.ParseAndCheckDocument (document,projectOptions,allowStaleResults=true,sourceText=sourceText)
            let idRange = lexerSymbol.Ident.idRange
            let! findSigDeclarationResult = 
                checkFileResults.GetDeclarationLocationAlternate
                    (idRange.StartLine, idRange.EndColumn, lineText, lexerSymbol.FullIsland, preferFlag=preferSignature)  |> liftAsync
            let! targetRange =
                match findSigDeclarationResult with 
                | FSharpFindDeclResult.DeclNotFound _failReason -> None 
                | FSharpFindDeclResult.DeclFound declRange -> Some declRange 
            
            let! targetDocument = document.Project.Solution.TryGetDocumentFromFSharpRange(targetRange,document.Project.Id)
            return! rangeToNavigableItem (targetRange, targetDocument, cancellationToken)
        }  |> Async.RunSynchronously


    // find the declaration location (signature file/.fsi) of the target symbol if possible, fall back to definition 
    let findDeclarationOfSymbolAtRange
        (document:Document, range:range, sourceText:SourceText, checker: FSharpChecker, projectInfoManager: ProjectInfoManager,cancellationToken) =
        findSymbolHelper (document, range, sourceText,true, checker, projectInfoManager,cancellationToken) 




    // find the definition location (implementation file/.fs) of the target symbol
    let findDefinitionOfSymbolAtRange
        (document:Document, range:range, sourceText:SourceText, checker: FSharpChecker, projectInfoManager: ProjectInfoManager,cancellationToken) =
        findSymbolHelper (document, range, sourceText,false, checker, projectInfoManager,cancellationToken)



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

    let createNavigationTask navFunction args (cancellationToken:CancellationToken) = 
        let navAction () : seq<INavigableItem> =
            try let results = List<INavigableItem>()
                match navFunction args with
                | None -> results.AsEnumerable()
                | Some navItem -> results.Add navItem; results.AsEnumerable()
            with e ->
                debug "\n%s\n%s\n%s\n" e.Message (e.TargetSite.ToString()) e.StackTrace
                Seq.empty
        let definitionTask = new Task<_>(navAction, cancellationToken) : INavigableItem seq Task
        definitionTask


    let attemptNavigation (document:Document, navigationTask:Task<seq<INavigableItem>>) =
        // Running the task synchronously improves the speed significantly
        // UI locks are not an issue thanks to the cancellation token
        navigationTask.RunSynchronously ()

        if navigationTask.Status = TaskStatus.RanToCompletion && navigationTask.Result.Any() then
            let navigableItem = navigationTask.Result.First() // F# API provides only one INavigableItem
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


    member this.TryNavigateToTextSpan (document:Document, textSpan:TextSpan) =
        let navigableItem = FSharpNavigableItem (document, textSpan) :> INavigableItem
        let workspace = document.Project.Solution.Workspace
        let navigationService = workspace.Services.GetService<IDocumentNavigationService>()
        let options = workspace.Options.WithChangedOption (NavigationOptions.PreferProvisionalTab, true)
        let result = navigationService.TryNavigateToSpan (workspace, navigableItem.Document.Id, navigableItem.SourceSpan, options)
        if result then true else
        statusBar.SetText "Could Not Navigate to Definition of Symbol Under Caret" |> ignore
        false



    member this.TryNavigateToSymbolDeclaration (originDocument:Document,originSourceText:SourceText,symbolRange:range,cancellationToken)=
        let declarationTask = 
            createNavigationTask FSharpGoToDefinition.findDeclarationOfSymbolAtRange 
                                 (originDocument, symbolRange, originSourceText, checkerProvider.Checker, projectInfoManager,cancellationToken) 
                                 cancellationToken
        attemptNavigation (originDocument, declarationTask)
        
    

    member this.TryNavigateToSymbolDefinition (originDocument:Document,originSourceText:SourceText,symbolRange:range,cancellationToken)=
        let definitionTask = 
            createNavigationTask FSharpGoToDefinition.findDefinitionOfSymbolAtRange 
                                 (originDocument, symbolRange, originSourceText, checkerProvider.Checker, projectInfoManager,cancellationToken) 
                                 cancellationToken
        attemptNavigation (originDocument, definitionTask)


    member this.FindDefinitionsTask (document, position, cancellationToken) =
        createNavigationTask FSharpGoToDefinition.findDefinition 
                            (document, position,checkerProvider.Checker,projectInfoManager, cancellationToken)
                            cancellationToken


    member this.TryGoToDefinition(document: Document, position: int, cancellationToken: CancellationToken) =
        attemptNavigation (document , this.FindDefinitionsTask (document,position,cancellationToken))

        
    interface IGoToDefinitionService with
        
        member this.FindDefinitionsAsync(document: Document, position: int, cancellationToken: CancellationToken) =
            this.FindDefinitionsTask (document, position, cancellationToken)
        

        member this.TryGoToDefinition(document: Document, position: int, cancellationToken: CancellationToken) =
            this.TryGoToDefinition (document,position,cancellationToken)
