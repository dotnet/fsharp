// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.BasicGrammarElements

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module PrecedenceAndOperators =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicGrammarElements/PrecedenceAndOperators)
    //<Expects id="FS0003" span="(15,12-15,21)" status="error">This value is not a function and cannot be applied</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicGrammarElements/PrecedenceAndOperators", Includes=[|"E_ExclamationMark01.fs"|])>]
    let ``PrecedenceAndOperators - E_ExclamationMark01.fs - --test:ErrorRanges -a`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "-a"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0003
        |> withDiagnosticMessageMatches "This value is not a function and cannot be applied"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicGrammarElements/PrecedenceAndOperators)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicGrammarElements/PrecedenceAndOperators", Includes=[|"ExclamationMark02.fs"|])>]
    let ``PrecedenceAndOperators - ExclamationMark02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicGrammarElements/PrecedenceAndOperators)
    //<Expects status="error" span="(18,27-18,28)" id="FS1208">Invalid prefix operator$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicGrammarElements/PrecedenceAndOperators", Includes=[|"E_Tilde01.fs"|])>]
    let ``PrecedenceAndOperators - E_Tilde01.fs - --test:ErrorRanges -a`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "-a"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1208
        |> withDiagnosticMessageMatches "Invalid prefix operator$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicGrammarElements/PrecedenceAndOperators)
    //<Expects status="error" span="(16,16-16,17)" id="FS1208">Invalid prefix operator$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicGrammarElements/PrecedenceAndOperators", Includes=[|"E_Tilde02.fs"|])>]
    let ``PrecedenceAndOperators - E_Tilde02.fs - --test:ErrorRanges -a`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "-a"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1208
        |> withDiagnosticMessageMatches "Invalid prefix operator$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicGrammarElements/PrecedenceAndOperators)
    //<Expects status="error" span="(13,31-13,32)" id="FS0001">The type 'uint64' does not support the operator '~-'$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicGrammarElements/PrecedenceAndOperators", Includes=[|"E_Negation01.fs"|])>]
    let ``PrecedenceAndOperators - E_Negation01.fs - --test:ErrorRanges -a`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "-a"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "The type 'uint64' does not support the operator '~-'$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicGrammarElements/PrecedenceAndOperators)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicGrammarElements/PrecedenceAndOperators", Includes=[|"Negation01.fs"|])>]
    let ``PrecedenceAndOperators - Negation01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicGrammarElements/PrecedenceAndOperators)
    //<Expects status="error" span="(16,27-16,28)" id="FS1208">Invalid prefix operator</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicGrammarElements/PrecedenceAndOperators", Includes=[|"E_QuestionMark01.fs"|])>]
    let ``PrecedenceAndOperators - E_QuestionMark01.fs - --test:ErrorRanges -a`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "-a"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1208
        |> withDiagnosticMessageMatches "Invalid prefix operator"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicGrammarElements/PrecedenceAndOperators)
    //<Expects status="error" span="(14,16-14,17)" id="FS1208">Invalid prefix operator$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicGrammarElements/PrecedenceAndOperators", Includes=[|"E_QuestionMark02.fs"|])>]
    let ``PrecedenceAndOperators - E_QuestionMark02.fs - --test:ErrorRanges -a`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "-a"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1208
        |> withDiagnosticMessageMatches "Invalid prefix operator$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicGrammarElements/PrecedenceAndOperators)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicGrammarElements/PrecedenceAndOperators", Includes=[|"VerticalbarOptionalinDU.fs"|])>]
    let ``PrecedenceAndOperators - VerticalbarOptionalinDU.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicGrammarElements/PrecedenceAndOperators)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicGrammarElements/PrecedenceAndOperators", Includes=[|"DotNotationAfterGenericMethod01.fs"|])>]
    let ``PrecedenceAndOperators - DotNotationAfterGenericMethod01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicGrammarElements/PrecedenceAndOperators)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicGrammarElements/PrecedenceAndOperators", Includes=[|"checkedOperatorsOverflow.fs"|])>]
    let ``PrecedenceAndOperators - checkedOperatorsOverflow.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

