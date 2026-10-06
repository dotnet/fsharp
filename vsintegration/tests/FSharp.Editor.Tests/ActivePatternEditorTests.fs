module FSharp.Editor.Tests.ActivePatternEditorTests

open System
open System.Collections.Immutable
open System.Threading

open Microsoft.CodeAnalysis.Completion
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop

open FSharp.Editor.Tests.CodeFixes.CodeFixTestFramework
open FSharp.Editor.Tests.Helpers
open Xunit

let private definitions =
    """namespace Candidates

module Normal =
    let (|Even|Odd|) value = if value % 2 = 0 then Even value else Odd value
    let (|Positive|_|) value = if value > 0 then Some value else None
    let (|Above|_|) threshold value = if value > threshold then Some value else None
    let internal (|Internal|_|) value = Some value
    let private (|Private|_|) value = Some value
    [<CompilerMessage("Hidden", 1234, IsHidden = true)>]
    let (|Hidden|_|) value = Some value
    [<System.Obsolete("Use Positive")>]
    let (|Old|_|) value = Some value

module Other =
    let (|Positive|_|) value = Some value

module private PrivateModule =
    let (|Secret|_|) value = Some value

[<AutoOpen>]
module Auto =
    let (|AutoCase|_|) value = Some value
    [<AutoOpen>]
    module Nested =
        let (|Deep|_|) value = Some value
    module Ordinary =
        let (|OrdinaryCase|_|) value = Some value

[<RequireQualifiedAccess>]
module Qualified =
    let (|Restricted|_|) value = Some value

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Suffix =
    let (|SuffixCase|_|) value = Some value

module ``Space module`` =
    let (|``Has space``|_|) value = Some value

"""

let private source (body: string) =
    definitions + $"namespace Consumer\n\nmodule Use =\n\n    {body}\n"

let private withOpen atTop (ns: string) (code: string) =
    if atTop then
        code.Replace("namespace Consumer\n\n", $"namespace Consumer\n\nopen {ns}\n\n")
    else
        code.Replace("module Use =\n\n", $"module Use =\n\n    open {ns}\n\n")

let private assertCompiles code =
    let document = RoslynTestHelpers.GetFsDocument code

    let diagnostics =
        FSharpDiagnostics.generate Auto document
        |> CancellableTask.runSynchronouslyWithoutCancellation

    Assert.Empty diagnostics

[<Theory>]
[<InlineData("Even", "", "Candidates.Normal", true)>]
[<InlineData("Positive", "", "Candidates.Normal", false)>]
[<InlineData("Above", " 0", "Candidates.Normal", true)>]
[<InlineData("AutoCase", "", "Candidates", false)>]
[<InlineData("Deep", "", "Candidates", true)>]
[<InlineData("OrdinaryCase", "", "Candidates.Auto.Ordinary", false)>]
[<InlineData("SuffixCase", "", "Candidates.Suffix", true)>]
[<InlineData("Internal", "", "Candidates.Normal", false)>]
[<InlineData("``Has space``", "", "Candidates.``Space module``", true)>]
[<InlineData("Normal.Positive", "", "Candidates", true)>]
[<InlineData("Qualified.Restricted", "", "Candidates", true)>]
let ``Add Open applied case uses exact target and placement`` (caseName: string, parameters: string, ns: string, atTop: bool) =
    let code =
        source $"let classify value = match value with | {caseName}{parameters} n -> n | _ -> 0"

    let provider = AddOpenCodeFixProvider(AssemblyContentProvider())

    let mode =
        WithSettings
            { CodeFixesOptions.Default with
                AlwaysPlaceOpensAtTopLevel = atTop
            }

    let fix = provider |> tryFix code mode |> Option.get
    Assert.Equal($"open {ns}", fix.Message)
    Assert.Equal(withOpen atTop ns code, fix.FixedCode.Replace("\r\n", "\n"))
    assertCompiles fix.FixedCode
    Assert.Equal(None, provider |> tryFix fix.FixedCode mode)

[<Fact>]
let ``RQA bare case fix supplies the qualification still required`` () =
    let code =
        source "let classify value = match value with | Restricted n -> n | _ -> 0"

    let fix =
        AddOpenCodeFixProvider(AssemblyContentProvider())
        |> tryFix code Auto
        |> Option.get

    Assert.Equal("Candidates.Qualified.Restricted", fix.Message)
    let expected = code.Replace("| Restricted n", "| Candidates.Qualified.Restricted n")

    Assert.Equal(expected, fix.FixedCode.Replace("\r\n", "\n"))
    assertCompiles fix.FixedCode

[<Theory>]
[<InlineData("let value = Positive 1")>]
[<InlineData("let classify value = match value with | Private n -> n | _ -> 0")>]
[<InlineData("let classify value = match value with | Secret n -> n | _ -> 0")>]
[<InlineData("let classify value = match value with | Hidden n -> n | _ -> 0")>]
[<InlineData("let classify value = match value with | Old n -> n | _ -> 0")>]
[<InlineData("let Even = 1")>]
let ``Add Open does not suggest unusable case edits`` body =
    let code = source body
    let actual = AddOpenCodeFixProvider(AssemblyContentProvider()) |> tryFix code Auto
    Assert.Equal(None, actual)

    if body = "let Even = 1" then
        assertCompiles code

[<Fact>]
let ``Add Open does not duplicate an already visible case open`` () =
    let code =
        source "let classify value = match value with | Positive n -> n | _ -> 0"
        |> withOpen true "Candidates.Normal"

    assertCompiles code
    Assert.Equal(None, AddOpenCodeFixProvider(AssemblyContentProvider()) |> tryFix code Auto)

let private serviceProvider =
    let xmlIndex =
        { new IVsXMLMemberIndexService with
            member _.CreateXMLMemberIndex(_, _) =
                failwith "Completion commit must not request documentation"

            member _.GetMemberDataFromXML(_, _) =
                failwith "Completion commit must not request documentation"
        }

    let documentation =
        { new IDocumentationBuilder with
            override _.AppendDocumentationFromProcessedXML(_, _, _, _, _, _, _) =
                failwith "Completion commit must not request documentation"

            override _.AppendDocumentation(_, _, _, _, _, _, _, _) =
                failwith "Completion commit must not request documentation"
        }

    XmlDocumentation.documentationBuilderCache.Add(xmlIndex, documentation)

    { new SVsServiceProvider

      interface IServiceProvider with
          member _.GetService serviceType =
              if serviceType = typeof<SVsXMLMemberIndexService> then
                  box xmlIndex
              else
                  failwith $"Unexpected service: {serviceType}"
    }

[<Theory>]
[<InlineData("Positive", "Candidates.Normal", true, false)>]
[<InlineData("Positive", "Candidates.Other", false, false)>]
[<InlineData("Deep", "Candidates", false, false)>]
[<InlineData("Qualified.Restricted", "Candidates", true, false)>]
[<InlineData("``Has space``", "Candidates.``Space module``", false, false)>]
[<InlineData("Positive", "Candidates.Normal", false, true)>]
let ``completion commit applies case text and avoids duplicate opens`` (nameInCode: string, ns: string, atTop: bool, alreadyOpen: bool) =
    let placeholder = "Missing"

    let initial =
        source $"let classify value = match value with | {placeholder} n -> n | _ -> 0"

    let code = if alreadyOpen then withOpen atTop ns initial else initial

    let document =
        RoslynTestHelpers.GetFsDocument(
            code,
            customEditorOptions =
                { CodeFixesOptions.Default with
                    AlwaysPlaceOpensAtTopLevel = atTop
                }
        )

    let catalogue = AssemblyContentProvider()
    let position = code.IndexOf(placeholder, StringComparison.Ordinal)

    let item =
        FSharpCompletionProvider.ProvideCompletionsAsyncAux(
            document,
            position + placeholder.Length,
            catalogue.GetAllEntitiesInProjectAndReferencedAssemblies,
            false
        )
        |> CancellableTask.runSynchronouslyWithoutCancellation
        |> Seq.filter (fun item ->
            let actual =
                match item.Properties.TryGetValue "NameInCode" with
                | true, value -> value
                | _ -> item.DisplayText

            actual = nameInCode
            && if alreadyOpen then
                   not (item.Properties.ContainsKey "NamespaceToOpen")
               else
                   match item.Properties.TryGetValue "NamespaceToOpen" with
                   | true, value -> value = ns
                   | _ -> false)
        |> Assert.Single

    if alreadyOpen then
        Assert.False(item.Properties.ContainsKey "NamespaceToOpen")
    else
        Assert.Equal(ns, item.Properties["NamespaceToOpen"])

    let workspace = document.Project.Solution.Workspace

    let provider =
        FSharpCompletionProvider(workspace, serviceProvider, catalogue, workspace.Services.GetService<EditorOptions>())

    let item =
        CompletionList.Create(TextSpan(position, placeholder.Length), ImmutableArray.Create item).ItemsList[0]

    let change =
        provider.GetChangeAsync(document, item, Nullable<char>(), CancellationToken.None).GetAwaiter().GetResult()

    let actual = (SourceText.From(code).WithChanges change.TextChange).ToString()
    let committed = code.Replace(placeholder, nameInCode)

    let expected =
        if alreadyOpen then
            committed
        else
            withOpen atTop ns committed

    Assert.Equal(expected, actual.Replace("\r\n", "\n"))
    assertCompiles actual

[<Theory>]
[<InlineData("let classify value = match value with | Missing n -> n | _ -> 0", false, false)>]
[<InlineData("let classify value = match value with | Missing n -> n | _ -> 0", true, true)>]
[<InlineData("let value = Missing", true, false)>]
let ``completion respects pattern context and caller unopened catalogue setting`` (body: string, includeUnopened: bool, hasCase: bool) =
    let code = source body
    let document = RoslynTestHelpers.GetFsDocument code
    let catalogue = AssemblyContentProvider()

    let symbols results =
        if includeUnopened then
            catalogue.GetAllEntitiesInProjectAndReferencedAssemblies results
        else
            [||]

    let position = code.IndexOf("Missing", StringComparison.Ordinal) + "Missing".Length

    let items =
        FSharpCompletionProvider.ProvideCompletionsAsyncAux(document, position, symbols, false)
        |> CancellableTask.runSynchronouslyWithoutCancellation

    Assert.Equal(hasCase, items |> Seq.exists (fun item -> item.DisplayText = "Positive"))
