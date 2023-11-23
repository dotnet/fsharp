module internal Internal.Utilities.TaskAgent

open System.Threading
open System.Threading.Tasks

open System.Collections.Concurrent
open System

type AgentMessage<'Message, 'MessageNoReply, 'Reply> =
    | ExpectsReply of 'Message * TaskCompletionSource<'Reply>
    | DoNotReply of 'MessageNoReply

[<Sealed>]
type TaskInbox<'Msg, 'MsgNoReply, 'Reply>() =

    let queue = ConcurrentQueue<AgentMessage<'Msg, 'MsgNoReply, 'Reply>>()

    let messageNotifications = new SemaphoreSlim(0)

    member _.PostAndAwaitReply(msg) =
        let replySource = TaskCompletionSource<'Reply>()

        queue.Enqueue(ExpectsReply(msg, replySource))

        messageNotifications.Release() |> ignore

        replySource.Task

    member _.Post(msg) =
        queue.Enqueue(DoNotReply msg)
        messageNotifications.Release() |> ignore

    member _.Receive() =
        task {
            do! messageNotifications.WaitAsync()

            return
                match queue.TryDequeue() with
                | true, msg -> msg
                | false, _ -> failwith "Message notifications broken"
        }

    interface IDisposable with
        member _.Dispose() = messageNotifications.Dispose()

[<Sealed>]
type TaskAgent<'Msg, 'MsgNoReply, 'Reply>
    (processMessage: ('MsgNoReply -> unit) -> 'Msg -> 'Reply, processMessageNoReply: ('MsgNoReply -> unit) -> 'MsgNoReply -> unit) =
    let inbox = new TaskInbox<'Msg, 'MsgNoReply, 'Reply>()

    let exceptionEvent = new Event<_>()

    let mutable running = true

    let _loop =
        backgroundTask {
            while running do
                match! inbox.Receive() with
                | ExpectsReply(msg, replySource) ->
                    try
                        let reply = processMessage inbox.Post msg
                        replySource.SetResult reply
                    with ex ->
                        replySource.SetException ex

                | DoNotReply msg ->
                    try
                        processMessageNoReply inbox.Post msg
                    with ex ->
                        exceptionEvent.Trigger(msg, ex)
        }

    member _.NoReplyExceptions = exceptionEvent.Publish

    member _.Status = _loop.Status

    member _.PostAndAwaitReply(msg) =
        if not running then
            failwith "Agent has been disposed and is no longer processing messages"

        inbox.PostAndAwaitReply(msg)

    member _.Post(msg) =
        if not running then
            failwith "Agent has been disposed and is no longer processing messages"

        inbox.Post(msg)

    interface IDisposable with
        member _.Dispose() =
            running <- false
            (inbox :> IDisposable).Dispose()
