module LanguageServer.ProtocolHelpers

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

let createLanguageServer (workspace: FSharpWorkspace) (config: FSharpLanguageServerConfig option) =
    let rpcTrace = new StringWriter()

    let (inputStream, outputStream), server =
        match config with
        | Some cfg -> FSharpLanguageServer.Create(LspLogger Trace.TraceInformation, workspace, config = cfg)
        | None -> FSharpLanguageServer.Create(workspace)

    let formatter = new JsonMessageFormatter()
    VSExtensionUtilities.AddVSExtensionConverters(formatter.JsonSerializer)

    let messageHandler =
        new HeaderDelimitedMessageHandler(inputStream, outputStream, formatter)

    let jsonRpc = new JsonRpc(messageHandler)

    let listener = new TextWriterTraceListener(rpcTrace)
    server.JsonRpc.TraceSource.Listeners.Add(listener) |> ignore
    server.JsonRpc.TraceSource.Switch.Level <- SourceLevels.All

    let initializeParams =
        InitializeParams(
            ProcessId = Process.GetCurrentProcess().Id,
            RootUri = Uri("file:///c:/temp"),
            InitializationOptions = None
        )

    jsonRpc.StartListening()

    task {
        let! response = jsonRpc.InvokeAsync<InitializeResult>("initialize", initializeParams)
        return TestRpcClient(jsonRpc, rpcTrace, workspace, response)
    }

let initializeLanguageServer (workspace) =
    let workspace = defaultArg workspace (FSharpWorkspace())
    createLanguageServer workspace None

[<Literal>]
let cleanCode = "let x = 1"

[<Literal>]
let notMutableCode = "let x = 1\nx <- 2"

let openDocument (client: TestRpcClient) (fileUri: Uri) (content: string) (version: int) =
    client.JsonRpc.NotifyAsync(
        Methods.TextDocumentDidOpenName,
        DidOpenTextDocumentParams(
            TextDocument = TextDocumentItem(Uri = fileUri, LanguageId = "F#", Version = version, Text = content)))

let changeDocument (client: TestRpcClient) (fileUri: Uri) (content: string) (version: int) =
    client.JsonRpc.NotifyAsync(
        Methods.TextDocumentDidChangeName,
        DidChangeTextDocumentParams(
            TextDocument = VersionedTextDocumentIdentifier(Uri = fileUri, Version = version),
            ContentChanges = [| TextDocumentContentChangeEvent(Text = content) |]))

let closeDocument (client: TestRpcClient) (fileUri: Uri) =
    client.JsonRpc.NotifyAsync(
        Methods.TextDocumentDidCloseName,
        DidCloseTextDocumentParams(TextDocument = TextDocumentIdentifier(Uri = fileUri)))

let pullDiagnosticResponse (client: TestRpcClient) (fileUri: Uri) =
    client.JsonRpc.InvokeAsync<SumType<RelatedFullDocumentDiagnosticReport, RelatedUnchangedDocumentDiagnosticReport>>(
        Methods.TextDocumentDiagnosticName,
        DocumentDiagnosticParams(TextDocument = TextDocumentIdentifier(Uri = fileUri)))

let pullDiagnostics (client: TestRpcClient) (fileUri: Uri) =
    task {
        let! response = pullDiagnosticResponse client fileUri
        return response.First
    }

let setupSingleFileProject (client: TestRpcClient) (content: string) =
    let fileOnDisk = sourceFileOnDisk content
    let _pid = client.Workspace.Projects.AddOrUpdate(ProjectConfig.Create(), [ fileOnDisk.LocalPath ])
    fileOnDisk

let openAndPullDiagnostics (client: TestRpcClient) (fileUri: Uri) (content: string) =
    task {
        do! openDocument client fileUri content 1
        let! report = pullDiagnostics client fileUri
        return report.Items
    }

let pullVsDiagnosticsRaw(client: TestRpcClient) (fileUri: Uri) =
    client.JsonRpc.InvokeWithParameterObjectAsync<Newtonsoft.Json.Linq.JToken>(
        Methods.TextDocumentDiagnosticName,
        DocumentDiagnosticParams(TextDocument = TextDocumentIdentifier(Uri = fileUri)))

let openAndPullVsDiagnosticsRaw (client: TestRpcClient) (fileUri: Uri) (content: string) =
    task {
        do! openDocument client fileUri content 1
        return! pullVsDiagnosticsRaw client fileUri
    }

let getVsDiagnosticItems (response: Newtonsoft.Json.Linq.JToken) =
    let items = response["items"]
    Assert.True(items <> null, "Expected 'items' property in diagnostic response")
    items :?> Newtonsoft.Json.Linq.JArray

let getVsProjects (diagnosticItem: Newtonsoft.Json.Linq.JToken) =
    let projects = diagnosticItem["_vs_projects"]
    Assert.True(projects <> null, "Expected '_vs_projects' property in VS diagnostic")
    projects :?> Newtonsoft.Json.Linq.JArray

let getVsProjectName (project: Newtonsoft.Json.Linq.JToken) =
    let name = project["_vs_projectName"]
    Assert.True(name <> null, "Expected '_vs_projectName' property in project info")
    name.ToString()

let getVsProjectIdentifier (project: Newtonsoft.Json.Linq.JToken) =
    let id = project["_vs_projectIdentifier"]
    Assert.True(id <> null, "Expected '_vs_projectIdentifier' property in project info")
    id.ToString()

let getProjectContexts (client: TestRpcClient) (fileUri: Uri) =
    client.JsonRpc.InvokeAsync<VSProjectContextList>(
        "textDocument/_vs_getProjectContexts",
        VSGetProjectContextsParams(TextDocument = TextDocumentItem(Uri = fileUri)))

[<Literal>]
let sharedModuleContent = "module Shared\nlet x = 1"

let setupMultiProjectFile (client: TestRpcClient) (content: string) (projectNames: string list) =
    let fileOnDisk = sourceFileOnDisk content
    for name in projectNames do
        client.Workspace.Projects.AddOrUpdate(ProjectConfig.Empty(name = name), [ fileOnDisk.LocalPath ]) |> ignore
    fileOnDisk

let requestCodeActions (client: TestRpcClient) (fileUri: Uri) (line: int) =
    let range = Range(Start = Position(Line = line, Character = 0), End = Position(Line = line, Character = 999))
    client.JsonRpc.InvokeAsync<CodeAction array>(
        Methods.TextDocumentCodeActionName,
        CodeActionParams(
            TextDocument = TextDocumentIdentifier(Uri = fileUri),
            Range = range,
            Context = CodeActionContext(Diagnostics = [||])))

let openAndRequestCodeActions (client: TestRpcClient) (fileUri: Uri) (content: string) (line: int) =
    task {
        do! openDocument client fileUri content 1
        // Pull diagnostics first so the server has them cached
        let! _diags = pullDiagnostics client fileUri
        return! requestCodeActions client fileUri line
    }
