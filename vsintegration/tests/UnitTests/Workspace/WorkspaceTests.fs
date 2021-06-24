namespace VisualFSharp.UnitTests

open System
open System.IO
open System.Reflection
open System.Linq
open System.Composition.Hosting
open System.Collections.Generic
open System.Collections.Immutable
open Microsoft.VisualStudio.Composition
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Host
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.VisualStudio.LanguageServices
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.FSharp.Editor.Tests.Roslyn
open NUnit.Framework

[<TestFixture>]
module WorkspaceTests =

    type TestFSharpWorkspaceProjectContext(mainProj: Project) =

        let mutable mainProj = mainProj

        interface IFSharpWorkspaceProjectContext with

            member this.Dispose(): unit = ()

            member this.FilePath: string = mainProj.FilePath

            member this.HasProjectReference(filePath: string): bool = 
                mainProj.ProjectReferences
                |> Seq.exists (fun x ->
                    let projRef = mainProj.Solution.GetProject(x.ProjectId)
                    if projRef <> null then
                        String.Equals(filePath, projRef.FilePath, StringComparison.OrdinalIgnoreCase)
                    else
                        false
                )

            member this.Id: ProjectId = mainProj.Id

            member this.ProjectReferenceCount: int = mainProj.ProjectReferences.Count()

            member this.SetProjectReferences(projRefs: seq<IFSharpWorkspaceProjectContext>): unit = 
                let currentProj = mainProj
                let mutable solution = currentProj.Solution

                mainProj.ProjectReferences
                |> Seq.iter (fun projRef ->
                    solution <- solution.RemoveProjectReference(currentProj.Id, projRef)
                )

                projRefs
                |> Seq.iter (fun projRef ->
                    solution <- 
                        solution.AddProjectReference(
                            currentProj.Id, 
                            ProjectReference(projRef.Id)
                        )
                )

                if not (solution.Workspace.TryApplyChanges(solution)) then
                    failwith "Unable to apply workspace changes."

                mainProj <- solution.GetProject(currentProj.Id)

    type TestFSharpWorkspaceProjectContextFactory(workspace: Workspace) =
                
        interface IFSharpWorkspaceProjectContextFactory with
            member this.CreateProjectContext(filePath: string): IFSharpWorkspaceProjectContext =
                let projInfo = RoslynTestHelpers.CreateProjectInfoWithSingleDocument(filePath)
                if not (workspace.TryApplyChanges(workspace.CurrentSolution.AddProject(projInfo))) then
                    failwith "Unable to apply workspace changes."

                let proj = workspace.CurrentSolution.GetProject(projInfo.Id)
                new TestFSharpWorkspaceProjectContext(proj) :> IFSharpWorkspaceProjectContext

    let createOnDiskScript src =
        let tmpFilePath = Path.GetTempFileName()
        let tmpRealFilePath = Path.ChangeExtension(tmpFilePath, ".fsx")
        try File.Delete(tmpFilePath) with | _ -> ()
        File.WriteAllText(tmpRealFilePath, src)
        tmpRealFilePath

    let createWorkspace() =
        new AdhocWorkspace(TestHostServices())

    let createMiscFileWorkspace() =
        createWorkspace()

    [<Test>]
    let ``Script file opened in misc files workspace will get transferred to normal workspace``() =
        let workspace = createWorkspace()
        let miscFilesWorkspace = createMiscFileWorkspace()
        let projectContextFactory = TestFSharpWorkspaceProjectContextFactory(workspace)

        let miscFileService = FSharpMiscellaneousFileService(workspace, miscFilesWorkspace, projectContextFactory)

        let filePath = 
            createOnDiskScript 
                """
module Script1

let x = 1
                """
        
        try
            let solutionInfo = RoslynTestHelpers.CreateSolutionInfoWithSingleDocument(filePath)
            miscFilesWorkspace.AddSolution(solutionInfo) |> ignore
            
            let doc = 
                miscFilesWorkspace.CurrentSolution.GetDocumentIdsWithFilePath(filePath) 
                |> Seq.exactlyOne
                |> miscFilesWorkspace.CurrentSolution.GetDocument

            Assert.IsFalse(miscFilesWorkspace.IsDocumentOpen(doc.Id))
            Assert.AreEqual(0, workspace.CurrentSolution.GetDocumentIdsWithFilePath(filePath).Length)

        finally
            try File.Delete(filePath) with | _ -> ()
