module FSharp.Compiler.ComponentTests.TypeChecks.Graph.DependencyResolutionTests

open NUnit.Framework
open FSharp.Compiler.GraphChecking
open Scenarios

let scenarios = codebases

[<TestCaseSource(nameof scenarios)>]
let ``Supported scenario`` (scenario: Scenario) =
    let files = Array.map (fun f -> f.FileWithAST) scenario.Files
    let filePairs = FilePairMap(files)
    let graph = DependencyResolution.mkGraph filePairs files

    for file in scenario.Files do
        let expectedDeps = file.ExpectedDependencies
        let actualDeps = graph.[file.FileWithAST.Idx]
        Assert.AreEqual(expectedDeps, actualDeps, $"Dependencies don't match for {System.IO.Path.GetFileName file.FileWithAST.File}")
