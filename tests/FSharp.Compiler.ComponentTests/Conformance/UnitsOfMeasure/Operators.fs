// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.UnitsOfMeasure

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module Operators =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Operators)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Operators", Includes=[|"Operators_decimal_01.fs"|])>]
    let ``Operators - Operators_decimal_01.fs - -a`` compilation =
        compilation
        |> withOptions ["-a"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Operators)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Operators", Includes=[|"Operators_float32_01.fs"|])>]
    let ``Operators - Operators_float32_01.fs - -a`` compilation =
        compilation
        |> withOptions ["-a"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Operators)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Operators", Includes=[|"Operators_float_01.fs"|])>]
    let ``Operators - Operators_float_01.fs - -a`` compilation =
        compilation
        |> withOptions ["-a"]
        |> typecheck
        |> shouldSucceed

