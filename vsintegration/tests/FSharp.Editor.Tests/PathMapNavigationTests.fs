// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// A library whose build maps its source paths, as DeterministicSourcePaths does: the symbols another
/// project imports from it must still name the files of the workspace.
module FSharp.Editor.Tests.PathMapNavigationTests

open System
open System.IO
open System.Threading
open Xunit
open Microsoft.CodeAnalysis
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks
open FSharp.Compiler.EditorServices
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

[<AutoOpen>]
module private Fixture =
    let struct (solution, _) = RoslynTestHelpers.CreateMultiProjectSolution app

let private documentAt (solution: Solution) (path: string) =
    solution.GetDocumentIdsWithFilePath path
    |> Seq.exactlyOne
    |> solution.GetDocument

let private documentOf (project: SyntheticProject) fileId =
    documentAt solution (project.GetFilePath fileId)

let private lastCharacterOf (document: Document) (name: string) =
    let text = document.GetTextAsync(CancellationToken.None).Result.ToString()
    text.IndexOf(name, StringComparison.Ordinal) + name.Length - 1

let private definitionAt (document: Document) (name: string) =
    GoToDefinition(FSharpMetadataAsSourceService()).FindDefinitionAtPosition(document, lastCharacterOf document name)
    |> CancellableTask.runSynchronouslyWithoutCancellation

[<Fact>]
let ``the path map of a project is not applied in the IDE`` () =
    let _, _, _, options =
        (documentOf library "Library").GetFSharpCompilationOptionsAsync "test"
        |> CancellableTask.runSynchronouslyWithoutCancellation

    Assert.DoesNotContain(options.OtherOptions, fun option -> option.StartsWith("--pathmap:", StringComparison.Ordinal))

[<Fact>]
let ``goto definition into a project built with a path map reaches its source`` () =
    match definitionAt (documentOf app "App") "ModuleLibrary.f" with
    | ValueSome(FSharpGoToDefinitionResult.NavigableItem item, _) -> Assert.Equal(library.GetFilePath "Library", item.Document.FilePath)
    | result -> failwith $"expected a navigable item, got %A{result}"

[<Literal>]
let private LibraryFile = @"D:\repo\src\Lib\File.fs"

[<Theory>]
[<InlineData(@".\\src\Lib\File.fs", true, Label = "the separator doubled after the map's replacement, as the compiler writes it")>]
[<InlineData(@"./src/Lib/File.fs", true)>]
[<InlineData(LibraryFile, true)>]
[<InlineData(@"ib\File.fs", false, Label = "the tail of a directory name")>]
[<InlineData(@".\src\Other\File.fs", false)>]
[<InlineData(@".\src\Scratch\..\Lib\File.fs", true)>]
[<InlineData(@".\src\.\Lib\Deep\Deeper\..\..\File.fs", true)>]
[<InlineData(@".\src\Lib\..\File.fs", false)>]
[<InlineData(@"..\src\Lib\File.fs", false, Label = "climbs above its start: a file only from the root, which is not recorded")>]
[<InlineData(@".\Scratch\..\..\src\Lib\File.fs", false, Label = "climbs above its start")>]
let ``a relative name denotes the file whose path ends with the directories it spells`` (fileName: string) (expected: bool) =
    Assert.Equal(expected, fileName |> isTheFileAt LibraryFile)

[<Fact>]
let ``a range a path map left relative names its document, given the assembly declaring it`` () =
    let real = library.GetFilePath "Library"

    let relative =
        $@".\{real.Substring(Path.GetDirectoryName(library.ProjectDir).Length + 1)}"

    let range = Range.mkRange relative Position.pos0 Position.pos0

    let reached name =
        solution.GetDocumentIdsWithFSharpFileName(name, library.Name)

    let documentId =
        solution.TryGetDocumentIdFromFSharpRange(range, declaringAssembly = library.Name)
        |> Option.defaultWith (fun () -> failwith $"no document is named by {relative}")

    Assert.Equal(real, solution.GetDocument(documentId).FilePath)
    Assert.Equal(None, solution.TryGetDocumentIdFromFSharpRange range)

    for _ in 1..2 do // the second answers come from the solution's cache
        Assert.Equal<DocumentId list>([ documentId ], reached relative)
        Assert.Empty(reached @".\Nowhere\Nothing.fs")

/// A solution of bare projects, written `Assembly=path;path|Assembly=path`.
let private bareSolutionOf (projects: string) =
    RoslynTestHelpers.CreateSolution
        [
            for project in projects.Split '|' do
                match project.Split '=' with
                | [| assembly; paths |] ->
                    let id = ProjectId.CreateNewId()

                    ProjectInfo.Create(
                        id,
                        VersionStamp.Default,
                        assembly,
                        assembly,
                        LanguageNames.FSharp,
                        documents =
                            [
                                for path in paths.Split ';' -> RoslynTestHelpers.CreateDocumentInfo id path ""
                            ]
                    )
                | _ -> failwith $"not a project: {project}"
        ]

