namespace Microsoft.FSharp.Compiler.Server

open System
open System.IO
open System.Diagnostics
open System.Threading
open System.Threading.Tasks
open System.IO.Pipes

open Newtonsoft.Json

type IpcMessageServer<'Receive, 'Response>(name, f : 'Receive -> Async<'Response>) =

    let buffer = Array.zeroCreate<char> Constants.IpcBufferSize

    member this.Run() =
        use fcs = new NamedPipeServerStream(name, PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.None)

        printfn "[FSharp Compiler Server] - Waiting for connection"

        fcs.WaitForConnection()

        printfn "[FSharp Compiler Server] - Client connected"

        let writer = new StreamWriter(fcs, AutoFlush = true)
        let reader = new StreamReader(fcs)

        try
            while true do
                if not fcs.IsConnected then
                    failwith "Client disconnected"

                let count = reader.Read(buffer, 0, buffer.Length)
                if count > 0 then
                    let msgString = String(buffer, 0, count)

                    let receivedMsg = JsonConvert.DeserializeObject<'Receive>(msgString)

                    printfn "Received: %A" (receivedMsg.GetType().Name)

                    let responseMsg = f receivedMsg |> Async.RunSynchronously

                    if not fcs.IsConnected then
                        failwith "Client disconnected"

                    writer.Write(JsonConvert.SerializeObject(responseMsg))
        with
        | ex -> 
            printfn "[FSharp Compiler Server] - Closing due to: %s" ex.Message
