// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.SourceCodeServices

open System
open System.Collections.Concurrent
open System.Diagnostics
open System.IO
open System.Reflection

open FSharp.Compiler 
open FSharp.Compiler.AbstractIL
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILBinaryReader
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.AbstractIL.Internal.Utils
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.CompilerDiagnostics
open FSharp.Compiler.CompilerImports
open FSharp.Compiler.CompilerOptions
open FSharp.Compiler.Driver
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.Lib
open FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Range
open FSharp.Compiler.ScriptClosure
open FSharp.Compiler.SyntaxTree
open FSharp.Compiler.TcGlobals 

open Microsoft.DotNet.DependencyManager

open Internal.Utilities
open Internal.Utilities.Collections

[<AutoOpen>]
module EnvMisc =
    let braceMatchCacheSize = GetEnvInteger "FCS_BraceMatchCacheSize" 5
    let parseFileCacheSize = GetEnvInteger "FCS_ParseFileCacheSize" 2
    let checkFileInProjectCacheSize = GetEnvInteger "FCS_CheckFileInProjectCacheSize" 10

    let projectCacheSizeDefault   = GetEnvInteger "FCS_ProjectCacheSizeDefault" 3
    let frameworkTcImportsCacheStrongSize = GetEnvInteger "FCS_frameworkTcImportsCacheStrongSizeDefault" 8
    let maxMBDefault =  GetEnvInteger "FCS_MaxMB" 1000000 // a million MB = 1TB = disabled
    //let maxMBDefault = GetEnvInteger "FCS_maxMB" (if sizeof<int> = 4 then 1700 else 3400)

type FSharpUnresolvedReferencesSet = FSharpUnresolvedReferencesSet of UnresolvedAssemblyReference list

// NOTE: may be better just to move to optional arguments here
type FSharpProjectOptions =
    { 
      ProjectFileName: string
      ProjectId: string option
      SourceFiles: string[]
      OtherOptions: string[]
      ReferencedProjects: (string * FSharpProjectOptions)[]
      IsIncompleteTypeCheckEnvironment : bool
      UseScriptResolutionRules : bool      
      LoadTime : System.DateTime
      UnresolvedReferences : FSharpUnresolvedReferencesSet option
      OriginalLoadReferences: (range * string * string) list
      ExtraProjectInfo : obj option
      Stamp : int64 option
    }
    member x.ProjectOptions = x.OtherOptions
    /// Whether the two parse options refer to the same project.
    static member UseSameProject(options1,options2) =
        match options1.ProjectId, options2.ProjectId with
        | Some(projectId1), Some(projectId2) when not (String.IsNullOrWhiteSpace(projectId1)) && not (String.IsNullOrWhiteSpace(projectId2)) -> 
            projectId1 = projectId2
        | Some(_), Some(_)
        | None, None -> options1.ProjectFileName = options2.ProjectFileName
        | _ -> false

    /// Compare two options sets with respect to the parts of the options that are important to building.
    static member AreSameForChecking(options1,options2) =
        match options1.Stamp, options2.Stamp with 
        | Some x, Some y -> (x = y)
        | _ -> 
        FSharpProjectOptions.UseSameProject(options1, options2) &&
        options1.SourceFiles = options2.SourceFiles &&
        options1.OtherOptions = options2.OtherOptions &&
        options1.UnresolvedReferences = options2.UnresolvedReferences &&
        options1.OriginalLoadReferences = options2.OriginalLoadReferences &&
        options1.ReferencedProjects.Length = options2.ReferencedProjects.Length &&
        Array.forall2 (fun (n1,a) (n2,b) ->
            n1 = n2 && 
            FSharpProjectOptions.AreSameForChecking(a,b)) options1.ReferencedProjects options2.ReferencedProjects &&
        options1.LoadTime = options2.LoadTime

    /// Compute the project directory.
    member po.ProjectDirectory = System.IO.Path.GetDirectoryName(po.ProjectFileName)
    override this.ToString() = "FSharpProjectOptions(" + this.ProjectFileName + ")"
 
//----------------------------------------------------------------------------
// BackgroundCompiler
//

/// Callback that indicates whether a requested result has become obsolete.    
[<NoComparison;NoEquality>]
type IsResultObsolete = 
    | IsResultObsolete of (unit->bool)


[<AutoOpen>]
module Helpers = 

    /// Determine whether two (fileName,options) keys are identical w.r.t. affect on checking
    let AreSameForChecking2((fileName1: string, options1: FSharpProjectOptions), (fileName2, options2)) =
        (fileName1 = fileName2) 
        && FSharpProjectOptions.AreSameForChecking(options1,options2)
        
    /// Determine whether two (fileName,options) keys should be identical w.r.t. resource usage
    let AreSubsumable2((fileName1:string,o1:FSharpProjectOptions),(fileName2:string,o2:FSharpProjectOptions)) =
        (fileName1 = fileName2)
        && FSharpProjectOptions.UseSameProject(o1,o2)

    /// Determine whether two (fileName,sourceText,options) keys should be identical w.r.t. parsing
    let AreSameForParsing((fileName1: string, source1Hash: int, options1), (fileName2, source2Hash, options2)) =
        fileName1 = fileName2 && options1 = options2 && source1Hash = source2Hash

    let AreSimilarForParsing((fileName1, _, _), (fileName2, _, _)) =
        fileName1 = fileName2
        
    /// Determine whether two (fileName,sourceText,options) keys should be identical w.r.t. checking
    let AreSameForChecking3((fileName1: string, source1Hash: int, options1: FSharpProjectOptions), (fileName2, source2Hash, options2)) =
        (fileName1 = fileName2) 
        && FSharpProjectOptions.AreSameForChecking(options1,options2)
        && source1Hash = source2Hash

    /// Determine whether two (fileName,sourceText,options) keys should be identical w.r.t. resource usage
    let AreSubsumable3((fileName1:string,_,o1:FSharpProjectOptions),(fileName2:string,_,o2:FSharpProjectOptions)) =
        (fileName1 = fileName2)
        && FSharpProjectOptions.UseSameProject(o1,o2)

module CompileHelpers =
    let mkCompilationErrorHandlers() = 
        let errors = ResizeArray<_>()

        let errorSink isError exn = 
            let mainError, relatedErrors = SplitRelatedDiagnostics exn
            let oneError e = errors.Add(FSharpDiagnostic.CreateFromException (e, isError, Range.range0, true)) // Suggest names for errors
            oneError mainError
            List.iter oneError relatedErrors

        let errorLogger = 
            { new ErrorLogger("CompileAPI") with 
                member x.DiagnosticSink(exn, isError) = errorSink isError exn
                member x.ErrorCount = errors |> Seq.filter (fun e -> e.Severity = FSharpDiagnosticSeverity.Error) |> Seq.length }

        let loggerProvider = 
            { new ErrorLoggerProvider() with 
                member x.CreateErrorLoggerUpToMaxErrors(_tcConfigBuilder, _exiter) = errorLogger    }
        errors, errorLogger, loggerProvider

    let tryCompile errorLogger f = 
        use unwindParsePhase = PushThreadBuildPhaseUntilUnwind (BuildPhase.Parse)            
        use unwindEL_2 = PushErrorLoggerPhaseUntilUnwind (fun _ -> errorLogger)
        let exiter = { new Exiter with member x.Exit n = raise StopProcessing }
        try 
            f exiter
            0
        with e -> 
            stopProcessingRecovery e Range.range0
            1

    /// Compile using the given flags.  Source files names are resolved via the FileSystem API. The output file must be given by a -o flag. 
    let compileFromArgs (ctok, argv: string[], legacyReferenceResolver, tcImportsCapture, dynamicAssemblyCreator)  = 
    
        let errors, errorLogger, loggerProvider = mkCompilationErrorHandlers()
        let result = 
            tryCompile errorLogger (fun exiter -> 
                mainCompile (ctok, argv, legacyReferenceResolver, (*bannerAlreadyPrinted*)true, ReduceMemoryFlag.Yes, CopyFSharpCoreFlag.No, exiter, loggerProvider, tcImportsCapture, dynamicAssemblyCreator) )
    
        errors.ToArray(), result

    let compileFromAsts (ctok, legacyReferenceResolver, asts, assemblyName, outFile, dependencies, noframework, pdbFile, executable, tcImportsCapture, dynamicAssemblyCreator) =

        let errors, errorLogger, loggerProvider = mkCompilationErrorHandlers()
    
        let executable = defaultArg executable true
        let target = if executable then CompilerTarget.ConsoleExe else CompilerTarget.Dll
    
        let result = 
            tryCompile errorLogger (fun exiter -> 
                compileOfAst (ctok, legacyReferenceResolver, ReduceMemoryFlag.Yes, assemblyName, target, outFile, pdbFile, dependencies, noframework, exiter, loggerProvider, asts, tcImportsCapture, dynamicAssemblyCreator))

        errors.ToArray(), result

    let createDynamicAssembly (ctok, debugInfo: bool, tcImportsRef: TcImports option ref, execute: bool, assemblyBuilderRef: _ option ref) (tcConfig: TcConfig, tcGlobals:TcGlobals, outfile, ilxMainModule) =

        // Create an assembly builder
        let assemblyName = System.Reflection.AssemblyName(System.IO.Path.GetFileNameWithoutExtension outfile)
        let flags = System.Reflection.Emit.AssemblyBuilderAccess.Run
#if FX_NO_APP_DOMAINS
        let assemblyBuilder = System.Reflection.Emit.AssemblyBuilder.DefineDynamicAssembly(assemblyName, flags)
        let moduleBuilder = assemblyBuilder.DefineDynamicModule("IncrementalModule")
#else
        let assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, flags)
        let moduleBuilder = assemblyBuilder.DefineDynamicModule("IncrementalModule", debugInfo)
#endif            
        // Omit resources in dynamic assemblies, because the module builder is constructed without a filename the module 
        // is tagged as transient and as such DefineManifestResource will throw an invalid operation if resources are present.
        // 
        // Also, the dynamic assembly creator can't currently handle types called "<Module>" from statically linked assemblies.
        let ilxMainModule = 
            { ilxMainModule with 
                TypeDefs = ilxMainModule.TypeDefs.AsList |> List.filter (fun td -> not (isTypeNameForGlobalFunctions td.Name)) |> mkILTypeDefs
                Resources=mkILResources [] }

        // The function used to resolve types while emitting the code
        let assemblyResolver s = 
            match tcImportsRef.Value.Value.TryFindExistingFullyQualifiedPathByExactAssemblyRef (ctok, s) with 
            | Some res -> Some (Choice1Of2 res)
            | None -> None

        // Emit the code
        let _emEnv,execs = ILRuntimeWriter.emitModuleFragment(tcGlobals.ilg, tcConfig.emitTailcalls, ILRuntimeWriter.emEnv0, assemblyBuilder, moduleBuilder, ilxMainModule, debugInfo, assemblyResolver, tcGlobals.TryFindSysILTypeRef)

        // Execute the top-level initialization, if requested
        if execute then 
            for exec in execs do 
                match exec() with 
                | None -> ()
                | Some exn -> 
                    PreserveStackTrace(exn)
                    raise exn

        // Register the reflected definitions for the dynamically generated assembly
        for resource in ilxMainModule.Resources.AsList do 
            if IsReflectedDefinitionsResource resource then 
                Quotations.Expr.RegisterReflectedDefinitions (assemblyBuilder, moduleBuilder.Name, resource.GetBytes().ToArray())

        // Save the result
        assemblyBuilderRef := Some assemblyBuilder
        
    let setOutputStreams execute = 
        // Set the output streams, if requested
        match execute with
        | Some (writer,error) -> 
            System.Console.SetOut writer
            System.Console.SetError error
        | None -> ()

type SourceTextHash = int        
type FileName = string      
type FilePath = string
type ProjectPath = string
type FileVersion = int

type ParseCacheLockToken() = interface LockToken
type ScriptClosureCacheToken() = interface LockToken


// There is only one instance of this type, held in FSharpChecker
type BackgroundCompiler(legacyReferenceResolver, projectCacheSize, keepAssemblyContents, keepAllBackgroundResolutions, tryGetMetadataSnapshot, suggestNamesForErrors, keepAllBackgroundSymbolUses, enableBackgroundItemKeyStoreAndSemanticClassification, enablePartialTypeChecking) as self =
    // STATIC ROOT: FSharpLanguageServiceTestable.FSharpChecker.backgroundCompiler.reactor: The one and only Reactor
    let reactor = Reactor.Singleton
    let beforeFileChecked = Event<string * obj option>()
    let fileParsed = Event<string * obj option>()
    let fileChecked = Event<string * obj option>()
    let projectChecked = Event<string * obj option>()


    let mutable implicitlyStartBackgroundWork = true
    let reactorOps = 
        { new IReactorOperations with 
                member _.EnqueueAndAwaitOpAsync (userOpName, opName, opArg, op) = reactor.EnqueueAndAwaitOpAsync (userOpName, opName, opArg, op)
                member _.EnqueueOp (userOpName, opName, opArg, op) = reactor.EnqueueOp (userOpName, opName, opArg, op) }

    // STATIC ROOT: FSharpLanguageServiceTestable.FSharpChecker.backgroundCompiler.scriptClosureCache 
    /// Information about the derived script closure.
    let scriptClosureCache = 
        MruCache<AnyCallerThreadToken, FSharpProjectOptions, LoadClosure>(projectCacheSize, 
            areSame=FSharpProjectOptions.AreSameForChecking, 
            areSimilar=FSharpProjectOptions.UseSameProject)

    let frameworkTcImportsCache = FrameworkImportsCache(frameworkTcImportsCacheStrongSize)

    // We currently share one global dependency provider for all scripts for the FSharpChecker.
    // For projects, one is used per project.
    // 
    // Sharing one for all scripts is necessary for good performance from GetProjectOptionsFromScript,
    // which requires a dependency provider to process through the project options prior to working out
    // if the cached incremental builder can be used for the project.
    let dependencyProviderForScripts = new DependencyProvider()

    /// CreateOneIncrementalBuilder (for background type checking). Note that fsc.fs also
    /// creates an incremental builder used by the command line compiler.
    let CreateOneIncrementalBuilder (ctok, options:FSharpProjectOptions, userOpName) = 
      cancellable {
        Trace.TraceInformation("FCS: {0}.{1} ({2})", userOpName, "CreateOneIncrementalBuilder", options.ProjectFileName)
        let projectReferences =  
            [ for (nm,opts) in options.ReferencedProjects do
               
               // Don't use cross-project references for FSharp.Core, since various bits of code require a concrete FSharp.Core to exist on-disk.
               // The only solutions that have these cross-project references to FSharp.Core are VisualFSharp.sln and FSharp.sln. The only ramification
               // of this is that you need to build FSharp.Core to get intellisense in those projects.

               if (try Path.GetFileNameWithoutExtension(nm) with _ -> "") <> GetFSharpCoreLibraryName() then

                 yield
                    { new IProjectReference with 
                        member x.EvaluateRawContents(ctok) = 
                          cancellable {
                            Trace.TraceInformation("FCS: {0}.{1} ({2})", userOpName, "GetAssemblyData", nm)
                            return! self.GetAssemblyData(opts, ctok, userOpName + ".CheckReferencedProject("+nm+")")
                          }
                        member x.TryGetLogicalTimeStamp(cache, ctok) = 
                            self.TryGetLogicalTimeStampForProject(cache, ctok, opts, userOpName + ".TimeStampReferencedProject("+nm+")")
                        member x.FileName = nm } ]

        let loadClosure = scriptClosureCache.TryGet(AnyCallerThread, options)

        let! builderOpt, diagnostics = 
            IncrementalBuilder.TryCreateIncrementalBuilderForProjectOptions
                  (ctok, legacyReferenceResolver, FSharpCheckerResultsSettings.defaultFSharpBinariesDir, frameworkTcImportsCache, loadClosure, Array.toList options.SourceFiles, 
                   Array.toList options.OtherOptions, projectReferences, options.ProjectDirectory, 
                   options.UseScriptResolutionRules, keepAssemblyContents, keepAllBackgroundResolutions, FSharpCheckerResultsSettings.maxTimeShareMilliseconds,
                   tryGetMetadataSnapshot, suggestNamesForErrors, keepAllBackgroundSymbolUses,
                   enableBackgroundItemKeyStoreAndSemanticClassification,
                   enablePartialTypeChecking,
                   (if options.UseScriptResolutionRules then Some dependencyProviderForScripts else None))

        match builderOpt with 
        | None -> ()
        | Some builder -> 

#if !NO_EXTENSIONTYPING
            // Register the behaviour that responds to CCUs being invalidated because of type
            // provider Invalidate events. This invalidates the configuration in the build.
            builder.ImportsInvalidatedByTypeProvider.Add (fun _ -> 
                self.InvalidateConfiguration(options, None, userOpName))
#endif

            // Register the callback called just before a file is typechecked by the background builder (without recording
            // errors or intellisense information).
            //
            // This indicates to the UI that the file type check state is dirty. If the file is open and visible then 
            // the UI will sooner or later request a typecheck of the file, recording errors and intellisense information.
            builder.BeforeFileChecked.Add (fun file -> beforeFileChecked.Trigger(file, options.ExtraProjectInfo))
            builder.FileParsed.Add (fun file -> fileParsed.Trigger(file, options.ExtraProjectInfo))
            builder.FileChecked.Add (fun file -> fileChecked.Trigger(file, options.ExtraProjectInfo))
            builder.ProjectChecked.Add (fun () -> projectChecked.Trigger (options.ProjectFileName, options.ExtraProjectInfo))

        return (builderOpt, diagnostics)
      }

    // STATIC ROOT: FSharpLanguageServiceTestable.FSharpChecker.backgroundCompiler.incrementalBuildersCache. This root typically holds more 
    // live information than anything else in the F# Language Service, since it holds up to 3 (projectCacheStrongSize) background project builds
    // strongly.
    // 
    /// Cache of builds keyed by options.        
    let incrementalBuildersCache = 
        MruCache<AnyCallerThreadToken, FSharpProjectOptions, (IncrementalBuilder option * FSharpDiagnostic[])>
                (keepStrongly=projectCacheSize, keepMax=projectCacheSize, 
                 areSame =  FSharpProjectOptions.AreSameForChecking, 
                 areSimilar =  FSharpProjectOptions.UseSameProject)

    let tryGetBuilder options =
        incrementalBuildersCache.TryGet (AnyCallerThread, options)

    let tryGetSimilarBuilder options =
        incrementalBuildersCache.TryGetSimilar (AnyCallerThread, options)

    let tryGetAnyBuilder options =
        incrementalBuildersCache.TryGetAny (AnyCallerThread, options)

    let getOrCreateBuilder (ctok, options, userOpName) =
      cancellable {
          match tryGetBuilder options with
          | Some (builderOpt,creationErrors) -> 
              Logger.Log LogCompilerFunctionId.Service_IncrementalBuildersCache_GettingCache
              return builderOpt,creationErrors
          | None -> 
              Logger.Log LogCompilerFunctionId.Service_IncrementalBuildersCache_BuildingNewCache
              let! (builderOpt,creationErrors) as info = CreateOneIncrementalBuilder (ctok, options, userOpName)
              incrementalBuildersCache.Set (AnyCallerThread, options, info)
              return builderOpt, creationErrors
      }

    let getSimilarOrCreateBuilder (ctok, options, userOpName) =
        RequireCompilationThread ctok
        match tryGetSimilarBuilder options with
        | Some res -> Cancellable.ret res
        // The builder does not exist at all. Create it.
        | None -> getOrCreateBuilder (ctok, options, userOpName)

    let getOrCreateBuilderWithInvalidationFlag (ctok, options, canInvalidateProject, userOpName) =
        if canInvalidateProject then
            getOrCreateBuilder (ctok, options, userOpName)
        else
            getSimilarOrCreateBuilder (ctok, options, userOpName)

    let parseCacheLock = Lock<ParseCacheLockToken>()
    
    // STATIC ROOT: FSharpLanguageServiceTestable.FSharpChecker.parseFileInProjectCache. Most recently used cache for parsing files.
    let parseFileCache = MruCache<ParseCacheLockToken,_,_>(parseFileCacheSize, areSimilar = AreSimilarForParsing, areSame = AreSameForParsing)

    // STATIC ROOT: FSharpLanguageServiceTestable.FSharpChecker.checkFileInProjectCachePossiblyStale 
    // STATIC ROOT: FSharpLanguageServiceTestable.FSharpChecker.checkFileInProjectCache
    //
    /// Cache which holds recently seen type-checks.
    /// This cache may hold out-of-date entries, in two senses
    ///    - there may be a more recent antecedent state available because the background build has made it available
    ///    - the source for the file may have changed
    
    let checkFileInProjectCachePossiblyStale = 
        MruCache<ParseCacheLockToken,string * FSharpProjectOptions, FSharpParseFileResults * FSharpCheckFileResults * int>
            (keepStrongly=checkFileInProjectCacheSize,
             areSame=AreSameForChecking2,
             areSimilar=AreSubsumable2)

    // Also keyed on source. This can only be out of date if the antecedent is out of date
    let checkFileInProjectCache = 
        MruCache<ParseCacheLockToken,FileName * SourceTextHash * FSharpProjectOptions, FSharpParseFileResults * FSharpCheckFileResults * FileVersion * DateTime>
            (keepStrongly=checkFileInProjectCacheSize,
             areSame=AreSameForChecking3,
             areSimilar=AreSubsumable3)

    /// Holds keys for files being currently checked. It's used to prevent checking same file in parallel (interleaving chunk queued to Reactor).
    let beingCheckedFileTable = 
        ConcurrentDictionary<FilePath * FSharpProjectOptions * FileVersion, unit>
            (HashIdentity.FromFunctions
                hash
                (fun (f1, o1, v1) (f2, o2, v2) -> f1 = f2 && v1 = v2 && FSharpProjectOptions.AreSameForChecking(o1, o2)))

    static let mutable foregroundParseCount = 0

    static let mutable foregroundTypeCheckCount = 0

    member _.RecordTypeCheckFileInProjectResults(filename,options,parsingOptions,parseResults,fileVersion,priorTimeStamp,checkAnswer,sourceText) =        
        match checkAnswer with 
        | None
        | Some FSharpCheckFileAnswer.Aborted -> ()
        | Some (FSharpCheckFileAnswer.Succeeded typedResults) -> 
            foregroundTypeCheckCount <- foregroundTypeCheckCount + 1
            parseCacheLock.AcquireLock (fun ltok -> 
                checkFileInProjectCachePossiblyStale.Set(ltok, (filename,options),(parseResults,typedResults,fileVersion))  
                checkFileInProjectCache.Set(ltok, (filename, sourceText, options),(parseResults,typedResults,fileVersion,priorTimeStamp))
                parseFileCache.Set(ltok, (filename, sourceText, parsingOptions), parseResults))

    member bc.ImplicitlyStartCheckProjectInBackground(options, userOpName) =        
        if implicitlyStartBackgroundWork then 
            bc.CheckProjectInBackground(options, userOpName + ".ImplicitlyStartCheckProjectInBackground")

    member _.ParseFile(filename: string, sourceText: ISourceText, options: FSharpParsingOptions, userOpName: string) =
        async {
            let hash = sourceText.GetHashCode()
            match parseCacheLock.AcquireLock(fun ltok -> parseFileCache.TryGet(ltok, (filename, hash, options))) with
            | Some res -> return res
            | None ->
                foregroundParseCount <- foregroundParseCount + 1
                let parseErrors, parseTreeOpt, anyErrors = ParseAndCheckFile.parseFile(sourceText, filename, options, userOpName, suggestNamesForErrors)
                let res = FSharpParseFileResults(parseErrors, parseTreeOpt, anyErrors, options.SourceFiles)
                parseCacheLock.AcquireLock(fun ltok -> parseFileCache.Set(ltok, (filename, hash, options), res))
                return res
        }

    member bc.ParseFileNoCache(filename, sourceText, options, userOpName) =
        async {
            let parseErrors, parseTreeOpt, anyErrors = ParseAndCheckFile.parseFile(sourceText, filename, options, userOpName, false)
            return FSharpParseFileResults(parseErrors, parseTreeOpt, anyErrors, options.SourceFiles)
        }

    /// Fetch the parse information from the background compiler (which checks w.r.t. the FileSystem API)
    member _.GetBackgroundParseResultsForFileInProject(filename, options, userOpName) =
        reactor.EnqueueAndAwaitOpAsync(userOpName, "GetBackgroundParseResultsForFileInProject ", filename, fun ctok -> 
            cancellable {
                let! builderOpt, creationErrors = getOrCreateBuilder (ctok, options, userOpName)
                match builderOpt with
                | None -> return FSharpParseFileResults(creationErrors, None, true, [| |])
                | Some builder -> 
                    let! parseTreeOpt,_,_,parseErrors = builder.GetParseResultsForFile (ctok, filename)
                    let errors = [| yield! creationErrors; yield! ErrorHelpers.CreateErrorInfos (builder.TcConfig.errorSeverityOptions, false, filename, parseErrors, suggestNamesForErrors) |]
                    return FSharpParseFileResults(errors = errors, input = parseTreeOpt, parseHadErrors = false, dependencyFiles = builder.AllDependenciesDeprecated)
            }
        )

    member _.GetCachedCheckFileResult(builder: IncrementalBuilder, filename, sourceText: ISourceText, options) =
        // Check the cache. We can only use cached results when there is no work to do to bring the background builder up-to-date
        let cachedResults = parseCacheLock.AcquireLock (fun ltok -> checkFileInProjectCache.TryGet(ltok, (filename, sourceText.GetHashCode(), options)))

        match cachedResults with 
//            | Some (parseResults, checkResults, _, _) when builder.AreCheckResultsBeforeFileInProjectReady(filename) -> 
        | Some (parseResults, checkResults,_,priorTimeStamp) 
                when 
                (match builder.GetCheckResultsBeforeFileInProjectEvenIfStale filename with 
                | None -> false
                | Some(tcPrior) -> 
                    tcPrior.TimeStamp = priorTimeStamp &&
                    builder.AreCheckResultsBeforeFileInProjectReady(filename)) -> 
            Some (parseResults,checkResults)
        | _ -> None

    /// 1. Repeatedly try to get cached file check results or get file "lock". 
    /// 
    /// 2. If it've got cached results, returns them.
    ///
    /// 3. If it've not got the lock for 1 minute, returns `FSharpCheckFileAnswer.Aborted`.
    ///
    /// 4. Type checks the file.
    ///
    /// 5. Records results in `BackgroundCompiler` caches.
    ///
    /// 6. Starts whole project background compilation.
    ///
    /// 7. Releases the file "lock".
    member private bc.CheckOneFileImpl
        (parseResults: FSharpParseFileResults,
         sourceText: ISourceText,
         fileName: string,
         options: FSharpProjectOptions,
         fileVersion: int,
         builder: IncrementalBuilder,
         tcConfig,
         tcGlobals,
         tcImports,
         tcDependencyFiles,
         timeStamp,
         prevTcState,
         prevModuleNamesDict,
         prevTcErrors,
         creationErrors: FSharpDiagnostic[],
         userOpName: string) = 
    
        async {
            let beingCheckedFileKey = fileName, options, fileVersion
            let stopwatch = Stopwatch.StartNew()
            let rec loop() =
                async {
                    // results may appear while we were waiting for the lock, let's recheck if it's the case
                    let cachedResults = bc.GetCachedCheckFileResult(builder, fileName, sourceText, options) 
            
                    match cachedResults with
                    | Some (_, checkResults) -> return FSharpCheckFileAnswer.Succeeded checkResults
                    | None ->
                        if beingCheckedFileTable.TryAdd(beingCheckedFileKey, ()) then
                            try
                                // Get additional script #load closure information if applicable.
                                // For scripts, this will have been recorded by GetProjectOptionsFromScript.
                                let loadClosure = scriptClosureCache.TryGet(AnyCallerThread, options)
                                let! checkAnswer = 
                                    FSharpCheckFileResults.CheckOneFile
                                        (parseResults,
                                            sourceText,
                                            fileName,
                                            options.ProjectFileName, 
                                            tcConfig,
                                            tcGlobals,
                                            tcImports, 
                                            prevTcState,
                                            prevModuleNamesDict,
                                            loadClosure,
                                            prevTcErrors,
                                            reactorOps, 
                                            userOpName,
                                            options.IsIncompleteTypeCheckEnvironment, 
                                            builder, 
                                            Array.ofList tcDependencyFiles, 
                                            creationErrors, 
                                            parseResults.Errors, 
                                            keepAssemblyContents,
                                            suggestNamesForErrors)
                                let parsingOptions = FSharpParsingOptions.FromTcConfig(tcConfig, Array.ofList builder.SourceFiles, options.UseScriptResolutionRules)
                                reactor.SetPreferredUILang tcConfig.preferredUiLang
                                bc.RecordTypeCheckFileInProjectResults(fileName, options, parsingOptions, parseResults, fileVersion, timeStamp, Some checkAnswer, sourceText.GetHashCode()) 
                                return checkAnswer
                            finally
                                let dummy = ref ()
                                beingCheckedFileTable.TryRemove(beingCheckedFileKey, dummy) |> ignore
                        else 
                            do! Async.Sleep 100
                            if stopwatch.Elapsed > TimeSpan.FromMinutes 1. then 
                                return FSharpCheckFileAnswer.Aborted
                            else
                                return! loop()
                }
            return! loop()
        }

    /// Type-check the result obtained by parsing, but only if the antecedent type checking context is available. 
    member bc.CheckFileInProjectAllowingStaleCachedResults(parseResults: FSharpParseFileResults, filename, fileVersion, sourceText: ISourceText, options, userOpName) =
        let execWithReactorAsync action = reactor.EnqueueAndAwaitOpAsync(userOpName, "CheckFileInProjectAllowingStaleCachedResults ", filename, action)
        async {
            try
                if implicitlyStartBackgroundWork then 
                    reactor.CancelBackgroundOp() // cancel the background work, since we will start new work after we're done

                let! cachedResults = 
                  execWithReactorAsync <| fun ctok ->   
                   cancellable {
                    let! _builderOpt,_creationErrors = getOrCreateBuilder (ctok, options, userOpName)

                    match tryGetAnyBuilder options with
                    | Some (Some builder, creationErrors) ->
                        match bc.GetCachedCheckFileResult(builder, filename, sourceText, options) with
                        | Some (_, checkResults) -> return Some (builder, creationErrors, Some (FSharpCheckFileAnswer.Succeeded checkResults))
                        | _ -> return Some (builder, creationErrors, None)
                    | _ -> return None // the builder wasn't ready
                   }
                        
                match cachedResults with
                | None -> return None
                | Some (_, _, Some x) -> return Some x
                | Some (builder, creationErrors, None) ->
                    Trace.TraceInformation("FCS: {0}.{1} ({2})", userOpName, "CheckFileInProjectAllowingStaleCachedResults.CacheMiss", filename)
                    let! tcPrior = 
                        execWithReactorAsync <| fun ctok -> 
                          cancellable {
                            DoesNotRequireCompilerThreadTokenAndCouldPossiblyBeMadeConcurrent  ctok
                            let tcPrior = builder.GetCheckResultsBeforeFileInProjectEvenIfStale filename
                            return
                                tcPrior
                                |> Option.map (fun tcPrior ->
                                    (tcPrior, tcPrior.TcInfo ctok)
                                )
                          }
                            
                    match tcPrior with
                    | Some(tcPrior, tcInfo) -> 
                        let! checkResults = bc.CheckOneFileImpl(parseResults, sourceText, filename, options, fileVersion, builder, tcPrior.TcConfig, tcPrior.TcGlobals, tcPrior.TcImports, tcInfo.tcDependencyFiles, tcPrior.TimeStamp, tcInfo.tcState, tcInfo.moduleNamesDict, tcInfo.TcErrors, creationErrors, userOpName)
                        return Some checkResults
                    | None -> return None  // the incremental builder was not up to date
            finally 
                bc.ImplicitlyStartCheckProjectInBackground(options, userOpName)
        }

    /// Type-check the result obtained by parsing. Force the evaluation of the antecedent type checking context if needed.
    member bc.CheckFileInProject(parseResults: FSharpParseFileResults, filename, fileVersion, sourceText: ISourceText, options, userOpName) =
        let execWithReactorAsync action = reactor.EnqueueAndAwaitOpAsync(userOpName, "CheckFileInProject", filename, action)
        async {
            try 
                if implicitlyStartBackgroundWork then 
                    reactor.CancelBackgroundOp() // cancel the background work, since we will start new work after we're done
                let! builderOpt,creationErrors = execWithReactorAsync (fun ctok -> getOrCreateBuilder (ctok, options, userOpName))
                match builderOpt with
                | None -> return FSharpCheckFileAnswer.Succeeded (FSharpCheckFileResults.MakeEmpty(filename, creationErrors, keepAssemblyContents))
                | Some builder -> 
                    // Check the cache. We can only use cached results when there is no work to do to bring the background builder up-to-date
                    let cachedResults = bc.GetCachedCheckFileResult(builder, filename, sourceText, options)

                    match cachedResults with
                    | Some (_, checkResults) -> return FSharpCheckFileAnswer.Succeeded checkResults
                    | _ ->
                        Trace.TraceInformation("FCS: {0}.{1} ({2})", userOpName, "CheckFileInProject.CacheMiss", filename)
                        let! tcPrior, tcInfo =
                            execWithReactorAsync <| fun ctok -> 
                                cancellable {
                                    let! tcPrior = builder.GetCheckResultsBeforeFileInProject (ctok, filename)
                                    return (tcPrior, tcPrior.TcInfo ctok)
                                }
                        let! checkAnswer = bc.CheckOneFileImpl(parseResults, sourceText, filename, options, fileVersion, builder, tcPrior.TcConfig, tcPrior.TcGlobals, tcPrior.TcImports, tcInfo.tcDependencyFiles, tcPrior.TimeStamp, tcInfo.tcState, tcInfo.moduleNamesDict, tcInfo.TcErrors, creationErrors, userOpName)
                        return checkAnswer
            finally 
                bc.ImplicitlyStartCheckProjectInBackground(options, userOpName)
        }

    /// Parses and checks the source file and returns untyped AST and check results.
    member bc.ParseAndCheckFileInProject (filename:string, fileVersion, sourceText: ISourceText, options:FSharpProjectOptions, userOpName) =
        let execWithReactorAsync action = reactor.EnqueueAndAwaitOpAsync(userOpName, "ParseAndCheckFileInProject", filename, action)
        async {
            try 
                let strGuid = "_ProjectId=" + (options.ProjectId |> Option.defaultValue "null")
                Logger.LogBlockMessageStart (filename + strGuid) LogCompilerFunctionId.Service_ParseAndCheckFileInProject

                if implicitlyStartBackgroundWork then 
                    Logger.LogMessage (filename + strGuid + "-Cancelling background work") LogCompilerFunctionId.Service_ParseAndCheckFileInProject
                    reactor.CancelBackgroundOp() // cancel the background work, since we will start new work after we're done

                let! builderOpt,creationErrors = execWithReactorAsync (fun ctok -> getOrCreateBuilder (ctok, options, userOpName))
                match builderOpt with
                | None -> 
                    Logger.LogBlockMessageStop (filename + strGuid + "-Failed_Aborted") LogCompilerFunctionId.Service_ParseAndCheckFileInProject

                    let parseResults = FSharpParseFileResults(creationErrors, None, true, [| |])
                    return (parseResults, FSharpCheckFileAnswer.Aborted)

                | Some builder -> 
                    let cachedResults = bc.GetCachedCheckFileResult(builder, filename, sourceText, options)

                    match cachedResults with 
                    | Some (parseResults, checkResults) -> 
                        Logger.LogBlockMessageStop (filename + strGuid + "-Successful_Cached") LogCompilerFunctionId.Service_ParseAndCheckFileInProject

                        return parseResults, FSharpCheckFileAnswer.Succeeded checkResults
                    | _ ->
                        // todo this blocks the Reactor queue until all files up to the current are type checked. It's OK while editing the file,
                        // but results with non cooperative blocking when a firts file from a project opened.
                        let! tcPrior, tcInfo =
                            execWithReactorAsync <| fun ctok -> 
                                cancellable {
                                    let! tcPrior = builder.GetCheckResultsBeforeFileInProject (ctok, filename)
                                    return (tcPrior, tcPrior.TcInfo ctok)
                                } 
                    
                        // Do the parsing.
                        let parsingOptions = FSharpParsingOptions.FromTcConfig(builder.TcConfig, Array.ofList (builder.SourceFiles), options.UseScriptResolutionRules)
                        reactor.SetPreferredUILang tcPrior.TcConfig.preferredUiLang
                        let parseErrors, parseTreeOpt, anyErrors = ParseAndCheckFile.parseFile (sourceText, filename, parsingOptions, userOpName, suggestNamesForErrors)
                        let parseResults = FSharpParseFileResults(parseErrors, parseTreeOpt, anyErrors, builder.AllDependenciesDeprecated)
                        let! checkResults = bc.CheckOneFileImpl(parseResults, sourceText, filename, options, fileVersion, builder, tcPrior.TcConfig, tcPrior.TcGlobals, tcPrior.TcImports, tcInfo.tcDependencyFiles, tcPrior.TimeStamp, tcInfo.tcState, tcInfo.moduleNamesDict, tcInfo.TcErrors, creationErrors, userOpName)

                        Logger.LogBlockMessageStop (filename + strGuid + "-Successful") LogCompilerFunctionId.Service_ParseAndCheckFileInProject

                        return parseResults, checkResults
            finally 
                bc.ImplicitlyStartCheckProjectInBackground(options, userOpName)
        }

    /// Fetch the check information from the background compiler (which checks w.r.t. the FileSystem API)
    member _.GetBackgroundCheckResultsForFileInProject(filename, options, userOpName) =
        reactor.EnqueueAndAwaitOpAsync(userOpName, "GetBackgroundCheckResultsForFileInProject", filename, fun ctok -> 
          cancellable {
            let! builderOpt, creationErrors = getOrCreateBuilder (ctok, options, userOpName)
            match builderOpt with
            | None ->
                let parseResults = FSharpParseFileResults(creationErrors, None, true, [| |])
                let typedResults = FSharpCheckFileResults.MakeEmpty(filename, creationErrors, keepAssemblyContents)
                return (parseResults, typedResults)
            | Some builder -> 
                let! (parseTreeOpt, _, _, untypedErrors) = builder.GetParseResultsForFile (ctok, filename)
                let! tcProj = builder.GetFullCheckResultsAfterFileInProject (ctok, filename)

                let tcInfo, tcInfoOptional = tcProj.TcInfoWithOptional ctok

                let tcResolutionsRev = tcInfoOptional.tcResolutionsRev
                let tcSymbolUsesRev = tcInfoOptional.tcSymbolUsesRev
                let tcOpenDeclarationsRev = tcInfoOptional.tcOpenDeclarationsRev
                let latestCcuSigForFile = tcInfo.latestCcuSigForFile
                let tcState = tcInfo.tcState
                let tcEnvAtEnd = tcInfo.tcEnvAtEndOfFile
                let latestImplementationFile = tcInfoOptional.latestImplFile
                let tcDependencyFiles = tcInfo.tcDependencyFiles
                let tcErrors = tcInfo.TcErrors
                let errorOptions = builder.TcConfig.errorSeverityOptions
                let untypedErrors = [| yield! creationErrors; yield! ErrorHelpers.CreateErrorInfos (errorOptions, false, filename, untypedErrors, suggestNamesForErrors) |]
                let tcErrors = [| yield! creationErrors; yield! ErrorHelpers.CreateErrorInfos (errorOptions, false, filename, tcErrors, suggestNamesForErrors) |]
                let parseResults = FSharpParseFileResults(errors = untypedErrors, input = parseTreeOpt, parseHadErrors = false, dependencyFiles = builder.AllDependenciesDeprecated)
                let loadClosure = scriptClosureCache.TryGet(AnyCallerThread, options)
                let typedResults = 
                    FSharpCheckFileResults.Make
                        (filename, 
                            options.ProjectFileName, 
                            tcProj.TcConfig, 
                            tcProj.TcGlobals, 
                            options.IsIncompleteTypeCheckEnvironment, 
                            builder, 
                            Array.ofList tcDependencyFiles, 
                            creationErrors, 
                            parseResults.Errors, 
                            tcErrors,
                            keepAssemblyContents,
                            Option.get latestCcuSigForFile, 
                            tcState.Ccu, 
                            tcProj.TcImports, 
                            tcEnvAtEnd.AccessRights,
                            List.head tcResolutionsRev, 
                            List.head tcSymbolUsesRev,
                            tcEnvAtEnd.NameEnv,
                            loadClosure, 
                            latestImplementationFile,
                            List.head tcOpenDeclarationsRev) 
                return (parseResults, typedResults)
          }
        )

    member _.FindReferencesInFile(filename: string, options: FSharpProjectOptions, symbol: FSharpSymbol, canInvalidateProject: bool, userOpName: string) =
        reactor.EnqueueAndAwaitOpAsync(userOpName, "FindReferencesInFile", filename, fun ctok -> 
            cancellable {
            let! builderOpt, _ = getOrCreateBuilderWithInvalidationFlag (ctok, options, canInvalidateProject, userOpName)
            match builderOpt with
            | None -> return Seq.empty
            | Some builder -> 
                if builder.ContainsFile filename then
                    let! checkResults = builder.GetFullCheckResultsAfterFileInProject (ctok, filename)
                    match checkResults.TryGetItemKeyStore ctok with
                    | None -> return Seq.empty
                    | Some reader -> return reader.FindAll symbol.Item
                else
                    return Seq.empty })


    member _.GetSemanticClassificationForFile(filename: string, options: FSharpProjectOptions, userOpName: string) =
        reactor.EnqueueAndAwaitOpAsync(userOpName, "GetSemanticClassificationForFile", filename, fun ctok -> 
            cancellable {
                let! builderOpt, _ = getOrCreateBuilder (ctok, options, userOpName)
                match builderOpt with
                | None -> return [||]
                | Some builder -> 
                    let! checkResults = builder.GetFullCheckResultsAfterFileInProject (ctok, filename)
                    return checkResults.GetSemanticClassification ctok })

    /// Try to get recent approximate type check results for a file. 
    member _.TryGetRecentCheckResultsForFile(filename: string, options:FSharpProjectOptions, sourceText: ISourceText option, _userOpName: string) =
        parseCacheLock.AcquireLock (fun ltok -> 
            match sourceText with 
            | Some sourceText -> 
                match checkFileInProjectCache.TryGet(ltok,(filename,sourceText.GetHashCode(),options)) with
                | Some (a,b,c,_) -> Some (a,b,c)
                | None -> checkFileInProjectCachePossiblyStale.TryGet(ltok,(filename,options))
            | None -> checkFileInProjectCachePossiblyStale.TryGet(ltok,(filename,options)))

    /// Parse and typecheck the whole project (the implementation, called recursively as project graph is evaluated)
    member private _.ParseAndCheckProjectImpl(options, ctok, userOpName) =
      cancellable {
          let! builderOpt,creationErrors = getOrCreateBuilder (ctok, options, userOpName)
          match builderOpt with 
          | None -> 
              return FSharpCheckProjectResults (options.ProjectFileName, None, keepAssemblyContents, creationErrors, None)
          | Some builder -> 
              let! (tcProj, ilAssemRef, tcAssemblyDataOpt, tcAssemblyExprOpt) = builder.GetFullCheckResultsAndImplementationsForProject(ctok)
              let errorOptions = tcProj.TcConfig.errorSeverityOptions
              let fileName = TcGlobals.DummyFileNameForRangesWithoutASpecificLocation

              let tcInfo, tcInfoOptional = tcProj.TcInfoWithOptional ctok

              let tcSymbolUses = tcInfoOptional.TcSymbolUses
              let topAttribs = tcInfo.topAttribs
              let tcState = tcInfo.tcState
              let tcEnvAtEnd = tcInfo.tcEnvAtEndOfFile
              let tcErrors = tcInfo.TcErrors
              let tcDependencyFiles = tcInfo.tcDependencyFiles
              let errors = [| yield! creationErrors; yield! ErrorHelpers.CreateErrorInfos (errorOptions, true, fileName, tcErrors, suggestNamesForErrors) |]
              return FSharpCheckProjectResults (options.ProjectFileName, Some tcProj.TcConfig, keepAssemblyContents, errors, 
                                                    Some(tcProj.TcGlobals, tcProj.TcImports, tcState.Ccu, tcState.CcuSig, 
                                                            tcSymbolUses, topAttribs, tcAssemblyDataOpt, ilAssemRef, 
                                                            tcEnvAtEnd.AccessRights, tcAssemblyExprOpt, Array.ofList tcDependencyFiles))
      }

    member _.GetAssemblyData(options, ctok, userOpName) =
        cancellable {
            let! builderOpt,_ = getOrCreateBuilder (ctok, options, userOpName)
            match builderOpt with 
            | None -> 
                return None
            | Some builder -> 
                let! (_, _, tcAssemblyDataOpt, _) = builder.GetCheckResultsAndImplementationsForProject(ctok)
                return tcAssemblyDataOpt
        }

    /// Get the timestamp that would be on the output if fully built immediately
    member private _.TryGetLogicalTimeStampForProject(cache, ctok, options, userOpName: string) =

        // NOTE: This creation of the background builder is currently run as uncancellable.  Creating background builders is generally
        // cheap though the timestamp computations look suspicious for transitive project references.
        let builderOpt,_creationErrors = getOrCreateBuilder (ctok, options, userOpName + ".TryGetLogicalTimeStampForProject") |> Cancellable.runWithoutCancellation
        match builderOpt with 
        | None -> None
        | Some builder -> Some (builder.GetLogicalTimeStampForProject(cache, ctok))


    /// Parse and typecheck the whole project.
    member bc.ParseAndCheckProject(options, userOpName) =
        reactor.EnqueueAndAwaitOpAsync(userOpName, "ParseAndCheckProject", options.ProjectFileName, fun ctok -> bc.ParseAndCheckProjectImpl(options, ctok, userOpName))

    member _.GetProjectOptionsFromScript(filename, sourceText, previewEnabled, loadedTimeStamp, otherFlags, useFsiAuxLib: bool option, useSdkRefs: bool option, sdkDirOverride: string option, assumeDotNetFramework: bool option, extraProjectInfo: obj option, optionsStamp: int64 option, userOpName) = 

        reactor.EnqueueAndAwaitOpAsync (userOpName, "GetProjectOptionsFromScript", filename, fun ctok -> 
          cancellable {
            use errors = new ErrorScope()

            // Do we add a reference to FSharp.Compiler.Interactive.Settings by default?
            let useFsiAuxLib = defaultArg useFsiAuxLib true
            let useSdkRefs =  defaultArg useSdkRefs true
            let reduceMemoryUsage = ReduceMemoryFlag.Yes
            let previewEnabled = defaultArg previewEnabled false

            // Do we assume .NET Framework references for scripts?
            let assumeDotNetFramework = defaultArg assumeDotNetFramework true
            let extraFlags =
                if previewEnabled then
                    [| "--langversion:preview" |]
                else
                    [||]
            let otherFlags = defaultArg otherFlags extraFlags
            let useSimpleResolution = 
#if ENABLE_MONO_SUPPORT
                runningOnMono || otherFlags |> Array.exists (fun x -> x = "--simpleresolution")
#else
                true
#endif
            let loadedTimeStamp = defaultArg loadedTimeStamp DateTime.MaxValue // Not 'now', we don't want to force reloading
            let applyCompilerOptions tcConfigB  = 
                let fsiCompilerOptions = CompilerOptions.GetCoreFsiCompilerOptions tcConfigB 
                CompilerOptions.ParseCompilerOptions (ignore, fsiCompilerOptions, Array.toList otherFlags)

            let loadClosure = 
                LoadClosure.ComputeClosureOfScriptText(ctok, legacyReferenceResolver, 
                    FSharpCheckerResultsSettings.defaultFSharpBinariesDir, filename, sourceText, 
                    CodeContext.Editing, useSimpleResolution, useFsiAuxLib, useSdkRefs, sdkDirOverride, new Lexhelp.LexResourceManager(), 
                    applyCompilerOptions, assumeDotNetFramework, 
                    tryGetMetadataSnapshot, reduceMemoryUsage, dependencyProviderForScripts)

            let otherFlags = 
                [| yield "--noframework"; yield "--warn:3";
                   yield! otherFlags 
                   for r in loadClosure.References do yield "-r:" + fst r
                   for (code,_) in loadClosure.NoWarns do yield "--nowarn:" + code
                |]

            let options = 
                {
                    ProjectFileName = filename + ".fsproj" // Make a name that is unique in this directory.
                    ProjectId = None
                    SourceFiles = loadClosure.SourceFiles |> List.map fst |> List.toArray
                    OtherOptions = otherFlags 
                    ReferencedProjects= [| |]  
                    IsIncompleteTypeCheckEnvironment = false
                    UseScriptResolutionRules = true 
                    LoadTime = loadedTimeStamp
                    UnresolvedReferences = Some (FSharpUnresolvedReferencesSet(loadClosure.UnresolvedReferences))
                    OriginalLoadReferences = loadClosure.OriginalLoadReferences
                    ExtraProjectInfo=extraProjectInfo
                    Stamp = optionsStamp
                }
            scriptClosureCache.Set(AnyCallerThread, options, loadClosure) // Save the full load closure for later correlation.
            let diags = loadClosure.LoadClosureRootFileDiagnostics |> List.map (fun (exn, isError) -> FSharpDiagnostic.CreateFromException(exn, isError, range.Zero, false))
            return options, (diags @ errors.Diagnostics)
          })
            
    member bc.InvalidateConfiguration(options : FSharpProjectOptions, startBackgroundCompileIfAlreadySeen, userOpName) =
        let startBackgroundCompileIfAlreadySeen = defaultArg startBackgroundCompileIfAlreadySeen implicitlyStartBackgroundWork

        // This operation can't currently be cancelled nor awaited
        reactor.EnqueueOp(userOpName, "InvalidateConfiguration: Stamp(" + (options.Stamp |> Option.defaultValue 0L).ToString() + ")", options.ProjectFileName, fun ctok -> 
            // If there was a similar entry then re-establish an empty builder .  This is a somewhat arbitrary choice - it
            // will have the effect of releasing memory associated with the previous builder, but costs some time.
            if incrementalBuildersCache.ContainsSimilarKey (AnyCallerThread, options) then

                // We do not need to decrement here - it is done by disposal.
                let newBuilderInfo = CreateOneIncrementalBuilder (ctok, options, userOpName) |> Cancellable.runWithoutCancellation
                incrementalBuildersCache.Set(AnyCallerThread, options, newBuilderInfo)

                // Start working on the project.  Also a somewhat arbitrary choice
                if startBackgroundCompileIfAlreadySeen then 
                    bc.CheckProjectInBackground(options, userOpName + ".StartBackgroundCompile"))

    member bc.ClearCache(options : FSharpProjectOptions seq, userOpName) =
        // This operation can't currently be cancelled nor awaited
        reactor.EnqueueOp(userOpName, "ClearCache", String.Empty, fun _ ->
            options
            |> Seq.iter (fun options -> incrementalBuildersCache.RemoveAnySimilar(AnyCallerThread, options)))

    member _.NotifyProjectCleaned (options : FSharpProjectOptions, userOpName) =
        reactor.EnqueueAndAwaitOpAsync(userOpName, "NotifyProjectCleaned", options.ProjectFileName, fun ctok -> 
            cancellable {
                // If there was a similar entry (as there normally will have been) then re-establish an empty builder .  This 
                // is a somewhat arbitrary choice - it will have the effect of releasing memory associated with the previous 
                // builder, but costs some time.
                if incrementalBuildersCache.ContainsSimilarKey (AnyCallerThread, options) then
                    // We do not need to decrement here - it is done by disposal.
                    let! newBuilderInfo = CreateOneIncrementalBuilder (ctok, options, userOpName) 
                    incrementalBuildersCache.Set(AnyCallerThread, options, newBuilderInfo)
            })

    member _.CheckProjectInBackground (options, userOpName) =
        reactor.SetBackgroundOp (Some (userOpName, "CheckProjectInBackground", options.ProjectFileName, (fun ctok ct -> 
            // The creation of the background builder can't currently be cancelled
            match getOrCreateBuilder (ctok, options, userOpName) |> Cancellable.run ct with
            | ValueOrCancelled.Cancelled _ -> false
            | ValueOrCancelled.Value (builderOpt,_) ->
                match builderOpt with 
                | None -> false
                | Some builder -> 
                    // The individual steps of the background build 
                    match builder.Step(ctok) |> Cancellable.run ct with
                    | ValueOrCancelled.Value v -> v
                    | ValueOrCancelled.Cancelled _ -> false)))

    member _.StopBackgroundCompile   () =
        reactor.SetBackgroundOp(None)

    member _.WaitForBackgroundCompile() =
        reactor.WaitForBackgroundOpCompletion() 

    member _.CompleteAllQueuedOps() =
        reactor.CompleteAllQueuedOps() 

    member _.Reactor  = reactor

    member _.ReactorOps  = reactorOps

    member _.BeforeBackgroundFileCheck = beforeFileChecked.Publish

    member _.FileParsed = fileParsed.Publish

    member _.FileChecked = fileChecked.Publish

    member _.ProjectChecked = projectChecked.Publish

    member _.CurrentQueueLength = reactor.CurrentQueueLength

    member _.ClearCachesAsync (userOpName) =
        reactor.EnqueueAndAwaitOpAsync (userOpName, "ClearCachesAsync", "", fun ctok -> 
            parseCacheLock.AcquireLock (fun ltok -> 
                checkFileInProjectCachePossiblyStale.Clear ltok
                checkFileInProjectCache.Clear ltok
                parseFileCache.Clear(ltok))
            incrementalBuildersCache.Clear(AnyCallerThread)
            frameworkTcImportsCache.Clear ctok
            scriptClosureCache.Clear (AnyCallerThread)
            cancellable.Return ())

    member _.DownsizeCaches(userOpName) =
        reactor.EnqueueAndAwaitOpAsync (userOpName, "DownsizeCaches", "", fun ctok -> 
            parseCacheLock.AcquireLock (fun ltok -> 
                checkFileInProjectCachePossiblyStale.Resize(ltok, newKeepStrongly=1)
                checkFileInProjectCache.Resize(ltok, newKeepStrongly=1)
                parseFileCache.Resize(ltok, newKeepStrongly=1))
            incrementalBuildersCache.Resize(AnyCallerThread, newKeepStrongly=1, newKeepMax=1)
            frameworkTcImportsCache.Downsize(ctok)
            scriptClosureCache.Resize(AnyCallerThread,newKeepStrongly=1, newKeepMax=1)
            cancellable.Return ())
         
    member _.FrameworkImportsCache = frameworkTcImportsCache

    member _.ImplicitlyStartBackgroundWork with get() = implicitlyStartBackgroundWork and set v = implicitlyStartBackgroundWork <- v

    static member GlobalForegroundParseCountStatistic = foregroundParseCount

    static member GlobalForegroundTypeCheckCountStatistic = foregroundTypeCheckCount


[<Sealed; AutoSerializable(false)>]
// There is typically only one instance of this type in an IDE process.
type FSharpChecker(legacyReferenceResolver, 
                    projectCacheSize, 
                    keepAssemblyContents,
                    keepAllBackgroundResolutions,
                    tryGetMetadataSnapshot,
                    suggestNamesForErrors,
                    keepAllBackgroundSymbolUses,
                    enableBackgroundItemKeyStoreAndSemanticClassification,
                    enablePartialTypeChecking) =

    let backgroundCompiler = BackgroundCompiler(legacyReferenceResolver, projectCacheSize, keepAssemblyContents, keepAllBackgroundResolutions, tryGetMetadataSnapshot, suggestNamesForErrors, keepAllBackgroundSymbolUses, enableBackgroundItemKeyStoreAndSemanticClassification, enablePartialTypeChecking)

    static let globalInstance = lazy FSharpChecker.Create()
            
    // STATIC ROOT: FSharpLanguageServiceTestable.FSharpChecker.braceMatchCache. Most recently used cache for brace matching. Accessed on the
    // background UI thread, not on the compiler thread.
    //
    // This cache is safe for concurrent access.
    let braceMatchCache = MruCache<AnyCallerThreadToken,_,_>(braceMatchCacheSize, areSimilar = AreSimilarForParsing, areSame = AreSameForParsing) 

    let mutable maxMemoryReached = false

    let mutable maxMB = maxMBDefault

    let maxMemEvent = new Event<unit>()

    /// Instantiate an interactive checker.    
    static member Create(?projectCacheSize, ?keepAssemblyContents, ?keepAllBackgroundResolutions, ?legacyReferenceResolver, ?tryGetMetadataSnapshot, ?suggestNamesForErrors, ?keepAllBackgroundSymbolUses, ?enableBackgroundItemKeyStoreAndSemanticClassification, ?enablePartialTypeChecking) = 

        let legacyReferenceResolver = 
            match legacyReferenceResolver with
            | Some rr -> rr
            | None -> SimulatedMSBuildReferenceResolver.getResolver()

        let keepAssemblyContents = defaultArg keepAssemblyContents false
        let keepAllBackgroundResolutions = defaultArg keepAllBackgroundResolutions true
        let projectCacheSizeReal = defaultArg projectCacheSize projectCacheSizeDefault
        let tryGetMetadataSnapshot = defaultArg tryGetMetadataSnapshot (fun _ -> None)
        let suggestNamesForErrors = defaultArg suggestNamesForErrors false
        let keepAllBackgroundSymbolUses = defaultArg keepAllBackgroundSymbolUses true
        let enableBackgroundItemKeyStoreAndSemanticClassification = defaultArg enableBackgroundItemKeyStoreAndSemanticClassification false
        let enablePartialTypeChecking = defaultArg enablePartialTypeChecking false

        if keepAssemblyContents && enablePartialTypeChecking then
            invalidArg "enablePartialTypeChecking" "'keepAssemblyContents' and 'enablePartialTypeChecking' cannot be both enabled."

        new FSharpChecker(legacyReferenceResolver, projectCacheSizeReal,keepAssemblyContents, keepAllBackgroundResolutions, tryGetMetadataSnapshot, suggestNamesForErrors, keepAllBackgroundSymbolUses, enableBackgroundItemKeyStoreAndSemanticClassification, enablePartialTypeChecking)

    member _.ReferenceResolver = legacyReferenceResolver

    member _.MatchBraces(filename, sourceText: ISourceText, options: FSharpParsingOptions, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"
        let hash = sourceText.GetHashCode()
        async {
            match braceMatchCache.TryGet(AnyCallerThread, (filename, hash, options)) with
            | Some res -> return res
            | None ->
                let res = ParseAndCheckFile.matchBraces(sourceText, filename, options, userOpName, suggestNamesForErrors)
                braceMatchCache.Set(AnyCallerThread, (filename, hash, options), res)
                return res
        }

    member ic.MatchBraces(filename, source: string, options: FSharpProjectOptions, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"
        let parsingOptions, _ = ic.GetParsingOptionsFromProjectOptions(options)
        ic.MatchBraces(filename, SourceText.ofString source, parsingOptions, userOpName)

    member ic.GetParsingOptionsFromProjectOptions(options): FSharpParsingOptions * _ =
        let sourceFiles = List.ofArray options.SourceFiles
        let argv = List.ofArray options.OtherOptions
        ic.GetParsingOptionsFromCommandLineArgs(sourceFiles, argv, options.UseScriptResolutionRules)

    member ic.ParseFile(filename, sourceText, options, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"
        ic.CheckMaxMemoryReached()
        backgroundCompiler.ParseFile(filename, sourceText, options, userOpName)

    member ic.ParseFileNoCache(filename, sourceText, options, ?userOpName) =
        let userOpName = defaultArg userOpName "Unknown"
        ic.CheckMaxMemoryReached()
        backgroundCompiler.ParseFileNoCache(filename, sourceText, options, userOpName)

    member ic.ParseFileInProject(filename, source: string, options, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"
        let parsingOptions, _ = ic.GetParsingOptionsFromProjectOptions(options)
        ic.ParseFile(filename, SourceText.ofString source, parsingOptions, userOpName)

    member _.GetBackgroundParseResultsForFileInProject (filename,options, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"
        backgroundCompiler.GetBackgroundParseResultsForFileInProject(filename, options, userOpName)
        
    member _.GetBackgroundCheckResultsForFileInProject (filename,options, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"
        backgroundCompiler.GetBackgroundCheckResultsForFileInProject(filename,options, userOpName)
        
    /// Try to get recent approximate type check results for a file. 
    member _.TryGetRecentCheckResultsForFile(filename: string, options:FSharpProjectOptions, ?sourceText, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"
        backgroundCompiler.TryGetRecentCheckResultsForFile(filename,options,sourceText, userOpName)

    member _.Compile(argv: string[], ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"
        backgroundCompiler.Reactor.EnqueueAndAwaitOpAsync (userOpName, "Compile", "", fun ctok -> 
            cancellable {
                return CompileHelpers.compileFromArgs (ctok, argv, legacyReferenceResolver, None, None)
            })

    member _.Compile (ast:ParsedInput list, assemblyName:string, outFile:string, dependencies:string list, ?pdbFile:string, ?executable:bool, ?noframework:bool, ?userOpName: string) =
      let userOpName = defaultArg userOpName "Unknown"
      backgroundCompiler.Reactor.EnqueueAndAwaitOpAsync (userOpName, "Compile", assemblyName, fun ctok -> 
       cancellable {
            let noframework = defaultArg noframework false
            return CompileHelpers.compileFromAsts (ctok, legacyReferenceResolver, ast, assemblyName, outFile, dependencies, noframework, pdbFile, executable, None, None)
       }
      )

    member _.CompileToDynamicAssembly (otherFlags: string[], execute: (TextWriter * TextWriter) option, ?userOpName: string)  = 
      let userOpName = defaultArg userOpName "Unknown"
      backgroundCompiler.Reactor.EnqueueAndAwaitOpAsync (userOpName, "CompileToDynamicAssembly", "<dynamic>", fun ctok -> 
       cancellable {
        CompileHelpers.setOutputStreams execute
        
        // References used to capture the results of compilation
        let tcImportsRef = ref None
        let assemblyBuilderRef = ref None
        let tcImportsCapture = Some (fun tcImports -> tcImportsRef := Some tcImports)

        // Function to generate and store the results of compilation 
        let debugInfo =  otherFlags |> Array.exists (fun arg -> arg = "-g" || arg = "--debug:+" || arg = "/debug:+")
        let dynamicAssemblyCreator = Some (CompileHelpers.createDynamicAssembly (ctok, debugInfo, tcImportsRef, execute.IsSome, assemblyBuilderRef))

        // Perform the compilation, given the above capturing function.
        let errorsAndWarnings, result = CompileHelpers.compileFromArgs (ctok, otherFlags, legacyReferenceResolver, tcImportsCapture, dynamicAssemblyCreator)

        // Retrieve and return the results
        let assemblyOpt = 
            match assemblyBuilderRef.Value with 
            | None -> None
            | Some a ->  Some (a :> System.Reflection.Assembly)

        return errorsAndWarnings, result, assemblyOpt
       }
      )

    member _.CompileToDynamicAssembly (ast:ParsedInput list, assemblyName:string, dependencies:string list, execute: (TextWriter * TextWriter) option, ?debug:bool, ?noframework:bool, ?userOpName: string) =
      let userOpName = defaultArg userOpName "Unknown"
      backgroundCompiler.Reactor.EnqueueAndAwaitOpAsync (userOpName, "CompileToDynamicAssembly", assemblyName, fun ctok -> 
       cancellable {
        CompileHelpers.setOutputStreams execute

        // References used to capture the results of compilation
        let tcImportsRef = ref (None: TcImports option)
        let assemblyBuilderRef = ref None
        let tcImportsCapture = Some (fun tcImports -> tcImportsRef := Some tcImports)

        let debugInfo = defaultArg debug false
        let noframework = defaultArg noframework false
        let location = Path.Combine(Path.GetTempPath(),"test"+string(hash assemblyName))
        try Directory.CreateDirectory(location) |> ignore with _ -> ()

        let outFile = Path.Combine(location, assemblyName + ".dll")

        // Function to generate and store the results of compilation 
        let dynamicAssemblyCreator = Some (CompileHelpers.createDynamicAssembly (ctok, debugInfo, tcImportsRef, execute.IsSome, assemblyBuilderRef))

        // Perform the compilation, given the above capturing function.
        let errorsAndWarnings, result = 
            CompileHelpers.compileFromAsts (ctok, legacyReferenceResolver, ast, assemblyName, outFile, dependencies, noframework, None, Some execute.IsSome, tcImportsCapture, dynamicAssemblyCreator)

        // Retrieve and return the results
        let assemblyOpt = 
            match assemblyBuilderRef.Value with 
            | None -> None
            | Some a ->  Some (a :> System.Reflection.Assembly)

        return errorsAndWarnings, result, assemblyOpt
       }
      )

    /// This function is called when the entire environment is known to have changed for reasons not encoded in the ProjectOptions of any project/compilation.
    /// For example, the type provider approvals file may have changed.
    member ic.InvalidateAll() =
        ic.ClearCaches()
            
    member _.ClearCachesAsync(?userOpName: string) =
        let utok = AnyCallerThread
        let userOpName = defaultArg userOpName "Unknown"
        braceMatchCache.Clear(utok)
        backgroundCompiler.ClearCachesAsync(userOpName) 

    member ic.ClearCaches(?userOpName) =
        ic.ClearCachesAsync(?userOpName=userOpName) |> Async.Start // this cache clearance is not synchronous, it will happen when the background op gets run

    member _.CheckMaxMemoryReached() =
        if not maxMemoryReached && System.GC.GetTotalMemory(false) > int64 maxMB * 1024L * 1024L then 
            Trace.TraceWarning("!!!!!!!! MAX MEMORY REACHED, DOWNSIZING F# COMPILER CACHES !!!!!!!!!!!!!!!")
            // If the maxMB limit is reached, drastic action is taken
            //   - reduce strong cache sizes to a minimum
            let userOpName = "MaxMemoryReached"
            backgroundCompiler.CompleteAllQueuedOps()
            maxMemoryReached <- true
            braceMatchCache.Resize(AnyCallerThread, newKeepStrongly=10)
            backgroundCompiler.DownsizeCaches(userOpName) |> Async.RunSynchronously
            maxMemEvent.Trigger( () )

    // This is for unit testing only
    member ic.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients() =
        backgroundCompiler.CompleteAllQueuedOps() // flush AsyncOp
        ic.ClearCachesAsync() |> Async.RunSynchronously
        System.GC.Collect()
        System.GC.WaitForPendingFinalizers() 
        backgroundCompiler.CompleteAllQueuedOps() // flush AsyncOp
        FxResolver.ClearStaticCaches()
            
    /// This function is called when the configuration is known to have changed for reasons not encoded in the ProjectOptions.
    /// For example, dependent references may have been deleted or created.
    member _.InvalidateConfiguration(options: FSharpProjectOptions, ?startBackgroundCompile, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"
        backgroundCompiler.InvalidateConfiguration(options, startBackgroundCompile, userOpName)

    /// Clear the internal cache of the given projects.
    member _.ClearCache(options: FSharpProjectOptions seq, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"
        backgroundCompiler.ClearCache(options, userOpName)

    /// This function is called when a project has been cleaned, and thus type providers should be refreshed.
    member _.NotifyProjectCleaned(options: FSharpProjectOptions, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"
        backgroundCompiler.NotifyProjectCleaned (options, userOpName)
              
    /// Typecheck a source code file, returning a handle to the results of the 
    /// parse including the reconstructed types in the file.
    member _.CheckFileInProjectAllowingStaleCachedResults(parseResults:FSharpParseFileResults, filename:string, fileVersion:int, source:string, options:FSharpProjectOptions, ?userOpName: string) =        
        let userOpName = defaultArg userOpName "Unknown"
        backgroundCompiler.CheckFileInProjectAllowingStaleCachedResults(parseResults,filename,fileVersion,SourceText.ofString source,options,userOpName)

    /// Typecheck a source code file, returning a handle to the results of the 
    /// parse including the reconstructed types in the file.
    member ic.CheckFileInProject(parseResults:FSharpParseFileResults, filename:string, fileVersion:int, sourceText:ISourceText, options:FSharpProjectOptions, ?userOpName: string) =        
        let userOpName = defaultArg userOpName "Unknown"
        ic.CheckMaxMemoryReached()
        backgroundCompiler.CheckFileInProject(parseResults,filename,fileVersion,sourceText,options,userOpName)

    /// Typecheck a source code file, returning a handle to the results of the 
    /// parse including the reconstructed types in the file.
    member ic.ParseAndCheckFileInProject(filename:string, fileVersion:int, sourceText:ISourceText, options:FSharpProjectOptions, ?userOpName: string) =        
        let userOpName = defaultArg userOpName "Unknown"
        ic.CheckMaxMemoryReached()
        backgroundCompiler.ParseAndCheckFileInProject(filename, fileVersion, sourceText, options, userOpName)
            
    member ic.ParseAndCheckProject(options, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"
        ic.CheckMaxMemoryReached()
        backgroundCompiler.ParseAndCheckProject(options, userOpName)

    member ic.FindBackgroundReferencesInFile(filename:string, options: FSharpProjectOptions, symbol: FSharpSymbol, ?canInvalidateProject: bool, ?userOpName: string) =
        let canInvalidateProject = defaultArg canInvalidateProject true
        let userOpName = defaultArg userOpName "Unknown"
        ic.CheckMaxMemoryReached()
        backgroundCompiler.FindReferencesInFile(filename, options, symbol, canInvalidateProject, userOpName)

    member ic.GetBackgroundSemanticClassificationForFile(filename:string, options: FSharpProjectOptions, ?userOpName) =
        let userOpName = defaultArg userOpName "Unknown"
        ic.CheckMaxMemoryReached()
        backgroundCompiler.GetSemanticClassificationForFile(filename, options, userOpName)

    /// For a given script file, get the ProjectOptions implied by the #load closure
    member _.GetProjectOptionsFromScript(filename, source, ?previewEnabled, ?loadedTimeStamp, ?otherFlags, ?useFsiAuxLib, ?useSdkRefs, ?assumeDotNetFramework, ?sdkDirOverride, ?extraProjectInfo: obj, ?optionsStamp: int64, ?userOpName: string) = 
        let userOpName = defaultArg userOpName "Unknown"
        backgroundCompiler.GetProjectOptionsFromScript(filename, source, previewEnabled, loadedTimeStamp, otherFlags, useFsiAuxLib, useSdkRefs, sdkDirOverride, assumeDotNetFramework, extraProjectInfo, optionsStamp, userOpName)

    member _.GetProjectOptionsFromCommandLineArgs(projectFileName, argv, ?loadedTimeStamp, ?extraProjectInfo: obj) = 
        let loadedTimeStamp = defaultArg loadedTimeStamp DateTime.MaxValue // Not 'now', we don't want to force reloading
        { ProjectFileName = projectFileName
          ProjectId = None
          SourceFiles = [| |] // the project file names will be inferred from the ProjectOptions
          OtherOptions = argv 
          ReferencedProjects= [| |]  
          IsIncompleteTypeCheckEnvironment = false
          UseScriptResolutionRules = false
          LoadTime = loadedTimeStamp
          UnresolvedReferences = None
          OriginalLoadReferences=[]
          ExtraProjectInfo=extraProjectInfo
          Stamp = None }

    member _.GetParsingOptionsFromCommandLineArgs(sourceFiles, argv, ?isInteractive) =
        let isInteractive = defaultArg isInteractive false
        use errorScope = new ErrorScope()
        let tcConfigBuilder = TcConfigBuilder.Initial

        // Apply command-line arguments and collect more source files if they are in the arguments
        let sourceFilesNew = ApplyCommandLineArgs(tcConfigBuilder, sourceFiles, argv)
        FSharpParsingOptions.FromTcConfigBuilder(tcConfigBuilder, Array.ofList sourceFilesNew, isInteractive), errorScope.Diagnostics

    member ic.GetParsingOptionsFromCommandLineArgs(argv, ?isInteractive: bool) =
        ic.GetParsingOptionsFromCommandLineArgs([], argv, ?isInteractive=isInteractive)

    /// Begin background parsing the given project.
    member _.StartBackgroundCompile(options, ?userOpName) = 
        let userOpName = defaultArg userOpName "Unknown"
        backgroundCompiler.CheckProjectInBackground(options, userOpName) 

    /// Begin background parsing the given project.
    member ic.CheckProjectInBackground(options, ?userOpName) = 
        ic.StartBackgroundCompile(options, ?userOpName=userOpName)

    /// Stop the background compile.
    member _.StopBackgroundCompile() = 
        backgroundCompiler.StopBackgroundCompile()

    /// Block until the background compile finishes.
    //
    // This is for unit testing only
    member _.WaitForBackgroundCompile() = backgroundCompiler.WaitForBackgroundCompile()

    // Publish the ReactorOps from the background compiler for internal use
    member ic.ReactorOps = backgroundCompiler.ReactorOps

    member _.CurrentQueueLength = backgroundCompiler.CurrentQueueLength

    member _.BeforeBackgroundFileCheck  = backgroundCompiler.BeforeBackgroundFileCheck

    member _.FileParsed  = backgroundCompiler.FileParsed

    member _.FileChecked  = backgroundCompiler.FileChecked

    member _.ProjectChecked = backgroundCompiler.ProjectChecked

    member _.ImplicitlyStartBackgroundWork with get() = backgroundCompiler.ImplicitlyStartBackgroundWork and set v = backgroundCompiler.ImplicitlyStartBackgroundWork <- v

    member _.PauseBeforeBackgroundWork with get() = Reactor.Singleton.PauseBeforeBackgroundWork and set v = Reactor.Singleton.PauseBeforeBackgroundWork <- v

    static member GlobalForegroundParseCountStatistic = BackgroundCompiler.GlobalForegroundParseCountStatistic

    static member GlobalForegroundTypeCheckCountStatistic = BackgroundCompiler.GlobalForegroundTypeCheckCountStatistic
          
    member _.MaxMemoryReached = maxMemEvent.Publish

    member _.MaxMemory with get() = maxMB and set v = maxMB <- v
    
    static member Instance with get() = globalInstance.Force()

    member internal _.FrameworkImportsCache = backgroundCompiler.FrameworkImportsCache

    /// Tokenize a single line, returning token information and a tokenization state represented by an integer
    member _.TokenizeLine (line: string, state: FSharpTokenizerLexState) = 
        let tokenizer = FSharpSourceTokenizer([], None)
        let lineTokenizer = tokenizer.CreateLineTokenizer line
        let mutable state = (None, state)
        let tokens = 
            [| while (state <- lineTokenizer.ScanToken (snd state); (fst state).IsSome) do
                    yield (fst state).Value |]
        tokens, snd state 

    /// Tokenize an entire file, line by line
    member x.TokenizeFile (source: string) : FSharpTokenInfo[][] = 
        let lines = source.Split('\n')
        let tokens = 
            [| let mutable state = FSharpTokenizerLexState.Initial
               for line in lines do 
                   let tokens, n = x.TokenizeLine(line, state) 
                   state <- n 
                   yield tokens |]
        tokens

type CompilerEnvironment =
  static member BinFolderOfDefaultFSharpCompiler(?probePoint) =
      FSharpEnvironment.BinFolderOfDefaultFSharpCompiler(probePoint)

/// Information about the compilation environment
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module CompilerEnvironment =

    // Legacy entry point, no longer used by FSharp.Editor
    let DefaultReferencesForOrphanSources assumeDotNetFramework =
        let currentDirectory = Directory.GetCurrentDirectory()
        let fxResolver = FxResolver(Some assumeDotNetFramework, currentDirectory, rangeForErrors=range0, useSdkRefs=true, isInteractive=false, sdkDirOverride=None)
        let references, _ = fxResolver.GetDefaultReferences (useFsiAuxLib=false, assumeDotNetFramework=assumeDotNetFramework)
        references
    
    /// Publish compiler-flags parsing logic. Must be fast because its used by the colorizer.
    let GetCompilationDefinesForEditing (parsingOptions: FSharpParsingOptions) =
        SourceFileImpl.AdditionalDefinesForUseInEditor(parsingOptions.IsInteractive) @
        parsingOptions.ConditionalCompilationDefines
            
    /// Return true if this is a subcategory of error or warning message that the language service can emit
    let IsCheckerSupportedSubcategory(subcategory:string) =
        // Beware: This code logic is duplicated in DocumentTask.cs in the language service
        PhasedDiagnostic.IsSubcategoryOfCompile(subcategory)

/// Information about the debugging environment
module DebuggerEnvironment =
    /// Return the language ID, which is the expression evaluator id that the
    /// debugger will use.
    let GetLanguageID() =
        System.Guid(0xAB4F38C9u, 0xB6E6us, 0x43baus, 0xBEuy, 0x3Buy, 0x58uy, 0x08uy, 0x0Buy, 0x2Cuy, 0xCCuy, 0xE3uy)
        
module FSharpFileUtilities =
    let isScriptFile (fileName: string) = ParseAndCheckInputs.IsScript fileName