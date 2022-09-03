// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Diagnostics

open System
open System.Diagnostics
open System.Diagnostics.Tracing

module Activity =
    
    type ActivityFacade(activity : Activity option) =
        member this.AddTag key (value : #obj) = match activity with | Some activity -> activity.AddTag(key, value) |> ignore | None -> ()
        member this.Perform action = match activity with | Some activity -> action activity | None -> ()
        member this.Dispose() = match activity with | Some activity -> activity.Dispose() | None -> ()
        interface IDisposable with
            member this.Dispose() = this.Dispose()

    let start (source : ActivitySource) (activityName : string) (tags : (string * #obj) seq) =
        let activity = source.StartActivity(activityName) |> Option.ofObj
        let facade = new ActivityFacade(activity)
        for key, value in tags do
            facade.AddTag key value
        facade
        
    let startNoTags (source : ActivitySource) (activityName : string) = start source activityName []
    
    type ActivitySourceFacade(source : ActivitySource) =
        member this.Start (name : string) (tags : (string * #obj) seq) = start source name tags
        member this.StartNoTags name = startNoTags source name
        member this.Name = source.Name
        member this.Dispose() = source.Dispose()
        interface IDisposable with
            member this.Dispose() = this.Dispose()
    
    let private activitySourceName = "fsc"
    let private activitySource = new ActivitySource(activitySourceName)
    let instance = new ActivitySourceFacade(activitySource)

type LogCompilerFunctionId =
    | Service_ParseAndCheckFileInProject = 1
    | Service_CheckOneFile = 2
    | Service_IncrementalBuildersCache_BuildingNewCache = 3
    | Service_IncrementalBuildersCache_GettingCache = 4
    | CompileOps_TypeCheckOneInputAndFinishEventually = 5
    | IncrementalBuild_CreateItemKeyStoreAndSemanticClassification = 6
    | IncrementalBuild_TypeCheck = 7

/// This is for ETW tracing across FSharp.Compiler.
[<Sealed; EventSource(Name = "FSharpCompiler")>]
type FSharpCompilerEventSource() =
    inherit EventSource()

    static let instance = new FSharpCompilerEventSource()
    static member Instance = instance

    [<Event(1)>]
    member this.Log(functionId: LogCompilerFunctionId) =
        if this.IsEnabled() then this.WriteEvent(1, int functionId)

    [<Event(2)>]
    member this.LogMessage(message: string, functionId: LogCompilerFunctionId) =
        if this.IsEnabled() then
            this.WriteEvent(2, message, int functionId)

    [<Event(3)>]
    member this.BlockStart(functionId: LogCompilerFunctionId) =
        if this.IsEnabled() then this.WriteEvent(3, int functionId)

    [<Event(4)>]
    member this.BlockStop(functionId: LogCompilerFunctionId) =
        if this.IsEnabled() then this.WriteEvent(4, int functionId)

    [<Event(5)>]
    member this.BlockMessageStart(message: string, functionId: LogCompilerFunctionId) =
        if this.IsEnabled() then
            this.WriteEvent(5, message, int functionId)

    [<Event(6)>]
    member this.BlockMessageStop(message: string, functionId: LogCompilerFunctionId) =
        if this.IsEnabled() then
            this.WriteEvent(6, message, int functionId)

[<RequireQualifiedAccess>]
module Logger =

    let Log functionId =
        FSharpCompilerEventSource.Instance.Log(functionId)

    let LogMessage message functionId =
        FSharpCompilerEventSource.Instance.LogMessage(message, functionId)

    let LogBlockStart functionId =
        FSharpCompilerEventSource.Instance.BlockStart(functionId)

    let LogBlockStop functionId =
        FSharpCompilerEventSource.Instance.BlockStop(functionId)

    let LogBlockMessageStart message functionId =
        FSharpCompilerEventSource.Instance.BlockMessageStart(message, functionId)

    let LogBlockMessageStop message functionId =
        FSharpCompilerEventSource.Instance.BlockMessageStop(message, functionId)

    let LogBlock functionId =
        FSharpCompilerEventSource.Instance.BlockStart(functionId)

        { new IDisposable with
            member _.Dispose() =
                FSharpCompilerEventSource.Instance.BlockStop(functionId)
        }

    let LogBlockMessage message functionId =
        FSharpCompilerEventSource.Instance.BlockMessageStart(message, functionId)

        { new IDisposable with
            member _.Dispose() =
                FSharpCompilerEventSource.Instance.BlockMessageStop(message, functionId)
        }
