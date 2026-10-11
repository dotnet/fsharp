// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Language

open Xunit
open FSharp.Test.Compiler

// FS-1031: ranges mixed with values in list, array, sequence and computation expressions.
module MixedSequenceExpressionTests =

    let private runWithPreview compilation =
        compilation
        |> asExe
        |> withLangVersionPreview
        |> withOptions [ "--nowarn:988" ]
        |> compileAndRun
        |> shouldSucceed

    let private typecheckWithPreview compilation =
        compilation |> asLibrary |> withLangVersionPreview |> typecheck

    let private featureNotAvailable =
        "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 11.0. Please use language version 'PREVIEW' or greater."

    let private rangeIsNotASingleValue =
        "A range expression cannot be used as a single value here. Use 'yield!' to splice the range, or wrap it in a list or sequence expression to use it as one value."

    [<Fact>]
    let ``Ranges mixed with values give the spliced elements`` () =
        FSharp
            """
module MixedRanges

let check name expected actual =
    if actual <> expected then failwith $"{name}: {actual}"

check "list: value, range" [ -3; 1; 2; 3; 4; 5; 6; 7; 8; 9; 10 ] [ -3; 1..10 ]
check "list: value, range, value" [ -3; 1; 2; 3; 4; 5; 6; 7; 8; 9; 10; 19 ] [ -3; 1..10; 19 ]
check "list: two ranges, value" [ 1; 2; 3; 5; 6; 7; 10 ] [ 1..3; 5..7; 10 ]
check "list: stepped range" [ 0; 2; 4; 6; 8; 10; 15 ] [ 0; 2..2..10; 15 ]
check "list: empty and one-element ranges" [ 0; 2; 4 ] [ 0; 1..0; 2..2; 3..1; 4 ]
check "list: bigint" [ 0I; 2I; 4I; 6I; 10I ] [ 0I..2I..6I; 10I ]

check "array: value, range" [| -3; 1; 2; 3; 4; 5; 6; 7; 8; 9; 10 |] [| -3; 1..10 |]
check "array: value, range, value" [| -3; 1; 2; 3; 4; 5; 6; 7; 8; 9; 10; 19 |] [| -3; 1..10; 19 |]
check "array: two ranges, value" [| 1; 2; 3; 5; 6; 7; 10 |] [| 1..3; 5..7; 10 |]
check "array: stepped range" [| 0; 2; 4; 6; 8; 10; 15 |] [| 0; 2..2..10; 15 |]
check "array: empty range" [| 1; 10 |] [| 1; 5..4; 10 |]
check "array: one-element range" [| 0; 5; 10 |] [| 0; 5..5; 10 |]

check "seq: range, value" [ 1; 2; 3; 4; 5; 6; 7; 8; 9; 10; 19 ] (List.ofSeq (seq { 1..10; 19 }))
check "seq: value, two ranges" [ -3; 1; 2; 3; 4; 5; 10; 11; 12 ] (List.ofSeq (seq { -3; 1..5; 10..12 }))
check "seq: stepped range" [ 0; 2; 4; 6; 8; 10; 15 ] (List.ofSeq (seq { 0; 2..2..10; 15 }))
check "seq: char" [| 'a'; 'b'; 'c'; 'Z' |] (Seq.toArray (seq { 'a'..'c'; 'Z' }))

type ArrayBuilder() =
    member _.Yield(x) = [| x |]
    member _.YieldFrom(xs: seq<_>) = Seq.toArray xs
    member _.Zero() = [||]
    member _.Combine(a: int array, b: unit -> int array) = Array.append a (b ())
    member _.Delay(f) = f
    member _.Run(f) = f ()

let array = ArrayBuilder()

check "builder: value, range, value" [| -3; 1; 2; 3; 4; 5; 6; 7; 8; 9; 10; 19 |] (array { -3; 1..10; 19 })
check "builder: two ranges, value" [| 1; 2; 3; 5; 6; 7; 10 |] (array { 1..3; 5..7; 10 })
check "builder: stepped range" [| 0; 2; 4; 6; 8; 10; 15 |] (array { 0; 2..2..10; 15 })

let annotatedList: int list = [ 1..5; 10 ]
let annotatedArray: int array = [| 1..5; 10 |]
let annotatedSeq: seq<int> = seq { 1..5; 10 }

check "annotated list" [ 1; 2; 3; 4; 5; 10 ] annotatedList
check "annotated array" [| 1; 2; 3; 4; 5; 10 |] annotatedArray
check "annotated seq" [ 1; 2; 3; 4; 5; 10 ] (List.ofSeq annotatedSeq)

let inline mixedRange start finish extra = [ start..finish; extra ]

check "inline: int" [ 1; 2; 3; 4; 5; 10 ] (mixedRange 1 5 10)
check "inline: float" [ 1.0; 2.0; 3.0; 4.0; 5.0; 10.0 ] (mixedRange 1.0 5.0 10.0)
check "inline: char" [ 'a'; 'b'; 'c'; 'd'; 'e'; 'z' ] (mixedRange 'a' 'e' 'z')
"""
        |> runWithPreview

    [<Theory>]
    [<InlineData("1..5", "21..25")>]
    [<InlineData("1..2..10", "21..1..30")>]
    let ``Elements on separate lines mix values and ranges`` (first: string) (last: string) =
        FSharp(
            """
module SeparateLines

let a = seq {
    0
    FIRST
    10
    11..15
    20
    LAST
}

let b = [
    0
    FIRST
    10
    11..15
    20
    LAST
]

let c = [|
    0
    FIRST
    10
    11..15
    20
    LAST
|]

let d = seq {
    0
    yield! [FIRST]
    10
    yield! [11..15]
    20
    yield! [LAST]
}

let e = [
    0
    yield! [FIRST]
    10
    yield! [11..15]
    20
    yield! [LAST]
]

let f = [|
    0
    yield! [FIRST]
    10
    yield! [11..15]
    20
    yield! [LAST]
|]

if List.ofSeq a <> List.ofSeq d then failwith $"seq: {List.ofSeq a}"
if b <> e then failwith $"list: {b}"
if c <> f then failwith $"array: {c}"
"""
                .Replace("FIRST", first)
                .Replace("LAST", last)
        )
        |> runWithPreview

    [<Fact>]
    let ``yield! takes a range and gives the elements of the same collection`` () =
        FSharp
            """
module YieldBang

let check name expected actual =
    if actual <> expected then failwith $"{name}: {actual}"

let oneToFive = [ 1; 2; 3; 4; 5 ]
let oneToTen = [ 1; 2; 3; 4; 5; 6; 7; 8; 9; 10 ]

check "list: yield! range" [ 1..10 ] [ yield! 1..10 ]
check "list: yield! list" (List.ofSeq (seq { 1..10 })) [ yield! [ 1..10 ] ]
check "seq: yield! seq" [| 1..10 |] (Array.ofSeq (seq { yield! seq { 1..10 } }))

check "list: yield! range, value, yield! list" [ 1..10; 19; yield! 20..30 ] [ yield! 1..10; 19; yield! [ 20..30 ] ]
check "list: yield! list, value, yield! range" (List.ofSeq (seq { yield! 1..10; 19; yield! 20..30 })) [ yield! [ 1..10 ]; 19; yield! 20..30 ]
check "array: yield! ranges" (Array.ofSeq (seq { yield! seq { 1..10 }; 19; yield! seq { 20..30 } })) [| yield! 1..10; 19; yield! 20..30 |]

check "seq: range as yield! seq" (List.ofSeq (seq { yield! seq { 1..10 }; 19 })) (List.ofSeq (seq { 1..10; 19 }))
check "list: range as yield! list" [ -3; yield! [ 1..10 ] ] [ -3; 1..10 ]
check "array: range as yield! array" [| -3; yield! [| 1..10 |]; 19 |] [| -3; 1..10; 19 |]

check "seq: yield and yield! seq" [ 1; 2; 3; 4; 5; 10; 15; 17; 19 ] (List.ofSeq (seq { yield 1; yield! seq { 2..5 }; yield 10; yield! seq { 15..2..20 } }))
check "seq: yield and yield! list, array" [ 1; 2; 3; 4; 5; 10; 15; 17; 19 ] (List.ofSeq (seq { yield 1; yield! [ 2..5 ]; yield 10; yield! [| 15..2..20 |] }))

let attributes =
    [|
        if true then
            yield "attr1"
            yield! [ "attr2"; "attr3" ]
        yield! [ "attr4" ]
    |]

check "explicit yields without ranges" [| "attr1"; "attr2"; "attr3"; "attr4" |] attributes

check "yield! 1..5: list" oneToFive [ yield! 1..5 ]
check "yield! 1..5: seq" oneToFive (List.ofSeq (seq { yield! 1..5 }))
check "yield! 1..5: array" oneToFive (List.ofArray [| yield! 1..5 |])

let annotatedList: int list = [ yield! 1..5 ]
let annotatedSeq: int seq = seq { yield! 1..5 }
let annotatedArray: int array = [| yield! 1..5 |]

check "annotated list" oneToFive annotatedList
check "annotated seq" oneToFive (List.ofSeq annotatedSeq)
check "annotated array" oneToFive (List.ofArray annotatedArray)

let a: int list = [ yield! 1..10 ]
let a1: int list = [ yield! [ 1..10 ] ]
let b: int seq = seq { yield! seq { 1..10 } }
let c: int list = [ if true then yield 1 else yield! 1..10 ]
let c1: int list = [ if true then yield! 1..10 ]
let d: int list = [ if true then yield 1 else yield! [ 1..10 ] ]
let d1: int list = [ if true then yield! [ 1..10 ] ]
let e: int seq = seq { if true then yield 1 else yield! seq { 1..10 } }
let e1: int seq = seq { if true then yield! seq { 1..10 } }
let f: int list = [ match true with | true -> yield 1 | false -> yield! 1..10 ]
let f1: int list = [ match true with | true -> yield 1 | false -> yield! [ 1..10 ] ]
let g: int seq = seq { match true with | true -> yield 1 | false -> yield! seq { 1..10 } }

check "a" oneToTen a
check "a1" oneToTen a1
check "b" oneToTen (List.ofSeq b)
check "c" [ 1 ] c
check "c1" oneToTen c1
check "d" [ 1 ] d
check "d1" oneToTen d1
check "e" [ 1 ] (List.ofSeq e)
check "e1" oneToTen (List.ofSeq e1)
check "f" [ 1 ] f
check "f1" [ 1 ] f1
check "g" [ 1 ] (List.ofSeq g)

let nestedIf p q r =
    [
        if p then
            if q then 3
        if p then
            if q then
                if r then 3
    ]

let nestedIfWithRange p q r =
    [
        if p then
            if q then yield! 1..10
        if p then
            if q then
                if r then yield! 1..10
    ]

let nestedMatch x (|P|_|) (|Q|_|) (|R|_|) =
    [
        match x with
        | P _ -> 3
        | _ -> ()

        match x with
        | P y ->
            match y with
            | Q z ->
                match z with
                | R -> 3
                | _ -> ()
            | _ -> ()
        | _ -> ()
    ]

let nestedMatchWithRange x (|P|_|) (|Q|_|) (|R|_|) =
    [
        match x with
        | P _ -> yield! 1..10
        | _ -> ()

        match x with
        | P y ->
            match y with
            | Q z ->
                match z with
                | R -> yield! 1..10
                | _ -> ()
            | _ -> ()
        | _ -> ()
    ]
"""
        |> runWithPreview

    [<Fact>]
    let ``A range splices in every element position`` () =
        FSharp
            """
module Positions

let check name expected actual =
    if actual <> expected then failwith $"{name}: {actual}"

type ListBuilder() =
    member _.Yield(x) = [ x ]
    member _.YieldFrom(xs: seq<_>) = List.ofSeq xs
    member _.Zero() = []
    member _.Combine(a, b) = a @ b
    member _.Delay(f: unit -> _ list) = f ()
    member _.Bind(x: _ option, f) = match x with Some v -> f v | None -> []

let list = ListBuilder()
let disposable = { new System.IDisposable with member _.Dispose() = () }

check "if" [ 2; 3 ] [ if false then 1 else 2..3 ]
check "match" [ 1; 2 ] [ match Some 2 with Some n -> 1..n | None -> 0 ]
check "for" [ 1; 2; 2; 3 ] [ for x in [ 1; 2 ] do x..x + 1 ]
check "while" [ 0; 1; 1; 2 ] [
    let i = ref 0
    while i.Value < 2 do
        i.Value..i.Value + 1
        i.Value <- i.Value + 1 ]
check "try/with" [ 1; 2 ] [ try 1..2 with _ -> () ]
check "try/finally" [ 1; 2 ] [ try 1..2 finally () ]
check "let" [ 1; 2; 3; 10 ] [ let lo = 1 in lo..3; 10 ]
check "use" [ 1; 2 ] [ use _d = disposable in 1..2 ]
check "seq" [ 0; 1; 2 ] (List.ofSeq (seq { 0; 1..2 }))
check "array" [| 0; 1; 2 |] [| 0; 1..2 |]
check "computation expression" [ 0; 1; 2; 9 ] (list { 0; 1..2; 9 })
check "let!" [ 1; 2; 3 ] (list { let! n = Some 3 in 1..n })
"""
        |> runWithPreview

    [<Fact>]
    let ``A range neither counts as an explicit yield nor disables implicit yields`` () =
        FSharp
            """
module ImplicitYields

let xs = [ 0 ]
if [ yield! xs; 2..3; 4 ] <> [ 0; 2; 3; 4 ] then failwith "yield!"
if [ yield 1; 2..3 ] <> [ 1; 2; 3 ] then failwith "yield"
"""
        |> runWithPreview

    [<Fact>]
    let ``In a flat list or array every value stays an element`` () =
        FSharp
            """
module FlatLiterals

let withRange c = [ 1; 2..3; c ]
let withoutRange c = [ 1; c ]
if withRange 5 <> [ 1; 2; 3; 5 ] || withoutRange 5 <> [ 1; 5 ] then failwith "list"
let array c = [| 0..1; c |]
if array 7 <> [| 0; 1; 7 |] then failwith "array"
"""
        |> runWithPreview

    [<Fact>]
    let ``A plain value next to a range keeps the explicit yield rule`` () =
        FSharp
            """
module M

let a = [ yield 1; 2..3; 4 ]
"""
        |> typecheckWithPreview
        |> shouldFail
        |> withDiagnostics
            [ (Warning 3221,
               Line 4,
               Col 26,
               Line 4,
               Col 27,
               "This expression returns a value of type 'int' but is implicitly discarded. Consider using 'let' to bind the result to a name, e.g. 'let result = expression'. If you intended to use the expression as a value in the sequence then use an explicit 'yield'.") ]

    [<Fact>]
    let ``A range is not yielded or returned as one value`` () =
        FSharp
            """
module M

type B() =
    member _.Yield(x) = [ x ]
    member _.Return(x) = [ x ]
    member _.Delay(f) = f ()

let b = B()
let a = [ yield 1..3 ]
let c = [ for x in [ 1 ] -> 1..3 ]
let d = b { yield 1..3 }
let e = b { return 1..3 }
let f = [ return 1..3 ]
let g = [ 1; (2..3) ]
"""
        |> typecheckWithPreview
        |> shouldFail
        |> withDiagnostics
            [ (Error 3926, Line 10, Col 17, Line 10, Col 21, rangeIsNotASingleValue)
              (Error 3926, Line 11, Col 29, Line 11, Col 33, rangeIsNotASingleValue)
              (Error 3926, Line 12, Col 19, Line 12, Col 23, rangeIsNotASingleValue)
              (Error 3926, Line 13, Col 20, Line 13, Col 24, rangeIsNotASingleValue)
              (Error 635, Line 14, Col 11, Line 14, Col 22, "In sequence expressions, results are generated using 'yield'")
              (Error 751, Line 15, Col 15, Line 15, Col 19, "Incomplete expression or invalid use of indexer syntax") ]

    [<Fact>]
    let ``A yielded or returned range gives no type error for a builder that takes an element`` () =
        FSharp
            """
module M

type WithYieldFrom() =
    member _.Yield(x: int) = [ x ]
    member _.YieldFrom(xs: seq<int>) = List.ofSeq xs
    member _.Return(x: int) = [ x ]
    member _.Delay(f: unit -> int list) = f ()

type WithoutYieldFrom() =
    member _.Yield(x: int) = [ x ]
    member _.Delay(f: unit -> int list) = f ()

let splicing = WithYieldFrom()
let single = WithoutYieldFrom()
let a = splicing { yield 1..3 }
let b = splicing { return 1..3 }
let c = single { yield 1..3 }
"""
        |> typecheckWithPreview
        |> shouldFail
        |> withDiagnostics
            [ (Error 3926, Line 16, Col 26, Line 16, Col 30, rangeIsNotASingleValue)
              (Error 3926, Line 17, Col 27, Line 17, Col 31, rangeIsNotASingleValue)
              (Error 3926, Line 18, Col 24, Line 18, Col 28, rangeIsNotASingleValue) ]

    [<Fact>]
    let ``A splice uses the range operator in scope`` () =
        FSharp
            """
module CustomRange

let (..) (a: int) (b: int) = seq { a; b }
if [ 0; 1..3 ] <> [ 0; 1; 3 ] then failwith "custom (..)"
"""
        |> runWithPreview

    [<Fact>]
    let ``A builder without YieldFrom cannot splice a range`` () =
        FSharp
            """
module M

type B() =
    member _.Yield(x) = [ x ]
    member _.Combine(a, b) = a @ b
    member _.Delay(f: unit -> _ list) = f ()

let b = B()
let r = b { 1; 2..3 }
"""
        |> typecheckWithPreview
        |> shouldFail
        |> withDiagnostics
            [ (Error 708,
               Line 10,
               Col 16,
               Line 10,
               Col 20,
               "This control construct may only be used if the computation expression builder defines a 'YieldFrom' method") ]

    [<Fact>]
    let ``A splice in tail position uses YieldFromFinal`` () =
        FSharp
            """
module Final

let mutable calls = []

type B() =
    member _.Yield(x) = [ x ]
    member _.YieldFrom(xs: seq<int>) =
        calls <- "YieldFrom" :: calls
        List.ofSeq xs
    member _.YieldFromFinal(xs: seq<int>) =
        calls <- "YieldFromFinal" :: calls
        List.ofSeq xs
    member _.Combine(a, b) = a @ b
    member _.Delay(f: unit -> _ list) = f ()

let b = B()
let r = b { 1..2; 3..4 }
if r <> [ 1; 2; 3; 4 ] || calls <> [ "YieldFromFinal"; "YieldFrom" ] then failwith $"{r} {calls}"
"""
        |> runWithPreview

    [<Fact>]
    let ``Quotations show the splice as a range call`` () =
        FSharp
            """
module Quoted

open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns

let rec callsRange (e: Expr) =
    match e with
    | Call(_, mi, _) when mi.Name = "op_Range" -> true
    | ExprShape.ShapeVar _ -> false
    | ExprShape.ShapeLambda(_, body) -> callsRange body
    | ExprShape.ShapeCombination(_, args) -> List.exists callsRange args

if not (callsRange <@ [ 0; 1..2 ] @>) then failwith "op_Range"
"""
        |> runWithPreview

    [<Fact>]
    let ``An integral splice in a list or array is a counted loop`` () =
        FSharp
            """
module M

let list n = [ 0; 1..n ]
let array n = [| 0; 1..2..n |]
"""
        |> asLibrary
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed
        |> verifyILNotPresent [ "GetEnumerator"; "AddMany" ]

    [<Fact>]
    let ``A counted splice keeps the range semantics`` () =
        FSharp
            """
module CountedSplices

let check name expected actual =
    if actual <> expected then failwith $"{name}: {actual}"

let n = 3
check "list" [ 0; 1; 2; 3; 9 ] [ 0; 1..n; 9 ]
check "array" [| 0; 3; 2; 1 |] [| 0; n .. -1 .. 1 |]
check "empty" [ 0 ] [ 0; n..1 ]
check "int64" [ 0L; 1L; 2L ] [ 0L; 1L..2L ]
check "byte" [ 0uy; 254uy; 255uy ] [ 0uy; 254uy..255uy ]
check "uint64" [ 0UL; System.UInt64.MaxValue ] [ 0UL; System.UInt64.MaxValue..System.UInt64.MaxValue ]
check "char" [ 'a'; 'b'; 'c' ] [ 'a'; 'b'..'c' ]
let threw = try ignore [ 0; 1..0..5 ]; false with :? System.ArgumentException -> true
if not threw then failwith "zero step"
"""
        |> runWithPreview

    [<Fact>]
    let ``Before the feature each new form reports FS3350 once`` () =
        FSharp
            """
module M

type B() =
    member _.Yield(x) = [ x ]
    member _.YieldFrom(xs: seq<_>) = List.ofSeq xs
    member _.Combine(a, b) = a @ b
    member _.Delay(f: unit -> _ list) = f ()

let b = B()
let a = [ 0; 1..3 ]
let c = seq { 0; 1..3 }
let d = [| yield! 1..3 |]
let e = b { 0; 1..3 }
let f = [ yield 1..3 ]
"""
        |> asLibrary
        |> withLangVersion11
        |> typecheck
        |> shouldFail
        |> withDiagnostics
            [ (Error 3350, Line 11, Col 14, Line 11, Col 18, featureNotAvailable)
              (Error 3350, Line 12, Col 18, Line 12, Col 22, featureNotAvailable)
              (Error 3350, Line 13, Col 19, Line 13, Col 23, featureNotAvailable)
              (Error 3350, Line 14, Col 16, Line 14, Col 20, featureNotAvailable)
              (Error 751, Line 15, Col 17, Line 15, Col 21, "Incomplete expression or invalid use of indexer syntax") ]
