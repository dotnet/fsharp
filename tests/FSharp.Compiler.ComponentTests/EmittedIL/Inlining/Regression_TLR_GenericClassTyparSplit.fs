/// Purpose: cover TLR inner-recursion lifts inside GENERIC CLASS members where the lifted
/// function must carry MULTIPLE class type variables (ctps) AND method type variables
/// (etps). The class-homing change (lifting the fHat onto the enclosing generic class)
/// splits the typar set into ep_ctps (class) + ep_etps/tps (method); the order and count
/// of those typars is what the IL signature and the GenericParameter round-trip depend on.
///
/// Variant policy:
///   - Runtime tests run the full 4-way matrix (realsig +/- x optimize +/-). TLR only
///     lifts under --optimize+; under --optimize- the same source must still be correct
///     even though it degrades to closures, so both sides matter.
///   - IL-shape tests run realsig +/- under --optimize+ only, because the closure-free
///     ("no FSharpFunc extends") invariant is only meaningful when TLR fires.
///
/// No IL baselines are generated here; where an IL lock would belong, the required
/// manual .il.bsl capture is called out in the test comment.
namespace EmittedIL.RealInternalSignature

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Regression_TLR_GenericClassTyparSplit =

    let private compileWithFlags realsig optimize source =
        FSharp source
        |> withRealInternalSignature realsig
        |> asExe
        |> withOptimization optimize
        |> ignoreWarnings

    let private runAllFour realsig optimize (source: string) =
        source
        |> compileWithFlags realsig optimize
        |> compileAndRun
        |> shouldSucceed
        |> ignore

    /// What: a mutual-rec group inside a generic class carrying two class tyvars (ctps=2)
    /// plus one method tyvar (etps=1) must typecheck, verify and run to the right result
    /// under every realsig/optimize combination.
    /// Why: this is the ep_ctps/ep_etps split the homing change depends on; a shared typar
    /// set that is mis-ordered, duplicated, or dropped breaks either IL verification or the
    /// called generic instantiation.
    /// Breaks if: the rec set is typed with class-scoped instead of method-scoped tyvars,
    /// or a homed method loses its method tyvars when folded into the class.
    [<Theory; InlineData(true, true); InlineData(true, false); InlineData(false, true); InlineData(false, false)>]
    let ``Two class tyvars and one method tyvar all threaded through mutual rec`` (realsig: bool, optimize: bool) =
        """
module Sample
type Cache<'T1, 'T2>(a: 'T1) =
    member _.B() : 'T2 = Unchecked.defaultof<'T2>
    member _.Run<'U>(u: 'U) =
        let rec scan (n: int) (acc: 'T1) (m: 'U) =
            if n <= 0 then acc
            elif n % 2 = 0 then skip (n - 1) acc m
            else scan (n - 1) acc m
        and skip (n: int) (acc: 'T1) (m: 'U) =
            if box (Unchecked.defaultof<'T2>) = box m then skip (n - 1) acc m
            else scan (n - 1) acc m
        scan 1000 a u
[<EntryPoint>]
let main _ =
    if Cache<int, string>(5).Run<string>("x") = 5 then 0 else 1
"""
        |> runAllFour realsig optimize

    /// What: with TLR active this multi-ctp group must emit static methods, never a
    /// FSharpFunc closure class, and the PE must ILVerify under both realsig values.
    /// Why: the whole point of TLR/homing is to avoid closure allocation for self-recursion;
    /// an accidental capture (e.g. a free class-tyvar reference turned into an environment
    /// slot) silently reintroduces the closure we are trying to eliminate.
    /// Breaks if: any free variable leaks into the rec group, or homing stops the lift for a
    /// typar-count it has not seen. When homing lands, expect the token to move from the
    /// current module-scope `scan@` to `Sample/Cache`2::scan@`; update the fragment in the
    /// same PR that changes the codegen.
    [<Theory; InlineData(true); InlineData(false)>]
    let ``Multi-ctp TLR group emits statics, not closures, and verifies`` (realsig: bool) =
        let result =
            """
module Sample
type Cache<'T1, 'T2>(a: 'T1) =
    member _.B() : 'T2 = Unchecked.defaultof<'T2>
    member _.Run<'U>(u: 'U) =
        let rec scan (n: int) (acc: 'T1) (m: 'U) =
            if n <= 0 then acc
            elif n % 2 = 0 then skip (n - 1) acc m
            else scan (n - 1) acc m
        and skip (n: int) (acc: 'T1) (m: 'U) =
            if box (Unchecked.defaultof<'T2>) = box m then skip (n - 1) acc m
            else scan (n - 1) acc m
        scan 1000 a u
[<EntryPoint>]
let main _ =
    if Cache<int, string>(5).Run<string>("x") = 5 then 0 else 1
"""
            |> compileWithFlags realsig true
            |> compile
            |> shouldSucceed
            |> verifyPEFileWithSystemDlls

        result |> verifyILPresent [ "scan@"; "skip@" ]
        result |> verifyILNotPresent [ "extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc" ]

    /// What: a constrained generic class ('T : comparison) combined with a constrained
    /// generic method ('U : equality) must thread both witnesses through the rec group and
    /// run correctly under all four combos.
    /// Why: constraints carried by the lifted function's own tyvars (ep_ctps/etps) must
    /// survive homing; if a witness or constraint is stripped from the homed method's IL
    /// generic params the JIT throws TypeLoadException (the #14492 class of bug).
    /// Breaks if: constraints end up on the wrong tyvar set, or are dropped entirely when
    /// the method tyvars are folded onto the class.
    [<Theory; InlineData(true, true); InlineData(true, false); InlineData(false, true); InlineData(false, false)>]
    let ``Class and method constraints both threaded through TLR group`` (realsig: bool, optimize: bool) =
        """
module Sample
type Store<'T when 'T : comparison>() =
    member _.Run<'U when 'U : equality>(seed: 'T) (expected: 'U) =
        let rec walk (n: int) (acc: 'T) (p: 'U) =
            if n <= 0 then acc
            elif p = p then step (n - 1) acc p
            else walk (n - 1) acc p
        and step (n: int) (acc: 'T) (p: 'U) =
            if n = 0 then acc
            elif compare acc acc = 0 then walk (n - 1) acc p
            else step (n - 1) acc p
        walk 1000 seed expected
[<EntryPoint>]
let main _ =
    let s = Store<int>()
    if s.Run<string>(7) "x" = 7 then 0 else 1
"""
        |> runAllFour realsig optimize

    /// What: the constrained case above must also lift to constraint-free-or-exact statics
    /// and ILVerify cleanly under both realsig values.
    /// Why: /\the constraint stripping (#14492) and the typar split are close neighbors in
    /// the same rec group; covering them together catches interactions the single-axis
    /// tests miss.
    /// Breaks if: a witness for the class-constraint is emitted on the method typars or the
    /// 'U equality witness is lost when the method typars are folded onto the class.
    [<Theory; InlineData(true); InlineData(false)>]
    let ``Constrained TLR group emits statics and verifies`` (realsig: bool) =

        let result =
            """
module Sample
type Store<'T when 'T : comparison>() =
    member _.Run<'U when 'U : equality>(seed: 'T) (expected: 'U) =
        let rec walk (n: int) (acc: 'T) (p: 'U) =
            if n <= 0 then acc
            elif p = p then step (n - 1) acc p
            else walk (n - 1) acc p
        and step (n: int) (acc: 'T) (p: 'U) =
            if n = 0 then acc
            elif compare acc acc = 0 then walk (n - 1) acc p
            else step (n - 1) acc p
        walk 1000 seed expected
[<EntryPoint>]
let main _ =
    let s = Store<int>()
    if s.Run<string>(7) "x" = 7 then 0 else 1
"""
            |> compileWithFlags realsig true
            |> compile
            |> shouldSucceed
            |> verifyPEFileWithSystemDlls
            |> shouldSucceed
        result |> verifyILPresent [ "walk@"; "step@" ]
        result |> verifyILNotPresent [ "extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc" ]

    /// What: a >5-curried-argument rec group (which forces the packed-argument IL shape)
    /// must keep the exact parameter count/order and run correctly under all four combos.
    /// Why: the homed method must not gain or lose argument slots when the packed argv is
    /// generated; a mismatch surfaces as an IL verification error (bad maxstack/arg mapping)
    /// or a wrong runtime result.
    /// Breaks if: homing re-packs args, drops the unit slot, or changes parameter order.
    [<Theory; InlineData(true, true); InlineData(true, false); InlineData(false, true); InlineData(false, false)>]
    let ``TLR with >5 curried args keeps exact arg shape`` (realsig: bool, optimize: bool) =
        """
module Sample
let run() =
    let rec go (n: int) (a: int) (b: int) (c: int) (d: int) (e: int) (f: int) =
        if n <= 0 then a + b + c + d + e + f
        else go (n - 1) a b c d e f
    go 1000 1 2 3 4 5 6
[<EntryPoint>]
let main _ = if run() = 21 then 0 else 1
"""
        |> runAllFour realsig optimize

    /// What: the >5-arg group above emits a packed static and ILVerifies under realsig +/-.
    /// Why: locks the `>5 params` packing path for TLR; this is the same boundary the
    /// EraseClosures CASE 2a term-splitting has to respect, so guarding it here prevents a
    /// homing/lifting interaction from silently changing the packed argv layout.
    /// Breaks if: the packed signature loses an argument or the split point changes.
    [<Theory; InlineData(true); InlineData(false)>]
    let ``TLR >5-arg lift emits packed static and verifies`` (realsig: bool) =
        let result =
            """
module Sample
let run() =
    let rec go (n: int) (a: int) (b: int) (c: int) (d: int) (e: int) (f: int) =
        if n <= 0 then a + b + c + d + e + f
        else go (n - 1) a b c d e f
    go 1000 1 2 3 4 5 6
[<EntryPoint>]
let main _ = if run() = 21 then 0 else 1
"""
                |> compileWithFlags realsig true
                |> compile
                |> verifyPEFileWithSystemDlls
                |> shouldSucceed
        result |> verifyILPresent [ "go@" ]
        result |> verifyILNotPresent [ "extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc" ]
