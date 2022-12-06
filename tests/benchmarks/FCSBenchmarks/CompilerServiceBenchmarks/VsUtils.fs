/// Stuff taken from VsIntegration
module FSharp.Benchmarks.VsUtils

open System
open System.Threading
open System.Threading.Tasks

let rec private interleave' result seqs =
    match seqs with
    | [] -> result
    | _ ->
        let items, newSeqs =
            [ for s in seqs do
                match s |> Seq.tryHead with
                | Some item -> item, Seq.tail s
                | None -> () ]
            |> List.unzip

        interleave' (Seq.append result items) newSeqs

/// Combines sequences of varying lengths into one by interleaving the elements
/// E.g. aaaaa bb ccc -> abcabcacaa
let interleave seqs = seqs |> Seq.toList |> interleave' Seq.empty


type VolatileBarrier() =
    [<VolatileField>]
    let mutable isStopped = false
    member _.Proceed = not isStopped
    member _.Stop() = isStopped <- true

// This is like Async.StartAsTask, but
//  1. If cancellation occurs we explicitly associate the cancellation with cancellationToken
//  2. If exception occurs then set result to Unchecked.defaultof<_>, i.e. swallow exceptions
//     and hope that Roslyn copes with the null
//  3. Never, ever run the computation on the UI thread - switch to thread pool if necessary
//     This is because Roslyn makes blocking invocations of tasks from the UI thread at
//     several points, and relies on those tasks completing in the thread pool if necessary.
//     See for example https://github.com/dotnet/fsharp/issues/11946#issuecomment-896071454
//     Note that no async { ... } code in FSharp.Editor is ever intended to run on the UI
//     thread in any form. That is, our async code in FSharp.Editor is always "backgroundAsync"
//     in the sense that it is valid switch away from the UI thread if it is ever started on
//     the UI thread, e.g. see the corresponding backgroundTask { ... } in RFC FS-1097

let StartAsyncAsTask (cancellationToken: CancellationToken) computation =
    // Protect against blocking the UI thread by switching to thread pool
    let computation =
        match SynchronizationContext.Current with
        | null -> computation
        | _ ->
            async {
                do! Async.SwitchToThreadPool()
                return! computation
            }
    let tcs = new TaskCompletionSource<_>(TaskCreationOptions.None)
    let barrier = VolatileBarrier()
    let reg = cancellationToken.Register(fun _ -> if barrier.Proceed then tcs.TrySetCanceled(cancellationToken) |> ignore)
    let task = tcs.Task
    let disposeReg() = barrier.Stop(); if not task.IsCanceled then reg.Dispose()
    Async.StartWithContinuations(
              computation,
              continuation=(fun result ->
                  disposeReg()
                  tcs.TrySetResult(result) |> ignore
              ),
              exceptionContinuation=(fun exn ->
                  disposeReg()
                  match exn with
                  | :? OperationCanceledException ->
                      tcs.TrySetCanceled(cancellationToken)  |> ignore
                  | exn ->
                      System.Diagnostics.Trace.TraceWarning("Visual F# Tools: exception swallowed and not passed to Roslyn: {0}", exn)
                      let res = Unchecked.defaultof<_>
                      tcs.TrySetResult(res) |> ignore
              ),
              cancellationContinuation=(fun _oce ->
                  disposeReg()
                  tcs.TrySetCanceled(cancellationToken) |> ignore
              ),
              cancellationToken=cancellationToken)
    task

/// Execute given tasks on a background thread, running at most Environment.ProcessorCount at the same time
let ParallelBackgroundTasksA (ct: CancellationToken) (jobs: (unit -> Task<'a>) seq) = backgroundTask {

    use semaphore = new SemaphoreSlim(Environment.ProcessorCount)

    let runningTasks = ResizeArray()

    for job in jobs do
        if not ct.IsCancellationRequested then
            do! semaphore.WaitAsync ct
            runningTasks.Add(
                Task.Run<'a>(job, ct)
                    .ContinueWith(fun (t: Task<'a>) ->
                        semaphore.Release() |> ignore
                        t.Result))

    return! Task.WhenAll runningTasks
}

/// Execute given tasks on a background thread, running at most Environment.ProcessorCount at the same time
let ParallelBackgroundTasksB (ct: CancellationToken) (jobs: (unit -> Task<'a>) seq) = backgroundTask {

    use semaphore = new SemaphoreSlim(Environment.ProcessorCount)

    let runningTasks = ResizeArray()

    let jobs = jobs |> Seq.map (fun job () ->
        backgroundTask {
            let! result = job ()
            semaphore.Release() |> ignore
            return result
        })

    for job in jobs do
        if not ct.IsCancellationRequested then
            do! semaphore.WaitAsync ct
            runningTasks.Add (job ())

    return! Task.WhenAll runningTasks
}


/// Execute given asyncs on a background thread, running at most Environment.ProcessorCount at the same time
let ParallelProcessAsyncsA ct =
    Seq.map (fun job () -> job |> StartAsyncAsTask ct)
    >> ParallelBackgroundTasksA ct

/// Execute given asyncs on a background thread, running at most Environment.ProcessorCount at the same time
let ParallelProcessAsyncsB ct =
    Seq.map (fun job () -> job |> StartAsyncAsTask ct)
    >> ParallelBackgroundTasksB ct
