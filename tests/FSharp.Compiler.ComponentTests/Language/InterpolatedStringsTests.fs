// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Language

open System
open Xunit
open FSharp.Test.Compiler

module InterpolatedStringsTests =

    [<Theory>]
    [<InlineData("$\"%t{fun () -> printed}\"", "printed")>]
    [<InlineData("$@\"%t{fun () -> printed}\"", "printed")>]
    [<InlineData("$\"\"\"%t{fun () -> printed}\"\"\"", "printed")>]
    [<InlineData("$\"%t{fun () -> x}%02d{1}%t{fun () -> y}\"", "x01y")>]
    let ``Issue 20757 - printf thunk holes do not warn about ToString`` (expression: string, expected: string) =
        Fsx ("let printed, x, y = \"printed\", \"x\", \"y\"\nlet s = " + expression + "\nif s <> \"" + expected + "\" then failwith \"Unexpected thunk formatting\"")
        |> withLangVersionPreview
        |> withOptions ["--warnaserror:3884"]
        |> compileExeAndRun
        |> shouldSucceed

    [<Theory>]
    [<InlineData("string")>]
    [<InlineData("obj")>]
    [<InlineData("System.IComparable")>]
    [<InlineData("System.ICloneable")>]
    let ``Issue 20757 - string-rendering thunk holes enforce state and residue types`` (targetType: string) =
        for thunk in ["fun (_: int) -> \"bad\""; "fun () -> 42"] do
            Fsx ("let value: " + targetType + " = $\"\"\"%t{" + thunk + "}\"\"\"")
            |> withLangVersionPreview
            |> typecheck
            |> shouldFail
            |> withErrorCode 1
            |> ignore

    [<Theory>]
    [<InlineData("string")>]
    [<InlineData("obj")>]
    [<InlineData("System.IComparable")>]
    [<InlineData("System.ICloneable")>]
    let ``Issue 20757 - string-rendering thunk holes render before upcasting`` (targetType: string) =
        Fsx ("let value: " + targetType + " = $\"\"\"%t{fun () -> \"ok\"}%02d{1}\"\"\"\nif string value <> \"ok01\" then failwith \"Unexpected thunk formatting\"")
        |> withLangVersionPreview
        |> withOptions ["--warnaserror:3884"]
        |> compileExeAndRun
        |> shouldSucceed

    [<Theory>]
    [<InlineData("PrintfFormat<string, int, string, string>", "fun (state: int) -> string state")>]
    [<InlineData("PrintfFormat<string, unit, int, string>", "fun () -> 42")>]
    let ``Issue 20757 - captured format thunk holes retain custom state and residue types`` (targetType: string, thunk: string) =
        Fsx ("let format: " + targetType + " = $\"\"\"%t{" + thunk + "}\"\"\"")
        |> withLangVersionPreview
        |> withOptions ["--nowarn:3884"]
        |> typecheck
        |> shouldSucceed

    [<Theory>]
    [<InlineData("%A")>]
    [<InlineData("%O")>]
    let ``Issue 20757 - explicit printf function formatting retains diagnostics`` (specifier: string) =
        Fsx ("let s = $\"" + specifier + "{fun () -> 1}\"")
        |> withLangVersionPreview
        |> withOptions ["--warnaserror:3884"]
        |> typecheck
        |> shouldSucceed

    [<Fact>]
    let ``Issue 20757 - printf interpolation matrix across files and assemblies`` () =
        let fixture = System.IO.Path.Combine(__SOURCE_DIRECTORY__, "..", "..", "AheadOfTime", "NativeAOT", "PrintfInterpolation")
        let library =
            FsFromPath(System.IO.Path.Combine(fixture, "Library", "Formatting.fs"))
            |> withName "PrintfInterpolationLibrary"
            |> asLibrary
            |> withLangVersionPreview
        FsFromPath(System.IO.Path.Combine(fixture, "Formatting.fs"))
        |> withAdditionalSourceFiles [SourceFromPath(System.IO.Path.Combine(fixture, "Program.fs"))]
        |> withReferences [library]
        |> withLangVersionPreview
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContains "01:02:03"

    [<Fact>]
    let ``Issue 20757 - non-string printf surfaces remain unchanged`` () =
        Fsx """
let check expected actual = if actual <> expected then failwith "Unexpected formatting"
let format: Printf.StringFormat<string> = $"%02d{1}:%02d{2}"
check "01:02" (sprintf format)
check "01" (sprintf "%02d" 1)
check "[1; 2]" $"%A{[1; 2]}"
let inline structured value = $"%02d{1}|%A{value}|%02d{2}"
check "01|[1; 2]|02" (structured [1; 2])
check "01|Some \"x\"|02" (structured (Some "x"))
let optional = Some "x"
check "[1; 2]|Some \"x\"" $"%A{[1; 2]}|%A{optional}"
let formattable: System.FormattableString = $"{1,3:D2}"
check " 01" (formattable.ToString(System.Globalization.CultureInfo.InvariantCulture))
let asInterface: System.IFormattable = $"{1:D2}"
check "01" (asInterface.ToString(null, System.Globalization.CultureInfo.InvariantCulture))
"""
        |> withLangVersionPreview
        |> compileExeAndRun
        |> shouldSucceed

    [<Theory>]
    [<InlineData("%02d", "true", "int", "bool", 20)>]
    [<InlineData("%b", "1", "bool", "int", 15)>]
    let ``Issue 20757 - fallback hole diagnostics retain user ranges`` (specifier: string, value: string, expected: string, actual: string, endColumn: int) =
        Fsx ("let s = $\"" + specifier + "{" + value + "}\"")
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 1, Line 1, Col (endColumn - value.Length), Line 1, Col endColumn,
            if specifier = "%02d" then
                "The type 'bool' is not compatible with any of the types byte,int16,int32,int64,sbyte,uint16,uint32,uint64,nativeint,unativeint, arising from the use of a printf-style format string"
            else
                "This expression was expected to have type" + Environment.NewLine + "    '" + expected + "'    " + Environment.NewLine +
                "but here has type" + Environment.NewLine + "    '" + actual + "'    ")

    [<Fact>]
    let ``Issue 20752 - multiple fallback formats compile and run`` () =
        Fsx """let flag = true
let s = $"%b{flag} %b{flag}"
if s <> "true true" then failwithf "Unexpected interpolation: %s" s
"""
        |> withLangVersionPreview
        |> compileExeAndRun
        |> shouldSucceed

    [<Theory>]
    [<InlineData("")>]
    [<InlineData("fr-FR")>]
    let ``Issue 20752 - fallback and fast formats preserve exact invariant output`` (culture: string) =
        let fallbacks =
            [ "%b{true}", "true"; "%A{[1; 2]}", "[1; 2]"; "%f{1.25}", "1.250000"
              "%5d{42}", "   42"; "%.2f{1.25}", "1.25" ]
        let fastFormats =
            [ "%s{\"x\"}", "x"; "%c{'x'}", "x"; "%d{42}", "42"; "%i{42}", "42"; "%M{1.25M}", "1.25" ]
        let checks =
            [ for fallback, fallbackOutput in fallbacks do
                  for fast, fastOutput in fastFormats do
                      for parts, expected in
                          [ [fallback; fast], fallbackOutput + " " + fastOutput
                            [fast; fallback], fastOutput + " " + fallbackOutput
                            [fast; fallback; fast], fastOutput + " " + fallbackOutput + " " + fastOutput ] do
                          let expression = "$\"\"\"" + String.concat " " parts + "\"\"\""
                          yield "check (" + expression + ") " + sprintf "%A" expected ]
            |> String.concat "\n"
        Fsx (
            "let originalCulture = System.Threading.Thread.CurrentThread.CurrentCulture\ntry\n"
            + "    System.Threading.Thread.CurrentThread.CurrentCulture <- System.Globalization.CultureInfo "
            + sprintf "%A" culture
            + "\n    let check actual expected = if actual <> expected then failwithf \"Expected %A, got %A\" expected actual\n"
            + String.concat "\n" (checks.Split('\n') |> Array.map (fun line -> "    " + line))
            + "\nfinally\n    System.Threading.Thread.CurrentThread.CurrentCulture <- originalCulture\n"
        )
        |> withLangVersionPreview
        |> compileExeAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Issue 20752 - hole boundaries and string syntaxes preserve exact output`` () =
        let cases =
            [ "$\"\"", ""; "$\"%%\"", "%"; "$\"%b{true}\"", "true"
              "$\"%b{true}%b{false}\"", "truefalse"; "$\"%b{true}%b{false}%b{true}\"", "truefalsetrue"
              "$\"%d{42}%i{42}%M{1.25M}\"", "42421.25"
              "$@\"%b{true} %b{false}\"", "true false"; "@$\"%b{true} %b{false}\"", "true false"
              "$\"%b{true}\\n%b{false}\"", "true\nfalse"; "$\"%b{true}\n%b{false}\"", "true\nfalse"
              "$\"\"\"%b{true} %b{false}\"\"\"", "true false"
              "$$\"\"\"%%b{{true}} %%b{{false}}\"\"\"", "true false"
              "$\"{{%%}} %b{true} %b{false}\"", "{%} true false"
              "$\"\"\"{$\"%b{true} %b{false}\"} %b{true}\"\"\"", "true false true"
              "$\"\"\"{sprintf \"%b %d\" true 42} %b{false}\"\"\"", "true 42 false" ]
        let checks =
            cases
            |> List.map (fun (expression, expected) -> "check (" + expression + ") " + sprintf "%A" expected)
            |> String.concat "\n"
        Fsx (
            "let check actual expected = if actual <> expected then failwithf \"Expected %A, got %A\" expected actual\n"
            + checks
        )
        |> withLangVersionPreview
        |> compileExeAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Issue 20752 - contexts generic holes and other formatting paths are preserved`` () =
        Fsx """
open System
open System.Globalization
let check actual expected = if actual <> expected then failwithf "Expected %A, got %A" expected actual
let flag = true
let s: string = $"%b{flag} %b{flag}"
check s "true true"
let messages = ResizeArray<string>()
let log (msg: string) = messages.Add msg
log $"%b{flag}"
log $"%b{flag} %b{flag}"
check (List.ofSeq messages) ["true"; "true true"]
let logInferred msg = printfn "%s" msg
logInferred $"%b{flag} %b{flag}"
let show x = $"%A{x} %b{true}"
check (show 42) "42 true"
check (show [1; 2]) "[1; 2] true"
check (sprintf "%b %d" true 42) "true 42"
check $"{1.25,6:F2}" "  1.25"
check (sprintf $"%b{true} %d{42}") "true 42"
let format: PrintfFormat<string, unit, string, string> = $"%b{true} %d{42}"
check (sprintf format) "true 42"
let formattable: FormattableString = $"{1.25,6:F2} {42}"
check formattable.ArgumentCount 2
check formattable.Format "{0,6:F2} {1}"
check (formattable.ToString CultureInfo.InvariantCulture) "  1.25 42"
let interfaceFormat: IFormattable = $"{1.25,6:F2} {42}"
check (interfaceFormat.ToString(null, CultureInfo.InvariantCulture)) "  1.25 42"
"""
        |> withLangVersionPreview
        |> compileExeAndRun
        |> shouldSucceed

    [<Theory>]
    [<InlineData("$\"\"\"%b{hole 1 true}%s{hole 2 \"x\"}%5d{hole 3 42}\"\"\"", "truex   42")>]
    [<InlineData("$\"\"\"{$\"%b{hole 1 true} %b{hole 2 false}\"} {sprintf \"%d\" (hole 3 42)}\"\"\"", "true false 42")>]
    let ``Issue 20752 - adjacent nested and sprintf holes evaluate once left to right`` (expression: string, expected: string) =
        let source = """
let trace = ResizeArray<int>()
let counters = Array.zeroCreate<int> 3
let hole n value =
    trace.Add n
    counters[n - 1] <- counters[n - 1] + 1
    value
let check actual expected = if actual <> expected then failwithf "Expected %A, got %A" expected actual
        """
        Fsx (
            source + "\ncheck (" + expression + ") " + sprintf "%A" expected
            + "\ncheck (List.ofSeq trace) [1; 2; 3]\ncheck counters [|1; 1; 1|]"
        )
        |> withLangVersionPreview
        |> compileExeAndRun
        |> shouldSucceed

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
    let ``Interpolation holes are rendered with invariant culture`` () =
        Fsx """
System.Threading.Thread.CurrentThread.CurrentCulture <- System.Globalization.CultureInfo "de-DE"
printf "%s" $"{1.5}"
        """
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContains "1.5"

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

    // Issue 16696: '=' immediately followed (no space) by an interpolated-string opener was
    // greedily lexed as the invalid operator '=$' instead of '=' + an interpolated string.
    // The hole {n} proves an interpolated string (not a plain one) is what gets lexed.
    [<Fact>]
    let ``Issue 16696 - '=' adjacent to an interpolated string binds it`` () =
        Fsx """
let n = 42
let x =$"{n}"
if x <> "42" then failwith "expected 42"
        """
        |> compileExeAndRun
        |> shouldSucceed

    // (The triple-quote form '=$"""..."""' is covered by the SyntaxTree baseline
    // SynExprInterpolatedStringAdjacentEqualsTripleQuote.fs; '=$"' is a prefix of '=$"""', so the
    // same lexer rule handles it after the rewind.)

    // The reported cases from the issue: named-argument and record-field initialization.
    [<Fact>]
    let ``Issue 16696 - '=' adjacent to an interpolated string in named-argument and record contexts`` () =
        Fsx """
type C() = member val Name = "" with get, set
type R = { Name: string }
let n = 42
let c = C(Name=$"{n}")
let r = { Name=$"{n}" }
if c.Name <> "42" || r.Name <> "42" then failwith "expected 42"
        """
        |> compileExeAndRun
        |> shouldSucceed

    // The verbatim ($@" / @$") forms, also adjacent to '='.
    [<Fact>]
    let ``Issue 16696 - '=' adjacent to a verbatim interpolated string binds it`` () =
        Fsx """
let n = 42
let a =$@"{n}"
let b =@$"{n}"
if a <> "42" || b <> "42" then failwith "expected 42"
        """
        |> compileExeAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Issue 16696 - '=' adjacent to a verbatim interpolated string in named-argument and record contexts`` () =
        Fsx """
type C() = member val Name = "" with get, set
type R = { Name: string }
let n = 42
let c = C(Name=$@"{n}")
let r = { Name=@$"{n}" }
if c.Name <> "42" || r.Name <> "42" then failwith "expected 42"
        """
        |> compileExeAndRun
        |> shouldSucceed

    // The extended multi-dollar ($$) form, also adjacent to '='. Note that a $$ string uses double
    // braces for holes ({{n}}), so {n} would be literal text. Uses an escaped string literal because
    // the source contains """, which cannot be embedded in an F# """...""" string.
    [<Fact>]
    let ``Issue 16696 - '=' adjacent to an extended multi-dollar interpolated string binds it`` () =
        Fsx "let n = 42\nlet x =$$\"\"\"{{n}}\"\"\"\nif x <> \"42\" then failwith \"expected 42\""
        |> compileExeAndRun
        |> shouldSucceed

    // Operator lexing is unchanged: a '$' anywhere in an operator is still reserved (FS0035).
    // The only thing the fix changes is '=' directly before an interpolated-string opener;
    // everything below still lexes as an operator exactly as before.
    [<Theory>]
    [<InlineData("let x =$abc")>]          // '=$' not before a quote
    [<InlineData("let (=$) a b = a")>]     // defining (=$)
    [<InlineData("let f a b = a =$ b")>]   // '=$' used as infix
    [<InlineData("let (<$>) f x = f x")>]  // '$' inside an operator
    [<InlineData("let (<=$=>) a b = a")>]  // '$' in the middle of an operator
    let ``Issue 16696 - operators containing '$' are still rejected (operator lexing unchanged)`` (code: string) =
        Fsx code
        |> compile
        |> shouldFail
        |> withDiagnosticMessageMatches "is not permitted as a character in operator names"
