// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Language

open Xunit
open FSharp.Test
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

    [<Theory;
      Directory(__SOURCE_DIRECTORY__,
                Includes =
                    [| "SequenceExpressions02a.fs"
                       "SequenceExpressions02b.fs"
                       "SequenceExpressions02c.fs"
                       "SequenceExpressions02d.fs"
                       "SequenceExpressions03.fs"
                       "SequenceExpressions04.fs"
                       "SequenceExpressions05.fs"
                       "SequenceExpressions06.fs"
                       "SequenceExpressions07.fs"
                       "SequenceExpressions08.fs"
                       "SequenceExpressions10.fs"
                       "SequenceExpressions11.fs"
                       "SequenceExpressions12.fs"
                       "SequenceExpressions13.fs"
                       "SequenceExpressions14.fs"
                       "SequenceExpressions15.fs"
                       "SequenceExpressions16.fs"
                       "SequenceExpressions17.fs"
                       "SequenceExpressions18.fs"
                       "SequenceExpressions19.fs"
                       "SequenceExpressions20.fs" |])>]
    let ``Ranges mixed with values give the spliced elements`` compilation = runWithPreview compilation

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
            [ (Error 3925, Line 10, Col 17, Line 10, Col 21, rangeIsNotASingleValue)
              (Error 3925, Line 11, Col 29, Line 11, Col 33, rangeIsNotASingleValue)
              (Error 3925, Line 12, Col 19, Line 12, Col 23, rangeIsNotASingleValue)
              (Error 3925, Line 13, Col 20, Line 13, Col 24, rangeIsNotASingleValue)
              (Error 635, Line 14, Col 11, Line 14, Col 22, "In sequence expressions, results are generated using 'yield'")
              (Error 751, Line 15, Col 15, Line 15, Col 19, "Incomplete expression or invalid use of indexer syntax") ]

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
