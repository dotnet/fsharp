// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Language

open System
open Xunit
open FSharp.Test.Compiler

module InterpolatedStringsTests =

    [<Fact>]
    let ``Regression: Empty Interpolated String properly typechecks with explicit type on binding`` () =
        Fsx """ let a:byte = $"string" """
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Error 1, Line 1, Col 15, Line 1, Col 24, "This expression was expected to have type" + Environment.NewLine + "    'byte'    " + Environment.NewLine + "but here has type" + Environment.NewLine + "    'string'    ")

    [<Fact>]
    let ``Interpolated String with hole properly typechecks with explicit type on binding`` () =
        Fsx """ let a:byte = $"strin{'g'}" """
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Error 1, Line 1, Col 15, Line 1, Col 28, "This expression was expected to have type" + Environment.NewLine + "    'byte'    " + Environment.NewLine + "but here has type" + Environment.NewLine + "    'string'    ")

    [<Fact>]
    let ``Interpolated String without holes properly typeckecks with explicit type on binding`` () = 
        Fsx """
let a: obj = $"string"
let b: System.IComparable = $"string"
let c: System.IFormattable = $"string"
        """
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Interpolated string literal typed as FormattableString handles double braces correctly`` () =
        Fsx """
let a = $"{{hello}} world" : System.FormattableString
printf $"{a.Format}"
        """
        |> withLangVersion80
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContains "{{hello}} world"

    [<Fact>]
    let ``Interpolated string with 2 leading dollar characters uses double braces for delimiters`` () =
        // let s = $$"""{{42 + 0}} = {41 + 1}"""
        // printfn "%s" s
        Fsx "let s = $$\"\"\"{{42 + 0}} = {41 + 1}\"\"\"\n\
printfn \"%s\" s"
        |> withLangVersion80
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContains "42 = {41 + 1}"

    [<Fact>]
    let ``Too many consecutive opening braces in interpolated string result in an error`` () =
        // $$"""{{{{42 - 0}}"""
        Fsx "$$\"\"\"{{{{42 - 0}}\"\"\""
        |> withLangVersion80
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Error 1248, Line 1, Col 1, Line 1, Col 10, "The interpolated triple quoted string literal does not start with enough '$' characters to allow this many consecutive opening braces as content.")

    [<Fact>]
    let ``Too many consecutive closing braces in interpolated string result in an error`` () =
        // $$"""{{42 - 0}}}}"""
        Fsx "$$\"\"\"{{42 - 0}}}}\"\"\""
        |> withLangVersion80
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Error 1249, Line 1, Col 14, Line 1, Col 21, "The interpolated string contains unmatched closing braces.")

    [<Fact>]
    let ``Percent sign characters in interpolated strings`` () =
        Assert.Equal("%", $"%%")
        Assert.Equal("42%", $"{42}%%")
        Assert.Equal("% 42", $"%%%3d{42}")

    [<Fact>]
    let ``Double percent sign characters in triple quote interpolated strings`` () =
        Fsx "printfn \"%s\" $$$\"\"\"%%\"\"\""
        |> withLangVersion80
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContains "%%"

    [<Fact>]
    let ``Percent sign after interpolation hole in triple quote strings`` () =
        Fsx "printfn \"%s\" $$\"\"\"{{42}}%\"\"\""
        |> withLangVersion80
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContains "42%"

    [<Fact>]
    let ``Percent sign before format specifier in triple quote interpolated strings`` () =
        Fsx "printfn \"%s\" $$\"\"\"%%%3d{{42}}\"\"\""
        |> withLangVersion80
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContains "% 42"

    [<Fact>]
    let ``Percent signs separated by format specifier's flags`` () =
        Fsx """
let s = $"...%-%...{0}"
        """
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Warning 3376, Line 2, Col 9, Line 2, Col 24, "Bad format specifier: '%'")

    [<Fact>]
    let ``Interpolated expression can be offside`` () =
        Fsx """
let a() =
    let b() =
        $"
{1}"
    b()

type Foo () =
    member _.Bar () =
        let x =
            $"
{2}"
        x
        """
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Percent signs and format specifiers with string expression`` () =
        Fsx """
let x = "abc"
let s = $"%%%s{x}%%"
printfn "%s" s
        """
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContains "%abc%"

    [<Theory>]
    // Test different number of interpolated string parts
    [<InlineData("$\"\"\"abc{\"d\"}e\"\"\"")>]
    [<InlineData("$\"\"\"abc{\"d\"}{\"e\"}\"\"\"")>]
    [<InlineData("$\"\"\"a{\"b\"}c{\"d\"}e\"\"\"")>]
    let ``Interpolated expressions are strings`` (strToPrint: string) =
        Fsx $"""
let x = {strToPrint}
printfn "%%s" x
        """
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContains "abcde"

    let ``Multiline interpolated expression is a string`` () =
        let strToPrint = String.Join(Environment.NewLine, "$\"\"\"a", "b", "c", "{\"d\"}", "e\"\"\"")
        Fsx $"""
let x = {strToPrint}
printfn "%%s" x
        """
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContains """a
b
c
d
e"""

    [<Theory>]
    [<InlineData("$\"\"\"abc{\"d\"}e\"\"\"", 1)>]
    [<InlineData("$\"\"\"abc{\"d\"}{\"e\"}\"\"\"", 2)>]
    [<InlineData("$\"\"\"a{\"b\"}c{\"d\"}e\"\"\"", 2)>]
    let ``In FormattableString, interpolated expressions are strings`` (formattableStr: string, argCount: int) =
        Fsx $"""
let x = {formattableStr} : System.FormattableString
assert(x.ArgumentCount = {argCount})
printfn "%%s" (System.Globalization.CultureInfo "en-US" |> x.ToString)
        """
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContains "abcde"

    [<Fact>]
    let ``Warn when lambda is used as interpolated string argument`` () =
        Fsx """
let f = fun x -> x + 1
let s = $"{f}"
        """
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Warning 3884, Line 3, Col 12, Line 3, Col 13, "This expression is a function value. When used in an interpolated string it will be formatted using its 'ToString' method, which is likely not the intended behavior. Consider applying the function to its arguments.")

    [<Fact>]
    let ``Warn when underscore dot shorthand is used as interpolated string argument`` () =
        Fsx """
type R = { Name: string }
let r = { Name = "hello" }
let s = $"{_.Name}" : string
        """
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Warning 3884, Line 4, Col 12, Line 4, Col 18, "This expression is a function value. When used in an interpolated string it will be formatted using its 'ToString' method, which is likely not the intended behavior. Consider applying the function to its arguments.")

    [<Fact>]
    let ``Warn when partially applied function is used as interpolated string argument`` () =
        Fsx """
let add x y = x + y
let s = $"{add 1}"
        """
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Warning 3884, Line 3, Col 12, Line 3, Col 17, "This expression is a function value. When used in an interpolated string it will be formatted using its 'ToString' method, which is likely not the intended behavior. Consider applying the function to its arguments.")

    [<Fact>]
    let ``Warn when named function is used as interpolated string argument`` () =
        Fsx """
let myFunc (x: int) = string x
let s = $"result: {myFunc}"
        """
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Warning 3884, Line 3, Col 20, Line 3, Col 26, "This expression is a function value. When used in an interpolated string it will be formatted using its 'ToString' method, which is likely not the intended behavior. Consider applying the function to its arguments.")

    [<Fact>]
    let ``No warn when non-function value is used as interpolated string argument`` () =
        Fsx """
let x = 42
let s1 = $"{x}"
let s2 = $"{System.DateTime.Now}"
        """
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``No warn when function is applied in interpolated string argument`` () =
        Fsx """
let f x = x + 1
let s = $"{f 42}"
        """
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``No warn for function value in interpolated string with older language version`` () =
        Fsx """
let f = fun x -> x + 1
let s = $"{f}"
        """
        |> withLangVersion10
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Warn when multiple function values are used in interpolated string`` () =
        Fsx """
let f x = x + 1
let g x = x * 2
let s = $"{f} and {g}"
        """
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 3884, Line 4, Col 12, Line 4, Col 13, "This expression is a function value. When used in an interpolated string it will be formatted using its 'ToString' method, which is likely not the intended behavior. Consider applying the function to its arguments.")
            (Warning 3884, Line 4, Col 20, Line 4, Col 21, "This expression is a function value. When used in an interpolated string it will be formatted using its 'ToString' method, which is likely not the intended behavior. Consider applying the function to its arguments.")
        ]

    [<Fact>]
    let ``Warn for function value in FormattableString interpolated string`` () =
        Fsx """
let f x = x + 1
let s : System.FormattableString = $"{f}"
        """
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Warning 3884, Line 3, Col 39, Line 3, Col 40, "This expression is a function value. When used in an interpolated string it will be formatted using its 'ToString' method, which is likely not the intended behavior. Consider applying the function to its arguments.")

    [<Fact>]
    let ``Warn for function value with format specifier in interpolated string`` () =
        Fsx """
let f x = x + 1
let s = $"{f:N2}"
        """
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 3884, Line 3, Col 12, Line 3, Col 13, "This expression is a function value. When used in an interpolated string it will be formatted using its 'ToString' method, which is likely not the intended behavior. Consider applying the function to its arguments.")
        ]

    [<Fact>]
    let ``Warn can be suppressed with nowarn`` () =
        Fsx """
#nowarn "3884"
let f x = x + 1
let s = $"{f}"
        """
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Warn when System.Action delegate is used as interpolated string argument`` () =
        Fsx """
let a = System.Action(fun () -> ())
let s = $"{a}"
        """
        |> withLangVersionPreview
        |> compile
        |> withDiagnosticMessageMatches "This expression is a function value"

    [<Fact>]
    let ``Warn when System.Func delegate is used as interpolated string argument`` () =
        Fsx """
let f = System.Func<int, string>(fun x -> string x)
let s = $"{f}"
        """
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Warning 3884, Line 3, Col 12, Line 3, Col 13, "This expression is a function value. When used in an interpolated string it will be formatted using its 'ToString' method, which is likely not the intended behavior. Consider applying the function to its arguments.")

    [<Fact>]
    let ``No warn when delegate is invoked in interpolated string argument`` () =
        Fsx """
let f = System.Func<int, string>(fun x -> string x)
let s = $"{f.Invoke(42)}"
        """
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed

    // See https://github.com/dotnet/fsharp/issues/19367.
    [<CulturedFact([|"th"|])>]
    let ``Hole without specifier parsed correctly when culture set to Thai`` () =
        Fsx
            """
            let s = $"{3}"
            if s <> "3" then
                failwith $"Expected \"3\" but got \"%s{s}\"."
            """
        |> compileExeAndRun
        |> shouldSucceed

    // See https://github.com/dotnet/fsharp/issues/19367.
    [<CulturedFact([|"th"|])>]
    let ``Explicit %P does not cause exception when culture set to Thai`` () =
        Fsx
            """
            let s = $"%P({3})"
            """
        |> compileExeAndRun
        |> shouldSucceed
