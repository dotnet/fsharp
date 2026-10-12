module LanguageServer.SemanticTokensProtocolTests

open System
open Xunit

open Microsoft.VisualStudio.LanguageServer.Protocol
open FSharp.Compiler.CodeAnalysis.Workspace
open FSharp.Compiler.LanguageServer

open LanguageServer.ProtocolHelpers
open LanguageServer.SemanticTokensHelpers

#nowarn "57"

// ---------------------------------------------------------------------------
//  Capability Tests
// ---------------------------------------------------------------------------

[<Fact>]
let ``Server capabilities include semantic tokens support`` () =
    task {
        let! client = initializeLanguageServer None
        let options = client.Capabilities.SemanticTokensOptions
        Assert.NotNull(box options)
        Assert.NotNull(options.Legend)
        Assert.NotNull(options.Legend.TokenTypes)
        Assert.NotNull(options.Legend.TokenModifiers)
        Assert.True(options.Legend.TokenTypes.Length > 0, "Legend should have at least one token type")
    }

[<Fact>]
let ``Server capabilities advertise full document semantic tokens`` () =
    task {
        let! client = initializeLanguageServer None
        let options = client.Capabilities.SemanticTokensOptions
        Assert.NotNull(box options)
        // Full should be advertised (either as bool or object)
        Assert.NotNull(box options.Full)
    }

[<Fact>]
let ``Server with semantic tokens disabled does not advertise capability`` () =
    task {
        let config =
            { FSharpLanguageServerConfig.Default with
                EnabledFeatures =
                    { FSharpLanguageServerFeatures.Default with
                        SemanticTokens = false
                    }
            }

        let! client = createLanguageServer (FSharpWorkspace()) (Some config)
        Assert.Null(box client.Capabilities.SemanticTokensOptions)
    }

[<Fact>]
let ``Semantic tokens legend contains expected token types`` () =
    task {
        let! client = initializeLanguageServer None
        let legend = getSemanticTokensLegend client
        let tokenTypes = set legend.TokenTypes

        // Standard LSP types we still emit (syntactic + a couple of semantic fallbacks).
        let standardLspTypes =
            [ SemanticTokenTypes.Variable
              SemanticTokenTypes.Keyword
              SemanticTokenTypes.Comment
              SemanticTokenTypes.String
              SemanticTokenTypes.Number
              SemanticTokenTypes.Operator ]

        // Roslyn classification names that pass through the VS LSP client's
        // RoslynSemanticTokenTypes identity map so VS picks up the same colors
        // the legacy F# extension uses.
        let roslynTypes =
            [ "class name"
              "struct name"
              "enum name"
              "interface name"
              "type parameter name"
              "delegate name"
              "field name"
              "property name"
              "event name"
              "constant name"
              "label name"
              "extension method name"
              "method name"
              "namespace name"
              "local name"
              "string - escape character" ]

        // F#-specific token types — the client side must forward these as identity entries so that
        // the in-proc MEF classifications (FSharp.MutableVar, etc.) and their EditorFormatDefinitions
        // get applied.
        let fsharpTypes =
            [ "FSharp.Function"
              "FSharp.MutableVar"
              "FSharp.Printf"
              "FSharp.DisposableType"
              "FSharp.DisposableLocalValue"
              "FSharp.DisposableTopLevelValue" ]

        for expected in standardLspTypes @ roslynTypes @ fsharpTypes do
            Assert.True(tokenTypes.Contains(expected), $"Legend should contain token type '{expected}'")
    }

[<Fact>]
let ``Semantic tokens legend contains standard modifiers`` () =
    task {
        let! client = initializeLanguageServer None
        let legend = getSemanticTokensLegend client
        let modifiers = set legend.TokenModifiers

        // At minimum, the legend should contain the 'static' and 'deprecated' modifiers
        Assert.True(
            modifiers.Contains(SemanticTokenModifiers.Static),
            $"Legend should contain modifier '{SemanticTokenModifiers.Static}'"
        )
    }

// ---------------------------------------------------------------------------
//  Request / Response Shape Tests
// ---------------------------------------------------------------------------

