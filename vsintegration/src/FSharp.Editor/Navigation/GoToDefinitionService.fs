// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
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

open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.VisualStudio.FSharp.Editor.Logging
open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem 
open Microsoft.VisualStudio.LanguageServices.Implementation.TaskList
open Microsoft.VisualStudio.LanguageServices.ProjectSystem
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.LanguageServices
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.TextManager.Interop
open Microsoft.VisualStudio
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Utilities


[<NoComparison; NoEquality>]
type internal GoToDefinitionResult =
    | FoundInternal of range
    | FoundExternal of  
        // The document containing the symbol being searched for
        sourceDocument:Document * ast:ParsedInput  * symbol:FSharpSymbolUse
  //| FoundLoadDirective of string  // TODO - Implement navigation across fsx loads

type internal FSharpNavigableItem(document: Document, textSpan: Microsoft.CodeAnalysis.Text.TextSpan) =

    interface INavigableItem with
        member this.Glyph = Glyph.BasicFile
        member this.DisplayFileLocation = true
        member this.IsImplicitlyDeclared = false
        member this.Document = document
        member this.SourceSpan = textSpan
        member this.DisplayTaggedParts = ImmutableArray<TaggedText>.Empty
        member this.ChildItems = ImmutableArray<INavigableItem>.Empty

[<Shared>]
[<Export>]
type internal MetadataWorkspace
    [<ImportingConstructor>]
    (   vsworkspace:VisualStudioWorkspaceImpl
    ,   textBufferFactoryService:ITextBufferFactoryService
    ,   contentRegistryService:IContentTypeRegistryService
    ) =
    inherit Workspace (vsworkspace.Services.HostServices, "Signature Metadata Workspace")

    override __.CanApplyChange _ = true
    member __.TextBufferFactoryService = textBufferFactoryService
    member __.ContentRegistryService = contentRegistryService 


[<Shared>]
[<ExportLanguageService(typeof<IGoToDefinitionService>, FSharpCommonConstants.FSharpLanguageName)>]
type internal FSharpGoToDefinitionService 
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider,
        projectInfoManager: ProjectInfoManager,
        metadataService : NavigateToSignatureMetadataService,
        //fsharpLanguageService:FSharpLanguageService,
        metadataWorkspace:MetadataWorkspace,
        [<ImportMany>]presenters: IEnumerable<INavigableItemsPresenter>
    ) =

    static member FindDefinition (checker: FSharpChecker, document: Document, sourceText: SourceText, filePath: string, position: int, defines: string list, options: FSharpProjectOptions, textVersionHash: int) : Async<GoToDefinitionResult option> = 
        asyncMaybe {
            let textLine = sourceText.Lines.GetLineFromPosition position
            let textLinePos = sourceText.Lines.GetLinePosition position
            let fcsTextLineNumber = Line.fromZ textLinePos.Line
            let! lexerSymbol = CommonHelpers.getSymbolAtPosition (document.Id, sourceText, position, filePath, defines, SymbolLookupKind.Greedy)
            let! _, ast, checkFileResults = checker.ParseAndCheckDocument (filePath, textVersionHash, sourceText.ToString (), options, allowStaleResults = true)
            let! declarations = 
                    checkFileResults.GetDeclarationLocationAlternate 
                        (fcsTextLineNumber, lexerSymbol.Ident.idRange.EndColumn, textLine.ToString (), lexerSymbol.FullIsland, false) |> liftAsync
            
            match declarations with
            | FSharpFindDeclResult.DeclFound range -> return! Some (FoundInternal range)
            | _ -> 
                let! fsSymbol = checkFileResults.GetSymbolUseAtLocation(fcsTextLineNumber, lexerSymbol.Ident.idRange.EndColumn, textLine.ToString (), lexerSymbol.FullIsland)
                // TODO tryget range from navigate to metadata
                return! Some (FoundExternal (document, ast, fsSymbol))  
        }
    
    // FSROSLYNTODO: Since we are not integrated with the Roslyn project system yet, the below call
    // document.Project.Solution.GetDocumentIdsWithFilePath() will only access files in F# projects.
    // Either Roslyn INavigableItem needs to be extended to allow arbitary full paths, or we need to
    // fully integrate with their project system.
    member this.FindDefinitionsAsyncAux (document: Document, position: int, cancellationToken: CancellationToken) = asyncMaybe {
        let results = List<INavigableItem>()
        let! options = projectInfoManager.TryGetOptionsForEditingDocumentOrProject document
        let! sourceText = document.GetTextAsync cancellationToken
        let! textVersion = document.GetTextVersionAsync cancellationToken
        let defines = CompilerEnvironment.GetCompilationDefinesForEditing (document.Name, options.OtherOptions |> Seq.toList)
        let! gotoDefnResult = FSharpGoToDefinitionService.FindDefinition (checkerProvider.Checker, document, sourceText, document.FilePath, position, defines, options, textVersion.GetHashCode ())
            
        match gotoDefnResult with 
        | FoundInternal range ->
        // REVIEW: 
            let fileName = try System.IO.Path.GetFullPath range.FileName with _ -> range.FileName
            let refDocumentIds = document.Project.Solution.GetDocumentIdsWithFilePath fileName
            if not refDocumentIds.IsEmpty then 
                let refDocumentId = refDocumentIds.First ()
                let refDocument = document.Project.Solution.GetDocument refDocumentId
                let! refSourceText = refDocument.GetTextAsync cancellationToken
                let refTextSpan = CommonRoslynHelpers.FSharpRangeToTextSpan (refSourceText, range)
                results.Add (FSharpNavigableItem (refDocument, refTextSpan))
            return false, results.AsEnumerable ()
        | FoundExternal (sourceDocument, ast, symbol) ->
            let! signatureText, sigPath,range = metadataService.TryFindMetadataRange (sourceDocument, ast, symbol)
                
            let sigSourceText = SourceText.From (signatureText, System.Text.Encoding.UTF8)

            debug "Path to generated signature file - %s"sigPath

            let fsharpContentType = metadataWorkspace.ContentRegistryService.GetContentType "F#"
            let buffer = metadataWorkspace.TextBufferFactoryService.CreateTextBuffer (signatureText, fsharpContentType)//signatureText,Utilities.Con (fsharpContentType:>IContentTypeLanguageService).GetDefaultContentType)
            debug "buffer content type - %A" buffer.ContentType
            let bufferContainer = buffer.AsTextContainer ()
            let sigLoader = TextLoader.From(bufferContainer, VersionStamp.Default, sigPath)
            //let sigLoader = TextLoader.From (TextAndVersion.Create (sourceText, VersionStamp.Create(), filePath=sigPath))
            let sigDocId = DocumentId.CreateNewId document.Project.Id
            let sigDocInfo = DocumentInfo.Create (sigDocId, Path.GetFileNameWithoutExtension sigPath, filePath=sigPath, loader=sigLoader)


            let sigProject = 
                let project = metadataWorkspace.CurrentSolution.GetProject document.Project.Id
                if not (isNull project) then project else
                let sigProjectInfo = 
                    ProjectInfo.Create (document.Project.Id, VersionStamp.Default, 
                                        document.Project.Name+".sig", document.Project.Name+".sig", "F#",
                                        documents=[sigDocInfo])
                let sln = metadataWorkspace.CurrentSolution.AddProject sigProjectInfo
                let applyResult = metadataWorkspace.TryApplyChanges sln 
                debug "applied sigProject change to metadataworkspace - %b" applyResult
                metadataWorkspace.CurrentSolution.GetProject document.Project.Id


            let sigDocument = 
                match sigProject.Documents|> Seq.tryFind(fun doc -> doc.FilePath=sigPath) with
                | Some doc -> doc
                | None -> metadataWorkspace.CurrentSolution.GetDocument sigDocId
                
            debug "SigDocument - %s [Project - %s] %s" sigDocument.Name sigDocument.Project.Name sigDocument.FilePath

            let sigTextSpan = CommonRoslynHelpers.FSharpRangeToTextSpan (sigSourceText, range)
            debug "range is - %A" range
            results.Add (FSharpNavigableItem (sigDocument, sigTextSpan))
            return true, results.AsEnumerable ()
    }
         

    interface IGoToDefinitionService with
        member this.FindDefinitionsAsync (document: Document, position: int, cancellationToken: CancellationToken) =
            this.FindDefinitionsAsyncAux (document, position, cancellationToken)
            |> Async.map (Option.map snd >> Option.defaultValue Seq.empty)
            |> CommonRoslynHelpers.StartAsyncAsTask cancellationToken


        member this.TryGoToDefinition(document: Document, position: int, cancellationToken: CancellationToken) =
            let definitionTask = 
                this.FindDefinitionsAsyncAux (document, position, cancellationToken)
                |> Async.map (Option.defaultValue (false,Seq.empty))
                |> CommonRoslynHelpers.StartAsyncAsTask cancellationToken
            // REVIEW: document this use of a blocking wait on the cancellation token, explaining why it is ok
            definitionTask.Wait cancellationToken
            
            //if definitionTask.Status = TaskStatus.RanToCompletion && not(isNull definitionTask.Result) && definitionTask.Result.Any() then
            if definitionTask.Status = TaskStatus.RanToCompletion then
                let sigNav,navs = definitionTask.Result
                let navigableItem = navs.First () // F# API provides only one INavigableItem
                ignore presenters
                if not sigNav then
                //let navigableItem = definitionTask.Result.First () // F# API provides only one INavigableItem
                    let workspace = document.Project.Solution.Workspace
                    let navigationService = workspace.Services.GetService<IDocumentNavigationService>()
                    let options = workspace.Options.WithChangedOption (NavigationOptions.PreferProvisionalTab, true)
                    navigationService.TryNavigateToSpan (workspace, navigableItem.Document.Id, navigableItem.SourceSpan, options)
                else
                // prefer open documents in the preview tab
                    let metaNav = metadataWorkspace.Services.GetService<IDocumentNavigationService>()
                    let options = metadataWorkspace.Options.WithChangedOption (NavigationOptions.PreferProvisionalTab, true)
                    metaNav.TryNavigateToSpan(metadataWorkspace,navigableItem.Document.Id, navigableItem.SourceSpan, options) 

                // FSROSLYNTODO: potentially display multiple results here
                // If GotoDef returns one result then it should try to jump to a discovered location. If it returns multiple results then it should use 
                // presenters to render items so user can choose whatever he needs. Given that per comment F# API always returns only one item then we 
                // should always navigate to definition and get rid of presenters.
                //
                //let refDisplayString = refSourceText.GetSubText(refTextSpan).ToString()
                //for presenter in presenters do
                //    presenter.DisplayResult(navigableItem.DisplayString, definitionTask.Result)
                //true

            else false
