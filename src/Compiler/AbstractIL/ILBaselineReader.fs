/// Minimal binary reader for baseline metadata extraction.
/// Replaces SRM MetadataReader dependency for hot reload baseline creation.
/// Parses PE/CLI metadata headers to extract heap sizes and table row counts.
///
/// This module provides a pure F# implementation for reading the minimum metadata
/// needed to create an FSharpEmitBaseline, without requiring System.Reflection.Metadata.
///
/// References:
/// - ECMA-335 II.24 (Metadata physical layout)
/// - Roslyn DeltaMetadataWriter.cs for heap offset handling
module internal FSharp.Compiler.AbstractIL.ILBaselineReader

open System
open FSharp.Compiler.AbstractIL.ILBinaryWriter

/// Read a little-endian 16-bit integer from bytes at offset.
let private readUInt16 (bytes: byte[]) (offset: int) =
    uint16 bytes.[offset] ||| (uint16 bytes.[offset + 1] <<< 8)

/// Read a little-endian 32-bit integer from bytes at offset.
let private readInt32 (bytes: byte[]) (offset: int) =
    int bytes.[offset]
    ||| (int bytes.[offset + 1] <<< 8)
    ||| (int bytes.[offset + 2] <<< 16)
    ||| (int bytes.[offset + 3] <<< 24)

/// Read a little-endian 64-bit integer from bytes at offset.
let private readInt64 (bytes: byte[]) (offset: int) =
    int64 (readInt32 bytes offset) ||| (int64 (readInt32 bytes (offset + 4)) <<< 32)

/// Number of metadata tables per ECMA-335.
let private tableCount = 64

/// Find the CLI metadata root in PE file bytes.
/// Returns the offset to the metadata root, or None if not found.
let private findMetadataRoot (bytes: byte[]) : int option =
    // Check DOS header magic
    if bytes.Length < 64 || bytes.[0] <> 0x4Duy || bytes.[1] <> 0x5Auy then
        None
    else
        // e_lfanew at offset 0x3C points to PE signature
        let peOffset = readInt32 bytes 0x3C
        if peOffset < 0 || peOffset + 24 > bytes.Length then
            None
        else
            // Check PE signature "PE\0\0"
            if bytes.[peOffset] <> 0x50uy || bytes.[peOffset+1] <> 0x45uy
               || bytes.[peOffset+2] <> 0uy || bytes.[peOffset+3] <> 0uy then
                None
            else
                // COFF header at peOffset + 4
                let coffHeader = peOffset + 4
                let sizeOfOptionalHeader = int (readUInt16 bytes (coffHeader + 16))
                let optionalHeader = coffHeader + 20

                // PE32 vs PE32+ - check magic
                let magic = readUInt16 bytes optionalHeader
                let isPE32Plus = magic = 0x20Bus

                // CLI header RVA is in data directory entry 14 (0-indexed)
                // PE32: starts at optionalHeader + 96; PE32+: starts at optionalHeader + 112
                let dataDirectoryStart =
                    if isPE32Plus then optionalHeader + 112
                    else optionalHeader + 96

                let cliHeaderRVA = readInt32 bytes (dataDirectoryStart + 14 * 8)

                if cliHeaderRVA = 0 then
                    None
                else
                    // Convert RVA to file offset using section headers
                    let numberOfSections = int (readUInt16 bytes (coffHeader + 2))
                    let sectionHeadersStart = optionalHeader + sizeOfOptionalHeader

                    let rec findSection sectionIndex =
                        if sectionIndex >= numberOfSections then
                            None
                        else
                            let sectionOffset = sectionHeadersStart + sectionIndex * 40
                            let virtualAddress = readInt32 bytes (sectionOffset + 12)
                            let virtualSize = readInt32 bytes (sectionOffset + 8)
                            let pointerToRawData = readInt32 bytes (sectionOffset + 20)

                            if cliHeaderRVA >= virtualAddress && cliHeaderRVA < virtualAddress + virtualSize then
                                let cliHeaderOffset = cliHeaderRVA - virtualAddress + pointerToRawData
                                // CLI header contains MetaData RVA at offset 8
                                let metadataRVA = readInt32 bytes (cliHeaderOffset + 8)
                                // Convert metadata RVA to file offset
                                Some (metadataRVA - virtualAddress + pointerToRawData)
                            else
                                findSection (sectionIndex + 1)

                    findSection 0

/// Stream header information.
type private StreamHeader =
    { Offset: int
      Size: int
      Name: string }

/// Parse stream headers from metadata root.
let private parseStreamHeaders (bytes: byte[]) (metadataRoot: int) : StreamHeader list =
    // Metadata root signature at offset 0
    let signature = readInt32 bytes metadataRoot
    if signature <> 0x424A5342 then // "BSJB"
        []
    else
        // Version string length at offset 12
        let versionLength = readInt32 bytes (metadataRoot + 12)
        let paddedVersionLength = (versionLength + 3) &&& ~~~3

        // Number of streams at offset 16 + paddedVersionLength + 2
        let streamsOffset = metadataRoot + 16 + paddedVersionLength
        let numberOfStreams = int (readUInt16 bytes (streamsOffset + 2))

        // Stream headers start at streamsOffset + 4
        let mutable currentOffset = streamsOffset + 4
        let headers = ResizeArray<StreamHeader>()

        for _ in 1..numberOfStreams do
            let offset = readInt32 bytes currentOffset
            let size = readInt32 bytes (currentOffset + 4)

            // Read null-terminated stream name (padded to 4-byte boundary)
            let mutable nameEnd = currentOffset + 8
            while bytes.[nameEnd] <> 0uy do
                nameEnd <- nameEnd + 1
            let name = System.Text.Encoding.ASCII.GetString(bytes, currentOffset + 8, nameEnd - currentOffset - 8)
            let paddedNameLength = ((nameEnd - currentOffset - 8 + 1) + 3) &&& ~~~3

            headers.Add({ Offset = metadataRoot + offset; Size = size; Name = name })
            currentOffset <- currentOffset + 8 + paddedNameLength

        headers |> Seq.toList

/// Find a stream by name.
let private findStream (headers: StreamHeader list) (name: string) : StreamHeader option =
    headers |> List.tryFind (fun h -> h.Name = name)

/// Parse table row counts from the #~ or #- stream.
/// Returns (heapSizes byte, table row counts array, tables stream offset).
let private parseTablesStream (bytes: byte[]) (tablesStream: StreamHeader) : byte * int[] * int =
    let offset = tablesStream.Offset

    // Header structure:
    // 0-3: Reserved (0)
    // 4: MajorVersion
    // 5: MinorVersion
    // 6: HeapSizes byte
    // 7: Reserved
    // 8-15: Valid (bitmask of present tables)
    // 16-23: Sorted (bitmask of sorted tables)
    // 24+: Row counts for present tables

    let heapSizes = bytes.[offset + 6]
    let valid = readInt64 bytes (offset + 8)

    let rowCounts = Array.zeroCreate tableCount
    let mutable rowCountOffset = offset + 24

    for i in 0..63 do
        if (valid &&& (1L <<< i)) <> 0L then
            rowCounts.[i] <- readInt32 bytes rowCountOffset
            rowCountOffset <- rowCountOffset + 4

    heapSizes, rowCounts, offset

/// Extract metadata snapshot from PE file bytes.
/// This replaces metadataSnapshotFromReader for hot reload baseline creation.
let metadataSnapshotFromBytes (bytes: byte[]) : MetadataSnapshot option =
    match findMetadataRoot bytes with
    | None -> None
    | Some metadataRoot ->
        let streamHeaders = parseStreamHeaders bytes metadataRoot

        // Find required streams
        let stringsStream = findStream streamHeaders "#Strings"
        let userStringsStream = findStream streamHeaders "#US"
        let blobStream = findStream streamHeaders "#Blob"
        let guidStream = findStream streamHeaders "#GUID"
        let tablesStream =
            findStream streamHeaders "#~"
            |> Option.orElse (findStream streamHeaders "#-")

        match tablesStream with
        | None -> None
        | Some tables ->
            let _, rowCounts, _ = parseTablesStream bytes tables

            let heapSizeInfo =
                { StringHeapSize = stringsStream |> Option.map (fun s -> s.Size) |> Option.defaultValue 0
                  UserStringHeapSize = userStringsStream |> Option.map (fun s -> s.Size) |> Option.defaultValue 0
                  BlobHeapSize = blobStream |> Option.map (fun s -> s.Size) |> Option.defaultValue 0
                  GuidHeapSize = guidStream |> Option.map (fun s -> s.Size) |> Option.defaultValue 0 }

            Some
                { HeapSizes = heapSizeInfo
                  TableRowCounts = rowCounts
                  GuidHeapStart = heapSizeInfo.GuidHeapSize }

/// Read GUID from #GUID stream at 1-based index.
let readGuidFromBytes (bytes: byte[]) (guidIndex: int) : Guid option =
    if guidIndex <= 0 then
        None
    else
        match findMetadataRoot bytes with
        | None -> None
        | Some metadataRoot ->
            let streamHeaders = parseStreamHeaders bytes metadataRoot
            match findStream streamHeaders "#GUID" with
            | None -> None
            | Some guidStream ->
                // GUID indices are 1-based; each GUID is 16 bytes
                let offset = guidStream.Offset + (guidIndex - 1) * 16
                if offset + 16 > bytes.Length then
                    None
                else
                    let guidBytes = bytes.[offset..offset+15]
                    Some (System.Guid(guidBytes))

// ============================================================================
// Table row reading infrastructure
// ============================================================================

/// Table indices per ECMA-335 II.22
module private TableIndices =
    let Module = 0
    let TypeRef = 1
    let TypeDef = 2
    let Field = 4
    let MethodDef = 6
    let Param = 8
    let MemberRef = 10
    let Constant = 11
    let CustomAttribute = 12
    let FieldMarshal = 13
    let DeclSecurity = 14
    let ClassLayout = 15
    let FieldLayout = 16
    let StandAloneSig = 17
    let EventMap = 18
    let Event = 20
    let PropertyMap = 21
    let Property = 23
    let MethodSemantics = 24
    let MethodImpl = 25
    let ModuleRef = 26
    let TypeSpec = 27
    let ImplMap = 28
    let FieldRVA = 29
    let Assembly = 32
    let AssemblyRef = 35
    let File = 38
    let ExportedType = 39
    let ManifestResource = 40
    let NestedClass = 41
    let GenericParam = 42
    let MethodSpec = 43
    let GenericParamConstraint = 44

/// Parsed metadata context for reading table rows.
type private MetadataContext = {
    Bytes: byte[]
    HeapSizes: byte
    RowCounts: int[]
    TablesStart: int
    StringIndexSize: int
    GuidIndexSize: int
    BlobIndexSize: int
    StringsStreamOffset: int
    BlobStreamOffset: int
}

