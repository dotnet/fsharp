// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

module Language.RuntimeAsyncCoreBuildersTests

open Xunit
open FSharp.Test.Compiler

#if NETCOREAPP
[<Fact>]
let ``Core builders coexist with third party values and classic extensions`` () =
    let library =
        FSharp """
namespace ThirdParty
module Extensions =
    type global.Microsoft.FSharp.Control.TaskBuilder with
        member _.ClassicExtension = 42
module Builders =
    let task = global.Microsoft.FSharp.Control.TaskBuilder.task
    let taskSeq = global.Microsoft.FSharp.Control.RuntimeAsync.TaskSeqBuilder()
"""
        |> asLibrary
        |> withFSharpCoreShippedNet
    FSharp """
module Consumer
open Microsoft.FSharp.Control.RuntimeAsync
open ThirdParty.Extensions
let nativeTask = task
let nativeSequence = taskSeq
open ThirdParty.Builders
[<EntryPoint>]
let main _ =
    if nativeTask.GetType() <> typeof<TaskBuilder> then failwith "Native task owner"
    if obj.ReferenceEquals(nativeSequence, taskSeq) then failwith "Later open"
    if task.ClassicExtension <> 42 then failwith "Classic extension"
    if (task { return 42 }).Result <> 42 || (nativeTask { return 42 }).Result <> 42 then failwith "Task result"
    let iterator = (taskSeq { yield 42 }).GetAsyncEnumerator()
    if not (iterator.MoveNextAsync().Result) || iterator.Current <> 42 then failwith "Sequence result"
    iterator.DisposeAsync().GetAwaiter().GetResult()
    0
"""
    |> withReferences [library]
    |> withLangVersionPreview
    |> withFSharpCoreShippedNet
    |> withOptions ["--features:runtime-async=on"]
    |> compileExeAndRun
    |> shouldSucceed

[<Theory>]
[<InlineData(false, false)>]
[<InlineData(false, true)>]
[<InlineData(true, false)>]
[<InlineData(true, true)>]
let ``Core task preserves stateful conditional fold callbacks`` (enabled: bool, optimized: bool) =
    for list in [false; true] do
        FSharp """
module Consumer
open System.Collections.Generic
open System.Threading.Tasks
let work deduplicate (ready: Task<int>) = task {
    let! initial = ready
    return
#if LIST
        List.fold
#else
        Array.fold
#endif
            (if deduplicate then
                 let seen = HashSet<int>()
                 fun total item -> if seen.Add item then total + item else total
             else fun total item -> total + item)
            initial
#if LIST
            [1; 1; 2]
#else
            [|1; 1; 2|]
#endif
}
[<EntryPoint>]
let main _ =
    for deduplicate in [true; false] do
        let gate = TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously)
        let result = work deduplicate gate.Task
        if result.IsCompleted then failwith "Expected suspension"
        gate.SetResult 0
        let expected = if deduplicate then 3 else 4
        let actual = result.GetAwaiter().GetResult()
        if actual <> expected then failwith $"Expected {expected}, actual {actual}"
    0
"""
        |> withLangVersionPreview
        |> withFSharpCoreShippedNet
        |> withOptimization optimized
        |> withOptions [if enabled then "--features:runtime-async=on" else "--features:runtime-async=off"
                        if list then "--define:LIST"]
        |> compileExeAndRun
        |> shouldSucceed
        |> ignore

[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``Core task selects real runtime managed code only when enabled`` enabled =
    let result =
        FSharp """
module Consumer
open System.Threading.Tasks
let work () = task {
    let! value = Task.FromResult 40
    return value + 2
}
[<EntryPoint>]
let main _ =
    if work().GetAwaiter().GetResult() <> 42 then failwith "Task result"
    0
"""
        |> withLangVersionPreview
        |> withFSharpCoreShippedNet
        |> withOptions [if enabled then "--features:runtime-async=on" else "--features:runtime-async=off"]
        |> compileExeAndRun
        |> shouldSucceed
    result
    |> withMetadataReader (fun metadata ->
        let method =
            metadata.MethodDefinitions
            |> Seq.map metadata.GetMethodDefinition
            |> Seq.filter (fun method -> metadata.GetString method.Name = "work")
            |> Assert.Single
        Assert.Equal((if enabled then 0x2000 else 0), int method.ImplAttributes &&& 0x2000))

[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``Core taskSeq supports custom awaitables inside runtime methods`` optimized =
    FSharp """
module Consumer
open System.Threading.Tasks
open Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers
type Input() =
    member _.GetAwaiter() = ValueTask<int>(42).GetAwaiter()
let work () = __runtimeAsyncReturn (
    let source = taskSeq { let! value = Input() in yield value }
    let iterator = source.GetAsyncEnumerator()
    if System.Runtime.CompilerServices.AsyncHelpers.Await(iterator.MoveNextAsync()) then iterator.Current else 0)
"""
    |> asLibrary
    |> withLangVersionPreview
    |> withFSharpCoreShippedNet
    |> withOptimization optimized
    |> withOptions ["--features:runtime-async=on"]
    |> compile
    |> shouldSucceed
#endif
