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

    let private moduleAdded =
        """
namespace Sample

module Tools =
    let mutable counter = 5
    let bump (x: int) =
        counter <- counter + x
        counter
    let peek (x: int) = counter + x

module Library =
    let compute (x: int) = Tools.peek x + 1
"""

    let private enumAdded =
        """
namespace Sample

type Color =
    | Red = 1
    | Green = 2
    | Blue = 4

module Library =
    let compute (x: int) = x + int Color.Green
"""

    let private moduleAddedGen2 =
        """
namespace Sample

module Tools =
    let mutable counter = 5
    let bump (x: int) =
        counter <- counter + x + 100
        counter
    let peek (x: int) = counter + x

module Library =
    let compute (x: int) = Tools.peek x + 1
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
                  Assert.True(shapeType.GetProperty("IsCircle").GetValue(circle) :?> bool)

                  // The Tags holder's literal fields carry Constant rows (the latent
                  // pre-Constant-table gap): their values decode through reflection.
                  let tagsType = shapeType.GetNestedType("Tags", BindingFlags.Public ||| BindingFlags.NonPublic)
                  Assert.True(not (isNull tagsType), "Shape.Tags holder type not found after ApplyUpdate.")
                  Assert.Equal(0, tagsType.GetField("Circle").GetRawConstantValue() :?> int)
                  Assert.Equal(1, tagsType.GetField("Square").GetRawConstantValue() :?> int)) ]

    [<Fact>]
    let ``ApplyUpdate succeeds for added enum used from an edited function`` () =
        // An added enum is a new TypeDef whose member fields are literals: each carries
        // its value in a Constant row (C# 'new_enum' reference template — the writer's
        // Constant table support). The edited method consumes the enum value.
        applyGenerationsAndVerify
            fullCapabilities
            "added-enum"
            baseline
            (fun assembly ->
                let libType = assembly.GetType("Sample.Library", throwOnError = true)
                let compute = libType.GetMethod("compute", BindingFlags.Public ||| BindingFlags.Static)
                Assert.Equal(42, compute.Invoke(null, [| box 41 |]) :?> int))
            [ enumAdded,
              (fun assembly ->
                  let libType = assembly.GetType("Sample.Library", throwOnError = true)
                  let compute = libType.GetMethod("compute", BindingFlags.Public ||| BindingFlags.Static)

                  // compute consumes the enum: 41 + int Color.Green.
                  Assert.Equal(43, compute.Invoke(null, [| box 41 |]) :?> int)

                  let colorType = assembly.GetType("Sample.Color", throwOnError = true)
                  Assert.True colorType.IsEnum

                  // The Constant rows decode on the live type: literal values and names
                  // round-trip through reflection and Enum.Parse.
                  Assert.Equal(2, colorType.GetField("Green").GetRawConstantValue() :?> int)
                  Assert.Equal(4, colorType.GetField("Blue").GetRawConstantValue() :?> int)
                  let parsed = Enum.Parse(colorType, "Blue")
                  Assert.Equal(4, Convert.ToInt32 parsed)
                  Assert.Equal("Red", Enum.GetName(colorType, box 1))) ]

    [<Fact>]
    let ``ApplyUpdate succeeds for added literal module value`` () =
        // [<Literal>] values lower to static literal fields (HasDefault) on the module
        // type: the addition needs a Constant row parented to the new Field row on an
        // EXISTING TypeDef — the non-enum Constant case.
        let updated =
            """
namespace Sample

module Library =
    [<Literal>]
    let Version = 7

    let compute (x: int) = x + Version
