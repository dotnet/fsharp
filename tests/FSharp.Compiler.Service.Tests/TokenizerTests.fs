#nowarn "57" // FSharpLexer.Tokenize is experimental
module FSharp.Compiler.Service.Tests.TokenizerTests

open FSharp.Compiler.Tokenization
open FSharp.Test
open Xunit

let rec parseLine(line: string, state: FSharpTokenizerLexState ref, tokenizer: FSharpLineTokenizer) = seq {
  match tokenizer.ScanToken(state.Value) with
  | Some(tok), nstate ->
      let str = line.Substring(tok.LeftColumn, tok.RightColumn - tok.LeftColumn + 1)
      yield str, tok
      state.Value <- nstate
      yield! parseLine(line, state, tokenizer)
  | None, nstate -> 
      state.Value <- nstate }

let tokenizeLines (lines:string[]) =
  let sourceTok = FSharpSourceTokenizer([], Some "C:\\test.fsx", None, None)
  [ let state = ref FSharpTokenizerLexState.Initial
    for n, line in lines |> Seq.zip [ 0 .. lines.Length-1 ] do

      let tokenizer = sourceTok.CreateLineTokenizer(line)
      yield n, parseLine(line, state, tokenizer) |> List.ofSeq ]

/// Scans every token of a (possibly multi-line) source using a single line tokenizer,
/// threading the lex state across embedded newlines (column index resets at each newline).
let scanTokens (defines: string list) (source: string) =
    let sourceTok = FSharpSourceTokenizer(defines, Some "C:\\test.fsx", None, None)
    let tokenizer = sourceTok.CreateLineTokenizer(source)
    let rec loop (state: FSharpTokenizerLexState) acc =
        match tokenizer.ScanToken(state) with
        | Some tok, nstate -> loop nstate (tok :: acc)
        | None, _ -> List.rev acc
    loop FSharpTokenizerLexState.Initial []

[<Fact>]
let ``Tokenizer test - simple let with string``() =
    let tokenizedLines = 
      tokenizeLines
        [| "// Sets the hello world variable"
           "let hello = \"Hello world\" " |]

    let actual = 
        [ for lineNo, lineToks in tokenizedLines do
            yield lineNo, [ for str, info in lineToks do yield info.TokenName, str ] ]
    let expected = 
      [(0,
        [("LINE_COMMENT", "//"); ("LINE_COMMENT", " "); ("LINE_COMMENT", "Sets");
         ("LINE_COMMENT", " "); ("LINE_COMMENT", "the"); ("LINE_COMMENT", " ");
         ("LINE_COMMENT", "hello"); ("LINE_COMMENT", " ");
         ("LINE_COMMENT", "world"); ("LINE_COMMENT", " ");
         ("LINE_COMMENT", "variable")]);
       (1,
        [("LET", "let"); ("WHITESPACE", " "); ("IDENT", "hello");
         ("WHITESPACE", " "); ("EQUALS", "="); ("WHITESPACE", " ");
         ("STRING_TEXT", "\""); ("STRING_TEXT", "Hello"); ("STRING_TEXT", " ");
         ("STRING_TEXT", "world"); ("STRING", "\""); ("WHITESPACE", " ")])]

    if actual <> expected then 
        printfn "actual   = %A" actual
        printfn "expected = %A" expected
        actual |> Assert.shouldBeEqualWith expected (sprintf "actual and expected did not match,actual =\n%A\nexpected=\n%A\n" actual expected)

