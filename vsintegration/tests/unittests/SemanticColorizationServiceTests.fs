// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace Microsoft.VisualStudio.FSharp.Editor.Tests.Roslyn

open System
open NUnit.Framework
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler
open Microsoft.CodeAnalysis.Text

[<TestFixture; Category "Roslyn Services">]
type SemanticClassificationServiceTests() =
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
    let perfOptions = { LanguageServicePerformanceOptions.Default with AllowStaleCompletionResults = false }

    let getRanges (sourceText: string) : (Range.range * SemanticClassificationType) list =
        asyncMaybe {

            let! _, _, checkFileResults = checker.ParseAndCheckDocument(filePath, 0, sourceText, projectOptions, perfOptions, "")
            return checkFileResults.GetSemanticClassification(None)
        } 
        |> Async.RunSynchronously
        |> Option.toList
        |> List.collect Array.toList

    let verifyColorizerAtEndOfMarker(fileContents: string, marker: string, classificationType: string) =
        let text = SourceText.From fileContents
        let ranges = getRanges fileContents
        let line = text.Lines.GetLinePosition (fileContents.IndexOf(marker) + marker.Length - 1)
        let markerPos = Range.mkPos (Range.Line.fromZ line.Line) (line.Character + marker.Length - 1)
        match ranges |> List.tryFind (fun (range, _) -> Range.rangeContainsPos range markerPos) with
        | None -> Assert.Fail("Cannot find colorization data for end of marker")
        | Some(_, ty) -> Assert.AreEqual(classificationType, FSharpClassificationTypes.getClassificationTypeName ty, "Classification data doesn't match for end of marker")

    [<TestCase("(*1*)", FSharpClassificationTypes.ValueType)>]
    [<TestCase("(*2*)", FSharpClassificationTypes.ReferenceType)>]
    [<TestCase("(*3*)", FSharpClassificationTypes.ValueType)>]
    [<TestCase("(*4*)", FSharpClassificationTypes.ReferenceType)>]
    [<TestCase("(*5*)", FSharpClassificationTypes.ValueType)>]
    [<TestCase("(*6*)", FSharpClassificationTypes.ValueType)>]
    [<TestCase("(*7*)", FSharpClassificationTypes.ReferenceType)>]
    member __.Measured_Types(marker: string, classificationType: string) =
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