// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace EmittedIL

open Xunit
open FSharp.Test.Compiler

/// Characterization (emitted IL, --optimize+) of when a higher-order-function call site
/// allocates a heap closure for its function argument (`newobj` of a closure). Each test
/// compiles its own `module Test`; the argument captures `env` so any allocated closure is a
/// real per-call allocation, and the HOFs return bool so the only possible caller `newobj` is
/// the closure itself. The test names state the contract being locked in.
module InlineIfLambdaClosureForms =

    let private prelude = """
module Test

let eqf (env: int) (a: string) (b: string) = a.Length = b.Length + env

// (2) inline HOF that FORWARDS the function to a non-inline callee (mirrors the current
//     List.lengthsEqAndForall2 body `List.length l1 = List.length l2 && List.forall2 p l1 l2`).
let inline forall2Forward ([<InlineIfLambda>] p: string -> string -> bool) (l1: string list) (l2: string list) =
    List.length l1 = List.length l2 && List.forall2 p l1 l2

// (3) inline HOF that APPLIES the function directly in a while-loop (mirrors the proposed fix).
//     Nested matches (not `match r1, r2 with`) avoid a per-iteration tuple allocation.
let inline forall2Direct ([<InlineIfLambda>] p: string -> string -> bool) (l1: string list) (l2: string list) =
    let mutable r1 = l1
    let mutable r2 = l2
    let mutable ok = true
    let mutable go = true
    while go do
        match r1 with
        | h1 :: t1 ->
            match r2 with
            | h2 :: t2 -> if p h1 h2 then r1 <- t1; r2 <- t2 else (ok <- false; go <- false)
            | [] -> ok <- false; go <- false
        | [] ->
            match r2 with
            | [] -> go <- false
            | _ -> ok <- false; go <- false
    ok
"""

    let private assertClosure src =
        FSharp (prelude + src) |> withOptimize |> compile |> shouldSucceed |> verifyILPresent [ "newobj" ]

    let private assertNoClosure src =
        FSharp (prelude + src) |> withOptimize |> compile |> shouldSucceed |> verifyILNotPresent [ "newobj" ]

    // ---- (1) Vanilla (non-inline) List.map: always allocates the mapping closure ----

    [<Fact>]
    let ``Vanilla List.map + partial application allocates a closure`` () =
        assertClosure "let f (env: int) (s: string) = string (s.Length + env)\nlet test (env: int) (xs: string list) = List.map (f env) xs"

    // ---- (2) Forwarding inline HOF: still allocates; eta-expansion does not help ----

    [<Fact>]
    let ``Forwarding inline HOF + partial application allocates a closure`` () =
        assertClosure "let test (env: int) (a: string list) (b: string list) = forall2Forward (eqf env) a b"

    [<Fact>]
    let ``Forwarding inline HOF + eta-expanded lambda still allocates a closure`` () =
        // Eta-expanding the call site does not help: `List.forall2` forces `p` into value position.
        assertClosure "let test (env: int) (a: string list) (b: string list) = forall2Forward (fun x y -> eqf env x y) a b"

    // ---- (3) Direct-apply inline HOF: allocates nothing, regardless of call-site form ----

    [<Fact>]
    let ``Direct-apply inline HOF + partial application allocates no closure`` () =
        // The partial application `(eqf env)` is beta-reduced into a direct call; no closure.
        assertNoClosure "let test (env: int) (a: string list) (b: string list) = forall2Direct (eqf env) a b"

    [<Fact>]
    let ``Direct-apply inline HOF + eta-expanded lambda allocates no closure`` () =
        assertNoClosure "let test (env: int) (a: string list) (b: string list) = forall2Direct (fun x y -> eqf env x y) a b"
