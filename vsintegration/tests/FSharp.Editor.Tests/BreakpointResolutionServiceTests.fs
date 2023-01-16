// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests

open System
open Xunit
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor
open FSharp.Editor.Tests.Helpers
open FSharp.Compiler.CodeAnalysis

type BreakpointResolutionServiceTests() =

    let fileName = "C:\\test.fs"

    let projectOptions: FSharpProjectOptions =
        {
            ProjectFileName = "C:\\test.fsproj"
            ProjectId = None
            SourceFiles = [| fileName |]
            ReferencedProjects = [||]
            OtherOptions = [||]
            IsIncompleteTypeCheckEnvironment = true
            UseScriptResolutionRules = false
            LoadTime = DateTime.MaxValue
            OriginalLoadReferences = []
            UnresolvedReferences = None
            Stamp = None
        }
        
    let parsingOptions =
        { FSharpParsingOptions.Default with
            SourceFiles = [| fileName |]
        }

    let code =
        "
// This is a comment

type exampleType(parameter: int) =
    member this.exampleMember = parameter

[<EntryPoint>]
let main argv = 
    let integerValue = 123456
    let stringValue = \"This is a string\"
    let objectValue = exampleType(789)

    printfn \"%d %s %A\" integerValue stringValue objectValue

    let booleanValue = true
    match booleanValue with
    | true -> printfn \"correct\"
    | false -> printfn \"wrong\"

    0 // return an integer exit code
    "

    static member testCases: Object[][] =
        [|
            [| "This is a comment"; None |]
            [| "123456"; Some("let integerValue = 123456") |]
            [| "stringValue"; Some("let stringValue = \"This is a string\"") |]
            [| "789"; Some("let objectValue = exampleType(789)") |]
            [| "correct"; Some("printfn \"correct\"") |]
            [| "wrong"; Some("printfn \"wrong\"") |]
            [| "0"; Some("0") |]
        |]

    [<Theory>]
    [<MemberData(nameof(BreakpointResolutionServiceTests.testCases))>]
    member this.TestBreakpointResolution(searchToken: string, expectedResolution: string option) =
        let searchPosition = code.IndexOf(searchToken)
        Assert.True(searchPosition >= 0, $"SearchToken '{searchToken}' is not found in code")

        let document, sourceText =
            RoslynTestHelpers.CreateSingleDocumentSolution(fileName, code, projectOptions, parsingOptions)

        let searchSpan =
            TextSpan.FromBounds(searchPosition, searchPosition + searchToken.Length)

        let actualResolutionOption =
            FSharpBreakpointResolutionService.GetBreakpointLocation(document, searchSpan)
            |> Async.RunSynchronously

        match actualResolutionOption with
        | None -> Assert.True(expectedResolution.IsNone, "BreakpointResolutionService failed to resolve breakpoint position")
        | Some (actualResolutionRange) ->
            let actualResolution =
                sourceText
                    .GetSubText(RoslynHelpers.FSharpRangeToTextSpan(sourceText, actualResolutionRange))
                    .ToString()

            Assert.True(
                expectedResolution.IsSome,
                $"BreakpointResolutionService resolved a breakpoint while it shouldn't at: {actualResolution}"
            )

            Assert.Equal(expectedResolution.Value, actualResolution)
