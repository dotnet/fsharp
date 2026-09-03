// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Build.UnitTests

open System
open System.IO
open System.Threading
open System.Threading.Tasks
open Microsoft.Build.Framework
open Microsoft.Build.Utilities
open FSharp.Build
open Xunit
open BuildTaskTestHelpers

/// Verifies that FSharp.Build tasks performing file I/O (WriteCodeFragment, GenerateILLinkSubstitutions)
/// resolve relative paths against the TaskEnvironment assigned to each task instance, rather than the
/// single, process-wide Environment.CurrentDirectory. Each test runs two task instances concurrently,
/// each wired to its own project directory/environment/build engine, while the process current directory
/// points at a third, unrelated "decoy" directory that must remain untouched.
[<Collection(nameof FSharp.Test.NotThreadSafeResourceCollection)>]
type FileTaskEnvironmentTests() =

    /// Points Environment.CurrentDirectory at a fresh decoy directory for the duration of `body`, then
    /// restores the original current directory. Used to prove tasks don't fall back to ambient process state.
    let withDecoyCurrentDirectory (body: DirectoryInfo -> unit) =
        let decoyDirectory = TestFramework.createTemporaryDirectory ()
        let originalCurrentDirectory = Environment.CurrentDirectory

        try
            Environment.CurrentDirectory <- decoyDirectory.FullName
            body decoyDirectory
        finally
            Environment.CurrentDirectory <- originalCurrentDirectory

    /// Starts both executions on the thread pool and releases them together via a Barrier, so neither
    /// execution can complete before both have started, then waits for both to finish. Bounded by a
    /// timeout so that a deadlock in the code under test fails the test instead of hanging forever.
    let runConcurrently (executeA: unit -> bool) (executeB: unit -> bool) =
        let barrier = new Barrier(2)

        let runOne (execute: unit -> bool) =
            Task.Run(fun () ->
                barrier.SignalAndWait()
                execute ())

        let taskA = runOne executeA
        let taskB = runOne executeB

        let tasks: System.Threading.Tasks.Task[] =
            [| taskA :> System.Threading.Tasks.Task; taskB :> System.Threading.Tasks.Task |]

        let completed = Task.WaitAll(tasks, TimeSpan.FromSeconds 30.0)
        Assert.True(completed, "Concurrent task executions did not complete within the timeout; possible deadlock.")

        taskA.Result, taskB.Result

    [<Fact>]
    member _.``WriteCodeFragment writes relative to each task's TaskEnvironment, not the process current directory``
        ()
        =
        withDecoyCurrentDirectory (fun decoyDirectory ->
            let environmentA, directoryA = createTaskEnvironmentInTemporaryDirectory ()
            let environmentB, directoryB = createTaskEnvironmentInTemporaryDirectory ()

            try
                let makeTask (attributeName: string) (environment: TaskEnvironment) =
                    let attribute = TaskItem(attributeName) :> ITaskItem

                    let task =
                        WriteCodeFragment(
                            BuildEngine = MockEngine(),
                            Language = "F#",
                            AssemblyAttributes = [| attribute |],
                            OutputFile = (TaskItem("Generated.fs") :> ITaskItem)
                        )

                    assignTaskEnvironment (task :> IMultiThreadableTask) environment
                    task

                let taskA = makeTask "AssemblyMetadataA" environmentA
                let taskB = makeTask "AssemblyMetadataB" environmentB

                let successA, successB =
                    runConcurrently (fun () -> taskA.Execute()) (fun () -> taskB.Execute())

                Assert.True successA
                Assert.True successB

                let pathA = Path.Combine(directoryA.FullName, "Generated.fs")
                let pathB = Path.Combine(directoryB.FullName, "Generated.fs")

                Assert.True(File.Exists pathA, sprintf "Expected generated file at %s" pathA)
                Assert.True(File.Exists pathB, sprintf "Expected generated file at %s" pathB)

                let contentsA = File.ReadAllText pathA
                let contentsB = File.ReadAllText pathB

                Assert.Contains("AssemblyMetadataA", contentsA)
                Assert.Contains("AssemblyMetadataB", contentsB)
                Assert.DoesNotContain("AssemblyMetadataB", contentsA)
                Assert.DoesNotContain("AssemblyMetadataA", contentsB)

                // The output item's ItemSpec must remain the original, unrooted relative value.
                Assert.Equal("Generated.fs", taskA.OutputFile.ItemSpec)
                Assert.Equal("Generated.fs", taskB.OutputFile.ItemSpec)

                Assert.Empty(Directory.GetFiles(decoyDirectory.FullName, "*", SearchOption.AllDirectories))
            finally
                disposeTaskEnvironment environmentA
                disposeTaskEnvironment environmentB)

    [<Fact>]
    member _.``WriteCodeFragment preserves its OutputDirectory quirk while rooting the write against TaskEnvironment``
        ()
        =
        let environment, directory = createTaskEnvironmentInTemporaryDirectory ()

        try
            let attribute = TaskItem("SomeAttribute") :> ITaskItem

            let task =
                WriteCodeFragment(
                    BuildEngine = MockEngine(),
                    Language = "F#",
                    AssemblyAttributes = [| attribute |],
                    OutputDirectory = (TaskItem("SubDir") :> ITaskItem),
                    OutputFile = (TaskItem("Generated2.fs") :> ITaskItem)
                )

            assignTaskEnvironment (task :> IMultiThreadableTask) environment

            Assert.True(task.Execute())

            // Existing quirk (preserved unchanged): the file is written using OutputFile.ItemSpec alone,
            // ignoring OutputDirectory, even though OutputDirectory is folded into the returned OutputFile item.
            let writtenPath = Path.Combine(directory.FullName, "Generated2.fs")
            Assert.True(File.Exists writtenPath, sprintf "Expected generated file at %s" writtenPath)
            Assert.False(File.Exists(Path.Combine(directory.FullName, "SubDir", "Generated2.fs")))

            Assert.Equal(Path.Combine("SubDir", "Generated2.fs"), task.OutputFile.ItemSpec)
        finally
            disposeTaskEnvironment environment

    [<Fact>]
    member _.``GenerateILLinkSubstitutions writes relative to each task's TaskEnvironment, not the process current directory``
        ()
        =
        withDecoyCurrentDirectory (fun decoyDirectory ->
            let environmentA, directoryA = createTaskEnvironmentInTemporaryDirectory ()
            let environmentB, directoryB = createTaskEnvironmentInTemporaryDirectory ()

            try
                let relativeIntermediateA = Path.Combine("obj", "DebugA")
                let relativeIntermediateB = Path.Combine("obj", "DebugB")

                let makeTask (assemblyName: string) (relativeIntermediate: string) (environment: TaskEnvironment) =
                    let task =
                        GenerateILLinkSubstitutions(
                            BuildEngine = MockEngine(),
                            AssemblyName = assemblyName,
                            IntermediateOutputPath = relativeIntermediate
                        )

                    assignTaskEnvironment (task :> IMultiThreadableTask) environment
                    task

                let taskA = makeTask "AssemblyA" relativeIntermediateA environmentA
                let taskB = makeTask "AssemblyB" relativeIntermediateB environmentB

                let successA, successB =
                    runConcurrently (fun () -> taskA.Execute()) (fun () -> taskB.Execute())

                Assert.True successA
                Assert.True successB

                let expectedItemSpecA = Path.Combine(relativeIntermediateA, "ILLink.Substitutions.xml")
                let expectedItemSpecB = Path.Combine(relativeIntermediateB, "ILLink.Substitutions.xml")

                // The generated item's ItemSpec must remain the original, unrooted relative value, and its
                // LogicalName metadata must be unchanged.
                Assert.Equal(expectedItemSpecA, taskA.GeneratedItems.[0].ItemSpec)
                Assert.Equal(expectedItemSpecB, taskB.GeneratedItems.[0].ItemSpec)
                Assert.Equal("ILLink.Substitutions.xml", taskA.GeneratedItems.[0].GetMetadata("LogicalName"))
                Assert.Equal("ILLink.Substitutions.xml", taskB.GeneratedItems.[0].GetMetadata("LogicalName"))

                let pathA = Path.Combine(directoryA.FullName, expectedItemSpecA)
                let pathB = Path.Combine(directoryB.FullName, expectedItemSpecB)

                Assert.True(File.Exists pathA, sprintf "Expected generated file at %s" pathA)
                Assert.True(File.Exists pathB, sprintf "Expected generated file at %s" pathB)

                let contentsA = File.ReadAllText pathA
                let contentsB = File.ReadAllText pathB

                Assert.Contains("AssemblyA", contentsA)
                Assert.Contains("AssemblyB", contentsB)
                Assert.DoesNotContain("AssemblyB", contentsA)
                Assert.DoesNotContain("AssemblyA", contentsB)

                Assert.Empty(Directory.GetFiles(decoyDirectory.FullName, "*", SearchOption.AllDirectories))
            finally
                disposeTaskEnvironment environmentA
                disposeTaskEnvironment environmentB)

    [<Fact>]
    member _.``FSharpEmbedResourceText generates .fs, .fsi and .resx relative to each task's TaskEnvironment, not the process current directory``
        ()
        =
        withDecoyCurrentDirectory (fun decoyDirectory ->
            let environmentA, directoryA = createTaskEnvironmentInTemporaryDirectory ()
            let environmentB, directoryB = createTaskEnvironmentInTemporaryDirectory ()

            try
                // Both instances share the exact same relative paths; only the TaskEnvironment each is
                // wired to (and therefore the project directory the paths are rooted against) differs.
                let relativeIntermediate = Path.Combine("obj", "Debug")
                let relativeText = "Strings.txt"

                Directory.CreateDirectory(Path.Combine(directoryA.FullName, relativeIntermediate))
                |> ignore

                Directory.CreateDirectory(Path.Combine(directoryB.FullName, relativeIntermediate))
                |> ignore

                File.WriteAllText(Path.Combine(directoryA.FullName, relativeText), "greeting,\"Hello from A\"\n")
                File.WriteAllText(Path.Combine(directoryB.FullName, relativeText), "greeting,\"Hello from B\"\n")

                let makeTask (environment: TaskEnvironment) =
                    let task =
                        FSharpEmbedResourceText(
                            BuildEngine = MockEngine(),
                            EmbeddedText = [| TaskItem(relativeText) :> ITaskItem |],
                            IntermediateOutputPath = relativeIntermediate
                        )

                    assignTaskEnvironment (task :> IMultiThreadableTask) environment
                    task

                let taskA = makeTask environmentA
                let taskB = makeTask environmentB

                let successA, successB =
                    runConcurrently (fun () -> taskA.Execute()) (fun () -> taskB.Execute())

                Assert.True successA
                Assert.True successB

                let expectedSignatureItemSpec = Path.Combine(relativeIntermediate, "Strings.fsi")
                let expectedSourceItemSpec = Path.Combine(relativeIntermediate, "Strings.fs")
                let expectedResxItemSpec = Path.Combine(relativeIntermediate, "Strings.resx")

                // The generated items' ItemSpecs must remain the original, unrooted relative values, and
                // are identical for both instances since both were configured with identical inputs.
                let itemSpecs (source: ITaskItem[]) = source |> Array.map (fun item -> item.ItemSpec)
                let expectedSourceItemSpecs = [| expectedSignatureItemSpec; expectedSourceItemSpec |]

                Assert.Equal<string[]>(expectedSourceItemSpecs, itemSpecs taskA.GeneratedSource)
                Assert.Equal<string[]>(expectedSourceItemSpecs, itemSpecs taskB.GeneratedSource)
                Assert.Equal(expectedResxItemSpec, taskA.GeneratedResx.[0].ItemSpec)
                Assert.Equal(expectedResxItemSpec, taskB.GeneratedResx.[0].ItemSpec)

                let sourcePathA = Path.Combine(directoryA.FullName, expectedSourceItemSpec)
                let sourcePathB = Path.Combine(directoryB.FullName, expectedSourceItemSpec)
                let resxPathA = Path.Combine(directoryA.FullName, expectedResxItemSpec)
                let resxPathB = Path.Combine(directoryB.FullName, expectedResxItemSpec)

                for path in
                    [
                        sourcePathA
                        sourcePathB
                        resxPathA
                        resxPathB
                        Path.Combine(directoryA.FullName, expectedSignatureItemSpec)
                        Path.Combine(directoryB.FullName, expectedSignatureItemSpec)
                    ] do
                    Assert.True(File.Exists path, sprintf "Expected generated file at %s" path)

                let sourceContentsA = File.ReadAllText sourcePathA
                let sourceContentsB = File.ReadAllText sourcePathB
                let resxContentsA = File.ReadAllText resxPathA
                let resxContentsB = File.ReadAllText resxPathB

                Assert.Contains("Hello from A", sourceContentsA)
                Assert.Contains("Hello from B", sourceContentsB)
                Assert.DoesNotContain("Hello from B", sourceContentsA)
                Assert.DoesNotContain("Hello from A", sourceContentsB)

                Assert.Contains("Hello from A", resxContentsA)
                Assert.Contains("Hello from B", resxContentsB)
                Assert.DoesNotContain("Hello from B", resxContentsA)
                Assert.DoesNotContain("Hello from A", resxContentsB)

                Assert.Empty(Directory.GetFiles(decoyDirectory.FullName, "*", SearchOption.AllDirectories))
            finally
                disposeTaskEnvironment environmentA
                disposeTaskEnvironment environmentB)

    [<Fact>]
    member _.``FSharpEmbedResXSource loads each relative resx and writes a generated .fs relative to each task's TaskEnvironment, not the process current directory``
        ()
        =
        withDecoyCurrentDirectory (fun decoyDirectory ->
            let environmentA, directoryA = createTaskEnvironmentInTemporaryDirectory ()
            let environmentB, directoryB = createTaskEnvironmentInTemporaryDirectory ()

            try
                // Both instances share the exact same relative paths; only the TaskEnvironment each is
                // wired to (and therefore the project directory the paths are rooted against) differs.
                let relativeIntermediate = Path.Combine("obj", "Debug")
                let relativeResx = "Resource.resx"

                let resxContent (value: string) =
                    sprintf
                        "<?xml version=\"1.0\" encoding=\"utf-8\"?><root><data name=\"Greeting\"><value>%s</value></data></root>"
                        value

                Directory.CreateDirectory(Path.Combine(directoryA.FullName, relativeIntermediate))
                |> ignore

                Directory.CreateDirectory(Path.Combine(directoryB.FullName, relativeIntermediate))
                |> ignore

                File.WriteAllText(Path.Combine(directoryA.FullName, relativeResx), resxContent "Hello from A")
                File.WriteAllText(Path.Combine(directoryB.FullName, relativeResx), resxContent "Hello from B")

                let makeTask (environment: TaskEnvironment) =
                    let embeddedResource = TaskItem(relativeResx) :> ITaskItem
                    embeddedResource.SetMetadata("GenerateSource", "true")

                    let task =
                        FSharpEmbedResXSource(
                            BuildEngine = MockEngine(),
                            EmbeddedResource = [| embeddedResource |],
                            IntermediateOutputPath = relativeIntermediate
                        )

                    assignTaskEnvironment (task :> IMultiThreadableTask) environment
                    task

                let taskA = makeTask environmentA
                let taskB = makeTask environmentB

                let successA, successB =
                    runConcurrently (fun () -> taskA.Execute()) (fun () -> taskB.Execute())

                Assert.True successA
                Assert.True successB

                let expectedSourceItemSpec = Path.Combine(relativeIntermediate, "Resource.fs")

                // The generated item's ItemSpec must remain the original, unrooted relative value, and is
                // identical for both instances since both were configured with identical inputs.
                Assert.Equal(expectedSourceItemSpec, taskA.GeneratedSource.[0].ItemSpec)
                Assert.Equal(expectedSourceItemSpec, taskB.GeneratedSource.[0].ItemSpec)

                let sourcePathA = Path.Combine(directoryA.FullName, expectedSourceItemSpec)
                let sourcePathB = Path.Combine(directoryB.FullName, expectedSourceItemSpec)

                Assert.True(File.Exists sourcePathA, sprintf "Expected generated file at %s" sourcePathA)
                Assert.True(File.Exists sourcePathB, sprintf "Expected generated file at %s" sourcePathB)

                let contentsA = File.ReadAllText sourcePathA
                let contentsB = File.ReadAllText sourcePathB

                Assert.Contains("Hello from A", contentsA)
                Assert.Contains("Hello from B", contentsB)
                Assert.DoesNotContain("Hello from B", contentsA)
                Assert.DoesNotContain("Hello from A", contentsB)

                Assert.Empty(Directory.GetFiles(decoyDirectory.FullName, "*", SearchOption.AllDirectories))
            finally
                disposeTaskEnvironment environmentA
                disposeTaskEnvironment environmentB)

    [<Fact>]
    member _.``FSharpEmbedResXSource logs exactly one MSBuild error and writes no console output for malformed XML``
        ()
        =
        let environment, directory = createTaskEnvironmentInTemporaryDirectory ()

        try
            let relativeResx = "Malformed.resx"

            File.WriteAllText(
                Path.Combine(directory.FullName, relativeResx),
                "<?xml version=\"1.0\" encoding=\"utf-8\"?><root><data name=\"Broken\"><value>Oops</root>"
            )

            let embeddedResource = TaskItem(relativeResx) :> ITaskItem
            embeddedResource.SetMetadata("GenerateSource", "true")

            let engine = MockEngine()

            let task =
                FSharpEmbedResXSource(
                    BuildEngine = engine,
                    EmbeddedResource = [| embeddedResource |],
                    IntermediateOutputPath = "obj"
                )

            assignTaskEnvironment (task :> IMultiThreadableTask) environment

            let originalConsoleOut = Console.Out
            let originalConsoleError = Console.Error
            use capturedConsoleOut = new StringWriter()
            use capturedConsoleError = new StringWriter()
            Console.SetOut capturedConsoleOut
            Console.SetError capturedConsoleError

            let result =
                try
                    task.Execute()
                finally
                    Console.SetOut originalConsoleOut
                    Console.SetError originalConsoleError

            // Execute must fail without throwing (Execute=false), and without emitting any output
            // via Console.Out/Console.Error, since only MSBuild's own error reporting is permitted.
            Assert.False result
            Assert.Equal("", capturedConsoleOut.ToString())
            Assert.Equal("", capturedConsoleError.ToString())

            let error = Assert.Single(engine.Errors)

            // The diagnostic must name the original, unrooted relative resx input, and must never
            // surface the rooted task project directory the file was actually loaded from.
            Assert.Contains(relativeResx, error.Message)
            Assert.DoesNotContain(directory.FullName, error.Message)
        finally
            disposeTaskEnvironment environment

    [<Fact>]
    member _.``FSharpEmbedResXSource logs exactly one MSBuild error for a data element missing its name attribute``
        ()
        =
        let environment, directory = createTaskEnvironmentInTemporaryDirectory ()

        try
            let relativeResx = "MissingName.resx"

            // Well-formed XML, but the `<data>` element lacks the required `name` attribute, which
            // fails via `failTask` (shared with FSharpEmbedResourceText) rather than an XML parse error.
            File.WriteAllText(
                Path.Combine(directory.FullName, relativeResx),
                "<?xml version=\"1.0\" encoding=\"utf-8\"?><root><data><value>Oops</value></data></root>"
            )

            let embeddedResource = TaskItem(relativeResx) :> ITaskItem
            embeddedResource.SetMetadata("GenerateSource", "true")

            let engine = MockEngine()

            let task =
                FSharpEmbedResXSource(
                    BuildEngine = engine,
                    EmbeddedResource = [| embeddedResource |],
                    IntermediateOutputPath = "obj"
                )

            assignTaskEnvironment (task :> IMultiThreadableTask) environment

            let result = task.Execute()

            // failTask already logs the error and raises TaskFailed; the catch in generateSource must
            // not log a second error for this exception, so exactly one error is expected, and it must
            // not contain a duplicated stack trace (which only the general-exception branch would emit).
            Assert.False result
            let error = Assert.Single(engine.Errors)
            Assert.Contains("Missing resource name", error.Message)
            Assert.DoesNotContain("An exception occurred when processing", error.Message)
            Assert.DoesNotContain("TaskFailed", error.Message)
            Assert.DoesNotContain("   at ", error.Message)
        finally
            disposeTaskEnvironment environment

    [<Fact>]
    member _.``SubstituteText reads and writes relative to each task's TaskEnvironment, not the process current directory``
        ()
        =
        withDecoyCurrentDirectory (fun decoyDirectory ->
            let environmentA, directoryA = createTaskEnvironmentInTemporaryDirectory ()
            let environmentB, directoryB = createTaskEnvironmentInTemporaryDirectory ()

            try
                // Both instances share the exact same relative source/target names; only the
                // TaskEnvironment each is wired to (and therefore the project directory the paths are
                // rooted against) differs.
                let relativeIntermediate = Path.Combine("obj", "Debug")
                let relativeSource = "Source.txt"

                File.WriteAllText(Path.Combine(directoryA.FullName, relativeSource), "Hello from A. Token: PLACEHOLDER")
                File.WriteAllText(Path.Combine(directoryB.FullName, relativeSource), "Hello from B. Token: PLACEHOLDER")

                let makeTask (environment: TaskEnvironment) =
                    let embeddedResource = TaskItem(relativeSource) :> ITaskItem
                    embeddedResource.SetMetadata("IntermediateTargetPath", relativeIntermediate)
                    embeddedResource.SetMetadata("Pattern1", "PLACEHOLDER")
                    embeddedResource.SetMetadata("Replacement1", "REPLACED")

                    let task =
                        SubstituteText(BuildEngine = MockEngine(), EmbeddedResources = [| embeddedResource |])

                    assignTaskEnvironment (task :> IMultiThreadableTask) environment
                    task

                let taskA = makeTask environmentA
                let taskB = makeTask environmentB

                let successA, successB =
                    runConcurrently (fun () -> taskA.Execute()) (fun () -> taskB.Execute())

                Assert.True successA
                Assert.True successB

                let expectedItemSpec = Path.Combine(relativeIntermediate, "Source.txt")

                // The copied item's ItemSpec must remain the original, unrooted relative value, and is
                // identical for both instances since both were configured with identical inputs.
                Assert.Equal(expectedItemSpec, taskA.CopiedFiles.[0].ItemSpec)
                Assert.Equal(expectedItemSpec, taskB.CopiedFiles.[0].ItemSpec)

                let targetPathA = Path.Combine(directoryA.FullName, expectedItemSpec)
                let targetPathB = Path.Combine(directoryB.FullName, expectedItemSpec)

                Assert.True(File.Exists targetPathA, sprintf "Expected substituted file at %s" targetPathA)
                Assert.True(File.Exists targetPathB, sprintf "Expected substituted file at %s" targetPathB)

                let contentsA = File.ReadAllText targetPathA
                let contentsB = File.ReadAllText targetPathB

                Assert.Equal("Hello from A. Token: REPLACED", contentsA)
                Assert.Equal("Hello from B. Token: REPLACED", contentsB)

                Assert.Empty(Directory.GetFiles(decoyDirectory.FullName, "*", SearchOption.AllDirectories))
            finally
                disposeTaskEnvironment environmentA
                disposeTaskEnvironment environmentB)

    [<Fact>]
    member _.``SubstituteText swallows a missing source file but still records the computed target ItemSpec``
        ()
        =
        let environment, _directory = createTaskEnvironmentInTemporaryDirectory ()

        try
            // Deliberately do not create "Missing.txt" on disk: File.ReadAllText must throw, and the
            // existing broad catch must swallow it, preserving Execute=true.
            let embeddedResource = TaskItem("Missing.txt") :> ITaskItem
            embeddedResource.SetMetadata("IntermediateTargetPath", "obj")
            embeddedResource.SetMetadata("Pattern1", "PLACEHOLDER")
            embeddedResource.SetMetadata("Replacement1", "REPLACED")

            let task =
                SubstituteText(BuildEngine = MockEngine(), EmbeddedResources = [| embeddedResource |])

            assignTaskEnvironment (task :> IMultiThreadableTask) environment

            let result = task.Execute()

            let expectedItemSpec = Path.Combine("obj", "Missing.txt")

            // Existing semantics (preserved unchanged): Execute still reports success, the item is
            // still recorded in CopiedFiles, and its ItemSpec was already rewritten to the computed
            // target before the (swallowed) I/O failure, even though nothing was ever written.
            Assert.True result
            let copiedItem = Assert.Single(task.CopiedFiles)
            Assert.Equal(expectedItemSpec, copiedItem.ItemSpec)
            Assert.False(File.Exists(Path.Combine(_directory.FullName, expectedItemSpec)))
        finally
            disposeTaskEnvironment environment
