// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.LanguageServer

open StreamJsonRpc

// https://microsoft.github.io/language-server-protocol/specification
type Methods(state: State) =

    [<JsonRpcMethod("initialize")>]
    member __.Initialize (args: InitializeParams) =
        async {
            // note, it's important that this method is `async` because unit tests can then properly verify that the
            // JSON RPC handling of async methods is correct
            return ServerCapabilities.DefaultCapabilities()
        } |> Async.StartAsTask

    [<JsonRpcMethod("shutdown")>]
    member __.Shutdown() = state.DoShutdown()

    [<JsonRpcMethod("textDocument/hover")>]
    member __.TextDocumentHover (args: TextDocumentPositionParams) = TextDocument.Hover(state, args) |> Async.StartAsTask
