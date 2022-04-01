namespace FSharp.Compiler.ComponentTests.EmittedIL

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ListExpressionStepping =

    let verifyCompilation compilation =
        compilation
        |> withOptions [ "--test:EmitFeeFeeAs100001" ]
        |> asExe
        |> withNoOptimize
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> ignoreWarnings
        |> verifyILBaseline

    // SOURCE=ListExpressionSteppingTest1.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ListExpressionSteppingTest1.exe"    # ListExpressionSteppingTest1.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ListExpressionStepping01.fs"|])>]
    let ``ListExpressionStepping01_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=ListExpressionSteppingTest2.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ListExpressionSteppingTest2.exe"    # ListExpressionSteppingTest2.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ListExpressionStepping02.fs"|])>]
    let ``ListExpressionStepping02_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=ListExpressionSteppingTest3.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ListExpressionSteppingTest3.exe"    # ListExpressionSteppingTest3.fs 
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ListExpressionStepping03.fs"|])>]
    let ``ListExpressionStepping03_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=ListExpressionSteppingTest4.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ListExpressionSteppingTest4.exe"    # ListExpressionSteppingTest4.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ListExpressionStepping04.fs"|])>]
    let ``ListExpressionStepping04_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=ListExpressionSteppingTest5.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ListExpressionSteppingTest5.exe"    # ListExpressionSteppingTest5.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ListExpressionStepping05.fs"|])>]
    let ``ListExpressionStepping05_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=ListExpressionSteppingTest6.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ListExpressionSteppingTest6.exe"    # ListExpressionSteppingTest6.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ListExpressionStepping06.fs"|])>]
    let ``ListExpressionStepping06_fs`` compilation =
        compilation
        |> verifyCompilation
