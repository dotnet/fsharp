// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Computes coded index sizing for delta metadata emission.
///
/// This module determines whether various metadata indices require 2 or 4 bytes
/// based on row counts in the metadata tables. This is per ECMA-335 II.24.2.6.
///
/// Uses TableNames from BinaryConstants.fs for ECMA-335 metadata table indices,
/// following the same pattern as the baseline IL writer (ilwrite.fs).
module internal FSharp.Compiler.CodeGen.DeltaIndexSizing

open FSharp.Compiler.AbstractIL.BinaryConstants
open FSharp.Compiler.AbstractIL.ILDeltaHandles
open FSharp.Compiler.CodeGen.DeltaMetadataEncoding

type MetadataHeapSizes = FSharp.Compiler.AbstractIL.ILBinaryWriter.MetadataHeapSizes

/// Holds computed "bigness" flags for all coded index types.
/// When true, the index requires 4 bytes; when false, 2 bytes suffice.
type CodedIndexSizes =
    { StringsBig: bool
      GuidsBig: bool
      BlobsBig: bool
      SimpleIndexBig: bool[]
      TypeDefOrRefBig: bool
      TypeOrMethodDefBig: bool
      HasConstantBig: bool
      HasCustomAttributeBig: bool
      HasFieldMarshalBig: bool
      HasDeclSecurityBig: bool
      MemberRefParentBig: bool
      HasSemanticsBig: bool
      MethodDefOrRefBig: bool
      MemberForwardedBig: bool
      ImplementationBig: bool
      CustomAttributeTypeBig: bool
      ResolutionScopeBig: bool }

let private tableSize (tableRowCounts: int[]) (table: int) =
    tableRowCounts.[table]

let private totalRowCount
    (tableRowCounts: int[])
    (externalRowCounts: int[])
    (table: int)
    =
    let index = table
    let external =
        if externalRowCounts.Length = tableRowCounts.Length then
            externalRowCounts.[index]
        else
            0
    tableRowCounts.[index] + external

let private referenceExceedsLimit
    (tableRowCounts: int[])
    (externalRowCounts: int[])
    (maxValueExclusive: int)
    (tables: int[])
    =
    tables
    |> Array.exists (fun table ->
        totalRowCount tableRowCounts externalRowCounts table >= maxValueExclusive)

/// Determines if a coded index requires 4 bytes (big) or 2 bytes (small).
/// For EnC deltas (uncompressed), all indices are 4 bytes.
/// For compressed metadata, size depends on whether any referenced table
/// has enough rows to overflow the available bits after the tag.
let private codedBigness
    (tagBits: int)
    (tableRowCounts: int[])
    (externalRowCounts: int[])
    (isCompressed: bool)
    (tables: int[])
    =
    if not isCompressed then
        // EnC deltas always use 4-byte indices
        true
    else
        let limit = pown 2 (16 - tagBits)
        referenceExceedsLimit tableRowCounts externalRowCounts limit tables

let private isSimpleIndexBig
    (tableRowCounts: int[])
    (externalRowCounts: int[])
    (isCompressed: bool)
    (tableIndex: int)
    =
    if not isCompressed then
        true
    else
        let local =
            if tableIndex < tableRowCounts.Length then tableRowCounts.[tableIndex] else 0
        let external =
            if tableIndex < externalRowCounts.Length then externalRowCounts.[tableIndex] else 0
        local + external >= 0x10000