/// Calculate index size for a simple table reference (2 if <=65535 rows, else 4).
let private tableIndexSize (rowCounts: int[]) (tableIndex: int) =
    if rowCounts.[tableIndex] <= 65535 then 2 else 4

/// Calculate index size for a coded index (multiple possible tables).
/// The tag takes some bits, so max row must fit in remaining bits.
let private codedIndexSize (rowCounts: int[]) (tableIndices: int[]) (tagBits: int) =
    let maxRows = tableIndices |> Array.map (fun i -> if i < 64 then rowCounts.[i] else 0) |> Array.max
    let maxValue = (maxRows <<< tagBits) ||| ((1 <<< tagBits) - 1)
    if maxValue <= 65535 then 2 else 4

/// ResolutionScope coded index: Module(0), ModuleRef(1), AssemblyRef(2), TypeRef(3) - 2 tag bits
let private resolutionScopeSize (rowCounts: int[]) =
    codedIndexSize rowCounts [| TableIndices.Module; TableIndices.ModuleRef; TableIndices.AssemblyRef; TableIndices.TypeRef |] 2

/// TypeDefOrRef coded index: TypeDef(0), TypeRef(1), TypeSpec(2) - 2 tag bits
let private typeDefOrRefSize (rowCounts: int[]) =
    codedIndexSize rowCounts [| TableIndices.TypeDef; TableIndices.TypeRef; TableIndices.TypeSpec |] 2

/// HasConstant coded index - 2 tag bits
let private hasConstantSize (rowCounts: int[]) =
    codedIndexSize rowCounts [| TableIndices.Field; TableIndices.Param; TableIndices.Property |] 2

/// HasCustomAttribute coded index - 5 tag bits (22 possible tables)
let private hasCustomAttributeSize (rowCounts: int[]) =
    // Simplified: just use the relevant tables
    let tables = [| TableIndices.MethodDef; TableIndices.Field; TableIndices.TypeRef; TableIndices.TypeDef;
                    TableIndices.Param; TableIndices.Property; TableIndices.Event; TableIndices.Assembly;
                    TableIndices.AssemblyRef; TableIndices.ModuleRef; TableIndices.TypeSpec; TableIndices.Module |]
    codedIndexSize rowCounts tables 5

/// HasFieldMarshal coded index - 1 tag bit
let private hasFieldMarshalSize (rowCounts: int[]) =
    codedIndexSize rowCounts [| TableIndices.Field; TableIndices.Param |] 1

/// HasDeclSecurity coded index - 2 tag bits
let private hasDeclSecuritySize (rowCounts: int[]) =
    codedIndexSize rowCounts [| TableIndices.TypeDef; TableIndices.MethodDef; TableIndices.Assembly |] 2

/// MemberRefParent coded index - 3 tag bits
let private memberRefParentSize (rowCounts: int[]) =
    codedIndexSize rowCounts [| TableIndices.TypeDef; TableIndices.TypeRef; TableIndices.ModuleRef;
                                TableIndices.MethodDef; TableIndices.TypeSpec |] 3

/// HasSemantics coded index - 1 tag bit
let private hasSemanticsSize (rowCounts: int[]) =
    codedIndexSize rowCounts [| TableIndices.Event; TableIndices.Property |] 1

/// MethodDefOrRef coded index - 1 tag bit
let private methodDefOrRefSize (rowCounts: int[]) =
    codedIndexSize rowCounts [| TableIndices.MethodDef; TableIndices.MemberRef |] 1

/// MemberForwarded coded index - 1 tag bit
let private memberForwardedSize (rowCounts: int[]) =
    codedIndexSize rowCounts [| TableIndices.Field; TableIndices.MethodDef |] 1

/// Implementation coded index - 2 tag bits
let private implementationSize (rowCounts: int[]) =
    codedIndexSize rowCounts [| TableIndices.File; TableIndices.AssemblyRef; TableIndices.ExportedType |] 2

/// CustomAttributeType coded index - 3 tag bits
let private customAttributeTypeSize (rowCounts: int[]) =
    // Only MethodDef(2) and MemberRef(3) are used
    codedIndexSize rowCounts [| 0; 0; TableIndices.MethodDef; TableIndices.MemberRef; 0 |] 3

/// TypeOrMethodDef coded index - 1 tag bit
let private typeOrMethodDefSize (rowCounts: int[]) =
    codedIndexSize rowCounts [| TableIndices.TypeDef; TableIndices.MethodDef |] 1

/// Calculate row size for each table per ECMA-335 II.22.
let private calculateTableRowSizes (ctx: MetadataContext) : int[] =
    let rc = ctx.RowCounts
    let strIdx = ctx.StringIndexSize
    let guidIdx = ctx.GuidIndexSize
    let blobIdx = ctx.BlobIndexSize

    let sizes = Array.zeroCreate tableCount

    // Module: Generation(2) + Name(str) + Mvid(guid) + EncId(guid) + EncBaseId(guid)
    sizes.[0] <- 2 + strIdx + guidIdx + guidIdx + guidIdx

    // TypeRef: ResolutionScope(coded) + TypeName(str) + TypeNamespace(str)
    sizes.[1] <- resolutionScopeSize rc + strIdx + strIdx

    // TypeDef: Flags(4) + TypeName(str) + TypeNamespace(str) + Extends(TypeDefOrRef) + FieldList(Field) + MethodList(MethodDef)
    sizes.[2] <- 4 + strIdx + strIdx + typeDefOrRefSize rc + tableIndexSize rc 4 + tableIndexSize rc 6

    // Field: Flags(2) + Name(str) + Signature(blob)
    sizes.[4] <- 2 + strIdx + blobIdx

    // MethodDef: RVA(4) + ImplFlags(2) + Flags(2) + Name(str) + Signature(blob) + ParamList(Param)
    sizes.[6] <- 4 + 2 + 2 + strIdx + blobIdx + tableIndexSize rc 8

    // Param: Flags(2) + Sequence(2) + Name(str)
    sizes.[8] <- 2 + 2 + strIdx

    // MemberRef: Class(MemberRefParent) + Name(str) + Signature(blob)
    sizes.[10] <- memberRefParentSize rc + strIdx + blobIdx

    // Constant: Type(2) + Parent(HasConstant) + Value(blob)
    sizes.[11] <- 2 + hasConstantSize rc + blobIdx

    // CustomAttribute: Parent(HasCustomAttribute) + Type(CustomAttributeType) + Value(blob)
    sizes.[12] <- hasCustomAttributeSize rc + customAttributeTypeSize rc + blobIdx

    // FieldMarshal: Parent(HasFieldMarshal) + NativeType(blob)
    sizes.[13] <- hasFieldMarshalSize rc + blobIdx

    // DeclSecurity: Action(2) + Parent(HasDeclSecurity) + PermissionSet(blob)
    sizes.[14] <- 2 + hasDeclSecuritySize rc + blobIdx

    // ClassLayout: PackingSize(2) + ClassSize(4) + Parent(TypeDef)
    sizes.[15] <- 2 + 4 + tableIndexSize rc 2

    // FieldLayout: Offset(4) + Field(Field)
    sizes.[16] <- 4 + tableIndexSize rc 4

    // StandAloneSig: Signature(blob)
    sizes.[17] <- blobIdx

    // EventMap: Parent(TypeDef) + EventList(Event)
    sizes.[18] <- tableIndexSize rc 2 + tableIndexSize rc 20

    // Event: EventFlags(2) + Name(str) + EventType(TypeDefOrRef)
    sizes.[20] <- 2 + strIdx + typeDefOrRefSize rc

    // PropertyMap: Parent(TypeDef) + PropertyList(Property)
    sizes.[21] <- tableIndexSize rc 2 + tableIndexSize rc 23

    // Property: Flags(2) + Name(str) + Type(blob)
    sizes.[23] <- 2 + strIdx + blobIdx

    // MethodSemantics: Semantics(2) + Method(MethodDef) + Association(HasSemantics)
    sizes.[24] <- 2 + tableIndexSize rc 6 + hasSemanticsSize rc

    // MethodImpl: Class(TypeDef) + MethodBody(MethodDefOrRef) + MethodDeclaration(MethodDefOrRef)
    sizes.[25] <- tableIndexSize rc 2 + methodDefOrRefSize rc + methodDefOrRefSize rc

    // ModuleRef: Name(str)
    sizes.[26] <- strIdx

    // TypeSpec: Signature(blob)
    sizes.[27] <- blobIdx

    // ImplMap: MappingFlags(2) + MemberForwarded(MemberForwarded) + ImportName(str) + ImportScope(ModuleRef)
    sizes.[28] <- 2 + memberForwardedSize rc + strIdx + tableIndexSize rc 26

    // FieldRVA: RVA(4) + Field(Field)
    sizes.[29] <- 4 + tableIndexSize rc 4

    // Assembly: HashAlgId(4) + MajorVersion(2) + MinorVersion(2) + BuildNumber(2) + RevisionNumber(2) +
    //           Flags(4) + PublicKey(blob) + Name(str) + Culture(str)
    sizes.[32] <- 4 + 2 + 2 + 2 + 2 + 4 + blobIdx + strIdx + strIdx

    // AssemblyRef: MajorVersion(2) + MinorVersion(2) + BuildNumber(2) + RevisionNumber(2) +
    //              Flags(4) + PublicKeyOrToken(blob) + Name(str) + Culture(str) + HashValue(blob)
    sizes.[35] <- 2 + 2 + 2 + 2 + 4 + blobIdx + strIdx + strIdx + blobIdx

    // File: Flags(4) + Name(str) + HashValue(blob)
    sizes.[38] <- 4 + strIdx + blobIdx

    // ExportedType: Flags(4) + TypeDefId(4) + TypeName(str) + TypeNamespace(str) + Implementation(Implementation)
    sizes.[39] <- 4 + 4 + strIdx + strIdx + implementationSize rc

    // ManifestResource: Offset(4) + Flags(4) + Name(str) + Implementation(Implementation)
    sizes.[40] <- 4 + 4 + strIdx + implementationSize rc

    // NestedClass: NestedClass(TypeDef) + EnclosingClass(TypeDef)
    sizes.[41] <- tableIndexSize rc 2 + tableIndexSize rc 2

    // GenericParam: Number(2) + Flags(2) + Owner(TypeOrMethodDef) + Name(str)
    sizes.[42] <- 2 + 2 + typeOrMethodDefSize rc + strIdx

    // MethodSpec: Method(MethodDefOrRef) + Instantiation(blob)
    sizes.[43] <- methodDefOrRefSize rc + blobIdx

    // GenericParamConstraint: Owner(GenericParam) + Constraint(TypeDefOrRef)
    sizes.[44] <- tableIndexSize rc 42 + typeDefOrRefSize rc

    sizes

/// Calculate the byte offset where each table starts within the tables stream.
let private calculateTableOffsets (ctx: MetadataContext) (rowSizes: int[]) : int[] =
    let offsets = Array.zeroCreate tableCount
    let mutable currentOffset = ctx.TablesStart

    for i in 0..tableCount-1 do
        offsets.[i] <- currentOffset
        currentOffset <- currentOffset + rowSizes.[i] * ctx.RowCounts.[i]

    offsets

/// Read a heap index (2 or 4 bytes) from the given offset.
let private readHeapIndex (bytes: byte[]) (offset: int) (indexSize: int) =
    if indexSize = 2 then int (readUInt16 bytes offset) else readInt32 bytes offset

/// Create a metadata context for reading table rows.
let private createMetadataContext (bytes: byte[]) : MetadataContext option =
    match findMetadataRoot bytes with
    | None -> None
    | Some metadataRoot ->
        let streamHeaders = parseStreamHeaders bytes metadataRoot
        let tablesStreamOpt =
            findStream streamHeaders "#~"
            |> Option.orElse (findStream streamHeaders "#-")

        match tablesStreamOpt with
        | None -> None
        | Some tablesStream ->
            let heapSizes, rowCounts, tablesOffset = parseTablesStream bytes tablesStream

            let stringsBig = (heapSizes &&& 0x01uy) <> 0uy
            let guidsBig = (heapSizes &&& 0x02uy) <> 0uy
            let blobsBig = (heapSizes &&& 0x04uy) <> 0uy

            // Calculate where row data starts (after row count array)
            let mutable rowCountSize = 0
            for i in 0..63 do
                if rowCounts.[i] > 0 then
                    rowCountSize <- rowCountSize + 4
            let tablesStart = tablesOffset + 24 + rowCountSize

            let stringsOffset = streamHeaders |> List.tryFind (fun h -> h.Name = "#Strings") |> Option.map (fun h -> h.Offset) |> Option.defaultValue 0
            let blobOffset = streamHeaders |> List.tryFind (fun h -> h.Name = "#Blob") |> Option.map (fun h -> h.Offset) |> Option.defaultValue 0

            Some {
                Bytes = bytes
                HeapSizes = heapSizes
                RowCounts = rowCounts
                TablesStart = tablesStart
                StringIndexSize = if stringsBig then 4 else 2
                GuidIndexSize = if guidsBig then 4 else 2
                BlobIndexSize = if blobsBig then 4 else 2
                StringsStreamOffset = stringsOffset
                BlobStreamOffset = blobOffset
            }

/// Read a null-terminated string from the #Strings heap.
let private readStringFromHeap (ctx: MetadataContext) (offset: int) : string =
    if offset = 0 then ""
    else
        let start = ctx.StringsStreamOffset + offset
        let mutable endPos = start
        while ctx.Bytes.[endPos] <> 0uy do
            endPos <- endPos + 1
        System.Text.Encoding.UTF8.GetString(ctx.Bytes, start, endPos - start)

// ============================================================================
// Table row reading functions
// ============================================================================

/// MethodDef row data needed for baseline cache.
type MethodDefRowData = {
    RVA: int
    ImplFlags: int
    Flags: int
    NameOffset: int
    SignatureOffset: int
    ParamList: int  // First Param row ID (1-based)
}

/// Read a MethodDef row by 1-based row ID.
let private readMethodDefRow (ctx: MetadataContext) (rowSizes: int[]) (tableOffsets: int[]) (rowId: int) : MethodDefRowData option =
    if rowId < 1 || rowId > ctx.RowCounts.[TableIndices.MethodDef] then
        None
    else
        let rowSize = rowSizes.[TableIndices.MethodDef]
        let offset = tableOffsets.[TableIndices.MethodDef] + (rowId - 1) * rowSize
        let bytes = ctx.Bytes

        // MethodDef: RVA(4) + ImplFlags(2) + Flags(2) + Name(str) + Signature(blob) + ParamList(Param)
        let rva = readInt32 bytes offset
        let implFlags = int (readUInt16 bytes (offset + 4))
        let flags = int (readUInt16 bytes (offset + 6))
        let nameOffset = readHeapIndex bytes (offset + 8) ctx.StringIndexSize
        let sigOffset = readHeapIndex bytes (offset + 8 + ctx.StringIndexSize) ctx.BlobIndexSize
        let paramList = readHeapIndex bytes (offset + 8 + ctx.StringIndexSize + ctx.BlobIndexSize) (tableIndexSize ctx.RowCounts TableIndices.Param)

        Some { RVA = rva; ImplFlags = implFlags; Flags = flags; NameOffset = nameOffset; SignatureOffset = sigOffset; ParamList = paramList }

/// Param row data.
type ParamRowData = {
    Flags: int
    Sequence: int
    NameOffset: int
}

/// Read a Param row by 1-based row ID.
let private readParamRow (ctx: MetadataContext) (rowSizes: int[]) (tableOffsets: int[]) (rowId: int) : ParamRowData option =
    if rowId < 1 || rowId > ctx.RowCounts.[TableIndices.Param] then
        None
    else
        let rowSize = rowSizes.[TableIndices.Param]
        let offset = tableOffsets.[TableIndices.Param] + (rowId - 1) * rowSize
        let bytes = ctx.Bytes

        // Param: Flags(2) + Sequence(2) + Name(str)
        let flags = int (readUInt16 bytes offset)
        let sequence = int (readUInt16 bytes (offset + 2))
        let nameOffset = readHeapIndex bytes (offset + 4) ctx.StringIndexSize

        Some { Flags = flags; Sequence = sequence; NameOffset = nameOffset }

