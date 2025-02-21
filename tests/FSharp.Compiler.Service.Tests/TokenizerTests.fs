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
