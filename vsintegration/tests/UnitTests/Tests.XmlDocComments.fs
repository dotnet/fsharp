// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Tests.LanguageService

open System
open Xunit
open Salsa.Salsa
open Salsa.VsOpsUtils
open UnitTests.TestLib.Salsa
open UnitTests.TestLib.Utils

type XmlDocComments() =
    inherit UnitTests.TestLib.LanguageService.LanguageServiceBaseTests(VsOpts = InstalledMSBuildTestFlavour())

    // Work around an innocuous 'feature' with how QuickInfo is displayed, lines which
    // should have a "\r\n" just have a "\r"
    let trimnewlines (str : string) =
        str.Replace("\r", "").Replace("\n", "")

    member public this.AssertQuickInfoContainsAtStartOfMarker code marker expected =
        let (_solution, _project, file) = this.CreateSingleFileProject(code : string)
        MoveCursorToStartOfMarker(file, marker)
        let tooltip = GetQuickInfoAtCursor file 
        printfn "%A" tooltip
        AssertContains(trimnewlines tooltip, trimnewlines expected) 

    member private this.TestMalFormedXML(marker : string) =   
        let fileContent1 = """
            namespace XML
            module Doc =
                /// <summary>
                /// Adds two <see cref="T:System.Int32" /> values.
                /// </summary>
                /// <param name="x">The first value to sum</param>
                /// <param name="x">The second value to sum</param
                let Add x y= x + y"""
        let fileContent2 = """
            module XMLDoc
            /// <summary>
            /// Subtract two <see cref="T:System.Int32" /> values.
            /// <summary>
            let Subtract x y = x - y

            /// <summary>
            /// Multiply two <see cref="T:System.Int32" /> values.
            /// </summary>
            /// <param name="x">The first value to sum</param>
            /// <param name="x">The second value to sum</param>
            let Multiply x y = x * y

            let foo = XML.Doc.Add(*Marker1*)
            let bar = Subtract(*Marker2*) 3 2
            let baz = Multiply(*Marker3*) 2 3"""
        use _guard = this.UsingNewVS()
        let solution = this.CreateSolution()
        let project1 = CreateProject(solution, "FSLibrary")
        let project2 = CreateProject(solution, "FSClient")
        let file1 = AddFileFromTextBlob(project1,"File1.fs", fileContent1)
        AddProjectReference(project2,project1)       
        Build(project1) |> fun result -> Assert.True(result.BuildSucceeded)

        let file2 = AddFileFromTextBlob(project2,"File2.fs", fileContent2)
        let file = OpenFile(project2, "File2.fs")
        MoveCursorToStartOfMarker(file, marker)
        GetQuickInfoAtCursor file

    [<Fact(Skip = "GetQuickInfoAtCursor miss XMLDoc analyzing")>]
    member this.``MalFormedXML.FromXMLDoc``() = 
        let expected = "XML comment"
        let tooltip = this.TestMalFormedXML("(*Marker1*)")
        printfn "%A" tooltip
        AssertContains(trimnewlines tooltip, trimnewlines expected) 

    [<Fact(Skip = "GetQuickInfoAtCursor miss XMLDoc analyzing")>]
    member this.``MalFormedXML.FromCurrentProject``() = 
        let expected = "'summary'"
        let tooltip = this.TestMalFormedXML("(*Marker2*)")
        printfn "%A" tooltip
        AssertContains(trimnewlines tooltip, trimnewlines expected) 

    [<Fact(Skip = "GetQuickInfoAtCursor miss XMLDoc analyzing")>]
    member this.``MalFormedXML.NoXMLComment.Bug5858``() = 
        let notexpected = "summary"
        let notexpected2 = "param name="
        let tooltip = this.TestMalFormedXML("(*Marker3*)")
        printfn "%A" tooltip
        AssertNotContains(tooltip, notexpected)   
        AssertNotContains(tooltip, notexpected2)   

    [<Fact(Skip = "GetQuickInfoAtCursor miss XMLDoc analyzing")>]
    member this.Test() = 
        let fileContent = """
            //local custom type value
            /// <summary>
            /// local custom value
            /// </summary>
            let customType(*Mark*) = new CustomType()"""
        this.AssertQuickInfoContainsAtStartOfMarker fileContent "(*Mark*)" "local custom value"