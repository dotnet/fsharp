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

    [<FactForNETCOREAPP>]
    let ``ORPA - inapplicable high-priority does not shadow applicable low-priority`` () =
        Fs """
module T
open System.Runtime.CompilerServices
type C() =
    [<OverloadResolutionPriority(1)>] member _.M(s: string) = "string"
    member _.M(i: int) = "int"
let r = C().M(42)
if r <> "int" then failwithf "expected int, got %s" r
"""
        |> withLangVersionPreview
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore

    [<FactForNETCOREAPP>]
    let ``ORPA - high-priority params beats low-priority exact`` () =
        Fs """
module T
open PriorityTests
let r = ParamsPriority.M1(1)
if r <> "params" then failwithf "expected params, got %s" r
"""
        |> withReferences [csharpPriorityLib]
        |> withLangVersionPreview
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore

    [<FactForNETCOREAPP>]
    let ``ORPA - high-priority optional-arg overload beats low-priority exact`` () =
        // Exact-match must not pre-empt priority: M(int) is the *exact* match for M(1), but the
        // higher-priority M(int, ?int) overload (non-exact, optional omitted) must win.
        Fs """
module T
open System.Runtime.CompilerServices
type C() =
    [<OverloadResolutionPriority(1)>] member _.M(x: int, ?y: int) = "opt-high"
    member _.M(x: int) = "plain-low"
let r = C().M(1)
if r <> "opt-high" then failwithf "expected opt-high, got %s" r
"""
        |> withLangVersionPreview
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore

    [<FactForNETCOREAPP>]
    let ``ORPA - no applicable overload preserves normal diagnostics`` () =
        // Priority is pruned only among *applicable* members; when none applies the full set is kept so
        // the "no overloads found" diagnostic lists every overload (pre-fix the high-priority string overload
        // was kept before applicability and gave a bare type-mismatch instead of the overload listing).
        Fs """
module T
open System.Runtime.CompilerServices
type C() =
    [<OverloadResolutionPriority(1)>] member _.M(s: string) = "string"
    member _.M(b: bool) = "bool"
let r = C().M(42)
"""
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withErrorCode 41
        |> withDiagnosticMessageMatches "bool"
        |> withDiagnosticMessageMatches "string"
        |> ignore

    [<FactForNETCOREAPP>]
    let ``ORPA - equal priority stays ambiguous when concreteness is incomparable`` () =
        // Both overloads share priority 1, so the group keeps both; ordinary betterness then finds
        // them incomparable (each more concrete in one position) and the call stays ambiguous.
        // Guards that priority pruning does not arbitrarily pick a survivor among equal priorities.
        Fs """
module T
open System.Runtime.CompilerServices
type C() =
    [<OverloadResolutionPriority(1)>] member _.M(x: int, y: obj) = "int-obj"
    [<OverloadResolutionPriority(1)>] member _.M(x: obj, y: int) = "obj-int"
let r = C().M(1, 1)
"""
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withErrorCode 41
        |> ignore

    [<FactForNETCOREAPP>]
    let ``ORPA - same extension class priority beats exact overload`` () =
        // Complement of the cross-class H6/H7 tests: two extension methods in the *same* static
        // class DO have their priorities compared, so the high-priority object overload wins over
        // the exact int overload.
        Fs """
module T
open SameClassExtensionPriority
let r = "receiver".Pick(42)
if r <> "high-obj" then failwithf "expected high-obj, got %s" r
"""
        |> withReferences [csharpPriorityLib]
        |> withLangVersionPreview
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore

    [<FactForNETCOREAPP>]
    let ``ORPA - priority is not compared across extension classes (concreteness decides)`` () =
        // C#-parity: OverloadResolutionPriority is scoped per containing type. Two extension
        // methods on System.Guid declared in *different* static classes must not have their
        // priorities compared; ordinary betterness (concreteness) picks the more specific int.
        Fs """
module T
open H6Observable
let g = System.Guid.NewGuid()
let r = g.Pick(42)
if r <> "low-int" then failwithf "expected low-int, got %s" r
"""
        |> withReferences [csharpPriorityLib]
        |> withLangVersionPreview
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore

    [<FactForNETCOREAPP>]
    let ``ORPA - naked-generic-receiver priority is not compared across extension classes`` () =
        // Both extensions have a generic receiver (this T) that instantiates to int here, so
        // the buggy "group by extended type" logic puts them in the same (int) bucket and lets
        // the high-priority object overload suppress the int one across the two static classes.
        // C#-parity groups by static class, so concreteness picks the int overload.
        Fs """
module T
open H7GenericReceiver
let r = (42).Pick(7)
if r <> "low-generic-int" then failwithf "expected low-generic-int, got %s" r
"""
        |> withReferences [csharpPriorityLib]
        |> withLangVersionPreview
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore

    [<FactForNETCOREAPP>]
    let ``ORPA - override uses least-derived base priority`` () =
        Fs """
module T
open ExtensionPriorityTests
let d = DerivedOverridesVirtual()
let r = d.Compute("hello")
if r <> "derived-object" then failwithf "expected derived-object, got %s" r
"""
        |> withReferences [csharpPriorityLib]
        |> withLangVersionPreview
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore

    [<FactForNETCOREAPP>]
    let ``ORPA - equal priority resolves by concreteness`` () =
        Fs """
module T
open System.Runtime.CompilerServices
type C() =
    [<OverloadResolutionPriority(1)>] member _.M(x: obj) = "obj"
    [<OverloadResolutionPriority(1)>] member _.M(x: string) = "string"
let r = C().M("hi")
if r <> "string" then failwithf "got %s" r
"""
        |> withLangVersionPreview
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore
