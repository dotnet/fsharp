namespace FSharp.Compiler.Service

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
open FSharp.Compiler.Service.Utilities

/// Accumulated results of type checking.
[<NoEquality; NoComparison>]
type TcAccumulator =
    { tcState: TcState
      tcEnvAtEndOfFile: TcEnv

      /// Accumulated resolutions, last file first
      tcResolutionsRev: TcResolutions list

      /// Accumulated symbol uses, last file first
      tcSymbolUsesRev: TcSymbolUses list

      /// Accumulated 'open' declarations, last file first
      tcOpenDeclarationsRev: OpenDeclaration[] list

      topAttribs: TopAttribs option

      /// Result of checking most recent file, if any
      latestImplFile: TypedImplFile option

      latestCcuSigForFile: ModuleOrNamespaceType option

      tcDependencyFiles: string list

      /// Disambiguation table for module names
      tcModuleNamesDict: ModuleNamesDict

      /// Accumulated errors, last file first
      tcErrorsRev:(PhasedDiagnostic * FSharpErrorSeverity)[] list }

[<RequireQualifiedAccess>]
type CompilationResult =
   // | NotParsed of Source
    | Parsed of Source * SyntaxTree
   // | SignatureChecked of SyntaxTree * TcAccumulator // is an impl file, but only checked its signature file (.fsi)
    | Checked of Source * WeakReference<SyntaxTree> * TcAccumulator

type ParsingOptions =
    {
        isExecutable: bool
        lexResourceManager: Lexhelp.LexResourceManager
    }

type IncrementalCheckerState =
    {
        tcConfig: TcConfig
        parsingOptions: ParsingOptions
        orderedFilePaths: ImmutableArray<string>
        resultCache: ImmutableDictionary<string, int * CompilationResult ref>
        version: VersionStamp
    }

    static member Create (tcConfig, parsingOptions) =
        {
            tcConfig = tcConfig
            parsingOptions = parsingOptions
            orderedFilePaths = ImmutableArray.Empty
            resultCache = ImmutableDictionary.Empty
            version = VersionStamp.Create ()
        }

    member this.ParseSource (source: Source, isLastFile) =
        let parsingInfo =
            {
                tcConfig = this.tcConfig
                isLastFileOrScript = isLastFile
                isExecutable = this.parsingOptions.isExecutable
                conditionalCompilationDefines = []
                sourceText = source.SourceText
                filePath = source.FilePath
            }

        Parser.Parse parsingInfo

    member this.ParseSource (source: Source) =
        match this.resultCache.TryGetValue source.FilePath with
        | false, _ -> failwith "source does not exist in incremental checker"
        | true, (i, _) ->

            let isLastFile = (this.orderedFilePaths.Length - 1) = i
            this.ParseSource (source, isLastFile)

    member this.AddSources (orderedSources: ImmutableArray<Source>) =
        if Seq.isEmpty orderedSources then this
        else
            let orderedFilePathsBuilder = ImmutableArray.CreateBuilder (this.orderedFilePaths.Length + orderedSources.Length)
            let resultCacheBuilder = ImmutableDictionary.CreateBuilder StringComparer.OrdinalIgnoreCase

            orderedFilePathsBuilder.AddRange this.orderedFilePaths
            resultCacheBuilder.AddRange this.resultCache

            orderedFilePathsBuilder.Count <- this.orderedFilePaths.Length + orderedSources.Length
                
            let offset = this.orderedFilePaths.Length
            Parallel.For (0, orderedSources.Length, fun i ->
                let source = orderedSources.[i]
                let isLastFile = (orderedFilePathsBuilder.Count - 1) = (i + offset)
                let syntaxTree = this.ParseSource (source, isLastFile)
                resultCacheBuilder.Add (source.FilePath, (offset + i, ref (CompilationResult.Parsed (source, syntaxTree))))
            ) |> ignore

            { this with
                orderedFilePaths = orderedFilePathsBuilder.ToImmutableArray ()
                resultCache = resultCacheBuilder.ToImmutableDictionary ()
                version = VersionStamp.Create ()
            }

    member this.ReplaceSource (source: Source) =
        match this.resultCache.TryGetValue source.FilePath with
        | false, _ -> failwith "syntax tree does not exist in incremental checker"
        | true, (_i, _) ->

            let mutable resultCache = this.resultCache//this.resultCache.SetItem(source.FilePath, (i, ref (CompilationResult.Parsed source)))

            //for i = i + 1 to this.orderedFilePaths.Length - 1 do
            //    let filePath = this.orderedFilePaths.[i]
            //    match this.resultCache.TryGetValue filePath with
            //    | false, _ -> failwith "should not happen"
            //    | true, (i, refResult) ->
            //        let syntaxTree =
            //            match refResult.contents with
            //            | CompilationResult.Parsed syntaxTree -> syntaxTree
            //            | CompilationResult.Checked (syntaxTree, _) -> syntaxTree
            //        resultCache <- resultCache.SetItem(syntaxTree.FilePath, (i, ref (CompilationResult.Parsed syntaxTree)))

            { this with
                resultCache = resultCache
                version = this.version.NewVersionStamp ()
            }

