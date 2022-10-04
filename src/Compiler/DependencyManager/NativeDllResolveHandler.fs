// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.DependencyManager

open System
open System.Collections.Concurrent
open System.IO
open System.Reflection
open System.Runtime.InteropServices
open Internal.Utilities
open Internal.Utilities.FSharpEnvironment
open FSharp.Compiler.IO


type internal ProbingPathsStore() =

    let addedPaths = ConcurrentBag<string>()

    static member AppendPathSeparator (p: string) =
        let separator = string Path.PathSeparator

        if not (p.EndsWith(separator, StringComparison.OrdinalIgnoreCase)) then
            p + separator
        else
            p

    static member RemoveProbeFromProcessPath probePath =
        if not (String.IsNullOrWhiteSpace(probePath)) then
            let probe = ProbingPathsStore.AppendPathSeparator probePath
            let path = ProbingPathsStore.AppendPathSeparator (Environment.GetEnvironmentVariable("PATH"))

            if path.Contains(probe) then
                Environment.SetEnvironmentVariable("PATH", path.Replace(probe, ""))

    member _.AddProbeToProcessPath probePath =
        let probe = ProbingPathsStore.AppendPathSeparator probePath
        let path = ProbingPathsStore.AppendPathSeparator (Environment.GetEnvironmentVariable("PATH"))

        if not (path.Contains(probe)) then
            Environment.SetEnvironmentVariable("PATH", path + probe)
            addedPaths.Add probe

    member this.RefreshPathsInEnvironment(roots) =
        for probePath in roots do
            this.AddProbeToProcessPath(probePath)

    member this.Dispose() =
        let mutable probe: string = Unchecked.defaultof<string>
        while (addedPaths.TryTake(&probe)) do
            ProbingPathsStore.RemoveProbeFromProcessPath(probe)

    interface IDisposable with
        member _.Dispose() =
            let mutable probe: string = Unchecked.defaultof<string>
            while (addedPaths.TryTake(&probe)) do
                ProbingPathsStore.RemoveProbeFromProcessPath(probe)

/// Signature for Native library resolution probe callback
/// host implements this, it's job is to return a list of package roots to probe.
type NativeResolutionProbe = delegate of Unit -> seq<string>

/// Type that encapsulates Native library probing for managed packages
type internal NativeDllResolveHandlerCoreClr(nativeProbingRoots: NativeResolutionProbe option) =

    let probingPaths = new ProbingPathsStore()

    let nativeLibraryTryLoad =
        let nativeLibraryType: Type =
            Type.GetType("System.Runtime.InteropServices.NativeLibrary, System.Runtime.InteropServices", false)

        nativeLibraryType.GetMethod("TryLoad", [| typeof<string>; typeof<IntPtr>.MakeByRefType () |])

    let loadNativeLibrary path =
        let arguments = [| path :> obj; IntPtr.Zero :> obj |]

        if nativeLibraryTryLoad.Invoke(null, arguments) :?> bool then
            arguments[1] :?> IntPtr
        else
            IntPtr.Zero

    let probingFileNames (name: string) =
        // coreclr native library probing algorithm: https://github.com/dotnet/coreclr/blob/9773db1e7b1acb3ec75c9cc0e36bd62dcbacd6d5/src/System.Private.CoreLib/shared/System/Runtime/Loader/LibraryNameVariation.Unix.cs
        let isRooted = Path.IsPathRooted name

        let useSuffix s =
            not (name.Contains(s + ".") || name.EndsWith(s)) // linux devs often append version # to libraries I.e mydll.so.5.3.2

        let usePrefix =
            name.IndexOf(Path.DirectorySeparatorChar) = -1 // If name has directory information no add no prefix
            && name.IndexOf(Path.AltDirectorySeparatorChar) = -1
            && name.IndexOf(Path.PathSeparator) = -1
            && name.IndexOf(Path.VolumeSeparatorChar) = -1

        let prefix = [| "lib" |]

        let suffix =
            [|
                if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) then
                    ".dll"
                    ".exe"
                elif RuntimeInformation.IsOSPlatform(OSPlatform.OSX) then
                    ".dylib"
                else
                    ".so"
            |]

        [|
            yield name // Bare name
            if not isRooted then
                for s in suffix do
                    if useSuffix s then // Suffix without prefix
                        yield (sprintf "%s%s" name s)

                        if usePrefix then
                            for p in prefix do // Suffix with prefix
                                yield (sprintf "%s%s%s" p name s)
                    elif usePrefix then
                        for p in prefix do // Prefix
                            yield (sprintf "%s%s" p name)
        |]

    let resolveUnmanagedDll (_: Assembly) (name: string) : IntPtr =
        // Enumerate probing roots looking for a dll that matches the probing name in the probed locations
        let probeForNativeLibrary root rid name =
            // Look for name in root
            probingFileNames name
            |> Array.tryPick (fun name ->
                let path = Path.Combine(root, "runtimes", rid, "native", name)
                if FileSystem.FileExistsShim(path) then Some path else None)

        let probe =
            match nativeProbingRoots with
            | None -> None
            | Some nativeProbingRoots ->
                nativeProbingRoots.Invoke()
                |> Seq.tryPick (fun root ->
                    probingFileNames name
                    |> Seq.tryPick (fun name ->
                        let path = Path.Combine(root, name)

                        if FileSystem.FileExistsShim(path) then
                            Some path
                        else
                            RidHelpers.probingRids
                            |> Seq.tryPick (fun rid -> probeForNativeLibrary root rid name)))

        match probe with
        | Some path -> loadNativeLibrary (path)
        | None -> IntPtr.Zero

    // netstandard 2.1 has this property, unfortunately we don't build with that yet
    //public event Func<Assembly, string, IntPtr> ResolvingUnmanagedDll
    let assemblyLoadContextType: Type =
        Type.GetType("System.Runtime.Loader.AssemblyLoadContext, System.Runtime.Loader", false)

    let eventInfo, handler, defaultAssemblyLoadContext =
        assemblyLoadContextType.GetEvent("ResolvingUnmanagedDll"),
        Func<Assembly, string, IntPtr> resolveUnmanagedDll,
        assemblyLoadContextType
            .GetProperty("Default", BindingFlags.Static ||| BindingFlags.Public)
            .GetValue(null, null)

    do eventInfo.AddEventHandler(defaultAssemblyLoadContext, handler)

    member _.RefreshPathsInEnvironment(roots: string seq) =
        probingPaths.RefreshPathsInEnvironment(roots)

    member _.Dispose() =
        eventInfo.RemoveEventHandler(defaultAssemblyLoadContext, handler)
        probingPaths.Dispose()

    interface IDisposable with
        member this.Dispose() = this.Dispose()

type NativeDllResolveHandler(nativeProbingRoots: NativeResolutionProbe option) =

    let handler: NativeDllResolveHandlerCoreClr option =
        nativeProbingRoots
        |> Option.filter(fun _ -> isRunningOnCoreClr)
        |> Option.map (fun _ -> new NativeDllResolveHandlerCoreClr(nativeProbingRoots))

    new(nativeProbingRoots: NativeResolutionProbe) = new NativeDllResolveHandler(Option.ofObj nativeProbingRoots)

    member internal _.RefreshPathsInEnvironment(roots: string seq) =
        handler |> Option.iter (fun handler -> handler.RefreshPathsInEnvironment(roots))

    interface IDisposable with
        member _.Dispose() =
            handler |> Option.iter (fun handler -> handler.Dispose())
