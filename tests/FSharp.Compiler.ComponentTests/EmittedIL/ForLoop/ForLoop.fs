namespace FSharp.Compiler.ComponentTests.EmittedIL

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
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd03.fs SCFLAGS="--optimize-"	# NonTrivialBranchingBindingInEnd03.fs --optimize-
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NonTrivialBranchingBindingInEnd03.fs"|])>]
    let ``NonTrivialBranchingBindingInEnd03_fs_nonopt`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd04.fs SCFLAGS="--optimize+"	# NonTrivialBranchingBindingInEnd04.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".opt", Includes=[|"NonTrivialBranchingBindingInEnd04.fs"|])>]
    let ``NonTrivialBranchingBindingInEnd04_fs_opt`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd04.fs SCFLAGS="--optimize-"	# NonTrivialBranchingBindingInEnd04.fs --optimize-
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NonTrivialBranchingBindingInEnd04.fs"|])>]
    let ``NonTrivialBranchingBindingInEnd04_fs_nonopt`` compilation =
        compilation
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

