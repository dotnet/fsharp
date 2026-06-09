// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Snapshot tests pinning `sprintf "%A"` of quoted expressions that pattern-match compilation can rewrite.
// Regression coverage for https://github.com/dotnet/fsharp/issues/19873.
//
// Design notes:
//   - Quotation literals are written directly as F# in the test method body. They are desugared by
//     the F# compiler that builds the test project — the bootstrapped (proto) fsc, NOT the freshly
//     built FSharp.Compiler.Service.dll that the test ProjectReferences. CI typically rebuilds the
//     proto per run; inner-loop dev after touching `src/Compiler/Checking/PatternMatchCompilation.fs`
//     (or other desugar paths) requires `./build.sh --bootstrap -test`, otherwise these tests
//     exercise the cached proto compiler.
//   - Conversely, this is a stronger regression gate than a runtime assertion: if the #19873 fix is
//     ever reverted such that `match x with ""` lowers to inline IL, the literals in this file fail
//     to compile with FS0452 (see QuotationUnsupportedConstructsTests.fs).
//   - All lambda parameters are named `x` so the binder name carries no signal and parameter renames
//     never cause baseline diffs.
//   - Baselines start with `// <name>` so a `.bsl` opened in isolation is self-describing.

namespace Conformance.Expressions.ExpressionQuotations

open System.IO
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open Microsoft.FSharp.Quotations.ExprShape
open Xunit
open FSharp.Test.Compiler

