namespace FSharp.Compiler.Compilation

open System
open System.Threading
open FSharp.Compiler.AbstractIL.Internal.Library

type CompilationWorkerMessage =
    | WorkUnit of (CompilationThreadToken -> unit)
    | Work of (CompilationThreadToken -> obj) * AsyncReplyChannel<Result<obj, Exception>> * CancellationToken
    | WorkAsync of (CompilationThreadToken -> Async<obj>) * AsyncReplyChannel<Result<obj, Exception>> * CancellationToken

[<Sealed>]
type CompilationWorkerInstance () =

    let ctok = CompilationThreadToken ()

    let loop (agent: MailboxProcessor<CompilationWorkerMessage>) =
        async {
            while true do
                match! agent.Receive() with
                | WorkUnit work ->
                    try
                        work ctok
                    with
                    | _ -> ()
                | Work (work, replyChannel, ct) ->
                    try
                        ct.ThrowIfCancellationRequested ()
                        replyChannel.Reply (Result.Ok (work ctok))
                    with 
                    | ex ->
                        replyChannel.Reply (Result.Error ex)

                | WorkAsync (work, replyChannel, ct) ->
                    try
                        ct.ThrowIfCancellationRequested ()
                        let! result = work ctok
                        replyChannel.Reply (Result.Ok result)
                    with
                    | ex ->
                        replyChannel.Reply (Result.Error ex)
        }

    let agent = new MailboxProcessor<CompilationWorkerMessage> (fun x -> loop x)

    do
        agent.Start ()

    member __.QueueEnabled = false

    member this.Enqueue (work: CompilationThreadToken -> unit) =
        if this.QueueEnabled then
            agent.Post (WorkUnit work)
        else
            work ctok

    member this.EnqueueAndAwaitAsync (work: CompilationThreadToken -> 'T) =
        if this.QueueEnabled then
            async {
                let! ct = Async.CancellationToken
                match! agent.PostAndAsyncReply (fun replyChannel -> Work ((fun ctok -> (work ctok) :> obj), replyChannel, ct)) with
                | Result.Ok result -> return (result :?> 'T)
                | Result.Error ex -> return raise ex
            }
        else
            async { return work ctok }

    member this.EnqueueAsyncAndAwaitAsync (work: CompilationThreadToken -> Async<'T>) =
        if this.QueueEnabled then
            async {
                let work ctok = 
                    async { 
                        let! result = work ctok
                        return (result :> obj)
                    }
                let! ct = Async.CancellationToken
                match! agent.PostAndAsyncReply (fun replyChannel -> WorkAsync (work, replyChannel, ct)) with
                | Result.Ok result -> return (result :?> 'T)
                | Result.Error ex -> return raise ex
            }
        else
            async { return! work ctok }

module CompilationWorker =

    let instance = CompilationWorkerInstance ()

    let Enqueue work = instance.Enqueue work

    let EnqueueAndAwaitAsync work = instance.EnqueueAndAwaitAsync work

    let EnqueueAsyncAndAwaitAsync work = instance.EnqueueAsyncAndAwaitAsync work