"""

        applyGenerationsAndVerify
            fullCapabilities
            "added-literal-value"
            baseline
            (fun assembly ->
                let libType = assembly.GetType("Sample.Library", throwOnError = true)
                let compute = libType.GetMethod("compute", BindingFlags.Public ||| BindingFlags.Static)
                Assert.Equal(42, compute.Invoke(null, [| box 41 |]) :?> int))
            [ updated,
              (fun assembly ->
                  let libType = assembly.GetType("Sample.Library", throwOnError = true)
                  let compute = libType.GetMethod("compute", BindingFlags.Public ||| BindingFlags.Static)

                  // The literal is inlined into the edited body (41 + 7), and the field's
                  // Constant row decodes through reflection.
                  Assert.Equal(48, compute.Invoke(null, [| box 41 |]) :?> int)

                  // CoreCLR reflection enumerates an EnC-ADDED literal field twice
                  // (RuntimeType PopulateFields runs both PopulateRtFields — the EnC
                  // AddField path creates a FieldDesc the VM enumerates as RtFieldInfo —
                  // and PopulateLiteralFields' metadata walk producing MdFieldInfo; a
                  // baseline literal has no FieldDesc so the paths never overlap). Same
                  // token, same row — a runtime quirk, not delta corruption — so
                  // GetField's AmbiguousMatchException is expected here; only the
                  // metadata-backed MdFieldInfo implements GetRawConstantValue.
                  let versionFields =
                      libType.GetFields(BindingFlags.Public ||| BindingFlags.Static)
                      |> Array.filter (fun field -> field.Name = "Version")

                  Assert.True(versionFields.Length > 0, "Version literal field not found after ApplyUpdate.")
                  Assert.Equal(1, versionFields |> Array.distinctBy (fun field -> field.MetadataToken) |> Array.length)
                  Assert.True(versionFields |> Array.forall (fun field -> field.IsLiteral))

                  // The Constant row decodes through the metadata-backed FieldInfo.
                  let rawValues =
                      versionFields
                      |> Array.choose (fun field ->
                          try
                              Some(field.GetRawConstantValue() :?> int)
                          with :? InvalidOperationException ->
                              None)

                  Assert.Contains(7, rawValues)) ]

    [<Fact>]
    let ``Added enum delta matches the C# new_enum template shape`` () =
        emitDeltaAndInspect
            fullCapabilities
            "added-enum-template"
            baseline
            enumAdded
            (fun delta ->
                use provider = MetadataReaderProvider.FromMetadataImage(ImmutableArray.ToImmutableArray(delta.Metadata: byte[]))
                let reader = provider.GetMetadataReader()

                let encLog =
                    reader.GetEditAndContinueLogEntries()
                    |> Seq.map (fun entry ->
                        MetadataTokens.GetToken entry.Handle >>> 24, MetadataTokens.GetRowNumber entry.Handle, entry.Operation)
                    |> Seq.toList

                printfn "[added-enum-template] EncLog: %A" encLog

                // The new TypeDef row is a plain Default entry preceding the AddField
                // pairs (value__ + Red/Green/Blue -> four pairs, C# template parity).
                let typeDefDefaultIndex =
                    encLog |> List.findIndex (fun (table, _, op) -> table = 0x02 && op = EditAndContinueOperation.Default)

                let firstAddFieldIndex =
                    encLog |> List.findIndex (fun (table, _, op) -> table = 0x02 && op = EditAndContinueOperation.AddField)

                Assert.True(typeDefDefaultIndex < firstAddFieldIndex)

                let addFieldCount =
                    encLog
                    |> List.filter (fun (table, _, op) -> table = 0x02 && op = EditAndContinueOperation.AddField)
                    |> List.length

                Assert.Equal(4, addFieldCount)

                // Three Constant rows (one per literal member, NOT value__) trail the log
                // as plain Default entries; EncMap lists them as adds.
                let constantEntries =
                    encLog
                    |> List.filter (fun (table, _, op) -> table = 0x0B && op = EditAndContinueOperation.Default)

                Assert.Equal(3, List.length constantEntries)
                Assert.Equal(3, reader.GetTableRowCount(TableIndex.Constant))

                let lastConstantIndex =
                    encLog |> List.findIndexBack (fun (table, _, _) -> table = 0x0B)

                let lastFieldIndex =
                    encLog |> List.findIndexBack (fun (table, _, _) -> table = 0x04)

                Assert.True(lastFieldIndex < lastConstantIndex)

                let encMapTables =
                    reader.GetEditAndContinueMapEntries()
                    |> Seq.map (fun handle -> MetadataTokens.GetToken handle >>> 24)
                    |> Set.ofSeq

                Assert.Contains(0x0B, encMapTables))

    [<Fact>]
    let ``ApplyUpdate succeeds for added interface implemented by an added class`` () =
        // The added interface's abstract slot has NO IL body: its MethodDef row ships
        // with RVA 0, exactly as Roslyn does (C# 'new_interface' reference template —
        // 'Area' at RVA 0x00000000, Abstract|Virtual). The added class implements it
        // explicitly (InterfaceImpl row Interface = the NEW delta TypeDef; MethodImpl
        // row Declaration = the NEW bodiless MethodDef), and the edited function
        // consumes it through the interface.
        let updated =
            """
