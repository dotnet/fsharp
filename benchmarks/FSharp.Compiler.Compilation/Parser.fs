namespace FSharp.Compiler.Service

open System.IO
open Microsoft.CodeAnalysis.Text
open FSharp.Compiler
open FSharp.Compiler.Text
open FSharp.Compiler.CompileOps

[<RequireQualifiedAccess>]
type SourceValue =
    | SourceText of SourceText
    | Stream of Stream

type ParsingInfo =
    {
        tcConfig: TcConfig
        isLastFileOrScript: bool
        isExecutable: bool
        conditionalCompilationDefines: string list
        sourceValue: SourceValue
        filePath: string
    }

[<RequireQualifiedAccess>]
module Parser =

    let Parse (info: ParsingInfo) =
        let tcConfig = info.tcConfig
        let filePath = info.filePath
        let errorLogger = CompilationErrorLogger("ParseFile", tcConfig.errorSeverityOptions)

        let input =
            match info.sourceValue with
            | SourceValue.SourceText sourceText ->
                ParseOneInputSourceText (info.tcConfig, Lexhelp.LexResourceManager (), info.conditionalCompilationDefines, filePath, sourceText.ToFSharpSourceText (), (info.isLastFileOrScript, info.isExecutable), errorLogger)
            | SourceValue.Stream stream ->
                ParseOneInputStream (info.tcConfig, Lexhelp.LexResourceManager (), info.conditionalCompilationDefines, filePath, stream, (info.isLastFileOrScript, info.isExecutable), errorLogger)
        (input, errorLogger.GetErrors ())