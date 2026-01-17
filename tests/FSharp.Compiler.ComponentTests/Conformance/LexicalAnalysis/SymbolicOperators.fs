// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.LexicalAnalysis

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module SymbolicOperators =

    // SOURCE: LessThanDotOpenParen001.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicOperators", Includes=[|"LessThanDotOpenParen001.fs"|])>]
    let ``SymbolicOperators - LessThanDotOpenParen001_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: QMarkSimple.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicOperators", Includes=[|"QMarkSimple.fs"|])>]
    let ``SymbolicOperators - QMarkSimple_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: QMarkNested.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicOperators", Includes=[|"QMarkNested.fs"|])>]
    let ``SymbolicOperators - QMarkNested_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: QMarkArguments.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicOperators", Includes=[|"QMarkArguments.fs"|])>]
    let ``SymbolicOperators - QMarkArguments_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: QMarkAssignSimple.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicOperators", Includes=[|"QMarkAssignSimple.fs"|])>]
    let ``SymbolicOperators - QMarkAssignSimple_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: E_QMarkGeneric.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicOperators", Includes=[|"E_QMarkGeneric.fs"|])>]
    let ``SymbolicOperators - E_QMarkGeneric_fs`` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0717
        |> ignore

    // SOURCE: QMarkPrecedenceSpace.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicOperators", Includes=[|"QMarkPrecedenceSpace.fs"|])>]
    let ``SymbolicOperators - QMarkPrecedenceSpace_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: QMarkPrecedenceArray.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicOperators", Includes=[|"QMarkPrecedenceArray.fs"|])>]
    let ``SymbolicOperators - QMarkPrecedenceArray_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: QMarkPrecedenceMethodCall.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicOperators", Includes=[|"QMarkPrecedenceMethodCall.fs"|])>]
    let ``SymbolicOperators - QMarkPrecedenceMethodCall_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: QMarkPrecedenceMethodCallSpace.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicOperators", Includes=[|"QMarkPrecedenceMethodCallSpace.fs"|])>]
    let ``SymbolicOperators - QMarkPrecedenceMethodCallSpace_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: QMarkPrecedenceInArrays.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicOperators", Includes=[|"QMarkPrecedenceInArrays.fs"|])>]
    let ``SymbolicOperators - QMarkPrecedenceInArrays_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: QMarkPrecedenceCurrying.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicOperators", Includes=[|"QMarkPrecedenceCurrying.fs"|])>]
    let ``SymbolicOperators - QMarkPrecedenceCurrying_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: QMarkExpressionAsArgument.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicOperators", Includes=[|"QMarkExpressionAsArgument.fs"|])>]
    let ``SymbolicOperators - QMarkExpressionAsArgument_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: QMarkExpressionAsArgument2.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicOperators", Includes=[|"QMarkExpressionAsArgument2.fs"|])>]
    let ``SymbolicOperators - QMarkExpressionAsArgument2_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: E_CantUseDollarSign.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicOperators", Includes=[|"E_CantUseDollarSign.fs"|])>]
    let ``SymbolicOperators - E_CantUseDollarSign_fs`` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0035
        |> ignore
