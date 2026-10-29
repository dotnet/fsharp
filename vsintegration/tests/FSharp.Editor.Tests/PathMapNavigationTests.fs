// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// A library whose build maps its source paths, as DeterministicSourcePaths does: the symbols another
/// project imports from it must still name the files of the workspace.
module FSharp.Editor.Tests.PathMapNavigationTests

open System
open System.Collections.Concurrent
open System.IO
open System.Threading
open System.Threading.Tasks
open Xunit
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Editor.FindUsages
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.FindUsages
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

/// The one rule the document lookup and the search for a declaration inside a document both go through.
/// A mapped name arrives with the separator its replacement doubled (`.\` + `\rest`), which is what the
/// compiler writes and what a build on a path map hands back. Its `.` and `..` segments are resolved in the
/// name itself; one that climbs above where it starts denotes a file only from a root, which is not known.
[<Theory>]
[<InlineData(@"D:\repo\src\Lib\File.fs", @".\\src\Lib\File.fs", true)>]
[<InlineData(@"D:\repo\src\Lib\File.fs", @"./src/Lib/File.fs", true)>]
[<InlineData(@"D:\repo\src\Lib\File.fs", @"D:\repo\src\Lib\File.fs", true)>]
[<InlineData(@"D:\repo\src\Lib\File.fs", @"ib\File.fs", false)>]
[<InlineData(@"D:\repo\src\Lib\File.fs", @".\src\Other\File.fs", false)>]
[<InlineData(@"D:\repo\src\Lib\File.fs", @".\src\Scratch\..\Lib\File.fs", true)>]
[<InlineData(@"D:\repo\src\Lib\File.fs", @".\src\.\Lib\Deep\Deeper\..\..\File.fs", true)>]
[<InlineData(@"D:\repo\src\Lib\File.fs", @".\src\Lib\..\File.fs", false)>]
[<InlineData(@"D:\repo\src\Lib\File.fs", @"..\src\Lib\File.fs", false)>]
[<InlineData(@"D:\repo\src\Lib\File.fs", @".\Scratch\..\..\src\Lib\File.fs", false)>]
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

    match solution.TryGetDocumentIdFromFSharpRange(range, declaringAssembly = library.Name) with
    | Some documentId -> Assert.Equal(real, solution.GetDocument(documentId).FilePath)
    | None -> failwith $"no document is named by {relative}"

    // Without the assembly that declares what the range locates, nothing says the file is the one it names.
    Assert.Equal(None, solution.TryGetDocumentIdFromFSharpRange range)

[<Fact>]
let ``a relative name is matched by whole directories, not by the tail of one`` () =
    let real = library.GetFilePath "Library"
    let root = Path.GetDirectoryName library.ProjectDir
    let insideASegment = real.Substring(root.Length + 2)

    let range = Range.mkRange insideASegment (Position.mkPos 1 0) (Position.mkPos 1 0)

    match solution.TryGetDocumentIdFromFSharpRange(range, declaringAssembly = library.Name) with
    | Some documentId -> failwith $"{insideASegment} must not name {solution.GetDocument(documentId).FilePath}"
    | None -> ()

[<Fact>]
let ``a name this platform cannot spell names no document`` () =
    // A declaration imported from an assembly can carry a name written on another operating system.
    let foreign = "/home/build/a|b/Library.fs"
    let range = Range.mkRange foreign (Position.mkPos 1 0) (Position.mkPos 1 0)

    match solution.TryGetDocumentIdFromFSharpRange(range, declaringAssembly = library.Name) with
    | Some documentId -> failwith $"{foreign} must not name {solution.GetDocument(documentId).FilePath}"
    | None -> ()

