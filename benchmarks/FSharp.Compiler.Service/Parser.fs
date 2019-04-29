namespace FSharp.Compiler.Service

open FSharp.Compiler
open FSharp.Compiler.Text
open FSharp.Compiler.CompileOps

type ParsingInfo =
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
module Parser =

    let Parse (info: ParsingInfo) =
        let tcConfig = info.tcConfig
        let filePath = info.filePath
        let errorLogger = CompilationErrorLogger("ParseFile", tcConfig.errorSeverityOptions)
        let input = ParseOneInputSourceText (info.tcConfig, Lexhelp.LexResourceManager (), info.conditionalCompilationDefines, filePath, info.sourceText, (info.isLastFileOrScript, info.isExecutable), errorLogger)
        {
            filePath = filePath
            parseResult = (input, errorLogger.GetErrors ())
        }