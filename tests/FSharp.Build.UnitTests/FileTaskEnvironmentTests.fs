// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Build.UnitTests

open System
open System.IO
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks
open Microsoft.Build.Framework
open Microsoft.Build.Utilities
open FSharp.Build
open Xunit
open BuildTaskTestHelpers

/// Verifies FSharp.Build file-I/O tasks resolve relative paths against each task instance's
/// TaskEnvironment rather than the process-wide current directory. Each test runs two instances
/// concurrently, each wired to its own project directory/engine, while the process current directory
/// points at a third "decoy" directory that must stay untouched.
[<Collection(nameof FSharp.Test.NotThreadSafeResourceCollection)>]
type FileTaskEnvironmentTests() =

    /// Points Environment.CurrentDirectory at a fresh decoy directory for `body`, then restores it.
    let withDecoyCurrentDirectory (body: DirectoryInfo -> unit) =
        let decoyDirectory = TestFramework.createTemporaryDirectory ()
        let originalCurrentDirectory = Environment.CurrentDirectory

        try
            Environment.CurrentDirectory <- decoyDirectory.FullName
            body decoyDirectory
        finally
            Environment.CurrentDirectory <- originalCurrentDirectory

    /// Releases both executions together via a Barrier, then waits for both. A timeout turns a deadlock
    /// in the code under test into a failure rather than a hang.
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

    /// Runs `body` against a decoy current directory and a disposed TaskEnvironment pair, then asserts
    /// the decoy received no writes (proving no task fell back to ambient process state).
    let withIsolatedTaskEnvironmentPair body =
        withDecoyCurrentDirectory (fun decoyDirectory ->
            withTaskEnvironmentPair (fun environmentA directoryA environmentB directoryB ->
                body environmentA directoryA environmentB directoryB
                Assert.Empty(Directory.GetFiles(decoyDirectory.FullName, "*", SearchOption.AllDirectories))))

    /// Counts non-overlapping occurrences of `needle` in `text`, proving an absolute input path survives
    /// diagnostic scrubbing in the exception body, which a blind project-directory strip would corrupt.
    let countOccurrences (needle: string) (text: string) =
        let mutable count = 0
        let mutable index = text.IndexOf(needle, StringComparison.Ordinal)

        while index >= 0 do
            count <- count + 1
            index <- text.IndexOf(needle, index + needle.Length, StringComparison.Ordinal)

        count

    /// Asserts a failing task's diagnostic keeps a Windows partially qualified input's original spelling
    /// (root-relative '\foo' / drive-relative 'C:foo') and leaks neither its expanded rooted form nor the
    /// project directory, even though TaskEnvironment expands it against the project directory.
    let assertPartiallyQualifiedDiagnostic
        (environment: TaskEnvironment)
        (projectDirectory: string)
        (original: string)
        (message: string)
        =
        let rooted = environment.GetAbsolutePath(original).Value

        Assert.Contains(original, message)
        Assert.DoesNotContain(rooted, message)
        Assert.DoesNotContain(projectDirectory, message)

    [<Fact>]
    member _.``WriteCodeFragment writes relative to each task's TaskEnvironment, not the process current directory``
        ()
        =
        withIsolatedTaskEnvironmentPair (fun environmentA directoryA environmentB directoryB ->
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

            Assert.Equal("Generated.fs", taskA.OutputFile.ItemSpec)
            Assert.Equal("Generated.fs", taskB.OutputFile.ItemSpec))

    [<Fact>]
    member _.``WriteCodeFragment preserves its OutputDirectory quirk while rooting the write against TaskEnvironment``
        ()
        =
        withTaskEnvironment (fun environment directory ->
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

            // Preserved quirk: the file is written using OutputFile.ItemSpec alone, ignoring
            // OutputDirectory, even though OutputDirectory is folded into the returned OutputFile item.
            let writtenPath = Path.Combine(directory.FullName, "Generated2.fs")
            Assert.True(File.Exists writtenPath, sprintf "Expected generated file at %s" writtenPath)
            Assert.False(File.Exists(Path.Combine(directory.FullName, "SubDir", "Generated2.fs")))

            Assert.Equal(Path.Combine("SubDir", "Generated2.fs"), task.OutputFile.ItemSpec))

    [<Fact>]
    member _.``GenerateILLinkSubstitutions writes relative to each task's TaskEnvironment, not the process current directory``
        ()
        =
        withIsolatedTaskEnvironmentPair (fun environmentA directoryA environmentB directoryB ->
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

            // The generated item's ItemSpec must remain the original, unrooted relative value.
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
            Assert.DoesNotContain("AssemblyA", contentsB))

    [<Fact>]
    member _.``FSharpEmbedResourceText generates .fs, .fsi and .resx relative to each task's TaskEnvironment, not the process current directory``
        ()
        =
        withIsolatedTaskEnvironmentPair (fun environmentA directoryA environmentB directoryB ->
            // Both instances share identical relative paths; only their TaskEnvironment differs.
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
            Assert.DoesNotContain("Hello from A", resxContentsB))

    [<Fact>]
    member _.``FSharpEmbedResXSource loads each relative resx and writes a generated .fs relative to each task's TaskEnvironment, not the process current directory``
        ()
        =
        withIsolatedTaskEnvironmentPair (fun environmentA directoryA environmentB directoryB ->
            // Both instances share identical relative paths; only their TaskEnvironment differs.
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
            Assert.DoesNotContain("Hello from A", contentsB))

    [<Fact>]
    member _.``FSharpEmbedResXSource logs exactly one MSBuild error and writes no console output for malformed XML``
        ()
        =
        withTaskEnvironment (fun environment directory ->
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

            // Execute must fail without throwing and without any Console output (only MSBuild's own
            // error reporting is permitted).
            Assert.False result
            Assert.Equal("", capturedConsoleOut.ToString())
            Assert.Equal("", capturedConsoleError.ToString())

            let error = Assert.Single(engine.Errors)

            Assert.Contains(relativeResx, error.Message)
            Assert.DoesNotContain(directory.FullName, error.Message))

    [<Fact>]
    member _.``FSharpEmbedResXSource logs exactly one MSBuild error for a data element missing its name attribute``
        ()
        =
        withTaskEnvironment (fun environment directory ->
            let relativeResx = "MissingName.resx"

            // Well-formed XML whose `<data>` element lacks the required `name` attribute, so it fails
            // via the shared `failTask` rather than an XML parse error.
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

            // failTask logs the error and raises TaskFailed; the catch must not log a second error, so
            // exactly one error is expected, with no duplicated stack trace.
            Assert.False result
            let error = Assert.Single(engine.Errors)
            Assert.Contains("Missing resource name", error.Message)
            Assert.DoesNotContain("An exception occurred when processing", error.Message)
            Assert.DoesNotContain("TaskFailed", error.Message)
            Assert.DoesNotContain("   at ", error.Message))

    [<Fact>]
    member _.``FSharpEmbedResXSource keeps a relative resx input relative in diagnostics and never leaks the project directory``
        ()
        =
        withTaskEnvironment (fun environment directory ->
            // The resx does not exist, so XDocument.Load throws a FileNotFoundException naming the rooted
            // absolute path; the diagnostic must be restored to the original relative input.
            let relativeResx = "Missing.resx"

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

            Assert.False(task.Execute())
            let error = Assert.Single(engine.Errors)

            Assert.Contains(relativeResx, error.Message)
            Assert.DoesNotContain(directory.FullName, error.Message))

    [<Fact>]
    member _.``FSharpEmbedResXSource preserves an absolute resx input beneath the project directory in diagnostics``
        ()
        =
        withTaskEnvironment (fun environment directory ->
            // An absolute input beneath the project directory must survive intact: restoration by exact
            // rooted-value replacement is a no-op (rooted == original), so a blind project-directory
            // strip would wrongly corrupt it into a relative fragment.
            let absoluteResx = Path.Combine(directory.FullName, "AbsentUnderProject.resx")

            let embeddedResource = TaskItem(absoluteResx) :> ITaskItem
            embeddedResource.SetMetadata("GenerateSource", "true")

            let engine = MockEngine()

            let task =
                FSharpEmbedResXSource(
                    BuildEngine = engine,
                    EmbeddedResource = [| embeddedResource |],
                    IntermediateOutputPath = "obj"
                )

            assignTaskEnvironment (task :> IMultiThreadableTask) environment

            Assert.False(task.Execute())
            let error = Assert.Single(engine.Errors)

            // Present in the message prefix AND in the (FileNotFoundException) body, proving the absolute
            // path was not stripped to a relative fragment.
            Assert.Contains(absoluteResx, error.Message)
            Assert.True(countOccurrences absoluteResx error.Message >= 2, error.Message))

    [<Fact>]
    member _.``FSharpEmbedResXSource restores a dot-segment relative resx input and never leaks the project directory``
        ()
        =
        withTaskEnvironment (fun environment directory ->
            // GetAbsolutePath leaves the 'sub/..' segments in the rooted path, but the framework
            // canonicalizes to '<project>/Missing.resx' before throwing; both forms must map back to the
            // caller's original relative spelling.
            let relativeResx = Path.Combine("sub", "..", "Missing.resx")

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

            Assert.False(task.Execute())
            let error = Assert.Single(engine.Errors)

            Assert.Contains(relativeResx, error.Message)
            Assert.DoesNotContain(directory.FullName, error.Message))

    [<Fact>]
    member _.``FSharpEmbedResXSource handles an invalid path input inside its protected handler``() =
        withTaskEnvironment (fun environment directory ->
            // A '|' is rejected by the framework Path APIs on .NET Framework (path derivation throws) and
            // is a valid-but-missing filename on .NET Core (load throws). Because derivation stays inside
            // the try, both are caught: Execute fails without propagating, the diagnostic names the
            // caller's input, and the project directory never leaks.
            let invalidResx = "in|valid.resx"

            let embeddedResource = TaskItem(invalidResx) :> ITaskItem
            embeddedResource.SetMetadata("GenerateSource", "true")

            let engine = MockEngine()

            let task =
                FSharpEmbedResXSource(
                    BuildEngine = engine,
                    EmbeddedResource = [| embeddedResource |],
                    IntermediateOutputPath = "obj"
                )

            assignTaskEnvironment (task :> IMultiThreadableTask) environment

            Assert.False(task.Execute())
            let error = Assert.Single(engine.Errors)

            Assert.Contains(invalidResx, error.Message)
            Assert.DoesNotContain(directory.FullName, error.Message))

    [<Fact>]
    member _.``FSharpEmbedResXSource restores a Windows partially qualified resx input and never leaks the expanded rooted form``
        ()
        =
        // Windows-only: a leading '\' or 'C:' is an ordinary filename character on Unix.
        if RuntimeInformation.IsOSPlatform OSPlatform.Windows then
            withTaskEnvironment (fun environment directory ->
                // Root-relative ('\Missing.resx') and drive-relative ('C:Missing.resx') inputs are both
                // reported as rooted yet GetAbsolutePath expands them against the project directory before
                // XDocument.Load throws. The diagnostic must keep the caller's partially qualified spelling.
                for original in [ @"\Missing.resx"; @"C:Missing.resx" ] do
                    let embeddedResource = TaskItem(original) :> ITaskItem
                    embeddedResource.SetMetadata("GenerateSource", "true")

                    let engine = MockEngine()

                    let task =
                        FSharpEmbedResXSource(
                            BuildEngine = engine,
                            EmbeddedResource = [| embeddedResource |],
                            IntermediateOutputPath = "obj"
                        )

                    assignTaskEnvironment (task :> IMultiThreadableTask) environment

                    Assert.False(task.Execute())
                    let error = Assert.Single(engine.Errors)

                    assertPartiallyQualifiedDiagnostic environment directory.FullName original error.Message)

    [<Fact>]
    member _.``FSharpEmbedResourceText keeps a relative input relative in diagnostics and never leaks the project directory``
        ()
        =
        withTaskEnvironment (fun environment directory ->
            // The .txt does not exist, so File.ReadAllLines throws naming the rooted absolute path; the
            // diagnostic must surface only the original relative input.
            let relativeText = "Missing.txt"

            let engine = MockEngine()

            let task =
                FSharpEmbedResourceText(
                    BuildEngine = engine,
                    EmbeddedText = [| TaskItem(relativeText) :> ITaskItem |],
                    IntermediateOutputPath = "obj"
                )

            assignTaskEnvironment (task :> IMultiThreadableTask) environment

            Assert.False(task.Execute())
            let error = Assert.Single(engine.Errors)

            Assert.Contains(relativeText, error.Message)
            Assert.DoesNotContain(directory.FullName, error.Message))

    [<Fact>]
    member _.``FSharpEmbedResourceText preserves an absolute input beneath the project directory in diagnostics``
        ()
        =
        withTaskEnvironment (fun environment directory ->
            let absoluteText = Path.Combine(directory.FullName, "AbsentUnderProject.txt")

            let engine = MockEngine()

            let task =
                FSharpEmbedResourceText(
                    BuildEngine = engine,
                    EmbeddedText = [| TaskItem(absoluteText) :> ITaskItem |],
                    IntermediateOutputPath = "obj"
                )

            assignTaskEnvironment (task :> IMultiThreadableTask) environment

            Assert.False(task.Execute())
            let error = Assert.Single(engine.Errors)

            // Preserved in both the prefix and the (FileNotFoundException) body, unstripped.
            Assert.Contains(absoluteText, error.Message)
            Assert.True(countOccurrences absoluteText error.Message >= 2, error.Message))

    [<Fact>]
    member _.``FSharpEmbedResourceText restores a dot-segment relative input and never leaks the project directory``
        ()
        =
        withTaskEnvironment (fun environment directory ->
            // File.ReadAllLines canonicalizes 'sub/../Missing.txt' to '<project>/Missing.txt' before
            // throwing; both forms must map back to the caller's original spelling.
            let relativeText = Path.Combine("sub", "..", "Missing.txt")

            let engine = MockEngine()

            let task =
                FSharpEmbedResourceText(
                    BuildEngine = engine,
                    EmbeddedText = [| TaskItem(relativeText) :> ITaskItem |],
                    IntermediateOutputPath = "obj"
                )

            assignTaskEnvironment (task :> IMultiThreadableTask) environment

            Assert.False(task.Execute())
            let error = Assert.Single(engine.Errors)

            Assert.Contains(relativeText, error.Message)
            Assert.DoesNotContain(directory.FullName, error.Message))

    [<Fact>]
    member _.``FSharpEmbedResourceText handles an invalid path input inside its protected handler``() =
        withTaskEnvironment (fun environment directory ->
            // A '|' is rejected by the framework Path APIs on .NET Framework (derivation throws) and is a
            // valid-but-missing filename on .NET Core (tripping the letters-and-digits guard). Either way
            // the failure is handled inside the try, so Execute fails without propagating and no diagnostic
            // leaks the project directory. The diagnostic count differs by framework, so assert over the set.
            let invalidText = "in|valid.txt"

            let engine = MockEngine()

            let task =
                FSharpEmbedResourceText(
                    BuildEngine = engine,
                    EmbeddedText = [| TaskItem(invalidText) :> ITaskItem |],
                    IntermediateOutputPath = "obj"
                )

            assignTaskEnvironment (task :> IMultiThreadableTask) environment

            Assert.False(task.Execute())
            Assert.NotEmpty(engine.Errors)

            for error in engine.Errors do
                Assert.DoesNotContain(directory.FullName, error.Message)

            Assert.Contains(engine.Errors, (fun error -> error.Message.Contains invalidText)))

    [<Fact>]
    member _.``FSharpEmbedResourceText restores the longest rooted path first so overlapping prefixes are not corrupted``
        ()
        =
        withTaskEnvironment (fun environment directory ->
            // Prefix-overlap oracle. 'shortOriginal' roots to '<project>/p', a path-boundary prefix of
            // the canonicalized form of 'longOriginal' ('<project>/p/deeper'). Restoring the shorter form
            // first would rewrite the longer path into 'p/deeper', dropping the caller's dot-segment
            // spelling; longest-first yields the exact original. Exercised directly by reflection.
            let shortOriginal = "p"
            let longOriginal = Path.Combine("p", "deeper", "..", "deeper")

            // The framework canonicalizes before it throws, so the exception embeds the collapsed form.
            let canonicalLong = Path.GetFullPath(environment.GetAbsolutePath(longOriginal).Value)

            let task = FSharpEmbedResourceText(environment)

            let message = $"Could not find a part of the path '{canonicalLong}'."
            let expected = $"Could not find a part of the path '{longOriginal}'."

            let restore =
                FSharp.Test.ReflectionHelper.getPrivateInstanceMethod "InternalRestoreOriginalPaths" (task.GetType())

            let actual =
                restore.Invoke(task, [| box message; box [| shortOriginal; longOriginal |] |]) :?> string

            Assert.Equal(expected, actual)
            Assert.DoesNotContain(directory.FullName, actual))

    [<Fact>]
    member _.``FSharpEmbedResourceText restores a Windows partially qualified input and never leaks the expanded rooted form``
        ()
        =
        // Windows-only: a leading '\' or 'C:' is an ordinary file name character on Unix.
        if RuntimeInformation.IsOSPlatform OSPlatform.Windows then
            withTaskEnvironment (fun environment directory ->
                // Root-relative ('\Missing.txt') and drive-relative ('C:Missing.txt') inputs are both
                // reported as rooted yet GetAbsolutePath expands them against the project directory before
                // File.ReadAllLines throws. The diagnostic must keep the caller's partially qualified spelling.
                for original in [ @"\Missing.txt"; @"C:Missing.txt" ] do
                    let engine = MockEngine()

                    let task =
                        FSharpEmbedResourceText(
                            BuildEngine = engine,
                            EmbeddedText = [| TaskItem(original) :> ITaskItem |],
                            IntermediateOutputPath = "obj"
                        )

                    assignTaskEnvironment (task :> IMultiThreadableTask) environment

                    Assert.False(task.Execute())
                    let error = Assert.Single(engine.Errors)

                    assertPartiallyQualifiedDiagnostic environment directory.FullName original error.Message)

    [<Fact>]
    member _.``SubstituteText reads and writes relative to each task's TaskEnvironment, not the process current directory``
        ()
        =
        withIsolatedTaskEnvironmentPair (fun environmentA directoryA environmentB directoryB ->
            // Both instances share identical relative source/target names; only their TaskEnvironment differs.
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

            Assert.Equal(expectedItemSpec, taskA.CopiedFiles.[0].ItemSpec)
            Assert.Equal(expectedItemSpec, taskB.CopiedFiles.[0].ItemSpec)

            let targetPathA = Path.Combine(directoryA.FullName, expectedItemSpec)
            let targetPathB = Path.Combine(directoryB.FullName, expectedItemSpec)

            Assert.True(File.Exists targetPathA, sprintf "Expected substituted file at %s" targetPathA)
            Assert.True(File.Exists targetPathB, sprintf "Expected substituted file at %s" targetPathB)

            let contentsA = File.ReadAllText targetPathA
            let contentsB = File.ReadAllText targetPathB

            Assert.Equal("Hello from A. Token: REPLACED", contentsA)
            Assert.Equal("Hello from B. Token: REPLACED", contentsB))

    [<Fact>]
    member _.``SubstituteText swallows a missing source file but still records the computed target ItemSpec``
        ()
        =
        withTaskEnvironment (fun environment _directory ->
            // "Missing.txt" is never created: File.ReadAllText throws and the existing broad catch must
            // swallow it, preserving Execute=true.
            let embeddedResource = TaskItem("Missing.txt") :> ITaskItem
            embeddedResource.SetMetadata("IntermediateTargetPath", "obj")
            embeddedResource.SetMetadata("Pattern1", "PLACEHOLDER")
            embeddedResource.SetMetadata("Replacement1", "REPLACED")

            let task =
                SubstituteText(BuildEngine = MockEngine(), EmbeddedResources = [| embeddedResource |])

            assignTaskEnvironment (task :> IMultiThreadableTask) environment

            let result = task.Execute()

            let expectedItemSpec = Path.Combine("obj", "Missing.txt")

            // Preserved semantics: Execute still succeeds, the item is recorded in CopiedFiles, and its
            // ItemSpec was rewritten to the computed target before the swallowed I/O failure.
            Assert.True result
            let copiedItem = Assert.Single(task.CopiedFiles)
            Assert.Equal(expectedItemSpec, copiedItem.ItemSpec)
            Assert.False(File.Exists(Path.Combine(_directory.FullName, expectedItemSpec))))
