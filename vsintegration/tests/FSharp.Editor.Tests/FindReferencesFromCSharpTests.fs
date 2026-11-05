// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.FindReferencesFromCSharpTests

open System
open System.Collections.Immutable
open System.IO
open Xunit
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Editor.FindUsages
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks
open FSharp.Editor.Tests.Helpers
open FSharp.Test.ProjectGeneration

let private library =
    SyntheticProject.Create(
        { sourceFile "First" [] with
            ExtraSource =
                "let twice x = x * 2\n[<Literal>]\nlet answer = 42\ntype Color = Red = 0 | Blue = 1\n"
                + "type Generic<'T>() =\n    static member (+) (a: Generic<'T>, _: Generic<'T>) = a\n"
        }
    )

let private moduleName = $"{library.Name}.ModuleFirst"

// A reference to the F# project's own output becomes a project reference once another project is added: consumers get copies.
let private copiedTo (folder: string) =
    let directory = Path.Combine(Path.GetDirectoryName library.OutputFilename, folder)
    Directory.CreateDirectory directory |> ignore
    let copy = Path.Combine(directory, Path.GetFileName library.OutputFilename)
    File.Copy(library.OutputFilename, copy, true)
    copy

let private withConsumer language name source (assemblies: string list) (solution: Solution) =
    let options =
        library.GetProjectOptions(solution.GetFSharpWorkspaceService().Checker)

    RoslynTestHelpers.AddProject(solution, language, name, source, options, ImmutableArray.CreateRange assemblies)

let private csharpConsumer =
    $"class Consumer {{ int M() => {moduleName}.twice(1); int N() => {moduleName}.answer; int O() => (int){moduleName}.Color.Red; object P({moduleName}.Generic<int> a, {moduleName}.Generic<int> b) => a + b; }}"

let private vbConsumer =
    $"Class Consumer\n    Dim a = {moduleName}.twice(1)\n    Dim b = {moduleName}.answer\n    Dim c = {moduleName}.Color.Red\n    Function P(x As {moduleName}.Generic(Of Integer), y As {moduleName}.Generic(Of Integer)) As Object\n        Return x + y\n    End Function\nEnd Class"

let private solution =
    let struct (librarySolution, checker) =
        RoslynTestHelpers.CreateMultiProjectSolution library

    RoslynTestHelpers.CompileToAssembly(library, checker) |> ignore

    librarySolution
    |> withConsumer LanguageNames.CSharp "CSharpConsumer" csharpConsumer [ copiedTo "CSharpConsumer" ]
    |> withConsumer LanguageNames.VisualBasic "VbConsumer" vbConsumer [ copiedTo "VbConsumer" ]

let private firstPath = library.GetFilePath "First"

let private positionOf name =
    FindReferencesTests.getPositionOf name firstPath

let private fsharpDocument = solution.TryGetDocumentFromPath(firstPath).Value

let private findUsagesService =
    FSharpFindUsagesService() :> IFSharpFindUsagesService

[<Fact>]
let ``the consumers are found as projects referencing the F# assembly`` () =
    let referencing =
        ProjectFiltering.getProjectsReferencingAssembly fsharpDocument.Project.OutputFilePath solution

    Assert.Equal<string>([ "CSharpConsumer"; "VbConsumer" ], referencing |> Seq.map _.Name)

[<Fact>]
let ``a reference without a file path is not the F# assembly even when another project writes its name`` () =
    let added =
        solution
        |> withConsumer LanguageNames.CSharp "ExactConsumer" csharpConsumer []
        |> withConsumer LanguageNames.CSharp "InMemoryConsumer" csharpConsumer []

    let idOf name =
        (added.Projects |> Seq.find (fun project -> project.Name = name)).Id

    let other = added.AddProject("Other", "Other", LanguageNames.CSharp)

    let referencing =
        other.Solution
            .AddMetadataReference(idOf "ExactConsumer", MetadataReference.CreateFromFile library.OutputFilename)
            .AddMetadataReference(idOf "InMemoryConsumer", MetadataReference.CreateFromImage(File.ReadAllBytes library.OutputFilename))
            .WithProjectOutputFilePath(other.Id, Path.Combine("C:\\Other", Path.GetFileName library.OutputFilename))
        |> ProjectFiltering.getProjectsReferencingAssembly fsharpDocument.Project.OutputFilePath

    Assert.Equal<string>([ "ExactConsumer" ], referencing |> Seq.map _.Name)

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

