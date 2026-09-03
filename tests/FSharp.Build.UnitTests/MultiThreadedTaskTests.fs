// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Build.UnitTests

open System
open System.IO
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks
open Microsoft.Build.Framework
open Microsoft.Build.Utilities
open FSharp.Build
open Xunit

/// Minimal ITaskHost stand-in mirroring the shape MSBuild passes to Fsc/Fsi via HostObject.
/// Its Compile method is discovered by reflection inside Fsc/Fsi.ExecuteTool and receives the
/// flags and source file names captured for that specific task instance.
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

/// Reflection helpers for the internal test hooks Fsc/Fsi expose (InternalGenerateFullPathToTool,
/// InternalGenerateResponseFileCommands, InternalExecuteTool). FSharp.Build only grants
/// InternalsVisibleTo to VisualFSharp.UnitTests, so this assembly reaches them via reflection.
module private FscFsiTestHooks =

    let private invoke (task: obj) (name: string) (args: obj[]) =
        let m =
            FSharp.Test.ReflectionHelper.getPrivateInstanceMethod name (task.GetType())

        m.Invoke(task, args)

    let fullPathToTool (task: obj) : string =
        invoke task "InternalGenerateFullPathToTool" [||] :?> string

    let generateResponseFileCommands (task: obj) : string =
        invoke task "InternalGenerateResponseFileCommands" [||] :?> string

    let executeTool (task: obj) : int =
        invoke task "InternalExecuteTool" [| box ""; box ""; box "" |] :?> int

    let normalizePathToTool (task: obj) (pathToTool: string) : string =
        invoke task "InternalNormalizePathToTool" [| box pathToTool |] :?> string

/// Verifies that the FSharp.Build tasks which are multithread-safe (their outputs depend only on
/// their inputs and on an injected TaskEnvironment, with no reliance on ambient process state such
/// as the current directory or environment variables) are marked directly with
/// MSBuildMultiThreadableTaskAttribute, opting them into MSBuild's multi-threaded build scheduler
/// instead of the single-threaded task queue.
type MultiThreadedTaskTests() =

    static member MultiThreadableTaskTypes: obj[] seq =
        seq {
            yield [| typeof<CreateFSharpManifestResourceName> |]
            yield [| typeof<MapSourceRoots> |]
            yield [| typeof<WriteCodeFragment> |]
            yield [| typeof<GenerateILLinkSubstitutions> |]
            yield [| typeof<FSharpEmbedResourceText> |]
            yield [| typeof<FSharpEmbedResXSource> |]
            yield [| typeof<SubstituteText> |]
            yield [| typeof<Fsc> |]
            yield [| typeof<Fsi> |]
        }

    [<Theory>]
    [<MemberData(nameof MultiThreadedTaskTests.MultiThreadableTaskTypes)>]
    member _.``task is marked directly multithreadable`` (taskType: Type) =
        let attributes = taskType.GetCustomAttributes(typeof<MSBuildMultiThreadableTaskAttribute>, false)

        Assert.Equal(1, attributes.Length)

