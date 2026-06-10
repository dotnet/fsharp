// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Computes metadata table bit masks for delta emission.
///
/// The #~ stream header contains two 64-bit masks:
///   - Valid: which tables have rows (bit set = table present)
///   - Sorted: which tables are sorted (per ECMA-335)
///
/// Uses TableNames from BinaryConstants.fs for ECMA-335 metadata tables,
/// and DeltaTokens for Portable PDB tables (which aren't in TableNames).
module internal FSharp.Compiler.CodeGen.DeltaTableLayout

open FSharp.Compiler.AbstractIL.BinaryConstants
open FSharp.Compiler.AbstractIL.ILDeltaHandles

type TableBitMasks =
    { ValidLow: int
      ValidHigh: int
      SortedLow: int
      SortedHigh: int }

// -------------------------------------------------------------------------
// Sorted Tables (per ECMA-335 II.22)
// -------------------------------------------------------------------------
// These tables must be sorted by their primary key column for binary search.
// The sorted bit mask indicates which tables the runtime can expect to be sorted.

/// ECMA-335 metadata tables that are sorted by primary key
let private sortedTypeSystemTables =
    [ TableNames.InterfaceImpl.Index        // Sorted by Class column
      TableNames.Constant.Index             // Sorted by Parent column
      TableNames.CustomAttribute.Index      // Sorted by Parent column
      TableNames.FieldMarshal.Index         // Sorted by Parent column
      TableNames.Permission.Index           // Sorted by Parent column (DeclSecurity)
      TableNames.ClassLayout.Index          // Sorted by Parent column
      TableNames.FieldLayout.Index          // Sorted by Field column
      TableNames.MethodSemantics.Index      // Sorted by Association column
      TableNames.MethodImpl.Index           // Sorted by Class column
      TableNames.ImplMap.Index              // Sorted by MemberForwarded column
      TableNames.FieldRVA.Index             // Sorted by Field column
      TableNames.Nested.Index               // Sorted by NestedClass column
      TableNames.GenericParam.Index         // Sorted by Owner column
      TableNames.GenericParamConstraint.Index ] // Sorted by Owner column

/// Portable PDB tables that are sorted (not in TableNames, use DeltaTokens)
let private sortedDebugTables =
    [ DeltaTokens.tableLocalScope           // 0x32: Sorted by Method column
      DeltaTokens.tableStateMachineMethod   // 0x36: Sorted by MoveNextMethod column
      DeltaTokens.tableCustomDebugInformation ] // 0x37: Sorted by Parent column

let private maskForTables (tables: int list) =
    tables
    |> List.fold
        (fun acc tableIndex ->
            acc ||| (1UL <<< tableIndex))
        0UL

let private sortedTypeSystemMask = maskForTables sortedTypeSystemTables
let private sortedDebugMask = maskForTables sortedDebugTables

let private toLow (mask: uint64) = int (mask &&& 0xFFFFFFFFUL)
let private toHigh (mask: uint64) = int ((mask >>> 32) &&& 0xFFFFFFFFUL)

/// Compute Valid and Sorted bit masks for the #~ stream header.
///
/// For EnC deltas, CustomAttribute is excluded from the sorted mask
/// to match Roslyn's behavior (it's not pre-sorted in deltas).
let computeBitMasks (tableRowCounts: int[]) (isEncDelta: bool) : TableBitMasks =
    // Valid mask: bit set for each table with rows
    let presentMask =
        tableRowCounts
        |> Array.mapi (fun index count -> if count <> 0 then 1UL <<< index else 0UL)
        |> Array.fold (|||) 0UL

    // Sorted mask: which present tables are sorted
    let typeSystemMask =
        if isEncDelta then
            // Roslyn clears CustomAttribute for EnC deltas to mirror MetadataSizes.
            // CustomAttribute table in deltas is appended, not globally sorted.
            sortedTypeSystemMask &&& ~~~(1UL <<< TableNames.CustomAttribute.Index)
        else
            sortedTypeSystemMask

    // Combine type system sorted tables with present debug tables that are sorted
    let sortedMask = typeSystemMask ||| (presentMask &&& sortedDebugMask)

    { ValidLow = toLow presentMask
      ValidHigh = toHigh presentMask
      SortedLow = toLow sortedMask
      SortedHigh = toHigh sortedMask }
