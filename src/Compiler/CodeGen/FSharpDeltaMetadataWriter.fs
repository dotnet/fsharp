module internal FSharp.Compiler.CodeGen.FSharpDeltaMetadataWriter

open System
open System.Collections.Generic
open FSharp.Compiler.EnvironmentHelpers
open Microsoft.FSharp.Collections
open FSharp.Compiler.AbstractIL.ILBinaryWriter
open FSharp.Compiler.AbstractIL.BinaryConstants
open FSharp.Compiler.AbstractIL.ILDeltaHandles
open FSharp.Compiler.IlxDeltaStreams
open FSharp.Compiler.HotReloadBaseline
open FSharp.Compiler.CodeGen.DeltaMetadataTables
open FSharp.Compiler.CodeGen.DeltaMetadataTypes
open FSharp.Compiler.CodeGen.DeltaTableLayout
open FSharp.Compiler.CodeGen.DeltaMetadataSerializer

[<Literal>]
let private TraceMetadataFlagName = "FSHARP_HOTRELOAD_TRACE_METADATA"

[<Literal>]
let private TraceHeapsFlagName = "FSHARP_HOTRELOAD_TRACE_HEAPS"

[<Literal>]
let private TraceMethodsFlagName = "FSHARP_HOTRELOAD_TRACE_METHODS"

// Keep a parallel SRM writer path available so metadata serializer changes can be validated
// (or temporarily switched) without touching the main delta construction pipeline.
[<Literal>]
let private UseSrmTablesFlagName = "FSHARP_HOTRELOAD_USE_SRM_TABLES"

[<Literal>]
let private CompareSrmMetadataFlagName = "FSHARP_HOTRELOAD_COMPARE_SRM_METADATA"

let private shouldTraceMetadata () = isEnvVarTruthy TraceMetadataFlagName

let private shouldTraceHeaps () = isEnvVarTruthy TraceHeapsFlagName

let private shouldTraceMethodRows () = isEnvVarTruthy TraceMethodsFlagName

let private shouldUseSrmMetadataWriter () = isEnvVarTruthy UseSrmTablesFlagName

let private shouldCompareSrmMetadataWriter () = isEnvVarTruthy CompareSrmMetadataFlagName

type MethodDefinitionRowInfo = DeltaMetadataTypes.MethodDefinitionRowInfo

type ParameterDefinitionRowInfo = DeltaMetadataTypes.ParameterDefinitionRowInfo

type MethodMetadataUpdate =
    {
        MethodKey: MethodDefinitionKey
        MethodToken: int
        MethodHandle: MethodDefHandle
        Body: MethodBodyUpdate
    }

type PropertyDefinitionRowInfo = DeltaMetadataTypes.PropertyDefinitionRowInfo

type EventDefinitionRowInfo = DeltaMetadataTypes.EventDefinitionRowInfo

type MethodSpecificationRowInfo = DeltaMetadataTypes.MethodSpecificationRowInfo

type PropertyMapRowInfo = DeltaMetadataTypes.PropertyMapRowInfo

type EventMapRowInfo = DeltaMetadataTypes.EventMapRowInfo

type MethodSemanticsMetadataUpdate = DeltaMetadataTypes.MethodSemanticsMetadataUpdate
type StandaloneSignatureUpdate = FSharp.Compiler.IlxDeltaStreams.StandaloneSignatureUpdate

/// Result of delta metadata emission.
/// Contains serialized metadata bytes and all supporting data structures.
type MetadataDelta =
    {
        Metadata: byte[]
        StringHeap: byte[]
        BlobHeap: byte[]
        GuidHeap: byte[]
        /// EncLog entries: (table, rowId, operation) using TableName from BinaryConstants
        EncLog: (TableName * int * EditAndContinueOperation) array
        /// EncMap entries: (table, rowId) using TableName from BinaryConstants
        EncMap: (TableName * int) array
        TableRowCounts: int[]
        HeapSizes: MetadataHeapSizes
        HeapOffsets: MetadataHeapOffsets
        Tables: TableRows
        TableBitMasks: TableBitMasks
        IndexSizes: DeltaIndexSizing.CodedIndexSizes
        TableStream: DeltaTableStream
        /// The EncId GUID for this generation (used as EncBaseId for subsequent generations)
        GenerationId: Guid
        /// The EncBaseId GUID (EncId of the previous generation, or Empty for generation 1)
        BaseGenerationId: Guid
    }


