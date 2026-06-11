module internal FSharp.Compiler.HotReloadBaseline

open System
open System.Collections.Generic
open System.Collections.Immutable
open System.Reflection
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILBinaryWriter
open FSharp.Compiler.AbstractIL.BinaryConstants
open FSharp.Compiler.AbstractIL.ILDeltaHandles
open FSharp.Compiler.EncMethodDebugInformation
open FSharp.Compiler.IlxGen
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree

module ILBaselineReader = FSharp.Compiler.AbstractIL.ILBaselineReader
open FSharp.Compiler.Syntax.PrettyNaming
open FSharp.Compiler.EnvironmentHelpers

let private tableCount = DeltaTokens.TableCount

[<Literal>]
let private TraceHeapOffsetsFlagName = "FSHARP_HOTRELOAD_TRACE_HEAP_OFFSETS"

let private traceHeapOffsets = lazy (isEnvVarTruthy TraceHeapOffsetsFlagName)

/// Align a size to a 4-byte boundary (stream alignment per ECMA-335).
/// Used for Blob and UserString heap cumulative tracking, per Roslyn behavior.
let private align4 value = (value + 3) &&& ~~~3

/// <summary>Metadata describing a method body that was added or changed in a delta.</summary>
type AddedOrChangedMethodInfo =
    {
        MethodToken: int
        LocalSignatureToken: int
        CodeOffset: int
        CodeLength: int
    }

/// <summary>Stable identifier for a method definition used when correlating baseline tokens.</summary>
type MethodDefinitionKey =
    {
        DeclaringType: string
        Name: string
        GenericArity: int
        ParameterTypes: ILType list
        ReturnType: ILType
    }

/// Baseline metadata handles reused to keep heap offsets stable across deltas.

/// <summary>Stable identifier for a method parameter (sequence number within a method).</summary>
type ParameterDefinitionKey =
    {
        Method: MethodDefinitionKey
        SequenceNumber: int
    }

/// <summary>Stable identifier for a field definition in the baseline assembly.</summary>
type FieldDefinitionKey =
    {
        DeclaringType: string
        Name: string
        FieldType: ILType
    }

/// <summary>Stable identifier for a property definition (including indexer parameter shapes).</summary>
type PropertyDefinitionKey =
    {
        DeclaringType: string
        Name: string
        PropertyType: ILType
        IndexParameterTypes: ILType list
    }

/// <summary>Stable identifier for an event definition in the baseline assembly.</summary>
type EventDefinitionKey =
    {
        DeclaringType: string
        Name: string
        EventType: ILType option
    }

type MethodDefinitionMetadataHandles =
    { NameOffset: StringOffset option
      SignatureOffset: BlobOffset option
      FirstParameterRowId: int option
      Rva: int option
      Attributes: MethodAttributes option
      ImplAttributes: MethodImplAttributes option }

/// <summary>
/// Typed identity for a TypeRef resolution scope. Baseline TypeRef tables routinely contain
/// duplicate type names under different scopes (e.g. two 'Object' rows under different
/// AssemblyRefs, 'LowPriority' under two namespaces) and nested TypeRefs whose scope is the
/// enclosing TypeRef row, so a TypeRef can only be matched by its full scope chain - never by
/// name alone.
/// </summary>
[<RequireQualifiedAccess>]
type TypeReferenceScope =
    /// TypeRef resolved against an AssemblyRef row, identified by the referenced assembly's simple name.
    | Assembly of assemblyName: string
    /// Nested TypeRef whose resolution scope is its enclosing TypeRef.
    | Nested of enclosing: TypeReferenceKey

and TypeReferenceKey =
    { Scope: TypeReferenceScope
      Namespace: string
      Name: string }

type ParameterDefinitionMetadataHandles =
    { NameOffset: StringOffset option
      RowId: int option }

type PropertyDefinitionMetadataHandles =
    { NameOffset: StringOffset option
      SignatureOffset: BlobOffset option }

type EventDefinitionMetadataHandles = { NameOffset: StringOffset option }

type BaselineHandleCache =
    { MethodHandles: Map<MethodDefinitionKey, MethodDefinitionMetadataHandles>
      ParameterHandles: Map<ParameterDefinitionKey, ParameterDefinitionMetadataHandles>
      PropertyHandles: Map<PropertyDefinitionKey, PropertyDefinitionMetadataHandles>
      EventHandles: Map<EventDefinitionKey, EventDefinitionMetadataHandles> }

    static member Empty =
        { MethodHandles = Map.empty
          ParameterHandles = Map.empty
          PropertyHandles = Map.empty
          EventHandles = Map.empty }

type MethodSemanticsAssociation =
    | PropertyAssociation of PropertyDefinitionKey * rowId:int
    | EventAssociation of EventDefinitionKey * rowId:int

type MethodSemanticsEntry =
    {
        RowId: int
        Attributes: MethodSemanticsAttributes
        Association: MethodSemanticsAssociation
    }

/// <summary>Portable PDB snapshot captured during baseline emission.</summary>
type PortablePdbSnapshot =
    {
        Bytes: byte[]
        TableRowCounts: ImmutableArray<int>
        EntryPointToken: int option
    }

/// <summary>
/// Represents the captured state of a baseline emission, mirroring Roslyn's EmitBaseline. It stores metadata
/// snapshots along with stable token maps so delta emission can reuse pre-existing metadata handles.
/// </summary>
type FSharpEmitBaseline =
    {
        ModuleId: Guid
        EncId: Guid
        EncBaseId: Guid
        NextGeneration: int
        ModuleNameOffset: StringOffset option
        Metadata: MetadataSnapshot
        TokenMappings: ILTokenMappings
        TypeTokens: Map<string, int>
        MethodTokens: Map<MethodDefinitionKey, int>
        FieldTokens: Map<FieldDefinitionKey, int>
        PropertyTokens: Map<PropertyDefinitionKey, int>
        EventTokens: Map<EventDefinitionKey, int>
        PropertyMapEntries: Map<string, int>
        EventMapEntries: Map<string, int>
        MethodSemanticsEntries: Map<MethodDefinitionKey, MethodSemanticsEntry list>
        IlxGenEnvironment: IlxGenEnvSnapshot option
        PortablePdb: PortablePdbSnapshot option
        SynthesizedNameSnapshot: Map<string, string[]>
        MetadataHandles: BaselineHandleCache
        TypeReferenceTokens: Map<TypeReferenceKey, int>
        AssemblyReferenceTokens: Map<string, int>
        TableEntriesAdded: int[]
        StringStreamLengthAdded: int
        UserStringStreamLengthAdded: int
        BlobStreamLengthAdded: int
        GuidStreamLengthAdded: int
        AddedOrChangedMethods: AddedOrChangedMethodInfo list
        /// <summary>
        /// Per-method Edit-and-Continue debug information (lambda/closure occurrence maps),
        /// keyed by MethodDef token (0x06xxxxxx). Decoded from the baseline portable PDB's EnC
        /// CustomDebugInformation rows when the baseline is captured, and refreshed in memory
        /// for updated/added methods as each delta is applied (see chainEncMethodDebugInfos).
        /// A baseline compiled without --enable:hotreloaddeltas (or a pre-C2 PDB) yields the
        /// empty map. NOTE: delta PDBs do not yet re-emit EnC CDI rows, so within a session
        /// this in-memory chain is the only generation-accurate source; persistence across
        /// session restarts is the remaining gap.
        /// </summary>
        EncMethodDebugInfos: Map<int, EncMethodDebugInformation>
        /// <summary>
        /// Per-method closure-class name tables (occurrence-chain -> emitted closure type
        /// name), keyed by MethodDef token (0x06xxxxxx) — the C3 companion of
        /// EncMethodDebugInfos. The Roslyn CDI blob formats carry no name slots (C# names
        /// are recomputed from DebugId alone, which F# cannot do for baseline names that
        /// embed line numbers or replay ordinals), so the trustworthy chain -> name table
        /// is captured during baseline IlxGen (stamp -> name recording at the closure call
        /// site, joined with the same tree's occurrence extraction) and chained in memory
        /// like EncMethodDebugInfos. Empty for baselines created without an in-process
        /// flag-on emit (e.g. read back from disk): the occurrence-keyed naming then stays
        /// inert and delta compiles keep sequence replay (fail closed).
        /// </summary>
        EncClosureNames: Map<int, Map<int list, string>>
    }

type private BaselineMaps =
    {
        TypeTokens: Map<string, int>
        MethodTokens: Map<MethodDefinitionKey, int>
        FieldTokens: Map<FieldDefinitionKey, int>
        PropertyTokens: Map<PropertyDefinitionKey, int>
        EventTokens: Map<EventDefinitionKey, int>
        PropertyMapEntries: Map<string, int>
        EventMapEntries: Map<string, int>
    }

let private emptyMaps =
    {
        TypeTokens = Map.empty
        MethodTokens = Map.empty
        FieldTokens = Map.empty
        PropertyTokens = Map.empty
        EventTokens = Map.empty
        PropertyMapEntries = Map.empty
        EventMapEntries = Map.empty
    }

let private collectSynthesizedNameSnapshot (ilModule: ILModuleDef) =
    let buckets = Dictionary<string, ResizeArray<string>>(StringComparer.Ordinal)

    let recordName (name: string) =
        if not (String.IsNullOrWhiteSpace name) && IsCompilerGeneratedName name then
            let basicName = GetBasicNameOfPossibleCompilerGeneratedName name
            if not (String.IsNullOrWhiteSpace basicName) then
                let bucket =
                    match buckets.TryGetValue basicName with
                    | true, existing -> existing
                    | _ ->
                        let created = ResizeArray<string>()
                        buckets[basicName] <- created
                        created

                if not (bucket.Contains name) then
                    bucket.Add(name)

    let rec collectTypeDef (typeDef: ILTypeDef) =
        recordName typeDef.Name

        typeDef.Fields.AsList()
        |> List.iter (fun fieldDef -> recordName fieldDef.Name)

        typeDef.Methods.AsList()
        |> List.iter (fun methodDef -> recordName methodDef.Name)

        typeDef.Properties.AsList()
        |> List.iter (fun propertyDef -> recordName propertyDef.Name)

        typeDef.Events.AsList()
        |> List.iter (fun eventDef -> recordName eventDef.Name)

        typeDef.NestedTypes.AsList()
        |> List.iter collectTypeDef

    ilModule.TypeDefs.AsList()
    |> List.iter collectTypeDef

    buckets
    |> Seq.map (fun (KeyValue(key, bucket)) -> key, bucket.ToArray())
    |> Map.ofSeq

/// <summary>
/// Populate the baseline token maps by walking type definitions and their nested members.
/// </summary>
let rec private collectType
    (tokenMappings: ILTokenMappings)
    (scope: ILScopeRef)
    (enclosing: ILTypeDef list)
    (maps: BaselineMaps)
    (tdef: ILTypeDef)
    : BaselineMaps =
    let typeRef = mkRefForNestedILTypeDef scope (enclosing, tdef)
    let typeName = typeRef.FullName
    let typeToken = tokenMappings.TypeDefTokenMap(enclosing, tdef)

    let maps =
        { maps with
            TypeTokens = maps.TypeTokens |> Map.add typeName typeToken
        }

    let maps =
        tdef.Methods.AsList()
        |> List.fold
            (fun (acc: BaselineMaps) mdef ->
                let key =
                    {
                        DeclaringType = typeName
                        Name = mdef.Name
                        GenericArity = mdef.GenericParams.Length
                        ParameterTypes = mdef.ParameterTypes
                        ReturnType = mdef.Return.Type
                    }

                let token = tokenMappings.MethodDefTokenMap (enclosing, tdef) mdef

                { acc with
                    MethodTokens = acc.MethodTokens |> Map.add key token
                })
            maps

    let maps =
        tdef.Fields.AsList()
        |> List.fold
            (fun (acc: BaselineMaps) fdef ->
                let key =
                    {
                        DeclaringType = typeName
                        Name = fdef.Name
                        FieldType = fdef.FieldType
                    }

                let token = tokenMappings.FieldDefTokenMap (enclosing, tdef) fdef

                { acc with
                    FieldTokens = acc.FieldTokens |> Map.add key token
                })
            maps

    let propertyDefs = tdef.Properties.AsList()

    let maps =
        propertyDefs
        |> List.fold
            (fun (acc: BaselineMaps) pdef ->
                let key =
                    {
                        DeclaringType = typeName
                        Name = pdef.Name
                        PropertyType = pdef.PropertyType
                        IndexParameterTypes = List.ofSeq pdef.Args
                    }

                let token = tokenMappings.PropertyTokenMap (enclosing, tdef) pdef

                { acc with
                    PropertyTokens = acc.PropertyTokens |> Map.add key token
                })
            maps

    let maps =
        match propertyDefs with
        | first :: _ ->
            let token = tokenMappings.PropertyTokenMap (enclosing, tdef) first
            let rowId = token &&& 0x00FFFFFF
            { maps with PropertyMapEntries = maps.PropertyMapEntries |> Map.add typeName rowId }
        | [] -> maps

    let eventDefs = tdef.Events.AsList()

    let maps =
        eventDefs
        |> List.fold
            (fun (acc: BaselineMaps) edef ->
                let key =
                    {
                        DeclaringType = typeName
                        Name = edef.Name
                        EventType = edef.EventType
                    }

                let token = tokenMappings.EventTokenMap (enclosing, tdef) edef

                { acc with
                    EventTokens = acc.EventTokens |> Map.add key token
                })
            maps

    let maps =
        match eventDefs with
        | first :: _ ->
            let token = tokenMappings.EventTokenMap (enclosing, tdef) first
            let rowId = token &&& 0x00FFFFFF
            { maps with EventMapEntries = maps.EventMapEntries |> Map.add typeName rowId }
        | [] -> maps

    tdef.NestedTypes.AsList()
    |> List.fold (collectType tokenMappings scope (enclosing @ [ tdef ])) maps

let private methodKeyFromRef (methodRef: ILMethodRef) =
    { MethodDefinitionKey.DeclaringType = methodRef.DeclaringTypeRef.FullName
      Name = methodRef.Name
      GenericArity = methodRef.GenericArity
      ParameterTypes = methodRef.ArgTypes |> Seq.toList
      ReturnType = methodRef.ReturnType }

