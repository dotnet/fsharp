// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module ErrorMessages.UnitOfMeasureTests

open Xunit
open FSharp.Test.Compiler

[<Fact>]
let ``Error - Expected unit-of-measure type parameter must be marked with the [<Measure>] attribute.`` () =
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
let ``Expected unit-of-measure type parameter must be marked with the [<Measure>] attribute.`` () =
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
let ``Measure types with instance members should report multiple errors`` () =
    Fsx """
[<Measure>] 
type kg =
    member x.Value = 1.0
    member x.GetWeight() = 2.0
    member x.Property = 3.0
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 897, Line 4, Col 5, Line 4, Col 25, "Measure declarations may have only static members")
            (Error 897, Line 5, Col 5, Line 5, Col 31, "Measure declarations may have only static members")
            (Error 897, Line 6, Col 5, Line 6, Col 28, "Measure declarations may have only static members")
        ]

[<Fact>]
let ``Measure types with constructors should report all constructor errors`` () =
    Fsx """
[<Measure>] 
type meter =
    new() = { }
    new(x: int) = { }
    member x.Value = 1.0
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 904, Line 4, Col 5, Line 4, Col 16, "Measure declarations may have only static members: constructors are not available")
            (Error 904, Line 5, Col 5, Line 5, Col 22, "Measure declarations may have only static members: constructors are not available")
            (Error 897, Line 6, Col 5, Line 6, Col 25, "Measure declarations may have only static members")
        ]

[<Fact>]
let ``Measure types with type parameters should report error`` () =
    Fsx """
[<Measure>]
type meter<'a> =
    class end

[<Measure>]
type second<'a, 'b> =
    class end
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 928, Line 3, Col 6, Line 3, Col 11, "Measure definitions cannot have type parameters");
            (Error 928, Line 7, Col 6, Line 7, Col 12, "Measure definitions cannot have type parameters")
        ]

[<Fact>]
let ``Measure types with inherit declarations should report error`` () =
    Fsx """
[<Measure>] 
type Fahrenheit =
    inherit Foo()
    member x.Value = 1.0
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 962, Line 4, Col 5, Line 4, Col 18, "This 'inherit' declaration has arguments, but is not in a type with a primary constructor. Consider adding arguments to your type definition, e.g. 'type X(args) = ...'.")
            (Error 39, Line 4, Col 13, Line 4, Col 16, "The type 'Foo' is not defined.")
            (Error 897, Line 4, Col 5, Line 4, Col 18, "Measure declarations may have only static members")
            (Error 897, Line 5, Col 5, Line 5, Col 25, "Measure declarations may have only static members")
        ]

[<Fact>]
let ``Measure types with instance let bindings should report multiple errors`` () =
    Fsx """
[<Measure>] 
type Celsius =
    let instanceValue = 10
    let mutable mut = 20
    do printfn "init"
    static member Valid = 30
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 963, Line 4, Col 5, Line 4, Col 27, "This definition may only be used in a type with a primary constructor. Consider adding arguments to your type definition, e.g. 'type X(args) = ...'.")
            (Error 897, Line 4, Col 5, Line 4, Col 27, "Measure declarations may have only static members")
            (Error 897, Line 5, Col 5, Line 5, Col 25, "Measure declarations may have only static members")
            (Error 897, Line 6, Col 5, Line 6, Col 22, "Measure declarations may have only static members")
        ]

[<Fact>]
let ``Measure types with mixed valid and invalid members should report only errors`` () =
    Fsx """
[<Measure>] 
type Kelvin =
    static member AbsoluteZero = -273.15  // OK
    member x.Value = 0.0  // Error: instance member
    static member Convert(c: float<Celsius>) = c + 273.15<Kelvin>  // OK
    new() = { }  // Error: constructor
    static let cache = System.Collections.Generic.Dictionary<float, float>()  // OK
    member x.GetKelvin() = x.Value  // Error: instance member
and [<Measure>] Celsius
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 897, Line 5, Col 5, Line 5, Col 25, "Measure declarations may have only static members")
            (Error 904, Line 7, Col 5, Line 7, Col 16, "Measure declarations may have only static members: constructors are not available")
            (Error 897, Line 9, Col 5, Line 9, Col 35, "Measure declarations may have only static members")
            (Error 1, Line 6, Col 52, Line 6, Col 66, "The unit of measure 'Kelvin' does not match the unit of measure 'Celsius'")
            (Error 43, Line 6, Col 50, Line 6, Col 51, "The unit of measure 'Kelvin' does not match the unit of measure 'Celsius'")
        ]

[<Fact>]
let ``Measure type in class with implicit constructor should report all errors`` () =
    Fsx """
[<Measure>]
type newton() =
    let force = 10.0 
    member x.Force = force
    static member Valid = 1.0  // OK
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 897, Line 3, Col 6, Line 3, Col 12, "Measure declarations may have only static members")
            (Error 897, Line 4, Col 5, Line 4, Col 21, "Measure declarations may have only static members")
            (Error 897, Line 5, Col 5, Line 5, Col 27, "Measure declarations may have only static members")
        ]

[<Fact>]
let ``Measure type augmentations with invalid members should report all errors`` () =
    Fsx """
[<Measure>]
type joule

type joule with
    member x.Energy = 1.0 
    new() = { }
    member x.GetEnergy() = 2.0
    static member Valid = 3.0  // OK
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 897, Line 6, Col 5, Line 6, Col 26, "Measure declarations may have only static members")
            (Error 904, Line 7, Col 5, Line 7, Col 16, "Measure declarations may have only static members: constructors are not available")
            (Error 897, Line 8, Col 5, Line 8, Col 31, "Measure declarations may have only static members")
        ]

[<Fact>]
let ``Complex measure type with multiple error types should report all`` () =
    Fsx """
[<Measure>]
type pascal<'a> =
    inherit System.Object()
    new() = { }
    new(x: int) = { }
    member x.Pressure = pressure
    member x.SetPressure(p) = pressure <- p
    interface System.IComparable with
        member x.CompareTo(obj) = 0
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 962, Line 4, Col 5, Line 4, Col 28, "This 'inherit' declaration has arguments, but is not in a type with a primary constructor. Consider adding arguments to your type definition, e.g. 'type X(args) = ...'.")
            (Error 928, Line 3, Col 6, Line 3, Col 12, "Measure definitions cannot have type parameters")
            (Error 897, Line 4, Col 5, Line 4, Col 28, "Measure declarations may have only static members")
            (Error 904, Line 5, Col 5, Line 5, Col 16, "Measure declarations may have only static members: constructors are not available")
            (Error 904, Line 6, Col 5, Line 6, Col 22, "Measure declarations may have only static members: constructors are not available")
            (Error 897, Line 7, Col 5, Line 7, Col 33, "Measure declarations may have only static members")
            (Error 897, Line 8, Col 5, Line 8, Col 44, "Measure declarations may have only static members")
            (Error 897, Line 10, Col 9, Line 10, Col 36, "Measure declarations may have only static members")
            (Error 39, Line 7, Col 25, Line 7, Col 33, "The value or constructor 'pressure' is not defined.")
            (Error 39, Line 8, Col 31, Line 8, Col 39, "The value or constructor 'pressure' is not defined.")
        ]

[<Fact>]
let ``Measure type with static and instance let bindings in wrong order`` () =
    Fsx """
[<Measure>]
type volt =
    static let y = 2  // OK
    let z = 3
    member this.Voltage = 0.0 
    static member Current = 1.0  // OK
    do printfn "hello"
    static do printfn "world"  // OK
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 960, Line 8, Col 5, Line 8, Col 23, "'let' and 'do' bindings must come before member and interface definitions in type definitions")
            (Error 897, Line 5, Col 5, Line 5, Col 14, "Measure declarations may have only static members")
            (Error 897, Line 8, Col 5, Line 8, Col 23, "Measure declarations may have only static members")
            (Error 897, Line 6, Col 5, Line 6, Col 30, "Measure declarations may have only static members")
        ]
