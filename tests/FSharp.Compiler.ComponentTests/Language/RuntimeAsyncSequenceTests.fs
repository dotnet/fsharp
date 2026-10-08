module Language.RuntimeAsyncSequenceTests

open System.IO
open System.Text.RegularExpressions
open FSharp.Test
open FSharp.Test.Compiler
open Xunit

#if NETCOREAPP
open Language.RuntimeAsyncTests

let private source name = Path.Combine(__SOURCE_DIRECTORY__, "RuntimeAsync", name)
let private preview compilation = compilation |> withLangVersionPreview |> withFSharpCoreShippedNet
let private optimize enabled compilation = if enabled then withOptimize compilation else withOptions ["--optimize-"] compilation

[<Theory>]
[<InlineData(false, false)>]
[<InlineData(false, true)>]
[<InlineData(true, false)>]
[<InlineData(true, true)>]
let ``runtime async sequence library executes`` (crossAssembly: bool, optimized: bool) =
    let entry = FsSource "RuntimeAsyncSequence.run().Wait()"
    (if crossAssembly then
         let builders = withSampleBuilders [] |> optimize optimized |> asLibrary
         FsFromPath(source "RuntimeAsyncSequence.fs") |> withAdditionalSourceFile entry |> withReferences [builders]
     else
         withSampleBuilders [ "RuntimeAsyncSequence.fs" ] |> withAdditionalSourceFile entry)
    |> preview
    |> optimize optimized
    |> compileExeAndRun
    |> shouldSucceed

let private header = """
module M
open System.Runtime.CompilerServices
open System.Threading
open System.Threading.Tasks
open Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers
"""

