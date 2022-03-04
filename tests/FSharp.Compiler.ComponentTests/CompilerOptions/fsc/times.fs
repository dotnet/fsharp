// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.CompilerOptions.fsc

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module times =

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/times)
    //<Expects id="FS0243" status="error">Unrecognized option: '--Times'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/times", Includes=[|"error_01.fs"|])>]
    let ``times - error_01.fs - --Times`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--Times"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0243
        |> withDiagnosticMessageMatches "Unrecognized option: '--Times'"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/times)
    //<Expects id="FS0243" status="error">Unrecognized option: '--times-'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/times", Includes=[|"error_02.fs"|])>]
    let ``times - error_02.fs - --times-`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--times-"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0243
        |> withDiagnosticMessageMatches "Unrecognized option: '--times-'"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/times)
    //<Expects id="FS0243" status="error">Unrecognized option: '--times\+'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/times", Includes=[|"error_03.fs"|])>]
    let ``times - error_03.fs - --times+`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--times+"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0243
        |> withDiagnosticMessageMatches "Unrecognized option: '--times\+'"
        |> ignore

