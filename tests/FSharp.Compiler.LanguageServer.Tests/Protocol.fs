module LanguageServer.Protocol

open System
open Xunit

open FSharp.Compiler.LanguageServer
open Microsoft.VisualStudio.LanguageServer.Protocol
open FSharp.Compiler.CodeAnalysis.Workspace

open LanguageServer.ProtocolHelpers

#nowarn "57"

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

