namespace FSharp.Compiler.ComponentTests.EmittedIL

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Platform =

    let verifyPlatformedExe compilation =
        compilation
        |> asExe
        |> withName "platformedExe.exe"
        |> PEVerifier.verifyPEFile

    let verifyPlatformedDll compilation =
        compilation
        |> asLibrary
        |> withName "platformedDll.dll"
        |> PEVerifier.verifyPEFile

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"PlatformedExeBody.fs"|])>]
    let platformExeAnyCpuDefault compilation =
        compilation
        |> verifyPlatformedExe
        |> PEVerifier.shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"PlatformedExeBody.fs"|])>]
    let platformExeAnyCpu32BitPreferred compilation =
        compilation
        |> withPlatform ExecutionPlatform.AnyCpu32bitPreferred
        |> verifyPlatformedExe
        |> PEVerifier.shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"PlatformedExeBody.fs"|])>]
    let platformExeArm compilation =
        compilation
        |> withPlatform ExecutionPlatform.Arm
        |> verifyPlatformedExe
        |> PEVerifier.shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"PlatformedExeBody.fs"|])>]
    let platformExeArm64 compilation =
        compilation
        |> withPlatform ExecutionPlatform.Arm64
        |> verifyPlatformedExe
        |> PEVerifier.shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"PlatformedExeBody.fs"|])>]
    let platformExeItanium compilation =
        compilation
        |> withPlatform ExecutionPlatform.Itanium
        |> verifyPlatformedExe
        |> PEVerifier.shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"PlatformedExeBody.fs"|])>]
    let platformExeX86 compilation =
        compilation
        |> withPlatform ExecutionPlatform.X86
        |> verifyPlatformedExe
        |> PEVerifier.shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"PlatformedExeBody.fs"|])>]
    let platformExeX64 compilation =
        compilation
        |> withPlatform ExecutionPlatform.X64
        |> verifyPlatformedExe
        |> PEVerifier.shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"PlatformedDllBody.fs"|])>]
    let platformDllDefault compilation =
        compilation
        |> verifyPlatformedExe
        |> PEVerifier.shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"PlatformedDllBody.fs"|])>]
    let platformDllAnyCpuDefault compilation =
        compilation
        |> withPlatform ExecutionPlatform.Anycpu
        |> verifyPlatformedExe
        |> PEVerifier.shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"PlatformedDllBody.fs"|])>]
    let platformDllAnyCpu32BitPreferred compilation =
        compilation
        |> withPlatform ExecutionPlatform.AnyCpu32bitPreferred
        |> verifyPlatformedExe
        |> PEVerifier.shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"PlatformedDllBody.fs"|])>]
    let platformDllArm compilation =
        compilation
        |> withPlatform ExecutionPlatform.Arm
        |> verifyPlatformedExe
        |> PEVerifier.shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"PlatformedDllBody.fs"|])>]
    let platformDllArm64 compilation =
        compilation
        |> withPlatform ExecutionPlatform.Arm64
        |> verifyPlatformedExe
        |> PEVerifier.shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"PlatformedDllBody.fs"|])>]
    let platformDllItanium compilation =
        compilation
        |> withPlatform ExecutionPlatform.Itanium
        |> verifyPlatformedExe
        |> PEVerifier.shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"PlatformedDllBody.fs"|])>]
    let platformDllX86 compilation =
        compilation
        |> withPlatform ExecutionPlatform.X86
        |> verifyPlatformedExe
        |> PEVerifier.shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"PlatformedDllBody.fs"|])>]
    let platformDllX64 compilation =
        compilation
        |> withPlatform ExecutionPlatform.X64
        |> verifyPlatformedExe
        |> PEVerifier.shouldSucceed
