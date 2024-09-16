namespace CompilerDirectives

open Microsoft.FSharp.Control
open Xunit
open FSharp.Test.Compiler
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Text
open FSharp.Compiler.Syntax

module Line =

    let checker = FSharpChecker.Create()

    let parse (source: string) =
        let langVersion = "preview"
        let sourceFileName = __SOURCE_FILE__
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
    [<InlineData(2, case2, "Line.fs:(2,0--2,1)")>]
    [<InlineData(3, case3, "Line.fs:(2,0--2,1)")>]
    let ``check expr range interacting with line directive`` (case, source, expectedRange) =
        let parseResults = parse source
        if parseResults.ParseHadErrors then failwith "unexpected: parse error"
        let exprRange =
            match parseResults.ParseTree with
            | ParsedInput.ImplFile(ParsedImplFileInput(contents = contents)) ->
                let (SynModuleOrNamespace(decls = decls)) = List.exactlyOne contents
                match List.exactlyOne decls with
                | SynModuleDecl.Expr(_, range) -> $"{range.FileName}:{range}"
                | _ -> failwith $"unexpected: not an expr"
            | ParsedInput.SigFile _ -> failwith "unexpected: sig file"
        if exprRange <> expectedRange then
            failwith $"case{case}: expected: {expectedRange}, found {exprRange}"
        