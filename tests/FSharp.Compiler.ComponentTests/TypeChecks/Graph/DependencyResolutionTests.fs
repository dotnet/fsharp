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

/// Verify that Module+Module merge in the trie creates dependency edges to both files.
/// Without the merge fix, the second module's file index would be lost, and the consumer
/// would only depend on the first file.
[<Fact>]
let ``Module+Module merge creates dependency edges to both defining files`` () =
    let files =
        [|
            {
                Idx = 0
                FileName = "M1.fs"
                ParsedInput =
                    parseSourceCode (
                        "M1.fs",
                        """
namespace N

module M =
    let x = 1
"""
                    )
            }
            {
                Idx = 1
                FileName = "M2.fs"
                ParsedInput =
                    parseSourceCode (
                        "M2.fs",
                        """
namespace N

module M =
    let y = 2
"""
                    )
            }
            {
                Idx = 2
                FileName = "Consumer.fs"
                ParsedInput =
                    parseSourceCode (
                        "Consumer.fs",
                        """
namespace N.Sub
open N

module C =
    let z = M.x + M.y
"""
                    )
            }
        |]

    let filePairs = FilePairMap(files)
    let graph, _trie = DependencyResolution.mkGraph filePairs files
    let consumerDeps = set graph.[2]

    Assert.True(
        consumerDeps.Contains 0,
        "Consumer should depend on first file defining module M"
    )

    Assert.True(
        consumerDeps.Contains 1,
        "Consumer should depend on second file defining module M (requires Module+Module merge)"
    )
