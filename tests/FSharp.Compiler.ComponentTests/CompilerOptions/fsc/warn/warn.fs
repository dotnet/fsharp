// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsc

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module CompilerWarningLevel =

    [<Theory; FileInlineData("warn_level0.fs")>]
    let ``warn_level0_fs --warn:0`` compilation =
        compilation
        |> getCompilation 
        |> withLangVersion80
        |> asExe
        |> withOptions ["--warn:0"]
        |> compileAndRun
        |> shouldSucceed
        |> withDiagnostics []

    [<Theory;FileInlineData("warn_level1.fs")>]
    let ``warn_level1_fs --warn:1 --warnaserror:52`` compilation =
        compilation
        |> getCompilation 
        |> asExe
        |> withOptions ["--warn:1"; "--warnaserror:52"]
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("warn_level2.fs")>]
    let ``warn_level2_fs --warn:2 --warnaserror:52`` compilation =
        compilation
        |> getCompilation 
        |> asExe
        |> withOptions ["--warn:2"; "--warnaserror:52"]
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("warn_level3.fs")>]
    let ``warn_level3_fs --warn:3 --warnaserror:52`` compilation =
        compilation
        |> getCompilation 
        |> asExe
        |> withOptions ["--warn:3"; "--warnaserror:52"]
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("warn_level4.fs")>]
    let ``warn_level4_fs --warn:4 --warnaserror:52`` compilation =
        compilation
        |> getCompilation 
        |> asExe
        |> withOptions ["--warn:4"; "--warnaserror:52"]
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("warn_level5.fs")>]
    let ``warn_level5_fs --warn:5 --warnaserror:52`` compilation =
        compilation
        |> getCompilation 
        |> asExe
        |> withOptions ["--warn:5"; "--warnaserror:52"]
        |> compile
        |> shouldFail
        |> withErrorCode 0052
        |> withDiagnosticMessageMatches "The value has been copied to ensure the original is not mutated by this operation or because the copy is implicit when returning a struct from a member and another member is then accessed$"
        |> ignore

    [<Theory; FileInlineData("warn_level5.fs")>]
    let ``warn_level5_fs --warn:5`` compilation =
        compilation
        |> getCompilation 
        |> asExe
        |> withOptions ["--warn:5"]
        |> compile
        |> shouldFail
        |> withWarningCode 0052
        |> withDiagnosticMessageMatches "The value has been copied to ensure the original is not mutated by this operation or because the copy is implicit when returning a struct from a member and another member is then accessed$"
        |> ignore

#if NETSTANDARD 
// This test works with KeyValuePair, which is not  a 'readonly struct' in net472
    [<Theory; FileInlineData("nowarn_readonlystruct.fs")>]
    let ``no error 52 with readonly struct`` compilation =
        compilation
        |> getCompilation 
        |> asExe
        |> withOptions ["--warn:5"; "--warnaserror:52"]
        |> compile
        |> shouldSucceed
        |> ignore
#endif

    [<Theory; FileInlineData("warn_level6.fs")>]
    let ``warn_level6_fs --warn:6`` compilation =
        compilation
        |> getCompilation 
        |> asExe
        |> withOptions ["--warn:6"]
        |> compile
        |> shouldFail
        |> withErrorCode 1050
        |> withDiagnosticMessageMatches "Invalid warning level '6'"
        |> ignore

    [<Theory; FileInlineData("nowarn.fs")>]
    let ``nowarn_fs --warnaserror`` compilation =
        compilation
        |> getCompilation 
        |> asExe
        |> withOptions ["--warnaserror"]
        |> compileAndRun
        |> shouldSucceed
        |> ignore

    [<Theory; FileInlineData("warn40.fs")>]
    let ``warn40_fs`` compilation =
        compilation
        |> getCompilation 
        |> asExe
        |> compile
        |> shouldFail
        |> withWarningCodes [21; 40]
        |> withWarningMessages [
            """This recursive use will be checked for initialization-soundness at runtime. This warning is usually harmless, and may be suppressed by using '#nowarn "21"' or '--nowarn:21'."""
            """This and other recursive references to the object(s) being defined will be checked for initialization-soundness at runtime through the use of a delayed reference. This is because you are defining one or more recursive objects, rather than recursive functions. This warning may be suppressed by using '#nowarn "40"' or '--nowarn:40'."""
            ]
        |> ignore

    [<Theory; FileInlineData("warn40.fs")>]
    let ``warn40_fs --warnaserror`` compilation =
        compilation
        |> getCompilation 
        |> asExe
        |> withOptions ["--warnaserror"]
        |> compile
        |> shouldFail
        |> withErrorCodes [21; 40]
        |> withErrorMessages [
            """This recursive use will be checked for initialization-soundness at runtime. This warning is usually harmless, and may be suppressed by using '#nowarn "21"' or '--nowarn:21'."""
            """This and other recursive references to the object(s) being defined will be checked for initialization-soundness at runtime through the use of a delayed reference. This is because you are defining one or more recursive objects, rather than recursive functions. This warning may be suppressed by using '#nowarn "40"' or '--nowarn:40'."""
            ]
        |> ignore

    [<Theory; FileInlineData("warn40.fs")>]
    let ``warn40_fs --nowarn:40;21`` compilation =
        compilation
        |> getCompilation 
        |> asExe
        |> withOptions ["--nowarn:40;21"]
        |> compileAndRun
        |> shouldSucceed
        |> ignore

    [<Theory; FileInlineData("warn40.fs")>]
    let ``warn40_fs --nowarn:NU0000;FS40;NU0001`` compilation =
        compilation
        |> getCompilation 
        |> asExe
        |> withOptions ["--nowarn:NU0000;FS40;NU0001;FS21"]
        |> compileAndRun
        |> shouldSucceed
        |> ignore

    [<Theory; FileInlineData("warn40.fs")>]
    let ``warn40_fs --nowarn:FS0040`` compilation =
        compilation
        |> getCompilation 
        |> asExe
        |> withOptions ["--nowarn:FS0040"; "--nowarn:FS0021"]
        |> compileAndRun
        |> shouldSucceed
        |> ignore

        //	SOURCE=nowarn_with_warnaserror01.fs SCFLAGS="--warnaserror --warn:4"   COMPILE_ONLY=1	# nowarn_with_warnaserror01.fs
        //	SOURCE=nowarn_with_warnaserror02.fs SCFLAGS="--warnaserror --warn:4"   COMPILE_ONLY=1	# nowarn_with_warnaserror02.fs
        //	SOURCE=nowarn_with_warnaserror03.fs SCFLAGS="--warnaserror --warn:4"   COMPILE_ONLY=1 	# nowarn_with_warnaserror03.fs
        //	SOURCE=nowarn_with_warnaserror01.fs SCFLAGS="--warnaserror:FS0040  --warn:4"   COMPILE_ONLY=1	# nowarn_with_warnaserror01a.fs
        //	SOURCE=nowarn_with_warnaserror02.fs SCFLAGS="--warnaserror:FS0040  --warn:4"   COMPILE_ONLY=1	# nowarn_with_warnaserror02a.fs
        //	SOURCE=nowarn_with_warnaserror03.fs SCFLAGS="--warnaserror:FS0040  --warn:4"   COMPILE_ONLY=1 	# nowarn_with_warnaserror03a.fs
