// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Language

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
        |> withLangVersionPreview
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContains "{{hello}} world"

    [<Fact>]
    let ``Percent sign characters in interpolated strings`` () =
        Assert.Equal("%", $"%%")
        Assert.Equal("42%", $"{42}%%")
        Assert.Equal("% 42", $"%%%3d{42}")