/// Verifies that Fsc and Fsi resolve their tool location and capture their per-invocation state
/// against the TaskEnvironment injected into their constructor, so concurrent instances running on
/// the multi-threaded scheduler cannot observe each other's ambient state.
type FscFsiMultiThreadedTaskTests() =

    /// Creates a TaskEnvironment whose FSHARP_COMPILER_BIN points at a fresh temporary directory,
    /// returning the environment and that directory. Callers must dispose the environment.
    static let environmentWithCompilerBin () =
        let projectDirectory = TestFramework.createTemporaryDirectory().FullName
        let compilerBin = TestFramework.createTemporaryDirectory().FullName

        let variables = Dictionary<string, string>()
        variables["FSHARP_COMPILER_BIN"] <- compilerBin

        let environment =
            TaskEnvironment.CreateWithProjectDirectoryAndEnvironment(projectDirectory, variables)

        environment, compilerBin

    /// Creates two compiler-bin TaskEnvironments, runs `body` against both, and disposes them afterwards.
    /// Nested try/finally guarantees the first environment is disposed even if the second fails to construct,
    /// and that both are disposed (second then first) once `body` completes.
    static let withCompilerBinEnvironmentPair body =
        let environmentA, binA = environmentWithCompilerBin ()

        try
            let environmentB, binB = environmentWithCompilerBin ()

            try
                body environmentA binA environmentB binB
            finally
                BuildTaskTestHelpers.disposeTaskEnvironment environmentB
        finally
            BuildTaskTestHelpers.disposeTaskEnvironment environmentA

    [<Fact>]
    member _.``Fsc and Fsi resolve tool path against distinct injected compiler-bin environments``() =
        withCompilerBinEnvironmentPair (fun fscEnvironment fscBin fsiEnvironment fsiBin ->
            // Constructing with the environment must route the eager compiler-bin lookup through it,
            // which requires assigning TaskEnvironment before the defaultToolPath binding runs.
            let fsc = Fsc(fscEnvironment)
            let fsi = Fsi(fsiEnvironment)

            let fscPath = FscFsiTestHooks.fullPathToTool fsc
            let fsiPath = FscFsiTestHooks.fullPathToTool fsi

            // Each task resolves an absolute path to its own tool under its own injected bin directory.
            Assert.True(Path.IsPathRooted fscPath, $"expected rooted path, got '{fscPath}'")
            Assert.True(Path.IsPathRooted fsiPath, $"expected rooted path, got '{fsiPath}'")

            Assert.Equal("fsc.exe", Path.GetFileName fscPath)
            Assert.Equal("fsi.exe", Path.GetFileName fsiPath)

            Assert.Equal(Path.GetFullPath fscBin, Path.GetDirectoryName fscPath)
            Assert.Equal(Path.GetFullPath fsiBin, Path.GetDirectoryName fsiPath)

            // The two injected environments are distinct, so the resolved directories differ.
            Assert.NotEqual<string>(Path.GetDirectoryName fscPath, Path.GetDirectoryName fsiPath))

    [<Fact>]
    member _.``Two Fsc tasks resolve tool paths against their own injected compiler-bin environments``() =
        withCompilerBinEnvironmentPair (fun firstEnvironment firstBin secondEnvironment secondBin ->
            let first = Fsc(firstEnvironment)
            let second = Fsc(secondEnvironment)

            let firstPath = FscFsiTestHooks.fullPathToTool first
            let secondPath = FscFsiTestHooks.fullPathToTool second

            Assert.Equal(Path.GetFullPath firstBin, Path.GetDirectoryName firstPath)
            Assert.Equal(Path.GetFullPath secondBin, Path.GetDirectoryName secondPath)
            Assert.NotEqual<string>(firstPath, secondPath))

    [<Fact>]
    member _.``Two concurrent Fsc tasks route flags and sources to their own host objects``() =
        // Each task gets its own HostObject, OtherFlags and Sources. Running them concurrently must
        // not let one instance's captured arguments/filenames leak into another (no shared state).
        let makeTask (flag: string) (sourceNames: string list) =
            let task = Fsc()
            task.BuildEngine <- MockEngine()
            task.OtherFlags <- flag

            task.Sources <-
                sourceNames
                |> List.map (fun name -> TaskItem(name) :> ITaskItem)
                |> List.toArray

            let host = FauxHostObject()
            task.HostObject <- host
            task, host

        let firstTask, firstHost = makeTask "--firstflag" [ "first1.fs"; "first2.fs" ]
        let secondTask, secondHost = makeTask "--secondflag" [ "second1.fs" ]

        // A barrier maximises the chance both tasks are inside their compile path simultaneously.
        use barrier = new Barrier(2)

        let run (task: Fsc) =
            Task.Run(fun () ->
                // Populate this instance's captured arguments/filenames, mirroring ToolTask.Execute,
                // then hand off to the host object exactly as a real build would.
                FscFsiTestHooks.generateResponseFileCommands task |> ignore
                barrier.SignalAndWait()
                FscFsiTestHooks.executeTool task |> ignore)

        Task.WaitAll(run firstTask, run secondTask)

        Assert.Equal<string[]>([| "first1.fs"; "first2.fs" |], firstHost.Sources)
        Assert.Equal<string[]>([| "second1.fs" |], secondHost.Sources)

        Assert.Contains("--firstflag", firstHost.Flags)
        Assert.DoesNotContain("--secondflag", firstHost.Flags)

        Assert.Contains("--secondflag", secondHost.Flags)
        Assert.DoesNotContain("--firstflag", secondHost.Flags)

    [<Fact>]
    member _.``Fsc roots a relative pathToTool against its injected project directory``() =
        // MSBuild's ToolTask.ComputePathToTool can hand a relative base ToolPath straight to the
        // derived ExecuteTool, and ProcessStartInfo.FileName then resolves it against the host
        // current directory rather than the child WorkingDirectory. The production normalization must
        // root such a relative path (with directory components) against this task's project directory.
        BuildTaskTestHelpers.withTaskEnvironment (fun environment directory ->
            let fsc = Fsc(environment)
            let relative = Path.Combine("tools", "fsc.exe")

            let normalized = FscFsiTestHooks.normalizePathToTool fsc relative

            Assert.True(Path.IsPathRooted normalized, $"expected rooted path, got '{normalized}'")
            Assert.Equal(Path.Combine(directory.FullName, relative), normalized)

            // A bare filename must be left untouched so the OS/ComputePathToTool PATH lookup still works.
            Assert.Equal("fsc.exe", FscFsiTestHooks.normalizePathToTool fsc "fsc.exe"))

    [<Fact>]
    member _.``Fsi roots a relative pathToTool against its injected project directory``() =
        BuildTaskTestHelpers.withTaskEnvironment (fun environment directory ->
            let fsi = Fsi(environment)
            let relative = Path.Combine("tools", "fsi.exe")

            let normalized = FscFsiTestHooks.normalizePathToTool fsi relative

            Assert.True(Path.IsPathRooted normalized, $"expected rooted path, got '{normalized}'")
            Assert.Equal(Path.Combine(directory.FullName, relative), normalized)

            // A bare filename must be left untouched so the OS/ComputePathToTool PATH lookup still works.
            Assert.Equal("fsi.exe", FscFsiTestHooks.normalizePathToTool fsi "fsi.exe"))