[<Fact>]
let ``the documents a mapped name reaches are the same on every search`` () =
    let real = library.GetFilePath "Library"
    let root = Path.GetDirectoryName library.ProjectDir

    let relative =
        $".\{real.Substring(root.Length).TrimStart(Path.DirectorySeparatorChar)}"

    let reached name =
        solution.GetDocumentIdsWithFSharpFileName(name, library.Name)
        |> List.map (fun documentId -> solution.GetDocument(documentId).FilePath)

    Assert.Equal<string list>([ real ], reached relative)
    Assert.Equal<string list>(reached relative, reached relative)

    // A search asks for the same name once per project, and a name that reaches nothing is asked just as often.
    Assert.Empty(reached ".\Nowhere\Nothing.fs")
    Assert.Empty(reached ".\Nowhere\Nothing.fs")

/// A solution of bare projects, written `Assembly=path;path|Assembly=path`.
let private solutionOf (projects: string) =
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

/// The path of an unrelated file can end the way a mapped name does, so such a name is the file of a
/// declaration only where the projects compiling the declaring assembly, `Lib` here, hold exactly one file
/// ending that way. The rows are: that file alone; the source absent and an unrelated file in its place; an
/// unrelated file beside it; two files of the assembly; the one file in two projects of the assembly, as
/// multi-targeting has it; an equivalent name; and a name that needs the root.
[<Theory>]
[<InlineData(@"Lib=D:\repo\src\Lib\File.fs", @".\src\Lib\File.fs", @"D:\repo\src\Lib\File.fs")>]
[<InlineData(@"Other=D:\other\src\Lib\File.fs", @".\src\Lib\File.fs", "")>]
[<InlineData(@"Other=D:\other\src\Lib\File.fs|Lib=D:\repo\src\Lib\File.fs", @".\src\Lib\File.fs", @"D:\repo\src\Lib\File.fs")>]
[<InlineData(@"Lib=D:\repo\src\Lib\File.fs;D:\repo\copy\src\Lib\File.fs", @".\src\Lib\File.fs", "")>]
[<InlineData(@"Lib=D:\repo\src\Lib\File.fs|Lib=D:\repo\src\Lib\File.fs",
             @".\src\Lib\File.fs",
             @"D:\repo\src\Lib\File.fs;D:\repo\src\Lib\File.fs")>]
[<InlineData(@"Lib=D:\repo\src\Lib\File.fs", @".\src\Scratch\..\Lib\.\File.fs", @"D:\repo\src\Lib\File.fs")>]
[<InlineData(@"Lib=D:\repo\src\Lib\File.fs", @"..\src\Lib\File.fs", "")>]
let ``a mapped name reaches the one file of the declaring assembly that ends with it``
    (projects: string)
    (name: string)
    (expected: string)
    =
    let solution = solutionOf projects

    let reached =
        solution.GetDocumentIdsWithFSharpFileName(name, "Lib")
        |> List.map (fun documentId -> solution.GetDocument(documentId).FilePath)

    Assert.Equal<string list>(expected.Split([| ';' |], StringSplitOptions.RemoveEmptyEntries) |> List.ofArray, reached)

/// A library the solutions hold as an assembly on disk, compiled from its own directory with a path map as a
/// build under DeterministicSourcePaths compiles it: it names its files `.\FileLibrary.fsi` and `.\FileLibrary.fs`.
type private MappedBinary =
    {
        Library: SyntheticProject
        /// References the assembly, never the project it was built from.
        Consumer: SyntheticProject
        /// The consumer and an unrelated project with a file of the library's name: the library's source is absent.
        WithoutSource: Solution
        /// The consumer and the project the assembly was built from.
        WithSource: Solution
    }

