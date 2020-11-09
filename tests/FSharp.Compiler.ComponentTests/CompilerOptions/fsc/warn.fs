// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.CompilerOptions.fsc

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module warn =

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/warn)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/warn", Includes=[|"warning_level_0.fs"|])>]
    let ``warn - warning_level_0.fs - --warn:0 --warnaserror`` compilation =
        compilation
        |> withOptions ["--warn:0"; "--warnaserror"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/warn)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/warn", Includes=[|"warn0_level5.fs"|])>]
    let ``warn - warn0_level5.fs - --warn:0 --warnaserror`` compilation =
        compilation
        |> withOptions ["--warn:0"; "--warnaserror"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/warn)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/warn", Includes=[|"warn1_level5.fs"|])>]
    let ``warn - warn1_level5.fs - --warn:1 --warnaserror`` compilation =
        compilation
        |> withOptions ["--warn:1"; "--warnaserror"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/warn)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/warn", Includes=[|"warn2_level5.fs"|])>]
    let ``warn - warn2_level5.fs - --warn:2 --warnaserror`` compilation =
        compilation
        |> withOptions ["--warn:2"; "--warnaserror"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/warn)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/warn", Includes=[|"warn3_level5.fs"|])>]
    let ``warn - warn3_level5.fs - --warn:3 --warnaserror`` compilation =
        compilation
        |> withOptions ["--warn:3"; "--warnaserror"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/warn)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/warn", Includes=[|"warn4_level5.fs"|])>]
    let ``warn - warn4_level5.fs - --warn:4 --warnaserror`` compilation =
        compilation
        |> withOptions ["--warn:4"; "--warnaserror"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/warn)
    //<Expects status="error" span="(11,14)" id="FS0052">The value has been copied to ensure the original is not mutated by this operation or because the copy is implicit when returning a struct from a member and another member is then accessed$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/warn", Includes=[|"warn5_level5.fs"|])>]
    let ``warn - warn5_level5.fs - --warn:5 --warnaserror`` compilation =
        compilation
        |> withOptions ["--warn:5"; "--warnaserror"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0052
        |> withDiagnosticMessageMatches "The value has been copied to ensure the original is not mutated by this operation or because the copy is implicit when returning a struct from a member and another member is then accessed$"

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/warn)
    //<Expects status="warning" span="(11,14)" id="FS0052">The value has been copied to ensure the original is not mutated by this operation or because the copy is implicit when returning a struct from a member and another member is then accessed$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/warn", Includes=[|"warn5_level5w.fs"|])>]
    let ``warn - warn5_level5w.fs - --warn:5`` compilation =
        compilation
        |> withOptions ["--warn:5"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0052
        |> withDiagnosticMessageMatches "The value has been copied to ensure the original is not mutated by this operation or because the copy is implicit when returning a struct from a member and another member is then accessed$"

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/warn)
    //<Expects id="FS1050" status="error">Invalid warning level '6'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/warn", Includes=[|"invalid_warning_level_6.fs"|])>]
    let ``warn - invalid_warning_level_6.fs - --warn:6`` compilation =
        compilation
        |> withOptions ["--warn:6"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1050
        |> withDiagnosticMessageMatches "Invalid warning level '6'"

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/warn)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/warn", Includes=[|"nowarn.fs"|])>]
    let ``warn - nowarn.fs - --warnaserror`` compilation =
        compilation
        |> withOptions ["--warnaserror"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/warn)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/warn", Includes=[|"warn40.fs"|])>]
    let ``warn - warn40.fs - --nowarn:40`` compilation =
        compilation
        |> withOptions ["--nowarn:40"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/warn)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/warn", Includes=[|"warn40.fs"|])>]
    let ``warn - warn40.fs - --nowarn:NU0000;FS40;NU0001`` compilation =
        compilation
        |> withOptions ["--nowarn:NU0000;FS40;NU0001"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/warn)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/warn", Includes=[|"warn40.fs"|])>]
    let ``warn - warn40.fs - --nowarn:FS0040`` compilation =
        compilation
        |> withOptions ["--nowarn:FS0040"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/warn)
    //<Expects id="FS0040" span="(6,48)" status="error">This and other recursive references to the object\(s\) being defined will be checked for initialization-soundness at runtime through the use of a delayed reference\. This is because you are defining one or more recursive objects, rather than recursive functions\. This warning may be suppressed by using '#nowarn "40"' or '--nowarn:40'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/warn", Includes=[|"nowarn_with_warnaserror01.fs"|])>]
    let ``warn - nowarn_with_warnaserror01.fs - --warnaserror --warn:4`` compilation =
        compilation
        |> withOptions ["--warnaserror"; "--warn:4"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0040
        |> withDiagnosticMessageMatches "This and other recursive references to the object\(s\) being defined will be checked for initialization-soundness at runtime through the use of a delayed reference\. This is because you are defining one or more recursive objects, rather than recursive functions\. This warning may be suppressed by using '#nowarn \"40\"' or '--nowarn:40'\.$"

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/warn)
    //<Expects id="FS0040" span="(7,48)" status="error">This and other recursive references to the object\(s\) being defined will be checked for initialization-soundness at runtime through the use of a delayed reference\. This is because you are defining one or more recursive objects, rather than recursive functions\. This warning may be suppressed by using '#nowarn "40"' or '--nowarn:40'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/warn", Includes=[|"nowarn_with_warnaserror02.fs"|])>]
    let ``warn - nowarn_with_warnaserror02.fs - --warnaserror --warn:4`` compilation =
        compilation
        |> withOptions ["--warnaserror"; "--warn:4"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0040
        |> withDiagnosticMessageMatches "This and other recursive references to the object\(s\) being defined will be checked for initialization-soundness at runtime through the use of a delayed reference\. This is because you are defining one or more recursive objects, rather than recursive functions\. This warning may be suppressed by using '#nowarn \"40\"' or '--nowarn:40'"

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/warn)
    //<Expects status="notin">FS0021</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/warn", Includes=[|"nowarn_with_warnaserror03.fs"|])>]
    let ``warn - nowarn_with_warnaserror03.fs - --warnaserror --warn:4`` compilation =
        compilation
        |> withOptions ["--warnaserror"; "--warn:4"]
        |> typecheck
        |> withDiagnosticMessageMatches "FS0021"

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/warn)
    //<Expects id="FS0040" span="(6,48)" status="error">This and other recursive references to the object\(s\) being defined will be checked for initialization-soundness at runtime through the use of a delayed reference\. This is because you are defining one or more recursive objects, rather than recursive functions\. This warning may be suppressed by using '#nowarn "40"' or '--nowarn:40'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/warn", Includes=[|"nowarn_with_warnaserror01.fs"|])>]
    let ``warn - nowarn_with_warnaserror01.fs - --warnaserror:FS0040 --warn:4`` compilation =
        compilation
        |> withOptions ["--warnaserror:FS0040"; "--warn:4"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0040
        |> withDiagnosticMessageMatches "This and other recursive references to the object\(s\) being defined will be checked for initialization-soundness at runtime through the use of a delayed reference\. This is because you are defining one or more recursive objects, rather than recursive functions\. This warning may be suppressed by using '#nowarn \"40\"' or '--nowarn:40'\.$"

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/warn)
    //<Expects id="FS0040" span="(7,48)" status="error">This and other recursive references to the object\(s\) being defined will be checked for initialization-soundness at runtime through the use of a delayed reference\. This is because you are defining one or more recursive objects, rather than recursive functions\. This warning may be suppressed by using '#nowarn "40"' or '--nowarn:40'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/warn", Includes=[|"nowarn_with_warnaserror02.fs"|])>]
    let ``warn - nowarn_with_warnaserror02.fs - --warnaserror:FS0040 --warn:4`` compilation =
        compilation
        |> withOptions ["--warnaserror:FS0040"; "--warn:4"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0040
        |> withDiagnosticMessageMatches "This and other recursive references to the object\(s\) being defined will be checked for initialization-soundness at runtime through the use of a delayed reference\. This is because you are defining one or more recursive objects, rather than recursive functions\. This warning may be suppressed by using '#nowarn \"40\"' or '--nowarn:40'"

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/warn)
    //<Expects status="notin">FS0021</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/warn", Includes=[|"nowarn_with_warnaserror03.fs"|])>]
    let ``warn - nowarn_with_warnaserror03.fs - --warnaserror:FS0040 --warn:4`` compilation =
        compilation
        |> withOptions ["--warnaserror:FS0040"; "--warn:4"]
        |> typecheck
        |> withDiagnosticMessageMatches "FS0021"