module QuotationRendering =

    let private baselineDir = __SOURCE_DIRECTORY__

    let private renderToBaseline (name: string) (q: Expr) =
        let body = sprintf "%A" q |> normalizeNewlines
        let actual = sprintf "// %s\n%s\n" name body
        checkBaseline actual (Path.Combine(baselineDir, name + ".bsl"))

    /// Belt-and-suspenders sniff for the two specific lowering shapes #19873 was about:
    /// `String.Length` PropertyGet and `String.Equals` Call. The `.bsl` baseline below is the
    /// authoritative guard (it catches any %A shape drift); this walker only adds a clearer
    /// failure message for the two known shapes, so a future leaked lowering through a different
    /// API (String.IsNullOrEmpty, indexer, etc.) is still caught by the baseline diff, not here.
    let rec private containsEmptyStringLowering (e: Expr) =
        let isStringMember (t: System.Type) =
            not (isNull t) && t.FullName = "System.String"
        match e with
        | PropertyGet (_, pi, _) when isStringMember pi.DeclaringType && pi.Name = "Length" -> true
        | Call (_, mi, _) when isStringMember mi.DeclaringType && mi.Name = "Equals" -> true
        | ShapeCombination (_, args) -> args |> List.exists containsEmptyStringLowering
        | ShapeLambda (_, body) -> containsEmptyStringLowering body
        | ShapeVar _ -> false

    let private assertNoEmptyStringLowering (q: Expr) =
        Assert.False(containsEmptyStringLowering q,
            sprintf "Quotation leaked the empty-string lowering (String.Length or String.Equals):\n%A" q)

    // Shared quotation literals: the convergence assertion below proves the SAME exprs that feed
    // the EmptyString and IfEqualEmpty baselines desugar identically. Without this hoist, an editor
    // could change one literal and leave the convergence test green against an out-of-date pair.
    let private qMatchEmpty = <@ fun (x: string) -> match x with "" -> 1 | _ -> 0 @>
    let private qIfEqualEmpty = <@ fun (x: string) -> if x = "" then 1 else 0 @>

    /// Authoritative list of case names. The orphan-guard test below enforces that
    /// `*.bsl` on disk equals this set exactly. Skipped during baseline refresh
    /// (`TEST_UPDATE_BSL=1`) to avoid racing with in-progress baseline writes.
    let private expectedBaselines =
        Set.ofList [
            "EmptyString"
            "NullOrEmpty"
            "NonEmptyString"
            "ConsecutiveInts"
            "Chars"
            "Int64"
            "Decimal"
            "IfEqualEmpty"
        ]

    [<Fact>]
    let ``Baseline directory: no orphans, no missing`` () =
        if System.Environment.GetEnvironmentVariable("TEST_UPDATE_BSL") <> null then () else
        let onDisk =
            Directory.GetFiles(baselineDir, "*.bsl")
            |> Array.map Path.GetFileNameWithoutExtension
            |> Set.ofArray
        let orphans = Set.difference onDisk expectedBaselines
        let missing = Set.difference expectedBaselines onDisk
        if not orphans.IsEmpty || not missing.IsEmpty then
            let parts = [
                if not orphans.IsEmpty then
                    yield sprintf "Orphan .bsl files (delete the file, or add a matching test + entry in expectedBaselines): %A" orphans
                if not missing.IsEmpty then
                    yield sprintf "Missing .bsl files (add the test that calls renderToBaseline and run with TEST_UPDATE_BSL=1): %A" missing
            ]
            Assert.Fail(String.concat "\n" parts)

    // Regression for #19873: empty-string optimization must not leak `if s <> null then s.Length = 0 else false`
    // into quotation ASTs. Each Fact pairs the baseline pin with a structural assertion that doesn't
    // depend on `%A` formatting.

    [<Fact>]
    let ``match x with empty string renders as op_Equality (#19873)`` () =
        assertNoEmptyStringLowering qMatchEmpty
        renderToBaseline "EmptyString" qMatchEmpty

    [<Fact>]
    let ``match x with null or empty string renders as op_Equality (#19873)`` () =
        let q = <@ fun (x: string) -> match x with null | "" -> 1 | _ -> 0 @>
        assertNoEmptyStringLowering q
        renderToBaseline "NullOrEmpty" q

    // Reference baselines: pattern-match-compilation lowerings that have never leaked into
    // quotations. Pins the BuildSwitch arms so a future optimization that starts rewriting
    // them in-place (the bug class behind #19873) shows up as a diff.
    //   - NonEmptyString, ConsecutiveInts, Chars : op_Equality + compactify/jump-table path
    //   - Int64                                  : the `mkILAsmCeq` arm + `[AI_ceq]` -> op_Equality recovery
    //   - Decimal                                : op_Equality with a MakeDecimal literal

    [<Fact>]
    let NonEmptyString () =
        renderToBaseline "NonEmptyString"
            <@ fun (x: string) -> match x with "a" -> 1 | "b" -> 2 | _ -> 0 @>

    [<Fact>]
    let ConsecutiveInts () =
        renderToBaseline "ConsecutiveInts"
            <@ fun (x: int) -> match x with 1 -> "a" | 2 -> "b" | 3 -> "c" | _ -> "z" @>

    [<Fact>]
    let Chars () =
        renderToBaseline "Chars"
            <@ fun (x: char) -> match x with 'a' -> 1 | 'b' -> 2 | _ -> 0 @>

    [<Fact>]
    let Int64 () =
        renderToBaseline "Int64"
            <@ fun (x: int64) -> match x with 1L -> "a" | _ -> "b" @>

    [<Fact>]
    let Decimal () =
        renderToBaseline "Decimal"
            <@ fun (x: decimal) -> match x with 1m -> "a" | _ -> "b" @>

    // Side effect of the move: `if x = ""` was already clean in quotations (no BuildSwitch path)
    // and stays clean after the rewrite moved to the Optimizer (which skips Expr.Quote bodies).
    [<Fact>]
    let ``if x = empty string renders cleanly (second repro from #19873)`` () =
        assertNoEmptyStringLowering qIfEqualEmpty
        renderToBaseline "IfEqualEmpty" qIfEqualEmpty

    // Convergence: surface forms `match x with "" -> _` and `if x = "" then _` desugar to the
    // SAME quoted Expr after binder standardization. Asserts on the SHARED literals so the
    // convergence proof cannot drift relative to the baselined exprs above.
    [<Fact>]
    let ``match-empty and if-equal-empty quotations converge to the same Expr`` () =
        Assert.Equal(sprintf "%A" qMatchEmpty, sprintf "%A" qIfEqualEmpty)
