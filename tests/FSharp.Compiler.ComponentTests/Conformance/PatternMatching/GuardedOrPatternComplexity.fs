// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.PatternMatching

open Xunit
open FSharp.Test.Compiler

module GuardedOrPatternComplexity =

    // https://github.com/dotnet/fsharp/issues/18425
    // A single match clause of N disjuncts that SHARE one `when` guard, whose disjuncts contain
    // partial active patterns, used to compile in exponential (2^N) time and space: each disjunct
    // contributes both a fail edge and a guard-false edge to the same residual decision state, which
    // the pattern-match compiler re-investigated along all 2^N paths, blowing up compile time, DLL
    // size and finally the stack. Join-point memoization compiles each distinct residual state once,
    // making it linear while preserving exact runtime behaviour.
    let private guardedOrSource n =
        let disjuncts =
            [ for k in 1..n -> sprintf "    | (A p, E %d _)" k ]
            |> String.concat "\n"

        let template = """module Test
let (|A|_|) (x: int) = if x % 2 = 0 then Some(x / 2) else None
let (|E|_|) (n: int) (x: int) = if x = n then Some x else None
let g (p: int) = p > 1000
let f (a: int) (b: int) =
    match a, b with
__DISJUNCTS__
        when g p -> p
    | _ -> -1
[<EntryPoint>]
let main _ =
    // 8 -> A matches (p = 4), b = 3 -> disjunct (A p, E 3 _) matches, guard g 4 is false -> -1
    let r1 = f 8 3
    // 4000 -> A matches (p = 2000), b = 1 -> disjunct (A p, E 1 _) matches, guard g 2000 is true -> 2000
    let r2 = f 4000 1
    printfn "r1=%d r2=%d" r1 r2
    0
"""

        template.Replace("__DISJUNCTS__", disjuncts)

    // A 24-disjunct guarded shared-or match: on the pre-fix compiler this exhausts the stack during
    // analysis (never produces an assembly). It must now compile, run and yield the exact results a
    // linear left-to-right evaluation of the clause would give.
    [<Fact>]
    let ``Issue 18425 - guarded shared-or partial active pattern match compiles and runs`` () =
        guardedOrSource 24
        |> FSharp
        |> asExe
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContains "r1=-1 r2=2000"

    // Join-point memoization must never FUSE two residual states that bind the same clause variable to a
    // DIFFERENT projection of the match input. Here `x` is bound at a different tuple position in each of the
    // eight disjuncts of one guarded clause: the states share an (empty) active set and captures but differ
    // in which element feeds the guard and the result, so fusing them would bake one projection into all of
    // them and miscompile. Eight disjuncts cross the promotion threshold, so the memo path is exercised; each
    // `f` call must still return the element that made the guard true.
    [<Fact>]
    let ``Issue 18425 - shared guard binding a variable at different positions is not over-fused`` () =
        """module Test
let (|Z|_|) (v: int) = if v = 0 then Some() else None
let (|Pos|_|) (v: int) = if v > 100 then Some v else None
let f (t: int*int*int*int*int*int*int*int) =
    match t with
    | (Pos x, Z, Z, Z, Z, Z, Z, Z)
    | (Z, Pos x, Z, Z, Z, Z, Z, Z)
    | (Z, Z, Pos x, Z, Z, Z, Z, Z)
    | (Z, Z, Z, Pos x, Z, Z, Z, Z)
    | (Z, Z, Z, Z, Pos x, Z, Z, Z)
    | (Z, Z, Z, Z, Z, Pos x, Z, Z)
    | (Z, Z, Z, Z, Z, Z, Pos x, Z)
    | (Z, Z, Z, Z, Z, Z, Z, Pos x) when x > 100 -> x
    | _ -> -1
[<EntryPoint>]
let main _ =
    printfn "%d %d %d %d" (f (150,0,0,0,0,0,0,0)) (f (0,0,0,160,0,0,0,0)) (f (0,0,0,0,0,0,0,170)) (f (1,2,3,4,5,6,7,8))
    0
"""
        |> FSharp
        |> asExe
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContains "150 160 170 -1"

    // A join thunk is an FSharpFunc over the captured locals returning the match result, but the CLR forbids a
    // byref-like type (here byref<int>) as a generic type argument, so a promoted state returning one would emit
    // FSharpFunc<_, int&> and fail with FS0412. This guarded shared-or match returns a byref and has enough
    // disjuncts to cross the promotion threshold, so memoization must recognise the byref result and leave the
    // state inline exactly as the pristine compiler does. It must compile and mutate through the returned byref.
    [<Fact>]
    let ``Issue 18425 - guarded shared-or returning a byref stays inline and compiles`` () =
        """module Test
let (|E|_|) (n: int) (x: int) = if x = n then Some x else None
let f (arr: int[]) (b: int) : byref<int> =
    match b with
    | E 1 _ | E 2 _ | E 3 _ | E 4 _ | E 5 _ | E 6 _ | E 7 _ | E 8 _ when arr.Length > 2 -> &arr[0]
    | _ -> &arr[1]
[<EntryPoint>]
let main _ =
    let arr = [| 10; 20; 30 |]
    (f arr 3) <- 99
    (f arr 42) <- 77
    printfn "%d %d" arr[0] arr[1]
    0
"""
        |> FSharp
        |> asExe
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContains "99 77"
