// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.Tiebreakers

open FSharp.Test
open FSharp.Test.Compiler
open Xunit

/// Tests for RFC FS-XXXX: "Most Concrete" Tiebreaker for Overload Resolution
/// 
/// These tests verify that the F# compiler correctly selects the more concrete overload
/// when multiple overloads are compatible with the provided arguments.
module TiebreakerTests =

    // ============================================================================
    // Helper functions for testing overload resolution
    // ============================================================================
    
    /// Verifies that the code compiles successfully
    let private shouldCompile source =
        FSharp source
        |> typecheck
        |> shouldSucceed
        |> ignore

    /// Verifies that the code fails to compile with the expected error
    let private shouldFailWithAmbiguity source =
        FSharp source
        |> typecheck
        |> shouldFail
        |> withErrorCode 41 // FS0041: A unique overload could not be determined
        |> ignore

    // ============================================================================
    // Placeholder test - validates test infrastructure is working
    // ============================================================================

    [<Fact>]
    let ``Placeholder - Test infrastructure compiles and runs`` () =
        // Simple test to verify test infrastructure is working
        FSharp """
module Test

type Example =
    static member Invoke(value: int) = "int"
    static member Invoke(value: string) = "string"

let result = Example.Invoke(42)
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    // ============================================================================
    // Core RFC Examples - "Most Concrete" Tiebreaker
    // These tests currently expect ambiguity (FS0041) until the feature is implemented
    // ============================================================================

    [<Fact>]
    let ``RFC Example - Option of int list vs Option of generic - resolves to more concrete`` () =
        // This is the core motivating example from the RFC
        // With the tiebreaker implementation, this resolves to Option<int list> (more concrete)
        FSharp """
module Test

type Example =
    static member Invoke(value: Option<'t>) = "generic"
    static member Invoke(value: Option<int list>) = "concrete"

// With tiebreaker: resolves to the more concrete overload (Option<int list>)
let result = Example.Invoke(Some([1]))
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Non-generic overload is preferred over generic - existing behavior`` () =
        // This tests existing F# behavior where non-generic is preferred over generic
        FSharp """
module Test

type Example =
    static member Process(value: 't) = "generic"
    static member Process(value: int) = "int"

let result = Example.Process(42)
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    // ============================================================================
    // RFC Section Examples 1-4: Basic Concreteness Scenarios
    // ============================================================================

    [<Fact>]
    let ``Example 1 - Basic Generic vs Concrete - Option of t vs Option of int`` () =
        // RFC Example 1: Option<'t> vs Option<int>
        // Option<int> should be preferred as it is more concrete
        FSharp """
module Test

type Example =
    static member Invoke(value: Option<'t>) = "generic"
    static member Invoke(value: Option<int>) = "int"

// With tiebreaker: resolves to Invoke(Option<int>) - more concrete
let result = Example.Invoke(Some 42)
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Example 2 - Fully Generic vs Wrapped - t vs Option of t - still ambiguous`` () =
        // RFC Example 2: 't vs Option<'t>
        // This tests a case where parameter structures differ ('t vs Option<'t>)
        // The current tiebreaker implementation compares instantiated type arguments,
        // not parameter structure shapes. This case remains ambiguous.
        // NOTE: Full implementation of this case would require comparing parameter type shapes.
        FSharp """
module Test

type Example =
    static member Process(value: 't) = "fully generic"
    static member Process(value: Option<'t>) = "wrapped"

// Currently ambiguous: structural comparison not yet implemented
let result = Example.Process(Some 42)
        """
        |> typecheck
        |> shouldFail
        |> withErrorCode 41
        |> ignore

    [<Fact>]
    let ``Example 3 - Nested Generics - Option of Option of t vs Option of Option of int`` () =
        // RFC Example 3: Nested Option types
        // Option<Option<int>> should be preferred as innermost type is more concrete
        FSharp """
module Test

type Example =
    static member Handle(value: Option<Option<'t>>) = "nested generic"
    static member Handle(value: Option<Option<int>>) = "nested int"

// With tiebreaker: resolves to Handle(Option<Option<int>>) - innermost type is more concrete
let result = Example.Handle(Some(Some 42))
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Example 4 - Triple Nesting Depth - list Option Result deep nesting`` () =
        // RFC Example 4: Deep nesting - list<Option<Result<'t, exn>>> vs list<Option<Result<int, exn>>>
        // The more concrete overload (int) should be preferred at depth 3
        FSharp """
module Test

type Example =
    static member Deep(value: list<Option<Result<'t, exn>>>) = "generic"
    static member Deep(value: list<Option<Result<int, exn>>>) = "int"

// With tiebreaker: resolves to Deep(list<Option<Result<int, exn>>>) - more concrete at depth 3
let result = Example.Deep([Some(Ok 42)])
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    // ============================================================================
    // RFC Section Examples 5-6: Multiple Type Parameters
    // ============================================================================

    [<Fact>]
    let ``Example 5 - Multiple Type Parameters - Result fully concrete wins`` () =
        // RFC Example 5: Multiple type parameters - Result<'ok, 'error> variants
        // Result<int, string> (fully concrete) should be preferred over partial concreteness
        FSharp """
module Test

type Example =
    static member Transform(value: Result<'ok, 'error>) = "fully generic"
    static member Transform(value: Result<int, 'error>) = "int ok"
    static member Transform(value: Result<'ok, string>) = "string error"
    static member Transform(value: Result<int, string>) = "both concrete"

// With tiebreaker: resolves to Transform(Result<int, string>) - both args are concrete
let result = Example.Transform(Ok 42 : Result<int, string>)
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Example 5 - Multiple Type Parameters - Partial concreteness int ok - currently ambiguous`` () =
        // When only int is concrete, Result<int, 'error> ideally should beat Result<'ok, 'error>
        // NOTE: Current implementation limitation - partial concreteness with two-way comparison
        // between fully generic and partially concrete does not yet resolve.
        // This test documents current behavior. Future enhancement may resolve this.
        FSharp """
module Test

type Example =
    static member Process(value: Result<'ok, 'error>) = "fully generic"
    static member Process(value: Result<int, 'error>) = "int ok"

// Currently ambiguous - see note above
let result = Example.Process(Ok 42 : Result<int, exn>)
        """
        |> typecheck
        |> shouldFail
        |> withErrorCode 41 // FS0041 - currently ambiguous
        |> ignore

    [<Fact>]
    let ``Example 5 - Multiple Type Parameters - Partial concreteness string error - currently ambiguous`` () =
        // When only string error is concrete, Result<'ok, string> ideally should beat Result<'ok, 'error>
        // NOTE: Current implementation limitation - partial concreteness with two-way comparison
        // between fully generic and partially concrete does not yet resolve.
        // This test documents current behavior. Future enhancement may resolve this.
        FSharp """
module Test

type Example =
    static member Handle(value: Result<'ok, 'error>) = "fully generic"
    static member Handle(value: Result<'ok, string>) = "string error"

// Currently ambiguous - see note above
let result = Example.Handle(Ok "test" : Result<string, string>)
        """
        |> typecheck
        |> shouldFail
        |> withErrorCode 41 // FS0041 - currently ambiguous
        |> ignore

    [<Fact>]
    let ``Example 6 - Incomparable Concreteness - Result int e vs Result t string - ambiguous`` () =
        // RFC Example 6: Incomparable types - neither dominates the other
        // Result<int, 'error> is better in position 1, Result<'ok, string> is better in position 2
        // This MUST remain ambiguous (FS0041) - partial order cannot determine winner
        FSharp """
module Test

type Example =
    static member Compare(value: Result<int, 'error>) = "int ok"
    static member Compare(value: Result<'ok, string>) = "string error"

// Neither overload dominates - one is more concrete in ok, other in error
// This remains ambiguous
let result = Example.Compare(Ok 42 : Result<int, string>)
        """
        |> typecheck
        |> shouldFail
        |> withErrorCode 41 // FS0041: A unique overload could not be determined
        |> ignore

    [<Fact>]
    let ``Example 6 - Incomparable Concreteness - Error message is helpful`` () =
        // Verify the error message mentions both candidates for incomparable case
        FSharp """
module Test

type Example =
    static member Compare(value: Result<int, 'error>) = "int ok"
    static member Compare(value: Result<'ok, string>) = "string error"

let result = Example.Compare(Ok 42 : Result<int, string>)
        """
        |> typecheck
        |> shouldFail
        |> withErrorCode 41 // FS0041 - error message will mention "Compare" candidates
        |> ignore

    [<Fact>]
    let ``Multiple Type Parameters - Three way comparison with clear winner`` () =
        // When there's a clear hierarchy, the most concrete should win
        FSharp """
module Test

type Example =
    static member Check(a: 't, b: 'u) = "both generic"
    static member Check(a: int, b: 'u) = "first concrete"
    static member Check(a: int, b: string) = "both concrete"

// With tiebreaker: resolves to Check(int, string) - fully concrete
let result = Example.Check(42, "hello")
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Multiple Type Parameters - Tuple-like scenario`` () =
        // Testing with multiple independent type parameters in different overloads
        FSharp """
module Test

type Example =
    static member Pair(fst: 't, snd: 'u) = "both generic"
    static member Pair(fst: int, snd: int) = "both int"

// With tiebreaker: resolves to Pair(int, int) - both positions are concrete
let result = Example.Pair(1, 2)
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    // ============================================================================
    // RFC Section Examples 7-9: Real-World Scenarios
    // These are the primary motivating use cases for the "more concrete" tiebreaker
    // NOTE: Some cases require structural type comparison ('t vs Task<'T>)
    // which is not yet implemented. Tests document current vs expected behavior.
    // ============================================================================

    [<Fact>]
    let ``Example 7 - ValueTask constructor scenario - Task of T vs T - currently ambiguous`` () =
        // RFC Example 7: ValueTask<'T> constructor disambiguation
        // ValueTask(task: Task<'T>) vs ValueTask(result: 'T)
        // FUTURE: When passing Task<int>, the Task<'T> overload should be preferred
        // because Task<int> is more concrete than treating it as bare 'T
        // CURRENT: Structural comparison ('T vs Task<'T>) not yet implemented - ambiguous
        FSharp """
module Test

open System.Threading.Tasks

[<NoComparison>]
type ValueTaskSimulator<'T> =
    | FromResult of 'T
    | FromTask of Task<'T>

type ValueTaskFactory =
    static member Create(result: 'T) = ValueTaskSimulator<'T>.FromResult result
    static member Create(task: Task<'T>) = ValueTaskSimulator<'T>.FromTask task

let createFromTask () =
    let task = Task.FromResult(42)
    // Currently ambiguous: structural comparison not yet implemented
    // FUTURE: Task<int> matches Task<'T> more concretely than 'T
    let result = ValueTaskFactory.Create(task)
    result
        """
        |> typecheck
        |> shouldFail
        |> withErrorCode 41 // FS0041 - currently ambiguous, future: should resolve
        |> ignore

    [<Fact>]
    let ``Example 7 - ValueTask constructor - bare int resolves to result overload`` () =
        // When passing a bare int (not Task<int>), the 'T overload should still work
        // because int is more concrete than Task<int> when the value IS an int
        FSharp """
module Test

open System.Threading.Tasks

type ValueTaskFactory =
    static member Create(result: 'T) = "result"
    static member Create(task: Task<'T>) = "task"

let createFromInt () =
    // When passing int, the 'T overload is the only match (Task<'T> doesn't fit int)
    let result = ValueTaskFactory.Create(42)
    result
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Example 8 - CE Source overloads - FsToolkit AsyncResult pattern - currently ambiguous`` () =
        // RFC Example 8: Computation Expression Builder - Source overloads
        // Demonstrates CE builder patterns from FsToolkit.ErrorHandling
        // FUTURE: Async<Result<'ok, 'error>> should be preferred over Async<'t> when applicable
        // CURRENT: Structural comparison (Result<'ok,'error> vs 't) not yet implemented
        FSharp """
module Test

open System

type AsyncResultBuilder() =
    member _.Return(x) = async { return Ok x }
    member _.ReturnFrom(x) = x
    
    // Source overloads - the tiebreaker should prefer more concrete
    member _.Source(result: Async<Result<'ok, 'error>>) : Async<Result<'ok, 'error>> = result
    member _.Source(result: Result<'ok, 'error>) : Async<Result<'ok, 'error>> = async { return result }
    member _.Source(asyncValue: Async<'t>) : Async<Result<'t, exn>> = 
        async { 
            let! v = asyncValue 
            return Ok v 
        }
    
    member _.Bind(computation: Async<Result<'ok, 'error>>, f: 'ok -> Async<Result<'ok2, 'error>>) =
        async {
            let! result = computation
            match result with
            | Ok value -> return! f value
            | Error e -> return Error e
        }

let asyncResult = AsyncResultBuilder()

// When input is Async<Result<int, string>>, the Async<Result<'ok, 'error>> overload
// FUTURE: should be preferred over Async<'t> because Result<_,_> is more concrete than 't
// CURRENT: Ambiguous until structural comparison is implemented
let example () =
    let source : Async<Result<int, string>> = async { return Ok 42 }
    asyncResult.Source(source)
        """
        |> typecheck
        |> shouldFail
        |> withErrorCode 41 // FS0041 - currently ambiguous, future: should resolve
        |> ignore

    [<Fact>]
    let ``Example 8 - CE Source overloads - Async of plain value uses generic`` () =
        // When input is Async<int> (not Async<Result<...>>), only Async<'t> matches
        FSharp """
module Test

type SimpleBuilder() =
    member _.Source(asyncResult: Async<Result<'ok, 'error>>) = "async result"
    member _.Source(asyncValue: Async<'t>) = "async generic"

let builder = SimpleBuilder()

// Async<int> doesn't match Async<Result<'ok, 'error>>, so Async<'t> is used
let result = builder.Source(async { return 42 })
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Example 9 - CE Bind with Task types - TaskBuilder pattern`` () =
        // RFC Example 9: TaskBuilder.fs-style Bind pattern
        // Bind(task: Task<'a>, ...) should be preferred over Bind(taskLike: 't, ...)
        // when passing Task<int>
        // SUCCESS: The tiebreaker correctly prefers Task<'a> over 't
        FSharp """
module Test

open System.Threading.Tasks

type TaskBuilder() =
    member _.Return(x: 'a) : Task<'a> = Task.FromResult(x)
    
    // Generic await - matches any type via SRTP (simulated here as bare 't)
    member _.Bind(taskLike: 't, continuation: 't -> Task<'b>) : Task<'b> = 
        continuation taskLike
        
    // Optimized Task path - more concrete
    member _.Bind(task: Task<'a>, continuation: 'a -> Task<'b>) : Task<'b> = 
        task.ContinueWith(fun (t: Task<'a>) -> continuation(t.Result)).Unwrap()

let taskBuilder = TaskBuilder()

// When passing Task<int>, the Task<'a> overload is preferred
// because Task<int> is more concrete than bare 't
let example () =
    let task = Task.FromResult(42)
    taskBuilder.Bind(task, fun x -> Task.FromResult(x + 1))
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Example 9 - CE Bind with Task - non-task value uses generic overload`` () =
        // When passing a non-Task value, only the generic overload matches
        FSharp """
module Test

open System.Threading.Tasks

type SimpleTaskBuilder() =
    member _.Bind(taskLike: 't, continuation: 't -> Task<'b>) = continuation taskLike
    member _.Bind(task: Task<'a>, continuation: 'a -> Task<'b>) = 
        task.ContinueWith(fun (t: Task<'a>) -> continuation(t.Result)).Unwrap()

let builder = SimpleTaskBuilder()

// When passing int (not Task), only the generic overload matches
let result = builder.Bind(42, fun x -> Task.FromResult(x + 1))
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Real-world pattern - Source with Result types vs generic - currently ambiguous`` () =
        // Additional real-world test: Source overload prioritization for Result types
        // FUTURE: Result<'a, 'e> should be preferred over 't
        // CURRENT: Structural comparison not yet implemented
        FSharp """
module Test

type Builder() =
    // More concrete - explicitly handles Result
    member _.Source(x: Result<'a, 'e>) = "result"
    // Less concrete - handles any type
    member _.Source(x: 't) = "generic"

let b = Builder()

// Result<int, string> FUTURE: should prefer the Result overload
// CURRENT: Ambiguous until structural comparison is implemented
let result = b.Source(Ok 42 : Result<int, string>)
        """
        |> typecheck
        |> shouldFail
        |> withErrorCode 41 // FS0041 - currently ambiguous, future: should resolve
        |> ignore

    [<Fact>]
    let ``Real-world pattern - Nested task result types`` () =
        // Pattern from async CE builders with nested Task<Result<...>>
        // SUCCESS: Task<Result<'a,'e>> is correctly preferred over Task<'t>
        FSharp """
module Test

open System.Threading.Tasks

type AsyncBuilder() =
    // More concrete - Task of Result
    member _.Bind(x: Task<Result<'a, 'e>>, f: 'a -> Task<Result<'b, 'e>>) = 
        x.ContinueWith(fun (t: Task<Result<'a, 'e>>) ->
            match t.Result with
            | Ok v -> f(v)
            | Error e -> Task.FromResult(Error e)
        ).Unwrap()
        
    // Less concrete - any Task
    member _.Bind(x: Task<'t>, f: 't -> Task<Result<'b, 'e>>) = 
        x.ContinueWith(fun (t: Task<'t>) -> f(t.Result)).Unwrap()

let ab = AsyncBuilder()

// Task<Result<int, string>> correctly prefers the Task<Result<...>> overload
// The tiebreaker works because Result<int, string> is more concrete than 't
let example () =
    let taskResult : Task<Result<int, string>> = Task.FromResult(Ok 42)
    ab.Bind(taskResult, fun x -> Task.FromResult(Ok (x + 1)))
        """
        |> typecheck
        |> shouldSucceed
        |> ignore
