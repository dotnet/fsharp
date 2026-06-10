/// PDB delta emission for hot reload.
///
/// SRM Boundary Note:
/// - createSnapshot: Uses pure F# parsing via ILBaselineReader (SRM-free)
/// - emitDelta: Uses SRM's MetadataBuilder and PortablePdbBuilder for PDB delta serialization
///
/// Full SRM removal from emitDelta would require implementing a pure F# Portable PDB
/// delta writer. This is deferred as non-blocking work since:
/// 1. Core metadata delta emission is fully SRM-free (DeltaMetadataTables, DeltaMetadataSerializer)
/// 2. PDB deltas are a separate concern (debug info only)
/// 3. The PDB read path is already SRM-free
module internal FSharp.Compiler.HotReloadPdb

open System
open System.Collections.Immutable
open System.Collections.Generic
open System.Collections.Immutable
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

    isEnabled "FSHARP_HOTRELOAD_TRACE_PDB" || isEnabled "FSHARP_HOTRELOAD_TRACE_METADATA"

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
        // Index 6 = StateMachineMethod (0x36), not commonly used
        counts.[DeltaTokens.tableCustomDebugInformation] <- pdbMeta.TableRowCounts.[7]

        { Bytes = Array.copy pdbBytes
          TableRowCounts = ImmutableArray.CreateRange counts
          EntryPointToken = pdbMeta.EntryPointToken }

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
    | Some snapshot ->
        let distinctTokens =
            addedOrChangedMethods
            |> List.map (fun info -> info.MethodToken)
            |> List.distinct
            |> List.filter (fun token -> token <> 0)

        if List.isEmpty distinctTokens then
            if shouldTracePdb () then
                printfn "[hotreload-pdb] distinct token list empty"
            None
        else
            use provider = MetadataReaderProvider.FromPortablePdbImage(ImmutableArray.CreateRange updatedPdbBytes)
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
                        let nameBytes = reader.GetBlobBytes document.Name
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

                        let nameHandle = metadata.GetOrAddBlob nameBytes
                        let hashHandle = metadata.GetOrAddBlob hashBytes
                        let hashAlgorithmHandle = metadata.GetOrAddGuid hashAlgorithmGuid
                        let languageHandle = metadata.GetOrAddGuid languageGuid

                        let added =
                            metadata.AddDocument(nameHandle, hashAlgorithmHandle, hashHandle, languageHandle)

                        documentMap[sourceHandle] <- added
                        added
                    with
                    | :? BadImageFormatException as ex ->
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
                        let methodRow = MetadataTokens.GetRowNumber sourceHandle

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

                            metadata.AddMethodDebugInformation(targetDocument, sequencePointsHandle) |> ignore
                            emittedMethodRows.Add(methodRow)
                            emitted <- true
                        else
                            // Newly added methods may not have debug info in the updated PDB if their row
                            // exceeds the MethodDebugInformation table count. This is a known limitation -
                            // debuggers won't be able to step into newly added methods until a full rebuild.
                            // TODO: Emit empty MethodDebugInformation entries for new methods to enable debugging.
                            if shouldTracePdb () then
                                let rowCount = reader.MethodDebugInformation.Count
                                printfn
                                    $"[hotreload-pdb] skipping newly added method (row %d{methodRow} > count %d{rowCount}) - debugger stepping unavailable (delta=0x%08x{token}, source=0x%08x{sourceToken})"

            // Per Roslyn DeltaMetadataWriter.cs: PDB delta EncMap should contain MethodDebugInformation
            // entries (which correspond 1:1 to MethodDef), not metadata table entries. The PDB EncLog
            // is not used - only EncMap with MethodDebugInformation handles.
            // MethodDebugInformationHandle is a PDB-specific handle that doesn't implicitly convert
            // to EntityHandle, so we construct the EntityHandle from the table/row token directly.
            // Token format: (table_index << 24) | row_number, where MethodDebugInformation = 0x31
            for methodRow in emittedMethodRows |> Seq.distinct |> Seq.sort do
                let token = (DeltaTokens.tableMethodDebugInformation <<< 24) ||| methodRow
                let entityHandle = MetadataTokens.EntityHandle token
                metadata.AddEncMapEntry entityHandle

            if not emitted then
                if shouldTracePdb () then
                    printfn $"[hotreload-pdb] no method debug info emitted for tokens {distinctTokens}"
                None
            else
                let entryPointHandle =
                    match snapshot.EntryPointToken with
                    | Some token -> MetadataTokens.MethodDefinitionHandle token
                    | None -> MethodDefinitionHandle()

                // Use shared content ID provider from ILPdbWriter
                let idProvider = createPortablePdbContentIdProvider HashAlgorithm.Sha256

                let zeroCounts =
                    ImmutableArray.CreateRange(Array.zeroCreate<int> DeltaTokens.TableCount)

                let builder = PortablePdbBuilder(metadata, zeroCounts, entryPointHandle, idProvider)
                let blobBuilder = BlobBuilder()
                builder.Serialize blobBuilder |> ignore
                Some(blobBuilder.ToArray())
