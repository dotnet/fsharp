// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.UnitsOfMeasure

open Xunit
open FSharp.Test.Compiler
open FSharp.Test.Xunit.Attributes

module Diagnostics =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Diagnostics)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Diagnostics", Includes=[|"RangeExpression01.fs"|])>]
    let ``Diagnostics - RangeExpression01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldSucceed
        |> ignore

