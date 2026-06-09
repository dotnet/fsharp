// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Quotation rendering snapshots — regression for https://github.com/dotnet/fsharp/issues/19873.
// Literals are desugared by the bootstrap fsc that built the test project; rerun with
// `./build.sh --bootstrap -test` after touching `src/Compiler/Checking/PatternMatchCompilation.fs`.

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
        checkBaseline (sprintf "// %s\n%s\n" name body) (Path.Combine(baselineDir, name + ".bsl"))

    let rec private containsEmptyStringLowering (e: Expr) =
        let isString (t: System.Type) = not (isNull t) && t.FullName = "System.String"
        match e with
        | PropertyGet (_, pi, _) when isString pi.DeclaringType && pi.Name = "Length" -> true
        | Call (_, mi, _) when isString mi.DeclaringType && mi.Name = "Equals" -> true
        | ShapeCombination (_, args) -> args |> List.exists containsEmptyStringLowering
        | ShapeLambda (_, body) -> containsEmptyStringLowering body
        | ShapeVar _ -> false

    let private assertNoEmptyStringLowering (q: Expr) =
        Assert.False(containsEmptyStringLowering q,
            sprintf "Quotation leaked the empty-string lowering (String.Length or String.Equals):\n%A" q)

    // Shared so the convergence test below cannot drift from the baselined exprs.
    let private qMatchEmpty   = <@ fun (x: string) -> match x with "" -> 1 | _ -> 0 @>
    let private qIfEqualEmpty = <@ fun (x: string) -> if x = "" then 1 else 0 @>

    [<Fact>]
    let ``match x with empty string renders as op_Equality (#19873)`` () =
        assertNoEmptyStringLowering qMatchEmpty
        renderToBaseline "EmptyString" qMatchEmpty

    [<Fact>]
    let ``match x with null or empty string renders as op_Equality (#19873)`` () =
        let q = <@ fun (x: string) -> match x with null | "" -> 1 | _ -> 0 @>
        assertNoEmptyStringLowering q
        renderToBaseline "NullOrEmpty" q

    [<Fact>]
    let NonEmptyString () = renderToBaseline "NonEmptyString" <@ fun (x: string) -> match x with "a" -> 1 | "b" -> 2 | _ -> 0 @>

    [<Fact>]
    let ConsecutiveInts () = renderToBaseline "ConsecutiveInts" <@ fun (x: int) -> match x with 1 -> "a" | 2 -> "b" | 3 -> "c" | _ -> "z" @>

    [<Fact>]
    let Chars () = renderToBaseline "Chars" <@ fun (x: char) -> match x with 'a' -> 1 | 'b' -> 2 | _ -> 0 @>

    // Int64 takes the mkILAsmCeq arm + [AI_ceq] -> op_Equality recovery; the other primitives go through op_Equality directly.
    [<Fact>]
    let Int64 () = renderToBaseline "Int64" <@ fun (x: int64) -> match x with 1L -> "a" | _ -> "b" @>

    [<Fact>]
    let Decimal () = renderToBaseline "Decimal" <@ fun (x: decimal) -> match x with 1m -> "a" | _ -> "b" @>

    [<Fact>]
    let ``if x = empty string renders cleanly (second repro from #19873)`` () =
        assertNoEmptyStringLowering qIfEqualEmpty
        renderToBaseline "IfEqualEmpty" qIfEqualEmpty

    [<Fact>]
    let ``match-empty and if-equal-empty quotations converge to the same Expr`` () =
        Assert.Equal(sprintf "%A" qMatchEmpty, sprintf "%A" qIfEqualEmpty)
