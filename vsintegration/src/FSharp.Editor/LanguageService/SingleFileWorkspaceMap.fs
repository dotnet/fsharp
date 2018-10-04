// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

#nowarn "40"

open System
open System.Collections.Concurrent
open System.Collections.Generic
open System.Diagnostics
open System.IO
open System.Linq
open System.Runtime.CompilerServices
open Microsoft.CodeAnalysis
open Microsoft.VisualStudio
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.SiteProvider
open Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem
open Microsoft.VisualStudio.LanguageServices.ProjectSystem
open Microsoft.VisualStudio.Shell.Interop

[<Sealed>]
type internal SingleFileWorkspaceMap(workspace: VisualStudioWorkspaceImpl,
                                     projectInfoManager: FSharpProjectOptionsManager,
                                     projectContextFactory: IWorkspaceProjectContextFactory) =

    let mutable isInitialized = false
    let singleFileProjects = ConcurrentDictionary<_, IWorkspaceProjectContext>()

    let tryRemoveSingleFileProject projectId =
        match singleFileProjects.TryRemove(projectId) with
        | true, project ->
            projectInfoManager.ClearInfoForSingleFileProject(projectId)
            project.Dispose()
        | _ -> ()

    let projectDisplayNameOf projectFileName =
        if String.IsNullOrWhiteSpace projectFileName then projectFileName
        else Path.GetFileNameWithoutExtension projectFileName

    let tryGetOrCreateProjectId (workspace: VisualStudioWorkspaceImpl) (projectFileName: string) =
        let projectDisplayName = projectDisplayNameOf projectFileName
        Some (workspace.ProjectTracker.GetOrCreateProjectIdForPath(projectFileName, projectDisplayName))

    member this.Initialize() =
        if not isInitialized then
            workspace.DocumentClosed.Add <| fun args -> tryRemoveSingleFileProject args.Document.Project.Id
            isInitialized <- true

    member this.AddFile(fileName: string, fileContents: string, hier: IVsHierarchy) =
        if not isInitialized then
            failwith "SingleFileWorkspaceMap is not initialized."

        let loadTime = DateTime.Now
        let projectFileName = fileName
        let projectDisplayName = projectDisplayNameOf projectFileName

        let mutable projectId = workspace.ProjectTracker.GetOrCreateProjectIdForPath(projectFileName, projectDisplayName)

        if isNull (workspace.ProjectTracker.GetProject projectId) then
            let projectContext = projectContextFactory.CreateProjectContext(FSharpConstants.FSharpLanguageName, projectDisplayName, projectFileName, projectId.Id, hier, null)
            
            projectId <- workspace.ProjectTracker.GetOrCreateProjectIdForPath(projectFileName, projectDisplayName)

            projectContext.AddSourceFile(fileName)
            
            singleFileProjects.[projectId] <- projectContext

        let _referencedProjectFileNames, parsingOptions, projectOptions = projectInfoManager.ComputeSingleFileOptions (tryGetOrCreateProjectId workspace, fileName, loadTime, fileContents) |> Async.RunSynchronously
        projectInfoManager.AddOrUpdateSingleFileProject(projectId, (loadTime, parsingOptions, projectOptions))