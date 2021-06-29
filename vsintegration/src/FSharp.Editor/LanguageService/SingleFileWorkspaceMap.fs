// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Concurrent
open Microsoft.CodeAnalysis
open Microsoft.VisualStudio
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem
open Microsoft.VisualStudio.LanguageServices.ProjectSystem
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.LanguageServices

[<Sealed>]
type internal SingleFileWorkspaceMap(workspace: VisualStudioWorkspace,
                                     miscFilesWorkspace: MiscellaneousFilesWorkspace,
                                     optionsManager: FSharpProjectOptionsManager, 
                                     projectContextFactory: IWorkspaceProjectContextFactory,
                                     rdt: IVsRunningDocumentTable) as this =

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
            if document.Project.Language = FSharpConstants.FSharpLanguageName && document.Project.Name <> FSharpConstants.FSharpMiscellaneousFilesName then
                match files.TryRemove(document.FilePath) with
                | true, projectContext ->
                    optionsManager.ClearSingleFileOptionsCache(document.Id)
                    projectContext.Dispose()
                | _ -> ()
        )

        workspace.DocumentClosed.Add(fun args ->
            let document = args.Document
            match files.TryRemove(document.FilePath) with
            | true, projectContext ->
                optionsManager.ClearSingleFileOptionsCache(document.Id)
                projectContext.Dispose()
            | _ -> ()
        )

        do
            rdt.AdviseRunningDocTableEvents(this) |> ignore

    interface IVsRunningDocTableEvents with

        member __.OnAfterAttributeChange(_, _) = VSConstants.E_NOTIMPL

        member __.OnAfterDocumentWindowHide(_, _) = VSConstants.E_NOTIMPL

        member __.OnAfterFirstDocumentLock(_, _, _, _) = VSConstants.E_NOTIMPL

        member __.OnAfterSave(_) = VSConstants.E_NOTIMPL

        member __.OnBeforeDocumentWindowShow(_, _, _) = VSConstants.E_NOTIMPL

        member __.OnBeforeLastDocumentUnlock(_, _, _, _) = VSConstants.E_NOTIMPL

    interface IVsRunningDocTableEvents2 with

        member __.OnAfterAttributeChange(_, _) = VSConstants.E_NOTIMPL

        member __.OnAfterAttributeChangeEx(_, grfAttribs, _, _, pszMkDocumentOld, _, _, pszMkDocumentNew) = 
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

        member __.OnAfterDocumentWindowHide(_, _) = VSConstants.E_NOTIMPL

        member __.OnAfterFirstDocumentLock(_, _, _, _) = VSConstants.E_NOTIMPL

        member __.OnAfterSave(_) = VSConstants.E_NOTIMPL

        member __.OnBeforeDocumentWindowShow(_, _, _) = VSConstants.E_NOTIMPL

        member __.OnBeforeLastDocumentUnlock(_, _, _, _) = VSConstants.E_NOTIMPL