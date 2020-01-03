namespace FSharp.Compiler.AbstractIL

open System
open System.IO
open System.Linq
open System.Collections.Generic
open System.Collections.Immutable
open System.Collections.ObjectModel
open System.Reflection
open System.Reflection.PortableExecutable
open System.Reflection.Metadata
open System.Reflection.Metadata.Ecma335
open FSharp.Compiler.Lib
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.Internal
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.AbstractIL.Internal.BinaryConstants

#nowarn "9"

[<AutoOpen>]
module rec ILBinaryReaderImpl =

    type OperandType = System.Reflection.Emit.OperandType

    type PdbReaderProvider = MetadataReaderProvider * string

    type MetadataReader with

        member this.TryGetString(handle: StringHandle) =
            if handle.IsNil then ValueNone
            else ValueSome(this.GetString(handle))

        member this.GetTypeName(namespaceHandle: StringHandle, nameHandle: StringHandle) =
            let name = this.GetString nameHandle
            if namespaceHandle.IsNil then name
            else this.GetString namespaceHandle + "." + name

    [<Sealed>]
    type cenv(
                peReader: PEReader, 
                mdReader: MetadataReader,
                pdbReaderProviderOpt: PdbReaderProvider option,
                entryPointToken: int,
                isMetadataOnly: bool,
                canReduceMemory: bool,
                sigTyProvider: ISignatureTypeProvider<ILType, unit>,
                localSigTyProvider: ISignatureTypeProvider<ILLocal, unit>) =

        let typeDefCache = Dictionary()
        let typeRefCache = Dictionary()
        let typeSpecCache = Dictionary()
        let asmRefCache = Dictionary()
        let memberRefToILMethSpecCache = Dictionary()
        let methDefToILMethSpecCache = Dictionary()

        let isCachingEnabled = not canReduceMemory

        member _.IsMetadataOnly = isMetadataOnly

        member _.CanReduceMemory = canReduceMemory

        member _.PEReader = peReader

        member _.MetadataReader = mdReader

        member _.PdbReaderProvider = pdbReaderProviderOpt

        member _.EntryPointToken = entryPointToken

        member _.SignatureTypeProvider = sigTyProvider

        member _.LocalSignatureTypeProvider = localSigTyProvider

        member _.CacheILType(typeDefHandle: TypeDefinitionHandle, ilType: ILType) =
            if isCachingEnabled then
                typeDefCache.Add(typeDefHandle, ilType)

        member _.CacheILType(typeRefHandle: TypeReferenceHandle, ilType: ILType) =
            if isCachingEnabled then
                typeRefCache.Add(typeRefHandle, ilType)

        member _.CacheILType(typeSpecHandle: TypeSpecificationHandle, ilType: ILType) =
            if isCachingEnabled then
                typeSpecCache.Add(typeSpecHandle, ilType)

        member _.CacheILAssemblyRef(asmRefHandle: AssemblyReferenceHandle, ilAsmRef: ILAssemblyRef) =
            asmRefCache.Add(asmRefHandle, ilAsmRef)

        member _.CacheILMethodSpec(memberRefHandle: MemberReferenceHandle, ilMethSpec: ILMethodSpec) =
            if isCachingEnabled then
                memberRefToILMethSpecCache.Add(memberRefHandle, ilMethSpec)

        member _.CacheILMethodSpec(methDefHandle: MethodDefinitionHandle, ilMethSpec: ILMethodSpec) =
            if isCachingEnabled then
                methDefToILMethSpecCache.Add(methDefHandle, ilMethSpec)
        
        member _.TryGetCachedILType(typeDefHandle) =
            match typeDefCache.TryGetValue(typeDefHandle) with
            | true, ilType -> ValueSome(ilType)
            | _ -> ValueNone

        member _.TryGetCachedILType(typeRefHandle) =
            match typeRefCache.TryGetValue(typeRefHandle) with
            | true, ilType -> ValueSome(ilType)
            | _ -> ValueNone
   
        member _.TryGetCachedILType(typeSpecHandle) =
            match typeSpecCache.TryGetValue(typeSpecHandle) with
            | true, ilType -> ValueSome(ilType)
            | _ -> ValueNone

        member _.TryGetCachedILAssemblyRef(asmRefHandle: AssemblyReferenceHandle) =
            match asmRefCache.TryGetValue(asmRefHandle) with
            | true, ilAsmRef -> ValueSome(ilAsmRef)
            | _ -> ValueNone

        member _.TryGetCachedILMethodSpec(memberRefHandle) =
            match memberRefToILMethSpecCache.TryGetValue(memberRefHandle) with
            | true, ilMethSpec -> ValueSome(ilMethSpec)
            | _ -> ValueNone

        member _.TryGetCachedILMethodSpec(methDefHandle) =
            match methDefToILMethSpecCache.TryGetValue(methDefHandle) with
            | true, ilMethSpec -> ValueSome(ilMethSpec)
            | _ -> ValueNone

    let mkILVersionInfo (v: Version) =
        ILVersionInfo(uint16 v.Major, uint16 v.Minor, uint16 v.Build, uint16 v.Revision)
    
    let mkILMemberAccess (attributes: TypeAttributes) =
        let attributes = attributes &&& TypeAttributes.VisibilityMask
        match attributes with
        | TypeAttributes.Public -> ILMemberAccess.Public
        | TypeAttributes.NestedPublic -> ILMemberAccess.Public
        | TypeAttributes.NestedPrivate -> ILMemberAccess.Private
        | TypeAttributes.NestedFamily -> ILMemberAccess.Family
        | TypeAttributes.NestedAssembly -> ILMemberAccess.Assembly
        | TypeAttributes.NestedFamANDAssem -> ILMemberAccess.FamilyAndAssembly
        | TypeAttributes.NestedFamORAssem -> ILMemberAccess.FamilyOrAssembly
        | _ -> ILMemberAccess.Private

    let mkILTypeDefAccess (attributes: TypeAttributes) =
        let attributes = attributes &&& TypeAttributes.VisibilityMask
        match attributes with
        | TypeAttributes.Public -> ILTypeDefAccess.Public
        | TypeAttributes.NestedPublic
        | TypeAttributes.NestedPrivate
        | TypeAttributes.NestedFamily
        | TypeAttributes.NestedAssembly
        | TypeAttributes.NestedFamANDAssem
        | TypeAttributes.NestedFamORAssem -> ILTypeDefAccess.Nested (mkILMemberAccess attributes)
        | _ -> ILTypeDefAccess.Private

    let mkILAssemblyLongevity (flags: AssemblyFlags) =
        let  masked = int flags &&& 0x000e
        if   masked = 0x0000 then ILAssemblyLongevity.Unspecified
        elif masked = 0x0002 then ILAssemblyLongevity.Library
        elif masked = 0x0004 then ILAssemblyLongevity.PlatformAppDomain
        elif masked = 0x0006 then ILAssemblyLongevity.PlatformProcess
        elif masked = 0x0008 then ILAssemblyLongevity.PlatformSystem
        else                      ILAssemblyLongevity.Unspecified

    let mkILTypeDefLayoutInfo (layout: TypeLayout) =
        if layout.IsDefault then
            { Size = None; Pack = None }
        else
            { Size = Some(layout.Size); Pack = Some(uint16 layout.PackingSize) }

    let mkILTypeDefLayout (attributes: TypeAttributes) (layout: TypeLayout) =
        match attributes &&& TypeAttributes.LayoutMask with
        | TypeAttributes.SequentialLayout ->
            ILTypeDefLayout.Sequential(mkILTypeDefLayoutInfo layout)
        | TypeAttributes.ExplicitLayout ->
            ILTypeDefLayout.Explicit(mkILTypeDefLayoutInfo layout)
        | _ ->
            ILTypeDefLayout.Auto

    let mkILSecurityAction (declSecurityAction: DeclarativeSecurityAction) =
        match declSecurityAction with
        | DeclarativeSecurityAction.Demand -> ILSecurityAction.Demand
        | DeclarativeSecurityAction.Assert -> ILSecurityAction.Assert
        | DeclarativeSecurityAction.Deny -> ILSecurityAction.Deny
        | DeclarativeSecurityAction.PermitOnly -> ILSecurityAction.PermitOnly
        | DeclarativeSecurityAction.LinkDemand -> ILSecurityAction.LinkCheck
        | DeclarativeSecurityAction.InheritanceDemand -> ILSecurityAction.InheritCheck
        | DeclarativeSecurityAction.RequestMinimum -> ILSecurityAction.ReqMin
        | DeclarativeSecurityAction.RequestOptional -> ILSecurityAction.ReqOpt
        | DeclarativeSecurityAction.RequestRefuse -> ILSecurityAction.ReqRefuse
        | _ ->
            // Comment below is from System.Reflection.Metadata
            // Wait for an actual need before exposing these. They all have ilasm keywords, but some are missing from the CLI spec and 
            // and none are defined in System.Security.Permissions.SecurityAction.
            //Request = 0x0001,
            //PrejitGrant = 0x000B,
            //PrejitDeny = 0x000C,
            //NonCasDemand = 0x000D,
            //NonCasLinkDemand = 0x000E,
            //NonCasInheritanceDemand = 0x000F,
            match int declSecurityAction with
            | 0x0001 -> ILSecurityAction.Request
            | 0x000b -> ILSecurityAction.PreJitGrant
            | 0x000c -> ILSecurityAction.PreJitDeny
            | 0x000d -> ILSecurityAction.NonCasDemand
            | 0x000e -> ILSecurityAction.NonCasLinkDemand
            | 0x000f -> ILSecurityAction.NonCasInheritance
            | 0x0010 -> ILSecurityAction.LinkDemandChoice
            | 0x0011 -> ILSecurityAction.InheritanceDemandChoice
            | 0x0012 -> ILSecurityAction.DemandChoice
            | x -> failwithf "Invalid DeclarativeSecurityAction: %i" x

    let mkILThisConvention (sigHeader: SignatureHeader) =
        if sigHeader.IsInstance then
            if sigHeader.HasExplicitThis then ILThisConvention.InstanceExplicit
            else ILThisConvention.Instance
        else ILThisConvention.Static
    
    let mkILCallingConv (sigHeader: SignatureHeader) =
        let ilThisConvention = mkILThisConvention sigHeader

        let ilArgConvention =
            match sigHeader.CallingConvention with
            | SignatureCallingConvention.Default -> ILArgConvention.Default
            | SignatureCallingConvention.CDecl -> ILArgConvention.CDecl
            | SignatureCallingConvention.StdCall -> ILArgConvention.StdCall
            | SignatureCallingConvention.ThisCall -> ILArgConvention.ThisCall
            | SignatureCallingConvention.FastCall -> ILArgConvention.FastCall
            | SignatureCallingConvention.VarArgs -> ILArgConvention.VarArg
            | _ -> failwithf "Invalid Signature Calling Convention: %A" sigHeader.CallingConvention

        // Optimize allocations.
        if ilThisConvention = ILThisConvention.Instance && ilArgConvention = ILArgConvention.Default then
            ILCallingConv.Instance
        elif ilThisConvention = ILThisConvention.Static && ilArgConvention = ILArgConvention.Default then
            ILCallingConv.Static
        else
            ILCallingConv.Callconv(ilThisConvention, ilArgConvention)

    let mkPInvokeCallingConvention (methImportAttributes: MethodImportAttributes) =
        match methImportAttributes &&& MethodImportAttributes.CallingConventionMask with
        | MethodImportAttributes.CallingConventionCDecl ->
            PInvokeCallingConvention.Cdecl
        | MethodImportAttributes.CallingConventionStdCall ->
            PInvokeCallingConvention.Stdcall
        | MethodImportAttributes.CallingConventionThisCall ->
            PInvokeCallingConvention.Thiscall
        | MethodImportAttributes.CallingConventionFastCall ->
            PInvokeCallingConvention.Fastcall
        | MethodImportAttributes.CallingConventionWinApi ->
            PInvokeCallingConvention.WinApi
        | _ ->
            PInvokeCallingConvention.None

    let mkPInvokeCharEncoding (methImportAttributes: MethodImportAttributes) =
        match methImportAttributes &&& MethodImportAttributes.CharSetMask with
        | MethodImportAttributes.CharSetAnsi ->
            PInvokeCharEncoding.Ansi
        | MethodImportAttributes.CharSetUnicode ->
            PInvokeCharEncoding.Unicode
        | MethodImportAttributes.CharSetAuto ->
            PInvokeCharEncoding.Auto
        | _ ->
            PInvokeCharEncoding.None

    let mkPInvokeThrowOnUnmappableChar (methImportAttrs: MethodImportAttributes) =
        match methImportAttrs &&& MethodImportAttributes.ThrowOnUnmappableCharMask with
        | MethodImportAttributes.ThrowOnUnmappableCharEnable ->
            PInvokeThrowOnUnmappableChar.Enabled
        | MethodImportAttributes.ThrowOnUnmappableCharDisable ->
            PInvokeThrowOnUnmappableChar.Disabled
        | _ -> 
            PInvokeThrowOnUnmappableChar.UseAssembly

    let mkPInvokeCharBestFit (methImportAttrs: MethodImportAttributes) =
        match methImportAttrs &&& MethodImportAttributes.BestFitMappingMask with
        | MethodImportAttributes.BestFitMappingEnable ->
            PInvokeCharBestFit.Enabled
        | MethodImportAttributes.BestFitMappingDisable ->
            PInvokeCharBestFit.Disabled
        | _ ->
            PInvokeCharBestFit.UseAssembly

    let mkILTypeFunctionPointer (sigHeader: SignatureHeader) argTypes returnType =
        let callingSig =
            {
                CallingConv = mkILCallingConv sigHeader
                ArgTypes = argTypes
                ReturnType = returnType
            }
        ILType.FunctionPointer(callingSig)

    let mkILTypeTypeVar index =
        ILType.TypeVar(uint16 index)

    let mkILTypeModified isRequired typeRef unmodifiedType =
        ILType.Modified(isRequired, typeRef, unmodifiedType)
        
    let mkILTypePrimitive (primitiveTypeCode: PrimitiveTypeCode) (ilg: ILGlobals) =
        match primitiveTypeCode with
        | PrimitiveTypeCode.Boolean -> ilg.typ_Bool
        | PrimitiveTypeCode.Byte -> ilg.typ_Byte
        | PrimitiveTypeCode.Char -> ilg.typ_Char
        | PrimitiveTypeCode.Double -> ilg.typ_Double
        | PrimitiveTypeCode.Int16 -> ilg.typ_Int16
        | PrimitiveTypeCode.Int32 -> ilg.typ_Int32
        | PrimitiveTypeCode.Int64 -> ilg.typ_Int64
        | PrimitiveTypeCode.IntPtr -> ilg.typ_IntPtr
        | PrimitiveTypeCode.Object -> ilg.typ_Object
        | PrimitiveTypeCode.SByte -> ilg.typ_SByte
        | PrimitiveTypeCode.Single -> ilg.typ_Single
        | PrimitiveTypeCode.String -> ilg.typ_String
        | PrimitiveTypeCode.TypedReference -> ilg.typ_TypedReference
        | PrimitiveTypeCode.UInt16 -> ilg.typ_UInt16
        | PrimitiveTypeCode.UInt32 -> ilg.typ_UInt32
        | PrimitiveTypeCode.UInt64 -> ilg.typ_UInt64
        | PrimitiveTypeCode.UIntPtr -> ilg.typ_UIntPtr
        | PrimitiveTypeCode.Void -> ILType.Void
        | _ -> failwithf "Invalid Primitive Type Code: %A" primitiveTypeCode

    let mkILTypeGeneric typeRef boxity typeArgs =
        let ilTypeSpec = ILTypeSpec.Create(typeRef, typeArgs)
        mkILTy boxity ilTypeSpec

    let mkILTypeArray elementType (shape: ArrayShape) =
        let lowerBounds = shape.LowerBounds
        let sizes = shape.Sizes
        let rank = shape.Rank
        let shape = 
            let dim i =
              (if i < lowerBounds.Length then Some (Seq.item i lowerBounds) else None), 
              (if i < sizes.Length then Some (Seq.item i sizes) else None)
            ILArrayShape (List.init rank dim)
        mkILArrTy (elementType, shape)

    [<Sealed>]
    type SignatureTypeProvider(ilg: ILGlobals) =

        member val cenv : cenv = Unchecked.defaultof<_> with get, set

        interface ISignatureTypeProvider<ILType, unit> with

            member _.GetFunctionPointerType(si) =
                mkILTypeFunctionPointer si.Header (si.ParameterTypes |> Seq.toList) si.ReturnType

            member _.GetGenericMethodParameter(_, index) =
                mkILTypeTypeVar index

            member _.GetGenericTypeParameter(_, index) =
                mkILTypeTypeVar index

            member _.GetModifiedType(modifier, unmodifiedType, isRequired) =
                mkILTypeModified isRequired modifier.TypeRef unmodifiedType

            member _.GetPinnedType(elementType) = elementType

            member this.GetTypeFromSpecification(_, _, typeSpecHandle, _) =
                readILTypeFromTypeSpecification this.cenv typeSpecHandle
            
        interface ISimpleTypeProvider<ILType> with

            member _.GetPrimitiveType(typeCode) =
                mkILTypePrimitive typeCode ilg
            
            member this.GetTypeFromDefinition(_, typeDefHandle, _) =
                readILTypeFromTypeDefinition this.cenv typeDefHandle

            member this.GetTypeFromReference(_, typeRefHandle, _) =
                readILTypeFromTypeReference this.cenv typeRefHandle

        interface IConstructedTypeProvider<ILType> with

            member _.GetGenericInstantiation(genericType, typeArgs) =
                mkILTypeGeneric genericType.TypeRef genericType.Boxity (typeArgs |> List.ofSeq)

            member _.GetArrayType(elementType, shape) =
                mkILTypeArray elementType shape

            member _.GetByReferenceType(elementType) =
                ILType.Byref(elementType)

            member _.GetPointerType(elementType) =
                ILType.Ptr(elementType)

        interface ISZArrayTypeProvider<ILType> with

            member _.GetSZArrayType(elementType) =
                mkILArr1DTy elementType

    [<Sealed>]
    type LocalSignatureTypeProvider(ilg: ILGlobals) =

        member val cenv : cenv = Unchecked.defaultof<_> with get, set

        interface ISignatureTypeProvider<ILLocal, unit> with

            member _.GetFunctionPointerType(si) =
                {
                    IsPinned = false
                    Type = mkILTypeFunctionPointer si.Header (si.ParameterTypes |> Seq.map (fun x -> x.Type) |> Seq.toList) si.ReturnType.Type
                    DebugInfo = None
                }

            member _.GetGenericMethodParameter(_, index) =
                {
                    IsPinned = false
                    Type = mkILTypeTypeVar index
                    DebugInfo = None
                }

            member _.GetGenericTypeParameter(_, index) =
                {
                    IsPinned = false
                    Type = mkILTypeTypeVar index
                    DebugInfo = None
                }

            member _.GetModifiedType(modifier, unmodifiedType, isRequired) =
                {
                    IsPinned = false
                    Type = mkILTypeModified isRequired modifier.Type.TypeRef unmodifiedType.Type
                    DebugInfo = None
                }

            member _.GetPinnedType(elementType) =
                {
                    IsPinned = true
                    Type = elementType.Type
                    DebugInfo = None
                }

            member this.GetTypeFromSpecification(_, _, typeSpecHandle, _) =
                {
                    IsPinned = false
                    Type = readILTypeFromTypeSpecification this.cenv typeSpecHandle
                    DebugInfo = None
                }
            
        interface ISimpleTypeProvider<ILLocal> with

            member _.GetPrimitiveType(typeCode) =
                {
                    IsPinned = false
                    Type = mkILTypePrimitive typeCode ilg
                    DebugInfo = None
                }
            
            member this.GetTypeFromDefinition(_, typeDefHandle, _) =
                {
                    IsPinned = false
                    Type = readILTypeFromTypeDefinition this.cenv typeDefHandle
                    DebugInfo = None
                }    

            member this.GetTypeFromReference(_, typeRefHandle, _) =
                {
                    IsPinned = false
                    Type = readILTypeFromTypeReference this.cenv typeRefHandle
                    DebugInfo = None
                } 
            
        interface IConstructedTypeProvider<ILLocal> with

            member _.GetGenericInstantiation(genericType, typeArgs) =
                {
                    IsPinned = false
                    Type = mkILTypeGeneric genericType.Type.TypeRef genericType.Type.Boxity (typeArgs |> Seq.map (fun x -> x.Type) |> List.ofSeq)
                    DebugInfo = None
                }

            member _.GetArrayType(elementType, shape) =
                {
                    IsPinned = false
                    Type = mkILTypeArray elementType.Type shape
                    DebugInfo = None
                }

            member _.GetByReferenceType(elementType) =
                {
                    IsPinned = false
                    Type = ILType.Byref(elementType.Type)
                    DebugInfo = None
                }

            member _.GetPointerType(elementType) =
                {
                    IsPinned = false
                    Type = ILType.Ptr(elementType.Type)
                    DebugInfo = None
                }

        interface ISZArrayTypeProvider<ILLocal> with

            member _.GetSZArrayType(elementType) =
                {
                    IsPinned = false
                    Type =  mkILArr1DTy elementType.Type
                    DebugInfo = None
                }

    let rec readILScopeRef (cenv: cenv) (handle: EntityHandle) =
        let mdReader = cenv.MetadataReader

        match handle.Kind with
        | HandleKind.AssemblyFile ->
            let asmFile = mdReader.GetAssemblyFile(AssemblyFileHandle.op_Explicit(handle))
            ILScopeRef.Module(readILModuleRefFromAssemblyFile cenv asmFile)

        | HandleKind.AssemblyReference ->
            ILScopeRef.Assembly(readILAssemblyRefFromAssemblyReference cenv (AssemblyReferenceHandle.op_Explicit(handle)))

        | HandleKind.ModuleReference ->
            let modRef = mdReader.GetModuleReference(ModuleReferenceHandle.op_Explicit(handle))
            ILScopeRef.Module(readILModuleRefFromModuleReference cenv modRef)

        | HandleKind.TypeReference ->
            let typeRef = mdReader.GetTypeReference(TypeReferenceHandle.op_Explicit(handle))
            readILScopeRef cenv typeRef.ResolutionScope

        | HandleKind.ModuleDefinition ->
            ILScopeRef.Local

        | _ ->
            failwithf "Invalid Handle Kind: %A" handle.Kind

    let readILAssemblyRefFromAssemblyReferenceUncached (cenv: cenv) (asmRefHandle: AssemblyReferenceHandle) =
        let mdReader = cenv.MetadataReader

        let asmRef = mdReader.GetAssemblyReference(asmRefHandle)
        let name = mdReader.GetString(asmRef.Name)
        let flags = asmRef.Flags

        let hash = 
            let hashValue = asmRef.HashValue
            if hashValue.IsNil then None
            else Some(mdReader.GetBlobBytes(hashValue))

        let publicKey =
            if asmRef.PublicKeyOrToken.IsNil then None
            else 
                let bytes = mdReader.GetBlobBytes(asmRef.PublicKeyOrToken)
                let publicKey = 
                    if int (flags &&& AssemblyFlags.PublicKey) <> 0 then
                        PublicKey(bytes)
                    else
                        PublicKeyToken(bytes)
                Some(publicKey)

        let retargetable = int (flags &&& AssemblyFlags.Retargetable) <> 0

        let version = mkILVersionInfo asmRef.Version

        let locale =
            let locale = mdReader.GetString(asmRef.Culture)
            if String.IsNullOrWhiteSpace(locale) then None
            else Some(locale)

        ILAssemblyRef.Create(name, hash, publicKey, retargetable, Some version, locale)

    let readILAssemblyRefFromAssemblyReference (cenv: cenv) (asmRefHandle: AssemblyReferenceHandle) =
        match cenv.TryGetCachedILAssemblyRef(asmRefHandle) with
        | ValueSome(ilAsmRef) -> ilAsmRef
        | _ ->
            let ilAsmRef = readILAssemblyRefFromAssemblyReferenceUncached cenv asmRefHandle
            cenv.CacheILAssemblyRef(asmRefHandle, ilAsmRef)
            ilAsmRef

    let readILModuleRefFromAssemblyFile (cenv: cenv) (asmFile: AssemblyFile) =
        let mdReader = cenv.MetadataReader

        let name = mdReader.GetString(asmFile.Name)

        let hash = 
            let hashValue = asmFile.HashValue
            if hashValue.IsNil then None
            else Some(mdReader.GetBlobBytes(hashValue))

        ILModuleRef.Create(name, asmFile.ContainsMetadata, hash) 

    let readILType (cenv: cenv) (handle: EntityHandle) : ILType =
        match handle.Kind with
        | HandleKind.TypeReference ->
            readILTypeFromTypeReference cenv (TypeReferenceHandle.op_Explicit(handle))

        | HandleKind.TypeDefinition ->
            readILTypeFromTypeDefinition cenv (TypeDefinitionHandle.op_Explicit(handle))

        | HandleKind.TypeSpecification ->
            readILTypeFromTypeSpecification cenv (TypeSpecificationHandle.op_Explicit(handle))

        | _ ->
            failwithf "Invalid Handle Kind: %A" handle.Kind

    let readILModuleRefFromModuleReference (cenv: cenv) (modRef: ModuleReference) =
        let name = cenv.MetadataReader.GetString(modRef.Name)
        ILModuleRef.Create(name, hasMetadata = true, hash = None)

    let readILTypeRefFromTypeReference (cenv: cenv) (typeRef: TypeReference) =
        let mdReader = cenv.MetadataReader

        let ilScopeRef = readILScopeRef cenv typeRef.ResolutionScope

        let name = mdReader.GetString(typeRef.Name)
        let namespac = mdReader.GetString(typeRef.Namespace)

        ILTypeRef.Create(ilScopeRef, [], namespac + "." + name)

    let readILTypeFromTypeReference (cenv: cenv) (typeRefHandle: TypeReferenceHandle) =
        match cenv.TryGetCachedILType(typeRefHandle) with
        | ValueSome(ilType) -> ilType
        | _ ->
            let mdReader = cenv.MetadataReader

            let typeRef = mdReader.GetTypeReference(typeRefHandle)
            let ilTypeRef = readILTypeRefFromTypeReference cenv typeRef
            let ilTypeSpec = ILTypeSpec.Create(ilTypeRef, ILGenericArgs.Empty)

            let ilType = mkILTy AsObject (* AsObject probably not nok *) ilTypeSpec
            cenv.CacheILType(typeRefHandle, ilType)
            ilType

    let rec readILTypeRefFromTypeDefinition (cenv: cenv) (typeDef: TypeDefinition) : ILTypeRef =
        let mdReader = cenv.MetadataReader

        let enclosing =
            if typeDef.IsNested then
                let parentTypeDefHandle = typeDef.GetDeclaringType()
                let parentTypeDef = mdReader.GetTypeDefinition(parentTypeDefHandle)
                let ilTypeRef = readILTypeRefFromTypeDefinition cenv parentTypeDef
                ilTypeRef.Enclosing @ [ ilTypeRef.Name ]
            else
                []
  
        let name =
            let name = mdReader.GetString(typeDef.Name)
            if enclosing.Length > 0 then 
                name
            else
                let namespac = mdReader.GetString(typeDef.Namespace)
                namespac + "." + name

        ILTypeRef.Create(ILScopeRef.Local, enclosing, name)

    let readILTypeFromTypeDefinition (cenv: cenv) (typeDefHandle: TypeDefinitionHandle) =
        match cenv.TryGetCachedILType(typeDefHandle) with
        | ValueSome(ilType) -> ilType
        | _ ->
            let mdReader = cenv.MetadataReader

            let typeDef = mdReader.GetTypeDefinition(typeDefHandle)
            let ilTypeRef = readILTypeRefFromTypeDefinition cenv typeDef

            let ilGenericArgs = 
                let enclILGenericArgCount = typeDef.GetGenericParameters().Count
                typeDef.GetGenericParameters()
                |> Seq.mapi (fun i _ -> mkILTyvarTy (uint16 (enclILGenericArgCount + i)))
                |> List.ofSeq

            let ilTypeSpec = ILTypeSpec.Create(ilTypeRef, ilGenericArgs)

            let boxity = 
                if int (typeDef.Attributes &&& TypeAttributes.Class) <> 0 then AsObject
                else AsValue

            let ilType = mkILTy boxity ilTypeSpec
            cenv.CacheILType(typeDefHandle, ilType)
            ilType

    let readILTypeFromTypeSpecification (cenv: cenv) (typeSpecHandle: TypeSpecificationHandle) =
        match cenv.TryGetCachedILType(typeSpecHandle) with
        | ValueSome(ilType) -> ilType
        | _ ->
            let mdReader = cenv.MetadataReader

            let typeSpec = mdReader.GetTypeSpecification(typeSpecHandle)

            let ilType = typeSpec.DecodeSignature(cenv.SignatureTypeProvider, ())
            cenv.CacheILType(typeSpecHandle, ilType)
            ilType

    let readILGenericParameterDef (cenv: cenv) (genParamHandle: GenericParameterHandle) : ILGenericParameterDef =
        let mdReader = cenv.MetadataReader

        let genParam = mdReader.GetGenericParameter(genParamHandle)
        let attributes = genParam.Attributes

        let constraints =
            genParam.GetConstraints()
            |> Seq.map (fun genParamCnstrHandle ->
                let genParamCnstr = mdReader.GetGenericParameterConstraint(genParamCnstrHandle)
                readILType cenv genParamCnstr.Type
            )
            |> List.ofSeq     

        let variance = 
            if int (attributes &&& GenericParameterAttributes.Covariant) <> 0 then
                ILGenericVariance.CoVariant
            elif int (attributes &&& GenericParameterAttributes.Contravariant) <> 0 then
                ILGenericVariance.ContraVariant
            else
                ILGenericVariance.NonVariant

        {
            Name = mdReader.GetString(genParam.Name)
            Constraints = constraints
            Variance = variance
            HasReferenceTypeConstraint = int (attributes &&& GenericParameterAttributes.ReferenceTypeConstraint) <> 0
            HasNotNullableValueTypeConstraint = int (attributes &&& GenericParameterAttributes.NotNullableValueTypeConstraint) <> 0
            HasDefaultConstructorConstraint = int (attributes &&& GenericParameterAttributes.DefaultConstructorConstraint) <> 0
            CustomAttrsStored = readILAttributesStored cenv (genParam.GetCustomAttributes())
            MetadataIndex = MetadataTokens.GetRowNumber(GenericParameterHandle.op_Implicit(genParamHandle))
        }

    let readILGenericParameterDefs (cenv: cenv) (genParamHandles: GenericParameterHandleCollection) =
        genParamHandles
        |> Seq.map (readILGenericParameterDef cenv)
        |> List.ofSeq

    let rec readDeclaringTypeInfoFromMemberOrMethod (cenv: cenv) (handle: EntityHandle) : string * ILType =
        let mdReader = cenv.MetadataReader
        match handle.Kind with
        | HandleKind.MemberReference ->
            let memberRef = mdReader.GetMemberReference(MemberReferenceHandle.op_Explicit(handle))
            (mdReader.GetString(memberRef.Name), readILType cenv memberRef.Parent)

        | HandleKind.MethodDefinition ->
            let methodDef = mdReader.GetMethodDefinition(MethodDefinitionHandle.op_Explicit(handle))
            (mdReader.GetString(methodDef.Name), readILTypeFromTypeDefinition cenv (methodDef.GetDeclaringType()))

        | HandleKind.MethodSpecification ->
            let methodSpec = mdReader.GetMethodSpecification(MethodSpecificationHandle.op_Explicit(handle))
            readDeclaringTypeInfoFromMemberOrMethod cenv methodSpec.Method

        | _ ->
            failwithf "Invalid Entity Handle Kind: %A" handle.Kind

    let readILMethodSpecFromMemberReferencedUncached (cenv: cenv) (memberRefHandle: MemberReferenceHandle) =
        let mdReader = cenv.MetadataReader

        let memberRef = mdReader.GetMemberReference(memberRefHandle)
        let si = memberRef.DecodeMethodSignature(cenv.SignatureTypeProvider, ())

        let name = mdReader.GetString(memberRef.Name)
        let enclILTy = readILType cenv memberRef.Parent
        let ilCallingConv = mkILCallingConv si.Header
        let genericArity = si.GenericParameterCount

        let ilMethodRef = ILMethodRef.Create(enclILTy.TypeRef, ilCallingConv, name, genericArity, si.ParameterTypes |> List.ofSeq, si.ReturnType)

        ILMethodSpec.Create(enclILTy, ilMethodRef, [])

    let readILMethodSpecFromMemberReference (cenv: cenv) (memberRefHandle: MemberReferenceHandle) =
        match cenv.TryGetCachedILMethodSpec(memberRefHandle) with
        | ValueSome(ilMethSpec) -> ilMethSpec
        | _ ->
            let ilMethSpec = readILMethodSpecFromMemberReferencedUncached cenv memberRefHandle
            cenv.CacheILMethodSpec(memberRefHandle, ilMethSpec)
            ilMethSpec

    let readILMethodSpecFromMethodDefinitionUncached (cenv: cenv) (methDefHandle: MethodDefinitionHandle) =
        let mdReader = cenv.MetadataReader

        let methodDef = mdReader.GetMethodDefinition(methDefHandle)
        let si = methodDef.DecodeSignature(cenv.SignatureTypeProvider, ())

        let name = mdReader.GetString(methodDef.Name)
        let enclILTy = readILTypeFromTypeDefinition cenv (methodDef.GetDeclaringType())
        let ilCallingConv =
            if si.Header.IsInstance then
                ILCallingConv.Instance
            else
                ILCallingConv.Static
        let genericArity = si.GenericParameterCount

        let ilMethodRef = ILMethodRef.Create(enclILTy.TypeRef, ilCallingConv, name, genericArity, si.ParameterTypes |> List.ofSeq, si.ReturnType)

        let ilGenericArgs =
            let enclILGenericArgCount = enclILTy.GenericArgs.Length
            methodDef.GetGenericParameters()
            |> Seq.mapi (fun i _ -> mkILTyvarTy (uint16 (enclILGenericArgCount + i)))
            |> List.ofSeq

        ILMethodSpec.Create(enclILTy, ilMethodRef, ilGenericArgs)

    let readILMethodSpecFromMethodDefinition (cenv: cenv) (methDefHandle: MethodDefinitionHandle) =
        match cenv.TryGetCachedILMethodSpec(methDefHandle) with
        | ValueSome(ilMethSpec) -> ilMethSpec
        | _ ->
            let ilMethSpec = readILMethodSpecFromMethodDefinitionUncached cenv methDefHandle
            cenv.CacheILMethodSpec(methDefHandle, ilMethSpec)
            ilMethSpec

    let rec readILMethodSpec (cenv: cenv) (handle: EntityHandle) : ILMethodSpec =
        let mdReader = cenv.MetadataReader
        match handle.Kind with
        | HandleKind.MemberReference ->
            readILMethodSpecFromMemberReference cenv (MemberReferenceHandle.op_Explicit(handle))

        | HandleKind.MethodDefinition ->
            readILMethodSpecFromMethodDefinition cenv (MethodDefinitionHandle.op_Explicit(handle))

        | HandleKind.MethodSpecification ->
            let methodSpec = mdReader.GetMethodSpecification(MethodSpecificationHandle.op_Explicit(handle))
            readILMethodSpec cenv methodSpec.Method

        | _ ->
            failwithf "Invalid Entity Handle Kind: %A" handle.Kind

    let readILSecurityDecl (cenv: cenv) (declSecurityAttributeHandle: DeclarativeSecurityAttributeHandle) =
        let mdReader = cenv.MetadataReader

        let declSecurityAttribute = mdReader.GetDeclarativeSecurityAttribute(declSecurityAttributeHandle)

        let bytes = mdReader.GetBlobBytes(declSecurityAttribute.PermissionSet)
        ILSecurityDecl(mkILSecurityAction declSecurityAttribute.Action, bytes)

    let readILSecurityDeclsStored (cenv: cenv) (declSecurityAttributeHandles: DeclarativeSecurityAttributeHandleCollection) =
        mkILSecurityDeclsReader (fun _ ->
            let securityDeclsArray = Array.zeroCreate declSecurityAttributeHandles.Count
            let mutable i = 0
            for declSecurityAttributeHandle in declSecurityAttributeHandles do
                securityDeclsArray.[i] <- readILSecurityDecl cenv declSecurityAttributeHandle
                i <- i + 1
            securityDeclsArray
        )

    let readILAttribute (cenv: cenv) (customAttrHandle: CustomAttributeHandle) =
        let mdReader = cenv.MetadataReader
        let customAttr = mdReader.GetCustomAttribute(customAttrHandle)

        let bytes = 
            if customAttr.Value.IsNil then [||]
            else mdReader.GetBlobBytes(customAttr.Value)

        let elements = [] // Why are we not putting elements in here?
        ILAttribute.Encoded(readILMethodSpec cenv customAttr.Constructor, bytes, elements)

    let readILAttributesStored (cenv: cenv) (customAttrs: CustomAttributeHandleCollection) =
        mkILCustomAttrsReader (fun _ ->
            let customAttrsArray = Array.zeroCreate customAttrs.Count
            let mutable i = 0
            for customAttrHandle in customAttrs do
                customAttrsArray.[i] <- readILAttribute cenv customAttrHandle
                i <- i + 1
            customAttrsArray
        )

    let readILNestedExportedType (cenv: cenv) nested (exportedTyHandle: ExportedTypeHandle) =
        let mdReader = cenv.MetadataReader

        let exportedTy = mdReader.GetExportedType(exportedTyHandle)

        let name = mdReader.GetTypeName(exportedTy.Namespace, exportedTy.Name)

        {
            Name = name
            Access = mkILMemberAccess exportedTy.Attributes
            Nested = realILNestedExportedTypes cenv nested exportedTyHandle
            CustomAttrsStored = readILAttributesStored cenv (exportedTy.GetCustomAttributes())
            MetadataIndex = MetadataTokens.GetRowNumber(ExportedTypeHandle.op_Implicit(exportedTyHandle))
        }

    let realILNestedExportedTypes (cenv: cenv) (nested: ReadOnlyDictionary<ExportedTypeHandle, ResizeArray<ExportedTypeHandle>>) (parentExportedTyHandle: ExportedTypeHandle) =
        match nested.TryGetValue parentExportedTyHandle with
        | true, nestedTys ->
            nestedTys
            |> Seq.map (fun x -> readILNestedExportedType cenv nested x)
            |> List.ofSeq
            |> mkILNestedExportedTypes
        | _ ->
            mkILNestedExportedTypes List.empty

    let readILExportedType (cenv: cenv) (nested: ReadOnlyDictionary<ExportedTypeHandle, ResizeArray<ExportedTypeHandle>>) (exportedTyHandle: ExportedTypeHandle) =
        let mdReader = cenv.MetadataReader

        let exportedTy = mdReader.GetExportedType(exportedTyHandle)

        let name = mdReader.GetTypeName(exportedTy.Namespace, exportedTy.Name)

        {
            ScopeRef = readILScopeRef cenv exportedTy.Implementation
            Name = name
            Attributes = exportedTy.Attributes
            Nested = realILNestedExportedTypes cenv nested exportedTyHandle
            CustomAttrsStored = readILAttributesStored cenv (exportedTy.GetCustomAttributes())
            MetadataIndex = MetadataTokens.GetRowNumber(ExportedTypeHandle.op_Implicit(exportedTyHandle))
        }

    let readILExportedTypes (cenv: cenv) (exportedTys: ExportedTypeHandleCollection) =
        let mdReader = cenv.MetadataReader
        let nested =
            lazy
                let lookup = Dictionary<ExportedTypeHandle, ResizeArray<ExportedTypeHandle>>()

                for exportedTyHandle in exportedTys do
                    let exportedTy = mdReader.GetExportedType(exportedTyHandle)
                    let access = mkILTypeDefAccess exportedTy.Attributes
                    if not (access = ILTypeDefAccess.Public || access = ILTypeDefAccess.Private) && exportedTy.Implementation.Kind = HandleKind.ExportedType then
                        let parentExportedTyHandle = ExportedTypeHandle.op_Explicit exportedTy.Implementation
                        let nested =
                            match lookup.TryGetValue parentExportedTyHandle with
                            | true, nested -> nested
                            | _ ->
                                let nested = ResizeArray()
                                lookup.[parentExportedTyHandle] <- nested
                                nested
                        nested.Add exportedTyHandle

                ReadOnlyDictionary lookup
        let f =
            lazy
                let nested = nested.Value
                [
                    for exportedTyHandle in exportedTys do
                        let exportedTy = mdReader.GetExportedType(exportedTyHandle)
                        let access = mkILTypeDefAccess exportedTy.Attributes
                        // Not a nested type
                        if (access = ILTypeDefAccess.Public || access = ILTypeDefAccess.Private) && exportedTy.Implementation.Kind <> HandleKind.ExportedType then
                            yield readILExportedType cenv nested exportedTyHandle
                ]
        mkILExportedTypesLazy f

    let readILAssemblyManifest (cenv: cenv) (entryPointToken: int) =
        let mdReader = cenv.MetadataReader

        let asmDef = mdReader.GetAssemblyDefinition()

        let publicKey =
            let bytes = 
                asmDef.PublicKey
                |> mdReader.GetBlobBytes
            if bytes.Length = 0 then None
            else Some(bytes)

        let locale =
            let str =
                asmDef.Culture
                |> mdReader.GetString
            if str.Length = 0 then None
            else Some(str)

        let flags = asmDef.Flags

        let entrypointElsewhere =
            let handle = MetadataTokens.EntityHandle(entryPointToken)
            if handle.IsNil then None
            else
                match handle.Kind with
                | HandleKind.AssemblyFile -> 
                    let asmFile = mdReader.GetAssemblyFile(AssemblyFileHandle.op_Explicit(handle))
                    Some(readILModuleRefFromAssemblyFile cenv asmFile)
                | _ -> None

        {
            Name = mdReader.GetString(asmDef.Name)
            AuxModuleHashAlgorithm = int asmDef.HashAlgorithm
            SecurityDeclsStored = readILSecurityDeclsStored cenv (asmDef.GetDeclarativeSecurityAttributes())
            PublicKey = publicKey
            Version = Some(mkILVersionInfo asmDef.Version)
            Locale = locale
            CustomAttrsStored = readILAttributesStored cenv (asmDef.GetCustomAttributes())
            AssemblyLongevity = mkILAssemblyLongevity flags
            DisableJitOptimizations = int (flags &&& AssemblyFlags.DisableJitCompileOptimizer) <> 0
            JitTracking = int (flags &&& AssemblyFlags.EnableJitCompileTracking) <> 0
            IgnoreSymbolStoreSequencePoints = (int flags &&& 0x2000) <> 0 // Not listed in AssemblyFlags
            Retargetable = int (flags &&& AssemblyFlags.Retargetable) <> 0
            ExportedTypes = readILExportedTypes cenv mdReader.ExportedTypes
            EntrypointElsewhere = entrypointElsewhere
            MetadataIndex = 1 // always one
        }

    let readILNativeResources (peReader: PEReader) =
        peReader.PEHeaders.SectionHeaders
        |> Seq.choose (fun s ->
            if s.Name.Equals(".rsrc", StringComparison.OrdinalIgnoreCase) then
                let memBlock = peReader.GetSectionData(s.VirtualAddress)
                // REVIEW: We should not read the entire raw bytes.
                let bytes = memBlock.GetContent().ToArray()
                ILNativeResource.Out(bytes)
                |> Some
            else
                None
        )
        |> Seq.toList

    let tryReadILFieldInit (cenv: cenv) (constantHandle: ConstantHandle) =
        if constantHandle.IsNil then None
        else
            let mdReader = cenv.MetadataReader

            let constant = mdReader.GetConstant(constantHandle)
            let blobReader = mdReader.GetBlobReader(constant.Value)
            match constant.TypeCode with
            | ConstantTypeCode.Boolean -> ILFieldInit.Bool(blobReader.ReadBoolean()) |> Some
            | ConstantTypeCode.Byte -> ILFieldInit.UInt8(blobReader.ReadByte()) |> Some
            | ConstantTypeCode.Char -> ILFieldInit.Char(blobReader.ReadChar() |> uint16) |> Some // Why does ILFieldInit.Char not just take a char?
            | ConstantTypeCode.Double -> ILFieldInit.Double(blobReader.ReadDouble()) |> Some
            | ConstantTypeCode.Int16 -> ILFieldInit.Int16(blobReader.ReadInt16()) |> Some
            | ConstantTypeCode.Int32 -> ILFieldInit.Int32(blobReader.ReadInt32()) |> Some
            | ConstantTypeCode.Int64 -> ILFieldInit.Int64(blobReader.ReadInt64()) |> Some
            | ConstantTypeCode.SByte -> ILFieldInit.Int8(blobReader.ReadSByte()) |> Some
            | ConstantTypeCode.Single -> ILFieldInit.Single(blobReader.ReadSingle()) |> Some
            | ConstantTypeCode.String -> ILFieldInit.String(blobReader.ReadUTF16(blobReader.Length)) |> Some
            | ConstantTypeCode.UInt16 -> ILFieldInit.UInt16(blobReader.ReadUInt16()) |> Some
            | ConstantTypeCode.UInt32 -> ILFieldInit.UInt32(blobReader.ReadUInt32()) |> Some
            | ConstantTypeCode.UInt64 -> ILFieldInit.UInt64(blobReader.ReadUInt64()) |> Some
            | ConstantTypeCode.NullReference -> ILFieldInit.Null |> Some
            | _ -> (* possible warning? *) None

    let ilNativeTypeLookup = (ILNativeTypeMap.Value |> Seq.map (fun x -> x)).ToDictionary((fun (key, _) -> key), fun (_, value) -> value) // This looks terrible. Cleanup later.
    let ilVariantTypeMap = (ILVariantTypeMap.Value |> Seq.map (fun x -> x)).ToDictionary((fun (_, key) -> key), fun (value, _) -> value) // This looks terrible. Cleanup later.

    let rec mkILVariantType (kind: int) =
        match ilVariantTypeMap.TryGetValue(kind) with
        | true, ilVariantType -> ilVariantType
        | _ ->
            match kind with
            | _ when (kind &&& vt_ARRAY) <> 0 -> ILNativeVariant.Array(mkILVariantType (kind &&& (~~~vt_ARRAY)))
            | _ when (kind &&& vt_VECTOR) <> 0 -> ILNativeVariant.Vector(mkILVariantType (kind &&& (~~~vt_VECTOR)))
            | _ when (kind &&& vt_BYREF) <> 0 -> ILNativeVariant.Byref(mkILVariantType (kind &&& (~~~vt_BYREF)))
            | _ -> (* possible warning? *) ILNativeVariant.Empty

    let rec readILNativeType (cenv: cenv) (reader: byref<BlobReader>) =
        let kind = reader.ReadByte()
        match ilNativeTypeLookup.TryGetValue(kind) with
        | true, ilNativeType -> ilNativeType
        | _ ->
            match kind with
            | 0x0uy -> ILNativeType.Empty
            | _ when kind = nt_FIXEDSYSSTRING -> ILNativeType.FixedSysString(reader.ReadCompressedInteger())
            | _ when kind = nt_FIXEDARRAY -> ILNativeType.FixedArray(reader.ReadCompressedInteger())

            | _ when kind = nt_SAFEARRAY ->
                if reader.RemainingBytes = 0 then
                    ILNativeType.SafeArray(ILNativeVariant.Empty, None)
                else
                    let variantKind = reader.ReadCompressedInteger()
                    let ilVariantType = mkILVariantType variantKind
                    if reader.RemainingBytes = 0 then
                        ILNativeType.SafeArray(ilVariantType, None)
                    else
                        let s = reader.ReadUTF16(reader.Length)
                        ILNativeType.SafeArray(ilVariantType, Some(s))

            | _ when kind = nt_ARRAY ->
                if reader.RemainingBytes = 0 then
                    ILNativeType.Array(None, None)
                else
                    let nt = 
                        let oldReader = reader
                        let u = reader.ReadCompressedInteger() // What is 'u'?
                        if u = int nt_MAX then // What is this doing?
                            ILNativeType.Empty
                        else
                            // NOTE: go back to start and read native type
                            reader <- oldReader
                            readILNativeType cenv &reader
              
                    if reader.RemainingBytes = 0 then
                        ILNativeType.Array(Some(nt), None)
                    else
                        let pnum = reader.ReadCompressedInteger()
                        if reader.RemainingBytes = 0 then
                            ILNativeType.Array(Some(nt), Some(pnum, None))
                        else
                            let additive = reader.ReadCompressedInteger()
                            ILNativeType.Array(Some(nt), Some(pnum, Some(additive)))

            | _ when kind = nt_CUSTOMMARSHALER ->
                let guid = reader.ReadBytes(reader.ReadCompressedInteger())
                let nativeTypeName = reader.ReadUTF16(reader.ReadCompressedInteger())
                let custMarshallerName = reader.ReadUTF16(reader.ReadCompressedInteger())
                let cookieString = reader.ReadBytes(reader.ReadCompressedInteger())
                ILNativeType.Custom(guid, nativeTypeName, custMarshallerName, cookieString)

            | _ -> ILNativeType.Empty

    let tryReadILNativeType (cenv: cenv) (marshalDesc: BlobHandle) =
        if marshalDesc.IsNil then None
        else
            let mdReader = cenv.MetadataReader

            let mutable (* it doesn't have to be mutable, but it's best practice for .NET structs *) reader = mdReader.GetBlobReader(marshalDesc)
            Some(readILNativeType cenv &reader)

    let readILParameter (cenv: cenv) (returnType: ILReturn) (parameters: ILParameter []) (paramHandle: ParameterHandle) : struct(ILParameter * int) =
        let mdReader = cenv.MetadataReader

        let param = mdReader.GetParameter(paramHandle)

        let nameOpt = mdReader.TryGetString(param.Name)

        let typ = 
            if param.SequenceNumber = 0 then returnType.Type
            else parameters.[param.SequenceNumber - 1].Type

        let defaul =
            if int (param.Attributes &&& ParameterAttributes.HasDefault) <> 0 then
                tryReadILFieldInit cenv (param.GetDefaultValue())
            else
                None

        let marshal =
            if int (param.Attributes &&& ParameterAttributes.HasFieldMarshal) <> 0 then 
                tryReadILNativeType cenv (param.GetMarshallingDescriptor())
            else
                None

        let ilParameter =
            {
                Name = match nameOpt with | ValueNone -> None | ValueSome(name) -> Some(name)
                Type = typ
                Default = defaul
                Marshal = marshal
                IsIn = int (param.Attributes &&& ParameterAttributes.In) <> 0
                IsOut = int (param.Attributes &&& ParameterAttributes.Out) <> 0
                IsOptional = int (param.Attributes &&& ParameterAttributes.Optional) <> 0
                CustomAttrsStored = readILAttributesStored cenv (param.GetCustomAttributes())
                MetadataIndex = MetadataTokens.GetRowNumber(ParameterHandle.op_Implicit paramHandle)
            } : ILParameter
        struct(ilParameter, param.SequenceNumber)

    let readILParameters (cenv: cenv) (si: MethodSignature<ILType>) (methDef: MethodDefinition) =
        let ret = ref (mkILReturn si.ReturnType)
        let parameters = [| for paramType in si.ParameterTypes do yield mkILParamAnon paramType |]
        let paramHandles = methDef.GetParameters()

        if paramHandles.Count > 0 then
            paramHandles
            |> Seq.iter (fun paramHandle ->
                let struct(ilParameter, sequenceNumber) = readILParameter cenv !ret parameters paramHandle
                if sequenceNumber = 0 then
                    ret := 
                        { Marshal = ilParameter.Marshal
                          Type = ilParameter.Type
                          CustomAttrsStored = ilParameter.CustomAttrsStored
                          MetadataIndex = ilParameter.MetadataIndex }
                else
                    parameters.[sequenceNumber - 1] <- ilParameter)

        !ret, parameters |> List.ofArray

    // -------------------------------------------------------------------- 
    // IL Instruction reading
    // --------------------------------------------------------------------

    [<NoEquality; NoComparison>]
    type ILOperandPrefixEnv =
        {
            mutable al: ILAlignment
            mutable tl: ILTailcall
            mutable vol: ILVolatility
            mutable ro: ILReadonly
            mutable constrained: ILType option
        }

    let noPrefixes mk prefixes = 
        if prefixes.al <> Aligned then failwith "an unaligned prefix is not allowed here"
        if prefixes.vol <> Nonvolatile then failwith "a volatile prefix is not allowed here"
        if prefixes.tl <> Normalcall then failwith "a tailcall prefix is not allowed here"
        if prefixes.ro <> NormalAddress then failwith "a readonly prefix is not allowed here"
        if prefixes.constrained <> None then failwith "a constrained prefix is not allowed here"
        mk

    let volatileOrUnalignedPrefix mk prefixes = 
        if prefixes.tl <> Normalcall then failwith "a tailcall prefix is not allowed here"
        if prefixes.constrained <> None then failwith "a constrained prefix is not allowed here"
        if prefixes.ro <> NormalAddress then failwith "a readonly prefix is not allowed here"
        mk (prefixes.al, prefixes.vol) 

    let volatilePrefix mk prefixes = 
        if prefixes.al <> Aligned then failwith "an unaligned prefix is not allowed here"
        if prefixes.tl <> Normalcall then failwith "a tailcall prefix is not allowed here"
        if prefixes.constrained <> None then failwith "a constrained prefix is not allowed here"
        if prefixes.ro <> NormalAddress then failwith "a readonly prefix is not allowed here"
        mk prefixes.vol

    let tailPrefix mk prefixes = 
        if prefixes.al <> Aligned then failwith "an unaligned prefix is not allowed here"
        if prefixes.vol <> Nonvolatile then failwith "a volatile prefix is not allowed here"
        if prefixes.constrained <> None then failwith "a constrained prefix is not allowed here"
        if prefixes.ro <> NormalAddress then failwith "a readonly prefix is not allowed here"
        mk prefixes.tl 

    let constraintOrTailPrefix mk prefixes = 
        if prefixes.al <> Aligned then failwith "an unaligned prefix is not allowed here"
        if prefixes.vol <> Nonvolatile then failwith "a volatile prefix is not allowed here"
        if prefixes.ro <> NormalAddress then failwith "a readonly prefix is not allowed here"
        mk (prefixes.constrained, prefixes.tl )

    let readonlyPrefix mk prefixes = 
        if prefixes.al <> Aligned then failwith "an unaligned prefix is not allowed here"
        if prefixes.vol <> Nonvolatile then failwith "a volatile prefix is not allowed here"
        if prefixes.tl <> Normalcall then failwith "a tailcall prefix is not allowed here"
        if prefixes.constrained <> None then failwith "a constrained prefix is not allowed here"
        mk prefixes.ro

    type ILOperandDecoder =
        | NoDecoder
        | InlineNone of (ILOperandPrefixEnv -> ILInstr)
        | ShortInlineVar of (ILOperandPrefixEnv -> sbyte -> ILInstr)
        | ShortInlineI of (ILOperandPrefixEnv -> sbyte -> ILInstr)
        | InlineI of (ILOperandPrefixEnv -> int -> ILInstr)
        | InlineI8 of (ILOperandPrefixEnv -> int64 -> ILInstr)
        | ShortInlineR of (ILOperandPrefixEnv -> single -> ILInstr)
        | InlineR of (ILOperandPrefixEnv -> double -> ILInstr)
        | InlineMethod of (ILOperandPrefixEnv -> ILMethodSpec * ILVarArgs -> ILInstr)
        | InlineSig of (ILOperandPrefixEnv -> ILCallingSignature * ILVarArgs -> ILInstr)
        | ShortInlineBrTarget of (ILOperandPrefixEnv -> sbyte -> ILInstr)
        | InlineBrTarget of (ILOperandPrefixEnv -> int -> ILInstr)
        | InlineSwitch of (ILOperandPrefixEnv -> int list -> ILInstr)
        | InlineType of (ILOperandPrefixEnv -> ILType -> ILInstr)
        | InlineString of (ILOperandPrefixEnv -> string -> ILInstr)
        | InlineField of (ILOperandPrefixEnv -> ILFieldSpec -> ILInstr)
        | InlineTok of (ILOperandPrefixEnv -> ILToken -> ILInstr)
        | InlineVar of (ILOperandPrefixEnv -> uint16 -> ILInstr)

        | PrefixShortInlineI of (ILOperandPrefixEnv -> uint16 -> unit)
        | PrefixInlineNone of (ILOperandPrefixEnv -> unit)
        | PrefixInlineType of (ILOperandPrefixEnv -> ILType -> unit)

    let OneByteDecoders = 
        [|
            InlineNone(noPrefixes AI_nop)//byte OperandType.InlineNone           // nop
            InlineNone(noPrefixes I_break)//byte OperandType.InlineNone           // break
            InlineNone(noPrefixes (I_ldarg(0us)))//byte OperandType.InlineNone           // ldarg.0
            InlineNone(noPrefixes (I_ldarg(1us)))//byte OperandType.InlineNone           // ldarg.1
            InlineNone(noPrefixes (I_ldarg(2us)))//byte OperandType.InlineNone           // ldarg.2
            InlineNone(noPrefixes (I_ldarg(3us)))//byte OperandType.InlineNone           // ldarg.3
            InlineNone(noPrefixes (I_ldloc(0us)))//byte OperandType.InlineNone           // ldloc.0
            InlineNone(noPrefixes (I_ldloc(1us)))//byte OperandType.InlineNone           // ldloc.1
            InlineNone(noPrefixes (I_ldloc(2us)))//byte OperandType.InlineNone           // ldloc.2
            InlineNone(noPrefixes (I_ldloc(3us)))//byte OperandType.InlineNone           // ldloc.3
            InlineNone(noPrefixes (I_stloc(0us)))//byte OperandType.InlineNone           // stloc.0
            InlineNone(noPrefixes (I_stloc(1us)))//byte OperandType.InlineNone           // stloc.1
            InlineNone(noPrefixes (I_stloc(2us)))//byte OperandType.InlineNone           // stloc.2
            InlineNone(noPrefixes (I_stloc(3us)))//byte OperandType.InlineNone           // stloc.3
            ShortInlineVar(noPrefixes (fun index -> I_ldarg(uint16 index))) //byte OperandType.ShortInlineVar       // ldarg.s
            ShortInlineVar(noPrefixes (fun index -> I_ldarga(uint16 index)))//byte OperandType.ShortInlineVar       // ldarga.s
            ShortInlineVar(noPrefixes (fun index -> I_starg(uint16 index)))//byte OperandType.ShortInlineVar       // starg.s
            ShortInlineVar(noPrefixes (fun index -> I_ldloc(uint16 index)))//byte OperandType.ShortInlineVar       // ldloc.s
            ShortInlineVar(noPrefixes (fun index -> I_ldloca(uint16 index)))//byte OperandType.ShortInlineVar       // ldloca.s
            ShortInlineVar(noPrefixes (fun index -> I_stloc(uint16 index)))//byte OperandType.ShortInlineVar       // stloc.s
            InlineNone(noPrefixes AI_ldnull)//byte OperandType.InlineNone           // ldnull
            InlineNone(noPrefixes (AI_ldc(DT_I4, ILConst.I4(-1))))//byte OperandType.InlineNone           // ldc.i4.m1
            InlineNone(noPrefixes (AI_ldc(DT_I4, ILConst.I4(0))))//byte OperandType.InlineNone           // ldc.i4.0
            InlineNone(noPrefixes (AI_ldc(DT_I4, ILConst.I4(1))))//byte OperandType.InlineNone           // ldc.i4.1
            InlineNone(noPrefixes (AI_ldc(DT_I4, ILConst.I4(2))))//byte OperandType.InlineNone           // ldc.i4.2
            InlineNone(noPrefixes (AI_ldc(DT_I4, ILConst.I4(3))))//byte OperandType.InlineNone           // ldc.i4.3
            InlineNone(noPrefixes (AI_ldc(DT_I4, ILConst.I4(4))))//byte OperandType.InlineNone           // ldc.i4.4
            InlineNone(noPrefixes (AI_ldc(DT_I4, ILConst.I4(5))))//byte OperandType.InlineNone           // ldc.i4.5
            InlineNone(noPrefixes (AI_ldc(DT_I4, ILConst.I4(6))))//byte OperandType.InlineNone           // ldc.i4.6
            InlineNone(noPrefixes (AI_ldc(DT_I4, ILConst.I4(7))))//byte OperandType.InlineNone           // ldc.i4.7
            InlineNone(noPrefixes (AI_ldc(DT_I4, ILConst.I4(8))))//byte OperandType.InlineNone           // ldc.i4.8
            ShortInlineI(noPrefixes (fun value -> AI_ldc(DT_I4, ILConst.I4(int32 value))))//byte OperandType.ShortInlineI         // ldc.i4.s
            InlineI(noPrefixes (fun value -> AI_ldc(DT_I4, ILConst.I4(value))))//byte OperandType.InlineI              // ldc.i4
            InlineI8(noPrefixes (fun value -> AI_ldc(DT_I8, ILConst.I8(value))))//byte OperandType.InlineI8             // ldc.i8
            ShortInlineR(noPrefixes (fun value -> AI_ldc(DT_R4, ILConst.R4(value))))//byte OperandType.ShortInlineR         // ldc.r4
            InlineR(noPrefixes (fun value -> AI_ldc(DT_R8, ILConst.R8(value))))//byte OperandType.InlineR              // ldc.r8
            NoDecoder
            InlineNone(noPrefixes AI_dup)//byte OperandType.InlineNone           // dup
            InlineNone(noPrefixes AI_pop)//byte OperandType.InlineNone           // pop
            InlineMethod(noPrefixes (fun (ilMethSpec, _) -> I_jmp(ilMethSpec)))//byte OperandType.InlineMethod         // jmp
            InlineMethod(tailPrefix (fun ilTailcall (ilMethSpec, ilVarArgs) -> I_call(ilTailcall, ilMethSpec, ilVarArgs)))//byte OperandType.InlineMethod         // call
            InlineSig(tailPrefix (fun ilTailcall (ilCallSig, ilVarArgs) -> I_calli(ilTailcall, ilCallSig, ilVarArgs)))//byte OperandType.InlineSig            // calli
            InlineNone(noPrefixes I_ret)//byte OperandType.InlineNone           // ret
            ShortInlineBrTarget(noPrefixes (fun value -> I_br(int value)))//byte OperandType.ShortInlineBrTarget  // br.s
            ShortInlineBrTarget(noPrefixes (fun value -> I_brcmp(BI_brfalse, int value)))//byte OperandType.ShortInlineBrTarget  // brfalse.s
            ShortInlineBrTarget(noPrefixes (fun value -> I_brcmp(BI_brtrue, int value)))//byte OperandType.ShortInlineBrTarget  // brtrue.s
            ShortInlineBrTarget(noPrefixes (fun value -> I_brcmp(BI_beq, int value)))//byte OperandType.ShortInlineBrTarget  // beq.s
            ShortInlineBrTarget(noPrefixes (fun value -> I_brcmp(BI_bge, int value)))//byte OperandType.ShortInlineBrTarget  // bge.s
            ShortInlineBrTarget(noPrefixes (fun value -> I_brcmp(BI_bgt, int value)))//byte OperandType.ShortInlineBrTarget  // bgt.s
            ShortInlineBrTarget(noPrefixes (fun value -> I_brcmp(BI_ble, int value)))//byte OperandType.ShortInlineBrTarget  // ble.s
            ShortInlineBrTarget(noPrefixes (fun value -> I_brcmp(BI_blt, int value)))//byte OperandType.ShortInlineBrTarget  // blt.s
            ShortInlineBrTarget(noPrefixes (fun value -> I_brcmp(BI_bne_un, int value)))//byte OperandType.ShortInlineBrTarget  // bne.un.s
            ShortInlineBrTarget(noPrefixes (fun value -> I_brcmp(BI_bge_un, int value)))//byte OperandType.ShortInlineBrTarget  // bge.un.s
            ShortInlineBrTarget(noPrefixes (fun value -> I_brcmp(BI_bgt_un, int value)))//byte OperandType.ShortInlineBrTarget  // bgt.un.s
            ShortInlineBrTarget(noPrefixes (fun value -> I_brcmp(BI_ble_un, int value)))//byte OperandType.ShortInlineBrTarget  // ble.un.s
            ShortInlineBrTarget(noPrefixes (fun value -> I_brcmp(BI_blt_un, int value)))//byte OperandType.ShortInlineBrTarget  // blt.un.s
            InlineBrTarget(noPrefixes (fun value -> I_br(value))) // br
            InlineBrTarget(noPrefixes (fun value -> I_brcmp(BI_brfalse, value))) // brfalse
            InlineBrTarget(noPrefixes (fun value -> I_brcmp(BI_brtrue, value))) // brtrue
            InlineBrTarget(noPrefixes (fun value -> I_brcmp(BI_beq, value))) // beq
            InlineBrTarget(noPrefixes (fun value -> I_brcmp(BI_bge, value))) // bge
            InlineBrTarget(noPrefixes (fun value -> I_brcmp(BI_bgt, value))) // bgt
            InlineBrTarget(noPrefixes (fun value -> I_brcmp(BI_ble, value))) // ble
            InlineBrTarget(noPrefixes (fun value -> I_brcmp(BI_blt, value))) // blt
            InlineBrTarget(noPrefixes (fun value -> I_brcmp(BI_bne_un, value))) // bne.un
            InlineBrTarget(noPrefixes (fun value -> I_brcmp(BI_bge_un, value))) // bge.un
            InlineBrTarget(noPrefixes (fun value -> I_brcmp(BI_bgt_un, value))) // bgt.un
            InlineBrTarget(noPrefixes (fun value -> I_brcmp(BI_ble_un, value))) // ble.un
            InlineBrTarget(noPrefixes (fun value -> I_brcmp(BI_blt_un, value))) // blt.un
            InlineSwitch(noPrefixes (fun values -> ILInstr.I_switch(values)))//byte OperandType.InlineSwitch         // switch
            InlineNone(volatileOrUnalignedPrefix (fun (ilAlignment, ilVolatility) -> I_ldind(ilAlignment, ilVolatility, DT_I1)))//byte OperandType.InlineNone           // ldind.i1
            InlineNone(volatileOrUnalignedPrefix (fun (ilAlignment, ilVolatility) -> I_ldind(ilAlignment, ilVolatility, DT_U1)))//byte OperandType.InlineNone           // ldind.u1
            InlineNone(volatileOrUnalignedPrefix (fun (ilAlignment, ilVolatility) -> I_ldind(ilAlignment, ilVolatility, DT_I2)))//byte OperandType.InlineNone           // ldind.i2
            InlineNone(volatileOrUnalignedPrefix (fun (ilAlignment, ilVolatility) -> I_ldind(ilAlignment, ilVolatility, DT_U2)))//byte OperandType.InlineNone           // ldind.u2
            InlineNone(volatileOrUnalignedPrefix (fun (ilAlignment, ilVolatility) -> I_ldind(ilAlignment, ilVolatility, DT_I4)))//byte OperandType.InlineNone           // ldind.i4
            InlineNone(volatileOrUnalignedPrefix (fun (ilAlignment, ilVolatility) -> I_ldind(ilAlignment, ilVolatility, DT_U4)))//byte OperandType.InlineNone           // ldind.u4
            InlineNone(volatileOrUnalignedPrefix (fun (ilAlignment, ilVolatility) -> I_ldind(ilAlignment, ilVolatility, DT_I8)))//byte OperandType.InlineNone           // ldind.i8
            InlineNone(volatileOrUnalignedPrefix (fun (ilAlignment, ilVolatility) -> I_ldind(ilAlignment, ilVolatility, DT_I)))//byte OperandType.InlineNone           // ldind.i
            InlineNone(volatileOrUnalignedPrefix (fun (ilAlignment, ilVolatility) -> I_ldind(ilAlignment, ilVolatility, DT_R4)))//byte OperandType.InlineNone           // ldind.r4
            InlineNone(volatileOrUnalignedPrefix (fun (ilAlignment, ilVolatility) -> I_ldind(ilAlignment, ilVolatility, DT_R8)))//byte OperandType.InlineNone           // ldind.r8
            InlineNone(volatileOrUnalignedPrefix (fun (ilAlignment, ilVolatility) -> I_ldind(ilAlignment, ilVolatility, DT_REF)))//byte OperandType.InlineNone           // ldind.ref
            InlineNone(volatileOrUnalignedPrefix (fun (ilAlignment, ilVolatility) -> I_stind(ilAlignment, ilVolatility, DT_REF)))//byte OperandType.InlineNone           // stind.ref
            InlineNone(volatileOrUnalignedPrefix (fun (ilAlignment, ilVolatility) -> I_stind(ilAlignment, ilVolatility, DT_I1)))//byte OperandType.InlineNone           // stind.i1
            InlineNone(volatileOrUnalignedPrefix (fun (ilAlignment, ilVolatility) -> I_stind(ilAlignment, ilVolatility, DT_I2)))//byte OperandType.InlineNone           // stind.i2
            InlineNone(volatileOrUnalignedPrefix (fun (ilAlignment, ilVolatility) -> I_stind(ilAlignment, ilVolatility, DT_I4)))//byte OperandType.InlineNone           // stind.i4
            InlineNone(volatileOrUnalignedPrefix (fun (ilAlignment, ilVolatility) -> I_stind(ilAlignment, ilVolatility, DT_I8)))//byte OperandType.InlineNone           // stind.i8
            InlineNone(volatileOrUnalignedPrefix (fun (ilAlignment, ilVolatility) -> I_stind(ilAlignment, ilVolatility, DT_R4)))//byte OperandType.InlineNone           // stind.r4
            InlineNone(volatileOrUnalignedPrefix (fun (ilAlignment, ilVolatility) -> I_stind(ilAlignment, ilVolatility, DT_R8)))//byte OperandType.InlineNone           // stind.r8
            InlineNone(noPrefixes AI_add)//byte OperandType.InlineNone           // add
            InlineNone(noPrefixes AI_sub)//byte OperandType.InlineNone           // sub
            InlineNone(noPrefixes AI_mul)//byte OperandType.InlineNone           // mul
            InlineNone(noPrefixes AI_div)//byte OperandType.InlineNone           // div
            InlineNone(noPrefixes AI_div_un)//byte OperandType.InlineNone           // div.un
            InlineNone(noPrefixes AI_rem)//byte OperandType.InlineNone           // rem
            InlineNone(noPrefixes AI_rem_un)//byte OperandType.InlineNone           // rem.un
            InlineNone(noPrefixes AI_and)//byte OperandType.InlineNone           // and
            InlineNone(noPrefixes AI_or)//byte OperandType.InlineNone           // or
            InlineNone(noPrefixes AI_xor)//byte OperandType.InlineNone           // xor
            InlineNone(noPrefixes AI_shl)//byte OperandType.InlineNone           // shl
            InlineNone(noPrefixes AI_shr)//byte OperandType.InlineNone           // shr
            InlineNone(noPrefixes AI_shr_un)//byte OperandType.InlineNone           // shr.un
            InlineNone(noPrefixes AI_neg)//byte OperandType.InlineNone           // neg
            InlineNone(noPrefixes AI_not)//byte OperandType.InlineNone           // not
            InlineNone(noPrefixes (AI_conv(DT_I1)))//byte OperandType.InlineNone           // conv.i1
            InlineNone(noPrefixes (AI_conv(DT_I2)))//byte OperandType.InlineNone           // conv.i2
            InlineNone(noPrefixes (AI_conv(DT_I4)))//byte OperandType.InlineNone           // conv.i4
            InlineNone(noPrefixes (AI_conv(DT_I8)))//byte OperandType.InlineNone           // conv.i8
            InlineNone(noPrefixes (AI_conv(DT_R4)))//byte OperandType.InlineNone           // conv.r4
            InlineNone(noPrefixes (AI_conv(DT_R8)))//byte OperandType.InlineNone           // conv.r8
            InlineNone(noPrefixes (AI_conv(DT_U4)))//byte OperandType.InlineNone           // conv.u4
            InlineNone(noPrefixes (AI_conv(DT_U8)))//byte OperandType.InlineNone           // conv.u8
            InlineMethod(constraintOrTailPrefix (fun (ilConstraint, ilTailcall) (ilMethSpec, ilVarArgs) -> match ilConstraint with | Some(ilType) -> I_callconstraint(ilTailcall, ilType, ilMethSpec, ilVarArgs) | _ -> I_callvirt(ilTailcall, ilMethSpec, ilVarArgs)))//byte OperandType.InlineMethod         // callvirt
            InlineType(noPrefixes (fun ilType -> I_cpobj(ilType)))//byte OperandType.InlineType           // cpobj
            InlineType(volatileOrUnalignedPrefix (fun (ilAlignment, ilVolatility) ilType -> I_ldobj(ilAlignment, ilVolatility, ilType)))//byte OperandType.InlineType           // ldobj
            InlineString(noPrefixes (fun value -> I_ldstr(value)))//byte OperandType.InlineString         // ldstr
            InlineMethod(noPrefixes (fun (ilMethSpec, ilVarArgs) -> I_newobj(ilMethSpec, ilVarArgs)))//byte OperandType.InlineMethod         // newobj
            InlineType(noPrefixes (fun ilType -> I_castclass(ilType)))//byte OperandType.InlineType           // castclass
            InlineType(noPrefixes (fun ilType -> I_isinst(ilType)))//byte OperandType.InlineType           // isinst
            InlineNone(noPrefixes (AI_conv(DT_R)))//byte OperandType.InlineNone           // conv.r.un // TODO: Looks like we don't have this?
            NoDecoder
            NoDecoder
            InlineType(noPrefixes (fun ilType -> I_unbox(ilType)))//byte OperandType.InlineType           // unbox
            InlineNone(noPrefixes I_throw)//byte OperandType.InlineNone           // throw
            InlineField(volatileOrUnalignedPrefix (fun (ilAlignment, ilVolatility) ilFieldSpec -> I_ldfld(ilAlignment, ilVolatility, ilFieldSpec)))//byte OperandType.InlineField          // ldfld
            InlineField(noPrefixes (fun ilFieldSpec -> I_ldflda(ilFieldSpec)))//byte OperandType.InlineField          // ldflda
            InlineField(volatileOrUnalignedPrefix (fun (ilAlignment, ilVolatility) ilFieldSpec -> I_stfld(ilAlignment, ilVolatility, ilFieldSpec)))//byte OperandType.InlineField          // stfld
            InlineField(volatilePrefix (fun ilVolatility ilFieldSpec -> I_ldsfld(ilVolatility, ilFieldSpec)))//byte OperandType.InlineField          // ldsfld
            InlineField(noPrefixes (fun ilFieldSpec -> I_ldsflda(ilFieldSpec)))//byte OperandType.InlineField          // ldsflda
            InlineField(volatilePrefix (fun ilVolatility ilFieldSpec -> I_stsfld(ilVolatility, ilFieldSpec)))//byte OperandType.InlineField          // stsfld
            InlineType(volatileOrUnalignedPrefix (fun (ilAlignment, ilVolatility) ilType -> I_stobj(ilAlignment, ilVolatility, ilType)))//byte OperandType.InlineType           // stobj
            InlineNone(noPrefixes (AI_conv_ovf_un(DT_I1)))//byte OperandType.InlineNone           // conv.ovf.i1.un
            InlineNone(noPrefixes (AI_conv_ovf_un(DT_I2)))//byte OperandType.InlineNone           // conv.ovf.i2.un
            InlineNone(noPrefixes (AI_conv_ovf_un(DT_I4)))//byte OperandType.InlineNone           // conv.ovf.i4.un
            InlineNone(noPrefixes (AI_conv_ovf_un(DT_I8)))//byte OperandType.InlineNone           // conv.ovf.i8.un
            InlineNone(noPrefixes (AI_conv_ovf_un(DT_U1)))//byte OperandType.InlineNone           // conv.ovf.u1.un
            InlineNone(noPrefixes (AI_conv_ovf_un(DT_U2)))//byte OperandType.InlineNone           // conv.ovf.u2.un
            InlineNone(noPrefixes (AI_conv_ovf_un(DT_U4)))//byte OperandType.InlineNone           // conv.ovf.u4.un
            InlineNone(noPrefixes (AI_conv_ovf_un(DT_U8)))//byte OperandType.InlineNone           // conv.ovf.u8.un
            InlineNone(noPrefixes (AI_conv_ovf_un(DT_I)))//byte OperandType.InlineNone           // conv.ovf.i.un
            InlineNone(noPrefixes (AI_conv_ovf_un(DT_U)))//byte OperandType.InlineNone           // conv.ovf.u.un
            InlineType(noPrefixes (fun ilType -> I_unbox(ilType)))//byte OperandType.InlineType           // box
            InlineType(noPrefixes (fun ilType -> I_newarr(ILArrayShape.SingleDimensional, ilType)))//byte OperandType.InlineType           // newarr
            InlineNone(noPrefixes I_ldlen)//byte OperandType.InlineNone           // ldlen
            InlineType(readonlyPrefix (fun ilReadonly ilType -> I_ldelema(ilReadonly, false, ILArrayShape.SingleDimensional, ilType)))//byte OperandType.InlineType           // ldelema
            InlineNone(noPrefixes (I_ldelem(DT_I1)))//byte OperandType.InlineNone           // ldelem.i1
            InlineNone(noPrefixes (I_ldelem(DT_U1)))//byte OperandType.InlineNone           // ldelem.u1
            InlineNone(noPrefixes (I_ldelem(DT_I2)))//byte OperandType.InlineNone           // ldelem.i2
            InlineNone(noPrefixes (I_ldelem(DT_U2)))//byte OperandType.InlineNone           // ldelem.u2
            InlineNone(noPrefixes (I_ldelem(DT_I4)))//byte OperandType.InlineNone           // ldelem.i4
            InlineNone(noPrefixes (I_ldelem(DT_U4)))//byte OperandType.InlineNone           // ldelem.u4
            InlineNone(noPrefixes (I_ldelem(DT_I8)))//byte OperandType.InlineNone           // ldelem.i8
            InlineNone(noPrefixes (I_ldelem(DT_I)))//byte OperandType.InlineNone           // ldelem.i
            InlineNone(noPrefixes (I_ldelem(DT_R4)))//byte OperandType.InlineNone           // ldelem.r4
            InlineNone(noPrefixes (I_ldelem(DT_R8)))//byte OperandType.InlineNone           // ldelem.r8
            InlineNone(noPrefixes (I_ldelem(DT_REF)))//byte OperandType.InlineNone           // ldelem.ref
            InlineNone(noPrefixes (I_stelem(DT_I)))//byte OperandType.InlineNone           // stelem.i
            InlineNone(noPrefixes (I_stelem(DT_I1)))//byte OperandType.InlineNone           // stelem.i1
            InlineNone(noPrefixes (I_stelem(DT_I2)))//byte OperandType.InlineNone           // stelem.i2
            InlineNone(noPrefixes (I_stelem(DT_I4)))//byte OperandType.InlineNone           // stelem.i4
            InlineNone(noPrefixes (I_stelem(DT_I8)))//byte OperandType.InlineNone           // stelem.i8
            InlineNone(noPrefixes (I_stelem(DT_R4)))//byte OperandType.InlineNone           // stelem.r4
            InlineNone(noPrefixes (I_stelem(DT_R8)))//byte OperandType.InlineNone           // stelem.r8
            InlineNone(noPrefixes (I_stelem(DT_REF)))//byte OperandType.InlineNone           // stelem.ref
            InlineType(noPrefixes (fun ilType -> I_ldelem_any(ILArrayShape.SingleDimensional, ilType)))//byte OperandType.InlineType           // ldelem
            InlineType(noPrefixes (fun ilType -> I_stelem_any(ILArrayShape.SingleDimensional, ilType)))//byte OperandType.InlineType           // stelem
            InlineType(noPrefixes (fun ilType -> I_unbox_any(ilType)))//byte OperandType.InlineType           // unbox.any
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
            InlineNone(noPrefixes (AI_conv_ovf(DT_I1)))//byte OperandType.InlineNone           // conv.ovf.i1
            InlineNone(noPrefixes (AI_conv_ovf(DT_U1)))//byte OperandType.InlineNone           // conv.ovf.u1
            InlineNone(noPrefixes (AI_conv_ovf(DT_I2)))//byte OperandType.InlineNone           // conv.ovf.i2
            InlineNone(noPrefixes (AI_conv_ovf(DT_U2)))//byte OperandType.InlineNone           // conv.ovf.u2
            InlineNone(noPrefixes (AI_conv_ovf(DT_I4)))//byte OperandType.InlineNone           // conv.ovf.i4
            InlineNone(noPrefixes (AI_conv_ovf(DT_U4)))//byte OperandType.InlineNone           // conv.ovf.u4
            InlineNone(noPrefixes (AI_conv_ovf(DT_I8)))//byte OperandType.InlineNone           // conv.ovf.i8
            InlineNone(noPrefixes (AI_conv_ovf(DT_U8)))//byte OperandType.InlineNone           // conv.ovf.u8
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
            InlineType(noPrefixes (fun ilType -> I_refanyval(ilType)))//byte OperandType.InlineType           // refanyval
            InlineNone(noPrefixes AI_ckfinite)//byte OperandType.InlineNone           // ckfinite
            NoDecoder
            NoDecoder
            InlineType(noPrefixes (fun ilType -> I_mkrefany(ilType)))//byte OperandType.InlineType           // mkrefany
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
            InlineTok(noPrefixes (fun ilToken -> I_ldtoken(ilToken)))//byte OperandType.InlineTok            // ldtoken
            InlineNone(noPrefixes (AI_conv(DT_U2)))//byte OperandType.InlineNone           // conv.u2
            InlineNone(noPrefixes (AI_conv(DT_U1)))//byte OperandType.InlineNone           // conv.u1
            InlineNone(noPrefixes (AI_conv(DT_I)))//byte OperandType.InlineNone           // conv.i
            InlineNone(noPrefixes (AI_conv_ovf(DT_I)))//byte OperandType.InlineNone           // conv.ovf.i
            InlineNone(noPrefixes (AI_conv_ovf(DT_U)))//byte OperandType.InlineNone           // conv.ovf.u
            InlineNone(noPrefixes AI_add_ovf)//byte OperandType.InlineNone           // add.ovf
            InlineNone(noPrefixes AI_add_ovf_un)//byte OperandType.InlineNone           // add.ovf.un
            InlineNone(noPrefixes AI_mul_ovf)//byte OperandType.InlineNone           // mul.ovf
            InlineNone(noPrefixes AI_mul_ovf_un)//byte OperandType.InlineNone           // mul.ovf.un
            InlineNone(noPrefixes AI_sub_ovf)//byte OperandType.InlineNone           // sub.ovf
            InlineNone(noPrefixes AI_sub_ovf_un)//byte OperandType.InlineNone           // sub.ovf.un
            InlineNone(noPrefixes I_endfinally)//byte OperandType.InlineNone           // endfinally
            InlineBrTarget(noPrefixes (fun value -> I_leave(value))) // leave
            ShortInlineBrTarget(noPrefixes (fun value -> I_leave(int value)))//byte OperandType.ShortInlineBrTarget  // leave.s
            InlineNone(volatileOrUnalignedPrefix (fun (ilAlignment, ilVolatility) -> I_stind(ilAlignment, ilVolatility, DT_I)))//byte OperandType.InlineNone           // stind.i
            InlineNone(noPrefixes (AI_conv(DT_U)))//byte OperandType.InlineNone           // conv.u            (0xe0)
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
            NoDecoder
        |]

    let TwoByteDecoders =
        [|
            InlineNone(noPrefixes I_arglist)//(byte)OperandType.InlineNone,           // arglist           (0xfe 0x00)
            InlineNone(noPrefixes AI_ceq)//(byte)OperandType.InlineNone,           // ceq
            InlineNone(noPrefixes AI_cgt)//(byte)OperandType.InlineNone,           // cgt
            InlineNone(noPrefixes AI_cgt_un)//(byte)OperandType.InlineNone,           // cgt.un
            InlineNone(noPrefixes AI_clt)//(byte)OperandType.InlineNone,           // clt
            InlineNone(noPrefixes AI_clt_un)//(byte)OperandType.InlineNone,           // clt.un
            InlineMethod(noPrefixes (fun (ilMethSpec, _) -> I_ldftn(ilMethSpec)))//(byte)OperandType.InlineMethod,         // ldftn
            InlineMethod(noPrefixes (fun (ilMethSpec, _) -> I_ldvirtftn(ilMethSpec)))//(byte)OperandType.InlineMethod,         // ldvirtftn
            NoDecoder
            InlineVar(noPrefixes (fun index -> I_ldarg(index)))//(byte)OperandType.InlineVar,            // ldarg
            InlineVar(noPrefixes (fun index -> I_ldarga(index)))//(byte)OperandType.InlineVar,            // ldarga
            InlineVar(noPrefixes (fun index -> I_starg(index)))//(byte)OperandType.InlineVar,            // starg
            InlineVar(noPrefixes (fun index -> I_ldloc(index)))//(byte)OperandType.InlineVar,            // ldloc
            InlineVar(noPrefixes (fun index -> I_ldloca(index)))//(byte)OperandType.InlineVar,            // ldloca
            InlineVar(noPrefixes (fun index -> I_stloc(index)))//(byte)OperandType.InlineVar,            // stloc
            InlineNone(noPrefixes I_localloc)//(byte)OperandType.InlineNone,           // localloc
            NoDecoder
            InlineNone(noPrefixes I_endfilter)//(byte)OperandType.InlineNone,           // endfilter
            PrefixShortInlineI(fun prefixes value -> prefixes.al <- match value with | 1us -> Unaligned1 | 2us -> Unaligned2 | 4us -> Unaligned4 | _ -> (* possible warning? *) Aligned)//(byte)OperandType.ShortInlineI,         // unaligned.
            PrefixInlineNone(fun prefixes -> prefixes.vol <- Volatile)//(byte)OperandType.InlineNone,           // volatile.
            PrefixInlineNone(fun prefixes -> prefixes.tl <- Tailcall)//(byte)OperandType.InlineNone,           // tail.
            InlineType(noPrefixes (fun ilType -> I_initobj(ilType)))//(byte)OperandType.InlineType,           // initobj
            PrefixInlineType(fun prefixes ilType -> prefixes.constrained <- Some(ilType))//(byte)OperandType.InlineType,           // constrained.
            InlineNone(volatileOrUnalignedPrefix (fun (ilAlignment, ilVolatility) -> I_cpblk(ilAlignment, ilVolatility)))//(byte)OperandType.InlineNone,           // cpblk
            InlineNone(volatileOrUnalignedPrefix (fun (ilAlignment, ilVolatility) -> I_initblk(ilAlignment, ilVolatility)))//(byte)OperandType.InlineNone,           // initblk
            NoDecoder
            InlineNone(noPrefixes I_rethrow)//(byte)OperandType.InlineNone,           // rethrow
            NoDecoder
            InlineType(noPrefixes (fun ilType -> I_sizeof(ilType)))//(byte)OperandType.InlineType,           // sizeof
            InlineNone(noPrefixes I_refanytype)//(byte)OperandType.InlineNone,           // refanytype
            PrefixInlineNone(fun prefixes -> prefixes.ro <- ReadonlyAddress)//(byte)OperandType.InlineNone,           // readonly.         (0xfe 0x1e)
        |]

    let readILOperandDecoder (ilReader: byref<BlobReader>) : ILOperandDecoder =
        let operation = int (ilReader.ReadByte())
        if operation = 0xfe then TwoByteDecoders.[int (ilReader.ReadByte())]
        else OneByteDecoders.[operation]

    let readILInstrs (cenv: cenv) (ilReader: byref<BlobReader>) =
        let mdReader = cenv.MetadataReader

        let instrs = ResizeArray()

        let prefixes =
            {
                al = Aligned
                tl = Normalcall
                vol = Nonvolatile
                ro = NormalAddress
                constrained = None
            }
    
        while ilReader.RemainingBytes > 0 do
            match readILOperandDecoder &ilReader with
            | PrefixInlineNone(f) -> f prefixes
            | PrefixShortInlineI(f) -> f prefixes (ilReader.ReadUInt16())
            | PrefixInlineType(f) ->
                let handle = MetadataTokens.EntityHandle(ilReader.ReadInt32())
                let ilType = readILType cenv handle
                f prefixes ilType

            | decoder ->
                let instr =
                    match decoder with
                    | NoDecoder -> failwith "Bad IL reading format"
                    | InlineNone(f) -> f prefixes
                    | ShortInlineVar(f) -> f prefixes (ilReader.ReadSByte())
                    | ShortInlineI(f) -> f prefixes (ilReader.ReadSByte())
                    | InlineI(f) -> f prefixes (ilReader.ReadInt32())
                    | InlineI8(f) -> f prefixes (ilReader.ReadInt64())
                    | ShortInlineR(f) -> f prefixes (ilReader.ReadSingle())
                    | InlineR(f) -> f prefixes (ilReader.ReadDouble())

                    | InlineMethod(f) ->
                        let handle = MetadataTokens.EntityHandle(ilReader.ReadInt32())

                        match readDeclaringTypeInfoFromMemberOrMethod cenv handle with
                        | name, ILType.Array(shape, ilType) ->
                            match name with
                            | "Get" -> I_ldelem_any(shape, ilType)
                            | "Set" -> I_stelem_any(shape, ilType)
                            | "Address" -> I_ldelema(prefixes.ro, false, shape, ilType)
                            | ".ctor" -> I_newarr(shape, ilType)
                            | _ -> failwith "Bad method on array type"
                        | _ ->
                            let ilMethSpec = readILMethodSpec cenv handle
                            let ilVarArgs =
                                match ilMethSpec.GenericArgs with
                                | [] -> None
                                | xs -> Some(xs)
                            f prefixes (ilMethSpec, ilVarArgs)

                    | InlineSig(f) ->
                        let handle = MetadataTokens.EntityHandle(ilReader.ReadInt32())
                        let ilMethSpec = readILMethodSpec cenv handle
                        let ilVarArgs =
                            match ilMethSpec.GenericArgs with
                            | [] -> None
                            | xs -> Some(xs)
                        f prefixes (ilMethSpec.MethodRef.CallingSignature, ilVarArgs)

                    | ShortInlineBrTarget(f) -> f prefixes (ilReader.ReadSByte())
                    | InlineBrTarget(f) -> f prefixes (ilReader.ReadInt32())

                    | InlineSwitch(f) ->
                        let deltas = Array.zeroCreate (ilReader.ReadInt32())
                        for i = 0 to deltas.Length - 1 do deltas.[i] <- ilReader.ReadInt32()
                        f prefixes (deltas |> List.ofArray)

                    | InlineType(f) ->
                        let handle = MetadataTokens.EntityHandle(ilReader.ReadInt32())
                        let ilType = readILType cenv handle
                        f prefixes ilType
                    
                    | InlineString(f) ->
                        let handle = MetadataTokens.Handle(ilReader.ReadInt32())
                        let value =
                            match handle.Kind with
                            | HandleKind.String -> mdReader.GetString(StringHandle.op_Explicit(handle))
                            | HandleKind.UserString -> mdReader.GetUserString(UserStringHandle.op_Explicit(handle))
                            | _ -> failwithf "Invalid Handle Kind: %A" handle.Kind
                        f prefixes value

                    | InlineField(f) ->
                        let handle = MetadataTokens.EntityHandle(ilReader.ReadInt32())
                        let ilFieldSpec = readILFieldSpec cenv handle
                        f prefixes ilFieldSpec

                    | InlineTok(f) ->
                        let handle = MetadataTokens.EntityHandle(ilReader.ReadInt32())

                        let ilToken =
                            match handle.Kind with
                            | HandleKind.MethodDefinition
                            | HandleKind.MemberReference -> ILToken.ILMethod(readILMethodSpec cenv handle)
                            | HandleKind.FieldDefinition -> ILToken.ILField(readILFieldSpec cenv handle)
                            | HandleKind.TypeDefinition
                            | HandleKind.TypeReference
                            | HandleKind.TypeSpecification -> ILToken.ILType(readILType cenv handle)
                            | _ -> failwithf "Invalid Handle Kind: %A" handle.Kind

                        f prefixes ilToken

                    | InlineVar(f) -> f prefixes (ilReader.ReadUInt16())
                    | _ -> failwith "Incorrect IL reading decoder at this point"

                instrs.Add(instr)

                // Reset prefixes
                prefixes.al <- Aligned
                prefixes.tl <- Normalcall
                prefixes.vol <- Nonvolatile
                prefixes.ro <- NormalAddress
                prefixes.constrained <- None

        instrs.ToArray()

    // --------------------------------------------------------------------

    let decodeLocalSignature (cenv: cenv) (mdReader: MetadataReader) localSignature =
        let si = mdReader.GetStandaloneSignature localSignature
        si.DecodeLocalSignature(cenv.LocalSignatureTypeProvider, ())
        |> List.ofSeq

    let mkMethodCodeLabelLookup (debugInfo: MethodDebugInformation) =
        let lookup = Dictionary()

        for seqPoint in debugInfo.GetSequencePoints() do
            lookup.Add(generateCodeLabel(), seqPoint.Offset)

        lookup

    let readILLocalDebugInfo (pdbReader: MetadataReader) (debugInfoHandle: MethodDebugInformationHandle) =
        let localScopes = pdbReader.GetLocalScopes debugInfoHandle |> Seq.map pdbReader.GetLocalScope
        localScopes 
        |> Seq.map (fun localScope ->
            {
                Range = (localScope.StartOffset, localScope.EndOffset)
                DebugMappings = 
                    localScope.GetLocalVariables()
                    |> Seq.choose (fun handle -> 
                        let x = pdbReader.GetLocalVariable handle
                        if x.Attributes &&& LocalVariableAttributes.DebuggerHidden <> LocalVariableAttributes.DebuggerHidden then
                            Some({ LocalIndex = x.Index; LocalName = pdbReader.GetString x.Name } : ILLocalDebugMapping)
                        else
                            None)
                    |> List.ofSeq
            } : ILLocalDebugInfo)
        |> List.ofSeq

    let readMethodDebugInfo (cenv: cenv) (methDef: MethodDefinition) =
        let mdReader = cenv.MetadataReader

        match cenv.PdbReaderProvider with
        | Some(readerProvider, _) ->
            let pdbReader = readerProvider.GetMetadataReader()
            let debugInfoOpt = 
                pdbReader.MethodDebugInformation 
                |> Seq.tryPick (fun handle -> 
                    if handle.IsNil then None 
                    else
                        let debugInfo = pdbReader.GetMethodDebugInformation handle
                        let doc = pdbReader.GetDocument debugInfo.Document
                        if pdbReader.GetString doc.Name = mdReader.GetString methDef.Name then
                            Some(handle, mkMethodCodeLabelLookup debugInfo)
                        else
                            None)

            match debugInfoOpt with
            | Some(debugInfoHandle, labels) when not debugInfoHandle.IsNil ->
                (labels, readILLocalDebugInfo pdbReader debugInfoHandle)
            | _ ->
                (Dictionary(), List.empty)
        | _ ->
            (Dictionary(), List.empty)

    let readILCode (cenv: cenv) (methDef: MethodDefinition) (methBodyBlock: MethodBodyBlock) : ILCode =
        let exceptions =
            methBodyBlock.ExceptionRegions
            |> Seq.map (fun region ->
                let start = region.HandlerOffset
                let finish = region.HandlerOffset + region.HandlerLength
                let clause =
                    match region.Kind with
                    | ExceptionRegionKind.Finally ->
                        ILExceptionClause.Finally(start, finish)
                    | ExceptionRegionKind.Fault ->
                        ILExceptionClause.Fault(start, finish)
                    | ExceptionRegionKind.Filter ->
                        let filterStart = region.FilterOffset
                        let filterFinish = region.HandlerOffset
                        ILExceptionClause.FilterCatch((filterStart, filterFinish), (start, finish))
                    | ExceptionRegionKind.Catch ->
                        ILExceptionClause.TypeCatch(readILType cenv region.CatchType, (start, finish))
                    | _ ->
                        failwithf "Invalid Exception Region Kind: %A" region.Kind

                {
                    ILExceptionSpec.Range = (region.TryOffset, region.TryOffset + region.TryLength)
                    ILExceptionSpec.Clause = clause
                }
            )
            |> List.ofSeq

        let labels, locals = readMethodDebugInfo cenv methDef

        let mutable ilReader = methBodyBlock.GetILReader()
        let instrs = readILInstrs cenv &ilReader

        {
            Labels = labels
            Instrs = instrs
            Exceptions = exceptions
            Locals = locals
        }

    let readILMethodBody (cenv: cenv) (methDef: MethodDefinition) : ILMethodBody =
        let peReader = cenv.PEReader
        let mdReader = cenv.MetadataReader

        let methBodyBlock = peReader.GetMethodBody(methDef.RelativeVirtualAddress)

        let ilLocals =
            if methBodyBlock.LocalSignature.IsNil then []
            else decodeLocalSignature cenv mdReader methBodyBlock.LocalSignature

        let ilCode = readILCode cenv methDef methBodyBlock
    
        {
            IsZeroInit = methBodyBlock.LocalVariablesInitialized
            MaxStack = methBodyBlock.MaxStack
            NoInlining = int (methDef.ImplAttributes &&& MethodImplAttributes.NoInlining) <> 0
            AggressiveInlining = int (methDef.ImplAttributes &&& MethodImplAttributes.AggressiveInlining) <> 0
            Locals = ilLocals
            Code = ilCode
            SourceMarker = None // Note: The original reader never set this.
        }

    let readMethodBody (cenv: cenv) (methDef: MethodDefinition) =
        let mdReader = cenv.MetadataReader
        let attrs = methDef.Attributes
        let implAttrs = methDef.ImplAttributes

        let isPInvoke = int (attrs &&& MethodAttributes.PinvokeImpl) <> 0
        let codeType = int (implAttrs &&& MethodImplAttributes.CodeTypeMask)

        if codeType = 0x01 && isPInvoke then
            MethodBody.Native
        elif isPInvoke then
            let import = methDef.GetImport()
            let importAttrs = import.Attributes
            let pInvokeMethod : PInvokeMethod =
                {
                    Where = readILModuleRefFromModuleReference cenv (mdReader.GetModuleReference(import.Module))
                    Name = mdReader.GetString(import.Name)
                    CallingConv = mkPInvokeCallingConvention importAttrs
                    CharEncoding = mkPInvokeCharEncoding importAttrs
                    NoMangle = int (importAttrs &&& MethodImportAttributes.ExactSpelling) <> 0
                    LastError = int (importAttrs &&& MethodImportAttributes.SetLastError) <> 0
                    ThrowOnUnmappableChar = mkPInvokeThrowOnUnmappableChar importAttrs
                    CharBestFit = mkPInvokeCharBestFit importAttrs
                }
            MethodBody.PInvoke(pInvokeMethod)
        elif codeType <> 0x00 || int (attrs &&& MethodAttributes.Abstract) <> 0 || int (implAttrs &&& MethodImplAttributes.InternalCall) <> 0 || int (implAttrs &&& MethodImplAttributes.Unmanaged) <> 0 then
            MethodBody.Abstract
        elif not cenv.IsMetadataOnly then
            MethodBody.IL(readILMethodBody cenv methDef)
        else
            MethodBody.NotAvailable

    let readILMethodDef (cenv: cenv) (methDefHandle: MethodDefinitionHandle) =
        let mdReader = cenv.MetadataReader

        let methDef = mdReader.GetMethodDefinition(methDefHandle)
        let si = methDef.DecodeSignature(cenv.SignatureTypeProvider, ())

        let isEntryPoint =
            let handle = MetadataTokens.MethodDefinitionHandle cenv.EntryPointToken
            handle = methDefHandle

        let ret, parameters = readILParameters cenv si methDef

        ILMethodDef(
            name = mdReader.GetString(methDef.Name),
            attributes = methDef.Attributes,
            implAttributes = methDef.ImplAttributes,
            callingConv = mkILCallingConv si.Header,
            parameters = parameters,
            ret = ret,
            body = mkMethBodyLazyAux (lazy readMethodBody cenv methDef),
            isEntryPoint = isEntryPoint,
            genericParams = readILGenericParameterDefs cenv (methDef.GetGenericParameters()),
            securityDeclsStored = readILSecurityDeclsStored cenv (methDef.GetDeclarativeSecurityAttributes()),
            customAttrsStored = readILAttributesStored cenv (methDef.GetCustomAttributes()),
            metadataIndex = MetadataTokens.GetRowNumber(MethodDefinitionHandle.op_Implicit methDefHandle))

    let readILFieldDef (cenv: cenv) (fieldDefHandle: FieldDefinitionHandle) : ILFieldDef =
        let mdReader = cenv.MetadataReader

        let fieldDef = mdReader.GetFieldDefinition(fieldDefHandle)

        let data = 
            if not cenv.IsMetadataOnly && int (fieldDef.Attributes &&& FieldAttributes.HasFieldRVA) <> 0 then
                cenv.PEReader.GetSectionData(fieldDef.GetRelativeVirtualAddress()).GetContent().ToArray() // We should just return the immutable array instead of making a copy....
                |> Some
            else
                None

        let literalValue =
            if int (fieldDef.Attributes &&& FieldAttributes.HasDefault) <> 0 then
                tryReadILFieldInit cenv (fieldDef.GetDefaultValue())
            else
                None

        let offset =
            let isStatic = int (fieldDef.Attributes &&& FieldAttributes.Static) <> 0
            if not isStatic then
                let offset = fieldDef.GetOffset()
                if offset = -1 then None else Some(offset)
            else
                None
            
        let marshal =
            if int (fieldDef.Attributes &&& FieldAttributes.HasFieldMarshal) <> 0 then 
                tryReadILNativeType cenv (fieldDef.GetMarshallingDescriptor())
            else
                None

        ILFieldDef(
            name = mdReader.GetString(fieldDef.Name),
            fieldType = fieldDef.DecodeSignature(cenv.SignatureTypeProvider, ()),
            attributes = fieldDef.Attributes,
            data = data,
            literalValue = literalValue,
            offset = offset,
            marshal = marshal,
            customAttrsStored = readILAttributesStored cenv (fieldDef.GetCustomAttributes()),
            metadataIndex = MetadataTokens.GetRowNumber(FieldDefinitionHandle.op_Implicit fieldDefHandle))

    let readILFieldSpec (cenv: cenv) (handle: EntityHandle) : ILFieldSpec =
        let mdReader = cenv.MetadataReader

        match handle.Kind with
        | HandleKind.FieldDefinition ->
            let fieldDef = mdReader.GetFieldDefinition(FieldDefinitionHandle.op_Explicit(handle))

            let declaringILType = readILTypeFromTypeDefinition cenv (fieldDef.GetDeclaringType())

            let ilFieldRef =
                {
                    DeclaringTypeRef = declaringILType.TypeRef
                    Name = mdReader.GetString(fieldDef.Name)
                    Type = fieldDef.DecodeSignature(cenv.SignatureTypeProvider, ())
                }

            {
                FieldRef = ilFieldRef
                DeclaringType = declaringILType
            }

        | HandleKind.MemberReference ->
            let memberRef = mdReader.GetMemberReference(MemberReferenceHandle.op_Explicit(handle))

            let declaringType = readILType cenv memberRef.Parent

            let ilFieldRef =
                {
                    DeclaringTypeRef = declaringType.TypeRef
                    Name = mdReader.GetString(memberRef.Name)
                    Type = memberRef.DecodeFieldSignature(cenv.SignatureTypeProvider, ())
                }

            {
                FieldRef = ilFieldRef
                DeclaringType = declaringType
            }

        | _ -> failwithf "Invalid Handle Kind: %A" handle.Kind


    let readILFieldDefs (cenv: cenv) (fieldDefHandles: FieldDefinitionHandleCollection) =
        let f =
            lazy
                fieldDefHandles
                |> Seq.map (readILFieldDef cenv)
                |> List.ofSeq
        mkILFieldsLazy f

    let readILPropertyDef (cenv: cenv) (propDefHandle: PropertyDefinitionHandle) =
        let mdReader = cenv.MetadataReader

        let propDef = mdReader.GetPropertyDefinition(propDefHandle)
        let si = propDef.DecodeSignature(cenv.SignatureTypeProvider, ())
        let accessors = propDef.GetAccessors()

        let getMethod =
            if accessors.Getter.IsNil then None
            else
                let spec = readILMethodSpec cenv (MethodDefinitionHandle.op_Implicit(accessors.Getter))
                Some(spec.MethodRef)

        let setMethod =
            if accessors.Setter.IsNil then None
            else
                let spec = readILMethodSpec cenv (MethodDefinitionHandle.op_Implicit(accessors.Setter))
                Some(spec.MethodRef)

        let init =
            if int (propDef.Attributes &&& PropertyAttributes.HasDefault) <> 0 then
                tryReadILFieldInit cenv (propDef.GetDefaultValue())
            else
                None

        let args = si.ParameterTypes |> List.ofSeq

        ILPropertyDef(
            name = mdReader.GetString(propDef.Name),
            attributes = propDef.Attributes,
            setMethod = setMethod,
            getMethod = getMethod,
            callingConv = mkILThisConvention si.Header,
            propertyType = si.ReturnType,
            init = init,
            args = args,
            customAttrsStored = readILAttributesStored cenv (propDef.GetCustomAttributes()),
            metadataIndex = MetadataTokens.GetRowNumber(PropertyDefinitionHandle.op_Implicit propDefHandle))

    let readILPropertyDefs (cenv: cenv) (propDefHandles: PropertyDefinitionHandleCollection) =
        let f =
            lazy
                propDefHandles
                |> Seq.map (readILPropertyDef cenv)
                |> List.ofSeq
        mkILPropertiesLazy f

    let readILOverridesSpec (cenv: cenv) (handle: EntityHandle) =
        let ilMethSpec = readILMethodSpec cenv handle
        OverridesSpec(ilMethSpec.MethodRef, ilMethSpec.DeclaringType)

    let readILMethodImpl (cenv: cenv) (methImplHandle: MethodImplementationHandle) =
        let mdReader = cenv.MetadataReader

        let methImpl = mdReader.GetMethodImplementation(methImplHandle)

        {
            OverrideBy = readILMethodSpec cenv methImpl.MethodBody
            Overrides = readILOverridesSpec cenv methImpl.MethodDeclaration
        }

    let readILMethodImpls (cenv: cenv) (methImplHandles: MethodImplementationHandleCollection) =
        let f =
            lazy
                methImplHandles
                |> Seq.map (readILMethodImpl cenv)
                |> List.ofSeq
        mkILMethodImplsLazy f

    let readILMethodRef (cenv: cenv) (handle: EntityHandle) =
        (readILMethodSpec cenv handle).MethodRef

    let tryReadILMethodRef (cenv: cenv) (handle: EntityHandle) =
        if handle.IsNil then None
        else
            readILMethodRef cenv handle
            |> Some

    let tryReadILType (cenv: cenv) (handle: EntityHandle) =
        if handle.IsNil then None
        else
            readILType cenv handle
            |> Some

    let readILEventDef (cenv: cenv) (eventDefHandle: EventDefinitionHandle) =
        let mdReader = cenv.MetadataReader

        let eventDef = mdReader.GetEventDefinition(eventDefHandle)
        let accessors = eventDef.GetAccessors()

        let otherMethods = accessors.Others |> Seq.map (fun h -> readILMethodRef cenv (MethodDefinitionHandle.op_Implicit(h))) |> List.ofSeq

        ILEventDef(
            eventType = tryReadILType cenv eventDef.Type,
            name = mdReader.GetString(eventDef.Name),
            attributes = eventDef.Attributes,
            addMethod = readILMethodRef cenv (MethodDefinitionHandle.op_Implicit(accessors.Adder)),
            removeMethod = readILMethodRef cenv (MethodDefinitionHandle.op_Implicit(accessors.Remover)),
            fireMethod = tryReadILMethodRef cenv (MethodDefinitionHandle.op_Implicit(accessors.Raiser)),
            otherMethods = otherMethods,
            customAttrsStored = readILAttributesStored cenv (eventDef.GetCustomAttributes()),
            metadataIndex = MetadataTokens.GetRowNumber(EventDefinitionHandle.op_Implicit eventDefHandle))
 
    let readILEventDefs (cenv: cenv) (eventDefHandles: EventDefinitionHandleCollection) =
        let f =
            lazy
                eventDefHandles
                |> Seq.map (readILEventDef cenv)
                |> List.ofSeq
        mkILEventsLazy f

    let rec readILTypeDef (cenv: cenv) (typeDefHandle: TypeDefinitionHandle) =
        let mdReader = cenv.MetadataReader

        let typeDef = mdReader.GetTypeDefinition(typeDefHandle)

        let name = mdReader.GetTypeName(typeDef.Namespace, typeDef.Name)

        let implements =
            typeDef.GetInterfaceImplementations()
            |> Seq.map (fun h ->
                let interfaceImpl = mdReader.GetInterfaceImplementation(h)
                readILType cenv interfaceImpl.Interface
            )
            |> List.ofSeq

        let genericParams = readILGenericParameterDefs cenv (typeDef.GetGenericParameters())

        let extends =
            if typeDef.BaseType.IsNil then None
            else Some(readILType cenv typeDef.BaseType)

        let methods =
            mkILMethodsComputed (fun () ->
                let methDefHandles = typeDef.GetMethods()
                let ilMethodDefs = Array.zeroCreate methDefHandles.Count

                let mutable i = 0
                for methDefHandle in methDefHandles do
                    ilMethodDefs.[i] <- readILMethodDef cenv methDefHandle
                    i <- i + 1

                ilMethodDefs
            )

        let nestedTypes =
            mkILTypeDefsComputed (fun () ->
                typeDef.GetNestedTypes()
                |> Seq.map (fun h -> 
                    readILPreTypeDef cenv h
                )
                |> Array.ofSeq
            )

        let ilTypeDefLayout = mkILTypeDefLayout typeDef.Attributes (typeDef.GetLayout())

        ILTypeDef(
            name = name,
            attributes = typeDef.Attributes,
            layout = ilTypeDefLayout,
            implements = implements,
            genericParams = genericParams,
            extends = extends,
            methods = methods,
            nestedTypes = nestedTypes,
            fields = readILFieldDefs cenv (typeDef.GetFields()),
            methodImpls = readILMethodImpls cenv (typeDef.GetMethodImplementations()),
            events = readILEventDefs cenv (typeDef.GetEvents()),
            properties = readILPropertyDefs cenv (typeDef.GetProperties()),
            securityDeclsStored = readILSecurityDeclsStored cenv (typeDef.GetDeclarativeSecurityAttributes()),
            customAttrsStored = readILAttributesStored cenv (typeDef.GetCustomAttributes()),
            metadataIndex = MetadataTokens.GetRowNumber(TypeDefinitionHandle.op_Implicit(typeDefHandle))
        )

    let readILPreTypeDef (cenv: cenv) (typeDefHandle: TypeDefinitionHandle) =
        let mdReader = cenv.MetadataReader

        let typeDef = mdReader.GetTypeDefinition(typeDefHandle)

        let namespaceOpt = mdReader.TryGetString(typeDef.Namespace)
        let name = mdReader.GetString(typeDef.Name)

        let namespaceSplit =
            match namespaceOpt with
            | ValueNone -> []
            | ValueSome(namespac) -> splitNamespace namespac

        mkILPreTypeDefComputed (namespaceSplit, name, (fun () -> readILTypeDef cenv typeDefHandle))

    let readILPreTypeDefs (cenv: cenv) = 
        let mdReader = cenv.MetadataReader

        [|
            for typeDefHandle in mdReader.TypeDefinitions do
                let typeDef = mdReader.GetTypeDefinition(typeDefHandle)
                // Only get top types.
                if not typeDef.IsNested then
                    yield readILPreTypeDef cenv typeDefHandle
        |]

    let readILResources (cenv: cenv) =
        let mdReader = cenv.MetadataReader

        mdReader.ManifestResources
        |> Seq.map mdReader.GetManifestResource
        |> Seq.choose (fun resource ->
            if resource.Implementation.IsNil then None
            else
                let location =
                    match readILScopeRef cenv resource.Implementation with
                    | ILScopeRef.Local ->
                        let bytes =
                            let bytes = mdReader.GetBlobBytes(MetadataTokens.BlobHandle(int resource.Offset))
                            if cenv.CanReduceMemory then
                                ByteMemory.CreateMemoryMappedFile (ByteMemory.FromFile(mdReader.GetString resource.Name, FileAccess.Read, canShadowCopy=true).AsReadOnly())
                            else
                                ByteMemory.FromArray bytes
                        ILResourceLocation.Local(bytes.AsReadOnly())
                    
                    | ILScopeRef.Module mref -> ILResourceLocation.File (mref, int resource.Offset)
                    | ILScopeRef.Assembly aref -> ILResourceLocation.Assembly aref

                Some
                    {
                        Name = mdReader.GetString resource.Name
                        Location = location
                        Access = (if resource.Attributes &&& ManifestResourceAttributes.Public = ManifestResourceAttributes.Public then ILResourceAccess.Public else ILResourceAccess.Private)
                        CustomAttrsStored = resource.GetCustomAttributes() |> readILAttributesStored cenv
                        MetadataIndex = MetadataTokens.GetRowNumber(resource.Implementation)
                    })
        |> List.ofSeq
        |> mkILResources

    let readModuleDef ilGlobals (peReader: PEReader) isMetadataOnly canReduceMemory (pdbReaderProviderOpt: PdbReaderProvider option) =
        let nativeResources = readILNativeResources peReader

        let subsys =
            int16 peReader.PEHeaders.PEHeader.Subsystem

        let subsysversion =
            (int32 peReader.PEHeaders.PEHeader.MajorSubsystemVersion, int32 peReader.PEHeaders.PEHeader.MinorSubsystemVersion)

        let useHighEntropyVA =
            int (peReader.PEHeaders.PEHeader.DllCharacteristics &&& DllCharacteristics.HighEntropyVirtualAddressSpace) <> 0

        let ilOnly =
            int (peReader.PEHeaders.CorHeader.Flags &&& CorFlags.ILOnly) <> 0

        let only32 =
            int (peReader.PEHeaders.CorHeader.Flags &&& CorFlags.Requires32Bit) <> 0

        let is32bitpreferred =
            int (peReader.PEHeaders.CorHeader.Flags &&& CorFlags.Prefers32Bit) <> 0

        let only64 =
            peReader.PEHeaders.CoffHeader.SizeOfOptionalHeader = 240s (* May want to read in the optional header Magic number and check that as well... *)

        let platform = 
            match peReader.PEHeaders.CoffHeader.Machine with
            | Machine.Amd64 -> Some(AMD64)
            | Machine.IA64 -> Some(IA64)
            | _ -> Some(X86)

        let isDll = peReader.PEHeaders.IsDll

        let alignVirt =
            peReader.PEHeaders.PEHeader.SectionAlignment

        let alignPhys =
            peReader.PEHeaders.PEHeader.FileAlignment

        let imageBaseReal = int peReader.PEHeaders.PEHeader.ImageBase

        let entryPointToken = peReader.PEHeaders.CorHeader.EntryPointTokenOrRelativeVirtualAddress

        let mdReader = peReader.GetMetadataReader()
        let moduleDef = mdReader.GetModuleDefinition()
        let ilModuleName = mdReader.GetString(moduleDef.Name)
        let ilMetadataVersion = mdReader.MetadataVersion
    
        let cenv = 
            let sigTyProvider = SignatureTypeProvider(ilGlobals)
            let localSigTyProvider = LocalSignatureTypeProvider(ilGlobals)
            let cenv = cenv(peReader, mdReader, pdbReaderProviderOpt, entryPointToken, isMetadataOnly, canReduceMemory, sigTyProvider, localSigTyProvider)
            sigTyProvider.cenv <- cenv
            localSigTyProvider.cenv <- cenv
            cenv

        let ilAsmRefs =
            let asmRefs = mdReader.AssemblyReferences

            let arr = Array.zeroCreate asmRefs.Count
            let mutable i = 0
            for asmRefHandle in asmRefs do
                arr.[i] <- readILAssemblyRefFromAssemblyReference cenv asmRefHandle
                i <- i + 1
            arr |> List.ofArray

        { Manifest = Some(readILAssemblyManifest cenv entryPointToken)
          CustomAttrsStored = readILAttributesStored cenv (moduleDef.GetCustomAttributes())
          MetadataIndex = 1 // Note: The original reader set this to 1.
          Name = ilModuleName
          NativeResources = nativeResources
          TypeDefs = mkILTypeDefsComputed (fun () -> readILPreTypeDefs cenv)
          SubSystemFlags = int32 subsys
          IsILOnly = ilOnly
          SubsystemVersion = subsysversion
          UseHighEntropyVA = useHighEntropyVA
          Platform = platform
          StackReserveSize = None  // TODO - Note: The original reader did not set this and was marked as a TODO.
          Is32Bit = only32
          Is32BitPreferred = is32bitpreferred
          Is64Bit = only64
          IsDLL=isDll
          VirtualAlignment = alignVirt
          PhysicalAlignment = alignPhys
          ImageBase = imageBaseReal
          MetadataVersion = ilMetadataVersion
          Resources = readILResources cenv
        }, ilAsmRefs

module ILBinaryReader =

    type ILReaderMetadataSnapshot = (obj * nativeint * int) 
    type ILReaderTryGetMetadataSnapshot = (* path: *) string * (* snapshotTimeStamp: *) System.DateTime -> ILReaderMetadataSnapshot option

    [<RequireQualifiedAccess>]
    type MetadataOnlyFlag = Yes | No

    [<RequireQualifiedAccess>]
    type ReduceMemoryFlag = Yes | No

    type ILReaderOptions =
        { pdbDirPath: string option
          ilGlobals: ILGlobals
          reduceMemoryUsage: ReduceMemoryFlag
          metadataOnly: MetadataOnlyFlag
          tryGetMetadataSnapshot: ILReaderTryGetMetadataSnapshot }

    type ILModuleReader =
        abstract ILModuleDef : ILModuleDef
        abstract ILAssemblyRefs : ILAssemblyRef list
    
        /// ILModuleReader objects only need to be explicitly disposed if memory mapping is used, i.e. reduceMemoryUsage = false
        inherit  System.IDisposable

    type Statistics = 
        { mutable rawMemoryFileCount : int
          mutable memoryMapFileOpenedCount : int
          mutable memoryMapFileClosedCount : int
          mutable weakByteFileCount : int
          mutable byteFileCount : int }

    let defaultStatistics = 
        { rawMemoryFileCount = 0
          memoryMapFileOpenedCount = 0
          memoryMapFileClosedCount = 0
          weakByteFileCount = 0
          byteFileCount = 0 }

    let GetStatistics() = defaultStatistics

    let OpenILModuleReaderAux (memory: ReadOnlyByteMemory) (opts: ILReaderOptions) =
        let peReader = new PEReader(memory.AsStream())
        let pdbReaderProviderOpt = 
            opts.pdbDirPath
            |> Option.bind (fun pdbDirPath ->
                let streamProvider = System.Func<_,_>(fun pdbPath -> ByteMemory.FromFile(pdbPath, FileAccess.Read, canShadowCopy=true).AsReadOnlyStream())
                match peReader.TryOpenAssociatedPortablePdb(pdbDirPath, streamProvider) with
                | true, pdbReaderProvider, pdbPath -> Some(pdbReaderProvider, pdbPath)
                | _ -> None)
        let ilModuleDef, ilAsmRefs = readModuleDef opts.ilGlobals peReader (opts.metadataOnly = MetadataOnlyFlag.Yes) (opts.reduceMemoryUsage = ReduceMemoryFlag.Yes) pdbReaderProviderOpt
        {   new Object() with
    
                override _.Finalize() =
                    peReader.Dispose()

            interface ILModuleReader with

                member _.ILModuleDef = ilModuleDef

                member _.ILAssemblyRefs = ilAsmRefs

            interface IDisposable with

                member _.Dispose() = () }

    let OpenILModuleReaderFromBytes (_fileNameForDebugOutput: string) assemblyContents opts =
        let memory = ByteMemory.FromArray assemblyContents
        OpenILModuleReaderAux (memory.AsReadOnly()) opts

    let OpenILModuleReaderFromFile fileName opts =
        let memory = ByteMemory.FromFile(fileName, FileAccess.Read, canShadowCopy=true)
        OpenILModuleReaderAux (memory.AsReadOnly()) opts

    type ILModuleReaderCacheKey = ILModuleReaderCacheKey of string * writeStamp: DateTime * ILScopeRef * bool * ReduceMemoryFlag * MetadataOnlyFlag with

        member x.WriteStamp =
            match x with
            | ILModuleReaderCacheKey(writeStamp=writeStamp) -> writeStamp

    /// Due to .NET Core including many assemblies, we set this cache size to 200 to ensure we have them in the cache as well as other assemblies.
    [<Literal>]
    let strongCacheSize = 200

    /// This is a reasonable limit if we truly exceed way outside the bounds of how many assemblies could be loaded.
    [<Literal>]
    let weakCacheSize = 1000

    let cacheKeyEquality =
        let stringComparer = StringComparer.OrdinalIgnoreCase
        { new System.Collections.Generic.IEqualityComparer<ILModuleReaderCacheKey> with

            member _.GetHashCode (ILModuleReaderCacheKey (fullPath,_,_,_,_,_)) = 
                stringComparer.GetHashCode fullPath
        
            member _.Equals(key1, key2) =
                let (ILModuleReaderCacheKey (fullPath1,_,scope1,hasPdbPath1,reduceMemoryUsage1,metadataOnlyFlag1)) = key1
                let (ILModuleReaderCacheKey (fullPath2,_,scope2,hasPdbPath2,reduceMemoryUsage2,metadataOnlyFlag2)) = key2
                stringComparer.Equals(fullPath1, fullPath2) && scope1 = scope2 && hasPdbPath1 = hasPdbPath2 &&
                reduceMemoryUsage1 = reduceMemoryUsage2 && metadataOnlyFlag1 = metadataOnlyFlag2 }
    let cache = LruWeakCache<ILModuleReaderCacheKey, ILModuleReader>.Create(strongCacheSize, weakCacheSize, cacheKeyEquality)

    let ClearAllILModuleReaderCache () =
        cache.Clear()

    let OpenILModuleReader fileName opts =
        let (ILModuleReaderCacheKey (fullPath,_,_,_,_,_) as key) = 
            let fullPath = FileSystem.GetFullPathShim fileName
            let writeTime = FileSystem.GetLastWriteTimeShim fileName
            ILModuleReaderCacheKey (fullPath, writeTime, opts.ilGlobals.primaryAssemblyScopeRef, opts.pdbDirPath.IsSome, opts.reduceMemoryUsage, opts.metadataOnly)

        cache.GetOrSet(key, 
            (fun () -> OpenILModuleReaderFromFile fullPath opts), 
            isExistingKeyValid = (fun existingKey -> existingKey.WriteStamp = key.WriteStamp))
        
    [<AutoOpen>]
    module Shim =

        type IAssemblyReader =
            abstract GetILModuleReader: filename: string * readerOptions: ILReaderOptions -> ILModuleReader

        [<Sealed>]
        type DefaultAssemblyReader() =
            interface IAssemblyReader with
                member __.GetILModuleReader(filename, readerOptions) =
                    OpenILModuleReader filename readerOptions

        let mutable AssemblyReader = DefaultAssemblyReader() :> IAssemblyReader