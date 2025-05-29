namespace FSharp.Compiler.LanguageServer

open Microsoft.CommonLanguageServerProtocol.Framework
open FSharp.Compiler.LanguageServer.Common
open FSharp.Compiler.LanguageServer.Handlers

type FSharpLanguageServer() =
    interface ILanguageServer with
        member _.InitializeAsync() = System.Threading.Tasks.Task.CompletedTask
        member _.ShutdownAsync() = System.Threading.Tasks.Task.CompletedTask