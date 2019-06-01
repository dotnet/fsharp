// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.LanguageServer.UnitTests

open System.Diagnostics
open System.Threading.Tasks
open FSharp.Compiler.LanguageServer
open NUnit.Framework
open StreamJsonRpc

[<TestFixture>]
type ProtocolTests() =

#if !NETCOREAPP
    // The `netcoreapp2.1` version of `FSharp.Compiler.LanguageServer.exe` can't be run without a `publish` step so
    // we're artificially restricting this test to the full framework.
    [<Test>]
#endif
    member __.``Server consuming stdin and stdout``() =
        async {
            // start server as a console app
            let serverAssemblyPath = typeof<Server>.Assembly.Location
            let startInfo = ProcessStartInfo(serverAssemblyPath)
            startInfo.UseShellExecute <- false
            startInfo.RedirectStandardInput <- true
            startInfo.RedirectStandardOutput <- true
            let proc = Process.Start(startInfo)

            // create a fake client
            let client = new JsonRpc(proc.StandardInput.BaseStream, proc.StandardOutput.BaseStream)
            client.StartListening()

            // initialize
            let! capabilitites = client.InvokeAsync<ServerCapabilities>("initialize", "") |> Async.AwaitTask
            Assert.True(capabilitites.hoverProvider)

            // shutdown the server
            do! client.NotifyAsync("shutdown") |> Async.AwaitTask
            if not (proc.WaitForExit(5000)) then failwith "Expected server process to exit."
        } |> Async.StartAsTask :> Task
