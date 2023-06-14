﻿module FSharpChecker.SymbolUse


open FSharp.Compiler.CodeAnalysis
open Xunit
open FSharp.Test.ProjectGeneration
open FSharp.Compiler.Symbols

open FSharp.Compiler.EditorServices
open FSharp.Compiler.NameResolution


module IsPrivateToFile =

    let functionParameter = "param"
    let source = $"""
    let f x = x + 1
    let f2 {functionParameter} = {functionParameter} + 1
    """
    let testFile = { sourceFile "Test" [] with Source = source }

    let signature = $"""
    val f: x:int -> int
    val f2: {functionParameter}: int -> int
    """

    let testFileWithSignature = { testFile with SignatureFile = Custom signature }

    [<Fact>]
    let ``Function definition in signature file`` () =
        let project = SyntheticProject.Create(
            testFileWithSignature,
            sourceFile "Second" [testFile.Id])

        project.Workflow {
            checkSymbolUse testFile.Id "f2" (fun symbolUse ->
                Assert.False(symbolUse.IsPrivateToFile)
                Assert.False(symbolUse.IsPrivateToFileAndSignatureFile))
        }

    [<Fact>]
    let ``Function definition, no signature file`` () =
        let project = SyntheticProject.Create(
            testFile,
            sourceFile "Second" [testFile.Id])

        project.Workflow {
            checkSymbolUse testFile.Id "f2" (fun symbolUse ->
                Assert.False(symbolUse.IsPrivateToFile)
                Assert.False(symbolUse.IsPrivateToFileAndSignatureFile))
        }

    [<Fact>]
    let ``Function definition not in signature file`` () =
        let signature = "val f: x: int -> int"
        let project = SyntheticProject.Create(
            { testFile with SignatureFile = Custom signature },
            sourceFile "Second" [testFile.Id])

        project.Workflow {
            checkSymbolUse testFile.Id "f2" (fun symbolUse ->
                Assert.True(symbolUse.IsPrivateToFile)
                Assert.False(symbolUse.IsPrivateToFileAndSignatureFile))
        }

    [<Fact>]
    let ``Function parameter, no signature file`` () =
        SyntheticProject.Create(testFile).Workflow {
            checkSymbolUse testFile.Id functionParameter (fun symbolUse ->
                Assert.True(symbolUse.IsPrivateToFile)
                Assert.False(symbolUse.IsPrivateToFileAndSignatureFile))
        }

    [<Fact>]
    let ``Function parameter, with signature file, part 1`` () =
        SyntheticProject.Create(testFileWithSignature).Workflow {
            checkSymbolUse testFile.Id functionParameter (fun symbolUse ->
                Assert.False(symbolUse.IsPrivateToFile))
        }

    [<Fact>]
    let ``Function parameter, with signature file, part 2`` () =
        SyntheticProject.Create(testFileWithSignature).Workflow {
            checkSymbolUse testFile.Id functionParameter (fun symbolUse ->
                Assert.True(symbolUse.IsPrivateToFileAndSignatureFile))
        }

    // [<Fact>] This is a bug - https://github.com/dotnet/fsharp/issues/14419
    let ``Private function, with signature file`` () =
        SyntheticProject.Create(
            { sourceFile "First" [] with ExtraSource = "let private f3 x = x + 1" } 
            |> addSignatureFile).Workflow {
            checkFile "First" (fun (typeCheckResult: FSharpCheckFileResults) ->
                let symbolUse = typeCheckResult.GetSymbolUseAtLocation(6, 14, "let private f3 x = x + 1", ["f3"]) |> Option.defaultWith (fun () -> failwith "no symbol use found")
                Assert.False(symbolUse.IsPrivateToFile))
        }
