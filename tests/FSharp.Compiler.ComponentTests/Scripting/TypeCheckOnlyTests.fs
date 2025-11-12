module FSharp.Compiler.ComponentTests.Scripting.TypeCheckOnlyTests

open System
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
        
        File.WriteAllText(domainPath, "type T = {\n    Field: string\n}\n\nprintfn \"printfn Domain.fsx\"")
        // Use absolute path in #load directive since runFsi uses EvalInteraction which doesn't preserve script location context
        let mainContent = sprintf "#load \"%s\"\n\nopen Domain\n\nlet y = {\n    Field = 1\n}\n\nprintfn \"printfn A.fsx\"" (domainPath.Replace("\\", "\\\\"))
        File.WriteAllText(mainPath, mainContent)
        
        FsxFromPath mainPath
        |> withOptions ["--typecheck-only"]
        |> runFsi
        |> shouldFail
        |> withStdErrContains "This expression was expected to have type"
    finally
        try
            if Directory.Exists(tempDir) then
                Directory.Delete(tempDir, true)
        with _ -> ()

[<Fact>]
let ``typecheck-only flag catches type errors in loaded file``() =
    let tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName())
    Directory.CreateDirectory(tempDir) |> ignore
    
    try
        let domainPath = Path.Combine(tempDir, "Domain.fsx")
        let mainPath = Path.Combine(tempDir, "A.fsx")
        
        File.WriteAllText(domainPath, "type T = { Field: string }\nlet x: int = \"error\"\nprintfn \"D\"")
        // Use absolute path in #load directive since runFsi uses EvalInteraction which doesn't preserve script location context
        let mainContent = sprintf "#load \"%s\"\nopen Domain\nlet y = { Field = \"ok\" }\nprintfn \"A\"" (domainPath.Replace("\\", "\\\\"))
        File.WriteAllText(mainPath, mainContent)
        
        FsxFromPath mainPath
        |> withOptions ["--typecheck-only"]
        |> runFsi
        |> shouldFail
        |> withStdErrContains "This expression was expected to have type"
    finally
        try
            if Directory.Exists(tempDir) then
                Directory.Delete(tempDir, true)
        with _ -> ()

[<Fact>]
let ``typecheck-only flag prevents execution with #load``() =
    let tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName())
    Directory.CreateDirectory(tempDir) |> ignore
    
    try
        let domainPath = Path.Combine(tempDir, "Domain.fsx")
        let mainPath = Path.Combine(tempDir, "A.fsx")
        
        File.WriteAllText(domainPath, "type T = { Field: string }\nprintfn \"Domain.fsx output\"")
        // Use absolute path in #load directive since runFsi uses EvalInteraction which doesn't preserve script location context
        let mainContent = sprintf "#load \"%s\"\nopen Domain\nlet y = { Field = \"test\" }\nprintfn \"A.fsx output\"" (domainPath.Replace("\\", "\\\\"))
        File.WriteAllText(mainPath, mainContent)
        
        FsxFromPath mainPath
        |> withOptions ["--typecheck-only"]
        |> runFsi
        |> shouldSucceed
        |> verifyNotInOutput "Domain.fsx output"
        |> verifyNotInOutput "A.fsx output"
    finally
        try
            if Directory.Exists(tempDir) then
                Directory.Delete(tempDir, true)
        with _ -> ()