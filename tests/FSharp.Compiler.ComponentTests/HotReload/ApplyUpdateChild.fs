module FSharp.Compiler.ComponentTests.HotReload.ApplyUpdateChild

open System
open System.Reflection
open System.Reflection.Metadata
open System.Runtime.Loader
open System.Diagnostics
open Xunit
open FSharp.Compiler.ComponentTests.HotReload.ApplyUpdateShared

[<Fact>]
let ``ApplyUpdate child process`` () =
    let originalMessage = "Hello baseline"
    let updatedMessage = "Hello updated"

    printfn "[applyupdate-child] MetadataUpdater.IsSupported=%b" (MetadataUpdater.IsSupported)

    let artifacts = createApplyUpdateDeltaArtifacts updatedMessage
    let baselineArtifacts = artifacts.BaselineArtifacts
    let typeName = artifacts.TypeName
    let delta = artifacts.Delta

    // Load baseline into a fresh collectible ALC to avoid collisions.
    let alc = new AssemblyLoadContext("ApplyUpdateChild_" + Guid.NewGuid().ToString("N"), isCollectible = true)
    let assembly = alc.LoadFromAssemblyPath baselineArtifacts.AssemblyPath
    let sampleType = assembly.GetType(typeName, throwOnError = true)
    let method = sampleType.GetMethod("GetMessage", BindingFlags.Public ||| BindingFlags.Static)

    let moduleType, encCapable =
        probeApplyUpdateAssembly "applyupdate-child" baselineArtifacts.AssemblyPath assembly

    assembly.GetCustomAttributes<DebuggableAttribute>()
    |> Seq.iter (fun a ->
        printfn "[applyupdate-child] Debuggable: tracking=%b disableOpt=%b modes=%A" a.IsJITTrackingEnabled a.IsJITOptimizerDisabled a.DebuggingFlags)

    printfn "[applyupdate-child] AssemblyName=%s Path=%s" assembly.FullName assembly.Location

    [ "m_debuggerInfoBits"; "m_debuggerBits"; "m_dwTransientFlags" ]
    |> List.iter (fun name ->
        match moduleType.GetField(name, BindingFlags.Instance ||| BindingFlags.NonPublic) with
        | null -> ()
        | field ->
            let value = field.GetValue(assembly.ManifestModule)
            printfn "[applyupdate-child] %s=%A" name value)

    if not encCapable then
        printfn "[applyupdate-child] Skipping body: module not EnC-capable."
    else
        let before = method.Invoke(null, [||]) :?> string
        Assert.Equal(originalMessage, before)

        let pdbBytes = pdbBytesOrEmpty delta.Pdb
        MetadataUpdater.ApplyUpdate(assembly, delta.Metadata.AsSpan(), delta.IL.AsSpan(), pdbBytes.AsSpan())

        let after = method.Invoke(null, [||]) :?> string
        Assert.Equal(updatedMessage, after)
