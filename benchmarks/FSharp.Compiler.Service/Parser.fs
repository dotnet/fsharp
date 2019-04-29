namespace FSharp.Compiler.Service

open FSharp.Compiler
open FSharp.Compiler.Ast
open FSharp.Compiler.CompileOps
open FSharp.Compiler.ErrorLogger

type ParsingInfo =
    {
        TcConfig: TcConfig
        IsLastFileOrScript: bool
        IsExecutable: bool
        LexResourceManager: Lexhelp.LexResourceManager
        FilePath: string
    }

type ParseResult = ParsedInput option * (PhasedDiagnostic * FSharpErrorSeverity) []

[<RequireQualifiedAccess>]
module Parser =

    let Parse (info: ParsingInfo) =
        let tcConfig = info.TcConfig
        let filePath = info.FilePath
        let errorLogger = CompilationErrorLogger("ParseFile", tcConfig.errorSeverityOptions)
        let input = ParseOneInputFile (tcConfig, info.LexResourceManager, [], filePath, (info.IsLastFileOrScript, info.IsExecutable), errorLogger, (*retrylocked*) true)
        (input, errorLogger.GetErrors ())