[<Fact>]
let ``Tokenizer test 2 - single line non-nested string interpolation``() =
    let tokenizedLines = 
      tokenizeLines
        [| "// Tests tokenizing string interpolation"
           "let hello0 = $\"\""
           "let hello1 = $\"Hello world\"  "
           "let hello2 = $\"Hello world {1+1} = {2}\" "
           "let hello0v = @$\"\""
           "let hello1v = @$\"Hello world\"  "
           "let hello2v = @$\"Hello world {1+1} = {2}\" " |]

    let actual = 
        [ for lineNo, lineToks in tokenizedLines do
            yield lineNo, [ for str, info in lineToks do yield info.TokenName, str ] ]
    let expected = 
        [(0,
          [("LINE_COMMENT", "//"); ("LINE_COMMENT", " "); ("LINE_COMMENT", "Tests");
           ("LINE_COMMENT", " "); ("LINE_COMMENT", "tokenizing"); ("LINE_COMMENT", " ");
           ("LINE_COMMENT", "string"); ("LINE_COMMENT", " ");
           ("LINE_COMMENT", "interpolation")]);
         (1,
          [("LET", "let"); ("WHITESPACE", " "); ("IDENT", "hello0"); ("WHITESPACE", " ");
           ("EQUALS", "="); ("WHITESPACE", " "); ("STRING_TEXT", "$\"");
           ("INTERP_STRING_BEGIN_END", "\"")]);
         (2,
          [("LET", "let"); ("WHITESPACE", " "); ("IDENT", "hello1"); ("WHITESPACE", " ");
           ("EQUALS", "="); ("WHITESPACE", " "); ("STRING_TEXT", "$\"");
           ("STRING_TEXT", "Hello"); ("STRING_TEXT", " "); ("STRING_TEXT", "world");
           ("INTERP_STRING_BEGIN_END", "\""); ("WHITESPACE", "  ")]);
         (3,
          [("LET", "let"); ("WHITESPACE", " "); ("IDENT", "hello2"); ("WHITESPACE", " ");
           ("EQUALS", "="); ("WHITESPACE", " "); ("STRING_TEXT", "$\"");
           ("STRING_TEXT", "Hello"); ("STRING_TEXT", " "); ("STRING_TEXT", "world");
           ("STRING_TEXT", " "); ("INTERP_STRING_BEGIN_PART", "{"); ("INT32", "1");
           ("PLUS_MINUS_OP", "+"); ("INT32", "1"); ("STRING_TEXT", "}");
           ("STRING_TEXT", " "); ("STRING_TEXT", "="); ("STRING_TEXT", " ");
           ("INTERP_STRING_PART", "{"); ("INT32", "2"); ("STRING_TEXT", "}");
           ("INTERP_STRING_END", "\""); ("WHITESPACE", " ")]);
         (4,
          [("LET", "let"); ("WHITESPACE", " "); ("IDENT", "hello0v");
           ("WHITESPACE", " "); ("EQUALS", "="); ("WHITESPACE", " ");
           ("STRING_TEXT", "@$\""); ("INTERP_STRING_BEGIN_END", "\"")]);
         (5,
          [("LET", "let"); ("WHITESPACE", " "); ("IDENT", "hello1v");
           ("WHITESPACE", " "); ("EQUALS", "="); ("WHITESPACE", " ");
           ("STRING_TEXT", "@$\""); ("STRING_TEXT", "Hello"); ("STRING_TEXT", " ");
           ("STRING_TEXT", "world"); ("INTERP_STRING_BEGIN_END", "\"");
           ("WHITESPACE", "  ")]);
         (6,
          [("LET", "let"); ("WHITESPACE", " "); ("IDENT", "hello2v");
           ("WHITESPACE", " "); ("EQUALS", "="); ("WHITESPACE", " ");
           ("STRING_TEXT", "@$\""); ("STRING_TEXT", "Hello"); ("STRING_TEXT", " ");
           ("STRING_TEXT", "world"); ("STRING_TEXT", " ");
           ("INTERP_STRING_BEGIN_PART", "{"); ("INT32", "1"); ("PLUS_MINUS_OP", "+");
           ("INT32", "1"); ("STRING_TEXT", "}"); ("STRING_TEXT", " ");
           ("STRING_TEXT", "="); ("STRING_TEXT", " "); ("INTERP_STRING_PART", "{");
           ("INT32", "2"); ("STRING_TEXT", "}"); ("INTERP_STRING_END", "\"");
           ("WHITESPACE", " ")]);]
  
    if actual <> expected then 
        printfn "actual   = %A" actual
        printfn "expected = %A" expected
        actual |> Assert.shouldBeEqualWith expected (sprintf "actual and expected did not match,actual =\n%A\nexpected=\n%A\n" actual expected)

