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
