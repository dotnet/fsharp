module Language.RuntimeAsyncTests

open Xunit
open FSharp.Test
open FSharp.Test.Compiler
open System.IO
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
[<Theory(Skip="not fixed yet")>]
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

[<Fact>]
let ``runtime async is gated without preview when optimization is disabled`` () =
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
