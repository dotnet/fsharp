// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Quotation rendering snapshots — regression for https://github.com/dotnet/fsharp/issues/19873.

namespace Conformance.Expressions.ExpressionQuotations

open System.IO
open Microsoft.FSharp.Quotations
open Xunit
open FSharp.Test.Compiler

module QuotationRendering =

    let private baselineDir = __SOURCE_DIRECTORY__

    let private renderToBaseline (name: string) (q: Expr) =
        checkBaseline (sprintf "%A\n" q) (Path.Combine(baselineDir, name + ".bsl"))

    [<Fact>]
    let EmptyString () =
        renderToBaseline "EmptyString" <@ fun (x: string) -> match x with "" -> 1 | _ -> 0 @>

    [<Fact>]
    let NullOrEmpty () =
        renderToBaseline "NullOrEmpty" <@ fun (x: string) -> match x with null | "" -> 1 | _ -> 0 @>

    [<Fact>]
    let NonEmptyString () =
        renderToBaseline "NonEmptyString" <@ fun (x: string) -> match x with "a" -> 1 | "b" -> 2 | _ -> 0 @>

    [<Fact>]
    let ConsecutiveInts () =
        renderToBaseline "ConsecutiveInts" <@ fun (x: int) -> match x with 1 -> "a" | 2 -> "b" | 3 -> "c" | _ -> "z" @>

    [<Fact>]
    let Chars () =
        renderToBaseline "Chars" <@ fun (x: char) -> match x with 'a' -> 1 | 'b' -> 2 | _ -> 0 @>

    // Int64 takes the mkILAsmCeq arm + [AI_ceq] -> op_Equality recovery (distinct from the op_Equality-direct primitives).
    [<Fact>]
    let Int64 () =
        renderToBaseline "Int64" <@ fun (x: int64) -> match x with 1L -> "a" | _ -> "b" @>

    [<Fact>]
    let Decimal () =
        renderToBaseline "Decimal" <@ fun (x: decimal) -> match x with 1m -> "a" | _ -> "b" @>
