// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// An F# library and a C# project referencing its built assembly, as VS wires a C# → F# project reference.
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
open Microsoft.CodeAnalysis.FindSymbols
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

let private solution =
    let struct (librarySolution, checker) =
        RoslynTestHelpers.CreateMultiProjectSolution library

    let assembly = RoslynTestHelpers.CompileToAssembly(library, checker)

    RoslynTestHelpers.AddCSharpProject(
        librarySolution,
        "Consumer",
        $"class Consumer {{ int M() => {moduleName}.twice(1); int N() => {moduleName}.answer; }}",
        library.GetProjectOptions checker,
        [ assembly ]
    )

let private consumer =
    solution.Projects |> Seq.find (fun p -> p.Language = LanguageNames.CSharp)

let private firstPath = library.GetFilePath "First"

let private declarationPosition =
    (File.ReadAllText firstPath).IndexOf("twice", StringComparison.Ordinal)

let private fsharpDocument =
    solution.GetDocumentIdsWithFilePath firstPath
    |> Seq.exactlyOne
    |> solution.GetDocument

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

[<Fact>]
let ``the C# compilation resolves the doc comment id of an F# function`` () =
    let compilation = consumer.GetCompilationAsync(CancellationToken.None).Result

    let errors =
        compilation.GetDiagnostics()
        |> Seq.filter (fun d -> d.Severity = DiagnosticSeverity.Error)

    Assert.Empty errors

    let twice =
        DocumentationCommentId.GetFirstSymbolForDeclarationId($"M:{moduleName}.twice(System.Int32)", compilation)

    Assert.NotNull twice

    let references =
        SymbolFinder.FindReferencesAsync(twice, solution, ImmutableHashSet.CreateRange consumer.Documents, CancellationToken.None).Result

    Assert.Single(references |> Seq.collect _.Locations) |> ignore

[<Fact>]
let ``the consumer is found as a project referencing the F# assembly`` () =
    let referencing =
        ProjectFiltering.getProjectsReferencingAssembly fsharpDocument.Project.OutputFilePath solution

    Assert.Equal<ProjectId list>([ consumer.Id ], referencing |> List.map _.Id)

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

[<Fact>]
let ``Find All References on an F# function reports its C# call site`` () =
    let context, foundDefinitions, foundReferences =
        RoslynTestHelpers.CreateFindUsagesContext()

    findUsagesService.FindReferencesAsync(fsharpDocument, declarationPosition, context).Wait()

    Assert.Equal(1, foundDefinitions.Count)
    let document, span = documentSpanOf (Assert.Single foundReferences)
    Assert.Equal(LanguageNames.CSharp, document.Project.Language)

    let text = document.GetTextAsync(CancellationToken.None).Result
    Assert.Equal("twice", text.ToString span)

[<Fact>]
let ``Find All References on an F# literal reports its C# use`` () =
    let context, _, foundReferences = RoslynTestHelpers.CreateFindUsagesContext()

    let position =
        (File.ReadAllText firstPath).IndexOf("answer", StringComparison.Ordinal)

    findUsagesService.FindReferencesAsync(fsharpDocument, position, context).Wait()

    let document, span = documentSpanOf (Assert.Single foundReferences)
    Assert.Equal(LanguageNames.CSharp, document.Project.Language)

    let text = document.GetTextAsync(CancellationToken.None).Result
    Assert.Equal("answer", text.ToString span)

[<Fact>]
let ``Find Implementations on an F# function does not report C# call sites`` () =
    let context, _, foundReferences = RoslynTestHelpers.CreateFindUsagesContext()

    findUsagesService.FindImplementationsAsync(fsharpDocument, declarationPosition, context).Wait()

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

    let outputs =
        [
            for _, target in consumers ->
                let directory = Path.Combine(Path.GetDirectoryName library.OutputFilename, target)
                Directory.CreateDirectory directory |> ignore
                let output = Path.Combine(directory, Path.GetFileName library.OutputFilename)
                File.Copy(library.OutputFilename, output, true)
                output
        ]

    let options =
        library.GetProjectOptions(librarySolution.GetFSharpWorkspaceService().Checker)

    let withConsumers =
        (librarySolution, List.zip consumers outputs)
        ||> List.fold (fun solution ((name, _), output) ->
            RoslynTestHelpers.AddCSharpProject(
                solution,
                name,
                $"class Consumer {{ int M() => {moduleName}.twice(1); }}",
                options,
                [ output ]
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

    findUsagesService.FindReferencesAsync(document, declarationPosition, context).Wait()

    let reportedIn =
        foundReferences
        |> Seq.map (fun reference -> (documentSpanOf reference |> fst).Project.Name)
        |> Seq.sort
        |> Seq.toList

    Assert.Equal<string list>(consumers |> List.map fst, reportedIn)
