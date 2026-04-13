module FSharp.Compiler.ComponentTests.Scripting.TypeCheckOnlyTests

open System
open System.IO
open Xunit
open FSharp.Test
open FSharp.Test.Compiler

let private withTempDirectory (test: string -> unit) =
    let tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName())
    Directory.CreateDirectory(tempDir) |> ignore

    try
        test tempDir
    finally
        try
            if Directory.Exists(tempDir) then
                Directory.Delete(tempDir, true)
        with _ -> ()

let private writeScript (dir: string) (filename: string) (content: string) =
    let path = Path.Combine(dir, filename)
    File.WriteAllText(path, content)
    path

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
    withTempDirectory (fun tempDir ->
        let domainPath = writeScript tempDir "Domain.fsx" "type T = {\n    Field: string\n}\n\nprintfn \"printfn Domain.fsx\""
        
        let mainContent = sprintf "#load \"%s\"\n\nopen Domain\n\nlet y = {\n    Field = 1\n}\n\nprintfn \"printfn A.fsx\"" (domainPath.Replace("\\", "\\\\"))
        let mainPath = writeScript tempDir "A.fsx" mainContent
        
        FsxFromPath mainPath
        |> withOptions ["--typecheck-only"]
        |> runFsi
        |> shouldFail
        |> withStdErrContains "This expression was expected to have type"
        |> ignore)

[<Fact>]
let ``typecheck-only flag catches type errors in loaded file``() =
    withTempDirectory (fun tempDir ->
        let domainPath = writeScript tempDir "Domain.fsx" "type T = { Field: string }\nlet x: int = \"error\"\nprintfn \"D\""
        
        let mainContent = sprintf "#load \"%s\"\nopen Domain\nlet y = { Field = \"ok\" }\nprintfn \"A\"" (domainPath.Replace("\\", "\\\\"))
        let mainPath = writeScript tempDir "A.fsx" mainContent
        
        FsxFromPath mainPath
        |> withOptions ["--typecheck-only"]
        |> runFsi
        |> shouldFail
        |> withStdErrContains "This expression was expected to have type"
        |> ignore)

[<Fact>]
let ``typecheck-only flag prevents execution with #load``() =
    withTempDirectory (fun tempDir ->
        let domainPath = writeScript tempDir "Domain.fsx" "type T = { Field: string }\nprintfn \"Domain.fsx output\""

        let mainContent = sprintf "#load \"%s\"\nopen Domain\nlet y = { Field = \"test\" }\nprintfn \"A.fsx output\"" (domainPath.Replace("\\", "\\\\"))
        let mainPath = writeScript tempDir "A.fsx" mainContent

        FsxFromPath mainPath
        |> withOptions ["--typecheck-only"]
        |> runFsi
        |> shouldSucceed
        |> verifyNotInOutput "Domain.fsx output"
        |> verifyNotInOutput "A.fsx output"
        |> ignore)

// https://github.com/dotnet/fsharp/issues/15070
[<Fact>]
let ``Issue 15070 - hash-load with dot-slash prefix does not produce FS1141`` () =
    // Before the fix, #load "./path" produced FS1141 "Identifiers followed by '!' are reserved for future use"
    // After the fix, the parser accepts "./path" and reports a file-not-found error instead.
    withTempDirectory (fun tempDir ->
        let mainContent = "#load \"./non-existent-subfolder/some-script.fsx\""
        let mainPath = writeScript tempDir "main.fsx" mainContent

        FsxFromPath mainPath
        |> runFsi
        |> shouldFail
        |> withStdErrContains "Unable to find the file"
        |> ignore)
