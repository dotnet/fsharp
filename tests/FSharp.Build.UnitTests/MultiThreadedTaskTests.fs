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
    let normalizePathToTool task path = invoke<string> task "NormalizePathToTool" [| box path |]

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

/// One compiler task under test, named so scenarios stay self-describing instead of
/// destructuring anonymous tuples.
type private CompilerTaskCase =
    { Name: string
      Executable: string
      Create: TaskEnvironment -> obj }

type FscFsiMultiThreadedTaskTests() =

    static let environmentWithCompilerBin () =
        let projectDirectory = TestFramework.createTemporaryDirectory().FullName
        let compilerBin = TestFramework.createTemporaryDirectory().FullName
        let variables = dict [ "FSHARP_COMPILER_BIN", compilerBin ]
        TaskEnvironment.CreateWithProjectDirectoryAndEnvironment(projectDirectory, variables), compilerBin

    let fscTask =
        { Name = "Fsc"
          Executable = "fsc.exe"
          Create = fun environment -> Fsc() |> assignTaskEnvironment environment |> box }

    let fsiTask =
        { Name = "Fsi"
          Executable = "fsi.exe"
          Create = fun environment -> Fsi() |> assignTaskEnvironment environment |> box }

    let compilerTasks = [ fscTask; fsiTask ]

    [<Fact>]
    member _.``compiler tasks resolve tool paths from isolated compiler-bin environments``() =
        for caseA, caseB in [ fscTask, fsiTask; fscTask, fscTask ] do
            withTaskEnvironmentPairUsing environmentWithCompilerBin (fun environmentA binA environmentB binB ->
                let scenario = $"{caseA.Name}/{caseB.Name}"
                let taskA, taskB = caseA.Create environmentA, caseB.Create environmentB
                let pathA = FscFsiTestHooks.fullPathToTool taskA
                let pathB = FscFsiTestHooks.fullPathToTool taskB

                Assert.True(Path.IsPathRooted pathA, $"{scenario}: expected rooted path, got '{pathA}'")
                Assert.True(Path.IsPathRooted pathB, $"{scenario}: expected rooted path, got '{pathB}'")
                Assert.Equal(caseA.Executable, Path.GetFileName pathA)
                Assert.Equal(caseB.Executable, Path.GetFileName pathB)
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
                    Sources = [| for name in sourceNames -> TaskItem name :> ITaskItem |],
                    HostObject = host
                )

            task, host

        let firstTask, firstHost = makeTask "--firstflag" [ "first1.fs"; "first2.fs" ]
        let secondTask, secondHost = makeTask "--secondflag" [ "second1.fs" ]

        let run task release =
            // Response-file preparation stays single-threaded; the barrier then releases both executions together.
            FscFsiTestHooks.generateResponseFileCommands task |> ignore
            release ()
            FscFsiTestHooks.executeTool task |> ignore

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

    [<Fact>]
    member _.``compiler tasks normalize every tool-path shape against TaskEnvironment``() =
        withTaskEnvironment (fun environment directory ->
            for case in compilerTasks do
                let task = case.Create environment
                let relative = Path.Combine("tools", case.Executable)
                let normalized = FscFsiTestHooks.normalizePathToTool task relative

                Assert.True(Path.IsPathRooted normalized, $"{case.Name}: expected rooted path, got '{normalized}'")
                Assert.Equal(Path.Combine(directory.FullName, relative), normalized)
                Assert.Equal(case.Executable, FscFsiTestHooks.normalizePathToTool task case.Executable)

                // Empty and whitespace tool names stay bare (never rooted): the host resolves them via PATH,
                // and guarding before Path.GetDirectoryName avoids its net472 throw-on-whitespace behaviour.
                Assert.Equal("", FscFsiTestHooks.normalizePathToTool task "")
                Assert.Equal("   ", FscFsiTestHooks.normalizePathToTool task "   ")

                if RuntimeInformation.IsOSPlatform OSPlatform.Windows then
                    for input in [ $@"\tools\{case.Executable}"; $@"C:tools\{case.Executable}" ] do
                        Assert.Equal(
                            environment.GetAbsolutePath(input).Value,
                            FscFsiTestHooks.normalizePathToTool task input
                        ))
