module internal FSharp.Compiler.CodeGen.DeltaMetadataSrmWriter

open System
open System.Collections.Generic
open System.Reflection
open System.Reflection.Metadata
open System.Reflection.Metadata.Ecma335
open FSharp.Compiler.AbstractIL.BinaryConstants
open FSharp.Compiler.AbstractIL.ILDeltaHandles
open FSharp.Compiler.IlxDeltaStreams
open FSharp.Compiler.HotReloadBaseline
open FSharp.Compiler.CodeGen.DeltaMetadataTypes

let private toTableIndex (table: TableName) : TableIndex =
    LanguagePrimitives.EnumOfValue<byte, TableIndex>(byte table.Index)

let private toEntityHandle (table: TableName) (rowId: int) : EntityHandle =
    MetadataTokens.Handle(toTableIndex table, rowId)
    |> EntityHandle.op_Explicit

let private toResolutionScopeHandle (scope: ResolutionScope) : EntityHandle =
    match scope with
    | RS_Module handle -> toEntityHandle TableNames.Module handle.RowId
    | RS_ModuleRef handle -> toEntityHandle TableNames.ModuleRef handle.RowId
    | RS_AssemblyRef handle -> toEntityHandle TableNames.AssemblyRef handle.RowId
    | RS_TypeRef handle -> toEntityHandle TableNames.TypeRef handle.RowId

let private toMemberRefParentHandle (parent: MemberRefParent) : EntityHandle =
    match parent with
    | MRP_TypeDef handle -> toEntityHandle TableNames.TypeDef handle.RowId
    | MRP_TypeRef handle -> toEntityHandle TableNames.TypeRef handle.RowId
    | MRP_ModuleRef handle -> toEntityHandle TableNames.ModuleRef handle.RowId
    | MRP_MethodDef handle -> toEntityHandle TableNames.Method handle.RowId
    | MRP_TypeSpec handle -> toEntityHandle TableNames.TypeSpec handle.RowId

let private toMethodDefOrRefHandle (methodRef: MethodDefOrRef) : EntityHandle =
    match methodRef with
    | MDOR_MethodDef handle -> toEntityHandle TableNames.Method handle.RowId
    | MDOR_MemberRef handle -> toEntityHandle TableNames.MemberRef handle.RowId

let private toTypeDefOrRefHandle (eventType: TypeDefOrRef) : EntityHandle =
    match eventType with
    | TDR_TypeDef handle -> toEntityHandle TableNames.TypeDef handle.RowId
    | TDR_TypeRef handle -> toEntityHandle TableNames.TypeRef handle.RowId
    | TDR_TypeSpec handle -> toEntityHandle TableNames.TypeSpec handle.RowId

let private toHasCustomAttributeHandle (parent: HasCustomAttribute) : EntityHandle =
    match parent with
    | HCA_MethodDef handle -> toEntityHandle TableNames.Method handle.RowId
    | HCA_Field handle -> toEntityHandle TableNames.Field handle.RowId
    | HCA_TypeRef handle -> toEntityHandle TableNames.TypeRef handle.RowId
    | HCA_TypeDef handle -> toEntityHandle TableNames.TypeDef handle.RowId
    | HCA_Param handle -> toEntityHandle TableNames.Param handle.RowId
    | HCA_InterfaceImpl handle -> toEntityHandle TableNames.InterfaceImpl handle.RowId
    | HCA_MemberRef handle -> toEntityHandle TableNames.MemberRef handle.RowId
    | HCA_Module handle -> toEntityHandle TableNames.Module handle.RowId
    | HCA_DeclSecurity handle -> toEntityHandle TableNames.Permission handle.RowId
    | HCA_Property handle -> toEntityHandle TableNames.Property handle.RowId
    | HCA_Event handle -> toEntityHandle TableNames.Event handle.RowId
    | HCA_StandAloneSig handle -> toEntityHandle TableNames.StandAloneSig handle.RowId
    | HCA_ModuleRef handle -> toEntityHandle TableNames.ModuleRef handle.RowId
    | HCA_TypeSpec handle -> toEntityHandle TableNames.TypeSpec handle.RowId
    | HCA_Assembly handle -> toEntityHandle TableNames.Assembly handle.RowId
    | HCA_AssemblyRef handle -> toEntityHandle TableNames.AssemblyRef handle.RowId
    | HCA_File handle -> toEntityHandle TableNames.File handle.RowId
    | HCA_ExportedType handle -> toEntityHandle TableNames.ExportedType handle.RowId
    | HCA_ManifestResource handle -> toEntityHandle TableNames.ManifestResource handle.RowId
    | HCA_GenericParam handle -> toEntityHandle TableNames.GenericParam handle.RowId
    | HCA_GenericParamConstraint handle -> toEntityHandle TableNames.GenericParamConstraint handle.RowId
    | HCA_MethodSpec handle -> toEntityHandle TableNames.MethodSpec handle.RowId

