namespace EmittedIL.RealInternalSignature

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

/// Runtime regression tests for dotnet/fsharp#19933.
///
/// A closure synthesized inside a member declared in an *intrinsic augmentation*
/// (`type C with member ...`) used to be emitted as a sibling of `C` in the
/// enclosing module class rather than nested inside `C`. Under `--realsig+` a
/// source-`private` member of `C` compiles to IL `private` (type-scoped), so the
/// sibling closure could not reach it and the CLR raised `MethodAccessException`
/// at first invocation. Members declared in the type's own body were always nested
/// correctly; the fix makes augmentation members consistent with them.
///
/// These programs are legal F# (the type checker accepts private access from any
/// lexical position within the declaring type, including inner lambdas / `task` /
/// `seq` / quotations). The bug was purely in IL closure placement; the fix does
/// not change access semantics — the private member stays IL `private`.
///
/// IMPORTANT: every private member here is marked `[<NoCompilerInlining>]` (or
/// reads non-inlinable state). A trivial private body is inlined by the optimizer
/// before codegen, which removes the call site and HIDES the bug — such a test
/// would pass even on the buggy compiler and guard nothing. The IL-nesting shape
/// is additionally locked by `Regression_RealsigAugmentationClosure_StructuralAssertions.fs`.
///
/// Each test runs under both realsig settings so a regression in either path is
/// caught at runtime.
module Regression_RealsigAugmentationClosure =

    let private compileOptimized realsig source =
        FSharp source
        |> withRealInternalSignature realsig
        |> asExe
        |> withOptimize
        |> ignoreWarnings

    let private compileRunSucceeds realsig source =
        source |> compileOptimized realsig |> compileExeAndRun |> shouldSucceed |> ignore

    /// Canonical #19933 shape: bare inner `let rec` in an augmentation member of a
    /// generic type, calling a type-private static.
    [<Theory; InlineData(true); InlineData(false)>]
    let ``Augmentation inner-rec calls type-private static of generic type`` (realsig: bool) =
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
"""
        |> compileRunSucceeds realsig

    /// Same shape on a non-generic type.
    [<Theory; InlineData(true); InlineData(false)>]
    let ``Augmentation inner-rec calls type-private static of non-generic type`` (realsig: bool) =
        """module Sample
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
    C.Set 41
    if C().Run() = 42 then 0 else 1
"""
        |> compileRunSucceeds realsig

    /// Type-private *instance* method accessed from an inner-rec in an augmentation.
    /// `[<NoCompilerInlining>]` is essential — without it the trivial instance getter
    /// inlines to an `assembly` field read and the test passes even on the buggy
    /// compiler.
    [<Theory; InlineData(true); InlineData(false)>]
    let ``Augmentation inner-rec calls type-private instance method`` (realsig: bool) =
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
"""
        |> compileRunSucceeds realsig

    /// Augmentation property getter with an inner-rec calling a type-private member.
    [<Theory; InlineData(true); InlineData(false)>]
    let ``Augmentation property getter inner-rec calls type-private`` (realsig: bool) =
        """module Sample
type C() =
    static let mutable backing = 0
    static member Set v = backing <- v
    [<NoCompilerInlining>]
    static member private Secret() = backing + 1
type C with
    member _.Prop =
        let rec h n = if n = 0 then C.Secret() else h (n - 1)
        h 5
[<EntryPoint>]
let main _ =
    C.Set 41
    if C().Prop = 42 then 0 else 1
"""
        |> compileRunSucceeds realsig

    /// Augmentation property setter with an inner-rec calling a type-private member.
    [<Theory; InlineData(true); InlineData(false)>]
    let ``Augmentation property setter inner-rec calls type-private`` (realsig: bool) =
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
"""
        |> compileRunSucceeds realsig

    /// Augmentation indexed property (`Item with get(i)`).
    [<Theory; InlineData(true); InlineData(false)>]
    let ``Augmentation indexer inner-rec calls type-private`` (realsig: bool) =
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
"""
        |> compileRunSucceeds realsig

    /// Augmentation STATIC method (not instance) with an inner-rec calling a private.
    [<Theory; InlineData(true); InlineData(false)>]
    let ``Augmentation static method inner-rec calls type-private`` (realsig: bool) =
        """module Sample
type C() =
    static let mutable backing = 0
    static member Set v = backing <- v
    [<NoCompilerInlining>]
    static member private Secret() = backing + 1
type C with
    static member Run() =
        let rec h n = if n = 0 then C.Secret() else h (n - 1)
        h 5
[<EntryPoint>]
let main _ =
    C.Set 41
    if C.Run() = 42 then 0 else 1
"""
        |> compileRunSucceeds realsig

    /// Operator (`static member (+)`) declared in an augmentation.
    [<Theory; InlineData(true); InlineData(false)>]
    let ``Augmentation operator inner-rec calls type-private`` (realsig: bool) =
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
"""
        |> compileRunSucceeds realsig

    /// `task { }` state machine in an augmentation referencing a type-private member.
    [<Theory; InlineData(true); InlineData(false)>]
    let ``Augmentation task computation expression calls type-private member`` (realsig: bool) =
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
"""
        |> compileRunSucceeds realsig

    /// `async { }` variant.
    [<Theory; InlineData(true); InlineData(false)>]
    let ``Augmentation async computation expression calls type-private member`` (realsig: bool) =
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
"""
        |> compileRunSucceeds realsig

    /// `seq { }` state machine in an augmentation.
    [<Theory; InlineData(true); InlineData(false)>]
    let ``Augmentation seq computation expression calls type-private member`` (realsig: bool) =
        """module Sample
