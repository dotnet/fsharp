// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// <summary>
/// Active-statement and sequence-point-update model for F# hot reload (Phase G).
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
[<RequireQualifiedAccess>]
type FSharpActiveStatementRemapResult =
    /// <summary>The containing method was recompiled by this delta; the statement maps to a new span.</summary>
    | Remapped of update: FSharpManagedActiveStatementUpdate
    /// <summary>The containing method is untouched by this delta; the executing version remains current.</summary>
    | MethodUpToDate of instruction: FSharpManagedInstructionId