type IncrementalCheckerOptions =
    {
        keepAssemblyContents: bool
        keepAllBackgroundResolutions: bool
        parsingOptions: ParsingOptions
    }

type CheckerFlags =
    | None = 0x0
    | ReturnResolutions = 0x1 

[<Sealed>]
type IncrementalChecker (tcConfig: TcConfig, tcGlobals: TcGlobals, tcImports: TcImports, initialTcAcc: TcAccumulator, options: IncrementalCheckerOptions, state: IncrementalCheckerState) =

    let maxTimeShareMilliseconds = 100L

    member __.Version = state.version

    member __.AddSources sources =
        let newState = state.AddSources sources
        IncrementalChecker (tcConfig, tcGlobals, tcImports, initialTcAcc, options, newState)

    member __.ReplaceSource source =
        let newState = state.ReplaceSource source
        IncrementalChecker (tcConfig, tcGlobals, tcImports, initialTcAcc, options, newState)

    member this.GetTcAcc (source: Source, cancellationToken) =
        match state.resultCache.TryGetValue source.FilePath with
        | true, (0, refResult) -> 
            let syntaxTree =
                match refResult.contents with
                | CompilationResult.Parsed (_, syntaxTree) -> syntaxTree
                | CompilationResult.Checked (source, _, _) -> state.ParseSource source // TODO:
            Eventually.Done (initialTcAcc, syntaxTree, refResult)
        | true, (i, refResult) ->
            match state.resultCache.TryGetValue state.orderedFilePaths.[i - 1] with
            | true, (_, nextRefResult) -> 
                match nextRefResult.contents with
                | CompilationResult.Parsed (source, syntaxTree) ->
                    eventually {
                        // We set no checker flags as we don't want to ask for extra information when checking a dependent file.
                        let! tcAcc, _ = this.Check (source, CheckerFlags.None, cancellationToken)
                        return (tcAcc, syntaxTree, refResult)
                    }
                | CompilationResult.Checked (source, _, tcAcc) ->
                    let syntaxTree = state.ParseSource source // TODO:
                    Eventually.Done (tcAcc, syntaxTree, refResult)
            | _ -> failwith "file does not exist in incremental checker"
        | _ -> failwith "file does not exist in incremental checker"

    member this.Check (source: Source, flags: CheckerFlags, cancellationToken: CancellationToken) =
        eventually {
            cancellationToken.ThrowIfCancellationRequested ()

            let filePath = source.FilePath
            let! (tcAcc, syntaxTree, refResult) = this.GetTcAcc (source, cancellationToken)
            let (inputOpt, parseErrors) = syntaxTree.parseResult
            match inputOpt with
            | Some input ->
                let capturingErrorLogger = CompilationErrorLogger("Check", tcConfig.errorSeverityOptions)
                let errorLogger = GetErrorLoggerFilteringByScopedPragmas(false, GetScopedPragmasForInput input, capturingErrorLogger)

                let fullComputation = 
                    eventually {                    
                        ApplyMetaCommandsFromInputToTcConfig (tcConfig, input, Path.GetDirectoryName filePath) |> ignore
                        let sink = TcResultsSinkImpl(tcGlobals)
                        let hadParseErrors = not (Array.isEmpty parseErrors)

                        let input, moduleNamesDict = DeduplicateParsedInputModuleName tcAcc.tcModuleNamesDict input

                        let! (tcEnvAtEndOfFile, topAttribs, implFile, ccuSigForFile), tcState = 
                            TypeCheckOneInputEventually 
                                ((fun () -> hadParseErrors || errorLogger.ErrorCount > 0), 
                                    tcConfig, tcImports, 
                                    tcGlobals, 
                                    None, 
                                    TcResultsSink.WithSink sink, 
                                    tcAcc.tcState, input)
                
                        /// Only keep the typed interface files when doing a "full" build for fsc.exe, otherwise just throw them away
                        let implFile = if options.keepAssemblyContents then implFile else None
                        let tcResolutions = if options.keepAllBackgroundResolutions then sink.GetResolutions() else TcResolutions.Empty
                        let tcEnvAtEndOfFile = (if options.keepAllBackgroundResolutions then tcEnvAtEndOfFile else tcState.TcEnvFromImpls)
                        let tcSymbolUses = sink.GetSymbolUses()

                        let tcResolutionsOpt =
                            if options.keepAllBackgroundResolutions then Some tcResolutions
                            elif (flags &&& CheckerFlags.ReturnResolutions = CheckerFlags.ReturnResolutions) then
                                Some (sink.GetResolutions ())
                            else
                                None
                                
                    
                        let newErrors = Array.append parseErrors (capturingErrorLogger.GetErrors())
                        return {tcAcc with  tcState=tcState 
                                            tcEnvAtEndOfFile=tcEnvAtEndOfFile
                                            topAttribs=Some topAttribs
                                            latestImplFile=implFile
                                            latestCcuSigForFile=Some ccuSigForFile
                                            tcResolutionsRev=tcResolutions :: tcAcc.tcResolutionsRev
                                            tcSymbolUsesRev=tcSymbolUses :: tcAcc.tcSymbolUsesRev
                                            tcOpenDeclarationsRev = sink.GetOpenDeclarations() :: tcAcc.tcOpenDeclarationsRev
                                            tcErrorsRev = newErrors :: tcAcc.tcErrorsRev 
                                            tcModuleNamesDict = moduleNamesDict
                                            tcDependencyFiles = filePath :: tcAcc.tcDependencyFiles }, tcResolutionsOpt
                    }

                // Run part of the Eventually<_> computation until a timeout is reached. If not complete, 
                // return a new Eventually<_> computation which recursively runs more of the computation.
                //   - When the whole thing is finished commit the error results sent through the errorLogger.
                //   - Each time we do real work we reinstall the CompilationGlobalsScope
                let timeSlicedComputation = 
                        fullComputation |> 
                            Eventually.repeatedlyProgressUntilDoneOrTimeShareOverOrCanceled 
                                maxTimeShareMilliseconds
                                cancellationToken
                                (fun ctok f -> 
                                    // Reinstall the compilation globals each time we start or restart
                                    use unwind = new CompilationGlobalsScope (errorLogger, BuildPhase.TypeCheck) 
                                    f ctok)

                let! tcAcc, tcResolutionsOpt = timeSlicedComputation
                refResult := CompilationResult.Checked (source, WeakReference<_> syntaxTree, tcAcc)
                return (tcAcc, tcResolutionsOpt)
                                       
            | _ ->
                return (tcAcc, None)
        }

