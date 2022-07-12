// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.UnitsOfMeasure

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Parsing =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Parsing)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Parsing", Includes=[|"GreaterBarRBrack01.fs"|])>]
    let ``Parsing - GreaterBarRBrack01.fs - `` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Parsing)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Parsing", Includes=[|"Reciprocal01.fs"|])>]
    let ``Parsing - Reciprocal01.fs - `` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Parsing)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Parsing", Includes=[|"QuotientAssoc.fs"|])>]
    let ``Parsing - QuotientAssoc.fs - `` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Parsing)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Parsing", Includes=[|"Quotient.fs"|])>]
    let ``Parsing - Quotient.fs - `` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Parsing)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Parsing", Includes=[|"PowerSynonym.fs"|])>]
    let ``Parsing - PowerSynonym.fs - `` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldSucceed
        |> ignore

