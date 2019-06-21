// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.LanguageServer

open System
open System.Runtime.InteropServices
open System.Threading
open Newtonsoft.Json.Linq
open StreamJsonRpc

// https://microsoft.github.io/language-server-protocol/specification
type Methods(state: State) =

    /// Helper to run Async<'T> with a CancellationToken.
    let runAsync (cancellationToken: CancellationToken) (computation: Async<'T>) = Async.StartAsTask(computation, cancellationToken=cancellationToken)

    member __.State = state

    //--------------------------------------------------------------------------
    // official LSP methods
    //--------------------------------------------------------------------------

    [<JsonRpcMethod("initialize")>]
    member __.Initialize
        (
            processId: Nullable<int>,
            [<Optional; DefaultParameterValue(null: string)>] rootPath: string,
            [<Optional; DefaultParameterValue(null: string)>] rootUri: DocumentUri,
            [<Optional; DefaultParameterValue(null: JToken)>] initializationOptions: JToken,
            capabilities: ClientCapabilities,
            [<Optional; DefaultParameterValue(null: string)>] trace: string,
            [<Optional; DefaultParameterValue(null: WorkspaceFolder[])>] workspaceFolders: WorkspaceFolder[]
        ) =
        { InitializeResult.capabilities = ServerCapabilities.DefaultCapabilities() }

    [<JsonRpcMethod("initialized")>]
    member __.Initialized () = ()

    [<JsonRpcMethod("shutdown")>]
    member __.Shutdown(): obj = state.DoShutdown(); null

    [<JsonRpcMethod("exit")>]
    member __.Exit() = state.DoExit()

    [<JsonRpcMethod("$/cancelRequest")>]
    member __.cancelRequest (id: JToken) = state.DoCancel()

    [<JsonRpcMethod("textDocument/hover")>]
    member __.TextDocumentHover
        (
            textDocument: TextDocumentIdentifier,
            position: Position,
            [<Optional; DefaultParameterValue(CancellationToken())>] cancellationToken: CancellationToken
        ) =
        TextDocument.Hover state textDocument position |> runAsync cancellationToken

    //--------------------------------------------------------------------------
    // unofficial LSP methods that we implement separately
    //--------------------------------------------------------------------------

    [<JsonRpcMethod(OptionsSet)>]
    member __.OptionsSet
        (
            options: Options
        ) =
        sprintf "got options %A" options |> Console.Error.WriteLine
        state.Options <- options
