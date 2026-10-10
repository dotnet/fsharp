namespace Language

module RuntimeAsyncTests =
    open Xunit
    open FSharp.Test
    open FSharp.Test.Compiler
    open System.IO
    open System.Reflection.Metadata
    open System.Text.RegularExpressions

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

#if NETCOREAPP
    let private nestedTaskSource = """module NestedTask

open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers

let run (ready: Task<int>) : Task<int> =
    __runtimeAsyncReturn (
        let child = task {
            let value = AsyncHelpers.Await ready
            return value + 1
        }
        AsyncHelpers.Await child)
"""

    [<Theory>]
    [<InlineData(false, "direct")>]
    [<InlineData(true, "direct")>]
    [<InlineData(false, "local")>]
    [<InlineData(true, "local")>]
    [<InlineData(false, "imported")>]
    [<InlineData(true, "imported")>]
    [<InlineData(false, "unit")>]
    [<InlineData(true, "unit")>]
    let ``Issue 20576 rejects runtime Await in ordinary task methods`` (optimize: bool, shape: string) =
        let configure = withLangVersionPreview >> withFSharpCoreShippedNet >> withOptimization optimize
        let helper = "let inline awaitValue (ready: Task<int>) = System.Runtime.CompilerServices.AsyncHelpers.Await ready"
        let source, references =
            match shape with
            | "direct" -> nestedTaskSource, []
            | "local" ->
                nestedTaskSource
                    .Replace("let child = task {", $"{helper}\n        let child = task {{")
                    .Replace("let value = AsyncHelpers.Await ready", "let value = awaitValue ready"), []
            | "imported" ->
                let library = FSharp($"module AwaitLibrary\nopen System.Threading.Tasks\n{helper}") |> asLibrary |> withName "AwaitLibrary" |> configure
                library |> compile |> shouldSucceed |> ignore
                nestedTaskSource.Replace("let value = AsyncHelpers.Await ready", "let value = AwaitLibrary.awaitValue ready"), [ library ]
            | "unit" ->
                nestedTaskSource.Replace("Task<int>", "Task<unit>").Replace("let value = AsyncHelpers.Await ready", "AsyncHelpers.Await ready").Replace("return value + 1", "return ()"), []
            | _ -> failwith $"Unexpected shape: {shape}"
        let result =
            FSharp source
            |> asLibrary
            |> configure
            |> withReferences references
            |> compile
        result |> shouldFail |> withErrorCode 3918 |> ignore
        let ranges =
            match optimize, shape with
            // Optimized task-template inlining attributes the call to the inner task keyword.
            | true, "local" -> [10, 21, 25]
            | true, _ -> [9, 21, 25]
            | false, "direct" -> [10, 25, 49]
            | false, "local" -> [9, 52, 108; 11, 25, 41]
            | false, "imported" -> [10, 25, 54]
            | false, "unit" -> [10, 13, 37]
            | _ -> failwith $"Unexpected shape: {shape}"
        Assert.Equal(ranges.Length, result.Output.Diagnostics.Length)
        Assert.Equal(ranges.Length, result.Output.PerFileErrors.Length)
        result
        |> withDiagnostics [
            for line, startCol, endCol in ranges ->
                Error 3918, Line line, Col startCol, Line line, Col endCol,
                "Runtime async suspension method 'Await' may only be called from a runtime async method."
        ]
        |> ignore

    [<Theory>]
    [<InlineData(false)>]
    [<InlineData(true)>]
    let ``Issue 20576 preserves ordinary task composition`` (optimize: bool) =
        let result =
            FSharp """
module TaskComposition
open System
open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers

let child (ready: Task<'T>) : Task<'T> = __runtimeAsyncReturn (AsyncHelpers.Await ready)

let run (before: Task<unit>) (ready: Task<'T>) (after: Task<unit>)
        (enteredReady: TaskCompletionSource<unit>) (enteredAfter: TaskCompletionSource<unit>) marked transform : Task<'U> =
    __runtimeAsyncReturn (
        AsyncHelpers.Await before
        let nested = task {
            enteredReady.SetResult ()
            let! value = if marked then child ready else ready
            return transform value
        }
        let result = AsyncHelpers.Await nested
        enteredAfter.SetResult ()
        AsyncHelpers.Await after
        result)

let gate<'T> () = TaskCompletionSource<'T>(TaskCreationOptions.RunContinuationsAsynchronously)
let wait (work: Task<'T>) = work.WaitAsync(TimeSpan.FromSeconds 30.).GetAwaiter().GetResult()
let pending (work: Task) = if work.IsCompleted then failwith "Expected pending operation"

let check marked input transform expected =
    let before, ready, after = gate<unit>(), gate<_>(), gate<unit>()
    let enteredReady, enteredAfter = gate<unit>(), gate<unit>()
    let work = run before.Task ready.Task after.Task enteredReady enteredAfter marked transform
    pending work
    before.SetResult ()
    wait enteredReady.Task
    pending work
    ready.SetResult input
    wait enteredAfter.Task
    pending work
    after.SetResult ()
    if wait work <> expected then failwith "Unexpected result"

[<EntryPoint>]
let main _ =
    for marked in [false; true] do
        check marked 41 ((+) 1) 42
        check marked "forty" (fun value -> value + "-two") "forty-two"
        let mutable observed = false
        check marked () (fun () -> observed <- true) ()
        if not observed then failwith "Missing unit side effect"
    0
"""
            |> withLangVersionPreview
            |> withFSharpCoreShippedNet
            |> withOptimization optimize
            |> compileExeAndRun
            |> shouldSucceed
        result |> withMetadataReader (fun md ->
            let methods = [ for handle in md.MethodDefinitions -> md.GetMethodDefinition handle ]
            let generatedNames = ["MoveNext"; "SetStateMachine"; "get_ResumptionPoint"; "get_Data"; "set_Data"]
            for name in generatedNames do
                let method = methods |> List.filter (fun method -> md.GetString method.Name = name) |> Assert.Single
                Assert.Equal(0, int method.ImplAttributes &&& 0x2000)
                if name = "MoveNext" then
                    let mutable signature = md.GetBlobReader method.Signature
                    Assert.False(signature.ReadSignatureHeader().IsGeneric)
                    Assert.Equal(0, signature.ReadCompressedInteger())
                    Assert.Equal(System.Reflection.Metadata.SignatureTypeCode.Void, signature.ReadSignatureTypeCode())
            for name in ["child"; "run"] do
                let method = methods |> List.filter (fun method -> md.GetString method.Name = name) |> Assert.Single
                Assert.Equal(0x2000, int method.ImplAttributes &&& 0x2000)
            if optimize then
                let liftedChild =
                    methods |> List.filter (fun method -> md.GetString method.Name = "Invoke" && int method.ImplAttributes &&& 0x2000 <> 0)
                Assert.Single(liftedChild) |> ignore)
        let _, _, il = ILChecker.verifyILAndReturnActual [] result.OutputPath.Value []
        let methodBody name =
            Regex.Match(il, @"(?ms)^(?<indent>[ \t]*)\.method[^{}]*\b" + name + @"(?:<[^>]+>)?\([^{}]*\{.*?^\k<indent>\}").Value
        let moveNext = methodBody "MoveNext"
        Assert.NotEmpty(moveNext)
        Assert.Contains("void", moveNext)
        Assert.DoesNotContain("AsyncHelpers::Await", moveNext)
        if optimize then
            Assert.Contains("AsyncHelpers::Await", methodBody "Invoke")
            Assert.Contains("FSharpFunc`2", moveNext)
        else
            Assert.Contains("TaskComposition::child", moveNext)
        for name in ["child"; "run"] do
            Assert.Contains("AsyncHelpers::Await", methodBody name)

    let private runtimeAsyncCrossAssemblyLibrary = """
module RuntimeAsyncCrossAssemblyLibrary

open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices

let inline getValue () : Task<int> =
    StateMachineHelpers.__runtimeAsyncReturn (
        AsyncHelpers.Await(Task.Delay(1))
        42)
"""

    let private runtimeAsyncCrossAssemblyConsumer = """
module RuntimeAsyncCrossAssemblyConsumer

open RuntimeAsyncCrossAssemblyLibrary

[<EntryPoint>]
let main _ =
    if getValue().GetAwaiter().GetResult() = 42 then 0 else 1
"""

    [<InlineData(false)>]
    [<InlineData(true)>]
    [<Theory>]
    let ``runtime async rejects pinning from an imported inline function after suspension`` (optimize: bool) =
        let library =
            FSharp """
module RuntimeAsyncPinningCrossAssemblyLibrary

open FSharp.NativeInterop

let inline comparePin (array: byte[]) ([<InlineIfLambda>] action) =
    use before = fixed array
    action ()
    use after = fixed array
    struct (NativePtr.toNativeInt before, NativePtr.toNativeInt after)
"""
            |> withName "RuntimeAsyncPinningCrossAssemblyLibrary"
            |> withLangVersionPreview
            |> withFSharpCoreShippedNet
            |> withNoWarn 9

        FSharp """
module RuntimeAsyncPinningCrossAssemblyConsumer

open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices
open RuntimeAsyncPinningCrossAssemblyLibrary

let run (data: byte[]) (gate: Task<unit>) =
    StateMachineHelpers.__runtimeAsyncReturn (
        comparePin data (fun () -> AsyncHelpers.Await gate))
"""
        |> withLangVersionPreview
        |> withFSharpCoreShippedNet
        |> withOptimization optimize
        |> withNoWarn 9
        |> withReferences [ library ]
        |> compile
        |> shouldFail
        |> withErrorCode 3919

    [<InlineData(false)>]
    [<InlineData(true)>]
    [<Theory>]
    let ``runtime async methods execute across assemblies`` (optimize: bool) =
        FSharp runtimeAsyncCrossAssemblyConsumer
        |> withLangVersionPreview
        |> withFSharpCoreShippedNet
        |> withOptimization optimize
        |> withReferences [
            FSharp runtimeAsyncCrossAssemblyLibrary
            |> withName "RuntimeAsyncCrossAssemblyLibrary"
            |> withLangVersionPreview
            |> withFSharpCoreShippedNet
        ]
        |> compileExeAndRun
        |> shouldSucceed
