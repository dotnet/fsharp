module internal FSharp.Compiler.AbstractIL.FSharpDeltaMetadataWriter

open System
open System.Collections.Generic
open Microsoft.FSharp.Collections
open FSharp.Compiler.AbstractIL.ILBinaryWriter
open FSharp.Compiler.AbstractIL.BinaryConstants
open FSharp.Compiler.AbstractIL.ILDeltaHandles
open FSharp.Compiler.AbstractIL.IlxDeltaStreams
open FSharp.Compiler.AbstractIL.DeltaMetadataTables
open FSharp.Compiler.AbstractIL.DeltaMetadataTypes
open FSharp.Compiler.AbstractIL.DeltaTableLayout
open FSharp.Compiler.AbstractIL.DeltaMetadataSerializer

[<Literal>]
let private TraceMetadataFlagName = "FSHARP_HOTRELOAD_TRACE_METADATA"

[<Literal>]
let private TraceHeapsFlagName = "FSHARP_HOTRELOAD_TRACE_HEAPS"

[<Literal>]
let private TraceMethodsFlagName = "FSHARP_HOTRELOAD_TRACE_METHODS"

/// Local copy of FSharp.Compiler.EnvironmentHelpers.isEnvVarTruthy. That module is a new
/// utility file added by the hot-reload feature branch and isn't part of this extraction's
/// scope, so the writer's trace-flag checks carry their own tiny copy instead of pulling in
/// an extra out-of-scope file.
let private isEnvVarTruthy (name: string) =
    match Environment.GetEnvironmentVariable(name) with
    | null
    | "" -> false
    | value when String.Equals(value, "1", StringComparison.OrdinalIgnoreCase) -> true
    | value when String.Equals(value, "true", StringComparison.OrdinalIgnoreCase) -> true
    | _ -> false

let private shouldTraceMetadata () = isEnvVarTruthy TraceMetadataFlagName

let private shouldTraceHeaps () = isEnvVarTruthy TraceHeapsFlagName

let private shouldTraceMethodRows () = isEnvVarTruthy TraceMethodsFlagName

type MethodDefinitionRowInfo = DeltaMetadataTypes.MethodDefinitionRowInfo

type ParameterDefinitionRowInfo = DeltaMetadataTypes.ParameterDefinitionRowInfo

type FieldDefinitionRowInfo = DeltaMetadataTypes.FieldDefinitionRowInfo

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

type TypeSpecificationRowInfo = DeltaMetadataTypes.TypeSpecificationRowInfo

type GenericParamRowInfo = DeltaMetadataTypes.GenericParamRowInfo

type GenericParamConstraintRowInfo = DeltaMetadataTypes.GenericParamConstraintRowInfo

type PropertyMapRowInfo = DeltaMetadataTypes.PropertyMapRowInfo

type EventMapRowInfo = DeltaMetadataTypes.EventMapRowInfo

type MethodSemanticsMetadataUpdate = DeltaMetadataTypes.MethodSemanticsMetadataUpdate
type StandaloneSignatureUpdate = FSharp.Compiler.AbstractIL.IlxDeltaStreams.StandaloneSignatureUpdate

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

