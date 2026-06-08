namespace EmittedIL.RealInternalSignature

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

/// Runtime regression smoke tests for the interaction between this PR's TLR routing
/// (lifting inner-rec functions to module/init-class statics under `--realsig+`) and
/// source-level `private` visibility (which realsig+ emits as IL `private` rather
/// than IL `assembly` as realsig- does).
///
/// Each test exercises a hypothetical access-check risk: a TLR-lifted helper that
/// touches source-`private` data living in a different IL container than where the
/// helper lands. Failure mode would be `MethodAccessException` / `FieldAccessException`
/// / `TypeAccessException` at first invocation. Tests run under both realsig settings
/// so a regression in either path is caught.
module Regression_TLR_RealsigPrivate =

    open Regression_TLR_MutualInnerRec_StructuralAssertions

    let private compileRunSucceeds realsig source =
        source |> compileOptimized realsig |> compileExeAndRun |> shouldSucceed |> ignore

    /// Module-private value + TLR-lifted inner-rec at module level.
    /// Lifted helper lives in the same module class as `secret`, so access should hold.
    [<Theory; InlineData(true); InlineData(false)>]
    let ``Module-private value accessed from TLR-lifted inner-rec`` (realsig: bool) =
        """module Sample
let private secret = 42
let outer () =
    let rec h n = if n = 0 then secret else h (n - 1)
    h 10
[<EntryPoint>]
let main _ = if outer() = 42 then 0 else 1
"""
        |> compileRunSucceeds realsig

    /// Type-private static accessed from inner-rec inside a member of the same class.
    /// Under realsig+, `C.Secret` is IL-private to `C`; the lifted helper is routed to
    /// the per-file InitClass (different IL type). Demonstrates routing must preserve
    /// access — would `MethodAccessException` if the lift's IL accessibility were wrong.
    [<Theory; InlineData(true); InlineData(false)>]
    let ``Type-private static accessed from TLR-lifted inner-rec inside same type`` (realsig: bool) =
        """module Sample
type C() =
    static member private Secret = 42
    static member Run() =
        let rec h n = if n = 0 then C.Secret else h (n - 1)
        h 10
[<EntryPoint>]
let main _ = if C.Run() = 42 then 0 else 1
"""
        |> compileRunSucceeds realsig

    /// Private DU + structural compare (Match01-shape). The compiler-generated
    /// `CompareTo$cont` continuation is lifted out of the DU type to the module class;
    /// case fields are source-private. Would `FieldAccessException` if visibility leaked.
    [<Theory; InlineData(true); InlineData(false)>]
    let ``Private DU structural compare via TLR-lifted continuation`` (realsig: bool) =
        """module Sample
type private DU = A of int | B of int
[<EntryPoint>]
let main _ =
    let cmp (x: DU) (y: DU) = compare x y
    if cmp (A 1) (A 1) = 0 && cmp (A 1) (B 1) <> 0 then 0 else 1
"""
        |> compileRunSucceeds realsig

    /// Generic + private nested type captured by a TLR-lifted inner-rec.
    /// Lifted helper's signature mentions the private generic type — would
    /// `TypeAccessException` on JIT/load if visibility were not preserved.
    [<Theory; InlineData(true); InlineData(false)>]
    let ``Generic + private nested type captured by TLR-lifted inner-rec`` (realsig: bool) =
        """module Sample
type private Box<'T> = { mutable v: 'T }
[<EntryPoint>]
let main _ =
    let outer (b: Box<int>) =
        let rec h n = if n = 0 then b.v else h (n - 1)
        h 10
    let b = { v = 42 }
    if outer b = 42 then 0 else 1
"""
        |> compileRunSucceeds realsig
