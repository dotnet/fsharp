namespace EmittedIL

open FSharp.Test
open FSharp.Test.Compiler
open Xunit

#nowarn  3391                   //

module ComputedCollections =
    let verifyCompilation (compilation: CompilationUnit) =
        compilation
        |> asExe
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> ignoreWarnings
        |> verifyILBaseline

    [<Theory; FileInlineData(__SOURCE_DIRECTORY__, "Int32RangeArrays.fs")>]
    let Int32RangeArrays_fs (compilation: CompilationHelper) =
        verifyCompilation compilation

    [<Theory; FileInlineData(__SOURCE_DIRECTORY__, "Int32RangeArrays.fs")>]
    let ``Int32RangeLists_fs`` (compilation: CompilationHelper) =
        verifyCompilation compilation
        
    [<Theory; FileInlineData(__SOURCE_DIRECTORY__, "UInt64RangeArrays.fs")>]
    let ``UInt64RangeArrays_fs`` (compilation: CompilationHelper) =
        verifyCompilation compilation

    [<Theory; FileInlineData(__SOURCE_DIRECTORY__, "UInt64RangeLists.fs")>]
    let ``UInt64RangeLists_fs`` (compilation: CompilationHelper) =
        verifyCompilation compilation

    [<Theory; FileInlineData(__SOURCE_DIRECTORY__, "ForNInRangeArrays.fs")>]
    let ``ForNInRangeArrays_fs`` (compilation: CompilationHelper) =
        verifyCompilation compilation

    [<Theory; FileInlineData(__SOURCE_DIRECTORY__, "ForNInRangeLists.fs")>]
    let ``ForNInRangeLists_fs`` (compilation: CompilationHelper) =
        verifyCompilation compilation

    [<Theory; FileInlineData(__SOURCE_DIRECTORY__, "ForXInArray_ToArray.fs")>]
    let ``ForXInArray_ToArray_fs`` (compilation: CompilationHelper) =
        verifyCompilation compilation

    [<Theory; FileInlineData(__SOURCE_DIRECTORY__, "ForXInArray_ToList.fs")>]
    let ``ForXInArray_ToList_fs`` (compilation: CompilationHelper) =
        verifyCompilation compilation

    [<Theory; FileInlineData(__SOURCE_DIRECTORY__, "ForXInList_ToArray.fs")>]
    let ``ForXInList_ToArray_fs`` (compilation: CompilationHelper) =
        verifyCompilation compilation

    [<Theory; FileInlineData(__SOURCE_DIRECTORY__, "ForXInList_ToList.fs")>]
    let ``ForXInList_ToList_fs`` (compilation: CompilationHelper) =
        verifyCompilation compilation

    [<Theory; FileInlineData(__SOURCE_DIRECTORY__, "ForXInSeq_ToArray.fs")>]
    let ``ForXInSeq_ToArray_fs`` (compilation: CompilationHelper) =
        verifyCompilation compilation

    [<Theory; FileInlineData(__SOURCE_DIRECTORY__, "ForXInSeq_ToList.fs")>]
    let ``ForXInSeq_ToList_fs`` (compilation: CompilationHelper) =
        verifyCompilation compilation
