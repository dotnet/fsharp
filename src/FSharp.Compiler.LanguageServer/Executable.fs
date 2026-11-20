module FSharp.Compiler.LanguageServer.Executable

open System
open StreamJsonRpc

[<EntryPoint>]
let main _argv =

    let formatter = createConfiguredFormatter ()

    let messageHandler =
        new HeaderDelimitedMessageHandler(Console.OpenStandardOutput(), Console.OpenStandardInput(), formatter)

    let jsonRpc = new JsonRpc(messageHandler)

    let server =
        new FSharpLanguageServer(jsonRpc, formatter.JsonSerializerOptions, (LspLogger Console.Error.Write))

    jsonRpc.StartListening()

    server.WaitForExitAsync().GetAwaiter().GetResult()

    0
