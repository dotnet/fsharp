// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Compiler.Service.Tests.CompilerGlobalStateTests

open FSharp.Compiler.CompilerGlobalState
open FSharp.Compiler.Text.Range
open FSharp.Test.Assert
open Xunit

[<Fact>]
let ``NiceNameGenerator drifts across calls and ResetCompilerGeneratedNameState restores a fresh-process layout`` () =
    let nng = NiceNameGenerator()
    let r = rangeN "niceNameGenerator.fs" 10

    // First batch: occurrence counters start from zero, so names drift f@10, f@10-1, f@10-2.
    let batch1 = [ for _ in 1 .. 3 -> nng.FreshCompilerGeneratedName("f", r) ]
    batch1 |> shouldEqual [ "f@10"; "f@10-1"; "f@10-2" ]

    // Without a reset, further calls keep drifting from where the counters left off.
    let keepsDriftingWithoutReset = [ for _ in 1 .. 2 -> nng.FreshCompilerGeneratedName("f", r) ]
    keepsDriftingWithoutReset |> shouldEqual [ "f@10-3"; "f@10-4" ]

    // Resetting clears the occurrence counters, so a subsequent run reproduces the very first batch.
    nng.ResetCompilerGeneratedNameState()
    let batch2 = [ for _ in 1 .. 3 -> nng.FreshCompilerGeneratedName("f", r) ]
    batch2 |> shouldEqual batch1

[<Fact>]
let ``StableNiceNameGenerator caches by uniq and ResetCompilerGeneratedNameState clears both the cache and the counters`` () =
    let gen = StableNiceNameGenerator()
    let r = rangeN "stableNiceNameGenerator.fs" 20

    // First occurrence of "h" for uniq 1.
    let first = gen.GetUniqueCompilerGeneratedName("h", r, 1L)
    first |> shouldEqual "h@20"

    // A different uniq for the same basic name advances the shared occurrence counter.
    let second = gen.GetUniqueCompilerGeneratedName("h", r, 2L)
    second |> shouldEqual "h@20-1"

    // Re-querying uniq 1 must return the cached name, not a recomputed (drifted) one, even though
    // the shared occurrence counter for "h" has since advanced to produce `second`.
    let cachedAgain = gen.GetUniqueCompilerGeneratedName("h", r, 1L)
    cachedAgain |> shouldEqual first

    gen.ResetCompilerGeneratedNameState()

    // Replaying the exact same sequence of calls after a reset reproduces the exact same names
    // ("h@20" then "h@20-1"), because both the stable-name cache and the shared occurrence
    // counter were cleared: a fresh call for uniq 1 is once again the first-ever occurrence.
    let afterResetForUniq1 = gen.GetUniqueCompilerGeneratedName("h", r, 1L)
    afterResetForUniq1 |> shouldEqual first

    let afterResetForUniq2 = gen.GetUniqueCompilerGeneratedName("h", r, 2L)
    afterResetForUniq2 |> shouldEqual second

    // The cache is fully functional again after reset: re-querying uniq 1 still returns the
    // cached (post-reset) name rather than drifting further.
    let cachedAgainAfterReset = gen.GetUniqueCompilerGeneratedName("h", r, 1L)
    cachedAgainAfterReset |> shouldEqual afterResetForUniq1

    // Prove the stable-name CACHE itself was cleared, not just the inner counters: after another
    // reset, the same (name, uniq) key queried with a DIFFERENT range must be recomputed from the
    // new range ("h@99"). A stale cache entry would instead return the pre-reset "h@20".
    gen.ResetCompilerGeneratedNameState()
    let differentRange = rangeN "stableNiceNameGenerator.fs" 99
    let recomputedForUniq1 = gen.GetUniqueCompilerGeneratedName("h", differentRange, 1L)
    recomputedForUniq1 |> shouldEqual "h@99"

[<Fact>]
let ``CompilerGlobalState.ResetCompilerGeneratedNameState resets all three generators together`` () =
    let state = CompilerGlobalState()
    let r = rangeN "compilerGlobalState.fs" 30

    let niceName1 = state.NiceNameGenerator.FreshCompilerGeneratedName("f", r)
    let ilxName1 = state.IlxGenNiceNameGenerator.FreshCompilerGeneratedName("g", r)
    let stableName1 = state.StableNameGenerator.GetUniqueCompilerGeneratedName("h", r, 1L)

    // Drift each generator away from its first-occurrence name before resetting.
    state.NiceNameGenerator.FreshCompilerGeneratedName("f", r) |> ignore
    state.IlxGenNiceNameGenerator.FreshCompilerGeneratedName("g", r) |> ignore
    state.StableNameGenerator.GetUniqueCompilerGeneratedName("h", r, 2L) |> ignore

    state.ResetCompilerGeneratedNameState()

    let niceName2 = state.NiceNameGenerator.FreshCompilerGeneratedName("f", r)
    let ilxName2 = state.IlxGenNiceNameGenerator.FreshCompilerGeneratedName("g", r)
    // Replaying the same first call (uniq 1) after the aggregate reset reproduces the original
    // stable name, confirming the reset reached the StableNameGenerator too. (StableNiceNameGenerator's
    // own tests separately confirm that the reset actually clears its cache, rather than merely
    // resetting the shared occurrence counter.)
    let stableName2 = state.StableNameGenerator.GetUniqueCompilerGeneratedName("h", r, 1L)

    niceName2 |> shouldEqual niceName1
    ilxName2 |> shouldEqual ilxName1
    stableName2 |> shouldEqual stableName1
