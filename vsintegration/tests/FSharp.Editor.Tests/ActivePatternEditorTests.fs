module FSharp.Editor.Tests.ActivePatternEditorTests

open System
open System.Collections.Immutable
open System.Threading

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Completion
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop

open FSharp.Compiler.EditorServices
open FSharp.Compiler.Symbols
open FSharp.Editor.Tests.CodeFixes.CodeFixTestFramework
open FSharp.Editor.Tests.Helpers
open FSharp.Test.Compiler
open Xunit

let private definitions =
    """namespace Candidates

module Normal =
    let (|Even|Odd|) value = if value % 2 = 0 then Even value else Odd value
    let (|Alpha|Beta|Gamma|) (value: int) = if value < 0 then Alpha value elif value = 0 then Beta value else Gamma value
    [<return: Struct>]
    let (|StructPartial|_|) value = if value > 0 then ValueSome value else ValueNone
    let (|Total|) value = value
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

module ``(=)`` =
    let (|Punctuation|_|) value = Some value
    [<RequireQualifiedAccess>]
    module RqaOps =
        let (+++) left right = left + right

"""

let private sameFileSource (container: string) (body: string) =
    let margin = if container = "" then "" else "    "
    let outer = if container = "" then "" else $"module {container} =\n"
    let consumer = if container = "Closed" then "module Outer =\n" else ""

    if container = "Cross" then
        $"namespace N.A\nmodule Patterns =\n    let (|Case|_|) value = Some value\nnamespace N.B\nmodule Use =\n    {body}\n"
    else
        $"namespace N\n{outer}{margin}module Patterns =\n{margin}    let (|Case|_|) value = Some value\n{margin}    type Thing = class end\n{consumer}{margin}module Use =\n{margin}    {body}\n"

let private source (body: string) =
    definitions.Replace("\r\n", "\n")
    + $"namespace Consumer\n\nmodule Use =\n\n    {body}\n"

let private withOpen atTop (ns: string) (code: string) =
    if atTop then
        code.Replace("namespace Consumer\n\n", $"namespace Consumer\n\nopen {ns}\n\n")
    else
        code.Replace("module Use =\n\n", $"module Use =\n\n    open {ns}\n\n")

let private assertCompiles code =
    let document = RoslynTestHelpers.GetFsDocument code

    let _, results =
        document.GetFSharpParseAndCheckResultsAsync("test")
        |> CancellableTask.runSynchronouslyWithoutCancellation

    Assert.Empty results.Diagnostics
    let text = SourceText.From code

    let unused =
        UnusedOpens.getUnusedOpens (results, fun line -> text.Lines[line - 1].ToString())
        |> Async.RunSynchronously

    Assert.Empty unused

[<Theory>]
[<InlineData("Even", "", "Candidates.Normal", true)>]
[<InlineData("Odd", "", "Candidates.Normal", false)>]
[<InlineData("Gamma", "", "Candidates.Normal", true)>]
[<InlineData("StructPartial", "", "Candidates.Normal", false)>]
[<InlineData("Total", "", "Candidates.Normal", true)>]
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
[<InlineData("Punctuation", "", "Candidates.``(=)``", true)>]
[<InlineData("``(=)``.Punctuation", "", "Candidates", false)>]
let ``Add Open applied case uses exact target and placement`` (caseName: string, parameters: string, ns: string, atTop: bool) =
    let fallback = if caseName = "Total" then "" else " | _ -> 0"

    let code =
        source $"let classify value = match value with | {caseName}{parameters} n -> n{fallback}"

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

[<Theory>]
[<InlineData("let classify value = match value with | Deep n -> n | _ -> 0", "Candidates", true, false, "")>]
[<InlineData("let classify value = match value with | Deep n -> n | _ -> 0", "Candidates", false, false, "")>]
[<InlineData("let value: DateTime = Unchecked.defaultof<_>", "System", true, false, "")>]
[<InlineData("let value: DateTime = Unchecked.defaultof<_>", "System", false, false, "")>]
[<InlineData("let classify value = match value with | Case n -> n | _ -> 0", "Patterns", true, true, "")>]
[<InlineData("let classify value = match value with | Case n -> n | _ -> 0", "Patterns", false, true, "")>]
[<InlineData("let classify value = match value with | Case n -> n | _ -> 0", "Patterns", true, true, "Outer")>]
[<InlineData("let classify value = match value with | Case n -> n | _ -> 0", "Patterns", false, true, "Outer")>]
[<InlineData("let value: Thing = Unchecked.defaultof<_>", "Patterns", true, true, "Outer")>]
[<InlineData("let value: Thing = Unchecked.defaultof<_>", "Patterns", false, true, "Outer")>]
[<InlineData("let classify value = match value with | Case n -> n | _ -> 0", "Closed.Patterns", true, true, "Closed")>]
[<InlineData("let classify value = match value with | Case n -> n | _ -> 0", "Closed.Patterns", false, true, "Closed")>]
let ``Add Open supports adjacent headers without changing existing spacing``
    (body: string, ns: string, atTop: bool, sameFile: bool, container: string)
    =
    let code =
        if sameFile then
            sameFileSource container body
        else
            (source body).Replace("namespace Consumer\n\nmodule Use =\n\n", "namespace Consumer\nmodule Use =\n")

    let provider = AddOpenCodeFixProvider(AssemblyContentProvider())

    let mode =
        WithSettings
            { CodeFixesOptions.Default with
                AlwaysPlaceOpensAtTopLevel = atTop
            }

    let fix = provider |> tryFix code mode |> Option.get
    Assert.Equal($"open {ns}", fix.Message)

    let expected =
        if container = "Closed" then
            code.Replace("module Outer =\n", $"open {ns}\n\nmodule Outer =\n")
        elif sameFile then
            let margin = if container = "" then "" else "    "
            let anchor = $"{margin}module Use =\n"
            code.Replace(anchor, $"{margin}open {ns}\n\n{anchor}")
        elif atTop then
            code.Replace("namespace Consumer\n", $"namespace Consumer\nopen {ns}\n\n")
        else
            code.Replace("module Use =\n", $"module Use =\n    open {ns}\n\n")

    Assert.Equal(expected, fix.FixedCode.Replace("\r\n", "\n"))
    assertCompiles fix.FixedCode
    Assert.Equal(None, provider |> tryFix fix.FixedCode mode)

[<Theory>]
[<InlineData(true)>]
[<InlineData(false)>]
let ``Add Open supports a top module header without a blank line`` (atTop: bool) =
    let code =
        "module Consumer\nlet value: DateTime = Unchecked.defaultof<_>\nlet other = 1\n"

    let mode =
        WithSettings
            { CodeFixesOptions.Default with
                AlwaysPlaceOpensAtTopLevel = atTop
            }

    let fix =
        AddOpenCodeFixProvider(AssemblyContentProvider())
        |> tryFix code mode
        |> Option.get

    Assert.Equal("open System", fix.Message)

    Assert.Equal(
        "module Consumer\nopen System\n\nlet value: DateTime = Unchecked.defaultof<_>\nlet other = 1\n",
        fix.FixedCode.Replace("\r\n", "\n")
    )

    assertCompiles fix.FixedCode

[<Theory>]
[<InlineData("let classify value = match value with | Restricted n -> n | _ -> 0", "Candidates.Qualified.Restricted")>]
[<InlineData("let value = RqaOps.(+++) 1 2", "open Candidates.``(=)``")>]
let ``RQA bare case fix supplies the qualification still required`` (body: string, message: string) =
    let code = source body

    let fix =
        AddOpenCodeFixProvider(AssemblyContentProvider())
        |> tryFix code Auto
        |> Option.get

    Assert.Equal(message, fix.Message)

    let expected =
        if message = "Candidates.Qualified.Restricted" then
            code.Replace("| Restricted n", "| Candidates.Qualified.Restricted n")
        else
            withOpen true "Candidates.``(=)``" code

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
[<InlineData("Positive", "Candidates.Normal", true, false, false, false, "")>]
[<InlineData("Positive", "Candidates.Other", false, false, false, false, "")>]
[<InlineData("Deep", "Candidates", false, false, false, false, "")>]
[<InlineData("Qualified.Restricted", "Candidates", true, false, false, false, "")>]
[<InlineData("``Has space``", "Candidates.``Space module``", false, false, false, false, "")>]
[<InlineData("Positive", "Candidates.Normal", false, true, false, false, "")>]
[<InlineData("Positive", "Candidates.Normal", false, false, true, false, "")>]
[<InlineData("Case", "Patterns", true, false, false, true, "")>]
[<InlineData("Case", "Patterns", false, false, false, true, "")>]
[<InlineData("Case", "Patterns", true, false, false, true, "Outer")>]
[<InlineData("Case", "Patterns", false, false, false, true, "Outer")>]
[<InlineData("Case", "Closed.Patterns", true, false, false, true, "Closed")>]
[<InlineData("Case", "Closed.Patterns", false, false, false, true, "Closed")>]
[<InlineData("Case", "N.A.Patterns", true, false, false, true, "Cross")>]
let ``completion commit applies case text and avoids duplicate opens``
    (nameInCode: string, ns: string, atTop: bool, alreadyOpen: bool, closedInner: bool, sameFile: bool, container: string)
    =
    let placeholder = "Missing"

    let initial =
        let code =
            source $"let classify value = match value with | {placeholder} n -> n | _ -> 0"

        if sameFile then
            sameFileSource container $"let classify value = match value with | {placeholder} n -> n | _ -> 0"
        elif closedInner then
            code.Replace("module Use =\n\n", "module Use =\n    module Inner = let value = 1\n")
        else
            code

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
            let property name =
                match item.Properties.TryGetValue name with
                | true, value -> Some value
                | _ -> None

            defaultArg (property "NameInCode") item.DisplayText = nameInCode
            && property "NamespaceToOpen" = (if alreadyOpen then None else Some ns))
        |> Assert.Single

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
        elif container = "Cross" then
            committed.Replace("namespace N.B\n", $"namespace N.B\n\nopen {ns}\n\n")
        elif container = "Closed" then
            committed.Replace("module Outer =\n", $"\nopen {ns}\n\nmodule Outer =\n")
        elif sameFile then
            let margin = if container = "" then "" else "    "
            let anchor = $"{margin}module Use =\n"
            let before = if atTop || container = "" then "\n" else ""
            committed.Replace(anchor, $"{before}{margin}open {ns}\n\n{anchor}")
        elif atTop then
            withOpen true ns committed
        elif closedInner then
            committed.Replace("module Use =\n", $"module Use =\n    open {ns}\n\n")
        else
            committed.Replace("module Use =\n\n", $"module Use =\n    open {ns}\n\n")

    Assert.Equal(expected, actual.Replace("\r\n", "\n"))
    assertCompiles actual

[<Theory>]
[<InlineData("DateTime", "System", false)>]
[<InlineData("StringBuilder", "System.Text", true)>]
let ``ordinary imports stay in the consuming namespace`` (name: string, ns: string, blankLineBeforeBody: bool) =
    let code =
        let code = source $"let value: {name} = Unchecked.defaultof<_>"

        if blankLineBeforeBody then
            code
        else
            code.Replace("module Use =\n\n", "module Use =\n")

    let document = RoslynTestHelpers.GetFsDocument code
    let text = SourceText.From code

    let parse =
        document.GetFSharpParseResultsAsync("ordinary namespace import")
        |> CancellableTask.runSynchronouslyWithoutCancellation

    let line =
        text.Lines.GetLineFromPosition(code.IndexOf("let value", StringComparison.Ordinal))

    let context =
        ParsedInput.FindNearestPointToInsertOpenDeclaration
            (global.FSharp.Compiler.Text.Line.fromZ line.LineNumber)
            parse.ParseTree
            ($"{ns}.{name}".Split '.')
            OpenStatementInsertionPoint.TopLevel

    let actual, _ = OpenDeclarationHelper.insertOpenDeclaration text context ns
    Assert.Equal(withOpen true ns code, actual.ToString())
    assertCompiles (actual.ToString())

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