let collectMethodSemanticsEntries
    (ilModule: ILModuleDef)
    (methodTokens: Map<MethodDefinitionKey, int>)
    (propertyTokens: Map<PropertyDefinitionKey, int>)
    (eventTokens: Map<EventDefinitionKey, int>)
    =
    let entries = Dictionary<MethodDefinitionKey, ResizeArray<MethodSemanticsEntry>>(HashIdentity.Structural)
    let mutable nextRowId = 0

    let addEntry methodKey entry =
        match entries.TryGetValue methodKey with
        | true, bucket -> bucket.Add entry
        | _ ->
            let bucket = ResizeArray()
            bucket.Add entry
            entries[methodKey] <- bucket

    let tryAddSemantics association attributes methodRefOpt =
        match methodRefOpt with
        | None -> ()
        | Some methodRef ->
            let methodKey = methodKeyFromRef methodRef
            if methodTokens.ContainsKey methodKey then
                nextRowId <- nextRowId + 1
                addEntry methodKey
                    { RowId = nextRowId
                      Attributes = attributes
                      Association = association }

    let rec visitType enclosing (typeDef: ILTypeDef) =
        let typeRef = mkRefForNestedILTypeDef ILScopeRef.Local (enclosing, typeDef)
        let typeName = typeRef.FullName

        let buildPropertyKey (prop: ILPropertyDef) =
            { PropertyDefinitionKey.DeclaringType = typeName
              Name = prop.Name
              PropertyType = prop.PropertyType
              IndexParameterTypes = List.ofSeq prop.Args }

        let buildEventKey (eventDef: ILEventDef) =
            { EventDefinitionKey.DeclaringType = typeName
              Name = eventDef.Name
              EventType = eventDef.EventType }

        for prop in typeDef.Properties.AsList() do
            let propertyKey = buildPropertyKey prop
            match propertyTokens |> Map.tryFind propertyKey with
            | Some propertyToken ->
                let rowId = propertyToken &&& 0x00FFFFFF
                let association = MethodSemanticsAssociation.PropertyAssociation(propertyKey, rowId)
                tryAddSemantics association MethodSemanticsAttributes.Setter prop.SetMethod
                tryAddSemantics association MethodSemanticsAttributes.Getter prop.GetMethod
            | None -> ()

        for eventDef in typeDef.Events.AsList() do
            let eventKey = buildEventKey eventDef
            match eventTokens |> Map.tryFind eventKey with
            | Some eventToken ->
                let rowId = eventToken &&& 0x00FFFFFF
                let association = MethodSemanticsAssociation.EventAssociation(eventKey, rowId)
                tryAddSemantics association MethodSemanticsAttributes.Adder (Some eventDef.AddMethod)
                tryAddSemantics association MethodSemanticsAttributes.Remover (Some eventDef.RemoveMethod)
                eventDef.FireMethod |> Option.iter (fun fire -> tryAddSemantics association MethodSemanticsAttributes.Raiser (Some fire))
                eventDef.OtherMethods |> List.iter (fun other -> tryAddSemantics association MethodSemanticsAttributes.Other (Some other))
            | None -> ()

        typeDef.NestedTypes.AsList()
        |> List.iter (fun nested -> visitType (enclosing @ [ typeDef ]) nested)

    ilModule.TypeDefs.AsList()
    |> List.iter (visitType [])

    entries
    |> Seq.map (fun kvp -> kvp.Key, kvp.Value |> Seq.toList)
    |> Map.ofSeq

let private createCore
    (moduleId: Guid)
    (ilModule: ILModuleDef)
    (tokenMappings: ILTokenMappings)
    (metadataSnapshot: MetadataSnapshot)
    (ilxGenEnvironment: IlxGenEnvSnapshot option)
    (portablePdbSnapshot: PortablePdbSnapshot option)
    =
    let scope = ILScopeRef.Local

    let maps =
        ilModule.TypeDefs.AsList()
        |> List.fold (collectType tokenMappings scope []) emptyMaps

    let methodSemanticsEntries =
        collectMethodSemanticsEntries ilModule maps.MethodTokens maps.PropertyTokens maps.EventTokens

    let synthesizedNames = collectSynthesizedNameSnapshot ilModule

    {
        ModuleId = moduleId
        EncId = System.Guid.Empty
        EncBaseId = System.Guid.Empty
        NextGeneration = 1
        Metadata = metadataSnapshot
        TokenMappings = tokenMappings
        TypeTokens = maps.TypeTokens
        MethodTokens = maps.MethodTokens
        FieldTokens = maps.FieldTokens
        PropertyTokens = maps.PropertyTokens
        EventTokens = maps.EventTokens
        PropertyMapEntries = maps.PropertyMapEntries
        EventMapEntries = maps.EventMapEntries
        MethodSemanticsEntries = methodSemanticsEntries
        IlxGenEnvironment = ilxGenEnvironment
        PortablePdb = portablePdbSnapshot
        SynthesizedNameSnapshot = synthesizedNames
        MetadataHandles = BaselineHandleCache.Empty
        TypeReferenceTokens = Map.empty
        AssemblyReferenceTokens = Map.empty
        TableEntriesAdded = Array.zeroCreate tableCount
        StringStreamLengthAdded = 0
        UserStringStreamLengthAdded = 0
        BlobStreamLengthAdded = 0
        GuidStreamLengthAdded = 0
        AddedOrChangedMethods = []
        // The baseline PDB is already in memory here (captured alongside the emitted
        // assembly), so the EnC CDI rows are decoded eagerly; flag-off baselines and
        // pre-C2 PDBs decode to the empty map.
        EncMethodDebugInfos =
            portablePdbSnapshot
            |> Option.map (fun snapshot -> readEncMethodDebugInfoFromPortablePdb snapshot.Bytes)
            |> Option.defaultValue Map.empty
        // Closure-name tables are joined from the emit-time stamp recording, which is
        // only available to the capture hook; it attaches them after creation (see
        // resolveClosureNameRowsByToken).
        EncClosureNames = Map.empty
        ModuleNameOffset = None
    }

