// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.CompilerOptions.fsc

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module optimize =

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/optimize)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/optimize", Includes=[|"optimize01.fs"|])>]
    let ``optimize - optimize01.fs - --optimize`` compilation =
        compilation
        |> withOptions ["--optimize"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/optimize)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/optimize", Includes=[|"optimize01.fs"|])>]
    let ``optimize - optimize01.fs - --optimize+`` compilation =
        compilation
        |> withOptions ["--optimize+"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/optimize)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/optimize", Includes=[|"optimize01.fs"|])>]
    let ``optimize - optimize01.fs - -O`` compilation =
        compilation
        |> withOptions ["-O"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/optimize)
    //<Expects id="FS0243" status="error">Unrecognized option: '-O\+'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/optimize", Includes=[|"E_optimizeOPlus.fs"|])>]
    let ``optimize - E_optimizeOPlus.fs - -O+`` compilation =
        compilation
        |> withOptions ["-O+"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0243
        |> withDiagnosticMessageMatches "Unrecognized option: '-O\+'"

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/optimize)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/optimize", Includes=[|"optimize01.fs"|])>]
    let ``optimize - optimize01.fs - --optimize-`` compilation =
        compilation
        |> withOptions ["--optimize-"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/optimize)
    //<Expects id="FS0243" status="error">Unrecognized option: '-O-'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/optimize", Includes=[|"E_optimizeOMinus.fs"|])>]
    let ``optimize - E_optimizeOMinus.fs - -O-`` compilation =
        compilation
        |> withOptions ["-O-"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0243
        |> withDiagnosticMessageMatches "Unrecognized option: '-O-'"

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/optimize)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/optimize", Includes=[|"Regressions01.fs"|])>]
    let ``optimize - Regressions01.fs - --debug --optimize-`` compilation =
        compilation
        |> withOptions ["--debug"; "--optimize-"]
        |> typecheck
        |> shouldSucceed

