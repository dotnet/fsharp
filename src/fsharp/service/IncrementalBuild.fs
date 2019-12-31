// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler

open System
open System.IO
open System.Threading
open System.Runtime.CompilerServices
open FSharp.Compiler
open FSharp.Compiler.NameResolution
open FSharp.Compiler.Tastops
open FSharp.Compiler.Lib
open FSharp.Compiler.AbstractIL
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILBinaryReader
open FSharp.Compiler.AbstractIL.Internal.Library 
open FSharp.Compiler.CompileOps
open FSharp.Compiler.CompileOptions
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypeChecker
open FSharp.Compiler.Tast 
open FSharp.Compiler.Range
open FSharp.Compiler.SourceCodeServices
open Internal.Utilities.Collections        

// Record the most recent IncrementalBuilder events, so we can more easily unit test/debug the 
// 'incremental' behavior of the product.
module IncrementalBuilderEventTesting = 

    type internal FixedLengthMRU<'T>() =
        let MAX = 400   // Length of the MRU.  For our current unit tests, 400 is enough.
        let data = Array.create MAX None
        let mutable curIndex = 0
        let mutable numAdds = 0
        // called by the product, to note when a parse/typecheck happens for a file
        member this.Add(fileName:'T) =
            numAdds <- numAdds + 1
            data.[curIndex] <- Some fileName
            curIndex <- (curIndex + 1) % MAX
        member this.CurrentEventNum = numAdds
        // called by unit tests, returns 'n' most recent additions.
        member this.MostRecentList(n: int) : list<'T> =
            if n < 0 || n > MAX then
                raise <| new System.ArgumentOutOfRangeException("n", sprintf "n must be between 0 and %d, inclusive, but got %d" MAX n)
            let mutable remaining = n
            let mutable s = []
            let mutable i = curIndex - 1
            while remaining <> 0 do
                if i < 0 then
                    i <- MAX - 1
                match data.[i] with
                | None -> ()
                | Some x -> s <- x :: s
                i <- i - 1
                remaining <- remaining - 1
            List.rev s

    type IBEvent =
        | IBEParsed of string // fileName
        | IBETypechecked of string // fileName
        | IBECreated

    // ++GLOBAL MUTABLE STATE FOR TESTING++
    let MRU = new FixedLengthMRU<IBEvent>()  
    let GetMostRecentIncrementalBuildEvents n = MRU.MostRecentList n
    let GetCurrentIncrementalBuildEventNum() = MRU.CurrentEventNum 

module Tc = FSharp.Compiler.TypeChecker


/// Accumulated results of type checking.
[<NoEquality; NoComparison>]
type TypeCheckAccumulator =
    { tcState: TcState
      tcImports: TcImports
      tcGlobals: TcGlobals
      tcConfig: TcConfig
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

      
/// Global service state
type FrameworkImportsCacheKey = (*resolvedpath*)string list * string * (*TargetFrameworkDirectories*)string list * (*fsharpBinaries*)string * (*langVersion*)decimal

/// Represents a cache of 'framework' references that can be shared between multiple incremental builds
type FrameworkImportsCache(keepStrongly) = 

    // Mutable collection protected via CompilationThreadToken 
    let frameworkTcImportsCache = AgedLookup<CompilationThreadToken, FrameworkImportsCacheKey, (TcGlobals * TcImports)>(keepStrongly, areSimilar=(fun (x, y) -> x = y)) 

    /// Reduce the size of the cache in low-memory scenarios
    member __.Downsize ctok = frameworkTcImportsCache.Resize(ctok, keepStrongly=0)

    /// Clear the cache
    member __.Clear ctok = frameworkTcImportsCache.Clear ctok

    /// This function strips the "System" assemblies from the tcConfig and returns a age-cached TcImports for them.
    member __.Get(ctok, tcConfig: TcConfig) =
      cancellable {
        // Split into installed and not installed.
        let frameworkDLLs, nonFrameworkResolutions, unresolved = TcAssemblyResolutions.SplitNonFoundationalResolutions(ctok, tcConfig)
        let frameworkDLLsKey = 
            frameworkDLLs 
            |> List.map (fun ar->ar.resolvedPath) // The cache key. Just the minimal data.
            |> List.sort  // Sort to promote cache hits.

        let! tcGlobals, frameworkTcImports = 
          cancellable {
            // Prepare the frameworkTcImportsCache
            //
            // The data elements in this key are very important. There should be nothing else in the TcConfig that logically affects
            // the import of a set of framework DLLs into F# CCUs. That is, the F# CCUs that result from a set of DLLs (including
            // FSharp.Core.dll and mscorlib.dll) must be logically invariant of all the other compiler configuration parameters.
            let key = (frameworkDLLsKey,
                        tcConfig.primaryAssembly.Name,
                        tcConfig.GetTargetFrameworkDirectories(),
                        tcConfig.fsharpBinariesDir,
                        tcConfig.langVersion.SpecifiedVersion)

            match frameworkTcImportsCache.TryGet (ctok, key) with
            | Some res -> return res
            | None -> 
                let tcConfigP = TcConfigProvider.Constant tcConfig
                let! ((tcGlobals, tcImports) as res) = TcImports.BuildFrameworkTcImports (ctok, tcConfigP, frameworkDLLs, nonFrameworkResolutions)
                frameworkTcImportsCache.Put(ctok, key, res)
                return tcGlobals, tcImports
          }
        return tcGlobals, frameworkTcImports, nonFrameworkResolutions, unresolved
      }


//------------------------------------------------------------------------------------
// Rules for reactive building.
//
// This phrases the compile as a series of vector functions and vector manipulations.
// Rules written in this language are then transformed into a plan to execute the 
// various steps of the process.
//-----------------------------------------------------------------------------------


/// Represents the interim state of checking an assembly
type PartialCheckResults = 
    { TcState: TcState 
      TcImports: TcImports 
      TcGlobals: TcGlobals 
      TcConfig: TcConfig 
      TcEnvAtEnd: TcEnv 

      /// Kept in a stack so that each incremental update shares storage with previous files
      TcErrorsRev: (PhasedDiagnostic * FSharpErrorSeverity)[] list 

      /// Kept in a stack so that each incremental update shares storage with previous files
      TcResolutionsRev: TcResolutions list 

      /// Kept in a stack so that each incremental update shares storage with previous files
      TcSymbolUsesRev: TcSymbolUses list 

      /// Kept in a stack so that each incremental update shares storage with previous files
      TcOpenDeclarationsRev: OpenDeclaration[] list

      /// Disambiguation table for module names
      ModuleNamesDict: ModuleNamesDict

      TcDependencyFiles: string list 

      TopAttribs: TopAttribs option

      TimeStamp: DateTime

      LatestImplementationFile: TypedImplFile option 

      LatestCcuSigForFile: ModuleOrNamespaceType option }

    member x.TcErrors  = Array.concat (List.rev x.TcErrorsRev)
    member x.TcSymbolUses  = List.rev x.TcSymbolUsesRev

    static member Create (tcAcc: TypeCheckAccumulator, timestamp) = 
        { TcState = tcAcc.tcState
          TcImports = tcAcc.tcImports
          TcGlobals = tcAcc.tcGlobals
          TcConfig = tcAcc.tcConfig
          TcEnvAtEnd = tcAcc.tcEnvAtEndOfFile
          TcErrorsRev = tcAcc.tcErrorsRev
          TcResolutionsRev = tcAcc.tcResolutionsRev
          TcSymbolUsesRev = tcAcc.tcSymbolUsesRev
          TcOpenDeclarationsRev = tcAcc.tcOpenDeclarationsRev
          TcDependencyFiles = tcAcc.tcDependencyFiles
          TopAttribs = tcAcc.topAttribs
          ModuleNamesDict = tcAcc.tcModuleNamesDict
          TimeStamp = timestamp 
          LatestImplementationFile = tcAcc.latestImplFile 
          LatestCcuSigForFile = tcAcc.latestCcuSigForFile }


[<AutoOpen>]
module Utilities = 
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
        member __.GetRawFSharpSignatureData(_m, _ilShortAssemName, _fileName) = sigData
        member __.GetRawFSharpOptimizationData(_m, _ilShortAssemName, _fileName) = [ ]
        member __.GetRawTypeForwarders() = mkILExportedTypes []  // TODO: cross-project references with type forwarders
        member __.ShortAssemblyName = assemblyName
        member __.ILScopeRef = IL.ILScopeRef.Assembly ilAssemRef
        member __.ILAssemblyRefs = [] // These are not significant for service scenarios
        member __.HasAnyFSharpSignatureDataAttribute =  true
        member __.HasMatchingFSharpSignatureDataAttribute _ilg = true

module IncrementalBuild =

    let mutable injectCancellationFault = false
    let locallyInjectCancellationFault() = 
        injectCancellationFault <- true
        { new IDisposable with member _.Dispose() =  injectCancellationFault <- false }

type SourceFile =
    {
        FileName: string
        SourceRange: range
        IsLastCompiland: (bool * bool)
    }

type ParsedInfo =
    {
        FileName: string
        SourceRange: range
        Input: Ast.ParsedInput option
        Errors: (PhasedDiagnostic * FSharpErrorSeverity)[]
    }

type SourceTypeCheckState =
    | NotParsed of SourceFile
    | Parsed of SourceFile * ParsedInfo
    | Checked of SourceFile * TypeCheckAccumulator * WeakReference<ParsedInfo>

    member x.SourceFile =
        match x with
        | NotParsed sourceFile -> sourceFile
        | Parsed (sourceFile, _) -> sourceFile
        | Checked (sourceFile, _, _) -> sourceFile

    member x.FileName = x.SourceFile.FileName

type TypeCheckOperation =
    | UseCache
    | ReTypeCheck

/// Manages an incremental build graph for the build of a single F# project
type IncrementalBuilder(tcGlobals, frameworkTcImports, nonFrameworkAssemblyInputs, nonFrameworkResolutions, unresolvedReferences, tcConfig: TcConfig, projectDirectory, outfile, 
                        assemblyName, niceNameGen: NiceNameGenerator, lexResourceManager, 
                        sourceFiles, loadClosureOpt: LoadClosure option, 
                        keepAssemblyContents, keepAllBackgroundResolutions, maxTimeShareMilliseconds) =

    let tcConfigP = TcConfigProvider.Constant tcConfig
    let fileParsed = new Event<string>()
    let beforeFileChecked = new Event<string>()
    let fileChecked = new Event<string>()
    let projectChecked = new Event<unit>()
#if !NO_EXTENSIONTYPING
    let importsInvalidatedByTypeProvider = new Event<string>()
#endif
    let mutable currentTcImportsOpt = None

    // Check for the existence of loaded sources and prepend them to the sources list if present.
    let sourceFiles = tcConfig.GetAvailableLoadedSources() @ (sourceFiles |>List.map (fun s -> rangeStartup, s))

    // Mark up the source files with an indicator flag indicating if they are the last source file in the project
    let sourceFiles = 
        let flags, isExe = tcConfig.ComputeCanContainEntryPoint(sourceFiles |> List.map snd)
        ((sourceFiles, flags) ||> List.map2 (fun (m, nm) flag -> (m, nm, (flag, isExe))))

    let defaultTimeStamp = DateTime.UtcNow

    let basicDependencies = 
        [ for (UnresolvedAssemblyReference(referenceText, _))  in unresolvedReferences do
            // Exclude things that are definitely not a file name
            if not(FileSystem.IsInvalidPathShim referenceText) then 
                let file = if FileSystem.IsPathRootedShim referenceText then referenceText else Path.Combine(projectDirectory, referenceText) 
                yield file 

          for r in nonFrameworkResolutions do 
                yield  r.resolvedPath  ]

    let allDependencies =
        [| yield! basicDependencies
           for (_, f, _) in sourceFiles do
                yield f |]

    //----------------------------------------------------
    // START OF BUILD TASK FUNCTIONS 
                
    /// This is a build task function that gets placed into the build rules as the computation for a VectorMap
    ///
    /// Parse the given file and return the given input.
    let parseTask ctok (sourceRange: range, fileName: string, isLastCompiland) =
        DoesNotRequireCompilerThreadTokenAndCouldPossiblyBeMadeConcurrent  ctok

        let errorLogger = CompilationErrorLogger("ParseTask", tcConfig.errorSeverityOptions)
        // Return the disposable object that cleans up
        use _holder = new CompilationGlobalsScope(errorLogger, BuildPhase.Parse)

        try  
            IncrementalBuilderEventTesting.MRU.Add(IncrementalBuilderEventTesting.IBEParsed fileName)
            let input = ParseOneInputFile(tcConfig, lexResourceManager, [], fileName, isLastCompiland, errorLogger, (*retryLocked*)true)
            fileParsed.Trigger fileName

            input, sourceRange, fileName, errorLogger.GetErrors ()
        with exn -> 
            let msg = sprintf "unexpected failure in IncrementalFSharpBuild.Parse\nerror = %s" (exn.ToString())
            System.Diagnostics.Debug.Assert(false, msg)
            failwith msg
         
    /// This is a build task function that gets placed into the build rules as the computation for a Vector.Demultiplex
    ///
    // Link all the assemblies together and produce the input typecheck accumulator               
    let combineImportedAssembliesTask ctok : Cancellable<TypeCheckAccumulator> =
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
                    let capturedImportsInvalidated = WeakReference<_>(importsInvalidatedByTypeProvider)
                    ccu.Deref.InvalidateEvent.Add(fun msg -> 
                        match capturedImportsInvalidated.TryGetTarget() with 
                        | true, tg -> tg.Trigger msg
                        | _ -> ()))  
#endif
                currentTcImportsOpt <- Some tcImports
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
        let tcAcc = 
            { tcGlobals=tcGlobals
              tcImports=tcImports
              tcState=tcState
              tcConfig=tcConfig
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
        return tcAcc }
                
    /// This is a build task function that gets placed into the build rules as the computation for a Vector.ScanLeft
    ///
    /// Type check all files.     
    let typeCheckTask ctok (tcAcc: TypeCheckAccumulator) input: Eventually<TypeCheckAccumulator> =    
        match input with 
        | Some input, _sourceRange, fileName, parseErrors->
            IncrementalBuilderEventTesting.MRU.Add(IncrementalBuilderEventTesting.IBETypechecked fileName)
            let capturingErrorLogger = CompilationErrorLogger("TypeCheckTask", tcConfig.errorSeverityOptions)
            let errorLogger = GetErrorLoggerFilteringByScopedPragmas(false, GetScopedPragmasForInput input, capturingErrorLogger)
            let fullComputation = 
                eventually {
                    beforeFileChecked.Trigger fileName

                    ApplyMetaCommandsFromInputToTcConfig (tcConfig, input, Path.GetDirectoryName fileName) |> ignore
                    let sink = TcResultsSinkImpl(tcAcc.tcGlobals)
                    let hadParseErrors = not (Array.isEmpty parseErrors)

                    let input, moduleNamesDict = DeduplicateParsedInputModuleName tcAcc.tcModuleNamesDict input

                    let! (tcEnvAtEndOfFile, topAttribs, implFile, ccuSigForFile), tcState = 
                        TypeCheckOneInputEventually 
                            ((fun () -> hadParseErrors || errorLogger.ErrorCount > 0), 
                             tcConfig, tcAcc.tcImports, 
                             tcAcc.tcGlobals, 
                             None, 
                             TcResultsSink.WithSink sink, 
                             tcAcc.tcState, input)
                        
                    /// Only keep the typed interface files when doing a "full" build for fsc.exe, otherwise just throw them away
                    let implFile = if keepAssemblyContents then implFile else None
                    let tcResolutions = if keepAllBackgroundResolutions then sink.GetResolutions() else TcResolutions.Empty
                    let tcEnvAtEndOfFile = (if keepAllBackgroundResolutions then tcEnvAtEndOfFile else tcState.TcEnvFromImpls)
                    let tcSymbolUses = sink.GetSymbolUses()  
                    
                    RequireCompilationThread ctok // Note: events get raised on the CompilationThread

                    fileChecked.Trigger fileName
                    let newErrors = Array.append parseErrors (capturingErrorLogger.GetErrors())
                    return {tcAcc with tcState=tcState 
                                       tcEnvAtEndOfFile=tcEnvAtEndOfFile
                                       topAttribs=Some topAttribs
                                       latestImplFile=implFile
                                       latestCcuSigForFile=Some ccuSigForFile
                                       tcResolutionsRev=tcResolutions :: tcAcc.tcResolutionsRev
                                       tcSymbolUsesRev=tcSymbolUses :: tcAcc.tcSymbolUsesRev
                                       tcOpenDeclarationsRev = sink.GetOpenDeclarations() :: tcAcc.tcOpenDeclarationsRev
                                       tcErrorsRev = newErrors :: tcAcc.tcErrorsRev 
                                       tcModuleNamesDict = moduleNamesDict
                                       tcDependencyFiles = fileName :: tcAcc.tcDependencyFiles } 
                }
                    
            // Run part of the Eventually<_> computation until a timeout is reached. If not complete, 
            // return a new Eventually<_> computation which recursively runs more of the computation.
            //   - When the whole thing is finished commit the error results sent through the errorLogger.
            //   - Each time we do real work we reinstall the CompilationGlobalsScope
            let timeSlicedComputation = 
                    fullComputation |> 
                        Eventually.repeatedlyProgressUntilDoneOrTimeShareOverOrCanceled 
                            maxTimeShareMilliseconds
                            CancellationToken.None
                            (fun ctok f -> 
                                // Reinstall the compilation globals each time we start or restart
                                use unwind = new CompilationGlobalsScope (errorLogger, BuildPhase.TypeCheck) 
                                f ctok)
                               
            timeSlicedComputation
        | _ -> 
            Eventually.Done tcAcc


    /// This is a build task function that gets placed into the build rules as the computation for a Vector.Demultiplex
    ///
    /// Finish up the typechecking to produce outputs for the rest of the compilation process
    let finalizeTypeCheckTask ctok (tcStates: TypeCheckAccumulator[]) = 
      cancellable {
        DoesNotRequireCompilerThreadTokenAndCouldPossiblyBeMadeConcurrent  ctok

        let errorLogger = CompilationErrorLogger("CombineImportedAssembliesTask", tcConfig.errorSeverityOptions)
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

    // END OF BUILD TASK FUNCTIONS
    // ---------------------------------------------------------------------------------------------

    let mutable initialTcAccCache = None
    let getInitialTcAcc ctok =      
        cancellable {
            match initialTcAccCache with
            | Some result -> return result
            | _ ->
                let! result = combineImportedAssembliesTask ctok
                initialTcAccCache <- Some result
                return result }
    
    let typeCheckCache =
        sourceFiles
        |> Array.ofList
        |> Array.map (fun (m, nm, isLastCompiland) -> 
            NotParsed { FileName = nm; SourceRange = m; IsLastCompiland = isLastCompiland })

    let checkSlot slot =
        if slot < 0 || slot >= typeCheckCache.Length then
            invalidArg "slot" "Invalid slot value"

    let parse ctok (sourceFile: SourceFile) slot =
        checkSlot slot

        let (input, sourceRange, fileName, errors) = parseTask ctok (sourceFile.SourceRange, sourceFile.FileName, sourceFile.IsLastCompiland)
        { FileName = fileName; SourceRange = sourceRange; Input = input; Errors = errors }

    let rec typeCheck ctok op slot =
        if IncrementalBuild.injectCancellationFault then Cancellable.canceled ()
        else

        checkSlot slot

        match typeCheckCache.[slot] with
        | NotParsed sourceFile ->
            cancellable {
                typeCheckCache.[slot] <- Parsed(sourceFile, parse ctok sourceFile slot)
                return! typeCheck ctok op slot }
        | Parsed(sourceFile, ({ FileName = fileName; SourceRange = sourceRange; Input = input; Errors = errors } as parsedInfo)) ->
            cancellable {
                let! ct = Cancellable.token ()
                let! priorTcAcc = priorTypeCheck ctok UseCache slot
                let tcAccTask = typeCheckTask ctok priorTcAcc (input, sourceRange, fileName, errors)
                let tcAccOpt = Eventually.forceWhile ctok (fun () -> not ct.IsCancellationRequested) tcAccTask
                match tcAccOpt with
                | Some tcAcc ->
                    typeCheckCache.[slot] <- Checked(sourceFile, tcAcc, WeakReference<_> parsedInfo)
                    return tcAcc
                | _ ->
                    if ct.IsCancellationRequested then
                        return! Cancellable.canceled ()
                    else
                        return invalidOp "Type checking was not canceled." }
        | Checked(sourceFile, tcAcc, weakInfo) ->
            match op with
            | UseCache -> Cancellable.ret tcAcc
            | ReTypeCheck ->
                match weakInfo.TryGetTarget() with
                | true, parsedInfo ->
                    match parsedInfo.Input with
                    | Some _ ->
                        typeCheckCache.[slot] <- Parsed (sourceFile, parsedInfo)
                    | _ ->
                        typeCheckCache.[slot] <- NotParsed sourceFile
                | _ ->
                    typeCheckCache.[slot] <- NotParsed sourceFile

                typeCheck ctok UseCache slot

    and priorTypeCheck ctok op slot =
        if IncrementalBuild.injectCancellationFault then Cancellable.canceled ()
        else

        match slot with
        | 0 (* first file *) -> getInitialTcAcc ctok
        | _ -> 
            cancellable {
                let slot = slot - 1
                checkSlot slot
                match typeCheckCache.[slot] with
                | Checked (_, tcAcc, _) -> return tcAcc
                | _ ->
                    // Find the first slot that has not been checked.
                    let startingSlot = typeCheckCache |> Array.findIndex (function Checked _ -> false | _ -> true)
                    let! tcAccs = cancellable { for i = startingSlot to slot do yield! typeCheck ctok op i }
                    if tcAccs.IsEmpty then
                        failwith "Should not happen. Expected at least one type-checked result."
                    return List.last tcAccs }

    let step ctok =
        cancellable {
            match typeCheckCache |> Array.tryFindIndex (function NotParsed _ | Parsed _ -> true | _ -> false) with
            | Some nextSlotToTypeCheck ->
                let! result = typeCheck ctok UseCache nextSlotToTypeCheck
                return Some result
            | _ ->
                return None }

    let isChecked slot =
        checkSlot slot

        match typeCheckCache.[slot] with
        | Checked _ -> true
        | _ -> false

    let invalidateSlot slot =
        checkSlot slot

        for i = slot to typeCheckCache.Length - 1 do
            typeCheckCache.[i] <- NotParsed typeCheckCache.[i].SourceFile

    let invalidateBuild () =
        initialTcAccCache <- None
        invalidateSlot 0

    let getSlot fileName =
        typeCheckCache 
        |> Array.findIndex (fun x ->
            let f2 = x.FileName
            String.Equals(fileName, f2, StringComparison.OrdinalIgnoreCase)
            || String.Equals(FileSystem.GetFullPathShim fileName, FileSystem.GetFullPathShim f2, StringComparison.OrdinalIgnoreCase))

    let getParseResults ctok slot =
        checkSlot slot

        match typeCheckCache.[slot] with
        | Parsed(_, parsedInfo) -> parsedInfo
        | NotParsed sourceFile ->
            let parsedInfo = parse ctok sourceFile slot
            typeCheckCache.[slot] <- Parsed(sourceFile, parse ctok sourceFile slot)
            parsedInfo
        | Checked(sourceFile, _, weakInfo) ->
            match weakInfo.TryGetTarget() with
            | true, parsedInfo -> parsedInfo
            | _ -> 
                let parsedInfo = parse ctok sourceFile slot
                weakInfo.SetTarget parsedInfo
                parsedInfo

    let sourceFileTimeStamps =
        sourceFiles
        |> Array.ofList
        |> Array.map (fun (_, nm, _) -> (nm, DateTime()))

    /// Checks source file timestamps.
    /// Source files can be invalidated by this call, which will cause re-type-checking.
    let checkSourceFileTimeStamps (cache: TimeStampCache) =
        sourceFileTimeStamps
        |> Array.iteri (fun slot (fileName, currentTimeStamp) ->
            let newTimeStamp = cache.GetFileTimeStamp fileName
            if newTimeStamp <> currentTimeStamp then
                invalidateSlot slot
                sourceFileTimeStamps.[slot] <- (fileName, newTimeStamp))

    let referenceAssemblyTimeStamps =
        nonFrameworkAssemblyInputs
        |> Array.ofList
        |> Array.map (fun (_, getTimeStamp) -> (getTimeStamp, DateTime()))

    /// Checks reference assembly timestamps.
    /// Build can be invalidated by this call.
    let checkReferenceAssemblyTimeStamps ctok (cache: TimeStampCache) =
        referenceAssemblyTimeStamps
        |> Array.iteri (fun i (getTimeStamp, currentTimeStamp) ->
            let newTimeStamp = getTimeStamp cache ctok
            if newTimeStamp <> currentTimeStamp then
                invalidateBuild ()
                referenceAssemblyTimeStamps.[i] <- (getTimeStamp, newTimeStamp))

    /// Checks source files and reference assembly timestamps.
    /// Source files and/or build can be invalidated by this call.
    let checkTimeStamps ctok cache =
        checkSourceFileTimeStamps cache
        checkReferenceAssemblyTimeStamps ctok cache

    let tryCreatePartialCheckResultsBeforeSlot slot =
        match slot, initialTcAccCache with
        | _, None -> None
        | 0, Some initialTcAcc ->
            Some(PartialCheckResults.Create(initialTcAcc, defaultTimeStamp))
        | _ -> 
            let slot = slot - 1
            checkSlot slot
            match typeCheckCache.[slot] with
            | Checked(_, tcAcc, _) ->
                Some(PartialCheckResults.Create(tcAcc, snd sourceFileTimeStamps.[slot]))
            | _ -> 
                None

    do IncrementalBuilderEventTesting.MRU.Add(IncrementalBuilderEventTesting.IBECreated)

    member _.TcConfig = tcConfig

    member _.FileParsed = fileParsed.Publish

    member _.BeforeFileChecked = beforeFileChecked.Publish

    member _.FileChecked = fileChecked.Publish

    member _.ProjectChecked = projectChecked.Publish

#if !NO_EXTENSIONTYPING
    member _.ImportsInvalidatedByTypeProvider = importsInvalidatedByTypeProvider.Publish
#endif

    member _.TryGetCurrentTcImports () = currentTcImportsOpt

    member _.AllDependenciesDeprecated = allDependencies

    member _.SlotCount = typeCheckCache.Length

    member _.Step (ctok: CompilationThreadToken) =  
      cancellable {
        let cache = TimeStampCache defaultTimeStamp // One per step
        checkTimeStamps ctok cache

        match! step ctok with
        | None ->
            projectChecked.Trigger()
            return false
        | Some _ ->
            return true }
    
    member _.GetCheckResultsBeforeFileInProjectEvenIfStale fileName: PartialCheckResults option =
        getSlot fileName
        |> tryCreatePartialCheckResultsBeforeSlot       
    
    member _.AreCheckResultsBeforeFileInProjectReady fileName = 
        match getSlot fileName with
        | 0 (* first file *) -> true
        | slot -> isChecked (slot - 1)
        
    member _.GetCheckResultsBeforeSlotInProject (ctok: CompilationThreadToken, slot) = 
      cancellable {
        let cache = TimeStampCache defaultTimeStamp
        checkTimeStamps ctok cache

        match tryCreatePartialCheckResultsBeforeSlot slot with
        | Some results -> return results
        | _ ->
            let! tcAcc = priorTypeCheck ctok UseCache slot
            match slot with
            | 0 -> 
                return PartialCheckResults.Create(tcAcc, defaultTimeStamp)
            | _ ->
                return PartialCheckResults.Create(tcAcc, snd sourceFileTimeStamps.[slot - 1]) }

    member builder.GetCheckResultsBeforeFileInProject (ctok: CompilationThreadToken, fileName) = 
        let slot = getSlot fileName
        builder.GetCheckResultsBeforeSlotInProject (ctok, slot)

    member builder.GetCheckResultsAfterFileInProject (ctok: CompilationThreadToken, fileName) = 
        let slot = getSlot fileName + 1
        builder.GetCheckResultsBeforeSlotInProject (ctok, slot)

    member builder.GetCheckResultsAfterLastFileInProject (ctok: CompilationThreadToken) = 
        builder.GetCheckResultsBeforeSlotInProject(ctok, builder.SlotCount) 

    member builder.GetCheckResultsAndImplementationsForProject(ctok: CompilationThreadToken) = 
      cancellable {
        let cache = TimeStampCache defaultTimeStamp       
        let timeStamp = builder.GetLogicalTimeStampForProject(cache, ctok)
        let! multipleTcAcc = 
            cancellable {
                for i = 0 to builder.SlotCount do
                    yield! priorTypeCheck ctok UseCache i }

        let! ilAssemRef, tcAssemblyDataOpt, tcAssemblyExprOpt, tcAcc = finalizeTypeCheckTask ctok (multipleTcAcc |> Array.ofList)
        return PartialCheckResults.Create(tcAcc, timeStamp), ilAssemRef, tcAssemblyDataOpt, tcAssemblyExprOpt }
        
    member _.GetLogicalTimeStampForProject(cache, ctok: CompilationThreadToken) = 
        checkTimeStamps ctok cache
        let (_, t1) = sourceFileTimeStamps |> Array.maxBy (fun (_, dt) -> dt)
        if referenceAssemblyTimeStamps.Length > 0 then
            let (_, t2) = referenceAssemblyTimeStamps |> Array.maxBy (fun (_, dt) -> dt)
            max t1 t2
        else
            t1
      
    member _.GetParseResultsForFile (ctok: CompilationThreadToken, fileName) =
      cancellable {
        let cache = TimeStampCache defaultTimeStamp 
        checkSourceFileTimeStamps cache

        let parsedInfo =
            getSlot fileName
            |> getParseResults ctok

        return (parsedInfo.Input, parsedInfo.SourceRange, parsedInfo.FileName, parsedInfo.Errors) }

    member _.SourceFiles  = sourceFiles  |> List.map (fun (_, f, _) -> f)

    /// CreateIncrementalBuilder (for background type checking). Note that fsc.fs also
    /// creates an incremental builder used by the command line compiler.
    static member TryCreateBackgroundBuilderForProjectOptions
                      (ctok, legacyReferenceResolver, defaultFSharpBinariesDir,
                       frameworkTcImportsCache: FrameworkImportsCache,
                       loadClosureOpt: LoadClosure option,
                       sourceFiles: string list,
                       commandLineArgs: string list,
                       projectReferences, projectDirectory,
                       useScriptResolutionRules, keepAssemblyContents,
                       keepAllBackgroundResolutions, maxTimeShareMilliseconds,
                       tryGetMetadataSnapshot, suggestNamesForErrors) =
      let useSimpleResolutionSwitch = "--simpleresolution"

      cancellable {

        // Trap and report warnings and errors from creation.
        let delayedLogger = CapturingErrorLogger("IncrementalBuilderCreation")
        use _unwindEL = PushErrorLoggerPhaseUntilUnwind (fun _ -> delayedLogger)
        use _unwindBP = PushThreadBuildPhaseUntilUnwind BuildPhase.Parameter

        let! builderOpt =
         cancellable {
          try

            // Create the builder.         
            // Share intern'd strings across all lexing/parsing
            let resourceManager = new Lexhelp.LexResourceManager() 

            /// Create a type-check configuration
            let tcConfigB, sourceFilesNew = 

                let getSwitchValue switchString =
                    match commandLineArgs |> Seq.tryFindIndex(fun s -> s.StartsWithOrdinal switchString) with
                    | Some idx -> Some(commandLineArgs.[idx].Substring(switchString.Length))
                    | _ -> None

                // see also fsc.fs: runFromCommandLineToImportingAssemblies(), as there are many similarities to where the PS creates a tcConfigB
                let tcConfigB = 
                    TcConfigBuilder.CreateNew(legacyReferenceResolver, 
                         defaultFSharpBinariesDir, 
                         implicitIncludeDir=projectDirectory, 
                         reduceMemoryUsage=ReduceMemoryFlag.Yes, 
                         isInteractive=useScriptResolutionRules, 
                         isInvalidationSupported=true, 
                         defaultCopyFSharpCore=CopyFSharpCoreFlag.No, 
                         tryGetMetadataSnapshot=tryGetMetadataSnapshot) 

                tcConfigB.resolutionEnvironment <- (ReferenceResolver.ResolutionEnvironment.EditingOrCompilation true)

                tcConfigB.conditionalCompilationDefines <- 
                    let define = if useScriptResolutionRules then "INTERACTIVE" else "COMPILED"
                    define :: tcConfigB.conditionalCompilationDefines

                tcConfigB.projectReferences <- projectReferences

                tcConfigB.useSimpleResolution <- (getSwitchValue useSimpleResolutionSwitch) |> Option.isSome

                // Apply command-line arguments and collect more source files if they are in the arguments
                let sourceFilesNew = ApplyCommandLineArgs(tcConfigB, sourceFiles, commandLineArgs)

                // Never open PDB files for the language service, even if --standalone is specified
                tcConfigB.openDebugInformationForLaterStaticLinking <- false

                tcConfigB.compilationThread <- 
                    { new ICompilationThread with 
                        member __.EnqueueWork work = 
                            Reactor.Singleton.EnqueueOp ("Unknown", "ICompilationThread.EnqueueWork", "work", fun ctok ->
                                work ctok
                            )
                    }

                tcConfigB, sourceFilesNew

            match loadClosureOpt with
            | Some loadClosure ->
                let dllReferences =
                    [for reference in tcConfigB.referencedDLLs do
                        // If there's (one or more) resolutions of closure references then yield them all
                        match loadClosure.References  |> List.tryFind (fun (resolved, _)->resolved=reference.Text) with
                        | Some (resolved, closureReferences) -> 
                            for closureReference in closureReferences do
                                yield AssemblyReference(closureReference.originalReference.Range, resolved, None)
                        | None -> yield reference]
                tcConfigB.referencedDLLs <- []
                // Add one by one to remove duplicates
                dllReferences |> List.iter (fun dllReference ->
                    tcConfigB.AddReferencedAssemblyByPath(dllReference.Range, dllReference.Text))
                tcConfigB.knownUnresolvedReferences <- loadClosure.UnresolvedReferences
            | None -> ()

            let tcConfig = TcConfig.Create(tcConfigB, validate=true)
            let niceNameGen = NiceNameGenerator()
            let outfile, _, assemblyName = tcConfigB.DecideNames sourceFilesNew

            // Resolve assemblies and create the framework TcImports. This is done when constructing the
            // builder itself, rather than as an incremental task. This caches a level of "system" references. No type providers are 
            // included in these references. 
            let! (tcGlobals, frameworkTcImports, nonFrameworkResolutions, unresolvedReferences) = frameworkTcImportsCache.Get(ctok, tcConfig)

            // Note we are not calling errorLogger.GetErrors() anywhere for this task. 
            // This is ok because not much can actually go wrong here.
            let errorOptions = tcConfig.errorSeverityOptions
            let errorLogger = CompilationErrorLogger("nonFrameworkAssemblyInputs", errorOptions)
            // Return the disposable object that cleans up
            use _holder = new CompilationGlobalsScope(errorLogger, BuildPhase.Parameter) 

            // Get the names and time stamps of all the non-framework referenced assemblies, which will act 
            // as inputs to one of the nodes in the build. 
            //
            // This operation is done when constructing the builder itself, rather than as an incremental task. 
            let nonFrameworkAssemblyInputs = 
                // Note we are not calling errorLogger.GetErrors() anywhere for this task. 
                // This is ok because not much can actually go wrong here.
                let errorLogger = CompilationErrorLogger("nonFrameworkAssemblyInputs", errorOptions)
                // Return the disposable object that cleans up
                use _holder = new CompilationGlobalsScope(errorLogger, BuildPhase.Parameter) 

                [ for r in nonFrameworkResolutions do
                    let fileName = r.resolvedPath
                    yield (Choice1Of2 fileName, (fun (cache: TimeStampCache) _ctok -> cache.GetFileTimeStamp fileName))  

                  for pr in projectReferences  do
                    yield Choice2Of2 pr, (fun (cache: TimeStampCache) ctok -> cache.GetProjectReferenceTimeStamp (pr, ctok)) ]
            
            let builder = 
                new IncrementalBuilder(tcGlobals, frameworkTcImports, nonFrameworkAssemblyInputs, nonFrameworkResolutions, unresolvedReferences, 
                                        tcConfig, projectDirectory, outfile, assemblyName, niceNameGen, 
                                        resourceManager, sourceFilesNew, loadClosureOpt, 
                                        keepAssemblyContents=keepAssemblyContents, 
                                        keepAllBackgroundResolutions=keepAllBackgroundResolutions, 
                                        maxTimeShareMilliseconds=maxTimeShareMilliseconds)
            return Some builder
          with e -> 
            errorRecoveryNoRange e
            return None
         }

        let diagnostics =
            match builderOpt with
            | Some builder ->
                let errorSeverityOptions = builder.TcConfig.errorSeverityOptions
                let errorLogger = CompilationErrorLogger("IncrementalBuilderCreation", errorSeverityOptions)
                delayedLogger.CommitDelayedDiagnostics errorLogger
                errorLogger.GetErrors() |> Array.map (fun (d, severity) -> d, severity = FSharpErrorSeverity.Error)
            | _ ->
                Array.ofList delayedLogger.Diagnostics
            |> Array.map (fun (d, isError) -> FSharpErrorInfo.CreateFromException(d, isError, range.Zero, suggestNamesForErrors))

        return builderOpt, diagnostics
      }
