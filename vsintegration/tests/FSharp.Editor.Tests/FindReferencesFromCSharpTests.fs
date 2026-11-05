// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// An F# library and C# and VB projects referencing its built assembly, as VS wires a C# or VB → F# project reference.
module FSharp.Editor.Tests.FindReferencesFromCSharpTests

open System
open System.Collections.Immutable
open System.IO
open System.Reflection
open System.Threading
open Xunit
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Editor.FindUsages
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.FindUsages
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks
open FSharp.Editor.Tests.Helpers
open FSharp.Test.ProjectGeneration

let private library =
    SyntheticProject.Create(
        { sourceFile "First" [] with
            ExtraSource =
                "let twice x = x * 2
[<Literal>]
let answer = 42
type Color =
    | Red = 0
    | Blue = 1
"
        }
    )

/// The synthetic project puts its modules in a namespace named after the project.
let private moduleName = $"{library.Name}.ModuleFirst"

/// A consumer references the copy of the assembly in its own output, not the file the F# project writes.
let private copiedTo (folder: string) =
    let directory = Path.Combine(Path.GetDirectoryName library.OutputFilename, folder)
    Directory.CreateDirectory directory |> ignore
    let copy = Path.Combine(directory, Path.GetFileName library.OutputFilename)
    File.Copy(library.OutputFilename, copy, true)
    copy

let private solution =
    let struct (librarySolution, checker) =
        RoslynTestHelpers.CreateMultiProjectSolution library

    RoslynTestHelpers.CompileToAssembly(library, checker) |> ignore
    let options = library.GetProjectOptions checker

    let csharp =
        $"class Consumer {{ int M() => {moduleName}.twice(1); int N() => {moduleName}.answer; int O() => (int){moduleName}.Color.Red; }}"

    let vb =
        $"Class Consumer\n    Dim a = {moduleName}.twice(1)\n    Dim b = {moduleName}.answer\n    Dim c = {moduleName}.Color.Red\nEnd Class"

    (librarySolution,
     [
         LanguageNames.CSharp, "CSharpConsumer", csharp
         LanguageNames.VisualBasic, "VbConsumer", vb
     ])
    ||> List.fold (fun solution (language, name, source) ->
        RoslynTestHelpers.AddProject(solution, language, name, source, options, ImmutableArray.Create(copiedTo name)))

let private firstPath = library.GetFilePath "First"

let private positionOf (name: string) =
    (File.ReadAllText firstPath).IndexOf(name, StringComparison.Ordinal)

let private fsharpDocumentIn (solution: Solution) =
    solution.GetDocumentIdsWithFilePath firstPath
    |> Seq.exactlyOne
    |> solution.GetDocument

let private fsharpDocument = fsharpDocumentIn solution

let private findUsagesService =
    FSharpFindUsagesService() :> IFSharpFindUsagesService

/// ExternalAccess exposes no span on a reference item; its Roslyn item is read through reflection.
let private documentSpanOf (reference: FSharpSourceReferenceItem) =
    let flags = BindingFlags.Instance ||| BindingFlags.NonPublic ||| BindingFlags.Public

    let property (target: obj) name =
        target.GetType().GetProperty(name, flags).GetValue target

    let documentSpan =
        property (property reference "RoslynSourceReferenceItem") "SourceSpan"

    property documentSpan "Document" :?> Document, property documentSpan "SourceSpan" :?> TextSpan

let private textOf (document: Document) =
    document.GetTextAsync(CancellationToken.None).Result

[<Fact>]
let ``the consumers are found as projects referencing the F# assembly`` () =
    let referencing =
        ProjectFiltering.getProjectsReferencingAssembly fsharpDocument.Project.OutputFilePath solution

    Assert.Equal<string list>([ "CSharpConsumer"; "VbConsumer" ], referencing |> List.map _.Name)

[<Theory>]
[<InlineData("twice", "M:{0}.twice(System.Int32)")>]
[<InlineData("ModuleFirst", "T:{0}")>]
[<InlineData("answer", "F:{0}.answer")>]
[<InlineData("Red", "F:{0}.Color.Red")>]
[<InlineData("x", null)>]
let ``DocumentationCommentId is the compiled form Roslyn resolves`` (symbolName: string, expectedFormat: string) =
    let expected =
        expectedFormat
        |> ValueOption.ofObj
        |> ValueOption.map (fun format -> String.Format(format, moduleName))

    let _, checkFileResults =
        fsharpDocument.GetFSharpParseAndCheckResultsAsync "test"
        |> CancellableTask.runSynchronouslyWithoutCancellation

    let symbol =
        checkFileResults.GetAllUsesOfAllSymbolsInFile()
        |> Seq.find (fun symbolUse -> symbolUse.IsFromDefinition && symbolUse.Symbol.DisplayName = symbolName)
        |> _.Symbol

    Assert.Equal(expected, symbol.DocumentationCommentId)

