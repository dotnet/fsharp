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
    [<Theory; FileInlineData("SeqExpressionSteppingTest01.fs", Realsig=BooleanOptions.Both)>]
    let ``SeqExpressionSteppingTest01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=SeqExpressionSteppingTest02.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd SeqExpressionSteppingTest2.exe"	# SeqExpressionSteppingTest2.fs -
    [<Theory; FileInlineData("SeqExpressionSteppingTest02.fs", Realsig=BooleanOptions.Both)>]
    let ``SeqExpressionSteppingTest02_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    //SOURCE=SeqExpressionSteppingTest03.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd SeqExpressionSteppingTest3.exe"	# SeqExpressionSteppingTest3.fs -
    [<Theory; FileInlineData("SeqExpressionSteppingTest03.fs", Realsig=BooleanOptions.Both)>]
    let ``SeqExpressionSteppingTest03_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=SeqExpressionSteppingTest04.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd SeqExpressionSteppingTest4.exe"	# SeqExpressionSteppingTest4.fs -
    [<Theory; FileInlineData("SeqExpressionSteppingTest04.fs", Realsig=BooleanOptions.Both)>]
    let ``SeqExpressionSteppingTest04_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=SeqExpressionSteppingTest05.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd SeqExpressionSteppingTest5.exe"	# SeqExpressionSteppingTest5.fs -
    [<Theory; FileInlineData("SeqExpressionSteppingTest05.fs", Realsig=BooleanOptions.Both)>]
    let ``SeqExpressionSteppingTest05_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=SeqExpressionSteppingTest06.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd SeqExpressionSteppingTest6.exe"	# SeqExpressionSteppingTest6.fs -
    [<Theory; FileInlineData("SeqExpressionSteppingTest06.fs", Realsig=BooleanOptions.Both)>]
    let ``SeqExpressionSteppingTest06_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

#if NETCOREAPP
    // SOURCE=SeqExpressionSteppingTest07.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd SeqExpressionSteppingTest7.exe"	# SeqExpressionSteppingTest7.fs -
    [<Theory; FileInlineData("SeqExpressionSteppingTest07.fs", Realsig=BooleanOptions.Both)>]
    let ``SeqExpressionSteppingTest07_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation
#endif