namespace EmittedIL

open FSharp.Test
open FSharp.Test.Compiler
open Xunit

module ComputedCollections =
    let verifyCompilation realsig optimize compilation=
        compilation
        |> asExe
        |> withRealInternalSignature realsig
        |> withOptimization optimize
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> ignoreWarnings
        |> verifyILBaseline

//    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Int32RangeArrays.fs"|])>]
    [<Theory; FileInlineData("Int32RangeArrays.fs", Directory=__SOURCE_DIRECTORY__, Realsig=BooleanOptions.False, Optimize=BooleanOptions.Both)>]
    let Int32RangeArrays_fs (compilation) =
        compilation |> verifyCompilation

//    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Int32RangeLists.fs"|])>]
//    [<Theory; FileInlineData("Int32RangeArrays.fs", Directory=__SOURCE_DIRECTORY__, Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
    let ``Int32RangeLists_fs`` (filename, realsig, optimize) =
        ignore (filename, realsig, optimize)
//        FileInlineDataAttribute.GetCompilation()
//        |> verifyCompilation true true 
        
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"UInt64RangeArrays.fs"|])>]
    let ``UInt64RangeArrays_fs`` compilation =
        compilation
        |> verifyCompilation true true 

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"UInt64RangeLists.fs"|])>]
    let ``UInt64RangeLists_fs`` compilation =
        compilation
        |> verifyCompilation true true 

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ForNInRangeArrays.fs"|])>]
    let ``ForNInRangeArrays_fs`` compilation =
        compilation
        |> verifyCompilation true true 

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ForNInRangeLists.fs"|])>]
    let ``ForNInRangeLists_fs`` compilation =
        compilation
        |> verifyCompilation true true 

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ForXInArray_ToArray.fs"|])>]
    let ``ForXInArray_ToArray_fs`` compilation =
        compilation
        |> verifyCompilation true true 

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ForXInArray_ToList.fs"|])>]
    let ``ForXInArray_ToList_fs`` compilation =
        compilation
        |> verifyCompilation true true 

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ForXInList_ToArray.fs"|])>]
    let ``ForXInList_ToArray_fs`` compilation =
        compilation
        |> verifyCompilation true true 

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ForXInList_ToList.fs"|])>]
    let ``ForXInList_ToList_fs`` compilation =
        compilation
        |> verifyCompilation true true 

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ForXInSeq_ToArray.fs"|])>]
    let ``ForXInSeq_ToArray_fs`` compilation =
        compilation
        |> verifyCompilation true true 

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ForXInSeq_ToList.fs"|])>]
    let ``ForXInSeq_ToList_fs`` compilation =
        compilation
        |> verifyCompilation true true 
