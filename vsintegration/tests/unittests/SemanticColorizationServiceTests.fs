// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace Microsoft.VisualStudio.FSharp.Editor.Tests.Roslyn

open System
open NUnit.Framework
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text

[<TestFixture>][<Category "Roslyn Services">]
type SemanticClassificationServiceTests()  =
    let filePath = "C:\\test.fs"

    let projectOptions = { 
        ProjectFileName = "C:\\test.fsproj"
        ProjectId = None
        SourceFiles =  [| filePath |]
        ReferencedProjects = [| |]
        OtherOptions = [| |]
        IsIncompleteTypeCheckEnvironment = true
        UseScriptResolutionRules = false
        LoadTime = DateTime.MaxValue
        UnresolvedReferences = None
        OriginalLoadReferences = []
        ExtraProjectInfo = None
        Stamp = None
    }

    let checker = FSharpChecker.Create()

    let getRanges (sourceText: string) : (Range.range * SemanticClassificationType) list =
        asyncMaybe {
            let! _, _, checkFileResults = checker.ParseAndCheckDocument(filePath, 0, sourceText, projectOptions, false, "")
            return checkFileResults.GetSemanticClassification(None)
        } 
        |> Async.RunSynchronously
        |> Option.toList
        |> List.collect Array.toList

    let verifyColorizerAtEndOfMarker(fileContents: string, marker: string, classificationType: string) =
        let text = SourceText.From fileContents
        let ranges = getRanges fileContents
        let line = text.Lines.GetLineFromPosition (fileContents.IndexOf(marker) + marker.Length - 1)
        let col = line.ToString().IndexOf(marker) + marker.Length - 1
        let markerPos = Range.mkPos (Range.Line.toZ line.LineNumber) col
        printfn "parker pos: %A, ranges: %A" markerPos ranges
        match ranges |> List.tryFind (fun (range, _) -> Range.rangeContainsPos range markerPos) with
        | None -> Assert.Fail("Cannot find colorization data for end of marker")
        | Some(_, ty) -> Assert.AreEqual(classificationType, FSharpClassificationTypes.getClassificationTypeName ty, "Classification data doesn't match for end of marker")

    [<TestCase("(*1*)Guid", FSharpClassificationTypes.ReferenceType)>]
    [<TestCase("(*2*)string", FSharpClassificationTypes.ReferenceType)>]
    [<TestCase("(*3*)Guid", FSharpClassificationTypes.ValueType)>]
    [<TestCase("(*4*)string", FSharpClassificationTypes.ReferenceType)>]
    [<TestCase("(*5*)int", FSharpClassificationTypes.ValueType)>]
    [<TestCase("(*6*)Guid", FSharpClassificationTypes.ValueType)>]
    [<TestCase("(*7*)string", FSharpClassificationTypes.ReferenceType)>]
    member public this.Measured_Types(marker: string, classificationType: string) =
        verifyColorizerAtEndOfMarker(
             """#light (*Light*)
                open System
                
                [<MeasureAnnotatedAbbreviation>] type (*1*)Guid<[<Measure>] 'm> = Guid
                [<MeasureAnnotatedAbbreviation>] type (*2*)string<[<Measure>] 'm> = string
                
                let inline cast<'a, 'b> (a : 'a) : 'b = (# "" a : 'b #)
                
                type Uom =
                    static member inline tag<[<Measure>]'m> (x : Guid) : (*3*)Guid<'m> = cast x
                    static member inline tag<[<Measure>]'m> (x : string) : (*4*)string<'m> = cast x
                
                type [<Measure>] Ms
                
                let i: (*5*)int<Ms> = 1<Ms>
                let g: (*6*)Guid<Ms> = Uom.tag Guid.Empty
                let s: (*7*)string<Ms> = Uom.tag "foo" """,
            marker, 
            classificationType)