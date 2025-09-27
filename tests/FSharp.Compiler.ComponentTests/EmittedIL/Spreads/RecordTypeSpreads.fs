module EmittedIL.RecordTypeSpreads

open FSharp.Test
open FSharp.Test.Compiler
open Xunit

let [<Literal>] SupportedLangVersion = "preview"

let verifyCompilation compilation =
    compilation
    |> withLangVersion SupportedLangVersion
    |> asExe
    |> withEmbeddedPdb
    |> withEmbedAllSource
    |> ignoreWarnings
    |> compile
    |> verifyILBaseline

[<Theory; FileInlineData("Type_ExplicitShadowsSpread.fs", Realsig = BooleanOptions.True, Optimize = BooleanOptions.True)>]
let Type_ExplicitShadowsSpread_fs compilation =
    compilation
    |> getCompilation
    |> verifyCompilation

[<Theory; FileInlineData("Type_NoOverlap_Explicit_Spread.fs", Realsig = BooleanOptions.True, Optimize = BooleanOptions.True)>]
let Type_NoOverlap_Explicit_Spread_fs compilation =
    compilation
    |> getCompilation
    |> verifyCompilation

[<Theory; FileInlineData("Type_NoOverlap_Spread_Explicit.fs", Realsig = BooleanOptions.True, Optimize = BooleanOptions.True)>]
let Type_NoOverlap_Spread_Explicit_fs compilation =
    compilation
    |> getCompilation
    |> verifyCompilation

[<Theory; FileInlineData("Type_NoOverlap_Spread_Spread.fs", Realsig = BooleanOptions.True, Optimize = BooleanOptions.True)>]
let Type_NoOverlap_Spread_Spread_fs compilation =
    compilation
    |> getCompilation
    |> verifyCompilation

[<Theory; FileInlineData("Type_NoOverlap_SpreadFromAnon.fs", Realsig = BooleanOptions.True, Optimize = BooleanOptions.True)>]
let Type_NoOverlap_SpreadFromAnon_fs compilation =
    compilation
    |> getCompilation
    |> verifyCompilation

[<Theory; FileInlineData("Type_SpreadShadowsSpread.fs", Realsig = BooleanOptions.True, Optimize = BooleanOptions.True)>]
let Type_SpreadShadowsSpread_fs compilation =
    compilation
    |> getCompilation
    |> verifyCompilation

[<Theory; FileInlineData("Type_SpreadShadowsExplicit.fs", Realsig = BooleanOptions.True, Optimize = BooleanOptions.True)>]
let Type_SpreadShadowsExplicit_fs compilation =
    compilation
    |> getCompilation
    |> verifyCompilation
