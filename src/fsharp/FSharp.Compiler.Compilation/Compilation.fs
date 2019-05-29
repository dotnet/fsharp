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
open FSharp.Compiler.AbstractIL
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.Tastops

[<AutoOpen>]
module CompilationHelpers = 
    let TryFindFSharpStringAttribute tcGlobals attribSpec attribs =
        match TryFindFSharpAttribute tcGlobals attribSpec attribs with
        | Some (Attrib(_, _, [ AttribStringArg s ], _, _, _, _))  -> Some s
        | _ -> None

/// The implementation of the information needed by TcImports in CompileOps.fs for an F# assembly reference.
//
/// Constructs the build data (IRawFSharpAssemblyData) representing the assembly when used 
/// as a cross-assembly reference.  Note the assembly has not been generated on disk, so this is
/// a virtualized view of the assembly contents as computed by background checking.
type RawFSharpAssemblyDataBackedByLanguageService (tcConfig, tcGlobals, tcState: TcState, outfile, topAttrs, assemblyName, ilAssemRef) = 

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

[<Struct>]
type CompilationId private (_guid: Guid) =

    static member Create () = CompilationId (Guid.NewGuid ())

type CompilationCaches =
    {
        incrementalCheckerCache: MruWeakCache<struct (CompilationId * VersionStamp), IncrementalChecker>
        frameworkTcImportsCache: FrameworkImportsCache
    }

type CompilationGlobalOptions =
    {
        LegacyReferenceResolver: ReferenceResolver.Resolver
        DefaultFSharpBinariesDir: string
        TryGetMetadataSnapshot: ILReaderTryGetMetadataSnapshot
    }

    static member Create () =
        let legacyReferenceResolver = SimulatedMSBuildReferenceResolver.GetBestAvailableResolver()
        
        // TcState is arbitrary, doesn't matter as long as the type is inside FSharp.Compiler.Private, also this is yuck.
        let defaultFSharpBinariesDir = FSharpEnvironment.BinFolderOfDefaultFSharpCompiler(Some(typeof<FSharp.Compiler.CompileOps.TcState>.Assembly.Location)).Value
        
        let tryGetMetadataSnapshot = (fun _ -> None)
        
        {
            LegacyReferenceResolver = legacyReferenceResolver
            DefaultFSharpBinariesDir = defaultFSharpBinariesDir
            TryGetMetadataSnapshot = tryGetMetadataSnapshot
        }

type CompilationOptions =
    {
        SuggestNamesForErrors: bool
        CommandLineArgs: string list
        ProjectDirectory: string
        UseScriptResolutionRules: bool
        AssemblyPath: string
        IsExecutable: bool
        KeepAssemblyContents: bool
        KeepAllBackgroundResolutions: bool
        SourceSnapshots: ImmutableArray<SourceSnapshot>
        CompilationReferences: ImmutableArray<Compilation>
    }

    static member Create (assemblyPath, projectDirectory, sourceSnapshots, compilationReferences) =
        let suggestNamesForErrors = false
        let useScriptResolutionRules = false
        {
            SuggestNamesForErrors = suggestNamesForErrors
            CommandLineArgs = []
            ProjectDirectory = projectDirectory
            UseScriptResolutionRules = useScriptResolutionRules
            AssemblyPath = assemblyPath
            IsExecutable = false
            KeepAssemblyContents = false
            KeepAllBackgroundResolutions = false
            SourceSnapshots = sourceSnapshots
            CompilationReferences = compilationReferences
        }

    member options.CreateTcInitial (frameworkTcImportsCache, globalOptions, ctok) =
        let tcInitialOptions =
            {
                frameworkTcImportsCache = frameworkTcImportsCache
                legacyReferenceResolver = globalOptions.LegacyReferenceResolver
                defaultFSharpBinariesDir = globalOptions.DefaultFSharpBinariesDir
                tryGetMetadataSnapshot = globalOptions.TryGetMetadataSnapshot
                suggestNamesForErrors = options.SuggestNamesForErrors
                sourceFiles = options.SourceSnapshots |> Seq.map (fun x -> x.FilePath) |> List.ofSeq
                commandLineArgs = options.CommandLineArgs
                projectDirectory = options.ProjectDirectory
                projectReferences = [] // TODO:
                useScriptResolutionRules = options.UseScriptResolutionRules
                assemblyPath = options.AssemblyPath
                isExecutable = options.IsExecutable
                keepAssemblyContents = options.KeepAssemblyContents
                keepAllBackgroundResolutions = options.KeepAllBackgroundResolutions
            }
        TcInitial.create tcInitialOptions ctok

    member options.CreateIncrementalChecker (frameworkTcImportsCache, globalOptions, ctok) =
        let tcInitial = options.CreateTcInitial (frameworkTcImportsCache, globalOptions, ctok)
        let tcImports, tcAcc = TcAccumulator.createInitial tcInitial ctok |> Cancellable.runWithoutCancellation
        let checkerOptions =
            {
                keepAssemblyContents = options.KeepAssemblyContents
                keepAllBackgroundResolutions = options.KeepAllBackgroundResolutions
                parsingOptions = { isExecutable = options.IsExecutable }
            }
        IncrementalChecker.create tcInitial tcImports tcAcc checkerOptions options.SourceSnapshots
        |> Cancellable.runWithoutCancellation

and [<NoEquality; NoComparison>] CompilationState =
    {
        filePathIndexMap: ImmutableDictionary<string, int>
        caches: CompilationCaches
        options: CompilationOptions
        globalOptions: CompilationGlobalOptions
        asyncLazyGetChecker: AsyncLazy<IncrementalChecker>
    }

    static member Create (options, globalOptions, caches: CompilationCaches) =
        let sourceSnapshots = options.SourceSnapshots

        let filePathIndexMapBuilder = ImmutableDictionary.CreateBuilder (StringComparer.OrdinalIgnoreCase)
        for i = 0 to sourceSnapshots.Length - 1 do
            let sourceSnapshot = sourceSnapshots.[i]
            if filePathIndexMapBuilder.ContainsKey sourceSnapshot.FilePath then
                failwithf "Duplicate file path when creating a compilation. File path: %s" sourceSnapshot.FilePath
            else
                filePathIndexMapBuilder.Add (sourceSnapshot.FilePath, i)

        let asyncLazyGetChecker =
            AsyncLazy (async {
                return! CompilationWorker.EnqueueAndAwaitAsync (fun ctok -> 
                    options.CreateIncrementalChecker (caches.frameworkTcImportsCache, globalOptions, ctok)
                )
            })

        {
            filePathIndexMap = filePathIndexMapBuilder.ToImmutableDictionary ()
            caches = caches
            options = options
            globalOptions = globalOptions
            asyncLazyGetChecker = asyncLazyGetChecker
        }

    member this.SetOptions options =
        CompilationState.Create (options, this.globalOptions, this.caches)

    member this.ReplaceSourceSnapshot (sourceSnapshot: SourceSnapshot) =
        match this.filePathIndexMap.TryGetValue sourceSnapshot.FilePath with
        | false, _ -> failwith "source snapshot does not exist in compilation"
        | _, filePathIndex ->

            let sourceSnapshotsBuilder = this.options.SourceSnapshots.ToBuilder ()

            sourceSnapshotsBuilder.[filePathIndex] <- sourceSnapshot

            let options = { this.options with SourceSnapshots = sourceSnapshotsBuilder.MoveToImmutable () }

            let asyncLazyGetChecker =
                AsyncLazy (async {
                    let! checker = this.asyncLazyGetChecker.GetValueAsync ()
                    return checker.ReplaceSourceSnapshot sourceSnapshot
                })

            { this with options = options; asyncLazyGetChecker = asyncLazyGetChecker }

and [<Sealed>] Compilation (id: CompilationId, state: CompilationState, version: VersionStamp) =

    let asyncLazyGetChecker =
        AsyncLazy (async {
            match state.caches.incrementalCheckerCache.TryGetValue struct (id, version) with
            | ValueSome checker -> return checker
            | _ -> 
                let! checker = state.asyncLazyGetChecker.GetValueAsync ()
                state.caches.incrementalCheckerCache.Set (struct (id, version), checker)
                return checker
        })

    let checkFilePath filePath =
        if not (state.filePathIndexMap.ContainsKey filePath) then
            failwithf "File path does not exist in compilation. File path: %s" filePath

    member __.Id = id

    member __.Version = version

    member __.Options = state.options

    member __.SetOptions (options: CompilationOptions) =
        Compilation (id, state.SetOptions options, version.NewVersionStamp ())

    member __.ReplaceSourceSnapshot (sourceSnapshot: SourceSnapshot) =
        checkFilePath sourceSnapshot.FilePath
        Compilation (id, state.ReplaceSourceSnapshot sourceSnapshot, version.NewVersionStamp ())

    member __.GetSemanticModel filePath =
        checkFilePath filePath
        SemanticModel (filePath, asyncLazyGetChecker)

    member __.GetSyntaxTree filePath =
        checkFilePath filePath
        // Note: Getting a syntax tree requires that the checker be built. This is due to the tcConfig dependency when parsing a syntax tree.
        //       When parsing does not require tcConfig, we can build the syntax trees in Compilation and pass them directly to the checker when it gets built.
        // TODO: Remove this when we fix the tcConfig dependency on parsing.
        Async.RunSynchronously (async {
            let! checker = state.asyncLazyGetChecker.GetValueAsync ()
            return checker.GetSyntaxTree filePath
        })

    member __.PreEmitAsync () =
        // TODO: Maybe break this out into separate functions? Compilation is responsible though.
        async {
            let! checker = asyncLazyGetChecker.GetValueAsync ()
            let! tcStates = checker.FinishAsync ()
            let tcConfig = checker.TcInitial.tcConfig
            let tcGlobals = checker.TcInitial.tcGlobals
            let assemblyName = checker.TcInitial.assemblyName
            let outfile = checker.TcInitial.outfile            

            let errorLogger = CompilationErrorLogger("PreEmitAsync", tcConfig.errorSeverityOptions)
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
                    tcErrorsRev = errorLogger.GetErrors() :: finalAcc.tcErrorsRev 
                    topAttribs = Some topAttrs
                }
            return ilAssemRef, tcAssemblyDataOpt, tcAssemblyExprOpt, finalAccWithErrors
        }

module Compilation =

    let create options globalOptions caches =
        Compilation (CompilationId.Create (), CompilationState.Create (options, globalOptions, caches), VersionStamp.Create ())
