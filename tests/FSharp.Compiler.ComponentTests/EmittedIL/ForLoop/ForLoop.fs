namespace EmittedIL

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ForLoop =

    let verifyCompilation compilation =
        compilation
        |> withOptions [ "--test:EmitFeeFeeAs100001" ]
        |> asExe
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> ignoreWarnings
        |> verifyILBaseline

    // SOURCE=ForLoop01.fs                 SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ForLoop01.exe"	# ForLoop01.fs -
    [<Theory; FileInlineData("ForLoop01.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.False)>]
    let ``ForLoop01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=ForLoop02.fs                 SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ForLoop02.exe"	# ForLoop02.fs
    [<Theory; FileInlineData("ForLoop02.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.False)>]
    let ``ForLoop02_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=ForLoop03.fs                 SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ForLoop03.exe"	# ForLoop03.fs
    [<Theory; FileInlineData("ForLoop03.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.False)>]
    let ``ForLoop03_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=NoAllocationOfTuple01.fs SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd NoAllocationOfTuple01.dll" # NoAllocationOfTuple01.fs
    [<Theory; FileInlineData("NoAllocationOfTuple01.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.True)>]
    let ``NoAllocationOfTuple01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=ForEachOnArray01.fs      SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ForEachOnArray01.dll"      # ForEachOnArray01.fs
    [<Theory; FileInlineData("ForEachOnArray01.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.True)>]
    let ``ForEachOnArray01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=ForEachOnList01.fs       SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ForEachOnList01.dll"       # ForEachOnList01.fs
    [<Theory; FileInlineData("ForEachOnList01.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.True)>]
    let ``ForEachOnList01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=ForEachOnString01.fs     SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ForEachOnString01.dll"     # ForEachOnString01.fs
    [<Theory; FileInlineData("ForEachOnString01.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.True)>]
    let ``ForEachOnString01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=ZeroToArrLength01.fs     SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ZeroToArrLength01.dll"     # ZeroToArrLength01.fs
    [<Theory; FileInlineData("ZeroToArrLength01.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.True)>]
    let ``ZeroToArrLength01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=ZeroToArrLength02.fs     SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ZeroToArrLength02.dll"     # ZeroToArrLength02.fs
    [<Theory; FileInlineData("ZeroToArrLength02.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.True)>]
    let ``ZeroToArrLength02_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=NoIEnumerable01.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd NoIEnumerable01.dll"            # NoIEnumerable01.fsx
    [<Theory; FileInlineData("NoIEnumerable01.fsx", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.True)>]
    let ``NoIEnumerable01_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=NoIEnumerable02.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd NoIEnumerable02.dll"            # NoIEnumerable02.fsx
    [<Theory; FileInlineData("NoIEnumerable02.fsx", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.True)>]
    let ``NoIEnumerable02_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=NoIEnumerable03.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd NoIEnumerable03.dll"            # NoIEnumerable03.fsx
    [<Theory; FileInlineData("NoIEnumerable03.fsx", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.True)>]
    let ``NoIEnumerable03_fsx`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd01.fs SCFLAGS="--optimize+"	# NonTrivialBranchingBindingInEnd01.fs --optimize+
    [<Theory; FileInlineData("NonTrivialBranchingBindingInEnd01.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.Both)>]
    let ``NonTrivialBranchingBindingInEnd01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd02.fs SCFLAGS="--optimize+"	# NonTrivialBranchingBindingInEnd02.fs --optimize+
    [<Theory; FileInlineData("NonTrivialBranchingBindingInEnd02.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.Both)>]
    let ``NonTrivialBranchingBindingInEnd02_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd03.fs SCFLAGS="--optimize+"	# NonTrivialBranchingBindingInEnd03.fs --optimize+
    [<Theory; FileInlineData("NonTrivialBranchingBindingInEnd03.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.Both)>]
    let ``NonTrivialBranchingBindingInEnd03_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd04.fs SCFLAGS="--optimize+"	# NonTrivialBranchingBindingInEnd04.fs --optimize+
    [<Theory; FileInlineData("NonTrivialBranchingBindingInEnd04.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.Both)>]
    let ``NonTrivialBranchingBindingInEnd04_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd05.fs SCFLAGS="--optimize+"	# NonTrivialBranchingBindingInEnd05.fs --optimize+
    [<Theory; FileInlineData("NonTrivialBranchingBindingInEnd05.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.Both)>]
    let ``NonTrivialBranchingBindingInEnd05_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=ForEachRangeStepSByte.fs SCFLAGS="--optimize+"	# ForEachRangeStepSByte.fs --optimize+
    [<Theory; FileInlineData("ForEachRangeStepSByte.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.True)>]
    let ``ForEachRangeStepSByte_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=ForEachRangeStepByte.fs SCFLAGS="--optimize+"	# ForEachRangeStepByte.fs --optimize+
    [<Theory; FileInlineData("ForEachRangeStepByte.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.True)>]
    let ``ForEachRangeStepByte_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=ForEachRangeStepChar.fs SCFLAGS="--optimize+"	# ForEachRangeStepChar.fs --optimize+
    [<Theory; FileInlineData("ForEachRangeStepChar.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.True)>]
    let ``ForEachRangeStepChar_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=ForEachRangeStepInt16.fs SCFLAGS="--optimize+"	# ForEachRangeStepInt16.fs --optimize+
    [<Theory; FileInlineData("ForEachRangeStepInt16.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.True)>]
    let ``ForEachRangeStepInt16_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=ForEachRangeStepUInt16.fs SCFLAGS="--optimize+"	# ForEachRangeStepUInt16.fs --optimize+
    [<Theory; FileInlineData("ForEachRangeStepUInt16.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.True)>]
    let ``ForEachRangeStepUInt16_`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=ForEachRangeStepInt32.fs SCFLAGS="--optimize+"	# ForEachRangeStepInt32.fs --optimize+
    [<Theory; FileInlineData("ForEachRangeStepInt32.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.True)>]
    let ``ForEachRangeStepInt32_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=ForEachRangeStepUInt32.fs SCFLAGS="--optimize+"	# ForEachRangeStepUInt32.fs --optimize+
    [<Theory; FileInlineData("ForEachRangeStepUInt32.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.True)>]
    let ``ForEachRangeStepUInt32_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=ForEachRangeStepInt64.fs SCFLAGS="--optimize+"	# ForEachRangeStepInt64.fs --optimize+
    [<Theory; FileInlineData("ForEachRangeStepInt64.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.True)>]
    let ``ForEachRangeStepInt64_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=ForEachRangeStepUInt64.fs SCFLAGS="--optimize+"	# ForEachRangeStepUInt64.fs --optimize+
    [<Theory; FileInlineData("ForEachRangeStepUInt64.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.True)>]
    let ``ForEachRangeStepUInt64_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=ForEachRangeStepIntPtr.fs SCFLAGS="--optimize+"	# ForEachRangeStepIntPtr.fs --optimize+
    [<Theory; FileInlineData("ForEachRangeStepIntPtr.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.True)>]
    let ``ForEachRangeStepIntPtr_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=ForEachRangeStepUIntPtr.fs SCFLAGS="--optimize+"	# ForEachRangeStepUIntPtr.fs --optimize+
    [<Theory; FileInlineData("ForEachRangeStepUIntPtr.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.True)>]
    let ``ForEachRangeStepUIntPtr_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=ForEachRangeStep_UnitsOfMeasure.fs SCFLAGS="--optimize+"	# ForEachRangeStep_UnitsOfMeasure.fs --optimize+
    [<Theory; FileInlineData("ForEachRangeStep_UnitsOfMeasure.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.True)>]
    let ``ForEachRangeStep_UnitsOfMeasure_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation
