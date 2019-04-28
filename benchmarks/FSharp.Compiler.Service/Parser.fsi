namespace FSharp.Compiler.Service

open FSharp.Compiler
open FSharp.Compiler.CompileOps

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

    val ParseFile: ParsingInfo -> SyntaxTree
