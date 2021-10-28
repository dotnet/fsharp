// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.CompilerOptions.fsc

open Xunit
open FSharp.Test.Compiler
open FSharp.Test.Xunit.Attributes

module platform =

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/platform)
    //<Expects id="FS3150" status="error">The 'anycpu32bitpreferred' platform can only be used with EXE targets\. You must use 'anycpu' instead\.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/platform", Includes=[|"error_16.fs"|])>]
    let ``platform - error_16.fs - --target:library --platform:anycpu32bitpreferred`` compilation =
        compilation
        |> asFs
        |> withOptions ["--target:library"; "--platform:anycpu32bitpreferred"]
        |> compile
        |> shouldFail
        |> withErrorCode 3150
        |> withDiagnosticMessageMatches "The 'anycpu32bitpreferred' platform can only be used with EXE targets\. You must use 'anycpu' instead\."
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/platform)
    //<Expects id="FS0243" status="error">Unrecognized option: '--PLATFORM'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/platform", Includes=[|"error_01.fs"|])>]
    let ``platform - error_01.fs - --PLATFORM:anycpu`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--PLATFORM:anycpu"]
        |> compile
        |> shouldFail
        |> withErrorCode 0243
        |> withDiagnosticMessageMatches "Unrecognized option: '--PLATFORM'"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/platform)
    //<Expects id="FS0243" status="error">Unrecognized option: '--PlatForm'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/platform", Includes=[|"error_02.fs"|])>]
    let ``platform - error_02.fs - --PlatForm:anycpu`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--PlatForm:anycpu"]
        |> compile
        |> shouldFail
        |> withErrorCode 0243
        |> withDiagnosticMessageMatches "Unrecognized option: '--PlatForm'"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/platform)
    //<Expects id="FS1064" status="error">Unrecognized platform 'ITANIUM', valid values are 'x86', 'x64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/platform", Includes=[|"error_03.fs"|])>]
    let ``platform - error_03.fs - --platform:ITANIUM`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--platform:ITANIUM"]
        |> compile
        |> shouldFail
        |> withErrorCode 1064
        |> withDiagnosticMessageMatches "Unrecognized platform 'ITANIUM', valid values are 'x86', 'x64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/platform)
    //<Expects id="FS1064" status="error">Unrecognized platform 'ANYCPU', valid values are 'x86', 'x64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/platform", Includes=[|"error_04.fs"|])>]
    let ``platform - error_04.fs - --platform:ANYCPU`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--platform:ANYCPU"]
        |> compile
        |> shouldFail
        |> withErrorCode 1064
        |> withDiagnosticMessageMatches "Unrecognized platform 'ANYCPU', valid values are 'x86', 'x64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/platform)
    //<Expects id="FS1064" status="error">Unrecognized platform 'X86', valid values are 'x86', 'x64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/platform", Includes=[|"error_05.fs"|])>]
    let ``platform - error_05.fs - --platform:X86`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--platform:X86"]
        |> compile
        |> shouldFail
        |> withErrorCode 1064
        |> withDiagnosticMessageMatches "Unrecognized platform 'X86', valid values are 'x86', 'x64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/platform)
    //<Expects id="FS1064" status="error">Unrecognized platform 'X64', valid values are 'x86', 'x64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/platform", Includes=[|"error_06.fs"|])>]
    let ``platform - error_06.fs - --platform:X64`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--platform:X64"]
        |> compile
        |> shouldFail
        |> withErrorCode 1064
        |> withDiagnosticMessageMatches "Unrecognized platform 'X64', valid values are 'x86', 'x64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/platform)
    //<Expects id="FS1064" status="error">Unrecognized platform 'IA64', valid values are 'x86', 'x64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/platform", Includes=[|"error_07.fs"|])>]
    let ``platform - error_07.fs - --platform:IA64`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--platform:IA64"]
        |> compile
        |> shouldFail
        |> withErrorCode 1064
        |> withDiagnosticMessageMatches "Unrecognized platform 'IA64', valid values are 'x86', 'x64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/platform)
    //<Expects id="FS1064" status="error">Unrecognized platform 'i386', valid values are 'x86', 'x64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/platform", Includes=[|"error_08.fs"|])>]
    let ``platform - error_08.fs - --platform:i386`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--platform:i386"]
        |> compile
        |> shouldFail
        |> withErrorCode 1064
        |> withDiagnosticMessageMatches "Unrecognized platform 'i386', valid values are 'x86', 'x64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/platform)
    //<Expects id="FS1064" status="error">Unrecognized platform 'AMD64', valid values are 'x86', 'x64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/platform", Includes=[|"error_09.fs"|])>]
    let ``platform - error_09.fs - --platform:AMD64`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--platform:AMD64"]
        |> compile
        |> shouldFail
        |> withErrorCode 1064
        |> withDiagnosticMessageMatches "Unrecognized platform 'AMD64', valid values are 'x86', 'x64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/platform)
    //<Expects id="FS1064" status="error">Unrecognized platform 'PPC', valid values are 'x86', 'x64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/platform", Includes=[|"error_10.fs"|])>]
    let ``platform - error_10.fs - --platform:PPC`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--platform:PPC"]
        |> compile
        |> shouldFail
        |> withErrorCode 1064
        |> withDiagnosticMessageMatches "Unrecognized platform 'PPC', valid values are 'x86', 'x64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/platform)
    //<Expects id="FS1064" status="error">Unrecognized platform 'ARM', valid values are 'x86', 'x64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/platform", Includes=[|"error_15.fs"|])>]
    let ``platform - error_15.fs - --platform:ARM`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--platform:ARM"]
        |> compile
        |> shouldFail
        |> withErrorCode 1064
        |> withDiagnosticMessageMatches "Unrecognized platform 'ARM', valid values are 'x86', 'x64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/platform)
    //<Expects id="FS0243" status="error">Unrecognized option: '--platform-'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/platform", Includes=[|"error_11.fs"|])>]
    let ``platform - error_11.fs - --platform-:anycpu`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--platform-:anycpu"]
        |> compile
        |> shouldFail
        |> withErrorCode 0243
        |> withDiagnosticMessageMatches "Unrecognized option: '--platform-'"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/platform)
    //<Expects id="FS0243" status="error">Unrecognized option: '--PLATFORM\+'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/platform", Includes=[|"error_12.fs"|])>]
    let ``platform - error_12.fs - --PLATFORM+:anycpu`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--PLATFORM+:anycpu"]
        |> compile
        |> shouldFail
        |> withErrorCode 0243
        |> withDiagnosticMessageMatches "Unrecognized option: '--PLATFORM\+'"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/platform)
    //<Expects id="FS0243" status="error">Unrecognized option: '---platform'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/platform", Includes=[|"error_13.fs"|])>]
    let ``platform - error_13.fs - ---platform:anycpu`` compilation =
        compilation
        |> asFsx
        |> withOptions ["---platform:anycpu"]
        |> compile
        |> shouldFail
        |> withErrorCode 0243
        |> withDiagnosticMessageMatches "Unrecognized option: '---platform'"
        |> ignore

