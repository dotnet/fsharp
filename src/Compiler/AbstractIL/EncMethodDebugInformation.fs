/// Edit-and-Continue method debug information blobs.
///
/// This module replicates, byte for byte, the three Portable-PDB CustomDebugInformation
/// blob formats Roslyn persists per method to support Edit and Continue
/// (roslyn/src/Compilers/Core/Portable/Emit/EditAndContinueMethodDebugInformation.cs):
///
///   - EnC Local Slot Map           (kind 755F52A8-91C5-45BE-B4B8-209571E552BD)
///   - EnC Lambda and Closure Map   (kind A643004C-0240-496F-A783-30D64F4979DE)
///   - EnC State Machine State Map  (kind 8B78CD68-2EDE-420B-980B-E15884B8AAA3)
///
/// (GUIDs: roslyn/src/Dependencies/CodeAnalysis.Debugging/PortableCustomDebugInfoKinds.cs.)
///
/// All multi-byte integers use the ECMA-335 compressed unsigned/signed encodings via
/// System.Reflection.Metadata's BlobBuilder.WriteCompressedInteger /
/// WriteCompressedSignedInteger and BlobReader.ReadCompressedInteger /
/// ReadCompressedSignedInteger, exactly as Roslyn writes/reads them.
///
/// Every "syntax offset" slot in these blobs is an opaque, caller-defined integer key
/// (Roslyn: the syntax offset of the lambda/closure/state-machine-suspension syntax
/// node). This module does not require the key to be a source offset; it only requires
/// determinism across generations. tryEncodeOccurrenceKey/decodeOccurrenceKey provide one
/// reusable way to pack a short (depth <= 2) ordinal chain into such a key.
module internal FSharp.Compiler.AbstractIL.EncMethodDebugInformation

#nowarn "9" // NativePtr: BlobReader only exposes a byte*-based constructor

open System
open System.Collections.Generic
open System.Collections.Immutable
open System.IO
open System.Reflection.Metadata
open System.Reflection.Metadata.Ecma335
open System.Runtime.InteropServices
open Microsoft.FSharp.NativeInterop

/// Portable-PDB CustomDebugInformation kind GUIDs for the EnC blobs, copied verbatim
/// from roslyn/src/Dependencies/CodeAnalysis.Debugging/PortableCustomDebugInfoKinds.cs.
[<RequireQualifiedAccess>]
module PortableCustomDebugInfoKinds =

    /// EnC Local Slot Map CDI kind.
    let encLocalSlotMap = Guid("755F52A8-91C5-45BE-B4B8-209571E552BD")

    /// EnC Lambda and Closure Map CDI kind.
    let encLambdaAndClosureMap = Guid("A643004C-0240-496F-A783-30D64F4979DE")

    /// EnC State Machine State Map CDI kind.
    let encStateMachineStateMap = Guid("8B78CD68-2EDE-420B-980B-E15884B8AAA3")

/// Closure ordinal of a lambda that is lowered to a static (non-capturing) method.
/// Mirrors Roslyn's LambdaDebugInfo.StaticClosureOrdinal.
[<Literal>]
let StaticClosureOrdinal = -1

/// Closure ordinal of a lambda closed over the 'this' pointer only.
/// Mirrors Roslyn's LambdaDebugInfo.ThisOnlyClosureOrdinal.
[<Literal>]
let ThisOnlyClosureOrdinal = -2

/// Smallest valid closure ordinal. Mirrors Roslyn's LambdaDebugInfo.MinClosureOrdinal.
[<Literal>]
let MinClosureOrdinal = ThisOnlyClosureOrdinal

/// Method ordinal of a method that has no lambda map (an empty blob decodes to this).
/// Mirrors Roslyn's DebugId.UndefinedOrdinal.
[<Literal>]
let UndefinedMethodOrdinal = -1

/// Marker byte introducing the (optional) negative syntax-offset baseline in the
/// local-slot-map blob. Mirrors Roslyn's SyntaxOffsetBaseline = 0xFF.
[<Literal>]
let private SyntaxOffsetBaselineMarker = 0xFFuy

/// Largest synthesized-local kind serializable in the slot map: the kind is stored as
/// (kind + 1) in bits 0-5 of the leading byte (bit 6 is unused, bit 7 flags a trailing ordinal), and
/// Roslyn's reader recovers it with mask 0x3F, so only kinds 0..0x3E round-trip.
[<Literal>]
let MaxSerializableLocalKind = 0x3E

