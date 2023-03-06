﻿module FSharp.Compiler.ComponentTests.FSharpChecker.SymbolUse


open FSharp.Compiler.CodeAnalysis
open Xunit
open FSharp.Test.ProjectGeneration
open FSharp.Compiler.Symbols

open FSharp.Compiler.EditorServices
open FSharp.Compiler.NameResolution


module IsPrivateToFile =

    [<Fact>]
    let ``Function definition in signature file`` () =
        let project = SyntheticProject.Create(
            sourceFile "First" [] |> addSignatureFile,
            sourceFile "Second" ["First"])

        project.Workflow {
            checkFile "First" (fun (typeCheckResult: FSharpCheckFileResults) ->
                let symbolUse = typeCheckResult.GetSymbolUseAtLocation(5, 6, "let f2 x = x + 1", ["f2"]) |> Option.defaultWith (fun () -> failwith "no symbol use found")
                Assert.False(symbolUse.IsPrivateToFile)
                Assert.False(symbolUse.IsPrivateToFileAndSignatureFile))
        }

    [<Fact>]
    let ``Function definition, no signature file`` () =
        let project = SyntheticProject.Create(
            sourceFile "First" [],
            sourceFile "Second" ["First"])

        project.Workflow {
            checkFile "First" (fun (typeCheckResult: FSharpCheckFileResults) ->
                let symbolUse = typeCheckResult.GetSymbolUseAtLocation(5, 6, "let f2 x = x + 1", ["f2"]) |> Option.defaultWith (fun () -> failwith "no symbol use found")
                Assert.False(symbolUse.IsPrivateToFile)
                Assert.False(symbolUse.IsPrivateToFileAndSignatureFile))
        }

    [<Fact>]
    let ``Function definition not in signature file`` () =
        let projectName = "IsPrivateToFileTest1"
        let signature = $"""
type TFirstV_1<'a> = | TFirst of 'a
val f: x: 'a -> TFirstV_1<'a>
// no f2 here
"""
        let project = SyntheticProject.Create(projectName,
            { sourceFile "First" [] with SignatureFile = Custom signature },
            sourceFile "Second" ["First"])

        project.Workflow {
            checkFile "First" (fun (typeCheckResult: FSharpCheckFileResults) ->
                let symbolUse = typeCheckResult.GetSymbolUseAtLocation(5, 6, "let f2 x = x + 1", ["f2"]) |> Option.defaultWith (fun () -> failwith "no symbol use found")
                Assert.True(symbolUse.IsPrivateToFile)
                Assert.False(symbolUse.IsPrivateToFileAndSignatureFile))
        }

    [<Fact>]
    let ``Function parameter, no signature file`` () =
        SyntheticProject.Create(sourceFile "First" []).Workflow {
            checkFile "First" (fun (typeCheckResult: FSharpCheckFileResults) ->
                let symbolUse = typeCheckResult.GetSymbolUseAtLocation(5, 8, "let f2 x = x + 1", ["x"]) |> Option.defaultWith (fun () -> failwith "no symbol use found")
                Assert.True(symbolUse.IsPrivateToFile)
                Assert.False(symbolUse.IsPrivateToFileAndSignatureFile))
        }

    [<Fact>]
    let ``Function parameter, with signature file, part 1`` () =
        SyntheticProject.Create(sourceFile "First" [] |> addSignatureFile).Workflow {
            checkFile "First" (fun (typeCheckResult: FSharpCheckFileResults) ->
                let symbolUse = typeCheckResult.GetSymbolUseAtLocation(5, 8, "let f2 x = x + 1", ["x"]) |> Option.defaultWith (fun () -> failwith "no symbol use found")
                Assert.False(symbolUse.IsPrivateToFile))
        }

    [<Fact>]
    let ``Function parameter, with signature file, part 2`` () =
        SyntheticProject.Create(sourceFile "First" [] |> addSignatureFile).Workflow {
            checkFile "First" (fun (typeCheckResult: FSharpCheckFileResults) ->
                let symbolUse = typeCheckResult.GetSymbolUseAtLocation(5, 8, "let f2 x = x + 1", ["x"]) |> Option.defaultWith (fun () -> failwith "no symbol use found")
                Assert.True(symbolUse.IsPrivateToFileAndSignatureFile))
        }

    [<Fact>]
    let ``Function parameter, with signature file, part 3`` () =

        for attempt in 1 .. 20 do
            SyntheticProject.Create(sourceFile "First" [] |> addSignatureFile).Workflow {
                checkFile "First" (fun (typeCheckResult: FSharpCheckFileResults) ->
                    let symbolUse = typeCheckResult.GetSymbolUseAtLocation(5, 8, "let f2 x = x + 1", ["x"]) |> Option.defaultWith (fun () -> failwith "no symbol use found")

                    let couldBeParameter, declarationLocation =
                        match symbolUse.Symbol with
                        | :? FSharpParameter as p -> true, Some p.DeclarationLocation
                        | :? FSharpMemberOrFunctionOrValue as m when not m.IsModuleValueOrMember -> true, Some m.DeclarationLocation
                        | _ -> false, None

                    let thisIsSignature = SourceFileImpl.IsSignatureFile symbolUse.Range.FileName

                    let argReprInfo =
                        match symbolUse.Symbol.Item with
                        | Item.Value v -> v.Deref.ArgReprInfoForDisplay
                        | _ -> None

                    let signatureLocation = argReprInfo |> Option.bind (fun a -> a.OtherRange)

                    let diagnostics = $"#{attempt} couldBeParameter: {couldBeParameter} \n declarationLocation: {declarationLocation} \n thisIsSignature: {thisIsSignature} \n signatureLocation: {signatureLocation} \n argReprInfo: {argReprInfo}"

                    let result =
                        couldBeParameter
                        && (thisIsSignature
                            || (signatureLocation.IsSome && signatureLocation <> declarationLocation))

                    if not result then
                        failwith diagnostics)
            }
            |> ignore

    // [<Fact>] This is a bug - https://github.com/dotnet/fsharp/issues/14419
    let ``Private function, with signature file`` () =
        SyntheticProject.Create(
            { sourceFile "First" [] with ExtraSource = "let private f3 x = x + 1" } 
            |> addSignatureFile).Workflow {
            checkFile "First" (fun (typeCheckResult: FSharpCheckFileResults) ->
                let symbolUse = typeCheckResult.GetSymbolUseAtLocation(6, 14, "let private f3 x = x + 1", ["f3"]) |> Option.defaultWith (fun () -> failwith "no symbol use found")
                Assert.False(symbolUse.IsPrivateToFile))
        }
