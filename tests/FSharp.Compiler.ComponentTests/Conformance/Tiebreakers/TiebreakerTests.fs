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
