namespace FSharp.Compiler.Service

open FSharp.Compiler
open FSharp.Compiler.Ast
open FSharp.Compiler.CompileOps
open FSharp.Compiler.ErrorLogger

type internal ParsingInfo =
    {
        TcConfig: TcConfig
        IsLastFileOrScript: bool
        IsExecutable: bool
        LexResourceManager: Lexhelp.LexResourceManager
        FilePath: string
    }

[<RequireQualifiedAccess>]
module internal Parser =

    val Parse: ParsingInfo -> ParseResult
