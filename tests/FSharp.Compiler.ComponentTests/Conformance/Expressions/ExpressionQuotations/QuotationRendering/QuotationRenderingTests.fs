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
