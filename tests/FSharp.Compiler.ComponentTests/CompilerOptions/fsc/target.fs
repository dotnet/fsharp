// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.CompilerOptions.fsc

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module target =

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/target)
    //<Expects id="FS0243" status="error">Unrecognized option: '--a'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/target", Includes=[|"error01.fs"|])>]
    let ``target - error01.fs - --a`` compilation =
        compilation
        |> withOptions ["--a"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0243
        |> withDiagnosticMessageMatches "Unrecognized option: '--a'"

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/target)
    //<Expects status="error" id="FS0226">The file extension of '//a' is not recognized\. Source files must have extension \.fs, \.fsi, \.fsx, \.fsscript, \.ml or \.mli\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/target", Includes=[|"error02.fs"|])>]
    let ``target - error02.fs - //a`` compilation =
        compilation
        |> withOptions ["//a"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0226
        |> withDiagnosticMessageMatches "The file extension of '//a' is not recognized\. Source files must have extension \.fs, \.fsi, \.fsx, \.fsscript, \.ml or \.mli\.$"

