namespace EmittedIL

open FSharp.Test
open FSharp.Test.Compiler
open Xunit

module ComputedCollections =
    let verifyCompilation realsig compilation =
        compilation
        |> asExe
        |> withRealInternalSignature realsig
        |> withOptimize
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> ignoreWarnings
        |> verifyILBaseline

    [<Theory; FileInlineData("Int32RangeArrays.fs")>]
    let Int32RangeArrays_fs (compilation: CompilationHelper) = 
        compilation
        |> getCompilation
        |> verifyCompilation false

    [<Theory; FileInlineData("Int32RangeLists.fs")>]
    let ``Int32RangeLists_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation false

    [<Theory; FileInlineData("UInt64RangeArrays.fs")>]
    let ``UInt64RangeArrays_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation false

    [<Theory; FileInlineData("UInt64RangeLists.fs")>]
    let ``UInt64RangeLists_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation false

    [<Theory; FileInlineData("ForNInRangeArrays.fs")>]
    let ``ForNInRangeArrays_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation false

    [<Theory; FileInlineData("ForNInRangeLists.fs")>]
    let ``ForNInRangeLists_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation false

    [<Theory; FileInlineData("ForXInArray_ToArray.fs")>]
    let ``ForXInArray_ToArray_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation false

    [<Theory; FileInlineData("ForXInArray_ToList.fs")>]
    let ``ForXInArray_ToList_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation false

    [<Theory; FileInlineData("ForXInList_ToArray.fs")>]
    let ``ForXInList_ToArray_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation false

    [<Theory; FileInlineData("ForXInList_ToList.fs")>]
    let ``ForXInList_ToList_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation false

    [<Theory; FileInlineData("ForXInSeq_ToArray.fs")>]
    let ``ForXInSeq_ToArray_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation false

    [<Theory; FileInlineData("ForXInSeq_ToList.fs")>]
    let ``ForXInSeq_ToList_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation false
