// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.LanguageServer

open System

module TextDocument =

    let Hover (state: State) (textDocument: TextDocumentIdentifier) (position: Position) =
        async {
            Console.Error.WriteLine("hover at " + position.line.ToString() + "," + position.character.ToString())
            if not state.Options.usePreviewTextHover then return None
            else
                let startCol, endCol =
                    if position.character = 0 then 0, 1
                    else position.character, position.character + 1
                return Some { contents = { kind = MarkupKind.PlainText
                                           value = "serving textDocument/hover from LSP" }
                              range = Some { start = { line = position.line; character = startCol }
                                             ``end`` = { line = position.line; character = endCol } }
                }
        }

    let PublishDiagnostics(state: State) =
        async {
            return {
                PublishDiagnosticsParams.uri = ""
                diagnostics = [||]
            }
        }
