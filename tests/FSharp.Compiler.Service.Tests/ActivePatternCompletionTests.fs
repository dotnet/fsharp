module FSharp.Compiler.Service.Tests.ActivePatternCompletionTests

open System
open System.IO
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
    let (|Alpha|Beta|Gamma|) (value: int) = if value < 0 then Alpha value elif value = 0 then Beta value else Gamma value
    [<return: Struct>]
    let (|StructPartial|_|) value = if value > 0 then ValueSome value else ValueNone
    let (|BooleanPartial|_|) value = value > 0
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

[<RequireQualifiedAccess>]
module RqaOps =
    let (+++) a b = a + b

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Suffix =
    let (|SuffixCase|_|) value = Some value

module ``Space module`` =
    let (|``Has space``|_|) value = Some value

module ``(=)`` =
    let (|Punctuation|_|) value = Some value

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
    let insertAt mode (symbol: AssemblySymbol) partiallyQualified =
        ParsedInput.TryFindInsertionContext context.Pos.Line parse.ParseTree partiallyQualified mode
            (symbol.TopRequireQualifiedAccessParent, symbol.AutoOpenParent, symbol.Namespace, symbol.CleanedIdents)
        |> Assert.Single
    let insert = insertAt OpenStatementInsertionPoint.TopLevel
    let checkEdit edited =
        let _, checkedResults = parseAndCheckFile options.SourceFiles[1] edited options
        assertNoDiagnostics checkedResults
        checkedResults
    struct {| Context = context; Parse = parse; Results = results; Catalogue = catalogue; Complete = complete; Item = item; Insert = insert; InsertAt = insertAt; CheckEdit = checkEdit |}

let private assertNoUnusedOpens (results: FSharpCheckFileResults) (source: string) =
    Assert.Empty(UnusedOpens.getUnusedOpens(results, fun line -> (SourceContext.getLines source)[line - 1]) |> Async.RunSynchronouslyImmediate)

let private assertCaseUsed (results: FSharpCheckFileResults) (symbol: AssemblySymbol) =
    Assert.Contains(results.GetAllUsesOfAllSymbolsInFile(), fun usage ->
        usage.Symbol :? FSharpActivePatternCase && usage.Symbol.FullName = symbol.FullName && not usage.IsFromDefinition)

[<Theory>]
[<InlineData("Even", "Normal", "", "Even", "Candidates.Normal", "Candidates.Normal.Even")>]
[<InlineData("Odd", "Normal", "", "Odd", "Candidates.Normal", "Candidates.Normal.Odd")>]
[<InlineData("Gamma", "Normal", " n", "Gamma", "Candidates.Normal", "Candidates.Normal.Gamma")>]
[<InlineData("StructPartial", "Normal", " n", "StructPartial", "Candidates.Normal", "Candidates.Normal.StructPartial")>]
[<InlineData("BooleanPartial", "Normal", "", "BooleanPartial", "Candidates.Normal", "Candidates.Normal.BooleanPartial")>]
[<InlineData("Positive", "Normal", " n", "Positive", "Candidates.Normal", "Candidates.Normal.Positive")>]
[<InlineData("Above", "Normal", " 0 n", "Above", "Candidates.Normal", "Candidates.Normal.Above")>]
[<InlineData("AutoCase", "Auto", " n", "AutoCase", "Candidates", "Candidates.Auto.AutoCase")>]
[<InlineData("Deep", "Auto.Nested", " n", "Deep", "Candidates", "Candidates.Auto.Nested.Deep")>]
[<InlineData("OrdinaryCase", "Auto.Ordinary", " n", "OrdinaryCase", "Candidates.Auto.Ordinary", "Candidates.Auto.Ordinary.OrdinaryCase")>]
[<InlineData("Restricted", "Qualified", " n", "Qualified.Restricted", "Candidates", "Candidates.Qualified.Restricted")>]
[<InlineData("SuffixCase", "Suffix", " n", "SuffixCase", "Candidates.Suffix", "Candidates.Suffix.SuffixCase")>]
[<InlineData("Has space", "Space module", " n", "``Has space``", "Candidates.``Space module``", "Candidates.``Space module``.``Has space``")>]
[<InlineData("Punctuation", "(=)", " n", "Punctuation", "Candidates.``(=)``", "Candidates.``(=)``.Punctuation")>]
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
            assertNoUnusedOpens results edited
    checkEdit item.NameInCode item.NamespaceToOpen
    checkEdit entity.Qualifier None