namespace Sample

type IShape =
    abstract member Area: unit -> int

type Square(side: int) =
    member _.Side = side
    interface IShape with
        member _.Area() = side * side

module Library =
    let compute (x: int) =
        let shape = Square(x) :> IShape
        shape.Area() + 1
"""

        applyGenerationsAndVerify
            fullCapabilities
            "added-interface"
            baseline
            (fun assembly ->
                let libType = assembly.GetType("Sample.Library", throwOnError = true)
                let compute = libType.GetMethod("compute", BindingFlags.Public ||| BindingFlags.Static)
                Assert.Equal(42, compute.Invoke(null, [| box 41 |]) :?> int))
            [ updated,
              (fun assembly ->
                  let libType = assembly.GetType("Sample.Library", throwOnError = true)
                  let compute = libType.GetMethod("compute", BindingFlags.Public ||| BindingFlags.Static)

                  // compute routes through the interface dispatch: 6*6 + 1.
                  Assert.Equal(37, compute.Invoke(null, [| box 6 |]) :?> int)

                  let shapeInterface = assembly.GetType("Sample.IShape", throwOnError = true)
                  Assert.True shapeInterface.IsInterface

                  let squareType = assembly.GetType("Sample.Square", throwOnError = true)
                  Assert.True(shapeInterface.IsAssignableFrom squareType)

                  // Interface dispatch through the bodiless slot works on a live
                  // instance created via reflection.
                  let square = Activator.CreateInstance(squareType, box 5)
                  let area = shapeInterface.GetMethod("Area")
                  Assert.Equal(25, area.Invoke(square, [||]) :?> int)) ]

    [<Fact>]
    let ``ApplyUpdate succeeds for added delegate constructed and invoked`` () =
        // The added delegate is a sealed class whose .ctor/Invoke/BeginInvoke/EndInvoke
        // are runtime-implemented (ImplFlags CodeTypeMask = Runtime) with RVA 0 (C#
        // 'new_delegate' reference template). The construction lives in an ADDED module
        // (delegate construction synthesizes a closure class; nested in the added module
        // TypeDef it flows through the nested-in-added path) and the edited function
        // calls through it.
        let updated =
            """
namespace Sample

type Combiner = delegate of int * int -> int

module Ops =
    let add (a: int) (b: int) = a + b

    let combine (x: int) =
        let c = Combiner(add)
        c.Invoke(x, 1)

module Library =
    let compute (x: int) = Ops.combine x
