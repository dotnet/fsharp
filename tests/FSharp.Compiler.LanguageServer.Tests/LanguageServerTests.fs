module LanguageServerTests

open System
open Xunit

open FSharp.Compiler.LanguageServer
open StreamJsonRpc
open System.IO
open System.Diagnostics

open Microsoft.VisualStudio.LanguageServer.Protocol
open Nerdbank.Streams


[<Fact>]
let ``The server can process the initialization message`` () =

     // Create a StringWriter to capture the output
    let rpcTrace = new StringWriter()

    try

    let struct (clientStream, _serverStream) = FullDuplexStream.CreatePair()

    use formatter = new JsonMessageFormatter()

    use messageHandler = new HeaderDelimitedMessageHandler(clientStream, clientStream, formatter)

    use jsonRpc = new JsonRpc(messageHandler)
    

    // Create a new TraceListener with the StringWriter
    let listener = new TextWriterTraceListener(rpcTrace)

    // Add the listener to the JsonRpc TraceSource
    jsonRpc.TraceSource.Listeners.Add(listener) |> ignore

    // Set the TraceLevel to Information to get all informational, warning and error messages
    jsonRpc.TraceSource.Switch.Level <- SourceLevels.Information

    //jsonRpc.inv

    // Now all JsonRpc debug information will be written to the StringWriter

    let log = ResizeArray()

    let _s = new FSharpLanguageServer(jsonRpc, (LspLogger log.Add))

    jsonRpc.StartListening()

    //let initializeParams = InitializeParams(
    //    ProcessId = System.Diagnostics.Process.GetCurrentProcess().Id,
    //    RootUri = Uri("file:///c:/temp"),
    //    InitializationOptions = None,
    //    RootPath = "file:///c:/temp")

    
    

    finally

        let _output = rpcTrace.ToString()
        ()