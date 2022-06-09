namespace FSharp.Compiler.ComponentTests.EmittedIL

open Xunit
open FSharp.Test
open FSharp.Test.Compiler
open System.IO
open System.Runtime.InteropServices

module Platform =

    let assemblyHasMvidSection =
        CsFromPath (Path.Combine(__SOURCE_DIRECTORY__, "AssemblyHasMvidSection.fs"))
        |> withName "AssemblyHasMvidSection"

    let isArm =
        match System.Runtime.InteropServices.Architecture() with
        | Architecture.Arm | Architecture.Arm64 -> true
        | _ -> false

    let buildPlatformedDll =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PlatformedDll.fs")
        |> asLibrary
        |> withName "PlatformedDll.dll"

    let buildPlatformedExe =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "PlatformedExe.fs")
        |> asExe
        |> withName "PlatformedExe.exe"

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AssemblyNameForExe.fs"|])>]
    let platformExeAnyCpuDefault compilation =
        compilation
        |> withReferences [ buildPlatformedExe |> withPlatform ExecutionPlatform.Anycpu ]
        |> compileExeAndRun
        |> shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AssemblyNameForExe.fs"|])>]
    let platformExeAnyCpu32BitPreferred compilation =
        compilation
        |> asExe
        |> withReferences [ buildPlatformedExe |> withPlatform ExecutionPlatform.AnyCpu32bitPreferred ]
        |> compileExeAndRun
        |> shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AssemblyNameForExe.fs"|])>]
    let platformExeArm compilation =
        compilation
        |> asExe
        |> withReferences [ buildPlatformedExe |> withPlatform ExecutionPlatform.Arm ]
        |>  if isArm then compileExeAndRun else compile
        |> shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AssemblyNameForExe.fs"|])>]
    let platformExeArm64 compilation =
        compilation
        |> asExe
        |> withPlatform ExecutionPlatform.Arm64
        |> withReferences [ buildPlatformedExe |> withPlatform ExecutionPlatform.Arm64 ]
        |>  if isArm then compileExeAndRun else compile
        |> shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AssemblyNameForExe.fs"|])>]
    let platformExeItanium compilation =
        compilation
        |> asExe
        |> withReferences [ buildPlatformedExe |> withPlatform ExecutionPlatform.Itanium ]
        |> compileExeAndRun
        |> shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AssemblyNameForExe.fs"|])>]
    let platformExeX86 compilation =
        compilation
        |> asExe
        |> withReferences [ buildPlatformedExe |> withPlatform ExecutionPlatform.X86 ]
        |> compileExeAndRun
        |> shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AssemblyNameForExe.fs"|])>]
    let platformExeX64 compilation =
        compilation
        |> asExe
        |> withReferences [ buildPlatformedExe |> withPlatform ExecutionPlatform.X64 ]
        |> compileExeAndRun
        |> shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AssemblyNameForDll.fs"|])>]
    let platformDllDefault compilation =
        compilation
        |> asExe
        |> withReferences [ buildPlatformedDll ]
        |> compileExeAndRun
        |> shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AssemblyNameForDll.fs"|])>]
    let platformDllAnyCpuDefault compilation =
        compilation
        |> asExe
        |> withReferences [ buildPlatformedDll |> withPlatform ExecutionPlatform.Anycpu ]
        |> compileExeAndRun
        |> shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AssemblyNameForDll.fs"|])>]
    let platformDllArm compilation =
        compilation
        |> asExe
        |> withReferences [ buildPlatformedDll |> withPlatform ExecutionPlatform.Arm ]
        |>  if isArm then compileExeAndRun else compile
        |> shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AssemblyNameForDll.fs"|])>]
    let platformDllArm64 compilation =
        compilation
        |> asExe
        |> withPlatform ExecutionPlatform.Arm64
        |> withReferences [ buildPlatformedDll |> withPlatform ExecutionPlatform.Arm64 ]
        |>  if isArm then compileExeAndRun else compile
        |> shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AssemblyNameForDll.fs"|])>]
    let platformDllItanium compilation =
        compilation
        |> asExe
        |> withReferences [ buildPlatformedDll |> withPlatform ExecutionPlatform.Itanium ]
        |> compileExeAndRun
        |> shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AssemblyNameForDll.fs"|])>]
    let platformDllX86 compilation =
        compilation
        |> asExe
        |> withReferences [ buildPlatformedDll |> withPlatform ExecutionPlatform.X86 ]
        |> compileExeAndRun
        |> shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AssemblyNameForDll.fs"|])>]
    let platformDllX64 compilation =
        compilation
        |> asExe
        |> withReferences [ buildPlatformedDll |> withPlatform ExecutionPlatform.X64 ]
        |> compileExeAndRun
        |> shouldSucceed

    //[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AssemblyHasMvidSection.fs"|])>]
    //let generatedExeHasMvidSection compilation =
    //    compilation
    //    |> asExe
    //    |> withReferences [assemblyHasMvidSection]
    //    |> withRefOut "test"
    //    |> compileExeAndRun
    //    |> shouldSucceed
