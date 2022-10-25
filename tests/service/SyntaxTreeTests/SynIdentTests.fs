module FSharp.Compiler.Service.Tests.SyntaxTreeTests.SynIdentTests

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open NUnit.Framework

[<Test>]
let ``Incomplete long ident`` () =
    let ast =
        """
module Module

A.
"""
        |> getParseResults

    match ast with
    | ParsedInput.ImplFile (ParsedImplFileInput(contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls =
        [ SynModuleDecl.Expr(expr = SynExpr.LongIdent (longDotId = lid)) ]) ])) ->
        Assert.AreEqual(1, lid.IdentsWithTrivia.Length)
    | _ -> Assert.Fail $"Could not get valid AST, got {ast}"

[<Test>]
let ``IdentsWithTrivia with unbalance collection should not throw`` () =
    let synLongIdent =
        SynLongIdent([ Ident("A", Range.Zero); Ident("B", Range.Zero) ], [ Range.Zero ], [ None ])

    match synLongIdent.IdentsWithTrivia with
    | [ SynIdent (_, None); SynIdent (_, None) ] -> Assert.Pass()
    | identsWithTrivia -> Assert.Fail $"Unexpected identsWithTrivia, got {identsWithTrivia}"
