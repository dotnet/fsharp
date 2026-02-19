// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

[<Xunit.Collection(nameof FSharp.Test.NotThreadSafeResourceCollection)>]
module FSharp.Compiler.Service.Tests.OverloadCacheTests

open System
open System.IO
open System.Text
open Xunit
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Text
open FSharp.Compiler.Caches
open FSharp.Test.Assert
open FSharp.Compiler.Service.Tests.Common
open TestFramework

let checkSourceHasNoErrors (source: string) =
    let file = Path.ChangeExtension(getTemporaryFileName (), ".fsx")
    let _, checkResults = parseAndCheckScriptPreview (file, source)
    let errors = checkResults.Diagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
    errors |> shouldBeEmpty
    checkResults

let generateRepetitiveOverloadCalls (callCount: int) =
    let sb = StringBuilder()
    sb.AppendLine("open System") |> ignore
    sb.AppendLine() |> ignore
    
    sb.AppendLine("type TestAssert =") |> ignore
    sb.AppendLine("    static member Equal(expected: int, actual: int) = expected = actual") |> ignore
    sb.AppendLine("    static member Equal(expected: string, actual: string) = expected = actual") |> ignore
    sb.AppendLine("    static member Equal(expected: float, actual: float) = expected = actual") |> ignore
    sb.AppendLine("    static member Equal(expected: bool, actual: bool) = expected = actual") |> ignore
    sb.AppendLine("    static member Equal(expected: byte, actual: byte) = expected = actual") |> ignore
    sb.AppendLine("    static member Equal(expected: int16, actual: int16) = expected = actual") |> ignore
    sb.AppendLine("    static member Equal(expected: int64, actual: int64) = expected = actual") |> ignore
    sb.AppendLine("    static member Equal(expected: obj, actual: obj) = obj.Equals(expected, actual)") |> ignore
    sb.AppendLine() |> ignore
    
    sb.AppendLine("let runTests() =") |> ignore
    sb.AppendLine("    let mutable x: int = 0") |> ignore
    sb.AppendLine("    let mutable y: int = 0") |> ignore
    for i in 1 .. callCount do
        sb.AppendLine(sprintf "    x <- %d" i) |> ignore
        sb.AppendLine(sprintf "    y <- %d" (i + 1)) |> ignore
        sb.AppendLine("    ignore (TestAssert.Equal(x, y))") |> ignore
    
    sb.AppendLine() |> ignore
    sb.AppendLine("runTests()") |> ignore
    
    sb.ToString()


[<Fact>]
let ``Overload cache hit rate exceeds 70 percent for repetitive int-int calls`` () =
    use listener = FSharpChecker.CreateOverloadCacheMetricsListener()
    checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()
    
    let callCount = 150
    let source = generateRepetitiveOverloadCalls callCount
    checkSourceHasNoErrors source |> ignore
    
    let hits = listener.Hits
    let misses = listener.Misses
    Assert.True(hits + misses > 0L, "Expected cache activity but got no hits or misses - is the cache enabled?")
    Assert.True(listener.Ratio > 0.70, sprintf "Expected hit ratio > 70%%, but got %.2f%%" (listener.Ratio * 100.0))

[<Fact>]
let ``Overload cache returns correct resolution`` () =
    use listener = FSharpChecker.CreateOverloadCacheMetricsListener()
    checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()
    
    let source = """
type Overloaded =
    static member Process(x: int) = "int"
    static member Process(x: string) = "string"
    static member Process(x: float) = "float"

let r1 = Overloaded.Process(1)
let r2 = Overloaded.Process(2)
let r3 = Overloaded.Process(3)
let r4 = Overloaded.Process(4)
let r5 = Overloaded.Process(5)

let s1 = Overloaded.Process("a")
let s2 = Overloaded.Process("b")

let f1 = Overloaded.Process(1.0)
let f2 = Overloaded.Process(2.0)
"""
    
    checkSourceHasNoErrors source |> ignore
    Assert.True(listener.Hits > 0L, "Expected cache hits for repeated overload calls")

