// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.Expressions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module DataExpressions_ComputationExpressions =

    // SOURCE=MinMaxValuesInLoop01.fs
    [<Theory; FileInlineData("MinMaxValuesInLoop01.fs")>]
    let ``MinMaxValuesInLoop01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=MinMaxValuesInLoop02.fs
    [<Theory; FileInlineData("MinMaxValuesInLoop02.fs")>]
    let ``MinMaxValuesInLoop02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=CompExprMethods01.fs
    [<Theory; FileInlineData("CompExprMethods01.fs")>]
    let ``CompExprMethods01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    // SOURCE=CompExprMethods02.fs
    [<Theory; FileInlineData("CompExprMethods02.fs")>]
    let ``CompExprMethods02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    // SOURCE=CompExprMethods03.fs
    [<Theory; FileInlineData("CompExprMethods03.fs")>]
    let ``CompExprMethods03_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=CompExprMethods04.fs
    [<Theory; FileInlineData("CompExprMethods04.fs")>]
    let ``CompExprMethods04_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=DifferentGenericBuilders.fs
    [<Theory; FileInlineData("DifferentGenericBuilders.fs")>]
    let ``DifferentGenericBuilders_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=BuilderViaExtMethods.fs
    [<Theory; FileInlineData("BuilderViaExtMethods.fs")>]
    let ``BuilderViaExtMethods_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=NonClassWorkflow01.fs
    [<Theory; FileInlineData("NonClassWorkflow01.fs")>]
    let ``NonClassWorkflow01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=MutateBuilders.fs
    [<Theory; FileInlineData("MutateBuilders.fs")>]
    let ``MutateBuilders_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    // SOURCE=RunAndDelay01.fs
    [<Theory; FileInlineData("RunAndDelay01.fs")>]
    let ``RunAndDelay01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=Capacity01.fs (ReqRetail - skipped since requires specific build config)
    [<Theory; FileInlineData("Capacity01.fs")>]
    let ``Capacity01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=E_MissingCombine.fs - error FS0708
    [<Theory; FileInlineData("E_MissingCombine.fs")>]
    let ``E_MissingCombine_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withErrorCode 708
        |> withDiagnosticMessageMatches "'Combine' method"

    // SOURCE=E_MissingFor.fs - error FS0708
    [<Theory; FileInlineData("E_MissingFor.fs")>]
    let ``E_MissingFor_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withErrorCode 708
        |> withDiagnosticMessageMatches "'For' method"

    // SOURCE=E_MissingReturn.fs - error FS0708
    [<Theory; FileInlineData("E_MissingReturn.fs")>]
    let ``E_MissingReturn_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withErrorCode 708
        |> withDiagnosticMessageMatches "'Return' method"

    // SOURCE=E_MissingReturnFrom.fs - error FS0708
    [<Theory; FileInlineData("E_MissingReturnFrom.fs")>]
    let ``E_MissingReturnFrom_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withErrorCode 708
        |> withDiagnosticMessageMatches "'ReturnFrom' method"

    // SOURCE=E_MissingTryFinally.fs - error FS0708
    [<Theory; FileInlineData("E_MissingTryFinally.fs")>]
    let ``E_MissingTryFinally_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withErrorCode 708
        |> withDiagnosticMessageMatches "'TryFinally' method"

    // SOURCE=E_MissingTryWith.fs - error FS0708
    [<Theory; FileInlineData("E_MissingTryWith.fs")>]
    let ``E_MissingTryWith_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withErrorCode 708
        |> withDiagnosticMessageMatches "'TryWith' method"

    // SOURCE=E_MissingUsing.fs - error FS0708
    [<Theory; FileInlineData("E_MissingUsing.fs")>]
    let ``E_MissingUsing_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withErrorCode 708
        |> withDiagnosticMessageMatches "'Using' method"

    // SOURCE=E_MissingWhile.fs - error FS0708
    [<Theory; FileInlineData("E_MissingWhile.fs")>]
    let ``E_MissingWhile_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withErrorCode 708
        |> withDiagnosticMessageMatches "'While' method"

    // SOURCE=E_MissingYield.fs - error FS0708
    [<Theory; FileInlineData("E_MissingYield.fs")>]
    let ``E_MissingYield_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withErrorCode 708
        |> withDiagnosticMessageMatches "'Yield' method"

    // SOURCE=E_MissingYieldFrom.fs - error FS0708
    [<Theory; FileInlineData("E_MissingYieldFrom.fs")>]
    let ``E_MissingYieldFrom_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withErrorCode 708
        |> withDiagnosticMessageMatches "'YieldFrom' method"

    // SOURCE=E_MissingZero.fs - error FS0708
    [<Theory; FileInlineData("E_MissingZero.fs")>]
    let ``E_MissingZero_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withErrorCode 708
        |> withDiagnosticMessageMatches "'Zero' method"

    // SOURCE=E_TypeAlias01.fs - error FS0001, FS0740
    [<Theory; FileInlineData("E_TypeAlias01.fs")>]
    let ``E_TypeAlias01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withErrorCode 1
