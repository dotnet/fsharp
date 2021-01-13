// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.DotNet.DependencyManager

open System
open System.IO
open System.Reflection
open Internal.Utilities.FSharpEnvironment

/// Signature for ResolutionProbe callback
/// host implements this, it's job is to return a list of assembly paths to probe.
type AssemblyResolutionProbe = delegate of Unit -> seq<string>

open System.Runtime.Loader

/// Type that encapsulates AssemblyResolveHandler for managed packages
type AssemblyResolveHandlerCoreclr (assemblyProbingPaths: AssemblyResolutionProbe, nativeProbingRoots: NativeResolutionProbe) as this =

    let nativeHandler =
        new NativeDllResolveHandler(nativeProbingRoots) :> IRegisterResolvers

    let assemblyLoadContextType: Type = Type.GetType("System.Runtime.Loader.AssemblyLoadContext, System.Runtime.Loader", false)

    let loadFromAssemblyPathMethod =
        assemblyLoadContextType.GetMethod("LoadFromAssemblyPath", [| typeof<string> |])

    let eventInfo, handler, defaultAssemblyLoadContext =
        let eventInfo = assemblyLoadContextType.GetEvent("Resolving")
        let mi =
            let gmi = this.GetType().GetMethod("ResolveAssemblyNetStandard", BindingFlags.Instance ||| BindingFlags.NonPublic)
            gmi.MakeGenericMethod(assemblyLoadContextType)

        eventInfo,
        Delegate.CreateDelegate(eventInfo.EventHandlerType, this, mi),
        assemblyLoadContextType.GetProperty("Default", BindingFlags.Static ||| BindingFlags.Public).GetValue(null, null)

    do eventInfo.AddEventHandler(defaultAssemblyLoadContext, handler)

    member this.ResolveAssemblyNetStandard (ctxt: 'T) (assemblyName: AssemblyName): Assembly =
        let loadAssembly path =
            let assembly = loadFromAssemblyPathMethod.Invoke(ctxt, [| path |]) :?> Assembly
            (this :> IRegisterResolvers).RegisterAssemblyNativeResolvers(assembly)
            assembly

        let assemblyPaths =
            match assemblyProbingPaths with
            | null -> Seq.empty<string>
            | _ ->  assemblyProbingPaths.Invoke()

        try
            // args.Name is a displayname formatted assembly version.
            // E.g:  "System.IO.FileSystem, Version=4.1.1.0, Culture=en-US, PublicKeyToken=b03f5f7f11d50a3a"
            let simpleName = assemblyName.Name
            let assemblyPathOpt = assemblyPaths |> Seq.tryFind(fun path -> Path.GetFileNameWithoutExtension(path) = simpleName)
            match assemblyPathOpt with
            | Some path -> loadAssembly path
            | None -> Unchecked.defaultof<Assembly>

        with | _ -> Unchecked.defaultof<Assembly>

    interface IRegisterResolvers with
        member _.RegisterAssemblyNativeResolvers(assembly: Assembly) =
            nativeHandler.RegisterAssemblyNativeResolvers(assembly)

        member _.RegisterPackageRoots(roots: string seq) =
            nativeHandler.RegisterPackageRoots(roots)

    interface IDisposable with
        member _.Dispose() =
            nativeHandler.Dispose()
            eventInfo.RemoveEventHandler(defaultAssemblyLoadContext, handler)

/// Type that encapsulates AssemblyResolveHandler for managed packages
type AssemblyResolveHandlerDeskTop (assemblyProbingPaths: AssemblyResolutionProbe) =

    let resolveAssemblyNET (assemblyName: AssemblyName): Assembly =

        let loadAssembly assemblyPath =
            Assembly.LoadFrom(assemblyPath)

        let assemblyPaths =
            match assemblyProbingPaths with
            | null -> Seq.empty<string>
            | _ ->  assemblyProbingPaths.Invoke()

        try
            // args.Name is a displayname formatted assembly version.
            // E.g:  "System.IO.FileSystem, Version=4.1.1.0, Culture=en-US, PublicKeyToken=b03f5f7f11d50a3a"
            let simpleName = assemblyName.Name
            let assemblyPathOpt = assemblyPaths |> Seq.tryFind(fun path -> Path.GetFileNameWithoutExtension(path) = simpleName)
            match assemblyPathOpt with
            | Some path -> loadAssembly path
            | None -> Unchecked.defaultof<Assembly>

        with | _ -> Unchecked.defaultof<Assembly>

    let handler = new ResolveEventHandler(fun _ (args: ResolveEventArgs) -> resolveAssemblyNET (new AssemblyName(args.Name)))
    do AppDomain.CurrentDomain.add_AssemblyResolve(handler)

    interface IRegisterResolvers with
        member _.RegisterAssemblyNativeResolvers(assembly: Assembly) =
            ignore assembly

        member _.RegisterPackageRoots(roots: string seq) =
            ignore roots

    interface IDisposable with
        member _x.Dispose() =
            AppDomain.CurrentDomain.remove_AssemblyResolve(handler)

type AssemblyResolveHandler (assemblyProbingPaths: AssemblyResolutionProbe, nativeProbingRoots: NativeResolutionProbe) =

    let handler =
        if isRunningOnCoreClr then
            (new AssemblyResolveHandlerCoreclr(assemblyProbingPaths, nativeProbingRoots)) :> IRegisterResolvers
        else
            (new AssemblyResolveHandlerDeskTop(assemblyProbingPaths)) :> IRegisterResolvers

    member this.RefreshPathsInEnvironment(roots: string seq) =
        handler.RegisterPackageRoots(roots)

    interface IDisposable with
        member _.Dispose() =
            handler.Dispose()
