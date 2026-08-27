// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace EmittedIL

open Xunit
open FSharp.Test.Compiler

/// Characterization (emitted IL, --optimize+) of when a higher-order-function call site allocates a
/// heap closure for its function argument (a `newobj` of a closure). Each test compiles the shared
/// `prelude` plus one `test` function; the argument captures `env`, so any closure it needs is a real
/// per-call allocation, and the probe HOFs return `bool` (no list building) so the only `newobj` a
/// caller could show is the function closure itself. The two sub-modules split the cases by outcome.
module InlineIfLambdaClosureForms =

    let private prelude =
        """
module Test

let eqf (env: int) (a: string) (b: string) = a.Length = b.Length + env

// Forwards the function to a non-inline callee (the OLD List.lengthsEqAndForall2 shape).
let inline forall2Forward ([<InlineIfLambda>] p: string -> string -> bool) l1 l2 =
    List.length l1 = List.length l2 && List.forall2 p l1 l2

// Applies the function directly in a loop (the NEW shape).
let inline forall2Direct ([<InlineIfLambda>] p: string -> string -> bool) l1 l2 =
    let mutable r1 = l1
    let mutable r2 = l2
    while not (List.isEmpty r1) && not (List.isEmpty r2) && p (List.head r1) (List.head r2) do
        r1 <- List.tail r1
        r2 <- List.tail r2
    List.isEmpty r1 && List.isEmpty r2

// Single-argument inline HOF, used to probe `<|`.
let inline applyDirect ([<InlineIfLambda>] f: unit -> int) = f ()
"""

    let private allocatesClosure body =
        FSharp(prelude + body) |> withOptimize |> compile |> shouldSucceed |> verifyILPresent [ "newobj" ]

    let private allocatesNoClosure body =
        FSharp(prelude + body) |> withOptimize |> compile |> shouldSucceed |> verifyILNotPresent [ "newobj" ]

    module DoesNotAllocate =

        // An inline + InlineIfLambda HOF that applies the function directly allocates nothing - for a
        // lambda literal, a forward pipe, or a partial application of a top-level function alike; the
        // optimizer beta-reduces it into a direct call, so no call-site eta-expansion is needed.

        [<Fact>]
        let ``direct-apply inline HOF, lambda literal`` () =
            allocatesNoClosure
                """
let test (env: int) (a: string list) (b: string list) =
    forall2Direct (fun x y -> eqf env x y) a b
"""

        // Partial application of a TOP-LEVEL function: the optimizer knows its arity and forms the
        // saturated call, so no closure. (Contrast with the local-function case in AllocatesClosure.)
        [<Fact>]
        let ``direct-apply inline HOF, partial application of a top-level function`` () =
            allocatesNoClosure
                """
let test (env: int) (a: string list) (b: string list) =
    forall2Direct (eqf env) a b
"""

        [<Fact>]
        let ``direct-apply inline HOF, forward-piped`` () =
            allocatesNoClosure
                """
let test (env: int) (a: string list) (b: string list) =
    (a, b) ||> forall2Direct (eqf env)
"""

        // `<|` does not defeat InlineIfLambda for a module-level `let inline` whose param does not escape:
        // the optimizer recovers the saturated call, so both the direct and back-piped forms are clean.

        [<Fact>]
        let ``direct-apply inline HOF, direct call`` () =
            allocatesNoClosure
                """
let test (env: int) =
    applyDirect (fun () -> eqf env "a" "b" |> System.Convert.ToInt32)
"""

        [<Fact>]
        let ``direct-apply inline HOF, back-piped with <|`` () =
            allocatesNoClosure
                """
let test (env: int) =
    applyDirect <| (fun () -> eqf env "a" "b" |> System.Convert.ToInt32)
"""

        // InlineIfLambda chains: an inline HOF that delegates to another inline + InlineIfLambda
        // combinator is still closure-free. This is what lets List.mapq / lengthsEqAndForall2 keep their
        // elegant bodies and call the ListInline combinators without allocating.
        [<Fact>]
        let ``inline HOF delegating to another inline combinator`` () =
            allocatesNoClosure
                """
let inline forall2Chained ([<InlineIfLambda>] p: string -> string -> bool) l1 l2 = forall2Direct p l1 l2
let test (env: int) (a: string list) (b: string list) =
    forall2Chained (eqf env) a b
"""

        // A direct-apply instance `member inline` whose lambda does NOT escape keeps InlineIfLambda through
        // `<|`. This does not generalise: once the lambda escapes (e.g. captured by a slow-path closure, as
        // in StackGuard.Guard), `<|` defeats InlineIfLambda and materialises it UNCONDITIONALLY every call,
        // whereas a method-call `Guard(fun ..)` keeps InlineIfLambda firing so the closure stays in the cold
        // escape branch. That is a per-call placement/byte difference a newobj-presence check cannot see.
        [<Fact>]
        let ``direct-apply inline instance member, back-piped with <|`` () =
            allocatesNoClosure
                """
type H() =
    member inline _.M ([<InlineIfLambda>] f: unit -> int) = f ()
let test (h: H) (env: int) = h.M <| (fun () -> env)
"""

    module AllocatesClosure =

        // Vanilla List.map is not inline, so the mapping function is always materialised as a value -
        // a closure is allocated whatever the syntactic form.

        [<Fact>]
        let ``vanilla List.map, lambda literal`` () =
            allocatesClosure
                """
let test (env: int) (xs: string list) =
    List.map (fun (s: string) -> string (s.Length + env)) xs
"""

        [<Fact>]
        let ``vanilla List.map, partial application`` () =
            allocatesClosure
                """
let g (env: int) (s: string) = string (s.Length + env)
let test (env: int) (xs: string list) =
    List.map (g env) xs
"""

        // An inline + InlineIfLambda HOF that FORWARDS the function to a non-inline callee still allocates,
        // and eta-expanding the call site does not change that.

        [<Fact>]
        let ``forwarding inline HOF, partial application`` () =
            allocatesClosure
                """
let test (env: int) (a: string list) (b: string list) =
    forall2Forward (eqf env) a b
"""

        [<Fact>]
        let ``forwarding inline HOF, eta-expanded lambda`` () =
            allocatesClosure
                """
let test (env: int) (a: string list) (b: string list) =
    forall2Forward (fun x y -> eqf env x y) a b
"""

        // Partial application of a LOCAL function that closes over a local: unlike a top-level function
        // (see DoesNotAllocate), the local is itself a closure value the optimizer cannot reduce, so it is
        // materialised even though the HOF applies it directly.
        [<Fact>]
        let ``direct-apply inline HOF, partial application of a local closure`` () =
            allocatesClosure
                """
let test (env: int) (a: string list) (b: string list) =
    let local (cap: int) (x: string) (y: string) = x.Length = y.Length + cap + env
    forall2Direct (local 5) a b
"""
