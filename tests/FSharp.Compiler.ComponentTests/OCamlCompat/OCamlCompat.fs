// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.OCamlCompat

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module OCamlCompat =

    // This test was automatically generated (moved from FSharpQA suite - OCamlCompat)
    //<Expects status="error" span="(4,1-4,14)" id="FS0062">This construct is for ML compatibility\. Consider using a file with extension '\.ml' or '\.mli' instead\. You can disable this warning by using '--mlcompatibility' or '--nowarn:62'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/OCamlCompat", Includes=[|"E_IndentOff01.fs"|])>]
    let ``OCamlCompat - E_IndentOff01.fs - --warnaserror --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--warnaserror"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0062
        |> withDiagnosticMessageMatches "This construct is for ML compatibility\. Consider using a file with extension '\.ml' or '\.mli' instead\. You can disable this warning by using '--mlcompatibility' or '--nowarn:62'\.$"

    // This test was automatically generated (moved from FSharpQA suite - OCamlCompat)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/OCamlCompat", Includes=[|"IndentOff02.fs"|])>]
    let ``OCamlCompat - IndentOff02.fs - --warnaserror --mlcompatibility`` compilation =
        compilation
        |> withOptions ["--warnaserror"; "--mlcompatibility"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - OCamlCompat)
    //<Expects status="warning" span="(4,1-4,14)" id="FS0062">This construct is for ML compatibility\. Consider using a file with extension '\.ml' or '\.mli' instead\. You can disable this warning by using '--mlcompatibility' or '--nowarn:62'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/OCamlCompat", Includes=[|"W_IndentOff03.fs"|])>]
    let ``OCamlCompat - W_IndentOff03.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0062
        |> withDiagnosticMessageMatches "This construct is for ML compatibility\. Consider using a file with extension '\.ml' or '\.mli' instead\. You can disable this warning by using '--mlcompatibility' or '--nowarn:62'\.$"

    // This test was automatically generated (moved from FSharpQA suite - OCamlCompat)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/OCamlCompat", Includes=[|"Hat01.fs"|])>]
    let ``OCamlCompat - Hat01.fs - --warnaserror --mlcompatibility`` compilation =
        compilation
        |> withOptions ["--warnaserror"; "--mlcompatibility"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - OCamlCompat)
    //<Expects status="warning" span="(6,13-6,14)" id="FS0062">This construct is for ML compatibility\. Consider using the '\+' operator instead\. This may require a type annotation to indicate it acts on strings\. This message can be disabled using '--nowarn:62' or '#nowarn "62"'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/OCamlCompat", Includes=[|"W_Hat01.fs"|])>]
    let ``OCamlCompat - W_Hat01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0062
        |> withDiagnosticMessageMatches "This construct is for ML compatibility\. Consider using the '\+' operator instead\. This may require a type annotation to indicate it acts on strings\. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'\.$"

    // This test was automatically generated (moved from FSharpQA suite - OCamlCompat)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/OCamlCompat", Includes=[|"NoParensInLet01.fs"|])>]
    let ``OCamlCompat - NoParensInLet01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - OCamlCompat)
    //<Expects status="warning" span="(10,19)" id="FS0062">This construct is for ML compatibility\. The syntax '\(typ,\.\.\.,typ\) ident' is not used in F# code\. Consider using 'ident<typ,\.\.\.,typ>' instead\. You can disable this warning by using '--mlcompatibility' or '--nowarn:62'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/OCamlCompat", Includes=[|"W_MultiArgumentGenericType.fs"|])>]
    let ``OCamlCompat - W_MultiArgumentGenericType.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withWarningCode 0062
        |> withDiagnosticMessageMatches "' instead\. You can disable this warning by using '--mlcompatibility' or '--nowarn:62'\.$"

    // This test was automatically generated (moved from FSharpQA suite - OCamlCompat)
    //<Expects status="error" span="(11,44)" id="FS0010">Unexpected identifier in expression$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/OCamlCompat", Includes=[|"MultiArgumentGenericType.fs"|])>]
    let ``OCamlCompat - MultiArgumentGenericType.fs - --mlcompatibility`` compilation =
        compilation
        |> withOptions ["--mlcompatibility"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected identifier in expression$"

    // This test was automatically generated (moved from FSharpQA suite - OCamlCompat)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/OCamlCompat", Includes=[|"OCamlStyleArrayIndexing.fs"|])>]
    let ``OCamlCompat - OCamlStyleArrayIndexing.fs - --mlcompatibility`` compilation =
        compilation
        |> withOptions ["--mlcompatibility"]
        |> typecheck
        |> shouldSucceed

