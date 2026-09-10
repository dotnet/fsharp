namespace EmittedIL.RealInternalSignature

open Xunit
open FSharp.Test.Compiler

/// Regression tests for dotnet/fsharp#19933.
///
/// A closure synthesized inside a member declared in an *intrinsic augmentation*
/// (`type C with member ...`) used to be emitted as a sibling of `C` in the enclosing
/// module class instead of nested inside `C`. Under `--realsig+` a source-`private`
/// member of `C` is IL `private` (type-scoped), so the sibling closure could not reach
/// it and the CLR raised `MethodAccessException` at first call. Members declared in the
/// type's own body were always nested correctly; the fix makes augmentation members
/// consistent with them. These programs are legal F#; only IL placement was wrong.
///
/// THE FIX IS GATED ON `--realsig+`. Under `--realsig-` the member is IL `assembly` and
/// the closure reaches it regardless (and the inner-rec is often lambda-lifted to a module
/// static), so the emitted IL is identical before and after the fix — a `--realsig-`
/// runtime test cannot distinguish the buggy compiler from the fixed one. The matrix below
/// therefore runs under `--realsig+` only; one `--realsig-` smoke is kept for
/// defense-in-depth, and both realsig modes of the canonical shape are snapshotted by a
/// `.bsl` baseline (`AugmentationClosureNesting.fs`, hosted in `Inlining.fs`).
///
/// Each case compiles `--realsig+ --optimize+`, asserts the closure nests INSIDE the
/// declaring type (`<Type>/<closure>@N`, present) and is NOT a module/namespace sibling
/// (`<scope>/<closure>@N`, absent), then runs it. present/absent are raw IL substrings:
/// keep the trailing `@` but omit the `@N` digits so the fragment survives line-number
/// churn. Each assertion FAILS on the pre-fix compiler (sibling) and PASSES on the fix.
///
/// Every private member is `[<NoCompilerInlining>]`: a trivial private body is inlined
/// before codegen, erasing the call site and hiding the bug.
module Regression_RealsigAugmentationClosure =

    /// Shared skeleton for the shapes that differ only in the augmentation member body:
    /// a non-generic `C` with a static private `Secret`, augmented with `memberBody`, then
    /// invoked by `invoke` (which must yield 42). Triple-quoted-string concatenation (not
    /// interpolation) keeps embedded `seq { }` braces literal.
    let private header =
        "module Sample\n"
        + "type C() =\n"
        + "    static let mutable backing = 0\n"
        + "    static member Set v = backing <- v\n"
        + "    [<NoCompilerInlining>]\n"
        + "    static member private Secret() = backing + 1\n"
        + "type C with\n"

    let private cWith (memberBody: string) (invoke: string) =
        header
        + memberBody
        + "\n[<EntryPoint>]\nlet main _ =\n    C.Set 41\n    if "
        + invoke
        + " = 42 then 0 else 1\n"

    /// (name, source, present [nested], absent [sibling]).
    let shapeCases =
        [
          // ---- shapes sharing the C/Secret/EntryPoint skeleton (cWith) ----
          "inner-rec (canonical)",
            cWith "    member _.Run() =\n        let rec h n = if n = 0 then C.Secret() else h (n - 1)\n        h 5" "C().Run()",
            [| "Sample/C/h@" |], [| "Sample/h@" |]

          ; "property getter",
            cWith "    member _.Prop =\n        let rec h n = if n = 0 then C.Secret() else h (n - 1)\n        h 5" "C().Prop",
            [| "Sample/C/h@" |], [| "Sample/h@" |]

          ; "static method",
            cWith "    static member Run() =\n        let rec h n = if n = 0 then C.Secret() else h (n - 1)\n        h 5" "C.Run()",
            [| "Sample/C/h@" |], [| "Sample/h@" |]

          ; "nested closures",
            cWith "    member _.Run() =\n        let rec h n =\n            let inner () = C.Secret()\n            if n = 0 then inner () else h (n - 1)\n        h 5" "C().Run()",
            [| "Sample/C/h@" |], [| "Sample/h@" |]

          ; "mutual inner-rec",
            cWith "    member _.Run() =\n        let rec ev n = if n = 0 then C.Secret() else od (n - 1)\n        and od n = if n = 0 then C.Secret() else ev (n - 1)\n        ev 5" "C().Run()",
            [| "Sample/C/ev@"; "Sample/C/od@" |], [| "Sample/ev@"; "Sample/od@" |]

          ; "seq state machine",
            cWith "    member _.Run() = seq { yield C.Secret() }" "(C().Run() |> Seq.head)",
            [| "Sample/C/Run@" |], [| "Sample/Run@" |]

          // ---- shapes with genuinely different skeletons ----
          ; "instance private method",
            """module Sample
type C() =
    let mutable backing = 0
    member _.Set v = backing <- v
    [<NoCompilerInlining>]
    member private _.Secret() = backing + 1
type C with
    member this.Run() =
        let rec h n = if n = 0 then this.Secret() else h (n - 1)
        h 5
[<EntryPoint>]
let main _ =
    let c = C() in c.Set 41
    if c.Run() = 42 then 0 else 1
""",
            [| "Sample/C/h@" |], [| "Sample/h@" |]

          ; "generic type inner-rec",
            """module Sample
type Holder<'T>() =
    static let mutable backing = 0
    static member Set v = backing <- v
    [<NoCompilerInlining>]
    static member private Secret() = backing + 1
type Holder<'T> with
    member _.Run() =
        let rec h n = if n = 0 then Holder<'T>.Secret() else h (n - 1)
        h 5
[<EntryPoint>]
let main _ =
    Holder<int>.Set 41
    if Holder<int>().Run() = 42 then 0 else 1
""",
            [| "Sample/Holder`1/h@" |], [| "Sample/h@" |]

          ; "generic method threads typars",
            """module Sample
type Holder<'T>() =
    static let mutable backing = 0
    static member Set v = backing <- v
    [<NoCompilerInlining>]
    static member private Secret() = backing + 1
type Holder<'T> with
    member _.M<'U>(u: 'U) =
        let rec h n (acc: 'U) = if n = 0 then (acc, Holder<'T>.Secret()) else h (n - 1) acc
        h 5 u
[<EntryPoint>]
let main _ =
    Holder<int>.Set 41
    let (_, s) = Holder<int>().M<string>("x")
    if s = 42 then 0 else 1
""",
            [| "Sample/Holder`1/h@" |], [| "Sample/h@" |]

          ; "property setter",
            """module Sample
type C() =
    static let mutable result = 0
    static member Get() = result
    [<NoCompilerInlining>]
    static member private Secret(v) = v + 1
    static member internal Store v = result <- v
type C with
    member _.Prop
        with set (v: int) =
            let rec h n acc = if n = 0 then acc else h (n - 1) (acc + C.Secret(v))
            C.Store(h 1 0)
[<EntryPoint>]
let main _ =
    let c = C()
    c.Prop <- 41
    if C.Get() = 42 then 0 else 1
""",
            [| "Sample/C/h@" |], [| "Sample/h@" |]

          ; "indexer",
            """module Sample
type C() =
    [<NoCompilerInlining>]
    static member private Secret(i) = i + 1
type C with
    member _.Item
        with get (i: int) =
            let rec h n acc = if n = 0 then acc else h (n - 1) (acc + C.Secret(i))
            h 1 0
[<EntryPoint>]
let main _ = if C().[41] = 42 then 0 else 1
""",
            [| "Sample/C/h@" |], [| "Sample/h@" |]

          ; "operator",
            """module Sample
type C(v: int) =
    member _.V = v
    [<NoCompilerInlining>]
    static member private Secret(x: int) = x + 1
type C with
    static member (+) (a: C, b: C) =
        let rec h n acc = if n = 0 then acc else h (n - 1) (acc + C.Secret(a.V))
        C(h b.V 0)
[<EntryPoint>]
let main _ = if (C(41) + C(1)).V = 42 then 0 else 1
""",
            [| "Sample/C/h@" |], [| "Sample/h@" |]

          ; "task state machine",
            """module Sample
open System.Threading.Tasks
type C() =
    static let mutable backing = 0
    static member Set v = backing <- v
    [<NoCompilerInlining>]
    static member private Secret() = backing + 1
type C with
    member _.Run() : Task<int> = task { return C.Secret() }
[<EntryPoint>]
let main _ =
    C.Set 41
    if (C().Run()).Result = 42 then 0 else 1
""",
            [| "Sample/C/Run@" |], [| "Sample/Run@"; "Sample/'Run@" |]

          ; "async",
            """module Sample
type C() =
    static let mutable backing = 0
    static member Set v = backing <- v
    [<NoCompilerInlining>]
    static member private Secret() = backing + 1
type C with
    member _.Run() = async { return C.Secret() }
[<EntryPoint>]
let main _ =
    C.Set 41
    if (C().Run() |> Async.RunSynchronously) = 42 then 0 else 1
""",
            [| "Sample/C/Run@" |], [| "Sample/Run@"; "Sample/'Run@" |]

          ; "quotation splice",
            """module Sample
open Microsoft.FSharp.Quotations
type C() =
    static let mutable backing = 0
    static member Set v = backing <- v
    [<NoCompilerInlining>]
    static member private Secret() = backing + 1
type C with
    member _.MakeQ() : Expr<int> =
        let rec h n acc = if n = 0 then acc else h (n - 1) (acc + C.Secret())
        let v = h 3 0
        <@ %%(Expr.Value v) : int @>
[<EntryPoint>]
let main _ =
    C.Set 9
    match C().MakeQ() with
    | Patterns.Value(o, _) -> if unbox<int> o = 30 then 0 else 1
    | _ -> 2
""",
            [| "Sample/C/h@" |], [| "Sample/h@" |]

          ; "record augmentation",
            """module Sample
type R = { X: int } with
    [<NoCompilerInlining>]
    static member private Secret(v) = v + 1
type R with
    member this.Run() =
        let rec h n acc = if n = 0 then acc else h (n - 1) (acc + R.Secret(this.X))
        h 1 0
[<EntryPoint>]
let main _ = if { X = 41 }.Run() = 42 then 0 else 1
""",
            [| "Sample/R/h@" |], [| "Sample/h@" |]

          ; "DU augmentation",
            """module Sample
type D =
    | A of int
    [<NoCompilerInlining>]
    static member private Secret(v) = v + 1
type D with
    member this.Run() =
        let x = match this with A v -> v
        let rec h n acc = if n = 0 then acc else h (n - 1) (acc + D.Secret(x))
        h 1 0
[<EntryPoint>]
let main _ = if (A 41).Run() = 42 then 0 else 1
""",
            [| "Sample/D/h@" |], [| "Sample/h@" |]

          ; "override member",
            """module Sample
[<AbstractClass>]
type B() =
    abstract M : unit -> int
type C() =
    inherit B()
    [<NoCompilerInlining>]
    static member private Secret() = 42
type C with
    override _.M() =
        let rec h n = if n = 0 then C.Secret() else h (n - 1)
        h 5
[<EntryPoint>]
let main _ = if (C() :> B).M() = 42 then 0 else 1
""",
            [| "Sample/C/h@" |], [| "Sample/h@" |]

          ; "secondary constructor",
            """module Sample
type C(x: int) =
    member _.X = x
    [<NoCompilerInlining>]
    static member private Secret() = 42
type C with
    new() =
        let rec h n = if n = 0 then C.Secret() else h (n - 1)
        C(h 5)
[<EntryPoint>]
let main _ = if C().X = 42 then 0 else 1
""",
            [| "Sample/C/h@" |], [| "Sample/h@" |]

          ; "struct augmentation",
            """module Sample
[<Struct>]
type C =
    val X: int
    new(x) = { X = x }
    [<NoCompilerInlining>]
    static member private Secret(v) = v + 1
type C with
    member this.Run() =
        let captured = this.X
        let rec h n acc = if n = 0 then acc else h (n - 1) (acc + C.Secret(captured))
        h 1 0
[<EntryPoint>]
let main _ = if C(41).Run() = 42 then 0 else 1
""",
            [| "Sample/C/h@" |], [| "Sample/h@" |]

          ; "namespace-scoped type",
            """namespace MyNs
type C() =
    static let mutable backing = 0
    static member Set v = backing <- v
    [<NoCompilerInlining>]
    static member private Secret() = backing + 1
type C with
    member _.Run() =
        let rec h n = if n = 0 then C.Secret() else h (n - 1)
        h 5
module Main =
    [<EntryPoint>]
    let main _ =
        C.Set 41
        if C().Run() = 42 then 0 else 1
""",
            [| "MyNs.C/h@" |], [| "MyNs.h@" |]

          ; "nested-module type",
            """module Top
module Inner =
    type C() =
        static let mutable backing = 0
        static member Set v = backing <- v
        [<NoCompilerInlining>]
        static member private Secret() = backing + 1
    type C with
        member _.Run() =
            let rec h n = if n = 0 then C.Secret() else h (n - 1)
            h 5
[<EntryPoint>]
let main _ =
    Inner.C.Set 41
    if Inner.C().Run() = 42 then 0 else 1
""",
            [| "Top/Inner/C/h@" |], [| "Top/Inner/h@" |]

          // Two augmentations sharing the local closure name `h`: each must nest under its
          // OWN type and NEITHER may remain a module sibling. The shipped compiler emits the
          // second sibling with a disambiguator (`'h@N-1'`), so the absent list covers both
          // the plain and the quoted spellings rather than pinning Beta's mangled name.
          ; "two types, same closure name",
            """module Sample
type Alpha<'T>() =
    [<NoCompilerInlining>]
    static member private S() = 1
type Beta<'U>() =
    [<NoCompilerInlining>]
    static member private S() = 2
type Alpha<'T> with
    member _.Run() =
        let rec h n acc = if n = 0 then acc else h (n - 1) (acc + Alpha<'T>.S())
        h 5 0
type Beta<'U> with
    member _.Run() =
        let rec h n acc = if n = 0 then acc else h (n - 1) (acc + Beta<'U>.S())
        h 5 0
[<EntryPoint>]
let main _ =
    if Alpha<int>().Run() = 5 && Beta<int>().Run() = 10 then 0 else 1
""",
            [| "Sample/Alpha`1/h@" |], [| "Sample/h@"; "Sample/'h@" |]
        ]
        |> List.map (fun (name: string, src: string, nested: string[], sibling: string[]) ->
            [| box name; box src; box nested; box sibling |])

    /// Compile `--realsig+ --optimize+`, assert the closure nests under its declaring type
    /// (present) and is not a module/namespace sibling (absent), then run. The IL check sits
    /// between `compile` and `run`, so this cannot reuse `compileExeAndRun`.
    [<Theory; MemberData(nameof shapeCases)>]
    let ``Augmentation closure nests under its declaring type``
        (name: string)
        (source: string)
        (nested: string[])
        (sibling: string[])
        =
        ignore name

        let result =
            FSharp source
            |> withRealInternalSignature true
            |> asExe
            |> withOptimize
            |> ignoreWarnings
            |> compile
            |> shouldSucceed

        result |> verifyILPresent (List.ofArray nested)
        result |> verifyILNotPresent (List.ofArray sibling)
        result |> run |> shouldSucceed |> ignore

    /// Defense-in-depth only: under `--realsig-` the fix is a no-op (the legacy path emits
    /// the same IL before and after), so this does NOT guard #19933 — it guards against a
    /// future un-gating or a shared-path regression breaking the legacy path.
    [<Fact>]
    let ``Canonical augmentation closure still compiles and runs under realsig-`` () =
        cWith "    member _.Run() =\n        let rec h n = if n = 0 then C.Secret() else h (n - 1)\n        h 5" "C().Run()"
        |> FSharp
        |> withRealInternalSignature false
        |> asExe
        |> withOptimize
        |> ignoreWarnings
        |> compileAndRun
        |> shouldSucceed
        |> ignore
