module internal FSharp.Compiler.IlxDeltaStreams

open System
open System.Collections.Generic
open System.Text
open FSharp.Compiler.AbstractIL.BinaryConstants
open FSharp.Compiler.AbstractIL.ILBinaryWriter
open FSharp.Compiler.AbstractIL.ILDeltaHandles
open FSharp.Compiler.IO

// ============================================================================
// Pure F# Token Calculators (replaces SRM MetadataBuilder for token arithmetic)
// ============================================================================

/// User string heap token calculator.
/// Tracks user strings added during delta emission and computes tokens.
/// Token format: 0x70000000 | heap_offset
type UserStringTokenCalculator(heapStartOffset: int) =
    let cache = Dictionary<string, int>(StringComparer.Ordinal)
    // #US heaps reserve offset 0 for the null/empty entry.
    // First emitted delta literal must start at relative offset 1.
    let mutable currentOffset = 1

    /// Encode a user string per ECMA-335 II.24.2.4:
    /// - Compressed length prefix (1-4 bytes)
    /// - UTF-16LE encoded characters
    /// - Terminal byte computed via markerForUnicodeBytes (shared with ilwrite.fs)
    let encodeUserString (value: string) : byte[] =
        let utf16Bytes = Encoding.Unicode.GetBytes(value)
        let blobLength = utf16Bytes.Length + 1 // +1 for terminal byte

        // Compute compressed length encoding size
        let lengthBytes =
            if blobLength <= 0x7F then 1
            elif blobLength <= 0x3FFF then 2
            else 4

        let result = Array.zeroCreate<byte>(lengthBytes + utf16Bytes.Length + 1)
        let mutable pos = 0

        // Write compressed length
        if blobLength <= 0x7F then
            result.[pos] <- byte blobLength
            pos <- pos + 1
        elif blobLength <= 0x3FFF then
            result.[pos] <- byte (0x80 ||| (blobLength >>> 8))
            result.[pos + 1] <- byte blobLength
            pos <- pos + 2
        else
            result.[pos] <- byte (0xC0 ||| (blobLength >>> 24))
            result.[pos + 1] <- byte (blobLength >>> 16)
            result.[pos + 2] <- byte (blobLength >>> 8)
            result.[pos + 3] <- byte blobLength
            pos <- pos + 4

        // Write UTF-16LE bytes
        Buffer.BlockCopy(utf16Bytes, 0, result, pos, utf16Bytes.Length)
        pos <- pos + utf16Bytes.Length

        // Write terminal byte - use shared markerForUnicodeBytes from ILBinaryWriter
        result.[pos] <- byte (markerForUnicodeBytes utf16Bytes)

        result

    /// Get or add a user string, returning the absolute token.
    member _.GetOrAddUserString(value: string) : int =
        match cache.TryGetValue(value) with
        | true, token -> token
        | _ ->
            let absoluteOffset = heapStartOffset + currentOffset
            let token = 0x70000000 ||| absoluteOffset
            cache.[value] <- token
            let encoded = encodeUserString value
            currentOffset <- currentOffset + encoded.Length
            token

    /// Get the list of (originalToken, newToken, value) tuples for all added strings.
    member _.GetUpdates() : (int * string) list =
        cache |> Seq.map (fun kvp -> (kvp.Value, kvp.Key)) |> Seq.toList

/// Standalone signature token calculator.
/// Tracks signatures added during delta emission and computes tokens.
/// Token format: 0x11000000 | row_id (StandaloneSig table = 0x11)
type StandaloneSignatureTokenCalculator(baselineRowCount: int) =
    let cache = Dictionary<byte[], int>(HashIdentity.Structural)
    let signatures = ResizeArray<int * byte[]>()
    let mutable nextRowId = baselineRowCount + 1

    /// Add a standalone signature and return its token.
    member _.AddStandaloneSignature(signature: byte[]) : int =
        if signature.Length = 0 then
            0
        else
            match cache.TryGetValue(signature) with
            | true, token -> token
            | _ ->
                let rowId = nextRowId
                nextRowId <- nextRowId + 1
                let token = 0x11000000 ||| rowId
                cache.[Array.copy signature] <- token
                signatures.Add((rowId, Array.copy signature))
                token

    /// Get the list of (rowId, blob) tuples for serialization.
    member _.GetSignatures() : (int * byte[]) list =
        signatures |> Seq.toList

