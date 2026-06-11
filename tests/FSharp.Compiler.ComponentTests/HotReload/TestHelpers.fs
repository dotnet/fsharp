namespace FSharp.Compiler.ComponentTests.HotReload

open System
open System.Collections.Generic
open System.Collections.Immutable
open System.IO
open System.Reflection
open System.Diagnostics
open System.Reflection.Metadata
open System.Reflection.Metadata.Ecma335
open System.Reflection.PortableExecutable
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILBinaryWriter
open FSharp.Compiler.AbstractIL.ILPdbWriter
open Internal.Utilities
open Internal.Utilities.Library
open FSharp.Compiler.HotReload
open FSharp.Compiler.HotReloadBaseline
open FSharp.Compiler.TypedTreeDiff

type internal BaselineArtifacts =
    {
        Baseline: FSharpEmitBaseline
        TokenMappings: ILTokenMappings
        ModuleId: Guid
        AssemblyName: string
        MetadataSnapshot: MetadataSnapshot
        AssemblyPath: string
        PdbPath: string option
    }

/// Managed probe to mirror CoreCLR ComputeDebuggingConfig (DebuggerAssemblyControlFlags).
module internal DebuggerFlagProbe =
    [<System.Flags>]
    type DebuggerFlags =
        | TrackJitInfo = 0x1
        | IgnorePdbs   = 0x2
        | AllowJitOpts = 0x4

    /// Given a DebuggableAttribute blob (expected 6 or 8 bytes), compute DACF flags per CoreCLR logic.
    let computeFlagsFromDebuggableBlob (blob: byte[]) =
        if isNull (box blob) || blob.Length < 4 then
            None
        else
            let trackAndOpts = blob[2]
            let disableOpts = blob[3]
            let mutable flags = DebuggerFlags.AllowJitOpts
            // Track JIT info?
            if (trackAndOpts &&& 0x1uy) <> 0uy then
                flags <- flags ||| DebuggerFlags.TrackJitInfo
            else
                flags <- flags &&& ~~~DebuggerFlags.TrackJitInfo
            // Ignore PDBs?
            if (trackAndOpts &&& 0x2uy) <> 0uy then
                flags <- flags ||| DebuggerFlags.IgnorePdbs
            else
                flags <- flags &&& ~~~DebuggerFlags.IgnorePdbs
            // Allow JIT opts? (per CoreCLR: allow if tracking bit = 0 OR disableOpts = 0)
            if ((trackAndOpts &&& 0x1uy) = 0uy) || disableOpts = 0uy then
                flags <- flags ||| DebuggerFlags.AllowJitOpts
            else
                flags <- flags &&& ~~~DebuggerFlags.AllowJitOpts
            Some flags

    /// Read DebuggableAttribute blobs from an assembly path and compute DACF flags if possible.
    let tryComputeFlags (assemblyPath: string) =
        use peReader = new PEReader(File.OpenRead assemblyPath)
        let mdReader = peReader.GetMetadataReader()
        let asmDef = mdReader.GetAssemblyDefinition()
        asmDef.GetCustomAttributes()
        |> Seq.choose (fun h ->
            let ca = mdReader.GetCustomAttribute h
            let ctor = ca.Constructor
            let isDebuggable =
                match ctor.Kind with
                | HandleKind.MemberReference ->
                    let mr = mdReader.GetMemberReference(MemberReferenceHandle.op_Explicit ctor)
                    let parent = mr.Parent
                    match parent.Kind with
                    | HandleKind.TypeReference ->
                        let tr = mdReader.GetTypeReference(TypeReferenceHandle.op_Explicit parent)
                        mdReader.GetString tr.Name = "DebuggableAttribute"
                    | HandleKind.TypeDefinition ->
                        let td = mdReader.GetTypeDefinition(TypeDefinitionHandle.op_Explicit parent)
                        mdReader.GetString td.Name = "DebuggableAttribute"
                    | _ -> false
                | HandleKind.MethodDefinition ->
                    let md = mdReader.GetMethodDefinition(MethodDefinitionHandle.op_Explicit ctor)
                    let td = mdReader.GetTypeDefinition(md.GetDeclaringType())
                    mdReader.GetString td.Name = "DebuggableAttribute"
                | _ -> false
            if isDebuggable then
                Some(mdReader.GetBlobBytes ca.Value)
            else
                None)
        |> Seq.tryPick computeFlagsFromDebuggableBlob

