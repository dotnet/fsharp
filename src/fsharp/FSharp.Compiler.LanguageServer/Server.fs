// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.LanguageServer

open System
open System.IO
open StreamJsonRpc

type Server(sendingStream: Stream, receivingStream: Stream) =

    let state = State()
    let methods = Methods(state)
    let rpc = new JsonRpc(sendingStream, receivingStream, methods)

    member __.StartListening() =
        rpc.StartListening()

    member __.WaitForExitAsync() =
        async {
            do! Async.AwaitEvent (state.Shutdown)
        }

    interface IDisposable with
        member __.Dispose() =
            rpc.Dispose()
