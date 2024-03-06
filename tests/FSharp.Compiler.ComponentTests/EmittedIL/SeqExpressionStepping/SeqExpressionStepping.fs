namespace EmittedIL.RealInternalSignature

open Xunit
open System.IO
open FSharp.Test
open FSharp.Test.Compiler

module SeqExpressionStepping =

    let verifyCompilation compilation =
        compilation
        |> withOptions [ "--test:EmitFeeFeeAs100001" ]
        |> asExe
        |> withNoOptimize
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> ignoreWarnings
        |> verifyILBaseline

    //Retry SOURCE=SeqExpressionSteppingTest01.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd SeqExpressionSteppingTest1.exe"	# SeqExpressionSteppingTest1.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"SeqExpressionSteppingTest01.fs"|])>]
    let ``SeqExpressionSteppingTest01_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    //Retry SOURCE=SeqExpressionSteppingTest01.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd SeqExpressionSteppingTest1.exe"	# SeqExpressionSteppingTest1.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"SeqExpressionSteppingTest01.fs"|])>]
    let ``SeqExpressionSteppingTest01_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=SeqExpressionSteppingTest02.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd SeqExpressionSteppingTest2.exe"	# SeqExpressionSteppingTest2.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"SeqExpressionSteppingTest02.fs"|])>]
    let ``SeqExpressionSteppingTest02_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    // SOURCE=SeqExpressionSteppingTest02.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd SeqExpressionSteppingTest2.exe"	# SeqExpressionSteppingTest2.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"SeqExpressionSteppingTest02.fs"|])>]
    let ``SeqExpressionSteppingTest02_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    //SOURCE=SeqExpressionSteppingTest03.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd SeqExpressionSteppingTest3.exe"	# SeqExpressionSteppingTest3.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"SeqExpressionSteppingTest03.fs"|])>]
    let ``SeqExpressionSteppingTest03_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    //SOURCE=SeqExpressionSteppingTest03.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd SeqExpressionSteppingTest3.exe"	# SeqExpressionSteppingTest3.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"SeqExpressionSteppingTest03.fs"|])>]
    let ``SeqExpressionSteppingTest03_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=SeqExpressionSteppingTest04.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd SeqExpressionSteppingTest4.exe"	# SeqExpressionSteppingTest4.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"SeqExpressionSteppingTest04.fs"|])>]
    let ``SeqExpressionSteppingTest04_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    // SOURCE=SeqExpressionSteppingTest04.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd SeqExpressionSteppingTest4.exe"	# SeqExpressionSteppingTest4.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"SeqExpressionSteppingTest04.fs"|])>]
    let ``SeqExpressionSteppingTest04_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=SeqExpressionSteppingTest05.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd SeqExpressionSteppingTest5.exe"	# SeqExpressionSteppingTest5.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"SeqExpressionSteppingTest05.fs"|])>]
    let ``SeqExpressionSteppingTest05_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    // SOURCE=SeqExpressionSteppingTest05.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd SeqExpressionSteppingTest5.exe"	# SeqExpressionSteppingTest5.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"SeqExpressionSteppingTest05.fs"|])>]
    let ``SeqExpressionSteppingTest05_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=SeqExpressionSteppingTest06.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd SeqExpressionSteppingTest6.exe"	# SeqExpressionSteppingTest6.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"SeqExpressionSteppingTest06.fs"|])>]
    let ``SeqExpressionSteppingTest06_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    // SOURCE=SeqExpressionSteppingTest06.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd SeqExpressionSteppingTest6.exe"	# SeqExpressionSteppingTest6.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"SeqExpressionSteppingTest06.fs"|])>]
    let ``SeqExpressionSteppingTest06_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=SeqExpressionSteppingTest07.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd SeqExpressionSteppingTest7.exe"	# SeqExpressionSteppingTest7.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"SeqExpressionSteppingTest07.fs"|])>]
    let ``SeqExpressionSteppingTest07_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> withLangVersionPreview // TODO https://github.com/dotnet/fsharp/issues/16739: Remove this when LanguageFeature.LowerIntegralRangesToFastLoops is out of preview.
        |> verifyCompilation

    // SOURCE=SeqExpressionSteppingTest07.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd SeqExpressionSteppingTest7.exe"	# SeqExpressionSteppingTest7.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"SeqExpressionSteppingTest07.fs"|])>]
    let ``SeqExpressionSteppingTest07_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> withLangVersionPreview // TODO https://github.com/dotnet/fsharp/issues/16739: Remove this when LanguageFeature.LowerIntegralRangesToFastLoops is out of preview.
        |> verifyCompilation
