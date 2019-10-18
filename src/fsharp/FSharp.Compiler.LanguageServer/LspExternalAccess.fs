// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.LanguageServer

open StreamJsonRpc

[<AutoOpen>]
module FunctionNames =
    [<Literal>]
    let OptionsSet = "options/set"

    [<Literal>]
    let TextDocumentPublishDiagnostics = "textDocument/publishDiagnostics"

type Options =
    { usePreviewTextHover: bool
      usePreviewDiagnostics: bool }
    static member Default() =
        { usePreviewTextHover = false
          usePreviewDiagnostics = false }
    static member AllOn() =
        { usePreviewTextHover = true
          usePreviewDiagnostics = true }

module Extensions =
    type JsonRpc with
        member jsonRpc.SetOptionsAsync (options: Options) =
            async {
                do! jsonRpc.InvokeAsync(OptionsSet, options) |> Async.AwaitTask
            }