[<Fact>]
let ``Tokenizer test - multiline non-nested string interpolation``() =
    let tokenizedLines = 
      tokenizeLines
        [| "let hello1t = $\"\"\"abc {1+"
           " 1} def\"\"\"" |]

    let actual = 
        [ for lineNo, lineToks in tokenizedLines do
            yield lineNo, [ for str, info in lineToks do yield info.TokenName, str ] ]
    let expected = 
        [(0,
          [("LET", "let"); ("WHITESPACE", " "); ("IDENT", "hello1t");
           ("WHITESPACE", " "); ("EQUALS", "="); ("WHITESPACE", " ");
           ("STRING_TEXT", "$\"\"\""); ("STRING_TEXT", "abc"); ("STRING_TEXT", " ");
           ("INTERP_STRING_BEGIN_PART", "{"); ("INT32", "1"); ("PLUS_MINUS_OP", "+")]);
         (1,
          [("WHITESPACE", " "); ("INT32", "1"); ("STRING_TEXT", "}");
           ("STRING_TEXT", " "); ("STRING_TEXT", "def"); ("INTERP_STRING_END", "\"\"\"")])]
  
    if actual <> expected then 
        printfn "actual   = %A" actual
        printfn "expected = %A" expected
        actual |> Assert.shouldBeEqualWith expected (sprintf "actual and expected did not match,actual =\n%A\nexpected=\n%A\n" actual expected)

[<Fact>]
// checks nested '{' and nested single-quote strings
let ``Tokenizer test - multi-line nested string interpolation``() =
    let tokenizedLines = 
      tokenizeLines
        [| "let hello1t = $\"\"\"abc {\"a\" +               "
           "                          {                     "
           "                           contents = \"b\"     "
           "                          }.contents            "
           "                         } def\"\"\"" |]

    let actual = 
        [ for lineNo, lineToks in tokenizedLines do
            yield lineNo, [ for str, info in lineToks do yield info.TokenName, str ] ]
    let expected = 
        [(0,
          [("LET", "let"); ("WHITESPACE", " "); ("IDENT", "hello1t");
           ("WHITESPACE", " "); ("EQUALS", "="); ("WHITESPACE", " ");
           ("STRING_TEXT", "$\"\"\""); ("STRING_TEXT", "abc"); ("STRING_TEXT", " ");
           ("INTERP_STRING_BEGIN_PART", "{"); ("STRING_TEXT", "\""); ("STRING_TEXT", "a");
           ("STRING", "\""); ("WHITESPACE", " "); ("PLUS_MINUS_OP", "+");
           ("WHITESPACE", "               ")]);
         (1,
          [("WHITESPACE", "                          "); ("LBRACE", "{");
           ("WHITESPACE", "                     ")]);
         (2,
          [("WHITESPACE", "                           "); ("IDENT", "contents");
           ("WHITESPACE", " "); ("EQUALS", "="); ("WHITESPACE", " ");
           ("STRING_TEXT", "\""); ("STRING_TEXT", "b"); ("STRING", "\"");
           ("WHITESPACE", "     ")]);
         (3,
          [("WHITESPACE", "                          "); ("RBRACE", "}"); ("DOT", ".");
           ("IDENT", "contents"); ("WHITESPACE", "            ")]);
         (4,
          [("WHITESPACE", "                         "); ("STRING_TEXT", "}");
           ("STRING_TEXT", " "); ("STRING_TEXT", "def"); ("INTERP_STRING_END", "\"\"\"")])]
  
    if actual <> expected then 
        printfn "actual   = %A" actual
        printfn "expected = %A" expected
        actual |> Assert.shouldBeEqualWith expected (sprintf "actual and expected did not match,actual =\n%A\nexpected=\n%A\n" actual expected)

[<Fact>]
let ``Tokenizer test - single-line nested string interpolation``() =
    let tokenizedLines = 
      tokenizeLines
        [| " $\"abc { { contents = 1 } }\"     " |]

    let actual = 
        [ for lineNo, lineToks in tokenizedLines do
            yield lineNo, [ for str, info in lineToks do yield info.TokenName, str ] ]
    let expected = 
        [(0,
          [("WHITESPACE", " "); ("STRING_TEXT", "$\""); ("STRING_TEXT", "abc");
           ("STRING_TEXT", " "); ("INTERP_STRING_BEGIN_PART", "{"); ("WHITESPACE", " ");
           ("LBRACE", "{"); ("WHITESPACE", " "); ("IDENT", "contents");
           ("WHITESPACE", " "); ("EQUALS", "="); ("WHITESPACE", " "); ("INT32", "1");
           ("WHITESPACE", " "); ("RBRACE", "}"); ("WHITESPACE", " ");
           ("STRING_TEXT", "}"); ("INTERP_STRING_END", "\""); ("WHITESPACE", "     ")])]
  
    if actual <> expected then 
        printfn "actual   = %A" actual
        printfn "expected = %A" expected
        actual |> Assert.shouldBeEqualWith expected (sprintf "actual and expected did not match,actual =\n%A\nexpected=\n%A\n" actual expected)

[<Fact>]
let ``Tokenizer test - elif directive produces HASH_ELIF token``() =
    let defines = ["DEBUG"]
    let sourceTok = FSharpSourceTokenizer(defines, Some "C:\\test.fsx", None, None)
    let lines =
        [| "#if DEBUG"
           "let x = 1"
           "#elif RELEASE"
           "let x = 2"
           "#endif" |]
    let state = ref FSharpTokenizerLexState.Initial
    let allTokens =
        [ for line in lines do
            let tokenizer = sourceTok.CreateLineTokenizer(line)
            let lineTokens = parseLine(line, state, tokenizer) |> List.ofSeq
            yield lineTokens ]

    // Line 0: #if DEBUG → HASH_IF keyword
    let line0Names = allTokens.[0] |> List.map (fun (_, tok) -> tok.TokenName)
    Assert.Contains("HASH_IF", line0Names)

    // Line 1: let x = 1 → active code, should have LET token
    let line1Names = allTokens.[1] |> List.map (fun (_, tok) -> tok.TokenName)
    Assert.Contains("LET", line1Names)

    // Line 2: #elif RELEASE → split into HASH_IF + WHITESPACE + IDENT by processHashIfLine
    let line2Names = allTokens.[2] |> List.map (fun (_, tok) -> tok.TokenName)
    Assert.Contains("HASH_IF", line2Names)

    // Line 3: let x = 2 → should be INACTIVECODE (since DEBUG is defined, #elif branch is skipped)
    let line3Names = allTokens.[3] |> List.map (fun (_, tok) -> tok.TokenName)
    Assert.Contains("INACTIVECODE", line3Names)

[<Fact>]
let ``FSharpLexer.Tokenize produces HashElif token kind``() =
    let source = """#if DEBUG
let x = 1
#elif RELEASE
let x = 2
#endif"""
    let tokens = ResizeArray<FSharpToken>()
    let flags = FSharpLexerFlags.Default &&& ~~~FSharpLexerFlags.SkipTrivia
    FSharpLexer.Tokenize(FSharp.Compiler.Text.SourceText.ofString source, tokens.Add, langVersion = "preview", conditionalDefines = ["DEBUG"], flags = flags)
    let hasHashElif = tokens |> Seq.exists (fun t -> t.Kind = FSharpTokenKind.HashElif)
    Assert.True(hasHashElif, "Expected at least one token with Kind = FSharpTokenKind.HashElif")

[<Fact>]
let ``Unfinished idents``() =
    let tokenizedLines =
      tokenizeLines
        [| "`"; "``"; "``a"; "``a`"; "```" |]

    let actual =
        [ for lineTokens in tokenizedLines |> List.map snd do
            [ for str, info in lineTokens -> info.TokenName, str ] ]

    let expected =
        [["IDENT", "`"]
         ["IDENT", "``"]
         ["IDENT", "``a"]
         ["IDENT", "``a`"]
         ["IDENT", "```"]]

    actual |> Assert.shouldBe expected

[<Fact>]
let ``Tokenizer test - optional parameters with question mark``() =
    let tokenizedLines =
      tokenizeLines
        [| "member _.memb(?optional:string) = optional" |]

    let actual =
        [ for lineNo, lineToks in tokenizedLines do
            yield lineNo, [ for str, info in lineToks do yield info.TokenName, str ] ]
    
    let expected =
        [(0,
          [("MEMBER", "member"); ("WHITESPACE", " "); ("UNDERSCORE", "_"); ("DOT", ".");
           ("IDENT", "memb"); ("LPAREN", "("); ("QMARK", "?");
           ("IDENT", "optional"); ("COLON", ":"); ("IDENT", "string");
           ("RPAREN", ")"); ("WHITESPACE", " "); ("EQUALS", "="); ("WHITESPACE", " ");
           ("IDENT", "optional")])]
    
    if actual <> expected then
        printfn "actual   = %A" actual
        printfn "expected = %A" expected
        actual |> Assert.shouldBeEqualWith expected (sprintf "actual and expected did not match,actual =\n%A\nexpected=\n%A\n" actual expected)

[<Fact>]
let ``Lexer.CommentsLexing.Bug1548``() =
    let cm = FSharpTokenColorKind.Comment
    let kw = FSharpTokenColorKind.Keyword

    // This specifies the source code to test and a collection of tokens that
    // we want to find in the result (note: it doesn't have to contain every token, because
    // behavior for some of them is undefined - e.g. "(* "\"*)" - what is token here?
    let sources =
      [ "// some comment",
            [ ((0, 1), cm); ((2, 2), cm); ((3, 6), cm); ((7, 7), cm); ((8, 14), cm) ]
        "// (* hello // 12345\nlet",
            [ ((6, 10), cm); ((15, 19), cm); ((0, 2), kw) ] // checks 'hello', '12345' and keyword 'let'
        "//- test",
            [ ((0, 2), cm); ((4, 7), cm) ] // checks whether '//-' isn't treated as an operator

        // same thing for XML comments - these are treated in a different lexer branch
        "/// some comment",
            [ ((0, 2), cm); ((3, 3), cm); ((4, 7), cm); ((8, 8), cm); ((9, 15), cm) ]
        "/// (* hello // 12345\nmember",
            [ ((7, 11), cm); ((16, 20), cm); ((0, 5), kw) ]
        "///- test",
            [ ((0, 3), cm); ((5, 8), cm) ]

        // same thing for "////" - these are treated in a different lexer branch
        "//// some comment",
            [ ((0, 3), cm); ((4, 4), cm); ((5, 8), cm); ((9, 9), cm); ((10, 16), cm) ]
        "//// (* hello // 12345\nlet",
            [ ((8, 12), cm); ((17, 21), cm); ((0, 2), kw) ]
        "////- test",
            [ ((0, 4), cm); ((6, 9), cm) ]

        "(* test 123 (* 456 nested *) comments *)",
            [ ((3, 6), cm); ((8, 10), cm); ((15, 17), cm); ((19, 24), cm); ((29, 36), cm) ] // checks 'test', '123', '456', 'nested', 'comments'
        "(* \"with 123 \\\" *)\" string *)",
            [ ((4, 7), cm); ((9, 11), cm); ((20, 25), cm) ] // checks 'with', '123', 'string'
        "(* @\"with 123 \"\" *)\" string *)",
            [ ((5, 8), cm); ((10, 12), cm); ((21, 26), cm) ] // checks 'with', '123', 'string'
      ]

    for lineText, expected in sources do
        // Lex the (possibly multi-line) source and add every lexed token's color to a dictionary
        let lexed = System.Collections.Generic.Dictionary<int * int, FSharpTokenColorKind>()
        for tok in scanTokens [ "COMPILED"; "EDITING" ] lineText do
            lexed[(tok.LeftColumn, tok.RightColumn)] <- tok.ColorClass

        // Verify that all tokens in the specified list occur in the lexed result with the right color
        for pos, clr in expected do
            let succ, v = lexed.TryGetValue(pos)
            let found = [ for kvp in lexed -> kvp.Key, kvp.Value ]
            Assert.True(succ, sprintf "Cannot find token %A at %A in %A\nFound: %A" clr pos lineText found)
            Assert.True((clr = v), sprintf "Wrong color of token %A at %A in %A\nFound: %A" clr pos lineText found)

[<Fact>]
let ``TokenInfo.TriggerClasses``() =
    let punct = FSharpTokenColorKind.Punctuation
    let delim = FSharpTokenCharKind.Delimiter

    // Tokenize a minimal source, return the (ColorClass, CharClass, TriggerClass) of the first token
    let triggerInfoOf (tokenName: string) (source: string) =
        let toks = scanTokens [] source
        match toks |> List.tryFind (fun t -> t.TokenName = tokenName) with
        | Some t -> (t.ColorClass, t.CharClass, t.FSharpTokenTriggerClass)
        | None ->
            failwithf "Token %s was not produced by source %A. Tokens: %A"
                tokenName source (toks |> List.map (fun t -> t.TokenName))

    // important - tokens with specific trigger classes used to drive IntelliSense
    triggerInfoOf "DOT" "a.b"
    |> Assert.shouldBe (punct, delim, FSharpTokenTriggerClass.MemberSelect)             // member select for dot completions
    triggerInfoOf "LPAREN" "f(x,y)"
    |> Assert.shouldBe (punct, delim, FSharpTokenTriggerClass.ParamStart ||| FSharpTokenTriggerClass.MatchBraces) // for parameter info
    triggerInfoOf "COMMA" "f(x,y)"
    |> Assert.shouldBe (punct, delim, FSharpTokenTriggerClass.ParamNext)
    triggerInfoOf "RPAREN" "f(x,y)"
    |> Assert.shouldBe (punct, delim, FSharpTokenTriggerClass.ParamEnd ||| FSharpTokenTriggerClass.MatchBraces)

    // matching - other cases where we expect MatchBraces
    let matchBracesInfo = (punct, delim, FSharpTokenTriggerClass.MatchBraces)
    triggerInfoOf "LQUOTE" "<@ 1 @>" |> Assert.shouldBe matchBracesInfo
    triggerInfoOf "LBRACK" "[ 1 ]" |> Assert.shouldBe matchBracesInfo
    triggerInfoOf "LBRACE" "{ x = 1 }" |> Assert.shouldBe matchBracesInfo
    triggerInfoOf "LBRACK_BAR" "[| 1 |]" |> Assert.shouldBe matchBracesInfo
    triggerInfoOf "RQUOTE" "<@ 1 @>" |> Assert.shouldBe matchBracesInfo
    triggerInfoOf "RBRACK" "[ 1 ]" |> Assert.shouldBe matchBracesInfo
    triggerInfoOf "RBRACE" "{ x = 1 }" |> Assert.shouldBe matchBracesInfo
    triggerInfoOf "BAR_RBRACK" "[| 1 |]" |> Assert.shouldBe matchBracesInfo

