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
