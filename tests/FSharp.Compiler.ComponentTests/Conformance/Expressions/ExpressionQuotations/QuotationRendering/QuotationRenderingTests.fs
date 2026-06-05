// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Snapshot tests for the F# rendering of quoted expressions (sprintf "%A" <@ ... @>).
//
// Each test compiles a tiny program that prints the rendered quotation, executes it, and diffs
// stdout against the matching .bsl file in this folder. Regenerate baselines with TEST_UPDATE_BSL=1.
//
// Purpose: detect accidental leakage of pattern-match-compilation lowerings (jump tables, null
// + Length rewrites, raw IL asm, ...) into the public quotation AST. The quotation translator
// runs *after* PatternMatchCompilation, so any rewrite done there shows up here as a (potentially
// surprising) shape change.
//
// Related: https://github.com/dotnet/fsharp/issues/19873

namespace Conformance.Expressions.ExpressionQuotations

open System.IO
open Xunit
open FSharp.Test.Compiler

module QuotationRendering =

    let private baselineDir = __SOURCE_DIRECTORY__

    /// Wrap a quotation expression in the smallest compilable program that prints its rendering.
    let private printerProgram quoteExpr =
        $"module Test\nprintfn \"%%A\" {quoteExpr}\n"

    /// Compile, run, diff stdout against <name>.bsl. Use TEST_UPDATE_BSL=1 to regenerate.
    let private quoteShouldRender name quoteExpr =
        FSharp (printerProgram quoteExpr)
        |> asExe
        |> ignoreWarnings
        |> compileExeAndRun
        |> shouldSucceed
        |> verifyOutputAgainstBaseline (Path.Combine(baselineDir, name + ".bsl"))
        |> ignore

    // ---------------------------------------------------------------------------
    // Regression: shapes the empty-string optimization used to leak (#19873).
    // After moving the rewrite out of BuildSwitch the baselines render `op_Equality`
    // rather than the null + Length lowering. A leak resurfacing here will fail
    // the test with a diff.
    // ---------------------------------------------------------------------------
    module Regression =

        [<Fact>]
        let ``match x with empty string`` () =
            quoteShouldRender "EmptyString"
                """<@ fun (x: string) -> match x with "" -> 1 | _ -> 0 @>"""

        [<Fact>]
        let ``match x with null or empty string`` () =
            quoteShouldRender "NullOrEmpty"
                """<@ fun (x: string) -> match x with null | "" -> 1 | _ -> 0 @>"""

    // ---------------------------------------------------------------------------
    // Reference: PatternMatchCompilation lowerings that have *never* leaked.
    // These baselines prove that compactification (consecutive int → TDSwitch → jump
    // table), char tables, and per-type `mkILAsmCeq` paths all survive as clean
    // `op_Equality` chains in quotations. If a future BuildSwitch optimization
    // changes this, the diff is the smoking gun.
    // ---------------------------------------------------------------------------
    module NoLeakReference =

        [<Fact>]
        let ``match x with non-empty string`` () =
            quoteShouldRender "NonEmptyString"
                """<@ fun (x: string) -> match x with "a" -> 1 | "b" -> 2 | _ -> 0 @>"""

        /// Two-or-more consecutive ints trigger BuildSwitch's compactify branch.
        /// We want to verify the resulting TDSwitch still renders as op_Equality calls
        /// (not as an opaque jump-table marker) inside quotations.
        [<Fact>]
        let ``match i with consecutive ints`` () =
            quoteShouldRender "ConsecutiveInts"
                """<@ fun (i: int) -> match i with 1 -> "a" | 2 -> "b" | 3 -> "c" | _ -> "z" @>"""

        [<Fact>]
        let ``match c with chars`` () =
            quoteShouldRender "Chars"
                """<@ fun (c: char) -> match c with 'a' -> 1 | 'b' -> 2 | _ -> 0 @>"""

        /// Int64/Float/Decimal each take a different BuildSwitch arm but all rely on
        /// QuotationTranslator's `[AI_ceq]` → `op_Equality` recovery to render cleanly.
        [<Fact>]
        let ``match i with int64`` () =
            quoteShouldRender "Int64"
                """<@ fun (i: int64) -> match i with 1L -> "a" | _ -> "b" @>"""

        [<Fact>]
        let ``match f with float`` () =
            quoteShouldRender "Float"
                """<@ fun (f: float) -> match f with 1.0 -> "a" | _ -> "b" @>"""

        [<Fact>]
        let ``match d with decimal`` () =
            quoteShouldRender "Decimal"
                """<@ fun (d: decimal) -> match d with 1m -> "a" | _ -> "b" @>"""

    // ---------------------------------------------------------------------------
    // Side effect of the fix: pezipink's second example in #19873 — `if s = ""`
    // was already rendering cleanly in quotations (no BuildSwitch path involved)
    // and continues to do so even after the Optimizer-level rewrite, because the
    // Optimizer skips Expr.Quote bodies.
    // ---------------------------------------------------------------------------
    module SideEffect =

        [<Fact>]
        let ``if s = empty string`` () =
            quoteShouldRender "IfEqualEmpty"
                """<@ fun (s: string) -> if s = "" then 1 else 0 @>"""

    // ---------------------------------------------------------------------------
    // Pre-existing leakage: array-length patterns lower to raw `I_ldlen` IL via
    // `mkLdlen` *before* the quotation translator runs, which then rejects them
    // with FS0452. This was broken before #19189 and is left untouched. If a
    // future change moves this lowering out of BuildSwitch, flip to `shouldSucceed`
    // and add an `ArrayLength.bsl` baseline.
    // ---------------------------------------------------------------------------
    module PreExistingError =

        [<Fact>]
        let ``match a with array length`` () =
            FSharp """module Test
let q = <@ fun (a: int[]) -> match a with [| _ |] -> 1 | _ -> 0 @>
"""
            |> asLibrary
            |> typecheck
            |> shouldFail
            |> withErrorCode 452
            |> withDiagnosticMessageMatches "Quotations cannot contain inline assembly code or pattern matching on arrays"
            |> ignore
