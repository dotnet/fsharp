// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.Expressions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module DataExpressions_ObjectExpressions =

    // SOURCE=GenericObjectExpression01.fs
    [<Theory; FileInlineData("GenericObjectExpression01.fs")>]
    let ``GenericObjectExpression01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=GenericObjectExpression02.fs SCFLAGS="--warnaserror:64"
    [<Theory; FileInlineData("GenericObjectExpression02.fs")>]
    let ``GenericObjectExpression02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--warnaserror:64"]
        |> compile
        |> shouldSucceed

    // SOURCE=E_MembersMustBeVirtual01.fs - error FS0767, FS0017
    [<Theory; FileInlineData("E_MembersMustBeVirtual01.fs")>]
    let ``E_MembersMustBeVirtual01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withErrorCode 767

    // SOURCE=ObjExprWithOverloadedMethod01.fs
    [<Theory; FileInlineData("ObjExprWithOverloadedMethod01.fs")>]
    let ``ObjExprWithOverloadedMethod01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=E_ObjExprWithDuplOverride01.fs - error FS0359
    [<Theory; FileInlineData("E_ObjExprWithDuplOverride01.fs")>]
    let ``E_ObjExprWithDuplOverride01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withErrorCode 359
        |> withDiagnosticMessageMatches "More than one override"

    // SOURCE=GenericTypeAnnotations01.fs SCFLAGS="--warnaserror"
    [<Theory; FileInlineData("GenericTypeAnnotations01.fs")>]
    let ``GenericTypeAnnotations01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--warnaserror"]
        |> compile
        |> shouldSucceed

    // SOURCE=StaticType01.fs
    [<Theory; FileInlineData("StaticType01.fs")>]
    let ``StaticType01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=W_Deprecated01.fs - error FS0035 (deprecated syntax is now an error)
    [<Theory; FileInlineData("W_Deprecated01.fs")>]
    let ``W_Deprecated01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withErrorCode 35
        |> withDiagnosticMessageMatches "deprecated"

    // SOURCE=E_InvalidSelfReferentialStructConstructor.fs - error FS0658, FS0696
    [<Theory; FileInlineData("E_InvalidSelfReferentialStructConstructor.fs")>]
    let ``E_InvalidSelfReferentialStructConstructor_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withErrorCode 658

    // SOURCE=ValidStructConstructor.fs
    [<Theory; FileInlineData("ValidStructConstructor.fs")>]
    let ``ValidStructConstructor_fs`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> compile
        |> shouldSucceed

    // SOURCE=ObjExprWithOverride01.fs
    [<Theory; FileInlineData("ObjExprWithOverride01.fs")>]
    let ``ObjExprWithOverride01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=E_ObjExprWithOverride01.fs SCFLAGS="-r:Helper.dll" - requires C# interop
    // SKIP: Requires C# interop with Helper.cs
    [<Theory(Skip = "Requires C# interop with Helper.dll"); FileInlineData("E_ObjExprWithOverride01.fs")>]
    let ``E_ObjExprWithOverride01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withErrorCode 360

    // SOURCE=InterfaceObjectExpression01.fs
    [<Theory; FileInlineData("InterfaceObjectExpression01.fs")>]
    let ``InterfaceObjectExpression01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=MultipleInterfacesInObjExpr.fs
    [<Theory; FileInlineData("MultipleInterfacesInObjExpr.fs")>]
    let ``MultipleInterfacesInObjExpr_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed
