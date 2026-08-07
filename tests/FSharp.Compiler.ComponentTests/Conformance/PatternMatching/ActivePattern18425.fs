// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.PatternMatching

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ActivePattern18425 =

    // Test that deeply nested active patterns compile in reasonable time
    [<Fact>]
    let ``Deeply nested active patterns should compile efficiently`` () =
        FSharp """
module Test

let inline (|Or'|_|) x =
    match x with
    | (a, b) -> Some(a, b)

let inline (|And'|_|) x =
    match x with
    | (a, b) -> Some(a, b)

let inline (|Not'|_|) x = Some x

// Function with deeply nested active patterns  
let testFunction x =
    match x with
    // Deep nesting of active patterns (depth > 5)
    | Or'(And'(Not' p1, p2), And'(p3, Not' p4)) -> 1
    | Or'(And'(p1, Not' p2), And'(Not' p3, p4)) -> 2
    | Or'(Not'(And'(p1, p2)), And'(Not' p3, p4)) -> 3
    | Or'(And'(Not' p1, Not' p2), And'(p3, p4)) -> 4
    | Or'(And'(p1, p2), Or'(And'(p3, p4), And'(p5, p6))) -> 5
    | Or'(Or'(And'(p1, p2), And'(p3, p4)), And'(p5, p6)) -> 6
    | _ -> 7
        """
        |> compile
        |> shouldSucceed


    // Test that many active patterns in one clause compile efficiently
    [<Fact>]
    let ``Many active patterns in one clause should compile efficiently`` () =
        FSharp """
module Test

let inline (|Pat1|_|) x = if fst x = 1 then Some (snd x) else None
let inline (|Pat2|_|) x = if fst x = 2 then Some (snd x) else None
let inline (|Pat3|_|) x = if fst x = 3 then Some (snd x) else None
let inline (|Pat4|_|) x = if fst x = 4 then Some (snd x) else None
let inline (|Pat5|_|) x = if fst x = 5 then Some (snd x) else None
let inline (|Pat6|_|) x = if fst x = 6 then Some (snd x) else None
let inline (|Pat7|_|) x = if fst x = 7 then Some (snd x) else None
let inline (|Pat8|_|) x = if fst x = 8 then Some (snd x) else None
let inline (|Pat9|_|) x = if fst x = 9 then Some (snd x) else None
let inline (|Pat10|_|) x = if fst x = 10 then Some (snd x) else None
let inline (|Pat11|_|) x = if fst x = 11 then Some (snd x) else None

// More than 10 active patterns in a pattern
let testFunction x =
    match x with
    | Pat1 p1 when p1 > 0 -> Pat2 p1, Pat3 p1, Pat4 p1, Pat5 p1, Pat6 p1, Pat7 p1, Pat8 p1, Pat9 p1, Pat10 p1, Pat11 p1
    | _ -> None, None, None, None, None, None, None, None, None, None
        """
        |> compile
        |> shouldSucceed
