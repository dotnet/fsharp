module FSharp.Compiler.Service.Tests.SyntaxTreeTests.ConditionalDirectiveTestsTests

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTrivia
open NUnit.Framework

let private getDirectiveTrivia isSignatureFile source =
    let ast = (if isSignatureFile then getParseResultsOfSignatureFile else getParseResults) source
    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(trivia = { ConditionalDirectives = trivia }))
    | ParsedInput.SigFile(ParsedSigFileInput(trivia = { ConditionalDirectives = trivia })) -> trivia

[<Test>]
let ``single #if / #endif`` () =
    let trivia =
        getDirectiveTrivia false """
let v =
    #if DEBUG
    ()
    #endif
    42
"""

    match trivia with
    | [ ConditionalDirectiveTrivia.If(expr, mIf)
        ConditionalDirectiveTrivia.EndIf mEndif ] ->
        assertRange (3, 4) (3, 13) mIf
        assertRange (5, 4) (5, 10) mEndif
        
        match expr with
        | IfDirectiveExpression.Ident "DEBUG" -> ()
        | _ -> Assert.Fail $"Expected different expression, got {expr}"
    | _ ->
        Assert.Fail $"Unexpected trivia, got {trivia}"

[<Test>]
let ``single #if / #else / #endif`` () =
    let trivia =
        getDirectiveTrivia false """
let v =
    #if DEBUG
    30
    #else
    42
    #endif
"""

    match trivia with
    | [ ConditionalDirectiveTrivia.If(expr, mIf)
        ConditionalDirectiveTrivia.Else mElse
        ConditionalDirectiveTrivia.EndIf mEndif ] ->
        assertRange (3, 4) (3, 13) mIf
        assertRange (5, 4) (5, 9) mElse
        assertRange (7, 4) (7, 10) mEndif
        
        match expr with
        | IfDirectiveExpression.Ident "DEBUG" -> ()
        | _ -> Assert.Fail $"Expected different expression, got {expr}"
    | _ ->
        Assert.Fail $"Unexpected trivia, got {trivia}"

[<Test>]
let ``nested #if / #else / #endif`` () =
    let trivia =
        getDirectiveTrivia false """
let v =
    #if FOO
        #if MEH
        1
        #else
        2
        #endif
    #else
        3
    #endif
"""

    match trivia with
    | [ ConditionalDirectiveTrivia.If(expr1, mIf1)
        ConditionalDirectiveTrivia.If(expr2, mIf2)
        ConditionalDirectiveTrivia.Else mElse1
        ConditionalDirectiveTrivia.EndIf mEndif1
        ConditionalDirectiveTrivia.Else mElse2
        ConditionalDirectiveTrivia.EndIf mEndif2 ] ->
        assertRange (3, 4) (3, 11) mIf1
        assertRange (4, 8) (4, 15) mIf2
        assertRange (6, 8) (6, 13) mElse1
        assertRange (8, 8) (8, 14) mEndif1
        assertRange (9, 4) (9, 9) mElse2
        assertRange (11, 4) (11, 10) mEndif2
        
        match expr1 with
        | IfDirectiveExpression.Ident "FOO" -> ()
        | _ -> Assert.Fail $"Expected different expression, got {expr1}"

        match expr2 with
        | IfDirectiveExpression.Ident "MEH" -> ()
        | _ -> Assert.Fail $"Expected different expression, got {expr2}"
    | _ ->
        Assert.Fail $"Unexpected trivia, got {trivia}"

