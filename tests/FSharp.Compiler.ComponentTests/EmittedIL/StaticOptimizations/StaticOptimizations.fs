namespace EmittedIL

open FSharp.Test
open FSharp.Test.Compiler
open Xunit

module StaticOptimizations =
    let verifyCompilation compilation =
        compilation
        |> asExe
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> ignoreWarnings
        |> verifyILBaseline

    [<Theory; FileInlineData("String_Enum.fs", Realsig = BooleanOptions.True, Optimize = BooleanOptions.True)>]
    let String_Enum_fs compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    [<Theory; FileInlineData("String_SignedIntegralTypes.fs", Realsig = BooleanOptions.True, Optimize = BooleanOptions.True)>]
    let String_SignedIntegralTypes_fs compilation =
        compilation
        |> getCompilation
        |> verifyCompilation
