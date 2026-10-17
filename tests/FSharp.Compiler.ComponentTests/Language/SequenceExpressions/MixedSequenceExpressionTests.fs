// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module Language.MixedSequenceExpressionTests

open Xunit
open FSharp.Test.Compiler

let private diagnostic kind code message line col endCol =
    (kind code, Line line, Col col, Line line, Col endCol, message)

let private rangeAsValue =
    diagnostic Error 3925
        "A range expression cannot be used as a single value here. Use 'yield!' to splice the range, or wrap it in a list or sequence expression to use it as one value."

let private featureUnavailable =
    diagnostic Error 3350
        "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 11.0. Please use language version 'PREVIEW' or greater."

let private indexerSyntax =
    diagnostic Error 751 "Incomplete expression or invalid use of indexer syntax"

[<Fact>]
let ``Ranges mixed with values splice their elements`` () =
    FSharp
        """
module Runtime

let check name expected actual =
    if actual <> expected then failwith $"{name}: {actual}"

type ListBuilder() =
    member _.Yield(x) = [ x ]
    member _.YieldFrom(xs: seq<_>) = List.ofSeq xs
    member _.Combine(a, b) = a @ b
    member _.Delay(f: unit -> _ list) = f ()
    member _.Bind(x: _ option, f) = match x with Some v -> f v | None -> []

let list = ListBuilder()

check "value, range, value" [ -3; 1; 2; 3; 19 ] [ -3; 1..3; 19 ]
check "two ranges" [ 1; 2; 5; 6; 10 ] [ 1..2; 5..6; 10 ]
check "stepped" [ 0; 2; 4; 15 ] [ 0; 2..2..4; 15 ]
check "negative step" [ 0; 3; 2; 1 ] [ 0; 3 .. -1 .. 1 ]
let down start = [ 0; start .. -1 .. 1 ]
check "negative step from a runtime start" [ 0; 3; 2; 1 ] (down 3)
check "empty and singleton" [ 0; 2; 4 ] [ 0; 1..0; 2..2; 3..1; 4 ]
check "separate lines" [ 0; 1; 2; 10; 11; 12 ] [
    0
    1..2
    10
    11..12 ]
check "array" [ 0; 1; 2; 9 ] (List.ofArray [| 0; 1..2; 9 |])
check "seq" [ 0; 1; 2; 9 ] (List.ofSeq (seq { 0; 1..2; 9 }))
check "seq with a stepped tail" [ 0; 2; 4 ] (List.ofSeq (seq { 0; 2..2..4 }))
check "builder" [ 0; 1; 2; 9 ] (list { 0; 1..2; 9 })

check "char" [ 'a'; 'b'; 'Z' ] [ 'a'..'b'; 'Z' ]
check "bigint" [ 0I; 2I; 10I ] [ 0I..2I..2I; 10I ]
check "int64" [ 0L; 1L; 2L ] [ 0L; 1L..2L ]
check "byte at the limit" [ 0uy; 254uy; 255uy ] [ 0uy; 254uy..255uy ]
check "uint64 at the limit" [ 0UL; System.UInt64.MaxValue ] [ 0UL; System.UInt64.MaxValue..System.UInt64.MaxValue ]
check "singleton nativeint" [ 0n; 2n ] [ 0n; 2n..2n ]
check "singleton unativeint" [ 0un; 2un ] [ 0un; 2un..2un ]
check "singleton unativeint alone" [ 2un ] [ 2un..2un ]
check "singleton unativeint yield!" [ 0un; 2un ] [ yield 0un; yield! (..) 2un 2un ]
let mutable visited = 0
for _ in 2un..2un do visited <- visited + 1
check "singleton unativeint for loop" 1 visited
check "zero step" true (try ignore [ 0; 1..0..5 ]; false with :? System.ArgumentException -> true)
check "nativeint with the smallest step" [ 7n ] [ 7n; -1n .. -9223372036854775808n .. 0n ]
let mutable first = 1
let bump () = first <- 100; 3
check "operands are read in order" [ 0; 1; 2; 3 ] [ 0; first..bump () ]

let inline mixed a b c = [ a..b; c ]
let flat c = [ 1; 2..3; c ]
let flatArray c = [| 0..1; c |]
check "annotated" [ 1; 2; 5 ] ([ 1..2; 5 ]: int list)
check "inline" [ 1.0; 2.0; 5.0 ] (mixed 1.0 2.0 5.0)
check "inline int" [ 1; 2; 5 ] (mixed 1 2 5)
check "flat list keeps the value" [ 1; 2; 3; 5 ] (flat 5)
check "flat array keeps the value" [| 0; 1; 7 |] (flatArray 7)

let counter = ref 0
check "if" [ 2; 3 ] [ if false then 1 else 2..3 ]
check "match" [ 1; 2 ] [ match Some 2 with Some n -> 1..n | None -> 0 ]
check "for" [ 1; 2; 2; 3 ] [ for x in [ 1; 2 ] do x..x + 1 ]
check "while" [ 0; 1; 1; 2 ] [ while counter.Value < 2 do counter.Value..counter.Value + 1; counter.Value <- counter.Value + 1 ]
check "try/with" [ 1; 2 ] [ try 1..2 with _ -> () ]
check "try/finally" [ 1; 2 ] [ try 1..2 finally () ]
check "let" [ 1; 2; 3; 10 ] [ let lo = 1 in lo..3; 10 ]
check "use" [ 1; 2 ] [ use _d = new System.IO.StringReader("") in 1..2 ]
check "let!" [ 1; 2; 3 ] (list { let! n = Some 3 in 1..n })

check "yield! range" [ 1; 2; 3 ] [ yield! 1..3 ]
check "yield! range in seq" [ 1; 2; 3 ] (List.ofSeq (seq { yield! 1..3 }))
check "yield! range in array" [ 1; 2; 3 ] (List.ofArray [| yield! 1..3 |])
check "yield! range, value, yield! list" [ 1; 2; 3; 9; 20; 21 ] [ yield! 1..3; 9; yield! [ 20..21 ] ]
check "yield! in if" [ 1; 2; 3 ] [ if false then yield 1 else yield! 1..3 ]
check "yield! in match" [ 1; 2; 3 ] [ match false with true -> yield 1 | false -> yield! 1..3 ]
check "after yield!" [ 0; 2; 3; 4 ] [ yield! [ 0 ]; 2..3; 4 ]
check "after yield" [ 1; 2; 3 ] [ yield 1; 2..3 ]

check "quotation" (string <@ [ yield 0; yield! (..) 1 2 ] @>) (string <@ [ 0; 1..2 ] @>)

module CustomRange =
    let (..) (a: int) (b: int) = seq { a; b }
    check "range operator in scope" [ 0; 1; 3 ] [ 0; 1..3 ]

module TailPosition =
    let calls = ResizeArray()

    type Builder() =
        member _.YieldFrom(xs: seq<int>) = calls.Add "YieldFrom"; List.ofSeq xs
        member _.YieldFromFinal(xs: seq<int>) = calls.Add "YieldFromFinal"; List.ofSeq xs
        member _.Combine(a, b) = a @ b
        member _.Delay(f: unit -> _ list) = f ()

    let builder = Builder()
    check "tail position elements" [ 1; 2; 3; 4 ] (builder { 1..2; 3..4 })
    check "tail position calls" [ "YieldFrom"; "YieldFromFinal" ] (List.ofSeq calls)
"""
    |> asExe
    |> withLangVersionPreview
    |> compileAndRun
    |> shouldSucceed

[<Fact>]
let ``A range is not a single value and needs YieldFrom to splice`` () =
    FSharp
        """
module Diagnostics

type Builder() =
    member _.Yield(x: int) = [ x ]
    member _.YieldFrom(xs: seq<int>) = List.ofSeq xs
    member _.Return(x: int) = [ x ]
    member _.Bind(x: int, f: int -> int list) = f x
    member _.BindReturn(x: int, f: int -> int) = [ f x ]

type WithoutYieldFrom() =
    member _.Yield(x: int) = [ x ]

let builder = Builder()
let withoutYieldFrom = WithoutYieldFrom()
let a = [ yield 1..3 ]
let b = [ for x in [ 1 ] -> 1..3 ]
let c = [ return 1..3 ]
let d = [ 1; (2..3) ]
let e = builder { yield 1..3 }
let f = builder { return 1..3 }
let g = builder { let! x = 1 in return x..3 }
let h = withoutYieldFrom { yield 1..3 }
let i = withoutYieldFrom { 1..3 }
let j = [ yield 1; 2..3; 4 ]
let k = [ 1..2; 3 then 4 ]
let l = query { return 1..3 }
"""
    |> asLibrary
    |> withLangVersionPreview
    |> typecheck
    |> shouldFail
    |> withDiagnostics
        [ rangeAsValue 16 17 21
          rangeAsValue 17 29 33
          (Error 635, Line 18, Col 11, Line 18, Col 22, "In sequence expressions, results are generated using 'yield'")
          indexerSyntax 19 15 19
          rangeAsValue 20 25 29
          rangeAsValue 21 26 30
          rangeAsValue 22 40 44
          rangeAsValue 23 34 38
          (Error 708, Line 24, Col 28, Line 24, Col 32, "This control construct may only be used if the computation expression builder defines a 'YieldFrom' method")
          (Warning 3221, Line 25, Col 26, Line 25, Col 27, "This expression returns a value of type 'int' but is implicitly discarded. Consider using 'let' to bind the result to a name, e.g. 'let result = expression'. If you intended to use the expression as a value in the sequence then use an explicit 'yield'.")
          (Error 743, Line 26, Col 17, Line 26, Col 25, "The expression form 'expr then expr' may only be used as part of an explicit object constructor")
          (Warning 20, Line 26, Col 24, Line 26, Col 25, "The result of this expression has type 'int' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'.")
          (Error 3144, Line 27, Col 17, Line 27, Col 23, "'return' and 'return!' may not be used in queries") ]

[<Fact>]
let ``Before the feature each new form reports FS3350`` () =
    FSharp
        """
module Gated

type Builder() =
    member _.Yield(x) = [ x ]
    member _.YieldFrom(xs: seq<_>) = List.ofSeq xs
    member _.Combine(a, b) = a @ b
    member _.Delay(f: unit -> _ list) = f ()

let builder = Builder()
let a = [ 0; 1..3 ]
let b = seq { 0; 1..3 }
let c = [| yield! 1..3 |]
let d = builder { 0; 1..3 }
let e = [ yield 1..3 ]
"""
    |> asLibrary
    |> withLangVersion11
    |> typecheck
    |> shouldFail
    |> withDiagnostics
        [ featureUnavailable 11 14 18
          featureUnavailable 12 18 22
          featureUnavailable 13 19 23
          featureUnavailable 14 22 26
          indexerSyntax 15 17 21 ]

[<Fact>]
let ``An integral splice in a list or array is a counted loop`` () =
    FSharp
        """
module Loops

let list n = [ 0; 1..n ]
let array n = [| 0; 1..2..n |]
"""
    |> asLibrary
    |> withLangVersionPreview
    |> compile
    |> shouldSucceed
    |> verifyILNotPresent [ "GetEnumerator"; "AddMany"; "RangeInt32" ]
