module internal FSharp.Compiler.HotReloadBaseline

open System
open System.Collections.Generic
open System.Collections.Immutable

open FSharp.Compiler.AbstractIL.EncMethodDebugInformation
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.CodeGen
open FSharp.Compiler.CompilerGeneratedNameMapState
open FSharp.Compiler.GeneratedNames
open FSharp.Compiler.Syntax.PrettyNaming

[<RequireQualifiedAccess>]
type SynthesizedNameSnapshotSource =
    | Recorded
    | Reconstructed

type PortablePdbSnapshot =
    {
        Bytes: byte[]
        TableRowCounts: ImmutableArray<int>
        EntryPointToken: int option
    }

type TypeDefinitionKey =
    {
        RowId: int
        Namespace: string
        Name: string
    }

type MethodDefinitionKey =
    {
        DeclaringType: TypeDefinitionKey
        Name: string
        Signature: byte list
    }

type FieldDefinitionKey =
    {
        DeclaringType: TypeDefinitionKey
        Name: string
        Signature: byte list
    }

type PropertyDefinitionKey =
    {
        DeclaringType: TypeDefinitionKey
        Name: string
        Signature: byte list
    }

type EventDefinitionKey =
    {
        DeclaringType: TypeDefinitionKey
        Name: string
        EventType: int
    }

type BaselineTokenMaps =
    {
        TypeTokens: Map<TypeDefinitionKey, int>
        MethodTokens: Map<MethodDefinitionKey, int>
        FieldTokens: Map<FieldDefinitionKey, int>
        PropertyTokens: Map<PropertyDefinitionKey, int>
        EventTokens: Map<EventDefinitionKey, int>
    }

type FSharpEmitBaseline =
    {
        ModuleId: Guid
        Metadata: ILBaselineReader.MetadataSnapshot
        PortablePdb: PortablePdbSnapshot option
        TokenMaps: BaselineTokenMaps
        SynthesizedNameSnapshot: Map<string, string[]>
        SynthesizedNameSnapshotSource: SynthesizedNameSnapshotSource
        EncMethodDebugInfos: Map<int, EncMethodDebugInformation>
        EncClosureNames: Map<int, Map<int list, string>>
    }

let private typeDefToken rowId = (0x02 <<< 24) ||| rowId
let private fieldToken rowId = (0x04 <<< 24) ||| rowId
let private methodDefToken rowId = (0x06 <<< 24) ||| rowId
let private eventToken rowId = (0x14 <<< 24) ||| rowId
let private propertyToken rowId = (0x17 <<< 24) ||| rowId

let private typeFullName (key: TypeDefinitionKey) =
    if String.IsNullOrEmpty key.Namespace then
        key.Name
    else
        key.Namespace + "." + key.Name

let private signatureList (bytes: byte[]) = bytes |> Array.toList

let private buildTypeKeys (reader: ILBaselineReader.BaselineMetadataReader) =
    [
        for rowId in 1 .. reader.TypeDefCount do
            match reader.GetTypeDef rowId with
            | Some row ->
                yield
                    rowId,
                    {
                        RowId = rowId
                        Namespace = reader.GetString row.NamespaceOffset
                        Name = reader.GetString row.NameOffset
                    }
            | None -> ()
    ]
    |> Map.ofList

let private emptyTokenMaps =
    {
        TypeTokens = Map.empty
        MethodTokens = Map.empty
        FieldTokens = Map.empty
        PropertyTokens = Map.empty
        EventTokens = Map.empty
    }

let private buildTokenMaps (reader: ILBaselineReader.BaselineMetadataReader) =
    let typeKeys = buildTypeKeys reader

    let typeTokens: Map<TypeDefinitionKey, int> =
        typeKeys
        |> Map.toSeq
        |> Seq.map (fun (rowId, key) -> key, typeDefToken rowId)
        |> Map.ofSeq

    let methodTokens: Map<MethodDefinitionKey, int> =
        seq {
            for KeyValue(typeRowId, typeKey) in typeKeys do
                match reader.GetTypeMethodRange typeRowId with
                | None -> ()
                | Some(firstMethod, lastMethod) ->
                    for methodRowId in firstMethod..lastMethod do
                        match reader.GetMethodDef methodRowId with
                        | None -> ()
                        | Some methodDef ->
                            let key: MethodDefinitionKey =
                                {
                                    DeclaringType = typeKey
                                    Name = reader.GetString methodDef.NameOffset
                                    Signature = reader.GetBlob methodDef.SignatureOffset |> signatureList
                                }

                            yield key, methodDefToken methodRowId
        }
        |> Map.ofSeq

    let fieldTokens: Map<FieldDefinitionKey, int> =
        seq {
            for KeyValue(typeRowId, typeKey) in typeKeys do
                match reader.GetTypeFieldRange typeRowId with
                | None -> ()
                | Some(firstField, lastField) ->
                    for fieldRowId in firstField..lastField do
                        match reader.GetField fieldRowId with
                        | None -> ()
                        | Some fieldDef ->
                            let key: FieldDefinitionKey =
                                {
                                    DeclaringType = typeKey
                                    Name = reader.GetString fieldDef.NameOffset
                                    Signature = reader.GetBlob fieldDef.SignatureOffset |> signatureList
                                }

                            yield key, fieldToken fieldRowId
        }
        |> Map.ofSeq

    let propertyTokens: Map<PropertyDefinitionKey, int> =
        seq {
            for propertyMapRowId in 1 .. reader.PropertyMapCount do
                match reader.GetPropertyMapRange propertyMapRowId with
                | Some(parentTypeRowId, firstProperty, lastProperty) ->
                    match Map.tryFind parentTypeRowId typeKeys with
                    | None -> ()
                    | Some typeKey ->
                        for propertyRowId in firstProperty..lastProperty do
                            match reader.GetProperty propertyRowId with
                            | None -> ()
                            | Some propertyDef ->
                                let key: PropertyDefinitionKey =
                                    {
                                        DeclaringType = typeKey
                                        Name = reader.GetString propertyDef.NameOffset
                                        Signature = reader.GetBlob propertyDef.SignatureOffset |> signatureList
                                    }

                                yield key, propertyToken propertyRowId
                | None -> ()
        }
        |> Map.ofSeq

    let eventTokens: Map<EventDefinitionKey, int> =
        seq {
            for eventMapRowId in 1 .. reader.EventMapCount do
                match reader.GetEventMapRange eventMapRowId with
                | Some(parentTypeRowId, firstEvent, lastEvent) ->
                    match Map.tryFind parentTypeRowId typeKeys with
                    | None -> ()
                    | Some typeKey ->
                        for eventRowId in firstEvent..lastEvent do
                            match reader.GetEvent eventRowId with
                            | None -> ()
                            | Some eventDef ->
                                let key: EventDefinitionKey =
                                    {
                                        DeclaringType = typeKey
                                        Name = reader.GetString eventDef.NameOffset
                                        EventType = eventDef.EventType
                                    }

                                yield key, eventToken eventRowId
                | None -> ()
        }
        |> Map.ofSeq

    {
        TypeTokens = typeTokens
        MethodTokens = methodTokens
        FieldTokens = fieldTokens
        PropertyTokens = propertyTokens
        EventTokens = eventTokens
    }

let private addSynthesizedName (buckets: Dictionary<string, ResizeArray<string>>) (name: string) =
    if not (String.IsNullOrWhiteSpace name) && IsCompilerGeneratedName name then
        let basicName = GetBasicNameOfPossibleCompilerGeneratedName name
        let mapKey = SynthesizedNameMapKey basicName

        if not (String.IsNullOrWhiteSpace mapKey) then
            let bucket =
                match buckets.TryGetValue mapKey with
                | true, existing -> existing
                | _ ->
                    let created = ResizeArray<string>()
                    buckets[mapKey] <- created
                    created

            if not (bucket.Contains name) then
                bucket.Add name

let private snapshotFromBuckets (buckets: Dictionary<string, ResizeArray<string>>) =
    buckets
    |> Seq.map (fun (KeyValue(key, bucket)) -> key, bucket.ToArray())
    |> Map.ofSeq