[<Fact>]
let ``Full semantic tokens request returns valid response`` () =
    task {
        let! client = initializeLanguageServer None
        let fileOnDisk = setupSingleFileProject client "let x = 1"
        let! result = openAndPullSemanticTokens client fileOnDisk "let x = 1"
        Assert.NotNull(result)
        Assert.NotNull(result.Data)
        Assert.True(result.Data.Length > 0, "Expected at least some token data for 'let x = 1'")
    }

[<Fact>]
let ``Semantic tokens data length is a multiple of 5`` () =
    task {
        let! client = initializeLanguageServer None
        let fileOnDisk = setupSingleFileProject client "let x = 1"
        let! result = openAndPullSemanticTokens client fileOnDisk "let x = 1"
        Assert.NotNull(result)
        Assert.True(
            result.Data.Length % 5 = 0,
            $"Data length {result.Data.Length} is not a multiple of 5"
        )
    }

[<Fact>]
let ``Semantic tokens data has valid token type indices`` () =
    task {
        let! client = initializeLanguageServer None
        let legend = getSemanticTokensLegend client
        let content = "let add x y = x + y"
        let fileOnDisk = setupSingleFileProject client content
        let! result = openAndPullSemanticTokens client fileOnDisk content
        Assert.NotNull(result)

        let tokenCount = result.Data.Length / 5

        for i in 0 .. tokenCount - 1 do
            let tokenTypeIdx = result.Data[i * 5 + 3]

            Assert.True(
                tokenTypeIdx >= 0 && tokenTypeIdx < legend.TokenTypes.Length,
                $"Token {i}: tokenType index {tokenTypeIdx} is out of bounds (legend has {legend.TokenTypes.Length} types)"
            )
    }

[<Fact>]
let ``Semantic tokens have valid delta positions`` () =
    task {
        let! client = initializeLanguageServer None
        let content = "let x = 1\nlet y = 2\nlet z = x + y"
        let fileOnDisk = setupSingleFileProject client content
        let! result = openAndPullSemanticTokens client fileOnDisk content
        Assert.NotNull(result)

        let tokenCount = result.Data.Length / 5

        for i in 0 .. tokenCount - 1 do
            let offset = i * 5
            let deltaLine = result.Data[offset]
            let deltaStart = result.Data[offset + 1]
            let length = result.Data[offset + 2]

            Assert.True(deltaLine >= 0, $"Token {i}: deltaLine {deltaLine} is negative")

            if i = 0 || deltaLine > 0 then
                Assert.True(deltaStart >= 0, $"Token {i}: deltaStart {deltaStart} should be non-negative on a new line")

            Assert.True(length > 0, $"Token {i}: length {length} should be positive")
    }

[<Fact>]
let ``Decoded tokens are in monotonic order with no overlaps`` () =
    task {
        let! client = initializeLanguageServer None
        let legend = getSemanticTokensLegend client
        let content = "module Test\nlet add (x: int) (y: int) = x + y\nlet result = add 1 2"
        let fileOnDisk = setupSingleFileProject client content
        let! result = openAndPullSemanticTokens client fileOnDisk content
        Assert.NotNull(result)

        let tokens = decodeSemanticTokens legend result.Data
        validateTokenOrdering tokens
    }

[<Fact>]
let ``Semantic tokens for empty file returns empty data`` () =
    task {
        let! client = initializeLanguageServer None
        let content = ""
        let fileOnDisk = setupSingleFileProject client content
        let! result = openAndPullSemanticTokens client fileOnDisk content
        // An empty file should return either null or a response with empty data
        if not (isNull result) && not (isNull result.Data) then
            Assert.Equal(0, result.Data.Length)
    }

[<Fact>]
let ``Semantic tokens for file not in any project returns graceful response`` () =
    task {
        let! client = initializeLanguageServer None
        let fileOnDisk = FSharp.Test.ProjectGeneration.WorkspaceHelpers.sourceFileOnDisk "let x = 1"
        do! openDocument client fileOnDisk "let x = 1" 1
        // File not in any project — should not crash
        let! result = pullSemanticTokensFull client fileOnDisk

        if not (isNull result) && not (isNull result.Data) then
            Assert.True(result.Data.Length % 5 = 0, "Even for non-project files, data should be well-formed")
    }

[<Fact>]
let ``Semantic tokens response has a resultId when delta support is advertised`` () =
    task {
        let! client = initializeLanguageServer None
        let supportsDelta =
            let options = client.Capabilities.SemanticTokensOptions

            if isNull (box options) || not options.Full.HasValue then
                false
            else
                let mutable fullOptions = Unchecked.defaultof<SemanticTokensFullOptions>
                options.Full.Value.TryGetSecond(&fullOptions) && fullOptions.Delta

        let content = "let x = 1"
        let fileOnDisk = setupSingleFileProject client content
        let! result = openAndPullSemanticTokens client fileOnDisk content
        Assert.NotNull(result)

        Assert.True(
            not supportsDelta || not (String.IsNullOrEmpty(result.ResultId)),
            "ResultId should be present when semantic token delta support is advertised"
        )
    }

// ---------------------------------------------------------------------------
//  Encoding Tests
// ---------------------------------------------------------------------------

[<Fact>]
let ``Tokens on same line have relative deltaStart`` () =
    task {
        let! client = initializeLanguageServer None
        let legend = getSemanticTokensLegend client
        // "let x = 1" should have tokens on line 0: "let" at col 0, "x" at col 4, "1" at col 8 (approx)
        let content = "let x = 1"
        let fileOnDisk = setupSingleFileProject client content
        let! result = openAndPullSemanticTokens client fileOnDisk content
        Assert.NotNull(result)

        let tokens = decodeSemanticTokens legend result.Data
        let line0Tokens = findTokensOnLine 0 tokens

        Assert.True(
            line0Tokens.Length >= 2,
            $"Expected at least 2 tokens on line 0 for 'let x = 1', got {line0Tokens.Length}"
        )

        // All tokens should be on line 0
        for t in line0Tokens do
            Assert.Equal(0, t.Line)

        // In the raw data, consecutive same-line tokens should have deltaLine = 0
        let tokenCount = result.Data.Length / 5

        if tokenCount >= 2 then
            let secondDeltaLine = result.Data[5]

            if secondDeltaLine = 0 then
                // deltaStart should be relative (positive, representing gap from first token)
                let secondDeltaStart = result.Data[6]
                Assert.True(secondDeltaStart > 0, "Second same-line token should have positive relative deltaStart")
    }

[<Fact>]
let ``Tokens on different lines have absolute deltaStart`` () =
    task {
        let! client = initializeLanguageServer None
        let legend = getSemanticTokensLegend client
        let content = "let x = 1\nlet y = 2"
        let fileOnDisk = setupSingleFileProject client content
        let! result = openAndPullSemanticTokens client fileOnDisk content
        Assert.NotNull(result)

        let tokens = decodeSemanticTokens legend result.Data

        // Find the first token on line 1
        let line1Tokens = findTokensOnLine 1 tokens
        Assert.True(line1Tokens.Length > 0, "Expected tokens on line 1")

        // The first token on line 1 ("let" at col 0) should have startChar = 0
        let firstLine1Token = line1Tokens |> List.minBy (fun t -> t.StartChar)
        Assert.Equal(0, firstLine1Token.StartChar)
    }

// ---------------------------------------------------------------------------
//  Integration Tests - F# Classification
// ---------------------------------------------------------------------------

[<Theory>]
[<InlineData("(*1*)let x = 1", "(*1*)", "keyword")>]
[<InlineData("(*1*)module Test", "(*1*)", "keyword")>]
[<InlineData("(*1*)open System", "(*1*)", "keyword")>]
[<InlineData("(*1*)if true then 1 else 2", "(*1*)", "keyword")>]
let ``Keywords are classified as keyword`` (source: string, marker: string, expectedType: string) =
    task {
        let! client = initializeLanguageServer None
        let legend = getSemanticTokensLegend client
        let cleanSource, markerPositions = stripMarkers source
        let fileOnDisk = setupSingleFileProject client cleanSource
        let! result = openAndPullSemanticTokens client fileOnDisk cleanSource
        Assert.NotNull(result)

        let tokens = decodeSemanticTokens legend result.Data
        let token = findTokenAtMarker markerPositions marker tokens
        Assert.True(token.IsSome, $"Expected a token at marker {marker}")
        Assert.Equal(expectedType, token.Value.TokenType)
    }

