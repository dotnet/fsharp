namespace FSharp.Compiler.Compilation

open System
open System.IO
open System.Threading
open System.Threading.Tasks
open System.Collections.Immutable
open System.Collections.Generic
open System.Collections.Concurrent
open Internal.Utilities.Collections
open FSharp.Compiler
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.Ast
open FSharp.Compiler.CompileOps
open FSharp.Compiler.Driver
open FSharp.Compiler.Tast
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.AbstractIL.ILBinaryReader
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.CompileOptions
open FSharp.Compiler.TypeChecker
open FSharp.Compiler.NameResolution
open Internal.Utilities
open FSharp.Compiler.Compilation.Utilities
open FSharp.Compiler.Compilation.IncrementalChecker
open FSharp.Compiler.AbstractIL
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.Tastops
open System.Diagnostics
open Microsoft.CodeAnalysis

#nowarn "9" // NativePtr.toNativeInt

[<AutoOpen>]
module FSharpCompilationHelpers = 

    open FSharp.NativeInterop

    let legacyReferenceResolver = LegacyMSBuildReferenceResolver.getResolver ()

    // TcState is arbitrary, doesn't matter as long as the type is inside FSharp.Compiler.Private, also this is yuck.
    let defaultFSharpBinariesDir = FSharpEnvironment.BinFolderOfDefaultFSharpCompiler(Some(typeof<FSharp.Compiler.CompileOps.TcState>.Assembly.Location)).Value

    let getRawPointerMetadataSnapshot (md: Microsoft.CodeAnalysis.Metadata) =
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

        (objToHold, NativePtr.toNativeInt mmr.MetadataPointer, mmr.MetadataLength)

    /// The implementation of the information needed by TcImports in CompileOps.fs for an F# assembly reference.
    //
    /// Constructs the build data (IRawFSharpAssemblyData) representing the assembly when used 
    /// as a cross-assembly reference.  Note the assembly has not been generated on disk, so this is
    /// a virtualized view of the assembly contents as computed by background checking.
    type private RawFSharpAssemblyDataBackedByLanguageService (tcConfig, tcGlobals, tcState: TcState, outfile, topAttrs, assemblyName, ilAssemRef) = 
    
        let generatedCcu = tcState.Ccu
        let exportRemapping = MakeExportRemapping generatedCcu generatedCcu.Contents
                          
        let sigData = 
            let _sigDataAttributes, sigDataResources = Driver.EncodeInterfaceData(tcConfig, tcGlobals, exportRemapping, generatedCcu, outfile, true)
            [ for r in sigDataResources  do
                let ccuName = GetSignatureDataResourceName r
                yield (ccuName, (fun () -> r.GetBytes())) ]
    
        let autoOpenAttrs = topAttrs.assemblyAttrs |> List.choose (List.singleton >> TryFindFSharpStringAttribute tcGlobals tcGlobals.attrib_AutoOpenAttribute)
    
        let ivtAttrs = topAttrs.assemblyAttrs |> List.choose (List.singleton >> TryFindFSharpStringAttribute tcGlobals tcGlobals.attrib_InternalsVisibleToAttribute)
    
        interface IRawFSharpAssemblyData with 
            member __.GetAutoOpenAttributes(_ilg) = autoOpenAttrs
            member __.GetInternalsVisibleToAttributes(_ilg) =  ivtAttrs
            member __.TryGetILModuleDef() = None
            member __.GetRawFSharpSignatureData(_m, _ilShortAssemName, _filename) = sigData
            member __.GetRawFSharpOptimizationData(_m, _ilShortAssemName, _filename) = [ ]
            member __.GetRawTypeForwarders() = mkILExportedTypes []  // TODO: cross-project references with type forwarders
            member __.ShortAssemblyName = assemblyName
            member __.ILScopeRef = IL.ILScopeRef.Assembly ilAssemRef
            member __.ILAssemblyRefs = [] // These are not significant for service scenarios
            member __.HasAnyFSharpSignatureDataAttribute =  true
            member __.HasMatchingFSharpSignatureDataAttribute _ilg = true

    let private TryFindFSharpStringAttribute tcGlobals attribSpec attribs =
        match TryFindFSharpAttribute tcGlobals attribSpec attribs with
        | Some (Attrib(_, _, [ AttribStringArg s ], _, _, _, _))  -> Some s
        | _ -> None

    type PreEmitResult =
        {
            ilAssemblyRef: ILAssemblyRef
            assemblyDataOpt: IRawFSharpAssemblyData option
            assemblyExprOpt: TypedImplFile list option
            finalAccWithErrors: TcAccumulator
            tcStates: TcAccumulator []
        }

    let preEmit (assemblyName: string) (outfile: string) (tcConfig: TcConfig) (tcGlobals: TcGlobals) (tcStates: TcAccumulator []) =
        let errorLogger = CompilationErrorLogger("preEmit", tcConfig.errorSeverityOptions)
        use _holder = new CompilationGlobalsScope(errorLogger, BuildPhase.TypeCheck)

        // Get the state at the end of the type-checking of the last file
        let finalAcc = tcStates.[tcStates.Length-1]

        // Finish the checking
        let (_tcEnvAtEndOfLastFile, topAttrs, mimpls, _), tcState = 
            let results = tcStates |> List.ofArray |> List.map (fun acc-> acc.tcEnvAtEndOfFile, defaultArg acc.topAttribs EmptyTopAttrs, acc.latestImplFile, acc.latestCcuSigForFile)
            TypeCheckMultipleInputsFinish (results, finalAcc.tcState)
  
        let ilAssemRef, tcAssemblyDataOpt, tcAssemblyExprOpt = 
            try
                // TypeCheckClosedInputSetFinish fills in tcState.Ccu but in incremental scenarios we don't want this, 
                // so we make this temporary here
                let oldContents = tcState.Ccu.Deref.Contents
                try
                    let tcState, tcAssemblyExpr = TypeCheckClosedInputSetFinish (mimpls, tcState)

                    // Compute the identity of the generated assembly based on attributes, options etc.
                    // Some of this is duplicated from fsc.fs
                    let ilAssemRef = 
                        let publicKey = 
                            try 
                                let signingInfo = Driver.ValidateKeySigningAttributes (tcConfig, tcGlobals, topAttrs)
                                match Driver.GetStrongNameSigner signingInfo with 
                                | None -> None
                                | Some s -> Some (PublicKey.KeyAsToken(s.PublicKey))
                            with e -> 
                                errorRecoveryNoRange e
                                None
                        let locale = TryFindFSharpStringAttribute tcGlobals (tcGlobals.FindSysAttrib  "System.Reflection.AssemblyCultureAttribute") topAttrs.assemblyAttrs
                        let assemVerFromAttrib = 
                            TryFindFSharpStringAttribute tcGlobals (tcGlobals.FindSysAttrib "System.Reflection.AssemblyVersionAttribute") topAttrs.assemblyAttrs 
                            |> Option.bind  (fun v -> try Some (parseILVersion v) with _ -> None)
                        let ver = 
                            match assemVerFromAttrib with 
                            | None -> tcConfig.version.GetVersionInfo(tcConfig.implicitIncludeDir)
                            | Some v -> v
                        ILAssemblyRef.Create(assemblyName, None, publicKey, false, Some ver, locale)
            
                    let tcAssemblyDataOpt = 
                        try

                          // Assemblies containing type provider components can not successfully be used via cross-assembly references.
                          // We return 'None' for the assembly portion of the cross-assembly reference 
                          let hasTypeProviderAssemblyAttrib = 
                              topAttrs.assemblyAttrs |> List.exists (fun (Attrib(tcref, _, _, _, _, _, _)) -> 
                                  let nm = tcref.CompiledRepresentationForNamedType.BasicQualifiedName 
                                  nm = typeof<Microsoft.FSharp.Core.CompilerServices.TypeProviderAssemblyAttribute>.FullName)

                          if tcState.CreatesGeneratedProvidedTypes || hasTypeProviderAssemblyAttrib then
                            None
                          else
                            Some  (RawFSharpAssemblyDataBackedByLanguageService (tcConfig, tcGlobals, tcState, outfile, topAttrs, assemblyName, ilAssemRef) :> IRawFSharpAssemblyData)

                        with e -> 
                            errorRecoveryNoRange e
                            None
                    ilAssemRef, tcAssemblyDataOpt, Some tcAssemblyExpr
                finally 
                    tcState.Ccu.Deref.Contents <- oldContents
            with e -> 
                errorRecoveryNoRange e
                mkSimpleAssemblyRef assemblyName, None, None

        let finalAccWithErrors = 
            { finalAcc with 
                tcErrorsRev = errorLogger.GetErrorInfos() :: finalAcc.tcErrorsRev 
                topAttribs = Some topAttrs
            }

        {
            ilAssemblyRef = ilAssemRef
            assemblyDataOpt = tcAssemblyDataOpt
            assemblyExprOpt = tcAssemblyExprOpt
            finalAccWithErrors = finalAccWithErrors
            tcStates = tcStates
        }

[<Struct>]
type CompilationId private (_guid: Guid) =

    static member Create () = CompilationId (Guid.NewGuid ())

[<RequireQualifiedAccess>]
module IncrementalCheckerCache =

    let private cache = 
        // Is a Mru algorithm really the right cache for these? A Lru might be better.
        MruWeakCache<struct (CompilationId * VersionStamp), IncrementalChecker> (
            cacheSize = 3, 
            weakReferenceCacheSize = 1000, 
            equalityComparer = EqualityComparer<struct (CompilationId * VersionStamp)>.Default
        ) 

    let set key value =
        cache.Set (key, value)

    let tryGetValue key =
        cache.TryGetValue key

type FSharpCompilationOptions =
    {
        SuggestNamesForErrors: bool
        CommandLineArgs: string list
        ProjectDirectory: string
        UseScriptResolutionRules: bool
        Script: FSharpSourceSnapshot option
        AssemblyPath: string
        IsExecutable: bool
        KeepAssemblyContents: bool
        KeepAllBackgroundResolutions: bool
        SourceSnapshots: ImmutableArray<FSharpSourceSnapshot>
        MetadataReferences: ImmutableArray<FSharpMetadataReference>
    }

    member options.CreateTcInitial ctok =
        let tryGetMetadataSnapshot (path, _) =
            let metadataReferenceOpt = 
                options.MetadataReferences
                |> Seq.choose(function
                    | FSharpMetadataReference.PortableExecutable peReference -> Some peReference
                    | _ -> None
                )
                |> Seq.tryFind (fun x -> String.Equals (path, x.FilePath, StringComparison.OrdinalIgnoreCase))
            match metadataReferenceOpt with
            | Some metadata -> Some (getRawPointerMetadataSnapshot (metadata.GetMetadata ()))
            | _ -> failwith "should not happen" // this should not happen because we construct references for the command line args here. existing references from the command line args are removed.

        let commandLineArgs =
            // clear any references as we get them from metadata references
            options.CommandLineArgs
            |> List.filter (fun x -> not (x.Contains("-r:")))

        let commandLineArgs =
            ["--noframework"] @
            commandLineArgs @
            (options.MetadataReferences
             |> Seq.map (function
                | FSharpMetadataReference.PortableExecutable peReference ->
                    "-r:" + peReference.FilePath
                | FSharpMetadataReference.FSharpCompilation compilation ->
                    "-r:" + compilation.OutputFilePath
             )
             |> List.ofSeq)

        let projectReferences =
            options.MetadataReferences
            |> Seq.choose(function
                | FSharpMetadataReference.FSharpCompilation compilation ->
                    Some compilation
                | _ ->
                    None
            )
            |> Seq.map (fun x ->
                { new IProjectReference with
                
                    member __.FileName = x.OutputFilePath

                    member __.EvaluateRawContents _ =
                        cancellable {
                            let! ct = Cancellable.token ()
                            return Async.RunSynchronously(x.GetAssemblyDataAsync (), cancellationToken = ct)
                        }

                    member __.TryGetLogicalTimeStamp (_, _) = None
                }
            )
            |> List.ofSeq

        let tcInitialOptions =
            {
                legacyReferenceResolver = legacyReferenceResolver
                defaultFSharpBinariesDir = defaultFSharpBinariesDir
                tryGetMetadataSnapshot = tryGetMetadataSnapshot
                suggestNamesForErrors = options.SuggestNamesForErrors
                sourceFiles = options.SourceSnapshots |> Seq.map (fun x -> x.FilePath) |> List.ofSeq
                commandLineArgs = commandLineArgs
                projectDirectory = options.ProjectDirectory
                projectReferences = projectReferences
                useScriptResolutionRules = options.UseScriptResolutionRules
                assemblyPath = options.AssemblyPath
                isExecutable = options.IsExecutable
                keepAssemblyContents = options.KeepAssemblyContents
                keepAllBackgroundResolutions = options.KeepAllBackgroundResolutions
                script = match options.Script with Some script -> Some (script.FilePath, script.GetText(CancellationToken.None).ToFSharpSourceText()) | _ -> None
            }
        TcInitial.create ctok tcInitialOptions

    member options.CreateIncrementalChecker ctok =
        let tcInitial = options.CreateTcInitial ctok
        let tcGlobals, tcImports, tcAcc = TcAccumulator.createInitial tcInitial ctok |> Cancellable.runWithoutCancellation
        let checkerOptions =
            {
                keepAssemblyContents = options.KeepAssemblyContents
                keepAllBackgroundResolutions = options.KeepAllBackgroundResolutions
                parsingOptions = { isExecutable = options.IsExecutable; isScript = options.UseScriptResolutionRules }
            }

        let sourceSnapshots =
            match tcInitial.loadClosureOpt with
            | Some loadClosure ->
                loadClosure.SourceFiles
                |> List.map (fun (filePath, _) ->
                    if options.Script.Value.FilePath = filePath then
                        options.Script.Value
                    else
                        FSharpSourceSnapshot.FromText (filePath, Text.SourceText.From (File.ReadAllText filePath))
                )
                |> ImmutableArray.CreateRange
            | _ ->
                options.SourceSnapshots

        IncrementalChecker.Create (tcInitial, tcGlobals, tcImports, tcAcc, checkerOptions, sourceSnapshots)
        |> Cancellable.runWithoutCancellation

and [<RequireQualifiedAccess>] FSharpMetadataReference =
    | PortableExecutable of PortableExecutableReference
    | FSharpCompilation of FSharpCompilation 

and [<NoEquality; NoComparison>] CompilationState =
    {
        filePathIndexMap: ImmutableDictionary<string, int>
        options: FSharpCompilationOptions
        lazyGetChecker: CancellableLazy<IncrementalChecker>
        asyncLazyPreEmit: AsyncLazy<PreEmitResult>
    }

    static member Create options =
        let sourceSnapshots = options.SourceSnapshots

        let filePathIndexMapBuilder = ImmutableDictionary.CreateBuilder (StringComparer.OrdinalIgnoreCase)
        for i = 0 to sourceSnapshots.Length - 1 do
            let sourceSnapshot = sourceSnapshots.[i]
            if filePathIndexMapBuilder.ContainsKey sourceSnapshot.FilePath then
                failwithf "Duplicate file path when creating a compilation. File path: %s" sourceSnapshot.FilePath
            else
                filePathIndexMapBuilder.Add (sourceSnapshot.FilePath, i)

        let lazyGetChecker =
            CancellableLazy (fun ct ->
                let work =
                    CompilationWorker.EnqueueAndAwaitAsync (fun ctok -> 
                        options.CreateIncrementalChecker (ctok)
                    )
                Async.RunSynchronously (work, cancellationToken = ct)
            )

        let asyncLazyPreEmit =
            AsyncLazy (async {
                let! ct = Async.CancellationToken
                let checker = lazyGetChecker.GetValue ct
                let! tcAccs = checker.FinishAsync ()
                let tcInitial = checker.TcInitial
                let tcGlobals = checker.TcGlobals
                return preEmit tcInitial.assemblyName tcInitial.outfile tcInitial.tcConfig tcGlobals tcAccs
            })
               

        {
            filePathIndexMap = filePathIndexMapBuilder.ToImmutableDictionary ()
            options = options
            lazyGetChecker = lazyGetChecker
            asyncLazyPreEmit= asyncLazyPreEmit
        }

    member this.SetOptions options =
        CompilationState.Create options

    member this.ReplaceSourceSnapshot (sourceSnapshot: FSharpSourceSnapshot) =
        match this.filePathIndexMap.TryGetValue sourceSnapshot.FilePath with
        | false, _ -> failwith "source snapshot does not exist in compilation"
        | _, filePathIndex ->

            let sourceSnapshotsBuilder = this.options.SourceSnapshots.ToBuilder ()

            sourceSnapshotsBuilder.[filePathIndex] <- sourceSnapshot

            let options = { this.options with SourceSnapshots = sourceSnapshotsBuilder.MoveToImmutable () }

            let lazyGetChecker =
                CancellableLazy (fun ct ->
                    let checker = this.lazyGetChecker.GetValue ct
                    checker.ReplaceSourceSnapshot sourceSnapshot
                )

            { this with options = options; lazyGetChecker = lazyGetChecker }

and [<Sealed>] FSharpCompilation (id: CompilationId, state: CompilationState, version: VersionStamp) as this =

    let lazyGetChecker =
        CancellableLazy (fun ct ->
            match IncrementalCheckerCache.tryGetValue struct (id, version) with
            | ValueSome checker -> checker
            | _ -> 
                let checker = state.lazyGetChecker.GetValue ct
                IncrementalCheckerCache.set struct (id, version) checker
                checker
        )

    let checkFilePath filePath =
        if not (state.filePathIndexMap.ContainsKey filePath) then
            failwithf "File path does not exist in compilation. File path: %s" filePath

    let getSemanticModel filePath =
        checkFilePath filePath
        FSharpSemanticModel (filePath, lazyGetChecker, this)

    member __.Id = id

    member __.Version = version

    member __.Options = state.options

    member __.SetOptions options =
        FSharpCompilation (id, state.SetOptions options, version.GetNewerVersion ())

    member __.ReplaceSourceSnapshot (sourceSnapshot: FSharpSourceSnapshot) =
        checkFilePath sourceSnapshot.FilePath
        FSharpCompilation (id, state.ReplaceSourceSnapshot sourceSnapshot, version.GetNewerVersion ())

    member __.GetSemanticModel filePath =
        getSemanticModel filePath

    member __.GetSyntaxTree filePath =
        checkFilePath filePath
        // Note: Getting a syntax tree requires that the checker be built. This is due to the tcConfig dependency when parsing a syntax tree.
        //       When parsing does not require tcConfig, we can build the syntax trees in Compilation and pass them directly to the checker when it gets built.
        // TODO: Remove this when we fix the tcConfig dependency on parsing.
        let checker = state.lazyGetChecker.GetValue ()
        checker.GetSyntaxTree filePath

    member __.GetAssemblyDataAsync () =
        async {
            let! result = state.asyncLazyPreEmit.GetValueAsync ()
            return result.assemblyDataOpt
        }

    member __.OutputFilePath = state.options.AssemblyPath

    member this.GetDiagnostics ?ct =
        let ct = defaultArg ct CancellationToken.None

        let preEmitResult = Async.RunSynchronously (state.asyncLazyPreEmit.GetValueAsync (), cancellationToken = ct)
        
        let initialDiagnostics =
            let errors =
                preEmitResult.finalAccWithErrors.tcErrorsRev
                |> List.head
            errors.ToDiagnostics ()

        let allSemanticModelDiagnostics =
            let builder = ImmutableArray.CreateBuilder ()
            state.options.SourceSnapshots
            |> ImmutableArray.iter (fun x -> 
                let semanticModel = getSemanticModel x.FilePath
                builder.AddRange(semanticModel.GetDiagnostics ct)
            )
            builder.ToImmutable ()

        let finalDiagnostics =
            let errors =
                preEmitResult.finalAccWithErrors.tcErrorsRev
                |> List.last
            errors.ToDiagnostics ()
        
        let builder = ImmutableArray.CreateBuilder (allSemanticModelDiagnostics.Length + finalDiagnostics.Length + initialDiagnostics.Length)
        builder.AddRange initialDiagnostics
        builder.AddRange allSemanticModelDiagnostics
        builder.AddRange finalDiagnostics
        builder.ToImmutable ()

    member this.Emit (peStream, ?pdbStream, ?ct) =
        let ct = defaultArg ct CancellationToken.None

        if not this.Options.KeepAssemblyContents then
            failwith "This kind of compilation does not support Emit."

        let diags = this.GetDiagnostics ct

        if not diags.IsEmpty then
            Result.Error diags
        else

        Async.RunSynchronously (async {
            let! ct = Async.CancellationToken
            let! preEmitResult = state.asyncLazyPreEmit.GetValueAsync ()
            let checker = state.lazyGetChecker.GetValue ct

            let finalAcc = preEmitResult.finalAccWithErrors
            
            let tcConfig = checker.TcInitial.tcConfig
            let tcGlobals = checker.TcGlobals
            let tcImports = checker.TcImports
            let generatedCcu = finalAcc.tcState.Ccu
            let topAttribs = finalAcc.topAttribs.Value
            let outfile = this.OutputFilePath
            let assemblyName = Path.GetFileNameWithoutExtension this.OutputFilePath
            let pdbfile = Some (Path.ChangeExtension(this.OutputFilePath, ".pdb"))

            let typedImplFiles = 
                preEmitResult.tcStates
                |> Array.map (fun tcAcc ->
                    if tcAcc.latestImplFile.IsNone then
                        failwith "No imple file"
                    tcAcc.latestImplFile.Value
                )
                |> List.ofArray

            let signingInfo = Driver.ValidateKeySigningAttributes (tcConfig, tcGlobals, topAttribs)

            let dynamicAssemblyCreator asmStream pdbStreamOpt =
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
                    ILBinaryWriter.WriteILBinaryToStreams (this.OutputFilePath, options, ilModDef, (fun x -> x), asmStream, pdbStreamOpt)
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

    static member Create options =
        FSharpCompilation (CompilationId.Create (), CompilationState.Create options, VersionStamp.Create ())

    static member CreateAux (assemblyPath, projectDirectory, sourceSnapshots, metadataReferences, ?canEmit, ?scriptSnapshot, ?args) =
        let canEmit = defaultArg canEmit true

        let isScript = scriptSnapshot.IsSome
        let suggestNamesForErrors = not canEmit
        let useScriptResolutionRules = isScript

        let options =
            {
                SuggestNamesForErrors = suggestNamesForErrors
                CommandLineArgs = defaultArg args []
                ProjectDirectory = projectDirectory
                UseScriptResolutionRules = useScriptResolutionRules
                Script = scriptSnapshot
                AssemblyPath = assemblyPath
                IsExecutable = isScript
                KeepAssemblyContents = canEmit
                KeepAllBackgroundResolutions = false
                SourceSnapshots = if isScript then ImmutableArray.Create scriptSnapshot.Value else sourceSnapshots
                MetadataReferences = metadataReferences
            }
        FSharpCompilation.Create options

    static member Create (assemblyPath, projectDirectory, sourceSnapshots, metadataReferences, ?args) =
        FSharpCompilation.CreateAux (assemblyPath, projectDirectory, sourceSnapshots, metadataReferences, args = defaultArg args [])

    static member CreateScript (assemblyPath, projectDirectory, scriptSnapshot, metadataReferences, ?args) =
        FSharpCompilation.CreateAux (assemblyPath, projectDirectory, ImmutableArray.Empty, metadataReferences, scriptSnapshot = scriptSnapshot, args = defaultArg args [])

[<AutoOpen>]
module FSharpSemanticModelExtensions =

    type FSharpSemanticModel with

        // this is a hack because of file ordering
        member this.Compilation = this.CompilationObj :?> FSharpCompilation
