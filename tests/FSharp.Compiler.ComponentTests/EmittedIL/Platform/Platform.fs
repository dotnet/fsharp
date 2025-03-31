namespace EmittedIL

open Xunit
open FSharp.Test
open FSharp.Test.Compiler
open System.IO
open System.Runtime.InteropServices

module Platform =

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

    [<Theory; FileInlineData("AssemblyNameForExe.fs")>]
    let platformExeAnyCpuDefault compilation =
        compilation
        |> getCompilation
        |> withReferences [ buildPlatformedExe |> withPlatform ExecutionPlatform.Anycpu ]
        |> compileExeAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("AssemblyNameForExe.fs")>]
    let platformExeAnyCpu32BitPreferred compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withReferences [ buildPlatformedExe |> withPlatform ExecutionPlatform.AnyCpu32bitPreferred ]
        |> compileExeAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("AssemblyNameForExe.fs")>]
    let platformExeArm compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withReferences [ buildPlatformedExe |> withPlatform ExecutionPlatform.Arm ]
        |>  if isArm then compileExeAndRun else compile
        |> shouldSucceed

    [<Theory; FileInlineData("AssemblyNameForExe.fs")>]
    let platformExeArm64 compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withPlatform ExecutionPlatform.Arm64
        |> withReferences [ buildPlatformedExe |> withPlatform ExecutionPlatform.Arm64 ]
        |>  if isArm then compileExeAndRun else compile
        |> shouldSucceed

    [<Theory; FileInlineData("AssemblyNameForExe.fs")>]
    let platformExeItanium compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withReferences [ buildPlatformedExe |> withPlatform ExecutionPlatform.Itanium ]
        |> compileExeAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("AssemblyNameForExe.fs")>]
    let platformExeX86 compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withReferences [ buildPlatformedExe |> withPlatform ExecutionPlatform.X86 ]
        |> compileExeAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("AssemblyNameForExe.fs")>]
    let platformExeX64 compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withReferences [ buildPlatformedExe |> withPlatform ExecutionPlatform.X64 ]
        |> compileExeAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("AssemblyNameForDll.fs")>]
    let platformDllDefault compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withReferences [ buildPlatformedDll ]
        |> compileExeAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("AssemblyNameForDll.fs")>]
    let platformDllAnyCpuDefault compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withReferences [ buildPlatformedDll |> withPlatform ExecutionPlatform.Anycpu ]
        |> compileExeAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("AssemblyNameForDll.fs")>]
    let platformDllArm compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withReferences [ buildPlatformedDll |> withPlatform ExecutionPlatform.Arm ]
        |>  if isArm then compileExeAndRun else compile
        |> shouldSucceed

    [<Theory; FileInlineData("AssemblyNameForDll.fs")>]
    let platformDllArm64 compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withPlatform ExecutionPlatform.Arm64
        |> withReferences [ buildPlatformedDll |> withPlatform ExecutionPlatform.Arm64 ]
        |>  if isArm then compileExeAndRun else compile
        |> shouldSucceed

    [<Theory; FileInlineData("AssemblyNameForDll.fs")>]
    let platformDllItanium compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withReferences [ buildPlatformedDll |> withPlatform ExecutionPlatform.Itanium ]
        |> compileExeAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("AssemblyNameForDll.fs")>]
    let platformDllX86 compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withReferences [ buildPlatformedDll |> withPlatform ExecutionPlatform.X86 ]
        |> compileExeAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("AssemblyNameForDll.fs")>]
    let platformDllX64 compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withReferences [ buildPlatformedDll |> withPlatform ExecutionPlatform.X64 ]
        |> compileExeAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("AssemblyHasMvidSection.fs")>]
    let withRefOnlyGeneratesMvidSection compilation =

        let mvidReader =
            CsFromPath (Path.Combine(__SOURCE_DIRECTORY__, "MvidReader.cs"))
            |> withName "MvidReader"

        let assemblyHasMvidSection =
            FsFromPath (Path.Combine(__SOURCE_DIRECTORY__, "SimpleFsProgram.fs"))
            |> asLibrary
            |> withOptions ["--test:DumpSignatureData"]
            |> withRefOnly

        compilation
        |> getCompilation
        |> asExe
        |> withReferences [mvidReader]
        |> withReferences [assemblyHasMvidSection]
        |> withOptions ["--test:DumpSignatureData"]
        |> compileExeAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("AssemblyHasMvidSection.fs")>]
    let withoutRefOnlyGeneratesNoMvidSection compilation =

        let mvidReader =
            CsFromPath (Path.Combine(__SOURCE_DIRECTORY__, "MvidReader.cs"))
            |> withName "MvidReader"

        let assemblyHasMvidSection =
            FsFromPath (Path.Combine(__SOURCE_DIRECTORY__, "SimpleFsProgram.fs"))
            |> asLibrary

        compilation
        |> getCompilation
        |> asExe
        |> withReferences [mvidReader]
        |> withReferences [assemblyHasMvidSection]
        |> compileExeAndRun
        |> shouldFail
