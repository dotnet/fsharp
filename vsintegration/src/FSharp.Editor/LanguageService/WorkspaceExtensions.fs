[<AutoOpen>]
module internal Microsoft.VisualStudio.FSharp.Editor.WorkspaceExtensions

open System
open System.Runtime.CompilerServices

open Microsoft.CodeAnalysis
open Microsoft.VisualStudio.FSharp.Editor

open FSharp.Compiler
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.CodeAnalysis.ProjectSnapshot
open FSharp.Compiler.Symbols
open FSharp.Compiler.BuildGraph

open CancellableTasks

open Internal.Utilities.Collections
open Newtonsoft.Json
open Newtonsoft.Json.Linq
open System.Text.Json.Nodes

#nowarn "57" // Experimental stuff

[<RequireQualifiedAccess>]
module internal ProjectCache =

    /// This is a cache to maintain FSharpParsingOptions and FSharpProjectOptions per Roslyn Project.
    /// The Roslyn Project is held weakly meaning when it is cleaned up by the GC, the FSharParsingOptions and FSharpProjectOptions will be cleaned up by the GC.
    /// At some point, this will be the main caching mechanism for FCS projects instead of FCS itself.
    let Projects =
        ConditionalWeakTable<Project, FSharpChecker * FSharpProjectOptionsManager * FSharpParsingOptions * FSharpProjectOptions>()

type Solution with

    /// Get the instance of IFSharpWorkspaceService.
    member internal this.GetFSharpWorkspaceService() =
        this.Workspace.Services.GetRequiredService<IFSharpWorkspaceService>()

module internal FSharpProjectSnapshotSerialization =

    let serializeFileSnapshot (snapshot: FSharpFileSnapshot) =
        let output = JObject()
        output.Add("FileName", snapshot.FileName)
        output.Add("Version", snapshot.Version)
        output

    let serializeReferenceOnDisk (reference: ReferenceOnDisk) =
        let output = JObject()
        output.Add("Path", reference.Path)
        output.Add("LastModified", reference.LastModified)
        output

    let rec serializeReferencedProject (reference: FSharpReferencedProjectSnapshot) =
        let output = JObject()

        match reference with
        | FSharpReference(projectOutputFile, snapshot) ->
            output.Add("projectOutputFile", projectOutputFile)
            output.Add("snapshot", serializeSnapshot snapshot)
        | _ -> ()

        output

    and serializeSnapshot (snapshot: FSharpProjectSnapshot) =

        let output = JObject()
        let snapshot = snapshot.ProjectSnapshot

        output.Add("ProjectFileName", snapshot.ProjectFileName)
        output.Add("ProjectId", (snapshot.ProjectId |> Option.defaultValue null |> JToken.FromObject))
        output.Add("SourceFiles", snapshot.SourceFiles |> Seq.map serializeFileSnapshot |> JArray)
        output.Add("ReferencesOnDisk", snapshot.ReferencesOnDisk |> Seq.map serializeReferenceOnDisk |> JArray)
        output.Add("OtherOptions", JArray(snapshot.OtherOptions))
        output.Add("ReferencedProjects", snapshot.ReferencedProjects |> Seq.map serializeReferencedProject |> JArray)
        output.Add("IsIncompleteTypeCheckEnvironment", snapshot.IsIncompleteTypeCheckEnvironment)
        output.Add("UseScriptResolutionRules", snapshot.UseScriptResolutionRules)
        output.Add("LoadTime", snapshot.LoadTime)
        // output.Add("UnresolvedReferences", snapshot.UnresolvedReferences)
        output.Add(
            "OriginalLoadReferences",
            snapshot.OriginalLoadReferences
            |> Seq.map (fun (r: Text.range, a, b) -> JArray(r.FileName, r.Start, r.End, a, b))
            |> JArray
        )

        output.Add("Stamp", (snapshot.Stamp |> (Option.defaultValue 0) |> JToken.FromObject))

        output

    let dumpToJson (snapshot) =

        let jObject = serializeSnapshot snapshot

        let json = jObject.ToString(Formatting.Indented)

        json

open FSharpProjectSnapshotSerialization
open System.Collections.Concurrent