[<Theory>]
[<InlineData("let classify value = match value with | Total{caret} n -> n")>]
[<InlineData("let classify value = match value with | 0 -> 0 | Total{caret} n -> n")>]
[<InlineData("let classify (Total{caret} n) = n")>]
[<InlineData("let classify (Total{caret} n, value) = n, value")>]
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

[<Theory>]
[<InlineData("let value = P{caret}", true)>]
[<InlineData("let value: P{caret} = Unchecked.defaultof<_>", true)>]
[<InlineData("let value = { P{caret} = 1 }", false)>]
[<InlineData("[<Positive{caret}>]\ntype T = class end", true)>]
let ``case catalogue does not leak into expressions`` (body: string, hasModule: bool) =
    let test = check $"module Consumer\n{body}"
    let info = test.Complete FSharpCodeCompletionOptions.Default (fun () -> test.Catalogue)
    let identities =
        test.Catalogue
        |> List.filter (fun symbol -> symbol.Symbol :? FSharpActivePatternCase)
        |> List.map _.FullName
        |> Set.ofList
    Assert.DoesNotContain(info.Items, fun item -> identities.Contains item.FullName)
    if hasModule then
        Assert.Contains(info.Items, fun item -> item.NameInCode = "Normal" && item.NamespaceToOpen = Some "Candidates")
    else
        Assert.DoesNotContain(info.Items, fun item -> item.NameInCode = "Normal" && item.NamespaceToOpen = Some "Candidates")
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
    Assert.True item.Accessibility.IsPrivate

[<Theory>]
[<InlineData("Normal", "Positive", "Candidates.Normal")>]
[<InlineData("(=)", "Punctuation", "Candidates.``(=)``")>]
let ``partially qualified case uses existing import and qualification paths`` (owner: string, caseName: string, qualifier: string) =
    let pattern = $"{NormalizeIdentifierBackticks owner}.{caseName}"
    let test = check $"module Consumer\nlet classify value = match value with | {pattern}{{caret}} n -> 1 | _ -> 0"
    let symbol = test.Catalogue |> List.find (fun symbol -> symbol.CleanedIdents = [| "Candidates"; owner; caseName |])
    let entity, _ =
        test.Insert symbol [| { Ident = owner; Resolved = false }; { Ident = caseName; Resolved = true } |]
    Assert.Equal(Some "Candidates", entity.Namespace)
    Assert.Equal(qualifier, entity.Qualifier)
    let edited = test.Context.Source.Replace(pattern, $"{entity.Qualifier}.{caseName}")
    test.CheckEdit edited |> ignore
    let opened = test.Context.Source.Replace("module Consumer", "module Consumer\nopen Candidates")
    test.CheckEdit opened |> ignore
    let visible = check (opened.Replace(pattern, $"{pattern}{{caret}}"))
    let item = visible.Item (fun item -> item.NameInCode = caseName)
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
[<InlineData("ordinaryValue", "let value = ordinaryValue{caret}", "Normal")>]
[<InlineData("Thing", "let value: Thing{caret} option = None", "Normal")>]
[<InlineData("(++)", "let value = (++){caret} 4 2", "Normal")>]
[<InlineData("(+++)", "let value = (+++){caret} 4 2", "RqaOps")>]
let ``existing value type and operator import paths still compile`` (name: string, body: string, owner: string) =
    let test = check $"module Consumer\n{body}"
    let symbol = test.Catalogue |> List.find (fun symbol -> symbol.CleanedIdents = [| "Candidates"; owner; name |])
    let entity, _ =
        test.Insert symbol [| { Ident = name; Resolved = false } |]
    let ns = if owner = "RqaOps" then "Candidates" else "Candidates.Normal"
    Assert.Equal(Some ns, entity.Namespace)
    Assert.Equal($"Candidates.{owner}.{name}", entity.Qualifier)
    Assert.Equal($"Candidates.{owner}.{name}", entity.FullRelativeName)
    Assert.Equal((if owner = "RqaOps" then $"RqaOps.{name}" else ""), entity.FullDisplayName)
    let opened = test.Context.Source.Replace("module Consumer", $"module Consumer\nopen {ns}")
    let opened = if entity.FullDisplayName = "" then opened else opened.Replace(name, entity.FullDisplayName)
    for edited in
        [ test.Context.Source.Replace(name, entity.Qualifier)
          opened ] do
        test.CheckEdit edited |> ignore
    if not (IsOperatorDisplayName name) then
        let item = test.Item (fun item -> item.FullName = symbol.FullName)
        Assert.Equal($"Normal.{name}", item.NameInCode)
        Assert.Equal(Some "Candidates", item.NamespaceToOpen)

[<Theory>]
[<InlineData("(|Even|Odd|)")>]
[<InlineData("(|Positive|_|)")>]
let ``backing group expressions retain their catalogue and source invocation behavior`` (name: string) =
    let test = check $"module Consumer\nlet value = {name}{{caret}} 2"
    let symbol = test.Catalogue |> List.filter (fun symbol -> symbol.CleanedIdents = [| "Candidates"; "Normal"; name |]) |> Assert.Single
    let value = Assert.IsType<FSharpMemberOrFunctionOrValue> symbol.Symbol
    Assert.True value.IsActivePattern
    Assert.Contains(test.Results.Diagnostics, fun diagnostic -> diagnostic.ErrorNumber = 39)
    let items = (test.Complete FSharpCodeCompletionOptions.Default (fun () -> test.Catalogue)).Items
    Assert.DoesNotContain(items, fun item -> item.FullName = symbol.FullName)
    let opened = test.Context.Source.Replace("module Consumer", "module Consumer\nopen Candidates.Normal")
    let results = test.CheckEdit opened
    assertNoUnusedOpens results opened
    test.CheckEdit (test.Context.Source.Replace(name, $"Candidates.Normal.{name}")) |> ignore

[<Theory>]
[<InlineData("", "Patterns", 4, 0, true)>]
[<InlineData("", "Patterns", 4, 0, false)>]
[<InlineData("Outer", "Patterns", 5, 4, true)>]
[<InlineData("Outer", "Patterns", 5, 4, false)>]
[<InlineData("Closed", "Closed.Patterns", 5, 0, true)>]
[<InlineData("Closed", "Closed.Patterns", 5, 0, false)>]
let ``same-file case names agree with their after-definition insertion scope`` (container: string, ns: string, insertionLine: int, column: int, atTop: bool) =
    let definition =
        "module Patterns =\n    let (|Case|_|) value = Some value\n"
    let marked =
        if container = "" then
            $"namespace N\n{definition}module Use =\n    let classify value = match value with | Case{{caret}} n -> n | _ -> 0\n"
        else
            let nestedDefinition = definition.Replace("module Patterns", "    module Patterns").Replace("\n    let", "\n        let")
            let consumer = if container = "Closed" then "module Outer =\n" else ""
            $"namespace N\nmodule {container} =\n{nestedDefinition}{consumer}    module Use =\n        let classify value = match value with | Case{{caret}} n -> n | _ -> 0\n"
    let test = check marked
    test.CheckEdit(test.Context.Source.Replace("let classify value = match value with | Case n -> n | _ -> 0", "let value = 1")) |> ignore
    Assert.Contains(test.Results.Diagnostics, fun diagnostic -> diagnostic.ErrorNumber = 39)
    let idents =
        [| yield "N"
           if container <> "" then yield container
           yield "Patterns"
           yield "Case" |]
    let symbol = test.Catalogue |> List.find (fun symbol -> symbol.CleanedIdents = idents)
    let item = test.Item (fun item -> item.FullName = symbol.FullName)
    Assert.Equal("Case", item.NameInCode)
    Assert.Equal(Some ns, item.NamespaceToOpen)
    let mode = if atTop then OpenStatementInsertionPoint.TopLevel else OpenStatementInsertionPoint.Nearest
    let entity, context = test.InsertAt mode symbol [| { Ident = "Case"; Resolved = false } |]
    Assert.Equal(Some ns, entity.Namespace)
    Assert.Equal($"{ns}.Case", entity.Qualifier)
    Assert.Equal(Position.mkPos insertionLine column, context.Pos)
    Assert.Equal((if atTop then ScopeKind.Namespace else ScopeKind.NestedModule), context.ScopeKind)
    let completionContext =
        ParsedInput.FindNearestPointToInsertOpenDeclaration test.Context.Pos.Line test.Parse.ParseTree (item.FullName.Split '.') mode
    Assert.Equal(context, completionContext)
    let margin = String.replicate column " "
    let edited =
        SourceContext.getLines test.Context.Source
        |> Array.insertAt (insertionLine - 1) $"{margin}open {item.NamespaceToOpen.Value}"
        |> String.concat "\n"
    let results = test.CheckEdit edited
    assertNoUnusedOpens results edited
    assertCaseUsed results symbol
    test.CheckEdit(test.Context.Source.Replace("| Case n ->", $"| {entity.Qualifier} n ->")) |> ignore

[<Theory>]
[<InlineData(true)>]
[<InlineData(false)>]
let ``nested ordinary completion keeps its existing namespace projection`` atTop =
    let test = check "namespace N\nmodule Outer =\n    module Patterns =\n        type Thing = class end\n    module Use =\n        let value: Thing{caret} = Unchecked.defaultof<_>\n"
    test.CheckEdit(test.Context.Source.Replace("let value: Thing = Unchecked.defaultof<_>", "let value = 1")) |> ignore
    Assert.Contains(test.Results.Diagnostics, fun diagnostic -> diagnostic.ErrorNumber = 39)
    let symbol = test.Catalogue |> List.find (fun symbol -> symbol.CleanedIdents = [| "N"; "Outer"; "Patterns"; "Thing" |])
    let item = test.Item (fun item -> item.FullName = symbol.FullName)
    Assert.Equal("Outer.Patterns.Thing", item.NameInCode)
    Assert.Equal(None, item.NamespaceToOpen)
    let mode = if atTop then OpenStatementInsertionPoint.TopLevel else OpenStatementInsertionPoint.Nearest
    let entity, context = test.InsertAt mode symbol [| { Ident = "Thing"; Resolved = false } |]
    Assert.Equal(Some "Patterns", entity.Namespace)
    Assert.Equal("Patterns.Thing", entity.Qualifier)
    Assert.Equal(Position.mkPos 5 4, context.Pos)
    Assert.Equal([| "N" |], ParsedInput.GetFullNameOfSmallestModuleOrNamespaceAtPoint(test.Context.Pos, test.Parse.ParseTree))
    let opened = test.Context.Source.Replace("    module Use =\n", "    open Patterns\n    module Use =\n")
    let results = test.CheckEdit opened
    assertNoUnusedOpens results opened
    test.CheckEdit(test.Context.Source.Replace("let value: Thing =", $"let value: {entity.Qualifier} =")) |> ignore

[<Fact>]
let ``same-file cases in a different namespace keep the full import path`` () =
    let test = check "namespace N.A\nmodule Patterns =\n    let (|Case|_|) value = Some value\nnamespace N.B\nmodule Use =\n    let classify value = match value with | Case{caret} n -> n | _ -> 0\n"
    test.CheckEdit(test.Context.Source.Replace("let classify value = match value with | Case n -> n | _ -> 0", "let value = 1")) |> ignore
    Assert.Contains(test.Results.Diagnostics, fun diagnostic -> diagnostic.ErrorNumber = 39)
    let symbol = test.Catalogue |> List.find (fun symbol -> symbol.CleanedIdents = [| "N"; "A"; "Patterns"; "Case" |])
    let item = test.Item (fun item -> item.FullName = symbol.FullName)
    Assert.Equal("Case", item.NameInCode)
    Assert.Equal(Some "N.A.Patterns", item.NamespaceToOpen)
    let entity, _ = test.Insert symbol [| { Ident = "Case"; Resolved = false } |]
    Assert.Equal(Some "N.A.Patterns", entity.Namespace)
    Assert.Equal("N.A.Patterns.Case", entity.Qualifier)
    for edited in
        [ test.Context.Source.Replace("namespace N.B\n", $"namespace N.B\nopen {item.NamespaceToOpen.Value}\n")
          test.Context.Source.Replace("| Case n", $"| {entity.Qualifier} n") ] do
        let results = test.CheckEdit edited
        assertNoUnusedOpens results edited
        assertCaseUsed results symbol

