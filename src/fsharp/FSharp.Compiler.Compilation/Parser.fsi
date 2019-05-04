namespace FSharp.Compiler.Compilation

open System
open System.IO
open Microsoft.CodeAnalysis.Text
open FSharp.Compiler
open FSharp.Compiler.Text
open FSharp.Compiler.CompileOps

[<RequireQualifiedAccess>]
type internal SourceValue =
    | SourceText of SourceText
    | Stream of Stream

    interface IDisposable

type internal ParsingInfo =
    {
        tcConfig: TcConfig
        isLastFileOrScript: bool
        isExecutable: bool
        conditionalCompilationDefines: string list
        filePath: string
    }

[<RequireQualifiedAccess>]
module internal Parser =

    val Parse: ParsingInfo -> SourceValue -> ParseResult
