namespace EmittedIL

open Xunit
open System.IO
open FSharp.Test
open FSharp.Test.Compiler

module SteppingMatch =

    let verifyCompilation compilation =
        compilation
        |> withOptions [ "--test:EmitFeeFeeAs100001" ]
        |> asExe
        |> withNoOptimize
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> ignoreWarnings
        |> verifyILBaseline


    // SOURCE=SteppingMatch01.fs                SCFLAGS="-a -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd SteppingMatch01.dll"	# SteppingMatch01.fs
    [<Theory; FileInlineData("SteppingMatch01.fs")>]
    let ``SteppingMatch01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=SteppingMatch02.fs                SCFLAGS="-a -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd SteppingMatch02.dll"	# SteppingMatch02.fs
    [<Theory; FileInlineData("SteppingMatch02.fs")>]
    let ``SteppingMatch02_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=SteppingMatch03.fs                SCFLAGS="-a -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd SteppingMatch03.dll"	# SteppingMatch03.fs
    [<Theory; FileInlineData("SteppingMatch03.fs")>]
    let ``SteppingMatch03_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=SteppingMatch04.fs                SCFLAGS="-a -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd SteppingMatch04.dll"	# SteppingMatch04.fs
    [<Theory; FileInlineData("SteppingMatch04.fs")>]
    let ``SteppingMatch04_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=SteppingMatch05.fs                SCFLAGS="-a -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd SteppingMatch05.dll"	# SteppingMatch05.fs
    [<Theory; FileInlineData("SteppingMatch05.fs")>]
    let ``SteppingMatch05_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=SteppingMatch06.fs                SCFLAGS="-a -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd SteppingMatch06.dll"	# SteppingMatch06.fs -
    [<Theory; FileInlineData("SteppingMatch06.fs")>]
    let ``SteppingMatch06_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=SteppingMatch07.fs                SCFLAGS="-a -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd SteppingMatch07.dll"	# SteppingMatch07.fs -
    [<Theory; FileInlineData("SteppingMatch07.fs")>]
    let ``SteppingMatch07_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=SteppingMatch08.fs                SCFLAGS="-a -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd SteppingMatch08.dll"	# SteppingMatch08.fs
    [<Theory; FileInlineData("SteppingMatch08.fs")>]
    let ``SteppingMatch08_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=SteppingMatch09.fs                SCFLAGS="-a -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd SteppingMatch09.dll"	# SteppingMatch09.fs
    [<Theory; FileInlineData("SteppingMatch09.fs")>]
    let ``SteppingMatch09_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation
