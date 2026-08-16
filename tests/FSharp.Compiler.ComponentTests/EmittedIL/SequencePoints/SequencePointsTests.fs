module EmittedIL.SequencePointsTests

open Xunit
open FSharp.Test.Compiler

let private baseline = SequencePointsBaseline(__SOURCE_DIRECTORY__)

module Function =
    [<Fact>]
    let ``Body - Unit 01`` () =
        baseline.verify """
module Module

let f () =
    ()
"""

    [<Fact>]
    let ``Body - LetThenValue 01`` () =
        baseline.verify """
module Module

let f () =
    let i = 1
    1
"""

    [<Fact>]
    let ``Body - SequentialUnits 01`` () =
        baseline.verify """
module Module

let f () =
    ()
    ()
"""

    [<Fact>]
    let ``Body - ErasedCall 01`` () =
        baseline.verify """
module Module

let f () =
    System.Diagnostics.Debug.Write ""
"""

    [<Fact>]
    let ``Body - Erased call 02`` () =
        baseline.verify """
module Module

let f () =
    System.Diagnostics.Debug.Write ""
    System.Diagnostics.Debug.Write ""
"""

    [<Fact>]
    let ``Body - Erased call 03`` () =
        baseline.verify """
module Module

let f () =
    System.Diagnostics.Debug.Write ""
    System.Diagnostics.Debug.Write ""
    System.Diagnostics.Debug.Write ""
"""

    [<Fact>]
    let ``Body - Erased call 04`` () =
        baseline.verify """
module Module

let f () =
    ()
    System.Diagnostics.Debug.Write ""
    System.Diagnostics.Debug.Write ""
"""

    [<Fact>]
    let ``Body - Erased call 05`` () =
        baseline.verify """
module Module

let f () =
    System.Diagnostics.Debug.Write ""
    ()
    System.Diagnostics.Debug.Write ""
"""

    [<Fact>]
    let ``Body - Erased call 06`` () =
        baseline.verify """
module Module

let f () =
    System.Diagnostics.Debug.Write ""
    System.Diagnostics.Debug.Write ""
    ()
"""

    [<Fact>]
    let ``Body - ErasedThenKeptCall 01`` () =
        baseline.verify """
module Module

let f () =
    System.Diagnostics.Debug.Write ""
    System.Console.WriteLine ""
"""

    [<Fact>]
    let ``Body - KeptThenErasedCall 01`` () =
        baseline.verify """
module Module

let f () =
    System.Console.WriteLine ""
    System.Diagnostics.Debug.Write ""
"""

module ForEach =
    [<Fact>]
    let ``List - Simple 01`` () =
        baseline.verify """
module Module

let f (l: int list) =
    for i in l do
        ()
"""

    [<Fact>]
    let ``List - Body - SingleStatement 01`` () =
        baseline.verify """
module Module

let f (l: int list) =
    for i in l do
        System.Console.WriteLine i
"""

    [<Fact>]
    let ``List - Body - MultipleStatements 01`` () =
        baseline.verify """
module Module

let f (l: int list) =
    for i in l do
        System.Console.WriteLine i
        System.Console.WriteLine(i + 1)
"""

    [<Fact>]
    let ``List - Body - ErasedCall 01`` () =
        baseline.verify """
module Module

let f (l: int list) =
    for i in l do
        System.Diagnostics.Debug.Write ""
"""

    [<Fact>]
    let ``List - Body - ErasedThenKeptCall 01`` () =
        baseline.verify """
module Module

let f (l: int list) =
    for i in l do
        System.Diagnostics.Debug.Write ""
        System.Console.WriteLine ""
"""

    [<Fact>]
    let ``List - Body - KeptThenErasedCall 01`` () =
        baseline.verify """
module Module

let f (l: int list) =
    for i in l do
        System.Console.WriteLine ""
        System.Diagnostics.Debug.Write ""
"""

    [<Fact>]
    let ``List - Body - ParenUnit 01`` () =
        baseline.verify """
module Module

let f (l: int list) =
    for i in l do
        (())
"""

    [<Fact>]
    let ``List - Body - SequentialUnits 01`` () =
        baseline.verify """
module Module

let f (l: int list) =
    for i in l do
        ()
        ()
"""

    [<Fact>]
    let ``List - Body - LetUnit 01`` () =
        baseline.verify """
module Module

let f (l: int list) =
    for i in l do
        let _ = ()
        ()
"""

    [<Fact>]
    let ``List - Pattern - Tuple 01`` () =
        baseline.verify """
module Module

let f (l: (int * int) list) =
    for i1, i2 in l do
        ()
"""

    [<Fact>]
    let ``List - Pattern - ActivePattern 01`` () =
        baseline.verify """
module Module

let (|Id|) (x: int) = x

let f (l: int list) =
    for Id i in l do
        ()
"""

    [<Fact>]
    let ``List - Comprehensions - Value 01`` () =
        baseline.verify """
module Module

let a =
    [
        for n in 1..10 do
            yield n
    ]
"""

    [<Fact>]
    let ``List - Comprehensions - Value 02`` () =
        baseline.verify """
module Module

let f (l: int list) =
    [
        for n in l do
            yield n
    ]
"""

    [<Fact>]
    let ``List - Comprehensions - Tuple 01`` () =
        baseline.verify """
module Module

let f (l: (int * int) list) =
    [
        for n in l do
            yield n
    ]
"""

    [<Fact>]
    let ``List - Comprehensions - Tuple 02`` () =
        baseline.verify """
module Module

let f (l: (int * int) list) =
    [
        for i, i1 in l do
            yield i
    ]
"""

    [<Fact>]
    let ``List - Comprehensions - Arrow 01`` () =
        baseline.verify """
module Module

let f (l: int list) =
    [
        for n in l -> n
    ]
"""

    [<Fact>]
    let ``List - Comprehensions - ActivePattern 01`` () =
        baseline.verify """
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
        baseline.verify """
module Module

let f (l: int[]) =
    for i in l do
        ()
"""

    [<Fact>]
    let ``Array - Body - SingleStatement 01`` () =
        baseline.verify """
module Module

let f (l: int[]) =
    for i in l do
        System.Console.WriteLine i
"""

    [<Fact>]
    let ``Array - Body - MultipleStatements 01`` () =
        baseline.verify """
module Module

let f (l: int[]) =
    for i in l do
        System.Console.WriteLine i
        System.Console.WriteLine(i + 1)
"""

    [<Fact>]
    let ``Array - Pattern - Tuple 01`` () =
        baseline.verify """
module Module

let f (l: (int * int)[]) =
    for i1, i2 in l do
        ()
"""

    [<Fact>]
    let ``Array - Pattern - ActivePattern 01`` () =
        baseline.verify """
module Module

let (|Id|) (x: int) = x

let f (l: int[]) =
    for Id i in l do
        ()
"""

    [<Fact>]
    let ``Array - Comprehensions - Value 01`` () =
        baseline.verify """
module Module

let a =
    [|
        for n in 1..10 do
            yield n
    |]
"""

    [<Fact>]
    let ``Array - Comprehensions - Value 02`` () =
        baseline.verify """
module Module

let f (l: int list) =
    [|
        for n in l do
            yield n
    |]
"""

    [<Fact>]
    let ``Array - Comprehensions - Tuple 01`` () =
        baseline.verify """
module Module

let f (l: (int * int) list) =
    [|
        for n in l do
            yield n
    |]
"""

    [<Fact>]
    let ``Array - Comprehensions - Tuple 02`` () =
        baseline.verify """
module Module

let f (l: (int * int) list) =
    [|
        for i, i1 in l do
            yield i
    |]
"""

    [<Fact>]
    let ``Array - Comprehensions - Arrow 01`` () =
        baseline.verify """
module Module

let f (l: int list) =
    [|
        for n in l -> n
    |]
"""

    [<Fact>]
    let ``Array - Comprehensions - ActivePattern 01`` () =
        baseline.verify """
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
        baseline.verify """
module Module

let f (l: int seq) =
    for i in l do
        ()
"""

    [<Fact>]
    let ``Seq - Body - SingleStatement 01`` () =
        baseline.verify """
module Module

let f (l: int seq) =
    for i in l do
        System.Console.WriteLine i
"""

    [<Fact>]
    let ``Seq - Body - MultipleStatements 01`` () =
        baseline.verify """
module Module

let f (l: int seq) =
    for i in l do
        System.Console.WriteLine i
        System.Console.WriteLine(i + 1)
"""

    [<Fact>]
    let ``Seq - Pattern - Tuple 01`` () =
        baseline.verify """
module Module

let f (l: (int * int) seq) =
    for i1, i2 in l do
        ()
"""

    [<Fact>]
    let ``Seq - Pattern - ActivePattern 01`` () =
        baseline.verify """
module Module

let (|Id|) (x: int) = x

let f (l: int seq) =
    for Id i in l do
        ()
"""

    [<Fact>]
    let ``Seq - Comprehensions - Value 01`` () =
        baseline.verify """
module Module

let a =
    seq {
        for n in 1..10 do
            yield n
    }
"""

    [<Fact>]
    let ``Seq - Comprehensions - Value 02`` () =
        baseline.verify """
module Module

let f (l: int list) =
    seq {
        for n in l do
            yield n
    }
"""

    [<Fact>]
    let ``Seq - Comprehensions - Tuple 01`` () =
        baseline.verify """
module Module

let f (l: (int * int) list) =
    seq {
        for n in l do
            yield n
    }
"""

    [<Fact>]
    let ``Seq - Comprehensions - Tuple 02`` () =
        baseline.verify """
module Module

let f (l: (int * int) list) =
    seq {
        for i, i1 in l do
            yield i
    }
"""

    [<Fact>]
    let ``Seq - Comprehensions - Arrow 01`` () =
        baseline.verify """
module Module

let f (l: int list) =
    seq {
        for n in l -> n
    }
"""

    [<Fact>]
    let ``Seq - Comprehensions - ActivePattern 01`` () =
        baseline.verify """
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
        baseline.verify """
