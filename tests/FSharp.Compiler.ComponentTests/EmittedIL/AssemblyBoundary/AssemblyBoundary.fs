namespace FSharp.Compiler.ComponentTests.EmittedIL

open Xunit
open System.IO
open FSharp.Test
open FSharp.Test.Compiler

module AssemblyBoundary =

    let verifyCompileAndExecution compilation =
        compilation
        |> asFs
        |> withOptions [ "--test:EmitFeeFeeAs100001" ]
        |> withOptimize
        |> compileExeAndRun
        |> shouldSucceed


    //NoMT SOURCE=test01.fs SCFLAGS="--optimize+ -r:lib01.dll" PRECMD="\$FSC_PIPE -a --optimize+ lib01.fs" # test01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"test01.fs"|])>]
    let ``test01_fs`` compilation =
        let lib01 =
            FsFromPath (Path.Combine(__SOURCE_DIRECTORY__,  "lib01.fs"))
            |> withOptimize
            |> asLibrary

        compilation
        |> withReferences [lib01]
        |> verifyCompileAndExecution

    //NoMT SOURCE=test01.fs SCFLAGS="--optimize+ -r:lib01.dll" PRECMD="\$FSC_PIPE -a --optimize+ lib01.fs" # test01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"test02.fs"|])>]
    let ``test02_fs`` compilation =
        let lib02 =
            FsFromPath (Path.Combine(__SOURCE_DIRECTORY__,  "lib02.fs"))
            |> withOptimize
            |> asLibrary

        compilation
        |> withReferences [lib02]
        |> verifyCompileAndExecution

    //NoMT SOURCE=test03.fs SCFLAGS="--optimize+ -r:lib03.dll" PRECMD="\$FSC_PIPE -a --optimize+ lib03.fs" # test03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"test03.fs"|])>]
    let ``test03_fs`` compilation =
        let lib03 =
            FsFromPath (Path.Combine(__SOURCE_DIRECTORY__,  "lib03.fs"))
            |> withOptimize
            |> asLibrary

        compilation
        |> withReferences [lib03]
        |> verifyCompileAndExecution

    //NoMT SOURCE=test04.fs SCFLAGS="--optimize+ -r:lib04.dll" PRECMD="\$FSC_PIPE -a --optimize+ lib04.fs" # test04.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"test04.fs"|])>]
    let ``test04_fs`` compilation =
        let lib04 =
            FsFromPath (Path.Combine(__SOURCE_DIRECTORY__,  "lib04.fs"))
            |> withOptimize
            |> asLibrary

        compilation
        |> withReferences [lib04]
        |> verifyCompileAndExecution


    // SOURCE=InlineWithPrivateValues01.fs SCFLAGS="-r:TypeLib01.dll" PRECMD="\$FSC_PIPE -a --optimize+ TypeLib01.fs" # InlineWithPrivateValuesStruct
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"InlineWithPrivateValues01.fs"|])>]
    let ``InlineWithPrivateValues01_fs_TypeLib01_fs`` compilation =
        let typeLib01 =
            FsFromPath (Path.Combine(__SOURCE_DIRECTORY__,  "TypeLib01.fs"))
            |> withOptimize
            |> asLibrary

        compilation
        |> withReferences [typeLib01]
        |> verifyCompileAndExecution

    // SOURCE=InlineWithPrivateValues01.fs SCFLAGS="-r:TypeLib02.dll" PRECMD="\$FSC_PIPE -a --optimize+ TypeLib02.fs" # InlineWithPrivateValuesRef
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"InlineWithPrivateValues01.fs"|])>]
    let ``InlineWithPrivateValues01_fs_TypeLib02_fs`` compilation =
        let typeLib02 =
            FsFromPath (Path.Combine(__SOURCE_DIRECTORY__,  "TypeLib02.fs"))
            |> withOptimize
            |> asLibrary

        compilation
        |> withReferences [typeLib02]
        |> verifyCompileAndExecution
