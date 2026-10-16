namespace FSharp.Compiler.Benchmarks

open System
open System.IO
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Text
open FSharp.Compiler.AbstractIL.ILBinaryReader
open BenchmarkDotNet.Attributes
open FSharp.Benchmarks.Common.Categories

// Importing an assembly should realise only the namespaces the code touches, so a project referencing
// large assemblies but opening a couple of namespaces should read and retain less. Only non-F# assemblies
// take this path: F# ones are unpickled from FSharpSignatureData. Narrow opens one namespace, Wide many -
// Wide is the control that should stay flat.
[<AutoOpen>]
module private NamespaceImportHelpers =

    let narrowSource =
        """module Bench.Narrow
open System
let s: String = String.Empty
let sb = StringComparer.Ordinal"""

    let wideSource =
        """module Bench.Wide
open System
open System.Collections
open System.Collections.Generic
open System.Diagnostics
open System.Globalization
open System.IO
open System.Reflection
open System.Runtime.InteropServices
open System.Text
open System.Threading
open System.Threading.Tasks
let s: String = String.Empty
let l = List<int>()
let d = Dictionary<int, int>()
let sb = StringBuilder()
let ci = CultureInfo.InvariantCulture
let ms = new MemoryStream()"""

    /// Script options: the full framework referenced, so there are many namespaces to (not) realise.
    let getScriptOptions (checker: FSharpChecker) (fileName: string) (source: string) =
        let options, diagnostics =
            checker.GetProjectOptionsFromScript(fileName, SourceText.ofString source, assumeDotNetFramework = false, useSdkRefs = true)
            |> Async.RunSynchronously
        if diagnostics |> List.exists (fun (d: FSharpDiagnostic) -> d.Severity = FSharpDiagnosticSeverity.Error) then
            failwithf "script options had errors: %A" diagnostics
        options

    let check (checker: FSharpChecker) (fileName: string) (source: string) (options: FSharpProjectOptions) =
        let _, answer =
            checker.ParseAndCheckFileInProject(fileName, 0, SourceText.ofString source, options)
            |> Async.RunSynchronously
        match answer with
        | FSharpCheckFileAnswer.Aborted -> failwith "check aborted"
        | FSharpCheckFileAnswer.Succeeded results ->
            let errors = results.Diagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
            if errors.Length > 0 then failwithf "check had errors: %A" errors
            answer

    /// Empty reader cache and a fresh checker, so the namespace trees are built from scratch.
    let coldCheck (fileName: string) (source: string) (options: FSharpProjectOptions) =
        ClearAllILModuleReaderCache()
        let checker = FSharpChecker.Create(projectCacheSize = 200)
        check checker fileName source options |> ignore
        checker

    let consoleAppSource =
        """module Program
open System
[<EntryPoint>]
let main argv =
    Console.WriteLine("Hello, World!")
    let sum = [ 1 .. 10 ] |> List.map (fun x -> x * x) |> List.sum
    Console.WriteLine(sum)
    0"""

    let buildConsoleAppArgv (checker: FSharpChecker) (extraArgs: string list) =
        let dir = Path.Combine(Path.GetTempPath(), "fcsConsoleAppBench")
        Directory.CreateDirectory(dir) |> ignore
        let sourceFile = Path.Combine(dir, "Program.fs")
        File.WriteAllText(sourceFile, consoleAppSource)
        let outFile = Path.Combine(dir, "Program.exe")
        let options = getScriptOptions checker (Path.Combine(dir, "resolve.fsx")) "let x = 1"
        let refs = options.OtherOptions |> Array.filter (fun o -> o.StartsWith "-r:")
        [| yield "fsc.dll"
           yield! refs
           yield "--noframework"
           yield "--target:exe"
           yield "--optimize+"
           yield "--out:" + outFile
           yield! extraArgs
           yield sourceFile |]

/// Cold type-check: allocation here counts the namespace trees built for un-opened namespaces.
[<MemoryDiagnoser>]
[<BenchmarkCategory(LongCategory)>]
type NamespaceImportStartupBenchmarks() =

    let narrowFile = "narrow.fsx"
    let wideFile = "wide.fsx"
    let mutable narrowOptions = Unchecked.defaultof<FSharpProjectOptions>
    let mutable wideOptions = Unchecked.defaultof<FSharpProjectOptions>

    [<GlobalSetup>]
    member _.Setup() =
        // Resolving script references is unrelated to what we measure; do it once.
        let checker = FSharpChecker.Create()
        narrowOptions <- getScriptOptions checker narrowFile narrowSource
        wideOptions <- getScriptOptions checker wideFile wideSource

    [<Benchmark(Baseline = true)>]
    member _.NarrowImport() =
        coldCheck narrowFile narrowSource narrowOptions |> ignore

    [<Benchmark>]
    member _.WideImport() =
        coldCheck wideFile wideSource wideOptions |> ignore

    [<IterationCleanup>]
    member _.Cleanup() = ClearAllILModuleReaderCache()

/// End-to-end compile of a console app: the realistic workload driving reference reading. Each iteration
/// starts with a cleared reader cache.
[<MemoryDiagnoser>]
[<BenchmarkCategory(LongCategory)>]
type ConsoleAppCompileBenchmarks() =

    let mutable checker = Unchecked.defaultof<FSharpChecker>
    let mutable argv = Array.empty<string>

    [<GlobalSetup>]
    member _.Setup() =
        checker <- FSharpChecker.Create()
        argv <- buildConsoleAppArgv checker []

    [<Benchmark>]
    member _.CompileConsoleApp() =
        let diagnostics, exnOpt = checker.Compile(argv) |> Async.RunSynchronously
        match exnOpt with
        | Some e -> raise e
        | None ->
            let errors = diagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
            if errors.Length > 0 then failwithf "compile had errors: %A" errors

    [<IterationCleanup>]
    member _.Cleanup() = ClearAllILModuleReaderCache()

/// Per-phase breakdown of the console-app compile via `--times`, to see the effect on the import phase.
///
/// Not a BDN benchmark: run from Program.fs with the `times` argument.
module TimesProbe =

    let run () =
        let checker = FSharpChecker.Create()
        // Warm up JIT and reference resolution.
        checker.Compile(buildConsoleAppArgv checker []) |> Async.RunSynchronously |> ignore

        for i in 1..3 do
            ClearAllILModuleReaderCache()
            printfn "===== compile %d (--times) =====" i
            let argv = buildConsoleAppArgv checker [ "--times" ]
            let _, exnOpt = checker.Compile(argv) |> Async.RunSynchronously
            exnOpt |> Option.iter raise

