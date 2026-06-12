#nowarn "57"

namespace FSharp.Compiler.Service.Tests.HotReload

open System
open System.IO
open System.Reflection.Metadata
open System.Reflection.Metadata.Ecma335
open System.Reflection.PortableExecutable
open Xunit

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Text
open FSharp.Test
open FSharp.Test.Utilities

open FSharp.Compiler.Service.Tests.Common

/// Coverage for sequence-point line updates for line-shift edits, and active statement
/// remapping (remap to new span / MethodUpToDate / rude edits) for emitted deltas.
[<Collection(nameof NotThreadSafeResourceCollection)>]
module ActiveStatementTests =

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

    /// Snapshot of the project's CURRENT on-disk state (snapshots are immutable and
    /// content-versioned, so each edit needs a fresh one).
    let private snapshotOf (projectOptions: FSharpProjectOptions) =
        FSharpProjectSnapshot.FromOptions(projectOptions, DocumentSource.FileSystem)
        |> Async.RunImmediate

    let private prepareProjectOptions
        (checker: FSharpChecker)
        (fsPath: string)
        (dllPath: string)
        (source: string)
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
                |> Array.append
                    [| "--target:library"
                       "--langversion:preview"
                       "--optimize-"
                       "--debug:portable"
                       "--deterministic"
                       "--enable:hotreloaddeltas"
                       $"--out:{dllPath}" |] }

    let private compileProjectCore
        (checker: FSharpChecker)
        (projectOptions: FSharpProjectOptions)
        (includeHotReloadCapture: bool)
        =
        let options =
            if includeHotReloadCapture then
                projectOptions.OtherOptions
            else
                // Rebuilds of EDITS must not recapture an in-process baseline.
                projectOptions.OtherOptions
                |> Array.filter (fun opt ->
                    not (opt.StartsWith("--enable:hotreloaddeltas", StringComparison.OrdinalIgnoreCase)))

        let argv =
            Array.concat [ [| "fsc.exe" |]; options; projectOptions.SourceFiles ]

        let diagnostics, exOpt = checker.Compile(argv) |> Async.RunImmediate

        let errors =
            diagnostics
            |> Array.filter (fun diagnostic -> diagnostic.Severity = FSharpDiagnosticSeverity.Error)

        match errors, exOpt with
        | [||], None -> ()
        | errs, _ -> failwithf "Compilation failed: %A" (errs |> Array.map (fun d -> d.Message))

    let private compileBaseline (checker: FSharpChecker) (projectOptions: FSharpProjectOptions) =
        compileProjectCore checker projectOptions true

    let private compileUpdate (checker: FSharpChecker) (projectOptions: FSharpProjectOptions) =
        compileProjectCore checker projectOptions false

    let private getMethodToken (dllPath: string) (declaringType: string) (methodName: string) =
        use peReader = new PEReader(File.OpenRead(dllPath))
        let metadataReader = peReader.GetMetadataReader()

        metadataReader.MethodDefinitions
        |> Seq.tryPick (fun handle ->
            let methodDef = metadataReader.GetMethodDefinition(handle)
            let typeDef = metadataReader.GetTypeDefinition(methodDef.GetDeclaringType())

            if
                metadataReader.GetString(typeDef.Name) = declaringType
                && metadataReader.GetString(methodDef.Name) = methodName
            then
                Some(MetadataTokens.GetToken(EntityHandle.op_Implicit handle))
            else
                None)
        |> Option.defaultWith (fun () -> failwithf "Method %s::%s not found in %s" declaringType methodName dllPath)

    /// Independent decode of a portable PDB's visible sequence points (offset plus zero-based
    /// span, Roslyn SourceSpan convention), keyed by MethodDebugInformation row id.
    let private decodeVisibleSequencePoints (pdbBytes: byte[]) =
        use provider =
            MetadataReaderProvider.FromPortablePdbImage(
                System.Collections.Immutable.ImmutableArray.CreateRange pdbBytes
            )

        let reader = provider.GetMetadataReader()

        [ for rowId in 1 .. reader.MethodDebugInformation.Count do
              let handle = MetadataTokens.MethodDebugInformationHandle rowId
              let info = reader.GetMethodDebugInformation(handle)

              if not info.SequencePointsBlob.IsNil then
                  let points =
                      info.GetSequencePoints()
                      |> Seq.filter (fun sp -> not sp.IsHidden)
                      |> Seq.map (fun sp ->
                          sp.Offset,
                          { StartLine = sp.StartLine - 1
                            StartColumn = sp.StartColumn - 1
                            EndLine = sp.EndLine - 1
                            EndColumn = sp.EndColumn - 1 })
                      |> List.ofSeq

                  if not points.IsEmpty then
                      yield rowId, points ]
        |> Map.ofList

    let private mkActiveStatement (methodToken: int) (ilOffset: int) (frameKind: FSharpActiveStatementFrameKind) =
        { ActiveInstruction =
            { Method = { Token = methodToken; Version = 1 }
              ILOffset = ilOffset }
          DocumentName = None
          SourceSpan =
            { StartLine = 0
              StartColumn = 0
              EndLine = 0
              EndColumn = 0 }
          Flags =
            { FrameKind = frameKind
              IsMethodUpToDate = true
              IsPartiallyExecuted = false
              IsNonUserCode = false
              IsStale = false } }

    let private withProject (testName: string) (action: string -> string -> unit) =
        let projectDir =
            Path.Combine(Path.GetTempPath(), testName, Guid.NewGuid().ToString("N"))

        Directory.CreateDirectory(projectDir) |> ignore
        let fsPath = Path.Combine(projectDir, "Library.fs")
        let dllPath = Path.Combine(projectDir, "Library.dll")

        try
            action fsPath dllPath
        finally
            try
                Directory.Delete(projectDir, true)
            with _ ->
                ()

    // ------------------------------------------------------------------------------------------
    // Line-shift edits

    let private lineShiftBaselineSource =
        """module Sample.Lib

let helper () = 1

let target () = 41
"""

    // Two comment lines above 'target' shift it down without any semantic change.
    let private lineShiftUpdatedSource =
        """module Sample.Lib

let helper () = 1

// shifted
// shifted again
let target () = 41
"""

    [<Fact>]
    let ``Line-shift edit produces sequence point updates without method body rows`` () =
        withProject "fcs-hotreload-lineshift" (fun fsPath dllPath ->
            File.WriteAllText(fsPath, lineShiftBaselineSource)
            let checker = createChecker ()
            let projectOptions = prepareProjectOptions checker fsPath dllPath lineShiftBaselineSource

            checker.InvalidateAll()
            compileBaseline checker projectOptions

            use session = checker.CreateHotReloadSession()

            match session.AddProject(snapshotOf projectOptions) |> Async.RunImmediate with
            | Error error -> failwithf "Failed to start session: %A" error
            | Ok() -> ()

            File.WriteAllText(fsPath, lineShiftUpdatedSource)
            checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate
            compileUpdate checker projectOptions

            match session.EmitDelta(snapshotOf projectOptions) |> Async.RunImmediate with
            | Error error -> failwithf "Expected a line-update-only delta, got error: %A" error
            | Ok delta ->
                // A pure line shift recompiles nothing: no metadata/IL rows, no generation.
                Assert.Empty(delta.Metadata)
                Assert.Empty(delta.IL)
                Assert.Empty(delta.UpdatedMethods)
                Assert.Empty(delta.AddedOrChangedMethods)
                Assert.Equal(Guid.Empty, delta.GenerationId)

                let updates = Assert.Single(delta.SequencePointUpdates)
                Assert.EndsWith("Library.fs", updates.FileName)

                // 'helper' is unshifted (zero-delta segments are never emitted); 'target' is the
                // last method of the document, so exactly one line update is expected.
                let lineUpdate = Assert.Single(updates.LineUpdates)
                Assert.Equal(2, lineUpdate.NewLine - lineUpdate.OldLine)

                // The update starts at 'target''s old (zero-based) start line: 'let target () = 41'
                // is the fifth line (index 4) of the baseline source.
                Assert.Equal(4, lineUpdate.OldLine))

    [<Fact>]
    let ``Line-shift edit produces sequence point updates in a disk-started session`` () =
        withProject "fcs-hotreload-lineshift-disk" (fun fsPath dllPath ->
            File.WriteAllText(fsPath, lineShiftBaselineSource)
            let checker = createChecker ()
            let projectOptions = prepareProjectOptions checker fsPath dllPath lineShiftBaselineSource

            checker.InvalidateAll()
            compileBaseline checker projectOptions

            // Start a session, then throw it away and start a fresh one: the dotnet-watch
            // topology, where the committed sequence-point view must be reconstructed from
            // the on-disk dll + pdb rather than carried over in memory (sessions are
            // independent instances, so the fresh session sees disk state only).
            do
                use discarded = checker.CreateHotReloadSession()

                match discarded.AddProject(snapshotOf projectOptions) |> Async.RunImmediate with
                | Error error -> failwithf "Failed to start session: %A" error
                | Ok() -> ()

            use session = checker.CreateHotReloadSession()

            match session.AddProject(snapshotOf projectOptions) |> Async.RunImmediate with
            | Error error -> failwithf "Failed to restart session from disk: %A" error
            | Ok() -> ()

            File.WriteAllText(fsPath, lineShiftUpdatedSource)
            checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate
            compileUpdate checker projectOptions

            match session.EmitDelta(snapshotOf projectOptions) |> Async.RunImmediate with
            | Error error -> failwithf "Expected a line-update-only delta after restart, got error: %A" error
            | Ok delta ->
                Assert.Empty(delta.UpdatedMethods)
                let updates = Assert.Single(delta.SequencePointUpdates)
                let lineUpdate = Assert.Single(updates.LineUpdates)
                Assert.Equal(2, lineUpdate.NewLine - lineUpdate.OldLine)
                Assert.Equal(4, lineUpdate.OldLine))

    // ------------------------------------------------------------------------------------------
    // Active statement remapping

    let private remapBaselineSource =
        """namespace Sample

type Type =
    static member GetValue() = 1
    static member Other() = 7
"""

    let private remapUpdatedSource =
        """namespace Sample

type Type =
    static member GetValue() = 100
    static member Other() = 7
"""

    [<Fact>]
    let ``Active statement in an edited method remaps to the new span`` () =
        withProject "fcs-hotreload-as-remap" (fun fsPath dllPath ->
            File.WriteAllText(fsPath, remapBaselineSource)
            let checker = createChecker ()
            let projectOptions = prepareProjectOptions checker fsPath dllPath remapBaselineSource

            checker.InvalidateAll()
            compileBaseline checker projectOptions

            let getValueToken = getMethodToken dllPath "Type" "GetValue"
            let otherToken = getMethodToken dllPath "Type" "Other"

            use session = checker.CreateHotReloadSession()

            match session.AddProject(snapshotOf projectOptions) |> Async.RunImmediate with
            | Error error -> failwithf "Failed to start session: %A" error
            | Ok() -> ()

            // The debugger reports a break: a LEAF frame in GetValue at IL offset 0, and a
            // statement in the untouched Other method.
            session.SetActiveStatements(
                    [ mkActiveStatement getValueToken 0 FSharpActiveStatementFrameKind.Leaf
                      mkActiveStatement otherToken 0 FSharpActiveStatementFrameKind.Leaf ]
                )

            File.WriteAllText(fsPath, remapUpdatedSource)
            checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate
            compileUpdate checker projectOptions

            match session.EmitDelta(snapshotOf projectOptions) |> Async.RunImmediate with
            | Error error -> failwithf "EmitDelta failed: %A" error
            | Ok delta ->
                Assert.Contains(getValueToken, delta.UpdatedMethods)
                Assert.Equal(2, List.length delta.ActiveStatementUpdates)

                let remapped =
                    delta.ActiveStatementUpdates
                    |> List.pick (function
                        | FSharpActiveStatementRemapResult.Remapped update -> Some update
                        | _ -> None)

                Assert.Equal(getValueToken, remapped.Method.Token)
                Assert.Equal(0, remapped.ILOffset)

                // The statement in the untouched method reports MethodUpToDate.
                let upToDate =
                    delta.ActiveStatementUpdates
                    |> List.pick (function
                        | FSharpActiveStatementRemapResult.MethodUpToDate instruction -> Some instruction
                        | _ -> None)

                Assert.Equal(otherToken, upToDate.Method.Token)

                // Cross-check the remapped span against the sequence-point table of the emitted
                // delta PDB: GetValue is the only recompiled method, so the delta PDB carries a
                // single MethodDebugInformation row, whose first visible point must be the span
                // the statement was remapped to.
                let deltaPdb =
                    match delta.Pdb with
                    | Some bytes -> bytes
                    | None -> failwith "Expected the delta to carry a PDB delta"

                let deltaPoints = decodeVisibleSequencePoints deltaPdb
                let row, points = Assert.Single(Map.toSeq deltaPoints)
                Assert.Equal(1, row)
                let _, firstSpan = points.Head
                Assert.Equal(firstSpan, remapped.NewSpan))

    let private deleteBaselineSource =
        """namespace Sample

type Type =
    static member Run(flag: bool) =
        let a = if flag then 1 else 2
        let b = a + 3
        a + b
"""

    // The statement 'let b = a + 3' is deleted while a frame is suspended in it.
    let private deleteUpdatedSource =
        """namespace Sample

type Type =
    static member Run(flag: bool) =
        let a = if flag then 1 else 2
        a + a
"""

    [<Fact>]
    let ``Edit deleting the active statement is a rude edit`` () =
        withProject "fcs-hotreload-as-delete" (fun fsPath dllPath ->
            File.WriteAllText(fsPath, deleteBaselineSource)
            let checker = createChecker ()
            let projectOptions = prepareProjectOptions checker fsPath dllPath deleteBaselineSource

            checker.InvalidateAll()
            compileBaseline checker projectOptions

            let runToken = getMethodToken dllPath "Type" "Run"

            // Find a mid-body visible sequence point of Run in the baseline PDB to stand on.
            let baselinePoints =
                decodeVisibleSequencePoints (File.ReadAllBytes(Path.ChangeExtension(dllPath, ".pdb")))

            let runRow = runToken &&& 0x00FFFFFF

            let midOffset =
                match Map.tryFind runRow baselinePoints with
                | Some(_ :: (offset, _) :: _) -> offset
                | _ -> failwith "Expected Run to carry at least two visible sequence points"

            use session = checker.CreateHotReloadSession()

            match session.AddProject(snapshotOf projectOptions) |> Async.RunImmediate with
            | Error error -> failwithf "Failed to start session: %A" error
            | Ok() -> ()

            session.SetActiveStatements(
                    [ mkActiveStatement runToken midOffset FSharpActiveStatementFrameKind.Leaf ]
                )

            File.WriteAllText(fsPath, deleteUpdatedSource)
            checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate
            compileUpdate checker projectOptions

            match session.EmitDelta(snapshotOf projectOptions) |> Async.RunImmediate with
            | Ok _ -> failwith "Expected a rude edit for an edit deleting the active statement"
            | Error(FSharpHotReloadError.UnsupportedEdit message) ->
                Assert.Contains("active statement", message, StringComparison.OrdinalIgnoreCase)
            | Error error -> failwithf "Expected UnsupportedEdit, got: %A" error

            // The rude edit blocked the update without corrupting the session: clearing the
            // break state lets the same edit through as a regular method-body update.
            Assert.Equal(1, session.ProjectIdentifiers.Length)
            session.SetActiveStatements([])

            match session.EmitDelta(snapshotOf projectOptions) |> Async.RunImmediate with
            | Error error -> failwithf "Expected the edit to apply once no statements are active: %A" error
            | Ok delta -> Assert.Contains(runToken, delta.UpdatedMethods))

    [<Fact>]
    let ``Editing the statement a non-leaf frame is suspended in is a rude edit`` () =
        withProject "fcs-hotreload-as-nonleaf" (fun fsPath dllPath ->
            File.WriteAllText(fsPath, remapBaselineSource)
            let checker = createChecker ()
            let projectOptions = prepareProjectOptions checker fsPath dllPath remapBaselineSource

            checker.InvalidateAll()
            compileBaseline checker projectOptions

            let getValueToken = getMethodToken dllPath "Type" "GetValue"

            use session = checker.CreateHotReloadSession()

            match session.AddProject(snapshotOf projectOptions) |> Async.RunImmediate with
            | Error error -> failwithf "Failed to start session: %A" error
            | Ok() -> ()

            // A NON-LEAF frame is suspended in the statement that the edit changes ('= 1' to
            // '= 100' changes the statement's span): the frame cannot be remapped.
            session.SetActiveStatements(
                    [ mkActiveStatement getValueToken 0 FSharpActiveStatementFrameKind.NonLeaf ]
                )

            File.WriteAllText(fsPath, remapUpdatedSource)
            checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate
            compileUpdate checker projectOptions

            match session.EmitDelta(snapshotOf projectOptions) |> Async.RunImmediate with
            | Ok _ -> failwith "Expected a rude edit for editing a non-leaf frame's active statement"
            | Error(FSharpHotReloadError.UnsupportedEdit message) ->
                Assert.Contains("non-leaf", message, StringComparison.OrdinalIgnoreCase)
            | Error error -> failwithf "Expected UnsupportedEdit, got: %A" error)
