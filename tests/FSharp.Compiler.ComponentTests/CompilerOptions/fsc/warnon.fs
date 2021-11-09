// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.CompilerOptions.fsc

open Xunit
open FSharp.Test.Compiler
open FSharp.Test.Xunit.Attributes

module warnon =

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/warnon)
    //<Expects status="warning" span="(18,11-18,12)" id="FS1182">The value 'n' is unused$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/warnon", Includes=[|"warnon01.fs"|])>]
    let ``warnon - warnon01.fs - --warnon:1182 --test:ErrorRanges`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--warnon:1182"; "--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withWarningCode 1182
        |> withDiagnosticMessageMatches "The value 'n' is unused$"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/warnon)
    //<Expects status="warning" span="(18,11-18,12)" id="FS1182">The value 'n' is unused$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/warnon", Includes=[|"warnon01.fs"|])>]
    let ``warnon - warnon01.fs - --warnon:NU0001;FS1182;NU0001 --test:ErrorRanges`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--warnon:NU0001;FS1182;NU0001"; "--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withWarningCode 1182
        |> withDiagnosticMessageMatches "The value 'n' is unused$"
        |> ignore

