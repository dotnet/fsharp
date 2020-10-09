
namespace FSharp.Build.UnitTests

open Microsoft.Build.Framework
open Microsoft.Build.Utilities
open FSharp.Build
open NUnit.Framework
open System.Collections.Generic

type MockEngine() =
    member val Errors = ResizeArray() with get
    member val Warnings = ResizeArray() with get
    member val Custom = ResizeArray() with get
    member val Messages = ResizeArray() with get

    interface IBuildEngine with
        member this.BuildProjectFile(projectFileName: string, targetNames: string [], globalProperties: System.Collections.IDictionary, targetOutputs: System.Collections.IDictionary): bool =
            failwith "Not Implemented"
        member this.ColumnNumberOfTaskNode: int = 0
        member this.ContinueOnError: bool = true
        member this.LineNumberOfTaskNode: int = 0
        member this.LogCustomEvent(e: CustomBuildEventArgs): unit =
            this.Custom.Add e
            failwith "Not Implemented"
        member this.LogErrorEvent(e: BuildErrorEventArgs): unit =
            this.Errors.Add e
        member this.LogMessageEvent(e: BuildMessageEventArgs): unit =
            this.Messages.Add e
        member this.LogWarningEvent(e: BuildWarningEventArgs): unit =
            this.Warnings.Add e
        member this.ProjectFileOfTaskNode: string = ""

type SourceRoot =
    SourceRoot of
        path: string *
        props: list<string * string> *
        expectedProps: list<string * string>


