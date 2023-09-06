// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsc

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module platform =

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/platform)
    //<Expects id="FS3150" status="error">The 'anycpu32bitpreferred' platform can only be used with EXE targets\. You must use 'anycpu' instead\.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"error_16.fs"|])>]
    let ``platform - error_16_fs - --target:library --platform:anycpu32bitpreferred`` compilation =
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
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"error_01.fs"|])>]
    let ``platform - error_01_fs - --PLATFORM:anycpu`` compilation =
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
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"error_02.fs"|])>]
    let ``platform - error_02_fs - --PlatForm:anycpu`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--PlatForm:anycpu"]
        |> compile
        |> shouldFail
        |> withErrorCode 0243
        |> withDiagnosticMessageMatches "Unrecognized option: '--PlatForm'"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/platform)
    //<Expects id="FS1064" status="error">Unrecognized platform 'ITANIUM', valid values are 'x86', 'x64', 'Arm', 'Arm64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"error_03.fs"|])>]
    let ``platform - error_03_fs - --platform:ITANIUM`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--platform:ITANIUM"]
        |> compile
        |> shouldFail
        |> withErrorCode 1064
        |> withDiagnosticMessageMatches "Unrecognized platform 'ITANIUM', valid values are 'x86', 'x64', 'Arm', 'Arm64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/platform)
    //<Expects id="FS1064" status="error">Unrecognized platform 'ANYCPU', valid values are 'x86', 'x64', 'Arm', 'Arm64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"error_04.fs"|])>]
    let ``platform - error_04_fs - --platform:ANYCPU`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--platform:ANYCPU"]
        |> compile
        |> shouldFail
        |> withErrorCode 1064
        |> withDiagnosticMessageMatches "Unrecognized platform 'ANYCPU', valid values are 'x86', 'x64', 'Arm', 'Arm64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/platform)
    //<Expects id="FS1064" status="error">Unrecognized platform 'X86', valid values are 'x86', 'x64', 'Arm', 'Arm64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"error_05.fs"|])>]
    let ``platform - error_05_fs - --platform:X86`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--platform:X86"]
        |> compile
        |> shouldFail
        |> withErrorCode 1064
        |> withDiagnosticMessageMatches "Unrecognized platform 'X86', valid values are 'x86', 'x64', 'Arm', 'Arm64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/platform)
    //<Expects id="FS1064" status="error">Unrecognized platform 'X64', valid values are 'x86', 'x64', 'Arm', 'Arm64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"error_06.fs"|])>]
    let ``platform - error_06_fs - --platform:X64`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--platform:X64"]
        |> compile
        |> shouldFail
        |> withErrorCode 1064
        |> withDiagnosticMessageMatches "Unrecognized platform 'X64', valid values are 'x86', 'x64', 'Arm', 'Arm64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/platform)
    //<Expects id="FS1064" status="error">Unrecognized platform 'IA64', valid values are 'x86', 'x64', 'Arm', 'Arm64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"error_07.fs"|])>]
    let ``platform - error_07_fs - --platform:IA64`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--platform:IA64"]
        |> compile
        |> shouldFail
        |> withErrorCode 1064
        |> withDiagnosticMessageMatches "Unrecognized platform 'IA64', valid values are 'x86', 'x64', 'Arm', 'Arm64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/platform)
    //<Expects id="FS1064" status="error">Unrecognized platform 'i386', valid values are 'x86', 'x64', 'Arm', 'Arm64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"error_08.fs"|])>]
    let ``platform - error_08_fs - --platform:i386`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--platform:i386"]
        |> compile
        |> shouldFail
        |> withErrorCode 1064
        |> withDiagnosticMessageMatches "Unrecognized platform 'i386', valid values are 'x86', 'x64', 'Arm', 'Arm64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/platform)
    //<Expects id="FS1064" status="error">Unrecognized platform 'AMD64', valid values are 'x86', 'x64', 'Arm', 'Arm64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"error_09.fs"|])>]
    let ``platform - error_09_fs - --platform:AMD64`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--platform:AMD64"]
        |> compile
        |> shouldFail
        |> withErrorCode 1064
        |> withDiagnosticMessageMatches "Unrecognized platform 'AMD64', valid values are 'x86', 'x64', 'Arm', 'Arm64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/platform)
    //<Expects id="FS1064" status="error">Unrecognized platform 'PPC', valid values are 'x86', 'x64', 'Arm', 'Arm64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"error_10.fs"|])>]
    let ``platform - error_10_fs - --platform:PPC`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--platform:PPC"]
        |> compile
        |> shouldFail
        |> withErrorCode 1064
        |> withDiagnosticMessageMatches "Unrecognized platform 'PPC', valid values are 'x86', 'x64', 'Arm', 'Arm64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/platform)
    //<Expects id="FS1064" status="error">Unrecognized platform 'ARM', valid values are 'x86', 'x64', 'Arm', 'Arm64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"error_15.fs"|])>]
    let ``platform - error_15_fs - --platform:ARM`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--platform:ARM"]
        |> compile
        |> shouldFail
        |> withErrorCode 1064
        |> withDiagnosticMessageMatches "Unrecognized platform 'ARM', valid values are 'x86', 'x64', 'Arm', 'Arm64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/platform)
    //<Expects id="FS0243" status="error">Unrecognized option: '--platform-'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"error_11.fs"|])>]
    let ``platform - error_11_fs - --platform-:anycpu`` compilation =
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
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"error_12.fs"|])>]
    let ``platform - error_12_fs - --PLATFORM+:anycpu`` compilation =
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
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"error_13.fs"|])>]
    let ``platform - error_13_fs - ---platform:anycpu`` compilation =
        compilation
        |> asFsx
        |> withOptions ["---platform:anycpu"]
        |> compile
        |> shouldFail
        |> withErrorCode 0243
        |> withDiagnosticMessageMatches "Unrecognized option: '---platform'"
        |> ignore

