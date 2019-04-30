namespace FSharp.Compiler.Service

open System.IO
open Microsoft.CodeAnalysis.Text
open FSharp.Compiler
open FSharp.Compiler.Text
open FSharp.Compiler.CompileOps

[<RequireQualifiedAccess>]
type internal SourceValue =
    | SourceText of SourceText
    | Stream of Stream

type internal ParsingInfo =
    {
        tcConfig: TcConfig
        isLastFileOrScript: bool
        isExecutable: bool
        conditionalCompilationDefines: string list
        sourceValue: SourceValue
        filePath: string
    }

[<RequireQualifiedAccess>]
module internal Parser =

    val Parse: ParsingInfo -> ParseResult
