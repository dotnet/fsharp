[<AutoOpen>]
module internal Microsoft.VisualStudio.FSharp.Editor.WorkspaceExtensions

open System
open System.Runtime.CompilerServices
open System.Threading
open Microsoft.CodeAnalysis
open FSharp.Compiler
open FSharp.Compiler.CodeAnalysis
open Microsoft.VisualStudio.LanguageServices
open FSharp.NativeInterop

#nowarn "9" // NativePtr.toNativeInt

[<RequireQualifiedAccess>]
module private ProjectCache =

    /// This is a cache to maintain FSharpParsingOptions and FSharpProjectOptions per Roslyn Project.
    /// The Roslyn Project is held weakly meaning when it is cleaned up by the GC, the FSharpProject will be cleaned up by the GC.
    let Projects = ConditionalWeakTable<Project, FSharpProjectOptionsManager * FSharpProject>()    

type Solution with

    /// Get the instance of IFSharpWorkspaceService.
    member private this.GetFSharpWorkspaceService() =
        this.Workspace.Services.GetRequiredService<IFSharpWorkspaceService>()

let private legacyReferenceResolver = LegacyMSBuildReferenceResolver.getResolver()

type Project with

    member this.GetOrCreateFSharpProjectAsync(parsingOptions, projectOptions) =
        async {
            if not this.IsFSharp then
                raise(System.OperationCanceledException("Roslyn Project is not FSharp."))

            match ProjectCache.Projects.TryGetValue(this) with
            | true, result -> return result
            | _ ->

            let tryGetMetadataSnapshot (path, timeStamp) = 
                match this.Solution.Workspace with
                | :? VisualStudioWorkspace as workspace ->
                    try
                        let md = Microsoft.CodeAnalysis.ExternalAccess.FSharp.LanguageServices.FSharpVisualStudioWorkspaceExtensions.GetMetadata(workspace, path, timeStamp)
                        let amd = (md :?> AssemblyMetadata)
                        let mmd = amd.GetModules().[0]
                        let mmr = mmd.GetMetadataReader()

                        // "lifetime is timed to Metadata you got from the GetMetadata(...). As long as you hold it strongly, raw 
                        // memory we got from metadata reader will be alive. Once you are done, just let everything go and 
                        // let finalizer handle resource rather than calling Dispose from Metadata directly. It is shared metadata. 
                        // You shouldn't dispose it directly."

                        let objToHold = box md

                        // We don't expect any ilread WeakByteFile to be created when working in Visual Studio
                        // Debug.Assert((FSharp.Compiler.AbstractIL.ILBinaryReader.GetStatistics().weakByteFileCount = 0), "Expected weakByteFileCount to be zero when using F# in Visual Studio. Was there a problem reading a .NET binary?")

                        Some (objToHold, NativePtr.toNativeInt mmr.MetadataPointer, mmr.MetadataLength)
                    with ex -> 
                        // We catch all and let the backup routines in the F# compiler find the error
                        Assert.Exception(ex)
                        None 
                | _ ->
                    None

            let! result =
                FSharpProject.CreateAsync(parsingOptions, projectOptions, 
                    legacyReferenceResolver = legacyReferenceResolver,
                    tryGetMetadataSnapshot = tryGetMetadataSnapshot)

            match result with
            | Ok fsProject ->
                let service = this.Solution.GetFSharpWorkspaceService()
                let projectOptionsManager = service.FSharpProjectOptionsManager
                let result = (projectOptionsManager, fsProject)
                return ProjectCache.Projects.GetValue(this, Runtime.CompilerServices.ConditionalWeakTable<_,_>.CreateValueCallback(fun _ -> result))
            | Error(diags) ->
                return raise(System.OperationCanceledException($"Unable to create a FSharpProject:\n%A{ diags }"))
        }

