// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.Expressions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module SyntacticSugar =

    // Migrated from: tests/fsharpqa/Source/Conformance/Expressions/SyntacticSugar
    // Test count: 10

    let verifyCompile compilation =
        compilation
        |> asExe
        |> compile
        |> shouldSucceed

    let verifyTypecheck compilation =
        compilation
        |> asExe
        |> typecheck

    // SOURCE=infix_op01.fs - Sample negative test for infix operators
    // <Expects status="error" id="FS0003"></Expects>
    [<Theory; FileInlineData("infix_op01.fs")>]
    let ``infix_op01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyTypecheck
        |> shouldFail
        |> withErrorCode 3

    // SOURCE=Slices01.fs
    [<Theory; FileInlineData("Slices01.fs")>]
    let ``Slices01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompile

    // SOURCE=Slices02.fs
    [<Theory; FileInlineData("Slices02.fs")>]
    let ``Slices02_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompile

    // SOURCE=Slices03.fs
    [<Theory; FileInlineData("Slices03.fs")>]
    let ``Slices03_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompile

    // SOURCE=Slices04.fs
    [<Theory; FileInlineData("Slices04.fs")>]
    let ``Slices04_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompile

    // SOURCE=Slices05.fs - Skipped per https://github.com/dotnet/fsharp/issues/7735
    [<Theory(Skip = "https://github.com/dotnet/fsharp/issues/7735"); FileInlineData("Slices05.fs")>]
    let ``Slices05_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompile

    // SOURCE=Slices06.fs
    [<Theory; FileInlineData("Slices06.fs")>]
    let ``Slices06_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompile

    // SOURCE=Slices07.fs
    [<Theory; FileInlineData("Slices07.fs")>]
    let ``Slices07_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompile

    // SOURCE=E_GetSliceNotDef01.fs
    // <Expects id="FS0039" status="error">The type 'DU' does not define the field, constructor or member 'GetSlice'</Expects>
    [<Theory; FileInlineData("E_GetSliceNotDef01.fs")>]
    let ``E_GetSliceNotDef01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyTypecheck
        |> shouldFail
        |> withErrorCode 39
        |> withDiagnosticMessageMatches "The type 'DU' does not define the field, constructor or member 'GetSlice'"

    // SOURCE=E_GetSliceNotDef02.fs
    // <Expects id="FS0501" status="error">...</Expects>
    [<Theory; FileInlineData("E_GetSliceNotDef02.fs")>]
    let ``E_GetSliceNotDef02_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyTypecheck
        |> shouldFail
        |> withErrorCode 501
        |> withDiagnosticMessageMatches "GetSlice.*takes 4 argument"
