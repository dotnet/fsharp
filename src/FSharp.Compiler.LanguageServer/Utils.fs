[<AutoOpen>]
module FSharp.Compiler.LanguageServer.Utils

open Microsoft.CommonLanguageServerProtocol.Framework

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
