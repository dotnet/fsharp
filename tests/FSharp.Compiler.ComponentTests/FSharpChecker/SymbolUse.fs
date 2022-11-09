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
