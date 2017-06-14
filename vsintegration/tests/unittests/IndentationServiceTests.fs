// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace Microsoft.VisualStudio.FSharp.Editor.Tests.Roslyn

open System
open System.Threading

open NUnit.Framework

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.FSharp.Compiler.SourceCodeServices

[<TestFixture>][<Category "Roslyn Services">]
type IndentationServiceTests()  =
    let filePath = "C:\\test.fs"
    let options: FSharpProjectOptions = { 
        ProjectFileName = "C:\\test.fsproj"
        SourceFiles =  [| filePath |]
        ReferencedProjects = [| |]
        OtherOptions = [| |]
        IsIncompleteTypeCheckEnvironment = true
        UseScriptResolutionRules = false
        LoadTime = DateTime.MaxValue
        OriginalLoadReferences = []
        UnresolvedReferences = None
        ExtraProjectInfo = None
        Stamp = None
    }
    let documentId = DocumentId.CreateNewId(ProjectId.CreateNewId())

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
        static member testmember = 1

"

    static let autoIndentTemplate = "
let plus x y =
    x + y // $Indent: 4$

let mutable x = 0
x <-     
    10 * 2 // $Indent: 4$

match some 10 with
| none -> 0
| some x -> 
    x + 1 // $Indent: 4$

try
    failwith \"fail\" // $Indent: 4$
with
    | :? system.exception -> \"error\"

if 10 > 0 then
    true // $Indent: 4$
else
    false // $Indent: 4$

(
    1, // $Indent: 4$
    2
)

[
    1 // $Indent: 4$
    2
]

[|
    1 // $Indent: 4$
    2
|]

[<
    literal // $Indent: 4$
>]
let constx = 10

let t = seq { // $Indent: 0$
    yield 1 // $Indent: 4$
}

let g = function
    | none -> 1 // $Indent: 4$
    | some _ -> 0

module mymodule = begin
end // $Indent: 4$

type mytype() = class
end // $Indent: 4$

type mystruct = struct
end // $Indent: 4$

while true do
    printfn \"never end\" // $Indent: 4$

// After line has keyword in comment such as function
// should not be indented $Indent: 0$
"
    
    static member private testCases: Object[][] = [|
        [| None;     0; consoleProjectTemplate |]
        [| None;     1; consoleProjectTemplate |]
        [| Some(0);  2; consoleProjectTemplate |]
        [| Some(0);  3; consoleProjectTemplate |]
        [| Some(0);  4; consoleProjectTemplate |]
        [| Some(0);  5; consoleProjectTemplate |]
        [| Some(4);  6; consoleProjectTemplate |]
        [| Some(4);  7; consoleProjectTemplate |]
        [| Some(4);  8; consoleProjectTemplate |]
        
        [| None;     0; libraryProjectTemplate |]
        [| None;     1; libraryProjectTemplate |]
        [| Some(0);  2; libraryProjectTemplate |]
        [| Some(0);  3; libraryProjectTemplate |]
        [| Some(4);  4; libraryProjectTemplate |]
        [| Some(4);  5; libraryProjectTemplate |]
        
        [| None;     0; nestedTypesTemplate |]
        [| None;     1; nestedTypesTemplate |]
        [| Some(0);  2; nestedTypesTemplate |]
        [| Some(4);  3; nestedTypesTemplate |]
        [| Some(8);  4; nestedTypesTemplate |]
        [| Some(8);  5; nestedTypesTemplate |]
    |]

    [<TestCaseSource("testCases")>]
    member this.TestIndentation(expectedIndentation: Option<int>, lineNumber: int, template: string) = 
        let actualIndentation = FSharpIndentationService.GetDesiredIndentation(documentId, SourceText.From(template), filePath, lineNumber, tabSize, (Some options))
        match expectedIndentation with
        | None -> Assert.IsTrue(actualIndentation.IsNone, "No indentation was expected at line {0}", lineNumber)
        | Some(indentation) -> Assert.AreEqual(expectedIndentation.Value, actualIndentation.Value, "Indentation on line {0} doesn't match", lineNumber)
    
    [<Test>]
    member this.TestAutoIndentation() =
        let indentComment = System.Text.RegularExpressions.Regex(@"\/\/\s*\$\s*Indent:\s*(\d+)\s*\$")

        autoIndentTemplate.Split [|'\n'|]
        |> Seq.map (fun s -> s.Trim())
        |> Seq.indexed
        |> Seq.choose (fun (line, text) ->
            let m = indentComment.Match text
            if m.Success then Some (line, System.Convert.ToInt32 m.Groups.[1].Value)
            else None )
        |> Seq.iter (fun (lineNumber, expectedIndentation) ->
            let actualIndentation = FSharpIndentationService.GetDesiredIndentation(documentId, SourceText.From(autoIndentTemplate), filePath, lineNumber, tabSize, (Some options))
            Assert.AreEqual(expectedIndentation, actualIndentation.Value, "Indentation on line {0} doesn't match", lineNumber) )