/// Simple decoder to convert .NET metadata signatures to ILType for method key construction.
module internal SignatureDecoder =
    open System.Reflection.Metadata

    let private ilg = PrimaryAssemblyILGlobals

    /// Decode a compressed unsigned integer from a signature blob.
    let private decodeCompressedUInt (reader: byref<BlobReader>) =
        let first = reader.ReadByte()
        if (first &&& 0x80uy) = 0uy then
            int first
        elif (first &&& 0xC0uy) = 0x80uy then
            let second = reader.ReadByte()
            ((int first &&& 0x3F) <<< 8) ||| int second
        else
            let b2 = reader.ReadByte()
            let b3 = reader.ReadByte()
            let b4 = reader.ReadByte()
            ((int first &&& 0x1F) <<< 24) ||| (int b2 <<< 16) ||| (int b3 <<< 8) ||| int b4

    /// Decode a type signature to ILType. Only handles primitive types and common cases.
    let rec private decodeType (mdReader: MetadataReader) (reader: byref<BlobReader>) : ILType =
        let typeCode = reader.ReadByte()
        match int typeCode with
        | 0x01 -> ILType.Void  // ELEMENT_TYPE_VOID
        | 0x02 -> ilg.typ_Bool // ELEMENT_TYPE_BOOLEAN
        | 0x03 -> ilg.typ_Char // ELEMENT_TYPE_CHAR
        | 0x04 -> ilg.typ_SByte // ELEMENT_TYPE_I1
        | 0x05 -> ilg.typ_Byte // ELEMENT_TYPE_U1
        | 0x06 -> ilg.typ_Int16 // ELEMENT_TYPE_I2
        | 0x07 -> ilg.typ_UInt16 // ELEMENT_TYPE_U2
        | 0x08 -> ilg.typ_Int32 // ELEMENT_TYPE_I4
        | 0x09 -> ilg.typ_UInt32 // ELEMENT_TYPE_U4
        | 0x0A -> ilg.typ_Int64 // ELEMENT_TYPE_I8
        | 0x0B -> ilg.typ_UInt64 // ELEMENT_TYPE_U8
        | 0x0C -> ilg.typ_Single // ELEMENT_TYPE_R4
        | 0x0D -> ilg.typ_Double // ELEMENT_TYPE_R8
        | 0x0E -> ilg.typ_String // ELEMENT_TYPE_STRING
        | 0x18 -> ilg.typ_IntPtr // ELEMENT_TYPE_I
        | 0x19 -> ilg.typ_UIntPtr // ELEMENT_TYPE_U
        | 0x1C -> ilg.typ_Object // ELEMENT_TYPE_OBJECT
        | 0x11 | 0x12 -> // ELEMENT_TYPE_VALUETYPE, ELEMENT_TYPE_CLASS
            let _token = decodeCompressedUInt &reader
            // For now, return Object as a placeholder for class types
            ilg.typ_Object
        | 0x1D -> // ELEMENT_TYPE_SZARRAY
            let elemType = decodeType mdReader &reader
            ILType.Array(ILArrayShape.SingleDimensional, elemType)
        | 0x0F -> // ELEMENT_TYPE_PTR
            let elemType = decodeType mdReader &reader
            ILType.Ptr elemType
        | 0x10 -> // ELEMENT_TYPE_BYREF
            let elemType = decodeType mdReader &reader
            ILType.Byref elemType
        | _ ->
            // Unknown type - use Object as fallback
            ilg.typ_Object

    /// Decode a method signature and return (paramTypes, returnType).
    let decodeMethodSignature (mdReader: MetadataReader) (sigBlob: BlobHandle) : ILType list * ILType =
        let mutable reader = mdReader.GetBlobReader sigBlob
        let callingConv = reader.ReadByte()

        // Check for generic method
        let _genericParamCount =
            if (callingConv &&& 0x10uy) <> 0uy then
                decodeCompressedUInt &reader
            else
                0

        let paramCount = decodeCompressedUInt &reader
        let returnType = decodeType mdReader &reader

        let paramTypes =
            [ for _ in 1..paramCount do
                yield decodeType mdReader &reader ]

        paramTypes, returnType

