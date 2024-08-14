// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.UnitsOfMeasure

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Parsing =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Parsing)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Parsing", Includes=[|"GreaterBarRBrack01.fs"|])>]
    let ``Parsing - GreaterBarRBrack01_fs`` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Parsing)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Parsing", Includes=[|"Reciprocal01.fs"|])>]
    let ``Parsing - Reciprocal01_fs`` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Parsing)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Parsing", Includes=[|"QuotientAssoc.fs"|])>]
    let ``Parsing - QuotientAssoc_fs`` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Parsing)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Parsing", Includes=[|"Quotient.fs"|])>]
    let ``Parsing - Quotient_fs`` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Parsing)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Parsing", Includes=[|"PowerSynonym.fs"|])>]
    let ``Parsing - PowerSynonym_fs`` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldSucceed
        |> ignore

