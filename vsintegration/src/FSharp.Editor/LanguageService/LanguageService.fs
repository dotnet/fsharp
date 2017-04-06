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
open Microsoft.FSharp.Compiler.SourceCodeServices


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

    member this.AddSingleFileProject(projectId, timeStampAndOptions) =
        projectCache.AddSingleFileProject(projectId,timeStampAndOptions) |> Async.Start


    member this.AddProject (project:Project) =
        projectCache.AddProject project |> Async.Start


    member this.AddProject (projectId:ProjectId) =
        projectCache.AddProject projectId |> Async.Start


    member this.RemoveSingleFileProject(projectId) =
        projectCache.RemoveSingleFileProject projectId |> Async.Start


    /// Clear a project from the project table
    member this.ClearProjectInfo(projectId: ProjectId) =
        projectCache.RemoveProject projectId |> Async.Start

        
    /// Get the exact options for a single-file script
    member this.ComputeSingleFileOptions (fileName) = 
        projectCache.ComputeSingleFileOptions fileName
    

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
    member this.TryGetOptionsForProject(projectId: ProjectId) = 
        projectCache.TryGetOptions projectId

    /// Get the exact options for a document or project
    member this.TryGetOptionsForDocumentOrProject(document: Document) =
        projectCache.TryGetOptionsForDocumentOrProject document
        

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
    internal FSharpLanguageService (package:FSharpPackage) =
    inherit AbstractLanguageService<FSharpPackage, FSharpLanguageService>(package)

    let projectInfoManager = package.ComponentModel.DefaultExportProvider.GetExport<ProjectInfoManager>().Value
    let mutable projectTracker = Unchecked.defaultof<_>

    let projectDisplayNameOf projectFileName = 
        if String.IsNullOrWhiteSpace projectFileName then projectFileName
        else Path.GetFileNameWithoutExtension projectFileName


    override this.Initialize () =
        base.Initialize ()

        this.Workspace.Options <- this.Workspace.Options.WithChangedOption(Completion.CompletionOptions.BlockForCompletionItems, FSharpConstants.FSharpLanguageName, false)
        this.Workspace.Options <- this.Workspace.Options.WithChangedOption(Shared.Options.ServiceFeatureOnOffOptions.ClosedFileDiagnostic, FSharpConstants.FSharpLanguageName, Nullable false)
        
        this.Workspace.DocumentClosed.Add (fun args ->
            projectInfoManager.RemoveSingleFileProject args.Document.Project.Id 
        )
        
        this.Workspace.WorkspaceChanged |> Observable.add (fun args ->
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
        
        Events.SolutionEvents.OnAfterBackgroundSolutionLoadComplete.Add (fun _ ->
            this.Workspace.CurrentSolution.Projects |> Seq.iter !? projectInfoManager.AddProject
        )

        Events.SolutionEvents.OnAfterOpenSolution.Add (fun _ ->
            this.Workspace.CurrentSolution.Projects |> Seq.iter !? projectInfoManager.AddProject
            projectTracker <- this.Workspace.GetProjectTrackerAndInitializeIfNecessary this.SystemServiceProvider
        )

        Events.SolutionEvents.OnAfterCloseSolution.Add (fun _ ->
            projectInfoManager.Clear ()
        )

        let theme = package.ComponentModel.DefaultExportProvider.GetExport<ISetThemeColors>().Value
        theme.SetColors ()
     

    member __.SetupStandAloneFile (fileName: string, workspace: VisualStudioWorkspaceImpl) =
        let loadTime = DateTime.Now
        let options = projectInfoManager.ComputeSingleFileOptions fileName
        let projectFileName = fileName
        let projectDisplayName = projectDisplayNameOf projectFileName
        let projectId = workspace.ProjectTracker.GetOrCreateProjectIdForPath(projectFileName, projectDisplayName)
        projectInfoManager.AddSingleFileProject (projectId, (loadTime, options))

    member __.ProjectTracker = projectTracker
    override __.ContentTypeName = FSharpConstants.FSharpContentTypeName
    override __.LanguageName = FSharpConstants.FSharpLanguageName
    override __.RoslynLanguageName = FSharpConstants.FSharpLanguageName
    override __.LanguageServiceId = new Guid (FSharpConstants.languageServiceGuidString)
    override __.DebuggerLanguageId = DebuggerEnvironment.GetLanguageID ()

    override __.CreateContext (_,_,_,_,_) = raise (System.NotImplementedException ())

    override self.SetupNewTextView textView =
        base.SetupNewTextView textView
        match textView.GetBuffer() with
        | (VSConstants.S_OK, textLines) ->
            let filename = VsTextLines.GetFilename textLines
            self.SetupStandAloneFile (filename, self.Workspace)            
        | _ -> ()

      
