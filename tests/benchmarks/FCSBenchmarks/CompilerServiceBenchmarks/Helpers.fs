module FSharp.Compiler.Benchmarks.Helpers

open System
open System.IO
open FSharp.Compiler.CodeAnalysis

let createProject name referencedProjects =
    let tmpPath = Path.GetTempPath()
    let file = Path.Combine(tmpPath, Path.ChangeExtension(name, ".fs"))
    {
        ProjectFileName = Path.Combine(tmpPath, Path.ChangeExtension(name, ".dll"))
        ProjectId = None
        SourceFiles = [|file|]
        OtherOptions = 
            Array.append [|"--optimize+"; "--target:library" |] (referencedProjects |> Array.ofList |> Array.map (fun x -> "-r:" + x.ProjectFileName))
        ReferencedProjects =
            referencedProjects
            |> List.map (fun x -> FSharpReferencedProject.CreateFSharp (x.ProjectFileName, x))
            |> Array.ofList
        IsIncompleteTypeCheckEnvironment = false
        UseScriptResolutionRules = false
        LoadTime = DateTime()
        UnresolvedReferences = None
        OriginalLoadReferences = []
        Stamp = Some 0L (* set the stamp to 0L on each run so we don't evaluate the whole project again *)
    }

let generateSourceCode moduleName =
    $"""
module Benchmark.%s{moduleName}

type %s{moduleName} =

val X : int

val Y : int

val Z : int

let function%s{moduleName} (x: %s{moduleName}) =
let x = 1
let y = 2
let z = x + y
z"""

let sourcePath = "../decentlySizedStandAloneFile.fs"
let decentlySizedStandAloneFile = File.ReadAllText(Path.Combine(__SOURCE_DIRECTORY__, sourcePath))