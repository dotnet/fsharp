namespace FSharp.Compiler.Compilation

open System
open System.IO
open System.Threading
open System.Collections.Generic
open System.Collections.Immutable
open System.Collections.ObjectModel
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
open FSharp.Compiler.Parser
open Internal.Utilities

[<Flags>]
type LexerFlags =
    | None                          = 0x00000
    | LightSyntaxOn                 = 0x00001
    | Compiling                     = 0x00010 
    | CompilingFSharpCore           = 0x00110
    | SkipTrivia                    = 0x01000
    | SkipTriviaButResolveComments  = 0x10000

type LexerConfig =
    {
        filePath: string
        flags: LexerFlags
        conditionalCompilationDefines: string list
        pathMap: PathMap
    }

    member this.IsLightSyntaxOn =
        (this.flags &&& LexerFlags.LightSyntaxOn) = LexerFlags.LightSyntaxOn

    member this.IsCompiling =
        (this.flags &&& LexerFlags.Compiling) = LexerFlags.Compiling

    member this.IsCompilingFSharpCore =
        (this.flags &&& LexerFlags.CompilingFSharpCore) = LexerFlags.CompilingFSharpCore

    member this.CanSkipTrivia =
        (this.flags &&& LexerFlags.SkipTrivia) = LexerFlags.SkipTrivia

    member this.CanSkipTriviaButResolveComments =
        (this.flags &&& LexerFlags.SkipTriviaButResolveComments) = LexerFlags.SkipTriviaButResolveComments

[<AutoOpen>]
module LexerHelpers =

    open FSharp.Compiler.Lexhelp

    type ParsingConfig with
    
        member this.ToLexerConfig () =
            let mutable flags = LexerFlags.SkipTrivia
            flags <- if this.tcConfig.ComputeLightSyntaxInitialStatus this.filePath then flags ||| LexerFlags.LightSyntaxOn else flags
            flags <- if this.tcConfig.compilingFslib then LexerFlags.CompilingFSharpCore else flags
            {
                filePath = this.filePath
                flags = flags
                conditionalCompilationDefines = this.conditionalCompilationDefines @ this.tcConfig.conditionalCompilationDefines
                pathMap = this.tcConfig.pathMap
            }

    [<Flags>]
    type LexFlags =
        | None =        0x00
        | SkipTrivia =      0x01
        | UseLexFilter =    0x11 // lexfilter must skip all trivia

    let private lexLexbuf (pConfig: ParsingConfig) (flags: LexFlags) errorLogger lexbuf lexCallback (ct: CancellationToken) =
        let lexConfig = pConfig.ToLexerConfig ()

        let filePath = lexConfig.filePath
        let lightSyntaxStatus = LightSyntaxStatus (lexConfig.IsLightSyntaxOn, true) 
        let lexargs = mkLexargs (filePath, lexConfig.conditionalCompilationDefines, lightSyntaxStatus, Lexhelp.LexResourceManager (), ref [], errorLogger, lexConfig.pathMap)

        let getNextToken =
            let skip = (flags &&& LexFlags.SkipTrivia) = LexFlags.SkipTrivia
            let lexer = Lexer.token lexargs skip

            if (flags &&& LexFlags.UseLexFilter) = LexFlags.UseLexFilter then
                (fun lexbuf ->
                    let tokenizer = LexFilter.LexFilter(lexargs.lightSyntaxStatus, lexConfig.IsCompilingFSharpCore, lexer, lexbuf)
                    tokenizer.Lexer lexbuf
                )
            else
                lexer

        usingLexbufForParsing (lexbuf, filePath) (fun lexbuf ->
            lexCallback lexbuf (fun lexbuf -> ct.ThrowIfCancellationRequested (); getNextToken lexbuf)
        )

    let lexText pConfig flags errorLogger (text: SourceText) lexCallback ct =
        let lexbuf = UnicodeLexing.SourceTextAsLexbuf (text.ToFSharpSourceText ())
        lexLexbuf pConfig flags errorLogger lexbuf lexCallback ct

    let lexStream pConfig flags errorLogger (stream: Stream) lexCallback ct =
        let streamReader = new StreamReader (stream) // don't dispose of stream reader
        let lexbuf = 
            UnicodeLexing.FunctionAsLexbuf (fun (chars, start, length) ->
                streamReader.ReadBlock (chars, start, length)
            )
        lexLexbuf pConfig flags errorLogger lexbuf lexCallback ct

[<Struct>]
type TokenItem = TokenItem of rawToken: Parser.token * span: TextSpan

[<Struct>]
type TokenCacheItem =
    {
        t: token
        columnStart: int
        columnEnd: int
        lineCount: int
    }

// TODO: We need to chunk the tokens instead of having a large resize array.
[<Sealed>]
type TokenCache private (pConfig: ParsingConfig, text: SourceText, tokens: ResizeArray<ResizeArray<TokenCacheItem>>) =

    member __.Add (t: token, m: range) =
        let lineNumber = m.StartLine - 1
        let lineTokens =
            if tokens.Count > lineNumber && tokens.[lineNumber] <> null then
                tokens.[lineNumber]
            else
                if lineNumber >= tokens.Count then
                    tokens.AddRange (Array.zeroCreate (lineNumber - (tokens.Count - 1)))

                let lineTokens = ResizeArray ()
                tokens.[lineNumber] <- lineTokens
                lineTokens

        let x = { t = t; columnStart = m.StartColumn; columnEnd = m.EndColumn; lineCount = m.EndLine - m.StartLine }
        lineTokens.Add x

    member private __.InsertRange (index, collection) =
        tokens.InsertRange (index, collection)

    member private __.RemoveRange (index, count) =
        tokens.RemoveRange (index, count)

    member private __.Item
        with get i = tokens.[i]
        and  set i value = tokens.[i] <- value

    member this.TryCreateIncremental (newText: SourceText) =
        let changes = newText.GetTextChanges text
        if changes.Count = 0 || changes.Count > 1 || (changes.[0].Span.Start = 0 && changes.[0].Span.Length = text.Length) then
            printfn "no incremental"
            None
        else
            // For now, we only do incremental lexing on one change.
            let change = Seq.head changes

            let newTokens = TokenCache (pConfig, newText, ResizeArray tokens)
            let lineNumbersToEval = ResizeArray ()

            let span = TextSpan (change.Span.Start, change.NewText.Length)

            let linePosSpan = text.Lines.GetLinePositionSpan change.Span
            let newLinePosSpan = newText.Lines.GetLinePositionSpan span

            let startLine = linePosSpan.Start.Line
            let lineLength = linePosSpan.End.Line - linePosSpan.Start.Line
            let newLineLength = newLinePosSpan.End.Line - linePosSpan.Start.Line

            for i = startLine to startLine + newLineLength do
                lineNumbersToEval.Add i

            if newLineLength > lineLength then
                let start = startLine + lineLength
                let length = startLine + newLineLength - start
                newTokens.InsertRange (start + 1, Array.init length (fun _ -> ResizeArray ()))

            elif newLineLength < lineLength then
                let start = startLine + newLineLength
                let length = startLine + lineLength - start
                newTokens.RemoveRange (start + 1, length)

            for i = 0 to lineNumbersToEval.Count - 1 do
                let lineNumber = lineNumbersToEval.[i]
                newTokens.[lineNumber] <- ResizeArray ()
                let subText = (newText.GetSubText (newText.Lines.[lineNumber].Span))

                let errorLogger = CompilationErrorLogger("TryCreateIncremental", pConfig.tcConfig.errorSeverityOptions)
                lexText pConfig LexFlags.SkipTrivia errorLogger subText (fun lexbuf getNextToken ->
                    while not lexbuf.IsPastEndOfStream do
                        match getNextToken lexbuf with
                        // EOFs will be created at the end of any given text.
                        // Because we are lexing multiple texts, we don't want to include EOF except when are on the last line.
                        | token.EOF _ when not ((newText.Lines.Count - 1) = lineNumber) -> ()
                        | t -> 
                            let m = lexbuf.LexemeRange
                            let adjustedm = 
                                let startPos = (mkPos (m.Start.Line + lineNumber) m.Start.Column)
                                let endPos = (mkPos (m.End.Line + lineNumber) m.End.Column)
                                mkFileIndexRange m.FileIndex startPos endPos
                            newTokens.Add (t, adjustedm)
                            
                ) CancellationToken.None

            Some newTokens

    member __.GetTokens (span: TextSpan) : TokenItem seq =
        let lines = text.Lines
        let linePosSpan = lines.GetLinePositionSpan span
        seq {
            for lineNumber = linePosSpan.Start.Line to linePosSpan.End.Line do
                if tokens.Count > lineNumber then
                    let lineTokens = tokens.[lineNumber]
                    if lineTokens <> null then
                        let line = lines.[lineNumber]
                        for i = 0 to lineTokens.Count - 1 do
                            let tokenCacheItem = lineTokens.[i]
                            let span = 
                                if tokenCacheItem.lineCount > 0 then
                                    let start = line.Start + tokenCacheItem.columnStart
                                    TextSpan (start, lines.[lineNumber + tokenCacheItem.lineCount].Start + tokenCacheItem.columnEnd - start)
                                else
                                    let start = line.Start + tokenCacheItem.columnStart
                                    TextSpan (start, line.Start + tokenCacheItem.columnEnd - start)
                            yield TokenItem (tokenCacheItem.t, span)
        }

    static member Create (pConfig, text) =
        TokenCache (pConfig, text, ResizeArray ())

