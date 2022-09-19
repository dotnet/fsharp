module FSharp.Compiler.Service.Tests.SyntaxTreeTests.ComputationExpressionTestsTests

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open NUnit.Framework

[<Test>]
let ``SynExprAndBang range starts at and! and ends after expression`` () =
    let ast =
        getParseResults """
async {
    let! bar = getBar ()

    and! foo = getFoo ()

    return bar
}
"""

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(modules = [
        SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr (expr = SynExpr.App(argExpr = SynExpr.ComputationExpr(expr = SynExpr.LetOrUseBang(andBangs = [
                SynExprAndBang(range = mAndBang)
                ]))))
            ])
        ])) ->
        assertRange (5, 4) (5, 24) mAndBang
    | _ ->
        Assert.Fail "Could not get valid AST"

[<Test>]
let ``multiple SynExprAndBang have range that starts at and! and ends after expression`` () =
    let ast =
        getParseResults """
async {
    let! bar = getBar ()
    and! foo = getFoo () in
    and! meh = getMeh ()
    return bar
}
"""

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(modules = [
        SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Expr (expr = SynExpr.App(argExpr = SynExpr.ComputationExpr(expr = SynExpr.LetOrUseBang(andBangs = [
                SynExprAndBang(range = mAndBang1; trivia={ InKeyword = Some mIn })
                SynExprAndBang(range = mAndBang2)
                ]))))
            ])
        ])) ->
        assertRange (4, 4) (4, 24) mAndBang1
        assertRange (4, 25) (4, 27) mIn
        assertRange (5, 4) (5, 24) mAndBang2
    | _ ->
        Assert.Fail "Could not get valid AST"