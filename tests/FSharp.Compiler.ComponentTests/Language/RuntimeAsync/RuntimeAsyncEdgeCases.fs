// Runtime-behavior edge cases for runtime-async, exercised through the runtimeTask CE builder
// (RuntimeTaskBuilder.fs, treated here as a hypothetical library). Verified to run green on the
// pinned net11 preview runtime. Referenced from RuntimeAsyncEdgeCaseTests.fs via compileExeAndRun.
module RuntimeAsyncEdgeCases

open System
open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices
open RuntimeTaskBuilder.RuntimeTask

let private delayed v = Task.Delay(1).ContinueWith(fun (_: Task) -> v)
let private resultOf (t: Task<'T>) = t.GetAwaiter().GetResult()

// locals: a normal local is hoisted by the JIT and preserved across a suspension.
let normalLocalAcross () : Task<int> =
    runtimeTask {
        let captured = 40
        let! delta = delayed 2
        return captured + delta
    }

// loops: await inside both a while body and a for body; results checked separately.
let loopsAcrossAwait () : Task<int * int> =
    runtimeTask {
        let mutable whileAcc = 0
        let mutable i = 0
        while i < 3 do
            let! x = delayed 1
            whileAcc <- whileAcc + x
            i <- i + 1
        let mutable forAcc = 0
        for x in [ 1; 2; 3 ] do
            let! y = delayed x
            forAcc <- forAcc + y
        return (whileAcc, forAcc)
    }

// a non-ref (readonly) value struct is hoisted and keeps its fields across a suspension, unlike the
// ref-struct/ReadOnlySpan case which is contract-forbidden.
[<Struct>]
type Pair = { A: int; B: int }

let structAcross () : Task<int> =
    runtimeTask {
        let p = { A = 20; B = 22 }
        let! _ = delayed 0
        return p.A + p.B
    }

// operand: change a bound source from Task to ValueTask keeps working.
let awaitValueTask () : Task<int> =
    runtimeTask {
        let! x = ValueTask<int>(41)
        return x + 1
    }

// exception thrown after a suspension surfaces through the returned Task.
let exnAfterAwait () : Task<int> =
    runtimeTask {
        let! _ = delayed 1
        return failwith "boom"
    }

// use on an IAsyncDisposable: the builder's Using hoists DisposeAsync out of the finally,
// so disposal (which suspends) never runs inside an EH region. DisposeAsync genuinely suspends
// before recording the disposal, so a passing `sink = 1` proves the builder awaited it.
type AsyncProbe(sink: int ref) =
    interface IAsyncDisposable with
        member _.DisposeAsync() =
            ValueTask(Task.Delay(1).ContinueWith(fun (_: Task) -> sink.Value <- 1))

let useAsyncDisposable (sink: int ref) : Task<int> =
    runtimeTask {
        use _p = new AsyncProbe(sink)
        let! x = delayed 7
        return x
    }

[<EntryPoint>]
let main _ =
    let mutable failures = 0
    let check name cond = if not cond then eprintfn "FAILED: %s" name; failures <- failures + 1

    check "normalLocalAcross" (resultOf (normalLocalAcross ()) = 42)
    let (whileAcc, forAcc) = resultOf (loopsAcrossAwait ())
    check "awaitInWhile" (whileAcc = 3)
    check "awaitInFor" (forAcc = 6)
    check "structAcross" (resultOf (structAcross ()) = 42)
    check "awaitValueTask" (resultOf (awaitValueTask ()) = 42)

    let threw =
        try resultOf (exnAfterAwait ()) |> ignore; false
        with _ -> true
    check "exnAfterAwait propagates" threw

    let sink = ref 0
    check "useAsyncDisposable result" (resultOf (useAsyncDisposable sink) = 7)
    check "useAsyncDisposable disposed" (sink.Value = 1)

    if failures = 0 then 0 else 1
