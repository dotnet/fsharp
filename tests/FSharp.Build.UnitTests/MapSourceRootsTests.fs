
namespace FSharp.Build.UnitTests

open Microsoft.Build.Framework
open Microsoft.Build.Utilities
open FSharp.Build
open NUnit.Framework
open System.Collections.Generic

type SourceRoot = 
    SourceRoot of 
        path: string * 
        props: list<string * string> *
        expectedProps: list<string * string>

/// these tests are ported from https://github.com/dotnet/roslyn/blob/093ea477717001c58be6231cf2a793f4245cbf72/src/Compilers/Core/MSBuildTaskTests/MapSourceRootTests.cs
/// Same scenarios, slightly different setup/teardown
[<TestFixture>]
type MapSourceRootsTests() =
    
    let toTaskItem (SourceRoot(path, props, _)) = 
        let dict = Dictionary()
        for (k, v) in props do dict.Add(k, v)
        TaskItem(path, dict) :> ITaskItem
    
    let checkExpectations position (SourceRoot(path, _, expectedProps), mapping: ITaskItem) = 
        Assert.AreEqual(Utilities.FixFilePath path, mapping.ItemSpec, sprintf "expected paths to be the same while checking position %d" position)
        for (key, value) in expectedProps do 
            Assert.AreEqual(value, mapping.GetMetadata(key), sprintf "expected values for metadata key %s to be the same while checking position %d" key position)

    [<Test>]
    member this.``basic deterministic scenarios`` () = 
        let items = 
            [|
            SourceRoot(@"c:\packages\SourcePackage1\", [], ["MappedPath", @"/_1/"])
            SourceRoot(@"/packages/SourcePackage2/", [], ["MappedPath", @"/_2/"])
            SourceRoot(@"c:\MyProjects\MyProject\", ["SourceControl", "Git"], [
                "SourceControl", "Git"
                "MappedPath", @"/_/"
            ])
            SourceRoot(@"c:\MyProjects\MyProject\a\b\", [
                "SourceControl", "Git"
                "NestedRoot", "a/b"
                "ContainingRoot", @"c:\MyProjects\MyProject\"
                "some metadata", "some value"
            ], [
                "SourceControl", "Git"
                "some metadata", "some value"
                "MappedPath", @"/_/a/b/"
            ])
            |]
        let task = MapSourceRoots()
        let outputs = MapSourceRoots.PerformMapping task.Log (items |> Array.map toTaskItem) true
        
        match outputs with
        | None -> 
            Assert.Fail("Expected to get some mappings back from ths scenario")
        | Some mappings ->
            Array.zip items mappings
            |> Array.iteri checkExpectations
