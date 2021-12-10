// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Tests.ProjectSystem

open System
open System.IO
open NUnit.Framework
open UnitTests.TestLib.Utils.Asserts
open UnitTests.TestLib.ProjectSystem
open Microsoft.VisualStudio.FSharp.ProjectSystem


[<TestFixture>][<Category "ProjectSystem">]
type ProjectItems() = 
    inherit TheTests()
    
    //TODO: look for a way to remove the helper functions
    static let ANYTREE = Tree("",Nil,Nil)

    [<Test>]
    member public this.``RemoveAssemblyReference.NoIVsTrackProjectDocuments2Events``() =
        this.MakeProjectAndDo(["file.fs"], ["System.Numerics"],"", (fun project ->
            let listener = project.Site.GetService(typeof<Salsa.VsMocks.IVsTrackProjectDocuments2Listener>) :?> Salsa.VsMocks.IVsTrackProjectDocuments2Listener
            project.ComputeSourcesAndFlags()

            let containsSystemNumerics () = 
                project.CompilationOptions
                |> Array.exists (fun f -> f.IndexOf("System.Numerics") <> -1)

            let mutable wasCalled = false
            Assert.IsTrue(containsSystemNumerics (), "Project should contains reference to System.Numerics")

            let refContainer = project.GetReferenceContainer()
            let reference = 
                refContainer.EnumReferences() 
                |> Seq.find(fun r -> r.SimpleName = "System.Numerics")
            (
                use _guard = listener.OnAfterRemoveFiles.Subscribe(fun _ -> wasCalled <- true)
                reference.Remove(false)
            )

            Assert.IsFalse(wasCalled, "No events from IVsTrackProjectDocuments2 are expected")
            Assert.IsFalse(containsSystemNumerics(), "Project should not contains reference to System.Numerics")            
            ))

    [<Test>]
    member public this.``AddNewItem.ItemAppearsAtBottomOfFsprojFile``() =
        this.MakeProjectAndDo(["orig.fs"], [], "", (fun project ->
            let absFilePath = Path.Combine(project.ProjectFolder, "a.fs")
            try
                File.AppendAllText(absFilePath, "#light")
                // Note: this is not the same code path as the UI, but it is close
                project.MoveNewlyAddedFileToBottomOfGroup (fun () ->
                    project.AddNewFileNodeToHierarchy(project,absFilePath) |> ignore)
                let msbuildInfo = TheTests.MsBuildCompileItems(project.BuildProject)
                AssertEqual ["orig.fs"; "a.fs"] msbuildInfo
            finally
                File.Delete(absFilePath)
            ))

    [<Test>]
    member public this.``AddNewItem.ToAFolder.ItemAppearsAtBottomOfFolder``() =
        this.MakeProjectAndDo(["orig.fs"; "Folder\\f1.fs"; "Folder\\f2.fs"; "final.fs"], [], "", (fun project ->
            let dir = Path.Combine(project.ProjectFolder, "Folder")
            Directory.CreateDirectory(dir) |> ignore
            let absFilePath = Path.Combine(dir, "a.fs")
            try
                File.AppendAllText(absFilePath, "#light")
                // Note: this is not the same code path as the UI, but it is close
                project.MoveNewlyAddedFileToBottomOfGroup (fun () ->
                    project.AddNewFileNodeToHierarchy(project.FindChild("Folder"),absFilePath) |> ignore)
                let msbuildInfo = TheTests.MsBuildCompileItems(project.BuildProject)
                AssertEqual ["orig.fs"; "Folder\\f1.fs"; "Folder\\f2.fs"; "Folder\\a.fs"; "final.fs"] msbuildInfo
            finally
                File.Delete(absFilePath)
            ))
    
    [<Test>]
    member public this.``AddNewItemBelow.ItemAppearsInRightSpot``() =
        this.MakeProjectAndDo(["orig1.fs"; "orig2.fs"], [], "", (fun project ->
            let absFilePath = Path.Combine(project.ProjectFolder, "new.fs")
            try
                File.AppendAllText(absFilePath, "#light")
                // Note: this is not the same code path as the UI, but it is close
                let orig1 = TheTests.FindNodeWithCaption(project, "orig1.fs")
                project.MoveNewlyAddedFileBelow (orig1, fun () ->
                    project.AddNewFileNodeToHierarchy(project,absFilePath) |> ignore)
                // ensure right in .fsproj
                let msbuildInfo = TheTests.MsBuildCompileItems(project.BuildProject)
                AssertEqual ["orig1.fs"; "new.fs"; "orig2.fs"] msbuildInfo
                // ensure right in solution explorer
                let expect = Tree("References", ANYTREE,
                             Tree("orig1.fs", Nil,
                             Tree("new.fs", Nil,
                             Tree("orig2.fs", Nil, Nil))))
                TheTests.AssertSameTree(expect, project.FirstChild)
            finally
                File.Delete(absFilePath)
            ))
    
    [<Test>]
    member public this.``AddNewItemAbove.ItemAppearsInRightSpot.Case1``() =
        this.MakeProjectAndDo(["orig1.fs"; "orig2.fs"], [], "", (fun project ->
            let absFilePath = Path.Combine(project.ProjectFolder, "new.fs")
            try
                File.AppendAllText(absFilePath, "#light")
                // Note: this is not the same code path as the UI, but it is close
                let orig2 = TheTests.FindNodeWithCaption(project, "orig2.fs")
                project.MoveNewlyAddedFileAbove (orig2, fun () ->
                    project.AddNewFileNodeToHierarchy(project,absFilePath) |> ignore)
                // ensure right in .fsproj
                let msbuildInfo = TheTests.MsBuildCompileItems(project.BuildProject)
                AssertEqual ["orig1.fs"; "new.fs"; "orig2.fs"] msbuildInfo
                // ensure right in solution explorer
                let expect = Tree("References", ANYTREE,
                             Tree("orig1.fs", Nil,
                             Tree("new.fs", Nil,
                             Tree("orig2.fs", Nil, Nil))))
                TheTests.AssertSameTree(expect, project.FirstChild)
            finally
                File.Delete(absFilePath)
            ))
    
    [<Test>]
    member public this.``AddNewItemAbove.ItemAppearsInRightSpot.Case2``() =
        this.MakeProjectAndDo(["orig1.fs"; "orig2.fs"], [], "", (fun project ->
            let absFilePath = Path.Combine(project.ProjectFolder, "new.fs")
            try
                File.AppendAllText(absFilePath, "#light")
                // Note: this is not the same code path as the UI, but it is close
                let orig1 = TheTests.FindNodeWithCaption(project, "orig1.fs")
                project.MoveNewlyAddedFileAbove (orig1, fun () ->
                    project.AddNewFileNodeToHierarchy(project,absFilePath) |> ignore)
                // ensure right in .fsproj
                let msbuildInfo = TheTests.MsBuildCompileItems(project.BuildProject)
                AssertEqual ["new.fs"; "orig1.fs"; "orig2.fs"] msbuildInfo
                // ensure right in solution explorer
                let expect = Tree("References", ANYTREE,
                             Tree("new.fs", Nil,
                             Tree("orig1.fs", Nil,
                             Tree("orig2.fs", Nil, Nil))))
                TheTests.AssertSameTree(expect, project.FirstChild)
            finally
                File.Delete(absFilePath)
            ))