let emitWithTypeDefinitions
    (moduleName: string)
    (moduleNameOffset: StringOffset option)
    (generation: int)
    (encId: Guid)
    (encBaseId: Guid)
    (moduleId: Guid)
    (typeDefinitionRows: TypeDefinitionRowInfo list)
    (nestedClassRows: NestedClassRowInfo list)
    (interfaceImplRows: InterfaceImplRowInfo list)
    (methodImplRows: MethodImplRowInfo list)
    (constantRows: ConstantRowInfo list)
    (methodDefinitionRows: MethodDefinitionRowInfo list)
    (parameterDefinitionRows: ParameterDefinitionRowInfo list)
    (fieldDefinitionRows: FieldDefinitionRowInfo list)
    (typeReferenceRows: TypeReferenceRowInfo list)
    (memberReferenceRows: MemberReferenceRowInfo list)
    (methodSpecificationRows: MethodSpecificationRowInfo list)
    (typeSpecificationRows: TypeSpecificationRowInfo list)
    (genericParamRows: GenericParamRowInfo list)
    (genericParamConstraintRows: GenericParamConstraintRowInfo list)
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
                | Some(StringOffset o) -> Some o
                | None -> None

            printfn "[fsharp-hotreload][metadata-writer] method-row name=%s isAdded=%b offset=%A" row.Name row.IsAdded offset

    let normalizedExternalRowCounts =
        if externalRowCounts.Length = DeltaTokens.TableCount then
            externalRowCounts
        else
            Array.zeroCreate DeltaTokens.TableCount

    // A delta can carry row additions without any method-body update: a [<DefaultValue>]
    // instance field appends a Field row but changes no constructor. Only
    // short-circuit when there is genuinely nothing to write.
    let hasRowPayload =
        not (List.isEmpty updates)
        || not (List.isEmpty typeDefinitionRows)
        || not (List.isEmpty nestedClassRows)
        || not (List.isEmpty methodDefinitionRows)
        || not (List.isEmpty parameterDefinitionRows)
        || not (List.isEmpty fieldDefinitionRows)
        || not (List.isEmpty typeReferenceRows)
        || not (List.isEmpty memberReferenceRows)
        || not (List.isEmpty methodSpecificationRows)
        || not (List.isEmpty typeSpecificationRows)
        || not (List.isEmpty genericParamRows)
        || not (List.isEmpty genericParamConstraintRows)
        || not (List.isEmpty assemblyReferenceRows)
        || not (List.isEmpty interfaceImplRows)
        || not (List.isEmpty methodImplRows)
        || not (List.isEmpty constantRows)
        || not (List.isEmpty propertyDefinitionRows)
        || not (List.isEmpty eventDefinitionRows)
        || not (List.isEmpty propertyMapRows)
        || not (List.isEmpty eventMapRows)
        || not (List.isEmpty methodSemanticsRows)
        || not (List.isEmpty standaloneSignatureRows)
        || not (List.isEmpty customAttributeRows)

    if not hasRowPayload then
        let emptyMirror = DeltaMetadataTables(heapOffsets)

        let emptySizes =
            DeltaMetadataSerializer.computeMetadataSizes emptyMirror normalizedExternalRowCounts

        {
            Metadata = Array.empty
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
                {
                    Bytes = Array.empty
                    UnpaddedSize = 0
                    PaddedSize = 0
                }
            GenerationId = encId
            BaseGenerationId = encBaseId
        }
    else

        if shouldTraceMetadata () then
            printfn
                "[fsharp-hotreload][metadata-writer] generation=%d moduleId=%A encId=%A encBaseId=%A"
                generation
                moduleId
                encId
                encBaseId

        let tableMirror = DeltaMetadataTables(heapOffsets)
        tableMirror.AddModuleRow(moduleName, moduleNameOffset, generation, moduleId, encId, encBaseId)

        let updatesByKey =
            Dictionary<MethodDefinitionKey, MethodMetadataUpdate>(HashIdentity.Structural)

        for update in updates do
            updatesByKey[update.MethodKey] <- update

        // Build EncLog and EncMap entries using TableName for type safety.
        // EncLog records each modification; EncMap provides sorted token listing.
        let mutable encLog =
            ResizeArray<struct (TableName * int * EditAndContinueOperation)>()

        let mutable encMap = ResizeArray<struct (TableName * int)>()

        // Module row is always present in deltas
        encLog.Add(struct (TableNames.Module, 1, EditAndContinueOperation.Default))
        encMap.Add(struct (TableNames.Module, 1))

        // ---------------------------------------------------------------------------------
        // EncLog shape for ADDED members (Roslyn DeltaMetadataWriter.PopulateEncLogTableRows
        // parity, verified against a hotreload-delta-gen C# reference delta and the CLR's
        // EnC applier CMiniMdRW::ApplyDelta): an added member is logged as its PARENT row
        // tagged with the Add* operation, immediately followed by the new member row with
        // the Default operation. The runtime reads the parent token from the Add* entry and
        // links the member created by the FOLLOWING entry into the parent's member list, so
        // each pair must stay adjacent and the parent must already exist when processed:
        //   AddMethod / AddField -> parent TypeDef row
        //   AddParameter         -> parent MethodDef row
        //   AddProperty/AddEvent -> parent PropertyMap/EventMap row
        // Only the added member row (never the parent entry) appears in EncMap.
        // ---------------------------------------------------------------------------------
        let methodEncLogEntries =
            ResizeArray<struct (TableName * int * EditAndContinueOperation)>()

        let methodRowsByKey =
            Dictionary<MethodDefinitionKey, MethodDefinitionRowInfo>(HashIdentity.Structural)

        // Added TypeDef rows are logged as plain Default entries (the row content is
        // applied via ApplyTableDelta, like PropertyMap/EventMap rows) and MUST precede
        // every AddField/AddMethod entry that names them as the parent. C# reference
        // (csharp_enc_reference, added capturing lambda -> new display class): the new
        // TypeDef row's Default entry comes immediately before its AddField/AddMethod
        // member pairs; the NestedClass row trails at the end of the log.
        let typeDefEncLogEntries =
            ResizeArray<struct (TableName * int * EditAndContinueOperation)>()

        for row in typeDefinitionRows |> List.sortBy (fun row -> row.RowId) do
            tableMirror.AddTypeDefinitionRow row
            typeDefEncLogEntries.Add(struct (TableNames.TypeDef, row.RowId, EditAndContinueOperation.Default))
            encMap.Add(struct (TableNames.TypeDef, row.RowId))

        let nestedClassEncLogEntries =
            ResizeArray<struct (TableName * int * EditAndContinueOperation)>()

        for row in nestedClassRows |> List.sortBy (fun row -> row.RowId) do
            tableMirror.AddNestedClassRow row
            nestedClassEncLogEntries.Add(struct (TableNames.Nested, row.RowId, EditAndContinueOperation.Default))
            encMap.Add(struct (TableNames.Nested, row.RowId))

        // InterfaceImpl/MethodImpl rows of ADDED types are plain Default adds applied via
        // ApplyTableDelta. The C# 'new_class' reference template logs the InterfaceImpl
        // row trailing the generation-1 log; MethodImpl rows (F#'s explicit interface
        // implementations) follow the same shape.
        let interfaceImplEncLogEntries =
            ResizeArray<struct (TableName * int * EditAndContinueOperation)>()

        for row in interfaceImplRows |> List.sortBy (fun row -> row.RowId) do
            tableMirror.AddInterfaceImplRow row
            interfaceImplEncLogEntries.Add(struct (TableNames.InterfaceImpl, row.RowId, EditAndContinueOperation.Default))
            encMap.Add(struct (TableNames.InterfaceImpl, row.RowId))

        let methodImplEncLogEntries =
            ResizeArray<struct (TableName * int * EditAndContinueOperation)>()

        for row in methodImplRows |> List.sortBy (fun row -> row.RowId) do
            tableMirror.AddMethodImplRow row
            methodImplEncLogEntries.Add(struct (TableNames.MethodImpl, row.RowId, EditAndContinueOperation.Default))
            encMap.Add(struct (TableNames.MethodImpl, row.RowId))

        // Constant rows (literal values of ADDED fields) are plain Default adds trailing
        // the log: the C# 'new_enum' reference template logs the three Constant rows of
        // an added enum LAST, after the member pairs and the updated-method rows.
        let constantEncLogEntries =
            ResizeArray<struct (TableName * int * EditAndContinueOperation)>()

        for row in constantRows |> List.sortBy (fun row -> row.RowId) do
            tableMirror.AddConstantRow row
            constantEncLogEntries.Add(struct (TableNames.Constant, row.RowId, EditAndContinueOperation.Default))
            encMap.Add(struct (TableNames.Constant, row.RowId))

        for row in methodDefinitionRows do
            match updatesByKey.TryGetValue row.Key with
            | true, update ->
                tableMirror.AddMethodRow(row, update.Body)
                methodRowsByKey[row.Key] <- row

                if shouldTraceMethodRows () then
                    printfn
                        "[fsharp-hotreload][writer] method-row key=%s::%s rowId=%d isAdded=%b"
                        row.Key.DeclaringType
                        row.Key.Name
                        row.RowId
                        row.IsAdded

                if row.IsAdded then
                    match row.ParentTypeDefRowId with
                    | Some parentRowId ->
                        methodEncLogEntries.Add(struct (TableNames.TypeDef, parentRowId, EditAndContinueOperation.AddMethod))
                    | None ->
                        invalidOp
                            $"Added method '{row.Key.DeclaringType}::{row.Key.Name}' has no parent TypeDef row id; the AddMethod EncLog entry cannot be emitted."

                methodEncLogEntries.Add(struct (TableNames.Method, row.RowId, EditAndContinueOperation.Default))
                encMap.Add(struct (TableNames.Method, row.RowId))
            | _ ->
                if shouldTraceMetadata () then
                    printfn "[fsharp-hotreload][metadata-writer] missing update payload for %A" row.Key

        let parameterEncLogEntries =
            ResizeArray<struct (TableName * int * EditAndContinueOperation)>()

        for row in parameterDefinitionRows do
            tableMirror.AddParameterRow row

            if row.IsAdded then
                match methodRowsByKey.TryGetValue row.Key.Method with
                | true, methodRow ->
                    parameterEncLogEntries.Add(struct (TableNames.Method, methodRow.RowId, EditAndContinueOperation.AddParameter))
                | _ ->
                    invalidOp
                        $"Added parameter (sequence {row.SequenceNumber}) of '{row.Key.Method.DeclaringType}::{row.Key.Method.Name}' has no method row; the AddParameter EncLog entry cannot be emitted."

            parameterEncLogEntries.Add(struct (TableNames.Param, row.RowId, EditAndContinueOperation.Default))
            encMap.Add(struct (TableNames.Param, row.RowId))

        for row in fieldDefinitionRows do
            if row.IsAdded then
                tableMirror.AddFieldRow row
                encMap.Add(struct (TableNames.Field, row.RowId))

        let fieldEncLogPairs =
            fieldDefinitionRows
            |> List.filter (fun row -> row.IsAdded)
            |> List.sortBy (fun row -> row.RowId)
            |> List.collect (fun row ->
                [
                    struct (TableNames.TypeDef, row.ParentTypeDefRowId, EditAndContinueOperation.AddField)
                    struct (TableNames.Field, row.RowId, EditAndContinueOperation.Default)
                ])

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

        // Appended TypeSpec rows (new generic instantiations) are plain Default adds
        // applied via ApplyTableDelta, exactly like the C# reference template's
        // "TypeSpec 0x1b00xxxx Default" entry for an added-lambda delta.
        for row in typeSpecificationRows do
            tableMirror.AddTypeSpecificationRow row

            encLog.Add(struct (TableNames.TypeSpec, row.RowId, EditAndContinueOperation.Default))
            encMap.Add(struct (TableNames.TypeSpec, row.RowId))

        // GenericParam rows of ADDED generic methods/types are plain Default adds applied
        // via ApplyTableDelta — the C# reference template ('generic_method_add') logs
        // 'GenericParam 0x2a000001 Default' trailing the AddMethod/AddParameter pairs and
        // lists the row in EncMap. Kept as a dedicated group appended after the parameter
        // pairs so the owning method rows are already logged.
        let genericParamEncLogEntries =
            ResizeArray<struct (TableName * int * EditAndContinueOperation)>()

        for row in genericParamRows |> List.sortBy (fun row -> row.RowId) do
            tableMirror.AddGenericParamRow row
            genericParamEncLogEntries.Add(struct (TableNames.GenericParam, row.RowId, EditAndContinueOperation.Default))
            encMap.Add(struct (TableNames.GenericParam, row.RowId))

        // GenericParamConstraint rows of ADDED generic definitions are plain Default
        // adds trailing the GenericParam entries (C# reference template
        // 'generic_constraint_add': GenericParamConstraint 0x2c000001 Default follows
        // GenericParam 0x2a000001 Default; both EncMap adds).
        for row in genericParamConstraintRows |> List.sortBy (fun row -> row.RowId) do
            tableMirror.AddGenericParamConstraintRow row
            genericParamEncLogEntries.Add(struct (TableNames.GenericParamConstraint, row.RowId, EditAndContinueOperation.Default))
            encMap.Add(struct (TableNames.GenericParamConstraint, row.RowId))

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

        // Newly created PropertyMap/EventMap rows are logged as plain Default entries (the
        // row content is applied via ApplyTableDelta) and MUST precede the AddProperty /
        // AddEvent entries that reference them as parents.
        let propertyMapEncLogEntries =
            ResizeArray<struct (TableName * int * EditAndContinueOperation)>()

        let propertyMapRowIdByType = Dictionary<string, int>(StringComparer.Ordinal)

        for row in propertyMapRows do
            if row.IsAdded then
                tableMirror.AddPropertyMapRow row
                propertyMapEncLogEntries.Add(struct (TableNames.PropertyMap, row.RowId, EditAndContinueOperation.Default))
                encMap.Add(struct (TableNames.PropertyMap, row.RowId))

            propertyMapRowIdByType[row.DeclaringType] <- row.RowId

        let eventMapEncLogEntries =
            ResizeArray<struct (TableName * int * EditAndContinueOperation)>()

        let eventMapRowIdByType = Dictionary<string, int>(StringComparer.Ordinal)

        for row in eventMapRows do
            if row.IsAdded then
                tableMirror.AddEventMapRow row
                eventMapEncLogEntries.Add(struct (TableNames.EventMap, row.RowId, EditAndContinueOperation.Default))
                encMap.Add(struct (TableNames.EventMap, row.RowId))

            eventMapRowIdByType[row.DeclaringType] <- row.RowId

        let propertyEncLogEntries =
            ResizeArray<struct (TableName * int * EditAndContinueOperation)>()

        for row in propertyDefinitionRows do
            if row.IsAdded then
                tableMirror.AddPropertyRow row

                let parentMapRowId =
                    match row.ParentPropertyMapRowId with
                    | Some rowId -> rowId
                    | None ->
                        match propertyMapRowIdByType.TryGetValue row.Key.DeclaringType with
                        | true, rowId -> rowId
                        | _ ->
                            invalidOp
                                $"Added property '{row.Key.DeclaringType}::{row.Key.Name}' has no parent PropertyMap row id; the AddProperty EncLog entry cannot be emitted."

                propertyEncLogEntries.Add(struct (TableNames.PropertyMap, parentMapRowId, EditAndContinueOperation.AddProperty))
                propertyEncLogEntries.Add(struct (TableNames.Property, row.RowId, EditAndContinueOperation.Default))
                encMap.Add(struct (TableNames.Property, row.RowId))

        let eventEncLogEntries =
            ResizeArray<struct (TableName * int * EditAndContinueOperation)>()

        for row in eventDefinitionRows do
            if row.IsAdded then
                tableMirror.AddEventRow row

                let parentMapRowId =
                    match row.ParentEventMapRowId with
                    | Some rowId -> rowId
                    | None ->
                        match eventMapRowIdByType.TryGetValue row.Key.DeclaringType with
                        | true, rowId -> rowId
                        | _ ->
                            invalidOp
                                $"Added event '{row.Key.DeclaringType}::{row.Key.Name}' has no parent EventMap row id; the AddEvent EncLog entry cannot be emitted."

                eventEncLogEntries.Add(struct (TableNames.EventMap, parentMapRowId, EditAndContinueOperation.AddEvent))
                eventEncLogEntries.Add(struct (TableNames.Event, row.RowId, EditAndContinueOperation.Default))
                encMap.Add(struct (TableNames.Event, row.RowId))

        // MethodSemantics rows are logged as plain Default entries (Roslyn parity); the CLR
        // applies them via ApplyTableDelta like any other appended row.
        let methodSemanticsEncLogEntries =
            ResizeArray<struct (TableName * int * EditAndContinueOperation)>()

        for row in methodSemanticsRows do
            if row.IsAdded then
                tableMirror.AddMethodSemanticsRow row

                methodSemanticsEncLogEntries.Add(struct (TableNames.MethodSemantics, row.RowId, EditAndContinueOperation.Default))
                encMap.Add(struct (TableNames.MethodSemantics, row.RowId))

        for _, newToken, literal in userStringUpdates do
            let offset = newToken &&& 0x00FFFFFF
            tableMirror.AddUserStringLiteral(offset, literal)

        // Assemble the EncLog. Groups follow the established F# ordering (Module first, then
        // member tables, then reference tables); parent/member Add* pairs are appended as
        // pre-built adjacent sequences so no per-table sorting can separate a parent entry
        // from the member row it creates. Map rows precede the Add* entries that use them as
        // parents, and method entries precede the parameter pairs that reference them.
        let encLogEntries =
            let snapshot = encLog |> Seq.toArray

            let referenceTables =
                [|
                    TableNames.TypeRef
                    TableNames.MemberRef
                    TableNames.MethodSpec
                    TableNames.TypeSpec
                    TableNames.AssemblyRef
                    TableNames.StandAloneSig
                    TableNames.CustomAttribute
                |]

            let handledTables =
                Set.ofList
                    [
                        TableNames.Module.Index
                        yield! referenceTables |> Seq.map (fun t -> t.Index)
                    ]

            let builder = ResizeArray()

            let appendEntries (table: TableName) =
                snapshot
                |> Seq.filter (fun struct (t, _, _) -> t.Index = table.Index)
                |> Seq.sortBy (fun struct (_, rowId, _) -> rowId)
                |> Seq.iter builder.Add

            appendEntries TableNames.Module
            // ECMA table order: TypeDef (0x02) / Field (0x04) precede Method (0x06); Roslyn
            // likewise logs added-field pairs ahead of the method rows that consume them.
            // New TypeDef rows come first of all: their Default entries must be applied
            // before any AddField/AddMethod pair that names them as the parent.
            builder.AddRange typeDefEncLogEntries
            fieldEncLogPairs |> List.iter builder.Add
            builder.AddRange methodEncLogEntries
            builder.AddRange parameterEncLogEntries
            // GenericParam rows trail the method/parameter pairs that introduced their
            // owners (C# reference order: the GenericParam Default entry is logged after
            // the AddParameter pair of the added generic method).
            builder.AddRange genericParamEncLogEntries
            referenceTables |> Array.iter appendEntries
            builder.AddRange propertyMapEncLogEntries
            builder.AddRange propertyEncLogEntries
            builder.AddRange eventMapEncLogEntries
            builder.AddRange eventEncLogEntries
            builder.AddRange methodSemanticsEncLogEntries
            // InterfaceImpl/MethodImpl rows trail the log (C# reference order: the
            // 'new_class' template's InterfaceImpl entry is the last log entry), followed
            // by NestedClass rows; the CLR applies all three via ApplyTableDelta after
            // the new TypeDef row already exists.
            builder.AddRange interfaceImplEncLogEntries
            builder.AddRange methodImplEncLogEntries
            builder.AddRange nestedClassEncLogEntries
            // Constant rows trail the whole log (C# 'new_enum' reference order); the CLR
            // only needs their parent Field rows applied first.
            builder.AddRange constantEncLogEntries

            // Any tables not handled above are appended sorted by token.
            snapshot
            |> Seq.filter (fun struct (table, _, _) -> not (handledTables |> Set.contains table.Index))
            |> Seq.sortBy (fun struct (table, rowId, _) -> (table.Index <<< 24) ||| (rowId &&& 0x00FFFFFF))
            |> Seq.iter builder.Add

            builder.ToArray()

        // Sort EncMap entries by token (table index << 24 | row ID)
        let encMapEntries =
            encMap
            |> Seq.sortBy (fun struct (table, rowId) -> (table.Index <<< 24) ||| (rowId &&& 0x00FFFFFF))
            |> Seq.toArray

        // Write EncLog and EncMap rows to the mirror
        for struct (table, rowId, operation) in encLogEntries do
            tableMirror.AddEncLogRow(table, rowId, operation)

        for struct (table, rowId) in encMapEntries do
            tableMirror.AddEncMapRow(table, rowId)

        let metadataSizes =
            DeltaMetadataSerializer.computeMetadataSizes tableMirror normalizedExternalRowCounts

        let tableRowCounts = metadataSizes.RowCounts
        let tableBitMasks = metadataSizes.BitMasks
        let indexSizes = metadataSizes.IndexSizes

        let tableStreamInput =
            {
                DeltaMetadataSerializer.DeltaTableSerializerInput.Tables = tableMirror.TableRows
                MetadataSizes = metadataSizes
                StringHeap = tableMirror.StringHeapBytes
                StringHeapOffsets = tableMirror.StringHeapOffsets
                BlobHeap = tableMirror.BlobHeapBytes
                BlobHeapOffsets = tableMirror.BlobHeapOffsets
                GuidHeap = tableMirror.GuidHeapBytes
                HeapOffsets = heapOffsets
            }

        let tableStream = DeltaMetadataSerializer.buildTableStream tableStreamInput
        let heapStreams = DeltaMetadataSerializer.buildHeapStreams tableMirror

        let metadataBytes =
            DeltaMetadataSerializer.serializeMetadataRoot tableStreamInput heapStreams tableStream

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
        let heapSizes: MetadataHeapSizes =
            {
                StringHeapSize = tableMirror.StringHeapBytes.Length // unpadded - SRM trims trailing zeros
                UserStringHeapSize = heapStreams.UserStringsLength // padded - SRM does not trim
                BlobHeapSize = heapStreams.BlobsLength // padded - SRM does not trim
                GuidHeapSize = heapStreams.GuidsLength
            } // padded - SRM does not trim

        {
            Metadata = metadataBytes
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
            BaseGenerationId = encBaseId
        }

