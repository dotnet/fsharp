/// Purpose: cover sequence / task / async state-machine generators declared as members of
/// GENERIC classes. Each generator compiles to a nested state-machine type; class tyvars
/// must be threaded into that generated type correctly and the state machine must stay
/// scoped inside its declaring class (never a module sibling), which is the same scope
/// rule the closures in Regression_RealsigAugmentationClosure.fs enforce.
///
/// Existing coverage is limited to seq/task/async in NON-generic augmentation members
/// (#19955); generic-class members (body-declared AND generic methods) are the gap here.
///
/// Variant policy: runtime = 4-way matrix; IL = realsig +/- under --optimize+ only (the
/// nested-state-machine token is an optimize+ codegen artifact). No IL baselines are
/// generated; the nested/absent fragment pairs below pin only the *scope*, which is the
/// invariant that matters.
namespace EmittedIL.RealInternalSignature

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Regression_TLR_StateMachineGenerators =

    let private compileWithFlags realsig optimize source =
        FSharp source
        |> withRealInternalSignature realsig
        |> asExe
        |> withOptimization optimize
        |> ignoreWarnings

    /// What: seq/task/async/seq-in-generic-method generators on a generic class run to the
    /// correct values under every realsig/optimize combination.
    /// Why: the generators capture the class tyvar 'T, so any tyvar or closure-scope slip in
    /// the generated state-machine class produces either a type error or a runtime failure.
    /// Breaks if: the state machine is emitted in a scope that cannot reach the enclosing
    /// type's members/tyvars, or the tyvar is dropped from the generated type entirely.

    /// What: IL scope lock -- the generated seq/task/async state machines must be nested
    /// inside the generic class (`Holder`1/Seq@`, `/*/Gen@`, `/*/Task@`, `/*/Async@`) and
    /// never appear as module-class siblings.
    /// Why: a sibling state machine that closes over the class tyvar or a type-private
    /// member is unverifiable or throws at first use (the #19933 failure mode); nesting is
    /// the contract.
    /// Breaks if: the state machine is hoisted out of the class. Manual .il.bsl capture for
    /// both realsig values belongs beside this file once the homing change lands.
    [<Theory; InlineData(true, true); InlineData(true, false); InlineData(false, true); InlineData(false, false)>]
    let ``Seq, task and async generators on a generic class run`` (realsig: bool, optimize: bool) =
        let src =
            """
module Sample
open System.Threading.Tasks
type Holder<'T>(initial: 'T) =
    member _.Seq() = seq { yield initial }
    member _.Gen<'U>(u: 'U) = seq { yield u }
    member _.Task() : Task<int> = task { return 42 }
    member _.Async() = async { return initial }

[<EntryPoint>]
let main _ =
    let h = Holder(7)
    let s = h.Seq() |> Seq.head
    let g = h.Gen<string>("x") |> Seq.head
    let t = h.Task().Result
    let a = h.Async() |> Async.RunSynchronously
    if s = 7 && g = "x" && t = 42 && a = 7 then 0 else 1
"""
        let (present, absent) =
            match realsig, optimize with
            | false, _ -> [ "Sample/Seq@"; "Sample/Gen@"; "Sample/Task@"; "Sample/Async@" ], [ "Holder`1/Seq@"; "Holder`1/Gen@"; "Holder`1/Task@"; "Holder`1/Async@" ]
            | true, _ -> [ "Holder`1/Seq@"; "Holder`1/Gen@"; "Holder`1/Task@"; "Holder`1/Async@" ], [ "Sample/Seq@"; "Sample/Gen@"; "Sample/Task@"; "Sample/Async@" ]

        let result =
            src
            |> compileWithFlags realsig optimize
            |> compileAndRun
            |> shouldSucceed
            |> verifyPEFileWithSystemDlls
        result |> verifyILPresent present
        result |> verifyILNotPresent absent
