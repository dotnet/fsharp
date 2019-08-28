[<AutoOpen>] 
module FSharp.Compiler.Compilation.FSharpCompilation

open System
open System.IO
open System.Threading
open System.Diagnostics
open System.Collections.Immutable
open System.Collections.Generic
open FSharp.NativeInterop

open FSharp.Compiler
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.CompileOps
open FSharp.Compiler.Driver
open FSharp.Compiler.Compilation.Utilities
open FSharp.Compiler.Compilation.IncrementalChecker
open FSharp.Compiler.AbstractIL
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILBinaryReader
open FSharp.Compiler.ErrorLogger
open Internal.Utilities

open Microsoft.CodeAnalysis

#nowarn "9" // NativePtr.toNativeInt

let legacyReferenceResolver = LegacyMSBuildReferenceResolver.getResolver ()

// TcState is arbitrary, doesn't matter as long as the type is inside FSharp.Compiler.Private, also this is yuck.
let defaultFSharpBinariesDir = FSharpEnvironment.BinFolderOfDefaultFSharpCompiler(Some(typeof<FSharp.Compiler.CompileOps.TcState>.Assembly.Location)).Value

let getRawPointerMetadataSnapshot (md: Microsoft.CodeAnalysis.Metadata, peDataOpt) =
    let amd = (md :?> Microsoft.CodeAnalysis.AssemblyMetadata)
    let mmd = amd.GetModules().[0]
    let mmr = mmd.GetMetadataReader()

    // "lifetime is timed to Metadata you got from the GetMetadata(...). As long as you hold it strongly, raw 
    // memory we got from metadata reader will be alive. Once you are done, just let everything go and 
    // let finalizer handle resource rather than calling Dispose from Metadata directly. It is shared metadata. 
    // You shouldn't dispose it directly."

    let objToHold = box md
    // We don't expect any ilread WeakByteFile to be created when working in Visual Studio
    Debug.Assert((FSharp.Compiler.AbstractIL.ILBinaryReader.GetStatistics().weakByteFileCount = 0), "Expected weakByteFileCount to be zero when using F# in Visual Studio. Was there a problem reading a .NET binary?")

    (objToHold, NativePtr.toNativeInt mmr.MetadataPointer, mmr.MetadataLength), peDataOpt

// TODO: Revisit this. A lot of the code below was taken from FCS and has assumptions on language service.
//     This will mean that some config options may not be right for actual compilation.
let createTcConfig assemblyPath projectDirectory projectReferences tryGetMetadataSnapshot useScriptResolutionRules =
    /// Create a type-check configuration
    let tcConfigB =
        // see also fsc.fs: runFromCommandLineToImportingAssemblies(), as there are many similarities to where the PS creates a tcConfigB
        let tcConfigB = 
            TcConfigBuilder.CreateNew(
                legacyReferenceResolver, 
                defaultFSharpBinariesDir, 
                implicitIncludeDir = projectDirectory, 
                reduceMemoryUsage = ReduceMemoryFlag.Yes, 
                isInteractive = false, 
                isInvalidationSupported = true, 
                defaultCopyFSharpCore = CopyFSharpCoreFlag.No, 
                tryGetMetadataSnapshot = tryGetMetadataSnapshot) 

        tcConfigB.resolutionEnvironment <- (ReferenceResolver.ResolutionEnvironment.EditingOrCompilation true)

        tcConfigB.conditionalCompilationDefines <- 
            let define = if useScriptResolutionRules then "INTERACTIVE" else "COMPILED"
            define :: tcConfigB.conditionalCompilationDefines

        tcConfigB.projectReferences <- projectReferences

        tcConfigB.useSimpleResolution <- true

        tcConfigB.includewin32manifest <- false

        tcConfigB.implicitlyResolveAssemblies <- false

        // Never open PDB files for the language service, even if --standalone is specified
        tcConfigB.openDebugInformationForLaterStaticLinking <- false

        tcConfigB.compilationThread <-
            { new ICompilationThread with
                member __.EnqueueWork work =
                    CompilationWorker.Enqueue work
            }

        tcConfigB.outputFile <- Some assemblyPath

        tcConfigB.primaryAssembly <- PrimaryAssembly.System_Private_CoreLib // TODO: fix this.

        tcConfigB.skipAssemblyResolution <- true

        tcConfigB

    TcConfig.Create(tcConfigB, validate = true)

let createLoadClosure filename sourceText tryGetMetadataSnapshot ctok =
    let applyCompilerOptions tcConfigB  = 
        let fsiCompilerOptions = CompileOptions.GetCoreFsiCompilerOptions tcConfigB 
        CompileOptions.ParseCompilerOptions (ignore, fsiCompilerOptions, [ ])

    LoadClosure.ComputeClosureOfScriptText(ctok, legacyReferenceResolver, 
        defaultFSharpBinariesDir, filename, sourceText, 
        CodeContext.CompilationAndEvaluation, true, false, false, new Lexhelp.LexResourceManager(), 
        applyCompilerOptions, false, 
        tryGetMetadataSnapshot = tryGetMetadataSnapshot, 
        reduceMemoryUsage = ReduceMemoryFlag.Yes)

let createTcGlobalsAndTcImports (tcConfig: TcConfig) assemblyResolutions (importsInvalidated: Event<string>) ctok =
      cancellable {
        let errorLogger = CompilationErrorLogger("createTcGlobalsAndTcImports", tcConfig.errorSeverityOptions)
        // Return the disposable object that cleans up
        use _holder = new CompilationGlobalsScope(errorLogger, BuildPhase.Parameter)

        let tcConfigP = TcConfigProvider.Constant tcConfig

        let! (tcGlobals, tcImports) = TcImports.BuildFrameworkTcImports (ctok, tcConfigP, assemblyResolutions, [])

#if !NO_EXTENSIONTYPING
        tcImports.GetCcusExcludingBase() |> Seq.iter (fun ccu -> 
            // When a CCU reports an invalidation, merge them together and just report a 
            // general "imports invalidated". This triggers a rebuild.
            //
            // We are explicit about what the handler closure captures to help reason about the
            // lifetime of captured objects, especially in case the type provider instance gets leaked
            // or keeps itself alive mistakenly, e.g. via some global state in the type provider instance.
            //
            // The handler only captures
            //    1. a weak reference to the importsInvalidated event.
            //
            // In the invalidation handler we use a weak reference to allow the owner to 
            // be collected if, for some reason, a TP instance is not disposed or not GC'd.
            let capturedImportsInvalidated = WeakReference<_>(importsInvalidated)
            ccu.Deref.InvalidateEvent.Add(fun msg -> 
                match capturedImportsInvalidated.TryGetTarget() with 
                | true, tg -> tg.Trigger msg
                | _ -> ()))
#endif

        return (tcGlobals, tcImports) } |> Cancellable.runWithoutCancellation

type FSharpOutputKind =
    | Exe = 0
    | WinExe = 1
    | Library = 2

