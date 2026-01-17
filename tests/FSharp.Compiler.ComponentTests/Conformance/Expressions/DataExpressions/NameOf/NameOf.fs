// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.Expressions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module NameOf =

    // SOURCE=E_NameOfIntConst.fs SCFLAGS="--langversion:5.0" - error FS3250
    [<Theory; FileInlineData("E_NameOfIntConst.fs")>]
    let ``E_NameOfIntConst_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withLangVersion50
        |> compile
        |> shouldFail
        |> withErrorCode 3250
        |> withDiagnosticMessageMatches "Expression does not have a name"

    // SOURCE=E_NameOfStringConst.fs SCFLAGS="--langversion:5.0" - error FS3250
    [<Theory; FileInlineData("E_NameOfStringConst.fs")>]
    let ``E_NameOfStringConst_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withLangVersion50
        |> compile
        |> shouldFail
        |> withErrorCode 3250
        |> withDiagnosticMessageMatches "Expression does not have a name"

    // SOURCE=E_NameOfAppliedFunction.fs SCFLAGS="--langversion:5.0" - error FS3250
    [<Theory; FileInlineData("E_NameOfAppliedFunction.fs")>]
    let ``E_NameOfAppliedFunction_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withLangVersion50
        |> compile
        |> shouldFail
        |> withErrorCode 3250
        |> withDiagnosticMessageMatches "Expression does not have a name"

    // SOURCE=E_NameOfIntegerAppliedFunction.fs SCFLAGS="--langversion:5.0" - error FS3250
    [<Theory; FileInlineData("E_NameOfIntegerAppliedFunction.fs")>]
    let ``E_NameOfIntegerAppliedFunction_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withLangVersion50
        |> compile
        |> shouldFail
        |> withErrorCode 3250
        |> withDiagnosticMessageMatches "Expression does not have a name"

    // SOURCE=E_NameOfPartiallyAppliedFunction.fs SCFLAGS="--langversion:5.0" - error FS3250
    [<Theory; FileInlineData("E_NameOfPartiallyAppliedFunction.fs")>]
    let ``E_NameOfPartiallyAppliedFunction_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withLangVersion50
        |> compile
        |> shouldFail
        |> withErrorCode 3250
        |> withDiagnosticMessageMatches "Expression does not have a name"

    // SOURCE=E_NameOfDictLookup.fs SCFLAGS="--langversion:5.0" - error FS3250
    [<Theory; FileInlineData("E_NameOfDictLookup.fs")>]
    let ``E_NameOfDictLookup_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withLangVersion50
        |> compile
        |> shouldFail
        |> withErrorCode 3250
        |> withDiagnosticMessageMatches "Expression does not have a name"

    // SOURCE=E_NameOfParameterAppliedFunction.fs SCFLAGS="--langversion:5.0" - error FS3250
    [<Theory; FileInlineData("E_NameOfParameterAppliedFunction.fs")>]
    let ``E_NameOfParameterAppliedFunction_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withLangVersion50
        |> compile
        |> shouldFail
        |> withErrorCode 3250
        |> withDiagnosticMessageMatches "Expression does not have a name"

    // SOURCE=E_NameOfAsAFunction.fs SCFLAGS="--langversion:5.0" - error FS3251
    [<Theory; FileInlineData("E_NameOfAsAFunction.fs")>]
    let ``E_NameOfAsAFunction_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withLangVersion50
        |> compile
        |> shouldFail
        |> withErrorCode 3251
        |> withDiagnosticMessageMatches "first-class function value"

    // SOURCE=E_NameOfWithPipe.fs SCFLAGS="--langversion:5.0" - error FS3251
    [<Theory; FileInlineData("E_NameOfWithPipe.fs")>]
    let ``E_NameOfWithPipe_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withLangVersion50
        |> compile
        |> shouldFail
        |> withErrorCode 3251
        |> withDiagnosticMessageMatches "first-class function value"

    // SOURCE=E_NameOfUnresolvableName.fs SCFLAGS="--langversion:5.0" - error FS0039
    [<Theory; FileInlineData("E_NameOfUnresolvableName.fs")>]
    let ``E_NameOfUnresolvableName_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withLangVersion50
        |> compile
        |> shouldFail
        |> withErrorCode 39
        |> withDiagnosticMessageMatches "not defined"
