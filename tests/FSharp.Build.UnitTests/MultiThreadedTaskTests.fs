// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Build.UnitTests

open System
open System.Collections.Generic
open System.IO
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks
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

module private FscFsiTestHooks =

    let private invoke<'T> (task: obj) name args =
        (FSharp.Test.ReflectionHelper.getPrivateInstanceMethod name (task.GetType())).Invoke(task, args)
        :?> 'T

    let fullPathToTool task = invoke<string> task "InternalGenerateFullPathToTool" [||]
    let generateResponseFileCommands task = invoke<string> task "InternalGenerateResponseFileCommands" [||]
    let executeTool task = invoke<int> task "InternalExecuteTool" [| box ""; box ""; box "" |]
    let normalizePathToTool task path = invoke<string> task "InternalNormalizePathToTool" [| box path |]

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
    member _.``task is marked directly multithreadable`` (taskType: Type) =
        let attributes = taskType.GetCustomAttributes(typeof<MSBuildMultiThreadableTaskAttribute>, false)
        Assert.True(attributes.Length = 1, $"{taskType.Name}: expected one direct multithreadable attribute")

type FscFsiMultiThreadedTaskTests() =

    static let environmentWithCompilerBin () =
        let projectDirectory = TestFramework.createTemporaryDirectory().FullName
        let compilerBin = TestFramework.createTemporaryDirectory().FullName
        let variables = Dictionary<string, string>()
        variables["FSHARP_COMPILER_BIN"] <- compilerBin
        TaskEnvironment.CreateWithProjectDirectoryAndEnvironment(projectDirectory, variables), compilerBin

    let compilerTasks =
        [
            "Fsc", "fsc.exe", (fun environment -> box (Fsc(environment)))
            "Fsi", "fsi.exe", (fun environment -> box (Fsi(environment)))
        ]

    [<Fact>]
    member _.``compiler tasks resolve tool paths from isolated compiler-bin environments``() =
        let taskPairs =
            [
                "Fsc/Fsi",
                "fsc.exe",
                "fsi.exe",
                (fun environmentA environmentB -> box (Fsc(environmentA)), box (Fsi(environmentB)))
                "Fsc/Fsc",
                "fsc.exe",
                "fsc.exe",
                (fun environmentA environmentB -> box (Fsc(environmentA)), box (Fsc(environmentB)))
            ]

        for scenario, executableA, executableB, create in taskPairs do
            withTaskEnvironmentPairUsing environmentWithCompilerBin (fun environmentA binA environmentB binB ->
                let taskA, taskB = create environmentA environmentB
                let pathA = FscFsiTestHooks.fullPathToTool taskA
                let pathB = FscFsiTestHooks.fullPathToTool taskB

                Assert.True(Path.IsPathRooted pathA, $"{scenario}: expected rooted path, got '{pathA}'")
                Assert.True(Path.IsPathRooted pathB, $"{scenario}: expected rooted path, got '{pathB}'")
                Assert.Equal(executableA, Path.GetFileName pathA)
                Assert.Equal(executableB, Path.GetFileName pathB)
                Assert.Equal(Path.GetFullPath binA, Path.GetDirectoryName pathA)
                Assert.Equal(Path.GetFullPath binB, Path.GetDirectoryName pathB)
                Assert.NotEqual<string>(pathA, pathB))

    [<Fact>]
    member _.``concurrent Fsc tasks route flags and sources to their own host objects``() =
        let makeTask (flag: string) (sourceNames: string list) =
            let host = FauxHostObject()

            let task =
                Fsc(
                    BuildEngine = MockEngine(),
                    OtherFlags = flag,
                    Sources = (sourceNames |> List.map (TaskItem >> fun item -> item :> ITaskItem) |> List.toArray),
                    HostObject = host
                )

            task, host

        let firstTask, firstHost = makeTask "--firstflag" [ "first1.fs"; "first2.fs" ]
        let secondTask, secondHost = makeTask "--secondflag" [ "second1.fs" ]
        use barrier = new Barrier(2)

        let run task =
            Task.Run(fun () ->
                FscFsiTestHooks.generateResponseFileCommands task |> ignore
                barrier.SignalAndWait()
                FscFsiTestHooks.executeTool task |> ignore)

        Task.WaitAll(run firstTask, run secondTask)
        Assert.Equal<string[]>([| "first1.fs"; "first2.fs" |], firstHost.Sources)
        Assert.Equal<string[]>([| "second1.fs" |], secondHost.Sources)

        for own, other, host in
            [
                "--firstflag", "--secondflag", firstHost
                "--secondflag", "--firstflag", secondHost
            ] do
            Assert.Contains(own, host.Flags)
            Assert.DoesNotContain(other, host.Flags)

    [<Fact>]
    member _.``compiler tasks normalize every tool-path shape against TaskEnvironment``() =
        withTaskEnvironment (fun environment directory ->
            for name, executable, create in compilerTasks do
                let task = create environment
                let relative = Path.Combine("tools", executable)
                let normalized = FscFsiTestHooks.normalizePathToTool task relative

                Assert.True(Path.IsPathRooted normalized, $"{name}: expected rooted path, got '{normalized}'")
                Assert.Equal(Path.Combine(directory.FullName, relative), normalized)
                Assert.Equal(executable, FscFsiTestHooks.normalizePathToTool task executable)

                if RuntimeInformation.IsOSPlatform OSPlatform.Windows then
                    for input in [ $@"\tools\{executable}"; $@"C:tools\{executable}" ] do
                        Assert.Equal(
                            environment.GetAbsolutePath(input).Value,
                            FscFsiTestHooks.normalizePathToTool task input
                        ))
