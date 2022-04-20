namespace FSharp.Compiler.ComponentTests.EmittedIL

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
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SeqExpressionSteppingTest01.fs"|])>]
    let ``SeqExpressionSteppingTest01_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=SeqExpressionSteppingTest02.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd SeqExpressionSteppingTest2.exe"	# SeqExpressionSteppingTest2.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SeqExpressionSteppingTest02.fs"|])>]
    let ``SeqExpressionSteppingTest02_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=SeqExpressionSteppingTest03.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd SeqExpressionSteppingTest3.exe"	# SeqExpressionSteppingTest3.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SeqExpressionSteppingTest03.fs"|])>]
    let ``SeqExpressionSteppingTest03_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=SeqExpressionSteppingTest04.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd SeqExpressionSteppingTest4.exe"	# SeqExpressionSteppingTest4.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SeqExpressionSteppingTest04.fs"|])>]
    let ``SeqExpressionSteppingTest04_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=SeqExpressionSteppingTest05.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd SeqExpressionSteppingTest5.exe"	# SeqExpressionSteppingTest5.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SeqExpressionSteppingTest05.fs"|])>]
    let ``SeqExpressionSteppingTest05_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=SeqExpressionSteppingTest06.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd SeqExpressionSteppingTest6.exe"	# SeqExpressionSteppingTest6.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SeqExpressionSteppingTest06.fs"|])>]
    let ``SeqExpressionSteppingTest06_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=SeqExpressionSteppingTest07.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd SeqExpressionSteppingTest7.exe"	# SeqExpressionSteppingTest7.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SeqExpressionSteppingTest07.fs"|])>]
    let ``SeqExpressionSteppingTest07_fs`` compilation =
        compilation
        |> verifyCompilation
