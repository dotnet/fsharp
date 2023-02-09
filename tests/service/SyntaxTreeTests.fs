module Tests.Service.SyntaxTree

open System.IO
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Text
open NUnit.Framework

let testCasesDir = Path.Combine(__SOURCE_DIRECTORY__, "data", "SyntaxTree")
let fsharpFiles = set [| ".fs"; ".fsi"; ".fsx" |]

let allTestCases =
    Directory.EnumerateFiles(testCasesDir)
    |> Seq.filter (fun file -> FileInfo(file).Extension |> fsharpFiles.Contains)
    |> Seq.map (fun f ->
        let fileName = Path.GetFileName f
        [| fileName :> obj |])
    |> Seq.toArray

[<Literal>]
let RootDirectory = @"/root"

let parseSourceCode (name: string, code: string) =
    let location = Path.Combine(RootDirectory, name).Replace("\\", "/")

    let parseResults =
        checker.ParseFile(
            location,
            SourceText.ofString code,
            { FSharpParsingOptions.Default with
                SourceFiles = [| location |] }
        )
        |> Async.RunImmediate

    parseResults.ParseTree

/// Asserts the parsed untyped tree matches the expected baseline.
///
/// To update a baseline:
///     CMD: set TEST_UPDATE_BSL=1 & dotnet test --filter "ParseFile"
///     PowerShell: $env:TEST_UPDATE_BSL = "1" ; dotnet test --filter "ParseFile"
///     Linux/macOS: export TEST_UPDATE_BSL=1 & dotnet test --filter "ParseFile"
///
/// Assuming your current directory is tests/FSharp.Compiler.Service.Tests
[<TestCaseSource(nameof allTestCases)>]
let ParseFile fileName =
    let fullPath = Path.Combine(testCasesDir, fileName)
    let contents = File.ReadAllText fullPath
    let ast = parseSourceCode (fileName, contents)

    let normalize (s: string) = s.Replace("\r", "")
    let actual = sprintf "%A" ast |> normalize
    let bslPath = $"{fullPath}.bsl"

    let expected =
        if File.Exists bslPath then
            File.ReadAllText bslPath |> normalize
        else
            "No baseline was found"

    let testUpdateBSLEnv = System.Environment.GetEnvironmentVariable("TEST_UPDATE_BSL")

    if not (isNull testUpdateBSLEnv) && testUpdateBSLEnv.Trim() = "1" then
        File.WriteAllText(bslPath, actual)

    Assert.AreEqual(expected, actual)
