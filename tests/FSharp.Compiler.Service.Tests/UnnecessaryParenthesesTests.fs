module FSharp.Compiler.EditorServices.Tests.UnnecessaryParenthesesTests

open FSharp.Compiler.EditorServices
open FSharp.Compiler.Service.Tests.Common
open NUnit.Framework

let noUnneededParens =
    [
        "printfn \"Hello, world.\""
        "()"
        "(1 + 2) * 3"
        "let (~-) x = x in id -(<@ 3 @>)"
    ]

[<Theory; TestCaseSource(nameof noUnneededParens)>]
let ``No results returned when there are no unnecessary parentheses`` src =
    task {
        let ast = getParseResults src
        let! unnecessaryParentheses = UnnecessaryParentheses.getUnnecessaryParentheses (fun _ -> src) ast
        Assert.IsEmpty unnecessaryParentheses
    }

let unneededParens =
    [
        "(printfn \"Hello, world.\")"
        "(())"
        "(1 * 2) * 3"
        "let (~-) x = x in -(<@ 3 @>)"
    ]

[<Theory; TestCaseSource(nameof unneededParens)>]
let ``Results returned when there are unnecessary parentheses`` src =
    task {
        let ast = getParseResults src
        let! unnecessaryParentheses = UnnecessaryParentheses.getUnnecessaryParentheses (fun _ -> src) ast
        Assert.AreEqual(1, Seq.length unnecessaryParentheses, $"Expected one range but got: %A{unnecessaryParentheses}.")
    }

let nestedUnneededParens =
    [
        "((printfn \"Hello, world.\"))"
        "((3))"
        "let (~-) x = x in id (-(<@ 3 @>))"
    ]

[<Theory; TestCaseSource(nameof nestedUnneededParens)>]
let ``Results returned for nested, potentially mutually-exclusive, unnecessary parentheses`` src =
    task {
        let ast = getParseResults src
        let! unnecessaryParentheses = UnnecessaryParentheses.getUnnecessaryParentheses (fun _ -> src) ast
        Assert.AreEqual(2, Seq.length unnecessaryParentheses, $"Expected two ranges but got: %A{unnecessaryParentheses}.")
    }