let internal collectSynthesizedNameSnapshot (ilModule: ILModuleDef) =
    let buckets = Dictionary<string, ResizeArray<string>>(StringComparer.Ordinal)

    let rec collectTypeDef (typeDef: ILTypeDef) =
        addSynthesizedName buckets typeDef.Name

        typeDef.Fields.AsList()
        |> List.iter (fun fieldDef -> addSynthesizedName buckets fieldDef.Name)

        typeDef.Methods.AsList()
        |> List.iter (fun methodDef -> addSynthesizedName buckets methodDef.Name)

        typeDef.Properties.AsList()
        |> List.iter (fun propertyDef -> addSynthesizedName buckets propertyDef.Name)

        typeDef.Events.AsList()
        |> List.iter (fun eventDef -> addSynthesizedName buckets eventDef.Name)

        typeDef.NestedTypes.AsList() |> List.iter collectTypeDef

    ilModule.TypeDefs.AsList() |> List.iter collectTypeDef
    snapshotFromBuckets buckets

let internal collectRecordedSynthesizedNameSnapshot (_compilerGlobalState: obj) (map: ICompilerGeneratedNameMap) = map.Snapshot

let private collectSynthesizedNameSnapshotFromTokens (tokenMaps: BaselineTokenMaps) =
    let buckets = Dictionary<string, ResizeArray<string>>(StringComparer.Ordinal)

    for KeyValue(typeKey, _) in tokenMaps.TypeTokens do
        addSynthesizedName buckets typeKey.Name

    for KeyValue(methodKey, _) in tokenMaps.MethodTokens do
        addSynthesizedName buckets methodKey.Name

    for KeyValue(fieldKey, _) in tokenMaps.FieldTokens do
        addSynthesizedName buckets fieldKey.Name

    for KeyValue(propertyKey, _) in tokenMaps.PropertyTokens do
        addSynthesizedName buckets propertyKey.Name

    for KeyValue(eventKey, _) in tokenMaps.EventTokens do
        addSynthesizedName buckets eventKey.Name

    snapshotFromBuckets buckets

let private formatOccurrenceChainKey (ordinalChain: int list) =
    ordinalChain |> List.map string |> String.concat "_"

let private formatGenerationSuffixedClosureName baseName generation ordinalChain =
    CompilerGeneratedNameSuffix baseName $"hotreload#g{generation}_o{formatOccurrenceChainKey ordinalChain}"

let private cleanUpGeneratedTypeName (name: string) =
    if name.IndexOfAny IllegalCharactersInTypeAndNamespaceNames = -1 then
        name
    else
        (name, IllegalCharactersInTypeAndNamespaceNames)
        ||> Array.fold (fun acc c -> acc.Replace(string c, "-"))

let private typeDefSimpleNames (tokenMaps: BaselineTokenMaps) =
    tokenMaps.TypeTokens
    |> Map.toSeq
    |> Seq.map (fun (key, _) -> key.Name)
    |> Set.ofSeq

let private methodNamesByToken (methodTokens: Map<MethodDefinitionKey, int>) =
    methodTokens
    |> Map.toSeq
    |> Seq.map (fun (key, token) -> token, key.Name)
    |> Map.ofSeq

let deriveEncClosureNamesFromEncDebugInfos
    (encMethodDebugInfos: Map<int, EncMethodDebugInformation>)
    (methodNamesByToken: Map<int, string>)
    (typeDefSimpleNames: Set<string>)
    : Map<int, Map<int list, string>> =

    if Map.isEmpty encMethodDebugInfos then
        Map.empty
    else
        let hasMidSessionClosureNames =
            typeDefSimpleNames
            |> Set.exists (fun name ->
                match TryGetHotReloadNameGeneration name with
                | Some generation -> generation >= 1
                | None -> false)

        if hasMidSessionClosureNames then
            Map.empty
        else
            let hasReplayNamedTypeDef nameBase =
                let prefix = nameBase + "@hotreload"

                typeDefSimpleNames
                |> Set.exists (fun name ->
                    name.StartsWith(prefix, StringComparison.Ordinal)
                    && not (IsHotReloadGenerationSuffixedName name))

            let derivedRows =
                encMethodDebugInfos
                |> Map.toList
                |> List.choose (fun (methodToken, info) ->
                    match info.Closures, Map.tryFind methodToken methodNamesByToken with
                    | [], _
                    | _, None -> None
                    | closures, Some methodName ->
                        let nameBase = cleanUpGeneratedTypeName methodName

                        let rows =
                            closures
                            |> List.choose (fun closure ->
                                let chain = decodeOccurrenceKey closure.SyntaxOffset
                                let name = formatGenerationSuffixedClosureName nameBase 0 chain

                                if Set.contains name typeDefSimpleNames then
                                    Some(chain, name)
                                else
                                    None)

                        Some(methodToken, nameBase, rows))

            let hasReplayOnlyCdiMethod =
                derivedRows
                |> List.exists (fun (_, nameBase, rows) -> List.isEmpty rows && hasReplayNamedTypeDef nameBase)

            if hasReplayOnlyCdiMethod then
                Map.empty
            else
                derivedRows
                |> List.choose (fun (methodToken, _, rows) ->
                    match rows with
                    | [] -> None
                    | _ -> Some(methodToken, Map.ofList rows))
                |> Map.ofList