let private toCustomAttributeTypeHandle (ctor: CustomAttributeType) : EntityHandle =
    match ctor with
    | CAT_MethodDef handle -> toEntityHandle TableNames.Method handle.RowId
    | CAT_MemberRef handle -> toEntityHandle TableNames.MemberRef handle.RowId

let private toMethodSemanticsAssociationHandle (association: MethodSemanticsAssociation) : EntityHandle =
    match association with
    | MethodSemanticsAssociation.PropertyAssociation(_, rowId) -> toEntityHandle TableNames.Property rowId
    | MethodSemanticsAssociation.EventAssociation(_, rowId) -> toEntityHandle TableNames.Event rowId

let private toSrmEncOperation (operation: EditAndContinueOperation) : System.Reflection.Metadata.Ecma335.EditAndContinueOperation =
    LanguagePrimitives.EnumOfValue<int, System.Reflection.Metadata.Ecma335.EditAndContinueOperation>(operation.Value)


let private toStringHandle
    (metadataBuilder: MetadataBuilder)
    (value: string)
    (_offset: StringOffset option)
    : StringHandle =
    // SRM MetadataBuilder owns heap indexing. Baseline offsets are not valid SRM handles.
    if String.IsNullOrEmpty value then StringHandle() else metadataBuilder.GetOrAddString value

let private toOptionalStringHandle
    (metadataBuilder: MetadataBuilder)
    (value: string option)
    (_offset: StringOffset option)
    : StringHandle =
    // SRM MetadataBuilder owns heap indexing. Baseline offsets are not valid SRM handles.
    match value with
    | Some stringValue when not (String.IsNullOrEmpty stringValue) -> metadataBuilder.GetOrAddString stringValue
    | _ -> StringHandle()

let private toBlobHandle
    (metadataBuilder: MetadataBuilder)
    (value: byte[])
    (_offset: BlobOffset option)
    : BlobHandle =
    // SRM MetadataBuilder owns heap indexing. Baseline offsets are not valid SRM handles.
    if isNull (box value) || value.Length = 0 then BlobHandle() else metadataBuilder.GetOrAddBlob value
/// Serialize the supplied delta row model through SRM MetadataBuilder for parity validation or fallback output mode.
let serialize
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
    (updatesByKey: Dictionary<MethodDefinitionKey, MethodBodyUpdate>)
    (encLogEntries: struct (TableName * int * EditAndContinueOperation) array)
    (encMapEntries: struct (TableName * int) array)
    : byte[] =
    let metadataBuilder = MetadataBuilder()

    let methodCount = methodDefinitionRows.Length
    let parameterCount = parameterDefinitionRows.Length
    let typeRefCount = typeReferenceRows.Length
    let memberRefCount = memberReferenceRows.Length
    let methodSpecCount = methodSpecificationRows.Length
    let assemblyRefCount = assemblyReferenceRows.Length
    let customAttributeCount = customAttributeRows.Length
    let standaloneSigCount = standaloneSignatureRows.Length
    let propertyAddCount = propertyDefinitionRows |> List.filter (fun row -> row.IsAdded) |> List.length
    let eventAddCount = eventDefinitionRows |> List.filter (fun row -> row.IsAdded) |> List.length
    let propertyMapAddCount = propertyMapRows |> List.filter (fun row -> row.IsAdded) |> List.length
    let eventMapAddCount = eventMapRows |> List.filter (fun row -> row.IsAdded) |> List.length
    let methodSemanticsAddCount = methodSemanticsRows |> List.filter (fun row -> row.IsAdded) |> List.length

    metadataBuilder.SetCapacity(TableIndex.Module, 1)
    metadataBuilder.SetCapacity(TableIndex.TypeRef, typeRefCount)
    metadataBuilder.SetCapacity(TableIndex.TypeDef, 0)
    metadataBuilder.SetCapacity(TableIndex.Field, 0)
    metadataBuilder.SetCapacity(TableIndex.MethodDef, methodCount)
    metadataBuilder.SetCapacity(TableIndex.Param, parameterCount)
    metadataBuilder.SetCapacity(TableIndex.InterfaceImpl, 0)
    metadataBuilder.SetCapacity(TableIndex.MemberRef, memberRefCount)
    metadataBuilder.SetCapacity(TableIndex.Constant, 0)
    metadataBuilder.SetCapacity(TableIndex.CustomAttribute, customAttributeCount)
    metadataBuilder.SetCapacity(TableIndex.FieldMarshal, 0)
    metadataBuilder.SetCapacity(TableIndex.DeclSecurity, 0)
    metadataBuilder.SetCapacity(TableIndex.ClassLayout, 0)
    metadataBuilder.SetCapacity(TableIndex.FieldLayout, 0)
    metadataBuilder.SetCapacity(TableIndex.StandAloneSig, standaloneSigCount)
    metadataBuilder.SetCapacity(TableIndex.EventMap, eventMapAddCount)
    metadataBuilder.SetCapacity(TableIndex.Event, eventAddCount)
    metadataBuilder.SetCapacity(TableIndex.PropertyMap, propertyMapAddCount)
    metadataBuilder.SetCapacity(TableIndex.Property, propertyAddCount)
    metadataBuilder.SetCapacity(TableIndex.MethodSemantics, methodSemanticsAddCount)
    metadataBuilder.SetCapacity(TableIndex.MethodImpl, 0)
    metadataBuilder.SetCapacity(TableIndex.ModuleRef, 0)
    metadataBuilder.SetCapacity(TableIndex.TypeSpec, 0)
    metadataBuilder.SetCapacity(TableIndex.ImplMap, 0)
    metadataBuilder.SetCapacity(TableIndex.FieldRva, 0)
    metadataBuilder.SetCapacity(TableIndex.Assembly, 0)
    metadataBuilder.SetCapacity(TableIndex.AssemblyProcessor, 0)
    metadataBuilder.SetCapacity(TableIndex.AssemblyOS, 0)
    metadataBuilder.SetCapacity(TableIndex.AssemblyRef, assemblyRefCount)
    metadataBuilder.SetCapacity(TableIndex.AssemblyRefProcessor, 0)
    metadataBuilder.SetCapacity(TableIndex.AssemblyRefOS, 0)
    metadataBuilder.SetCapacity(TableIndex.File, 0)
    metadataBuilder.SetCapacity(TableIndex.ExportedType, 0)
    metadataBuilder.SetCapacity(TableIndex.ManifestResource, 0)
    metadataBuilder.SetCapacity(TableIndex.NestedClass, 0)
    metadataBuilder.SetCapacity(TableIndex.GenericParam, 0)
    metadataBuilder.SetCapacity(TableIndex.MethodSpec, methodSpecCount)
    metadataBuilder.SetCapacity(TableIndex.GenericParamConstraint, 0)
    metadataBuilder.SetCapacity(TableIndex.EncLog, encLogEntries.Length)
    metadataBuilder.SetCapacity(TableIndex.EncMap, encMapEntries.Length)

    let moduleNameHandle = toStringHandle metadataBuilder moduleName moduleNameOffset
    let mvidHandle = metadataBuilder.GetOrAddGuid(moduleId)
    let encIdHandle = metadataBuilder.GetOrAddGuid(encId)

    let encBaseHandle =
        if encBaseId = Guid.Empty then
            GuidHandle()
        else
            metadataBuilder.GetOrAddGuid(encBaseId)

    metadataBuilder.AddModule(generation, moduleNameHandle, mvidHandle, encIdHandle, encBaseHandle)
    |> ignore

    for row in methodDefinitionRows do
        match updatesByKey.TryGetValue row.Key with
        | true, methodBody ->
            let nameHandle = toStringHandle metadataBuilder row.Name row.NameOffset
            let signatureHandle = toBlobHandle metadataBuilder row.Signature row.SignatureOffset

            let firstParameterHandle =
                match row.FirstParameterRowId with
                | Some rowId when rowId > 0 -> MetadataTokens.ParameterHandle rowId
                | _ -> ParameterHandle()

            let codeRva =
                if methodBody.CodeLength > 0 then
                    methodBody.CodeOffset
                else
                    defaultArg row.CodeRva 0

            metadataBuilder.AddMethodDefinition(
                row.Attributes,
                row.ImplAttributes,
                nameHandle,
                signatureHandle,
                codeRva,
                firstParameterHandle)
            |> ignore
        | _ ->
            invalidArg "methodDefinitionRows" (sprintf "Missing update payload for method key %A" row.Key)

    for row in parameterDefinitionRows do
        let nameHandle = toOptionalStringHandle metadataBuilder row.Name row.NameOffset
        metadataBuilder.AddParameter(row.Attributes, nameHandle, row.SequenceNumber) |> ignore

    for row in typeReferenceRows do
        let scopeHandle = toResolutionScopeHandle row.ResolutionScope
        let namespaceHandle = toStringHandle metadataBuilder row.Namespace row.NamespaceOffset
        let nameHandle = toStringHandle metadataBuilder row.Name row.NameOffset
        metadataBuilder.AddTypeReference(scopeHandle, namespaceHandle, nameHandle) |> ignore

    for row in memberReferenceRows do
        let parentHandle = toMemberRefParentHandle row.Parent
        let nameHandle = toStringHandle metadataBuilder row.Name row.NameOffset
        let signatureHandle = toBlobHandle metadataBuilder row.Signature row.SignatureOffset
        metadataBuilder.AddMemberReference(parentHandle, nameHandle, signatureHandle) |> ignore

    for row in methodSpecificationRows do
        let methodHandle = toMethodDefOrRefHandle row.Method
        let signatureHandle = toBlobHandle metadataBuilder row.Signature row.SignatureOffset
        metadataBuilder.AddMethodSpecification(methodHandle, signatureHandle) |> ignore

    for row in assemblyReferenceRows do
        let nameHandle = toStringHandle metadataBuilder row.Name row.NameOffset
        let cultureHandle = toOptionalStringHandle metadataBuilder row.Culture row.CultureOffset
        let publicKeyHandle = toBlobHandle metadataBuilder row.PublicKeyOrToken row.PublicKeyOrTokenOffset
        let hashHandle = toBlobHandle metadataBuilder row.HashValue row.HashValueOffset
        metadataBuilder.AddAssemblyReference(nameHandle, row.Version, cultureHandle, publicKeyHandle, row.Flags, hashHandle)
        |> ignore

    for signature in standaloneSignatureRows do
        if not (isNull (box signature.Blob)) && signature.Blob.Length > 0 then
            let signatureHandle = metadataBuilder.GetOrAddBlob signature.Blob
            metadataBuilder.AddStandaloneSignature(signatureHandle) |> ignore

    for row in customAttributeRows do
        let parentHandle = toHasCustomAttributeHandle row.Parent
        let constructorHandle = toCustomAttributeTypeHandle row.Constructor
        let valueHandle = toBlobHandle metadataBuilder row.Value row.ValueOffset
        metadataBuilder.AddCustomAttribute(parentHandle, constructorHandle, valueHandle) |> ignore

    for row in propertyDefinitionRows do
        if row.IsAdded then
            let nameHandle = toStringHandle metadataBuilder row.Name row.NameOffset
            let signatureHandle = toBlobHandle metadataBuilder row.Signature row.SignatureOffset
            metadataBuilder.AddProperty(row.Attributes, nameHandle, signatureHandle) |> ignore

    for row in eventDefinitionRows do
        if row.IsAdded then
            let nameHandle = toStringHandle metadataBuilder row.Name row.NameOffset
            let eventTypeHandle = toTypeDefOrRefHandle row.EventType
            metadataBuilder.AddEvent(row.Attributes, nameHandle, eventTypeHandle) |> ignore

    for row in propertyMapRows do
        if row.IsAdded then
            let parentHandle = MetadataTokens.TypeDefinitionHandle row.TypeDefRowId

            let propertyListHandle =
                match row.FirstPropertyRowId with
                | Some rowId -> MetadataTokens.PropertyDefinitionHandle rowId
                | None -> invalidArg "propertyMapRows" (sprintf "PropertyMap row %d missing FirstPropertyRowId" row.RowId)

            metadataBuilder.AddPropertyMap(parentHandle, propertyListHandle) |> ignore

    for row in eventMapRows do
        if row.IsAdded then
            let parentHandle = MetadataTokens.TypeDefinitionHandle row.TypeDefRowId

            let eventListHandle =
                match row.FirstEventRowId with
                | Some rowId -> MetadataTokens.EventDefinitionHandle rowId
                | None -> invalidArg "eventMapRows" (sprintf "EventMap row %d missing FirstEventRowId" row.RowId)

            metadataBuilder.AddEventMap(parentHandle, eventListHandle) |> ignore

    for row in methodSemanticsRows do
        if row.IsAdded then
            let methodHandle = MetadataTokens.MethodDefinitionHandle(DeltaTokens.getRowNumber row.MethodToken)
            let associationHandle = toMethodSemanticsAssociationHandle row.AssociationInfo
            metadataBuilder.AddMethodSemantics(associationHandle, row.Attributes, methodHandle) |> ignore

    userStringUpdates
    |> List.sortBy (fun (_, newToken, _) -> newToken &&& 0x00FFFFFF)
    |> List.iter (fun (_, _, literal) -> metadataBuilder.GetOrAddUserString literal |> ignore)

    for struct (table, rowId, operation) in encLogEntries do
        let handle = toEntityHandle table rowId
        metadataBuilder.AddEncLogEntry(handle, toSrmEncOperation operation) |> ignore

    for struct (table, rowId) in encMapEntries do
        let handle = toEntityHandle table rowId
        metadataBuilder.AddEncMapEntry(handle) |> ignore

    let metadataRoot = MetadataRootBuilder(metadataBuilder)
    let blob = BlobBuilder()
    metadataRoot.Serialize(blob, methodBodyStreamRva = 0, mappedFieldDataStreamRva = 0)
    blob.ToArray()

