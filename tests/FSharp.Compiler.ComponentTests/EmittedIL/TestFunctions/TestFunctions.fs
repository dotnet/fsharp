namespace FSharp.Compiler.ComponentTests.EmittedIL

open Xunit
open System.IO
open FSharp.Test
open FSharp.Test.Compiler

module TestFunctions =

    let verifyCore compilation =
        compilation
        |> withOptions [ "--test:EmitFeeFeeAs100001"; "--nowarn:988"; "--nowarn:3370"]
        |> asExe
        |> withNoOptimize
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> ignoreWarnings

    let verifyCompileAndRun compilation =
        compilation
        |> verifyCore
        |> compileAndRun

    let verifyCompilation compilation =
        compilation
        |> verifyCore
        |> verifyILBaseline

    //SOURCE=TestFunction01.fs   SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction01.exe"	# TestFunction01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TestFunction01.fs"|])>]
    let ``TestFunction01_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=TestFunction02.fs   SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction02.exe"	# TestFunction02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TestFunction02.fs"|])>]
    let ``TestFunction02_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=TestFunction03.fs   SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction03.exe"	# TestFunction03.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TestFunction03.fs"|])>]
    let ``TestFunction03_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=TestFunction03b.fs  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction03b.exe"	# TestFunction03b.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TestFunction03b.fs"|])>]
    let ``TestFunction03b_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=TestFunction03c.fs  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction03c.exe"	# TestFunction03c.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TestFunction03c.fs"|])>]
    let ``TestFunction03c_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=TestFunction04.fs   SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction04.exe"	# TestFunction04.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TestFunction04.fs"|])>]
    let ``TestFunction04_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=TestFunction05.fs   SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction05.exe"	# TestFunction05.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TestFunction05.fs"|])>]
    let ``TestFunction05_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=TestFunction06.fs   SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction06.exe"	# TestFunction06.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TestFunction06.fs"|])>]
    let ``TestFunction06_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=TestFunction07.fs   SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction07.exe"	# TestFunction07.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TestFunction07.fs"|])>]
    let ``TestFunction07_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=TestFunction08.fs   SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction08.exe"	# TestFunction08.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TestFunction08.fs"|])>]
    let ``TestFunction08_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=TestFunction09.fs   SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction09.exe"	# TestFunction09.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TestFunction09.fs"|])>]
    let ``TestFunction09_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=TestFunction09b.fs  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction09b.exe"	# TestFunction09b.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TestFunction09b.fs"|])>]
    let ``TestFunction09b_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=TestFunction09b1.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction09b1.exe"	# TestFunction09b1.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TestFunction09b1.fs"|])>]
    let ``TestFunction09b1_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=TestFunction09b2.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction09b2.exe"	# TestFunction09b2.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TestFunction09b2.fs"|])>]
    let ``TestFunction09b2_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=TestFunction09b3.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction09b3.exe"	# TestFunction09b3.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TestFunction09b3.fs"|])>]
    let ``TestFunction09b3_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=TestFunction09b4.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction09b4.exe"	# TestFunction09b4.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TestFunction09b4.fs"|])>]
    let ``TestFunction09b4_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=TestFunction10.fs  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction10.exe"	# TestFunction10.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TestFunction10.fs"|])>]
    let ``TestFunction10_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=TestFunction11.fs  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction11.exe"	# TestFunction11.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TestFunction11.fs"|])>]
    let ``TestFunction11_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=TestFunction12.fs  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction12.exe"	# TestFunction12.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TestFunction12.fs"|])>]
    let ``TestFunction12_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=TestFunction13.fs  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction13.exe"	# TestFunction13.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TestFunction13.fs"|])>]
    let ``TestFunction13_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=TestFunction14.fs  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction14.exe"	# TestFunction14.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TestFunction14.fs"|])>]
    let ``TestFunction14_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=TestFunction15.fs  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction15.exe"	# TestFunction15.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TestFunction15.fs"|])>]
    let ``TestFunction15_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=TestFunction16.fs  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction16.exe"	# TestFunction16.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TestFunction16.fs"|])>]
    let ``TestFunction16_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=TestFunction17.fs  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction17.exe"	# TestFunction17.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TestFunction17.fs"|])>]
    let ``TestFunction17_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=TestFunction18.fs  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction18.exe"	# TestFunction18.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TestFunction18.fs"|])>]
    let ``TestFunction18_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=TestFunction19.fs  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction19.exe"	# TestFunction19.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TestFunction19.fs"|])>]
    let ``TestFunction19_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=TestFunction20.fs  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction20.exe"	# TestFunction20.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TestFunction20.fs"|])>]
    let ``TestFunction20_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=TestFunction21.fs  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction21.exe"	# TestFunction21.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TestFunction21.fs"|])>]
    let ``TestFunction21_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=TestFunction22.fs    SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction22.exe"	# TestFunction22.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TestFunction22.fs"|])>]
    let ``TestFunction22_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=TestFunction22b.fs   SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction22b.exe"	# TestFunction22b.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TestFunction22b.fs"|])>]
    let ``TestFunction22b_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=TestFunction22c.fs   SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction22c.exe"	# TestFunction22c.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TestFunction22c.fs"|])>]
    let ``TestFunction22c_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=TestFunction22d.fs   SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction22d.exe"	# TestFunction22d.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TestFunction22d.fs"|])>]
    let ``TestFunction22d_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=TestFunction22e.fs   SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction22e.exe"	# TestFunction22e.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TestFunction22e.fs"|])>]
    let ``TestFunction22e_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=TestFunction22f.fs   SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction22f.exe"	# TestFunction22f.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TestFunction22f.fs"|])>]
    let ``TestFunction22f_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=TestFunction22g.fs   SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction22g.exe"	# TestFunction22g.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TestFunction22g.fs"|])>]
    let ``TestFunction22g_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=TestFunction22h.fs   SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction22h.exe"	# TestFunction22h.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TestFunction22h.fs"|])>]
    let ``TestFunction22h_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=TestFunction23.fs  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction23.exe"	# TestFunction23.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TestFunction23.fs"|])>]
    let ``TestFunction23_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=TestFunction24.fs   SCFLAGS="-g --optimize-" PEVER=/Exp_Fail COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction24.exe"	# TestFunction24.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TestFunction24.fs"|])>]
    let ``TestFunction24_fs`` compilation =
        compilation
        |> verifyCompilation

    // Verify IL 13043
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Verify13043.fs"|])>]
    let ``Verify13043_il`` compilation =
        compilation
        |> withDebug
        |> verifyCompilation

    // Verify Execution 13043 run it built not optimized with debug
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Verify13043.fs"|])>]
    let ``Verify13043_execution_noopt`` compilation =
        compilation
        |> withDebug
        |> verifyCompileAndRun
        |> shouldSucceed

    // Verify Execution 13043 --- run it built optimized no debug
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Verify13043.fs"|])>]
    let ``Verify13043_execution_opt`` compilation =
        compilation
        |> withOptions [ "--test:EmitFeeFeeAs100001"; "--nowarn:988"; "--nowarn:3370"]
        |> withOptimize
        |> withNoDebug
        |> asExe
        |> verifyCompileAndRun
        |> shouldSucceed
