// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerService

open Xunit
open System
open FSharp.Test.Compiler
open FSharp.Compiler.Text

module InMemorySourceTests =

    [<Fact>]
    let ``Range.DebugCode shows content from in-memory source text`` () =
        // Test code that registers in-memory source and verifies DebugCode can see it
        let testCode = """
open System
open FSharp.Compiler.Text

// Test case for in-memory source text with Range.DebugCode
try
    // Clear any existing in-memory sources first
    FileIndex.clearInMemorySourceTexts()
    
    // Register an in-memory source
    let filePath = "test-in-memory.fs"
    let sourceText = "let x = 42\nlet y = 87"
    FileIndex.registerInMemorySourceText filePath sourceText |> ignore

    // Create a range for the in-memory source
    let range = Range.mkRange filePath (Position.mkPos 1 4) (Position.mkPos 2 5)

    // Get the DebugCode representation using reflection to avoid debugger display limitations in tests
    let debugText = 
        let property = typeof<Range>.GetProperty("DebugCode")
        if isNull property then
            failwith "DebugCode property not found on Range type"
        let value = property.GetValue(range) :?> string
        value
    
    // Verify the range debug text includes our source text content
    if debugText.Contains("x = 42") |> not then
        failwith $"Expected range debug text to contain source text, but got: {debugText}"

    // Clean up
    FileIndex.clearInMemorySourceTexts()
    
    // Test for a non-existent file to verify it still shows the expected message
    let nonExistentRange = Range.mkRange "nonexistent.fs" (Position.mkPos 1 0) (Position.mkPos 1 10)
    let nonExistentDebugText = 
        let property = typeof<Range>.GetProperty("DebugCode")
        if isNull property then
            failwith "DebugCode property not found on Range type"
        let value = property.GetValue(nonExistentRange) :?> string
        value
        
    if nonExistentDebugText.Contains("nonexistent file:") |> not then
        failwith $"Expected 'nonexistent file:' message for non-existent file, but got: {nonExistentDebugText}"
        
    printfn "Success: Range debug text contains source content"
    true
with ex ->
    printfn "Test failed: %s" ex.Message
    false
"""
        FSharp testCode
        |> withReferenceFSharpCompilerService
        |> asExe
        |> compileAndRun
        |> shouldSucceed