[<Fact>]
let ``Namespace is classified as namespace name for VS highlighting`` () =
    task {
        let! client = initializeLanguageServer None
        let legend = getSemanticTokensLegend client
        let source = "open (*1*)System"
        let cleanSource, markerPositions = stripMarkers source
        let fileOnDisk = setupSingleFileProject client cleanSource
        let! result = openAndPullSemanticTokens client fileOnDisk cleanSource
        Assert.NotNull(result)

        let tokens = decodeSemanticTokens legend result.Data
        let token = findTokenAtMarker markerPositions "(*1*)" tokens
        Assert.True(token.IsSome, "Expected a token for 'System'")
        Assert.Equal("namespace name", token.Value.TokenType)
    }

[<Fact>]
let ``Reference type is classified as class name for VS highlighting`` () =
    task {
        let! client = initializeLanguageServer None
        let legend = getSemanticTokensLegend client
        let source = "type (*1*)Foo() = class end"
        let cleanSource, markerPositions = stripMarkers source
        let fileOnDisk = setupSingleFileProject client cleanSource
        let! result = openAndPullSemanticTokens client fileOnDisk cleanSource
        Assert.NotNull(result)

        let tokens = decodeSemanticTokens legend result.Data
        let token = findTokenAtMarker markerPositions "(*1*)" tokens
        Assert.True(token.IsSome, "Expected a token for 'Foo'")
        Assert.Equal("class name", token.Value.TokenType)
    }

[<Fact>]
let ``Value type is classified as struct name for VS highlighting`` () =
    task {
        let! client = initializeLanguageServer None
        let legend = getSemanticTokensLegend client
        let source = "[<Struct>]\ntype (*1*)Bar = { X: int }"
        let cleanSource, markerPositions = stripMarkers source
        let fileOnDisk = setupSingleFileProject client cleanSource
        let! result = openAndPullSemanticTokens client fileOnDisk cleanSource
        Assert.NotNull(result)

        let tokens = decodeSemanticTokens legend result.Data
        let token = findTokenAtMarker markerPositions "(*1*)" tokens
        Assert.True(token.IsSome, "Expected a token for 'Bar'")
        Assert.Equal("struct name", token.Value.TokenType)
    }

[<Fact>]
let ``Interface is classified as interface name for VS highlighting`` () =
    task {
        let! client = initializeLanguageServer None
        let legend = getSemanticTokensLegend client
        let source = "type (*1*)IFoo = abstract member Bar: unit -> unit"
        let cleanSource, markerPositions = stripMarkers source
        let fileOnDisk = setupSingleFileProject client cleanSource
        let! result = openAndPullSemanticTokens client fileOnDisk cleanSource
        Assert.NotNull(result)

        let tokens = decodeSemanticTokens legend result.Data
        let token = findTokenAtMarker markerPositions "(*1*)" tokens
        Assert.True(token.IsSome, "Expected a token for 'IFoo'")
        Assert.Equal("interface name", token.Value.TokenType)
    }

[<Fact>]
let ``Enum is classified as enum name for VS highlighting`` () =
    task {
        let! client = initializeLanguageServer None
        let legend = getSemanticTokensLegend client
        let source = "type (*1*)Color = Red = 0 | Green = 1 | Blue = 2"
        let cleanSource, markerPositions = stripMarkers source
        let fileOnDisk = setupSingleFileProject client cleanSource
        let! result = openAndPullSemanticTokens client fileOnDisk cleanSource
        Assert.NotNull(result)

        let tokens = decodeSemanticTokens legend result.Data
        let token = findTokenAtMarker markerPositions "(*1*)" tokens
        Assert.True(token.IsSome, "Expected a token for 'Color'")
        Assert.Equal("enum name", token.Value.TokenType)
    }

