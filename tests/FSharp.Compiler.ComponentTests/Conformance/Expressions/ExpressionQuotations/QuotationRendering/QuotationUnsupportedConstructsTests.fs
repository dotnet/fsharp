// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Diagnostic test (FS0452): array-length patterns lower to `I_ldlen` IL via `mkLdlen` BEFORE the
// quotation translator runs, which rejects them. Lives in its own file because it uses the
// typecheck/diagnostic harness (FSharp |> typecheck |> shouldFail), not the in-process baseline
// harness in the sibling QuotationRenderingTests.fs.
//
// Out of scope for #19873. Flip to shouldSucceed and add an `ArrayLength.bsl` baseline when that
// lowering moves out of BuildSwitch.

namespace Conformance.Expressions.ExpressionQuotations

open Xunit
open FSharp.Test.Compiler

module QuotationUnsupportedConstructs =

    [<Fact>]
    let ``match x with array length still errors with FS0452`` () =
        FSharp """module Test
let q = <@ fun (x: int[]) -> match x with [| _ |] -> 1 | _ -> 0 @>
"""
        |> asLibrary
        |> typecheck
        |> shouldFail
        |> withErrorCode 452
        |> withDiagnosticMessageMatches "Quotations cannot contain inline assembly code or pattern matching on arrays"
        |> ignore
