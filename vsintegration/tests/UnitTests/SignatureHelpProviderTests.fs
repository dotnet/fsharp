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
open FSharp.Compiler.SourceCodeServices
open UnitTests.TestLib.LanguageService

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

let private DefaultDocumentationProvider = 
    { new IDocumentationBuilder with
        override doc.AppendDocumentationFromProcessedXML(_, _, _, _, _, _) = ()
        override doc.AppendDocumentation(_, _, _, _, _, _, _) = ()
    }

let GetSignatureHelp (project:FSharpProject) (fileName:string) (caretPosition:int) =
    async {
        let triggerChar = None // TODO:
        let code = File.ReadAllText(fileName)
        let! triggered = FSharpSignatureHelpProvider.ProvideMethodsAsyncAux(checker, DefaultDocumentationProvider, SourceText.From(code), caretPosition, project.Options, triggerChar, fileName, 0)
        return triggered
    } |> Async.RunSynchronously

let GetCompletionTypeNames (project:FSharpProject) (fileName:string) (caretPosition:int) =
    let sigHelp = GetSignatureHelp project fileName caretPosition
    match sigHelp with
        | None -> [||]
        | Some (items, _applicableSpan, _argumentIndex, _argumentCount, _argumentName) ->
            let completionTypeNames =
                items
                |> Array.map (fun (_, _, _, _, _, x, _) -> x |> Array.map (fun (_, _, x, _, _) -> x))
            completionTypeNames

let GetCompletionTypeNamesFromCursorPosition (project:FSharpProject) =
    let fileName, caretPosition = project.GetCaretPosition()
    let completionNames = GetCompletionTypeNames project fileName caretPosition
    completionNames

let GetCompletionTypeNamesFromXmlString (xml:string) =
    use project = CreateProject xml
    GetCompletionTypeNamesFromCursorPosition project

let GetCompletionTypeNamesFromCode (code:string) =
    use project = SingleFileProject code
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
             (")", None)]);
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
          ]

    let sb = StringBuilder()
    for (fileContents, testCases) in manyTestCases do
      printfn "Test case: fileContents = %s..." fileContents.[2..4]
      
      let actual = 
        [ for (marker, expected) in testCases do
            printfn "Test case: marker = %s" marker 

            let caretPosition = fileContents.IndexOf(marker) + marker.Length

            let triggerChar = if marker = "," then Some ',' elif marker = "(" then Some '(' elif marker = "<" then Some '<' else None
            let triggered = FSharpSignatureHelpProvider.ProvideMethodsAsyncAux(checker, DefaultDocumentationProvider, SourceText.From(fileContents), caretPosition, projectOptions, triggerChar, filePath, 0) |> Async.RunSynchronously
            checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()
            let actual = 
                match triggered with 
                | None -> None
                | Some (_,applicableSpan,argumentIndex,argumentCount,argumentName) -> Some (applicableSpan.ToString(),argumentIndex,argumentCount,argumentName)

            if expected <> actual then 
                sb.AppendLine(sprintf "FSharpCompletionProvider.ProvideMethodsAsyncAux() gave unexpected results, expected %A, got %A" expected actual) |> ignore
            yield (marker, actual) ]

      printfn "(\"\"\"%s\n\"\"\",\n%s)" fileContents ((sprintf "%A" actual).Replace("null","None"))
        
            
    match sb.ToString() with
    | "" -> ()
    | errorText -> Assert.Fail errorText

// migrated from legacy test
[<Test>]
let ``Multi.ReferenceToProjectLibrary``() =
    let completionNames = GetCompletionTypeNamesFromXmlString @"
<Projects>

  <Project Name=""TestLibrary.fsproj"">
    <Reference>HelperLibrary.fsproj</Reference>
    <File Name=""Test.fs"">
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
