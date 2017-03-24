// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.IO
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
  
    /// Parse and check the provided document and try to find the defition of the symbol at the position
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

    /// Use an origin document to provide the solution & workspace used to 
    /// find the corresponding textSpan and INavigableItem for the range
    let rangeToNavigableItem (range:range, document:Document) = async {
        let fileName = try System.IO.Path.GetFullPath range.FileName with _ -> range.FileName
        let refDocumentIds = document.Project.Solution.GetDocumentIdsWithFilePath fileName
        if not refDocumentIds.IsEmpty then 
            let refDocumentId = refDocumentIds.First()
            let refDocument = document.Project.Solution.GetDocument refDocumentId
            let! refSourceText = refDocument.GetTextAsync()
            let refTextSpan = CommonRoslynHelpers.FSharpRangeToTextSpan (refSourceText, range)
            return Some (FSharpNavigableItem (refDocument, refTextSpan))
        else return None
    }

    /// Find the defition of the Symbol located at the provided position in the document
    let findDefinition (document: Document, position: int, checker:FSharpChecker, projectInfoManager:ProjectInfoManager)  =
        asyncMaybe {
            let! options = projectInfoManager.TryGetOptionsForEditingDocumentOrProject document
            let! sourceText = document.GetTextAsync ()
            let! textVersion = document.GetTextVersionAsync ()
            let defines = CompilerEnvironment.GetCompilationDefinesForEditing (document.Name, options.OtherOptions |> Seq.toList)
            let! range = checkAndFindDefinition
                            (checker, document.Id, sourceText, document.FilePath, position, defines, options, textVersion.GetHashCode())
            return! rangeToNavigableItem (range, document)
        }
    

    /// helper function that used to determine the navigation strategy to apply, can be tuned towards signatures or implementation files
    let private findSymbolHelper 
        (originDocument:Document, range:range, sourceText:SourceText, preferSignature:bool, checker: FSharpChecker, projectInfoManager: ProjectInfoManager) =
        asyncMaybe {
            let! projectOptions = projectInfoManager.TryGetOptionsForEditingDocumentOrProject originDocument
            let defines = CompilerEnvironment.GetCompilationDefinesForEditing (originDocument.FilePath, projectOptions.OtherOptions |> Seq.toList)

            let originTextSpan = CommonRoslynHelpers.FSharpRangeToTextSpan (sourceText, range)
            let position = originTextSpan.Start

            let! lexerSymbol = 
                CommonHelpers.getSymbolAtPosition 
                    (originDocument.Id, sourceText, position, originDocument.FilePath, defines, SymbolLookupKind.Greedy)
            
            let textLinePos = sourceText.Lines.GetLinePosition position
            let fcsTextLineNumber = Line.fromZ textLinePos.Line
            let lineText = (sourceText.Lines.GetLineFromPosition position).ToString()  
            
            let! _, _, checkFileResults = 
                checker.ParseAndCheckDocument (originDocument,projectOptions,allowStaleResults=true,sourceText=sourceText)
            let idRange = lexerSymbol.Ident.idRange

            let! fsSymbolUse = checkFileResults.GetSymbolUseAtLocation (fcsTextLineNumber, idRange.EndColumn, lineText, lexerSymbol.FullIsland)
            let symbol = fsSymbolUse.Symbol
            
            // if the tooltip was spawned in an implementation file and we have a range targeting
            // a signature file, try to find the corresponding implementation file and target the
            // desired symbol
            if isSignatureFile fsSymbolUse.FileName && preferSignature = false then 
                let fsfilePath = Path.ChangeExtension (range.FileName,"fs")
                if not (File.Exists fsfilePath) then return! None else
                let! implDoc = originDocument.Project.Solution.TryGetDocumentFromPath fsfilePath
                let! implSourceText = implDoc.GetTextAsync ()
                let! projectOptions = projectInfoManager.TryGetOptionsForEditingDocumentOrProject implDoc
                let! _, _, checkFileResults = 
                    checker.ParseAndCheckDocument (implDoc, projectOptions, allowStaleResults=true, sourceText=implSourceText)
                let! symbolUses = checkFileResults.GetUsesOfSymbolInFile symbol |> liftAsync
                let! implSymbol  = symbolUses |> Array.tryHead 
                let implTextSpan = CommonRoslynHelpers.FSharpRangeToTextSpan (implSourceText, implSymbol.RangeAlternate)
                return FSharpNavigableItem (implDoc, implTextSpan)
            else
                let! targetDocument = originDocument.Project.Solution.TryGetDocumentFromFSharpRange fsSymbolUse.RangeAlternate
                return! rangeToNavigableItem (fsSymbolUse.RangeAlternate, targetDocument)
        }  


    /// find the declaration location (signature file/.fsi) of the target symbol if possible, fall back to definition 
    let findDeclarationOfSymbolAtRange
        (targetDocument:Document, symbolRange:range, targetSource:SourceText, checker: FSharpChecker, projectInfoManager: ProjectInfoManager) =
        findSymbolHelper (targetDocument, symbolRange, targetSource,true, checker, projectInfoManager) 


    /// find the definition location (implementation file/.fs) of the target symbol
    let findDefinitionOfSymbolAtRange
        (targetDocument:Document, symbolRange:range, targetSourceText:SourceText, checker: FSharpChecker, projectInfoManager: ProjectInfoManager) =
        findSymbolHelper (targetDocument, symbolRange, targetSourceText,false, checker, projectInfoManager)



