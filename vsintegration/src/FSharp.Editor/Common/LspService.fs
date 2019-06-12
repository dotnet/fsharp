// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.ComponentModel.Composition
open FSharp.Compiler.LanguageServer
open FSharp.Compiler.LanguageServer.Extensions
open StreamJsonRpc

[<Export(typeof<LspService>)>]
type LspService() =
    let mutable options = Options.Default()
    let mutable jsonRpc: JsonRpc option = None

    let sendOptions () =
        async {
            match jsonRpc with
            | None -> ()
            | Some rpc -> do! rpc.SetOptionsAsync(options)
        }

    member __.SetJsonRpc(rpc: JsonRpc) =
        jsonRpc <- Some rpc
        sendOptions()

    member __.SetOptions(opt: Options) =
        options <- opt
        sendOptions()
