module internal FSharp.Compiler.Compilation.Utilities

open System
open System.Threading
open System.Collections.Immutable
open FSharp.Compiler.AbstractIL.Internal.Library

[<RequireQualifiedAccess>]
module Cancellable =

    let toAsync e = 
        async { 
          let! ct = Async.CancellationToken
          return! 
             Async.FromContinuations(fun (cont, econt, ccont) -> 
               // Run the computation synchronously using the given cancellation token
               let res = try Choice1Of2 (Cancellable.run ct e) with err -> Choice2Of2 err
               match res with 
               | Choice1Of2 (ValueOrCancelled.Value v) -> cont v
               | Choice1Of2 (ValueOrCancelled.Cancelled err) -> ccont err
               | Choice2Of2 err -> econt err) 
        }

[<RequireQualifiedAccess>]
module ImmutableArray =

    let inline iter f (arr: ImmutableArray<_>) =
        for i = 0 to arr.Length - 1 do
            f arr.[i]

    let inline iteri f (arr: ImmutableArray<_>) =
        for i = 0 to arr.Length - 1 do
            f i arr.[i]

[<Sealed>]
type NonReentrantLock () =

   let syncLock = obj ()
   let mutable currentThreadId = 0

   let isLocked () = currentThreadId <> 0

   member this.Wait (cancellationToken: CancellationToken) =

       if currentThreadId = Environment.CurrentManagedThreadId then
           failwith "AsyncLazy tried to re-enter computation recursively."
       
       use _cancellationTokenRegistration = cancellationToken.Register((fun o -> lock o (fun () -> Monitor.PulseAll o |> ignore)), syncLock, useSynchronizationContext = false)

       let spin = SpinWait ()
       while isLocked () && not spin.NextSpinWillYield do
           spin.SpinOnce ()

       lock syncLock <| fun () ->
           while isLocked () do
               cancellationToken.ThrowIfCancellationRequested ()
               Monitor.Wait syncLock |> ignore
           currentThreadId <- Environment.CurrentManagedThreadId

       new SemaphoreDisposer (this) :> IDisposable

   member this.Release () =
       lock syncLock <| fun() ->
           currentThreadId <- 0
           Monitor.Pulse syncLock |> ignore

and [<Struct>] private SemaphoreDisposer (semaphore: NonReentrantLock) =

   interface IDisposable with

       member __.Dispose () = semaphore.Release ()

type private AsyncLazyWeakMessage<'T> =
    | GetValue of AsyncReplyChannel<Result<'T, Exception>>

[<Sealed>]
type AsyncLazyWeak<'T when 'T : not struct> (computation: Async<'T>) =

    let gate = NonReentrantLock ()
    let mutable requestCount = 0
    let mutable cachedResult: WeakReference<'T> voption = ValueNone

    let tryGetResult () =
        match cachedResult with
        | ValueSome weak ->
            match weak.TryGetTarget () with
            | true, result -> ValueSome result
            | _ -> ValueNone
        | _ -> ValueNone

    let loop (agent: MailboxProcessor<AsyncLazyWeakMessage<'T>>) =
        async {
            while true do
                match! agent.Receive() with
                | GetValue replyChannel ->
                    try
                        match tryGetResult () with
                        | ValueSome result ->
                            replyChannel.Reply (Ok result)
                        | _ ->
                            let! result = computation
                            cachedResult <- ValueSome (WeakReference<_> result)
                            replyChannel.Reply (Ok result) 
                    with 
                    | ex ->
                        replyChannel.Reply (Error ex)
        }

    let mutable agentInstance: (MailboxProcessor<AsyncLazyWeakMessage<'T>> * CancellationTokenSource) option = None

    member __.GetValueAsync () =
       async {
           match tryGetResult () with
           | ValueSome result -> return result
           | _ ->
               let! cancellationToken = Async.CancellationToken
               let agent, cts =
                    use _semaphoreDisposer = gate.Wait cancellationToken
                    requestCount <- requestCount + 1
                    match agentInstance with
                    | Some agentInstance -> agentInstance
                    | _ ->
                        let cts = new CancellationTokenSource ()
                        let agent = new MailboxProcessor<AsyncLazyWeakMessage<'T>>((fun x -> loop x), cancellationToken = cts.Token)
                        let newAgentInstance = (agent, cts)
                        agentInstance <- Some newAgentInstance
                        agent.Start ()
                        newAgentInstance
                        
               let! result = agent.PostAndAsyncReply (fun replyChannel -> GetValue replyChannel)

               use _semaphoreDisposer = gate.Wait cancellationToken
               requestCount <- requestCount - 1
               if requestCount = 0 then
                    cts.Cancel ()
                    (agent :> IDisposable).Dispose ()
                    cts.Dispose ()
                    agentInstance <- None
               match result with
               | Ok result -> return result
               | Error ex -> return raise ex
       }

type AsyncLazy<'T when 'T : not struct> (computation) =
    
    let weak = AsyncLazyWeak<'T> computation
    let mutable cachedResult = ValueNone // hold strongly

    member __.GetValueAsync () =
        async {
            match cachedResult with
            | ValueSome result -> return result
            | _ ->
                let! result = weak.GetValueAsync ()
                cachedResult <- ValueSome result
                return result
        }