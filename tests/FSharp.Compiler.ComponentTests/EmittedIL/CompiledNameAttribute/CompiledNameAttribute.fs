namespace EmittedIL.RealInternalSignature

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

    [<Theory; FileInlineData("CompiledNameAttribute01.fs")>]
    let ``CompiledNameAttribute01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    [<Theory; FileInlineData("CompiledNameAttribute02.fs")>]
    let ``CompiledNameAttribute02_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    [<Theory; FileInlineData("CompiledNameAttribute03.fs")>]
    let ``CompiledNameAttribute03_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    [<Theory; FileInlineData("CompiledNameAttribute04.fs", Realsig=BooleanOptions.Both)>]
    let ``CompiledNameAttribute04_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation 

    [<Theory; FileInlineData("CompiledNameAttribute05.fs")>]
    let ``CompiledNameAttribute05_fs`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> asExe
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> withNoWarn 988
        |> compile
        |> shouldSucceed
        |> ignore