type InitialInfo =
    {
        ctok: CompilationThreadToken
        tcConfig: TcConfig
        tcConfigP: TcConfigProvider
        tcGlobals: TcGlobals
        frameworkTcImports: TcImports
        nonFrameworkResolutions: AssemblyResolution list
        unresolvedReferences: UnresolvedAssemblyReference list
        importsInvalidated: Event<string>
        assemblyName: string
        niceNameGen: NiceNameGenerator
        loadClosureOpt: LoadClosure option
        projectDirectory: string
        checkerOptions: IncrementalCheckerOptions
    }

module IncrementalChecker =

    let rangeStartup = FSharp.Compiler.Range.rangeN "startup" 1

    let Create (info: InitialInfo) =
      let ctok = info.ctok
      let tcConfig = info.tcConfig
      let tcConfigP = info.tcConfigP
      let tcGlobals = info.tcGlobals
      let frameworkTcImports = info.frameworkTcImports
      let nonFrameworkResolutions = info.nonFrameworkResolutions
      let unresolvedReferences = info.unresolvedReferences
      let importsInvalidated = info.importsInvalidated
      let assemblyName = info.assemblyName
      let niceNameGen = info.niceNameGen
      let loadClosureOpt = info.loadClosureOpt
      let projectDirectory = info.projectDirectory

      cancellable {
        let errorLogger = CompilationErrorLogger("CombineImportedAssembliesTask", tcConfig.errorSeverityOptions)
        // Return the disposable object that cleans up
        use _holder = new CompilationGlobalsScope(errorLogger, BuildPhase.Parameter)

        let! tcImports = 
          cancellable {
            try
                let! tcImports = TcImports.BuildNonFrameworkTcImports(ctok, tcConfigP, tcGlobals, frameworkTcImports, nonFrameworkResolutions, unresolvedReferences)  
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
                    // The IncrementalBuilder holds the strong reference the importsInvalidated event.
                    //
                    // In the invalidation handler we use a weak reference to allow the IncrementalBuilder to 
                    // be collected if, for some reason, a TP instance is not disposed or not GC'd.
                    let capturedImportsInvalidated = WeakReference<_>(importsInvalidated)
                    ccu.Deref.InvalidateEvent.Add(fun msg -> 
                        match capturedImportsInvalidated.TryGetTarget() with 
                        | true, tg -> tg.Trigger msg
                        | _ -> ()))
    #endif

                return tcImports
            with e -> 
                System.Diagnostics.Debug.Assert(false, sprintf "Could not BuildAllReferencedDllTcImports %A" e)
                errorLogger.Warning e
                return frameworkTcImports           
          }

        let tcInitial = GetInitialTcEnv (assemblyName, rangeStartup, tcConfig, tcImports, tcGlobals)
        let tcState = GetInitialTcState (rangeStartup, assemblyName, tcConfig, tcGlobals, tcImports, niceNameGen, tcInitial)
        let loadClosureErrors = 
           [ match loadClosureOpt with 
             | None -> ()
             | Some loadClosure -> 
                for inp in loadClosure.Inputs do
                    for (err, isError) in inp.MetaCommandDiagnostics do 
                        yield err, (if isError then FSharpErrorSeverity.Error else FSharpErrorSeverity.Warning) ]

        let initialErrors = Array.append (Array.ofList loadClosureErrors) (errorLogger.GetErrors())

        let basicDependencies = 
            [ for (UnresolvedAssemblyReference(referenceText, _))  in unresolvedReferences do
                // Exclude things that are definitely not a file name
                if not(FileSystem.IsInvalidPathShim referenceText) then 
                    let file = if FileSystem.IsPathRootedShim referenceText then referenceText else Path.Combine(projectDirectory, referenceText) 
                    yield file 

              for r in nonFrameworkResolutions do 
                    yield  r.resolvedPath  ]

        let tcAcc = 
            { tcState=tcState
              tcEnvAtEndOfFile=tcInitial
              tcResolutionsRev=[]
              tcSymbolUsesRev=[]
              tcOpenDeclarationsRev=[]
              topAttribs=None
              latestImplFile=None
              latestCcuSigForFile=None
              tcDependencyFiles=basicDependencies
              tcErrorsRev = [ initialErrors ] 
              tcModuleNamesDict = Map.empty }
        return IncrementalChecker (tcConfig, tcGlobals, tcImports, tcAcc, info.checkerOptions, IncrementalCheckerState.Create (tcConfig, info.checkerOptions.parsingOptions))
        }