[<AutoOpen>]
module private CheckerExtensions =

    let snapshotCache = AsyncMemoize(1000, 500, "SnapshotCache")

    let latestSnapshots = ConcurrentDictionary<_, _>()

    let exist xs = xs |> Seq.isEmpty |> not

    let getFSharpOptionsForProject (this: Project) =
        if not this.IsFSharp then
            raise (OperationCanceledException("Project is not a FSharp project."))
        else
            match ProjectCache.Projects.TryGetValue(this) with
            | true, result -> CancellableTask.singleton result
            | _ ->
                cancellableTask {

                    let! ct = CancellableTask.getCancellationToken ()

                    let service = this.Solution.GetFSharpWorkspaceService()
                    let projectOptionsManager = service.FSharpProjectOptionsManager

                    match! projectOptionsManager.TryGetOptionsByProject(this, ct) with
                    | ValueNone -> return raise (OperationCanceledException("FSharp project options not found."))
                    | ValueSome(parsingOptions, projectOptions) ->
                        let result =
                            (service.Checker, projectOptionsManager, parsingOptions, projectOptions)

                        return ProjectCache.Projects.GetValue(this, ConditionalWeakTable<_, _>.CreateValueCallback(fun _ -> result))
                }

    let documentToSnapshot (document: Document) =
        cancellableTask {
            let! version = document.GetTextVersionAsync()

            let getSource () =
                task {
                    let! sourceText = document.GetTextAsync()
                    return sourceText.ToFSharpSourceText()
                }

            return FSharpFileSnapshot(FileName = document.FilePath, Version = version.ToString(), GetSource = getSource)
        }

    let getReferencedProjectVersions (project: Project) =
        project.GetAllProjectsThisProjectDependsOn()
        |> Seq.map (fun r ct -> r.GetDependentSemanticVersionAsync(ct))
        |> CancellableTask.whenAll
        |> CancellableTask.map (Seq.map (fun x -> x.ToString()) >> Set)

    let getOnDiskReferences (options: FSharpProjectOptions) =
        options.OtherOptions
        |> Seq.filter (fun x -> x.StartsWith("-r:"))
        |> Seq.map (fun x ->
            let path = x.Substring(3)

            {
                Path = path
                LastModified = System.IO.File.GetLastWriteTimeUtc path
            })
        |> Seq.toList

    let createProjectSnapshot (snapshotAccumulatorOpt) (project: Project) (options: FSharpProjectOptions option) =
        cancellableTask {

            let! options =
                match options with
                | Some options -> CancellableTask.singleton options
                | None ->
                    cancellableTask {
                        let! _, _, _, options = getFSharpOptionsForProject project
                        return options
                    }

            let! projectVersion = project.GetDependentSemanticVersionAsync()

            let! referenceVersions = getReferencedProjectVersions project

            let updatedSnapshot =
                match project.IsTransparentCompilerSnapshotReuseEnabled, latestSnapshots.TryGetValue project.Id with
                | true, (true, (_, _, oldReferenceVersions, _, _)) when referenceVersions <> oldReferenceVersions ->
                    System.Diagnostics.Trace.TraceWarning "Reference versions changed"
                    None

                | true, (true, (_, _, _, _, oldSnapshot: FSharpProjectSnapshot)) when
                    oldSnapshot.ProjectSnapshot.ReferencesOnDisk <> (getOnDiskReferences options)
                    ->
                    System.Diagnostics.Trace.TraceWarning "References on disk changed"
                    None

                | true, (true, (_, oldProjectVersion, _, _, oldSnapshot: FSharpProjectSnapshot)) when projectVersion = oldProjectVersion ->
                    Some(CancellableTask.singleton oldSnapshot)

                | true, (true, (oldProject, _oldProjectVersion, _oldReferencesVersion, oldOptions, oldSnapshot: FSharpProjectSnapshot)) when
                    FSharpProjectOptions.AreSameForChecking(options, oldOptions)
                    ->

                    let changes = project.GetChanges(oldProject)

                    if
                        changes.GetAddedDocuments() |> exist
                        || changes.GetRemovedDocuments() |> exist
                        || changes.GetAddedMetadataReferences() |> exist
                        || changes.GetRemovedMetadataReferences() |> exist
                        || changes.GetAddedProjectReferences() |> exist
                        || changes.GetRemovedProjectReferences() |> exist
                    then
                        // if any of that happened, we create it from scratch
                        System.Diagnostics.Trace.TraceWarning "Project change not covered by options - suspicious"
                        None

                    else
                        // we build it from the previous one

                        let changedDocuments = changes.GetChangedDocuments() |> Seq.toList

                        System.Diagnostics.Trace.TraceInformation
                            $"Incremental update of FSharpProjectSnapshot ({oldSnapshot.Label}) - {changedDocuments.Length} changed documents"

                        if changedDocuments.Length = 0 then
                            // this is suspicious
                            let _breakpoint = "here"
                            ()

                        changedDocuments
                        |> Seq.map (project.GetDocument >> documentToSnapshot)
                        |> CancellableTask.whenAll
                        |> CancellableTask.map (Array.toList >> oldSnapshot.Replace)
                        |> Some

                | _ -> None

            let! newSnapshot =

                match updatedSnapshot with
                | Some snapshot -> snapshot
                | _ ->
                    cancellableTask {

                        let solution = project.Solution

                        let projects =
                            solution.Projects
                            |> Seq.map (fun p -> p.FilePath, p.Documents |> Seq.map (fun d -> d.FilePath, d) |> Map)
                            |> Map

                        let getFileSnapshot (options: FSharpProjectOptions) path =
                            async {
                                let project = projects.TryFind options.ProjectFileName

                                if project.IsNone then
                                    System.Diagnostics.Trace.TraceError(
                                        "Could not find project {0} in solution {1}",
                                        options.ProjectFileName,
                                        solution.FilePath
                                    )

                                let documentOpt = project |> Option.bind (Map.tryFind path)

                                let! version, getSource =
                                    match documentOpt with
                                    | Some document ->
                                        async {

                                            let! version = document.GetTextVersionAsync() |> Async.AwaitTask

                                            let getSource () =
                                                task {
                                                    let! sourceText = document.GetTextAsync()
                                                    return sourceText.ToFSharpSourceText()
                                                }

                                            return version.ToString(), getSource

                                        }
                                    | None ->
                                        // This happens with files that are read from /obj

                                        // Fall back to file system
                                        let version = System.IO.File.GetLastWriteTimeUtc(path)

                                        let getSource () =
                                            task { return System.IO.File.ReadAllText(path) |> FSharp.Compiler.Text.SourceTextNew.ofString }

                                        async.Return(version.ToString(), getSource)

                                return FSharpFileSnapshot(FileName = path, Version = version, GetSource = getSource)
                            }

                        let! snapshot =
                            FSharpProjectSnapshot.FromOptions(options, getFileSnapshot, ?snapshotAccumulator = snapshotAccumulatorOpt)

                        System.Diagnostics.Trace.TraceInformation $"Created new FSharpProjectSnapshot ({snapshot.Label})"

                        return snapshot
                    }

            let latestSnapshotData =
                project, projectVersion, referenceVersions, options, newSnapshot

            latestSnapshots.AddOrUpdate(project.Id, latestSnapshotData, (fun _ _ -> latestSnapshotData))
            |> ignore

            return newSnapshot
        }

    let getOrCreateSnapshotForProject (project: Project) options snapshotAccumulatorOpt =

        let key =
            { new ICacheKey<_, _> with
                member _.GetKey() = project.Id
                member _.GetVersion() = project
                member _.GetLabel() = project.FilePath
            }

        snapshotCache.Get(
            key,
            async {
                let! ct = Async.CancellationToken

                return!
                    createProjectSnapshot snapshotAccumulatorOpt project options ct
                    |> Async.AwaitTask
            }
        )

    let getProjectSnapshotForDocument (document: Document, options: FSharpProjectOptions) =
        getOrCreateSnapshotForProject document.Project (Some options) None

    type FSharpChecker with

        /// Parse the source text from the Roslyn document.
        member checker.ParseDocument(document: Document, parsingOptions: FSharpParsingOptions, userOpName: string) =
            cancellableTask {
                let! ct = CancellableTask.getCancellationToken ()
                let! sourceText = document.GetTextAsync(ct)

                return! checker.ParseFile(document.FilePath, sourceText.ToFSharpSourceText(), parsingOptions, userOpName = userOpName)
            }

        member checker.ParseDocumentUsingTransparentCompiler(document: Document, options: FSharpProjectOptions, userOpName: string) =
            cancellableTask {
                let! projectSnapshot = getProjectSnapshotForDocument (document, options)
                return! checker.ParseFile(document.FilePath, projectSnapshot, userOpName = userOpName)
            }

        member checker.ParseAndCheckDocumentUsingTransparentCompiler
            (
                document: Document,
                options: FSharpProjectOptions,
                userOpName: string
            ) =
            cancellableTask {

                checker.TransparentCompiler.SetCacheSizeFactor(document.Project.TransparentCompilerCacheFactor)

                let! projectSnapshot = getProjectSnapshotForDocument (document, options)

                let! (parseResults, checkFileAnswer) = checker.ParseAndCheckFileInProject(document.FilePath, projectSnapshot, userOpName)

                return
                    match checkFileAnswer with
                    | FSharpCheckFileAnswer.Aborted -> None
                    | FSharpCheckFileAnswer.Succeeded(checkFileResults) -> Some(parseResults, checkFileResults)
            }

        /// Parse and check the source text from the Roslyn document with possible stale results.
        member checker.ParseAndCheckDocumentWithPossibleStaleResults
            (
                document: Document,
                options: FSharpProjectOptions,
                allowStaleResults: bool,
                userOpName: string
            ) =
            cancellableTask {
                let! ct = CancellableTask.getCancellationToken ()

                let! sourceText = document.GetTextAsync(ct)
                let! textVersion = document.GetTextVersionAsync(ct)

                let filePath = document.FilePath
                let textVersionHash = textVersion.GetHashCode()

                let parseAndCheckFile =
                    cancellableTask {
                        let! (parseResults, checkFileAnswer) =
                            checker.ParseAndCheckFileInProject(
                                filePath,
                                textVersionHash,
                                sourceText.ToFSharpSourceText(),
                                options,
                                userOpName = userOpName
                            )

                        return
                            match checkFileAnswer with
                            | FSharpCheckFileAnswer.Aborted -> None
                            | FSharpCheckFileAnswer.Succeeded(checkFileResults) -> Some(parseResults, checkFileResults)
                    }

                let tryGetFreshResultsWithTimeout () =
                    cancellableTask {
                        let! worker =
                            Async.StartChild(
                                async {
                                    try
                                        return! parseAndCheckFile
                                    with _ ->
                                        return None
                                },
                                millisecondsTimeout = document.Project.FSharpTimeUntilStaleCompletion
                            )

                        try
                            return! worker
                        with :? TimeoutException ->
                            return None // worker is cancelled at this point, we cannot return it and wait its completion anymore
                    }

                if allowStaleResults then
                    let! freshResults = tryGetFreshResultsWithTimeout ()

                    let! results =
                        match freshResults with
                        | Some x -> CancellableTask.singleton (Some x)
                        | None ->
                            cancellableTask {
                                match checker.TryGetRecentCheckResultsForFile(filePath, options, userOpName = userOpName) with
                                | Some(parseResults, checkFileResults, _) -> return Some(parseResults, checkFileResults)
                                | None -> return! parseAndCheckFile
                            }

                    return results
                else
                    return! parseAndCheckFile
            }

        /// Parse and check the source text from the Roslyn document.
        member checker.ParseAndCheckDocument
            (
                document: Document,
                options: FSharpProjectOptions,
                userOpName: string,
                ?allowStaleResults: bool
            ) =
            cancellableTask {

                if checker.UsesTransparentCompiler then
                    return! checker.ParseAndCheckDocumentUsingTransparentCompiler(document, options, userOpName)
                else
                    let allowStaleResults =
                        match allowStaleResults with
                        | Some b -> b
                        | _ -> document.Project.IsFSharpStaleCompletionResultsEnabled

                    return!
                        checker.ParseAndCheckDocumentWithPossibleStaleResults(document, options, allowStaleResults, userOpName = userOpName)
            }

