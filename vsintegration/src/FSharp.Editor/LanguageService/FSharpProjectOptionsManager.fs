// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Concurrent
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
        [<Import(typeof<SVsServiceProvider>)>] serviceProvider: System.IServiceProvider,
        settings: EditorOptions
    ) =

    // A table of information about projects, excluding single-file projects.
    let projectOptionsTable = FSharpProjectOptionsTable()

    // A table of information about single-file projects.  Currently we only need the load time of each such file, plus
    // the original options for editing
    let singleFileProjectTable = ConcurrentDictionary<ProjectId, DateTime * FSharpParsingOptions * FSharpProjectOptions>()

    let tryGetOrCreateProjectId (projectFileName:string) =
        let projectDisplayName = projectDisplayNameOf projectFileName
        Some (workspace.ProjectTracker.GetOrCreateProjectIdForPath(projectFileName, projectDisplayName))

    /// Retrieve the projectOptionsTable
    member __.FSharpOptions = projectOptionsTable

    /// Clear a project from the project table
    member this.ClearInfoForProject(projectId:ProjectId) = projectOptionsTable.ClearInfoForProject(projectId)

    /// Clear a project from the single file project table
    member this.ClearInfoForSingleFileProject(projectId) =
        singleFileProjectTable.TryRemove(projectId) |> ignore

    /// Update a project in the single file project table
    member this.AddOrUpdateSingleFileProject(projectId, data) = singleFileProjectTable.[projectId] <- data

    /// Get the exact options for a single-file script
    member this.ComputeSingleFileOptions (tryGetOrCreateProjectId, fileName, loadTime, fileContents) =
        async {
            let extraProjectInfo = Some(box workspace)
            if SourceFile.MustBeSingleFileProject(fileName) then 
                // NOTE: we don't use a unique stamp for single files, instead comparing options structurally.
                // This is because we repeatedly recompute the options.
                let optionsStamp = None 
                let! options, _diagnostics = checkerProvider.Checker.GetProjectOptionsFromScript(fileName, fileContents, loadTime, [| |], ?extraProjectInfo=extraProjectInfo, ?optionsStamp=optionsStamp) 
                // NOTE: we don't use FCS cross-project references from scripts to projects.  THe projects must have been
                // compiled and #r will refer to files on disk
                let referencedProjectFileNames = [| |] 
                let site = ProjectSitesAndFiles.CreateProjectSiteForScript(fileName, referencedProjectFileNames, options)
                let deps, projectOptions = ProjectSitesAndFiles.GetProjectOptionsForProjectSite(settings.LanguageServicePerformance.EnableInMemoryCrossProjectReferences, site, serviceProvider, (tryGetOrCreateProjectId fileName), fileName, options.ExtraProjectInfo, Some projectOptionsTable)
                let parsingOptions, _ = checkerProvider.Checker.GetParsingOptionsFromProjectOptions(projectOptions)
                return (deps, parsingOptions, projectOptions)
            else
                let site = ProjectSitesAndFiles.ProjectSiteOfSingleFile(fileName)
                let deps, projectOptions = ProjectSitesAndFiles.GetProjectOptionsForProjectSite(settings.LanguageServicePerformance.EnableInMemoryCrossProjectReferences, site, serviceProvider, (tryGetOrCreateProjectId fileName), fileName, extraProjectInfo, Some projectOptionsTable)
                let parsingOptions, _ = checkerProvider.Checker.GetParsingOptionsFromProjectOptions(projectOptions)
                return (deps, parsingOptions, projectOptions)
        }

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
        async { 
            let projectId = document.Project.Id

            // The options for a single-file script project are re-requested each time the file is analyzed.  This is because the
            // single-file project may contain #load and #r references which are changing as the user edits, and we may need to re-analyze
            // to determine the latest settings.  FCS keeps a cache to help ensure these are up-to-date.
            match singleFileProjectTable.TryGetValue(projectId) with
            | true, (loadTime, _, _) ->
                try
                    let fileName = document.FilePath
                    let! cancellationToken = Async.CancellationToken
                    let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                    // NOTE: we don't use FCS cross-project references from scripts to projects.  The projects must have been
                    // compiled and #r will refer to files on disk.
                    let tryGetOrCreateProjectId _ = None 
                    let! _referencedProjectFileNames, parsingOptions, projectOptions = this.ComputeSingleFileOptions (tryGetOrCreateProjectId, fileName, loadTime, sourceText.ToString())
                    this.AddOrUpdateSingleFileProject(projectId, (loadTime, parsingOptions, projectOptions))
                    return Some (parsingOptions, None, projectOptions)
                with ex -> 
                    Assert.Exception(ex)
                    return None
            | _ -> return this.TryGetOptionsForProject(projectId)
        }

    /// Get the options for a document or project relevant for syntax processing.
    /// Quicker then TryGetOptionsForDocumentOrProject as it doesn't need to recompute the exact project options for a script.
    member this.TryGetOptionsForEditingDocumentOrProject(document:Document) = 
        let projectId = document.Project.Id
        match singleFileProjectTable.TryGetValue(projectId) with 
        | true, (_loadTime, parsingOptions, originalOptions) -> Some (parsingOptions, originalOptions)
        | _ -> this.TryGetOptionsForProject(projectId) |> Option.map(fun (parsingOptions, _, projectOptions) -> parsingOptions, projectOptions)

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
