// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.BasicGrammarElements

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module PrecedenceAndOperators =


    // SOURCE=checkedOperatorsNoOverflow.fs                             # checkedOperatorsNoOverflow.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"checkedOperatorsNoOverflow.fs"|])>]
    let ``checkedOperatorsNoOverflow_fs`` compilation =
        compilation
        |> asExe
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> compileExeAndRun
        |> shouldSucceed

    // SOURCE=checkedOperatorsOverflow.fs                               # checkedOperatorsOverflow.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"checkedOperatorsOverflow.fs"|])>]
    let ``checkedOperatorsOverflow_fs`` compilation =
        compilation
        |> asExe
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> compileExeAndRun
        |> shouldSucceed

    // SOURCE=DotNotationAfterGenericMethod01.fs                        # DotNotationAfterGenericMethod01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"DotNotationAfterGenericMethod01.fs"|])>]
    let ``DotNotationAfterGenericMethod01_fs`` compilation =
        compilation
        |> asExe
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> compileExeAndRun
        |> shouldSucceed

    // SOURCE=E_ExclamationMark01.fs   SCFLAGS="--test:ErrorRanges -a"  # E_ExclamationMark01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_ExclamationMark01.fs"|])>]
    let ``E_ExclamationMark01_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3, Line 14, Col 11, Line 14, Col 20, "This value is not a function and cannot be applied.")
            (Error 3, Line 15, Col 12, Line 15, Col 21, "This value is not a function and cannot be applied.")
        ]

    // SOURCE=E_Negation01.fs          SCFLAGS="--test:ErrorRanges -a"  # E_Negation01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_Negation01.fs"|])>]
    let ``E_Negation01_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 10, Col 31, Line 10, Col 32, "The type 'byte' does not support the operator '~-'")
            (Error 1, Line 11, Col 31, Line 11, Col 32, "The type 'uint16' does not support the operator '~-'")
            (Error 1, Line 12, Col 31, Line 12, Col 32, "The type 'uint32' does not support the operator '~-'")
            (Error 1, Line 13, Col 31, Line 13, Col 32, "The type 'uint64' does not support the operator '~-'")
        ]

    // SOURCE=E_QuestionMark01.fs      SCFLAGS="--test:ErrorRanges -a"  # E_QuestionMark01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_QuestionMark01.fs"|])>]
    let ``E_QuestionMark01_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1208, Line 12, Col 6, Line 12, Col 9, "Invalid operator definition. Prefix operator definitions must use a valid prefix operator name.")
            (Error 1208, Line 13, Col 6, Line 13, Col 10, "Invalid operator definition. Prefix operator definitions must use a valid prefix operator name.")
            (Error 1208, Line 15, Col 25, Line 15, Col 26, "Invalid prefix operator")
            (Error 1208, Line 16, Col 27, Line 16, Col 28, "Invalid prefix operator")
        ]

    // SOURCE=E_QuestionMark02.fs      SCFLAGS="--test:ErrorRanges -a"  # E_QuestionMark02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_QuestionMark02.fs"|])>]
    let ``E_QuestionMark02_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1208, Line 11, Col 6, Line 11, Col 10, "Invalid operator definition. Prefix operator definitions must use a valid prefix operator name.")
            (Error 1208, Line 13, Col 15, Line 13, Col 22, "Invalid prefix operator")
            (Error 1208, Line 14, Col 16, Line 14, Col 17, "Invalid prefix operator")
        ]

    // SOURCE=E_Tilde01.fs             SCFLAGS="--test:ErrorRanges -a"  # E_Tilde01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_Tilde01.fs"|])>]
    let ``E_Tilde01_01_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1208, Line 14, Col 6, Line 14, Col 9, "Invalid operator definition. Prefix operator definitions must use a valid prefix operator name.")
            (Error 1208, Line 15, Col 6, Line 15, Col 10, "Invalid operator definition. Prefix operator definitions must use a valid prefix operator name.")
            (Error 1208, Line 17, Col 25, Line 17, Col 26, "Invalid prefix operator")
            (Error 1208, Line 18, Col 27, Line 18, Col 28, "Invalid prefix operator")
        ]

    // SOURCE=E_Tilde02.fs             SCFLAGS="--test:ErrorRanges -a"  # E_Tilde02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_Tilde02.fs"|])>]
    let ``E_Tilde02_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1208, Line 12, Col 6, Line 12, Col 9, "Invalid operator definition. Prefix operator definitions must use a valid prefix operator name.")
            (Error 1208, Line 13, Col 6, Line 13, Col 10, "Invalid operator definition. Prefix operator definitions must use a valid prefix operator name.")
            (Error 1208, Line 15, Col 15, Line 15, Col 22, "Invalid prefix operator")
            (Error 1208, Line 16, Col 16, Line 16, Col 17, "Invalid prefix operator")
        ]

    // SOURCE=ExclamationMark02.fs                                      # ExclamationMark02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ExclamationMark02.fs"|])>]
    let ``ExclamationMark02_fs`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--nowarn:44"]
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE=Negation01.fs                                             # Negation01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Negation01.fs"|])>]
    let ``Negation01_fs`` compilation =
        compilation
        |> asExe
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> compileExeAndRun
        |> shouldSucceed

    // SOURCE=VerticalbarOptionalinDU.fs                                # VerticalbarOptionalinDU.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"VerticalbarOptionalinDU.fs"|])>]
    let ``VerticalbarOptionalinDU_fs`` compilation =
        compilation
        |> asExe
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> compileExeAndRun
        |> shouldSucceed
