
#if INTERACTIVE
#r "../../Debug/net40/bin/FSharp.Compiler.Service.dll" // note, run 'build fcs' to generate this, this DLL has a public API so can be used from F# Interactive
#r "../../packages/NUnit.3.5.0/lib/net45/nunit.framework.dll"
#load "FsUnit.fs"
#load "Common.fs"
#else
module FSharp.Compiler.Service.Tests.TokenizerTests
#endif

open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Interactive.Shell
open Microsoft.FSharp.Compiler.SourceCodeServices

open NUnit.Framework
open FsUnit
open System
open System.IO


let sourceTok = FSharpSourceTokenizer([], Some "C:\\test.fsx")

let tokenizeLines (lines:string[]) =
  [ let state = ref 0L
    for n, line in lines |> Seq.zip [ 0 .. lines.Length ] do
      let tokenizer = sourceTok.CreateLineTokenizer(line)
      let rec parseLine() = seq {
        match tokenizer.ScanToken(!state) with
        | Some(tok), nstate ->
            let str = line.Substring(tok.LeftColumn, tok.RightColumn - tok.LeftColumn + 1)
            yield str, tok
            state := nstate
            yield! parseLine()
        | None, nstate -> state := nstate }
      yield n, parseLine() |> List.ofSeq ]

[<Test>]
let ``Tokenizer test 1``() =
    let tokenizedLines = 
      tokenizeLines
        [| "// Sets the hello wrold variable"
           "let hello = \"Hello world\" " |]

    let actual = 
        [ for lineNo, lineToks in tokenizedLines do
            yield lineNo, [ for str, info in lineToks do yield info.TokenName, str ] ]
    let expected = 
      [(0,
        [("LINE_COMMENT", "//"); ("LINE_COMMENT", " "); ("LINE_COMMENT", "Sets");
         ("LINE_COMMENT", " "); ("LINE_COMMENT", "the"); ("LINE_COMMENT", " ");
         ("LINE_COMMENT", "hello"); ("LINE_COMMENT", " ");
         ("LINE_COMMENT", "wrold"); ("LINE_COMMENT", " ");
         ("LINE_COMMENT", "variable")]);
       (1,
        [("LET", "let"); ("WHITESPACE", " "); ("IDENT", "hello");
         ("WHITESPACE", " "); ("EQUALS", "="); ("WHITESPACE", " ");
         ("STRING_TEXT", "\""); ("STRING_TEXT", "Hello"); ("STRING_TEXT", " ");
         ("STRING_TEXT", "world"); ("STRING", "\""); ("WHITESPACE", " ")])]

    Assert.AreEqual(actual, expected)

