namespace VisualFSharp.UnitTests

open System
open System.IO
open System.Reflection
open System.Linq
open System.Composition.Hosting
open System.Collections.Generic
open System.Collections.Immutable
open System.Threading
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

    type TestFSharpWorkspaceProjectContextFactory(workspace: Workspace, miscFilesWorkspace: Workspace) =
                
        interface IFSharpWorkspaceProjectContextFactory with
            member this.CreateProjectContext(filePath: string): IFSharpWorkspaceProjectContext =
                match miscFilesWorkspace.CurrentSolution.GetDocumentIdsWithFilePath(filePath) |> Seq.tryExactlyOne with
                | Some docId ->
                    let doc = miscFilesWorkspace.CurrentSolution.GetDocument(docId)
                    if not (miscFilesWorkspace.TryApplyChanges(miscFilesWorkspace.CurrentSolution.RemoveProject(doc.Project.Id))) then
                        failwith "Unable to apply workspace changes."
                | _ ->
                    ()

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

    let openDocument (workspace: Workspace) (docId: DocumentId) =
        use waitHandle = new ManualResetEventSlim(false)
        use _sub = workspace.DocumentOpened.Subscribe(fun _ ->
            waitHandle.Set()
        )
        workspace.OpenDocument(docId)
        waitHandle.Wait()

    [<Test>]
    let ``Script file opened in misc files workspace will get transferred to normal workspace``() =
        let workspace = createWorkspace()
        let miscFilesWorkspace = createMiscFileWorkspace()
        let projectContextFactory = TestFSharpWorkspaceProjectContextFactory(workspace, miscFilesWorkspace)

        let _miscFileService = FSharpMiscellaneousFileService(workspace, miscFilesWorkspace, projectContextFactory)

        let filePath = 
            createOnDiskScript 
                """
module Script1

let x = 1
                """
        
        try
            let projInfo = RoslynTestHelpers.CreateProjectInfoWithSingleDocument(filePath)
            if not (miscFilesWorkspace.TryApplyChanges(miscFilesWorkspace.CurrentSolution.AddProject(projInfo))) then
                failwith "Unable to apply workspace changes."
            
            let doc = 
                miscFilesWorkspace.CurrentSolution.GetDocumentIdsWithFilePath(filePath) 
                |> Seq.exactlyOne
                |> miscFilesWorkspace.CurrentSolution.GetDocument

            Assert.IsFalse(miscFilesWorkspace.IsDocumentOpen(doc.Id))
            Assert.AreEqual(0, workspace.CurrentSolution.GetDocumentIdsWithFilePath(filePath).Length)

            openDocument miscFilesWorkspace doc.Id
            // Although we opened the document, this is false as it has been transferred to the other workspace.
            Assert.IsFalse(miscFilesWorkspace.IsDocumentOpen(doc.Id))

            Assert.AreEqual(0, miscFilesWorkspace.CurrentSolution.GetDocumentIdsWithFilePath(filePath).Length)
            Assert.AreEqual(1, workspace.CurrentSolution.GetDocumentIdsWithFilePath(filePath).Length)
            Assert.IsFalse(workspace.IsDocumentOpen(doc.Id))

        finally
            try File.Delete(filePath) with | _ -> ()
