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

    // https://github.com/dotnet/fsharp/issues/20632
    // Guarded rules on the second column between rules on the first column, as in LexFilter's hwTokenFetch.
    // Every branch of a switch on the first column would get its own copy of the guarded rules.
    let private interleavedGuardsSource =
        let ctxs, toks = 20, 12

        let rules =
            [ yield "T0", "C0 p :: _", "col < p"
              for k in 0 .. ctxs - 1 -> "_", $"C%d{k} p :: _", $"col <= p + %d{k}"
              for k in 1 .. toks - 1 -> $"T%d{k}", "_ :: _", (if k % 3 = 0 then $"col %% %d{k} = 0" else "true") ]

        let clauses =
            rules
            |> List.mapi (fun i (tok, stack, guard) -> $"    | %s{tok}, %s{stack} when %s{guard} -> %d{i}")
            |> String.concat "\n"

        let reference =
            rules
            |> List.mapi (fun i (tok, stack, guard) ->
                let tokTest = if tok = "_" then "true" else $"tok = %s{tok}"
                $"    elif (%s{tokTest}) && (match stack with %s{stack} -> %s{guard} | _ -> false) then %d{i}")
            |> String.concat "\n"

        let cases prefix n field =
            [ for k in 0 .. n - 1 -> $"    | %s{prefix}%d{k}%s{field}" ] |> String.concat "\n"

        $"""module Test
open Microsoft.FSharp.Reflection
type Ctx =
{cases "C" ctxs " of int"}
type Tok =
{cases "T" toks ""}
let f (tok: Tok) (stack: Ctx list) (col: int) =
    match tok, stack with
{clauses}
    | _ -> -1
let reference (tok: Tok) (stack: Ctx list) (col: int) =
    if false then -2
{reference}
    else -1
[<EntryPoint>]
let main _ =
    let tokens = FSharpType.GetUnionCases typeof<Tok> |> Array.map (fun c -> FSharpValue.MakeUnion(c, [||]) :?> Tok)
    let ctx k (p: int) = FSharpValue.MakeUnion((FSharpType.GetUnionCases typeof<Ctx>).[k], [| box p |]) :?> Ctx
    let stacks = [ yield []; for k in 0 .. {ctxs - 1} do for p in 0 .. 3 -> [ ctx k p ] ]
    let mismatches = Seq.length (seq {{ for tok in tokens do for stack in stacks do for col in 0 .. 25 do if f tok stack col <> reference tok stack col then yield () }})
    let ilSize = typeof<Ctx>.DeclaringType.GetMethod("f").GetMethodBody().GetILAsByteArray().Length
    printfn "mismatches=%%d small=%%b" mismatches (ilSize < 4000)
    0
"""

    [<Fact>]
    let ``Issue 20632 - guarded rules between first-column rules are emitted once`` () =
        interleavedGuardsSource
        |> FSharp
        |> withOptimize
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContains "mismatches=0 small=true"

    // https://github.com/dotnet/fsharp/pull/20718#discussion_r4219278089
    // A failed guard that writes the data the match tests must not change which later rule matches.
    let private guardWritesTestedDataSource =
        let matchOn header write (pat: string -> string -> string) =
            [ yield header
              yield $"""    | %s{pat "0" "0"} when (%s{write}; false) -> 0"""
              for c in 0..7 -> $"""    | %s{pat "_" (string c)} when col <= %d{c} -> 1%d{c}"""
              for t in 1..7 -> $"""    | %s{pat (string t) "_"} -> %d{t}"""
              yield "    | _ -> -1" ]
            |> String.concat "\n"

        let pair =
            matchOn "let pair (value: byref<Pair>) col =\n    match value with" "value <- Pair (7, 7)" (fun t c -> $"Pair (%s{t}, %s{c})")

        let array =
            matchOn "let array (arr: int[]) col =\n    match arr with" "arr.[0] <- 7; arr.[1] <- 7" (fun t c -> $"[| %s{t}; %s{c} |]")

        let record =
            matchOn "let record (r: R) col =\n    match r with" "r.T <- 7; r.C <- 7" (fun t c -> $"{{ T = %s{t}; C = %s{c} }}")

        $"""module Test
[<Struct>]
type Pair = Pair of token: int * context: int
type R = {{ mutable T: int; mutable C: int }}
{pair}
{array}
{record}
[<EntryPoint>]
let main _ =
    let mutable input = Pair (0, 0)
    printfn "pair=%%d array=%%d record=%%d" (pair &input 100) (array [| 0; 0 |] 100) (record {{ T = 0; C = 0 }} 100)
    0
"""

    [<Theory>]
    [<InlineData(false)>]
    [<InlineData(true)>]
    let ``Guards that write the tested data keep the rule the whole match would pick`` optimize =
        guardWritesTestedDataSource
        |> FSharp
        |> (if optimize then withOptimize else withNoOptimize)
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContains "pair=-1 array=-1 record=-1"
