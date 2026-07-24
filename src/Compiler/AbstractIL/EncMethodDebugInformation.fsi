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

/// Portable-PDB CustomDebugInformation kind GUIDs for the EnC blobs, copied verbatim
/// from roslyn/src/Dependencies/CodeAnalysis.Debugging/PortableCustomDebugInfoKinds.cs.
[<RequireQualifiedAccess>]
module PortableCustomDebugInfoKinds =

    /// EnC Local Slot Map CDI kind.
    val encLocalSlotMap: System.Guid

    /// EnC Lambda and Closure Map CDI kind.
    val encLambdaAndClosureMap: System.Guid

    /// EnC State Machine State Map CDI kind.
    val encStateMachineStateMap: System.Guid

    /// F#-owned hot reload synthesized-name snapshot CDI kind.
    val fsharpSynthesizedNameSnapshot: System.Guid

/// Closure ordinal of a lambda that is lowered to a static (non-capturing) method.
/// Mirrors Roslyn's LambdaDebugInfo.StaticClosureOrdinal.
[<Literal>]
val StaticClosureOrdinal: int = -1

/// Closure ordinal of a lambda closed over the 'this' pointer only.
/// Mirrors Roslyn's LambdaDebugInfo.ThisOnlyClosureOrdinal.
[<Literal>]
val ThisOnlyClosureOrdinal: int = -2

/// Smallest valid closure ordinal. Mirrors Roslyn's LambdaDebugInfo.MinClosureOrdinal.
[<Literal>]
val MinClosureOrdinal: int = -2

/// Method ordinal of a method that has no lambda map (an empty blob decodes to this).
/// Mirrors Roslyn's DebugId.UndefinedOrdinal.
[<Literal>]
val UndefinedMethodOrdinal: int = -1

/// Largest synthesized-local kind serializable in the slot map: the kind is stored as
/// (kind + 1) in bits 0-5 of the leading byte (bit 6 is unused, bit 7 flags a trailing ordinal), and
/// Roslyn's reader recovers it with mask 0x3F, so only kinds 0..0x3E round-trip.
[<Literal>]
val MaxSerializableLocalKind: int = 0x3E

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
    static member Empty: EncMethodDebugInformation

/// Packs an ordinal chain (root-first enclosing ordinals, ending with the innermost
/// ordinal) into a deterministic int suitable for a "syntax offset" blob slot. Fails
/// closed (None) past the limits: chains deeper than 2, ordinals > 0xFFFF, or keys
/// exceeding the compressed-integer budget.
val tryEncodeOccurrenceKey: ordinalChain: int list -> int option

/// Unpacks an occurrence key produced by tryEncodeOccurrenceKey back into its
/// root-first ordinal chain.
val decodeOccurrenceKey: key: int -> int list

/// Serializes an allocation-ordered synthesized-name snapshot into the F#-owned module
/// CDI blob. An empty snapshot returns an empty blob so no CDI row needs to be emitted.
val serializeSynthesizedNameSnapshot: snapshot: seq<struct (string * string[])> -> byte[]

/// Deserializes the F#-owned synthesized-name snapshot CDI blob. Bucket order in the
/// blob is deterministic only; each bucket array is returned exactly in recorded slot order.
val deserializeSynthesizedNameSnapshot: blob: byte[] -> Map<string, string[]>

/// Creates the module-level CustomDebugInformation row for the allocation-ordered
/// synthesized-name snapshot. Empty snapshots emit no row.
val computeSynthesizedNameSnapshotCustomDebugInfoRows:
    snapshot: seq<struct (string * string[])> -> FSharp.Compiler.AbstractIL.ILPdbWriter.PdbModuleCustomDebugInfo list

/// Serializes the EnC Local Slot Map blob for 'info', byte-for-byte as Roslyn's
/// SerializeLocalSlots. Returns the empty array when there are no slots (no CDI row
/// should be emitted then).
val serializeLocalSlots: info: EncMethodDebugInformation -> byte[]

/// Deserializes an EnC Local Slot Map blob, byte-for-byte as Roslyn's UncompressSlotMap.
/// An empty (or null) blob yields no slots.
val deserializeLocalSlots: blob: byte[] -> EncLocalSlotInfo list

/// Serializes the EnC Lambda and Closure Map blob for 'info', byte-for-byte as Roslyn's
/// SerializeLambdaMap. Returns the empty array when there are no lambdas and no closures
/// (Roslyn's MetadataWriter skips the CDI row in that case; note the method ordinal is
/// then not persisted and decodes back as UndefinedMethodOrdinal).
val serializeLambdaMap: info: EncMethodDebugInformation -> byte[]

/// Deserializes an EnC Lambda and Closure Map blob, byte-for-byte as Roslyn's
/// UncompressLambdaMap. An empty (or null) blob yields (UndefinedMethodOrdinal, [], []).
val deserializeLambdaMap: blob: byte[] -> int * EncClosureInfo list * EncLambdaInfo list

/// Serializes the EnC State Machine State Map blob for 'info', byte-for-byte as
/// Roslyn's SerializeStateMachineStates: entries are sorted by syntax offset (stably,
/// preserving relative order of equal offsets, which encodes the per-offset relative
/// ordinal). Returns the empty array when there are no states (no CDI row then).
val serializeStateMachineStates: info: EncMethodDebugInformation -> byte[]

/// Deserializes an EnC State Machine State Map blob, byte-for-byte as Roslyn's
/// UncompressStateMachineStates (including the ordered-by-offset and <= 256-per-offset
/// validations). An empty (or null) blob yields no states.
val deserializeStateMachineStates: blob: byte[] -> EncStateMachineStateInfo list

/// Deserializes EnC method debug information from the three blobs (any of which may be
/// null or empty). Mirrors Roslyn's EditAndContinueMethodDebugInformation.Create.
val deserialize:
    slotMapBlob: byte[] -> lambdaMapBlob: byte[] -> stateMachineStateMapBlob: byte[] -> EncMethodDebugInformation

/// Decodes every method-level EnC CustomDebugInformation row of a portable PDB image into
/// per-method EnC debug information, keyed by MethodDef token (0x06xxxxxx). The CDI parent
/// of the EnC rows is always a MethodDef handle, so token keying is unambiguous.
/// Fail safe: a null/empty or non-PDB image yields the empty map, and a method whose
/// blobs do not decode is omitted rather than guessed.
val readEncMethodDebugInfoFromPortablePdb: pdbBytes: byte[] -> Map<int, EncMethodDebugInformation>

/// Reads the F#-owned allocation-ordered synthesized-name snapshot from a portable PDB.
/// None means either the record is absent or invalid; callers must fall back to IL
/// reconstruction rather than trusting a partial layout.
val readSynthesizedNameSnapshotFromPortablePdb: pdbBytes: byte[] -> Map<string, string[]> option
