// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Immutable
open System.ComponentModel.Composition
open System.IO
open System.Linq
open Microsoft.CodeAnalysis
open Microsoft.FSharp.Compiler.CompileOps
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.VisualStudio
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.SiteProvider
open Microsoft.VisualStudio.LanguageServices
open Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem
open Microsoft.VisualStudio.Shell

/// Exposes FCS FSharpProjectOptions information management as MEF component.
//
// This service allows analyzers to get an appropriate FSharpProjectOptions value for a project or single file.
// It also allows a 'cheaper' route to get the project options relevant to parsing (e.g. the #define values).
// The main entrypoints are TryGetOptionsForDocumentOrProject and TryGetOptionsForEditingDocumentOrProject.
[<Export(typeof<FSharpProjectOptionsManager>); Composition.Shared>]
type internal FSharpProjectOptionsManager
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider,
        [<Import(typeof<VisualStudioWorkspace>)>] workspace: VisualStudioWorkspaceImpl,
        [<Import(typeof<MiscellaneousFilesWorkspace>)>] miscWorkspace: MiscellaneousFilesWorkspace,
        [<Import(typeof<SVsServiceProvider>)>] serviceProvider: System.IServiceProvider,
        settings: EditorOptions
    ) =

    // A table of information about projects, excluding single-file projects.
    let projectOptionsTable = FSharpProjectOptionsTable()

    let cache = new FSharpProjectOptionsCache(checkerProvider, settings, projectOptionsTable, serviceProvider)

    let tryGetOrCreateProjectId (projectFileName:string) =
        let projectDisplayName = projectDisplayNameOf projectFileName
        Some (workspace.ProjectTracker.GetOrCreateProjectIdForPath(projectFileName, projectDisplayName))

    do
        workspace.DocumentClosed.Add(fun args -> 
            cache.ClearSingleDocumentCacheById(args.Document.Id)
        )

        miscWorkspace.DocumentClosed.Add(fun args ->
            cache.ClearSingleDocumentCacheById(args.Document.Id)
        )

    /// Retrieve the projectOptionsTable
    member __.FSharpOptions = projectOptionsTable

    /// Clear a project from the project table
    member this.ClearInfoForProject(projectId:ProjectId) = projectOptionsTable.ClearInfoForProject(projectId)

    /// Update the info for a project in the project table
    member this.UpdateProjectInfo(tryGetOrCreateProjectId, projectId, site, userOpName, invalidateConfig) =
        Logger.Log LogEditorFunctionId.LanguageService_UpdateProjectInfo
        projectOptionsTable.AddOrUpdateProject(projectId, (fun isRefresh ->
            let extraProjectInfo = Some(box workspace)
            let referencedProjects, projectOptions = ProjectSitesAndFiles.GetProjectOptionsForProjectSite(settings.LanguageServicePerformance.EnableInMemoryCrossProjectReferences, site, serviceProvider, Some(projectId), site.ProjectFileName, extraProjectInfo,  Some projectOptionsTable)
            if invalidateConfig then checkerProvider.Checker.InvalidateConfiguration(projectOptions, startBackgroundCompileIfAlreadySeen = not isRefresh, userOpName = userOpName + ".UpdateProjectInfo")
            let referencedProjectIds = referencedProjects |> Array.choose tryGetOrCreateProjectId
            let parsingOptions, _ = checkerProvider.Checker.GetParsingOptionsFromProjectOptions(projectOptions)
            referencedProjectIds, parsingOptions, Some site, projectOptions))

    /// Get compilation defines relevant for syntax processing.  
    /// Quicker then TryGetOptionsForDocumentOrProject as it doesn't need to recompute the exact project 
    /// options for a script.
    member this.GetCompilationDefinesForEditingDocument(document:Document) = 
        let projectOptionsOpt = this.TryGetOptionsForProject(document.Project.Id)  
        let parsingOptions = 
            match projectOptionsOpt with 
            | Some (parsingOptions, _site, _projectOptions) -> parsingOptions
            | _ -> { FSharpParsingOptions.Default with IsInteractive = IsScript document.Name }
        CompilerEnvironment.GetCompilationDefinesForEditing parsingOptions

    /// Try and get the Options for a project 
    member this.TryGetOptionsForProject(projectId:ProjectId) = projectOptionsTable.TryGetOptionsForProject(projectId)

    /// Get the exact options for a document or project
    member this.TryGetOptionsForDocumentOrProject(document: Document) =
        cache.TryGetProjectOptionsAsync(document)

    /// Get the options for a document or project relevant for syntax processing.
    /// Quicker then TryGetOptionsForDocumentOrProject as it doesn't need to recompute the exact project options for a script.
    member this.TryGetOptionsForEditingDocumentOrProject(document:Document) =
        async {
            match! this.TryGetOptionsForDocumentOrProject(document) with
            | Some(parsingOptions, _, projectOptions) -> return Some(parsingOptions, projectOptions)
            | _ -> return None
        }

    /// get a siteprovider
    member this.ProvideProjectSiteProvider(project:Project) = provideProjectSiteProvider(workspace, project, serviceProvider, Some projectOptionsTable)

    /// Tell the checker to update the project info for the specified project id
    member this.UpdateProjectInfoWithProjectId(projectId:ProjectId, userOpName, invalidateConfig) =
        let hier = workspace.GetHierarchy(projectId)
        match hier with
        | null -> ()
        | h when (h.IsCapabilityMatch("CPS")) ->
            let project = workspace.CurrentSolution.GetProject(projectId)
            if not (isNull project) then
                let siteProvider = this.ProvideProjectSiteProvider(project)
                let projectSite = siteProvider.GetProjectSite()
                if projectSite.CompilationSourceFiles.Length <> 0 then
                    this.UpdateProjectInfo(tryGetOrCreateProjectId, projectId, projectSite, userOpName, invalidateConfig)
        | _ -> ()

    /// Tell the checker to update the project info for the specified project id
    member this.UpdateDocumentInfoWithProjectId(projectId:ProjectId, documentId:DocumentId, userOpName, invalidateConfig) =
        if workspace.IsDocumentOpen(documentId) then
            this.UpdateProjectInfoWithProjectId(projectId, userOpName, invalidateConfig)

    [<Export>]
    /// This handles commandline change notifications from the Dotnet Project-system
    /// Prior to VS 15.7 path contained path to project file, post 15.7 contains target binpath
    /// binpath is more accurate because a project file can have multiple in memory projects based on configuration
    member this.HandleCommandLineChanges(path:string, sources:ImmutableArray<CommandLineSourceFile>, references:ImmutableArray<CommandLineReference>, options:ImmutableArray<string>) =
        use _logBlock = Logger.LogBlock(LogEditorFunctionId.LanguageService_HandleCommandLineArgs)

        let projectId =
            match workspace.ProjectTracker.TryGetProjectByBinPath(path) with
            | true, project -> project.Id
            | false, _ -> workspace.ProjectTracker.GetOrCreateProjectIdForPath(path, projectDisplayNameOf path)
        let project =  workspace.ProjectTracker.GetProject(projectId)
        let path = project.ProjectFilePath
        let fullPath p =
            if Path.IsPathRooted(p) || path = null then p
            else Path.Combine(Path.GetDirectoryName(path), p)
        let sourcePaths = sources |> Seq.map(fun s -> fullPath s.Path) |> Seq.toArray
        let referencePaths = references |> Seq.map(fun r -> fullPath r.Reference) |> Seq.toArray

        projectOptionsTable.SetOptionsWithProjectId(projectId, sourcePaths, referencePaths, options.ToArray())
        this.UpdateProjectInfoWithProjectId(projectId, "HandleCommandLineChanges", invalidateConfig=true)

    member __.Checker = checkerProvider.Checker
