module FSharp.Compiler.Service.Tests.ActivePatternCompletionTests

open System
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Symbols
open FSharp.Compiler.Syntax.PrettyNaming
open FSharp.Compiler.Text
open Xunit

let private patterns = """
namespace Candidates
open System

module Normal =
    let ordinaryValue = 1
    let (++) left right = 10 * left + right
    type Thing = class end
    let (|Even|Odd|) value = if value % 2 = 0 then Even else Odd
    let (|Total|) value = value
    let (|Positive|_|) value = if value > 0 then Some value else None
    let (|Above|_|) threshold value = if value > threshold then Some value else None
    let private (|Private|_|) value = Some value
    let internal (|Internal|_|) value = Some value
    [<Obsolete("Use Positive")>]
    let (|Old|_|) value = Some value
    [<CompilerMessage("Hidden", 1234, IsHidden = true)>]
    let (|Hidden|_|) value = Some value

module private PrivateModule =
    let (|Secret|_|) value = Some value

module Other =
    let (|Positive|_|) value = Some value

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

namespace Microsoft.FSharp.Core
module Unopened =
    let coreValue = 1
    let (|CoreCase|_|) value = Some value
"""

let private check markedSource =
    let context = Checker.getCompletionContext markedSource
    let options = createProjectOptionsFromNamedSources [ "Patterns.fs", patterns; "Consumer.fs", context.Source ] []
    let _, producer = parseAndCheckFile options.SourceFiles[0] patterns options
    Assert.Empty producer.Diagnostics
    let parse, results = parseAndCheckFile options.SourceFiles[1] context.Source options
    let catalogue = AssemblyContent.GetAssemblySignatureContent AssemblyContentType.Full results.PartialAssemblySignature
    let complete completionOptions getAllEntities =
        results.GetDeclarationListInfo(
            Some parse, context.Pos.Line, context.LineText, context.PartialIdentifier,
            getAllEntities = getAllEntities, options = completionOptions)
    let item predicate =
        (complete FSharpCodeCompletionOptions.Default (fun () -> catalogue)).Items
        |> Array.filter predicate
        |> Assert.Single
    let insert (symbol: AssemblySymbol) partiallyQualified =
        ParsedInput.TryFindInsertionContext context.Pos.Line parse.ParseTree partiallyQualified OpenStatementInsertionPoint.TopLevel
            (symbol.TopRequireQualifiedAccessParent, symbol.AutoOpenParent, symbol.Namespace, symbol.CleanedIdents)
        |> Assert.Single
    let checkEdit edited =
        let _, checkedResults = parseAndCheckFile options.SourceFiles[1] edited options
        assertNoDiagnostics checkedResults
        checkedResults
    struct {| Context = context; Parse = parse; Results = results; Catalogue = catalogue; Complete = complete; Item = item; Insert = insert; CheckEdit = checkEdit |}

[<Theory>]
[<InlineData("Even", "Normal", "", "Even", "Candidates.Normal", "Candidates.Normal.Even")>]
[<InlineData("Positive", "Normal", " n", "Positive", "Candidates.Normal", "Candidates.Normal.Positive")>]
[<InlineData("Above", "Normal", " 0 n", "Above", "Candidates.Normal", "Candidates.Normal.Above")>]
[<InlineData("AutoCase", "Auto", " n", "AutoCase", "Candidates", "Candidates.Auto.AutoCase")>]
[<InlineData("Deep", "Auto.Nested", " n", "Deep", "Candidates", "Candidates.Auto.Nested.Deep")>]
[<InlineData("OrdinaryCase", "Auto.Ordinary", " n", "OrdinaryCase", "Candidates.Auto.Ordinary", "Candidates.Auto.Ordinary.OrdinaryCase")>]
[<InlineData("Restricted", "Qualified", " n", "Qualified.Restricted", "Candidates", "Candidates.Qualified.Restricted")>]
[<InlineData("SuffixCase", "Suffix", " n", "SuffixCase", "Candidates.Suffix", "Candidates.Suffix.SuffixCase")>]
[<InlineData("Has space", "Space module", " n", "``Has space``", "Candidates.``Space module``", "Candidates.``Space module``.``Has space``")>]
let ``unopened cases provide compiling completion and import edits`` (caseName: string, owner: string, arguments: string, nameInCode: string, namespaceToOpen: string, qualifiedName: string) =
    let sourceName = NormalizeIdentifierBackticks caseName
    let marked = $"module Consumer\nlet classify value = match value with | {sourceName}{{caret}}{arguments} -> 1 | _ -> 0"
    let test = check marked
    if arguments <> "" then
        Assert.Contains(test.Results.Diagnostics, fun diagnostic -> diagnostic.ErrorNumber = 39)
    let symbol =
        test.Catalogue
        |> List.find (fun symbol -> String.concat "." symbol.CleanedIdents = $"Candidates.{owner}.{caseName}")
    let entity, insertionContext =
        test.Insert symbol [| { Ident = caseName; Resolved = false } |]
    Assert.Equal(Some namespaceToOpen, entity.Namespace)
    Assert.Equal(qualifiedName, entity.FullRelativeName)
    Assert.Equal(qualifiedName, entity.Qualifier)
    let importedName = if entity.FullDisplayName = "" then sourceName else entity.FullDisplayName
    Assert.Equal(nameInCode, importedName)

    let item = test.Item (fun item -> item.FullName = symbol.FullName)
    Assert.Equal(nameInCode, item.NameInCode)
    Assert.Equal(Some namespaceToOpen, item.NamespaceToOpen)

    let checkEdit insertedName openNamespace =
        let source = test.Context.Source.Replace(sourceName, insertedName)
        let lines = SourceContext.getLines source
        let edited =
            match openNamespace with
            | Some ns ->
                let pos = ParsedInput.AdjustInsertionPoint (fun line -> lines[line].Trim()) insertionContext
                Assert.Equal(Position.mkPos 2 0, pos)
                lines |> Array.insertAt (pos.Line - 1) $"open {ns}" |> String.concat "\n"
            | None -> source
        let results = test.CheckEdit edited
        if openNamespace.IsSome then
            let unused = UnusedOpens.getUnusedOpens(results, fun line -> (SourceContext.getLines edited)[line - 1]) |> Async.RunSynchronouslyImmediate
            Assert.Empty unused
    checkEdit item.NameInCode item.NamespaceToOpen
    checkEdit entity.Qualifier None

