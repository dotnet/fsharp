namespace FSharp.Compiler.Benchmarks

open System
open System.Text
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Text
open FSharp.Compiler.AbstractIL.ILBinaryReader
open BenchmarkDotNet.Attributes
open FSharp.Benchmarks.Common.Categories

// Every `comparison` or `equality` constraint on a BCL type makes the checker walk the type's interface
// hierarchy. On modern .NET the primitives implement the generic math interfaces, so each walk visits
// dozens of interfaces. The file below forces that walk many times over a handful of BCL types.
[<AutoOpen>]
module private TypeHierarchyHelpers =

    let functionsPerType = 300

    let source =
        let sb = StringBuilder()
        sb.AppendLine "module Bench.TypeHierarchy" |> ignore
        sb.AppendLine "open System" |> ignore
        for i in 1 .. functionsPerType do
            sb.AppendLine $"let cmpInt{i} (a: int) (b: int) = compare a b" |> ignore
            sb.AppendLine $"let maxDec{i} (a: decimal) (b: decimal) = max a b" |> ignore
            sb.AppendLine $"let eqDate{i} (a: DateTime) (b: DateTime) = a = b" |> ignore
            sb.AppendLine $"let minGuid{i} (a: Guid) (b: Guid) = min a b" |> ignore
            sb.AppendLine $"let setInt64{i} (xs: int64 list) = Set.ofList xs" |> ignore
            sb.AppendLine $"let mapStr{i} (xs: (string * float) list) = Map.ofList xs" |> ignore
            sb.AppendLine $"let hashSpan{i} (a: TimeSpan) = hash a" |> ignore
        sb.ToString()

    let fileName = "typehierarchy.fsx"

    let getScriptOptions (checker: FSharpChecker) =
        let options, diagnostics =
            checker.GetProjectOptionsFromScript(fileName, SourceText.ofString source, assumeDotNetFramework = false, useSdkRefs = true)
            |> Async.RunSynchronously
        if diagnostics |> List.exists (fun (d: FSharpDiagnostic) -> d.Severity = FSharpDiagnosticSeverity.Error) then
            failwithf "script options had errors: %A" diagnostics
        options

    let check (checker: FSharpChecker) (version: int) (text: string) (options: FSharpProjectOptions) =
        let _, answer =
            checker.ParseAndCheckFileInProject(fileName, version, SourceText.ofString text, options)
            |> Async.RunSynchronously
        match answer with
        | FSharpCheckFileAnswer.Aborted -> failwith "check aborted"
        | FSharpCheckFileAnswer.Succeeded results ->
            let errors = results.Diagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
            if errors.Length > 0 then failwithf "check had errors: %A" errors

/// Type checking a file full of comparison and equality constraints on BCL types.
///
/// `ColdCheck` uses a fresh checker per iteration, so every interface import starts from nothing: the
/// compiler's situation. `WarmRecheck` keeps one checker and edits the file, so the project's import
/// state survives between checks: the IDE's situation.
[<MemoryDiagnoser>]
[<BenchmarkCategory(LongCategory)>]
type TypeHierarchyBenchmarks() =

    let mutable options = Unchecked.defaultof<FSharpProjectOptions>
    let mutable warmChecker = Unchecked.defaultof<FSharpChecker>
    let mutable version = 0

    [<GlobalSetup>]
    member _.Setup() =
        let checker = FSharpChecker.Create()
        options <- getScriptOptions checker
        warmChecker <- FSharpChecker.Create(projectCacheSize = 200)
        check warmChecker 0 source options

    [<Benchmark>]
    member _.ColdCheck() =
        clearReaderCache.Invoke()
        let checker = FSharpChecker.Create(projectCacheSize = 200)
        check checker 0 source options

    [<Benchmark>]
    member _.WarmRecheck() =
        version <- version + 1
        check warmChecker version (source + "\n// edit " + string version) options

    [<IterationCleanup>]
    member _.Cleanup() = ClearAllILModuleReaderCache()
