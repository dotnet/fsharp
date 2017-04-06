// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

#nowarn "40"

open System
open System.IO
open System.ComponentModel.Composition
open System.Runtime.InteropServices
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Diagnostics
open Microsoft.CodeAnalysis.Completion
open Microsoft.CodeAnalysis.Options
open Microsoft.VisualStudio
open Microsoft.VisualStudio.TextManager.Interop
open Microsoft.VisualStudio.LanguageServices.Implementation.LanguageService
open Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.ComponentModelHost
open Microsoft.VisualStudio.LanguageServices.ProjectSystem
open Microsoft.VisualStudio.LanguageServices.Implementation.TaskList
open Microsoft.FSharp.Compiler.SourceCodeServices
open System.Collections.Generic
open Microsoft.VisualStudio.Editor
open System.Collections.Concurrent



// Workaround to access non-public settings persistence type.
// GetService( ) with this will work as long as the GUID matches the real type.
[<Guid (FSharpConstants.svsSettingsPersistenceManagerGuidString)>]
type internal SVsSettingsPersistenceManager = class end


// Exposes FSharpChecker as MEF export
[<Export (typeof<FSharpCheckerProvider>); Composition.Shared>]
type internal FSharpCheckerProvider [<ImportingConstructor>]
    (   analyzerService: IDiagnosticAnalyzerService
    ) =
    let checker = 
        lazy
            let checker = FSharpChecker.Create(projectCacheSize = 200, keepAllBackgroundResolutions = false)

            // This is one half of the bridge between the F# background builder and the Roslyn analysis engine.
            // When the F# background builder refreshes the background semantic build context for a file,
            // we request Roslyn to reanalyze that individual file.
            checker.BeforeBackgroundFileCheck.Add (fun (fileName, extraProjectInfo) ->  
               async {
                try match extraProjectInfo with 
                    | Some (:? Workspace as workspace) -> 
                        let solution = workspace.CurrentSolution
                        let documentIds = solution.GetDocumentIdsWithFilePath(fileName)
                        if not documentIds.IsEmpty then 
                            analyzerService.Reanalyze(workspace,documentIds=documentIds)
                    | _ -> ()
                with ex -> Assert.Exception(ex)
               } |> Async.StartImmediate
            )
            checker

    member this.Checker = checker.Value