type CompilationConfig =
    {
        suggestNamesForErrors: bool
        commandLineArgs: string list
        useScriptResolutionRules: bool
        script: FSharpSource option
        assemblyPath: string
        assemblyName: string
        isExecutable: bool
        keepAssemblyContents: bool
        keepAllBackgroundResolutions: bool
        sources: ImmutableArray<FSharpSource>
        metadataReferences: ImmutableArray<FSharpMetadataReference>
    }

    member this.CreateIncrementalChecker (importsInvalidated) =

        let metadataPointers =
            this.metadataReferences
            |> Seq.map (fun x ->
                match x with
                | FSharpMetadataReference.PortableExecutable metadata -> 
                    x.Id, (getRawPointerMetadataSnapshot (metadata.GetMetadata (), None))

                // TODO: This could be optimized in that we may not need to emit the entire compilation and/or keep the PE reference in a weak table.
                //       At the moment, this is just necessary for correct importing.
                //       A possible way to optimize this is to grab the results just before IL generation (IlxGen) and after optimization.
                //           Once we have the results, we should have a Ccu and other pieces such as optimization data. 
                //           The Ccu will need to be remapped to a different scope so that it can be used from the outside.
                //           The reason is that the Ccu has the assumption that it is being used in a local context.
                //           At the moment, the code responsible for remapping is not fitted to fix the local scope in all cases, but can be.
                //           When the Ccu is remapped properly, an IRawFSharpAssemblyData will need to be implemented based off of it.
                //           Once that is complete, it should be able to passed to an IProjectReference which AssemblyResolution has as an option.
                //           IProjectReference is not exactly a good name for itself. Compiler should not know about 'projects'.
                //       Other optimizations will be necessary for editor/tooling scenarios in IDEs where emitting is too expensive.
                //           For that scenario, we only need to get the results right after type checking.
                //           The current state of compilation is not ready for a full IDE experience yet.
                | FSharpMetadataReference.FSharpCompilation c ->
                    let ms = new MemoryStream ()
                    match c.Emit ms with
                    | Ok _ -> ()
                    | Result.Error diags -> failwithf "%A" diags
                    ms.Position <- 0L
                    let peReader = new System.Reflection.PortableExecutable.PEReader(ms)
                    let md = peReader.GetMetadata()
                    let pe = peReader.GetEntireImage()

                    let o =
                        { new System.Object () with
                            override this.Finalize() =
                                GC.SuppressFinalize(this)
                                peReader.Dispose() }

                    let mddata = (o, md.Pointer |> NativePtr.toNativeInt, md.Length)
                    let pedata = (o, pe.Pointer |> NativePtr.toNativeInt, pe.Length)
                    x.Id, (mddata, Some pedata)
            )
            |> Array.ofSeq

        let tryGetMetadataSnapshot (id, _) = 
            metadataPointers
            |> Seq.tryFind (fun (metadataId, _) -> 
                String.Equals (id, metadataId, StringComparison.OrdinalIgnoreCase) ||
                // TODO: This is currently a hack to get around from having to have a path.
                //       OpenILModuleReader calls FileSystem.GetPullPathShim and passes that result to tryGetMetadataSnapshot.
                String.Equals (id, FileSystem.GetFullPathShim metadataId, StringComparison.OrdinalIgnoreCase)
            )
            |> Option.map (fun (_, pointerInfo) -> pointerInfo)

        let tcConfig = createTcConfig this.assemblyName this.assemblyPath [] tryGetMetadataSnapshot this.useScriptResolutionRules

        let assemblyResolutions =
            this.metadataReferences
            |> Seq.map (function
                | FSharpMetadataReference.PortableExecutable peRef ->
                    {
                        originalReference = AssemblyReference (Range.range0, peRef.FilePath, None)
                        resolvedPath = peRef.FilePath
                        prepareToolTip = fun () -> peRef.Display
                        sysdir = false
                        ilAssemblyRef = ref None
                    }
                | FSharpMetadataReference.FSharpCompilation c ->
                    {
                        originalReference = AssemblyReference (Range.range0, c.AssemblyName, None)
                        resolvedPath = c.AssemblyName // ugly be ok
                        prepareToolTip = fun () -> c.AssemblyName
                        sysdir = false
                        ilAssemblyRef = ref None
                    }
            ) |> List.ofSeq

        CompilationWorker.EnqueueAndAwaitAsync (fun ctok ->
            let tcGlobals, tcImports = createTcGlobalsAndTcImports tcConfig assemblyResolutions importsInvalidated ctok

            let checkerOptions =
                {
                    keepAssemblyContents = this.keepAssemblyContents
                    keepAllBackgroundResolutions = this.keepAllBackgroundResolutions
                    parsingOptions = { isExecutable = this.isExecutable; isScript = this.useScriptResolutionRules }
                }

            let loadClosureOpt =           
                this.script
                |> Option.map (fun src -> (src.FilePath, src.GetText(CancellationToken.None).ToFSharpSourceText()))
                |> Option.map (fun (filename, sourceText) -> createLoadClosure filename sourceText tryGetMetadataSnapshot ctok)

            let srcs =
                match loadClosureOpt with
                | Some loadClosure ->
                    loadClosure.SourceFiles
                    |> List.map (fun (filePath, _) ->
                        if this.script.Value.FilePath = filePath then
                            this.script.Value
                        else
                            FSharpSource.FromFile filePath
                    )
                    |> ImmutableArray.CreateRange
                | _ ->
                    this.sources

            let tcAcc = TcAccumulator.create this.assemblyName tcConfig tcGlobals tcImports (NiceNameGenerator ()) loadClosureOpt

            IncrementalChecker.Create (tcConfig, tcGlobals, tcImports, tcAcc, checkerOptions, srcs)
        ) |> Async.RunSynchronously

and [<NoEquality;NoComparison;RequireQualifiedAccess>] FSharpMetadataReference =
    | PortableExecutable of PortableExecutableReference
    | FSharpCompilation of FSharpCompilation 
    // TODO: Add CSharp and VB compilations.

    member this.Id =
        match this with
        | FSharpMetadataReference.PortableExecutable peRef -> peRef.FilePath
        | FSharpMetadataReference.FSharpCompilation c -> c.AssemblyName

    static member FromPortableExecutableReference peRef =
        FSharpMetadataReference.PortableExecutable peRef

    static member FromFSharpCompilation c =
        FSharpMetadataReference.FSharpCompilation c

and [<NoEquality; NoComparison>] CompilationState =
    {
        filePathIndexMap: ImmutableDictionary<FSharpSource, int>
        cConfig: CompilationConfig
        lazyGetChecker: CancellableLazy<IncrementalChecker>
        asyncLazyPreEmit: AsyncLazy<PreEmitState>
        invalidated: Event<string>
    }

    static member Create cConfig =
        let srcs = cConfig.sources

        let filePathIndexMapBuilder = ImmutableDictionary.CreateBuilder ()
        for i = 0 to srcs.Length - 1 do
            let src = srcs.[i]
            if filePathIndexMapBuilder.ContainsKey src then
                failwith "Duplicate source when creating a compilation"
            else
                filePathIndexMapBuilder.Add (src, i)

        let invalidated = Event<string> ()

        let lazyGetChecker =
            CancellableLazy (fun _ ->
                cConfig.CreateIncrementalChecker (invalidated)
            )

        let asyncLazyPreEmit =
            AsyncLazy (async {
                let! ct = Async.CancellationToken
                let checker = lazyGetChecker.GetValue ct
                return! checker.FinishAsync ()
            })            

        {
            filePathIndexMap = filePathIndexMapBuilder.ToImmutableDictionary ()
            cConfig = cConfig
            lazyGetChecker = lazyGetChecker
            asyncLazyPreEmit= asyncLazyPreEmit
            invalidated = invalidated
        }

    member this.SetOptions cConfig =
        CompilationState.Create cConfig

    member this.ReplaceSource (oldSrc: FSharpSource, newSrc: FSharpSource) =
        match this.filePathIndexMap.TryGetValue oldSrc with
        | false, _ -> failwith "source does not exist in compilation"
        | _, filePathIndex ->

            let sourcesBuilder = this.cConfig.sources.ToBuilder ()

            sourcesBuilder.[filePathIndex] <- newSrc

            let options = { this.cConfig with sources = sourcesBuilder.MoveToImmutable () }

            let lazyGetChecker =
                CancellableLazy (fun ct ->
                    let checker = this.lazyGetChecker.GetValue ct
                    checker.ReplaceSource (oldSrc, newSrc)
                )

            let asyncLazyPreEmit =
                AsyncLazy (async {
                    let! ct = Async.CancellationToken
                    let checker = lazyGetChecker.GetValue ct
                    return! checker.FinishAsync ()
                })  

            { this with cConfig = options; lazyGetChecker = lazyGetChecker; asyncLazyPreEmit = asyncLazyPreEmit }

    member this.SubmitSource (src: FSharpSource) =
        let cConfig = 
            { this.cConfig with 
                sources = ImmutableArray.Create src
                assemblyName = Path.GetFileNameWithoutExtension src.FilePath }

        let lazyGetChecker =
            CancellableLazy (fun ct ->
                let checker = this.lazyGetChecker.GetValue ct
                checker.SubmitSource (src, ct)
            )

        let asyncLazyPreEmit =
            AsyncLazy (async {
                let! ct = Async.CancellationToken
                let checker = lazyGetChecker.GetValue ct
                return! checker.FinishAsync ()
            })  

        { this with 
            cConfig = cConfig
            lazyGetChecker = lazyGetChecker
            filePathIndexMap = ImmutableDictionary.CreateRange [|KeyValuePair(src, 0)|]
            asyncLazyPreEmit = asyncLazyPreEmit }              

