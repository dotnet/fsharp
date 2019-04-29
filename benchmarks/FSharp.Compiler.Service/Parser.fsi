namespace FSharp.Compiler.Service

open FSharp.Compiler
open FSharp.Compiler.Text
open FSharp.Compiler.CompileOps

type internal ParsingInfo =
    {
        tcConfig: TcConfig
        isLastFileOrScript: bool
        isExecutable: bool
        conditionalCompilationDefines: string list
        sourceText: ISourceText
        filePath: string
    }

type internal SyntaxTree =
    {
        filePath: string
        parseResult: ParseResult
    }

[<RequireQualifiedAccess>]
module internal Parser =

    val Parse: ParsingInfo -> SyntaxTree
