namespace FSharp.Compiler.ComponentTests.EmittedIL

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module CompiledNameAttribute =

    let verifyCompilation compilation =
        compilation
        |> asFs
        |> withOptions [ "--test:EmitFeeFeeAs100001" ]
        |> asExe
        |> withNoOptimize
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> ignoreWarnings
        |> verifyBaseline
        |> verifyILBaseline

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"CompiledNameAttribute01.fs"|])>]
    let ``CompiledNameAttribute01_fs`` compilation =
        compilation
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"CompiledNameAttribute02.fs"|])>]
    let ``CompiledNameAttribute02_fs`` compilation =
        compilation
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"CompiledNameAttribute03.fs"|])>]
    let ``CompiledNameAttribute03_fs`` compilation =
        compilation
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"CompiledNameAttribute04.fs"|])>]
    let ``CompiledNameAttribute04_fs`` compilation =
        compilation
        |> verifyCompilation 

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"CompiledNameAttribute05.fs"|])>]
    let ``CompiledNameAttribute05_fs`` compilation =
        compilation
        |> asFs
        |> asExe
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> withNoWarn 988
        |> compile
        |> shouldSucceed
        |> ignore
