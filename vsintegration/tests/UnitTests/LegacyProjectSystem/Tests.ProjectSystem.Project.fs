// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Tests.ProjectSystem

// System namespaces
open System
open System.Collections.Generic
open System.IO
open System.Text.RegularExpressions
open System.Xml.Linq
open NUnit.Framework

// VS namespaces 
open Microsoft.VisualStudio
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.FSharp.ProjectSystem

// Internal unittest namespaces
open Salsa
open UnitTests.TestLib.Utils.Asserts
open UnitTests.TestLib.Utils.FilesystemHelpers
open UnitTests.TestLib.ProjectSystem


[<TestFixture>][<Category "ProjectSystem">]
type Project() = 
    inherit TheTests()


    //TODO: look for a way to remove the helper functions
    static let ANYTREE = Tree("",Nil,Nil)
    /////////////////////////////////
    // project helpers
    static let SaveProject(project : UnitTestingFSharpProjectNode) =
        project.Save(null, 1, 0u) |> ignore

    [<Test>]    
    member public _.NewFolderOnProjectMenu() =
        printfn "starting..."
        let package = new FSharpProjectPackage()
        let project = new FSharpProjectNode(package)
        let guidCmdGroup = VsMenus.guidStandardCommandSet97
        let cmdEnum = Microsoft.VisualStudio.VSConstants.VSStd97CmdID.NewFolder
        let mutable result = new QueryStatusResult()
        let (pCmdText : IntPtr) = 0n
        printfn "call qson..."
        let x = project.QueryStatusOnNode(guidCmdGroup, uint32(cmdEnum), pCmdText, &result)
        printfn "and..."
        AssertEqual x VSConstants.S_OK
        if (result &&& QueryStatusResult.ENABLED) <> QueryStatusResult.ENABLED then
            Assert.Fail("Unexpected: New Folder was not enabled")
        ()    

    [<Test>]
    member public this.``FsprojFileToSolutionExplorer.FileOrderInFsprojIsRespected.Case1``() =
        let compileItems = ["one.fs"; "two.fs"; "three.fs"]
        let expect = Tree("References", ANYTREE,
                     Tree("one.fs", Nil,
                     Tree("two.fs", Nil,
                     Tree("three.fs", Nil, Nil))))
        // We expect nodes in the solution explorer to appear in the same order as
        // the msbuild file - e.g. "one" "two" "three" rather than alphabetized
        // "one" "three" "two"
        this.``FsprojFileToSolutionExplorer.PositiveTest``(compileItems, "", expect)

    [<Test>]
    member public this.``FsprojFileToSolutionExplorer.FileOrderInFsprojIsRespected.Case2``() =
        let compileItems = [@"A\B\D\foo.fs"; @"A\B\C\bar.fs"]
        let expect = Tree("References", ANYTREE,
                     Tree("A", 
                         Tree("B",
                             Tree("D", 
                                 Tree("foo.fs", Nil, Nil),
                             Tree("C", 
                                 Tree("bar.fs", Nil, Nil),
                                 Nil)), Nil), Nil))
        // no alphabetization of files or folders
        this.``FsprojFileToSolutionExplorer.PositiveTest``(compileItems, "", expect)

    [<Test>]
    member public this.``FsprojFileToSolutionExplorer.FileOrderInFsprojIsRespected.Case3``() =
        let compileItems = [@"B\foo.fs"; @"A\bar.fs"]
        let other = @"
          <ItemGroup>
            <Folder Include='A' />
          </ItemGroup>
          "
        let expect = Tree("References", ANYTREE,
                     Tree("B",
                         Tree("foo.fs", Nil, Nil),
                     Tree("A", 
                         Tree("bar.fs", Nil, Nil),
                         Nil)))
        // Including folder should not put folder at front of other folders
        this.``FsprojFileToSolutionExplorer.PositiveTest``(compileItems, other, expect)

    [<Test>]
    member public this.``FsprojFileToSolutionExplorer.FileOrderInFsprojIsRespected.Case4``() =
        let compileItems = [@"foo.fs"; @"A\bar.fs"]
        let other = @"
          <ItemGroup>
            <Folder Include='A' />
          </ItemGroup>
          "
        let expect = Tree("References", ANYTREE,
                     Tree("foo.fs", Nil,
                     Tree("A", 
                         Tree("bar.fs", Nil, Nil),
                         Nil)))
        // Including folder should not put folder at front of files
        this.``FsprojFileToSolutionExplorer.PositiveTest``(compileItems, other, expect)


    [<Test>]
    member public this.``FsprojFileToSolutionExplorer.LinksIntoFoldersAreRespected``() =
        let compileItems = []
        let other = @"
          <ItemGroup>
            <Compile Include='foo.fs' />
            <Compile Include='..\bar.fs' >
                <Link>A\bar.fs</Link>
            </Compile>
            <Compile Include='A\qux.fs' />
          </ItemGroup>
          "
        let expect = Tree("References", ANYTREE,
                     Tree("foo.fs", Nil,
                     Tree("A", 
                         Tree("bar.fs", Nil,
                         Tree("qux.fs", Nil, Nil)),
                         Nil)))
        this.``FsprojFileToSolutionExplorer.PositiveTest``(compileItems, other, expect)


    [<Test>]
    member public this.``Links.AddLinkToRootWorks``() =
        let compileItems = [@"Folder\foo.fs"; @"bar.fs"; ]
        this.MakeProjectAndDoWithProjectFile(compileItems, [], "", (fun project fileName ->
            this.EnsureCausesNotification(project, fun() ->
                let f = System.IO.Path.GetFullPath(Path.Combine(project.ProjectFolder, @"..\qux.fs"))
                project.AddLinkedItem(project, [| f |], Array.create 1 (new VSADDRESULT())) |> ValidateOK
            )
            let expect = Tree("References", ANYTREE,
                         Tree("Folder", 
                            Tree("foo.fs",Nil,Nil), 
                         Tree("bar.fs", Nil,
                         Tree("qux.fs", Nil,Nil))))
            TheTests.AssertSameTree(expect, project.FirstChild)
            SaveProject(project)
            let fsprojFileText = File.ReadAllText(fileName)
            printfn "%s" fsprojFileText
            let regexStr = @"<ItemGroup>\s*<Compile Include=""Folder\\foo.fs"" />\s*<Compile Include=""bar.fs"" />\s*<Compile Include=""..\\qux.fs"">\s*<Link>qux.fs</Link>"
            TheTests.HelpfulAssertMatches '<' regexStr fsprojFileText
        ))

    [<Test>]
    member public this.``Links.AddLinkToSubfolderWorks``() =
        let compileItems = [@"bar.fs"; @"Folder\foo.fs"; ]
        this.MakeProjectAndDoWithProjectFile(compileItems, [], "", (fun project fileName ->
            let folder = TheTests.FindNodeWithCaption(project, "Folder")
            this.EnsureCausesNotification(project, fun() ->
                let f = System.IO.Path.GetFullPath(Path.Combine(project.ProjectFolder, @"..\qux.fs"))
                project.AddLinkedItem(folder, [| f |], Array.create 1 (new VSADDRESULT())) |> ValidateOK
            )
            let expect = Tree("References", ANYTREE,
                         Tree("bar.fs", Nil,
                         Tree("Folder", 
                            Tree("foo.fs", Nil,
                            Tree("qux.fs", Nil,Nil)), Nil)))
            TheTests.AssertSameTree(expect, project.FirstChild)
            SaveProject(project)
            let fsprojFileText = File.ReadAllText(fileName)
            printfn "%s" fsprojFileText
            let regexStr = @"<ItemGroup>\s*<Compile Include=""bar.fs"" />\s*<Compile Include=""Folder\\foo.fs"" />\s*<Compile Include=""..\\qux.fs"">\s*<Link>Folder\\qux.fs</Link>"
            TheTests.HelpfulAssertMatches '<' regexStr fsprojFileText
        ))

    [<Test>]
    member public this.``Links.AddLinkToRootWorksForNonFsFile``() =
        let compileItems = [@"Folder\foo.fs"; @"bar.fs"; ]
        this.MakeProjectAndDoWithProjectFile(compileItems, [], "", (fun project fileName ->
            this.EnsureCausesNotification(project, fun() ->
                // Note: this is not the same code path as the UI, but it is close
                project.MoveNewlyAddedFileToBottomOfGroup (fun () ->
                    let f = System.IO.Path.GetFullPath(Path.Combine(project.ProjectFolder, @"..\qux.resx"))
                    project.AddLinkedItem(project, [| f |], Array.create 1 (new VSADDRESULT())) |> ValidateOK
                )
            )
            let expect = Tree("References", ANYTREE,
                         Tree("Folder", 
                            Tree("foo.fs",Nil,Nil), 
                         Tree("bar.fs", Nil,
                         Tree("qux.resx", Nil,Nil))))
            TheTests.AssertSameTree(expect, project.FirstChild)
            SaveProject(project)
            let fsprojFileText = File.ReadAllText(fileName)
            printfn "%s" fsprojFileText
            let regexStr = @"<ItemGroup>\s*<Compile Include=""Folder\\foo.fs"" />\s*<Compile Include=""bar.fs"" />\s*<EmbeddedResource Include=""..\\qux.resx"">\s*<Link>qux.resx</Link>"
            TheTests.HelpfulAssertMatches '<' regexStr fsprojFileText
        ))
    
    [<Test>]
    member public this.``Removal.ExcludeFileShouldDirtyProjectFileAndBeSeenOnDiskAfterSave``() =
        let items = MSBuildItems([CompileItem "foo.fs"; CompileItem "bar.fs"])
        this.MakeProjectAndDoWithProjectFile([], [], items.ToString(), (fun project fileName ->
            let toVerify = @"<Compile Include=""foo.fs"""
            // ensure is there to start
            SaveProject(project)
            let fsprojFileText = File.ReadAllText(fileName)
            printfn "%s" fsprojFileText
            AssertEqualMsg true (fsprojFileText.Contains(toVerify)) "original assumption of this test was invalid"
            // remove it
            let foo = TheTests.FindNodeWithCaption(project, "foo.fs")
            foo.Remove(false) // false='removeFromStorage' - thus this is like 'Exclude from project'
            // ensure things are right
            AssertEqualMsg true project.IsProjectFileDirty "the project file was not dirtied"
            SaveProject(project)
            let fsprojFileText = File.ReadAllText(fileName)
            printfn "%s" fsprojFileText
            AssertEqualMsg false (fsprojFileText.Contains(toVerify)) "it was not removed from the .fsproj on disk"
        ))

    [<Test>]
    member public this.``Removal.RemoveReferenceShouldDirtyProjectFileAndBeSeenOnDiskAfterSave``() =
        let items = MSBuildItems([CompileItem "foo.fs"; CompileItem "bar.fs"])
        this.MakeProjectAndDoWithProjectFile([], ["System"], items.ToString(), (fun project fileName ->
            let toVerify = @"<Reference Include=""System"""
            // ensure is there to start
            SaveProject(project)
            let fsprojFileText = File.ReadAllText(fileName)
            printfn "%s" fsprojFileText
            AssertEqualMsg true (fsprojFileText.Contains(toVerify))  "original assumption of this test was invalid"
            // remove it
            let refSystem = TheTests.FindNodeWithCaption(project, "System")
            refSystem.Remove(false)
            // ensure things are right
            AssertEqualMsg true project.IsProjectFileDirty "the project file was not dirtied"
            SaveProject(project)
            let fsprojFileText = File.ReadAllText(fileName)
            printfn "%s" fsprojFileText
            AssertEqualMsg false (fsprojFileText.Contains(toVerify)) "it was not removed from the .fsproj on disk"
        ))

    [<Test>]
    member public this.``FsprojFileToSolutionExplorer.FileMovement.MoveUpShouldDirtyProject``() =
        let items = MSBuildItems([CompileItem "foo.fs"; CompileItem "bar.fs"])
        this.MakeProjectAndDoWithProjectFile([], [], items.ToString(), (fun project fileName ->
            // Save the project first, then move the file, and check for dirty.
            SaveProject(project)
            let foo = TheTests.FindNodeWithCaption(project, "foo.fs")
            let bar = TheTests.FindNodeWithCaption(project, "bar.fs")
            TheTests.MoveUp(bar)
            AssertEqual true project.IsProjectFileDirty
            // Tests the tree
            let expect = Tree("References", ANYTREE,
                         Tree("bar.fs", Nil,
                         Tree("foo.fs", Nil, Nil)))
            TheTests.AssertSameTree(expect, project.FirstChild)
        ))

    [<Test>]
    member public this.``FsprojFileToSolutionExplorer.FileMovement.MoveDownShouldDirtyProject``() =
        let items = MSBuildItems([CompileItem "foo.fs"; CompileItem "bar.fs"])
        this.MakeProjectAndDoWithProjectFile([], [], items.ToString(), (fun project fileName ->
            // Save the project first, then move the file, and check for dirty.
            SaveProject(project)
            let foo = TheTests.FindNodeWithCaption(project, "foo.fs")
            let bar = TheTests.FindNodeWithCaption(project, "bar.fs")
            TheTests.MoveDown(foo)
            AssertEqual true project.IsProjectFileDirty
            // Tests the tree
            let expect = Tree("References", ANYTREE,
                         Tree("bar.fs", Nil,
                         Tree("foo.fs", Nil, Nil)))
            TheTests.AssertSameTree(expect, project.FirstChild)
        ))

    member this.SampleEntities = [CompileItem @"foo.fs"; FolderItem @"AnEmptyFolder\"; LinkedCompileItem(@"..\blah.fs", @"link.fs"); OtherItem(@"Content", @"foo.txt"); OtherItem(@"FsLex", @"lex.mll")]

    member private this.SampleFileEntity = ([CompileItem "bar.fs"], fun t -> Tree("bar.fs", Nil, t))
    
    member private this.SampleEmptyFolderEntity = ([FolderItem @"MyFolder\"], fun t -> Tree("MyFolder", Nil, t))

    member private this.SampleFolderWithItemsEntity = ([CompileItem @"MyFolder\x1.fs"; CompileItem @"MyFolder\Sub\x2.fs"; CompileItem @"MyFolder\x3.fs"], 
                                                       fun t -> Tree("MyFolder", 
                                                                    Tree("x1.fs", Nil, 
                                                                    Tree("Sub", 
                                                                        Tree("x2.fs", Nil, Nil), 
                                                                    Tree("x3.fs", Nil, Nil))),
                                                                t))

    [<Test>]
    member public this.``SpecificVersion.OptionsSavedToFsprojFile``() =
        let items = MSBuildItems( [CompileItem "foo.fs"] )
        this.MakeProjectAndDoWithProjectFile([], ["System"], items.ToString(), (fun project fileName ->
            let WithName name seq = Seq.filter (fun (e:XElement) -> e.Name.LocalName = name) seq
            let WithAttrName name seq = Seq.filter (fun (e:XAttribute) -> e.Name.LocalName = name) seq
            let system = TheTests.FindNodeWithCaption(project, "System")
            let system = system :?> AssemblyReferenceNode
            let a = [| false, (fun (e:XElement) -> 
                            let expected = XDocument.Load(new StringReader(@"<Reference Include=""System""><SpecificVersion>False</SpecificVersion></Reference>")).Root
                            TheTests.AssertSimilarXml(expected, e))
                       true, (fun (e:XElement) -> 
                            let expected = XDocument.Load(new StringReader(@"<Reference Include=""ANY""><SpecificVersion>True</SpecificVersion></Reference>")).Root
                            TheTests.AssertSimilarXml(expected, e)
                            let inc = e.Attributes() |> WithAttrName "Include" |> Seq.head
                            Assert.IsTrue(inc.Value.StartsWith("System, Version", StringComparison.Ordinal), "assembly reference lacks version"))
                       false, (fun (e:XElement) -> 
                            let expected = XDocument.Load(new StringReader(@"<Reference Include=""ANY""><SpecificVersion>False</SpecificVersion></Reference>")).Root
                            TheTests.AssertSimilarXml(expected, e)
                            let inc = e.Attributes() |> WithAttrName "Include" |> Seq.head
                            Assert.IsTrue(inc.Value.StartsWith("System, Version", StringComparison.Ordinal), "assembly reference lacks version")) 
                    |]
            let props = system.NodeProperties :?> AssemblyReferenceProperties
            for v, f in a do
                props.SpecificVersion <- v
                // test that it manifests properly in .fsproj file
                SaveProject(project)
                let fsprojFileText = File.ReadAllText(fileName)
                let xDoc = XDocument.Load(new StringReader(fsprojFileText))
                let refNode = xDoc.Descendants() |> WithName "Reference" |> Seq.head 
                printfn "%s" fsprojFileText
                f refNode
        ))
    
    [<Test>]
    member public this.``FsprojFileToSolutionExplorer.FileRenaming.RenamingAFileDoesNotChangeOrderInSolutionExplorerOrMSBuild``() =
        for entity, treeMaker in [this.SampleFileEntity; this.SampleEmptyFolderEntity] do
            let items = MSBuildItems( [CompileItem "foo.fs"] @ entity )
            this.MakeProjectAndDoWithProjectFile([], [], items.ToString(), (fun project fileName ->
                // ensure things look right at start
                let expect = Tree("References", ANYTREE,
                             Tree("foo.fs", Nil,
                             treeMaker(Nil)))
                TheTests.AssertSameTree(expect, project.FirstChild)
                let foo = TheTests.FindNodeWithCaption(project, "foo.fs")
                // rename it
                VsMocks.vsRunningDocumentTableFindAndLockDocumentVsHierarchyMock <- project
                (foo :?> FileNode).RenameFileNode(project.ProjectFolder + "\\foo.fs", project.ProjectFolder + "\\zzz.fs", foo.Parent.ID) |> ignore
                // test that it did not move in solution explorer
                let expect = Tree("References", ANYTREE,
                             Tree("zzz.fs", Nil,
                             treeMaker(Nil)))
                TheTests.AssertSameTree(expect, project.FirstChild) 
                // test that it did not move in MSBuild
                SaveProject(project)
                let fsprojFileText = File.ReadAllText(fileName)
                printfn "%s" fsprojFileText
                let expectedItems = MSBuildItems( [CompileItem "zzz.fs"] @ entity )
                let regexStr = expectedItems.AsRegexString()
                TheTests.HelpfulAssertMatches '<' regexStr fsprojFileText
            ))

    member public this.``FsprojFileToSolutionExplorer.FileMovement.EntityCanBeMovedUpAbove``(otherEntity) =
        let (otherEntityItems, otherTreeMaker) = otherEntity
        for entity in this.SampleEntities do
            printfn "=========> testing moving %s" (entity.ToString())
            let items = MSBuildItems( otherEntityItems @ [entity] )
            this.MakeProjectAndDoWithProjectFile([], [], items.ToString(), (fun project fileName ->
                // ensure things look right at start
                let expect = Tree("References", ANYTREE,
                             otherTreeMaker(
                             Tree(entity.Caption(), Nil, Nil)))
                TheTests.AssertSameTree(expect, project.FirstChild)
                let foo = TheTests.FindNodeWithCaption(project, entity.Caption())
                TheTests.EnsureMoveUpEnabled(foo)
                // move it up
                TheTests.MoveUp(foo)
                // test that it moved up in solution explorer
                let expect = Tree("References", ANYTREE,
                             Tree(entity.Caption(), Nil,
                             otherTreeMaker(Nil)))
                TheTests.AssertSameTree(expect, project.FirstChild) 
                // test that it moved up in MSBuild
                SaveProject(project)
                let fsprojFileText = File.ReadAllText(fileName)
                printfn "%s" fsprojFileText
                let expectedItems = MSBuildItems( [entity] @ otherEntityItems )
                let regexStr = expectedItems.AsRegexString()
                TheTests.HelpfulAssertMatches '<' regexStr fsprojFileText
            ))

    [<Test>]
    member public this.``FsprojFileToSolutionExplorer.FileMovement.EntityCanBeMovedUpAboveFile``() =
        this.``FsprojFileToSolutionExplorer.FileMovement.EntityCanBeMovedUpAbove``(this.SampleFileEntity)

    [<Test>]
    member public this.``FsprojFileToSolutionExplorer.FileMovement.EntityCanBeMovedUpAboveEmptyFolder``() =
        this.``FsprojFileToSolutionExplorer.FileMovement.EntityCanBeMovedUpAbove``(this.SampleEmptyFolderEntity)

    [<Test>]
    member public this.``FsprojFileToSolutionExplorer.FileMovement.EntityCanBeMovedUpAboveFolderWithItems``() =
        this.``FsprojFileToSolutionExplorer.FileMovement.EntityCanBeMovedUpAbove``(this.SampleFolderWithItemsEntity)

    member public this.``FsprojFileToSolutionExplorer.FileMovement.EntityCanBeMovedDownBelow``(otherEntity) =
        let (otherEntityItems, otherTreeMaker) = otherEntity
        for entity in this.SampleEntities do
            printfn "=========> testing moving %s" (entity.ToString())
            let items = MSBuildItems( entity :: otherEntityItems )
            this.MakeProjectAndDoWithProjectFile([], [], items.ToString(), (fun project fileName ->
                // ensure things look right at start
                let expect = Tree("References", ANYTREE,
                             Tree(entity.Caption(), Nil,
                             otherTreeMaker(Nil)))
                TheTests.AssertSameTree(expect, project.FirstChild)
                let foo = TheTests.FindNodeWithCaption(project, entity.Caption())
                TheTests.EnsureMoveDownEnabled(foo)
                // move it down
                TheTests.MoveDown(foo)
                // test that it moved down in solution explorer
                let expect = Tree("References", ANYTREE,
                             otherTreeMaker(
                             Tree(entity.Caption(), Nil, Nil)))
                TheTests.AssertSameTree(expect, project.FirstChild) 
                // test that it moved down in MSBuild
                SaveProject(project)
                let fsprojFileText = File.ReadAllText(fileName)
                printfn "%s" fsprojFileText
                let expectedItems = MSBuildItems( otherEntityItems @ [entity] )
                let regexStr = expectedItems.AsRegexString()
                TheTests.HelpfulAssertMatches '<' regexStr fsprojFileText
            ))

    [<Test>]
    member public this.``FsprojFileToSolutionExplorer.FileMovement.EntityCanBeMovedDownBelowFile``() =
        this.``FsprojFileToSolutionExplorer.FileMovement.EntityCanBeMovedDownBelow``(this.SampleFileEntity)

    [<Test>]
    member public this.``FsprojFileToSolutionExplorer.FileMovement.EntityCanBeMovedDownBelowEmptyFolder``() =
        this.``FsprojFileToSolutionExplorer.FileMovement.EntityCanBeMovedDownBelow``(this.SampleEmptyFolderEntity)

    [<Test>]
    member public this.``FsprojFileToSolutionExplorer.FileMovement.EntityCanBeMovedDownBelowFolderWithItems``() =
        this.``FsprojFileToSolutionExplorer.FileMovement.EntityCanBeMovedDownBelow``(this.SampleFolderWithItemsEntity)

    member public this.``FsprojFileToSolutionExplorer.FileMovement.FolderWithItemsCanBeMovedUpAbove``(otherEntity) =
        let (otherEntityItems, otherTreeMaker) = otherEntity
        let folderA = [CompileItem @"A\foo.fs"; CompileItem @"A\B\qux.fs"; FolderItem @"A\Empty\"; CompileItem @"A\zot.fs"]
        let after = [CompileItem "after.fs"]
        let items = MSBuildItems( otherEntityItems @ folderA @ after )
        this.MakeProjectAndDoWithProjectFile([], [], items.ToString(), (fun project fileName ->
            // ensure things look right at start
            let expect = Tree("References", ANYTREE,
                         otherTreeMaker(
                         Tree("A", 
                             Tree("foo.fs", Nil,
                             Tree("B",
                                 Tree("qux.fs", Nil, Nil),
                             Tree("Empty", Nil,
                             Tree("zot.fs", Nil, Nil)))),
                         Tree("after.fs", Nil, Nil))))
            TheTests.AssertSameTree(expect, project.FirstChild)
            let foo = TheTests.FindNodeWithCaption(project, "A")
            TheTests.EnsureMoveUpEnabled(foo)
            // move it up
            TheTests.MoveUp(foo)
            // test that it moved up in solution explorer
            let expect = Tree("References", ANYTREE,
                         Tree("A", 
                             Tree("foo.fs", Nil,
                             Tree("B",
                                 Tree("qux.fs", Nil, Nil),
                             Tree("Empty", Nil,
                             Tree("zot.fs", Nil, Nil)))),
                         otherTreeMaker(
                         Tree("after.fs", Nil, Nil))))
            TheTests.AssertSameTree(expect, project.FirstChild) 
            // test that it moved up in MSBuild
            SaveProject(project)
            let fsprojFileText = File.ReadAllText(fileName)
            printfn "%s" fsprojFileText
            let expectedItems = MSBuildItems( folderA @ otherEntityItems @ after )
            let regexStr = expectedItems.AsRegexString()
            TheTests.HelpfulAssertMatches '<' regexStr fsprojFileText
        ))

    [<Test>]
    member public this.``FsprojFileToSolutionExplorer.FileMovement.FolderWithItemsCanBeMovedUpAboveFile``() =
        this.``FsprojFileToSolutionExplorer.FileMovement.FolderWithItemsCanBeMovedUpAbove``(this.SampleFileEntity)

    [<Test>]
    member public this.``FsprojFileToSolutionExplorer.FileMovement.FolderWithItemsCanBeMovedUpAboveEmptyFolder``() =
        this.``FsprojFileToSolutionExplorer.FileMovement.FolderWithItemsCanBeMovedUpAbove``(this.SampleEmptyFolderEntity)

    [<Test>]
    member public this.``FsprojFileToSolutionExplorer.FileMovement.FolderWithItemsCanBeMovedUpAboveFolderWithItems``() =
        this.``FsprojFileToSolutionExplorer.FileMovement.FolderWithItemsCanBeMovedUpAbove``(this.SampleFolderWithItemsEntity)

    member public this.``FsprojFileToSolutionExplorer.FileMovement.FolderWithItemsCanBeMovedDownBelow``(otherEntity) =
        let (otherEntityItems, otherTreeMaker) = otherEntity
        let folderA = [CompileItem @"A\foo.fs"; CompileItem @"A\B\qux.fs"; FolderItem @"A\Empty\"; CompileItem @"A\zot.fs"]
        let after = [CompileItem "after.fs"]
        let items = MSBuildItems( folderA @ otherEntityItems @ after )
        this.MakeProjectAndDoWithProjectFile([], [], items.ToString(), (fun project fileName ->
            // ensure things look right at start
            let expect = Tree("References", ANYTREE,
                         Tree("A", 
                             Tree("foo.fs", Nil,
                             Tree("B",
                                 Tree("qux.fs", Nil, Nil),
                             Tree("Empty", Nil,
                             Tree("zot.fs", Nil, Nil)))),
                         otherTreeMaker(
                         Tree("after.fs", Nil, Nil))))
            TheTests.AssertSameTree(expect, project.FirstChild)
            let foo = TheTests.FindNodeWithCaption(project, "A")
            TheTests.EnsureMoveDownEnabled(foo)
            // move it down
            TheTests.MoveDown(foo)
            // test that it moved down in solution explorer
            let expect = Tree("References", ANYTREE,
                         otherTreeMaker(
                         Tree("A", 
                             Tree("foo.fs", Nil,
                             Tree("B",
                                 Tree("qux.fs", Nil, Nil),
                             Tree("Empty", Nil,
                             Tree("zot.fs", Nil, Nil)))),
                         Tree("after.fs", Nil, Nil))))
            TheTests.AssertSameTree(expect, project.FirstChild) 
            // test that it moved down in MSBuild
            SaveProject(project)
            let fsprojFileText = File.ReadAllText(fileName)
            printfn "%s" fsprojFileText
            let expectedItems = MSBuildItems( otherEntityItems @ folderA @ after )
            let regexStr = expectedItems.AsRegexString()
            TheTests.HelpfulAssertMatches '<' regexStr fsprojFileText
        ))

    [<Test>]
    member public this.``FsprojFileToSolutionExplorer.FileMovement.FolderWithItemsCanBeMovedDownBelowFile``() =
        this.``FsprojFileToSolutionExplorer.FileMovement.FolderWithItemsCanBeMovedDownBelow``(this.SampleFileEntity)

    [<Test>]
    member public this.``FsprojFileToSolutionExplorer.FileMovement.FolderWithItemsCanBeMovedDownBelowEmptyFolder``() =
        this.``FsprojFileToSolutionExplorer.FileMovement.FolderWithItemsCanBeMovedDownBelow``(this.SampleEmptyFolderEntity)

    [<Test>]
    member public this.``FsprojFileToSolutionExplorer.FileMovement.FolderWithItemsCanBeMovedDownBelowFolderWithItems``() =
        this.``FsprojFileToSolutionExplorer.FileMovement.FolderWithItemsCanBeMovedDownBelow``(this.SampleFolderWithItemsEntity)

    [<Test>]
    member public this.``FsprojFileToSolutionExplorer.FileMovement.NegativeTests.EntityCannotBeMovedAboveReferences``() =
        for entity in this.SampleEntities do
            printfn "=========> testing moving %s" (entity.ToString())
            let items = MSBuildItems [entity; CompileItem "bar.fs"]
            use project = this.MakeProject([], [], items.ToString())
            let expect = Tree("References", ANYTREE,
                         Tree(entity.Caption(), Nil,
                         Tree("bar.fs", Nil, Nil)))
            TheTests.AssertSameTree(expect, project.FirstChild)
            let foo = TheTests.FindNodeWithCaption(project, entity.Caption())
            TheTests.EnsureMoveUpDisabled(foo)

    [<Test>]
    member public this.``FsprojFileToSolutionExplorer.FileMovement.NegativeTests.EntityCannotBeMovedUpWhenTopOfFolder``() =
        for entity in this.SampleEntities |> List.map (fun e -> e.IntoFolder(@"Folder\")) do
            printfn "=========> testing moving %s" (entity.ToString())
            let items = MSBuildItems [FolderItem @"Folder\"; entity]
            use project = this.MakeProject([], [], items.ToString())
            let expect = Tree("References", ANYTREE,
                         Tree("Folder",
                            Tree(entity.Caption(), Nil, Nil), Nil))
            TheTests.AssertSameTree(expect, project.FirstChild)
            let bar = TheTests.FindNodeWithCaption(project, entity.Caption())
            TheTests.EnsureMoveUpDisabled(bar)

    [<Test>]
    member public this.``FsprojFileToSolutionExplorer.FileMovement.NegativeTests.EntityCannotBeMovedDownWhenBottomOfFolder``() =
        for entity in this.SampleEntities |> List.map (fun e -> e.IntoFolder(@"Folder\")) do
            printfn "=========> testing moving %s" (entity.ToString())
            let items = MSBuildItems [FolderItem @"Folder\"; entity]
            use project = this.MakeProject([], [], items.ToString())
            let expect = Tree("References", ANYTREE,
                         Tree("Folder",
                            Tree(entity.Caption(), Nil, Nil), Nil))
            TheTests.AssertSameTree(expect, project.FirstChild)
            let bar = TheTests.FindNodeWithCaption(project, entity.Caption())
            TheTests.EnsureMoveDownDisabled(bar)

    [<Test>]
    member public this.``RenameFile.FailureToRenameInRDT.Bug616680.EnsureRevertToKnownConsistentState``() =
        this.MakeProjectAndDo(["orig1.fs"], [], "", (fun project ->
            let absFilePath = Path.Combine(project.ProjectFolder, "orig1.fs")
            try
                File.AppendAllText(absFilePath, "#light")
                let orig1 = TheTests.FindNodeWithCaption(project, "orig1.fs")
                VsMocks.vsRunningDocumentTableNextRenameDocumentCallThrows <- true
                VsMocks.vsRunningDocumentTableFindAndLockDocumentVsHierarchyMock <- project
                try
                    ErrorHandler.ThrowOnFailure(orig1.SetEditLabel("orig2.fs")) |> ignore  // rename the file
                    Assert.Fail("expected an exception")
                with 
                    | e -> printfn "Got expected exception: %s" e.Message
                SaveProject(project)
                // TODO ensure no events were fired
                // ensure right in .fsproj
                let msbuildInfo = TheTests.MsBuildCompileItems(project.BuildProject)
                AssertEqual ["orig1.fs"] msbuildInfo
                // ensure right in solution explorer
                let expect = Tree("References", ANYTREE,
                             Tree("orig1.fs", Nil, Nil))
                TheTests.AssertSameTree(expect, project.FirstChild)
            finally
                File.Delete(absFilePath)
            ))
    
    [<Test>]
    member public this.``RenameFile.FailureToRenameInRDT.Bug616680.EnsureThatFileOrderDidNotChange``() =
        this.MakeProjectAndDo(["a.fs";"b.fs";"orig1.fs";"c.fs";"d.fs"], [], "", (fun project ->
            let absFilePath = Path.Combine(project.ProjectFolder, "orig1.fs")
            try
                File.AppendAllText(absFilePath, "#light")
                let orig1 = TheTests.FindNodeWithCaption(project, "orig1.fs")
                VsMocks.vsRunningDocumentTableNextRenameDocumentCallThrows <- true
                VsMocks.vsRunningDocumentTableFindAndLockDocumentVsHierarchyMock <- project
                try
                    ErrorHandler.ThrowOnFailure(orig1.SetEditLabel("orig2.fs")) |> ignore  // rename the file
                    Assert.Fail("expected an exception")
                with 
                    | e -> printfn "Got expected exception: %s" e.Message
                SaveProject(project)
                // TODO ensure no events were fired
                // ensure right in .fsproj
                let msbuildInfo = TheTests.MsBuildCompileItems(project.BuildProject)
                AssertEqual ["a.fs";"b.fs";"orig1.fs";"c.fs";"d.fs"] msbuildInfo
                // ensure right in solution explorer
                let expect = Tree("References", ANYTREE,
                             Tree("a.fs", Nil,
                             Tree("b.fs", Nil,
                             Tree("orig1.fs", Nil, 
                             Tree("c.fs", Nil,
                             Tree("d.fs", Nil, Nil))))))
                TheTests.AssertSameTree(expect, project.FirstChild)
            finally
                File.Delete(absFilePath)
            ))

    [<Test>]
    member public this.``RenameFile.VerifyItemIdsRemainsTheSame``() =
        let name1 = "orig.fs"
        let name2 = "orig2.fs"
        this.MakeProjectAndDo([name1], [], "", (fun project ->
            let absFilePath = Path.Combine(project.ProjectFolder, name1)
            try
                File.AppendAllText(absFilePath, "#light")
                let orig1 = TheTests.FindNodeWithCaption(project, name1)
                VsMocks.vsRunningDocumentTableFindAndLockDocumentVsHierarchyMock <- project
                try
                    ErrorHandler.ThrowOnFailure(orig1.SetEditLabel(name2)) |> ignore  // rename the file
                with 
                    | e -> Assert.Fail("no exception expected")

                let orig2 = TheTests.FindNodeWithCaption(project, name2)
                AssertEqual orig1.ID orig2.ID
                
            finally
                File.Delete(absFilePath)
            ))
    
    [<Test>]
    member public this.``RenameFile.MainlineSuccessCase``() =
        this.MakeProjectAndDo(["orig1.fs"], [], "", (fun project ->
            let absFilePath = Path.Combine(project.ProjectFolder, "orig1.fs")
            try
                File.AppendAllText(absFilePath, "#light")
                let orig1 = TheTests.FindNodeWithCaption(project, "orig1.fs")
                VsMocks.vsRunningDocumentTableFindAndLockDocumentVsHierarchyMock <- project
                try
                    ErrorHandler.ThrowOnFailure(orig1.SetEditLabel("orig2.fs")) |> ignore  // rename the file
                with 
                    | e -> Assert.Fail("no exception expected")
                SaveProject(project)
                // TODO ensure IVsHierarchyEvents Delete/Add was fired
                // TODO ensure IVsTrackProjectDocumentsEvents Renamed was fired
                // ensure right in .fsproj
                let msbuildInfo = TheTests.MsBuildCompileItems(project.BuildProject)
                AssertEqual ["orig2.fs"] msbuildInfo
                // ensure right in solution explorer
                let expect = Tree("References", ANYTREE,
                             Tree("orig2.fs", Nil, Nil))
                TheTests.AssertSameTree(expect, project.FirstChild)
            finally
                File.Delete(absFilePath)
            ))
    
    [<Test>] //ref bug https://github.com/dotnet/fsharp/issues/259
    member public this.``RenameFile.InFolder``() =
        this.MakeProjectAndDo(["file1.fs"; @"Folder1\file2.fs"; @"Folder1\nested1.fs"], [], "", (fun project ->
            let absFilePath = Path.Combine(project.ProjectFolder, "Folder1", "nested1.fs")
            try
                Directory.CreateDirectory(Path.GetDirectoryName(absFilePath)) |> ignore;
                File.AppendAllText(absFilePath, "#light")
                let orig1 = TheTests.FindNodeWithCaption(project, "nested1.fs")
                let folder1 = TheTests.FindNodeWithCaption(project, "Folder1")
                VsMocks.vsRunningDocumentTableFindAndLockDocumentVsHierarchyMock <- project
                
                let added, deleted = ResizeArray(), ResizeArray()

                let sink = 
                    { new IVsHierarchyEvents with
                        member x.OnInvalidateIcon _hicon = VSConstants.S_OK
                        member x.OnInvalidateItems _itemidParent = VSConstants.S_OK
                        member x.OnItemAdded (itemidParent, itemidSiblingPrev, itemidAdded) = 
                            added.Add(itemidParent, itemidSiblingPrev, itemidAdded)
                            VSConstants.S_OK
                        member x.OnItemDeleted (itemid) =
                            deleted.Add(itemid)
                            VSConstants.S_OK
                        member x.OnItemsAppended (itemidParent) =
                            VSConstants.S_OK
                        member x.OnPropertyChanged (itemid, propid, flags) =
                            VSConstants.S_OK }

                let cookie = ref 0u
                project.AdviseHierarchyEvents(sink, cookie) |> ErrorHandler.ThrowOnFailure |> ignore

                // rename the file
                orig1.SetEditLabel("renamedNested2.fs") |> ErrorHandler.ThrowOnFailure |> ignore

                SaveProject project

                let file2 = TheTests.FindNodeWithCaption (project, "file2.fs")
                let renamedNested2 = TheTests.FindNodeWithCaption (project, "renamedNested2.fs")

                AssertEqual [ folder1.ID, file2.ID, renamedNested2.ID ] (added |> Seq.distinct |> List.ofSeq)
                AssertEqual [ orig1.ID ] (deleted |> Seq.distinct |> List.ofSeq)

                // TODO ensure IVsTrackProjectDocumentsEvents Renamed was fired

                // ensure right in .fsproj
                let msbuildInfo = TheTests.MsBuildCompileItems(project.BuildProject)
                AssertEqual ["file1.fs"; @"Folder1\file2.fs"; @"Folder1\renamedNested2.fs"] msbuildInfo

                // ensure right in solution explorer
                let expect = 
                    Tree("References", ANYTREE,
                    Tree("file1.fs", Nil,
                    Tree("Folder1", 
                       Tree("file2.fs", Nil,
                       Tree("renamedNested2.fs", Nil, Nil)),
                    Nil)))
                TheTests.AssertSameTree (expect, project.FirstChild)

            finally
                if File.Exists(absFilePath) then File.Delete(absFilePath)
            ))
    
(* Disabled for now - see https://github.com/dotnet/fsharp/pull/3071 - this is testing old project system features

    [<Test>]
    member public this.``RenameFile.BuildActionIsResetBasedOnFilenameExtension``() =
        let GetTextFromBuildAction (action:VSLangProj.prjBuildAction) =
            match action with
            | VSLangProj.prjBuildAction.prjBuildActionCompile -> "Compile"
            | VSLangProj.prjBuildAction.prjBuildActionContent -> "Content"
            | VSLangProj.prjBuildAction.prjBuildActionEmbeddedResource -> "EmbeddedResource"
            |_ -> "None"
        let COMPILE = "Compile"
        let NONE = "None"
        let EMBEDDEDRESOURCE = "EmbeddedResource"
        let CONTENT = "Content"
        // briefly, we just want out-of-the-box defaults to be like C#, with simpler logic, so we just hardcode a few values to be like C# and then otherwise default to NONE
        let defaultBuildActionTable = dict [ "config", CONTENT
                                             "css", CONTENT
                                             "map", CONTENT
                                             "manifest", NONE
                                             "bmp", CONTENT
                                             "cd", NONE
                                             "cur", CONTENT
                                             "xsd", NONE
                                             "ico", CONTENT
                                             "js", CONTENT
                                             "sync", NONE
                                             "tt", NONE
                                             "resx", EMBEDDEDRESOURCE
                                             "ruleset", NONE
                                             "settings", NONE
                                             "txt", CONTENT
                                             "wsf", CONTENT
                                             "xml", CONTENT
                                             "ml", COMPILE
                                             "mli", COMPILE
                                             "fs", COMPILE
                                             "fsi", COMPILE
                                             "fsx", NONE
                                             "fsscript", NONE
                                             "someunexpectedgobbledegook", NONE ]
        // A rename that changes a file extension will pop up a modal dialog box, this tells the mock to say 'yes' to the dialog
        VsMocks.vsUIShellShowMessageBoxResult <- Some 6 // IDYES = 6

        this.MakeProjectAndDo(["foo.fs"], [], "", (fun project ->
            let Absolutize fileName = Path.Combine(project.ProjectFolder, fileName)
            let mutable currentAbsoluteFilePath = Absolutize "foo.fs"
            File.AppendAllText(currentAbsoluteFilePath, "// dummy content")
            try
                for KeyValue(extension, buildAction) in defaultBuildActionTable do
                    let node = TheTests.FindNodeWithCaption(project, Path.GetFileName(currentAbsoluteFilePath))
                    let newFileName = Absolutize("foo." + extension)
                    VsMocks.vsRunningDocumentTableFindAndLockDocumentVsHierarchyMock <- project
                    try
                        ErrorHandler.ThrowOnFailure(node.SetEditLabel(Path.GetFileName(newFileName))) |> ignore  // rename the file
                    with 
                        | e -> Assert.Fail("no exception expected, but got " + e.ToString())
                    
                    // check that the OM has the updated build action
                    let node = TheTests.FindNodeWithCaption(project, Path.GetFileName(newFileName)) :?> FSharpFileNode
                    let props = node.NodeProperties :?> FSharpFileNodeProperties 
                    AssertEqual buildAction (GetTextFromBuildAction props.BuildAction)
                    
                    // check that the build action in the .fsproj file has the expected value
                    SaveProject(project)
                    let fsprojFileText = File.ReadAllText(Absolutize project.ProjectFile)
                    printfn "%s" fsprojFileText
                    let expectedRegexStr = "<" + buildAction + " Include=\"" + Path.GetFileName(newFileName) + "\" />"
                    TheTests.HelpfulAssertMatches '<' expectedRegexStr fsprojFileText
                    
                    currentAbsoluteFilePath <- newFileName
            finally
                VsMocks.vsUIShellShowMessageBoxResult <- None
                File.Delete(currentAbsoluteFilePath) 
        ))
*)


    [<Test>]
    member public this.``FsprojOutputWindow.ErrorOriginColumnsAreBase1``() =
        let (outputWindowPaneErrors : string list ref) = ref [] // output window pane errors
        let vso = VsMocks.vsOutputWindowPane(outputWindowPaneErrors)
        let compileItem = ["foo.fs"]
        let expectedError = "foo\.fs\(1,1\): error FS0039: The value or constructor 'bar' is not defined." // expected error
        
        DoWithTempFile "Test.fsproj" (fun projFile ->
            File.AppendAllText(projFile, TheTests.SimpleFsprojText(compileItem, [], ""))
            use project = TheTests.CreateProject(projFile)
            let srcFile = (Path.GetDirectoryName projFile) + "\\" + "foo.fs"
            File.AppendAllText(srcFile, "bar") ; // foo.fs will be cleaned up by parent call to DoWithTempFile
            project.BuildToOutput("Build", vso, null) |> ignore // Build the project using vso as the output logger
        
            let errors = List.filter (fun s -> (new Regex(expectedError)).IsMatch(s)) !outputWindowPaneErrors
        
            for e in errors do
                printfn "Output Window Pane Error: %s" e
                
            // there should be one and only one error for 'bar', located at (1,1)
            AssertEqual (List.length errors) 1
        )

    [<Test>]
    member public this.``FsprojOutputWindow.HighUnicodeCharactersAreProperlyDisplayed``() =
        let (outputWindowPaneErrors : string list ref) = ref [] // output window pane errors
        let vso = VsMocks.vsOutputWindowPane(outputWindowPaneErrors)
        let compileItem = ["新規bcrogram.fs"]
        let expectedError = "新規bcrogram\.fs\(1,1\): error FS0039: The value or constructor 'bar' is not defined" // expected error

        DoWithTempFile "Test.fsproj" (fun projFile ->
            File.AppendAllText(projFile, TheTests.SimpleFsprojText(compileItem, [], ""))
            use project = TheTests.CreateProjectWithUTF8Output(projFile)
            let srcFile = (Path.GetDirectoryName projFile) + "\\" + "新規bcrogram.fs"
            File.AppendAllText(srcFile, "bar") ;            // 新規bcrogram.fs will be cleaned up by parent call to DoWithTempFile
            project.BuildToOutput("Build", vso, null) |> ignore   // Build the project using vso as the output logger

            // The console inserts hard line breaks accumulate the output as a single line then look for the expected output.
            let output = (!outputWindowPaneErrors |> String.concat "").Replace("\r\n", "")
            let errors = Regex.IsMatch(output, expectedError, RegexOptions.CultureInvariant)

            // there should be one and only one error for 'bar', located at (1,1)
            AssertEqual errors true
            ()
        )
