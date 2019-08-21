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
open System.Buffers
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

open FSharp.Compiler.Lexhelp

[<AutoOpen>]
module LexerHelpers =

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

    let lexLexbufCustom (lexConfig: LexerConfig) lexbuf getNextToken lexCallback (ct: CancellationToken) : unit =
        usingLexbufForParsing (lexbuf, lexConfig.filePath) (fun lexbuf ->
            lexCallback lexbuf (fun lexbuf -> ct.ThrowIfCancellationRequested (); getNextToken lexbuf)
        )

    let lexLexbuf (pConfig: ParsingConfig) (flags: LexFlags) errorLogger lexbuf lexCallback (ct: CancellationToken) : unit =
        let lexConfig = pConfig.ToLexerConfig ()

        let filePath = lexConfig.filePath
        let lightSyntaxStatus = LightSyntaxStatus (lexConfig.IsLightSyntaxOn, true) 
        let lexargs = mkLexargs (filePath, lexConfig.conditionalCompilationDefines, lightSyntaxStatus, Lexhelp.LexResourceManager (), ref [], errorLogger, lexConfig.pathMap)
        let lexargs = { lexargs with applyLineDirectives = not lexConfig.IsCompiling }

        let getNextToken =
            let skip = (flags &&& LexFlags.SkipTrivia) = LexFlags.SkipTrivia
            let lexer = Lexer.token lexargs skip

            if (flags &&& LexFlags.UseLexFilter) = LexFlags.UseLexFilter then
                (fun lexbuf ->
                    let tokenizer = LexFilter.LexFilter(lexargs.lightSyntaxStatus, false, lexer, lexbuf)
                    tokenizer.Lexer lexbuf
                )
            else
                lexer

        lexLexbufCustom lexConfig lexbuf getNextToken lexCallback ct

    let lexText pConfig flags errorLogger (text: SourceText) lexCallback ct : unit =
        let lexbuf = UnicodeLexing.SourceTextAsLexbuf (pConfig.supportsFeature, text.ToFSharpSourceText ())
        lexLexbuf pConfig flags errorLogger lexbuf lexCallback ct

    let lexStream pConfig flags errorLogger (stream: Stream) lexCallback ct : unit =
        let streamReader = new StreamReader (stream) // don't dispose of stream reader
        let lexbuf = 
            UnicodeLexing.FunctionAsLexbuf (pConfig.supportsFeature, fun (chars, start, length) ->
                streamReader.ReadBlock (chars, start, length)
            )
        lexLexbuf pConfig flags errorLogger lexbuf lexCallback ct

[<Struct>]
type TokenItem = TokenItem of rawToken: Parser.token * span: TextSpan * startIndex: int

[<Struct>]
type TokenTextSpan =
    {
        startLineNumber: int
        startIndex: int
        startPos: int
        endLineNumber: int
        endIndex: int
        endPos: int
    }

[<Struct>]
type TokenCacheItem =
    {
        t: token
        columnStart: int
        columnEnd: int
        extraLineCount: int
    }

