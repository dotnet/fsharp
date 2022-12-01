module FSharp.Compiler.ComponentTests.FSharpChecker.SymbolUse

open FSharp.Compiler.CodeAnalysis
open Xunit
open FSharp.Test.ProjectGeneration


module IsPrivateToFile =

    [<Fact>]
    let ``Function definition in signature file`` () =
        let project = SyntheticProject.Create(
            sourceFile "First" [] |> addSignatureFile,
            sourceFile "Second" ["First"])

        project.Workflow {
            checkFile "First" (fun (typeCheckResult: FSharpCheckFileResults) ->
                let symbolUse = typeCheckResult.GetSymbolUseAtLocation(5, 6, "let f2 x = x + 1", ["f2"]) |> Option.defaultWith (fun () -> failwith "no symbol use found")
                Assert.False(symbolUse.IsPrivateToFile))
        }

    [<Fact>]
    let ``Function definition, no signature file`` () =
        let project = SyntheticProject.Create(
            sourceFile "First" [],
            sourceFile "Second" ["First"])

        project.Workflow {
            checkFile "First" (fun (typeCheckResult: FSharpCheckFileResults) ->
                let symbolUse = typeCheckResult.GetSymbolUseAtLocation(5, 6, "let f2 x = x + 1", ["f2"]) |> Option.defaultWith (fun () -> failwith "no symbol use found")
                Assert.False(symbolUse.IsPrivateToFile))
        }

    [<Fact>]
    let ``Function definition not in signature file`` () =
        let projectName = "IsPrivateToFileTest1"
        let signature = $"""
module {projectName}.ModuleFirst
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
                Assert.True(symbolUse.IsPrivateToFile))
        }

    [<Fact>]
    let ``Function parameter, no signature file`` () =
        SyntheticProject.Create(sourceFile "First" []).Workflow {
            checkFile "First" (fun (typeCheckResult: FSharpCheckFileResults) ->
                let symbolUse = typeCheckResult.GetSymbolUseAtLocation(5, 8, "let f2 x = x + 1", ["x"]) |> Option.defaultWith (fun () -> failwith "no symbol use found")
                Assert.True(symbolUse.IsPrivateToFile))
        }

    /// This is a bug: https://github.com/dotnet/fsharp/issues/14277
    [<Fact>]
    let ``Function parameter, with signature file`` () =
        SyntheticProject.Create(sourceFile "First" [] |> addSignatureFile).Workflow {
            checkFile "First" (fun (typeCheckResult: FSharpCheckFileResults) ->
                let symbolUse = typeCheckResult.GetSymbolUseAtLocation(5, 8, "let f2 x = x + 1", ["x"]) |> Option.defaultWith (fun () -> failwith "no symbol use found")
                // This should be false, because it's also in the signature file
                Assert.True(symbolUse.IsPrivateToFile))
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