/// Cold compile of a real, large project from a captured fsc response file - the "many large references,
/// import a subset" workload. MemoryDiagnoser can't take a runtime file, so this is a standalone probe.
///
/// Run from Program.fs: `compile-project <response-file> <project-dir>` - the project dir becomes the
/// working directory so the response file's relative paths resolve.
module CompileProjectProbe =

    let private forceGC () =
        GC.Collect(2, GCCollectionMode.Forced, blocking = true)
        GC.WaitForPendingFinalizers()
        GC.Collect(2, GCCollectionMode.Forced, blocking = true)

    let run (responseFile: string) (projectDir: string) =
        Environment.CurrentDirectory <- projectDir
        let argv =
            File.ReadAllLines responseFile
            |> Array.filter (fun l -> l.Trim().Length > 0)
        let checker = FSharpChecker.Create()

        let compile () =
            let diagnostics, exnOpt = checker.Compile(argv) |> Async.RunSynchronously
            exnOpt |> Option.iter raise
            diagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error) |> Array.length

        printfn "Compiling %d args (%d refs); warming up..."
            argv.Length (argv |> Array.filter (fun a -> a.StartsWith "-r:") |> Array.length)
        let errs = compile ()
        printfn "warm-up done (%d errors)" errs

        for i in 1..3 do
            ClearAllILModuleReaderCache()
            forceGC ()
            let before = GC.GetTotalAllocatedBytes true
            let sw = System.Diagnostics.Stopwatch.StartNew()
            let errs = compile ()
            sw.Stop()
            let allocated = GC.GetTotalAllocatedBytes true - before
            printfn "run %d: %6.0f ms | allocated %8.1f MB | %d errors"
                i sw.Elapsed.TotalMilliseconds (float allocated / 1024.0 / 1024.0) errs

        // What a long-lived process keeps alive. Isolated as the heap drop when the cache is cleared, so
        // it excludes JIT / checker / GC noise.
        let mb (b: int64) = float b / 1024.0 / 1024.0
        for i in 1..3 do
            ClearAllILModuleReaderCache()
            forceGC ()
            let baseHeap = GC.GetTotalMemory true
            compile () |> ignore
            forceGC ()
            let withCache = GC.GetTotalMemory true
            ClearAllILModuleReaderCache()
            forceGC ()
            let afterClear = GC.GetTotalMemory true
            printfn "retain %d: reader-cache holds %7.1f MB | total post-compile %7.1f MB (base %6.1f, withCache %6.1f, afterClear %6.1f)"
                i (mb (withCache - afterClear)) (mb (withCache - baseHeap)) (mb baseHeap) (mb withCache) (mb afterClear)

/// Retained memory for a real project: keeps ParseAndCheckProject's results alive so the imported
/// structures stay on the heap, as an IDE holding a project's analysis does.
///
/// Run from Program.fs: `retain-project <response-file> <project-dir>`.
module RetainProjectProbe =

    let private forceGC () =
        GC.Collect(2, GCCollectionMode.Forced, blocking = true)
        GC.WaitForPendingFinalizers()
        GC.Collect(2, GCCollectionMode.Forced, blocking = true)

    let run (responseFile: string) (projectDir: string) =
        Environment.CurrentDirectory <- projectDir
        let lines =
            File.ReadAllLines responseFile
            |> Array.filter (fun l -> l.Trim().Length > 0)
        // Kept in response-file order: signature files must precede their implementations.
        let sources =
            lines
            |> Array.filter (fun l -> (l.EndsWith ".fs" || l.EndsWith ".fsi") && not (l.StartsWith "-"))
        let otherOptions =
            lines |> Array.filter (fun l ->
                l <> "fsc.dll" && not (l.StartsWith "-o:") && not (Array.contains l sources))

        let options: FSharpProjectOptions =
            { ProjectFileName = Path.Combine(projectDir, "FSharp.Common.fsproj")
              ProjectId = None
              SourceFiles = sources
              OtherOptions = otherOptions
              ReferencedProjects = [||]
              IsIncompleteTypeCheckEnvironment = false
              UseScriptResolutionRules = false
              LoadTime = System.DateTime(2020, 1, 1)
              UnresolvedReferences = None
              OriginalLoadReferences = []
              Stamp = None }

        let mb (b: int64) = float b / 1024.0 / 1024.0
        printfn "ParseAndCheckProject: %d sources, %d refs"
            sources.Length (otherOptions |> Array.filter (fun o -> o.StartsWith "-r:") |> Array.length)

        // One measurement per process: FSharpChecker's static caches contaminate a second sample.
        ClearAllILModuleReaderCache()
        let checker = FSharpChecker.Create(projectCacheSize = 0)
        forceGC ()
        let baseHeap = GC.GetTotalMemory true
        let results = checker.ParseAndCheckProject(options) |> Async.RunSynchronously
        let errs = results.Diagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error) |> Array.length
        forceGC ()
        let held = GC.GetTotalMemory true
        // Keep the imported structures alive across the measurement.
        GC.KeepAlive results
        GC.KeepAlive checker
        printfn "analysis holds %7.1f MB (base %6.1f -> held %6.1f) | %d errors"
            (mb (held - baseHeap)) (mb baseHeap) (mb held) errs

/// Single-file check in a real project - the IDE hot path - holding the analysis alive so an external heap
/// dump can attribute retained memory per type.
///
/// Run from Program.fs: `check-file <response-file> <project-dir> <file-to-check>`. Prints its PID and
/// sleeps, so `dotnet-gcdump collect -p <pid>` can run.
module CheckFileProbe =

    let private forceGC () =
        GC.Collect(2, GCCollectionMode.Forced, blocking = true)
        GC.WaitForPendingFinalizers()
        GC.Collect(2, GCCollectionMode.Forced, blocking = true)

    let run (responseFile: string) (projectDir: string) (fileToCheck: string) =
        Environment.CurrentDirectory <- projectDir
        let lines = File.ReadAllLines responseFile |> Array.filter (fun l -> l.Trim().Length > 0)
        let sources = lines |> Array.filter (fun l -> l.EndsWith ".fs" && not (l.StartsWith "-"))
        let otherOptions =
            lines |> Array.filter (fun l ->
                l <> "fsc.dll" && not (l.StartsWith "-o:") && not (Array.contains l sources))

        let options: FSharpProjectOptions =
            { ProjectFileName = Path.Combine(projectDir, "FSharp.Common.fsproj")
              ProjectId = None
              SourceFiles = sources
              OtherOptions = otherOptions
              ReferencedProjects = [||]
              IsIncompleteTypeCheckEnvironment = false
              UseScriptResolutionRules = false
              LoadTime = System.DateTime(2020, 1, 1)
              OriginalLoadReferences = []
              UnresolvedReferences = None
              Stamp = None }

        // Keeps the incremental builder, and so the imported assemblies, alive.
        let checker = FSharpChecker.Create(projectCacheSize = 1)
        ClearAllILModuleReaderCache()
        let source = SourceText.ofString (File.ReadAllText fileToCheck)
        let _, answer = checker.ParseAndCheckFileInProject(fileToCheck, 0, source, options) |> Async.RunSynchronously
        let errs =
            match answer with
            | FSharpCheckFileAnswer.Aborted -> failwith "check aborted"
            | FSharpCheckFileAnswer.Succeeded r ->
                r.Diagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error) |> Array.length

        forceGC ()
        let held = GC.GetTotalMemory true
        let pid = System.Diagnostics.Process.GetCurrentProcess().Id
        printfn "checked %s (%d errors)" (Path.GetFileName fileToCheck) errs
        printfn "PID %d retained %.1f MB" pid (float held / 1024.0 / 1024.0)
        printfn "READY_FOR_DUMP"
        Console.Out.Flush()

        // Hold everything rooted while the external dump is collected.
        System.Threading.Thread.Sleep(180000)
        GC.KeepAlive answer
        GC.KeepAlive checker

/// Retained memory after a cold check: MemoryDiagnoser measures allocation during an op, not what
/// survives, and what survives is the point.
///
/// Not a BDN benchmark: run from Program.fs with the `retained-memory` argument.
module RetainedMemoryProbe =

    let private forceGC () =
        GC.Collect(2, GCCollectionMode.Forced, blocking = true)
        GC.WaitForPendingFinalizers()
        GC.Collect(2, GCCollectionMode.Forced, blocking = true)

    let private measureOne label fileName source =
        ClearAllILModuleReaderCache()
        let setupChecker = FSharpChecker.Create()
        let options = getScriptOptions setupChecker fileName source

        // Baseline before any assembly namespaces are read.
        ClearAllILModuleReaderCache()
        let checker = FSharpChecker.Create(projectCacheSize = 200)
        forceGC ()
        let before = GC.GetTotalMemory(true)
        let allocatedBefore = GC.GetTotalAllocatedBytes true

        let answer = check checker fileName source options

        let allocated = GC.GetTotalAllocatedBytes true - allocatedBefore
        forceGC ()
        let after = GC.GetTotalMemory(true)
        // Keep the check's output alive, else the delta is meaningless.
        GC.KeepAlive answer
        GC.KeepAlive checker
        printfn "%-8s retained: %10.2f KB  allocated: %10.2f KB (before %10.2f KB, after %10.2f KB)"
            label (float (after - before) / 1024.0) (float allocated / 1024.0) (float before / 1024.0) (float after / 1024.0)

    /// Forces every type and namespace of every reference, isolating the reader's per-object cost from
    /// anything the type-checker does with it.
    let private measureReadAll () =
        // Implementation assemblies, not reference ones: these hold real type bodies.
        let refs =
            Directory.GetFiles(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory(), "*.dll")

        let readerOptions =
            { pdbDirPath = None
              reduceMemoryUsage = ReduceMemoryFlag.Yes
              metadataOnly = MetadataOnlyFlag.Yes
              tryGetMetadataSnapshot = fun _ -> None }

        ClearAllILModuleReaderCache()
        forceGC ()
        let before = GC.GetTotalMemory true
        let allocatedBefore = GC.GetTotalAllocatedBytes true

        let readers = ResizeArray()
        let mutable typeCount = 0

        let rec forceTypeDefs (tdefs: FSharp.Compiler.AbstractIL.IL.ILTypeDefs) =
            for tdef in tdefs do
                typeCount <- typeCount + 1
                forceTypeDefs tdef.NestedTypes

        for r in refs do
            let reader = OpenILModuleReader r readerOptions
            readers.Add reader
            forceTypeDefs reader.ILModuleDef.TypeDefs

        let allocated = GC.GetTotalAllocatedBytes true - allocatedBefore
        forceGC ()
        let after = GC.GetTotalMemory true
        GC.KeepAlive readers
        printfn "%-8s retained: %10.2f KB  allocated: %10.2f KB (%d assemblies, %d type defs)"
            "ReadAll" (float (after - before) / 1024.0) (float allocated / 1024.0) refs.Length typeCount

    /// Forces the import of every entity of every reference, isolating the IL-to-TAST cost.
    let private measureImportAll () =
        let fileName = "importall.fsx"
        let setupChecker = FSharpChecker.Create()
        let options = getScriptOptions setupChecker fileName narrowSource

        ClearAllILModuleReaderCache()
        let checker = FSharpChecker.Create(projectCacheSize = 200)
        forceGC ()
        let before = GC.GetTotalMemory true
        let allocatedBefore = GC.GetTotalAllocatedBytes true

        let answer = check checker fileName narrowSource options

        let results =
            match answer with
            | FSharpCheckFileAnswer.Succeeded results -> results
            | FSharpCheckFileAnswer.Aborted -> failwith "check aborted"

        let mutable entityCount = 0

        let rec walk (entity: FSharp.Compiler.Symbols.FSharpEntity) =
            entityCount <- entityCount + 1
            for nested in entity.NestedEntities do
                walk nested

        for asm in results.ProjectContext.GetReferencedAssemblies() do
            for entity in asm.Contents.Entities do
                walk entity

        let allocated = GC.GetTotalAllocatedBytes true - allocatedBefore
        forceGC ()
        let after = GC.GetTotalMemory true
        GC.KeepAlive answer
        GC.KeepAlive checker
        printfn "%-8s retained: %10.2f KB  allocated: %10.2f KB (%d entities)"
            "ImportAll" (float (after - before) / 1024.0) (float allocated / 1024.0) entityCount

    let run () =
        printfn "Retained-memory probe (lower is better; compare across branches):"
        measureOne "Narrow" "narrow.fsx" narrowSource
        measureOne "Wide" "wide.fsx" wideSource
        measureReadAll ()
        measureImportAll ()
