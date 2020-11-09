// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.LexicalFiltering

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module HashLight =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalFiltering/HashLight)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalFiltering/HashLight", Includes=[|"IndentationWithComputationExpression01.fs"|])>]
    let ``HashLight - IndentationWithComputationExpression01.fs - --warnaserror+`` compilation =
        compilation
        |> withOptions ["--warnaserror+"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalFiltering/HashLight)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalFiltering/HashLight", Includes=[|"MissingEndToken01.fs"|])>]
    let ``HashLight - MissingEndToken01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalFiltering/HashLight)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalFiltering/HashLight", Includes=[|"OffsideAccessibility01.fs"|])>]
    let ``HashLight - OffsideAccessibility01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalFiltering/HashLight)
    //<Expects status="warning" span="(20,5-20,6)" id="FS0058">Possible incorrect indentation: this token is offside of context started at position \(17:5\)\. Try indenting this token further or using standard formatting conventions\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalFiltering/HashLight", Includes=[|"W_OffsideAccessibility01.fs"|])>]
    let ``HashLight - W_OffsideAccessibility01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0058
        |> withDiagnosticMessageMatches "Possible incorrect indentation: this token is offside of context started at position \(17:5\)\. Try indenting this token further or using standard formatting conventions\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalFiltering/HashLight)
    //<Expects id="FS0003" status="error">This value is not a function and cannot be applied</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalFiltering/HashLight", Includes=[|"indent_off_01.fs"|])>]
    let ``HashLight - indent_off_01.fs - --warnaserror --mlcompatibility`` compilation =
        compilation
        |> withOptions ["--warnaserror"; "--mlcompatibility"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0003
        |> withDiagnosticMessageMatches "This value is not a function and cannot be applied"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalFiltering/HashLight)
    //<Expects id="FS0988" status="warning" span="(12,1)">Main module of program is empty</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalFiltering/HashLight", Includes=[|"indent_off_01.fsscript"|])>]
    let ``HashLight - indent_off_01.fsscript - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withDiagnosticMessageMatches "Main module of program is empty"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalFiltering/HashLight)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalFiltering/HashLight", Includes=[|"indent_off_after_comment01.fs"|])>]
    let ``HashLight - indent_off_after_comment01.fs - -a --warnaserror --mlcompatibility`` compilation =
        compilation
        |> withOptions ["-a"; "--warnaserror"; "--mlcompatibility"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalFiltering/HashLight)
    //<Expects status="warning" span="(6,1)" id="FS0988">Main module of program is empty: nothing will happen when it is run</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalFiltering/HashLight", Includes=[|"First_Non_Comment_Text01.fs"|])>]
    let ``HashLight - First_Non_Comment_Text01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withDiagnosticMessageMatches "Main module of program is empty: nothing will happen when it is run"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalFiltering/HashLight)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalFiltering/HashLight", Includes=[|"light_off_01.fs"|])>]
    let ``HashLight - light_off_01.fs - --warnaserror --mlcompatibility`` compilation =
        compilation
        |> withOptions ["--warnaserror"; "--mlcompatibility"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalFiltering/HashLight)
    //<Expects id="FS0588" status="error" span="(10,5)">The block following this 'let' is unfinished. Every code block is an expression and must have a result. 'let' cannot be the final code element in a block. Consider giving this block an explicit result.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalFiltering/HashLight", Includes=[|"E_UnclosedLetBlock01.fs"|])>]
    let ``HashLight - E_UnclosedLetBlock01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0588
        |> withDiagnosticMessageMatches "The block following this 'let' is unfinished. Every code block is an expression and must have a result. 'let' cannot be the final code element in a block. Consider giving this block an explicit result."

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalFiltering/HashLight)
    //<Expects status="error" span="(9,5)" id="FS3118">Incomplete value or function definition\. If this is in an expression, the body of the expression must be indented to the same column as the 'let' keyword\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalFiltering/HashLight", Includes=[|"E_SpacesAfterLetBinding.fs"|])>]
    let ``HashLight - E_SpacesAfterLetBinding.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 3118
        |> withDiagnosticMessageMatches "Incomplete value or function definition\. If this is in an expression, the body of the expression must be indented to the same column as the 'let' keyword\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalFiltering/HashLight)
    //<Expects status="error" id="FS1161" span="(5,1-5,2)">TABs are not allowed in F# code unless the #indent "off" option is used</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalFiltering/HashLight", Includes=[|"E_TABsNotAllowedIndentOff.fs"|])>]
    let ``HashLight - E_TABsNotAllowedIndentOff.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1161
        |> withDiagnosticMessageMatches "TABs are not allowed in F# code unless the #indent \"off\" option is used"

