// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Language

open Xunit
open FSharp.Test.Compiler

/// Tests for https://github.com/dotnet/fsharp/issues/19550
///
/// SYMPTOM:
///   When a type-constraint mismatch occurs on a *empty-bodied* computation
///   expression that participates in a pipeline / function-argument /
///   type-annotated context (e.g. `"" |> foo {} |> ...`, `take (foo {})`,
///   `let x : int = foo {}`), the diagnostic location is reported as
///   `unknown(1,1)` (i.e. `range0`) instead of the actual source location
///   of the CE expression.
///
/// ROOT CAUSE (informational):
///   The empty CE body is desugared through the `EmptyFieldListAsUnit`
///   active pattern in `CheckExpressions.fs` to
///   `SynExpr.Const(SynConst.Unit, range0)`. That `range0` flows through
///   `SynExpr.ImplicitZero` in `CheckComputationExpressions.fs` and ends
///   up as the range of the synthesized `builder.Zero()` call - which is
///   the typed expression the surrounding context tries to unify against.
///
/// SPRINT 01 (this file):
///   Add the failing test set. On current `main` the bug-repro tests
///   (1, 3, 4, 6, 8, 10, 11, 12) FAIL because the actual reported range
///   is `(1,0)-(1,0)` (the `unknown(1,1)` symptom) rather than the
///   expected CE expression range. Sprint 02 will land the fix and these
///   tests will pass without any further edits.
///
///   Tests 2, 5a, 5b, 5c are *regression guards*: they cover scenarios
///   adjacent to the bug (single-pipe-into-printf, non-empty Yield/Return/
///   Bind bodies) where the current diagnostic already has a real,
///   non-zero source range. We assert only that the range is not
///   `(1,1)-(1,1)`. These guards must continue to hold after Sprint 02
///   so the fix does not regress these neighbours.
///
///   Test 7 is a positive case (empty CE body that type-checks must
///   continue to compile cleanly). Test 9 is a positive-negative case
///   (a *non*-CE pipeline mismatch already has a good range and must
///   keep it).
module CE_PipelineRange19550 =

    // ------------------------------------------------------------------ //
    // Helpers
    // ------------------------------------------------------------------ //

    /// Returns all errors from the result (success or failure).
    let private errorsOf (result: CompilationResult) : ErrorInfo list =
        let diags =
            match result with
            | CompilationResult.Success r
            | CompilationResult.Failure r -> r.Diagnostics
        diags
        |> List.filter (fun d ->
            match d.Error with ErrorType.Error _ -> true | _ -> false)

    /// Pretty-print all diagnostics on a result.
    let private dump (result: CompilationResult) : string =
        errorsOf result
        |> List.map (fun d ->
            let n =
                match d.Error with
                | ErrorType.Error n
                | ErrorType.Warning n
                | ErrorType.Information n
                | ErrorType.Hidden n -> n
            // Convert raw 0-based columns to 1-based for readability.
            sprintf "  FS%04d (L%d,C%d)-(L%d,C%d): %s"
                n d.Range.StartLine (d.Range.StartColumn + 1)
                d.Range.EndLine (d.Range.EndColumn + 1) d.Message)
        |> String.concat "\n"

    /// Assert the result has at least one diagnostic with the given error
    /// code at the given (1-based) range. Ignores message text.
    let private hasDiagAt (code: int) (sLine, sCol, eLine, eCol) (result: CompilationResult) : CompilationResult =
        let found =
            errorsOf result
            |> List.exists (fun d ->
                (match d.Error with ErrorType.Error c -> c = code | _ -> false)
                && d.Range.StartLine = sLine
                && d.Range.StartColumn + 1 = sCol
                && d.Range.EndLine = eLine
                && d.Range.EndColumn + 1 = eCol)
        if not found then
            failwithf
                "Expected diagnostic FS%04d at (Line %d, Col %d)-(Line %d, Col %d). Actual diagnostics:\n%s"
                code sLine sCol eLine eCol (dump result)
        result

    /// Assert NO error diagnostic has the zero/unknown range (1,1)-(1,1)
    /// (1-based), which is the symptom of issue 19550.
    let private hasNoRange0Error (result: CompilationResult) : CompilationResult =
        let bad =
            errorsOf result
            |> List.filter (fun d ->
                d.Range.StartLine = 1 && d.Range.StartColumn = 0
                && d.Range.EndLine = 1 && d.Range.EndColumn = 0)
        if not (List.isEmpty bad) then
            failwithf
                "Unexpected range0 / unknown(1,1) error diagnostic. All diagnostics:\n%s"
                (dump result)
        result

    // ------------------------------------------------------------------ //
    // Case 1 (PRIMARY): empty CE body in a left-to-right pipeline.
    //
    // Body of `foo {}` is empty -> Zero() is synthesized and returns
    // `int -> int`. Pipeline `"" |> foo {} |> printfn "%d"` fails to unify
    // `int -> int` with `string -> 'a`. The diagnostic must point at
    // `foo {}` (line 8, cols 7-13).
    //
    // CURRENT (broken) behaviour on `main`:
    //   FS0193 (1,0)-(1,0)  -- range0
    //
    // EXPECTED (post Sprint 02):
    //   FS0193 at Line 8, Col 7 - Line 8, Col 13.
    // ------------------------------------------------------------------ //
    [<Fact>]
    let ``Issue 19550 - empty CE body in pipeline reports source range``() =
        FSharp """
module Repro19550_01
type FooBuilder() =
  member _.Zero() = fun x -> x + 42

let foo = FooBuilder()

"" |> foo {} |> printfn "%d"
        """
        |> compile
        |> shouldFail
        |> hasDiagAt 193 (8, 7, 8, 13)

    // ------------------------------------------------------------------ //
    // Case 2 - single pipe with empty CE body (regression guard).
    //
    // For `foo {} |> printfn "%d"`, the type mismatch surfaces on the
    // printf format-string side (`"%d"` against an `int -> int`), so the
    // diagnostic already has a real, non-zero source range on `main`.
    // The bug (range0) does not appear here.
    //
    // We assert only that no error has the zero/unknown range, both as a
    // sanity check and to guard against the Sprint 02 fix regressing
    // this neighbouring path.
    // ------------------------------------------------------------------ //
    [<Fact>]
    let ``Issue 19550 - single pipe with empty CE body has non-zero range``() =
        FSharp """
module Repro19550_02
type FooBuilder() =
  member _.Zero() = fun x -> x + 42

let foo = FooBuilder()

foo {} |> printfn "%d"
        """
        |> compile
        |> shouldFail
        |> hasNoRange0Error

    // ------------------------------------------------------------------ //
    // Case 3 - empty CE body as function argument with type mismatch.
    // `take` expects `int`, `foo {}` has type `int -> int`.
    //
    // CURRENT: FS0193 (1,0)-(1,0)
    // EXPECTED: FS0193 at Line 9, Col 15 - Line 9, Col 21 (range of `foo {}`).
    // ------------------------------------------------------------------ //
    [<Fact>]
    let ``Issue 19550 - empty CE body as function argument``() =
        FSharp """
module Repro19550_03
type FooBuilder() =
  member _.Zero() = fun x -> x + 42

let foo = FooBuilder()

let take (x: int) = x
let _ = take (foo {})
        """
        |> compile
        |> shouldFail
        |> hasDiagAt 193 (9, 15, 9, 21)

    // ------------------------------------------------------------------ //
    // Case 4 - explicit type annotation mismatch on empty CE body.
    //
    // CURRENT: FS0193 (1,0)-(1,0)
    // EXPECTED: FS0193 at Line 8, Col 15 - Line 8, Col 21 (range of `foo {}`).
    //
    // (Per the prompt's Case 8 note: this single source proves the
    // bug surfaces even outside a pipeline - the `range0` flows from
    // `EmptyFieldListAsUnit` regardless of the surrounding context.)
    // ------------------------------------------------------------------ //
    [<Fact>]
    let ``Issue 19550 - explicit type annotation mismatch on empty CE body``() =
        FSharp """
module Repro19550_04
type FooBuilder() =
  member _.Zero() = fun x -> x + 42

let foo = FooBuilder()

let x : int = foo {}
        """
        |> compile
        |> shouldFail
        |> hasDiagAt 193 (8, 15, 8, 21)

    // ------------------------------------------------------------------ //
    // Case 5a - Yield builder in a pipeline (regression guard).
    //
    // The body is non-empty (`yield 1`), so `Yield(1)` is called rather
    // than `Zero()`; the bug does NOT manifest here (no range0). We
    // assert only that the diagnostic does not have range0, ensuring
    // the Sprint 02 fix does not regress non-Zero CE methods.
    // ------------------------------------------------------------------ //
    [<Fact>]
    let ``Issue 19550 - Yield builder in pipeline keeps non-zero range``() =
        FSharp """
module Repro19550_05a
type YBuilder() =
  member _.Yield(x) = fun y -> x + y

let yb = YBuilder()

"" |> yb { yield 1 } |> printfn "%d"
        """
        |> compile
        |> shouldFail
        |> hasNoRange0Error

    // ------------------------------------------------------------------ //
    // Case 5b - Return builder in a pipeline (regression guard).
    // Non-empty body, no Zero path triggered. As 5a.
    // ------------------------------------------------------------------ //
    [<Fact>]
    let ``Issue 19550 - Return builder in pipeline keeps non-zero range``() =
        FSharp """
module Repro19550_05b
type RBuilder() =
  member _.Return(x) = fun y -> x + y

let rb = RBuilder()

"" |> rb { return 1 } |> printfn "%d"
        """
        |> compile
        |> shouldFail
        |> hasNoRange0Error

    // ------------------------------------------------------------------ //
    // Case 5c - Bind builder in a pipeline (regression guard).
    // Non-empty body, no Zero path triggered. As 5a.
    // ------------------------------------------------------------------ //
    [<Fact>]
    let ``Issue 19550 - Bind builder in pipeline keeps non-zero range``() =
        FSharp """
module Repro19550_05c
type BBuilder() =
  member _.Bind(x, f) = f x
  member _.Return(x) = fun y -> x + y

let bb = BBuilder()

"" |> bb { let! x = 1 in return x } |> printfn "%d"
        """
        |> compile
        |> shouldFail
        |> hasNoRange0Error

    // ------------------------------------------------------------------ //
    // Case 6 - nested CE: inner empty `foo {}` inside an outer CE, in a
    // pipeline. The inner empty body must surface a non-zero source range.
    //
    // CURRENT: FS0193 (1,0)-(1,0)
    // EXPECTED: FS0193 at Line 16, Col 26 - Line 16, Col 32 (range of inner `foo {}`).
    // ------------------------------------------------------------------ //
    [<Fact>]
    let ``Issue 19550 - nested CE - inner empty body in pipeline``() =
        FSharp """
module Repro19550_06
type FooBuilder() =
  member _.Zero() = fun x -> x + 42

let foo = FooBuilder()

type Outer() =
  member _.Yield(_) : unit = ()
  member _.Combine(_, _) = ()
  member _.Delay(f: unit -> unit) = f ()
  member _.Zero() = ()

let outer = Outer()

let _ = outer { do "" |> foo {} |> ignore }
        """
        |> compile
        |> shouldFail
        |> hasDiagAt 193 (16, 26, 16, 32)

    // ------------------------------------------------------------------ //
    // Case 7 - POSITIVE: empty CE body with matching types must still
    // compile cleanly (guards `EmptyBodiedComputationExpressions`).
    // ------------------------------------------------------------------ //
    [<Fact>]
    let ``Issue 19550 - valid empty CE body without type mismatch still compiles``() =
        FSharp """
module Repro19550_07
type IdBuilder() =
  member _.Zero() = 0

let id1 = IdBuilder()
let v : int = id1 {}
        """
        |> compile
        |> shouldSucceed
        |> ignore

    // ------------------------------------------------------------------ //
    // Case 8 - non-pipeline empty CE body with explicit annotation.
    // Confirms the bug is NOT pipeline-specific: any context that tries
    // to unify the empty CE body's synthesized type triggers range0.
    //
    // CURRENT: FS0193 (1,0)-(1,0)
    // EXPECTED: FS0193 at Line 8, Col 28 - Line 8, Col 34 (range of `foo {}`).
    // ------------------------------------------------------------------ //
    [<Fact>]
    let ``Issue 19550 - non-pipeline empty CE body reports source range``() =
        FSharp """
module Repro19550_08
type FooBuilder() =
  member _.Zero() = fun x -> x + 42

let foo = FooBuilder()

let f : string -> string = foo {}
        """
        |> compile
        |> shouldFail
        |> hasDiagAt 193 (8, 28, 8, 34)

    // ------------------------------------------------------------------ //
    // Case 9 - POSITIVE-NEGATIVE: a non-CE pipeline mismatch already
    // has a proper source range and must keep it (independent of the
    // CE fix). Guards against the fix over-applying.
    // ------------------------------------------------------------------ //
    [<Fact>]
    let ``Issue 19550 - non-CE pipeline mismatch keeps non-zero range``() =
        FSharp """
module Repro19550_09
let _ = "" |> (fun (x: int) -> x) |> printfn "%d"
        """
        |> compile
        |> shouldFail
        |> hasNoRange0Error

    // ------------------------------------------------------------------ //
    // Case 10 - multi-line empty CE body.
    //
    // CURRENT: FS0193 (1,0)-(1,0)
    // EXPECTED: FS0193 spanning Line 9, Col 4 - Line 10, Col 5
    //           (i.e. the `foo {\n   }` span on lines 9-10).
    // ------------------------------------------------------------------ //
    [<Fact>]
    let ``Issue 19550 - multi-line empty CE body in pipeline``() =
        FSharp """
module Repro19550_10
type FooBuilder() =
  member _.Zero() = fun x -> x + 42

let foo = FooBuilder()

"" |>
   foo {
   } |>
   printfn "%d"
        """
        |> compile
        |> shouldFail
        |> hasDiagAt 193 (9, 4, 10, 5)

    // ------------------------------------------------------------------ //
    // Case 11 - empty CE body inside a match arm with type mismatch.
    //
    // CURRENT: FS0193 (1,0)-(1,0)
    // EXPECTED: FS0193 at Line 9, Col 22 - Line 9, Col 28 (range of `foo {}`).
    // ------------------------------------------------------------------ //
    [<Fact>]
    let ``Issue 19550 - empty CE body in match arm pipeline``() =
        FSharp """
module Repro19550_11
type FooBuilder() =
  member _.Zero() = fun x -> x + 42

let foo = FooBuilder()

let r = match 0 with
        | _ -> "" |> foo {} |> printfn "%d"
        """
        |> compile
        |> shouldFail
        |> hasDiagAt 193 (9, 22, 9, 28)

    // ------------------------------------------------------------------ //
    // Case 12 - builder exposing both `Zero` and `Yield`: an empty body
    // must still pick the `Zero` path and the diagnostic must point at
    // `bb {}` (not at range0).
    //
    // CURRENT: FS0193 (1,0)-(1,0)
    // EXPECTED: FS0193 at Line 9, Col 7 - Line 9, Col 12 (range of `bb {}`).
    // ------------------------------------------------------------------ //
    [<Fact>]
    let ``Issue 19550 - both Zero and Yield - empty body picks Zero path``() =
        FSharp """
module Repro19550_12
type BothBuilder() =
  member _.Zero() = fun x -> x + 1
  member _.Yield(x) = fun y -> x + y

let bb = BothBuilder()

"" |> bb {} |> printfn "%d"
        """
        |> compile
        |> shouldFail
        |> hasDiagAt 193 (9, 7, 9, 12)
