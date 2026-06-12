module EmittedIL.SequencePointsTests

open System.Diagnostics
open System.IO
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Xunit
open FSharp.Test.Compiler

[<AbstractClass; Sealed>]
type private Baseline =
    static member verify(source, [<CallerMemberName; Optional; DefaultParameterValue("")>] name: string) =
        let moduleName = StackTrace().GetFrame(1).GetMethod().DeclaringType.Name
        FSharp source
        |> asLibrary
        |> withPortablePdb
        |> withNoOptimize
        |> compile
        |> shouldSucceed
        |> verifySequencePointsBaseline source (Path.Combine(__SOURCE_DIRECTORY__, moduleName, name + ".bsl"))
        |> ignore

module Function =
    [<Fact>]
    let ``Body - Unit 01`` () =
        Baseline.verify """
module Module

let f () =
    ()
"""

    [<Fact>]
    let ``Body - LetThenValue 01`` () =
        Baseline.verify """
module Module

let f () =
    let i = 1
    1
"""

    [<Fact>]
    let ``Body - SequentialUnits 01`` () =
        Baseline.verify """
module Module

let f () =
    ()
    ()
"""

module ForEach =
    [<Fact>]
    let ``List - Simple 01`` () =
        Baseline.verify """
module Module

let f (l: int list) =
    for i in l do
        ()
"""

    [<Fact>]
    let ``List - Body - SingleStatement 01`` () =
        Baseline.verify """
module Module

let f (l: int list) =
    for i in l do
        System.Console.WriteLine i
"""

    [<Fact>]
    let ``List - Body - MultipleStatements 01`` () =
        Baseline.verify """
module Module

let f (l: int list) =
    for i in l do
        System.Console.WriteLine i
        System.Console.WriteLine(i + 1)
"""

    [<Fact>]
    let ``List - Body - ParenUnit 01`` () =
        Baseline.verify """
module Module

let f (l: int list) =
    for i in l do
        (())
"""

    [<Fact>]
    let ``List - Body - SequentialUnits 01`` () =
        Baseline.verify """
module Module

let f (l: int list) =
    for i in l do
        ()
        ()
"""

    [<Fact>]
    let ``List - Body - LetUnit 01`` () =
        Baseline.verify """
module Module

let f (l: int list) =
    for i in l do
        let _ = ()
        ()
"""

    [<Fact>]
    let ``List - Pattern - Tuple 01`` () =
        Baseline.verify """
module Module

let f (l: (int * int) list) =
    for i1, i2 in l do
        ()
"""

    [<Fact>]
    let ``List - Pattern - ActivePattern 01`` () =
        Baseline.verify """
module Module

let (|Id|) (x: int) = x

let f (l: int list) =
    for Id i in l do
        ()
"""

    [<Fact>]
    let ``List - Comprehensions - Value 01`` () =
        Baseline.verify """
module Module

let a =
    [
        for n in 1..10 do
            yield n
    ]
"""

    [<Fact>]
    let ``List - Comprehensions - Value 02`` () =
        Baseline.verify """
module Module

let f (l: int list) =
    [
        for n in l do
            yield n
    ]
"""

    [<Fact>]
    let ``List - Comprehensions - Tuple 01`` () =
        Baseline.verify """
module Module

let f (l: (int * int) list) =
    [
        for n in l do
            yield n
    ]
"""

    [<Fact>]
    let ``List - Comprehensions - Tuple 02`` () =
        Baseline.verify """
module Module

let f (l: (int * int) list) =
    [
        for i, i1 in l do
            yield i
    ]
"""

    [<Fact>]
    let ``List - Comprehensions - Arrow 01`` () =
        Baseline.verify """
module Module

let f (l: int list) =
    [
        for n in l -> n
    ]
"""

    [<Fact>]
    let ``List - Comprehensions - ActivePattern 01`` () =
        Baseline.verify """
module Module

let (|Id|) (x: int) = x

let f (l: int list) =
    [
        for Id i in l do
            yield i
    ]
"""

    [<Fact>]
    let ``Array - Simple 01`` () =
        Baseline.verify """
module Module

let f (l: int[]) =
    for i in l do
        ()
"""

    [<Fact>]
    let ``Array - Body - SingleStatement 01`` () =
        Baseline.verify """
module Module

let f (l: int[]) =
    for i in l do
        System.Console.WriteLine i
"""

    [<Fact>]
    let ``Array - Body - MultipleStatements 01`` () =
        Baseline.verify """
module Module

let f (l: int[]) =
    for i in l do
        System.Console.WriteLine i
        System.Console.WriteLine(i + 1)
"""

    [<Fact>]
    let ``Array - Pattern - Tuple 01`` () =
        Baseline.verify """
module Module

let f (l: (int * int)[]) =
    for i1, i2 in l do
        ()
"""

    [<Fact>]
    let ``Array - Pattern - ActivePattern 01`` () =
        Baseline.verify """
module Module

let (|Id|) (x: int) = x

let f (l: int[]) =
    for Id i in l do
        ()
"""

    [<Fact>]
    let ``Array - Comprehensions - Value 01`` () =
        Baseline.verify """
module Module

let a =
    [|
        for n in 1..10 do
            yield n
    |]
"""

    [<Fact>]
    let ``Array - Comprehensions - Value 02`` () =
        Baseline.verify """
module Module

let f (l: int list) =
    [|
        for n in l do
            yield n
    |]
"""

    [<Fact>]
    let ``Array - Comprehensions - Tuple 01`` () =
        Baseline.verify """
module Module

let f (l: (int * int) list) =
    [|
        for n in l do
            yield n
    |]
"""

    [<Fact>]
    let ``Array - Comprehensions - Tuple 02`` () =
        Baseline.verify """
module Module

let f (l: (int * int) list) =
    [|
        for i, i1 in l do
            yield i
    |]
"""

    [<Fact>]
    let ``Array - Comprehensions - Arrow 01`` () =
        Baseline.verify """
module Module

let f (l: int list) =
    [|
        for n in l -> n
    |]
"""

    [<Fact>]
    let ``Array - Comprehensions - ActivePattern 01`` () =
        Baseline.verify """
module Module

let (|Id|) (x: int) = x

let f (l: int list) =
    [|
        for Id i in l do
            yield i
    |]
"""

    [<Fact>]
    let ``Seq - Simple 01`` () =
        Baseline.verify """
module Module

let f (l: int seq) =
    for i in l do
        ()
"""

    [<Fact>]
    let ``Seq - Body - SingleStatement 01`` () =
        Baseline.verify """
module Module

let f (l: int seq) =
    for i in l do
        System.Console.WriteLine i
"""

    [<Fact>]
    let ``Seq - Body - MultipleStatements 01`` () =
        Baseline.verify """
module Module

let f (l: int seq) =
    for i in l do
        System.Console.WriteLine i
        System.Console.WriteLine(i + 1)
"""

    [<Fact>]
    let ``Seq - Pattern - Tuple 01`` () =
        Baseline.verify """
module Module

let f (l: (int * int) seq) =
    for i1, i2 in l do
        ()
"""

    [<Fact>]
    let ``Seq - Pattern - ActivePattern 01`` () =
        Baseline.verify """
module Module

let (|Id|) (x: int) = x

let f (l: int seq) =
    for Id i in l do
        ()
"""

    [<Fact>]
    let ``Seq - Comprehensions - Value 01`` () =
        Baseline.verify """
module Module

let a =
    seq {
        for n in 1..10 do
            yield n
    }
"""

    [<Fact>]
    let ``Seq - Comprehensions - Value 02`` () =
        Baseline.verify """
module Module

let f (l: int list) =
    seq {
        for n in l do
            yield n
    }
"""

    [<Fact>]
    let ``Seq - Comprehensions - Tuple 01`` () =
        Baseline.verify """
module Module

let f (l: (int * int) list) =
    seq {
        for n in l do
            yield n
    }
"""

    [<Fact>]
    let ``Seq - Comprehensions - Tuple 02`` () =
        Baseline.verify """
module Module

let f (l: (int * int) list) =
    seq {
        for i, i1 in l do
            yield i
    }
"""

    [<Fact>]
    let ``Seq - Comprehensions - Arrow 01`` () =
        Baseline.verify """
module Module

let f (l: int list) =
    seq {
        for n in l -> n
    }
"""

    [<Fact>]
    let ``Seq - Comprehensions - ActivePattern 01`` () =
        Baseline.verify """
module Module

let (|Id|) (x: int) = x

let f (l: int list) =
    seq {
        for Id i in l do
            yield i
    }
"""

    [<Fact>]
    let ``String - Body - SingleStatement 01`` () =
        Baseline.verify """
module Module

let f (l: string) =
    for c in l do
        System.Console.WriteLine c
"""
