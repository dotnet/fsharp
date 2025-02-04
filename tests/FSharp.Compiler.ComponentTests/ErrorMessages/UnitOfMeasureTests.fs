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

