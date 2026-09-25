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

    let private renderFsx source =
        let result = Fsx source |> evalInSharedSession fsiSession |> shouldSucceed

        match result.RunOutput with
        | Some(EvalOutput e) -> e.StdOut |> normalizeNewlines
        | _ -> failwith "Expected eval output from shared FSI session."

    let private quoteShouldRender (name: string) (quoteExpr: string) =
        checkBaseline (renderFsx (sprintf "printfn \"%%A\" %s" quoteExpr)) (Path.Combine(baselineDir, name + ".bsl"))

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
        checkBaseline (renderFsx source) (Path.Combine(baselineDir, "RecordConstructor.bsl"))

    let private renderGuardedOrQuote quoteExpr =
        renderFsx (
            "let (|E|_|) (n: int) (x: int) = if x = n then Some x else None\n"
            + "let (|A|_|) (x: int) = if x % 2 = 0 then Some (x / 2) else None\n"
            + "let g (p: int) = p > 1000\n"
            + sprintf "printfn \"%%A\" %s" quoteExpr
        )

    [<Theory>]
    [<InlineData(6, false)>]
    [<InlineData(8, true)>]
    let ``Issue 18425 - guarded shared-or shares quotations only above the threshold`` disjunctCount expectJoin =
        let patterns = [ 1..disjunctCount ] |> List.map (sprintf "E %d _") |> String.concat " | "
        let rendered = renderGuardedOrQuote (sprintf "<@ fun (x: int) -> match x with (%s) when g 0 -> 1 | _ -> 0 @>" patterns)
        if expectJoin then Assert.Contains("joinThunk", rendered) else Assert.DoesNotContain("joinThunk", rendered)

    [<Fact>]
    let ``Issue 18425 - shared join threads a bound pattern variable through the tuple or-pattern`` () =
        let patterns = [ 1..8 ] |> List.map (sprintf "(A p, E %d _)") |> String.concat " | "
        let rendered = renderGuardedOrQuote (sprintf "<@ fun (a: int) (b: int) -> match a, b with %s when g p -> p | _ -> 0 @>" patterns)
        Assert.Contains("joinThunk", rendered)