/// Compute coded index sizes for all index types.
/// This determines the byte width of each reference type in the metadata tables.
let compute
    (tableRowCounts: int[])
    (externalRowCounts: int[])
    (heapSizes: MetadataHeapSizes)
    (isEncDelta: bool)
    : CodedIndexSizes =

    let isCompressed = not isEncDelta

    // Heap indices: 4 bytes if uncompressed or heap >= 64KB
    let stringsBig = (not isCompressed) || heapSizes.StringHeapSize >= 0x10000
    let blobsBig = (not isCompressed) || heapSizes.BlobHeapSize >= 0x10000
    let guidsBig = (not isCompressed) || heapSizes.GuidHeapSize >= 0x10000

    // Simple table indices
    let simpleIndexBig =
        Array.init DeltaTokens.TableCount (fun i ->
            isSimpleIndexBig tableRowCounts externalRowCounts isCompressed i)

    // Helper to compute coded index bigness for a set of tables
    let coded tag tables =
        codedBigness tag tableRowCounts externalRowCounts isCompressed tables

    // -------------------------------------------------------------------------
    // Coded Index Definitions (per ECMA-335 II.24.2.6)
    // -------------------------------------------------------------------------
    // Each coded index combines a tag (to identify which table) with a row index.
    // The tag uses the low N bits; the row index uses the remaining bits.
    // If any table in the coded index exceeds (2^(16-N) - 1) rows, we need 4 bytes.

    // TypeDefOrRef: TypeDef(0), TypeRef(1), TypeSpec(2) - 2-bit tag
    let typeDefOrRefBig =
        coded CodedIndices.TypeDefOrRef.TagBits CodedIndices.TypeDefOrRef.Tables

    // TypeOrMethodDef: TypeDef(0), MethodDef(1) - 1-bit tag
    let typeOrMethodDefBig =
        coded CodedIndices.TypeOrMethodDef.TagBits CodedIndices.TypeOrMethodDef.Tables

    // HasConstant: Field(0), Param(1), Property(2) - 2-bit tag
    let hasConstantBig =
        coded CodedIndices.HasConstant.TagBits CodedIndices.HasConstant.Tables

    // HasCustomAttribute: 22 possible parent types - 5-bit tag
    // This is the largest coded index, covering most metadata entities
    let hasCustomAttributeBig =
        coded CodedIndices.HasCustomAttribute.TagBits CodedIndices.HasCustomAttribute.Tables

    // HasFieldMarshal: Field(0), Param(1) - 1-bit tag
    let hasFieldMarshalBig =
        coded CodedIndices.HasFieldMarshal.TagBits CodedIndices.HasFieldMarshal.Tables

    // HasDeclSecurity: TypeDef(0), MethodDef(1), Assembly(2) - 2-bit tag
    let hasDeclSecurityBig =
        coded CodedIndices.HasDeclSecurity.TagBits CodedIndices.HasDeclSecurity.Tables

    // MemberRefParent: TypeDef(0), TypeRef(1), ModuleRef(2), MethodDef(3), TypeSpec(4) - 3-bit tag
    let memberRefParentBig =
        coded CodedIndices.MemberRefParent.TagBits CodedIndices.MemberRefParent.Tables

    // HasSemantics: Event(0), Property(1) - 1-bit tag
    let hasSemanticsBig =
        coded CodedIndices.HasSemantics.TagBits CodedIndices.HasSemantics.Tables

    // MethodDefOrRef: MethodDef(0), MemberRef(1) - 1-bit tag
    let methodDefOrRefBig =
        coded CodedIndices.MethodDefOrRef.TagBits CodedIndices.MethodDefOrRef.Tables

    // MemberForwarded: Field(0), MethodDef(1) - 1-bit tag
    let memberForwardedBig =
        coded CodedIndices.MemberForwarded.TagBits CodedIndices.MemberForwarded.Tables

    // Implementation: File(0), AssemblyRef(1), ExportedType(2) - 2-bit tag
    let implementationBig =
        coded CodedIndices.Implementation.TagBits CodedIndices.Implementation.Tables

    // CustomAttributeType: MethodDef(2), MemberRef(3) - 3-bit tag
    // Note: tags 0, 1, 4 are reserved/unused
    let customAttributeTypeBig =
        coded CodedIndices.CustomAttributeType.TagBits CodedIndices.CustomAttributeType.Tables

    // ResolutionScope: Module(0), ModuleRef(1), AssemblyRef(2), TypeRef(3) - 2-bit tag
    let resolutionScopeBig =
        coded CodedIndices.ResolutionScope.TagBits CodedIndices.ResolutionScope.Tables

    { StringsBig = stringsBig
      GuidsBig = guidsBig
      BlobsBig = blobsBig
      SimpleIndexBig = simpleIndexBig
      TypeDefOrRefBig = typeDefOrRefBig
      TypeOrMethodDefBig = typeOrMethodDefBig
      HasConstantBig = hasConstantBig
      HasCustomAttributeBig = hasCustomAttributeBig
      HasFieldMarshalBig = hasFieldMarshalBig
      HasDeclSecurityBig = hasDeclSecurityBig
      MemberRefParentBig = memberRefParentBig
      HasSemanticsBig = hasSemanticsBig
      MethodDefOrRefBig = methodDefOrRefBig
      MemberForwardedBig = memberForwardedBig
      ImplementationBig = implementationBig
      CustomAttributeTypeBig = customAttributeTypeBig
      ResolutionScopeBig = resolutionScopeBig }
