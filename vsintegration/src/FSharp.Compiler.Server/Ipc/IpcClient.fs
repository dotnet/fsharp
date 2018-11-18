namespace Microsoft.FSharp.Compiler.Server

open System
open System.IO
open System.Diagnostics
open System.Threading
open System.Threading.Tasks
open System.IO.Pipes

open Newtonsoft.Json

type IpcMessage<'Send, 'Receive> =
    {
        Data: 'Send
        Reply: AsyncReplyChannel<Result<'Receive, string>>
    }

type IpcMessageClient<'Send, 'Receive>(name) =

    let buffer = Array.zeroCreate<char> Constants.IpcBufferSize

    let restartingEvent = Event<unit>()

    let mutable agent = Unchecked.defaultof<_>

    member this.Start() =
        agent <- MailboxProcessor<IpcMessage<'Send, 'Receive>>.Start(fun mailbox -> async {
            let mutable currentMsgOpt = None
            let getMsg () = async {
                match currentMsgOpt with
                | Some(msg) -> return msg
                | _ ->
                    return! mailbox.Receive()
            }

            while true do
                try
                    use fcs = new NamedPipeClientStream(".", name, PipeDirection.InOut, PipeOptions.None)
                    fcs.Connect()
                    fcs.ReadMode <- PipeTransmissionMode.Message

                    let writer = new StreamWriter(fcs, AutoFlush = true)
                    let reader = new StreamReader(fcs)

                    while true do
                        if not fcs.IsConnected then
                            failwith "Disconnected"

                        let! msg = getMsg ()

                        currentMsgOpt <- Some(msg)
                    
                        try
                            if not fcs.IsConnected then
                                failwith "Disconnected"

                            writer.Write(JsonConvert.SerializeObject(msg.Data))

                            currentMsgOpt <- None

                            let count = reader.Read(buffer, 0, buffer.Length)
                            if count > 0 then
                                let msgString = String(buffer, 0, count)

                                msg.Reply.Reply(Result.Ok(JsonConvert.DeserializeObject<'Receive>(msgString)))
                            else
                                msg.Reply.Reply(Result.Error("Unable to get response"))
                        with
                        | ex -> msg.Reply.Reply(Result.Error(ex.Message))
                        
                with
                | ex ->
                    printfn "[FSharp Compiler Client] - Restarting due to: %s" ex.Message
                    restartingEvent.Trigger()
        })

    member __.Send(msg) =
        if not (obj.ReferenceEquals(agent, null)) then
            agent.PostAndAsyncReply(fun reply -> { Data = msg; Reply = reply })
        else
            failwith "IpcClient not started."

    [<CLIEvent>]
    member __.Restarting = restartingEvent.Publish

    interface IDisposable with

        member __.Dispose() =
            if not (obj.ReferenceEquals(agent, null)) then
                (agent :> IDisposable).Dispose()

