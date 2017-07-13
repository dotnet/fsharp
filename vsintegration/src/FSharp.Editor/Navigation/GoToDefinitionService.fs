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
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
open System
open System.Windows.Forms

type internal FSharpNavigableItem(document: Document, textSpan: TextSpan) =
    interface INavigableItem with
        member this.Glyph = Glyph.BasicFile
        member this.DisplayFileLocation = true
        member this.IsImplicitlyDeclared = false
        member this.Document = document
        member this.SourceSpan = textSpan
        member this.DisplayTaggedParts = ImmutableArray<TaggedText>.Empty
        member this.ChildItems = ImmutableArray<INavigableItem>.Empty

type internal GoToDefinition(checker: FSharpChecker, projectInfoManager: ProjectInfoManager) =

    static let userOpName = "GoToDefinition"

    /// Use an origin document to provide the solution & workspace used to 
    /// find the corresponding textSpan and INavigableItem for the range
    let rangeToNavigableItem (range: range, document: Document) = 
        async {
            let fileName = try System.IO.Path.GetFullPath range.FileName with _ -> range.FileName
            let refDocumentIds = document.Project.Solution.GetDocumentIdsWithFilePath fileName
            if not refDocumentIds.IsEmpty then 
                let refDocumentId = refDocumentIds.First()
                let refDocument = document.Project.Solution.GetDocument refDocumentId
                let! cancellationToken = Async.CancellationToken
                let! refSourceText = refDocument.GetTextAsync(cancellationToken) |> Async.AwaitTask
                match RoslynHelpers.TryFSharpRangeToTextSpan (refSourceText, range) with 
                | None -> return None
                | Some refTextSpan -> return Some (FSharpNavigableItem (refDocument, refTextSpan))
            else return None
        }

    /// Helper function that is used to determine the navigation strategy to apply, can be tuned towards signatures or implementation files.
    let findSymbolHelper (originDocument: Document, originRange: range, sourceText: SourceText, preferSignature: bool) : Async<FSharpNavigableItem option> =
        asyncMaybe {
            let! projectOptions = projectInfoManager.TryGetOptionsForEditingDocumentOrProject originDocument
            let defines = CompilerEnvironment.GetCompilationDefinesForEditing (originDocument.FilePath, projectOptions.OtherOptions |> Seq.toList)
            let! originTextSpan = RoslynHelpers.TryFSharpRangeToTextSpan (sourceText, originRange)
            let position = originTextSpan.Start
            let! lexerSymbol = Tokenizer.getSymbolAtPosition (originDocument.Id, sourceText, position, originDocument.FilePath, defines, SymbolLookupKind.Greedy, false)
            
            let textLinePos = sourceText.Lines.GetLinePosition position
            let fcsTextLineNumber = Line.fromZ textLinePos.Line
            let lineText = (sourceText.Lines.GetLineFromPosition position).ToString()  
            
            let! _, _, checkFileResults = checker.ParseAndCheckDocument (originDocument,projectOptions,allowStaleResults=true,sourceText=sourceText, userOpName = userOpName)
            let idRange = lexerSymbol.Ident.idRange
            let! fsSymbolUse = checkFileResults.GetSymbolUseAtLocation (fcsTextLineNumber, idRange.EndColumn, lineText, lexerSymbol.FullIsland, userOpName=userOpName)
            let symbol = fsSymbolUse.Symbol
            // if the tooltip was spawned in an implementation file and we have a range targeting
            // a signature file, try to find the corresponding implementation file and target the
            // desired symbol
            if isSignatureFile fsSymbolUse.FileName && preferSignature = false then 
                let fsfilePath = Path.ChangeExtension (originRange.FileName,"fs")
                if not (File.Exists fsfilePath) then return! None else
                let! implDoc = originDocument.Project.Solution.TryGetDocumentFromPath fsfilePath
                let! implSourceText = implDoc.GetTextAsync ()
                let! projectOptions = projectInfoManager.TryGetOptionsForEditingDocumentOrProject implDoc
                let! _, _, checkFileResults = checker.ParseAndCheckDocument (implDoc, projectOptions, allowStaleResults=true, sourceText=implSourceText, userOpName = userOpName)
                let! symbolUses = checkFileResults.GetUsesOfSymbolInFile symbol |> liftAsync
                let! implSymbol  = symbolUses |> Array.tryHead 
                let! implTextSpan = RoslynHelpers.TryFSharpRangeToTextSpan (implSourceText, implSymbol.RangeAlternate)
                return FSharpNavigableItem (implDoc, implTextSpan)
            else
                let! targetDocument = originDocument.Project.Solution.TryGetDocumentFromFSharpRange fsSymbolUse.RangeAlternate
                return! rangeToNavigableItem (fsSymbolUse.RangeAlternate, targetDocument)
        }  

    /// find the declaration location (signature file/.fsi) of the target symbol if possible, fall back to definition 
    member __.FindDeclarationOfSymbolAtRange(targetDocument: Document, symbolRange: range, targetSource: SourceText) =
        findSymbolHelper (targetDocument, symbolRange, targetSource, true)

    /// find the definition location (implementation file/.fs) of the target symbol
    member __.FindDefinitionOfSymbolAtRange(targetDocument: Document, symbolRange: range, targetSourceText: SourceText) =
        findSymbolHelper (targetDocument, symbolRange, targetSourceText, false)
    
    /// use the targetSymbol to find the first instance of its presence in the provided source file
    member __.FindSymbolDeclarationInFile(targetSymbolUse: FSharpSymbolUse, filePath: string, source: string, options: FSharpProjectOptions, fileVersion:int) = 
        asyncMaybe {
            let! _, checkFileAnswer = checker.ParseAndCheckFileInProject (filePath, fileVersion, source, options, userOpName = userOpName) |> liftAsync
            match checkFileAnswer with 
            | FSharpCheckFileAnswer.Aborted -> return! None
            | FSharpCheckFileAnswer.Succeeded checkFileResults ->
                let! symbolUses = checkFileResults.GetUsesOfSymbolInFile targetSymbolUse.Symbol |> liftAsync
                let! implSymbol  = symbolUses |> Array.tryHead 
                return implSymbol.RangeAlternate
        }