[<Theory>]
[<InlineData(@"Lib=D:\repo\src\Lib\File.fs", @".\src\Lib\File.fs", @"D:\repo\src\Lib\File.fs")>]
[<InlineData(@"Other=D:\other\src\Lib\File.fs", @".\src\Lib\File.fs", "", Label = "an unrelated file where the source is absent")>]
[<InlineData(@"Other=D:\other\src\Lib\File.fs|Lib=D:\repo\src\Lib\File.fs",
             @".\src\Lib\File.fs",
             @"D:\repo\src\Lib\File.fs",
             Label = "an unrelated file beside the source")>]
[<InlineData(@"Lib=D:\repo\src\Lib\File.fs;D:\repo\copy\src\Lib\File.fs", @".\src\Lib\File.fs", "", Label = "two files of the assembly")>]
[<InlineData(@"Lib=D:\repo\src\Lib\File.fs|Lib=D:\repo\src\Lib\File.fs",
             @".\src\Lib\File.fs",
             @"D:\repo\src\Lib\File.fs;D:\repo\src\Lib\File.fs",
             Label = "the one file in two projects of the assembly, as multi-targeting has it")>]
[<InlineData(@"Lib=D:\repo\src\Lib\File.fs", @".\src\Scratch\..\Lib\.\File.fs", @"D:\repo\src\Lib\File.fs", Label = "an equivalent name")>]
[<InlineData(@"Lib=D:\repo\src\Lib\File.fs", @"..\src\Lib\File.fs", "", Label = "a name that needs the root")>]
[<InlineData(@"Lib=D:\repo\src\Lib\File.fs", @"ib\File.fs", "", Label = "the tail of a directory name")>]
[<InlineData(@"Lib=D:\repo\src\Lib\File.fs", "/home/build/a|b/File.fs", "", Label = "a name this platform cannot spell")>]
let ``a mapped name reaches the one file of the declaring assembly that ends with it``
    (projects: string)
    (name: string)
    (expected: string)
    =
    let solution = bareSolutionOf projects

    let reached =
        solution.GetDocumentIdsWithFSharpFileName(name, "Lib")
        |> List.map (fun documentId -> solution.GetDocument(documentId).FilePath)

    Assert.Equal<string list>(expected.Split([| ';' |], StringSplitOptions.RemoveEmptyEntries) |> List.ofArray, reached)

let private declarations = "type Kind =\n    | A\n    | B\n"

/// Compiled from its own directory with the path map DeterministicSourcePaths gives: its files are named
/// `.\FileLibrary.fsi` and `.\FileLibrary.fs`.
let private mappedLibrary =
    SyntheticProject.Create(
        "External",
        { sourceFile "Library" [] with
            SignatureFile = Custom $"{declarations}\nval value: int"
            Source = $"{declarations}\nlet value = 42"
        }
    )

let private mappedAssembly =
    Path.Combine(mappedLibrary.ProjectDir, "bin", $"{mappedLibrary.Name}.dll")

/// References the assembly, never the project it was built from.
let private consumer =
    { SyntheticProject.Create(
          "Consumer",
          { sourceFile "App" [] with
              SignatureFile = Custom $"val result: int\nval kind: {mappedLibrary.Name}.ModuleLibrary.Kind"
              Source = $"let result = {mappedLibrary.Name}.ModuleLibrary.value\nlet kind = {mappedLibrary.Name}.ModuleLibrary.Kind.A"
          }
      ) with
        OtherOptions = [ $"-r:{mappedAssembly}" ]
    }

/// A file of the library's name with a `value` on the library's line: the false match a wrong lookup would reach.
let private unrelated =
    SyntheticProject.Create(
        "Unrelated",
        { sourceFile "Library" [] with
            Source = $"{declarations}\nlet value = 99"
        }
    )

let private buildMappedAssembly =
    lazy
        (let options = mappedLibrary.GetProjectOptions(mappedLibrary.SaveAndCheck())
         Directory.CreateDirectory(Path.GetDirectoryName mappedAssembly) |> ignore

         File.WriteAllLines(
             Path.Combine(mappedLibrary.ProjectDir, "build.rsp"),
             [
                 "--target:library"
                 $"--pathmap:{mappedLibrary.ProjectDir}=."
                 $"-o:{mappedAssembly}"
                 yield! options.OtherOptions
                 yield! options.SourceFiles
             ]
         )

         let exitCode, output, errors =
             TestFramework.Commands.executeProcess TestFramework.initialConfig.FSC "@build.rsp" mappedLibrary.ProjectDir

         Assert.True((exitCode = 0), $"fsc exited with {exitCode}:\n{output}\n{errors}"))

let private solutionWith (projects: SyntheticProject list) =
    lazy
        (buildMappedAssembly.Force()

         let root =
             { SyntheticProject.Create("Root", sourceFile "Root" []) with
                 DependsOn = consumer :: projects
             }

         let struct (solution, _) = RoslynTestHelpers.CreateMultiProjectSolution root
         solution)

