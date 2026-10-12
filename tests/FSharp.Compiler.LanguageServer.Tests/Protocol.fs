module LanguageServer.Protocol

open System
open System.Text.Json
open Xunit

open FSharp.Compiler.LanguageServer
open Microsoft.VisualStudio.LanguageServer.Protocol
open FSharp.Compiler.CodeAnalysis.Workspace

open LanguageServer.ProtocolHelpers

#nowarn "57"

[<Fact>]
let ``Protocol formatter round trips all VS base type mappings`` () =
    let options = (createConfiguredFormatter ()).JsonSerializerOptions
    let uri = Uri("file:///c:/project%20name/file%23name.fs")
    let range = Range(Start = Position(Line = 1, Character = 2), End = Position(Line = 3, Character = 4))

    let values =
        [ Diagnostic(Range = range, Message = "diagnostic") :> obj, typeof<Diagnostic>, typeof<VSDiagnostic>
          Location(Uri = uri, Range = range) :> obj, typeof<Location>, typeof<VSLocation>
          ServerCapabilities() :> obj, typeof<ServerCapabilities>, typeof<VSServerCapabilities>
          SymbolInformation(Name = "symbol", Kind = SymbolKind.Function, Location = Location(Uri = uri, Range = range)) :> obj,
          typeof<SymbolInformation>,
          typeof<VSSymbolInformation>
          TextDocumentIdentifier(Uri = uri) :> obj, typeof<TextDocumentIdentifier>, typeof<VSTextDocumentIdentifier> ]

    for value, baseType, extensionType in values do
        let json = JsonSerializer.Serialize(value, baseType, options)
        let result = JsonSerializer.Deserialize(json, baseType, options)
        Assert.NotNull(result)
        Assert.Equal(extensionType, result.GetType())
        use original = JsonDocument.Parse(json)
        use roundTripped = JsonDocument.Parse(JsonSerializer.Serialize(result, baseType, options))

        for property in original.RootElement.EnumerateObject() do
            Assert.True(JsonElement.DeepEquals(property.Value, roundTripped.RootElement.GetProperty(property.Name)))

[<Fact>]
let ``Protocol formatter preserves VS project context and document URI`` () =
    let options = (createConfiguredFormatter ()).JsonSerializerOptions
    let uri = Uri("file:///c:/project%20name/file%23name.fs")
    let context = VSProjectContext(Id = "project-id", Label = "project", Kind = VSProjectKind.CSharp)
    let identifier = VSTextDocumentIdentifier(Uri = uri, ProjectContext = context)
    let json = JsonSerializer.Serialize<TextDocumentIdentifier>(identifier, options)
    let result = Assert.IsType<VSTextDocumentIdentifier>(JsonSerializer.Deserialize<TextDocumentIdentifier>(json, options))
    Assert.Equal(uri, result.Uri)
    Assert.Equal(context.Id, result.ProjectContext.Id)
    Assert.Equal(context.Label, result.ProjectContext.Label)
    Assert.Null(JsonSerializer.Deserialize<TextDocumentIdentifier>("null", options))

[<Fact>]
let ``The server can process the initialization message`` () =
    task {

        let! client = initializeLanguageServer None
        Assert.NotNull(client.Capabilities)

    }

[<Fact>]
let ``Basic server workflow`` () =
    task {
        let! client = initializeLanguageServer None
        let fileOnDisk = setupSingleFileProject client cleanCode
        do! openDocument client fileOnDisk cleanCode 1

        let! report = pullDiagnostics client fileOnDisk
        Assert.Equal(0, report.Items.Length)

        do! changeDocument client fileOnDisk notMutableCode 2

        let! report = pullDiagnostics client fileOnDisk
        Assert.Equal(1, report.Items.Length)
        Assert.Contains("This value is not mutable", report.Items[0].Message)

        do! closeDocument client fileOnDisk

        let! report = pullDiagnostics client fileOnDisk
        Assert.Equal(0, report.Items.Length)
    }

[<Fact>]
let ``Shutdown and exit`` () =
    task {
        let! client = initializeLanguageServer None

        let! _respone = client.JsonRpc.InvokeAsync<_>(Methods.ShutdownName)

        do! client.JsonRpc.NotifyAsync(Methods.ExitName)
    }

[<Fact>]
let ``Server capabilities include diagnostics support`` () =
    task {
        let! client = initializeLanguageServer None
        Assert.NotNull(client.Capabilities.DiagnosticOptions)
        Assert.True(client.Capabilities.DiagnosticOptions.InterFileDependencies, "DiagnosticOptions should advertise InterFileDependencies")
        Assert.True(client.Capabilities.DiagnosticOptions.WorkspaceDiagnostics, "DiagnosticOptions should advertise WorkspaceDiagnostics")
    }

[<Fact>]
let ``Server capabilities include text document sync`` () =
    task {
        let! client = initializeLanguageServer None
        let syncOptions: TextDocumentSyncOptions = unbox client.Capabilities.TextDocumentSync
        Assert.True(syncOptions.OpenClose, "TextDocumentSync should support OpenClose")
        Assert.True(syncOptions.Change.HasValue, "TextDocumentSync should specify a Change kind")
        Assert.Equal(TextDocumentSyncKind.Full, syncOptions.Change.Value)
    }
