// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Optimizations

open Xunit
open FSharp.Test.Compiler

module InlineIfLambdaEtaFloat =

    // Each snippet runs under both --optimize+ (transform on) and --optimize- (off, as a reference oracle),
    // in-process, so a failure signals with failwith, not exit.
    let private run (optimize: bool) (source: string) =
        Fsx source |> withOptimization optimize |> compileExeAndRun |> shouldSucceed |> ignore

    [<Theory; InlineData(true); InlineData(false)>]
    let ``Captured argument is read once, before the parameter body is copied`` (optimize: bool) =
        run optimize """
let mutable state = 10
let f4 (a:int) (b:int) (c:int) (x:int) = a + b + c + x
let inline twiceMut ([<InlineIfLambda>] f: int -> int) (x: int) =
    let r1 = f x
    state <- 99
    let r2 = f x
    r1 + r2
if twiceMut (f4 1 state 3) 5 <> 38 then failwith "capture was not read exactly once at 10"
"""

    [<Theory; InlineData(true); InlineData(false)>]
    let ``Captured effect runs exactly once for a multi-use parameter`` (optimize: bool) =
        run optimize """
let mutable reads = 0
type Box() = member _.Value = reads <- reads + 1; 10
let f4 (a:int) (b:int) (c:int) (x:int) = a + b + c + x
let inline twice ([<InlineIfLambda>] f: int -> int) (x: int) = f x + f x
let r = twice (f4 1 (Box().Value) 3) 5
if r <> 38 then failwithf "Expected 38 but got %d" r
if reads <> 1 then failwithf "Captured getter evaluated %d times, expected 1" reads
"""

    // Floating must not sink the capture into the used branch.
    [<Theory; InlineData(true); InlineData(false)>]
    let ``Captured effect runs even when the parameter is unused`` (optimize: bool) =
        run optimize """
let mutable eff = 0
type Box() = member _.Value = eff <- eff + 1; 10
let f4 (a:int) (b:int) (c:int) (x:int) = a + b + c + x
None   |> Option.map (f4 1 (Box().Value) 3) |> ignore
Some 5 |> Option.map (f4 1 (Box().Value) 3) |> ignore
if eff <> 2 then failwithf "Expected 2 captures but got %d" eff
"""

    [<Theory; InlineData(true); InlineData(false)>]
    let ``Captured arguments preserve left-to-right evaluation order`` (optimize: bool) =
        run optimize """
let log = System.Collections.Generic.List<string>()
let tap (name: string) (v: int) = log.Add name; v
let f4 (a:int) (b:int) (c:int) (x:int) = a + b + c + x
Some 5 |> Option.map (f4 (tap "a" 1) (tap "b" 2) (tap "c" 3)) |> ignore
if String.concat "," (List.ofSeq log) <> "a,b,c" then failwithf "Wrong order: %A" (List.ofSeq log)
"""

    // The transform re-applies to the second parameter's binding, nested under the first.
    [<Theory; InlineData(true); InlineData(false)>]
    let ``Two InlineIfLambda parameters each capture once, left-to-right`` (optimize: bool) =
        run optimize """
let log = System.Collections.Generic.List<string>()
let eff (n: string) (v: int) = log.Add n; v
let f4 (a:int) (b:int) (c:int) (x:int) = a + b + c + x
let inline combine ([<InlineIfLambda>] f: int -> int) ([<InlineIfLambda>] g: int -> int) (x: int) =
    f x + g x + f x + g x
let r = combine (f4 1 (eff "f" 10) 3) (f4 2 (eff "g" 20) 4) 5
if r <> 100 then failwithf "Expected 100 but got %d" r
if String.concat "," (List.ofSeq log) <> "f,g" then failwithf "Wrong capture order/count: %A" (List.ofSeq log)
"""

    // The "body" marker stays out of the log, proving the throw happens during capture, before the body.
    [<Theory; InlineData(true); InlineData(false)>]
    let ``A throwing capture is raised eagerly, in order, before the body`` (optimize: bool) =
        run optimize """
let log = System.Collections.Generic.List<string>()
let tapOk (name: string) (v: int) = log.Add name; v
let tapThrow (name: string) : int = log.Add name; failwith ("throw-" + name)
let f4 (a:int) (b:int) (c:int) (x:int) = a + b + c + x
let inline twice ([<InlineIfLambda>] f: int -> int) (x: int) = log.Add "body"; f x + f x
let mutable msg = "NOTHROW"
(try twice (f4 (tapOk "a" 1) (tapThrow "b") (tapOk "c" 3)) 5 |> ignore
 with e -> msg <- e.Message)
if String.concat "," (List.ofSeq log) <> "a,b" then failwithf "Wrong pre-throw order: %A" (List.ofSeq log)
if msg <> "throw-b" then failwithf "Wrong exception surfaced: %s" msg
"""

    [<Theory; InlineData(true); InlineData(false)>]
    let ``Capture is evaluated once when the parameter escapes in a returned closure`` (optimize: bool) =
        run optimize """
let mutable reads = 0
type Box() = member _.Value = reads <- reads + 1; 10
let f4 (a:int) (b:int) (c:int) (x:int) = a + b + c + x
let inline makeAdder ([<InlineIfLambda>] f: int -> int) = fun y -> f y + f y
let g = makeAdder (f4 1 (Box().Value) 3)
let r1 = g 5
let r2 = g 6
if r1 <> 38 || r2 <> 40 then failwithf "Expected 38/40 but got %d/%d" r1 r2
if reads <> 1 then failwithf "Captured getter evaluated %d times, expected 1" reads
"""

    [<Theory; InlineData(true); InlineData(false)>]
    let ``A compound-expression capture is evaluated once`` (optimize: bool) =
        run optimize """
let mutable reads = 0
let bump () = reads <- reads + 1; reads
let f4 (a:int) (b:int) (c:int) (x:int) = a + b + c + x
let inline twice ([<InlineIfLambda>] f: int -> int) (x: int) = f x + f x
let r = twice (f4 1 (let n = bump () in n * 10 + 5) 3) 5
if r <> 48 then failwithf "Expected 48 but got %d" r
if reads <> 1 then failwithf "Compound capture evaluated %d times, expected 1" reads
"""

    // The transform does not fire here: the inline SRTP body collapses first.
    [<Theory; InlineData(true); InlineData(false)>]
    let ``SRTP partial application stays correct`` (optimize: bool) =
        run optimize """
let mutable reads = 0
type Box() = member _.Value = reads <- reads + 1; 10
let inline addThree (a: ^T) (b: ^T) (c: ^T) (x: ^T) = a + b + c + x
let inline mapTwice ([<InlineIfLambda>] f: ^U -> ^U) (x: ^U) = f (f x)
let r = mapTwice (addThree 1 (Box()).Value 3) 5
if r <> 33 then failwithf "Expected 33 but got %d" r
if reads <> 1 then failwithf "Captured getter evaluated %d times, expected 1" reads
"""