let private withSource = solutionWith [ mappedLibrary ]
let private withoutSource = solutionWith [ unrelated ]

let private mappedSolution (sourceLoaded: bool) =
    (if sourceLoaded then withSource else withoutSource).Value

let private consumerIn (solution: Solution) =
    documentAt solution (consumer.GetFilePath "App")

[<Theory>]
[<InlineData(false, "ModuleLibrary.value", Label = "only an unrelated file ends as the mapped name does")>]
[<InlineData(true, "ModuleLibrary.Kind", Label = "the mapped source gives no location: a type is declared in the signature only")>]
let ``goto definition into a mapped assembly stays external where`` (sourceLoaded: bool) (name: string) =
    match definitionAt (consumerIn (mappedSolution sourceLoaded)) name with
    | ValueSome(FSharpGoToDefinitionResult.ExternalAssembly _, _) -> ()
    | result -> failwith $"expected the external assembly, got %A{result}"

[<Theory>]
[<InlineData(false, "ModuleLibrary.value", "value", Label = "from an implementation file, to the implementation")>]
[<InlineData(true, "ModuleLibrary.Kind", "Kind", Label = "from a signature file, to the signature")>]
let ``goto definition into a mapped assembly reaches the file of the origin's kind``
    (fromSignature: bool)
    (name: string)
    (expected: string)
    =
    let origin, destination =
        if fromSignature then
            consumer.GetSignatureFilePath "App", mappedLibrary.GetSignatureFilePath "Library"
        else
            consumer.GetFilePath "App", mappedLibrary.GetFilePath "Library"

    match definitionAt (documentAt withSource.Value origin) name with
    | ValueSome(FSharpGoToDefinitionResult.NavigableItem item, _) ->
        Assert.Equal(destination, item.Document.FilePath)
        Assert.Equal(expected, item.Document.GetTextAsync(CancellationToken.None).Result.ToString(item.SourceSpan))
    | result -> failwith $"expected a navigable item, got %A{result}"

let private declarationOf (solution: Solution) =
    let document = consumerIn solution
    let position = lastCharacterOf document "ModuleLibrary.value"
    let text = document.GetTextAsync(CancellationToken.None).Result
    let line = text.Lines.GetLineFromPosition(position).ToString()
    let linePosition = text.Lines.GetLinePosition position
    let names = [ mappedLibrary.Name; "ModuleLibrary"; "value" ]

    let _, checkResults =
        document.GetFSharpParseAndCheckResultsAsync "test"
        |> CancellableTask.runSynchronouslyWithoutCancellation

    let symbolUse =
        checkResults.GetSymbolUseAtLocation(linePosition.Line + 1, linePosition.Character + 1, line, names)
        |> Option.defaultWith (fun () -> failwith "the consumer does not use the library's value")

    match checkResults.GetDeclarationLocation(linePosition.Line + 1, linePosition.Character + 1, line, names, false) with
    | FindDeclResult.DeclFound range -> struct (range, symbolUse.Symbol.Assembly.SimpleName)
    | result -> failwith $"expected the declaration of the library's value, got %A{result}"

let private declarationSpans (solution: Solution) =
    let struct (range, assembly) = declarationOf solution

    FSharpFindUsagesService.rangeToDocumentSpans (solution, range, "value", assembly)
    |> CancellableTask.runSynchronouslyWithoutCancellation

[<Fact>]
let ``find all references reports a mapped declaration only in the source of its own assembly`` () =
    Assert.Empty(declarationSpans withoutSource.Value)

    let span = declarationSpans withSource.Value |> Array.exactlyOne
    Assert.Equal(mappedLibrary.GetFilePath "Library", span.Document.FilePath)

let private findReferences (solution: Solution) =
    let context, definitions, references =
        FindReferencesTests.FindReferences.getContext ()

    let document = consumerIn solution

    FSharpFindUsagesService.findReferencedSymbolsAsync (document, lastCharacterOf document "ModuleLibrary.value", context, true, "test")
    |> CancellableTask.runSynchronouslyWithoutCancellation

    struct (definitions.Count, references.Count)

[<Theory>]
[<InlineData(false, 1, 1, Label = "the declaration and the consumer's use; an unrelated file ending as the name does is left out")>]
[<InlineData(true, 1, 3, Label = "the library's own two uses too: the project holding the assembly as a file is searched")>]
let ``find all references on a mapped symbol reports`` (sourceLoaded: bool) (definitions: int) (references: int) =
    Assert.Equal(struct (definitions, references), findReferences (mappedSolution sourceLoaded))

[<Fact>]
let ``a quick info link reaches the mapped source of an assembly the project reads`` () =
    let isLink (solution: Solution) =
        let struct (range, _) = declarationOf solution
        FSharpNavigation(FSharpMetadataAsSourceService(), consumerIn solution, Range.rangeStartup).IsTargetValid range

    Assert.True(isLink withSource.Value)
    Assert.False(isLink withoutSource.Value)
