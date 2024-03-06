namespace EmittedIL.RealInternalSignature

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ForLoopRealInternalSignatureOn =

    let verifyCompilation compilation =
        compilation
        |> withOptions [ "--test:EmitFeeFeeAs100001" ]
        |> asExe
        |> withOptimize
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> ignoreWarnings
        |> withRealInternalSignatureOn
        |> verifyILBaseline

    // SOURCE=NoAllocationOfTuple01.fs SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd NoAllocationOfTuple01.dll" # NoAllocationOfTuple01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NoAllocationOfTuple01.fs"|])>]
    let ``NoAllocationOfTuple01_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=ForEachOnArray01.fs      SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ForEachOnArray01.dll"      # ForEachOnArray01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ForEachOnArray01.fs"|])>]
    let ``ForEachOnArray01_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=ForEachOnList01.fs       SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ForEachOnList01.dll"       # ForEachOnList01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ForEachOnList01.fs"|])>]
    let ``ForEachOnList01_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=ForEachOnString01.fs     SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ForEachOnString01.dll"     # ForEachOnString01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ForEachOnString01.fs"|])>]
    let ``ForEachOnString01_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=ZeroToArrLength01.fs     SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ZeroToArrLength01.dll"     # ZeroToArrLength01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ZeroToArrLength01.fs"|])>]
    let ``ZeroToArrLength01_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=ZeroToArrLength02.fs     SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ZeroToArrLength02.dll"     # ZeroToArrLength02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ZeroToArrLength02.fs"|])>]
    let ``ZeroToArrLength02_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=NoIEnumerable01.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd NoIEnumerable01.dll"            # NoIEnumerable01.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NoIEnumerable01.fsx"|])>]
    let ``NoIEnumerable01_fsx`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=NoIEnumerable02.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd NoIEnumerable02.dll"            # NoIEnumerable02.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NoIEnumerable02.fsx"|])>]
    let ``NoIEnumerable02_fsx`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=NoIEnumerable03.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd NoIEnumerable03.dll"            # NoIEnumerable03.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NoIEnumerable03.fsx"|])>]
    let ``NoIEnumerable03_fsx`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd01.fs SCFLAGS="--optimize+"	# NonTrivialBranchingBindingInEnd01.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".opt", Includes=[|"NonTrivialBranchingBindingInEnd01.fs"|])>]
    let ``NonTrivialBranchingBindingInEnd01_fs_opt`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd01.fs SCFLAGS="--optimize-"	# NonTrivialBranchingBindingInEnd01.fs --optimize-
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NonTrivialBranchingBindingInEnd01.fs"|])>]
    let ``NonTrivialBranchingBindingInEnd01_fs_nonopt`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd02.fs SCFLAGS="--optimize+"	# NonTrivialBranchingBindingInEnd02.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".opt", Includes=[|"NonTrivialBranchingBindingInEnd02.fs"|])>]
    let ``NonTrivialBranchingBindingInEnd02_fs_opt`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd02.fs SCFLAGS="--optimize-"	# NonTrivialBranchingBindingInEnd02.fs --optimize-
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NonTrivialBranchingBindingInEnd02.fs"|])>]
    let ``NonTrivialBranchingBindingInEnd02_fs_nonopt`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd03.fs SCFLAGS="--optimize+"	# NonTrivialBranchingBindingInEnd03.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".opt", Includes=[|"NonTrivialBranchingBindingInEnd03.fs"|])>]
    let ``NonTrivialBranchingBindingInEnd03_fs_opt`` compilation =
        compilation
        |> withLangVersionPreview // TODO https://github.com/dotnet/fsharp/issues/16739: Remove this when LanguageFeature.LowerIntegralRangesToFastLoops is out of preview.
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd03.fs SCFLAGS="--optimize-"	# NonTrivialBranchingBindingInEnd03.fs --optimize-
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NonTrivialBranchingBindingInEnd03.fs"|])>]
    let ``NonTrivialBranchingBindingInEnd03_fs_nonopt`` compilation =
        compilation
        |> withLangVersionPreview // TODO https://github.com/dotnet/fsharp/issues/16739: Remove this when LanguageFeature.LowerIntegralRangesToFastLoops is out of preview.
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd04.fs SCFLAGS="--optimize+"	# NonTrivialBranchingBindingInEnd04.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".opt", Includes=[|"NonTrivialBranchingBindingInEnd04.fs"|])>]
    let ``NonTrivialBranchingBindingInEnd04_fs_opt`` compilation =
        compilation
        |> withLangVersionPreview // TODO https://github.com/dotnet/fsharp/issues/16739: Remove this when LanguageFeature.LowerIntegralRangesToFastLoops is out of preview.
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd04.fs SCFLAGS="--optimize-"	# NonTrivialBranchingBindingInEnd04.fs --optimize-
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NonTrivialBranchingBindingInEnd04.fs"|])>]
    let ``NonTrivialBranchingBindingInEnd04_fs_nonopt`` compilation =
        compilation
        |> withLangVersionPreview // TODO https://github.com/dotnet/fsharp/issues/16739: Remove this when LanguageFeature.LowerIntegralRangesToFastLoops is out of preview.
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd05.fs SCFLAGS="--optimize+"	# NonTrivialBranchingBindingInEnd05.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".opt", Includes=[|"NonTrivialBranchingBindingInEnd05.fs"|])>]
    let ``NonTrivialBranchingBindingInEnd05_fs_opt`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd05.fs SCFLAGS="--optimize-"	# NonTrivialBranchingBindingInEnd05.fs --optimize-
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NonTrivialBranchingBindingInEnd05.fs"|])>]
    let ``NonTrivialBranchingBindingInEnd05_fs_nonopt`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=ForEachRangeStepSByte.fs SCFLAGS="--optimize+"	# ForEachRangeStepSByte.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".opt", Includes=[|"ForEachRangeStepSByte.fs"|])>]
    let ``ForEachRangeStepSByte_fs_opt`` compilation =
        compilation
        |> withLangVersionPreview // TODO https://github.com/dotnet/fsharp/issues/16739: Remove this when LanguageFeature.LowerIntegralRangesToFastLoops is out of preview.
        |> verifyCompilation

    // SOURCE=ForEachRangeStepByte.fs SCFLAGS="--optimize+"	# ForEachRangeStepByte.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".opt", Includes=[|"ForEachRangeStepByte.fs"|])>]
    let ``ForEachRangeStepByte_fs_opt`` compilation =
        compilation
        |> withLangVersionPreview // TODO https://github.com/dotnet/fsharp/issues/16739: Remove this when LanguageFeature.LowerIntegralRangesToFastLoops is out of preview.
        |> verifyCompilation

    // SOURCE=ForEachRangeStepChar.fs SCFLAGS="--optimize+"	# ForEachRangeStepChar.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".opt", Includes=[|"ForEachRangeStepChar.fs"|])>]
    let ``ForEachRangeStepChar_fs_opt`` compilation =
        compilation
        |> withLangVersionPreview // TODO https://github.com/dotnet/fsharp/issues/16739: Remove this when LanguageFeature.LowerIntegralRangesToFastLoops is out of preview.
        |> verifyCompilation

    // SOURCE=ForEachRangeStepInt16.fs SCFLAGS="--optimize+"	# ForEachRangeStepInt16.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".opt", Includes=[|"ForEachRangeStepInt16.fs"|])>]
    let ``ForEachRangeStepInt16_fs_opt`` compilation =
        compilation
        |> withLangVersionPreview // TODO https://github.com/dotnet/fsharp/issues/16739: Remove this when LanguageFeature.LowerIntegralRangesToFastLoops is out of preview.
        |> verifyCompilation

    // SOURCE=ForEachRangeStepUInt16.fs SCFLAGS="--optimize+"	# ForEachRangeStepUInt16.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".opt", Includes=[|"ForEachRangeStepUInt16.fs"|])>]
    let ``ForEachRangeStepUInt16_fs_opt`` compilation =
        compilation
        |> withLangVersionPreview // TODO https://github.com/dotnet/fsharp/issues/16739: Remove this when LanguageFeature.LowerIntegralRangesToFastLoops is out of preview.
        |> verifyCompilation

    // SOURCE=ForEachRangeStepInt32.fs SCFLAGS="--optimize+"	# ForEachRangeStepInt32.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".opt", Includes=[|"ForEachRangeStepInt32.fs"|])>]
    let ``ForEachRangeStepInt32_fs_opt`` compilation =
        compilation
        |> withLangVersionPreview // TODO https://github.com/dotnet/fsharp/issues/16739: Remove this when LanguageFeature.LowerIntegralRangesToFastLoops is out of preview.
        |> verifyCompilation

    // SOURCE=ForEachRangeStepUInt32.fs SCFLAGS="--optimize+"	# ForEachRangeStepUInt32.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".opt", Includes=[|"ForEachRangeStepUInt32.fs"|])>]
    let ``ForEachRangeStepUInt32_fs_opt`` compilation =
        compilation
        |> withLangVersionPreview // TODO https://github.com/dotnet/fsharp/issues/16739: Remove this when LanguageFeature.LowerIntegralRangesToFastLoops is out of preview.
        |> verifyCompilation

    // SOURCE=ForEachRangeStepInt64.fs SCFLAGS="--optimize+"	# ForEachRangeStepInt64.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".opt", Includes=[|"ForEachRangeStepInt64.fs"|])>]
    let ``ForEachRangeStepInt64_fs_opt`` compilation =
        compilation
        |> withLangVersionPreview // TODO https://github.com/dotnet/fsharp/issues/16739: Remove this when LanguageFeature.LowerIntegralRangesToFastLoops is out of preview.
        |> verifyCompilation

    // SOURCE=ForEachRangeStepUInt64.fs SCFLAGS="--optimize+"	# ForEachRangeStepUInt64.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".opt", Includes=[|"ForEachRangeStepUInt64.fs"|])>]
    let ``ForEachRangeStepUInt64_fs_opt`` compilation =
        compilation
        |> withLangVersionPreview // TODO https://github.com/dotnet/fsharp/issues/16739: Remove this when LanguageFeature.LowerIntegralRangesToFastLoops is out of preview.
        |> verifyCompilation

    // SOURCE=ForEachRangeStepIntPtr.fs SCFLAGS="--optimize+"	# ForEachRangeStepIntPtr.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".opt", Includes=[|"ForEachRangeStepIntPtr.fs"|])>]
    let ``ForEachRangeStepIntPtr_fs_opt`` compilation =
        compilation
        |> withLangVersionPreview // TODO https://github.com/dotnet/fsharp/issues/16739: Remove this when LanguageFeature.LowerIntegralRangesToFastLoops is out of preview.
        |> verifyCompilation

    // SOURCE=ForEachRangeStepUIntPtr.fs SCFLAGS="--optimize+"	# ForEachRangeStepUIntPtr.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".opt", Includes=[|"ForEachRangeStepUIntPtr.fs"|])>]
    let ``ForEachRangeStepUIntPtr_fs_opt`` compilation =
        compilation
        |> withLangVersionPreview // TODO https://github.com/dotnet/fsharp/issues/16739: Remove this when LanguageFeature.LowerIntegralRangesToFastLoops is out of preview.
        |> verifyCompilation
