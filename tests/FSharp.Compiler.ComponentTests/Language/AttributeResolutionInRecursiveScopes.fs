// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Language

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module AttributeResolutionInRecursiveScopes =

    // Baselines: these attribute positions already worked before #5795.

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
    let ``attribute on union case in namespace rec resolves to attribute defined in same namespace`` () =
        Fsx """
namespace rec Ns

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

    [<Fact>]
    let ``attribute on record field in module rec resolves to attribute defined in same module`` () =
        Fsx """
module rec M

type CustomAttribute() = inherit System.Attribute()

type R = { [<CustomAttribute>] X: int }
"""
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``attribute on multiple record fields in module rec resolves to attributes defined in same module`` () =
        Fsx """
module rec M

type CustomAttribute() = inherit System.Attribute()
type AnotherAttribute() = inherit System.Attribute()

type R = {
    [<CustomAttribute>] X: int
    [<AnotherAttribute>] Y: string
    [<CustomAttribute; AnotherAttribute>] Z: float
}
"""
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``attribute on record field in namespace rec resolves to attribute defined in same namespace`` () =
        Fsx """
namespace rec Ns

type CustomAttribute() = inherit System.Attribute()

type R = { [<CustomAttribute>] X: int }
"""
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``attribute on type parameter in module rec resolves to attribute defined in same module`` () =
        Fsx """
module rec M

type CustomAttribute() = inherit System.Attribute()

type B<[<CustomAttribute>]'a> = | B of 'a
"""
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``attribute on type parameter in namespace rec resolves to attribute defined in same namespace`` () =
        Fsx """
namespace rec Ns

type CustomAttribute() = inherit System.Attribute()

type B<[<CustomAttribute>]'a> = | B of 'a
"""
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``attribute on type parameter combined with framework Measure attribute in module rec compiles`` () =
        Fsx """
module rec M

type CustomAttribute() = inherit System.Attribute()

type B<[<Measure>]'u, [<CustomAttribute>]'a> = B of 'a
"""
        |> compile
        |> shouldSucceed

    // Edge cases

    [<Fact>]
    let ``attribute defined in nested module of rec scope resolves on union case`` () =
        Fsx """
module rec M

module Nested =
    type CustomAttribute() = inherit System.Attribute()

type A = | [<Nested.CustomAttribute>] A
"""
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``attribute defined in nested module of rec scope resolves on type parameter`` () =
        Fsx """
module rec M

module Nested =
    type CustomAttribute() = inherit System.Attribute()

type B<[<Nested.CustomAttribute>]'a> = B of 'a
"""
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``attribute defined in nested module of rec scope resolves on record field`` () =
        Fsx """
module rec M

module Nested =
    type CustomAttribute() = inherit System.Attribute()

type R = { [<Nested.CustomAttribute>] X: int }
"""
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``multiple attributes mixing framework Obsolete and rec-scope custom on union case compile`` () =
        Fsx """
module rec M

open System

type CustomAttribute() = inherit System.Attribute()

type A = | [<Custom; Obsolete("test")>] A
"""
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``rec-scope attribute shadows outer-scope attribute on union case in nested rec module`` () =
        Fsx """
module Root

type CustomAttribute() = inherit System.Attribute()

module rec M =
    type CustomAttribute() = inherit System.Attribute()
    type A = | [<CustomAttribute>] A
"""
        |> compile
        |> shouldSucceed

    // [<Measure>] resolves to the user's MeasureAttribute by name, so kind inference breaks.
    // Unrelated to #5795 rec-scope fix.
    [<Fact(Skip = "Tracked separately: user-shadow of MeasureAttribute changes kind inference by name-resolution, unrelated to #5795 fix")>]
    let ``user-defined MeasureAttribute in rec scope does not break framework Measure kind inference`` () =
        Fsx """
module rec M

type MeasureAttribute() = inherit System.Attribute()

[<Measure>] type kg
"""
        |> compile
        |> shouldSucceed

    // Negative tests — must still error after the fix.

    [<Fact>]
    let ``non-attribute type used on union case in module rec still produces diagnostic`` () =
        // FS3242: "does not inherit Attribute" — warning, not error.
        Fsx """
module rec M

type NotAnAttribute() = class end

type A = | [<NotAnAttribute>] A
"""
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnosticMessageMatches "does not inherit Attribute"

    [<Fact>]
    let ``unknown attribute name on union case in module rec still errors with FS0039`` () =
        Fsx """
module rec M

type A = | [<DoesNotExist>] A
"""
        |> compile
        |> shouldFail
        |> withErrorCode 39

    [<Fact>]
    let ``unknown attribute name on type parameter in module rec still errors with FS0039`` () =
        Fsx """
module rec M

type B<[<DoesNotExist>]'a> = B of 'a
"""
        |> compile
        |> shouldFail
        |> withErrorCode 39

    [<Fact>]
    let ``unknown attribute name on record field in module rec still errors with FS0039`` () =
        Fsx """
module rec M

type R = { [<DoesNotExist>] X: int }
"""
        |> compile
        |> shouldFail
        |> withErrorCode 39