[<Sealed>]
type TokenCache private (_pConfig: ParsingConfig, text: SourceText, tokens: ResizeArray<TokenCacheItem> []) =

    member __.Add (t: token, m: range) =
        let lineNumber = m.StartLine - 1
        let lineTokens =
            if tokens.[lineNumber] <> null then
                tokens.[lineNumber]
            else
                let lineTokens = ResizeArray ()
                tokens.[lineNumber] <- lineTokens
                lineTokens

        lineTokens.Add { t = t; columnStart = m.StartColumn; columnEnd = m.EndColumn; extraLineCount = m.EndLine - m.StartLine }

    member this.GetTokenTextSpan (span: TextSpan) =
        let startLineNumber, startIndex, startPos, startEndPos = 
            match this.TryGetPreviousAvailableToken span.Start with
            | Some (TokenItem (span = span; startIndex = startIndex)) -> text.Lines.GetLinePosition(span.Start).Line, startIndex, span.Start, span.End
            | _ -> 0, 0, 0, 0

        let endLineNumber, endIndex, endPos =
            match this.TryGetNextAvailableToken startEndPos with
            | Some (TokenItem (span = span; startIndex = startIndex)) -> text.Lines.GetLinePosition(span.End).Line, startIndex, span.End
            | _ -> failwith "EOF token not found"

        { startLineNumber = startLineNumber; startIndex = startIndex; startPos = startPos
          endLineNumber = endLineNumber; endIndex = endIndex; endPos = endPos }

    member this.GetTokens (span: TextSpan) : TokenItem seq =
        let linePosSpan = text.Lines.GetLinePositionSpan span
        let lines = text.Lines
        seq {
            for lineNumber = linePosSpan.Start.Line to linePosSpan.End.Line do
                let lineTokens = tokens.[lineNumber]
                if lineTokens <> null then
                    let line = lines.[lineNumber]
                    for i = 0 to lineTokens.Count - 1 do
                        let tokenCacheItem = lineTokens.[i]
                        let startPos = line.Start + tokenCacheItem.columnStart
                        if tokenCacheItem.extraLineCount > 0 then
                            let endPos = lines.[lineNumber + tokenCacheItem.extraLineCount].Start + tokenCacheItem.columnEnd
                            let length = endPos - startPos
                            if length >= 0 && startPos >= span.Start && endPos <= span.End then
                                yield TokenItem (tokenCacheItem.t, TextSpan (startPos, length), i)
                        else
                            let endPos = line.Start + tokenCacheItem.columnEnd
                            let length = endPos - startPos
                            if length >= 0 && startPos >= span.Start && endPos <= span.End then
                                yield TokenItem (tokenCacheItem.t, TextSpan (startPos, length), i)
        }

    member this.GetTokensReverse (span: TextSpan) : TokenItem seq =
        let linePosSpan = text.Lines.GetLinePositionSpan span
        let lines = text.Lines
        seq {
            for lineNumber = linePosSpan.End.Line downto linePosSpan.Start.Line do
                let lineTokens = tokens.[lineNumber]
                if lineTokens <> null then
                    let line = lines.[lineNumber]
                    for i = lineTokens.Count - 1 downto 0 do
                        let tokenCacheItem = lineTokens.[i]
                        let startPos = line.Start + tokenCacheItem.columnStart
                        if tokenCacheItem.extraLineCount > 0 then
                            let endPos = lines.[lineNumber + tokenCacheItem.extraLineCount].Start + tokenCacheItem.columnEnd
                            let length = endPos - startPos
                            if length >= 0 && startPos >= span.Start && endPos <= span.End then
                                yield TokenItem (tokenCacheItem.t, TextSpan (startPos, length), i)
                        else
                            let endPos = line.Start + tokenCacheItem.columnEnd
                            let length = endPos - startPos
                            if length >= 0 && startPos >= span.Start && endPos <= span.End then
                                yield TokenItem (tokenCacheItem.t, TextSpan (startPos, length), i)
        }

    member this.TryFindTokenItem (position: int) =
        let line = text.Lines.GetLineFromPosition position
        this.GetTokens (line.Span)
        |> Seq.tryFind (fun (TokenItem (_, span, _)) ->
            span.Contains position || span.End = position
        )

    member this.TryGetNextAvailableToken (position: int) =
        this.GetTokens (TextSpan (position, text.Length - position))
        |> Seq.tryFind (fun (TokenItem (t, span, _)) ->
            span.End >= position
        )

    member this.TryGetPreviousAvailableToken (position: int) =
        this.GetTokensReverse (TextSpan (0, position))
        |> Seq.tryFind (fun (TokenItem (t, span, _)) ->
            match t with
            | token.EOF _ -> false
            | _ ->
                span.Start <= position
        )
        
    override __.Finalize () =
        printfn "finalizer kicked off, returning array to pool"
        ArrayPool<ResizeArray<TokenCacheItem>>.Shared.Return (tokens, clearArray = true)

    static member Create (pConfig, text) =
        TokenCache (pConfig, text, ArrayPool<ResizeArray<TokenCacheItem>>.Shared.Rent text.Lines.Count)

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

    member this.AreTokensCached = lazyCachedTokens.IsSome

    member this.GetTokens (span, ct) =
        let tokens = this.GetCachedTokens ct
        tokens.GetTokens span

    member this.LexFilter (errorLogger, f, ct) =
        lex LexFlags.UseLexFilter errorLogger f ct

    member this.WithChangedTextSnapshot (newTextSnapshot: FSharpSourceSnapshot) =
        if obj.ReferenceEquals (textSnapshot, newTextSnapshot) then
            this
        else
            match newTextSnapshot.TryGetText (), this.TryGetCachedTokens () with
            | Some newText, Some tokens ->
                IncrementalLexer (pConfig, newTextSnapshot, lazy None) // TODO: Actual incremental?
            | _ ->
                IncrementalLexer (pConfig, newTextSnapshot, lazy None)

    static member Create (pConfig, textSnapshot) =
        IncrementalLexer (pConfig, textSnapshot, lazy None)