/// One slot in the EnC Local Slot Map: the local variable layout of a method body,
/// recorded so a later generation can map its locals onto the same slot indices.
[<RequireQualifiedAccess>]
type EncLocalSlotInfo =
    /// A short-lived lowering temp: serialized as the single byte 0x00, carrying no
    /// identity (a later generation never reuses it).
    | Temp

    /// A long-lived synthesized local.
    /// kind: synthesized-local kind (Roslyn SynthesizedLocalKind value, 0..MaxSerializableLocalKind;
    /// 0 = user-defined local).
    /// syntaxOffset: caller-defined key of the declaring occurrence
    /// (Roslyn: syntax offset of the local's declarator).
    /// ordinal: zero-based disambiguator among slots sharing the same kind and offset (>= 0).
    | Slot of kind: int * syntaxOffset: int * ordinal: int

/// One closure scope in the EnC Lambda and Closure Map. The closure's ordinal is its
/// index in EncMethodDebugInformation.Closures; lambdas reference closures by that index.
type EncClosureInfo =
    {
        /// Caller-defined key (Roslyn: syntax offset of the scope owning the closure).
        SyntaxOffset: int
    }

/// One lambda in the EnC Lambda and Closure Map.
type EncLambdaInfo =
    {
        /// Caller-defined key (Roslyn: syntax offset of the lambda body).
        SyntaxOffset: int
        /// Index into EncMethodDebugInformation.Closures of the closure holding the
        /// lambda's captures, or StaticClosureOrdinal / ThisOnlyClosureOrdinal.
        ClosureOrdinal: int
    }

/// One suspension point in the EnC State Machine State Map.
type EncStateMachineStateInfo =
    {
        /// State machine state number assigned to the suspension point (may be negative:
        /// Roslyn uses negative numbers for increasing-iteration finalize states).
        StateNumber: int
        /// Caller-defined key (Roslyn: syntax offset of the await/yield syntax node).
        SyntaxOffset: int
    }

/// Debugging information associated with a method, persisted by the compiler in the
/// Portable PDB to support Edit and Continue. Mirrors Roslyn's
/// EditAndContinueMethodDebugInformation.
type EncMethodDebugInformation =
    {
        /// Ordinal of the method within its generation (>= -1; UndefinedMethodOrdinal when absent).
        MethodOrdinal: int
        /// Local slot layout, in slot-index order (EnC Local Slot Map).
        LocalSlots: EncLocalSlotInfo list
        /// Closure scopes, in ordinal order (EnC Lambda and Closure Map).
        Closures: EncClosureInfo list
        /// Lambdas, in ordinal order (EnC Lambda and Closure Map).
        Lambdas: EncLambdaInfo list
        /// State machine suspension points (EnC State Machine State Map).
        StateMachineStates: EncStateMachineStateInfo list
    }

    /// An empty map (no slots, lambdas, closures or states; undefined method ordinal).
    static member Empty =
        {
            MethodOrdinal = UndefinedMethodOrdinal
            LocalSlots = []
            Closures = []
            Lambdas = []
            StateMachineStates = []
        }

// ---------------------------------------------------------------------------
// Occurrence-key packing
// ---------------------------------------------------------------------------

/// Maximum encodable occurrence ordinal: each chain segment is 16 bits.
[<Literal>]
let private MaxOccurrenceSegment = 0xFFFF

/// Compressed unsigned integers must lie in [0, 0x1FFFFFFF); after baseline adjustment
/// the serialized value is (key - baseline) with baseline <= -1, so keys must stay
/// strictly below 0x1FFFFFFF - 1 to be writable. Cap at 29 bits minus the adjustment.
[<Literal>]
let private MaxOccurrenceKey = 0x1FFFFFFD

/// Packs an ordinal chain (root-first enclosing ordinals, ending with the innermost
/// ordinal) into a deterministic int suitable for a "syntax offset" blob slot. Packing:
/// 16-bit segments, least-significant segment = the innermost ordinal; an enclosing
/// ordinal p is stored as (p + 1) shifted left 16 so that depth-1 keys (< 0x10000) and
/// depth-2 keys (>= 0x10000) never collide. Fails closed (None) past the limits: chains
/// deeper than 2, ordinals > 0xFFFF, or keys exceeding the compressed-integer budget —
/// callers must then treat the chain as unmappable, never truncate.
let tryEncodeOccurrenceKey (ordinalChain: int list) : int option =
    match ordinalChain with
    | [ ordinal ] when ordinal >= 0 && ordinal <= MaxOccurrenceSegment -> Some ordinal
    | [ parent; ordinal ] when
        parent >= 0
        && ordinal >= 0
        && ordinal <= MaxOccurrenceSegment
        && parent < MaxOccurrenceSegment
        ->
        // Pack in int64: a large parent (e.g. 0xFFFE) would wrap ((parent + 1) <<< 16) negative in
        // int32 and a negative key slips past the <= MaxOccurrenceKey bound, failing OPEN.
        let key = ((int64 parent + 1L) <<< 16) ||| int64 ordinal

        if key <= int64 MaxOccurrenceKey then
            Some(int key)
        else
            None
    | _ -> None

/// Unpacks an occurrence key produced by tryEncodeOccurrenceKey back into its
/// root-first ordinal chain.
let decodeOccurrenceKey (key: int) : int list =
    if key < 0 then
        invalidArg (nameof key) $"occurrence key must be non-negative, got %d{key}"
    elif key <= MaxOccurrenceSegment then
        [ key ]
    else
        [ (key >>> 16) - 1; key &&& MaxOccurrenceSegment ]

// ---------------------------------------------------------------------------
// Blob helpers
// ---------------------------------------------------------------------------

let private invalidData (blobName: string) (offset: int) =
    raise (InvalidDataException $"invalid EnC %s{blobName} blob: unexpected data at offset %d{offset}")

// Absent CDI rows arrive as null at runtime even though the parameter is non-null in the
// nullness model, so guard with box (FS3261-safe) rather than dropping the check.
let private isEmpty (blob: byte[]) = isNull (box blob) || blob.Length = 0

// ---------------------------------------------------------------------------
// EnC Local Slot Map
// Format (EditAndContinueMethodDebugInformation.cs, SerializeLocalSlots lines 145-191,
// UncompressSlotMap lines 92-143): optional baseline record [0xFF, compressed(-baseline)],
// then one record per slot: 0x00 for a temp, otherwise a leading byte with bits 0-5 =
// kind + 1 and bit 7 = has-ordinal flag, followed by compressed(syntaxOffset - baseline)
// and, when flagged, compressed(ordinal).
// ---------------------------------------------------------------------------

/// Serializes the EnC Local Slot Map blob for 'info', byte-for-byte as Roslyn's
/// SerializeLocalSlots. Returns the empty array when there are no slots (no CDI row
/// should be emitted then).
let serializeLocalSlots (info: EncMethodDebugInformation) : byte[] =
    match info.LocalSlots with
    | [] -> Array.empty
    | slots ->
        let builder = BlobBuilder()

        // The baseline is the most negative syntax offset, or -1 when none is negative
        // (Roslyn lines 147-160). Offsets are stored relative to it so the common
        // all-non-negative case costs no baseline record.
        let syntaxOffsetBaseline =
            (-1, slots)
            ||> List.fold (fun acc slot ->
                match slot with
                | EncLocalSlotInfo.Temp -> acc
                | EncLocalSlotInfo.Slot(_, syntaxOffset, _) -> min acc syntaxOffset)

        if syntaxOffsetBaseline <> -1 then
            builder.WriteByte SyntaxOffsetBaselineMarker
            builder.WriteCompressedInteger(-syntaxOffsetBaseline)

        for slot in slots do
            match slot with
            | EncLocalSlotInfo.Temp -> builder.WriteByte 0uy
            | EncLocalSlotInfo.Slot(kind, syntaxOffset, ordinal) ->
                if kind < 0 || kind > MaxSerializableLocalKind then
                    invalidArg (nameof info) $"local slot kind %d{kind} is outside the serializable range 0..%d{MaxSerializableLocalKind}"

                if ordinal < 0 then
                    invalidArg (nameof info) $"local slot ordinal must be non-negative, got %d{ordinal}"

                let hasOrdinal = ordinal > 0
                let b = byte (kind + 1) ||| (if hasOrdinal then 0x80uy else 0uy)
                builder.WriteByte b
                builder.WriteCompressedInteger(syntaxOffset - syntaxOffsetBaseline)

                if hasOrdinal then
                    builder.WriteCompressedInteger ordinal

        builder.ToArray()

