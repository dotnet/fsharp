// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.CodeAnalysis

open System
open System.Collections.Generic
open System.Diagnostics
open System.IO
open Internal.Utilities.Library
open Internal.Utilities.Collections
open FSharp.Compiler
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILBinaryReader
open FSharp.Compiler.CheckBasics
open FSharp.Compiler.CheckDeclarations
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.CompilerDiagnostics
open FSharp.Compiler.CompilerImports
open FSharp.Compiler.CompilerOptions
open FSharp.Compiler.CreateILModule
open FSharp.Compiler.DependencyManager
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.EditorServices
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.IO
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.NameResolution
open FSharp.Compiler.ParseAndCheckInputs
open FSharp.Compiler.ScriptClosure
open FSharp.Compiler.Syntax
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Range
open FSharp.Compiler.Xml
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.BuildGraph

[<AutoOpen>]
module internal IncrementalBuild =

    let mutable injectCancellationFault = false
    let LocallyInjectCancellationFault() =
        injectCancellationFault <- true
        { new IDisposable with member _.Dispose() = injectCancellationFault <- false }

// Record the most recent IncrementalBuilder events, so we can more easily unit test/debug the
// 'incremental' behavior of the product.
module IncrementalBuilderEventTesting =

    type internal FixedLengthMRU<'T>() =
        let MAX = 400   // Length of the MRU.  For our current unit tests, 400 is enough.
        let data = Array.create MAX None
        let mutable curIndex = 0
        let mutable numAdds = 0

        // called by the product, to note when a parse/typecheck happens for a file
        member _.Add(fileName:'T) =
            numAdds <- numAdds + 1
            data[curIndex] <- Some fileName
            curIndex <- (curIndex + 1) % MAX

        member _.CurrentEventNum = numAdds
        // called by unit tests, returns 'n' most recent additions.

        member _.MostRecentList(n: int) : 'T list =
            if n < 0 || n > MAX then
                raise <| ArgumentOutOfRangeException("n", sprintf "n must be between 0 and %d, inclusive, but got %d" MAX n)
            let mutable remaining = n
            let mutable s = []
            let mutable i = curIndex - 1
            while remaining <> 0 do
                if i < 0 then
                    i <- MAX - 1
                match data[i] with
                | None -> ()
                | Some x -> s <- x :: s
                i <- i - 1
                remaining <- remaining - 1
            List.rev s

    type IBEvent =
        | IBEParsed of fileName: string
        | IBETypechecked of fileName: string
        | IBECreated

    // ++GLOBAL MUTABLE STATE FOR TESTING++
    let MRU = FixedLengthMRU<IBEvent>()
    let GetMostRecentIncrementalBuildEvents n = MRU.MostRecentList n
    let GetCurrentIncrementalBuildEventNum() = MRU.CurrentEventNum

module Tc = CheckExpressions

type internal FSharpFile = {
        Name: string
        HasSignature: bool
        Range: range
        Source: FSharpSource
        Flags: bool * bool
    }
 
// This module is only here to contain the SyntaxTree type as to avoid ambiguity with the module FSharp.Compiler.Syntax.
[<AutoOpen>]
module IncrementalBuildSyntaxTree =

    type ParseResult = ParsedInput * range * string * (PhasedDiagnostic * FSharpDiagnosticSeverity) array

    /// Information needed to lazily parse a file to get a ParsedInput. Internally uses GraphNodes to cache results.
    [<Sealed>]
    type SyntaxTree (
            tcConfig: TcConfig,
            fileParsed: Event<string>,
            lexResourceManager,
            file: FSharpFile,
            timeStamp: DateTime
        ) =

        let source = file.Source
        let isLastCompiland = file.Flags

        let skippedImplFilePlaceholder sigName =
            ParsedInput.ImplFile(
                ParsedImplFileInput(
                    file.Name,
                    false,
                    sigName,
                    [],
                    [],
                    [],
                    isLastCompiland,
                    { ConditionalDirectives = []; CodeComments = [] },
                    Set.empty
                )
            )

        let parse (source: FSharpSource) =
            node {
                let fileName = source.FilePath
                IncrementalBuilderEventTesting.MRU.Add(IncrementalBuilderEventTesting.IBEParsed fileName)
                use _ =
                    Activity.start "IncrementalBuildSyntaxTree.parse"
                        [|
                            Activity.Tags.fileName, fileName
                            Activity.Tags.buildPhase, BuildPhase.Parse.ToString()
                        |]

                try
                    let diagnosticsLogger = CompilationDiagnosticLogger("Parse", tcConfig.diagnosticsOptions)
                    // Return the disposable object that cleans up
                    use _holder = new CompilationGlobalsScope(diagnosticsLogger, BuildPhase.Parse)
                    use! text = source.GetTextContainer() |> NodeCode.AwaitAsync
                    let input =
                        match text :?> TextContainer with
                        | TextContainer.Stream(stream) ->
                            ParseOneInputStream(tcConfig, lexResourceManager, fileName, isLastCompiland, diagnosticsLogger, false, stream)
                        | TextContainer.SourceText(sourceText) ->
                            ParseOneInputSourceText(tcConfig, lexResourceManager, fileName, isLastCompiland, diagnosticsLogger, sourceText)
                        | TextContainer.OnDisk ->
                            ParseOneInputFile(tcConfig, lexResourceManager, fileName, isLastCompiland, diagnosticsLogger, true)

                    fileParsed.Trigger fileName

                    return input, file.Range, fileName, diagnosticsLogger.GetDiagnostics()
                with exn ->
                    let msg = sprintf "unexpected failure in SyntaxTree.parse\nerror = %s" (exn.ToString())
                    System.Diagnostics.Debug.Assert(false, msg)
                    failwith msg
                    return Unchecked.defaultof<_>
            }

        /// Parse the source and cache the result in a GraphNode.
        member val ParseNode : GraphNode<ParseResult> = parse source |> GraphNode

        member _.Invalidate(timeStamp) =
            SyntaxTree(tcConfig, fileParsed, lexResourceManager, file, timeStamp)

        member _.Skip = skippedImplFilePlaceholder

        member _.FileName = file.Name

        member _.HasSignature = file.HasSignature

        member _.TimeStamp = timeStamp

/// Accumulated results of type checking. The minimum amount of state in order to continue type-checking following files.
[<NoEquality; NoComparison>]
type TcInfo =
    {
        tcState: TcState
        tcEnvAtEndOfFile: TcEnv

        /// Disambiguation table for module names
        moduleNamesDict: ModuleNamesDict

        topAttribs: TopAttribs option

        latestCcuSigForFile: ModuleOrNamespaceType option

        /// Accumulated diagnostics, last file first
        tcDiagnosticsRev:(PhasedDiagnostic * FSharpDiagnosticSeverity)[] list

        tcDependencyFiles: string list

        sigNameOpt: (string * QualifiedNameOfFile) option
    }

    member x.TcDiagnostics =
        Array.concat (List.rev x.tcDiagnosticsRev)

/// Accumulated results of type checking. Optional data that isn't needed to type-check a file, but needed for more information for in tooling.
[<NoEquality; NoComparison>]
type TcInfoExtras =
    {
      tcResolutions: TcResolutions
      tcSymbolUses: TcSymbolUses
      tcOpenDeclarations: OpenDeclaration[]

      /// Result of checking most recent file, if any
      latestImplFile: CheckedImplFile option

      /// If enabled, stores a linear list of ranges and strings that identify an Item(symbol) in a file. Used for background find all references.
      itemKeyStore: ItemKeyStore option

      /// If enabled, holds semantic classification information for Item(symbol)s in a file.
      semanticClassificationKeyStore: SemanticClassificationKeyStore option
    }

    member x.TcSymbolUses =
        x.tcSymbolUses

module ValueOption =
    let toOption = function
        | ValueSome x -> Some x
        | _ -> None

type private SingleFileDiagnostics = (PhasedDiagnostic * FSharpDiagnosticSeverity) array

/// Represents the interim state of checking an assembly
[<Sealed>]
type internal PartialCheckResults(
        tcInfo: GraphNode<TcInfo>,
        diags: GraphNode<SingleFileDiagnostics>,
        tcInfoExtras: GraphNode<TcInfoExtras>,
        timeStamp: DateTime,
        tcConfig,
        tcGlobals,
        tcImports
    ) =
    member _.TcInfo = tcInfo

    member _.Diags = diags

    /// Max timestamp of the inputs so far.
    member _.TimeStamp = timeStamp

    member _.TcConfig = tcConfig

    member _.TcGlobals = tcGlobals

    member _.TcImports = tcImports

    member _.TryPeekTcInfo() = tcInfo.TryPeekValue() |> ValueOption.toOption
    
    member _.TryPeekTcInfoWithExtras() = 
        (tcInfo.TryPeekValue(), tcInfoExtras.TryPeekValue())
        ||> ValueOption.map2 (fun a b -> a, b)
        |> ValueOption.toOption
    
    member _.GetOrComputeTcInfo() = tcInfo.GetOrComputeValue()
    
    member _.GetOrComputeTcInfoExtras() = tcInfoExtras.GetOrComputeValue()
    
    member _.GetOrComputeTcInfoWithExtras() = node {
        let! tcInfo = tcInfo.GetOrComputeValue()
        let! tcInfoExtras = tcInfoExtras.GetOrComputeValue()
        return tcInfo, tcInfoExtras
    }

    member _.GetOrComputeItemKeyStoreIfEnabled() =
        node {
            let! info = tcInfoExtras.GetOrComputeValue()
            return info.itemKeyStore
        }

    member _.GetOrComputeSemanticClassificationIfEnabled() =
        node {
            let! info = tcInfoExtras.GetOrComputeValue()
            return info.semanticClassificationKeyStore
        }

    member _.Finalize(finalTcInfo) =
        PartialCheckResults(GraphNode.FromResult finalTcInfo, GraphNode.FromResult [||], tcInfoExtras, timeStamp, tcConfig, tcGlobals, tcImports)

type private TypeCheck = TcInfo * TcResultsSinkImpl * CheckedImplFile option * string * SingleFileDiagnostics


type PartialCheckResultsBuilder(
    tcImports: TcImports,
    tcConfig: TcConfig,
    tcGlobals,
    enableBackgroundItemKeyStoreAndSemanticClassification,
    keepAssemblyContents,
    keepAllBackgroundResolutions,
    keepAllBackgroundSymbolUses,
    beforeFileChecked: Event<string>,
    fileChecked: Event<string>
    ) =
    let getTcInfoExtras (typeCheck: GraphNode<TypeCheck>) =
        node {
            let! _ , sink, implFile, fileName, _ = typeCheck.GetOrComputeValue()
            // Build symbol keys
            let itemKeyStore, semanticClassification =
                if enableBackgroundItemKeyStoreAndSemanticClassification then
                    use _ = Activity.start "IncrementalBuild.CreateItemKeyStoreAndSemanticClassification" [|Activity.Tags.fileName, fileName|]
                    let sResolutions = sink.GetResolutions()
                    let builder = ItemKeyStoreBuilder()
                    let preventDuplicates = HashSet({ new IEqualityComparer<struct(pos * pos)> with
                                                        member _.Equals((s1, e1): struct(pos * pos), (s2, e2): struct(pos * pos)) = Position.posEq s1 s2 && Position.posEq e1 e2
                                                        member _.GetHashCode o = o.GetHashCode() })
                    sResolutions.CapturedNameResolutions
                    |> Seq.iter (fun cnr ->
                        let r = cnr.Range
                        if preventDuplicates.Add struct(r.Start, r.End) then
                            builder.Write(cnr.Range, cnr.Item))
                    
                    let semanticClassification = sResolutions.GetSemanticClassification(tcGlobals, tcImports.GetImportMap(), sink.GetFormatSpecifierLocations(), None)
                    
                    let sckBuilder = SemanticClassificationKeyStoreBuilder()
                    sckBuilder.WriteAll semanticClassification
                    
                    let res = builder.TryBuildAndReset(), sckBuilder.TryBuildAndReset()
                    res
                else
                    None, None
            return
                {
                    // Only keep the typed interface files when doing a "full" build for fsc.exe, otherwise just throw them away
                    latestImplFile = if keepAssemblyContents then implFile else None
                    tcResolutions = (if keepAllBackgroundResolutions then sink.GetResolutions() else TcResolutions.Empty)
                    tcSymbolUses = (if keepAllBackgroundSymbolUses then sink.GetSymbolUses() else TcSymbolUses.Empty)
                    tcOpenDeclarations = sink.GetOpenDeclarations()
                    itemKeyStore = itemKeyStore
                    semanticClassificationKeyStore = semanticClassification
                }
        } |> GraphNode

 
    let getTypeCheck (prevTcInfo: GraphNode<TcInfo>) (syntaxTree: SyntaxTree) : NodeCode<TypeCheck> =
        node {
            let! prevTcInfo = prevTcInfo.GetOrComputeValue()
            let! input, _sourceRange, fileName, parseErrors = syntaxTree.ParseNode.GetOrComputeValue()
            use _ = Activity.start "BoundModel.TypeCheck" [|Activity.Tags.fileName, fileName|]

            IncrementalBuilderEventTesting.MRU.Add(IncrementalBuilderEventTesting.IBETypechecked fileName)
            let capturingDiagnosticsLogger = CapturingDiagnosticsLogger("TypeCheck")
            let diagnosticsLogger = GetDiagnosticsLoggerFilteringByScopedPragmas(false, input.ScopedPragmas, tcConfig.diagnosticsOptions, capturingDiagnosticsLogger)
            use _ = new CompilationGlobalsScope(diagnosticsLogger, BuildPhase.TypeCheck)

            beforeFileChecked.Trigger fileName
                        
            ApplyMetaCommandsFromInputToTcConfig (tcConfig, input, Path.GetDirectoryName fileName, tcImports.DependencyProvider) |> ignore
            let sink = TcResultsSinkImpl(tcGlobals)
            let hadParseErrors = not (Array.isEmpty parseErrors)
            let input, moduleNamesDict = DeduplicateParsedInputModuleName prevTcInfo.moduleNamesDict input

            let! (tcEnvAtEndOfFile, topAttribs, implFile, ccuSigForFile), tcState =
                CheckOneInput (
                        (fun () -> hadParseErrors || diagnosticsLogger.ErrorCount > 0),
                        tcConfig, tcImports,
                        tcGlobals,
                        None,
                        TcResultsSink.WithSink sink,
                        prevTcInfo.tcState, input )
                |> NodeCode.FromCancellable

            fileChecked.Trigger fileName

            let newErrors = Array.append parseErrors (capturingDiagnosticsLogger.Diagnostics |> List.toArray)
            let tcEnvAtEndOfFile = if keepAllBackgroundResolutions then tcEnvAtEndOfFile else tcState.TcEnvFromImpls

            let tcInfo =
                {
                    tcState = tcState
                    tcEnvAtEndOfFile = tcEnvAtEndOfFile
                    moduleNamesDict = moduleNamesDict
                    latestCcuSigForFile = Some ccuSigForFile
                    tcDiagnosticsRev = newErrors :: prevTcInfo.tcDiagnosticsRev
                    topAttribs = Some topAttribs
                    tcDependencyFiles = fileName :: prevTcInfo.tcDependencyFiles
                    sigNameOpt =
                        match input with
                        | ParsedInput.SigFile sigFile ->
                            Some(sigFile.FileName, sigFile.QualifiedName)
                        | _ ->
                            None
                }
            return tcInfo, sink, implFile, fileName, newErrors
        }

    let skippedImplemetationTypeCheck prevTcInfo (syntaxTree: SyntaxTree) =
        node {
            match prevTcInfo.sigNameOpt with
            | Some (_, qualifiedName) when syntaxTree.HasSignature ->
                let input = syntaxTree.Skip qualifiedName
                let fileName = syntaxTree.FileName
                return
                    SkippedImplFilePlaceholder(tcConfig, tcImports, tcGlobals, prevTcInfo.tcState, input)
                    |> Option.map (fun ((_, topAttribs, _, ccuSigForFile), tcState) ->
                            {
                                tcState = tcState
                                tcEnvAtEndOfFile = tcState.TcEnvFromImpls
                                moduleNamesDict = prevTcInfo.moduleNamesDict
                                latestCcuSigForFile = Some ccuSigForFile
                                tcDiagnosticsRev = prevTcInfo.tcDiagnosticsRev
                                topAttribs = Some topAttribs
                                tcDependencyFiles = fileName :: prevTcInfo.tcDependencyFiles
                                sigNameOpt = Some(fileName, qualifiedName)
                            })
            | _ -> return None
        }

    member _.Create(tcInfo, diags, tcInfoExtras, timeStamp)  =
        PartialCheckResults(tcInfo, diags, tcInfoExtras, timeStamp, tcConfig, tcGlobals, tcImports)

    /// Return TcInfoExtras node for initial PartialCheckResults.
    member _.DefaultTcInfoExtras(tcInfo) =
        let defaultTypeCheck = tcInfo, TcResultsSinkImpl(tcGlobals), None, "default typecheck - no syntaxTree", [||]
        getTcInfoExtras (GraphNode.FromResult defaultTypeCheck)

    /// Return PartialCheckResults for next file in compilation order.
    member _.Next(priorCheckresults: PartialCheckResults, syntaxTree: SyntaxTree) =

        // Pass on dependecies time stamp if newer.
        // Implementation files with backing signatures do not contribute to the partial check timestamp.
        let timeStamp =
            if syntaxTree.HasSignature then priorCheckresults.TimeStamp else max syntaxTree.TimeStamp priorCheckresults.TimeStamp

        let typeCheckNode = getTypeCheck priorCheckresults.TcInfo syntaxTree |> GraphNode

        let tcInfoExtras = getTcInfoExtras typeCheckNode

        let diags =
            node {
                let! _, _, _, _, diags = typeCheckNode.GetOrComputeValue()
                return diags
            } |> GraphNode

        let startComputingExtras =
            node {
                let! _ = tcInfoExtras.GetOrComputeValue()
                return! diags.GetOrComputeValue()      
            }

        let tcInfo =
            node {
                let! prevTcInfo = priorCheckresults.TcInfo.GetOrComputeValue()
                match! skippedImplemetationTypeCheck prevTcInfo syntaxTree with
                | Some tcInfo -> return tcInfo
                | _ -> 
                    let! tcInfo , _, _, _, _ = typeCheckNode.GetOrComputeValue()
                    // typeCheckNode holds type check results, which can be big.
                    // We want it to get garbage collected soon, that's why we also start computing extras in parallel here.
                    // Once this is computed, the intermediate type check results can be freed from memory.
                    startComputingExtras |> Async.AwaitNodeCode |> Async.Ignore |> Async.Start
                    return tcInfo
            } |> GraphNode

        PartialCheckResults(tcInfo, diags, tcInfoExtras, timeStamp, tcConfig, tcGlobals, tcImports)

/// Global service state
type FrameworkImportsCacheKey = FrameworkImportsCacheKey of resolvedpath: string list * assemblyName: string * targetFrameworkDirectories: string list * fsharpBinaries: string * langVersion: decimal

/// Represents a cache of 'framework' references that can be shared between multiple incremental builds
type FrameworkImportsCache(size) =

    let gate = obj()

    // Mutable collection protected via CompilationThreadToken
    let frameworkTcImportsCache = AgedLookup<AnyCallerThreadToken, FrameworkImportsCacheKey, GraphNode<TcGlobals * TcImports>>(size, areSimilar=(fun (x, y) -> x = y))

    /// Reduce the size of the cache in low-memory scenarios
    member _.Downsize() = frameworkTcImportsCache.Resize(AnyCallerThread, newKeepStrongly=0)

    /// Clear the cache
    member _.Clear() = frameworkTcImportsCache.Clear AnyCallerThread

    /// This function strips the "System" assemblies from the tcConfig and returns a age-cached TcImports for them.
    member _.GetNode(tcConfig: TcConfig, frameworkDLLs: AssemblyResolution list, nonFrameworkResolutions: AssemblyResolution list) =
        let frameworkDLLsKey =
            frameworkDLLs
            |> List.map (fun ar->ar.resolvedPath) // The cache key. Just the minimal data.
            |> List.sort  // Sort to promote cache hits.

        // Prepare the frameworkTcImportsCache
        //
        // The data elements in this key are very important. There should be nothing else in the TcConfig that logically affects
        // the import of a set of framework DLLs into F# CCUs. That is, the F# CCUs that result from a set of DLLs (including
        // FSharp.Core.dll and mscorlib.dll) must be logically invariant of all the other compiler configuration parameters.
        let key =
            FrameworkImportsCacheKey(frameworkDLLsKey,
                    tcConfig.primaryAssembly.Name,
                    tcConfig.GetTargetFrameworkDirectories(),
                    tcConfig.fsharpBinariesDir,
                    tcConfig.langVersion.SpecifiedVersion)

        let node =
            lock gate (fun () ->
                match frameworkTcImportsCache.TryGet (AnyCallerThread, key) with
                | Some lazyWork -> lazyWork
                | None ->
                    let lazyWork = GraphNode(node {
                        let tcConfigP = TcConfigProvider.Constant tcConfig
                        return! TcImports.BuildFrameworkTcImports (tcConfigP, frameworkDLLs, nonFrameworkResolutions)
                    })
                    frameworkTcImportsCache.Put(AnyCallerThread, key, lazyWork)
                    lazyWork
            )
        node
                

    /// This function strips the "System" assemblies from the tcConfig and returns a age-cached TcImports for them.
    member this.Get(tcConfig: TcConfig) =
      node {
        // Split into installed and not installed.
        let frameworkDLLs, nonFrameworkResolutions, unresolved = TcAssemblyResolutions.SplitNonFoundationalResolutions(tcConfig)
        let node = this.GetNode(tcConfig, frameworkDLLs, nonFrameworkResolutions)
        let! tcGlobals, frameworkTcImports = node.GetOrComputeValue()
        return tcGlobals, frameworkTcImports, nonFrameworkResolutions, unresolved
      }

[<AutoOpen>]
module Utilities =
    let TryFindFSharpStringAttribute tcGlobals attribSpec attribs =
        match TryFindFSharpAttribute tcGlobals attribSpec attribs with
        | Some (Attrib(_, _, [ AttribStringArg s ], _, _, _, _))  -> Some s
        | _ -> None

/// The implementation of the information needed by TcImports in CompileOps.fs for an F# assembly reference.
///
/// Constructs the build data (IRawFSharpAssemblyData) representing the assembly when used
/// as a cross-assembly reference.  Note the assembly has not been generated on disk, so this is
/// a virtualized view of the assembly contents as computed by background checking.
type RawFSharpAssemblyDataBackedByLanguageService (tcConfig, tcGlobals, generatedCcu: CcuThunk, outfile, topAttrs, assemblyName, ilAssemRef) =

    let exportRemapping = MakeExportRemapping generatedCcu generatedCcu.Contents

    let sigData =
        let _sigDataAttributes, sigDataResources = EncodeSignatureData(tcConfig, tcGlobals, exportRemapping, generatedCcu, outfile, true)
        [ for r in sigDataResources  do
            GetResourceNameAndSignatureDataFunc r
        ]

    let autoOpenAttrs = topAttrs.assemblyAttrs |> List.choose (List.singleton >> TryFindFSharpStringAttribute tcGlobals tcGlobals.attrib_AutoOpenAttribute)

    let ivtAttrs = topAttrs.assemblyAttrs |> List.choose (List.singleton >> TryFindFSharpStringAttribute tcGlobals tcGlobals.attrib_InternalsVisibleToAttribute)

    interface IRawFSharpAssemblyData with
        member _.GetAutoOpenAttributes() = autoOpenAttrs
        member _.GetInternalsVisibleToAttributes() =  ivtAttrs
        member _.TryGetILModuleDef() = None
        member _.GetRawFSharpSignatureData(_m, _ilShortAssemName, _filename) = sigData
        member _.GetRawFSharpOptimizationData(_m, _ilShortAssemName, _filename) = [ ]
        member _.GetRawTypeForwarders() = mkILExportedTypes []  // TODO: cross-project references with type forwarders
        member _.ShortAssemblyName = assemblyName
        member _.ILScopeRef = ILScopeRef.Assembly ilAssemRef
        member _.ILAssemblyRefs = [] // These are not significant for service scenarios
        member _.HasAnyFSharpSignatureDataAttribute =  true
        member _.HasMatchingFSharpSignatureDataAttribute = true

[<AutoOpen>]
module IncrementalBuilderHelpers =

    /// Timestamps of referenced assemblies are taken from the file's timestamp.
    let StampReferencedAssemblyTask (cache: TimeStampCache) (_ref, timeStamper) =
        timeStamper cache

    // Link all the assemblies together and produce the input typecheck accumulator
    let CombineImportedAssembliesTask (
        tcConfig: TcConfig, 
        frameworkTcImports, 
        nonFrameworkResolutions, 
        unresolvedReferences, 
        dependencyProvider
#if !NO_TYPEPROVIDERS
        ,importsInvalidatedByTypeProvider: Event<unit>
#endif
        ) =

      node {
        let diagnosticsLogger = CompilationDiagnosticLogger("CombineImportedAssembliesTask", tcConfig.diagnosticsOptions)
        use _ = new CompilationGlobalsScope(diagnosticsLogger, BuildPhase.Parameter)

        let! tcImports =
          node {
            try
                let! tcImports = TcImports.BuildNonFrameworkTcImports(TcConfigProvider.Constant tcConfig, frameworkTcImports, nonFrameworkResolutions, unresolvedReferences, dependencyProvider)
#if !NO_TYPEPROVIDERS
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
                    ccu.Deref.InvalidateEvent.Add(fun _ ->
                        match capturedImportsInvalidated.TryGetTarget() with
                        | true, tg -> tg.Trigger()
                        | _ -> ()))
#endif
                return tcImports
            with exn ->
                Debug.Assert(false, sprintf "Could not BuildAllReferencedDllTcImports %A" exn)
                diagnosticsLogger.Warning exn
                return frameworkTcImports
          }

        return tcImports, diagnosticsLogger.GetDiagnostics()
      }

    let GetInitialTcInfo (tcConfig: TcConfig) assemblyName (loadClosureOpt: LoadClosure option) basicDependencies tcGlobals tcImports diagnostics =
        let diagnosticsLogger = CompilationDiagnosticLogger("InitialTcInfoTask", tcConfig.diagnosticsOptions)
        use _ = new CompilationGlobalsScope(diagnosticsLogger, BuildPhase.Parameter)
        let tcInitial, openDecls0 = GetInitialTcEnv (assemblyName, rangeStartup, tcConfig, tcImports, tcGlobals)
        let tcState = GetInitialTcState (rangeStartup, assemblyName, tcConfig, tcGlobals, tcImports, tcInitial, openDecls0)
        let loadClosureErrors =
           [ match loadClosureOpt with
             | None -> ()
             | Some loadClosure ->
                for inp in loadClosure.Inputs do
                    yield! inp.MetaCommandDiagnostics ]

        let initialErrors = [| yield! loadClosureErrors; yield! diagnosticsLogger.GetDiagnostics(); yield! diagnostics |]
        let tcInfo =
            {
              tcState = tcState
              tcEnvAtEndOfFile = tcInitial
              topAttribs = None
              latestCcuSigForFile = None
              tcDiagnosticsRev = [ initialErrors ]
              moduleNamesDict = Map.empty
              tcDependencyFiles = basicDependencies
              sigNameOpt = None
            }
        tcInfo

    /// Finish up the typechecking to produce outputs for the rest of the compilation process
    let FinalizeTypeCheckTask (tcConfig: TcConfig) tcGlobals partialCheck assemblyName outfile (initialCheckResults: PartialCheckResults) (partialCheckResults: PartialCheckResults seq) =
      node {
        let diagnosticsLogger = CompilationDiagnosticLogger("FinalizeTypeCheckTask", tcConfig.diagnosticsOptions)
        use _ = new CompilationGlobalsScope(diagnosticsLogger, BuildPhase.TypeCheck)

        let! tcInfos =
            partialCheckResults
            |> Seq.map (fun result -> node { return! result.GetOrComputeTcInfo() })
            |> NodeCode.Sequential

        // tcInfoExtras can be computed in parallel. This will check any previously skipped implementation files in parallel, too.
        let! latestImplFiles =
            partialCheckResults
            |> Seq.map (fun result -> node {
                    if partialCheck then
                        return None
                    else
                        let! tcInfoExtras = result.GetOrComputeTcInfoExtras()
                        return tcInfoExtras.latestImplFile
                })
            |> NodeCode.Parallel

        let results = [
            for tcInfo, latestImplFile in Seq.zip tcInfos latestImplFiles ->
                tcInfo.tcEnvAtEndOfFile, defaultArg tcInfo.topAttribs EmptyTopAttrs, latestImplFile, tcInfo.latestCcuSigForFile
        ]

        // Get the state at the end of the type-checking of the last file
        let finalInfo = Array.last tcInfos

        // Finish the checking
        let (_tcEnvAtEndOfLastFile, topAttrs, mimpls, _), tcState =
            CheckMultipleInputsFinish (results, finalInfo.tcState)

        let ilAssemRef, tcAssemblyDataOpt, tcAssemblyExprOpt =
            try
                let tcState, tcAssemblyExpr, ccuContents = CheckClosedInputSetFinish (mimpls, tcState)

                let generatedCcu = tcState.Ccu.CloneWithFinalizedContents(ccuContents)

                // Compute the identity of the generated assembly based on attributes, options etc.
                // Some of this is duplicated from fsc.fs
                let ilAssemRef =
                    let publicKey =
                        try
                            let signingInfo = ValidateKeySigningAttributes (tcConfig, tcGlobals, topAttrs)
                            match GetStrongNameSigner signingInfo with
                            | None -> None
                            | Some s -> Some (PublicKey.KeyAsToken(s.PublicKey))
                        with exn ->
                            errorRecoveryNoRange exn
                            None
                    let locale = TryFindFSharpStringAttribute tcGlobals (tcGlobals.FindSysAttrib "System.Reflection.AssemblyCultureAttribute") topAttrs.assemblyAttrs
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
                            ProjectAssemblyDataResult.Unavailable true
                        else
                            ProjectAssemblyDataResult.Available (RawFSharpAssemblyDataBackedByLanguageService (tcConfig, tcGlobals, generatedCcu, outfile, topAttrs, assemblyName, ilAssemRef) :> IRawFSharpAssemblyData)
                    with exn ->
                        errorRecoveryNoRange exn
                        ProjectAssemblyDataResult.Unavailable true
                ilAssemRef, tcAssemblyDataOpt, Some tcAssemblyExpr
            with exn ->
                errorRecoveryNoRange exn
                mkSimpleAssemblyRef assemblyName, ProjectAssemblyDataResult.Unavailable true, None

        let finalCheckResult = Seq.last partialCheckResults
        let! diags =
            [ initialCheckResults; yield! partialCheckResults ]
            |> Seq.map (fun result -> result.Diags.GetOrComputeValue())
            |> NodeCode.Sequential
        let diagnostics = [ diagnosticsLogger.GetDiagnostics(); yield! diags |> Array.rev ]
        let! finalTcInfo = finalCheckResult.TcInfo.GetOrComputeValue()
        let finalBoundModelWithErrors = finalCheckResult.Finalize { finalTcInfo with tcDiagnosticsRev = diagnostics; topAttribs = Some topAttrs }
        return finalBoundModelWithErrors, ilAssemRef, tcAssemblyDataOpt, tcAssemblyExprOpt
    }

[<NoComparison;NoEquality>]
type IncrementalBuilderInitialState =
    {
        tcGlobals: TcGlobals
        referencedAssemblies: (Choice<string, IProjectReference> * (TimeStampCache -> DateTime)) list
        tcConfig: TcConfig
        outfile: string
        assemblyName: string
        lexResourceManager: Lexhelp.LexResourceManager
        fileNames: FSharpFile list
        enablePartialTypeChecking: bool
        beforeFileChecked: Event<string>
        fileChecked: Event<string>
        fileParsed: Event<string>
        projectChecked: Event<unit>
#if !NO_TYPEPROVIDERS
        importsInvalidatedByTypeProvider: Event<unit>
#endif
        allDependencies: string []
        defaultTimeStamp: DateTime
        mutable isImportsInvalidated: bool
        useChangeNotifications: bool
        useSyntaxTreeCache: bool
    }

    static member Create
        (
            tcGlobals,
            nonFrameworkAssemblyInputs,
            tcConfig: TcConfig,
            outfile,
            assemblyName,
            lexResourceManager,
            sourceFiles,
            enablePartialTypeChecking,
            beforeFileChecked: Event<string>,
            fileChecked: Event<string>,
#if !NO_TYPEPROVIDERS
            importsInvalidatedByTypeProvider: Event<unit>,
#endif
            allDependencies,
            defaultTimeStamp,
            useChangeNotifications: bool,
            useSyntaxTreeCache
        ) =

        let initialState =
            {
                tcGlobals = tcGlobals
                referencedAssemblies = nonFrameworkAssemblyInputs |> List.ofSeq
                tcConfig = tcConfig
                outfile = outfile
                assemblyName = assemblyName
                lexResourceManager = lexResourceManager
                fileNames = sourceFiles
                enablePartialTypeChecking = enablePartialTypeChecking
                beforeFileChecked = beforeFileChecked
                fileChecked = fileChecked
                fileParsed = Event<string>()
                projectChecked = Event<unit>()
#if !NO_TYPEPROVIDERS
                importsInvalidatedByTypeProvider = importsInvalidatedByTypeProvider
#endif
                allDependencies = allDependencies
                defaultTimeStamp = defaultTimeStamp
                isImportsInvalidated = false
                useChangeNotifications = useChangeNotifications
                useSyntaxTreeCache = useSyntaxTreeCache
            }
#if !NO_TYPEPROVIDERS
        importsInvalidatedByTypeProvider.Publish.Add(fun () -> initialState.isImportsInvalidated <- true)
#endif
        initialState

type BoundModel =
    {
        Stamp: DateTime
        SyntaxTree: SyntaxTree
        PartialCheckResults: PartialCheckResults
    }
    member this.FileName = this.SyntaxTree.FileName
    member this.HasSignature = this.SyntaxTree.HasSignature
    member this.Notify timeStamp =
        if this.Stamp <> timeStamp then { this with Stamp = timeStamp } else this
    member this.Notified = this.Stamp <> this.SyntaxTree.TimeStamp

[<NoComparison;NoEquality>]
type IncrementalBuilderState =
    {
        boundModels: BoundModel list
        stampedReferencedAssemblies: DateTime list
        initialCheckResults: PartialCheckResults
        partialCheckResultsBuilder: PartialCheckResultsBuilder
        finalizedCheckResults: GraphNode<(PartialCheckResults * ILAssemblyRef * ProjectAssemblyDataResult * CheckedImplFile list option)>
    }

[<AutoOpen>]
module IncrementalBuilderStateHelpers =

    // Used to thread the status of the build in computeStampedFileNames mapFold.
    type private BuildStatus = Invalidated | Good

    let createFinalizeCheckResultsGraphNode (initialState: IncrementalBuilderInitialState) initialErrors (partialCheckResults: PartialCheckResults seq) =
        GraphNode(node {
            use _ = Activity.start "GetCheckResultsAndImplementationsForProject" [|Activity.Tags.project, initialState.outfile|]
            return!
                FinalizeTypeCheckTask 
                    initialState.tcConfig 
                    initialState.tcGlobals
                    initialState.enablePartialTypeChecking
                    initialState.assemblyName 
                    initialState.outfile
                    initialErrors
                    partialCheckResults
        })

    let checkResultsForFile (builder: PartialCheckResultsBuilder) prevCheckResults syntaxTree =
        builder.Next(prevCheckResults, syntaxTree)

    let computeStampedFileNames (initialState: IncrementalBuilderInitialState) (state: IncrementalBuilderState) =
        let folder (priorCheckResults: PartialCheckResults) (bm: BoundModel) =
            let syntaxTree = if bm.Notified then bm.SyntaxTree.Invalidate(bm.Stamp) else bm.SyntaxTree
            let partialCheckResults =
                if bm.Notified || priorCheckResults.TimeStamp > bm.PartialCheckResults.TimeStamp then
                    checkResultsForFile state.partialCheckResultsBuilder priorCheckResults syntaxTree
                else
                    bm.PartialCheckResults
            let updated = 
                { bm with
                    SyntaxTree = syntaxTree
                    PartialCheckResults = partialCheckResults }

            updated, partialCheckResults

        let slots, _ = state.boundModels |> List.mapFold folder state.initialCheckResults
        let partialCheckResults = slots |> Seq.map (fun s -> s.PartialCheckResults)
        { state with
            boundModels = slots
            finalizedCheckResults = createFinalizeCheckResultsGraphNode initialState state.initialCheckResults partialCheckResults }

    let computeStampedReferencedAssemblies referencedAssemblies (cache: TimeStampCache) =
        [ for asmInfo in referencedAssemblies -> StampReferencedAssemblyTask cache asmInfo ]

    let checkFileStamps (state: IncrementalBuilderState) (cache: TimeStampCache) =
        { state with boundModels = [ for slot in state.boundModels -> cache.GetFileTimeStamp slot.FileName |> slot.Notify ] }

type IncrementalBuilderState with

    (*
        The data below represents a dependency graph.

        ReferencedAssembliesStamps => FileStamps => BoundModels => FinalizedBoundModel
    *)
    static member Create(initialState: IncrementalBuilderInitialState, partialCheckResultsBuilder, initialCheckResults, cache) =
        let syntaxTrees =
            [
                for sourceFile in initialState.fileNames ->
                    SyntaxTree(initialState.tcConfig, initialState.fileParsed, initialState.lexResourceManager, sourceFile, DateTime.MinValue)
            ]

        let partialCheckResults = 
            syntaxTrees
            |> Seq.scan (checkResultsForFile partialCheckResultsBuilder) initialCheckResults
            |> Seq.skip 1

        let slots =
            [
                for results, syntaxTree in Seq.zip partialCheckResults syntaxTrees do
                    {
                        Stamp = DateTime.MinValue
                        SyntaxTree = syntaxTree
                        PartialCheckResults = results
                    }
            ]

        let stampedReferencedAssemblies = computeStampedReferencedAssemblies initialState.referencedAssemblies cache

        let state =
            {
                boundModels = slots
                stampedReferencedAssemblies = stampedReferencedAssemblies
                partialCheckResultsBuilder = partialCheckResultsBuilder
                initialCheckResults = initialCheckResults
                finalizedCheckResults = createFinalizeCheckResultsGraphNode initialState initialCheckResults partialCheckResults
            }
        let state = if initialState.useChangeNotifications then state else checkFileStamps state cache
        let state = computeStampedFileNames initialState state
        state

/// Manages an incremental build graph for the build of a single F# project
type IncrementalBuilder(initialState: IncrementalBuilderInitialState, state: IncrementalBuilderState) =

    let tcConfig = initialState.tcConfig
    let fileNames = initialState.fileNames
    let beforeFileChecked = initialState.beforeFileChecked
    let fileChecked = initialState.fileChecked
#if !NO_TYPEPROVIDERS
    let importsInvalidatedByTypeProvider = initialState.importsInvalidatedByTypeProvider
#endif
    let defaultTimeStamp = initialState.defaultTimeStamp
    let fileParsed = initialState.fileParsed
    let projectChecked = initialState.projectChecked

    let getSlot (state: IncrementalBuilderState) slot =
        if slot = -1 then
            state.initialCheckResults
        else
            state.boundModels[slot].PartialCheckResults

    let getBeforeSlot (state: IncrementalBuilderState) slot =
        getSlot state (slot - 1)

    let mutable currentState = state 

    let guard = obj()

    let setCurrentState state =
        lock guard <| fun () ->
            currentState <- computeStampedFileNames initialState state

    let checkFileTimeStampsIfNotUsingNotifications cache =
        if not initialState.useChangeNotifications then
            checkFileStamps currentState cache
            |> setCurrentState

    let updateFileTimeStampsIfNotUsingNotifications () =
        if not initialState.useChangeNotifications then
            let cache = TimeStampCache defaultTimeStamp
            checkFileTimeStampsIfNotUsingNotifications cache

    do IncrementalBuilderEventTesting.MRU.Add(IncrementalBuilderEventTesting.IBECreated)

    member _.TcConfig = tcConfig

    member _.FileParsed = fileParsed.Publish

    member _.BeforeFileChecked = beforeFileChecked.Publish

    member _.FileChecked = fileChecked.Publish

    member _.ProjectChecked = projectChecked.Publish

#if !NO_TYPEPROVIDERS
    member _.ImportsInvalidatedByTypeProvider = importsInvalidatedByTypeProvider.Publish
#endif

    member _.IsReferencesInvalidated = 
        // fast path
        if initialState.isImportsInvalidated then true
        else
            let stampedReferencedAssemblies =
                computeStampedReferencedAssemblies initialState.referencedAssemblies (TimeStampCache defaultTimeStamp)
            state.stampedReferencedAssemblies <> stampedReferencedAssemblies

    member _.AllDependenciesDeprecated = initialState.allDependencies

    member builder.GetCheckResultsBeforeFileInProjectEvenIfStale fileName =
        updateFileTimeStampsIfNotUsingNotifications ()
        let slotOfFile = builder.GetSlotOfFileName fileName
        getBeforeSlot currentState slotOfFile

    member builder.GetCheckResultsForFileInProjectEvenIfStale fileName =
        updateFileTimeStampsIfNotUsingNotifications ()
        let slotOfFile = builder.GetSlotOfFileName fileName
        getSlot currentState slotOfFile

    member builder.TryGetCheckResultsBeforeFileInProject fileName =
        updateFileTimeStampsIfNotUsingNotifications ()
        let slotOfFile = builder.GetSlotOfFileName fileName
        getBeforeSlot currentState slotOfFile

    member _.GetCheckResultsBeforeSlotInProject slotOfFile =
        updateFileTimeStampsIfNotUsingNotifications ()
        getBeforeSlot currentState slotOfFile

    member builder.GetFullCheckResultsBeforeSlotInProject slotOfFile =
      node {
        updateFileTimeStampsIfNotUsingNotifications ()
        let results = getBeforeSlot currentState slotOfFile
        let! _ = results.GetOrComputeTcInfoExtras()
        return results
      }

    member builder.GetCheckResultsBeforeFileInProject fileName =
        let slotOfFile = builder.GetSlotOfFileName fileName
        builder.GetCheckResultsBeforeSlotInProject slotOfFile

    member builder.GetCheckResultsAfterFileInProject fileName =
        let slotOfFile = builder.GetSlotOfFileName fileName + 1
        builder.GetCheckResultsBeforeSlotInProject slotOfFile

    member builder.GetFullCheckResultsBeforeFileInProject fileName =
        let slotOfFile = builder.GetSlotOfFileName fileName
        builder.GetFullCheckResultsBeforeSlotInProject slotOfFile

    member builder.GetFullCheckResultsAfterFileInProject fileName =
        node {
            let slotOfFile = builder.GetSlotOfFileName fileName + 1
            let! result = builder.GetFullCheckResultsBeforeSlotInProject(slotOfFile)
            return result
        }

    member builder.GetCheckResultsAfterLastFileInProject () =
        builder.GetCheckResultsBeforeSlotInProject(builder.GetSlotsCount())

    member _.GetCheckResultsAndImplementationsForProject() =
        node {
            updateFileTimeStampsIfNotUsingNotifications ()
            return! currentState.finalizedCheckResults.GetOrComputeValue()
        }

    member builder.GetFullCheckResultsAndImplementationsForProject() =
        node {
            let! result = builder.GetCheckResultsAndImplementationsForProject()
            let results, _, _, _ = result
            let! _ = results.GetOrComputeTcInfoWithExtras() // Make sure we forcefully evaluate the info
            return result
        }

    member _.GetLogicalTimeStampForProject(cache) =
        checkFileTimeStampsIfNotUsingNotifications cache
        let slot = currentState.boundModels |> List.last
        slot.PartialCheckResults.TimeStamp

    member _.TryGetSlotOfFileName(fileName: string) =
        // Get the slot of the given file.
        let CompareFileNames f =
            let result =
                   String.Compare(fileName, f.Name, StringComparison.CurrentCultureIgnoreCase)=0
                || String.Compare(FileSystem.GetFullPathShim fileName, FileSystem.GetFullPathShim f.Name, StringComparison.CurrentCultureIgnoreCase)=0
            result
        match fileNames |> List.tryFindIndex CompareFileNames with
        | Some slot -> Some slot
        | None -> None

    member builder.GetSlotOfFileName(fileName: string) =
        match builder.TryGetSlotOfFileName(fileName) with
        | Some slot -> slot
        | None -> failwith (sprintf "The file '%s' was not part of the project. Did you call InvalidateConfiguration when the list of files in the project changed?" fileName)

    member _.GetSlotsCount () = fileNames.Length

    member builder.ContainsFile(fileName: string) =
        (builder.TryGetSlotOfFileName fileName).IsSome

    member builder.GetParseResultsForFile fileName =
        let slotOfFile = builder.GetSlotOfFileName fileName
        let syntaxTree = currentState.boundModels[slotOfFile].SyntaxTree
        syntaxTree.ParseNode.GetOrComputeValue()
        |> Async.AwaitNodeCode
        |> Async.RunSynchronously

    member builder.NotifyFileChanged(fileName, timeStamp) =
        let slotOfFile = builder.GetSlotOfFileName fileName        
        setCurrentState
            { currentState with 
                boundModels = currentState.boundModels |> List.updateAt slotOfFile (currentState.boundModels[slotOfFile].Notify timeStamp) }

    member _.SourceFiles = fileNames |> Seq.map (fun f -> f.Name) |> List.ofSeq

    /// CreateIncrementalBuilder (for background type checking). Note that fsc.fs also
    /// creates an incremental builder used by the command line compiler.
    static member TryCreateIncrementalBuilderForProjectOptions
        (
            legacyReferenceResolver,
            defaultFSharpBinariesDir,
            frameworkTcImportsCache: FrameworkImportsCache,
            loadClosureOpt: LoadClosure option,
            sourceFiles: string list,
            commandLineArgs: string list,
            projectReferences,
            projectDirectory,
            useScriptResolutionRules,
            keepAssemblyContents,
            keepAllBackgroundResolutions,
            tryGetMetadataSnapshot,
            suggestNamesForErrors,
            keepAllBackgroundSymbolUses,
            enableBackgroundItemKeyStoreAndSemanticClassification,
            enablePartialTypeChecking,
            dependencyProvider,
            parallelReferenceResolution,
            captureIdentifiersWhenParsing,
            getSource,
            useChangeNotifications,
            useSyntaxTreeCache
        ) =

      let useSimpleResolutionSwitch = "--simpleresolution"

      node {

        // Trap and report diagnostics from creation.
        let delayedLogger = CapturingDiagnosticsLogger("IncrementalBuilderCreation")
        use _ = new CompilationGlobalsScope(delayedLogger, BuildPhase.Parameter)

        let! builderOpt =
         node {
          try

            // Create the builder.
            // Share intern'd strings across all lexing/parsing
            let resourceManager = Lexhelp.LexResourceManager()

            /// Create a type-check configuration
            let tcConfigB, sourceFiles =

                let getSwitchValue switchString =
                    match commandLineArgs |> List.tryFindIndex(fun s -> s.StartsWithOrdinal switchString) with
                    | Some idx -> Some(commandLineArgs[idx].Substring(switchString.Length))
                    | _ -> None

                let sdkDirOverride =
                    match loadClosureOpt with
                    | None -> None
                    | Some loadClosure -> loadClosure.SdkDirOverride

                // see also fsc.fs: runFromCommandLineToImportingAssemblies(), as there are many similarities to where the PS creates a tcConfigB
                let tcConfigB =
                    TcConfigBuilder.CreateNew(legacyReferenceResolver,
                         defaultFSharpBinariesDir,
                         implicitIncludeDir=projectDirectory,
                         reduceMemoryUsage=ReduceMemoryFlag.Yes,
                         isInteractive=useScriptResolutionRules,
                         isInvalidationSupported=true,
                         defaultCopyFSharpCore=CopyFSharpCoreFlag.No,
                         tryGetMetadataSnapshot=tryGetMetadataSnapshot,
                         sdkDirOverride=sdkDirOverride,
                         rangeForErrors=range0)

                tcConfigB.primaryAssembly <-
                    match loadClosureOpt with
                    | None -> PrimaryAssembly.Mscorlib
                    | Some loadClosure ->
                        if loadClosure.UseDesktopFramework then
                            PrimaryAssembly.Mscorlib
                        else
                            PrimaryAssembly.System_Runtime

                tcConfigB.resolutionEnvironment <- (LegacyResolutionEnvironment.EditingOrCompilation true)

                tcConfigB.conditionalDefines <-
                    let define = if useScriptResolutionRules then "INTERACTIVE" else "COMPILED"
                    define :: tcConfigB.conditionalDefines

                tcConfigB.projectReferences <- projectReferences

                tcConfigB.useSimpleResolution <- (getSwitchValue useSimpleResolutionSwitch) |> Option.isSome

                // Apply command-line arguments and collect more source files if they are in the arguments
                let sourceFilesNew = ApplyCommandLineArgs(tcConfigB, sourceFiles, commandLineArgs)

                // Never open PDB files for the language service, even if --standalone is specified
                tcConfigB.openDebugInformationForLaterStaticLinking <- false

                tcConfigB.xmlDocInfoLoader <-
                    { new IXmlDocumentationInfoLoader with
                        /// Try to load xml documentation associated with an assembly by the same file path with the extension ".xml".
                        member _.TryLoad(assemblyFileName) =
                            let xmlFileName = Path.ChangeExtension(assemblyFileName, ".xml")

                            // REVIEW: File IO - Will eventually need to change this to use a file system interface of some sort.
                            XmlDocumentationInfo.TryCreateFromFile(xmlFileName)
                    }
                    |> Some

                tcConfigB.parallelReferenceResolution <- parallelReferenceResolution
                tcConfigB.captureIdentifiersWhenParsing <- captureIdentifiersWhenParsing

                tcConfigB, sourceFilesNew

            // If this is a builder for a script, re-apply the settings inferred from the
            // script and its load closure to the configuration.
            //
            // NOTE: it would probably be cleaner and more accurate to re-run the load closure at this point.
            let setupConfigFromLoadClosure () =
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
                    tcConfigB.primaryAssembly <- (if loadClosure.UseDesktopFramework then PrimaryAssembly.Mscorlib else PrimaryAssembly.System_Runtime)
                    // Add one by one to remove duplicates
                    dllReferences |> List.iter (fun dllReference ->
                        tcConfigB.AddReferencedAssemblyByPath(dllReference.Range, dllReference.Text))
                    tcConfigB.knownUnresolvedReferences <- loadClosure.UnresolvedReferences
                | None -> ()

            setupConfigFromLoadClosure()

            let tcConfig = TcConfig.Create(tcConfigB, validate=true)
            let outfile, _, assemblyName = tcConfigB.DecideNames sourceFiles

            // Resolve assemblies and create the framework TcImports. This is done when constructing the
            // builder itself, rather than as an incremental task. This caches a level of "system" references. No type providers are
            // included in these references.
            let! tcGlobals, frameworkTcImports, nonFrameworkResolutions, unresolvedReferences = frameworkTcImportsCache.Get(tcConfig)

            // Note we are not calling diagnosticsLogger.GetDiagnostics() anywhere for this task.
            // This is ok because not much can actually go wrong here.
            let diagnosticsLogger = CompilationDiagnosticLogger("nonFrameworkAssemblyInputs", tcConfig.diagnosticsOptions)
            use _ = new CompilationGlobalsScope(diagnosticsLogger, BuildPhase.Parameter)

            // Get the names and time stamps of all the non-framework referenced assemblies, which will act
            // as inputs to one of the nodes in the build.
            //
            // This operation is done when constructing the builder itself, rather than as an incremental task.
            let nonFrameworkAssemblyInputs =
                // Note we are not calling diagnosticsLogger.GetDiagnostics() anywhere for this task.
                // This is ok because not much can actually go wrong here.
                let diagnosticsLogger = CompilationDiagnosticLogger("nonFrameworkAssemblyInputs", tcConfig.diagnosticsOptions)
                // Return the disposable object that cleans up
                use _holder = new CompilationGlobalsScope(diagnosticsLogger, BuildPhase.Parameter)

                [ for r in nonFrameworkResolutions do
                    let fileName = r.resolvedPath
                    yield (Choice1Of2 fileName, (fun (cache: TimeStampCache) -> cache.GetFileTimeStamp fileName))

                  for pr in projectReferences  do
                    yield Choice2Of2 pr, (fun (cache: TimeStampCache) -> cache.GetProjectReferenceTimeStamp pr) ]

            // Start importing

            let beforeFileChecked = Event<string>()
            let fileChecked = Event<string>()

#if !NO_TYPEPROVIDERS
            let importsInvalidatedByTypeProvider = Event<unit>()
#endif

            // Check for the existence of loaded sources and prepend them to the sources list if present.
            let sourceFiles = tcConfig.GetAvailableLoadedSources() @ (sourceFiles |>List.map (fun s -> rangeStartup, s))

            // Mark up the source files with an indicator flag indicating if they are the last source file in the project
            let sourceFiles =
                let flags, isExe = tcConfig.ComputeCanContainEntryPoint(sourceFiles |> List.map snd)
                ((sourceFiles, flags) ||> List.map2 (fun (m, nm) flag -> (m, nm, (flag, isExe))))

            let basicDependencies =
                [ for UnresolvedAssemblyReference(referenceText, _)  in unresolvedReferences do
                    // Exclude things that are definitely not a file name
                    if not(FileSystem.IsInvalidPathShim referenceText) then
                        let file = if FileSystem.IsPathRootedShim referenceText then referenceText else Path.Combine(projectDirectory, referenceText)
                        yield file

                  for r in nonFrameworkResolutions do
                        yield  r.resolvedPath ]

            let allDependencies =
                [| yield! basicDependencies
                   for _, f, _ in sourceFiles do
                        yield f |]

            // For scripts, the dependency provider is already available.
            // For projects create a fresh one for the project.
            let dependencyProvider =
                match dependencyProvider with
                | None -> new DependencyProvider()
                | Some dependencyProvider -> dependencyProvider

            let defaultTimeStamp = DateTime.UtcNow
            let cache = TimeStampCache defaultTimeStamp

            let! tcImports, diagnostics = 
                CombineImportedAssembliesTask(
                    tcConfig,
                    frameworkTcImports,
                    nonFrameworkResolutions,
                    unresolvedReferences,
                    dependencyProvider
#if !NO_TYPEPROVIDERS
                    ,importsInvalidatedByTypeProvider
#endif
                )

            let partialCheckResultsBuilder =
                PartialCheckResultsBuilder(
                    tcImports,
                    tcConfig,
                    tcGlobals,
                    enableBackgroundItemKeyStoreAndSemanticClassification,
                    keepAssemblyContents,
                    keepAllBackgroundResolutions,
                    keepAllBackgroundSymbolUses,
                    beforeFileChecked,
                    fileChecked   
                )

            let tcInfo = GetInitialTcInfo tcConfig assemblyName loadClosureOpt basicDependencies tcGlobals tcImports diagnostics
            let defaultTcInfoExtras = partialCheckResultsBuilder.DefaultTcInfoExtras(tcInfo)

            let initialCheckResults =
                let diags = tcInfo.TcDiagnostics
                partialCheckResultsBuilder.Create(GraphNode.FromResult tcInfo, GraphNode.FromResult diags, defaultTcInfoExtras, defaultTimeStamp)

            let getFSharpSource fileName =
                getSource
                |> Option.map(fun getSource ->
                    let timeStamp = DateTime.UtcNow
                    let getTimeStamp = fun () -> timeStamp
                    let getSourceText() = getSource fileName
                    FSharpSource.Create(fileName, getTimeStamp, getSourceText))
                |> Option.defaultWith(fun () -> FSharpSource.CreateFromFile(fileName))

            let hasSignature =
                let isImplFile fileName = FSharpImplFileSuffixes |> List.exists (FileSystemUtils.checkSuffix fileName)
                let isSigFile fileName = FSharpSigFileSuffixes |> List.exists (FileSystemUtils.checkSuffix fileName)
                let isBackingSignature fileName sigName =
                    isImplFile fileName && isSigFile sigName &&
                        FileSystemUtils.fileNameWithoutExtension sigName = FileSystemUtils.fileNameWithoutExtension fileName
                let fileNames = sourceFiles |> List.map (fun (_, fileName, _) -> fileName )
                [
                    false
                    for prev, file in fileNames |> List.pairwise do
                        isBackingSignature file prev
                ]
            
            let sourceFiles =
                [ for (m, fileName, isLastCompiland), hasSignature in List.zip sourceFiles hasSignature ->
                    { Name = fileName; HasSignature = hasSignature; Range = m; Source = getFSharpSource fileName; Flags = isLastCompiland } ]

            let initialState =
                IncrementalBuilderInitialState.Create(
                    tcGlobals,
                    nonFrameworkAssemblyInputs,
                    tcConfig,
                    outfile,
                    assemblyName,
                    resourceManager,
                    sourceFiles,
                    enablePartialTypeChecking,
                    beforeFileChecked,
                    fileChecked,
#if !NO_TYPEPROVIDERS
                    importsInvalidatedByTypeProvider,
#endif
                    allDependencies,
                    defaultTimeStamp,
                    useChangeNotifications,
                    useSyntaxTreeCache
                )

            let builder = IncrementalBuilder(initialState, IncrementalBuilderState.Create(initialState, partialCheckResultsBuilder, initialCheckResults, cache))
            return Some builder
          with exn ->
            errorRecoveryNoRange exn
            return None
         }

        let diagnostics =
            let diagnostics, flatErrors =
                match builderOpt with
                | Some builder ->
                    let diagnosticsOptions = builder.TcConfig.diagnosticsOptions
                    let diagnosticsLogger = CompilationDiagnosticLogger("IncrementalBuilderCreation", diagnosticsOptions)
                    delayedLogger.CommitDelayedDiagnostics diagnosticsLogger
                    diagnosticsLogger.GetDiagnostics(), builder.TcConfig.flatErrors
                | _ ->
                    Array.ofList delayedLogger.Diagnostics, false
            diagnostics
            |> Array.map (fun (diagnostic, severity) ->
                FSharpDiagnostic.CreateFromException(diagnostic, severity, range.Zero, suggestNamesForErrors, flatErrors))

        return builderOpt, diagnostics
      }