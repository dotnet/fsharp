// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace Microsoft.VisualStudio.FSharp.Editor.Tests.Roslyn

open System
open System.Threading

open NUnit.Framework

open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Text

open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.LanguageService

open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.Range

[<TestFixture>]
type BreakpointResolutionServiceTests()  =

    let fileName = "C:\\test.fs"
    let options: FSharpProjectOptions = { 
        ProjectFileName = "C:\\test.fsproj"
        ProjectFileNames =  [| fileName |]
        ReferencedProjects = [| |]
        OtherOptions = [| |]
        IsIncompleteTypeCheckEnvironment = true
        UseScriptResolutionRules = false
        LoadTime = DateTime.MaxValue
        UnresolvedReferences = None
    }
    let code = "
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
    
    static member private testCases: Object[][] = [|
        [| "This is a comment";         None |]
        [| "123456";                    Some("let integerValue = 123456") |]
        [| "stringValue";               Some("let stringValue = \"This is a string\"") |]
        [| "789";                       Some("let objectValue = exampleType(789)") |]
        [| "correct";                   Some("printfn \"correct\"") |]
        [| "wrong";                     Some("printfn \"wrong\"") |]
        [| "0";                         Some("0") |]
    |]

    [<TestCaseSource("testCases")>]
    member this.TestBreakpointResolution(searchToken: string, expectedResolution: string option) = 
        let searchPosition = code.IndexOf(searchToken)
        Assert.IsTrue(searchPosition >= 0, "SearchToken '{0}' is not found in code", searchToken)
        
        let sourceText = SourceText.From(code)
        let searchSpan = TextSpan.FromBounds(searchPosition, searchPosition + searchToken.Length)
        let actualResolutionOption = FSharpBreakpointResolutionService.GetBreakpointLocation(sourceText, fileName, searchSpan, options) |> Async.RunSynchronously
        
        match actualResolutionOption with
        | None -> Assert.IsTrue(expectedResolution.IsNone, "BreakpointResolutionService failed to resolve breakpoint position")
        | Some(actualResolutionRange) ->
            let actualResolution = sourceText.GetSubText(CommonRoslynHelpers.FSharpRangeToTextSpan(sourceText, actualResolutionRange)).ToString()
            Assert.IsTrue(expectedResolution.IsSome, "BreakpointResolutionService resolved a breakpoint while it shouldn't at: {0}", actualResolution)
            Assert.AreEqual(expectedResolution.Value, actualResolution, "Expected and actual resolutions should match")
    