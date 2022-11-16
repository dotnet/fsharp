// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Concurrent
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.ExternalAccess.FSharp
open Microsoft.VisualStudio
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.Shell.Interop
open FSharp.Compiler.CodeAnalysis

type internal FSharpMiscellaneousFileService(workspace: Workspace, 
                                             miscFilesWorkspace: Workspace, 
                                             projectContextFactory: IFSharpWorkspaceProjectContextFactory) =

    let files = ConcurrentDictionary<string, Lazy<IFSharpWorkspaceProjectContext>>(StringComparer.OrdinalIgnoreCase)
    let optionsManager = workspace.Services.GetRequiredService<IFSharpWorkspaceService>().FSharpProjectOptionsManager

    static let createSourceCodeKind (filePath: string) =
        if isScriptFile filePath then
            SourceCodeKind.Script
        else
            SourceCodeKind.Regular

    static let mustUpdateProjectReferences (refSourceFiles: string []) (projectContext: IFSharpWorkspaceProjectContext) =
        refSourceFiles.Length <> projectContext.ProjectReferenceCount ||
        (
             refSourceFiles 
             |> Seq.forall projectContext.HasProjectReference
             |> not
        )

    static let mustUpdateMetadataReferences (referencePaths: string []) (projectContext: IFSharpWorkspaceProjectContext) =
        referencePaths.Length <> projectContext.MetadataReferenceCount ||
        (
             referencePaths
             |> Seq.forall projectContext.HasMetadataReference
             |> not
        )

    let createProjectContextForDocument (filePath: string) =
        let context = projectContextFactory.CreateProjectContext(filePath, filePath)
        context.DisplayName <- FSharpConstants.FSharpMiscellaneousFilesName
        context.AddSourceFile(filePath, createSourceCodeKind filePath)
        context

    let tryRemove (document: Document) =
        let projIds = document.Project.Solution.GetDependentProjectIds(document.Project.Id)
        if projIds.Count = 0 then
            optionsManager.ClearSingleFileOptionsCache(document.Id)

            match files.TryRemove(document.FilePath) with
            | true, projectContext ->
                (projectContext.Value :> IDisposable).Dispose()
            | _ ->
                ()

    do
        optionsManager.ScriptUpdated.Add(fun scriptProjectOptions ->
            if scriptProjectOptions.SourceFiles.Length > 0 then
                // The last file in the project options is the main script file.
                let filePath = scriptProjectOptions.SourceFiles.[scriptProjectOptions.SourceFiles.Length - 1]
                let refSourceFiles = scriptProjectOptions.SourceFiles |> Array.take (scriptProjectOptions.SourceFiles.Length - 1)

                let referencePaths =
                    scriptProjectOptions.OtherOptions
                    |> Seq.filter (fun x -> x.StartsWith("-r:", StringComparison.OrdinalIgnoreCase))
                    |> Seq.map (fun x -> 
                        let startIndex = "-r:".Length
                        x.Substring(startIndex, x.Length - startIndex)
                    )
                    |> Array.ofSeq

                match files.TryGetValue(filePath) with
                | true, (projectContext: Lazy<IFSharpWorkspaceProjectContext>) ->
                    let projectContext = projectContext.Value
                    if mustUpdateProjectReferences refSourceFiles projectContext then
                        let newProjRefs =
                            refSourceFiles
                            |> Array.map (fun filePath ->
                                let createProjectContext = lazy createProjectContextForDocument(filePath)

                                if files.TryAdd(filePath, createProjectContext) then
                                    createProjectContext.Value
                                else
                                    files.[filePath].Value
                            )

                        // We throw away the error in-case projectContext is disposed.
                        try
                            projectContext.SetProjectReferences(newProjRefs)
                        with
                        | _ -> ()

                    if mustUpdateMetadataReferences referencePaths projectContext then
                        // We throw away the error in-case projectContext is disposed.
                        try
                            projectContext.SetMetadataReferences(referencePaths)
                        with
                        | _ -> ()
                | _ ->
                    ()
        )

        miscFilesWorkspace.DocumentOpened.Add(fun args ->
            let document = args.Document

            // If the file does not exist in the current solution, then we can create new project in the VisualStudioWorkspace that represents
            // a F# miscellaneous project, which could be a script or not.
            if document.Project.IsFSharp && workspace.CurrentSolution.GetDocumentIdsWithFilePath(document.FilePath).Length = 0 then
                let filePath = document.FilePath

                let createProjectContext = lazy createProjectContextForDocument(filePath)

                if files.TryAdd(filePath, createProjectContext) then
                    createProjectContext.Force() |> ignore
        )

        workspace.DocumentOpened.Add(fun args ->
            let document = args.Document
            if not document.Project.IsFSharpMiscellaneousOrMetadata then
                match files.TryRemove(document.FilePath) with
                | true, projectContext ->
                    (projectContext.Value :> IDisposable).Dispose()
                    tryRemove document
                | _ ->
                    ()
        )

        workspace.DocumentClosed.Add(fun args ->
            let document = args.Document
            if document.Project.IsFSharpMiscellaneousOrMetadata then
                tryRemove document
        )

        workspace.WorkspaceChanged.Add(fun args ->
            match args.Kind with
            | WorkspaceChangeKind.ProjectRemoved ->
                let proj = args.OldSolution.GetProject(args.ProjectId)
                if proj.IsFSharpMiscellaneousOrMetadata then
                    let projRefs = 
                        proj.GetAllProjectsThisProjectDependsOn()
                        |> Array.ofSeq

                    if projRefs.Length > 0 then
                        projRefs
                        |> Array.iter (fun proj ->
                            let proj = args.NewSolution.GetProject(proj.Id)
                            match proj with
                            | null -> ()
                            | _ ->
                                if proj.IsFSharpMiscellaneousOrMetadata then
                                    match proj.Documents |> Seq.tryExactlyOne with
                                    | Some doc when not (workspace.IsDocumentOpen(doc.Id)) ->
                                        tryRemove doc
                                    | _ ->
                                        ()
                        )
            | _ ->
                ()
        )

    member _.Workspace = workspace

    member _.ProjectContextFactory = projectContextFactory

    member _.ContainsFile filePath = files.ContainsKey(filePath)

    member _.RenameFile(filePath, newFilePath) =
        match files.TryRemove(filePath) with
        | true, projectContext ->
            let project = workspace.CurrentSolution.GetProject(projectContext.Value.Id)
            if project <> null then
                let documentOpt =
                    project.Documents 
                    |> Seq.tryFind (fun x -> String.Equals(x.FilePath, filePath, StringComparison.OrdinalIgnoreCase))
                match documentOpt with
                | None -> ()
                | Some(document) ->                           
                    optionsManager.ClearSingleFileOptionsCache(document.Id)
                    projectContext.Value.Dispose()

                    let newProjectContext = lazy createProjectContextForDocument(newFilePath)

                    if files.TryAdd(newFilePath, newProjectContext) then
                        newProjectContext.Force() |> ignore
            else
                projectContext.Value.Dispose() // fallback, shouldn't happen, but in case it does let's dispose of the project context so we don't leak
        | _ -> ()

[<Sealed>]
type internal SingleFileWorkspaceMap(miscFileService: FSharpMiscellaneousFileService,
                                     rdt: IVsRunningDocumentTable) as this =

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
            if (grfAttribs &&& (uint32 __VSRDTATTRIB.RDTA_MkDocument)) <> 0u && miscFileService.ContainsFile(pszMkDocumentOld) then
                miscFileService.RenameFile(pszMkDocumentOld, pszMkDocumentNew)
            VSConstants.S_OK

        member _.OnAfterDocumentWindowHide(_, _) = VSConstants.E_NOTIMPL

        member _.OnAfterFirstDocumentLock(_, _, _, _) = VSConstants.E_NOTIMPL

        member _.OnAfterSave(_) = VSConstants.E_NOTIMPL

        member _.OnBeforeDocumentWindowShow(_, _, _) = VSConstants.E_NOTIMPL

        member _.OnBeforeLastDocumentUnlock(_, _, _, _) = VSConstants.E_NOTIMPL