/// Deserializes an EnC Local Slot Map blob, byte-for-byte as Roslyn's UncompressSlotMap.
/// An empty (or null) blob yields no slots.
let deserializeLocalSlots (blob: byte[]) : EncLocalSlotInfo list =
    if isEmpty blob then
        []
    else
        let handle = GCHandle.Alloc(blob, GCHandleType.Pinned)

        try
            let mutable reader =
                BlobReader(NativePtr.ofNativeInt<byte> (handle.AddrOfPinnedObject()), blob.Length)

            let slots = ResizeArray<EncLocalSlotInfo>()
            let mutable syntaxOffsetBaseline = -1

            try
                while reader.RemainingBytes > 0 do
                    let b = reader.ReadByte()

                    if b = SyntaxOffsetBaselineMarker then
                        syntaxOffsetBaseline <- -reader.ReadCompressedInteger()
                    elif b = 0uy then
                        slots.Add EncLocalSlotInfo.Temp
                    else
                        // Roslyn recovers the kind with mask 0x3F (line 126); bit 7 flags
                        // a trailing ordinal, bit 6 is unused by the writer.
                        let kind = int (b &&& 0x3Fuy) - 1
                        let hasOrdinal = b &&& 0x80uy <> 0uy
                        let syntaxOffset = reader.ReadCompressedInteger() + syntaxOffsetBaseline
                        let ordinal = if hasOrdinal then reader.ReadCompressedInteger() else 0
                        slots.Add(EncLocalSlotInfo.Slot(kind, syntaxOffset, ordinal))
            with :? BadImageFormatException ->
                invalidData "local slot map" reader.Offset

            List.ofSeq slots
        finally
            handle.Free()

// ---------------------------------------------------------------------------
// EnC Lambda and Closure Map
// Format (SerializeLambdaMap lines 261-302, UncompressLambdaMap lines 197-259):
// compressed(methodOrdinal + 1), compressed(-baseline), compressed(closureCount),
// closureCount * compressed(syntaxOffset - baseline), then until the blob ends:
// [compressed(syntaxOffset - baseline), compressed(closureOrdinal - MinClosureOrdinal)]
// per lambda.
// ---------------------------------------------------------------------------

/// Serializes the EnC Lambda and Closure Map blob for 'info', byte-for-byte as Roslyn's
/// SerializeLambdaMap. Returns the empty array when there are no lambdas and no closures
/// (Roslyn's MetadataWriter skips the CDI row in that case; note the method ordinal is
/// then not persisted and decodes back as UndefinedMethodOrdinal).
let serializeLambdaMap (info: EncMethodDebugInformation) : byte[] =
    match info.Closures, info.Lambdas with
    | [], [] -> Array.empty
    | closures, lambdas ->
        if info.MethodOrdinal < -1 then
            invalidArg (nameof info) $"method ordinal must be >= -1, got %d{info.MethodOrdinal}"

        let builder = BlobBuilder()
        builder.WriteCompressedInteger(info.MethodOrdinal + 1)

        // Negative offsets are rare (Roslyn: field/property initializers), so the
        // baseline is -1 unless a smaller offset exists (Roslyn lines 266-286).
        let syntaxOffsetBaseline =
            let closureMin = (-1, closures) ||> List.fold (fun acc c -> min acc c.SyntaxOffset)
            (closureMin, lambdas) ||> List.fold (fun acc l -> min acc l.SyntaxOffset)

        builder.WriteCompressedInteger(-syntaxOffsetBaseline)
        builder.WriteCompressedInteger closures.Length

        for closure in closures do
            builder.WriteCompressedInteger(closure.SyntaxOffset - syntaxOffsetBaseline)

        for lambda in lambdas do
            if
                lambda.ClosureOrdinal < MinClosureOrdinal
                || lambda.ClosureOrdinal >= closures.Length
            then
                invalidArg
                    (nameof info)
                    $"lambda closure ordinal %d{lambda.ClosureOrdinal} is outside [%d{MinClosureOrdinal}, %d{closures.Length})"

            builder.WriteCompressedInteger(lambda.SyntaxOffset - syntaxOffsetBaseline)
            builder.WriteCompressedInteger(lambda.ClosureOrdinal - MinClosureOrdinal)

        builder.ToArray()

