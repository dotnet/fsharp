namespace FSharp.Compiler.Compilation

open System.Threading
open Microsoft.CodeAnalysis.Text
open FSharp.Compiler

[<Struct>]
type internal TokenItem = TokenItem of rawToken: Parser.token * span: TextSpan * startIndex: int

[<Sealed>]
type internal IncrementalLexer =

    member GetTokens: span: TextSpan * ct: CancellationToken -> TokenItem seq

    member LexFilter: ErrorLogger.ErrorLogger * (UnicodeLexing.Lexbuf -> (UnicodeLexing.Lexbuf -> Parser.token) -> unit) * CancellationToken -> unit

    member WithChangedTextSnapshot: newTextSnapshot: FSharpSourceSnapshot -> IncrementalLexer

    static member Create: ParsingConfig * textSnapshot: FSharpSourceSnapshot -> IncrementalLexer