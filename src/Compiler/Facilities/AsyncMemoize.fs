namespace Internal.Utilities.Collections

open FSharp.Compiler.BuildGraph
open System.Threading
open System.Collections.Generic

type internal Action<'TKey, 'TValue> =
    | GetOrCompute of ('TKey -> NodeCode<'TValue>) * CancellationToken
    | CancelRequest
    | JobCompleted

type MemoizeRequest<'TKey, 'TValue> = 'TKey * Action<'TKey, 'TValue> * AsyncReplyChannel<NodeCode<'TValue>>

type internal Job<'TValue> =
    | Running of NodeCode<'TValue> * CancellationTokenSource
    | Completed of NodeCode<'TValue>

type internal JobEvent<'TKey> =
    | Started of 'TKey
    | Finished of 'TKey
    | Canceled of 'TKey

    member this.Key =
        match this with
        | Started key -> key
        | Finished key -> key
        | Canceled key -> key

type internal AsyncMemoize<'TKey, 'TValue when 'TKey: equality>(?eventLog: ResizeArray<JobEvent<'TKey>>) =

    let tok = obj ()

    let cache =
        MruCache<_, 'TKey, Job<'TValue>>(keepStrongly = 10, areSame = (fun (x, y) -> x = y))

    let requestCounts = Dictionary<'TKey, int>()

    let incrRequestCount key =
        requestCounts.[key] <-
            if requestCounts.ContainsKey key then
                requestCounts.[key] + 1
            else
                1

    let sendAsync (inbox: MailboxProcessor<_>) key msg =
        inbox.PostAndAsyncReply(fun rc -> key, msg, rc) |> Async.Ignore |> Async.Start

    let log event =
        eventLog |> Option.iter (fun log -> log.Add event)

    let agent =
        MailboxProcessor.Start(fun (inbox: MailboxProcessor<MemoizeRequest<_, _>>) ->

            let post = sendAsync inbox

            async {
                while true do

                    let! key, action, replyChannel = inbox.Receive()

                    match action, cache.TryGet(tok, key) with
                    | GetOrCompute _, Some (Completed job) -> replyChannel.Reply job
                    | GetOrCompute (_, ct), Some (Running (job, _)) ->

                        incrRequestCount key
                        replyChannel.Reply job
                        ct.Register(fun _ -> post key CancelRequest) |> ignore

                    | GetOrCompute (computation, ct), None ->

                        let cts = new CancellationTokenSource()

                        let startedComputation =
                            Async.StartAsTask(
                                Async.AwaitNodeCode(
                                    node {
                                        let! result = computation key
                                        post key JobCompleted
                                        return result
                                    }
                                ),
                                cancellationToken = cts.Token
                            )

                        log (Started key)

                        let job = NodeCode.AwaitTask startedComputation

                        cache.Set(tok, key, (Running(job, cts)))

                        incrRequestCount key

                        ct.Register(fun _ -> post key CancelRequest) |> ignore

                        replyChannel.Reply job

                    | CancelRequest, Some (Running (_, cts)) ->
                        let requestCount = requestCounts.TryGetValue key |> snd

                        if requestCount > 1 then
                            requestCounts.[key] <- requestCount - 1

                        else
                            cts.Cancel()
                            cache.RemoveAnySimilar(tok, key)
                            requestCounts.Remove key |> ignore
                            log (Canceled key)

                    | CancelRequest, None
                    | CancelRequest, Some (Completed _) -> ()

                    | JobCompleted, Some (Running (job, _)) ->
                        cache.Set(tok, key, (Completed job))
                        requestCounts.Remove key |> ignore
                        log (Finished key)

                    | JobCompleted, _ -> failwith "If this happens there's a bug"

            })

    member _.Get(key, computation) =
        node {
            let! ct = NodeCode.CancellationToken

            let! job =
                agent.PostAndAsyncReply(fun rc -> key, (GetOrCompute(computation, ct)), rc)
                |> NodeCode.AwaitAsync

            return! job
        }
