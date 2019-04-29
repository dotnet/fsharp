module internal FSharp.Compiler.Service.Utilities

open System
open System.Threading
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

[<Sealed>]
type AsyncLazy<'T> (computation: Async<'T>) =

   let mutable computation = computation

   let gate = NonReentrantLock ()
   let mutable cachedResult = ValueNone

   member __.GetValueAsync () =
       async {
           match cachedResult with
           | ValueSome result -> return result
           | _ ->
               let! cancellationToken = Async.CancellationToken
               use _semaphoreDisposer = gate.Wait cancellationToken

               cancellationToken.ThrowIfCancellationRequested ()

               match cachedResult with
               | ValueSome result -> return result
               | _ ->
                   let! result = computation
                   cachedResult <- ValueSome result
                   computation <- Unchecked.defaultof<Async<'T>> // null out function since we have result, so we don't strongly hold onto more stuff
                   return result               
       }