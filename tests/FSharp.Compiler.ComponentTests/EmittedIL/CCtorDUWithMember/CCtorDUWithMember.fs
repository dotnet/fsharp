namespace FSharp.Compiler.ComponentTests.EmittedIL

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module CCtorDUWithMember =

    let verifyCompilation compilation =
        compilation
        |> withOptions [ "--test:EmitFeeFeeAs100001" ]
        |> asExe
        |> withNoOptimize
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> ignoreWarnings
        |> verifyBaseline
        |> verifyILBaseline

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"CCtorDUWithMember01a.fs"|])>]
    let ``CCtorDUWithMember01a_fs`` compilation =
        compilation
        |> asFs
        |> withAdditionalSourceFile (SourceFromPath (__SOURCE_DIRECTORY__ ++ "CCtorDUWithMember01.fs"))
        |> verifyCompilation 

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"CCtorDUWithMember02a.fs"|])>]
    let ``CCtorDUWithMember02a_fs`` compilation =
        compilation
        |> asFs
        |> withAdditionalSourceFile (SourceFromPath (__SOURCE_DIRECTORY__ ++ "CCtorDUWithMember02.fs"))
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"CCtorDUWithMember03a.fs"|])>]
    let ``CCtorDUWithMember03a_fs`` compilation =
        compilation
        |> asFs
        |> withAdditionalSourceFile (SourceFromPath (__SOURCE_DIRECTORY__ ++ "CCtorDUWithMember03.fs"))
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[| "CCtorDUWithMember04a.fs" |])>]
    let ``CCtorDUWithMember04a_fs`` compilation =
        compilation
        |> asFs
        |> withAdditionalSourceFile (SourceFromPath (__SOURCE_DIRECTORY__ ++ "CCtorDUWithMember04.fs"))
        |> verifyCompilation 
