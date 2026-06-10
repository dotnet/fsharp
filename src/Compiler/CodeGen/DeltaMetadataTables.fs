module internal FSharp.Compiler.CodeGen.DeltaMetadataTables

open System
open System.Collections.Generic
open System.IO
open System.Text
open Microsoft.FSharp.Collections
open FSharp.Compiler.AbstractIL.ILBinaryWriter
open FSharp.Compiler.AbstractIL.BinaryConstants
open FSharp.Compiler.AbstractIL.ILDeltaHandles
open FSharp.Compiler.AbstractIL.ILMetadataHeaps
open FSharp.Compiler.AbstractIL.ILEncLogWriter
open FSharp.Compiler.HotReloadBaseline
open FSharp.Compiler.IlxDeltaStreams
open FSharp.Compiler.CodeGen.DeltaMetadataTypes

module Encoding = FSharp.Compiler.CodeGen.DeltaMetadataEncoding

let private traceHeapOffsets =
    lazy (
        match Environment.GetEnvironmentVariable("FSHARP_HOTRELOAD_TRACE_HEAP_OFFSETS") with
        | null | "" -> false
        | value -> value = "1" || String.Equals(value, "true", StringComparison.OrdinalIgnoreCase)
    )

/// Mirrors the AbstractIL metadata tables for the subset of rows emitted by
/// hot reload deltas. The tables are populated alongside the SRM metadata
/// builder so we can eventually serialize deltas directly via AbstractIL.
type MetadataHeapOffsets =
    {
        StringHeapStart: int
        BlobHeapStart: int
        GuidHeapStart: int
        UserStringHeapStart: int
    }

    static member Zero =
        { StringHeapStart = 0
          BlobHeapStart = 0
          GuidHeapStart = 0
          UserStringHeapStart = 0 }

    static member OfHeapSizes(heapSizes: MetadataHeapSizes) =
        { StringHeapStart = heapSizes.StringHeapSize
          BlobHeapStart = heapSizes.BlobHeapSize
          GuidHeapStart = heapSizes.GuidHeapSize
          UserStringHeapStart = heapSizes.UserStringHeapSize }

let private byteArrayComparer : IEqualityComparer<byte[]> =
    { new IEqualityComparer<byte[]> with
        member _.Equals(x, y) =
            if obj.ReferenceEquals(x, y) then true
            elif isNull (box x) || isNull (box y) then false
            elif x.Length <> y.Length then false
            else
                let mutable idx = 0
                let mutable equal = true
                while equal && idx < x.Length do
                    if x[idx] <> y[idx] then
                        equal <- false
                    idx <- idx + 1
                equal

        member _.GetHashCode(array: byte[]) =
            if isNull (box array) then 0
            else
                let mutable hash = 17
                for value in array do
                    hash <- (hash * 23) + int value
                hash }

let private writeCompressedUnsigned (writer: BinaryWriter) (value: int) =
    if value <= 0x7F then
        writer.Write(byte value)
    elif value <= 0x3FFF then
        let b1 = byte ((value >>> 8) ||| 0x80)
        let b0 = byte (value &&& 0xFF)
        writer.Write(b1)
        writer.Write(b0)
    elif value <= 0x1FFFFFFF then
        let b2 = byte ((value >>> 24) ||| 0xC0)
        let b1 = byte ((value >>> 16) &&& 0xFF)
        let b0 = byte ((value >>> 8) &&& 0xFF)
        let bLowest = byte (value &&& 0xFF)
        writer.Write(b2)
        writer.Write(b1)
        writer.Write(b0)
        writer.Write(bLowest)
    else
        invalidArg (nameof value) "Compressed integer is too large for CLI metadata."

type private RowTableBuilder() =
    let rows = ResizeArray<RowElementData[]>()

    member _.Add(elements: RowElementData[]) = rows.Add elements
    member _.Entries = rows.ToArray()
    member _.Count = rows.Count

type private StringHeapBuilder() =
    let entries = ResizeArray<string>()
    let lookup = Dictionary<string, int>(StringComparer.Ordinal)
    let utf8 = Encoding.UTF8
    let mutable bytesCache: byte[] option = None
    let mutable offsetsCache: int[] option = None

    member _.AddSharedEntry(value: string) : int =
        if String.IsNullOrEmpty value then
            0
        else
            match lookup.TryGetValue value with
            | true, index -> index
            | _ ->
                let index = entries.Count + 1
                entries.Add value
                lookup[value] <- index
                bytesCache <- None
                offsetsCache <- None
                index

    member private this.BuildIfNeeded() =
        match bytesCache, offsetsCache with
        | Some _, Some _ -> ()
        | _ ->
            use ms = new MemoryStream()
            use writer = new BinaryWriter(ms, utf8, leaveOpen = true)
            let entryOffsets = Array.zeroCreate (entries.Count + 1)
            writer.Write(byte 0)
            let mutable currentOffset = int ms.Length
            for i = 0 to entries.Count - 1 do
                let entryIndex = i + 1
                entryOffsets.[entryIndex] <- currentOffset
                let bytes = utf8.GetBytes entries.[i]
                writer.Write(bytes)
                writer.Write(byte 0)
                currentOffset <- currentOffset + bytes.Length + 1
            writer.Flush()
            bytesCache <- Some(ms.ToArray())
            offsetsCache <- Some entryOffsets

    member this.Bytes
        with get () =
            this.BuildIfNeeded()
            bytesCache.Value

    member this.EntryOffsets
        with get () =
            this.BuildIfNeeded()
            offsetsCache.Value

type private ByteArrayHeapBuilder() =
    let entries = ResizeArray<byte[]>()
    let lookup = Dictionary<byte[], int>(byteArrayComparer)
    let mutable bytesCache: byte[] option = None
    let mutable offsetsCache: int[] option = None

    let encodeCompressedUnsigned value =
        use ms = new MemoryStream()
        use writer = new BinaryWriter(ms, Encoding.UTF8, leaveOpen = true)
        writeCompressedUnsigned writer value
        writer.Flush()
        ms.ToArray()

    member _.AddSharedEntry(value: byte[]) : int =
        if isNull (box value) || value.Length = 0 then
            0
        else
            match lookup.TryGetValue value with
            | true, index -> index
            | _ ->
                let index = entries.Count + 1
                entries.Add value
                lookup[value] <- index
                bytesCache <- None
                offsetsCache <- None
                index

    member private this.BuildIfNeeded() =
        match bytesCache, offsetsCache with
        | Some _, Some _ -> ()
        | _ ->
            use ms = new MemoryStream()
            use writer = new BinaryWriter(ms, Encoding.UTF8, leaveOpen = true)
            let entryOffsets = Array.zeroCreate (entries.Count + 1)
            writer.Write(byte 0)
            let mutable currentOffset = int ms.Length
            for i = 0 to entries.Count - 1 do
                let entryIndex = i + 1
                entryOffsets.[entryIndex] <- currentOffset
                let value = entries.[i]
                writeCompressedUnsigned writer value.Length
                if value.Length > 0 then
                    writer.Write(value)
                currentOffset <- int ms.Length
            writer.Flush()
            bytesCache <- Some(ms.ToArray())
            offsetsCache <- Some entryOffsets

    member this.Bytes
        with get () =
            this.BuildIfNeeded()
            bytesCache.Value

    member this.EntryOffsets
        with get () =
            this.BuildIfNeeded()
            offsetsCache.Value

    member _.Entries = entries |> Seq.toArray

