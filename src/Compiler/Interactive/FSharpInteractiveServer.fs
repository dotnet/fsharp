// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace FSharp.Compiler.Interactive

open System
open System.Text
open System.Diagnostics
open System.IO
open System.IO.Pipes
open System.Threading

module CtrlBreakHandlers =

    let interuptCommand = "Interactive-CtrlCNotificationCommand-Interupt"

    let lineInteruptCommand =
        Encoding.UTF8.GetBytes(interuptCommand + Environment.NewLine)

    let connectionTimeout = 1000

    [<AbstractClass>]
    type public CtrlBreakService(channelName: string) =

        abstract Interrupt: unit -> unit

        // Exceptions percolate to callsite, IO exceptions must be handled by caller
        // Should be run on a new thread
        member this.Run() : unit =
            let service = Some(new NamedPipeServerStream(channelName, PipeDirection.In))

            match service with
            | Some service ->
                // Wait for a client to connect
                service.WaitForConnection()
                use stream = new StreamReader(service)

                try
                    while not (stream.EndOfStream) do
                        let line = stream.ReadLine()

                        if line = interuptCommand then
                            this.Interrupt()
                finally
                    stream.Close()
                    service.Close()
            | None -> ()

    type public CtrlBreakClient(channelName: string) =

        let mutable service: NamedPipeClientStream option = None

        member this.Interrupt() =
            match service with
            | None -> ()
            | Some service ->
                try
                    if not (service.IsConnected) then
                        service.Connect(connectionTimeout)
                with _ ->
                    ()

                service.Write(lineInteruptCommand, 0, lineInteruptCommand.Length)
                service.Flush()

        member _.Start() =
            service <- Some(new NamedPipeClientStream(".", channelName, PipeDirection.Out))
