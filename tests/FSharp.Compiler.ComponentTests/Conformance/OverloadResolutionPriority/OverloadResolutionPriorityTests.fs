// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.OverloadResolutionPriority

open FSharp.Test
open FSharp.Test.Compiler
open Xunit

/// Tests for OverloadResolutionPriority attribute support (.NET 9)
/// 
/// This test compiles the C# library ONCE and runs all F# assertions in a single test.
/// The tests verify:
/// 1. F# correctly respects [OverloadResolutionPriority] from C# libraries
/// 2. F# code can USE the [OverloadResolutionPriority] attribute to define prioritized overloads
module OverloadResolutionPriorityTests =

    /// Single comprehensive test that compiles C# library once and runs all assertions
    [<Fact>]
    let ``OverloadResolutionPriority - comprehensive test`` () =
        // Compile C# library with all ORP test types (compiled ONCE)
        let csharpLib =
            CSharpFromPath (__SOURCE_DIRECTORY__ ++ "CSharpPriorityLib.cs")
            |> withCSharpLanguageVersionPreview
            |> withName "CSharpPriorityLib"
        
        // F# test runner with all assertions (compiled and run ONCE)
        FsFromPath (__SOURCE_DIRECTORY__ ++ "ORPTestRunner.fs")
        |> withReferences [csharpLib]
        |> withLangVersionPreview
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore
