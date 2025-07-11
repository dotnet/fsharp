﻿// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor
 
open System
open System.Collections.Generic
open System.Collections.Concurrent
open System.Collections.Immutable
open System.ComponentModel.Composition
open System.IO
open System.Linq
open Microsoft.CodeAnalysis
open FSharp.Compiler
open FSharp.Compiler.CodeAnalysis
open Microsoft.VisualStudio
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.LanguageServices
open Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem
open Microsoft.VisualStudio.Shell
open System.Threading
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.LanguageServices.Implementation.TaskList
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.LanguageServices
open Microsoft.VisualStudio.FSharp.Interactive.Session
open System.Runtime.CompilerServices

open Microsoft.VisualStudio.FSharp.Editor.Extensions
open System.Windows
open Microsoft.VisualStudio
open FSharp.Compiler.Text
open Microsoft.VisualStudio.TextManager.Interop

#nowarn "57"

[<AutoOpen>]
module private FSharpProjectOptionsHelpers =

    let mapCpsProjectToSite(project:Project, cpsCommandLineOptions: IDictionary<ProjectId, string[] * string[]>) =
        let sourcePaths, referencePaths, options =
            match cpsCommandLineOptions.TryGetValue(project.Id) with
            | true, (sourcePaths, options) -> sourcePaths, [||], options
            | false, _ -> [||], [||], [||]
        let mutable errorReporter = Unchecked.defaultof<_>
        {
            new IProjectSite with
                member _.Description = project.Name
                member _.CompilationSourceFiles = sourcePaths
                member _.CompilationOptions =
                    Array.concat [options; referencePaths |> Array.map(fun r -> "-r:" + r)]
                member _.CompilationReferences = referencePaths
                member site.CompilationBinOutputPath = site.CompilationOptions |> Array.tryPick (fun s -> if s.StartsWith("-o:") then Some s.[3..] else None)
                member _.ProjectFileName = project.FilePath
                member _.AdviseProjectSiteChanges(_,_) = ()
                member _.AdviseProjectSiteCleaned(_,_) = ()
                member _.AdviseProjectSiteClosed(_,_) = ()
                member _.IsIncompleteTypeCheckEnvironment = false
                member _.TargetFrameworkMoniker = ""
                member _.ProjectGuid =  project.Id.Id.ToString()
                member _.LoadTime = System.DateTime.Now
                member _.ProjectProvider = None
                member _.BuildErrorReporter with get () = errorReporter and set (v) = errorReporter <- v
        }

    let hasProjectVersionChanged (oldProject: Project) (newProject: Project) =
        oldProject.Version <> newProject.Version

    let hasDependentVersionChanged (oldProject: Project) (newProject: Project) (ct: CancellationToken) =
        let oldProjectRefs = oldProject.ProjectReferences
        let newProjectRefs = newProject.ProjectReferences
        oldProjectRefs.Count() <> newProjectRefs.Count() ||
        (oldProjectRefs, newProjectRefs)
        ||> Seq.exists2 (fun p1 p2 ->
            ct.ThrowIfCancellationRequested()
            let doesProjectIdDiffer = p1.ProjectId <> p2.ProjectId
            let p1 = oldProject.Solution.GetProject(p1.ProjectId)
            let p2 = newProject.Solution.GetProject(p2.ProjectId)
            doesProjectIdDiffer || 
            (
                // For F# projects, just check the version until we have a better in-memory model for them.
                if p1.Language = LanguageNames.FSharp then
                    p1.Version <> p2.Version
                else
                    let v1 = p1.GetDependentVersionAsync(ct).Result
                    let v2 = p2.GetDependentVersionAsync(ct).Result
                    v1 <> v2
            )
        )

    let isProjectInvalidated (oldProject: Project) (newProject: Project) (settings: EditorOptions) ct =
        let hasProjectVersionChanged = hasProjectVersionChanged oldProject newProject
        if settings.LanguageServicePerformance.EnableInMemoryCrossProjectReferences then
            hasProjectVersionChanged || hasDependentVersionChanged oldProject newProject ct
        else
            hasProjectVersionChanged

[<RequireQualifiedAccess>]
type private FSharpProjectOptionsMessage =
    | TryGetOptionsByDocument of Document * AsyncReplyChannel<(FSharpParsingOptions * FSharpProjectOptions) option> * CancellationToken * userOpName: string
    | TryGetOptionsByProject of Project * AsyncReplyChannel<(FSharpParsingOptions * FSharpProjectOptions) option> * CancellationToken
    | ClearOptions of ProjectId
    | ClearSingleFileOptionsCache of DocumentId

