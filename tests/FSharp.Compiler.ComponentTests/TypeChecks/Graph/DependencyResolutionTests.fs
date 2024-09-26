module TypeChecks.DependencyResolutionTests

open TypeChecks.TestUtils
open Xunit
open FSharp.Compiler.GraphChecking
open Scenarios

let scenarios = scenarios |> Seq.map (fun p -> [| box p |])

[<Theory>]
[<MemberData(nameof scenarios)>]
let ``Supported scenario`` (scenario: Scenario) =
    let files =
        scenario.Files
        |> List.map (fun f ->
            {Idx = f.Index; FileName = f.FileName; ParsedInput = parseSourceCode(f.FileName, f.Content)}) 
        |> List.toArray
    let filePairs = FilePairMap(files)
    let graph, _trie = DependencyResolution.mkGraph filePairs files

    for file in scenario.Files do
        let expectedDeps = file.ExpectedDependencies
        let actualDeps = set graph.[file.Index]
        Assert.True((expectedDeps = actualDeps), $"Dependencies don't match for {System.IO.Path.GetFileName file.FileName}")
