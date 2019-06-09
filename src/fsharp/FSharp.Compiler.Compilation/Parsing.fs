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

    let Lex (pConfig: ParsingConfig) sourceValue tokenCallback =
        let skipWhitespace = true
        let tcConfig = pConfig.tcConfig
        let filePath = pConfig.filePath
        let errorLogger = CompilationErrorLogger("Lex", tcConfig.errorSeverityOptions)

        let lightSyntaxStatus = LightSyntaxStatus (tcConfig.ComputeLightSyntaxInitialStatus filePath, true) 
        let conditionalCompilationDefines = pConfig.conditionalCompilationDefines
        let lexargs = mkLexargs (filePath, conditionalCompilationDefines@tcConfig.conditionalCompilationDefines, lightSyntaxStatus, Lexhelp.LexResourceManager (), ref [], errorLogger, tcConfig.pathMap)

        match sourceValue with
        | SourceValue.SourceText sourceText ->
            let lexbuf = UnicodeLexing.SourceTextAsLexbuf (sourceText.ToFSharpSourceText ())
            usingLexbufForParsing (lexbuf, filePath) (fun lexbuf ->
                while not lexbuf.IsPastEndOfStream do
                    tokenCallback (Lexer.token lexargs skipWhitespace lexbuf) lexbuf.LexemeRange
            )
        | SourceValue.Stream stream ->
            let streamReader = new StreamReader(stream) // don't dispose of stream reader
            let lexbuf = 
                UnicodeLexing.FunctionAsLexbuf (fun (chars, start, length) ->
                    streamReader.ReadBlock (chars, start, length)
                )
            usingLexbufForParsing (lexbuf, filePath) (fun lexbuf ->
                while not lexbuf.IsPastEndOfStream do
                    tokenCallback (Lexer.token lexargs skipWhitespace lexbuf) lexbuf.LexemeRange
            )

[<RequireQualifiedAccess>]
module Parser =

    let Parse (pConfig: ParsingConfig) sourceValue =
        let tcConfig = pConfig.tcConfig
        let filePath = pConfig.filePath
        let errorLogger = CompilationErrorLogger("Parse", tcConfig.errorSeverityOptions)

        let input =
            match sourceValue with
            | SourceValue.SourceText sourceText ->
                ParseOneInputSourceText (pConfig.tcConfig, Lexhelp.LexResourceManager (), pConfig.conditionalCompilationDefines, filePath, sourceText.ToFSharpSourceText (), (pConfig.isLastFileOrScript, pConfig.isExecutable), errorLogger)
            | SourceValue.Stream stream ->
                ParseOneInputStream (pConfig.tcConfig, Lexhelp.LexResourceManager (), pConfig.conditionalCompilationDefines, filePath, stream, (pConfig.isLastFileOrScript, pConfig.isExecutable), errorLogger)
        (input, errorLogger.GetErrors ())