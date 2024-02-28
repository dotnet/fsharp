// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests

open System
open System.Collections.Generic
open System.Threading
open Xunit
open Microsoft.VisualStudio.FSharp.Editor
open FSharp.Compiler.CodeAnalysis
open FSharp.Editor.Tests.Helpers
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks

// AppDomain helper
type Worker() =

    let filePath = "C:\\test.fsx"

    member _.VerifyCompletionListExactly(fileContents: string, marker: string, expected: List<string>) =
        let caretPosition = fileContents.IndexOf(marker) + marker.Length

        let options =
            { RoslynTestHelpers.DefaultProjectOptions with
                SourceFiles = [| filePath |]
            }

        let document =
            RoslynTestHelpers.CreateSolution(fileContents, options = options)
            |> RoslynTestHelpers.GetSingleDocument

        let expected = expected |> Seq.toList

        let actual =
            let x =
                FSharpCompletionProvider.ProvideCompletionsAsyncAux(document, caretPosition, (fun _ -> [||]))
                |> CancellableTask.start CancellationToken.None

            x.Result
            |> Seq.toList
            // sort items as Roslyn do - by `SortText`
            |> List.sortBy (fun x -> x.SortText)

        let actualNames = actual |> List.map (fun x -> x.DisplayText)

        if actualNames <> expected then
            failwithf
                "Expected:\n%s,\nbut was:\n%s\nactual with sort text:\n%s"
                (String.Join("; ", expected |> List.map (sprintf "\"%s\"")))
                (String.Join("; ", actualNames |> List.map (sprintf "\"%s\"")))
                (String.Join("\n", actual |> List.map (fun x -> sprintf "%s => %s" x.DisplayText x.SortText)))

module FsxCompletionProviderTests =

    let getWorker () = Worker()

#if RELEASE
    [<Fact(Skip = "Fails in some CI, reproduces locally in Release mode, needs investigation")>]
#else
    [<Fact>]
#endif
    let fsiShouldTriggerCompletionInFsxFile () =
        let fileContents =
            """
    fsi.
    """

        let expected =
            List<string>(
                [
                    "CommandLineArgs"
                    "EventLoop"
                    "FloatingPointFormat"
                    "FormatProvider"
                    "PrintDepth"
                    "PrintLength"
                    "PrintSize"
                    "PrintWidth"
                    "ShowDeclarationValues"
                    "ShowIEnumerable"
                    "ShowProperties"
                    "AddPrinter"
                    "AddPrintTransformer"
                    "Equals"
                    "GetHashCode"
                    "GetType"
                    "ToString"
                ]
            )

        // We execute in a separate appdomain so that we can set BaseDirectory to a nonexistent location
        getWorker().VerifyCompletionListExactly(fileContents, "fsi.", expected)
