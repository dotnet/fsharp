module Reraise

open System
open System.IO
open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers

let equal expected actual =
    if actual <> expected then failwith $"Expected {expected}, got {actual}"

let same expected actual =
    if not (obj.ReferenceEquals(expected, actual)) then failwith "Exception/result identity changed"

let observe action =
    try Ok(action ()) with error -> Error error

let checkOutcome original expected outcome =
    match expected, outcome with
    | Some value, Ok actual -> equal value actual
    | None, Error error -> same original error
    | _ -> failwith $"Unexpected outcome: {outcome}"

let guard (trace: ResizeArray<string>) name value =
    trace.Add name
    value

let synchronousSelection (original: exn) fallback (trace: ResizeArray<string>) =
    try
        raise original
    with _ ->
        try
            reraise ()
        with
        | :? IOException when guard trace "false" false -> failwith "False guard selected"
        | :? IOException as caught when guard trace "true" true ->
            same original caught
            trace.Add "handler"
            7
        | caught when guard trace "fallback" fallback ->
            same original caught
            trace.Add "fallback-handler"
            -1

let synchronousCleanup (original: exn) (trace: ResizeArray<string>) =
    try
        raise original
    with _ ->
        try
            try
                try reraise () finally trace.Add "inner-finally"
            with :? IOException as caught ->
                same original caught
                trace.Add "handler"
                7
        finally
            trace.Add "outer-finally"

[<NoCompilerInlining>]
let Await () = 0x00001AFE

let filteredReraise (original: exn) (trace: ResizeArray<string>) =
    try raise original
    with caught when guard trace "filter" true ->
        same original caught
        trace.Add(string (Await ()))
        reraise ()

#if !CONTROLS
let recover (original: exn) (audit: Task) : Task<int> =
    __runtimeAsyncReturn (
        try
            raise original
        with _ ->
            AsyncHelpers.Await audit
            try
                reraise ()
            with :? IOException ->
                7)

#if MATRIX
let recoverString (original: exn) (audit: Task) (trace: ResizeArray<string>) : Task<string> =
    __runtimeAsyncReturn (
        try raise original
        with _ ->
            trace.Add "await"
            AsyncHelpers.Await audit
            let n: int =
                try reraise ()
                with :? IOException as caught ->
                    same original caught
                    7
            string n)

let recoverValue (original: exn) (audit: Task) (trace: ResizeArray<string>) (value: 'T) : ValueTask<'T> =
    __runtimeAsyncReturnValueTask (
        try raise original
        with _ ->
            trace.Add "await"
            AsyncHelpers.Await audit
            let n: int =
                try reraise ()
                with :? IOException as caught ->
                    same original caught
                    7
            equal 7 n
            value)

let innerOwner (outer: exn) (inner: exn) (audit: Task) (trace: ResizeArray<string>) : Task<int> =
    __runtimeAsyncReturn (
        try raise outer
        with _ ->
            trace.Add "await"
            AsyncHelpers.Await audit
            try raise inner
            with _ -> reraise ())

let selection (original: exn) fallback (audit: Task) (trace: ResizeArray<string>) : Task<int> =
    __runtimeAsyncReturn (
        try raise original
        with _ ->
            trace.Add "await"
            AsyncHelpers.Await audit
            try
                reraise ()
            with
            | :? IOException when guard trace "false" false -> failwith "False guard selected"
            | :? IOException as caught when guard trace "true" true ->
                same original caught
                trace.Add "handler"
                7
            | caught when guard trace "fallback" fallback ->
                same original caught
                trace.Add "fallback-handler"
                -1)

let cleanup (original: exn) (audit: Task) (trace: ResizeArray<string>) : Task<int> =
    __runtimeAsyncReturn (
        try raise original
        with _ ->
            trace.Add "await"
            AsyncHelpers.Await audit
            try
                try
                    try reraise () finally trace.Add "inner-finally"
                with :? IOException as caught ->
                    same original caught
                    trace.Add "handler"
                    7
            finally
                trace.Add "outer-finally")
#endif

let pending before probe =
    let audit = TaskCompletionSource<unit>(TaskCreationOptions.RunContinuationsAsynchronously)
    let trace = ResizeArray<string>()
    let work: Task<'T> = probe audit.Task trace
    equal before (List.ofSeq trace)
    if audit.Task.IsCompleted || work.IsCompleted then failwith "Expected pending audit and work"
    audit.SetResult(())
    let outcome = observe (fun () -> work.WaitAsync(TimeSpan.FromSeconds 30.).GetAwaiter().GetResult())
    outcome, List.ofSeq trace
#endif

[<EntryPoint>]
let main _ =
    for original in [ IOException() :> exn; InvalidOperationException() ] do
        let matching = original :? IOException
        let expected = if matching then Some 7 else None
        let filteredTrace = ResizeArray<string>()
        observe (fun () -> filteredReraise original filteredTrace) |> checkOutcome original None
        equal "6910" filteredTrace.[filteredTrace.Count - 1]
        if filteredTrace.Count < 2 || Seq.exists ((<>) "filter") (Seq.take (filteredTrace.Count - 1) filteredTrace) then
            failwith $"Unexpected filter trace: {filteredTrace}"
        let cleanupTrace = ResizeArray<string>()
        observe (fun () -> synchronousCleanup original cleanupTrace) |> checkOutcome original expected
        equal
            (if matching then ["inner-finally"; "handler"; "outer-finally"] else ["inner-finally"; "outer-finally"])
            (List.ofSeq cleanupTrace)
        for fallback in [false; true] do
            let selected = if matching then Some 7 elif fallback then Some -1 else None
            let syncTrace = ResizeArray<string>()
            observe (fun () -> synchronousSelection original fallback syncTrace) |> checkOutcome original selected
#if MATRIX
            let outcome, trace = pending ["await"] (selection original fallback)
            checkOutcome original selected outcome
            equal ("await" :: List.ofSeq syncTrace) trace
#endif
#if !CONTROLS
        pending [] (fun audit _ -> recover original audit) |> fst |> checkOutcome original expected
#endif
#if MATRIX
        pending ["await"] (recoverString original) |> fst |> checkOutcome original (Option.map string expected)
        let inner = ArgumentException("inner")
        pending ["await"] (innerOwner original inner) |> fst |> checkOutcome inner None
        let outcome, trace = pending ["await"] (cleanup original)
        checkOutcome original expected outcome
        equal ("await" :: List.ofSeq cleanupTrace) trace
        let valueResult, _ = pending ["await"] (fun audit trace -> (recoverValue original audit trace 42).AsTask())
        checkOutcome original (Option.map (fun _ -> 42) expected) valueResult
        let reference = obj()
        let referenceResult, _ = pending ["await"] (fun audit trace -> (recoverValue original audit trace reference).AsTask())
        checkOutcome original (Option.map (fun _ -> reference) expected) referenceResult
        match referenceResult with
        | Ok actual -> same reference actual
        | Error _ -> ()
#endif
    0