/// these tests are ported from https://github.com/dotnet/roslyn/blob/093ea477717001c58be6231cf2a793f4245cbf72/src/Compilers/Core/MSBuildTaskTests/MapSourceRootTests.cs
/// Same scenarios, slightly different setup/teardown
[<TestFixture>]
type MapSourceRootsTests() =

    let assertNoErrors (t: MapSourceRoots) =
        let engine = t.BuildEngine :?> MockEngine
        let errors = engine.Errors
        Assert.AreEqual(0, errors.Count, sprintf "Expected no errors, but found the following: %A" errors)
    let newTask () =
        MapSourceRoots(BuildEngine = MockEngine())
    let toTaskItem (SourceRoot(path, props, _)) =
        let dict = Dictionary()
        for (k, v) in props do dict.Add(k, v)
        TaskItem(path, dict) :> ITaskItem
    let checkExpectations position (SourceRoot(path, _, expectedProps), mapping: ITaskItem) =
        Assert.AreEqual(Utilities.FixFilePath path, mapping.ItemSpec, sprintf "expected paths to be the same while checking position %d" position)
        for (key, value) in expectedProps do
            Assert.AreEqual(value, mapping.GetMetadata(key), sprintf "expected values for metadata key %s to be the same while checking position %d" key position)

    let successfulTest items =
        let task = newTask()
        let outputs = MapSourceRoots.PerformMapping task.Log (items |> Array.map toTaskItem) true

        assertNoErrors task

        match outputs with
        | None ->
            Assert.Fail("Expected to get some mappings back from this scenario")
        | Some mappings ->
            Array.zip items mappings
            |> Array.iteri checkExpectations

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

        successfulTest items


    [<Test>]
    member this.``invalid chars`` () =
        let items =
            [|
                SourceRoot(@"!@#:;$%^&*()_+|{}\", [], ["MappedPath", @"/_1/"])
                SourceRoot(@"****/", ["SourceControl", "Git"], [
                    "MappedPath", @"/_/"
                    "SourceControl", "Git"
                ])
                SourceRoot(@"****\|||:;\", [
                    "SourceControl", "Git"
                    "NestedRoot","|||:;"
                    "ContainingRoot", @"****/"
                ], [
                    "MappedPath", @"/_/|||:;/"
                    "SourceControl", "Git"
                ])
            |]
        successfulTest items

    [<Test>]
    member this.``input paths must end with separator`` () =
        let items =
            [|
                SourceRoot(@"C:\", [], [])
                SourceRoot(@"C:/", [], [])
                SourceRoot(@"C:", [], [])
                SourceRoot(@"C", [], [])
            |]
        let task = newTask()
        let outputs = MapSourceRoots.PerformMapping task.Log (items |> Array.map toTaskItem) true

        match outputs with
        | None ->
            let errors = (task.BuildEngine :?> MockEngine).Errors
            Assert.AreEqual(2, errors.Count, "Should have had some errors with path mappings")
            let expectedErrors = ["'C:'"; "'C'"]
            let errorMessages = errors |> Seq.map (fun e -> e.Message)

            Assert.IsTrue(errorMessages |> Seq.forall (fun error -> error.Contains("end with a slash or backslash")))

            expectedErrors
            |> Seq.iter (fun expectedErrorPath ->
                Assert.IsTrue(errorMessages |> Seq.exists (fun err -> err.EndsWith expectedErrorPath),
                              sprintf "expected an error to end with '%s', none did.\nMessages were:\n%A" expectedErrorPath errorMessages)
            )
        | Some mappings ->
            Assert.Fail("Expected to fail on the inputs")

    [<Test>]
    member this.``nested roots separators`` () =
        let items =
            [|
                SourceRoot(@"c:\MyProjects\MyProject\", [], [
                    "MappedPath", @"/_/"
                ])
                SourceRoot(@"c:\MyProjects\MyProject\a\a\", [
                    "NestedRoot", @"a/a/"
                    "ContainingRoot", @"c:\MyProjects\MyProject\"
                ], [
                    "MappedPath", @"/_/a/a/"
                ])
                SourceRoot(@"c:\MyProjects\MyProject\a\b\", [
                    "NestedRoot", @"a/b\"
                    "ContainingRoot", @"c:\MyProjects\MyProject\"
                ],[
                    "MappedPath", @"/_/a/b/"
                ])
                SourceRoot(@"c:\MyProjects\MyProject\a\c\", [
                    "NestedRoot", @"a\c"
                    "ContainingRoot", @"c:\MyProjects\MyProject\"
                ], [
                    "MappedPath", @"/_/a/c/"
                ])
            |]

        successfulTest items

    [<Test>]
    member this.``sourceroot case sensitivity``() =
        let items = [|
            SourceRoot(@"c:\packages\SourcePackage1\", [], ["MappedPath", @"/_/"])
            SourceRoot(@"C:\packages\SourcePackage1\", [], ["MappedPath", @"/_1/"])
            SourceRoot(@"c:\packages\SourcePackage2\", [], ["MappedPath", @"/_2/"])
        |]

        successfulTest items

    [<Test>]
    member this.``recursion error`` () =
        let path1 = Utilities.FixFilePath @"c:\MyProjects\MyProject\a\1\"
        let path2 = Utilities.FixFilePath @"c:\MyProjects\MyProject\a\2\"
        let path3 = Utilities.FixFilePath @"c:\MyProjects\MyProject\"
        let items =
            [|
                SourceRoot(path1, [
                    "ContainingRoot", path2
                    "NestedRoot", "a/1"
                ], [])
                SourceRoot(path2, [
                    "ContainingRoot", path1
                    "NestedRoot", "a/2"
                ], [])
                SourceRoot(path3, [], [])
            |]

        let task = newTask()
        let outputs = MapSourceRoots.PerformMapping task.Log (items |> Array.map toTaskItem) true

        match outputs with
        | None ->
            let errors = (task.BuildEngine :?> MockEngine).Errors
            Assert.AreEqual(2, errors.Count, "Should have had some errors with path mappings")
            let expectedErrors = [path2; path1] |> List.map (sprintf "'%s'")
            let errorMessages = errors |> Seq.map (fun e -> e.Message)

            Assert.IsTrue(errorMessages |> Seq.forall (fun error -> error.Contains("ContainingRoot was not found in SourceRoot items")),
                sprintf "Expected to have the same type of errors but had %A" errorMessages
            )

            expectedErrors
            |> Seq.iter (fun expectedErrorPath ->
                Assert.IsTrue(errorMessages |> Seq.exists (fun err -> err.EndsWith expectedErrorPath), sprintf "expected an error to end with '%s', none did.\nMessages were:\n%A" expectedErrorPath errorMessages)
            )
        | Some mappings ->
            Assert.Fail("Expected to fail on the inputs")

    [<TestCase(true)>]
    [<TestCase(false)>]
    [<Test>]
    member this.``metadata merge 1`` (deterministic: bool) =
        let path1 = Utilities.FixFilePath @"c:\packages\SourcePackage1\"
        let path2 = Utilities.FixFilePath @"c:\packages\SourcePackage2\"
        let path3 = Utilities.FixFilePath @"c:\packages\SourcePackage3\"

        let items = [|
            SourceRoot(path1, [
                "NestedRoot", @"NR1A"
                "ContainingRoot", path3
                "RevisionId", "RevId1"
                "SourceControl", "git"
                "MappedPath", "MP1"
                "SourceLinkUrl", "URL1"
            ], [])
            SourceRoot(path1, [
                "NestedRoot", @"NR1B"
                "ContainingRoot", @"CR"
                "RevisionId", "RevId2"
                "SourceControl", "tfvc"
                "MappedPath", "MP2"
                "SourceLinkUrl", "URL2"
            ], [])
            SourceRoot(path2, [
                "NestedRoot", @"NR2"
                "SourceControl", "git"
            ], [])
            SourceRoot(path2, [
                "ContainingRoot", path3
                "SourceControl", "git"
            ], [])
            SourceRoot(path3, [], [])
        |]

        /// because this test isn't one to one we have to put the expecations in another structure
        let actualExpectations = [|
            SourceRoot(path1, [], [
                "SourceControl", "git"
                "RevisionId", "RevId1"
                "NestedRoot", "NR1A"
                "ContainingRoot", path3
                "MappedPath", if deterministic then "/_/NR1A/" else path1
                "SourceLinkUrl", "URL1"
            ])
            SourceRoot(path2, [], [
                "SourceControl", "git"
                "RevisionId", ""
                "NestedRoot", "NR2"
                "ContainingRoot", path3
                "MappedPath", if deterministic then "/_/NR2/" else path2
                "SourceLinkUrl", ""
            ])
            SourceRoot(path3, [], [
                "SourceControl", ""
                "RevisionId", ""
                "NestedRoot", ""
                "ContainingRoot", ""
                "MappedPath", if deterministic then "/_/" else path3
                "SourceLinkUrl", ""
            ])
        |]

        let task = newTask()
        let outputs = MapSourceRoots.PerformMapping task.Log (items |> Array.map toTaskItem) deterministic

        assertNoErrors task

        match outputs with
        | None ->
            Assert.Fail("Expected to get some mappings back from this scenario")
        | Some mappings ->
            let warnings = (task.BuildEngine :?> MockEngine).Warnings |> Seq.map (fun w -> w.Message)

            Assert.AreEqual(6, Seq.length warnings)
            Assert.IsTrue(warnings |> Seq.forall (fun w -> w.Contains "duplicate items"))

            [
                "SourceControl", "git", "tfvc"
                "RevisionId", "RevId1", "RevId2"
                "NestedRoot", "NR1A", "NR1B"
                "ContainingRoot", path3, "CR"
                "MappedPath", "MP1", "MP2"
                "SourceLinkUrl", "URL1", "URL2"
            ]
            |> List.iter (fun (key, lval, rval) ->
                Assert.IsTrue(
                    (warnings |> Seq.exists (fun warn -> warn.Contains(sprintf "SourceRoot contains duplicate items '%s' with conflicting metadata '%s': '%s' and '%s'" path1 key lval rval))),
                    sprintf "Expected to find an error message for %s comparing %s and %s, but got %A" key lval rval warnings
                )
            )

            Array.zip actualExpectations mappings
            |> Array.iteri checkExpectations

    [<Test>]
    member this.``missing containing root`` () =
        let items = [|
            SourceRoot(@"c:\MyProjects\MYPROJECT\", [], [])
            SourceRoot(@"c:\MyProjects\MyProject\a\b\", [
                "SourceControl", "Git"
                "NestedRoot", "a/b"
                "ContainingRoot", @"c:\MyProjects\MyProject\"
            ], []
            )
        |]

        let task = newTask()
        let outputs = MapSourceRoots.PerformMapping task.Log (items |> Array.map toTaskItem) true

        match outputs with
        | None ->
            let errors = (task.BuildEngine :?> MockEngine).Errors
            Assert.AreEqual(1, errors.Count, "Should have had some errors with path mappings")
            let expectedErrors = [@"c:\MyProjects\MyProject\"] |> List.map (sprintf "'%s'")
            let errorMessages = errors |> Seq.map (fun e -> e.Message)

            Assert.IsTrue(errorMessages |> Seq.forall (fun error -> error.Contains("corresponding item is not a top-level source root")),
                sprintf "Expected to have the same type of errors but had %A" errorMessages
            )

            expectedErrors
            |> Seq.iter (fun expectedErrorPath ->
                Assert.IsTrue(errorMessages |> Seq.exists (fun err -> err.EndsWith expectedErrorPath), sprintf "expected an error to end with '%s', none did.\nMessages were:\n%A" expectedErrorPath errorMessages)
            )
        | Some mappings ->
            Assert.Fail("Expected to fail on the inputs")

    [<Test>]
    member this.``no containing root`` () =
        let items = [|
            SourceRoot(@"c:\MyProjects\MyProject\", [], [])
            SourceRoot(@"c:\MyProjects\MyProject\a\b\", [
                "SourceControl", "Git"
                "NestedRoot", "a/b"
            ], [])
        |]

        let task = newTask()
        let outputs = MapSourceRoots.PerformMapping task.Log (items |> Array.map toTaskItem) true

        match outputs with
        | None ->
            let errors = (task.BuildEngine :?> MockEngine).Errors
            Assert.AreEqual(1, errors.Count, "Should have had some errors with path mappings")
            let expectedErrors = [@""] |> List.map (sprintf "'%s'")
            let errorMessages = errors |> Seq.map (fun e -> e.Message)

            Assert.IsTrue(errorMessages |> Seq.forall (fun error -> error.Contains("corresponding item is not a top-level source root")),
                sprintf "Expected to have the same type of errors but had %A" errorMessages
            )

            expectedErrors
            |> Seq.iter (fun expectedErrorPath ->
                Assert.IsTrue(errorMessages |> Seq.exists (fun err -> err.EndsWith expectedErrorPath), sprintf "expected an error to end with '%s', none did.\nMessages were:\n%A" expectedErrorPath errorMessages)
            )
        | Some mappings ->
            Assert.Fail("Expected to fail on the inputs")

    [<TestCase(true)>]
    [<TestCase(false)>]
    [<Test>]
    member this.``no top level source root`` (deterministic: bool) =
        let path1 = Utilities.FixFilePath @"c:\MyProjects\MyProject\a\b\"
        let items = [|
            SourceRoot(path1, [
                "ContainingRoot", path1
                "NestedRoot", "a/b"
            ], [
                "SourceControl", ""
                "RevisionId", ""
                "NestedRoot", "a/b"
                "ContainingRoot", path1
                "MappedPath", path1
                "SourceLinkUrl", ""
            ])
        |]

        let task = newTask()
        let outputs = MapSourceRoots.PerformMapping task.Log (items |> Array.map toTaskItem) deterministic

        match outputs, deterministic with
        | Some _, true ->
            Assert.Fail "Expected to fail when deterministic"
        | None, true ->
            let errors = (task.BuildEngine :?> MockEngine).Errors
            Assert.AreEqual(1, errors.Count, "Should have had some errors with path mappings")
            let error = errors.[0].Message
            Assert.IsTrue(error.Contains "when DeterministicSourcePaths is true")
        | None, false ->
            Assert.Fail (sprintf "Expected to succeed when not deterministic")
        | Some mappings, false ->
            Array.zip items mappings
            |> Array.iteri checkExpectations
