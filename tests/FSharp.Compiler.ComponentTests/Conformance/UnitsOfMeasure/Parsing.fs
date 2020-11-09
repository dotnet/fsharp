// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.UnitsOfMeasure

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module Parsing =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Parsing)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Parsing", Includes=[|"GreaterBarRBrack01.fs"|])>]
    let ``Parsing - GreaterBarRBrack01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Parsing)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Parsing", Includes=[|"Reciprocal01.fs"|])>]
    let ``Parsing - Reciprocal01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Parsing)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Parsing", Includes=[|"QuotientAssoc.fs"|])>]
    let ``Parsing - QuotientAssoc.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Parsing)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Parsing", Includes=[|"Quotient.fs"|])>]
    let ``Parsing - Quotient.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Parsing)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Parsing", Includes=[|"PowerSynonym.fs"|])>]
    let ``Parsing - PowerSynonym.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Parsing)
    //<Expects span="(24,38-24,41)" id="FS0705" status="error">Expected unit-of-measure, not type</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Parsing", Includes=[|"KnownTypeAsUnit01.fs"|])>]
    let ``Parsing - KnownTypeAsUnit01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0705
        |> withDiagnosticMessageMatches "Expected unit-of-measure, not type"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Parsing)
    //<Expects span="(26,38-26,41)" status="error" id="FS0705">Expected unit-of-measure, not type</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Parsing", Includes=[|"KnownTypeAsUnit01b.fs"|])>]
    let ``Parsing - KnownTypeAsUnit01b.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0705
        |> withDiagnosticMessageMatches "Expected unit-of-measure, not type"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Parsing)
    //<Expects id="FS0707" span="(17,17-17,21)" status="error">Unit-of-measure cannot be used in type constructor application</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Parsing", Includes=[|"E_Nesting01.fs"|])>]
    let ``Parsing - E_Nesting01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0707
        |> withDiagnosticMessageMatches "Unit-of-measure cannot be used in type constructor application"

