// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.OptimizeInputs

open System.Collections.Generic
open System.Diagnostics
open System.IO
open System.Threading
open FSharp.Compiler.Service.Driver
open FSharp.Compiler.Service.Driver.OptimizeTypes
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

let collectResults (inputs: CollectorInputs) : CollectorOutputs =
    let files =
        inputs
        |> Array.map (fun {Phase1 = phase1; Phase2 = _phase2; Phase3 = phase3} ->
            let (_, _, implFileOptData, _), optimizeDuringCodeGen = phase1
            let _, implFile = phase3
            let implFile =
                {
                    ImplFile = implFile
                    OptimizeDuringCodeGen = optimizeDuringCodeGen
                }
            implFile, implFileOptData
        )
        
    let lastFilePhase1Env =
        inputs
        |> Array.last
        |> fun {Phase1 = phase1} ->
            let (optEnvPhase1, _, _, _), _ = phase1
            optEnvPhase1
            
    files, lastFilePhase1Env

type Phase =
    | Phase1
    | Phase2
    | Phase3

type FilePhaseFuncs = Phase1Fun * Phase2Fun * Phase3Fun   
type FileResults =
    {
        mutable Phase1: Phase1Res option
        mutable Phase2: Phase2Res option
        mutable Phase3: Phase3Res option
    }
    with
        member this.HasResult (phase: Phase) =
            match phase with
            | Phase.Phase1 -> this.Phase1 |> Option.isSome
            | Phase.Phase2 -> this.Phase2 |> Option.isSome
            | Phase.Phase3 -> this.Phase3 |> Option.isSome
        static member Empty =
            {
                Phase1 = None
                Phase2 = None
                Phase3 = None
            }

type WorkItem =
    | Phase1 of Phase1Inputs
    | Phase2 of Phase2Inputs
    | Phase3 of Phase3Inputs

type Idx = int
type Node =
    {
        Idx: Idx
        Phase: Phase
    }
    with override this.ToString() = $"[{this.Idx}-{this.Phase}]"


let getPhase1Res (p: FileResults) =
    p.Phase1
    |> Option.get
    |> fun ((env, _, _, hidden), _) -> env, hidden

let getPhase2Res (p: FileResults) =
    p.Phase2
    |> Option.get

let getPhase3Res (p: FileResults) =
    p.Phase3
    |> Option.get
    |> fun (env, _) -> env

let go (env0: Optimizer.IncrementalOptimizationEnv) ((phase1, phase2, phase3): FilePhaseFuncs) (files: CheckedImplFile list) : CollectorOutputs =
    let files = files |> List.toArray
    // Schedule File1-Phase1
    let firstNode = { Idx = 0; Phase = Phase.Phase1 }
    
    let results =
        files
        |> Array.map (fun _ -> FileResults.Empty)
    
    let _lock = obj()
    let nodeCanBeProcessed {Idx = idx; Phase = phase} : bool =
        lock (_lock) (fun () ->
            let previousFileReady =
                if idx = 0 then true else results[idx-1].HasResult phase
            let previousPhase =
                match phase with
                | Phase.Phase1 -> None
                | Phase.Phase2 -> Some Phase.Phase1
                | Phase.Phase3 -> Some Phase.Phase2
            let previousPhaseReady =
                match previousPhase with
                | Some previousPhase -> results[idx].HasResult previousPhase
                | None -> true
            previousFileReady && previousPhaseReady
        )        
    
    let visited = HashSet<Node>()
    
    let worker ({Idx = idx; Phase = phase} as node : Node) : Node[] =
        let notPreviouslyVisited =
            lock (_lock) (fun () ->
                visited.Add node
            )
        if notPreviouslyVisited = false then [||]
        else
        let res = results[idx]
        let file = files[idx]
        let previous = if idx > 0 then Some results[idx-1] else None
        let hidingInfo0 = SignatureHidingInfo.Empty        
        
        let getPhase1Res (p: FileResults) =
            p.Phase1
            |> Option.get
            |> fun ((env, file, info, hidden), _) -> env, file, info, hidden
                
        match phase with
        | Phase.Phase1 ->
            // take env from previous file
            let env, hidingInfo =
                previous
                |> Option.map getPhase1Res
                |> Option.map (fun (a, _b, _c, d) -> a, d)
                |> Option.defaultValue (env0, hidingInfo0)
            let inputs = env, hidingInfo, file
            let phase1Res = phase1 inputs
            res.Phase1 <- Some phase1Res
            
            // Schedule Phase2
            let phase2Node = { Idx = idx; Phase = Phase.Phase2 }
            seq {
                yield phase2Node
                if idx < files.Length-1 then yield { Idx = idx + 1; Phase = Phase.Phase1 }
            }
            |> Seq.toArray
            
        | Phase.Phase2 ->
            // take env from previous file
            let env =
                previous
                |> Option.map getPhase2Res
                |> Option.map fst
                |> Option.defaultValue env0
            let file, info, hidingInfo =
                res
                |> getPhase1Res
                |> fun (_a, b, c, d) -> b, c, d
            let inputs = env, hidingInfo, info, file
            let phase2Res = phase2 inputs
            res.Phase2 <- Some phase2Res
            
            seq {
                // Schedule Phase3
                let phase3Node = { Idx = idx; Phase = Phase.Phase3 }
                yield phase3Node
                // Schedule Phase2 for the next file if it exists
                if idx < files.Length-1 then yield { Idx = idx + 1; Phase = Phase.Phase2 }
            }
            |> Seq.toArray
            
        | Phase.Phase3 ->
            // take env from previous file
            let env =
                previous
                |> Option.map getPhase3Res
                |> Option.defaultValue env0
            // impl file
            let _, file =
                res
                |> getPhase2Res
            let hidingInfo =
                res
                |> getPhase1Res
                |> fun (_a,_b,_c,d) -> d
            let inputs = env, hidingInfo, file
            let phase3Res = phase3 inputs
            res.Phase3 <- Some phase3Res
            
            seq {
                // Schedule Phase3 for the next file if it exists
                if idx < files.Length-1 then yield { Idx = idx + 1; Phase = Phase.Phase3 }
            }
            |> Seq.toArray
        |> fun nodes ->
            nodes
            |> Array.filter nodeCanBeProcessed
    
    Parallel.processInParallel
        [|firstNode|]
        worker
        10
        (fun _ -> visited.Count >= files.Length * 3)
        (CancellationToken.None)
        (fun node -> node.ToString())
        
    Debug.Assert(visited.Count = files.Length * 3)
    
    let results =
        results
        |> Array.mapi (fun i {Phase1 = phase1; Phase2 = phase2; Phase3 = phase3} ->
            match phase1, phase2, phase3 with
            | Some phase1, Some phase2, Some phase3 -> {FileResultsComplete.Phase1 = phase1; Phase2 = phase2; Phase3 = phase3}
            | _ -> failwith $"Unexpected lack of results for file [{i}]"
        )
    let collected = results |> collectResults
    collected