/// <summary>Represents a method body update captured for an Edit-and-Continue delta.</summary>
type MethodBodyUpdate =
    {
        MethodToken: int
        LocalSignatureToken: int
        CodeOffset: int
        CodeLength: int
    }

/// <summary>Represents a standalone signature (e.g., local signature) emitted in the delta metadata.</summary>
type StandaloneSignatureUpdate =
    {
        RowId: int
        Blob: byte[]
    }

/// <summary>The emitted metadata and IL payloads produced by <see cref="IlDeltaStreamBuilder"/>.</summary>
type IlDeltaStreams =
    {
        IL: byte[]
        MethodBodies: MethodBodyUpdate list
        StandaloneSignatures: StandaloneSignatureUpdate list
    }

/// <summary>
/// Accumulates metadata tables, Edit-and-Continue bookkeeping, and encoded method bodies prior to serialising
/// a hot reload delta. Uses pure F# token calculators instead of SRM MetadataBuilder.
/// Callers retrieve the resulting byte arrays via <see cref="Build"/>.
/// </summary>
type IlDeltaStreamBuilder(baselineMetadata: MetadataSnapshot option) =
    // Initialize token calculators with baseline heap/table offsets
    let userStringHeapStart, standaloneSigRowCount =
        match baselineMetadata with
        | Some snapshot ->
            snapshot.HeapSizes.UserStringHeapSize,
            snapshot.TableRowCounts.[TableNames.StandAloneSig.Index]
        | None -> 0, 0

    let userStringCalculator = UserStringTokenCalculator(userStringHeapStart)
    let standaloneSigCalculator = StandaloneSignatureTokenCalculator(standaloneSigRowCount)

    let methodBodyStream = ByteBuffer.Create(256)
    let methodBodies = ResizeArray<MethodBodyUpdate>()
    let mutable isBuilt = false

    let alignStream alignment =
        // Align to N-byte boundary by padding with zeros
        let pos = methodBodyStream.Position
        let padding = (alignment - (pos % alignment)) % alignment
        for _ = 1 to padding do
            methodBodyStream.EmitByte 0uy

    /// <summary>Expose the user string token calculator for advanced scenarios.</summary>
    member _.UserStringCalculator = userStringCalculator

    /// <summary>Inspection hook primarily used in unit tests.</summary>
    member _.MethodBodies = methodBodies |> Seq.toList

    /// <summary>Get the standalone signatures that were added.</summary>
    member _.StandaloneSignatures =
        standaloneSigCalculator.GetSignatures()
        |> List.map (fun (rowId, blob) -> { RowId = rowId; Blob = blob })

    /// <summary>Add a method body update for the supplied metadata token.</summary>
    member _.AddMethodBody(
        methodToken: int,
        localSignatureToken: int,
        ilBytes: byte[],
        maxStack: int,
        initLocals: bool,
        exceptionRegions: IlExceptionRegion[],
        remapEntityToken: int -> int
    ) =
        let ilLength = ilBytes.Length
        let hasExceptionRegions = exceptionRegions.Length > 0

        let flags =
            int e_CorILMethod_FatFormat
            ||| (if hasExceptionRegions then int e_CorILMethod_MoreSects else 0)
            ||| (if initLocals then int e_CorILMethod_InitLocals else 0)

        alignStream 4
        let offset = methodBodyStream.Position

        methodBodyStream.EmitByte(byte flags)
        methodBodyStream.EmitByte(0x30uy)
        methodBodyStream.EmitUInt16(uint16 maxStack)
        methodBodyStream.EmitInt32(ilLength)
        methodBodyStream.EmitInt32(localSignatureToken)
        methodBodyStream.EmitBytes(ilBytes)

        let padding = (4 - (ilLength % 4)) &&& 0x3
        if padding > 0 then
            for _ = 1 to padding do
                methodBodyStream.EmitByte 0uy

        if hasExceptionRegions then
            alignStream 4
            let regions = exceptionRegions
            let smallSize = regions.Length * 12 + 4
            let canUseSmall =
                smallSize <= 0xFF
                && regions
                   |> Array.forall (fun region ->
                       region.TryOffset <= 0xFFFF
                       && region.HandlerOffset <= 0xFFFF
                       && region.TryLength <= 0xFF
                       && region.HandlerLength <= 0xFF)

            let encodeKind (region: IlExceptionRegion) : int * int =
                match region.Kind with
                | IlExceptionRegionKind.Catch ->
                    let token =
                        if region.CatchTypeToken = 0 then 0
                        else remapEntityToken region.CatchTypeToken
                    e_COR_ILEXCEPTION_CLAUSE_EXCEPTION, token
                | IlExceptionRegionKind.Filter -> e_COR_ILEXCEPTION_CLAUSE_FILTER, region.FilterOffset
                | IlExceptionRegionKind.Finally -> e_COR_ILEXCEPTION_CLAUSE_FINALLY, 0
                | IlExceptionRegionKind.Fault -> e_COR_ILEXCEPTION_CLAUSE_FAULT, 0
                | _ -> e_COR_ILEXCEPTION_CLAUSE_EXCEPTION, 0

            if canUseSmall then
                methodBodyStream.EmitByte(e_CorILMethod_Sect_EHTable)
                methodBodyStream.EmitByte(byte smallSize)
                methodBodyStream.EmitByte(0uy)
                methodBodyStream.EmitByte(0uy)
                for region in regions do
                    let kind, extra = encodeKind region
                    methodBodyStream.EmitUInt16(uint16 kind)
                    methodBodyStream.EmitUInt16(uint16 region.TryOffset)
                    methodBodyStream.EmitByte(byte region.TryLength)
                    methodBodyStream.EmitUInt16(uint16 region.HandlerOffset)
                    methodBodyStream.EmitByte(byte region.HandlerLength)
                    methodBodyStream.EmitInt32(extra)
            else
                let bigSize = regions.Length * 24 + 4
                methodBodyStream.EmitByte(e_CorILMethod_Sect_EHTable ||| e_CorILMethod_Sect_FatFormat)
                methodBodyStream.EmitByte(byte bigSize)
                methodBodyStream.EmitByte(byte (bigSize >>> 8))
                methodBodyStream.EmitByte(byte (bigSize >>> 16))
                for region in regions do
                    let kind, extra = encodeKind region
                    methodBodyStream.EmitInt32(kind)
                    methodBodyStream.EmitInt32(region.TryOffset)
                    methodBodyStream.EmitInt32(region.TryLength)
                    methodBodyStream.EmitInt32(region.HandlerOffset)
                    methodBodyStream.EmitInt32(region.HandlerLength)
                    methodBodyStream.EmitInt32(extra)

        let update =
            {
                MethodToken = methodToken
                LocalSignatureToken = localSignatureToken
                CodeOffset = offset
                CodeLength = ilLength
            }

        methodBodies.Add(update)
        update

    /// <summary>Adds a standalone signature blob to the metadata stream and returns its token.</summary>
    member _.AddStandaloneSignature(signature: byte[]) =
        standaloneSigCalculator.AddStandaloneSignature(signature)

    /// <summary>
    /// Finalise the builder and emit the metadata and IL blobs. The builder can only be consumed once; subsequent
    /// invocations throw to prevent mismatched Edit-and-Continue state.
    /// </summary>
    member this.Build() =
        if isBuilt then invalidOp "IlDeltaStreamBuilder.Build may only be called once per builder instance."
        isBuilt <- true

        {
            IL = methodBodyStream.AsMemory().ToArray()
            MethodBodies = methodBodies |> Seq.toList
            StandaloneSignatures = this.StandaloneSignatures
        }
