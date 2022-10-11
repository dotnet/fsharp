module FSharp.Compiler.Service.Tests.SyntaxTreeTests.LambdaTestsTests

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open NUnit.Framework

[<Test>]
let ``Lambda with two parameters gives correct body`` () =
    let parseResults = 
        getParseResults
            "fun a b -> x"

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Expr(
            expr = SynExpr.Lambda(parsedData = Some([SynPat.Named _; SynPat.Named _], SynExpr.Ident ident))
        )
    ]) ])) ->
        Assert.AreEqual("x", ident.idText)
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Lambda with wild card parameter gives correct body`` () =
    let parseResults = 
        getParseResults
            "fun a _ b -> x"

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Expr(
            expr = SynExpr.Lambda(parsedData = Some([SynPat.Named _; SynPat.Wild _; SynPat.Named _], SynExpr.Ident ident))
        )
    ]) ])) ->
        Assert.AreEqual("x", ident.idText)
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Lambda with tuple parameter with wild card gives correct body`` () =
    let parseResults = 
        getParseResults
            "fun a (b, _) c -> x"

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Expr(
            expr = SynExpr.Lambda(parsedData = Some([SynPat.Named _; SynPat.Paren(SynPat.Tuple _,_); SynPat.Named _], SynExpr.Ident ident))
        )
    ]) ])) ->
        Assert.AreEqual("x", ident.idText)
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Lambda with wild card that returns a lambda gives correct body`` () =
    let parseResults = 
        getParseResults
            "fun _ -> fun _ -> x"

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Expr(
            expr = SynExpr.Lambda(parsedData = Some([SynPat.Wild _], SynExpr.Lambda(parsedData = Some([SynPat.Wild _], SynExpr.Ident ident))))
        )
    ]) ])) ->
        Assert.AreEqual("x", ident.idText)
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Simple lambda has arrow range`` () =
    let parseResults = 
        getParseResults
            "fun x -> x"

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Expr(
            expr = SynExpr.Lambda(trivia={ ArrowRange = Some mArrow })
        )
    ]) ])) ->
        assertRange (1, 6) (1, 8) mArrow
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Multiline lambda has arrow range`` () =
    let parseResults = 
        getParseResults
            "fun x y z
                            ->
                                x * y * z"

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Expr(
            expr = SynExpr.Lambda(trivia={ ArrowRange = Some mArrow })
        )
    ]) ])) ->
        assertRange (2, 28) (2, 30) mArrow
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Destructed lambda has arrow range`` () =
    let parseResults = 
        getParseResults
            "fun { X = x } -> x * 2"

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Expr(
            expr = SynExpr.Lambda(trivia={ ArrowRange = Some mArrow })
        )
    ]) ])) ->
        assertRange (1, 14) (1, 16) mArrow
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Tuple in lambda has arrow range`` () =
    let parseResults = 
        getParseResults
            "fun (x, _) -> x * 3"

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Expr(
            expr = SynExpr.Lambda(trivia={ ArrowRange = Some mArrow })
        )
    ]) ])) ->
        assertRange (1, 11) (1, 13) mArrow
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Complex arguments lambda has arrow range`` () =
    let parseResults = 
        getParseResults
            "fun (x, _) 
    ({ Y = h::_ }) 
    (SomePattern(z)) 
    -> 
    x * y + z"

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Expr(
            expr = SynExpr.Lambda(trivia={ ArrowRange = Some mArrow })
        )
    ]) ])) ->
        assertRange (4, 4) (4, 6) mArrow
    | _ -> Assert.Fail "Could not get valid AST"
