// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Build.UnitTests

open System
open System.IO
open System.Reflection
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks
open Microsoft.Build.Framework
open Microsoft.Build.Utilities
open FSharp.Build
open Xunit
open BuildTaskTestHelpers

type private ResourceTaskKind =
    | Resx
    | Text

type private ResourceTask =
    | ResxTask of FSharpEmbedResXSource
    | TextTask of FSharpEmbedResourceText

type private PathExpectation =
    | Restored
    | PreservedAbsolute
    | PartiallyQualified

[<Collection(nameof FSharp.Test.NotThreadSafeResourceCollection)>]
type FileTaskEnvironmentTests() =

    let assertContains scenario (needle: string) (text: string) =
        Assert.True(
            text.IndexOf(needle, StringComparison.Ordinal) >= 0,
            $"{scenario}: expected '{text}' to contain '{needle}'"
        )

    let assertNotContains scenario (needle: string) (text: string) =
        Assert.True(
            text.IndexOf(needle, StringComparison.Ordinal) < 0,
            $"{scenario}: expected '{text}' not to contain '{needle}'"
        )

    let assertFileExists scenario path =
        Assert.True(File.Exists path, $"{scenario}: expected generated file at '{path}'")

    let withDecoyCurrentDirectory body =
        let decoy = TestFramework.createTemporaryDirectory ()
        let original = Environment.CurrentDirectory

        try
            Environment.CurrentDirectory <- decoy.FullName
            body decoy
        finally
            Environment.CurrentDirectory <- original

    let runConcurrently scenario (executeA: unit -> bool) (executeB: unit -> bool) =
        use barrier = new Barrier(2)

        let run (execute: unit -> bool) : System.Threading.Tasks.Task<bool> =
            System.Threading.Tasks.Task.Run<bool>(fun () ->
                barrier.SignalAndWait()
                execute ())

        let taskA, taskB = run executeA, run executeB
        let tasks: System.Threading.Tasks.Task[] =
            [| taskA :> System.Threading.Tasks.Task; taskB :> System.Threading.Tasks.Task |]

        Assert.True(
            System.Threading.Tasks.Task.WaitAll(tasks, TimeSpan.FromSeconds 30.0),
            "Concurrent task executions timed out; possible deadlock."
        )

        Assert.True(taskA.Result, $"{scenario}: task A failed")
        Assert.True(taskB.Result, $"{scenario}: task B failed")

    let withIsolatedTaskEnvironmentPair body =
        withDecoyCurrentDirectory (fun decoy ->
            withTaskEnvironmentPairUsing
                createTaskEnvironmentInTemporaryDirectory
                (fun environmentA directoryA environmentB directoryB ->
                    body environmentA directoryA environmentB directoryB
                    Assert.Empty(Directory.GetFiles(decoy.FullName, "*", SearchOption.AllDirectories))))

    let assign environment (task: #IMultiThreadableTask) =
        (task :> IMultiThreadableTask).TaskEnvironment <- environment
        task

    let createResourceTask kind environment engine (input: string) (intermediate: string) =
        match kind with
        | Resx ->
            let item = TaskItem(input) :> ITaskItem
            item.SetMetadata("GenerateSource", "true")

            FSharpEmbedResXSource(
                BuildEngine = engine,
                EmbeddedResource = [| item |],
                IntermediateOutputPath = intermediate
            )
            |> assign environment
            |> ResxTask
        | Text ->
            FSharpEmbedResourceText(
                BuildEngine = engine,
                EmbeddedText = [| TaskItem(input) :> ITaskItem |],
                IntermediateOutputPath = intermediate
            )
            |> assign environment
            |> TextTask

    let executeResourceTask =
        function
        | ResxTask task -> task.Execute()
        | TextTask task -> task.Execute()

    let resourceKindInfo =
        function
        | Resx -> "FSharpEmbedResXSource", ".resx"
        | Text -> "FSharpEmbedResourceText", ".txt"

    let countOccurrences (needle: string) (text: string) =
        let rec count total start =
            let index = text.IndexOf(needle, start, StringComparison.Ordinal)
            if index < 0 then total else count (total + 1) (index + needle.Length)

        count 0 0

    let pathScenarios extension (directory: DirectoryInfo) =
        [
            "relative", $"Missing{extension}", Restored
            "absolute beneath project", Path.Combine(directory.FullName, $"AbsentUnderProject{extension}"), PreservedAbsolute
            "dot segment", Path.Combine("sub", "..", $"Missing{extension}"), Restored
            "invalid path", $"in|valid{extension}", Restored

            if RuntimeInformation.IsOSPlatform OSPlatform.Windows then
                "Windows root relative", $@"\Missing{extension}", PartiallyQualified
                "Windows drive relative", $@"C:Missing{extension}", PartiallyQualified
        ]

    [<Fact>]
    member _.``WriteCodeFragment isolates relative output paths per task``() =
        withIsolatedTaskEnvironmentPair (fun environmentA directoryA environmentB directoryB ->
            let makeTask (name: string) environment =
                WriteCodeFragment(
                    BuildEngine = MockEngine(),
                    Language = "F#",
                    AssemblyAttributes = [| TaskItem(name) :> ITaskItem |],
                    OutputFile = (TaskItem("Generated.fs") :> ITaskItem)
                )
                |> assign environment

            let taskA = makeTask "AssemblyMetadataA" environmentA
            let taskB = makeTask "AssemblyMetadataB" environmentB
            let scenario = "WriteCodeFragment isolates relative output paths per task"
            runConcurrently scenario taskA.Execute taskB.Execute

            let check (directory: DirectoryInfo) own other (task: WriteCodeFragment) =
                let path = Path.Combine(directory.FullName, "Generated.fs")
                assertFileExists own path
                let contents = File.ReadAllText path
                assertContains own own contents
                assertNotContains own other contents
                Assert.Equal("Generated.fs", task.OutputFile.ItemSpec)

            check directoryA "AssemblyMetadataA" "AssemblyMetadataB" taskA
            check directoryB "AssemblyMetadataB" "AssemblyMetadataA" taskB)

    [<Fact>]
    member _.``WriteCodeFragment preserves its OutputDirectory quirk``() =
        withTaskEnvironment (fun environment directory ->
            let task =
                WriteCodeFragment(
                    BuildEngine = MockEngine(),
                    Language = "F#",
                    AssemblyAttributes = [| TaskItem("SomeAttribute") :> ITaskItem |],
                    OutputDirectory = (TaskItem("SubDir") :> ITaskItem),
                    OutputFile = (TaskItem("Generated2.fs") :> ITaskItem)
                )
                |> assign environment

            Assert.True(task.Execute())
            assertFileExists "OutputDirectory quirk" (Path.Combine(directory.FullName, "Generated2.fs"))
            Assert.False(File.Exists(Path.Combine(directory.FullName, "SubDir", "Generated2.fs")))
            Assert.Equal(Path.Combine("SubDir", "Generated2.fs"), task.OutputFile.ItemSpec))

    [<Fact>]
    member _.``GenerateILLinkSubstitutions isolates relative output paths per task``() =
        withIsolatedTaskEnvironmentPair (fun environmentA directoryA environmentB directoryB ->
            let makeTask (assemblyName: string) (intermediate: string) environment =
                GenerateILLinkSubstitutions(
                    BuildEngine = MockEngine(),
                    AssemblyName = assemblyName,
                    IntermediateOutputPath = intermediate
                )
                |> assign environment

            let intermediateA = Path.Combine("obj", "DebugA")
            let intermediateB = Path.Combine("obj", "DebugB")
            let taskA = makeTask "AssemblyA" intermediateA environmentA
            let taskB = makeTask "AssemblyB" intermediateB environmentB
            let scenario = "GenerateILLinkSubstitutions isolates relative output paths per task"
            runConcurrently scenario taskA.Execute taskB.Execute

            let check
                (directory: DirectoryInfo)
                (intermediate: string)
                own
                other
                (task: GenerateILLinkSubstitutions)
                =
                let item = Assert.Single(task.GeneratedItems)
                let expectedItemSpec = Path.Combine(intermediate, "ILLink.Substitutions.xml")
                Assert.Equal(expectedItemSpec, item.ItemSpec)
                Assert.Equal("ILLink.Substitutions.xml", item.GetMetadata("LogicalName"))
                let path = Path.Combine(directory.FullName, expectedItemSpec)
                assertFileExists own path
                let contents = File.ReadAllText path
                assertContains own own contents
                assertNotContains own other contents

            check directoryA intermediateA "AssemblyA" "AssemblyB" taskA
            check directoryB intermediateB "AssemblyB" "AssemblyA" taskB)

    [<Fact>]
    member _.``Resource generators isolate relative input and output paths per task``() =
        for kind in [ Resx; Text ] do
            withIsolatedTaskEnvironmentPair (fun environmentA directoryA environmentB directoryB ->
                let scenario, extension = resourceKindInfo kind
                let intermediate = Path.Combine("obj", "Debug")
                let input = "Resource" + extension

                for directory in [ directoryA; directoryB ] do
                    Directory.CreateDirectory(Path.Combine(directory.FullName, intermediate))
                    |> ignore

                let content marker =
                    match kind with
                    | Resx ->
                        $"""<?xml version="1.0" encoding="utf-8"?><root><data name="Greeting"><value>{marker}</value></data></root>"""
                    | Text -> $"""greeting,"{marker}"
"""

                File.WriteAllText(Path.Combine(directoryA.FullName, input), content "Hello from A")
                File.WriteAllText(Path.Combine(directoryB.FullName, input), content "Hello from B")

                let taskA = createResourceTask kind environmentA (MockEngine()) input intermediate
                let taskB = createResourceTask kind environmentB (MockEngine()) input intermediate
                runConcurrently scenario (fun () -> executeResourceTask taskA) (fun () -> executeResourceTask taskB)

                let generatedSpecs task =
                    match task with
                    | ResxTask task -> task.GeneratedSource
                    | TextTask task -> Array.append task.GeneratedSource task.GeneratedResx
                    |> Array.map _.ItemSpec

                let source = Path.Combine(intermediate, "Resource.fs")
                let expectedSpecs, contentSpecs =
                    match kind with
                    | Resx -> [ source ], [ source ]
                    | Text ->
                        let signature = Path.Combine(intermediate, "Resource.fsi")
                        let resx = Path.Combine(intermediate, "Resource.resx")
                        [ signature; source; resx ], [ source; resx ]

                for task in [ taskA; taskB ] do
                    Assert.Equal<string[]>(List.toArray expectedSpecs, generatedSpecs task)

                for directory, own, other in
                    [ directoryA, "Hello from A", "Hello from B"; directoryB, "Hello from B", "Hello from A" ] do
                    for spec in expectedSpecs do
                        assertFileExists scenario (Path.Combine(directory.FullName, spec))

                    for spec in contentSpecs do
                        let contents = File.ReadAllText(Path.Combine(directory.FullName, spec))
                        assertContains scenario own contents
                        assertNotContains scenario other contents)

    [<Fact>]
    member _.``FSharpEmbedResXSource reports malformed XML only through one MSBuild error``() =
        withTaskEnvironment (fun environment directory ->
            let input = "Malformed.resx"

            File.WriteAllText(
                Path.Combine(directory.FullName, input),
                "<?xml version=\"1.0\" encoding=\"utf-8\"?><root><data name=\"Broken\"><value>Oops</root>"
            )

            let engine = MockEngine()
            let task = createResourceTask Resx environment engine input "obj"
            let originalOut, originalError = Console.Out, Console.Error
            use capturedOut = new StringWriter()
            use capturedError = new StringWriter()
            Console.SetOut capturedOut
            Console.SetError capturedError

            let result =
                try
                    executeResourceTask task
                finally
                    Console.SetOut originalOut
                    Console.SetError originalError

            Assert.False result
            Assert.Equal("", capturedOut.ToString())
            Assert.Equal("", capturedError.ToString())
            let error = Assert.Single(engine.Errors)
            assertContains "malformed resx" input error.Message
            assertNotContains "malformed resx" directory.FullName error.Message)

    [<Fact>]
    member _.``FSharpEmbedResXSource logs failTask errors exactly once``() =
        withTaskEnvironment (fun environment directory ->
            let input = "MissingName.resx"

            File.WriteAllText(
                Path.Combine(directory.FullName, input),
                "<?xml version=\"1.0\" encoding=\"utf-8\"?><root><data><value>Oops</value></data></root>"
            )

            let engine = MockEngine()
            let task = createResourceTask Resx environment engine input "obj"
            Assert.False(executeResourceTask task)
            let message = (Assert.Single(engine.Errors)).Message
            assertContains "missing resource name" "Missing resource name" message

            for leaked in [ "An exception occurred when processing"; "TaskFailed"; "   at " ] do
                assertNotContains "missing resource name" leaked message)

    [<Fact>]
    member _.``Resource task diagnostics preserve every input path shape``() =
        for kind in [ Resx; Text ] do
            withTaskEnvironment (fun environment directory ->
                let kindName, extension = resourceKindInfo kind

                for name, input, expectation in pathScenarios extension directory do
                    let scenario = $"{kindName}: {name}"
                    let engine = MockEngine()
                    let task = createResourceTask kind environment engine input "obj"

                    Assert.False(executeResourceTask task, scenario)
                    Assert.NotEmpty engine.Errors

                    if kind <> Text || name <> "invalid path" then
                        Assert.True(engine.Errors.Count = 1, $"{scenario}: expected one error, got {engine.Errors.Count}")

                    let message = engine.Errors |> Seq.map _.Message |> String.concat Environment.NewLine
                    assertContains scenario input message

                    match expectation with
                    | PreservedAbsolute ->
                        Assert.True(
                            countOccurrences input message >= 2,
                            $"{scenario}: expected the absolute path in both diagnostic prefix and exception body"
                        )
                    | Restored -> assertNotContains scenario directory.FullName message
                    | PartiallyQualified ->
                        assertNotContains scenario (environment.GetAbsolutePath(input).Value) message
                        assertNotContains scenario directory.FullName message)

    [<Fact>]
    member _.``FSharpEmbedResourceText restores overlapping paths longest first``() =
        withTaskEnvironment (fun environment directory ->
            let shortOriginal = "p"
            let longOriginal = Path.Combine("p", "deeper", "..", "deeper")
            let canonicalLong = Path.GetFullPath(environment.GetAbsolutePath(longOriginal).Value)
            let restore =
                typeof<FSharpEmbedResourceText>
                    .Assembly.GetType("FSharp.Build.TaskEnvironmentPaths")
                    .GetMethod("restoreOriginalPaths", BindingFlags.NonPublic ||| BindingFlags.Static)

            let message = $"Could not find a part of the path '{canonicalLong}'."
            let actual =
                restore.Invoke(null, [| box environment; box message; box [ shortOriginal; longOriginal ] |]) :?> string

            Assert.Equal($"Could not find a part of the path '{longOriginal}'.", actual)
            assertNotContains "overlapping path restoration" directory.FullName actual)

    [<Fact>]
    member _.``SubstituteText isolates relative input and output paths per task``() =
        withIsolatedTaskEnvironmentPair (fun environmentA directoryA environmentB directoryB ->
            let input = "Source.txt"
            let intermediate = Path.Combine("obj", "Debug")
            File.WriteAllText(Path.Combine(directoryA.FullName, input), "Hello from A. Token: PLACEHOLDER")
            File.WriteAllText(Path.Combine(directoryB.FullName, input), "Hello from B. Token: PLACEHOLDER")

            let makeTask environment =
                let item = TaskItem(input) :> ITaskItem
                item.SetMetadata("IntermediateTargetPath", intermediate)
                item.SetMetadata("Pattern1", "PLACEHOLDER")
                item.SetMetadata("Replacement1", "REPLACED")
                SubstituteText(BuildEngine = MockEngine(), EmbeddedResources = [| item |])
                |> assign environment

            let taskA, taskB = makeTask environmentA, makeTask environmentB
            let scenario = "SubstituteText isolates relative input and output paths per task"
            runConcurrently scenario taskA.Execute taskB.Execute

            let expectedItemSpec = Path.Combine(intermediate, input)

            for directory, expected, task in
                [
                    directoryA, "Hello from A. Token: REPLACED", taskA
                    directoryB, "Hello from B. Token: REPLACED", taskB
                ] do
                Assert.Equal(expectedItemSpec, Assert.Single(task.CopiedFiles).ItemSpec)
                let path = Path.Combine(directory.FullName, expectedItemSpec)
                assertFileExists expected path
                Assert.Equal(expected, File.ReadAllText path))

    [<Fact>]
    member _.``SubstituteText preserves its missing-source success behavior``() =
        withTaskEnvironment (fun environment directory ->
            let item = TaskItem("Missing.txt") :> ITaskItem
            item.SetMetadata("IntermediateTargetPath", "obj")
            item.SetMetadata("Pattern1", "PLACEHOLDER")
            item.SetMetadata("Replacement1", "REPLACED")

            let task =
                SubstituteText(BuildEngine = MockEngine(), EmbeddedResources = [| item |])
                |> assign environment

            Assert.True(task.Execute())
            let expectedItemSpec = Path.Combine("obj", "Missing.txt")
            Assert.Equal(expectedItemSpec, Assert.Single(task.CopiedFiles).ItemSpec)
            Assert.False(File.Exists(Path.Combine(directory.FullName, expectedItemSpec))))
