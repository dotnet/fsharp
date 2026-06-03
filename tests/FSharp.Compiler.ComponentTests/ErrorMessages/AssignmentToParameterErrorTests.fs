// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace ErrorMessages

open Xunit
open FSharp.Test.Compiler

module ``FS0027 on function parameters (issue 15803)`` =

    [<Theory>]
    [<InlineData("let f (x: int) = x <- 5")>]
    [<InlineData("let f (x: int) (y: int) = y <- 5")>]
    [<InlineData("type T() =\n    member _.M(x: int) = x <- 5")>]
    let ``FS0027 on parameter does not suggest illegal 'let mutable' shadow`` source =
        FSharp source
        |> typecheck
        |> shouldFail
        |> withErrorCode 27
        |> withDiagnosticMessageDoesntMatch "let mutable .* = expression"

    [<Fact>]
    let ``FS0027 on parameter still mentions mutability`` () =
        FSharp "let f (x: int) = x <- 5"
        |> typecheck
        |> shouldFail
        |> withErrorCode 27
        |> withDiagnosticMessageMatches "mutable"

    [<Fact>]
    let ``FS0027 on parameter suggests plain let shadowing`` () =
        FSharp "let f (x: int) = x <- 5"
        |> typecheck
        |> shouldFail
        |> withErrorCode 27
        |> withDiagnosticMessageMatches "'let x = \.\.\.'"

    [<Fact>]
    let ``FS0027 on local let binding still suggests 'let mutable'`` () =
        FSharp """
let f () =
    let x = 5
    x <- 10
"""
        |> typecheck
        |> shouldFail
        |> withErrorCode 27
        |> withDiagnosticMessageMatches "let mutable x = expression"

    [<Fact>]
    let ``FS0027 on constructor parameter does not suggest illegal 'let mutable' shadow`` () =
        FSharp """
type C(x: int) =
    member _.M() = x <- 5
"""
        |> typecheck
        |> shouldFail
        |> withErrorCode 27
        |> withDiagnosticMessageDoesntMatch "let mutable .* = expression"

    [<Fact>]
    let ``FS0027 on closure-captured parameter does not suggest illegal 'let mutable' shadow`` () =
        FSharp """
let f (x: int) =
    let inner () = x <- 5
    inner()
"""
        |> typecheck
        |> shouldFail
        |> withErrorCode 27
        |> withDiagnosticMessageDoesntMatch "let mutable .* = expression"

    [<Fact>]
    let ``Mutable local does not trigger FS0027`` () =
        FSharp """
let f () =
    let mutable x = 5
    x <- 10
    x
"""
        |> typecheck
        |> shouldSucceed

    [<Fact>]
    let ``Byref parameter does not trigger FS0027`` () =
        FSharp "let f (x: byref<int>) = x <- 5"
        |> typecheck
        |> shouldSucceed
