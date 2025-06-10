namespace FSharp.Compiler.LanguageServer

open Microsoft.CommonLanguageServerProtocol.Framework
open Microsoft.VisualStudio.LanguageServer.Protocol

open System.Diagnostics

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

module Activity =
    let listen (filter) logMsg =
        let indent (activity: Activity) =
            let rec loop (activity: Activity) n =
                match activity.Parent with
                | Null -> n
                | NonNull parent -> loop (parent) (n + 1)

            String.replicate (loop activity 0) "    "

        let collectTags (activity: Activity) =
            [ for tag in activity.Tags -> $"{tag.Key}: %A{tag.Value}" ]
            |> String.concat ", "

        let listener =
            new ActivityListener(
                ShouldListenTo = (fun source -> source.Name = FSharp.Compiler.Diagnostics.ActivityNames.FscSourceName),
                Sample =
                    (fun context ->
                        if filter context.Name then
                            ActivitySamplingResult.AllDataAndRecorded
                        else
                            ActivitySamplingResult.None),
                ActivityStarted = (fun a -> logMsg $"{indent a}{a.OperationName}     {collectTags a}")
            )

        ActivitySource.AddActivityListener(listener)

    let listenToAll () =
        listen (fun _ -> true) Trace.TraceInformation

    let listenToSome () =
        listen (fun x -> not <| x.Contains "StackGuard") Trace.TraceInformation
