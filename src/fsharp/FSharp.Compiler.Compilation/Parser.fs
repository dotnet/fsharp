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
        supportsFeature: Features.LanguageFeature -> bool
    }

[<RequireQualifiedAccess>]
type SourceValue =
    | SourceText of SourceText
    | Stream of Stream

    member this.CreateLexbuf (supportsFeature) =
        match this with
        | SourceValue.SourceText sourceText ->
            UnicodeLexing.SourceTextAsLexbuf (supportsFeature, sourceText.ToFSharpSourceText ())
        | SourceValue.Stream stream ->
            let streamReader = new StreamReader(stream) // don't dispose of stream reader
            UnicodeLexing.FunctionAsLexbuf (supportsFeature, fun (chars, start, length) ->
                streamReader.ReadBlock (chars, start, length)
            )

[<RequireQualifiedAccess>]
module Lexer =

    open FSharp.Compiler.Lexhelp

    let LexAux (pConfig: ParsingConfig) lexbuf errorLogger lexbufCallback =
        let tcConfig = pConfig.tcConfig
        let filePath = pConfig.filePath

        let lightSyntaxStatus = LightSyntaxStatus (tcConfig.ComputeLightSyntaxInitialStatus filePath, true) 
        let conditionalCompilationDefines = pConfig.conditionalCompilationDefines
        let lexargs = mkLexargs (filePath, conditionalCompilationDefines@tcConfig.conditionalCompilationDefines, lightSyntaxStatus, Lexhelp.LexResourceManager (), ref [], errorLogger, tcConfig.pathMap)

        usingLexbufForParsing (lexbuf, filePath) (fun lexbuf ->
            lexbufCallback lexargs lexbuf
        )

    let Lex pConfig (sourceValue: SourceValue) tokenCallback (ct: CancellationToken) =
        let skip = false
        let errorLogger = CompilationErrorLogger("Lex", pConfig.tcConfig.errorSeverityOptions)
        LexAux pConfig (sourceValue.CreateLexbuf pConfig.supportsFeature) errorLogger (fun lexargs lexbuf ->
            while not lexbuf.IsPastEndOfStream do
                ct.ThrowIfCancellationRequested ()
                tokenCallback (Lexer.token lexargs skip lexbuf) lexbuf.LexemeRange
        )

[<RequireQualifiedAccess>]
module Parser =

    let ParseAux (pConfig: ParsingConfig) lexbuf lex =
        let isLastCompiland = (pConfig.isLastFileOrScript, pConfig.isExecutable)
        let tcConfig = pConfig.tcConfig
        let filePath = pConfig.filePath
        let errorLogger = CompilationErrorLogger("Parse", tcConfig.errorSeverityOptions)

        let input =
                try
                    Lexer.LexAux pConfig lexbuf errorLogger (fun lexargs lexbuf ->
                        let tokenizer = LexFilter.LexFilter(lexargs.lightSyntaxStatus, tcConfig.compilingFslib, lex lexargs, lexbuf)
                        ParseInput(tokenizer.Lexer, errorLogger, lexbuf, None, filePath, isLastCompiland)
                    ) |> Some
                with
                | _ -> None
        (input, errorLogger.GetErrorInfos ())

    let Parse pConfig (sourceValue: SourceValue) (ct: CancellationToken) =
        let skip = true
        ParseAux pConfig (sourceValue.CreateLexbuf pConfig.supportsFeature) (fun lexargs -> 
            (fun lexbuf -> ct.ThrowIfCancellationRequested (); Lexer.token lexargs skip lexbuf))

    let ParseWithTokens pConfig (tokens: ImmutableArray<Parser.token * range>) =
        if tokens.Length = 0 then
            invalidArg "tokens" "no tokens"

        let dummyLexbuf =
            Internal.Utilities.Text.Lexing.LexBuffer<char>.FromFunction (pConfig.supportsFeature, fun _ -> 0)

        let mutable index = 0
        let lex =
            (fun _ (lexbuf: Internal.Utilities.Text.Lexing.LexBuffer<char>) ->
                let t, r = tokens.[index]
                index <- index + 1
                lexbuf.StartPos <- Internal.Utilities.Text.Lexing.Position (r.FileIndex, r.StartLine, r.StartLine, 0, r.StartColumn)
                lexbuf.EndPos <- Internal.Utilities.Text.Lexing.Position (r.FileIndex, r.EndLine, r.EndLine, 0, r.EndColumn)
                t
            )

        ParseAux pConfig dummyLexbuf lex