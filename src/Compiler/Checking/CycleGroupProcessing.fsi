// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

/// Cycle group processing for cross-file mutual recursion (Level B).
///
/// When --file-order-auto detects a strongly connected component (SCC) of files
/// that mutually depend on each other, those files cannot be type-checked
/// independently. This module synthesizes a single ParsedImplFileInput from
/// the cycle group's files, marking the top-level modules as recursive so
/// the existing F# type checker treats them as mutually recursive.
module internal FSharp.Compiler.CycleGroupProcessing

open FSharp.Compiler.Syntax

/// Synthesize a single ParsedImplFileInput from a list of files in a cycle group.
/// All top-level SynModuleOrNamespace entries are marked isRecursive=true so the
/// F# type checker processes them as a mutually-recursive group.
///
/// The synthetic file:
/// - Has a synthetic file name based on the group's first file
/// - Concatenates the contents of all cycle group files
/// - Sets IsLastCompiland based on whether any input file was last
/// - Preserves original ranges so error messages still point to the right files
val synthesizeCycleGroupImpl: groupId: int -> files: ParsedImplFileInput list -> ParsedImplFileInput

/// Same as synthesizeCycleGroupImpl but for signature files.
val synthesizeCycleGroupSig: groupId: int -> files: ParsedSigFileInput list -> ParsedSigFileInput

/// High-level entry point: apply --file-order-auto+ behavior to a list of parsed inputs.
/// Used by both fsc.fs and FCS (BackgroundCompiler/IncrementalBuilder).
///
/// Steps:
/// 1. Run symbol collection enter phase to populate TcEnv with stubs
/// 2. Compute dependency-ordered compilation units (SingleFile or CycleGroup)
/// 3. Synthesize cycle groups via namespace-rec wrapping (when no .fsi files in group)
/// 4. Fix up IsLastCompiland flag on the actual last file
///
/// Returns: (reordered+synthesized inputs, pre-populated TcEnv).
/// The TcEnv is the input env enriched with stubs for all top-level declarations.
val applyAutoFileOrder:
    g: TcGlobals.TcGlobals ->
    amap: Import.ImportMap ->
    tcEnv: CheckBasics.TcEnv ->
    inputs: ParsedInput list ->
        ParsedInput list * CheckBasics.TcEnv

/// Lightweight Level-A-only reorder used by FCS (IncrementalBuilder).
/// Takes parsed inputs (paired with their file paths) and returns the
/// dependency-respecting order WITHOUT synthesizing cycle groups
/// (cycle groups remain in their original positions).
///
/// Returns reordered file paths so FCS can keep its disk-backed source
/// file model intact. Materially less powerful than `applyAutoFileOrder`:
/// - No cycle group synthesis (Level B is build-only)
/// - No TcEnv pre-population (FCS has its own incremental machinery)
/// But sufficient for IDE diagnostics to match `dotnet build` for the
/// common case (projects without cycles).
val computeReorderedFileNames: inputs: (ParsedInput * string) list -> string list