type Document with

    /// Get the FSharpParsingOptions and FSharpProjectOptions from the F# project that is associated with the given F# document.
    member this.GetFSharpCompilationOptionsAsync(userOpName) =
        if not this.Project.IsFSharp then
            raise (OperationCanceledException("Document is not a FSharp document."))
        else
            match ProjectCache.Projects.TryGetValue(this.Project) with
            | true, result -> CancellableTask.singleton result
            | _ ->
                cancellableTask {
                    let service = this.Project.Solution.GetFSharpWorkspaceService()
                    let projectOptionsManager = service.FSharpProjectOptionsManager
                    let! ct = CancellableTask.getCancellationToken ()

                    match! projectOptionsManager.TryGetOptionsForDocumentOrProject(this, ct, userOpName) with
                    | ValueNone -> return raise (OperationCanceledException("FSharp project options not found."))
                    | ValueSome(parsingOptions, projectOptions) ->
                        let result =
                            (service.Checker, projectOptionsManager, parsingOptions, projectOptions)

                        return ProjectCache.Projects.GetValue(this.Project, ConditionalWeakTable<_, _>.CreateValueCallback(fun _ -> result))
                }

    /// Get the compilation defines and language version from F# project that is associated with the given F# document.
    member this.GetFsharpParsingOptionsAsync(userOpName) =
        async {
            let! _, _, parsingOptions, _ = this.GetFSharpCompilationOptionsAsync(userOpName)

            return
                CompilerEnvironment.GetConditionalDefinesForEditing parsingOptions,
                parsingOptions.LangVersionText,
                parsingOptions.StrictIndentation
        }

    /// Get the instance of the FSharpChecker from the workspace by the given F# document.
    member this.GetFSharpChecker() =
        let workspaceService = this.Project.Solution.GetFSharpWorkspaceService()
        workspaceService.Checker

    /// Get the instance of the FSharpMetadataAsSourceService from the workspace by the given F# document.
    member this.GetFSharpMetadataAsSource() =
        let workspaceService = this.Project.Solution.GetFSharpWorkspaceService()
        workspaceService.MetadataAsSource

    /// A non-async call that quickly gets FSharpParsingOptions of the given F# document.
    /// This tries to get the FSharpParsingOptions by looking at an internal cache; if it doesn't exist in the cache it will create an inaccurate but usable form of the FSharpParsingOptions.
    member this.GetFSharpQuickParsingOptions() =
        let workspaceService = this.Project.Solution.GetFSharpWorkspaceService()
        workspaceService.FSharpProjectOptionsManager.TryGetQuickParsingOptionsForEditingDocumentOrProject(this.Id, this.FilePath)

    /// A non-async call that quickly gets the defines and F# language version of the given F# document.
    /// This tries to get the data by looking at an internal cache; if it doesn't exist in the cache it will create an inaccurate but usable form of the defines and the language version.
    member this.GetFsharpParsingOptions() =
        let workspaceService = this.Project.Solution.GetFSharpWorkspaceService()
        workspaceService.FSharpProjectOptionsManager.GetCompilationDefinesAndLangVersionForEditingDocument(this)

    /// A non-async call that quickly gets the defines of the given F# document.
    /// This tries to get the defines by looking at an internal cache; if it doesn't exist in the cache it will create an inaccurate but usable form of the defines.
    member this.GetFSharpQuickDefines() =
        match this.GetFsharpParsingOptions() with
        | defines, _, _ -> defines

    /// Parses the given F# document.
    member this.GetFSharpParseResultsAsync(userOpName) =
        cancellableTask {
            let! checker, _, parsingOptions, options = this.GetFSharpCompilationOptionsAsync(userOpName)

            if this.Project.UseTransparentCompiler then
                return! checker.ParseDocumentUsingTransparentCompiler(this, options, userOpName)
            else
                return! checker.ParseDocument(this, parsingOptions, userOpName)
        }

    /// Parses and checks the given F# document.
    member this.GetFSharpParseAndCheckResultsAsync(userOpName) =
        cancellableTask {
            let! checker, _, _, projectOptions = this.GetFSharpCompilationOptionsAsync(userOpName)

            match! checker.ParseAndCheckDocument(this, projectOptions, userOpName, allowStaleResults = false) with
            | Some results -> return results
            | _ -> return raise (OperationCanceledException("Unable to get FSharp parse and check results."))
        }

    /// Get the semantic classifications of the given F# document.
    member this.GetFSharpSemanticClassificationAsync(userOpName) =
        cancellableTask {
            let! checker, _, _, projectOptions = this.GetFSharpCompilationOptionsAsync(userOpName)

            let! result =
                if this.Project.UseTransparentCompiler then
                    async {
                        let! projectSnapshot = getProjectSnapshotForDocument (this, projectOptions)
                        return! checker.GetBackgroundSemanticClassificationForFile(this.FilePath, projectSnapshot)
                    }
                else
                    checker.GetBackgroundSemanticClassificationForFile(this.FilePath, projectOptions)

            return
                result
                |> Option.defaultWith (fun _ -> raise (OperationCanceledException("Unable to get FSharp semantic classification.")))
        }

    /// Find F# references in the given F# document.
    member inline this.FindFSharpReferencesAsync(symbol, projectSnapshot: FSharpProjectSnapshot, [<InlineIfLambda>] onFound, userOpName) =
        cancellableTask {
            let! checker, _, _, projectOptions = this.GetFSharpCompilationOptionsAsync(userOpName)

            let! symbolUses =

                if this.Project.UseTransparentCompiler then
                    checker.FindBackgroundReferencesInFile(this.FilePath, projectSnapshot, symbol)
                else
                    checker.FindBackgroundReferencesInFile(
                        this.FilePath,
                        projectOptions,
                        symbol,
                        canInvalidateProject = false,
                        fastCheck = this.Project.IsFastFindReferencesEnabled
                    )

            do!
                symbolUses
                |> Seq.map onFound
                |> CancellableTask.whenAll
                |> CancellableTask.ignore
        }

    /// Try to find a F# lexer/token symbol of the given F# document and position.
    member this.TryFindFSharpLexerSymbolAsync(position, lookupKind, wholeActivePattern, allowStringToken, userOpName) =
        cancellableTask {
            let! defines, langVersion, strictIndentation = this.GetFsharpParsingOptionsAsync(userOpName)
            let! ct = CancellableTask.getCancellationToken ()
            let! sourceText = this.GetTextAsync(ct)

            return
                Tokenizer.getSymbolAtPosition (
                    this.Id,
                    sourceText,
                    position,
                    this.FilePath,
                    defines,
                    lookupKind,
                    wholeActivePattern,
                    allowStringToken,
                    Some langVersion,
                    strictIndentation,
                    ct
                )
        }

