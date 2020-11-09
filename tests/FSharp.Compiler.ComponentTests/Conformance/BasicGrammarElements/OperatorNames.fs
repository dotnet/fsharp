// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.BasicGrammarElements

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module OperatorNames =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicGrammarElements/OperatorNames)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicGrammarElements/OperatorNames", Includes=[|"AstrSymbOper01.fs"|])>]
    let ``OperatorNames - AstrSymbOper01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicGrammarElements/OperatorNames)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicGrammarElements/OperatorNames", Includes=[|"BasicOperatorNames.fs"|])>]
    let ``OperatorNames - BasicOperatorNames.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicGrammarElements/OperatorNames)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicGrammarElements/OperatorNames", Includes=[|"EqualOperatorsOverloading.fs"|])>]
    let ``OperatorNames - EqualOperatorsOverloading.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicGrammarElements/OperatorNames)
    //<Expects id="FS0035" span="(8,5-8,22)" status="error">This construct is deprecated: '\$' is not permitted as a character in operator names and is reserved for future use</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicGrammarElements/OperatorNames", Includes=[|"E_BasicOperatorNames01.fs"|])>]
    let ``OperatorNames - E_BasicOperatorNames01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0035
        |> withDiagnosticMessageMatches "This construct is deprecated: '\$' is not permitted as a character in operator names and is reserved for future use"

