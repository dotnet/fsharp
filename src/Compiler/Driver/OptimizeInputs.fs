// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.OptimizeInputs

open System.Collections.Generic
open System.IO
open System.Threading.Tasks
open Internal.Utilities.Library
open FSharp.Compiler
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.Diagnostics
open FSharp.Compiler.CheckDeclarations
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.CompilerImports
open FSharp.Compiler.CompilerOptions
open FSharp.Compiler.IlxGen
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.IO
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps

let mutable showTermFileCount = 0

let PrintWholeAssemblyImplementation (tcConfig: TcConfig) outfile header expr =
    if tcConfig.showTerms then
        if tcConfig.writeTermsToFiles then
            let fileName = outfile + ".terms"

            use f =
                FileSystem
                    .OpenFileForWriteShim(fileName + "-" + string showTermFileCount + "-" + header, FileMode.Create)
                    .GetWriter()

            showTermFileCount <- showTermFileCount + 1
            LayoutRender.outL f (Display.squashTo 192 (DebugPrint.implFilesL expr))
        else
            dprintf "\n------------------\nshowTerm: %s:\n" header
            LayoutRender.outL stderr (Display.squashTo 192 (DebugPrint.implFilesL expr))
            dprintf "\n------------------\n"

let AddExternalCcuToOptimizationEnv tcGlobals optEnv (ccuinfo: ImportedAssembly) =
    match ccuinfo.FSharpOptimizationData.Force() with
    | None -> optEnv
    | Some data -> Optimizer.BindCcu ccuinfo.FSharpViewOfMetadata data optEnv tcGlobals

let GetInitialOptimizationEnv (tcImports: TcImports, tcGlobals: TcGlobals) =
    let ccuinfos = tcImports.GetImportedAssemblies()
    let optEnv = Optimizer.IncrementalOptimizationEnv.Empty
    let optEnv = List.fold (AddExternalCcuToOptimizationEnv tcGlobals) optEnv ccuinfos
    optEnv

/// Result of the 'FirstLoop' phase
type FirstLoopRes =
    {
        OptEnv: Optimizer.IncrementalOptimizationEnv
        OptInfo: Optimizer.ImplFileOptimizationInfo
        HidingInfo: SignatureHidingInfo
        OptDuringCodeGen: bool -> Expr -> Expr
    }

/// All the state optimization phases can produce (except 'CheckedImplFile') and most of what they require as input.
type PhaseContext =
    {
        FirstLoopRes: FirstLoopRes
        OptEnvExtraLoop: Optimizer.IncrementalOptimizationEnv
        OptEnvFinalSimplify: Optimizer.IncrementalOptimizationEnv
    }

/// The result of running a single Optimization Phase.
type PhaseRes = CheckedImplFile * PhaseContext

/// 0-based index of an Optimization Phase.
type PhaseIdx = int

/// All inputs required to evaluate each Optimization Phase.
type PhaseInputs =
    {
        File: CheckedImplFile
        FileIdx: int
        /// State returned by processing the previous phase for the current file, or initial state.
        PrevPhase: PhaseContext
        /// State returned by processing the current phase for the previous file, or initial state.
        PrevFile: PhaseContext
    }

/// Signature of each optimization phase.
type PhaseFunc = PhaseInputs -> CheckedImplFile * PhaseContext

/// Each file's optimization can be split into up to seven different phases, executed one after another.
type Phase =
    {
        Idx: PhaseIdx
        Name: string
    }

    override this.ToString() = $"{this.Idx}-{this.Name}"

type PhaseInfo = { Phase: Phase; Func: PhaseFunc }

[<RequireQualifiedAccess>]
module private ParallelOptimization =
    open Optimizer

    /// Identifies a work item scheduled independently of others - consists of a (file) index and OptimizationPhase.
    /// There are (NumberOfFiles * NumberOfPhases) nodes in the whole optimization process.
    type private Node =
        {
            FileIdx: int
            Phase: PhaseIdx
        }

        override this.ToString() = $"[{this.FileIdx}-{this.Phase}]"

    /// Final processing of file results to produce output needed for further compilation steps.
    let private collectFinalResults
        (fileResults: PhaseRes[])
        : (CheckedImplFileAfterOptimization * ImplFileOptimizationInfo)[] * IncrementalOptimizationEnv =
        let finalFileResults =
            fileResults
            |> Array.map (fun (file, res) ->
                let implFile =
                    {
                        ImplFile = file
                        OptimizeDuringCodeGen = res.FirstLoopRes.OptDuringCodeGen
                    }

                implFile, res.FirstLoopRes.OptInfo)

        let lastFileFirstLoopEnv =
            fileResults |> Array.last |> (fun (_file, res) -> res.FirstLoopRes.OptEnv)

        finalFileResults, lastFileFirstLoopEnv

    let optimizeFilesInParallel
        (env0: IncrementalOptimizationEnv)
        (phases: PhaseInfo[])
        (files: CheckedImplFile list)
        : (CheckedImplFileAfterOptimization * ImplFileOptimizationInfo)[] * IncrementalOptimizationEnv =

        let files = files |> List.toArray

        /// Initial state for processing the current file.
        let initialState =
            {
                FirstLoopRes =
                    {
                        OptEnv = env0
                        OptInfo =
                            InterruptibleLazy(fun _ ->
                                failwith "This dummy value wrapped in a Lazy was not expected to be evaluated before being replaced.")
                        HidingInfo = SignatureHidingInfo.Empty
                        // A no-op optimizer
                        OptDuringCodeGen = fun _ expr -> expr
                    }
                OptEnvExtraLoop = env0
                OptEnvFinalSimplify = env0
            }

        // Functions for accessing a set of node jobs and their results
        let getTask, setTask =
            let tasks: Task<PhaseRes>[,] = Array2D.zeroCreate files.Length phases.Length
            let getTask (node: Node) = tasks[node.FileIdx, node.Phase]
            let setTask (node: Node) (task: Task<PhaseRes>) = tasks[node.FileIdx, node.Phase] <- task
            getTask, setTask

        /// Asynchronously wait for dependencies of a node
        let getNodeInputs (node: Node) =
            backgroundTask {
                let prevPhaseNode = { node with Phase = node.Phase - 1 }
                let prevFileNode = { node with FileIdx = node.FileIdx - 1 }

                let prevPhaseTask =
                    if node.Phase > 0 then
                        backgroundTask {
                            // Make sure that creating the task does not block before other node tasks finish
                            do! Task.Yield()
                            return! getTask prevPhaseNode
                        }
                    else
                        // First phase uses input file without modifications
                        (files[node.FileIdx], initialState) |> Task.FromResult

                let prevFileTask =
                    if node.FileIdx > 0 then
                        backgroundTask {
                            // Make sure that creating the task does not block before other node tasks finish
                            do! Task.Yield()
                            return! getTask prevFileNode
                        }
                    else
                        // We don't use the file result in this case, but we need something, so just take the first input file as a placeholder.
                        (files[0], initialState) |> Task.FromResult

                let! results = [| prevPhaseTask; prevFileTask |] |> Task.WhenAll
                let prevPhaseFile, prevPhaseRes = results[0]
                let _prevFileFile, prevFileRes = results[1]

                let inputs =
                    {
                        File = prevPhaseFile
                        FileIdx = node.FileIdx
                        PrevPhase = prevPhaseRes
                        PrevFile = prevFileRes
                    }

                return inputs
            }

        let startNodeTask (phase: PhaseInfo) (node: Node) =
            backgroundTask {
                // Make sure that creating the task does not block before other node tasks finish
                do! Task.Yield()
                let! inputs = getNodeInputs node
                let res = phase.Func inputs
                return res
            }

        let fileIndices = [| 0 .. files.Length - 1 |]

        for fileIdx in fileIndices do
            for phase in phases do
                let node =
                    {
                        Node.Phase = phase.Phase.Idx
                        FileIdx = fileIdx
                    }

                let task = startNodeTask phase node
                setTask node task

        let lastPhaseResultsTask =
            let lastPhaseIndex = phases[phases.Length - 1].Phase.Idx

            fileIndices
            |> Array.map (fun fileIdx ->
                getTask
                    {
                        Node.FileIdx = fileIdx
                        Phase = lastPhaseIndex
                    })
            |> Task.WhenAll

        let lastPhaseResults =
            try
                lastPhaseResultsTask.GetAwaiter().GetResult()
            // If multiple exceptions returned by multiple tasks, ignore all but the first one.
            with :? System.AggregateException as ex when ex.InnerExceptions.Count > 0 ->
                raise ex.InnerExceptions[0]

        collectFinalResults lastPhaseResults

