namespace EmittedIL

open Xunit
open FSharp.Test.Compiler

module OptimizeClosureIfNotInlined =

    let private prelude =
        """
module M
let inline fold2 ([<InlineIfLambda; OptimizeClosureIfNotInlined>] folder: 'State -> 'T1 -> 'T2 -> 'State) (state: 'State) (a: 'T1[]) (b: 'T2[]) =
    let mutable s = state
    for i in 0 .. a.Length - 1 do
        s <- folder s a.[i] b.[i]
    s

[<System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)>]
let mkFolder () : int -> int -> int -> int = fun s x y -> s + x * y
"""

    let private optimized source =
        FSharp source
        |> withLangVersionPreview
        |> withOptions [ "--optimize+" ]
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``opaque multi-arg callback is Adapt-ed`` () =
        optimized (prelude + "let callOpaque (a: int[]) (b: int[]) = fold2 (mkFolder ()) 0 a b")
        |> verifyILPresent [ "::Adapt(" ]

    [<Fact>]
    let ``inlinable lambda callback is not Adapt-ed`` () =
        optimized (prelude + "let callLambda (a: int[]) (b: int[]) (k: int) = fold2 (fun s x y -> s + x * y + k) 0 a b")
        |> verifyILNotPresent [ "Adapt" ]

    // Declared arity exceeds the arity actually applied, so nothing is rewritten.
    [<Fact>]
    let ``over-arrows callback emits no dead Adapt`` () =
        optimized """
module M
let inline applyOverArrows ([<InlineIfLambda; OptimizeClosureIfNotInlined>] f: (int -> int) -> int -> (int -> int)) (g0: int -> int) (a: int[]) =
    let mutable acc = g0
    for i in 0 .. a.Length - 1 do
        acc <- f acc a.[i]
    acc

[<System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)>]
let mkF () : (int -> int) -> int -> (int -> int) = fun g x -> (fun z -> g z + x)

let callOverArrows (a: int[]) = applyOverArrows (mkF ()) id a
"""
        |> verifyILNotPresent [ "Adapt" ]

    [<Theory>]
    [<InlineData(2, "FSharpFunc`3")>]
    [<InlineData(5, "FSharpFunc`6")>]
    let ``boundary arity uses the matching OptimizedClosures type`` (arity: int) (expectedType: string) =
        let tyArrows = String.concat " -> " (List.replicate (arity + 1) "int")
        let formalArgs = String.concat " " [ for i in 1 .. arity -> $"x{i}" ]
        let applyArgs = String.concat " " [ yield "acc"; for _ in 2 .. arity -> "xs.[i]" ]
        let sumBody = String.concat " + " [ for i in 1 .. arity -> $"x{i}" ]
        optimized $"""
module M
let inline foldN ([<InlineIfLambda; OptimizeClosureIfNotInlined>] folder: {tyArrows}) (state: int) (xs: int[]) =
    let mutable acc = state
    for i in 0 .. xs.Length - 1 do
        acc <- folder {applyArgs}
    acc

[<System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)>]
let mkFolder () : {tyArrows} = fun {formalArgs} -> {sumBody}

let callOpaque (xs: int[]) = foldN (mkFolder ()) 0 xs
"""
        |> verifyILPresent [ $"OptimizedClosures/{expectedType}"; "::Adapt("; "::Invoke(" ]

    // The adapted form must be observationally identical to the un-attributed (InvokeFast) form: same result,
    // callback invoked once per element in order, opaque actual evaluated exactly once.
    [<Fact>]
    let ``optimized opaque callback matches un-attributed results and effect order`` () =
        FSharp """
module M
open System.Collections.Generic
let log = List<string>()

let inline fold2_ocini ([<InlineIfLambda; OptimizeClosureIfNotInlined>] folder: 'S -> 'a -> 'b -> 'S) (state: 'S) (a: 'a[]) (b: 'b[]) =
    let mutable s = state
    for i in 0 .. a.Length - 1 do s <- folder s a.[i] b.[i]
    s
let inline fold2_plain ([<InlineIfLambda>] folder: 'S -> 'a -> 'b -> 'S) (state: 'S) (a: 'a[]) (b: 'b[]) =
    let mutable s = state
    for i in 0 .. a.Length - 1 do s <- folder s a.[i] b.[i]
    s

[<System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)>]
let mkLoggingFolder (tag: string) : int -> int -> int -> int = fun s x y -> log.Add(tag); s + x * y

let mutable factoryCalls = 0
[<System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)>]
let mkCountingFolder () : int -> int -> int -> int =
    factoryCalls <- factoryCalls + 1
    fun s x y -> s + x + y

[<EntryPoint>]
let main _ =
    let a = [| 1; 2; 3; 4 |]
    let b = [| 5; 6; 7; 8 |]
    let rOcini = fold2_ocini (mkLoggingFolder "O") 0 a b
    let orderOcini = String.concat "," log
    log.Clear()
    let rPlain = fold2_plain (mkLoggingFolder "P") 0 a b
    let orderPlain = String.concat "," log
    factoryCalls <- 0
    let _ = fold2_ocini (mkCountingFolder ()) 0 a b
    let ok = rOcini = rPlain && orderOcini = "O,O,O,O" && orderPlain = "P,P,P,P" && factoryCalls = 1
    printfn "RESULT=%s" (if ok then "PASS" else "FAIL")
    0
"""
        |> withLangVersionPreview
        |> withOptions [ "--optimize+" ]
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContains "RESULT=PASS"

    // Cross-assembly: the optimization crosses the assembly boundary into a consumer pinned to an old
    // language version, with no error and no leftover per-element InvokeFast dispatch.
    [<Fact>]
    let ``opaque callback is adapted across an assembly boundary at old langversion`` () =
        let library =
            FSharp """
module Lib
let inline fold2 ([<InlineIfLambda; OptimizeClosureIfNotInlined>] folder: 'S -> 'a -> 'b -> 'S) (state: 'S) (a: 'a[]) (b: 'b[]) =
    let mutable s = state
    for i in 0 .. a.Length - 1 do s <- folder s a.[i] b.[i]
    s
"""
            |> withLangVersionPreview
            |> withOptions [ "--optimize+" ]
            |> asLibrary

        let consumer =
            FSharp """
module App
[<System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)>]
let mkFolder () : int -> int -> int -> int = fun s x y -> s + x * y
let callOpaque (a: int[]) (b: int[]) = Lib.fold2 (mkFolder ()) 0 a b
"""
            |> withLangVersion "7.0"
            |> withOptions [ "--optimize+" ]
            |> withReferences [ library ]
            |> compile
            |> shouldSucceed

        consumer |> verifyILPresent [ "OptimizedClosures/FSharpFunc`4"; "::Adapt(" ]
        consumer |> verifyILNotPresent [ "InvokeFast" ]

    // Accepted only with InlineIfLambda, on an inlined function whose type is a curried arity-2..5 F# function.
    [<Theory>]
    [<InlineData("let inline f ([<InlineIfLambda; OptimizeClosureIfNotInlined>] g: int -> int) x = g x")>]
    [<InlineData("let inline f ([<InlineIfLambda; OptimizeClosureIfNotInlined>] g: int -> int -> int -> int -> int -> int -> int) a b c d e h = g a b c d e h")>]
    [<InlineData("let inline f ([<InlineIfLambda; OptimizeClosureIfNotInlined>] g: int) x = g + x")>]
    [<InlineData("let inline f ([<InlineIfLambda; OptimizeClosureIfNotInlined>] g: (int * int) -> int) x = g x")>]
    [<InlineData("let f ([<InlineIfLambda; OptimizeClosureIfNotInlined>] g: int -> int -> int) x y = g x y")>]
    [<InlineData("let inline f ([<OptimizeClosureIfNotInlined>] g: int -> int -> int) x y = g x y")>]
    let ``attribute misuse is rejected with FS3916`` (decl: string) =
        FSharp ("module M\n" + decl)
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withErrorCode 3916

    [<Fact>]
    let ``attribute requires the preview language feature`` () =
        FSharp "module M\nlet inline f ([<InlineIfLambda; OptimizeClosureIfNotInlined>] g: int -> int -> int) x y = g x y"
        |> withLangVersion "7.0"
        |> compile
        |> shouldFail
        |> withErrorCode 3350