[<Sealed>]
type IncrementalLexer (pConfig: ParsingConfig, textSnapshot: FSharpSourceSnapshot, incrementalTokenCacheOpt: Lazy<TokenCache option>) =

    let gate = obj ()

    let lex flags errorLogger lexCallback ct =

        let lex = 
            // Check to see if we have a cached text; can happen when GetText is called, even for a stream.
            match textSnapshot.TryGetText () with
            | Some text -> lexText pConfig flags errorLogger text
            | _ ->
                // If we don't have cached text, check to see if textSnapshot is a stream.
                match textSnapshot.TryGetStream ct with
                | Some stream -> lexStream pConfig flags errorLogger stream
                | _ ->
                    // Looks like we have text that is not cached.
                    let text = textSnapshot.GetText ct
                    lexText pConfig flags errorLogger text


        lex lexCallback ct

    let lexEverything lexCallback ct =
        let errorLogger = CompilationErrorLogger("simpleLexEverything", pConfig.tcConfig.errorSeverityOptions)
        lex LexFlags.SkipTrivia errorLogger lexCallback ct

    let getCachedTokens ct =
        let text = textSnapshot.GetText ct
        let tokenCache = TokenCache.Create (pConfig, text)

        lexEverything (fun lexbuf getNextToken ->
            while not lexbuf.IsPastEndOfStream do
                let t = getNextToken lexbuf
                let m = lexbuf.LexemeRange
                tokenCache.Add (t, m)
        ) ct

        tokenCache

    let mutable lazyCachedTokens = None

    member __.TryGetCachedTokens () = lazyCachedTokens
    
    member __.GetCachedTokens ct =
        match lazyCachedTokens with
        | Some tokens ->
            tokens
        | _ ->
            lock gate (fun () ->
                match lazyCachedTokens with
                | Some tokens ->
                    tokens
                | _ ->
                    match incrementalTokenCacheOpt.Value with
                    | Some cachedTokens ->
                        lazyCachedTokens <- Some cachedTokens
                        cachedTokens
                    | _ ->
                        let tokens = getCachedTokens ct
                        lazyCachedTokens <- Some tokens
                        tokens
            )

    member this.GetTokens (span, ct) =
        let tokens = this.GetCachedTokens ct
        tokens.GetTokens span

    member __.LexFilter (errorLogger, f, ct) =
        lex LexFlags.UseLexFilter errorLogger f ct

    member this.WithChangedTextSnapshot (newTextSnapshot: FSharpSourceSnapshot) =
        if obj.ReferenceEquals (textSnapshot, newTextSnapshot) then
            this
        else
            match newTextSnapshot.TryGetText (), this.TryGetCachedTokens () with
            | Some newText, Some tokens ->
                IncrementalLexer (pConfig, newTextSnapshot, lazy tokens.TryCreateIncremental newText)
            | _ ->
                IncrementalLexer (pConfig, newTextSnapshot, lazy None)

    static member Create (pConfig, textSnapshot) =
        IncrementalLexer (pConfig, textSnapshot, lazy None)