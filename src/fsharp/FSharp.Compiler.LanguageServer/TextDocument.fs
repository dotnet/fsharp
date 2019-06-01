// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.LanguageServer

module TextDocument =

    let Hover(state: State, args: TextDocumentPositionParams) =
        async {
            return {
                Hover.contents = {
                    MarkupContent.kind = MarkupKind.PlainText
                    value = "TODO"
                }
                range = Some({
                    Range.start = {
                        Position.line = 0
                        character = 0
                    }
                    ``end`` = {
                        Position.line = 0
                        character = 0
                    }
                })
            }
        }

    let PublishDiagnostics(state: State) =
        async {
            return {
                PublishDiagnosticsParams.uri = ""
                diagnostics = [||]
            }
        }