let emitWithUserStrings
    (moduleName: string)
    (moduleNameOffset: StringOffset option)
    (generation: int)
    (encId: Guid)
    (encBaseId: Guid)
    (moduleId: Guid)
    (methodDefinitionRows: MethodDefinitionRowInfo list)
    (parameterDefinitionRows: ParameterDefinitionRowInfo list)
    (typeReferenceRows: TypeReferenceRowInfo list)
    (memberReferenceRows: MemberReferenceRowInfo list)
    (methodSpecificationRows: MethodSpecificationRowInfo list)
    (assemblyReferenceRows: AssemblyReferenceRowInfo list)
    (propertyDefinitionRows: PropertyDefinitionRowInfo list)
    (eventDefinitionRows: EventDefinitionRowInfo list)
    (propertyMapRows: PropertyMapRowInfo list)
    (eventMapRows: EventMapRowInfo list)
    (methodSemanticsRows: MethodSemanticsMetadataUpdate list)
    (standaloneSignatureRows: StandaloneSignatureUpdate list)
    (customAttributeRows: CustomAttributeRowInfo list)
    (userStringUpdates: (int * int * string) list)
    (updates: MethodMetadataUpdate list)
    (heapOffsets: MetadataHeapOffsets)
    (externalRowCounts: int[])
    : MetadataDelta =
    if shouldTraceMetadata () then
        printfn "[fsharp-hotreload][metadata-writer] emit invoked updates=%d" (List.length updates)
        for row in methodDefinitionRows do
            let offset =
                match row.NameOffset with
                | Some (StringOffset o) -> Some o
                | None -> None
            printfn
                "[fsharp-hotreload][metadata-writer] method-row name=%s isAdded=%b offset=%A"
                row.Name
                row.IsAdded
                offset
    let normalizedExternalRowCounts =
        if externalRowCounts.Length = DeltaTokens.TableCount then
            externalRowCounts
        else
            Array.zeroCreate DeltaTokens.TableCount

    if List.isEmpty updates then
        let emptyMirror = DeltaMetadataTables(heapOffsets)
        let emptySizes = DeltaMetadataSerializer.computeMetadataSizes emptyMirror normalizedExternalRowCounts

        { Metadata = Array.empty
          StringHeap = Array.empty
          BlobHeap = Array.empty
          GuidHeap = Array.empty
          EncLog = Array.empty
          EncMap = Array.empty
          TableRowCounts = emptySizes.RowCounts
          HeapSizes = emptySizes.HeapSizes
          HeapOffsets = heapOffsets
          Tables = emptyMirror.TableRows
          TableBitMasks = emptySizes.BitMasks
          IndexSizes = emptySizes.IndexSizes
          TableStream =
            { Bytes = Array.empty
              UnpaddedSize = 0
              PaddedSize = 0 }
          GenerationId = encId
          BaseGenerationId = encBaseId }
    else

        if shouldTraceMetadata () then
            printfn
                "[fsharp-hotreload][metadata-writer] generation=%d moduleId=%A encId=%A encBaseId=%A"
                generation
                moduleId
                encId
                encBaseId
        let useSrmMetadataWriter = shouldUseSrmMetadataWriter ()
        let compareSrmMetadataWriter = shouldCompareSrmMetadataWriter ()
        let enableSrmShadowWriter = useSrmMetadataWriter || compareSrmMetadataWriter
        let tableMirror = DeltaMetadataTables(heapOffsets)
        tableMirror.AddModuleRow(moduleName, moduleNameOffset, generation, moduleId, encId, encBaseId)

        let updatesByKey = Dictionary<MethodDefinitionKey, MethodMetadataUpdate>(HashIdentity.Structural)
        let updatesByKeyBody = Dictionary<MethodDefinitionKey, MethodBodyUpdate>(HashIdentity.Structural)

        for update in updates do
            updatesByKey[update.MethodKey] <- update
            updatesByKeyBody[update.MethodKey] <- update.Body

        // Build EncLog and EncMap entries using TableName for type safety.
        // EncLog records each modification; EncMap provides sorted token listing.
        let mutable encLog = ResizeArray<struct (TableName * int * EditAndContinueOperation)>()
        let mutable encMap = ResizeArray<struct (TableName * int)>()

        // Module row is always present in deltas
        encLog.Add(struct (TableNames.Module, 1, EditAndContinueOperation.Default))
        encMap.Add(struct (TableNames.Module, 1))

        for row in methodDefinitionRows do
            match updatesByKey.TryGetValue row.Key with
            | true, update ->
                tableMirror.AddMethodRow(row, update.Body)
                if shouldTraceMethodRows () then
                    printfn
                        "[fsharp-hotreload][writer] method-row key=%s::%s rowId=%d isAdded=%b"
                        row.Key.DeclaringType
                        row.Key.Name
                        row.RowId
                        row.IsAdded

                let operation = if row.IsAdded then EditAndContinueOperation.AddMethod else EditAndContinueOperation.Default
                encLog.Add(struct (TableNames.Method, row.RowId, operation))
                encMap.Add(struct (TableNames.Method, row.RowId))
            | _ ->
                if shouldTraceMetadata () then
                    printfn "[fsharp-hotreload][metadata-writer] missing update payload for %A" row.Key

        for row in parameterDefinitionRows do
            tableMirror.AddParameterRow row

            let operation = if row.IsAdded then EditAndContinueOperation.AddParameter else EditAndContinueOperation.Default
            encLog.Add(struct (TableNames.Param, row.RowId, operation))
            encMap.Add(struct (TableNames.Param, row.RowId))

        for row in typeReferenceRows do
            tableMirror.AddTypeReferenceRow row

            encLog.Add(struct (TableNames.TypeRef, row.RowId, EditAndContinueOperation.Default))
            encMap.Add(struct (TableNames.TypeRef, row.RowId))

        for row in memberReferenceRows do
            tableMirror.AddMemberReferenceRow row

            encLog.Add(struct (TableNames.MemberRef, row.RowId, EditAndContinueOperation.Default))
            encMap.Add(struct (TableNames.MemberRef, row.RowId))

        for row in methodSpecificationRows do
            tableMirror.AddMethodSpecificationRow row

            encLog.Add(struct (TableNames.MethodSpec, row.RowId, EditAndContinueOperation.Default))
            encMap.Add(struct (TableNames.MethodSpec, row.RowId))

        for row in assemblyReferenceRows do
            tableMirror.AddAssemblyReferenceRow row

            encLog.Add(struct (TableNames.AssemblyRef, row.RowId, EditAndContinueOperation.Default))
            encMap.Add(struct (TableNames.AssemblyRef, row.RowId))

        for signature in standaloneSignatureRows do
            let rowId = signature.RowId
            tableMirror.AddStandaloneSignatureRow(signature.Blob)

            let operation = EditAndContinueOperation.Default
            encLog.Add(struct (TableNames.StandAloneSig, rowId, operation))
            encMap.Add(struct (TableNames.StandAloneSig, rowId))

        for row in customAttributeRows do
            tableMirror.AddCustomAttributeRow row

            encLog.Add(struct (TableNames.CustomAttribute, row.RowId, EditAndContinueOperation.Default))
            encMap.Add(struct (TableNames.CustomAttribute, row.RowId))

        for row in propertyDefinitionRows do
            if row.IsAdded then
                tableMirror.AddPropertyRow row

                encLog.Add(struct (TableNames.Property, row.RowId, EditAndContinueOperation.AddProperty))
                encMap.Add(struct (TableNames.Property, row.RowId))

        for row in eventDefinitionRows do
            if row.IsAdded then
                tableMirror.AddEventRow row

                encLog.Add(struct (TableNames.Event, row.RowId, EditAndContinueOperation.AddEvent))
                encMap.Add(struct (TableNames.Event, row.RowId))

        for row in propertyMapRows do
            if row.IsAdded then
                encLog.Add(struct (TableNames.PropertyMap, row.RowId, EditAndContinueOperation.AddProperty))
                encMap.Add(struct (TableNames.PropertyMap, row.RowId))
                tableMirror.AddPropertyMapRow row

        for row in eventMapRows do
            if row.IsAdded then
                encLog.Add(struct (TableNames.EventMap, row.RowId, EditAndContinueOperation.AddEvent))
                encMap.Add(struct (TableNames.EventMap, row.RowId))
                tableMirror.AddEventMapRow row

        for row in methodSemanticsRows do
            if row.IsAdded then
                tableMirror.AddMethodSemanticsRow row

                encLog.Add(struct (TableNames.MethodSemantics, row.RowId, EditAndContinueOperation.AddMethod))
                encMap.Add(struct (TableNames.MethodSemantics, row.RowId))

        for _, newToken, literal in userStringUpdates do
            let offset = newToken &&& 0x00FFFFFF
            tableMirror.AddUserStringLiteral(offset, literal)

        // Sort EncLog entries by table order (Roslyn's canonical ordering), then by row ID.
        // This ensures consistent delta format across generations.
        let encLogEntries =
            let snapshot = encLog |> Seq.toArray
            // Roslyn orders EncLog by this specific table sequence
            let orderedTables =
                [| TableNames.Module
                   TableNames.Method
                   TableNames.Param
                   TableNames.TypeRef
                   TableNames.MemberRef
                   TableNames.MethodSpec
                   TableNames.AssemblyRef
                   TableNames.StandAloneSig
                   TableNames.CustomAttribute
                   TableNames.Property
                   TableNames.Event
                   TableNames.PropertyMap
                   TableNames.EventMap
                   TableNames.MethodSemantics |]

            let orderedTableSet = orderedTables |> Set.ofArray
            let builder = ResizeArray()
            let appendEntries (table: TableName) =
                snapshot
                |> Seq.filter (fun struct (t, _, _) -> t.Index = table.Index)
                |> Seq.sortBy (fun struct (_, rowId, _) -> rowId)
                |> Seq.iter builder.Add

            orderedTables |> Array.iter appendEntries

            // Any tables not in the canonical order are appended sorted by token
            snapshot
            |> Seq.filter (fun struct (table, _, _) -> not (orderedTableSet |> Set.exists (fun t -> t.Index = table.Index)))
            |> Seq.sortBy (fun struct (table, rowId, _) ->
                (table.Index <<< 24) ||| (rowId &&& 0x00FFFFFF))
            |> Seq.iter builder.Add

            builder.ToArray()

        // Sort EncMap entries by token (table index << 24 | row ID)
        let encMapEntries =
            encMap
            |> Seq.sortBy (fun struct (table, rowId) ->
                (table.Index <<< 24) ||| (rowId &&& 0x00FFFFFF))
            |> Seq.toArray

        // Write EncLog and EncMap rows to the mirror
        for struct (table, rowId, operation) in encLogEntries do
            tableMirror.AddEncLogRow(table, rowId, operation)

        for struct (table, rowId) in encMapEntries do
            tableMirror.AddEncMapRow(table, rowId)

        let metadataSizes = DeltaMetadataSerializer.computeMetadataSizes tableMirror normalizedExternalRowCounts
        let tableRowCounts = metadataSizes.RowCounts
        let tableBitMasks = metadataSizes.BitMasks
        let indexSizes = metadataSizes.IndexSizes

        let tableStreamInput =
            { DeltaMetadataSerializer.DeltaTableSerializerInput.Tables = tableMirror.TableRows
              MetadataSizes = metadataSizes
              StringHeap = tableMirror.StringHeapBytes
              StringHeapOffsets = tableMirror.StringHeapOffsets
              BlobHeap = tableMirror.BlobHeapBytes
              BlobHeapOffsets = tableMirror.BlobHeapOffsets
              GuidHeap = tableMirror.GuidHeapBytes
              HeapOffsets = heapOffsets }

        let tableStream = DeltaMetadataSerializer.buildTableStream tableStreamInput
        let heapStreams = DeltaMetadataSerializer.buildHeapStreams tableMirror
        let abstractMetadataBytes = DeltaMetadataSerializer.serializeMetadataRoot tableStreamInput heapStreams tableStream

        let srmMetadataBytes =
            if enableSrmShadowWriter then
                Some(
                    DeltaMetadataSrmWriter.serialize
                        moduleName
                        moduleNameOffset
                        generation
                        encId
                        encBaseId
                        moduleId
                        methodDefinitionRows
                        parameterDefinitionRows
                        typeReferenceRows
                        memberReferenceRows
                        methodSpecificationRows
                        assemblyReferenceRows
                        propertyDefinitionRows
                        eventDefinitionRows
                        propertyMapRows
                        eventMapRows
                        methodSemanticsRows
                        standaloneSignatureRows
                        customAttributeRows
                        userStringUpdates
                        updatesByKeyBody
                        encLogEntries
                        encMapEntries)
            else
                None

        if enableSrmShadowWriter then
            match srmMetadataBytes with
            | Some srmBytes ->
                match DeltaMetadataSrmWriter.compareMetadataStructure abstractMetadataBytes srmBytes with
                | Some message -> failwithf "Hot reload metadata SRM parity mismatch: %s" message
                | None -> ()
            | None -> ()

        let metadataBytes =
            match srmMetadataBytes, useSrmMetadataWriter with
            | Some srmBytes, true -> srmBytes
            | _ -> abstractMetadataBytes

        if shouldTraceMetadata () then
            printfn
                "[fsharp-hotreload][index-sizes] stringsBig=%b guidsBig=%b blobsBig=%b"
                indexSizes.StringsBig
                indexSizes.GuidsBig
                indexSizes.BlobsBig
            let methodRows = tableRowCounts[TableNames.Method.Index]
            let paramRows = tableRowCounts[TableNames.Param.Index]
            let propertyRows = tableRowCounts[TableNames.Property.Index]
            let eventRows = tableRowCounts[TableNames.Event.Index]
            printfn
                "[fsharp-hotreload][metadata-writer] rows method=%d param=%d property=%d event=%d stringHeap=%d blobHeap=%d guidHeap=%d"
                methodRows
                paramRows
                propertyRows
                eventRows
                heapStreams.StringsLength
                heapStreams.BlobsLength
                heapStreams.GuidsLength

        if shouldTraceHeaps () then
            printfn
                "[fsharp-hotreload][heap-summary] baseline:string=%d blob=%d guid=%d | delta:string=%d blob=%d guid=%d"
                heapOffsets.StringHeapStart
                heapOffsets.BlobHeapStart
                heapOffsets.GuidHeapStart
                heapStreams.StringsLength
                heapStreams.BlobsLength
                heapStreams.GuidsLength
            printfn "[fsharp-hotreload][heap-bytes] blob-bytes=%A" heapStreams.Blobs

        // HeapSizes should match what SRM's GetHeapSize returns:
        // - StringHeap: SRM trims trailing zeros, so use unpadded size
        // - UserStringHeap, BlobHeap, GuidHeap: SRM does NOT trim, so use padded size (stream header size)
        // This is important for EnC offset calculations via MetadataAggregator
        let heapSizes : MetadataHeapSizes =
            { StringHeapSize = tableMirror.StringHeapBytes.Length  // unpadded - SRM trims trailing zeros
              UserStringHeapSize = heapStreams.UserStringsLength   // padded - SRM does not trim
              BlobHeapSize = heapStreams.BlobsLength               // padded - SRM does not trim
              GuidHeapSize = heapStreams.GuidsLength }             // padded - SRM does not trim

        { Metadata = metadataBytes
          StringHeap = heapStreams.Strings
          BlobHeap = heapStreams.Blobs
          GuidHeap = heapStreams.Guids
          EncLog = encLogEntries |> Array.map (fun struct (a, b, c) -> (a, b, c))
          EncMap = encMapEntries |> Array.map (fun struct (a, b) -> (a, b))
          TableRowCounts = tableRowCounts
          HeapSizes = heapSizes
          HeapOffsets = heapOffsets
          Tables = tableMirror.TableRows
          TableBitMasks = tableBitMasks
          IndexSizes = indexSizes
          TableStream = tableStream
          GenerationId = encId
          BaseGenerationId = encBaseId }

let emitWithReferences
    (moduleName: string)
    (moduleNameOffset: StringOffset option)
    (generation: int)
    (encId: Guid)
    (encBaseId: Guid)
    (moduleId: Guid)
    (methodDefinitionRows: MethodDefinitionRowInfo list)
    (parameterDefinitionRows: ParameterDefinitionRowInfo list)
    (typeReferenceRows: TypeReferenceRowInfo list)
    (memberReferenceRows: MemberReferenceRowInfo list)
    (methodSpecificationRows: MethodSpecificationRowInfo list)
    (assemblyReferenceRows: AssemblyReferenceRowInfo list)
    (propertyDefinitionRows: PropertyDefinitionRowInfo list)
    (eventDefinitionRows: EventDefinitionRowInfo list)
    (propertyMapRows: PropertyMapRowInfo list)
    (eventMapRows: EventMapRowInfo list)
    (methodSemanticsRows: MethodSemanticsMetadataUpdate list)
    (standaloneSignatureRows: StandaloneSignatureUpdate list)
    (customAttributeRows: CustomAttributeRowInfo list)
    (userStringUpdates: (int * int * string) list)
    (updates: MethodMetadataUpdate list)
    (heapOffsets: MetadataHeapOffsets)
    (externalRowCounts: int[])
    : MetadataDelta =
    emitWithUserStrings
        moduleName
        moduleNameOffset
        generation
        encId
        encBaseId
        moduleId
        methodDefinitionRows
        parameterDefinitionRows
        typeReferenceRows
        memberReferenceRows
        methodSpecificationRows
        assemblyReferenceRows
        propertyDefinitionRows
        eventDefinitionRows
        propertyMapRows
        eventMapRows
        methodSemanticsRows
        standaloneSignatureRows
        customAttributeRows
        userStringUpdates
        updates
        heapOffsets
        externalRowCounts

