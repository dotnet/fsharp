// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.CompilerOptions.fsc

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module cliversion =

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/cliversion)
    //<Expects id="FS0243" status="error">Unrecognized option: '--cliversion'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/cliversion", Includes=[|"E_fsc_cliversion.fs"|])>]
    let ``cliversion - E_fsc_cliversion.fs - --cliversion:2.0`` compilation =
        compilation
        |> withOptions ["--cliversion:2.0"]
        |> compile
        |> shouldFail
        |> withErrorCode 0243
        |> withDiagnosticMessageMatches "Unrecognized option: '--cliversion'"

