// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// <summary>
/// Active-statement and sequence-point-update model for F# hot reload.
///
/// The public types in this file mirror the debugger-facing Edit-and-Continue contract types in
/// <c>Microsoft.CodeAnalysis.Contracts.EditAndContinue</c> (Roslyn: src/Features/Core/Portable/Contracts/EditAndContinue):
///
/// <list type="bullet">
///   <item><see cref="T:FSharp.Compiler.CodeAnalysis.FSharpSourceSpan"/> mirrors <c>SourceSpan</c></item>
///   <item><see cref="T:FSharp.Compiler.CodeAnalysis.FSharpManagedModuleMethodId"/> mirrors <c>ManagedModuleMethodId</c></item>
///   <item><see cref="T:FSharp.Compiler.CodeAnalysis.FSharpManagedInstructionId"/> mirrors <c>ManagedInstructionId</c></item>
///   <item><see cref="T:FSharp.Compiler.CodeAnalysis.FSharpManagedActiveStatementDebugInfo"/> mirrors <c>ManagedActiveStatementDebugInfo</c></item>
///   <item><see cref="T:FSharp.Compiler.CodeAnalysis.FSharpSourceLineUpdate"/> mirrors <c>SourceLineUpdate</c></item>
///   <item><see cref="T:FSharp.Compiler.CodeAnalysis.FSharpSequencePointUpdates"/> mirrors <c>SequencePointUpdates</c></item>
///   <item><see cref="T:FSharp.Compiler.CodeAnalysis.FSharpManagedActiveStatementUpdate"/> mirrors <c>ManagedActiveStatementUpdate</c></item>
/// </list>
///
/// Documented deviations from the Roslyn contract shapes (F#-shaped, illegal states unrepresentable):
///
/// <list type="bullet">
///   <item>Roslyn's <c>ManagedMethodId</c> carries a module version id (MVID); an F# hot reload session is
///   single-module, so the module identity is implicit in the session and only the token/version pair is
///   modeled (<c>FSharpManagedModuleMethodId</c>).</item>
///   <item>Roslyn's <c>ActiveStatementFlags</c> is a [Flags] enum that can express contradictory states
///   (e.g. a frame that is neither leaf nor non-leaf). F# models the frame position as a closed union
///   (<c>FSharpActiveStatementFrameKind</c>) plus independent booleans for the genuinely independent bits.</item>
///   <item>Roslyn's <c>SourceSpan</c> encodes "column information missing" as -1 column values; F# PDBs always
///   carry columns, so spans here are total. All coordinates are ZERO-based (Roslyn contract convention;
///   note this is one less than the 1-based lines/columns stored in Portable PDB sequence points).</item>
/// </list>
/// </summary>
namespace FSharp.Compiler.CodeAnalysis

/// <summary>
/// The start/end line/column range for a contiguous span of source text.
/// Mirrors <c>Microsoft.CodeAnalysis.Contracts.EditAndContinue.SourceSpan</c>.
/// All coordinates are zero-based.
/// </summary>
[<Experimental("This FCS API is experimental and subject to change.")>]
type FSharpSourceSpan =
    {
        /// <summary>Zero-based start line.</summary>
        StartLine: int
        /// <summary>Zero-based start column.</summary>
        StartColumn: int
        /// <summary>Zero-based end line.</summary>
        EndLine: int
        /// <summary>Zero-based end column.</summary>
        EndColumn: int
    }

/// <summary>
/// Token/version pair uniquely identifying a method version within the session's module.
/// Mirrors <c>Microsoft.CodeAnalysis.Contracts.EditAndContinue.ManagedModuleMethodId</c>
/// (the module MVID of Roslyn's <c>ManagedMethodId</c> is implicit in the single-module session).
/// </summary>
[<Experimental("This FCS API is experimental and subject to change.")>]
type FSharpManagedModuleMethodId =
    {
        /// <summary>MethodDef metadata token (0x06xxxxxx) of the method that contains the statement.</summary>
        Token: int
        /// <summary>
        /// 1-based method version. 1 for methods never edited through EnC; for edited methods the version
        /// reflects the EnC generation that produced the executing body. Debugger-supplied.
        /// </summary>
        Version: int
    }

