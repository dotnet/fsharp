﻿module FSharp.Compiler.LanguageServer.Executable

open System
open StreamJsonRpc

[<EntryPoint>]
let main _argv =

    let jsonRpc = new JsonRpc(Console.OpenStandardOutput(), Console.OpenStandardInput())

    let _s = new FSharpLanguageServer(jsonRpc, (LspLogger Console.Out.Write))

    jsonRpc.StartListening()

    async {
        while true do
            do! Async.Sleep 1000
    }
    |> Async.RunSynchronously

    0
