// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Generic
open System.Collections.Concurrent
open System.Collections.Immutable
open System.IO
open System.Linq
open System.Runtime.CompilerServices
open System.Threading
open System.Threading.Tasks
open System.Windows
open FSharp.Compiler
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Text
open Microsoft.CodeAnalysis
open Microsoft.VisualStudio
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Interactive.Session
open Microsoft.VisualStudio.FSharp.Editor.Extensions

open CancellableTasks

#nowarn "57"

/// Runs an action once no trigger has arrived for the given delay.
[<Sealed>]
type internal Debouncer(wait: CancellationToken -> Task, action: unit -> Task<unit>) =
    let mutable pending: CancellationTokenSource | null = null

    // Dispose and a newer Trigger take the source out first: a run that finds it gone must not start its action.
    let tryTake (cts: CancellationTokenSource) =
        obj.ReferenceEquals(Interlocked.CompareExchange(&pending, null, cts), cts)

    new(delay: TimeSpan, action: unit -> Task<unit>) = new Debouncer((fun token -> Task.Delay(delay, token)), action)

    member _.Trigger() : Task =
        let cts = new CancellationTokenSource()
        let token = cts.Token

        match Interlocked.Exchange(&pending, cts) with
        | null -> ()
        | previous ->
            previous.Cancel()
            previous.Dispose()

        backgroundTask {
            try
                do! wait token

                if tryTake cts then
                    cts.Dispose()
                    do! action ()
            with :? OperationCanceledException ->
                ()
        }

    interface IDisposable with
        member _.Dispose() =
            match Interlocked.Exchange(&pending, null) with
            | null -> ()
            | cts ->
                cts.Cancel()
                cts.Dispose()

[<AutoOpen>]
module internal FSharpProjectOptionsHelpers =

    let mapCpsProjectToSite (project: Project, cpsCommandLineOptions: IDictionary<ProjectId, struct (string[] * string[])>) =
        let sourcePaths, referencePaths, options =
            match cpsCommandLineOptions.TryGetValue(project.Id) with
            | true, (sourcePaths, options) -> sourcePaths, [||], options
            | false, _ -> [||], [||], [||]

        let mutable errorReporter = Unchecked.defaultof<_>

        { new IProjectSite with
            member _.Description = project.Name
            member _.CompilationSourceFiles = sourcePaths

            member _.CompilationOptions =
                Array.concat [ options; referencePaths |> Array.map (fun r -> "-r:" + r) ]

            member _.CompilationReferences = referencePaths

            member site.CompilationBinOutputPath =
                site.CompilationOptions
                |> Array.tryPickV (fun s -> if s.StartsWith("-o:") then ValueSome s.[3..] else ValueNone)

            member _.ProjectFileName = project.FilePath
            member _.AdviseProjectSiteChanges(_, _) = ()
            member _.AdviseProjectSiteCleaned(_, _) = ()
            member _.AdviseProjectSiteClosed(_, _) = ()
            member _.IsIncompleteTypeCheckEnvironment = false
            member _.TargetFrameworkMoniker = ""
            member _.ProjectGuid = project.Id.Id.ToString()
            member _.LoadTime = System.DateTime.Now
            member _.ProjectProvider = None

            member _.BuildErrorReporter
                with get () = errorReporter
                and set (v) = errorReporter <- v
        }

    let inline hasProjectVersionChanged (oldProject: Project) (newProject: Project) =
        oldProject.Version <> newProject.Version

    let hasDependentVersionChanged (oldProject: Project) (newProject: Project) =
        cancellableTask {
            let! ct = CancellableTask.getCancellationToken ()
            let oldProjectMetadataRefs = oldProject.MetadataReferences
            let newProjectMetadataRefs = newProject.MetadataReferences

            if oldProjectMetadataRefs.Count <> newProjectMetadataRefs.Count then
                return true
            else

                let oldProjectRefs = oldProject.ProjectReferences
                let newProjectRefs = newProject.ProjectReferences

                if oldProjectRefs.Count() <> newProjectRefs.Count() then
                    return true
                else
                    let mutable result = false
                    let mutable enum1 = oldProjectRefs.GetEnumerator()
                    let mutable enum2 = newProjectRefs.GetEnumerator()

                    while not result && enum1.MoveNext() && enum2.MoveNext() do
                        ct.ThrowIfCancellationRequested()
                        let p1 = enum1.Current
                        let p2 = enum2.Current
                        let doesProjectIdDiffer = p1.ProjectId <> p2.ProjectId
                        let p1 = oldProject.Solution.GetProject(p1.ProjectId)
                        let p2 = newProject.Solution.GetProject(p2.ProjectId)

                        if doesProjectIdDiffer then
                            result <- true
                        elif p1.IsFSharp then
                            result <- p1.Version <> p2.Version
                        else
                            let! v1 = p1.GetDependentVersionAsync(ct)
                            and! v2 = p2.GetDependentVersionAsync(ct)
                            result <- v1 <> v2

                    return result
        }

    let isProjectInvalidated (oldProject: Project) (newProject: Project) =
        cancellableTask {
            let hasProjectVersionChanged = hasProjectVersionChanged oldProject newProject

            if newProject.AreFSharpInMemoryCrossProjectReferencesEnabled then
                if hasProjectVersionChanged then
                    return true
                else
                    return! hasDependentVersionChanged oldProject newProject
            else
                return hasProjectVersionChanged
        }