let private toPortablePdbSnapshot (expectedContentId: byte[]) (pdbBytes: byte[]) =
    ILBaselineReader.readPortablePdbMetadata pdbBytes
    |> Option.filter (fun metadata -> metadata.ContentId.AsSpan().SequenceEqual(expectedContentId))
    |> Option.map (fun metadata ->
        {
            Bytes = Array.copy pdbBytes
            TableRowCounts = ImmutableArray.CreateRange metadata.TableRowCounts
            EntryPointToken = metadata.EntryPointToken
        })

let private createCore moduleId metadata portablePdb tokenMaps =
    let reconstructedSynthesizedNames =
        collectSynthesizedNameSnapshotFromTokens tokenMaps

    let synthesizedNames, synthesizedNameSnapshotSource =
        match
            portablePdb
            |> Option.bind (fun snapshot -> readSynthesizedNameSnapshotFromPortablePdb snapshot.Bytes)
        with
        | Some recordedSnapshot -> recordedSnapshot, SynthesizedNameSnapshotSource.Recorded
        | None -> reconstructedSynthesizedNames, SynthesizedNameSnapshotSource.Reconstructed

    let encMethodDebugInfos =
        portablePdb
        |> Option.map (fun snapshot -> readEncMethodDebugInfoFromPortablePdb snapshot.Bytes)
        |> Option.defaultValue Map.empty

    {
        ModuleId = moduleId
        Metadata = metadata
        PortablePdb = portablePdb
        TokenMaps = tokenMaps
        SynthesizedNameSnapshot = synthesizedNames
        SynthesizedNameSnapshotSource = synthesizedNameSnapshotSource
        EncMethodDebugInfos = encMethodDebugInfos
        EncClosureNames =
            deriveEncClosureNamesFromEncDebugInfos
                encMethodDebugInfos
                (methodNamesByToken tokenMaps.MethodTokens)
                (typeDefSimpleNames tokenMaps)
    }

let tryReadFromAssemblyAndPdbBytes (assemblyBytes: byte[]) (portablePdbBytes: byte[] option) =
    match
        ILBaselineReader.metadataSnapshotFromBytes assemblyBytes,
        ILBaselineReader.BaselineMetadataReader.Create assemblyBytes,
        ILBaselineReader.readModuleMvidFromBytes assemblyBytes
    with
    | Some metadata, Some reader, Some moduleId when moduleId <> Guid.Empty ->
        let portablePdb =
            match ILBaselineReader.readCodeViewContentIdFromBytes assemblyBytes with
            | Some expectedContentId -> portablePdbBytes |> Option.bind (toPortablePdbSnapshot expectedContentId)
            | None -> None

        Some(createCore moduleId metadata portablePdb (buildTokenMaps reader))
    | _ -> None

let readFromAssemblyAndPdbBytes (assemblyBytes: byte[]) (portablePdbBytes: byte[] option) =
    match tryReadFromAssemblyAndPdbBytes assemblyBytes portablePdbBytes with
    | Some baseline -> baseline
    | None -> invalidArg (nameof assemblyBytes) "assembly bytes do not contain readable CLI metadata"

let metadataSnapshotFromBytes = ILBaselineReader.metadataSnapshotFromBytes

let readModuleMvid = ILBaselineReader.readModuleMvidFromBytes