type private UserStringHeapBuilder() =
    let entries = HashSet<int>()
    let mutable buffer : byte[] option = None
    let mutable maxLength = 1
    let mutable bytesCache : byte[] option = None

    /// Encodes a user string per ECMA-335 II.24.2.4:
    /// - Compressed unsigned length prefix (byte count including trailing flag)
    /// - UTF-16LE encoded string bytes
    /// - Trailing flag byte (computed via markerForUnicodeBytes from ILBinaryWriter)
    let encodeUserString (value: string) =
        let utf16Bytes = Text.Encoding.Unicode.GetBytes(value)
        let byteCount = utf16Bytes.Length + 1 // +1 for trailing flag

        // Use existing markerForUnicodeBytes from ilwrite.fs for trailing flag
        let trailingFlag = byte (markerForUnicodeBytes utf16Bytes)

        // Encode compressed length prefix
        use ms = new MemoryStream()
        use writer = new BinaryWriter(ms, Text.Encoding.UTF8, leaveOpen = true)
        writeCompressedUnsigned writer byteCount
        writer.Write(utf16Bytes)
        writer.Write(trailingFlag)
        writer.Flush()
        ms.ToArray()

    let ensureBuffer lengthNeeded =
        let requiredLength = max lengthNeeded 1
        match buffer with
        | Some existing when existing.Length >= requiredLength -> existing
        | Some existing ->
            let resized = Array.zeroCreate<byte> requiredLength
            Buffer.BlockCopy(existing, 0, resized, 0, existing.Length)
            buffer <- Some resized
            resized
        | None ->
            let initial = Array.zeroCreate<byte> requiredLength
            initial[0] <- 0uy
            buffer <- Some initial
            initial

    member _.AddEntry(offset: int, value: string) =
        // Use < 0 instead of <= 0 because offset 0 is valid for delta heaps
        // (the null byte at offset 0 is only in the baseline heap, not the delta)
        if offset < 0 then
            ()
        elif entries.Add offset then
            let bytes = encodeUserString value
            let neededLength = offset + bytes.Length
            let storage = ensureBuffer neededLength
            Buffer.BlockCopy(bytes, 0, storage, offset, bytes.Length)
            maxLength <- max maxLength neededLength
            bytesCache <- None

    member _.NextOffset = maxLength

    member this.Bytes
        with get () =
            match buffer with
            | Some data ->
                match bytesCache with
                | Some cached -> cached
                | None ->
                    let length = max maxLength 1
                    let trimmed =
                        if data.Length = length then
                            data
                        else
                            let slice = Array.zeroCreate<byte> length
                            Buffer.BlockCopy(data, 0, slice, 0, min data.Length length)
                            slice
                    bytesCache <- Some trimmed
                    trimmed
            | None ->
                let minimal = Array.zeroCreate<byte> 1
                minimal[0] <- 0uy
                minimal

type DeltaMetadataTables(?heapOffsets: MetadataHeapOffsets) =
    let heapOffsets = defaultArg heapOffsets MetadataHeapOffsets.Zero
    let strings = StringHeapBuilder()
    let blobs = ByteArrayHeapBuilder()
    let guids = ByteArrayHeapBuilder()
    let userStrings = UserStringHeapBuilder()
    let userStringLookup = Dictionary<string, int>(StringComparer.Ordinal)
    let mutable stringHeapBytesCache: byte[] option = None
    let mutable blobHeapBytesCache: byte[] option = None
    let mutable guidHeapBytesCache: byte[] option = None
    let mutable userStringHeapBytesCache: byte[] option = None

    let moduleRows = RowTableBuilder()
    let methodRows = RowTableBuilder()
    let paramRows = RowTableBuilder()
    let typeRefRows = RowTableBuilder()
    let memberRefRows = RowTableBuilder()
    let methodSpecRows = RowTableBuilder()
    let assemblyRefRows = RowTableBuilder()
    let standAloneSigRows = RowTableBuilder()
    let customAttributeRows = RowTableBuilder()
    let propertyRows = RowTableBuilder()
    let eventRows = RowTableBuilder()
    let propertyMapRows = RowTableBuilder()
    let eventMapRows = RowTableBuilder()
    let methodSemanticsRows = RowTableBuilder()
    let encLogRows = RowTableBuilder()
    let encMapRows = RowTableBuilder()

    let rowElement tag value =
        { Tag = tag
          Value = value
          IsAbsolute = false }

    let rowElementAbsolute tag value =
        { Tag = tag
          Value = value
          IsAbsolute = true }

    let rowElementUShort (value: uint16) = rowElement Encoding.RowElementTags.UShort (int value)
    let rowElementULong (value: int) = rowElement Encoding.RowElementTags.ULong value
    let rowElementString value = rowElement Encoding.RowElementTags.String value
    let rowElementBlob value = rowElement Encoding.RowElementTags.Blob value
    let rowElementStringAbsolute value = rowElementAbsolute Encoding.RowElementTags.String value
    let rowElementBlobAbsolute value = rowElementAbsolute Encoding.RowElementTags.Blob value
    let rowElementGuid value = rowElement Encoding.RowElementTags.Guid value
    let rowElementGuidAbsolute value = rowElementAbsolute Encoding.RowElementTags.Guid value
    let rowElementSimpleIndex table value = rowElement (Encoding.RowElementTags.SimpleIndex table) value
    let rowElementTypeDefOrRef tag value = rowElement (Encoding.RowElementTags.TypeDefOrRefOrSpec tag) value
    let rowElementHasSemantics tag value = rowElement (Encoding.RowElementTags.HasSemantics tag) value
    let rowElementMethodDefOrRef (methodRef: MethodDefOrRef) =
        rowElement (Encoding.RowElementTags.MethodDefOrRef (mkMethodDefOrRefTag methodRef.CodedTag)) methodRef.RowId

    let rowElementResolutionScope (scope: ResolutionScope) =
        rowElement (Encoding.RowElementTags.ResolutionScopeMin + scope.CodedTag) scope.RowId

    let rowElementMemberRefParent (parent: MemberRefParent) =
        rowElement (Encoding.RowElementTags.MemberRefParentMin + parent.CodedTag) parent.RowId

    /// HasCustomAttribute coded index per ECMA-335 II.24.2.6.
    /// Uses the HasCustomAttribute DU from ILDeltaHandles.
    let rowElementHasCustomAttribute (parent: HasCustomAttribute) =
        rowElement (Encoding.RowElementTags.HasCustomAttributeMin + parent.CodedTag) parent.RowId

    /// CustomAttributeType coded index per ECMA-335 II.24.2.6.
    /// Uses the CustomAttributeType DU from ILDeltaHandles.
    let rowElementCustomAttributeType (ctor: CustomAttributeType) =
        let tag = mkILCustomAttributeTypeTag ctor.CodedTag
        rowElement (Encoding.RowElementTags.CustomAttributeType tag) ctor.RowId

    let addStringValue (value: string) = if String.IsNullOrEmpty value then 0 else strings.AddSharedEntry value

    let addUserStringValue (value: string) =
        if String.IsNullOrEmpty value then
            0
        else
            match userStringLookup.TryGetValue value with
            | true, offset -> offset
            | _ ->
                // #US tokens store offsets, so allocate a new literal at the next free delta-local offset
                // and translate it back to the absolute heap offset expected by IL operands.
                let relativeOffset = userStrings.NextOffset
                let absoluteOffset = heapOffsets.UserStringHeapStart + relativeOffset
                userStrings.AddEntry(relativeOffset, value)
                userStringLookup[value] <- absoluteOffset
                userStringHeapBytesCache <- None
                absoluteOffset

    let addExistingStringOffset (offsetOpt: StringOffset option) (value: string) : int * bool =
        match offsetOpt with
        | Some (StringOffset offset) -> offset, true
        | None ->
            let idx = addStringValue value
            idx, false

    let addExistingStringOffsetOption (offsetOpt: StringOffset option) (valueOpt: string option) : int * bool =
        match offsetOpt with
        | Some (StringOffset offset) -> offset, true
        | None ->
            match valueOpt with
            | Some v when not (String.IsNullOrEmpty v) -> strings.AddSharedEntry v, false
            | _ -> 0, false

    let addStringOption (value: string option) : int * bool =
        match value with
        | Some v when not (String.IsNullOrEmpty v) ->
            let idx = strings.AddSharedEntry v
            idx, false
        | _ -> 0, false

    let addBlobBytes (bytes: byte[]) = if obj.ReferenceEquals(bytes, null) || bytes.Length = 0 then 0 else blobs.AddSharedEntry bytes

    let addExistingBlobOffset (offsetOpt: BlobOffset option) (value: byte[]) : int * bool =
        match offsetOpt with
        | Some (BlobOffset offset) -> offset, true
        | None ->
            let idx = addBlobBytes value
            idx, false

    let addGuidValue (value: Guid) =
        if value = System.Guid.Empty then
            0
        else
            let idx = guids.AddSharedEntry(value.ToByteArray())
            idx

    /// Force-add a GUID to the heap, even if it's the nil GUID.
    /// Returns the 1-based index in the delta's GUID heap.
    let forceAddGuidValue (value: Guid) =
        guids.AddSharedEntry(value.ToByteArray())

    let stringElement (token, isAbsolute) = if isAbsolute then rowElementStringAbsolute token else rowElementString token
    let blobElement (token, isAbsolute) = if isAbsolute then rowElementBlobAbsolute token else rowElementBlob token

    let encodeTypeDefOrRef (typeRef: TypeDefOrRef) =
        match typeRef with
        | TDR_TypeDef(TypeDefHandle rowId) -> tdor_TypeDef, rowId
        | TDR_TypeRef(TypeRefHandle rowId) -> tdor_TypeRef, rowId
        | TDR_TypeSpec(TypeSpecHandle rowId) -> tdor_TypeSpec, rowId

    let buildStringHeapBytes () = strings.Bytes

    let buildBlobHeapBytes () = blobs.Bytes

    let buildGuidHeapBytes () =
        use ms = new MemoryStream()
        use writer = new BinaryWriter(ms, Encoding.UTF8, leaveOpen = true)
        // Guid heap is a packed list of 16-byte entries; no sentinel is emitted.
        for entry in guids.Entries do
            if entry.Length = 16 then
                writer.Write(entry)
            else
                invalidArg "entry" "GUID entries must be 16 bytes."
        if Environment.GetEnvironmentVariable("FSHARP_HOTRELOAD_TRACE_METADATA") = "1" then
            let dumpGuid (bytes: byte[]) =
                if bytes.Length >= 16 then
                    BitConverter.ToString(bytes, 0, 16)
                else
                    "<invalid>"
            printfn "[delta-guid-heap] entries=%d" guids.Entries.Length
            guids.Entries
            |> Seq.mapi (fun idx b -> idx + 1, dumpGuid b)
            |> Seq.iter (fun (idx, g) -> printfn "[delta-guid-heap] idx=%d guidBytes=%s" idx g)
        writer.Flush()
        ms.ToArray()
    let buildUserStringHeapBytes () = userStrings.Bytes

    member _.AddModuleRow(name: string, nameOffsetOpt: StringOffset option, generation: int, moduleId: Guid, encId: Guid, encBaseId: Guid) =
        if moduleRows.Count = 0 then
            let nameToken =
                match nameOffsetOpt with
                | Some (StringOffset offset) -> offset, true
                | None -> addStringValue name, false
            // For EnC deltas:
            // - Delta GUID heap contains: nil at 1, MVID at 2, EncId at 3
            // - Module row stores raw delta-local indices using rowElementGuidAbsolute
            // - The runtime interprets these as-is (no baseline offset adjustment needed)
            // Force-add GUIDs in order to get predictable indices:
            let _nilGuidIndex = forceAddGuidValue System.Guid.Empty  // Index 1 (nil placeholder)
            let mvidIndex = forceAddGuidValue moduleId               // Index 2
            let encIdIndex = forceAddGuidValue encId                 // Index 3
            // EncBaseId is 0 (nil) for generation 1, otherwise reference previous EncId
            let encBaseIdIndex =
                if encBaseId = System.Guid.Empty then 0
                else forceAddGuidValue encBaseId                     // Index 4 if not nil
            if traceHeapOffsets.Value then
                printfn "[fsharp-hotreload][module-row-write] generation=%d mvidIndex=%d encIdIndex=%d encBaseIdIndex=%d"
                    generation mvidIndex encIdIndex encBaseIdIndex
            moduleRows.Add
                [|
                    rowElementUShort (uint16 generation)
                    stringElement nameToken
                    rowElementGuidAbsolute mvidIndex      // MVID - delta-local absolute index
                    rowElementGuidAbsolute encIdIndex     // EncId - delta-local absolute index
                    rowElementGuidAbsolute encBaseIdIndex // EncBaseId - 0 or delta-local index
                |]

    member _.AddMethodRow(row: MethodDefinitionRowInfo, body: MethodBodyUpdate) =
        let nameToken = addExistingStringOffset row.NameOffset row.Name

        let signatureToken = addExistingBlobOffset row.SignatureOffset row.Signature

        let codeRva =
            if body.CodeLength > 0 then
                body.CodeOffset
            else
                match row.CodeRva with
                | Some rva -> rva
                | None -> 0

        let rowElements =
            [|
                rowElementULong codeRva
                rowElementUShort (uint16 row.ImplAttributes)
                rowElementUShort (uint16 row.Attributes)
                stringElement nameToken
                blobElement signatureToken
                rowElementSimpleIndex TableNames.Param (row.FirstParameterRowId |> Option.defaultValue 0)
            |]
        methodRows.Add rowElements

    member _.AddParameterRow(row: ParameterDefinitionRowInfo) =
        // Validate parameter row per ECMA-335 II.22.33
        if row.RowId <= 0 then
            invalidArg "row" $"Parameter RowId must be > 0, got {row.RowId}"
        if row.SequenceNumber < 0 then
            invalidArg "row" $"Parameter SequenceNumber must be >= 0, got {row.SequenceNumber}"
        let nameToken = addExistingStringOffsetOption row.NameOffset row.Name
        let rowElements =
            [|
                rowElementUShort (uint16 row.Attributes)
                rowElementUShort (uint16 row.SequenceNumber)
                stringElement nameToken
            |]
        paramRows.Add rowElements

    member _.AddTypeReferenceRow(row: TypeReferenceRowInfo) =
        let nameToken = addExistingStringOffset row.NameOffset row.Name
        let namespaceToken = addExistingStringOffset row.NamespaceOffset row.Namespace
        let rowElements =
            [|
                rowElementResolutionScope row.ResolutionScope
                stringElement nameToken
                stringElement namespaceToken
            |]
        typeRefRows.Add rowElements

    member _.AddMemberReferenceRow(row: MemberReferenceRowInfo) =
        let nameToken = addExistingStringOffset row.NameOffset row.Name
        let signatureToken = addExistingBlobOffset row.SignatureOffset row.Signature
        let rowElements =
            [|
                rowElementMemberRefParent row.Parent
                stringElement nameToken
                blobElement signatureToken
            |]
        memberRefRows.Add rowElements

    member _.AddMethodSpecificationRow(row: MethodSpecificationRowInfo) =
        let signatureToken = addExistingBlobOffset row.SignatureOffset row.Signature
        let rowElements =
            [|
                rowElementMethodDefOrRef row.Method
                blobElement signatureToken
            |]
        methodSpecRows.Add rowElements

    member _.AddAssemblyReferenceRow(row: AssemblyReferenceRowInfo) =
        let publicKeyToken = addExistingBlobOffset row.PublicKeyOrTokenOffset row.PublicKeyOrToken
        let nameToken = addExistingStringOffset row.NameOffset row.Name
        let cultureToken = addExistingStringOffsetOption row.CultureOffset row.Culture
        let hashToken = addExistingBlobOffset row.HashValueOffset row.HashValue
        let versionComponent value =
            if value >= 0s then uint16 value else 0us
        let rowElements =
            [|
                rowElementUShort (versionComponent (int16 row.Version.Major))
                rowElementUShort (versionComponent (int16 row.Version.Minor))
                rowElementUShort (versionComponent (int16 row.Version.Build))
                rowElementUShort (versionComponent (int16 row.Version.Revision))
                rowElementULong (int row.Flags)
                blobElement publicKeyToken
                stringElement nameToken
                stringElement cultureToken
                blobElement hashToken
            |]
        assemblyRefRows.Add rowElements

    member _.AddStandaloneSignatureRow(signatureBytes: byte[]) =
        if not (isNull (box signatureBytes)) && signatureBytes.Length > 0 then
            let blobIndex = addBlobBytes signatureBytes
            let rowElements =
                [|
                    blobElement (blobIndex, false)
                |]
            standAloneSigRows.Add rowElements

    member _.AddCustomAttributeRow(row: CustomAttributeRowInfo) =
        let valueToken = addExistingBlobOffset row.ValueOffset row.Value

        let rowElements =
            [|
                rowElementHasCustomAttribute row.Parent
                rowElementCustomAttributeType row.Constructor
                blobElement valueToken
            |]

        customAttributeRows.Add rowElements

    member _.AddPropertyRow(row: PropertyDefinitionRowInfo) =
        let nameToken = addExistingStringOffset row.NameOffset row.Name

        let signatureToken = addExistingBlobOffset row.SignatureOffset row.Signature

        let rowElements =
            [|
                rowElementUShort (uint16 row.Attributes)
                stringElement nameToken
                blobElement signatureToken
            |]
        propertyRows.Add rowElements

    member _.AddEventRow(row: EventDefinitionRowInfo) =
        let tdorTag, tdorRow = encodeTypeDefOrRef row.EventType
        let nameToken = addExistingStringOffset row.NameOffset row.Name
        let rowElements =
            [|
                rowElementUShort (uint16 row.Attributes)
                stringElement nameToken
                rowElementTypeDefOrRef tdorTag tdorRow
            |]
        eventRows.Add rowElements

    member _.AddPropertyMapRow(row: PropertyMapRowInfo) =
        let rowElements =
            [|
                rowElementSimpleIndex TableNames.TypeDef row.TypeDefRowId
                rowElementSimpleIndex TableNames.Property (row.FirstPropertyRowId |> Option.defaultValue 0)
            |]
        propertyMapRows.Add rowElements

    member _.AddEventMapRow(row: EventMapRowInfo) =
        let rowElements =
            [|
                rowElementSimpleIndex TableNames.TypeDef row.TypeDefRowId
                rowElementSimpleIndex TableNames.Event (row.FirstEventRowId |> Option.defaultValue 0)
            |]
        eventMapRows.Add rowElements

    member _.AddMethodSemanticsRow(row: MethodSemanticsMetadataUpdate) =
        let methodRowId = DeltaTokens.getRowNumber row.MethodToken
        let assocTag, assocRowId =
            match row.AssociationInfo with
            | MethodSemanticsAssociation.PropertyAssociation(_, propertyRowId) -> hs_Property, propertyRowId
            | MethodSemanticsAssociation.EventAssociation(_, eventRowId) -> hs_Event, eventRowId
        let rowElements =
            [|
                rowElementUShort (uint16 row.Attributes)
                rowElementSimpleIndex TableNames.Method methodRowId
                rowElementHasSemantics assocTag assocRowId
            |]
        methodSemanticsRows.Add rowElements

    /// Add an entry to the EncLog table.
    /// The EncLog records each modification made in this delta generation.
    /// Per ECMA-335 II.22.7, each entry contains a token and operation.
    member _.AddEncLogRow(table: TableName, rowId: int, operation: EditAndContinueOperation) =
        let token = DeltaTokens.makeToken table rowId
        let rowElements =
            [|
                rowElementULong token
                rowElementULong operation.Value
            |]
        encLogRows.Add rowElements

    /// Add an entry to the EncMap table.
    /// The EncMap provides a sorted list of all tokens present in this delta.
    /// Per ECMA-335 II.22.6, entries are sorted by table then row.
    member _.AddEncMapRow(table: TableName, rowId: int) =
        let token = DeltaTokens.makeToken table rowId
        let rowElements =
            [|
                rowElementULong token
            |]
        encMapRows.Add rowElements

    member _.StringHeapBytes
        with get () =
            match stringHeapBytesCache with
            | Some bytes -> bytes
            | None ->
                let bytes = buildStringHeapBytes ()
                stringHeapBytesCache <- Some bytes
                bytes

    member _.StringHeapOffsets = strings.EntryOffsets

    member _.BlobHeapBytes
        with get () =
            match blobHeapBytesCache with
            | Some bytes -> bytes
            | None ->
                let bytes = buildBlobHeapBytes ()
                blobHeapBytesCache <- Some bytes
                bytes

    member _.BlobHeapOffsets = blobs.EntryOffsets

    member _.GuidHeapBytes
        with get () =
            match guidHeapBytesCache with
            | Some bytes -> bytes
            | None ->
                let bytes = buildGuidHeapBytes ()
                guidHeapBytesCache <- Some bytes
                bytes

    member _.UserStringHeapBytes
        with get () =
            match userStringHeapBytesCache with
            | Some bytes -> bytes
            | None ->
                let bytes = buildUserStringHeapBytes ()
                userStringHeapBytesCache <- Some bytes
                bytes

    member this.StringHeapSize = this.StringHeapBytes.Length

    member this.BlobHeapSize = this.BlobHeapBytes.Length

    member this.GuidHeapSize = this.GuidHeapBytes.Length

    member this.HeapSizes : MetadataHeapSizes =
        { StringHeapSize = this.StringHeapSize
          UserStringHeapSize = this.UserStringHeapBytes.Length
          BlobHeapSize = this.BlobHeapSize
          GuidHeapSize = this.GuidHeapSize }

    member _.TableRows : TableRows =
        { Module = moduleRows.Entries
          MethodDef = methodRows.Entries
          Param = paramRows.Entries
          TypeRef = typeRefRows.Entries
          MemberRef = memberRefRows.Entries
          MethodSpec = methodSpecRows.Entries
          AssemblyRef = assemblyRefRows.Entries
          StandAloneSig = standAloneSigRows.Entries
          CustomAttribute = customAttributeRows.Entries
          Property = propertyRows.Entries
          Event = eventRows.Entries
          PropertyMap = propertyMapRows.Entries
          EventMap = eventMapRows.Entries
          MethodSemantics = methodSemanticsRows.Entries
          EncLog = encLogRows.Entries
          EncMap = encMapRows.Entries }

    member _.HeapOffsets = heapOffsets

    /// Returns an array of row counts indexed by table number.
    /// Uses TableNames from BinaryConstants for ECMA-335 table indices.
    member _.TableRowCounts : int[] =
        let counts = Array.zeroCreate DeltaTokens.TableCount
        counts[TableNames.Module.Index] <- moduleRows.Count
        counts[TableNames.Method.Index] <- methodRows.Count
        counts[TableNames.Param.Index] <- paramRows.Count
        counts[TableNames.TypeRef.Index] <- typeRefRows.Count
        counts[TableNames.MemberRef.Index] <- memberRefRows.Count
        counts[TableNames.MethodSpec.Index] <- methodSpecRows.Count
        counts[TableNames.AssemblyRef.Index] <- assemblyRefRows.Count
        counts[TableNames.StandAloneSig.Index] <- standAloneSigRows.Count
        counts[TableNames.CustomAttribute.Index] <- customAttributeRows.Count
        counts[TableNames.Property.Index] <- propertyRows.Count
        counts[TableNames.Event.Index] <- eventRows.Count
        counts[TableNames.PropertyMap.Index] <- propertyMapRows.Count
        counts[TableNames.EventMap.Index] <- eventMapRows.Count
        counts[TableNames.MethodSemantics.Index] <- methodSemanticsRows.Count
        counts[TableNames.ENCLog.Index] <- encLogRows.Count
        counts[TableNames.ENCMap.Index] <- encMapRows.Count
        counts

    /// Add a user string literal to the delta's #US heap.
    /// The offset parameter is the ABSOLUTE offset from IL tokens (baseline size + delta-local offset).
    /// We convert to RELATIVE offset within the delta heap bytes, since the delta heap starts at 0
    /// but the stream header will indicate it represents data starting at heapOffsets.UserStringHeapStart.
    /// This matches how the runtime resolves tokens: absolute_token - stream_header_offset = position_in_delta_bytes.
    member _.AddUserStringLiteral(offset: int, value: string) =
        let start = heapOffsets.UserStringHeapStart
        // Use >= to properly compute relative offset when offset equals the heap start
        let relativeOffset = if offset >= start then offset - start else offset
        if traceHeapOffsets.Value then
            printfn "[fsharp-hotreload][heap-offsets] AddUserStringLiteral: absolute offset=%d, heapStart=%d, relative=%d, value=%A%s"
                offset start relativeOffset (value.Substring(0, min 20 value.Length)) (if value.Length > 20 then "..." else "")
            if offset <= start then
                printfn "[fsharp-hotreload][heap-offsets] WARNING: offset %d <= heapStart %d - this may indicate stale baseline!" offset start
        userStrings.AddEntry(relativeOffset, value)
        userStringHeapBytesCache <- None

    // =========================================================================
    // IMetadataHeaps interface implementation
    // Provides unified heap access for code that works with both full assembly
    // and delta emission.
    // =========================================================================

    /// Get the IMetadataHeaps interface for unified heap access.
    member this.AsMetadataHeaps() : IMetadataHeaps =
        { new IMetadataHeaps with
            member _.GetStringHeapIdx s = addStringValue s
            member _.GetBlobHeapIdx bytes = addBlobBytes bytes
            member _.GetGuidIdx info = guids.AddSharedEntry info
            member _.GetUserStringHeapIdx s = addUserStringValue s }

    // =========================================================================
    // IEncLogWriter interface implementation
    // Provides unified EncLog recording for delta emission.
    // =========================================================================

    /// Get an IEncLogWriter that records to this table's EncLog/EncMap.
    member this.AsEncLogWriter() : IEncLogWriter =
        { new IEncLogWriter with
            member _.RecordAddition(table, rowId, operation) =
                this.AddEncLogRow(table, rowId, operation)
            member _.RecordUpdate(table, rowId) =
                this.AddEncLogRow(table, rowId, EditAndContinueOperation.Default)
            member _.RecordEncMapEntry(table, rowId) =
                this.AddEncMapRow(table, rowId) }
