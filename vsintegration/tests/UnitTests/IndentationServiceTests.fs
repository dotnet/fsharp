// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace Microsoft.VisualStudio.FSharp.Editor.Tests.Roslyn

open System
open System.Threading

open NUnit.Framework

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor
open FSharp.Compiler.CodeAnalysis
open Microsoft.CodeAnalysis.Formatting

open UnitTests.TestLib.LanguageService

[<TestFixture>][<Category "Roslyn Services">]
type IndentationServiceTests()  =
    let filePath = "C:\\test.fs"
    let projectOptions: FSharpProjectOptions = {
        ProjectFileName = "C:\\test.fsproj"
        ProjectId = None
        SourceFiles =  [| filePath |]
        ReferencedProjects = [| |]
        OtherOptions = [| |]
        IsIncompleteTypeCheckEnvironment = true
        UseScriptResolutionRules = false
        LoadTime = DateTime.MaxValue
        OriginalLoadReferences = []
        UnresolvedReferences = None
        Stamp = None
    }

    let documentId = DocumentId.CreateNewId(ProjectId.CreateNewId())
    let tabSize = 4
    let indentStyle = FormattingOptions.IndentStyle.Smart

    let indentComment = System.Text.RegularExpressions.Regex(@"\$\s*Indent:\s*(\d+)\s*\$")

    let consoleProjectTemplate = "
// Learn more about F# at https://fsharp.org
// See the 'F# Tutorial' project for more help.

[<EntryPoint>]
let main argv =
    printfn \"%A\" argv
    0 // return an integer exit code"

    let libraryProjectTemplate = "
namespace ProjectNamespace

type Class1() =
    member this.X = \"F#\""

    let nestedTypesTemplate = "
namespace testspace
    type testtype
        static member testmember = 1

"

    let autoIndentTemplate = "
let plus x y =
    x + y // $Indent: 4$

let mutable x = 0
x <-
    10 * 2 // $Indent: 4$

match some 10 with
| None -> 0
| Some x ->
    x + 1 // $Indent: 4$

try
    failwith \"fail\" // $Indent: 4$
with
    | :? System.Exception -> \"error\"

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
    Literal // $Indent: 4$
>]
let constx = 10

let t = seq { // $Indent: 0$
    yield 1 // $Indent: 4$
}

let g =
    function
    | None -> 1 // $Indent: 4$
    | Some _ -> 0

module MyModule = begin
end // $Indent: 4$

type MyType() = class
end // $Indent: 4$

type MyStruct = struct
end // $Indent: 4$

while true do
    printfn \"never end\" // $Indent: 4$

// After line has keyword in comment such as function
// should not be indented $Indent: 0$

    // Even if the line before only had comment like this
// The follwing line should inherit that indentation too $Indent: 4$
"

    let testCases = [|
        ( None,     0, consoleProjectTemplate )
        ( None,     1, consoleProjectTemplate )
        ( Some(0),  2, consoleProjectTemplate )
        ( Some(0),  3, consoleProjectTemplate )
        ( Some(0),  4, consoleProjectTemplate )
        ( Some(0),  5, consoleProjectTemplate )
        ( Some(4),  6, consoleProjectTemplate )
        ( Some(4),  7, consoleProjectTemplate )
        ( Some(4),  8, consoleProjectTemplate )

        ( None,     0, libraryProjectTemplate )
        ( None,     1, libraryProjectTemplate )
        ( Some(0),  2, libraryProjectTemplate )
        ( Some(0),  3, libraryProjectTemplate )
        ( Some(4),  4, libraryProjectTemplate )
        ( Some(4),  5, libraryProjectTemplate )

        ( None,     0, nestedTypesTemplate )
        ( None,     1, nestedTypesTemplate )
        ( Some(0),  2, nestedTypesTemplate )
        ( Some(4),  3, nestedTypesTemplate )
        ( Some(8),  4, nestedTypesTemplate )
        ( Some(8),  5, nestedTypesTemplate )
    |]

    let autoIndentTestCases =
        autoIndentTemplate.Split [|'\n'|]
        |> Array.map (fun s -> s.Trim())
        |> Array.indexed
        |> Array.choose (fun (line, text) ->
            let m = indentComment.Match text
            if m.Success then Some (line, System.Convert.ToInt32 m.Groups.[1].Value)
            else None )
        |> Array.map (fun (lineNumber, expectedIndentation) ->
            ( Some(expectedIndentation), lineNumber, autoIndentTemplate ))

    [<Test>]
    member this.TestIndentation() =
        for (expectedIndentation, lineNumber, template) in testCases do
            let sourceText = SourceText.From(template)

            let parsingOptions, _ = checker.GetParsingOptionsFromProjectOptions projectOptions
            let actualIndentation = FSharpIndentationService.GetDesiredIndentation(documentId, sourceText, filePath, lineNumber, tabSize, indentStyle, parsingOptions)
            match expectedIndentation with
            | None -> Assert.IsTrue(actualIndentation.IsNone, "No indentation was expected at line {0}", lineNumber)
            | Some indentation -> Assert.AreEqual(expectedIndentation.Value, actualIndentation.Value, "Indentation on line {0} doesn't match", lineNumber)

    [<Test>]
    member this.TestAutoIndentation() =
        for (expectedIndentation, lineNumber, template) in autoIndentTestCases do


            let sourceText = SourceText.From(template)

            let parsingOptions, _ = checker.GetParsingOptionsFromProjectOptions projectOptions
            let actualIndentation = FSharpIndentationService.GetDesiredIndentation(documentId, sourceText, filePath, lineNumber, tabSize, indentStyle, parsingOptions)
            match expectedIndentation with
            | None -> Assert.IsTrue(actualIndentation.IsNone, "No indentation was expected at line {0}", lineNumber)
            | Some indentation -> Assert.AreEqual(expectedIndentation.Value, actualIndentation.Value, "Indentation on line {0} doesn't match", lineNumber)
