// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.AbstractIL.Support

open System
open System.IO
open System.Reflection
open System.Runtime.InteropServices
open Internal.Utilities.Library
open FSharp.Compiler.AbstractIL.NativeRes
open FSharp.Compiler.IO

let DateTime1970Jan01 =
    DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) (* ECMA Spec (Oct2002), Part II, 24.2.2 PE File Header. *)

let absilWriteGetTimeStamp () =
    (DateTime.UtcNow - DateTime1970Jan01).TotalSeconds |> int

// Force inline, so GetLastWin32Error calls are immediately after interop calls as seen by FxCop under Debug build.
let inline ignore _x = ()

// Native Resource linking/unlinking
type IStream = System.Runtime.InteropServices.ComTypes.IStream

let check _action hresult =
    if uint32 hresult >= 0x80000000ul then
        Marshal.ThrowExceptionForHR hresult
//printf "action = %s, hresult = 0x%nx \n" action hresult

let MAX_PATH = 260

let E_FAIL = 0x80004005

let bytesToWord (b0: byte, b1: byte) = int16 b0 ||| (int16 b1 <<< 8)

let bytesToDWord (b0: byte, b1: byte, b2: byte, b3: byte) =
    int b0 ||| (int b1 <<< 8) ||| (int b2 <<< 16) ||| (int b3 <<< 24)

let bytesToQWord (b0: byte, b1: byte, b2: byte, b3: byte, b4: byte, b5: byte, b6: byte, b7: byte) =
    int64 b0
    ||| (int64 b1 <<< 8)
    ||| (int64 b2 <<< 16)
    ||| (int64 b3 <<< 24)
    ||| (int64 b4 <<< 32)
    ||| (int64 b5 <<< 40)
    ||| (int64 b6 <<< 48)
    ||| (int64 b7 <<< 56)

let dwToBytes n =
    [|
        byte (n &&& 0xff)
        byte ((n >>> 8) &&& 0xff)
        byte ((n >>> 16) &&& 0xff)
        byte ((n >>> 24) &&& 0xff)
    |],
    4

let wToBytes (n: int16) =
    [| byte (n &&& 0xffs); byte ((n >>> 8) &&& 0xffs) |], 2

// REVIEW: factor these classes under one hierarchy, use reflection for creation from buffer and toBytes()
// Though, everything I'd like to unify is static - metaclasses?
type IMAGE_FILE_HEADER(m: int16, secs: int16, tds: int32, ptst: int32, nos: int32, soh: int16, c: int16) =
    let mutable machine = m
    let mutable numberOfSections = secs
    let mutable timeDateStamp = tds
    let mutable pointerToSymbolTable = ptst
    let mutable numberOfSymbols = nos
    let mutable sizeOfOptionalHeader = soh
    let mutable characteristics = c

    member x.Machine
        with get () = machine
        and set value = machine <- value

    member x.NumberOfSections
        with get () = numberOfSections
        and set value = numberOfSections <- value

    member x.TimeDateStamp
        with get () = timeDateStamp
        and set value = timeDateStamp <- value

    member x.PointerToSymbolTable
        with get () = pointerToSymbolTable
        and set value = pointerToSymbolTable <- value

    member x.NumberOfSymbols
        with get () = numberOfSymbols
        and set value = numberOfSymbols <- value

    member x.SizeOfOptionalHeader
        with get () = sizeOfOptionalHeader
        and set value = sizeOfOptionalHeader <- value

    member x.Characteristics
        with get () = characteristics
        and set value = characteristics <- value

    static member Width = 20

    member x.toBytes() =
        use buf = ByteBuffer.Create IMAGE_FILE_HEADER.Width
        buf.EmitUInt16(uint16 machine)
        buf.EmitUInt16(uint16 numberOfSections)
        buf.EmitInt32 timeDateStamp
        buf.EmitInt32 pointerToSymbolTable
        buf.EmitInt32 numberOfSymbols
        buf.EmitUInt16(uint16 sizeOfOptionalHeader)
        buf.EmitUInt16(uint16 characteristics)
        buf.AsMemory().ToArray()

let bytesToIFH (buffer: byte[]) (offset: int) =
    if (buffer.Length - offset) < IMAGE_FILE_HEADER.Width then
        invalidArg "buffer" "buffer too small to fit an IMAGE_FILE_HEADER"

    IMAGE_FILE_HEADER(
        bytesToWord (buffer[offset], buffer[offset + 1]), // Machine
        bytesToWord (buffer[offset + 2], buffer[offset + 3]), // NumberOfSections
        bytesToDWord (buffer[offset + 4], buffer[offset + 5], buffer[offset + 6], buffer[offset + 7]), // TimeDateStamp
        bytesToDWord (buffer[offset + 8], buffer[offset + 9], buffer[offset + 10], buffer[offset + 11]), // PointerToSymbolTable
        bytesToDWord (buffer[offset + 12], buffer[offset + 13], buffer[offset + 14], buffer[offset + 15]), // NumberOfSymbols
        bytesToWord (buffer[offset + 16], buffer[offset + 17]), // SizeOfOptionalHeader
        bytesToWord (buffer[offset + 18], buffer[offset + 19])
    ) // Characteristics

type IMAGE_SECTION_HEADER(n: int64, ai: int32, va: int32, srd: int32, prd: int32, pr: int32, pln: int32, nr: int16, nl: int16, c: int32) =
    let mutable name = n
    let mutable addressInfo = ai // PhysicalAddress / VirtualSize
    let mutable virtualAddress = va
    let mutable sizeOfRawData = srd
    let mutable pointerToRawData = prd
    let mutable pointerToRelocations = pr
    let mutable pointerToLineNumbers = pln
    let mutable numberOfRelocations = nr
    let mutable numberOfLineNumbers = nl
    let mutable characteristics = c

    member x.Name
        with get () = name
        and set value = name <- value

    member x.PhysicalAddress
        with get () = addressInfo
        and set value = addressInfo <- value

    member x.VirtualSize
        with get () = addressInfo
        and set value = addressInfo <- value

    member x.VirtualAddress
        with get () = virtualAddress
        and set value = virtualAddress <- value

    member x.SizeOfRawData
        with get () = sizeOfRawData
        and set value = sizeOfRawData <- value

    member x.PointerToRawData
        with get () = pointerToRawData
        and set value = pointerToRawData <- value

    member x.PointerToRelocations
        with get () = pointerToRelocations
        and set value = pointerToRelocations <- value

    member x.PointerToLineNumbers
        with get () = pointerToLineNumbers
        and set value = pointerToLineNumbers <- value

    member x.NumberOfRelocations
        with get () = numberOfRelocations
        and set value = numberOfRelocations <- value

    member x.NumberOfLineNumbers
        with get () = numberOfLineNumbers
        and set value = numberOfLineNumbers <- value

    member x.Characteristics
        with get () = characteristics
        and set value = characteristics <- value

    static member Width = 40

    member x.toBytes() =
        use buf = ByteBuffer.Create IMAGE_SECTION_HEADER.Width
        buf.EmitInt64 name
        buf.EmitInt32 addressInfo
        buf.EmitInt32 virtualAddress
        buf.EmitInt32 sizeOfRawData
        buf.EmitInt32 pointerToRawData
        buf.EmitInt32 pointerToRelocations
        buf.EmitInt32 pointerToLineNumbers
        buf.EmitUInt16(uint16 numberOfRelocations)
        buf.EmitUInt16(uint16 numberOfLineNumbers)
        buf.EmitInt32 characteristics
        buf.AsMemory().ToArray()

let bytesToISH (buffer: byte[]) (offset: int) =
    if (buffer.Length - offset) < IMAGE_SECTION_HEADER.Width then
        invalidArg "buffer" "buffer too small to fit an IMAGE_SECTION_HEADER"

    IMAGE_SECTION_HEADER(
        bytesToQWord (
            buffer[offset],
            buffer[offset + 1],
            buffer[offset + 2],
            buffer[offset + 3],
            buffer[offset + 4],
            buffer[offset + 5],
            buffer[offset + 6],
            buffer[offset + 7]
        ), // Name
        bytesToDWord (buffer[offset + 8], buffer[offset + 9], buffer[offset + 10], buffer[offset + 11]), // AddressInfo
        bytesToDWord (buffer[offset + 12], buffer[offset + 13], buffer[offset + 14], buffer[offset + 15]), // VirtualAddress
        bytesToDWord (buffer[offset + 16], buffer[offset + 17], buffer[offset + 18], buffer[offset + 19]), // SizeOfRawData
        bytesToDWord (buffer[offset + 20], buffer[offset + 21], buffer[offset + 22], buffer[offset + 23]), // PointerToRawData
        bytesToDWord (buffer[offset + 24], buffer[offset + 25], buffer[offset + 26], buffer[offset + 27]), // PointerToRelocations
        bytesToDWord (buffer[offset + 28], buffer[offset + 29], buffer[offset + 30], buffer[offset + 31]), // PointerToLineNumbers
        bytesToWord (buffer[offset + 32], buffer[offset + 33]), // NumberOfRelocations
        bytesToWord (buffer[offset + 34], buffer[offset + 35]), // NumberOfLineNumbers
        bytesToDWord (buffer[offset + 36], buffer[offset + 37], buffer[offset + 38], buffer[offset + 39])
    ) // Characteristics

type IMAGE_SYMBOL(n: int64, v: int32, sn: int16, t: int16, sc: byte, nas: byte) =
    let mutable name = n
    let mutable value = v
    let mutable sectionNumber = sn
    let mutable stype = t
    let mutable storageClass = sc
    let mutable numberOfAuxSymbols = nas

    member x.Name
        with get () = name
        and set v = name <- v

    member x.Value
        with get () = value
        and set v = value <- v

    member x.SectionNumber
        with get () = sectionNumber
        and set v = sectionNumber <- v

    member x.Type
        with get () = stype
        and set v = stype <- v

    member x.StorageClass
        with get () = storageClass
        and set v = storageClass <- v

    member x.NumberOfAuxSymbols
        with get () = numberOfAuxSymbols
        and set v = numberOfAuxSymbols <- v

    static member Width = 18

    member x.toBytes() =
        use buf = ByteBuffer.Create IMAGE_SYMBOL.Width
        buf.EmitInt64 name
        buf.EmitInt32 value
        buf.EmitUInt16(uint16 sectionNumber)
        buf.EmitUInt16(uint16 stype)
        buf.EmitByte storageClass
        buf.EmitByte numberOfAuxSymbols
        buf.AsMemory().ToArray()

let bytesToIS (buffer: byte[]) (offset: int) =
    if (buffer.Length - offset) < IMAGE_SYMBOL.Width then
        invalidArg "buffer" "buffer too small to fit an IMAGE_SYMBOL"

    IMAGE_SYMBOL(
        bytesToQWord (
            buffer[offset],
            buffer[offset + 1],
            buffer[offset + 2],
            buffer[offset + 3],
            buffer[offset + 4],
            buffer[offset + 5],
            buffer[offset + 6],
            buffer[offset + 7]
        ), // Name
        bytesToDWord (buffer[offset + 8], buffer[offset + 9], buffer[offset + 10], buffer[offset + 11]), // Value
        bytesToWord (buffer[offset + 12], buffer[offset + 13]), // SectionNumber
        bytesToWord (buffer[offset + 14], buffer[offset + 15]), // Type
        buffer[offset + 16],
        buffer[offset + 17]
    ) // NumberOfAuxSymbols

type IMAGE_RELOCATION(va: int32, sti: int32, t: int16) =
    let mutable virtualAddress = va // Also RelocCount
    let mutable symbolTableIndex = sti
    let mutable ty = t // type

    member x.VirtualAddress
        with get () = virtualAddress
        and set v = virtualAddress <- v

    member x.RelocCount
        with get () = virtualAddress
        and set v = virtualAddress <- v

    member x.SymbolTableIndex
        with get () = symbolTableIndex
        and set v = symbolTableIndex <- v

    member x.Type
        with get () = ty
        and set v = ty <- v

    static member Width = 10

    member x.toBytes() =
        use buf = ByteBuffer.Create IMAGE_RELOCATION.Width
        buf.EmitInt32 virtualAddress
        buf.EmitInt32 symbolTableIndex
        buf.EmitUInt16(uint16 ty)
        buf.AsMemory().ToArray()

let bytesToIR (buffer: byte[]) (offset: int) =
    if (buffer.Length - offset) < IMAGE_RELOCATION.Width then
        invalidArg "buffer" "buffer too small to fit an IMAGE_RELOCATION"

    IMAGE_RELOCATION(
        bytesToDWord (buffer[offset], buffer[offset + 1], buffer[offset + 2], buffer[offset + 3]),
        bytesToDWord (buffer[offset + 4], buffer[offset + 5], buffer[offset + 6], buffer[offset + 7]),
        bytesToWord (buffer[offset + 8], buffer[offset + 9])
    )

type IMAGE_RESOURCE_DIRECTORY(c: int32, tds: int32, mjv: int16, mnv: int16, nne: int16, nie: int16) =
    let mutable characteristics = c
    let mutable timeDateStamp = tds
    let mutable majorVersion = mjv
    let mutable minorVersion = mnv
    let mutable numberOfNamedEntries = nne
    let mutable numberOfIdEntries = nie

    member x.Characteristics
        with get () = characteristics
        and set v = characteristics <- v

    member x.TimeDateStamp
        with get () = timeDateStamp
        and set v = timeDateStamp <- v

    member x.MajorVersion
        with get () = majorVersion
        and set v = majorVersion <- v

    member x.MinorVersion
        with get () = minorVersion
        and set v = minorVersion <- v

    member x.NumberOfNamedEntries
        with get () = numberOfNamedEntries
        and set v = numberOfNamedEntries <- v

    member x.NumberOfIdEntries
        with get () = numberOfIdEntries
        and set v = numberOfIdEntries <- v

    static member Width = 16

    member x.toBytes() =
        use buf = ByteBuffer.Create IMAGE_RESOURCE_DIRECTORY.Width
        buf.EmitInt32 characteristics
        buf.EmitInt32 timeDateStamp
        buf.EmitUInt16(uint16 majorVersion)
        buf.EmitUInt16(uint16 minorVersion)
        buf.EmitUInt16(uint16 numberOfNamedEntries)
        buf.EmitUInt16(uint16 numberOfIdEntries)
        buf.AsMemory().ToArray()

let bytesToIRD (buffer: byte[]) (offset: int) =
    if (buffer.Length - offset) < IMAGE_RESOURCE_DIRECTORY.Width then
        invalidArg "buffer" "buffer too small to fit an IMAGE_RESOURCE_DIRECTORY"

    IMAGE_RESOURCE_DIRECTORY(
        bytesToDWord (buffer[offset], buffer[offset + 1], buffer[offset + 2], buffer[offset + 3]), // Characteristics
        bytesToDWord (buffer[offset + 4], buffer[offset + 5], buffer[offset + 6], buffer[offset + 7]), // TimeDateStamp
        bytesToWord (buffer[offset + 8], buffer[offset + 9]), // MajorVersion
        bytesToWord (buffer[offset + 10], buffer[offset + 11]), // MinorVersion
        bytesToWord (buffer[offset + 12], buffer[offset + 13]), // NumberOfNamedEntries
        bytesToWord (buffer[offset + 14], buffer[offset + 15])
    ) // NumberOfIdEntries

type IMAGE_RESOURCE_DIRECTORY_ENTRY(n: int32, o: int32) =
    let mutable name = n
    let mutable offset = o

    member x.Name
        with get () = name
        and set v = name <- v

    member x.OffsetToData
        with get () = offset
        and set v = offset <- v

    member x.OffsetToDirectory = offset &&& 0x7fffffff

    member x.DataIsDirectory = (offset &&& 0x80000000) <> 0

    static member Width = 8

    member x.toBytes() =
        use buf = ByteBuffer.Create IMAGE_RESOURCE_DIRECTORY_ENTRY.Width
        buf.EmitInt32 name
        buf.EmitInt32 offset
        buf.AsMemory().ToArray()

