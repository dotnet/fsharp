// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.Interactive.DependencyManager

open System
open System.Collections.Generic
open System.IO
open System.Reflection
open System.Runtime.InteropServices
open Internal.Utilities.FSharpEnvironment

/// Signature for Native library resolution probe callback
/// host implements this, it's job is to return a list of package roots to probe.
type NativeResolutionProbe = delegate of Unit -> seq<string>

#if NETSTANDARD
open System.Runtime.Loader

// Cut down AssemblyLoadContext, for loading native libraries
type NativeAssemblyLoadContext () =
    inherit AssemblyLoadContext()

    member this.LoadNativeLibrary(path: string): IntPtr =
        base.LoadUnmanagedDllFromPath(path)

    override _.Load(_path: AssemblyName): Assembly =
        raise (NotImplementedException())

    static member NativeLoadContext = new NativeAssemblyLoadContext()


/// Type that encapsulates Native library probing for managed packages
type NativeDllResolveHandlerCoreClr (_nativeProbingRoots: NativeResolutionProbe) =
    let probingFileNames (name: string) =
        // coreclr native library probing algorithm: https://github.com/dotnet/coreclr/blob/9773db1e7b1acb3ec75c9cc0e36bd62dcbacd6d5/src/System.Private.CoreLib/shared/System/Runtime/Loader/LibraryNameVariation.Unix.cs
        let isRooted = Path.IsPathRooted name
        let useSuffix s = not (name.Contains(s + ".") || name.EndsWith(s))          // linux devs often append version # to libraries I.e mydll.so.5.3.2
        let usePrefix = name.IndexOf(Path.DirectorySeparatorChar) = -1              // If name has directory information no add no prefix
                        && name.IndexOf(Path.AltDirectorySeparatorChar) = -1
                        && name.IndexOf(Path.PathSeparator) = -1
                        && name.IndexOf(Path.VolumeSeparatorChar) = -1
        let prefix = [| "lib" |]
        let suffix = [|
                if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) then
                    ".dll"
                    ".exe"
                elif RuntimeInformation.IsOSPlatform(OSPlatform.OSX) then
                    ".dylib"
                else
                    ".so"
            |]

        [|
            yield name                                                                              // Bare name
            if not (isRooted) then
                for s in suffix do
                    if useSuffix s then                                                             // Suffix without prefix
                        yield (sprintf "%s%s" name s)
                        if usePrefix then
                            for p in prefix do                                                      // Suffix with prefix
                                yield (sprintf "%s%s%s" p name s)
                    elif usePrefix then
                        for p in prefix do                                                          // Prefix
                            yield (sprintf "%s%s" p name)
        |]

    // Computer valid dotnet-rids for this environment:
    //      https://docs.microsoft.com/en-us/dotnet/core/rid-catalog
    //
    // Where rid is: win, win-x64, win-x86, osx-x64, linux-x64 etc ...
    let probingRids =
        let processArchitecture = RuntimeInformation.ProcessArchitecture
        let baseRid =
            if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) then "win"
            elif RuntimeInformation.IsOSPlatform(OSPlatform.OSX) then "osx"
            else "linux"
        let platformRid =
            match processArchitecture with
            | Architecture.X64 ->  baseRid + "-x64"
            | Architecture.X86 -> baseRid + "-x86"
            | Architecture.Arm64 -> baseRid + "-arm64"
            | _ -> baseRid + "arm"
        [| "any"; baseRid; platformRid |]

    let _resolveUnmanagedDll (_: Assembly) (name: string): IntPtr =

        // Enumerate probing roots looking for a dll that matches the probing name in the probed locations
        let probeForNativeLibrary root rid name =
            // Look for name in root
            probingFileNames name |> Array.tryPick(fun name ->
                let path = Path.Combine(root, "runtimes", rid, "native", name)
                if File.Exists(path) then
                    Some path
                else
                    None)

        let probe =
            match _nativeProbingRoots with
            | null -> None
            | _ ->  _nativeProbingRoots.Invoke()
                    |> Seq.tryPick(fun root ->
                    probingFileNames name |> Seq.tryPick(fun name ->
                        let path = Path.Combine(root, name)
                        if File.Exists(path) then
                            Some path
                        else
                            probingRids |> Seq.tryPick(fun rid -> probeForNativeLibrary root rid name)))

        match probe with
        | Some path -> NativeAssemblyLoadContext.NativeLoadContext.LoadNativeLibrary(path)
        | None -> IntPtr.Zero

    // netstandard 2.1 has this property, unfortunately we don't build with that yet
    //public event Func<Assembly, string, IntPtr> ResolvingUnmanagedDll
    let eventInfo = typeof<AssemblyLoadContext>.GetEvent("ResolvingUnmanagedDll")
    let handler = Func<Assembly, string, IntPtr> (_resolveUnmanagedDll)

    do if not (isNull eventInfo) then eventInfo.AddEventHandler(AssemblyLoadContext.Default, handler)

    interface IDisposable with
        member _x.Dispose() =
            if not (isNull eventInfo) then
                eventInfo.RemoveEventHandler(AssemblyLoadContext.Default, handler)
            ()

#endif

type NativeDllResolveHandler (_nativeProbingRoots: NativeResolutionProbe) =

    let handler:IDisposable option =
#if NETSTANDARD
        if isRunningOnCoreClr then
            Some (new NativeDllResolveHandlerCoreClr(_nativeProbingRoots) :> IDisposable)
        else
#endif
            None

    interface IDisposable with
        member _.Dispose() =
            match handler with
            | None -> ()
            | Some handler -> handler.Dispose()