type C() =
    static let mutable backing = 0
    static member Set v = backing <- v
    [<NoCompilerInlining>]
    static member private Secret() = backing + 1
type C with
    member _.Run() = seq { yield C.Secret() }
[<EntryPoint>]
let main _ =
    C.Set 41
    if (C().Run() |> Seq.head) = 42 then 0 else 1
"""
        |> compileRunSucceeds realsig

    /// Mutual recursion (`let rec ... and ...`) in an augmentation member.
    [<Theory; InlineData(true); InlineData(false)>]
    let ``Augmentation mutual inner-rec calls type-private member`` (realsig: bool) =
        """module Sample
type C() =
    static let mutable backing = 0
    static member Set v = backing <- v
    [<NoCompilerInlining>]
    static member private Secret() = backing + 1
type C with
    member _.Run() =
        let rec ev n = if n = 0 then C.Secret() else od (n - 1)
        and od n = if n = 0 then C.Secret() else ev (n - 1)
        ev 5
[<EntryPoint>]
let main _ =
    C.Set 41
    if C().Run() = 42 then 0 else 1
"""
        |> compileRunSucceeds realsig

    /// Nested closures (a lambda inside the inner-rec) touching a type-private member.
    [<Theory; InlineData(true); InlineData(false)>]
    let ``Augmentation nested closures call type-private member`` (realsig: bool) =
        """module Sample
type C() =
    static let mutable backing = 0
    static member Set v = backing <- v
    [<NoCompilerInlining>]
    static member private Secret() = backing + 1
type C with
    member _.Run() =
        let rec h n =
            let inner () = C.Secret()
            if n = 0 then inner () else h (n - 1)
        h 5
[<EntryPoint>]
let main _ =
    C.Set 41
    if C().Run() = 42 then 0 else 1
"""
        |> compileRunSucceeds realsig

    /// Generic class with a generic augmentation method whose closure captures both
    /// the class typar 'T and the method typar 'U and calls a type-private member.
    [<Theory; InlineData(true); InlineData(false)>]
    let ``Augmentation generic method threads typars and calls type-private`` (realsig: bool) =
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
"""
        |> compileRunSucceeds realsig

    /// Two augmentation members of DIFFERENT generic types in the same module whose
    /// inner-recs share the local name `h` — verifies no IL type-name collision after
    /// re-homing the closures under their respective types.
    [<Theory; InlineData(true); InlineData(false)>]
    let ``Augmentations on two types with same closure name do not collide`` (realsig: bool) =
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
"""
        |> compileRunSucceeds realsig

    /// Record-type augmentation.
    [<Theory; InlineData(true); InlineData(false)>]
    let ``Augmentation on record type inner-rec calls type-private`` (realsig: bool) =
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
"""
        |> compileRunSucceeds realsig

    /// DU-type augmentation.
    [<Theory; InlineData(true); InlineData(false)>]
    let ``Augmentation on DU type inner-rec calls type-private`` (realsig: bool) =
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
"""
        |> compileRunSucceeds realsig

    /// Quotation splice whose value is computed by an inner-rec in the augmentation.
    [<Theory; InlineData(true); InlineData(false)>]
    let ``Augmentation quotation splice driven by inner-rec calls type-private`` (realsig: bool) =
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
"""
        |> compileRunSucceeds realsig

    /// Type declared inside a namespace (not a module), augmentation in same namespace.
    [<Theory; InlineData(true); InlineData(false)>]
    let ``Augmentation inner-rec on namespace-scoped type calls type-private`` (realsig: bool) =
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
"""
        |> compileRunSucceeds realsig

    /// Type declared in a nested module, augmentation in the same nested module.
    [<Theory; InlineData(true); InlineData(false)>]
    let ``Augmentation inner-rec on nested-module type calls type-private`` (realsig: bool) =
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
"""
        |> compileRunSucceeds realsig
