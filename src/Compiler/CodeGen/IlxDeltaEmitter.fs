module internal FSharp.Compiler.IlxDeltaEmitter

open System
open System.Collections.Generic
open System.Collections.Immutable
open System.IO
open System.Linq
open System.Reflection.Metadata
open System.Reflection.Metadata.Ecma335
open System.Reflection
open System.Reflection.Emit
open System.Reflection.PortableExecutable
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.BinaryConstants
open FSharp.Compiler.AbstractIL.ILDeltaHandles
open FSharp.Compiler.AbstractIL.ILPdbWriter
open FSharp.Compiler.EditAndContinue
open FSharp.Compiler.HotReload
open FSharp.Compiler.HotReload.SymbolChanges
open FSharp.Compiler.HotReload.SymbolMatcher
open FSharp.Compiler.HotReloadBaseline
open FSharp.Compiler.HotReloadPdb
open FSharp.Compiler.AbstractIL.IlxDeltaStreams
open FSharp.Compiler.CodeGen.FSharpDefinitionIndex
open FSharp.Compiler.GeneratedNames
open FSharp.Compiler.SynthesizedTypeMaps
open FSharp.Compiler.Syntax.PrettyNaming
open FSharp.Compiler.TypedTreeDiff
open Internal.Utilities
open FSharp.Compiler.EnvironmentHelpers

module MetadataWriter = FSharp.Compiler.AbstractIL.FSharpDeltaMetadataWriter

open MetadataWriter
open FSharp.Compiler.AbstractIL.DeltaMetadataTables
open FSharp.Compiler.AbstractIL.DeltaMetadataTypes

exception HotReloadUnsupportedEditException of string

module ILWriter = FSharp.Compiler.AbstractIL.ILBinaryWriter

let private normalizeGeneratedFieldName (name: string) =
    match name.IndexOf('@') with
    | -1 -> name
    | idx when idx > 0 -> name.Substring(0, idx)
    | _ -> name

/// Returns a synthesized-type match only when structural matching produced one
/// unambiguous candidate; recorded snapshots never justify choosing an arbitrary row.
let internal tryGetUniqueSynthesizedTypeMatch (matches: 'T array) =
    match matches with
    | [| single |] -> Some single
    | _ -> None

/// Converts an SRM EntityHandle to our TypeDefOrRef type for EventType fields
let private entityHandleToTypeDefOrRef (handle: EntityHandle) : TypeDefOrRef =
    let rowId = MetadataTokens.GetRowNumber handle

    match handle.Kind with
    | HandleKind.TypeDefinition -> TDR_TypeDef(TypeDefHandle rowId)
    | HandleKind.TypeReference -> TDR_TypeRef(TypeRefHandle rowId)
    | HandleKind.TypeSpecification -> TDR_TypeSpec(TypeSpecHandle rowId)
    | _ -> TDR_TypeDef(TypeDefHandle 0) // Nil handle maps to TypeDef 0

/// Converts SRM ExceptionRegion to our IlExceptionRegion type
let private convertExceptionRegions (regions: ImmutableArray<ExceptionRegion>) : IlExceptionRegion[] =
    if regions.IsDefaultOrEmpty then
        [||]
    else
        regions.ToArray()
        |> Array.map (fun region ->
            let kind =
                match region.Kind with
                | ExceptionRegionKind.Catch -> IlExceptionRegionKind.Catch
                | ExceptionRegionKind.Filter -> IlExceptionRegionKind.Filter
                | ExceptionRegionKind.Finally -> IlExceptionRegionKind.Finally
                | ExceptionRegionKind.Fault -> IlExceptionRegionKind.Fault
                | _ -> IlExceptionRegionKind.Catch

            let catchToken =
                if region.CatchType.IsNil then
                    0
                else
                    MetadataTokens.GetToken(region.CatchType)

            {
                IlExceptionRegion.Kind = kind
                TryOffset = region.TryOffset
                TryLength = region.TryLength
                HandlerOffset = region.HandlerOffset
                HandlerLength = region.HandlerLength
                CatchTypeToken = catchToken
                FilterOffset = region.FilterOffset
            })

/// Represents the emitted artifacts for a hot reload delta.
/// This is the primary output from IlxDeltaEmitter, containing all deltas needed
/// for MetadataUpdater.ApplyUpdate.
type IlxDelta =
    {
        Metadata: byte[]
        IL: byte[]
        Pdb: byte[] option
        /// EncLog entries using TableName from BinaryConstants for type safety
        EncLog: (TableName * int * EditAndContinueOperation) array
        /// EncMap entries using TableName from BinaryConstants for type safety
        EncMap: (TableName * int) array
        UpdatedTypeTokens: int list
        UpdatedMethodTokens: int list
        /// Runtime capabilities the host must verify before applying this delta.
        RequiredCapabilities: string list
        AddedOrChangedMethods: HotReloadBaseline.AddedOrChangedMethodInfo list
        MethodBodies: MethodBodyUpdate list
        StandaloneSignatures: StandaloneSignatureUpdate list
        GenerationId: Guid
        BaseGenerationId: Guid
        UserStringUpdates: (int * int * string) list
        MethodDefinitionRows: MethodDefinitionRowInfo list
        UpdatedBaseline: FSharpEmitBaseline option
        /// Per-document line updates for methods whose code MOVED without changing (Roslyn
        /// SequencePointUpdates). The debugger rebinds these methods' sequence points
        /// without any metadata/IL being applied for them.
        SequencePointUpdates: FSharp.Compiler.CodeAnalysis.FSharpSequencePointUpdates list
        /// The next committed sequence-point view, keyed by baseline/delta MethodDef token —
        /// the fresh compile's sequence points for every matched method. None when the
        /// analysis was unavailable (no baseline PDB or no fresh PDB). Hosts/sessions replace
        /// FSharpEmitBaseline.SequencePointSnapshots with this map when the update is committed;
        /// for deltas that chain a baseline this is already folded into UpdatedBaseline.
        ChainedSequencePoints: Map<int, ActiveStatementAnalysis.MethodSequencePoints> option
        /// Per-statement remap results for the host-supplied active statements (Roslyn
        /// ManagedHotReloadUpdate.ActiveStatements). Populated by the language service after a
        /// successful emit; the raw emitter leaves it empty.
        ActiveStatementUpdates: FSharp.Compiler.CodeAnalysis.FSharpActiveStatementRemapResult list
    }

/// Bytes and token mappings produced by the same whole-module write that created a fresh output
/// assembly. Callers pass None when no such write is available, preserving the legacy emitter
/// re-serialization path.
type HotReloadEmittedArtifacts =
    {
        AssemblyBytes: byte[]
        PdbBytes: byte[] option
        TokenMappings: ILWriter.ILTokenMappings
    }

/// Request payload used when producing a delta.
type IlxDeltaRequest =
    {
        Baseline: FSharpEmitBaseline
        UpdatedTypes: string list
        UpdatedMethods: MethodDefinitionKey list
        UpdatedAccessors: AccessorUpdate list
        Module: ILModuleDef
        SymbolChanges: FSharpSymbolChanges option
        CurrentGeneration: int
        PreviousGenerationId: Guid option
        SynthesizedNames: FSharpSynthesizedTypeMaps option
        EmittedArtifacts: HotReloadEmittedArtifacts option
    }

type private MethodMetadataInfo = MethodAttributes * MethodImplAttributes * string * byte[] * StringOffset option * BlobOffset option

[<RequireQualifiedAccess>]
type internal EntityTokenRemapKind =
    | TypeDef
    | FieldDef
    | MethodDef
    | MemberRef
    | MethodSpec
    | TypeRef
    | TypeSpec
    | Event
    | Property
    | AssemblyRef
    | Passthrough

let internal classifyEntityTokenRemapKind (token: int) : EntityTokenRemapKind =
    match token &&& 0xFF000000 with
    | 0x02000000 -> EntityTokenRemapKind.TypeDef
    | 0x04000000 -> EntityTokenRemapKind.FieldDef
    | 0x06000000 -> EntityTokenRemapKind.MethodDef
    | 0x0A000000 -> EntityTokenRemapKind.MemberRef
    | 0x2B000000 -> EntityTokenRemapKind.MethodSpec
    | 0x01000000 -> EntityTokenRemapKind.TypeRef
    | 0x1B000000 -> EntityTokenRemapKind.TypeSpec
    | 0x14000000 -> EntityTokenRemapKind.Event
    | 0x17000000 -> EntityTokenRemapKind.Property
    | 0x23000000 -> EntityTokenRemapKind.AssemblyRef
    // Existing baseline tables that can legitimately appear in IL but do not participate
    // in delta remapping. Keep these explicit so new table tags fail closed by default.
    | 0x00000000
    | 0x11000000
    | 0x1A000000 -> EntityTokenRemapKind.Passthrough
    | tableTag ->
        raise (
            HotReloadUnsupportedEditException(
                sprintf
                    "Unsupported metadata token table 0x%02X in method-body remap (token=0x%08X). Please rebuild."
                    (tableTag >>> 24)
                    token
            )
        )

/// Keeps synthesized baseline aliases that are either unpaired or already paired with the
/// current fresh type. This makes the fresh-to-baseline TypeDef relation injective while still
/// allowing a repeated lookup of the same pairing during the emitter's recursive walks.
let internal filterAvailableBaselineTypeMatches
    (newTypeNameByBaseline: Dictionary<string, string>)
    (newFullName: string)
    (matches: (string * 'Token)[])
    =
    matches
    |> Array.filter (fun (matchedName, _) ->
        match newTypeNameByBaseline.TryGetValue matchedName with
        | true, existingNewName -> String.Equals(existingNewName, newFullName, StringComparison.Ordinal)
        | false, _ -> true)

/// Enables baseline-alias recovery only when a complete recorded name snapshot accompanies a
/// whole-module in-process emit. Other emit paths keep their established fail-closed mapping.
let internal shouldFilterSynthesizedBaselineAliases usesRecordedSnapshot hasWholeModuleArtifacts =
    usesRecordedSnapshot && hasWholeModuleArtifacts

/// Helper that produces an empty delta payload.
let private emptyDelta: IlxDelta =
    {
        Metadata = Array.empty
        IL = Array.empty
        Pdb = None
        EncLog = Array.empty
        EncMap = Array.empty
        UpdatedTypeTokens = []
        UpdatedMethodTokens = []
        RequiredCapabilities = []
        AddedOrChangedMethods = []
        MethodBodies = []
        StandaloneSignatures = []
        GenerationId = Guid.Empty
        BaseGenerationId = Guid.Empty
        UserStringUpdates = []
        MethodDefinitionRows = []
        UpdatedBaseline = None
        SequencePointUpdates = []
        ChainedSequencePoints = None
        ActiveStatementUpdates = []
    }

let private defaultWriterOptions (ilg: ILGlobals) (checksumAlgorithm: HashAlgorithm) : ILWriter.options =
    // ILBinaryWriter insists on having an output path even when we emit to memory. Generate a
    // unique, throwaway file name per invocation so parallel sessions never collide, and so we
    // leave a breadcrumb for debugging when traces mention the synthetic assembly.
    let scratchDll =
        let fileName =
            sprintf "fsharp-hotreload-%s.dll" (System.Guid.NewGuid().ToString("N"))

        Path.Combine(Path.GetTempPath(), fileName)

    let scratchPdb =
        match Path.ChangeExtension(scratchDll, ".pdb") with
        | null -> scratchDll + ".pdb"
        | path -> path

    {
        ilg = ilg
        outfile = scratchDll
        pdbfile = Some scratchPdb
        portablePDB = true
        embeddedPDB = false
        embedAllSource = false
        embedSourceList = []
        allGivenSources = []
        sourceLink = ""
        checksumAlgorithm = checksumAlgorithm
        signer = None
        emitTailcalls = false
        deterministic = true
        dumpDebugInfo = false
        referenceAssemblyOnly = false
        referenceAssemblyAttribOpt = None
        referenceAssemblySignatureHash = None
        pathMap = PathMap.empty
        moduleCustomDebugInfoRows = []
        methodCustomDebugInfoRows = Map.empty
    }

let private opCodeLookup: Lazy<Dictionary<int, OpCode>> =
    lazy
        (let dict = Dictionary<int, OpCode>()

         for field in typeof<OpCodes>.GetFields(BindingFlags.Public ||| BindingFlags.Static) do
             let op = field.GetValue(null) :?> OpCode
             let value = int (uint16 op.Value)

             if not (dict.ContainsKey(value)) then
                 dict[value] <- op

         dict)

let private traceFlag name = lazy (isEnvVarTruthy name)

/// Trace flags for hot reload debugging - controlled via environment variables
let private traceUserStringUpdates = traceFlag "FSHARP_HOTRELOAD_TRACE_STRINGS"

let private traceSynthesizedMappings =
    traceFlag "FSHARP_HOTRELOAD_TRACE_SYNTHESIZED"

let private traceMethodUpdates = traceFlag "FSHARP_HOTRELOAD_TRACE_METHODS"
let private traceMetadata = traceFlag "FSHARP_HOTRELOAD_TRACE_METADATA"
let private traceHeapOffsets = traceFlag "FSHARP_HOTRELOAD_TRACE_HEAP_OFFSETS"

type private PositionalTypeInfo =
    {
        FullName: string
        EnclosingFullName: string
        NormalizedBasicName: string
        Ordinal: int list
        Shape: SynthesizedTypeShape
    }

let internal tryFindSynthesizedTypeShapeMismatch (baseline: SynthesizedTypeShape) (fresh: SynthesizedTypeShape) =
    if baseline.GenericArity <> fresh.GenericArity then
        Some($"generic arity changed baseline={baseline.GenericArity} fresh={fresh.GenericArity}")
    elif baseline.BaseType <> fresh.BaseType then
        Some($"base type changed baseline={baseline.BaseType} fresh={fresh.BaseType}")
    elif baseline.InterfaceTypes <> fresh.InterfaceTypes then
        Some($"interface set changed baseline={baseline.InterfaceTypes} fresh={fresh.InterfaceTypes}")
    elif baseline.FieldTypeNames <> fresh.FieldTypeNames then
        Some($"field type multiset changed baseline={baseline.FieldTypeNames} fresh={fresh.FieldTypeNames}")
    elif baseline.MethodNameAndArities <> fresh.MethodNameAndArities then
        Some($"method set changed baseline={baseline.MethodNameAndArities} fresh={fresh.MethodNameAndArities}")
    else
        None

/// Deduplicates method keys while preserving order
let private dedupeMethodKeys (keys: MethodDefinitionKey list) =
    let seen = HashSet<MethodDefinitionKey>(HashIdentity.Structural)

    keys
    |> List.fold (fun acc key -> if seen.Add key then key :: acc else acc) []
    |> List.rev

let private rewriteMethodBody (remapUserString: int -> int) (remapEntityToken: int -> int) (body: MethodBodyBlock) =
    let ilBytes = body.GetILBytes().ToArray()
    let rewritten = Array.copy ilBytes
    let referencedMethodSpecs = HashSet<int>()
    let mutable offset = 0
    let length = ilBytes.Length

    let advance count = offset <- offset + count

    while offset < length do
        let opcodeValue, size =
            let first = int ilBytes.[offset]

            if first = 0xFE then
                let second = int ilBytes.[offset + 1]
                ((0xFE00 ||| second), 2)
            else
                (first, 1)

        advance size

        let operandType =
            match opCodeLookup.Value.TryGetValue opcodeValue with
            | true, op -> op.OperandType
            | _ -> OperandType.InlineNone

        let operandStart = offset

        let inline readInt32 () =
            let value = BitConverter.ToInt32(ilBytes, operandStart)
            advance 4
            value

        let inline readInt16 () =
            let value = BitConverter.ToInt16(ilBytes, operandStart)
            advance 2
            value

        let inline readSByte () =
            let value = sbyte ilBytes.[operandStart]
            advance 1
            value

        let inline readByte () =
            let value = ilBytes.[operandStart]
            advance 1
            value

        match operandType with
        | OperandType.InlineNone -> ()
        | OperandType.ShortInlineI -> readSByte () |> ignore
        | OperandType.InlineI -> readInt32 () |> ignore
        | OperandType.InlineI8 -> advance 8
        | OperandType.ShortInlineR -> advance 4
        | OperandType.InlineR -> advance 8
        | OperandType.InlineBrTarget -> readInt32 () |> ignore
        | OperandType.ShortInlineBrTarget -> readSByte () |> ignore
        | OperandType.ShortInlineVar -> readByte () |> ignore
        | OperandType.InlineVar -> readInt16 () |> ignore
        | OperandType.InlineString ->
            let original = readInt32 ()
            let updated = remapUserString original
            let tokenBytes = BitConverter.GetBytes(updated: int)
            Buffer.BlockCopy(tokenBytes, 0, rewritten, operandStart, 4)
        | OperandType.InlineField
        | OperandType.InlineMethod
        | OperandType.InlineTok
        | OperandType.InlineType ->
            let original = readInt32 ()
            let updated = remapEntityToken original

            if original <> updated then
                let tokenBytes = BitConverter.GetBytes(updated: int)
                Buffer.BlockCopy(tokenBytes, 0, rewritten, operandStart, 4)

            if (updated &&& 0xFF000000) = 0x2B000000 then
                referencedMethodSpecs.Add(updated) |> ignore
        | OperandType.InlineSig ->
            // A calli operand is a StandAloneSig token. Passing the fresh-compile row id
            // through would bind the instruction to an unrelated baseline signature. Until
            // call-site signatures participate in delta row remapping, reject the edit.
            raise (
                HotReloadUnsupportedEditException(
                    "Updated method contains a calli signature that cannot yet be remapped safely; please rebuild."
                )
            )
        | OperandType.InlineSwitch ->
            let count = readInt32 ()
            advance (count * 4)
        | OperandType.InlinePhi ->
            let count = int (readByte ())
            advance (count * 2)
        | _ -> ()

    rewritten, (referencedMethodSpecs |> Seq.toList)

/// Remaps a TypeDefOrRefOrSpec coded index (ECMA-335 II.23.2.8) using the entity-token remapper.
/// TypeSpec coded indexes route through the content-validated TypeSpec remap, which reuses a
/// matching baseline row or appends a new TypeSpec row to the delta.
let private remapTypeDefOrRefCodedIndexWith (remapEntityToken: int -> int) (coded: int) : int =
    let rowId = coded >>> 2

    if rowId = 0 then
        coded
    else
        match coded &&& 0x3 with
        | 0 -> (((remapEntityToken (0x02000000 ||| rowId)) &&& 0x00FFFFFF) <<< 2)
        | 1 -> (((remapEntityToken (0x01000000 ||| rowId)) &&& 0x00FFFFFF) <<< 2) ||| 1
        | 2 -> (((remapEntityToken (0x1B000000 ||| rowId)) &&& 0x00FFFFFF) <<< 2) ||| 2
        | _ -> coded

/// Rewrites the TypeDefOrRefOrSpec coded indexes embedded in an ECMA-335 signature blob so the blob
/// can be stored in a delta against the baseline metadata tables.
///
/// Signature blobs captured from the in-memory recompile embed TypeDef/TypeRef row ids of that
/// compile. When the baseline tables have a different shape (for example SDK-built baselines carry
/// import-scope TypeRefs that the in-memory rewrite does not regenerate), copying the blob raw makes
/// its coded indexes resolve to arbitrary baseline rows - observed at runtime as
/// "The generic type 'IntrinsicOperators' was used with the wrong number of generic arguments".
/// Walks the signature grammar (II.23.2) and remaps every embedded coded index; fails closed on
/// unknown element types.
let private remapSignatureBlobCore (isTypeSpecBlob: bool) (remapTypeDefOrRefCodedIndex: int -> int) (signature: byte[]) : byte[] =
    if isNull (box signature) || signature.Length = 0 then
        signature
    else
        let builder = BlobBuilder()
        let mutable pos = 0

        let fail (message: string) : 'T =
            raise (
                HotReloadUnsupportedEditException(
                    sprintf "Unsupported signature blob during delta remap at offset %d: %s. Please rebuild." pos message
                )
            )

        let peek () =
            if pos >= signature.Length then
                fail "unexpected end of signature"
            else
                int signature.[pos]

        let readByteValue () =
            let value = peek ()
            pos <- pos + 1
            value

        let copyByte () =
            builder.WriteByte(byte (readByteValue ()))

        let compressedLength (first: int) =
            if first &&& 0x80 = 0 then
                1
            elif first &&& 0xC0 = 0x80 then
                2
            elif first &&& 0xE0 = 0xC0 then
                4
            else
                fail (sprintf "invalid compressed integer lead byte 0x%02X" first)

        let readCompressedUInt () =
            let first = readByteValue ()

            match compressedLength first with
            | 1 -> first
            | 2 -> ((first &&& 0x3F) <<< 8) ||| readByteValue ()
            | _ ->
                let b2 = readByteValue ()
                let b3 = readByteValue ()
                let b4 = readByteValue ()
                ((first &&& 0x1F) <<< 24) ||| (b2 <<< 16) ||| (b3 <<< 8) ||| b4

        // Copies a compressed (possibly signed) integer without re-encoding.
        let copyCompressed () =
            let length = compressedLength (peek ())

            for _ in 1..length do
                copyByte ()

        let remapCodedIndex () =
            let coded = readCompressedUInt ()
            builder.WriteCompressedInteger(remapTypeDefOrRefCodedIndex coded)

        // CMOD_REQD/CMOD_OPT carry a TypeDefOrRefOrSpec coded index; PINNED is a bare prefix.
        let rec copyCustomModsAndConstraints () =
            if pos < signature.Length then
                match peek () with
                | 0x1F
                | 0x20 ->
                    copyByte ()
                    remapCodedIndex ()
                    copyCustomModsAndConstraints ()
                | 0x45 ->
                    copyByte ()
                    copyCustomModsAndConstraints ()
                | _ -> ()

        let rec copyType () =
            copyCustomModsAndConstraints ()

            match readByteValue () with
            // VOID and primitive element types, TYPEDBYREF, I, U, OBJECT
            | (0x01 | 0x02 | 0x03 | 0x04 | 0x05 | 0x06 | 0x07 | 0x08 | 0x09 | 0x0A | 0x0B | 0x0C | 0x0D | 0x0E | 0x16 | 0x18 | 0x19 | 0x1C) as code ->
                builder.WriteByte(byte code)
            // PTR, BYREF, SZARRAY wrap a single type
            | (0x0F | 0x10 | 0x1D) as code ->
                builder.WriteByte(byte code)
                copyType ()
            // VALUETYPE, CLASS carry a TypeDefOrRefOrSpec coded index
            | (0x11 | 0x12) as code ->
                builder.WriteByte(byte code)
                remapCodedIndex ()
            // VAR, MVAR carry a generic parameter ordinal
            | (0x13 | 0x1E) as code ->
                builder.WriteByte(byte code)
                copyCompressed ()
            // ARRAY: type, rank, sizes, lower bounds
            | 0x14 ->
                builder.WriteByte 0x14uy
                copyType ()
                copyCompressed () // rank
                let numSizes = readCompressedUInt ()
                builder.WriteCompressedInteger numSizes

                for _ in 1..numSizes do
                    copyCompressed ()

                let numLoBounds = readCompressedUInt ()
                builder.WriteCompressedInteger numLoBounds

                for _ in 1..numLoBounds do
                    copyCompressed () // signed; copied verbatim

            // GENERICINST: (CLASS | VALUETYPE), coded index, argument count, arguments
            | 0x15 ->
                builder.WriteByte 0x15uy

                match readByteValue () with
                | (0x11 | 0x12) as kind ->
                    builder.WriteByte(byte kind)
                    remapCodedIndex ()
                    let argCount = readCompressedUInt ()
                    builder.WriteCompressedInteger argCount

                    for _ in 1..argCount do
                        copyType ()
                | kind -> fail (sprintf "unexpected GENERICINST kind 0x%02X" kind)
            // FNPTR wraps a full method signature
            | 0x1B ->
                builder.WriteByte 0x1Buy
                copyMethodSignature ()
            | code -> fail (sprintf "unexpected signature element type 0x%02X" code)

        and copyMethodSignature () =
            let callingConvention = readByteValue ()
            builder.WriteByte(byte callingConvention)

            if callingConvention &&& 0x10 <> 0 then
                copyCompressed () // generic parameter count

            let paramCount = readCompressedUInt ()
            builder.WriteCompressedInteger paramCount
            copyType () // return type

            let mutable remaining = paramCount

            while remaining > 0 do
                if peek () = 0x41 then
                    copyByte () // SENTINEL (vararg) does not consume a parameter slot
                else
                    copyType ()
                    remaining <- remaining - 1

        (if isTypeSpecBlob then
             // TypeSpec blob (ECMA-335 II.23.2.14): a bare Type with no calling-convention
             // header.
             copyType ()
         else

             match peek () with
             // FieldSig
             | 0x06 ->
                 copyByte ()
                 copyType ()
             // LocalVarSig
             | 0x07 ->
                 copyByte ()
                 let count = readCompressedUInt ()
                 builder.WriteCompressedInteger count

                 for _ in 1..count do
                     copyType ()
             // MethodSpec instantiation (GENERICINST; not a valid method calling convention)
             | 0x0A ->
                 copyByte ()
                 let argCount = readCompressedUInt ()
                 builder.WriteCompressedInteger argCount

                 for _ in 1..argCount do
                     copyType ()
             // MethodDefSig/MethodRefSig/PropertySig
             | _ -> copyMethodSignature ())

        if pos <> signature.Length then
            fail "trailing bytes in signature"

        builder.ToArray()

/// Remaps the TypeDefOrRef coded indexes embedded in a method/field/local/property
/// signature blob (ECMA-335 II.23.2) from fresh-compile rows to baseline rows.
let internal remapSignatureBlobWith (remapTypeDefOrRefCodedIndex: int -> int) (signature: byte[]) : byte[] =
    remapSignatureBlobCore false remapTypeDefOrRefCodedIndex signature

/// Remaps the TypeDefOrRef coded indexes embedded in a TypeSpec signature blob
/// (ECMA-335 II.23.2.14: a bare Type, no calling-convention header).
/// HasCustomAttribute tag -> owning table number (ECMA-335 II.24.2.6 ordering), used to
/// project emitted CA row parents back to metadata tokens for baseline pairing/chaining.
let private hcaTagToTable =
    [|
        0x06
        0x04
        0x01
        0x02
        0x08
        0x09
        0x0A
        0x00
        0x0E
        0x17
        0x14
        0x11
        0x1A
        0x1B
        0x20
        0x23
        0x26
        0x27
        0x28
        0x2A
        0x2C
        0x2B
    |]

let internal hasCustomAttributeParentToken (parent: HasCustomAttribute) =
    (hcaTagToTable.[parent.CodedTag] <<< 24) ||| parent.RowId

let internal customAttributeConstructorToken (ctor: CustomAttributeType) =
    match ctor with
    | CAT_MethodDef handle -> 0x06000000 ||| handle.RowId
    | CAT_MemberRef handle -> 0x0A000000 ||| handle.RowId

let internal remapTypeSpecBlobWith (remapTypeDefOrRefCodedIndex: int -> int) (blob: byte[]) : byte[] =
    remapSignatureBlobCore true remapTypeDefOrRefCodedIndex blob

let private buildUpdatedTypeTokens
    (tryGetBaselineTypeName: string -> string)
    (baselineTypeTokens: Map<string, int>)
    (updatedTypes: string list)
    (symbolChangeTypeNames: string list)
    (resolvedMethods: (ILTypeDef list * ILTypeDef * ILMethodDef * MethodDefinitionKey) list)
    =
    let methodTypeNames =
        resolvedMethods
        |> List.map (fun (enclosing, typeDef, _, _) ->
            let typeRef = mkRefForNestedILTypeDef ILScopeRef.Local (enclosing, typeDef)
            tryGetBaselineTypeName typeRef.FullName)

    (updatedTypes @ symbolChangeTypeNames @ methodTypeNames)
    |> List.map tryGetBaselineTypeName
    |> List.distinct
    |> List.choose (fun typeName -> baselineTypeTokens |> Map.tryFind typeName)

/// Converts a delta MemberRef row's parent to the metadata token stored in the chained
/// baseline's MemberReferenceRows (used for content-validated passthrough next generation).
let private memberRefParentToken (parent: MemberRefParent) =
    match parent with
    | MRP_TypeDef handle -> 0x02000000 ||| handle.RowId
    | MRP_TypeRef handle -> 0x01000000 ||| handle.RowId
    | MRP_ModuleRef handle -> 0x1A000000 ||| handle.RowId
    | MRP_MethodDef handle -> 0x06000000 ||| handle.RowId
    | MRP_TypeSpec handle -> 0x1B000000 ||| handle.RowId

let private buildUpdatedBaseline
    (updatedBaselineCore: FSharpEmitBaseline)
    (parameterDefinitionRowsSnapshot: ParameterDefinitionRowInfo list)
    (memberReferenceRowList: MemberReferenceRowInfo list)
    (typeSpecificationRowList: TypeSpecificationRowInfo list)
    (customAttributeRowList: CustomAttributeRowInfo list)
    (propertyMapRowsSnapshot: PropertyMapRowInfo list)
    (eventMapRowsSnapshot: EventMapRowInfo list)
    (methodSemanticsRowsSnapshot: MethodSemanticsMetadataUpdate list)
    (methodTokenToKey: Dictionary<int, MethodDefinitionKey>)
    (addedMethodDeltaTokens: Dictionary<MethodDefinitionKey, int>)
    (addedFieldDeltaTokens: Dictionary<FieldDefinitionKey, int>)
    (addedPropertyDeltaTokens: Dictionary<PropertyDefinitionKey, int>)
    (addedEventDeltaTokens: Dictionary<EventDefinitionKey, int>)
    (addedTypeDeltaTokens: Dictionary<string, int>)
    (addedTypeShapes: Dictionary<string, SynthesizedTypeShape>)
    (addedTypeReferenceTokens: Dictionary<TypeReferenceKey, int>)
    (addedAssemblyReferenceTokens: Dictionary<AssemblyReferenceKey, int>)
    =
    let addPropertyMapEntry (entries: Map<string, int>) (row: PropertyMapRowInfo) =
        if row.IsAdded then
            entries |> Map.add row.DeclaringType row.RowId
        else
            entries

    let addEventMapEntry (entries: Map<string, int>) (row: EventMapRowInfo) =
        if row.IsAdded then
            entries |> Map.add row.DeclaringType row.RowId
        else
            entries

    let extendMethodSemanticsMap (entries: Map<MethodDefinitionKey, MethodSemanticsEntry list>) (row: MethodSemanticsMetadataUpdate) =
        if row.IsAdded then
            match methodTokenToKey.TryGetValue row.MethodToken with
            | true, methodKey ->
                let newEntry =
                    {
                        MethodSemanticsEntry.RowId = row.RowId
                        Attributes = row.Attributes
                        Association = row.AssociationInfo
                    }

                let updatedList =
                    match entries |> Map.tryFind methodKey with
                    | Some existing -> newEntry :: existing |> List.distinctBy (fun entry -> entry.RowId)
                    | None -> [ newEntry ]

                entries |> Map.add methodKey updatedList
            | _ -> entries
        else
            entries

    let updatedPropertyMapEntries =
        propertyMapRowsSnapshot
        |> List.fold addPropertyMapEntry updatedBaselineCore.PropertyMapEntries

    let updatedEventMapEntries =
        eventMapRowsSnapshot
        |> List.fold addEventMapEntry updatedBaselineCore.EventMapEntries

    let updatedMethodSemanticsEntries =
        methodSemanticsRowsSnapshot
        |> List.fold extendMethodSemanticsMap updatedBaselineCore.MethodSemanticsEntries

    let updatedMethodTokenMap =
        addedMethodDeltaTokens
        |> Seq.fold (fun acc (KeyValue(key, token)) -> acc |> Map.add key token) updatedBaselineCore.MethodTokens

    // Chain added field tokens into the next-generation baseline so a later generation can
    // resolve (and not re-add) the field, mirroring AddedOrChangedMethods chaining.
    let updatedFieldTokenMap =
        addedFieldDeltaTokens
        |> Seq.fold (fun acc (KeyValue(key, token)) -> acc |> Map.add key token) updatedBaselineCore.FieldTokens

    let updatedPropertyTokenMap =
        addedPropertyDeltaTokens
        |> Seq.fold (fun acc (KeyValue(key, token)) -> acc |> Map.add key token) updatedBaselineCore.PropertyTokens

    let updatedEventTokenMap =
        addedEventDeltaTokens
        |> Seq.fold (fun acc (KeyValue(key, token)) -> acc |> Map.add key token) updatedBaselineCore.EventTokens

    // Chain added TypeDef tokens (new closure classes) so later generations resolve the
    // type in place (e.g. a gen-3 body edit of a lambda added in gen 2) instead of
    // re-adding it.
    let updatedTypeTokenMap =
        addedTypeDeltaTokens
        |> Seq.fold (fun acc (KeyValue(fullName, token)) -> acc |> Map.add fullName token) updatedBaselineCore.TypeTokens

    let updatedSynthesizedTypeShapes =
        addedTypeShapes
        |> Seq.fold (fun acc (KeyValue(fullName, shape)) -> acc |> Map.add fullName shape) updatedBaselineCore.SynthesizedTypeShapes

    // Param rows added by a delta are part of the committed metadata baseline. Retain their
    // stable keys and row ids so a later edit of the same method reuses them instead of
    // appending duplicate Param rows with broken MethodDef.ParamList ranges.
    let updatedParameterHandles =
        parameterDefinitionRowsSnapshot
        |> List.fold
            (fun acc row ->
                acc
                |> Map.add
                    row.Key
                    {
                        ParameterDefinitionMetadataHandles.NameOffset = row.NameOffset
                        Name = row.Name
                        RowId = Some row.RowId
                    })
            updatedBaselineCore.MetadataHandles.ParameterHandles

    let updatedTypeReferenceTokens =
        addedTypeReferenceTokens
        |> Seq.fold (fun acc (KeyValue(key, token)) -> acc |> Map.add key token) updatedBaselineCore.TypeReferenceTokens

    let updatedAssemblyReferenceTokens =
        addedAssemblyReferenceTokens
        |> Seq.fold (fun acc (KeyValue(key, token)) -> acc |> Map.add key token) updatedBaselineCore.AssemblyReferenceTokens

    // Chain delta-appended MemberRef rows (already in baseline coordinates) so the next
    // generation's content-validated passthrough can recognize and reuse them.
    let updatedMemberReferenceRows =
        memberReferenceRowList
        |> List.fold
            (fun acc (row: MemberReferenceRowInfo) ->
                acc
                |> Map.add
                    row.RowId
                    {
                        HotReloadBaseline.BaselineMemberRefRow.Name = row.Name
                        ParentToken = memberRefParentToken row.Parent
                        Signature = row.Signature
                    })
            updatedBaselineCore.MemberReferenceRows

    // Chain delta-appended TypeSpec rows (signature blobs already in baseline
    // coordinates) so the next generation's content search recognizes and reuses
    // them instead of appending duplicates.
    let updatedTypeSpecSignatures =
        typeSpecificationRowList
        |> List.fold
            (fun acc (row: TypeSpecificationRowInfo) -> acc |> Map.add row.RowId row.Signature)
            updatedBaselineCore.TypeSpecSignatures

    // Chain emitted CustomAttribute rows (updates REPLACE the baseline entry, adds extend
    // the map) so the next generation's attribute pairing sees the current row contents.
    let updatedCustomAttributeRows =
        customAttributeRowList
        |> List.fold
            (fun acc (row: CustomAttributeRowInfo) ->
                acc
                |> Map.add
                    row.RowId
                    {
                        HotReloadBaseline.BaselineCustomAttributeRow.ParentToken = hasCustomAttributeParentToken row.Parent
                        ConstructorToken = customAttributeConstructorToken row.Constructor
                        Value = row.Value
                    })
            updatedBaselineCore.CustomAttributeRows

    { updatedBaselineCore with
        MetadataHandles =
            { updatedBaselineCore.MetadataHandles with
                ParameterHandles = updatedParameterHandles
            }
        TypeTokens = updatedTypeTokenMap
        SynthesizedTypeShapes = updatedSynthesizedTypeShapes
        MemberReferenceRows = updatedMemberReferenceRows
        TypeSpecSignatures = updatedTypeSpecSignatures
        CustomAttributeRows = updatedCustomAttributeRows
        MethodTokens = updatedMethodTokenMap
        FieldTokens = updatedFieldTokenMap
        PropertyTokens = updatedPropertyTokenMap
        EventTokens = updatedEventTokenMap
        PropertyMapEntries = updatedPropertyMapEntries
        EventMapEntries = updatedEventMapEntries
        MethodSemanticsEntries = updatedMethodSemanticsEntries
        TypeReferenceTokens = updatedTypeReferenceTokens
        AssemblyReferenceTokens = updatedAssemblyReferenceTokens
    }

let private tryBuildMethodUpdateInput
    (traceMethodUpdates: bool)
    (metadataReader: MetadataReader)
    (peReader: PEReader)
    (baselineMethodTokens: Map<MethodDefinitionKey, int>)
    (freshMethodTokenByBaseline: Dictionary<int, int>)
    (addedMethodTokens: Dictionary<MethodDefinitionKey, int>)
    (addedMethodDeltaTokens: Dictionary<MethodDefinitionKey, int>)
    (key: MethodDefinitionKey)
    : struct (MethodDefinitionKey * int * MethodDefinitionHandle * MethodDefinition * MethodBodyBlock) option =

    // readerToken addresses the FRESH compile's metadata reader; deltaToken is the
    // baseline-coordinate row the update is emitted at.
    let tryCreateInput (readerToken: int) (deltaToken: int) (isAddedMethod: bool) =
        let methodHandle = MetadataTokens.MethodDefinitionHandle readerToken

        if methodHandle.IsNil then
            None
        else
            let methodDef = metadataReader.GetMethodDefinition methodHandle

            if isAddedMethod && methodDef.RelativeVirtualAddress = 0 then
                // Bodiless added method. Abstract slots (added interfaces, abstract
                // members of added classes) and runtime-implemented members (a delegate's
                // .ctor/Invoke, ImplFlags CodeTypeMask = Runtime) legitimately have no IL
                // body: Roslyn emits their MethodDef rows with RVA 0 (C# 'new_interface'
                // and 'new_delegate' reference templates) and so do we — the input carries
                // a null MethodBodyBlock and the row's RVA column stays 0. Anything else
                // bodiless (extern/pinvoke would also need ImplMap rows) keeps failing
                // closed precisely.
                let isAbstract = methodDef.Attributes.HasFlag MethodAttributes.Abstract

                let isRuntimeImplemented =
                    methodDef.ImplAttributes &&& MethodImplAttributes.CodeTypeMask = MethodImplAttributes.Runtime

                if isAbstract || isRuntimeImplemented then
                    if traceMethodUpdates then
                        printfn "[fsharp-hotreload][method-add] %s::%s token=0x%08X (bodiless, RVA 0)" key.DeclaringType key.Name deltaToken

                    Some(struct (key, deltaToken, methodHandle, methodDef, Unchecked.defaultof<MethodBodyBlock>))
                else
                    raise (
                        HotReloadUnsupportedEditException(
                            $"Added method '{key.DeclaringType}::{key.Name}' has no IL body (extern); hot reload deltas cannot express extern added methods yet. Please rebuild."
                        )
                    )
            else

                let body = peReader.GetMethodBody(methodDef.RelativeVirtualAddress)

                if traceMethodUpdates then
                    if isAddedMethod then
                        printfn "[fsharp-hotreload][method-add] %s::%s token=0x%08X" key.DeclaringType key.Name deltaToken
                    else
                        printfn
                            "[fsharp-hotreload][method-update] %s::%s readerToken=0x%08X token=0x%08X"
                            key.DeclaringType
                            key.Name
                            readerToken
                            deltaToken

                Some(struct (key, deltaToken, methodHandle, methodDef, body))

    match baselineMethodTokens |> Map.tryFind key with
    | Some methodToken ->
        // The baseline token addresses the LOADED module's row space, which is not generally
        // valid in the fresh compile's reader: methods added by an earlier delta chain sit past
        // the original baseline tables, while the fresh compile lays them out at their natural
        // source positions, displacing later fresh rows. Read through the fresh token (recorded
        // by collectTypeMappings when it differs from the baseline token) and emit at the
        // baseline token.
        let readerToken =
            match freshMethodTokenByBaseline.TryGetValue methodToken with
            | true, freshToken -> freshToken
            | _ -> methodToken

        tryCreateInput readerToken methodToken false
    | None ->
        match addedMethodTokens.TryGetValue key, addedMethodDeltaTokens.TryGetValue key with
        | (true, methodToken), (true, deltaToken) -> tryCreateInput methodToken deltaToken true
        | _ -> None

let private buildReferenceRows
    (traceMetadata: bool)
    (typeReferenceRows: ResizeArray<TypeReferenceRowInfo>)
    (memberReferenceRows: ResizeArray<MemberReferenceRowInfo>)
    (assemblyReferenceRows: ResizeArray<AssemblyReferenceRowInfo>)
    (typeSpecificationRows: ResizeArray<TypeSpecificationRowInfo>)
    (methodSpecificationRowsSnapshot: MethodSpecificationRowInfo list)
    (customAttributeRowList: CustomAttributeRowInfo list)
    =
    let typeReferenceRowList =
        typeReferenceRows |> Seq.sortBy (fun row -> row.RowId) |> Seq.toList

    let memberReferenceRowList =
        memberReferenceRows |> Seq.sortBy (fun row -> row.RowId) |> Seq.toList

    let assemblyReferenceRowList =
        assemblyReferenceRows |> Seq.sortBy (fun row -> row.RowId) |> Seq.toList

    let typeSpecificationRowList =
        typeSpecificationRows |> Seq.sortBy (fun row -> row.RowId) |> Seq.toList

    if traceMetadata then
        printfn
            "[fsharp-hotreload][metadata] row-counts typeRef=%d memberRef=%d methodSpec=%d typeSpec=%d assemblyRef=%d customAttr=%d"
            typeReferenceRowList.Length
            memberReferenceRowList.Length
            methodSpecificationRowsSnapshot.Length
            typeSpecificationRowList.Length
            assemblyReferenceRowList.Length
            customAttributeRowList.Length

        for row in typeReferenceRowList do
            printfn
                "[fsharp-hotreload][metadata] typeref rowId=%d name=%s scope=%A row=%d"
                row.RowId
                row.Name
                row.ResolutionScope
                row.ResolutionScope.RowId

        for row in memberReferenceRowList do
            printfn
                "[fsharp-hotreload][metadata] memberref rowId=%d name=%s parent=%A row=%d"
                row.RowId
                row.Name
                row.Parent
                row.Parent.RowId

        for row in methodSpecificationRowsSnapshot do
            printfn
                "[fsharp-hotreload][metadata] methodspec rowId=%d methodTag=%d methodRow=%d"
                row.RowId
                row.Method.CodedTag
                row.Method.RowId

        for row in typeSpecificationRowList do
            printfn "[fsharp-hotreload][metadata] typespec rowId=%d blobLength=%d" row.RowId row.Signature.Length

        for row in assemblyReferenceRowList do
            printfn "[fsharp-hotreload][metadata] assemblyref rowId=%d name=%s" row.RowId row.Name

    typeReferenceRowList, memberReferenceRowList, assemblyReferenceRowList, typeSpecificationRowList

let private emitMetadataDelta
    (traceMetadata: bool)
    (moduleName: string)
    (baselineModuleNameOffset: StringOffset option)
    (currentGeneration: int)
    (encId: Guid)
    (encBaseId: Guid)
    (moduleMvid: Guid)
    (typeDefinitionRowsSnapshot: TypeDefinitionRowInfo list)
    (nestedClassRowsSnapshot: NestedClassRowInfo list)
    (interfaceImplRowsSnapshot: InterfaceImplRowInfo list)
    (methodImplRowsSnapshot: MethodImplRowInfo list)
    (constantRowsSnapshot: ConstantRowInfo list)
    (methodDefinitionRowsSnapshot: MethodDefinitionRowInfo list)
    (parameterDefinitionRowsSnapshot: ParameterDefinitionRowInfo list)
    (fieldDefinitionRowsSnapshot: FieldDefinitionRowInfo list)
    (typeReferenceRowList: TypeReferenceRowInfo list)
    (memberReferenceRowList: MemberReferenceRowInfo list)
    (methodSpecificationRowsSnapshot: MethodSpecificationRowInfo list)
    (typeSpecificationRowList: TypeSpecificationRowInfo list)
    (genericParamRowsSnapshot: GenericParamRowInfo list)
    (genericParamConstraintRowsSnapshot: GenericParamConstraintRowInfo list)
    (assemblyReferenceRowList: AssemblyReferenceRowInfo list)
    (propertyDefinitionRowsSnapshot: PropertyDefinitionRowInfo list)
    (eventDefinitionRowsSnapshot: EventDefinitionRowInfo list)
    (propertyMapRowsSnapshot: PropertyMapRowInfo list)
    (eventMapRowsSnapshot: EventMapRowInfo list)
    (methodSemanticsRowsSnapshot: MethodSemanticsMetadataUpdate list)
    (standaloneSignatures: StandaloneSignatureUpdate list)
    (customAttributeRowList: CustomAttributeRowInfo list)
    (userStringEntries: (int * int * string) list)
    (methodUpdates: MethodMetadataUpdate list)
    (baselineHeapOffsets: MetadataHeapOffsets)
    (baselineTableRowCounts: int[])
    =
    let writerStandaloneSignatures: FSharp.Compiler.AbstractIL.IlxDeltaStreams.StandaloneSignatureUpdate list =
        standaloneSignatures
        |> List.map (fun signature ->
            {
                RowId = signature.RowId
                Blob = signature.Blob
            })

    let metadataDelta =
        MetadataWriter.emitWithTypeDefinitions
            moduleName
            baselineModuleNameOffset
            currentGeneration
            encId
            encBaseId
            moduleMvid
            typeDefinitionRowsSnapshot
            nestedClassRowsSnapshot
            interfaceImplRowsSnapshot
            methodImplRowsSnapshot
            constantRowsSnapshot
            methodDefinitionRowsSnapshot
            parameterDefinitionRowsSnapshot
            fieldDefinitionRowsSnapshot
            typeReferenceRowList
            memberReferenceRowList
            methodSpecificationRowsSnapshot
            typeSpecificationRowList
            genericParamRowsSnapshot
            genericParamConstraintRowsSnapshot
            assemblyReferenceRowList
            propertyDefinitionRowsSnapshot
            eventDefinitionRowsSnapshot
            propertyMapRowsSnapshot
            eventMapRowsSnapshot
            methodSemanticsRowsSnapshot
            writerStandaloneSignatures
            customAttributeRowList
            userStringEntries
            methodUpdates
            baselineHeapOffsets
            baselineTableRowCounts

    if traceMetadata then
        let count idx = metadataDelta.TableRowCounts.[idx]

        printfn
            "[fsharp-hotreload][metadata] table-counts module=%d method=%d param=%d typeRef=%d memberRef=%d methodSpec=%d typeSpec=%d assemblyRef=%d customAttr=%d standAloneSig=%d"
            (count TableNames.Module.Index)
            (count TableNames.Method.Index)
            (count TableNames.Param.Index)
            (count TableNames.TypeRef.Index)
            (count TableNames.MemberRef.Index)
            (count TableNames.MethodSpec.Index)
            (count TableNames.TypeSpec.Index)
            (count TableNames.AssemblyRef.Index)
            (count TableNames.CustomAttribute.Index)
            (count TableNames.StandAloneSig.Index)

    metadataDelta

let private buildMethodUpdatesWithMetadata
    (orderedMethodInputs: struct (MethodDefinitionKey * int * MethodDefinitionHandle * MethodDefinition * MethodBodyBlock) list)
    (metadataReader: MetadataReader)
    (builder: IlDeltaStreamBuilder)
    (remapUserString: int -> int)
    (remapEntityToken: int -> int)
    =
    let methodUpdatesWithDefs =
        orderedMethodInputs
        |> List.map (fun struct (key, methodToken, methodHandle, methodDef, body) ->
            let bodyUpdate, referencedMethodSpecs =
                if isNull (box body) then
                    // Bodiless added method (abstract slot of an added interface/class or
                    // runtime-implemented delegate member): no IL chunk enters the delta;
                    // the MethodDef row's RVA column stays 0 (Roslyn template parity —
                    // AddMethodRow writes CodeOffset only when CodeLength > 0 and added
                    // rows have no baseline RVA to fall back to).
                    {
                        MethodToken = methodToken
                        LocalSignatureToken = 0
                        CodeOffset = 0
                        CodeLength = 0
                    },
                    []
                else
                    let ilBytes, referencedMethodSpecs =
                        rewriteMethodBody remapUserString remapEntityToken body

                    let localSigToken =
                        if body.LocalSignature.IsNil then
                            0
                        else
                            let standalone = metadataReader.GetStandaloneSignature body.LocalSignature
                            // Local signatures are emitted into the delta blob heap; remap the embedded
                            // TypeDefOrRef coded indexes from the fresh compile to baseline rows.
                            let signatureBytes =
                                metadataReader.GetBlobBytes standalone.Signature
                                |> remapSignatureBlobWith (remapTypeDefOrRefCodedIndexWith remapEntityToken)

                            builder.AddStandaloneSignature(signatureBytes)

                    builder.AddMethodBody(
                        methodToken,
                        localSigToken,
                        ilBytes,
                        body.MaxStack,
                        body.LocalVariablesInitialized,
                        convertExceptionRegions body.ExceptionRegions,
                        remapEntityToken
                    ),
                    referencedMethodSpecs

            // Convert SRM MethodDefinitionHandle to F# MethodDefHandle
            let methodHandleEntity: EntityHandle =
                MethodDefinitionHandle.op_Implicit methodHandle

            let methodRowId = MetadataTokens.GetRowNumber(methodHandleEntity)

            ({
                MethodKey = key
                MethodToken = methodToken
                MethodHandle = MethodDefHandle methodRowId
                Body = bodyUpdate
             },
             methodDef,
             referencedMethodSpecs))

    let methodMetadataLookup =
        let dict: Dictionary<MethodDefinitionKey, MethodMetadataInfo> =
            Dictionary(HashIdentity.Structural)

        for update, methodDef, _ in methodUpdatesWithDefs do
            let name = metadataReader.GetString methodDef.Name
            let signature = metadataReader.GetBlobBytes methodDef.Signature

            let nameOffset =
                if methodDef.Name.IsNil then
                    None
                else
                    Some(StringOffset(MetadataTokens.GetHeapOffset methodDef.Name))

            let signatureOffset =
                if methodDef.Signature.IsNil then
                    None
                else
                    Some(BlobOffset(MetadataTokens.GetHeapOffset methodDef.Signature))

            dict[update.MethodKey] <- (methodDef.Attributes, methodDef.ImplAttributes, name, signature, nameOffset, signatureOffset)

        dict

    methodUpdatesWithDefs, methodMetadataLookup

let private buildParameterDefinitionRowsSnapshot
    (parameterDefinitionRowsRaw: struct (int * ParameterDefinitionKey * bool) list)
    (parameterHandleLookup: Dictionary<ParameterDefinitionKey, ParameterHandle>)
    (baselineParameterHandles: Map<ParameterDefinitionKey, ParameterDefinitionMetadataHandles>)
    (syntheticParameterInfo: Dictionary<ParameterDefinitionKey, ParameterAttributes>)
    (firstParamRowByMethod: Dictionary<MethodDefinitionKey, int>)
    (returnParameterKeys: HashSet<ParameterDefinitionKey>)
    (metadataReader: MetadataReader)
    : ParameterDefinitionRowInfo list =
    let rows =
        parameterDefinitionRowsRaw
        |> List.choose (fun struct (rowId, key, isAdded) ->
            if rowId = 0 then
                None
            else
                let attrs, sequence, nameOpt, resolvedOffsetOpt =
                    match parameterHandleLookup.TryGetValue key with
                    | true, handle when not handle.IsNil ->
                        let parameter = metadataReader.GetParameter handle

                        let name =
                            if parameter.Name.IsNil then
                                None
                            else
                                metadataReader.GetString parameter.Name |> Some

                        let baselineInfo = baselineParameterHandles |> Map.tryFind key

                        let resolvedOffset =
                            match baselineInfo |> Option.bind (fun info -> info.NameOffset) with
                            | Some offset ->
                                // Reuse the baseline name offset only when the fresh name
                                // matches the baseline name. A differing name is a
                                // parameter RENAME (classification gates it on the
                                // UpdateParameters capability): the re-emitted Param row
                                // writes the NEW name into the delta string heap — the C#
                                // 'param_rename' template shape.
                                match baselineInfo |> Option.bind (fun info -> info.Name) with
                                | Some baselineName when name <> Some baselineName -> None
                                | _ -> Some offset
                            | None ->
                                // Added parameter rows must write their name into the delta
                                // string heap; fresh-compile heap offsets are not valid
                                // against the baseline+delta heap layout.
                                if isAdded || parameter.Name.IsNil then
                                    None
                                else
                                    Some(StringOffset(MetadataTokens.GetHeapOffset parameter.Name))

                        parameter.Attributes, int parameter.SequenceNumber, name, resolvedOffset
                    | _ ->
                        let attrs =
                            match syntheticParameterInfo.TryGetValue key with
                            | true, value -> value
                            | _ -> ParameterAttributes.None

                        attrs, key.SequenceNumber, None, None

                match firstParamRowByMethod.TryGetValue key.Method with
                | true, existing when existing <= rowId -> ()
                | _ -> firstParamRowByMethod[key.Method] <- rowId

                // Treat synthesized return parameter rows as added so EncLog/EncMap
                // reflect the new Param table entry, mirroring Roslyn ENC behavior.
                let effectiveIsAdded = if returnParameterKeys.Contains key then true else isAdded

                let nameChanged =
                    baselineParameterHandles
                    |> Map.tryFind key
                    |> Option.exists (fun baseline -> baseline.Name <> nameOpt)

                // Existing unchanged parameters only seed MethodDef.ParamList resolution;
                // they are not rows in the physical delta. Parameter renames are the sole
                // supported existing-Param update and must re-emit their baseline row.
                if not effectiveIsAdded && not nameChanged then
                    None
                else
                    Some
                        {
                            ParameterDefinitionRowInfo.Key = key
                            RowId = rowId
                            IsAdded = effectiveIsAdded
                            Attributes = attrs
                            SequenceNumber = sequence
                            Name = nameOpt
                            NameOffset = resolvedOffsetOpt
                        })

    if traceMethodUpdates.Value then
        printfn "[fsharp-hotreload][param-rows] count=%d" rows.Length

    rows

let private buildMethodDefinitionRowsSnapshot
    (methodDefinitionRowsRaw: struct (int * MethodDefinitionKey * bool) list)
    (methodUpdatesWithDefs: (MethodMetadataUpdate * MethodDefinition * int list) list)
    (methodMetadataLookup: Dictionary<MethodDefinitionKey, MethodMetadataInfo>)
    (baselineMethodHandles: Map<MethodDefinitionKey, MethodDefinitionMetadataHandles>)
    (firstParamRowByMethod: Dictionary<MethodDefinitionKey, int>)
    (baselineMethodTokens: Map<MethodDefinitionKey, int>)
    (methodDefinitionIndex: DefinitionIndex<MethodDefinitionKey>)
    (remapAddedSignature: byte[] -> byte[])
    (tryGetTypeDefRowId: string -> int option)
    : MethodDefinitionRowInfo list =

    let tryBuildMethodRow rowId key isAdded =
        match methodMetadataLookup.TryGetValue key with
        | true, (attrs, implAttrs, name, signature, _, _) ->
            let baselineHandles = baselineMethodHandles |> Map.tryFind key
            // Methods without baseline heap entries - added this generation OR added by an
            // EARLIER delta and re-emitted now (the handle cache only covers the on-disk
            // baseline) - must write their name/signature into THIS delta's heaps (offset
            // None). Offsets captured from the fresh compile's heaps are meaningless
            // against the baseline+delta heap layout and produce garbage references.
            let resolvedNameOffset =
                baselineHandles |> Option.bind (fun info -> info.NameOffset)

            let resolvedSignatureOffset =
                baselineHandles |> Option.bind (fun info -> info.SignatureOffset)
            // Signature blobs entering the delta blob heap embed TypeDefOrRef coded
            // indexes of the fresh compile; remap them to baseline rows.
            let resolvedSignature =
                if resolvedSignatureOffset.IsNone then
                    remapAddedSignature signature
                else
                    signature

            let resolvedAttributes =
                match baselineHandles |> Option.bind (fun info -> info.Attributes) with
                | Some value -> value
                | None -> attrs

            let resolvedImplAttributes =
                match baselineHandles |> Option.bind (fun info -> info.ImplAttributes) with
                | Some value -> value
                | None -> implAttrs

            let resolvedCodeRva = baselineHandles |> Option.bind (fun info -> info.Rva)

            let baselineFirstParam =
                baselineHandles |> Option.bind (fun info -> info.FirstParameterRowId)

            let firstParam =
                match firstParamRowByMethod.TryGetValue key with
                | true, value when value > 0 -> Some value
                | _ ->
                    match baselineFirstParam with
                    | Some _ as baselineRow -> baselineRow
                    | None -> None

            // Parent TypeDef row id is required for ADDED methods: the CLR EnC applier links
            // the new method into the parent's member list via the AddMethod EncLog entry.
            let parentTypeDefRowId =
                if isAdded then
                    tryGetTypeDefRowId key.DeclaringType
                else
                    None

            Some
                {
                    MethodDefinitionRowInfo.Key = key
                    RowId = rowId
                    IsAdded = isAdded
                    ParentTypeDefRowId = parentTypeDefRowId
                    Attributes = resolvedAttributes
                    ImplAttributes = resolvedImplAttributes
                    Name = name
                    NameOffset = resolvedNameOffset
                    Signature = resolvedSignature
                    SignatureOffset = resolvedSignatureOffset
                    FirstParameterRowId = firstParam
                    CodeRva = resolvedCodeRva
                }
        | _ -> None

    let initialRows =
        methodDefinitionRowsRaw
        |> List.choose (fun struct (rowId, key, isAdded) -> tryBuildMethodRow rowId key isAdded)

    let existingKeys =
        HashSet<MethodDefinitionKey>(initialRows |> Seq.map (fun row -> row.Key), HashIdentity.Structural)

    let missingRows =
        methodUpdatesWithDefs
        |> List.choose (fun (update, _, _) ->
            if existingKeys.Contains update.MethodKey then
                None
            else
                let rowId =
                    match baselineMethodTokens |> Map.tryFind update.MethodKey with
                    | Some token -> token &&& 0x00FFFFFF
                    | None -> methodDefinitionIndex.GetRowId update.MethodKey

                tryBuildMethodRow rowId update.MethodKey false)

    let rows = initialRows @ missingRows

    if traceMethodUpdates.Value then
        printfn "[fsharp-hotreload][method-rows] count=%d (missing=%d)" rows.Length missingRows.Length
        printfn "[fsharp-hotreload][params] firstParamRowByMethod entries:"

        for KeyValue(k, v) in firstParamRowByMethod do
            printfn "  %s::%s firstParamRowId=%d" k.DeclaringType k.Name v

        printfn "[fsharp-hotreload][methods] FirstParameterRowId after merge:"

        for row in rows do
            let fp = defaultArg row.FirstParameterRowId 0
            printfn "  method=%s::%s rowId=%d firstParam=%d isAdded=%b" row.Key.DeclaringType row.Key.Name row.RowId fp row.IsAdded

    rows

let private buildMethodSpecificationRowsSnapshot
    (traceMetadata: bool)
    (methodUpdatesWithDefs: (MethodMetadataUpdate * MethodDefinition * int list) list)
    (baselineMethodSpecRowCount: int)
    (methodSpecRowsByToken: Dictionary<int, MethodSpecificationRowInfo>)
    : MethodSpecificationRowInfo list =

    let referencedMethodSpecTokens =
        methodUpdatesWithDefs
        |> List.collect (fun (_, _, methodSpecs) -> methodSpecs)
        |> List.distinct

    if traceMetadata then
        printfn
            "[fsharp-hotreload][metadata] methodspec candidates=%d baselineRows=%d tokens=%s"
            referencedMethodSpecTokens.Length
            baselineMethodSpecRowCount
            (referencedMethodSpecTokens
             |> List.map (fun token -> sprintf "0x%08X" token)
             |> String.concat ",")

    referencedMethodSpecTokens
    |> List.choose (fun methodSpecToken ->
        match methodSpecRowsByToken.TryGetValue methodSpecToken with
        | true, row -> Some row
        | _ ->
            if traceMetadata then
                printfn "[fsharp-hotreload][metadata] missing mapped methodspec token=0x%08X" methodSpecToken

            None)
    |> Seq.sortBy _.RowId
    |> Seq.toList

let private buildPropertyEventAndSemanticsRows
    (traceMethodUpdates: bool)
    (request: IlxDeltaRequest)
    (tryGetTypeDefToken: string -> int option)
    (metadataReader: MetadataReader)
    (propertyDefinitionIndex: DefinitionIndex<PropertyDefinitionKey>)
    (eventDefinitionIndex: DefinitionIndex<EventDefinitionKey>)
    (propertyHandleLookup: Dictionary<PropertyDefinitionKey, PropertyDefinitionHandle>)
    (eventHandleLookup: Dictionary<EventDefinitionKey, EventDefinitionHandle>)
    (baselinePropertyHandles: Map<PropertyDefinitionKey, PropertyDefinitionMetadataHandles>)
    (baselineEventHandles: Map<EventDefinitionKey, EventDefinitionMetadataHandles>)
    (baselineTableRowCounts: int[])
    (remapMethodToken: int -> int)
    (remapEventTypeToken: int -> int)
    (remapAddedSignature: byte[] -> byte[])
    =
    let propertyDefinitionRowsSnapshot =
        propertyDefinitionIndex.Rows
        |> List.choose (fun struct (rowId, key, isAdded) ->
            match propertyHandleLookup.TryGetValue key with
            | true, handle when not handle.IsNil ->
                let propertyDef = metadataReader.GetPropertyDefinition handle
                let name = metadataReader.GetString propertyDef.Name
                let baselineHandles = baselinePropertyHandles |> Map.tryFind key
                // Properties without baseline heap entries - added this generation OR added
                // by an EARLIER delta and re-registered now (the handle cache only covers
                // the on-disk baseline) - must carry their name/signature as delta heap
                // content (offset None). Fresh-compile heap offsets are never valid against
                // the baseline+delta heap layout (same rule as method rows).
                let resolvedNameOffset =
                    baselineHandles |> Option.bind (fun info -> info.NameOffset)

                let resolvedSignatureOffset =
                    baselineHandles |> Option.bind (fun info -> info.SignatureOffset)

                let signature =
                    let rawSignature = metadataReader.GetBlobBytes propertyDef.Signature
                    // PropertySig blobs entering the delta blob heap need their embedded
                    // TypeDefOrRef coded indexes remapped to baseline rows. Gated on isAdded
                    // because the writers only EMIT added Property rows (an accessor body
                    // edit re-registers the existing row but its snapshot row is dropped);
                    // remapping a never-emitted row could side-effect TypeRef appends.
                    if isAdded && resolvedSignatureOffset.IsNone then
                        remapAddedSignature rawSignature
                    else
                        rawSignature

                Some
                    {
                        PropertyDefinitionRowInfo.Key = key
                        RowId = rowId
                        IsAdded = isAdded
                        // Filled below once the PropertyMap row ids are allocated.
                        ParentPropertyMapRowId = None
                        Name = name
                        NameOffset = resolvedNameOffset
                        Signature = signature
                        SignatureOffset = resolvedSignatureOffset
                        Attributes = propertyDef.Attributes
                    }
            | _ -> None)

    if traceMethodUpdates then
        printfn "[fsharp-hotreload][property-rows] count=%d" propertyDefinitionRowsSnapshot.Length

    let eventDefinitionRowsSnapshot =
        eventDefinitionIndex.Rows
        |> List.choose (fun struct (rowId, key, isAdded) ->
            match eventHandleLookup.TryGetValue key with
            | true, handle when not handle.IsNil ->
                let eventDef = metadataReader.GetEventDefinition handle
                let name = metadataReader.GetString eventDef.Name
                // Events without a baseline heap entry write their name into the delta
                // string heap (see the property snapshot above for rationale).
                let resolvedNameOffset =
                    baselineEventHandles
                    |> Map.tryFind key
                    |> Option.bind (fun info -> info.NameOffset)

                let eventType =
                    // Added events carry a fresh-compile TypeDefOrRef in their EventType
                    // column; remap it to baseline/delta rows (the content-validated
                    // reference remapper appends TypeRef rows as needed).
                    if isAdded && not eventDef.Type.IsNil then
                        let remappedToken = remapEventTypeToken (MetadataTokens.GetToken eventDef.Type)
                        let rowNumber = remappedToken &&& 0x00FFFFFF

                        match remappedToken >>> 24 with
                        | 0x02 -> TDR_TypeDef(TypeDefHandle rowNumber)
                        | 0x01 -> TDR_TypeRef(TypeRefHandle rowNumber)
                        | 0x1b -> TDR_TypeSpec(TypeSpecHandle rowNumber)
                        | _ -> TDR_TypeDef(TypeDefHandle 0)
                    else
                        entityHandleToTypeDefOrRef eventDef.Type

                Some
                    {
                        EventDefinitionRowInfo.Key = key
                        RowId = rowId
                        IsAdded = isAdded
                        // Filled below once the EventMap row ids are allocated.
                        ParentEventMapRowId = None
                        Name = name
                        NameOffset = resolvedNameOffset
                        Attributes = eventDef.Attributes
                        EventType = eventType
                    }
            | _ -> None)

    let propertyRowsByType =
        propertyDefinitionRowsSnapshot
        |> Seq.groupBy (fun row -> row.Key.DeclaringType)
        |> dict

    let eventRowsByType =
        eventDefinitionRowsSnapshot
        |> Seq.groupBy (fun row -> row.Key.DeclaringType)
        |> dict

    let baselinePropertyMapRowCount =
        baselineTableRowCounts.[TableNames.PropertyMap.Index]

    let baselineEventMapRowCount = baselineTableRowCounts.[TableNames.EventMap.Index]

    let propertyMapDefinitionIndex =
        let tryExisting typeName =
            request.Baseline.PropertyMapEntries |> Map.tryFind typeName

        DefinitionIndex<string>(tryExisting, baselinePropertyMapRowCount)

    let eventMapDefinitionIndex =
        let tryExisting typeName =
            request.Baseline.EventMapEntries |> Map.tryFind typeName

        DefinitionIndex<string>(tryExisting, baselineEventMapRowCount)

    let propertyMapRowsSnapshot =
        let missingTypes =
            propertyDefinitionRowsSnapshot
            |> Seq.filter _.IsAdded
            |> Seq.map (fun row -> row.Key.DeclaringType)
            |> Seq.filter (fun typeName -> not (request.Baseline.PropertyMapEntries |> Map.containsKey typeName))
            |> Seq.distinct
            |> Seq.toList

        for typeName in missingTypes do
            propertyMapDefinitionIndex.Add typeName |> ignore

        propertyMapDefinitionIndex.Rows
        |> List.choose (fun struct (rowId, typeName, isAdded) ->
            // ADDED types (closure classes and user-defined) have no baseline TypeDef token; their map rows
            // parent the new delta TypeDef row.
            let typeTokenOpt = tryGetTypeDefToken typeName

            let firstPropertyRowIdOpt =
                match propertyRowsByType.TryGetValue typeName with
                | true, rows -> rows |> Seq.sortBy _.RowId |> Seq.tryHead |> Option.map _.RowId
                | _ -> None

            let shouldAdd = isAdded || List.contains typeName missingTypes

            match typeTokenOpt, firstPropertyRowIdOpt, shouldAdd with
            | Some typeToken, Some firstRowId, true ->
                Some
                    {
                        PropertyMapRowInfo.DeclaringType = typeName
                        RowId = rowId
                        TypeDefRowId = typeToken &&& 0x00FFFFFF
                        FirstPropertyRowId = Some firstRowId
                        IsAdded = true
                    }
            | _ -> None)

    let eventMapRowsSnapshot =
        let missingTypes =
            eventDefinitionRowsSnapshot
            |> Seq.filter _.IsAdded
            |> Seq.map (fun row -> row.Key.DeclaringType)
            |> Seq.filter (fun typeName -> not (request.Baseline.EventMapEntries |> Map.containsKey typeName))
            |> Seq.distinct
            |> Seq.toList

        for typeName in missingTypes do
            eventMapDefinitionIndex.Add typeName |> ignore

        eventMapDefinitionIndex.Rows
        |> List.choose (fun struct (rowId, typeName, isAdded) ->
            let typeTokenOpt = tryGetTypeDefToken typeName

            let firstEventRowIdOpt =
                match eventRowsByType.TryGetValue typeName with
                | true, rows -> rows |> Seq.sortBy _.RowId |> Seq.tryHead |> Option.map _.RowId
                | _ -> None

            let shouldAdd = isAdded || List.contains typeName missingTypes

            match typeTokenOpt, firstEventRowIdOpt, shouldAdd with
            | Some typeToken, Some firstRowId, true ->
                Some
                    {
                        EventMapRowInfo.DeclaringType = typeName
                        RowId = rowId
                        TypeDefRowId = typeToken &&& 0x00FFFFFF
                        FirstEventRowId = Some firstRowId
                        IsAdded = true
                    }
            | _ -> None)

    let mutable nextMethodSemanticsRowId =
        baselineTableRowCounts.[TableNames.MethodSemantics.Index]

    // MethodSemantics rows for ADDED properties/events are derived from the fresh
    // compile's accessor relationships (Roslyn parity: DeltaMetadataWriter emits the
    // semantics rows from the symbol model, not from the edit list), so every added
    // Property/Event row carries its Getter/Setter/Adder/Remover/Raiser bindings even
    // when the accessors are compiler-synthesized (module values, [<CLIEvent>] members).
    // Accessor method tokens come from the fresh metadata and are remapped to
    // baseline/delta MethodDef rows. A Property/Event row without its semantics rows is
    // corrupt metadata, so a missing/unmappable accessor fails closed below.
    let methodSemanticsRowsSnapshot =
        let accessorRow (memberDisplay: string) (attrs: MethodSemanticsAttributes) (handle: MethodDefinitionHandle) association =
            if handle.IsNil then
                None
            else
                let freshToken = MetadataTokens.GetToken(EntityHandle.op_Implicit handle)
                let mappedToken = remapMethodToken freshToken

                if mappedToken >>> 24 <> 0x06 || mappedToken &&& 0x00FFFFFF = 0 then
                    raise (
                        HotReloadUnsupportedEditException(
                            $"Added member '{memberDisplay}' has an accessor that does not map to a baseline or delta MethodDef row; please rebuild."
                        )
                    )

                nextMethodSemanticsRowId <- nextMethodSemanticsRowId + 1

                Some
                    {
                        MethodSemanticsMetadataUpdate.RowId = nextMethodSemanticsRowId
                        MethodToken = mappedToken
                        Attributes = attrs
                        IsAdded = true
                        AssociationInfo = association
                    }

        let propertySemantics =
            propertyDefinitionRowsSnapshot
            |> List.filter (fun row -> row.IsAdded)
            |> List.collect (fun row ->
                let display = $"{row.Key.DeclaringType}::{row.Key.Name}"
                let association = MethodSemanticsAssociation.PropertyAssociation(row.Key, row.RowId)

                let rows =
                    match propertyHandleLookup.TryGetValue row.Key with
                    | true, handle when not handle.IsNil ->
                        let accessors = (metadataReader.GetPropertyDefinition handle).GetAccessors()

                        [
                            yield!
                                accessorRow display MethodSemanticsAttributes.Getter accessors.Getter association
                                |> Option.toList
                            yield!
                                accessorRow display MethodSemanticsAttributes.Setter accessors.Setter association
                                |> Option.toList
                            for other in accessors.Others do
                                yield!
                                    accessorRow display MethodSemanticsAttributes.Other other association
                                    |> Option.toList
                        ]
                    | _ -> []

                if List.isEmpty rows then
                    // Fail closed: a Property row whose accessors cannot be bound would be
                    // unreachable, corrupt metadata after apply.
                    raise (
                        HotReloadUnsupportedEditException(
                            $"Added property '{display}' has no resolvable accessor methods; please rebuild."
                        )
                    )

                rows)

        let eventSemantics =
            eventDefinitionRowsSnapshot
            |> List.filter (fun row -> row.IsAdded)
            |> List.collect (fun row ->
                let display = $"{row.Key.DeclaringType}::{row.Key.Name}"
                let association = MethodSemanticsAssociation.EventAssociation(row.Key, row.RowId)

                let rows =
                    match eventHandleLookup.TryGetValue row.Key with
                    | true, handle when not handle.IsNil ->
                        let accessors = (metadataReader.GetEventDefinition handle).GetAccessors()

                        [
                            yield!
                                accessorRow display MethodSemanticsAttributes.Adder accessors.Adder association
                                |> Option.toList
                            yield!
                                accessorRow display MethodSemanticsAttributes.Remover accessors.Remover association
                                |> Option.toList
                            yield!
                                accessorRow display MethodSemanticsAttributes.Raiser accessors.Raiser association
                                |> Option.toList
                            for other in accessors.Others do
                                yield!
                                    accessorRow display MethodSemanticsAttributes.Other other association
                                    |> Option.toList
                        ]
                    | _ -> []

                if List.isEmpty rows then
                    raise (
                        HotReloadUnsupportedEditException($"Added event '{display}' has no resolvable accessor methods; please rebuild.")
                    )

                rows)

        propertySemantics @ eventSemantics

    // Fill parent map row ids for ADDED properties/events now that map rows are allocated;
    // the AddProperty/AddEvent EncLog entries carry the parent map token (CLR requirement).
    // Covers both newly added map rows and maps that already exist in the baseline.
    let propertyDefinitionRowsSnapshot =
        propertyDefinitionRowsSnapshot
        |> List.map (fun row ->
            if
                row.IsAdded
                && row.ParentPropertyMapRowId.IsNone
                && propertyMapDefinitionIndex.Contains row.Key.DeclaringType
            then
                { row with
                    ParentPropertyMapRowId = Some(propertyMapDefinitionIndex.GetRowId row.Key.DeclaringType)
                }
            else
                row)

    let eventDefinitionRowsSnapshot =
        eventDefinitionRowsSnapshot
        |> List.map (fun row ->
            if
                row.IsAdded
                && row.ParentEventMapRowId.IsNone
                && eventMapDefinitionIndex.Contains row.Key.DeclaringType
            then
                { row with
                    ParentEventMapRowId = Some(eventMapDefinitionIndex.GetRowId row.Key.DeclaringType)
                }
            else
                row)

    propertyDefinitionRowsSnapshot, eventDefinitionRowsSnapshot, propertyMapRowsSnapshot, eventMapRowsSnapshot, methodSemanticsRowsSnapshot

/// Fresh-compile tokens and delta row tokens of the members ADDED by this delta, used to
/// emit their CustomAttribute rows (the fresh compile's attributes ARE the attributes the
/// added member must carry; C# reference templates show 2-4 CA rows per added member,
/// e.g. [CompilerGenerated]/[DebuggerBrowsable] on auto-property backing fields and
/// accessors).
type private AddedMemberAttributeSources =
    {
        AddedFieldTokens: Dictionary<FieldDefinitionKey, int>
        AddedFieldDeltaTokens: Dictionary<FieldDefinitionKey, int>
        AddedPropertyTokens: Dictionary<PropertyDefinitionKey, int>
        AddedPropertyDeltaTokens: Dictionary<PropertyDefinitionKey, int>
        AddedEventTokens: Dictionary<EventDefinitionKey, int>
        AddedEventDeltaTokens: Dictionary<EventDefinitionKey, int>
        AddedTypeNewTokens: Dictionary<string, int>
        AddedTypeDeltaTokens: Dictionary<string, int>
    }

let private assemblyReferenceKey (metadataReader: MetadataReader) (row: System.Reflection.Metadata.AssemblyReference) =
    let getBlob (handle: BlobHandle) =
        if handle.IsNil then
            []
        else
            metadataReader.GetBlobBytes handle |> Array.toList

    {
        AssemblyReferenceKey.Name =
            if row.Name.IsNil then
                ""
            else
                metadataReader.GetString row.Name
        MajorVersion = row.Version.Major
        MinorVersion = row.Version.Minor
        BuildNumber = row.Version.Build
        RevisionNumber = row.Version.Revision
        Culture =
            if row.Culture.IsNil then
                ""
            else
                metadataReader.GetString row.Culture
        PublicKeyOrToken = getBlob row.PublicKeyOrToken
        Flags = int row.Flags
    }

let private buildCustomAttributeRows
    (traceMetadata: bool)
    (metadataReader: MetadataReader)
    (baselineTableRowCounts: int[])
    (baselineCustomAttributeRows: Map<int, BaselineCustomAttributeRow>)
    (methodUpdateInputs: struct (MethodDefinitionKey * int * MethodDefinitionHandle * MethodDefinition * MethodBodyBlock) list)
    (methodDefinitionRowsRaw: struct (int * MethodDefinitionKey * bool) list)
    (methodDefinitionIndex: DefinitionIndex<MethodDefinitionKey>)
    (methodUpdatesWithDefs: (MethodMetadataUpdate * MethodDefinition * int list) list)
    (addedMemberSources: AddedMemberAttributeSources)
    (remapEntityToken: int -> int)
    (remapAssemblyRefToken: int -> int)
    (baselineTypeReferenceTokens: Map<TypeReferenceKey, int>)
    (getNextTypeRefRowId: unit -> int)
    (setNextTypeRefRowId: int -> unit)
    (getNextMemberRefRowId: unit -> int)
    (setNextMemberRefRowId: int -> unit)
    (typeReferenceRows: ResizeArray<TypeReferenceRowInfo>)
    (memberReferenceRows: ResizeArray<MemberReferenceRowInfo>)
    =
    // Rows UPDATING an existing CA row keep their baseline row ids; ADDED rows are
    // renumbered past the chained baseline row count at the end.
    let updatedRows = ResizeArray<CustomAttributeRowInfo>()
    let rows = ResizeArray<CustomAttributeRowInfo>()

    let allocateTypeRefRow () =
        let rowId = getNextTypeRefRowId () + 1
        setNextTypeRefRowId rowId
        rowId

    let allocateMemberRefRow () =
        let rowId = getNextMemberRefRowId () + 1
        setNextMemberRefRowId rowId
        rowId

    // Baselines without byte-derived CA snapshots (legacy construction paths) keep the
    // historic append-only behavior; F# baselines always carry assembly-level attribute
    // rows, so an empty map means the snapshot was unavailable.
    let hasBaselineCaSnapshot = not (Map.isEmpty baselineCustomAttributeRows)

    // Baseline CA rows grouped by parent token, ascending row id (Map iteration order).
    let baselineRowsByParent =
        Dictionary<int, ResizeArray<int * BaselineCustomAttributeRow>>()

    for KeyValue(rowId, row) in baselineCustomAttributeRows do
        match baselineRowsByParent.TryGetValue row.ParentToken with
        | true, list -> list.Add(rowId, row)
        | _ ->
            let list = ResizeArray()
            list.Add(rowId, row)
            baselineRowsByParent[row.ParentToken] <- list

    let methodRowIdToKey = Dictionary<int, MethodDefinitionKey>(HashIdentity.Structural)

    for struct (rowId, key, _) in methodDefinitionRowsRaw do
        methodRowIdToKey[rowId] <- key

    let methodsWithCustomAttribute =
        HashSet<MethodDefinitionKey>(HashIdentity.Structural)

    let methodsWithNullableContextAttribute =
        HashSet<MethodDefinitionKey>(HashIdentity.Structural)

    let mutable nullableContextAttributeSeen = false

    let encodeNullableContextValue () =
        [| 0x01uy; 0x00uy; 0x01uy; 0x00uy; 0x00uy |]

    let rec getFullTypeName (handle: TypeDefinitionHandle) =
        let def = metadataReader.GetTypeDefinition handle
        let name = metadataReader.GetString def.Name

        let ns =
            if def.Namespace.IsNil then
                ""
            else
                metadataReader.GetString def.Namespace

        let declaring = def.GetDeclaringType()

        if declaring.IsNil then
            if String.IsNullOrEmpty ns then name else $"{ns}.{name}"
        else
            $"{getFullTypeName declaring}+{name}"

    let tryFindTypeDefinition fullName =
        metadataReader.TypeDefinitions
        |> Seq.tryPick (fun handle ->
            let def = metadataReader.GetTypeDefinition handle
            let name = metadataReader.GetString def.Name

            let ns =
                if def.Namespace.IsNil then
                    ""
                else
                    metadataReader.GetString def.Namespace

            let decl = if String.IsNullOrEmpty ns then name else $"{ns}.{name}"

            if String.Equals(decl, fullName, StringComparison.Ordinal) then
                Some handle
            else
                None)

    let tryFindStateMachineType (methodKey: MethodDefinitionKey) =
        match tryFindTypeDefinition methodKey.DeclaringType with
        | None -> ValueNone
        | Some parentHandle ->
            let parentDef = metadataReader.GetTypeDefinition parentHandle
            let nestedTypes = parentDef.GetNestedTypes()
            let prefix = methodKey.Name + "@hotreload"

            // Closure classes share the '{method}@hotreload' naming with async state
            // machines; only genuine state machines carry a MoveNext method, so require
            // one before synthesizing an AsyncStateMachineAttribute for the method.
            let hasMoveNext (handle: TypeDefinitionHandle) =
                metadataReader.GetTypeDefinition(handle).GetMethods()
                |> Seq.exists (fun methodHandle ->
                    metadataReader.GetString(metadataReader.GetMethodDefinition(methodHandle).Name) = "MoveNext")

            let matches =
                nestedTypes
                |> Seq.choose (fun nested ->
                    let nestedDef = metadataReader.GetTypeDefinition nested
                    let name = metadataReader.GetString nestedDef.Name

                    if name.StartsWith(prefix, StringComparison.Ordinal) && hasMoveNext nested then
                        Some(name, nested)
                    else
                        None)
                |> Seq.toArray

            if matches.Length = 0 then
                ValueNone
            else
                matches
                |> Array.tryFind (fun (name, _) -> String.Equals(name, prefix, StringComparison.Ordinal))
                |> ValueOption.ofOption
                |> ValueOption.orElseWith (fun () -> matches |> Array.tryHead |> ValueOption.ofOption)
                |> ValueOption.map snd

    let tryFindAssemblyReference scopeName =
        metadataReader.AssemblyReferences
        |> Seq.tryPick (fun handle ->
            let reference = metadataReader.GetAssemblyReference handle
            let name = metadataReader.GetString reference.Name

            if String.Equals(name, scopeName, StringComparison.OrdinalIgnoreCase) then
                Some(handle, reference, assemblyReferenceKey metadataReader reference)
            else
                None)

    let findAssemblyReferenceRow scopeName =
        tryFindAssemblyReference scopeName
        |> Option.map (fun (handle, _, _) ->
            let token = MetadataTokens.GetToken(EntityHandle.op_Implicit handle)
            let remapped = remapAssemblyRefToken token
            let rowId = remapped &&& 0x00FFFFFF
            RS_AssemblyRef(AssemblyRefHandle rowId))

    let tryGetAssemblyScope () =
        match findAssemblyReferenceRow "System.Runtime" with
        | Some scope -> Some scope
        | None -> findAssemblyReferenceRow "mscorlib"

    let tryFindSystemTypeRef () =
        metadataReader.TypeReferences
        |> Seq.tryPick (fun handle ->
            let typeRef = metadataReader.GetTypeReference handle
            let name = metadataReader.GetString typeRef.Name

            let ns =
                if typeRef.Namespace.IsNil then
                    ""
                else
                    metadataReader.GetString typeRef.Namespace

            if
                String.Equals(name, "Type", StringComparison.Ordinal)
                && String.Equals(ns, "System", StringComparison.Ordinal)
            then
                Some handle
            else
                None)

    let tryReuseBaselineTypeRef scopeName namespaceName typeName =
        tryFindAssemblyReference scopeName
        |> Option.bind (fun (_, _, assemblyKey) ->
            let key =
                {
                    TypeReferenceKey.Scope = TypeReferenceScope.Assembly assemblyKey
                    Namespace = namespaceName
                    Name = typeName
                }

            baselineTypeReferenceTokens |> Map.tryFind key)

    let tryFindExistingTypeRef scopeName namespaceName typeName =
        tryFindAssemblyReference scopeName
        |> Option.bind (fun (_, _, expectedAssemblyKey) ->
            metadataReader.TypeReferences
            |> Seq.tryPick (fun handle ->
                let typeRef = metadataReader.GetTypeReference handle
                let name = metadataReader.GetString typeRef.Name

                if name = typeName then
                    let ns =
                        if typeRef.Namespace.IsNil then
                            ""
                        else
                            metadataReader.GetString typeRef.Namespace

                    if ns = namespaceName then
                        match typeRef.ResolutionScope.Kind with
                        | HandleKind.AssemblyReference ->
                            let asm =
                                metadataReader.GetAssemblyReference(AssemblyReferenceHandle.op_Explicit typeRef.ResolutionScope)

                            if assemblyReferenceKey metadataReader asm = expectedAssemblyKey then
                                Some handle
                            else
                                None
                        | _ -> None
                    else
                        None
                else
                    None))

    let mutable systemObjectTypeRefToken: int option = None
    let mutable asyncAttributeTypeRefToken: int option = None
    let mutable asyncAttributeCtorToken: int option = None
    let mutable nullableContextAttributeTypeRefToken: int option = None
    let mutable nullableContextAttributeCtorToken: int option = None

    let ensureSystemObjectTypeRef () =
        match systemObjectTypeRefToken with
        | Some token -> token
        | None ->
            let scope =
                match tryGetAssemblyScope () with
                | Some value -> value
                | None -> RS_Module(ModuleHandle 1)

            let nextRowId = allocateTypeRefRow ()

            typeReferenceRows.Add(
                {
                    RowId = nextRowId
                    ResolutionScope = scope
                    Name = "Object"
                    NameOffset = None
                    Namespace = "System"
                    NamespaceOffset = None
                }
            )

            let token = 0x01000000 ||| nextRowId
            systemObjectTypeRefToken <- Some token
            token

    let ensureSystemTypeRefHandle () =
        match tryFindSystemTypeRef () with
        | Some handle -> handle
        | None ->
            let scope =
                match tryGetAssemblyScope () with
                | Some value -> value
                | None -> RS_Module(ModuleHandle 1)

            let nextRowId = allocateTypeRefRow ()

            typeReferenceRows.Add(
                {
                    RowId = nextRowId
                    ResolutionScope = scope
                    Name = "Type"
                    NameOffset = None
                    Namespace = "System"
                    NamespaceOffset = None
                }
            )

            MetadataTokens.TypeReferenceHandle nextRowId

    let ensureAsyncAttributeTypeRef () =
        match asyncAttributeTypeRefToken with
        | Some token -> token
        | None ->
            let scope =
                match tryGetAssemblyScope () with
                | Some value -> value
                | None ->
                    raise (
                        HotReloadUnsupportedEditException
                            "Unable to locate System.Runtime/mscorlib assembly reference for AsyncStateMachineAttribute. Please rebuild."
                    )

            let nextRowId = allocateTypeRefRow ()

            typeReferenceRows.Add(
                {
                    RowId = nextRowId
                    ResolutionScope = scope
                    Name = "AsyncStateMachineAttribute"
                    NameOffset = None
                    Namespace = "System.Runtime.CompilerServices"
                    NamespaceOffset = None
                }
            )

            let token = 0x01000000 ||| nextRowId
            asyncAttributeTypeRefToken <- Some token
            token

    let ensureAsyncAttributeCtor () =
        match asyncAttributeCtorToken with
        | Some token -> token
        | None ->
            let attrTypeToken = ensureAsyncAttributeTypeRef ()
            let systemTypeRefHandle = ensureSystemTypeRefHandle ()

            let signatureBytes =
                let blob = BlobBuilder()

                let instanceHeader =
                    (int SignatureCallingConvention.Default) ||| (int SignatureAttributes.Instance)
                    |> byte

                blob.WriteByte(instanceHeader)
                blob.WriteCompressedInteger 1
                blob.WriteByte(LanguagePrimitives.EnumToValue SignatureTypeCode.Void)
                blob.WriteByte(0x12uy)

                let typeRefEntity: EntityHandle =
                    TypeReferenceHandle.op_Implicit systemTypeRefHandle

                let codedIndex = CodedIndex.TypeDefOrRef typeRefEntity
                blob.WriteCompressedInteger codedIndex
                blob.ToArray()

            let parentRowId = attrTypeToken &&& 0x00FFFFFF
            let nextRowId = allocateMemberRefRow ()

            memberReferenceRows.Add(
                {
                    RowId = nextRowId
                    Parent = MRP_TypeRef(TypeRefHandle parentRowId)
                    Name = ".ctor"
                    NameOffset = None
                    Signature = signatureBytes
                    SignatureOffset = None
                }
            )

            let token = 0x0A000000 ||| nextRowId
            asyncAttributeCtorToken <- Some token
            token

    let ensureNullableContextAttributeTypeRef () =
        match nullableContextAttributeTypeRefToken with
        | Some token -> token
        | None ->
            let baselineOrExistingToken =
                [ "System.Runtime"; "mscorlib" ]
                |> List.tryPick (fun scope ->
                    match tryReuseBaselineTypeRef scope "System.Runtime.CompilerServices" "NullableContextAttribute" with
                    | Some token -> Some token
                    | None ->
                        match tryFindExistingTypeRef scope "System.Runtime.CompilerServices" "NullableContextAttribute" with
                        | Some handle -> Some(MetadataTokens.GetToken(EntityHandle.op_Implicit handle))
                        | None -> None)

            match baselineOrExistingToken with
            | Some token ->
                nullableContextAttributeTypeRefToken <- Some token
                token
            | None ->
                let scope =
                    match tryGetAssemblyScope () with
                    | Some value -> value
                    | None ->
                        raise (
                            HotReloadUnsupportedEditException
                                "Unable to locate System.Runtime/mscorlib assembly reference for NullableContextAttribute. Please rebuild."
                        )

                let nextRowId = allocateTypeRefRow ()

                typeReferenceRows.Add(
                    {
                        RowId = nextRowId
                        ResolutionScope = scope
                        Name = "NullableContextAttribute"
                        NameOffset = None
                        Namespace = "System.Runtime.CompilerServices"
                        NamespaceOffset = None
                    }
                )

                let token = 0x01000000 ||| nextRowId
                nullableContextAttributeTypeRefToken <- Some token
                let _ = ensureSystemObjectTypeRef ()
                token

    let ensureNullableContextAttributeCtor () =
        match nullableContextAttributeCtorToken with
        | Some token -> token
        | None ->
            let attrTypeToken = ensureNullableContextAttributeTypeRef ()

            let signatureBytes =
                let blob = BlobBuilder()

                let instanceHeader =
                    (int SignatureCallingConvention.Default) ||| (int SignatureAttributes.Instance)
                    |> byte

                blob.WriteByte(instanceHeader)
                blob.WriteCompressedInteger 1
                blob.WriteByte(LanguagePrimitives.EnumToValue SignatureTypeCode.Void)
                blob.WriteByte(LanguagePrimitives.EnumToValue SignatureTypeCode.Byte)
                blob.ToArray()

            let parentRowId = attrTypeToken &&& 0x00FFFFFF
            let nextRowId = allocateMemberRefRow ()

            memberReferenceRows.Add(
                {
                    RowId = nextRowId
                    Parent = MRP_TypeRef(TypeRefHandle parentRowId)
                    Name = ".ctor"
                    NameOffset = None
                    Signature = signatureBytes
                    SignatureOffset = None
                }
            )

            let token = 0x0A000000 ||| nextRowId
            nullableContextAttributeCtorToken <- Some token
            token

    let encodeAsyncAttributeValue (stateMachineFullName: string) =
        let blob = BlobBuilder()
        blob.WriteUInt16(0x0001us)
        blob.WriteSerializedString(stateMachineFullName)
        blob.WriteUInt16(0us)
        blob.ToArray()

    let isAsyncStateMachineAttribute (attribute: CustomAttribute) =
        match attribute.Constructor.Kind with
        | HandleKind.MemberReference ->
            let memberRef =
                metadataReader.GetMemberReference(MemberReferenceHandle.op_Explicit attribute.Constructor)

            match memberRef.Parent.Kind with
            | HandleKind.TypeReference ->
                let typeRef =
                    metadataReader.GetTypeReference(TypeReferenceHandle.op_Explicit memberRef.Parent)

                let name = metadataReader.GetString typeRef.Name

                let ns =
                    if typeRef.Namespace.IsNil then
                        ""
                    else
                        metadataReader.GetString typeRef.Namespace

                ns = "System.Runtime.CompilerServices"
                && name.EndsWith("StateMachineAttribute", StringComparison.Ordinal)
            | _ -> false
        | _ -> false

    let isNullableContextAttribute (attribute: CustomAttribute) =
        match attribute.Constructor.Kind with
        | HandleKind.MemberReference ->
            let memberRef =
                metadataReader.GetMemberReference(MemberReferenceHandle.op_Explicit attribute.Constructor)

            match memberRef.Parent.Kind with
            | HandleKind.TypeReference ->
                let typeRef =
                    metadataReader.GetTypeReference(TypeReferenceHandle.op_Explicit memberRef.Parent)

                let name = metadataReader.GetString typeRef.Name

                let ns =
                    if typeRef.Namespace.IsNil then
                        ""
                    else
                        metadataReader.GetString typeRef.Namespace

                ns = "System.Runtime.CompilerServices" && name = "NullableContextAttribute"
            | _ -> false
        | _ -> false

    // Remaps the fresh-compile attribute constructor and builds the row payload. RowId 0
    // marks an APPENDED row (renumbered at the end); the value blob is written into the
    // DELTA blob heap when `reuseFreshValueOffset` is false (fresh heap offsets are not
    // valid against the baseline+delta heap layout); the legacy no-snapshot path keeps
    // the historic offset reuse for re-emitted rows of updated methods.
    let buildAttributeRow (reuseFreshValueOffset: bool) (parent: HasCustomAttribute) (attribute: CustomAttribute) =
        let constructorToken = MetadataTokens.GetToken attribute.Constructor
        let remappedConstructorToken = remapEntityToken constructorToken
        let ctorRowId = remappedConstructorToken &&& 0x00FFFFFF

        let valueBytes =
            if attribute.Value.IsNil then
                Array.empty
            else
                metadataReader.GetBlobBytes attribute.Value

        let ctorType =
            match attribute.Constructor.Kind with
            | HandleKind.MethodDefinition -> CAT_MethodDef(MethodDefHandle ctorRowId)
            | HandleKind.MemberReference -> CAT_MemberRef(MemberRefHandle ctorRowId)
            | _ -> CAT_MemberRef(MemberRefHandle ctorRowId)

        {
            RowId = 0
            Parent = parent
            Constructor = ctorType
            Value = valueBytes
            ValueOffset =
                if attribute.Value.IsNil || not reuseFreshValueOffset then
                    None
                else
                    Some(BlobOffset(MetadataTokens.GetHeapOffset attribute.Value))
        }

    // A zeroed CA row deletes the attribute (C# attr_remove template: Parent/Constructor/
    // Value columns all nil; raw row bytes 00000000-03000000-00000000 — the nil
    // constructor keeps the MemberRef tag).
    let zeroedAttributeRow (rowId: int) =
        {
            CustomAttributeRowInfo.RowId = rowId
            Parent = HCA_MethodDef(MethodDefHandle 0)
            Constructor = CAT_MemberRef(MemberRefHandle 0)
            Value = Array.empty
            ValueOffset = None
        }

    for struct (key: MethodDefinitionKey, _, _, methodDef, _) in methodUpdateInputs do
        if methodDefinitionIndex.Contains key then
            let parentRowId = methodDefinitionIndex.GetRowId key
            let isAddedMethod = methodDefinitionIndex.IsAdded key
            let parent = HCA_MethodDef(MethodDefHandle parentRowId)
            let freshRows = ResizeArray<CustomAttributeRowInfo>()

            for attributeHandle in methodDef.GetCustomAttributes() do
                let attribute = metadataReader.GetCustomAttribute attributeHandle

                if isAsyncStateMachineAttribute attribute then
                    match methodRowIdToKey.TryGetValue parentRowId with
                    | true, methodKey -> methodsWithCustomAttribute.Add methodKey |> ignore
                    | _ -> ()

                if isNullableContextAttribute attribute then
                    nullableContextAttributeSeen <- true

                    match methodRowIdToKey.TryGetValue parentRowId with
                    | true, methodKey ->
                        if traceMetadata then
                            printfn "[fsharp-hotreload][metadata] nullable-context attribute detected on %s" methodKey.Name

                        methodsWithNullableContextAttribute.Add methodKey |> ignore
                    | _ -> ()

                freshRows.Add(buildAttributeRow (not (hasBaselineCaSnapshot || isAddedMethod)) parent attribute)

            if not hasBaselineCaSnapshot then
                // Legacy behavior: re-emit every attribute of the method as an appended row.
                rows.AddRange freshRows
            else
                // Roslyn DeltaMetadataWriter parity (validated against the C# attr_add /
                // attr_change / attr_remove templates): pair the fresh compile's attribute
                // rows against the baseline rows of this parent IN ORDER — unchanged sets
                // emit nothing, changed attributes UPDATE the baseline row in place, extra
                // fresh attributes append new rows, extra baseline rows are ZEROED.
                let parentToken = 0x06000000 ||| parentRowId

                let baselineRows =
                    match baselineRowsByParent.TryGetValue parentToken with
                    | true, list -> list
                    | _ -> ResizeArray()

                let contentMatches =
                    freshRows.Count = baselineRows.Count
                    && Seq.forall2
                        (fun (freshRow: CustomAttributeRowInfo) (_, baselineRow: BaselineCustomAttributeRow) ->
                            customAttributeConstructorToken freshRow.Constructor = baselineRow.ConstructorToken
                            && freshRow.Value = baselineRow.Value)
                        freshRows
                        baselineRows

                if not contentMatches then
                    let shared = min freshRows.Count baselineRows.Count

                    for i in 0 .. shared - 1 do
                        let existingRowId, _ = baselineRows.[i]

                        updatedRows.Add
                            { freshRows.[i] with
                                RowId = existingRowId
                            }

                    for i in shared .. freshRows.Count - 1 do
                        rows.Add freshRows.[i]

                    for i in shared .. baselineRows.Count - 1 do
                        let existingRowId, _ = baselineRows.[i]
                        updatedRows.Add(zeroedAttributeRow existingRowId)

    // CustomAttribute rows of members ADDED by this delta: snapshot the fresh compile's
    // attribute rows for added fields/properties/events/types (added METHODS flow through
    // the methodUpdateInputs loop above). Parents are the new delta rows; constructors go
    // through the content-validated MemberRef reuse/append remap; value blobs enter the
    // delta blob heap. C# reference templates ('prop_add': [CompilerGenerated] +
    // [DebuggerBrowsable] on the backing field and accessors; 'added lambda':
    // [CompilerGenerated] on the new TypeDef) show the same shape.
    for KeyValue(fieldKey, freshToken) in addedMemberSources.AddedFieldTokens do
        match addedMemberSources.AddedFieldDeltaTokens.TryGetValue fieldKey with
        | true, deltaToken ->
            let fieldDef =
                metadataReader.GetFieldDefinition(MetadataTokens.FieldDefinitionHandle(freshToken &&& 0x00FFFFFF))

            let parent = HCA_Field(FieldHandle(deltaToken &&& 0x00FFFFFF))

            for attributeHandle in fieldDef.GetCustomAttributes() do
                rows.Add(buildAttributeRow false parent (metadataReader.GetCustomAttribute attributeHandle))
        | _ -> ()

    if traceMetadata then
        printfn
            "[fsharp-hotreload][metadata] added-member CA sources: fields=%d/%d properties=%d/%d events=%d/%d types=%d/%d"
            addedMemberSources.AddedFieldTokens.Count
            addedMemberSources.AddedFieldDeltaTokens.Count
            addedMemberSources.AddedPropertyTokens.Count
            addedMemberSources.AddedPropertyDeltaTokens.Count
            addedMemberSources.AddedEventTokens.Count
            addedMemberSources.AddedEventDeltaTokens.Count
            addedMemberSources.AddedTypeNewTokens.Count
            addedMemberSources.AddedTypeDeltaTokens.Count

    for KeyValue(propertyKey, freshToken) in addedMemberSources.AddedPropertyTokens do
        match addedMemberSources.AddedPropertyDeltaTokens.TryGetValue propertyKey with
        | true, deltaToken ->
            let propertyDef =
                metadataReader.GetPropertyDefinition(MetadataTokens.PropertyDefinitionHandle(freshToken &&& 0x00FFFFFF))

            let parent = HCA_Property(PropertyHandle(deltaToken &&& 0x00FFFFFF))
            let attributes = propertyDef.GetCustomAttributes()

            if traceMetadata then
                printfn
                    "[fsharp-hotreload][metadata] added-property CA: %s::%s fresh=0x%08X delta=0x%08X attrs=%d"
                    propertyKey.DeclaringType
                    propertyKey.Name
                    freshToken
                    deltaToken
                    attributes.Count

            for attributeHandle in attributes do
                rows.Add(buildAttributeRow false parent (metadataReader.GetCustomAttribute attributeHandle))
        | _ -> ()

    for KeyValue(eventKey, freshToken) in addedMemberSources.AddedEventTokens do
        match addedMemberSources.AddedEventDeltaTokens.TryGetValue eventKey with
        | true, deltaToken ->
            let eventDef =
                metadataReader.GetEventDefinition(MetadataTokens.EventDefinitionHandle(freshToken &&& 0x00FFFFFF))

            let parent = HCA_Event(EventHandle(deltaToken &&& 0x00FFFFFF))

            for attributeHandle in eventDef.GetCustomAttributes() do
                rows.Add(buildAttributeRow false parent (metadataReader.GetCustomAttribute attributeHandle))
        | _ -> ()

    for KeyValue(fullName, freshToken) in addedMemberSources.AddedTypeNewTokens do
        match addedMemberSources.AddedTypeDeltaTokens.TryGetValue fullName with
        | true, deltaToken ->
            let typeDef =
                metadataReader.GetTypeDefinition(MetadataTokens.TypeDefinitionHandle(freshToken &&& 0x00FFFFFF))

            let parent = HCA_TypeDef(TypeDefHandle(deltaToken &&& 0x00FFFFFF))

            for attributeHandle in typeDef.GetCustomAttributes() do
                rows.Add(buildAttributeRow false parent (metadataReader.GetCustomAttribute attributeHandle))
        | _ -> ()

    for (update, _, _) in methodUpdatesWithDefs do
        let methodKey = update.MethodKey

        if methodsWithCustomAttribute.Contains methodKey |> not then
            match tryFindStateMachineType methodKey with
            | ValueSome stateMachineHandle ->
                let ctorToken = ensureAsyncAttributeCtor ()
                let ctorRowId = ctorToken &&& 0x00FFFFFF
                let methodRowId = methodDefinitionIndex.GetRowId methodKey
                let stateMachineFullName = getFullTypeName stateMachineHandle
                let valueBytes = encodeAsyncAttributeValue stateMachineFullName

                rows.Add(
                    {
                        RowId = 0
                        Parent = HCA_MethodDef(MethodDefHandle methodRowId)
                        Constructor = CAT_MemberRef(MemberRefHandle ctorRowId)
                        Value = valueBytes
                        ValueOffset = None
                    }
                )

                methodsWithCustomAttribute.Add methodKey |> ignore
            | ValueNone -> ()

    if nullableContextAttributeSeen then
        for KeyValue(methodRowId, methodKey) in methodRowIdToKey do
            if methodsWithNullableContextAttribute.Contains methodKey |> not then
                let ctorToken = ensureNullableContextAttributeCtor ()
                let ctorRowId = ctorToken &&& 0x00FFFFFF

                rows.Add(
                    {
                        RowId = 0
                        Parent = HCA_MethodDef(MethodDefHandle methodRowId)
                        Constructor = CAT_MemberRef(MemberRefHandle ctorRowId)
                        Value = encodeNullableContextValue ()
                        ValueOffset = None
                    }
                )

                methodsWithNullableContextAttribute.Add methodKey |> ignore

    // The CustomAttribute table sorts on the Parent coded index (ECMA-335 II.22.10);
    // order the APPENDED delta rows the same way (Roslyn DeltaMetadataWriter parity) and
    // renumber them contiguously past the chained baseline row count. Rows UPDATING an
    // existing CA row keep their baseline row ids; the final list is ordered by row id so
    // the physical delta rows line up with the token-sorted EncMap entries.
    let firstAddedRowId = baselineTableRowCounts.[TableNames.CustomAttribute.Index] + 1

    let addedRowList =
        rows
        |> Seq.sortBy (fun row -> (row.Parent.RowId <<< 5) ||| row.Parent.CodedTag)
        |> Seq.mapi (fun index row ->
            { row with
                RowId = firstAddedRowId + index
            })
        |> Seq.toList

    let rowList =
        (List.ofSeq updatedRows) @ addedRowList |> List.sortBy (fun row -> row.RowId)

    if traceMetadata then
        printfn
            "[fsharp-hotreload][metadata] custom-attributes rows=%d (updates=%d adds=%d)"
            (List.length rowList)
            updatedRows.Count
            addedRowList.Length

    rowList

let private buildMethodAndParameterRows
    (orderedMethodInputs: struct (MethodDefinitionKey * int * MethodDefinitionHandle * MethodDefinition * MethodBodyBlock) list)
    (metadataReader: MetadataReader)
    (builder: IlDeltaStreamBuilder)
    (remapUserString: int -> int)
    (remapEntityToken: int -> int)
    (parameterDefinitionRowsRaw: struct (int * ParameterDefinitionKey * bool) list)
    (parameterHandleLookup: Dictionary<ParameterDefinitionKey, ParameterHandle>)
    (baselineParameterHandles: Map<ParameterDefinitionKey, ParameterDefinitionMetadataHandles>)
    (syntheticParameterInfo: Dictionary<ParameterDefinitionKey, ParameterAttributes>)
    (firstParamRowByMethod: Dictionary<MethodDefinitionKey, int>)
    (returnParameterKeys: HashSet<ParameterDefinitionKey>)
    (methodDefinitionRowsRaw: struct (int * MethodDefinitionKey * bool) list)
    (baselineMethodHandles: Map<MethodDefinitionKey, MethodDefinitionMetadataHandles>)
    (baselineMethodTokens: Map<MethodDefinitionKey, int>)
    (methodDefinitionIndex: DefinitionIndex<MethodDefinitionKey>)
    (traceMetadata: bool)
    (baselineMethodSpecRowCount: int)
    (methodSpecRowsByToken: Dictionary<int, MethodSpecificationRowInfo>)
    (tryGetTypeDefRowId: string -> int option)
    (baselineParamRowCount: int)
    =
    let methodUpdatesWithDefs, methodMetadataLookup =
        buildMethodUpdatesWithMetadata orderedMethodInputs metadataReader builder remapUserString remapEntityToken

    let parameterDefinitionRowsSnapshot =
        buildParameterDefinitionRowsSnapshot
            parameterDefinitionRowsRaw
            parameterHandleLookup
            baselineParameterHandles
            syntheticParameterInfo
            firstParamRowByMethod
            returnParameterKeys
            metadataReader

    let methodDefinitionRowsSnapshot =
        buildMethodDefinitionRowsSnapshot
            methodDefinitionRowsRaw
            methodUpdatesWithDefs
            methodMetadataLookup
            baselineMethodHandles
            firstParamRowByMethod
            baselineMethodTokens
            methodDefinitionIndex
            (remapSignatureBlobWith (remapTypeDefOrRefCodedIndexWith remapEntityToken))
            tryGetTypeDefRowId

    let methodSpecificationRowsSnapshot =
        buildMethodSpecificationRowsSnapshot traceMetadata methodUpdatesWithDefs baselineMethodSpecRowCount methodSpecRowsByToken

    // Keep the ParamList column of ADDED method rows monotone (Roslyn parity / ECMA list
    // encoding): a parameterless added method points at the NEXT param row rather than 0,
    // so readers compute an empty [next, next) range instead of a bogus one. The runtime
    // ignores this column for EnC deltas (suppressed), but mdv and other readers do not.
    let methodDefinitionRowsSnapshot =
        let paramRowsByMethod =
            parameterDefinitionRowsSnapshot
            |> List.groupBy (fun row -> row.Key.Method)
            |> dict

        // Added param rows continue from the baseline Param table, so the cursor starts at
        // the first appended row id (or one past the baseline table when nothing is added).
        let mutable nextParamRowId =
            parameterDefinitionRowsSnapshot
            |> List.filter (fun row -> row.IsAdded)
            |> List.map (fun row -> row.RowId)
            |> function
                | [] -> baselineParamRowCount + 1
                | rows -> List.min rows

        methodDefinitionRowsSnapshot
        |> List.sortBy (fun row -> row.RowId)
        |> List.map (fun row ->
            if not row.IsAdded then
                row
            else
                let ownParams =
                    match paramRowsByMethod.TryGetValue row.Key with
                    | true, rows -> rows |> List.filter (fun p -> p.IsAdded)
                    | _ -> []

                match ownParams with
                | [] ->
                    { row with
                        FirstParameterRowId = Some nextParamRowId
                    }
                | rows ->
                    let firstRow = rows |> List.map (fun p -> p.RowId) |> List.min
                    let lastRow = rows |> List.map (fun p -> p.RowId) |> List.max
                    nextParamRowId <- lastRow + 1

                    { row with
                        FirstParameterRowId = Some firstRow
                    })

    methodUpdatesWithDefs, parameterDefinitionRowsSnapshot, methodDefinitionRowsSnapshot, methodSpecificationRowsSnapshot

let private buildAddedOrChangedMethods (methodBodies: MethodBodyUpdate list) =
    methodBodies
    |> List.map (fun body ->
        {
            HotReloadBaseline.AddedOrChangedMethodInfo.MethodToken = body.MethodToken
            LocalSignatureToken = body.LocalSignatureToken
            CodeOffset = body.CodeOffset
            CodeLength = body.CodeLength
        })

let private isCompilerGeneratedMethodDeclaredOnUserType (key: MethodDefinitionKey) =
    IsCompilerGeneratedName key.Name
    && not (IsCompilerGeneratedName key.DeclaringType)

let private filterPublicAddedOrChangedMethods
    (methodTokenToKey: Dictionary<int, MethodDefinitionKey>)
    (addedOrChangedMethods: HotReloadBaseline.AddedOrChangedMethodInfo list)
    =
    addedOrChangedMethods
    |> List.filter (fun info ->
        match methodTokenToKey.TryGetValue info.MethodToken with
        | true, key -> not (isCompilerGeneratedMethodDeclaredOnUserType key)
        | _ -> true)

let private buildDeltaToUpdatedMethodTokenMap
    (methodTokenMap: Dictionary<int, int>)
    (addedOrChangedMethods: HotReloadBaseline.AddedOrChangedMethodInfo list)
    : IReadOnlyDictionary<int, int> =
    let dict = Dictionary<int, int>()

    for KeyValue(newToken, baselineToken) in methodTokenMap do
        dict[baselineToken] <- newToken

    for methodInfo in addedOrChangedMethods do
        if not (dict.ContainsKey methodInfo.MethodToken) then
            dict[methodInfo.MethodToken] <- methodInfo.MethodToken

    dict :> IReadOnlyDictionary<_, _>

let private finalizeDeltaArtifacts
    (request: IlxDeltaRequest)
    (pdbBytesOpt: byte[] option)
    (sequencePointUpdates: FSharp.Compiler.CodeAnalysis.FSharpSequencePointUpdates list)
    (freshSequencePointsByToken: Map<int, ActiveStatementAnalysis.MethodSequencePoints>)
    (encId: Guid)
    (encBaseId: Guid)
    (metadataDelta: MetadataWriter.MetadataDelta)
    (streams: IlDeltaStreams)
    (updatedTypeTokens: int list)
    (updatedMethodTokenList: int list)
    (methodTokenMap: Dictionary<int, int>)
    (matchedMethodTokenPairs: Dictionary<int, int>)
    (userStringEntries: (int * int * string) list)
    (methodDefinitionRowsSnapshot: MethodDefinitionRowInfo list)
    (parameterDefinitionRowsSnapshot: ParameterDefinitionRowInfo list)
    (memberReferenceRowList: MemberReferenceRowInfo list)
    (typeSpecificationRowList: TypeSpecificationRowInfo list)
    (customAttributeRowList: CustomAttributeRowInfo list)
    (propertyMapRowsSnapshot: PropertyMapRowInfo list)
    (eventMapRowsSnapshot: EventMapRowInfo list)
    (methodSemanticsRowsSnapshot: MethodSemanticsMetadataUpdate list)
    (methodTokenToKey: Dictionary<int, MethodDefinitionKey>)
    (addedMethodDeltaTokens: Dictionary<MethodDefinitionKey, int>)
    (addedFieldDeltaTokens: Dictionary<FieldDefinitionKey, int>)
    (addedPropertyDeltaTokens: Dictionary<PropertyDefinitionKey, int>)
    (addedEventDeltaTokens: Dictionary<EventDefinitionKey, int>)
    (addedTypeDeltaTokens: Dictionary<string, int>)
    (addedTypeShapes: Dictionary<string, SynthesizedTypeShape>)
    (addedTypeReferenceTokens: Dictionary<TypeReferenceKey, int>)
    (addedAssemblyReferenceTokens: Dictionary<AssemblyReferenceKey, int>)
    =
    let addedOrChangedMethods = buildAddedOrChangedMethods streams.MethodBodies

    let publicAddedOrChangedMethods =
        filterPublicAddedOrChangedMethods methodTokenToKey addedOrChangedMethods

    let deltaToUpdatedMethodToken =
        buildDeltaToUpdatedMethodTokenMap methodTokenMap addedOrChangedMethods

    let pdbDelta =
        match pdbBytesOpt with
        | None -> None
        | Some pdbBytes ->
            HotReloadPdb.emitDelta
                request.Baseline
                pdbBytes
                addedOrChangedMethods
                deltaToUpdatedMethodToken
                metadataDelta.EncLog
                metadataDelta.EncMap

    let synthesizedSnapshot =
        request.SynthesizedNames
        |> Option.map (fun map -> map.Snapshot |> Seq.map (fun struct (k, v) -> k, v) |> Map.ofSeq)

    // Chain only APPENDED rows into the next-generation table counts. The delta's
    // physical tables also contain re-emitted rows for UPDATED definitions (method-body
    // updates, accessor edits of existing properties/events), which must not advance the
    // row cursors: a later generation would otherwise allocate its added rows past a gap
    // and emit an EncMap that readers reject ("EnCMap not sorted or missing records") and
    // whose members the runtime cannot link. Every appended row carries an EncMap entry
    // past the baseline row count (Roslyn parity), so derive the counts from the EncMap.
    let addedTableCounts =
        let counts = Array.zeroCreate DeltaTokens.TableCount
        let baselineCounts = request.Baseline.Metadata.TableRowCounts

        for (table: TableName), rowId in metadataDelta.EncMap do
            if rowId > baselineCounts.[table.Index] then
                counts.[table.Index] <- counts.[table.Index] + 1

        counts

    let updatedBaselineCore =
        HotReloadBaseline.applyDelta
            request.Baseline
            addedTableCounts
            metadataDelta.HeapSizes
            addedOrChangedMethods
            encId
            encBaseId
            synthesizedSnapshot

    let updatedBaseline =
        buildUpdatedBaseline
            updatedBaselineCore
            parameterDefinitionRowsSnapshot
            memberReferenceRowList
            typeSpecificationRowList
            customAttributeRowList
            propertyMapRowsSnapshot
            eventMapRowsSnapshot
            methodSemanticsRowsSnapshot
            methodTokenToKey
            addedMethodDeltaTokens
            addedFieldDeltaTokens
            addedPropertyDeltaTokens
            addedEventDeltaTokens
            addedTypeDeltaTokens
            addedTypeShapes
            addedTypeReferenceTokens
            addedAssemblyReferenceTokens

    // Sequence-point tracking: replace the committed sequence-point view wholesale with the fresh compile's
    // points, re-keyed from fresh tokens to baseline/delta tokens. Updated methods get their
    // delta-PDB points; unchanged methods get their (possibly line-shift-adjusted) points,
    // matching the line updates the host applies to the debugger alongside this delta.
    let chainedSequencePoints =
        if Map.isEmpty freshSequencePointsByToken then
            None
        else
            let mutable chained = Map.empty

            for KeyValue(newToken, mappedToken) in matchedMethodTokenPairs do
                match Map.tryFind newToken freshSequencePointsByToken with
                | Some points -> chained <- Map.add mappedToken points chained
                | None -> ()

            Some chained

    let updatedBaseline =
        match chainedSequencePoints with
        | Some chained ->
            { updatedBaseline with
                SequencePointSnapshots = chained
            }
        | None -> updatedBaseline

    let delta =
        { emptyDelta with
            Metadata = metadataDelta.Metadata
            IL = streams.IL
            UpdatedTypeTokens = updatedTypeTokens
            UpdatedMethodTokens = updatedMethodTokenList
            RequiredCapabilities =
                request.SymbolChanges
                |> Option.map (fun changes -> changes.RequiredCapabilities)
                |> Option.defaultValue []
                |> List.append [ EditAndContinueCapability.Baseline ]
                |> Set.ofList
                |> Set.toList
                |> List.map (fun capability -> capability.Name)
            EncLog = metadataDelta.EncLog
            EncMap = metadataDelta.EncMap
            MethodBodies = streams.MethodBodies
            StandaloneSignatures = streams.StandaloneSignatures
            Pdb = pdbDelta
            GenerationId = encId
            BaseGenerationId = encBaseId
            UserStringUpdates = userStringEntries
            MethodDefinitionRows = methodDefinitionRowsSnapshot
            AddedOrChangedMethods = publicAddedOrChangedMethods
            UpdatedBaseline = Some updatedBaseline
            SequencePointUpdates = sequencePointUpdates
            ChainedSequencePoints = chainedSequencePoints
        }

    if traceUserStringUpdates.Value then
        for (original, updated, text) in delta.UserStringUpdates do
            printfn "[fsharp-hotreload][userstring-summary] original=0x%08X new=0x%08X text=%s" original updated text

    delta

// Definition-table token remapping stays isolated from metadata-reference row remapping
// so accessor/association resolution can evolve without touching TypeRef/MemberRef/MethodSpec logic.
type private DefinitionTokenRemapper =
    {
        RemapDefinitionToken: int -> int
        RemapPropertyAssociationToken: int -> int
        RemapEventAssociationToken: int -> int
    }

type private DefinitionTokenRemapContext =
    {
        TypeTokenMap: Dictionary<int, int>
        FieldTokenMap: Dictionary<int, int>
        MethodTokenMap: Dictionary<int, int>
        PropertyTokenMap: Dictionary<int, int>
        EventTokenMap: Dictionary<int, int>
    }

let private createDefinitionTokenRemapper (context: DefinitionTokenRemapContext) : DefinitionTokenRemapper =
    let inline remapWith (dict: Dictionary<int, int>) token =
        match dict.TryGetValue token with
        | true, mapped -> mapped
        | _ -> token

    let remapDefinitionToken token =
        match classifyEntityTokenRemapKind token with
        | EntityTokenRemapKind.TypeDef -> remapWith context.TypeTokenMap token
        | EntityTokenRemapKind.FieldDef -> remapWith context.FieldTokenMap token
        | EntityTokenRemapKind.MethodDef -> remapWith context.MethodTokenMap token
        | EntityTokenRemapKind.Event -> remapWith context.EventTokenMap token
        | EntityTokenRemapKind.Property -> remapWith context.PropertyTokenMap token
        | _ -> token

    {
        RemapDefinitionToken = remapDefinitionToken
        RemapPropertyAssociationToken = remapWith context.PropertyTokenMap
        RemapEventAssociationToken = remapWith context.EventTokenMap
    }

type private MetadataReferenceRemapper =
    {
        RemapEntityToken: int -> int
        RemapAssemblyRefToken: int -> int
    }

type private MetadataReferenceRemapContext =
    {
        MetadataReader: MetadataReader
        TraceMetadata: bool
        BaselineMemberRefRowCount: int
        /// Baseline MemberRef row contents by row id (content-validated passthrough);
        /// empty for legacy baselines without byte-derived row snapshots.
        BaselineMemberRefRows: Map<int, HotReloadBaseline.BaselineMemberRefRow>
        /// Baseline TypeSpec signature blobs by row id; empty for legacy baselines.
        BaselineTypeSpecSignatures: Map<int, byte[]>
        BaselineTypeSpecRowCount: int
        TypeSpecTokenMap: Dictionary<int, int>
        TypeSpecificationRows: ResizeArray<TypeSpecificationRowInfo>
        TryReuseBaselineTypeRef: TypeReferenceKey -> int option
        TryReuseBaselineAssemblyRef: AssemblyReferenceKey -> int option
        RemapDefinitionToken: int -> int
        TypeReferenceRows: ResizeArray<TypeReferenceRowInfo>
        MemberReferenceRows: ResizeArray<MemberReferenceRowInfo>
        AssemblyReferenceRows: ResizeArray<AssemblyReferenceRowInfo>
        AddedTypeReferenceTokens: Dictionary<TypeReferenceKey, int>
        AddedAssemblyReferenceTokens: Dictionary<AssemblyReferenceKey, int>
        TypeRefTokenMap: Dictionary<int, int>
        AssemblyRefTokenMap: Dictionary<int, int>
        MemberRefTokenMap: Dictionary<int, int>
        MethodSpecTokenMap: Dictionary<int, int>
        MethodSpecRowsByToken: Dictionary<int, MethodSpecificationRowInfo>
        GetNextTypeRefRowId: unit -> int
        SetNextTypeRefRowId: int -> unit
        GetNextMemberRefRowId: unit -> int
        SetNextMemberRefRowId: int -> unit
        GetNextAssemblyRefRowId: unit -> int
        SetNextAssemblyRefRowId: int -> unit
        GetNextMethodSpecRowId: unit -> int
        SetNextMethodSpecRowId: int -> unit
        GetNextTypeSpecRowId: unit -> int
        SetNextTypeSpecRowId: int -> unit
    }

let private createMetadataReferenceRemapper (context: MetadataReferenceRemapContext) : MetadataReferenceRemapper =
    let metadataReader = context.MetadataReader
    let traceMetadata = context.TraceMetadata

    let rec remapAssemblyRefToken token =
        match context.AssemblyRefTokenMap.TryGetValue token with
        | true, mapped -> mapped
        | _ ->
            let handle = MetadataTokens.AssemblyReferenceHandle token
            let row = metadataReader.GetAssemblyReference handle

            let name =
                if row.Name.IsNil then
                    ""
                else
                    metadataReader.GetString row.Name

            let key = assemblyReferenceKey metadataReader row

            match context.TryReuseBaselineAssemblyRef key with
            | Some reused ->
                context.AssemblyRefTokenMap[token] <- reused
                reused
            | None ->
                let nextRowId = context.GetNextAssemblyRefRowId() + 1
                context.SetNextAssemblyRefRowId nextRowId

                let getBlob (blob: BlobHandle) =
                    if blob.IsNil then
                        Array.empty
                    else
                        metadataReader.GetBlobBytes blob

                let info =
                    {
                        RowId = nextRowId
                        Version = row.Version
                        Flags = row.Flags
                        PublicKeyOrToken = getBlob row.PublicKeyOrToken
                        PublicKeyOrTokenOffset = None
                        Name = name
                        NameOffset = None
                        Culture =
                            if row.Culture.IsNil then
                                None
                            else
                                Some(metadataReader.GetString row.Culture)
                        CultureOffset = None
                        HashValue = getBlob row.HashValue
                        HashValueOffset = None
                    }

                context.AssemblyReferenceRows.Add info
                let deltaToken = 0x23000000 ||| nextRowId
                context.AssemblyRefTokenMap[token] <- deltaToken
                context.AddedAssemblyReferenceTokens[key] <- deltaToken
                deltaToken

    and tryTypeRefKey (handle: TypeReferenceHandle) (depth: int) : TypeReferenceKey option =
        // Build the full typed scope chain for a TypeRef in the freshly emitted metadata so it can
        // be matched against the baseline table by identity. Nested TypeRefs recurse into their
        // enclosing TypeRef; depth-guard against malformed metadata cycles.
        if depth > 64 then
            None
        else
            let row = metadataReader.GetTypeReference handle

            let name =
                if row.Name.IsNil then
                    ""
                else
                    metadataReader.GetString row.Name

            let namespaceName =
                if row.Namespace.IsNil then
                    ""
                else
                    metadataReader.GetString row.Namespace

            let scopeOpt =
                match row.ResolutionScope.Kind with
                | HandleKind.AssemblyReference ->
                    let assemblyHandle = AssemblyReferenceHandle.op_Explicit row.ResolutionScope
                    let assemblyRef = metadataReader.GetAssemblyReference assemblyHandle
                    Some(TypeReferenceScope.Assembly(assemblyReferenceKey metadataReader assemblyRef))
                | HandleKind.TypeReference ->
                    tryTypeRefKey (TypeReferenceHandle.op_Explicit row.ResolutionScope) (depth + 1)
                    |> Option.map TypeReferenceScope.Nested
                | _ ->
                    // Module/ModuleRef scopes have no stable cross-compilation identity; fall through
                    // to the add-new-row path.
                    None

            scopeOpt
            |> Option.map (fun scope ->
                {
                    TypeReferenceKey.Scope = scope
                    Namespace = namespaceName
                    Name = name
                })

    and remapTypeRefToken token =
        match context.TypeRefTokenMap.TryGetValue token with
        | true, mapped -> mapped
        | _ ->
            let handle = MetadataTokens.TypeReferenceHandle token
            let row = metadataReader.GetTypeReference handle

            let name =
                if row.Name.IsNil then
                    ""
                else
                    metadataReader.GetString row.Name

            let namespaceName =
                if row.Namespace.IsNil then
                    ""
                else
                    metadataReader.GetString row.Namespace

            let baselineToken =
                tryTypeRefKey handle 0 |> Option.bind context.TryReuseBaselineTypeRef

            match baselineToken with
            | Some reused ->
                context.TypeRefTokenMap[token] <- reused
                reused
            | None ->
                if traceMetadata then
                    printfn
                        "[fsharp-hotreload][metadata] remap typeref miss scope=%A ns=%s name=%s"
                        row.ResolutionScope.Kind
                        namespaceName
                        name

                let resolutionScope =
                    if row.ResolutionScope.IsNil then
                        RS_Module(ModuleHandle 1)
                    else
                        let scopeToken = MetadataTokens.GetToken(row.ResolutionScope)

                        match row.ResolutionScope.Kind with
                        | HandleKind.AssemblyReference ->
                            let mapped = remapAssemblyRefToken scopeToken
                            RS_AssemblyRef(AssemblyRefHandle(mapped &&& 0x00FFFFFF))
                        | HandleKind.TypeReference ->
                            let mapped = remapTypeRefToken scopeToken
                            RS_TypeRef(TypeRefHandle(mapped &&& 0x00FFFFFF))
                        | HandleKind.ModuleDefinition ->
                            let rowId = MetadataTokens.GetRowNumber row.ResolutionScope
                            RS_Module(ModuleHandle rowId)
                        | HandleKind.ModuleReference ->
                            let rowId = MetadataTokens.GetRowNumber row.ResolutionScope
                            RS_ModuleRef(ModuleRefHandle rowId)
                        | _ -> RS_Module(ModuleHandle 1)

                let nextRowId = context.GetNextTypeRefRowId() + 1
                context.SetNextTypeRefRowId nextRowId

                context.TypeReferenceRows.Add(
                    {
                        RowId = nextRowId
                        ResolutionScope = resolutionScope
                        Name = name
                        NameOffset = None
                        Namespace = namespaceName
                        NamespaceOffset = None
                    }
                )

                let deltaToken = 0x01000000 ||| nextRowId
                context.TypeRefTokenMap[token] <- deltaToken

                match tryTypeRefKey handle 0 with
                | Some key -> context.AddedTypeReferenceTokens[key] <- deltaToken
                | None -> ()

                deltaToken

    and remapMemberRefToken token =
        match context.MemberRefTokenMap.TryGetValue token with
        | true, mapped -> mapped
        | _ ->
            let rowId = token &&& 0x00FFFFFF
            let handle = MetadataTokens.MemberReferenceHandle token
            let row = metadataReader.GetMemberReference handle

            let name =
                if row.Name.IsNil then
                    ""
                else
                    metadataReader.GetString row.Name

            // Remap the parent and signature into baseline coordinates first: they are
            // needed both for content validation of a positional passthrough and for an
            // appended delta row.
            let remappedParentToken =
                if row.Parent.IsNil then
                    0x01000001
                else
                    let parentToken = MetadataTokens.GetToken(row.Parent)

                    match row.Parent.Kind with
                    | HandleKind.TypeDefinition
                    | HandleKind.MethodDefinition -> context.RemapDefinitionToken parentToken
                    | HandleKind.TypeReference -> remapTypeRefToken parentToken
                    | HandleKind.TypeSpecification -> remapTypeSpecToken parentToken
                    | HandleKind.ModuleReference -> parentToken
                    | _ -> 0x01000001

            let signature =
                if row.Signature.IsNil then
                    Array.empty
                else
                    // Signature blobs embed TypeDefOrRef coded indexes of the fresh compile;
                    // remap them to baseline rows before comparing or appending.
                    metadataReader.GetBlobBytes row.Signature |> remapSignature

            let signaturesEqual (a: byte[]) (b: byte[]) =
                not (isNull (box a))
                && not (isNull (box b))
                && System.MemoryExtensions.SequenceEqual(System.Span.op_Implicit (a.AsSpan()), System.Span.op_Implicit (b.AsSpan()))

            let matchesBaselineRow (baselineRow: HotReloadBaseline.BaselineMemberRefRow) =
                String.Equals(baselineRow.Name, name, StringComparison.Ordinal)
                && baselineRow.ParentToken = remappedParentToken
                && signaturesEqual baselineRow.Signature signature

            // 1. Positional passthrough, content-validated when the baseline row snapshot is
            //    available: the fresh in-memory compile's MemberRef row order can shift
            //    relative to the baseline (an added lambda changes the order of first use),
            //    so a row id is only trusted when its content matches the baseline row.
            //    Legacy baselines (no snapshot) keep the historical positional behavior.
            let positionalReuse =
                if rowId > 0 && rowId <= context.BaselineMemberRefRowCount then
                    if Map.isEmpty context.BaselineMemberRefRows then
                        Some token
                    else
                        match context.BaselineMemberRefRows.TryFind rowId with
                        | Some baselineRow when matchesBaselineRow baselineRow -> Some token
                        | _ -> None
                else
                    None

            // 2. Content search: the same member may exist in the baseline at a DIFFERENT
            //    row id (shifted order); reuse it only when exactly one row matches.
            let contentReuse () =
                let matches =
                    context.BaselineMemberRefRows
                    |> Map.toSeq
                    |> Seq.filter (fun (_, baselineRow) -> matchesBaselineRow baselineRow)
                    |> Seq.truncate 2
                    |> Seq.toList

                match matches with
                | [ (baselineRowId, _) ] -> Some(0x0A000000 ||| baselineRowId)
                | _ -> None

            let mapped =
                match positionalReuse with
                | Some mapped -> mapped
                | None ->
                    match contentReuse () with
                    | Some mapped -> mapped
                    | None ->
                        // 3. Append a new MemberRef row to the delta (duplicates of baseline
                        //    rows are legal; correctness only needs the remapped parent/signature).
                        let parent =
                            let parentRowId = remappedParentToken &&& 0x00FFFFFF

                            match remappedParentToken &&& 0xFF000000 with
                            | 0x02000000 -> MRP_TypeDef(TypeDefHandle parentRowId)
                            | 0x01000000 -> MRP_TypeRef(TypeRefHandle parentRowId)
                            | 0x1A000000 -> MRP_ModuleRef(ModuleRefHandle parentRowId)
                            | 0x06000000 -> MRP_MethodDef(MethodDefHandle parentRowId)
                            | 0x1B000000 -> MRP_TypeSpec(TypeSpecHandle parentRowId)
                            | _ -> MRP_TypeRef(TypeRefHandle 1)

                        let nextRowId = context.GetNextMemberRefRowId() + 1
                        context.SetNextMemberRefRowId nextRowId

                        context.MemberReferenceRows.Add(
                            {
                                RowId = nextRowId
                                Parent = parent
                                Name = name
                                NameOffset = None
                                Signature = signature
                                SignatureOffset = None
                            }
                        )

                        0x0A000000 ||| nextRowId

            context.MemberRefTokenMap[token] <- mapped

            if traceMetadata then
                printfn "[fsharp-hotreload][metadata] remap memberref token=0x%08X -> 0x%08X name=%s" token mapped name

            mapped

    and remapTypeSpecToken token =
        match context.TypeSpecTokenMap.TryGetValue token with
        | true, mapped -> mapped
        | _ ->
            let rowId = token &&& 0x00FFFFFF

            // Legacy baselines without TypeSpec snapshots keep the historical positional
            // passthrough; validated baselines compare the remapped fresh signature blob
            // against the baseline row (and search for the instantiation elsewhere in the
            // baseline when the row order shifted). An empty snapshot with a zero row
            // count is NOT legacy — the baseline genuinely has no TypeSpec rows, so every
            // fresh TypeSpec is a new instantiation and must be appended.
            let mapped =
                if
                    Map.isEmpty context.BaselineTypeSpecSignatures
                    && context.BaselineTypeSpecRowCount > 0
                then
                    token
                else
                    let handle = MetadataTokens.TypeSpecificationHandle rowId
                    let spec = metadataReader.GetTypeSpecification handle

                    let signature =
                        if spec.Signature.IsNil then
                            Array.empty
                        else
                            metadataReader.GetBlobBytes spec.Signature |> remapTypeSpecBlob

                    let blobsEqual (a: byte[]) (b: byte[]) =
                        a.Length = b.Length
                        && System.MemoryExtensions.SequenceEqual(System.Span.op_Implicit (a.AsSpan()), System.Span.op_Implicit (b.AsSpan()))

                    let positionalMatch =
                        rowId > 0
                        && rowId <= context.BaselineTypeSpecRowCount
                        && (match context.BaselineTypeSpecSignatures.TryFind rowId with
                            | Some baselineBlob -> blobsEqual baselineBlob signature
                            | None -> false)

                    if positionalMatch then
                        token
                    else
                        let matches =
                            context.BaselineTypeSpecSignatures
                            |> Map.toSeq
                            |> Seq.filter (fun (_, blob) -> blobsEqual blob signature)
                            |> Seq.truncate 2
                            |> Seq.toList

                        match matches with
                        | [ (baselineRowId, _) ] -> 0x1B000000 ||| baselineRowId
                        | _ ->
                            // Genuinely new generic instantiation: append a TypeSpec row to
                            // the delta (C# reference template parity — Roslyn's added-lambda
                            // delta carries "TypeSpec 0x1b00xxxx Default"). The signature blob
                            // is already remapped to baseline coordinates; the new row id
                            // chains forward through the next-generation baseline snapshot so
                            // later compiles content-match it (mirrors appended MemberRef rows).
                            let nextRowId = context.GetNextTypeSpecRowId() + 1
                            context.SetNextTypeSpecRowId nextRowId

                            context.TypeSpecificationRows.Add(
                                {
                                    TypeSpecificationRowInfo.RowId = nextRowId
                                    Signature = signature
                                    SignatureOffset = None
                                }
                            )

                            if traceMetadata then
                                let hex (bytes: byte[]) =
                                    bytes |> Array.map (sprintf "%02X") |> String.concat "-"

                                printfn "[fsharp-hotreload][typespec-add] token=0x%08X rowId=%d blob=%s" token nextRowId (hex signature)

                            0x1B000000 ||| nextRowId

            context.TypeSpecTokenMap[token] <- mapped

            if traceMetadata && mapped <> token then
                printfn "[fsharp-hotreload][metadata] remap typespec token=0x%08X -> 0x%08X" token mapped

            mapped

    and remapMethodSpecToken token =
        match context.MethodSpecTokenMap.TryGetValue token with
        | true, mapped -> mapped
        | _ ->
            let methodSpecHandle = MetadataTokens.MethodSpecificationHandle token

            if methodSpecHandle.IsNil then
                token
            else
                let methodSpec = metadataReader.GetMethodSpecification methodSpecHandle
                let originalMethodToken = MetadataTokens.GetToken(methodSpec.Method)
                let remappedMethodToken = remapEntityToken originalMethodToken

                let methodDefOrRef =
                    let rowId = remappedMethodToken &&& 0x00FFFFFF

                    match remappedMethodToken &&& 0xFF000000 with
                    | 0x06000000 -> Some(MDOR_MethodDef(MethodDefHandle rowId))
                    | 0x0A000000 -> Some(MDOR_MemberRef(MemberRefHandle rowId))
                    | _ -> None

                match methodDefOrRef with
                | None ->
                    if traceMetadata then
                        printfn
                            "[fsharp-hotreload][metadata] keeping methodspec token=0x%08X (unsupported remapped method token=0x%08X)"
                            token
                            remappedMethodToken

                    token
                | Some method ->
                    let rowId = context.GetNextMethodSpecRowId() + 1
                    context.SetNextMethodSpecRowId rowId

                    let signature =
                        if methodSpec.Signature.IsNil then
                            Array.empty
                        else
                            // The instantiation blob embeds TypeDefOrRef coded indexes of the fresh
                            // compile; remap them so the delta resolves against the baseline tables.
                            metadataReader.GetBlobBytes methodSpec.Signature |> remapSignature

                    let row =
                        {
                            MethodSpecificationRowInfo.RowId = rowId
                            Method = method
                            Signature = signature
                            SignatureOffset = None
                        }

                    let mapped = 0x2B000000 ||| rowId
                    context.MethodSpecTokenMap[token] <- mapped
                    context.MethodSpecRowsByToken[mapped] <- row

                    if traceMetadata then
                        printfn "[fsharp-hotreload][metadata] remap methodspec token=0x%08X -> 0x%08X" token mapped

                    mapped

    and remapEntityToken token =
        match classifyEntityTokenRemapKind token with
        | EntityTokenRemapKind.TypeDef
        | EntityTokenRemapKind.FieldDef
        | EntityTokenRemapKind.MethodDef
        | EntityTokenRemapKind.Event
        | EntityTokenRemapKind.Property -> context.RemapDefinitionToken token
        | EntityTokenRemapKind.MemberRef -> remapMemberRefToken token
        | EntityTokenRemapKind.MethodSpec -> remapMethodSpecToken token
        | EntityTokenRemapKind.TypeRef -> remapTypeRefToken token
        | EntityTokenRemapKind.TypeSpec -> remapTypeSpecToken token
        | EntityTokenRemapKind.AssemblyRef -> remapAssemblyRefToken token
        | EntityTokenRemapKind.Passthrough -> token

    and remapSignature (signature: byte[]) : byte[] =
        remapSignatureBlobWith (remapTypeDefOrRefCodedIndexWith remapEntityToken) signature

    and remapTypeSpecBlob (blob: byte[]) : byte[] =
        remapTypeSpecBlobWith (remapTypeDefOrRefCodedIndexWith remapEntityToken) blob

    {
        RemapEntityToken = remapEntityToken
        RemapAssemblyRefToken = remapAssemblyRefToken
    }

/// Emits the delta artifacts for a request. The current implementation populates token projections
/// while leaving the raw metadata/IL/PDB payload empty; future work will replace the placeholders
/// with fully emitted heaps.
///
/// <paramref name="freshDebugPdb"/> carries the FRESH compile's on-disk portable PDB when the
/// caller has one and no emitted-artifact PDB is available. <c>request.EmittedArtifacts</c>
/// carries bytes and token mappings from the real fresh compile write when the session compiled
/// in-process. When neither is present, the emitter falls back to the legacy in-memory rewrite
/// (callers that construct modules with debug points, e.g. the component tests, need no sibling file).
let emitDeltaWithDebugData (freshDebugPdb: byte[] option) (request: IlxDeltaRequest) : IlxDelta =
    let usesRecordedSynthesizedSnapshot =
        request.SynthesizedNames |> Option.exists (fun map -> map.UsesRecordedSnapshot)

    let synthesizedBuckets =
        request.SynthesizedNames
        |> Option.map (fun map -> map.Snapshot |> Seq.map (fun struct (basic, names) -> basic, names) |> dict)

    let symbolMatcher =
        match request.SynthesizedNames with
        | Some map -> FSharpSymbolMatcher.createWithSynthesizedNames request.Module map
        | None -> FSharpSymbolMatcher.create request.Module

    let symbolChangeTypeNames =
        request.SymbolChanges
        |> Option.map FSharpSymbolChanges.entitySymbolsWithChanges
        |> Option.defaultValue []
        |> List.map (fun symbol -> symbol.QualifiedName)

    // User-defined types ADDED by this edit: classification gates them on
    // NewTypeDefinition and projects their IL names into the added-entity symbols. The
    // symbol path is dot-joined while IL nests with '+', so matching normalizes both
    // sides to dot-separated segments.
    let normalizeTypePathName (name: string) =
        name.Split([| '.'; '+' |], StringSplitOptions.RemoveEmptyEntries)
        |> String.concat "."

    let addedUserTypeNames =
        request.SymbolChanges
        |> Option.map FSharpSymbolChanges.addedEntitySymbols
        |> Option.defaultValue []
        |> List.map (fun symbol -> normalizeTypePathName symbol.QualifiedName)
        |> Set.ofList

    let cleanUpGeneratedTypeName (name: string) =
        if name.IndexOfAny IllegalCharactersInTypeAndNamespaceNames = -1 then
            name
        else
            (name, IllegalCharactersInTypeAndNamespaceNames)
            ||> Array.fold (fun acc c -> acc.Replace(string c, "-"))

    let updatedMethodNameBases =
        request.UpdatedMethods
        |> List.map (fun methodKey -> cleanUpGeneratedTypeName methodKey.Name)
        |> Set.ofList

    let isResumableCodeShape (shape: SynthesizedTypeShape) =
        let mentionsResumableCode (text: string) =
            text.IndexOf("Microsoft.FSharp.Core.CompilerServices.ResumableCode", StringComparison.Ordinal)
            >= 0

        shape.BaseType |> Option.exists mentionsResumableCode
        || shape.InterfaceTypes |> List.exists mentionsResumableCode
        || shape.FieldTypeNames |> List.exists mentionsResumableCode

    let isUpdatedMethodResumableCodeHelper (typeDef: ILTypeDef) =
        if IsCompilerGeneratedName typeDef.Name then
            let basicName = GetBasicNameOfPossibleCompilerGeneratedName typeDef.Name

            Set.contains basicName updatedMethodNameBases
            && (typeDef |> shapeOfSynthesizedTypeDef |> isResumableCodeShape)
        else
            false

    let builder =
        IlDeltaStreamBuilder(
            request.Baseline.Metadata.HeapSizes.UserStringHeapSize,
            request.Baseline.Metadata.TableRowCounts.[TableNames.StandAloneSig.Index]
        )

    if traceHeapOffsets.Value then
        let heaps = request.Baseline.Metadata.HeapSizes

        printfn
            "[fsharp-hotreload][heap-offsets] Generation %d - Baseline heap sizes passed to IlDeltaStreamBuilder:"
            request.CurrentGeneration

        printfn "[fsharp-hotreload][heap-offsets]   UserStringHeapSize = %d" heaps.UserStringHeapSize
        printfn "[fsharp-hotreload][heap-offsets]   StringHeapSize = %d" heaps.StringHeapSize
        printfn "[fsharp-hotreload][heap-offsets]   BlobHeapSize = %d" heaps.BlobHeapSize
        printfn "[fsharp-hotreload][heap-offsets]   GuidHeapSize = %d" heaps.GuidHeapSize
        printfn "[fsharp-hotreload][heap-offsets]   NextGeneration = %d, EncId = %A" request.Baseline.NextGeneration request.Baseline.EncId

    let baselineTypeTokens = request.Baseline.TypeTokens

    let primaryScopeRef =
        match request.Module.Manifest with
        | Some manifest ->
            let publicKey =
                manifest.PublicKey |> Option.map (fun key -> PublicKey.KeyAsToken key)

            let asmRef =
                ILAssemblyRef.Create(manifest.Name, None, publicKey, manifest.Retargetable, manifest.Version, manifest.Locale)

            ILScopeRef.Assembly asmRef
        | None -> ILScopeRef.PrimaryAssembly

    let fsharpCoreScopeRef =
        ILScopeRef.Assembly(ILAssemblyRef.Create("FSharp.Core", None, None, false, None, None))

    let ilg = mkILGlobals (primaryScopeRef, [], fsharpCoreScopeRef)

    let writerOptions = defaultWriterOptions ilg HashAlgorithm.Sha256

    let assemblyBytes, pdbBytesOpt, emittedTokenMappings =
        match request.EmittedArtifacts with
        | Some artifacts -> artifacts.AssemblyBytes, artifacts.PdbBytes, artifacts.TokenMappings
        | None ->
            let assemblyBytes, pdbBytesOpt, emittedTokenMappings, _ =
                ILWriter.WriteILBinaryInMemoryWithArtifacts(writerOptions, request.Module, id)

            assemblyBytes, pdbBytesOpt, emittedTokenMappings

    // The fresh compile's debug data: the caller-supplied on-disk PDB when available, else the
    // artifact or rewrite PDB. Feeds the sequence-point analysis and the PDB delta.
    let effectiveDebugPdb = freshDebugPdb |> Option.orElse pdbBytesOpt

    if traceUserStringUpdates.Value then
        try
            let tempDll =
                Path.Combine(Path.GetTempPath(), $"fsharp-hotreload-ilmodule-{System.Guid.NewGuid():N}.dll")

            File.WriteAllBytes(tempDll, assemblyBytes)
            printfn "[fsharp-hotreload][trace] wrote IL module snapshot to %s" tempDll
        with ex ->
            printfn "[fsharp-hotreload][trace] failed to write IL module snapshot: %s" ex.Message

    use peStream = new MemoryStream(assemblyBytes, writable = false)
    use peReader = new PEReader(peStream)
    let metadataReader = peReader.GetMetadataReader()
    let moduleDef = metadataReader.GetModuleDefinition()
    let moduleName = metadataReader.GetString moduleDef.Name
    let baselineModuleNameOffset = request.Baseline.ModuleNameOffset
    let userStringCalculator = builder.UserStringCalculator
    let stringTokenCache = Dictionary<int, int>()
    let userStringUpdates = ResizeArray<int * int * string>()

    let logUserString originalToken newToken text =
        if traceUserStringUpdates.Value then
            printfn "[fsharp-hotreload][userstring] original=0x%08X new=0x%08X text=%s" originalToken newToken text

    let remapUserString token =
        match stringTokenCache.TryGetValue token with
        | true, mapped -> mapped
        | _ ->
            let handle = MetadataTokens.UserStringHandle token
            let value = metadataReader.GetUserString handle
            let newToken = userStringCalculator.GetOrAddUserString value
            stringTokenCache[token] <- newToken
            userStringUpdates.Add((token, newToken, value))
            logUserString token newToken value
            newToken

    let typeTokenMap = Dictionary<int, int>()
    let fieldTokenMap = Dictionary<int, int>()
    let methodTokenMap = Dictionary<int, int>()
    // Every fresh-compile method paired with its baseline (or delta-added) token, INCLUDING
    // identity pairs: methodTokenMap deliberately records only tokens that DIFFER (its consumers
    // remap IL), but the sequence-point analysis must see every matched method —
    // unchanged compiles keep identical tokens.
    let matchedMethodTokenPairs = Dictionary<int, int>()
    let propertyTokenMap = Dictionary<int, int>()
    let eventTokenMap = Dictionary<int, int>()

    let addedMethodTokens =
        Dictionary<MethodDefinitionKey, int>(HashIdentity.Structural)

    let addedFieldTokens = Dictionary<FieldDefinitionKey, int>(HashIdentity.Structural)
    // ADDED type definitions (closure classes synthesized for lambdas added in
    // a delta compile). Keyed by the fresh compile's full type name — added types have no
    // baseline alias, so the new name IS the chained baseline name. The machinery is
    // general (rows, EncLog shape, chaining); only the *detection* below is closure-scoped.
    let addedTypeNewTokens = Dictionary<string, int>(StringComparer.Ordinal)
    let addedTypeDeltaTokens = Dictionary<string, int>(StringComparer.Ordinal)
    let addedTypeDefs = ResizeArray<ILTypeDef list * ILTypeDef * string>()

    let addedTypeShapes =
        Dictionary<string, SynthesizedTypeShape>(StringComparer.Ordinal)

    let addedTypeKeyByFreshFullName = Dictionary<string, string>(StringComparer.Ordinal)
    let freshFullNameByAddedTypeKey = Dictionary<string, string>(StringComparer.Ordinal)
    let addedTypeMetadataNameByKey = Dictionary<string, string>(StringComparer.Ordinal)

    let baselineTypeDefRowCount =
        request.Baseline.Metadata.TableRowCounts.[TableNames.TypeDef.Index]

    let addedPropertyTokens =
        Dictionary<PropertyDefinitionKey, int>(HashIdentity.Structural)

    let addedEventTokens = Dictionary<EventDefinitionKey, int>(HashIdentity.Structural)
    let addedPropertyTokenLookup = Dictionary<int, PropertyDefinitionKey>()
    let addedEventTokenLookup = Dictionary<int, EventDefinitionKey>()

    let propertyHandleLookup =
        Dictionary<PropertyDefinitionKey, PropertyDefinitionHandle>()

    let eventHandleLookup = Dictionary<EventDefinitionKey, EventDefinitionHandle>()
    let baselineTypeNameByNew = Dictionary<string, string>(StringComparer.Ordinal)
    let forcedAddedTypeNames = HashSet<string>(StringComparer.Ordinal)
    // Reverse of baselineTypeNameByNew: the new->baseline synthesized type mapping must
    // stay INJECTIVE. Closure-chain CE lowerings (async) carry legacy `-N`-suffixed
    // names; a structural CE change shifts the numbering, and alias-bucket matching
    // could then silently pair two different fresh closures with one baseline class -
    // emitting fresh bodies into the wrong rows. Fail closed instead.
    let newTypeNameByBaseline = Dictionary<string, string>(StringComparer.Ordinal)

    let splitTypeFullName (fullName: string) =
        let nestedIndex = fullName.LastIndexOf('+')

        if nestedIndex >= 0 then
            fullName.Substring(0, nestedIndex), fullName.Substring(nestedIndex + 1)
        else
            let namespaceIndex = fullName.LastIndexOf('.')

            if namespaceIndex >= 0 then
                fullName.Substring(0, namespaceIndex), fullName.Substring(namespaceIndex + 1)
            else
                "", fullName

    let positionalGroupKey (info: PositionalTypeInfo) =
        info.EnclosingFullName, info.NormalizedBasicName

    let tryFreshPositionalInfo (enclosing: ILTypeDef list) (typeDef: ILTypeDef) =
        match tryNormalizeSynthesizedTypeNameForPositionalPairing typeDef.Name with
        | None -> None
        | Some normalized ->
            let typeRef = mkRefForNestedILTypeDef ILScopeRef.Local (enclosing, typeDef)

            let enclosingFullName =
                match enclosing with
                | [] -> ""
                | _ ->
                    let parentType = List.last enclosing
                    let parentEnclosing = enclosing |> List.take (List.length enclosing - 1)
                    (mkRefForNestedILTypeDef ILScopeRef.Local (parentEnclosing, parentType)).FullName

            Some
                {
                    FullName = typeRef.FullName
                    EnclosingFullName = enclosingFullName
                    NormalizedBasicName = normalized.NormalizedBasicName
                    Ordinal = normalized.Ordinal
                    Shape = shapeOfSynthesizedTypeDef typeDef
                }

    let collectFreshPositionalInfos () =
        let infos = ResizeArray<PositionalTypeInfo>()

        let rec visit (enclosing: ILTypeDef list) (typeDef: ILTypeDef) =
            match tryFreshPositionalInfo enclosing typeDef with
            | Some info -> infos.Add info
            | None -> ()

            typeDef.NestedTypes.AsList() |> List.iter (visit (enclosing @ [ typeDef ]))

        request.Module.TypeDefs.AsList() |> List.iter (visit [])
        infos |> Seq.toArray

    let collectBaselinePositionalInfos () =
        request.Baseline.SynthesizedTypeShapes
        |> Map.toSeq
        |> Seq.choose (fun (fullName, shape) ->
            let enclosingFullName, simpleName = splitTypeFullName fullName

            match tryNormalizeSynthesizedTypeNameForPositionalPairing simpleName with
            | None -> None
            | Some normalized ->
                Some
                    {
                        FullName = fullName
                        EnclosingFullName = enclosingFullName
                        NormalizedBasicName = normalized.NormalizedBasicName
                        Ordinal = normalized.Ordinal
                        Shape = shape
                    })
        |> Seq.toArray

    let hasDistinctOrdinals (items: PositionalTypeInfo[]) =
        let distinct = items |> Array.map (fun item -> item.Ordinal) |> Array.distinct

        distinct.Length = items.Length

    let tracePositionalRejection (enclosingFullName, normalizedBasicName) reason =
        if traceSynthesizedMappings.Value then
            printfn
                "[fsharp-hotreload][synthesized-map] positional group rejected enclosing=%s name=%s reason=%s"
                enclosingFullName
                normalizedBasicName
                reason

    let shapeTypeNameVariants (fullName: string) =
        let enclosingFullName, simpleName = splitTypeFullName fullName

        [
            fullName

            if not (String.IsNullOrEmpty enclosingFullName) then
                enclosingFullName + "+" + simpleName
                enclosingFullName + "." + simpleName
        ]
        |> List.distinct

    let freshTypeFullNames =
        lazy
            let names = HashSet<string>(StringComparer.Ordinal)

            let rec visit (enclosing: ILTypeDef list) (typeDef: ILTypeDef) =
                let typeRef = mkRefForNestedILTypeDef ILScopeRef.Local (enclosing, typeDef)

                for variant in shapeTypeNameVariants typeRef.FullName do
                    names.Add variant |> ignore

                typeDef.NestedTypes.AsList() |> List.iter (visit (enclosing @ [ typeDef ]))

            request.Module.TypeDefs.AsList() |> List.iter (visit [])
            names

    let normalizeSelfReferencesInShape (fullName: string) (shape: SynthesizedTypeShape) =
        let replacements = shapeTypeNameVariants fullName

        let normalizeText (text: string) =
            (text, replacements)
            ||> List.fold (fun acc replacement -> acc.Replace(replacement, "<self>"))

        { shape with
            BaseType = shape.BaseType |> Option.map normalizeText
            InterfaceTypes = shape.InterfaceTypes |> List.map normalizeText
            FieldTypeNames = shape.FieldTypeNames |> List.map normalizeText
            MethodNameAndArities = shape.MethodNameAndArities
        }

    let normalizeAliasReferencesInShape (fullNames: string[]) (shape: SynthesizedTypeShape) =
        let replacements =
            fullNames |> Array.toList |> List.collect shapeTypeNameVariants |> List.distinct

        let normalizeText (text: string) =
            (text, replacements)
            ||> List.fold (fun acc replacement -> acc.Replace(replacement, "<self>"))

        { shape with
            BaseType = shape.BaseType |> Option.map normalizeText
            InterfaceTypes = shape.InterfaceTypes |> List.map normalizeText
            FieldTypeNames = shape.FieldTypeNames |> List.map normalizeText
            MethodNameAndArities = shape.MethodNameAndArities
        }

    let tryFindPositionalShapeMismatch (baselineInfo: PositionalTypeInfo) (freshInfo: PositionalTypeInfo) =
        tryFindSynthesizedTypeShapeMismatch
            (normalizeSelfReferencesInShape baselineInfo.FullName baselineInfo.Shape)
            (normalizeSelfReferencesInShape freshInfo.FullName freshInfo.Shape)

    let positionalBaselineByFresh =
        let accepted = Dictionary<string, string * int>(StringComparer.Ordinal)
        let usedBaselineNames = HashSet<string>(StringComparer.Ordinal)

        let baselineGroups =
            collectBaselinePositionalInfos ()
            |> Array.groupBy positionalGroupKey
            |> Map.ofArray

        let freshGroups = collectFreshPositionalInfos () |> Array.groupBy positionalGroupKey

        for groupKey, freshGroup in freshGroups do
            match baselineGroups |> Map.tryFind groupKey with
            | None -> tracePositionalRejection groupKey "no baseline group"
            | Some baselineGroup ->
                if baselineGroup.Length <> freshGroup.Length then
                    tracePositionalRejection groupKey $"count mismatch baseline={baselineGroup.Length} fresh={freshGroup.Length}"
                elif not (hasDistinctOrdinals baselineGroup) then
                    tracePositionalRejection groupKey "baseline group has duplicate ordinals"
                elif not (hasDistinctOrdinals freshGroup) then
                    tracePositionalRejection groupKey "fresh group has duplicate ordinals"
                else
                    let baselineSorted = baselineGroup |> Array.sortBy (fun item -> item.Ordinal)
                    let freshSorted = freshGroup |> Array.sortBy (fun item -> item.Ordinal)

                    let shapeMismatch =
                        Array.zip baselineSorted freshSorted
                        |> Array.tryPick (fun (baselineInfo, freshInfo) ->
                            tryFindPositionalShapeMismatch baselineInfo freshInfo
                            |> Option.map (fun reason -> $"{baselineInfo.FullName} -> {freshInfo.FullName}: {reason}"))

                    match shapeMismatch with
                    | Some reason -> tracePositionalRejection groupKey reason
                    | None ->
                        let duplicateBaseline =
                            baselineSorted
                            |> Array.tryFind (fun baselineInfo -> usedBaselineNames.Contains baselineInfo.FullName)

                        match duplicateBaseline with
                        | Some baselineInfo -> tracePositionalRejection groupKey $"baseline type already paired: {baselineInfo.FullName}"
                        | None ->
                            for baselineInfo, freshInfo in Array.zip baselineSorted freshSorted do
                                match request.Baseline.TypeTokens |> Map.tryFind baselineInfo.FullName with
                                | Some baselineToken ->
                                    usedBaselineNames.Add baselineInfo.FullName |> ignore
                                    accepted[freshInfo.FullName] <- (baselineInfo.FullName, baselineToken)

                                    if traceSynthesizedMappings.Value then
                                        printfn
                                            "[fsharp-hotreload][synthesized-map] positional %s -> %s"
                                            freshInfo.FullName
                                            baselineInfo.FullName
                                | None -> tracePositionalRejection groupKey $"baseline token missing for {baselineInfo.FullName}"

        accepted

    let getAliasCandidates (typeName: string) =
        match synthesizedBuckets with
        // Generation-suffixed closure names (allocator format: {base}@hotreload#g{N}_o{i})
        // identify occurrences ADDED in a delta compile: they never alias a baseline
        // closure class, so basic-name bucket expansion must not apply (it would make
        // the new class ambiguously match the baseline closures sharing its base name).
        | Some _ when FSharp.Compiler.ClosureNameAllocator.isGenerationSuffixedClosureName typeName -> [| typeName |]
        | Some buckets when IsCompilerGeneratedName typeName ->
            let basicName = GetBasicNameOfPossibleCompilerGeneratedName typeName
            let mapKey = SynthesizedNameMapKey basicName

            match buckets.TryGetValue mapKey with
            | true, aliases when aliases.Length > 0 ->
                if aliases |> Array.exists (fun alias -> alias = typeName) then
                    Array.append
                        [| typeName |]
                        (aliases
                         |> Array.filter (fun alias -> not (String.Equals(alias, typeName, StringComparison.Ordinal))))
                else
                    Array.append [| typeName |] aliases
            | _ -> [| typeName |]
        | _ -> [| typeName |]

    let resolveBaselineTypeFullName (enclosing: ILTypeDef list) (typeDef: ILTypeDef) =
        let typeRef = mkRefForNestedILTypeDef ILScopeRef.Local (enclosing, typeDef)
        let newFullName = typeRef.FullName

        let parentBaselinePrefixOpt =
            match enclosing with
            | [] -> None
            | _ ->
                let parentType = List.last enclosing
                let parentEnclosing = enclosing |> List.take (List.length enclosing - 1)

                let parentRef =
                    mkRefForNestedILTypeDef ILScopeRef.Local (parentEnclosing, parentType)

                let parentBaseline =
                    match baselineTypeNameByNew.TryGetValue parentRef.FullName with
                    | true, baselineParent -> baselineParent
                    | _ -> parentRef.FullName

                Some(parentBaseline + "+")

        let basePrefix =
            match parentBaselinePrefixOpt with
            | Some prefix -> prefix
            | None ->
                let lastDot = newFullName.LastIndexOf('.')

                if lastDot >= 0 then
                    newFullName.Substring(0, lastDot + 1)
                else
                    ""

        let candidateNames =
            let aliases =
                if
                    usesRecordedSynthesizedSnapshot
                    && isUpdatedMethodResumableCodeHelper typeDef
                    && FSharp.Compiler.ClosureNameAllocator.isGenerationSuffixedClosureName typeDef.Name
                then
                    [| typeDef.Name |]
                else
                    getAliasCandidates typeDef.Name

            let prefixes =
                if basePrefix.EndsWith("+", StringComparison.Ordinal) then
                    let withoutPlus = basePrefix.Substring(0, basePrefix.Length - 1)

                    let dotPrefix =
                        if String.IsNullOrEmpty withoutPlus then
                            ""
                        else
                            withoutPlus + "."

                    [| basePrefix; dotPrefix |]
                else
                    [| basePrefix |]

            let projected =
                prefixes
                |> Array.collect (fun prefix ->
                    aliases
                    |> Array.map (fun alias ->
                        if
                            prefix.EndsWith("+", StringComparison.Ordinal)
                            || prefix.EndsWith(".", StringComparison.Ordinal)
                        then
                            prefix + alias
                        elif prefix = "" then
                            alias
                        else
                            prefix + alias))

            Array.concat
                [|
                    projected
                    prefixes
                    |> Array.collect (fun prefix ->
                        [|
                            if
                                prefix.EndsWith("+", StringComparison.Ordinal)
                                || prefix.EndsWith(".", StringComparison.Ordinal)
                            then
                                yield prefix + typeDef.Name
                            elif prefix = "" then
                                yield typeDef.Name
                            else
                                yield prefix + typeDef.Name
                        |])
                    [| newFullName |]
                |]
            |> Array.filter (fun name -> not (String.IsNullOrWhiteSpace name))
            |> Array.distinct

        let baselineMatches =
            candidateNames
            |> Array.choose (fun candidate ->
                match request.Baseline.TypeTokens |> Map.tryFind candidate with
                | Some token -> Some(candidate, token)
                | None -> None)
            |> Array.distinctBy fst

        let tryGetPositionalBaselineMatch () =
            match positionalBaselineByFresh.TryGetValue newFullName with
            | true, positionalMatch -> Some positionalMatch
            | _ -> None

        // Full in-process emits regenerate helpers outside the edited file. Several of those
        // helpers can receive the same synthesized-name aliases, but one baseline TypeDef token
        // may still be paired with only one fresh type. Recorded snapshots provide the complete
        // allocation order needed to choose the next alias. Restrict this recovery to the
        // whole-module in-process artifact path: external compiles retain their established
        // mapping behavior, including state-machine shape edits. Reconstructed legacy snapshots
        // keep the original fail-closed behavior as well.
        let availableBaselineMatches =
            if shouldFilterSynthesizedBaselineAliases usesRecordedSynthesizedSnapshot request.EmittedArtifacts.IsSome then
                filterAvailableBaselineTypeMatches newTypeNameByBaseline newFullName baselineMatches
            else
                baselineMatches

        let tryGetShapeCompatibleAliasMatch (matches: (string * int)[]) =
            let freshShape =
                shapeOfSynthesizedTypeDef typeDef
                |> normalizeAliasReferencesInShape candidateNames

            let availableMatches =
                matches
                |> Array.filter (fun (matchedName, _) ->
                    String.Equals(matchedName, newFullName, StringComparison.Ordinal)
                    || forcedAddedTypeNames.Contains matchedName
                    || not (freshTypeFullNames.Value.Contains matchedName))

            let shapeMatches =
                availableMatches
                |> Array.filter (fun (matchedName, _) ->
                    match request.Baseline.SynthesizedTypeShapes |> Map.tryFind matchedName with
                    | Some baselineShape ->
                        let baselineShape = baselineShape |> normalizeAliasReferencesInShape candidateNames

                        (tryFindSynthesizedTypeShapeMismatch baselineShape freshShape).IsNone
                    | None -> false)

            tryGetUniqueSynthesizedTypeMatch shapeMatches

        let shouldAddShapeChangedResumableHelper (matchedName: string) =
            isUpdatedMethodResumableCodeHelper typeDef
            && String.Equals(matchedName, newFullName, StringComparison.Ordinal)
            && match request.Baseline.SynthesizedTypeShapes |> Map.tryFind matchedName with
               | Some baselineShape ->
                   let freshShape = shapeOfSynthesizedTypeDef typeDef

                   (tryFindSynthesizedTypeShapeMismatch
                       (normalizeSelfReferencesInShape matchedName baselineShape)
                       (normalizeSelfReferencesInShape newFullName freshShape))
                       .IsSome
               | None -> false

        let markShapeChangedResumableHelperAdded () =
            forcedAddedTypeNames.Add newFullName |> ignore
            None

        let baselineNameOpt =
            match availableBaselineMatches with
            | [||] -> tryGetPositionalBaselineMatch ()
            | [| single |] ->
                if shouldAddShapeChangedResumableHelper (fst single) then
                    markShapeChangedResumableHelperAdded ()
                else
                    match tryGetPositionalBaselineMatch () with
                    | Some positionalMatch when
                        usesRecordedSynthesizedSnapshot
                        && IsCompilerGeneratedName typeDef.Name
                        && not (String.Equals(fst positionalMatch, fst single, StringComparison.Ordinal))
                        ->
                        Some positionalMatch
                    | _ -> Some single
            | matches ->
                let exactMatch =
                    matches
                    |> Array.tryFind (fun (matchedName, _) -> String.Equals(matchedName, newFullName, StringComparison.Ordinal))

                match exactMatch with
                | Some matchResult ->
                    if shouldAddShapeChangedResumableHelper (fst matchResult) then
                        markShapeChangedResumableHelperAdded ()
                    else
                        match tryGetPositionalBaselineMatch () with
                        | Some positionalMatch when
                            usesRecordedSynthesizedSnapshot
                            && IsCompilerGeneratedName typeDef.Name
                            && not (String.Equals(fst positionalMatch, fst matchResult, StringComparison.Ordinal))
                            ->
                            Some positionalMatch
                        | _ -> Some matchResult
                | None ->
                    let normalizeTypePath (name: string) =
                        name.Split([| '.'; '+' |], StringSplitOptions.RemoveEmptyEntries)
                        |> String.concat "."

                    let normalizedTarget = normalizeTypePath newFullName

                    let normalizedMatches =
                        matches
                        |> Array.filter (fun (matchedName, _) ->
                            String.Equals(normalizeTypePath matchedName, normalizedTarget, StringComparison.Ordinal))

                    match normalizedMatches with
                    | [| normalizedMatch |] -> Some normalizedMatch
                    | _ ->
                        match tryGetPositionalBaselineMatch () with
                        | Some positionalMatch -> Some positionalMatch
                        | None ->
                            match tryGetShapeCompatibleAliasMatch matches with
                            | Some shapeMatch -> Some shapeMatch
                            | None when isUpdatedMethodResumableCodeHelper typeDef -> markShapeChangedResumableHelperAdded ()
                            | None ->
                                let matchedNames = matches |> Array.map fst |> String.concat "; "
                                let allCandidates = candidateNames |> String.concat "; "

                                raise (
                                    HotReloadUnsupportedEditException(
                                        $"Ambiguous synthesized type mapping for '{newFullName}' (candidates=[{allCandidates}], baselineMatches=[{matchedNames}]); full rebuild required."
                                    )
                                )

        if traceSynthesizedMappings.Value then
            match baselineNameOpt with
            | Some(baselineName, _) when not (String.Equals(newFullName, baselineName, StringComparison.Ordinal)) ->
                printfn "[fsharp-hotreload][synthesized-map] %s -> %s" newFullName baselineName
            | None -> printfn "[fsharp-hotreload][synthesized-map] no baseline match for %s candidates=%A" newFullName candidateNames
            | _ -> ()

        let baselineName, baselineTokenOpt =
            match baselineNameOpt with
            | Some(baseline, token) -> baseline, Some token
            | None -> newFullName, None

        let baselineName, baselineTokenOpt =
            match baselineTokenOpt with
            | Some _ when
                isUpdatedMethodResumableCodeHelper typeDef
                && FSharp.Compiler.ClosureNameAllocator.isGenerationSuffixedClosureName typeDef.Name
                ->
                match newTypeNameByBaseline.TryGetValue baselineName with
                | true, existingNewName when not (String.Equals(existingNewName, newFullName, StringComparison.Ordinal)) ->
                    forcedAddedTypeNames.Add newFullName |> ignore
                    newFullName, None
                | _ -> baselineName, baselineTokenOpt
            | _ -> baselineName, baselineTokenOpt

        match baselineTokenOpt with
        | Some _ ->
            match newTypeNameByBaseline.TryGetValue baselineName with
            | true, existingNewName when not (String.Equals(existingNewName, newFullName, StringComparison.Ordinal)) ->
                raise (
                    HotReloadUnsupportedEditException(
                        $"Computation-expression closure chain changed: synthesized types '{existingNewName}' and '{newFullName}' both map to baseline type '{baselineName}'; the closure chain cannot be aligned with the baseline. Please rebuild."
                    )
                )
            | _ -> newTypeNameByBaseline[baselineName] <- newFullName
        | None -> ()

        baselineTypeNameByNew[newFullName] <- baselineName

        if traceSynthesizedMappings.Value then
            printfn "[fsharp-hotreload][synthesized-map] stored %s -> %s" newFullName baselineName

        baselineName, baselineTokenOpt

    let tryGetBaselineTypeName fullName =
        match addedTypeKeyByFreshFullName.TryGetValue fullName with
        | true, addedKey -> addedKey
        | _ ->
            match baselineTypeNameByNew.TryGetValue fullName with
            | true, baseline -> baseline
            | _ -> fullName

    let getAddedTypeKey fullName =
        match addedTypeKeyByFreshFullName.TryGetValue fullName with
        | true, addedKey -> addedKey
        | _ -> fullName

    let createAddedTypeKey (typeDef: ILTypeDef) fullName =
        match addedTypeKeyByFreshFullName.TryGetValue fullName with
        | true, addedKey -> addedKey
        | _ ->
            let addedKey, metadataNameOpt =
                if request.Baseline.TypeTokens |> Map.containsKey fullName then
                    let rec choose index =
                        let suffix = $"@hotreload-added{index}"
                        let candidate = fullName + suffix

                        if
                            (request.Baseline.TypeTokens |> Map.containsKey candidate)
                            || addedTypeDeltaTokens.ContainsKey candidate
                        then
                            choose (index + 1)
                        else
                            candidate, Some(typeDef.Name + suffix)

                    choose 1
                else
                    fullName, None

            addedTypeKeyByFreshFullName[fullName] <- addedKey
            freshFullNameByAddedTypeKey[addedKey] <- fullName

            match metadataNameOpt with
            | Some metadataName -> addedTypeMetadataNameByKey[addedKey] <- metadataName
            | None -> ()

            addedKey

    let tryResolveFreshTypeNameForKey declaringType =
        match freshFullNameByAddedTypeKey.TryGetValue declaringType with
        | true, freshFullName -> Some freshFullName
        | _ ->
            match newTypeNameByBaseline.TryGetValue declaringType with
            | true, freshFullName when not (String.Equals(freshFullName, declaringType, StringComparison.Ordinal)) -> Some freshFullName
            | _ -> None

    let tryGetBaselineTypeToken (enclosing: ILTypeDef list) (typeDef: ILTypeDef) =
        let typeRef = mkRefForNestedILTypeDef ILScopeRef.Local (enclosing, typeDef)
        let baselineName = tryGetBaselineTypeName typeRef.FullName
        baselineTypeTokens |> Map.tryFind baselineName

    let addMapping (dict: Dictionary<int, int>) newToken baselineToken =
        if newToken <> 0 && baselineToken <> 0 && newToken <> baselineToken then
            dict[newToken] <- baselineToken

    let addAddedTypeDefinition (enclosing: ILTypeDef list) (typeDef: ILTypeDef) fullName newTypeToken =
        let addedTypeKey = createAddedTypeKey typeDef fullName

        match addedTypeDeltaTokens.TryGetValue addedTypeKey with
        | true, token -> token
        | _ ->
            let rowId = baselineTypeDefRowCount + addedTypeDeltaTokens.Count + 1
            let deltaToken = 0x02000000 ||| rowId
            addedTypeDeltaTokens[addedTypeKey] <- deltaToken
            addedTypeNewTokens[addedTypeKey] <- newTypeToken
            addedTypeShapes[addedTypeKey] <- shapeOfSynthesizedTypeDef typeDef
            addedTypeDefs.Add(enclosing, typeDef, addedTypeKey)
            deltaToken

    let rec collectTypeMappings (enclosing: ILTypeDef list) (typeDef: ILTypeDef) =
        let newTypeToken = emittedTokenMappings.TypeDefTokenMap(enclosing, typeDef)

        let fullName =
            (mkRefForNestedILTypeDef ILScopeRef.Local (enclosing, typeDef)).FullName

        if traceSynthesizedMappings.Value then
            printfn "[fsharp-hotreload][synthesized-map] visiting %s" fullName

        let _, baselineTokenOpt = resolveBaselineTypeFullName enclosing typeDef

        let baselineTypeToken =
            match baselineTokenOpt with
            | Some token -> token
            | None when forcedAddedTypeNames.Contains fullName -> addAddedTypeDefinition enclosing typeDef fullName newTypeToken
            | None ->
                match tryGetBaselineTypeToken enclosing typeDef with
                | Some token -> token
                | None when FSharp.Compiler.ClosureNameAllocator.isGenerationSuffixedClosureName typeDef.Name ->
                    // ADDED closure class: the closure name allocator assigned this
                    // occurrence a `{base}@hotreload#g{N}_o{i}` name precisely because it
                    // has no baseline counterpart. Allocate the next delta TypeDef row;
                    // the type's fields/methods are registered as added members below,
                    // parented to this new row via AddField/AddMethod EncLog entries.
                    // Generic closure classes (closures inside generic members) are
                    // supported: the writer emits GenericParam rows for the
                    // added TypeDef (constrained typars still fail closed at row build).
                    let fullName =
                        (mkRefForNestedILTypeDef ILScopeRef.Local (enclosing, typeDef)).FullName

                    addAddedTypeDefinition enclosing typeDef fullName newTypeToken
                | None when isUpdatedMethodResumableCodeHelper typeDef ->
                    // Resumable-code helper classes are part of the task/class-state-machine
                    // lowering for the updated method. A complete recorded snapshot can put
                    // their replay names in the same bucket as older closure helpers; absence of
                    // an exact baseline TypeDef means this helper is new, not closure-chain
                    // evidence.
                    let fullName =
                        (mkRefForNestedILTypeDef ILScopeRef.Local (enclosing, typeDef)).FullName

                    addAddedTypeDefinition enclosing typeDef fullName newTypeToken
                | None when
                    (let fullName =
                        (mkRefForNestedILTypeDef ILScopeRef.Local (enclosing, typeDef)).FullName

                     addedUserTypeNames.Contains(normalizeTypePathName fullName)
                     || (match enclosing with
                         | [] -> false
                         | _ ->
                             // Types declared inside an ADDED type (union case classes,
                             // Tags holders, DebugTypeProxy companions) are part of the
                             // same addition; the enclosing type allocates first because
                             // collectTypeMappings visits parents before nested types.
                             let parentType = List.last enclosing
                             let parentEnclosing = enclosing |> List.take (List.length enclosing - 1)

                             let parentRef =
                                 mkRefForNestedILTypeDef ILScopeRef.Local (parentEnclosing, parentType)

                             addedTypeDeltaTokens.ContainsKey(getAddedTypeKey parentRef.FullName)))
                    ->
                    // ADDED user-defined type: classification gated the entity
                    // addition on NewTypeDefinition and projected its IL name into the
                    // added-entity symbols. The added-TypeDef machinery applies
                    // unchanged; the only difference is the detection.
                    let fullName =
                        (mkRefForNestedILTypeDef ILScopeRef.Local (enclosing, typeDef)).FullName

                    addAddedTypeDefinition enclosing typeDef fullName newTypeToken
                | None when IsCompilerGeneratedName typeDef.Name ->
                    // A compiler-generated type (closure/CE-lowering class, under either the
                    // stable `@hotreload...` naming scheme or the external/fresh-fsc `@<line>`
                    // scheme, e.g. `Pipe #1 stage #3 at line 16@16`) with no baseline
                    // counterpart. Closure-chain CE lowerings and pipe-stage lambdas number
                    // their classes by emission order, so a structural change (an added CE
                    // bind, an added pipe stage) shifts every later name off its baseline row.
                    // Tokens looked up through the baseline mappings would be garbage; fail
                    // closed before making a token-map call known to be invalid.
                    let fullName =
                        (mkRefForNestedILTypeDef ILScopeRef.Local (enclosing, typeDef)).FullName

                    raise (
                        HotReloadUnsupportedEditException(
                            $"Computation-expression closure chain changed: synthesized type '{fullName}' has no baseline counterpart; the closure chain cannot be aligned with the baseline. Please rebuild."
                        )
                    )
                | None -> request.Baseline.TokenMappings.TypeDefTokenMap(enclosing, typeDef)

        addMapping typeTokenMap newTypeToken baselineTypeToken

        typeDef.Fields.AsList()
        |> List.iter (fun fieldDef ->
            let declaringTypeRef = mkRefForNestedILTypeDef ILScopeRef.Local (enclosing, typeDef)
            let baselineDeclaringType = tryGetBaselineTypeName declaringTypeRef.FullName
            let declaringTypeIsAdded = addedTypeDeltaTokens.ContainsKey baselineDeclaringType

            let fieldKey: FieldDefinitionKey =
                {
                    DeclaringType = baselineDeclaringType
                    Name = fieldDef.Name
                    FieldType = fieldDef.FieldType
                }

            let baselineFieldTokenOpt =
                if declaringTypeIsAdded then
                    None
                else
                    match request.Baseline.FieldTokens |> Map.tryFind fieldKey with
                    | Some token -> Some token
                    | None ->
                        let sanitizedTarget = normalizeGeneratedFieldName fieldDef.Name

                        request.Baseline.FieldTokens
                        |> Map.tryPick (fun key token ->
                            if key.DeclaringType = baselineDeclaringType && key.FieldType = fieldDef.FieldType then
                                if normalizeGeneratedFieldName key.Name = sanitizedTarget then
                                    Some token
                                else
                                    None
                            else
                                None)

            match baselineFieldTokenOpt with
            | Some baselineFieldToken ->
                let newFieldToken =
                    emittedTokenMappings.FieldDefTokenMap (enclosing, typeDef) fieldDef

                addMapping fieldTokenMap newFieldToken baselineFieldToken
            | None when declaringTypeIsAdded ->
                // Field of an ADDED type (closure capture field / cached-instance field):
                // appended to the delta Field table, parented to the NEW TypeDef row. The
                // instance-field restriction below only applies to EXISTING types (the
                // runtime cannot re-layout them); a new type defines its own layout.
                if not (addedFieldTokens.ContainsKey fieldKey) then
                    let newFieldToken =
                        emittedTokenMappings.FieldDefTokenMap (enclosing, typeDef) fieldDef

                    addedFieldTokens[fieldKey] <- newFieldToken
            | None when
                not fieldDef.IsStatic
                && typeDef.IsStructOrEnum
                && request.Baseline.TypeTokens |> Map.containsKey baselineDeclaringType
                ->
                // A fresh instance field on an EXISTING struct: struct layouts are
                // immutable under hot reload. This must run BEFORE the synthesized-name
                // skip below - for compiler-generated structs the field is a state
                // machine's new awaiter or hoisted local (state-machine emission backstop: before this
                // gate, the skip silently dropped the field and the patched MoveNext
                // crashed at runtime with garbage field tokens).
                let fieldDisplay = $"{declaringTypeRef.FullName}::{fieldDef.Name}"

                let message =
                    if IsCompilerGeneratedName typeDef.Name then
                        $"Edit changes the layout of state machine struct '{declaringTypeRef.FullName}': field '{fieldDef.Name}' has no baseline counterpart (a resume point or hoisted local changed); struct layouts cannot change under hot reload. Please rebuild."
                    else
                        $"Edit adds instance field '{fieldDisplay}' to a struct; struct layouts cannot change under hot reload. Please rebuild."

                raise (HotReloadUnsupportedEditException message)
            | None when
                not fieldDef.IsStatic
                && not typeDef.IsStructOrEnum
                && request.Baseline.TypeTokens |> Map.containsKey baselineDeclaringType
                ->
                // A fresh instance field on an EXISTING compiler-generated CLASS: a class-form
                // resumable state machine (hot reload) that gained a hoisted local / awaiter
                // when a let!/do!/yield was added. Unlike a struct, a reference type CAN grow at
                // runtime (AddInstanceFieldToExistingType), so the field must be emitted rather
                // than dropped by the synthesized-name skip below - the patched MoveNext
                // references it, and dropping it crashes at runtime with an invalid field token.
                // (Must run before the synthesized-name skip and after the struct-layout gate.)
                if not (addedFieldTokens.ContainsKey fieldKey) then
                    let newFieldToken =
                        emittedTokenMappings.FieldDefTokenMap (enclosing, typeDef) fieldDef

                    addedFieldTokens[fieldKey] <- newFieldToken
            | None when synthesizedBuckets.IsSome && IsCompilerGeneratedName typeDef.Name -> ()
            | None when
                (fieldDef.IsStatic || not typeDef.IsStructOrEnum)
                && request.Baseline.TypeTokens |> Map.containsKey baselineDeclaringType
                ->
                // Added field on a type that exists in the baseline: static backing fields
                // for module-level values and instance fields on classes
                // (the CLR appends them; existing instances read default(T)).
                // The field is appended to the delta Field table; the parent TypeDef row is
                // logged with the AddField operation by the metadata writer. Struct layouts
                // cannot grow, so instance fields on structs fail closed below.
                if not (addedFieldTokens.ContainsKey fieldKey) then
                    let newFieldToken =
                        emittedTokenMappings.FieldDefTokenMap (enclosing, typeDef) fieldDef

                    addedFieldTokens[fieldKey] <- newFieldToken
            | None ->
                let fieldDisplay = $"{declaringTypeRef.FullName}::{fieldDef.Name}"

                let message =
                    if not fieldDef.IsStatic && typeDef.IsStructOrEnum then
                        $"Edit adds instance field '{fieldDisplay}' to a struct; struct layouts cannot change under hot reload. Please rebuild."
                    else
                        $"Edit adds field '{fieldDisplay}' to a type that has no baseline TypeDef token; please rebuild."

                raise (HotReloadUnsupportedEditException message))

        // Reverse direction of the struct layout gate above: a BASELINE field missing
        // from the fresh compile of an existing compiler-generated struct (a removed
        // awaiter or hoisted local of a state machine) is the same immutable-layout
        // violation as an added field, and is invisible to the per-fresh-field loop.
        if typeDef.IsStructOrEnum && IsCompilerGeneratedName typeDef.Name then
            let declaringTypeRef = mkRefForNestedILTypeDef ILScopeRef.Local (enclosing, typeDef)
            let baselineDeclaringType = tryGetBaselineTypeName declaringTypeRef.FullName

            if
                not (addedTypeDeltaTokens.ContainsKey baselineDeclaringType)
                && request.Baseline.TypeTokens |> Map.containsKey baselineDeclaringType
            then
                let freshFieldNames =
                    typeDef.Fields.AsList()
                    |> List.map (fun f -> normalizeGeneratedFieldName f.Name)
                    |> Set.ofList

                request.Baseline.FieldTokens
                |> Map.iter (fun key _ ->
                    if
                        String.Equals(key.DeclaringType, baselineDeclaringType, StringComparison.Ordinal)
                        && not (Set.contains (normalizeGeneratedFieldName key.Name) freshFieldNames)
                    then
                        raise (
                            HotReloadUnsupportedEditException(
                                $"Edit changes the layout of state machine struct '{declaringTypeRef.FullName}': baseline field '{key.Name}' is gone (a resume point or hoisted local changed); struct layouts cannot change under hot reload. Please rebuild."
                            )
                        ))

        typeDef.Methods.AsList()
        |> List.iter (fun methodDef ->
            let newMethodToken =
                emittedTokenMappings.MethodDefTokenMap (enclosing, typeDef) methodDef

            let declaringTypeRef = mkRefForNestedILTypeDef ILScopeRef.Local (enclosing, typeDef)
            let baselineDeclaringType = tryGetBaselineTypeName declaringTypeRef.FullName
            let declaringTypeIsAdded = addedTypeDeltaTokens.ContainsKey baselineDeclaringType

            let methodKey: MethodDefinitionKey =
                {
                    DeclaringType = baselineDeclaringType
                    Name = methodDef.Name
                    GenericArity = methodDef.GenericParams.Length
                    ParameterTypes = methodDef.ParameterTypes
                    ReturnType = methodDef.Return.Type
                }

            let tryFindBaselineResumableHelperMethodIgnoringReturnType () =
                if isUpdatedMethodResumableCodeHelper typeDef then
                    request.Baseline.MethodTokens
                    |> Map.toArray
                    |> Array.filter (fun (candidate, _) ->
                        String.Equals(candidate.DeclaringType, baselineDeclaringType, StringComparison.Ordinal)
                        && String.Equals(candidate.Name, methodDef.Name, StringComparison.Ordinal)
                        && candidate.GenericArity = methodDef.GenericParams.Length
                        && candidate.ParameterTypes = methodDef.ParameterTypes)
                    |> function
                        | [| candidate, token |] -> Some(candidate, token)
                        | _ -> None
                else
                    None

            let baselineMethodTokenOpt =
                if declaringTypeIsAdded then
                    None
                else
                    match request.Baseline.MethodTokens |> Map.tryFind methodKey with
                    | Some token -> Some(methodKey, token)
                    | None -> tryFindBaselineResumableHelperMethodIgnoringReturnType ()

            match baselineMethodTokenOpt with
            | Some(_, baselineMethodToken) ->
                if newMethodToken <> 0 && baselineMethodToken <> 0 then
                    matchedMethodTokenPairs[newMethodToken] <- baselineMethodToken

                addMapping methodTokenMap newMethodToken baselineMethodToken
            | None when declaringTypeIsAdded ->
                // Method of an ADDED type (closure .ctor / Invoke override): emitted as an
                // added method parented to the NEW TypeDef row.
                if not (addedMethodTokens.ContainsKey methodKey) then
                    addedMethodTokens[methodKey] <- newMethodToken
            | None when isUpdatedMethodResumableCodeHelper typeDef ->
                // Method of an existing task/class-state-machine helper whose signature
                // changed as the resumable step shape changed. The diff already gated this
                // path on AddMethodToExistingType; do not let the generic synthesized skip
                // drop the fresh Invoke/.ctor body.
                if not (addedMethodTokens.ContainsKey methodKey) then
                    addedMethodTokens[methodKey] <- newMethodToken
            | None when synthesizedBuckets.IsSome && IsCompilerGeneratedName typeDef.Name -> ()
            | None ->
                if not (addedMethodTokens.ContainsKey methodKey) then
                    addedMethodTokens[methodKey] <- newMethodToken)

        typeDef.Properties.AsList()
        |> List.iter (fun propertyDef ->
            let newPropertyToken =
                emittedTokenMappings.PropertyTokenMap (enclosing, typeDef) propertyDef

            let declaringTypeRef = mkRefForNestedILTypeDef ILScopeRef.Local (enclosing, typeDef)
            let baselineDeclaringType = tryGetBaselineTypeName declaringTypeRef.FullName
            let declaringTypeIsAdded = addedTypeDeltaTokens.ContainsKey baselineDeclaringType

            let propertyKey: PropertyDefinitionKey =
                {
                    DeclaringType = baselineDeclaringType
                    Name = propertyDef.Name
                    PropertyType = propertyDef.PropertyType
                    IndexParameterTypes = List.ofSeq propertyDef.Args
                }

            let baselinePropertyTokenOpt =
                if declaringTypeIsAdded then
                    None
                else
                    request.Baseline.PropertyTokens |> Map.tryFind propertyKey

            match baselinePropertyTokenOpt with
            | Some baselinePropertyToken -> addMapping propertyTokenMap newPropertyToken baselinePropertyToken
            | None when declaringTypeIsAdded ->
                if not (addedPropertyTokens.ContainsKey propertyKey) then
                    addedPropertyTokens[propertyKey] <- newPropertyToken
                    addedPropertyTokenLookup[newPropertyToken] <- propertyKey
                    let rowId = newPropertyToken &&& 0x00FFFFFF
                    propertyHandleLookup[propertyKey] <- MetadataTokens.PropertyDefinitionHandle rowId
            | None when synthesizedBuckets.IsSome && IsCompilerGeneratedName typeDef.Name -> ()
            | None ->
                if not (addedPropertyTokens.ContainsKey propertyKey) then
                    addedPropertyTokens[propertyKey] <- newPropertyToken
                    addedPropertyTokenLookup[newPropertyToken] <- propertyKey
                    let rowId = newPropertyToken &&& 0x00FFFFFF
                    propertyHandleLookup[propertyKey] <- MetadataTokens.PropertyDefinitionHandle rowId)

        typeDef.Events.AsList()
        |> List.iter (fun eventDef ->
            let newEventToken = emittedTokenMappings.EventTokenMap (enclosing, typeDef) eventDef
            let declaringTypeRef = mkRefForNestedILTypeDef ILScopeRef.Local (enclosing, typeDef)
            let baselineDeclaringType = tryGetBaselineTypeName declaringTypeRef.FullName
            let declaringTypeIsAdded = addedTypeDeltaTokens.ContainsKey baselineDeclaringType

            let eventKey: EventDefinitionKey =
                {
                    DeclaringType = baselineDeclaringType
                    Name = eventDef.Name
                    EventType = eventDef.EventType
                }

            let baselineEventTokenOpt =
                if declaringTypeIsAdded then
                    None
                else
                    request.Baseline.EventTokens |> Map.tryFind eventKey

            match baselineEventTokenOpt with
            | Some baselineEventToken -> addMapping eventTokenMap newEventToken baselineEventToken
            | None when declaringTypeIsAdded ->
                if not (addedEventTokens.ContainsKey eventKey) then
                    addedEventTokens[eventKey] <- newEventToken
                    addedEventTokenLookup[newEventToken] <- eventKey
                    let rowId = newEventToken &&& 0x00FFFFFF
                    eventHandleLookup[eventKey] <- MetadataTokens.EventDefinitionHandle rowId
            | None when synthesizedBuckets.IsSome && IsCompilerGeneratedName typeDef.Name -> ()
            | None ->
                if not (addedEventTokens.ContainsKey eventKey) then
                    addedEventTokens[eventKey] <- newEventToken
                    addedEventTokenLookup[newEventToken] <- eventKey
                    let rowId = newEventToken &&& 0x00FFFFFF
                    eventHandleLookup[eventKey] <- MetadataTokens.EventDefinitionHandle rowId)

        typeDef.NestedTypes.AsList()
        |> List.iter (fun nested -> collectTypeMappings (enclosing @ [ typeDef ]) nested)

    request.Module.TypeDefs.AsList() |> List.iter (collectTypeMappings [])

    // An edit that declares nothing (no updated/added symbols, no accessor edits) cannot ADD
    // definitions: fresh definitions without a baseline counterpart are then regeneration noise
    // (e.g. a generative type provider re-emitting its members under a changed static argument
    // while the consumed IL is unchanged), not additions. Such emits exist because sequence-point
    // tracking runs the emitter even for semantically empty diffs to detect sequence-point line shifts; drop
    // the spurious registrations so they cannot materialize into delta rows.
    let editDeclaresDefinitions =
        not (List.isEmpty request.UpdatedMethods)
        || not (List.isEmpty request.UpdatedTypes)
        || not (List.isEmpty request.UpdatedAccessors)
        || (request.SymbolChanges
            |> Option.map (fun changes -> not (List.isEmpty changes.Added))
            |> Option.defaultValue false)

    if not editDeclaresDefinitions then
        if traceMethodUpdates.Value && addedMethodTokens.Count > 0 then
            let names =
                addedMethodTokens
                |> Seq.map (fun kvp -> $"{kvp.Key.DeclaringType}::{kvp.Key.Name}")
                |> String.concat ", "

            printfn "[fsharp-hotreload][sequence-points] ignoring unmatched definitions of no-edit emit: %s" names

        addedMethodTokens.Clear()
        addedFieldTokens.Clear()
        addedPropertyTokens.Clear()
        addedEventTokens.Clear()
        addedPropertyTokenLookup.Clear()
        addedEventTokenLookup.Clear()
        propertyHandleLookup.Clear()
        eventHandleLookup.Clear()

        // Added TypeDefs are discovered before the semantic edit surface is known. A
        // no-edit emit must discard every associated registry and token remap together,
        // otherwise regeneration noise can survive as a real TypeDef delta.
        for newTypeToken in addedTypeNewTokens.Values do
            typeTokenMap.Remove newTypeToken |> ignore

        addedTypeNewTokens.Clear()
        addedTypeDeltaTokens.Clear()
        addedTypeDefs.Clear()
        addedTypeShapes.Clear()
        addedTypeKeyByFreshFullName.Clear()
        freshFullNameByAddedTypeKey.Clear()
        addedTypeMetadataNameByKey.Clear()

    let addedMethodKeys =
        addedMethodTokens |> Seq.map (fun kvp -> kvp.Key) |> Seq.toList

    // ------------------------------------------------------------------------------------------
    // Sequence-point analysis. Diff every baseline-matched method's fresh sequence
    // points against the committed snapshot (the debugger's current view of its lines):
    //   - UniformLineShift -> a line-edit segment (the debugger rebinds without recompilation;
    //     Roslyn: AbstractEditAndContinueAnalyzer.GetLineEdits)
    //   - Different        -> recompile the method body so its debug information stays accurate
    //     (Roslyn: trivia edits force a member body update)
    // Methods already recompiled by the request are excluded (the debugger ignores line deltas
    // for recompiled methods). An empty committed snapshot (no baseline PDB) keeps this inert.
    let committedSequencePoints = request.Baseline.SequencePointSnapshots

    let freshSequencePointsByToken =
        match effectiveDebugPdb with
        | Some freshPdbBytes when not (Map.isEmpty committedSequencePoints) ->
            ActiveStatementAnalysis.decodeMethodSequencePoints freshPdbBytes
        | _ -> Map.empty

    let lineShiftSegments, triviaRecompileTokens =
        if Map.isEmpty freshSequencePointsByToken then
            [], []
        else
            let requestRecompiledBaselineTokens =
                let accessorMethodKeys =
                    request.UpdatedAccessors |> List.choose (fun accessor -> accessor.Method)

                request.UpdatedMethods @ accessorMethodKeys
                |> List.choose (fun key -> request.Baseline.MethodTokens |> Map.tryFind key)
                |> Set.ofList

            let segments = ResizeArray<ActiveStatementAnalysis.LineShiftSegment>()
            let recompileTokens = ResizeArray<int>()

            for KeyValue(newToken, baselineToken) in matchedMethodTokenPairs do
                if not (Set.contains baselineToken requestRecompiledBaselineTokens) then
                    match Map.tryFind baselineToken committedSequencePoints, Map.tryFind newToken freshSequencePointsByToken with
                    | Some committed, Some fresh ->
                        match ActiveStatementAnalysis.compareMethodSequencePoints committed fresh with
                        | ActiveStatementAnalysis.SequencePointComparison.Identical ->
                            // Zero-delta segment: overlap detection only (Roslyn parity).
                            ActiveStatementAnalysis.tryCreateLineShiftSegment baselineToken committed 0
                            |> Option.iter segments.Add
                        | ActiveStatementAnalysis.SequencePointComparison.UniformLineShift lineDelta ->
                            if traceMethodUpdates.Value then
                                printfn "[fsharp-hotreload][sequence-points] line shift token=0x%08X delta=%d" baselineToken lineDelta

                            match ActiveStatementAnalysis.tryCreateLineShiftSegment baselineToken committed lineDelta with
                            | Some segment -> segments.Add segment
                            | None -> recompileTokens.Add baselineToken
                        | ActiveStatementAnalysis.SequencePointComparison.Different ->
                            if traceMethodUpdates.Value then
                                printfn "[fsharp-hotreload][sequence-points] different token=0x%08X" baselineToken

                            recompileTokens.Add baselineToken
                    | None, None -> ()
                    | Some _, None
                    | None, Some _ ->
                        // Debug information appeared or disappeared without a semantic edit;
                        // recompile so the committed view stays accurate (fail closed).
                        if traceMethodUpdates.Value then
                            printfn
                                "[fsharp-hotreload][sequence-points] debug info presence changed token=0x%08X new=0x%08X"
                                baselineToken
                                newToken

                        recompileTokens.Add baselineToken

            List.ofSeq segments, List.ofSeq recompileTokens

    let sequencePointUpdates, overlapRecompileTokens =
        ActiveStatementAnalysis.mergeLineShiftSegments lineShiftSegments

    let triviaRecompileKeys =
        let methodKeyByBaselineToken =
            request.Baseline.MethodTokens
            |> Map.toSeq
            |> Seq.map (fun (key, token) -> token, key)
            |> Map.ofSeq

        triviaRecompileTokens @ overlapRecompileTokens
        |> List.distinct
        |> List.sort
        |> List.choose (fun token -> Map.tryFind token methodKeyByBaselineToken)

    if traceMethodUpdates.Value && not (List.isEmpty triviaRecompileKeys) then
        let names =
            triviaRecompileKeys
            |> List.map (fun key -> $"{key.DeclaringType}::{key.Name}")
            |> String.concat ", "

        printfn "[fsharp-hotreload][sequence-points] trivia-edit recompiles: %s" names

    let allUpdatedMethods =
        (request.UpdatedMethods @ triviaRecompileKeys @ addedMethodKeys)
        |> dedupeMethodKeys

    let tryResolveMethodThroughMappedDeclaringType (key: MethodDefinitionKey) =
        match tryResolveFreshTypeNameForKey key.DeclaringType with
        | Some freshDeclaringType ->
            let freshKey =
                { key with
                    DeclaringType = freshDeclaringType
                }

            match FSharpSymbolMatcher.tryGetMethodDef symbolMatcher freshKey with
            | Some(enclosing, typeDef, methodDef) ->
                if traceMethodUpdates.Value then
                    printfn
                        "[fsharp-hotreload][method-update] resolved mapped helper: %s::%s -> %s::%s"
                        key.DeclaringType
                        key.Name
                        freshDeclaringType
                        key.Name

                Some(enclosing, typeDef, methodDef)
            | None -> None
        | None -> None

    let tryResolveResumableHelperMethodIgnoringReturnType (key: MethodDefinitionKey) =
        let declaringType =
            tryResolveFreshTypeNameForKey key.DeclaringType
            |> Option.defaultValue key.DeclaringType

        match FSharpSymbolMatcher.tryGetTypeDef symbolMatcher declaringType with
        | Some(enclosing, typeDef) when isUpdatedMethodResumableCodeHelper typeDef ->
            typeDef.Methods.AsList()
            |> List.filter (fun methodDef ->
                String.Equals(methodDef.Name, key.Name, StringComparison.Ordinal)
                && methodDef.GenericParams.Length = key.GenericArity
                && methodDef.ParameterTypes = key.ParameterTypes)
            |> function
                | [ methodDef ] ->
                    if traceMethodUpdates.Value then
                        printfn
                            "[fsharp-hotreload][method-update] resolved resumable helper by stable signature: %s::%s"
                            key.DeclaringType
                            key.Name

                    Some(enclosing, typeDef, methodDef)
                | _ -> None
        | _ -> None

    let resolvedMethods, unresolvedMethods =
        (([], []), allUpdatedMethods)
        ||> List.fold (fun (resolved, unresolved) key ->
            match FSharpSymbolMatcher.tryGetMethodDef symbolMatcher key with
            | Some(enclosing, typeDef, methodDef) -> (enclosing, typeDef, methodDef, key) :: resolved, unresolved
            | None ->
                match tryResolveMethodThroughMappedDeclaringType key with
                | Some(enclosing, typeDef, methodDef) -> (enclosing, typeDef, methodDef, key) :: resolved, unresolved
                | None ->
                    match tryResolveResumableHelperMethodIgnoringReturnType key with
                    | Some(enclosing, typeDef, methodDef) -> (enclosing, typeDef, methodDef, key) :: resolved, unresolved
                    | None -> resolved, key :: unresolved)

    // Compiler-generated closures and companions (declaring type contains '@', e.g.
    // `view@hotreload#g0_o0`) carry generation-specific names that do not exist under the
    // same name in a fresh full recompile, so they cannot be resolved by reconstructed key.
    // They are best-effort recompile targets: the user-facing edit is still emitted through
    // the owning member, so tolerate them (the historical drop behavior) and fail closed only
    // on genuinely-requested user methods that fail to resolve, which would otherwise advance
    // the baseline while the runtime keeps stale code.
    let isCompilerGeneratedCompanion (key: MethodDefinitionKey) = key.DeclaringType.Contains "@"

    let unresolvedUserMethods =
        unresolvedMethods |> List.filter (isCompilerGeneratedCompanion >> not)

    if not (List.isEmpty unresolvedUserMethods) then
        let names =
            unresolvedUserMethods
            |> List.rev
            |> List.map (fun key -> $"{key.DeclaringType}::{key.Name}")
            |> String.concat ", "

        raise (HotReloadUnsupportedEditException($"Unable to resolve updated method(s) in the fresh compilation: {names}. Please rebuild."))

    let resolvedMethods = List.rev resolvedMethods

    if traceUserStringUpdates.Value then
        for (_, _, methodDef, _) in resolvedMethods do
            match methodDef.Code with
            | None -> ()
            | Some code ->
                for instr in code.Instrs do
                    match instr with
                    | I_ldstr literal -> printfn "[fsharp-hotreload][method] %s ldstr literal=%s" methodDef.Name literal
                    | _ -> ()

    let moduleMvid = request.Baseline.ModuleId

    let encBaseId =
        match request.PreviousGenerationId with
        | Some prev when prev <> Guid.Empty -> prev
        | _ ->
            let baselineEncId = request.Baseline.EncId

            if baselineEncId <> Guid.Empty then
                baselineEncId
            else
                Guid.Empty

    if traceMetadata.Value then
        printfn
            "[fsharp-hotreload][metadata] generation=%d prevGeneration=%A baselineEncId=%A resolvedBase=%A"
            request.CurrentGeneration
            request.PreviousGenerationId
            request.Baseline.EncId
            encBaseId

    let encId = System.Guid.NewGuid()

    let methodRowLookup =
        let baselineTokens = request.Baseline.MethodTokens

        fun key ->
            baselineTokens
            |> Map.tryFind key
            |> Option.map (fun token -> token &&& 0x00FFFFFF)

    let baselineTableRowCounts = request.Baseline.Metadata.TableRowCounts
    let baselineTypeRefRowCount = baselineTableRowCounts.[TableNames.TypeRef.Index]
    let baselineMemberRefRowCount = baselineTableRowCounts.[TableNames.MemberRef.Index]

    let baselineAssemblyRefRowCount =
        baselineTableRowCounts.[TableNames.AssemblyRef.Index]

    let baselineMethodSpecRowCount =
        baselineTableRowCounts.[TableNames.MethodSpec.Index]

    let lastMethodRowId = baselineTableRowCounts.[TableNames.Method.Index]
    let mutable nextTypeRefRowId = baselineTypeRefRowCount
    let mutable nextMemberRefRowId = baselineMemberRefRowCount
    let mutable nextAssemblyRefRowId = baselineAssemblyRefRowCount
    let typeReferenceRows = ResizeArray<TypeReferenceRowInfo>()
    let memberReferenceRows = ResizeArray<MemberReferenceRowInfo>()
    let assemblyReferenceRows = ResizeArray<AssemblyReferenceRowInfo>()
    let typeSpecificationRows = ResizeArray<TypeSpecificationRowInfo>()
    let baselineTypeReferenceTokens = request.Baseline.TypeReferenceTokens
    let baselineAssemblyReferenceTokens = request.Baseline.AssemblyReferenceTokens

    if traceMetadata.Value then
        printfn
            "[fsharp-hotreload][metadata] baseline-typerefs=%d baseline-assemblyrefs=%d"
            (baselineTypeReferenceTokens |> Map.count)
            (baselineAssemblyReferenceTokens |> Map.count)

    let tryReuseBaselineTypeRef (key: TypeReferenceKey) =
        baselineTypeReferenceTokens |> Map.tryFind key

    let tryReuseBaselineAssemblyRef key =
        baselineAssemblyReferenceTokens |> Map.tryFind key

    let typeRefTokenMap = Dictionary<int, int>()
    let assemblyRefTokenMap = Dictionary<int, int>()
    let memberRefTokenMap = Dictionary<int, int>()

    let addedTypeReferenceTokens =
        Dictionary<TypeReferenceKey, int>(HashIdentity.Structural)

    let addedAssemblyReferenceTokens =
        Dictionary<AssemblyReferenceKey, int>(HashIdentity.Structural)

    let methodDefinitionIndex =
        DefinitionIndex<MethodDefinitionKey>(methodRowLookup, lastMethodRowId)

    let processedMethodKeys = HashSet<MethodDefinitionKey>()

    let addedMethodDeltaTokens =
        Dictionary<MethodDefinitionKey, int>(HashIdentity.Structural)

    let methodSpecTokenMap = Dictionary<int, int>()
    let methodSpecRowsByToken = Dictionary<int, MethodSpecificationRowInfo>()
    let mutable nextMethodSpecRowId = baselineMethodSpecRowCount
    let mutable nextTypeSpecRowId = baselineTableRowCounts.[TableNames.TypeSpec.Index]

    for KeyValue(key, newToken) in addedMethodTokens |> Seq.sortBy (fun kvp -> kvp.Value) do
        if not (methodDefinitionIndex.IsAdded key) then
            let rowId = methodDefinitionIndex.Add key
            let deltaToken = 0x06000000 ||| rowId
            addedMethodDeltaTokens[key] <- deltaToken
            matchedMethodTokenPairs[newToken] <- deltaToken
            addMapping methodTokenMap newToken deltaToken

    // Allocate delta Field rows for added static fields. Row ids continue from the baseline
    // Field table (like AddedOrChangedMethods); the fresh-compile token maps to the delta
    // token so accessor/initializer bodies referencing the new field resolve to the appended
    // row. Iterate in fresh-compile token order to keep delta row ids deterministic.
    let baselineFieldRowCount = baselineTableRowCounts.[TableNames.Field.Index]

    let fieldRowLookup (key: FieldDefinitionKey) =
        request.Baseline.FieldTokens
        |> Map.tryFind key
        |> Option.map (fun token -> token &&& 0x00FFFFFF)

    let fieldDefinitionIndex =
        DefinitionIndex<FieldDefinitionKey>(fieldRowLookup, baselineFieldRowCount)

    let addedFieldDeltaTokens =
        Dictionary<FieldDefinitionKey, int>(HashIdentity.Structural)

    for KeyValue(key, newToken) in addedFieldTokens |> Seq.sortBy (fun kvp -> kvp.Value) do
        if not (fieldDefinitionIndex.IsAdded key) then
            let rowId = fieldDefinitionIndex.Add key
            let deltaToken = 0x04000000 ||| rowId
            addedFieldDeltaTokens[key] <- deltaToken
            addMapping fieldTokenMap newToken deltaToken

    // Keep definition-token projection separate from metadata-reference remapping.
    let definitionTokenRemapper =
        createDefinitionTokenRemapper
            {
                TypeTokenMap = typeTokenMap
                FieldTokenMap = fieldTokenMap
                MethodTokenMap = methodTokenMap
                PropertyTokenMap = propertyTokenMap
                EventTokenMap = eventTokenMap
            }

    let metadataReferenceRemapper =
        createMetadataReferenceRemapper
            {
                MetadataReader = metadataReader
                TraceMetadata = traceMetadata.Value
                BaselineMemberRefRowCount = baselineMemberRefRowCount
                BaselineMemberRefRows = request.Baseline.MemberReferenceRows
                BaselineTypeSpecSignatures = request.Baseline.TypeSpecSignatures
                BaselineTypeSpecRowCount = baselineTableRowCounts.[TableNames.TypeSpec.Index]
                TypeSpecTokenMap = Dictionary<int, int>()
                TypeSpecificationRows = typeSpecificationRows
                TryReuseBaselineTypeRef = tryReuseBaselineTypeRef
                TryReuseBaselineAssemblyRef = tryReuseBaselineAssemblyRef
                RemapDefinitionToken = definitionTokenRemapper.RemapDefinitionToken
                TypeReferenceRows = typeReferenceRows
                MemberReferenceRows = memberReferenceRows
                AssemblyReferenceRows = assemblyReferenceRows
                AddedTypeReferenceTokens = addedTypeReferenceTokens
                AddedAssemblyReferenceTokens = addedAssemblyReferenceTokens
                TypeRefTokenMap = typeRefTokenMap
                AssemblyRefTokenMap = assemblyRefTokenMap
                MemberRefTokenMap = memberRefTokenMap
                MethodSpecTokenMap = methodSpecTokenMap
                MethodSpecRowsByToken = methodSpecRowsByToken
                GetNextTypeRefRowId = (fun () -> nextTypeRefRowId)
                SetNextTypeRefRowId = (fun value -> nextTypeRefRowId <- value)
                GetNextMemberRefRowId = (fun () -> nextMemberRefRowId)
                SetNextMemberRefRowId = (fun value -> nextMemberRefRowId <- value)
                GetNextAssemblyRefRowId = (fun () -> nextAssemblyRefRowId)
                SetNextAssemblyRefRowId = (fun value -> nextAssemblyRefRowId <- value)
                GetNextMethodSpecRowId = (fun () -> nextMethodSpecRowId)
                SetNextMethodSpecRowId = (fun value -> nextMethodSpecRowId <- value)
                GetNextTypeSpecRowId = (fun () -> nextTypeSpecRowId)
                SetNextTypeSpecRowId = (fun value -> nextTypeSpecRowId <- value)
            }

    let remapEntityToken = metadataReferenceRemapper.RemapEntityToken
    let remapAssemblyRefToken = metadataReferenceRemapper.RemapAssemblyRefToken

    // Fresh-compile MethodDef tokens keyed by the baseline-coordinate token they map to.
    // addMapping records fresh -> baseline pairs only when the row ids DIFFER, so a
    // missing entry means the fresh row coincides with the baseline row. Method bodies,
    // attributes, signatures and parameters of updated methods must be read from the
    // fresh reader through THIS map; the baseline token is only the emission row.
    let freshMethodTokenByBaseline = Dictionary<int, int>()

    for KeyValue(freshToken, baselineToken) in methodTokenMap do
        freshMethodTokenByBaseline[baselineToken] <- freshToken

    let methodUpdateInputs =
        resolvedMethods
        |> List.choose (fun (_, _, _, key) ->
            tryBuildMethodUpdateInput
                traceMethodUpdates.Value
                metadataReader
                peReader
                request.Baseline.MethodTokens
                freshMethodTokenByBaseline
                addedMethodTokens
                addedMethodDeltaTokens
                key)

    let parameterRowLookup = Dictionary<ParameterDefinitionKey, int>()
    let parameterHandleLookup = Dictionary<ParameterDefinitionKey, ParameterHandle>()

    let syntheticParameterInfo =
        Dictionary<ParameterDefinitionKey, ParameterAttributes>(HashIdentity.Structural)

    let returnParameterKeys = HashSet<ParameterDefinitionKey>(HashIdentity.Structural)
    let lastParamRowId = baselineTableRowCounts.[TableNames.Param.Index]

    let parameterDefinitionIndex =
        let tryExisting key =
            match parameterRowLookup.TryGetValue key with
            | true, rowId -> Some rowId
            | _ -> None

        DefinitionIndex<ParameterDefinitionKey>(tryExisting, lastParamRowId)

    let propertyTokenToKey =
        let dict = Dictionary<int, PropertyDefinitionKey>()

        for KeyValue(key, token) in request.Baseline.PropertyTokens do
            dict[token] <- key

        dict

    for KeyValue(token, key) in addedPropertyTokenLookup do
        propertyTokenToKey[token] <- key

    let eventTokenToKey =
        let dict = Dictionary<int, EventDefinitionKey>()

        for KeyValue(key, token) in request.Baseline.EventTokens do
            dict[token] <- key

        dict

    for KeyValue(token, key) in addedEventTokenLookup do
        eventTokenToKey[token] <- key

    let methodTokenToKey =
        let dict = Dictionary<int, MethodDefinitionKey>()

        for KeyValue(key, token) in request.Baseline.MethodTokens do
            dict[token] <- key

        for KeyValue(key, token) in addedMethodDeltaTokens do
            dict[token] <- key

        dict

    let propertyRowLookup key =
        request.Baseline.PropertyTokens
        |> Map.tryFind key
        |> Option.map (fun token -> token &&& 0x00FFFFFF)

    let eventRowLookup key =
        request.Baseline.EventTokens
        |> Map.tryFind key
        |> Option.map (fun token -> token &&& 0x00FFFFFF)

    let lastPropertyRowId = baselineTableRowCounts.[TableNames.Property.Index]

    let propertyDefinitionIndex =
        DefinitionIndex<PropertyDefinitionKey>(propertyRowLookup, lastPropertyRowId)

    let processedPropertyKeys = HashSet<PropertyDefinitionKey>()

    let addedPropertyDeltaTokens =
        Dictionary<PropertyDefinitionKey, int>(HashIdentity.Structural)

    for KeyValue(key, newToken) in addedPropertyTokens do
        if not (propertyDefinitionIndex.IsAdded key) then
            let rowId = propertyDefinitionIndex.Add key
            let deltaToken = 0x17000000 ||| rowId
            addedPropertyDeltaTokens[key] <- deltaToken
            addMapping propertyTokenMap newToken deltaToken

    for KeyValue(key, token) in addedPropertyDeltaTokens do
        propertyTokenToKey[token] <- key

    let lastEventRowId = baselineTableRowCounts.[TableNames.Event.Index]

    let eventDefinitionIndex =
        DefinitionIndex<EventDefinitionKey>(eventRowLookup, lastEventRowId)

    let processedEventKeys = HashSet<EventDefinitionKey>()

    let addedEventDeltaTokens =
        Dictionary<EventDefinitionKey, int>(HashIdentity.Structural)

    for KeyValue(key, newToken) in addedEventTokens do
        if not (eventDefinitionIndex.IsAdded key) then
            let rowId = eventDefinitionIndex.Add key
            let deltaToken = 0x14000000 ||| rowId
            addedEventDeltaTokens[key] <- deltaToken
            addMapping eventTokenMap newToken deltaToken

    for KeyValue(key, token) in addedEventDeltaTokens do
        eventTokenToKey[token] <- key

    for struct (key, _, _, _, _) in methodUpdateInputs do
        if processedMethodKeys.Add key then
            if methodDefinitionIndex.IsAdded key then
                ()
            else
                methodDefinitionIndex.AddExisting key

    let methodUpdateLookup =
        let dict =
            Dictionary<MethodDefinitionKey, struct (MethodDefinitionKey * int * MethodDefinitionHandle * MethodDefinition * MethodBodyBlock)>()

        for struct (key, methodToken, methodHandle, methodDef, body) in methodUpdateInputs do
            dict[key] <- struct (key, methodToken, methodHandle, methodDef, body)

        dict

    let propertyAccessorLookup =
        Dictionary<MethodDefinitionHandle, PropertyDefinitionHandle>()

    for propertyHandle in metadataReader.PropertyDefinitions do
        let propertyDef = metadataReader.GetPropertyDefinition propertyHandle
        let accessors = propertyDef.GetAccessors()
        let getter = accessors.Getter

        if not getter.IsNil then
            propertyAccessorLookup[getter] <- propertyHandle

        let setter = accessors.Setter

        if not setter.IsNil then
            propertyAccessorLookup[setter] <- propertyHandle

        for otherHandle in accessors.Others do
            if not otherHandle.IsNil then
                propertyAccessorLookup[otherHandle] <- propertyHandle

    let eventAccessorLookup =
        Dictionary<MethodDefinitionHandle, EventDefinitionHandle>()

    for eventHandle in metadataReader.EventDefinitions do
        let eventDef = metadataReader.GetEventDefinition eventHandle
        let accessors = eventDef.GetAccessors()
        let adder = accessors.Adder

        if not adder.IsNil then
            eventAccessorLookup[adder] <- eventHandle

        let remover = accessors.Remover

        if not remover.IsNil then
            eventAccessorLookup[remover] <- eventHandle

        let raiser = accessors.Raiser

        if not raiser.IsNil then
            eventAccessorLookup[raiser] <- eventHandle

    let methodDefinitionRowsRaw = methodDefinitionIndex.Rows

    let orderedMethodInputs =
        methodDefinitionRowsRaw
        |> List.choose (fun struct (_, key, _) ->
            match methodUpdateLookup.TryGetValue key with
            | true, data -> Some data
            | _ -> None)

    let baselineMethodHandles = request.Baseline.MetadataHandles.MethodHandles
    let baselineParameterHandles = request.Baseline.MetadataHandles.ParameterHandles

    let baselineParametersByMethod =
        let dict =
            Dictionary<MethodDefinitionKey, (ParameterDefinitionKey * ParameterDefinitionMetadataHandles) list>(HashIdentity.Structural)

        for KeyValue(paramKey, info) in baselineParameterHandles do
            let methodKey = paramKey.Method

            let existing =
                match dict.TryGetValue methodKey with
                | true, entries -> entries
                | _ -> []

            dict[methodKey] <- (paramKey, info) :: existing

        dict

    if traceMethodUpdates.Value then
        for KeyValue(methodKey, entries) in baselineParametersByMethod do
            printfn
                "[fsharp-hotreload][param-baseline] method=%s::%s entries=%d"
                methodKey.DeclaringType
                methodKey.Name
                (List.length entries)

    let baselinePropertyHandles = request.Baseline.MetadataHandles.PropertyHandles
    let baselineEventHandles = request.Baseline.MetadataHandles.EventHandles

    let firstParamRowByMethod =
        Dictionary<MethodDefinitionKey, int>(HashIdentity.Structural)

    let addSyntheticParameter key sequence attrs =
        let paramKey =
            {
                ParameterDefinitionKey.Method = key
                SequenceNumber = sequence
            }

        if not (parameterRowLookup.ContainsKey paramKey) then
            let rowId = parameterDefinitionIndex.Add paramKey
            parameterRowLookup[paramKey] <- rowId
            syntheticParameterInfo[paramKey] <- attrs
            rowId
        else
            parameterRowLookup[paramKey]

    let ensureReturnParameterRow key isAdded =
        let paramKey =
            {
                ParameterDefinitionKey.Method = key
                SequenceNumber = 0
            }

        if parameterRowLookup.ContainsKey paramKey then
            Some parameterRowLookup[paramKey]
        else
            match
                baselineParameterHandles
                |> Map.tryFind paramKey
                |> Option.bind (fun info -> info.RowId)
            with
            | Some baselineRow when baselineRow > 0 ->
                parameterRowLookup[paramKey] <- baselineRow
                parameterDefinitionIndex.AddExisting paramKey
                Some baselineRow
            | _ ->
                // Never SYNTHESIZE a return parameter row (Roslyn parity): the fresh compile
                // emits Param rows only for parameters that exist, and rows must stay ordered
                // by sequence number within a method. A synthesized seq-0 row appended after
                // real seq-1+ rows is out of sequence, which forces the CLR EnC applier
                // (FixParamSequence) onto the indirect ParamPtr-table path and makes
                // ApplyUpdate reject the delta (observed with added module-value accessors).
                ignore isAdded
                None

    let enqueueParameters key methodHandle =
        let methodDef = metadataReader.GetMethodDefinition methodHandle
        let parameters = methodDef.GetParameters()
        let mutable sawParameter = false

        for parameterHandle in parameters do
            sawParameter <- true
            let parameter = metadataReader.GetParameter parameterHandle

            if methodDefinitionIndex.IsAdded key then
                let display =
                    $"{key.DeclaringType}::{key.Name} parameter {parameter.SequenceNumber}"

                if parameter.GetCustomAttributes().Count > 0 then
                    raise (
                        HotReloadUnsupportedEditException(
                            $"Added method {display} has custom attributes, which hot reload deltas cannot emit yet; please rebuild."
                        )
                    )

                if not (parameter.GetDefaultValue().IsNil) then
                    raise (
                        HotReloadUnsupportedEditException(
                            $"Added method {display} has a default value, which requires an unsupported Constant row; please rebuild."
                        )
                    )

                if not (parameter.GetMarshallingDescriptor().IsNil) then
                    raise (
                        HotReloadUnsupportedEditException(
                            $"Added method {display} has marshalling metadata, which requires an unsupported FieldMarshal row; please rebuild."
                        )
                    )

            let paramKey =
                {
                    ParameterDefinitionKey.Method = key
                    SequenceNumber = int parameter.SequenceNumber
                }

            if methodDefinitionIndex.IsAdded key then
                if not (parameterRowLookup.ContainsKey paramKey) then
                    let rowId = parameterDefinitionIndex.Add paramKey
                    parameterRowLookup[paramKey] <- rowId
                    parameterHandleLookup[paramKey] <- parameterHandle
            else if not (parameterRowLookup.ContainsKey paramKey) then
                let rowId =
                    match
                        baselineParameterHandles
                        |> Map.tryFind paramKey
                        |> Option.bind (fun info -> info.RowId)
                    with
                    | Some baselineRow when baselineRow > 0 -> baselineRow
                    | _ -> MetadataTokens.GetRowNumber parameterHandle

                parameterRowLookup[paramKey] <- rowId
                parameterHandleLookup[paramKey] <- parameterHandle
                parameterDefinitionIndex.AddExisting paramKey

        if not sawParameter then
            match baselineParametersByMethod.TryGetValue key with
            | true, entries when not (List.isEmpty entries) ->
                if traceMethodUpdates.Value then
                    printfn "[fsharp-hotreload][param-fallback] method=%s::%s entries=%d" key.DeclaringType key.Name (List.length entries)

                for (paramKey, info) in entries do
                    if not (parameterRowLookup.ContainsKey paramKey) then
                        match info.RowId with
                        | Some rowId when rowId > 0 ->
                            parameterRowLookup[paramKey] <- rowId
                            parameterDefinitionIndex.AddExisting paramKey
                        | _ ->
                            let syntheticRow =
                                addSyntheticParameter key paramKey.SequenceNumber ParameterAttributes.None

                            if traceMethodUpdates.Value then
                                printfn
                                    "[fsharp-hotreload][param-fallback] synthesized baseline entry method=%s::%s seq=%d row=%d"
                                    key.DeclaringType
                                    key.Name
                                    paramKey.SequenceNumber
                                    syntheticRow
            | _ -> ()

        let isAdded = methodDefinitionIndex.IsAdded key
        let _ = ensureReturnParameterRow key isAdded
        ()

    orderedMethodInputs
    |> List.iter (fun struct (key, _, methodHandle, _, _) -> enqueueParameters key methodHandle)

    let registerPropertyDefinition key handle =
        if processedPropertyKeys.Add key then
            if propertyDefinitionIndex.IsAdded key then
                ()
            else
                propertyDefinitionIndex.AddExisting key

        propertyHandleLookup[key] <- handle

    let registerEventDefinition key handle =
        if processedEventKeys.Add key then
            if eventDefinitionIndex.IsAdded key then
                ()
            else
                eventDefinitionIndex.AddExisting key

        eventHandleLookup[key] <- handle

    let tryGetMethodToken key =
        match request.Baseline.MethodTokens |> Map.tryFind key with
        | Some token -> Some token
        | None ->
            match addedMethodDeltaTokens.TryGetValue key with
            | true, token -> Some token
            | _ -> None

    let tryResolveAccessor methodToken =
        // The accessor lookups below are keyed by FRESH reader handles; translate the
        // baseline-coordinate token first (same fresh-vs-baseline row rule as
        // tryBuildMethodUpdateInput).
        let readerToken =
            match freshMethodTokenByBaseline.TryGetValue methodToken with
            | true, freshToken -> freshToken
            | _ -> methodToken

        let methodHandle = MetadataTokens.MethodDefinitionHandle readerToken

        match propertyAccessorLookup.TryGetValue methodHandle with
        | true, propertyHandle ->
            if traceMethodUpdates.Value then
                printfn
                    "[fsharp-hotreload][accessor] property handle matched token=0x%08X"
                    (MetadataTokens.GetToken(EntityHandle.op_Implicit propertyHandle))

            let associationToken =
                MetadataTokens.GetToken(EntityHandle.op_Implicit propertyHandle)

            let baselineToken =
                definitionTokenRemapper.RemapPropertyAssociationToken associationToken

            match propertyTokenToKey.TryGetValue(baselineToken) with
            | true, key ->
                // Register the FRESH reader's handle: snapshot rows read their contents
                // through it (same fresh-vs-baseline coordinate rule as method rows). The
                // chained baseline token only names the emission row - indexing the fresh
                // reader with it reads a DISPLACED row once an earlier generation added
                // Property rows ahead of this one in the fresh layout.
                registerPropertyDefinition key propertyHandle
                true
            | _ -> false
        | _ ->
            if traceMethodUpdates.Value then
                printfn
                    "[fsharp-hotreload][accessor] property handle missing for method token=0x%08X"
                    (MetadataTokens.GetToken(EntityHandle.op_Implicit methodHandle))

            match eventAccessorLookup.TryGetValue methodHandle with
            | true, eventHandle ->
                let associationToken = MetadataTokens.GetToken(EntityHandle.op_Implicit eventHandle)

                let baselineToken =
                    definitionTokenRemapper.RemapEventAssociationToken associationToken

                match eventTokenToKey.TryGetValue(baselineToken) with
                | true, key ->
                    // Fresh handle for the same reason as the property branch above.
                    registerEventDefinition key eventHandle
                    true
                | _ -> false
            | _ -> false

    for accessor in request.UpdatedAccessors do
        let resolved =
            accessor.Method
            |> Option.bind tryGetMethodToken
            |> Option.exists tryResolveAccessor

        if not resolved then
            raise (
                HotReloadUnsupportedEditException(
                    $"Unable to resolve updated accessor '{accessor.Symbol.QualifiedName}' on '{accessor.ContainingType}' to emitted property or event metadata; please rebuild."
                )
            )

    let updatedTypeTokens =
        let baselineTokens =
            buildUpdatedTypeTokens
                tryGetBaselineTypeName
                request.Baseline.TypeTokens
                request.UpdatedTypes
                symbolChangeTypeNames
                resolvedMethods

        // Added types (new closure classes) are "changed types" of this delta too
        // (Roslyn parity: EmitDifferenceResult.ChangedTypes includes new TypeDefs).
        baselineTokens @ (addedTypeDeltaTokens.Values |> Seq.sort |> Seq.toList)
        |> List.distinct

    let updatedMethodTokenList =
        orderedMethodInputs
        |> List.map (fun struct (_, methodToken, _, _, _) -> methodToken)

    let hasAllocatedAddedRows =
        addedFieldDeltaTokens.Count > 0
        || addedPropertyDeltaTokens.Count > 0
        || addedEventDeltaTokens.Count > 0
        || addedTypeDeltaTokens.Count > 0

    if
        List.isEmpty methodUpdateInputs
        && List.isEmpty updatedTypeTokens
        && not hasAllocatedAddedRows
    then
        // No metadata/IL to emit. Line-shift-only edits land here: the delta carries only
        // SequencePointUpdates (plus the next committed sequence-point view) and consumes no
        // generation — there is nothing for the runtime to apply, the host just rebinds the
        // debugger's lines. (Roslyn emits a Module-row-only generation for this case; the F#
        // emitter deliberately skips the no-op ApplyUpdate. See
        // docs/hot-reload-active-statements.md.)
        let chainedSequencePoints =
            if Map.isEmpty freshSequencePointsByToken then
                None
            else
                let mutable chained = Map.empty

                for KeyValue(newToken, mappedToken) in matchedMethodTokenPairs do
                    match Map.tryFind newToken freshSequencePointsByToken with
                    | Some points -> chained <- Map.add mappedToken points chained
                    | None -> ()

                Some chained

        { emptyDelta with
            SequencePointUpdates = sequencePointUpdates
            ChainedSequencePoints = chainedSequencePoints
        }
    else
        let (methodUpdatesWithDefs, parameterDefinitionRowsSnapshot, methodDefinitionRowsSnapshot, methodSpecificationRowsSnapshot) =
            buildMethodAndParameterRows
                orderedMethodInputs
                metadataReader
                builder
                remapUserString
                remapEntityToken
                parameterDefinitionIndex.Rows
                parameterHandleLookup
                baselineParameterHandles
                syntheticParameterInfo
                firstParamRowByMethod
                returnParameterKeys
                methodDefinitionRowsRaw
                baselineMethodHandles
                request.Baseline.MethodTokens
                methodDefinitionIndex
                traceMetadata.Value
                baselineMethodSpecRowCount
                methodSpecRowsByToken
                (fun typeName ->
                    match request.Baseline.TypeTokens |> Map.tryFind typeName with
                    | Some token -> Some(token &&& 0x00FFFFFF)
                    | None ->
                        // Methods of an ADDED type are parented to its new delta TypeDef row.
                        match addedTypeDeltaTokens.TryGetValue typeName with
                        | true, token -> Some(token &&& 0x00FFFFFF)
                        | _ -> None)
                lastParamRowId

        let (propertyDefinitionRowsSnapshot,
             eventDefinitionRowsSnapshot,
             propertyMapRowsSnapshot,
             eventMapRowsSnapshot,
             methodSemanticsRowsSnapshot) =
            buildPropertyEventAndSemanticsRows
                traceMethodUpdates.Value
                request
                (fun typeName ->
                    match request.Baseline.TypeTokens |> Map.tryFind typeName with
                    | Some token -> Some token
                    | None ->
                        match addedTypeDeltaTokens.TryGetValue typeName with
                        | true, token -> Some token
                        | _ -> None)
                metadataReader
                propertyDefinitionIndex
                eventDefinitionIndex
                propertyHandleLookup
                eventHandleLookup
                baselinePropertyHandles
                baselineEventHandles
                baselineTableRowCounts
                definitionTokenRemapper.RemapDefinitionToken
                remapEntityToken
                (remapSignatureBlobWith (remapTypeDefOrRefCodedIndexWith remapEntityToken))

        let methodUpdates = methodUpdatesWithDefs |> List.map (fun (update, _, _) -> update)

        // Snapshot the added Field rows from the fresh compile. Field signatures (FieldSig,
        // ECMA-335 II.23.2.4) embed TypeDefOrRef coded indexes of the in-memory compile, so
        // they flow through the same signature-blob remapper as method/local signatures
        // before entering the delta blob heap.
        let fieldDefinitionRowsSnapshot: FieldDefinitionRowInfo list =
            addedFieldDeltaTokens
            |> Seq.sortBy (fun kvp -> kvp.Value)
            |> Seq.map (fun (KeyValue(key, deltaToken)) ->
                let newToken = addedFieldTokens.[key]
                let fieldHandle = MetadataTokens.FieldDefinitionHandle(newToken &&& 0x00FFFFFF)
                let fieldDef = metadataReader.GetFieldDefinition fieldHandle

                // FieldLayout (explicit [<FieldOffset>]), FieldMarshal (interop
                // marshalling) and FieldRVA (static data blobs, e.g. large array
                // initializers) rows have no writer support; shipping the Field row
                // without them silently corrupts layout/marshalling/data. Fail closed
                // precisely.
                if fieldDef.GetOffset() >= 0 then
                    raise (
                        HotReloadUnsupportedEditException(
                            $"Added field '{key.DeclaringType}::{key.Name}' declares an explicit offset (FieldLayout); hot reload deltas cannot express FieldLayout rows yet. Please rebuild."
                        )
                    )

                if not (fieldDef.GetMarshallingDescriptor().IsNil) then
                    raise (
                        HotReloadUnsupportedEditException(
                            $"Added field '{key.DeclaringType}::{key.Name}' carries marshalling metadata (FieldMarshal); hot reload deltas cannot express FieldMarshal rows yet. Please rebuild."
                        )
                    )

                if fieldDef.GetRelativeVirtualAddress() <> 0 then
                    raise (
                        HotReloadUnsupportedEditException(
                            $"Added field '{key.DeclaringType}::{key.Name}' maps static data (FieldRVA); hot reload deltas cannot express FieldRVA rows yet. Please rebuild."
                        )
                    )

                let signature =
                    metadataReader.GetBlobBytes fieldDef.Signature
                    |> remapSignatureBlobWith (remapTypeDefOrRefCodedIndexWith remapEntityToken)

                let parentRowId =
                    match request.Baseline.TypeTokens |> Map.tryFind key.DeclaringType with
                    | Some typeToken -> typeToken &&& 0x00FFFFFF
                    | None ->
                        match addedTypeDeltaTokens.TryGetValue key.DeclaringType with
                        | true, deltaTypeToken -> deltaTypeToken &&& 0x00FFFFFF
                        | _ ->
                            // Registration already gated on the baseline type token, so this only
                            // fires if the baseline maps changed under us; fail closed.
                            raise (
                                HotReloadUnsupportedEditException(
                                    $"Added field '{key.DeclaringType}::{key.Name}' has no baseline TypeDef token; please rebuild."
                                )
                            )

                {
                    FieldDefinitionRowInfo.Key = key
                    RowId = deltaToken &&& 0x00FFFFFF
                    IsAdded = true
                    ParentTypeDefRowId = parentRowId
                    Attributes = fieldDef.Attributes
                    Name = metadataReader.GetString fieldDef.Name
                    NameOffset = None
                    Signature = signature
                    SignatureOffset = None
                })
            |> Seq.toList

        if traceMethodUpdates.Value then
            for row in fieldDefinitionRowsSnapshot do
                printfn
                    "[fsharp-hotreload][field-add] %s::%s rowId=%d parentTypeDef=%d"
                    row.Key.DeclaringType
                    row.Key.Name
                    row.RowId
                    row.ParentTypeDefRowId

        // Constant rows for ADDED literal fields (enum members, union Tags holder
        // constants, [<Literal>] values): ECMA-335 II.22.9 — Type is the fresh compile's
        // ELEMENT_TYPE code, Parent the new delta Field row (HasConstant, tag Field),
        // Value the raw constant blob copied into the DELTA blob heap (primitive bytes,
        // nothing to remap). Rows are ordered by the parent Field row id (the table's
        // HasConstant sort key — all parents here are Fields) and numbered past the
        // chained baseline Constant row count. C# reference template: 'new_enum'
        // (reference_mdv_new_enum.txt) — three Constant rows trail the generation-1 log
        // as plain Default entries, EncMap adds.
        let constantRowsSnapshot: ConstantRowInfo list =
            let baselineConstantRowCount = baselineTableRowCounts.[TableNames.Constant.Index]

            addedFieldDeltaTokens
            |> Seq.sortBy (fun kvp -> kvp.Value)
            |> Seq.choose (fun (KeyValue(key, deltaToken)) ->
                let newToken = addedFieldTokens.[key]
                let fieldHandle = MetadataTokens.FieldDefinitionHandle(newToken &&& 0x00FFFFFF)
                let fieldDef = metadataReader.GetFieldDefinition fieldHandle
                let constantHandle = fieldDef.GetDefaultValue()

                if constantHandle.IsNil then
                    None
                else
                    let constant = metadataReader.GetConstant constantHandle

                    Some
                        {
                            ConstantRowInfo.RowId = 0
                            TypeCode = byte constant.TypeCode
                            Parent = HC_Field(FieldHandle(deltaToken &&& 0x00FFFFFF))
                            Value = metadataReader.GetBlobBytes constant.Value
                        })
            |> Seq.mapi (fun index row ->
                { row with
                    RowId = baselineConstantRowCount + index + 1
                })
            |> Seq.toList

        if traceMethodUpdates.Value then
            for row in constantRowsSnapshot do
                printfn
                    "[fsharp-hotreload][constant-add] rowId=%d parentFieldRow=%d typeCode=0x%02X valueBytes=%d"
                    row.RowId
                    row.Parent.RowId
                    (int row.TypeCode)
                    row.Value.Length

        // Snapshot ADDED TypeDef rows (closure classes synthesized for added lambdas).
        // Name/namespace/attributes come from the fresh compile's metadata; the Extends
        // coded index is remapped from fresh rows to baseline/delta rows. Nested types
        // (F# closure classes are nested in their module type) additionally produce a
        // NestedClass row pointing at the enclosing TypeDef.
        let typeDefinitionRowsSnapshot, nestedClassRowsSnapshot =
            if addedTypeDefs.Count = 0 then
                [], []
            else
                let baselineNestedClassRowCount = baselineTableRowCounts.[TableNames.Nested.Index]
                let typeRows = ResizeArray<TypeDefinitionRowInfo>()
                let nestedRows = ResizeArray<NestedClassRowInfo>()

                let resolveEnclosingTypeDefRowId (enclosing: ILTypeDef list) (fullName: string) =
                    match enclosing with
                    | [] -> None
                    | _ ->
                        let parentType = List.last enclosing
                        let parentEnclosing = enclosing |> List.take (List.length enclosing - 1)

                        let parentRef =
                            mkRefForNestedILTypeDef ILScopeRef.Local (parentEnclosing, parentType)

                        let parentBaselineName = tryGetBaselineTypeName parentRef.FullName

                        match request.Baseline.TypeTokens |> Map.tryFind parentBaselineName with
                        | Some token -> Some(token &&& 0x00FFFFFF)
                        | None ->
                            match addedTypeDeltaTokens.TryGetValue parentBaselineName with
                            | true, token -> Some(token &&& 0x00FFFFFF)
                            | _ ->
                                raise (
                                    HotReloadUnsupportedEditException(
                                        $"Added type '{fullName}' is nested in '{parentBaselineName}', which has no baseline or delta TypeDef row; please rebuild."
                                    )
                                )

                for (enclosing, _typeDef, fullName) in addedTypeDefs do
                    let deltaToken = addedTypeDeltaTokens.[fullName]
                    let rowId = deltaToken &&& 0x00FFFFFF
                    let newToken = addedTypeNewTokens.[fullName]
                    let typeHandle = MetadataTokens.TypeDefinitionHandle(newToken &&& 0x00FFFFFF)
                    let freshTypeDef = metadataReader.GetTypeDefinition typeHandle

                    let name =
                        match addedTypeMetadataNameByKey.TryGetValue fullName with
                        | true, metadataName -> metadataName
                        | _ -> metadataReader.GetString freshTypeDef.Name

                    // Explicit/sized layouts need a ClassLayout row (ECMA-335 II.22.8)
                    // the writer cannot express; shipping the TypeDef without it would
                    // corrupt the runtime layout. Fail closed precisely.
                    let typeLayout = freshTypeDef.GetLayout()

                    if not typeLayout.IsDefault then
                        raise (
                            HotReloadUnsupportedEditException(
                                $"Added type '{fullName}' declares an explicit layout (ClassLayout PackingSize={typeLayout.PackingSize}, Size={typeLayout.Size}); hot reload deltas cannot express ClassLayout rows yet. Please rebuild."
                            )
                        )

                    let namespaceName =
                        if freshTypeDef.Namespace.IsNil then
                            ""
                        else
                            metadataReader.GetString freshTypeDef.Namespace

                    let extends =
                        if freshTypeDef.BaseType.IsNil then
                            None
                        else
                            let baseToken = MetadataTokens.GetToken freshTypeDef.BaseType

                            match freshTypeDef.BaseType.Kind with
                            | HandleKind.TypeReference ->
                                let mapped = remapEntityToken baseToken
                                Some(TDR_TypeRef(TypeRefHandle(mapped &&& 0x00FFFFFF)))
                            | HandleKind.TypeDefinition ->
                                let mapped = definitionTokenRemapper.RemapDefinitionToken baseToken
                                Some(TDR_TypeDef(TypeDefHandle(mapped &&& 0x00FFFFFF)))
                            | HandleKind.TypeSpecification ->
                                // Content-validated TypeSpec remap: resolves to the matching
                                // baseline row, or appends a new TypeSpec row to the delta
                                // for a genuinely new instantiation (e.g. the first
                                // FSharpFunc<A,B> closure of that shape in the assembly).
                                let mapped = remapEntityToken baseToken
                                Some(TDR_TypeSpec(TypeSpecHandle(mapped &&& 0x00FFFFFF)))
                            | kind ->
                                raise (
                                    HotReloadUnsupportedEditException(
                                        $"Added type '{fullName}' has unsupported base-type handle kind {kind}; please rebuild."
                                    )
                                )

                    let enclosingRowId = resolveEnclosingTypeDefRowId enclosing fullName

                    typeRows.Add
                        {
                            TypeDefinitionRowInfo.FullName = fullName
                            RowId = rowId
                            Attributes = freshTypeDef.Attributes
                            Name = name
                            NameOffset = None
                            Namespace = namespaceName
                            NamespaceOffset = None
                            Extends = extends
                            EnclosingTypeDefRowId = enclosingRowId
                        }

                    match enclosingRowId with
                    | Some enclosingRow ->
                        nestedRows.Add
                            {
                                NestedClassRowInfo.RowId = baselineNestedClassRowCount + nestedRows.Count + 1
                                NestedTypeDefRowId = rowId
                                EnclosingTypeDefRowId = enclosingRow
                            }
                    | None -> ()

                typeRows |> Seq.sortBy (fun row -> row.RowId) |> Seq.toList, nestedRows |> Seq.toList

        if traceMethodUpdates.Value then
            for row in typeDefinitionRowsSnapshot do
                printfn
                    "[fsharp-hotreload][type-add] %s rowId=%d nestedIn=%A extends=%A"
                    row.FullName
                    row.RowId
                    row.EnclosingTypeDefRowId
                    row.Extends

        // GenericParam rows for ADDED generic methods and ADDED generic types (closure
        // classes over generic members). C# reference template (csharp_enc_reference
        // 'generic_method_add'): the added MethodDef's AddMethod/AddParameter pairs are
        // followed by a plain 'GenericParam ... Default' EncLog entry, and the row appears
        // in EncMap as an add. GenericParam rows of UPDATED definitions are baseline rows
        // and are never re-emitted. IL constraints on the type parameters emit
        // GenericParamConstraint rows owned by the new GenericParam rows (C# reference
        // template 'generic_constraint_add').
        let genericParamRowsSnapshot, genericParamConstraintRowsSnapshot =
            let pending = ResizeArray<int * GenericParameterHandle * TypeOrMethodDef * string>()

            let collect (display: string) (owner: TypeOrMethodDef) (handles: GenericParameterHandleCollection) =
                for handle in handles do
                    let genericParam = metadataReader.GetGenericParameter handle

                    if genericParam.GetCustomAttributes().Count > 0 then
                        raise (
                            HotReloadUnsupportedEditException(
                                $"Added generic definition '{display}' has type-parameter custom attributes, which hot reload deltas cannot emit yet; please rebuild."
                            )
                        )

                    pending.Add(genericParam.Index, handle, owner, display)

            for KeyValue(key, deltaToken) in addedMethodDeltaTokens do
                let freshToken = addedMethodTokens.[key]
                let methodHandle = MetadataTokens.MethodDefinitionHandle(freshToken &&& 0x00FFFFFF)
                let freshMethodDef = metadataReader.GetMethodDefinition methodHandle

                collect
                    $"{key.DeclaringType}::{key.Name}"
                    (TOMD_MethodDef(MethodDefHandle(deltaToken &&& 0x00FFFFFF)))
                    (freshMethodDef.GetGenericParameters())

            for (_, _, fullName) in addedTypeDefs do
                let deltaToken = addedTypeDeltaTokens.[fullName]
                let newToken = addedTypeNewTokens.[fullName]
                let typeHandle = MetadataTokens.TypeDefinitionHandle(newToken &&& 0x00FFFFFF)
                let freshTypeDef = metadataReader.GetTypeDefinition typeHandle

                collect fullName (TOMD_TypeDef(TypeDefHandle(deltaToken &&& 0x00FFFFFF))) (freshTypeDef.GetGenericParameters())

            let baselineGenericParamRowCount =
                baselineTableRowCounts.[TableNames.GenericParam.Index]

            // The GenericParam table is sorted by the Owner coded index (ECMA-335 II.22.20:
            // owner, then Number); keep the appended delta rows in the same relative order.
            let orderedParams =
                pending
                |> Seq.sortBy (fun (number, _, owner, _) -> (owner.RowId <<< 1) ||| owner.CodedTag, number)
                |> Seq.mapi (fun index (number, handle, owner, display) ->
                    let genericParam = metadataReader.GetGenericParameter handle

                    {
                        GenericParamRowInfo.RowId = baselineGenericParamRowCount + index + 1
                        Number = number
                        Attributes = genericParam.Attributes
                        Owner = owner
                        Name = metadataReader.GetString genericParam.Name
                        NameOffset = None
                    },
                    handle,
                    display)
                |> Seq.toList

            // Constraint rows are owned by the new GenericParam rows. The table sorts on
            // Owner (ECMA-335 II.22.21); the params are already in owner order, and the
            // constraints of one param keep their fresh-compile order.
            let baselineConstraintRowCount =
                baselineTableRowCounts.[TableNames.GenericParamConstraint.Index]

            let constraintRows =
                let rows = ResizeArray<GenericParamConstraintRowInfo>()

                for (paramRow, handle, display) in orderedParams do
                    let genericParam = metadataReader.GetGenericParameter handle

                    for constraintHandle in genericParam.GetConstraints() do
                        let genericConstraint =
                            metadataReader.GetGenericParameterConstraint constraintHandle

                        let constraintToken = MetadataTokens.GetToken genericConstraint.Type

                        let constraintRef =
                            match genericConstraint.Type.Kind with
                            | HandleKind.TypeReference -> TDR_TypeRef(TypeRefHandle(remapEntityToken constraintToken &&& 0x00FFFFFF))
                            | HandleKind.TypeDefinition ->
                                TDR_TypeDef(TypeDefHandle(definitionTokenRemapper.RemapDefinitionToken constraintToken &&& 0x00FFFFFF))
                            | HandleKind.TypeSpecification -> TDR_TypeSpec(TypeSpecHandle(remapEntityToken constraintToken &&& 0x00FFFFFF))
                            | kind ->
                                raise (
                                    HotReloadUnsupportedEditException(
                                        $"Added generic definition '{display}' has a type-parameter constraint with unsupported handle kind {kind}; please rebuild."
                                    )
                                )

                        rows.Add
                            {
                                GenericParamConstraintRowInfo.RowId = baselineConstraintRowCount + rows.Count + 1
                                OwnerGenericParamRowId = paramRow.RowId
                                Constraint = constraintRef
                            }

                rows |> List.ofSeq

            orderedParams |> List.map (fun (row, _, _) -> row), constraintRows

        if traceMethodUpdates.Value then
            for row in genericParamRowsSnapshot do
                printfn "[fsharp-hotreload][generic-param] rowId=%d number=%d owner=%A name=%s" row.RowId row.Number row.Owner row.Name

        // InterfaceImpl rows for ADDED types (C# 'new_class' reference template: the new
        // class's InterfaceImpl row is a plain Default EncLog entry trailing the log,
        // EncMap add) and MethodImpl rows for their explicit interface implementations
        // (F# classes implement interfaces explicitly — `interface X with` — so unlike
        // C#'s implicit public mapping every implemented slot carries a MethodImpl row;
        // records/unions map their synthesized comparers publicly and need none).
        let interfaceImplRowsSnapshot, methodImplRowsSnapshot =
            if addedTypeDefs.Count = 0 then
                [], []
            else
                let interfaceRows = ResizeArray<int * TypeDefOrRef>()
                let methodImplRows = ResizeArray<int * MethodDefOrRef * MethodDefOrRef>()

                let remapMethodDefOrRef (context: string) (handle: EntityHandle) =
                    let token = MetadataTokens.GetToken handle

                    match handle.Kind with
                    | HandleKind.MethodDefinition ->
                        MDOR_MethodDef(MethodDefHandle(definitionTokenRemapper.RemapDefinitionToken token &&& 0x00FFFFFF))
                    | HandleKind.MemberReference -> MDOR_MemberRef(MemberRefHandle(remapEntityToken token &&& 0x00FFFFFF))
                    | kind ->
                        raise (
                            HotReloadUnsupportedEditException(
                                $"Added type member implementation '{context}' has unsupported method handle kind {kind}; please rebuild."
                            )
                        )

                for (_, _, fullName) in addedTypeDefs do
                    let deltaToken = addedTypeDeltaTokens.[fullName]
                    let classRowId = deltaToken &&& 0x00FFFFFF
                    let newToken = addedTypeNewTokens.[fullName]
                    let typeHandle = MetadataTokens.TypeDefinitionHandle(newToken &&& 0x00FFFFFF)
                    let freshTypeDef = metadataReader.GetTypeDefinition typeHandle

                    for implHandle in freshTypeDef.GetInterfaceImplementations() do
                        let impl = metadataReader.GetInterfaceImplementation implHandle
                        let interfaceToken = MetadataTokens.GetToken impl.Interface

                        let interfaceRef =
                            match impl.Interface.Kind with
                            | HandleKind.TypeReference -> TDR_TypeRef(TypeRefHandle(remapEntityToken interfaceToken &&& 0x00FFFFFF))
                            | HandleKind.TypeDefinition ->
                                TDR_TypeDef(TypeDefHandle(definitionTokenRemapper.RemapDefinitionToken interfaceToken &&& 0x00FFFFFF))
                            | HandleKind.TypeSpecification -> TDR_TypeSpec(TypeSpecHandle(remapEntityToken interfaceToken &&& 0x00FFFFFF))
                            | kind ->
                                raise (
                                    HotReloadUnsupportedEditException(
                                        $"Added type '{fullName}' implements an interface with unsupported handle kind {kind}; please rebuild."
                                    )
                                )

                        interfaceRows.Add(classRowId, interfaceRef)

                    for implHandle in freshTypeDef.GetMethodImplementations() do
                        let impl = metadataReader.GetMethodImplementation implHandle

                        methodImplRows.Add(
                            classRowId,
                            remapMethodDefOrRef fullName impl.MethodBody,
                            remapMethodDefOrRef fullName impl.MethodDeclaration
                        )

                let baselineInterfaceImplRowCount =
                    baselineTableRowCounts.[TableNames.InterfaceImpl.Index]

                let baselineMethodImplRowCount =
                    baselineTableRowCounts.[TableNames.MethodImpl.Index]

                // Both tables sort on the Class column (ECMA-335 II.22.23 / II.22.27;
                // InterfaceImpl additionally on the Interface coded index); keep the
                // appended delta rows in the same relative order.
                let interfaceImplList =
                    interfaceRows
                    |> Seq.sortBy (fun (classRowId, interfaceRef) -> (classRowId, (interfaceRef.RowId <<< 2) ||| interfaceRef.CodedTag))
                    |> Seq.mapi (fun index (classRowId, interfaceRef) ->
                        {
                            InterfaceImplRowInfo.RowId = baselineInterfaceImplRowCount + index + 1
                            ClassTypeDefRowId = classRowId
                            Interface = interfaceRef
                        })
                    |> Seq.toList

                let methodImplList =
                    methodImplRows
                    |> Seq.sortBy (fun (classRowId, _, _) -> classRowId)
                    |> Seq.mapi (fun index (classRowId, body, declaration) ->
                        {
                            MethodImplRowInfo.RowId = baselineMethodImplRowCount + index + 1
                            ClassTypeDefRowId = classRowId
                            MethodBody = body
                            MethodDeclaration = declaration
                        })
                    |> Seq.toList

                interfaceImplList, methodImplList

        if traceMethodUpdates.Value then
            for row in interfaceImplRowsSnapshot do
                printfn "[fsharp-hotreload][interface-impl] rowId=%d class=%d interface=%A" row.RowId row.ClassTypeDefRowId row.Interface

            for row in methodImplRowsSnapshot do
                printfn
                    "[fsharp-hotreload][method-impl] rowId=%d class=%d body=%A declaration=%A"
                    row.RowId
                    row.ClassTypeDefRowId
                    row.MethodBody
                    row.MethodDeclaration

        let baselineHeapOffsets =
            request.Baseline.Metadata.HeapSizes |> MetadataHeapOffsets.OfHeapSizes

        let userStringEntries = userStringUpdates |> Seq.toList

        let customAttributeRowList =
            buildCustomAttributeRows
                traceMetadata.Value
                metadataReader
                baselineTableRowCounts
                request.Baseline.CustomAttributeRows
                methodUpdateInputs
                methodDefinitionRowsRaw
                methodDefinitionIndex
                methodUpdatesWithDefs
                {
                    AddedFieldTokens = addedFieldTokens
                    AddedFieldDeltaTokens = addedFieldDeltaTokens
                    AddedPropertyTokens = addedPropertyTokens
                    AddedPropertyDeltaTokens = addedPropertyDeltaTokens
                    AddedEventTokens = addedEventTokens
                    AddedEventDeltaTokens = addedEventDeltaTokens
                    AddedTypeNewTokens = addedTypeNewTokens
                    AddedTypeDeltaTokens = addedTypeDeltaTokens
                }
                remapEntityToken
                remapAssemblyRefToken
                baselineTypeReferenceTokens
                (fun () -> nextTypeRefRowId)
                (fun value -> nextTypeRefRowId <- value)
                (fun () -> nextMemberRefRowId)
                (fun value -> nextMemberRefRowId <- value)
                typeReferenceRows
                memberReferenceRows

        let typeReferenceRowList, memberReferenceRowList, assemblyReferenceRowList, typeSpecificationRowList =
            buildReferenceRows
                traceMetadata.Value
                typeReferenceRows
                memberReferenceRows
                assemblyReferenceRows
                typeSpecificationRows
                methodSpecificationRowsSnapshot
                customAttributeRowList

        let streams = builder.Build()

        let metadataDelta =
            emitMetadataDelta
                traceMetadata.Value
                moduleName
                baselineModuleNameOffset
                request.CurrentGeneration
                encId
                encBaseId
                moduleMvid
                typeDefinitionRowsSnapshot
                nestedClassRowsSnapshot
                interfaceImplRowsSnapshot
                methodImplRowsSnapshot
                constantRowsSnapshot
                methodDefinitionRowsSnapshot
                parameterDefinitionRowsSnapshot
                fieldDefinitionRowsSnapshot
                typeReferenceRowList
                memberReferenceRowList
                methodSpecificationRowsSnapshot
                typeSpecificationRowList
                genericParamRowsSnapshot
                genericParamConstraintRowsSnapshot
                assemblyReferenceRowList
                propertyDefinitionRowsSnapshot
                eventDefinitionRowsSnapshot
                propertyMapRowsSnapshot
                eventMapRowsSnapshot
                methodSemanticsRowsSnapshot
                streams.StandaloneSignatures
                customAttributeRowList
                userStringEntries
                methodUpdates
                baselineHeapOffsets
                request.Baseline.Metadata.TableRowCounts

        finalizeDeltaArtifacts
            request
            effectiveDebugPdb
            sequencePointUpdates
            freshSequencePointsByToken
            encId
            encBaseId
            metadataDelta
            streams
            updatedTypeTokens
            updatedMethodTokenList
            methodTokenMap
            matchedMethodTokenPairs
            userStringEntries
            methodDefinitionRowsSnapshot
            parameterDefinitionRowsSnapshot
            memberReferenceRowList
            typeSpecificationRowList
            customAttributeRowList
            propertyMapRowsSnapshot
            eventMapRowsSnapshot
            methodSemanticsRowsSnapshot
            methodTokenToKey
            addedMethodDeltaTokens
            addedFieldDeltaTokens
            addedPropertyDeltaTokens
            addedEventDeltaTokens
            addedTypeDeltaTokens
            addedTypeShapes
            addedTypeReferenceTokens
            addedAssemblyReferenceTokens

/// Emits a delta using only the in-memory rewrite's debug data (no sibling on-disk PDB).
let emitDelta (request: IlxDeltaRequest) : IlxDelta = emitDeltaWithDebugData None request
