// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace FSharp.Compiler.DependencyManager

open System
open System.IO
open System.Reflection
open Internal.Utilities.FSharpEnvironment

/// Signature for ResolutionProbe callback
/// host implements this, it's job is to return a list of assembly paths to probe.
type AssemblyResolutionProbe = delegate of Unit -> seq<string>

/// Type that encapsulates AssemblyResolveHandler for managed packages
type AssemblyResolveHandlerCoreclr(assemblyProbingPaths: AssemblyResolutionProbe option) as this =
    let loadContextType =
        Type.GetType("System.Runtime.Loader.AssemblyLoadContext, System.Runtime.Loader", false)

    let loadFromAssemblyPathMethod =
        loadContextType.GetMethod("LoadFromAssemblyPath", [| typeof<string> |])

    let eventInfo = loadContextType.GetEvent("Resolving")

    let handler, defaultAssemblyLoadContext =
        let ti = typeof<AssemblyResolveHandlerCoreclr>

        let gmi =
            ti.GetMethod("ResolveAssemblyNetStandard", BindingFlags.Instance ||| BindingFlags.NonPublic)

        let mi = gmi.MakeGenericMethod(loadContextType)
        let del = Delegate.CreateDelegate(eventInfo.EventHandlerType, this, mi)

        let prop =
            loadContextType
                .GetProperty("Default", BindingFlags.Static ||| BindingFlags.Public)
                .GetValue(null, null)

        del, prop

    do eventInfo.AddEventHandler(defaultAssemblyLoadContext, handler)

    member _.ResolveAssemblyNetStandard (ctxt: 'T) (assemblyName: AssemblyName) : Assembly =
        let loadAssembly path =
            loadFromAssemblyPathMethod.Invoke(ctxt, [| path |]) :?> Assembly

        let assemblyPaths =
            match assemblyProbingPaths with
            | None -> Seq.empty<string>
            | Some assemblyProbingPaths -> assemblyProbingPaths.Invoke()

        try
            // args.Name is a displayname formatted assembly version.
            // E.g:  "System.IO.FileSystem, Version=4.1.1.0, Culture=en-US, PublicKeyToken=b03f5f7f11d50a3a"
            let simpleName = assemblyName.Name

            let assemblyPathOpt =
                assemblyPaths
                |> Seq.tryFind (fun path -> Path.GetFileNameWithoutExtension(path) = simpleName)

            match assemblyPathOpt with
            | Some path -> loadAssembly path
            | None -> Unchecked.defaultof<Assembly>

        with _ ->
            Unchecked.defaultof<Assembly>

    interface IDisposable with
        member _x.Dispose() =
            eventInfo.RemoveEventHandler(defaultAssemblyLoadContext, handler)

/// Type that encapsulates AssemblyResolveHandler for managed packages
type AssemblyResolveHandlerDeskTop(assemblyProbingPaths: AssemblyResolutionProbe option) =

    let resolveAssemblyNET (assemblyName: AssemblyName) : Assembly =

        let assemblyPaths =
            match assemblyProbingPaths with
            | None -> Seq.empty<string>
            | Some assemblyProbingPaths -> assemblyProbingPaths.Invoke()

        try
            // args.Name is a displayname formatted assembly version.
            // E.g:  "System.IO.FileSystem, Version=4.1.1.0, Culture=en-US, PublicKeyToken=b03f5f7f11d50a3a"
            let simpleName = assemblyName.Name

            let assemblyPathOpt =
                assemblyPaths
                |> Seq.tryFind (fun path -> Path.GetFileNameWithoutExtension(path) = simpleName)

            match assemblyPathOpt with
            | Some path -> Assembly.LoadFrom path
            | None -> Unchecked.defaultof<Assembly>

        with _ ->
            Unchecked.defaultof<Assembly>

    let handler =
        ResolveEventHandler(fun _ (args: ResolveEventArgs) -> resolveAssemblyNET (AssemblyName(args.Name)))

    do AppDomain.CurrentDomain.add_AssemblyResolve (handler)

    interface IDisposable with
        member _x.Dispose() =
            AppDomain.CurrentDomain.remove_AssemblyResolve (handler)

type AssemblyResolveHandler internal (assemblyProbingPaths: AssemblyResolutionProbe option) =

    let handler =
        if isRunningOnCoreClr then
            new AssemblyResolveHandlerCoreclr(assemblyProbingPaths) :> IDisposable
        else
            new AssemblyResolveHandlerDeskTop(assemblyProbingPaths) :> IDisposable

    new(assemblyProbingPaths: AssemblyResolutionProbe) = new AssemblyResolveHandler(Option.ofObj assemblyProbingPaths)

    interface IDisposable with
        member _.Dispose() = handler.Dispose()
