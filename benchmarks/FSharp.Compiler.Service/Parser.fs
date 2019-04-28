namespace FSharp.Compiler.Service

open FSharp.Compiler
open FSharp.Compiler.CompileOps

type ParsingInfo =
    {
        TcConfig: TcConfig
        IsLastFileOrScript: bool
        IsExecutable: bool
        LexResourceManager: Lexhelp.LexResourceManager
        FilePath: string
    }

[<RequireQualifiedAccess>]
module Parser =

    let ParseFile (info: ParsingInfo) =
        let tcConfig = info.TcConfig
        let filePath = info.FilePath
        let errorLogger = CompilationErrorLogger("ParseFile", tcConfig.errorSeverityOptions)
        let input = ParseOneInputFile (tcConfig, info.LexResourceManager, [], filePath, (info.IsLastFileOrScript, info.IsExecutable), errorLogger, (*retrylocked*) true)

        {
            FilePath = filePath
            ParseResult = (input, errorLogger.GetErrors ())
        }