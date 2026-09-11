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
let ``runtime async supports inlining of a lambda`` () =
    FSharp """
module RuntimeAsyncInlineLambdaTest

open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers

let inline makeFragment () =
    fun x ->
        AsyncHelpers.Await (Task.Delay 1000)
        printfn "Hello from async function with input: %d" x

let inline consume([<InlineIfLambda>] f) =
    __runtimeAsyncReturn(f 42)

[<EntryPoint>]
let main _ =
    consume (makeFragment()) |> _.Result |> ignore
    0
"""
    |> withLangVersionPreview
    |> withFSharpCoreShippedNet
    |> compileExeAndRun
    |> shouldSucceed

[<Fact>]
let ``runtime async supports inlining of a multi argument lambda`` () =
    FSharp """
module RuntimeAsyncInlineMultiArgumentLambdaTest

open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers

let inline makeFragment () =
    fun x y ->
        AsyncHelpers.Await (Task.Delay 1)
        x + y

let inline consume([<InlineIfLambda>] f) =
    __runtimeAsyncReturn(f 40 2)

[<EntryPoint>]
let main _ =
    if (consume (makeFragment())).Result <> 42 then 1 else 0
"""
    |> withLangVersionPreview
    |> withFSharpCoreShippedNet
    |> compileExeAndRun
    |> shouldSucceed

[<Fact>]
let ``runtime async fuses suspension in inline returned closures`` () =
    FSharp """
module RuntimeAsyncInlineReturnedClosureTest

open System
open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices

type Code = obj -> unit

type Builder() =
    member inline _.Delay([<InlineIfLambda>] generator: unit -> Code) : Code =
        fun state -> generator() state

    member inline _.Zero() : Code =
        fun _ -> ()

    member inline _.Yield(_: int) : Code =
        fun _ -> ()

    member inline _.Bind(task: Task, [<InlineIfLambda>] continuation: unit -> Code) : Code =
        fun state ->
            AsyncHelpers.Await task
            continuation() state

    member inline _.Combine(first: Code, [<InlineIfLambda>] second: Code) : Code =
        fun state ->
            first state
            second state

    member inline _.Run([<InlineIfLambda>] code: Code) : Task =
        StateMachineHelpers.__runtimeAsyncReturnUnit (code null)

[<EntryPoint>]
let main _ =
    let builder = Builder()
    builder {
        yield 1
        do! Task.Delay(1)
    }
    |> _.Wait()
    0
"""
    |> withLangVersionPreview
    |> withFSharpCoreShippedNet
    |> compileExeAndRun
    |> shouldSucceed

[<Fact>]
let ``runtime async ignores unreachable suspension`` () =
    FSharp """
module RuntimeAsyncUnreachableSuspensionTest

open System.Threading.Tasks
open System.Runtime.CompilerServices

let f () =
    if false then
        AsyncHelpers.Await (Task.Delay 1)

[<EntryPoint>]
let main _ =
    f ()
    0
"""
    |> withLangVersionPreview
    |> withFSharpCoreShippedNet
    |> compileExeAndRun
    |> shouldSucceed

[<InlineData(false)>]
[<InlineData(true)>]
[<Theory>]
let ``runtime async preserves reraise after a suspending handler`` (optimize: bool) =
    FSharp """
module RuntimeAsyncReraiseTest

open System
open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices

let f () : Task<int> =
    StateMachineHelpers.__runtimeAsyncReturn (
        try
            failwith "boom"
            0
        with _ ->
            AsyncHelpers.Await(Task.Delay 1)
            reraise ())

[<EntryPoint>]
let main _ =
    try
        f().GetAwaiter().GetResult() |> ignore
        1
    with
    | e when e.Message = "boom" -> 0
    | _ -> 1
"""
    |> withLangVersionPreview
    |> withFSharpCoreShippedNet
    |> withOptimization optimize
    |> compileExeAndRun
    |> shouldSucceed