type private SingleFileCacheEntry =
    {
        Project: Project
        FileStamp: VersionStamp
        ParsingOptions: FSharpParsingOptions
        ProjectOptions: FSharpProjectOptions
        Subscription: IDisposable voption
    }

module internal PEReferenceVersion =

    // Source generators come only from analyzer references. Which of those carry one is known to Roslyn's
    // out-of-process host alone: asking here would load the generators into Visual Studio.
    let private mayRunSourceGenerators (project: Project) =
        let solution = project.Solution

        let hasAnalyzerReferences (projectId: ProjectId) =
            match solution.GetProject projectId with
            | null -> false
            | dependency -> dependency.AnalyzerReferences.Count > 0

        project.AnalyzerReferences.Count > 0
        || solution.GetProjectDependencyGraph().GetProjectsThatThisProjectTransitivelyDependsOn(project.Id)
           |> Seq.exists hasAnalyzerReferences

    /// The version a referenced project's in-memory PE reference is kept for. A source generator can turn an
    /// edit that leaves the dependent semantic version standing into different declarations.
    let getAsync (referencedProject: Project) (ct: CancellationToken) =
        if mayRunSourceGenerators referencedProject then
            referencedProject.GetDependentVersionAsync(ct)
        else
            referencedProject.GetDependentSemanticVersionAsync(ct)

/// <summary>
/// The in-memory PE reference of a referenced project, kept while the project's
/// <c>PEReferenceVersion</c> is unchanged. Roslyn recreates
/// <see cref="T:Microsoft.CodeAnalysis.Compilation"/> instances freely - on every solution fork,
/// and under memory pressure because it holds the final compilation weakly - and a reference
/// created per instance carries a fresh stamp that invalidates every FCS cache keyed on it.
/// </summary>
[<Sealed>]
type private PEReferenceCacheEntry(version: VersionStamp, compilation: Compilation) =
    // Pinned until the first emit result, so the reader can always be materialised.
    let mutable pinned = compilation
    let latest = WeakReference<Compilation>(compilation)

    member _.Version = version

    member _.TryGetCompilation() =
        match pinned with
        | null ->
            match latest.TryGetTarget() with
            | true, compilation -> ValueSome compilation
            | _ -> ValueNone
        | pinned -> ValueSome pinned

    member _.Emitted() = pinned <- null

[<RequireQualifiedAccess>]
type private FSharpProjectOptionsMessage =
    | TryGetOptionsByDocument of
        Document *
        AsyncReplyChannel<struct (FSharpParsingOptions * FSharpProjectOptions) voption> *
        CancellationToken *
        userOpName: string
    | TryGetOptionsByProject of
        Project *
        AsyncReplyChannel<struct (FSharpParsingOptions * FSharpProjectOptions) voption> *
        CancellationToken
    | ClearOptions of ProjectId
    | ClearSingleFileOptionsCache of DocumentId
    | ClearAllSingleFileOptions

