module ParallelTypeCheckingTests.TestDependencyResolution
#nowarn "1182"
open Buildalyzer
open ParallelTypeCheckingTests
open ParallelTypeCheckingTests.Types
open ParallelTypeCheckingTests.Utils
open ParallelTypeCheckingTests.DepResolving
open NUnit.Framework
open Newtonsoft.Json

let buildFiles (files : (string * string) seq) =
    files
    |> Seq.mapi (fun i (name, code) ->
        {
            Idx = FileIdx.make i
            AST = parse name code
        })
    |> Seq.toArray

let private assertGraphEqual (graph : DepsResult) (expected : (string * string list) seq) =
    let edges =
        graph.Edges()
        // Here we disregard directory path, but that's ok for current test cases.
        |> Array.map (fun (node, dep) -> node.ToString(), dep.ToString())
    
    let expectedEdges = expected |> Seq.collect (fun (node, deps) -> deps |> List.map (fun d -> node, d))
    
    Assert.That(edges, Is.EquivalentTo expectedEdges)

[<Test>]
let ``Simple 'open' reference is detected``() =
    let files =
        [|
            "A.fs", """module A"""
            "B.fs", """module B
open A
"""
        |]
        |> buildFiles
        
    let deps = DependencyResolution.detectFileDependencies files
    
    let expectedEdges =
        [
            "B.fs", ["A.fs"]
        ]
    assertGraphEqual deps expectedEdges
    
[<Test>]
let ``With no references there is no dependency``() =
    let files =
        [|
            "A.fs", """module A"""
            "B.fs", """module B; let x = 1"""
        |]
        |> buildFiles
        
    let deps = DependencyResolution.detectFileDependencies files
    
    let expectedEdges =
        [
        ]
    assertGraphEqual deps expectedEdges
    
[<Test>]
let ``Impl files always depend on their backing signature files, but not always on other signature files``() =
    let files =
        [|
            "A.fsi", """
module A
"""
            "A.fs", """
module A
let x = 1
"""
            "B.fs", """
module B
let x = 1
"""
        |]
        |> buildFiles
        
    let deps = DependencyResolution.detectFileDependencies files
    
    let expectedEdges =
        [
            "A.fs", ["A.fsi"]
        ]
    assertGraphEqual deps expectedEdges


let sampleFiles =
    [
        "Abbr.fs", """
module Abbr

module X = A
"""
        "A.fsi", """
module A
val a : int
type X = int
"""
        "A.fs", """
module A
let a = 3
type X = int
"""
        "B.fs", """
namespace B
let b = 3
"""
        "C.fs", """
module C.X
let c = 3
"""
        "D.fs", """
module D
let d : A.X = 3
"""
        "E.fs", """
module E
let e = C.X.x
open A
let x = a
"""
        "F.fs", """
module F
open C
let x = X.c
"""
        "G.fs", """
namespace GH
type A = int
"""
        "H.fs", """
namespace GH
module GH2 =
    type B = int
"""
        "I.fs", """
namespace GH
module GH3 =
    type B = int
"""
    ]
    |> buildFiles

let analyseResult (result : DepsResult) =
    analyseEfficiency result
    
    let totalDeps = result.Graph |> Seq.sumBy (fun (KeyValue(_k, v)) -> v.Length)
    let topFirstDeps =
        result.Graph
        |> Seq.sumBy (
            fun (KeyValue(_k, v)) ->
                if v.Length = 0 then 0
                else v |> Array.map (fun d -> result.Graph[d].Length) |> Array.max 
        )
    printfn $"TotalDeps: {totalDeps}, topFirstDeps: {topFirstDeps}"

[<Test>]
let ``Analyse hardcoded files``() =
    let deps = DependencyResolution.detectFileDependencies sampleFiles
    printfn "Detected file dependencies:"
    deps.Graph |> Graph.print

let private parseProjectAndGetSourceFiles (projectFile : string) =
    //let cacheDir = "."
    //let getName projectFile = Path.Combine(Path.GetFileName(projectFile), ".fsharp"
    let m = AnalyzerManager()
    let analyzer = m.GetProject(projectFile)
    let results = analyzer.Build()
    // TODO Generalise for multiple TFMs
    let res = results.Results |> Seq.head
    let files = res.SourceFiles
    log "built project using Buildalyzer"
    files

[<TestCase(@"C:\projekty\fsharp\heuristic\tests\FSharp.Compiler.ComponentTests\FSharp.Compiler.ComponentTests.fsproj")>]
[<TestCase(@"C:\projekty\fsharp\fsharp_main\src\Compiler\FSharp.Compiler.Service.fsproj")>]
[<Explicit("Slow as it uses Buildalyzer to analyse (build) projects first")>]
let ``Test fsproj files`` (projectFile : string) =
    log $"Start finding file dependency graph for {projectFile}"
    let files = parseProjectAndGetSourceFiles projectFile
    let files =
        files
        |> Array.Parallel.mapi (fun i f ->
            let code = System.IO.File.ReadAllText(f)
            let ast = getParseResults code
            {
                Idx = FileIdx.make i
                //Code = code
                AST = ast
            } : SourceFile
        )
    let N = files.Length
    log $"{N} files read and parsed"
    
    let graph = DependencyResolution.detectFileDependencies files
    log "Deps detected"
    
    let totalDeps = graph.Graph |> Seq.sumBy (fun (KeyValue(_file, deps)) -> deps.Length)
    let maxPossibleDeps = (N * (N-1)) / 2 
    
    let graphJson = graph.Graph |> Seq.map (fun (KeyValue(file, deps)) -> file.Name, deps |> Array.map (fun _d -> file.Name)) |> dict
    let json = JsonConvert.SerializeObject(graphJson, Formatting.Indented)
    let path = $"{System.IO.Path.GetFileName(projectFile)}.deps.json"
    System.IO.File.WriteAllText(path, json)
    
    log $"Analysed {N} files, detected {totalDeps}/{maxPossibleDeps} file dependencies (%.1f{100.0 * double(totalDeps) / double(maxPossibleDeps)}%%)."
    log $"Wrote graph in {path}"
    
    analyseEfficiency graph
    
    let totalDeps = graph.Graph |> Seq.sumBy (fun (KeyValue(k, v)) -> v.Length)
    let topFirstDeps =
        graph.Graph
        |> Seq.sumBy (
            fun (KeyValue(k, v)) ->
                if v.Length = 0 then 0
                else v |> Array.map (fun d -> graph.Graph[d].Length) |> Array.max 
        )
    printfn $"TotalDeps: {totalDeps}, topFirstDeps: {topFirstDeps}, diff: {totalDeps - topFirstDeps}"