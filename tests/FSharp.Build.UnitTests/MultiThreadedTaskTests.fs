// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Build.UnitTests

open System
open System.IO
open System.Runtime.InteropServices
open System.Threading
open Microsoft.Build.Framework
open Microsoft.Build.Utilities
open FSharp.Build
open Xunit
open BuildTaskTestHelpers

type FauxHostObject(?executeCompiler: bool) =
    let mutable flags: string[] = [||]
    let mutable sources: string[] = [||]

    member _.Compile(compile: Func<int>, flagsIn: string[], sourcesIn: string[]) =
        flags <- flagsIn
        sources <- sourcesIn
        if defaultArg executeCompiler false then compile.Invoke() else 0

    member _.Flags = flags
    member _.Sources = sources

    interface ITaskHost

type MultiThreadedTaskTests() =

    static member MultiThreadableTaskTypes: obj[] seq =
        [
            typeof<CreateFSharpManifestResourceName>
            typeof<MapSourceRoots>
            typeof<WriteCodeFragment>
            typeof<GenerateILLinkSubstitutions>
            typeof<FSharpEmbedResourceText>
            typeof<FSharpEmbedResXSource>
            typeof<SubstituteText>
            typeof<Fsc>
            typeof<Fsi>
        ]
        |> Seq.map (fun taskType -> [| box taskType |])

    [<Theory>]
    [<MemberData(nameof MultiThreadedTaskTests.MultiThreadableTaskTypes)>]
    member _.``task preserves its public shape and is marked directly multithreadable`` (taskType: Type) =
        let attributes = taskType.GetCustomAttributes(typeof<MSBuildMultiThreadableTaskAttribute>, false)
        Assert.True(attributes.Length = 1, $"{taskType.Name}: expected one direct multithreadable attribute")
        Assert.NotNull(taskType.GetConstructor(Type.EmptyTypes))
        Assert.Single(taskType.GetConstructors()) |> ignore

        let expectedBase =
            if taskType = typeof<Fsc> || taskType = typeof<Fsi> then
                typeof<ToolTask>
            elif taskType = typeof<CreateFSharpManifestResourceName> then
                typeof<Microsoft.Build.Tasks.CreateCSharpManifestResourceName>
            else
                typeof<Microsoft.Build.Utilities.Task>

        Assert.Equal(expectedBase, taskType.BaseType)
        if taskType <> typeof<MapSourceRoots> then
            Assert.True(typeof<IMultiThreadableTask>.IsAssignableFrom(taskType))

    [<Fact>]
    member _.``all concrete build tasks have a reviewed contract``() =
        let discovered =
            typeof<Fsc>.Assembly.GetTypes()
            |> Seq.filter (fun taskType -> taskType.IsPublic && not taskType.IsAbstract && typeof<ITask>.IsAssignableFrom taskType)
            |> Seq.map _.FullName
            |> Set.ofSeq
        let reviewed =
            MultiThreadedTaskTests.MultiThreadableTaskTypes
            |> Seq.map (fun row -> (row[0] :?> Type).FullName)
            |> Set.ofSeq
        Assert.Equal<Set<string>>(reviewed, discovered)

    [<Fact>]
    member _.``failed preparation releases waiting workers``() =
        use finished = new ManualResetEventSlim()
        let errors =
            Assert.Throws<AggregateException>(fun () ->
                runConcurrentlyWithBarrier
                    "failed preparation"
                    [
                        (fun _ -> failwith "prepare failed")
                        (fun release ->
                            try release ()
                            finally finished.Set())
                    ]
                |> ignore)
        Assert.True(finished.IsSet, "The waiting worker must terminate before the helper returns")
        Assert.True(errors.InnerExceptions |> Seq.exists (fun error -> error.Message = "prepare failed"))

type CompilerTaskKind =
    | Compiler
    | Interactive