let internal applyDelta
    (baseline: FSharpEmitBaseline)
    (deltaTableCounts: int[])
    (deltaHeapSizes: MetadataHeapSizes)
    (addedOrChangedMethods: AddedOrChangedMethodInfo list)
    (encId: Guid)
    (encBaseId: Guid)
    (synthesizedSnapshot: Map<string, string[]> option)
    : FSharpEmitBaseline =

    let tableCounts =
        if deltaTableCounts.Length = tableCount then
            deltaTableCounts
        else
            Array.zeroCreate tableCount

    let updatedTableEntries =
        Array.init tableCount (fun i ->
            let previous = baseline.TableEntriesAdded[i]
            previous + tableCounts.[i])

    let updatedMetadataSnapshot =
        // Per Roslyn DeltaMetadataWriter.cs: Blob and UserString streams are concatenated
        // aligned to 4-byte boundaries; String stream is concatenated unaligned.
        let updatedHeapSizes =
            { StringHeapSize = baseline.Metadata.HeapSizes.StringHeapSize + deltaHeapSizes.StringHeapSize
              UserStringHeapSize = baseline.Metadata.HeapSizes.UserStringHeapSize + align4 deltaHeapSizes.UserStringHeapSize
              BlobHeapSize = baseline.Metadata.HeapSizes.BlobHeapSize + align4 deltaHeapSizes.BlobHeapSize
              GuidHeapSize = baseline.Metadata.HeapSizes.GuidHeapSize + deltaHeapSizes.GuidHeapSize }

        if traceHeapOffsets.Value then
            printfn "[fsharp-hotreload][heap-offsets] applyDelta: Updating baseline heap sizes"
            printfn "[fsharp-hotreload][heap-offsets]   Before: UserStringHeapSize = %d" baseline.Metadata.HeapSizes.UserStringHeapSize
            printfn "[fsharp-hotreload][heap-offsets]   Delta:  UserStringHeapSize = %d (aligned = %d)" deltaHeapSizes.UserStringHeapSize (align4 deltaHeapSizes.UserStringHeapSize)
            printfn "[fsharp-hotreload][heap-offsets]   After:  UserStringHeapSize = %d" updatedHeapSizes.UserStringHeapSize
            printfn "[fsharp-hotreload][heap-offsets]   Generation: %d -> %d" baseline.NextGeneration (baseline.NextGeneration + 1)

        let updatedTableCountsAbsolute =
            Array.init tableCount (fun i ->
                baseline.Metadata.TableRowCounts.[i] + tableCounts.[i])

        { baseline.Metadata with
            HeapSizes = updatedHeapSizes
            TableRowCounts = updatedTableCountsAbsolute }

    { baseline with
        EncId = encId
        EncBaseId = encBaseId
        NextGeneration = baseline.NextGeneration + 1
        ModuleNameOffset = baseline.ModuleNameOffset
        TableEntriesAdded = updatedTableEntries
        // Per Roslyn DeltaMetadataWriter.cs: String stream is concatenated unaligned,
        // Blob and UserString streams are concatenated aligned to 4-byte boundaries.
        StringStreamLengthAdded = baseline.StringStreamLengthAdded + deltaHeapSizes.StringHeapSize
        UserStringStreamLengthAdded = baseline.UserStringStreamLengthAdded + align4 deltaHeapSizes.UserStringHeapSize
        BlobStreamLengthAdded = baseline.BlobStreamLengthAdded + align4 deltaHeapSizes.BlobHeapSize
        GuidStreamLengthAdded = baseline.GuidStreamLengthAdded + deltaHeapSizes.GuidHeapSize
        Metadata = updatedMetadataSnapshot
        SynthesizedNameSnapshot =
            match synthesizedSnapshot with
            | Some snapshot -> snapshot
            | None -> baseline.SynthesizedNameSnapshot
        MethodSemanticsEntries = baseline.MethodSemanticsEntries
        AddedOrChangedMethods =
            (addedOrChangedMethods @ baseline.AddedOrChangedMethods)
            |> List.distinctBy (fun info -> info.MethodToken)
        TypeReferenceTokens = baseline.TypeReferenceTokens
        AssemblyReferenceTokens = baseline.AssemblyReferenceTokens
    }

/// <summary>
/// Carries per-method EnC debug information forward into the next-generation baseline after a
/// delta, mirroring how AddedOrChangedMethods chains method state: every updated or added
/// method's entry is replaced by its occurrence data recomputed from the fresh compile, or
/// dropped when the fresh compile produced none (fail closed — the method's lambdas must then
/// be treated as unmappable rather than matched against stale data). Unchanged methods keep
/// their baseline entries. NOTE: the delta PDB does not yet re-emit EnC CDI rows; this
/// in-memory chain is what the generation-aware closure lowering (C3) consumes, and PDB
/// persistence across session restarts is the remaining gap.
/// </summary>
let chainEncMethodDebugInfos
    (baseline: FSharpEmitBaseline)
    (refreshedEncDebugInfos: Map<int, EncMethodDebugInformation>)
    (updatedMethodTokens: int list)
    : FSharpEmitBaseline =
    let chainedInfos =
        (baseline.EncMethodDebugInfos, updatedMethodTokens)
        ||> List.fold (fun acc methodToken ->
            match Map.tryFind methodToken refreshedEncDebugInfos with
            | Some info -> Map.add methodToken info acc
            | None -> Map.remove methodToken acc)

    { baseline with
        EncMethodDebugInfos = chainedInfos }

/// <summary>
/// Recomputes the per-method EnC debug information from the fresh typed tree of an edited
/// compilation, keyed by baseline MethodDef token, for chaining into the next-generation
/// baseline (see chainEncMethodDebugInfos). Name-to-token resolution mirrors the fail-closed
/// write-side keying: only compiled names identifying exactly one baseline MethodDef row
/// resolve, so an entry can never attach to the wrong method. Methods added by the current
/// delta have no baseline token yet and carry no entry (added lambda members are a C4
/// concern).
/// </summary>
/// Baseline MethodDef tokens keyed by method name, restricted to names identifying
/// exactly ONE baseline MethodDef row — the shared fail-closed name -> token resolution
/// for typed-tree-derived per-method side tables (EnC debug info, closure-name tables).
let private tokensByUniqueMethodName (baseline: FSharpEmitBaseline) =
    baseline.MethodTokens
    |> Map.toSeq
    |> Seq.groupBy (fun (key, _) -> key.Name)
    |> Seq.choose (fun (name, entries) ->
        match entries |> Seq.truncate 2 |> List.ofSeq with
        | [ (_, token) ] -> Some(name, token)
        | _ -> None)
    |> Map.ofSeq

let computeRefreshedEncMethodDebugInfos
    (g: TcGlobals)
    (baseline: FSharpEmitBaseline)
    (implementationFiles: CheckedAssemblyAfterOptimization)
    : Map<int, EncMethodDebugInformation> =
    let (CheckedAssemblyAfterOptimization implFiles) = implementationFiles

    let infosByName =
        implFiles
        |> List.map (fun implFile -> implFile.ImplFile)
        |> computeMethodEncDebugInfo g

    if Map.isEmpty infosByName then
        Map.empty
    else
        let tokensByUniqueName = tokensByUniqueMethodName baseline

        (Map.empty, infosByName)
        ||> Map.fold (fun acc methName info ->
            match Map.tryFind methName tokensByUniqueName with
            | Some methodToken -> Map.add methodToken info acc
            | None -> acc)

/// <summary>
/// Re-keys name-keyed per-method closure-name tables (occurrence-chain -> closure type
/// name, produced by ClosureNameAllocator.computeBaselineClosureNameRows in the fsc emit
/// path) by baseline MethodDef token, for storage as FSharpEmitBaseline.EncClosureNames.
/// Resolution is fail closed exactly like computeRefreshedEncMethodDebugInfos: only
/// compiled names identifying exactly one baseline MethodDef row resolve, so a table can
/// never attach to the wrong method.
/// </summary>
let resolveClosureNameRowsByToken
    (baseline: FSharpEmitBaseline)
    (rowsByMethodName: Map<string, Map<int list, string>>)
    : Map<int, Map<int list, string>> =
    if Map.isEmpty rowsByMethodName then
        Map.empty
    else
        let tokensByUniqueName = tokensByUniqueMethodName baseline

        (Map.empty, rowsByMethodName)
        ||> Map.fold (fun acc methName rows ->
            match Map.tryFind methName tokensByUniqueName with
            | Some methodToken -> Map.add methodToken rows acc
            | None -> acc)

/// <summary>Create an <see cref="FSharpEmitBaseline"/> without capturing the ILX environment snapshot.</summary>
let create
    (ilModule: ILModuleDef)
    (tokenMappings: ILTokenMappings)
    (metadataSnapshot: MetadataSnapshot)
    (moduleId: Guid)
    (portablePdbSnapshot: PortablePdbSnapshot option)
    =
    createCore moduleId ilModule tokenMappings metadataSnapshot None portablePdbSnapshot

/// <summary>Create an <see cref="FSharpEmitBaseline"/> that carries the captured ILX environment snapshot.</summary>
let createWithEnvironment
    (ilModule: ILModuleDef)
    (tokenMappings: ILTokenMappings)
    (metadataSnapshot: MetadataSnapshot)
    (ilxGenEnvironment: IlxGenEnvSnapshot)
    (moduleId: Guid)
    (portablePdbSnapshot: PortablePdbSnapshot option)
    =
    createCore moduleId ilModule tokenMappings metadataSnapshot (Some ilxGenEnvironment) portablePdbSnapshot

// ============================================================================
// Byte-based functions using ILBaselineReader (no SRM dependency)
// ============================================================================

/// Extract metadata snapshot from PE file bytes without using SRM.
let metadataSnapshotFromBytes (bytes: byte[]) : MetadataSnapshot option =
    ILBaselineReader.metadataSnapshotFromBytes bytes

