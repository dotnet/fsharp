namespace EmittedIL.RealInternalSignature

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

/// Regression tests for dotnet/fsharp#19933.
///
/// A closure synthesized inside a member body that is declared in an *intrinsic
/// augmentation* (`type C with member ...`) used to be emitted as a sibling of `C`
/// in the enclosing module class, rather than nested inside `C`. Under `--realsig+`
/// a source-`private` member of `C` compiles to IL `private` (type-scoped), so the
/// sibling closure could not reach it and the CLR raised `MethodAccessException` at
/// first invocation. Members declared in the type's own body were always nested
/// correctly; the fix makes augmentation members consistent with them.
///
/// These programs are legal F# (the type checker accepts private access from any
/// lexical position within the declaring type, including inner lambdas / `task` /
/// quotations). The bug was purely in IL closure placement; the fix does not change
/// access semantics — the private member stays IL `private`.
///
/// Each test runs under both realsig settings so a regression in either path is
/// caught. The non-inlinable private member is essential: a trivial body is inlined
/// away by the optimizer before codegen, which hides the bug.
module Regression_RealsigAugmentationClosure =

    let private compileOptimized realsig source =
        FSharp source
        |> withRealInternalSignature realsig
        |> asExe
        |> withOptimize
        |> ignoreWarnings

    let private compileRunSucceeds realsig source =
        source |> compileOptimized realsig |> compileExeAndRun |> shouldSucceed |> ignore

    /// Bare inner `let rec` in an augmentation member, calling a type-private static
    /// of a generic type. This is the canonical #19933 shape.
    [<Theory; InlineData(true); InlineData(false)>]
    let ``Augmentation inner-rec calls type-private static of generic type`` (realsig: bool) =
        """module Sample
type Holder<'T>() =
    static let mutable backing = 0
    static member Set v = backing <- v
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

    /// Type-private *instance* member accessed from an inner-rec in an augmentation.
    [<Theory; InlineData(true); InlineData(false)>]
    let ``Augmentation inner-rec calls type-private instance method`` (realsig: bool) =
        """module Sample
type C() =
    let mutable backing = 0
    member _.Set v = backing <- v
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

    /// `task { }` state machine in an augmentation member referencing a type-private
    /// member directly — no inner-rec; the state machine itself is the closure.
    [<Theory; InlineData(true); InlineData(false)>]
    let ``Augmentation task computation expression calls type-private member`` (realsig: bool) =
        """module Sample
open System.Threading.Tasks
type C() =
    static let mutable backing = 0
    static member Set v = backing <- v
    static member private Secret() = backing + 1
type C with
    member _.Run() : Task<int> = task { return C.Secret() }
[<EntryPoint>]
let main _ =
    C.Set 41
    if (C().Run()).Result = 42 then 0 else 1
"""
        |> compileRunSucceeds realsig

    /// `async { }` variant, to confirm the fix is not task-specific.
    [<Theory; InlineData(true); InlineData(false)>]
    let ``Augmentation async computation expression calls type-private member`` (realsig: bool) =
        """module Sample
type C() =
    static let mutable backing = 0
    static member Set v = backing <- v
    static member private Secret() = backing + 1
type C with
    member _.Run() = async { return C.Secret() }
[<EntryPoint>]
let main _ =
    C.Set 41
    if (C().Run() |> Async.RunSynchronously) = 42 then 0 else 1
"""
        |> compileRunSucceeds realsig

    /// Mutual recursion (`let rec ... and ...`) in an augmentation member.
    [<Theory; InlineData(true); InlineData(false)>]
    let ``Augmentation mutual inner-rec calls type-private member`` (realsig: bool) =
        """module Sample
type C() =
    static let mutable backing = 0
    static member Set v = backing <- v
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

    /// A trivial private member forced non-inlinable with `[<NoCompilerInlining>]`,
    /// proving the failure is a real CLR access check and not constant folding.
    [<Theory; InlineData(true); InlineData(false)>]
    let ``Augmentation inner-rec calls NoCompilerInlining type-private`` (realsig: bool) =
        """module Sample
type C() =
    [<NoCompilerInlining>]
    static member private Secret() = 1
type C with
    member _.Run() =
        let rec h n acc = if n = 0 then acc else h (n - 1) (acc + C.Secret())
        h 5 0
[<EntryPoint>]
let main _ = if C().Run() = 5 then 0 else 1
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

    /// Type declared inside a namespace (not a module), with the augmentation in the
    /// same namespace. Exercises the namespace branch of the closure compile-location.
    [<Theory; InlineData(true); InlineData(false)>]
    let ``Augmentation inner-rec on namespace-scoped type calls type-private`` (realsig: bool) =
        """namespace MyNs
type C() =
    static let mutable backing = 0
    static member Set v = backing <- v
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
