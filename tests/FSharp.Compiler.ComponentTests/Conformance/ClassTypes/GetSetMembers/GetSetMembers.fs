// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.ClassTypes

open Xunit
open FSharp.Test.Compiler

module GetSetMembers =

    [<Fact>]
    let WithGetAndSet() =
        Fsx """
type Foo() =
    let mutable bar = ""

    member this.Bar
        with get () = bar
        and set nextBar = bar <- nextBar
        """
        |> withLangVersion50
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let WithGet() =
        Fsx """
type Foo() =
    let mutable bar = ""

    member this.Bar
        with get () = bar
        """
        |> withLangVersion50
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let WithSet() =
        Fsx """
type Foo() =
    let mutable bar = ""

    member this.Bar
        with set nextBar = bar <- nextBar
        """
        |> withLangVersion50
        |> typecheck
        |> shouldSucceed
        |> ignore