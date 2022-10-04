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
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 62, Line 4, Col 1, Line 4, Col 14, """This construct is deprecated. The use of '#light "off"' or '#indent "off"' was deprecated in F# 2.0 and is no longer supported. You can enable this feature by using '--langversion:5.0' and '--mlcompatibility'.""")
        ]


    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"IndentOff02.fs"|])>]
    let ``IndentOff02_fs  --warnaserror"; "--mlcompatibility`` compilation =
        compilation
        |> asFsx
        |> withOcamlCompat
        |> withLangVersion50
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
        |> withDiagnostics [
            (Error 62, Line 4, Col 1, Line 4, Col 14, """This construct is deprecated. The use of '#light "off"' or '#indent "off"' was deprecated in F# 2.0 and is no longer supported. You can enable this feature by using '--langversion:5.0' and '--mlcompatibility'.""")
        ]


    //NoMT	SOURCE=IndentOff04.fsx   COMPILE_ONLY=1 SCFLAGS="--warnaserror --mlcompatibility" FSIMODE=PIPE					# IndentOff04.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"IndentOff04.fsx"|])>]
    let ``IndentOff04_fsx  --warnaserror --mlcompatibility`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> withOcamlCompat
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 62, Line 3, Col 1, Line 3, Col 14, """This construct is deprecated. The use of '#light "off"' or '#indent "off"' was deprecated in F# 2.0 and is no longer supported. You can enable this feature by using '--langversion:5.0' and '--mlcompatibility'.""")
        ]


    //NoMT	SOURCE=W_IndentOff05.fsx COMPILE_ONLY=1 SCFLAGS="--test:ErrorRanges"              FSIMODE=PIPE					# W_IndentOff05.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"W_IndentOff05.fsx"|])>]
    let ``W_IndentOff05_fsx  --test:ErrorRanges`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 62, Line 3, Col 1, Line 3, Col 14, """This construct is deprecated. The use of '#light "off"' or '#indent "off"' was deprecated in F# 2.0 and is no longer supported. You can enable this feature by using '--langversion:5.0' and '--mlcompatibility'.""")
        ]


    //NoMT	SOURCE=E_IndentOff06.fsx COMPILE_ONLY=1 SCFLAGS="--warnaserror"                   FSIMODE=PIPE					# E_IndentOff06.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_IndentOff06.fsx"|])>]
    let ``E_IndentOff06_fsx  --test:ErrorRanges`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 62, Line 3, Col 1, Line 3, Col 14, """This construct is deprecated. The use of '#light "off"' or '#indent "off"' was deprecated in F# 2.0 and is no longer supported. You can enable this feature by using '--langversion:5.0' and '--mlcompatibility'.""")
        ]


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
        |> withLangVersion50
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
        |> withLangVersion50
        |> typecheck
        |> shouldSucceed


    //	SOURCE=W_Hat01.fs  COMPILE_ONLY=1 SCFLAGS="--test:ErrorRanges"						# W_Hat01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"W_Hat01.fs"|])>]
    let ``W_Hat01_fs  --warnaserror --mlcompatibility`` compilation =
        compilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"; "--mlcompatibility"]
        |> withLangVersion50
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
       |> shouldFail
        |> withDiagnostics [
            (Error 62, Line 10, Col 19, Line 10, Col 48, """This construct is deprecated. The use of multiple parenthesized type parameters before a generic type name such as '(int, int) Map' was deprecated in F# 2.0 and is no longer supported. You can enable this feature by using '--langversion:5.0' and '--mlcompatibility'.""")
        ]


    //	SOURCE=OCamlStyleArrayIndexing.fs SCFLAGS="--mlcompatibility"		# OCamlStyleArrayIndexing.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"OCamlStyleArrayIndexing.fs"|])>]
    let ``OCamlStyleArrayIndexing_fs  --mlcompatibility`` compilation =
        compilation
        |> asExe
        |> withOcamlCompat
        |> withLangVersion50
        |> compile
        |> shouldSucceed
