namespace FSharp.Compiler.Compilation

open System.IO
open System.Threading
open System.Collections.Generic
open System.Collections.Immutable
open System.Runtime.CompilerServices
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.Host
open FSharp.Compiler.Compilation.Utilities
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler
open FSharp.Compiler.Text
open FSharp.Compiler.CompileOps
open FSharp.Compiler.Ast
open FSharp.Compiler.Range

type ParsingConfig =
    {
        tcConfig: TcConfig
        isLastFileOrScript: bool
        isExecutable: bool
        conditionalCompilationDefines: string list
        filePath: string
    }

[<RequireQualifiedAccess>]
type SourceValue =
    | SourceText of SourceText
    | Stream of Stream

[<RequireQualifiedAccess>]
module Lexer =

    open FSharp.Compiler.Lexhelp

    let LexAux (pConfig: ParsingConfig) sourceValue errorLogger lexbufCallback =
        let tcConfig = pConfig.tcConfig
        let filePath = pConfig.filePath

        let lightSyntaxStatus = LightSyntaxStatus (tcConfig.ComputeLightSyntaxInitialStatus filePath, true) 
        let conditionalCompilationDefines = pConfig.conditionalCompilationDefines
        let lexargs = mkLexargs (filePath, conditionalCompilationDefines@tcConfig.conditionalCompilationDefines, lightSyntaxStatus, Lexhelp.LexResourceManager (), ref [], errorLogger, tcConfig.pathMap)

        match sourceValue with
        | SourceValue.SourceText sourceText ->
            let lexbuf = UnicodeLexing.SourceTextAsLexbuf (sourceText.ToFSharpSourceText ())
            usingLexbufForParsing (lexbuf, filePath) (fun lexbuf ->
                lexbufCallback lexargs lexbuf
            )
        | SourceValue.Stream stream ->
            let streamReader = new StreamReader(stream) // don't dispose of stream reader
            let lexbuf = 
                UnicodeLexing.FunctionAsLexbuf (fun (chars, start, length) ->
                    streamReader.ReadBlock (chars, start, length)
                )
            usingLexbufForParsing (lexbuf, filePath) (fun lexbuf ->
                lexbufCallback lexargs lexbuf
            )

    let Lex pConfig sourceValue tokenCallback =
        let skip = false
        let errorLogger = CompilationErrorLogger("Lex", pConfig.tcConfig.errorSeverityOptions)
        LexAux pConfig sourceValue errorLogger (fun lexargs lexbuf ->
            while not lexbuf.IsPastEndOfStream do
                tokenCallback (Lexer.token lexargs skip lexbuf) lexbuf.LexemeRange
        )

[<RequireQualifiedAccess>]
module Parser =

    let Parse (pConfig: ParsingConfig) sourceValue =
        let skip = true
        let isLastCompiland = (pConfig.isLastFileOrScript, pConfig.isExecutable)
        let tcConfig = pConfig.tcConfig
        let filePath = pConfig.filePath
        let errorLogger = CompilationErrorLogger("Parse", tcConfig.errorSeverityOptions)

        let input =
                try
                    Lexer.LexAux pConfig sourceValue errorLogger (fun lexargs lexbuf ->
                        let tokenizer = LexFilter.LexFilter(lexargs.lightSyntaxStatus, tcConfig.compilingFslib, Lexer.token lexargs skip, lexbuf)
                        ParseInput(tokenizer.Lexer, errorLogger, lexbuf, None, filePath, isLastCompiland)
                    ) |> Some
                with
                | _ -> None
        (input, errorLogger.GetErrors ())