/// Deserializes an EnC Lambda and Closure Map blob, byte-for-byte as Roslyn's
/// UncompressLambdaMap. An empty (or null) blob yields (UndefinedMethodOrdinal, [], []).
let deserializeLambdaMap (blob: byte[]) : int * EncClosureInfo list * EncLambdaInfo list =
    if isEmpty blob then
        UndefinedMethodOrdinal, [], []
    else
        let handle = GCHandle.Alloc(blob, GCHandleType.Pinned)

        try
            let mutable reader =
                BlobReader(NativePtr.ofNativeInt<byte> (handle.AddrOfPinnedObject()), blob.Length)

            let closures = ResizeArray<EncClosureInfo>()
            let lambdas = ResizeArray<EncLambdaInfo>()
            let mutable methodOrdinal = UndefinedMethodOrdinal

            try
                methodOrdinal <- reader.ReadCompressedInteger() - 1
                let syntaxOffsetBaseline = -reader.ReadCompressedInteger()
                let closureCount = reader.ReadCompressedInteger()

                for _ in 1..closureCount do
                    let syntaxOffset = reader.ReadCompressedInteger() + syntaxOffsetBaseline
                    closures.Add { SyntaxOffset = syntaxOffset }

                while reader.RemainingBytes > 0 do
                    let syntaxOffset = reader.ReadCompressedInteger() + syntaxOffsetBaseline
                    let closureOrdinal = reader.ReadCompressedInteger() + MinClosureOrdinal

                    if closureOrdinal >= closureCount then
                        invalidData "lambda map" reader.Offset

                    lambdas.Add
                        {
                            SyntaxOffset = syntaxOffset
                            ClosureOrdinal = closureOrdinal
                        }
            with :? BadImageFormatException ->
                invalidData "lambda map" reader.Offset

            methodOrdinal, List.ofSeq closures, List.ofSeq lambdas
        finally
            handle.Free()

// ---------------------------------------------------------------------------
// EnC State Machine State Map
// Format (SerializeStateMachineStates lines 364-381, UncompressStateMachineStates
// lines 309-362): compressed(count); when count > 0: compressed(-baseline) followed by
// count * [compressedSigned(stateNumber), compressed(syntaxOffset - baseline)], entries
// ordered by syntax offset.
// ---------------------------------------------------------------------------

/// Serializes the EnC State Machine State Map blob for 'info', byte-for-byte as
/// Roslyn's SerializeStateMachineStates: entries are sorted by syntax offset (stably,
/// preserving relative order of equal offsets, which encodes the per-offset relative
/// ordinal). Returns the empty array when there are no states (no CDI row then).
let serializeStateMachineStates (info: EncMethodDebugInformation) : byte[] =
    match info.StateMachineStates with
    | [] -> Array.empty
    | states ->
        let builder = BlobBuilder()
        builder.WriteCompressedInteger states.Length

        // Unlike the other two blobs the baseline here is min(minOffset, 0)
        // (Roslyn line 372).
        let syntaxOffsetBaseline =
            min (states |> List.map (fun s -> s.SyntaxOffset) |> List.min) 0

        builder.WriteCompressedInteger(-syntaxOffsetBaseline)

        // Roslyn's reader rejects more than 256 entries sharing one syntax offset
        // (relative ordinal must fit a byte, line 344); fail closed at write time.
        for _, group in states |> List.groupBy (fun s -> s.SyntaxOffset) do
            if group.Length > 256 then
                invalidArg (nameof info) $"more than 256 state machine states share syntax offset %d{group.Head.SyntaxOffset}"

        for state in states |> List.sortBy (fun s -> s.SyntaxOffset) do
            builder.WriteCompressedSignedInteger state.StateNumber
            builder.WriteCompressedInteger(state.SyntaxOffset - syntaxOffsetBaseline)

        builder.ToArray()

/// Deserializes an EnC State Machine State Map blob, byte-for-byte as Roslyn's
/// UncompressStateMachineStates (including the ordered-by-offset and <= 256-per-offset
/// validations). An empty (or null) blob yields no states.
let deserializeStateMachineStates (blob: byte[]) : EncStateMachineStateInfo list =
    if isEmpty blob then
        []
    else
        let handle = GCHandle.Alloc(blob, GCHandleType.Pinned)

        try
            let mutable reader =
                BlobReader(NativePtr.ofNativeInt<byte> (handle.AddrOfPinnedObject()), blob.Length)

            let states = ResizeArray<EncStateMachineStateInfo>()

            try
                let count = reader.ReadCompressedInteger()

                if count > 0 then
                    let syntaxOffsetBaseline = -reader.ReadCompressedInteger()
                    let mutable lastSyntaxOffset = Int32.MinValue
                    let mutable relativeOrdinal = 0

                    for _ in 1..count do
                        let stateNumber = reader.ReadCompressedSignedInteger()
                        let syntaxOffset = syntaxOffsetBaseline + reader.ReadCompressedInteger()

                        // Entries must be ordered by syntax offset and at most 256 may
                        // share one offset (Roslyn lines 336-347).
                        if syntaxOffset < lastSyntaxOffset then
                            invalidData "state machine state map" reader.Offset

                        relativeOrdinal <-
                            if syntaxOffset = lastSyntaxOffset then
                                relativeOrdinal + 1
                            else
                                0

                        if relativeOrdinal > 255 then
                            invalidData "state machine state map" reader.Offset

                        states.Add
                            {
                                StateNumber = stateNumber
                                SyntaxOffset = syntaxOffset
                            }

                        lastSyntaxOffset <- syntaxOffset
            with :? BadImageFormatException ->
                invalidData "state machine state map" reader.Offset

            List.ofSeq states
        finally
            handle.Free()