// Each case has exactly one brace pair: left brace at the start marker, right brace at the end marker.
[<Fact>]
let ``MatchingBraces.VerifyMatches``() =
    let lines =
        [ ""
          "                let x = (1, 2)//1"
          "                let y =    (  3 + 1  ) * 2"
          "                let z ="
          "                   async {"
          "                       return 10"
          "                   }"
          "                let lst = "
          "                    [// list_start"
          "                        1;2;3"
          "                    ]//list_end"
          "                let arr = "
          "                    [|"
          "                        1"
          "                        2"
          "                    |]"
          "                let quote = <@(* S0 *) 1 @>(* E0 *)"
          "                let quoteWithNestedList = <@(* S1 *) ['x';'y';'z'](* E_L*) @>(* E1 *)"
          "                [< System.Serializable() >]"
          "                type T = class end"
          "            " ]
    let source = String.concat "\n" lines
    let linesArr = List.toArray lines
    let braces = matchBraces ("MatchingBracesVerifyMatches", source)

    // Locate the START of the marker substring (0-based row/col).
    let findMarker (marker: string) =
        let mutable found = None
        let mutable i = 0
        while found.IsNone && i < linesArr.Length do
            let idx = linesArr[i].IndexOf(marker, System.StringComparison.Ordinal)
            if idx >= 0 then found <- Some(i, idx)
            i <- i + 1
        match found with
        | Some p -> p
        | None -> failwithf "Marker %A not found in source" marker

    let checkBraces startMarker endMarker (expectedSpanLen: int) =
        let (startRow, startCol) = findMarker startMarker
        let (endRow, endCol) = findMarker endMarker

        // exactly one matching pair has its left brace at the start marker (FCS line is 1-based)
        let matching =
            braces |> Array.filter (fun (l, _) -> l.StartLine = startRow + 1 && l.StartColumn = startCol)
        Assert.Equal(1, matching.Length)

        let (lbrace, rbrace) = matching[0]
        // left brace span: single line, starts at the start marker, expectedSpanLen columns wide
        Assert.Equal(lbrace.StartLine, lbrace.EndLine)
        Assert.Equal(startRow + 1, lbrace.StartLine)
        Assert.Equal(startCol, lbrace.StartColumn)
        Assert.Equal(startCol + expectedSpanLen, lbrace.EndColumn)
        // right brace span: single line, starts at the end marker, expectedSpanLen columns wide
        Assert.Equal(rbrace.StartLine, rbrace.EndLine)
        Assert.Equal(endRow + 1, rbrace.StartLine)
        Assert.Equal(endCol, rbrace.StartColumn)
        Assert.Equal(endCol + expectedSpanLen, rbrace.EndColumn)

    checkBraces "(1" ")//1" 1
    checkBraces "( " ") *" 1
    checkBraces "{" "}" 1
    checkBraces "[// list_start" "]//list_end" 1
    checkBraces "[|" "|]" 2
    checkBraces "<@(* S0 *)" "@>(* E0 *)" 2
    checkBraces "<@(* S1 *)" "@>(* E1 *)" 2
    checkBraces "['x'" "](* E_L*)" 1
    checkBraces "[<" ">]" 2
