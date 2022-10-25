﻿module FSharp.Compiler.Service.Tests2.TestDepResolving

open Buildalyzer
open FSharp.Compiler.Service.Tests
open FSharp.Compiler.Service.Tests2.ASTVisit
open FSharp.Compiler.Service.Tests2.DepResolving
open NUnit.Framework
open Newtonsoft.Json

let sampleFiles =
    
    // This file should be marked as depending on everything above and being depended upon by everything below
    let WithAbbreviations =
        """
module Abbr

module X = A
"""
    
    let A_fsi =
        """
module A
val a : int
type X = int
"""
    
    let A =
        """
module A
let a = 3
type X = int
""" 
    let B =
        """
namespace B
let b = 3
"""
    let C =
        """
module C.X
let c = 3
"""
    let D =
        """
module D
let d : A.X = 3
"""
    let E =
        """
module E
let e = C.X.x
open A
let x = a
"""
    let F =
        """
module F
open C
let x = X.c
"""
    let G =
        """
namespace GH
type A = int
"""
    let H =
        """
namespace GH
module GH2 =
    type B = int
"""
    let I =
        """
namespace GH
module GH3 =
    type B = int
"""

    [
        "Abbr.fs", WithAbbreviations
        "A.fsi", A_fsi
        "A.fs", A
        "B.fs", B
        "C.fs", C
        "D.fs", D
        "E.fs", E
        "F.fs", F
        "G.fs", G
        "H.fs", H
        "I.fs", I
    ]

[<Test>]
let TestHardcodedFiles() =
   
    let nodes =
        sampleFiles
        |> List.map (fun (name, code) -> {FileAST.Name = name; FileAST.Code = code; FileAST.AST = parseSourceCode(name, code)})
        |> List.toArray
    
    let graph = AutomatedDependencyResolving.detectFileDependencies nodes

    printfn "Detected file dependencies:"
    graph.Graph
    |> Seq.iter (fun (KeyValue(idx, deps)) -> printfn $"{graph.Files[idx].Name} -> %+A{deps |> Array.map(fun d -> graph.Files[d].Name)}")
    
    analyseEfficiency graph
    
    let totalDeps = graph.Graph |> Seq.sumBy (fun (KeyValue(k, v)) -> v.Length)
    let topFirstDeps =
        graph.Graph
        |> Seq.sumBy (
            fun (KeyValue(k, v)) ->
                if v.Length = 0 then 0
                else v |> Array.map (fun d -> graph.Graph[d].Length) |> Array.max 
        )
    printfn $"TotalDeps: {totalDeps}, topFirstDeps: {topFirstDeps}"

let private parseProjectAndGetSourceFiles (projectFile : string) =
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
let TestProject (projectFile : string) =
    log $"Start finding file dependency graph for {projectFile}"
    let files = parseProjectAndGetSourceFiles projectFile
    let files =
        files
        |> Array.Parallel.map (fun f ->
            let code = System.IO.File.ReadAllText(f)
            let ast = getParseResults code
            {Name = f; Code = code; AST = ast}
        )
        |> Array.filter (fun x ->
            // true
            ASTVisit.extractModuleRefs x.AST
            |> Array.forall (function | ReferenceOrAbbreviation.Reference _ -> true | ReferenceOrAbbreviation.Abbreviation _ -> false)
        )
    let N = files.Length
    log $"{N} files read and parsed"
    
    let graph = AutomatedDependencyResolving.detectFileDependencies files
    log "Deps detected"
    
    let totalDeps = graph.Graph |> Seq.sumBy (fun (KeyValue(idx, deps)) -> deps.Length)
    let maxPossibleDeps = (N * (N-1)) / 2 
    
    let graphJson = graph.Graph |> Seq.map (fun (KeyValue(idx, deps)) -> graph.Files[idx].Name, deps |> Array.map (fun d -> graph.Files[d].Name)) |> dict
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