"""

        applyGenerationsAndVerify
            fullCapabilities
            "added-delegate"
            baseline
            (fun assembly ->
                let libType = assembly.GetType("Sample.Library", throwOnError = true)
                let compute = libType.GetMethod("compute", BindingFlags.Public ||| BindingFlags.Static)
                Assert.Equal(42, compute.Invoke(null, [| box 41 |]) :?> int))
            [ updated,
              (fun assembly ->
                  let libType = assembly.GetType("Sample.Library", throwOnError = true)
                  let compute = libType.GetMethod("compute", BindingFlags.Public ||| BindingFlags.Static)

                  // compute routes through the delegate invocation: 41 + 1.
                  Assert.Equal(42, compute.Invoke(null, [| box 41 |]) :?> int)

                  let combinerType = assembly.GetType("Sample.Combiner", throwOnError = true)
                  Assert.True(combinerType.IsSubclassOf typeof<MulticastDelegate>)

                  // The runtime-implemented members drive a delegate created over the
                  // live added method.
                  let opsType = assembly.GetType("Sample.Ops", throwOnError = true)
                  let addMethod = opsType.GetMethod("add", BindingFlags.Public ||| BindingFlags.Static)
                  let combiner = Delegate.CreateDelegate(combinerType, addMethod)
                  Assert.Equal(9, combiner.DynamicInvoke(box 4, box 5) :?> int)) ]

    [<Fact>]
    let ``ApplyUpdate succeeds for added module with function and mutable value`` () =
        // The idiomatic F# new-type case (C# parity: adding a static class, which Roslyn
        // supports): an added module lowers to a sealed abstract static class TypeDef.
        // Its functions are AddMethod pairs on the new row; the mutable value lowers to a
        // property on the new row plus a backing field AND the startup-class constructor
        // on the BASELINE startup TypeDef (AddField/AddMethod on an existing type) — the
        // addition spans a new TypeDef and existing-type member additions in one delta.
        // Generation 2 body-edits the added module's function in place.
        applyGenerationsAndVerify
            fullCapabilities
            "added-module"
            baseline
            (fun assembly ->
                let libType = assembly.GetType("Sample.Library", throwOnError = true)
                let compute = libType.GetMethod("compute", BindingFlags.Public ||| BindingFlags.Static)
                Assert.Equal(42, compute.Invoke(null, [| box 41 |]) :?> int))
            [ moduleAdded,
              (fun assembly ->
                  let libType = assembly.GetType("Sample.Library", throwOnError = true)
                  let compute = libType.GetMethod("compute", BindingFlags.Public ||| BindingFlags.Static)

                  // compute routes through the added module: counter (5) + 41 + 1.
                  Assert.Equal(47, compute.Invoke(null, [| box 41 |]) :?> int)

                  let toolsType = assembly.GetType("Sample.Tools", throwOnError = true)

                  // The mutable value's initializer ran: the baseline startup class had
                  // no static state, so the ADDED startup constructor runs lazily on
                  // first field access (same semantics as the added-module-value test).
                  let counter = toolsType.GetProperty("counter", BindingFlags.Public ||| BindingFlags.Static)
                  Assert.Equal(5, counter.GetValue(null) :?> int)

                  // The added module's function mutates the added value.
                  let bump = toolsType.GetMethod("bump", BindingFlags.Public ||| BindingFlags.Static)
                  Assert.Equal(8, bump.Invoke(null, [| box 3 |]) :?> int)
                  Assert.Equal(8, counter.GetValue(null) :?> int)

                  // The property setter is wired to the same backing field.
                  counter.SetValue(null, box 10)
                  Assert.Equal(52, compute.Invoke(null, [| box 41 |]) :?> int))
              moduleAddedGen2,
              (fun assembly ->
                  let toolsType = assembly.GetType("Sample.Tools", throwOnError = true)
                  let counter = toolsType.GetProperty("counter", BindingFlags.Public ||| BindingFlags.Static)
                  let bump = toolsType.GetMethod("bump", BindingFlags.Public ||| BindingFlags.Static)

                  // Generation-1 state survives generation 2 (counter was set to 10),
                  // and the edited body adds 100: 10 + 1 + 100.
                  Assert.Equal(111, bump.Invoke(null, [| box 1 |]) :?> int)
                  Assert.Equal(111, counter.GetValue(null) :?> int)) ]

    [<Fact>]
    let ``ApplyUpdate succeeds for added module containing a nested module`` () =
        // Nested modules lower to nested static classes: the inner module's TypeDef is
        // detected as nested-in-added (parents visit first in collectTypeMappings) and
        // carries a NestedClass row pointing at the outer module's NEW delta TypeDef row.
        let updated =
            """
namespace Sample

module Tools =
    let double (x: int) = x * 2

    module Inner =
        let triple (x: int) = x * 3

module Library =
    let compute (x: int) = Tools.double x + Tools.Inner.triple x + 1
"""

        applyGenerationsAndVerify
            fullCapabilities
            "added-nested-module"
            baseline
            (fun assembly ->
                let libType = assembly.GetType("Sample.Library", throwOnError = true)
                let compute = libType.GetMethod("compute", BindingFlags.Public ||| BindingFlags.Static)
                Assert.Equal(42, compute.Invoke(null, [| box 41 |]) :?> int))
            [ updated,
              (fun assembly ->
                  let libType = assembly.GetType("Sample.Library", throwOnError = true)
                  let compute = libType.GetMethod("compute", BindingFlags.Public ||| BindingFlags.Static)

                  // 2*2 + 3*2 ... compute 2 = 4 + 6 + 1.
                  Assert.Equal(11, compute.Invoke(null, [| box 2 |]) :?> int)

                  let innerType = assembly.GetType("Sample.Tools+Inner", throwOnError = true)
                  let triple = innerType.GetMethod("triple", BindingFlags.Public ||| BindingFlags.Static)
                  Assert.Equal(9, triple.Invoke(null, [| box 3 |]) :?> int)) ]

    [<Fact>]
    let ``ApplyUpdate succeeds for module added inside an existing module`` () =
        // The added module's TypeDef nests in a BASELINE TypeDef: the NestedClass row's
        // EnclosingClass resolves to the existing module's baseline row.
        let updated =
            """
namespace Sample

module Library =
    module Helpers =
        let twice (x: int) = x * 2

    let compute (x: int) = Helpers.twice x + 1
"""

        applyGenerationsAndVerify
            fullCapabilities
            "module-added-inside-module"
            baseline
            (fun assembly ->
                let libType = assembly.GetType("Sample.Library", throwOnError = true)
                let compute = libType.GetMethod("compute", BindingFlags.Public ||| BindingFlags.Static)
                Assert.Equal(42, compute.Invoke(null, [| box 41 |]) :?> int))
            [ updated,
              (fun assembly ->
                  let libType = assembly.GetType("Sample.Library", throwOnError = true)
                  let compute = libType.GetMethod("compute", BindingFlags.Public ||| BindingFlags.Static)
                  Assert.Equal(83, compute.Invoke(null, [| box 41 |]) :?> int)

                  let helpersType = assembly.GetType("Sample.Library+Helpers", throwOnError = true)
                  let twice = helpersType.GetMethod("twice", BindingFlags.Public ||| BindingFlags.Static)
                  Assert.Equal(14, twice.Invoke(null, [| box 7 |]) :?> int)) ]

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
    let ``Added module delta matches the C# static-class template shape`` () =
        // C# parity: an added module is an added static class (no InterfaceImpl rows).
        // The new TypeDef row is a plain Default entry preceding its AddMethod pairs;
        // the mutable value's backing field and the startup constructor land as
        // AddField/AddMethod pairs on the BASELINE startup TypeDef in the same log.
        emitDeltaAndInspect
            fullCapabilities
            "added-module-template"
            baseline
            moduleAdded
            (fun delta ->
                use provider = MetadataReaderProvider.FromMetadataImage(ImmutableArray.ToImmutableArray(delta.Metadata: byte[]))
                let reader = provider.GetMetadataReader()

                let encLog =
                    reader.GetEditAndContinueLogEntries()
                    |> Seq.map (fun entry ->
                        MetadataTokens.GetToken entry.Handle >>> 24, MetadataTokens.GetRowNumber entry.Handle, entry.Operation)
                    |> Seq.toList

                printfn "[added-module-template] EncLog: %A" encLog

                let typeDefDefaultIndex =
                    encLog |> List.findIndex (fun (table, _, op) -> table = 0x02 && op = EditAndContinueOperation.Default)

                let firstAddMethodIndex =
                    encLog |> List.findIndex (fun (table, _, op) -> table = 0x02 && op = EditAndContinueOperation.AddMethod)

                Assert.True(typeDefDefaultIndex < firstAddMethodIndex)

                // get_counter + set_counter + bump + peek on the new module TypeDef, plus
                // the startup-class .cctor on the baseline startup TypeDef.
                let addMethodCount =
                    encLog
                    |> List.filter (fun (table, _, op) -> table = 0x02 && op = EditAndContinueOperation.AddMethod)
                    |> List.length

                Assert.Equal(5, addMethodCount)

                // counter@ backing field + init@ sentinel on the baseline startup class.
                let addFieldCount =
                    encLog
                    |> List.filter (fun (table, _, op) -> table = 0x02 && op = EditAndContinueOperation.AddField)
                    |> List.length

                Assert.Equal(2, addFieldCount)

                // The module value surfaces as a property of the new TypeDef: a fresh
                // PropertyMap row (Default) is the AddProperty PARENT, followed by the
                // Property row (CLR Add* pairing; the parent is the map row, not the
                // TypeDef).
                Assert.Contains(encLog, fun (table, _, op) -> table = 0x15 && op = EditAndContinueOperation.AddProperty)
                Assert.Contains(encLog, fun (table, _, op) -> table = 0x17 && op = EditAndContinueOperation.Default)

                // A static class implements nothing: no InterfaceImpl/MethodImpl rows.
                Assert.Equal(0, reader.GetTableRowCount(TableIndex.InterfaceImpl))
                Assert.Equal(0, reader.GetTableRowCount(TableIndex.MethodImpl)))

    [<Fact>]
    let ``Disk-started session applies an added module`` () =
        // dotnet-watch topology: the session is reconstructed from the on-disk dll +
        // pdb only, and the added module (new TypeDef + startup-class member additions)
        // still applies.
        applyGenerationsAndVerifyCore
            true
            fullCapabilities
            "disk-started-added-module"
            baseline
            (fun assembly ->
                let libType = assembly.GetType("Sample.Library", throwOnError = true)
                let compute = libType.GetMethod("compute", BindingFlags.Public ||| BindingFlags.Static)
                Assert.Equal(42, compute.Invoke(null, [| box 41 |]) :?> int))
            [ moduleAdded,
              (fun assembly ->
                  let libType = assembly.GetType("Sample.Library", throwOnError = true)
                  let compute = libType.GetMethod("compute", BindingFlags.Public ||| BindingFlags.Static)
                  Assert.Equal(47, compute.Invoke(null, [| box 41 |]) :?> int)

                  let toolsType = assembly.GetType("Sample.Tools", throwOnError = true)
                  let counter = toolsType.GetProperty("counter", BindingFlags.Public ||| BindingFlags.Static)
                  Assert.Equal(5, counter.GetValue(null) :?> int)) ]

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
    let ``EmitHotReloadDelta rejects added module without NewTypeDefinition`` () =
        // The module's TypeDef is a new type definition: without the capability the
        // entity addition is NotSupportedByRuntime naming NewTypeDefinition, even though
        // the member-addition capabilities are granted.
        emitDeltaAndExpectUnsupported
            [ "Baseline"; "AddMethodToExistingType"; "AddStaticFieldToExistingType"; "AddInstanceFieldToExistingType" ]
            "added-module-no-capability"
            baseline
            moduleAdded
            [ "NewTypeDefinition" ]

    [<Fact>]
    let ``EmitHotReloadDelta rejects added type abbreviation`` () =
        // Type abbreviations are fully erased (no TypeDef); the entity diff cannot
        // classify them as type additions and fails closed with the precise list of
        // supported representations.
        let updated =
            """
namespace Sample

type Count = int

module Library =
    let compute (x: int) = x + 1
"""

        emitDeltaAndExpectUnsupported
            fullCapabilities
            "added-abbreviation"
            baseline
            updated
            [ "only classes, records, unions, structs, enums, interfaces, delegates, and modules" ]