/// One lock for what the reactor publishes. Closing the solution moves the epoch, and a request publishes only
/// under the epoch it took when it started. Nothing that blocks (I/O, COM, FCS) may run inside the lock.
[<Sealed>]
type internal PublicationGate() =
    let gate = obj ()
    let mutable epoch = 0

    member _.Current = lock gate (fun () -> epoch)

    member _.Run(action: unit -> 'T) = lock gate action

    member _.Bump(clear: unit -> 'T) =
        lock gate (fun () ->
            epoch <- epoch + 1
            clear ())

    member _.TryPublish(captured: int, publish: unit -> unit) =
        lock gate (fun () ->
            if epoch = captured then
                publish ()
                true
            else
                false)

[<Sealed>]
type private FSharpProjectOptionsReactor(checker: FSharpChecker, fileChangeWatcher: IFSharpFileChangeWatcher) =
    let cancellationTokenSource = new CancellationTokenSource()

    // Store command line options
    let commandLineOptions =
        ConcurrentDictionary<ProjectId, struct (string[] * string[])>()

    let legacyProjectSites = ConcurrentDictionary<ProjectId, IProjectSite>()

    let cache =
        ConcurrentDictionary<ProjectId, struct (Project * FSharpParsingOptions * FSharpProjectOptions)>()

    let singleFileCache = ConcurrentDictionary<DocumentId, SingleFileCacheEntry>()

    let peReferences =
        ConcurrentDictionary<ProjectId, PEReferenceCacheEntry * FSharpReferencedProject>()

    let lastSuccessfulCompilations = ConcurrentDictionary<ProjectId, Compilation>()

    let scriptUpdatedEvent = Event<FSharpProjectOptions>()

    let disposeSingleFileCacheEntry ({ Subscription = subscription }: SingleFileCacheEntry) =
        subscription |> ValueOption.iter _.Dispose()

    let clearSingleFileCache () =
        for entry in singleFileCache do
            match singleFileCache.TryRemove(entry.Key) with
            | true, cacheEntry -> disposeSingleFileCacheEntry cacheEntry
            | _ -> ()

    let referenceChangeTracker = new FSharpReferenceChangeTracker(fileChangeWatcher)
    let referenceStamps = referenceChangeTracker :> IReferenceStamps

    let publication = PublicationGate()

    let referenceWatches = Dictionary<ProjectId, HashSet<string>>()

    let referencePaths (projectOptions: FSharpProjectOptions) =
        let paths = HashSet<string>(StringComparer.OrdinalIgnoreCase)

        for option in projectOptions.OtherOptions do
            if option.StartsWith("-r:", StringComparison.Ordinal) then
                paths.Add(option.Substring "-r:".Length) |> ignore

        paths

    let watchReferenceFilesNoLock (projectId: ProjectId) (projectOptions: FSharpProjectOptions) =
        let paths = referencePaths projectOptions

        match referenceWatches.TryGetValue projectId with
        | true, previous ->
            for path in previous do
                if not (paths.Contains path) then
                    referenceChangeTracker.StopWatchingReference path

            for path in paths do
                if not (previous.Contains path) then
                    referenceChangeTracker.StartWatchingReference path
        | _ ->
            for path in paths do
                referenceChangeTracker.StartWatchingReference path

        if paths.Count > 0 then
            referenceWatches[projectId] <- paths
        else
            referenceWatches.Remove projectId |> ignore

    let clearReferenceWatchesNoLock (projectId: ProjectId) =
        match referenceWatches.TryGetValue projectId with
        | true, paths ->
            referenceWatches.Remove projectId |> ignore

            for path in paths do
                referenceChangeTracker.StopWatchingReference path
        | _ -> ()

    let clearReferenceWatches (projectId: ProjectId) =
        publication.Run(fun () -> clearReferenceWatchesNoLock projectId)

    let clearAllReferenceWatchesNoLock () =
        for KeyValue(_, paths) in referenceWatches do
            for path in paths do
                referenceChangeTracker.StopWatchingReference path

        referenceWatches.Clear()

    let shouldWatchReferences (project: Project) =
        checker.UsesTransparentCompiler
        && project.IsTransparentCompilerSnapshotReuseEnabled

    let reconcileReferenceWatchesNoLock (projectId: ProjectId) (projectOptions: FSharpProjectOptions) shouldWatch =
        let watched = referenceWatches.ContainsKey projectId

        if shouldWatch then
            if not watched then
                watchReferenceFilesNoLock projectId projectOptions
        elif watched then
            clearReferenceWatchesNoLock projectId

    let buildPEReference (epoch: int) (referencedProject: Project) (entry: PEReferenceCacheEntry) =
        let projectId = referencedProject.Id
        let mutable stamp = DateTime.UtcNow

        // Getting a C# reference assembly can fail if there are compilation errors that cannot be resolved.
        // To mitigate this, we store the last successful compilation of a C# project and re-use it until we get a new successful compilation.
        let getStream =
            fun ct ->
                let tryStream (comp: Compilation) =
                    let ms = new MemoryStream() // do not dispose the stream as it will be owned on the reference.

                    let emitOptions =
                        Emit.EmitOptions(metadataOnly = true, includePrivateMembers = false, tolerateErrors = true)

                    try
                        let result = comp.Emit(ms, options = emitOptions, cancellationToken = ct)

                        if result.Success then
                            entry.Emitted()

                            if publication.Current = epoch then
                                lastSuccessfulCompilations.[projectId] <- comp

                            ms.Position <- 0L
                            ms :> Stream |> Some
                        else
                            entry.Emitted()
                            ms.Dispose() // it failed, dispose of stream
                            None
                    with
                    | :? OperationCanceledException ->
                        // Since we cancelled, keep the compilation pinned and update the stamp.
                        stamp <- DateTime.UtcNow
                        ms.Dispose()
                        None
                    | _ ->
                        entry.Emitted()
                        ms.Dispose() // it failed, dispose of stream
                        None

                let resultOpt =
                    match entry.TryGetCompilation() with
                    | ValueSome comp -> tryStream comp
                    | ValueNone -> None

                match resultOpt with
                | Some _ -> resultOpt
                | _ ->
                    match lastSuccessfulCompilations.TryGetValue(projectId) with
                    | true, comp -> tryStream comp
                    | _ -> None

        let getStamp = fun () -> stamp

        FSharpReferencedProject.PEReference(getStamp, DelayedILModuleReader(referencedProject.OutputFilePath, getStream))

    let tryGetPEReference (referencedProject: Project) (version: VersionStamp) =
        match peReferences.TryGetValue referencedProject.Id with
        | true, (entry, fsRefProj) when entry.Version = version -> ValueSome fsRefProj
        | _ -> ValueNone

    let createPEReference (epoch: int) (referencedProject: Project) (version: VersionStamp) (comp: Compilation) =
        let entry = PEReferenceCacheEntry(version, comp)
        let fsRefProj = buildPEReference epoch referencedProject entry

        publication.TryPublish(epoch, fun () -> peReferences.[referencedProject.Id] <- (entry, fsRefProj))
        |> ignore

        fsRefProj

    let rec tryComputeOptionsBySingleScriptOrFileUnder (epoch: int) (document: Document) userOpName =
        cancellableTask {
            let! ct = CancellableTask.getCancellationToken ()
            let! fileStamp = document.GetTextVersionAsync(ct)

            match singleFileCache.TryGetValue(document.Id) with
            | false, _ ->
                let! sourceText = document.GetTextAsync(ct)

                // FCS reads the caret only to skip resolving the `#r "nuget: …"` line being typed, and only scripts have those.
                let focusedCaret =
                    if isScriptFile document.FilePath then
                        FocusedCaret.TryGetOrCreate sourceText
                    else
                        ValueNone

                let getProjectOptionsFromScript (text: ISourceText) =
                    checker.GetProjectOptionsFromScript(
                        document.FilePath,
                        text,
                        ?caret = (focusedCaret |> ValueOption.toOption |> Option.bind _.Position),
                        previewEnabled = SessionsProperties.fsiPreview,
                        assumeDotNetFramework = not SessionsProperties.fsiUseNetCore,
                        userOpName = userOpName
                    )

                let! scriptProjectOptions, _ = getProjectOptionsFromScript (sourceText.ToFSharpSourceText())
                let project = document.Project

                let otherOptions =
                    if project.IsFSharpMetadata then
                        [|
                            for x in project.MetadataReferences.OfType<PortableExecutableReference>() do
                                yield "-r:" + x.FilePath
                            for x in project.ProjectReferences do
                                yield "-r:" + project.Solution.GetProject(x.ProjectId).OutputFilePath
                        |]
                    else
                        [||]

                let projectOptions =
                    if isScriptFile document.FilePath then
                        scriptUpdatedEvent.Trigger(scriptProjectOptions)
                        scriptProjectOptions
                    else
                        {
                            ProjectFileName = document.FilePath
                            ProjectId = None
                            SourceFiles = [| document.FilePath |]
                            OtherOptions = otherOptions
                            ReferencedProjects = [||]
                            IsIncompleteTypeCheckEnvironment = false
                            UseScriptResolutionRules = CompilerEnvironment.MustBeSingleFileProject(Path.GetFileName(document.FilePath))
                            LoadTime = DateTime.Now
                            UnresolvedReferences = None
                            OriginalLoadReferences = []
                            Stamp = Some(int64 (fileStamp.GetHashCode()))
                        }

                let parsingOptions, _ = checker.GetParsingOptionsFromProjectOptions(projectOptions)

                let debouncer =
                    new Debouncer(
                        TimeSpan.FromMilliseconds 500.,
                        fun () ->
                            backgroundTask {
                                let! scriptProjectOptions, _ =
                                    getProjectOptionsFromScript (sourceText.Container.CurrentText.ToFSharpSourceText())

                                do! checker.NotifyFileChanged(document.FilePath, scriptProjectOptions)
                            }
                    )

                let updateProjectOptions () = debouncer.Trigger() |> ignore

                let addToCacheAndSubscribe (entry: SingleFileCacheEntry) =
                    let textViewSubscription =
                        focusedCaret |> ValueOption.map _.LineChanged.Subscribe(updateProjectOptions)

                    let subscription =
                        ValueSome
                            { new IDisposable with
                                member _.Dispose() =
                                    textViewSubscription |> ValueOption.iter _.Dispose()
                                    (debouncer :> IDisposable).Dispose()
                            }

                    { entry with
                        Subscription = subscription
                    }

                let entry =
                    addToCacheAndSubscribe
                        {
                            Project = document.Project
                            FileStamp = fileStamp
                            ParsingOptions = parsingOptions
                            ProjectOptions = projectOptions
                            Subscription = ValueNone
                        }

                if publication.TryPublish(epoch, fun () -> singleFileCache.[document.Id] <- entry) then
                    return ValueSome struct (parsingOptions, projectOptions)
                else
                    disposeSingleFileCacheEntry entry
                    return ValueNone

            | true,
              {
                  Project = oldProject
                  FileStamp = oldFileStamp
                  ParsingOptions = parsingOptions
                  ProjectOptions = projectOptions
                  Subscription = _
              } ->
                let! isInvalidated =
                    if fileStamp <> oldFileStamp then
                        CancellableTask.singleton true
                    else
                        isProjectInvalidated document.Project oldProject

                if isInvalidated then
                    match singleFileCache.TryRemove(document.Id) with
                    | true, cacheEntry -> disposeSingleFileCacheEntry cacheEntry
                    | _ -> ()

                    return! tryComputeOptionsBySingleScriptOrFileUnder epoch document userOpName
                else
                    return ValueSome struct (parsingOptions, projectOptions)
        }

    let tryComputeOptionsBySingleScriptOrFile (document: Document) userOpName =
        cancellableTask { return! tryComputeOptionsBySingleScriptOrFileUnder publication.Current document userOpName }

    let tryGetProjectSite (project: Project) =
        // Cps
        if commandLineOptions.ContainsKey project.Id then
            ValueSome(mapCpsProjectToSite (project, commandLineOptions))
        else
            // Legacy
            match legacyProjectSites.TryGetValue project.Id with
            | true, site -> ValueSome site
            | _ -> ValueNone

    let invalidateStampsOfReplacedMetadataReferences (oldProject: Project) (project: Project) =
        for reference in project.GetChanges(oldProject).GetAddedMetadataReferences() do
            match reference with
            | :? PortableExecutableReference as peReference ->
                match peReference.FilePath with
                | null -> ()
                | path -> referenceStamps.Invalidate path
            | _ -> ()

    let rec tryComputeOptionsUnder (epoch: int) (project: Project) =
        cancellableTask {
            let projectId = project.Id
            let! ct = CancellableTask.getCancellationToken ()

            match cache.TryGetValue(projectId) with
            | false, _ ->

                // Because this code can be kicked off before the hack, HandleCommandLineChanges, occurs,
                //     the command line options will not be available and we should bail if one of the project references does not give us anything.
                let mutable canBail = false

                let referencedProjects = ResizeArray()

                if project.AreFSharpInMemoryCrossProjectReferencesEnabled then
                    for projectReference in project.ProjectReferences do
                        let referencedProject = project.Solution.GetProject(projectReference.ProjectId)

                        if referencedProject.Language = FSharpConstants.FSharpLanguageName then
                            match! tryComputeOptionsUnder epoch referencedProject with
                            | ValueNone -> canBail <- true
                            | ValueSome struct (_, projectOptions) ->
                                referencedProjects.Add(
                                    FSharpReferencedProject.FSharpReference(referencedProject.OutputFilePath, projectOptions)
                                )
                        elif referencedProject.SupportsCompilation then
                            let! version = PEReferenceVersion.getAsync referencedProject ct

                            match tryGetPEReference referencedProject version with
                            | ValueSome peRef -> referencedProjects.Add peRef
                            | ValueNone ->
                                let! comp = referencedProject.GetCompilationAsync(ct)
                                referencedProjects.Add(createPEReference epoch referencedProject version comp)

                if canBail then
                    return ValueNone
                else
                    match tryGetProjectSite project with
                    | ValueNone -> return ValueNone
                    | ValueSome projectSite ->

                        let otherOptions =
                            [|
                                // Clear any references from CompilationOptions.
                                // We get the references from Project.ProjectReferences/Project.MetadataReferences.
                                // A path map belongs to the build output: applied here it rewrites the file name of
                                // every range imported from a referenced project, and navigation finds no document.
                                for x in projectSite.CompilationOptions do
                                    if not (x.Contains("-r:") || x.StartsWith("--pathmap:", StringComparison.Ordinal)) then
                                        x

                                for x in project.MetadataReferences.OfType<PortableExecutableReference>() do
                                    "-r:" + x.FilePath

                                for x in project.ProjectReferences do
                                    "-r:" + project.Solution.GetProject(x.ProjectId).OutputFilePath

                                // In the IDE we always ignore all #line directives for all purposes.  This means
                                // IDE features work correctly within generated source files, but diagnostics are
                                // reported in the IDE with respect to the generated source, and will not unify with
                                // diagnostics from the build.
                                "--ignorelinedirectives"
                            |]

                        let! ver = project.GetDependentVersionAsync(ct)

                        let projectOptions =
                            {
                                ProjectFileName = projectSite.ProjectFileName
                                ProjectId = Some(projectId.ToFSharpProjectIdString())
                                SourceFiles = projectSite.CompilationSourceFiles
                                OtherOptions = otherOptions
                                ReferencedProjects = referencedProjects.ToArray()
                                IsIncompleteTypeCheckEnvironment = projectSite.IsIncompleteTypeCheckEnvironment
                                UseScriptResolutionRules = CompilerEnvironment.MustBeSingleFileProject(Path.GetFileName(project.FilePath))
                                LoadTime = projectSite.LoadTime
                                UnresolvedReferences = None
                                OriginalLoadReferences = []
                                Stamp = Some(int64 (ver.GetHashCode()))
                            }

                        // This can happen if we didn't receive the callback from HandleCommandLineChanges yet.
                        if Array.isEmpty projectOptions.SourceFiles then
                            return ValueNone
                        else
                            // Clear any caches that need clearing and invalidate the project.
                            let currentSolution = project.Solution.Workspace.CurrentSolution

                            let projectsToClearCache =
                                cache |> Seq.filter (fun pair -> not (currentSolution.ContainsProject pair.Key))

                            if not (Seq.isEmpty projectsToClearCache) then
                                projectsToClearCache
                                |> Seq.iter (fun pair ->
                                    cache.TryRemove pair.Key |> ignore
                                    clearReferenceWatches pair.Key)

                                let options =
                                    projectsToClearCache
                                    |> Seq.map (fun pair ->
                                        let struct (_, _, projectOptions) = pair.Value
                                        projectOptions)

                                checker.ClearCache(options, userOpName = "tryComputeOptions")

                            lastSuccessfulCompilations.ToArray()
                            |> Array.iter (fun pair ->
                                if not (currentSolution.ContainsProject(pair.Key)) then
                                    lastSuccessfulCompilations.TryRemove(pair.Key) |> ignore)

                            peReferences.ToArray()
                            |> Array.iter (fun pair ->
                                if not (currentSolution.ContainsProject(pair.Key)) then
                                    peReferences.TryRemove(pair.Key) |> ignore)

                            checker.InvalidateConfiguration(projectOptions, userOpName = "tryComputeOptions")

                            let parsingOptions, _ = checker.GetParsingOptionsFromProjectOptions(projectOptions)

                            let shouldWatch = shouldWatchReferences project

                            let published =
                                publication.TryPublish(
                                    epoch,
                                    fun () ->
                                        cache.[projectId] <- struct (project, parsingOptions, projectOptions)

                                        if shouldWatch then
                                            watchReferenceFilesNoLock projectId projectOptions
                                        else
                                            clearReferenceWatchesNoLock projectId
                                )

                            if published then
                                return ValueSome struct (parsingOptions, projectOptions)
                            else
                                // The solution closed meanwhile: take back what FCS was told about these options.
                                checker.ClearCache([ projectOptions ], userOpName = "tryComputeOptions")
                                return ValueNone

            | true, struct (oldProject, parsingOptions, projectOptions) ->
                let! isInvalidated = isProjectInvalidated oldProject project

                if isInvalidated then
                    invalidateStampsOfReplacedMetadataReferences oldProject project
                    cache.TryRemove(projectId) |> ignore
                    return! tryComputeOptionsUnder epoch project
                else
                    let shouldWatch = shouldWatchReferences project

                    publication.TryPublish(epoch, fun () -> reconcileReferenceWatchesNoLock projectId projectOptions shouldWatch)
                    |> ignore

                    return ValueSome struct (parsingOptions, projectOptions)
        }

    let tryComputeOptions (project: Project) =
        cancellableTask { return! tryComputeOptionsUnder publication.Current project }

    let loop (agent: MailboxProcessor<FSharpProjectOptionsMessage>) =
        async {
            while true do
                match! agent.Receive() with
                | FSharpProjectOptionsMessage.TryGetOptionsByDocument(document, reply, ct, userOpName) ->
                    if ct.IsCancellationRequested then
                        reply.Reply ValueNone
                    else
                        try
                            // For now, disallow miscellaneous workspace since we are using the hacky F# miscellaneous files project.
                            if document.Project.Solution.Workspace.Kind = WorkspaceKind.MiscellaneousFiles then
                                reply.Reply ValueNone
                            elif document.Project.IsFSharpMiscellaneousOrMetadata then
                                let! options =
                                    tryComputeOptionsBySingleScriptOrFile document userOpName
                                    |> CancellableTask.start ct
                                    |> Async.AwaitTask

                                if ct.IsCancellationRequested then
                                    reply.Reply ValueNone
                                else
                                    reply.Reply options
                            else
                                // We only care about the latest project in the workspace's solution.
                                // We do this to prevent any possible cache thrashing in FCS.
                                let project =
                                    document.Project.Solution.Workspace.CurrentSolution.GetProject(document.Project.Id)

                                if not (isNull project) then
                                    let! options = tryComputeOptions project |> CancellableTask.start ct |> Async.AwaitTask

                                    if ct.IsCancellationRequested then
                                        reply.Reply ValueNone
                                    else
                                        reply.Reply options
                                else
                                    reply.Reply ValueNone
                        with _ ->
                            reply.Reply ValueNone

                | FSharpProjectOptionsMessage.TryGetOptionsByProject(project, reply, ct) ->
                    if ct.IsCancellationRequested then
                        reply.Reply ValueNone
                    else
                        try
                            if
                                project.Solution.Workspace.Kind = WorkspaceKind.MiscellaneousFiles
                                || project.IsFSharpMiscellaneousOrMetadata
                            then
                                reply.Reply ValueNone
                            else
                                // We only care about the latest project in the workspace's solution.
                                // We do this to prevent any possible cache thrashing in FCS.
                                let project = project.Solution.Workspace.CurrentSolution.GetProject(project.Id)

                                if not (isNull project) then
                                    let! options = tryComputeOptions project |> CancellableTask.start ct |> Async.AwaitTask

                                    if ct.IsCancellationRequested then
                                        reply.Reply ValueNone
                                    else
                                        reply.Reply options
                                else
                                    reply.Reply ValueNone
                        with _ ->
                            reply.Reply ValueNone

                | FSharpProjectOptionsMessage.ClearOptions(projectId) ->
                    match cache.TryRemove(projectId) with
                    | true, struct (_, _, projectOptions) -> checker.ClearCache([ projectOptions ])
                    | _ -> ()

                    lastSuccessfulCompilations.TryRemove(projectId) |> ignore
                    peReferences.TryRemove(projectId) |> ignore
                    legacyProjectSites.TryRemove(projectId) |> ignore
                    clearReferenceWatches projectId
                | FSharpProjectOptionsMessage.ClearSingleFileOptionsCache(documentId) ->
                    match singleFileCache.TryRemove(documentId) with
                    | true, ({ ProjectOptions = projectOptions } as cacheEntry) ->
                        checker.ClearCache([ projectOptions ])
                        disposeSingleFileCacheEntry cacheEntry
                    | _ -> ()
                | FSharpProjectOptionsMessage.ClearAllSingleFileOptions -> clearSingleFileCache ()
        }

    let agent =
        MailboxProcessor.Start((fun agent -> loop agent), cancellationToken = cancellationTokenSource.Token)

    member _.TryGetOptionsByProjectAsync(project, ct) =
        agent.PostAndAsyncReply(fun reply -> FSharpProjectOptionsMessage.TryGetOptionsByProject(project, reply, ct))

    member _.TryGetOptionsByDocumentAsync(document, ct, userOpName) =
        agent.PostAndAsyncReply(fun reply -> FSharpProjectOptionsMessage.TryGetOptionsByDocument(document, reply, ct, userOpName))

    member _.ClearOptionsByProjectId(projectId) =
        agent.Post(FSharpProjectOptionsMessage.ClearOptions(projectId))

    member _.ClearSingleFileOptionsCache(documentId) =
        agent.Post(FSharpProjectOptionsMessage.ClearSingleFileOptionsCache(documentId))

    member _.SetCommandLineOptions(projectId, sourcePaths, options) =
        commandLineOptions.[projectId] <- struct (sourcePaths, options)

    member _.SetLegacyProjectSite(projectId, projectSite) =
        legacyProjectSites.[projectId] <- projectSite

    member _.TryGetCachedOptionsByProjectId(projectId) =
        match cache.TryGetValue(projectId) with
        | true, result -> Some(result)
        | _ -> None

    member _.ClearAllCaches() =
        let singleFileEntries =
            publication.Bump(fun () ->
                commandLineOptions.Clear()
                legacyProjectSites.Clear()
                cache.Clear()
                lastSuccessfulCompilations.Clear()
                peReferences.Clear()
                clearAllReferenceWatchesNoLock ()

                // Only whoever takes an entry out disposes it: a second unadvise ends the agent.
                [|
                    for documentId in singleFileCache.Keys do
                        match singleFileCache.TryRemove documentId with
                        | true, entry -> entry
                        | _ -> ()
                |])

        for entry in singleFileEntries do
            disposeSingleFileCacheEntry entry

    member _.ScriptUpdated = scriptUpdatedEvent.Publish

    member _.ReferenceStamps = referenceStamps

    interface IDisposable with
        member this.Dispose() =
            this.ClearAllCaches()
            referenceChangeTracker.Dispose()
            cancellationTokenSource.Cancel()
            cancellationTokenSource.Dispose()
            (agent :> IDisposable).Dispose()
            clearSingleFileCache ()

/// Manages mappings of Roslyn workspace Projects/Documents to FCS.
type internal FSharpProjectOptionsManager(checker: FSharpChecker, workspace: Workspace, fileChangeWatcher: IFSharpFileChangeWatcher) =

    let reactor = new FSharpProjectOptionsReactor(checker, fileChangeWatcher)

    do
        // We need to listen to this event for lifecycle purposes.
        workspace.WorkspaceChanged.Add(fun args ->
            match args.Kind with
            | WorkspaceChangeKind.ProjectRemoved -> reactor.ClearOptionsByProjectId(args.ProjectId)
            | _ -> ())

        // Options computed while a script was closed read its text from disk, with no buffer to follow the
        // caret of: opening it may keep the text's version, so the cached entry would outlive the open.
        workspace.DocumentOpened.Add(fun args ->
            let doc = args.Document
            let proj = doc.Project

            if
                proj.IsFSharp
                && proj.IsFSharpMiscellaneousOrMetadata
                && isScriptFile doc.FilePath
            then
                reactor.ClearSingleFileOptionsCache(doc.Id))

        workspace.DocumentClosed.Add(fun args ->
            let doc = args.Document
            let proj = doc.Project

            if proj.IsFSharp && proj.IsFSharpMiscellaneousOrMetadata then
                reactor.ClearSingleFileOptionsCache(doc.Id))

    member _.ScriptUpdated = reactor.ScriptUpdated

    member _.SetLegacyProjectSite(projectId, projectSite) =
        reactor.SetLegacyProjectSite(projectId, projectSite)

    /// Clear a project from the project table
    member _.ClearInfoForProject(projectId: ProjectId) =
        reactor.ClearOptionsByProjectId(projectId)

    member _.ClearSingleFileOptionsCache(documentId) =
        reactor.ClearSingleFileOptionsCache(documentId)

    /// Get compilation defines and language version relevant for syntax processing.
    /// Quicker then TryGetOptionsForDocumentOrProject as it doesn't need to recompute the exact project
    /// options for a script.
    member _.GetCompilationDefinesAndLangVersionForEditingDocument(document: Document) =
        let parsingOptions =
            match reactor.TryGetCachedOptionsByProjectId(document.Project.Id) with
            | Some(_, parsingOptions, _) -> parsingOptions
            | _ ->
                { FSharpParsingOptions.Default with
                    ApplyLineDirectives = false
                    IsInteractive = CompilerEnvironment.IsScriptFile document.Name
                }

        CompilerEnvironment.GetConditionalDefinesForEditing parsingOptions, parsingOptions.LangVersionText

    member _.TryGetCachedOptionsByProjectId(projectId) =
        reactor.TryGetCachedOptionsByProjectId(projectId)

    member _.TryGetOptionsByProject(project) =
        reactor.TryGetOptionsByProjectAsync(project)

    /// Get the exact options for a document or project
    member _.TryGetOptionsForDocumentOrProject(document: Document, cancellationToken, userOpName) =
        reactor.TryGetOptionsByDocumentAsync(document, cancellationToken, userOpName)

    /// Get the exact options for a document or project relevant for syntax processing.
    member this.TryGetOptionsForEditingDocumentOrProject(document: Document, cancellationToken, userOpName) =
        this.TryGetOptionsForDocumentOrProject(document, cancellationToken, userOpName)

    /// Get the options for a document or project relevant for syntax processing.
    /// Quicker it doesn't need to recompute the exact project options for a script.
    member this.TryGetQuickParsingOptionsForEditingDocumentOrProject(documentId: DocumentId, path: string) =
        match reactor.TryGetCachedOptionsByProjectId(documentId.ProjectId) with
        | Some(_, parsingOptions, _) -> parsingOptions
        | _ ->
            // ParseFile takes the last entry of SourceFiles as the last compiland; with none it throws.
            { FSharpParsingOptions.Default with
                SourceFiles = [| path |]
                IsInteractive = CompilerEnvironment.IsScriptFile path
            }

    member _.SetCommandLineOptions(projectId, sourcePaths, options: ImmutableArray<string>) =
        reactor.SetCommandLineOptions(projectId, sourcePaths, options.ToArray())

    member _.ClearAllCaches() = reactor.ClearAllCaches()

    member _.ReferenceStamps = reactor.ReferenceStamps

    member _.Checker = checker
