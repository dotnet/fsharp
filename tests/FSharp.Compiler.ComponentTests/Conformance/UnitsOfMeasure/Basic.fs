// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.UnitsOfMeasure

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Basic =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Basic)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Basic", Includes=[|"Misc01.fs"|])>]
    let ``Basic - Misc01_fs`` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Basic)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Basic", Includes=[|"Misc03.fs"|])>]
    let ``Basic - Misc03_fs`` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Basic)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Basic", Includes=[|"Stats.fs"|])>]
    let ``Basic - Stats_fs`` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Basic)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Basic", Includes=[|"SI.fs"|])>]
    let ``Basic - SI_fs`` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Basic)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Basic", Includes=[|"RationalExponents01.fs"|])>]
    let ``Basic - RationalExponents01_fs`` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Basic)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Basic", Includes=[|"Quotation04_hidden.fs"|])>]
    let ``Basic - Quotation04_hidden_fs`` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldSucceed
        |> ignore

