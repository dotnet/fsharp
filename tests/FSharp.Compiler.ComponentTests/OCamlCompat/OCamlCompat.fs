// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.OCamlCompat

open Xunit
open FSharp.Test.Compiler
open FSharp.Test.Xunit.Attributes

module OCamlCompat =

    // This test was automatically generated (moved from FSharpQA suite - OCamlCompat)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/OCamlCompat", Includes=[|"IndentOff02.fs"|])>]
    let ``OCamlCompat - IndentOff02.fs - --warnaserror --mlcompatibility`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--warnaserror"; "--mlcompatibility"]
        |> typecheck
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - OCamlCompat)
    //<Expects status="warning" span="(4,1-4,14)" id="FS0062">This construct is for ML compatibility\. Consider using a file with extension '\.ml' or '\.mli' instead\. You can disable this warning by using '--mlcompatibility' or '--nowarn:62'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/OCamlCompat", Includes=[|"W_IndentOff03.fs"|])>]
    let ``OCamlCompat - W_IndentOff03.fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withWarningCode 0062
        |> withDiagnosticMessageMatches "This construct is for ML compatibility\. Consider using a file with extension '\.ml' or '\.mli' instead\. You can disable this warning by using '--mlcompatibility' or '--nowarn:62'\.$"
        |> ignore

