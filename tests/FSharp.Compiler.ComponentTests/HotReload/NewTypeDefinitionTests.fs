namespace FSharp.Compiler.ComponentTests.HotReload

// Phase F: user-defined new type definitions (NewTypeDefinition).
//
// C# reference template (csharp_enc_reference `phasef` scenario 'new_class', Roslyn
// EmitDifference; mdv recording reference_mdv_new_class.txt): adding a top-level class
// implementing an interface and using it from an edited method emits, in generation 1:
//   - the new TypeDef row as a plain Default EncLog entry (EnclosingType nil for a
//     top-level type — no NestedClass row),
//   - (TypeDef, AddField) + (Field, Default) and (TypeDef, AddMethod) + (MethodDef,
//     Default) pairs for the class's field/ctor/methods, AddParameter pairs for their
//     parameters,
//   - the updated using method as a MethodDef/Param row update,
//   - an InterfaceImpl row as a plain Default entry TRAILING the log (EncMap add).
// Generation 2 (body edit of a method of the added class) is a plain MethodDef update
// of the generation-1 row: no TypeDef/Field/InterfaceImpl rows.

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
module NewTypeDefinitionTests =

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

            let projectDir = Path.Combine(Path.GetTempPath(), "fsharp-hotreload-new-types", Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(projectDir) |> ignore
            let fsPath = Path.Combine(projectDir, "Library.fs")
            let dllPath = Path.Combine(projectDir, "Library.dll")
            let runtimeDllPath = Path.Combine(projectDir, "Library.runtime.dll")
            let loadContext = new AssemblyLoadContext($"fsharp-hotreload-newtypes-{Guid.NewGuid():N}", isCollectible = true)

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
                    // Simulate the process boundary (dotnet-watch topology): drop ALL
                    // in-process session state so the session is reconstructed from the
                    // on-disk dll + pdb only.
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

        let projectDir = Path.Combine(Path.GetTempPath(), "fsharp-hotreload-new-types", Guid.NewGuid().ToString("N"))
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

    let private emitDeltaAndInspect capabilities testLabel baselineSource updatedSource (inspect: FSharpHotReloadDelta -> unit) =
        emitDeltaAndHandleResult capabilities testLabel baselineSource updatedSource (fun result ->
            match result with
            | Error error -> failwithf "[%s] EmitHotReloadDelta failed: %A" testLabel error
            | Ok delta -> inspect delta)

    let private emitDeltaAndExpectUnsupported capabilities testLabel baselineSource updatedSource (expectedMessageParts: string list) =
        emitDeltaAndHandleResult capabilities testLabel baselineSource updatedSource (fun result ->
            match result with
            | Ok _ -> failwithf "[%s] expected EmitHotReloadDelta to fail, but it succeeded." testLabel
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

    let private classAdded =
        """
namespace Sample

type Helper(seed: int) =
    member _.Boost(value: int) = value + seed
    interface System.IDisposable with
        member _.Dispose() = ()

module Library =
    let compute (x: int) =
        use helper = new Helper(10)
        helper.Boost(x) + 1
"""

    let private classAddedGen2 =
        """
namespace Sample

type Helper(seed: int) =
    member _.Boost(value: int) = value + seed + 100
    interface System.IDisposable with
        member _.Dispose() = ()

module Library =
    let compute (x: int) =
        use helper = new Helper(10)
        helper.Boost(x) + 1
"""

    let private recordAdded =
        """
namespace Sample

type Point = { X: int; Y: int }

module Library =
    let compute (x: int) =
        let point = { X = x; Y = 10 }
        point.X + point.Y + 1
"""

    // -----------------------------------------------------------------------------
    // Runtime tests
    // -----------------------------------------------------------------------------

    [<Fact>]
    let ``ApplyUpdate succeeds for added class implementing an interface`` () =
        // Generation 1: a brand-new top-level class (ctor + member + explicit
        // IDisposable implementation -> TypeDef + AddMethod pairs + InterfaceImpl +
        // MethodImpl rows) used from the edited module function. Generation 2
        // body-edits the added class's member: a plain MethodDef update of the
        // generation-1 row (C# 'new_class' gen-2 parity).
        applyGenerationsAndVerify
            fullCapabilities
            "added-class-with-interface"
            baseline
            (fun assembly ->
                let libType = assembly.GetType("Sample.Library", throwOnError = true)
                let compute = libType.GetMethod("compute", BindingFlags.Public ||| BindingFlags.Static)
                Assert.Equal(42, compute.Invoke(null, [| box 41 |]) :?> int))
            [ classAdded,
              (fun assembly ->
                  let libType = assembly.GetType("Sample.Library", throwOnError = true)
                  let compute = libType.GetMethod("compute", BindingFlags.Public ||| BindingFlags.Static)

                  // compute now routes through the added class: (41 + 10) + 1.
                  Assert.Equal(52, compute.Invoke(null, [| box 41 |]) :?> int)

                  // The added type is reachable through reflection, instantiable, and
                  // carries its interface implementation.
                  let helperType = assembly.GetType("Sample.Helper", throwOnError = true)
                  let helper = Activator.CreateInstance(helperType, box 5)
                  Assert.Equal(7, helperType.GetMethod("Boost").Invoke(helper, [| box 2 |]) :?> int)
                  Assert.True(typeof<IDisposable>.IsAssignableFrom helperType)
                  (helper :?> IDisposable).Dispose())
              classAddedGen2,
              (fun assembly ->
                  let libType = assembly.GetType("Sample.Library", throwOnError = true)
                  let compute = libType.GetMethod("compute", BindingFlags.Public ||| BindingFlags.Static)
                  Assert.Equal(152, compute.Invoke(null, [| box 41 |]) :?> int)) ]

    [<Fact>]
    let ``ApplyUpdate succeeds for added record used from an edited function`` () =
        // A record brings synthesized members (accessors, comparers, equality,
        // CompilationMapping attributes) and InterfaceImpl rows (IEquatable<Point>,
        // IStructuralEquatable, IComparable, ...). The delta must carry the complete
        // fresh-compile shape — additions never emit partially.
        applyGenerationsAndVerify
            fullCapabilities
            "added-record"
            baseline
            (fun assembly ->
                let libType = assembly.GetType("Sample.Library", throwOnError = true)
                let compute = libType.GetMethod("compute", BindingFlags.Public ||| BindingFlags.Static)
                Assert.Equal(42, compute.Invoke(null, [| box 41 |]) :?> int))
            [ recordAdded,
              (fun assembly ->
                  let libType = assembly.GetType("Sample.Library", throwOnError = true)
                  let compute = libType.GetMethod("compute", BindingFlags.Public ||| BindingFlags.Static)

                  // compute builds the record: 41 + 10 + 1.
                  Assert.Equal(52, compute.Invoke(null, [| box 41 |]) :?> int)

                  let pointType = assembly.GetType("Sample.Point", throwOnError = true)
                  Assert.True(typeof<IComparable>.IsAssignableFrom pointType)

                  let point = Activator.CreateInstance(pointType, box 3, box 4)
                  Assert.Equal(3, pointType.GetProperty("X").GetValue(point) :?> int)
                  Assert.Equal(4, pointType.GetProperty("Y").GetValue(point) :?> int)

                  // Synthesized structural equality works on the live type.
                  let samePoint = Activator.CreateInstance(pointType, box 3, box 4)
                  Assert.True(point.Equals samePoint)) ]

    [<Fact>]
    let ``ApplyUpdate succeeds for added union used from an edited function`` () =
        // A union brings nested case classes (detected as nested-in-added types ->
        // NestedClass rows), the Tags holder, DebugTypeProxy companions, synthesized
        // accessors/comparers and CompilationMapping/DebuggerDisplay attributes.
        let updated =
            """
namespace Sample

type Shape =
    | Circle of radius: int
    | Square of side: int

module Library =
    let compute (x: int) =
        let shape = Circle x
        match shape with
        | Circle r -> r + 1
        | Square s -> s * s
"""

        applyGenerationsAndVerify
            fullCapabilities
            "added-union"
            baseline
            (fun assembly ->
                let libType = assembly.GetType("Sample.Library", throwOnError = true)
                let compute = libType.GetMethod("compute", BindingFlags.Public ||| BindingFlags.Static)
                Assert.Equal(42, compute.Invoke(null, [| box 41 |]) :?> int))
            [ updated,
              (fun assembly ->
                  let libType = assembly.GetType("Sample.Library", throwOnError = true)
                  let compute = libType.GetMethod("compute", BindingFlags.Public ||| BindingFlags.Static)
                  Assert.Equal(42, compute.Invoke(null, [| box 41 |]) :?> int)

                  let shapeType = assembly.GetType("Sample.Shape", throwOnError = true)
                  let newCircle = shapeType.GetMethod("NewCircle", BindingFlags.Public ||| BindingFlags.Static)
                  let circle = newCircle.Invoke(null, [| box 7 |])
                  Assert.Equal(0, shapeType.GetProperty("Tag").GetValue(circle) :?> int)
                  Assert.True(shapeType.GetProperty("IsCircle").GetValue(circle) :?> bool)) ]

    // -----------------------------------------------------------------------------
    // Template-shape tests
    // -----------------------------------------------------------------------------

    [<Fact>]
    let ``Added class delta matches the C# new_class template shape`` () =
        emitDeltaAndInspect
            fullCapabilities
            "added-class-template"
            baseline
            classAdded
            (fun delta ->
                use provider = MetadataReaderProvider.FromMetadataImage(ImmutableArray.ToImmutableArray(delta.Metadata: byte[]))
                let reader = provider.GetMetadataReader()

                let encLog =
                    reader.GetEditAndContinueLogEntries()
                    |> Seq.map (fun entry ->
                        MetadataTokens.GetToken entry.Handle >>> 24, MetadataTokens.GetRowNumber entry.Handle, entry.Operation)
                    |> Seq.toList

                // The new TypeDef row is a plain Default entry preceding the AddMethod
                // pairs that name it as the parent.
                let typeDefDefaultIndex =
                    encLog |> List.findIndex (fun (table, _, op) -> table = 0x02 && op = EditAndContinueOperation.Default)

                let firstAddMethodIndex =
                    encLog |> List.findIndex (fun (table, _, op) -> table = 0x02 && op = EditAndContinueOperation.AddMethod)

                Assert.True(typeDefDefaultIndex < firstAddMethodIndex)

                // .ctor + Boost + Dispose -> three AddMethod pairs.
                let addMethodCount =
                    encLog
                    |> List.filter (fun (table, _, op) -> table = 0x02 && op = EditAndContinueOperation.AddMethod)
                    |> List.length

                Assert.Equal(3, addMethodCount)

                // InterfaceImpl and MethodImpl rows are plain Default adds in the log and
                // EncMap (C# template entry 16: InterfaceImpl 0x09000001 trailing).
                Assert.Contains(encLog, fun (table, _, op) -> table = 0x09 && op = EditAndContinueOperation.Default)
                Assert.Contains(encLog, fun (table, _, op) -> table = 0x19 && op = EditAndContinueOperation.Default)
                Assert.Equal(1, reader.GetTableRowCount(TableIndex.InterfaceImpl))
                Assert.Equal(1, reader.GetTableRowCount(TableIndex.MethodImpl))

                let encMapTables =
                    reader.GetEditAndContinueMapEntries()
                    |> Seq.map (fun handle -> MetadataTokens.GetToken handle >>> 24)
                    |> Set.ofSeq

                Assert.Contains(0x02, encMapTables)
                Assert.Contains(0x09, encMapTables)
                Assert.Contains(0x19, encMapTables))

    [<Fact>]
    let ``Disk-started session applies an added class with interface`` () =
        // dotnet-watch topology: the session is reconstructed from the on-disk dll +
        // pdb only, and the new-type addition (TypeDef + InterfaceImpl + MethodImpl
        // rows) still applies.
        applyGenerationsAndVerifyCore
            true
            fullCapabilities
            "disk-started-added-class"
            baseline
            (fun assembly ->
                let libType = assembly.GetType("Sample.Library", throwOnError = true)
                let compute = libType.GetMethod("compute", BindingFlags.Public ||| BindingFlags.Static)
                Assert.Equal(42, compute.Invoke(null, [| box 41 |]) :?> int))
            [ classAdded,
              (fun assembly ->
                  let libType = assembly.GetType("Sample.Library", throwOnError = true)
                  let compute = libType.GetMethod("compute", BindingFlags.Public ||| BindingFlags.Static)
                  Assert.Equal(52, compute.Invoke(null, [| box 41 |]) :?> int)

                  let helperType = assembly.GetType("Sample.Helper", throwOnError = true)
                  let helper = Activator.CreateInstance(helperType, box 5)
                  Assert.Equal(7, helperType.GetMethod("Boost").Invoke(helper, [| box 2 |]) :?> int)
                  Assert.True(typeof<IDisposable>.IsAssignableFrom helperType)) ]

    // -----------------------------------------------------------------------------
    // Negative tests
    // -----------------------------------------------------------------------------

    [<Fact>]
    let ``EmitHotReloadDelta rejects added type without NewTypeDefinition`` () =
        emitDeltaAndExpectUnsupported
            [ "Baseline"; "AddMethodToExistingType"; "AddStaticFieldToExistingType"; "AddInstanceFieldToExistingType" ]
            "added-class-no-capability"
            baseline
            classAdded
            [ "NewTypeDefinition" ]

    [<Fact>]
    let ``EmitHotReloadDelta rejects added interface declaration`` () =
        // Interfaces have abstract slots without bodies; the writer cannot express
        // bodiless added methods, so interface additions fail closed at classification.
        let updated =
            """
namespace Sample

type IShape =
    abstract member Area: unit -> int

module Library =
    let compute (x: int) = x + 1
"""

        emitDeltaAndExpectUnsupported
            fullCapabilities
            "added-interface"
            baseline
            updated
            [ "only classes, records, unions, and structs" ]