/// Property row data.
type PropertyRowData = {
    Flags: int
    NameOffset: int
    SignatureOffset: int
}

/// Read a Property row by 1-based row ID.
let private readPropertyRow (ctx: MetadataContext) (rowSizes: int[]) (tableOffsets: int[]) (rowId: int) : PropertyRowData option =
    if rowId < 1 || rowId > ctx.RowCounts.[TableIndices.Property] then
        None
    else
        let rowSize = rowSizes.[TableIndices.Property]
        let offset = tableOffsets.[TableIndices.Property] + (rowId - 1) * rowSize
        let bytes = ctx.Bytes

        // Property: Flags(2) + Name(str) + Type(blob)
        let flags = int (readUInt16 bytes offset)
        let nameOffset = readHeapIndex bytes (offset + 2) ctx.StringIndexSize
        let sigOffset = readHeapIndex bytes (offset + 2 + ctx.StringIndexSize) ctx.BlobIndexSize

        Some { Flags = flags; NameOffset = nameOffset; SignatureOffset = sigOffset }

/// Event row data.
type EventRowData = {
    Flags: int
    NameOffset: int
    EventType: int  // Coded index (TypeDefOrRef)
}

/// Read an Event row by 1-based row ID.
let private readEventRow (ctx: MetadataContext) (rowSizes: int[]) (tableOffsets: int[]) (rowId: int) : EventRowData option =
    if rowId < 1 || rowId > ctx.RowCounts.[TableIndices.Event] then
        None
    else
        let rowSize = rowSizes.[TableIndices.Event]
        let offset = tableOffsets.[TableIndices.Event] + (rowId - 1) * rowSize
        let bytes = ctx.Bytes

        // Event: EventFlags(2) + Name(str) + EventType(TypeDefOrRef)
        let flags = int (readUInt16 bytes offset)
        let nameOffset = readHeapIndex bytes (offset + 2) ctx.StringIndexSize

        Some { Flags = flags; NameOffset = nameOffset; EventType = 0 }

/// TypeRef row data.
type TypeRefRowData = {
    ResolutionScope: int  // Coded index
    NameOffset: int
    NamespaceOffset: int
}

/// Read a TypeRef row by 1-based row ID.
let private readTypeRefRow (ctx: MetadataContext) (rowSizes: int[]) (tableOffsets: int[]) (rowId: int) : TypeRefRowData option =
    if rowId < 1 || rowId > ctx.RowCounts.[TableIndices.TypeRef] then
        None
    else
        let rowSize = rowSizes.[TableIndices.TypeRef]
        let offset = tableOffsets.[TableIndices.TypeRef] + (rowId - 1) * rowSize
        let bytes = ctx.Bytes
        let resScopeSize = resolutionScopeSize ctx.RowCounts

        // TypeRef: ResolutionScope(coded) + TypeName(str) + TypeNamespace(str)
        let resScope = readHeapIndex bytes offset resScopeSize
        let nameOffset = readHeapIndex bytes (offset + resScopeSize) ctx.StringIndexSize
        let nsOffset = readHeapIndex bytes (offset + resScopeSize + ctx.StringIndexSize) ctx.StringIndexSize

        Some { ResolutionScope = resScope; NameOffset = nameOffset; NamespaceOffset = nsOffset }

/// AssemblyRef row data.
type AssemblyRefRowData = {
    MajorVersion: int
    MinorVersion: int
    BuildNumber: int
    RevisionNumber: int
    Flags: int
    PublicKeyOrToken: int  // Blob offset
    NameOffset: int
    Culture: int  // String offset
    HashValue: int  // Blob offset
}

/// Read an AssemblyRef row by 1-based row ID.
let private readAssemblyRefRow (ctx: MetadataContext) (rowSizes: int[]) (tableOffsets: int[]) (rowId: int) : AssemblyRefRowData option =
    if rowId < 1 || rowId > ctx.RowCounts.[TableIndices.AssemblyRef] then
        None
    else
        let rowSize = rowSizes.[TableIndices.AssemblyRef]
        let offset = tableOffsets.[TableIndices.AssemblyRef] + (rowId - 1) * rowSize
        let bytes = ctx.Bytes

        // AssemblyRef: MajorVersion(2) + MinorVersion(2) + BuildNumber(2) + RevisionNumber(2) +
        //              Flags(4) + PublicKeyOrToken(blob) + Name(str) + Culture(str) + HashValue(blob)
        let major = int (readUInt16 bytes offset)
        let minor = int (readUInt16 bytes (offset + 2))
        let build = int (readUInt16 bytes (offset + 4))
        let rev = int (readUInt16 bytes (offset + 6))
        let flags = readInt32 bytes (offset + 8)
        let pkOffset = readHeapIndex bytes (offset + 12) ctx.BlobIndexSize
        let nameOffset = readHeapIndex bytes (offset + 12 + ctx.BlobIndexSize) ctx.StringIndexSize
        let cultureOffset = readHeapIndex bytes (offset + 12 + ctx.BlobIndexSize + ctx.StringIndexSize) ctx.StringIndexSize
        let hashOffset = readHeapIndex bytes (offset + 12 + ctx.BlobIndexSize + ctx.StringIndexSize + ctx.StringIndexSize) ctx.BlobIndexSize

        Some {
            MajorVersion = major
            MinorVersion = minor
            BuildNumber = build
            RevisionNumber = rev
            Flags = flags
            PublicKeyOrToken = pkOffset
            NameOffset = nameOffset
            Culture = cultureOffset
            HashValue = hashOffset
        }

/// Module row data (including name offset).
type ModuleRowData = {
    Generation: int
    NameOffset: int
    MvidIndex: int
    EncIdIndex: int
    EncBaseIdIndex: int
}

/// Read the Module row (there's only one, row 1).
let private readModuleRow (ctx: MetadataContext) (_rowSizes: int[]) (tableOffsets: int[]) : ModuleRowData option =
    if ctx.RowCounts.[TableIndices.Module] < 1 then
        None
    else
        let offset = tableOffsets.[TableIndices.Module]
        let bytes = ctx.Bytes

        // Module: Generation(2) + Name(str) + Mvid(guid) + EncId(guid) + EncBaseId(guid)
        let generation = int (readUInt16 bytes offset)
        let nameOffset = readHeapIndex bytes (offset + 2) ctx.StringIndexSize
        let mvidIndex = readHeapIndex bytes (offset + 2 + ctx.StringIndexSize) ctx.GuidIndexSize
        let encIdIndex = readHeapIndex bytes (offset + 2 + ctx.StringIndexSize + ctx.GuidIndexSize) ctx.GuidIndexSize
        let encBaseIdIndex = readHeapIndex bytes (offset + 2 + ctx.StringIndexSize + ctx.GuidIndexSize + ctx.GuidIndexSize) ctx.GuidIndexSize

        Some { Generation = generation; NameOffset = nameOffset; MvidIndex = mvidIndex; EncIdIndex = encIdIndex; EncBaseIdIndex = encBaseIdIndex }

// ============================================================================
// Public API for baseline metadata extraction
// ============================================================================

/// Baseline metadata reader that provides access to table rows without SRM.
type BaselineMetadataReader private (ctx: MetadataContext, rowSizes: int[], tableOffsets: int[]) =

    /// Create a reader from PE file bytes.
    static member Create(bytes: byte[]) : BaselineMetadataReader option =
        match createMetadataContext bytes with
        | None -> None
        | Some ctx ->
            let rowSizes = calculateTableRowSizes ctx
            let tableOffsets = calculateTableOffsets ctx rowSizes
            Some (BaselineMetadataReader(ctx, rowSizes, tableOffsets))

    /// Get the table row counts.
    member _.RowCounts = ctx.RowCounts

    /// Read a MethodDef row by 1-based row ID.
    member _.GetMethodDef(rowId: int) = readMethodDefRow ctx rowSizes tableOffsets rowId

    /// Read a Param row by 1-based row ID.
    member _.GetParam(rowId: int) = readParamRow ctx rowSizes tableOffsets rowId

    /// Get the last param row for a method (based on next method's ParamList or table end).
    member this.GetMethodParamRange(methodRowId: int) : (int * int) option =
        match this.GetMethodDef(methodRowId) with
        | None -> None
        | Some methodDef ->
            let firstParam = methodDef.ParamList
            let lastParam =
                if methodRowId < ctx.RowCounts.[TableIndices.MethodDef] then
                    match this.GetMethodDef(methodRowId + 1) with
                    | Some next -> next.ParamList - 1
                    | None -> ctx.RowCounts.[TableIndices.Param]
                else
                    ctx.RowCounts.[TableIndices.Param]
            if firstParam > lastParam then None
            else Some (firstParam, lastParam)

    /// Read a Property row by 1-based row ID.
    member _.GetProperty(rowId: int) = readPropertyRow ctx rowSizes tableOffsets rowId

    /// Read an Event row by 1-based row ID.
    member _.GetEvent(rowId: int) = readEventRow ctx rowSizes tableOffsets rowId

    /// Read a TypeRef row by 1-based row ID.
    member _.GetTypeRef(rowId: int) = readTypeRefRow ctx rowSizes tableOffsets rowId

    /// Read an AssemblyRef row by 1-based row ID.
    member _.GetAssemblyRef(rowId: int) = readAssemblyRefRow ctx rowSizes tableOffsets rowId

    /// Get the AssemblyRef row count.
    member _.AssemblyRefCount = ctx.RowCounts.[TableIndices.AssemblyRef]

    /// Get the TypeRef row count.
    member _.TypeRefCount = ctx.RowCounts.[TableIndices.TypeRef]

    /// Read the Module row.
    member _.GetModule() = readModuleRow ctx rowSizes tableOffsets

    /// Read a string from the #Strings heap.
    member _.GetString(offset: int) = readStringFromHeap ctx offset

    /// Decode ResolutionScope coded index to (table index, row id).
    /// Tag bits: 0=Module, 1=ModuleRef, 2=AssemblyRef, 3=TypeRef
    member _.DecodeResolutionScope(codedIndex: int) : (int * int) =
        let tag = codedIndex &&& 0x3
        let rowId = codedIndex >>> 2
        let tableIndex =
            match tag with
            | 0 -> TableIndices.Module
            | 1 -> TableIndices.ModuleRef
            | 2 -> TableIndices.AssemblyRef
            | 3 -> TableIndices.TypeRef
            | _ -> -1
        (tableIndex, rowId)