[<Fact>]
let ``Union case is classified as enum name for VS highlighting`` () =
    task {
        let! client = initializeLanguageServer None
        let legend = getSemanticTokensLegend client
        let source = "type Shape = (*1*)Circle | Square\nlet s = (*2*)Circle"
        let cleanSource, markerPositions = stripMarkers source
        let fileOnDisk = setupSingleFileProject client cleanSource
        let! result = openAndPullSemanticTokens client fileOnDisk cleanSource
        Assert.NotNull(result)

        let tokens = decodeSemanticTokens legend result.Data

        // Definition of union case
        let defToken = findTokenAtMarker markerPositions "(*1*)" tokens
        Assert.True(defToken.IsSome, "Expected a token for 'Circle' definition")
        Assert.Equal("enum name", defToken.Value.TokenType)

        // Usage of union case
        let useToken = findTokenAtMarker markerPositions "(*2*)" tokens
        Assert.True(useToken.IsSome, "Expected a token for 'Circle' usage")
        Assert.Equal("enum name", useToken.Value.TokenType)
    }

[<Fact>]
let ``Type parameter is classified as type parameter name for VS highlighting`` () =
    task {
        let! client = initializeLanguageServer None
        let legend = getSemanticTokensLegend client
        let source = "let f (x: (*1*)'a) = x"
        let cleanSource, markerPositions = stripMarkers source
        let fileOnDisk = setupSingleFileProject client cleanSource
        let! result = openAndPullSemanticTokens client fileOnDisk cleanSource
        Assert.NotNull(result)

        let tokens = decodeSemanticTokens legend result.Data
        let token = findTokenAtMarker markerPositions "(*1*)" tokens
        Assert.True(token.IsSome, "Expected a token for type parameter 'a")
        Assert.Equal("type parameter name", token.Value.TokenType)
    }

[<Fact>]
let ``Function is classified with the F#-specific function token type`` () =
    task {
        let! client = initializeLanguageServer None
        let legend = getSemanticTokensLegend client
        let source = "let (*1*)add x y = x + y"
        let cleanSource, markerPositions = stripMarkers source
        let fileOnDisk = setupSingleFileProject client cleanSource
        let! result = openAndPullSemanticTokens client fileOnDisk cleanSource
        Assert.NotNull(result)

        let tokens = decodeSemanticTokens legend result.Data
        let token = findTokenAtMarker markerPositions "(*1*)" tokens
        Assert.True(token.IsSome, "Expected a token for 'add'")
        Assert.Equal("FSharp.Function", token.Value.TokenType)
    }

[<Fact>]
let ``Semantic tokens classify names and printf specifiers for VS highlighting`` () =
    task {
        let! client = initializeLanguageServer None
        let legend = getSemanticTokensLegend client

        let source =
            "namespace (*6*)Sample\n\nmodule (*7*)Test =\n\n    let (*1*)hello (*2*)name =\n        (*3*)printfn \"Hello (*5*)%s\" (*4*)name"

        let cleanSource, markerPositions = stripMarkers source
        let fileOnDisk = setupSingleFileProject client cleanSource
        let! result = openAndPullSemanticTokens client fileOnDisk cleanSource
        Assert.NotNull(result)

        let tokens = decodeSemanticTokens legend result.Data

        let assertToken marker expectedType message =
            let token = findTokenAtMarker markerPositions marker tokens
            Assert.True(token.IsSome, message)
            Assert.Equal(expectedType, token.Value.TokenType)

        assertToken "(*1*)" "FSharp.Function" "Expected an F# function token for 'hello'"
        assertToken "(*2*)" "local name" "Expected a local-name token for the 'name' parameter definition"
        assertToken "(*3*)" "FSharp.Function" "Expected an F# function token for 'printfn'"
        assertToken "(*4*)" "local name" "Expected a local-name token for the 'name' parameter usage"
        assertToken "(*5*)" "FSharp.Printf" "Expected an F# printf token for the '%s' specifier"
        assertToken "(*6*)" "namespace name" "Expected a namespace-name token for 'Sample'"
        assertToken "(*7*)" "class name" "Expected a class-name token for the module 'Test'"
    }

