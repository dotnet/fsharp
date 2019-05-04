namespace FSharp.Compiler.Compilation

open System
open System.IO
open Microsoft.CodeAnalysis.Text
open FSharp.Compiler
open FSharp.Compiler.Text
open FSharp.Compiler.CompileOps

[<RequireQualifiedAccess>]
type SourceValue =
    | SourceText of SourceText
    | Stream of Stream

    interface IDisposable with

        member this.Dispose () =
            match this with
            | SourceValue.Stream stream -> stream.Dispose ()
            | _ -> ()           

type ParsingInfo =
    {
        tcConfig: TcConfig
        isLastFileOrScript: bool
        isExecutable: bool
        conditionalCompilationDefines: string list
        filePath: string
    }

[<RequireQualifiedAccess>]
module Parser =

    let Parse (info: ParsingInfo) sourceValue =
        let tcConfig = info.tcConfig
        let filePath = info.filePath
        let errorLogger = CompilationErrorLogger("ParseFile", tcConfig.errorSeverityOptions)

        let input =
            match sourceValue with
            | SourceValue.SourceText sourceText ->
                ParseOneInputSourceText (info.tcConfig, Lexhelp.LexResourceManager (), info.conditionalCompilationDefines, filePath, sourceText.ToFSharpSourceText (), (info.isLastFileOrScript, info.isExecutable), errorLogger)
            | SourceValue.Stream stream ->
                ParseOneInputStream (info.tcConfig, Lexhelp.LexResourceManager (), info.conditionalCompilationDefines, filePath, stream, (info.isLastFileOrScript, info.isExecutable), errorLogger)
        (input, errorLogger.GetErrors ())