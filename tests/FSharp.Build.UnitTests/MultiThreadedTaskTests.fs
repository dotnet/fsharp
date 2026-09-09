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

type FauxHostObject() =
    let mutable flags: string[] = [||]
    let mutable sources: string[] = [||]

    member _.Compile(_compile: Func<int>, flagsIn: string[], sourcesIn: string[]) =
        flags <- flagsIn
        sources <- sourcesIn
        0

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