module Module

let f (l: string) =
    for c in l do
        System.Console.WriteLine c
"""

module If =
    [<Fact>]
    let ``If 01`` () =
        baseline.verify """
module Module

let f (x: int) =
    x + x

let g () =
    if f 5 = 10 then 0 else 1
"""

    [<Fact>]
    let ``If 02 - Bind`` () =
        baseline.verify """
module Module

let f (x: int) =
    x + x

let g () =
    let y = if f 5 = 10 then 0 else 1
    y + y
"""

    [<Fact>]
    let ``If 03 - Set`` () =
        baseline.verify """
module Module

let f (x: int) =
    x + x

let g (arr: int[]) =
    arr[0] <- if f 5 = 10 then 0 else 1
"""

    [<Fact>]
    let ``If 04`` () =
        baseline.verify """
module Module

let f (x: int) =
    x + x

let g () =
    (if f 5 = 10 then 0 else 1) + f 1
"""

    [<Fact>]
    let ``If 05`` () =
        baseline.verify """
module Module

let f (x: int) =
    x + x

let g () =
    let mutable x = 0
    if f 5 = 10 then x <- 1 else x <- 2
    x + x
"""

module Match =
    [<Fact>]
    let ``Match 01`` () =
        baseline.verify """
module Module

let f (x: int) =
    x + x

let g () =
    match f 5 with
    | 10 -> 0
    | _ -> 1
"""

    [<Fact>]
    let ``Match 02 - Bind`` () =
        baseline.verify """
module Module

let f (x: int) =
    x + x

let g () =
    let i =
        match f 5 with
        | 10 -> 0
        | _ -> 1
    i + 1
"""

module Binding =
    [<Fact>]
    let ``Module - Unit 01`` () =
        baseline.verify """
module Module

let i = ()
"""

    [<Fact>]
    let ``Module - SequentialUnits 01`` () =
        baseline.verify """
module Module

let i = ()
let j = ()
"""

    [<Fact>]
    let ``Module - ErasedCall 01`` () =
        baseline.verify """
module Module

let i = System.Diagnostics.Debug.Write ""
"""

    [<Fact>]
    let ``Module - Erased call 02`` () =
        baseline.verify """
module Module

let i =
    System.Diagnostics.Debug.Write ""
    System.Diagnostics.Debug.Write ""
"""

    [<Fact>]
    let ``Module - Erased call 03`` () =
        baseline.verify """
module Module

let i =
    System.Diagnostics.Debug.Write ""
    System.Diagnostics.Debug.Write ""
    System.Diagnostics.Debug.Write ""
"""

    [<Fact>]
    let ``Module - Erased call 04`` () =
        baseline.verify """
module Module

let i =
    ()
    System.Diagnostics.Debug.Write ""
    System.Diagnostics.Debug.Write ""
"""

    [<Fact>]
    let ``Module - Erased call 05`` () =
        baseline.verify """
module Module

let i =
    System.Diagnostics.Debug.Write ""
    ()
    System.Diagnostics.Debug.Write ""
"""

    [<Fact>]
    let ``Module - Erased call 06`` () =
        baseline.verify """
module Module

let i =
    System.Diagnostics.Debug.Write ""
    System.Diagnostics.Debug.Write ""
    ()
"""

    [<Fact>]
    let ``Local - Unit 01`` () =
        baseline.verify """
module Module

do
    let i = ()
    ()
"""

    [<Fact>]
    let ``Local - SequentialUnits 01`` () =
        baseline.verify """
module Module

do
    let i = ()
    let j = ()
    ()
"""

    [<Fact>]
    let ``Local - ErasedCall 01`` () =
        baseline.verify """
module Module

do
    let i = System.Diagnostics.Debug.Write ""
    ()
"""

    [<Fact>]
    let ``Local - Erased call 02`` () =
        baseline.verify """
module Module

let i =
    System.Diagnostics.Debug.Write ""
    System.Diagnostics.Debug.Write ""
"""

    [<Fact>]
    let ``Local - Erased call 03`` () =
        baseline.verify """
module Module

let i =
    System.Diagnostics.Debug.Write ""
    System.Diagnostics.Debug.Write ""
    System.Diagnostics.Debug.Write ""
"""

    [<Fact>]
    let ``Local - Erased call 04`` () =
        baseline.verify """
module Module

let i =
    ()
    System.Diagnostics.Debug.Write ""
    System.Diagnostics.Debug.Write ""
"""

    [<Fact>]
    let ``Local - Erased call 05`` () =
        baseline.verify """
module Module

let i =
    System.Diagnostics.Debug.Write ""
    ()
    System.Diagnostics.Debug.Write ""
"""

    [<Fact>]
    let ``Local - Erased call 06`` () =
        baseline.verify """
module Module

let i =
    System.Diagnostics.Debug.Write ""
    System.Diagnostics.Debug.Write ""
    ()
"""