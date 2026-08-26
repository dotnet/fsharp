module Language.RuntimeAsyncTests

open Xunit
open FSharp.Test.Compiler
open System.IO

let private runtimeAsyncSource = """
module RuntimeAsyncTest

open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices

let add (x: int) (y: int) : Task<int> =
    StateMachineHelpers.__runtimeAsyncReturn (
        AsyncHelpers.Await(Task.Delay(1))
        x + y)

let rawBody () : Task<int> =
    StateMachineHelpers.__runtimeAsyncReturn 1

type Calculator() =
    member _.Add(x: int, y: int) : Task<int> =
        StateMachineHelpers.__runtimeAsyncReturn (
            AsyncHelpers.Await(Task.Delay(1))
            x + y)

    member _.AddRaw(x: int) : Task<int> =
        StateMachineHelpers.__runtimeAsyncReturn (x + 1)
"""

let private runtimeAsyncRawSource = """
module RuntimeAsyncRawTest

open System.Threading.Tasks
open Microsoft.FSharp.Core.CompilerServices
open System.Runtime.CompilerServices

type RuntimeTaskBuilder() =
    member inline _.Delay([<InlineIfLambda>] generator: unit -> 'T) =
        generator

    member inline _.Run([<InlineIfLambda>] code: unit -> 'T) : Task<'T> =
        StateMachineHelpers.__runtimeAsyncReturn (code())

    member inline _.Zero() = ()

    member inline _.Return(value: 'T) = value

    member inline _.Bind(task: Task, [<InlineIfLambda>] continuation: unit -> 'U) =
        AsyncHelpers.Await task
        continuation()

    member inline _.Combine(
        [<InlineIfLambda>] first: unit -> unit,
        [<InlineIfLambda>] second: unit -> 'T
    ) =
        first()
        second()

[<AutoOpen>]
module RuntimeTask =
    let runtimeTask = RuntimeTaskBuilder()

type ICalculator =
    abstract Combined: unit -> Task<int>

type Calculator() =
    member _.Combined() : Task<int> =
        runtimeTask {
            do! Task.Delay(1)
            do! Task.Delay(1)
            return 42
        }

    interface ICalculator with
        member this.Combined() = this.Combined()

"""

let private runtimeAsyncNestedInlineSource = """
module RuntimeAsyncNestedInlineTest

open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices

type InlineAwait =
    static member inline Await1(task: Task<int>) = AsyncHelpers.Await task
    static member inline Await2(task: Task<int>) = InlineAwait.Await1 task
    static member inline AddOne(value: int) = value + 1
    static member inline Await3(task: Task<int>) = InlineAwait.AddOne (InlineAwait.Await2 task)

let f (task: Task<int>) : Task<int> =
    StateMachineHelpers.__runtimeAsyncReturn (InlineAwait.Await3 task)
"""

#if NETCOREAPP
[<Fact>]
let ``runtime async requires preview language version`` () =
    FSharp """
module RuntimeAsyncPreviewTest

open System.Threading.Tasks
open Microsoft.FSharp.Core.CompilerServices

let f : Task<int> =
    StateMachineHelpers.__runtimeAsyncReturn 1
"""
    |> withFSharpCoreShippedNet
    |> compile
    |> shouldFail
    |> withErrorCode 3350

[<Fact>]
let ``runtime async suspension outside runtime async is rejected`` () =
    FSharp """
module RuntimeAsyncSuspensionContextTest

open System.Threading.Tasks
open System.Runtime.CompilerServices

let f () =
    AsyncHelpers.Await(Task.Delay(1))
    AsyncHelpers.AwaitAwaiter(Task.Delay(1).GetAwaiter())
    AsyncHelpers.UnsafeAwaitAwaiter(Task.Delay(1).GetAwaiter())
"""
    |> withLangVersionPreview
    |> withFSharpCoreShippedNet
    |> compile
    |> shouldFail
    |> withErrorCodes [ 3916; 3916; 3916 ]

[<Fact>]
let ``runtime async rejects non Task result carriers`` () =
    FSharp """
module RuntimeAsyncCarrierTest

open Microsoft.FSharp.Core.CompilerServices

let f : string =
    StateMachineHelpers.__runtimeAsyncReturn "result"
"""
    |> withLangVersionPreview
    |> withFSharpCoreShippedNet
    |> compile
    |> shouldFail
    |> withErrorCode 1

