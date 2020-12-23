// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
//
// To run the tests in this file:
//
// Technique 1: Compile VisualFSharp.UnitTests.dll and run it as a set of unit tests
//
// Technique 2:
//
//   Enable some tests in the #if EXE section at the end of the file, 
//   then compile this file as an EXE that has InternalsVisibleTo access into the
//   appropriate DLLs.  This can be the quickest way to get turnaround on updating the tests
//   and capturing large amounts of structured output.
(*
    cd Debug\net40\bin
    .\fsc.exe --define:EXE -r:.\Microsoft.Build.Utilities.Core.dll -o VisualFSharp.UnitTests.exe -g --optimize- -r .\FSharp.Compiler.Private.dll  -r .\FSharp.Editor.dll -r nunit.framework.dll ..\..\..\tests\service\FsUnit.fs ..\..\..\tests\service\Common.fs /delaysign /keyfile:..\..\..\src\fsharp\msft.pubkey ..\..\..\vsintegration\tests\UnitTests\SignatureHelpProviderTests.fs 
    .\VisualFSharp.UnitTests.exe 
*)
// Technique 3: 
// 
//    Use F# Interactive.  This only works for FSharp.Compiler.Private.dll which has a public API
[<NUnit.Framework.Category "Roslyn Services">]
module Microsoft.VisualStudio.FSharp.Editor.Tests.Roslyn.SignatureHelpProvider

open System
open System.IO
open System.Text
open NUnit.Framework
open Microsoft.CodeAnalysis.Text
open VisualFSharp.UnitTests.Roslyn
open Microsoft.VisualStudio.FSharp.Editor
open UnitTests.TestLib.LanguageService
open FSharp.Compiler
open FSharp.Compiler.Text
open FSharp.Compiler.SourceCodeServices
open Microsoft.CodeAnalysis

let filePath = "C:\\test.fs"