[<Theory>]
[<InlineData("let classify value = match value with | Total{caret} n -> n")>]
[<InlineData("let classify (Total{caret} n) = n")>]
[<InlineData("let classify = fun (Total{caret} n) -> n")>]
let ``unopened cases complete match binding and lambda patterns`` (body: string) =
    let test = check $"module Consumer\n{body}"
    let symbol = test.Catalogue |> List.find (fun symbol -> symbol.CleanedIdents = [| "Candidates"; "Normal"; "Total" |])
    let item = test.Item (fun item -> item.FullName = symbol.FullName)
    Assert.Equal("Total", item.NameInCode)
    Assert.Equal(Some "Candidates.Normal", item.NamespaceToOpen)
    let edited = $"""module Consumer
open {item.NamespaceToOpen.Value}
{body.Replace("{caret}", "")}"""
    test.CheckEdit edited |> ignore

[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``case completion uses backing value visibility and obsolete settings`` suggestObsolete =
    let test = check "module Consumer\nlet classify value = match value with | P{caret} n -> 1 | _ -> 0"
    let options = { FSharpCodeCompletionOptions.Default with SuggestObsoleteSymbols = suggestObsolete }
    let items = (test.Complete options (fun () -> test.Catalogue)).Items
    let contains name = items |> Array.exists (fun item -> item.FullName.EndsWith($".{name}", StringComparison.Ordinal))
    Assert.False(contains "Private")
    Assert.False(contains "Secret")
    if not suggestObsolete then Assert.False(contains "Hidden")
    Assert.Equal(suggestObsolete, contains "Old")
    Assert.True(contains "Internal")
    let internalCase = items |> Array.find (fun item -> item.FullName.EndsWith(".Internal", StringComparison.Ordinal))
    Assert.True internalCase.Accessibility.IsInternal

[<Fact>]
let ``case catalogue does not leak into expressions`` () =
    let test = check "module Consumer\nlet value = P{caret}"
    let info = test.Complete FSharpCodeCompletionOptions.Default (fun () -> test.Catalogue)
    let identities =
        test.Catalogue
        |> List.filter (fun symbol -> symbol.Symbol :? FSharpActivePatternCase)
        |> List.map _.FullName
        |> Set.ofList
    Assert.DoesNotContain(info.Items, fun item -> identities.Contains item.FullName)
    Assert.Contains(info.Items, fun item -> item.NameInCode = "Normal" && item.NamespaceToOpen = Some "Candidates")
    let symbols =
        test.Results.GetDeclarationListSymbols(
            Some test.Parse, test.Context.Pos.Line, test.Context.LineText, test.Context.PartialIdentifier,
            getAllEntities = (fun () -> test.Catalogue))
        |> List.collect id
    Assert.DoesNotContain(symbols, fun symbol -> identities.Contains symbol.Symbol.FullName)

[<Theory>]
[<InlineData("let classify value = match value with | Positive{caret} n -> n | _ -> 0", true)>]
[<InlineData("let classify value = match value with | Normal.Positive{caret} n -> n | _ -> 0", true)>]
[<InlineData("let classify value = match value with | Normal{caret}.Positive n -> n | _ -> 0", true)>]
[<InlineData("let classify value = match value with | Qualified{caret}.Restricted n -> n | _ -> 0", true)>]
[<InlineData("let value = Positive{caret} 1", false)>]
let ``editor scoped case query reuses FCS context and backing value policies`` (body: string, isPattern: bool) =
    let test = check $"module Consumer\n{body}"
    let cases = test.Catalogue |> List.filter (fun symbol -> symbol.Symbol :? FSharpActivePatternCase)
    let partialName = { test.Context.PartialIdentifier with QualifyingIdents = []; PartialIdent = ""; LastDotPos = None }
    let identities =
        test.Results.GetDeclarationListSymbols(
            Some test.Parse, test.Context.Pos.Line, test.Context.LineText, partialName, getAllEntities = (fun () -> cases))
        |> List.collect id
        |> List.map (fun symbol -> symbol.Symbol.FullName)
        |> Set.ofList
    for case in cases do
        let name = Array.last case.CleanedIdents
        if name = "Private" || name = "Secret" || name = "Hidden" || name = "Old" then
            Assert.False(identities.Contains case.FullName)
        elif name = "Positive" then
            Assert.Equal(isPattern, identities.Contains case.FullName)

[<Theory>]
[<InlineData("Restricted", false)>]
[<InlineData("Qualified.Restricted", true)>]
let ``editor RQA action keeps the qualification needed by the applied pattern`` (pattern: string, canOpen: bool) =
    let test = check $"module Consumer\nlet classify value = match value with | {pattern}{{caret}} n -> n | _ -> 0"
    let symbol = test.Catalogue |> List.find (fun symbol -> symbol.CleanedIdents = [| "Candidates"; "Qualified"; "Restricted" |])
    let unresolved =
        pattern.Split '.'
        |> Array.mapi (fun index ident -> { Ident = ident; Resolved = index <> 0 })
    let entity, _ = test.Insert symbol unresolved
    Assert.Equal(canOpen, entity.FullDisplayName = "" || entity.FullDisplayName = pattern)
    let edited =
        if canOpen then
            Assert.Equal(Some "Candidates", entity.Namespace)
            test.Context.Source.Replace("module Consumer", "module Consumer\nopen Candidates")
        else
            Assert.Equal("Candidates.Qualified.Restricted", entity.Qualifier)
            test.Context.Source.Replace("| Restricted n", $"| {entity.Qualifier} n")
    test.CheckEdit edited |> ignore

[<Fact>]
let ``case completion preserves per call catalogues and distinct candidates`` () =
    let test = check "module Consumer\nlet classify value = match value with | Positive{caret} n -> 1 | _ -> 0"
    let complete catalogue = (test.Complete FSharpCodeCompletionOptions.Default (fun () -> catalogue)).Items
    let cases = test.Catalogue |> List.filter (fun symbol -> symbol.Symbol :? FSharpActivePatternCase)
    let candidates = complete cases |> Array.filter (fun item -> item.NameInCode = "Positive")
    Assert.Equal<string option array>([| Some "Candidates.Normal"; Some "Candidates.Other" |], candidates |> Array.map _.NamespaceToOpen |> Array.sort)
    Assert.DoesNotContain(complete [], fun item -> candidates |> Array.exists (fun candidate -> candidate.FullName = item.FullName))
    for candidate in candidates do
        let catalogue = cases |> List.filter (fun symbol -> symbol.FullName = candidate.FullName)
        let actual = complete catalogue |> Array.filter (fun item -> item.NameInCode = "Positive") |> Assert.Single
        Assert.Equal(candidate.NamespaceToOpen, actual.NamespaceToOpen)

[<Theory>]
[<InlineData("Candidates.Normal", "Positive")>]
[<InlineData("Candidates", "AutoCase")>]
[<InlineData("Candidates", "Deep")>]
let ``already visible cases are not duplicated by the catalogue`` (openNamespace: string, caseName: string) =
    let test = check $"module Consumer\nopen {openNamespace}\nlet classify value = match value with | {caseName}{{caret}} n -> 1 | _ -> 0"
    let candidates =
        (test.Complete FSharpCodeCompletionOptions.Default (fun () -> test.Catalogue)).Items
        |> Array.filter (fun item -> item.NameInCode = caseName)
    let expected = if caseName = "Positive" then [| None; Some "Candidates.Other" |] else [| None |]
    Assert.Equal<string option array>(expected, candidates |> Array.map _.NamespaceToOpen |> Array.sort)

[<Fact>]
let ``private cases remain available inside their defining module`` () =
    let test = check """
module Consumer
module Local =
    let private (|LocalPrivate|_|) value = Some value
    let classify value = match value with | LocalPrivate{caret} n -> n | _ -> 0
"""
    let symbol = test.Catalogue |> List.find (fun symbol -> symbol.CleanedIdents = [| "Consumer"; "Local"; "LocalPrivate" |])
    let item = test.Item (fun item -> item.FullName = symbol.FullName)
    Assert.Equal("LocalPrivate", item.NameInCode)
    Assert.Equal(None, item.NamespaceToOpen)

[<Fact>]
let ``partially qualified case uses existing import and qualification paths`` () =
    let test = check "module Consumer\nlet classify value = match value with | Normal.Positive{caret} n -> 1 | _ -> 0"
    let symbol = test.Catalogue |> List.find (fun symbol -> symbol.CleanedIdents = [| "Candidates"; "Normal"; "Positive" |])
    let entity, _ =
        test.Insert symbol [| { Ident = "Normal"; Resolved = false }; { Ident = "Positive"; Resolved = true } |]
    Assert.Equal(Some "Candidates", entity.Namespace)
    Assert.Equal("Candidates.Normal", entity.Qualifier)
    let edited = test.Context.Source.Replace("Normal.Positive", $"{entity.Qualifier}.Positive")
    test.CheckEdit edited |> ignore
    let opened = test.Context.Source.Replace("module Consumer", "module Consumer\nopen Candidates")
    test.CheckEdit opened |> ignore
    let visible = check (opened.Replace("Normal.Positive", "Normal.Positive{caret}"))
    let item = visible.Item (fun item -> item.NameInCode = "Positive")
    Assert.Equal(None, item.NamespaceToOpen)

[<Fact>]
let ``unknown bare uppercase binding remains valid`` () =
    let test = check "module Consumer\nlet Even{caret} = 1"
    Assert.Empty test.Results.Diagnostics
    test.Complete FSharpCodeCompletionOptions.Default (fun () -> test.Catalogue) |> ignore
    Assert.Empty test.Results.Diagnostics

[<Fact>]
let ``FSharp namespace does not make an unopened case module visible`` () =
    let test = check "module Consumer\nlet classify value = match value with | CoreCase{caret} n -> n | _ -> 0"
    let item = test.Item (fun item -> item.NameInCode = "CoreCase")
    Assert.Equal(Some "Microsoft.FSharp.Core.Unopened", item.NamespaceToOpen)
    let edited = test.Context.Source.Replace("module Consumer", $"module Consumer\nopen {item.NamespaceToOpen.Value}")
    test.CheckEdit edited |> ignore

[<Fact>]
let ``FSharp namespace keeps ordinary value completion without an extra open`` () =
    let test = check "module Consumer\nopen Microsoft.FSharp.Core\nlet value = coreValue{caret}"
    let symbol = test.Catalogue |> List.find (fun symbol -> symbol.CleanedIdents = [| "Microsoft"; "FSharp"; "Core"; "Unopened"; "coreValue" |])
    let item = test.Item (fun item -> item.FullName = symbol.FullName)
    Assert.Equal("Unopened.coreValue", item.NameInCode)
    Assert.Equal(None, item.NamespaceToOpen)
    let edited = test.Context.Source.Replace("coreValue", item.NameInCode)
    test.CheckEdit edited |> ignore

[<Theory>]
[<InlineData("ordinaryValue", "let value = ordinaryValue{caret}")>]
[<InlineData("Thing", "let value: Thing{caret} option = None")>]
[<InlineData("(++)", "let value = (++){caret} 4 2")>]
let ``existing value type and operator import paths still compile`` (name: string, body: string) =
    let test = check $"module Consumer\n{body}"
    let symbol = test.Catalogue |> List.find (fun symbol -> symbol.CleanedIdents = [| "Candidates"; "Normal"; name |])
    let entity, _ =
        test.Insert symbol [| { Ident = name; Resolved = false } |]
    Assert.Equal(Some "Candidates.Normal", entity.Namespace)
    Assert.Equal($"Candidates.Normal.{name}", entity.Qualifier)
    for edited in
        [ test.Context.Source.Replace(name, entity.Qualifier)
          test.Context.Source.Replace("module Consumer", "module Consumer\nopen Candidates.Normal") ] do
        test.CheckEdit edited |> ignore
    if name <> "(++)" then
        let item = test.Item (fun item -> item.FullName = symbol.FullName)
        Assert.Equal($"Normal.{name}", item.NameInCode)
        Assert.Equal(Some "Candidates", item.NamespaceToOpen)
