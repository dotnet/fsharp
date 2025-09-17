module EmittedIL.NominalRecordExpressionSpreads

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

[<Theory; FileInlineData("Expression_Nominal_ExplicitShadowsSpread.fs", Realsig = BooleanOptions.True, Optimize = BooleanOptions.True)>]
let Expression_Nominal_ExplicitShadowsSpread_fs compilation =
    compilation
    |> getCompilation
    |> verifyCompilation

[<Theory; FileInlineData("Expression_Nominal_ExtraFieldsAreIgnored.fs", Realsig = BooleanOptions.True, Optimize = BooleanOptions.True)>]
let Expression_Nominal_ExtraFieldsAreIgnored_fs compilation =
    compilation
    |> getCompilation
    |> verifyCompilation

[<Theory; FileInlineData("Expression_Nominal_NoOverlap_Explicit_Spread.fs", Realsig = BooleanOptions.True, Optimize = BooleanOptions.True)>]
let Expression_Nominal_NoOverlap_Explicit_Spread_fs compilation =
    compilation
    |> getCompilation
    |> verifyCompilation

[<Theory; FileInlineData("Expression_Nominal_NoOverlap_Spread_Explicit.fs", Realsig = BooleanOptions.True, Optimize = BooleanOptions.True)>]
let Expression_Nominal_NoOverlap_Spread_Explicit_fs compilation =
    compilation
    |> getCompilation
    |> verifyCompilation

[<Theory; FileInlineData("Expression_Nominal_NoOverlap_Spread_Spread.fs", Realsig = BooleanOptions.True, Optimize = BooleanOptions.True)>]
let Expression_Nominal_NoOverlap_Spread_Spread_fs compilation =
    compilation
    |> getCompilation
    |> verifyCompilation

[<Theory; FileInlineData("Expression_Nominal_NoOverlap_SpreadFromAnon.fs", Realsig = BooleanOptions.True, Optimize = BooleanOptions.True)>]
let Expression_Nominal_NoOverlap_SpreadFromAnon_fs compilation =
    compilation
    |> getCompilation
    |> verifyCompilation

[<Theory; FileInlineData("Expression_Nominal_SpreadShadowsExplicit.fs", Realsig = BooleanOptions.True, Optimize = BooleanOptions.True)>]
let Expression_Nominal_SpreadShadowsExplicit_fs compilation =
    compilation
    |> getCompilation
    |> verifyCompilation

[<Theory; FileInlineData("Expression_Nominal_SpreadShadowsSpread.fs", Realsig = BooleanOptions.True, Optimize = BooleanOptions.True)>]
let Expression_Nominal_SpreadShadowsSpread_fs compilation =
    compilation
    |> getCompilation
    |> verifyCompilation
