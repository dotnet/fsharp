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

[<Theory>]
[<InlineData("t.indexed1(a1 = \"x\") <- 1")>]
[<InlineData("t.indexed1(\"x\") <- 1")>]
[<InlineData("t.set_indexed1(\"x\", 1)")>]
let ``Indexed setter call shapes compile`` (call: string) =
    FSharp $"""
module Test
type T() =
    member x.indexed1
        with get (a1: obj) = 1
        and  set (a1: obj) (value: int) = ()

let t = T()
{call}
    """
    |> compile
    |> shouldSucceed

[<Fact>]
let ``Multi-arg indexer setter with named args compiles``() =
    FSharp """
module Test
type M() =
    member x.Item
        with get (a: int, b: string) = 1
        and  set (a: int, b: string) (v: int) = ()

let m = M()
m.[a = 1, b = "x"] <- 5
m.[b = "x", a = 1] <- 5
m.[1, b = "x"] <- 5
    """
    |> compile
    |> shouldSucceed

[<Fact>]
let ``Default Item indexer setter with named arg compiles``() =
    FSharp """
module Test
type D() =
    member x.Item
        with get (k: string) = 1
        and  set (k: string) (v: int) = ()

let d = D()
d.[k = "x"] <- 1
    """
    |> compile
    |> shouldSucceed
