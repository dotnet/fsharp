// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.PatternMatching

open Xunit
open FSharp.Test.Compiler

module GuardedOrPatternComplexity =

    // https://github.com/dotnet/fsharp/issues/18425
    let private runsWith expected source =
        source
        |> FSharp
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContains expected

    let private compiles source =
        source |> FSharp |> compile |> shouldSucceed

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
    let r1 = f 8 3
    let r2 = f 4000 1
    printfn "r1=%d r2=%d" r1 r2
    0
"""

        template.Replace("__DISJUNCTS__", disjuncts)

    [<Fact>]
    let ``Issue 18425 - guarded shared-or partial active pattern match compiles and runs`` () =
        guardedOrSource 24
        |> runsWith "r1=-1 r2=2000"

    // Emits ~506KB today. Without promotion this input does not merely exceed the bound, it OOMs
    // the compiler, so the exact constant is not load-bearing; the refuted shared-target design emitted ~3MB.
    [<Fact>]
    let ``Issue 18425 - promoted subtrees are shared by name, not copied`` () =
        guardedOrSource 32
        |> FSharp
        |> asExe
        |> compile
        |> shouldSucceed
        |> withPeReader (fun pe -> pe.GetEntireImage().Length)
        |> fun emitted -> Assert.True(emitted < 1_000_000, $"emitted assembly is {emitted} bytes")

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
        |> runsWith "150 160 170 -1"

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
        |> runsWith "99 77"

    [<Fact>]
    let ``Issue 18425 - guarded shared-or in a catch handler can rethrow`` () =
        """module Test
let (|E|_|) (n: int) (e: exn) = if e.Message = string n then Some() else None
let f () =
    try failwith "1" with
    | (E 1 | E 2 | E 3 | E 4 | E 5 | E 6 | E 7 | E 8) when System.Environment.TickCount >= System.Int32.MinValue -> 1
    | _ -> reraise()
"""
        |> compiles

    [<Fact>]
    let ``Issue 18425 - guarded shared-or with a byref-like clause target stays inline`` () =
        """module Test
let (|E|_|) (n: int) (x: int) = if x = n then Some x else None
let f (buffer: byref<int>) x =
    match x with
    | E 1 _ | E 2 _ | E 3 _ | E 4 _ | E 5 _ | E 6 _ | E 7 _ | E 8 _ when System.Environment.TickCount >= System.Int32.MinValue -> buffer
    | _ -> 0
"""
        |> compiles
