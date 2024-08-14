module TypeChecks.DependencyResolutionTests

open TypeChecks.TestUtils
open NUnit.Framework
open FSharp.Compiler.GraphChecking
open Scenarios

let scenarios = codebases

[<TestCaseSource(nameof scenarios)>]
let ``Supported scenario`` (scenario: Scenario) =
    let files = scenario.Files |> Array.map (fun f -> TestFileWithAST.Map f.FileWithAST) 
    let filePairs = FilePairMap(files)
    let graph, _trie = DependencyResolution.mkGraph false filePairs files

    for file in scenario.Files do
        let expectedDeps = file.ExpectedDependencies
        let actualDeps = set graph.[file.FileWithAST.Idx]
        Assert.AreEqual(expectedDeps, actualDeps, $"Dependencies don't match for {System.IO.Path.GetFileName file.FileWithAST.File}")
