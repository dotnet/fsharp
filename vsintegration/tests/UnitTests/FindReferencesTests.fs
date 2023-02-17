module FSharp.Editor.Tests.FindReferencesTests

open NUnit.Framework

open FSharp.Test.ProjectGeneration
open FSharp.Editor.Tests.Helpers

open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Editor.FindUsages
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.FindUsages

open Microsoft.VisualStudio.FSharp.Editor

open System.Threading.Tasks
open System.Threading
open System.IO


let getPositionOf (subString: string) (filePath) =
    filePath
    |> File.ReadAllText
    |> fun source -> source.IndexOf subString


[<Test>]
let ``Find references to a document-local symbol`` () =

    let project = SyntheticProject.Create(
        sourceFile "First" [] |> addSignatureFile,
        sourceFile "Second" [])
    
    let solution, checker = RoslynTestHelpers.CreateSolution project
    
    let _projectDir = project.ProjectDir

    ignore solution
    ignore checker

    let context =
        { new IFSharpFindUsagesContext with

            member _.OnDefinitionFoundAsync (definition: FSharpDefinitionItem) = Task.CompletedTask

            member _.OnReferenceFoundAsync (reference: FSharpSourceReferenceItem) = Task.CompletedTask

            member _.ReportMessageAsync (message: string) = Task.CompletedTask

            member _.ReportProgressAsync (current: int, maximum: int) = Task.CompletedTask

            member _.SetSearchTitleAsync (title: string) = Task.CompletedTask

            member _.CancellationToken = CancellationToken.None
        }

    let findUsagesService = FSharpFindUsagesService() :> IFSharpFindUsagesService

    let documentPath = project.GetFilePath "Second"
    let document = solution.TryGetDocumentFromPath documentPath |> Option.defaultWith (failwith "Document not found")
    
    //findService.FindReferencesAsync(

    ()
    ()


let ``Find references to a implementation + signature local symbol`` () = ()

let ``Find references to a symbol in project`` () = ()

let ``Find references to a symbol in multiple projects`` () = ()
