// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Snapshot tests for the F# rendering of quoted expressions (sprintf "%A" <@ ... @>).
//
// Each test compiles a tiny program that prints the rendered quotation, executes it, and diffs
// the captured stdout against a .bsl file in this folder. Update baselines with TEST_UPDATE_BSL=1.
//
// Purpose: detect accidental leakage of pattern-match-compilation lowerings (range checks, jump
// tables, null+Length rewrites, etc.) into the public quotation AST. The quotation translator
// runs after PatternMatchCompilation, so any rewrite that lives in PatternMatchCompilation will
// also show up here as a (potentially surprising) shape change.
//
// Related: https://github.com/dotnet/fsharp/issues/19873, PR https://github.com/dotnet/fsharp/pull/19532

namespace Conformance.Expressions.ExpressionQuotations

open System.IO
open Xunit
open FSharp.Test.Compiler

module QuotationRendering =

    let private baselineDir = __SOURCE_DIRECTORY__

    /// Compile a snippet that builds a quotation `q` and prints it with `printfn "%A" q`,
    /// then diff its stdout against `<bslName>.bsl`.
    let private renderingShouldMatch (bslName: string) (quoteBody: string) =
        let program = "module Test\n[<EntryPoint>]\nlet main _ =\n    let q = " + quoteBody + "\n    printfn \"%A\" q\n    0\n"
        let bslPath = Path.Combine(baselineDir, bslName + ".bsl")

        FSharp program
        |> asExe
        |> ignoreWarnings
        |> compileExeAndRun
        |> shouldSucceed
        |> verifyOutputAgainstBaseline bslPath
        |> ignore

    // ----------------------------------------------------------------------
    // The "leaky" cases: PatternMatchCompilation rewrites that show up in quotations.
    // ----------------------------------------------------------------------

    // https://github.com/dotnet/fsharp/issues/19873
    // Empty-string match leaks `IfThenElse (op_Inequality(x,null), x.Length = 0, false)`.
    [<Fact>]
    let ``Match empty string`` () =
        renderingShouldMatch "EmptyStringMatch" "<@ fun (x: string) -> match x with \"\" -> 1 | _ -> 0 @>"

    // pezipink's second example in #19873 — the same lowering also appears for plain `s = ""`.
    [<Fact>]
    let ``If-then-else with literal empty string equality`` () =
        renderingShouldMatch "IfThenElseLiteralEqEmpty" "<@ fun (s: string) -> if s = \"\" then 1 else 0 @>"

    // null | "" -> ... should consolidate to a single null check, then a length check.
    [<Fact>]
    let ``Match null or empty string`` () =
        renderingShouldMatch "NullOrEmptyMatch" "<@ fun (x: string) -> match x with null | \"\" -> 1 | _ -> 0 @>"

    // Array-length match also lowers to null + Length check inline.
    // This is a pre-existing leakage: BuildSwitch lowers `match a with [| _ |] -> _` into raw
    // `I_ldlen` inline IL via `mkLdlen` before the quotation translator runs. The translator
    // then rejects the raw IL with FS0452. When the optimization is moved to a later phase,
    // this test should be flipped to assert successful compilation and a clean rendering.
    [<Fact>]
    let ``Match array length fails in quotation (FS0452)`` () =
        FSharp """module Test
let q = <@ fun (a: int[]) -> match a with [| _ |] -> 1 | _ -> 0 @>
"""
        |> asLibrary
        |> typecheck
        |> shouldFail
        |> withErrorCode 452
        |> withDiagnosticMessageMatches "Quotations cannot contain inline assembly code or pattern matching on arrays"
        |> ignore

    // ----------------------------------------------------------------------
    // Reference cases: confirm other PatternMatchCompilation optimizations do
    // NOT leak. These baselines should be a clean if/then/else chain of
    // op_Equality calls regardless of compactification or jump-table lowering.
    // ----------------------------------------------------------------------

    [<Fact>]
    let ``Match non-empty strings`` () =
        renderingShouldMatch "NonEmptyStringMatch" "<@ fun (x: string) -> match x with \"foo\" -> 1 | \"bar\" -> 2 | _ -> 0 @>"

    // Consecutive ints 1..10 — PatternMatchCompilation compactifies them into a TDSwitch
    // which (in IL) eventually becomes a switch table. The quotation should *not* see that;
    // it should be a chain of op_Equality tests.
    [<Fact>]
    let ``Match consecutive ints 1 to 10`` () =
        renderingShouldMatch "ConsecutiveIntMatch"
            "<@ fun (i: int) -> match i with 1 -> \"one\" | 2 -> \"two\" | 3 -> \"three\" | 4 -> \"four\" | 5 -> \"five\" | 6 -> \"six\" | 7 -> \"seven\" | 8 -> \"eight\" | 9 -> \"nine\" | 10 -> \"ten\" | _ -> \"other\" @>"

    [<Fact>]
    let ``Match sparse ints`` () =
        renderingShouldMatch "SparseIntMatch"
            "<@ fun (i: int) -> match i with 1 -> \"a\" | 5 -> \"b\" | 10 -> \"c\" | _ -> \"other\" @>"

    [<Fact>]
    let ``Match chars`` () =
        renderingShouldMatch "CharMatch"
            "<@ fun (c: char) -> match c with 'a' -> 1 | 'b' -> 2 | 'c' -> 3 | _ -> 0 @>"

    [<Fact>]
    let ``Match int64`` () =
        renderingShouldMatch "Int64Match"
            "<@ fun (i: int64) -> match i with 1L -> \"a\" | 2L -> \"b\" | 3L -> \"c\" | _ -> \"other\" @>"

    [<Fact>]
    let ``Match float`` () =
        renderingShouldMatch "FloatMatch"
            "<@ fun (f: float) -> match f with 1.0 -> \"a\" | 2.0 -> \"b\" | _ -> \"other\" @>"

    [<Fact>]
    let ``Match decimal`` () =
        renderingShouldMatch "DecimalMatch"
            "<@ fun (d: decimal) -> match d with 1m -> \"a\" | 2m -> \"b\" | _ -> \"other\" @>"
