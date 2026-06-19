#nowarn "57"

namespace FSharp.Compiler.Service.Tests.HotReload

open System
open System.IO
open Xunit

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.CodeAnalysis.ProjectSnapshot
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.EditAndContinue
open FSharp.Compiler.Text
open FSharp.Test
open FSharp.Test.Utilities

open FSharp.Compiler.Service.Tests.Common

/// Tests for the FSharpHotReloadSession entity (the F# DebuggingSession analogue):
/// independent session instances, per-project committed baselines and generation chains
/// inside one session, solution-wide commit/discard, and session-wide
/// capabilities/active statements.
[<Collection(nameof NotThreadSafeResourceCollection)>]
module HotReloadSessionTests =

    let private createChecker () =
        FSharpChecker.Create(
            keepAssemblyContents = true,
            keepAllBackgroundResolutions = false,
            keepAllBackgroundSymbolUses = false,
            enableBackgroundItemKeyStoreAndSemanticClassification = false,
            enablePartialTypeChecking = false,
            captureIdentifiersWhenParsing = false,
            useTransparentCompiler = CompilerAssertHelpers.UseTransparentCompiler
        )

    /// Project options with the output passed as `-o:` so the project identity
    /// (FSharpProjectIdentifier = projectFileName * "-o:" output) carries the output path.
    let private prepareProjectOptions
        (checker: FSharpChecker)
        (fsPath: string)
        (dllPath: string)
        (source: string)
        (extraOptions: string list)
        =
        let projectOptions, _ =
            checker.GetProjectOptionsFromScript(
                fsPath,
                SourceText.ofString source,
                assumeDotNetFramework = false,
                useSdkRefs = true,
                useFsiAuxLib = false
            )
            |> Async.RunImmediate

        { projectOptions with
            SourceFiles = [| fsPath |]
            OtherOptions =
                projectOptions.OtherOptions
                |> Array.append (List.toArray extraOptions)
                |> Array.append
                    [| "--target:library"
                       "--langversion:preview"
                       "--optimize-"
                       "--debug:portable"
                       "--deterministic"
                       "--test:HotReloadDeltas"
                       $"-o:{dllPath}" |] }

    let private compileProject
        (checker: FSharpChecker)
        (projectOptions: FSharpProjectOptions)
        (includeHotReloadCapture: bool)
        =
        let options =
            if includeHotReloadCapture then
                projectOptions.OtherOptions
            else
                projectOptions.OtherOptions
                |> Array.filter (fun opt ->
                    not (opt.StartsWith("--test:HotReloadDeltas", StringComparison.OrdinalIgnoreCase)))

        let argv =
            Array.concat [ [| "fsc.exe" |]; options; projectOptions.SourceFiles ]

        let diagnostics, exOpt = checker.Compile(argv) |> Async.RunImmediate

        let errors =
            diagnostics
            |> Array.filter (fun diagnostic -> diagnostic.Severity = FSharpDiagnosticSeverity.Error)

        match errors, exOpt with
        | [||], None -> ()
        | errs, _ -> failwithf "Compilation failed: %A" (errs |> Array.map (fun d -> d.Message))

    let private createProjectSnapshot (projectOptions: FSharpProjectOptions) =
        FSharpProjectSnapshot.FromOptions(projectOptions, DocumentSource.FileSystem)
        |> Async.RunImmediate

    let private withProjectDir (testName: string) (action: string -> unit) =
        let projectDir =
            Path.Combine(Path.GetTempPath(), testName, Guid.NewGuid().ToString("N"))

        Directory.CreateDirectory(projectDir) |> ignore

        try
            action projectDir
        finally
            try
                Directory.Delete(projectDir, true)
            with _ ->
                ()

    let private writeAndCompile (checker: FSharpChecker) (fsPath: string) (options: FSharpProjectOptions) (source: string) capture =
        File.WriteAllText(fsPath, source)
        checker.NotifyFileChanged(fsPath, options) |> Async.RunImmediate
        compileProject checker options capture

    let private addProjectOrFail (session: FSharpHotReloadSession) snapshot =
        match session.AddProject(snapshot) |> Async.RunImmediate with
        | Ok() -> ()
        | Error error -> failwithf "AddProject failed: %A" error

    let private emitOrFail (session: FSharpHotReloadSession) snapshot =
        match session.EmitDelta(snapshot) |> Async.RunImmediate with
        | Ok delta -> delta
        | Error error -> failwithf "EmitDelta failed: %A" error

    let private libSource (generation: int) =
        $"""
module SessionLib

let libValue () = "lib generation {generation}"
"""

    let private appSource (generation: int) =
        $"""
module SessionApp

let appValue () = "app generation {generation}"
"""

    [<Fact>]
    let ``Two independent sessions emit deltas without interference`` () =
        withProjectDir "fcs-hotreload-session-independent" (fun projectDir ->
            let fsPathA = Path.Combine(projectDir, "LibraryA.fs")
            let dllPathA = Path.Combine(projectDir, "LibraryA.dll")
            let fsPathB = Path.Combine(projectDir, "LibraryB.fs")
            let dllPathB = Path.Combine(projectDir, "LibraryB.dll")

            File.WriteAllText(fsPathA, libSource 0)
            File.WriteAllText(fsPathB, appSource 0)

            let checker = createChecker ()
            let optionsA = prepareProjectOptions checker fsPathA dllPathA (libSource 0) []
            let optionsB = prepareProjectOptions checker fsPathB dllPathB (appSource 0) []

            checker.InvalidateAll()
            compileProject checker optionsA true
            compileProject checker optionsB true

            use sessionA = checker.CreateHotReloadSession()
            use sessionB = checker.CreateHotReloadSession()

            let snapshotA = createProjectSnapshot optionsA
            let snapshotB = createProjectSnapshot optionsB
            addProjectOrFail sessionA snapshotA
            addProjectOrFail sessionB snapshotB

            // Each session tracks exactly its own project, keyed by the snapshot identity.
            Assert.Equal<FSharpProjectIdentifier list>([ snapshotA.Identifier ], sessionA.ProjectIdentifiers)
            Assert.Equal<FSharpProjectIdentifier list>([ snapshotB.Identifier ], sessionB.ProjectIdentifiers)

            // Edit + emit on session A.
            writeAndCompile checker fsPathA optionsA (libSource 1) false
            let deltaA1 = emitOrFail sessionA (createProjectSnapshot optionsA)
            Assert.NotEmpty(deltaA1.Metadata)
            Assert.NotEmpty(deltaA1.UpdatedMethods)
            sessionA.Commit()

            // Edit + emit on session B — unaffected by session A's activity.
            writeAndCompile checker fsPathB optionsB (appSource 1) false
            let deltaB1 = emitOrFail sessionB (createProjectSnapshot optionsB)
            Assert.NotEmpty(deltaB1.Metadata)
            Assert.NotEmpty(deltaB1.UpdatedMethods)
            sessionB.Commit()

            // Session A chains its own generations: the next delta builds on A's gen-1 id even
            // though session B emitted in between.
            writeAndCompile checker fsPathA optionsA (libSource 2) false
            let deltaA2 = emitOrFail sessionA (createProjectSnapshot optionsA)
            Assert.Equal(deltaA1.GenerationId, deltaA2.BaseGenerationId)
            sessionA.Commit())

    [<Fact>]
    let ``Disposing a session ends it without affecting other sessions`` () =
        withProjectDir "fcs-hotreload-session-dispose" (fun projectDir ->
            let fsPathA = Path.Combine(projectDir, "LibraryA.fs")
            let dllPathA = Path.Combine(projectDir, "LibraryA.dll")
            let fsPathB = Path.Combine(projectDir, "LibraryB.fs")
            let dllPathB = Path.Combine(projectDir, "LibraryB.dll")

            File.WriteAllText(fsPathA, libSource 0)
            File.WriteAllText(fsPathB, appSource 0)

            let checker = createChecker ()
            let optionsA = prepareProjectOptions checker fsPathA dllPathA (libSource 0) []
            let optionsB = prepareProjectOptions checker fsPathB dllPathB (appSource 0) []

            checker.InvalidateAll()
            compileProject checker optionsA true
            compileProject checker optionsB true

            let sessionA = checker.CreateHotReloadSession()
            use sessionB = checker.CreateHotReloadSession()

            addProjectOrFail sessionA (createProjectSnapshot optionsA)
            addProjectOrFail sessionB (createProjectSnapshot optionsB)

            (sessionA :> IDisposable).Dispose()

            // The disposed session refuses further work...
            Assert.Throws<ObjectDisposedException>(fun () -> sessionA.Commit()) |> ignore
            Assert.Throws<ObjectDisposedException>(fun () -> sessionA.ProjectIdentifiers |> ignore)
            |> ignore

            // ...while the other session keeps emitting.
            writeAndCompile checker fsPathB optionsB (appSource 1) false
            let deltaB = emitOrFail sessionB (createProjectSnapshot optionsB)
            Assert.NotEmpty(deltaB.Metadata)
            sessionB.Commit())

    [<Fact>]
    let ``EmitDelta for an untracked project returns NoActiveSession`` () =
        withProjectDir "fcs-hotreload-session-untracked" (fun projectDir ->
            let fsPath = Path.Combine(projectDir, "Library.fs")
            let dllPath = Path.Combine(projectDir, "Library.dll")

            File.WriteAllText(fsPath, libSource 0)

            let checker = createChecker ()
            let options = prepareProjectOptions checker fsPath dllPath (libSource 0) []

            checker.InvalidateAll()
            compileProject checker options true

            use session = checker.CreateHotReloadSession()

            // No AddProject: emitting is unrepresentable as anything but an error.
            match session.EmitDelta(createProjectSnapshot options) |> Async.RunImmediate with
            | Error FSharpHotReloadError.NoActiveSession -> ()
            | Error other -> failwithf "Expected NoActiveSession, got %A" other
            | Ok _ -> failwith "Expected EmitDelta to fail for a project the session does not track.")

    [<Fact>]
    let ``Multi-project session keeps independent baselines and generation chains`` () =
        withProjectDir "fcs-hotreload-session-multiproject" (fun projectDir ->
            let libFsPath = Path.Combine(projectDir, "SessionLib.fs")
            let libDllPath = Path.Combine(projectDir, "SessionLib.dll")
            let appFsPath = Path.Combine(projectDir, "SessionApp.fs")
            let appDllPath = Path.Combine(projectDir, "SessionApp.dll")

            let appReferencingSource (generation: int) =
                $"""
module SessionApp

let appValue () = "app generation {generation}: " + SessionLib.libValue ()
"""

            File.WriteAllText(libFsPath, libSource 0)
            File.WriteAllText(appFsPath, appReferencingSource 0)

            let checker = createChecker ()
            let libOptions = prepareProjectOptions checker libFsPath libDllPath (libSource 0) []

            // The app project references the library's built output on disk.
            let appOptions =
                prepareProjectOptions checker appFsPath appDllPath (appReferencingSource 0) [ $"-r:{libDllPath}" ]

            checker.InvalidateAll()
            compileProject checker libOptions true
            compileProject checker appOptions true

            use session = checker.CreateHotReloadSession()

            // One session, two projects (AddProject twice).
            addProjectOrFail session (createProjectSnapshot libOptions)
            addProjectOrFail session (createProjectSnapshot appOptions)
            Assert.Equal(2, session.ProjectIdentifiers.Length)

            // Edit each project; each delta is emitted against ITS OWN baseline.
            writeAndCompile checker libFsPath libOptions (libSource 1) false
            let libDelta1 = emitOrFail session (createProjectSnapshot libOptions)
            Assert.NotEmpty(libDelta1.UpdatedMethods)

            writeAndCompile checker appFsPath appOptions (appReferencingSource 1) false
            let appDelta1 = emitOrFail session (createProjectSnapshot appOptions)
            Assert.NotEmpty(appDelta1.UpdatedMethods)

            // Distinct projects, distinct generation ids and base chains.
            Assert.NotEqual<Guid>(libDelta1.GenerationId, appDelta1.GenerationId)
            Assert.NotEqual<Guid>(libDelta1.GenerationId, appDelta1.BaseGenerationId)

            // Solution-wide commit advances BOTH pending project updates together.
            session.Commit()

            // The library's next generation chains from the library's own gen-1 id —
            // interleaved app edits do not perturb the lib chain (and vice versa).
            writeAndCompile checker libFsPath libOptions (libSource 2) false
            let libDelta2 = emitOrFail session (createProjectSnapshot libOptions)
            Assert.Equal(libDelta1.GenerationId, libDelta2.BaseGenerationId)

            writeAndCompile checker appFsPath appOptions (appReferencingSource 2) false
            let appDelta2 = emitOrFail session (createProjectSnapshot appOptions)
            Assert.Equal(appDelta1.GenerationId, appDelta2.BaseGenerationId)

            session.Commit())

    [<Fact>]
    let ``Discard drops pending updates so the next emit re-diffs against the committed view`` () =
        withProjectDir "fcs-hotreload-session-discard" (fun projectDir ->
            let fsPath = Path.Combine(projectDir, "Library.fs")
            let dllPath = Path.Combine(projectDir, "Library.dll")

            File.WriteAllText(fsPath, libSource 0)

            let checker = createChecker ()
            let options = prepareProjectOptions checker fsPath dllPath (libSource 0) []

            checker.InvalidateAll()
            compileProject checker options true

            use session = checker.CreateHotReloadSession()
            addProjectOrFail session (createProjectSnapshot options)

            writeAndCompile checker fsPath options (libSource 1) false
            let delta1 = emitOrFail session (createProjectSnapshot options)

            // The host did not apply the update: discard it. The next emit diffs against the
            // unchanged committed baseline, so it builds on the same base generation.
            session.Discard()

            let delta2 = emitOrFail session (createProjectSnapshot options)
            Assert.Equal(delta1.BaseGenerationId, delta2.BaseGenerationId)

            session.Commit()

            // After the commit the next edit chains from the committed generation.
            writeAndCompile checker fsPath options (libSource 2) false
            let delta3 = emitOrFail session (createProjectSnapshot options)
            Assert.Equal(delta2.GenerationId, delta3.BaseGenerationId)

            session.Commit())

    [<Fact>]
    let ``Capabilities and active statements are session-wide across projects`` () =
        withProjectDir "fcs-hotreload-session-capabilities" (fun projectDir ->
            let genericFsPath = Path.Combine(projectDir, "GenericLib.fs")
            let genericDllPath = Path.Combine(projectDir, "GenericLib.dll")
            let plainFsPath = Path.Combine(projectDir, "PlainLib.fs")
            let plainDllPath = Path.Combine(projectDir, "PlainLib.dll")

            let genericSource (generation: int) =
                $"""
module GenericLib

type Calculator<'T>() =
    member _.Describe(value: 'T) = sprintf "generation {generation}: %%A" value
"""

            File.WriteAllText(genericFsPath, genericSource 0)
            File.WriteAllText(plainFsPath, libSource 0)

            let checker = createChecker ()
            let genericOptions = prepareProjectOptions checker genericFsPath genericDllPath (genericSource 0) []
            let plainOptions = prepareProjectOptions checker plainFsPath plainDllPath (libSource 0) []

            checker.InvalidateAll()
            compileProject checker genericOptions true
            compileProject checker plainOptions true

            // Created without capabilities: Roslyn-conservative BaselineOnly default.
            use session = checker.CreateHotReloadSession()
            addProjectOrFail session (createProjectSnapshot genericOptions)
            addProjectOrFail session (createProjectSnapshot plainOptions)

            // Body-editing a member of a generic type requires the GenericUpdateMethod
            // runtime capability; without it the edit is rude.
            writeAndCompile checker genericFsPath genericOptions (genericSource 1) false

            match session.EmitDelta(createProjectSnapshot genericOptions) |> Async.RunImmediate with
            | Error(FSharpHotReloadError.UnsupportedEdit _) -> ()
            | Error other -> failwithf "Expected UnsupportedEdit without GenericUpdateMethod, got %A" other
            | Ok _ -> failwith "Expected generic method edit to be rude under BaselineOnly capabilities."

            // One session-wide capability update unblocks the edit...
            session.UpdateCapabilities [ "GenericUpdateMethod" ]

            let genericDelta = emitOrFail session (createProjectSnapshot genericOptions)
            Assert.NotEmpty(genericDelta.UpdatedMethods)
            session.Commit()

            // ...and is visible through EVERY project's session view (session-wide state).
            let genericView =
                match session.TryGetProjectView(createProjectSnapshot genericOptions) with
                | ValueSome view -> view
                | ValueNone -> failwith "Expected a session view for the generic project."

            let plainView =
                match session.TryGetProjectView(createProjectSnapshot plainOptions) with
                | ValueSome view -> view
                | ValueNone -> failwith "Expected a session view for the plain project."

            Assert.True(genericView.Capabilities.Supports EditAndContinueCapability.GenericUpdateMethod)
            Assert.Equal<EditAndContinueCapabilities>(genericView.Capabilities, plainView.Capabilities)

            // Active statements are session-wide too: one push is visible from both projects.
            let statement =
                {
                    ActiveInstruction =
                        {
                            Method = { Token = 0x06000001; Version = 1 }
                            ILOffset = 0
                        }
                    DocumentName = None
                    SourceSpan =
                        {
                            StartLine = 0
                            StartColumn = 0
                            EndLine = 0
                            EndColumn = 0
                        }
                    Flags =
                        {
                            FrameKind = FSharpActiveStatementFrameKind.Leaf
                            IsMethodUpToDate = true
                            IsPartiallyExecuted = false
                            IsNonUserCode = false
                            IsStale = false
                        }
                }

            session.SetActiveStatements [ statement ]

            let genericView =
                match session.TryGetProjectView(createProjectSnapshot genericOptions) with
                | ValueSome view -> view
                | ValueNone -> failwith "Expected a session view for the generic project."

            let plainView =
                match session.TryGetProjectView(createProjectSnapshot plainOptions) with
                | ValueSome view -> view
                | ValueNone -> failwith "Expected a session view for the plain project."

            Assert.Equal(1, genericView.ActiveStatements.Length)
            Assert.Equal(1, plainView.ActiveStatements.Length)

            // Clearing replaces the whole session-wide set.
            session.SetActiveStatements []

            let clearedView =
                match session.TryGetProjectView(createProjectSnapshot plainOptions) with
                | ValueSome view -> view
                | ValueNone -> failwith "Expected a session view for the plain project."

            Assert.Empty(clearedView.ActiveStatements))
