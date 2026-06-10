namespace FSharp.Compiler.Service.Tests.HotReload

#nowarn "3391" // Suppress implicit conversion warnings for SRM handle conversions

open System
open System.IO
open System.Reflection
open System.Collections.Generic
open System.Collections.Immutable
open System.Reflection.Metadata
open System.Reflection.Metadata.Ecma335
open System.Reflection.PortableExecutable
open System.Text
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILBinaryWriter
open FSharp.Compiler.AbstractIL.ILPdbWriter
open Internal.Utilities
open Internal.Utilities.Library
open FSharp.Compiler.HotReloadBaseline
open FSharp.Compiler.IlxDeltaStreams
open FSharp.Compiler.CodeGen
open FSharp.Compiler.CodeGen.DeltaMetadataTables
open FSharp.Compiler.CodeGen.DeltaMetadataTypes
open FSharp.Compiler.AbstractIL.BinaryConstants
open FSharp.Compiler.AbstractIL.ILDeltaHandles

module internal MetadataDeltaTestHelpers =
    module ILWriter = FSharp.Compiler.AbstractIL.ILBinaryWriter
    module ILPdbWriter = FSharp.Compiler.AbstractIL.ILPdbWriter
    module DeltaWriter = FSharp.Compiler.CodeGen.FSharpDeltaMetadataWriter

    let private shouldTraceMetadata () =
        match Environment.GetEnvironmentVariable("FSHARP_HOTRELOAD_TRACE_METADATA") with
        | null -> false
        | value when String.Equals(value, "1", StringComparison.OrdinalIgnoreCase) -> true
        | value when String.Equals(value, "true", StringComparison.OrdinalIgnoreCase) -> true
        | _ -> false

    /// Convert SRM MethodDefinitionHandle to F# MethodDefHandle
    let private toMethodDefHandle (handle: MethodDefinitionHandle) =
        let entityHandle: EntityHandle = handle
        MethodDefHandle (MetadataTokens.GetRowNumber entityHandle)

    let private mscorlibToken =
        PublicKeyToken [|
            0xb7uy; 0x7auy; 0x5cuy; 0x56uy; 0x19uy; 0x34uy; 0xe0uy; 0x89uy
        |]

    let private fsharpCoreToken =
        PublicKeyToken [|
            0xb0uy; 0x3fuy; 0x5fuy; 0x7fuy; 0x11uy; 0xd5uy; 0x0auy; 0x3auy
        |]

    let private mscorlibRef =
        ILAssemblyRef.Create(
            "mscorlib",
            None,
            Some mscorlibToken,
            false,
            Some(ILVersionInfo(4us, 0us, 0us, 0us)),
            None)

    let private fsharpCoreRef =
        ILAssemblyRef.Create(
            "FSharp.Core",
            None,
            Some fsharpCoreToken,
            false,
            Some(ILVersionInfo(0us, 0us, 0us, 0us)),
            None)

    let ilGlobals =
        mkILGlobals(ILScopeRef.Assembly mscorlibRef, [], ILScopeRef.Assembly fsharpCoreRef)

    let simpleTypeName (fullName: string) =
        match fullName.LastIndexOf('.') with
        | -1 -> fullName
        | idx when idx = fullName.Length - 1 -> ""
        | idx -> fullName.Substring(idx + 1)

    let findMethodHandle (metadataReader: MetadataReader) (typeFullName: string) (methodName: string) =
        let expectedType = simpleTypeName typeFullName

        metadataReader.MethodDefinitions
        |> Seq.find (fun handle ->
            let methodDef = metadataReader.GetMethodDefinition(handle)
            let declaringType = metadataReader.GetTypeDefinition(methodDef.GetDeclaringType())
            let declaringName = metadataReader.GetString(declaringType.Name)
            declaringName = expectedType
            && metadataReader.GetString(methodDef.Name) = methodName)

    let private getRowCounts (metadataReader: MetadataReader) =
        Array.init MetadataTokens.TableCount (fun i ->
            let table = LanguagePrimitives.EnumOfValue<byte, TableIndex>(byte i)
            metadataReader.GetTableRowCount table)

    let private inspectDeltaMetadata label (bytes: byte[]) =
        try
            use provider = MetadataReaderProvider.FromMetadataImage(ImmutableArray.CreateRange<byte>(bytes))
            let reader = provider.GetMetadataReader()
            let encMapCount = reader.GetTableRowCount(TableIndex.EncMap)
            let encLogCount = reader.GetTableRowCount(TableIndex.EncLog)
            let methodCount = reader.GetTableRowCount(TableIndex.MethodDef)
            let propertyCount = reader.GetTableRowCount(TableIndex.Property)
            printfn
                "[hotreload-metadata] %s encMap=%d encLog=%d methodRows=%d propertyRows=%d"
                label
                encMapCount
                encLogCount
                methodCount
                propertyCount
        with ex ->
            printfn "[hotreload-metadata] %s inspect failed: %s" label ex.Message

    let private defaultWriterOptions (ilg: ILGlobals) : ILWriter.options =
        { ilg = ilg
          outfile = Path.GetTempFileName()
          pdbfile = None
          portablePDB = true
          embeddedPDB = false
          embedAllSource = false
          embedSourceList = []
          allGivenSources = []
          sourceLink = ""
          checksumAlgorithm = ILPdbWriter.HashAlgorithm.Sha256
          signer = None
          emitTailcalls = false
          deterministic = true
          dumpDebugInfo = false
          referenceAssemblyOnly = false
          referenceAssemblyAttribOpt = None
          referenceAssemblySignatureHash = None
          pathMap = PathMap.empty }

    let createAssemblyBytes (moduleDef: ILModuleDef) =
        let options = defaultWriterOptions ilGlobals
        ILWriter.WriteILBinaryInMemoryWithArtifacts(options, moduleDef, id)

    let padTo4 (bytes: byte[]) =
        if bytes.Length % 4 = 0 then bytes
        else
            let padded = Array.zeroCreate<byte> (bytes.Length + (4 - (bytes.Length % 4)))
            Array.Copy(bytes, padded, bytes.Length)
            padded

    let tryExtractTablesStream (metadata: byte[]) =
        use stream = new MemoryStream(metadata, false)
        use reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen = true)

        let readUInt32 () = reader.ReadUInt32()
        let readUInt16 () = reader.ReadUInt16()

        let _signature = readUInt32 ()
        let _major = readUInt16 ()
        let _minor = readUInt16 ()
        let _reserved = readUInt32 ()
        let versionLength = int (readUInt32 ())
        reader.ReadBytes(versionLength) |> ignore
        while stream.Position % 4L <> 0L do
            reader.ReadByte() |> ignore

        let _flags = readUInt16 ()
        let streamCount = int (readUInt16 ())

        let readStreamName () =
            let buffer = ResizeArray()
            let mutable finished = false
            while not finished do
                let b = reader.ReadByte()
                if b = 0uy then
                    finished <- true
                else
                    buffer.Add b
            while stream.Position % 4L <> 0L do
                reader.ReadByte() |> ignore
            Encoding.UTF8.GetString(buffer.ToArray())

        let mutable tablesOffset = ValueNone
        let mutable tablesSize = 0u

        for _ in 1 .. streamCount do
            let offset = readUInt32 ()
            let size = readUInt32 ()
            let name = readStreamName ()
            if name = "#~" then
                tablesOffset <- ValueSome offset
                tablesSize <- size

        match tablesOffset with
        | ValueSome offset ->
            let start = int offset
            let size = int tablesSize
            let unpadded = Array.sub metadata start size
            let padded = padTo4 unpadded
            Some(size, padded)
        | ValueNone ->
            None

    let private dumpMetadataLayout label (metadata: byte[]) =
        use stream = new MemoryStream(metadata, false)
        use reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen = true)

        let signature = reader.ReadUInt32()
        let major = int (reader.ReadUInt16())
        let minor = int (reader.ReadUInt16())
        let _reserved = reader.ReadUInt32()
        let versionLength = int (reader.ReadUInt32 ())
        let versionBytes = reader.ReadBytes(versionLength)
        while stream.Position % 4L <> 0L do
            reader.ReadByte() |> ignore
        let flags = int (reader.ReadUInt16())
        let streamCount = int (reader.ReadUInt16())

        printfn
            "[hotreload-metadata] %s signature=0x%08X v%d.%d version=%s flags=0x%04X streams=%d"
            label
            signature
            major
            minor
            (Encoding.UTF8.GetString(versionBytes))
            flags
            streamCount

        let readStreamName () =
            let buffer = ResizeArray()
            let mutable finished = false
            while not finished do
                let b = reader.ReadByte()
                if b = 0uy then
                    finished <- true
                else
                    buffer.Add b
            while stream.Position % 4L <> 0L do
                reader.ReadByte() |> ignore
            Encoding.UTF8.GetString(buffer.ToArray())

        for _ = 1 to streamCount do
            let offset = reader.ReadUInt32()
            let size = reader.ReadUInt32()
            let name = readStreamName ()
            printfn "[hotreload-metadata]   stream %-8s offset=%6d size=%6d" name offset size

    let methodKeyWithParameters (typeName: string) name (parameterTypes: ILType list) returnType =
        { DeclaringType = typeName
          Name = name
          GenericArity = 0
          ParameterTypes = parameterTypes
          ReturnType = returnType }

    let methodKey (typeName: string) name returnType =
        methodKeyWithParameters typeName name [] returnType

    let private getHeapSizes (metadataReader: MetadataReader) =
        { StringHeapSize = metadataReader.GetHeapSize HeapIndex.String
          UserStringHeapSize = metadataReader.GetHeapSize HeapIndex.UserString
          BlobHeapSize = metadataReader.GetHeapSize HeapIndex.Blob
          GuidHeapSize = metadataReader.GetHeapSize HeapIndex.Guid }

    let private computeHeapOffsets metadataReader =
        metadataReader
        |> getHeapSizes
        |> MetadataHeapOffsets.OfHeapSizes

    let private advanceHeapOffsets (offsets: MetadataHeapOffsets) (delta: DeltaWriter.MetadataDelta) =
        { StringHeapStart = offsets.StringHeapStart + delta.HeapSizes.StringHeapSize
          BlobHeapStart = offsets.BlobHeapStart + delta.HeapSizes.BlobHeapSize
          GuidHeapStart = offsets.GuidHeapStart + delta.HeapSizes.GuidHeapSize
          UserStringHeapStart = offsets.UserStringHeapStart + delta.HeapSizes.UserStringHeapSize }

    let assertTableStreamMatches (metadataDelta: DeltaWriter.MetadataDelta) =
        match tryExtractTablesStream metadataDelta.Metadata with
        | Some(size, padded) ->
            Xunit.Assert.Equal(size, metadataDelta.TableStream.UnpaddedSize)
            Xunit.Assert.Equal<byte>(padded, metadataDelta.TableStream.Bytes)
        | None ->
            ()

    let serializeWithMetadataBuilder (metadataBuilder: MetadataBuilder) =
        let metadataRoot = MetadataRootBuilder(metadataBuilder)
        let blob = BlobBuilder()
        metadataRoot.Serialize(blob, 0, 0)
        blob.ToArray()

    let createPropertyModule (messageLiteral: string option) () =
        let ilg = ilGlobals
        let stringType = ilg.typ_String
        let typeName = "Sample.PropertyHost"
        let literal = defaultArg messageLiteral "delta"

        let getterBody =
            mkMethodBody(
                false,
                [],
                2,
                nonBranchingInstrsToCode [ I_ldstr literal; I_ret ],
                None,
                None)

        let getter =
            mkILNonGenericInstanceMethod(
                "get_Message",
                ILMemberAccess.Public,
                [],
                mkILReturn stringType,
                getterBody)
            |> fun def -> def.WithSpecialName.WithHideBySig(true)

        let propertyDef =
            ILPropertyDef(
                "Message",
                PropertyAttributes.None,
                None,
                Some(mkILMethRef(mkILTyRef(ILScopeRef.Local, typeName), ILCallingConv.Instance, "get_Message", 0, [], stringType)),
                ILThisConvention.Instance,
                stringType,
                None,
                [],
                emptyILCustomAttrs)

        let typeDef =
            mkILSimpleClass
                ilg
                (
                    typeName,
                    ILTypeDefAccess.Public,
                    mkILMethods [ getter ],
                    mkILFields [],
                    emptyILTypeDefs,
                    mkILProperties [ propertyDef ],
                    mkILEvents [],
                    emptyILCustomAttrs,
                    ILTypeInit.BeforeField )

        mkILSimpleModule
            "SampleAssembly"
            "SampleModule"
            true
            (4, 0)
            false
            (mkILTypeDefs [ typeDef ])
            None
            None
            0
            (mkILExportedTypes [])
            "v4.0.30319"

    let createLocalSignatureModule (messageLiteral: string option) () =
        let ilg = ilGlobals
        let stringType = ilg.typ_String
        let typeName = "Sample.LocalSignatureHost"
        let literal = defaultArg messageLiteral "local"

        let locals = [ mkILLocal stringType None ]

        let methodBody =
            mkMethodBody(
                false,
                locals,
                2,
                nonBranchingInstrsToCode [ I_ldstr literal; I_stloc 0us; I_ldloc 0us; I_ret ],
                None,
                None)

        let methodDef =
            mkILNonGenericStaticMethod(
                "FormatMessage",
                ILMemberAccess.Public,
                [],
                mkILReturn stringType,
                methodBody)

        let typeDef =
            mkILSimpleClass
                ilg
                (
                    typeName,
                    ILTypeDefAccess.Public,
                    mkILMethods [ methodDef ],
                    mkILFields [],
                    emptyILTypeDefs,
                    mkILProperties [],
                    mkILEvents [],
                    emptyILCustomAttrs,
                    ILTypeInit.BeforeField )

        mkILSimpleModule
            "SampleAssembly"
            "SampleModule"
            true
            (4, 0)
            false
            (mkILTypeDefs [ typeDef ])
            None
            None
            0
            (mkILExportedTypes [])
            "v4.0.30319"

    let createEventModule (messageLiteral: string option) () =
        let ilg = ilGlobals
        let typeName = "Sample.EventHost"
        let typeRef = mkILTyRef(ILScopeRef.Local, typeName)
        let literal = defaultArg messageLiteral "event baseline payload"
        let handlerType = ilg.typ_Object

        let addBody =
            mkMethodBody(
                false,
                [],
                2,
                nonBranchingInstrsToCode [ I_ldstr literal; AI_pop; I_ret ],
                None,
                None)

        let removeBody =
            mkMethodBody(
                false,
                [],
                1,
                nonBranchingInstrsToCode [ I_ret ],
                None,
                None)

        let makeAccessor name =
            mkILNonGenericInstanceMethod(
                name,
                ILMemberAccess.Public,
                [ mkILParamNamed("handler", handlerType) ],
                mkILReturn ILType.Void,
                if name.StartsWith("add", StringComparison.Ordinal) then addBody else removeBody)
            |> fun methodDef -> methodDef.WithSpecialName.WithHideBySig(true)

        let addMethod = makeAccessor "add_OnChanged"
        let removeMethod = makeAccessor "remove_OnChanged"

        let eventDef =
            ILEventDef(
                Some handlerType,
                "OnChanged",
                EventAttributes.None,
                mkILMethRef(typeRef, ILCallingConv.Instance, "add_OnChanged", 0, [ handlerType ], ILType.Void),
                mkILMethRef(typeRef, ILCallingConv.Instance, "remove_OnChanged", 0, [ handlerType ], ILType.Void),
                None,
                [],
                emptyILCustomAttrs)

        let typeDef =
            mkILSimpleClass
                ilg
                (
                    typeName,
                    ILTypeDefAccess.Public,
                    mkILMethods [ addMethod; removeMethod ],
                    mkILFields [],
                    emptyILTypeDefs,
                    mkILProperties [],
                    mkILEvents [ eventDef ],
                    emptyILCustomAttrs,
                    ILTypeInit.BeforeField )

        mkILSimpleModule
            "SampleAssembly"
            "SampleModule"
            true
            (4, 0)
            false
            (mkILTypeDefs [ typeDef ])
            None
            None
            0
            (mkILExportedTypes [])
            "v4.0.30319"

    let createMethodModule () =
        let ilg = ilGlobals
        let stringType = ilg.typ_String

        let formatBody =
            mkMethodBody(
                false,
                [],
                2,
                nonBranchingInstrsToCode [ I_ldstr "format"; I_ret ],
                None,
                None)

        let methodDef =
            mkILNonGenericStaticMethod(
                "FormatMessage",
                ILMemberAccess.Public,
                [ mkILParamNamed("count", ilg.typ_Int32) ],
                mkILReturn stringType,
                formatBody)

        let typeDef =
            mkILSimpleClass
                ilg
                (
                    "Sample.MethodHost",
                    ILTypeDefAccess.Public,
                    mkILMethods [ methodDef ],
                    mkILFields [],
                    emptyILTypeDefs,
                    mkILProperties [],
                    mkILEvents [],
                    emptyILCustomAttrs,
                    ILTypeInit.BeforeField )

        mkILSimpleModule
            "SampleAssembly"
            "SampleModule"
            true
            (4, 0)
            false
            (mkILTypeDefs [ typeDef ])
            None
            None
            0
            (mkILExportedTypes [])
            "v4.0.30319"

    /// Minimal module with a single parameterless method returning a string literal.
    let createParameterlessMethodModule (messageLiteral: string option) () =
        let ilg = ilGlobals
        let stringType = ilg.typ_String
        let literal = defaultArg messageLiteral "baseline"

        let methodBody =
            mkMethodBody(
                false,
                [],
                2,
                nonBranchingInstrsToCode [ I_ldstr literal; I_ret ],
                None,
                None)

        let methodDef =
            mkILNonGenericStaticMethod(
                "GetMessage",
                ILMemberAccess.Public,
                [],
                mkILReturn stringType,
                methodBody)

        let typeDef =
            mkILSimpleClass
                ilg
                (
                    "Sample.ParamlessHost",
                    ILTypeDefAccess.Public,
                    mkILMethods [ methodDef ],
                    mkILFields [],
                    emptyILTypeDefs,
                    mkILProperties [],
                    mkILEvents [],
                    emptyILCustomAttrs,
                    ILTypeInit.BeforeField )

        mkILSimpleModule
            "SampleAssembly"
            "SampleModule"
            true
            (4, 0)
            false
            (mkILTypeDefs [ typeDef ])
            None
            None
            0
            (mkILExportedTypes [])
            "v4.0.30319"

    let createClosureModule () =
        let ilg = ilGlobals
        let stringType = ilg.typ_String

        let outerBody =
            mkMethodBody(
                false,
                [],
                2,
                nonBranchingInstrsToCode [ I_ldstr "outer"; I_ret ],
                None,
                None)

        let innerBody =
            mkMethodBody(
                false,
                [],
                2,
                nonBranchingInstrsToCode [ I_ldstr "inner"; I_ret ],
                None,
                None)

        let outerMethod =
            mkILNonGenericInstanceMethod(
                "InvokeOuter",
                ILMemberAccess.Public,
                [ mkILParamNamed("value", stringType) ],
                mkILReturn stringType,
                outerBody)

        let innerMethod =
            mkILNonGenericInstanceMethod(
                "Invoke@40-1",
                ILMemberAccess.Public,
                [ mkILParamNamed("value", stringType) ],
                mkILReturn stringType,
                innerBody)

        let typeDef =
            mkILSimpleClass
                ilg
                (
                    "Sample.ClosureHost",
                    ILTypeDefAccess.Public,
                    mkILMethods [ outerMethod; innerMethod ],
                    mkILFields [],
                    emptyILTypeDefs,
                    mkILProperties [],
                    mkILEvents [],
                    emptyILCustomAttrs,
                    ILTypeInit.BeforeField )

        mkILSimpleModule
            "SampleAssembly"
            "SampleModule"
            true
            (4, 0)
            false
            (mkILTypeDefs [ typeDef ])
            None
            None
            0
            (mkILExportedTypes [])
            "v4.0.30319"

    let createAsyncModule (messageLiteral: string option) () =
        let ilg = ilGlobals
        let stringType = ilg.typ_String
        let boolType = ilg.typ_Bool
        let literal = defaultArg messageLiteral "async"

        let stateMachineTypeRef = mkILTyRef(ILScopeRef.Local, "Sample.AsyncHostStateMachine")
        let stateMachineLocalType = ILType.Value(mkILNonGenericTySpec stateMachineTypeRef)

        let runBody =
            mkMethodBody(
                false,
                [ mkILLocal stateMachineLocalType None ],
                2,
                nonBranchingInstrsToCode [ I_ldstr literal; I_ret ],
                None,
                None)

        let asyncStateMachineAttributeRef =
            ILTypeRef.Create(
                ILScopeRef.Assembly mscorlibRef,
                [ "System"; "Runtime"; "CompilerServices" ],
                "AsyncStateMachineAttribute")

        let asyncAttribute =
            mkILCustomAttribute(
                asyncStateMachineAttributeRef,
                [ ilGlobals.typ_Type ],
                [ ILAttribElem.TypeRef(Some stateMachineTypeRef) ],
                [])

        let runMethod =
            mkILNonGenericStaticMethod(
                "RunAsync",
                ILMemberAccess.Public,
                [ mkILParamNamed("token", ilg.typ_Int32) ],
                mkILReturn stringType,
                runBody)
            |> fun m -> m.With(customAttrs = mkILCustomAttrsFromArray [| asyncAttribute |])

        let moveNextBody =
            mkMethodBody(
                false,
                [],
                2,
                nonBranchingInstrsToCode [ AI_ldc(DT_I4, ILConst.I4 1); I_ret ],
                None,
                None)

        let moveNextMethod =
            mkILNonGenericInstanceMethod(
                "MoveNext",
                ILMemberAccess.Public,
                [],
                mkILReturn boolType,
                moveNextBody)

        let hostType =
            mkILSimpleClass
                ilg
                (
                    "Sample.AsyncHost",
                    ILTypeDefAccess.Public,
                    mkILMethods [ runMethod ],
                    mkILFields [],
                    emptyILTypeDefs,
                    mkILProperties [],
                    mkILEvents [],
                    emptyILCustomAttrs,
                    ILTypeInit.BeforeField )

        let stateMachineType =
            mkILSimpleClass
                ilg
                (
                    "Sample.AsyncHostStateMachine",
                    ILTypeDefAccess.Public,
                    mkILMethods [ moveNextMethod ],
                    mkILFields [],
                    emptyILTypeDefs,
                    mkILProperties [],
                    mkILEvents [],
                    emptyILCustomAttrs,
                    ILTypeInit.BeforeField )

        mkILSimpleModule
            "SampleAssembly"
            "SampleModule"
            true
            (4, 0)
            false
            (mkILTypeDefs [ hostType; stateMachineType ])
            None
            None
            0
            (mkILExportedTypes [])
            "v4.0.30319"

    type AddedMethodArtifacts =
        { MethodRow: DeltaWriter.MethodDefinitionRowInfo
          ParameterRows: DeltaWriter.ParameterDefinitionRowInfo list
          Update: DeltaWriter.MethodMetadataUpdate }

    type MetadataDeltaArtifacts =
        { BaselineBytes: byte[]
          BaselineHeapSizes: MetadataHeapSizes
          Delta: DeltaWriter.MetadataDelta }

    type MultiGenerationMetadataArtifacts =
        { BaselineBytes: byte[]
          BaselineHeapSizes: MetadataHeapSizes
          Generation1: DeltaWriter.MetadataDelta
          Generation2: DeltaWriter.MetadataDelta }

    let private tryGetGuidHeap (metadata: byte[]) =
        use ms = new MemoryStream(metadata, false)
        use reader = new BinaryReader(ms, Encoding.UTF8, leaveOpen = true)

        let align4 (v: int) = (v + 3) &&& ~~~3

        try
            let signature = reader.ReadUInt32()
            if signature <> 0x424A5342u then
                None
            else
                reader.ReadUInt16() |> ignore // major
                reader.ReadUInt16() |> ignore // minor
                reader.ReadUInt32() |> ignore // reserved

                let versionLength = reader.ReadUInt32() |> int
                let paddedVersionLength = align4 versionLength
                reader.ReadBytes(paddedVersionLength) |> ignore

                reader.ReadUInt16() |> ignore // flags
                let streamCount = reader.ReadUInt16() |> int

                let mutable guidBytes: byte[] option = None

                for _ = 0 to streamCount - 1 do
                    let offset = reader.ReadUInt32() |> int
                    let size = reader.ReadUInt32() |> int
                    let nameBytes = ResizeArray<byte>()
                    let mutable b = reader.ReadByte()
                    while b <> 0uy do
                        nameBytes.Add b
                        b <- reader.ReadByte()
                    while ms.Position % 4L <> 0L do
                        reader.ReadByte() |> ignore

                    let name = Encoding.UTF8.GetString(nameBytes.ToArray())
                    if name = "#GUID" && offset + size <= metadata.Length then
                        guidBytes <- Some(Array.sub metadata offset size)

                guidBytes
        with _ ->
            None

    let private getModuleGenerationId (metadata: byte[]) (baselineGuidEntries: int) =
        use provider = MetadataReaderProvider.FromMetadataImage(ImmutableArray.CreateRange<byte>(metadata))
        let reader = provider.GetMetadataReader()
        let moduleDef = reader.GetModuleDefinition()
        let handle = moduleDef.GenerationId

        if handle.IsNil then
            System.Guid.Empty
        else
            let rawIndex = (MetadataTokens.GetHeapOffset handle / 16) + 1

            match tryGetGuidHeap metadata with
            | Some heap ->
                printfn "[getModuleGenerationId] rawIndex=%d baselineEntries=%d heapLen=%d" rawIndex baselineGuidEntries heap.Length
                let deltaIndex = rawIndex - baselineGuidEntries
                let offset = (deltaIndex - 1) * 16
                if deltaIndex > 0 && offset >= 0 && offset + 16 <= heap.Length then
                    System.Guid(Array.sub heap offset 16)
                else
                    System.Guid.Empty
            | None ->
                // Fall back to the reader if the heap is present and in range.
                try
                    reader.GetGuid handle
                with _ ->
                    System.Guid.Empty

    let private emitPropertyDeltaCore
        (metadataReader: MetadataReader)
        (builder: IlDeltaStreamBuilder)
        (heapOffsets: MetadataHeapOffsets)
        (generation: int)
        (encBaseId: Guid)
        =
        let stringType = ilGlobals.typ_String

        let typeHandle =
            metadataReader.TypeDefinitions
            |> Seq.find (fun handle -> metadataReader.GetString(metadataReader.GetTypeDefinition(handle).Name) = "PropertyHost")

        let getterHandle = findMethodHandle metadataReader "Sample.PropertyHost" "get_Message"

        let propertyHandle =
            metadataReader.PropertyDefinitions
            |> Seq.find (fun handle -> metadataReader.GetString(metadataReader.GetPropertyDefinition(handle).Name) = "Message")

        let methodKey = methodKey "Sample.PropertyHost" "get_Message" stringType

        let getterDef = metadataReader.GetMethodDefinition getterHandle
        let methodRow: DeltaWriter.MethodDefinitionRowInfo =
            { Key = methodKey
              RowId = 1
              IsAdded = true
              Attributes = getterDef.Attributes
              ImplAttributes = getterDef.ImplAttributes
              Name = metadataReader.GetString getterDef.Name
              NameOffset = None
              Signature = metadataReader.GetBlobBytes getterDef.Signature
              SignatureOffset = None
              FirstParameterRowId = None
              CodeRva = None }
        let methodDefinitionRows = [ methodRow ]

        let updates: DeltaWriter.MethodMetadataUpdate list =
            [ { MethodKey = methodKey
                MethodToken = MetadataTokens.GetToken(EntityHandle.op_Implicit getterHandle)
                MethodHandle = toMethodDefHandle getterHandle
                Body =
                    { MethodToken = MetadataTokens.GetToken(EntityHandle.op_Implicit getterHandle)
                      LocalSignatureToken = 0
                      CodeOffset = 0
                      CodeLength = 1 } } ]

        let propertyKey : PropertyDefinitionKey =
            { DeclaringType = "Sample.PropertyHost"
              Name = "Message"
              PropertyType = stringType
              IndexParameterTypes = [] }

        let propertyDef = metadataReader.GetPropertyDefinition propertyHandle
        let propertyRows: DeltaWriter.PropertyDefinitionRowInfo list =
            [ { Key = propertyKey
                RowId = 1
                IsAdded = true
                Name = metadataReader.GetString propertyDef.Name
                NameOffset = None
                Signature = metadataReader.GetBlobBytes propertyDef.Signature
                SignatureOffset = None
                Attributes = propertyDef.Attributes } ]

        let propertyMapRows: DeltaWriter.PropertyMapRowInfo list =
            [ { DeclaringType = "Sample.PropertyHost"
                RowId = 1
                TypeDefRowId = MetadataTokens.GetRowNumber typeHandle
                FirstPropertyRowId = Some 1
                IsAdded = true } ]

        let moduleDef = metadataReader.GetModuleDefinition()
        let moduleName = metadataReader.GetString(moduleDef.Name)
        let moduleGuid = metadataReader.GetGuid(moduleDef.Mvid)

        DeltaWriter.emit
            moduleName
            None
            generation
            (System.Guid.NewGuid())
            encBaseId
            moduleGuid
            methodDefinitionRows
            []
            propertyRows
            []
            propertyMapRows
            []
            []
            builder.StandaloneSignatures
            []
            updates
            heapOffsets
            (getRowCounts metadataReader)

    let private emitPropertyDeltaFromBaseline (baselineBytes: byte[]) (heapOffsets: MetadataHeapOffsets) (generation: int) (encBaseId: Guid) =
        use peReader = new PEReader(new MemoryStream(baselineBytes, false))
        let metadataReader = peReader.GetMetadataReader()
        let metadataSnapshot = metadataSnapshotFromBytes baselineBytes |> Option.get
        let builder = IlDeltaStreamBuilder(Some metadataSnapshot)
        printfn "[property-delta] generation=%d encBaseId=%A" generation encBaseId
        emitPropertyDeltaCore metadataReader builder heapOffsets generation encBaseId

    let emitPropertyDeltaArtifacts (messageLiteral: string option) () : MetadataDeltaArtifacts =
        let moduleDef = createPropertyModule messageLiteral ()
        let assemblyBytes, _, _, _ = createAssemblyBytes moduleDef
        use peReader = new PEReader(new MemoryStream(assemblyBytes, false))
        let metadataReader = peReader.GetMetadataReader()
        let baselineHeapSizes = getHeapSizes metadataReader
        let builder = IlDeltaStreamBuilder None
        let heapOffsets = computeHeapOffsets metadataReader
        printfn "[property-delta] baseline guid heap size = %d" baselineHeapSizes.GuidHeapSize
        let metadataDelta = emitPropertyDeltaCore metadataReader builder heapOffsets 1 System.Guid.Empty

        inspectDeltaMetadata "delta" metadataDelta.Metadata

        if shouldTraceMetadata () then
            // Note: SRM MetadataBuilder comparison removed after SRM removal from IlDeltaStreamBuilder
            dumpMetadataLayout "delta-custom" metadataDelta.Metadata
            printfn "[hotreload-metadata] delta-custom total-bytes=%d" metadataDelta.Metadata.Length
            let dumpDir = Path.Combine(Path.GetTempPath(), "fsharp-hotreload-md-dumps")
            Directory.CreateDirectory(dumpDir) |> ignore
            File.WriteAllBytes(Path.Combine(dumpDir, "delta-custom.bin"), metadataDelta.Metadata)
            File.WriteAllBytes(Path.Combine(dumpDir, "delta-custom-table.bin"), metadataDelta.TableStream.Bytes)
            let logRowCounts label (counts: int[]) =
                counts
                |> Array.mapi (fun idx count -> idx, count)
                |> Array.filter (fun (_, count) -> count <> 0)
                |> Array.iter (fun (idx, count) ->
                    let table = LanguagePrimitives.EnumOfValue<byte, TableIndex>(byte idx)
                    printfn "[hotreload-metadata] %s row-count %-15A = %d" label table count)

            logRowCounts "delta-custom" metadataDelta.TableRowCounts
            printfn
                "[hotreload-metadata] delta-custom heap sizes strings=%d blobs=%d guids=%d"
                metadataDelta.HeapSizes.StringHeapSize
                metadataDelta.HeapSizes.BlobHeapSize
                metadataDelta.HeapSizes.GuidHeapSize

        { BaselineBytes = assemblyBytes
          BaselineHeapSizes = baselineHeapSizes
          Delta = metadataDelta }

    let private emitLocalSignatureDeltaCore
        (metadataReader: MetadataReader)
        (peReader: PEReader)
        (builder: IlDeltaStreamBuilder)
        (heapOffsets: MetadataHeapOffsets)
        =
        let stringType = ilGlobals.typ_String
        let typeName = "Sample.LocalSignatureHost"
        let methodName = "FormatMessage"

        let methodHandle = findMethodHandle metadataReader "Sample.LocalSignatureHost" methodName
        let methodDef = metadataReader.GetMethodDefinition methodHandle
        let methodBody = peReader.GetMethodBody methodDef.RelativeVirtualAddress

        let localSignatureToken =
            if methodBody.LocalSignature.IsNil then
                0
            else
                let standalone = metadataReader.GetStandaloneSignature methodBody.LocalSignature
                let signatureBytes = metadataReader.GetBlobBytes standalone.Signature
                builder.AddStandaloneSignature(signatureBytes)

        let methodKey = methodKey typeName methodName stringType

        let methodRow: DeltaWriter.MethodDefinitionRowInfo =
            { Key = methodKey
              RowId = 1
              IsAdded = true
              Attributes = methodDef.Attributes
              ImplAttributes = methodDef.ImplAttributes
              Name = metadataReader.GetString methodDef.Name
              NameOffset = None
              Signature = metadataReader.GetBlobBytes methodDef.Signature
              SignatureOffset = None
              FirstParameterRowId = None
              CodeRva = None }
        let methodRows = [ methodRow ]

        let updates: DeltaWriter.MethodMetadataUpdate list =
            [ { MethodKey = methodKey
                MethodToken = MetadataTokens.GetToken(EntityHandle.op_Implicit methodHandle)
                MethodHandle = toMethodDefHandle methodHandle
                Body =
                    { MethodToken = MetadataTokens.GetToken(EntityHandle.op_Implicit methodHandle)
                      LocalSignatureToken = localSignatureToken
                      CodeOffset = 0
                      CodeLength = 1 } } ]

        let moduleDef = metadataReader.GetModuleDefinition()
        let moduleName = metadataReader.GetString moduleDef.Name
        let moduleGuid = metadataReader.GetGuid moduleDef.Mvid

        DeltaWriter.emit
            moduleName
            None
            1
            (System.Guid.NewGuid())
            System.Guid.Empty
            moduleGuid
            methodRows
            [] // parameter rows
            [] // property rows
            [] // event rows
            [] // property map rows
            [] // event map rows
            [] // method semantics rows
            builder.StandaloneSignatures
            []
            updates
            heapOffsets
            (getRowCounts metadataReader)

    let emitLocalSignatureDeltaArtifacts (messageLiteral: string option) () : MetadataDeltaArtifacts =
        let moduleDef = createLocalSignatureModule messageLiteral ()
        let assemblyBytes, _, _, _ = createAssemblyBytes moduleDef
        use peReader = new PEReader(new MemoryStream(assemblyBytes, false))
        let metadataReader = peReader.GetMetadataReader()
        let baselineHeapSizes = getHeapSizes metadataReader
        let builder = IlDeltaStreamBuilder None
        let heapOffsets = computeHeapOffsets metadataReader
        let metadataDelta = emitLocalSignatureDeltaCore metadataReader peReader builder heapOffsets

        { BaselineBytes = assemblyBytes
          BaselineHeapSizes = baselineHeapSizes
          Delta = metadataDelta }

    let private emitLocalSignatureDeltaFromBaseline (baselineBytes: byte[]) (heapOffsets: MetadataHeapOffsets) =
        use peReader = new PEReader(new MemoryStream(baselineBytes, false))
        let metadataReader = peReader.GetMetadataReader()
        let builder = IlDeltaStreamBuilder None
        emitLocalSignatureDeltaCore metadataReader peReader builder heapOffsets

    let emitLocalSignatureMultiGenerationArtifacts () : MultiGenerationMetadataArtifacts =
        let generation1 = emitLocalSignatureDeltaArtifacts None ()

        let nextOffsets =
            use peReader = new PEReader(new MemoryStream(generation1.BaselineBytes, false))
            let metadataReader = peReader.GetMetadataReader()
            let baseOffsets = computeHeapOffsets metadataReader
            advanceHeapOffsets baseOffsets generation1.Delta

        let generation2 = emitLocalSignatureDeltaFromBaseline generation1.BaselineBytes nextOffsets

        { BaselineBytes = generation1.BaselineBytes
          BaselineHeapSizes = generation1.BaselineHeapSizes
          Generation1 = generation1.Delta
          Generation2 = generation2 }

    let private emitAsyncDeltaCore
        (metadataReader: MetadataReader)
        (peReader: PEReader)
        (builder: IlDeltaStreamBuilder)
        (heapOffsets: MetadataHeapOffsets)
        : DeltaWriter.MetadataDelta =
        let methodHandle = findMethodHandle metadataReader "Sample.AsyncHost" "RunAsync"

        let methodKey =
            methodKeyWithParameters "Sample.AsyncHost" "RunAsync" [ ilGlobals.typ_Int32 ] ilGlobals.typ_String

        let methodDef = metadataReader.GetMethodDefinition methodHandle

        if shouldTraceMetadata () then
            metadataReader.CustomAttributes
            |> Seq.iter (fun handle ->
                let attribute = metadataReader.GetCustomAttribute handle
                let parentToken = MetadataTokens.GetToken attribute.Parent
                let ctorToken = MetadataTokens.GetToken attribute.Constructor
                printfn
                    "[hotreload-metadata] custom attribute parent=%A parentToken=0x%08X ctor=%A ctorToken=0x%08X"
                    attribute.Parent.Kind
                    parentToken
                    attribute.Constructor.Kind
                    ctorToken)

        let methodBody = peReader.GetMethodBody methodDef.RelativeVirtualAddress

        let localSignatureToken =
            if methodBody.LocalSignature.IsNil then
                0
            else
                let standalone = metadataReader.GetStandaloneSignature methodBody.LocalSignature
                let signatureBytes = metadataReader.GetBlobBytes standalone.Signature
                builder.AddStandaloneSignature(signatureBytes)

        let methodRow : DeltaWriter.MethodDefinitionRowInfo =
            { Key = methodKey
              RowId = 1
              IsAdded = false
              Attributes = methodDef.Attributes
              ImplAttributes = methodDef.ImplAttributes
              Name = metadataReader.GetString methodDef.Name
              NameOffset = None
              Signature = metadataReader.GetBlobBytes methodDef.Signature
              SignatureOffset = None
              FirstParameterRowId = None
              CodeRva = None }
        let methodDefinitionRows = [ methodRow ]

        let updates: DeltaWriter.MethodMetadataUpdate list =
            [ { MethodKey = methodKey
                MethodToken = MetadataTokens.GetToken(EntityHandle.op_Implicit methodHandle)
                MethodHandle = toMethodDefHandle methodHandle
                Body =
                    { MethodToken = MetadataTokens.GetToken(EntityHandle.op_Implicit methodHandle)
                      LocalSignatureToken = localSignatureToken
                      CodeOffset = 0
                      CodeLength = 4 } } ]

        let assemblyReferenceRows = ResizeArray<AssemblyReferenceRowInfo>()
        let typeReferenceRows = ResizeArray<TypeReferenceRowInfo>()
        let memberReferenceRows = ResizeArray<MemberReferenceRowInfo>()
        let assemblyRefMap = Dictionary<AssemblyReferenceHandle, int>()
        let typeRefMap = Dictionary<TypeReferenceHandle, int>()
        let memberRefMap = Dictionary<MemberReferenceHandle, int>()

        let getBlobBytes (handle: BlobHandle) =
            if handle.IsNil then
                Array.empty
            else
                metadataReader.GetBlobBytes handle

        let rec addAssemblyReference (handle: AssemblyReferenceHandle) =
            match assemblyRefMap.TryGetValue handle with
            | true, rowId -> rowId
            | _ ->
                let rowId = assemblyReferenceRows.Count + 1
                let row = metadataReader.GetAssemblyReference handle
                assemblyReferenceRows.Add(
                    { RowId = rowId
                      Version = row.Version
                      Flags = row.Flags
                      PublicKeyOrToken = getBlobBytes row.PublicKeyOrToken
                      PublicKeyOrTokenOffset = None
                      Name = metadataReader.GetString row.Name
                      NameOffset = None
                      Culture =
                        if row.Culture.IsNil then
                            None
                        else
                            metadataReader.GetString row.Culture |> Some
                      CultureOffset = None
                      HashValue = getBlobBytes row.HashValue
                      HashValueOffset = None })
                assemblyRefMap[handle] <- rowId
                rowId

        let buildTypeReferenceInfo (handle: TypeReferenceHandle) =
            let rec loop current segments =
                let row = metadataReader.GetTypeReference current
                let updated = metadataReader.GetString row.Name :: segments
                if row.ResolutionScope.Kind = HandleKind.TypeReference then
                    loop (TypeReferenceHandle.op_Explicit row.ResolutionScope) updated
                else
                    row.ResolutionScope, updated, row
            loop handle []

        let rec addTypeReference (handle: TypeReferenceHandle) =
            match typeRefMap.TryGetValue handle with
            | true, rowId -> rowId
            | _ ->
                let resolutionScopeHandle, segments, innermostRow = buildTypeReferenceInfo handle
                let segmentsRev = List.rev segments
                let typeName = segmentsRev |> List.last
                let namespaceSegments =
                    segmentsRev
                    |> List.take (segmentsRev.Length - 1)
                let namespaceName =
                    if List.isEmpty namespaceSegments then
                        ""
                    else
                        String.Join(".", namespaceSegments)

                let resolutionScope =
                    match resolutionScopeHandle.Kind with
                    | HandleKind.AssemblyReference ->
                        let parent =
                            addAssemblyReference(AssemblyReferenceHandle.op_Explicit resolutionScopeHandle)
                        RS_AssemblyRef(AssemblyRefHandle parent)
                    | HandleKind.ModuleDefinition ->
                        let parent = MetadataTokens.GetRowNumber resolutionScopeHandle
                        RS_Module(ModuleHandle parent)
                    | HandleKind.ModuleReference ->
                        let parent = MetadataTokens.GetRowNumber resolutionScopeHandle
                        RS_ModuleRef(ModuleRefHandle parent)
                    | _ -> RS_Module(ModuleHandle 1)

                let rowId = typeReferenceRows.Count + 1
                if shouldTraceMetadata () then
                    printfn "[hotreload-metadata] add TypeRef rowId=%d name=%s scope=%A" rowId typeName resolutionScope

                typeReferenceRows.Add(
                    { RowId = rowId
                      ResolutionScope = resolutionScope
                      Name = typeName
                      NameOffset = None
                      Namespace = namespaceName
                      NamespaceOffset = None })
                typeRefMap[handle] <- rowId
                rowId

        let addMemberReference (handle: MemberReferenceHandle) =
            match memberRefMap.TryGetValue handle with
            | true, rowId -> rowId
            | _ ->
                let row = metadataReader.GetMemberReference handle
                let parent =
                    match row.Parent.Kind with
                    | HandleKind.TypeReference ->
                        let parentRow = addTypeReference(TypeReferenceHandle.op_Explicit row.Parent)
                        MRP_TypeRef(TypeRefHandle parentRow)
                    | HandleKind.TypeDefinition ->
                        let parentRow = MetadataTokens.GetRowNumber row.Parent
                        MRP_TypeDef(TypeDefHandle parentRow)
                    | HandleKind.ModuleReference ->
                        let parentRow = MetadataTokens.GetRowNumber row.Parent
                        MRP_ModuleRef(ModuleRefHandle parentRow)
                    | HandleKind.MethodDefinition ->
                        let parentRow = MetadataTokens.GetRowNumber row.Parent
                        MRP_MethodDef(MethodDefHandle parentRow)
                    | HandleKind.TypeSpecification ->
                        let parentRow = MetadataTokens.GetRowNumber row.Parent
                        MRP_TypeSpec(TypeSpecHandle parentRow)
                    | _ -> MRP_TypeRef(TypeRefHandle 0)

                let rowId = memberReferenceRows.Count + 1
                memberReferenceRows.Add(
                    { RowId = rowId
                      Parent = parent
                      Name = metadataReader.GetString row.Name
                      NameOffset = None
                      Signature = getBlobBytes row.Signature
                      SignatureOffset = None })
                memberRefMap[handle] <- rowId
                rowId

        let isAsyncStateMachineAttribute (attribute: CustomAttribute) =
            match attribute.Constructor.Kind with
            | HandleKind.MemberReference ->
                let memberRef = metadataReader.GetMemberReference(MemberReferenceHandle.op_Explicit attribute.Constructor)
                match memberRef.Parent.Kind with
                | HandleKind.TypeReference ->
                    let typeRef = metadataReader.GetTypeReference(TypeReferenceHandle.op_Explicit memberRef.Parent)
                    let name = metadataReader.GetString typeRef.Name
                    let ns =
                        if typeRef.Namespace.IsNil then
                            ""
                        else
                            metadataReader.GetString typeRef.Namespace
                    if shouldTraceMetadata () then
                        printfn "[hotreload-metadata] attribute type parentKind=%A ns=%s name=%s" memberRef.Parent.Kind ns name
                    name.EndsWith("StateMachineAttribute", StringComparison.OrdinalIgnoreCase)
                | kind ->
                    if shouldTraceMetadata () then
                        printfn "[hotreload-metadata] attribute parent kind=%A not handled" kind
                    false
            | _ -> false

        let customAttributeRows : CustomAttributeRowInfo list =
            let tryFindAsyncAttribute () =
                metadataReader.CustomAttributes
                |> Seq.tryFind (fun handle ->
                    let attribute = metadataReader.GetCustomAttribute handle
                    match attribute.Parent.Kind with
                    | HandleKind.MethodDefinition ->
                        let parentToken = MetadataTokens.GetToken attribute.Parent
                        let methodToken = MetadataTokens.GetToken(EntityHandle.op_Implicit methodHandle)

                        if shouldTraceMetadata () then
                            printfn
                                "[hotreload-metadata] async attribute candidate parent=0x%08X target=0x%08X match=%b"
                                parentToken
                                methodToken
                                (parentToken = methodToken)

                        parentToken = methodToken
                        && isAsyncStateMachineAttribute attribute
                    | _ -> false)

            let attributeOpt = tryFindAsyncAttribute ()

            if shouldTraceMetadata () then
                printfn "[hotreload-metadata] async attribute found=%b" (attributeOpt.IsSome)

            match attributeOpt with
            | Some attributeHandle ->
                let attribute = metadataReader.GetCustomAttribute attributeHandle

                let constructor : CustomAttributeType =
                    match attribute.Constructor.Kind with
                    | HandleKind.MemberReference ->
                        let rowId =
                            addMemberReference(MemberReferenceHandle.op_Explicit attribute.Constructor)
                        CAT_MemberRef(MemberRefHandle rowId)
                    | HandleKind.MethodDefinition ->
                        let rowId = MetadataTokens.GetRowNumber attribute.Constructor
                        CAT_MethodDef(MethodDefHandle rowId)
                    | _ ->
                        let rowId = MetadataTokens.GetRowNumber attribute.Constructor
                        CAT_MethodDef(MethodDefHandle rowId)

                let valueBytes =
                    if attribute.Value.IsNil then
                        Array.empty<byte>
                    else
                        metadataReader.GetBlobBytes attribute.Value

                [ { RowId = 1
                    Parent = HCA_MethodDef(MethodDefHandle 1)
                    Constructor = constructor
                    Value = valueBytes
                    ValueOffset = None } ]
            | None -> []

        // Include IAsyncStateMachine references to align with Roslyn parity expectations.
        let tryFindAssemblyReferenceByName name =
            metadataReader.AssemblyReferences
            |> Seq.tryFind (fun handle ->
                let row = metadataReader.GetAssemblyReference handle
                metadataReader.GetString row.Name = name)

        metadataReader.TypeReferences
        |> Seq.tryFind (fun handle ->
            let _, segments, _ = buildTypeReferenceInfo handle
            let segmentsRev = List.rev segments
            match segmentsRev with
            | [] -> false
            | name :: namespaceParts ->
                let namespaceName = String.Join(".", namespaceParts)
                namespaceName = "System.Runtime.CompilerServices" && name = "IAsyncStateMachine")
        |> function
            | Some handle -> addTypeReference handle |> ignore
            | None ->
                match tryFindAssemblyReferenceByName "mscorlib" with
                | Some asmHandle ->
                    let asmRowId = addAssemblyReference asmHandle
                    let rowId = typeReferenceRows.Count + 1
                    typeReferenceRows.Add(
                        { RowId = rowId
                          ResolutionScope = RS_AssemblyRef(AssemblyRefHandle asmRowId)
                          Name = "IAsyncStateMachine"
                          NameOffset = None
                          Namespace = "System.Runtime.CompilerServices"
                          NamespaceOffset = None })
                | None -> ()

        let moduleName = metadataReader.GetString(metadataReader.GetModuleDefinition().Name)

        let metadataDelta =
            DeltaWriter.emitWithReferences
                moduleName
                None
                1
                (System.Guid.NewGuid())
                (System.Guid.NewGuid())
                (System.Guid.NewGuid())
                methodDefinitionRows
                [] // parameter rows
                (typeReferenceRows |> Seq.toList)
                (memberReferenceRows |> Seq.toList)
                [] // method spec rows
                (assemblyReferenceRows |> Seq.toList)
                [] // property rows
                [] // event rows
                [] // property map rows
                [] // event map rows
                [] // method semantics rows
                builder.StandaloneSignatures
                customAttributeRows
                []
                updates
                heapOffsets
                (getRowCounts metadataReader)

        if shouldTraceMetadata () then
            printfn
                "[hotreload-metadata] async table counts typeRef=%d memberRef=%d assemblyRef=%d customAttr=%d"
                metadataDelta.TableRowCounts.[int TableIndex.TypeRef]
                metadataDelta.TableRowCounts.[int TableIndex.MemberRef]
                metadataDelta.TableRowCounts.[int TableIndex.AssemblyRef]
                metadataDelta.TableRowCounts.[int TableIndex.CustomAttribute]

        metadataDelta

    let emitAsyncDeltaArtifacts (messageLiteral: string option) () : MetadataDeltaArtifacts =
        let moduleDef = createAsyncModule messageLiteral ()
        let assemblyBytes, _, _, _ = createAssemblyBytes moduleDef
        use peReader = new PEReader(new MemoryStream(assemblyBytes, false))
        let metadataReader = peReader.GetMetadataReader()
        let baselineHeapSizes = getHeapSizes metadataReader
        // Use baseline metadata so row IDs continue from baseline counts (Roslyn parity)
        let metadataSnapshot = metadataSnapshotFromBytes assemblyBytes |> Option.get
        let builder = IlDeltaStreamBuilder(Some metadataSnapshot)
        let heapOffsets = computeHeapOffsets metadataReader
        let metadataDelta = emitAsyncDeltaCore metadataReader peReader builder heapOffsets

        assertTableStreamMatches metadataDelta

        { BaselineBytes = assemblyBytes
          BaselineHeapSizes = baselineHeapSizes
          Delta = metadataDelta }

    let private emitAsyncDeltaFromBaseline (baselineBytes: byte[]) (heapOffsets: MetadataHeapOffsets) =
        use peReader = new PEReader(new MemoryStream(baselineBytes, false))
        let metadataReader = peReader.GetMetadataReader()
        let metadataSnapshot = metadataSnapshotFromBytes baselineBytes |> Option.get
        let builder = IlDeltaStreamBuilder(Some metadataSnapshot)
        emitAsyncDeltaCore metadataReader peReader builder heapOffsets

    let emitAsyncMultiGenerationArtifacts () : MultiGenerationMetadataArtifacts =
        let generation1 = emitAsyncDeltaArtifacts None ()

        let nextOffsets =
            use peReader = new PEReader(new MemoryStream(generation1.BaselineBytes, false))
            let metadataReader = peReader.GetMetadataReader()
            let baseOffsets = computeHeapOffsets metadataReader
            advanceHeapOffsets baseOffsets generation1.Delta

        let generation2 = emitAsyncDeltaFromBaseline generation1.BaselineBytes nextOffsets

        { BaselineBytes = generation1.BaselineBytes
          BaselineHeapSizes = generation1.BaselineHeapSizes
          Generation1 = generation1.Delta
          Generation2 = generation2 }

    let emitPropertyMultiGenerationArtifacts () : MultiGenerationMetadataArtifacts =
        let generation1 = emitPropertyDeltaArtifacts None ()

        let nextOffsets =
            use peReader = new PEReader(new MemoryStream(generation1.BaselineBytes, false))
            let metadataReader = peReader.GetMetadataReader()
            let baseOffsets = computeHeapOffsets metadataReader
            advanceHeapOffsets baseOffsets generation1.Delta

        // Use GenerationId field from MetadataDelta directly, rather than trying to extract
        // from delta metadata bytes (which MetadataReader can't properly interpret)
        let gen1EncId = generation1.Delta.GenerationId
        printfn "[property-multigen] gen1 EncId = %A" gen1EncId
        let generation2 = emitPropertyDeltaFromBaseline generation1.BaselineBytes nextOffsets 2 gen1EncId

        // Use the GenerationId and BaseGenerationId fields directly from the delta
        let encId2 = generation2.GenerationId
        let baseId = generation2.BaseGenerationId

        printfn "[property-multigen] gen2 EncId = %A BaseId = %A" encId2 baseId

        { BaselineBytes = generation1.BaselineBytes
          BaselineHeapSizes = generation1.BaselineHeapSizes
          Generation1 = generation1.Delta
          Generation2 = generation2 }

    let private emitEventDeltaCore
        (metadataReader: MetadataReader)
        (builder: IlDeltaStreamBuilder)
        (heapOffsets: MetadataHeapOffsets)
        =
        let addHandle = findMethodHandle metadataReader "Sample.EventHost" "add_OnChanged"
        let methodKey = methodKey "Sample.EventHost" "add_OnChanged" ILType.Void
        let addDef = metadataReader.GetMethodDefinition addHandle

        let parameterRows: DeltaWriter.ParameterDefinitionRowInfo list =
            addDef.GetParameters()
            |> Seq.choose (fun parameterHandle ->
                if parameterHandle.IsNil then
                    None
                else
                    let parameter = metadataReader.GetParameter parameterHandle
                    let key: ParameterDefinitionKey =
                        { ParameterDefinitionKey.Method = methodKey
                          SequenceNumber = int parameter.SequenceNumber }
                    let row: DeltaWriter.ParameterDefinitionRowInfo =
                        { Key = key
                          RowId = MetadataTokens.GetRowNumber parameterHandle
                          IsAdded = true
                          Attributes = parameter.Attributes
                          SequenceNumber = int parameter.SequenceNumber
                          Name =
                            if parameter.Name.IsNil then
                                None
                            else
                                Some(metadataReader.GetString parameter.Name)
                          NameOffset = None }
                    Some row)
            |> Seq.toList

        let firstParamRowId = parameterRows |> List.tryHead |> Option.map (fun row -> row.RowId)

        let methodRow : DeltaWriter.MethodDefinitionRowInfo =
            { Key = methodKey
              RowId = 1
              IsAdded = true
              Attributes = addDef.Attributes
              ImplAttributes = addDef.ImplAttributes
              Name = metadataReader.GetString addDef.Name
              NameOffset = None
              Signature = metadataReader.GetBlobBytes addDef.Signature
              SignatureOffset = None
              FirstParameterRowId = firstParamRowId
              CodeRva = None }
        let methodDefinitionRows = [ methodRow ]

        let updates: DeltaWriter.MethodMetadataUpdate list =
            [ { MethodKey = methodKey
                MethodToken = MetadataTokens.GetToken(EntityHandle.op_Implicit addHandle)
                MethodHandle = toMethodDefHandle addHandle
                Body =
                    { MethodToken = MetadataTokens.GetToken(EntityHandle.op_Implicit addHandle)
                      LocalSignatureToken = 0
                      CodeOffset = 0
                      CodeLength = 1 } } ]

        let eventKey : EventDefinitionKey =
            { DeclaringType = "Sample.EventHost"
              Name = "OnChanged"
              EventType = Some ilGlobals.typ_Object }

        let eventHandle =
            metadataReader.EventDefinitions
            |> Seq.find (fun handle -> metadataReader.GetString(metadataReader.GetEventDefinition(handle).Name) = "OnChanged")

        let eventDef = metadataReader.GetEventDefinition eventHandle
        // Convert SRM EntityHandle to our TypeDefOrRef DU
        let eventTypeHandle = eventDef.Type
        let eventType =
            match eventTypeHandle.Kind with
            | HandleKind.TypeReference -> TDR_TypeRef(TypeRefHandle(MetadataTokens.GetRowNumber eventTypeHandle))
            | HandleKind.TypeDefinition -> TDR_TypeDef(TypeDefHandle(MetadataTokens.GetRowNumber eventTypeHandle))
            | HandleKind.TypeSpecification -> TDR_TypeSpec(TypeSpecHandle(MetadataTokens.GetRowNumber eventTypeHandle))
            | _ -> failwith $"Unexpected EventType handle kind: {eventTypeHandle.Kind}"

        let eventRows: DeltaWriter.EventDefinitionRowInfo list =
            [ { Key = eventKey
                RowId = 1
                IsAdded = true
                Name = metadataReader.GetString eventDef.Name
                NameOffset = None
                Attributes = eventDef.Attributes
                EventType = eventType } ]

        let eventMapRows: DeltaWriter.EventMapRowInfo list =
            [ { DeclaringType = "Sample.EventHost"
                RowId = 1
                TypeDefRowId =
                    metadataReader.TypeDefinitions
                    |> Seq.find (fun handle -> metadataReader.GetString(metadataReader.GetTypeDefinition(handle).Name) = "EventHost")
                    |> MetadataTokens.GetRowNumber
                FirstEventRowId = Some 1
                IsAdded = true } ]

        let moduleName = metadataReader.GetString(metadataReader.GetModuleDefinition().Name)

        let methodSemanticsRows: DeltaWriter.MethodSemanticsMetadataUpdate list =
            [ { RowId = 1
                MethodToken = MetadataTokens.GetToken(EntityHandle.op_Implicit addHandle)
                Attributes = MethodSemanticsAttributes.Adder
                IsAdded = true
                AssociationInfo = MethodSemanticsAssociation.EventAssociation(eventKey, 1) } ]

        DeltaWriter.emit
            moduleName
            None
            1
            (System.Guid.NewGuid())
            (System.Guid.NewGuid())
            (System.Guid.NewGuid())
            methodDefinitionRows
            parameterRows
            []
            eventRows
            []
            eventMapRows
            methodSemanticsRows
            builder.StandaloneSignatures
            []
            updates
            heapOffsets
            (getRowCounts metadataReader)

    let private emitEventDeltaFromBaseline (baselineBytes: byte[]) (heapOffsets: MetadataHeapOffsets) =
        use peReader = new PEReader(new MemoryStream(baselineBytes, false))
        let metadataReader = peReader.GetMetadataReader()
        let builder = IlDeltaStreamBuilder None
        emitEventDeltaCore metadataReader builder heapOffsets

    let emitEventDeltaArtifacts (messageLiteral: string option) () : MetadataDeltaArtifacts =
        let moduleDef = createEventModule messageLiteral ()
        let assemblyBytes, _, _, _ = createAssemblyBytes moduleDef
        use peReader = new PEReader(new MemoryStream(assemblyBytes, false))
        let metadataReader = peReader.GetMetadataReader()
        let baselineHeapSizes = getHeapSizes metadataReader
        let builder = IlDeltaStreamBuilder None
        let heapOffsets = computeHeapOffsets metadataReader
        let metadataDelta = emitEventDeltaCore metadataReader builder heapOffsets

        { BaselineBytes = assemblyBytes
          BaselineHeapSizes = baselineHeapSizes
          Delta = metadataDelta }

    let emitEventMultiGenerationArtifacts () : MultiGenerationMetadataArtifacts =
        let generation1 = emitEventDeltaArtifacts None ()

        let nextOffsets =
            use peReader = new PEReader(new MemoryStream(generation1.BaselineBytes, false))
            let metadataReader = peReader.GetMetadataReader()
            let baseOffsets = computeHeapOffsets metadataReader
            advanceHeapOffsets baseOffsets generation1.Delta

        let generation2 = emitEventDeltaFromBaseline generation1.BaselineBytes nextOffsets

        { BaselineBytes = generation1.BaselineBytes
          BaselineHeapSizes = generation1.BaselineHeapSizes
          Generation1 = generation1.Delta
          Generation2 = generation2 }

    let buildAddedMethod
        (metadataReader: MetadataReader)
        (nextMethodRowId: int ref)
        (nextParamRowId: int ref)
        (typeName: string)
        (methodName: string)
        (parameterTypes: ILType list)
        (returnType: ILType)
        =
        let methodHandle = findMethodHandle metadataReader typeName methodName
        let methodDef = metadataReader.GetMethodDefinition methodHandle

        let methodKey =
            { DeclaringType = typeName
              Name = methodName
              GenericArity = 0
              ParameterTypes = parameterTypes
              ReturnType = returnType }

        let methodRowId = !nextMethodRowId
        incr nextMethodRowId

        let parameterRows : DeltaWriter.ParameterDefinitionRowInfo list =
            methodDef.GetParameters()
            |> Seq.map metadataReader.GetParameter
            |> Seq.filter (fun paramDef -> paramDef.SequenceNumber <> 0)
            |> Seq.map (fun paramDef ->
                let rowId = !nextParamRowId
                incr nextParamRowId
                let row : DeltaWriter.ParameterDefinitionRowInfo =
                    { Key =
                        { Method = methodKey
                          SequenceNumber = paramDef.SequenceNumber }
                      RowId = rowId
                      IsAdded = true
                      Attributes = paramDef.Attributes
                      SequenceNumber = paramDef.SequenceNumber
                      Name =
                          if paramDef.Name.IsNil then
                              None
                          else
                              Some(metadataReader.GetString paramDef.Name)
                      NameOffset = None }
                row)
            |> Seq.toList

        let firstParamRowId = parameterRows |> List.tryHead |> Option.map (fun row -> row.RowId)

        let methodRow : DeltaWriter.MethodDefinitionRowInfo =
            { Key = methodKey
              RowId = methodRowId
              IsAdded = true
              Attributes = methodDef.Attributes
              ImplAttributes = methodDef.ImplAttributes
              Name = metadataReader.GetString methodDef.Name
              NameOffset = None
              Signature = metadataReader.GetBlobBytes methodDef.Signature
              SignatureOffset = None
              FirstParameterRowId = firstParamRowId
              CodeRva = None }

        let methodToken = MetadataTokens.GetToken(EntityHandle.op_Implicit methodHandle)

        let update : DeltaWriter.MethodMetadataUpdate =
            { MethodKey = methodKey
              MethodToken = methodToken
              MethodHandle = toMethodDefHandle methodHandle
              Body =
                { MethodToken = methodToken
                  LocalSignatureToken = 0
                  CodeOffset = 0
                  CodeLength = 4 } }

        { MethodRow = methodRow
          ParameterRows = parameterRows
          Update = update }

    let private emitClosureDeltaCore
        (metadataReader: MetadataReader)
        (builder: IlDeltaStreamBuilder)
        (heapOffsets: MetadataHeapOffsets)
        : DeltaWriter.MetadataDelta =
        let moduleName = metadataReader.GetString(metadataReader.GetModuleDefinition().Name)
        let stringType = ilGlobals.typ_String

        let nextMethodRowId = ref 1
        let nextParamRowId = ref 1

        let artifacts : AddedMethodArtifacts list =
            [ buildAddedMethod metadataReader nextMethodRowId nextParamRowId "Sample.ClosureHost" "InvokeOuter" [ stringType ] stringType
              buildAddedMethod metadataReader nextMethodRowId nextParamRowId "Sample.ClosureHost" "Invoke@40-1" [ stringType ] stringType ]

        let methodRows = artifacts |> List.map (fun a -> a.MethodRow)
        let parameterRows = artifacts |> List.collect (fun a -> a.ParameterRows)
        let updates = artifacts |> List.map (fun a -> a.Update)

        DeltaWriter.emit
            moduleName
            None
            1
            (System.Guid.NewGuid())
            (System.Guid.NewGuid())
            (System.Guid.NewGuid())
            methodRows
            parameterRows
            []
            []
            []
            []
            []
            builder.StandaloneSignatures
            []
            updates
            heapOffsets
            (getRowCounts metadataReader)

    let emitClosureDeltaArtifacts () : MetadataDeltaArtifacts =
        let moduleDef = createClosureModule ()
        let assemblyBytes, _, _, _ = createAssemblyBytes moduleDef
        use peReader = new PEReader(new MemoryStream(assemblyBytes, false))
        let metadataReader = peReader.GetMetadataReader()
        let baselineHeapSizes = getHeapSizes metadataReader
        let builder = IlDeltaStreamBuilder None
        let heapOffsets = computeHeapOffsets metadataReader
        let delta = emitClosureDeltaCore metadataReader builder heapOffsets

        assertTableStreamMatches delta

        { BaselineBytes = assemblyBytes
          BaselineHeapSizes = baselineHeapSizes
          Delta = delta }

    let private emitClosureDeltaFromBaseline (baselineBytes: byte[]) (heapOffsets: MetadataHeapOffsets) =
        use peReader = new PEReader(new MemoryStream(baselineBytes, false))
        let metadataReader = peReader.GetMetadataReader()
        let builder = IlDeltaStreamBuilder None
        emitClosureDeltaCore metadataReader builder heapOffsets

    let emitClosureMultiGenerationArtifacts () : MultiGenerationMetadataArtifacts =
        let generation1 = emitClosureDeltaArtifacts ()

        let nextOffsets =
            use peReader = new PEReader(new MemoryStream(generation1.BaselineBytes, false))
            let metadataReader = peReader.GetMetadataReader()
            let baseOffsets = computeHeapOffsets metadataReader
            advanceHeapOffsets baseOffsets generation1.Delta

        let generation2 = emitClosureDeltaFromBaseline generation1.BaselineBytes nextOffsets

        { BaselineBytes = generation1.BaselineBytes
          BaselineHeapSizes = generation1.BaselineHeapSizes
          Generation1 = generation1.Delta
          Generation2 = generation2 }

    type MetadataStreamHeader =
        { Name: string
          Offset: int
          Size: int }

    let private readAlignedString (reader: BinaryReader) =
        let buffer = ResizeArray()
        let mutable finished = false
        while not finished do
            let b = reader.ReadByte()
            if b = 0uy then
                finished <- true
            else
                buffer.Add b
        while reader.BaseStream.Position % 4L <> 0L do
            reader.ReadByte() |> ignore
        Encoding.UTF8.GetString(buffer.ToArray())

    let readMetadataStreamHeaders (metadata: byte[]) =
        use ms = new MemoryStream(metadata, false)
        use reader = new BinaryReader(ms, Encoding.UTF8, leaveOpen = false)

        let signature = reader.ReadUInt32()
        if signature <> 0x424A5342u then
            failwithf "Unexpected metadata signature: 0x%08x" signature

        reader.ReadUInt16() |> ignore
        reader.ReadUInt16() |> ignore
        reader.ReadUInt32() |> ignore
        let versionLength = reader.ReadUInt32() |> int
        reader.ReadBytes(versionLength) |> ignore
        while ms.Position % 4L <> 0L do
            reader.ReadByte() |> ignore

        reader.ReadUInt16() |> ignore
        let streamCount = reader.ReadUInt16() |> int

        [ for _ in 1 .. streamCount do
              let offset = reader.ReadUInt32() |> int
              let size = reader.ReadUInt32() |> int
              let name = readAlignedString reader
              yield { Name = name; Offset = offset; Size = size } ]

    let assertMetadataStreamsEqual expected actual =
        let expectedHeaders : MetadataStreamHeader list = readMetadataStreamHeaders expected
        let actualHeaders : MetadataStreamHeader list = readMetadataStreamHeaders actual
        Xunit.Assert.Equal<MetadataStreamHeader[]>(expectedHeaders |> List.toArray, actualHeaders |> List.toArray)
