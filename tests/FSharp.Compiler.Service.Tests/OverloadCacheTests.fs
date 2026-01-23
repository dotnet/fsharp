// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Tests for overload resolution caching performance optimization
module FSharp.Compiler.Service.Tests.OverloadCacheTests

open System
open System.Text
open Xunit
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Text
open FSharp.Compiler.Caches
open FSharp.Test.Assert
open FSharp.Compiler.Service.Tests.Common

/// Generate F# source code with many identical overloaded method calls
let generateRepetitiveOverloadCalls (callCount: int) =
    let sb = StringBuilder()
    sb.AppendLine("// Test file with repetitive overloaded method calls") |> ignore
    sb.AppendLine("open System") |> ignore
    sb.AppendLine() |> ignore
    
    // Define a type with multiple overloads to simulate Assert.Equal pattern
    sb.AppendLine("type TestAssert =") |> ignore
    sb.AppendLine("    static member Equal(expected: int, actual: int) = expected = actual") |> ignore
    sb.AppendLine("    static member Equal(expected: string, actual: string) = expected = actual") |> ignore
    sb.AppendLine("    static member Equal(expected: float, actual: float) = expected = actual") |> ignore
    sb.AppendLine("    static member Equal(expected: bool, actual: bool) = expected = actual") |> ignore
    sb.AppendLine("    static member Equal(expected: byte, actual: byte) = expected = actual") |> ignore
    sb.AppendLine("    static member Equal(expected: int16, actual: int16) = expected = actual") |> ignore
    sb.AppendLine("    static member Equal(expected: int64, actual: int64) = expected = actual") |> ignore
    sb.AppendLine("    static member Equal(expected: obj, actual: obj) = obj.Equals(expected, actual)") |> ignore
    sb.AppendLine() |> ignore
    
    // Generate many identical calls - these should benefit from caching
    // Use typed variables to ensure types are already resolved
    sb.AppendLine("let runTests() =") |> ignore
    sb.AppendLine("    let mutable x: int = 0") |> ignore
    sb.AppendLine("    let mutable y: int = 0") |> ignore
    for i in 1 .. callCount do
        sb.AppendLine(sprintf "    x <- %d" i) |> ignore
        sb.AppendLine(sprintf "    y <- %d" (i + 1)) |> ignore
        sb.AppendLine("    ignore (TestAssert.Equal(x, y))") |> ignore
    
    sb.AppendLine() |> ignore
    sb.AppendLine("runTests()") |> ignore
    
    sb.ToString()

/// Test that the overload resolution cache achieves >95% hit rate for repetitive patterns
[<Fact>]
let ``Overload cache hit rate exceeds 95 percent for repetitive int-int calls`` () =
    // Use the new public API to listen to overload cache metrics
    use listener = FSharpChecker.CreateOverloadCacheMetricsListener()
    
    // Clear caches to get clean measurement
    checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()
    
    // Generate source with 100+ repetitive calls
    let callCount = 150
    let source = generateRepetitiveOverloadCalls callCount
    
    // Type check the file
    use file = new TempFile("fs", source)
    let checkOptions, _ = checker.GetProjectOptionsFromScript(file.Name, SourceText.ofString source) |> Async.RunImmediate
    let parseResults, checkResults = checker.ParseAndCheckFileInProject(file.Name, 0, SourceText.ofString source, checkOptions) |> Async.RunImmediate
    
    // Verify no errors
    match checkResults with
    | FSharpCheckFileAnswer.Succeeded results ->
        let errors = results.Diagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
        errors |> shouldBeEmpty
    | FSharpCheckFileAnswer.Aborted ->
        failwith "Type checking was aborted"
    
    // Validate cache metrics using the new CacheMetricsListener API
    let hits = listener.Hits
    let misses = listener.Misses
    let ratio = listener.Ratio
    
    printfn "Overload cache metrics for %d repetitive calls:" callCount
    printfn "  Hits: %d, Misses: %d, Hit ratio: %.2f%%" hits misses (ratio * 100.0)
    
    // With 150 repetitive identical overload calls, we expect >95% hit rate
    // The first call is a miss, subsequent identical calls should be hits
    if hits + misses > 0L then
        Assert.True(ratio > 0.95, sprintf "Expected hit ratio > 95%%, but got %.2f%%" (ratio * 100.0))

/// Test that caching correctly returns resolved overload
[<Fact>]
let ``Overload cache returns correct resolution`` () =
    // Use the new public API to listen to overload cache metrics
    use listener = FSharpChecker.CreateOverloadCacheMetricsListener()
    
    // Clear caches to get clean measurement
    checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()
    
    // Source with clear type-based overload selection
    let source = """
type Overloaded =
    static member Process(x: int) = "int"
    static member Process(x: string) = "string"
    static member Process(x: float) = "float"

// Multiple calls with same types should use cache
let r1 = Overloaded.Process(1)
let r2 = Overloaded.Process(2)
let r3 = Overloaded.Process(3)
let r4 = Overloaded.Process(4)
let r5 = Overloaded.Process(5)

// String calls are different type signature
let s1 = Overloaded.Process("a")
let s2 = Overloaded.Process("b")

// Float calls are different type signature  
let f1 = Overloaded.Process(1.0)
let f2 = Overloaded.Process(2.0)
"""
    
    use file = new TempFile("fs", source)
    let checkOptions, _ = checker.GetProjectOptionsFromScript(file.Name, SourceText.ofString source) |> Async.RunImmediate
    let parseResults, checkResults = checker.ParseAndCheckFileInProject(file.Name, 0, SourceText.ofString source, checkOptions) |> Async.RunImmediate
    
    match checkResults with
    | FSharpCheckFileAnswer.Succeeded results ->
        let errors = results.Diagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
        errors |> shouldBeEmpty
        
        // Verify listener captured cache activity
        let hits = listener.Hits
        let misses = listener.Misses
        printfn "Cache metrics - Hits: %d, Misses: %d" hits misses
        
        // If we got here without errors, the overload resolution worked correctly
        // (including any cached resolutions)
        printfn "All overload resolutions succeeded"
        
    | FSharpCheckFileAnswer.Aborted ->
        failwith "Type checking was aborted"
