namespace EmittedIL

open FSharp.Test
open FSharp.Test.Compiler
open Xunit

module ComputedCollections =
    let verifyCompilation compilation =
        compilation
        |> asExe
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> ignoreWarnings
        |> verifyILBaseline

    [<Theory; FileInlineData("Int32RangeArrays.fs", Realsig = BooleanOptions.Both, Optimize = BooleanOptions.True)>]
    let Int32RangeArrays_fs compilation = 
        compilation
        |> getCompilation
        |> verifyCompilation

    [<Theory; FileInlineData("Int32RangeLists.fs", Realsig = BooleanOptions.Both, Optimize = BooleanOptions.True)>]
    let ``Int32RangeLists_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    [<Theory; FileInlineData("UInt64RangeArrays.fs", Realsig = BooleanOptions.Both, Optimize = BooleanOptions.True)>]
    let ``UInt64RangeArrays_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    [<Theory; FileInlineData("UInt64RangeLists.fs", Realsig = BooleanOptions.Both, Optimize = BooleanOptions.True)>]
    let ``UInt64RangeLists_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    [<Theory; FileInlineData("ForNInRangeArrays.fs", Realsig = BooleanOptions.Both, Optimize = BooleanOptions.True)>]
    let ``ForNInRangeArrays_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    [<Theory; FileInlineData("ForNInRangeLists.fs", Realsig = BooleanOptions.Both, Optimize = BooleanOptions.True)>]
    let ``ForNInRangeLists_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    [<Theory; FileInlineData("ForXInArray_ToArray.fs", Realsig = BooleanOptions.Both, Optimize = BooleanOptions.True)>]
    let ``ForXInArray_ToArray_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    [<Theory; FileInlineData("ForXInArray_ToList.fs", Realsig = BooleanOptions.Both, Optimize = BooleanOptions.True)>]
    let ``ForXInArray_ToList_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    [<Theory; FileInlineData("ForXInList_ToArray.fs", Realsig = BooleanOptions.Both, Optimize = BooleanOptions.True)>]
    let ``ForXInList_ToArray_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    [<Theory; FileInlineData("ForXInList_ToList.fs", Realsig = BooleanOptions.Both, Optimize = BooleanOptions.True)>]
    let ``ForXInList_ToList_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    [<Theory; FileInlineData("ForXInSeq_ToArray.fs", Realsig = BooleanOptions.Both, Optimize = BooleanOptions.True)>]
    let ``ForXInSeq_ToArray_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    [<Theory; FileInlineData("ForXInSeq_ToList.fs", Realsig = BooleanOptions.Both, Optimize = BooleanOptions.True)>]
    let ``ForXInSeq_ToList_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation
