/// PDB delta emission for hot reload. createSnapshot reads the baseline via the SRM-free
/// ILBaselineReader; emitDelta serializes the Portable PDB delta through SRM's PortablePdbBuilder.
module internal FSharp.Compiler.HotReloadPdb

open System
open System.Collections.Immutable
open System.Collections.Generic
open System.Reflection.Metadata
open System.Reflection.Metadata.Ecma335
open System.Security.Cryptography
open FSharp.Compiler.AbstractIL.BinaryConstants
open FSharp.Compiler.AbstractIL.ILDeltaHandles
open FSharp.Compiler.AbstractIL.ILPdbWriter
open FSharp.Compiler.HotReloadBaseline

module ILBaselineReader = FSharp.Compiler.AbstractIL.ILBaselineReader

let private shouldTracePdb () =
    let isEnabled (name: string) =
        match Environment.GetEnvironmentVariable(name) with
        | null -> false
        | value when String.Equals(value, "1", StringComparison.OrdinalIgnoreCase) -> true
        | value when String.Equals(value, "true", StringComparison.OrdinalIgnoreCase) -> true
        | _ -> false

    isEnabled "FSHARP_HOTRELOAD_TRACE_PDB"
    || isEnabled "FSHARP_HOTRELOAD_TRACE_METADATA"

/// Create a PDB snapshot from Portable PDB bytes.
/// Uses pure F# parsing instead of SRM for the reading path.
let private createPortablePdbContentIdProvider (checksumAlgorithm: HashAlgorithm) : Func<IEnumerable<Blob>, BlobContentId> =
    let algorithm =
        match checksumAlgorithm with
        | HashAlgorithm.Sha1 -> SHA1.Create() :> System.Security.Cryptography.HashAlgorithm
        | HashAlgorithm.Sha256 -> SHA256.Create() :> System.Security.Cryptography.HashAlgorithm

    Func<IEnumerable<Blob>, BlobContentId>(fun content ->
        let contentBytes = content |> Seq.collect (fun c -> c.GetBytes()) |> Array.ofSeq
        let hash = algorithm.ComputeHash contentBytes
        BlobContentId.FromHash hash)

let createSnapshot (pdbBytes: byte[]) : PortablePdbSnapshot =
    match ILBaselineReader.readPortablePdbMetadata pdbBytes with
    | None -> failwith "Failed to parse Portable PDB metadata"
    | Some pdbMeta ->
        // Convert PDB table row counts to full 64-element array
        // PDB tables start at index 0x30
        let counts = Array.zeroCreate<int> DeltaTokens.TableCount
        // pdbMeta.TableRowCounts has 8 elements (indices 0-7 map to PDB tables 0x30-0x37)
        counts.[DeltaTokens.tableDocument] <- pdbMeta.TableRowCounts.[0]
        counts.[DeltaTokens.tableMethodDebugInformation] <- pdbMeta.TableRowCounts.[1]
        counts.[DeltaTokens.tableLocalScope] <- pdbMeta.TableRowCounts.[2]
        counts.[DeltaTokens.tableLocalVariable] <- pdbMeta.TableRowCounts.[3]
        counts.[DeltaTokens.tableLocalConstant] <- pdbMeta.TableRowCounts.[4]
        counts.[DeltaTokens.tableImportScope] <- pdbMeta.TableRowCounts.[5]
        counts.[DeltaTokens.tableStateMachineMethod] <- pdbMeta.TableRowCounts.[6]
        counts.[DeltaTokens.tableCustomDebugInformation] <- pdbMeta.TableRowCounts.[7]

        {
            Bytes = Array.copy pdbBytes
            TableRowCounts = ImmutableArray.CreateRange counts
            EntryPointToken = pdbMeta.EntryPointToken
        }

/// Verifies that a portable PDB is the one named by the assembly's CodeView entry.
/// A mismatched sibling PDB must never seed EnC state for another module generation.
let matchesAssembly (assemblyBytes: byte[]) (pdbBytes: byte[]) =
    match
        ILBaselineReader.readCodeViewContentIdFromBytes assemblyBytes,
        ILBaselineReader.readPortablePdbMetadata pdbBytes
    with
    | Some expected, Some metadata -> metadata.ContentId.AsSpan().SequenceEqual(expected)
    | _ -> false

/// Creates a portable PDB snapshot only when its content ID matches the assembly.
let tryCreateSnapshotForAssembly (assemblyBytes: byte[]) (pdbBytes: byte[]) =
    if matchesAssembly assemblyBytes pdbBytes then
        Some(createSnapshot pdbBytes)
    else
        None

/// Emit a PDB delta for the given hot reload generation.
/// Takes the metadata EncLog and EncMap (using TableName for type safety)
/// and produces a Portable PDB delta that matches the metadata delta.
let emitDelta
    (baseline: FSharpEmitBaseline)
    (updatedPdbBytes: byte[])
    (addedOrChangedMethods: AddedOrChangedMethodInfo list)
    (deltaToUpdatedMethodToken: IReadOnlyDictionary<int, int>)
    (_metadataEncLog: (TableName * int * EditAndContinueOperation) array)
    (_metadataEncMap: (TableName * int) array)
    : byte[] option =
    match baseline.PortablePdb with
    | None -> None
    | Some _ ->
        // info.MethodToken values are BASELINE-coordinate MethodDef tokens (the row the
        // metadata delta re-emits the method at), NOT the fresh compile's tokens. Sort the
        // distinct tokens by their BASELINE MethodDef row so the PDB MethodDebugInformation
        // rows are appended in the same order the metadata writer sorts its method EncMap
        // entries (FSharpDeltaMetadataWriter.fs emits Method EncMap rows using row.RowId — the
        // baseline row — then sorts the whole EncMap ascending by token). Keeping both orders
        // identical is the ORDERING INVARIANT: the delta's Nth MethodDebugInformation row must
        // correspond to the Nth (baseline-row-sorted) PDB EncMap entry, or ApplyUpdate binds
        // sequence points to the wrong method. Sorting here (rather than later, only on the
        // EncMap) is required because for a multi-method delta after an add the fresh rows and
        // baseline rows no longer share an order.
        let distinctTokens =
            addedOrChangedMethods
            |> List.map (fun info -> info.MethodToken)
            |> List.distinct
            |> List.filter (fun token -> token <> 0)
            |> List.sortBy (fun token -> MetadataTokens.GetRowNumber(MetadataTokens.MethodDefinitionHandle token))

        if List.isEmpty distinctTokens then
            if shouldTracePdb () then
                printfn "[hotreload-pdb] distinct token list empty"

            None
        else
            use provider =
                MetadataReaderProvider.FromPortablePdbImage(ImmutableArray.CreateRange updatedPdbBytes)

            let reader = provider.GetMetadataReader()
            let metadata = MetadataBuilder()
            let documentMap = Dictionary<DocumentHandle, DocumentHandle>()
            let emittedMethodRows = ResizeArray<int>()
            let mutable emitted = false

            let getOrAddDocument (sourceHandle: DocumentHandle) =
                match documentMap.TryGetValue sourceHandle with
                | true, handle -> handle
                | _ ->
                    try
                        let document = reader.GetDocument sourceHandle
                        let name = reader.GetString document.Name

                        let hashBytes =
                            if document.Hash.IsNil then
                                Array.empty<byte>
                            else
                                reader.GetBlobBytes document.Hash

                        let hashAlgorithmGuid =
                            if document.HashAlgorithm.IsNil then
                                Guid.Empty
                            else
                                reader.GetGuid document.HashAlgorithm

                        let languageGuid =
                            if document.Language.IsNil then
                                Guid.Empty
                            else
                                reader.GetGuid document.Language

                        let nameHandle = metadata.GetOrAddDocumentName name
                        let hashHandle = metadata.GetOrAddBlob hashBytes
                        let hashAlgorithmHandle = metadata.GetOrAddGuid hashAlgorithmGuid
                        let languageHandle = metadata.GetOrAddGuid languageGuid

                        let added =
                            metadata.AddDocument(nameHandle, hashAlgorithmHandle, hashHandle, languageHandle)

                        documentMap[sourceHandle] <- added
                        added
                    with :? BadImageFormatException as ex ->
                        // Corrupted PDB metadata - skip this document gracefully
                        if shouldTracePdb () then
                            printfn "[hotreload-pdb] warning: could not read document (handle=%A): %s" sourceHandle ex.Message

                        DocumentHandle()

            for token in distinctTokens do
                let sourceToken =
                    match deltaToUpdatedMethodToken.TryGetValue token with
                    | true, mapped -> mapped
                    | _ -> token

                if sourceToken = 0 then
                    if shouldTracePdb () then
                        printfn "[hotreload-pdb] method token missing for delta token 0x%08x" token
                else
                    let sourceHandle = MetadataTokens.MethodDefinitionHandle sourceToken

                    if sourceHandle.IsNil then
                        if shouldTracePdb () then
                            printfn "[hotreload-pdb] source handle nil for delta token 0x%08x (source token=0x%08x)" token sourceToken
                    else
                        // Read at sourceHandle (fresh) and bounds-check against the fresh table
                        // count, but record the BASELINE MethodDef row in the EncMap: the metadata
                        // delta re-emits the method at its baseline row (FSharpDeltaMetadataWriter.fs),
                        // and the two EncMaps must agree even after an earlier edit shifts fresh rows.
                        let methodRow = MetadataTokens.GetRowNumber sourceHandle

                        let baselineMethodRow =
                            MetadataTokens.GetRowNumber(MetadataTokens.MethodDefinitionHandle token)

                        if methodRow <= reader.MethodDebugInformation.Count then
                            let methodInfo = reader.GetMethodDebugInformation sourceHandle

                            let targetDocument =
                                if methodInfo.Document.IsNil then
                                    DocumentHandle()
                                else
                                    getOrAddDocument methodInfo.Document

                            let sequencePointsHandle =
                                if methodInfo.SequencePointsBlob.IsNil then
                                    BlobHandle()
                                else
                                    metadata.GetOrAddBlob(reader.GetBlobBytes methodInfo.SequencePointsBlob)

                            metadata.AddMethodDebugInformation(targetDocument, sequencePointsHandle)
                            |> ignore

                            emittedMethodRows.Add(baselineMethodRow)
                            emitted <- true
                        else if
                            // A newly added method whose row exceeds the baseline
                            // MethodDebugInformation count has no debug info to re-emit here.
                            shouldTracePdb ()
                        then
                            let rowCount = reader.MethodDebugInformation.Count

                            printfn
                                $"[hotreload-pdb] skipping newly added method (row %d{methodRow} > count %d{rowCount}) - debugger stepping unavailable (delta=0x%08x{token}, source=0x%08x{sourceToken})"

            // Per Roslyn DeltaMetadataWriter.cs: PDB delta EncMap should contain MethodDebugInformation
            // entries (which correspond 1:1 to MethodDef), not metadata table entries. The PDB EncLog
            // is not used - only EncMap with MethodDebugInformation handles.
            // MethodDebugInformationHandle is a PDB-specific handle that doesn't implicitly convert
            // to EntityHandle, so we construct the EntityHandle from the table/row token directly.
            // Token format: (table_index << 24) | row_number, where MethodDebugInformation = 0x31
            //
            // ORDERING INVARIANT: the EncMap must be sorted by baseline row to match both the
            // appended MethodDebugInformation rows and the metadata writer's method EncMap
            // (FSharpDeltaMetadataWriter.fs), so the Nth row lines up with the Nth EncMap entry
            // and ApplyUpdate binds each method's sequence points correctly.
            for methodRow in emittedMethodRows |> Seq.distinct |> Seq.sort do
                let token = (DeltaTokens.tableMethodDebugInformation <<< 24) ||| methodRow
                let entityHandle = MetadataTokens.EntityHandle token
                metadata.AddEncMapEntry entityHandle

            if not emitted then
                if shouldTracePdb () then
                    printfn $"[hotreload-pdb] no method debug info emitted for tokens {distinctTokens}"

                None
            else
                let entryPointHandle = MethodDefinitionHandle()

                // Use shared content ID provider from ILPdbWriter
                let idProvider = createPortablePdbContentIdProvider HashAlgorithm.Sha256

                let zeroCounts =
                    ImmutableArray.CreateRange(Array.zeroCreate<int> DeltaTokens.TableCount)

                let builder = PortablePdbBuilder(metadata, zeroCounts, entryPointHandle, idProvider)
                let blobBuilder = BlobBuilder()
                builder.Serialize blobBuilder |> ignore
                Some(blobBuilder.ToArray())
