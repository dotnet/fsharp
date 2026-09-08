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

/// One compiler task under test, kept as a named record so scenarios read as fields instead of
/// destructured anonymous tuples.
type private CompilerTaskCase =
    { Executable: string
      Create: TaskEnvironment -> obj }

type FscFsiMultiThreadedTaskTests() =

    static let environmentWithCompilerBin () =
        let projectDirectory = TestFramework.createTemporaryDirectory().FullName
        // A relative FSHARP_COMPILER_BIN makes the "normalize tool path against the injected TaskEnvironment"
        // step observable: the resolved tool path must be rooted under this task's project directory (not the
        // host process current directory). The returned second element is that expected rooted bin directory.
        let relativeBin = "compilerBin"
        let variables = dict [ "FSHARP_COMPILER_BIN", relativeBin ]
        let environment = TaskEnvironment.CreateWithProjectDirectoryAndEnvironment(projectDirectory, variables)
        environment, Path.Combine(projectDirectory, relativeBin)

    let fscTask =
        { Executable = "fsc.exe"
          Create = fun environment -> Fsc() |> assignTaskEnvironment environment |> box }

    let fsiTask =
        { Executable = "fsi.exe"
          Create = fun environment -> Fsi() |> assignTaskEnvironment environment |> box }

    let compilerTasks = [ fscTask; fsiTask ]

    [<Fact>]
    member _.``compiler tasks resolve tool paths from isolated compiler-bin environments``() =
        for caseA, caseB in [ fscTask, fsiTask; fscTask, fscTask ] do
            withTaskEnvironmentPairUsing environmentWithCompilerBin (fun environmentA binA environmentB binB ->
                let taskA, taskB = caseA.Create environmentA, caseB.Create environmentB
                let pathA = FscFsiTestHooks.fullPathToTool taskA
                let pathB = FscFsiTestHooks.fullPathToTool taskB

                // Exact directory + filename oracles pin each rooted path to its own compiler-bin env, which
                // already implies both are rooted and distinct (binA and binB are separate temp directories).
                Assert.Equal(caseA.Executable, Path.GetFileName pathA)
                Assert.Equal(caseB.Executable, Path.GetFileName pathB)
                Assert.Equal(Path.GetFullPath binA, Path.GetDirectoryName pathA)
                Assert.Equal(Path.GetFullPath binB, Path.GetDirectoryName pathB))

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
            // Fsc/Fsi route their tool paths through this shared helper (covered end-to-end by the full-path
            // and ExecuteTool tests); here we exercise the helper directly across every path shape.
            let normalize path = TaskEnvironmentPaths.normalizePathToTool environment path

            for case in compilerTasks do
                let relative = Path.Combine("tools", case.Executable)

                Assert.Equal(Path.Combine(directory.FullName, relative), normalize relative)
                Assert.Equal(case.Executable, normalize case.Executable)

                // Empty and whitespace tool names stay bare (never rooted): the host resolves them via PATH,
                // and guarding before Path.GetDirectoryName avoids its net472 throw-on-whitespace behaviour.
                Assert.Equal("", normalize "")
                Assert.Equal("   ", normalize "   ")

                if RuntimeInformation.IsOSPlatform OSPlatform.Windows then
                    for input in [ $@"\tools\{case.Executable}"; $@"C:tools\{case.Executable}" ] do
                        Assert.Equal(environment.GetAbsolutePath(input).Value, normalize input))
