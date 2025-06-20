namespace CompilerDirectives

open Microsoft.FSharp.Control
open Xunit
open Internal.Utilities
open FSharp.Compiler
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Features
open FSharp.Compiler.Lexhelp
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open FSharp.Compiler.UnicodeLexing
open FSharp.Test.Compiler

module Line =

    let parse (source: string) =
        let checker = FSharpChecker.Create()
        let langVersion = "preview"
        let sourceFileName = "Line.fs"
        let parsingOptions =
            { FSharpParsingOptions.Default with
                SourceFiles = [| sourceFileName |]
                LangVersionText = langVersion
                ApplyLineDirectives = true
                }
        checker.ParseFile(sourceFileName, SourceText.ofString source, parsingOptions) |> Async.RunSynchronously


    [<Literal>]
    let private case1 = """module A
#line 1 "xyz.fs"
(
printfn ""
)
    """
    
    [<Literal>]
    let private case2 = """module A
(
#line 1 "xyz.fs"
printfn ""
)
    """
    
    [<Literal>]
    let private case3 = """module A
(
#line 1 "xyz.fs"
)
    """

    [<Theory>]
    [<InlineData(1, case1, "xyz.fs:(1,0--3,1)")>]
    [<InlineData(2, case2, "Line.fs:(2,0--5,1)")>]
    [<InlineData(3, case3, "Line.fs:(2,0--4,1)")>]
    let ``check expr range interacting with line directive`` (case, source, expectedRange) =
        let parseResults = parse source
        if parseResults.ParseHadErrors then failwith "unexpected: parse error"
        let exprRange =
            match parseResults.ParseTree with
            | ParsedInput.ImplFile(ParsedImplFileInput(contents = contents)) ->
                let (SynModuleOrNamespace(decls = decls)) = List.exactlyOne contents
                match List.exactlyOne decls with
                | SynModuleDecl.Expr(_, range) ->
                    let range = range.ApplyLineDirectives()
                    $"{range.FileName}:{range}"
                | _ -> failwith $"unexpected: not an expr"
            | ParsedInput.SigFile _ -> failwith "unexpected: sig file"
        if exprRange <> expectedRange then
            failwith $"case{case}: expected: {expectedRange}, found {exprRange}"


    [<Fact>]
    let TestLineDirectives () =
        let source =
            """
    module A
    3
    4
    #line 5 "test.fsl"
    6
    7
    #line 10 "test.fsy"
    9
    10
    """
        let errors =
            source
            |> FSharp
            |> withFileName "test.fs"
            |> compile
            |> shouldFail
            |> fun cr -> cr.Output.PerFileErrors |> List.map (fun (fn, d) -> fn, d.Range.StartLine)
        let expectedErrors =
            [("test.fs", 3); ("test.fs", 4); ("test.fsl", 5); ("test.fsl", 6); ("test.fsy", 10); ("test.fsy", 11)]
        Assert.True(
            List.forall2 (fun (e_fn, e_line) (fn, line) -> e_fn = fn && e_line = line) expectedErrors errors,
            sprintf "Expected: %A, Found: %A" expectedErrors errors
        )

    [<Fact>]
    let TestNoLineDirectives () =
        let source =
            """
    module A
    3
    4
    """
        let errors =
            source
            |> FSharp
            |> withFileName "test.fs"
            |> compile
            |> shouldFail
            |> fun cr -> cr.Output.PerFileErrors |> List.map (fun (fn, d) -> fn, d.Range.StartLine)
        let expectedErrors =
            [("test.fs", 3); ("test.fs", 4)]
        Assert.True(
            List.forall2 (fun (e_fn, e_line) (fn, line) -> e_fn = fn && e_line = line) expectedErrors errors,
            sprintf "Expected: %A, Found: %A" expectedErrors errors
        )



    let private getTokens sourceText =
        let langVersion = LanguageVersion.Default
        let lexargs =
            mkLexargs (
                [],
                IndentationAwareSyntaxStatus(true, false),
                LexResourceManager(),
                [],
                DiscardErrorsLogger,
                PathMap.empty,
                true
            )
        let lexbuf = StringAsLexbuf(true, langVersion, None, sourceText)
        resetLexbufPos "test.fs" lexbuf
        let tokenizer _ =
            let t = Lexer.token lexargs true lexbuf
            let p = lexbuf.StartPos
            t, FileIndex.fileOfFileIndex p.FileIndex, p.Line
        let isNotEof(t,_,_) = match t with Parser.EOF _ -> false | _ -> true
        Seq.initInfinite tokenizer |> Seq.takeWhile isNotEof |> Seq.toList

    let private code = """
1
#line 5 "other.fs"
2
#line 10 "test.fs"
3
"""

    let private expected = [
        "test.fs", 2
        "test.fs", 4
        "test.fs", 6
    ]

    [<Fact>]
    let checkOriginalLineNumbers() =
        let tokens = getTokens code
        Assert.Equal(expected.Length, tokens.Length)
        for ((e_idx, e_line), (_, idx, line)) in List.zip expected tokens do
            Assert.Equal(e_idx, idx)
            Assert.Equal(e_line, line)
