// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Snapshot tests pinning the shape of quoted expressions that pattern-match compilation can rewrite.
// Regression coverage for https://github.com/dotnet/fsharp/issues/19873.

namespace Conformance.Expressions.ExpressionQuotations

open System.IO
open Xunit
open FSharp.Test.Compiler

module QuotationRendering =

    let private baselineDir = __SOURCE_DIRECTORY__

    let private printerProgram quoteExpr =
        $"module Test\nprintfn \"%%A\" {quoteExpr}\n"

    let private quoteShouldRender name quoteExpr =
        FSharp (printerProgram quoteExpr)
        |> asExe
        |> ignoreWarnings
        |> compileExeAndRun
        |> shouldSucceed
        |> verifyOutputAgainstBaseline (Path.Combine(baselineDir, name + ".bsl"))
        |> ignore

    [<Fact>]
    let ``Regression #19873: match x with empty string renders as op_Equality`` () =
        quoteShouldRender "EmptyString"
            """<@ fun (x: string) -> match x with "" -> 1 | _ -> 0 @>"""

    [<Fact>]
    let ``Regression #19873: match x with null or empty string renders as op_Equality`` () =
        quoteShouldRender "NullOrEmpty"
            """<@ fun (x: string) -> match x with null | "" -> 1 | _ -> 0 @>"""

    // Reference baselines: pattern-match-compilation lowerings that have never leaked into
    // quotations. Pins the BuildSwitch arms so a future optimization that starts rewriting
    // them in-place (the bug class behind #19873) shows up as a diff.
    //   - NonEmptyString, ConsecutiveInts, Chars : op_Equality and the compactify/jump-table path
    //   - Int64                                  : the `mkILAsmCeq` arm + `[AI_ceq]` -> op_Equality recovery
    //   - Decimal                                : op_Equality with a MakeDecimal literal
    [<Theory>]
    [<InlineData("NonEmptyString", """<@ fun (x: string) -> match x with "a" -> 1 | "b" -> 2 | _ -> 0 @>""")>]
    [<InlineData("ConsecutiveInts", """<@ fun (i: int) -> match i with 1 -> "a" | 2 -> "b" | 3 -> "c" | _ -> "z" @>""")>]
    [<InlineData("Chars", """<@ fun (c: char) -> match c with 'a' -> 1 | 'b' -> 2 | _ -> 0 @>""")>]
    [<InlineData("Int64", """<@ fun (i: int64) -> match i with 1L -> "a" | _ -> "b" @>""")>]
    [<InlineData("Decimal", """<@ fun (d: decimal) -> match d with 1m -> "a" | _ -> "b" @>""")>]
    let ``no-leak reference: BuildSwitch arms render cleanly in quotations`` (name: string) (quoteExpr: string) =
        quoteShouldRender name quoteExpr

    [<Fact>]
    let ``Second repro from #19873: if s = empty string also renders cleanly`` () =
        quoteShouldRender "IfEqualEmpty"
            """<@ fun (s: string) -> if s = "" then 1 else 0 @>"""

    // Pre-existing leakage: array-length patterns lower to `I_ldlen` IL via `mkLdlen` *before* the
    // quotation translator runs, which rejects them with FS0452. Out of scope for #19873; flip to
    // shouldSucceed + add an ArrayLength.bsl when that lowering moves out of BuildSwitch.
    [<Fact>]
    let ``Pre-existing: match a with array length still errors with FS0452`` () =
        FSharp """module Test
let q = <@ fun (a: int[]) -> match a with [| _ |] -> 1 | _ -> 0 @>
"""
        |> asLibrary
        |> typecheck
        |> shouldFail
        |> withErrorCode 452
        |> withDiagnosticMessageMatches "Quotations cannot contain inline assembly code or pattern matching on arrays"
        |> ignore
