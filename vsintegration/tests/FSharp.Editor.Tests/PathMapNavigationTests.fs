// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// A library whose build maps its source paths, as DeterministicSourcePaths does: the symbols another
/// project imports from it must still name the files of the workspace.
module FSharp.Editor.Tests.PathMapNavigationTests

open System
open System.IO
open System.Threading
open Xunit
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks
open FSharp.Compiler.Text
open FSharp.Editor.Tests.Helpers
open FSharp.Test.ProjectGeneration

/// As Directory.Build.props would set it: the same map on every project of the solution.
let private pathMap (project: SyntheticProject) =
    [ $"--pathmap:{Path.GetDirectoryName project.ProjectDir}=.\\" ]

let private library =
    let library = SyntheticProject.Create("Library", sourceFile "Library" [])

    { library with
        OtherOptions = pathMap library
    }

let private app =
    let app = SyntheticProject.Create("App", sourceFile "App" [ "Library" ])

    { app with
        DependsOn = [ library ]
        OtherOptions = pathMap app
    }

let private solution, _ = RoslynTestHelpers.CreateMultiProjectSolution app

let private documentOf (project: SyntheticProject) fileId =
    solution.GetDocumentIdsWithFilePath(project.GetFilePath fileId)
    |> Seq.exactlyOne
    |> solution.GetDocument

[<Fact>]
let ``the path map of a project is not applied in the IDE`` () =
    let _, _, _, options =
        (documentOf library "Library").GetFSharpCompilationOptionsAsync "test"
        |> CancellableTask.runSynchronouslyWithoutCancellation

    Assert.DoesNotContain(options.OtherOptions, fun option -> option.StartsWith("--pathmap:", StringComparison.Ordinal))

[<Fact>]
let ``goto definition into a project built with a path map reaches its source`` () =
    let appDocument = documentOf app "App"
    let text = appDocument.GetTextAsync(CancellationToken.None).Result.ToString()

    let position =
        text.IndexOf("ModuleLibrary.f", StringComparison.Ordinal)
        + "ModuleLibrary.f".Length
        - 1

    let result =
        GoToDefinition(FSharpMetadataAsSourceService()).FindDefinitionAtPosition(appDocument, position)
        |> CancellableTask.runSynchronouslyWithoutCancellation

    match result with
    | ValueSome(FSharpGoToDefinitionResult.NavigableItem item, _) -> Assert.Equal(library.GetFilePath "Library", item.Document.FilePath)
    | result -> failwith $"expected a navigable item, got %A{result}"

/// A mapped name arrives with the separator its replacement doubled (`.\` + `\rest`), which is what the
/// compiler writes and what a build on a path map hands back.
[<Theory>]
[<InlineData(@"D:\repo\src\Lib\File.fs", @".\\src\Lib\File.fs", true)>]
[<InlineData(@"D:\repo\src\Lib\File.fs", @"./src/Lib/File.fs", true)>]
[<InlineData(@"D:\repo\src\Lib\File.fs", @"D:\repo\src\Lib\File.fs", true)>]
[<InlineData(@"D:\repo\src\Lib\File.fs", @"ib\File.fs", false)>]
[<InlineData(@"D:\repo\src\Lib\File.fs", @".\src\Other\File.fs", false)>]
let ``a relative name denotes the file whose path ends with it`` (path: string) (fileName: string) (expected: bool) =
    Assert.Equal(expected, fileName |> isTheFileAt path)

/// An assembly built with a path map records no root for the names it maps, so a name that arrives
/// relative cannot be resolved against the current directory: that belongs to the process, not to the
/// solution, and points wherever the last component to set it left it.
[<Fact>]
let ``a range a path map left relative still names its document`` () =
    let real = library.GetFilePath "Library"
    let root = Path.GetDirectoryName library.ProjectDir

    let relative =
        $".\\{real.Substring(root.Length).TrimStart(Path.DirectorySeparatorChar)}"

    let range = Range.mkRange relative (Position.mkPos 1 0) (Position.mkPos 1 0)

    match solution.TryGetDocumentIdFromFSharpRange range with
    | Some documentId -> Assert.Equal(real, solution.GetDocument(documentId).FilePath)
    | None -> failwith $"no document is named by {relative}"

[<Fact>]
let ``a relative name is matched by whole directories, not by the tail of one`` () =
    let real = library.GetFilePath "Library"
    let root = Path.GetDirectoryName library.ProjectDir
    let insideASegment = real.Substring(root.Length + 2)

    let range = Range.mkRange insideASegment (Position.mkPos 1 0) (Position.mkPos 1 0)

    match solution.TryGetDocumentIdFromFSharpRange range with
    | Some documentId -> failwith $"{insideASegment} must not name {solution.GetDocument(documentId).FilePath}"
    | None -> ()
