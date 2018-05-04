// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Diagnostics.Tracing

type LogEditorFunctionId =
    | SemanticClassification = 1
    | SyntacticClassification = 2
    | HandleCommandLineArgs = 3

/// This is for ETW tracing across FSharp.Editor.
[<Sealed;EventSource(Name = "FSharpEditor")>]
type FSharpEditorEventSource() =
    inherit EventSource()

    static let instance = new FSharpEditorEventSource()
    static member Instance = instance

    [<Event(1)>]
    member this.Log(functionId: LogEditorFunctionId) =
        if this.IsEnabled() then
            this.WriteEvent(1, int functionId)

    [<Event(2)>]
    member this.BlockStart(functionId: LogEditorFunctionId) =
        if this.IsEnabled() then
            this.WriteEvent(2, int functionId)

    [<Event(3)>]
    member this.BlockStop(functionId: LogEditorFunctionId) =
        if this.IsEnabled() then
            this.WriteEvent(3, int functionId)

[<RequireQualifiedAccess>]
module Logger =

    let Log(functionId) = FSharpEditorEventSource.Instance.Log(functionId)

    let LogBlockStart(functionId) = FSharpEditorEventSource.Instance.BlockStart(functionId)

    let LogBlockStop(functionId) = FSharpEditorEventSource.Instance.BlockStop(functionId)

    let LogBlock(functionId) =
        FSharpEditorEventSource.Instance.BlockStart(functionId)
        { new IDisposable with
            member __.Dispose() =
                FSharpEditorEventSource.Instance.BlockStop(functionId) }
    