let overloadCorrectnessTestCases () : obj[] seq =
    seq {
        [| "type inference" :> obj;
           """
type Overloaded =
    static member Process(x: int) = "int"
    static member Process(x: string) = "string"
    static member Process(x: float) = "float"
    static member Process(x: 'T list) = "list"

let inferredInt = Overloaded.Process(42)
let inferredString = Overloaded.Process("hello")
let inferredFloat = Overloaded.Process(3.14)
let inferredIntList = Overloaded.Process([1;2;3])
let inferredStringList = Overloaded.Process(["a";"b"])
let explicitInt: string = Overloaded.Process(100)
let explicitString: string = Overloaded.Process("world")
""" :> obj |]

        [| "nested generics" :> obj;
           """
type Container<'T> = { Value: 'T }

type Processor =
    static member Handle(x: Container<int>) = "int container"
    static member Handle(x: Container<string>) = "string container"
    static member Handle(x: Container<Container<int>>) = "nested int container"

let c1 = { Value = 42 }
let c2 = { Value = "hello" }
let c3 = { Value = { Value = 99 } }
let r1 = Processor.Handle(c1)
let r2 = Processor.Handle(c2)
let r3 = Processor.Handle(c3)
let r4 = Processor.Handle({ Value = 123 })
let r5 = Processor.Handle({ Value = "world" })
""" :> obj |]

        [| "out args" :> obj;
           """
open System

let test1 = Int32.TryParse("42")
let test2 = Double.TryParse("3.14")
let test3 = Boolean.TryParse("true")
let (success1: bool, value1: int) = test1
let (success2: bool, value2: float) = test2
let (success3: bool, value3: bool) = test3
let a = Int32.TryParse("1")
let b = Int32.TryParse("2")
let c = Int32.TryParse("3")
""" :> obj |]

        [| "type abbreviations" :> obj;
           """
type IntList = int list
type StringList = string list

type Processor =
    static member Handle(x: int list) = "int list"
    static member Handle(x: string list) = "string list"
    static member Handle(x: int) = "int"

let myIntList: IntList = [1; 2; 3]
let myStringList: StringList = ["a"; "b"]
let r1 = Processor.Handle(myIntList)
let r2 = Processor.Handle(myStringList)
let r3 = Processor.Handle([1; 2; 3])
let r4 = Processor.Handle(["x"; "y"])
let r5 = Processor.Handle(myIntList)
let r6 = Processor.Handle([4; 5; 6])
""" :> obj |]

        [| "inference variables" :> obj;
           """
type Overloaded =
    static member Process(x: int) = "int"
    static member Process(x: string) = "string"
    static member Process<'T>(x: 'T) = "generic"

let a = Overloaded.Process(42)
let b = Overloaded.Process("hello")
let c = Overloaded.Process(true)
let x1 = Overloaded.Process(1)
let x2 = Overloaded.Process(2)
let x3 = Overloaded.Process(3)
let y1 = Overloaded.Process("a")
let y2 = Overloaded.Process("b")
let y3 = Overloaded.Process("c")
""" :> obj |]

        [| "type subsumption" :> obj;
           """
open System.Collections.Generic

type Animal() = class end
type Dog() = inherit Animal()
type Cat() = inherit Animal()

type Zoo =
    static member Accept(animals: IEnumerable<Animal>) = "animals"
    static member Accept(dogs: IList<Dog>) = "dogs"
    static member Accept(x: obj) = "obj"

let dogs: IList<Dog> = [Dog(); Dog()] |> ResizeArray :> IList<Dog>
let animals: IEnumerable<Animal> = [Animal(); Dog(); Cat()] |> Seq.ofList
let r1 = Zoo.Accept(dogs)
let r2 = Zoo.Accept(animals)
let r3 = Zoo.Accept(42)
let d1 = Zoo.Accept(dogs)
let d2 = Zoo.Accept(dogs)
let d3 = Zoo.Accept(dogs)
let a1 = Zoo.Accept(animals)
let a2 = Zoo.Accept(animals)
let a3 = Zoo.Accept(animals)
let inline testWith<'T when 'T :> Animal>(items: seq<'T>) = Zoo.Accept(items)
let dogSeq = [Dog(); Dog()] |> Seq.ofList
let catSeq = [Cat(); Cat()] |> Seq.ofList
let t1 = testWith dogSeq
let t2 = testWith catSeq
""" :> obj |]

        [| "known vs inferred types" :> obj;
           """
type Overloaded =
    static member Call(x: int) = "int"
    static member Call(x: string) = "string"
    static member Call(x: float) = "float"

let r1 = Overloaded.Call(42)
let r2 = Overloaded.Call("hello")
let r3 = Overloaded.Call(3.14)
let a1 = Overloaded.Call(1)
let a2 = Overloaded.Call(2)
let a3 = Overloaded.Call(3)
let a4 = Overloaded.Call(4)
let a5 = Overloaded.Call(5)
let s1 = Overloaded.Call("a")
let s2 = Overloaded.Call("b")
let s3 = Overloaded.Call("c")
""" :> obj |]

        [| "generic overloads" :> obj;
           """
type GenericOverload =
    static member Process(x: int) = "int"
    static member Process(x: string) = "string"
    static member Process<'T>(x: 'T) = "generic"

let r1 = GenericOverload.Process(42)
let r2 = GenericOverload.Process("hello")
let r3 = GenericOverload.Process<bool>(true)
let x1 = GenericOverload.Process(1)
let x2 = GenericOverload.Process(2)
let x3 = GenericOverload.Process(3)
""" :> obj |]

        [| "nested generic types" :> obj;
           """
type Processor =
    static member Handle(x: int list) = "int list"
    static member Handle(x: string list) = "string list"
    static member Handle(x: float list) = "float list"

let r1 = Processor.Handle([1; 2; 3])
let r2 = Processor.Handle(["a"; "b"; "c"])
let r3 = Processor.Handle([1.0; 2.0; 3.0])
let a1 = Processor.Handle([1])
let a2 = Processor.Handle([2])
let a3 = Processor.Handle([3])
""" :> obj |]
    }

[<Theory>]
[<MemberData(nameof overloadCorrectnessTestCases)>]
let ``Overload resolution correctness`` (_scenario: string, source: string) =
    checkSourceHasNoErrors source |> ignore

[<Fact>]
let ``Overload cache benefits from rigid generic type parameters`` () =
    use listener = FSharpChecker.CreateOverloadCacheMetricsListener()
    checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()
    
    let source = """
type Assert =
    static member Equal(expected: int, actual: int) = expected = actual
    static member Equal(expected: string, actual: string) = expected = actual
    static member Equal(expected: float, actual: float) = expected = actual
    static member Equal<'T when 'T: equality>(expected: 'T, actual: 'T) = expected = actual

let inline check<'T when 'T: equality>(x: 'T, y: 'T) = Assert.Equal(x, y)

let test1() = check(1, 2)
let test2() = check(3, 4)
let test3() = check(5, 6)
let test4() = check("a", "b")
let test5() = check("c", "d")
let test6() = check(1.0, 2.0)
let test7() = check(3.0, 4.0)

let d1 = Assert.Equal<int>(10, 20)
let d2 = Assert.Equal<int>(30, 40)
let d3 = Assert.Equal<string>("x", "y")
let d4 = Assert.Equal<string>("z", "w")
"""
    
    checkSourceHasNoErrors source |> ignore
