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

    [<FactForNETCOREAPP>]
    let ``OverloadResolutionPriority - error on F# override`` () =
        Fs """
module TestORPOnOverride

open System.Runtime.CompilerServices

type Base() =
    abstract member DoWork: int -> string
    default _.DoWork(x: int) = "base"

    abstract member DoWork: string -> string
    default _.DoWork(s: string) = "base-string"

type Derived() =
    inherit Base()

    [<OverloadResolutionPriority(1)>]
    override _.DoWork(x: int) = "derived"
"""
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withErrorCode 3586
        |> ignore

    [<FactForNETCOREAPP>]
    let ``OverloadResolutionPriority - allowed on non-override F# member`` () =
        Fs """
module TestORPOnNonOverride

open System.Runtime.CompilerServices

type MyClass() =
    [<OverloadResolutionPriority(1)>]
    member _.Work(x: obj) = "obj"

    member _.Work(x: string) = "string"

let result = MyClass().Work("hello")
"""
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed
        |> ignore
