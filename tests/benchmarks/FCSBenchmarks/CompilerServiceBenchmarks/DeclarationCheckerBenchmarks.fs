// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Benchmarks

open System
open System.IO
open System.Runtime.CompilerServices
open System.Text
open System.Threading
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.CodeAnalysis.Declarations
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Text
open BenchmarkDotNet.Attributes
open FSharp.Benchmarks.Common.Categories

[<AutoOpen>]
module private DeclarationCheckerBenchHelpers =

    let mkSdkProjectOptions (tempDir: string) (filePath: string) (outDllName: string) =
        let checker = FSharpChecker.Create(projectCacheSize = 200)
        let probePath = Path.Combine(tempDir, "ResolveSdkProbe.fsx")

        let probeOptions, probeDiags =
            checker.GetProjectOptionsFromScript(probePath, SourceText.ofString "let x = 1", assumeDotNetFramework = false, useSdkRefs = true)
            |> Async.RunSynchronously

        let probeErrors =
            probeDiags |> List.filter (fun (d: FSharpDiagnostic) -> d.Severity = FSharpDiagnosticSeverity.Error)

        if not probeErrors.IsEmpty then
            failwithf "Script SDK resolution had errors: %A" probeErrors

        let refs = probeOptions.OtherOptions |> Array.filter (fun o -> o.StartsWith("-r:", StringComparison.Ordinal))
        let outDll = Path.Combine(tempDir, outDllName)

        {
            ProjectFileName = Path.Combine(tempDir, Path.ChangeExtension(outDllName, ".fsproj"))
            ProjectId = None
            SourceFiles = [| filePath |]
            OtherOptions =
                [|
                    yield "--noframework"
                    yield "--target:library"
                    yield "--optimize-"
                    yield "--out:" + outDll
                    yield! refs
                |]
            ReferencedProjects = [||]
            IsIncompleteTypeCheckEnvironment = false
            UseScriptResolutionRules = false
            LoadTime = DateTime(2020, 1, 1)
            UnresolvedReferences = None
            OriginalLoadReferences = []
            Stamp = Some 0L
        }

    let mkDeclarationsSource (bodyConst: int) (extraLinesAfterFirst: int) =
        let sb = StringBuilder()
        sb.AppendLine("module Bench.Declarations") |> ignore
        sb.AppendLine("") |> ignore

        for i in 0..59 do
            sb.AppendLine($"/// Step %02d{i}") |> ignore

            if i = 30 then
                sb.AppendLine($"let compute%02d{i} (x: int) =") |> ignore
                sb.AppendLine($"    let items = [ x; x + 1; x + 2 ] |> List.map (fun v -> v * 2 + %d{bodyConst})") |> ignore
                sb.AppendLine("    items |> List.sum") |> ignore
            elif i >= 31 && i <= 35 then
                sb.AppendLine($"let compute%02d{i} (x: int) =") |> ignore
                sb.AppendLine("    let baseVal = compute30 x") |> ignore
                sb.AppendLine($"    let items = [ baseVal; x + %d{i}; x + %d{i + 1} ]") |> ignore
                sb.AppendLine("    items |> List.sum") |> ignore
            else
                sb.AppendLine($"let compute%02d{i} (x: int) =") |> ignore
                sb.AppendLine($"    let items = [ x; x + %d{i + 1}; x + %d{i + 2} ] |> List.map (fun v -> v * 2 + 1)") |> ignore
                sb.AppendLine("    items |> List.sum") |> ignore

            sb.AppendLine("") |> ignore

            if i = 0 then
                for _ in 1..extraLinesAfterFirst do
                    sb.AppendLine("") |> ignore

        sb.ToString()

    let checkAllDeclarations (ctx: ProjectContext) (parse: FSharpParseFileResults) =
        let start, env0 = DeclarationChecker.BeginFile(ctx, [], parse)
        let decls = DeclarationChecker.Split parse
        let results = Array.zeroCreate<DeclarationResult> decls.Length
        let envsBefore = Array.zeroCreate<DeclarationEnvironment> decls.Length
        let mutable env = env0

        for i in 0 .. decls.Length - 1 do
            envsBefore[i] <- env
            let r = DeclarationChecker.CheckDeclaration(start, env, decls[i], CancellationToken.None)
            results[i] <- r
            env <- DeclarationChecker.ExtendEnv(env, r)

        let fileRes = DeclarationChecker.FinishFile(start, results, parse)

        if not (Array.isEmpty fileRes.Diagnostics) then
            failwithf "Expected zero diagnostics in setup, got %A" fileRes.Diagnostics

        start, decls, envsBefore, results, fileRes

