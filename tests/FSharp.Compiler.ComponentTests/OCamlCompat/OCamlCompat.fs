// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.OcamlCompat

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ``OCamlCompat test cases`` =

    //	SOURCE=E_IndentOff01.fs  COMPILE_ONLY=1 SCFLAGS="--warnaserror --test:ErrorRanges"				# E_IndentOff01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_IndentOff01.fs"|])>]
    let ``E_IndentOff01_fs  --warnaserror --test:ErrorRanges`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--warnaserror"; "--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withErrorCode 0062
        |> withDiagnosticMessageMatches "This construct is for ML compatibility\. Consider using a file with extension '\.ml' or '\.mli' instead\. You can disable this warning by using '--mlcompatibility' or '--nowarn:62'\."


    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"IndentOff02.fs"|])>]
    let ``IndentOff02_fs  --warnaserror"; "--mlcompatibility`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--warnaserror"; "--mlcompatibility"]
        |> typecheck
        |> shouldSucceed


    //<Expects status="warning" span="(4,1-4,14)" id="FS0062">This construct is for ML compatibility\. Consider using a file with extension '\.ml' or '\.mli' instead\. You can disable this warning by using '--mlcompatibility' or '--nowarn:62'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"W_IndentOff03.fs"|])>]
    let ``W_IndentOff03_fs  --test:ErrorRanges`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withWarningCode 0062
        |> withDiagnosticMessageMatches "This construct is for ML compatibility\. Consider using a file with extension '\.ml' or '\.mli' instead\. You can disable this warning by using '--mlcompatibility' or '--nowarn:62'\."


    //NoMT	SOURCE=IndentOff04.fsx   COMPILE_ONLY=1 SCFLAGS="--warnaserror --mlcompatibility" FSIMODE=PIPE					# IndentOff04.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"IndentOff04.fsx"|])>]
    let ``IndentOff04_fsx  --warnaserror --mlcompatibility`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--warnaserror"; " --mlcompatibility"]
        |> compile
        |> shouldFail
        |> withErrorCode 62
        |> withDiagnosticMessageMatches "This construct is for ML compatibility\. Consider using a file with extension '\.ml' or '\.mli' instead\. You can disable this warning by using '--mlcompatibility' or '--nowarn:62'\."


    //NoMT	SOURCE=W_IndentOff05.fsx COMPILE_ONLY=1 SCFLAGS="--test:ErrorRanges"              FSIMODE=PIPE					# W_IndentOff05.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"W_IndentOff05.fsx"|])>]
    let ``W_IndentOff05_fsx  --test:ErrorRanges`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withWarningCode 0062
        |> withDiagnosticMessageMatches "This construct is for ML compatibility\. Consider using a file with extension '\.ml' or '\.mli' instead\. You can disable this warning by using '--mlcompatibility' or '--nowarn:62'\."


    //NoMT	SOURCE=E_IndentOff06.fsx COMPILE_ONLY=1 SCFLAGS="--warnaserror"                   FSIMODE=PIPE					# E_IndentOff06.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_IndentOff06.fsx"|])>]
    let ``E_IndentOff06_fsx  --test:ErrorRanges`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--warnaserror"; "--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withErrorCode 0062
        |> withDiagnosticMessageMatches "This construct is for ML compatibility\. Consider using a file with extension '\.ml' or '\.mli' instead\. You can disable this warning by using '--mlcompatibility' or '--nowarn:62'\."


    //	SOURCE=E_mlExtension01.ml  COMPILE_ONLY=1 SCFLAGS="--warnaserror --test:ErrorRanges"				# E_mlExtension01.ml
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_mlExtension01.ml"|])>]
    let ``E_mlExtension01.ml --test:ErrorRanges`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldSucceed

    //	SOURCE=mlExtension02.ml    COMPILE_ONLY=1 SCFLAGS="--warnaserror --mlcompatibility"				# mlExtension02.ml
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_mlExtension01.ml"|])>]
    let ``mlExtension02_ml  --warnaserror --mlcompatibility`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--warnaserror"; "--mlcompatibility"]
        |> compile
        |> shouldSucceed


    //	SOURCE=W_mlExtension03.ml  COMPILE_ONLY=1 SCFLAGS="--test:ErrorRanges"						# W_mlExtension03.ml
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"W_mlExtension03.ml"|])>]
    let `` W_mlExtension03_ml  --test:ErrorRanges`` compilation =
        compilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldSucceed


    //	SOURCE=Hat01.fs  COMPILE_ONLY=1 SCFLAGS="--test:ErrorRanges"						# Hat01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Hat01.fs"|])>]
    let ``Hat01_fs  --warnaserror --mlcompatibility`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--warnaserror"; "--mlcompatibility"]
        |> typecheck
        |> shouldSucceed


    //	SOURCE=W_Hat01.fs  COMPILE_ONLY=1 SCFLAGS="--test:ErrorRanges"						# W_Hat01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"W_Hat01.fs"|])>]
    let ``W_Hat01_fs  --warnaserror --mlcompatibility`` compilation =
        compilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"; "--mlcompatibility"]
        |> compile
        |> shouldSucceed


    //	SOURCE=NoParensInLet01.fs  COMPILE_ONLY=1 SCFLAGS="--test:ErrorRanges"						# NoParensInLet01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NoParensInLet01.fs"|])>]
    let ``NoParensInLet01_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldSucceed


    //	SOURCE=W_MultiArgumentGenericType.fs					# W_MultiArgumentGenericType.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"W_MultiArgumentGenericType.fs"|])>]
    let ``W_MultiArgumentGenericType.fs``compilation =
        compilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withWarningCode 62
        |> withDiagnosticMessageMatches "This construct is for ML compatibility\. The syntax '\(typ,\.\.\.,typ\) ident' is not used in F# code. Consider using 'ident<typ,\.\.\.,typ>' instead. You can disable this warning by using '--mlcompatibility' or '--nowarn:62'\."


    //	SOURCE=OCamlStyleArrayIndexing.fs SCFLAGS="--mlcompatibility"		# OCamlStyleArrayIndexing.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"OCamlStyleArrayIndexing.fs"|])>]
    let ``OCamlStyleArrayIndexing_fs  --mlcompatibility`` compilation =
        compilation
        |> asExe
        |> withOptions ["--mlcompatibility"]
        |> compile
        |> shouldSucceed
