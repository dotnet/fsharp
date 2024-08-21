// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace FSharp.Compiler.Interactive

open System
open System.Text
open System.Diagnostics
open System.IO
open System.IO.Pipes
open System.Threading

module CtrlBreakHandlers =

    let interruptCommand = "Interactive-CtrlCNotificationCommand-Interrupt"

    let lineInterruptCommand =
        Encoding.UTF8.GetBytes(interruptCommand + Environment.NewLine)

    let connectionTimeout = 1000

    [<AbstractClass>]
    type public CtrlBreakService(channelName: string) =

        abstract Interrupt: unit -> unit

        // Exceptions percolate to callsite, IO exceptions must be handled by caller
        // Should be run on a new thread
        member this.Run() : unit =
            let service = new NamedPipeServerStream(channelName, PipeDirection.In)

            // Wait for a client to connect
            service.WaitForConnection()
            use stream = new StreamReader(service)

            try
                while not (stream.EndOfStream) do
                    let line = stream.ReadLine()

                    if line = interruptCommand then
                        this.Interrupt()
            finally
                stream.Close()
                service.Close()

    type public CtrlBreakClient(channelName: string) =

        let mutable service: NamedPipeClientStream option =
            Some(new NamedPipeClientStream(".", channelName, PipeDirection.Out))

        member this.Interrupt() =
            match service with
            | None -> ()
            | Some client ->
                try
                    if not (client.IsConnected) then
                        client.Connect(connectionTimeout)
                with _ ->
                    ()

                client.Write(lineInterruptCommand, 0, lineInterruptCommand.Length)
                client.Flush()

        interface IDisposable with
            member _.Dispose() =
                match service with
                | None -> ()
                | Some client ->
                    client.Dispose()
                    service <- None
