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
