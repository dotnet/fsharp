/// Minimal binary reader for baseline PE and portable PDB metadata.
module internal FSharp.Compiler.CodeGen.ILBaselineReader

open System
open System.Text

type MetadataHeapSizes =
    {
        StringHeapSize: int
        UserStringHeapSize: int
        BlobHeapSize: int
        GuidHeapSize: int
    }

type MetadataSnapshot =
    {
        HeapSizes: MetadataHeapSizes
        TableRowCounts: int[]
        GuidHeapStart: int
    }

type PortablePdbMetadata =
    {
        TableRowCounts: int[]
        EntryPointToken: int option
    }

let private readUInt16 (bytes: byte[]) (offset: int) =
    uint16 bytes[offset] ||| (uint16 bytes[offset + 1] <<< 8)

let private readInt32 (bytes: byte[]) (offset: int) =
    int bytes[offset]
    ||| (int bytes[offset + 1] <<< 8)
    ||| (int bytes[offset + 2] <<< 16)
    ||| (int bytes[offset + 3] <<< 24)

let private readInt64 (bytes: byte[]) (offset: int) =
    int64 (uint32 (readInt32 bytes offset))
    ||| (int64 (uint32 (readInt32 bytes (offset + 4))) <<< 32)

[<Literal>]
let private tableCount = 64

module private TableIndices =
    let Module = 0
    let TypeRef = 1
    let TypeDef = 2
    let Field = 4
    let MethodDef = 6
    let Param = 8
    let InterfaceImpl = 9
    let MemberRef = 10
    let Constant = 11
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

type private StreamHeader =
    { Offset: int; Size: int; Name: string }

let private tryRvaToOffset (bytes: byte[]) (coffHeader: int) (optionalHeader: int) (sizeOfOptionalHeader: int) (rva: int) =
    let numberOfSections = int (readUInt16 bytes (coffHeader + 2))
    let sectionHeadersStart = optionalHeader + sizeOfOptionalHeader

    let rec loop sectionIndex =
        if sectionIndex >= numberOfSections then
            None
        else
            let sectionOffset = sectionHeadersStart + sectionIndex * 40

            if sectionOffset + 40 > bytes.Length then
                None
            else
                let virtualSize = readInt32 bytes (sectionOffset + 8)
                let virtualAddress = readInt32 bytes (sectionOffset + 12)
                let rawSize = readInt32 bytes (sectionOffset + 16)
                let pointerToRawData = readInt32 bytes (sectionOffset + 20)
                let span = max virtualSize rawSize

                if rva >= virtualAddress && rva < virtualAddress + span then
                    Some(rva - virtualAddress + pointerToRawData)
                else
                    loop (sectionIndex + 1)

    loop 0

let private findMetadataRoot (bytes: byte[]) : int option =
    try
        if bytes.Length < 64 || bytes[0] <> 0x4Duy || bytes[1] <> 0x5Auy then
            None
        else
            let peOffset = readInt32 bytes 0x3C

            if peOffset < 0 || peOffset + 24 > bytes.Length then
                None
            elif
                bytes[peOffset] <> 0x50uy
                || bytes[peOffset + 1] <> 0x45uy
                || bytes[peOffset + 2] <> 0uy
                || bytes[peOffset + 3] <> 0uy
            then
                None
            else
                let coffHeader = peOffset + 4
                let sizeOfOptionalHeader = int (readUInt16 bytes (coffHeader + 16))
                let optionalHeader = coffHeader + 20
                let magic = readUInt16 bytes optionalHeader

                let dataDirectoryStart =
                    if magic = 0x20Bus then
                        optionalHeader + 112
                    else
                        optionalHeader + 96

                let cliDirectory = dataDirectoryStart + 14 * 8

                if cliDirectory + 8 > bytes.Length then
                    None
                else
                    let cliHeaderRva = readInt32 bytes cliDirectory

                    if cliHeaderRva = 0 then
                        None
                    else
                        match tryRvaToOffset bytes coffHeader optionalHeader sizeOfOptionalHeader cliHeaderRva with
                        | None -> None
                        | Some cliHeaderOffset when cliHeaderOffset + 12 > bytes.Length -> None
                        | Some cliHeaderOffset ->
                            let metadataRva = readInt32 bytes (cliHeaderOffset + 8)
                            tryRvaToOffset bytes coffHeader optionalHeader sizeOfOptionalHeader metadataRva
    with
    | :? IndexOutOfRangeException
    | :? ArgumentOutOfRangeException -> None

