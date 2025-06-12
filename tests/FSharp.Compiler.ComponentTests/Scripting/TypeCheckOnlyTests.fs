module FSharp.Compiler.ComponentTests.Scripting.TypeCheckOnlyTests

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

[<Fact>]
let ``typecheck-only flag works for valid script``() =
    Fsx """
let x = 42
printfn "This should not execute"
"""
    |> withOptions ["--typecheck-only"]
    |> compile
    |> shouldSucceed

[<Fact>]
let ``typecheck-only flag catches type errors``() =
    Fsx """
let x: int = "string"  // Type error
"""
    |> withOptions ["--typecheck-only"]
    |> compile
    |> shouldFail
    |> withDiagnostics [
        (Error 1, Line 2, Col 14, Line 2, Col 22, "This expression was expected to have type\n    'int'    \nbut here has type\n    'string'")
    ]

[<Fact>]
let ``typecheck-only flag prevents execution side effects``() =
    let testFilePath = System.IO.Path.GetTempFileName()
    System.IO.File.Delete(testFilePath) // Make sure file doesn't exist initially
    
    Fsx $"""
System.IO.File.WriteAllText("{testFilePath}", "should not be created")
let x = 42
"""
    |> withOptions ["--typecheck-only"]
    |> eval
    |> shouldSucceed
    
    // Verify file was not created
    Assert.False(System.IO.File.Exists(testFilePath), "File should not have been created when using --typecheck-only")

[<Fact>]
let ``script executes without typecheck-only flag``() =
    let testFilePath = System.IO.Path.GetTempFileName()
    System.IO.File.Delete(testFilePath) // Make sure file doesn't exist initially
    
    Fsx $"""
System.IO.File.WriteAllText("{testFilePath}", "file was created")
let x = 42
"""
    |> eval
    |> shouldSucceed
    
    // Verify file was created when not using --typecheck-only
    Assert.True(System.IO.File.Exists(testFilePath), "File should have been created when not using --typecheck-only")
    
    // Clean up
    if System.IO.File.Exists(testFilePath) then
        System.IO.File.Delete(testFilePath)