[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``runtime async sequence reuses its first enumeration and dispatches through the base`` optimized =
    let program = header + """
open System
open System.Collections.Generic

let check message condition = if not condition then failwith message

[<EntryPoint>]
let main _ =
    let source = __runtimeAsyncSequence(fun _ -> seq { yield 1 })
    let iterator = source.GetAsyncEnumerator()
    check "reuse first enumeration" (obj.ReferenceEquals(source, iterator))
    let fresh = source.GetAsyncEnumerator()
    let nested = (fresh :?> IAsyncEnumerable<int>).GetAsyncEnumerator()
    check "fresh clones are already acquired" (not (obj.ReferenceEquals(iterator, fresh)) && not (obj.ReferenceEquals(fresh, nested)))
    let generated = iterator.GetType()
    for interfaceType in [ typeof<IAsyncEnumerator<int>>; typeof<IAsyncDisposable> ] do
        let targets = generated.GetInterfaceMap(interfaceType).TargetMethods
        check "dispatch through the base" (targets |> Array.forall (fun m -> m.DeclaringType = generated.BaseType))
    let concurrent = __runtimeAsyncSequence(fun _ -> seq { yield 1 })
    let iterators = Task.WhenAll(Array.init 8 (fun _ -> Task.Run(fun () -> concurrent.GetAsyncEnumerator()))).Result
    check "independent acquisitions" (HashSet(iterators, HashIdentity.Reference).Count = 8)
    check "one first-enumeration reuse" ((iterators |> Array.filter (fun i -> obj.ReferenceEquals(concurrent, i))).Length = 1)
    0
"""
    FSharp program
    |> preview
    |> optimize optimized
    |> compileExeAndRun
    |> shouldSucceed

[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``runtime async input evaluation precedes throwing projections`` optimized =
    FSharp """
module PrefixOrder
open System
open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices

let events = ResizeArray<string>()
[<NoCompilerInlining; MethodImpl(MethodImplOptions.NoInlining)>]
let source () = events.Add "source"; Task.FromResult 42
type Holder = { Pair: int * int }
[<NoCompilerInlining>]
let probe (holder: Holder) =
    StateMachineHelpers.__runtimeAsyncReturn (
        let input = source()
        (fst holder.Pair, AsyncHelpers.Await input))

[<EntryPoint>]
let main _ =
    for holder, shouldThrow in
        [{ Pair = (1, 2) }, false
         { Pair = Unchecked.defaultof<int * int> }, true
         Unchecked.defaultof<Holder>, true] do
        events.Clear()
        let mutable threw = false
        try
            let result = (probe holder).GetAwaiter().GetResult()
            if result <> (1, 42) then failwith "result"
        with :? NullReferenceException -> threw <- true
        if threw <> shouldThrow then failwith "exception"
        if List.ofSeq events <> ["source"] then failwith "source must precede projection"
    0
"""
    |> preview
    |> optimize optimized
    |> compileExeAndRun
    |> shouldSucceed

[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``runtime async sequence pipe input compiles`` optimized =
    FSharp """
module RuntimeAsyncSequencePipeInput

open System.Threading
open System.Threading.Tasks
open System.Collections.Generic
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers

let consume (source: IAsyncEnumerable<int>) : Task<int> =
    StateMachineHelpers.__runtimeAsyncReturn (
        let iterator = source.GetAsyncEnumerator(CancellationToken.None)
        let moved = AsyncHelpers.Await(iterator.MoveNextAsync())
        if moved then iterator.Current else -1)

let run () : Task<int> =
    StateMachineHelpers.__runtimeAsyncReturn (
        AsyncHelpers.Await (
            __runtimeAsyncSequence (fun _ -> seq { yield 42 })
            |> consume))

[<EntryPoint>]
let main _ = if run().GetAwaiter().GetResult() = 42 then 0 else 1
"""
    |> preview
    |> optimize optimized
    |> compileExeAndRun
    |> shouldSucceed

[<Theory>]
[<InlineData(false, "value")>]
[<InlineData(true, "value")>]
[<InlineData(false, "value + 1")>]
[<InlineData(true, "value + 1")>]
[<InlineData(false, "value + value")>]
[<InlineData(true, "value + value")>]
let ``runtime async sequence keeps source calls adjacent to awaits`` (optimized, resultExpression) =
    let inputs =
        CSharp "public static class AwaitInputs { public static System.Threading.Tasks.Task<int> Next() => System.Threading.Tasks.Task.FromResult(42); }"
        |> withName "AwaitInputs"
    let builders = withSampleBuilders [] |> asLibrary
    let result =
        FSharp(header + "\nopen Microsoft.FSharp.Control\nlet values () = asyncSeq2 { let! value = AwaitInputs.Next() in yield " + resultExpression + " }")
        |> withReferences [inputs; builders]
        |> preview
        |> optimize optimized
        |> compile
        |> shouldSucceed
    match result with
    | CompilationResult.Success output ->
        let _, _, il = ILChecker.verifyILAndReturnActual [] output.OutputPath.Value []
        let start = il.IndexOf("AwaitInputs::Next()")
        let finish = il.IndexOf("AsyncHelpers::Await", start)
        Assert.True(start >= 0 && finish > start)
        Assert.DoesNotContain("stfld", il.Substring(start, finish - start))
        Assert.DoesNotContain("stloc", il.Substring(start, finish - start))
        Assert.DoesNotContain("ldloc", il.Substring(start, finish - start))
    | _ -> failwith "expected compiled sequence"

[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``runtime async sequence isolates ordinary sources`` optimized =
    let body = """
let factory (work: Task<int>) = __runtimeAsyncSequence(fun _ -> seq {
    for value in seq { try yield 1 with _ -> yield 2 } do
        yield value + AsyncHelpers.Await work
})
"""
    let result = FSharp(header + body) |> preview |> optimize optimized |> compile |> shouldSucceed
    result |> withMetadataReader (fun md ->
        let names = [for handle in md.MethodDefinitions -> md.GetString(md.GetMethodDefinition(handle).Name)]
        Assert.Single(names |> List.filter ((=) "MoveNextAsync")) |> ignore
        Assert.DoesNotContain("DisposeAsync", names)
        for handle in md.MethodDefinitions do
            let method = md.GetMethodDefinition handle
            let name = md.GetString method.Name
            if name = "MoveNextAsync" then
                Assert.Equal(0x2000, int method.ImplAttributes &&& 0x2000)
                let mutable signature = md.GetBlobReader method.Signature
                Assert.False(signature.ReadSignatureHeader().IsGeneric)
                Assert.Equal(0, signature.ReadCompressedInteger())
            elif name = "factory" || name = "GenerateNext" || name = "Close" then
                Assert.Equal(0, int method.ImplAttributes &&& 0x2000))
    match result with
    | CompilationResult.Success output ->
        let _, _, il = ILChecker.verifyILAndReturnActual [] output.OutputPath.Value []
        let methodBody =
            Regex.Match(il, @"(?ms)^(?<indent>[ \t]*)\.method[^\n]*MoveNextAsync\(\)[^\n]*\n\k<indent>\{.*?^\k<indent>\}").Value
        Assert.Contains("ValueTask`1<bool>", methodBody)
        Assert.Contains("AsyncHelpers::Await<int32>", methodBody)
        Assert.DoesNotContain("__runtimeAsyncSequence", il)
    | _ -> failwith "expected compiled sequence"

[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``runtime async sequence handles inline producer try with`` optimized =
    let body = """
let inline test () = seq {
    try
        yield 1
        failwith "error"
    with
    | _ ->
        yield -1
}

let sequence = __runtimeAsyncSequence test

[<EntryPoint>]
let main _ =
    let iterator = sequence.GetAsyncEnumerator()
    let first = iterator.MoveNextAsync().GetAwaiter().GetResult()
    let firstValue = iterator.Current
    let second = iterator.MoveNextAsync().GetAwaiter().GetResult()
    let secondValue = iterator.Current
    let finished = iterator.MoveNextAsync().GetAwaiter().GetResult()
    iterator.DisposeAsync().GetAwaiter().GetResult()
    if not first || firstValue <> 1 || not second || secondValue <> -1 || finished then
        failwith "unexpected sequence values"
    0
"""

    FSharp(header + body)
    |> preview
    |> optimize optimized
    |> compileExeAndRun
    |> shouldSucceed

[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``runtime async sequence awaits in producer try with`` optimized =
    let body = """
open System
open Microsoft.FSharp.Core.CompilerServices

let sequence (work: Task<int>) = __runtimeAsyncSequence(fun _ -> seq {
    try
        yield 1
        yield AsyncHelpers.Await work
    with :? InvalidOperationException ->
        yield -1
})

[<EntryPoint>]
let main _ =
    let source = sequence (Task.FromException<int>(InvalidOperationException()))
    let iterator = source.GetAsyncEnumerator()
    let first = iterator.MoveNextAsync().GetAwaiter().GetResult()
    let value = iterator.Current
    let second = iterator.MoveNextAsync().GetAwaiter().GetResult()
    let handled = iterator.Current
    let done_ = iterator.MoveNextAsync().GetAwaiter().GetResult()
    iterator.DisposeAsync().GetAwaiter().GetResult()
    if first && value = 1 && second && handled = -1 && not done_ then 0 else 1
"""
    FSharp(header + body)
    |> preview
    |> optimize optimized
    |> compileExeAndRun
    |> shouldSucceed

[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``runtime async sequence awaits in exception handler`` optimized =
    let body = """
module M
open System
open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers

let sequence (work: Task<int>) = __runtimeAsyncSequence(fun _ -> seq {
    try
        yield 1
        raise (InvalidOperationException())
    with
    | :? InvalidOperationException ->
        yield AsyncHelpers.Await work
        yield 3
})

[<EntryPoint>]
let main _ =
    let iterator = (sequence (Task.FromResult 2)).GetAsyncEnumerator()
    let values = ResizeArray<int>()
    while iterator.MoveNextAsync().GetAwaiter().GetResult() do
        values.Add(iterator.Current)
    iterator.DisposeAsync().GetAwaiter().GetResult()
    if Seq.toList values = [1; 2; 3] then 0 else 1
"""
    FSharp body |> preview |> optimize optimized |> compileExeAndRun |> shouldSucceed

[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``runtime async sequence preserves handler filtering and cleanup`` optimized =
    let body = """
module M
open System
open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers

let mutable closed = 0
let sequence (work: Task<int>) = __runtimeAsyncSequence(fun _ -> seq {
    try
        try
            yield AsyncHelpers.Await work
        finally
            closed <- closed + 1
    with
    | :? InvalidOperationException -> yield -1
})

[<EntryPoint>]
let main _ =
    let source = sequence (Task.FromException<int>(InvalidOperationException()))
    let iterator = source.GetAsyncEnumerator()
    let moved = iterator.MoveNextAsync().GetAwaiter().GetResult()
    let value = iterator.Current
    iterator.DisposeAsync().GetAwaiter().GetResult()
    if moved && value = -1 && closed = 1 then 0 else 1
"""
    FSharp body |> preview |> optimize optimized |> compileExeAndRun |> shouldSucceed

[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``runtime async sequence awaits source cleanup before handling an error`` optimized =
    let body = """
open System
open Microsoft.FSharp.Core.CompilerServices

let mutable closed = false
let sequence (work: Task<int>) (cleanup: Task) = __runtimeAsyncSequence(fun _ -> seq {
    try
        try
            yield AsyncHelpers.Await work
        finally
            AsyncHelpers.Await cleanup
            closed <- true
    with :? InvalidOperationException ->
        yield -1
})

[<EntryPoint>]
let main _ =
    let gate = TaskCompletionSource<unit>(TaskCreationOptions.RunContinuationsAsynchronously)
    let iterator = (sequence (Task.FromException<int>(InvalidOperationException())) gate.Task).GetAsyncEnumerator()
    let pending = iterator.MoveNextAsync()
    if pending.IsCompleted || closed then failwith "cleanup should be pending"
    gate.SetResult()
    if not (pending.GetAwaiter().GetResult()) || iterator.Current <> -1 || not closed then
        failwith "handler ran before cleanup"
    if iterator.MoveNextAsync().GetAwaiter().GetResult() then failwith "unexpected element"
    iterator.DisposeAsync().GetAwaiter().GetResult()
    0
"""
    FSharp(header + body) |> preview |> optimize optimized |> compileExeAndRun |> shouldSucceed

[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``runtime async sequence guards pending moves`` optimized =
    let body = """
let sequence (gate: Task) = __runtimeAsyncSequence(fun _ -> seq {
    AsyncHelpers.Await gate
    yield 1
    AsyncHelpers.Await gate
    yield 2
})

let rejects (action: unit -> unit) =
    try action (); false with :? InvalidOperationException -> true

[<EntryPoint>]
let main _ =
    let gate = TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously)
    let iterator = (sequence gate.Task).GetAsyncEnumerator()
    let move = iterator.MoveNextAsync()
    if move.IsCompleted then failwith "move should be pending"
    if not (rejects (fun () -> iterator.MoveNextAsync() |> ignore)) then failwith "concurrent move accepted"
    if not (rejects (fun () -> iterator.DisposeAsync() |> ignore)) then failwith "pending disposal accepted"
    gate.SetResult()
    if not (move.AsTask().Result && iterator.Current = 1) then failwith "first value"
    if not (iterator.MoveNextAsync().AsTask().Result && iterator.Current = 2) then failwith "second value"

    let failing = TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously)
    let faulted = (sequence failing.Task).GetAsyncEnumerator()
    let move = faulted.MoveNextAsync()
    failing.SetException(InvalidOperationException())
    if not (rejects (fun () -> move.AsTask().GetAwaiter().GetResult() |> ignore)) then failwith "fault lost"
    if faulted.MoveNextAsync().AsTask().Result then failwith "faulted sequence resumed"
    faulted.DisposeAsync().AsTask().Wait()
    iterator.DisposeAsync().AsTask().Wait()
    0
"""
    FSharp(header + "open System\n" + body) |> preview |> optimize optimized |> compileExeAndRun |> shouldSucceed

[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``runtime async sequence handles cleanup errors in try with`` optimized =
    let body = """
module M
open System
open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers

let sequence (work: Task<int>) (cleanupError: exn) = __runtimeAsyncSequence(fun _ -> seq {
    try
        try
            yield AsyncHelpers.Await work
        finally
            raise cleanupError
    with
    | :? InvalidOperationException as error -> yield if error.Message = "cleanup" then -2 else -1
})

[<EntryPoint>]
let main _ =
    let iterator =
        (sequence
            (Task.FromException<int>(InvalidOperationException("body")))
            (InvalidOperationException("cleanup"))).GetAsyncEnumerator()
    let moved = iterator.MoveNextAsync().GetAwaiter().GetResult()
    let value = iterator.Current
    iterator.DisposeAsync().GetAwaiter().GetResult()
    if not moved || value <> -2 then failwith "matched cleanup error"
    let iterator =
        (sequence
            (Task.FromException<int>(InvalidOperationException("body")))
            (ApplicationException("cleanup"))).GetAsyncEnumerator()
    let raised =
        try
            iterator.MoveNextAsync().GetAwaiter().GetResult() |> ignore
            false
        with :? ApplicationException as error when error.Message = "cleanup" -> true
    iterator.DisposeAsync().GetAwaiter().GetResult()
    if raised then 0 else 1
"""
    FSharp body |> preview |> optimize optimized |> compileExeAndRun |> shouldSucceed

[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``runtime async sequence handles suspended filter and unmatched errors`` optimized =
    let body = """
module M
open System
open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers

let sequence (work: Task<int>) (guard: Task<bool>) = __runtimeAsyncSequence(fun _ -> seq {
    try
        yield AsyncHelpers.Await work
    with
    | :? InvalidOperationException when AsyncHelpers.Await guard -> yield 42
    | :? ArgumentException -> yield -1
})

[<MethodImpl(MethodImplOptions.NoInlining)>]
let raiseOrigin () : int = raise (InvalidOperationException("origin"))

[<EntryPoint>]
let main _ =
    for work, guard, expected in
        [ Task.FromResult 3, Task.FromResult true, 3
          Task.FromException<int>(InvalidOperationException()), Task.FromResult true, 42
          Task.FromException<int>(ArgumentException()), Task.FromResult false, -1 ] do
        let iterator = (sequence work guard).GetAsyncEnumerator()
        if not (iterator.MoveNextAsync().GetAwaiter().GetResult()) || iterator.Current <> expected then
            failwith "unexpected value"
        if iterator.MoveNextAsync().GetAwaiter().GetResult() then failwith "unexpected extra value"
        iterator.DisposeAsync().GetAwaiter().GetResult()
    let error =
        try raiseOrigin () |> ignore; failwith "expected source error"
        with :? InvalidOperationException as error -> error
    let iterator = (sequence (Task.FromException<int> error) (Task.FromResult false)).GetAsyncEnumerator()
    try
        iterator.MoveNextAsync().GetAwaiter().GetResult() |> ignore
        failwith "unmatched error was swallowed"
    with :? InvalidOperationException as caught when obj.ReferenceEquals(error, caught) ->
        if not (caught.StackTrace.Contains("raiseOrigin")) then failwith "lost original stack"
    iterator.DisposeAsync().GetAwaiter().GetResult()
    let filterGate = TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously)
    let iterator = (sequence (Task.FromException<int>(InvalidOperationException())) filterGate.Task).GetAsyncEnumerator()
    let pending = iterator.MoveNextAsync()
    if pending.IsCompleted then failwith "filter should be pending"
    filterGate.SetResult true
    if not (pending.GetAwaiter().GetResult()) || iterator.Current <> 42 then
        failwith "suspended filter"
    iterator.DisposeAsync().GetAwaiter().GetResult()
    0
"""
    FSharp body |> preview |> optimize optimized |> compileExeAndRun |> shouldSucceed

[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``runtime async try with runs cleanup once on guard and handler faults`` optimized =
    let body = """
module M
open System
open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers

let mutable disposed = 0
let sequence (guard: Task<bool>) = __runtimeAsyncSequence(fun _ -> seq {
    try
        try
            yield AsyncHelpers.Await(Task.FromException<int>(InvalidOperationException("source")))
        finally
            disposed <- disposed + 1
    with :? InvalidOperationException when AsyncHelpers.Await guard ->
        yield AsyncHelpers.Await(Task.FromException<int>(ApplicationException("handler")))
})

[<EntryPoint>]
let main _ =
    for guard, expected in
        [ Task.FromException<bool>(ApplicationException("guard")), "guard"
          Task.FromResult false, "source"
          Task.FromResult true, "handler" ] do
        disposed <- 0
        let iterator = (sequence guard).GetAsyncEnumerator()
        let message =
            try
                iterator.MoveNextAsync().GetAwaiter().GetResult() |> ignore
                failwith "expected a fault"
            with
            | :? ApplicationException as error -> error.Message
            | :? InvalidOperationException as error -> error.Message
        if message <> expected || disposed <> 1 then failwithf "incorrect fault cleanup: %s" message
        if iterator.MoveNextAsync().GetAwaiter().GetResult() then failwith "faulted iterator restarted"
        iterator.DisposeAsync().GetAwaiter().GetResult()
        if disposed <> 1 then failwith "double disposal"
    0
"""
    FSharp body |> preview |> optimize optimized |> compileExeAndRun |> shouldSucceed

[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``runtime async sequence lowers direct try with calls`` optimized =
    let body = """
module M
open System
open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers

let mutable guards = 0
let handle (_: exn) = seq { yield 42 }
let source (value: int) =
    seq {
        if value = 1 then raise (InvalidOperationException())
        if value = 2 then raise (ArgumentException())
        yield value
    }

let direct (value: int) (gate: Task<bool>) = __runtimeAsyncSequence(fun _ ->
    RuntimeHelpers.EnumerateTryWith (source value) (fun e -> if AsyncHelpers.Await gate && (e :? InvalidOperationException) then 1 else 0) handle)

let expression (work: Task<int>) (gate: Task<bool>) = __runtimeAsyncSequence(fun _ -> seq {
    try
        yield AsyncHelpers.Await work
    with :? InvalidOperationException when (guards <- guards + 1; AsyncHelpers.Await gate) -> yield 42
})

let first (values: Collections.Generic.IAsyncEnumerable<int>) =
    let iterator = values.GetAsyncEnumerator()
    try
        try
            if iterator.MoveNextAsync().GetAwaiter().GetResult() then string iterator.Current else "end"
        with error -> error.GetType().Name
    finally
        iterator.DisposeAsync().GetAwaiter().GetResult()

[<EntryPoint>]
let main _ =
    let fault () = Task.FromException<int>(InvalidOperationException())
    let results =
        [ first (direct 3 (Task.FromResult true))
          first (direct 1 (Task.FromResult true))
          first (direct 1 (Task.FromResult false))
          first (direct 2 (Task.FromResult true))
          first (expression (fault ()) (Task.FromResult true)) ]
    if results <> [ "3"; "42"; "InvalidOperationException"; "ArgumentException"; "42" ] then failwithf "%A" results
    if guards <> 1 then failwithf "guard ran %d times" guards
    0
"""
    FSharp body |> preview |> optimize optimized |> compileExeAndRun |> shouldSucceed

[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``runtime async sequence disposal runs pending finally blocks but no handlers`` optimized =
    let body = """
module M
open System
open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers

let log = ResizeArray<string>()
let sequence (cleanup: Task) fail = __runtimeAsyncSequence(fun _ -> seq {
    try
        try
            try
                yield 1
            finally
                AsyncHelpers.Await cleanup
                log.Add "inner"
                if fail then raise (InvalidOperationException("inner"))
        with _ ->
            log.Add "handler"
            yield -1
    finally
        log.Add "outer"
})

[<EntryPoint>]
let main _ =
    for fail in [ false; true ] do
        log.Clear()
        let gate = TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously)
        let iterator = (sequence gate.Task fail).GetAsyncEnumerator()
        if not (iterator.MoveNextAsync().GetAwaiter().GetResult()) || iterator.Current <> 1 then failwith "first value"
        let disposal = iterator.DisposeAsync()
        if disposal.IsCompleted then failwith "cleanup should be pending"
        gate.SetResult()
        let message =
            try disposal.GetAwaiter().GetResult(); "" with :? InvalidOperationException as error -> error.Message
        if message <> (if fail then "inner" else "") || List.ofSeq log <> [ "inner"; "outer" ] then
            failwithf "disposal: %s %A" message log
        if iterator.MoveNextAsync().GetAwaiter().GetResult() then failwith "disposed iterator resumed"
        iterator.DisposeAsync().GetAwaiter().GetResult()
        if log.Count <> 2 then failwith "cleanup ran twice"
    0
"""
    FSharp body |> preview |> optimize optimized |> compileExeAndRun |> shouldSucceed

[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``runtime async sequence handles nested try with and early disposal`` optimized =
    let body = """
module M
open System
open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers

let mutable closed = 0
let sequence (work: Task<int>) = __runtimeAsyncSequence(fun _ -> seq {
    try
        try
            try
                yield AsyncHelpers.Await work
            finally
                closed <- closed + 1
        with :? ArgumentException ->
            yield 1
    with :? InvalidOperationException ->
        yield 2
})

[<EntryPoint>]
let main _ =
    let iterator = (sequence (Task.FromException<int>(InvalidOperationException()))).GetAsyncEnumerator()
    if not (iterator.MoveNextAsync().GetAwaiter().GetResult()) || iterator.Current <> 2 || closed <> 1 then
        failwith "outer handler"
    if iterator.MoveNextAsync().GetAwaiter().GetResult() then failwith "unexpected element"
    iterator.DisposeAsync().GetAwaiter().GetResult()
    let iterator = (sequence (Task.FromException<int>(ArgumentException()))).GetAsyncEnumerator()
    if not (iterator.MoveNextAsync().GetAwaiter().GetResult()) || iterator.Current <> 1 || closed <> 2 then
        failwith "inner handler"
    iterator.DisposeAsync().GetAwaiter().GetResult()
    let iterator = (sequence (Task.FromResult 42)).GetAsyncEnumerator()
    if not (iterator.MoveNextAsync().GetAwaiter().GetResult()) || iterator.Current <> 42 then
        failwith "ordinary yield"
    iterator.DisposeAsync().GetAwaiter().GetResult()
    if closed <> 3 then failwith "early disposal"
    0
"""
    FSharp body |> preview |> optimize optimized |> compileExeAndRun |> shouldSucceed

[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``runtime async sequence rejects overlapping moves and releases the guard`` optimized =
    let body = """
open System
open Microsoft.FSharp.Core.CompilerServices

let sequence (work: Task<int>) = __runtimeAsyncSequence(fun _ -> seq {
    yield AsyncHelpers.Await work
})

[<EntryPoint>]
let main _ =
    for fault in [false; true] do
        let gate = TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously)
        let iterator = (sequence gate.Task).GetAsyncEnumerator()
        let pending = iterator.MoveNextAsync()
        if pending.IsCompleted then failwith "expected pending move"
        let rejected =
            try iterator.MoveNextAsync() |> ignore; false
            with :? InvalidOperationException -> true
        if not rejected then failwith "overlapping move was accepted"
        if fault then gate.SetException(ApplicationException("failed"))
        else gate.SetResult 42
        if fault then
            try
                pending.GetAwaiter().GetResult() |> ignore
                failwith "expected failure"
            with :? ApplicationException -> ()
        elif not (pending.GetAwaiter().GetResult()) || iterator.Current <> 42 then
            failwith "expected value"
        if iterator.MoveNextAsync().GetAwaiter().GetResult() then
            failwith "expected completed enumeration"
        iterator.DisposeAsync().GetAwaiter().GetResult()
    0
"""
    FSharp(header + body) |> preview |> optimize optimized |> compileExeAndRun |> shouldSucceed

[<Theory>]
[<InlineData("preview", 3922, "let opaque (recipe: unit -> seq<int>) = __runtimeAsyncSequence recipe")>]
[<InlineData("preview", 3922, "let tail (input: seq<int>) = __runtimeAsyncSequence(fun _ -> seq { yield 1; yield! input })")>]
[<InlineData("preview", 3918, "let nested () = __runtimeAsyncSequence(fun _ -> seq { for n in seq { yield AsyncHelpers.Await(Task.FromResult 1) } do yield n })")>]
[<InlineData("9.0", 3350, "let sequence () = __runtimeAsyncSequence(fun _ -> seq { yield 1 })")>]
let ``runtime async sequence rejects unsupported entries`` (language: string, code: int, body: string) =
    FSharp(header + body)
    |> withFSharpCoreShippedNet
    |> withLangVersion language
    |> compile
    |> shouldFail
    |> withErrorCode code
#endif
