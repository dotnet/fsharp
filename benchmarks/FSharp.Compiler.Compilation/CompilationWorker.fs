namespace FSharp.Compiler.Compilation

open System
open FSharp.Compiler.AbstractIL.Internal.Library

type CompilationWorkerMessage =
    | Work of (CompilationThreadToken -> obj) * AsyncReplyChannel<Result<obj, Exception>>

[<Sealed>]
type CompilationWorkerInstance () =

    let ctok = CompilationThreadToken ()

    let loop (agent: MailboxProcessor<CompilationWorkerMessage>) =
        async {
            while true do
                match! agent.Receive() with
                | Work (work, replyChannel) ->
                    try
                        replyChannel.Reply (Result.Ok (work ctok))
                    with 
                    | ex ->
                        replyChannel.Reply (Result.Error ex)
        }

    let agent = new MailboxProcessor<CompilationWorkerMessage> (fun x -> loop x)

    do
        agent.Start ()

    member __.EnqueueAndAwaitAsync (work: CompilationThreadToken -> 'T) =
        async {
            match! agent.PostAndAsyncReply (fun replyChannel -> Work ((fun ctok -> (work ctok) :> obj), replyChannel)) with
            | Result.Ok result -> return (result :?> 'T)
            | Result.Error ex -> return raise ex
        }

module CompilationWorker =

    let instance = CompilationWorkerInstance ()

    let EnqueueAndAwaitAsync work = instance.EnqueueAndAwaitAsync work