#endif

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
    let ``runtime async is unavailable in FSharp 11.0`` () =
        FSharp """
module RuntimeAsyncPreviewTest

open System.Threading.Tasks
open Microsoft.FSharp.Core.CompilerServices

let f : Task<int> =
    StateMachineHelpers.__runtimeAsyncReturn 1
"""
        |> withLangVersion11
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
        |> withErrorCodes [ 3918; 3918; 3918 ]

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

    [<Theory>]
    [<InlineData("default")>]
    [<InlineData("11.2")>]
    [<InlineData("preview")>]
    let ``runtime async compiles functions and members`` langVersion =
        FSharp runtimeAsyncSource
        |> withLangVersion langVersion
        |> withFSharpCoreShippedNet
        |> compile
        |> shouldSucceed

    [<Theory>]
    [<InlineData(false, false)>]
    [<InlineData(false, true)>]
    [<InlineData(true, false)>]
    [<InlineData(true, true)>]
    let ``runtime async supports Task and ValueTask return intrinsics`` (optimize: bool, asClosure: bool) =
        let prefix, suffix = if asClosure then "invoke (fun () -> ", ")" else "", ""
        let result =
            FSharp $"""
module RuntimeAsyncReturnShapesTest

open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices

[<NoCompilerInlining>]
let invoke callback = callback ()

[<NoCompilerInlining>]
let taskResult (gate: Task<int>) : Task<int> =
    {prefix}StateMachineHelpers.__runtimeAsyncReturn (AsyncHelpers.Await gate + 1){suffix}

[<NoCompilerInlining>]
let valueTaskResult (gate: Task<int>) : ValueTask<int> =
    {prefix}StateMachineHelpers.__runtimeAsyncReturnValueTask (AsyncHelpers.Await gate + 1){suffix}

[<NoCompilerInlining>]
let taskUnit (gate: Task<int>) : Task =
    {prefix}StateMachineHelpers.__runtimeAsyncReturnUnit (AsyncHelpers.Await gate |> ignore){suffix}

[<NoCompilerInlining>]
let valueTaskUnit (gate: Task<int>) : ValueTask =
    {prefix}StateMachineHelpers.__runtimeAsyncReturnValueTaskUnit (AsyncHelpers.Await gate |> ignore){suffix}

[<NoCompilerInlining>]
let taskGenericUnit (gate: Task<int>) : Task<unit> =
    {prefix}StateMachineHelpers.__runtimeAsyncReturn (AsyncHelpers.Await gate |> ignore){suffix}

[<NoCompilerInlining>]
let valueTaskGenericUnit (gate: Task<int>) : ValueTask<unit> =
    {prefix}StateMachineHelpers.__runtimeAsyncReturnValueTask (AsyncHelpers.Await gate |> ignore){suffix}

[<EntryPoint>]
let main _ =
    let gate = TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously)
    let taskValue = taskResult gate.Task
    let valueTaskValue = valueTaskResult gate.Task
    let taskVoid = taskUnit gate.Task
    let valueTaskVoid = valueTaskUnit gate.Task
    let taskUnitValue = taskGenericUnit gate.Task
    let valueTaskUnitValue = valueTaskGenericUnit gate.Task
    if taskValue.IsCompleted || valueTaskValue.IsCompleted || taskVoid.IsCompleted
       || valueTaskVoid.IsCompleted || taskUnitValue.IsCompleted || valueTaskUnitValue.IsCompleted then
        failwith "Return marker did not suspend"
    gate.SetResult 41
    if taskValue.GetAwaiter().GetResult() <> 42 || valueTaskValue.AsTask().GetAwaiter().GetResult() <> 42 then
        failwith "Generic result changed"
    taskVoid.GetAwaiter().GetResult()
    valueTaskVoid.AsTask().GetAwaiter().GetResult()
    taskUnitValue.GetAwaiter().GetResult()
    valueTaskUnitValue.AsTask().GetAwaiter().GetResult()
    0
"""
            |> withLangVersionPreview
            |> withFSharpCoreShippedNet
            |> withOptimization optimize
            |> compileExeAndRun
            |> shouldSucceed

        result |> withMetadataReader (fun md ->
            let asyncMethods =
                [ for handle in md.MethodDefinitions do
                    let method = md.GetMethodDefinition handle
                    if int method.ImplAttributes &&& 0x2000 <> 0 then
                        yield md.GetString method.Name ]
            let expected =
                if asClosure then List.replicate 6 "Invoke"
                else [ "taskResult"; "valueTaskResult"; "taskUnit"; "valueTaskUnit"; "taskGenericUnit"; "valueTaskGenericUnit" ]
            Assert.Equal<string list>(List.sort expected, List.sort asyncMethods))

    [<Theory>]
    [<InlineData(false, "Task")>]
    [<InlineData(true, "Task")>]
    [<InlineData(false, "Task<int>")>]
    [<InlineData(true, "Task<int>")>]
    [<InlineData(false, "ValueTask")>]
    [<InlineData(true, "ValueTask")>]
    [<InlineData(false, "ValueTask<int>")>]
    [<InlineData(true, "ValueTask<int>")>]
    let ``Issue 20686 - await upcast Task`` (optimize: bool, carrier: string) =
        let marker, value, toTask =
            match carrier with
            | "Task" -> "__runtimeAsyncReturnUnit", "()", ""
            | "Task<int>" -> "__runtimeAsyncReturn", "42", ""
            | "ValueTask" -> "__runtimeAsyncReturnValueTaskUnit", "()", ".AsTask()"
            | "ValueTask<int>" -> "__runtimeAsyncReturnValueTask", "42", ".AsTask()"
            | _ -> failwith $"Unexpected carrier: {carrier}"
        FSharp $"""
module TaskUpcasts

open System
open System.Threading
open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers

type BaseTask = Task

let inline awaitTask (work: Task) = AsyncHelpers.Await work

let run input =
    {marker} (
        AsyncHelpers.Await(Task.FromResult input :> Task)
        AsyncHelpers.Await((Task.FromResult input :> Task).ConfigureAwait false)
        AsyncHelpers.Await((Task.FromResult input :> Task).ConfigureAwait true)
        AsyncHelpers.Await(Task.FromResult input :> BaseTask)
        awaitTask (Task.FromResult input)
        {value})

let awaitSource (source: TaskCompletionSource<int>) shape =
    {marker} (
        match shape with
        | "direct" -> AsyncHelpers.Await(source.Task :> Task)
        | "configured false" -> AsyncHelpers.Await((source.Task :> Task).ConfigureAwait false)
        | "configured true" -> AsyncHelpers.Await((source.Task :> Task).ConfigureAwait true)
        | _ -> failwith "Unexpected await shape"
        {value})

let wait (work: {carrier}) =
    let result = (work{toTask}).WaitAsync(TimeSpan.FromSeconds 30.).GetAwaiter().GetResult()
    if result <> {value} then failwith "Incorrect result"

[<EntryPoint>]
let main _ =
    wait (run 1)
    wait (run "result")
    wait (run ())
    for shape in ["direct"; "configured false"; "configured true"] do
        for outcome in ["success"; "fault"; "cancel"] do
            let source = TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously)
            let work = awaitSource source shape
            if work.IsCompleted then failwith "Expected suspension"
            match outcome with
            | "success" ->
                source.SetResult 1
                wait work
            | "fault" ->
                let failure = InvalidOperationException("await failed")
                source.SetException failure
                try
                    wait work
                    failwith "Expected fault"
                with :? InvalidOperationException as observed when Object.ReferenceEquals(failure, observed) -> ()
            | "cancel" ->
                let token = CancellationToken(true)
                source.SetCanceled token
                try
                    wait work
                    failwith "Expected cancellation"
                with :? OperationCanceledException as observed when observed.CancellationToken = token -> ()
            | _ -> failwith "Unexpected outcome"
    0
"""
        |> withLangVersionPreview
        |> withFSharpCoreShippedNet
        |> withOptimization optimize
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

    let private checkReraiseOwnership optimized mode =
        let methods =
            match mode with
            | "CONTROLS" -> [ "synchronousSelection", false; "synchronousCleanup", false; "filteredReraise", false; "Await", false ]
            | "PRIMARY" -> [ "recover", true ]
            | _ ->
                [ "recover", true; "recoverString", true; "recoverValue", true; "innerOwner", true
                  "selection", true; "cleanup", true ]
        let result =
            FsFromPath(Path.Combine(__SOURCE_DIRECTORY__, "RuntimeAsync", "RuntimeAsyncReraiseOwnership.fs"))
            |> withLangVersionPreview
            |> withFSharpCoreShippedNet
            |> withOptimization optimized
            |> withDefines [mode]
            |> asExe
            |> compile
            |> shouldSucceed

        result
        |> verifyRuntimeAsyncExceptionRegions (methods |> List.map (fun (name, awaits) -> $"Reraise::{name}", awaits))
        |> run
        |> shouldSucceed

    [<Theory>]
    [<InlineData(false)>]
    [<InlineData(true)>]
    let ``Issue 20575 runtime async nested reraise ownership`` optimized =
        checkReraiseOwnership optimized "PRIMARY"

    [<Theory>]
    [<InlineData(false)>]
    [<InlineData(true)>]
    let ``Issue 20575 runtime async ownership matrix`` optimized =
        checkReraiseOwnership optimized "MATRIX"

    [<Theory>]
    [<InlineData(false)>]
    [<InlineData(true)>]
    let ``Issue 20575 legal synchronous exception region controls`` optimized =
        checkReraiseOwnership optimized "CONTROLS"

    let private compileInspectionProbe (body: string) =
        // C# leaves these calls in their EH regions; these libraries must never be executed.
        CSharp $"""
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

public static class Inspection
{{
    public static void Probe(Task<int> audit)
    {{
        {body}
    }}
}}
"""
        |> withName "Inspection"
        |> asLibrary
        |> compile
        |> shouldSucceed

    [<Theory>]
    [<InlineData("try { throw new Exception(); } catch { AsyncHelpers.Await((Task)audit); }", true)>]
    [<InlineData("try { throw new Exception(); } catch { AsyncHelpers.Await(audit); }", true)>]
    [<InlineData("try { throw new Exception(); } catch { AsyncHelpers.AwaitAwaiter(audit.GetAwaiter()); }", true)>]
    [<InlineData("try { throw new Exception(); } catch { AsyncHelpers.UnsafeAwaitAwaiter(audit.GetAwaiter()); }", true)>]
    [<InlineData("try { throw new Exception(); } finally { AsyncHelpers.Await(audit); }", true)>]
    [<InlineData("try { throw new Exception(); } catch when (audit.IsCompleted) { AsyncHelpers.Await(audit); }", true)>]
    [<InlineData("try { throw new Exception(); } catch when (AsyncHelpers.Await(audit) == 7) { }", true)>]
    [<InlineData("try { AsyncHelpers.Await(audit); } catch { }", false)>]
    let ``Issue 20575 inspection rejects suspension in exception regions`` body forbidden =
        let result = compileInspectionProbe body
        if forbidden then
            let error =
                Assert.Throws<System.Exception>(fun () ->
                    result |> verifyRuntimeAsyncExceptionRegions ["Inspection::Probe", false] |> ignore)
            Assert.StartsWith("Inspection::Probe: suspension in exception handler/filter at IL_", error.Message)
            Assert.Contains("regions (kind, try offset/length, handler offset/length, filter offset):", error.Message)
        else
            result |> verifyRuntimeAsyncExceptionRegions ["Inspection::Probe", false] |> ignore

    [<Theory>]
    [<InlineData("RuntimeAsyncTest::missing", false, "Missing probe method body: ")>]
    [<InlineData("AbstractProbe::MissingBody", false, "Missing probe method body: ")>]
    [<InlineData("RuntimeAsyncTest::rawBody", true, "Missing runtime-async body with suspension in ")>]
    let ``Issue 20575 inspection rejects missing probes and suspension`` methodName requiresAwait message =
        let result =
            FSharp (runtimeAsyncSource + "\ntype AbstractProbe = abstract MissingBody: unit -> unit\n")
            |> withLangVersionPreview
            |> withFSharpCoreShippedNet
            |> asLibrary
            |> compile
            |> shouldSucceed

        let error =
            Assert.Throws<System.Exception>(fun () ->
                result |> verifyRuntimeAsyncExceptionRegions [methodName, requiresAwait] |> ignore)
        Assert.Equal($"{message}{methodName}", error.Message)

    [<Fact>]
    let ``Issue 20575 inspection requires runtime async metadata even with suspension`` () =
        let result = compileInspectionProbe "AsyncHelpers.Await(audit);"
        result |> verifyRuntimeAsyncExceptionRegions ["Inspection::Probe", false] |> ignore
        let error =
            Assert.Throws<System.Exception>(fun () ->
                result |> verifyRuntimeAsyncExceptionRegions ["Inspection::Probe", true] |> ignore)
        Assert.Equal("Missing runtime-async body with suspension in Inspection::Probe", error.Message)

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
        |> withErrorCode 3920

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
        |> withErrorCode 3920

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
        |> withErrorCode 3921

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
    let ``runtime async suspension in exception region executes`` () =
        Path.Combine(__SOURCE_DIRECTORY__, "RuntimeAsync", "RuntimeTasksAsyncDisposalException.fs")
        |> FsFromPath
        |> withLangVersionPreview
        |> withFSharpCoreShippedNet
        |> compileExeAndRun
        |> shouldSucceed

    [<Fact>]
    let ``runtime async reraise preserves the innermost exception after suspension`` () =
        FSharp """
module RuntimeAsyncReraiseTest

open System
open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices

let f () : Task<int> =
    StateMachineHelpers.__runtimeAsyncReturn (
        try
            raise (InvalidOperationException("outer"))
        with _ ->
            AsyncHelpers.Await(Task.Delay(1))
            try
                raise (ArgumentException("inner"))
            with _ ->
                reraise ())

[<EntryPoint>]
let main _ =
    try
        f().GetAwaiter().GetResult() |> ignore
        1
    with
    | :? ArgumentException as ex when ex.Message = "inner" -> 0
    | _ -> 1
"""
        |> withLangVersionPreview
        |> withFSharpCoreShippedNet
        |> compileExeAndRun
        |> shouldSucceed

    [<InlineData(false)>]
    [<InlineData(true)>]
    [<Theory>]
    let ``runtime async preserves an outer reraise after a nested suspension`` (optimize: bool) =
        FSharp """
module RuntimeAsyncNestedReraiseTest

open System
open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices

let run (gate: Task) (outer: exn) (trace: ResizeArray<string>) : Task<int> =
    StateMachineHelpers.__runtimeAsyncReturn (
        try
            raise outer
        with _ ->
            AsyncHelpers.Await gate
            try
                reraise ()
            finally
                trace.Add "finally")

[<EntryPoint>]
let main _ =
    let gate = TaskCompletionSource<unit>()
    let trace = ResizeArray<string>()
    let outer = InvalidOperationException("outer")
    let work = run gate.Task outer trace
    gate.SetResult(())

    try
        work.GetAwaiter().GetResult() |> ignore
        1
    with
    | ex when obj.ReferenceEquals(ex, outer) && trace.ToArray() = [| "finally" |] -> 0
    | _ -> 1
"""
        |> withLangVersionPreview
        |> withFSharpCoreShippedNet
        |> withOptimization optimize
        |> compileExeAndRun
        |> shouldSucceed

    [<InlineData(false)>]
    [<InlineData(true)>]
    [<Theory>]
    let ``runtime async rejects suspension in an unmarked object member`` (optimize: bool) =
        FSharp """
module RuntimeAsyncUnmarkedMemberTest

open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices

type IInt =
    abstract Get : unit -> int

let make (gate: Task<int>) =
    StateMachineHelpers.__runtimeAsyncReturn (
        { new IInt with
            member _.Get() = AsyncHelpers.Await gate })
"""
        |> withLangVersionPreview
        |> withFSharpCoreShippedNet
        |> withOptimization optimize
        |> compile
        |> shouldFail
        |> withErrorCode 3918

    [<InlineData(false)>]
    [<InlineData(true)>]
    [<Theory>]
    let ``runtime async marker inside an object member remains supported`` (optimize: bool) =
        FSharp """
module RuntimeAsyncMarkedObjectMemberTest

open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices

type IInt =
    abstract Get : unit -> Task<int>

let make (gate: Task<int>) =
    { new IInt with
        member _.Get() =
            StateMachineHelpers.__runtimeAsyncReturn (AsyncHelpers.Await gate) }

[<EntryPoint>]
let main _ =
    let value = (make (Task.FromResult 41)).Get().GetAwaiter().GetResult()
    if value = 41 then 0 else 1
"""
        |> withLangVersionPreview
        |> withFSharpCoreShippedNet
        |> withOptimization optimize
        |> compileExeAndRun
        |> shouldSucceed

    [<InlineData(false)>]
    [<InlineData(true)>]
    [<Theory>]
    let ``runtime async evaluates a suspending exception filter once`` (optimize: bool) =
        FSharp """
module RuntimeAsyncExceptionFilterTest

open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices

let run (gate: Task) (predicate: unit -> bool) : Task<int> =
    StateMachineHelpers.__runtimeAsyncReturn (
        try
            failwith "body"
        with _ when (AsyncHelpers.Await gate; predicate()) ->
            7)

[<EntryPoint>]
let main _ =
    let gate = TaskCompletionSource<unit>()
    let mutable calls = 0

    let firstTrue () =
        calls <- calls + 1
        calls = 1

    let work = run gate.Task firstTrue
    gate.SetResult(())

    try
        if work.GetAwaiter().GetResult() = 7 && calls = 1 then 0 else 1
    with _ ->
        1
"""
        |> withLangVersionPreview
        |> withFSharpCoreShippedNet
        |> withOptimization optimize
        |> compileExeAndRun
        |> shouldSucceed

    [<Fact>]
    let ``runtime async is unavailable in FSharp 9.0 when optimization is disabled`` () =
        FSharp """
module RuntimeAsyncNoOptimizePreviewTest

open System.Threading.Tasks
open Microsoft.FSharp.Core.CompilerServices

let f (x: int) : Task<int> =
    x |> StateMachineHelpers.__runtimeAsyncReturn
"""
        |> withLangVersion90
        |> withFSharpCoreShippedNet
        |> withNoOptimize
        |> compile
        |> shouldFail
        |> withErrorCode 3350

    [<Fact>]
    let ``runtime async rejects suspension inside an ordinary sequence`` () =
        FSharp """
module RuntimeAsyncNestedSequenceTest

open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices

let f (gate: Task<int>) : Task<int seq> =
    StateMachineHelpers.__runtimeAsyncReturn (
        seq {
            let value = AsyncHelpers.Await gate
            yield value
            yield value + 1
        })
"""
        |> withLangVersionPreview
        |> withFSharpCoreShippedNet
        |> compile
        |> shouldFail
        |> withErrorCode 3918

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

module RuntimeAsyncCallbackTests =
    open Xunit
    open FSharp.Test
    open FSharp.Test.Compiler
    open System.IO

#if NETCOREAPP
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

    [<Theory>]
    [<InlineData(false, false)>]
    [<InlineData(true, false)>]
    [<InlineData(false, true)>]
    [<InlineData(true, true)>]
    let ``runtime async fuses returned closures across recursive bindings`` (optimize: bool, nestedRuntimeAsync: bool) =
        let loopBody =
            if nestedRuntimeAsync then
                "__runtimeAsyncReturn (if count = 0 then AsyncHelpers.Await ready else AsyncHelpers.Await (loop (count - 1)))"
            else
                "if count = 0 then ready else loop (count - 1)"

        FSharp $"""
module RuntimeAsyncRecursiveReturnedClosureTest

open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers

let mutable constructions = 0
let mutable invocations = 0

let inline runBody ([<InlineIfLambda>] body: unit -> unit -> int) =
    __runtimeAsyncReturn (body () ())

let run (ready: Task<int>) =
    runBody (fun () ->
        constructions <- constructions + 1
        let rec loop count : Task<int> =
            {loopBody}
        fun () ->
            invocations <- invocations + 1
            AsyncHelpers.Await (loop 2))

[<EntryPoint>]
let main _ =
    let ready = TaskCompletionSource<int>()
    let result = run ready.Task
    if result.IsCompleted || constructions <> 1 || invocations <> 1 then
        failwith "Callback evaluation or suspension was lost"
    ready.SetResult 42
    if result.GetAwaiter().GetResult() <> 42 || constructions <> 1 || invocations <> 1 then
        failwith "Recursive callback returned an incorrect result"
    0
"""
        |> withLangVersionPreview
        |> withFSharpCoreShippedNet
        |> withOptimization optimize
        |> compileExeAndRun
        |> shouldSucceed

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

    [<InlineData(false)>]
    [<InlineData(true)>]
    [<Theory>]
    let ``runtime async evaluates conditional callback construction once`` (optimize: bool) =
        FSharp """
module RuntimeAsyncConditionalCallbackConstructionTest

open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices

let events = ResizeArray<string>()
let note text = events.Add text

[<NoCompilerInlining>]
let choose () =
    note "choose"
    true

let inline twice ([<InlineIfLambda>] body: unit -> int) =
    StateMachineHelpers.__runtimeAsyncReturn (body() + body())

let run (gate: Task<int>) =
    twice (
        if choose() then
            note "construct"
            fun () ->
                note "body"
                AsyncHelpers.Await gate
        else
            fun () -> 0)

[<EntryPoint>]
let main _ =
    let gate = TaskCompletionSource<int>()
    let work = run gate.Task
    gate.SetResult(21)
    let result = work.GetAwaiter().GetResult()

    if result = 42 && events.ToArray() = [| "choose"; "construct"; "body"; "body" |] then 0 else 1
"""
        |> withLangVersionPreview
        |> withFSharpCoreShippedNet
        |> withOptimization optimize
        |> compileExeAndRun
        |> shouldSucceed

    [<InlineData(false, false)>]
    [<InlineData(false, true)>]
    [<InlineData(true, false)>]
    [<InlineData(true, true)>]
    [<Theory>]
    let ``Issue 20577 preserves stateful conditional fold callbacks`` (optimize: bool, useList: bool) =
        let source = $"""
module RuntimeAsyncConditionalFoldTest

open System.Collections.Generic
open System.Threading.Tasks
open RuntimeTaskBuilder.RuntimeTask

let run deduplicate (ready: Task<int>) =
    runtimeTask {{
        let! initial = ready
        let folder =
            if deduplicate then
                let seen = HashSet<int>()
                fun total item -> if seen.Add item then total + item else total
            else
                fun total item -> total + item

        return
            {if useList then "List.fold folder initial [ 1; 1; 2 ]" else "Array.fold folder initial [| 1; 1; 2 |]"}
    }}

[<EntryPoint>]
let main _ =
    for deduplicate in [ true; false ] do
        let gate = TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously)
        let work = run deduplicate gate.Task
        if work.IsCompleted then failwith "Expected suspension"
        gate.SetResult 0
        let expected = if deduplicate then 3 else 4
        if work.GetAwaiter().GetResult() <> expected then failwith "Callback state was lost"
    0
"""

        FsFromPath (Path.Combine(__SOURCE_DIRECTORY__, "RuntimeAsync", "RuntimeTaskBuilder.fs"))
        |> withAdditionalSourceFile (FsSourceWithFileName "RuntimeAsyncConditionalFoldTest.fs" source)
        |> withLangVersionPreview
        |> withFSharpCoreShippedNet
        |> withOptimization optimize
        |> compileExeAndRun
        |> shouldSucceed

    [<InlineData(false)>]
    [<InlineData(true)>]
    [<Theory>]
    let ``runtime async conditional callbacks share mutable captures with closures`` (optimize: bool) =
        FSharp """
module RuntimeAsyncConditionalMutableCapture

open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices

let mutable observe = fun () -> -1

let inline invoke ([<InlineIfLambda>] callback: unit -> int) =
    StateMachineHelpers.__runtimeAsyncReturn (callback ())

let run flag =
    invoke (
        if flag then
            let mutable count = 0
            observe <- fun () -> count
            fun () ->
                AsyncHelpers.Await(Task.FromResult 0) |> ignore
                count <- count + 1
                count
        else
            fun () -> 0)

let runMultiple flag =
    invoke (
        if flag then
            let mutable count = 0
            let mutable additional = 0
            observe <- fun () -> count + additional
            fun () ->
                AsyncHelpers.Await(Task.FromResult 0) |> ignore
                count <- count + 1
                additional <- additional + 1
                count + additional
        else
            fun () -> 0)

[<EntryPoint>]
let main _ =
    let actual = (run true).Result, observe ()
    let multiple = (runMultiple true).Result, observe ()
    if actual = (1, 1) && multiple = (2, 2) then 0 else 1
"""
        |> withLangVersionPreview
        |> withFSharpCoreShippedNet
        |> withOptimization optimize
        |> compileExeAndRun
        |> shouldSucceed

    [<InlineData(false)>]
    [<InlineData(true)>]
    [<Theory>]
    let ``runtime async inlines a stateful conditional callback from an imported inline`` (optimize: bool) =
        let library =
            FSharp """
module RuntimeAsyncCallbackLibrary

let inline invokeTwice ([<InlineIfLambda>] callback: unit -> int) =
    callback() + callback()
"""
            |> withName "RuntimeAsyncCallbackLibrary"
            |> withLangVersionPreview
            |> withFSharpCoreShippedNet

        FSharp """
module RuntimeAsyncImportedCallbackTest

open System.Collections.Generic
open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices
open RuntimeAsyncCallbackLibrary

let mutable constructions = 0
let mutable invocations = 0

let run (gate: Task) deduplicate =
    StateMachineHelpers.__runtimeAsyncReturn (
        invokeTwice (
            if deduplicate then
                constructions <- constructions + 1
                let seen = HashSet<int>()
                fun () ->
                    invocations <- invocations + 1
                    let first = seen.Add 1
                    AsyncHelpers.Await gate
                    if first then 1 else 0
            else
                fun () -> 2))

[<EntryPoint>]
let main _ =
    let gate = TaskCompletionSource<unit>(TaskCreationOptions.RunContinuationsAsynchronously)
    let work = run gate.Task true
    if work.IsCompleted || constructions <> 1 || invocations <> 1 then
        failwith "Callback construction or first invocation did not run before suspension"
    gate.SetResult(())
    if work.GetAwaiter().GetResult() <> 1 || constructions <> 1 || invocations <> 2 then
        failwith "Callback state was not shared"
    if (run gate.Task false).GetAwaiter().GetResult() <> 4 || constructions <> 1 || invocations <> 2 then
        failwith "The other branch changed"
    0
"""
        |> withLangVersionPreview
        |> withFSharpCoreShippedNet
        |> withOptimization optimize
        |> withReferences [ library ]
        |> compileExeAndRun
        |> shouldSucceed

    [<InlineData(false)>]
    [<InlineData(true)>]
    [<Theory>]
    let ``runtime async inlined conditional callbacks preserve cleanup after suspension`` (optimize: bool) =
        FSharp """
module RuntimeAsyncConditionalCallbackCleanupTest

open System.Collections.Generic
open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices

let mutable cleaned = 0

let inline invokeTwice ([<InlineIfLambda>] callback: unit -> int) =
    StateMachineHelpers.__runtimeAsyncReturn (callback() + callback())

let run (gate: Task) (cleanupGate: Task) (cleanupStarted: TaskCompletionSource<unit>) =
    invokeTwice (
        if true then
            let seen = HashSet<int>()
            fun () ->
                try
                    let first = seen.Add 1
                    AsyncHelpers.Await gate
                    if first then 1 else 0
                finally
                    cleanupStarted.TrySetResult(()) |> ignore
                    AsyncHelpers.Await cleanupGate
                    cleaned <- cleaned + 1
        else
            fun () -> 0)

[<EntryPoint>]
let main _ =
    let gate = TaskCompletionSource<unit>(TaskCreationOptions.RunContinuationsAsynchronously)
    let cleanupGate = TaskCompletionSource<unit>(TaskCreationOptions.RunContinuationsAsynchronously)
    let cleanupStarted = TaskCompletionSource<unit>(TaskCreationOptions.RunContinuationsAsynchronously)
    let work = run gate.Task cleanupGate.Task cleanupStarted
    if work.IsCompleted || cleaned <> 0 then failwith "Cleanup ran before suspension"
    gate.SetResult(())
    if not (cleanupStarted.Task.Wait(5000)) || work.IsCompleted then
        failwith "Cleanup did not suspend"
    cleanupGate.SetResult(())
    if work.GetAwaiter().GetResult() <> 1 || cleaned <> 2 then
        failwith "Cleanup or callback state was lost"
    0
"""
        |> withLangVersionPreview
        |> withFSharpCoreShippedNet
        |> withOptimization optimize
        |> compileExeAndRun
        |> shouldSucceed

    [<InlineData(false, false)>]
    [<InlineData(false, true)>]
    [<InlineData(true, false)>]
    [<InlineData(true, true)>]
    [<Theory>]
    let ``runtime async does not rewrite opaque callback consumers`` (optimize: bool, quoted: bool) =
        let opaqueUse =
            if quoted then
                "let saved = <@ callback() @> in saved.ToString() |> ignore"
            else
                "consume callback |> ignore"

        FSharp $"""
module RuntimeAsyncOpaqueCallbackTest

open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices

[<NoCompilerInlining>]
let consume (callback: unit -> int) = callback()

let inline invoke ([<InlineIfLambda>] callback: unit -> int) =
    StateMachineHelpers.__runtimeAsyncReturn (
        let result = callback()
        {opaqueUse}
        result)

let run (gate: Task<int>) flag =
    invoke (if flag then fun () -> AsyncHelpers.Await gate else fun () -> 0)
"""
        |> withLangVersionPreview
        |> withFSharpCoreShippedNet
        |> withOptimization optimize
        |> compile
        |> shouldFail
        |> withErrorCode 3918

    [<Fact>]
    let ``runtime async evaluates inline callback construction once`` () =
        FSharp """
module RuntimeAsyncCallbackConstructionTest

open System.Threading.Tasks
open Microsoft.FSharp.Core.CompilerServices

let mutable constructed = 0

let inline twice ([<InlineIfLambda>] f: unit -> int) =
    StateMachineHelpers.__runtimeAsyncReturn (f () + f ())

let run () =
    twice (constructed <- constructed + 1; fun () -> 21)

[<EntryPoint>]
let main _ =
    let result = run().GetAwaiter().GetResult()
    if result = 42 && constructed = 1 then 0 else 1
"""
        |> withLangVersionPreview
        |> withFSharpCoreShippedNet
        |> compileExeAndRun
        |> shouldSucceed

    [<InlineData(false)>]
    [<InlineData(true)>]
    [<Theory>]
    let ``runtime async inlines a callback after one that cannot be inlined`` (optimize: bool) =
        FSharp """
module RuntimeAsyncInlineAfterOpaqueCallback
open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices

[<NoCompilerInlining>]
let consume (f: unit -> int) = f ()

let inline invokeBoth ([<InlineIfLambda>] a: unit -> int) ([<InlineIfLambda>] b: unit -> int) =
    consume a + b() + b()

let run (gate: Task<int>) flag =
    StateMachineHelpers.__runtimeAsyncReturn (
        invokeBoth
            (let v = AsyncHelpers.Await gate in fun () -> v)
            (if flag then (fun () -> AsyncHelpers.Await gate + 1) else (fun () -> 2)))

[<EntryPoint>]
let main _ =
    if (run (Task.FromResult 10) true).Result <> 32 then failwith "bad"
    if (run (Task.FromResult 10) false).Result <> 14 then failwith "bad2"
    0
"""
        |> withLangVersionPreview
        |> withFSharpCoreShippedNet
        |> withOptimization optimize
        |> compileExeAndRun
        |> shouldSucceed

    [<InlineData(false)>]
    [<InlineData(true)>]
    [<Theory>]
    let ``runtime async inlines pattern-bound captures of a branch-selected callback`` (optimize: bool) =
        FSharp """
module RuntimeAsyncPatternCapturedCallback
open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices

let mutable constructions = 0

let inline twice ([<InlineIfLambda>] f: int -> int) = f 1 + f 2

let run (gate: Task<int>) (choice: int option) =
    StateMachineHelpers.__runtimeAsyncReturn (
        twice (
            constructions <- constructions + 1
            match choice with
            | Some offset ->
                let scaled = offset * 10
                fun x -> AsyncHelpers.Await gate + scaled + x
            | None -> fun x -> x))

[<EntryPoint>]
let main _ =
    let gate = TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously)
    let pending = run gate.Task (Some 4)
    if pending.IsCompleted then failwith "Expected pending result"
    gate.SetResult 100
    if pending.GetAwaiter().GetResult() <> 283 then failwith "bad some"
    if (run gate.Task None).Result <> 3 then failwith "bad none"
    if constructions <> 2 then failwith "construction repeated"
    0
"""
        |> withLangVersionPreview
        |> withFSharpCoreShippedNet
        |> withOptimization optimize
        |> compileExeAndRun
        |> shouldSucceed

    [<InlineData(false)>]
    [<InlineData(true)>]
    [<Theory>]
    let ``runtime async reconstructs a branch-selected callback on each loop iteration`` (optimize: bool) =
        FSharp """
module RuntimeAsyncCallbackInLoop
open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices

let inline invoke ([<InlineIfLambda>] f: int -> int) = f 1 + f 2

let run (gate: Task<int>) =
    StateMachineHelpers.__runtimeAsyncReturn (
        let mutable total = 0
        for i in 0 .. 2 do
            total <-
                total
                + invoke (
                    if i % 2 = 0 then
                        (let k = i * 100 in fun x -> AsyncHelpers.Await gate + k + x)
                    else
                        (fun x -> x))
        total)

[<EntryPoint>]
let main _ =
    let result = (run (Task.FromResult 1000)).Result
    if result <> 4409 then failwithf "Unexpected: %d" result
    0
"""
        |> withLangVersionPreview
        |> withFSharpCoreShippedNet
        |> withOptimization optimize
        |> compileExeAndRun
        |> shouldSucceed

    [<InlineData(false)>]
    [<InlineData(true)>]
    [<Theory>]
    let ``runtime async inlines nested stateful branch-selected callbacks`` (optimize: bool) =
        FSharp """
module RuntimeAsyncNestedBranchCallbacks
open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices

let inline invoke ([<InlineIfLambda>] f: int -> int) = f 1 + f 2 + f 3

let inline pick flag ([<InlineIfLambda>] a: int -> int) ([<InlineIfLambda>] b: int -> int) =
    invoke (if flag then (fun x -> a x + b x) else (fun x -> b (a x)))

let run (gate: Task<int>) f1 f2 =
    StateMachineHelpers.__runtimeAsyncReturn (
        pick
            f1
            (if f2 then
                let mutable count = 0
                fun x ->
                    count <- count + 1
                    AsyncHelpers.Await gate + x + count
             else fun x -> x * 2)
            (if f2 then fun x -> x + 1
             else
                let mutable count = 0
                fun x ->
                    count <- count + 1
                    AsyncHelpers.Await gate - x + count))

[<EntryPoint>]
let main _ =
    let r a b = (run (Task.FromResult 10) a b).Result
    let results = r true true, r true false, r false true, r false false
    if results <> (51, 42, 45, 24) then failwithf "Unexpected: %A" results
    0
"""
        |> withLangVersionPreview
        |> withFSharpCoreShippedNet
        |> withOptimization optimize
        |> compileExeAndRun
        |> shouldSucceed

    [<Theory>]
    [<InlineData(false, false)>]
    [<InlineData(false, true)>]
    [<InlineData(true, false)>]
    [<InlineData(true, true)>]
    let ``runtime async lowers deeply nested callback constructions`` (optimize: bool, asDelegate: bool) =
        let depth = 1500
        let statements = String.replicate depth "            touch ()\n"
        let callbackType, invocation, wrap =
            if asDelegate then
                "Callback", "callback.Invoke", fun body -> $"Callback(fun x -> {body})"
            else
                "int -> int", "callback", fun body -> $"(fun x -> {body})"

        let source = $"""
module RuntimeAsyncDeepConstruction
open System
open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices

type Callback = delegate of int -> int
let mutable constructions = 0
[<NoCompilerInlining>]
let touch () = constructions <- constructions + 1
let inline twice ([<InlineIfLambda>] callback: {callbackType}) = {invocation} 1 + {invocation} 2

[<NoCompilerInlining>]
let run (gate: Task<int>) flag =
    StateMachineHelpers.__runtimeAsyncReturn (
        twice (
{statements}            let offset = 7
            if flag then {wrap "AsyncHelpers.Await gate + offset + x"}
            else {wrap "offset + x"}))

[<EntryPoint>]
let main _ =
    let gate = TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously)
    let pending = run gate.Task true
    if pending.IsCompleted then failwith "Expected pending result"
    if constructions <> {depth} then failwith "Construction was not evaluated once"
    gate.SetResult 41
    if pending.GetAwaiter().GetResult() <> 99 then failwith "Suspending branch changed"
    if (run gate.Task false).Result <> 17 then failwith "Non-suspending branch changed"
    if constructions <> {2 * depth} then failwith "Construction was repeated"
    0
"""
        let result =
            FSharp source
            |> withLangVersionPreview
            |> withFSharpCoreShippedNet
            |> withOptimization optimize
            |> compileExeAndRun
            |> shouldSucceed

        // In-process compilation can have a larger stack than command-line compilation.
        let outputDirectory = result.Output.OutputPath |> Option.get |> Path.GetDirectoryName
        let sourcePath = Path.Combine(outputDirectory, "DeepConstruction.fs")
        let outputPath = Path.Combine(outputDirectory, "DeepConstruction.exe")
        File.WriteAllText(sourcePath, source)
        let cliResult =
            runFscProcess [
                yield! CompilerAssert.DefaultProjectOptions(Utilities.TargetFramework.FSharpCoreShippedNet).OtherOptions
                yield "--target:exe"
                yield "--langversion:preview"
                yield if optimize then "--optimize+" else "--optimize-"
                yield $"-o:\"{outputPath}\""
                yield $"\"{sourcePath}\""
            ]
        Assert.True(cliResult.ExitCode = 0, $"{cliResult.StdOut}\n{cliResult.StdErr}")

    [<InlineData(16, false, true)>]
    [<InlineData(17, false, false)>]
    [<InlineData(16, true, true)>]
    [<InlineData(17, true, false)>]
    [<InlineData(40, false, false)>]
    [<Theory>]
    let ``runtime async bounds branch-selected callback copies across all invocations`` (invocations: int, nested: bool, withinBudget: bool) =
        let calls =
            if nested then
                [1 .. invocations] |> List.fold (fun arg _ -> $"f ({arg})") "1"
            else
                List.init invocations (sprintf "f %d") |> String.concat " + "

        let terms = List.init 30 (sprintf "x * %d") |> String.concat " + "

        let compilation =
            FSharp $"""
module RuntimeAsyncOversizedCallback
open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices

let inline invoke ([<InlineIfLambda>] f: int -> int) =
    let unrelated = <@ 42 @>
    unrelated.ToString() |> ignore
    {calls}

let run (gate: Task<int>) flag =
    StateMachineHelpers.__runtimeAsyncReturn (
        invoke (if flag then (fun x -> AsyncHelpers.Await gate + {terms}) else (fun x -> x)))

[<EntryPoint>]
let main _ =
    let evaluate x = List.fold (fun total n -> total + x * n) 10 [0 .. 29]
    let expected =
        if {nested.ToString().ToLowerInvariant()} then
            [1 .. {invocations}] |> List.fold (fun arg _ -> evaluate arg) 1
        else
            [0 .. {invocations - 1}] |> List.sumBy evaluate
    let expectedOtherBranch = if {nested.ToString().ToLowerInvariant()} then 1 else {invocations * (invocations - 1) / 2}
    let result flag = (run (Task.FromResult 10) flag).Result
    if result true <> expected then failwith "Unexpected suspending branch result"
    if result false <> expectedOtherBranch then failwith "Unexpected other branch result"
    0
"""
            |> asExe
            |> withLangVersionPreview
            |> withFSharpCoreShippedNet

        if withinBudget then
            compilation |> compileExeAndRun |> shouldSucceed
        else
            compilation |> compile |> shouldFail |> withErrorCode 3918

    [<InlineData(false)>]
    [<InlineData(true)>]
    [<Theory>]
    let ``runtime async callback selected by a branch keeps AsyncLocal changes`` (optimize: bool) =
        FSharp """
module RuntimeAsyncBranchCallbackAsyncLocal
open System.Threading
open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices

let context = AsyncLocal<string>()

let inline invoke ([<InlineIfLambda>] f: unit -> string) =
    let seen = f ()
    seen + "/" + context.Value

let run (gate: Task<int>) flag =
    StateMachineHelpers.__runtimeAsyncReturn (
        context.Value <- "outer"
        invoke (
            if flag then
                (let captured = context.Value + ":"
                 fun () ->
                    let before = context.Value
                    AsyncHelpers.Await gate |> ignore
                    let after = context.Value
                    context.Value <- "inner"
                    captured + before + "-" + after)
            else
                (fun () -> "none")))

[<EntryPoint>]
let main _ =
    let gate = TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously)
    let pending = run gate.Task true
    if pending.IsCompleted then failwith "Expected pending result"
    gate.SetResult 1
    let result = pending.GetAwaiter().GetResult()
    if result <> "outer:outer-outer/inner" then failwithf "Unexpected: %s" result
    let completed = (run (Task.FromResult 1) true).Result
    if completed <> "outer:outer-outer/inner" then failwithf "Unexpected completed: %s" completed
    if (run (Task.FromResult 1) false).Result <> "none/outer" then failwith "bad2"
    if not (isNull context.Value) then failwith "AsyncLocal leaked to caller"
    0
"""
        |> withLangVersionPreview
        |> withFSharpCoreShippedNet
        |> withOptimization optimize
        |> compileExeAndRun
        |> shouldSucceed

    [<InlineData(false)>]
    [<InlineData(true)>]
    [<Theory>]
    let ``runtime async prepares a body after a callback is inlined into its handler`` (optimize: bool) =
        FSharp """
module RuntimeAsyncCallbackInHandler
open System.Threading.Tasks
open Microsoft.FSharp.Core.CompilerServices

type Started<'T> = delegate of unit -> 'T

let inline run ([<InlineIfLambda>] code: Started<int>) =
    StateMachineHelpers.__runtimeAsyncReturn (
        try
            failwith "boom"
        with _ ->
            code.Invoke())

let execute (pending: Task<int>) chooseFirst =
    run (
        if chooseFirst then
            Started(fun () -> System.Runtime.CompilerServices.AsyncHelpers.Await pending)
        else
            Started(fun () -> System.Runtime.CompilerServices.AsyncHelpers.Await pending + 1))

[<EntryPoint>]
let main _ =
    let gate = TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously)
    let first = execute gate.Task true
    let second = execute gate.Task false
    if first.IsCompleted || second.IsCompleted then failwith "Expected pending results"
    gate.SetResult 41
    if first.GetAwaiter().GetResult() <> 41 || second.GetAwaiter().GetResult() <> 42 then failwith "Wrong result"
    0
"""
        |> withLangVersionPreview
        |> withFSharpCoreShippedNet
        |> withOptimization optimize
        |> compileExeAndRun
        |> shouldSucceed
#endif

module RuntimeAsyncDelegateTests =
    open Xunit
    open FSharp.Test.Compiler

#if NETCOREAPP
    [<InlineData(false, false)>]
    [<InlineData(false, true)>]
    [<InlineData(true, false)>]
    [<InlineData(true, true)>]
    [<Theory>]
    let ``runtime async delegate await does not repeat a pending sequence move`` (optimize: bool, recipe: bool) =
        FSharp $"""
module RuntimeAsyncDelegateAwait

open System
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices

type Started<'T> = delegate of unit -> 'T
let mutable constructions = 0

type Builder() =
    member inline _.Delay([<InlineIfLambda>] code: unit -> 'T) = code
    member inline _.Run([<InlineIfLambda>] code: unit -> 'T) : Task<'T> =
        StateMachineHelpers.__runtimeAsyncReturn(code())
    member inline _.Source(work: ValueTask<'T>) =
        constructions <- constructions + 1
        Started(fun () -> AsyncHelpers.Await work)
    member inline _.Bind([<InlineIfLambda>] work: Started<'T>, [<InlineIfLambda>] next: 'T -> 'U) =
        next (work.Invoke())
    member inline _.While([<InlineIfLambda>] guard: unit -> bool, [<InlineIfLambda>] body: unit -> unit) =
        while guard() do body()
    member inline _.Zero() = ()
    member inline _.Combine(_: unit, [<InlineIfLambda>] next: unit -> 'T) = next()
    member inline _.Return(value) = value

let builder = Builder()

type GuardedEnumerator(gate: Task) =
    let mutable active = 0
    let mutable count = 0
    interface IAsyncEnumerator<int> with
        member _.Current = count
        member _.MoveNextAsync() =
            if Interlocked.Exchange(&active, 1) = 1 then
                invalidOp "MoveNextAsync cannot be called concurrently."
            StateMachineHelpers.__runtimeAsyncReturnValueTask(
                try
                    AsyncHelpers.Await gate
                    count <- count + 1
                    count = 1
                finally
                    Interlocked.Exchange(&active, 0) |> ignore)
        member _.DisposeAsync() = ValueTask()

let collect (e: IAsyncEnumerator<int>) =
    builder {{
        let values = ResizeArray()
        while! e.MoveNextAsync() do
            values.Add(e.Current)
        return values.ToArray()
    }}

[<EntryPoint>]
let main _ =
    let gate = TaskCompletionSource<unit>(TaskCreationOptions.RunContinuationsAsynchronously)
    let enumerator =
        if {if recipe then "true" else "false"} then
            let source = StateMachineHelpers.__runtimeAsyncSequence(fun () -> seq {{
                do AsyncHelpers.Await gate.Task
                yield 1
            }})
            source.GetAsyncEnumerator()
        else
            GuardedEnumerator(gate.Task) :> IAsyncEnumerator<int>
    let pending = collect enumerator
    if pending.IsCompleted then
        pending.GetAwaiter().GetResult() |> ignore
        failwith "Expected a pending first move"
    if constructions <> 1 then failwith "Expected one construction before suspension"
    gate.SetResult(())
    if pending.GetAwaiter().GetResult() <> [| 1 |] || constructions <> 2 then
        failwith "Unexpected sequence values or delegate constructions"
    0
"""
        |> withLangVersionPreview
        |> withFSharpCoreShippedNet
        |> withOptimization optimize
        |> compileExeAndRun
        |> shouldSucceed

    [<InlineData(false)>]
    [<InlineData(true)>]
    [<Theory>]
    let ``runtime async handles imported InlineIfLambda delegate invocations`` (optimize: bool) =
        let library =
            FSharp """
module ImportedDelegateAwait
open Microsoft.FSharp.Core.CompilerServices

type Started<'T> = delegate of unit -> 'T

let inline run ([<InlineIfLambda>] callback: Started<int>) =
    StateMachineHelpers.__runtimeAsyncReturn(callback.Invoke())
"""
            |> withName "ImportedDelegateAwait"
            |> withLangVersionPreview
            |> withFSharpCoreShippedNet

        FSharp """
module RuntimeAsyncImportedDelegate
open System.Threading.Tasks
open System.Runtime.CompilerServices
open ImportedDelegateAwait

let run (gate: Task<int>) (cleanup: Task) (entered: TaskCompletionSource<unit>) =
    ImportedDelegateAwait.run (Started(fun () ->
        try
            AsyncHelpers.Await gate
        finally
            entered.SetResult(())
            AsyncHelpers.Await cleanup))

[<EntryPoint>]
let main _ =
    let gate = TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously)
    let cleanup = TaskCompletionSource<unit>(TaskCreationOptions.RunContinuationsAsynchronously)
    let entered = TaskCompletionSource<unit>(TaskCreationOptions.RunContinuationsAsynchronously)
    let pending = run gate.Task cleanup.Task entered
    if pending.IsCompleted then failwith "Expected suspension"
    gate.SetResult 42
    if not (entered.Task.Wait(5000)) || pending.IsCompleted then
        failwith "Cleanup must suspend before completing the delegate"
    cleanup.SetResult(())
    if pending.GetAwaiter().GetResult() <> 42 then failwith "Lost delegate await"
    0
"""
        |> withLangVersionPreview
        |> withFSharpCoreShippedNet
        |> withOptimization optimize
        |> withReferences [ library ]
        |> compileExeAndRun
        |> shouldSucceed

    [<InlineData(false)>]
    [<InlineData(true)>]
    [<Theory>]
    let ``runtime async inlines nested single-use delegate sources`` (optimize: bool) =
        let source =
            FSharp """
module NestedDelegateSource
open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices

type Started<'T> = delegate of unit -> 'T
let mutable constructions = 0

let inline compose ([<InlineIfLambda>] source: Started<int>) =
    Started(fun () -> source.Invoke() + 1)

let inline run ([<InlineIfLambda>] code: Started<int>) =
    StateMachineHelpers.__runtimeAsyncReturn(code.Invoke())

[<MethodImpl(MethodImplOptions.NoInlining)>]
let execute (pending: Task<int>) chooseFirst =
    run (compose (
        if chooseFirst then
            Started(fun () -> AsyncHelpers.Await pending)
        else
            Started(fun () -> AsyncHelpers.Await pending + 1)))

let inline runAfterConstruction ([<InlineIfLambda>] code: Started<int>) =
    StateMachineHelpers.__runtimeAsyncReturn(
        if constructions = 0 then failwith "Callback constructed after entering run"
        code.Invoke())

[<MethodImpl(MethodImplOptions.NoInlining)>]
let executeWithConstruction (pending: Task<int>) =
    runAfterConstruction (compose (
        constructions <- constructions + 1
        Started(fun () -> AsyncHelpers.Await pending)))

[<EntryPoint>]
let main _ =
    let gate = TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously)
    let first = execute gate.Task true
    let second = execute gate.Task false
    let third = executeWithConstruction gate.Task
    if first.IsCompleted || second.IsCompleted || third.IsCompleted || constructions <> 1 then
        failwith "Delegate construction or suspension changed"
    gate.SetResult 41
    if first.GetAwaiter().GetResult() <> 42 || second.GetAwaiter().GetResult() <> 43 || third.GetAwaiter().GetResult() <> 42 then
        failwith "Nested delegate result changed"
    0
"""
            |> withLangVersionPreview
            |> withFSharpCoreShippedNet
            |> withOptimization optimize
            |> compileExeAndRun
            |> shouldSucceed

        // The construction in executeWithConstruction precedes its runtime-async body, which therefore starts as
        // a closure; callbacks are inlined, so that closure is the only async Invoke.
        source |> withMetadataReader (fun md ->
            let asyncInvokes =
                [ for handle in md.TypeDefinitions do
                    let ty = md.GetTypeDefinition handle
                    for methodHandle in ty.GetMethods() do
                        let method = md.GetMethodDefinition methodHandle
                        if md.GetString method.Name = "Invoke" && int method.ImplAttributes &&& 0x2000 <> 0 then
                            yield md.GetString ty.Name ]
            let asyncInvoke = Assert.Single asyncInvokes
            Assert.StartsWith("executeWithConstruction@", asyncInvoke))

    [<InlineData(false)>]
    [<InlineData(true)>]
    [<Theory>]
    let ``runtime async merges delegate sources`` (optimize: bool) =
        FSharp """
module RuntimeAsyncMergedDelegates
open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices

type Started<'T> = delegate of unit -> 'T

type Builder() =
    member inline _.Delay([<InlineIfLambda>] code: unit -> 'T) = code
    member inline _.Source(task: Task<'T>) = Started(fun () -> AsyncHelpers.Await task)
    member inline _.MergeSources([<InlineIfLambda>] left: Started<'A>, [<InlineIfLambda>] right: Started<'B>) =
        Started(fun () -> struct (left.Invoke(), right.Invoke()))
    member inline _.Bind([<InlineIfLambda>] source: Started<'T>, [<InlineIfLambda>] next: 'T -> 'U) =
        next (source.Invoke())
    member inline _.Return(value: 'T) = value
    member inline _.Run([<InlineIfLambda>] code: unit -> 'T) : Task<'T> =
        StateMachineHelpers.__runtimeAsyncReturn (code())

let builder = Builder()

let run (left: Task<int>) (right: Task<int>) =
    builder {
        let! x = left
        and! y = right
        return x + y
    }

let runThree (first: Task<int>) (second: Task<int>) (third: Task<int>) =
    builder {
        let! x = first
        and! y = second
        and! z = third
        return x + y + z
    }

[<EntryPoint>]
let main _ =
    let left = TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously)
    let right = TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously)
    let pending = run left.Task right.Task
    if pending.IsCompleted then failwith "Expected a pending result"
    left.SetResult 1
    if pending.IsCompleted then failwith "Right source was not awaited"
    right.SetResult 2
    if pending.GetAwaiter().GetResult() <> 3 then failwith "Wrong result"
    let third = TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously)
    let nested = runThree left.Task right.Task third.Task
    if nested.IsCompleted then failwith "Nested merge completed before the third source"
    third.SetResult 3
    if nested.GetAwaiter().GetResult() <> 6 then failwith "Wrong nested result"
    0
"""
        |> withLangVersionPreview
        |> withFSharpCoreShippedNet
        |> withOptimization optimize
        |> compileExeAndRun
        |> shouldSucceed

    [<InlineData(false)>]
    [<InlineData(true)>]
    [<Theory>]
    let ``runtime async inlines repeated conditional delegate invocations`` (optimize: bool) =
        let library =
            FSharp """
module ImportedDelegateCallbacks
open Microsoft.FSharp.Core.CompilerServices

type Started<'T> = delegate of unit -> 'T

let inline invokeTwice ([<InlineIfLambda>] callback: Started<int>) =
    callback.Invoke() + callback.Invoke()
"""
            |> withName "ImportedDelegateCallbacks"
            |> withLangVersionPreview
            |> withFSharpCoreShippedNet

        FSharp """
module RuntimeAsyncRepeatedDelegate
open System.Collections.Generic
open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices
open ImportedDelegateCallbacks

let mutable constructions = 0
let mutable invocations = 0

let run (gate: Task) deduplicate =
    StateMachineHelpers.__runtimeAsyncReturn (
        invokeTwice (
            if deduplicate then
                constructions <- constructions + 1
                let seen = HashSet<int>()
                Started(fun () ->
                    invocations <- invocations + 1
                    let first = seen.Add 1
                    AsyncHelpers.Await gate
                    if first then 1 else 0)
            else
                Started(fun () -> 2)))

[<EntryPoint>]
let main _ =
    let gate = TaskCompletionSource<unit>(TaskCreationOptions.RunContinuationsAsynchronously)
    let pending = run gate.Task true
    if pending.IsCompleted || constructions <> 1 || invocations <> 1 then
        failwith "Callback construction or first invocation changed"
    gate.SetResult(())
    if pending.GetAwaiter().GetResult() <> 1 || constructions <> 1 || invocations <> 2 then
        failwith "Repeated calls did not share callback state"
    if (run gate.Task false).GetAwaiter().GetResult() <> 4 || constructions <> 1 then
        failwith "Other branch changed"
    0
"""
        |> withLangVersionPreview
        |> withFSharpCoreShippedNet
        |> withOptimization optimize
        |> withReferences [ library ]
        |> compileExeAndRun
        |> shouldSucceed

    [<InlineData(false, false)>]
    [<InlineData(false, true)>]
    [<InlineData(true, false)>]
    [<InlineData(true, true)>]
    [<Theory>]
    let ``runtime async does not rewrite delegates passed to opaque consumers`` (optimize: bool, quoted: bool) =
        let opaqueUse =
            if quoted then
                "let saved = <@ callback.Invoke() @> in saved.ToString() |> ignore"
            else
                "consume callback |> ignore"

        FSharp $"""
module RuntimeAsyncOpaqueDelegate

open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices

type Started<'T> = delegate of unit -> 'T

[<NoCompilerInlining>]
let consume (callback: Started<int>) = callback.Invoke()

let inline invoke ([<InlineIfLambda>] callback: Started<int>) =
    StateMachineHelpers.__runtimeAsyncReturn(
        let result = callback.Invoke()
        {opaqueUse}
        result)

let run (gate: Task<int>) flag =
    invoke (if flag then Started(fun () -> AsyncHelpers.Await gate) else Started(fun () -> 0))
"""
        |> withLangVersionPreview
        |> withFSharpCoreShippedNet
        |> withOptimization optimize
        |> compile
        |> shouldFail
        |> withErrorCode 3918

    [<InlineData(false, false)>]
    [<InlineData(false, true)>]
    [<InlineData(true, false)>]
    [<InlineData(true, true)>]
    [<Theory>]
    let ``runtime async rejects an unmarked delegate method`` (optimize: bool, marked: bool) =
        let invocation = "consume (Started(fun () -> AsyncHelpers.Await gate))"
        let body =
            if marked then
                $"StateMachineHelpers.__runtimeAsyncReturn ({invocation})"
            else
                invocation

        FSharp $"""
module RuntimeAsyncUnmarkedDelegate
open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices

type Started<'T> = delegate of unit -> 'T

[<NoCompilerInlining>]
let consume (callback: Started<int>) = callback.Invoke()

let run (gate: Task<int>) = {body}
"""
        |> withLangVersionPreview
        |> withFSharpCoreShippedNet
        |> withOptimization optimize
        |> compile
        |> shouldFail
        |> withErrorCode 3918

    [<Theory>]
    [<InlineData(false)>]
    [<InlineData(true)>]
    let ``runtime async evaluates a conditional delegate receiver before its argument`` (optimize: bool) =
        FSharp """
module RuntimeAsyncDelegateEvaluationOrder
open System.Collections.Generic
open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices

type Callback = delegate of int -> int

let inline invoke ([<InlineIfLambda>] callback: Callback) (trace: ResizeArray<string>) =
    callback.Invoke(trace.Add "argument"; 3)

let run (gate: Task<int>) flag (trace: ResizeArray<string>) =
    StateMachineHelpers.__runtimeAsyncReturn (
        invoke (
            if flag then
                trace.Add "construction"
                let mutable count = 10
                Callback(fun x ->
                    let value = AsyncHelpers.Await gate
                    count <- count + value + x
                    count)
            else
                trace.Add "other"
                Callback(fun x -> x * 2)) trace)

[<EntryPoint>]
let main _ =
    let gate = TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously)
    let trace = ResizeArray<string>()
    let pending = run gate.Task true trace
    if pending.IsCompleted || trace.ToArray() <> [| "construction"; "argument" |] then
        failwith "Receiver or argument evaluation order changed"
    gate.SetResult 41
    if pending.GetAwaiter().GetResult() <> 54 then failwith "Mutable capture result changed"
    trace.Clear()
    if (run gate.Task false trace).GetAwaiter().GetResult() <> 6
       || trace.ToArray() <> [| "other"; "argument" |] then
        failwith "Other branch evaluation changed"
    0
"""
        |> withLangVersionPreview
        |> withFSharpCoreShippedNet
        |> withOptimization optimize
        |> compileExeAndRun
        |> shouldSucceed
#endif

module RuntimeAsyncDebugTests =
    open Xunit
    open FSharp.Test
    open FSharp.Test.Compiler
    open System.IO
    open System.Reflection.Metadata

#if NETCOREAPP
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

    [<Theory>]
    [<InlineData(false)>]
    [<InlineData(true)>]
    let ``runtime async inlined InlineIfLambda callback keeps its statement debug points`` (optimize: bool) =
        let statementPoints = [ Line 13, Col 9, Line 13, Col 45; Line 14, Col 9, Line 14, Col 27; Line 15, Col 9, Line 15, Col 18 ]
        let callSitePoint = [ Line 12, Col 5, Line 15, Col 19 ]

        FSharp """
module CallbackDebugPoints

open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices

let inline run ([<InlineIfLambda>] body: unit -> int) : Task<int> =
    StateMachineHelpers.__runtimeAsyncReturn (body ())

let work (ready: Task<int>) =
    run (fun () ->
        let value = AsyncHelpers.Await ready
        printfn "%d" value
        value + 1)
"""
        |> withLangVersionPreview
        |> withFSharpCoreShippedNet
        |> withPortablePdb
        |> withOptimization optimize
        |> compile
        |> shouldSucceed
        |> verifyPdb [ VerifyMethodSequencePoints("work", (if optimize then statementPoints else callSitePoint @ statementPoints)) ]

    [<Theory>]
    [<InlineData(false, false)>]
    [<InlineData(false, true)>]
    [<InlineData(true, false)>]
    [<InlineData(true, true)>]
    let ``runtime async conditional callback keeps mutable capture in its lexical scope`` (insideContext: bool, asDelegate: bool) =
        let callbackType, callbackCall, callbackStart, callbackEnd =
            if asDelegate then "Started<int>", "callback.Invoke()", "Started(", ")"
            else "unit -> int", "callback ()", "", ""

        let source = $"""module MissingMutableLocal

open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices
type Started<'T> = delegate of unit -> 'T
let inline invoke ([<InlineIfLambda>] callback: {callbackType}) : Task<int> =
    StateMachineHelpers.__runtimeAsyncReturn ({callbackCall})

let run flag (ready: Task<int>) =
    let mutable count = 99
    invoke (
        if flag then
            printfn "outer = %%d" count
            let mutable count = 0
            printfn "%%d" count
            {callbackStart}fun () ->
                let value = AsyncHelpers.Await ready
                count <- count + value
                count{callbackEnd}
        else
            printfn "else = %%d" count
            {callbackStart}fun () -> 0{callbackEnd})
"""
        let source =
            if insideContext then
                source
                    .Replace(" : Task<int> =", " =")
                    .Replace($"StateMachineHelpers.__runtimeAsyncReturn ({callbackCall})", callbackCall)
                    .Replace("let run flag (ready: Task<int>) =", "let run flag (ready: Task<int>) = StateMachineHelpers.__runtimeAsyncReturn (")
                    .Replace($"fun () -> 0{callbackEnd})", $"fun () -> 0{callbackEnd}))")
            else
                source
        let source = source + """
[<EntryPoint>]
let main _ =
    let gate = TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously)
    let pending = run true gate.Task
    if pending.IsCompleted then failwith "Callback did not suspend"
    gate.SetResult 41
    if pending.GetAwaiter().GetResult() <> 41 then failwith "Mutable capture result changed"
    if (run false gate.Task).GetAwaiter().GetResult() <> 0 then failwith "Other branch result changed"
    0
"""
        let result =
            FSharp source
            |> asExe
            |> withLangVersionPreview
            |> withFSharpCoreShippedNet
            |> withPortablePdb
            |> withNoOptimize
            |> compileExeAndRun
            |> shouldSucceed

        use stream = File.OpenRead(Path.ChangeExtension(result.OutputPath.Value, "pdb"))
        use provider = MetadataReaderProvider.FromPortablePdbStream stream
        let pdb = provider.GetMetadataReader()

        result |> withMetadataReader (fun md ->
            let method =
                md.MethodDefinitions
                |> Seq.filter (fun handle -> md.GetString(md.GetMethodDefinition(handle).Name) = "run")
                |> Assert.Single
            let localsAt (method: MethodDefinitionHandle) line =
                let point =
                    pdb.GetMethodDebugInformation(method).GetSequencePoints()
                    |> Seq.filter (fun point -> not point.IsHidden && point.StartLine = line)
                    |> Assert.Single
                [ for handle in pdb.GetLocalScopes method do
                    let scope = pdb.GetLocalScope handle
                    if scope.StartOffset <= point.Offset && point.Offset < scope.EndOffset then
                        for local in scope.GetLocalVariables() do
                            let variable = pdb.GetLocalVariable local
                            yield pdb.GetString variable.Name, variable ]
            let outerLocals = localsAt method 14
            Assert.DoesNotContain("count (shadowed)", outerLocals |> List.map fst)
            let outer =
                outerLocals
                |> List.filter (fun (name, _) -> name = "count")
                |> Assert.Single
                |> snd
            let inner =
                localsAt method 16
                |> List.filter (fun (name, _) -> name = "count")
                |> Assert.Single
                |> snd
            Assert.NotEqual(outer.Index, inner.Index)
            Assert.Equal(LocalVariableAttributes.None, inner.Attributes)
            let elseLocals = localsAt method 22
            Assert.DoesNotContain("count (shadowed)", elseLocals |> List.map fst)
            let restoredOuter =
                elseLocals
                |> List.filter (fun (name, _) -> name = "count")
                |> Assert.Single
                |> snd
            Assert.Equal(outer.Index, restoredOuter.Index)
            let invocationMethod =
                md.MethodDefinitions
                |> Seq.filter (fun handle ->
                    pdb.GetMethodDebugInformation(handle).GetSequencePoints()
                    |> Seq.exists (fun point -> not point.IsHidden && point.StartLine = 19))
                |> Assert.Single
            let invoked =
                localsAt invocationMethod 19
                |> List.filter (fun (name, _) -> name = "count")
                |> Assert.Single
                |> snd
            Assert.Equal(LocalVariableAttributes.None, invoked.Attributes)
            if insideContext then
                Assert.Equal(inner.Index, invoked.Index))
        if insideContext then
            result |> verifyILNotPresent [ "FSharpRef" ]
#endif
