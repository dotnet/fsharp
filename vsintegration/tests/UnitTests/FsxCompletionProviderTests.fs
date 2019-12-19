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
    .\fsc.exe --define:EXE -r:.\Microsoft.Build.Utilities.Core.dll -o VisualFSharp.UnitTests.exe -g --optimize- -r .\FSharp.Compiler.Private.dll  -r .\FSharp.Editor.dll -r nunit.framework.dll ..\..\..\tests\service\FsUnit.fs ..\..\..\tests\service\Common.fs /delaysign /keyfile:..\..\..\src\fsharp\msft.pubkey ..\..\..\vsintegration\tests\UnitTests\FsxCompletionProviderTests.fs 
    .\VisualFSharp.UnitTests.exe 
*)
// Technique 3: 
// 
//    Use F# Interactive.  This only works for FSharp.Compiler.Service.dll which has a public API

// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace Microsoft.VisualStudio.FSharp.Editor.Tests.Roslyn

open System
open System.Collections.Generic
open System.IO
open System.Linq
open System.Reflection

open NUnit.Framework

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Completion
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor

open FSharp.Compiler.SourceCodeServices
open UnitTests.TestLib.LanguageService

// AppDomain helper
type Worker () =
    inherit MarshalByRefObject()
                            
    let filePath = "C:\\test.fsx"
    let projectOptions = { 
        ProjectFileName = "C:\\test.fsproj"
        ProjectId = None
        SourceFiles =  [| filePath |]
        ReferencedProjects = [| |]
        OtherOptions = [| |]
        IsIncompleteTypeCheckEnvironment = true
        UseScriptResolutionRules = true
        LoadTime = DateTime.MaxValue
        OriginalLoadReferences = []
        UnresolvedReferences = None
        ExtraProjectInfo = None
        Stamp = None
    }

    let formatCompletions(completions : string seq) =
        "\n\t" + String.Join("\n\t", completions)

    let VerifyCompletionList(fileContents: string, marker: string, expected: string list, unexpected: string list) =
        let caretPosition = fileContents.IndexOf(marker) + marker.Length
        let results = 
            FSharpCompletionProvider.ProvideCompletionsAsyncAux(checker, SourceText.From(fileContents), caretPosition, projectOptions, filePath, 0, (fun _ -> []), LanguageServicePerformanceOptions.Default, IntelliSenseOptions.Default) 
            |> Async.RunSynchronously 
            |> Option.defaultValue (ResizeArray())
            |> Seq.map(fun result -> result.DisplayText)

        let expectedFound =
            expected
            |> Seq.filter results.Contains

        let expectedNotFound =
            expected
            |> Seq.filter (expectedFound.Contains >> not)

        let unexpectedNotFound =
            unexpected
            |> Seq.filter (results.Contains >> not)

        let unexpectedFound =
            unexpected
            |> Seq.filter (unexpectedNotFound.Contains >> not)

        // If either of these are true, then the test fails.
        let hasExpectedNotFound = not (Seq.isEmpty expectedNotFound)
        let hasUnexpectedFound = not (Seq.isEmpty unexpectedFound)

        if hasExpectedNotFound || hasUnexpectedFound then
            let expectedNotFoundMsg = 
                if hasExpectedNotFound then
                    sprintf "\nExpected completions not found:%s\n" (formatCompletions expectedNotFound)
                else
                    String.Empty

            let unexpectedFoundMsg = 
                if hasUnexpectedFound then
                    sprintf "\nUnexpected completions found:%s\n" (formatCompletions unexpectedFound)
                else
                    String.Empty

            let completionsMsg = sprintf "\nin Completions:%s" (formatCompletions results)

            let msg = sprintf "%s%s%s" expectedNotFoundMsg unexpectedFoundMsg completionsMsg

            Assert.Fail(msg)

    member __.VerifyCompletionListExactly(fileContents: string, marker: string, expected: List<string>) =

        let caretPosition = fileContents.IndexOf(marker) + marker.Length
        let expected = expected |> Seq.toList
        let actual = 
            let x = FSharpCompletionProvider.ProvideCompletionsAsyncAux(checker, SourceText.From(fileContents), caretPosition, projectOptions, filePath, 0, (fun _ -> []), LanguageServicePerformanceOptions.Default, IntelliSenseOptions.Default) 
                    |> Async.RunSynchronously 
            x |> Option.defaultValue (ResizeArray())
            |> Seq.toList
            // sort items as Roslyn do - by `SortText`
            |> List.sortBy (fun x -> x.SortText)

        let actualNames = actual |> List.map (fun x -> x.DisplayText)

        if actualNames <> expected then
            Assert.Fail(sprintf "Expected:\n%s,\nbut was:\n%s\nactual with sort text:\n%s" 
                                (String.Join("; ", expected |> List.map (sprintf "\"%s\""))) 
                                (String.Join("; ", actualNames |> List.map (sprintf "\"%s\"")))
                                (String.Join("\n", actual |> List.map (fun x -> sprintf "%s => %s" x.DisplayText x.SortText))))

module FsxCompletionProviderTests =

    let pathToThisDll = Assembly.GetExecutingAssembly().CodeBase

    let getWorker () =

        let adSetup =
            let setup = new System.AppDomainSetup ()
            setup.PrivateBinPath <- pathToThisDll
            setup.ApplicationBase <- Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SomeNonExistentDirectory")
            setup

        let ad = AppDomain.CreateDomain((Guid()).ToString(), null, adSetup)
        (ad.CreateInstanceFromAndUnwrap(pathToThisDll, typeof<Worker>.FullName)) :?> Worker

    [<Test>]
    let fsiShouldTriggerCompletionInFsxFile() =
    
        let fileContents = """
    fsi.
    """
        let expected = List<string>([
            "CommandLineArgs"; "EventLoop"; "FloatingPointFormat"; "FormatProvider"; "PrintDepth"; 
            "PrintLength"; "PrintSize"; "PrintWidth"; "ShowDeclarationValues"; "ShowIEnumerable"; 
            "ShowProperties"; "AddPrinter"; "AddPrintTransformer"; "Equals"; "GetHashCode"; 
            "GetType"; "ToString"; ])

        // We execute in a seperate appdomain so that we can set BaseDirectory to a non-existent location
        getWorker().VerifyCompletionListExactly(fileContents, "fsi.", expected)

#if EXE
ShouldTriggerCompletionInFsxFile()
#endif
