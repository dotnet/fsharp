// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.UnitsOfMeasure

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module Parenthesis =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Parenthesis)
    //<Expects id="FS0464" span="(24,33-24,39)" status="warning">This code is less generic than indicated by its annotations\. A unit-of-measure specified using '_' has been determined to be '1', i.e. dimensionless\. Consider making the code generic, or removing the use of '_'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Parenthesis", Includes=[|"Positive01.fs"|])>]
    let ``Parenthesis - Positive01.fs - --test:ErrorRanges -a`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "-a"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0464
        |> withDiagnosticMessageMatches "This code is less generic than indicated by its annotations\. A unit-of-measure specified using '_' has been determined to be '1', i.e. dimensionless\. Consider making the code generic, or removing the use of '_'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Parenthesis)
    //<Expects id="FS0632" span="(11,25-11,32)" status="warning">Implicit product of measures following /</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Parenthesis", Includes=[|"W_ImplicitProduct01.fs"|])>]
    let ``Parenthesis - W_ImplicitProduct01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0632
        |> withDiagnosticMessageMatches "Implicit product of measures following /"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Parenthesis)
    //<Expects id="FS0620" span="(11,19-11,20)" status="error">Unexpected integer literal in unit-of-measure expression</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Parenthesis", Includes=[|"E_Error02.fs"|])>]
    let ``Parenthesis - E_Error02.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0620
        |> withDiagnosticMessageMatches "Unexpected integer literal in unit-of-measure expression"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Parenthesis)
    //<Expects id="FS0010" span="(11,31-11,32)" status="error">Unexpected symbol '\)' in binding</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Parenthesis", Includes=[|"E_Error03.fs"|])>]
    let ``Parenthesis - E_Error03.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected symbol '\)' in binding"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Parenthesis)
    //<Expects id="FS0010" span="(11,34-11,35)" status="error">Unexpected symbol '_' in binding</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Parenthesis", Includes=[|"E_Error04.fs"|])>]
    let ``Parenthesis - E_Error04.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected symbol '_' in binding"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Parenthesis)
    //<Expects id="FS0010" span="(11,28-11,29)" status="error">Unexpected symbol '\)' in binding</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Parenthesis", Includes=[|"E_Error05.fs"|])>]
    let ``Parenthesis - E_Error05.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected symbol '\)' in binding"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Parenthesis)
    //<Expects id="FS0620" span="(11,20-11,23)" status="error">Unexpected integer literal in unit-of-measure expression</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Parenthesis", Includes=[|"E_Error06.fs"|])>]
    let ``Parenthesis - E_Error06.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0620
        |> withDiagnosticMessageMatches "Unexpected integer literal in unit-of-measure expression"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Parenthesis)
    //<Expects status="error" span="(10,26-10,27)" id="FS0583">Unmatched '\('$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Parenthesis", Includes=[|"E_IncompleteParens01.fs"|])>]
    let ``Parenthesis - E_IncompleteParens01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0583
        |> withDiagnosticMessageMatches "Unmatched '\('$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Parenthesis)
    //<Expects status="error" span="(10,26-10,27)" id="FS0010">Unexpected symbol '\)' in expression$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Parenthesis", Includes=[|"E_IncompleteParens02.fs"|])>]
    let ``Parenthesis - E_IncompleteParens02.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected symbol '\)' in expression$"

