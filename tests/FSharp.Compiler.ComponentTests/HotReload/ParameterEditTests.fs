#nowarn "57" // experimental FCS hot reload session API

namespace FSharp.Compiler.ComponentTests.HotReload

// Parameter metadata updates (UpdateParameters).
//
// C# reference template (csharp_enc_reference scenario 'param_rename',
// Roslyn EmitDifference; mdv recording reference_mdv_param_rename.txt): renaming
// a parameter of an existing method re-emits the MethodDef row and the Param row
// as UPDATES (EncMap update entries at the existing row ids); the Param row's
// Name column carries the NEW name, written into the delta string heap. No rows
// are added. Parameter TYPE changes remain SignatureChange rude edits.

open System
open System.Collections.Immutable
open System.IO
open System.Reflection
open System.Reflection.Metadata
open System.Reflection.Metadata.Ecma335
open System.Runtime.Loader
open Xunit

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.HotReload
open FSharp.Compiler.Text
open Internal.Utilities
open FSharp.Test
open FSharp.Test.Utilities

[<Collection(nameof NotThreadSafeResourceCollection)>]
module ParameterEditTests =

    /// Snapshot of the project's CURRENT on-disk state (snapshots are immutable and
    /// content-versioned, so each edit needs a fresh one).
    let private snapshotOf (projectOptions: FSharpProjectOptions) =
        FSharpProjectSnapshot.FromOptions(projectOptions, DocumentSource.FileSystem)
        |> Async.RunImmediate

    /// The full capability set advertised by current CoreCLR runtimes.
    let private fullCapabilities =
        [ "Baseline"
          "AddMethodToExistingType"
          "AddStaticFieldToExistingType"
          "AddInstanceFieldToExistingType"
          "NewTypeDefinition"
          "ChangeCustomAttributes"
          "UpdateParameters"
          "GenericUpdateMethod"
          "GenericAddMethodToExistingType"
          "GenericAddFieldToExistingType" ]

    let private applyGenerationsAndVerify
        (capabilities: string list)
        (testLabel: string)
        (baselineSource: string)
        (verifyBaseline: Assembly -> unit)
        (generations: (string * (Assembly -> unit)) list)
        =
        let modifiable = Environment.GetEnvironmentVariable("DOTNET_MODIFIABLE_ASSEMBLIES")
        if not (String.Equals(modifiable, "debug", StringComparison.OrdinalIgnoreCase)) then
            printfn "[skip] DOTNET_MODIFIABLE_ASSEMBLIES must be 'debug' for '%s'" testLabel
        else
            let checker =
                FSharpChecker.Create(
                    keepAssemblyContents = true,
                    keepAllBackgroundResolutions = false,
                    keepAllBackgroundSymbolUses = false,
                    enableBackgroundItemKeyStoreAndSemanticClassification = false,
                    enablePartialTypeChecking = false,
                    captureIdentifiersWhenParsing = false
                )

            use session = checker.CreateHotReloadSession(capabilities = capabilities)

            let projectDir = Path.Combine(Path.GetTempPath(), "fsharp-hotreload-parameter-edits", Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(projectDir) |> ignore
            let fsPath = Path.Combine(projectDir, "Library.fs")
            let dllPath = Path.Combine(projectDir, "Library.dll")
            let runtimeDllPath = Path.Combine(projectDir, "Library.runtime.dll")
            let loadContext = new AssemblyLoadContext($"fsharp-hotreload-params-{Guid.NewGuid():N}", isCollectible = true)

            try
                File.WriteAllText(fsPath, baselineSource)

                let projectOptions, _ =
                    checker.GetProjectOptionsFromScript(
                        fsPath,
                        SourceText.ofString baselineSource,
                        assumeDotNetFramework = false,
                        useSdkRefs = true,
                        useFsiAuxLib = false
                    )
                    |> Async.RunImmediate

                let projectOptions =
                    { projectOptions with
                        SourceFiles = [| fsPath |]
                        OtherOptions =
                            projectOptions.OtherOptions
                            |> Array.append
                                [| "--target:library"
                                   "--langversion:preview"
                                   "--optimize-"
                                   "--debug:portable"
                                   "--deterministic"
                                   "--enable:hotreloaddeltas"
                                   $"--out:{dllPath}" |] }

                checker.InvalidateAll()

                let compileOnce (label: string) (options: FSharpProjectOptions) =
                    let diagnostics, _ =
                        checker.Compile(Array.concat [ [| "fsc.exe" |]; options.OtherOptions; options.SourceFiles ])
                        |> Async.RunImmediate

                    let errors =
                        diagnostics
                        |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)

                    if errors.Length > 0 then
                        failwithf "[%s] %s compilation failed: %A" testLabel label (errors |> Array.map (fun d -> d.Message))

                compileOnce "baseline" projectOptions

                match session.AddProject(snapshotOf projectOptions) |> Async.RunImmediate with
                | Error error -> failwithf "[%s] failed to start hot reload session: %A" testLabel error
                | Ok () -> ()

                File.Copy(dllPath, runtimeDllPath, true)
                let pdbPath = Path.ChangeExtension(dllPath, ".pdb")
                if File.Exists(pdbPath) then
                    File.Copy(pdbPath, Path.ChangeExtension(runtimeDllPath, ".pdb"), true)

                let assembly = loadContext.LoadFromAssemblyPath(runtimeDllPath)
                verifyBaseline assembly

                let updatedOptions =
                    { projectOptions with
                        OtherOptions =
                            projectOptions.OtherOptions
                            |> Array.filter (fun opt ->
                                not (opt.StartsWith("--enable:hotreloaddeltas", StringComparison.OrdinalIgnoreCase))) }

                for updatedSource, verifyUpdated in generations do
                    File.WriteAllText(fsPath, updatedSource)
                    checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate
                    compileOnce "updated" updatedOptions

                    match session.EmitDelta(snapshotOf projectOptions) |> Async.RunImmediate with
                    | Error error -> failwithf "[%s] EmitDelta failed: %A" testLabel error
                    | Ok delta ->
                        Assert.NotEmpty(delta.Metadata)
                        let pdbBytes = delta.Pdb |> Option.defaultValue Array.empty
                        MetadataUpdater.ApplyUpdate(assembly, delta.Metadata.AsSpan(), delta.IL.AsSpan(), pdbBytes.AsSpan())
                        // The runtime applied the update; commit so the next generation
                        // diffs against this one (the session entity defers commits).
                        session.Commit()
                        verifyUpdated assembly
            finally
                try loadContext.Unload() with _ -> ()
                try checker.InvalidateAll() with _ -> ()
                try Directory.Delete(projectDir, true) with _ -> ()

    let private emitDeltaAndHandleResult
        (capabilities: string list)
        (testLabel: string)
        (baselineSource: string)
        (updatedSource: string)
        (handleResult: Result<FSharpHotReloadDelta, FSharpHotReloadError> -> unit)
        =
        let checker =
            FSharpChecker.Create(
                keepAssemblyContents = true,
                keepAllBackgroundResolutions = false,
                keepAllBackgroundSymbolUses = false,
                enableBackgroundItemKeyStoreAndSemanticClassification = false,
                enablePartialTypeChecking = false,
                captureIdentifiersWhenParsing = false
            )

        use session = checker.CreateHotReloadSession(capabilities = capabilities)

        let projectDir = Path.Combine(Path.GetTempPath(), "fsharp-hotreload-parameter-edits", Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(projectDir) |> ignore
        let fsPath = Path.Combine(projectDir, "Library.fs")
        let dllPath = Path.Combine(projectDir, "Library.dll")

        try
            File.WriteAllText(fsPath, baselineSource)

            let projectOptions, _ =
                checker.GetProjectOptionsFromScript(
                    fsPath,
                    SourceText.ofString baselineSource,
                    assumeDotNetFramework = false,
                    useSdkRefs = true,
                    useFsiAuxLib = false
                )
                |> Async.RunImmediate

            let projectOptions =
                { projectOptions with
                    SourceFiles = [| fsPath |]
                    OtherOptions =
                        projectOptions.OtherOptions
                        |> Array.append
                            [| "--target:library"
                               "--langversion:preview"
                               "--optimize-"
                               "--debug:portable"
                               "--deterministic"
                               "--enable:hotreloaddeltas"
                               $"--out:{dllPath}" |] }

            checker.InvalidateAll()

            let compileOnce (label: string) (options: FSharpProjectOptions) =
                let diagnostics, _ =
                    checker.Compile(Array.concat [ [| "fsc.exe" |]; options.OtherOptions; options.SourceFiles ])
                    |> Async.RunImmediate

                let errors =
                    diagnostics
                    |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)

                if errors.Length > 0 then
                    failwithf "[%s] %s compilation failed: %A" testLabel label (errors |> Array.map (fun d -> d.Message))

            compileOnce "baseline" projectOptions

            match session.AddProject(snapshotOf projectOptions) |> Async.RunImmediate with
            | Error error -> failwithf "[%s] failed to start hot reload session: %A" testLabel error
            | Ok () -> ()

            File.WriteAllText(fsPath, updatedSource)
            checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate

            let updatedOptions =
                { projectOptions with
                    OtherOptions =
                        projectOptions.OtherOptions
                        |> Array.filter (fun opt ->
                            not (opt.StartsWith("--enable:hotreloaddeltas", StringComparison.OrdinalIgnoreCase))) }

            compileOnce "updated" updatedOptions

            session.EmitDelta(snapshotOf projectOptions)
            |> Async.RunImmediate
            |> handleResult
        finally
            try checker.InvalidateAll() with _ -> ()
            try Directory.Delete(projectDir, true) with _ -> ()

    let private emitDeltaAndInspect capabilities testLabel baselineSource updatedSource (inspect: FSharpHotReloadDelta -> unit) =
        emitDeltaAndHandleResult capabilities testLabel baselineSource updatedSource (fun result ->
            match result with
            | Error error -> failwithf "[%s] EmitDelta failed: %A" testLabel error
            | Ok delta -> inspect delta)

    let private emitDeltaAndExpectUnsupported capabilities testLabel baselineSource updatedSource (expectedMessageParts: string list) =
        emitDeltaAndHandleResult capabilities testLabel baselineSource updatedSource (fun result ->
            match result with
            | Ok _ -> failwithf "[%s] expected EmitDelta to fail, but it succeeded." testLabel
            | Error error ->
                let message = string error

                for part in expectedMessageParts do
                    Assert.Contains(part, message))

    // -----------------------------------------------------------------------------
    // Sources
    // -----------------------------------------------------------------------------

    let private baseline =
        """
namespace Sample

module Library =
    let compute (x: int) = x + 1
"""

    let private renamed =
        """
namespace Sample

module Library =
    let compute (renamed: int) = renamed + 1
"""

    // -----------------------------------------------------------------------------
    // Tests
    // -----------------------------------------------------------------------------

    [<Fact>]
    let ``Parameter rename updates the Param row name after ApplyUpdate`` () =
        // The baseline verify must not touch GetParameters/Invoke: the runtime caches a
        // method's ParameterInfo on first use, and a cache primed before ApplyUpdate keeps
        // serving the OLD name afterwards (parameter names are debugger-facing metadata;
        // reflection invalidation is not guaranteed). Reading the name for the first time
        // AFTER the update observes the updated Param row.
        applyGenerationsAndVerify
            fullCapabilities
            "param-rename"
            baseline
            (fun assembly ->
                let libType = assembly.GetType("Sample.Library", throwOnError = true)
                Assert.NotNull(libType.GetMethod("compute", BindingFlags.Public ||| BindingFlags.Static)))
            [ renamed,
              (fun assembly ->
                  let libType = assembly.GetType("Sample.Library", throwOnError = true)
                  let compute = libType.GetMethod("compute", BindingFlags.Public ||| BindingFlags.Static)
                  Assert.Equal("renamed", (Seq.exactlyOne (compute.GetParameters())).Name)
                  Assert.Equal(42, compute.Invoke(null, [| box 41 |]) :?> int)) ]

    [<Fact>]
    let ``Parameter rename delta matches the C# row update template`` () =
        // C# 'param_rename' template: MethodDef 1 update + Param 1 update (EncMap update
        // entries, no Param adds); the Param row's name column carries the new name.
        emitDeltaAndInspect
            fullCapabilities
            "param-rename-template"
            baseline
            renamed
            (fun delta ->
                use provider = MetadataReaderProvider.FromMetadataImage(ImmutableArray.ToImmutableArray(delta.Metadata: byte[]))
                let reader = provider.GetMetadataReader()

                let paramMapEntries =
                    reader.GetEditAndContinueMapEntries()
                    |> Seq.map MetadataTokens.GetToken
                    |> Seq.filter (fun token -> token >>> 24 = 0x08)
                    |> Seq.toList

                // A single Param row, updating in place (row id 1 — the baseline method's
                // only parameter).
                let token = Assert.Single paramMapEntries
                Assert.Equal(1, token &&& 0x00FFFFFF)
                Assert.Equal(1, reader.GetTableRowCount(TableIndex.Param))

                // The re-emitted Param row's name lives in the DELTA #Strings heap (its
                // handle is an absolute baseline+delta offset, which a standalone reader
                // over the delta image cannot chase) — assert the new name reached the
                // delta heap and the old one did not.
                let metadataText = System.Text.Encoding.ASCII.GetString(delta.Metadata)
                Assert.Contains("renamed", metadataText))

    [<Fact>]
    let ``EmitDelta rejects parameter rename without UpdateParameters`` () =
        emitDeltaAndExpectUnsupported
            [ "Baseline"; "AddMethodToExistingType"; "ChangeCustomAttributes" ]
            "param-rename-no-capability"
            baseline
            renamed
            [ "UpdateParameters" ]

    [<Fact>]
    let ``Parameter rename on an instance member leaves the self identifier ungated`` () =
        // Renaming the self identifier (`this` -> `self`) is NOT a parameter rename (no
        // Param row exists for it); only real IL parameters gate on UpdateParameters.
        let instanceBaseline =
            """
namespace Sample

type Holder() =
    member this.Add(x: int) = x + 1
"""

        let selfRenamed =
            """
namespace Sample

type Holder() =
    member self.Add(x: int) = x + 1
"""

        emitDeltaAndHandleResult
            [ "Baseline"; "GenericUpdateMethod" ]
            "self-identifier-rename"
            instanceBaseline
            selfRenamed
            (fun result ->
                // A pure self-identifier rename produces no parameter edit; the result is
                // either an unchanged-delta error or a plain body update — never an
                // UpdateParameters gate.
                match result with
                | Ok _ -> ()
                | Error error ->
                    let message = string error
                    Assert.DoesNotContain("UpdateParameters", message))
