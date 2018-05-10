// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Compiler

open System.Diagnostics.Tracing
open System

type LogCompilerFunctionId =
    | ParseAndCheckFileInProject = 1
    
/// This is for ETW tracing across FSharp.Compiler.
[<Sealed;EventSource(Name = "FSharpCompiler")>]
type FSharpCompilerEventSource() =
    inherit EventSource()

    static let instance = new FSharpCompilerEventSource()
    static member Instance = instance

    [<Event(1)>]
    member this.Log(functionId: LogCompilerFunctionId) =
        if this.IsEnabled() then
            this.WriteEvent(1, int functionId)

    [<Event(2)>]
    member this.BlockStart(functionId: LogCompilerFunctionId) =
        if this.IsEnabled() then
            this.WriteEvent(2, int functionId)

    [<Event(3)>]
    member this.BlockStop(functionId: LogCompilerFunctionId) =
        if this.IsEnabled() then
            this.WriteEvent(3, int functionId)

    [<Event(4)>]
    member this.BlockMessageStart(message: string, functionId: LogCompilerFunctionId) =
        if this.IsEnabled() then
            this.WriteEvent(4, message, int functionId)

    [<Event(5)>]
    member this.BlockMessageStop(message: string, functionId: LogCompilerFunctionId) =
        if this.IsEnabled() then
            this.WriteEvent(5, message, int functionId)

[<RequireQualifiedAccess>]
module Logger =

    let Log(functionId) = FSharpCompilerEventSource.Instance.Log(functionId)

    let LogBlockStart(functionId) = FSharpCompilerEventSource.Instance.BlockStart(functionId)

    let LogBlockStop(functionId) = FSharpCompilerEventSource.Instance.BlockStop(functionId)

    let LogBlock(functionId) =
        FSharpCompilerEventSource.Instance.BlockStart(functionId)
        { new IDisposable with
            member __.Dispose() =
                FSharpCompilerEventSource.Instance.BlockStop(functionId) }

    let LogBlockMessage message functionId =
        FSharpCompilerEventSource.Instance.BlockMessageStart(message, functionId)
        { new IDisposable with
            member __.Dispose() =
                FSharpCompilerEventSource.Instance.BlockMessageStop(message, functionId) }
