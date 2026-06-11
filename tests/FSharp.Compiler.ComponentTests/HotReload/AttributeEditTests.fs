#nowarn "57" // experimental FCS hot reload session API

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

            use session = checker.CreateHotReloadSession(capabilities = capabilities)

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

        use session = checker.CreateHotReloadSession(capabilities = capabilities)

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

    let private emitDeltaAndInspect
        (capabilities: string list)
        (testLabel: string)
        (baselineSource: string)
        (updatedSource: string)
        (inspect: FSharpHotReloadDelta -> unit)
        =
        emitDeltaAndHandleResult capabilities testLabel baselineSource updatedSource (fun result ->
            match result with
            | Error error -> failwithf "[%s] EmitDelta failed: %A" testLabel error
            | Ok delta -> inspect delta)

    let private emitDeltaAndExpectUnsupported
        (capabilities: string list)
        (testLabel: string)
        (baselineSource: string)
        (updatedSource: string)
        (expectedMessageParts: string list)
        =
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

    // -----------------------------------------------------------------------------
    // Sub-slice 2: attribute changes on EXISTING members (ChangeCustomAttributes)
    // -----------------------------------------------------------------------------

    let private attributedBaseline =
        """
namespace Sample

module Library =
    [<System.Obsolete("a")>]
    let existing (x: int) = x + 1
"""

    [<Fact>]
    let ``Attribute added to existing method appends one custom attribute row`` () =
        // C# 'attr_add' template: MethodDef + Param row updates plus ONE appended
        // CustomAttribute row (EncMap add); reflection sees the attribute afterwards.
        let updated =
            """
namespace Sample

module Library =
    [<System.Obsolete("warn")>]
    let existing (x: int) = x + 1
"""

        applyGenerationsAndVerify
            fullCapabilities
            "attr-add-existing-method"
            moduleBaseline
            (fun assembly ->
                let libType = assembly.GetType("Sample.Library", throwOnError = true)
                let existing = libType.GetMethod("existing", BindingFlags.Public ||| BindingFlags.Static)
                Assert.Empty(existing.GetCustomAttributes(typeof<ObsoleteAttribute>, false)))
            [ updated,
              (fun assembly ->
                  let libType = assembly.GetType("Sample.Library", throwOnError = true)
                  let existing = libType.GetMethod("existing", BindingFlags.Public ||| BindingFlags.Static)
                  Assert.Equal(42, existing.Invoke(null, [| box 41 |]) :?> int)

                  let obsolete =
                      existing.GetCustomAttributes(typeof<ObsoleteAttribute>, false)
                      |> Seq.cast<ObsoleteAttribute>
                      |> Seq.exactlyOne

                  Assert.Equal("warn", obsolete.Message)) ]

    [<Fact>]
    let ``Attribute argument change updates the custom attribute row in place`` () =
        // C# 'attr_change' template: the CustomAttribute row is UPDATED at its existing
        // row id (EncMap update, not add). Reflection must see exactly ONE attribute
        // with the new argument — a duplicated appended row would surface two.
        let updated = attributedBaseline.Replace("\"a\"", "\"b\"")

        applyGenerationsAndVerify
            fullCapabilities
            "attr-change-existing-method"
            attributedBaseline
            (fun assembly ->
                let libType = assembly.GetType("Sample.Library", throwOnError = true)
                let existing = libType.GetMethod("existing", BindingFlags.Public ||| BindingFlags.Static)

                let obsolete =
                    existing.GetCustomAttributes(typeof<ObsoleteAttribute>, false)
                    |> Seq.cast<ObsoleteAttribute>
                    |> Seq.exactlyOne

                Assert.Equal("a", obsolete.Message))
            [ updated,
              (fun assembly ->
                  let libType = assembly.GetType("Sample.Library", throwOnError = true)
                  let existing = libType.GetMethod("existing", BindingFlags.Public ||| BindingFlags.Static)

                  let obsolete =
                      existing.GetCustomAttributes(typeof<ObsoleteAttribute>, false)
                      |> Seq.cast<ObsoleteAttribute>
                      |> Seq.exactlyOne

                  Assert.Equal("b", obsolete.Message)) ]

    [<Fact>]
    let ``Attribute removal zeroes the custom attribute row`` () =
        // C# 'attr_remove' template: the CustomAttribute row is UPDATED in place with
        // all-nil columns (raw row bytes 00000000-03000000-00000000); reflection then
        // reports no attribute.
        applyGenerationsAndVerify
            fullCapabilities
            "attr-remove-existing-method"
            attributedBaseline
            (fun assembly ->
                let libType = assembly.GetType("Sample.Library", throwOnError = true)
                let existing = libType.GetMethod("existing", BindingFlags.Public ||| BindingFlags.Static)
                Assert.NotEmpty(existing.GetCustomAttributes(typeof<ObsoleteAttribute>, false)))
            [ moduleBaseline,
              (fun assembly ->
                  let libType = assembly.GetType("Sample.Library", throwOnError = true)
                  let existing = libType.GetMethod("existing", BindingFlags.Public ||| BindingFlags.Static)
                  Assert.Equal(42, existing.Invoke(null, [| box 41 |]) :?> int)
                  Assert.Empty(existing.GetCustomAttributes(typeof<ObsoleteAttribute>, false))) ]

    [<Fact>]
    let ``Attribute change delta matches the C# row update and zeroing templates`` () =
        // Template-shape assertions for attr_change: the CA EncLog entry addresses the
        // EXISTING row id (an update, not an add past the baseline count).
        let updated = attributedBaseline.Replace("\"a\"", "\"b\"")

        emitDeltaAndInspect
            fullCapabilities
            "attr-change-template"
            attributedBaseline
            updated
            (fun delta ->
                use provider = MetadataReaderProvider.FromMetadataImage(ImmutableArray.ToImmutableArray(delta.Metadata: byte[]))
                let reader = provider.GetMetadataReader()

                let caMapEntries =
                    reader.GetEditAndContinueMapEntries()
                    |> Seq.map MetadataTokens.GetToken
                    |> Seq.filter (fun token -> token >>> 24 = 0x0C)
                    |> Seq.toList

                // Exactly one CustomAttribute row in the delta, updating in place.
                let rowId = Assert.Single caMapEntries &&& 0x00FFFFFF
                Assert.Equal(1, reader.GetTableRowCount(TableIndex.CustomAttribute))

                // The fresh compile's Obsolete row is the FIRST method-parented CA row of
                // the baseline (assembly-level rows precede it); pinning rowId > 0 and the
                // row content keeps this robust without hardcoding the absolute id.
                Assert.True(rowId > 0)

                let attr = reader.GetCustomAttribute(MetadataTokens.CustomAttributeHandle 1)
                Assert.Equal(HandleKind.MethodDefinition, attr.Parent.Kind)
                Assert.False(attr.Value.IsNil))

    [<Fact>]
    let ``Attribute removal delta zeroes parent and constructor columns`` () =
        emitDeltaAndInspect
            fullCapabilities
            "attr-remove-template"
            attributedBaseline
            moduleBaseline
            (fun delta ->
                use provider = MetadataReaderProvider.FromMetadataImage(ImmutableArray.ToImmutableArray(delta.Metadata: byte[]))
                let reader = provider.GetMetadataReader()

                Assert.Equal(1, reader.GetTableRowCount(TableIndex.CustomAttribute))

                let attr = reader.GetCustomAttribute(MetadataTokens.CustomAttributeHandle 1)
                Assert.True(attr.Parent.IsNil)
                Assert.True(attr.Constructor.IsNil)
                Assert.True(attr.Value.IsNil))

    [<Fact>]
    let ``EmitDelta rejects attribute change without ChangeCustomAttributes`` () =
        let updated = attributedBaseline.Replace("\"a\"", "\"b\"")

        emitDeltaAndExpectUnsupported
            [ "Baseline"; "AddMethodToExistingType" ]
            "attr-change-no-capability"
            attributedBaseline
            updated
            [ "ChangeCustomAttributes" ]

    [<Fact>]
    let ``EmitDelta rejects attribute change on a property-backed module value`` () =
        // Module values route their attributes to the Property row, which the delta
        // writer cannot update yet: fail closed instead of applying without the change.
        let baseline =
            """
namespace Sample

module Library =
    [<System.Obsolete("a")>]
    let answer = 41
"""

        let updated = baseline.Replace("\"a\"", "\"b\"")

        emitDeltaAndExpectUnsupported
            fullCapabilities
            "attr-change-module-value"
            baseline
            updated
            [ "Property or Event row" ]

    // -----------------------------------------------------------------------------
    // Consolidation: multi-generation chaining and disk-started sessions
    // -----------------------------------------------------------------------------

    [<Fact>]
    let ``Attribute add then change chains across generations`` () =
        // Generation 1 APPENDS the CA row (attr_add template); generation 2 must UPDATE
        // the generation-1 row chained into the baseline CustomAttributeRows map —
        // appending again would surface two attributes through reflection.
        let gen1 =
            """
namespace Sample

module Library =
    [<System.Obsolete("a")>]
    let existing (x: int) = x + 1
"""

        let gen2 = gen1.Replace("\"a\"", "\"b\"")

        applyGenerationsAndVerify
            fullCapabilities
            "attr-add-then-change"
            moduleBaseline
            (fun assembly ->
                let libType = assembly.GetType("Sample.Library", throwOnError = true)
                let existing = libType.GetMethod("existing", BindingFlags.Public ||| BindingFlags.Static)
                Assert.Empty(existing.GetCustomAttributes(typeof<ObsoleteAttribute>, false)))
            [ gen1,
              (fun assembly ->
                  let libType = assembly.GetType("Sample.Library", throwOnError = true)
                  let existing = libType.GetMethod("existing", BindingFlags.Public ||| BindingFlags.Static)

                  let obsolete =
                      existing.GetCustomAttributes(typeof<ObsoleteAttribute>, false)
                      |> Seq.cast<ObsoleteAttribute>
                      |> Seq.exactlyOne

                  Assert.Equal("a", obsolete.Message))
              gen2,
              (fun assembly ->
                  let libType = assembly.GetType("Sample.Library", throwOnError = true)
                  let existing = libType.GetMethod("existing", BindingFlags.Public ||| BindingFlags.Static)

                  let obsolete =
                      existing.GetCustomAttributes(typeof<ObsoleteAttribute>, false)
                      |> Seq.cast<ObsoleteAttribute>
                      |> Seq.exactlyOne

                  Assert.Equal("b", obsolete.Message)) ]

    [<Fact>]
    let ``Disk-started session applies an attribute change on an existing method`` () =
        // dotnet-watch topology: the baseline (including the CustomAttribute row
        // snapshot driving in-place updates) is reconstructed from the on-disk dll +
        // pdb only.
        let updated = attributedBaseline.Replace("\"a\"", "\"b\"")

        applyGenerationsAndVerifyCore
            true
            fullCapabilities
            "disk-started-attr-change"
            attributedBaseline
            (fun assembly ->
                let libType = assembly.GetType("Sample.Library", throwOnError = true)
                let existing = libType.GetMethod("existing", BindingFlags.Public ||| BindingFlags.Static)
                Assert.NotEmpty(existing.GetCustomAttributes(typeof<ObsoleteAttribute>, false)))
            [ updated,
              (fun assembly ->
                  let libType = assembly.GetType("Sample.Library", throwOnError = true)
                  let existing = libType.GetMethod("existing", BindingFlags.Public ||| BindingFlags.Static)

                  let obsolete =
                      existing.GetCustomAttributes(typeof<ObsoleteAttribute>, false)
                      |> Seq.cast<ObsoleteAttribute>
                      |> Seq.exactlyOne

                  Assert.Equal("b", obsolete.Message)) ]
