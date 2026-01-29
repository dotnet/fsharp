// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// E2E tests for overload resolution caching correctness.
/// These tests verify that the CORRECT overload is picked, not just that compilation succeeds.
/// The strategy: compile and run code that returns indicators of which overload was chosen.
module FSharp.Compiler.ComponentTests.OverloadResolutionCacheE2ETests

open System.IO
open Xunit
open FSharp.Test.Compiler

[<Fact>]
let ``Overload resolution picks correct overloads and cache does not corrupt results`` () =
    FSharp (loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__, "OverloadResolutionBasicTests.fs")))
    |> withLangVersionPreview
    |> compileExeAndRun
    |> shouldSucceed
    |> verifyOutputContains [|"All 44 tests passed!"|]

[<Fact>]
let ``Adversarial tests: cache does not get poisoned by alternating types`` () =
    FSharp (loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__, "OverloadResolutionAdversarialTests.fs")))
    |> withLangVersionPreview
    |> compileExeAndRun
    |> shouldSucceed
    |> verifyOutputContains [|"All 23 adversarial tests passed!"|]