/// Read Module.Mvid GUID from PE file bytes without using SRM.
let readModuleMvid (bytes: byte[]) : Guid option =
    ILBaselineReader.readModuleMvidFromBytes bytes

/// Build method handles from baseline using ILBaselineReader.
let private buildMethodHandlesFromBytes (reader: ILBaselineReader.BaselineMetadataReader) (methodTokens: Map<MethodDefinitionKey, int>) : Map<MethodDefinitionKey, MethodDefinitionMetadataHandles> =
    methodTokens
    |> Seq.choose (fun kvp ->
        let key = kvp.Key
        let token = kvp.Value
        let rowId = token &&& 0x00FFFFFF
        match reader.GetMethodDef(rowId) with
        | None -> None
        | Some methodDef ->
            let firstParamRowId =
                match reader.GetMethodParamRange(rowId) with
                | Some (first, _) -> Some first
                | None -> None
            let result : MethodDefinitionMetadataHandles =
                { NameOffset = if methodDef.NameOffset = 0 then None else Some (StringOffset methodDef.NameOffset)
                  SignatureOffset = if methodDef.SignatureOffset = 0 then None else Some (BlobOffset methodDef.SignatureOffset)
                  FirstParameterRowId = firstParamRowId
                  Rva = Some methodDef.RVA
                  Attributes = Some (LanguagePrimitives.EnumOfValue<int, MethodAttributes> methodDef.Flags)
                  ImplAttributes = Some (LanguagePrimitives.EnumOfValue<int, MethodImplAttributes> methodDef.ImplFlags) }
            Some(key, result)
    )
    |> Map.ofSeq

/// Build parameter handles from baseline using ILBaselineReader.
let private buildParameterHandlesFromBytes
    (reader: ILBaselineReader.BaselineMetadataReader)
    (methodTokens: Map<MethodDefinitionKey, int>)
    : Map<ParameterDefinitionKey, ParameterDefinitionMetadataHandles>
    =
    methodTokens
    |> Seq.collect (fun kvp ->
        let methodKey = kvp.Key
        let token = kvp.Value
        let methodRowId = token &&& 0x00FFFFFF
        match reader.GetMethodParamRange(methodRowId) with
        | None -> Seq.empty
        | Some (firstParam, lastParam) ->
            seq {
                for paramRowId in firstParam..lastParam do
                    match reader.GetParam(paramRowId) with
                    | None -> ()
                    | Some param ->
                        let key =
                            { ParameterDefinitionKey.Method = methodKey
                              SequenceNumber = param.Sequence }
                        let result : ParameterDefinitionMetadataHandles =
                            { NameOffset = if param.NameOffset = 0 then None else Some (StringOffset param.NameOffset)
                              RowId = Some paramRowId }
                        yield key, result
            }
    )
    |> Map.ofSeq

/// Build property handles from baseline using ILBaselineReader.
let private buildPropertyHandlesFromBytes (reader: ILBaselineReader.BaselineMetadataReader) (propertyTokens: Map<PropertyDefinitionKey, int>) : Map<PropertyDefinitionKey, PropertyDefinitionMetadataHandles> =
    propertyTokens
    |> Seq.choose (fun kvp ->
        let key = kvp.Key
        let token = kvp.Value
        let rowId = token &&& 0x00FFFFFF
        match reader.GetProperty(rowId) with
        | None -> None
        | Some prop ->
            let result : PropertyDefinitionMetadataHandles =
                { NameOffset = if prop.NameOffset = 0 then None else Some (StringOffset prop.NameOffset)
                  SignatureOffset = if prop.SignatureOffset = 0 then None else Some (BlobOffset prop.SignatureOffset) }
            Some(key, result)
    )
    |> Map.ofSeq

/// Build event handles from baseline using ILBaselineReader.
let private buildEventHandlesFromBytes (reader: ILBaselineReader.BaselineMetadataReader) (eventTokens: Map<EventDefinitionKey, int>) : Map<EventDefinitionKey, EventDefinitionMetadataHandles> =
    eventTokens
    |> Seq.choose (fun kvp ->
        let key = kvp.Key
        let token = kvp.Value
        let rowId = token &&& 0x00FFFFFF
        match reader.GetEvent(rowId) with
        | None -> None
        | Some event ->
            let result : EventDefinitionMetadataHandles =
                { NameOffset = if event.NameOffset = 0 then None else Some (StringOffset event.NameOffset) }
            Some(key, result)
    )
    |> Map.ofSeq

/// Build assembly reference tokens from baseline using ILBaselineReader.
let private buildAssemblyReferenceTokensFromBytes (reader: ILBaselineReader.BaselineMetadataReader) : Map<string, int> =
    seq {
        for rowId in 1..reader.AssemblyRefCount do
            match reader.GetAssemblyRef(rowId) with
            | Some assemblyRef ->
                let name = reader.GetString(assemblyRef.NameOffset)
                // AssemblyRef table index is 0x23, token = (0x23 << 24) | rowId
                let token = (0x23 <<< 24) ||| rowId
                yield name, token
            | None -> ()
    }
    |> Map.ofSeq