[<MemoryDiagnoser>]
[<BenchmarkCategory(ShortCategory)>]
type DeclarationCheckerBenchmarks() =

    let tempDir = Path.Combine(Path.GetTempPath(), $"fcs-decl-bench-{Guid.NewGuid():N}")
    let filePath = Path.Combine(tempDir, "DeclarationsBench.fs")
    let wholeFilePath = Path.Combine(tempDir, "WholeFileBench.fs")

    let mutable wholeChecker = Unchecked.defaultof<FSharpChecker>
    let mutable wholeOptions = Unchecked.defaultof<FSharpProjectOptions>
    let mutable ctx = Unchecked.defaultof<ProjectContext>

    let mutable bodyEditA = ""
    let mutable bodyEditB = ""
    let mutable bodyTextA = Unchecked.defaultof<ISourceText>
    let mutable bodyTextB = Unchecked.defaultof<ISourceText>

    let mutable shiftedA = ""
    let mutable shiftedB = ""

    let mutable editIdx = 0
    let mutable basePrintEdit = ""

    let mutable bodyCutoffStart = Unchecked.defaultof<FileStart>
    let mutable bodyCutoffEnvsBefore = Array.empty<DeclarationEnvironment>
    let mutable bodyCutoffResults = Array.empty<DeclarationResult>

    let mutable bodyFullStart = Unchecked.defaultof<FileStart>
    let mutable bodyFullEnvsBefore = Array.empty<DeclarationEnvironment>
    let mutable bodyFullResults = Array.empty<DeclarationResult>

    let mutable shiftStart = Unchecked.defaultof<FileStart>
    let mutable shiftResults = Array.empty<DeclarationResult>

    let mutable wholeStep = 0
    let mutable wholeDeclStep = 0
    let mutable cutoffStep = 0
    let mutable fullStep = 0
    let mutable shiftStep = 0

    [<GlobalSetup>]
    member _.Setup() =
        Directory.CreateDirectory(tempDir) |> ignore

        bodyEditA <- DeclarationCheckerBenchHelpers.mkDeclarationsSource 101 0
        bodyEditB <- DeclarationCheckerBenchHelpers.mkDeclarationsSource 202 0
        bodyTextA <- SourceText.ofString bodyEditA
        bodyTextB <- SourceText.ofString bodyEditB

        shiftedA <- DeclarationCheckerBenchHelpers.mkDeclarationsSource 101 2
        shiftedB <- DeclarationCheckerBenchHelpers.mkDeclarationsSource 101 4

        File.WriteAllText(filePath, bodyEditA)
        File.WriteAllText(wholeFilePath, bodyEditA)

        wholeChecker <- FSharpChecker.Create(projectCacheSize = 200, keepAssemblyContents = true)
        wholeOptions <- DeclarationCheckerBenchHelpers.mkSdkProjectOptions tempDir wholeFilePath "WholeFileBench.dll"
        let declOptions = DeclarationCheckerBenchHelpers.mkSdkProjectOptions tempDir filePath "DeclarationsBench.dll"

        // Warm FSharpChecker once so TcImports and project setup are excluded from warm ops.
        match wholeChecker.ParseAndCheckFileInProject(wholeFilePath, 0, bodyTextA, wholeOptions) |> Async.RunSynchronously with
        | _, FSharpCheckFileAnswer.Succeeded r when r.Diagnostics.Length = 0 -> ()
        | _, other -> failwithf "Warmup wholeChecker failed: %A" other

        ctx <-
            DeclarationChecker.CreateProject(declOptions)
            |> Async.RunSynchronously

        let parseCutoff = DeclarationChecker.Parse(ctx, filePath, bodyEditA)
        let s1, decls1, envs1, res1, _ = DeclarationCheckerBenchHelpers.checkAllDeclarations ctx parseCutoff

        if decls1.Length < 60 then
            failwithf "Expected at least 60 declarations, got %d" decls1.Length

        editIdx <- decls1 |> Array.findIndex (fun d -> d.Key.Name = "compute30")
        basePrintEdit <- res1[editIdx].Fingerprint
        bodyCutoffStart <- s1
        bodyCutoffEnvsBefore <- envs1
        bodyCutoffResults <- res1

        let parseFull = DeclarationChecker.Parse(ctx, filePath, bodyEditA)
        let s2, _, envs2, res2, _ = DeclarationCheckerBenchHelpers.checkAllDeclarations ctx parseFull
        bodyFullStart <- s2
        bodyFullEnvsBefore <- envs2
        bodyFullResults <- res2

        let parseShift = DeclarationChecker.Parse(ctx, filePath, bodyEditA)
        let s3, _, _, res3, _ = DeclarationCheckerBenchHelpers.checkAllDeclarations ctx parseShift
        shiftStart <- s3
        shiftResults <- res3

    [<Benchmark(Baseline = true)>]
    member _.BodyEdit_WholeFile_FSharpChecker() : int =
        wholeStep <- wholeStep + 1
        let srcText = if wholeStep &&& 1 = 1 then bodyTextB else bodyTextA
        let _, answer = wholeChecker.ParseAndCheckFileInProject(wholeFilePath, wholeStep, srcText, wholeOptions) |> Async.RunSynchronously

        match answer with
        | FSharpCheckFileAnswer.Aborted -> failwith "Whole-file check aborted"
        | FSharpCheckFileAnswer.Succeeded res ->
            if res.Diagnostics.Length > 0 then
                failwithf "Whole-file check had diagnostics: %A" res.Diagnostics

            res.Diagnostics.Length

    [<Benchmark>]
    member _.BodyEdit_WholeFile_DeclarationChecker() : int =
        // CheckFile validates types and signature without building TcResultsSink unless CheckResults is read.
        wholeDeclStep <- wholeDeclStep + 1
        let src = if wholeDeclStep &&& 1 = 1 then bodyEditB else bodyEditA
        let parse = DeclarationChecker.Parse(ctx, filePath, src)
        let fileRes = DeclarationChecker.CheckFile(ctx, [], parse)

        if fileRes.Diagnostics.Length > 0 then
            failwithf "CheckFile had diagnostics: %A" fileRes.Diagnostics

        fileRes.Signature.Fingerprint.Length

    [<Benchmark>]
    member _.BodyEdit_Incremental_Cutoff() : int =
        cutoffStep <- cutoffStep + 1
        let src = if cutoffStep &&& 1 = 1 then bodyEditB else bodyEditA
        let parse = DeclarationChecker.Parse(ctx, filePath, src)
        let decls = DeclarationChecker.Split parse

        let r =
            DeclarationChecker.CheckDeclaration(
                bodyCutoffStart,
                bodyCutoffEnvsBefore[editIdx],
                decls[editIdx],
                CancellationToken.None
            )

        if r.Fingerprint <> basePrintEdit then
            failwith "Expected unchanged declaration signature fingerprint for body-only edit"

        bodyCutoffResults[editIdx] <- r
        let fileRes = DeclarationChecker.FinishFile(bodyCutoffStart, bodyCutoffResults, parse)

        if fileRes.Diagnostics.Length > 0 then
            failwithf "FinishFile had diagnostics: %A" fileRes.Diagnostics

        fileRes.Reused

    [<Benchmark>]
    member _.BodyEdit_Incremental_WithCheckResults() : bool =
        fullStep <- fullStep + 1
        let src = if fullStep &&& 1 = 1 then bodyEditB else bodyEditA
        let parse = DeclarationChecker.Parse(ctx, filePath, src)
        let decls = DeclarationChecker.Split parse

        let r =
            DeclarationChecker.CheckDeclaration(
                bodyFullStart,
                bodyFullEnvsBefore[editIdx],
                decls[editIdx],
                CancellationToken.None
            )

        if r.Fingerprint <> basePrintEdit then
            failwith "Expected unchanged declaration signature fingerprint for body-only edit"

        bodyFullResults[editIdx] <- r
        let fileRes = DeclarationChecker.FinishFile(bodyFullStart, bodyFullResults, parse)
        let checkRes = fileRes.CheckResults

        if checkRes.Diagnostics.Length > 0 then
            failwithf "CheckResults had diagnostics: %A" checkRes.Diagnostics

        checkRes.HasFullTypeCheckInfo

    [<Benchmark>]
    member _.LineShift_Incremental_DeclarationChecker() : int =
        shiftStep <- shiftStep + 1
        let src = if shiftStep &&& 1 = 1 then shiftedA else shiftedB
        let parse = DeclarationChecker.Parse(ctx, filePath, src)
        let decls = DeclarationChecker.Split parse
        let mutable shiftedCount = 0

        for i in 0 .. decls.Length - 1 do
            if decls[i].Range.StartLine <> shiftResults[i].Declaration.Range.StartLine then
                match DeclarationChecker.Shift(shiftResults[i], decls[i]) with
                | ValueSome shifted ->
                    shiftResults[i] <- shifted
                    shiftedCount <- shiftedCount + 1
                | ValueNone -> failwithf "Shift refused declaration %d (%s)" i decls[i].Key.Name

        if shiftedCount < 59 then
            failwithf "Expected at least 59 shifted declarations, got %d" shiftedCount

        let fileRes = DeclarationChecker.FinishFile(shiftStart, shiftResults, parse)

        if fileRes.Diagnostics.Length > 0 then
            failwithf "FinishFile after Shift had diagnostics: %A" fileRes.Diagnostics

        fileRes.Reused

    [<GlobalCleanup>]
    member _.Cleanup() =
        try
            Directory.Delete(tempDir, true)
        with _ ->
            ()