/// Back-compat entry point without added TypeDef/NestedClass rows.
let emitWithUserStrings
    (moduleName: string)
    (moduleNameOffset: StringOffset option)
    (generation: int)
    (encId: Guid)
    (encBaseId: Guid)
    (moduleId: Guid)
    (methodDefinitionRows: MethodDefinitionRowInfo list)
    (parameterDefinitionRows: ParameterDefinitionRowInfo list)
    (fieldDefinitionRows: FieldDefinitionRowInfo list)
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
    emitWithTypeDefinitions
        moduleName
        moduleNameOffset
        generation
        encId
        encBaseId
        moduleId
        ([]: TypeDefinitionRowInfo list)
        ([]: NestedClassRowInfo list)
        ([]: InterfaceImplRowInfo list)
        ([]: MethodImplRowInfo list)
        ([]: ConstantRowInfo list)
        methodDefinitionRows
        parameterDefinitionRows
        fieldDefinitionRows
        typeReferenceRows
        memberReferenceRows
        methodSpecificationRows
        ([]: TypeSpecificationRowInfo list)
        ([]: GenericParamRowInfo list)
        ([]: GenericParamConstraintRowInfo list)
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

let emitWithReferences
    (moduleName: string)
    (moduleNameOffset: StringOffset option)
    (generation: int)
    (encId: Guid)
    (encBaseId: Guid)
    (moduleId: Guid)
    (methodDefinitionRows: MethodDefinitionRowInfo list)
    (parameterDefinitionRows: ParameterDefinitionRowInfo list)
    (fieldDefinitionRows: FieldDefinitionRowInfo list)
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
        fieldDefinitionRows
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
        ([]: FieldDefinitionRowInfo list)
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
        ([]: (int * int * string) list)
        updates
        heapOffsets
        externalRowCounts
