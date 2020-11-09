// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.CompilerOptions.fsc

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module platform =

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/platform)
    //<Expects id="FS3150" status="error">The 'anycpu32bitpreferred' platform can only be used with EXE targets\. You must use 'anycpu' instead\.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/platform", Includes=[|"error_16.fs"|])>]
    let ``platform - error_16.fs - --target:library --platform:anycpu32bitpreferred`` compilation =
        compilation
        |> withOptions ["--target:library"; "--platform:anycpu32bitpreferred"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3150
        |> withDiagnosticMessageMatches "The 'anycpu32bitpreferred' platform can only be used with EXE targets\. You must use 'anycpu' instead\."

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/platform)
    //<Expects id="FS0243" status="error">Unrecognized option: '--PLATFORM'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/platform", Includes=[|"error_01.fs"|])>]
    let ``platform - error_01.fs - --PLATFORM:anycpu`` compilation =
        compilation
        |> withOptions ["--PLATFORM:anycpu"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0243
        |> withDiagnosticMessageMatches "Unrecognized option: '--PLATFORM'"

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/platform)
    //<Expects id="FS0243" status="error">Unrecognized option: '--PlatForm'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/platform", Includes=[|"error_02.fs"|])>]
    let ``platform - error_02.fs - --PlatForm:anycpu`` compilation =
        compilation
        |> withOptions ["--PlatForm:anycpu"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0243
        |> withDiagnosticMessageMatches "Unrecognized option: '--PlatForm'"

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/platform)
    //<Expects id="FS1064" status="error">Unrecognized platform 'ITANIUM', valid values are 'x86', 'x64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/platform", Includes=[|"error_03.fs"|])>]
    let ``platform - error_03.fs - --platform:ITANIUM`` compilation =
        compilation
        |> withOptions ["--platform:ITANIUM"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1064
        |> withDiagnosticMessageMatches "Unrecognized platform 'ITANIUM', valid values are 'x86', 'x64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'"

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/platform)
    //<Expects id="FS1064" status="error">Unrecognized platform 'ANYCPU', valid values are 'x86', 'x64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/platform", Includes=[|"error_04.fs"|])>]
    let ``platform - error_04.fs - --platform:ANYCPU`` compilation =
        compilation
        |> withOptions ["--platform:ANYCPU"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1064
        |> withDiagnosticMessageMatches "Unrecognized platform 'ANYCPU', valid values are 'x86', 'x64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'"

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/platform)
    //<Expects id="FS1064" status="error">Unrecognized platform 'X86', valid values are 'x86', 'x64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/platform", Includes=[|"error_05.fs"|])>]
    let ``platform - error_05.fs - --platform:X86`` compilation =
        compilation
        |> withOptions ["--platform:X86"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1064
        |> withDiagnosticMessageMatches "Unrecognized platform 'X86', valid values are 'x86', 'x64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'"

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/platform)
    //<Expects id="FS1064" status="error">Unrecognized platform 'X64', valid values are 'x86', 'x64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/platform", Includes=[|"error_06.fs"|])>]
    let ``platform - error_06.fs - --platform:X64`` compilation =
        compilation
        |> withOptions ["--platform:X64"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1064
        |> withDiagnosticMessageMatches "Unrecognized platform 'X64', valid values are 'x86', 'x64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'"

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/platform)
    //<Expects id="FS1064" status="error">Unrecognized platform 'IA64', valid values are 'x86', 'x64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/platform", Includes=[|"error_07.fs"|])>]
    let ``platform - error_07.fs - --platform:IA64`` compilation =
        compilation
        |> withOptions ["--platform:IA64"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1064
        |> withDiagnosticMessageMatches "Unrecognized platform 'IA64', valid values are 'x86', 'x64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'"

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/platform)
    //<Expects id="FS1064" status="error">Unrecognized platform 'i386', valid values are 'x86', 'x64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/platform", Includes=[|"error_08.fs"|])>]
    let ``platform - error_08.fs - --platform:i386`` compilation =
        compilation
        |> withOptions ["--platform:i386"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1064
        |> withDiagnosticMessageMatches "Unrecognized platform 'i386', valid values are 'x86', 'x64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'"

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/platform)
    //<Expects id="FS1064" status="error">Unrecognized platform 'AMD64', valid values are 'x86', 'x64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/platform", Includes=[|"error_09.fs"|])>]
    let ``platform - error_09.fs - --platform:AMD64`` compilation =
        compilation
        |> withOptions ["--platform:AMD64"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1064
        |> withDiagnosticMessageMatches "Unrecognized platform 'AMD64', valid values are 'x86', 'x64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'"

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/platform)
    //<Expects id="FS1064" status="error">Unrecognized platform 'PPC', valid values are 'x86', 'x64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/platform", Includes=[|"error_10.fs"|])>]
    let ``platform - error_10.fs - --platform:PPC`` compilation =
        compilation
        |> withOptions ["--platform:PPC"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1064
        |> withDiagnosticMessageMatches "Unrecognized platform 'PPC', valid values are 'x86', 'x64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'"

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/platform)
    //<Expects id="FS1064" status="error">Unrecognized platform 'ARM', valid values are 'x86', 'x64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/platform", Includes=[|"error_15.fs"|])>]
    let ``platform - error_15.fs - --platform:ARM`` compilation =
        compilation
        |> withOptions ["--platform:ARM"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1064
        |> withDiagnosticMessageMatches "Unrecognized platform 'ARM', valid values are 'x86', 'x64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'"

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/platform)
    //<Expects id="FS0243" status="error">Unrecognized option: '--platform-'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/platform", Includes=[|"error_11.fs"|])>]
    let ``platform - error_11.fs - --platform-:anycpu`` compilation =
        compilation
        |> withOptions ["--platform-:anycpu"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0243
        |> withDiagnosticMessageMatches "Unrecognized option: '--platform-'"

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/platform)
    //<Expects id="FS0243" status="error">Unrecognized option: '--PLATFORM\+'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/platform", Includes=[|"error_12.fs"|])>]
    let ``platform - error_12.fs - --PLATFORM+:anycpu`` compilation =
        compilation
        |> withOptions ["--PLATFORM+:anycpu"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0243
        |> withDiagnosticMessageMatches "Unrecognized option: '--PLATFORM\+'"

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/platform)
    //<Expects id="FS0243" status="error">Unrecognized option: '---platform'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/platform", Includes=[|"error_13.fs"|])>]
    let ``platform - error_13.fs - ---platform:anycpu`` compilation =
        compilation
        |> withOptions ["---platform:anycpu"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0243
        |> withDiagnosticMessageMatches "Unrecognized option: '---platform'"

