module FSharp.Compiler.ComponentTests.HotReload.ApplyUpdateConsole

open System
open System.Reflection
open System.Reflection.Metadata
open System.Runtime.Loader
open Xunit
open FSharp.Compiler.ComponentTests.HotReload.ApplyUpdateShared

[<Literal>]
let private DotnetModifiableAssembliesEnvVar = "DOTNET_MODIFIABLE_ASSEMBLIES"

/// Not a real test; used via `dotnet test --filter ...` as a console-style host to avoid vstest reuse.
[<Fact>]
let ``ApplyUpdate console host`` () =
    if not (String.Equals(Environment.GetEnvironmentVariable(DotnetModifiableAssembliesEnvVar), "debug", StringComparison.OrdinalIgnoreCase)) then
        failwith $"{DotnetModifiableAssembliesEnvVar} must be 'debug' for this host."

    printfn "[applyupdate-console] MetadataUpdater.IsSupported=%b" (MetadataUpdater.IsSupported)

    let updatedMessage = "Hello updated"
    let artifacts = createApplyUpdateDeltaArtifacts updatedMessage
    let baselineArtifacts = artifacts.BaselineArtifacts
    let typeName = artifacts.TypeName
    let delta = artifacts.Delta

    let alc = new AssemblyLoadContext("ApplyUpdateConsole_" + Guid.NewGuid().ToString("N"), isCollectible = true)
    let assembly = alc.LoadFromAssemblyPath baselineArtifacts.AssemblyPath
    let _, encCapable = probeApplyUpdateAssembly "applyupdate-console" baselineArtifacts.AssemblyPath assembly

    assembly.GetCustomAttributes()
    |> Seq.filter (fun a -> a.GetType().Name = "DebuggableAttribute")
    |> Seq.iter (fun a -> printfn "[applyupdate-console] Debuggable attr=%A" a)

    printfn "[applyupdate-console] IsEnCCapable=%b" encCapable

    if not encCapable then
        printfn "[applyupdate-console] Skipping ApplyUpdate: module not EnC-capable."
    else
        let sampleType = assembly.GetType(typeName, throwOnError = true)
        let method = sampleType.GetMethod("GetMessage", BindingFlags.Public ||| BindingFlags.Static)
        let before = method.Invoke(null, [||]) :?> string
        printfn "[applyupdate-console] before=%s" before

        let pdbBytes = pdbBytesOrEmpty delta.Pdb
        MetadataUpdater.ApplyUpdate(assembly, delta.Metadata.AsSpan(), delta.IL.AsSpan(), pdbBytes.AsSpan())

        let after = method.Invoke(null, [||]) :?> string
        printfn "[applyupdate-console] after=%s" after

        if after <> updatedMessage then
            failwith "ApplyUpdate did not apply."
