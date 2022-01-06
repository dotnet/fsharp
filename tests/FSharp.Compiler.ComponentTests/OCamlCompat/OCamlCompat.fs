// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.OcamlCompat

open Xunit
open System.IO
open FSharp.Test.Compiler

module ``OCamlCompat test cases`` =

    [<Fact>]  //	SOURCE=E_IndentOff01.fs  COMPILE_ONLY=1 SCFLAGS="--warnaserror --test:ErrorRanges"				# E_IndentOff01.fs
    let ``OCamlCompat - --warnaserror --test:ErrorRanges E_IndentOff01.fs`` () =
        FSharp (loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__, "E_IndentOff01.fs")))
        |> asFsx
        |> withOptions ["--warnaserror"; "--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withErrorCode 0062
        |> withDiagnosticMessageMatches "This construct is for ML compatibility\. Consider using a file with extension '\.ml' or '\.mli' instead\. You can disable this warning by using '--mlcompatibility' or '--nowarn:62'\."


    [<Fact>]
    let ``OCamlCompat - --warnaserror"; "--mlcompatibility IndentOff02.fs`` () =
        FSharp (loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__, "IndentOff02.fs")))
        |> asFsx
        |> withOptions ["--warnaserror"; "--mlcompatibility"]
        |> typecheck
        |> shouldSucceed


    //<Expects status="warning" span="(4,1-4,14)" id="FS0062">This construct is for ML compatibility\. Consider using a file with extension '\.ml' or '\.mli' instead\. You can disable this warning by using '--mlcompatibility' or '--nowarn:62'\.$</Expects>
    [<Fact>]
    let ``OCamlCompat - --test:ErrorRanges W_IndentOff03.fs`` () =
        FSharp (loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__, "W_IndentOff03.fs")))
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withWarningCode 0062
        |> withDiagnosticMessageMatches "This construct is for ML compatibility\. Consider using a file with extension '\.ml' or '\.mli' instead\. You can disable this warning by using '--mlcompatibility' or '--nowarn:62'\."


    [<Fact>]    //NoMT	SOURCE=IndentOff04.fsx   COMPILE_ONLY=1 SCFLAGS="--warnaserror --mlcompatibility" FSIMODE=PIPE					# IndentOff04.fsx
    let ``OCamlCompat - --warnaserror --mlcompatibility IndentOff04.fsx`` () =
        FSharp (loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__, "IndentOff04.fsx")))
        |> asFsx
        |> withOptions ["--warnaserror"; " --mlcompatibility"]
        |> compile
        |> shouldFail
        |> withErrorCode 62
        |> withDiagnosticMessageMatches "This construct is for ML compatibility\. Consider using a file with extension '\.ml' or '\.mli' instead\. You can disable this warning by using '--mlcompatibility' or '--nowarn:62'\."


    [<Fact>]    //NoMT	SOURCE=W_IndentOff05.fsx COMPILE_ONLY=1 SCFLAGS="--test:ErrorRanges"              FSIMODE=PIPE					# W_IndentOff05.fsx
    let ``OCamlCompat - --test:ErrorRanges W_IndentOff05.fsx`` () =
        FSharp (loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__, "W_IndentOff05.fsx")))
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withWarningCode 0062
        |> withDiagnosticMessageMatches "This construct is for ML compatibility\. Consider using a file with extension '\.ml' or '\.mli' instead\. You can disable this warning by using '--mlcompatibility' or '--nowarn:62'\."


    [<Fact>]    //NoMT	SOURCE=E_IndentOff06.fsx COMPILE_ONLY=1 SCFLAGS="--warnaserror"                   FSIMODE=PIPE					# E_IndentOff06.fsx
    let ``OCamlCompat - --test:ErrorRanges E_IndentOff06.fsx`` () =
        FSharp (loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__, "E_IndentOff06.fsx")))
        |> asFsx
        |> withOptions ["--warnaserror"; "--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withErrorCode 0062
        |> withDiagnosticMessageMatches "This construct is for ML compatibility\. Consider using a file with extension '\.ml' or '\.mli' instead\. You can disable this warning by using '--mlcompatibility' or '--nowarn:62'\."


    [<Fact>]  //	SOURCE=E_mlExtension01.ml  COMPILE_ONLY=1 SCFLAGS="--warnaserror --test:ErrorRanges"				# E_mlExtension01.ml
    let ``OCamlCompat - E_mlExtension01.ml - --test:ErrorRanges`` () =
        FSharp (loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__, "E_mlExtension01.ml")))
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldSucceed


    [<Fact>]  //	SOURCE=mlExtension02.ml    COMPILE_ONLY=1 SCFLAGS="--warnaserror --mlcompatibility"				# mlExtension02.ml
    let ``OCamlCompat - mlExtension02.ml - --warnaserror --mlcompatibility`` () =
        FSharp (loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__, "mlExtension02.ml")))
        |> asFsx
        |> withOptions ["--warnaserror"; "--mlcompatibility"]
        |> compile
        |> shouldSucceed


    [<Fact>]  //	SOURCE=W_mlExtension03.ml  COMPILE_ONLY=1 SCFLAGS="--test:ErrorRanges"						# W_mlExtension03.ml
    let ``OCamlCompat - W_mlExtension03.ml - --test:ErrorRanges`` () =
        FSharp (loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__, "W_mlExtension03.ml")))
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldSucceed


    [<Fact>]  //	SOURCE=Hat01.fs  COMPILE_ONLY=1 SCFLAGS="--test:ErrorRanges"						# Hat01.fs
    let ``OCamlCompat - Hat01.fs - --warnaserror --mlcompatibility`` () =
        FSharp (loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__, "Hat01.fs")))
        |> asFsx
        |> withOptions ["--warnaserror"; "--mlcompatibility"]
        |> typecheck
        |> shouldSucceed


    [<Fact>]  //	SOURCE=W_Hat01.fs  COMPILE_ONLY=1 SCFLAGS="--test:ErrorRanges"						# W_Hat01.fs
    let ``OCamlCompat - W_Hat01.fs - --warnaserror --mlcompatibility`` () =
        FSharp (loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__, "W_Hat01.fs")))
        |> asExe
        |> withOptions ["--test:ErrorRanges"; "--mlcompatibility"]
        |> compile
        |> shouldSucceed


    [<Fact>]  //	SOURCE=NoParensInLet01.fs  COMPILE_ONLY=1 SCFLAGS="--test:ErrorRanges"						# NoParensInLet01.fs
    let ``OCamlCompat - NoParensInLet01.fs`` () =
        FSharp (loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__, "NoParensInLet01.fs")))
        |> asExe
        |> compile
        |> shouldSucceed


    [<Fact>]  //	SOURCE=W_MultiArgumentGenericType.fs					# W_MultiArgumentGenericType.fs
    let ``OCamlCompat - W_MultiArgumentGenericType.fs`` () =
        FSharp (loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__, "W_MultiArgumentGenericType.fs")))
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withWarningCode 62
        |> withDiagnosticMessageMatches "This construct is for ML compatibility\. The syntax '\(typ,\.\.\.,typ\) ident' is not used in F# code. Consider using 'ident<typ,\.\.\.,typ>' instead. You can disable this warning by using '--mlcompatibility' or '--nowarn:62'\."


    [<Fact>]  //	SOURCE=OCamlStyleArrayIndexing.fs SCFLAGS="--mlcompatibility"		# OCamlStyleArrayIndexing.fs
    let ``OCamlCompat - OCamlStyleArrayIndexing.fs --mlcompatibility`` () =
        FSharp (loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__, "OCamlStyleArrayIndexing.fs")))
        |> asExe
        |> withOptions ["--mlcompatibility"]
        |> compile
        |> shouldSucceed
