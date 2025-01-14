namespace EmittedIL.RealInternalSignature

open Xunit
open System.IO
open FSharp.Test
open FSharp.Test.Compiler

module TestFunctions =

    let verifyCore compilation =
        compilation
        |> withOptions [ "--test:EmitFeeFeeAs100001"; "--nowarn:988"; "--nowarn:3370"]
        |> asExe
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> ignoreWarnings

    let verifyCompileAndRun compilation =
        compilation
        |> verifyCore
        |> verifyILBaseline
        |> compileAndRun

    let verifyCompilation compilation =
        compilation
        |> verifyCore
        |> verifyILBaseline

    //SOURCE=TestFunction01.fs   SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction01.exe"	# TestFunction01.fs
    [<Theory; FileInlineData("TestFunction01.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.Both)>]
    let ``TestFunction01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    //SOURCE=TestFunction02.fs   SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction02.exe"	# TestFunction02.fs
    [<Theory; FileInlineData("TestFunction02.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.Both)>]
    let ``TestFunction02_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    //SOURCE=TestFunction03.fs   SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction03.exe"	# TestFunction03.fs -
    [<Theory; FileInlineData("TestFunction03.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.Both)>]
    let ``TestFunction03_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    //SOURCE=TestFunction03b.fs  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction03b.exe"	# TestFunction03b.fs -
    [<Theory; FileInlineData("TestFunction03b.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.Both)>]
    let ``TestFunction03b_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    //SOURCE=TestFunction03c.fs  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction03c.exe"	# TestFunction03c.fs -
    [<Theory; FileInlineData("TestFunction03c.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.Both)>]
    let ``TestFunction03c_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    //SOURCE=TestFunction04.fs   SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction04.exe"	# TestFunction04.fs
    [<Theory; FileInlineData("TestFunction04.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.Both)>]
    let ``TestFunction04_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    //SOURCE=TestFunction05.fs   SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction05.exe"	# TestFunction05.fs
    [<Theory; FileInlineData("TestFunction05.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.Both)>]
    let ``TestFunction05_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    //SOURCE=TestFunction06.fs   SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction06.exe"	# TestFunction06.fs
    [<Theory; FileInlineData("TestFunction06.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.Both)>]
    let ``TestFunction06_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    //SOURCE=TestFunction07.fs   SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction07.exe"	# TestFunction07.fs
    [<Theory; FileInlineData("TestFunction07.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.Both)>]
    let ``TestFunction07_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    //SOURCE=TestFunction08.fs   SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction08.exe"	# TestFunction08.fs
    [<Theory; FileInlineData("TestFunction08.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.Both)>]
    let ``TestFunction08_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    //SOURCE=TestFunction09.fs   SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction09.exe"	# TestFunction09.fs
    [<Theory; FileInlineData("TestFunction09.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.Both)>]
    let ``TestFunction09_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    //SOURCE=TestFunction09b.fs  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction09b.exe"	# TestFunction09b.fs
    [<Theory; FileInlineData("TestFunction09b.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.Both)>]
    let ``TestFunction09b_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    //SOURCE=TestFunction09b1.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction09b1.exe"	# TestFunction09b1.fs
    [<Theory; FileInlineData("TestFunction09b1.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.Both)>]
    let ``TestFunction09b1_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    //SOURCE=TestFunction09b2.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction09b2.exe"	# TestFunction09b2.fs
    [<Theory; FileInlineData("TestFunction09b2.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.Both)>]
    let ``TestFunction09b2_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    //SOURCE=TestFunction09b3.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction09b3.exe"	# TestFunction09b3.fs
    [<Theory; FileInlineData("TestFunction09b3.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.Both)>]
    let ``TestFunction09b3_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    //SOURCE=TestFunction09b4.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction09b4.exe"	# TestFunction09b4.fs
    [<Theory; FileInlineData("TestFunction09b4.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.Both)>]
    let ``TestFunction09b4_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    //SOURCE=TestFunction10.fs  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction10.exe"	# TestFunction10.fs -
    [<Theory; FileInlineData("TestFunction10.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.Both)>]
    let ``TestFunction10_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    //SOURCE=TestFunction11.fs  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction11.exe"	# TestFunction11.fs
    [<Theory; FileInlineData("TestFunction11.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.Both)>]
    let ``TestFunction11_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    //SOURCE=TestFunction12.fs  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction12.exe"	# TestFunction12.fs
    [<Theory; FileInlineData("TestFunction12.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.Both)>]
    let ``TestFunction12_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    //SOURCE=TestFunction13.fs  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction13.exe"	# TestFunction13.fs -
    [<Theory; FileInlineData("TestFunction13.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.Both)>]
    let ``TestFunction13_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    //SOURCE=TestFunction14.fs  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction14.exe"	# TestFunction14.fs -
    [<Theory; FileInlineData("TestFunction14.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.Both)>]
    let ``TestFunction14_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    //SOURCE=TestFunction15.fs  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction15.exe"	# TestFunction15.fs
    [<Theory; FileInlineData("TestFunction15.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.Both)>]
    let ``TestFunction15_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    //SOURCE=TestFunction16.fs  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction16.exe"	# TestFunction16.fs -
    [<Theory; FileInlineData("TestFunction16.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.Both)>]
    let ``TestFunction16_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    //SOURCE=TestFunction17.fs  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction17.exe"	# TestFunction17.fs -
    [<Theory; FileInlineData("TestFunction17.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.Both)>]
    let ``TestFunction17_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    //SOURCE=TestFunction18.fs  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction18.exe"	# TestFunction18.fs
    [<Theory; FileInlineData("TestFunction18.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.Both)>]
    let ``TestFunction18_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    //SOURCE=TestFunction19.fs  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction19.exe"	# TestFunction19.fs -
    [<Theory; FileInlineData("TestFunction19.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.Both)>]
    let ``TestFunction19_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    //SOURCE=TestFunction20.fs  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction20.exe"	# TestFunction20.fs -
    [<Theory; FileInlineData("TestFunction20.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.Both)>]
    let ``TestFunction20_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    //SOURCE=TestFunction21.fs  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction21.exe"	# TestFunction21.fs -
    [<Theory; FileInlineData("TestFunction21.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.Both)>]
    let ``TestFunction21_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    //SOURCE=TestFunction22.fs    SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction22.exe"	# TestFunction22.fs
    [<Theory; FileInlineData("TestFunction22.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.Both)>]
    let ``TestFunction22_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    //SOURCE=TestFunction22b.fs   SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction22b.exe"	# TestFunction22b.fs
    [<Theory; FileInlineData("TestFunction22b.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.Both)>]
    let ``TestFunction22b_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    //SOURCE=TestFunction22c.fs   SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction22c.exe"	# TestFunction22c.fs
    [<Theory; FileInlineData("TestFunction22c.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.Both)>]
    let ``TestFunction22c_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    //SOURCE=TestFunction22d.fs   SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction22d.exe"	# TestFunction22d.fs
    [<Theory; FileInlineData("TestFunction22d.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.Both)>]
    let ``TestFunction22d_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    //SOURCE=TestFunction22e.fs   SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction22e.exe"	# TestFunction22e.fs
    [<Theory; FileInlineData("TestFunction22e.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.Both)>]
    let ``TestFunction22e_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    //SOURCE=TestFunction22f.fs   SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction22f.exe"	# TestFunction22f.fs
    [<Theory; FileInlineData("TestFunction22f.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.Both)>]
    let ``TestFunction22f_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    //SOURCE=TestFunction22g.fs   SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction22g.exe"	# TestFunction22g.fs
    [<Theory; FileInlineData("TestFunction22g.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.Both)>]
    let ``TestFunction22g_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    //SOURCE=TestFunction22h.fs   SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction22h.exe"	# TestFunction22h.fs -
    [<Theory; FileInlineData("TestFunction22h.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.Both)>]
    let ``TestFunction22h_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    //SOURCE=TestFunction22h.fs   SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction22h.exe"	# TestFunction22h.fs -

    //SOURCE=TestFunction23.fs  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction23.exe"	# TestFunction23.fs -
    [<Theory; FileInlineData("TestFunction23.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.Both)>]
    let ``TestFunction23_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    //SOURCE=TestFunction24.fs   SCFLAGS="-g --optimize-" PEVER=/Exp_Fail COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TestFunction24.exe"	# TestFunction24.fs -
    [<Theory; FileInlineData("TestFunction24.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.Both)>]
    let ``TestFunction24_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersion70
        |> verifyCompileAndRun

    // Verify Execution 13043 run it built not optimized with debug
    [<Theory; FileInlineData("Verify13043.fs", Realsig=BooleanOptions.Both, Optimize=BooleanOptions.Both)>]
    let ``Verify13043_CompileAndRun`` compilation =
        compilation
        |> getCompilation
        |> withDebug
        |> withRealInternalSignatureOff
        |> verifyCompileAndRun