[<RequireQualifiedAccess>]
type OptimizerMode =
    | Sequential
    | PartiallyParallel

let mutable optimizerMode: OptimizerMode = OptimizerMode.Sequential

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

    let optEnv0 = optEnv
    ReportTime tcConfig "Optimizations"

    // Only do abstract_big_targets on the first pass!  Only do it when TLR is on!
    let optSettings = tcConfig.optSettings

    let optSettings =
        { optSettings with
            abstractBigTargets = tcConfig.doTLR
        }

    let optSettings =
        { optSettings with
            reportingPhase = true
        }

    
    let env0 = optEnv0
    
    let phase1 (env: Optimizer.IncrementalOptimizationEnv, hidden: SignatureHidingInfo, implFile: CheckedImplFile) : Phase1Res =
        //ReportTime tcConfig ("Initial simplify")
        Optimizer.OptimizeImplFile(
            optSettings,
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
    
    let phase2 (env: Optimizer.IncrementalOptimizationEnv, hidden: SignatureHidingInfo, implFileOptData: Optimizer.ImplFileOptimizationInfo, implFile: CheckedImplFile) : Phase2Res =
        let implFile = LowerLocalMutables.TransformImplFile tcGlobals importMap implFile

        // Only do this on the first pass!
        let optSettings =
            { optSettings with
                abstractBigTargets = false
                reportingPhase = false
            }
#if DEBUG
        if tcConfig.showOptimizationData then
            dprintf
                "Optimization implFileOptData:\n%s\n"
                (LayoutRender.showL (Display.squashTo 192 (Optimizer.moduleInfoL tcGlobals implFileOptData)))
#endif

        if tcConfig.extraOptimizationIterations > 0 then

            //ReportTime tcConfig ("Extra simplification loop")
            let (optEnvExtraLoop, implFile, _, _), _ =
                Optimizer.OptimizeImplFile(
                    optSettings,
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

            //PrintWholeAssemblyImplementation tcConfig outfile (sprintf "extra-loop-%d" n) implFile
            optEnvExtraLoop, implFile
        else
            env, implFile
        
    let phase3 (env: Optimizer.IncrementalOptimizationEnv, hidden: SignatureHidingInfo, implFile: CheckedImplFile) : Phase3Res =
        // Only do this on the first pass!
        let optSettings =
            { optSettings with
                abstractBigTargets = false
                reportingPhase = false
            }

        let implFile =
            if tcConfig.doDetuple then
                //ReportTime tcConfig ("Detupled optimization")
                let implFile = implFile |> Detuple.DetupleImplFile ccu tcGlobals
                //PrintWholeAssemblyImplementation tcConfig outfile "post-detuple" implFile
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

            //ReportTime tcConfig ("Final simplify pass")
            let (optEnvFinalSimplify, implFile, _, _), _ =
                Optimizer.OptimizeImplFile(
                    optSettings,
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

            //PrintWholeAssemblyImplementation tcConfig outfile "post-rec-opt" implFile
            optEnvFinalSimplify, implFile
        else
            env, implFile
    
    let results, optEnvFirstLoop =
        match optimizerMode with
        | OptimizerMode.PartiallyParallel ->
            let a, b =
                go env0 (phase1, phase2, phase3) implFiles
            a |> Array.toList, b
        | OptimizerMode.Sequential ->
            let results, (optEnvFirstLoop, _, _, _) =
                ((optEnv0, optEnv0, optEnv0, SignatureHidingInfo.Empty), implFiles)

                ||> List.mapFold (fun (optEnvFirstLoop, optEnvExtraLoop, optEnvFinalSimplify, hidden) implFile ->
                    // Phase 1
                    //ReportTime tcConfig ("Initial simplify")
                    let (optEnvFirstLoop, implFile, implFileOptData, hidden), optimizeDuringCodeGen = phase1 (optEnvFirstLoop, hidden, implFile)

                    // Phase 2
                    let optEnvExtraLoop, implFile = phase2 (optEnvExtraLoop, hidden, implFileOptData, implFile)

                    // Phase 3
                    let optEnvFinalSimplify, implFile = phase3 (optEnvFinalSimplify, hidden, implFile)
                    
                    let implFile =
                        {
                            ImplFile = implFile
                            OptimizeDuringCodeGen = optimizeDuringCodeGen
                        }

                    (implFile, implFileOptData), (optEnvFirstLoop, optEnvExtraLoop, optEnvFinalSimplify, hidden))
            results, optEnvFirstLoop

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
