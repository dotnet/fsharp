module LanguageServer.Protocol

open System
open Xunit

open FSharp.Compiler.LanguageServer
open StreamJsonRpc
open System.IO
open System.Diagnostics

open Microsoft.VisualStudio.LanguageServer.Protocol
open Nerdbank.Streams

open FSharp.Test.ProjectGeneration.WorkspaceHelpers
open FSharp.Compiler.CodeAnalysis.Workspace
open FSharp.Compiler.CodeAnalysis.ProjectSnapshot

#nowarn "57"

type TestRpcClient(jsonRpc, rpcTrace, workspace, initializeResult: InitializeResult) =

    member val JsonRpc = jsonRpc
    member val RpcTraceWriter = rpcTrace
    member val Workspace = workspace
    member val Capabilities = initializeResult.Capabilities

    member _.RpcTrace = rpcTrace.ToString()

let initializeLanguageServer (workspace) =

    let workspace = defaultArg workspace (FSharpWorkspace())

    // Create a StringWriter to capture the output
    let rpcTrace = new StringWriter()

    let (inputStream, outputStream), server = FSharpLanguageServer.Create(workspace)

    let formatter = new JsonMessageFormatter()

    let messageHandler =
        new HeaderDelimitedMessageHandler(inputStream, outputStream, formatter)

    let jsonRpc = new JsonRpc(messageHandler)

    // Create a new TraceListener with the StringWriter
    let listener = new TextWriterTraceListener(rpcTrace)

    // Add the listener to the JsonRpc TraceSource
    server.JsonRpc.TraceSource.Listeners.Add(listener) |> ignore

    // Set the TraceLevel to Information to get all informational, warning and error messages
    server.JsonRpc.TraceSource.Switch.Level <- SourceLevels.All

    let initializeParams =
        InitializeParams(
            ProcessId = System.Diagnostics.Process.GetCurrentProcess().Id,
            RootUri = Uri("file:///c:/temp"),
            InitializationOptions = None
        )

    jsonRpc.StartListening()

    task {
        let! response = jsonRpc.InvokeAsync<InitializeResult>("initialize", initializeParams)
        return TestRpcClient(jsonRpc, rpcTrace, workspace, response)
    }

[<Fact>]
let ``The server can process the initialization message`` () =
    task {

        let! client = initializeLanguageServer None
        Assert.NotNull(client.Capabilities)

    }

/// Initialize workspace, open a document, get diagnostics, edit the document, get diagnostics, close the document
[<Fact>]
let ``Basic server workflow`` () =
    task {

        let! client = initializeLanguageServer None

        let workspace = client.Workspace

        let contentOnDisk = "let x = 1"
        let fileOnDisk = sourceFileOnDisk contentOnDisk

        let _projectIdentifier =
            workspace.Projects.AddOrUpdate(ProjectConfig.Create(), [ fileOnDisk.LocalPath ])

        do!
            client.JsonRpc.NotifyAsync(
                Methods.TextDocumentDidOpenName,
                DidOpenTextDocumentParams(
                    TextDocument = TextDocumentItem(Uri = fileOnDisk, LanguageId = "F#", Version = 1, Text = contentOnDisk)
                )
            )

        let! diagnosticsResponse =
            client.JsonRpc.InvokeAsync<SumType<RelatedFullDocumentDiagnosticReport, RelatedUnchangedDocumentDiagnosticReport>>(
                Methods.TextDocumentDiagnosticName,
                DocumentDiagnosticParams(TextDocument = TextDocumentIdentifier(Uri = fileOnDisk))
            )

        Assert.Equal(
            0,
            (diagnosticsResponse.First.RelatedDocuments
             |> Seq.head
             |> _.Value.First.Items.Length)
        )

        let contentEdit = $"{contentOnDisk}\nx <- 2"

        do!
            client.JsonRpc.NotifyAsync(
                Methods.TextDocumentDidChangeName,
                DidChangeTextDocumentParams(
                    TextDocument = VersionedTextDocumentIdentifier(Uri = fileOnDisk, Version = 2),
                    ContentChanges = [| TextDocumentContentChangeEvent(Text = contentEdit) |]
                )
            )

        let! diagnosticsResponse =
            client.JsonRpc.InvokeAsync<SumType<RelatedFullDocumentDiagnosticReport, RelatedUnchangedDocumentDiagnosticReport>>(
                Methods.TextDocumentDiagnosticName,
                DocumentDiagnosticParams(TextDocument = TextDocumentIdentifier(Uri = fileOnDisk))
            )

        let diagnostics =
            diagnosticsResponse.First.RelatedDocuments |> Seq.head |> _.Value.First.Items

        Assert.Equal(1, diagnostics.Length)
        Assert.Contains("This value is not mutable", diagnostics[0].Message)

        do!
            client.JsonRpc.NotifyAsync(
                Methods.TextDocumentDidCloseName,
                DidCloseTextDocumentParams(TextDocument = TextDocumentIdentifier(Uri = fileOnDisk))
            )

        let! diagnosticsResponse =
            client.JsonRpc.InvokeAsync<SumType<RelatedFullDocumentDiagnosticReport, RelatedUnchangedDocumentDiagnosticReport>>(
                Methods.TextDocumentDiagnosticName,
                DocumentDiagnosticParams(TextDocument = TextDocumentIdentifier(Uri = fileOnDisk))
            )

        // We didn't save the file, so it should be again read from disk and have no diagnostics
        Assert.Equal(
            0,
            (diagnosticsResponse.First.RelatedDocuments
             |> Seq.head
             |> _.Value.First.Items.Length)
        )
    }

[<Fact>]
let ``Full semantic tokens`` () =

    task {
        let! client = initializeLanguageServer None
        let workspace = client.Workspace
        let contentOnDisk = "let x = 1"
        let fileOnDisk = sourceFileOnDisk contentOnDisk
        let _projectIdentifier =
            workspace.Projects.AddOrUpdate(ProjectConfig.Create(), [ fileOnDisk.LocalPath ])
        do!
            client.JsonRpc.NotifyAsync(
                Methods.TextDocumentDidOpenName,
                DidOpenTextDocumentParams(
                    TextDocument = TextDocumentItem(Uri = fileOnDisk, LanguageId = "F#", Version = 1, Text = contentOnDisk)
                )
            )
        let! semanticTokensResponse =
            client.JsonRpc.InvokeAsync<SemanticTokens>(
                Methods.TextDocumentSemanticTokensFullName,
                SemanticTokensParams(TextDocument = TextDocumentIdentifier(Uri = fileOnDisk))
            )

        let expected = [| 0; 0; 0; 1; 0; 0; 0; 3; 15; 0; 0; 4; 1; 17; 0; 0; 4; 1; 19; 0 |]

        Assert.Equal<int array>(expected, semanticTokensResponse.Data)
    }

[<Fact>]
let ``Shutdown and exit`` () =
    task {
        let! client = initializeLanguageServer None

        let! _respone = client.JsonRpc.InvokeAsync<_>(Methods.ShutdownName)

        do! client.JsonRpc.NotifyAsync(Methods.ExitName)
    }
