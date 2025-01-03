namespace EmittedIL.RealInternalSignature

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
    [<Theory; FileInlineData("ListExpressionStepping01.fs", Realsig=BooleanOptions.Both)>]
    let ``ListExpressionStepping01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=ListExpressionSteppingTest2.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ListExpressionSteppingTest2.exe"    # ListExpressionSteppingTest2.fs -
    [<Theory; FileInlineData("ListExpressionStepping02.fs", Realsig=BooleanOptions.Both)>]
    let ``ListExpressionStepping02_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=ListExpressionSteppingTest3.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ListExpressionSteppingTest3.exe"    # ListExpressionSteppingTest3.fs 
    [<Theory; FileInlineData("ListExpressionStepping03.fs", Realsig=BooleanOptions.Both)>]
    let ``ListExpressionStepping03_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=ListExpressionSteppingTest4.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ListExpressionSteppingTest4.exe"    # ListExpressionSteppingTest4.fs -
    [<Theory; FileInlineData("ListExpressionStepping04.fs", Realsig=BooleanOptions.Both)>]
    let ``ListExpressionStepping04_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=ListExpressionSteppingTest5.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ListExpressionSteppingTest5.exe"    # ListExpressionSteppingTest5.fs -
    [<Theory; FileInlineData("ListExpressionStepping05.fs", Realsig=BooleanOptions.Both)>]
    let ``ListExpressionStepping05_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=ListExpressionSteppingTest6.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ListExpressionSteppingTest6.exe"    # ListExpressionSteppingTest6.fs -
    [<Theory; FileInlineData("ListExpressionStepping06.fs", Realsig=BooleanOptions.Both)>]
    let ``ListExpressionStepping06_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

