// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Interactive.DependencyManager

open System
open System.Collections.Generic
open System.IO
open System.Reflection

#if NETSTANDARD
open System.Runtime.Loader
#endif

type AssemblyResolutionProbe = delegate of Unit -> IEnumerable<string>

type AssemblyResolveHandler  (assemblyProbingPaths: AssemblyResolutionProbe) =

#if NETSTANDARD
    let resolveAssemblyNetStandard (ctxt: AssemblyLoadContext) (assemblyName: AssemblyName): Assembly =

        let loadAssembly path =
            ctxt.LoadFromAssemblyPath(path)

#else
    let resolveAssemblyNET (assemblyName: AssemblyName): Assembly =

        let loadAssembly assemblyPath =
            Assembly.LoadFrom(assemblyPath)

#endif

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

#if NETSTANDARD
    let handler = Func<AssemblyLoadContext, AssemblyName, Assembly>(resolveAssemblyNetStandard)
    do AssemblyLoadContext.Default.add_Resolving(handler)

    interface IDisposable with
        member _x.Dispose() =
            AssemblyLoadContext.Default.remove_Resolving(handler)

#else
    let handler = new ResolveEventHandler(fun _ (args: ResolveEventArgs) -> resolveAssemblyNET (new AssemblyName(args.Name)))
    do AppDomain.CurrentDomain.add_AssemblyResolve(handler)

    interface IDisposable with
        member _x.Dispose() =
            AppDomain.CurrentDomain.remove_AssemblyResolve(handler)

#endif