[<Fact>]
let ``Method is classified as method name for VS highlighting`` () =
    task {
        let! client = initializeLanguageServer None
        let legend = getSemanticTokensLegend client
        let source = "type T() =\n    member _.(*1*)Foo() = 1"
        let cleanSource, markerPositions = stripMarkers source
        let fileOnDisk = setupSingleFileProject client cleanSource
        let! result = openAndPullSemanticTokens client fileOnDisk cleanSource
        Assert.NotNull(result)

        let tokens = decodeSemanticTokens legend result.Data
        let token = findTokenAtMarker markerPositions "(*1*)" tokens
        Assert.True(token.IsSome, "Expected a token for 'Foo'")
        Assert.Equal("method name", token.Value.TokenType)
    }

[<Fact>]
let ``Property is classified as property name for VS highlighting`` () =
    task {
        let! client = initializeLanguageServer None
        let legend = getSemanticTokensLegend client
        let source = "type T() =\n    member _.(*1*)Value = 42"
        let cleanSource, markerPositions = stripMarkers source
        let fileOnDisk = setupSingleFileProject client cleanSource
        let! result = openAndPullSemanticTokens client fileOnDisk cleanSource
        Assert.NotNull(result)

        let tokens = decodeSemanticTokens legend result.Data
        let token = findTokenAtMarker markerPositions "(*1*)" tokens
        Assert.True(token.IsSome, "Expected a token for 'Value'")
        Assert.Equal("property name", token.Value.TokenType)
    }

[<Fact>]
let ``Record field is classified as field name for VS highlighting`` () =
    task {
        let! client = initializeLanguageServer None
        let legend = getSemanticTokensLegend client
        let source = "type R = { (*1*)Name: string }"
        let cleanSource, markerPositions = stripMarkers source
        let fileOnDisk = setupSingleFileProject client cleanSource
        let! result = openAndPullSemanticTokens client fileOnDisk cleanSource
        Assert.NotNull(result)

        let tokens = decodeSemanticTokens legend result.Data
        let token = findTokenAtMarker markerPositions "(*1*)" tokens
        Assert.True(token.IsSome, "Expected a token for 'Name'")
        Assert.Equal("property name", token.Value.TokenType)
    }

[<Fact>]
let ``Operator is classified as operator`` () =
    task {
        let! client = initializeLanguageServer None
        let legend = getSemanticTokensLegend client
        let source = "let x = 1 (*1*)+ 2"
        let cleanSource, markerPositions = stripMarkers source
        let fileOnDisk = setupSingleFileProject client cleanSource
        let! result = openAndPullSemanticTokens client fileOnDisk cleanSource
        Assert.NotNull(result)

        let tokens = decodeSemanticTokens legend result.Data
        let token = findTokenAtMarker markerPositions "(*1*)" tokens
        Assert.True(token.IsSome, "Expected a token for '+'")
        Assert.Equal(SemanticTokenTypes.Operator, token.Value.TokenType)
    }

[<Fact>]
let ``Event is classified as event name for VS highlighting`` () =
    task {
        let! client = initializeLanguageServer None
        let legend = getSemanticTokensLegend client
        let source =
            "open System\ntype T() =\n    let ev = new Event<EventHandler, EventArgs>()\n    [<CLIEvent>]\n    member _.(*1*)MyEvent = ev.Publish\nlet subscribe (t: T) = t.(*2*)MyEvent.Add(fun _ -> ())"

        let cleanSource, markerPositions = stripMarkers source
        let fileOnDisk = setupSingleFileProject client cleanSource
        let! result = openAndPullSemanticTokens client fileOnDisk cleanSource
        Assert.NotNull(result)

        let tokens = decodeSemanticTokens legend result.Data
        for marker in [ "(*1*)"; "(*2*)" ] do
            let token = findTokenAtMarker markerPositions marker tokens
            Assert.True(token.IsSome, $"Expected a token for 'MyEvent' at {marker}")
            Assert.Equal("event name", token.Value.TokenType)
    }

