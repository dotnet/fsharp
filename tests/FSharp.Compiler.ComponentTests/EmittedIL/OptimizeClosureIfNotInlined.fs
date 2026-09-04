namespace EmittedIL

open Xunit
open FSharp.Test.Compiler

module OptimizeClosureIfNotInlined =

    let private prelude =
        """
module M
open Microsoft.FSharp.Core

let inline fold2 ([<InlineIfLambda; OptimizeClosureIfNotInlined>] folder: 'State -> 'T1 -> 'T2 -> 'State) (state: 'State) (a: 'T1[]) (b: 'T2[]) =
    let mutable s = state
    for i in 0 .. a.Length - 1 do
        s <- folder s a.[i] b.[i]
    s

[<System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)>]
let mkFolder () : int -> int -> int -> int = fun s x y -> s + x * y
"""

    let private compileOptimized source =
        FSharp(prelude + source)
        |> withLangVersionPreview
        |> withOptions [ "--optimize+" ]
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``opaque multi-arg callback is Adapt-ed once`` () =
        compileOptimized """
let callOpaque (a: int[]) (b: int[]) =
    let f = mkFolder ()
    fold2 f 0 a b
"""
        |> verifyILPresent [ "::Adapt(" ]

    [<Fact>]
    let ``inlinable lambda callback is not Adapt-ed`` () =
        compileOptimized """
let callLambda (a: int[]) (b: int[]) (k: int) =
    fold2 (fun s x y -> s + x * y + k) 0 a b
"""
        |> verifyILNotPresent [ "Adapt" ]

    // A non-generic function-typed parameter whose declared type has more arrows than the body applies:
    // `stripFunTy` over-counts the arity, so no saturated application is rewritten. The transform must then
    // emit no `Adapt` at all rather than a dead adapter binding plus the slow per-element path.
    [<Fact>]
    let ``over-arrows callback is left untouched (no dead Adapt)`` () =
        FSharp """
module M
let inline applyOverArrows ([<InlineIfLambda; OptimizeClosureIfNotInlined>] f: (int -> int) -> int -> (int -> int)) (g0: int -> int) (a: int[]) =
    let mutable acc = g0
    for i in 0 .. a.Length - 1 do
        acc <- f acc a.[i]
    acc

[<System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)>]
let mkF () : (int -> int) -> int -> (int -> int) = fun g x -> (fun z -> g z + x)

let callOverArrows (a: int[]) =
    let f = mkF ()
    applyOverArrows f id a
"""
        |> withLangVersionPreview
        |> withOptions [ "--optimize+" ]
        |> compile
        |> shouldSucceed
        |> verifyILNotPresent [ "Adapt" ]

    [<Fact>]
    let ``attribute without InlineIfLambda is rejected`` () =
        FSharp """
module M
let inline f ([<OptimizeClosureIfNotInlined>] g: int -> int -> int) x y = g x y
"""
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withErrorCode 3916
