// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.CompilerOptions.fsc

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module Checked =

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/checked)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/checked", Includes=[|"unchecked01.fs"|])>]
    let ``Checked - unchecked01.fs - `` compilation =
        compilation
        |> compile
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/checked)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/checked", Includes=[|"checked01.fs"|])>]
    let ``Checked - checked01.fs - --checked`` compilation =
        compilation
        |> withOptions ["--checked"]
        |> compile
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/checked)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/checked", Includes=[|"checked01.fs"|])>]
    let ``Checked - checked01.fs - --checked+`` compilation =
        compilation
        |> withOptions ["--checked+"]
        |> compile
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/checked)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/checked", Includes=[|"unchecked01.fs"|])>]
    let ``Checked - unchecked01.fs - --checked-`` compilation =
        compilation
        |> withOptions ["--checked-"]
        |> compile
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/checked)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/checked", Includes=[|"checked01.fs"|])>]
    let ``Checked - checked01.fs - --checked --checked+`` compilation =
        compilation
        |> withOptions ["--checked"; "--checked+"]
        |> compile
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/checked)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/checked", Includes=[|"checked01.fs"|])>]
    let ``Checked - checked01.fs - --checked- --checked+`` compilation =
        compilation
        |> withOptions ["--checked-"; "--checked+"]
        |> compile
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/checked)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/checked", Includes=[|"unchecked01.fs"|])>]
    let ``Checked - unchecked01.fs - --checked+ --checked-`` compilation =
        compilation
        |> withOptions ["--checked+"; "--checked-"]
        |> compile
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/checked)
    //<Expects id="FS0243" status="error">Unrecognized option: .+</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/checked", Includes=[|"unrecogarg.fs"|])>]
    let ``Checked - unrecogarg.fs - --Checked`` compilation =
        compilation
        |> withOptions ["--Checked"]
        |> compile
        |> shouldFail
        |> withErrorCode 0243
        |> withDiagnosticMessageMatches "Unrecognized option: .+"

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/checked)
    //<Expects id="FS0243" status="error">Unrecognized option: .+</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/checked", Includes=[|"unrecogarg.fs"|])>]
    let ``Checked - unrecogarg.fs - --checked*`` compilation =
        compilation
        |> withOptions ["--checked*"]
        |> compile
        |> shouldFail
        |> withErrorCode 0243
        |> withDiagnosticMessageMatches "Unrecognized option: .+"

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/checked)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/checked", Includes=[|"UncheckedDefaultOf01.fs"|])>]
    let ``Checked - UncheckedDefaultOf01.fs - `` compilation =
        compilation
        |> compile
        |> shouldSucceed

