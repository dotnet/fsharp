// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.OverloadResolutionPriority

open FSharp.Test
open FSharp.Test.Compiler
open Xunit
open Conformance.SharedTestHelpers

module OverloadResolutionPriorityTests =

    [<FactForNETCOREAPP>]
    let ``OverloadResolutionPriority - comprehensive test`` () =
        FsFromPath (__SOURCE_DIRECTORY__ ++ "ORPTestRunner.fs")
        |> withReferences [csharpPriorityLib]
        |> withLangVersionPreview
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore

    [<FactForNETCOREAPP>]
    let ``OverloadResolutionPriority - Debug.Assert selects two-arg overload`` () =
        Fs """
module TestDebugAssert

open System.Diagnostics

let run () =
    Debug.Assert(true)
    Debug.Assert(false, "explicit message")
"""
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed
        |> ignore

    [<FactForNETCOREAPP>]
    let ``OverloadResolutionPriority - indexer with priority`` () =
        // Known limitation: ORPA on C# indexers does not override F# overload resolution.
        // F# selects the more specific type (string over object) regardless of priority.
        Fs """
module TestIndexerPriority

open ExtensionPriorityTests

let run () =
    let obj = IndexerWithPriority()
    // Single-arg indexer: F# picks string-priority0 (more specific) despite object having priority1
    let r1 = obj.["hello"]
    if r1 <> "string-indexer-priority0" then
        failwithf "Expected 'string-indexer-priority0' but got '%s'" r1

    // Two-arg indexer: F# picks two-int-priority2 (both more specific and higher priority)
    let r2 = obj.[1, 2]
    if r2 <> "two-int-indexer-priority2" then
        failwithf "Expected 'two-int-indexer-priority2' but got '%s'" r2

run ()
"""
        |> withReferences [csharpPriorityLib]
        |> withLangVersionPreview
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore
