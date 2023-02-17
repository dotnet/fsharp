module FSharp.Editor.Tests.FindReferencesTests

open Xunit
open FSharp.Test.ProjectGeneration
open FSharp.Editor.Tests.Helpers

open Microsoft.CodeAnalysis.ExternalAccess.FSharp.FindUsages


[<Fact>]
let ``Find references to a document-local symbol`` () =

    let project = SyntheticProject.Create(
        sourceFile "First" [] |> addSignatureFile,
        sourceFile "Second" [])
    
    let solution, checker = RoslynTestHelpers.CreateSolution project
    
    let _projectDir = project.ProjectDir

    ignore solution
    ignore checker
    

    //let context =
    //    { new IFSharpFindUsagesContext
    //        with member _.x = ()
    //        }

    ()


let ``Find references to a implementation + signature local symbol`` () = ()

let ``Find references to a symbol in project`` () = ()

let ``Find references to a symbol in multiple projects`` () = ()
