/// Purpose: cover the remaining generic-TLR scope shapes that the flat-routing tests do not
/// touch: (1) a generic class inside a NESTED MODULE (homing must pick the class, not the
/// module, as the lift's home), (2) value-recursion (`let rec x = ... and f = ...`) inside a
/// generic class member, and (3) same-named inner-rec lifts defined in separately compiled
/// namespace files must not collide when the housing types are generic.
///
/// Variant policy: runtime = 4-way matrix; IL = realsig +/- under --optimize+. No IL
/// baselines are generated here.
namespace EmittedIL.RealInternalSignature

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Regression_TLR_NestedScopesAndNameCollisions =

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

    /// What: a generic class declared inside a nested module whose member carries a mutual rec
    /// and runs correctly under all four combos.
    /// Why: with class-homing the lift's home must be the generic class regardless of how deep
    /// the module scope is; homing to the module instead would reintroduce the flattened
    /// mutation 'T as a method typar and change IL genericity.
    /// Breaks if: the home-resolving logic picks a non-generic enclosing module over the
    /// generic class, or loses a tyvar when crossing the module boundary.
    [<Theory; InlineData(true, true); InlineData(true, false); InlineData(false, true); InlineData(false, false)>]
    let ``Generic class inside a nested module TLRs correctly`` (realsig: bool, optimize: bool) =
        """
module Top
module Inner =
    type Box<'T>(seed: 'T) =
        member _.Run(seed: 'T) =
            let rec go (n: int) (acc: 'T) =
                if n <= 0 then acc
                elif n % 2 = 0 then again (n - 1) acc
                else go (n - 1) acc
            and again (n: int) (acc: 'T) =
                if n = 0 then acc else go (n - 1) acc
            go 1000 seed
[<EntryPoint>]
let main _ =
    if Inner.Box("x").Run("x") = "x" then 0 else 1
"""
        |> runAllFour realsig optimize

    /// What: IL lock -- the nested-module case still emits statics (no closure) and verifies.
    /// Why: the deep module scope must not degrade the lift into a closure.
    /// Breaks if: the lift is dropped (closure emitted) because the scope lookup failed. When
    /// homing lands the token moves from the current `Top/Inner::go@` flat form to a
    /// `Top/Inner/Box`1::go@` member form -- update this fragment in the same PR.
    [<Theory; InlineData(true); InlineData(false)>]
    let ``Nested-module generic class lift is static and verifies`` (realsig: bool) =
        let result =
            """
module Top
module Inner =
    type Box<'T>(seed: 'T) =
        member _.Run(seed: 'T) =
            let rec go (n: int) (acc: 'T) =
                if n <= 0 then acc
                elif n % 2 = 0 then again (n - 1) acc
                else go (n - 1) acc
            and again (n: int) (acc: 'T) =
                if n = 0 then acc else go (n - 1) acc
            go 1000 seed
[<EntryPoint>]
let main _ =
    if Inner.Box("x").Run("x") = "x" then 0 else 1
"""
            |> compileWithFlags realsig true
            |> compile
            |> verifyPEFileWithSystemDlls
            |> shouldSucceed
        result |> verifyILPresent [ "go@"; "again@" ]
        result |> verifyILNotPresent [ "extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc" ]

    /// What: a VALUE-recursion chain (`let rec values = ... and read = ...`) inside a generic
    /// class member runs correctly under all four combos.
    /// Why: value-rec bindings share the `and` chain with function bindings; TLR must leave
    /// the value binding alone and not knock the chain out of scope order (value-binding init
    /// before use) when the housing class is generic.
    /// Breaks if: the value binding is misordered or re-lifted, producing a null/default read
        /// or an initialization-order runtime error.
    [<Theory; InlineData(true, true); InlineData(true, false); InlineData(false, true); InlineData(false, false)>]
    let ``Value recursion inside a generic class member runs`` (realsig: bool, optimize: bool) =
        """
module Sample
type Store<'T>(seed: 'T) =
    member _.Run() =
        let rec cache = [| seed |]
        and read () = if cache.Length = 1 then cache.[0] else Unchecked.defaultof<'T>
        read ()
[<EntryPoint>]
let main _ =
    if Store("k").Run() = "k" then 0 else 1
"""
        |> runAllFour realsig optimize

    /// What: two separate namespace-scoped source files each define a generic class running an
    /// inner-rec whose generated name collides (`go@`), and the combined library must compile
    /// without a PrivateImplementationDetails collision under all four combos.
    /// Why: this extends the existing non-generic namespace-collision regression (IlxGen
    /// AllocValReprWithinExpr, FS2014) to GENERIC housing types, where homing multiplies the
    /// candidate scopes for generated names.
    /// Breaks if: generated member names collide across files (duplicate nested type or
    /// static) or land in a shared hidden implementation-class.
    [<Theory; InlineData(true, true); InlineData(true, false); InlineData(false, true); InlineData(false, false)>]
    let ``Same-named lifts in two generic classes across namespace files do not collide`` (realsig: bool, optimize: bool) =
        let src1 =
            "namespace Mine\ntype Alpha<'T>() =\n    static member Run() =\n        let rec go x = if x = 0 then 0 else go (x - 1)\n        go 10\n"
        let src2 =
            "namespace Mine\ntype Beta<'U>() =\n    static member Run() =\n        let rec go y = if y = 0 then 1 else go (y - 1)\n        go 20\n"
        FSharp src1
        |> withAdditionalSourceFile (SourceCodeFileKind.Create("B.fs", src2))
        |> withRealInternalSignature realsig
        |> asLibrary
        |> withOptimization optimize
        |> compile
        |> shouldSucceed
        |> verifyILNotPresent [ "PrivateImplementationDetails" ]
        |> ignore