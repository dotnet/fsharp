namespace FSharp.Compiler.ComponentTests.Conformance.Expressions

open Xunit
open FSharp.Test.Compiler

module StructObjectExpression =

    [<Fact>]
    let ``Object expression in struct should not generate byref field - simple case`` () =
        Fsx """
type Class(test : obj) = class end

[<Struct; NoComparison>]
type Struct(test : obj) =
    member _.Test() = {
        new Class(test) with
        member _.ToString() = ""
    }

let s = Struct(42)
let obj = s.Test()
        """
        |> withOptions [ "--nowarn:52" ] // Suppress struct copy warning
        |> compileExeAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Object expression in struct with multiple fields`` () =
        Fsx """
type Base(x: int, y: string) = class end

[<Struct; NoComparison>]
type MyStruct(x: int, y: string) =
    member _.CreateObj() = {
        new Base(x, y) with
        member _.ToString() = y + string x
    }

let s = MyStruct(42, "test")
let obj = s.CreateObj()
        """
        |> withOptions [ "--nowarn:52" ]
        |> compileExeAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Object expression in struct referencing field in override method`` () =
        Fsx """
type IFoo =
    abstract member DoSomething : unit -> int

[<Struct; NoComparison>]
type MyStruct(value: int) =
    member _.CreateFoo() = {
        new IFoo with
        member _.DoSomething() = value * 2
    }

let s = MyStruct(21)
let foo = s.CreateFoo()
let result = foo.DoSomething()
        """
        |> withOptions [ "--nowarn:52" ]
        |> compileExeAndRun
        |> shouldSucceed

    // Regression tests - these must continue to work

    [<Fact>]
    let ``Static member in struct with object expression should compile - StructBox regression`` () =
        // This is the StructBox.Comparer pattern from FSharp.Core/seqcore.fs
        // Static members don't have 'this' so should NOT be transformed
        Fsx """
open System.Collections.Generic

[<Struct; NoComparison; NoEquality>]
type StructBox<'T when 'T: equality>(value: 'T) =
    member x.Value = value
    
    static member Comparer =
        let gcomparer = HashIdentity.Structural<'T>
        { new IEqualityComparer<StructBox<'T>> with
            member _.GetHashCode(v) = gcomparer.GetHashCode(v.Value)
            member _.Equals(v1, v2) = gcomparer.Equals(v1.Value, v2.Value) }

let comparer = StructBox<int>.Comparer
let box1 = StructBox(42)
let box2 = StructBox(42)
let result = comparer.Equals(box1, box2)
if not result then failwith "Expected equal"
        """
        |> compileExeAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Module level object expression with struct parameter should compile`` () =
        // Module-level functions don't have instance context
        Fsx """
[<Struct>]
type MyStruct(value: int) =
    member x.Value = value

let createComparer () =
    { new System.Object() with
        member _.ToString() = "comparer" }

let c = createComparer()
if c.ToString() <> "comparer" then failwith "Failed"
        """
        |> compileExeAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Object expression in struct not capturing anything should compile`` () =
        // Object expression that doesn't reference any struct state
        Fsx """
[<Struct; NoComparison>]
type MyStruct(value: int) =
    member _.CreateObj() = {
        new System.Object() with
        member _.ToString() = "constant"
    }

let s = MyStruct(42)
let obj = s.CreateObj()
if obj.ToString() <> "constant" then failwith "Failed"
        """
        |> withOptions [ "--nowarn:52" ]
        |> compileExeAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Object expression in struct with method parameters should not confuse params with captures`` () =
        // Method parameters are not instance captures, should not trigger transformation
        Fsx """
[<Struct; NoComparison>]
type MyStruct(value: int) =
    member _.Transform(multiplier: int) = {
        new System.Object() with
        member _.ToString() = string (value * multiplier)
    }

let s = MyStruct(21)
let obj = s.Transform(2)
if obj.ToString() <> "42" then failwith "Expected 42"
        """
        |> withOptions [ "--nowarn:52" ]
        |> compileExeAndRun
        |> shouldSucceed
