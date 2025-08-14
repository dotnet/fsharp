// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module ErrorMessages.UnitOfMeasureTests

open Xunit
open FSharp.Test.Compiler

[<Fact>]
let ``Missing Measure attribute on type parameter`` () =
    Fsx """
type A<[<Measure>]'u>(x : int<'u>) =
    member this.X = x

type B<'u>(x: 'u) =
    member this.X = x

module M =
    type A<'u> with // Note the missing Measure attribute
        member this.Y = this.X

    type B<'u> with
        member this.Y = this.X

open System.Runtime.CompilerServices
type FooExt =
    [<Extension>]
    static member Bar(this: A<'u>, value: A<'u>) = this
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3874, Line 9, Col 12, Line 9, Col 14, "Expected unit-of-measure type parameter must be marked with the [<Measure>] attribute.")
        ]

[<Fact>]
let ``With Measure attribute on type parameter`` () =
        Fsx """
type A<[<Measure>]'u>(x : int<'u>) =
    member this.X = x

module M =
    type A<[<Measure>] 'u> with // Note the Measure attribute
        member this.Y = this.X

open System.Runtime.CompilerServices
type FooExt =
    [<Extension>]
    static member Bar(this: A<'u>, value: A<'u>) = this
        """
        |> typecheck
        |> shouldSucceed

[<Fact>]
let ``Instance members 01`` () =
    Fsx """
[<Measure>] 
type kg =
    member x.Value = 1.0
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 897, Line 4, Col 5, Line 4, Col 25, "Measure declarations may have only static members")
        ]

[<Fact>]
let ``Instance members 02 - Multiple`` () =
    Fsx """
[<Measure>] 
type kg =
    member x.Value = 1.0
    member x.GetWeight() = 2.0
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 897, Line 4, Col 5, Line 4, Col 25, "Measure declarations may have only static members")
            (Error 897, Line 5, Col 5, Line 5, Col 31, "Measure declarations may have only static members")
        ]

[<Fact>]
let ``Constructors`` () =
    Fsx """
[<Measure>] 
type meter =
    new() = { }
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 904, Line 4, Col 5, Line 4, Col 16, "Measure declarations may have only static members: constructors are not available")
        ]

[<Fact>]
let ``Type parameters`` () =
    Fsx """
[<Measure>]
type meter<'a> =
    class end
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 928, Line 3, Col 6, Line 3, Col 11, "Measure definitions cannot have type parameters");
        ]

[<Fact>]
let ``Inherit declarations`` () =
    Fsx """
[<Measure>] 
type Fahrenheit =
    inherit Foo()
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 962, Line 4, Col 5, Line 4, Col 18, "This 'inherit' declaration has arguments, but is not in a type with a primary constructor. Consider adding arguments to your type definition, e.g. 'type X(args) = ...'.")
            (Error 39, Line 4, Col 13, Line 4, Col 16, "The type 'Foo' is not defined.")
            (Error 897, Line 4, Col 5, Line 4, Col 18, "Measure declarations may have only static members")
        ]

[<Fact>]
let ``Instance let bindings`` () =
    Fsx """
[<Measure>] 
type Celsius =
    let instanceValue = 10
    do printfn "init"
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 963, Line 4, Col 5, Line 4, Col 27, "This definition may only be used in a type with a primary constructor. Consider adding arguments to your type definition, e.g. 'type X(args) = ...'.")
            (Error 897, Line 4, Col 5, Line 4, Col 27, "Measure declarations may have only static members")
            (Error 897, Line 5, Col 5, Line 5, Col 22, "Measure declarations may have only static members")
        ]

[<Fact>]
let ``Mixed valid and invalid 01`` () =
    Fsx """
[<Measure>] 
type Kelvin =
    static member AbsoluteZero = -273.15  // OK
    member x.Value = 0.0  // Error: instance member
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 897, Line 5, Col 5, Line 5, Col 25, "Measure declarations may have only static members")
        ]

[<Fact>]
let ``Mixed valid and invalid 02 - Constructor`` () =
    Fsx """
[<Measure>] 
type Kelvin =
    static member AbsoluteZero = -273.15  // OK
    new() = { }  // Error: constructor
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 904, Line 5, Col 5, Line 5, Col 16, "Measure declarations may have only static members: constructors are not available")
        ]
[<Fact>]
let ``Implicit constructor`` () =
    Fsx """
[<Measure>]
type newton() =
    let force = 10.0 
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 897, Line 3, Col 6, Line 3, Col 12, "Measure declarations may have only static members")
            (Error 897, Line 4, Col 5, Line 4, Col 21, "Measure declarations may have only static members")
        ]

[<Fact>]
let ``Augmentations 01`` () =
    Fsx """
[<Measure>]
type joule

type joule with
    member x.Energy = 1.0 
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 897, Line 6, Col 5, Line 6, Col 26, "Measure declarations may have only static members")
        ]

[<Fact>]
let ``Augmentations 02 - Multiple errors`` () =
    Fsx """
[<Measure>]
type joule

type joule with
    member x.Energy = 1.0 
    new() = { }
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 897, Line 6, Col 5, Line 6, Col 26, "Measure declarations may have only static members")
            (Error 904, Line 7, Col 5, Line 7, Col 16, "Measure declarations may have only static members: constructors are not available")
        ]

[<Fact>]
let ``Complex with type parameters`` () =
    Fsx """
[<Measure>]
type pascal<'a> =
    new() = { }
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 928, Line 3, Col 6, Line 3, Col 12, "Measure definitions cannot have type parameters")
            (Error 904, Line 4, Col 5, Line 4, Col 16, "Measure declarations may have only static members: constructors are not available")
        ]

[<Fact>]
let ``Let binding order`` () =
    Fsx """
[<Measure>]
type volt =
    static let y = 2  // OK
    let z = 3
    member this.Voltage = 0.0 
    static member Current = 1.0  // OK
    do printfn "hello"
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 960, Line 8, Col 5, Line 8, Col 23, "'let' and 'do' bindings must come before member and interface definitions in type definitions")
            (Error 897, Line 5, Col 5, Line 5, Col 14, "Measure declarations may have only static members")
            (Error 897, Line 8, Col 5, Line 8, Col 23, "Measure declarations may have only static members")
            (Error 897, Line 6, Col 5, Line 6, Col 30, "Measure declarations may have only static members")
        ]