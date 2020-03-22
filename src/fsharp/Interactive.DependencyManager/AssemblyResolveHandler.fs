// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.Interactive.DependencyManager

open System
open System.Collections.Generic
open System.IO
open System.Reflection
open Internal.Utilities.FSharpEnvironment

/// Signature for ResolutionProbe callback
/// host implements this, it's job is to return a list of assembly paths to probe.
type AssemblyResolutionProbe = delegate of Unit -> IEnumerable<string>

#if NETSTANDARD

open System.Runtime.Loader

/// Type that encapsulates AssemblyResolveHandler for managed packages
type AssemblyResolveHandlerCoreclr (assemblyProbingPaths: AssemblyResolutionProbe) =

    let resolveAssemblyNetStandard (ctxt: AssemblyLoadContext) (assemblyName: AssemblyName): Assembly =

        let loadAssembly path =
            ctxt.LoadFromAssemblyPath(path)

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
            | Some path ->
                loadAssembly path
            | None -> Unchecked.defaultof<Assembly>

        with | _ -> Unchecked.defaultof<Assembly>

    let handler = Func<AssemblyLoadContext, AssemblyName, Assembly>(resolveAssemblyNetStandard)
    do AssemblyLoadContext.Default.add_Resolving(handler)

    interface IDisposable with
        member _x.Dispose() =
            AssemblyLoadContext.Default.remove_Resolving(handler)

#endif

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
            | Some path ->
                loadAssembly path
            | None -> Unchecked.defaultof<Assembly>

        with | _ -> Unchecked.defaultof<Assembly>

    let handler = new ResolveEventHandler(fun _ (args: ResolveEventArgs) -> resolveAssemblyNET (new AssemblyName(args.Name)))
    do AppDomain.CurrentDomain.add_AssemblyResolve(handler)

    interface IDisposable with
        member _x.Dispose() =
            AppDomain.CurrentDomain.remove_AssemblyResolve(handler)

type AssemblyResolveHandler (assemblyProbingPaths: AssemblyResolutionProbe) =

    let handler =
#if NETSTANDARD
        if isRunningOnCoreClr then
            new AssemblyResolveHandlerCoreclr(assemblyProbingPaths) :> IDisposable
        else
#endif
            new AssemblyResolveHandlerDeskTop(assemblyProbingPaths) :> IDisposable

    interface IDisposable with
        member _.Dispose() = handler.Dispose()
