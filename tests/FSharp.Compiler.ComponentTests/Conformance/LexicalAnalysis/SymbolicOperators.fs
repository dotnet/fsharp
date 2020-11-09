// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.LexicalAnalysis

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module SymbolicOperators =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/SymbolicOperators)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicOperators", Includes=[|"GreaterThanDotParen01.fs"|])>]
    let ``SymbolicOperators - GreaterThanDotParen01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/SymbolicOperators)
    //<Expects status="error" span="(13,7)" id="FS1208">Invalid operator definition\. Prefix operator definitions must use a valid prefix operator name\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicOperators", Includes=[|"E_GreaterThanDotParen01.fs"|])>]
    let ``SymbolicOperators - E_GreaterThanDotParen01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 1208
        |> withDiagnosticMessageMatches "Invalid operator definition\. Prefix operator definitions must use a valid prefix operator name\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/SymbolicOperators)
    //<Expects status="error" id="FS0670">This code is not sufficiently generic\. The type variable  \^T when  \^T : \(static member \( \+ \) :  \^T \*  \^T ->  \^a\) could not be generalized because it would escape its scope</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicOperators", Includes=[|"E_LessThanDotOpenParen001.fs"|])>]
    let ``SymbolicOperators - E_LessThanDotOpenParen001.fs - --flaterrors`` compilation =
        compilation
        |> withOptions ["--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0670
        |> withDiagnosticMessageMatches "  \^a\) could not be generalized because it would escape its scope"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/SymbolicOperators)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicOperators", Includes=[|"LessThanDotOpenParen001.fs"|])>]
    let ``SymbolicOperators - LessThanDotOpenParen001.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/SymbolicOperators)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicOperators", Includes=[|"GreaterThanColon001.fs"|])>]
    let ``SymbolicOperators - GreaterThanColon001.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/SymbolicOperators)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicOperators", Includes=[|"E_GreaterThanColon002.fs"|])>]
    let ``SymbolicOperators - E_GreaterThanColon002.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/SymbolicOperators)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicOperators", Includes=[|"QMarkSimple.fs"|])>]
    let ``SymbolicOperators - QMarkSimple.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/SymbolicOperators)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicOperators", Includes=[|"QMarkNested.fs"|])>]
    let ``SymbolicOperators - QMarkNested.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/SymbolicOperators)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicOperators", Includes=[|"QMarkArguments.fs"|])>]
    let ``SymbolicOperators - QMarkArguments.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/SymbolicOperators)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicOperators", Includes=[|"QMarkAssignSimple.fs"|])>]
    let ``SymbolicOperators - QMarkAssignSimple.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/SymbolicOperators)
    //<Expects id="FS0717" status="error">Unexpected type arguments</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicOperators", Includes=[|"E_QMarkGeneric.fs"|])>]
    let ``SymbolicOperators - E_QMarkGeneric.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0717
        |> withDiagnosticMessageMatches "Unexpected type arguments"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/SymbolicOperators)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicOperators", Includes=[|"QMarkPrecedenceSpace.fs"|])>]
    let ``SymbolicOperators - QMarkPrecedenceSpace.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/SymbolicOperators)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicOperators", Includes=[|"QMarkPrecedenceArray.fs"|])>]
    let ``SymbolicOperators - QMarkPrecedenceArray.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/SymbolicOperators)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicOperators", Includes=[|"QMarkPrecedenceMethodCall.fs"|])>]
    let ``SymbolicOperators - QMarkPrecedenceMethodCall.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/SymbolicOperators)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicOperators", Includes=[|"QMarkPrecedenceMethodCallSpace.fs"|])>]
    let ``SymbolicOperators - QMarkPrecedenceMethodCallSpace.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/SymbolicOperators)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicOperators", Includes=[|"QMarkPrecedenceInArrays.fs"|])>]
    let ``SymbolicOperators - QMarkPrecedenceInArrays.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/SymbolicOperators)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicOperators", Includes=[|"QMarkPrecedenceCurrying.fs"|])>]
    let ``SymbolicOperators - QMarkPrecedenceCurrying.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/SymbolicOperators)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicOperators", Includes=[|"QMarkExpressionAsArgument.fs"|])>]
    let ``SymbolicOperators - QMarkExpressionAsArgument.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/SymbolicOperators)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicOperators", Includes=[|"QMarkExpressionAsArgument2.fs"|])>]
    let ``SymbolicOperators - QMarkExpressionAsArgument2.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

