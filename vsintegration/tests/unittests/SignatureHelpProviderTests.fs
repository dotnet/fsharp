// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
//
// To run the tests in this file:
//
// Technique 1: Compile VisualFSharp.Unittests.dll and run it as a set of unit tests
//
// Technique 2:
//
//   Enable some tests in the #if EXE section at the end of the file, 
//   then compile this file as an EXE that has InternalsVisibleTo access into the
//   appropriate DLLs.  This can be the quickest way to get turnaround on updating the tests
//   and capturing large amounts of structured output.
(*
    cd Debug\net40\bin
    .\fsc.exe --define:EXE -r:.\Microsoft.Build.Utilities.Core.dll -o VisualFSharp.Unittests.exe -g --optimize- -r .\FSharp.LanguageService.Compiler.dll  -r .\FSharp.Editor.dll -r nunit.framework.dll ..\..\..\tests\service\FsUnit.fs ..\..\..\tests\service\Common.fs /delaysign /keyfile:..\..\..\src\fsharp\msft.pubkey ..\..\..\vsintegration\tests\unittests\SignatureHelpProviderTests.fs 
    .\VisualFSharp.Unittests.exe 
*)
// Technique 3: 
// 
//    Use F# Interactive.  This only works for FSharp.Compiler.Service.dll which has a public API
[<NUnit.Framework.Category "Roslyn Services">]
module Microsoft.VisualStudio.FSharp.Editor.Tests.Roslyn.SignatureHelpProvider

open System
open System.IO
open System.Text
open NUnit.Framework
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.FSharp.Compiler.SourceCodeServices

let filePath = "C:\\test.fs"

let PathRelativeToTestAssembly p = Path.Combine(Path.GetDirectoryName(Uri( System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath), p)

let internal options = { 
    ProjectFileName = "C:\\test.fsproj"
    ProjectFileNames =  [| filePath |]
    ReferencedProjects = [| |]
    OtherOptions = [| "-r:" + PathRelativeToTestAssembly(@"UnitTests\MockTypeProviders\DummyProviderForLanguageServiceTesting.dll") |]
    IsIncompleteTypeCheckEnvironment = true
    UseScriptResolutionRules = false
    LoadTime = DateTime.MaxValue
    OriginalLoadReferences = []
    UnresolvedReferences = None
    ExtraProjectInfo = None
}

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

            let documentationProvider = 
                { new IDocumentationBuilder with
                    override doc.AppendDocumentationFromProcessedXML(_, _, _, _, _, _) = ()
                    override doc.AppendDocumentation(_, _, _, _, _, _, _) = ()
                } 

            let triggerChar = if marker = "," then Some ',' elif marker = "(" then Some '(' elif marker = "<" then Some '<' else None
            let triggered = FSharpSignatureHelpProvider.ProvideMethodsAsyncAux(FSharpChecker.Instance, documentationProvider, SourceText.From(fileContents), caretPosition, options, triggerChar, filePath, 0) |> Async.RunSynchronously
            FSharpChecker.Instance.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()
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



#if EXE
ShouldGiveSignatureHelpAtCorrectMarkers()
#endif
