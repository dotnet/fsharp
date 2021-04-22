// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Concurrent
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Diagnostics
open Microsoft.VisualStudio
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Editor
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem
open Microsoft.VisualStudio.LanguageServices.ProjectSystem
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.LanguageServices
open Microsoft.VisualStudio.TextManager.Interop
open Microsoft.VisualStudio.FSharp.Editor.Logging

[<Sealed>]
type internal SingleFileWorkspaceMap(workspace: VisualStudioWorkspace,
                                     miscFilesWorkspace: MiscellaneousFilesWorkspace,
                                     optionsManager: FSharpProjectOptionsManager, 
                                     projectContextFactory: IWorkspaceProjectContextFactory,
                                     rdt: IVsRunningDocumentTable,
                                     rdt4: IVsRunningDocumentTable4,
                                     editorAdaptersFactory: IVsEditorAdaptersFactoryService,
                                     analyzerService: IFSharpDiagnosticAnalyzerService) as this =

    let files = ConcurrentDictionary(StringComparer.OrdinalIgnoreCase)

    let createSourceCodeKind (filePath: string) =
        if isScriptFile filePath then
            SourceCodeKind.Script
        else
            SourceCodeKind.Regular

    let createProjectContext filePath =
        let projectContext = projectContextFactory.CreateProjectContext(FSharpConstants.FSharpLanguageName, filePath, filePath, Guid.NewGuid(), null, null)
        projectContext.DisplayName <- FSharpConstants.FSharpMiscellaneousFilesName
        projectContext.AddSourceFile(filePath, sourceCodeKind = createSourceCodeKind filePath)
        projectContext

    do
        miscFilesWorkspace.DocumentOpened.Add(fun args ->
            let document = args.Document

            if document.Project.Language = FSharpConstants.FSharpLanguageName && workspace.CurrentSolution.GetDocumentIdsWithFilePath(document.FilePath).Length = 0 then
                files.[document.FilePath] <- createProjectContext document.FilePath
        )

        workspace.DocumentOpened.Add(fun args ->
            let document = args.Document
            if document.Project.Language = FSharpConstants.FSharpLanguageName && 
               not document.Project.IsFSharpMiscellaneousOrMetadata then
                match files.TryRemove(document.FilePath) with
                | true, projectContext ->
                    optionsManager.ClearSingleFileOptionsCache(document.Id)
                    projectContext.Dispose()
                | _ -> ()
        )

        workspace.DocumentClosed.Add(fun args ->
            let document = args.Document
            if document.Project.Language = FSharpConstants.FSharpLanguageName && 
               document.Project.IsFSharpMiscellaneousOrMetadata then
                match files.TryRemove(document.FilePath) with
                | true, projectContext ->
                    optionsManager.ClearSingleFileOptionsCache(document.Id)
                    projectContext.Dispose()
                | _ -> ()
        )

        do
            rdt.AdviseRunningDocTableEvents(this) |> ignore

    interface IVsRunningDocTableEvents with

        member _.OnAfterAttributeChange(_, _) = VSConstants.E_NOTIMPL

        member _.OnAfterDocumentWindowHide(_, _) = VSConstants.E_NOTIMPL

        member _.OnAfterFirstDocumentLock(_, _, _, _) = VSConstants.E_NOTIMPL

        member _.OnAfterSave(docCookie) = 
            try
                // This causes re-analysis to happen when a F# document is saved.
                // We do this because FCS relies on the file system and existing open documents
                // need to be re-analyzed so the changes are propogated.
                // We only re-analyze F# documents that are dependent on the document that was just saved.
                // We ignore F# script documents here.
                let vsTextBuffer = rdt4.GetDocumentData(docCookie) :?> IVsTextBuffer
                let textBuffer = editorAdaptersFactory.GetDataBuffer(vsTextBuffer)
                let textContainer = textBuffer.AsTextContainer()
                let mutable workspace = Unchecked.defaultof<_>
                if Workspace.TryGetWorkspace(textContainer, &workspace) then
                    let solution = workspace.CurrentSolution
                    let documentId = workspace.GetDocumentIdInCurrentContext(textContainer)
                    match box documentId with
                    | null -> ()
                    | _ -> 
                        let document = solution.GetDocument(documentId)
                        if document.Project.Language = LanguageNames.FSharp && not document.IsFSharpScript then
                            let openDocIds = workspace.GetOpenDocumentIds()
                            let depProjIds = document.Project.GetDependentProjectIds().Add(document.Project.Id)

                            let docIdsToReanalyze =
                                openDocIds
                                |> Seq.filter (fun x ->
                                    depProjIds.Contains(x.ProjectId) && x <> document.Id &&
                                    (
                                        let doc = solution.GetDocument(x)
                                        match box doc with
                                        | null -> false
                                        | _ -> doc.Project.Language = LanguageNames.FSharp
                                    )
                                )
                                |> Array.ofSeq

                            if docIdsToReanalyze.Length > 0 then
                                analyzerService.Reanalyze(workspace, documentIds=docIdsToReanalyze)
              with
              | ex -> logException ex
        
            VSConstants.S_OK

        member _.OnBeforeDocumentWindowShow(_, _, _) = VSConstants.E_NOTIMPL

        member _.OnBeforeLastDocumentUnlock(_, _, _, _) = VSConstants.E_NOTIMPL

    interface IVsRunningDocTableEvents2 with

        member _.OnAfterAttributeChange(_, _) = VSConstants.E_NOTIMPL

        member _.OnAfterAttributeChangeEx(_, grfAttribs, _, _, pszMkDocumentOld, _, _, pszMkDocumentNew) = 
            // Handles renaming of a misc file
            if (grfAttribs &&& (uint32 __VSRDTATTRIB.RDTA_MkDocument)) <> 0u && files.ContainsKey(pszMkDocumentOld) then
                match files.TryRemove(pszMkDocumentOld) with
                | true, projectContext ->
                    let project = workspace.CurrentSolution.GetProject(projectContext.Id)
                    if project <> null then
                        let documentOpt =
                            project.Documents 
                            |> Seq.tryFind (fun x -> String.Equals(x.FilePath, pszMkDocumentOld, StringComparison.OrdinalIgnoreCase))
                        match documentOpt with
                        | None -> ()
                        | Some(document) ->                           
                            optionsManager.ClearSingleFileOptionsCache(document.Id)
                            projectContext.Dispose()
                            files.[pszMkDocumentNew] <- createProjectContext pszMkDocumentNew
                    else
                        projectContext.Dispose() // fallback, shouldn't happen, but in case it does let's dispose of the project context so we don't leak
                | _ -> ()
            VSConstants.S_OK

        member _.OnAfterDocumentWindowHide(_, _) = VSConstants.E_NOTIMPL

        member _.OnAfterFirstDocumentLock(_, _, _, _) = VSConstants.E_NOTIMPL

        member _.OnAfterSave(_) = VSConstants.E_NOTIMPL

        member _.OnBeforeDocumentWindowShow(_, _, _) = VSConstants.E_NOTIMPL

        member _.OnBeforeLastDocumentUnlock(_, _, _, _) = VSConstants.E_NOTIMPL