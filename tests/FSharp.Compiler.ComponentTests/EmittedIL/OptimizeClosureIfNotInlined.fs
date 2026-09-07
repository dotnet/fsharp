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

    let private runOutput source =
        FSharp source
        |> withLangVersionPreview
        |> withOptions [ "--optimize+" ]
        |> compileExeAndRun
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
    [<InlineData(3, "FSharpFunc`4")>]
    [<InlineData(4, "FSharpFunc`5")>]
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

    // Distinct argument/result types across an arity-4 callback catch any generic-slot mix-up in the adapted
    // Invoke that a homogeneous int callback would hide.
    [<Fact>]
    let ``adapted callback with heterogeneous argument types is correct`` () =
        runOutput """
module M
let inline apply4 ([<InlineIfLambda; OptimizeClosureIfNotInlined>] f: int -> string -> bool -> float -> decimal) (xs: int[]) =
    let mutable acc = 0M
    for x in xs do acc <- acc + f x "k" true 2.0
    acc

[<System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)>]
let mkF () : int -> string -> bool -> float -> decimal =
    fun i s b d -> decimal i + decimal s.Length + (if b then 10M else 0M) + decimal d

[<EntryPoint>]
let main _ =
    printfn "RESULT=%M" (apply4 (mkF ()) [| 1; 2; 3 |])
    0
"""
        |> withStdOutContains "RESULT=45"

    // The adapted form must be observationally identical to the un-attributed (InvokeFast) form: same result,
    // callback invoked once per element with the accumulator and elements in order, opaque actual evaluated once.
    [<Fact>]
    let ``optimized opaque callback matches un-attributed results and effect order`` () =
        runOutput """
module M
let log = ResizeArray<int * int * int>()
let mutable factoryCalls = 0

let inline fold2_ocini ([<InlineIfLambda; OptimizeClosureIfNotInlined>] folder: 'S -> 'a -> 'b -> 'S) (state: 'S) (a: 'a[]) (b: 'b[]) =
    let mutable s = state
    for i in 0 .. a.Length - 1 do s <- folder s a.[i] b.[i]
    s
let inline fold2_plain ([<InlineIfLambda>] folder: 'S -> 'a -> 'b -> 'S) (state: 'S) (a: 'a[]) (b: 'b[]) =
    let mutable s = state
    for i in 0 .. a.Length - 1 do s <- folder s a.[i] b.[i]
    s

[<System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)>]
let mkTracingFolder () : int -> int -> int -> int =
    factoryCalls <- factoryCalls + 1
    fun s x y -> log.Add((s, x, y)); s + x * y

[<EntryPoint>]
let main _ =
    let a = [| 1; 2; 3; 4 |]
    let b = [| 5; 6; 7; 8 |]
    let expected = [ (0, 1, 5); (5, 2, 6); (17, 3, 7); (38, 4, 8) ]

    factoryCalls <- 0
    log.Clear()
    let rOcini = fold2_ocini (mkTracingFolder ()) 0 a b
    let traceOcini = List.ofSeq log
    let callsOcini = factoryCalls

    log.Clear()
    let rPlain = fold2_plain (mkTracingFolder ()) 0 a b
    let tracePlain = List.ofSeq log

    let ok = rOcini = 70 && rPlain = 70 && traceOcini = expected && tracePlain = expected && callsOcini = 1
    printfn "RESULT=%s" (if ok then "PASS" else "FAIL")
    0
"""
        |> withStdOutContains "RESULT=PASS"

    // The callback used both saturated (in the loop) and partially applied (captured) in the same body: the
    // saturated calls are adapted and the partial application is left as-is; the result must still match the
    // un-attributed form.
    [<Fact>]
    let ``callback used saturated and partially applied stays correct`` () =
        runOutput """
module M
let inline foldMixed ([<InlineIfLambda; OptimizeClosureIfNotInlined>] folder: int -> int -> int -> int) (state: int) (a: int[]) =
    let mutable s = state
    let partial = folder state
    for i in 0 .. a.Length - 1 do s <- folder s a.[i] a.[i]
    s + partial 100 200

[<System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)>]
let mkFolder () : int -> int -> int -> int = fun s x y -> s + x * y

[<EntryPoint>]
let main _ =
    printfn "RESULT=%d" (foldMixed (mkFolder ()) 0 [| 1; 2; 3 |])
    0
"""
        |> withStdOutContains "RESULT=20014"

    // A quotation inside a rewritten inline body must be left intact: the transform sets RewriteQuotations to
    // false, so the reflected `f 1 2 3` stays an application even though the sibling call is adapted.
    [<Fact>]
    let ``quotation inside a rewritten body is not adapted`` () =
        runOutput """
module M
open Microsoft.FSharp.Quotations
let inline applyAndQuote ([<InlineIfLambda; OptimizeClosureIfNotInlined>] f: int -> int -> int -> int) : int * Expr<int> =
    f 1 2 3, <@ f 1 2 3 @>

[<System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)>]
let mkFolder () : int -> int -> int -> int = fun s x y -> s + x * y

[<EntryPoint>]
let main _ =
    let result, quoted = applyAndQuote (mkFolder ())
    let s = quoted.ToString()
    printfn "RESULT=%b" (result = 7 && not (s.Contains "Invoke") && not (s.Contains "Adapt"))
    0
"""
        |> withStdOutContains "RESULT=true"

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

    // FS3916: accepted only with InlineIfLambda, on an inlined function/member, where the parameter is alone
    // in its argument group and has a curried F# function type of arity 2..5. Rejected everywhere else,
    // including tupled/method groups and declaration-only positions (constructor, abstract member, delegate)
    // where the optimization can never fire.
    [<Theory>]
    [<InlineData("let inline f ([<InlineIfLambda; OptimizeClosureIfNotInlined>] g: int -> int) x = g x")>]
    [<InlineData("let inline f ([<InlineIfLambda; OptimizeClosureIfNotInlined>] g: int -> int -> int -> int -> int -> int -> int) a b c d e h = g a b c d e h")>]
    [<InlineData("let inline f ([<InlineIfLambda; OptimizeClosureIfNotInlined>] g: int) x = g + x")>]
    [<InlineData("let inline f ([<InlineIfLambda; OptimizeClosureIfNotInlined>] g: (int * int) -> int) x = g x")>]
    [<InlineData("let f ([<InlineIfLambda; OptimizeClosureIfNotInlined>] g: int -> int -> int) x y = g x y")>]
    [<InlineData("let inline f ([<OptimizeClosureIfNotInlined>] g: int -> int -> int) x y = g x y")>]
    [<InlineData("type H =\n    static member inline Fold([<InlineIfLambda; OptimizeClosureIfNotInlined>] f: int -> int -> int, xs: int[]) =\n        let mutable s = 0\n        for x in xs do s <- f s x\n        s")>]
    [<InlineData("type C([<OptimizeClosureIfNotInlined>] value: int) =\n    member _.Value = value")>]
    [<InlineData("type I =\n    abstract M: [<InlineIfLambda; OptimizeClosureIfNotInlined>] f: (int -> int -> int) -> unit")>]
    [<InlineData("type D = delegate of [<InlineIfLambda; OptimizeClosureIfNotInlined>] f: (int -> int -> int) -> unit")>]
    let ``attribute is rejected where it cannot take effect`` (decl: string) =
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
