// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace UnitTests.Tests.ProjectSystem

open System
open NUnit.Framework
open UnitTests.TestLib.Utils
open Microsoft.VisualStudio.FSharp.ProjectSystem

[<TestFixture>]
[<Category("wip")>]
module MSBuildUtilitiesTest = 
    open System.Xml
    open System.IO
    open UnitTests.TestLib

    open Microsoft.Build.Evaluation
    open Microsoft.Build.Construction
    open RoseTreeTestHelpers
    
    let private emptyMSBuildProject () =
        let xml = ProjectSystem.TheTests.SimpleFsprojText([],[],"")
        use xmlReader = new StringReader(xml) |> XmlReader.Create
        let prj = Microsoft.Build.Evaluation.Project(xmlReader)
        let itemgroup = prj.Xml.ItemGroups |> Seq.head
        prj,itemgroup

    let private addItems (itemgroup: ProjectItemGroupElement) items =
        let elements = 
            items 
            |> List.map (fun (itemType,name) -> itemgroup.ContainingProject.CreateItemElement(itemType,name))
        elements |> List.iter itemgroup.AppendChild
        elements

    let uri x = Uri(x, UriKind.Relative)
    let compile, folder = ProjectFileConstants.Compile, ProjectFileConstants.Folder
    let content = ProjectFileConstants.Content
    let folder' name = ProjectTreeNode.Folder name
    let compile' name x = ProjectTreeNode.Item (name, { Item = x; Include = uri name; Content = MSBuildProjectItem.Compile; Link = None })

    open MSBuildHelpers

    [<Test>]
    let ``EnsureProperFolderLogic throw if invalid folder logic`` () =
        let prj, itemgroup = emptyMSBuildProject ()
        
        let items =
            [ compile, @"project\src\Core\first.fs"
              compile, @"project\src\second.fs"
              compile, @"project\src\Core\subdir\third.fs" ]
            |> addItems itemgroup

        try
            let validatedGroup = EnsureProperFolderLogic prj itemgroup true
            Assert.Fail("exception expected")
        with :? InvalidOperationException as ex ->
            StringAssert.Contains(@"'project\src\Core'", ex.Message)

        //if exception is thrown, itemgroup is unchanged
        List.ofSeq itemgroup.Items
        |> Asserts.AssertEqual items 

    [<Test>]
    let ``EnsureProperFolderLogic valid remove unused folders from group`` () =
        let prj, itemgroup = emptyMSBuildProject ()
        
        let items =
            [ (*  0 *) compile, @"project\src\Core\first.fs"
              (*  1 *) folder,  @"project\src\Core"
              (*  2 *) compile, @"project\src\Core\second.fs"
              (*  3 *) compile, @"project\src\Core\subdir\third.fs"
              (*  4 *) compile, @"project\README.md"
              (*  5 *) folder,  @"project\tests\"
              (*  6 *) folder,  @"project\lib\"
              (*  7 *) folder,  @"project\lib\"
              (*  8 *) compile, @"project\README.md"
              (*  9 *) content, @"project\README.md"
              (* 10 *) compile, @"project\docs\readme.txt"
              (* 11 *) folder,  @"project\docs\examples\" 
              (* 12 *) compile, @"project\docs\examples\sample.md"
              (* 13 *) folder,  @"project\docs\" ]
            |> addItems itemgroup

        let validatedGroup = EnsureProperFolderLogic prj itemgroup true

        let foldersToRemove = [
            1;  // because project\src\Core\first.fs
            7;  // because project\lib ( 6 )
            11; // because project\docs\examples\sample.md
            13; // because project\docs\* ( like 10, 11, 12 )
            ]

        let expectedRemovedFolders =
            foldersToRemove |> List.map (List.nth items)
        let others =
            [0 .. items.Length - 1]
            |> List.filter (fun x -> not (List.exists ((=) x) foldersToRemove))
            |> List.map (List.nth items)

        Linq.Enumerable.Intersect(expectedRemovedFolders |> Seq.ofList, validatedGroup.Items)
        |> List.ofSeq
        |> Asserts.AssertEqual []

        others
        |> Asserts.AssertEqual (validatedGroup.Items |> List.ofSeq)