let optimizeFilesSequentially optEnv (phases: PhaseInfo[]) implFiles =
    let results, (optEnvFirstLoop, _, _, _) =
        let implFiles = implFiles |> List.mapi (fun i file -> i, file)

        ((optEnv, optEnv, optEnv, SignatureHidingInfo.Empty), implFiles)

        ||> List.mapFold
            (fun (optEnvFirstLoop: Optimizer.IncrementalOptimizationEnv, optEnvExtraLoop, optEnvFinalSimplify, hidden) (fileIdx, implFile) ->

                /// Initial state for processing the current file.
                let state =
                    implFile,
                    {
                        FirstLoopRes =
                            {
                                OptEnv = optEnvFirstLoop
                                OptInfo =
                                    InterruptibleLazy(fun _ ->
                                        failwith
                                            "This dummy value wrapped in a Lazy was not expected to be evaluated before being replaced.")
                                HidingInfo = hidden
                                // A no-op optimizer
                                OptDuringCodeGen = fun _ expr -> expr
                            }
                        OptEnvExtraLoop = optEnvExtraLoop
                        OptEnvFinalSimplify = optEnvFinalSimplify
                    }

                let runPhase (file: CheckedImplFile, state: PhaseContext) (phase: PhaseInfo) =
                    // In the sequential mode we always process all phases of the previous file before processing the current file.
                    // This is why the state returned by the previous phase of the current file contains all changes made in the previous file,
                    // and we can use it in both places.
                    let input =
                        {
                            File = file
                            FileIdx = fileIdx
                            PrevPhase = state
                            PrevFile = state
                        }

                    phase.Func input

                let implFile, state = Array.fold runPhase state phases

                let file =
                    {
                        ImplFile = implFile
                        OptimizeDuringCodeGen = state.FirstLoopRes.OptDuringCodeGen
                    }

                (file, state.FirstLoopRes.OptInfo),
                (state.FirstLoopRes.OptEnv, state.OptEnvExtraLoop, state.OptEnvFinalSimplify, state.FirstLoopRes.HidingInfo))

    results, optEnvFirstLoop

let ApplyAllOptimizations
    (
        tcConfig: TcConfig,
        tcGlobals,
        tcVal,
        outfile,
        importMap,
        isIncrementalFragment,
        optEnv,
        ccu: CcuThunk,
        implFiles
    ) =
    // NOTE: optEnv - threads through
    //
    // Always optimize once - the results of this step give the x-module optimization
    // info.  Subsequent optimization steps choose representations etc. which we don't
    // want to save in the x-module info (i.e. x-module info is currently "high level").
    PrintWholeAssemblyImplementation tcConfig outfile "pass-start" implFiles
#if DEBUG
    if tcConfig.showOptimizationData then
        dprintf "Expression prior to optimization:\n%s\n" (LayoutRender.showL (Display.squashTo 192 (DebugPrint.implFilesL implFiles)))

    if tcConfig.showOptimizationData then
        dprintf "CCU prior to optimization:\n%s\n" (LayoutRender.showL (Display.squashTo 192 (DebugPrint.entityL ccu.Contents)))
