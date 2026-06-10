module FSharp.Compiler.ComponentTests.HotReload.ApplyUpdateRunner

open System
open System.IO
open System.Reflection
open System.Reflection.Metadata
open System.Runtime.Loader
open System.Diagnostics
open Xunit
open FSharp.Compiler.ComponentTests.HotReload.ApplyUpdateShared

// This is a minimal console-style entry point that can be launched via `dotnet test --filter ...`
// to isolate hosting from vstest. It returns success if ApplyUpdate succeeds, otherwise throws.
[<Fact>]
let ``ApplyUpdate runner`` () =
    let modifiable = Environment.GetEnvironmentVariable("DOTNET_MODIFIABLE_ASSEMBLIES")

    if not (String.Equals(modifiable, "debug", StringComparison.OrdinalIgnoreCase)) then
        failwith "DOTNET_MODIFIABLE_ASSEMBLIES must be 'debug' for this runner."

    printfn "[applyupdate-runner] MetadataUpdater.IsSupported=%b" (MetadataUpdater.IsSupported)

    let updatedMessage = "Hello updated"
    let artifacts = createApplyUpdateDeltaArtifacts updatedMessage
    let baselineArtifacts = artifacts.BaselineArtifacts
    let typeName = artifacts.TypeName
    let delta = artifacts.Delta

    // Load baseline into a non-collectible ALC to match CoreCLR EnC code paths.
    let alc = new AssemblyLoadContext("ApplyUpdateRunner_" + Guid.NewGuid().ToString("N"), isCollectible = false)
    let assembly = alc.LoadFromAssemblyPath baselineArtifacts.AssemblyPath
    let moduleType, encCapable = probeApplyUpdateAssembly "applyupdate-runner" baselineArtifacts.AssemblyPath assembly

    assembly.GetCustomAttributes<DebuggableAttribute>()
    |> Seq.iter (fun a ->
        printfn "[applyupdate-runner] Debuggable: tracking=%b disableOpt=%b modes=%A" a.IsJITTrackingEnabled a.IsJITOptimizerDisabled a.DebuggingFlags)

    printfn "[applyupdate-runner] AssemblyName=%s Path=%s" assembly.FullName assembly.Location

    [ "m_debuggerInfoBits"; "m_debuggerBits"; "m_dwTransientFlags" ]
    |> List.iter (fun name ->
        match moduleType.GetField(name, BindingFlags.Instance ||| BindingFlags.NonPublic) with
        | null -> ()
        | field ->
            let value = field.GetValue(assembly.ManifestModule)
            printfn "[applyupdate-runner] %s=%A" name value)

    if not encCapable then
        printfn "[applyupdate-runner] IsEditAndContinueCapable returned false; continuing with ApplyUpdate probe."

    let method = assembly.GetType(typeName, throwOnError = true).GetMethod("GetMessage", BindingFlags.Public ||| BindingFlags.Static)
    let before = method.Invoke(null, [||]) :?> string
    printfn "[applyupdate-runner] Before update: %s" before

    if before <> "Hello baseline" then
        failwithf "Unexpected baseline result: %s" before

    let pdbBytes = pdbBytesOrEmpty delta.Pdb

    printfn
        "[applyupdate-runner] Applying delta: metadata=%d bytes, IL=%d bytes, PDB=%d bytes"
        delta.Metadata.Length
        delta.IL.Length
        pdbBytes.Length

    // Dump delta to /tmp for analysis with mdv
    let dumpDir = "/tmp/fsharp-delta-debug"

    if not (Directory.Exists dumpDir) then
        Directory.CreateDirectory dumpDir |> ignore

    File.WriteAllBytes(Path.Combine(dumpDir, "1.meta"), delta.Metadata)
    File.WriteAllBytes(Path.Combine(dumpDir, "1.il"), delta.IL)

    if pdbBytes.Length > 0 then
        File.WriteAllBytes(Path.Combine(dumpDir, "1.pdb"), pdbBytes)

    File.Copy(baselineArtifacts.AssemblyPath, Path.Combine(dumpDir, "baseline.dll"), true)
    printfn "[applyupdate-runner] Delta written to %s" dumpDir

    try
        MetadataUpdater.ApplyUpdate(assembly, delta.Metadata.AsSpan(), delta.IL.AsSpan(), pdbBytes.AsSpan())
        printfn "[applyupdate-runner] ApplyUpdate succeeded!"
    with
    | :? InvalidOperationException as ex when ex.Message.Contains("not editable") ->
        failwithf "Assembly is NOT EnC-capable: %s" ex.Message
    | :? InvalidOperationException as ex ->
        failwithf "ApplyUpdate failed (assembly IS EnC-capable, but delta rejected): %s" ex.Message

    let after = method.Invoke(null, [||]) :?> string
    printfn "[applyupdate-runner] After update: %s" after

    if after <> updatedMessage then
        failwithf "Unexpected updated result: expected '%s' but got '%s'" updatedMessage after

    printfn "[applyupdate-runner] SUCCESS: Hot reload worked! Value changed from '%s' to '%s'" before after
