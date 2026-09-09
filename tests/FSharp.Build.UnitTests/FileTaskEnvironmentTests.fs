// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Build.UnitTests

open System
open System.IO
open System.Runtime.InteropServices
open Microsoft.Build.Framework
open Microsoft.Build.Utilities
open FSharp.Build
open Xunit
open BuildTaskTestHelpers

type ResourceTaskKind =
    | Resx
    | Text

type ResourcePathShape =
    | Relative
    | Absolute
    | DotSegment
    | Invalid
    | RootRelative
    | DriveRelative

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

    let runConcurrently scenario executeA executeB =
        let results =
            runConcurrentlyWithBarrier
                scenario
                [
                    (fun release ->
                        release ()
                        executeA ())
                    (fun release ->
                        release ()
                        executeB ())
                ]

        Assert.True(results[0], $"{scenario}: task A failed")
        Assert.True(results[1], $"{scenario}: task B failed")

    let withIsolatedTaskEnvironmentPair body =
        withDecoyCurrentDirectory (fun decoy ->
            withTaskEnvironmentPairUsing
                createTaskEnvironmentInTemporaryDirectory
                (fun environmentA directoryA environmentB directoryB ->
                    body environmentA directoryA environmentB directoryB
                    Assert.Empty(Directory.GetFiles(decoy.FullName, "*", SearchOption.AllDirectories))))

    let createResourceTask kind environment engine (input: string) (intermediate: string) : ITask * (unit -> ITaskItem[]) =
        match kind with
        | Resx ->
            let item = TaskItem(input) :> ITaskItem
            item.SetMetadata("GenerateSource", "true")
            item.SetMetadata("GeneratedModuleName", "Resource")

            let task =
                FSharpEmbedResXSource(
                    BuildEngine = engine,
                    EmbeddedResource = [| item |],
                    IntermediateOutputPath = intermediate
                )
                |> assignTaskEnvironment environment

            (task :> ITask), (fun () -> task.GeneratedSource)
        | Text ->
            let task =
                FSharpEmbedResourceText(
                    BuildEngine = engine,
                    EmbeddedText = [| TaskItem(input) :> ITaskItem |],
                    IntermediateOutputPath = intermediate
                )
                |> assignTaskEnvironment environment

            (task :> ITask), (fun () -> Array.append task.GeneratedSource task.GeneratedResx)

    let resourceKindInfo =
        function
        | Resx -> "FSharpEmbedResXSource", ".resx"
        | Text -> "FSharpEmbedResourceText", ".txt"

    let resourceContent kind marker =
        match kind with
        | Resx -> $"""<root><data name="Greeting"><value>{marker}</value></data></root>"""
        | Text -> $"greeting,\"{marker}\"\n"

    let countOccurrences (needle: string) (text: string) =
        text.Split([| needle |], StringSplitOptions.None).Length - 1

    static member ResourceKinds = [ for kind in [ Resx; Text ] -> [| box kind |] ]

    static member DiagnosticPaths =
        [
            for kind in [ Resx; Text ] do
                for shape in
                    [
                        Relative; Absolute; DotSegment; Invalid
                        if RuntimeInformation.IsOSPlatform OSPlatform.Windows then
                            RootRelative
                            DriveRelative
                    ] do
                    yield [| box kind; box shape |]
        ]

    static member OutputFailures =
        [ for kind, extension in [ Resx, ".fs"; Text, ".fs"; Text, ".fsi"; Text, ".resx" ] ->
            [| box kind; box extension |] ]

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
                |> assignTaskEnvironment environment

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
                |> assignTaskEnvironment environment

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
                |> assignTaskEnvironment environment

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

    [<Theory>]
    [<InlineData("AssemblyA", false)>]
    [<InlineData("AssemblyB", true)>]
    member _.``ILLink substitutions only rewrite changed content``(assemblyName: string, changed: bool) =
        withTaskEnvironment (fun environment directory ->
            let task =
                GenerateILLinkSubstitutions(BuildEngine = MockEngine(), AssemblyName = "AssemblyA", IntermediateOutputPath = "obj")
                |> assignTaskEnvironment environment
            Assert.True(task.Execute())
            let output = Assert.Single(task.GeneratedItems).ItemSpec
            let file = Path.Combine(directory.FullName, output)
            let timestamp = DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            File.SetLastWriteTimeUtc(file, timestamp)
            task.AssemblyName <- assemblyName
            Assert.True(task.Execute())
            Assert.Equal(changed, File.GetLastWriteTimeUtc(file) <> timestamp)
            Assert.Contains($"fullname=\"{assemblyName}\"", File.ReadAllText file)
            Assert.Equal(output, Assert.Single(task.GeneratedItems).ItemSpec))

    [<Theory>]
    [<MemberData(nameof FileTaskEnvironmentTests.ResourceKinds)>]
    member _.``Resource generators isolate relative input and output paths per task``(kind: ResourceTaskKind) =
        withIsolatedTaskEnvironmentPair (fun environmentA directoryA environmentB directoryB ->
            let scenario, extension = resourceKindInfo kind
            let intermediate = Path.Combine("obj", "Debug")
            let input = "Resource" + extension

            for directory in [ directoryA; directoryB ] do
                Directory.CreateDirectory(Path.Combine(directory.FullName, intermediate))
                |> ignore

            File.WriteAllText(Path.Combine(directoryA.FullName, input), resourceContent kind "Hello from A")
            File.WriteAllText(Path.Combine(directoryB.FullName, input), resourceContent kind "Hello from B")

            let taskA, outputA = createResourceTask kind environmentA (MockEngine()) input intermediate
            let taskB, outputB = createResourceTask kind environmentB (MockEngine()) input intermediate
            runConcurrently scenario taskA.Execute taskB.Execute

            let generatedSpecs (output: unit -> ITaskItem[]) = output () |> Array.map _.ItemSpec

            let source = Path.Combine(intermediate, "Resource.fs")
            let expectedSpecs, contentSpecs =
                match kind with
                | Resx -> [ source ], [ source ]
                | Text ->
                    let signature = Path.Combine(intermediate, "Resource.fsi")
                    let resx = Path.Combine(intermediate, "Resource.resx")
                    [ signature; source; resx ], [ source; resx ]

            for output in [ outputA; outputB ] do
                Assert.Equal<string[]>(List.toArray expectedSpecs, generatedSpecs output)

            for directory, own, other in
                [ directoryA, "Hello from A", "Hello from B"; directoryB, "Hello from B", "Hello from A" ] do
                for spec in expectedSpecs do
                    assertFileExists scenario (Path.Combine(directory.FullName, spec))

                for spec in contentSpecs do
                    let contents = File.ReadAllText(Path.Combine(directory.FullName, spec))
                    assertContains scenario own contents
                    assertNotContains scenario other contents)

    [<Theory>]
    [<InlineData("Malformed.resx", "<root><data name=\"Broken\"><value>Oops</root>", "Malformed.resx", false)>]
    [<InlineData("MissingName.resx", "<root><data><value>Oops</value></data></root>", "Missing resource name", true)>]
    [<InlineData("MissingValue.resx", "<root><data name=\"Broken\"/></root>", "Missing resource value", true)>]
    [<InlineData("Malformed.txt", "bad syntax without comma", "comma", true)>]
    member _.``Resource failures log once without console output``(input: string, content: string, expected: string, taskFailed: bool) =
        withTaskEnvironment (fun environment directory ->
            File.WriteAllText(Path.Combine(directory.FullName, input), content)
            let kind = if Path.GetExtension(input) = ".txt" then Text else Resx
            let engine = MockEngine()
            let task, _ = createResourceTask kind environment engine input "obj"
            use capture = new FSharp.Test.TestConsole.ExecutionCapture()
            Assert.False(task.Execute())
            Assert.Equal("", capture.OutText)
            Assert.Equal("", capture.ErrorText)
            let message = (Assert.Single(engine.Errors)).Message
            assertContains input expected message
            assertNotContains input directory.FullName message

            if taskFailed then
                for leaked in [ "An exception occurred when processing"; "TaskFailed"; "   at " ] do
                    assertNotContains input leaked message)

    [<Theory>]
    [<MemberData(nameof FileTaskEnvironmentTests.DiagnosticPaths)>]
    member _.``Resource task diagnostics preserve every input path shape``(kind: ResourceTaskKind, shape: ResourcePathShape) =
        withTaskEnvironment (fun environment directory ->
            let kindName, extension = resourceKindInfo kind
            let input, expectation =
                match shape with
                | Relative -> $"Missing{extension}", Restored
                | Absolute -> Path.Combine(directory.FullName, $"AbsentUnderProject{extension}"), PreservedAbsolute
                | DotSegment -> Path.Combine("sub", "..", $"Missing{extension}"), Restored
                | Invalid -> $"in|valid{extension}", Restored
                | RootRelative -> $@"\Missing{extension}", PartiallyQualified
                | DriveRelative -> $@"C:Missing{extension}", PartiallyQualified
            let scenario = $"{kindName}: {shape}"
            let engine = MockEngine()
            let task, _ = createResourceTask kind environment engine input "obj"
            Assert.False(task.Execute(), scenario)
            let error = Assert.Single(engine.Errors)
            let message = error.Message
            if kind = Text then Assert.Equal(input, error.File)
            assertContains scenario input (message + "\n" + error.File)

            match expectation with
            | PreservedAbsolute ->
                Assert.True(countOccurrences input message >= 2, $"{scenario}: expected original path in prefix and exception")
            | Restored -> assertNotContains scenario directory.FullName message
            | PartiallyQualified ->
                assertNotContains scenario (environment.GetAbsolutePath(input).Value) message
                assertNotContains scenario directory.FullName message)

    [<Theory>]
    [<MemberData(nameof FileTaskEnvironmentTests.OutputFailures)>]
    member _.``Resource write failures preserve output paths``(kind: ResourceTaskKind, outputExtension: string) =
        withTaskEnvironment (fun environment directory ->
            let _, inputExtension = resourceKindInfo kind
            let input = "Resource" + inputExtension
            let output = Path.Combine("obj", "Resource" + outputExtension)
            File.WriteAllText(Path.Combine(directory.FullName, input), resourceContent kind "Hello")
            Directory.CreateDirectory(Path.Combine(directory.FullName, output)) |> ignore
            let engine = MockEngine()
            let task, _ = createResourceTask kind environment engine input "obj"
            Assert.False(task.Execute())
            let message = (Assert.Single(engine.Errors)).Message
            assertContains output output message
            assertNotContains output directory.FullName message)

    [<Fact>]
    member _.``FSharpEmbedResourceText restores overlapping paths longest first``() =
        withTaskEnvironment (fun environment directory ->
            let shortOriginal = "p"
            let longOriginal = Path.Combine("p", "deeper", "..", "deeper")
            let canonicalLong = Path.GetFullPath(environment.GetAbsolutePath(longOriginal).Value)

            let message = $"Could not find a part of the path '{canonicalLong}'."

            let actual =
                TaskEnvironmentPaths.restoreOriginalPaths environment message [ shortOriginal; longOriginal ]

            Assert.Equal($"Could not find a part of the path '{longOriginal}'.", actual)
            assertNotContains "overlapping path restoration" directory.FullName actual)

    [<Fact>]
    member _.``FSharpEmbedResourceText leaves a rooted path that is only a lexical prefix untouched``() =
        withTaskEnvironment (fun environment _ ->
            // rooted "p" is a lexical prefix of rooted "parts/x"; restoration must not rewrite the latter.
            let original = "p"
            let unrelated = environment.GetAbsolutePath(Path.Combine("parts", "x")).Value
            let message = $"Could not find a part of the path '{unrelated}'."

            let actual =
                TaskEnvironmentPaths.restoreOriginalPaths environment message [ original ]

            Assert.Equal(message, actual))

    [<Theory>]
    [<InlineData("'{0}'", true)>]
    [<InlineData("\"{0}\"", true)>]
    [<InlineData("{0}", true)>]
    [<InlineData("at Handler in {0}:line 42", true)>]
    [<InlineData("Cannot open {0} because it is locked", true)>]
    [<InlineData("({0})", true)>]
    [<InlineData("[{0}]", true)>]
    [<InlineData("{0}\nnext diagnostic", true)>]
    [<InlineData("'{0}/child'", true)>]
    [<InlineData("'{0}.backup'", false)>]
    [<InlineData("'{0} backup'", false)>]
    [<InlineData("'{0},backup'", false)>]
    [<InlineData("'{0})backup'", false)>]
    [<InlineData("prefix{0}'", false)>]
    member _.``Path restoration respects diagnostic delimiters and quoted filenames``(format: string, restore: bool) =
        withTaskEnvironment (fun environment _ ->
            let original = "file.fs"
            let rooted = environment.GetAbsolutePath(original).Value
            let message = String.Format(format, rooted)
            let expected = String.Format(format, if restore then original else rooted)
            Assert.Equal(expected, TaskEnvironmentPaths.restoreOriginalPaths environment message [ original ]))

    [<Theory>]
    [<InlineData(false, false)>]
    [<InlineData(false, true)>]
    [<InlineData(true, false)>]
    [<InlineData(true, true)>]
    member _.``RichText incremental generation respects metadata changes``(before: bool, after: bool) =
        withTaskEnvironment (fun environment directory ->
            let input = "Toggle.txt"
            let intermediate = "obj"
            Directory.CreateDirectory(Path.Combine(directory.FullName, intermediate)) |> ignore
            let source = Path.Combine(directory.FullName, input)
            File.WriteAllText(source, "greeting,\"Hello\"\n")
            File.SetLastWriteTimeUtc(source, DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc))

            let runWith richText =
                let item = TaskItem(input) :> ITaskItem
                item.SetMetadata("RichText", if richText then "true" else "false")

                let task =
                    FSharpEmbedResourceText(
                        BuildEngine = MockEngine(),
                        EmbeddedText = [| item |],
                        IntermediateOutputPath = intermediate
                    )
                    |> assignTaskEnvironment environment

                Assert.True(task.Execute(), "RichText toggle: task should succeed")

            let generatedFs = Path.Combine(directory.FullName, intermediate, "Toggle.fs")
            let richTextOpen = "open FSharp.Compiler.Text"
            runWith before
            Assert.Equal(before, File.ReadAllText(generatedFs).Contains richTextOpen)
            let outputs = [ for ext in [ ".fs"; ".fsi"; ".resx" ] -> Path.ChangeExtension(generatedFs, ext) ]
            let stamp = DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            for output in outputs do File.SetLastWriteTimeUtc(output, stamp)
            runWith after
            Assert.Equal(after, File.ReadAllText(generatedFs).Contains richTextOpen)
            for output in outputs do
                Assert.Equal((before = after), (File.GetLastWriteTimeUtc(output) = stamp)))

    [<Theory>]
    [<InlineData("PLACEHOLDER", "REPLACED", "", "", "REPLACED")>]
    [<InlineData("", "", "PLACEHOLDER", "SECOND", "SECOND")>]
    [<InlineData("PLACEHOLDER", "STAGE2", "STAGE2", "FINAL", "FINAL")>]
    [<InlineData(" ", "UNUSED", "", "UNUSED", "PLACEHOLDER")>]
    member _.``SubstituteText isolates ordered replacements``(pattern1: string, replacement1: string, pattern2: string, replacement2: string, expected: string) =
        withIsolatedTaskEnvironmentPair (fun environmentA directoryA environmentB directoryB ->
            let input = "Source.txt"
            let intermediate = Path.Combine("obj", "Debug")
            File.WriteAllText(Path.Combine(directoryA.FullName, input), "Hello from A. Token: PLACEHOLDER")
            File.WriteAllText(Path.Combine(directoryB.FullName, input), "Hello from B. Token: PLACEHOLDER")

            let makeTask environment =
                let item = TaskItem(input) :> ITaskItem
                item.SetMetadata("IntermediateTargetPath", intermediate)
                item.SetMetadata("Pattern1", pattern1)
                item.SetMetadata("Replacement1", replacement1)
                item.SetMetadata("Pattern2", pattern2)
                item.SetMetadata("Replacement2", replacement2)
                SubstituteText(BuildEngine = MockEngine(), EmbeddedResources = [| item |])
                |> assignTaskEnvironment environment

            let taskA, taskB = makeTask environmentA, makeTask environmentB
            let scenario = "SubstituteText isolates relative input and output paths per task"
            runConcurrently scenario taskA.Execute taskB.Execute

            let noReplacement = String.IsNullOrWhiteSpace pattern1 && String.IsNullOrWhiteSpace pattern2
            let expectedItemSpec = if noReplacement then input else Path.Combine(intermediate, input)

            for directory, expected, task in
                [
                    directoryA, $"Hello from A. Token: {expected}", taskA
                    directoryB, $"Hello from B. Token: {expected}", taskB
                ] do
                Assert.Equal(expectedItemSpec, Assert.Single(task.CopiedFiles).ItemSpec)
                let path = Path.Combine(directory.FullName, expectedItemSpec)
                assertFileExists expected path
                Assert.Equal(expected, File.ReadAllText path)
                if noReplacement then Assert.False(Directory.Exists(Path.Combine(directory.FullName, intermediate))))

    [<Fact>]
    member _.``SubstituteText preserves its missing-source success behavior``() =
        withTaskEnvironment (fun environment directory ->
            let item = TaskItem("Missing.txt") :> ITaskItem
            item.SetMetadata("IntermediateTargetPath", "obj")
            item.SetMetadata("Pattern1", "PLACEHOLDER")
            item.SetMetadata("Replacement1", "REPLACED")

            let task =
                SubstituteText(BuildEngine = MockEngine(), EmbeddedResources = [| item |])
                |> assignTaskEnvironment environment

            Assert.True(task.Execute())
            let expectedItemSpec = Path.Combine("obj", "Missing.txt")
            Assert.Equal(expectedItemSpec, Assert.Single(task.CopiedFiles).ItemSpec)
            Assert.False(File.Exists(Path.Combine(directory.FullName, expectedItemSpec))))
