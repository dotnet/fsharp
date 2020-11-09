// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.CompilerOptions.fsc

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module responsefile =

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/responsefile)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/responsefile", Includes=[|"responsefile01.fs"|])>]
    let ``responsefile - responsefile01.fs - --define:FROM_RESPONSE_FILE_1`` compilation =
        compilation
        |> withOptions ["--define:FROM_RESPONSE_FILE_1"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/responsefile)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/responsefile", Includes=[|"responsefile01.fs"|])>]
    let ``responsefile - responsefile01.fs - \@rs1.rsp`` compilation =
        compilation
        |> withOptions ["\@rs1.rsp"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/responsefile)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/responsefile", Includes=[|"responsefile01.fs"|])>]
    let ``responsefile - responsefile01.fs - \@rs1_multiline_and_comments.rsp`` compilation =
        compilation
        |> withOptions ["\@rs1_multiline_and_comments.rsp"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/responsefile)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/responsefile", Includes=[|"responsefile02.fs"|])>]
    let ``responsefile - responsefile02.fs - \@rs1_multiline_and_comments.rsp`` compilation =
        compilation
        |> withOptions ["\@rs1_multiline_and_comments.rsp"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/responsefile)
    //<Expects id="FS3194" status="error"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/responsefile", Includes=[|"E_responsefile_not_found.fs"|])>]
    let ``responsefile - E_responsefile_not_found.fs - \@not_exists`` compilation =
        compilation
        |> withOptions ["\@not_exists"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3194

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/responsefile)
    //<Expects id="FS3195" status="error"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/responsefile", Includes=[|"E_responsefile_path_invalid.fs"|])>]
    let ``responsefile - E_responsefile_path_invalid.fs - \@`` compilation =
        compilation
        |> withOptions ["\@"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3195