module internal TestHelpers =

    let private mscorlibToken =
        PublicKeyToken [|
            0xb7uy
            0x7auy
            0x5cuy
            0x56uy
            0x19uy
            0x34uy
            0xe0uy
            0x89uy
        |]

    let private fsharpCoreToken =
        PublicKeyToken [|
            0xb0uy
            0x3fuy
            0x5fuy
            0x7fuy
            0x11uy
            0xd5uy
            0x0auy
            0x3auy
        |]

    // Target the runtime core library for attributes/token resolution (use actual version + pkt).
    let private mscorlibRef =
        let an = typeof<int>.Assembly.GetName()
        let pkt = an.GetPublicKeyToken()
        ILAssemblyRef.Create(
            an.Name,
            None,
            (if isNull pkt || pkt.Length = 0 then None else Some(PublicKeyToken pkt)),
            false,
            Some(ILVersionInfo(uint16 an.Version.Major, uint16 an.Version.Minor, uint16 an.Version.Build, uint16 an.Version.Revision)),
            None)

    let private fsharpCoreRef =
        ILAssemblyRef.Create(
            "FSharp.Core",
            None,
            Some fsharpCoreToken,
            false,
            Some(ILVersionInfo(0us, 0us, 0us, 0us)),
            None)

    let private testIlGlobals =
        mkILGlobals(ILScopeRef.Assembly mscorlibRef, [], ILScopeRef.Assembly fsharpCoreRef)

    module ILWriter = FSharp.Compiler.AbstractIL.ILBinaryWriter

    let defaultWriterOptionsForTests (ilg: ILGlobals) : ILWriter.options =
        let scratchDll = Path.Combine(Path.GetTempPath(), sprintf "fsharp-hotreload-test-%s.dll" (System.Guid.NewGuid().ToString("N")))
        let scratchPdb = Path.ChangeExtension(scratchDll, ".pdb")
        { ilg = ilg
          outfile = scratchDll
          pdbfile = Some scratchPdb
          portablePDB = true
          embeddedPDB = false
          embedAllSource = false
          embedSourceList = []
          allGivenSources = []
          sourceLink = ""
          checksumAlgorithm = HashAlgorithm.Sha256
          signer = None
          emitTailcalls = false
          deterministic = true
          dumpDebugInfo = false
          referenceAssemblyOnly = false
          referenceAssemblyAttribOpt = None
          referenceAssemblySignatureHash = None
          pathMap = PathMap.empty
          methodCustomDebugInfoRows = Map.empty }

    let private collectSourceDocuments (ilModule: ILModuleDef) : ILSourceDocument list =
        let docs = HashSet<ILSourceDocument>(HashIdentity.Reference)

        let addDoc (doc: ILSourceDocument) =
            if not (isNull (box doc)) then
                docs.Add doc |> ignore

        let rec collectInstr (instr: ILInstr) =
            match instr with
            | I_seqpoint debugPoint -> addDoc debugPoint.Document
            | _ -> ()

        let collectCode (code: ILCode) =
            code.Instrs |> Array.iter collectInstr

        let collectMethod (methodDef: ILMethodDef) =
            match methodDef.Body with
            | MethodBody.IL ilBodyLazy ->
                let ilBody = ilBodyLazy.Value
                collectCode ilBody.Code
                match ilBody.DebugRange with
                | Some debugPoint -> addDoc debugPoint.Document
                | None -> ()
            | _ -> ()

        let rec collectTypeDef (typeDef: ILTypeDef) =
            typeDef.Methods.AsList() |> List.iter collectMethod
            typeDef.NestedTypes.AsList() |> List.iter collectTypeDef

        ilModule.TypeDefs.AsList() |> List.iter collectTypeDef

        docs |> Seq.toList

    let createPropertyModule (message: string) : ILModuleDef =
        let ilg = PrimaryAssemblyILGlobals
        let stringType = ilg.typ_String
        let typeName = "Sample.PropertyDemo"
        let typeRef = mkILTyRef(ILScopeRef.Local, typeName)
        let document = ILSourceDocument.Create(None, None, None, "PropertyDemo.fs")
        let debugPoint = ILDebugPoint.Create(document, 1, 1, 1, 40)

        let getterBody =
            mkMethodBody (
                false,
                [],
                2,
                nonBranchingInstrsToCode [ I_seqpoint debugPoint; I_ldstr message; I_ret ],
                Some debugPoint,
                None)

        let getter =
            mkILNonGenericInstanceMethod(
                "get_Message",
                ILMemberAccess.Public,
                [],
                mkILReturn stringType,
                getterBody)
            |> fun methodDef -> methodDef.WithSpecialName.WithHideBySig(true)

        let propertyDef =
            ILPropertyDef(
                "Message",
                PropertyAttributes.None,
                None,
                Some(mkILMethRef(typeRef, ILCallingConv.Instance, "get_Message", 0, [], stringType)),
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

    let createMethodModule (message: string) : ILModuleDef =
        let ilg = PrimaryAssemblyILGlobals
        let stringType = ilg.typ_String
        let typeName = "Sample.MethodDemo"
        let document = ILSourceDocument.Create(None, None, None, "MethodDemo.fs")
        let debugPoint = ILDebugPoint.Create(document, 1, 1, 1, 20)

        let body =
            mkMethodBody(
                false,
                [],
                2,
                nonBranchingInstrsToCode [ I_seqpoint debugPoint; I_ldstr message; I_ret ],
                Some debugPoint,
                None)

        let methodDef =
            mkILNonGenericStaticMethod(
                "GetMessage",
                ILMemberAccess.Public,
                [],
                mkILReturn stringType,
                body)

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

    let createClosureModule (message: string) : ILModuleDef =
        let ilg = PrimaryAssemblyILGlobals
        let stringType = ilg.typ_String
        let typeName = "Sample.ClosureDemo"
        let document = ILSourceDocument.Create(None, None, None, "ClosureDemo.fs")
        let debugPoint = ILDebugPoint.Create(document, 1, 1, 1, 40)

        let body =
            mkMethodBody(
                false,
                [],
                2,
                nonBranchingInstrsToCode [ I_seqpoint debugPoint; I_ldstr message; I_ret ],
                Some debugPoint,
                None)

        let methodDef =
            mkILNonGenericStaticMethod(
                "Invoke",
                ILMemberAccess.Public,
                [],
                mkILReturn stringType,
                body)

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
            "SampleClosureAssembly"
            "SampleClosureModule"
            true
            (4, 0)
            false
            (mkILTypeDefs [ typeDef ])
            None
            None
            0
            (mkILExportedTypes [])
            "v4.0.30319"

    let createAsyncModule (message: string) : ILModuleDef =
        let ilg = PrimaryAssemblyILGlobals
        let stringType = ilg.typ_String
        let boolType = ilg.typ_Bool
        let hostTypeName = "Sample.AsyncDemo"
        let stateMachineTypeName = "Sample.AsyncDemoStateMachine"
        let document = ILSourceDocument.Create(None, None, None, "AsyncDemo.fs")
        let debugPoint = ILDebugPoint.Create(document, 1, 1, 1, 50)

        let runBody =
            mkMethodBody(
                false,
                [],
                2,
                nonBranchingInstrsToCode [ I_seqpoint debugPoint; I_ldstr message; I_ret ],
                Some debugPoint,
                None)

        let runMethod =
            mkILNonGenericStaticMethod(
                "RunAsync",
                ILMemberAccess.Public,
                [ mkILParamNamed("token", ilg.typ_Int32) ],
                mkILReturn stringType,
                runBody)

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
                    hostTypeName,
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
                    stateMachineTypeName,
                    ILTypeDefAccess.Public,
                    mkILMethods [ moveNextMethod ],
                    mkILFields [],
                    emptyILTypeDefs,
                    mkILProperties [],
                    mkILEvents [],
                    emptyILCustomAttrs,
                    ILTypeInit.BeforeField )

        mkILSimpleModule
            "SampleAsyncAssembly"
            "SampleAsyncModule"
            true
            (4, 0)
            false
            (mkILTypeDefs [ hostType; stateMachineType ])
            None
            None
            0
            (mkILExportedTypes [])
            "v4.0.30319"

    let createPropertyHostBaselineModule () : ILModuleDef =
        let ilg = PrimaryAssemblyILGlobals
        let typeName = "Sample.PropertyDemo"
        let document = ILSourceDocument.Create(None, None, None, "PropertyDemo.fs")
        let debugPoint = ILDebugPoint.Create(document, 1, 1, 1, 20)

        let methodBody =
            mkMethodBody(
                false,
                [],
                2,
                nonBranchingInstrsToCode [ I_seqpoint debugPoint; I_ldstr "Host baseline"; I_ret ],
                Some debugPoint,
                None)

        let methodDef =
            mkILNonGenericInstanceMethod(
                "GetBaseline",
                ILMemberAccess.Public,
                [],
                mkILReturn ilg.typ_String,
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

    let createEventModule (message: string) : ILModuleDef =
        let ilg = PrimaryAssemblyILGlobals
        let typeName = "Sample.EventDemo"
        let typeRef = mkILTyRef(ILScopeRef.Local, typeName)
        let voidType = ILType.Void
        let handlerType = ilg.typ_Object

        let document = ILSourceDocument.Create(None, None, None, "EventDemo.fs")
        let addPoint = ILDebugPoint.Create(document, 1, 1, 1, 50)
        let removePoint = ILDebugPoint.Create(document, 10, 1, 10, 50)

        let addBody =
            mkMethodBody(
                false,
                [],
                2,
                nonBranchingInstrsToCode [ I_seqpoint addPoint; I_ldstr message; AI_pop; I_ret ],
                Some addPoint,
                None)

        let removeBody =
            mkMethodBody(
                false,
                [],
                1,
                nonBranchingInstrsToCode [ I_seqpoint removePoint; I_ret ],
                Some removePoint,
                None)

        let addMethod =
            mkILNonGenericInstanceMethod(
                "add_OnChanged",
                ILMemberAccess.Public,
                [ mkILParamNamed ("handler", handlerType) ],
                mkILReturn voidType,
                addBody)
            |> fun methodDef -> methodDef.WithSpecialName.WithHideBySig(true)

        let removeMethod =
            mkILNonGenericInstanceMethod(
                "remove_OnChanged",
                ILMemberAccess.Public,
                [ mkILParamNamed ("handler", handlerType) ],
                mkILReturn voidType,
                removeBody)
            |> fun methodDef -> methodDef.WithSpecialName.WithHideBySig(true)

        let eventDef =
            ILEventDef(
                Some handlerType,
                "OnChanged",
                EventAttributes.None,
                mkILMethRef(typeRef, ILCallingConv.Instance, "add_OnChanged", 0, [ handlerType ], voidType),
                mkILMethRef(typeRef, ILCallingConv.Instance, "remove_OnChanged", 0, [ handlerType ], voidType),
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

    let createEventHostBaselineModule () : ILModuleDef =
        let ilg = PrimaryAssemblyILGlobals
        let typeName = "Sample.EventDemo"
        let document = ILSourceDocument.Create(None, None, None, "EventDemo.fs")
        let debugPoint = ILDebugPoint.Create(document, 1, 1, 1, 30)

        let invokeBody =
            mkMethodBody(
                false,
                [],
                1,
                nonBranchingInstrsToCode [ I_seqpoint debugPoint; I_ldstr "Host"; AI_pop; I_ret ],
                Some debugPoint,
                None)

        let invokeMethod =
            mkILNonGenericInstanceMethod(
                "Invoke",
                ILMemberAccess.Public,
                [ mkILParamNamed("handler", ilg.typ_Object) ],
                mkILReturn ILType.Void,
                invokeBody)

        let typeDef =
            mkILSimpleClass
                ilg
                (
                    typeName,
                    ILTypeDefAccess.Public,
                    mkILMethods [ invokeMethod ],
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

    let private computePdbRowCounts (reader: MetadataReader) : ImmutableArray<int> =
        let counts = Array.zeroCreate<int> MetadataTokens.TableCount

        let inline setCount (index: TableIndex) (value: int) =
            counts[int index] <- value

        setCount TableIndex.Document reader.Documents.Count
        setCount TableIndex.MethodDebugInformation reader.MethodDebugInformation.Count
        setCount TableIndex.LocalScope reader.LocalScopes.Count
        setCount TableIndex.LocalVariable reader.LocalVariables.Count
        setCount TableIndex.LocalConstant reader.LocalConstants.Count
        setCount TableIndex.ImportScope reader.ImportScopes.Count
        setCount TableIndex.CustomDebugInformation reader.CustomDebugInformation.Count

        ImmutableArray.CreateRange counts

    let private createPortablePdbSnapshot (pdbBytes: byte[]) : PortablePdbSnapshot =
        use provider = MetadataReaderProvider.FromPortablePdbImage(ImmutableArray.CreateRange pdbBytes)
        let reader = provider.GetMetadataReader()
        let rowCounts = computePdbRowCounts reader
        let entryPointHandle = reader.DebugMetadataHeader.EntryPoint

        let entryPointToken =
            if entryPointHandle.IsNil then
                None
            else
                let entityHandle: EntityHandle = MethodDefinitionHandle.op_Implicit entryPointHandle
                Some(MetadataTokens.GetToken entityHandle)

        { Bytes = Array.copy pdbBytes
          TableRowCounts = rowCounts
          EntryPointToken = entryPointToken }

    /// Attach DebuggableAttribute(Default | DisableOptimizations | EnableEditAndContinue) so the runtime
    /// treats the module as EnC-capable (clears DACF_ALLOW_JIT_OPTS, sets DACF_ENC_ENABLED).
    let withDebuggableAttribute (ilModule: ILModuleDef) : ILModuleDef =
        let debuggableAttr =
            let attrTypeRef = mkILTyRef(ILScopeRef.Assembly mscorlibRef, "System.Diagnostics.DebuggableAttribute")
            let modesTypeRef = mkILTyRefInTyRef(attrTypeRef, "DebuggingModes")
            let modesType = ILType.Value (mkILNonGenericTySpec modesTypeRef)
            let modesValue =
                int32 DebuggableAttribute.DebuggingModes.Default
                ||| int32 DebuggableAttribute.DebuggingModes.DisableOptimizations
                ||| int32 DebuggableAttribute.DebuggingModes.EnableEditAndContinue
            let attrType = mkILBoxedType (mkILNonGenericTySpec attrTypeRef)
            let ctor = mkILNonGenericInstanceMethSpecInTy(attrType, ".ctor", [ modesType ], ILType.Void)
            mkILCustomAttribMethRef(ctor, [ ILAttribElem.Int32 modesValue ], [])

        let manifestWithAttr =
            let manifest =
                match ilModule.Manifest with
                | Some m -> m
                | None ->
                    { Name = ilModule.Name
                      AuxModuleHashAlgorithm = 0
                      SecurityDeclsStored = storeILSecurityDecls (mkILSecurityDecls [])
                      PublicKey = None
                      Version = Some(ILVersionInfo(1us, 0us, 0us, 0us))
                      Locale = None
                      CustomAttrsStored = storeILCustomAttrs emptyILCustomAttrs
                      AssemblyLongevity = ILAssemblyLongevity.Unspecified
                      DisableJitOptimizations = true
                      JitTracking = true
                      IgnoreSymbolStoreSequencePoints = false
                      Retargetable = false
                      ExportedTypes = mkILExportedTypes []
                      EntrypointElsewhere = None
                      MetadataIndex = 0 }

            // Force the DebuggableAttribute to reflect Debug semantics.
            let existing = manifest.CustomAttrs.AsArray()
            let combined = Array.append existing [| debuggableAttr |] |> mkILCustomAttrsFromArray
            { manifest with
                CustomAttrsStored = storeILCustomAttrs combined
                DisableJitOptimizations = true
                JitTracking = true }

        { ilModule with
            CustomAttrsStored = ilModule.CustomAttrsStored
            Manifest = Some manifestWithAttr }

    let createBaselineFromModule (ilModule: ILModuleDef) : BaselineArtifacts =
        let ilModule = withDebuggableAttribute ilModule
        let documents = collectSourceDocuments ilModule

        let writerOptions =
            { defaultWriterOptionsForTests testIlGlobals with
                allGivenSources = documents }
        let assemblyBytes, pdbBytesOpt, tokenMappings, _ =
            ILWriter.WriteILBinaryInMemoryWithArtifacts(writerOptions, ilModule, id)

        if System.Environment.GetEnvironmentVariable("FSHARP_HOTRELOAD_TRACE_BASELINE") = "1" then
            let pdbPathLogged =
                match writerOptions.pdbfile with
                | Some p -> p
                | None -> "<none>"
            printfn "[baseline] outfile=%s pdb=%s" writerOptions.outfile pdbPathLogged

        File.WriteAllBytes(writerOptions.outfile, assemblyBytes)

        let pdbPath =
            match writerOptions.pdbfile, pdbBytesOpt with
            | Some path, Some bytes ->
                File.WriteAllBytes(path, bytes)
                Some path
            | _ -> None

        // Extract module ID from PE metadata
        use peReader = new PEReader(new MemoryStream(assemblyBytes, writable = false))
        let metadataReader = peReader.GetMetadataReader()
        let moduleDef = metadataReader.GetModuleDefinition()
        let moduleId =
            if moduleDef.Mvid.IsNil then System.Guid.NewGuid() else metadataReader.GetGuid(moduleDef.Mvid)

        // Use SRM-free byte-based APIs
        let metadataSnapshot =
            match metadataSnapshotFromBytes assemblyBytes with
            | Some snapshot -> snapshot
            | None -> failwith "Failed to parse metadata snapshot from assembly bytes"

        let portablePdbSnapshot = pdbBytesOpt |> Option.map createPortablePdbSnapshot

        let baseline =
            let core = create ilModule tokenMappings metadataSnapshot moduleId portablePdbSnapshot
            attachMetadataHandlesFromBytes assemblyBytes core

        { Baseline = baseline
          TokenMappings = tokenMappings
          ModuleId = moduleId
          AssemblyName = ilModule.Name
          MetadataSnapshot = metadataSnapshot
          AssemblyPath = writerOptions.outfile
          PdbPath = pdbPath }

    /// Compile a tiny C# classlib in Debug and use its assembly as the baseline for runtime tests.
    /// Returns the compiled assembly path and baseline artifacts loaded from that file, with token maps built from the real metadata.
    let createBaselineFromRealCompiler (sourceText: string) : BaselineArtifacts =
        // Write source to temp folder
        let workDir = Path.Combine(Path.GetTempPath(), "fsharp-hotreload-real-baseline-" + System.Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(workDir) |> ignore
        let assemblyName = "Baseline_" + System.Guid.NewGuid().ToString("N")
        let projPath = Path.Combine(workDir, "Baseline.csproj")
        let srcPath = Path.Combine(workDir, "Baseline.cs")
        File.WriteAllText(srcPath, sourceText)
        let projContents =
            $"""<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Library</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <LangVersion>preview</LangVersion>
    <DebugType>portable</DebugType>
    <Optimize>false</Optimize>
    <DebugSymbols>true</DebugSymbols>
    <AssemblyName>{assemblyName}</AssemblyName>
    <Nullable>disable</Nullable>
  </PropertyGroup>
</Project>
"""
        File.WriteAllText(projPath, projContents)

        let psi = System.Diagnostics.ProcessStartInfo()
        psi.FileName <- Path.Combine(__SOURCE_DIRECTORY__, "..", "..", "..", ".dotnet", "dotnet")
        psi.ArgumentList.Add("build")
        psi.ArgumentList.Add(projPath)
        psi.ArgumentList.Add("-c")
        psi.ArgumentList.Add("Debug")
        psi.ArgumentList.Add("-v")
        psi.ArgumentList.Add("m")
        psi.RedirectStandardOutput <- true
        psi.RedirectStandardError <- true
        psi.WorkingDirectory <- workDir

        use p = new System.Diagnostics.Process()
        p.StartInfo <- psi
        p.Start() |> ignore
        let stdout = p.StandardOutput.ReadToEnd()
        let stderr = p.StandardError.ReadToEnd()
        p.WaitForExit()
        if p.ExitCode <> 0 then failwithf "dotnet build failed: %s\n%s" stdout stderr

        // Locate built DLL
        let preferred =
            let bin = Path.Combine(workDir, "bin", "Debug", "net10.0", assemblyName + ".dll")
            if File.Exists(bin) then Some bin else None
        let dllPath =
            match preferred with
            | Some path -> path
            | None ->
                Directory.EnumerateFiles(workDir, assemblyName + ".dll", SearchOption.AllDirectories)
                |> Seq.tryHead
                |> Option.defaultWith (fun () -> failwithf "%s.dll not found after build" assemblyName)

        // Load assembly bytes and metadata
        let assemblyBytes = File.ReadAllBytes(dllPath)
        use peReader = new PEReader(new MemoryStream(assemblyBytes, writable = false))
        let reader = peReader.GetMetadataReader()
        let moduleDef = reader.GetModuleDefinition()
        let moduleId = if moduleDef.Mvid.IsNil then System.Guid.NewGuid() else reader.GetGuid moduleDef.Mvid

        // Use SRM-free byte-based API for metadata snapshot
        let metadataSnapshot =
            match metadataSnapshotFromBytes assemblyBytes with
            | Some snapshot -> snapshot
            | None -> failwith "Failed to parse metadata snapshot from assembly bytes"

        // Build token maps directly from metadata
        let typeTokens =
            reader.TypeDefinitions
            |> Seq.map (fun h ->
                let td = reader.GetTypeDefinition h
                let ns = if td.Namespace.IsNil then "" else reader.GetString td.Namespace
                let name = reader.GetString td.Name
                let fullName = if String.IsNullOrEmpty ns then name else ns + "." + name
                let token = MetadataTokens.GetToken(EntityHandle.op_Implicit h)
                fullName, token)
            |> Map.ofSeq

        let methodTokens =
            reader.MethodDefinitions
            |> Seq.map (fun h ->
                let md = reader.GetMethodDefinition h
                let name = reader.GetString md.Name
                let parent = md.GetDeclaringType()
                let td = reader.GetTypeDefinition parent
                let ns = if td.Namespace.IsNil then "" else reader.GetString td.Namespace
                let tn = reader.GetString td.Name
                let fullType = if String.IsNullOrEmpty ns then tn else ns + "." + tn
                let paramTypes, returnType = SignatureDecoder.decodeMethodSignature reader md.Signature
                let key: MethodDefinitionKey =
                    { DeclaringType = fullType
                      Name = name
                      GenericArity = md.GetGenericParameters().Count
                      ParameterTypes = paramTypes
                      ReturnType = returnType }
                let token = MetadataTokens.GetToken(EntityHandle.op_Implicit h)
                key, token)
            |> Map.ofSeq

        let dummyMappings : ILTokenMappings =
            { TypeDefTokenMap = fun _ -> 0
              FieldDefTokenMap = fun _ _ -> 0
              MethodDefTokenMap = fun _ _ -> 0
              PropertyTokenMap = fun _ _ -> 0
              EventTokenMap = fun _ _ -> 0 }

        let baseline : FSharpEmitBaseline =
            { ModuleId = moduleId
              EncId = System.Guid.Empty
              EncBaseId = System.Guid.Empty
              NextGeneration = 1
              ModuleNameOffset = None
              Metadata = metadataSnapshot
              TokenMappings = dummyMappings
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
              MemberReferenceRows = Map.empty
              TypeSpecSignatures = Map.empty
              CustomAttributeRows = Map.empty
              TableEntriesAdded = Array.zeroCreate MetadataTokens.TableCount
              StringStreamLengthAdded = 0
              UserStringStreamLengthAdded = 0
              BlobStreamLengthAdded = 0
              GuidStreamLengthAdded = 0
              AddedOrChangedMethods = []
              EncMethodDebugInfos = Map.empty
              EncClosureNames = Map.empty
              SequencePointSnapshots = Map.empty }

        // Attach string handles from baseline metadata so delta can reuse them
        let baselineWithHandles = attachMetadataHandlesFromBytes assemblyBytes baseline

        { Baseline = baselineWithHandles
          TokenMappings = dummyMappings
          ModuleId = moduleId
          AssemblyName = assemblyName
          MetadataSnapshot = metadataSnapshot
          AssemblyPath = dllPath
          PdbPath = None }

    let methodKeyByName (baseline: FSharpEmitBaseline) typeName methodName =
        baseline.MethodTokens
        |> Map.toSeq
        |> Seq.map fst
        |> Seq.find (fun key -> key.DeclaringType = typeName && key.Name = methodName)

    let methodKey
        (typeName: string)
        (methodName: string)
        (parameterTypes: ILType list)
        (returnType: ILType)
        : MethodDefinitionKey =
        { DeclaringType = typeName
          Name = methodName
          GenericArity = 0
          ParameterTypes = parameterTypes
          ReturnType = returnType }

    let propertyKeyByName (baseline: FSharpEmitBaseline) typeName propertyName =
        baseline.PropertyTokens
        |> Map.toSeq
        |> Seq.map fst
        |> Seq.tryFind (fun key -> key.DeclaringType = typeName && key.Name = propertyName)

    let assertBaselineDocument (pdbPath: string option) (expectedName: string) : unit =
        match pdbPath with
        | None -> failwithf "Baseline PDB path missing (expected document '%s')." expectedName
        | Some path ->
            let bytes = File.ReadAllBytes path |> ImmutableArray.CreateRange
            use provider = MetadataReaderProvider.FromPortablePdbImage(bytes)
            let reader = provider.GetMetadataReader()
            let hasDocument =
                reader.Documents
                |> Seq.exists (fun handle ->
                    let document = reader.GetDocument handle
                    reader.GetString(document.Name) = expectedName)
            if not hasDocument then
                failwithf "Baseline PDB '%s' did not contain document '%s'." path expectedName

    let mkAccessorUpdate (typeName: string) (memberKind: SymbolMemberKind) (methodKey: MethodDefinitionKey) =
        let logicalName =
            match memberKind with
            | SymbolMemberKind.PropertyGet name
            | SymbolMemberKind.PropertySet name
            | SymbolMemberKind.EventAdd name
            | SymbolMemberKind.EventRemove name
            | SymbolMemberKind.EventInvoke name -> name
            | SymbolMemberKind.Method -> methodKey.Name

        let symbol =
            { Path = typeName.Split('.') |> Array.toList
              LogicalName = logicalName
              Stamp = 0L
              Kind = SymbolKind.Value
              MemberKind = Some memberKind
              IsSynthesized = false
              CompiledName = None
              TotalArgCount = None
              GenericArity = None
              ParameterTypeIdentities = None
              ReturnTypeIdentity = None }

        { AccessorUpdate.Symbol = symbol
          ContainingType = typeName
          MemberKind = memberKind
          Method = Some methodKey }