/// Retained live-heap comparison for a 60-declaration checked file after ProjectContext/TcImports warmup.
///
/// Not a BDN benchmark: run from Program.fs with the `declaration-memory` argument.
module DeclarationMemoryProbe =

    [<MethodImpl(MethodImplOptions.NoInlining)>]
    let private keepClosedSignature (ctx: ProjectContext) filePath src =
        let parse = DeclarationChecker.Parse(ctx, filePath, src)
        let fileRes = DeclarationChecker.CheckFile(ctx, [], parse)
        fileRes.Signature

    [<MethodImpl(MethodImplOptions.NoInlining)>]
    let private keepIncrementalState (ctx: ProjectContext) filePath src =
        let parse = DeclarationChecker.Parse(ctx, filePath, src)
        let start, _, _, results, fileRes = DeclarationCheckerBenchHelpers.checkAllDeclarations ctx parse
        box (start, results, fileRes.Signature)

    [<MethodImpl(MethodImplOptions.NoInlining)>]
    let private keepFullCheckResults (checker: FSharpChecker) filePath srcText options =
        let _, answer = checker.ParseAndCheckFileInProject(filePath, 1, srcText, options) |> Async.RunSynchronously

        match answer with
        | FSharpCheckFileAnswer.Succeeded res -> res
        | FSharpCheckFileAnswer.Aborted -> failwith "Probe check aborted"

    let private printRow (label: string) (before: int64) (after: int64) (allocated: int64) =
        printfn "| %-30s | %10.2f KB | %10.2f KB |" label (float (after - before) / 1024.0) (float allocated / 1024.0)

    let run () =
        let tempDir = Path.Combine(Path.GetTempPath(), $"fcs-decl-mem-{Guid.NewGuid():N}")
        Directory.CreateDirectory(tempDir) |> ignore

        try
            let warmPath = Path.Combine(tempDir, "Warmup.fs")
            let targetPath = Path.Combine(tempDir, "Target60.fs")
            let warmSrc = "module Bench.Warmup\nlet ready = 1\n"
            let targetSrc = DeclarationCheckerBenchHelpers.mkDeclarationsSource 101 0
            File.WriteAllText(warmPath, warmSrc)
            File.WriteAllText(targetPath, targetSrc)

            let baseOpts = DeclarationCheckerBenchHelpers.mkSdkProjectOptions tempDir targetPath "MemProbe.dll"
            let multiOpts = { baseOpts with SourceFiles = [| warmPath; targetPath |] }

            printfn "Declaration retention probe (60-declaration file, post-warmup TcImports):"
            printfn "| %-30s | %13s | %13s |" "State Retained" "Retained" "Allocated"
            printfn "| %s | %13s | %13s |" (String('-', 30)) "------------:" "------------:"

            // 1. Closed-file FileSignature only (demoted project-graph file state).
            do
                let ctx = DeclarationChecker.CreateProject(baseOpts) |> Async.RunSynchronously
                let warmParse = DeclarationChecker.Parse(ctx, targetPath, warmSrc)
                DeclarationChecker.CheckFile(ctx, [], warmParse) |> ignore
                forceGC ()
                let before = GC.GetTotalMemory true
                let allocBefore = GC.GetTotalAllocatedBytes true
                let sigOnly = keepClosedSignature ctx targetPath targetSrc
                let allocated = GC.GetTotalAllocatedBytes true - allocBefore
                forceGC ()
                let after = GC.GetTotalMemory true
                printRow "ClosedFile_FileSignature" before after allocated
                GC.KeepAlive sigOnly
                GC.KeepAlive ctx

            // 2. Open-file incremental state (FileStart + DeclarationResult[] + FileSignature, no CheckResults).
            do
                let ctx = DeclarationChecker.CreateProject(baseOpts) |> Async.RunSynchronously
                let warmParse = DeclarationChecker.Parse(ctx, targetPath, warmSrc)
                DeclarationChecker.CheckFile(ctx, [], warmParse) |> ignore
                forceGC ()
                let before = GC.GetTotalMemory true
                let allocBefore = GC.GetTotalAllocatedBytes true
                let incState = keepIncrementalState ctx targetPath targetSrc
                let allocated = GC.GetTotalAllocatedBytes true - allocBefore
                forceGC ()
                let after = GC.GetTotalMemory true
                printRow "OpenFile_IncrementalState" before after allocated
                GC.KeepAlive incState
                GC.KeepAlive ctx

            // 3. Standard FSharpChecker full FSharpCheckFileResults (warm IncrementalBuilder via Warmup.fs).
            do
                let checker = FSharpChecker.Create(projectCacheSize = 200, keepAssemblyContents = true)
                let warmText = SourceText.ofString warmSrc
                let targetText = SourceText.ofString targetSrc
                checker.ParseAndCheckFileInProject(warmPath, 0, warmText, multiOpts) |> Async.RunSynchronously |> ignore
                forceGC ()
                let before = GC.GetTotalMemory true
                let allocBefore = GC.GetTotalAllocatedBytes true
                let checkRes = keepFullCheckResults checker targetPath targetText multiOpts
                let allocated = GC.GetTotalAllocatedBytes true - allocBefore
                forceGC ()
                let after = GC.GetTotalMemory true
                printRow "FSharpChecker_CheckFileResults" before after allocated
                GC.KeepAlive checkRes
                GC.KeepAlive checker
        finally
            try
                Directory.Delete(tempDir, true)
            with _ ->
                ()