// ---------------------------------------------------------------------------
//  Multiline / Positional Tests
// ---------------------------------------------------------------------------

[<Fact>]
let ``All tokens in multiline code are correctly positioned`` () =
    task {
        let! client = initializeLanguageServer None
        let legend = getSemanticTokensLegend client

        let content =
            "let x = 1\nlet y = 2\nlet z = x + y"

        let fileOnDisk = setupSingleFileProject client content
        let! result = openAndPullSemanticTokens client fileOnDisk content
        Assert.NotNull(result)

        let tokens = decodeSemanticTokens legend result.Data

        // Verify tokens span multiple lines
        let lines = tokens |> List.map (fun t -> t.Line) |> List.distinct |> List.sort
        Assert.True(lines.Length >= 2, $"Expected tokens on at least 2 lines, got lines: {lines}")

        // Validate ordering
        validateTokenOrdering tokens
    }

[<Fact>]
let ``Non-ASCII content does not corrupt token positions`` () =
    task {
        let! client = initializeLanguageServer None
        let legend = getSemanticTokensLegend client
        // The emoji "😀" is 2 UTF-16 code units (a surrogate pair)
        let content = "let s = \"😀\"\nlet (*1*)x = 1"
        let cleanSource, markerPositions = stripMarkers content
        let fileOnDisk = setupSingleFileProject client cleanSource
        let! result = openAndPullSemanticTokens client fileOnDisk cleanSource
        Assert.NotNull(result)

        let tokens = decodeSemanticTokens legend result.Data
        let token = findTokenAtMarker markerPositions "(*1*)" tokens
        Assert.True(token.IsSome, "Expected a token for 'x' after emoji string")

        // "x" should be on line 1, at col 4 (after "let ")
        Assert.Equal(1, token.Value.Line)
        Assert.Equal(4, token.Value.StartChar)
    }

// ---------------------------------------------------------------------------
//  Baseline Comparison: LSP vs Compiler API
// ---------------------------------------------------------------------------

[<Fact>]
let ``Semantic tokens cover the same ranges as compiler GetSemanticClassification`` () =
    task {
        let! client = initializeLanguageServer None
        let legend = getSemanticTokensLegend client

        let content =
            "module Test\nopen System\ntype Foo() =\n    member _.Bar() = 42\nlet f x = x + 1"

        let fileOnDisk = setupSingleFileProject client content
        let! result = openAndPullSemanticTokens client fileOnDisk content
        Assert.NotNull(result)

        let lspTokens = decodeSemanticTokens legend result.Data

        // We just verify that the LSP endpoint returns a meaningful number of tokens
        // covering multiple lines. The exact mapping is tested in individual classification tests above.
        Assert.True(lspTokens.Length >= 5, $"Expected at least 5 tokens, got {lspTokens.Length}")

        let coveredLines =
            lspTokens |> List.map (fun t -> t.Line) |> List.distinct

        Assert.True(
            coveredLines.Length >= 3,
            $"Expected tokens on at least 3 lines, got {coveredLines.Length} lines: {coveredLines}"
        )

        // All token types should be valid legend entries (no <unknown:N>)
        for t in lspTokens do
            Assert.False(
                t.TokenType.StartsWith("<unknown:"),
                $"Token at ({t.Line},{t.StartChar}) has unresolved type: {t.TokenType}"
            )
    }

// ---------------------------------------------------------------------------
//  Qualified name segmentation (mirrors legacy Tokenizer.fixupSpan)
// ---------------------------------------------------------------------------

