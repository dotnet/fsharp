namespace EmittedIL

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ForLoop =

    let verifyCompilation compilation =
        compilation
        |> withOptions [ "--test:EmitFeeFeeAs100001" ]
        |> asExe
        |> withOptimize
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> ignoreWarnings
        |> verifyILBaseline

    // SOURCE=NoAllocationOfTuple01.fs SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd NoAllocationOfTuple01.dll" # NoAllocationOfTuple01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"NoAllocationOfTuple01.fs"|])>]
    let ``NoAllocationOfTuple01_fs_RealInternalSignatureOff`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=NoAllocationOfTuple01.fs SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd NoAllocationOfTuple01.dll" # NoAllocationOfTuple01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"NoAllocationOfTuple01.fs"|])>]
    let ``NoAllocationOfTuple01_fs_RealInternalSignatureOn`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    // SOURCE=ForEachOnArray01.fs      SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ForEachOnArray01.dll"      # ForEachOnArray01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"ForEachOnArray01.fs"|])>]
    let ``ForEachOnArray01_fs_RealInternalSignatureOff`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=ForEachOnArray01.fs      SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ForEachOnArray01.dll"      # ForEachOnArray01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"ForEachOnArray01.fs"|])>]
    let ``ForEachOnArray01_fs_RealInternalSignatureOn`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    // SOURCE=ForEachOnList01.fs       SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ForEachOnList01.dll"       # ForEachOnList01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"ForEachOnList01.fs"|])>]
    let ``ForEachOnList01_fs_RealInternalSignatureOff`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=ForEachOnList01.fs       SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ForEachOnList01.dll"       # ForEachOnList01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"ForEachOnList01.fs"|])>]
    let ``ForEachOnList01_fs_RealInternalSignatureOn`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    // SOURCE=ForEachOnString01.fs     SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ForEachOnString01.dll"     # ForEachOnString01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"ForEachOnString01.fs"|])>]
    let ``ForEachOnString01_fs_RealInternalSignatureOff`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=ForEachOnString01.fs     SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ForEachOnString01.dll"     # ForEachOnString01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"ForEachOnString01.fs"|])>]
    let ``ForEachOnString01_fs_RealInternalSignatureOn`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    // SOURCE=ZeroToArrLength01.fs     SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ZeroToArrLength01.dll"     # ZeroToArrLength01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"ZeroToArrLength01.fs"|])>]
    let ``ZeroToArrLength01_fs_RealInternalSignatureOff`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=ZeroToArrLength01.fs     SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ZeroToArrLength01.dll"     # ZeroToArrLength01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"ZeroToArrLength01.fs"|])>]
    let ``ZeroToArrLength01_fs_RealInternalSignatureOn`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    // SOURCE=ZeroToArrLength02.fs     SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ZeroToArrLength02.dll"     # ZeroToArrLength02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"ZeroToArrLength02.fs"|])>]
    let ``ZeroToArrLength02_fs_RealInternalSignatureOff`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=ZeroToArrLength02.fs     SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ZeroToArrLength02.dll"     # ZeroToArrLength02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"ZeroToArrLength02.fs"|])>]
    let ``ZeroToArrLength02_fs_RealInternalSignatureOn`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    // SOURCE=NoIEnumerable01.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd NoIEnumerable01.dll"            # NoIEnumerable01.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"NoIEnumerable01.fsx"|])>]
    let ``NoIEnumerable01_fsx_RealInternalSignatureOff`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=NoIEnumerable01.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd NoIEnumerable01.dll"            # NoIEnumerable01.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"NoIEnumerable01.fsx"|])>]
    let ``NoIEnumerable01_fsx_RealInternalSignatureOn`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    // SOURCE=NoIEnumerable02.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd NoIEnumerable02.dll"            # NoIEnumerable02.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"NoIEnumerable02.fsx"|])>]
    let ``NoIEnumerable02_fsx_RealInternalSignatureOff`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=NoIEnumerable02.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd NoIEnumerable02.dll"            # NoIEnumerable02.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"NoIEnumerable02.fsx"|])>]
    let ``NoIEnumerable02_fsx_RealInternalSignatureOn`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    // SOURCE=NoIEnumerable03.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd NoIEnumerable03.dll"            # NoIEnumerable03.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"NoIEnumerable03.fsx"|])>]
    let ``NoIEnumerable03_fsx_RealInternalSignatureOff`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=NoIEnumerable03.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd NoIEnumerable03.dll"            # NoIEnumerable03.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"NoIEnumerable03.fsx"|])>]
    let ``NoIEnumerable03_fsx_RealInternalSignatureOn`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd01.fs SCFLAGS="--optimize+"	# NonTrivialBranchingBindingInEnd01.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff.opt", Includes=[|"NonTrivialBranchingBindingInEnd01.fs"|])>]
    let ``NonTrivialBranchingBindingInEnd01_fs_RealInternalSignatureOff_opt`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd01.fs SCFLAGS="--optimize+"	# NonTrivialBranchingBindingInEnd01.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn.opt", Includes=[|"NonTrivialBranchingBindingInEnd01.fs"|])>]
    let ``NonTrivialBranchingBindingInEnd01_fs_RealInternalSignatureOn_opt`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd01.fs SCFLAGS="--optimize-"	# NonTrivialBranchingBindingInEnd01.fs --optimize-
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"NonTrivialBranchingBindingInEnd01.fs"|])>]
    let ``NonTrivialBranchingBindingInEnd01_fs_RealInternalSignatureOff_nonopt`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd01.fs SCFLAGS="--optimize-"	# NonTrivialBranchingBindingInEnd01.fs --optimize-
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"NonTrivialBranchingBindingInEnd01.fs"|])>]
    let ``NonTrivialBranchingBindingInEnd01_fs_RealInternalSignatureOn_nonopt`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd02.fs SCFLAGS="--optimize+"	# NonTrivialBranchingBindingInEnd02.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff.opt", Includes=[|"NonTrivialBranchingBindingInEnd02.fs"|])>]
    let ``NonTrivialBranchingBindingInEnd02_fs_RealInternalSignatureOff_opt`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd02.fs SCFLAGS="--optimize+"	# NonTrivialBranchingBindingInEnd02.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn.opt", Includes=[|"NonTrivialBranchingBindingInEnd02.fs"|])>]
    let ``NonTrivialBranchingBindingInEnd02_fs_RealInternalSignatureOn_opt`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd02.fs SCFLAGS="--optimize-"	# NonTrivialBranchingBindingInEnd02.fs --optimize-
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"NonTrivialBranchingBindingInEnd02.fs"|])>]
    let ``NonTrivialBranchingBindingInEnd02_fs_RealInternalSignatureOff_nonopt`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd02.fs SCFLAGS="--optimize-"	# NonTrivialBranchingBindingInEnd02.fs --optimize-
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"NonTrivialBranchingBindingInEnd02.fs"|])>]
    let ``NonTrivialBranchingBindingInEnd02_fs_RealInternalSignatureOn_nonopt`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd03.fs SCFLAGS="--optimize+"	# NonTrivialBranchingBindingInEnd03.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff.opt", Includes=[|"NonTrivialBranchingBindingInEnd03.fs"|])>]
    let ``NonTrivialBranchingBindingInEnd03_fs_RealInternalSignatureOff_opt`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd03.fs SCFLAGS="--optimize+"	# NonTrivialBranchingBindingInEnd03.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn.opt", Includes=[|"NonTrivialBranchingBindingInEnd03.fs"|])>]
    let ``NonTrivialBranchingBindingInEnd03_fs_RealInternalSignatureOn_opt`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd03.fs SCFLAGS="--optimize-"	# NonTrivialBranchingBindingInEnd03.fs --optimize-
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"NonTrivialBranchingBindingInEnd03.fs"|])>]
    let ``NonTrivialBranchingBindingInEnd03_fs_RealInternalSignatureOff_nonopt`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd03.fs SCFLAGS="--optimize-"	# NonTrivialBranchingBindingInEnd03.fs --optimize-
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"NonTrivialBranchingBindingInEnd03.fs"|])>]
    let ``NonTrivialBranchingBindingInEnd03_fs_RealInternalSignatureOn_nonopt`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd04.fs SCFLAGS="--optimize+"	# NonTrivialBranchingBindingInEnd04.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff.opt", Includes=[|"NonTrivialBranchingBindingInEnd04.fs"|])>]
    let ``NonTrivialBranchingBindingInEnd04_fs_RealInternalSignatureOff_opt`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd04.fs SCFLAGS="--optimize+"	# NonTrivialBranchingBindingInEnd04.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn.opt", Includes=[|"NonTrivialBranchingBindingInEnd04.fs"|])>]
    let ``NonTrivialBranchingBindingInEnd04_fs_RealInternalSignatureOn_opt`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd04.fs SCFLAGS="--optimize-"	# NonTrivialBranchingBindingInEnd04.fs --optimize-
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"NonTrivialBranchingBindingInEnd04.fs"|])>]
    let ``NonTrivialBranchingBindingInEnd04_fs_RealInternalSignatureOff_nonopt`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd04.fs SCFLAGS="--optimize-"	# NonTrivialBranchingBindingInEnd04.fs --optimize-
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"NonTrivialBranchingBindingInEnd04.fs"|])>]
    let ``NonTrivialBranchingBindingInEnd04_fs_RealInternalSignatureOn_nonopt`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd05.fs SCFLAGS="--optimize+"	# NonTrivialBranchingBindingInEnd05.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff.opt", Includes=[|"NonTrivialBranchingBindingInEnd05.fs"|])>]
    let ``NonTrivialBranchingBindingInEnd05_fs_RealInternalSignatureOff_opt`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd05.fs SCFLAGS="--optimize+"	# NonTrivialBranchingBindingInEnd05.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn.opt", Includes=[|"NonTrivialBranchingBindingInEnd05.fs"|])>]
    let ``NonTrivialBranchingBindingInEnd05_fs_RealInternalSignatureOn_opt`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd05.fs SCFLAGS="--optimize-"	# NonTrivialBranchingBindingInEnd05.fs --optimize-
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"NonTrivialBranchingBindingInEnd05.fs"|])>]
    let ``NonTrivialBranchingBindingInEnd05_fs_RealInternalSignatureOff_nonopt`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd05.fs SCFLAGS="--optimize-"	# NonTrivialBranchingBindingInEnd05.fs --optimize-
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"NonTrivialBranchingBindingInEnd05.fs"|])>]
    let ``NonTrivialBranchingBindingInEnd05_fs_RealInternalSignatureOn_nonopt`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    // SOURCE=ForEachRangeStepSByte.fs SCFLAGS="--optimize+"	# ForEachRangeStepSByte.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff.opt", Includes=[|"ForEachRangeStepSByte.fs"|])>]
    let ``ForEachRangeStepSByte_fs_RealInternalSignatureOff_opt`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=ForEachRangeStepSByte.fs SCFLAGS="--optimize+"	# ForEachRangeStepSByte.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn.opt", Includes=[|"ForEachRangeStepSByte.fs"|])>]
    let ``ForEachRangeStepSByte_fs_RealInternalSignatureOn_opt`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    // SOURCE=ForEachRangeStepByte.fs SCFLAGS="--optimize+"	# ForEachRangeStepByte.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff.opt", Includes=[|"ForEachRangeStepByte.fs"|])>]
    let ``ForEachRangeStepByte_fs_RealInternalSignatureOff_opt`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=ForEachRangeStepByte.fs SCFLAGS="--optimize+"	# ForEachRangeStepByte.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn.opt", Includes=[|"ForEachRangeStepByte.fs"|])>]
    let ``ForEachRangeStepByte_fs_RealInternalSignatureOn_opt`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    // SOURCE=ForEachRangeStepChar.fs SCFLAGS="--optimize+"	# ForEachRangeStepChar.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff.opt", Includes=[|"ForEachRangeStepChar.fs"|])>]
    let ``ForEachRangeStepChar_fs_RealInternalSignatureOff_opt`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=ForEachRangeStepChar.fs SCFLAGS="--optimize+"	# ForEachRangeStepChar.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn.opt", Includes=[|"ForEachRangeStepChar.fs"|])>]
    let ``ForEachRangeStepChar_fs_RealInternalSignatureOn_opt`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    // SOURCE=ForEachRangeStepInt16.fs SCFLAGS="--optimize+"	# ForEachRangeStepInt16.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff.opt", Includes=[|"ForEachRangeStepInt16.fs"|])>]
    let ``ForEachRangeStepInt16_fs_RealInternalSignatureOff_opt`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=ForEachRangeStepInt16.fs SCFLAGS="--optimize+"	# ForEachRangeStepInt16.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn.opt", Includes=[|"ForEachRangeStepInt16.fs"|])>]
    let ``ForEachRangeStepInt16_fs_RealInternalSignatureOn_opt`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    // SOURCE=ForEachRangeStepUInt16.fs SCFLAGS="--optimize+"	# ForEachRangeStepUInt16.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff.opt", Includes=[|"ForEachRangeStepUInt16.fs"|])>]
    let ``ForEachRangeStepUInt16_fs_RealInternalSignatureOff_opt`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=ForEachRangeStepUInt16.fs SCFLAGS="--optimize+"	# ForEachRangeStepUInt16.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn.opt", Includes=[|"ForEachRangeStepUInt16.fs"|])>]
    let ``ForEachRangeStepUInt16_fs_RealInternalSignatureOn_opt`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    // SOURCE=ForEachRangeStepInt32.fs SCFLAGS="--optimize+"	# ForEachRangeStepInt32.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff.opt", Includes=[|"ForEachRangeStepInt32.fs"|])>]
    let ``ForEachRangeStepInt32_fs_RealInternalSignatureOff_opt`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=ForEachRangeStepInt32.fs SCFLAGS="--optimize+"	# ForEachRangeStepInt32.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn.opt", Includes=[|"ForEachRangeStepInt32.fs"|])>]
    let ``ForEachRangeStepInt32_fs_RealInternalSignatureOn_opt`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    // SOURCE=ForEachRangeStepUInt32.fs SCFLAGS="--optimize+"	# ForEachRangeStepUInt32.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff.opt", Includes=[|"ForEachRangeStepUInt32.fs"|])>]
    let ``ForEachRangeStepUInt32_fs_RealInternalSignatureOff_opt`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=ForEachRangeStepUInt32.fs SCFLAGS="--optimize+"	# ForEachRangeStepUInt32.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn.opt", Includes=[|"ForEachRangeStepUInt32.fs"|])>]
    let ``ForEachRangeStepUInt32_fs_RealInternalSignatureOn_opt`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    // SOURCE=ForEachRangeStepInt64.fs SCFLAGS="--optimize+"	# ForEachRangeStepInt64.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff.opt", Includes=[|"ForEachRangeStepInt64.fs"|])>]
    let ``ForEachRangeStepInt64_fs_RealInternalSignatureOff_opt`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=ForEachRangeStepInt64.fs SCFLAGS="--optimize+"	# ForEachRangeStepInt64.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn.opt", Includes=[|"ForEachRangeStepInt64.fs"|])>]
    let ``ForEachRangeStepInt64_fs_RealInternalSignatureOn_opt`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    // SOURCE=ForEachRangeStepUInt64.fs SCFLAGS="--optimize+"	# ForEachRangeStepUInt64.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff.opt", Includes=[|"ForEachRangeStepUInt64.fs"|])>]
    let ``ForEachRangeStepUInt64_fs_RealInternalSignatureOff_opt`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=ForEachRangeStepUInt64.fs SCFLAGS="--optimize+"	# ForEachRangeStepUInt64.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn.opt", Includes=[|"ForEachRangeStepUInt64.fs"|])>]
    let ``ForEachRangeStepUInt64_fs_RealInternalSignatureOn_opt`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    // SOURCE=ForEachRangeStepIntPtr.fs SCFLAGS="--optimize+"	# ForEachRangeStepIntPtr.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff.opt", Includes=[|"ForEachRangeStepIntPtr.fs"|])>]
    let ``ForEachRangeStepIntPtr_fs_RealInternalSignatureOff_opt`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=ForEachRangeStepIntPtr.fs SCFLAGS="--optimize+"	# ForEachRangeStepIntPtr.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn.opt", Includes=[|"ForEachRangeStepIntPtr.fs"|])>]
    let ``ForEachRangeStepIntPtr_fs_RealInternalSignatureOn_opt`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    // SOURCE=ForEachRangeStepUIntPtr.fs SCFLAGS="--optimize+"	# ForEachRangeStepUIntPtr.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff.opt", Includes=[|"ForEachRangeStepUIntPtr.fs"|])>]
    let ``ForEachRangeStepUIntPtr_fs_RealInternalSignatureOff_opt`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=ForEachRangeStepUIntPtr.fs SCFLAGS="--optimize+"	# ForEachRangeStepUIntPtr.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn.opt", Includes=[|"ForEachRangeStepUIntPtr.fs"|])>]
    let ``ForEachRangeStepUIntPtr_fs_RealInternalSignatureOn_opt`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    // SOURCE=ForEachRangeStep_UnitsOfMeasure.fs SCFLAGS="--optimize+"	# ForEachRangeStep_UnitsOfMeasure.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff.opt", Includes=[|"ForEachRangeStep_UnitsOfMeasure.fs"|])>]
    let ``ForEachRangeStep_UnitsOfMeasure_fs_RealInternalSignatureOff_opt`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=ForEachRangeStep_UnitsOfMeasure.fs SCFLAGS="--optimize+"	# ForEachRangeStep_UnitsOfMeasure.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn.opt", Includes=[|"ForEachRangeStep_UnitsOfMeasure.fs"|])>]
    let ``ForEachRangeStep_UnitsOfMeasure_fs_RealInternalSignatureOn_opt`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation
