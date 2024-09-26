// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Tests.ProjectSystem

open System
open System.IO
open System.Text.RegularExpressions
open Xunit
open UnitTests.TestLib.Utils.Asserts
open UnitTests.TestLib.Utils.FilesystemHelpers
open UnitTests.TestLib.ProjectSystem
open Microsoft.VisualStudio.FSharp.ProjectSystem


type RoundTrip() = 
    inherit TheTests()
    
    /////////////////////////////////
    // project helpers
    static let SaveProject(project : UnitTestingFSharpProjectNode) =
        project.Save(null, 1, 0u) |> ignore
    
    member this.``FsprojRoundtrip.PositiveTest``(origItems : MSBuildItems, expectedItems : MSBuildItems) =
        // test that opening with origItems and saving yields expectedItems
        this.MakeProjectAndDoWithProjectFile([], [], origItems.ToString(), (fun project fileName ->
            SaveProject(project)
            let fsprojFileText = File.ReadAllText(fileName)
            printfn "%s" fsprojFileText
            let regexStr = expectedItems.AsRegexString()
            TheTests.HelpfulAssertMatches '<' regexStr fsprojFileText
            if Regex.IsMatch(fsprojFileText, "<ItemGroup>\s*</ItemGroup>") then
                Assert.Fail("did not remove empty ItemGroups")
        ))
        // test idempotency (opening with previous-saved results causes no change)
        this.MakeProjectAndDoWithProjectFile([], [], expectedItems.ToString(), (fun project fileName ->
            SaveProject(project)
            let fsprojFileText = File.ReadAllText(fileName)
            printfn "%s" fsprojFileText
            let regexStr = expectedItems.AsRegexString()
            TheTests.HelpfulAssertMatches '<' regexStr fsprojFileText
            if Regex.IsMatch(fsprojFileText, "<ItemGroup>\s*</ItemGroup>") then
                Assert.Fail("did not remove empty ItemGroups")
        ))

    [<Fact>]
    member public this.``FsprojRoundTrip.Basic.NonemptyFoldersRemoved.Case1``() =
        this.``FsprojRoundtrip.PositiveTest``(
            MSBuildItems [CompileItem @"bar.fs"
                          FolderItem @"Folder\"
                          CompileItem @"Folder\foo.fs"],
            MSBuildItems [CompileItem @"bar.fs"
                          CompileItem @"Folder\foo.fs"])

    [<Fact>]
    member public this.``FsprojRoundTrip.Basic.NonemptyFoldersRemoved.Case2``() =
        this.``FsprojRoundtrip.PositiveTest``(
            MSBuildItems [CompileItem @"bar.fs"
                          FolderItem @"A\"
                          FolderItem @"A\B\"
                          FolderItem @"A\B\C\"
                          CompileItem @"A\B\C\foo.fs"
                          CompileItem @"A\qux.fs"],
            MSBuildItems [CompileItem @"bar.fs"
                          CompileItem @"A\B\C\foo.fs"
                          CompileItem @"A\qux.fs"])

    [<Fact>]
    member public this.``FsprojRoundTrip.ComplexButLegalCase``() =
        let items = MSBuildItems [CompileItem @"A\B\foo.fs"
                                  CompileItem @"A\bar.fs"
                                  CompileItem @"A\C\D\qux.fs"
                                  CompileItem @"A\xyz.fs"
                                  CompileItem @"ddd.fs"
                                  CompileItem @"E\eee.fs"
                                  ]
        this.``FsprojRoundtrip.PositiveTest``(items, items)

    [<Fact>]
    member public this.``FsprojRoundTrip.EmptyFoldersArePreservedWhenRestIsIdempotent``() =
        let items = MSBuildItems [CompileItem @"bar.fs"
                                  FolderItem @"A\Empty1\"
                                  CompileItem @"A\B\C\foo.fs"
                                  FolderItem @"A\B\Empty2\"
                                  CompileItem @"A\qux.fs"]
        this.``FsprojRoundtrip.PositiveTest``(items, items)

    [<Fact>]
    member public this.``FsprojRoundTrip.EmptyFoldersArePreservedWhenRestIsLegalButNotIdempotent``() =
        let origItems = [CompileItem @"bar.fs"
                         FolderItem @"A\Empty1\"        
                         FolderItem @"A\B\"              // will get removed
                         CompileItem @"A\B\C\foo.fs"
                         FolderItem @"A\B\Empty2\"
                         CompileItem @"A\qux.fs"]
        let expectedItems = [CompileItem @"bar.fs"
                             FolderItem @"A\Empty1\"
                             CompileItem @"A\B\C\foo.fs"
                             FolderItem @"A\B\Empty2\"
                             CompileItem @"A\qux.fs"]
        this.``FsprojRoundtrip.PositiveTest``(MSBuildItems origItems, MSBuildItems expectedItems)

    [<Fact>]
    member public this.``FsprojRoundTrip.Regression.FoldersWithSameName``() =
        let items = MSBuildItems [CompileItem @"First\Second\bar.fs"
                                  CompileItem @"Second\qux.fs"]
        this.``FsprojRoundtrip.PositiveTest``(items, items)

    [<Fact>]
    member public this.``FsprojRoundTrip.Regression.FoldersWithSameName2``() =
        let items = MSBuildItems [CompileItem @"First\First\bar.fs"]
        this.``FsprojRoundtrip.PositiveTest``(items, items)

    member this.``Fsproj.NegativeTest``(items : MSBuildItems) =
        DoWithTempFile "Test.fsproj" (fun file ->
            File.AppendAllText(file, TheTests.SimpleFsprojText([], [], items.ToString()))
            let ex = new System.Exception()
            try
                use project = TheTests.CreateProject(file)
                let mutable node = project.FirstChild
                TheTests.PrintHierarchy(node)
                raise ex
            with
            | e when obj.ReferenceEquals(e,ex) -> Assert.Fail("did not expect to succeed creating project")
            | :? InvalidOperationException as e when e.ToString().Contains("rendered") ->
                printfn "As expected, failed to create project.  Reason: %s" (e.Message)
            | e -> Assert.Fail("failed to create project, but for wrong reason")
            ()
        )

    [<Fact>]
    member public this.``FsprojRoundTrip.Basic.Invalid.Case1``() =
        let items = MSBuildItems [CompileItem @"A\B\C\foo.fs"
                                  CompileItem @"B\bar.fs"
                                  CompileItem @"A\B\D\qux.fs"]  // would cause A to be rendered twice
        this.``Fsproj.NegativeTest`` items

    [<Fact>]
    member public this.``FsprojRoundTrip.Basic.Invalid.Case2``() =
        let items = MSBuildItems [CompileItem @"A\foo.fs"
                                  CompileItem @"bar.fs"
                                  CompileItem @"A\qux.fs"]  // would cause A to be rendered twice
        this.``Fsproj.NegativeTest`` items

    // REVIEW NYI: [<Fact>]
    member public this.``FsprojRoundTrip.Basic.Invalid.Case3``() =
        let items = MSBuildItems [CompileItem @"A\foo.fs"
                                  FolderItem @"A\"           // <Folder> must be before anything below it
                                  CompileItem @"bar.fs"]
        this.``Fsproj.NegativeTest`` items
        
