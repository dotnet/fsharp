module FSharp.Compiler.ComponentTests.ErrorMessages.IndexedSetterNamedArgTests

open Xunit
open FSharp.Test.Compiler

[<Fact>]
let ``Indexed property setter with named argument does not ICE - intrinsic``() =
    FSharp """
module Test
type T() =
    member x.Item
        with get (a1: obj) = 1
        and  set (a1: obj) (value: int) = ()

let t = T()
t.Item(a1 = "x") <- 1
    """
    |> compile
    |> shouldSucceed

[<Fact>]
let ``Indexed property setter with named argument does not ICE - named property``() =
    FSharp """
module Test
type T() =
    member x.indexed1
        with get (a1: obj) = 1
        and  set (a1: obj) (value: int) = ()

let t = T()
t.indexed1(a1 = "x") <- 1
    """
    |> compile
    |> shouldSucceed

[<Fact>]
let ``Indexed property setter with named argument does not ICE - extension``() =
    FSharp """
module Test
type T() =
    member x.indexed1
        with get (a1: obj) = 1
        and  set (a1: obj) (value: int) = ()

module Extensions =
    type T with
        member x.indexed1
            with get (aa1: obj) = 1
            and  set (aa1: obj) (value: int) = ()

open Extensions
let t = T()
t.indexed1(aa1 = "x") <- 1
    """
    |> compile
    |> shouldSucceed

[<Fact>]
let ``Indexed property getter with named argument still works``() =
    FSharp """
module Test
type T() =
    member x.indexed1
        with get (a1: obj) = 1
        and  set (a1: obj) (value: int) = ()

let t = T()
let _ = t.indexed1(a1 = "x")
    """
    |> compile
    |> shouldSucceed
