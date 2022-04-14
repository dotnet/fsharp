// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.Expressions.ApplicationExpressions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module BasicApplication =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ApplicationExpressions/BasicApplication)
    //<Expects status="error" span="(7,17-7,18)" id="FS0010">Unexpected identifier in expression$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_PostfixType01.fs"|])>]
    let ``E_PostfixType01_fs`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 10, Line 7, Col 17, Line 7, Col 18, "Unexpected identifier in expression")
        ]

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ApplicationExpressions/BasicApplication)
    //<Expects status="error" span="(9,31-9,32)" id="FS0010">Unexpected identifier in member definition$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_PostfixType03.fs"|])>]
    let ``E_PostfixType03_fs`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> shouldFail
        |> withDiagnostics [
            (Error 10, Line 9, Col 31, Line 9, Col 32, "Unexpected identifier in member definition")
        ]

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ApplicationExpressions/BasicApplication)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"PostfixType02.fs"|])>]
    let ``PostfixType02_fs`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldSucceed

