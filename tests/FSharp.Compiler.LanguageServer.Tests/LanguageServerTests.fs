module LanguageServerTests

open System
open Xunit

open FSharp.Compiler.LanguageServer
open StreamJsonRpc
open System.IO
open System.Diagnostics

open Microsoft.VisualStudio.LanguageServer.Protocol
open Nerdbank.Streams


type TestRpcClient(jsonRpc, rpcTrace, initializeResult: InitializeResult) =

    member val JsonRpc = jsonRpc
    member val RpcTraceWriter = rpcTrace
    member val Capabilities = initializeResult.Capabilities

    member _.RpcTrace = rpcTrace.ToString()


let initializeLanguageServer () =

    // Create a StringWriter to capture the output
    let rpcTrace = new StringWriter()

    let (inputStream, outputStream), server = FSharpLanguageServer.Create()

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

    let initializeParams = InitializeParams(
        ProcessId = System.Diagnostics.Process.GetCurrentProcess().Id,
        RootUri = Uri("file:///c:/temp"),
        InitializationOptions = None)

    jsonRpc.StartListening()

    task {
        let! response = jsonRpc.InvokeAsync<InitializeResult>("initialize", initializeParams);
        return TestRpcClient(jsonRpc, rpcTrace, response)
    }


[<Fact>]
let ``The server can process the initialization message`` () =
    task {

        let! client = initializeLanguageServer()
        Assert.NotNull(client.Capabilities)

    }