/// Build type reference tokens from baseline using ILBaselineReader.
/// Keys carry the full typed scope chain (AssemblyRef name, or the enclosing TypeRef key for
/// nested TypeRefs) so rows with duplicate names under different scopes stay distinguishable.
let private buildTypeReferenceTokensFromBytes (reader: ILBaselineReader.BaselineMetadataReader) : Map<TypeReferenceKey, int> =
    let keyCache = Dictionary<int, TypeReferenceKey option>()

    // Resolution scope chains are bounded by nesting depth; guard against malformed metadata cycles.
    let rec tryKeyForRow (rowId: int) (depth: int) : TypeReferenceKey option =
        if depth > 64 then
            None
        else
            match keyCache.TryGetValue rowId with
            | true, cached -> cached
            | _ ->
                let result =
                    match reader.GetTypeRef(rowId) with
                    | None -> None
                    | Some typeRef ->
                        let (tableIndex, scopeRowId) = reader.DecodeResolutionScope(typeRef.ResolutionScope)

                        let scopeOpt =
                            // AssemblyRef scope (table 0x23 = 35)
                            if tableIndex = 35 then
                                reader.GetAssemblyRef(scopeRowId)
                                |> Option.map (fun assemblyRef ->
                                    TypeReferenceScope.Assembly(reader.GetString(assemblyRef.NameOffset)))
                            // Nested TypeRef scope (table 0x01 = 1)
                            elif tableIndex = 1 && scopeRowId <> rowId then
                                tryKeyForRow scopeRowId (depth + 1) |> Option.map TypeReferenceScope.Nested
                            else
                                // Module/ModuleRef scopes have no stable cross-compilation identity here.
                                None

                        scopeOpt
                        |> Option.map (fun scope ->
                            { TypeReferenceKey.Scope = scope
                              Namespace = reader.GetString(typeRef.NamespaceOffset)
                              Name = reader.GetString(typeRef.NameOffset) })

                keyCache[rowId] <- result
                result

    seq {
        for rowId in 1..reader.TypeRefCount do
            match tryKeyForRow rowId 0 with
            | Some key ->
                // TypeRef table index is 0x01, token = (0x01 << 24) | rowId
                yield key, (0x01 <<< 24) ||| rowId
            | None -> ()
    }
    |> Map.ofSeq

/// Attach metadata handles from PE bytes without using SRM MetadataReader.
let attachMetadataHandlesFromBytes (bytes: byte[]) (baseline: FSharpEmitBaseline) : FSharpEmitBaseline =
    match ILBaselineReader.BaselineMetadataReader.Create(bytes) with
    | None -> baseline  // Return unchanged if we can't read the metadata
    | Some reader ->
        let methodHandles = buildMethodHandlesFromBytes reader baseline.MethodTokens
        let parameterHandles = buildParameterHandlesFromBytes reader baseline.MethodTokens
        let propertyHandles = buildPropertyHandlesFromBytes reader baseline.PropertyTokens
        let eventHandles = buildEventHandlesFromBytes reader baseline.EventTokens
        let typeReferenceTokens = buildTypeReferenceTokensFromBytes reader
        let assemblyReferenceTokens = buildAssemblyReferenceTokensFromBytes reader
        let cache =
            { MethodHandles = methodHandles
              ParameterHandles = parameterHandles
              PropertyHandles = propertyHandles
              EventHandles = eventHandles }
        let moduleNameOffset =
            match reader.GetModule() with
            | Some m when m.NameOffset > 0 -> Some (StringOffset m.NameOffset)
            | _ -> None
        { baseline with
            MetadataHandles = cache
            ModuleNameOffset = moduleNameOffset
            TypeReferenceTokens = typeReferenceTokens
            AssemblyReferenceTokens = assemblyReferenceTokens }

/// <summary>
/// Create a baseline directly from emitted assembly artifacts.
/// Shared by CLI and checker entry points to keep token/heap capture behavior aligned.
/// </summary>
let createFromEmittedArtifacts
    (ilModule: ILModuleDef)
    (tokenMappings: ILTokenMappings)
    (assemblyBytes: byte[])
    (portablePdbSnapshot: PortablePdbSnapshot option)
    (ilxGenEnvironment: IlxGenEnvSnapshot option)
    : FSharpEmitBaseline
    =
    let moduleId = readModuleMvid assemblyBytes |> Option.defaultWith System.Guid.NewGuid
    let metadataSnapshot =
        metadataSnapshotFromBytes assemblyBytes
        |> Option.defaultWith (fun () -> failwith "Failed to read metadata from assembly bytes")

    let baselineCore =
        match ilxGenEnvironment with
        | Some snapshot ->
            createWithEnvironment
                ilModule
                tokenMappings
                metadataSnapshot
                snapshot
                moduleId
                portablePdbSnapshot
        | None ->
            create
                ilModule
                tokenMappings
                metadataSnapshot
                moduleId
                portablePdbSnapshot

    attachMetadataHandlesFromBytes assemblyBytes baselineCore
