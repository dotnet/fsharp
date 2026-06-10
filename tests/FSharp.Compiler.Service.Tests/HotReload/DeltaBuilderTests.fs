namespace FSharp.Compiler.Service.Tests.HotReload

open System
open Xunit

open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILBinaryWriter
open FSharp.Compiler.HotReload.DeltaBuilder
open FSharp.Compiler.HotReload.SymbolChanges
open FSharp.Compiler.HotReloadBaseline
open FSharp.Compiler.TypedTreeDiff

module DeltaBuilderTests =

    let private createBaseline (typeTokens: Map<string, int>) (methodTokens: Map<MethodDefinitionKey, int>) =
        let metadataSnapshot: MetadataSnapshot =
            { HeapSizes =
                { StringHeapSize = 64
                  UserStringHeapSize = 32
                  BlobHeapSize = 64
                  GuidHeapSize = 16 }
              TableRowCounts = Array.create 64 0
              GuidHeapStart = 0 }

        { ModuleId = System.Guid.NewGuid()
          EncId = System.Guid.Empty
          EncBaseId = System.Guid.Empty
          NextGeneration = 1
          ModuleNameOffset = None
          Metadata = metadataSnapshot
          TokenMappings =
            { TypeDefTokenMap = fun _ -> 0
              FieldDefTokenMap = fun _ _ -> 0
              MethodDefTokenMap = fun _ _ -> 0
              PropertyTokenMap = fun _ _ -> 0
              EventTokenMap = fun _ _ -> 0 }
          TypeTokens = typeTokens
          MethodTokens = methodTokens
          FieldTokens = Map.empty
          PropertyTokens = Map.empty
          EventTokens = Map.empty
          PropertyMapEntries = Map.empty
          EventMapEntries = Map.empty
          MethodSemanticsEntries = Map.empty
          IlxGenEnvironment = None
          PortablePdb = None
          SynthesizedNameSnapshot = Map.empty
          MetadataHandles =
            { MethodHandles = Map.empty
              ParameterHandles = Map.empty
              PropertyHandles = Map.empty
              EventHandles = Map.empty }
          TypeReferenceTokens = Map.empty
          AssemblyReferenceTokens = Map.empty
          TableEntriesAdded = Array.zeroCreate 64
          StringStreamLengthAdded = 0
          UserStringStreamLengthAdded = 0
          BlobStreamLengthAdded = 0
          GuidStreamLengthAdded = 0
          AddedOrChangedMethods = [] }

    let private mkSymbol
        (path: string list)
        (logicalName: string)
        (stamp: int64)
        (kind: SymbolKind)
        (memberKind: SymbolMemberKind option)
        =
        { SymbolId.Path = path
          LogicalName = logicalName
          Stamp = stamp
          Kind = kind
          MemberKind = memberKind
          IsSynthesized = false
          CompiledName = None
          TotalArgCount = None
          GenericArity = None
          ParameterTypeIdentities = None
          ReturnTypeIdentity = None }

    [<Fact>]
    let ``mapSymbolChangesToDelta resolves nested entity by normalized type path`` () =
        let baselineTypeName = "Sample.Container+Nested"

        let baseline =
            createBaseline
                (Map.ofList [ baselineTypeName, 0x02000002 ])
                Map.empty

        let symbol =
            mkSymbol [ "Sample"; "Container" ] "Nested" 1L SymbolKind.Entity None

        let changes: FSharpSymbolChanges =
            { Added = []
              Updated =
                [ { UpdatedSymbolChange.Symbol = symbol
                    Kind = SemanticEditKind.TypeDefinition
                    ContainingEntity = None } ]
              Deleted = []
              Synthesized = []
              RudeEdits = [] }

        let updatedTypes, updatedMethods, accessorUpdates =
            match mapSymbolChangesToDelta baseline changes with
            | Ok result -> result
            | Error errors -> failwithf "Expected successful mapping, got %A" errors

        Assert.Equal<string list>([ baselineTypeName ], updatedTypes)
        Assert.Empty(updatedMethods)
        Assert.Empty(accessorUpdates)

    [<Fact>]
    let ``mapSymbolChangesToDelta resolves method update when nested type separators differ`` () =
        let baselineTypeName = "Sample.Container+Nested"

        let methodKey: MethodDefinitionKey =
            { DeclaringType = baselineTypeName
              Name = "Run"
              GenericArity = 0
              ParameterTypes = []
              ReturnType = ILType.Void }

        let baseline =
            createBaseline
                (Map.ofList [ baselineTypeName, 0x02000002 ])
                (Map.ofList [ methodKey, 0x06000002 ])

        let methodSymbol =
            { mkSymbol [ "Sample"; "Container"; "Nested" ] "Run" 2L SymbolKind.Value (Some SymbolMemberKind.Method) with
                CompiledName = Some "Run"
                TotalArgCount = Some 0
                GenericArity = Some 0
                ParameterTypeIdentities = Some []
                ReturnTypeIdentity = Some RuntimeTypeIdentity.VoidType }

        let changes: FSharpSymbolChanges =
            { Added = []
              Updated =
                [ { UpdatedSymbolChange.Symbol = methodSymbol
                    Kind = SemanticEditKind.MethodBody
                    ContainingEntity = None } ]
              Deleted = []
              Synthesized = []
              RudeEdits = [] }

        let updatedTypes, updatedMethods, accessorUpdates =
            match mapSymbolChangesToDelta baseline changes with
            | Ok result -> result
            | Error errors -> failwithf "Expected successful mapping, got %A" errors

        Assert.Empty(updatedTypes)
        Assert.Equal<MethodDefinitionKey list>([ methodKey ], updatedMethods)
        Assert.Empty(accessorUpdates)

    [<Fact>]
    let ``mapSymbolChangesToDelta resolves explicit containing entity with normalized separators`` () =
        let baselineTypeName = "Sample.Container+Nested"

        let methodKey: MethodDefinitionKey =
            { DeclaringType = baselineTypeName
              Name = "Run"
              GenericArity = 0
              ParameterTypes = []
              ReturnType = ILType.Void }

        let baseline =
            createBaseline
                (Map.ofList [ baselineTypeName, 0x02000002 ])
                (Map.ofList [ methodKey, 0x06000002 ])

        let methodSymbol =
            { mkSymbol [ "Sample"; "Container"; "Nested" ] "Run" 21L SymbolKind.Value (Some SymbolMemberKind.Method) with
                CompiledName = Some "Run"
                TotalArgCount = Some 0
                GenericArity = Some 0
                ParameterTypeIdentities = Some []
                ReturnTypeIdentity = Some RuntimeTypeIdentity.VoidType }

        let changes: FSharpSymbolChanges =
            { Added = []
              Updated =
                [ { UpdatedSymbolChange.Symbol = methodSymbol
                    Kind = SemanticEditKind.MethodBody
                    ContainingEntity = Some "Sample.Container.Nested" } ]
              Deleted = []
              Synthesized = []
              RudeEdits = [] }

        let updatedTypes, updatedMethods, accessorUpdates =
            match mapSymbolChangesToDelta baseline changes with
            | Ok result -> result
            | Error errors -> failwithf "Expected successful mapping, got %A" errors

        Assert.Empty(updatedTypes)
        Assert.Equal<MethodDefinitionKey list>([ methodKey ], updatedMethods)
        Assert.Empty(accessorUpdates)

    [<Fact>]
    let ``mapSymbolChangesToDelta fails closed when explicit containing entity cannot resolve`` () =
        let baselineTypeName = "Sample.Container+Nested"

        let methodKey: MethodDefinitionKey =
            { DeclaringType = baselineTypeName
              Name = "Run"
              GenericArity = 0
              ParameterTypes = []
              ReturnType = ILType.Void }

        let baseline =
            createBaseline
                (Map.ofList [ baselineTypeName, 0x02000002 ])
                (Map.ofList [ methodKey, 0x06000002 ])

        let methodSymbol =
            { mkSymbol [ "Sample"; "Container"; "Nested" ] "Run" 22L SymbolKind.Value (Some SymbolMemberKind.Method) with
                CompiledName = Some "Run"
                TotalArgCount = Some 0
                GenericArity = Some 0
                ParameterTypeIdentities = Some []
                ReturnTypeIdentity = Some RuntimeTypeIdentity.VoidType }

        let changes: FSharpSymbolChanges =
            { Added = []
              Updated =
                [ { UpdatedSymbolChange.Symbol = methodSymbol
                    Kind = SemanticEditKind.MethodBody
                    ContainingEntity = Some "Sample.Unrelated.Type" } ]
              Deleted = []
              Synthesized = []
              RudeEdits = [] }

        match mapSymbolChangesToDelta baseline changes with
        | Ok _ -> failwith "Expected explicit containing-entity mismatch to fail closed"
        | Error errors ->
            Assert.Contains(
                errors,
                fun message ->
                    message.Contains("Unable to resolve explicit containing entity", StringComparison.Ordinal)
                    && message.Contains("full rebuild required", StringComparison.Ordinal)
            )

    [<Fact>]
    let ``mapSymbolChangesToDelta fails closed on ambiguous containing type mapping`` () =
        let primaryTypeName = "Sample.Container+Nested"
        let secondaryTypeName = "Sample+Container+Nested"

        let primaryMethod: MethodDefinitionKey =
            { DeclaringType = primaryTypeName
              Name = "Run"
              GenericArity = 0
              ParameterTypes = []
              ReturnType = ILType.Void }

        let secondaryMethod: MethodDefinitionKey =
            { DeclaringType = secondaryTypeName
              Name = "Run"
              GenericArity = 0
              ParameterTypes = []
              ReturnType = ILType.Void }

        let baseline =
            createBaseline
                (Map.ofList [ primaryTypeName, 0x02000002
                              secondaryTypeName, 0x02000003 ])
                (Map.ofList [ primaryMethod, 0x06000002
                              secondaryMethod, 0x06000003 ])

        let methodSymbol =
            { mkSymbol [ "Sample"; "Container"; "Nested" ] "Run" 3L SymbolKind.Value (Some SymbolMemberKind.Method) with
                CompiledName = Some "Run"
                TotalArgCount = Some 0
                GenericArity = Some 0
                ParameterTypeIdentities = Some []
                ReturnTypeIdentity = Some RuntimeTypeIdentity.VoidType }

        let changes: FSharpSymbolChanges =
            { Added = []
              Updated =
                [ { UpdatedSymbolChange.Symbol = methodSymbol
                    Kind = SemanticEditKind.MethodBody
                    ContainingEntity = None } ]
              Deleted = []
              Synthesized = []
              RudeEdits = [] }

        match mapSymbolChangesToDelta baseline changes with
        | Ok _ -> failwith "Expected ambiguous containing type mapping to fail closed"
        | Error errors ->
            Assert.Contains(errors, fun message -> message.Contains("Ambiguous containing type mapping", StringComparison.Ordinal))

    [<Fact>]
    let ``mapSymbolChangesToDelta fails closed when runtime method identity is incomplete`` () =
        let baselineTypeName = "Sample.Container+Nested"

        let methodKey: MethodDefinitionKey =
            { DeclaringType = baselineTypeName
              Name = "Run"
              GenericArity = 0
              ParameterTypes = []
              ReturnType = ILType.Void }

        let baseline =
            createBaseline
                (Map.ofList [ baselineTypeName, 0x02000002 ])
                (Map.ofList [ methodKey, 0x06000002 ])

        let methodSymbol =
            { mkSymbol [ "Sample"; "Container"; "Nested" ] "Run" 4L SymbolKind.Value (Some SymbolMemberKind.Method) with
                CompiledName = Some "Run"
                TotalArgCount = Some 0
                GenericArity = Some 0
                ParameterTypeIdentities = None
                ReturnTypeIdentity = Some RuntimeTypeIdentity.VoidType }

        let changes: FSharpSymbolChanges =
            { Added = []
              Updated =
                [ { UpdatedSymbolChange.Symbol = methodSymbol
                    Kind = SemanticEditKind.MethodBody
                    ContainingEntity = None } ]
              Deleted = []
              Synthesized = []
              RudeEdits = [] }

        match mapSymbolChangesToDelta baseline changes with
        | Ok _ -> failwith "Expected incomplete runtime method identity to fail closed"
        | Error errors ->
            Assert.Contains(errors, fun message -> message.Contains("runtime signature identity is incomplete", StringComparison.Ordinal))

    [<Fact>]
    let ``mapSymbolChangesToDelta fails closed when parameter identity mismatches`` () =
        let baselineTypeName = "Sample.Container+Nested"

        let methodKey: MethodDefinitionKey =
            { DeclaringType = baselineTypeName
              Name = "Run"
              GenericArity = 0
              ParameterTypes = [ ILType.TypeVar 0us ]
              ReturnType = ILType.Void }

        let baseline =
            createBaseline
                (Map.ofList [ baselineTypeName, 0x02000002 ])
                (Map.ofList [ methodKey, 0x06000002 ])

        let methodSymbol =
            { mkSymbol [ "Sample"; "Container"; "Nested" ] "Run" 5L SymbolKind.Value (Some SymbolMemberKind.Method) with
                CompiledName = Some "Run"
                TotalArgCount = Some 1
                GenericArity = Some 0
                ParameterTypeIdentities = Some [ RuntimeTypeIdentity.NamedType("System.String", []) ]
                ReturnTypeIdentity = Some RuntimeTypeIdentity.VoidType }

        let changes: FSharpSymbolChanges =
            { Added = []
              Updated =
                [ { UpdatedSymbolChange.Symbol = methodSymbol
                    Kind = SemanticEditKind.MethodBody
                    ContainingEntity = None } ]
              Deleted = []
              Synthesized = []
              RudeEdits = [] }

        match mapSymbolChangesToDelta baseline changes with
        | Ok _ -> failwith "Expected parameter mismatch to fail closed"
        | Error errors ->
            Assert.Contains(
                errors,
                fun message ->
                    message.Contains("Unable to resolve changed method symbol", StringComparison.Ordinal)
                    && message.Contains("full rebuild required", StringComparison.Ordinal)
            )

    [<Fact>]
    let ``mapSymbolChangesToDelta fails closed when explicit accessor containing entity cannot resolve`` () =
        let accessorSymbol =
            { mkSymbol [ "Sample"; "Container" ] "get_Value" 99L SymbolKind.Value (Some(SymbolMemberKind.PropertyGet "Value")) with
                CompiledName = Some "get_Value"
                TotalArgCount = Some 0
                GenericArity = Some 0
                ParameterTypeIdentities = Some []
                ReturnTypeIdentity = Some(RuntimeTypeIdentity.NamedType("System.Int32", [])) }

        let changes: FSharpSymbolChanges =
            { Added = []
              Updated =
                [ { UpdatedSymbolChange.Symbol = accessorSymbol
                    Kind = SemanticEditKind.MethodBody
                    ContainingEntity = Some "Sample.Missing" } ]
              Deleted = []
              Synthesized = []
              RudeEdits = [] }

        let baseline = createBaseline Map.empty Map.empty

        match mapSymbolChangesToDelta baseline changes with
        | Ok _ -> failwith "Expected explicit accessor containing entity mismatch to fail closed"
        | Error errors ->
            Assert.Contains(errors, fun message -> message.Contains("explicit accessor containing entity", StringComparison.Ordinal))

    [<Fact>]
    let ``mapSymbolChangesToDelta fails closed when return identity mismatches`` () =
        let baselineTypeName = "Sample.Container+Nested"

        let methodKey: MethodDefinitionKey =
            { DeclaringType = baselineTypeName
              Name = "Run"
              GenericArity = 0
              ParameterTypes = []
              ReturnType = ILType.TypeVar 0us }

        let baseline =
            createBaseline
                (Map.ofList [ baselineTypeName, 0x02000002 ])
                (Map.ofList [ methodKey, 0x06000002 ])

        let methodSymbol =
            { mkSymbol [ "Sample"; "Container"; "Nested" ] "Run" 6L SymbolKind.Value (Some SymbolMemberKind.Method) with
                CompiledName = Some "Run"
                TotalArgCount = Some 0
                GenericArity = Some 0
                ParameterTypeIdentities = Some []
                ReturnTypeIdentity = Some RuntimeTypeIdentity.VoidType }

        let changes: FSharpSymbolChanges =
            { Added = []
              Updated =
                [ { UpdatedSymbolChange.Symbol = methodSymbol
                    Kind = SemanticEditKind.MethodBody
                    ContainingEntity = None } ]
              Deleted = []
              Synthesized = []
              RudeEdits = [] }

        match mapSymbolChangesToDelta baseline changes with
        | Ok _ -> failwith "Expected return-type mismatch to fail closed"
        | Error errors ->
            Assert.Contains(
                errors,
                fun message ->
                    message.Contains("Unable to resolve changed method symbol", StringComparison.Ordinal)
                    && message.Contains("full rebuild required", StringComparison.Ordinal)
            )
