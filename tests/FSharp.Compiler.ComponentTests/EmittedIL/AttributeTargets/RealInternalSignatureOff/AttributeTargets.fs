namespace EmittedIL.RealInternalSignature

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module AttributeTargetsRealInternalSignatureOff =

    let verifyCompilation compilation =
        compilation
        |> asFs
        |> withOptions [ "--test:EmitFeeFeeAs100001" ]
        |> withNoOptimize
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> ignoreWarnings
        |> withRealInternalSignatureOff
        |> verifyBaseline
        |> verifyILBaseline

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Default.fs"|])>]
    let ``Default_fs`` compilation =
        verifyCompilation compilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Field.fs"|])>]
    let ``Field_fs`` compilation =
        verifyCompilation compilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Property.fs"|])>]
    let ``Property_fs`` compilation =
        verifyCompilation compilation