[<Shared>]
[<ExportLanguageService(typeof<IGoToDefinitionService>, FSharpCommonConstants.FSharpLanguageName)>]
[<Export(typeof<FSharpGoToDefinitionService>)>]
type internal FSharpGoToDefinitionService [<ImportingConstructor>]
    (checkerProvider: FSharpCheckerProvider,
     projectInfoManager: ProjectInfoManager,
     [<ImportMany>] presenters: IEnumerable<INavigableItemsPresenter>) =

    let serviceProvider =  ServiceProvider.GlobalProvider  
    let statusBar = serviceProvider.GetService<SVsStatusbar,IVsStatusbar>()

    /// helper composition function to construct a task from a function that returns an Async Computation and its arguments
    let createNavigationTask (navFunction: _ -> Async<FSharpNavigableItem  option>) args (cancellationToken:CancellationToken) = 
        asyncMaybe {
            try let navList = List<INavigableItem>()
                let! navItem = navFunction args 
                navList.Add navItem 
                return navList.AsEnumerable()
            with e ->
                debug "\n%s\n%s\n%s\n" e.Message (e.TargetSite.ToString()) e.StackTrace
                return Seq.empty
        }   
        |> Async.map (Option.defaultValue Seq.empty)
        |> fun comp -> Async.StartAsTask(comp,cancellationToken=cancellationToken)


    /// Run the provided task synchronously and if it returns a result navigate to that 
    /// document in the solution
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


    let navigate  (document:Document) (navigableItem:#INavigableItem)  =
        let workspace = document.Project.Solution.Workspace
        let navigationService = workspace.Services.GetService<IDocumentNavigationService>()
        let options = workspace.Options.WithChangedOption (NavigationOptions.PreferProvisionalTab, true)
        let result = navigationService.TryNavigateToSpan (workspace, navigableItem.Document.Id, navigableItem.SourceSpan, options)
        if result then true else
        statusBar.SetText "Could Not Navigate to Definition of Symbol Under Caret" |> ignore
        false


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


    /// Navigate to the positon of the textSpan in the provided document
    member this.TryNavigateToTextSpan (document:Document, textSpan:TextSpan) =
        let navigableItem = FSharpNavigableItem (document, textSpan) :> INavigableItem
        let workspace = document.Project.Solution.Workspace
        let navigationService = workspace.Services.GetService<IDocumentNavigationService>()
        let options = workspace.Options.WithChangedOption (NavigationOptions.PreferProvisionalTab, true)
        let result = navigationService.TryNavigateToSpan (workspace, navigableItem.Document.Id, navigableItem.SourceSpan, options)
        if result then true else
        statusBar.SetText "Could Not Navigate to Definition of Symbol Under Caret" |> ignore
        false


    /// find the declaration location (signature file/.fsi) of the target symbol if possible, fall back to definition 
    member this.NavigateToSymbolDeclarationAsync (targetDocument:Document, targetSourceText:SourceText, symbolRange:range) = asyncMaybe {
        statusBar.SetText "Trying to locate symbol..." |> ignore
        let! navigableItem = 
            FSharpGoToDefinition.findDeclarationOfSymbolAtRange 
                (targetDocument, symbolRange, targetSourceText, checkerProvider.Checker, projectInfoManager)
        return navigate targetDocument navigableItem
    }
    

     /// find the definition location (implementation file/.fs) of the target symbol
    member this.NavigateToSymbolDefinitionAsync (targetDocument:Document, targetSourceText:SourceText, symbolRange:range)= asyncMaybe {
        statusBar.SetText "Trying to locate symbol..." |> ignore
        let! navigableItem = 
            FSharpGoToDefinition.findDefinitionOfSymbolAtRange 
                (targetDocument, symbolRange, targetSourceText, checkerProvider.Checker, projectInfoManager) 
        return navigate targetDocument navigableItem
    }
    
    /// Construct a task that will return a navigation target for the implementation definition of the symbol 
    /// at the provided position in the document
    member this.FindDefinitionsTask (document, position, cancellationToken) =
        createNavigationTask FSharpGoToDefinition.findDefinition 
                            (document, position, checkerProvider.Checker, projectInfoManager)
                            cancellationToken


    /// Try to navigate to the signature declaration of the symbol at the symbolRange in the originDocument
    member this.TryNavigateToSymbolDeclaration (originDocument:Document, originSourceText:SourceText, symbolRange:range,cancellationToken)=
        statusBar.SetText "Trying to locate symbol..." |> ignore
        let declarationTask = 
            createNavigationTask FSharpGoToDefinition.findDeclarationOfSymbolAtRange 
                                 (originDocument, symbolRange, originSourceText, checkerProvider.Checker, projectInfoManager) 
                                 cancellationToken
        attemptNavigation (originDocument, declarationTask)
        

    /// Try to navigate to the implementation definiton of the symbol at the symbolRange in the originDocument
    member this.TryNavigateToSymbolDefinition (originDocument:Document, originSourceText:SourceText, symbolRange:range, cancellationToken)=
        statusBar.SetText "Trying to locate symbol..." |> ignore
        let definitionTask = 
            createNavigationTask FSharpGoToDefinition.findDefinitionOfSymbolAtRange 
                                 (originDocument, symbolRange, originSourceText, checkerProvider.Checker, projectInfoManager) 
                                 cancellationToken
        attemptNavigation (originDocument, definitionTask)

    
    /// Try to navigate to the definiton of the symbol at the symbolRange in the originDocument
    member this.TryGoToDefinition (document: Document, position: int, cancellationToken: CancellationToken) =
        statusBar.SetText "Trying to locate symbol..." |> ignore
        attemptNavigation (document , this.FindDefinitionsTask (document, position, cancellationToken))

        
    interface IGoToDefinitionService with
        
        member this.FindDefinitionsAsync (document: Document, position: int, cancellationToken: CancellationToken) =
            this.FindDefinitionsTask (document, position, cancellationToken)
        

        member this.TryGoToDefinition (document: Document, position: int, cancellationToken: CancellationToken) =
            this.TryGoToDefinition (document, position, cancellationToken)
