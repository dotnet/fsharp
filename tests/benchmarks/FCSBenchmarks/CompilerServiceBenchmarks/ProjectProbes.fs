// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Benchmarks

open System
open System.Diagnostics
open System.IO
open System.Reflection

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Diagnostics

[<AutoOpen>]
module internal ProjectProbeHelpers =

    // Captured compiler binaries can give this internal method different IL visibility.
    let clearReaderCache =
        typeof<FSharpChecker>.Assembly
            .GetType("FSharp.Compiler.AbstractIL.ILBinaryReader", throwOnError = true)
            .GetMethod("ClearAllILModuleReaderCache", BindingFlags.Static ||| BindingFlags.Public ||| BindingFlags.NonPublic)
            .CreateDelegate<Action>()

    let forceGC () =
        GC.Collect(2, GCCollectionMode.Forced, blocking = true)
        GC.WaitForPendingFinalizers()
        GC.Collect(2, GCCollectionMode.Forced, blocking = true)

    let readProjectOptions responseFile projectDir =
        let lines = File.ReadAllLines responseFile |> Array.filter (fun l -> l.Trim().Length > 0)
        let sources =
            lines
            |> Array.filter (fun l ->
                not (l.StartsWith("-", StringComparison.Ordinal))
                && (l.EndsWith(".fs", StringComparison.Ordinal) || l.EndsWith(".fsi", StringComparison.Ordinal)))
        { ProjectFileName = Path.Combine(projectDir, "FSharp.Common.fsproj")
          ProjectId = None
          SourceFiles = sources
          OtherOptions =
            lines
            |> Array.filter (fun l ->
                l <> "fsc.dll" && not (l.StartsWith("-o:", StringComparison.Ordinal)) && not (Array.contains l sources))
          ReferencedProjects = [||]
          IsIncompleteTypeCheckEnvironment = false
          UseScriptResolutionRules = false
          LoadTime = DateTime(2020, 1, 1)
          UnresolvedReferences = None
          OriginalLoadReferences = []
          Stamp = None }

module CheckProjectProbe =

    let run iterations responseFile projectDir =
        if iterations <= 0 then invalidArg (nameof iterations) "The sample count must be positive."
        Environment.CurrentDirectory <- projectDir
        let options = readProjectOptions responseFile projectDir
        clearReaderCache.Invoke()
        let checker = FSharpChecker.Create(projectCacheSize = 1, keepAssemblyContents = true)
        forceGC ()
        let baseHeap = GC.GetTotalMemory true
        printfn $"serverGC={System.Runtime.GCSettings.IsServerGC}"
        for i in 1..iterations do
            if i > 1 then checker.InvalidateConfiguration options
            forceGC ()
            let before = GC.GetTotalAllocatedBytes true
            let sw = Stopwatch.StartNew()
            let results = checker.ParseAndCheckProject options |> Async.RunSynchronously
            sw.Stop()
            let allocated = GC.GetTotalAllocatedBytes true - before
            let errors = results.Diagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
            forceGC ()
            let retained = GC.GetTotalMemory true - baseHeap
            GC.KeepAlive results
            printfn "sample %d: %.3f ms | allocated %d bytes | retained %d bytes | %d errors"
                i sw.Elapsed.TotalMilliseconds allocated retained errors.Length
            if errors.Length > 0 then failwith $"Project check failed: %A{errors}"
        GC.KeepAlive checker
