// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.UnitsOfMeasure

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module Bounds =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Bounds)
    //<Expects id="FS0717" span="(15,19-15,27)" status="error">Unexpected type arguments</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Bounds", Includes=[|"infinity_01.fs"|])>]
    let ``Bounds - infinity_01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0717
        |> withDiagnosticMessageMatches "Unexpected type arguments"