[<Fact>]
let ``runtime async intrinsic does not capture user-defined same-named values`` () =
    FSharp """
let __runtimeAsyncReturn value = value
let result = __runtimeAsyncReturn 1
"""
    |> typecheck
    |> shouldSucceed

[<Fact>]
let ``runtime async compiles functions and members`` () =
    FSharp runtimeAsyncSource
    |> withLangVersionPreview
    |> withFSharpCoreShippedNet
    |> compile
    |> shouldSucceed

[<Fact>]
let ``runtime async supports Task and ValueTask return intrinsics`` () =
    FSharp """
module RuntimeAsyncReturnShapesTest

open System.Threading.Tasks
open Microsoft.FSharp.Core.CompilerServices

let taskResult () : Task<int> =
    StateMachineHelpers.__runtimeAsyncReturn 1

let valueTaskResult () : ValueTask<int> =
    StateMachineHelpers.__runtimeAsyncReturnValueTask 1

let taskUnit () : Task =
    StateMachineHelpers.__runtimeAsyncReturnUnit ()

let valueTaskUnit () : ValueTask =
    StateMachineHelpers.__runtimeAsyncReturnValueTaskUnit ()

[<EntryPoint>]
let main _ =
    taskUnit().Wait()
    taskResult().Result |> ignore
    valueTaskResult().Result |> ignore
    valueTaskUnit().AsTask().Wait()
    0
"""
    |> withLangVersionPreview
    |> withFSharpCoreShippedNet
    |> compileExeAndRun
    |> shouldSucceed

[<Fact>]
let ``runtime async combines awaited chunks without delegates`` () =
    FSharp runtimeAsyncRawSource
    |> withLangVersionPreview
    |> withFSharpCoreShippedNet
    |> compile
    |> verifyILContains [
        "Task::Delay(int32)"
        "AsyncHelpers::Await(class [runtime]System.Threading.Tasks.Task)"
    ]
    |> shouldSucceed

[<Fact>]
let ``runtime async specializes nested inline suspensions without optimization`` () =
    FSharp runtimeAsyncNestedInlineSource
    |> withLangVersionPreview
    |> withFSharpCoreShippedNet
    |> withNoOptimize
    |> compile
    |> verifyILContains [ "AsyncHelpers::Await<int32>(class [runtime]System.Threading.Tasks.Task`1<!!0>)" ]

[<InlineData(false)>]
[<InlineData(true)>]
[<Theory>]
let ``runtime task builder fixture executes through runtime async`` (optimize: bool) =
    FsFromPath (Path.Combine(__SOURCE_DIRECTORY__, "RuntimeAsync", "RuntimeTaskBuilder.fs"))
    |> withAdditionalSourceFile (
        SourceFromPath (Path.Combine(__SOURCE_DIRECTORY__, "RuntimeAsync", "RuntimeTasks.fs"))
    )
    |> withLangVersionPreview
    |> withFSharpCoreShippedNet
    |> withOptimization optimize
    |> compileExeAndRun
    |> shouldSucceed

[<Fact>]
let ``runtime async direct intrinsic fixture executes`` () =
    Path.Combine(__SOURCE_DIRECTORY__, "RuntimeAsync", "RuntimeAsyncBasic.fs")
    |> FsFromPath
    |> withLangVersionPreview
    |> withFSharpCoreShippedNet
    |> compileExeAndRun
    |> shouldSucceed

[<Fact>]
let ``runtime async low level async enumerable fixture executes`` () =
    Path.Combine(__SOURCE_DIRECTORY__, "RuntimeAsync", "RuntimeAsyncEnumerable.fs")
    |> FsFromPath
    |> withLangVersionPreview
    |> withFSharpCoreShippedNet
    |> compileExeAndRun
    |> shouldSucceed

[<Fact>]
let ``runtime async suspension in exception region executes`` () =
    Path.Combine(__SOURCE_DIRECTORY__, "RuntimeAsync", "RuntimeTasksAsyncDisposalException.fs")
    |> FsFromPath
    |> withLangVersionPreview
    |> withFSharpCoreShippedNet
    |> compileExeAndRun
    |> shouldSucceed

#else
[<Fact>]
let ``runtime async intrinsic is only available in the shipped net FSharp.Core`` () =
    FSharp """
open System.Threading.Tasks
open Microsoft.FSharp.Core.CompilerServices

let f : Task<int> =
    StateMachineHelpers.__runtimeAsyncReturn 1
"""
    |> typecheck
    |> shouldFail
    |> withErrorCode 39
#endif