let private usesAt (position: int) =
    let context, definitions, references = RoslynTestHelpers.CreateFindUsagesContext()
    findUsagesService.FindReferencesAsync(fsharpDocument, position, context).Wait()
    Assert.Single definitions |> ignore

    references
    |> Seq.map (fun reference ->
        let document, span = InlineRenameTests.getReferenceSpan reference
        document.Project.Language, document.GetTextAsync().Result.ToString span)
    |> Seq.sort

[<Theory>]
[<InlineData("twice")>]
[<InlineData("answer")>]
[<InlineData("Red")>]
let ``Find All References on an F# symbol reports its C# and VB uses`` (symbolName: string) =
    Assert.Equal<string * string>(
        [ LanguageNames.CSharp, symbolName; LanguageNames.VisualBasic, symbolName ],
        usesAt (positionOf symbolName)
    )

[<Fact>]
let ``Find All References on an operator of a generic F# type reports its C# and VB uses`` () =
    Assert.Equal<string * string>([ LanguageNames.CSharp, "+"; LanguageNames.VisualBasic, "+" ], usesAt (positionOf "(+) (a" + 1))

[<Fact>]
let ``Find All References on an F# function reports a call through an extern alias, not the namesake in the consumer`` () =
    let alias = "FSLib"
    let name = "AliasedConsumer"

    let source =
        $"extern alias {alias};\nnamespace {library.Name} {{ public static class ModuleFirst {{ public static int twice(int x) => x; }} }}\nclass Consumer {{\n    int Real() => {alias}::{moduleName}.twice(1);\n    int Other() => {moduleName}.twice(1);\n}}"

    let added = solution |> withConsumer LanguageNames.CSharp name source []
    let consumer = added.Projects |> Seq.find (fun project -> project.Name = name)

    let aliased =
        added.AddMetadataReference(consumer.Id, MetadataReference.CreateFromFile(copiedTo name).WithAliases [ alias ])

    let context, _, references = RoslynTestHelpers.CreateFindUsagesContext()
    findUsagesService.FindReferencesAsync(aliased.GetDocument(fsharpDocument.Id), positionOf "twice", context).Wait()

    let linesInConsumer =
        references
        |> Seq.map InlineRenameTests.getReferenceSpan
        |> Seq.filter (fun (document, _) -> document.Project.Name = name)
        |> Seq.map (fun (document, span) -> document.GetTextAsync().Result.Lines.GetLineFromPosition(span.Start).ToString())

    Assert.Contains("Real", Assert.Single linesInConsumer)

[<Fact>]
let ``Find Implementations on an F# function does not report C# call sites`` () =
    let context, _, references = RoslynTestHelpers.CreateFindUsagesContext()
    findUsagesService.FindImplementationsAsync(fsharpDocument, positionOf "twice", context).Wait()
    Assert.Empty references

[<Theory>]
[<InlineData(false, false)>]
[<InlineData(true, false)>]
[<InlineData(true, true)>]
let ``Find All References on a multi-targeted F# library reports the C# consumers of every target``
    (anotherProjectWritesTheName: bool, consumersReferenceTheReferenceAssembly: bool)
    =
    let consumers = [ "Consumer8"; "Consumer9" ]

    let struct (librarySolution, instances) =
        RoslynTestHelpers.CreateMultiTargetSolution(library, [ for _ in consumers -> { Defines = []; ExcludedFileIds = [] } ])

    let outputs = consumers |> List.map copiedTo

    let withConsumers =
        (librarySolution, List.zip consumers outputs)
        ||> List.fold (fun solution (name, output) -> solution |> withConsumer LanguageNames.CSharp name csharpConsumer [ output ])

    let withOutputs =
        (withConsumers, List.zip instances outputs)
        ||> List.fold (fun solution (instance, output) ->
            if consumersReferenceTheReferenceAssembly then
                solution.WithProjectOutputFilePath(instance, library.OutputFilename).WithProjectOutputRefFilePath(instance, output)
            else
                solution.WithProjectOutputFilePath(instance, output))

    let solution =
        if anotherProjectWritesTheName then
            let other = withOutputs.AddProject("Other", "Other", LanguageNames.CSharp)
            other.Solution.WithProjectOutputFilePath(other.Id, Path.Combine("C:\\Other", Path.GetFileName library.OutputFilename))
        else
            withOutputs

    let document = solution.GetProject(List.head instances).Documents |> Seq.exactlyOne
    let context, _, references = RoslynTestHelpers.CreateFindUsagesContext()
    findUsagesService.FindReferencesAsync(document, positionOf "twice", context).Wait()

    let reportedIn =
        references
        |> Seq.map (fun reference -> (InlineRenameTests.getReferenceSpan reference |> fst).Project.Name)
        |> Seq.sort

    Assert.Equal<string>(consumers, reportedIn)
