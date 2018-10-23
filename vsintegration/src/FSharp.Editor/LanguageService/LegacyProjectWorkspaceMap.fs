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
type internal LegacyProjectWorkspaceMap(workspace: VisualStudioWorkspaceImpl, 
                                        solution: IVsSolution, 
                                        projectInfoManager: FSharpProjectOptionsManager,
                                        projectContextFactory: IWorkspaceProjectContextFactory,
                                        serviceProvider: IServiceProvider) as this =

    let invalidPathChars = set (Path.GetInvalidPathChars())
    let optionsAssociation = ConditionalWeakTable<IWorkspaceProjectContext, string[]>()
    let isPathWellFormed (path: string) = not (String.IsNullOrWhiteSpace path) && path |> Seq.forall (fun c -> not (Set.contains c invalidPathChars))
   
    let legacyProjectLookup = ConcurrentDictionary()

    let tryGetOrCreateProjectId (workspace: VisualStudioWorkspaceImpl) (projectFileName: string) =
        let projectDisplayName = projectDisplayNameOf projectFileName
        Some (workspace.ProjectTracker.GetOrCreateProjectIdForPath(projectFileName, projectDisplayName))

    member this.Initialize() =
        solution.AdviseSolutionEvents(this) |> ignore

    /// Sync the Roslyn information for the project held in 'projectContext' to match the information given by 'site'.
    /// Also sync the info in ProjectInfoManager if necessary.
    member this.SyncLegacyProject(projectId: ProjectId, projectContext: IWorkspaceProjectContext, site: IProjectSite, workspace: VisualStudioWorkspaceImpl, forceUpdate, userOpName) =
        let wellFormedFilePathSetIgnoreCase (paths: seq<string>) =
            HashSet(paths |> Seq.filter isPathWellFormed |> Seq.map (fun s -> try Path.GetFullPath(s) with _ -> s), StringComparer.OrdinalIgnoreCase)

        let mutable updated = forceUpdate

        // Sync the source files in projectContext.  Note that these source files are __not__ maintained in order in projectContext
        // as edits are made. It seems this is ok because the source file list is only used to drive roslyn per-file checking.
        let updatedFiles = site.CompilationSourceFiles |> wellFormedFilePathSetIgnoreCase
        let originalFiles =  
            match legacyProjectLookup.TryGetValue(projectId) with
            | true, (originalFiles, _) -> originalFiles
            | _ -> HashSet()
        
        for file in updatedFiles do
            if not(originalFiles.Contains(file)) then
                projectContext.AddSourceFile(file)
                updated <- true

        for file in originalFiles do
            if not(updatedFiles.Contains(file)) then
                projectContext.RemoveSourceFile(file)
                updated <- true

        let updatedRefs = site.CompilationReferences |> wellFormedFilePathSetIgnoreCase
        let originalRefs =
            match legacyProjectLookup.TryGetValue(projectId) with
            | true, (_, originalRefs) -> originalRefs
            | _ -> HashSet()

        for ref in updatedRefs do
            if not(originalRefs.Contains(ref)) then
                projectContext.AddMetadataReference(ref, MetadataReferenceProperties.Assembly)
                updated <- true

        for ref in originalRefs do
            if not(updatedRefs.Contains(ref)) then
                projectContext.RemoveMetadataReference(ref)
                updated <- true

        // Update the project options association
        let ok,originalOptions = optionsAssociation.TryGetValue(projectContext)
        let updatedOptions = site.CompilationOptions
        if not ok || originalOptions <> updatedOptions then 

            // OK, project options have changed, try to fake out Roslyn to convince it to reparse things.
            // Calling SetOptions fails because the CPS project system being used by the F# project system 
            // imlpementation at the moment has no command line parser installed, so we remove/add all the files 
            // instead.  A change of flags doesn't happen very often and the remove/add is fast in any case.
            //projectContext.SetOptions(String.concat " " updatedOptions)
            for file in updatedFiles do
                projectContext.RemoveSourceFile(file)
                projectContext.AddSourceFile(file)

            // Record the last seen options as an associated value
            if ok then optionsAssociation.Remove(projectContext) |> ignore
            optionsAssociation.Add(projectContext, updatedOptions)

            updated <- true

        // update the cached options
        if updated then
            projectInfoManager.UpdateProjectInfo(tryGetOrCreateProjectId workspace, projectId, site, userOpName + ".SyncLegacyProject", invalidateConfig=true)

        let info = (updatedFiles, updatedRefs)
        legacyProjectLookup.AddOrUpdate(projectId, info, fun _ _ -> info) |> ignore

    member this.SetupLegacyProjectFile(siteProvider: IProvideProjectSite, workspace: VisualStudioWorkspaceImpl, userOpName) =
        let userOpName = userOpName + ".SetupProjectFile"
        let  rec setup (site: IProjectSite) =
            let projectGuid = Guid(site.ProjectGuid)
            let projectFileName = site.ProjectFileName
            let projectDisplayName = projectDisplayNameOf projectFileName

            // This projectId is not guaranteed to be the same ProjectId that will actually be created once we call CreateProjectContext
            // in Roslyn versions once https://github.com/dotnet/roslyn/pull/26931 is merged. Roslyn will still guarantee that once
            // there is a project in the workspace with the same path, it'll return the ID of that. So this is sufficient to use
            // in that case as long as we only use it to call GetProject.
            let fakeProjectId = workspace.ProjectTracker.GetOrCreateProjectIdForPath(projectFileName, projectDisplayName)

            if isNull (workspace.ProjectTracker.GetProject fakeProjectId) then
                let hierarchy =
                    site.ProjectProvider
                    |> Option.map (fun p -> p :?> IVsHierarchy)
                    |> Option.toObj

                // Roslyn is expecting site to be an IVsHierarchy.
                // It just so happens that the object that implements IProvideProjectSite is also
                // an IVsHierarchy. This assertion is to ensure that the assumption holds true.
                Debug.Assert(not (isNull hierarchy), "About to CreateProjectContext with a non-hierarchy site")

                let projectContext = 
                    projectContextFactory.CreateProjectContext(
                        FSharpConstants.FSharpLanguageName,
                        projectDisplayName,
                        projectFileName,
                        projectGuid,
                        hierarchy,
                        Option.toObj site.CompilationBinOutputPath)
                
                // The real project ID that was actually added. See comments for fakeProjectId why this one is actually good.
                let realProjectId = workspace.ProjectTracker.GetOrCreateProjectIdForPath(projectFileName, projectDisplayName)

                // Sync IProjectSite --> projectContext, and IProjectSite --> ProjectInfoManage
                this.SyncLegacyProject(realProjectId, projectContext, site, workspace, forceUpdate=true, userOpName=userOpName)

                site.BuildErrorReporter <- Some (projectContext :?> Microsoft.VisualStudio.Shell.Interop.IVsLanguageServiceBuildErrorReporter2)

                // TODO: consider forceUpdate = false here.  forceUpdate=true may be causing repeated computation?
                site.AdviseProjectSiteChanges(FSharpConstants.FSharpLanguageServiceCallbackName, 
                                              AdviseProjectSiteChanges(fun () -> this.SyncLegacyProject(realProjectId, projectContext, site, workspace, forceUpdate=true, userOpName="AdviseProjectSiteChanges."+userOpName)))

                site.AdviseProjectSiteClosed(FSharpConstants.FSharpLanguageServiceCallbackName, 
                                             AdviseProjectSiteChanges(fun () -> 
                                                projectInfoManager.ClearInfoForProject(realProjectId)
                                                optionsAssociation.Remove(projectContext) |> ignore
                                                projectContext.Dispose()))

                for referencedSite in ProjectSitesAndFiles.GetReferencedProjectSites(Some realProjectId, site, serviceProvider, Some (workspace :>obj), Some projectInfoManager.FSharpOptions ) do
                    setup referencedSite

        setup (siteProvider.GetProjectSite()) 

    interface IVsSolutionEvents with

        member __.OnAfterCloseSolution(_) = VSConstants.S_OK

        member __.OnAfterLoadProject(_, _) = VSConstants.S_OK

        member __.OnAfterOpenProject(hier, _) = 
            match hier with
            | :? IProvideProjectSite as siteProvider ->
                this.SetupLegacyProjectFile(siteProvider, workspace, "LegacyProjectWorkspaceMap.Initialize")
            | _ -> ()
            VSConstants.S_OK

        member __.OnAfterOpenSolution(_, _) = VSConstants.S_OK

        member __.OnBeforeCloseProject(hier, _) = 
            match hier with
            | :? IProvideProjectSite as siteProvider ->
                let site = siteProvider.GetProjectSite()
                let projectFileName = site.ProjectFileName
                let projectDisplayName = projectDisplayNameOf projectFileName
                let projectId = workspace.ProjectTracker.GetOrCreateProjectIdForPath(projectFileName, projectDisplayName)
                legacyProjectLookup.TryRemove(projectId) |> ignore
            | _ -> ()
            VSConstants.S_OK

        member __.OnBeforeCloseSolution(_) = VSConstants.S_OK

        member __.OnBeforeUnloadProject(_, _) = VSConstants.S_OK

        member __.OnQueryCloseProject(_, _, _) = VSConstants.S_OK

        member __.OnQueryCloseSolution(_, _) = VSConstants.S_OK

        member __.OnQueryUnloadProject(_, _) = VSConstants.S_OK