type Document with

    /// Get the FSharpParsingOptions and FSharpProjectOptions from the F# project that is associated with the given F# document.
    member this.GetFSharpProjectAsync(userOpName) =
        async {
            if this.Project.IsFSharp then
                match ProjectCache.Projects.TryGetValue(this.Project) with
                | true, result -> return result
                | _ ->
                    let service = this.Project.Solution.GetFSharpWorkspaceService()
                    let projectOptionsManager = service.FSharpProjectOptionsManager
                    let! ct = Async.CancellationToken
                    match! projectOptionsManager.TryGetOptionsForDocumentOrProject(this, ct, userOpName) with
                    | None -> return raise(System.OperationCanceledException("FSharp project options not found."))
                    | Some(parsingOptions, _, projectOptions) ->
                        return! this.Project.GetOrCreateFSharpProjectAsync(parsingOptions, projectOptions)
            else
                return raise(System.OperationCanceledException("Roslyn Document is not FSharp."))
        }

    /// Get the compilation defines from F# project that is associated with the given F# document.
    member this.GetFSharpCompilationDefinesAsync(userOpName) =
        async {
            let! _, fsProject = this.GetFSharpProjectAsync(userOpName)
            return CompilerEnvironment.GetCompilationDefinesForEditing fsProject.ParsingOptions
        }

    /// Get the instance of the FSharpChecker from the workspace by the given F# document.
    member this.GetFSharpChecker() =
        let workspaceService = this.Project.Solution.GetFSharpWorkspaceService()
        workspaceService.Checker

    /// A non-async call that quickly gets FSharpParsingOptions of the given F# document.
    /// This tries to get the FSharpParsingOptions by looking at an internal cache; if it doesn't exist in the cache it will create an inaccurate but usable form of the FSharpParsingOptions.
    member this.GetFSharpQuickParsingOptions() =
        let workspaceService = this.Project.Solution.GetFSharpWorkspaceService()
        workspaceService.FSharpProjectOptionsManager.TryGetQuickParsingOptionsForEditingDocumentOrProject(this)

    /// A non-async call that quickly gets the defines of the given F# document.
    /// This tries to get the defines by looking at an internal cache; if it doesn't exist in the cache it will create an inaccurate but usable form of the defines.
    member this.GetFSharpQuickDefines() =
        let workspaceService = this.Project.Solution.GetFSharpWorkspaceService()
        workspaceService.FSharpProjectOptionsManager.GetCompilationDefinesForEditingDocument(this)
    
    /// Parses the given F# document.
    member this.GetFSharpParseResultsAsync(userOpName) =
        async {
            let! _, fsProject = this.GetFSharpProjectAsync(userOpName)
            return fsProject.GetParseFileResults(this.FilePath)
        }

    /// Parses and checks the given F# document.
    member this.GetFSharpParseAndCheckResultsAsync(userOpName) =
        async {
            let! _, fsProject = this.GetFSharpProjectAsync(userOpName)
            return! fsProject.GetParseAndCheckFileResultsAsync(this.FilePath)
        }

    /// Get the semantic classifications of the given F# document.
    member this.GetFSharpSemanticClassificationAsync(userOpName) =
        async {
            let! _, fsProject = this.GetFSharpProjectAsync(userOpName)
            match! fsProject.TryGetSemanticClassificationForFileAsync(this.FilePath) with
            | Some results -> return results
            | _ -> return raise(System.OperationCanceledException("Unable to get FSharp semantic classification."))
        }

    /// Find F# references in the given F# document.
    member this.FindFSharpReferencesAsync(symbol, onFound, userOpName) =
        async {
            let! _, fsProject = this.GetFSharpProjectAsync(userOpName)
            let! symbolUses = fsProject.FindReferencesInFileAsync(this.FilePath, symbol)
            let! ct = Async.CancellationToken
            let! sourceText = this.GetTextAsync ct |> Async.AwaitTask
            for symbolUse in symbolUses do 
                match RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, symbolUse) with
                | Some textSpan ->
                    do! onFound textSpan symbolUse
                | _ ->
                    ()
        }

    /// Try to find a F# lexer/token symbol of the given F# document and position.
    member this.TryFindFSharpLexerSymbolAsync(position, lookupKind, wholeActivePattern, allowStringToken, userOpName) =
        async {
            let! defines = this.GetFSharpCompilationDefinesAsync(userOpName)
            let! ct = Async.CancellationToken
            let! sourceText = this.GetTextAsync(ct) |> Async.AwaitTask
            return Tokenizer.getSymbolAtPosition(this.Id, sourceText, position, this.FilePath, defines, lookupKind, wholeActivePattern, allowStringToken)
        }

    /// This is only used for testing purposes. It sets the ProjectCache.Projects with the given FSharpProjectOptions and F# document's project.
    member this.SetFSharpProjectOptionsForTesting(projectOptions: FSharpProjectOptions) =
        let workspaceService = this.Project.Solution.GetFSharpWorkspaceService()
        let parsingOptions, _, _ = 
            workspaceService.FSharpProjectOptionsManager.TryGetOptionsForDocumentOrProject(this, CancellationToken.None, nameof(this.SetFSharpProjectOptionsForTesting))
            |> Async.RunSynchronously
            |> Option.get
        let result = FSharpProject.CreateAsync(parsingOptions, projectOptions) |> Async.RunSynchronously
        match result with
        | Ok fsProject ->
            ProjectCache.Projects.Add(this.Project, (workspaceService.FSharpProjectOptionsManager, fsProject))
        | Error diags ->
            failwithf $"Unable to create a FSharpProject for testing: %A{ diags }"

type Project with

    /// Find F# references in the given project.
    member this.FindFSharpReferencesAsync(symbol, onFound, userOpName) =
        async {
            for doc in this.Documents do
                do! doc.FindFSharpReferencesAsync(symbol, (fun textSpan range -> onFound doc textSpan range), userOpName)
        }