let PathRelativeToTestAssembly p = Path.Combine(Path.GetDirectoryName(Uri( System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath), p)

let internal projectOptions = { 
    ProjectFileName = "C:\\test.fsproj"
    ProjectId = None
    SourceFiles =  [| filePath |]
    ReferencedProjects = [| |]
    OtherOptions = [| "-r:" + PathRelativeToTestAssembly(@"DummyProviderForLanguageServiceTesting.dll") |]
    IsIncompleteTypeCheckEnvironment = true
    UseScriptResolutionRules = false
    LoadTime = DateTime.MaxValue
    OriginalLoadReferences = []
    UnresolvedReferences = None
    ExtraProjectInfo = None
    Stamp = None
}

let internal parsingOptions =
    { FSharpParsingOptions.Default with SourceFiles = [| filePath |] }

let private DefaultDocumentationProvider = 
    { new IDocumentationBuilder with
        override doc.AppendDocumentationFromProcessedXML(_, _, _, _, _, _) = ()
        override doc.AppendDocumentation(_, _, _, _, _, _, _) = ()
    }

let GetSignatureHelp (project:FSharpProject) (fileName:string) (caretPosition:int) =
    async {
        let triggerChar = None
        let fileContents = File.ReadAllText(fileName)
        let sourceText = SourceText.From(fileContents)
        let textLines = sourceText.Lines
        let caretLinePos = textLines.GetLinePosition(caretPosition)
        let caretLineColumn = caretLinePos.Character
        let perfOptions = LanguageServicePerformanceOptions.Default
        let textVersionHash = 1
        
        let parseResults, _, checkFileResults =
            let x =
                checker.ParseAndCheckDocument(fileName, textVersionHash, sourceText, project.Options, perfOptions, "TestSignatureHelpProvider")
                |> Async.RunSynchronously
            x.Value

        let paramInfoLocations = parseResults.FindNoteworthyParamInfoLocations(Pos.fromZ caretLinePos.Line caretLineColumn).Value
        let triggered =
            FSharpSignatureHelpProvider.ProvideMethodsAsyncAux(
                caretLinePos,
                caretLineColumn,
                paramInfoLocations,
                checkFileResults,
                DefaultDocumentationProvider,
                sourceText,
                caretPosition,
                triggerChar)
            |> Async.RunSynchronously
        return triggered
    } |> Async.RunSynchronously

let GetCompletionTypeNames (project:FSharpProject) (fileName:string) (caretPosition:int) =
    let sigHelp = GetSignatureHelp project fileName caretPosition
    match sigHelp with
        | None -> [||]
        | Some data ->
            let completionTypeNames =
                data.SignatureHelpItems
                |> Array.map (fun r -> r.Parameters |> Array.map (fun p -> p.CanonicalTypeTextForSorting))
            completionTypeNames

let GetCompletionTypeNamesFromCursorPosition (project:FSharpProject) =
    let fileName, caretPosition = project.GetCaretPosition()
    let completionNames = GetCompletionTypeNames project fileName caretPosition
    completionNames

let GetCompletionTypeNamesFromXmlString (xml:string) =
    use project = CreateProject xml
    GetCompletionTypeNamesFromCursorPosition project

[<Test>]
let ShouldGiveSignatureHelpAtCorrectMarkers() =
    let manyTestCases = 
        [ ("""
//1
System.Console.WriteLine(format="Hello, {0}",arg0="World")
""",
            [(".", None); 
             ("System", None); 
             ("WriteLine", None);
             ("(", Some ("[7..64)", 0, 2, Some "format")); 
             ("format", Some ("[7..64)", 0, 2, Some "format"));
             (",", None);
             ("""",""", Some ("[7..64)", 1, 2, Some "arg0"));
             ("arg0", Some ("[7..64)", 1, 2, Some "arg0"));
             ("arg0=", Some ("[7..64)", 1, 2, Some "arg0")); 
             ("World", Some ("[7..64)", 1, 2, Some "arg0"));
             (")", Some("[7..64)", 0, 2, Some "format"))]);
          ( """
//2
open System
Console.WriteLine([(1,2)])
""",
            [
             ("WriteLine(", Some ("[20..45)", 0, 0, None));
             (",", None); 
             ("[(", Some ("[20..45)", 0, 1, None))
            ]);
          ( """
//3
type foo = N1.T< 
type foo2 = N1.T<Param1=
type foo3 = N1.T<ParamIgnored=
type foo4 = N1.T<Param1=1,
type foo5 = N1.T<Param1=1,ParamIgnored=
""",
            [("type foo = N1.T<", Some ("[18..26)", 0, 0, None));
             ("type foo2 = N1.T<", Some ("[38..52)", 0, 0, Some "Param1"));
             ("type foo2 = N1.T<Param1", Some ("[38..52)", 0, 1, Some "Param1"));
             ("type foo2 = N1.T<Param1=", Some ("[38..52)", 0, 1, Some "Param1"));
             ("type foo3 = N1.T<", Some ("[64..84)", 0, 0, Some "ParamIgnored"));
             ("type foo3 = N1.T<ParamIgnored=", Some ("[64..84)", 0, 1, Some "ParamIgnored"));
             ("type foo4 = N1.T<Param1", Some ("[96..112)", 0, 2, Some "Param1"));
             ("type foo4 = N1.T<Param1=", Some ("[96..112)", 0, 2, Some "Param1"));
             ("type foo4 = N1.T<Param1=1", Some ("[96..112)", 0, 2, Some "Param1"));
             ("type foo5 = N1.T<Param1", Some ("[124..153)", 0, 2, Some "Param1"));
             ("type foo5 = N1.T<Param1=", Some ("[124..153)", 0, 2, Some "Param1"));
             ("type foo5 = N1.T<Param1=1", Some ("[124..153)", 0, 2, Some "Param1"));
             ("type foo5 = N1.T<Param1=1,", Some ("[124..153)", 1, 2, Some "ParamIgnored"));
             ("type foo5 = N1.T<Param1=1,ParamIgnored",Some ("[124..153)", 1, 2, Some "ParamIgnored"));
             ("type foo5 = N1.T<Param1=1,ParamIgnored=",Some ("[124..153)", 1, 2, Some "ParamIgnored"))
            ]);
          ( """
//4
type foo = N1.T< > 
type foo2 = N1.T<Param1= >
type foo3 = N1.T<ParamIgnored= >
type foo4 = N1.T<Param1=1, >
type foo5 = N1.T<Param1=1,ParamIgnored= >
""",
            [("type foo = N1.T<", Some ("[18..24)", 0, 0, None));
             ("type foo2 = N1.T<", Some ("[40..53)", 0, 0, Some "Param1"));
             ("type foo2 = N1.T<Param1", Some ("[40..53)", 0, 1, Some "Param1"));
             ("type foo2 = N1.T<Param1=", Some ("[40..53)", 0, 1, Some "Param1"));
             ("type foo3 = N1.T<", Some ("[68..87)", 0, 0, Some "ParamIgnored"));
             ("type foo3 = N1.T<ParamIgnored=", Some ("[68..87)", 0, 1, Some "ParamIgnored"));
             ("type foo4 = N1.T<Param1", Some ("[102..117)", 0, 2, Some "Param1"));
             ("type foo4 = N1.T<Param1=", Some ("[102..117)", 0, 2, Some "Param1"));
             ("type foo4 = N1.T<Param1=1", Some ("[102..117)", 0, 2, Some "Param1"));
             ("type foo5 = N1.T<Param1", Some ("[132..160)", 0, 2, Some "Param1"));
             ("type foo5 = N1.T<Param1=", Some ("[132..160)", 0, 2, Some "Param1"));
             ("type foo5 = N1.T<Param1=1", Some ("[132..160)", 0, 2, Some "Param1"));
             ("type foo5 = N1.T<Param1=1,", Some ("[132..160)", 1, 2, Some "ParamIgnored"));
             ("type foo5 = N1.T<Param1=1,ParamIgnored",Some ("[132..160)", 1, 2, Some "ParamIgnored"));
             ("type foo5 = N1.T<Param1=1,ParamIgnored=",Some ("[132..160)", 1, 2, Some "ParamIgnored"))])
//Test case 5
          ( """let _ = System.DateTime(""",
            [("let _ = System.DateTime(",  Some ("[8..24)", 0, 0, None)) ])
          ( """let _ = System.DateTime(1L,""",
            [("let _ = System.DateTime(1L,", Some ("[8..27)", 1, 2, None )) ])
          ]

    let sb = StringBuilder()
    for (fileContents, testCases) in manyTestCases do
      printfn "Test case: fileContents = %s..." fileContents.[2..4]
      
      let actual = 
        [ for (marker, expected) in testCases do
            printfn "Test case: marker = %s" marker 
            let caretPosition = fileContents.IndexOf(marker) + marker.Length
            let triggerChar = if marker ="," then Some ',' elif marker = "(" then Some '(' elif marker = "<" then Some '<' else None
            let sourceText = SourceText.From(fileContents)
            let textLines = sourceText.Lines
            let caretLinePos = textLines.GetLinePosition(caretPosition)
            let caretLineColumn = caretLinePos.Character
            let perfOptions = LanguageServicePerformanceOptions.Default
            let textVersionHash = 0
            
            let parseResults, _, checkFileResults =
                let x =
                    checker.ParseAndCheckDocument(filePath, textVersionHash, sourceText, projectOptions, perfOptions, "TestSignatureHelpProvider")
                    |> Async.RunSynchronously

                if x.IsNone then
                    Assert.Fail("Could not parse and check document.")
                x.Value

            let actual = 
                let paramInfoLocations = parseResults.FindNoteworthyParamInfoLocations(Pos.fromZ caretLinePos.Line caretLineColumn)
                match paramInfoLocations with
                | None -> None
                | Some paramInfoLocations ->
                    let triggered =
                        FSharpSignatureHelpProvider.ProvideMethodsAsyncAux(
                            caretLinePos,
                            caretLineColumn,
                            paramInfoLocations,
                            checkFileResults,
                            DefaultDocumentationProvider,
                            sourceText,
                            caretPosition,
                            triggerChar)
                        |> Async.RunSynchronously
                    
                    checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()
                    match triggered with 
                    | None -> None
                    | Some data -> Some (data.ApplicableSpan.ToString(),data.ArgumentIndex,data.ArgumentCount,data.ArgumentName)

            if expected <> actual then 
                sb.AppendLine(sprintf "FSharpCompletionProvider.ProvideMethodsAsyncAux() gave unexpected results, expected %A, got %A" expected actual) |> ignore
            yield (marker, actual) ]

      printfn "(\"\"\"%s\n\"\"\",\n%s)" fileContents ((sprintf "%A" actual).Replace("null","None"))
        
            
    match sb.ToString() with
    | "" -> ()
    | errorText -> Assert.Fail errorText

[<Test>]
let ``single argument function application``() =
    let fileContents = """
sqrt 
"""
    let marker = "sqrt "
    let caretPosition = fileContents.IndexOf(marker) + marker.Length
    let sourceText = SourceText.From(fileContents)
    let perfOptions = LanguageServicePerformanceOptions.Default
    let textVersionHash = 0
    let documentId = DocumentId.CreateNewId(ProjectId.CreateNewId())
    
    let parseResults, _, checkFileResults =
        let x =
            checker.ParseAndCheckDocument(filePath, textVersionHash, sourceText, projectOptions, perfOptions, "TestSignatureHelpProvider")
            |> Async.RunSynchronously

        if x.IsNone then
            Assert.Fail("Could not parse and check document.")
        x.Value
    
    let sigHelp =
        FSharpSignatureHelpProvider.ProvideParametersAsyncAux(
            parseResults,
            checkFileResults,
            documentId,
            [],
            DefaultDocumentationProvider,
            sourceText,
            caretPosition,
            filePath)
        |> Async.RunSynchronously

    checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()

    match sigHelp with
    | None -> Assert.Fail("Expected signature help")
    | Some sigHelp ->
        Assert.AreEqual(1, sigHelp.ArgumentCount)
        Assert.AreEqual(0, sigHelp.ArgumentIndex)

[<Test>]
let ``multi-argument function application``() =
    let fileContents = """
let add2 x y = x + y
add2 1 
"""
    let marker = "add2 1 "
    let caretPosition = fileContents.IndexOf(marker) + marker.Length
    let sourceText = SourceText.From(fileContents)
    let perfOptions = LanguageServicePerformanceOptions.Default
    let textVersionHash = 0
    let documentId = DocumentId.CreateNewId(ProjectId.CreateNewId())
    
    let parseResults, _, checkFileResults =
        let x =
            checker.ParseAndCheckDocument(filePath, textVersionHash, sourceText, projectOptions, perfOptions, "TestSignatureHelpProvider")
            |> Async.RunSynchronously

        if x.IsNone then
            Assert.Fail("Could not parse and check document.")
        x.Value
    
    let sigHelp =
        FSharpSignatureHelpProvider.ProvideParametersAsyncAux(
            parseResults,
            checkFileResults,
            documentId,
            [],
            DefaultDocumentationProvider,
            sourceText,
            caretPosition,
            filePath)
        |> Async.RunSynchronously

    checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()

    match sigHelp with
    | None -> Assert.Fail("Expected signature help")
    | Some sigHelp ->
        Assert.AreEqual(2, sigHelp.ArgumentCount)
        Assert.AreEqual(1, sigHelp.ArgumentIndex)

[<Test>]
let ``qualified function application``() =
    let fileContents = """
module M =
    let f x = x
M.f 
"""
    let marker = "M.f "
    let caretPosition = fileContents.IndexOf(marker) + marker.Length
    let sourceText = SourceText.From(fileContents)
    let perfOptions = LanguageServicePerformanceOptions.Default
    let textVersionHash = 0
    let documentId = DocumentId.CreateNewId(ProjectId.CreateNewId())
    
    let parseResults, _, checkFileResults =
        let x =
            checker.ParseAndCheckDocument(filePath, textVersionHash, sourceText, projectOptions, perfOptions, "TestSignatureHelpProvider")
            |> Async.RunSynchronously

        if x.IsNone then
            Assert.Fail("Could not parse and check document.")
        x.Value
    
    let sigHelp =
        FSharpSignatureHelpProvider.ProvideParametersAsyncAux(
            parseResults,
            checkFileResults,
            documentId,
            [],
            DefaultDocumentationProvider,
            sourceText,
            caretPosition,
            filePath)
        |> Async.RunSynchronously

    checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()

    match sigHelp with
    | None -> Assert.Fail("Expected signature help")
    | Some sigHelp ->
        Assert.AreEqual(1, sigHelp.ArgumentCount)
        Assert.AreEqual(0, sigHelp.ArgumentIndex)

[<Test>]
let ``function application in single pipeline with no additional args``() =
    let fileContents = """
[1..10] |> id 
"""
    let marker = "id "
    let caretPosition = fileContents.IndexOf(marker) + marker.Length
    let sourceText = SourceText.From(fileContents)
    let perfOptions = LanguageServicePerformanceOptions.Default
    let textVersionHash = 0
    let documentId = DocumentId.CreateNewId(ProjectId.CreateNewId())
    
    let parseResults, _, checkFileResults =
        let x =
            checker.ParseAndCheckDocument(filePath, textVersionHash, sourceText, projectOptions, perfOptions, "TestSignatureHelpProvider")
            |> Async.RunSynchronously

        if x.IsNone then
            Assert.Fail("Could not parse and check document.")
        x.Value
    
    let sigHelp =
        FSharpSignatureHelpProvider.ProvideParametersAsyncAux(
            parseResults,
            checkFileResults,
            documentId,
            [],
            DefaultDocumentationProvider,
            sourceText,
            caretPosition,
            filePath)
        |> Async.RunSynchronously

    checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()

    Assert.True(sigHelp.IsNone, "No signature help is expected because there are no additional args to apply.")

[<Test>]
let ``function application in single pipeline with an additional argument``() =
    let fileContents = """
[1..10] |> List.map  
"""
    let marker = "List.map "
    let caretPosition = fileContents.IndexOf(marker) + marker.Length
    let sourceText = SourceText.From(fileContents)
    let perfOptions = LanguageServicePerformanceOptions.Default
    let textVersionHash = 0
    let documentId = DocumentId.CreateNewId(ProjectId.CreateNewId())
    
    let parseResults, _, checkFileResults =
        let x =
            checker.ParseAndCheckDocument(filePath, textVersionHash, sourceText, projectOptions, perfOptions, "TestSignatureHelpProvider")
            |> Async.RunSynchronously

        if x.IsNone then
            Assert.Fail("Could not parse and check document.")
        x.Value
    
    let sigHelp =
        FSharpSignatureHelpProvider.ProvideParametersAsyncAux(
            parseResults,
            checkFileResults,
            documentId,
            [],
            DefaultDocumentationProvider,
            sourceText,
            caretPosition,
            filePath)
        |> Async.RunSynchronously

    checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()

    match sigHelp with
    | None -> Assert.Fail("Expected signature help")
    | Some sigHelp ->
        Assert.AreEqual(1, sigHelp.ArgumentCount)
        Assert.AreEqual(0, sigHelp.ArgumentIndex)

[<Test>]
let ``function application in middle of pipeline with an additional argument``() =
    let fileContents = """
[1..10]
|> List.map 
|> List.filer (fun x -> x > 3)
"""
    let marker = "List.map "
    let caretPosition = fileContents.IndexOf(marker) + marker.Length
    let sourceText = SourceText.From(fileContents)
    let perfOptions = LanguageServicePerformanceOptions.Default
    let textVersionHash = 0
    let documentId = DocumentId.CreateNewId(ProjectId.CreateNewId())
    
    let parseResults, _, checkFileResults =
        let x =
            checker.ParseAndCheckDocument(filePath, textVersionHash, sourceText, projectOptions, perfOptions, "TestSignatureHelpProvider")
            |> Async.RunSynchronously

        if x.IsNone then
            Assert.Fail("Could not parse and check document.")
        x.Value
    
    let sigHelp =
        FSharpSignatureHelpProvider.ProvideParametersAsyncAux(
            parseResults,
            checkFileResults,
            documentId,
            [],
            DefaultDocumentationProvider,
            sourceText,
            caretPosition,
            filePath)
        |> Async.RunSynchronously

    checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()

    match sigHelp with
    | None -> Assert.Fail("Expected signature help")
    | Some sigHelp ->
        Assert.AreEqual(1, sigHelp.ArgumentCount)
        Assert.AreEqual(0, sigHelp.ArgumentIndex)

[<Test>]
let ``function application with function as parameter``() =
    let fileContents = """
let derp (f: int -> int -> int) x = f x 1
derp 
"""
    let marker = "derp "
    let caretPosition = fileContents.LastIndexOf(marker) + marker.Length
    let sourceText = SourceText.From(fileContents)
    let perfOptions = LanguageServicePerformanceOptions.Default
    let textVersionHash = 0
    let documentId = DocumentId.CreateNewId(ProjectId.CreateNewId())
    
    let parseResults, _, checkFileResults =
        let x =
            checker.ParseAndCheckDocument(filePath, textVersionHash, sourceText, projectOptions, perfOptions, "TestSignatureHelpProvider")
            |> Async.RunSynchronously

        if x.IsNone then
            Assert.Fail("Could not parse and check document.")
        x.Value
    
    let sigHelp =
        FSharpSignatureHelpProvider.ProvideParametersAsyncAux(
            parseResults,
            checkFileResults,
            documentId,
            [],
            DefaultDocumentationProvider,
            sourceText,
            caretPosition,
            filePath)
        |> Async.RunSynchronously

    checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()

    match sigHelp with
    | None -> Assert.Fail("Expected signature help")
    | Some sigHelp ->
        Assert.AreEqual(2, sigHelp.ArgumentCount)
        Assert.AreEqual(0, sigHelp.ArgumentIndex)

// migrated from legacy test
[<Test>]
let ``Multi.ReferenceToProjectLibrary``() =
    let completionNames = GetCompletionTypeNamesFromXmlString @"
<Projects>

  <Project Name=""TestLibrary.fsproj"">
    <Reference>HelperLibrary.fsproj</Reference>
    <File Name=""test.fs"">
      <![CDATA[
open Test
Foo.Sum(12, $$
      ]]>
    </File>
  </Project>

  <Project Name=""HelperLibrary.fsproj"">
    <File Name=""Helper.fs"">
      <![CDATA[
namespace Test

type public Foo() =
    static member Sum(x:int, y:int) = x + y
      ]]>
    </File>
  </Project>

</Projects>
"
    let expected = [|
        [|"System.Int32"; "System.Int32"|]
    |]
    Assert.AreEqual(expected, completionNames)