[<Theory>]
[<InlineData(true, false, true)>]
[<InlineData(false, false, false)>]
[<InlineData(false, true, true)>]
let ``real completion service respects the editor unopened-symbol option`` (includeUnopened: bool, visible: bool, hasCase: bool) =
    let pattern = if visible then "Positive" else "Missing"

    let code =
        source $"let (|Visible|_|) value = Some value\n    let classify value = match value with | {pattern} n -> n | _ -> 0"

    let code =
        if visible then
            withOpen true "Candidates.Normal" code
        else
            code

    let document =
        RoslynTestHelpers.GetFsDocument(
            code,
            customEditorOptions =
                { IntelliSenseOptions.Default with
                    IncludeSymbolsFromUnopenedNamespacesOrModules = includeUnopened
                }
        )

    let workspace = document.Project.Solution.Workspace
    workspace.OpenDocument(document.Id)
    let document = workspace.CurrentSolution.GetDocument(document.Id)
    let settings = workspace.Services.GetService<EditorOptions>()
    Assert.Equal(includeUnopened, settings.IntelliSense.IncludeSymbolsFromUnopenedNamespacesOrModules)

    let service =
        FSharpCompletionService(workspace, serviceProvider, AssemblyContentProvider(), settings)

    let position =
        code.IndexOf($"| {pattern} n", StringComparison.Ordinal) + $"| {pattern}".Length

    let completions =
        service.GetCompletionsAsync(document, position).GetAwaiter().GetResult()

    Assert.NotNull completions
    Assert.Contains(completions.ItemsList, fun item -> item.DisplayText = "Visible")
    Assert.Equal(hasCase, completions.ItemsList |> Seq.exists (fun item -> item.DisplayText = "Positive"))

    if visible then
        let item =
            completions.ItemsList
            |> Seq.filter (fun item -> item.DisplayText = "Positive")
            |> Assert.Single

        Assert.False(item.Properties.ContainsKey "NamespaceToOpen")
        assertCompiles code

[<Fact>]
let ``referenced case Add Open uses the nearest physical scope without self-qualification`` () =
    let reference =
        FSharp "namespace N\nmodule Outer =\n    module Patterns =\n        let (|Case|_|) value = Some value\n"
        |> withName "ActivePatternNearestReference"
        |> compile
        |> shouldSucceed

    let code =
        "namespace N\nmodule Outer =\n    module Use =\n        let classify value = match value with | Case n -> n | _ -> 0\n"

    let document =
        RoslynTestHelpers.GetFsDocument(
            code,
            customEditorOptions =
                { CodeFixesOptions.Default with
                    AlwaysPlaceOpensAtTopLevel = false
                }
        )

    let solution =
        document.Project.AddMetadataReference(MetadataReference.CreateFromFile reference.OutputPath.Value).Solution

    Assert.True(solution.Workspace.TryApplyChanges solution)
    let document = solution.Workspace.CurrentSolution.GetDocument(document.Id)
    let text = SourceText.From code

    let diagnostics =
        FSharpDiagnostics.generate Auto document
        |> CancellableTask.runSynchronouslyWithoutCancellation

    Assert.Contains(diagnostics, fun diagnostic -> diagnostic.ErrorNumber = 39)
    let provider = AddOpenCodeFixProvider(AssemblyContentProvider())

    let context =
        diagnostics
        |> Seq.map (Diagnostic.ofFSharpDiagnostic text document.FilePath)
        |> ImmutableArray.CreateRange
        |> CodeFixContext.tryCreate provider.CanFix document
        |> function
            | ValueSome context -> context
            | ValueNone -> failwith "Expected automatic referenced-case diagnostic"

    let fix =
        (provider :> IFSharpCodeFixProvider).GetCodeFixIfAppliesAsync context
        |> CancellableTask.runSynchronouslyWithoutCancellation
        |> function
            | ValueSome fix -> fix
            | ValueNone -> failwith "Expected referenced-case Add Open"

    Assert.Equal("open Outer.Patterns", fix.Message)
    let actual = text.WithChanges(fix.Changes).ToString()
    Assert.Equal(code.Replace("        let classify", "        open Outer.Patterns\n\n        let classify"), actual.Replace("\r\n", "\n"))

    let _, results =
        document.WithText(SourceText.From actual).GetFSharpParseAndCheckResultsAsync("referenced AP edit")
        |> CancellableTask.runSynchronouslyWithoutCancellation

    Assert.Empty results.Diagnostics
    let editedText = SourceText.From actual

    Assert.Empty(
        UnusedOpens.getUnusedOpens (results, fun line -> editedText.Lines[line - 1].ToString())
        |> Async.RunSynchronously
    )

    Assert.Contains(results.ProjectContext.GetReferencedAssemblies(), fun assembly -> assembly.FileName = reference.OutputPath)

    Assert.Contains(
        results.GetAllUsesOfAllSymbolsInFile(),
        fun usage ->
            usage.Symbol :? FSharpActivePatternCase
            && usage.Symbol.FullName = "N.Outer.Patterns.(|Case|_|).Case"
            && not usage.IsFromDefinition
    )
