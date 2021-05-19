namespace Internal.Utilities.Library

open System
open System.Threading
open System.Globalization

type private AsyncLazyWeakMessage<'T> =
    | GetValue of AsyncReplyChannel<Result<'T, Exception>> * CancellationToken

type private AgentInstance<'T> = (MailboxProcessor<AsyncLazyWeakMessage<'T>> * CancellationTokenSource)

[<RequireQualifiedAccess>]
type private AgentAction<'T> =
    | GetValue of AgentInstance<'T>
    | CachedValue of 'T

[<RequireQualifiedAccess>]
module AsyncLazy =

    // We need to store the culture for the VS thread that is executing now, 
    // so that when the agent in the async lazy object picks up thread from the thread pool we can set the culture
    let mutable culture = CultureInfo(CultureInfo.CurrentUICulture.Name)

    let SetPreferredUILang (preferredUiLang: string option) = 
        match preferredUiLang with
        | Some s -> 
            culture <- CultureInfo s
#if FX_RESHAPED_GLOBALIZATION
            CultureInfo.CurrentUICulture <- culture
#else
            Thread.CurrentThread.CurrentUICulture <- culture
#endif
        | None -> ()

[<Sealed>]
type AsyncLazy<'T> (computation: Async<'T>) =

    let gate = obj ()
    let mutable computation = computation
    let mutable requestCount = 0
    let mutable cachedResult = ValueNone
    let mutable cachedResultAsync = ValueNone

    let loop (agent: MailboxProcessor<AsyncLazyWeakMessage<'T>>) =
        async {
            while true do
                match! agent.Receive() with
                | GetValue (replyChannel, ct) ->
                    Thread.CurrentThread.CurrentUICulture <- AsyncLazy.culture
                    try
                        use _reg = 
                            // When a cancellation has occured, notify the reply channel to let the requester stop waiting for a response.
                            ct.Register (fun () -> 
                                let ex = OperationCanceledException() :> exn
                                replyChannel.Reply (Error ex)
                            )

                        ct.ThrowIfCancellationRequested ()

                        match cachedResult with
                        | ValueSome result ->
                            replyChannel.Reply (Ok result)
                        | _ ->
                            // This computation can only be canceled if the requestCount reaches zero.
                            let! result = computation
                            cachedResult <- ValueSome result
                            computation <- Unchecked.defaultof<_>
                            if not ct.IsCancellationRequested then
                                replyChannel.Reply (Ok result)
                    with 
                    | ex ->
                        replyChannel.Reply (Error ex)
        }

    let mutable agentInstance: (MailboxProcessor<AsyncLazyWeakMessage<'T>> * CancellationTokenSource) option = None

    member _.GetValueAsync () =
        // fast path
        match cachedResultAsync with
        | ValueSome resultAsync -> resultAsync
        | _ ->
            match cachedResult with
            | ValueSome result -> 
                let resultAsync = async { return result }
                cachedResultAsync <- ValueSome resultAsync
                resultAsync
            | _ ->
                async {
                   match cachedResult with
                   | ValueSome result -> return result
                   | _ ->
                       let action =
                            lock gate <| fun () ->
                                // We try to get the cached result after the lock so we don't spin up a new mailbox processor.
                                match cachedResult with
                                | ValueSome result -> AgentAction<'T>.CachedValue result
                                | _ ->
                                    requestCount <- requestCount + 1
                                    match agentInstance with
                                    | Some agentInstance -> AgentAction<'T>.GetValue agentInstance
                                    | _ ->
                                        let cts = new CancellationTokenSource ()
                                        let agent = new MailboxProcessor<AsyncLazyWeakMessage<'T>> (loop, cancellationToken = cts.Token)
                                        let newAgentInstance = (agent, cts)
                                        agentInstance <- Some newAgentInstance
                                        agent.Start ()
                                        AgentAction<'T>.GetValue newAgentInstance

                       match action with
                       | AgentAction.CachedValue result -> return result
                       | AgentAction.GetValue (agent, cts) ->                       
                           try
                               let! ct = Async.CancellationToken
                               match! agent.PostAndAsyncReply (fun replyChannel -> GetValue(replyChannel, ct)) with
                               | Ok result -> return result
                               | Error ex -> return raise ex
                            finally
                                lock gate <| fun () ->
                                    requestCount <- requestCount - 1
                                    if requestCount = 0 then
                                         cts.Cancel () // cancel computation when all requests are cancelled
                                         (agent :> IDisposable).Dispose ()
                                         cts.Dispose ()
                                         agentInstance <- None
               }

       member _.TryGetValue() = cachedResult

       member _.RequestCount = requestCount