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

[<Sealed>]
type internal SingleFileWorkspaceMap(workspace: VisualStudioWorkspace,
                                     miscFilesWorkspace: MiscellaneousFilesWorkspace,
                                     optionsManager: FSharpProjectOptionsManager, 
                                     projectContextFactory: IWorkspaceProjectContextFactory,
                                     rdt: IVsRunningDocumentTable) as this =

    let gate = obj()
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
        projectContext, ResizeArray()

    do
        optionsManager.ScriptUpdated.Add(fun scriptProjectOptions ->
            if scriptProjectOptions.SourceFiles.Length > 0 then
                // The last file in the project options is the main script file.
                let filePath = scriptProjectOptions.SourceFiles.[scriptProjectOptions.SourceFiles.Length - 1]

                lock gate (fun () ->
                    match files.TryGetValue(filePath) with
                    | true, (projectContext: IWorkspaceProjectContext, currentDepSourceFiles: ResizeArray<_>) -> 
                    
                        let depSourceFiles = scriptProjectOptions.SourceFiles |> Array.filter (fun x -> x.Equals(filePath, StringComparison.OrdinalIgnoreCase) |> not)

                        if depSourceFiles.Length <> currentDepSourceFiles.Count ||
                           (
                                (currentDepSourceFiles, depSourceFiles) 
                                ||> Seq.forall2 (fun (x: string) y -> x.Equals(y, StringComparison.OrdinalIgnoreCase))
                                |> not

                           ) then
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
                                    let result = createProjectContext filePath
                                    files.[filePath] <- result
                                    let depProjectContext, _ = result
                                    projectContext.AddProjectReference(depProjectContext, MetadataReferenceProperties.Assembly)
                            )

                    | _ -> ()
            )
        )

        miscFilesWorkspace.DocumentOpened.Add(fun args ->
            let document = args.Document

            if document.Project.Language = FSharpConstants.FSharpLanguageName && workspace.CurrentSolution.GetDocumentIdsWithFilePath(document.FilePath).Length = 0 then
                let filePath = document.FilePath
                lock gate (fun () ->
                    if files.ContainsKey(filePath) |> not then
                        files.[filePath] <- createProjectContext filePath
                )
        )

        workspace.DocumentOpened.Add(fun args ->
            let document = args.Document
            if document.Project.Language = FSharpConstants.FSharpLanguageName && 
               not document.Project.IsFSharpMiscellaneousOrMetadata then
                optionsManager.ClearSingleFileOptionsCache(document.Id)

                lock gate (fun () ->
                    match files.TryRemove(document.FilePath) with
                    | true, (projectContext, _) ->
                        projectContext.Dispose()
                    | _ -> ()
                )
        )

        workspace.DocumentClosed.Add(fun args ->
            let document = args.Document
            if document.Project.Language = FSharpConstants.FSharpLanguageName && 
               document.Project.IsFSharpMiscellaneousOrMetadata then
                optionsManager.ClearSingleFileOptionsCache(document.Id)

                lock gate (fun () ->
                    match files.TryRemove(document.FilePath) with
                    | true, (projectContext, _) ->
                        projectContext.Dispose()
                    | _ -> ()
                )
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