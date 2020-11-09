// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.CompilerOptions.fsc

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module warnaserror =

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/warnaserror)
    //<Expects status="warning" id="FS0988"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/warnaserror", Includes=[|"t1.fs"|])>]
    let ``warnaserror - t1.fs - --warnaserror+ --warnaserror-:FS25,FS26,FS988`` compilation =
        compilation
        |> withOptions ["--warnaserror+"; "--warnaserror-:FS25,FS26,FS988"]
        |> typecheck
        |> shouldFail

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/warnaserror)
    //<Expects status="warning" id="FS0988"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/warnaserror", Includes=[|"t1.fs"|])>]
    let ``warnaserror - t1.fs - --warnaserror+ --warnaserror-:25,26,988`` compilation =
        compilation
        |> withOptions ["--warnaserror+"; "--warnaserror-:25,26,988"]
        |> typecheck
        |> shouldFail

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/warnaserror)
    //<Expects status="error" id="FS0988"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/warnaserror", Includes=[|"t2.fs"|])>]
    let ``warnaserror - t2.fs - --warnaserror+ --warnaserror-:25,26`` compilation =
        compilation
        |> withOptions ["--warnaserror+"; "--warnaserror-:25,26"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0988

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/warnaserror)
    //<Expects status="error" id="FS0026"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/warnaserror", Includes=[|"t3.fs"|])>]
    let ``warnaserror - t3.fs - --warnaserror+ --warnaserror-:25`` compilation =
        compilation
        |> withOptions ["--warnaserror+"; "--warnaserror-:25"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0026

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/warnaserror)
    //<Expects status="error" id="FS0026"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/warnaserror", Includes=[|"t4.fs"|])>]
    let ``warnaserror - t4.fs - --warnaserror+ --warnaserror+:25,26,988`` compilation =
        compilation
        |> withOptions ["--warnaserror+"; "--warnaserror+:25,26,988"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0026

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/warnaserror)
    //<Expects status="error" id="FS0026"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/warnaserror", Includes=[|"t5.fs"|])>]
    let ``warnaserror - t5.fs - --warnaserror+ --warnaserror+:25,26`` compilation =
        compilation
        |> withOptions ["--warnaserror+"; "--warnaserror+:25,26"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0026

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/warnaserror)
    //<Expects status="error" id="FS0026"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/warnaserror", Includes=[|"t6.fs"|])>]
    let ``warnaserror - t6.fs - --warnaserror+ --warnaserror+:25`` compilation =
        compilation
        |> withOptions ["--warnaserror+"; "--warnaserror+:25"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0026

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/warnaserror)
    //<Expects status="warning" id="FS0988"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/warnaserror", Includes=[|"t7.fs"|])>]
    let ``warnaserror - t7.fs - --warnaserror- --warnaserror-:25,26,988`` compilation =
        compilation
        |> withOptions ["--warnaserror-"; "--warnaserror-:25,26,988"]
        |> typecheck
        |> shouldFail

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/warnaserror)
    //<Expects status="warning" id="FS0988"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/warnaserror", Includes=[|"t8.fs"|])>]
    let ``warnaserror - t8.fs - --warnaserror- --warnaserror-:25,26`` compilation =
        compilation
        |> withOptions ["--warnaserror-"; "--warnaserror-:25,26"]
        |> typecheck
        |> shouldFail

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/warnaserror)
    //<Expects status="warning" id="FS0988"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/warnaserror", Includes=[|"t9.fs"|])>]
    let ``warnaserror - t9.fs - --warnaserror- --warnaserror-:25`` compilation =
        compilation
        |> withOptions ["--warnaserror-"; "--warnaserror-:25"]
        |> typecheck
        |> shouldFail

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/warnaserror)
    //<Expects status="error" id="FS0026"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/warnaserror", Includes=[|"t10.fs"|])>]
    let ``warnaserror - t10.fs - --warnaserror- --warnaserror+:25,26,988`` compilation =
        compilation
        |> withOptions ["--warnaserror-"; "--warnaserror+:25,26,988"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0026

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/warnaserror)
    //<Expects status="error" id="FS0026"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/warnaserror", Includes=[|"t11.fs"|])>]
    let ``warnaserror - t11.fs - --warnaserror- --warnaserror+:25,26`` compilation =
        compilation
        |> withOptions ["--warnaserror-"; "--warnaserror+:25,26"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0026

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/warnaserror)
    //<Expects status="warning" id="FS0026"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/warnaserror", Includes=[|"t12.fs"|])>]
    let ``warnaserror - t12.fs - --warnaserror- --warnaserror+:25`` compilation =
        compilation
        |> withOptions ["--warnaserror-"; "--warnaserror+:25"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0026