/// Deserializes EnC method debug information from the three blobs (any of which may be
/// null or empty). Mirrors Roslyn's EditAndContinueMethodDebugInformation.Create.
let deserialize (slotMapBlob: byte[]) (lambdaMapBlob: byte[]) (stateMachineStateMapBlob: byte[]) : EncMethodDebugInformation =
    let methodOrdinal, closures, lambdas = deserializeLambdaMap lambdaMapBlob

    {
        MethodOrdinal = methodOrdinal
        LocalSlots = deserializeLocalSlots slotMapBlob
        Closures = closures
        Lambdas = lambdas
        StateMachineStates = deserializeStateMachineStates stateMachineStateMapBlob
    }

/// Decodes every method-level EnC CustomDebugInformation row of a portable PDB image into
/// per-method EnC debug information, keyed by MethodDef token (0x06xxxxxx). The CDI parent
/// of the EnC rows is always a MethodDef handle, so token keying is unambiguous.
/// Fail safe: a null/empty or non-PDB image yields the empty map, and a method whose
/// blobs do not decode is omitted rather than guessed.
let readEncMethodDebugInfoFromPortablePdb (pdbBytes: byte[]) : Map<int, EncMethodDebugInformation> =
    if isEmpty pdbBytes then
        Map.empty
    else
        try
            use provider =
                MetadataReaderProvider.FromPortablePdbImage(ImmutableArray.CreateRange pdbBytes)

            let reader = provider.GetMetadataReader()

            let slotMapBlobs = Dictionary<int, byte[]>()
            let lambdaMapBlobs = Dictionary<int, byte[]>()
            let stateMapBlobs = Dictionary<int, byte[]>()

            for cdiHandle in reader.CustomDebugInformation do
                let cdi = reader.GetCustomDebugInformation cdiHandle

                if cdi.Parent.Kind = HandleKind.MethodDefinition then
                    let methodToken = MetadataTokens.GetToken cdi.Parent
                    let kind = reader.GetGuid cdi.Kind

                    if kind = PortableCustomDebugInfoKinds.encLocalSlotMap then
                        slotMapBlobs[methodToken] <- reader.GetBlobBytes cdi.Value
                    elif kind = PortableCustomDebugInfoKinds.encLambdaAndClosureMap then
                        lambdaMapBlobs[methodToken] <- reader.GetBlobBytes cdi.Value
                    elif kind = PortableCustomDebugInfoKinds.encStateMachineStateMap then
                        stateMapBlobs[methodToken] <- reader.GetBlobBytes cdi.Value

            let methodTokens =
                Seq.concat [ slotMapBlobs.Keys :> seq<int>; lambdaMapBlobs.Keys; stateMapBlobs.Keys ]
                |> Seq.distinct

            let tryBlob (blobs: Dictionary<int, byte[]>) token =
                match blobs.TryGetValue token with
                | true, blob -> blob
                | _ -> Array.empty

            (Map.empty, methodTokens)
            ||> Seq.fold (fun acc token ->
                try
                    let info =
                        deserialize (tryBlob slotMapBlobs token) (tryBlob lambdaMapBlobs token) (tryBlob stateMapBlobs token)

                    Map.add token info acc
                with :? InvalidDataException ->
                    // Fail closed per method: an undecodable blob never yields a partial
                    // (and so potentially mismatched) map for its method.
                    acc)
        with :? BadImageFormatException ->
            // Not a portable PDB image (or a corrupted one): callers still get an empty
            // map instead of a crash.
            Map.empty
