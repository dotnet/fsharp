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
open FSharp.Compiler.CodeAnalysis

type private ScriptDependencies = ResizeArray<string>

[<Sealed>]
type internal SingleFileWorkspaceMap(workspace: Workspace,
                                     miscFilesWorkspace: MiscellaneousFilesWorkspace,
                                     projectContextFactory: IWorkspaceProjectContextFactory,
                                     rdt: IVsRunningDocumentTable) as this =

    // We have a lock because the `ScriptUpdated` event may happen concurrently when a document opens or closes.
    let gate = obj()
    let files = ConcurrentDictionary(StringComparer.OrdinalIgnoreCase)
    let optionsManager = workspace.Services.GetRequiredService<IFSharpWorkspaceService>().FSharpProjectOptionsManager

    static let createSourceCodeKind (filePath: string) =
        if isScriptFile filePath then
            SourceCodeKind.Script
        else
            SourceCodeKind.Regular

    static let canUpdateScript (depSourceFiles: string []) (currentDepSourceFiles: ScriptDependencies) =
        depSourceFiles.Length <> currentDepSourceFiles.Count ||
        (
             (currentDepSourceFiles, depSourceFiles) 
             ||> Seq.forall2 (fun (x: string) y -> x.Equals(y, StringComparison.OrdinalIgnoreCase))
             |> not

        )

    static let createProjectContext (projectContextFactory: IWorkspaceProjectContextFactory) filePath =
        let projectContext = projectContextFactory.CreateProjectContext(FSharpConstants.FSharpLanguageName, filePath, filePath, Guid.NewGuid(), null, null)
        projectContext.DisplayName <- FSharpConstants.FSharpMiscellaneousFilesName
        projectContext.AddSourceFile(filePath, sourceCodeKind = createSourceCodeKind filePath)
        projectContext, ScriptDependencies()

    do
        optionsManager.ScriptUpdated.Add(fun scriptProjectOptions ->
            if scriptProjectOptions.SourceFiles.Length > 0 then
                // The last file in the project options is the main script file.
                let filePath = scriptProjectOptions.SourceFiles.[scriptProjectOptions.SourceFiles.Length - 1]
                let depSourceFiles = scriptProjectOptions.SourceFiles |> Array.take (scriptProjectOptions.SourceFiles.Length - 1)

                lock gate (fun () ->
                    match files.TryGetValue(filePath) with
                    | true, (projectContext: IWorkspaceProjectContext, currentDepSourceFiles: ScriptDependencies) -> 
                        if canUpdateScript depSourceFiles currentDepSourceFiles then
                            currentDepSourceFiles
                            |> Seq.iter (fun x ->
                                match files.TryGetValue(x) with
                                | true, (depProjectContext, _) ->
                                    projectContext.RemoveProjectReference(depProjectContext)
                                | _ ->
                                    ()
                            )

                            currentDepSourceFiles.Clear()
                            depSourceFiles
                            |> Array.iter (fun filePath ->
                                currentDepSourceFiles.Add(filePath)
                                match files.TryGetValue(filePath) with
                                | true, (depProjectContext, _) ->
                                    projectContext.AddProjectReference(depProjectContext, MetadataReferenceProperties.Assembly)
                                | _ ->
                                    let result = createProjectContext projectContextFactory filePath
                                    files.[filePath] <- result
                                    let depProjectContext, _ = result
                                    projectContext.AddProjectReference(depProjectContext, MetadataReferenceProperties.Assembly)
                            )
                    | _ -> ()
            )
        )

        miscFilesWorkspace.DocumentOpened.Add(fun args ->
            let document = args.Document

            // If the file does not exist in the current solution, then we can create new project in the VisualStudioWorkspace that represents
            // a F# miscellaneous project, which could be a script or not.
            if document.Project.IsFSharp && workspace.CurrentSolution.GetDocumentIdsWithFilePath(document.FilePath).Length = 0 then
                let filePath = document.FilePath
                lock gate (fun () ->
                    if files.ContainsKey(filePath) |> not then
                        files.[filePath] <- createProjectContext projectContextFactory filePath
                )
        )

        workspace.DocumentOpened.Add(fun args ->
            let document = args.Document
            if not document.Project.IsFSharpMiscellaneousOrMetadata then
                optionsManager.ClearSingleFileOptionsCache(document.Id)

                let projectContextOpt =
                    lock gate (fun () ->
                        match files.TryRemove(document.FilePath) with
                        | true, (projectContext, _) ->
                            Some projectContext
                        | _ ->
                            None
                    )

                match projectContextOpt with
                | Some projectContext ->
                    projectContext.Dispose()
                | _ ->
                    ()
        )

        workspace.DocumentClosed.Add(fun args ->
            let document = args.Document
            if document.Project.IsFSharpMiscellaneousOrMetadata then
                optionsManager.ClearSingleFileOptionsCache(document.Id)

                let projectContextOpt =
                    lock gate (fun () ->
                        match files.TryRemove(document.FilePath) with
                        | true, (projectContext, _) ->
                            Some projectContext
                        | _ ->
                            None
                    )

                match projectContextOpt with
                | Some projectContext ->
                    let projIds = document.Project.Solution.GetDependentProjectIds(document.Project.Id)
                    if projIds.Count = 0 then
                        projectContext.Dispose()
                | _ ->
                    ()
        )

        do
            rdt.AdviseRunningDocTableEvents(this) |> ignore

    interface IVsRunningDocTableEvents with

        member _.OnAfterAttributeChange(_, _) = VSConstants.E_NOTIMPL

        member _.OnAfterDocumentWindowHide(_, _) = VSConstants.E_NOTIMPL

        member _.OnAfterFirstDocumentLock(_, _, _, _) = VSConstants.E_NOTIMPL

        member _.OnAfterSave(_) = VSConstants.E_NOTIMPL

        member _.OnBeforeDocumentWindowShow(_, _, _) = VSConstants.E_NOTIMPL

        member _.OnBeforeLastDocumentUnlock(_, _, _, _) = VSConstants.E_NOTIMPL

    interface IVsRunningDocTableEvents2 with

        member _.OnAfterAttributeChange(_, _) = VSConstants.E_NOTIMPL

        member _.OnAfterAttributeChangeEx(_, grfAttribs, _, _, pszMkDocumentOld, _, _, pszMkDocumentNew) = 
            // Handles renaming of a misc file
            if (grfAttribs &&& (uint32 __VSRDTATTRIB.RDTA_MkDocument)) <> 0u && files.ContainsKey(pszMkDocumentOld) then
                match files.TryRemove(pszMkDocumentOld) with
                | true, (projectContext, _) ->
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
                            files.[pszMkDocumentNew] <- createProjectContext projectContextFactory pszMkDocumentNew
                    else
                        projectContext.Dispose() // fallback, shouldn't happen, but in case it does let's dispose of the project context so we don't leak
                | _ -> ()
            VSConstants.S_OK

        member _.OnAfterDocumentWindowHide(_, _) = VSConstants.E_NOTIMPL

        member _.OnAfterFirstDocumentLock(_, _, _, _) = VSConstants.E_NOTIMPL

        member _.OnAfterSave(_) = VSConstants.E_NOTIMPL

        member _.OnBeforeDocumentWindowShow(_, _, _) = VSConstants.E_NOTIMPL

        member _.OnBeforeLastDocumentUnlock(_, _, _, _) = VSConstants.E_NOTIMPL