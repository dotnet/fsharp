// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.Expressions.DataExpressions

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module AddressOf =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/AddressOf)
    //<Expects id="FS0431" span="(9,7-9,8)" status="error">A byref typed value would be stored here\. Top-level let-bound byref values are not permitted</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/AddressOf", Includes=[|"E_byrefvaluesnotpermitted001.fs"|])>]
    let ``AddressOf - E_byrefvaluesnotpermitted001.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0431
        |> withDiagnosticMessageMatches "A byref typed value would be stored here\. Top-level let-bound byref values are not permitted"