#endif

    ReportTime tcConfig "Optimizations"

    let firstLoopSettings =
        { tcConfig.optSettings with
            // Only do abstractBigTargets in the first phase, and only when TLR is on.
            abstractBigTargets = tcConfig.doTLR
            reportingPhase = true
        }

    // Only do these two steps in the first phase.
    let extraAndFinalLoopSettings =
        { firstLoopSettings with
            abstractBigTargets = false
            reportingPhase = false
        }

    let addPhaseDiagnostics (f: PhaseFunc) (info: Phase) =
        fun (inputs: PhaseInputs) ->
            use _ =
                let tags =
                    [|
                        "QualifiedNameOfFile", inputs.File.QualifiedNameOfFile.Text
                        "OptimisationPhase", info.Name
                    |]

                FSharp.Compiler.Diagnostics.Activity.start $"file-{inputs.FileIdx}_phase-{info.Name}" tags

            f inputs

    let phases = List<PhaseInfo>()

    let addPhase (name: string) (phaseFunc: PhaseFunc) =
        let phase = { Idx = phases.Count; Name = name }

        let phaseInfo =
            {
                Phase = phase
                Func = addPhaseDiagnostics phaseFunc phase
            }

        phases.Add(phaseInfo)

    let firstLoop
        ({
             File = file
             PrevPhase = prevPhase
             PrevFile = prevFile
         }: PhaseInputs)
        : PhaseRes =
        let (env, file, optInfo, hidingInfo), optDuringCodeGen =
            Optimizer.OptimizeImplFile(
                firstLoopSettings,
                ccu,
                tcGlobals,
                tcVal,
                importMap,
                prevFile.FirstLoopRes.OptEnv,
                isIncrementalFragment,
                tcConfig.emitTailcalls,
                prevFile.FirstLoopRes.HidingInfo,
                file
            )

        file,
        { prevPhase with
            FirstLoopRes =
                {
                    OptEnv = env
                    OptInfo = optInfo
                    HidingInfo = hidingInfo
                    OptDuringCodeGen = optDuringCodeGen
                }
        }

    addPhase "firstLoop" firstLoop

    let lowerLocalMutables
        ({
             File = file
             PrevPhase = prevPhase
             PrevFile = _prevFile
         }: PhaseInputs)
        : PhaseRes =
        let file = LowerLocalMutables.TransformImplFile tcGlobals importMap file
        file, prevPhase

    addPhase "lowerLocalMutables" lowerLocalMutables

    let extraLoop
        ({
             File = file
             PrevPhase = prevPhase
             PrevFile = prevFile
         }: PhaseInputs)
        : PhaseRes =
        let (optEnvExtraLoop, file, _, _), _ =
            Optimizer.OptimizeImplFile(
                extraAndFinalLoopSettings,
                ccu,
                tcGlobals,
                tcVal,
                importMap,
                prevFile.OptEnvExtraLoop,
                isIncrementalFragment,
                tcConfig.emitTailcalls,
                prevPhase.FirstLoopRes.HidingInfo,
                file
            )

        file,
        { prevPhase with
            OptEnvExtraLoop = optEnvExtraLoop
        }

    if tcConfig.extraOptimizationIterations > 0 then
        addPhase "ExtraLoop" extraLoop

    let detuple
        ({
             File = file
             PrevPhase = prevPhase
             PrevFile = _prevFile
         }: PhaseInputs)
        : PhaseRes =
        let file = file |> Detuple.DetupleImplFile ccu tcGlobals
        file, prevPhase

    if tcConfig.doDetuple then
        addPhase "Detuple" detuple

    let innerLambdasToToplevelFuncs
        ({
             File = file
             PrevPhase = prevPhase
             PrevFile = _prevFile
         }: PhaseInputs)
        : PhaseRes =
        let file =
            file
            |> InnerLambdasToTopLevelFuncs.MakeTopLevelRepresentationDecisions ccu tcGlobals

        file, prevPhase

    if tcConfig.doTLR then
        addPhase "InnerLambdasToToplevelFuncs" innerLambdasToToplevelFuncs

    let lowerCalls
        ({
             File = file
             PrevPhase = prevPhase
             PrevFile = _prevFile
         }: PhaseInputs)
        : PhaseRes =
        let file = LowerCalls.LowerImplFile tcGlobals file
        file, prevPhase

    addPhase "LowerCalls" lowerCalls

    let finalSimplify
        ({
             File = file
             PrevPhase = prevPhase
             PrevFile = prevFile
         }: PhaseInputs)
        : PhaseRes =
        let (optEnvFinalSimplify, file, _, _), _ =
            Optimizer.OptimizeImplFile(
                extraAndFinalLoopSettings,
                ccu,
                tcGlobals,
                tcVal,
                importMap,
                prevFile.OptEnvFinalSimplify,
                isIncrementalFragment,
                tcConfig.emitTailcalls,
                prevPhase.FirstLoopRes.HidingInfo,
                file
            )

        file,
        { prevPhase with
            OptEnvFinalSimplify = optEnvFinalSimplify
        }

    if tcConfig.doFinalSimplify then
        addPhase "FinalSimplify" finalSimplify

    let phases = phases.ToArray()

    let results, optEnvFirstLoop =
        match tcConfig.optSettings.processingMode with
        // Parallel optimization breaks determinism - turn it off in deterministic builds.
        | Optimizer.OptimizationProcessingMode.Parallel when (not tcConfig.deterministic) ->
            let results, optEnvFirstPhase =
                ParallelOptimization.optimizeFilesInParallel optEnv phases implFiles

            results |> Array.toList, optEnvFirstPhase
        | Optimizer.OptimizationProcessingMode.Parallel
        | Optimizer.OptimizationProcessingMode.Sequential -> optimizeFilesSequentially optEnv phases implFiles

#if DEBUG
    if tcConfig.showOptimizationData then
        results
        |> List.map snd
        |> List.iter (fun implFileOptData ->
            let str =
                (LayoutRender.showL (Display.squashTo 192 (Optimizer.moduleInfoL tcGlobals implFileOptData)))

            dprintf $"Optimization implFileOptData:\n{str}\n")
#endif

    let implFiles, implFileOptDatas = List.unzip results
    let assemblyOptData = Optimizer.UnionOptimizationInfos implFileOptDatas
    let tassembly = CheckedAssemblyAfterOptimization implFiles
    PrintWholeAssemblyImplementation tcConfig outfile "pass-end" (implFiles |> List.map (fun implFile -> implFile.ImplFile))
    ReportTime tcConfig "Ending Optimizations"
    tassembly, assemblyOptData, optEnvFirstLoop

//----------------------------------------------------------------------------
// ILX generation
//----------------------------------------------------------------------------

let CreateIlxAssemblyGenerator (_tcConfig: TcConfig, tcImports: TcImports, tcGlobals, tcVal, generatedCcu) =
    let ilxGenerator =
        IlxAssemblyGenerator(tcImports.GetImportMap(), tcGlobals, tcVal, generatedCcu)

    let ccus = tcImports.GetCcusInDeclOrder()
    ilxGenerator.AddExternalCcus ccus
    ilxGenerator

let GenerateIlxCode
    (
        ilxBackend,
        isInteractiveItExpr,
        tcConfig: TcConfig,
        topAttrs: TopAttribs,
        optimizedImpls,
        fragName,
        ilxGenerator: IlxAssemblyGenerator
    ) =

    let mainMethodInfo =
        if
            (tcConfig.target = CompilerTarget.Dll)
            || (tcConfig.target = CompilerTarget.Module)
        then
            None
        else
            Some topAttrs.mainMethodAttrs

    let ilxGenOpts: IlxGenOptions =
        {
            generateFilterBlocks = tcConfig.generateFilterBlocks
            emitConstantArraysUsingStaticDataBlobs = true
            workAroundReflectionEmitBugs = tcConfig.isInteractive
            generateDebugSymbols = tcConfig.debuginfo // REVIEW: is this still required?
            fragName = fragName
            localOptimizationsEnabled = tcConfig.optSettings.LocalOptimizationsEnabled
            testFlagEmitFeeFeeAs100001 = tcConfig.testFlagEmitFeeFeeAs100001
            mainMethodInfo = mainMethodInfo
            ilxBackend = ilxBackend
            fsiMultiAssemblyEmit = tcConfig.fsiMultiAssemblyEmit
            useReflectionFreeCodeGen = tcConfig.useReflectionFreeCodeGen
            isInteractive = tcConfig.isInteractive
            isInteractiveItExpr = isInteractiveItExpr
            alwaysCallVirt = tcConfig.alwaysCallVirt
            parallelIlxGenEnabled = tcConfig.parallelIlxGen && not tcConfig.deterministic
        }

    ilxGenerator.GenerateCode(ilxGenOpts, optimizedImpls, topAttrs.assemblyAttrs, topAttrs.netModuleAttrs)

//----------------------------------------------------------------------------
// Assembly ref normalization: make sure all assemblies are referred to
// by the same references. Only used for static linking.
//----------------------------------------------------------------------------

let NormalizeAssemblyRefs (ctok, ilGlobals: ILGlobals, tcImports: TcImports) scoref =
    let normalizeAssemblyRefByName nm =
        match tcImports.TryFindDllInfo(ctok, Range.rangeStartup, nm, lookupOnly = false) with
        | Some dllInfo -> dllInfo.ILScopeRef
        | None -> scoref

    match scoref with
    | ILScopeRef.Local
    | ILScopeRef.Module _ -> scoref
    | ILScopeRef.PrimaryAssembly -> normalizeAssemblyRefByName ilGlobals.primaryAssemblyName
    | ILScopeRef.Assembly aref -> normalizeAssemblyRefByName aref.Name

let GetGeneratedILModuleName (t: CompilerTarget) (s: string) =
    // return the name of the file as a module name
    let ext =
        match t with
        | CompilerTarget.Dll -> "dll"
        | CompilerTarget.Module -> "netmodule"
        | _ -> "exe"

    s + "." + ext