and [<Sealed>] FSharpCompilation (state: CompilationState) as this =

    let lazyGetChecker = state.lazyGetChecker
    let gate = obj ()

    let check src =
        if not (state.filePathIndexMap.ContainsKey src) then
            failwith "Source does not exist in compilation."

    let getSemanticModel src =
        check src
        FSharpSemanticModel (src, lazyGetChecker, this)

    let getSyntaxTree src =
        check src
        // Note: Getting a syntax tree requires that the checker be built. This is due to the tcConfig dependency when parsing a syntax tree.
        //       When parsing does not require tcConfig, we can build the syntax trees in Compilation and pass them directly to the checker when it gets built.
        // TODO: Remove this when we fix the tcConfig dependency on parsing.
        let checker = state.lazyGetChecker.GetValue ()
        checker.GetSyntaxTree src

    member __.State = state

    member __.Config = state.cConfig

    member __.Gate = gate

    member __.SetConfig cConfig =
        FSharpCompilation (state.SetOptions cConfig)

    member __.ReplaceSource (oldSrc, newSrc) =
        check oldSrc
        FSharpCompilation (state.ReplaceSource (oldSrc, newSrc))

    member __.GetSemanticModel src =
        getSemanticModel src

    member __.GetSyntaxTree src =
        getSyntaxTree src

    member __.AssemblyName = state.cConfig.assemblyName

    member __.GetSyntaxAndSemanticDiagnostics ?ct =
        let ct = defaultArg ct CancellationToken.None

        let preEmitState = Async.RunSynchronously (state.asyncLazyPreEmit.GetValueAsync (), cancellationToken = ct)

        let syntaxDiagnostics =
            let builder = ImmutableArray.CreateBuilder ()
            state.cConfig.sources
            |> ImmutableArray.iter (fun x -> 
                let semanticModel = getSemanticModel x
                builder.AddRange(semanticModel.GetDiagnostics ct)
            )
            builder.ToImmutable ()

        let semanticDiagnostics = preEmitState.tcErrors.ToDiagnostics ()
        
        let builder = ImmutableArray.CreateBuilder (syntaxDiagnostics.Length + semanticDiagnostics.Length)
        builder.AddRange syntaxDiagnostics
        builder.AddRange semanticDiagnostics
        builder.ToImmutable ()

    member this.Emit (peStream: Stream, ?pdbStream, ?ct) =
        let ct = defaultArg ct CancellationToken.None

        if not this.Config.keepAssemblyContents then
            failwith "This kind of compilation does not support Emit."

        let synSemDiags = this.GetSyntaxAndSemanticDiagnostics ct

        if synSemDiags |> Seq.exists (fun x -> x.Severity = DiagnosticSeverity.Error) then
            Result.Error synSemDiags
        else

        Async.RunSynchronously (async {
            let! ct = Async.CancellationToken
            let! preEmitState = state.asyncLazyPreEmit.GetValueAsync ()
            let checker = state.lazyGetChecker.GetValue ct
            
            let tcConfig = checker.TcConfig
            let tcGlobals = checker.TcGlobals
            let tcImports = checker.TcImports
            let generatedCcu = preEmitState.tcState.Ccu
            let topAttribs = preEmitState.topAttribs
            let outfile = this.Config.assemblyPath
            let assemblyName = this.AssemblyName
            let pdbfile = Some (Path.ChangeExtension(outfile, ".pdb"))
            let typedImplFiles = preEmitState.implFiles

            let signingInfo = Driver.ValidateKeySigningAttributes (tcConfig, tcGlobals, topAttribs)

            // TODO: Need to look at what happens when we specify a dynamic assembly creator. I think it changes the IlBackend writing slightly.
            let dynamicAssemblyCreator asmStream pdbStream =
                Some (fun (_, _, ilModDef: ILModuleDef) ->
                    let options: ILBinaryWriter.options =
                        { ilg = tcGlobals.ilg
                          pdbfile=pdbfile
                          emitTailcalls = tcConfig.emitTailcalls
                          deterministic = tcConfig.deterministic
                          showTimes = tcConfig.showTimes
                          portablePDB = tcConfig.portablePDB
                          embeddedPDB = tcConfig.embeddedPDB
                          embedAllSource = tcConfig.embedAllSource
                          embedSourceList = tcConfig.embedSourceList
                          sourceLink = tcConfig.sourceLink
                          checksumAlgorithm = tcConfig.checksumAlgorithm
                          signer = GetStrongNameSigner signingInfo
                          dumpDebugInfo = tcConfig.dumpDebugInfo
                          pathMap = tcConfig.pathMap }
                    ILBinaryWriter.WriteILBinaryToStreams (outfile, options, ilModDef, (fun x -> x), asmStream, pdbStream)
                )

            let errorLogger = CompilationErrorLogger("Emit", tcConfig.errorSeverityOptions)

            let exiter =
                { new Exiter with
                    member __.Exit _ = Unchecked.defaultof<_> }

            do! CompilationWorker.EnqueueAndAwaitAsync (fun ctok ->
                let pdbStream2 =
                    match pdbStream with
                    | Some pdbStream -> pdbStream
                    | _ -> new MemoryStream() :> Stream
                try
                    Driver.encodeAndOptimizeAndCompile (
                        ctok, tcConfig, tcImports, tcGlobals, errorLogger, generatedCcu, outfile, typedImplFiles, 
                        topAttribs, pdbfile, assemblyName, signingInfo, exiter, dynamicAssemblyCreator peStream pdbStream2)
                finally
                    if pdbStream.IsNone then
                        pdbStream2.Dispose()
            )

            let diags = errorLogger.GetErrors().ToErrorInfos().ToDiagnostics()
            if not diags.IsEmpty then
                return Result.Error diags
            else
                return Result.Ok ()
        }, cancellationToken = ct)