let bytesToIRDE (buffer: byte[]) (offset: int) =
    if (buffer.Length - offset) < IMAGE_RESOURCE_DIRECTORY_ENTRY.Width then
        invalidArg "buffer" "buffer too small to fit an IMAGE_RESOURCE_DIRECTORY_ENTRY"

    IMAGE_RESOURCE_DIRECTORY_ENTRY(
        bytesToDWord (buffer[offset], buffer[offset + 1], buffer[offset + 2], buffer[offset + 3]), // Name
        bytesToDWord (buffer[offset + 4], buffer[offset + 5], buffer[offset + 6], buffer[offset + 7])
    ) // Offset

type IMAGE_RESOURCE_DATA_ENTRY(o: int32, s: int32, c: int32, r: int32) =
    let mutable offsetToData = o
    let mutable size = s
    let mutable codePage = c
    let mutable reserved = r

    member x.OffsetToData
        with get () = offsetToData
        and set v = offsetToData <- v

    member x.Size
        with get () = size
        and set v = size <- v

    member x.CodePage
        with get () = codePage
        and set v = codePage <- v

    member x.Reserved
        with get () = reserved
        and set v = reserved <- v

    static member Width = 16

    member x.toBytes() =
        use buf = ByteBuffer.Create IMAGE_RESOURCE_DATA_ENTRY.Width
        buf.EmitInt32 offsetToData
        buf.EmitInt32 size
        buf.EmitInt32 codePage
        buf.EmitInt32 reserved

let bytesToIRDataE (buffer: byte[]) (offset: int) =
    if (buffer.Length - offset) < IMAGE_RESOURCE_DATA_ENTRY.Width then
        invalidArg "buffer" "buffer too small to fit an IMAGE_RESOURCE_DATA_ENTRY"

    IMAGE_RESOURCE_DATA_ENTRY(
        bytesToDWord (buffer[offset], buffer[offset + 1], buffer[offset + 2], buffer[offset + 3]), // OffsetToData
        bytesToDWord (buffer[offset + 4], buffer[offset + 5], buffer[offset + 6], buffer[offset + 7]), // Size
        bytesToDWord (buffer[offset + 8], buffer[offset + 9], buffer[offset + 10], buffer[offset + 11]), // CodePage
        bytesToDWord (buffer[offset + 12], buffer[offset + 13], buffer[offset + 14], buffer[offset + 15])
    ) // Reserved

type ResFormatHeader() =
    let mutable dwDataSize = 0
    let mutable dwHeaderSize = 32 // The eventual supposed size of this structure in memory
    let mutable dwTypeID = 0xffff
    let mutable dwNameID = 0xffff
    let mutable dwDataVersion = 0
    let mutable wMemFlags = 0s
    let mutable wLangID = 0s
    let mutable dwVersion = 0
    let mutable dwCharacteristics = 0

    member x.DataSize
        with get () = dwDataSize
        and set v = dwDataSize <- v

    member x.HeaderSize
        with get () = dwHeaderSize
        and set v = dwHeaderSize <- v

    member x.TypeID
        with get () = dwTypeID
        and set v = dwTypeID <- v

    member x.NameID
        with get () = dwNameID
        and set v = dwNameID <- v

    member x.DataVersion
        with get () = dwDataVersion
        and set v = dwDataVersion <- v

    member x.MemFlags
        with get () = wMemFlags
        and set v = wMemFlags <- v

    member x.LangID
        with get () = wLangID
        and set v = wLangID <- v

    member x.Version
        with get () = dwVersion
        and set v = dwVersion <- v

    member x.Characteristics
        with get () = dwCharacteristics
        and set v = dwCharacteristics <- v

    static member Width = 32

    member x.toBytes() =
        use buf = ByteBuffer.Create ResFormatHeader.Width
        buf.EmitInt32 dwDataSize
        buf.EmitInt32 dwHeaderSize
        buf.EmitInt32 dwTypeID
        buf.EmitInt32 dwNameID
        buf.EmitInt32 dwDataVersion
        buf.EmitUInt16(uint16 wMemFlags)
        buf.EmitUInt16(uint16 wLangID)
        buf.EmitInt32 dwVersion
        buf.EmitInt32 dwCharacteristics
        buf.AsMemory().ToArray()

type ResFormatNode(tid: int32, nid: int32, lid: int32, dataOffset: int32, pbLinkedResource: byte[]) =
    let mutable resHdr = ResFormatHeader()
    let mutable dataEntry = Unchecked.defaultof<IMAGE_RESOURCE_DATA_ENTRY>
    let mutable cType = 0
    let mutable wzType = Unchecked.defaultof<byte[]>
    let mutable cName = 0
    let mutable wzName = Unchecked.defaultof<byte[]>

    do
        if (tid &&& 0x80000000) <> 0 then // REVIEW: Are names and types mutually exclusive?  The C++ code didn't seem to think so, but I can't find any documentation
            resHdr.TypeID <- 0
            let mtid = tid &&& 0x7fffffff

            cType <-
                bytesToDWord (pbLinkedResource[mtid], pbLinkedResource[mtid + 1], pbLinkedResource[mtid + 2], pbLinkedResource[mtid + 3])

            wzType <- Bytes.zeroCreate ((cType + 1) * 2)
            Bytes.blit pbLinkedResource 4 wzType 0 (cType * 2)
        else
            resHdr.TypeID <- (0xffff ||| ((tid &&& 0xffff) <<< 16))

        if (nid &&& 0x80000000) <> 0 then
            resHdr.NameID <- 0
            let mnid = nid &&& 0x7fffffff

            cName <-
                bytesToDWord (pbLinkedResource[mnid], pbLinkedResource[mnid + 1], pbLinkedResource[mnid + 2], pbLinkedResource[mnid + 3])

            wzName <- Bytes.zeroCreate ((cName + 1) * 2)
            Bytes.blit pbLinkedResource 4 wzName 0 (cName * 2)
        else
            resHdr.NameID <- (0xffff ||| ((nid &&& 0xffff) <<< 16))

        resHdr.LangID <- int16 lid
        dataEntry <- bytesToIRDataE pbLinkedResource dataOffset
        resHdr.DataSize <- dataEntry.Size

    member x.ResHdr = resHdr
    member x.DataEntry = dataEntry
    member x.Type = wzType
    member x.Name = wzName

    member x.Save(ulLinkedResourceBaseRVA: int32, pbLinkedResource: byte[], pUnlinkedResource: byte[], offset: int) =
        // Dump them to pUnlinkedResource
        // For each resource write header and data
        let mutable size = 0
        let mutable unlinkedResourceOffset = 0
        //resHdr.HeaderSize <- 32
        if Unchecked.defaultof<byte[]> <> wzType then
            resHdr.HeaderSize <- resHdr.HeaderSize + ((cType + 1) * 2) - 4

        if Unchecked.defaultof<byte[]> <> wzName then
            resHdr.HeaderSize <- resHdr.HeaderSize + ((cName + 1) * 2) - 4

        let SaveChunk (p: byte[], sz: int) =
            if Unchecked.defaultof<byte[]> <> pUnlinkedResource then
                Bytes.blit p 0 pUnlinkedResource (unlinkedResourceOffset + offset) sz
                unlinkedResourceOffset <- unlinkedResourceOffset + sz

            size <- size + sz

            ()

        // ---- Constant part of the header: DWORD, DWORD
        SaveChunk(dwToBytes resHdr.DataSize)
        SaveChunk(dwToBytes resHdr.HeaderSize)

        let mutable dwFiller = 0

        if Unchecked.defaultof<byte[]> <> wzType then
            SaveChunk(wzType, ((cType + 1) * 2))
            dwFiller <- dwFiller + cType + 1
        else
            SaveChunk(dwToBytes resHdr.TypeID)

        if Unchecked.defaultof<byte[]> <> wzName then
            SaveChunk(wzName, ((cName + 1) * 2))
            dwFiller <- dwFiller + cName + 1
        else
            SaveChunk(dwToBytes resHdr.NameID)

        let bNil = Bytes.zeroCreate 3

        // Align remaining fields on DWORD (nb. poor bit twiddling code taken from ildasm's dres.cpp)
        if (dwFiller &&& 0x1) <> 0 then
            SaveChunk(bNil, 2)

        //---- Constant part of the header: DWORD, WORD, WORD, DWORD, DWORD
        SaveChunk(dwToBytes resHdr.DataVersion)
        SaveChunk(wToBytes resHdr.MemFlags)
        SaveChunk(wToBytes resHdr.LangID)
        SaveChunk(dwToBytes resHdr.Version)
        SaveChunk(dwToBytes resHdr.Characteristics)

        //---- Header done, now data
        // just copying to make the code a bit cleaner - can blit if this ends up being a liability
        let pbData = pbLinkedResource[(dataEntry.OffsetToData - ulLinkedResourceBaseRVA) ..]
        SaveChunk(pbData, dataEntry.Size)

        dwFiller <- dataEntry.Size &&& 0x3

        if dwFiller <> 0 then
            SaveChunk(bNil, 4 - dwFiller)

        size

let linkNativeResources (unlinkedResources: byte[] list) (rva: int32) =
    let resources =
        unlinkedResources
        |> Seq.map (fun s -> new MemoryStream(s))
        |> Seq.map (fun s ->
            let res = CvtResFile.ReadResFile s
            s.Dispose()
            res)
        |> Seq.collect id
        // See MakeWin32ResourceList https://github.com/dotnet/roslyn/blob/f40b89234db51da1e1153c14af184e618504be41/src/Compilers/Core/Portable/Compilation/Compilation.cs
        |> Seq.map (fun r ->
            Win32Resource(
                data = r.data,
                codePage = 0u,
                languageId = uint32 r.LanguageId,
                id = int (int16 r.pstringName.Ordinal),
                name = r.pstringName.theString,
                typeId = int (int16 r.pstringType.Ordinal),
                typeName = r.pstringType.theString
            ))

    let bb = System.Reflection.Metadata.BlobBuilder()
    NativeResourceWriter.SerializeWin32Resources(bb, resources, rva)
    bb.ToArray()

let unlinkResource (ulLinkedResourceBaseRVA: int32) (pbLinkedResource: byte[]) =
    let mutable nResNodes = 0

    let pirdType = bytesToIRD pbLinkedResource 0
    let mutable pirdeType = Unchecked.defaultof<IMAGE_RESOURCE_DIRECTORY_ENTRY>
    let nEntries = pirdType.NumberOfNamedEntries + pirdType.NumberOfIdEntries

    // determine entry buffer size
    // TODO: coalesce these two loops
    for iEntry = 0 to (int nEntries - 1) do
        pirdeType <- bytesToIRDE pbLinkedResource (IMAGE_RESOURCE_DIRECTORY.Width + (iEntry * IMAGE_RESOURCE_DIRECTORY_ENTRY.Width))

        if pirdeType.DataIsDirectory then
            let nameBase = pirdeType.OffsetToDirectory
            let pirdName = bytesToIRD pbLinkedResource nameBase
            let mutable pirdeName = Unchecked.defaultof<IMAGE_RESOURCE_DIRECTORY_ENTRY>
            let nEntries2 = pirdName.NumberOfNamedEntries + pirdName.NumberOfIdEntries

            for iEntry2 = 0 to (int nEntries2 - 1) do
                pirdeName <- bytesToIRDE pbLinkedResource (nameBase + (iEntry2 * IMAGE_RESOURCE_DIRECTORY_ENTRY.Width))

                if pirdeName.DataIsDirectory then
                    let langBase = pirdeName.OffsetToDirectory
                    let pirdLang = bytesToIRD pbLinkedResource langBase
                    let nEntries3 = pirdLang.NumberOfNamedEntries + pirdLang.NumberOfIdEntries

                    nResNodes <- nResNodes + (int nEntries3)
                else
                    nResNodes <- nResNodes + 1
        else
            nResNodes <- nResNodes + 1

    let pResNodes: ResFormatNode[] = Array.zeroCreate nResNodes
    nResNodes <- 0

    // fill out the entry buffer
    for iEntry = 0 to (int nEntries - 1) do
        pirdeType <- bytesToIRDE pbLinkedResource (IMAGE_RESOURCE_DIRECTORY.Width + (iEntry * IMAGE_RESOURCE_DIRECTORY_ENTRY.Width))
        let dwTypeID = pirdeType.Name
        // Need to skip VERSION and RT_MANIFEST resources
        // REVIEW: ideally we shouldn't allocate space for these, or rename properly so we don't get the naming conflict
        let skipResource = (0x10 = dwTypeID) || (0x18 = dwTypeID)

        if pirdeType.DataIsDirectory then
            let nameBase = pirdeType.OffsetToDirectory
            let pirdName = bytesToIRD pbLinkedResource nameBase
            let mutable pirdeName = Unchecked.defaultof<IMAGE_RESOURCE_DIRECTORY_ENTRY>
            let nEntries2 = pirdName.NumberOfNamedEntries + pirdName.NumberOfIdEntries

            for iEntry2 = 0 to (int nEntries2 - 1) do
                pirdeName <- bytesToIRDE pbLinkedResource (nameBase + (iEntry2 * IMAGE_RESOURCE_DIRECTORY_ENTRY.Width))
                let dwNameID = pirdeName.Name

                if pirdeName.DataIsDirectory then
                    let langBase = pirdeName.OffsetToDirectory
                    let pirdLang = bytesToIRD pbLinkedResource langBase
                    let mutable pirdeLang = Unchecked.defaultof<IMAGE_RESOURCE_DIRECTORY_ENTRY>
                    let nEntries3 = pirdLang.NumberOfNamedEntries + pirdLang.NumberOfIdEntries

                    for iEntry3 = 0 to (int nEntries3 - 1) do
                        pirdeLang <- bytesToIRDE pbLinkedResource (langBase + (iEntry3 * IMAGE_RESOURCE_DIRECTORY_ENTRY.Width))
                        let dwLangID = pirdeLang.Name

                        if pirdeLang.DataIsDirectory then
                            // Resource hierarchy exceeds three levels
                            Marshal.ThrowExceptionForHR(E_FAIL)
                        else if (not skipResource) then
                            let rfn =
                                ResFormatNode(dwTypeID, dwNameID, dwLangID, pirdeLang.OffsetToData, pbLinkedResource)

                            pResNodes[nResNodes] <- rfn
                            nResNodes <- nResNodes + 1
                else if (not skipResource) then
                    let rfn =
                        ResFormatNode(dwTypeID, dwNameID, 0, pirdeName.OffsetToData, pbLinkedResource)

                    pResNodes[nResNodes] <- rfn
                    nResNodes <- nResNodes + 1
        else if (not skipResource) then
            let rfn = ResFormatNode(dwTypeID, 0, 0, pirdeType.OffsetToData, pbLinkedResource) // REVIEW: I believe these 0s are what's causing the duplicate res naming problems
            pResNodes[nResNodes] <- rfn
            nResNodes <- nResNodes + 1

    // Ok, all tree leaves are in ResFormatNode structs, and nResNodes ptrs are in pResNodes
    let mutable size = 0

    if nResNodes <> 0 then
        size <- size + ResFormatHeader.Width // sizeof ResFormatHeader

        for i = 0 to (nResNodes - 1) do
            size <-
                size
                + pResNodes[i]
                    .Save(ulLinkedResourceBaseRVA, pbLinkedResource, Unchecked.defaultof<byte[]>, 0)

    let pResBuffer = Bytes.zeroCreate size

    if nResNodes <> 0 then
        let mutable resBufferOffset = 0

        // Write a dummy header
        let rfh = ResFormatHeader()
        let rfhBytes = rfh.toBytes ()
        Bytes.blit rfhBytes 0 pResBuffer 0 ResFormatHeader.Width
        resBufferOffset <- resBufferOffset + ResFormatHeader.Width

        for i = 0 to (nResNodes - 1) do
            resBufferOffset <-
                resBufferOffset
                + pResNodes[i]
                    .Save(ulLinkedResourceBaseRVA, pbLinkedResource, pResBuffer, resBufferOffset)

    pResBuffer
