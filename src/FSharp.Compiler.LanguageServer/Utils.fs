namespace FSharp.Compiler.LanguageServer

open Microsoft.CommonLanguageServerProtocol.Framework
open Microsoft.VisualStudio.LanguageServer.Protocol

open FSharp.Compiler.Diagnostics
open System.Runtime.CompilerServices

[<AutoOpen>]
module Utils =

    type LspRange = Microsoft.VisualStudio.LanguageServer.Protocol.Range

    let LspLogger (output: string -> unit) =
        { new ILspLogger with
            member this.LogEndContext(message: string, ``params``: obj array) : unit =
                output $"EndContext :: {message} %A{``params``}"

            member this.LogError(message: string, ``params``: obj array) : unit =
                output $"ERROR :: {message} %A{``params``}"

            member this.LogException(``exception``: exn, message: string, ``params``: obj array) : unit =
                output $"EXCEPTION :: %A{``exception``} {message} %A{``params``}"

            member this.LogInformation(message: string, ``params``: obj array) : unit =
                output $"INFO :: {message} %A{``params``}"

            member this.LogStartContext(message: string, ``params``: obj array) : unit =
                output $"StartContext :: {message} %A{``params``}"

            member this.LogWarning(message: string, ``params``: obj array) : unit =
                output $"WARNING :: {message} %A{``params``}"
        }

    type FSharp.Compiler.Text.Range with

        member this.ToLspRange() =
            LspRange(
                Start = Position(Line = this.StartLine - 1, Character = this.StartColumn),
                End = Position(Line = this.EndLine - 1, Character = this.EndColumn)
            )

[<Extension>]
type FSharpDiagnosticExtensions =

    [<Extension>]
    static member ToLspDiagnostic(this: FSharpDiagnostic) =
        Diagnostic(
            Range = this.Range.ToLspRange(),
            Severity = DiagnosticSeverity.Error,
            Message = $"LSP: {this.Message}",
            //Source = "Intellisense",
            Code = SumType<int, _> this.ErrorNumberText
        )