let private mappedBinary =
    lazy
        (let declarations = "type Kind =\n    | A\n    | B\n"

         let library =
             SyntheticProject.Create(
                 "External",
                 { sourceFile "Library" [] with
                     SignatureFile = Custom $"{declarations}\nval value: int"
                     Source = $"{declarations}\nlet value = 42"
                 }
             )

         let options = library.GetProjectOptions(library.SaveAndCheck())
         let assembly = Path.Combine(library.ProjectDir, "bin", $"{library.Name}.dll")
         Directory.CreateDirectory(Path.GetDirectoryName assembly) |> ignore

         File.WriteAllLines(
             Path.Combine(library.ProjectDir, "build.rsp"),
             [
                 "--target:library"
                 $"--pathmap:{library.ProjectDir}=."
                 $"-o:{assembly}"
                 yield! options.OtherOptions
                 yield! options.SourceFiles
             ]
         )

         let exitCode, output, errors =
             TestFramework.Commands.executeProcess TestFramework.initialConfig.FSC "@build.rsp" library.ProjectDir

         Assert.True((exitCode = 0), $"fsc exited with {exitCode}:\n{output}\n{errors}")

         let consumer =
             { SyntheticProject.Create(
                   "Consumer",
                   { sourceFile "App" [] with
                       SignatureFile = Custom $"val result: int\nval kind: {library.Name}.ModuleLibrary.Kind"
                       Source = $"let result = {library.Name}.ModuleLibrary.value\nlet kind = {library.Name}.ModuleLibrary.Kind.A"
                   }
               ) with
                 OtherOptions = [ $"-r:{assembly}" ]
             }

         // Declares a value of the same name on the same line as the library does.
         let unrelated =
             SyntheticProject.Create(
                 "Unrelated",
                 { sourceFile "Library" [] with
                     Source = $"{declarations}\nlet value = 99"
                 }
             )

         let solutionWith (projects: SyntheticProject list) =
             let root =
                 { SyntheticProject.Create("Root", sourceFile "Root" []) with
                     DependsOn = projects
                 }

             let struct (solution, _) = RoslynTestHelpers.CreateMultiProjectSolution root
             solution

         {
             Library = library
             Consumer = consumer
             WithoutSource = solutionWith [ consumer; unrelated ]
             WithSource = solutionWith [ consumer; library ]
         })

let private documentAt (solution: Solution) (path: string) =
    solution.GetDocumentIdsWithFilePath path
    |> Seq.exactlyOne
    |> solution.GetDocument

/// The position of the last character of the name's first occurrence.
let private positionOf (document: Document) (name: string) =
    let text = document.GetTextAsync(CancellationToken.None).Result.ToString()
    text.IndexOf(name, StringComparison.Ordinal) + name.Length - 1

let private definitionAt (document: Document) (name: string) =
    GoToDefinition(FSharpMetadataAsSourceService()).FindDefinitionAtPosition(document, positionOf document name)
    |> CancellableTask.runSynchronouslyWithoutCancellation

[<Fact>]
let ``goto definition stays external when only an unrelated file ends as the mapped name does`` () =
    let binary = mappedBinary.Value

    let consumer = documentAt binary.WithoutSource (binary.Consumer.GetFilePath "App")

    match definitionAt consumer "ModuleLibrary.value" with
    | ValueSome(FSharpGoToDefinitionResult.ExternalAssembly _, _) -> ()
    | result -> failwith $"expected the external assembly, got %A{result}"

/// The assembly was built from a signature and an implementation. From an implementation file the definition
/// is the one in the implementation, though the symbol declares itself in the signature; from a signature
/// file it is the one in the signature.
[<Theory>]
[<InlineData(false, "ModuleLibrary.value", "value")>]
[<InlineData(true, "ModuleLibrary.Kind", "Kind")>]
let ``goto definition into a mapped assembly with a signature reaches the file of the origin's kind``
    (fromSignature: bool)
    (name: string)
    (expected: string)
    =
    let binary = mappedBinary.Value

    let origin, destination =
        if fromSignature then
            binary.Consumer.GetSignatureFilePath "App", binary.Library.GetSignatureFilePath "Library"
        else
            binary.Consumer.GetFilePath "App", binary.Library.GetFilePath "Library"

    match definitionAt (documentAt binary.WithSource origin) name with
    | ValueSome(FSharpGoToDefinitionResult.NavigableItem item, _) ->
        Assert.Equal(destination, item.Document.FilePath)
        Assert.Equal(expected, item.Document.GetTextAsync(CancellationToken.None).Result.ToString(item.SourceSpan))
    | result -> failwith $"expected a navigable item, got %A{result}"

/// The assembly records where a type is declared in the signature only, so from an implementation file its
/// source gives no location: the definition is then the external one it was before a mapped name reached a
/// document, not none at all.
[<Fact>]
let ``goto definition falls back to the assembly when the mapped source gives no location`` () =
    let binary = mappedBinary.Value
    let consumer = documentAt binary.WithSource (binary.Consumer.GetFilePath "App")

    match definitionAt consumer "ModuleLibrary.Kind" with
    | ValueSome(FSharpGoToDefinitionResult.ExternalAssembly _, _) -> ()
    | result -> failwith $"expected the external assembly, got %A{result}"

/// The spans Find All References reports the declaration of the library's value at.
let private declarationSpans (solution: Solution) (binary: MappedBinary) =
    cancellableTask {
        let consumer = documentAt solution (binary.Consumer.GetFilePath "App")
        let! cancellationToken = CancellableTask.getCancellationToken ()
        let! text = consumer.GetTextAsync cancellationToken
        let position = positionOf consumer "ModuleLibrary.value"
        let line = text.Lines.GetLineFromPosition(position).ToString()
        let linePosition = text.Lines.GetLinePosition position
        let names = [ binary.Library.Name; "ModuleLibrary"; "value" ]
        let! _, checkResults = consumer.GetFSharpParseAndCheckResultsAsync "test"

        let symbolUse =
            checkResults.GetSymbolUseAtLocation(linePosition.Line + 1, linePosition.Character + 1, line, names)
            |> Option.defaultWith (fun () -> failwith "the consumer does not use the library's value")

        match checkResults.GetDeclarationLocation(linePosition.Line + 1, linePosition.Character + 1, line, names, false) with
        | FindDeclResult.DeclFound range ->
            return! FSharpFindUsagesService.rangeToDocumentSpans (solution, range, "value", symbolUse.Symbol.Assembly.SimpleName)
        | result -> return failwith $"expected the declaration of the library's value, got %A{result}"
    }
    |> CancellableTask.runSynchronouslyWithoutCancellation

[<Fact>]
let ``find all references reports a mapped declaration only in the source of its own assembly`` () =
    let binary = mappedBinary.Value

    Assert.Empty(declarationSpans binary.WithoutSource binary)

    let span = declarationSpans binary.WithSource binary |> Array.exactlyOne
    Assert.Equal(binary.Library.GetFilePath "Library", span.Document.FilePath)

[<Fact>]
let ``find all references keeps the use of a mapped symbol when only an unrelated file ends as its name does`` () =
    let binary = mappedBinary.Value

    let consumer = documentAt binary.WithoutSource (binary.Consumer.GetFilePath "App")

    let definitions = ConcurrentBag()
    let references = ConcurrentBag()

    let context =
        { new IFSharpFindUsagesContext with
            member _.OnDefinitionFoundAsync(definition: FSharpDefinitionItem) =
                definitions.Add definition
                Task.CompletedTask

            member _.OnReferenceFoundAsync(reference: FSharpSourceReferenceItem) =
                references.Add reference
                Task.CompletedTask

            member _.ReportMessageAsync _ = Task.CompletedTask
            member _.ReportProgressAsync(_, _) = Task.CompletedTask
            member _.SetSearchTitleAsync _ = Task.CompletedTask
            member _.CancellationToken = CancellationToken.None
        }

    FSharpFindUsagesService.findReferencedSymbolsAsync (consumer, positionOf consumer "ModuleLibrary.value", context, true, "test")
    |> CancellableTask.runSynchronouslyWithoutCancellation

    // What an item holds cannot be read from here: the declaration is counted, and so is the consumer's one use.
    Assert.Equal(1, definitions.Count)
    Assert.Equal(1, references.Count)