let private trackedParityTables =
    [| TableIndex.Module
       TableIndex.TypeRef
       TableIndex.MethodDef
       TableIndex.Param
       TableIndex.MemberRef
       TableIndex.MethodSpec
       TableIndex.StandAloneSig
       TableIndex.CustomAttribute
       TableIndex.Property
       TableIndex.Event
       TableIndex.PropertyMap
       TableIndex.EventMap
       TableIndex.MethodSemantics
       TableIndex.AssemblyRef
       TableIndex.EncLog
       TableIndex.EncMap |]

let private withMetadataReader (metadata: byte[]) (action: MetadataReader -> 'T) : 'T =
    use provider = MetadataReaderProvider.FromMetadataImage(System.Collections.Immutable.ImmutableArray.CreateRange metadata)
    let reader = provider.GetMetadataReader()
    action reader

/// Compare metadata blobs using stable EnC structure only.
/// Heap byte layout is intentionally excluded because SRM and AbstractIL can serialize equivalent heaps differently.
let compareMetadataStructure (left: byte[]) (right: byte[]) : string option =
    try
        withMetadataReader left (fun leftReader ->
            withMetadataReader right (fun rightReader ->
                let mutable mismatch = None

                for table in trackedParityTables do
                    if mismatch.IsNone then
                        let leftCount = leftReader.GetTableRowCount table
                        let rightCount = rightReader.GetTableRowCount table

                        if leftCount <> rightCount then
                            mismatch <- Some(sprintf "table %A row-count mismatch (left=%d right=%d)" table leftCount rightCount)

                if mismatch.IsNone then
                    let leftEncLog =
                        leftReader.GetEditAndContinueLogEntries()
                        |> Seq.map (fun entry -> struct (MetadataTokens.GetToken(entry.Handle), int entry.Operation))
                        |> Seq.toArray

                    let rightEncLog =
                        rightReader.GetEditAndContinueLogEntries()
                        |> Seq.map (fun entry -> struct (MetadataTokens.GetToken(entry.Handle), int entry.Operation))
                        |> Seq.toArray

                    if leftEncLog <> rightEncLog then
                        mismatch <-
                            Some(
                                sprintf
                                    "EncLog mismatch (left-count=%d right-count=%d)"
                                    leftEncLog.Length
                                    rightEncLog.Length)

                if mismatch.IsNone then
                    let leftEncMap =
                        leftReader.GetEditAndContinueMapEntries()
                        |> Seq.map MetadataTokens.GetToken
                        |> Seq.toArray

                    let rightEncMap =
                        rightReader.GetEditAndContinueMapEntries()
                        |> Seq.map MetadataTokens.GetToken
                        |> Seq.toArray

                    if leftEncMap <> rightEncMap then
                        mismatch <-
                            Some(
                                sprintf
                                    "EncMap mismatch (left-count=%d right-count=%d)"
                                    leftEncMap.Length
                                    rightEncMap.Length)

                mismatch))
    with ex ->
        Some(sprintf "metadata reader parity inspection failed: %s" ex.Message)