[<Theory>]
[<InlineData("twice")>]
[<InlineData("answer")>]
[<InlineData("Red")>]
let ``Find All References on an F# symbol reports its C# and VB uses`` (symbolName: string) =
    let context, foundDefinitions, foundReferences =
        RoslynTestHelpers.CreateFindUsagesContext()

    findUsagesService.FindReferencesAsync(fsharpDocument, positionOf symbolName, context).Wait()

    Assert.Single foundDefinitions |> ignore

    let used =
        foundReferences
        |> Seq.map (fun reference ->
            let document, span = documentSpanOf reference
            document.Project.Language, (textOf document).ToString span)
        |> Seq.sort
        |> Seq.toList

    Assert.Equal<(string * string) list>([ LanguageNames.CSharp, symbolName; LanguageNames.VisualBasic, symbolName ], used)

[<Fact>]
let ``Find All References on an F# function reports a call through an extern alias, not the namesake in the consumer`` () =
    let alias = "FSLib"
    let assembly = copiedTo "AliasedConsumer"

    let source =
        $"extern alias {alias};\nnamespace {library.Name} {{ public static class ModuleFirst {{ public static int twice(int x) => x; }} }}\nclass Consumer {{\n    int Real() => {alias}::{moduleName}.twice(1);\n    int Other() => {moduleName}.twice(1);\n}}"

    let options =
        library.GetProjectOptions(solution.GetFSharpWorkspaceService().Checker)

    let withConsumer =
        RoslynTestHelpers.AddProject(solution, LanguageNames.CSharp, "AliasedConsumer", source, options, ImmutableArray.Create assembly)

    let consumer =
        withConsumer.Projects
        |> Seq.find (fun project -> project.Name = "AliasedConsumer")

    let aliasedReferences =
        consumer.MetadataReferences
        |> Seq.map (function
            | :? PortableExecutableReference as reference when reference.FilePath = assembly ->
                reference.WithAliases [ alias ] :> MetadataReference
            | reference -> reference)

    let aliased =
        withConsumer.WithProjectMetadataReferences(consumer.Id, aliasedReferences)

    let context, _, foundReferences = RoslynTestHelpers.CreateFindUsagesContext()

    findUsagesService.FindReferencesAsync(fsharpDocumentIn aliased, positionOf "twice", context).Wait()

    let linesInConsumer =
        foundReferences
        |> Seq.map documentSpanOf
        |> Seq.filter (fun (document, _) -> document.Project.Name = consumer.Name)
        |> Seq.map (fun (document, span) -> (textOf document).Lines.GetLineFromPosition(span.Start).ToString())
        |> Seq.toList

    Assert.Contains("Real", Assert.Single linesInConsumer)

[<Fact>]
let ``Find Implementations on an F# function does not report C# call sites`` () =
    let context, _, foundReferences = RoslynTestHelpers.CreateFindUsagesContext()

    findUsagesService.FindImplementationsAsync(fsharpDocument, positionOf "twice", context).Wait()

    Assert.Empty foundReferences

/// The library as a project of two target frameworks, each a Roslyn project that writes the assembly under
/// the same name to a folder of its own, and one C# consumer per target that references that target's file.
[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``Find All References on a multi-targeted F# library reports the C# consumers of every target`` (anotherProjectWritesTheName: bool) =
    let consumers = [ "Consumer8", "net8.0"; "Consumer9", "net9.0" ]

    let struct (librarySolution, instances) =
        RoslynTestHelpers.CreateMultiTargetSolution(library, [ for _ in consumers -> { Defines = []; ExcludedFileIds = [] } ])

    let outputs = [ for _, target in consumers -> copiedTo target ]

    let options =
        library.GetProjectOptions(librarySolution.GetFSharpWorkspaceService().Checker)

    let withConsumers =
        (librarySolution, List.zip consumers outputs)
        ||> List.fold (fun solution ((name, _), output) ->
            RoslynTestHelpers.AddProject(
                solution,
                LanguageNames.CSharp,
                name,
                $"class Consumer {{ int M() => {moduleName}.twice(1); }}",
                options,
                ImmutableArray.Create output
            ))

    let withOutputs =
        (withConsumers, List.zip instances outputs)
        ||> List.fold (fun solution (instance, output) -> solution.WithProjectOutputFilePath(instance, output))

    let solution =
        if anotherProjectWritesTheName then
            let other = withOutputs.AddProject("Other", "Other", LanguageNames.CSharp)
            other.Solution.WithProjectOutputFilePath(other.Id, Path.Combine("C:\\Other", Path.GetFileName library.OutputFilename))
        else
            withOutputs

    let document =
        solution.GetProject(List.head instances).Documents
        |> Seq.find (fun document -> document.FilePath = firstPath)

    let context, _, foundReferences = RoslynTestHelpers.CreateFindUsagesContext()

    findUsagesService.FindReferencesAsync(document, positionOf "twice", context).Wait()

    let reportedIn =
        foundReferences
        |> Seq.map (fun reference -> (documentSpanOf reference |> fst).Project.Name)
        |> Seq.sort
        |> Seq.toList

    Assert.Equal<string list>(consumers |> List.map fst, reportedIn)