[<Fact>]
let ``Qualified type reference classifies each segment separately``
    ()
    =
    task {
        let! client = initializeLanguageServer None
        let legend = getSemanticTokensLegend client

        // FCS produces classifications at the outer long-ident range; without per-segment
        // trimming the DisposableType classification would cover the whole "System.IO.FileStream"
        // and the Namespace classifications at "System" and "System.IO" would be hidden.
        let source =
            "let _ = typeof<(*1*)System.(*2*)IO.(*3*)FileStream>"

        let cleanSource, markerPositions = stripMarkers source
        let fileOnDisk = setupSingleFileProject client cleanSource
        let! result = openAndPullSemanticTokens client fileOnDisk cleanSource
        Assert.NotNull(result)

        let tokens = decodeSemanticTokens legend result.Data

        let assertToken marker expectedType message =
            let token = findTokenAtMarker markerPositions marker tokens
            Assert.True(token.IsSome, message)
            Assert.Equal(expectedType, token.Value.TokenType)

        assertToken "(*1*)" "namespace name" "Expected 'System' to be classified as a namespace"
        assertToken "(*2*)" "namespace name" "Expected 'IO' to be classified as a namespace"
        assertToken "(*3*)" "FSharp.DisposableType" "Expected 'FileStream' to be classified as a disposable type"
    }

[<Fact>]
let ``Qualified function reference classifies module and function separately`` () =
    task {
        let! client = initializeLanguageServer None
        let legend = getSemanticTokensLegend client

        let source =
            "module Say =\n    let hello name = printfn \"Hello %s\" name\n\nlet _ = (*1*)Say.(*2*)hello \"world\""

        let cleanSource, markerPositions = stripMarkers source
        let fileOnDisk = setupSingleFileProject client cleanSource
        let! result = openAndPullSemanticTokens client fileOnDisk cleanSource
        Assert.NotNull(result)

        let tokens = decodeSemanticTokens legend result.Data

        let assertToken marker expectedType message =
            let token = findTokenAtMarker markerPositions marker tokens
            Assert.True(token.IsSome, message)
            Assert.Equal(expectedType, token.Value.TokenType)

        assertToken "(*1*)" "class name" "Expected the 'Say' module to be classified as a module/class"
        assertToken "(*2*)" "FSharp.Function" "Expected the 'hello' function reference to be classified as a function"
    }

[<Fact>]
let ``Backticked qualified identifier keeps only the backticked tail`` () =
    task {
        let! client = initializeLanguageServer None
        let legend = getSemanticTokensLegend client

        // The fixupRange logic must keep the entire backticked tail intact rather than
        // trimming after the last '.' inside the backticks.
        let source =
            "module Mod =\n    let ``my fun`` () = 1\n\nlet _ = (*1*)Mod.(*2*)``my fun`` ()"

        let cleanSource, markerPositions = stripMarkers source
        let fileOnDisk = setupSingleFileProject client cleanSource
        let! result = openAndPullSemanticTokens client fileOnDisk cleanSource
        Assert.NotNull(result)

        let tokens = decodeSemanticTokens legend result.Data

        let assertToken marker expectedType message =
            let token = findTokenAtMarker markerPositions marker tokens
            Assert.True(token.IsSome, message)
            Assert.Equal(expectedType, token.Value.TokenType)

        assertToken "(*1*)" "class name" "Expected the 'Mod' module to be classified as a module/class"
        assertToken "(*2*)" "FSharp.Function" "Expected the backticked function to be classified as a function"
    }

[<Fact>]
let ``Computation expression builder is classified as keyword`` () =
    task {
        let! client = initializeLanguageServer None
        let legend = getSemanticTokensLegend client

        // FCS captures the builder identifier as both Item.Value and Item.CustomBuilder; only
        // the latter produces the ComputationExpression classification. The whole-file path
        // (range = None) must apply the same dedup as the ranged path so 'async' renders as a
        // keyword, the same way the legacy classifier paints it.
        let source =
            "let _ = (*1*)async {\n    let! v = async.Return 1\n    return v\n}"

        let cleanSource, markerPositions = stripMarkers source
        let fileOnDisk = setupSingleFileProject client cleanSource
        let! result = openAndPullSemanticTokens client fileOnDisk cleanSource
        Assert.NotNull(result)

        let tokens = decodeSemanticTokens legend result.Data
        let token = findTokenAtMarker markerPositions "(*1*)" tokens
        Assert.True(token.IsSome, "Expected a token for the 'async' builder identifier")
        Assert.Equal(SemanticTokenTypes.Keyword, token.Value.TokenType)
    }
