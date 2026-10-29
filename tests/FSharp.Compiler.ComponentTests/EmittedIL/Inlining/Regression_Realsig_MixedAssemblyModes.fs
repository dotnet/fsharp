/// Purpose: cross-assembly REALSIG MODE MIXING. The existing RealInternalSignature suite
/// compiles the referenced library and the consumer with the SAME realsig value; there is no
/// test where a --realsig+ library is consumed by a --realsig- executable or vice-versa.
/// This file covers that, because homed fHats are type-scoped (private/internal-of-class)
/// IL artifacts whose *calling convention and accessibility* must not leak into what a
/// differently-flagged consumer can call.
///
/// Relevant variants: the (libRealsig, exeRealsig) pair is the axis under test -- all four
/// combinations. optimize is folded in as a third dimension rather than pinned, so every
/// (realsig-pair, optimize) cell is exercised; the cost is a few extra rows and the benefit
/// is that the accessibility boundary is checked in both codegen modes.
namespace EmittedIL.RealInternalSignature

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Regression_Realsig_MixedAssemblyModes =

    let private libSource =
        """
namespace Lib
type Cache<'T>() =
    member _.Run(seed: 'T) =
        let rec go (n: int) (acc: 'T) =
            if n <= 0 then acc
            elif n % 2 = 0 then skip (n - 1) acc
            else go (n - 1) acc
        and skip (n: int) (acc: 'T) =
            if n = 0 then acc else go (n - 1) acc
        go 1000 seed
"""

    let private exeSource =
        """
open Lib
[<EntryPoint>]
let main _ =
    if Cache<string>().Run("ok") = "ok" then 0 else 1
"""

    /// What: a generic class with a TLR inner-rec lives in a library and is called from an
    /// executable, for every (libRealsig, exeRealsig, optimize) combination, and runs to the
    /// right result.
    /// Why: the hoisted `go@`/`skip@` statics are internal to the library under both realsig
    /// modes; the PUBLIC `Run` member must remain callable from a consumer built with the
    /// opposite realsig flag. This guards the accessibility boundary of generated lifts the
    /// moment they become class-scoped (homed) members, and proves the public surface is
    /// unaffected by any internal re-shape.
    /// Breaks if: a homed member is emitted private-to-class in a way even the library's own
    /// public member cannot reach, or the public contract differs between the two modes.
    [<Theory; InlineData(true, true, true); InlineData(true, true, false); InlineData(true, false, true); InlineData(true, false, false); InlineData(false, true, true); InlineData(false, true, false); InlineData(false, false, true); InlineData(false, false, false)>]
    let ``TLR library is consumable across mixed realsig modes`` (libRealsig: bool, exeRealsig: bool, optimize: bool) =
        let lib =
            FSharp libSource
            |> withRealInternalSignature libRealsig
            |> asLibrary
            |> withOptimization optimize

        FSharp exeSource
        |> withReferences [ lib ]
        |> withRealInternalSignature exeRealsig
        |> withOptimization optimize
        |> compileExeAndRun
        |> shouldSucceed
        |> ignore