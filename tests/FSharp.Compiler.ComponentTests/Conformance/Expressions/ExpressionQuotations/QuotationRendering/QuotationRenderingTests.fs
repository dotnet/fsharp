// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Quotation rendering snapshots — regression for https://github.com/dotnet/fsharp/issues/19873.
// Quotation literals are evaluated at test runtime via a shared FSI session that uses the
// just-built FSharp.Compiler.Service, so the desugar under test is the one this PR ships
// (the bootstrap fsc that builds this test project still has the pre-fix desugar and
// rejects literal `match s with ""` quotations with FS0452).

namespace Conformance.Expressions.ExpressionQuotations

open System.IO
open Xunit
open FSharp.Test.Compiler
open FSharp.Test.ScriptHelpers

module QuotationRendering =

    let private baselineDir = __SOURCE_DIRECTORY__

    let private fsiSession = getSessionForEval [||] LangVersion.Preview

    let private quoteShouldRender (name: string) (quoteExpr: string) =
        let result =
            Fsx (sprintf "printfn \"%%A\" %s" quoteExpr)
            |> evalInSharedSession fsiSession
            |> shouldSucceed
        match result.RunOutput with
        | Some (EvalOutput e) ->
            checkBaseline (e.StdOut |> normalizeNewlines) (Path.Combine(baselineDir, name + ".bsl"))
        | _ ->
            failwith "Expected eval output from shared FSI session."

    [<Fact>]
    let EmptyString () =
        quoteShouldRender "EmptyString" """<@ fun (x: string) -> match x with "" -> 1 | _ -> 0 @>"""

    [<Fact>]
    let NullOrEmpty () =
        quoteShouldRender "NullOrEmpty" """<@ fun (x: string) -> match x with null | "" -> 1 | _ -> 0 @>"""

    [<Fact>]
    let NonEmptyString () =
        quoteShouldRender "NonEmptyString" """<@ fun (x: string) -> match x with "a" -> 1 | "b" -> 2 | _ -> 0 @>"""

    [<Fact>]
    let ConsecutiveInts () =
        quoteShouldRender "ConsecutiveInts" """<@ fun (x: int) -> match x with 1 -> "a" | 2 -> "b" | 3 -> "c" | _ -> "z" @>"""

    [<Fact>]
    let Chars () =
        quoteShouldRender "Chars" """<@ fun (x: char) -> match x with 'a' -> 1 | 'b' -> 2 | _ -> 0 @>"""

    // Int64 takes the mkILAsmCeq arm + [AI_ceq] -> op_Equality recovery (distinct from the op_Equality-direct primitives).
    [<Fact>]
    let Int64 () =
        quoteShouldRender "Int64" """<@ fun (x: int64) -> match x with 1L -> "a" | _ -> "b" @>"""

    [<Fact>]
    let Decimal () =
        quoteShouldRender "Decimal" """<@ fun (x: decimal) -> match x with 1m -> "a" | _ -> "b" @>"""

    // FS-1073: a positional record-constructor call must quote identically to record syntax. Both lower to
    // the same NewRecord node before quotation translation, so the quotation contains no constructor call -
    // it renders exactly like { A = 1; B = 2 }.
    [<Fact>]
    let RecordConstructor () =
        let source = """
type R = { A: int; B: int }
let viaCtor = <@ R(1, 2) @>
let viaRecord = <@ { A = 1; B = 2 } @>
System.Console.WriteLine(viaCtor.ToString())
System.Console.WriteLine(viaCtor.ToString() = viaRecord.ToString())
"""
        let result =
            Fsx source
            |> evalInSharedSession fsiSession
            |> shouldSucceed
        match result.RunOutput with
        | Some (EvalOutput e) ->
            checkBaseline (e.StdOut |> normalizeNewlines) (Path.Combine(baselineDir, "RecordConstructor.bsl"))
        | _ ->
            failwith "Expected eval output from shared FSI session."

    // --- Issue #18425: join-point sharing of a guarded shared-or residual, as reflected in quotations. ---
    //
    // A single match clause whose disjuncts share one `when` guard and contain PARTIAL active patterns used
    // to duplicate the residual decision subtree once per disjunct. That expansion is the 2^N compile-time /
    // DLL-size / StackOverflow blow-up of #18425. The fix compiles each distinct residual state ONCE into a
    // let-bound `joinThunk` lambda that every later path calls, e.g.:
    //       Let (joinThunk, Lambda (unitArg, <the shared residual>),
    //            <body that reaches the residual by calling joinThunk ()>)
    // Because quotations reflect the elaborated decision tree, the change is directly observable here.
    //
    // Sharing only kicks in once a residual is reached MORE than the promotion threshold (32) times, so all
    // ordinary matches keep their pristine quotation verbatim (the N=6 control below is byte-identical); only
    // the exponential #18425 shape crosses the threshold (N>=7 here) and shares. Tests assert on the
    // stamp-free `joinThunk` marker rather than a full .bsl snapshot, which would churn on unrelated
    // `activePatternResultNNN` stamp shifts.
    let private renderGuardedOrQuote (quoteExpr: string) : string =
        let prelude =
            "let (|E|_|) (n: int) (x: int) = if x = n then Some x else None\n"
            + "let (|A|_|) (x: int) = if x % 2 = 0 then Some (x / 2) else None\n"
            + "let g (p: int) = p > 1000\n"
        let result =
            Fsx (prelude + sprintf "printfn \"%%A\" %s" quoteExpr)
            |> evalInSharedSession fsiSession
            |> shouldSucceed
        match result.RunOutput with
        | Some (EvalOutput e) -> e.StdOut |> normalizeNewlines
        | _ -> failwith "Expected eval output from shared FSI session."

    [<Fact>]
    let ``Issue 18425 - guarded shared-or below the sharing threshold keeps the pristine quotation`` () =
        // Six disjuncts stay under the promotion threshold, so no join is introduced: an ordinary match is
        // compiled exactly as the pristine compiler would.
        let rendered = renderGuardedOrQuote """<@ fun (x: int) -> match x with (E 1 _ | E 2 _ | E 3 _ | E 4 _ | E 5 _ | E 6 _) when g 0 -> 1 | _ -> 0 @>"""
        Assert.DoesNotContain("joinThunk", rendered)

    [<Fact>]
    let ``Issue 18425 - guarded shared-or shares the residual as a single join above the threshold`` () =
        // Above the threshold the shared residual is compiled once into a join that every path calls.
        let rendered = renderGuardedOrQuote """<@ fun (x: int) -> match x with (E 1 _ | E 2 _ | E 3 _ | E 4 _ | E 5 _ | E 6 _ | E 7 _ | E 8 _) when g 0 -> 1 | _ -> 0 @>"""
        Assert.Contains("joinThunk", rendered)

    [<Fact>]
    let ``Issue 18425 - shared join threads a bound pattern variable through the tuple or-pattern`` () =
        // The canonical #18425 shape: a shared partial AP in column 0 binds `p`, read by the shared guard and
        // result; the join captures and forwards `p` through its parameter.
        let rendered = renderGuardedOrQuote """<@ fun (a: int) (b: int) -> match a, b with (A p, E 1 _) | (A p, E 2 _) | (A p, E 3 _) | (A p, E 4 _) | (A p, E 5 _) | (A p, E 6 _) | (A p, E 7 _) | (A p, E 8 _) when g p -> p | _ -> 0 @>"""
        Assert.Contains("joinThunk", rendered)