[<Sealed>]
type private FSharpProjectOptionsReactor (workspace: Workspace, settings: EditorOptions, _serviceProvider, checkerProvider: FSharpCheckerProvider) =
    let cancellationTokenSource = new CancellationTokenSource()

    // Hack to store command line options from HandleCommandLineChanges
    let cpsCommandLineOptions = ConcurrentDictionary<ProjectId, string[] * string[]>()

    let legacyProjectSites = ConcurrentDictionary<ProjectId, IProjectSite>()

    let cache = ConcurrentDictionary<ProjectId, Project * FSharpParsingOptions * FSharpProjectOptions>()
    let singleFileCache = ConcurrentDictionary<DocumentId, VersionStamp * FSharpParsingOptions * FSharpProjectOptions * ConnectionPointSubscription>()

    // This is used to not constantly emit the same compilation.
    let weakPEReferences = ConditionalWeakTable<Compilation, FSharpReferencedProject>()
    let lastSuccessfulCompilations = ConcurrentDictionary<ProjectId, Compilation>()

    let createPEReference (referencedProject: Project) (comp: Compilation) =
        let projectId = referencedProject.Id

        match weakPEReferences.TryGetValue comp with
        | true, fsRefProj -> fsRefProj
        | _ ->
            let weakComp = WeakReference<Compilation>(comp)
            let getStream =
                fun ct ->
                    let tryStream (comp: Compilation) =
                        let ms = new MemoryStream() // do not dispose the stream as it will be owned on the reference.
                        let emitOptions = Emit.EmitOptions(metadataOnly = true, includePrivateMembers = false, tolerateErrors = true)
                        try
                            let result = comp.Emit(ms, options = emitOptions, cancellationToken = ct)

                            if result.Success then
                                lastSuccessfulCompilations.[projectId] <- comp
                                ms.Position <- 0L
                                ms :> Stream
                                |> Some
                            else
                                ms.Dispose() // it failed, dispose of stream
                                None
                        with
                        | _ ->
                            ms.Dispose() // it failed, dispose of stream
                            None

                    let resultOpt =
                        match weakComp.TryGetTarget() with
                        | true, comp -> tryStream comp
                        | _ -> None

                    match resultOpt with
                    | Some _ -> resultOpt
                    | _ ->
                        match lastSuccessfulCompilations.TryGetValue(projectId) with
                        | true, comp -> tryStream comp
                        | _ -> None                           

            let fsRefProj =
                FSharpReferencedProject.CreatePortableExecutable(
                    referencedProject.OutputFilePath, 
                    DateTime.UtcNow,
                    getStream
                )
            weakPEReferences.Add(comp, fsRefProj)
            fsRefProj

    let rec tryComputeOptionsByFile (document: Document) (ct: CancellationToken) userOpName =
        async {
            let! fileStamp = document.GetTextVersionAsync(ct) |> Async.AwaitTask
            let textViewAndCaret () : (IVsTextView * Position) option = document.TryGetTextViewAndCaretPos()
            match singleFileCache.TryGetValue(document.Id) with
            | false, _ ->
                let! sourceText = document.GetTextAsync(ct) |> Async.AwaitTask
                
                let getProjectOptionsFromScript textViewAndCaret =
                    let caret = textViewAndCaret ()

                    match caret with
                    | None ->
                        checkerProvider.Checker.GetProjectOptionsFromScript(
                            document.FilePath,
                            sourceText.ToFSharpSourceText(),
                            previewEnabled = SessionsProperties.fsiPreview,
                            assumeDotNetFramework = not SessionsProperties.fsiUseNetCore,
                            userOpName = userOpName
                        )

                    | Some(_, caret) ->
                        checkerProvider.Checker.GetProjectOptionsFromScript(
                            document.FilePath,
                            sourceText.ToFSharpSourceText(),
                            caret,
                            previewEnabled = SessionsProperties.fsiPreview,
                            assumeDotNetFramework = not SessionsProperties.fsiUseNetCore,
                            userOpName = userOpName
                        )

                let! scriptProjectOptions, _ = getProjectOptionsFromScript textViewAndCaret
                let project = document.Project

                let otherOptions =
                    if project.IsFSharpMetadata then
                        project.ProjectReferences
                        |> Seq.map (fun x -> "-r:" + project.Solution.GetProject(x.ProjectId).OutputFilePath)
                        |> Array.ofSeq
                        |> Array.append (
                                project.MetadataReferences.OfType<PortableExecutableReference>()
                                |> Seq.map (fun x -> "-r:" + x.FilePath)
                                |> Array.ofSeq)
                    else
                        [||]

                let projectOptions =
                    if isScriptFile document.FilePath then
                        scriptProjectOptions
                    else
                        {
                            ProjectFileName = document.FilePath
                            ProjectId = None
                            SourceFiles = [|document.FilePath|]
                            OtherOptions = otherOptions
                            ReferencedProjects = [||]
                            IsIncompleteTypeCheckEnvironment = false
                            UseScriptResolutionRules = CompilerEnvironment.MustBeSingleFileProject (Path.GetFileName(document.FilePath))
                            LoadTime = DateTime.Now
                            UnresolvedReferences = None
                            OriginalLoadReferences = []
                            Stamp = Some(int64 (fileStamp.GetHashCode()))
                        }

                checkerProvider.Checker.CheckProjectInBackground(projectOptions, userOpName="checkOptions")

                let parsingOptions, _ = checkerProvider.Checker.GetParsingOptionsFromProjectOptions(projectOptions)

                let updateProjectOptions () =
                    async {
                        let! scriptProjectOptions, _ = getProjectOptionsFromScript textViewAndCaret

                        checkerProvider.Checker.CheckProjectInBackground(scriptProjectOptions, userOpName="checkOptions")
                    }
                    |> Async.Start

                let onChangeCaretHandler (_, _newline: int, _oldline: int) = updateProjectOptions ()
                let onKillFocus (_) = updateProjectOptions ()
                let onSetFocus (_) = updateProjectOptions ()

                let addToCacheAndSubscribe value =
                    match value with
                    | _, parsingOptions, projectOptions, _ ->
                        let subscription =
                            match textViewAndCaret () with
                            | Some(textView, _) ->
                                subscribeToTextViewEvents (textView, (Some onChangeCaretHandler), (Some onKillFocus), (Some onSetFocus))
                            | None -> None

                        (fileStamp, parsingOptions, projectOptions, subscription)

                singleFileCache.AddOrUpdate(
                    document.Id, // The key to the cache
                    (fun _ value -> addToCacheAndSubscribe value), // Function to add the cached value if the key does not exist
                    (fun _ _ value -> value), // Function to update the value if the key exists
                    (fileStamp, parsingOptions, projectOptions, None) // The value to add or update
                )
                |> ignore

                return Some(parsingOptions, projectOptions)

            | true, (fileStamp2, parsingOptions, projectOptions, _) ->
                if fileStamp <> fileStamp2 then
                    match singleFileCache.TryRemove(document.Id) with
                    | true, (_, _, _, Some subscription) -> subscription.Dispose()
                    | _ -> ()

                    return! tryComputeOptionsByFile document ct userOpName
                else
                    return Some(parsingOptions, projectOptions)
        }

    let tryGetProjectSite (project: Project) =
        // Cps
        if cpsCommandLineOptions.ContainsKey project.Id then
            Some (mapCpsProjectToSite(project, cpsCommandLineOptions))
        else
            // Legacy
            match legacyProjectSites.TryGetValue project.Id with
            | true, site -> Some site
            | _ -> None
    
    let rec tryComputeOptions (project: Project) ct =
        async {
            let projectId = project.Id
            match cache.TryGetValue(projectId) with
            | false, _ ->

                // Because this code can be kicked off before the hack, HandleCommandLineChanges, occurs,
                //     the command line options will not be available and we should bail if one of the project references does not give us anything.
                let mutable canBail = false
            
                let referencedProjects = ResizeArray()

                if settings.LanguageServicePerformance.EnableInMemoryCrossProjectReferences then
                    for projectReference in project.ProjectReferences do
                        let referencedProject = project.Solution.GetProject(projectReference.ProjectId)
                        if referencedProject.Language = FSharpConstants.FSharpLanguageName then
                            match! tryComputeOptions referencedProject ct with
                            | None -> canBail <- true
                            | Some(_, projectOptions) -> referencedProjects.Add(FSharpReferencedProject.CreateFSharp(referencedProject.OutputFilePath, projectOptions))
                        elif referencedProject.SupportsCompilation then
                            let! comp = referencedProject.GetCompilationAsync(ct) |> Async.AwaitTask
                            let peRef = createPEReference referencedProject comp
                            referencedProjects.Add(peRef)

                if canBail then
                    return None
                else

                match tryGetProjectSite project with
                | None -> return None
                | Some projectSite ->             

                let otherOptions =
                    project.ProjectReferences
                    |> Seq.map (fun x -> "-r:" + project.Solution.GetProject(x.ProjectId).OutputFilePath)
                    |> Array.ofSeq
                    |> Array.append (
                            project.MetadataReferences.OfType<PortableExecutableReference>()
                            |> Seq.map (fun x -> "-r:" + x.FilePath)
                            |> Array.ofSeq
                            |> Array.append (
                                    // Clear any references from CompilationOptions. 
                                    // We get the references from Project.ProjectReferences/Project.MetadataReferences.
                                    projectSite.CompilationOptions
                                    |> Array.filter (fun x -> not (x.Contains("-r:")))
                                )
                        )

                let projectOptions =
                    {
                        ProjectFileName = projectSite.ProjectFileName
                        ProjectId = Some(projectId.ToFSharpProjectIdString())
                        SourceFiles = projectSite.CompilationSourceFiles
                        OtherOptions = otherOptions
                        ReferencedProjects = referencedProjects.ToArray()
                        IsIncompleteTypeCheckEnvironment = projectSite.IsIncompleteTypeCheckEnvironment
                        UseScriptResolutionRules = CompilerEnvironment.MustBeSingleFileProject (Path.GetFileName(project.FilePath))
                        LoadTime = projectSite.LoadTime
                        UnresolvedReferences = None
                        OriginalLoadReferences = []
                        Stamp = Some(int64 (project.Version.GetHashCode()))
                    }

                // This can happen if we didn't receive the callback from HandleCommandLineChanges yet.
                if Array.isEmpty projectOptions.SourceFiles then
                    return None
                else
                    // Clear any caches that need clearing and invalidate the project.
                    let currentSolution = workspace.CurrentSolution
                    let projectsToClearCache =
                        cache
                        |> Seq.filter (fun pair -> not (currentSolution.ContainsProject pair.Key))

                    if not (Seq.isEmpty projectsToClearCache) then
                        projectsToClearCache
                        |> Seq.iter (fun pair -> cache.TryRemove pair.Key |> ignore)
                        let options =
                            projectsToClearCache
                            |> Seq.map (fun pair ->
                                let _, _, projectOptions = pair.Value
                                projectOptions)
                        checkerProvider.Checker.ClearCache(options, userOpName = "tryComputeOptions")

                    lastSuccessfulCompilations.ToArray()
                    |> Array.iter (fun pair ->
                        if not (currentSolution.ContainsProject(pair.Key)) then
                            lastSuccessfulCompilations.TryRemove(pair.Key) |> ignore
                    )

                    checkerProvider.Checker.InvalidateConfiguration(projectOptions, startBackgroundCompile = false, userOpName = "computeOptions")

                    let parsingOptions, _ = checkerProvider.Checker.GetParsingOptionsFromProjectOptions(projectOptions)

                    cache.[projectId] <- (project, parsingOptions, projectOptions)

                    return Some(parsingOptions, projectOptions)
  
            | true, (oldProject, parsingOptions, projectOptions) ->
                if isProjectInvalidated oldProject project settings ct then
                    cache.TryRemove(projectId) |> ignore
                    return! tryComputeOptions project ct
                else
                    return Some(parsingOptions, projectOptions)
        }

    let loop (agent: MailboxProcessor<FSharpProjectOptionsMessage>) =
        async {
            while true do
                match! agent.Receive() with
                | FSharpProjectOptionsMessage.TryGetOptionsByDocument(document, reply, ct, userOpName) ->
                    if ct.IsCancellationRequested then
                        reply.Reply None
                    else
                        try
                            // For now, disallow miscellaneous workspace since we are using the hacky F# miscellaneous files project.
                            if document.Project.Solution.Workspace.Kind = WorkspaceKind.MiscellaneousFiles then
                                reply.Reply None
                            elif document.Project.IsFSharpMiscellaneousOrMetadata then
                                let! options = tryComputeOptionsByFile document ct userOpName
                                if ct.IsCancellationRequested then
                                    reply.Reply None
                                else
                                    reply.Reply options
                            else
                                // We only care about the latest project in the workspace's solution.
                                // We do this to prevent any possible cache thrashing in FCS.
                                let project = document.Project.Solution.Workspace.CurrentSolution.GetProject(document.Project.Id)
                                if not (isNull project) then
                                    let! options = tryComputeOptions project ct
                                    if ct.IsCancellationRequested then
                                        reply.Reply None
                                    else
                                        reply.Reply options
                                else
                                    reply.Reply None
                        with
                        | _ ->
                            reply.Reply None

                | FSharpProjectOptionsMessage.TryGetOptionsByProject(project, reply, ct) ->
                    if ct.IsCancellationRequested then
                        reply.Reply None
                    else
                        try
                            if project.Solution.Workspace.Kind = WorkspaceKind.MiscellaneousFiles || project.IsFSharpMiscellaneousOrMetadata then
                                reply.Reply None
                            else
                                // We only care about the latest project in the workspace's solution.
                                // We do this to prevent any possible cache thrashing in FCS.
                                let project = project.Solution.Workspace.CurrentSolution.GetProject(project.Id)
                                if not (isNull project) then
                                    let! options = tryComputeOptions project ct
                                    if ct.IsCancellationRequested then
                                        reply.Reply None
                                    else
                                        reply.Reply options
                                else
                                    reply.Reply None
                        with
                        | _ ->
                            reply.Reply None

                | FSharpProjectOptionsMessage.ClearOptions(projectId) ->
                    match cache.TryRemove(projectId) with
                    | true, (_, _, projectOptions) ->
                        lastSuccessfulCompilations.TryRemove(projectId) |> ignore
                        checkerProvider.Checker.ClearCache([projectOptions])
                    | _ ->
                        ()
                    legacyProjectSites.TryRemove(projectId) |> ignore
                | FSharpProjectOptionsMessage.ClearSingleFileOptionsCache(documentId) ->
                    match singleFileCache.TryRemove(documentId) with
                    | true, (_, _, _, Some subscription) -> subscription.Dispose()
                    | _ -> ()
        }

    let agent = MailboxProcessor.Start((fun agent -> loop agent), cancellationToken = cancellationTokenSource.Token)

    member _.TryGetOptionsByProjectAsync(project, ct) =
        agent.PostAndAsyncReply(fun reply -> FSharpProjectOptionsMessage.TryGetOptionsByProject(project, reply, ct))

    member _.TryGetOptionsByDocumentAsync(document, ct, userOpName) =
        agent.PostAndAsyncReply(fun reply -> FSharpProjectOptionsMessage.TryGetOptionsByDocument(document, reply, ct, userOpName))

    member _.ClearOptionsByProjectId(projectId) =
        agent.Post(FSharpProjectOptionsMessage.ClearOptions(projectId))

    member _.ClearSingleFileOptionsCache(documentId) =
        agent.Post(FSharpProjectOptionsMessage.ClearSingleFileOptionsCache(documentId))

    member _.SetCpsCommandLineOptions(projectId, sourcePaths, options) =
        cpsCommandLineOptions.[projectId] <- (sourcePaths, options)

    member _.SetLegacyProjectSite (projectId, projectSite) =
        legacyProjectSites.[projectId] <- projectSite

    member _.TryGetCachedOptionsByProjectId(projectId) =
        match cache.TryGetValue(projectId) with
        | true, result -> Some(result)
        | _ -> None

    member _.ClearAllCaches() =
        cpsCommandLineOptions.Clear()
        legacyProjectSites.Clear()
        cache.Clear()
        singleFileCache.Clear()
        lastSuccessfulCompilations.Clear()

    interface IDisposable with
        member _.Dispose() = 
            cancellationTokenSource.Cancel()
            cancellationTokenSource.Dispose() 
            (agent :> IDisposable).Dispose()

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
        [<Import(typeof<VisualStudioWorkspace>)>] workspace: VisualStudioWorkspace,
        [<Import(typeof<SVsServiceProvider>)>] serviceProvider: System.IServiceProvider,
        settings: EditorOptions
    ) =

    let projectDisplayNameOf projectFileName =
        if String.IsNullOrWhiteSpace projectFileName then projectFileName
        else Path.GetFileNameWithoutExtension projectFileName

    let reactor = new FSharpProjectOptionsReactor(workspace, settings, serviceProvider, checkerProvider)

    do
        // We need to listen to this event for lifecycle purposes.
        workspace.WorkspaceChanged.Add(fun args ->
            match args.Kind with
            | WorkspaceChangeKind.ProjectRemoved -> reactor.ClearOptionsByProjectId(args.ProjectId)
            | _ -> ()
        )

        workspace.DocumentClosed.Add(fun args ->
            let doc = args.Document
            let proj = doc.Project
            if proj.Language = LanguageNames.FSharp && proj.IsFSharpMiscellaneousOrMetadata then
                reactor.ClearSingleFileOptionsCache(doc.Id)
        )

    member _.SetLegacyProjectSite (projectId, projectSite) =
        reactor.SetLegacyProjectSite (projectId, projectSite)

    /// Clear a project from the project table
    member this.ClearInfoForProject(projectId:ProjectId) = 
        reactor.ClearOptionsByProjectId(projectId)

    member this.ClearSingleFileOptionsCache(documentId) =
        reactor.ClearSingleFileOptionsCache(documentId)

    /// Get compilation defines relevant for syntax processing.  
    /// Quicker then TryGetOptionsForDocumentOrProject as it doesn't need to recompute the exact project 
    /// options for a script.
    member this.GetCompilationDefinesForEditingDocument(document:Document) = 
        let parsingOptions =
            match reactor.TryGetCachedOptionsByProjectId(document.Project.Id) with
            | Some (_, parsingOptions, _) -> parsingOptions
            | _ -> { FSharpParsingOptions.Default with IsInteractive = CompilerEnvironment.IsScriptFile document.Name }
        CompilerEnvironment.GetCompilationDefinesForEditing parsingOptions     

    member this.TryGetOptionsByProject(project) =
        reactor.TryGetOptionsByProjectAsync(project)

    /// Get the exact options for a document or project
    member this.TryGetOptionsForDocumentOrProject(document: Document, cancellationToken, userOpName) =
        async { 
            match! reactor.TryGetOptionsByDocumentAsync(document, cancellationToken, userOpName) with
            | Some(parsingOptions, projectOptions) ->
                return Some(parsingOptions, None, projectOptions)
            | _ ->
                return None
        }

    /// Get the exact options for a document or project relevant for syntax processing.
    member this.TryGetOptionsForEditingDocumentOrProject(document:Document, cancellationToken, userOpName) = 
        async {
            let! result = this.TryGetOptionsForDocumentOrProject(document, cancellationToken, userOpName) 
            return result |> Option.map(fun (parsingOptions, _, projectOptions) -> parsingOptions, projectOptions)
        }

    /// Get the options for a document or project relevant for syntax processing.
    /// Quicker it doesn't need to recompute the exact project options for a script.
    member this.TryGetQuickParsingOptionsForEditingDocumentOrProject(document:Document) = 
        match reactor.TryGetCachedOptionsByProjectId(document.Project.Id) with
        | Some (_, parsingOptions, _) -> parsingOptions
        | _ -> { FSharpParsingOptions.Default with IsInteractive = CompilerEnvironment.IsScriptFile document.Name }

    member this.ClearAllCaches() =
        reactor.ClearAllCaches()

    [<Export>]
    /// This handles commandline change notifications from the Dotnet Project-system
    /// Prior to VS 15.7 path contained path to project file, post 15.7 contains target binpath
    /// binpath is more accurate because a project file can have multiple in memory projects based on configuration
    member _.HandleCommandLineChanges(path:string, sources:ImmutableArray<CommandLineSourceFile>, _references:ImmutableArray<CommandLineReference>, options:ImmutableArray<string>) =
        use _logBlock = Logger.LogBlock(LogEditorFunctionId.LanguageService_HandleCommandLineArgs)

        let projectId =
            match Microsoft.CodeAnalysis.ExternalAccess.FSharp.LanguageServices.FSharpVisualStudioWorkspaceExtensions.TryGetProjectIdByBinPath(workspace, path) with
            | true, projectId -> projectId
            | false, _ -> Microsoft.CodeAnalysis.ExternalAccess.FSharp.LanguageServices.FSharpVisualStudioWorkspaceExtensions.GetOrCreateProjectIdForPath(workspace, path, projectDisplayNameOf path)
        let path = Microsoft.CodeAnalysis.ExternalAccess.FSharp.LanguageServices.FSharpVisualStudioWorkspaceExtensions.GetProjectFilePath(workspace, projectId)

        let getFullPath p =
            let p' =
                if Path.IsPathRooted(p) || path = null then p
                else Path.Combine(Path.GetDirectoryName(path), p)
            Path.GetFullPathSafe(p')

        let sourcePaths = sources |> Seq.map(fun s -> getFullPath s.Path) |> Seq.toArray

        reactor.SetCpsCommandLineOptions(projectId, sourcePaths, options.ToArray())

    member _.Checker = checkerProvider.Checker

    member _.MetadataAsSource = checkerProvider.MetadataAsSource
