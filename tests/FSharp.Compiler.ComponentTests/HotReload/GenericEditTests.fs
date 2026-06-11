namespace FSharp.Compiler.ComponentTests.HotReload

// Phase E: generic method and generic-type-member edits.
//
// C# reference templates (csharp_enc_reference `generics` scenarios, Roslyn
// EmitDifference; mdv recordings in hot_reload_poc/src/csharp_enc_reference):
//   - generic method body update: MethodDef update only; GenericParam rows are
//     BASELINE rows and are NOT re-emitted; locals signature uses MVAR (!!0).
//   - generic class member body update: MethodDef update; own-field access goes
//     through a MemberRef parented by the TypeSpec self-instantiation
//     (Container`1<!0>); no GenericParam rows.
//   - adding a method to a generic class: ordinary (TypeDef, AddMethod) pair;
//     no GenericParam rows (the added method's signature simply uses VAR).
//   - adding a GENERIC method: AddMethod pair + a GenericParam row logged as a
//     plain Default EncLog entry, present in EncMap as an add.

open System
open System.IO
open System.Reflection
open System.Reflection.Metadata
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
module GenericEditTests =

    /// The full capability set advertised by current CoreCLR runtimes, including the
    /// generic-edit capabilities (MetadataUpdater.GetCapabilities()).
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

    /// Compiles `baselineSource`, starts a hot reload session with the given
    /// capabilities, loads the baseline assembly, asserts via `verify` (generation 0),
    /// then for each (updatedSource, verify) generation: recompiles, emits the delta,
    /// applies it with MetadataUpdater.ApplyUpdate and asserts `verify` again.
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

            let projectDir = Path.Combine(Path.GetTempPath(), "fsharp-hotreload-generic-edits", Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(projectDir) |> ignore
            let fsPath = Path.Combine(projectDir, "Library.fs")
            let dllPath = Path.Combine(projectDir, "Library.dll")
            let runtimeDllPath = Path.Combine(projectDir, "Library.runtime.dll")
            let loadContext = new AssemblyLoadContext($"fsharp-hotreload-generic-{Guid.NewGuid():N}", isCollectible = true)

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

                match checker.StartHotReloadSession(projectOptions, capabilities = capabilities) |> Async.RunImmediate with
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

                    match checker.EmitHotReloadDelta(projectOptions) |> Async.RunImmediate with
                    | Error error -> failwithf "[%s] EmitHotReloadDelta failed: %A" testLabel error
                    | Ok delta ->
                        Assert.NotEmpty(delta.Metadata)
                        let pdbBytes = delta.Pdb |> Option.defaultValue Array.empty
                        MetadataUpdater.ApplyUpdate(assembly, delta.Metadata.AsSpan(), delta.IL.AsSpan(), pdbBytes.AsSpan())
                        verifyUpdated assembly
            finally
                try loadContext.Unload() with _ -> ()
                try checker.EndHotReloadSession() with _ -> ()
                try checker.InvalidateAll() with _ -> ()
                try Directory.Delete(projectDir, true) with _ -> ()

    /// Compiles `baselineSource`, starts a session with the given capabilities, applies
    /// `updatedSource` and asserts EmitHotReloadDelta fails with a message containing
    /// every fragment of `expectedMessageParts`.
    let private emitDeltaAndExpectUnsupported
        (capabilities: string list)
        (testLabel: string)
        (baselineSource: string)
        (updatedSource: string)
        (expectedMessageParts: string list)
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

        let projectDir = Path.Combine(Path.GetTempPath(), "fsharp-hotreload-generic-edits", Guid.NewGuid().ToString("N"))
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

            match checker.StartHotReloadSession(projectOptions, capabilities = capabilities) |> Async.RunImmediate with
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

            match checker.EmitHotReloadDelta(projectOptions) |> Async.RunImmediate with
            | Ok _ -> failwithf "[%s] expected EmitHotReloadDelta to fail, but it succeeded." testLabel
            | Error error ->
                let message = string error
                for part in expectedMessageParts do
                    Assert.Contains(part, message)
        finally
            try checker.EndHotReloadSession() with _ -> ()
            try checker.InvalidateAll() with _ -> ()
            try Directory.Delete(projectDir, true) with _ -> ()

    // -----------------------------------------------------------------------------
    // Sources
    // -----------------------------------------------------------------------------

    let private genericFunctionBaseline =
        """
namespace Sample

module Lib =
    let describe<'T> (value: 'T) = "Hello " + value.ToString()
"""

    let private genericClassBaseline =
        """
namespace Sample

type Container<'T>(value: 'T) =
    member _.Describe() = "Hello " + value.ToString()
"""

    let private invokeDescribe (assembly: Assembly) (expected: string) =
        let libType = assembly.GetType("Sample.Lib", throwOnError = true)
        let methodDef = libType.GetMethod("describe", BindingFlags.Public ||| BindingFlags.Static)

        let forInt = methodDef.MakeGenericMethod(typeof<int>)
        Assert.Equal(expected + "42", forInt.Invoke(null, [| box 42 |]) :?> string)

        let forString = methodDef.MakeGenericMethod(typeof<string>)
        Assert.Equal(expected + "watch", forString.Invoke(null, [| box "watch" |]) :?> string)

    let private invokeContainerDescribe (assembly: Assembly) (expected: string) =
        let containerDef = assembly.GetType("Sample.Container`1", throwOnError = true)

        let intContainer = Activator.CreateInstance(containerDef.MakeGenericType(typeof<int>), box 42)
        let describeInt = intContainer.GetType().GetMethod("Describe")
        Assert.Equal(expected + "42", describeInt.Invoke(intContainer, [||]) :?> string)

        let stringContainer = Activator.CreateInstance(containerDef.MakeGenericType(typeof<string>), box "watch")
        let describeString = stringContainer.GetType().GetMethod("Describe")
        Assert.Equal(expected + "watch", describeString.Invoke(stringContainer, [||]) :?> string)

    // -----------------------------------------------------------------------------
    // Probes / runtime tests
    // -----------------------------------------------------------------------------

    [<Fact>]
    let ``ApplyUpdate succeeds for generic module function body edit`` () =
        let updated = genericFunctionBaseline.Replace("Hello ", "Welcome ")

        applyGenerationsAndVerify
            fullCapabilities
            "generic-function-body-edit"
            genericFunctionBaseline
            (fun assembly -> invokeDescribe assembly "Hello ")
            [ updated, (fun assembly -> invokeDescribe assembly "Welcome ") ]

    [<Fact>]
    let ``ApplyUpdate succeeds for generic class member body edit`` () =
        let updated = genericClassBaseline.Replace("Hello ", "Welcome ")

        applyGenerationsAndVerify
            fullCapabilities
            "generic-class-member-body-edit"
            genericClassBaseline
            (fun assembly -> invokeContainerDescribe assembly "Hello ")
            [ updated, (fun assembly -> invokeContainerDescribe assembly "Welcome ") ]

    [<Fact>]
    let ``ApplyUpdate succeeds for generic body edits across generations with new instantiations`` () =
        // Generation 1 body-edits the prefix; generation 2 additionally calls a generic
        // instantiation (List.replicate<'T>, a MethodSpec with an MVAR instantiation blob)
        // that the baseline body never referenced, exercising generic-context signature
        // remapping in the delta (MethodSpec/TypeSpec machinery from C4).
        let generation1 = genericFunctionBaseline.Replace("Hello ", "Welcome ")

        let generation2 =
            """
namespace Sample

module Lib =
    let describe<'T> (value: 'T) =
        let count = List.replicate 2 value |> List.length
        "Welcome x" + count.ToString() + " " + value.ToString()
"""

        applyGenerationsAndVerify
            fullCapabilities
            "generic-function-multi-generation"
            genericFunctionBaseline
            (fun assembly -> invokeDescribe assembly "Hello ")
            [ generation1, (fun assembly -> invokeDescribe assembly "Welcome ")
              generation2, (fun assembly -> invokeDescribe assembly "Welcome x2 ") ]

    [<Fact>]
    let ``EmitHotReloadDelta rejects generic body edit without GenericUpdateMethod capability`` () =
        // Roslyn parity: updating a generic method requires the GenericUpdateMethod
        // runtime capability; a baseline-only session reports the rude edit naming it.
        let updated = genericFunctionBaseline.Replace("Hello ", "Welcome ")

        emitDeltaAndExpectUnsupported
            [ "Baseline" ]
            "generic-function-body-edit-no-capability"
            genericFunctionBaseline
            updated
            [ "GenericUpdateMethod" ]
