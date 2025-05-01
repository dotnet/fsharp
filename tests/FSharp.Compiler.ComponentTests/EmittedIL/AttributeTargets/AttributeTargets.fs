namespace EmittedIL.RealInternalSignature

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module AttributeTargets =

    let verifyCompilation compilation =
        compilation
        |> asFs
        |> withOptions [ "--test:EmitFeeFeeAs100001" ]
        |> withNoOptimize
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> ignoreWarnings
        |> verifyBaseline
        |> verifyILBaseline

    [<Theory; FileInlineData("Default.fs", Realsig=BooleanOptions.Both)>]
    let ``Default_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    [<Theory; FileInlineData("Field.fs", Realsig=BooleanOptions.Both)>]
    let ``Field_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    [<Theory; FileInlineData("Property.fs", Realsig=BooleanOptions.Both)>]
    let ``Property_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation
