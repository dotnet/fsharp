// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsc

open FSharp.Test
open FSharp.Test.Compiler
open Xunit

module Checked =

    //  SOURCE=unchecked01.fs       # fsc-default
    [<Theory; FileInlineData("unchecked01.fs")>]
    let ``fsc-unchecked - unchecked01_fs`` compilation =
       compilation
        |> getCompilation 
        |> asFs
        |> compile
        |> shouldSucceed

    //  SOURCE=checked01.fs   SCFLAGS="--checked"       # fsc-checked
    [<Theory; FileInlineData("unchecked01.fs")>]
    let ``fsc-unchecked - unchecked01_fs --checked`` compilation =
       compilation
        |> getCompilation 
        |> asFs
        |> withOptions["--checked"]
        |> compile
        |> shouldSucceed

    //  SOURCE=checked01.fs   SCFLAGS="--checked+"      # fsc-checked+
    [<Theory; FileInlineData("unchecked01.fs")>]
    let ``fsc-unchecked - unchecked01_fs --checked+`` compilation =
       compilation
        |> getCompilation 
        |> asFs
        |> withOptions["--checked+"]
        |> compile
        |> shouldSucceed

    //  SOURCE=unchecked01.fs SCFLAGS="--checked-"      # fsc-checked-
    [<Theory; FileInlineData("unchecked01.fs")>]
    let ``fsc-unchecked - unchecked01_fs --checked-`` compilation =
       compilation
        |> getCompilation 
        |> asFs
        |> withOptions["--checked-"]
        |> compile
        |> shouldSucceed

    //  SOURCE=unchecked01.fs SCFLAGS="--checked-"      # fsc-checked-
    [<Theory; FileInlineData("unchecked01.fs")>]
    let ``fsi-unchecked - unchecked01_fs --checked-`` compilation =
       compilation
        |> getCompilation 
        |> asFsx
        |> withOptions["--checked-"]
        |> compile
        |> shouldSucceed

    //  SOURCE=checked01.fs   SCFLAGS="--checked"  FSIMODE=EXEC COMPILE_ONLY=1  # fsi-checked
    [<Theory; FileInlineData("checked01.fs")>]
    let ``fsi-checked - checked01_fs --checked`` compilation =
       compilation
        |> getCompilation 
        |> asFsx
        |> withOptions["--checked"]
        |> compile
        |> shouldSucceed

    //  SOURCE=checked01.fs   SCFLAGS="--checked+" FSIMODE=EXEC COMPILE_ONLY=1  # fsi-checked+
    [<Theory; FileInlineData("checked01.fs")>]
    let ``fsi-checked - checked01_fs --checked+`` compilation =
       compilation
        |> getCompilation 
        |> asFsx
        |> withOptions["--checked+"]
        |> compile
        |> shouldSucceed

    //  SOURCE=checked01.fs   SCFLAGS="--checked-" FSIMODE=EXEC COMPILE_ONLY=1  # fsi-checked+
    [<Theory; FileInlineData("checked01.fs")>]
    let ``fsi-checked - checked01_fs --checked-`` compilation =
       compilation
        |> getCompilation 
        |> asFsx
        |> withOptions["--checked-"]
        |> compile
        |> shouldSucceed


    //# Last one wins

    //  SOURCE=checked01.fs   SCFLAGS="--checked  --checked+"   # fsc-checkedchecked+
    [<Theory; FileInlineData("checked01.fs")>]
    let ``fsc-checked - checked01_fs --checked  --checked+`` compilation =
       compilation
        |> getCompilation 
        |> asFs
        |> withOptions["--checked"; "--checked+"]
        |> compile
        |> shouldSucceed

    //  SOURCE=checked01.fs   SCFLAGS="--checked- --checked+"   # fsc-checked-checked+
    [<Theory; FileInlineData("checked01.fs")>]
    let ``fsc-checked - checked01_fs --checked- --checked+`` compilation =
       compilation
        |> getCompilation 
        |> asFs
        |> withOptions["--checked-"; "--checked+"]
        |> compile
        |> shouldSucceed

    //  SOURCE=unchecked01.fs SCFLAGS="--checked+ --checked-"   # fsc-checked+checked-
    [<Theory; FileInlineData("unchecked01.fs")>]
    let ``fsc-checked - unchecked01_fs --checked+ --checked-`` compilation =
       compilation
        |> getCompilation 
        |> asFs
        |> withOptions["--checked+"; "--checked-"]
        |> compile
        |> shouldSucceed

    //  SOURCE=checked01.fs   SCFLAGS="--checked  --checked+"   # fsc-checkedchecked+
    [<Theory; FileInlineData("checked01.fs")>]
    let ``fsi-checked - checked01_fs --checked  --checked+`` compilation =
       compilation
        |> getCompilation 
        |> asFsx
        |> withOptions["--checked"; "--checked+"]
        |> compile
        |> shouldSucceed

    //  SOURCE=checked01.fs   SCFLAGS="--checked- --checked+"   # fsc-checked-checked+
    [<Theory; FileInlineData("checked01.fs")>]
    let ``fsi-checked - checked01_fs --checked- --checked+`` compilation =
       compilation
        |> getCompilation 
        |> asFsx
        |> withOptions["--checked-"; "--checked+"]
        |> compile
        |> shouldSucceed

    //  SOURCE=unchecked01.fs SCFLAGS="--checked+ --checked-"   # fsc-checked+checked-
    [<Theory; FileInlineData("unchecked01.fs")>]
    let ``fsi-checked - unchecked01_fs --checked+ --checked-`` compilation =
       compilation
        |> getCompilation 
        |> asFsx
        |> withOptions["--checked+"; "--checked-"]
        |> compile
        |> shouldSucceed

    //# Unrecognized argument
    //  SOURCE=unrecogarg.fs  SCFLAGS="--Checked"   # fsc--Checked
    [<Theory; FileInlineData("unrecogarg.fs")>]
    let ``fsc-checked - unchecked01_fs Checked`` compilation =
       compilation
        |> getCompilation 
        |> asFs
        |> withOptions["--Checked"]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 243, Line 0, Col 1, Line 0, Col 1, "Unrecognized option: '--Checked'. Use '--help' to learn about recognized command line options.")
        ]

    //  SOURCE=unrecogarg.fs  SCFLAGS="--checked*"  # fsc--checked*
    [<Theory; FileInlineData("unrecogarg.fs")>]
    let ``fsc-checked - unchecked01_fs --checked-star`` compilation =
       compilation
        |> getCompilation 
        |> asFs
        |> withOptions["--checked*"]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 243, Line 0, Col 1, Line 0, Col 1, "Unrecognized option: '--checked*'. Use '--help' to learn about recognized command line options.")
        ]

    //  SOURCE=unrecogarg.fs  SCFLAGS="--checked*"  # fsc--checked*
    [<Theory; FileInlineData( "UncheckedDefaultOf01.fs")>]
    let ``fsc-checked - UncheckedDefaultOf01`` compilation =
       compilation
        |> getCompilation 
        |> asFs
        |> asExe
        |> compile
        |> shouldSucceed