/// Read Module.Mvid GUID from assembly bytes.
/// Module table row 1 contains the Mvid index.
let readModuleMvidFromBytes (bytes: byte[]) : System.Guid option =
    match findMetadataRoot bytes with
    | None -> None
    | Some metadataRoot ->
        let streamHeaders = parseStreamHeaders bytes metadataRoot
        let tablesStreamOpt =
            findStream streamHeaders "#~"
            |> Option.orElse (findStream streamHeaders "#-")

        match tablesStreamOpt with
        | None -> None
        | Some tablesStream ->
            let heapSizes, rowCounts, tablesOffset = parseTablesStream bytes tablesStream

            // Check if Module table has at least 1 row
            if rowCounts.[0] < 1 then
                None
            else
                // Calculate offset to Module row
                // Module row structure: Generation (2), Name (string), Mvid (guid), EncId (guid), EncBaseId (guid)
                let stringsBig = (heapSizes &&& 0x01uy) <> 0uy
                let guidsBig = (heapSizes &&& 0x02uy) <> 0uy

                let stringIndexSize = if stringsBig then 4 else 2

                // Row counts end, then rows start
                let mutable rowCountSize = 0
                for i in 0..63 do
                    if rowCounts.[i] > 0 then
                        rowCountSize <- rowCountSize + 4

                let tablesStart = tablesOffset + 24 + rowCountSize

                // Module table is table 0, so it starts at tablesStart
                // Module row: Generation (2) + Name (string index) + Mvid (guid index) + EncId (guid index) + EncBaseId (guid index)
                let mvidOffset = tablesStart + 2 + stringIndexSize

                let mvidIndex =
                    if guidsBig then
                        readInt32 bytes mvidOffset
                    else
                        int (readUInt16 bytes mvidOffset)

                readGuidFromBytes bytes mvidIndex

// ============================================================================
// Portable PDB Reader
// ============================================================================

/// Portable PDB table indices (start at 0x30 to avoid collision with ECMA-335 tables)
module private PdbTableIndices =
    let Document = 0x30
    let MethodDebugInformation = 0x31
    let LocalScope = 0x32
    let LocalVariable = 0x33
    let LocalConstant = 0x34
    let ImportScope = 0x35
    let StateMachineMethod = 0x36
    let CustomDebugInformation = 0x37

/// Portable PDB metadata snapshot.
/// Contains table row counts and entry point info for hot reload baseline.
type PortablePdbMetadata = {
    /// Row counts for PDB tables (indexed by PDB table index - 0x30)
    /// Index 0 = Document, 1 = MethodDebugInformation, etc.
    TableRowCounts: int[]
    /// Entry point method token (if present)
    EntryPointToken: int option
}

/// Parse the #Pdb stream to extract PDB-specific info.
/// The #Pdb stream contains: PdbId (20 bytes), EntryPoint token (4 bytes), ReferencedTypeSystemTables (8 bytes), TypeSystemTableRows (var)
let private parsePdbStream (bytes: byte[]) (pdbStream: StreamHeader) : int option =
    if pdbStream.Size < 24 then
        None
    else
        let offset = pdbStream.Offset
        // PdbId: 20 bytes (GUID + 4 bytes stamp)
        // EntryPoint: 4 bytes (method def token, or 0 if no entry point)
        let entryPointToken = readInt32 bytes (offset + 20)
        if entryPointToken = 0 then None else Some entryPointToken

/// Parse Portable PDB table row counts from the #~ stream.
/// Portable PDB uses tables 0x30-0x37, but the valid bits are still in position 0x30+.
let private parsePdbTablesStream (bytes: byte[]) (tablesStream: StreamHeader) : int[] =
    let offset = tablesStream.Offset

    // Header: Reserved(4) + MajorVersion(1) + MinorVersion(1) + HeapSizes(1) + Reserved(1) + Valid(8) + Sorted(8) + RowCounts(var)
    let valid = readInt64 bytes (offset + 8)

    // PDB table row counts (8 tables, indices 0x30-0x37)
    let pdbRowCounts = Array.zeroCreate 8
    let mutable rowCountOffset = offset + 24

    for i in 0..63 do
        if (valid &&& (1L <<< i)) <> 0L then
            let count = readInt32 bytes rowCountOffset
            // Map table index to PDB array index
            if i >= 0x30 && i <= 0x37 then
                pdbRowCounts.[i - 0x30] <- count
            rowCountOffset <- rowCountOffset + 4

    pdbRowCounts

/// Extract metadata from Portable PDB bytes.
/// This replaces MetadataReaderProvider.FromPortablePdbImage for hot reload baseline creation.
let readPortablePdbMetadata (pdbBytes: byte[]) : PortablePdbMetadata option =
    // Portable PDB starts directly with metadata root (no PE header)
    // Check for BSJB signature at offset 0
    if pdbBytes.Length < 4 then
        None
    else
        try
            let signature = readInt32 pdbBytes 0
            if signature <> 0x424A5342 then // "BSJB"
                None
            else
                // Parse from offset 0 (metadata root)
                let metadataRoot = 0
                let streamHeaders = parseStreamHeaders pdbBytes metadataRoot

                // Find required streams
                let tablesStreamOpt =
                    findStream streamHeaders "#~"
                    |> Option.orElse (findStream streamHeaders "#-")
                let pdbStreamOpt = findStream streamHeaders "#Pdb"

                match tablesStreamOpt with
                | None -> None
                | Some tablesStream ->
                    let rowCounts = parsePdbTablesStream pdbBytes tablesStream
                    let entryPoint = pdbStreamOpt |> Option.bind (fun s -> parsePdbStream pdbBytes s)

                    Some {
                        TableRowCounts = rowCounts
                        EntryPointToken = entryPoint
                    }
        with
        | :? System.IndexOutOfRangeException -> None
        | :? System.ArgumentOutOfRangeException -> None
