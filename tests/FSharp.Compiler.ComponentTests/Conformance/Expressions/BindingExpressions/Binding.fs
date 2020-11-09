// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.Expressions.BindingExpressions

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module Binding =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/BindingExpressions/Binding)
    //<Expects status="error" span="(11,5-11,10)" id="FS0020">The result of this expression has type 'bool' and is implicitly ignored\. Consider using 'ignore' to discard this value explicitly, e\.g\. 'expr \|> ignore', or 'let' to bind the result to a name, e\.g\. 'let result = expr'.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/BindingExpressions/Binding", Includes=[|"in01.fs"|])>]
    let ``Binding - in01.fs - --warnaserror+ --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--warnaserror+"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0020
        |> withDiagnosticMessageMatches " ignore', or 'let' to bind the result to a name, e\.g\. 'let result = expr'.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/BindingExpressions/Binding)
    //<Expects status="error" span="(12,5-12,10)" id="FS0020">The result of this expression has type 'bool' and is implicitly ignored\. Consider using 'ignore' to discard this value explicitly, e\.g\. 'expr \|> ignore', or 'let' to bind the result to a name, e\.g\. 'let result = expr'.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/BindingExpressions/Binding", Includes=[|"in02.fs"|])>]
    let ``Binding - in02.fs - --warnaserror+ --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--warnaserror+"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0020
        |> withDiagnosticMessageMatches " ignore', or 'let' to bind the result to a name, e\.g\. 'let result = expr'.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/BindingExpressions/Binding)
    //<Expects status="error" span="(12,5-12,10)" id="FS0020">The result of this expression has type 'bool' and is implicitly ignored\. Consider using 'ignore' to discard this value explicitly, e\.g\. 'expr \|> ignore', or 'let' to bind the result to a name, e\.g\. 'let result = expr'.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/BindingExpressions/Binding", Includes=[|"in03.fs"|])>]
    let ``Binding - in03.fs - --warnaserror+ --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--warnaserror+"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0020
        |> withDiagnosticMessageMatches " ignore', or 'let' to bind the result to a name, e\.g\. 'let result = expr'.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/BindingExpressions/Binding)
    //<Expects status="error" span="(13,5-13,10)" id="FS0020">The result of this expression has type 'bool' and is implicitly ignored\. Consider using 'ignore' to discard this value explicitly, e\.g\. 'expr \|> ignore', or 'let' to bind the result to a name, e\.g\. 'let result = expr'.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/BindingExpressions/Binding", Includes=[|"in04.fs"|])>]
    let ``Binding - in04.fs - --warnaserror+ --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--warnaserror+"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0020
        |> withDiagnosticMessageMatches " ignore', or 'let' to bind the result to a name, e\.g\. 'let result = expr'.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/BindingExpressions/Binding)
    //<Expects status="error" span="(13,5-13,10)" id="FS0020">The result of this expression has type 'bool' and is implicitly ignored\. Consider using 'ignore' to discard this value explicitly, e\.g\. 'expr \|> ignore', or 'let' to bind the result to a name, e\.g\. 'let result = expr'.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/BindingExpressions/Binding", Includes=[|"in05.fs"|])>]
    let ``Binding - in05.fs - --warnaserror+ --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--warnaserror+"; "--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0020
        |> withDiagnosticMessageMatches " ignore', or 'let' to bind the result to a name, e\.g\. 'let result = expr'.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/BindingExpressions/Binding)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/BindingExpressions/Binding", Includes=[|"AmbigLetBinding.fs"|])>]
    let ``Binding - AmbigLetBinding.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/BindingExpressions/Binding)
    //<Expects id="FS0064" span="(7,28-7,29)" status="warning">This construct causes code to be less generic than indicated by the type annotations\. The type variable 'b has been constrained to be type ''a'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/BindingExpressions/Binding", Includes=[|"W_TypeInferforGenericType.fs"|])>]
    let ``Binding - W_TypeInferforGenericType.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0064
        |> withDiagnosticMessageMatches "This construct causes code to be less generic than indicated by the type annotations\. The type variable 'b has been constrained to be type ''a'"

