// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.LanguageServer

open System

module Program =

    [<EntryPoint>]
    let main(args: string[]) =
        async {
            let server = new Server(Console.OpenStandardOutput(), Console.OpenStandardInput())
            server.StartListening()
            do! server.WaitForExitAsync()
            return 0
        } |> Async.RunSynchronously