[<Test>]
let ``nested #if / #endif with complex expressions`` () =
    let trivia =
        getDirectiveTrivia false """
let v =
    #if !DEBUG
        #if FOO && BAR
            #if MEH || HMM
                printfn "oh some logging"
            #endif
        #endif
    #endif
"""

    match trivia with
    | [ ConditionalDirectiveTrivia.If(expr1, mIf1)
        ConditionalDirectiveTrivia.If(expr2, mIf2)
        ConditionalDirectiveTrivia.If(expr3, mIf3)
        ConditionalDirectiveTrivia.EndIf mEndif1
        ConditionalDirectiveTrivia.EndIf mEndif2
        ConditionalDirectiveTrivia.EndIf mEndif3 ] ->
        assertRange (3, 4) (3, 14) mIf1
        assertRange (4, 8) (4, 22) mIf2
        assertRange (5, 12) (5, 26) mIf3
        assertRange (7, 12) (7, 18) mEndif1
        assertRange (8, 8) (8, 14) mEndif2
        assertRange (9, 4) (9, 10) mEndif3
        
        match expr1 with
        | IfDirectiveExpression.Not (IfDirectiveExpression.Ident "DEBUG") -> ()
        | _ -> Assert.Fail $"Expected different expression, got {expr1}"

        match expr2 with
        | IfDirectiveExpression.And(IfDirectiveExpression.Ident "FOO", IfDirectiveExpression.Ident "BAR") -> ()
        | _ -> Assert.Fail $"Expected different expression, got {expr2}"

        match expr3 with
        | IfDirectiveExpression.Or(IfDirectiveExpression.Ident "MEH", IfDirectiveExpression.Ident "HMM") -> ()
        | _ -> Assert.Fail $"Expected different expression, got {expr3}"
    | _ ->
        Assert.Fail $"Unexpected trivia, got {trivia}"

[<Test>]
let ``directives in multiline comment are not reported as trivia`` () =
    let trivia =
        getDirectiveTrivia false """
let v =
(*
#if DEBUG
()
#endif
*)
42
"""

    match trivia with
    | [] -> Assert.Pass()
    | _ ->
        Assert.Fail $"Unexpected trivia, got {trivia}"

[<Test>]
let ``directives in multiline string are not reported as trivia`` () =
    let trivia =
        getDirectiveTrivia false "
let v = \"\"\"
#if DEBUG
()
#endif
42
\"\"\"
"

    match trivia with
    | [] -> Assert.Pass()
    | _ ->
        Assert.Fail $"Unexpected trivia, got {trivia}"

[<Test>]
let ``single #if / #endif, signature file`` () =
    let trivia =
        getDirectiveTrivia true """
namespace Foobar

val v: int =
    #if DEBUG
    1
    #endif
    42
"""

    match trivia with
    | [ ConditionalDirectiveTrivia.If(expr, mIf)
        ConditionalDirectiveTrivia.EndIf mEndif ] ->
        assertRange (5, 4) (5, 13) mIf
        assertRange (7, 4) (7, 10) mEndif
        
        match expr with
        | IfDirectiveExpression.Ident "DEBUG" -> ()
        | _ -> Assert.Fail $"Expected different expression, got {expr}"
    | _ ->
        Assert.Fail $"Unexpected trivia, got {trivia}"

[<Test>]
let ``single #if / #else / #endif, signature file`` () =
    let trivia =
        getDirectiveTrivia true """
namespace Foobar

val v : int =
    #if DEBUG
    30
    #else
    42
    #endif
"""

    match trivia with
    | [ ConditionalDirectiveTrivia.If(expr, mIf)
        ConditionalDirectiveTrivia.Else mElse
        ConditionalDirectiveTrivia.EndIf mEndif ] ->
        assertRange (5, 4) (5, 13) mIf
        assertRange (7, 4) (7, 9) mElse
        assertRange (9, 4) (9, 10) mEndif
        
        match expr with
        | IfDirectiveExpression.Ident "DEBUG" -> ()
        | _ -> Assert.Fail $"Expected different expression, got {expr}"
    | _ ->
        Assert.Fail $"Unexpected trivia, got {trivia}"

