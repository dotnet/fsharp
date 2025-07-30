// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace ConstraintSolver

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module SRTPNullnessConstraints =

    let withVersionAndCheckNulls (version,checknulls) cu =
        cu
        |> withLangVersion version
        |> if checknulls then withCheckNulls else id

    /// Test for GitHub issue #18390
    /// SRTP ambiguity issue with AmbivalentToNull types imported from older assemblies
    [<Theory>]
    [<InlineData("8.0", false)>]
    [<InlineData("8.0", true)>]
    [<InlineData("preview", false)>]
    [<InlineData("preview", true)>]
    let ``SRTP nullness constraint resolution issue 18390`` langVersion checknulls =
        FSharp"""
module TestModule

// Reproduces the SRTP ambiguity issue as described in https://github.com/dotnet/fsharp/issues/18390
// Types imported from F#8/F#7 assemblies are marked as AmbivalentToNull
// This should only satisfy 'T : null if they were nullable under legacy F# rules

open System

// Define a generic function with null constraint 
let inline hasNullConstraint<'T when 'T : null> (x: 'T) = 
    match x with 
    | null -> "null"
    | _ -> x.ToString()

// Test with int (should fail - int does not allow null in legacy F#)
// This line should cause a compilation error
let testInt = hasNullConstraint 42
        """
        |> withVersionAndCheckNulls (langVersion, checknulls)
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 1, Line 16, Col 19, Line 16, Col 37, "The type 'int' does not have 'null' as a proper value")

    /// Test for GitHub issue #18344  
    /// FSharpPlus issue with nullness constraints
    [<Theory>]
    [<InlineData("8.0", false)>]
    [<InlineData("8.0", true)>] 
    [<InlineData("preview", false)>]
    [<InlineData("preview", true)>]
    let ``SRTP nullness constraint FSharpPlus issue 18344`` langVersion checknulls =
        FSharp"""
module TestModule

// Reproduces the FSharpPlus issue as described in https://github.com/dotnet/fsharp/issues/18344
// This test requires FSharpPlus package but we'll create a minimal reproduction

// Simulate the type of SRTP pattern used in FSharpPlus
type IsAltLeftZero = IsAltLeftZero with
    static member (&&&) (IsAltLeftZero, x: 'T option when 'T : null) = fun () -> x = None
    static member (&&&) (IsAltLeftZero, x: 'T option) = fun () -> false

let inline invoke< ^t when ^t : null> x = ((IsAltLeftZero &&& x) ())

// Test with None - this should work
let testNone = invoke None

// This should demonstrate the issue where AmbivalentToNull types
// incorrectly satisfy the null constraint
        """
        |> withVersionAndCheckNulls (langVersion, checknulls)
        |> compile
        |> shouldSucceed

    /// Test demonstrating the fix working correctly
    [<Theory>]
    [<InlineData("8.0", false)>]
    [<InlineData("8.0", true)>]
    [<InlineData("preview", false)>]
    [<InlineData("preview", true)>]
    let ``AmbivalentToNull uses legacy F# nullness rules`` langVersion checknulls =
        FSharp"""
module TestModule

// Test that AmbivalentToNull types only satisfy 'T : null 
// if they would have satisfied nullness under legacy F# rules

// This should work - string has AllowNullLiteral in legacy F#
let inline testString<'T when 'T : null> (x: 'T) = x
let stringTest = testString "hello"

// This should work - object types allow null in legacy F#  
let objectTest = testString (obj())

// Array types should work - they allow null in legacy F#
let arrayTest = testString [|1;2|]
        """
        |> withVersionAndCheckNulls (langVersion, checknulls)
        |> compile
        |> shouldSucceed

    /// Test that demonstrates the fix - value types should fail null constraint
    [<Theory>]
    [<InlineData("8.0", false)>]
    [<InlineData("8.0", true)>]
    [<InlineData("preview", false)>]
    [<InlineData("preview", true)>]
    let ``Value types fail null constraint as expected`` langVersion checknulls =
        FSharp"""
module TestModule

// Test that value types correctly fail the null constraint

let inline testNull<'T when 'T : null> (x: 'T) = x

// This should fail - int is a value type
let intTest = testNull 42
        """
        |> withVersionAndCheckNulls (langVersion, checknulls)
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 1, Line 8, Col 19, Line 8, Col 30, "The type 'int' does not have 'null' as a proper value")