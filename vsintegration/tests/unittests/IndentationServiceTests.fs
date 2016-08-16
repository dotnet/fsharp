// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace Microsoft.VisualStudio.FSharp.Editor.Tests.Roslyn

open System
open System.Threading

open NUnit.Framework

open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor

[<TestFixture>]
type IndentationServiceTests()  =

    static let tabSize = 4

    static let consoleProjectTemplate = "
// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

[<EntryPoint>]
let main argv = 
    printfn \"%A\" argv
    0 // return an integer exit code"

    static let libraryProjectTemplate = "
namespace ProjectNamespace

type Class1() = 
    member this.X = \"F#\""

    static let nestedTypesTemplate = "
namespace testspace
    type testtype
        static member testmember = 1"
    
    static member private testCases: Object[][] = [|
        [| None;     0; consoleProjectTemplate |]
        [| Some(0);  1; consoleProjectTemplate |]
        [| Some(0);  2; consoleProjectTemplate |]
        [| Some(0);  3; consoleProjectTemplate |]
        [| Some(0);  4; consoleProjectTemplate |]
        [| Some(0);  5; consoleProjectTemplate |]
        [| Some(0);  6; consoleProjectTemplate |]
        [| Some(4);  7; consoleProjectTemplate |]
        [| Some(4);  8; consoleProjectTemplate |]
        
        [| None;     0; libraryProjectTemplate |]
        [| Some(0);  1; libraryProjectTemplate |]
        [| Some(0);  2; libraryProjectTemplate |]
        [| Some(0);  3; libraryProjectTemplate |]
        [| Some(0);  4; libraryProjectTemplate |]
        [| Some(4);  5; libraryProjectTemplate |]
        
        [| None;     0; nestedTypesTemplate |]
        [| Some(0);  1; nestedTypesTemplate |]
        [| Some(0);  2; nestedTypesTemplate |]
        [| Some(4);  3; nestedTypesTemplate |]
        [| Some(8);  4; nestedTypesTemplate |]
    |]

    [<TestCaseSource("testCases")>]
    member this.TestIndentation(expectedIndentation: Option<int>, lineNumber: int, template: string) = 
        let actualIndentation = FSharpIndentationService.GetDesiredIndentation(SourceText.From(template), lineNumber, tabSize)
        match expectedIndentation with
        | None -> Assert.IsTrue(actualIndentation.IsNone, "No indentation was expected at line {0}", lineNumber)
        | Some(indentation) -> Assert.AreEqual(expectedIndentation.Value, actualIndentation.Value, "Indentation on line {0} doesn't match", lineNumber)
    