[<Test>]
let ``nested #if / #else / #endif, signature file`` () =
    let trivia =
        getDirectiveTrivia true """
namespace Foobar

val v : int =
    #if FOO
        #if MEH
        1
        #else
        2
        #endif
    #else
        3
    #endif
"""

    match trivia with
    | [ ConditionalDirectiveTrivia.If(expr1, mIf1)
        ConditionalDirectiveTrivia.If(expr2, mIf2)
        ConditionalDirectiveTrivia.Else mElse1
        ConditionalDirectiveTrivia.EndIf mEndif1
        ConditionalDirectiveTrivia.Else mElse2
        ConditionalDirectiveTrivia.EndIf mEndif2 ] ->
        assertRange (5, 4) (5, 11) mIf1
        assertRange (6, 8) (6, 15) mIf2
        assertRange (8, 8) (8, 13) mElse1
        assertRange (10, 8) (10, 14) mEndif1
        assertRange (11, 4) (11, 9) mElse2
        assertRange (13, 4) (13, 10) mEndif2
        
        match expr1 with
        | IfDirectiveExpression.Ident "FOO" -> ()
        | _ -> Assert.Fail $"Expected different expression, got {expr1}"

        match expr2 with
        | IfDirectiveExpression.Ident "MEH" -> ()
        | _ -> Assert.Fail $"Expected different expression, got {expr2}"
    | _ ->
        Assert.Fail $"Unexpected trivia, got {trivia}"

[<Test>]
let ``nested #if / #endif with complex expressions, signature file`` () =
    let trivia =
        getDirectiveTrivia true """
namespace Foobar

val v : int =
    #if !DEBUG
        #if FOO && BAR
            #if MEH || HMM
                9
            #endif
        #endif
    #endif
    10
"""

    match trivia with
    | [ ConditionalDirectiveTrivia.If(expr1, mIf1)
        ConditionalDirectiveTrivia.If(expr2, mIf2)
        ConditionalDirectiveTrivia.If(expr3, mIf3)
        ConditionalDirectiveTrivia.EndIf mEndif1
        ConditionalDirectiveTrivia.EndIf mEndif2
        ConditionalDirectiveTrivia.EndIf mEndif3 ] ->
        assertRange (5, 4) (5, 14) mIf1
        assertRange (6, 8) (6, 22) mIf2
        assertRange (7, 12) (7, 26) mIf3
        assertRange (9, 12) (9, 18) mEndif1
        assertRange (10, 8) (10, 14) mEndif2
        assertRange (11, 4) (11, 10) mEndif3
        
        match expr1 with
        | IfDirectiveExpression.Not (IfDirectiveExpression.Ident "DEBUG") -> ()
        | _ -> Assert.Fail $"Expected different expression, got {expr1}"

        match expr2 with
        | IfDirectiveExpression.And(IfDirectiveExpression.Ident "FOO", IfDirectiveExpression.Ident "BAR") -> ()
        | _ -> Assert.Fail $"Expected different expression, got {expr2}"

        match expr3 with
        | IfDirectiveExpression.Or(IfDirectiveExpression.Ident "MEH", IfDirectiveExpression.Ident "HMM") -> ()
        | _ -> Assert.Fail $"Expected different expression, got {expr3}"
    | _ ->
        Assert.Fail $"Unexpected trivia, got {trivia}"

[<Test>]
let ``directives in multiline comment are not reported as trivia, signature file`` () =
    let trivia =
        getDirectiveTrivia true """
namespace Foobar

val v : int =
(*
#if DEBUG
()
#endif
*)
42
"""

    match trivia with
    | [] -> Assert.Pass()
    | _ ->
        Assert.Fail $"Unexpected trivia, got {trivia}"

[<Test>]
let ``directives in multiline string are not reported as trivia, signature file`` () =
    let trivia =
        getDirectiveTrivia true "
namespace Foobar

let v : string = \"\"\"
#if DEBUG
()
#endif
42
\"\"\"
"

    match trivia with
    | [] -> Assert.Pass()
    | _ ->
        Assert.Fail $"Unexpected trivia, got {trivia}"