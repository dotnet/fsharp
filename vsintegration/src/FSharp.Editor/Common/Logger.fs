// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Diagnostics.Tracing

type LogEditorFunctionId =
    | Classification_Semantic = 1
    | Classification_Syntactic = 2
    | LanguageService_HandleCommandLineArgs = 3
    | LanguageService_UpdateProjectInfo = 4
    | Completion_ShouldTrigger = 5
    | Completion_ProvideCompletionsAsync = 6
    | Completion_GetDescriptionAsync = 7
    | Completion_GetChangeAsync = 9

/// This is for ETW tracing across FSharp.Editor.
[<Sealed; EventSource(Name = "FSharpEditor")>]
type FSharpEditorEventSource() =
    inherit EventSource()

    static let instance = new FSharpEditorEventSource()
    static member Instance = instance

    [<Event(1)>]
    member this.Log(functionId: LogEditorFunctionId) =
        if this.IsEnabled() then
            this.WriteEvent(1, int functionId)

    [<Event(2)>]
    member this.LogMessage(message: string, functionId: LogEditorFunctionId) =
        if this.IsEnabled() then
            this.WriteEvent(2, message, int functionId)

    [<Event(3)>]
    member this.BlockStart(functionId: LogEditorFunctionId) =
        if this.IsEnabled() then
            this.WriteEvent(3, int functionId)

    [<Event(4)>]
    member this.BlockStop(functionId: LogEditorFunctionId) =
        if this.IsEnabled() then
            this.WriteEvent(4, int functionId)

    [<Event(5)>]
    member this.BlockMessageStart(message: string, functionId: LogEditorFunctionId) =
        if this.IsEnabled() then
            this.WriteEvent(5, message, int functionId)

    [<Event(6)>]
    member this.BlockMessageStop(message: string, functionId: LogEditorFunctionId) =
        if this.IsEnabled() then
            this.WriteEvent(6, message, int functionId)

[<RequireQualifiedAccess>]
module Logger =

    let Log (functionId) =
        FSharpEditorEventSource.Instance.Log(functionId)

    let LogMessage message functionId =
        FSharpEditorEventSource.Instance.LogMessage(message, functionId)

    let LogBlockStart (functionId) =
        FSharpEditorEventSource.Instance.BlockStart(functionId)

    let LogBlockStop (functionId) =
        FSharpEditorEventSource.Instance.BlockStop(functionId)

    let LogBlockMessageStart message functionId =
        FSharpEditorEventSource.Instance.BlockMessageStart(message, functionId)

    let LogBlockMessageStop message functionId =
        FSharpEditorEventSource.Instance.BlockMessageStop(message, functionId)

    let LogBlock (functionId) =
        FSharpEditorEventSource.Instance.BlockStart(functionId)

        { new IDisposable with
            member _.Dispose() =
                FSharpEditorEventSource.Instance.BlockStop(functionId)
        }

    let LogBlockMessage message functionId =
        FSharpEditorEventSource.Instance.BlockMessageStart(message, functionId)

        { new IDisposable with
            member _.Dispose() =
                FSharpEditorEventSource.Instance.BlockMessageStop(message, functionId)
        }