let emit
    (moduleName: string)
    (moduleNameOffset: StringOffset option)
    (generation: int)
    (encId: Guid)
    (encBaseId: Guid)
    (moduleId: Guid)
    (methodDefinitionRows: MethodDefinitionRowInfo list)
    (parameterDefinitionRows: ParameterDefinitionRowInfo list)
    (propertyDefinitionRows: PropertyDefinitionRowInfo list)
    (eventDefinitionRows: EventDefinitionRowInfo list)
    (propertyMapRows: PropertyMapRowInfo list)
    (eventMapRows: EventMapRowInfo list)
    (methodSemanticsRows: MethodSemanticsMetadataUpdate list)
    (standaloneSignatureRows: StandaloneSignatureUpdate list)
    (customAttributeRows: CustomAttributeRowInfo list)
    (updates: MethodMetadataUpdate list)
    (heapOffsets: MetadataHeapOffsets)
    (externalRowCounts: int[])
    : MetadataDelta =
    emitWithReferences
        moduleName
        moduleNameOffset
        generation
        encId
        encBaseId
        moduleId
        methodDefinitionRows
        parameterDefinitionRows
        []
        []
        []
        []
        propertyDefinitionRows
        eventDefinitionRows
        propertyMapRows
        eventMapRows
        methodSemanticsRows
        standaloneSignatureRows
        customAttributeRows
        ([] : (int * int * string) list)
        updates
        heapOffsets
        externalRowCounts
