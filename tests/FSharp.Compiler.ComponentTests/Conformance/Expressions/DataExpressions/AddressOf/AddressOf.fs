// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.Expressions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module AddressOf =

    // SOURCE=addressof_local_unit.fsx SCFLAGS=-a
    [<Theory; FileInlineData("addressof_local_unit.fsx")>]
    let ``addressof_local_unit_fsx`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> compile
        |> shouldSucceed

    // SOURCE=E_byrefvaluesnotpermitted001.fs - error FS0431
    [<Theory; FileInlineData("E_byrefvaluesnotpermitted001.fs")>]
    let ``E_byrefvaluesnotpermitted001_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withErrorCode 431
        |> withDiagnosticMessageMatches "byref"

    // Issue #18841: let _ = &expr should compile (parity with let x = &expr)

    [<Fact>]
    let ``Issue 18841 - let discard with address-of struct value compiles`` () =
        Fsx """
module Test

type S =
    struct
        val Field: int
    end

let test () =
    let s = S()
    let _ = &s
    let a = &s
    ignore a
        """
        |> typecheck
        |> shouldSucceed

    [<Fact>]
    let ``Issue 18841 - let discard with address-of mutable local compiles`` () =
        Fsx """
module Test
let test () =
    let mutable x = 42
    let _ = &x
    ()
        """
        |> typecheck
        |> shouldSucceed

    [<Fact>]
    let ``Issue 18841 - let discard with address-of struct field compiles`` () =
        Fsx """
module Test

[<Struct>]
type S = { mutable Field: int }

let test () =
    let mutable s = { Field = 0 }
    let _ = &s.Field
    ()
        """
        |> typecheck
        |> shouldSucceed

    [<Fact>]
    let ``Issue 18841 - let discard with parenthesized address-of compiles`` () =
        Fsx """
module Test

type S =
    struct
        val Field: int
    end

let test () =
    let s = S()
    let _ = &(s)
    ()
        """
        |> typecheck
        |> shouldSucceed

    [<Fact>]
    let ``Issue 18841 - let discard with address-of inside function parameter compiles`` () =
        Fsx """
module Test

type S =
    struct
        val Field: int
    end

let f (s: S) =
    let _ = &s
    ()
        """
        |> typecheck
        |> shouldSucceed

    [<Fact>]
    let ``Issue 18841 - named binding let x = &s still compiles (regression guard)`` () =
        Fsx """
module Test

type S =
    struct
        val Field: int
    end

let test () =
    let s = S()
    let a = &s
    ignore a
        """
        |> typecheck
        |> shouldSucceed

    [<Fact>]
    let ``Issue 19608 - address of untyped ValueNone compiles`` () =
        Fsx """
module Test

let x (y: inref<ValueOption<int>>) = ()

let test () =
    let ffff = ValueNone
    x &ffff
        """
        |> asLibrary
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Issue 19608 - native address of untyped ValueNone binding fails`` () =
        Fsx """
#nowarn "51"

module Test

let test () =
    let ffff = ValueNone
    let _ = &&ffff
    ()
        """
        |> typecheck
        |> shouldFail
        |> withErrorCode 256
