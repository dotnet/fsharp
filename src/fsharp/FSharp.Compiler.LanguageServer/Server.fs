// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.LanguageServer

open System
open System.IO
open StreamJsonRpc

type Server(sendingStream: Stream, receivingStream: Stream) =

    let formatter = JsonMessageFormatter()
    let converter = JsonOptionConverter() // special handler to convert between `Option<'T>` and `obj/null`.
    do formatter.JsonSerializer.Converters.Add(converter)
    let handler = new HeaderDelimitedMessageHandler(sendingStream, receivingStream, formatter)
    let methods = Methods()
    let rpc = new JsonRpc(handler, methods)
    do methods.State.JsonRpc <- Some rpc

    member __.StartListening() =
        rpc.StartListening()

    member __.WaitForExitAsync() =
        async {
            do! Async.AwaitEvent (methods.State.Shutdown)
            do! Async.AwaitEvent (methods.State.Exit)
        }

    interface IDisposable with
        member __.Dispose() =
            rpc.Dispose()