let private parseStreamHeaders (bytes: byte[]) (metadataRoot: int) : StreamHeader list =
    let signature = readInt32 bytes metadataRoot

    if signature <> 0x424A5342 then
        []
    else
        let versionLength = readInt32 bytes (metadataRoot + 12)
        let paddedVersionLength = (versionLength + 3) &&& ~~~3
        let streamsOffset = metadataRoot + 16 + paddedVersionLength
        let numberOfStreams = int (readUInt16 bytes (streamsOffset + 2))
        let mutable currentOffset = streamsOffset + 4
        let headers = ResizeArray<StreamHeader>()

        for _ in 1..numberOfStreams do
            let offset = readInt32 bytes currentOffset
            let size = readInt32 bytes (currentOffset + 4)
            let mutable nameEnd = currentOffset + 8

            while nameEnd < bytes.Length && bytes[nameEnd] <> 0uy do
                nameEnd <- nameEnd + 1

            if nameEnd >= bytes.Length then
                invalidArg (nameof bytes) "invalid metadata stream header"

            let name =
                Encoding.ASCII.GetString(bytes, currentOffset + 8, nameEnd - currentOffset - 8)

            let paddedNameLength = ((nameEnd - currentOffset - 8 + 1) + 3) &&& ~~~3

            headers.Add(
                {
                    Offset = metadataRoot + offset
                    Size = size
                    Name = name
                }
            )

            currentOffset <- currentOffset + 8 + paddedNameLength

        headers |> Seq.toList

let private findStream (headers: StreamHeader list) (name: string) =
    headers |> List.tryFind (fun header -> header.Name = name)

let private parseTablesStream (bytes: byte[]) (tablesStream: StreamHeader) =
    let offset = tablesStream.Offset
    let heapSizes = bytes[offset + 6]
    let valid = readInt64 bytes (offset + 8)
    let rowCounts = Array.zeroCreate tableCount
    let mutable rowCountOffset = offset + 24

    for i in 0..63 do
        if (valid &&& (1L <<< i)) <> 0L then
            rowCounts[i] <- readInt32 bytes rowCountOffset
            rowCountOffset <- rowCountOffset + 4

    heapSizes, rowCounts, offset

let metadataSnapshotFromBytes (bytes: byte[]) : MetadataSnapshot option =
    try
        match findMetadataRoot bytes with
        | None -> None
        | Some metadataRoot ->
            let streamHeaders = parseStreamHeaders bytes metadataRoot
            let stringsStream = findStream streamHeaders "#Strings"
            let userStringsStream = findStream streamHeaders "#US"
            let blobStream = findStream streamHeaders "#Blob"
            let guidStream = findStream streamHeaders "#GUID"

            let tablesStream =
                findStream streamHeaders "#~" |> Option.orElse (findStream streamHeaders "#-")

            match tablesStream with
            | None -> None
            | Some tables ->
                let _, rowCounts, _ = parseTablesStream bytes tables

                let trimmedStringHeapSize =
                    match stringsStream with
                    | None -> 0
                    | Some stream ->
                        if stream.Size = 0 then
                            0
                        else
                            let last = stream.Offset + stream.Size - 1
                            let mutable i = last

                            while i >= stream.Offset && bytes[i] = 0uy do
                                i <- i - 1

                            if i = last then stream.Size else i - stream.Offset + 2

                let heapSizes =
                    {
                        StringHeapSize = trimmedStringHeapSize
                        UserStringHeapSize =
                            userStringsStream
                            |> Option.map (fun stream -> stream.Size)
                            |> Option.defaultValue 0
                        BlobHeapSize = blobStream |> Option.map (fun stream -> stream.Size) |> Option.defaultValue 0
                        GuidHeapSize = guidStream |> Option.map (fun stream -> stream.Size) |> Option.defaultValue 0
                    }

                Some
                    {
                        HeapSizes = heapSizes
                        TableRowCounts = rowCounts
                        GuidHeapStart = heapSizes.GuidHeapSize
                    }
    with
    | :? IndexOutOfRangeException
    | :? ArgumentOutOfRangeException -> None

let private readGuidFromBytes (bytes: byte[]) (guidIndex: int) =
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
                let offset = guidStream.Offset + (guidIndex - 1) * 16

                if offset + 16 > bytes.Length then
                    None
                else
                    Some(Guid(bytes[offset .. offset + 15]))

/// Parsed metadata context for reading table rows.
/// Internal (not private): tiny reader members can get cross-module inlined in Release
/// builds, and inlined code referencing a module-private type fails CLR visibility
/// checks at runtime.
type internal MetadataContext =
    {
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

let private tableIndexSize (rowCounts: int[]) tableIndex =
    if rowCounts[tableIndex] <= 65535 then 2 else 4

let private codedIndexSize (rowCounts: int[]) (tableIndices: int[]) tagBits =
    let maxRows =
        tableIndices
        |> Array.map (fun tableIndex -> if tableIndex < tableCount then rowCounts[tableIndex] else 0)
        |> Array.max

    let maxValue = (maxRows <<< tagBits) ||| ((1 <<< tagBits) - 1)
    if maxValue <= 65535 then 2 else 4

let private resolutionScopeSize rowCounts =
    codedIndexSize
        rowCounts
        [|
            TableIndices.Module
            TableIndices.ModuleRef
            TableIndices.AssemblyRef
            TableIndices.TypeRef
        |]
        2

let private typeDefOrRefSize rowCounts =
    codedIndexSize rowCounts [| TableIndices.TypeDef; TableIndices.TypeRef; TableIndices.TypeSpec |] 2

let private hasConstantSize rowCounts =
    codedIndexSize rowCounts [| TableIndices.Field; TableIndices.Param; TableIndices.Property |] 2

let private hasCustomAttributeSize rowCounts =
    codedIndexSize
        rowCounts
        [|
            TableIndices.MethodDef
            TableIndices.Field
            TableIndices.TypeRef
            TableIndices.TypeDef
            TableIndices.Param
            TableIndices.InterfaceImpl
            TableIndices.MemberRef
            TableIndices.Module
            TableIndices.DeclSecurity
            TableIndices.Property
            TableIndices.Event
            TableIndices.StandAloneSig
            TableIndices.ModuleRef
            TableIndices.TypeSpec
            TableIndices.Assembly
            TableIndices.AssemblyRef
            TableIndices.File
            TableIndices.ExportedType
            TableIndices.ManifestResource
            TableIndices.GenericParam
            TableIndices.GenericParamConstraint
            TableIndices.MethodSpec
        |]
        5

let private hasFieldMarshalSize rowCounts =
    codedIndexSize rowCounts [| TableIndices.Field; TableIndices.Param |] 1

let private hasDeclSecuritySize rowCounts =
    codedIndexSize rowCounts [| TableIndices.TypeDef; TableIndices.MethodDef; TableIndices.Assembly |] 2

let private memberRefParentSize rowCounts =
    codedIndexSize
        rowCounts
        [|
            TableIndices.TypeDef
            TableIndices.TypeRef
            TableIndices.ModuleRef
            TableIndices.MethodDef
            TableIndices.TypeSpec
        |]
        3

let private hasSemanticsSize rowCounts =
    codedIndexSize rowCounts [| TableIndices.Event; TableIndices.Property |] 1

let private methodDefOrRefSize rowCounts =
    codedIndexSize rowCounts [| TableIndices.MethodDef; TableIndices.MemberRef |] 1

let private memberForwardedSize rowCounts =
    codedIndexSize rowCounts [| TableIndices.Field; TableIndices.MethodDef |] 1

let private implementationSize rowCounts =
    codedIndexSize rowCounts [| TableIndices.File; TableIndices.AssemblyRef; TableIndices.ExportedType |] 2

let private customAttributeTypeSize rowCounts =
    codedIndexSize rowCounts [| 0; 0; TableIndices.MethodDef; TableIndices.MemberRef; 0 |] 3

let private typeOrMethodDefSize rowCounts =
    codedIndexSize rowCounts [| TableIndices.TypeDef; TableIndices.MethodDef |] 1

let private calculateTableRowSizes (ctx: MetadataContext) =
    let rowCounts = ctx.RowCounts
    let strIdx = ctx.StringIndexSize
    let guidIdx = ctx.GuidIndexSize
    let blobIdx = ctx.BlobIndexSize
    let sizes = Array.zeroCreate tableCount

    sizes[0] <- 2 + strIdx + guidIdx + guidIdx + guidIdx
    sizes[1] <- resolutionScopeSize rowCounts + strIdx + strIdx

    sizes[2] <-
        4
        + strIdx
        + strIdx
        + typeDefOrRefSize rowCounts
        + tableIndexSize rowCounts TableIndices.Field
        + tableIndexSize rowCounts TableIndices.MethodDef

    sizes[4] <- 2 + strIdx + blobIdx
    sizes[6] <- 4 + 2 + 2 + strIdx + blobIdx + tableIndexSize rowCounts TableIndices.Param
    sizes[8] <- 2 + 2 + strIdx
    sizes[9] <- tableIndexSize rowCounts TableIndices.TypeDef + typeDefOrRefSize rowCounts
    sizes[10] <- memberRefParentSize rowCounts + strIdx + blobIdx
    sizes[11] <- 2 + hasConstantSize rowCounts + blobIdx
    sizes[12] <- hasCustomAttributeSize rowCounts + customAttributeTypeSize rowCounts + blobIdx
    sizes[13] <- hasFieldMarshalSize rowCounts + blobIdx
    sizes[14] <- 2 + hasDeclSecuritySize rowCounts + blobIdx
    sizes[15] <- 2 + 4 + tableIndexSize rowCounts TableIndices.TypeDef
    sizes[16] <- 4 + tableIndexSize rowCounts TableIndices.Field
    sizes[17] <- blobIdx

    sizes[18] <-
        tableIndexSize rowCounts TableIndices.TypeDef
        + tableIndexSize rowCounts TableIndices.Event

    sizes[20] <- 2 + strIdx + typeDefOrRefSize rowCounts

    sizes[21] <-
        tableIndexSize rowCounts TableIndices.TypeDef
        + tableIndexSize rowCounts TableIndices.Property

    sizes[23] <- 2 + strIdx + blobIdx
    sizes[24] <- 2 + tableIndexSize rowCounts TableIndices.MethodDef + hasSemanticsSize rowCounts

    sizes[25] <-
        tableIndexSize rowCounts TableIndices.TypeDef
        + methodDefOrRefSize rowCounts
        + methodDefOrRefSize rowCounts

    sizes[26] <- strIdx
    sizes[27] <- blobIdx

    sizes[28] <-
        2
        + memberForwardedSize rowCounts
        + strIdx
        + tableIndexSize rowCounts TableIndices.ModuleRef

    sizes[29] <- 4 + tableIndexSize rowCounts TableIndices.Field
    sizes[32] <- 4 + 2 + 2 + 2 + 2 + 4 + blobIdx + strIdx + strIdx
    sizes[35] <- 2 + 2 + 2 + 2 + 4 + blobIdx + strIdx + strIdx + blobIdx
    sizes[38] <- 4 + strIdx + blobIdx
    sizes[39] <- 4 + 4 + strIdx + strIdx + implementationSize rowCounts
    sizes[40] <- 4 + 4 + strIdx + implementationSize rowCounts

    sizes[41] <-
        tableIndexSize rowCounts TableIndices.TypeDef
        + tableIndexSize rowCounts TableIndices.TypeDef

    sizes[42] <- 2 + 2 + typeOrMethodDefSize rowCounts + strIdx
    sizes[43] <- methodDefOrRefSize rowCounts + blobIdx
    sizes[44] <- tableIndexSize rowCounts TableIndices.GenericParam + typeDefOrRefSize rowCounts
    sizes

let private calculateTableOffsets (ctx: MetadataContext) (rowSizes: int[]) =
    let offsets = Array.zeroCreate tableCount
    let mutable currentOffset = ctx.TablesStart

    for i in 0 .. tableCount - 1 do
        offsets[i] <- currentOffset
        currentOffset <- currentOffset + rowSizes[i] * ctx.RowCounts[i]

    offsets

let private readHeapIndex (bytes: byte[]) offset indexSize =
    if indexSize = 2 then
        int (readUInt16 bytes offset)
    else
        readInt32 bytes offset

let private createMetadataContext (bytes: byte[]) =
    match findMetadataRoot bytes with
    | None -> None
    | Some metadataRoot ->
        let streamHeaders = parseStreamHeaders bytes metadataRoot

        let tablesStream =
            findStream streamHeaders "#~" |> Option.orElse (findStream streamHeaders "#-")

        match tablesStream with
        | None -> None
        | Some stream ->
            let heapSizes, rowCounts, tablesOffset = parseTablesStream bytes stream
            let stringsBig = (heapSizes &&& 0x01uy) <> 0uy
            let guidsBig = (heapSizes &&& 0x02uy) <> 0uy
            let blobsBig = (heapSizes &&& 0x04uy) <> 0uy
            let mutable rowCountSize = 0

            for i in 0..63 do
                if rowCounts[i] > 0 then
                    rowCountSize <- rowCountSize + 4

            Some
                {
                    Bytes = bytes
                    HeapSizes = heapSizes
                    RowCounts = rowCounts
                    TablesStart = tablesOffset + 24 + rowCountSize
                    StringIndexSize = if stringsBig then 4 else 2
                    GuidIndexSize = if guidsBig then 4 else 2
                    BlobIndexSize = if blobsBig then 4 else 2
                    StringsStreamOffset =
                        streamHeaders
                        |> List.tryFind (fun h -> h.Name = "#Strings")
                        |> Option.map (fun h -> h.Offset)
                        |> Option.defaultValue 0
                    BlobStreamOffset =
                        streamHeaders
                        |> List.tryFind (fun h -> h.Name = "#Blob")
                        |> Option.map (fun h -> h.Offset)
                        |> Option.defaultValue 0
                }

let private readStringFromHeap (ctx: MetadataContext) offset =
    if offset = 0 then
        ""
    else
        let start = ctx.StringsStreamOffset + offset
        let mutable endPos = start

        while endPos < ctx.Bytes.Length && ctx.Bytes[endPos] <> 0uy do
            endPos <- endPos + 1

        Encoding.UTF8.GetString(ctx.Bytes, start, endPos - start)

let private readBlobFromHeap (ctx: MetadataContext) offset =
    if offset <= 0 then
        Array.empty
    else
        let start = ctx.BlobStreamOffset + offset
        let b0 = int ctx.Bytes[start]

        let length, headerSize =
            if b0 &&& 0x80 = 0 then
                b0, 1
            elif b0 &&& 0xC0 = 0x80 then
                ((b0 &&& 0x3F) <<< 8) ||| int ctx.Bytes[start + 1], 2
            else
                (((b0 &&& 0x1F) <<< 24)
                 ||| (int ctx.Bytes[start + 1] <<< 16)
                 ||| (int ctx.Bytes[start + 2] <<< 8)
                 ||| int ctx.Bytes[start + 3]),
                4

        if length = 0 then
            Array.empty
        else
            ctx.Bytes[start + headerSize .. start + headerSize + length - 1]

type TypeDefRowData =
    {
        Flags: int
        NameOffset: int
        NamespaceOffset: int
        Extends: int
        FieldList: int
        MethodList: int
    }

type FieldRowData =
    {
        Flags: int
        NameOffset: int
        SignatureOffset: int
    }

type MethodDefRowData =
    {
        RVA: int
        ImplFlags: int
        Flags: int
        NameOffset: int
        SignatureOffset: int
        ParamList: int
    }

type PropertyMapRowData = { Parent: int; PropertyList: int }

type PropertyRowData =
    {
        Flags: int
        NameOffset: int
        SignatureOffset: int
    }

type EventMapRowData = { Parent: int; EventList: int }

type EventRowData =
    {
        Flags: int
        NameOffset: int
        EventType: int
    }

type ModuleRowData =
    {
        Generation: int
        NameOffset: int
        MvidIndex: int
        EncIdIndex: int
        EncBaseIdIndex: int
    }

let private rowOffset (ctx: MetadataContext) (rowSizes: int[]) (tableOffsets: int[]) tableIndex rowId =
    if rowId < 1 || rowId > ctx.RowCounts[tableIndex] then
        None
    else
        Some(tableOffsets[tableIndex] + (rowId - 1) * rowSizes[tableIndex])

let private readTypeDefRow ctx rowSizes tableOffsets rowId =
    rowOffset ctx rowSizes tableOffsets TableIndices.TypeDef rowId
    |> Option.map (fun offset ->
        let extendsOffset = offset + 4 + ctx.StringIndexSize + ctx.StringIndexSize

        {
            Flags = readInt32 ctx.Bytes offset
            NameOffset = readHeapIndex ctx.Bytes (offset + 4) ctx.StringIndexSize
            NamespaceOffset = readHeapIndex ctx.Bytes (offset + 4 + ctx.StringIndexSize) ctx.StringIndexSize
            Extends = readHeapIndex ctx.Bytes extendsOffset (typeDefOrRefSize ctx.RowCounts)
            FieldList =
                readHeapIndex ctx.Bytes (extendsOffset + typeDefOrRefSize ctx.RowCounts) (tableIndexSize ctx.RowCounts TableIndices.Field)
            MethodList =
                readHeapIndex
                    ctx.Bytes
                    (extendsOffset
                     + typeDefOrRefSize ctx.RowCounts
                     + tableIndexSize ctx.RowCounts TableIndices.Field)
                    (tableIndexSize ctx.RowCounts TableIndices.MethodDef)
        })

let private readFieldRow ctx rowSizes tableOffsets rowId =
    rowOffset ctx rowSizes tableOffsets TableIndices.Field rowId
    |> Option.map (fun offset ->
        {
            Flags = int (readUInt16 ctx.Bytes offset)
            NameOffset = readHeapIndex ctx.Bytes (offset + 2) ctx.StringIndexSize
            SignatureOffset = readHeapIndex ctx.Bytes (offset + 2 + ctx.StringIndexSize) ctx.BlobIndexSize
        })

let private readMethodDefRow ctx rowSizes tableOffsets rowId =
    rowOffset ctx rowSizes tableOffsets TableIndices.MethodDef rowId
    |> Option.map (fun offset ->
        {
            RVA = readInt32 ctx.Bytes offset
            ImplFlags = int (readUInt16 ctx.Bytes (offset + 4))
            Flags = int (readUInt16 ctx.Bytes (offset + 6))
            NameOffset = readHeapIndex ctx.Bytes (offset + 8) ctx.StringIndexSize
            SignatureOffset = readHeapIndex ctx.Bytes (offset + 8 + ctx.StringIndexSize) ctx.BlobIndexSize
            ParamList =
                readHeapIndex
                    ctx.Bytes
                    (offset + 8 + ctx.StringIndexSize + ctx.BlobIndexSize)
                    (tableIndexSize ctx.RowCounts TableIndices.Param)
        })

let private readPropertyMapRow ctx rowSizes tableOffsets rowId =
    rowOffset ctx rowSizes tableOffsets TableIndices.PropertyMap rowId
    |> Option.map (fun offset ->
        {
            Parent = readHeapIndex ctx.Bytes offset (tableIndexSize ctx.RowCounts TableIndices.TypeDef)
            PropertyList =
                readHeapIndex
                    ctx.Bytes
                    (offset + tableIndexSize ctx.RowCounts TableIndices.TypeDef)
                    (tableIndexSize ctx.RowCounts TableIndices.Property)
        })

let private readPropertyRow ctx rowSizes tableOffsets rowId =
    rowOffset ctx rowSizes tableOffsets TableIndices.Property rowId
    |> Option.map (fun offset ->
        {
            Flags = int (readUInt16 ctx.Bytes offset)
            NameOffset = readHeapIndex ctx.Bytes (offset + 2) ctx.StringIndexSize
            SignatureOffset = readHeapIndex ctx.Bytes (offset + 2 + ctx.StringIndexSize) ctx.BlobIndexSize
        })

let private readEventMapRow ctx rowSizes tableOffsets rowId =
    rowOffset ctx rowSizes tableOffsets TableIndices.EventMap rowId
    |> Option.map (fun offset ->
        {
            Parent = readHeapIndex ctx.Bytes offset (tableIndexSize ctx.RowCounts TableIndices.TypeDef)
            EventList =
                readHeapIndex
                    ctx.Bytes
                    (offset + tableIndexSize ctx.RowCounts TableIndices.TypeDef)
                    (tableIndexSize ctx.RowCounts TableIndices.Event)
        })

let private readEventRow ctx rowSizes tableOffsets rowId =
    rowOffset ctx rowSizes tableOffsets TableIndices.Event rowId
    |> Option.map (fun offset ->
        {
            Flags = int (readUInt16 ctx.Bytes offset)
            NameOffset = readHeapIndex ctx.Bytes (offset + 2) ctx.StringIndexSize
            EventType = readHeapIndex ctx.Bytes (offset + 2 + ctx.StringIndexSize) (typeDefOrRefSize ctx.RowCounts)
        })

let private readModuleRow (ctx: MetadataContext) (tableOffsets: int[]) =
    if ctx.RowCounts[TableIndices.Module] < 1 then
        None
    else
        let offset = tableOffsets[TableIndices.Module]

        Some
            {
                Generation = int (readUInt16 ctx.Bytes offset)
                NameOffset = readHeapIndex ctx.Bytes (offset + 2) ctx.StringIndexSize
                MvidIndex = readHeapIndex ctx.Bytes (offset + 2 + ctx.StringIndexSize) ctx.GuidIndexSize
                EncIdIndex = readHeapIndex ctx.Bytes (offset + 2 + ctx.StringIndexSize + ctx.GuidIndexSize) ctx.GuidIndexSize
                EncBaseIdIndex =
                    readHeapIndex ctx.Bytes (offset + 2 + ctx.StringIndexSize + ctx.GuidIndexSize + ctx.GuidIndexSize) ctx.GuidIndexSize
            }

type BaselineMetadataReader private (ctx: MetadataContext, rowSizes: int[], tableOffsets: int[]) =

    static member Create(bytes: byte[]) =
        try
            match createMetadataContext bytes with
            | None -> None
            | Some ctx ->
                let rowSizes = calculateTableRowSizes ctx
                let tableOffsets = calculateTableOffsets ctx rowSizes
                Some(BaselineMetadataReader(ctx, rowSizes, tableOffsets))
        with
        | :? IndexOutOfRangeException
        | :? ArgumentOutOfRangeException -> None

    member _.RowCounts = ctx.RowCounts

    member _.TypeDefCount = ctx.RowCounts[TableIndices.TypeDef]

    member _.FieldCount = ctx.RowCounts[TableIndices.Field]

    member _.MethodDefCount = ctx.RowCounts[TableIndices.MethodDef]

    member _.PropertyMapCount = ctx.RowCounts[TableIndices.PropertyMap]

    member _.PropertyCount = ctx.RowCounts[TableIndices.Property]

    member _.EventMapCount = ctx.RowCounts[TableIndices.EventMap]

    member _.EventCount = ctx.RowCounts[TableIndices.Event]

    member _.GetModule() = readModuleRow ctx tableOffsets

    member _.GetTypeDef(rowId: int) =
        readTypeDefRow ctx rowSizes tableOffsets rowId

    member _.GetField(rowId: int) =
        readFieldRow ctx rowSizes tableOffsets rowId

    member _.GetMethodDef(rowId: int) =
        readMethodDefRow ctx rowSizes tableOffsets rowId

    member _.GetPropertyMap(rowId: int) =
        readPropertyMapRow ctx rowSizes tableOffsets rowId

    member _.GetProperty(rowId: int) =
        readPropertyRow ctx rowSizes tableOffsets rowId

    member _.GetEventMap(rowId: int) =
        readEventMapRow ctx rowSizes tableOffsets rowId

    member _.GetEvent(rowId: int) =
        readEventRow ctx rowSizes tableOffsets rowId

    member _.GetString(offset: int) = readStringFromHeap ctx offset

    member _.GetBlob(offset: int) = readBlobFromHeap ctx offset

    member this.GetTypeFieldRange(typeRowId: int) =
        match this.GetTypeDef typeRowId with
        | None -> None
        | Some typeDef ->
            let firstField = typeDef.FieldList

            let lastField =
                if typeRowId < ctx.RowCounts[TableIndices.TypeDef] then
                    match this.GetTypeDef(typeRowId + 1) with
                    | Some next -> next.FieldList - 1
                    | None -> ctx.RowCounts[TableIndices.Field]
                else
                    ctx.RowCounts[TableIndices.Field]

            if firstField <= 0 || firstField > lastField then
                None
            else
                Some(firstField, lastField)

    member this.GetTypeMethodRange(typeRowId: int) =
        match this.GetTypeDef typeRowId with
        | None -> None
        | Some typeDef ->
            let firstMethod = typeDef.MethodList

            let lastMethod =
                if typeRowId < ctx.RowCounts[TableIndices.TypeDef] then
                    match this.GetTypeDef(typeRowId + 1) with
                    | Some next -> next.MethodList - 1
                    | None -> ctx.RowCounts[TableIndices.MethodDef]
                else
                    ctx.RowCounts[TableIndices.MethodDef]

            if firstMethod <= 0 || firstMethod > lastMethod then
                None
            else
                Some(firstMethod, lastMethod)

    member this.GetPropertyMapRange(propertyMapRowId: int) =
        match this.GetPropertyMap propertyMapRowId with
        | None -> None
        | Some map ->
            let firstProperty = map.PropertyList

            let lastProperty =
                if propertyMapRowId < ctx.RowCounts[TableIndices.PropertyMap] then
                    match this.GetPropertyMap(propertyMapRowId + 1) with
                    | Some next -> next.PropertyList - 1
                    | None -> ctx.RowCounts[TableIndices.Property]
                else
                    ctx.RowCounts[TableIndices.Property]

            if firstProperty <= 0 || firstProperty > lastProperty then
                None
            else
                Some(map.Parent, firstProperty, lastProperty)

    member this.GetEventMapRange(eventMapRowId: int) =
        match this.GetEventMap eventMapRowId with
        | None -> None
        | Some map ->
            let firstEvent = map.EventList

            let lastEvent =
                if eventMapRowId < ctx.RowCounts[TableIndices.EventMap] then
                    match this.GetEventMap(eventMapRowId + 1) with
                    | Some next -> next.EventList - 1
                    | None -> ctx.RowCounts[TableIndices.Event]
                else
                    ctx.RowCounts[TableIndices.Event]

            if firstEvent <= 0 || firstEvent > lastEvent then
                None
            else
                Some(map.Parent, firstEvent, lastEvent)

let readModuleMvidFromBytes (bytes: byte[]) : Guid option =
    try
        match BaselineMetadataReader.Create bytes with
        | None -> None
        | Some reader -> reader.GetModule() |> Option.bind (fun m -> readGuidFromBytes bytes m.MvidIndex)
    with
    | :? IndexOutOfRangeException
    | :? ArgumentOutOfRangeException -> None

let private parsePdbStream (bytes: byte[]) (pdbStream: StreamHeader) =
    if pdbStream.Size < 24 then
        None
    else
        let entryPointToken = readInt32 bytes (pdbStream.Offset + 20)
        if entryPointToken = 0 then None else Some entryPointToken

let private parsePdbTablesStream (bytes: byte[]) (tablesStream: StreamHeader) =
    let offset = tablesStream.Offset
    let valid = readInt64 bytes (offset + 8)
    let pdbRowCounts = Array.zeroCreate 8
    let mutable rowCountOffset = offset + 24

    for i in 0..63 do
        if (valid &&& (1L <<< i)) <> 0L then
            let count = readInt32 bytes rowCountOffset

            if i >= 0x30 && i <= 0x37 then
                pdbRowCounts[i - 0x30] <- count

            rowCountOffset <- rowCountOffset + 4

    pdbRowCounts

let readPortablePdbMetadata (pdbBytes: byte[]) =
    if pdbBytes.Length < 4 then
        None
    else
        try
            if readInt32 pdbBytes 0 <> 0x424A5342 then
                None
            else
                let streamHeaders = parseStreamHeaders pdbBytes 0

                let tablesStream =
                    findStream streamHeaders "#~" |> Option.orElse (findStream streamHeaders "#-")

                let pdbStream = findStream streamHeaders "#Pdb"

                tablesStream
                |> Option.map (fun stream ->
                    {
                        TableRowCounts = parsePdbTablesStream pdbBytes stream
                        EntryPointToken = pdbStream |> Option.bind (parsePdbStream pdbBytes)
                    })
        with
        | :? IndexOutOfRangeException
        | :? ArgumentOutOfRangeException -> None
