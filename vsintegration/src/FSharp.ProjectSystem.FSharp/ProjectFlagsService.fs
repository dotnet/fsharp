// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.ProjectSystem

open System
open System.ComponentModel.Composition
open System.Collections.Generic
open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.Build.Execution

(*
[<Export(typeof<ProjectFlagsService>)>]
type internal ProjectFlagsService 
    [<ImportingConstructor>]
    (
        [<Import(typeof<SVsBuildManagerAccessor>)>] accessor : IVsBuildManagerAccessor
    ) =
*)
type internal ProjectFlagsService 
    (
        accessor : IVsBuildManagerAccessor
    ) =

    let queueLock = obj()
    let projectQueue = Stack<ProjectNode>()
    let dirtyProjects = Dictionary<ProjectNode,bool>()

    /// Insert a project into the front of the queue to be processed.
    member __.QueueProject(project : ProjectNode) =
        lock queueLock <| fun () ->
            dirtyProjects.[project] <- true
            projectQueue.Push project
    
    /// Moves a project to the front of the queue if it is already in the queue.
    member __.PrioritiseProject(project : ProjectNode) =
        lock queueLock <| fun () ->
            match dirtyProjects.TryGetValue project with
            | true, true ->
                projectQueue.Push project
            | _ -> ()
    
    member __.Clear() =
        lock queueLock <| fun () ->
            dirtyProjects.Clear()
            projectQueue.Clear()
    
    member __.TryPop() =
        lock queueLock <| fun () ->
            if projectQueue.Count > 0 then 
                let project = projectQueue.Pop()

                // Only process the project if it is dirty.
                // A project can be in the queue and clean if it was in the queue multiple times
                // and it's already been processed.
                if dirtyProjects.[project] then
                    dirtyProjects.[project] <- false
                    Some project
                else
                    None
            else
                None
    
    member this.StartProcessing() =
        let processProject (project : ProjectNode) = async {
            let completeEvent = Event<unit>()
            
            // Keep trying to grab the exclusive lock for a design time build
            while accessor.BeginDesignTimeBuild() |> ErrorHandler.Failed do
                do! Async.Sleep 2000

            // Notify the project to not invoke compilation when a call comes from Fsc. We're
            // only interested in the source files and flags that are passed from MSBuild.
            let buildResource = project.AcquireExclusiveBuildResource(actuallyBuild = false)

            // Build completion callback - called from the UI thread
            let onComplete (_result : MSBuildResult) (_instance : ProjectInstance) =
                buildResource.Dispose()
                project.NotifySourcesAndFlags()
                accessor.EndDesignTimeBuild() |> ignore
                completeEvent.Trigger()

            // Notes regarding the _ResolveReferenceDependencies property
            // TODO: check whether this can be achieved in a less painful way 
            // (names that start with '_' are treated as implementation details and not recommended
            // for usage).
            // Setting this property enforces MSBuild to resolve second order dependencies, so if
            // desktop applications references .NET Core based portable assemblies then MSBuild
            // will detect that one of references requires System.Runtime and expand list of
            // references with framework facades for portables.
            // Usually facades are located at: %programfiles(x86)%\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\Facades\
            // If the property is not set then MSBuild will resolve only primary dependencies, and
            // the compiler will be very unhappy as all fundamental types should be taken from
            // System.Runtime that is not supplied.
            
            let projectInstance = ref null
            UIThread.Run(fun () ->
                project.DoMSBuildSubmission(BuildKind.ASYNC, "Compile", projectInstance, MSBuildCoda onComplete, [KeyValuePair("_ResolveReferenceDependencies", "true")]) |> ignore
                )
            
            // Wait until the build is complete before we continue servicing the queue
            do! Async.AwaitEvent completeEvent.Publish
        }

        async {
            while true do
                match this.TryPop() with
                | None -> ()
                | Some project ->
                    do! processProject project

                // Having an exclusive lock on design-time builds means the user can't initiate
                // any builds themselves. Wait a few seconds before trying to grab one again.
                do! Async.Sleep 2000
        }
    
    member this.Initialize() =
        this.StartProcessing() |> Async.StartImmediate

        // Clear the queue when the solution is closed
        Events.SolutionEvents.OnAfterCloseSolution.Add (ignore >> this.Clear)