[<Fact>]
let ``runtime async rejects stackalloc across suspension`` () =
    FSharp """
module RuntimeAsyncStackallocTest

open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.NativeInterop

let f () : Task<int> =
    StateMachineHelpers.__runtimeAsyncReturn (
        let p = NativePtr.stackalloc<int> 1
        NativePtr.write p 42
        AsyncHelpers.Await(Task.Delay 1)
        NativePtr.read p)
"""
    |> withLangVersionPreview
    |> withFSharpCoreShippedNet
    |> compile
    |> shouldFail
    |> withErrorCode 3918

[<Fact>]
let ``runtime async rejects stackalloc without suspension`` () =
    FSharp """
module RuntimeAsyncStackallocWithoutSuspensionTest

open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.NativeInterop

let f () : Task<int> =
    StateMachineHelpers.__runtimeAsyncReturn (
        let p = NativePtr.stackalloc<int> 1
        NativePtr.write p 42
        NativePtr.read p)
"""
    |> withLangVersionPreview
    |> withFSharpCoreShippedNet
    |> compile
    |> shouldFail
    |> withErrorCode 3918

[<Fact>]
let ``runtime async rejects a byref captured by an inlined closure`` () =
    FSharp """
module RuntimeAsyncByrefClosureTest

open System.Threading.Tasks
open Microsoft.FSharp.Core.CompilerServices

[<NoCompilerInlining>]
let f (x: byref<int>) : Task<int> =
    let y = x
    StateMachineHelpers.__runtimeAsyncReturn (x + y)
"""
    |> withLangVersionPreview
    |> withFSharpCoreShippedNet
    |> compile
    |> shouldFail
    |> withErrorCode 406

[<InlineData(false)>]
[<InlineData(true)>]
[<Theory>]
let ``runtime async does not duplicate effectful InlineIfLambda arguments`` (optimize: bool) =
    FSharp """
module RuntimeAsyncInlineIfLambdaEffectsTest

open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers

let mutable calls = 0

let effect () =
    calls <- calls + 1
    fun () -> 1

let inline plainTwice ([<InlineIfLambda>] f: unit -> int) = f () + f ()
let inline twice ([<InlineIfLambda>] f: unit -> int) =
    __runtimeAsyncReturn (f () + f ())
let inline unused ([<InlineIfLambda>] f: unit -> int) =
    __runtimeAsyncReturn 20

[<EntryPoint>]
let main _ =
    let plainResult = plainTwice (effect ())
    let plainCalls = calls
    calls <- 0
    let twiceResult = (twice (effect ())).GetAwaiter().GetResult()
    let twiceCalls = calls
    calls <- 0
    let unusedResult = (unused (effect ())).GetAwaiter().GetResult()
    let unusedCalls = calls

    if plainResult = 2 && plainCalls = 1
       && twiceResult = 2 && twiceCalls = 1
       && unusedResult = 20 && unusedCalls = 1 then
        0
    else
        1
"""
    |> withLangVersionPreview
    |> withFSharpCoreShippedNet
    |> withOptimization optimize
    |> compileExeAndRun
    |> shouldSucceed

[<Fact>]
let ``runtime async pipe syntax is gated by the language version`` () =
    FSharp """
module RuntimeAsyncPipeGateTest

open System.Threading.Tasks
open Microsoft.FSharp.Core.CompilerServices

let f (x: int) : Task<int> =
    x |> StateMachineHelpers.__runtimeAsyncReturn
"""
    |> withLangVersion90
    |> withFSharpCoreShippedNet
    |> compile
    |> shouldFail
    |> withErrorCode 3350

[<InlineData(false)>]
[<InlineData(true)>]
[<Theory>]
let ``runtime async preserves evaluation order for curried inline applications`` (optimize: bool) =
    FSharp """
module RuntimeAsyncCurriedApplicationTest

open System.Threading.Tasks
open Microsoft.FSharp.Core.CompilerServices

let events = ResizeArray<string>()

let step name value =
    events.Add name
    value

let inline apply f x y = f x y

let f () : Task<int> =
    StateMachineHelpers.__runtimeAsyncReturn (
        apply
            (fun x ->
                events.Add "body"
                fun y -> x + y)
            (step "arg1" 1)
            (step "arg2" 2))

[<EntryPoint>]
let main _ =
    let result = f().GetAwaiter().GetResult()

    if result = 3 && (events |> Seq.toList) = [ "arg1"; "arg2"; "body" ] then
        0
    else
        1
"""
    |> withLangVersionPreview
    |> withFSharpCoreShippedNet
    |> withOptimization optimize
    |> compileExeAndRun
    |> shouldSucceed

[<Fact>]
let ``runtime async rejects synchronized methods`` () =
    FSharp """
module RuntimeAsyncSynchronizedTest

open System
open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices

[<MethodImpl(MethodImplOptions.Synchronized)>]
let f () : Task<int> =
    StateMachineHelpers.__runtimeAsyncReturn (
        AsyncHelpers.Await(Task.Delay(1).ContinueWith(fun _ -> 1)))
"""
    |> withLangVersionPreview
    |> withFSharpCoreShippedNet
    |> compile
    |> shouldFail
    |> withErrorCode 3919

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
let ``runtime task AsyncLocal values propagate through runtime async`` () =
    FsFromPath (Path.Combine(__SOURCE_DIRECTORY__, "RuntimeAsync", "RuntimeTaskBuilder.fs"))
    |> withAdditionalSourceFile (
        SourceFromPath (Path.Combine(__SOURCE_DIRECTORY__, "RuntimeAsync", "RuntimeAsyncAsyncLocal.fs"))
    )
    |> withLangVersionPreview
    |> withFSharpCoreShippedNet
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

[<InlineData(false)>]
[<InlineData(true)>]
[<Theory>]
let ``runtime async low level async enumerable fixture executes`` (optimize: bool) =
    Path.Combine(__SOURCE_DIRECTORY__, "RuntimeAsync", "RuntimeAsyncEnumerableLowLevel.fs")
    |> FsFromPath
    |> withLangVersionPreview
    |> withFSharpCoreShippedNet
    |> withOptimization optimize
    |> compileExeAndRun
    |> shouldSucceed

[<InlineData(false)>]
[<InlineData(true)>]
[<Theory>]
let ``runtime async enumerable builder fixture executes`` (optimize: bool) =
    FsFromPath (Path.Combine(__SOURCE_DIRECTORY__, "RuntimeAsync", "RuntimeTaskBuilder.fs"))
    |> withAdditionalSourceFile (
        SourceFromPath (Path.Combine(__SOURCE_DIRECTORY__, "RuntimeAsync", "RuntimeAsyncEnumerable.fs"))
    )
    |> withAdditionalSourceFile (
        SourceFromPath (Path.Combine(__SOURCE_DIRECTORY__, "RuntimeAsync", "RuntimeAsyncEnumerableTests.fs"))
    )
    |> withLangVersionPreview
    |> withFSharpCoreShippedNet
    |> withOptimization optimize
    |> compileExeAndRun
    |> shouldSucceed

[<Fact>]
let ``runtime async enumerable CE debug points stay at call sites`` () =
    let source =
        """module RuntimeAsyncEnumerableDebug
open System.Threading.Tasks
open RuntimeAsyncEnumerable

let first () =
    asyncSeq {
        do! Task.Delay 1
        yield 1
    }

let second () =
    asyncSeq {
        do! Task.Delay 1
        yield 2
    }
"""

    FsFromPath (Path.Combine(__SOURCE_DIRECTORY__, "RuntimeAsync", "RuntimeTaskBuilder.fs"))
    |> withAdditionalSourceFile (
        SourceFromPath (Path.Combine(__SOURCE_DIRECTORY__, "RuntimeAsync", "RuntimeAsyncEnumerable.fs"))
    )
    |> withAdditionalSourceFile (FsSourceWithFileName "RuntimeAsyncEnumerableDebug.fs" source)
    |> withLangVersionPreview
    |> withFSharpCoreShippedNet
    |> withPortablePdb
    |> withNoOptimize
    |> compile
    |> shouldSucceed
    |> verifyPdb [
        VerifyRuntimeAsyncMethodSequencePointsInSource("RuntimeAsyncEnumerableDebug.fs", 6, 8)
        VerifyRuntimeAsyncMethodSequencePointsInSource("RuntimeAsyncEnumerableDebug.fs", 12, 14)
    ]

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