type FscFsiMultiThreadedTaskTests() =

    static let environmentWithCompilerBin () =
        let projectDirectory = TestFramework.createTemporaryDirectory().FullName
        let relativeBin = "compilerBin"
        let variables = dict [ "FSHARP_COMPILER_BIN", relativeBin ]
        let environment = TaskEnvironment.CreateWithProjectDirectoryAndEnvironment(projectDirectory, variables)
        environment, Path.Combine(projectDirectory, relativeBin)

    let toolPath kind environment =
        match kind with
        | Compiler ->
            let task = Fsc() |> assignTaskEnvironment environment
            "fsc.exe", task.InternalGenerateFullPathToTool()
        | Interactive ->
            let task = Fsi() |> assignTaskEnvironment environment
            "fsi.exe", task.InternalGenerateFullPathToTool()

    static member CompilerPairs =
        [ for first, second in [ Compiler, Interactive; Compiler, Compiler ] -> [| box first; box second |] ]

    [<Theory>]
    [<MemberData(nameof FscFsiMultiThreadedTaskTests.CompilerPairs)>]
    member _.``compiler tasks resolve isolated compiler-bin environments``(first: CompilerTaskKind, second: CompilerTaskKind) =
        withTaskEnvironmentPairUsing environmentWithCompilerBin (fun environmentA binA environmentB binB ->
            let executableA, pathA = toolPath first environmentA
            let executableB, pathB = toolPath second environmentB
            Assert.Equal(Path.Combine(binA, executableA), pathA)
            Assert.Equal(Path.Combine(binB, executableB), pathB))

    [<Fact>]
    member _.``concurrent Fsc tasks route flags and sources to their own host objects``() =
        let makeTask (flag: string) (sourceNames: string list) =
            let host = FauxHostObject()

            let task =
                Fsc(
                    BuildEngine = MockEngine(),
                    OtherFlags = flag,
                    Sources = [| for name in sourceNames -> TaskItem name :> ITaskItem |],
                    HostObject = host
                )

            task, host

        let firstTask, firstHost = makeTask "--firstflag" [ "first1.fs"; "first2.fs" ]
        let secondTask, secondHost = makeTask "--secondflag" [ "second1.fs" ]

        let run (task: Fsc) release =
            task.InternalGenerateResponseFileCommands() |> ignore
            release ()
            Assert.Equal(0, task.InternalExecuteTool("", "", ""))

        runConcurrentlyWithBarrier "concurrent Fsc host objects" [ run firstTask; run secondTask ]
        |> ignore

        Assert.Equal<string[]>([| "first1.fs"; "first2.fs" |], firstHost.Sources)
        Assert.Equal<string[]>([| "second1.fs" |], secondHost.Sources)

        for own, other, host in
            [
                "--firstflag", "--secondflag", firstHost
                "--secondflag", "--firstflag", secondHost
            ] do
            Assert.Contains(own, host.Flags)
            Assert.DoesNotContain(other, host.Flags)

    [<Theory>]
    [<InlineData("fsc.exe")>]
    [<InlineData("fsi.exe")>]
    member _.``tool-path normalization preserves every path shape``(executable: string) =
        withTaskEnvironment (fun environment directory ->
            let normalize path = TaskEnvironmentPaths.normalizePathToTool environment path
            let relative = Path.Combine("tools", executable)
            let absolute = Path.Combine(directory.FullName, relative)
            Assert.Equal(absolute, normalize relative)
            Assert.Equal(absolute, normalize absolute)
            Assert.Equal(executable, normalize executable)
            Assert.Null(normalize null)
            Assert.Equal("", normalize "")
            Assert.Equal("   ", normalize "   ")
            if RuntimeInformation.IsOSPlatform OSPlatform.Windows then
                for input in [ $@"\tools\{executable}"; $@"C:tools\{executable}" ] do
                    Assert.Equal(environment.GetAbsolutePath(input).Value, normalize input))

    static member HostCallbackCases =
        [ for kind in [ Compiler; Interactive ] do
            for exitCode in [ 0; 7 ] do
                yield [| box kind; box exitCode |] ]

    [<Theory>]
    [<MemberData(nameof FscFsiMultiThreadedTaskTests.HostCallbackCases)>]
    member _.``host callbacks execute relative tools in isolated task environments``(kind: CompilerTaskKind, exitCode: int) =
        withTaskEnvironmentPairUsing createTaskEnvironmentInTemporaryDirectory (fun environmentA directoryA environmentB directoryB ->
            let run (environment: TaskEnvironment) (directory: DirectoryInfo) marker release =
                let engine = MockEngine()
                let toolPaths = ResizeArray<string>()
                let task: ToolTask =
                    match kind with
                    | Compiler ->
                        { new Fsc() with
                            override _.GenerateCommandLineCommands() = "fsi --exec probe.fsx"
                            override _.GenerateResponseFileCommands() = ""
                            override _.GetProcessStartInfo(pathToTool, commands, responseFileSwitch) =
                                toolPaths.Add pathToTool
                                base.GetProcessStartInfo(pathToTool, commands, responseFileSwitch) }
                    | Interactive ->
                        { new Fsi() with
                            override _.GenerateCommandLineCommands() = "fsi --exec probe.fsx"
                            override _.GenerateResponseFileCommands() = ""
                            override _.GetProcessStartInfo(pathToTool, commands, responseFileSwitch) =
                                toolPaths.Add pathToTool
                                base.GetProcessStartInfo(pathToTool, commands, responseFileSwitch) }

                let toolDirectory = Path.GetFullPath(Path.Combine(TestFramework.repoRoot, ".dotnet")) + string Path.DirectorySeparatorChar
                let projectDirectory = Uri(directory.FullName + string Path.DirectorySeparatorChar)
                task.ToolPath <- projectDirectory.MakeRelativeUri(Uri(toolDirectory)).ToString() |> Uri.UnescapeDataString
                task.ToolExe <- if RuntimeInformation.IsOSPlatform OSPlatform.Windows then "dotnet.exe" else "dotnet"
                task.BuildEngine <- engine
                task.HostObject <- FauxHostObject(executeCompiler = true)
                task.Timeout <- 20000
                assignTaskEnvironment environment task |> ignore
                environment.SetEnvironmentVariable("FSHARP_MT_CALLBACK", marker)
                File.WriteAllText(Path.Combine(directory.FullName, "input.txt"), marker)
                File.WriteAllText(
                    Path.Combine(directory.FullName, "probe.fsx"),
                    String.concat "\n" [
                        "open System"
                        "open System.IO"
                        """printfn "CALLBACK=%s|%s" (Environment.GetEnvironmentVariable "FSHARP_MT_CALLBACK") (File.ReadAllText "input.txt")"""
                        """File.WriteAllText("result.txt", Environment.CurrentDirectory)"""
                        $"exit {exitCode}"
                    ])
                Assert.False(Path.IsPathRooted task.ToolPath)
                Assert.NotEqual<string>(Environment.CurrentDirectory, directory.FullName)
                release ()
                Assert.Equal((exitCode = 0), task.Execute())
                Assert.Equal(exitCode, task.ExitCode)
                Assert.Equal(environment.GetAbsolutePath(Path.Combine(task.ToolPath, task.ToolExe)).Value, Assert.Single(toolPaths))
                Assert.Equal(directory.FullName, File.ReadAllText(Path.Combine(directory.FullName, "result.txt")))
                Assert.Contains(engine.Messages, fun message -> message.Message.Contains($"CALLBACK={marker}|{marker}"))
                if exitCode = 0 then Assert.Empty(engine.Errors)
                else Assert.Single(engine.Errors) |> ignore

            runConcurrentlyWithBarrier "compiler host callbacks" [ run environmentA directoryA "first"; run environmentB directoryB "second" ]
            |> ignore)
