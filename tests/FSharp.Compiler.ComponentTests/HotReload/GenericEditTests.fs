#nowarn "57" // experimental FCS hot reload session API

namespace FSharp.Compiler.ComponentTests.HotReload

// Generic method and generic-type-member edits.
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
module GenericEditTests =

    /// Snapshot of the project's CURRENT on-disk state (snapshots are immutable and
    /// content-versioned, so each edit needs a fresh one).
    let private snapshotOf (projectOptions: FSharpProjectOptions) =
        FSharpProjectSnapshot.FromOptions(projectOptions, DocumentSource.FileSystem)
        |> Async.RunImmediate

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
    /// `diskStartedSession` simulates the dotnet-watch topology: ALL in-process session
    /// state left by the baseline capture compile is dropped before the session starts,
    /// so the on-disk dll + pdb are the only baseline inputs.
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

                if diskStartedSession then
                    // Simulate the process boundary (dotnet-watch topology): drop ALL
                    // in-process session state the capture compile created so the session
                    // is reconstructed from the on-disk dll + pdb only.
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

    /// Compiles `baselineSource`, starts a session with the given capabilities, applies
    /// `updatedSource` and hands the EmitDelta result to `handleResult`.
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

    /// Asserts EmitDelta fails with a message containing every fragment of
    /// `expectedMessageParts`.
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

    /// Asserts EmitDelta succeeds and hands the delta to `inspect`.
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
        // remapping in the delta (the content-validated MethodSpec/TypeSpec machinery).
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
    let ``ApplyUpdate succeeds for added generic module function`` () =
        // The added method is generic: the delta must carry a GenericParam row for 'T
        // (and one per typar) parented to the NEW MethodDef row — the C# reference
        // template ('generic_method_add') shape. Without those rows the added method's
        // metadata is corrupt (MakeGenericMethod throws).
        let updated =
            """
namespace Sample

module Lib =
    let describe<'T> (value: 'T) = "Hello " + value.ToString()
    let pair (x: 'a) (y: 'b) = (x, y)
"""

        applyGenerationsAndVerify
            fullCapabilities
            "added-generic-function"
            genericFunctionBaseline
            (fun assembly -> invokeDescribe assembly "Hello ")
            [ updated,
              (fun assembly ->
                  // Baseline members still work after the update.
                  invokeDescribe assembly "Hello "
                  let libType = assembly.GetType("Sample.Lib", throwOnError = true)
                  let pairDef = libType.GetMethod("pair", BindingFlags.Public ||| BindingFlags.Static)
                  Assert.True(pairDef.IsGenericMethodDefinition)

                  let genericArgs = pairDef.GetGenericArguments()
                  Assert.Equal(2, genericArgs.Length)
                  Assert.Equal("a", genericArgs.[0].Name)
                  Assert.Equal("b", genericArgs.[1].Name)

                  let forIntString = pairDef.MakeGenericMethod(typeof<int>, typeof<string>)
                  let result = forIntString.Invoke(null, [| box 1; box "a" |]) :?> int * string
                  Assert.Equal((1, "a"), result)

                  let forStringInt = pairDef.MakeGenericMethod(typeof<string>, typeof<int>)
                  let result2 = forStringInt.Invoke(null, [| box "b"; box 2 |]) :?> string * int
                  Assert.Equal(("b", 2), result2)) ]

    [<Fact>]
    let ``ApplyUpdate succeeds for method added to generic class`` () =
        // The added method itself is NOT generic; its signature references the declaring
        // type's VAR (!0) and its body reaches the field through the TypeSpec
        // self-instantiation MemberRef. The C# reference template ('generic_class_add')
        // shows an ordinary AddMethod pair with NO GenericParam rows.
        let updated =
            """
namespace Sample

type Container<'T>(value: 'T) =
    member _.Describe() = "Hello " + value.ToString()
    member _.DescribeAgain() = "Again " + value.ToString()
"""

        applyGenerationsAndVerify
            fullCapabilities
            "added-method-on-generic-class"
            genericClassBaseline
            (fun assembly -> invokeContainerDescribe assembly "Hello ")
            [ updated,
              (fun assembly ->
                  invokeContainerDescribe assembly "Hello "
                  let containerDef = assembly.GetType("Sample.Container`1", throwOnError = true)

                  let intContainer = Activator.CreateInstance(containerDef.MakeGenericType(typeof<int>), box 42)
                  let describeAgain = intContainer.GetType().GetMethod("DescribeAgain")
                  Assert.Equal("Again 42", describeAgain.Invoke(intContainer, [||]) :?> string)

                  let stringContainer = Activator.CreateInstance(containerDef.MakeGenericType(typeof<string>), box "watch")
                  let describeAgainString = stringContainer.GetType().GetMethod("DescribeAgain")
                  Assert.Equal("Again watch", describeAgainString.Invoke(stringContainer, [||]) :?> string)) ]

    [<Fact>]
    let ``ApplyUpdate succeeds for added lambda creating a generic closure class`` () =
        // The added lambda lives inside a generic member and mentions 'T, so its closure
        // class is generic over 'T: the delta's new TypeDef row carries a GenericParam
        // row (TypeOrMethodDef owner, TypeDef tag).
        // The baseline member already contains one lambda (the occurrence mapping needs a
        // baseline chain table for the member; first-lambda members stay fail-closed).
        let baseline =
            """
namespace Sample

module Lib =
    let describe<'T> (value: 'T) =
        let format = fun (x: 'T) -> "Hello " + x.ToString()
        format value
"""

        let updated =
            """
namespace Sample

module Lib =
    let describe<'T> (value: 'T) =
        let format = fun (x: 'T) -> "Hello " + x.ToString()
        let again = fun (x: 'T) -> " and again " + x.ToString()
        format value + again value
"""

        applyGenerationsAndVerify
            fullCapabilities
            "added-generic-closure-class"
            baseline
            (fun assembly -> invokeDescribe assembly "Hello ")
            [ updated,
              (fun assembly ->
                  let libType = assembly.GetType("Sample.Lib", throwOnError = true)
                  let methodDef = libType.GetMethod("describe", BindingFlags.Public ||| BindingFlags.Static)

                  let forInt = methodDef.MakeGenericMethod(typeof<int>)
                  Assert.Equal("Hello 42 and again 42", forInt.Invoke(null, [| box 42 |]) :?> string)

                  let forString = methodDef.MakeGenericMethod(typeof<string>)
                  Assert.Equal("Hello watch and again watch", forString.Invoke(null, [| box "watch" |]) :?> string)) ]

    [<Fact>]
    let ``Added generic function delta matches the C# GenericParam template shape`` () =
        // Recorded C# reference (csharp_enc_reference 'generic_method_add', Roslyn
        // EmitDifference): EncLog carries (TypeDef, AddMethod) + (MethodDef, Default)
        // followed later by a plain (GenericParam, Default) entry; EncMap lists the
        // GenericParam row as an add. A generic body UPDATE must carry NO GenericParam
        // rows (they are baseline rows).
        let updated =
            """
namespace Sample

module Lib =
    let describe<'T> (value: 'T) = "Hello " + value.ToString()
    let pair (x: 'a) (y: 'b) = (x, y)
"""

        emitDeltaAndInspect
            fullCapabilities
            "added-generic-function-template"
            genericFunctionBaseline
            updated
            (fun delta ->
                use provider = MetadataReaderProvider.FromMetadataImage(ImmutableArray.ToImmutableArray(delta.Metadata: byte[]))
                let reader = provider.GetMetadataReader()

                let encLog =
                    reader.GetEditAndContinueLogEntries()
                    |> Seq.map (fun entry ->
                        let typ, row = MetadataTokens.GetToken entry.Handle >>> 24, MetadataTokens.GetRowNumber entry.Handle
                        typ, row, entry.Operation)
                    |> Seq.toList

                // AddMethod parent pair for the added generic method.
                Assert.Contains(
                    encLog,
                    fun (table, _, op) -> table = 0x02 && op = System.Reflection.Metadata.Ecma335.EditAndContinueOperation.AddMethod
                )

                // Plain Default GenericParam entries, one per typar of 'pair'.
                let genericParamEntries =
                    encLog
                    |> List.filter (fun (table, _, op) ->
                        table = 0x2A && op = System.Reflection.Metadata.Ecma335.EditAndContinueOperation.Default)

                Assert.Equal(2, genericParamEntries.Length)

                // EncMap lists the same GenericParam rows (token-sorted adds).
                let encMapGenericParams =
                    reader.GetEditAndContinueMapEntries()
                    |> Seq.map MetadataTokens.GetToken
                    |> Seq.filter (fun token -> token >>> 24 = 0x2A)
                    |> Seq.toList

                Assert.Equal(2, encMapGenericParams.Length)

                // The GenericParam rows are physically present in the delta tables.
                Assert.Equal(2, reader.GetTableRowCount(TableIndex.GenericParam)))

    [<Fact>]
    let ``Generic body update delta carries no GenericParam rows`` () =
        let updated = genericFunctionBaseline.Replace("Hello ", "Welcome ")

        emitDeltaAndInspect
            fullCapabilities
            "generic-body-update-template"
            genericFunctionBaseline
            updated
            (fun delta ->
                use provider = MetadataReaderProvider.FromMetadataImage(ImmutableArray.ToImmutableArray(delta.Metadata: byte[]))
                let reader = provider.GetMetadataReader()
                // C# template parity: GenericParam rows of UPDATED methods are baseline
                // rows and are never re-emitted.
                Assert.Equal(0, reader.GetTableRowCount(TableIndex.GenericParam)))

    [<Fact>]
    let ``EmitDelta rejects added generic function without GenericAddMethodToExistingType`` () =
        let updated =
            """
namespace Sample

module Lib =
    let describe<'T> (value: 'T) = "Hello " + value.ToString()
    let pair (x: 'a) (y: 'b) = (x, y)
"""

        emitDeltaAndExpectUnsupported
            [ "Baseline"; "AddMethodToExistingType"; "GenericUpdateMethod" ]
            "added-generic-function-no-capability"
            genericFunctionBaseline
            updated
            [ "GenericAddMethodToExistingType" ]

    [<Fact>]
    let ``ApplyUpdate succeeds for added generic function with constrained typar`` () =
        // The writer emits GenericParamConstraint rows (C# reference template
        // 'generic_constraint_add': a plain Default entry trailing the GenericParam
        // entry, EncMap add, Owner = the new GenericParam row, Constraint = the
        // interface TypeRef). Previously this failed closed.
        let updated =
            """
namespace Sample

module Lib =
    let describe<'T> (value: 'T) = "Hello " + value.ToString()
    let dispose<'T when 'T :> System.IDisposable> (x: 'T) =
        x.Dispose()
        "disposed"
"""

        applyGenerationsAndVerify
            fullCapabilities
            "added-constrained-generic-function"
            genericFunctionBaseline
            (fun assembly -> invokeDescribe assembly "Hello ")
            [ updated,
              (fun assembly ->
                  let libType = assembly.GetType("Sample.Lib", throwOnError = true)
                  let disposeDef = libType.GetMethod("dispose", BindingFlags.Public ||| BindingFlags.Static)
                  Assert.True(disposeDef.IsGenericMethodDefinition)

                  // The constraint row is live metadata: reflection reports the
                  // IDisposable constraint on the added method's type parameter.
                  let typar = Assert.Single(disposeDef.GetGenericArguments())
                  let constraints = typar.GetGenericParameterConstraints()
                  Assert.Contains(typeof<System.IDisposable>, constraints)

                  // And the constrained method executes.
                  let stream = new System.IO.MemoryStream()
                  let forStream = disposeDef.MakeGenericMethod(typeof<System.IO.MemoryStream>)
                  Assert.Equal("disposed", forStream.Invoke(null, [| box stream |]) :?> string)) ]

    [<Fact>]
    let ``EmitDelta rejects generic body edit without GenericUpdateMethod capability`` () =
        // Roslyn parity: updating a generic method requires the GenericUpdateMethod
        // runtime capability; a baseline-only session reports the rude edit naming it.
        let updated = genericFunctionBaseline.Replace("Hello ", "Welcome ")

        emitDeltaAndExpectUnsupported
            [ "Baseline" ]
            "generic-function-body-edit-no-capability"
            genericFunctionBaseline
            updated
            [ "GenericUpdateMethod" ]

    [<Fact>]
    let ``Disk-started session applies a generic method body edit`` () =
        // dotnet-watch topology: the session is reconstructed from the on-disk dll + pdb
        // only (no in-process baseline state) and a generic method body edit applies.
        let updated = genericFunctionBaseline.Replace("Hello ", "Welcome ")

        applyGenerationsAndVerifyCore
            true
            fullCapabilities
            "disk-started-generic-body-edit"
            genericFunctionBaseline
            (fun assembly -> invokeDescribe assembly "Hello ")
            [ updated, (fun assembly -> invokeDescribe assembly "Welcome ") ]

    [<Fact>]
    let ``Disk-started session applies an added generic module function`` () =
        let updated =
            """
namespace Sample

module Lib =
    let describe<'T> (value: 'T) = "Hello " + value.ToString()
    let pair (x: 'a) (y: 'b) = (x, y)
"""

        applyGenerationsAndVerifyCore
            true
            fullCapabilities
            "disk-started-added-generic-function"
            genericFunctionBaseline
            (fun assembly -> invokeDescribe assembly "Hello ")
            [ updated,
              (fun assembly ->
                  let libType = assembly.GetType("Sample.Lib", throwOnError = true)
                  let pairDef = libType.GetMethod("pair", BindingFlags.Public ||| BindingFlags.Static)
                  Assert.True(pairDef.IsGenericMethodDefinition)
                  let forIntString = pairDef.MakeGenericMethod(typeof<int>, typeof<string>)
                  let result = forIntString.Invoke(null, [| box 1; box "a" |]) :?> int * string
                  Assert.Equal((1, "a"), result)) ]