type Project with

    /// Find F# references in the given project.
    member this.FindFSharpReferencesAsync(symbol: FSharpSymbol, projectSnapshot, onFound, userOpName) =
        cancellableTask {

            let declarationLocation =
                symbol.SignatureLocation
                |> Option.map Some
                |> Option.defaultValue symbol.DeclarationLocation

            let declarationDocument =
                declarationLocation |> Option.bind this.Solution.TryGetDocumentFromFSharpRange

            // Can we skip documents, which are above current, since they can't contain symbols from current one.
            let! canSkipDocuments =
                match declarationDocument with
                | Some document when this.IsFastFindReferencesEnabled && document.Project = this ->
                    cancellableTask {
                        let! _, _, _, options = document.GetFSharpCompilationOptionsAsync(userOpName)

                        let signatureFile =
                            if not (document.FilePath |> isSignatureFile) then
                                document.FilePath + "i"
                            else
                                null

                        return

                            options.SourceFiles
                            |> Seq.takeWhile ((<>) document.FilePath)
                            |> Seq.filter ((<>) signatureFile)
                            |> Set
                    }
                | _ -> CancellableTask.singleton Set.empty

            let documents =
                this.Documents
                |> Seq.filter (fun document -> not (canSkipDocuments.Contains document.FilePath))

            if this.IsFastFindReferencesEnabled then
                do!
                    documents
                    |> Seq.map (fun doc ->
                        doc.FindFSharpReferencesAsync(symbol, projectSnapshot, (fun range -> onFound doc range), userOpName))
                    |> CancellableTask.whenAll
            else
                for doc in documents do
                    do! doc.FindFSharpReferencesAsync(symbol, projectSnapshot, (onFound doc), userOpName)
        }

    member this.GetFSharpCompilationOptionsAsync() = this |> getFSharpOptionsForProject

    member this.GetFSharpProjectSnapshot(?snapshotAccumulator) =
        cancellableTask { return! getOrCreateSnapshotForProject this None snapshotAccumulator }