// Exposes project information as MEF component
[<Export(typeof<ProjectInfoManager>); Composition.Shared>]
type internal ProjectInfoManager [<ImportingConstructor>]
    (   checkerProvider: FSharpCheckerProvider,
        workspace : VisualStudioWorkspaceImpl
    ) =
    let projectCache = ProjectOptionsCache(checkerProvider.Checker, workspace)

    //let getProjectHierarchyPairs (references:EnvDTE.Project seq) =


    //let rec referencedProjectsOf (fileName, loadTime, hierarchy:IVsHierarchy, extraProjectInfo) =
    //    getProjectHierarchyPairs (hierarchy.GetReferencedProjects())
    //    |> Seq.choose (fun (_proj, hier) -> maybe {
    //        let! path = hier.TryGetOutputAssemblyPath ()
    //        let! options = getProjectOptionsForProjectSite (fileName, loadTime,hierarchy, extraProjectInfo)
    //        return  (path, options)
    //    })|> Seq.toArray

    //and getProjectOptionsForProjectSite ( fileName,loadTime,hierarchy,  extraProjectInfo) = maybe {
    let getProjectOptionsForProjectSite ( fileName,loadTime,options:FSharpProjectOptions,  extraProjectInfo) = maybe {
        //let! name = hierarchy.TryGetName ()
        //let files = hierarchy.GetFilesOnDisk ()        
        return
            {   ProjectFileName = options.ProjectFileName
                //ProjectFileNames = files |> List.toArray
                ProjectFileNames = options.ProjectFileNames
                OtherOptions = [||]
                //ReferencedProjects = referencedProjectsOf (fileName, loadTime,hierarchy,  extraProjectInfo)
                ReferencedProjects = options.ReferencedProjects
                IsIncompleteTypeCheckEnvironment = false
                UseScriptResolutionRules = SourceFile.MustBeSingleFileProject fileName
                LoadTime = loadTime
                UnresolvedReferences = None
                OriginalLoadReferences = []
                ExtraProjectInfo=extraProjectInfo 
            }  

    }

    member __.Checker = checkerProvider.Checker


    member __.GetProjectOptionsForProjectSite ( fileName, loadTime,options, extraProjectInfo) =
        getProjectOptionsForProjectSite ( fileName,loadTime,options,  extraProjectInfo)


    /// Get the exact options for a single-file script
    member self.ComputeSingleFileOptions (fileName, loadTime, fileContents, workspace: Workspace) = async {
        let solution = workspace.CurrentSolution
        let docId = solution.GetDocumentIdsWithFilePath fileName |> Seq.head
        let project = solution.GetProject docId.ProjectId

        let options = ProjectOptionsCache.toFSharpProjectOptions workspace projectCache.ProjectTable project
        
        let extraProjectInfo = Some (box workspace)
        if SourceFile.MustBeSingleFileProject fileName then 
        //if isScriptFile fileName then 
            let! _options, _diagnostics = self.Checker.GetProjectOptionsFromScript (fileName, fileContents, loadTime, [| |], ?extraProjectInfo=extraProjectInfo) 
            //ProjectOptionsCache.fsxProjectOptions fileName (Some workspace)
            return getProjectOptionsForProjectSite ( fileName, loadTime,options,extraProjectInfo)
        else
            //ProjectOptionsCache.singleFileProjectOptions fileName
            return getProjectOptionsForProjectSite ( fileName, loadTime,options, extraProjectInfo)
    }

    /// Get the exact options for a document or project
    member self.TryGetOptionsForDocumentOrProject(document: Document) = asyncMaybe { 
        let projectId = document.Project.Id
        // The options for a single-file script project are re-requested each time the file is analyzed.  This is because the
        // single-file project may contain #load and #r references which are changing as the user edits, and we may need to re-analyze
        // to determine the latest settings.  FCS keeps a cache to help ensure these are up-to-date.
        match tryGet projectId projectCache.SingleFileProjectTable with
        | Some (loadTime,_) ->
            let fileName = document.FilePath
            let! cancellationToken = Async.CancellationToken |> liftAsync
            let! sourceText = document.GetTextAsync(cancellationToken)
            //let options = ProjectOptionsCache.toFSharpProjectOptions document.Project.Solution.Workspace projectCache.ProjectTable document.Project
            let! options = self.ComputeSingleFileOptions( fileName, loadTime,sourceText.ToString(), document.Project.Solution.Workspace)
            projectCache.SingleFileProjectTable.[projectId] <- (loadTime, options)
            return options
        | None ->
            match self.TryGetOptionsForProject projectId with
            | Some options ->
                return options
            | None ->
                return! None
    }

    member this.GetProjectReferenceInfo (hierarchy:IVsHierarchy) =
        let solution = ServiceProvider.GlobalProvider.GetService<SVsSolution,IVsSolution>()
        (hierarchy.GetReferencedProjects ()) |> Seq.choose (fun proj ->
            match solution.GetProjectOfUniqueName proj.UniqueName with
            | VSConstants.S_OK, hierarchy -> Some (proj, hierarchy, hierarchy.TryGetProjectGuid()) | _ -> None
        )
        
    
    //member this.GetProjectHierarchyPairs (hierarchy:IVsHierarchy) =
    //    getProjectHierarchyPairs (hierarchy.GetReferencedProjects ()) 

    member this.AddSingleFileProject(projectId, timeStampAndOptions) =
        projectCache.AddSingleFileProject(projectId,timeStampAndOptions) |> Async.Start


    member this.AddProject (project:Project) =
        projectCache.AddProject project |> Async.Start


    member this.SingleFileProjectTable = projectCache.SingleFileProjectTable

    member this.AddProject (projectId:ProjectId) =
        projectCache.AddProject projectId |> Async.Start


    member this.RemoveSingleFileProject(projectId) =
        projectCache.RemoveSingleFileProject projectId |> Async.Start


    /// Clear a project from the project table
    member this.ClearProjectInfo(projectId: ProjectId) =
        projectCache.RemoveProject projectId |> Async.Start

        
    ///// Get the exact options for a single-file script
    //member this.ComputeSingleFileOptions (fileName, loadTime, fileContents,solution,hierarchy, workspace: Workspace) =
    //    projectCache.ComputeSingleFileOptions (fileName, loadTime, fileContents, solution,hierarchy,workspace)

    /// Update the info for a project in the project table
    member this.UpdateProjectInfo (projectId: ProjectId) =
        projectCache.UpdateProject projectId |> Async.Start
       

    /// Get compilation defines relevant for syntax processing.  
    /// Quicker then TryGetOptionsForDocumentOrProject as it doesn't need to recompute the exact project 
    /// options for a script.
    member this.GetCompilationDefinesForEditingDocument(document: Document) = 
        let projectOptionsOpt = this.TryGetOptionsForProject(document.Project.Id)  
        let otherOptions = 
            match projectOptionsOpt with 
            | None -> []
            | Some options -> options.OtherOptions |> Array.toList
        CompilerEnvironment.GetCompilationDefinesForEditing(document.Name, otherOptions)

    /// Get the options for a project
    member this.TryGetOptionsForProject(projectId: ProjectId) : FSharpProjectOptions option = 
        projectCache.TryGetOptions projectId

    ///// Get the exact options for a document or project
    //member this.TryGetOptionsForDocumentOrProject(document: Document) =
    //    projectCache.TryGetOptionsForDocumentOrProject document
        

    /// Get the options for a document or project relevant for syntax processing.
    /// Quicker then TryGetOptionsForDocumentOrProject as it doesn't need to recompute the exact project options for a script.
    member this.TryGetOptionsForEditingDocumentOrProject (document: Document) = 
        projectCache.TryGetOptionsForEditingDocumentOrProject document
    
    member self.Clear () = projectCache.Clear ()


