namespace EmittedIL.RealInternalSignature

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

/// IL-structural regression locks for dotnet/fsharp#19933.
///
/// The runtime tests in `Regression_RealsigAugmentationClosure.fs` confirm the
/// programs no longer crash, but a runtime-only test is a weak guard: it would pass
/// even if the IL silently regressed (under `--realsig-` the closure can be a module
/// sibling and still work because the member is IL `assembly`). These tests pin the
/// fix's actual property under `--realsig+`: the synthesized closure is nested INSIDE
/// the declaring type (`Sample/C/<closure>@N`), NOT a sibling in the module class
/// (`Sample/<closure>@N`).
///
/// Each assertion below FAILS on the pre-fix/shipped compiler (which emits the module
/// sibling) and PASSES on the fixed compiler — so it genuinely guards the fix.
module Regression_RealsigAugmentationClosure_StructuralAssertions =

    /// Compile under --realsig+ --optimize+, assert the closure nests under the type
    /// (present) and is not a module sibling (absent), then run.
    let private assertNestedAndRun (present: string list) (absent: string list) source =
        let result =
            FSharp source
            |> withRealInternalSignature true
            |> asExe
            |> withOptimize
            |> ignoreWarnings
            |> compile
            |> shouldSucceed
        result |> verifyILPresent present
        result |> verifyILNotPresent absent
        result |> run |> shouldSucceed |> ignore

    [<Fact>]
    let ``Non-generic augmentation closure nests under the type`` () =
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
        |> assertNestedAndRun [ "Sample/C/h@" ] [ "Sample/h@" ]

    [<Fact>]
    let ``Generic augmentation closure nests under the generic type and threads the typar`` () =
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
        |> assertNestedAndRun [ "Sample/Holder`1/h@" ] [ "Sample/h@" ]

    [<Fact>]
    let ``Property-getter augmentation closure nests under the type`` () =
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
        |> assertNestedAndRun [ "Sample/C/h@" ] [ "Sample/h@" ]

    [<Fact>]
    let ``Indexer augmentation closure nests under the type`` () =
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
        |> assertNestedAndRun [ "Sample/C/h@" ] [ "Sample/h@" ]

    [<Fact>]
    let ``Operator augmentation closure nests under the type`` () =
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
        |> assertNestedAndRun [ "Sample/C/h@" ] [ "Sample/h@" ]

    [<Fact>]
    let ``Static-method augmentation closure nests under the type`` () =
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
        |> assertNestedAndRun [ "Sample/C/h@" ] [ "Sample/h@" ]

    [<Fact>]
    let ``Seq state-machine augmentation closure nests under the type`` () =
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
        |> assertNestedAndRun [ "Sample/C/Run@" ] [ "Sample/Run@" ]

    [<Fact>]
    let ``Two-type augmentations with same closure name nest under their own types`` () =
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
        |> assertNestedAndRun [ "Sample/Alpha`1/h@"; "Sample/Beta`1/'h@" ] [ "Sample/h@" ]
