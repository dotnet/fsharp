// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.InferenceProcedures

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ConstraintSolving =

    // SOURCE=E_TypeFuncDeclaredExplicit01.fs
    [<Theory; FileInlineData("E_TypeFuncDeclaredExplicit01.fs")>]
    let ``E_TypeFuncDeclaredExplicit01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 30
        |> ignore

    // SOURCE=ValueRestriction01.fs
    [<Theory; FileInlineData("ValueRestriction01.fs")>]
    let ``ValueRestriction01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldSucceed

    // SOURCE=E_ValueRestriction01.fs
    [<Theory; FileInlineData("E_ValueRestriction01.fs")>]
    let ``E_ValueRestriction01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 30
        |> ignore

    // SOURCE=EnumConstraint01.fs
    [<Theory; FileInlineData("EnumConstraint01.fs")>]
    let ``EnumConstraint01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldSucceed

    // SOURCE=E_EnumConstraint01.fs
    [<Theory; FileInlineData("E_EnumConstraint01.fs")>]
    let ``E_EnumConstraint01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 1
        |> ignore

    // SOURCE=DelegateConstraint01.fs
    [<Theory; FileInlineData("DelegateConstraint01.fs")>]
    let ``DelegateConstraint01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withNoWarn 3370
        |> typecheck
        |> shouldSucceed

    // SOURCE=E_DelegateConstraint01.fs
    [<Theory; FileInlineData("E_DelegateConstraint01.fs")>]
    let ``E_DelegateConstraint01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 1
        |> ignore

    // SOURCE=ConstructorConstraint01.fs
    [<Theory; FileInlineData("ConstructorConstraint01.fs")>]
    let ``ConstructorConstraint01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldSucceed
