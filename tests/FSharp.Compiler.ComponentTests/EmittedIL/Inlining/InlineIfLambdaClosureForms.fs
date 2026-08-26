// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace EmittedIL

open Xunit
open FSharp.Test.Compiler

/// Characterization of when a higher-order-function call site allocates a heap closure for
/// its function argument, established by reading the emitted IL (`newobj` of a closure).
///
/// Each test compiles its own `module Test` with --optimize+ and asserts whether a closure
/// `newobj` appears. The function argument captures a runtime value (`env`) so that any
/// closure is a real per-call allocation, and the HOFs here return a bool (no list building)
/// so that the ONLY possible `newobj` in the caller is the function closure itself.
///
/// The contract these tests lock in (verified against emitted IL, F# 11, --optimize+):
///
///  1. Vanilla `List.map` is NOT inline, so its mapping function must be materialised as a
///     value => a closure is allocated in EVERY syntactic form (lambda literal or partial
///     application, direct or piped). Eta-expanding a `List.map` call site buys nothing.
///
///  2. An `inline` + `[<InlineIfLambda>]` HOF whose body FORWARDS the function to another
///     non-inline callee (e.g. `List.forall2 p` / `List.map f`) STILL allocates the closure,
///     because the callee forces the function into value position. Eta-expanding the call
///     site does NOT help here either.
///
///  3. An `inline` + `[<InlineIfLambda>]` HOF whose body APPLIES the function directly (a
///     while-loop, no forwarding, no nested closure) allocates NOTHING - and this holds
///     whether the call site passes a lambda literal OR a partial application. The optimizer
///     beta-reduces the partial application into a direct call. Therefore the closure win
///     comes from the HOF body applying the function directly, NOT from the call-site form.
///
/// Note on `<|`: a separately-observed "`prim <| (fun ..)` allocates" effect is context
/// dependent (it needs a HOF the optimizer cannot fully reduce, e.g. one touching private
/// state) and does NOT reproduce in minimal code - the optimizer recovers the saturated
/// call - so it is deliberately not asserted here.
module InlineIfLambdaClosureForms =

    // Shared definitions. `eqf env` is the partial application used by the "partial
    // application" call sites; capturing `env` makes any allocated closure per-call.
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

    // ---- (1) Vanilla List.map: allocates the mapping closure in every form ----

    [<Fact>]
    let ``Vanilla List.map + lambda literal allocates a closure`` () =
        assertClosure "let test (env: int) (xs: string list) = List.map (fun (s: string) -> string (s.Length + env)) xs"

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
