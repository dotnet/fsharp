// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Tests for FSI pretty-printing of anonymous records (regression for dotnet/fsharp#6116).
/// Anonymous records must be printed with `{| |}` braces, distinct from nominal records which use `{ }`.
namespace InteractiveSession

open Xunit
open FSharp.Test.Compiler

module AnonRecordPrinting =

    // --- Anonymous records: must use {| |} ---

    // NOTE: Assertions target the VALUE portion (after `=`) only, because the
    // `val it: ...` type-annotation already contains `{| ... |}` for anon
    // record types regardless of the value-printing bug being fixed here.

    [<Fact>]
    let ``Anonymous record prints with bar braces``() =
        Fsx """
let r = {| Name = "Phillip"; Age = 28 |};;
()
"""
        |> withOptions ["--nologo"]
        |> runFsi
        |> shouldSucceed
        |> withStdOutContains "= {| Age = 28; Name = \"Phillip\" |}"
        |> ignore

    [<Fact>]
    let ``Anonymous record with single field prints with bar braces``() =
        Fsx """
{| X = 1 |};;
()
"""
        |> withOptions ["--nologo"]
        |> runFsi
        |> shouldSucceed
        |> withStdOutContains "= {| X = 1 |}"
        |> ignore

    [<Fact>]
    let ``Nested anonymous record prints with bar braces at both levels``() =
        Fsx """
{| Inner = {| X = 1 |} |};;
()
"""
        |> withOptions ["--nologo"]
        |> runFsi
        |> shouldSucceed
        // Both outer and inner record values must use bar-braces.
        |> withStdOutContains "= {| Inner = {| X = 1 |} |}"
        |> ignore

    [<Fact>]
    let ``Struct anonymous record prints with struct keyword and bar braces``() =
        Fsx """
struct {| X = 1; Y = 2 |};;
()
"""
        |> withOptions ["--nologo"]
        |> runFsi
        |> shouldSucceed
        |> withStdOutContains "= struct {| X = 1; Y = 2 |}"
        |> ignore

    // --- Nominal records: must KEEP printing with { } (regression guard) ---

    [<Fact>]
    let ``Nominal record still prints with plain braces and not bar braces``() =
        Fsx """
type R = { Name: string; Age: int }
let r = { Name = "Phillip"; Age = 28 };;
()
"""
        |> withOptions ["--nologo"]
        |> runFsi
        |> shouldSucceed
        |> withStdOutContains "{ Name = \"Phillip\""
        |> withStdOutContains "Age = 28 }"
        |> ignore

    // --- Other shapes must be unchanged (regression guard) ---

    [<Fact>]
    let ``Tuple printing unchanged``() =
        Fsx """
(1, "x");;
()
"""
        |> withOptions ["--nologo"]
        |> runFsi
        |> shouldSucceed
        |> withStdOutContains "(1, \"x\")"
        |> ignore

    [<Fact>]
    let ``Discriminated union printing unchanged``() =
        Fsx """
type DU = A of int | B of string
A 42;;
()
"""
        |> withOptions ["--nologo"]
        |> runFsi
        |> shouldSucceed
        |> withStdOutContains "A 42"
        |> ignore

    [<Fact>]
    let ``List printing unchanged``() =
        Fsx """
[1; 2; 3];;
()
"""
        |> withOptions ["--nologo"]
        |> runFsi
        |> shouldSucceed
        |> withStdOutContains "[1; 2; 3]"
        |> ignore
