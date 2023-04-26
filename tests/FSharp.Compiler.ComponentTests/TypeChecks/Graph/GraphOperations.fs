module FSharp.Compiler.ComponentTests.TypeChecks.Graph.GraphOperations

open System.IO
open Xunit
open FSharp.Compiler.GraphChecking
open FSharp.Test.ProjectGeneration
open FSharp.Compiler.Text
open FSharp.Compiler


[<Fact>]
let ``Transform graph to layers of leaves`` () =

    let g = Graph.make [
        'B', [|'A'|]
        'C', [|'A'|]
        'E', [|'A'; 'B'; 'C'|]
        'F', [|'C'; 'D'|]
    ]

    let expected = [
        set [ 'A'; 'D' ]
        set [ 'B'; 'C' ]
        set [ 'E'; 'F' ]
    ]

    let result = Graph.leafSequence g |> Seq.toList

    Assert.Equal<Set<_> list>(expected, result)


[<Fact>]
let ``See what this does`` () =

    SyntheticProject.Create(
        sourceFile "A" [] |> addSignatureFile,
        sourceFile "B" ["A"] |> addSignatureFile,
        sourceFile "C" ["A"] |> addSignatureFile,
        sourceFile "D" [] |> addSignatureFile,
        sourceFile "E" ["A"; "B"; "C"] |> addSignatureFile,
        sourceFile "F" ["C"; "D"] |> addSignatureFile
    ).Workflow {
        withProject (fun project checker -> 
            async {
                let options = project.GetProjectOptions checker
                let options, _ = checker.GetParsingOptionsFromProjectOptions options
                let! inputs =
                    project.SourceFilePaths 
                    |> Seq.map (fun path -> path, File.ReadAllText path |> SourceText.ofString)
                    |> Seq.map (fun (path, text) -> checker.ParseFile(path, text, options))
                    |> Async.Parallel

                let sourceFiles: FileInProject array =
                    inputs
                    |> Seq.map (fun x -> x.ParseTree)
                    |> Seq.toArray
                    |> Array.mapi (fun idx (input: Syntax.ParsedInput) ->
                        {
                            Idx = idx
                            FileName = input.FileName
                            ParsedInput = input
                        })

                let filePairs = FilePairMap(sourceFiles)

                let fullGraph =
                    DependencyResolution.mkGraph false filePairs sourceFiles
                    |> Graph.map (fun idx -> project.SourceFilePaths[idx] |> Path.GetFileName)

                let subGraph = fullGraph |> Graph.subGraphFor "FileF.fs"

                let layers = Graph.leafSequence subGraph |> Seq.toList

                ignore layers

                return ()
            }
        )
    }

