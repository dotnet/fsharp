module FSharp.Compiler.Service.Tests.SyntaxTreeTests.IfThenElseTestsTests

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open NUnit.Framework

[<Test>]
let ``If keyword in IfThenElse`` () =
    let parseResults = 
        getParseResults
            "if a then b"

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Expr(
            expr = SynExpr.IfThenElse(trivia={ IfKeyword = mIfKw; IsElif = false; ThenKeyword = mThenKw; ElseKeyword = None })
        )
    ]) ])) ->
        assertRange (1, 0) (1, 2) mIfKw
        assertRange (1, 5) (1, 9) mThenKw
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Else keyword in simple IfThenElse`` () =
    let parseResults = 
        getParseResults
            "if a then b else c"

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Expr(
            expr =SynExpr.IfThenElse(trivia={ IfKeyword = mIfKw; IsElif = false; ThenKeyword = mThenKw; ElseKeyword = Some mElse })
        )
    ]) ])) ->
        assertRange (1, 0) (1, 2) mIfKw
        assertRange (1, 5) (1, 9) mThenKw
        assertRange (1, 12) (1, 16) mElse
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``If, Then and Else keyword on separate lines`` () =
    let parseResults = 
        getParseResults
            """
if a
then b
else c"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Expr(
            expr = SynExpr.IfThenElse(trivia={ IfKeyword = mIfKw; IsElif = false; ThenKeyword = mThenKw; ElseKeyword = Some mElse })
        )
    ]) ])) ->
        assertRange (2, 0) (2, 2) mIfKw
        assertRange (3, 0) (3, 4) mThenKw
        assertRange (4, 0) (4, 4) mElse
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Nested elif in IfThenElse`` () =
    let parseResults = 
        getParseResults
            """
if a then
b
elif c then d"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Expr(
            expr = SynExpr.IfThenElse(trivia={ IfKeyword = mIfKw; IsElif=false; ThenKeyword = mThenKw; ElseKeyword = None }
                                      elseExpr = Some (SynExpr.IfThenElse(trivia={ IfKeyword = mElif; IsElif = true })))
        )
    ]) ])) ->
        assertRange (2, 0) (2, 2) mIfKw
        assertRange (2, 5) (2, 9) mThenKw
        assertRange (4, 0) (4, 4) mElif
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Nested else if in IfThenElse`` () =
    let parseResults = 
        getParseResults
            """
if a then
    b
else
    if c then d"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Expr(
            expr = SynExpr.IfThenElse(trivia={ IfKeyword = mIfKw; IsElif = false; ThenKeyword = mThenKw; ElseKeyword = Some mElse }
                                      elseExpr = Some (SynExpr.IfThenElse(trivia={ IfKeyword = mElseIf; IsElif = false })))
        )
    ]) ])) ->
        assertRange (2, 0) (2, 2) mIfKw
        assertRange (2, 5) (2, 9) mThenKw
        assertRange (4, 0) (4, 4) mElse
        assertRange (5, 4) (5, 6) mElseIf
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Nested else if on the same line in IfThenElse`` () =
    let parseResults = 
        getParseResults
            """
if a then
b
else if c then
d"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Expr(
            expr = SynExpr.IfThenElse(trivia={ IfKeyword = mIfKw; IsElif=false; ThenKeyword = mThenKw; ElseKeyword = Some mElse }
                                      elseExpr = Some (SynExpr.IfThenElse(trivia={ IfKeyword = mElseIf; IsElif = false })))
        )
    ]) ])) ->
        assertRange (2, 0) (2, 2) mIfKw
        assertRange (2, 5) (2, 9) mThenKw
        assertRange (4, 0) (4, 4) mElse
        assertRange (4, 5) (4, 7) mElseIf
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Deeply nested IfThenElse`` () =
    let parseResults = 
        getParseResults
            """
if a then
    b
elif c then
    d
else
        if e then
            f
        else
            g"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Expr(
            expr = SynExpr.IfThenElse(trivia={ IfKeyword = mIf1; IsElif = false; ElseKeyword = None }
                                      elseExpr = Some (SynExpr.IfThenElse(trivia={ IfKeyword = mElif; IsElif = true; ElseKeyword = Some mElse1 }
                                                                          elseExpr = Some (SynExpr.IfThenElse(trivia={ IfKeyword = mIf2; IsElif = false; ElseKeyword = Some mElse2 }))))))
    ]) ])) ->
        assertRange (2, 0) (2, 2) mIf1
        assertRange (4, 0) (4, 4) mElif
        assertRange (6, 0) (6, 4) mElse1
        assertRange (7, 8) (7, 10) mIf2
        assertRange (9, 8) (9, 12) mElse2

    | _ -> Assert.Fail "Could not get valid AST"
    
[<Test>]
let ``Comment between else and if`` () =
    let parseResults = 
        getParseResults
            """
if a then
b
else (* some long comment here *) if c then
d"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ SynModuleOrNamespace.SynModuleOrNamespace(decls = [
        SynModuleDecl.Expr(
            expr = SynExpr.IfThenElse(trivia={ IfKeyword = mIf1; IsElif = false; ElseKeyword = Some mElse }
                                      elseExpr = Some (SynExpr.IfThenElse(trivia = { IfKeyword = mIf2; IsElif = false }))))
    ]) ])) ->
        assertRange (2, 0) (2, 2) mIf1
        assertRange (4, 0) (4, 4) mElse
        assertRange (4, 34) (4, 36) mIf2

    | _ -> Assert.Fail "Could not get valid AST"