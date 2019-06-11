// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor.LanguageService

open System.ComponentModel.Composition
open System.Diagnostics
open System.IO
open System.Threading
open System.Threading.Tasks
open FSharp.Compiler.LanguageServer
open Microsoft.FSharp.Control
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.Helpers
open Microsoft.VisualStudio.LanguageServer.Client
open Microsoft.VisualStudio.Utilities
open StreamJsonRpc

// https://docs.microsoft.com/en-us/visualstudio/extensibility/adding-an-lsp-extension?view=vs-2019

/// Provides exports necessary to register the language client.
type FSharpContentDefinition() =

    [<Export>]
    [<Name(FSharpConstants.FSharpLanguageName)>]
    [<BaseDefinition(CodeRemoteContentDefinition.CodeRemoteContentTypeName)>]
    static member val FSharpContentTypeDefinition: ContentTypeDefinition = null with get, set

    [<Export>]
    [<FileExtension(FSharpConstants.FSharpFileExtension)>]
    [<ContentType(FSharpConstants.FSharpLanguageName)>]
    static member val FSharpFileExtensionDefinition: FileExtensionToContentTypeDefinition = null with get, set

[<Export(typeof<ILanguageClient>)>]
[<ContentType(FSharpConstants.FSharpLanguageName)>]
type internal FSharpLanguageClient
    [<ImportingConstructor>]
    (
        lspService: LspService
    ) =
    inherit LanguageClient()
    override __.Name = "F# Language Service"
    override this.ActivateAsync(_token: CancellationToken) =
        async {
            let thisAssemblyPath = Path.GetDirectoryName(this.GetType().Assembly.Location)
            let serverAssemblyPath = Path.Combine(thisAssemblyPath, "Agent", "FSharp.Compiler.LanguageServer.exe")
            let startInfo = ProcessStartInfo(serverAssemblyPath)
            startInfo.UseShellExecute <- false
            startInfo.CreateNoWindow <- true // comment to see log messages written to stderr
            startInfo.RedirectStandardInput <- true
            startInfo.RedirectStandardOutput <- true
            let proc = new Process()
            proc.StartInfo <- startInfo
            return
                if proc.Start() then new Connection(proc.StandardOutput.BaseStream, proc.StandardInput.BaseStream)
                else null
        } |> Async.StartAsTask
    override __.ConfigurationSections = null
    override __.FilesToWatch = null
    override __.InitializationOptions = null
    override __.DoLoadAsync() = Task.CompletedTask
    override __.OnServerInitializeFailedAsync(_e: exn) = Task.CompletedTask
    override __.OnServerInitializedAsync() = Task.CompletedTask
    interface ILanguageClientCustomMessage with
        member __.CustomMessageTarget = null
        member __.MiddleLayer = null
        member __.AttachForCustomMessageAsync(rpc: JsonRpc) =
            rpc.JsonSerializer.Converters.Add(JsonOptionConverter()) // ensure we can set `'T option` values
            lspService.SetJsonRpc(rpc) |> Async.StartAsTask :> Task
