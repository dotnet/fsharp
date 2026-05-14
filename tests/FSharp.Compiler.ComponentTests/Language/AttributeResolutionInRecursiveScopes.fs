// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Language

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module AttributeResolutionInRecursiveScopes =

    // Regression baseline — these attribute positions already worked before issue #5795 was fixed.
    // They must continue to compile after the fix. Do NOT remove these tests.

    [<Fact>]
    let ``attribute on type declaration in module rec resolves to attribute defined in same module`` () =
        Fsx """
module rec M

type CustomAttribute() = inherit System.Attribute()

[<CustomAttribute>]
type A = | A
"""
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``attribute on let binding in module rec resolves to attribute defined in same module`` () =
        Fsx """
module rec M

type CustomAttribute() = inherit System.Attribute()

[<CustomAttribute>]
let a = ()
"""
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``attribute on type declaration in namespace rec resolves to attribute defined in same namespace`` () =
        Fsx """
namespace rec Ns

type CustomAttribute() = inherit System.Attribute()

[<CustomAttribute>]
type A = | A
"""
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``attribute on let binding in non-rec module resolves to attribute defined in same module`` () =
        // Non-rec baseline — should always work.
        Fsx """
module M

type CustomAttribute() = inherit System.Attribute()

[<CustomAttribute>]
let a = ()
"""
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``attribute on union case in module rec resolves to attribute defined in same module`` () =
        Fsx """
module rec M

type CustomAttribute() = inherit System.Attribute()

type A = | [<CustomAttribute>] A
"""
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``attribute on every case of a DU in module rec resolves to attribute defined in same module`` () =
        Fsx """
module rec M

type CustomAttribute() = inherit System.Attribute()

type Shape =
    | [<CustomAttribute>] Circle of float
    | [<CustomAttribute>] Square of float
"""
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``attribute shorthand on union case in module rec resolves to attribute defined in same module`` () =
        Fsx """
module rec M

type CustomAttribute() = inherit System.Attribute()

type A = | [<Custom>] A
"""
        |> compile
        |> shouldSucceed
