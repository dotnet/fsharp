namespace EmittedIL

open FSharp.Test
open FSharp.Test.Compiler
open Xunit

module ComputedCollections =
    let verifyCompilation compilation =
        compilation
        |> asExe
        |> withLangVersionPreview // TODO https://github.com/dotnet/fsharp/issues/16739: Remove this when LanguageFeature.LowerIntegralRangesToFastLoops is out of preview.
        |> withOptimize
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> ignoreWarnings
        |> verifyILBaseline

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Int32RangeArrays.fs"|])>]
    let ``Int32RangeArrays_fs`` compilation =
        compilation
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Int32RangeLists.fs"|])>]
    let ``Int32RangeLists_fs`` compilation =
        compilation
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"UInt64RangeArrays.fs"|])>]
    let ``UInt64RangeArrays_fs`` compilation =
        compilation
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"UInt64RangeLists.fs"|])>]
    let ``UInt64RangeLists_fs`` compilation =
        compilation
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ForNInRangeArrays.fs"|])>]
    let ``ForNInRangeArrays_fs`` compilation =
        compilation
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ForNInRangeLists.fs"|])>]
    let ``ForNInRangeLists_fs`` compilation =
        compilation
        |> verifyCompilation
