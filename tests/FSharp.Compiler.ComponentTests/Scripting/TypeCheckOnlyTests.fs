module FSharp.Compiler.ComponentTests.Scripting.TypeCheckOnlyTests

open System.IO
open Xunit
open FSharp.Test
open FSharp.Test.Compiler

[<Fact>]
let ``typecheck-only flag works for valid script``() =
    Fsx """
let x = 42
printfn "This should not execute"
exit 999 // this would have crashed if really running
"""
    |> withOptions ["--typecheck-only"]
    |> runFsi
    |> shouldSucceed

[<Fact>]
let ``typecheck-only flag catches type errors``() =
    Fsx """
let x: int = "string"  // Type error
"""
    |> withOptions ["--typecheck-only"]
    |> runFsi
    |> shouldFail
    |> withStdErrContains """This expression was expected to have type"""

[<Fact>]
let ``typecheck-only flag prevents execution side effects``() =
    Fsx """
printfn "MyCrazyString"
let x = 42
"""
    |> withOptions ["--typecheck-only"]
    |> runFsi
    |> shouldSucceed
    |> verifyNotInOutput "MyCrazyString"

[<Fact>]
let ``script executes without typecheck-only flag``() =
    Fsx """
let x = 21+21
"""
    |> withOptions ["--nologo"]
    |> runFsi
    |> shouldSucceed
    |> verifyOutputContains [|"val x: int = 42"|]

[<Fact>]
let ``typecheck-only flag catches type errors in scripts with #load``() =
    let tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName())
    Directory.CreateDirectory(tempDir) |> ignore
    
    try
        let domainPath = Path.Combine(tempDir, "Domain.fsx")
        let mainPath = Path.Combine(tempDir, "A.fsx")
        
        File.WriteAllText(domainPath, "type T = { Field: string }\nprintfn \"D\"")
        File.WriteAllText(mainPath, sprintf "#load \"%s\"\nopen Domain\nlet y = { Field = 1 }\nprintfn \"A\"" domainPath)
        
        FsxFromPath mainPath
        |> withOptions ["--typecheck-only"]
        |> runFsi
        |> shouldFail
        |> withStdErrContains "This expression was expected to have type"
    finally
        if Directory.Exists(tempDir) then
            Directory.Delete(tempDir, true)