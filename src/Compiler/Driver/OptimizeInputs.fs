// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.OptimizeInputs

open System.Collections.Generic
open System.Diagnostics
open System.IO
open System.Threading
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

[<RequireQualifiedAccess>]
module private ParallelOptimization =
    open Optimizer
    type OptimizeDuringCodeGen = bool -> Expr -> Expr

    type OptimizeRes =
        (IncrementalOptimizationEnv * CheckedImplFile * ImplFileOptimizationInfo * SignatureHidingInfo) * OptimizeDuringCodeGen

    type PhaseInputs = IncrementalOptimizationEnv * SignatureHidingInfo * CheckedImplFile
    type Phase1Inputs = PhaseInputs
    type Phase1Res = OptimizeRes
    type Phase1Fun = Phase1Inputs -> Phase1Res
    type Phase2Inputs = IncrementalOptimizationEnv * SignatureHidingInfo * ImplFileOptimizationInfo * CheckedImplFile
    type Phase2Res = IncrementalOptimizationEnv * CheckedImplFile
    type Phase2Fun = Phase2Inputs -> Phase2Res
    type Phase3Inputs = PhaseInputs
    type Phase3Res = IncrementalOptimizationEnv * CheckedImplFile
    type Phase3Fun = Phase3Inputs -> Phase3Res

    type FileResultsComplete =
        {
            Phase1: Phase1Res
            Phase2: Phase2Res
            Phase3: Phase3Res
        }

    type FilePhaseFuncs = Phase1Fun * Phase2Fun * Phase3Fun

    [<RequireQualifiedAccess>]
    type OptimizationPhase =
        | Phase1
        | Phase2
        | Phase3

    type FileResults =
        {
            mutable Phase1: Phase1Res option
            mutable Phase2: Phase2Res option
            mutable Phase3: Phase3Res option
        }

        member this.HasResult(phase: OptimizationPhase) =
            match phase with
            | OptimizationPhase.Phase1 -> this.Phase1 |> Option.isSome
            | OptimizationPhase.Phase2 -> this.Phase2 |> Option.isSome
            | OptimizationPhase.Phase3 -> this.Phase3 |> Option.isSome

        static member Empty =
            {
                Phase1 = None
                Phase2 = None
                Phase3 = None
            }

    type Node =
        {
            Idx: int
            Phase: OptimizationPhase
        }

        override this.ToString() = $"[{this.Idx}-{this.Phase}]"

    let collectResults
        (inputs: FileResultsComplete[])
        : (CheckedImplFileAfterOptimization * ImplFileOptimizationInfo)[] * IncrementalOptimizationEnv =
        let files =
            inputs
            |> Array.map
                (fun {
                         Phase1 = phase1
                         Phase2 = _phase2
                         Phase3 = phase3
                     } ->
                    let (_, _, implFileOptData, _), optimizeDuringCodeGen = phase1
                    let _, implFile = phase3

                    let implFile =
                        {
                            ImplFile = implFile
                            OptimizeDuringCodeGen = optimizeDuringCodeGen
                        }

                    implFile, implFileOptData)

        let lastFilePhase1Env =
            inputs
            |> Array.last
            |> fun { Phase1 = phase1 } ->
                let (optEnvPhase1, _, _, _), _ = phase1
                optEnvPhase1

        files, lastFilePhase1Env

    let getPhase2Res (p: FileResults) = p.Phase2 |> Option.get

    let getPhase3Res (p: FileResults) =
        p.Phase3 |> Option.get |> (fun (env, _) -> env)

    let optimizeFilesInParallel
        (env0: IncrementalOptimizationEnv)
        ((phase1, phase2, phase3): FilePhaseFuncs)
        (files: CheckedImplFile list)
        (ct: CancellationToken)
        : (CheckedImplFileAfterOptimization * ImplFileOptimizationInfo)[] * IncrementalOptimizationEnv =
        let files = files |> List.toArray

        let firstNode =
            {
                Idx = 0
                Phase = OptimizationPhase.Phase1
            }

        let results = files |> Array.map (fun _ -> FileResults.Empty)

        let _lock = obj ()

        let nodeCanBeProcessed { Idx = idx; Phase = phase } : bool =
            let previousPhase =
                match phase with
                | OptimizationPhase.Phase1 -> None
                | OptimizationPhase.Phase2 -> Some OptimizationPhase.Phase1
                | OptimizationPhase.Phase3 -> Some OptimizationPhase.Phase2

            lock _lock (fun () ->
                let previousFileReady = if idx = 0 then true else results[ idx - 1 ].HasResult phase

                let previousPhaseReady =
                    match previousPhase with
                    | Some previousPhase -> results[ idx ].HasResult previousPhase
                    | None -> true

                previousFileReady && previousPhaseReady)

        let visited = HashSet<Node>()

        let worker ({ Idx = idx; Phase = phase } as node: Node) : Node[] =
            let notPreviouslyVisited = lock _lock (fun () -> visited.Add node)

            if notPreviouslyVisited = false then
                [||]
            else
                let res = results[idx]
                let file = files[idx]
                let previous = if idx > 0 then Some results[idx - 1] else None
                let hidingInfo0 = SignatureHidingInfo.Empty

                let getPhase1Res (p: FileResults) =
                    p.Phase1
                    |> Option.get
                    |> fun ((env, file, info, hidden), _) -> env, file, info, hidden

                match phase with
                | OptimizationPhase.Phase1 ->
                    // Take env from the previous file
                    let env, hidingInfo =
                        previous
                        |> Option.map getPhase1Res
                        |> Option.map (fun (a, _b, _c, d) -> a, d)
                        |> Option.defaultValue (env0, hidingInfo0)

                    let inputs = env, hidingInfo, file
                    let phase1Res = phase1 inputs
                    res.Phase1 <- Some phase1Res

                    let phase2Node =
                        {
                            Idx = idx
                            Phase = OptimizationPhase.Phase2
                        }

                    seq {
                        // Schedule Phase2 for the current file
                        yield phase2Node
                        // Schedule Phase1 for the next file if it exists
                        if idx < files.Length - 1 then
                            yield
                                {
                                    Idx = idx + 1
                                    Phase = OptimizationPhase.Phase1
                                }
                    }
                    |> Seq.toArray

                | OptimizationPhase.Phase2 ->
                    // Take env from previous file if it exists
                    let env =
                        previous
                        |> Option.map getPhase2Res
                        |> Option.map fst
                        |> Option.defaultValue env0

                    // Take impl file from Phase1
                    let file, info, hidingInfo =
                        res
                        |> getPhase1Res
                        |> (fun (_, file, optimizationInfo, hidingInfo) -> file, optimizationInfo, hidingInfo)

                    let inputs = env, hidingInfo, info, file
                    let phase2Res = phase2 inputs
                    res.Phase2 <- Some phase2Res

                    let phase3Node =
                        {
                            Idx = idx
                            Phase = OptimizationPhase.Phase3
                        }

                    seq {
                        // Schedule Phase3 for the current file
                        yield phase3Node
                        // Schedule Phase2 for the next file if it exists
                        if idx < files.Length - 1 then
                            yield
                                {
                                    Idx = idx + 1
                                    Phase = OptimizationPhase.Phase2
                                }
                    }
                    |> Seq.toArray

                | OptimizationPhase.Phase3 ->
                    // Take env from previous file if it exists
                    let env =
                        match previous with
                        | None -> env0
                        | Some { Phase3 = Some (env, _) } -> env
                        | Some { Phase3 = None } -> failwith $"Unexpected lack of results for previous file [{idx - 1}], phase 3"

                    // Take impl file from Phase2
                    let _, file = res |> getPhase2Res
                    let hidingInfo = res |> getPhase1Res |> (fun (_, _, _, hidingInfo) -> hidingInfo)
                    let inputs = env, hidingInfo, file
                    let phase3Res = phase3 inputs
                    res.Phase3 <- Some phase3Res

                    seq {
                        // Schedule Phase3 for the next file if it exists
                        if idx < files.Length - 1 then
                            yield
                                {
                                    Idx = idx + 1
                                    Phase = OptimizationPhase.Phase3
                                }
                    }
                    |> Seq.toArray
                |> fun nodes -> nodes |> Array.filter nodeCanBeProcessed

        // TODO Do we need to pass in DiagnosticsLogger, or does optimization not use it?
        FSharp.Compiler.Service.Utilities.ParallelProcessing.processInParallel
            "OptimizeInputs"
            [| firstNode |]
            worker
            10
            (fun () -> visited.Count >= files.Length * 3)
            ct
            (fun node -> node.ToString())

        Debug.Assert(
            visited.Count = files.Length * 3,
            $"Expected to have visited all {files.Length} * 3 = {files.Length * 3} optimization nodes, but visited {visited.Count}"
        )

        let results =
            results
            |> Array.mapi
                (fun i {
                           Phase1 = phase1
                           Phase2 = phase2
                           Phase3 = phase3
                       } ->
                    match phase1, phase2, phase3 with
                    | Some phase1, Some phase2, Some phase3 ->
                        {
                            FileResultsComplete.Phase1 = phase1
                            Phase2 = phase2
                            Phase3 = phase3
                        }
                    | _ -> failwith $"Unexpected lack of optimization results for file [{i}]")

        let collected = results |> collectResults
        collected

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

    let phase1Settings =
        { tcConfig.optSettings with
            // Only do abstractBigTargets in the first phase, and only when TLR is on.
            abstractBigTargets = tcConfig.doTLR
            reportingPhase = true
        }

    // Only do these two steps in the first phase.
    let phase2And3Settings =
        { phase1Settings with
            abstractBigTargets = false
            reportingPhase = false
        }

    let phase1 (env: Optimizer.IncrementalOptimizationEnv, hidden: SignatureHidingInfo, implFile: CheckedImplFile) =
        use _ =
            FSharp.Compiler.Diagnostics.Activity.start "phase1" [| "QualifiedNameOfFile", implFile.QualifiedNameOfFile.Text |]

        Optimizer.OptimizeImplFile(
            phase1Settings,
            ccu,
            tcGlobals,
            tcVal,
            importMap,
            env,
            isIncrementalFragment,
            tcConfig.fsiMultiAssemblyEmit,
            tcConfig.emitTailcalls,
            hidden,
            implFile
        )

    let phase2
        (
            env: Optimizer.IncrementalOptimizationEnv,
            hidden: SignatureHidingInfo,
            _implFileOptData: Optimizer.ImplFileOptimizationInfo,
            implFile: CheckedImplFile
        ) =
        use _ =
            FSharp.Compiler.Diagnostics.Activity.start "phase2" [| "QualifiedNameOfFile", implFile.QualifiedNameOfFile.Text |]

        let implFile = LowerLocalMutables.TransformImplFile tcGlobals importMap implFile

        if tcConfig.extraOptimizationIterations > 0 then
            let (optEnvExtraLoop, implFile, _, _), _ =
                Optimizer.OptimizeImplFile(
                    phase2And3Settings,
                    ccu,
                    tcGlobals,
                    tcVal,
                    importMap,
                    env,
                    isIncrementalFragment,
                    tcConfig.fsiMultiAssemblyEmit,
                    tcConfig.emitTailcalls,
                    hidden,
                    implFile
                )

            optEnvExtraLoop, implFile
        else
            env, implFile

    let phase3 (env: Optimizer.IncrementalOptimizationEnv, hidden: SignatureHidingInfo, implFile: CheckedImplFile) =
        use _ =
            FSharp.Compiler.Diagnostics.Activity.start "phase3" [| "QualifiedNameOfFile", implFile.QualifiedNameOfFile.Text |]

        let implFile =
            if tcConfig.doDetuple then
                let implFile = implFile |> Detuple.DetupleImplFile ccu tcGlobals
                implFile
            else
                implFile

        let implFile =
            if tcConfig.doTLR then
                implFile
                |> InnerLambdasToTopLevelFuncs.MakeTopLevelRepresentationDecisions ccu tcGlobals
            else
                implFile

        let implFile = LowerCalls.LowerImplFile tcGlobals implFile

        if tcConfig.doFinalSimplify then
            let (optEnvFinalSimplify, implFile, _, _), _ =
                Optimizer.OptimizeImplFile(
                    phase2And3Settings,
                    ccu,
                    tcGlobals,
                    tcVal,
                    importMap,
                    env,
                    isIncrementalFragment,
                    tcConfig.fsiMultiAssemblyEmit,
                    tcConfig.emitTailcalls,
                    hidden,
                    implFile
                )

            optEnvFinalSimplify, implFile
        else
            env, implFile

    let results, optEnvFirstLoop =
        match tcConfig.optSettings.processingMode with
        | Optimizer.OptimizationProcessingMode.PartiallyParallel ->
            let ct = CancellationToken.None

            let results, optEnvFirstPhase =
                ParallelOptimization.optimizeFilesInParallel optEnv (phase1, phase2, phase3) implFiles ct

            results |> Array.toList, optEnvFirstPhase
        | Optimizer.OptimizationProcessingMode.Sequential ->
            let results, (optEnvFirstLoop, _, _, _) =
                ((optEnv, optEnv, optEnv, SignatureHidingInfo.Empty), implFiles)

                ||> List.mapFold (fun (optEnvFirstLoop, optEnvExtraLoop, optEnvFinalSimplify, hidden) implFile ->
                    let (optEnvFirstLoop, implFile, implFileOptData, hidden), optimizeDuringCodeGen =
                        phase1 (optEnvFirstLoop, hidden, implFile)

                    let optEnvExtraLoop, implFile =
                        phase2 (optEnvExtraLoop, hidden, implFileOptData, implFile)

                    let optEnvFinalSimplify, implFile = phase3 (optEnvFinalSimplify, hidden, implFile)

                    let implFile =
                        {
                            ImplFile = implFile
                            OptimizeDuringCodeGen = optimizeDuringCodeGen
                        }

                    (implFile, implFileOptData), (optEnvFirstLoop, optEnvExtraLoop, optEnvFinalSimplify, hidden))

            results, optEnvFirstLoop

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