/// <summary>
/// Active instruction identifier: the information necessary to track an active instruction within a
/// debug session. Mirrors <c>Microsoft.CodeAnalysis.Contracts.EditAndContinue.ManagedInstructionId</c>.
/// </summary>
[<Experimental("This FCS API is experimental and subject to change.")>]
type FSharpManagedInstructionId =
    {
        /// <summary>Method version which the instruction is scoped to.</summary>
        Method: FSharpManagedModuleMethodId
        /// <summary>IL offset of the instruction within the method body.</summary>
        ILOffset: int
    }

/// <summary>
/// Position of the frames owning an active statement, aggregated across all threads.
/// Replaces the <c>LeafFrame</c>/<c>NonLeafFrame</c> bits of Roslyn's <c>ActiveStatementFlags</c>
/// with a closed union so "neither leaf nor non-leaf" is unrepresentable.
/// </summary>
[<Experimental("This FCS API is experimental and subject to change.")>]
[<RequireQualifiedAccess>]
type FSharpActiveStatementFrameKind =
    /// <summary>Every thread owning the statement is stopped in a leaf (topmost) frame.</summary>
    | Leaf
    /// <summary>Every thread owning the statement is suspended in a non-leaf (caller) frame.</summary>
    | NonLeaf
    /// <summary>At least one owning thread is in a leaf frame and at least one is in a non-leaf frame.</summary>
    | LeafAndNonLeaf

    /// <summary>True when at least one owning thread is in a non-leaf frame (Roslyn <c>NonLeafFrame</c> bit).</summary>
    member this.HasNonLeafFrame =
        match this with
        | Leaf -> false
        | NonLeaf
        | LeafAndNonLeaf -> true

/// <summary>
/// Flags describing an active statement, aggregated across the threads that own it.
/// Mirrors <c>Microsoft.CodeAnalysis.Contracts.EditAndContinue.ActiveStatementFlags</c>.
/// </summary>
[<Experimental("This FCS API is experimental and subject to change.")>]
type FSharpActiveStatementFlags =
    {
        /// <summary>Leaf/non-leaf position of the owning frames (Roslyn <c>LeafFrame</c>/<c>NonLeafFrame</c> bits).</summary>
        FrameKind: FSharpActiveStatementFrameKind
        /// <summary>
        /// The instruction belongs to the latest version of the containing method (Roslyn <c>MethodUpToDate</c>).
        /// When false the containing method was updated but this frame has not been remapped yet.
        /// </summary>
        IsMethodUpToDate: bool
        /// <summary>
        /// The thread is stopped between two sequence points (Roslyn <c>PartiallyExecuted</c>).
        /// Partially executed active statements cannot be edited.
        /// </summary>
        IsPartiallyExecuted: bool
        /// <summary>The statement IL is not in user code (Roslyn <c>NonUserCode</c>).</summary>
        IsNonUserCode: bool
        /// <summary>
        /// The statement was left un-remapped by a previous update applied while the code was executing
        /// (Roslyn <c>Stale</c>). Stale statements are never up-to-date.
        /// </summary>
        IsStale: bool
    }

/// <summary>
/// Active statement debug information retrieved from the runtime and the PDB by the debugger host,
/// supplied to the compiler before a delta is emitted.
/// Mirrors <c>Microsoft.CodeAnalysis.Contracts.EditAndContinue.ManagedActiveStatementDebugInfo</c>.
/// </summary>
[<Experimental("This FCS API is experimental and subject to change.")>]
type FSharpManagedActiveStatementDebugInfo =
    {
        /// <summary>The instruction of the active statement that is being executed.</summary>
        ActiveInstruction: FSharpManagedInstructionId
        /// <summary>Document name as found in the PDB; None when the debugger could not determine the location.</summary>
        DocumentName: string option
        /// <summary>Location of the closest non-hidden sequence point retrieved from the PDB (zero-based).</summary>
        SourceSpan: FSharpSourceSpan
        /// <summary>Aggregated flags across the threads owning the active statement.</summary>
        Flags: FSharpActiveStatementFlags
    }

/// <summary>
/// Maps a source line affected by an update: a sequence point that started on <see cref="OldLine"/>
/// before the edit starts on <see cref="NewLine"/> after it. Zero-based line numbers.
/// Mirrors <c>Microsoft.CodeAnalysis.Contracts.EditAndContinue.SourceLineUpdate</c>.
/// </summary>
[<Experimental("This FCS API is experimental and subject to change.")>]
type FSharpSourceLineUpdate =
    {
        /// <summary>Zero-based line number before the update was made.</summary>
        OldLine: int
        /// <summary>Zero-based line number after the update was made.</summary>
        NewLine: int
    }

/// <summary>
/// Sequence points affected by an update in a single source file. Line updates are sorted by
/// <see cref="FSharpSourceLineUpdate.OldLine"/>; an update applies from its old line until the next
/// entry's old line (Roslyn emits explicit zero-delta entries to terminate shifted segments, and so
/// does F#). Mirrors <c>Microsoft.CodeAnalysis.Contracts.EditAndContinue.SequencePointUpdates</c>.
/// </summary>
[<Experimental("This FCS API is experimental and subject to change.")>]
type FSharpSequencePointUpdates =
    {
        /// <summary>Name of the modified file as stored in the PDB.</summary>
        FileName: string
        /// <summary>Line updates for the file, sorted by old line.</summary>
        LineUpdates: FSharpSourceLineUpdate list
    }

/// <summary>
/// Active statement affected by an update to its containing method: the debugger uses this to remap
/// the instruction pointer to the appropriate location in the new method version.
/// Mirrors <c>Microsoft.CodeAnalysis.Contracts.EditAndContinue.ManagedActiveStatementUpdate</c>.
/// </summary>
[<Experimental("This FCS API is experimental and subject to change.")>]
type FSharpManagedActiveStatementUpdate =
    {
        /// <summary>Method id (token and version) BEFORE the change was made, as supplied by the debugger.</summary>
        Method: FSharpManagedModuleMethodId
        /// <summary>Old IL offset of the active statement (in the executing method version).</summary>
        ILOffset: int
        /// <summary>Source span of the active statement after the edit (zero-based).</summary>
        NewSpan: FSharpSourceSpan
    }

/// <summary>
/// Per-statement outcome of active-statement remapping for an emitted delta.
/// F#-shaped extension of the Roslyn contract: Roslyn's <c>ManagedHotReloadUpdate.ActiveStatements</c>
/// carries only the remapped statements (statements in methods untouched by the delta are simply
/// absent); F# reports the untouched ones explicitly so hosts can assert method-up-to-date-ness.
/// </summary>
[<Experimental("This FCS API is experimental and subject to change.")>]
[<RequireQualifiedAccess>]
type FSharpActiveStatementRemapResult =
    /// <summary>The containing method was recompiled by this delta; the statement maps to a new span.</summary>
    | Remapped of update: FSharpManagedActiveStatementUpdate
    /// <summary>The containing method is untouched by this delta; the executing version remains current.</summary>
    | MethodUpToDate of instruction: FSharpManagedInstructionId

namespace FSharp.Compiler.HotReload

/// <summary>
/// Sequence-point analysis backing the active-statement machinery: decoding per-method
/// sequence points from Portable PDBs, classifying how a method's sequence points changed between
/// the committed state and a fresh compile, and merging per-method line shifts into the
/// debugger-facing <see cref="T:FSharp.Compiler.CodeAnalysis.FSharpSequencePointUpdates"/> shape
/// using Roslyn's <c>AbstractEditAndContinueAnalyzer.GetLineEdits</c> segment-merge algorithm.
/// </summary>
module internal ActiveStatementAnalysis =

    open System
    open System.Collections.Immutable
    open System.Reflection.Metadata
    open System.Reflection.Metadata.Ecma335
    open FSharp.Compiler.CodeAnalysis

    /// A visible (non-hidden) sequence point in raw Portable PDB coordinates (1-based lines/columns).
    type VisibleSequencePoint =
        {
            ILOffset: int
            StartLine: int
            StartColumn: int
            EndLine: int
            EndColumn: int
        }

        /// Converts the raw PDB coordinates to a zero-based contract span (Roslyn SourceSpan convention).
        member this.ToSourceSpan() : FSharpSourceSpan =
            {
                StartLine = this.StartLine - 1
                StartColumn = this.StartColumn - 1
                EndLine = this.EndLine - 1
                EndColumn = this.EndColumn - 1
            }

    /// A decoded sequence point: hidden points carry only their IL offset.
    [<RequireQualifiedAccess>]
    type PdbSequencePoint =
        | Hidden of ilOffset: int
        | Visible of VisibleSequencePoint

        member this.ILOffset =
            match this with
            | Hidden ilOffset -> ilOffset
            | Visible visible -> visible.ILOffset

    /// <summary>
    /// The decoded sequence points of one method. <see cref="Document"/> is the PDB document name when
    /// every sequence point of the method lives in a single document, and None for multi-document
    /// methods (e.g. bodies spliced with <c>#line</c>) — those are never line-shift candidates.
    /// </summary>
    type MethodSequencePoints =
        {
            Document: string option
            Points: PdbSequencePoint list
        }

        member this.VisiblePoints =
            this.Points
            |> List.choose (function
                | PdbSequencePoint.Visible visible -> Some visible
                | PdbSequencePoint.Hidden _ -> None)

    /// <summary>
    /// Decodes the MethodDebugInformation table of a Portable PDB image into per-method sequence
    /// points, keyed by MethodDef token (0x06xxxxxx; MethodDebugInformation rows map 1:1 to MethodDef
    /// rows). Methods without a sequence-point blob have no entry. Unreadable images decode to the
    /// empty map (fail closed: the active-statement machinery then stays inert rather than guessing).
    /// </summary>
    let decodeMethodSequencePoints (pdbBytes: byte[]) : Map<int, MethodSequencePoints> =
        try
            use provider =
                MetadataReaderProvider.FromPortablePdbImage(ImmutableArray.CreateRange pdbBytes)

            let reader = provider.GetMetadataReader()
            let mutable result = Map.empty

            for rowId in 1 .. reader.MethodDebugInformation.Count do
                let handle = MetadataTokens.MethodDebugInformationHandle rowId
                let methodDebugInfo = reader.GetMethodDebugInformation handle

                if not methodDebugInfo.SequencePointsBlob.IsNil then
                    let sequencePoints = methodDebugInfo.GetSequencePoints() |> List.ofSeq

                    let documentName =
                        let documents = sequencePoints |> List.map (fun sp -> sp.Document) |> List.distinct

                        match documents with
                        | [ single ] when not single.IsNil ->
                            let document = reader.GetDocument single
                            Some(reader.GetString document.Name)
                        | _ -> None

                    let points =
                        sequencePoints
                        |> List.map (fun sp ->
                            if sp.IsHidden then
                                PdbSequencePoint.Hidden sp.Offset
                            else
                                PdbSequencePoint.Visible
                                    {
                                        ILOffset = sp.Offset
                                        StartLine = sp.StartLine
                                        StartColumn = sp.StartColumn
                                        EndLine = sp.EndLine
                                        EndColumn = sp.EndColumn
                                    })

                    let methodToken = 0x06000000 ||| rowId

                    result <-
                        Map.add
                            methodToken
                            {
                                Document = documentName
                                Points = points
                            }
                            result

            result
        with :? BadImageFormatException ->
            Map.empty

    /// How a method's sequence points changed between the committed state and a fresh compile.
    [<RequireQualifiedAccess>]
    type SequencePointComparison =
        /// The sequence points are byte-for-byte unchanged.
        | Identical
        /// Every visible sequence point moved by the same line delta with identical IL offsets,
        /// columns and line extents — the method body did not change, the code around it did
        /// (Roslyn: a line-edit segment; the debugger rebinds without recompilation).
        | UniformLineShift of lineDelta: int
        /// Anything else: changed offsets/columns/structure, document changes, multi-document
        /// bodies. The method must be recompiled to keep its debug information accurate
        /// (Roslyn: trivia edits force a member body update).
        | Different

    let compareMethodSequencePoints (committed: MethodSequencePoints) (fresh: MethodSequencePoints) =
        if committed = fresh then
            SequencePointComparison.Identical
        elif
            committed.Document.IsNone
            || committed.Document <> fresh.Document
            || List.length committed.Points <> List.length fresh.Points
        then
            SequencePointComparison.Different
        else
            let mutable lineDelta = ValueNone
            let mutable different = false

            for committedPoint, freshPoint in List.zip committed.Points fresh.Points do
                match committedPoint, freshPoint with
                | PdbSequencePoint.Hidden oldOffset, PdbSequencePoint.Hidden newOffset when oldOffset = newOffset -> ()
                | PdbSequencePoint.Visible oldPoint, PdbSequencePoint.Visible newPoint when
                    oldPoint.ILOffset = newPoint.ILOffset
                    && oldPoint.StartColumn = newPoint.StartColumn
                    && oldPoint.EndColumn = newPoint.EndColumn
                    && newPoint.EndLine - newPoint.StartLine = oldPoint.EndLine - oldPoint.StartLine
                    ->
                    let delta = newPoint.StartLine - oldPoint.StartLine

                    match lineDelta with
                    | ValueNone -> lineDelta <- ValueSome delta
                    | ValueSome existing when existing = delta -> ()
                    | ValueSome _ -> different <- true
                | _ -> different <- true

            if different then
                SequencePointComparison.Different
            else
                match lineDelta with
                | ValueSome delta when delta <> 0 -> SequencePointComparison.UniformLineShift delta
                // All-equal pairs reduce to structural equality, handled above; an all-hidden or
                // zero-delta walk that still got here is by construction identical.
                | _ -> SequencePointComparison.Identical

    /// <summary>
    /// One per-method line-shift segment feeding the line-update merge: the method's visible
    /// sequence-point extent in the committed state (zero-based lines) plus the uniform line delta of
    /// the fresh compile. Zero-delta segments are emitted for unchanged methods purely so overlaps
    /// with shifted segments are detected (Roslyn parity). <see cref="MethodToken"/> is bookkeeping
    /// for the recompile fallback and never serialized.
    /// </summary>
    type LineShiftSegment =
        {
            FileName: string
            OldStartLine: int
            OldEndLine: int
            LineDelta: int
            MethodToken: int
        }

    /// <summary>
    /// Builds the line-shift segment of a method classified <c>Identical</c> (delta 0, overlap
    /// detection only) or <c>UniformLineShift</c>. None for methods without a single document or
    /// without visible sequence points.
    /// </summary>
    let tryCreateLineShiftSegment (methodToken: int) (committed: MethodSequencePoints) (lineDelta: int) =
        match committed.Document, committed.VisiblePoints with
        | Some document, (_ :: _ as visiblePoints) ->
            Some
                {
                    FileName = document
                    // PDB lines are 1-based; the contract is zero-based.
                    OldStartLine = (visiblePoints |> List.map (fun p -> p.StartLine) |> List.min) - 1
                    OldEndLine = (visiblePoints |> List.map (fun p -> p.EndLine) |> List.max) - 1
                    LineDelta = lineDelta
                    MethodToken = methodToken
                }
        | _ -> None

    /// <summary>
    /// Merges per-method segments into per-document line updates, mirroring Roslyn's
    /// <c>AbstractEditAndContinueAnalyzer.GetLineEdits</c>: segments are sorted by document and old
    /// start line; a zero-delta entry is inserted when a shifted segment range ends before the next
    /// segment so the shift does not leak past it; segments overlapping a previous segment with a
    /// DIFFERENT delta cannot be expressed as line updates and are returned as method tokens to
    /// recompile instead (Roslyn recompiles the overlapping member for the same reason).
    /// </summary>
    let mergeLineShiftSegments (segments: LineShiftSegment list) : FSharpSequencePointUpdates list * int list =
        match segments with
        | [] -> [], []
        | _ ->
            let sorted =
                segments
                |> List.sortWith (fun x y ->
                    let byFile = String.CompareOrdinal(x.FileName, y.FileName)

                    if byFile <> 0 then
                        byFile
                    else
                        compare x.OldStartLine y.OldStartLine)

            let lineEdits = ResizeArray<FSharpSequencePointUpdates>()
            let documentLineEdits = ResizeArray<FSharpSourceLineUpdate>()
            let overlapRecompileTokens = ResizeArray<int>()

            let mutable currentDocumentPath = sorted.Head.FileName
            let mutable previousOldEndLine = -1
            let mutable previousLineDelta = 0

            for segment in sorted do
                let mutable skipSegment = false

                if segment.FileName <> currentDocumentPath then
                    // Store results for the previous document and switch to the next one.
                    if documentLineEdits.Count > 0 then
                        lineEdits.Add
                            {
                                FileName = currentDocumentPath
                                LineUpdates = List.ofSeq documentLineEdits
                            }

                        documentLineEdits.Clear()

                    currentDocumentPath <- segment.FileName
                    previousOldEndLine <- -1
                    previousLineDelta <- 0
                elif
                    segment.OldStartLine <= previousOldEndLine
                    && segment.LineDelta <> previousLineDelta
                then
                    // The segment overlaps the previous one with a different line delta:
                    // the method must be recompiled (the debugger filters line deltas that
                    // correspond to recompiled methods).
                    overlapRecompileTokens.Add segment.MethodToken
                    skipSegment <- true

                if not skipSegment then
                    // Reset the delta to 0 for the lines between the previous segment and this one.
                    if documentLineEdits.Count > 0 && segment.OldStartLine > previousOldEndLine + 1 then
                        documentLineEdits.Add
                            {
                                OldLine = previousOldEndLine + 1
                                NewLine = previousOldEndLine + 1
                            }

                        previousLineDelta <- 0

                    // Skip zero-delta segments (overlap detection only) and repeated deltas.
                    if segment.LineDelta <> 0 && segment.LineDelta <> previousLineDelta then
                        documentLineEdits.Add
                            {
                                OldLine = segment.OldStartLine
                                NewLine = segment.OldStartLine + segment.LineDelta
                            }

                    previousOldEndLine <- segment.OldEndLine
                    previousLineDelta <- segment.LineDelta

            if documentLineEdits.Count > 0 then
                lineEdits.Add
                    {
                        FileName = currentDocumentPath
                        LineUpdates = List.ofSeq documentLineEdits
                    }

            List.ofSeq lineEdits, List.ofSeq overlapRecompileTokens

    /// Outcome of remapping a single active statement against an emitted delta.
    [<RequireQualifiedAccess>]
    type ActiveStatementRemapOutcome =
        /// The containing method was recompiled; the statement maps to this new span.
        | Update of FSharpManagedActiveStatementUpdate
        /// The containing method is untouched by the delta.
        | MethodUpToDate
        /// The edit destroys or ambiguously moves the active statement; the update must be blocked.
        | Rude of message: string

    /// <summary>
    /// Remaps one active statement after its containing method was recompiled by a delta
    /// (<paramref name="freshPoints"/> = None means the method was NOT recompiled).
    ///
    /// The F# remap aligns the committed and fresh sequence points by ordinal: the statement's
    /// instruction is resolved to the last visible committed sequence point at or before its IL
    /// offset, and maps to the fresh visible sequence point with the same ordinal when both sides
    /// have the SAME number of visible points. Roslyn instead tracks statement spans through the
    /// syntax map; the F# diff is typed-tree based, so where the sequence-point alignment is
    /// ambiguous the remap deliberately fails toward a rude edit (Roslyn parity for the
    /// statement-destroying cases: deleting the active statement, or editing the statement a
    /// non-leaf frame is suspended in, blocks the update):
    ///
    /// <list type="bullet">
    ///   <item>stale / not-up-to-date / partially-executed statements in a recompiled method -> rude</item>
    ///   <item>no committed sequence points for the method, or the IL offset resolves to no visible
    ///   point -> rude</item>
    ///   <item>visible sequence-point counts differ (a statement was added or deleted around or at
    ///   the active statement) -> rude</item>
    ///   <item>a NON-LEAF frame's statement whose aligned span changed by anything other than a pure
    ///   line shift -> rude (the runtime cannot remap a frame that is not topmost; leaf frames are
    ///   remapped by the debugger, so their statement may move freely)</item>
    /// </list>
    /// </summary>
    let remapActiveStatement
        (statement: FSharpManagedActiveStatementDebugInfo)
        (committedPoints: MethodSequencePoints option)
        (freshPoints: MethodSequencePoints option)
        : ActiveStatementRemapOutcome =
        let instruction = statement.ActiveInstruction

        let describe =
            $"method 0x%08X{instruction.Method.Token} IL_%04X{instruction.ILOffset}"

        match freshPoints with
        | None -> ActiveStatementRemapOutcome.MethodUpToDate
        | Some fresh ->
            if statement.Flags.IsStale || not statement.Flags.IsMethodUpToDate then
                ActiveStatementRemapOutcome.Rude
                    $"Active statement in {describe} executes a stale version of the method; updating the method again cannot be remapped. Please rebuild."
            elif statement.Flags.IsPartiallyExecuted then
                ActiveStatementRemapOutcome.Rude
                    $"Active statement in {describe} is partially executed and cannot be updated. Please rebuild."
            else
                match committedPoints with
                | None ->
                    ActiveStatementRemapOutcome.Rude
                        $"Active statement in {describe} has no committed sequence points to remap from. Please rebuild."
                | Some committed ->
                    // Resolve the instruction to the last visible committed point at or before
                    // its IL offset, tracking the point's ordinal among visible points.
                    let mutable coveringOrdinal = -1
                    let mutable coveringIsHidden = false
                    let mutable visibleIndex = -1

                    for point in committed.Points do
                        if point.ILOffset <= instruction.ILOffset then
                            match point with
                            | PdbSequencePoint.Visible _ ->
                                visibleIndex <- visibleIndex + 1
                                coveringOrdinal <- visibleIndex
                                coveringIsHidden <- false
                            | PdbSequencePoint.Hidden _ -> coveringIsHidden <- true
                        else
                            match point with
                            | PdbSequencePoint.Visible _ -> visibleIndex <- visibleIndex + 1
                            | PdbSequencePoint.Hidden _ -> ()

                    if coveringOrdinal < 0 || coveringIsHidden then
                        ActiveStatementRemapOutcome.Rude
                            $"Active statement in {describe} does not resolve to a visible sequence point. Please rebuild."
                    else
                        let committedVisible = committed.VisiblePoints
                        let freshVisible = fresh.VisiblePoints

                        if List.length committedVisible <> List.length freshVisible then
                            ActiveStatementRemapOutcome.Rude
                                $"Edit adds or deletes statements around the active statement in {describe}; the active statement cannot be remapped. Please rebuild."
                        else
                            let oldPoint = committedVisible[coveringOrdinal]
                            let newPoint = freshVisible[coveringOrdinal]

                            let pureLineShift =
                                oldPoint.StartColumn = newPoint.StartColumn
                                && oldPoint.EndColumn = newPoint.EndColumn
                                && newPoint.EndLine - newPoint.StartLine = oldPoint.EndLine - oldPoint.StartLine

                            if statement.Flags.FrameKind.HasNonLeafFrame && not pureLineShift then
                                ActiveStatementRemapOutcome.Rude
                                    $"Edit changes the active statement a non-leaf frame is suspended in ({describe}); the frame cannot be remapped. Please rebuild."
                            else
                                ActiveStatementRemapOutcome.Update
                                    {
                                        Method = instruction.Method
                                        ILOffset = instruction.ILOffset
                                        NewSpan = newPoint.ToSourceSpan()
                                    }

    /// <summary>
    /// Remaps the host-supplied active statements against an emitted delta.
    /// <paramref name="recompiledMethodTokens"/> are the MethodDef tokens recompiled by the delta;
    /// <paramref name="freshSnapshots"/> is the delta's next committed sequence-point view (None
    /// when sequence-point analysis was unavailable — any active statement in a recompiled method
    /// is then rude, fail closed). Statements in methods the delta does not touch report
    /// <see cref="FSharpActiveStatementRemapResult.MethodUpToDate"/>. Any rude statement blocks
    /// the whole update (Roslyn parity).
    /// </summary>
    let remapActiveStatements
        (committedSnapshots: Map<int, MethodSequencePoints>)
        (freshSnapshots: Map<int, MethodSequencePoints> option)
        (recompiledMethodTokens: Set<int>)
        (statements: FSharpManagedActiveStatementDebugInfo list)
        : Result<FSharpActiveStatementRemapResult list, string list> =
        let results, rudeMessages =
            (([], []), statements)
            ||> List.fold (fun (results, rudeMessages) statement ->
                let methodToken = statement.ActiveInstruction.Method.Token

                if not (Set.contains methodToken recompiledMethodTokens) then
                    FSharpActiveStatementRemapResult.MethodUpToDate statement.ActiveInstruction
                    :: results,
                    rudeMessages
                else
                    let committed = Map.tryFind methodToken committedSnapshots

                    let fresh =
                        match freshSnapshots with
                        | Some snapshots -> Map.tryFind methodToken snapshots
                        | None -> None

                    match committed, fresh with
                    | _, None ->
                        // The method WAS recompiled but no fresh sequence points are available
                        // (no PDB analysis, or the fresh body lost its debug information):
                        // remapping is impossible, fail closed.
                        results,
                        $"Active statement in method 0x%08X{methodToken} cannot be remapped: no sequence points are available for the updated method body. Please rebuild."
                        :: rudeMessages
                    | committed, fresh ->
                        match remapActiveStatement statement committed fresh with
                        | ActiveStatementRemapOutcome.Update update ->
                            FSharpActiveStatementRemapResult.Remapped update :: results, rudeMessages
                        | ActiveStatementRemapOutcome.MethodUpToDate ->
                            FSharpActiveStatementRemapResult.MethodUpToDate statement.ActiveInstruction
                            :: results,
                            rudeMessages
                        | ActiveStatementRemapOutcome.Rude message -> results, message :: rudeMessages)

        match rudeMessages with
        | [] -> Ok(List.rev results)
        | messages -> Error(List.rev messages)