[<Theory>]
[<InlineData("Case", true, "Outer.Patterns", "Outer.Patterns.Case")>]
[<InlineData("Case", false, "Outer.Patterns", "Outer.Patterns.Case")>]
[<InlineData("Patterns.Case", false, "Outer", "Outer.Patterns")>]
let ``referenced case names retain the namespace-level insertion scope`` (pattern: string, atTop: bool, ns: string, qualifier: string) =
    let producer = "namespace N\nmodule Outer =\n    module Patterns =\n        let (|Case|_|) value = Some value\n"
    let producerOptions = createProjectOptionsFromNamedSources [ "Patterns.fs", producer ] []
    let _, producerResults = parseAndCheckFile producerOptions.SourceFiles[0] producer producerOptions
    Assert.Empty producerResults.Diagnostics
    let output = Path.ChangeExtension(producerOptions.ProjectFileName, ".dll")
    let context = Checker.getCompletionContext $"namespace N\nmodule Outer =\n    module Use =\n        let classify value = match value with | {pattern}{{caret}} n -> n | _ -> 0\n"
    let options = createProjectOptionsFromNamedSources [ "Consumer.fs", context.Source ] [ $"-r:{output}" ]
    let options = { options with ReferencedProjects = [| FSharpReferencedProject.FSharpReference(output, producerOptions) |] }
    let checkSource source = parseAndCheckFile options.SourceFiles[0] source options
    let _, prefix = checkSource (context.Source.Replace($"let classify value = match value with | {pattern} n -> n | _ -> 0", "let value = 1"))
    Assert.Empty prefix.Diagnostics
    let parse, results = checkSource context.Source
    Assert.Contains(results.Diagnostics, fun diagnostic -> diagnostic.ErrorNumber = 39)
    let assemblies = results.ProjectContext.GetReferencedAssemblies() |> List.filter (fun assembly -> assembly.SimpleName = Path.GetFileNameWithoutExtension output)
    Assert.Single assemblies |> ignore
    let catalogue = AssemblyContent.GetAssemblyContent (EntityCache()).Locking AssemblyContentType.Public (Some producerOptions.SourceFiles[0]) assemblies
    let symbol = catalogue |> List.find (fun symbol -> symbol.CleanedIdents = [| "N"; "Outer"; "Patterns"; "Case" |])
    let partialName = { context.PartialIdentifier with QualifyingIdents = []; PartialIdent = ""; LastDotPos = None }
    let item =
        results.GetDeclarationListInfo(Some parse, context.Pos.Line, context.LineText, partialName, getAllEntities = (fun () -> catalogue)).Items
        |> Array.filter (fun item -> item.FullName = symbol.FullName)
        |> Assert.Single
    Assert.Equal("Case", item.NameInCode)
    Assert.Equal(Some "Outer.Patterns", item.NamespaceToOpen)
    let unresolved = pattern.Split '.' |> Array.mapi (fun index name -> { Ident = name; Resolved = index <> 0 })
    let mode = if atTop then OpenStatementInsertionPoint.TopLevel else OpenStatementInsertionPoint.Nearest
    let entity, insertion =
        ParsedInput.TryFindInsertionContext context.Pos.Line parse.ParseTree unresolved mode
            (symbol.TopRequireQualifiedAccessParent, symbol.AutoOpenParent, symbol.Namespace, symbol.CleanedIdents)
        |> Assert.Single
    Assert.Equal(Some ns, entity.Namespace)
    Assert.Equal(qualifier, entity.Qualifier)
    Assert.Equal((if atTop then Position.mkPos 1 0 else Position.mkPos 4 8), insertion.Pos)
    let opened =
        if atTop then
            context.Source.Replace("namespace N\n", $"namespace N\nopen {entity.Namespace.Value}\n")
        else
            context.Source.Replace("        let classify", $"        open {entity.Namespace.Value}\n        let classify")
    let qualifiedPattern = if pattern = "Case" then qualifier else $"{qualifier}.Case"
    for edited in
        [ opened
          context.Source.Replace($"| {pattern} n ->", $"| {qualifiedPattern} n ->") ] do
        let _, checkedResults = checkSource edited
        assertNoDiagnostics checkedResults
        assertNoUnusedOpens checkedResults edited
        assertCaseUsed checkedResults symbol
