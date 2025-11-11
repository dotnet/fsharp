namespace FSharp.Compiler.ComponentTests.Conformance.Expressions

open Xunit
open FSharp.Test.Compiler

module StructObjectExpression =

    [<Fact>]
    let ``Object expression in struct should not generate byref field - simple case`` () =
        FSharp """
type Class(test : obj) = class end

[<Struct>]
type Struct(test : obj) =
    member _.Test() = {
        new Class(test) with
        member _.ToString() = ""
    }

let s = Struct(42)
let obj = s.Test()
        """
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Object expression in struct with multiple fields`` () =
        FSharp """
type Base(x: int, y: string) = class end

[<Struct>]
type MyStruct(x: int, y: string) =
    member _.CreateObj() = {
        new Base(x, y) with
        member _.ToString() = y + string x
    }

let s = MyStruct(42, "test")
let obj = s.CreateObj()
        """
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Object expression in struct referencing field in override method`` () =
        FSharp """
type IFoo =
    abstract member DoSomething : unit -> int

[<Struct>]
type MyStruct(value: int) =
    member _.CreateFoo() = {
        new IFoo with
        member _.DoSomething() = value * 2
    }

let s = MyStruct(21)
let foo = s.CreateFoo()
let result = foo.DoSomething()
        """
        |> compile
        |> shouldSucceed