// Used to expose FSharpChecker/ProjectInfo manager to diagnostic providers
// Diagnostic providers can be executed in environment that does not use MEF so they can rely only
// on services exposed by the workspace
type internal FSharpCheckerWorkspaceService =
    inherit Microsoft.CodeAnalysis.Host.IWorkspaceService
    abstract Checker: FSharpChecker
    abstract ProjectInfoManager: ProjectInfoManager


type internal RoamingProfileStorageLocation (keyName: string) =
    inherit OptionStorageLocation ()
    
    member __.GetKeyNameForLanguage (languageName: string) =
        let unsubstitutedKeyName = keyName
        if isNull languageName then unsubstitutedKeyName else
        let substituteLanguageName = 
            if languageName = FSharpConstants.FSharpLanguageName then "FSharp" else languageName
        unsubstitutedKeyName.Replace ("%LANGUAGE%", substituteLanguageName)
 

[<Composition.Shared>]
[<Microsoft.CodeAnalysis.Host.Mef.ExportWorkspaceServiceFactory (typeof<FSharpCheckerWorkspaceService>, Microsoft.CodeAnalysis.Host.Mef.ServiceLayer.Default)>]
type internal FSharpCheckerWorkspaceServiceFactory [<Composition.ImportingConstructor>]
    (   checkerProvider: FSharpCheckerProvider,
        projectInfoManager: ProjectInfoManager
    ) =
    interface Microsoft.CodeAnalysis.Host.Mef.IWorkspaceServiceFactory with
        member this.CreateService(_workspaceServices) =
            upcast { new FSharpCheckerWorkspaceService with
                member this.Checker = checkerProvider.Checker
                member this.ProjectInfoManager = projectInfoManager }


type
    [<Guid(FSharpConstants.packageGuidString)>]
    [<ProvideLanguageService(languageService = typeof<FSharpLanguageService>,
                             strLanguageName = FSharpConstants.FSharpLanguageName,
                             languageResourceID = 100,
                             MatchBraces = true,
                             MatchBracesAtCaret = true,
                             ShowCompletion = true,
                             ShowMatchingBrace = true,
                             ShowSmartIndent = true,
                             EnableAsyncCompletion = true,
                             QuickInfo = true,
                             DefaultToInsertSpaces = true,
                             CodeSense = true,
                             DefaultToNonHotURLs = true,
                             EnableCommenting = true,
                             CodeSenseDelay = 100,
                             ShowDropDownOptions = true)>]
    internal FSharpPackage () =
    inherit AbstractPackage<FSharpPackage, FSharpLanguageService>()
    
    override __.RoslynLanguageName = FSharpConstants.FSharpLanguageName

    override self.CreateWorkspace () = self.ComponentModel.GetService<VisualStudioWorkspaceImpl>()

    override self.CreateLanguageService () = FSharpLanguageService self

    override this.CreateEditorFactories() = Seq.empty<IVsEditorFactory>

    override this.RegisterMiscellaneousFilesWorkspaceInformation _ = ()
    
and 
    [<Guid (FSharpConstants.languageServiceGuidString)>]
    [<ProvideLanguageExtension (typeof<FSharpLanguageService>, ".fs")>]
    [<ProvideLanguageExtension (typeof<FSharpLanguageService>, ".fsi")>]
    [<ProvideLanguageExtension (typeof<FSharpLanguageService>, ".fsx")>]
    [<ProvideLanguageExtension (typeof<FSharpLanguageService>, ".fsscript")>]
    [<ProvideLanguageExtension (typeof<FSharpLanguageService>, ".ml")>]
    [<ProvideLanguageExtension (typeof<FSharpLanguageService>, ".mli")>]
    [<ProvideEditorExtension (FSharpConstants.editorFactoryGuidString, ".fs", 97)>]
    [<ProvideEditorExtension (FSharpConstants.editorFactoryGuidString, ".fsi", 97)>]
    [<ProvideEditorExtension (FSharpConstants.editorFactoryGuidString, ".fsx", 97)>]
    [<ProvideEditorExtension (FSharpConstants.editorFactoryGuidString, ".fsscript", 97)>]
    [<ProvideEditorExtension (FSharpConstants.editorFactoryGuidString, ".ml", 97)>]
    [<ProvideEditorExtension (FSharpConstants.editorFactoryGuidString, ".mli", 97)>]
    internal FSharpLanguageService (package:FSharpPackage) as self =
    inherit AbstractLanguageService<FSharpPackage, FSharpLanguageService>(package)

    let workspace = package.Workspace
    let projectInfoManager = package.ComponentModel.DefaultExportProvider.GetExport<ProjectInfoManager>().Value
    let mutable projectTracker = Unchecked.defaultof<_>

    let projectDisplayNameOf projectFileName = 
        if String.IsNullOrWhiteSpace projectFileName then projectFileName
        else Path.GetFileNameWithoutExtension projectFileName
    
    let singleFileProjects = ConcurrentDictionary<_, AbstractProject>()

    let tryRemoveSingleFileProject projectId =
        match singleFileProjects.TryRemove(projectId) with
        | true, project ->
            projectInfoManager.RemoveSingleFileProject(projectId)
            project.Disconnect()
        | _ -> ()


    let syncProject (project: AbstractProject, projectContext: IWorkspaceProjectContext, hierarchy:IVsHierarchy, forceUpdate) = async {
        let hashSetIgnoreCase x = new HashSet<string>(x, StringComparer.OrdinalIgnoreCase)
        let updatedFiles = hierarchy.GetFilesOnDisk () |> hashSetIgnoreCase
        let workspaceFiles = project.GetCurrentDocuments() |> Seq.map(fun file -> file.FilePath) |> hashSetIgnoreCase

        // If syncing project upon some reference changes, we don't have a mechanism to recognize which references have been added/removed.
        // Hence, the current solution is to force update current project options.
        let mutable updated = forceUpdate
        for file in updatedFiles do
            if not (workspaceFiles.Contains file) then
                projectContext.AddSourceFile file
                updated <- true
        for file in workspaceFiles do
            if not (updatedFiles.Contains file) then
                projectContext.RemoveSourceFile file
                updated <- true
    }

    let rec setup (projectDisplayName, projectFileName, projectId:ProjectId,hierarchy) = async {
        if isNull (workspace.ProjectTracker.GetProject projectId) then
            projectInfoManager.UpdateProjectInfo projectId
            let projectContextFactory = package.ComponentModel.GetService<IWorkspaceProjectContextFactory>()
            let errorReporter = ProjectExternalErrorReporter (projectId, "FS", self.SystemServiceProvider)
            
            let projectContext = 
                projectContextFactory.CreateProjectContext(
                    FSharpConstants.FSharpLanguageName, projectDisplayName, projectFileName, projectId.Id, hierarchy, null, errorReporter
                )
            let project = projectContext :?> AbstractProject
            do! syncProject (project, projectContext, hierarchy, false) 

            projectInfoManager.GetProjectReferenceInfo hierarchy 
            |> Seq.iter (fun (proj,hier,guid) ->
                let projRef = 
                    match guid with
                    | Some guid -> ProjectId.CreateFromSerialized guid |> ProjectReference
                    | None -> ProjectId.CreateNewId () |> ProjectReference
                project.AddProjectReference projRef
                let projFileName = defaultArg (hierarchy.TryGetFilePath ()) proj.FullName
                let projDisplayName = projectDisplayNameOf projectFileName
                setup (projDisplayName,projFileName, projRef.ProjectId,hier )|> Async.Start
            )
        projectInfoManager.AddProject projectId
    }
    

    /// Get the exact options for a single-file script
    member __.ComputeSingleFileOptions (fileName, loadTime, fileContents, hierarchy, workspace: Workspace) = async {
        let extraProjectInfo = Some (box workspace)
        if SourceFile.MustBeSingleFileProject fileName then 
        //if isScriptFile fileName then 
            let! _options, _diagnostics = projectInfoManager.Checker.GetProjectOptionsFromScript (fileName, fileContents, loadTime, [| |], ?extraProjectInfo=extraProjectInfo) 
            //ProjectOptionsCache.fsxProjectOptions fileName (Some workspace)
            return projectInfoManager.GetProjectOptionsForProjectSite ( fileName, loadTime,hierarchy,extraProjectInfo)
        else
            //ProjectOptionsCache.singleFileProjectOptions fileName
            return projectInfoManager.GetProjectOptionsForProjectSite ( fileName, loadTime,hierarchy, extraProjectInfo)
    }


    member self.SetupProjectFile (hierarchy:IVsHierarchy) = 
        asyncMaybe {
            let! projectFileName = hierarchy.TryGetFilePath ()
            let projectDisplayName = projectDisplayNameOf projectFileName
            let projectId = self.Workspace.ProjectTracker.GetOrCreateProjectIdForPath (projectFileName, projectDisplayName)

            setup (projectDisplayName,projectFileName, projectId, hierarchy )|> Async.Start
        } |> Async.Ignore
        

    override self.Initialize () =
        base.Initialize ()

        self.Workspace.Options <- self.Workspace.Options.WithChangedOption (Completion.CompletionOptions.BlockForCompletionItems, FSharpConstants.FSharpLanguageName, false)
        self.Workspace.Options <- self.Workspace.Options.WithChangedOption (Shared.Options.ServiceFeatureOnOffOptions.ClosedFileDiagnostic, FSharpConstants.FSharpLanguageName, Nullable false)
        
        projectTracker <- self.Workspace.GetProjectTrackerAndInitializeIfNecessary self.SystemServiceProvider

        self.Workspace.DocumentClosed.Add (fun args ->
            tryRemoveSingleFileProject args.Document.Project.Id 
        )
        
        self.Workspace.WorkspaceChanged |> Observable.add (fun args ->
            match args.Kind with
            | WorkspaceChangeKind.SolutionAdded ->
                args.NewSolution.Projects |> Seq.iter !? projectInfoManager.AddProject
            | WorkspaceChangeKind.SolutionReloaded ->
                projectInfoManager.Clear ()
                args.NewSolution.Projects |> Seq.iter !? projectInfoManager.AddProject
            | WorkspaceChangeKind.SolutionRemoved
            | WorkspaceChangeKind.SolutionCleared ->
                projectInfoManager.Clear ()
            | WorkspaceChangeKind.ProjectAdded ->
                args.ProjectId ?> projectInfoManager.AddProject 
            | WorkspaceChangeKind.ProjectRemoved ->
                args.ProjectId ?> projectInfoManager.ClearProjectInfo 
            | WorkspaceChangeKind.ProjectReloaded 
            | WorkspaceChangeKind.ProjectChanged ->
                args.ProjectId ?> projectInfoManager.UpdateProjectInfo 
            | _ -> ()
        )

        let setupProjectsAfterSolutionOpen () = async {
            // waits for AfterOpenSolution and then starts projects setup
            do! Async.AwaitEvent Events.SolutionEvents.OnAfterOpenSolution |> Async.Ignore
            use _ = Events.SolutionEvents.OnAfterOpenProject |> Observable.subscribe ( fun args ->
                self.SetupProjectFile args.Hierarchy |> Async.Start
            )
            do! Async.AwaitEvent Events.SolutionEvents.OnAfterCloseSolution |> Async.Ignore
            //do! setupProjectsAfterSolutionOpen () 
        }
        setupProjectsAfterSolutionOpen () |> Async.StartImmediate

        let theme = package.ComponentModel.DefaultExportProvider.GetExport<ISetThemeColors>().Value
        theme.SetColors ()

        let _projects = workspace.CurrentSolution.Projects |> Array.ofSeq
        ()
     

    member self.SetupStandAloneFile (fileName: string, fileContents: string, workspace: VisualStudioWorkspaceImpl) =
        let loadTime = DateTime.Now
        let projectFileName = fileName
        let options = ProjectOptionsCache.singleFileProjectOptions fileName
        let projectDisplayName = projectDisplayNameOf projectFileName
        let projectId = workspace.ProjectTracker.GetOrCreateProjectIdForPath(projectFileName, projectDisplayName)
        self.ComputeSingleFileOptions (fileName, loadTime, fileContents,options, workspace)
        |> Async.map (fun opts -> opts |> Option.iter (fun options ->
            projectInfoManager.AddSingleFileProject (projectId, (loadTime, options)))
        ) |> Async.Start


    member __.ProjectTracker = projectTracker
    override __.ContentTypeName = FSharpConstants.FSharpContentTypeName
    override __.LanguageName = FSharpConstants.FSharpLanguageName
    override __.RoslynLanguageName = FSharpConstants.FSharpLanguageName
    override __.LanguageServiceId = new Guid (FSharpConstants.languageServiceGuidString)
    override __.DebuggerLanguageId = DebuggerEnvironment.GetLanguageID ()

    override __.CreateContext (_,_,_,_,_) = raise (System.NotImplementedException ())

    override self.SetupNewTextView textView =
        base.SetupNewTextView textView
        let textViewAdapter = package.ComponentModel.GetService<IVsEditorAdaptersFactoryService>()
        match textView.GetBuffer () with
        | (VSConstants.S_OK, textLines) ->
            let filename = VsTextLines.GetFilename textLines
            VsRunningDocumentTable.FindDocumentWithoutLocking (package.RunningDocumentTable, filename) 
            |> Option.iter (fun (hier, _) -> 
                hier.TryGetProject()
                |> Option.iter (fun _project ->
                    if not (isScriptFile filename) then 
                        self.SetupProjectFile hier |> Async.Start //RunSynchronously
                    else
                        let fileContents = VsTextLines.GetFileContents(textLines, textViewAdapter)
                        self.SetupStandAloneFile (filename,fileContents, self.Workspace)
                )
            )
        | _ -> ()

      
