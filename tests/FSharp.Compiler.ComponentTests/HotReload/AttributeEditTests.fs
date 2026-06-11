namespace FSharp.Compiler.ComponentTests.HotReload

// Phase F: custom attribute rows in deltas.
//
// C# reference templates (csharp_enc_reference `phasef` scenarios, Roslyn
// EmitDifference; mdv recordings in hot_reload_poc/src/csharp_enc_reference):
//   - attr_add:    [Description] added to an EXISTING method -> MethodDef/Param row
//     updates plus ONE appended CustomAttribute row (parent = existing MethodDef
//     token, EncMap add).
//   - attr_change: the attribute argument changes -> CustomAttribute row UPDATE in
//     place (EncLog Default at the EXISTING row id, EncMap update).
//   - attr_remove: the attribute is removed -> CustomAttribute row UPDATE whose
//     Parent/Constructor/Value columns are all nil (the row is zeroed).
//   - prop_add (members scenario): ADDED members carry their attribute rows in the
//     same delta (2-4 CA rows each: [CompilerGenerated]/[DebuggerBrowsable] on the
//     backing field and accessors), appended past the baseline row count and
//     ordered by the HasCustomAttribute parent coded index.

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
module AttributeEditTests =

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

    let private applyGenerationsAndVerifyCore
        (diskStartedSession: bool)
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

            let projectDir = Path.Combine(Path.GetTempPath(), "fsharp-hotreload-attribute-edits", Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(projectDir) |> ignore
            let fsPath = Path.Combine(projectDir, "Library.fs")
            let dllPath = Path.Combine(projectDir, "Library.dll")
            let runtimeDllPath = Path.Combine(projectDir, "Library.runtime.dll")
            let loadContext = new AssemblyLoadContext($"fsharp-hotreload-attrs-{Guid.NewGuid():N}", isCollectible = true)

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

                if diskStartedSession then
                    FSharpEditAndContinueLanguageService.Instance.ResetSessionState()

                    match FSharpEditAndContinueLanguageService.Instance.TryGetSession() with
                    | ValueSome _ -> failwithf "[%s] expected no in-process session after the reset." testLabel
                    | ValueNone -> ()

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

    let private applyGenerationsAndVerify capabilities testLabel baselineSource verifyBaseline generations =
        applyGenerationsAndVerifyCore false capabilities testLabel baselineSource verifyBaseline generations

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

        let projectDir = Path.Combine(Path.GetTempPath(), "fsharp-hotreload-attribute-edits", Guid.NewGuid().ToString("N"))
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

            checker.EmitHotReloadDelta(projectOptions)
            |> Async.RunImmediate
            |> handleResult
        finally
            try checker.EndHotReloadSession() with _ -> ()
            try checker.InvalidateAll() with _ -> ()
            try Directory.Delete(projectDir, true) with _ -> ()

    let private emitDeltaAndInspect
        (capabilities: string list)
        (testLabel: string)
        (baselineSource: string)
        (updatedSource: string)
        (inspect: FSharpHotReloadDelta -> unit)
        =
        emitDeltaAndHandleResult capabilities testLabel baselineSource updatedSource (fun result ->
            match result with
            | Error error -> failwithf "[%s] EmitHotReloadDelta failed: %A" testLabel error
            | Ok delta -> inspect delta)

    // -----------------------------------------------------------------------------
    // Sources
    // -----------------------------------------------------------------------------

    let private moduleBaseline =
        """
namespace Sample

module Library =
    let existing (x: int) = x + 1
"""

    let private classBaseline =
        """
namespace Sample

type Holder() =
    member _.Read() = 1
"""

    // -----------------------------------------------------------------------------
    // Sub-slice 1: CustomAttribute rows on ADDED members
    // -----------------------------------------------------------------------------

    [<Fact>]
    let ``Added method carries its custom attributes after ApplyUpdate`` () =
        // The added method's fresh-compile attributes must ship as CustomAttribute row
        // adds parented to the new MethodDef row, with the value blob written into the
        // DELTA blob heap (a fresh-compile heap offset would be garbage against the
        // baseline+delta layout).
        let updated =
            """
namespace Sample

module Library =
    let existing (x: int) = x + 1

    [<System.Obsolete("warn")>]
    let helper (x: int) = x * 2
"""

        applyGenerationsAndVerify
            fullCapabilities
            "added-method-with-attribute"
            moduleBaseline
            (fun assembly ->
                let libType = assembly.GetType("Sample.Library", throwOnError = true)
                let existing = libType.GetMethod("existing", BindingFlags.Public ||| BindingFlags.Static)
                Assert.Equal(42, existing.Invoke(null, [| box 41 |]) :?> int))
            [ updated,
              (fun assembly ->
                  let libType = assembly.GetType("Sample.Library", throwOnError = true)
                  let helper = libType.GetMethod("helper", BindingFlags.Public ||| BindingFlags.Static)
                  Assert.Equal(42, helper.Invoke(null, [| box 21 |]) :?> int)

                  let obsolete =
                      helper.GetCustomAttributes(typeof<ObsoleteAttribute>, false)
                      |> Seq.cast<ObsoleteAttribute>
                      |> Seq.exactlyOne

                  Assert.Equal("warn", obsolete.Message)) ]

    [<Fact>]
    let ``Added module value carries property and accessor attributes after ApplyUpdate`` () =
        // A module-level mutable value lowers to a startup-class backing field
        // ([DebuggerBrowsable] CA), get_/set_ accessors and a module property carrying
        // [CompilationMapping] — the F# counterpart of the C# 'prop_add' template's CA
        // rows. Reflection must see the property attribute on the LIVE type after
        // ApplyUpdate (the HCA_Property parent and the delta-heap value blob resolve).
        let updated =
            """
namespace Sample

module Library =
    let existing (x: int) = x + 1
    let mutable counter = 41
"""

        applyGenerationsAndVerify
            fullCapabilities
            "added-module-value-attributes"
            moduleBaseline
            (fun assembly ->
                let libType = assembly.GetType("Sample.Library", throwOnError = true)
                let existing = libType.GetMethod("existing", BindingFlags.Public ||| BindingFlags.Static)
                Assert.Equal(42, existing.Invoke(null, [| box 41 |]) :?> int))
            [ updated,
              (fun assembly ->
                  let libType = assembly.GetType("Sample.Library", throwOnError = true)

                  let prop = libType.GetProperty("counter", BindingFlags.Public ||| BindingFlags.Static)
                  Assert.NotNull prop
                  Assert.Equal(41, prop.GetValue(null) :?> int)

                  // CompilationMapping on the added Property row.
                  let mapping =
                      prop.GetCustomAttributes(typeof<CompilationMappingAttribute>, false)
                      |> Seq.cast<CompilationMappingAttribute>
                      |> Seq.exactlyOne

                  Assert.Equal(SourceConstructFlags.Value, mapping.SourceConstructFlags)) ]

    [<Fact>]
    let ``Added auto property accessors carry their attributes after ApplyUpdate`` () =
        // F# lowers `member val` to backing field + accessors + Property row; the fresh
        // compile decorates the accessors with [CompilerGenerated]/[DebuggerNonUserCode]
        // (the property itself and the backing field carry no attributes in F#'s
        // lowering, unlike C#'s k__BackingField). Reflection must see the accessor
        // attributes on the LIVE type after ApplyUpdate.
        let updated =
            """
namespace Sample

type Holder() =
    member _.Read() = 1
    member val Slot = 41 with get, set
"""

        applyGenerationsAndVerify
            fullCapabilities
            "added-auto-property-attributes"
            classBaseline
            (fun assembly ->
                let holderType = assembly.GetType("Sample.Holder", throwOnError = true)
                let instance = Activator.CreateInstance holderType
                Assert.Equal(1, holderType.GetMethod("Read").Invoke(instance, [||]) :?> int))
            [ updated,
              (fun assembly ->
                  let holderType = assembly.GetType("Sample.Holder", throwOnError = true)
                  let instance = Activator.CreateInstance holderType

                  let prop = holderType.GetProperty("Slot")
                  Assert.Equal(41, prop.GetValue(instance) :?> int)

                  let getter = prop.GetGetMethod()

                  let compilerGenerated =
                      getter.GetCustomAttributes(
                          typeof<System.Runtime.CompilerServices.CompilerGeneratedAttribute>,
                          false
                      )

                  Assert.NotEmpty compilerGenerated) ]

    [<Fact>]
    let ``Added module value delta carries parent-sorted custom attribute row adds`` () =
        // Template shape (C# 'prop_add' parity): the CA rows of added members are appended
        // past the baseline row count as plain Default EncLog entries / EncMap adds, the
        // physical delta table ordered by the HasCustomAttribute parent coded index, with
        // Field-parented ([DebuggerBrowsable] on the backing/init fields) and
        // Property-parented ([CompilationMapping]) rows present.
        let updated =
            """
namespace Sample

module Library =
    let existing (x: int) = x + 1
    let mutable counter = 41
"""

        emitDeltaAndInspect
            fullCapabilities
            "added-module-value-attribute-template"
            moduleBaseline
            updated
            (fun delta ->
                use provider = MetadataReaderProvider.FromMetadataImage(ImmutableArray.ToImmutableArray(delta.Metadata: byte[]))
                let reader = provider.GetMetadataReader()

                // Every CustomAttribute EncLog entry is a plain Default operation.
                let caLogEntries =
                    reader.GetEditAndContinueLogEntries()
                    |> Seq.filter (fun entry -> MetadataTokens.GetToken entry.Handle >>> 24 = 0x0C)
                    |> Seq.toList

                Assert.NotEmpty caLogEntries

                for entry in caLogEntries do
                    Assert.Equal(EditAndContinueOperation.Default, entry.Operation)

                // The physical rows are ordered by the HasCustomAttribute parent coded
                // index, and Field/MethodDef/Property parents are all present.
                let parents =
                    reader.CustomAttributes
                    |> Seq.map (fun handle -> (reader.GetCustomAttribute handle).Parent)
                    |> Seq.toList

                let codedIndex (handle: EntityHandle) =
                    let token = MetadataTokens.GetToken handle
                    let rowId = token &&& 0x00FFFFFF

                    let tag =
                        match token >>> 24 with
                        | 0x06 -> 0
                        | 0x04 -> 1
                        | 0x01 -> 2
                        | 0x02 -> 3
                        | 0x08 -> 4
                        | 0x17 -> 9
                        | 0x14 -> 10
                        | other -> other + 32

                    (rowId <<< 5) ||| tag

                let codes = parents |> List.map codedIndex
                Assert.Equal<int list>(List.sort codes, codes)

                let parentTables = parents |> List.map (fun p -> MetadataTokens.GetToken p >>> 24) |> Set.ofList
                Assert.Contains(0x04, parentTables)
                Assert.Contains(0x17, parentTables))