type FSharpCompilation with

    static member CreateAux (assemblyName, srcs, metadataReferences, ?outputKind, ?canEmit, ?script, ?args) =
        if Path.HasExtension assemblyName || Path.IsPathRooted assemblyName then
            failwith "Assembly name must not be a file path."

        if String.IsNullOrWhiteSpace assemblyName then
            failwith "Assembly name must not be null or contain whitespace."

        if assemblyName |> String.exists (Char.IsLetterOrDigit >> not) then
            failwith "Assembly name contains an invalid character."

        let outputKind = defaultArg outputKind FSharpOutputKind.Exe
        let canEmit = defaultArg canEmit true

        let isScript = script.IsSome
        let suggestNamesForErrors = canEmit
        let useScriptResolutionRules = isScript
        let isExecutable = outputKind = FSharpOutputKind.Exe || outputKind = FSharpOutputKind.WinExe

        let fileExt =
            if isExecutable then
                ".exe"
            else
                ".dll"

        let cConfig =
            {
                suggestNamesForErrors = suggestNamesForErrors
                commandLineArgs = defaultArg args []
                useScriptResolutionRules = useScriptResolutionRules
                script = script
                assemblyName = assemblyName
                assemblyPath = Path.Combine (Environment.CurrentDirectory, Path.ChangeExtension (assemblyName, fileExt))
                isExecutable = isExecutable
                keepAssemblyContents = canEmit
                keepAllBackgroundResolutions = false
                sources = if isScript then ImmutableArray.Create script.Value else srcs
                metadataReferences = metadataReferences
            }
        FSharpCompilation (CompilationState.Create cConfig)

    static member Create (assemblyName, srcs, metadataReferences, ?outputKind: FSharpOutputKind, ?args) =
        let outputKind = defaultArg outputKind FSharpOutputKind.Exe
        FSharpCompilation.CreateAux (assemblyName, srcs, metadataReferences, outputKind = outputKind, args = defaultArg args [])

    static member CreateScript (script: FSharpSource, metadataReferences, args) =
        FSharpCompilation.CreateAux (Path.GetFileNameWithoutExtension script.FilePath, ImmutableArray.Empty, metadataReferences, script = script, args = defaultArg args [])

    static member CreateScript (previousCompilation: FSharpCompilation, script, ?additionalMetadataReferences: ImmutableArray<FSharpMetadataReference>) =
        let _additionalMetadataReferences = defaultArg additionalMetadataReferences ImmutableArray.Empty

        FSharpCompilation (previousCompilation.State.SubmitSource script)

[<AutoOpen>]
module FSharpSemanticModelExtensions =

    type FSharpSemanticModel with

        // this is a hack because of file ordering
        member this.Compilation = this.CompilationObj :?> FSharpCompilation
