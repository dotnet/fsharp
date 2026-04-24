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
