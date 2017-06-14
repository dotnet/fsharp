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
    x + y

let mutable x = 0
x <-
    10 * 2

match Some 10 with
| None -> 0
| Some x ->
    x + 1

try
    failwith \"fail\"
with
    | :? System.Exception -> \"error\"

if 10 > 0 then
    true
else
    false

(
    1,
    2,
)

[
    1
    2
]

[|
    1
    2
|]

[<
    Literal
>]
let constX = 10

let t = seq {
    yield 1
}

let g = function
    | None -> 1
    | Some _ -> 0

module MyModule = begin
end

type MyType() = class
end

type MyStruct = struct
end

while true do
    printfn \"never end\"
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

        [| None;     0; autoIndentTemplate |]
        [| Some(4);  2; autoIndentTemplate |]
        [| Some(4);  6; autoIndentTemplate |]
        [| Some(4);  11; autoIndentTemplate |]
        [| Some(4);  14; autoIndentTemplate |]
        [| Some(4);  19; autoIndentTemplate |]
        [| Some(4);  21; autoIndentTemplate |]
        [| Some(4);  24; autoIndentTemplate |]
        [| Some(4);  29; autoIndentTemplate |]
        [| Some(4);  34; autoIndentTemplate |]
        [| Some(4);  39; autoIndentTemplate |]
        [| Some(4);  44; autoIndentTemplate |]
        [| Some(4);  48; autoIndentTemplate |]
        [| Some(4);  52; autoIndentTemplate |]
        [| Some(4);  55; autoIndentTemplate |]
        [| Some(4);  58; autoIndentTemplate |]
        [| Some(4);  61; autoIndentTemplate |]
    |]

    [<TestCaseSource("testCases")>]
    member this.TestIndentation(expectedIndentation: Option<int>, lineNumber: int, template: string) = 
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
        let actualIndentation = FSharpIndentationService.GetDesiredIndentation(documentId, SourceText.From(template), filePath, lineNumber, tabSize, (Some options))
        match expectedIndentation with
        | None -> Assert.IsTrue(actualIndentation.IsNone, "No indentation was expected at line {0}", lineNumber)
        | Some(indentation) -> Assert.AreEqual(expectedIndentation.Value, actualIndentation.Value, "Indentation on line {0} doesn't match", lineNumber)
    