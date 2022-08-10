module FSharp.Compiler.Service.Tests.SyntaxTreeTests.CodeCommentTestsTests

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTrivia
open NUnit.Framework

let private getCommentTrivia isSignatureFile source =
    let ast = (if isSignatureFile then getParseResultsOfSignatureFile else getParseResults) source
    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(trivia = { CodeComments = trivia }))
    | ParsedInput.SigFile(ParsedSigFileInput(trivia = { CodeComments = trivia })) -> trivia

[<Test>]
let ``comment on single line`` () =
    let trivia =
        getCommentTrivia false """
// comment!
foo()
"""

    match trivia with
    | [ CommentTrivia.LineComment mComment ] ->
        assertRange (2, 0) (2, 11) mComment
    | _ ->
        Assert.Fail "Could not get valid AST"

[<Test>]
let ``comment on single line, signature file`` () =
    let trivia =
        getCommentTrivia true """
namespace Meh
// comment!
foo()
"""

    match trivia with
    | [ CommentTrivia.LineComment mComment ] ->
        assertRange (3, 0) (3, 11) mComment
    | _ ->
        Assert.Fail "Could not get valid AST"

[<Test>]
let ``comment after source code`` () =
    let trivia =
        getCommentTrivia false """
foo() // comment!
"""

    match trivia with
    | [ CommentTrivia.LineComment mComment ] ->
        assertRange (2, 6) (2, 17) mComment
    | _ ->
        Assert.Fail "Could not get valid AST"

[<Test>]
let ``comment after source code, signature file`` () =
    let trivia =
        getCommentTrivia true """
namespace Meh

val foo : int // comment!
"""

    match trivia with
    | [ CommentTrivia.LineComment mComment ] ->
        assertRange (4, 14) (4, 25) mComment
    | _ ->
        Assert.Fail "Could not get valid AST"

[<Test>]
let ``block comment in source code`` () =
    let trivia =
        getCommentTrivia false """
let a (* b *)  c = c + 42
"""

    match trivia with
    | [ CommentTrivia.BlockComment mComment ] ->
        assertRange (2, 6) (2, 13) mComment
    | _ ->
        Assert.Fail "Could not get valid AST"

[<Test>]
let ``block comment in source code, signature file`` () =
    let trivia =
        getCommentTrivia true """
namespace Meh

val a (* b *) : int
"""

    match trivia with
    | [ CommentTrivia.BlockComment mComment ] ->
        assertRange (4, 6) (4, 13) mComment
    | _ ->
        Assert.Fail "Could not get valid AST"

[<Test>]
let ``comment at end of file`` () =
    let trivia =
        getCommentTrivia false "x // y"

    match trivia with
    | [ CommentTrivia.LineComment mComment ] ->
        assertRange (1, 2) (1, 6) mComment
    | _ ->
        Assert.Fail "Could not get valid AST"

[<Test>]
let ``triple slash comment should not be captured`` () =
    let trivia =
        getCommentTrivia false """
/// Some great documentation comment
let x = 0
"""

    match trivia with
    | [] ->
        Assert.Pass()
    | _ ->
        Assert.Fail "Could not get valid AST"

[<Test>]
let ``triple slash comment should be captured, if used in an invalid location`` () =
    let trivia =
        getCommentTrivia false """
/// Valid xml doc
let x =
    /// Some great documentation comment

    /// With a blank line in between
    /// but on a while loop
    while true do ()
    a + 1
"""

    match trivia with
    | [ CommentTrivia.LineComment m1
        CommentTrivia.LineComment m2
        CommentTrivia.LineComment m3 ] ->
        assertRange (4, 4) (4, 40) m1
        assertRange (6, 4) (6, 36) m2
        assertRange (7, 4) (7, 27) m3
    | _ ->
        Assert.Fail "Could not get valid AST"