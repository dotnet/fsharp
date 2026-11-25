// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// These tests are serialized (NotThreadSafeResourceCollection) because they read process-global state:
// the shared language-service `checker`, and the process-global cache metrics that
// `CacheMetrics.ListenToAll` aggregates by name (see the `use _ = CacheMetrics.ListenToAll()` in each
// test). Running them in parallel with each other, or alongside anything else that drives caches while
// a listener is attached, would let counts from unrelated work bleed into the before/after deltas.
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
    use _ = CacheMetrics.ListenToAll()
    checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()

    // Measure only this compilation's activity: the per-name totals are process-global, so snapshot
    // before/after and diff rather than reading absolute counts.
    let before = CacheMetrics.getTotalsByName "overloadResolutionCache"

    let callCount = 150
    let source = generateRepetitiveOverloadCalls callCount
    checkSourceHasNoErrors source |> ignore

    let after = CacheMetrics.getTotalsByName "overloadResolutionCache"
    let hits = after.["hits"] - before.["hits"]
    let misses = after.["misses"] - before.["misses"]
    Assert.True(hits + misses > 0L, "Expected cache activity but got no hits or misses - is the cache enabled?")
    let ratio = float hits / float (hits + misses)
    Assert.True(ratio > 0.70, sprintf "Expected hit ratio > 70%%, but got %.2f%%" (ratio * 100.0))

[<Fact>]
let ``Overload cache returns correct resolution`` () =
    use _ = CacheMetrics.ListenToAll()
    checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()

    let before = CacheMetrics.getTotalsByName "overloadResolutionCache"

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

    let after = CacheMetrics.getTotalsByName "overloadResolutionCache"
    Assert.True(after.["hits"] - before.["hits"] > 0L, "Expected cache hits for repeated overload calls")

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

let checkProject (otherOptions: string list) (sources: string list) =
    let options = createProjectOptions sources otherOptions
    let results = checker.ParseAndCheckProject options |> Async.RunSynchronouslyImmediate
    results.Diagnostics

let errorLines (diagnostics: FSharpDiagnostic[]) =
    diagnostics
    |> Seq.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
    |> Seq.map _.StartLine
    |> Seq.distinct
    |> Seq.toList

// The calls of a case differ only in what the case names and must choose different overloads.
// The first call chooses an overload that also applies to the second, so that reusing it is not caught
// when it is checked again. A wrong overload is a type error in the last line.
let callsDifferingInOneWayTestCases () : obj[] seq =
    seq {
        [| "explicit type arguments" :> obj;
           """
type Overloaded =
    static member Pick<'T>(x: 'T) = 1
    static member Pick<'T>(x: obj, ?y: 'T) = "obj"
""" :> obj;
           """
let asInt = Overloaded.Pick<int>("a")
let asString = Overloaded.Pick<string>("a")

let expected: string * int = asInt, asString
""" :> obj |]

        [| "type arguments of the declaring type" :> obj;
           """
type Overloaded<'T> =
    static member Pick(x: 'T) = 1
    static member Pick(x: obj) = "obj"
""" :> obj;
           """
let other = Overloaded<string>.Pick(1)
let matching = Overloaded<int>.Pick(1)
let otherAgain = Overloaded<string>.Pick(1)

let expected: string * int * string = other, matching, otherAgain
""" :> obj |]

        [| "type arguments solved by a lambda argument" :> obj;
           """
type Overloaded =
    static member Pick<'T>(x: 'T, f: 'T -> int) = "generic"
    static member Pick(x: System.IComparable, f: int -> int) = 1

let increment (x: int) = x + 1
""" :> obj;
           """
let fromFunction = Overloaded.Pick(1, increment)
let fromLambda = Overloaded.Pick(1, (fun x -> x + 1))

let expected: int * string = fromFunction, fromLambda
""" :> obj |]

        [| "function types" :> obj;
           """
type Overloaded =
    static member Pick(f: int -> int -> int) = 1
    static member Pick(f: obj) = "obj"

let higherOrder: (int -> int) -> int = fun f -> f 1
let curried: int -> int -> int = fun a b -> a + b
""" :> obj;
           """
let a = Overloaded.Pick(higherOrder)
let b = Overloaded.Pick(curried)

let expected: string * int = a, b
""" :> obj |]

        [| "nested tuples" :> obj;
           """
type Overloaded =
    static member Pick(x: ((int * int) * int * int) list) = 1
    static member Pick(x: obj) = "obj"

let tripleFirst = [ ((1, 2, 3), 4) ]
let pairFirst = [ ((1, 2), 3, 4) ]
""" :> obj;
           """
let a = Overloaded.Pick(tripleFirst)
let b = Overloaded.Pick(pairFirst)

let expected: string * int = a, b
""" :> obj |]

        [| "type parameters" :> obj;
           """
type Overloaded =
    static member Same<'T>(x: 'T, y: 'T, z: int) = 1
    static member Same(x: obj, y: obj, z: obj) = "obj"
""" :> obj;
           """
let calls<'a, 'b> (a: 'a) (b: 'b) =
    let different = Overloaded.Same(a, b, 1)
    let same = Overloaded.Same(a, a, 1)
    let expected: string * int = different, same
    ()
""" :> obj |]

        [| "measure type parameters" :> obj;
           """
type Overloaded =
    static member Same<[<Measure>] 'w>(x: float<'w>, y: float<'w>) = 1
    static member Same(x: obj, y: obj) = "obj"
""" :> obj;
           """
let calls<[<Measure>] 'u, [<Measure>] 'v> (a: float<'u>) (b: float<'v>) =
    let different = Overloaded.Same(a, b)
    let same = Overloaded.Same(a, a)
    let expected: string * int = different, same
    ()
""" :> obj |]

        [| "units of measure" :> obj;
           """
[<Measure>] type m
[<Measure>] type s

type Overloaded =
    static member Pick(x: float<s/m>) = "per metre"
    static member Pick(x: float<'u>, ?y: int) = 1
""" :> obj;
           """
let product: float<s m> = 1.0<_>
let quotient: float<s/m> = 1.0<_>
let a = Overloaded.Pick(product)
let b = Overloaded.Pick(quotient)

let expected: int * string = a, b
""" :> obj |]

        [| "expected type of out arguments" :> obj;
           """
open System.Runtime.InteropServices

type Overloaded =
    static member TryGet(key: string, [<Out>] value: byref<int>) : bool =
        value <- 1
        true

    static member TryGet(key: string, [<Out>] value: byref<string>) : bool =
        value <- ""
        true
""" :> obj;
           """
let asInt: bool * int = Overloaded.TryGet("key")
let asString: bool * string = Overloaded.TryGet("key")
let asIntAgain: bool * int = Overloaded.TryGet("key")
""" :> obj |]
    }

[<Theory>]
[<MemberData(nameof callsDifferingInOneWayTestCases)>]
let ``Calls differing in one way`` (_scenario: string, declarations: string, calls: string) =
    checkSourceHasNoErrors (declarations + calls) |> ignore

    // A type of the file being checked is not cached the way a type of an earlier file is.
    checkProject
        []
        [
            "module Declarations\n" + declarations
            "module Calls\nopen Declarations\n" + calls
        ]
    |> errorLines
    |> shouldBeEmpty

[<Fact>]
let ``Calls differing in nullness`` () =
    checkProject
        [ "--checknulls+" ]
        [
            """module Declarations

type Overloaded =
    static member Pick(x: System.IComparable) = "comparable"
    static member Pick(x: obj | null) = 1
"""
            """module Calls

open Declarations

let calls (notNull: string) (maybeNull: string | null) =
    let a = Overloaded.Pick(notNull)
    let b = Overloaded.Pick(maybeNull)
    let c = Overloaded.Pick(notNull)
    let expected: string * int * string = a, b, c
    ()
"""
        ]
    |> errorLines
    |> shouldBeEmpty

[<Fact>]
let ``Conversion declared after a call`` () =
    let source =
        """
open System

type Target = { Value: int }
type Source() = class end

type Overloaded =
    static member Pick(x: Target, y: DateTimeOffset) = 1
    static member Pick(x: obj, y: DateTimeOffset) = "obj"

let now = DateTime.Now
let before = Overloaded.Pick(Source(), now)

type Source with
    static member op_Implicit(source: Source) : Target = { Value = 1 }

let after = Overloaded.Pick(Source(), now)

let expected: string * int = before, after
"""

    checkSourceHasNoErrors source |> ignore

[<Fact>]
let ``Conversion private to one of the callers`` () =
    let source =
        """
type Target = { Value: int }

type Outside =
    static member Call() : string = Overloaded.Pick(Source(), 1)

and Source() =
    static member private op_Implicit(source: Source) : Target = { Value = 1 }
    static member Call() : int = Overloaded.Pick(Source(), 1)

and Overloaded =
    static member Pick(x: Target, y: int) : int = 1
    static member Pick(x: obj, y: int64) : string = "obj"
"""

    checkSourceHasNoErrors source |> ignore

[<Fact>]
let ``Repeated call reports the same error`` () =
    let diagnostics =
        checkProject
            []
            [
                """module Calls

type Inner =
    static member Pick(x: obj) = 1
    static member Pick(x: System.Uri) = ""

type Outer =
    static member Take(x: int64) = ()

let first = Outer.Take(Inner.Pick("a"))
let second = Outer.Take(Inner.Pick("a"))
"""
            ]

    Assert.Equal<int list>([ 10; 11 ], errorLines diagnostics)

[<Fact>]
let ``Repeated call reports the same tiebreak warning`` () =
    let diagnostics =
        checkProject
            [ "--warnon:3575" ]
            [
                """module Declarations

type Overloaded =
    static member Take<'T>(x: 'T option) = 1
    static member Take<'T>(x: 'T list option) = "list"
"""
                """module Calls

open Declarations

let first = Overloaded.Take(Some [ 1 ])
let second = Overloaded.Take(Some [ 1 ])
"""
            ]

    let warningLines =
        diagnostics
        |> Seq.filter (fun d -> d.ErrorNumber = 3575)
        |> Seq.map _.StartLine
        |> Seq.toList

    Assert.Equal<int list>([ 5; 6 ], warningLines)

[<Fact>]
let ``Call in an attribute of a type being defined`` () =
    let diagnostics =
        checkProject
            []
            [
                """module Declarations

type Overloaded =
    static member Pick(x: System.IDisposable) : int = 1
    static member Pick(x: obj) : string = "obj"
"""
                """module Calls

open System
open Declarations

[<Obsolete(Overloaded.Pick(Unchecked.defaultof<Derived>))>]
type Marked = interface end

and Derived =
    class
        inherit System.IO.MemoryStream
    end

let expected: int = Overloaded.Pick(Unchecked.defaultof<Derived>)
"""
            ]

    Assert.DoesNotContain(14, errorLines diagnostics)
