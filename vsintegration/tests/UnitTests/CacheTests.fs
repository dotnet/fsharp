// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace Microsoft.VisualStudio.FSharp.Editor.Tests.Roslyn

open System.IO
open System.Threading
open NUnit.Framework
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open UnitTests.TestLib.LanguageService

[<TestFixture; Category "Roslyn and FCS Integration"; NonParallelizable>]
type CacheTests() =

    let testPath = """C:\test\"""
    let testFileName = "TestFile.fs"
    let testFilePath = Path.Combine(testPath, testFileName)
    let testFileSourceText = SourceText.From("""module TestProject.TestFile""")       

    let createTestProjectInfo name =
        let projectId = ProjectId.CreateNewId ()
        let documentInfos =
            [
                DocumentInfo.Create (DocumentId.CreateNewId (projectId), testFileName)
            ]
        let dllPath = Path.Combine(testPath, name + ".dll")
        let projectPath = Path.Combine(testPath, name + ".fsproj")
        ProjectInfo.Create (projectId, VersionStamp.Create (), name, name, (* Can't use "F#" due to exception *) "C#", documents = documentInfos, outputFilePath = dllPath, filePath = projectPath)

    let createSolutionInfoWithProjectCount projectCount =
        let testProjectInfos = [ for i = 1 to projectCount do yield createTestProjectInfo ("Test" + string i) ]
        let solutionInfo = SolutionInfo.Create(SolutionId.CreateNewId (), VersionStamp.Create (), projects = testProjectInfos)
        solutionInfo

    let createCompilationEnvWithProjectCount projectCount =
        let solutionInfo = createSolutionInfoWithProjectCount projectCount
        let projectInfos = solutionInfo.Projects
        let workspace = new AdhocWorkspace()
        workspace.AddSolution solutionInfo |> ignore
        
        let cenv = FSharpCompilationHelpers.createCompilationEnv workspace checker (* enableInMemoryCrossProjectReferences *) true
        
        // This will go away when HandleCommandLineChanges goes away.
        projectInfos
        |> Seq.iter (fun testProjectInfo ->
            cenv.cpsCommandLineOptions.[testProjectInfo.Id] <- ([|testFilePath|], [||])
        )

        (workspace, cenv)

    let basicTest () =
        let (workspace, cenv) = createCompilationEnvWithProjectCount 3 (* we chose 3 because the Checker's default project cache size is 3 by default *)
        let checker = cenv.checker

        checker.StopBackgroundCompile ()
        checker.InvalidateAll ()

        // We shouldn't have a current solution yet.
        Assert.True (cenv.currentSolution.IsNone) 

        let fsharpProjects =
            workspace.CurrentSolution.Projects
            |> Seq.map (fun project ->
                FSharpCompilationHelpers.tryComputeOptions cenv project CancellationToken.None |> Async.RunSynchronously
            )
            |> List.ofSeq

        // We still don't have a current solution yet.
        Assert.True (cenv.currentSolution.IsNone)

        FSharpCompilationHelpers.setSolution cenv workspace.CurrentSolution

        // We should now have a current solution.
        Assert.True (cenv.currentSolution.IsSome)

        Assert.True (fsharpProjects |> List.forall (fun x -> x.IsSome))

        let fsharpProjects = fsharpProjects |> List.map (fun x -> snd x.Value)

        Assert.True (fsharpProjects |> List.forall (fun options ->  not (checker.ProjectExistsInAnyCache options |> Async.RunSynchronously)))

        fsharpProjects
        |> List.iter (fun options ->
            // We don't care about the results, we only care that it will create an incremental builder for each project when we make a call.
            checker.TryParseAndCheckFileInProject(options, testFilePath, testFileSourceText, "CacheTests") |> Async.RunSynchronously |> ignore
        )

        // Projects should now exist in the cache.
        Assert.True (fsharpProjects |> List.forall (fun options ->  checker.ProjectExistsInAnyCache options |> Async.RunSynchronously))
        (workspace, cenv)

    [<Test>]
    member __.ProjectCacheRemoveInvdividuallyBySolutionChanges () =
        let (workspace, cenv) = basicTest ()
        let checker = cenv.checker

        let mutable solution = workspace.CurrentSolution
        let projectIds = solution.ProjectIds
        let fsharpProjects =
            projectIds
            |> Seq.map (fun projectId ->
                let (_, _, options) = cenv.cache.[projectId]
                options
            ) |> List.ofSeq

        Assert.AreEqual (3, projectIds.Count)

        for i = 0 to projectIds.Count - 1 do
            solution <- solution.RemoveProject projectIds.[i]
            FSharpCompilationHelpers.setSolution cenv solution

            // Remaining projects should still be in the cache.
            for i = i + 1 to projectIds.Count - 1 do
                Assert.True (cenv.cache.ContainsKey projectIds.[i])
                Assert.True (checker.ProjectExistsInAnyCache fsharpProjects.[i] |> Async.RunSynchronously)

            Assert.False (cenv.cache.ContainsKey projectIds.[i])
            Assert.False (checker.ProjectExistsInAnyCache fsharpProjects.[i] |> Async.RunSynchronously)

        workspace.Dispose ()

    [<Test>]
    member __.ProjectCacheRemoveAllByNewSolution () =
        let (workspace, cenv) = basicTest ()
        let checker = cenv.checker

        let solution = workspace.CurrentSolution
        let projectIds = solution.ProjectIds
        let fsharpProjects =
            projectIds
            |> Seq.map (fun projectId ->
                let (_, _, options) = cenv.cache.[projectId]
                options
            ) |> List.ofSeq

        Assert.AreEqual (3, projectIds.Count)

        workspace.ClearSolution ()
        let solution = workspace.AddSolution (createSolutionInfoWithProjectCount 0)

        FSharpCompilationHelpers.setSolution cenv solution

        for i = 0 to projectIds.Count - 1 do
            Assert.False (cenv.cache.ContainsKey projectIds.[i])
            Assert.False (checker.ProjectExistsInAnyCache fsharpProjects.[i] |> Async.RunSynchronously)

        workspace.Dispose ()
        
        