type private StatusBar(statusBar: IVsStatusbar) =

    let mutable searchIcon = int16 Microsoft.VisualStudio.Shell.Interop.Constants.SBAI_Find :> obj
        
    let clear() =
        // unfreeze the statusbar
        statusBar.FreezeOutput 0 |> ignore  
        statusBar.Clear() |> ignore
        
    member __.Message(msg: string) =
        let _, frozen = statusBar.IsFrozen()
        // unfreeze the status bar
        if frozen <> 0 then statusBar.FreezeOutput 0 |> ignore
        statusBar.SetText msg |> ignore
        // freeze the status bar
        statusBar.FreezeOutput 1 |> ignore

    member this.TempMessage(msg: string) =
        this.Message msg
        async {
            do! Async.Sleep 4000
            match statusBar.GetText() with
            | 0, currentText when currentText <> msg -> ()
            | _ -> clear()
        }|> Async.Start
    
    member __.Clear() = clear()

    /// Animated magnifying glass that displays on the status bar while a symbol search is in progress.
    member __.Animate() : IDisposable = 
        statusBar.Animation (1, &searchIcon) |> ignore
        { new IDisposable with
            member __.Dispose() = statusBar.Animation(0, &searchIcon) |> ignore }

[<ExportLanguageService(typeof<IGoToDefinitionService>, FSharpConstants.FSharpLanguageName)>]
[<Export(typeof<FSharpGoToDefinitionService>)>]
type internal FSharpGoToDefinitionService 
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider,
        projectInfoManager: ProjectInfoManager,
        [<ImportMany>] _presenters: IEnumerable<INavigableItemsPresenter>
    ) =

    static let userOpName = "GoToDefinition"
    let gotoDefinition = GoToDefinition(checkerProvider.Checker, projectInfoManager)
    let serviceProvider =  ServiceProvider.GlobalProvider
    let statusBar = StatusBar(serviceProvider.GetService<SVsStatusbar,IVsStatusbar>())

    let tryNavigateToItem (navigableItem: #INavigableItem option) =
        use __ = statusBar.Animate()

        match navigableItem with
        | Some navigableItem ->
            statusBar.Message SR.NavigatingTo.Value

            let workspace = navigableItem.Document.Project.Solution.Workspace
            let navigationService = workspace.Services.GetService<IDocumentNavigationService>()
            // prefer open documents in the preview tab
            let options = workspace.Options.WithChangedOption (NavigationOptions.PreferProvisionalTab, true)
            let result = navigationService.TryNavigateToSpan (workspace, navigableItem.Document.Id, navigableItem.SourceSpan, options)
            
            if result then 
                statusBar.Clear()
            else 
                statusBar.TempMessage SR.CannotNavigateUnknown.Value
            
            result
        | None ->
            statusBar.TempMessage SR.CannotDetermineSymbol.Value
            true

    /// Navigate to the positon of the textSpan in the provided document
    /// used by quickinfo link navigation when the tooltip contains the correct destination range.
    member this.TryNavigateToTextSpan (document: Document, textSpan: TextSpan) =
        let navigableItem = FSharpNavigableItem (document, textSpan) :> INavigableItem
        let workspace = document.Project.Solution.Workspace
        let navigationService = workspace.Services.GetService<IDocumentNavigationService>()
        let options = workspace.Options.WithChangedOption (NavigationOptions.PreferProvisionalTab, true)
        if navigationService.TryNavigateToSpan (workspace, navigableItem.Document.Id, navigableItem.SourceSpan, options) then 
            true 
        else
            statusBar.TempMessage SR.CannotNavigateUnknown.Value
            false

    /// find the declaration location (signature file/.fsi) of the target symbol if possible, fall back to definition 
    member __.NavigateToSymbolDeclarationAsync (targetDocument: Document, targetSourceText: SourceText, symbolRange: range) =
        gotoDefinition.FindDeclarationOfSymbolAtRange(targetDocument, symbolRange, targetSourceText) |> Async.map tryNavigateToItem

    /// find the definition location (implementation file/.fs) of the target symbol
    member this.NavigateToSymbolDefinitionAsync (targetDocument: Document, targetSourceText: SourceText, symbolRange: range) = 
        gotoDefinition.FindDefinitionOfSymbolAtRange(targetDocument, symbolRange, targetSourceText) |> Async.map tryNavigateToItem

    /// Construct a task that will return a navigation target for the implementation definition of the symbol 
    /// at the provided position in the document.
    member __.FindDefinitionsTask(originDocument: Document, position: int, cancellationToken: CancellationToken) =
        asyncMaybe {
            let! projectOptions = projectInfoManager.TryGetOptionsForEditingDocumentOrProject originDocument
            let! sourceText = originDocument.GetTextAsync () |> liftTaskAsync
            let defines = CompilerEnvironment.GetCompilationDefinesForEditing (originDocument.FilePath, projectOptions.OtherOptions |> Seq.toList)
            let textLine = sourceText.Lines.GetLineFromPosition position
            let textLinePos = sourceText.Lines.GetLinePosition position
            let fcsTextLineNumber = Line.fromZ textLinePos.Line
            let lineText = (sourceText.Lines.GetLineFromPosition position).ToString()  
            
            let preferSignature = isSignatureFile originDocument.FilePath

            let! _, _, checkFileResults = checkerProvider.Checker.ParseAndCheckDocument (originDocument, projectOptions, allowStaleResults=true, sourceText=sourceText, userOpName=userOpName)
                
            let! lexerSymbol = Tokenizer.getSymbolAtPosition (originDocument.Id, sourceText, position,originDocument.FilePath, defines, SymbolLookupKind.Greedy, false)
            let idRange = lexerSymbol.Ident.idRange

            let! declarations = checkFileResults.GetDeclarationLocation (fcsTextLineNumber, lexerSymbol.Ident.idRange.EndColumn, textLine.ToString(), lexerSymbol.FullIsland, preferSignature, userOpName=userOpName) |> liftAsync
            let! targetSymbolUse = checkFileResults.GetSymbolUseAtLocation (fcsTextLineNumber, idRange.EndColumn, lineText, lexerSymbol.FullIsland, userOpName=userOpName)

            match declarations with
            | FSharpFindDeclResult.DeclFound targetRange -> 
                // if goto definition is called at we are alread at the declaration location of a symbol in
                // either a signature or an implementation file then we jump to it's respective postion in thethe
                if lexerSymbol.Range = targetRange then
                    // jump from signature to the corresponding implementation
                    if isSignatureFile originDocument.FilePath then
                        let implFilePath = Path.ChangeExtension (originDocument.FilePath,"fs")
                        if not (File.Exists implFilePath) then return! None else
                        let! implDocument = originDocument.Project.Solution.TryGetDocumentFromPath implFilePath
                        let! implSourceText = implDocument.GetTextAsync () |> liftTaskAsync
                        let! implVersion = implDocument.GetTextVersionAsync () |> liftTaskAsync
                        
                        let! targetRange = 
                            gotoDefinition.FindSymbolDeclarationInFile(targetSymbolUse, implFilePath, implSourceText.ToString(), projectOptions, implVersion.GetHashCode())

                        let! implTextSpan = RoslynHelpers.TryFSharpRangeToTextSpan (implSourceText, targetRange)
                        let navItem = FSharpNavigableItem (implDocument, implTextSpan)
                        return navItem
                    else // jump from implementation to the corresponding signature
                        let! declarations = checkFileResults.GetDeclarationLocation (fcsTextLineNumber, lexerSymbol.Ident.idRange.EndColumn, textLine.ToString(), lexerSymbol.FullIsland, true, userOpName=userOpName) |> liftAsync
                        match declarations with
                        | FSharpFindDeclResult.DeclFound targetRange -> 
                            let! sigDocument = originDocument.Project.Solution.TryGetDocumentFromPath targetRange.FileName
                            let! sigSourceText = sigDocument.GetTextAsync () |> liftTaskAsync
                            let! sigTextSpan = RoslynHelpers.TryFSharpRangeToTextSpan (sigSourceText, targetRange)
                            let navItem = FSharpNavigableItem (sigDocument, sigTextSpan)
                            return navItem
                        | _ -> return! None
                // when the target range is different follow the navigation convention of 
                // - gotoDefn origin = signature , gotoDefn destination = signature
                // - gotoDefn origin = implementation, gotoDefn destination = implementation 
                else
                    let! sigDocument = originDocument.Project.Solution.TryGetDocumentFromPath targetRange.FileName
                    let! sigSourceText = sigDocument.GetTextAsync () |> liftTaskAsync
                    let! sigTextSpan = RoslynHelpers.TryFSharpRangeToTextSpan (sigSourceText, targetRange)
                    // if the gotodef call originated from a signature and the returned target is a signature, navigate there
                    if isSignatureFile targetRange.FileName && preferSignature then 
                        let navItem = FSharpNavigableItem (sigDocument, sigTextSpan)
                        return navItem
                    else // we need to get an FSharpSymbol from the targetRange found in the signature
                         // that symbol will be used to find the destination in the corresponding implementation file
                        let implFilePath =
                            // Bugfix: apparently sigDocument not always is a signature file
                            if isSignatureFile sigDocument.FilePath then Path.ChangeExtension (sigDocument.FilePath, "fs") 
                            else sigDocument.FilePath

                        let! implDocument = originDocument.Project.Solution.TryGetDocumentFromPath implFilePath
                        let! implVersion = implDocument.GetTextVersionAsync () |> liftTaskAsync
                        let! implSourceText = implDocument.GetTextAsync () |> liftTaskAsync
                        let! projectOptions = projectInfoManager.TryGetOptionsForEditingDocumentOrProject implDocument
                        
                        let! targetRange = 
                            gotoDefinition.FindSymbolDeclarationInFile(targetSymbolUse, implFilePath, implSourceText.ToString(), projectOptions, implVersion.GetHashCode())                               
                        
                        let! implTextSpan = RoslynHelpers.TryFSharpRangeToTextSpan (implSourceText, targetRange)
                        let navItem = FSharpNavigableItem (implDocument, implTextSpan)
                        return navItem
                | _ -> return! None
        } 
        |> Async.map (Option.map (fun x -> x :> INavigableItem) >> Option.toArray >> Array.toSeq)
        |> RoslynHelpers.StartAsyncAsTask cancellationToken        
   
    interface IGoToDefinitionService with
        // used for 'definition peek'
        member this.FindDefinitionsAsync (document: Document, position: int, cancellationToken: CancellationToken) =
            this.FindDefinitionsTask (document, position, cancellationToken)

        // used for 'goto definition' proper
        /// Try to navigate to the definiton of the symbol at the symbolRange in the originDocument
        member this.TryGoToDefinition(document: Document, position: int, cancellationToken: CancellationToken) =
            let definitionTask = this.FindDefinitionsTask (document, position, cancellationToken)
            
            statusBar.Message SR.LocatingSymbol.Value
            use __ = statusBar.Animate()

            // Wrap this in a try/with as if the user clicks "Cancel" on the thread dialog, we'll be cancelled
            // Task.Wait throws an exception if the task is cancelled, so be sure to catch it.
            let completionError =
                try
                    // REVIEW: document this use of a blocking wait on the cancellation token, explaining why it is ok
                    definitionTask.Wait()
                    None
                with exc -> Some <| Exception.flattenMessage exc
            
            match completionError with
            | Some message ->
                statusBar.TempMessage <| String.Format(SR.NavigateToFailed.Value, message)

                // Don't show the dialog box as it's most likely that the user cancelled.
                // Don't make them click twice.
                true
            | None ->
                if definitionTask.Status = TaskStatus.RanToCompletion && definitionTask.Result <> null && definitionTask.Result.Any() then
                    let navigableItem = definitionTask.Result.First() // F# API provides only one INavigableItem
                    tryNavigateToItem (Some navigableItem)

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
                    statusBar.TempMessage SR.CannotDetermineSymbol.Value
                    false