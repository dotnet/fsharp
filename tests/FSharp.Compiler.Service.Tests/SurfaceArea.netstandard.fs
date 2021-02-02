// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Tests.Service.SurfaceArea

open Tests.Service.SurfaceArea.LibraryTestFx
open NUnit.Framework

type SurfaceAreaTest() =
    [<Test>]
    member _.VerifyArea() =
        let expected = @"
FSharp.Compiler.AbstractIL.IL
FSharp.Compiler.AbstractIL.IL+ILArrayShape: Boolean Equals(ILArrayShape)
FSharp.Compiler.AbstractIL.IL+ILArrayShape: Boolean Equals(System.Object)
FSharp.Compiler.AbstractIL.IL+ILArrayShape: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILArrayShape: ILArrayShape FromRank(Int32)
FSharp.Compiler.AbstractIL.IL+ILArrayShape: ILArrayShape SingleDimensional
FSharp.Compiler.AbstractIL.IL+ILArrayShape: ILArrayShape get_SingleDimensional()
FSharp.Compiler.AbstractIL.IL+ILArrayShape: Int32 CompareTo(ILArrayShape)
FSharp.Compiler.AbstractIL.IL+ILArrayShape: Int32 CompareTo(System.Object)
FSharp.Compiler.AbstractIL.IL+ILArrayShape: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.AbstractIL.IL+ILArrayShape: Int32 GetHashCode()
FSharp.Compiler.AbstractIL.IL+ILArrayShape: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILArrayShape: Int32 Rank
FSharp.Compiler.AbstractIL.IL+ILArrayShape: Int32 get_Rank()
FSharp.Compiler.AbstractIL.IL+ILArrayShape: System.String ToString()
FSharp.Compiler.AbstractIL.IL+ILAssemblyLongevity: Boolean Equals(ILAssemblyLongevity)
FSharp.Compiler.AbstractIL.IL+ILAssemblyLongevity: Boolean Equals(System.Object)
FSharp.Compiler.AbstractIL.IL+ILAssemblyLongevity: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILAssemblyLongevity: ILAssemblyLongevity Default
FSharp.Compiler.AbstractIL.IL+ILAssemblyLongevity: ILAssemblyLongevity get_Default()
FSharp.Compiler.AbstractIL.IL+ILAssemblyLongevity: Int32 CompareTo(ILAssemblyLongevity)
FSharp.Compiler.AbstractIL.IL+ILAssemblyLongevity: Int32 CompareTo(System.Object)
FSharp.Compiler.AbstractIL.IL+ILAssemblyLongevity: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.AbstractIL.IL+ILAssemblyLongevity: Int32 GetHashCode()
FSharp.Compiler.AbstractIL.IL+ILAssemblyLongevity: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILAssemblyLongevity: System.String ToString()
FSharp.Compiler.AbstractIL.IL+ILAssemblyManifest: Boolean DisableJitOptimizations
FSharp.Compiler.AbstractIL.IL+ILAssemblyManifest: Boolean IgnoreSymbolStoreSequencePoints
FSharp.Compiler.AbstractIL.IL+ILAssemblyManifest: Boolean JitTracking
FSharp.Compiler.AbstractIL.IL+ILAssemblyManifest: Boolean Retargetable
FSharp.Compiler.AbstractIL.IL+ILAssemblyManifest: Boolean get_DisableJitOptimizations()
FSharp.Compiler.AbstractIL.IL+ILAssemblyManifest: Boolean get_IgnoreSymbolStoreSequencePoints()
FSharp.Compiler.AbstractIL.IL+ILAssemblyManifest: Boolean get_JitTracking()
FSharp.Compiler.AbstractIL.IL+ILAssemblyManifest: Boolean get_Retargetable()
FSharp.Compiler.AbstractIL.IL+ILAssemblyManifest: ILAssemblyLongevity AssemblyLongevity
FSharp.Compiler.AbstractIL.IL+ILAssemblyManifest: ILAssemblyLongevity get_AssemblyLongevity()
FSharp.Compiler.AbstractIL.IL+ILAssemblyManifest: ILAttributes CustomAttrs
FSharp.Compiler.AbstractIL.IL+ILAssemblyManifest: ILAttributes get_CustomAttrs()
FSharp.Compiler.AbstractIL.IL+ILAssemblyManifest: ILAttributesStored CustomAttrsStored
FSharp.Compiler.AbstractIL.IL+ILAssemblyManifest: ILAttributesStored get_CustomAttrsStored()
FSharp.Compiler.AbstractIL.IL+ILAssemblyManifest: ILExportedTypesAndForwarders ExportedTypes
FSharp.Compiler.AbstractIL.IL+ILAssemblyManifest: ILExportedTypesAndForwarders get_ExportedTypes()
FSharp.Compiler.AbstractIL.IL+ILAssemblyManifest: ILSecurityDecls SecurityDecls
FSharp.Compiler.AbstractIL.IL+ILAssemblyManifest: ILSecurityDecls get_SecurityDecls()
FSharp.Compiler.AbstractIL.IL+ILAssemblyManifest: ILSecurityDeclsStored SecurityDeclsStored
FSharp.Compiler.AbstractIL.IL+ILAssemblyManifest: ILSecurityDeclsStored get_SecurityDeclsStored()
FSharp.Compiler.AbstractIL.IL+ILAssemblyManifest: Int32 AuxModuleHashAlgorithm
FSharp.Compiler.AbstractIL.IL+ILAssemblyManifest: Int32 MetadataIndex
FSharp.Compiler.AbstractIL.IL+ILAssemblyManifest: Int32 get_AuxModuleHashAlgorithm()
FSharp.Compiler.AbstractIL.IL+ILAssemblyManifest: Int32 get_MetadataIndex()
FSharp.Compiler.AbstractIL.IL+ILAssemblyManifest: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILModuleRef] EntrypointElsewhere
FSharp.Compiler.AbstractIL.IL+ILAssemblyManifest: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILModuleRef] get_EntrypointElsewhere()
FSharp.Compiler.AbstractIL.IL+ILAssemblyManifest: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILVersionInfo] Version
FSharp.Compiler.AbstractIL.IL+ILAssemblyManifest: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILVersionInfo] get_Version()
FSharp.Compiler.AbstractIL.IL+ILAssemblyManifest: Microsoft.FSharp.Core.FSharpOption`1[System.Byte[]] PublicKey
FSharp.Compiler.AbstractIL.IL+ILAssemblyManifest: Microsoft.FSharp.Core.FSharpOption`1[System.Byte[]] get_PublicKey()
FSharp.Compiler.AbstractIL.IL+ILAssemblyManifest: Microsoft.FSharp.Core.FSharpOption`1[System.String] Locale
FSharp.Compiler.AbstractIL.IL+ILAssemblyManifest: Microsoft.FSharp.Core.FSharpOption`1[System.String] get_Locale()
FSharp.Compiler.AbstractIL.IL+ILAssemblyManifest: System.String Name
FSharp.Compiler.AbstractIL.IL+ILAssemblyManifest: System.String ToString()
FSharp.Compiler.AbstractIL.IL+ILAssemblyManifest: System.String get_Name()
FSharp.Compiler.AbstractIL.IL+ILAssemblyManifest: Void .ctor(System.String, Int32, ILSecurityDeclsStored, Microsoft.FSharp.Core.FSharpOption`1[System.Byte[]], Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILVersionInfo], Microsoft.FSharp.Core.FSharpOption`1[System.String], ILAttributesStored, ILAssemblyLongevity, Boolean, Boolean, Boolean, Boolean, ILExportedTypesAndForwarders, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILModuleRef], Int32)
FSharp.Compiler.AbstractIL.IL+ILAssemblyRef: Boolean Equals(System.Object)
FSharp.Compiler.AbstractIL.IL+ILAssemblyRef: Boolean EqualsIgnoringVersion(ILAssemblyRef)
FSharp.Compiler.AbstractIL.IL+ILAssemblyRef: Boolean Retargetable
FSharp.Compiler.AbstractIL.IL+ILAssemblyRef: Boolean get_Retargetable()
FSharp.Compiler.AbstractIL.IL+ILAssemblyRef: ILAssemblyRef Create(System.String, Microsoft.FSharp.Core.FSharpOption`1[System.Byte[]], Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+PublicKey], Boolean, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILVersionInfo], Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.AbstractIL.IL+ILAssemblyRef: ILAssemblyRef FromAssemblyName(System.Reflection.AssemblyName)
FSharp.Compiler.AbstractIL.IL+ILAssemblyRef: Int32 GetHashCode()
FSharp.Compiler.AbstractIL.IL+ILAssemblyRef: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILVersionInfo] Version
FSharp.Compiler.AbstractIL.IL+ILAssemblyRef: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILVersionInfo] get_Version()
FSharp.Compiler.AbstractIL.IL+ILAssemblyRef: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+PublicKey] PublicKey
FSharp.Compiler.AbstractIL.IL+ILAssemblyRef: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+PublicKey] get_PublicKey()
FSharp.Compiler.AbstractIL.IL+ILAssemblyRef: Microsoft.FSharp.Core.FSharpOption`1[System.Byte[]] Hash
FSharp.Compiler.AbstractIL.IL+ILAssemblyRef: Microsoft.FSharp.Core.FSharpOption`1[System.Byte[]] get_Hash()
FSharp.Compiler.AbstractIL.IL+ILAssemblyRef: Microsoft.FSharp.Core.FSharpOption`1[System.String] Locale
FSharp.Compiler.AbstractIL.IL+ILAssemblyRef: Microsoft.FSharp.Core.FSharpOption`1[System.String] get_Locale()
FSharp.Compiler.AbstractIL.IL+ILAssemblyRef: System.String Name
FSharp.Compiler.AbstractIL.IL+ILAssemblyRef: System.String QualifiedName
FSharp.Compiler.AbstractIL.IL+ILAssemblyRef: System.String get_Name()
FSharp.Compiler.AbstractIL.IL+ILAssemblyRef: System.String get_QualifiedName()
FSharp.Compiler.AbstractIL.IL+ILAttribElem+Array: ILType Item1
FSharp.Compiler.AbstractIL.IL+ILAttribElem+Array: ILType get_Item1()
FSharp.Compiler.AbstractIL.IL+ILAttribElem+Array: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILAttribElem] Item2
FSharp.Compiler.AbstractIL.IL+ILAttribElem+Array: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILAttribElem] get_Item2()
FSharp.Compiler.AbstractIL.IL+ILAttribElem+Bool: Boolean Item
FSharp.Compiler.AbstractIL.IL+ILAttribElem+Bool: Boolean get_Item()
FSharp.Compiler.AbstractIL.IL+ILAttribElem+Byte: Byte Item
FSharp.Compiler.AbstractIL.IL+ILAttribElem+Byte: Byte get_Item()
FSharp.Compiler.AbstractIL.IL+ILAttribElem+Char: Char Item
FSharp.Compiler.AbstractIL.IL+ILAttribElem+Char: Char get_Item()
FSharp.Compiler.AbstractIL.IL+ILAttribElem+Double: Double Item
FSharp.Compiler.AbstractIL.IL+ILAttribElem+Double: Double get_Item()
FSharp.Compiler.AbstractIL.IL+ILAttribElem+Int16: Int16 Item
FSharp.Compiler.AbstractIL.IL+ILAttribElem+Int16: Int16 get_Item()
FSharp.Compiler.AbstractIL.IL+ILAttribElem+Int32: Int32 Item
FSharp.Compiler.AbstractIL.IL+ILAttribElem+Int32: Int32 get_Item()
FSharp.Compiler.AbstractIL.IL+ILAttribElem+Int64: Int64 Item
FSharp.Compiler.AbstractIL.IL+ILAttribElem+Int64: Int64 get_Item()
FSharp.Compiler.AbstractIL.IL+ILAttribElem+SByte: SByte Item
FSharp.Compiler.AbstractIL.IL+ILAttribElem+SByte: SByte get_Item()
FSharp.Compiler.AbstractIL.IL+ILAttribElem+Single: Single Item
FSharp.Compiler.AbstractIL.IL+ILAttribElem+Single: Single get_Item()
FSharp.Compiler.AbstractIL.IL+ILAttribElem+String: Microsoft.FSharp.Core.FSharpOption`1[System.String] Item
FSharp.Compiler.AbstractIL.IL+ILAttribElem+String: Microsoft.FSharp.Core.FSharpOption`1[System.String] get_Item()
FSharp.Compiler.AbstractIL.IL+ILAttribElem+Tags: Int32 Array
FSharp.Compiler.AbstractIL.IL+ILAttribElem+Tags: Int32 Bool
FSharp.Compiler.AbstractIL.IL+ILAttribElem+Tags: Int32 Byte
FSharp.Compiler.AbstractIL.IL+ILAttribElem+Tags: Int32 Char
FSharp.Compiler.AbstractIL.IL+ILAttribElem+Tags: Int32 Double
FSharp.Compiler.AbstractIL.IL+ILAttribElem+Tags: Int32 Int16
FSharp.Compiler.AbstractIL.IL+ILAttribElem+Tags: Int32 Int32
FSharp.Compiler.AbstractIL.IL+ILAttribElem+Tags: Int32 Int64
FSharp.Compiler.AbstractIL.IL+ILAttribElem+Tags: Int32 Null
FSharp.Compiler.AbstractIL.IL+ILAttribElem+Tags: Int32 SByte
FSharp.Compiler.AbstractIL.IL+ILAttribElem+Tags: Int32 Single
FSharp.Compiler.AbstractIL.IL+ILAttribElem+Tags: Int32 String
FSharp.Compiler.AbstractIL.IL+ILAttribElem+Tags: Int32 Type
FSharp.Compiler.AbstractIL.IL+ILAttribElem+Tags: Int32 TypeRef
FSharp.Compiler.AbstractIL.IL+ILAttribElem+Tags: Int32 UInt16
FSharp.Compiler.AbstractIL.IL+ILAttribElem+Tags: Int32 UInt32
FSharp.Compiler.AbstractIL.IL+ILAttribElem+Tags: Int32 UInt64
FSharp.Compiler.AbstractIL.IL+ILAttribElem+Type: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILType] Item
FSharp.Compiler.AbstractIL.IL+ILAttribElem+Type: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILType] get_Item()
FSharp.Compiler.AbstractIL.IL+ILAttribElem+TypeRef: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILTypeRef] Item
FSharp.Compiler.AbstractIL.IL+ILAttribElem+TypeRef: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILTypeRef] get_Item()
FSharp.Compiler.AbstractIL.IL+ILAttribElem+UInt16: UInt16 Item
FSharp.Compiler.AbstractIL.IL+ILAttribElem+UInt16: UInt16 get_Item()
FSharp.Compiler.AbstractIL.IL+ILAttribElem+UInt32: UInt32 Item
FSharp.Compiler.AbstractIL.IL+ILAttribElem+UInt32: UInt32 get_Item()
FSharp.Compiler.AbstractIL.IL+ILAttribElem+UInt64: UInt64 Item
FSharp.Compiler.AbstractIL.IL+ILAttribElem+UInt64: UInt64 get_Item()
FSharp.Compiler.AbstractIL.IL+ILAttribElem: Boolean Equals(ILAttribElem)
FSharp.Compiler.AbstractIL.IL+ILAttribElem: Boolean Equals(System.Object)
FSharp.Compiler.AbstractIL.IL+ILAttribElem: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILAttribElem: Boolean IsArray
FSharp.Compiler.AbstractIL.IL+ILAttribElem: Boolean IsBool
FSharp.Compiler.AbstractIL.IL+ILAttribElem: Boolean IsByte
FSharp.Compiler.AbstractIL.IL+ILAttribElem: Boolean IsChar
FSharp.Compiler.AbstractIL.IL+ILAttribElem: Boolean IsDouble
FSharp.Compiler.AbstractIL.IL+ILAttribElem: Boolean IsInt16
FSharp.Compiler.AbstractIL.IL+ILAttribElem: Boolean IsInt32
FSharp.Compiler.AbstractIL.IL+ILAttribElem: Boolean IsInt64
FSharp.Compiler.AbstractIL.IL+ILAttribElem: Boolean IsNull
FSharp.Compiler.AbstractIL.IL+ILAttribElem: Boolean IsSByte
FSharp.Compiler.AbstractIL.IL+ILAttribElem: Boolean IsSingle
FSharp.Compiler.AbstractIL.IL+ILAttribElem: Boolean IsString
FSharp.Compiler.AbstractIL.IL+ILAttribElem: Boolean IsType
FSharp.Compiler.AbstractIL.IL+ILAttribElem: Boolean IsTypeRef
FSharp.Compiler.AbstractIL.IL+ILAttribElem: Boolean IsUInt16
FSharp.Compiler.AbstractIL.IL+ILAttribElem: Boolean IsUInt32
FSharp.Compiler.AbstractIL.IL+ILAttribElem: Boolean IsUInt64
FSharp.Compiler.AbstractIL.IL+ILAttribElem: Boolean get_IsArray()
FSharp.Compiler.AbstractIL.IL+ILAttribElem: Boolean get_IsBool()
FSharp.Compiler.AbstractIL.IL+ILAttribElem: Boolean get_IsByte()
FSharp.Compiler.AbstractIL.IL+ILAttribElem: Boolean get_IsChar()
FSharp.Compiler.AbstractIL.IL+ILAttribElem: Boolean get_IsDouble()
FSharp.Compiler.AbstractIL.IL+ILAttribElem: Boolean get_IsInt16()
FSharp.Compiler.AbstractIL.IL+ILAttribElem: Boolean get_IsInt32()
FSharp.Compiler.AbstractIL.IL+ILAttribElem: Boolean get_IsInt64()
FSharp.Compiler.AbstractIL.IL+ILAttribElem: Boolean get_IsNull()
FSharp.Compiler.AbstractIL.IL+ILAttribElem: Boolean get_IsSByte()
FSharp.Compiler.AbstractIL.IL+ILAttribElem: Boolean get_IsSingle()
FSharp.Compiler.AbstractIL.IL+ILAttribElem: Boolean get_IsString()
FSharp.Compiler.AbstractIL.IL+ILAttribElem: Boolean get_IsType()
FSharp.Compiler.AbstractIL.IL+ILAttribElem: Boolean get_IsTypeRef()
FSharp.Compiler.AbstractIL.IL+ILAttribElem: Boolean get_IsUInt16()
FSharp.Compiler.AbstractIL.IL+ILAttribElem: Boolean get_IsUInt32()
FSharp.Compiler.AbstractIL.IL+ILAttribElem: Boolean get_IsUInt64()
FSharp.Compiler.AbstractIL.IL+ILAttribElem: FSharp.Compiler.AbstractIL.IL+ILAttribElem+Array
FSharp.Compiler.AbstractIL.IL+ILAttribElem: FSharp.Compiler.AbstractIL.IL+ILAttribElem+Bool
FSharp.Compiler.AbstractIL.IL+ILAttribElem: FSharp.Compiler.AbstractIL.IL+ILAttribElem+Byte
FSharp.Compiler.AbstractIL.IL+ILAttribElem: FSharp.Compiler.AbstractIL.IL+ILAttribElem+Char
FSharp.Compiler.AbstractIL.IL+ILAttribElem: FSharp.Compiler.AbstractIL.IL+ILAttribElem+Double
FSharp.Compiler.AbstractIL.IL+ILAttribElem: FSharp.Compiler.AbstractIL.IL+ILAttribElem+Int16
FSharp.Compiler.AbstractIL.IL+ILAttribElem: FSharp.Compiler.AbstractIL.IL+ILAttribElem+Int32
FSharp.Compiler.AbstractIL.IL+ILAttribElem: FSharp.Compiler.AbstractIL.IL+ILAttribElem+Int64
FSharp.Compiler.AbstractIL.IL+ILAttribElem: FSharp.Compiler.AbstractIL.IL+ILAttribElem+SByte
FSharp.Compiler.AbstractIL.IL+ILAttribElem: FSharp.Compiler.AbstractIL.IL+ILAttribElem+Single
FSharp.Compiler.AbstractIL.IL+ILAttribElem: FSharp.Compiler.AbstractIL.IL+ILAttribElem+String
FSharp.Compiler.AbstractIL.IL+ILAttribElem: FSharp.Compiler.AbstractIL.IL+ILAttribElem+Tags
FSharp.Compiler.AbstractIL.IL+ILAttribElem: FSharp.Compiler.AbstractIL.IL+ILAttribElem+Type
FSharp.Compiler.AbstractIL.IL+ILAttribElem: FSharp.Compiler.AbstractIL.IL+ILAttribElem+TypeRef
FSharp.Compiler.AbstractIL.IL+ILAttribElem: FSharp.Compiler.AbstractIL.IL+ILAttribElem+UInt16
FSharp.Compiler.AbstractIL.IL+ILAttribElem: FSharp.Compiler.AbstractIL.IL+ILAttribElem+UInt32
FSharp.Compiler.AbstractIL.IL+ILAttribElem: FSharp.Compiler.AbstractIL.IL+ILAttribElem+UInt64
FSharp.Compiler.AbstractIL.IL+ILAttribElem: ILAttribElem NewArray(ILType, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILAttribElem])
FSharp.Compiler.AbstractIL.IL+ILAttribElem: ILAttribElem NewBool(Boolean)
FSharp.Compiler.AbstractIL.IL+ILAttribElem: ILAttribElem NewByte(Byte)
FSharp.Compiler.AbstractIL.IL+ILAttribElem: ILAttribElem NewChar(Char)
FSharp.Compiler.AbstractIL.IL+ILAttribElem: ILAttribElem NewDouble(Double)
FSharp.Compiler.AbstractIL.IL+ILAttribElem: ILAttribElem NewInt16(Int16)
FSharp.Compiler.AbstractIL.IL+ILAttribElem: ILAttribElem NewInt32(Int32)
FSharp.Compiler.AbstractIL.IL+ILAttribElem: ILAttribElem NewInt64(Int64)
FSharp.Compiler.AbstractIL.IL+ILAttribElem: ILAttribElem NewSByte(SByte)
FSharp.Compiler.AbstractIL.IL+ILAttribElem: ILAttribElem NewSingle(Single)
FSharp.Compiler.AbstractIL.IL+ILAttribElem: ILAttribElem NewString(Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.AbstractIL.IL+ILAttribElem: ILAttribElem NewType(Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILType])
FSharp.Compiler.AbstractIL.IL+ILAttribElem: ILAttribElem NewTypeRef(Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILTypeRef])
FSharp.Compiler.AbstractIL.IL+ILAttribElem: ILAttribElem NewUInt16(UInt16)
FSharp.Compiler.AbstractIL.IL+ILAttribElem: ILAttribElem NewUInt32(UInt32)
FSharp.Compiler.AbstractIL.IL+ILAttribElem: ILAttribElem NewUInt64(UInt64)
FSharp.Compiler.AbstractIL.IL+ILAttribElem: ILAttribElem Null
FSharp.Compiler.AbstractIL.IL+ILAttribElem: ILAttribElem get_Null()
FSharp.Compiler.AbstractIL.IL+ILAttribElem: Int32 CompareTo(ILAttribElem)
FSharp.Compiler.AbstractIL.IL+ILAttribElem: Int32 CompareTo(System.Object)
FSharp.Compiler.AbstractIL.IL+ILAttribElem: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.AbstractIL.IL+ILAttribElem: Int32 GetHashCode()
FSharp.Compiler.AbstractIL.IL+ILAttribElem: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILAttribElem: Int32 Tag
FSharp.Compiler.AbstractIL.IL+ILAttribElem: Int32 get_Tag()
FSharp.Compiler.AbstractIL.IL+ILAttribElem: System.String ToString()
FSharp.Compiler.AbstractIL.IL+ILAttribute+Decoded: ILMethodSpec get_method()
FSharp.Compiler.AbstractIL.IL+ILAttribute+Decoded: ILMethodSpec method
FSharp.Compiler.AbstractIL.IL+ILAttribute+Decoded: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILAttribElem] fixedArgs
FSharp.Compiler.AbstractIL.IL+ILAttribute+Decoded: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILAttribElem] get_fixedArgs()
FSharp.Compiler.AbstractIL.IL+ILAttribute+Decoded: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`4[System.String,FSharp.Compiler.AbstractIL.IL+ILType,System.Boolean,FSharp.Compiler.AbstractIL.IL+ILAttribElem]] get_namedArgs()
FSharp.Compiler.AbstractIL.IL+ILAttribute+Decoded: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`4[System.String,FSharp.Compiler.AbstractIL.IL+ILType,System.Boolean,FSharp.Compiler.AbstractIL.IL+ILAttribElem]] namedArgs
FSharp.Compiler.AbstractIL.IL+ILAttribute+Encoded: Byte[] data
FSharp.Compiler.AbstractIL.IL+ILAttribute+Encoded: Byte[] get_data()
FSharp.Compiler.AbstractIL.IL+ILAttribute+Encoded: ILMethodSpec get_method()
FSharp.Compiler.AbstractIL.IL+ILAttribute+Encoded: ILMethodSpec method
FSharp.Compiler.AbstractIL.IL+ILAttribute+Encoded: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILAttribElem] elements
FSharp.Compiler.AbstractIL.IL+ILAttribute+Encoded: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILAttribElem] get_elements()
FSharp.Compiler.AbstractIL.IL+ILAttribute+Tags: Int32 Decoded
FSharp.Compiler.AbstractIL.IL+ILAttribute+Tags: Int32 Encoded
FSharp.Compiler.AbstractIL.IL+ILAttribute: Boolean Equals(ILAttribute)
FSharp.Compiler.AbstractIL.IL+ILAttribute: Boolean Equals(System.Object)
FSharp.Compiler.AbstractIL.IL+ILAttribute: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILAttribute: Boolean IsDecoded
FSharp.Compiler.AbstractIL.IL+ILAttribute: Boolean IsEncoded
FSharp.Compiler.AbstractIL.IL+ILAttribute: Boolean get_IsDecoded()
FSharp.Compiler.AbstractIL.IL+ILAttribute: Boolean get_IsEncoded()
FSharp.Compiler.AbstractIL.IL+ILAttribute: FSharp.Compiler.AbstractIL.IL+ILAttribute+Decoded
FSharp.Compiler.AbstractIL.IL+ILAttribute: FSharp.Compiler.AbstractIL.IL+ILAttribute+Encoded
FSharp.Compiler.AbstractIL.IL+ILAttribute: FSharp.Compiler.AbstractIL.IL+ILAttribute+Tags
FSharp.Compiler.AbstractIL.IL+ILAttribute: ILAttribute NewDecoded(ILMethodSpec, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILAttribElem], Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`4[System.String,FSharp.Compiler.AbstractIL.IL+ILType,System.Boolean,FSharp.Compiler.AbstractIL.IL+ILAttribElem]])
FSharp.Compiler.AbstractIL.IL+ILAttribute: ILAttribute NewEncoded(ILMethodSpec, Byte[], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILAttribElem])
FSharp.Compiler.AbstractIL.IL+ILAttribute: Int32 CompareTo(ILAttribute)
FSharp.Compiler.AbstractIL.IL+ILAttribute: Int32 CompareTo(System.Object)
FSharp.Compiler.AbstractIL.IL+ILAttribute: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.AbstractIL.IL+ILAttribute: Int32 GetHashCode()
FSharp.Compiler.AbstractIL.IL+ILAttribute: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILAttribute: Int32 Tag
FSharp.Compiler.AbstractIL.IL+ILAttribute: Int32 get_Tag()
FSharp.Compiler.AbstractIL.IL+ILAttribute: System.String ToString()
FSharp.Compiler.AbstractIL.IL+ILAttributes: ILAttribute[] AsArray
FSharp.Compiler.AbstractIL.IL+ILAttributes: ILAttribute[] get_AsArray()
FSharp.Compiler.AbstractIL.IL+ILAttributes: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILAttribute] AsList
FSharp.Compiler.AbstractIL.IL+ILAttributes: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILAttribute] get_AsList()
FSharp.Compiler.AbstractIL.IL+ILAttributesStored: System.String ToString()
FSharp.Compiler.AbstractIL.IL+ILCallingConv: Boolean Equals(ILCallingConv)
FSharp.Compiler.AbstractIL.IL+ILCallingConv: Boolean Equals(System.Object)
FSharp.Compiler.AbstractIL.IL+ILCallingConv: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILCallingConv: ILCallingConv Instance
FSharp.Compiler.AbstractIL.IL+ILCallingConv: ILCallingConv Static
FSharp.Compiler.AbstractIL.IL+ILCallingConv: ILCallingConv get_Instance()
FSharp.Compiler.AbstractIL.IL+ILCallingConv: ILCallingConv get_Static()
FSharp.Compiler.AbstractIL.IL+ILCallingConv: Int32 CompareTo(ILCallingConv)
FSharp.Compiler.AbstractIL.IL+ILCallingConv: Int32 CompareTo(System.Object)
FSharp.Compiler.AbstractIL.IL+ILCallingConv: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.AbstractIL.IL+ILCallingConv: Int32 GetHashCode()
FSharp.Compiler.AbstractIL.IL+ILCallingConv: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILCallingConv: System.String ToString()
FSharp.Compiler.AbstractIL.IL+ILCallingSignature: Boolean Equals(ILCallingSignature)
FSharp.Compiler.AbstractIL.IL+ILCallingSignature: Boolean Equals(System.Object)
FSharp.Compiler.AbstractIL.IL+ILCallingSignature: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILCallingSignature: ILCallingConv CallingConv
FSharp.Compiler.AbstractIL.IL+ILCallingSignature: ILCallingConv get_CallingConv()
FSharp.Compiler.AbstractIL.IL+ILCallingSignature: ILType ReturnType
FSharp.Compiler.AbstractIL.IL+ILCallingSignature: ILType get_ReturnType()
FSharp.Compiler.AbstractIL.IL+ILCallingSignature: Int32 CompareTo(ILCallingSignature)
FSharp.Compiler.AbstractIL.IL+ILCallingSignature: Int32 CompareTo(System.Object)
FSharp.Compiler.AbstractIL.IL+ILCallingSignature: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.AbstractIL.IL+ILCallingSignature: Int32 GetHashCode()
FSharp.Compiler.AbstractIL.IL+ILCallingSignature: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILCallingSignature: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILType] ArgTypes
FSharp.Compiler.AbstractIL.IL+ILCallingSignature: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILType] get_ArgTypes()
FSharp.Compiler.AbstractIL.IL+ILCallingSignature: System.String ToString()
FSharp.Compiler.AbstractIL.IL+ILCallingSignature: Void .ctor(ILCallingConv, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILType], ILType)
FSharp.Compiler.AbstractIL.IL+ILDefaultPInvokeEncoding+Tags: Int32 Ansi
FSharp.Compiler.AbstractIL.IL+ILDefaultPInvokeEncoding+Tags: Int32 Auto
FSharp.Compiler.AbstractIL.IL+ILDefaultPInvokeEncoding+Tags: Int32 Unicode
FSharp.Compiler.AbstractIL.IL+ILDefaultPInvokeEncoding: Boolean Equals(ILDefaultPInvokeEncoding)
FSharp.Compiler.AbstractIL.IL+ILDefaultPInvokeEncoding: Boolean Equals(System.Object)
FSharp.Compiler.AbstractIL.IL+ILDefaultPInvokeEncoding: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILDefaultPInvokeEncoding: Boolean IsAnsi
FSharp.Compiler.AbstractIL.IL+ILDefaultPInvokeEncoding: Boolean IsAuto
FSharp.Compiler.AbstractIL.IL+ILDefaultPInvokeEncoding: Boolean IsUnicode
FSharp.Compiler.AbstractIL.IL+ILDefaultPInvokeEncoding: Boolean get_IsAnsi()
FSharp.Compiler.AbstractIL.IL+ILDefaultPInvokeEncoding: Boolean get_IsAuto()
FSharp.Compiler.AbstractIL.IL+ILDefaultPInvokeEncoding: Boolean get_IsUnicode()
FSharp.Compiler.AbstractIL.IL+ILDefaultPInvokeEncoding: FSharp.Compiler.AbstractIL.IL+ILDefaultPInvokeEncoding+Tags
FSharp.Compiler.AbstractIL.IL+ILDefaultPInvokeEncoding: ILDefaultPInvokeEncoding Ansi
FSharp.Compiler.AbstractIL.IL+ILDefaultPInvokeEncoding: ILDefaultPInvokeEncoding Auto
FSharp.Compiler.AbstractIL.IL+ILDefaultPInvokeEncoding: ILDefaultPInvokeEncoding Unicode
FSharp.Compiler.AbstractIL.IL+ILDefaultPInvokeEncoding: ILDefaultPInvokeEncoding get_Ansi()
FSharp.Compiler.AbstractIL.IL+ILDefaultPInvokeEncoding: ILDefaultPInvokeEncoding get_Auto()
FSharp.Compiler.AbstractIL.IL+ILDefaultPInvokeEncoding: ILDefaultPInvokeEncoding get_Unicode()
FSharp.Compiler.AbstractIL.IL+ILDefaultPInvokeEncoding: Int32 CompareTo(ILDefaultPInvokeEncoding)
FSharp.Compiler.AbstractIL.IL+ILDefaultPInvokeEncoding: Int32 CompareTo(System.Object)
FSharp.Compiler.AbstractIL.IL+ILDefaultPInvokeEncoding: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.AbstractIL.IL+ILDefaultPInvokeEncoding: Int32 GetHashCode()
FSharp.Compiler.AbstractIL.IL+ILDefaultPInvokeEncoding: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILDefaultPInvokeEncoding: Int32 Tag
FSharp.Compiler.AbstractIL.IL+ILDefaultPInvokeEncoding: Int32 get_Tag()
FSharp.Compiler.AbstractIL.IL+ILDefaultPInvokeEncoding: System.String ToString()
FSharp.Compiler.AbstractIL.IL+ILEventDef: Boolean IsRTSpecialName
FSharp.Compiler.AbstractIL.IL+ILEventDef: Boolean IsSpecialName
FSharp.Compiler.AbstractIL.IL+ILEventDef: Boolean get_IsRTSpecialName()
FSharp.Compiler.AbstractIL.IL+ILEventDef: Boolean get_IsSpecialName()
FSharp.Compiler.AbstractIL.IL+ILEventDef: ILAttributes CustomAttrs
FSharp.Compiler.AbstractIL.IL+ILEventDef: ILAttributes get_CustomAttrs()
FSharp.Compiler.AbstractIL.IL+ILEventDef: ILMethodRef AddMethod
FSharp.Compiler.AbstractIL.IL+ILEventDef: ILMethodRef RemoveMethod
FSharp.Compiler.AbstractIL.IL+ILEventDef: ILMethodRef get_AddMethod()
FSharp.Compiler.AbstractIL.IL+ILEventDef: ILMethodRef get_RemoveMethod()
FSharp.Compiler.AbstractIL.IL+ILEventDef: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILMethodRef] OtherMethods
FSharp.Compiler.AbstractIL.IL+ILEventDef: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILMethodRef] get_OtherMethods()
FSharp.Compiler.AbstractIL.IL+ILEventDef: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILMethodRef] FireMethod
FSharp.Compiler.AbstractIL.IL+ILEventDef: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILMethodRef] get_FireMethod()
FSharp.Compiler.AbstractIL.IL+ILEventDef: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILType] EventType
FSharp.Compiler.AbstractIL.IL+ILEventDef: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILType] get_EventType()
FSharp.Compiler.AbstractIL.IL+ILEventDef: System.Reflection.EventAttributes Attributes
FSharp.Compiler.AbstractIL.IL+ILEventDef: System.Reflection.EventAttributes get_Attributes()
FSharp.Compiler.AbstractIL.IL+ILEventDef: System.String Name
FSharp.Compiler.AbstractIL.IL+ILEventDef: System.String ToString()
FSharp.Compiler.AbstractIL.IL+ILEventDef: System.String get_Name()
FSharp.Compiler.AbstractIL.IL+ILEventDef: Void .ctor(Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILType], System.String, System.Reflection.EventAttributes, ILMethodRef, ILMethodRef, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILMethodRef], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILMethodRef], ILAttributes)
FSharp.Compiler.AbstractIL.IL+ILEventDefs: System.String ToString()
FSharp.Compiler.AbstractIL.IL+ILExportedTypeOrForwarder: Boolean IsForwarder
FSharp.Compiler.AbstractIL.IL+ILExportedTypeOrForwarder: Boolean get_IsForwarder()
FSharp.Compiler.AbstractIL.IL+ILExportedTypeOrForwarder: ILAttributes CustomAttrs
FSharp.Compiler.AbstractIL.IL+ILExportedTypeOrForwarder: ILAttributes get_CustomAttrs()
FSharp.Compiler.AbstractIL.IL+ILExportedTypeOrForwarder: ILAttributesStored CustomAttrsStored
FSharp.Compiler.AbstractIL.IL+ILExportedTypeOrForwarder: ILAttributesStored get_CustomAttrsStored()
FSharp.Compiler.AbstractIL.IL+ILExportedTypeOrForwarder: ILNestedExportedTypes Nested
FSharp.Compiler.AbstractIL.IL+ILExportedTypeOrForwarder: ILNestedExportedTypes get_Nested()
FSharp.Compiler.AbstractIL.IL+ILExportedTypeOrForwarder: ILScopeRef ScopeRef
FSharp.Compiler.AbstractIL.IL+ILExportedTypeOrForwarder: ILScopeRef get_ScopeRef()
FSharp.Compiler.AbstractIL.IL+ILExportedTypeOrForwarder: ILTypeDefAccess Access
FSharp.Compiler.AbstractIL.IL+ILExportedTypeOrForwarder: ILTypeDefAccess get_Access()
FSharp.Compiler.AbstractIL.IL+ILExportedTypeOrForwarder: Int32 MetadataIndex
FSharp.Compiler.AbstractIL.IL+ILExportedTypeOrForwarder: Int32 get_MetadataIndex()
FSharp.Compiler.AbstractIL.IL+ILExportedTypeOrForwarder: System.Reflection.TypeAttributes Attributes
FSharp.Compiler.AbstractIL.IL+ILExportedTypeOrForwarder: System.Reflection.TypeAttributes get_Attributes()
FSharp.Compiler.AbstractIL.IL+ILExportedTypeOrForwarder: System.String Name
FSharp.Compiler.AbstractIL.IL+ILExportedTypeOrForwarder: System.String ToString()
FSharp.Compiler.AbstractIL.IL+ILExportedTypeOrForwarder: System.String get_Name()
FSharp.Compiler.AbstractIL.IL+ILExportedTypeOrForwarder: Void .ctor(ILScopeRef, System.String, System.Reflection.TypeAttributes, ILNestedExportedTypes, ILAttributesStored, Int32)
FSharp.Compiler.AbstractIL.IL+ILExportedTypesAndForwarders: Boolean Equals(ILExportedTypesAndForwarders)
FSharp.Compiler.AbstractIL.IL+ILExportedTypesAndForwarders: Boolean Equals(System.Object)
FSharp.Compiler.AbstractIL.IL+ILExportedTypesAndForwarders: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILExportedTypesAndForwarders: Int32 GetHashCode()
FSharp.Compiler.AbstractIL.IL+ILExportedTypesAndForwarders: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILExportedTypesAndForwarders: System.String ToString()
FSharp.Compiler.AbstractIL.IL+ILFieldDef: Boolean IsInitOnly
FSharp.Compiler.AbstractIL.IL+ILFieldDef: Boolean IsLiteral
FSharp.Compiler.AbstractIL.IL+ILFieldDef: Boolean IsSpecialName
FSharp.Compiler.AbstractIL.IL+ILFieldDef: Boolean IsStatic
FSharp.Compiler.AbstractIL.IL+ILFieldDef: Boolean NotSerialized
FSharp.Compiler.AbstractIL.IL+ILFieldDef: Boolean get_IsInitOnly()
FSharp.Compiler.AbstractIL.IL+ILFieldDef: Boolean get_IsLiteral()
FSharp.Compiler.AbstractIL.IL+ILFieldDef: Boolean get_IsSpecialName()
FSharp.Compiler.AbstractIL.IL+ILFieldDef: Boolean get_IsStatic()
FSharp.Compiler.AbstractIL.IL+ILFieldDef: Boolean get_NotSerialized()
FSharp.Compiler.AbstractIL.IL+ILFieldDef: ILAttributes CustomAttrs
FSharp.Compiler.AbstractIL.IL+ILFieldDef: ILAttributes get_CustomAttrs()
FSharp.Compiler.AbstractIL.IL+ILFieldDef: ILMemberAccess Access
FSharp.Compiler.AbstractIL.IL+ILFieldDef: ILMemberAccess get_Access()
FSharp.Compiler.AbstractIL.IL+ILFieldDef: ILType FieldType
FSharp.Compiler.AbstractIL.IL+ILFieldDef: ILType get_FieldType()
FSharp.Compiler.AbstractIL.IL+ILFieldDef: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILFieldInit] LiteralValue
FSharp.Compiler.AbstractIL.IL+ILFieldDef: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILFieldInit] get_LiteralValue()
FSharp.Compiler.AbstractIL.IL+ILFieldDef: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILNativeType] Marshal
FSharp.Compiler.AbstractIL.IL+ILFieldDef: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILNativeType] get_Marshal()
FSharp.Compiler.AbstractIL.IL+ILFieldDef: Microsoft.FSharp.Core.FSharpOption`1[System.Byte[]] Data
FSharp.Compiler.AbstractIL.IL+ILFieldDef: Microsoft.FSharp.Core.FSharpOption`1[System.Byte[]] get_Data()
FSharp.Compiler.AbstractIL.IL+ILFieldDef: Microsoft.FSharp.Core.FSharpOption`1[System.Int32] Offset
FSharp.Compiler.AbstractIL.IL+ILFieldDef: Microsoft.FSharp.Core.FSharpOption`1[System.Int32] get_Offset()
FSharp.Compiler.AbstractIL.IL+ILFieldDef: System.Reflection.FieldAttributes Attributes
FSharp.Compiler.AbstractIL.IL+ILFieldDef: System.Reflection.FieldAttributes get_Attributes()
FSharp.Compiler.AbstractIL.IL+ILFieldDef: System.String Name
FSharp.Compiler.AbstractIL.IL+ILFieldDef: System.String get_Name()
FSharp.Compiler.AbstractIL.IL+ILFieldDef: Void .ctor(System.String, ILType, System.Reflection.FieldAttributes, Microsoft.FSharp.Core.FSharpOption`1[System.Byte[]], Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILFieldInit], Microsoft.FSharp.Core.FSharpOption`1[System.Int32], Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILNativeType], ILAttributes)
FSharp.Compiler.AbstractIL.IL+ILFieldDefs: Boolean Equals(ILFieldDefs)
FSharp.Compiler.AbstractIL.IL+ILFieldDefs: Boolean Equals(System.Object)
FSharp.Compiler.AbstractIL.IL+ILFieldDefs: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILFieldDefs: Int32 GetHashCode()
FSharp.Compiler.AbstractIL.IL+ILFieldDefs: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILFieldDefs: System.String ToString()
FSharp.Compiler.AbstractIL.IL+ILFieldRef: Boolean Equals(ILFieldRef)
FSharp.Compiler.AbstractIL.IL+ILFieldRef: Boolean Equals(System.Object)
FSharp.Compiler.AbstractIL.IL+ILFieldRef: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILFieldRef: ILType Type
FSharp.Compiler.AbstractIL.IL+ILFieldRef: ILType get_Type()
FSharp.Compiler.AbstractIL.IL+ILFieldRef: ILTypeRef DeclaringTypeRef
FSharp.Compiler.AbstractIL.IL+ILFieldRef: ILTypeRef get_DeclaringTypeRef()
FSharp.Compiler.AbstractIL.IL+ILFieldRef: Int32 CompareTo(ILFieldRef)
FSharp.Compiler.AbstractIL.IL+ILFieldRef: Int32 CompareTo(System.Object)
FSharp.Compiler.AbstractIL.IL+ILFieldRef: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.AbstractIL.IL+ILFieldRef: Int32 GetHashCode()
FSharp.Compiler.AbstractIL.IL+ILFieldRef: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILFieldRef: System.String Name
FSharp.Compiler.AbstractIL.IL+ILFieldRef: System.String ToString()
FSharp.Compiler.AbstractIL.IL+ILFieldRef: System.String get_Name()
FSharp.Compiler.AbstractIL.IL+ILFieldRef: Void .ctor(ILTypeRef, System.String, ILType)
FSharp.Compiler.AbstractIL.IL+ILFieldSpec: Boolean Equals(ILFieldSpec)
FSharp.Compiler.AbstractIL.IL+ILFieldSpec: Boolean Equals(System.Object)
FSharp.Compiler.AbstractIL.IL+ILFieldSpec: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILFieldSpec: ILFieldRef FieldRef
FSharp.Compiler.AbstractIL.IL+ILFieldSpec: ILFieldRef get_FieldRef()
FSharp.Compiler.AbstractIL.IL+ILFieldSpec: ILType ActualType
FSharp.Compiler.AbstractIL.IL+ILFieldSpec: ILType DeclaringType
FSharp.Compiler.AbstractIL.IL+ILFieldSpec: ILType FormalType
FSharp.Compiler.AbstractIL.IL+ILFieldSpec: ILType get_ActualType()
FSharp.Compiler.AbstractIL.IL+ILFieldSpec: ILType get_DeclaringType()
FSharp.Compiler.AbstractIL.IL+ILFieldSpec: ILType get_FormalType()
FSharp.Compiler.AbstractIL.IL+ILFieldSpec: ILTypeRef DeclaringTypeRef
FSharp.Compiler.AbstractIL.IL+ILFieldSpec: ILTypeRef get_DeclaringTypeRef()
FSharp.Compiler.AbstractIL.IL+ILFieldSpec: Int32 CompareTo(ILFieldSpec)
FSharp.Compiler.AbstractIL.IL+ILFieldSpec: Int32 CompareTo(System.Object)
FSharp.Compiler.AbstractIL.IL+ILFieldSpec: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.AbstractIL.IL+ILFieldSpec: Int32 GetHashCode()
FSharp.Compiler.AbstractIL.IL+ILFieldSpec: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILFieldSpec: System.String Name
FSharp.Compiler.AbstractIL.IL+ILFieldSpec: System.String ToString()
FSharp.Compiler.AbstractIL.IL+ILFieldSpec: System.String get_Name()
FSharp.Compiler.AbstractIL.IL+ILFieldSpec: Void .ctor(ILFieldRef, ILType)
FSharp.Compiler.AbstractIL.IL+ILGenericParameterDef: Boolean HasDefaultConstructorConstraint
FSharp.Compiler.AbstractIL.IL+ILGenericParameterDef: Boolean HasNotNullableValueTypeConstraint
FSharp.Compiler.AbstractIL.IL+ILGenericParameterDef: Boolean HasReferenceTypeConstraint
FSharp.Compiler.AbstractIL.IL+ILGenericParameterDef: Boolean get_HasDefaultConstructorConstraint()
FSharp.Compiler.AbstractIL.IL+ILGenericParameterDef: Boolean get_HasNotNullableValueTypeConstraint()
FSharp.Compiler.AbstractIL.IL+ILGenericParameterDef: Boolean get_HasReferenceTypeConstraint()
FSharp.Compiler.AbstractIL.IL+ILGenericParameterDef: ILAttributes CustomAttrs
FSharp.Compiler.AbstractIL.IL+ILGenericParameterDef: ILAttributes get_CustomAttrs()
FSharp.Compiler.AbstractIL.IL+ILGenericParameterDef: ILAttributesStored CustomAttrsStored
FSharp.Compiler.AbstractIL.IL+ILGenericParameterDef: ILAttributesStored get_CustomAttrsStored()
FSharp.Compiler.AbstractIL.IL+ILGenericParameterDef: ILGenericVariance Variance
FSharp.Compiler.AbstractIL.IL+ILGenericParameterDef: ILGenericVariance get_Variance()
FSharp.Compiler.AbstractIL.IL+ILGenericParameterDef: Int32 MetadataIndex
FSharp.Compiler.AbstractIL.IL+ILGenericParameterDef: Int32 get_MetadataIndex()
FSharp.Compiler.AbstractIL.IL+ILGenericParameterDef: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILType] Constraints
FSharp.Compiler.AbstractIL.IL+ILGenericParameterDef: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILType] get_Constraints()
FSharp.Compiler.AbstractIL.IL+ILGenericParameterDef: System.String Name
FSharp.Compiler.AbstractIL.IL+ILGenericParameterDef: System.String ToString()
FSharp.Compiler.AbstractIL.IL+ILGenericParameterDef: System.String get_Name()
FSharp.Compiler.AbstractIL.IL+ILGenericParameterDef: Void .ctor(System.String, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILType], ILGenericVariance, Boolean, Boolean, Boolean, ILAttributesStored, Int32)
FSharp.Compiler.AbstractIL.IL+ILGenericVariance+Tags: Int32 CoVariant
FSharp.Compiler.AbstractIL.IL+ILGenericVariance+Tags: Int32 ContraVariant
FSharp.Compiler.AbstractIL.IL+ILGenericVariance+Tags: Int32 NonVariant
FSharp.Compiler.AbstractIL.IL+ILGenericVariance: Boolean Equals(ILGenericVariance)
FSharp.Compiler.AbstractIL.IL+ILGenericVariance: Boolean Equals(System.Object)
FSharp.Compiler.AbstractIL.IL+ILGenericVariance: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILGenericVariance: Boolean IsCoVariant
FSharp.Compiler.AbstractIL.IL+ILGenericVariance: Boolean IsContraVariant
FSharp.Compiler.AbstractIL.IL+ILGenericVariance: Boolean IsNonVariant
FSharp.Compiler.AbstractIL.IL+ILGenericVariance: Boolean get_IsCoVariant()
FSharp.Compiler.AbstractIL.IL+ILGenericVariance: Boolean get_IsContraVariant()
FSharp.Compiler.AbstractIL.IL+ILGenericVariance: Boolean get_IsNonVariant()
FSharp.Compiler.AbstractIL.IL+ILGenericVariance: FSharp.Compiler.AbstractIL.IL+ILGenericVariance+Tags
FSharp.Compiler.AbstractIL.IL+ILGenericVariance: ILGenericVariance CoVariant
FSharp.Compiler.AbstractIL.IL+ILGenericVariance: ILGenericVariance ContraVariant
FSharp.Compiler.AbstractIL.IL+ILGenericVariance: ILGenericVariance NonVariant
FSharp.Compiler.AbstractIL.IL+ILGenericVariance: ILGenericVariance get_CoVariant()
FSharp.Compiler.AbstractIL.IL+ILGenericVariance: ILGenericVariance get_ContraVariant()
FSharp.Compiler.AbstractIL.IL+ILGenericVariance: ILGenericVariance get_NonVariant()
FSharp.Compiler.AbstractIL.IL+ILGenericVariance: Int32 CompareTo(ILGenericVariance)
FSharp.Compiler.AbstractIL.IL+ILGenericVariance: Int32 CompareTo(System.Object)
FSharp.Compiler.AbstractIL.IL+ILGenericVariance: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.AbstractIL.IL+ILGenericVariance: Int32 GetHashCode()
FSharp.Compiler.AbstractIL.IL+ILGenericVariance: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILGenericVariance: Int32 Tag
FSharp.Compiler.AbstractIL.IL+ILGenericVariance: Int32 get_Tag()
FSharp.Compiler.AbstractIL.IL+ILGenericVariance: System.String ToString()
FSharp.Compiler.AbstractIL.IL+ILMemberAccess+Tags: Int32 Assembly
FSharp.Compiler.AbstractIL.IL+ILMemberAccess+Tags: Int32 CompilerControlled
FSharp.Compiler.AbstractIL.IL+ILMemberAccess+Tags: Int32 Family
FSharp.Compiler.AbstractIL.IL+ILMemberAccess+Tags: Int32 FamilyAndAssembly
FSharp.Compiler.AbstractIL.IL+ILMemberAccess+Tags: Int32 FamilyOrAssembly
FSharp.Compiler.AbstractIL.IL+ILMemberAccess+Tags: Int32 Private
FSharp.Compiler.AbstractIL.IL+ILMemberAccess+Tags: Int32 Public
FSharp.Compiler.AbstractIL.IL+ILMemberAccess: Boolean Equals(ILMemberAccess)
FSharp.Compiler.AbstractIL.IL+ILMemberAccess: Boolean Equals(System.Object)
FSharp.Compiler.AbstractIL.IL+ILMemberAccess: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILMemberAccess: Boolean IsAssembly
FSharp.Compiler.AbstractIL.IL+ILMemberAccess: Boolean IsCompilerControlled
FSharp.Compiler.AbstractIL.IL+ILMemberAccess: Boolean IsFamily
FSharp.Compiler.AbstractIL.IL+ILMemberAccess: Boolean IsFamilyAndAssembly
FSharp.Compiler.AbstractIL.IL+ILMemberAccess: Boolean IsFamilyOrAssembly
FSharp.Compiler.AbstractIL.IL+ILMemberAccess: Boolean IsPrivate
FSharp.Compiler.AbstractIL.IL+ILMemberAccess: Boolean IsPublic
FSharp.Compiler.AbstractIL.IL+ILMemberAccess: Boolean get_IsAssembly()
FSharp.Compiler.AbstractIL.IL+ILMemberAccess: Boolean get_IsCompilerControlled()
FSharp.Compiler.AbstractIL.IL+ILMemberAccess: Boolean get_IsFamily()
FSharp.Compiler.AbstractIL.IL+ILMemberAccess: Boolean get_IsFamilyAndAssembly()
FSharp.Compiler.AbstractIL.IL+ILMemberAccess: Boolean get_IsFamilyOrAssembly()
FSharp.Compiler.AbstractIL.IL+ILMemberAccess: Boolean get_IsPrivate()
FSharp.Compiler.AbstractIL.IL+ILMemberAccess: Boolean get_IsPublic()
FSharp.Compiler.AbstractIL.IL+ILMemberAccess: FSharp.Compiler.AbstractIL.IL+ILMemberAccess+Tags
FSharp.Compiler.AbstractIL.IL+ILMemberAccess: ILMemberAccess Assembly
FSharp.Compiler.AbstractIL.IL+ILMemberAccess: ILMemberAccess CompilerControlled
FSharp.Compiler.AbstractIL.IL+ILMemberAccess: ILMemberAccess Family
FSharp.Compiler.AbstractIL.IL+ILMemberAccess: ILMemberAccess FamilyAndAssembly
FSharp.Compiler.AbstractIL.IL+ILMemberAccess: ILMemberAccess FamilyOrAssembly
FSharp.Compiler.AbstractIL.IL+ILMemberAccess: ILMemberAccess Private
FSharp.Compiler.AbstractIL.IL+ILMemberAccess: ILMemberAccess Public
FSharp.Compiler.AbstractIL.IL+ILMemberAccess: ILMemberAccess get_Assembly()
FSharp.Compiler.AbstractIL.IL+ILMemberAccess: ILMemberAccess get_CompilerControlled()
FSharp.Compiler.AbstractIL.IL+ILMemberAccess: ILMemberAccess get_Family()
FSharp.Compiler.AbstractIL.IL+ILMemberAccess: ILMemberAccess get_FamilyAndAssembly()
FSharp.Compiler.AbstractIL.IL+ILMemberAccess: ILMemberAccess get_FamilyOrAssembly()
FSharp.Compiler.AbstractIL.IL+ILMemberAccess: ILMemberAccess get_Private()
FSharp.Compiler.AbstractIL.IL+ILMemberAccess: ILMemberAccess get_Public()
FSharp.Compiler.AbstractIL.IL+ILMemberAccess: Int32 CompareTo(ILMemberAccess)
FSharp.Compiler.AbstractIL.IL+ILMemberAccess: Int32 CompareTo(System.Object)
FSharp.Compiler.AbstractIL.IL+ILMemberAccess: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.AbstractIL.IL+ILMemberAccess: Int32 GetHashCode()
FSharp.Compiler.AbstractIL.IL+ILMemberAccess: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILMemberAccess: Int32 Tag
FSharp.Compiler.AbstractIL.IL+ILMemberAccess: Int32 get_Tag()
FSharp.Compiler.AbstractIL.IL+ILMemberAccess: System.String ToString()
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean HasSecurity
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean IsAbstract
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean IsAggressiveInline
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean IsCheckAccessOnOverride
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean IsClassInitializer
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean IsConstructor
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean IsEntryPoint
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean IsFinal
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean IsForwardRef
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean IsHideBySig
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean IsIL
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean IsInternalCall
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean IsManaged
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean IsMustRun
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean IsNewSlot
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean IsNoInline
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean IsNonVirtualInstance
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean IsPreserveSig
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean IsReqSecObj
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean IsSpecialName
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean IsStatic
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean IsSynchronized
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean IsUnmanagedExport
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean IsVirtual
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean IsZeroInit
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean get_HasSecurity()
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean get_IsAbstract()
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean get_IsAggressiveInline()
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean get_IsCheckAccessOnOverride()
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean get_IsClassInitializer()
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean get_IsConstructor()
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean get_IsEntryPoint()
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean get_IsFinal()
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean get_IsForwardRef()
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean get_IsHideBySig()
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean get_IsIL()
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean get_IsInternalCall()
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean get_IsManaged()
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean get_IsMustRun()
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean get_IsNewSlot()
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean get_IsNoInline()
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean get_IsNonVirtualInstance()
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean get_IsPreserveSig()
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean get_IsReqSecObj()
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean get_IsSpecialName()
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean get_IsStatic()
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean get_IsSynchronized()
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean get_IsUnmanagedExport()
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean get_IsVirtual()
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Boolean get_IsZeroInit()
FSharp.Compiler.AbstractIL.IL+ILMethodDef: ILAttributes CustomAttrs
FSharp.Compiler.AbstractIL.IL+ILMethodDef: ILAttributes get_CustomAttrs()
FSharp.Compiler.AbstractIL.IL+ILMethodDef: ILCallingConv CallingConv
FSharp.Compiler.AbstractIL.IL+ILMethodDef: ILCallingConv get_CallingConv()
FSharp.Compiler.AbstractIL.IL+ILMethodDef: ILCallingSignature CallingSignature
FSharp.Compiler.AbstractIL.IL+ILMethodDef: ILCallingSignature get_CallingSignature()
FSharp.Compiler.AbstractIL.IL+ILMethodDef: ILMemberAccess Access
FSharp.Compiler.AbstractIL.IL+ILMethodDef: ILMemberAccess get_Access()
FSharp.Compiler.AbstractIL.IL+ILMethodDef: ILMethodBody MethodBody
FSharp.Compiler.AbstractIL.IL+ILMethodDef: ILMethodBody get_MethodBody()
FSharp.Compiler.AbstractIL.IL+ILMethodDef: ILReturn Return
FSharp.Compiler.AbstractIL.IL+ILMethodDef: ILReturn get_Return()
FSharp.Compiler.AbstractIL.IL+ILMethodDef: ILSecurityDecls SecurityDecls
FSharp.Compiler.AbstractIL.IL+ILMethodDef: ILSecurityDecls get_SecurityDecls()
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Int32 MaxStack
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Int32 get_MaxStack()
FSharp.Compiler.AbstractIL.IL+ILMethodDef: MethodBody Body
FSharp.Compiler.AbstractIL.IL+ILMethodDef: MethodBody get_Body()
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILGenericParameterDef] GenericParams
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILGenericParameterDef] get_GenericParams()
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILLocal] Locals
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILLocal] get_Locals()
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILParameter] Parameters
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILParameter] get_Parameters()
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILType] ParameterTypes
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILType] get_ParameterTypes()
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILCode] Code
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILCode] get_Code()
FSharp.Compiler.AbstractIL.IL+ILMethodDef: System.Reflection.MethodAttributes Attributes
FSharp.Compiler.AbstractIL.IL+ILMethodDef: System.Reflection.MethodAttributes get_Attributes()
FSharp.Compiler.AbstractIL.IL+ILMethodDef: System.Reflection.MethodImplAttributes ImplAttributes
FSharp.Compiler.AbstractIL.IL+ILMethodDef: System.Reflection.MethodImplAttributes get_ImplAttributes()
FSharp.Compiler.AbstractIL.IL+ILMethodDef: System.String Name
FSharp.Compiler.AbstractIL.IL+ILMethodDef: System.String get_Name()
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Void .ctor(System.String, System.Reflection.MethodAttributes, System.Reflection.MethodImplAttributes, ILCallingConv, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILParameter], ILReturn, System.Lazy`1[FSharp.Compiler.AbstractIL.IL+MethodBody], Boolean, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILGenericParameterDef], ILSecurityDecls, ILAttributes)
FSharp.Compiler.AbstractIL.IL+ILMethodDefs: ILMethodDef[] AsArray
FSharp.Compiler.AbstractIL.IL+ILMethodDefs: ILMethodDef[] get_AsArray()
FSharp.Compiler.AbstractIL.IL+ILMethodDefs: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILMethodDef] AsList
FSharp.Compiler.AbstractIL.IL+ILMethodDefs: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILMethodDef] FindByName(System.String)
FSharp.Compiler.AbstractIL.IL+ILMethodDefs: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILMethodDef] get_AsList()
FSharp.Compiler.AbstractIL.IL+ILMethodDefs: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILMethodDef] TryFindInstanceByNameAndCallingSignature(System.String, ILCallingSignature)
FSharp.Compiler.AbstractIL.IL+ILMethodImplDef: Boolean Equals(ILMethodImplDef)
FSharp.Compiler.AbstractIL.IL+ILMethodImplDef: Boolean Equals(System.Object)
FSharp.Compiler.AbstractIL.IL+ILMethodImplDef: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILMethodImplDef: ILMethodSpec OverrideBy
FSharp.Compiler.AbstractIL.IL+ILMethodImplDef: ILMethodSpec get_OverrideBy()
FSharp.Compiler.AbstractIL.IL+ILMethodImplDef: ILOverridesSpec Overrides
FSharp.Compiler.AbstractIL.IL+ILMethodImplDef: ILOverridesSpec get_Overrides()
FSharp.Compiler.AbstractIL.IL+ILMethodImplDef: Int32 CompareTo(ILMethodImplDef)
FSharp.Compiler.AbstractIL.IL+ILMethodImplDef: Int32 CompareTo(System.Object)
FSharp.Compiler.AbstractIL.IL+ILMethodImplDef: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.AbstractIL.IL+ILMethodImplDef: Int32 GetHashCode()
FSharp.Compiler.AbstractIL.IL+ILMethodImplDef: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILMethodImplDef: System.String ToString()
FSharp.Compiler.AbstractIL.IL+ILMethodImplDef: Void .ctor(ILOverridesSpec, ILMethodSpec)
FSharp.Compiler.AbstractIL.IL+ILMethodImplDefs: Boolean Equals(ILMethodImplDefs)
FSharp.Compiler.AbstractIL.IL+ILMethodImplDefs: Boolean Equals(System.Object)
FSharp.Compiler.AbstractIL.IL+ILMethodImplDefs: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILMethodImplDefs: Int32 GetHashCode()
FSharp.Compiler.AbstractIL.IL+ILMethodImplDefs: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILMethodImplDefs: System.String ToString()
FSharp.Compiler.AbstractIL.IL+ILMethodRef: Boolean Equals(ILMethodRef)
FSharp.Compiler.AbstractIL.IL+ILMethodRef: Boolean Equals(System.Object)
FSharp.Compiler.AbstractIL.IL+ILMethodRef: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILMethodRef: ILCallingConv CallingConv
FSharp.Compiler.AbstractIL.IL+ILMethodRef: ILCallingConv get_CallingConv()
FSharp.Compiler.AbstractIL.IL+ILMethodRef: ILCallingSignature CallingSignature
FSharp.Compiler.AbstractIL.IL+ILMethodRef: ILCallingSignature get_CallingSignature()
FSharp.Compiler.AbstractIL.IL+ILMethodRef: ILMethodRef Create(ILTypeRef, ILCallingConv, System.String, Int32, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILType], ILType)
FSharp.Compiler.AbstractIL.IL+ILMethodRef: ILType ReturnType
FSharp.Compiler.AbstractIL.IL+ILMethodRef: ILType get_ReturnType()
FSharp.Compiler.AbstractIL.IL+ILMethodRef: ILTypeRef DeclaringTypeRef
FSharp.Compiler.AbstractIL.IL+ILMethodRef: ILTypeRef get_DeclaringTypeRef()
FSharp.Compiler.AbstractIL.IL+ILMethodRef: Int32 ArgCount
FSharp.Compiler.AbstractIL.IL+ILMethodRef: Int32 CompareTo(ILMethodRef)
FSharp.Compiler.AbstractIL.IL+ILMethodRef: Int32 CompareTo(System.Object)
FSharp.Compiler.AbstractIL.IL+ILMethodRef: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.AbstractIL.IL+ILMethodRef: Int32 GenericArity
FSharp.Compiler.AbstractIL.IL+ILMethodRef: Int32 GetHashCode()
FSharp.Compiler.AbstractIL.IL+ILMethodRef: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILMethodRef: Int32 get_ArgCount()
FSharp.Compiler.AbstractIL.IL+ILMethodRef: Int32 get_GenericArity()
FSharp.Compiler.AbstractIL.IL+ILMethodRef: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILType] ArgTypes
FSharp.Compiler.AbstractIL.IL+ILMethodRef: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILType] get_ArgTypes()
FSharp.Compiler.AbstractIL.IL+ILMethodRef: System.String Name
FSharp.Compiler.AbstractIL.IL+ILMethodRef: System.String ToString()
FSharp.Compiler.AbstractIL.IL+ILMethodRef: System.String get_Name()
FSharp.Compiler.AbstractIL.IL+ILMethodSpec: Boolean Equals(ILMethodSpec)
FSharp.Compiler.AbstractIL.IL+ILMethodSpec: Boolean Equals(System.Object)
FSharp.Compiler.AbstractIL.IL+ILMethodSpec: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILMethodSpec: ILCallingConv CallingConv
FSharp.Compiler.AbstractIL.IL+ILMethodSpec: ILCallingConv get_CallingConv()
FSharp.Compiler.AbstractIL.IL+ILMethodSpec: ILMethodRef MethodRef
FSharp.Compiler.AbstractIL.IL+ILMethodSpec: ILMethodRef get_MethodRef()
FSharp.Compiler.AbstractIL.IL+ILMethodSpec: ILMethodSpec Create(ILType, ILMethodRef, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILType])
FSharp.Compiler.AbstractIL.IL+ILMethodSpec: ILType DeclaringType
FSharp.Compiler.AbstractIL.IL+ILMethodSpec: ILType FormalReturnType
FSharp.Compiler.AbstractIL.IL+ILMethodSpec: ILType get_DeclaringType()
FSharp.Compiler.AbstractIL.IL+ILMethodSpec: ILType get_FormalReturnType()
FSharp.Compiler.AbstractIL.IL+ILMethodSpec: Int32 CompareTo(ILMethodSpec)
FSharp.Compiler.AbstractIL.IL+ILMethodSpec: Int32 CompareTo(System.Object)
FSharp.Compiler.AbstractIL.IL+ILMethodSpec: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.AbstractIL.IL+ILMethodSpec: Int32 GenericArity
FSharp.Compiler.AbstractIL.IL+ILMethodSpec: Int32 GetHashCode()
FSharp.Compiler.AbstractIL.IL+ILMethodSpec: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILMethodSpec: Int32 get_GenericArity()
FSharp.Compiler.AbstractIL.IL+ILMethodSpec: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILType] FormalArgTypes
FSharp.Compiler.AbstractIL.IL+ILMethodSpec: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILType] GenericArgs
FSharp.Compiler.AbstractIL.IL+ILMethodSpec: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILType] get_FormalArgTypes()
FSharp.Compiler.AbstractIL.IL+ILMethodSpec: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILType] get_GenericArgs()
FSharp.Compiler.AbstractIL.IL+ILMethodSpec: System.String Name
FSharp.Compiler.AbstractIL.IL+ILMethodSpec: System.String ToString()
FSharp.Compiler.AbstractIL.IL+ILMethodSpec: System.String get_Name()
FSharp.Compiler.AbstractIL.IL+ILModuleDef: Boolean HasManifest
FSharp.Compiler.AbstractIL.IL+ILModuleDef: Boolean Is32Bit
FSharp.Compiler.AbstractIL.IL+ILModuleDef: Boolean Is32BitPreferred
FSharp.Compiler.AbstractIL.IL+ILModuleDef: Boolean Is64Bit
FSharp.Compiler.AbstractIL.IL+ILModuleDef: Boolean IsDLL
FSharp.Compiler.AbstractIL.IL+ILModuleDef: Boolean IsILOnly
FSharp.Compiler.AbstractIL.IL+ILModuleDef: Boolean UseHighEntropyVA
FSharp.Compiler.AbstractIL.IL+ILModuleDef: Boolean get_HasManifest()
FSharp.Compiler.AbstractIL.IL+ILModuleDef: Boolean get_Is32Bit()
FSharp.Compiler.AbstractIL.IL+ILModuleDef: Boolean get_Is32BitPreferred()
FSharp.Compiler.AbstractIL.IL+ILModuleDef: Boolean get_Is64Bit()
FSharp.Compiler.AbstractIL.IL+ILModuleDef: Boolean get_IsDLL()
FSharp.Compiler.AbstractIL.IL+ILModuleDef: Boolean get_IsILOnly()
FSharp.Compiler.AbstractIL.IL+ILModuleDef: Boolean get_UseHighEntropyVA()
FSharp.Compiler.AbstractIL.IL+ILModuleDef: ILAssemblyManifest ManifestOfAssembly
FSharp.Compiler.AbstractIL.IL+ILModuleDef: ILAssemblyManifest get_ManifestOfAssembly()
FSharp.Compiler.AbstractIL.IL+ILModuleDef: ILAttributes CustomAttrs
FSharp.Compiler.AbstractIL.IL+ILModuleDef: ILAttributes get_CustomAttrs()
FSharp.Compiler.AbstractIL.IL+ILModuleDef: ILAttributesStored CustomAttrsStored
FSharp.Compiler.AbstractIL.IL+ILModuleDef: ILAttributesStored get_CustomAttrsStored()
FSharp.Compiler.AbstractIL.IL+ILModuleDef: ILResources Resources
FSharp.Compiler.AbstractIL.IL+ILModuleDef: ILResources get_Resources()
FSharp.Compiler.AbstractIL.IL+ILModuleDef: ILTypeDefs TypeDefs
FSharp.Compiler.AbstractIL.IL+ILModuleDef: ILTypeDefs get_TypeDefs()
FSharp.Compiler.AbstractIL.IL+ILModuleDef: Int32 ImageBase
FSharp.Compiler.AbstractIL.IL+ILModuleDef: Int32 MetadataIndex
FSharp.Compiler.AbstractIL.IL+ILModuleDef: Int32 PhysicalAlignment
FSharp.Compiler.AbstractIL.IL+ILModuleDef: Int32 SubSystemFlags
FSharp.Compiler.AbstractIL.IL+ILModuleDef: Int32 VirtualAlignment
FSharp.Compiler.AbstractIL.IL+ILModuleDef: Int32 get_ImageBase()
FSharp.Compiler.AbstractIL.IL+ILModuleDef: Int32 get_MetadataIndex()
FSharp.Compiler.AbstractIL.IL+ILModuleDef: Int32 get_PhysicalAlignment()
FSharp.Compiler.AbstractIL.IL+ILModuleDef: Int32 get_SubSystemFlags()
FSharp.Compiler.AbstractIL.IL+ILModuleDef: Int32 get_VirtualAlignment()
FSharp.Compiler.AbstractIL.IL+ILModuleDef: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILNativeResource] NativeResources
FSharp.Compiler.AbstractIL.IL+ILModuleDef: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILNativeResource] get_NativeResources()
FSharp.Compiler.AbstractIL.IL+ILModuleDef: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILAssemblyManifest] Manifest
FSharp.Compiler.AbstractIL.IL+ILModuleDef: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILAssemblyManifest] get_Manifest()
FSharp.Compiler.AbstractIL.IL+ILModuleDef: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILPlatform] Platform
FSharp.Compiler.AbstractIL.IL+ILModuleDef: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILPlatform] get_Platform()
FSharp.Compiler.AbstractIL.IL+ILModuleDef: Microsoft.FSharp.Core.FSharpOption`1[System.Int32] StackReserveSize
FSharp.Compiler.AbstractIL.IL+ILModuleDef: Microsoft.FSharp.Core.FSharpOption`1[System.Int32] get_StackReserveSize()
FSharp.Compiler.AbstractIL.IL+ILModuleDef: System.String MetadataVersion
FSharp.Compiler.AbstractIL.IL+ILModuleDef: System.String Name
FSharp.Compiler.AbstractIL.IL+ILModuleDef: System.String ToString()
FSharp.Compiler.AbstractIL.IL+ILModuleDef: System.String get_MetadataVersion()
FSharp.Compiler.AbstractIL.IL+ILModuleDef: System.String get_Name()
FSharp.Compiler.AbstractIL.IL+ILModuleDef: System.Tuple`2[System.Int32,System.Int32] SubsystemVersion
FSharp.Compiler.AbstractIL.IL+ILModuleDef: System.Tuple`2[System.Int32,System.Int32] get_SubsystemVersion()
FSharp.Compiler.AbstractIL.IL+ILModuleDef: Void .ctor(Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILAssemblyManifest], System.String, ILTypeDefs, System.Tuple`2[System.Int32,System.Int32], Boolean, Int32, Boolean, Boolean, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILPlatform], Microsoft.FSharp.Core.FSharpOption`1[System.Int32], Boolean, Boolean, Boolean, Int32, Int32, Int32, System.String, ILResources, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILNativeResource], ILAttributesStored, Int32)
FSharp.Compiler.AbstractIL.IL+ILModuleRef: Boolean Equals(ILModuleRef)
FSharp.Compiler.AbstractIL.IL+ILModuleRef: Boolean Equals(System.Object)
FSharp.Compiler.AbstractIL.IL+ILModuleRef: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILModuleRef: Boolean HasMetadata
FSharp.Compiler.AbstractIL.IL+ILModuleRef: Boolean get_HasMetadata()
FSharp.Compiler.AbstractIL.IL+ILModuleRef: ILModuleRef Create(System.String, Boolean, Microsoft.FSharp.Core.FSharpOption`1[System.Byte[]])
FSharp.Compiler.AbstractIL.IL+ILModuleRef: Int32 CompareTo(ILModuleRef)
FSharp.Compiler.AbstractIL.IL+ILModuleRef: Int32 CompareTo(System.Object)
FSharp.Compiler.AbstractIL.IL+ILModuleRef: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.AbstractIL.IL+ILModuleRef: Int32 GetHashCode()
FSharp.Compiler.AbstractIL.IL+ILModuleRef: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILModuleRef: Microsoft.FSharp.Core.FSharpOption`1[System.Byte[]] Hash
FSharp.Compiler.AbstractIL.IL+ILModuleRef: Microsoft.FSharp.Core.FSharpOption`1[System.Byte[]] get_Hash()
FSharp.Compiler.AbstractIL.IL+ILModuleRef: System.String Name
FSharp.Compiler.AbstractIL.IL+ILModuleRef: System.String ToString()
FSharp.Compiler.AbstractIL.IL+ILModuleRef: System.String get_Name()
FSharp.Compiler.AbstractIL.IL+ILNativeResource: Boolean Equals(ILNativeResource)
FSharp.Compiler.AbstractIL.IL+ILNativeResource: Boolean Equals(System.Object)
FSharp.Compiler.AbstractIL.IL+ILNativeResource: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILNativeResource: Int32 CompareTo(ILNativeResource)
FSharp.Compiler.AbstractIL.IL+ILNativeResource: Int32 CompareTo(System.Object)
FSharp.Compiler.AbstractIL.IL+ILNativeResource: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.AbstractIL.IL+ILNativeResource: Int32 GetHashCode()
FSharp.Compiler.AbstractIL.IL+ILNativeResource: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILNativeResource: System.String ToString()
FSharp.Compiler.AbstractIL.IL+ILNativeType: Boolean Equals(ILNativeType)
FSharp.Compiler.AbstractIL.IL+ILNativeType: Boolean Equals(System.Object)
FSharp.Compiler.AbstractIL.IL+ILNativeType: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILNativeType: Int32 CompareTo(ILNativeType)
FSharp.Compiler.AbstractIL.IL+ILNativeType: Int32 CompareTo(System.Object)
FSharp.Compiler.AbstractIL.IL+ILNativeType: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.AbstractIL.IL+ILNativeType: Int32 GetHashCode()
FSharp.Compiler.AbstractIL.IL+ILNativeType: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILNativeType: System.String ToString()
FSharp.Compiler.AbstractIL.IL+ILNestedExportedType: ILAttributes CustomAttrs
FSharp.Compiler.AbstractIL.IL+ILNestedExportedType: ILAttributes get_CustomAttrs()
FSharp.Compiler.AbstractIL.IL+ILNestedExportedType: ILAttributesStored CustomAttrsStored
FSharp.Compiler.AbstractIL.IL+ILNestedExportedType: ILAttributesStored get_CustomAttrsStored()
FSharp.Compiler.AbstractIL.IL+ILNestedExportedType: ILMemberAccess Access
FSharp.Compiler.AbstractIL.IL+ILNestedExportedType: ILMemberAccess get_Access()
FSharp.Compiler.AbstractIL.IL+ILNestedExportedType: ILNestedExportedTypes Nested
FSharp.Compiler.AbstractIL.IL+ILNestedExportedType: ILNestedExportedTypes get_Nested()
FSharp.Compiler.AbstractIL.IL+ILNestedExportedType: Int32 MetadataIndex
FSharp.Compiler.AbstractIL.IL+ILNestedExportedType: Int32 get_MetadataIndex()
FSharp.Compiler.AbstractIL.IL+ILNestedExportedType: System.String Name
FSharp.Compiler.AbstractIL.IL+ILNestedExportedType: System.String ToString()
FSharp.Compiler.AbstractIL.IL+ILNestedExportedType: System.String get_Name()
FSharp.Compiler.AbstractIL.IL+ILNestedExportedType: Void .ctor(System.String, ILMemberAccess, ILNestedExportedTypes, ILAttributesStored, Int32)
FSharp.Compiler.AbstractIL.IL+ILNestedExportedTypes: Boolean Equals(ILNestedExportedTypes)
FSharp.Compiler.AbstractIL.IL+ILNestedExportedTypes: Boolean Equals(System.Object)
FSharp.Compiler.AbstractIL.IL+ILNestedExportedTypes: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILNestedExportedTypes: Int32 GetHashCode()
FSharp.Compiler.AbstractIL.IL+ILNestedExportedTypes: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILNestedExportedTypes: System.String ToString()
FSharp.Compiler.AbstractIL.IL+ILParameter: Boolean IsIn
FSharp.Compiler.AbstractIL.IL+ILParameter: Boolean IsOptional
FSharp.Compiler.AbstractIL.IL+ILParameter: Boolean IsOut
FSharp.Compiler.AbstractIL.IL+ILParameter: Boolean get_IsIn()
FSharp.Compiler.AbstractIL.IL+ILParameter: Boolean get_IsOptional()
FSharp.Compiler.AbstractIL.IL+ILParameter: Boolean get_IsOut()
FSharp.Compiler.AbstractIL.IL+ILParameter: ILAttributes CustomAttrs
FSharp.Compiler.AbstractIL.IL+ILParameter: ILAttributes get_CustomAttrs()
FSharp.Compiler.AbstractIL.IL+ILParameter: ILAttributesStored CustomAttrsStored
FSharp.Compiler.AbstractIL.IL+ILParameter: ILAttributesStored get_CustomAttrsStored()
FSharp.Compiler.AbstractIL.IL+ILParameter: ILType Type
FSharp.Compiler.AbstractIL.IL+ILParameter: ILType get_Type()
FSharp.Compiler.AbstractIL.IL+ILParameter: Int32 MetadataIndex
FSharp.Compiler.AbstractIL.IL+ILParameter: Int32 get_MetadataIndex()
FSharp.Compiler.AbstractIL.IL+ILParameter: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILFieldInit] Default
FSharp.Compiler.AbstractIL.IL+ILParameter: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILFieldInit] get_Default()
FSharp.Compiler.AbstractIL.IL+ILParameter: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILNativeType] Marshal
FSharp.Compiler.AbstractIL.IL+ILParameter: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILNativeType] get_Marshal()
FSharp.Compiler.AbstractIL.IL+ILParameter: Microsoft.FSharp.Core.FSharpOption`1[System.String] Name
FSharp.Compiler.AbstractIL.IL+ILParameter: Microsoft.FSharp.Core.FSharpOption`1[System.String] get_Name()
FSharp.Compiler.AbstractIL.IL+ILParameter: System.String ToString()
FSharp.Compiler.AbstractIL.IL+ILParameter: Void .ctor(Microsoft.FSharp.Core.FSharpOption`1[System.String], ILType, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILFieldInit], Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILNativeType], Boolean, Boolean, Boolean, ILAttributesStored, Int32)
FSharp.Compiler.AbstractIL.IL+ILPlatform: Boolean Equals(ILPlatform)
FSharp.Compiler.AbstractIL.IL+ILPlatform: Boolean Equals(System.Object)
FSharp.Compiler.AbstractIL.IL+ILPlatform: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILPlatform: Int32 CompareTo(ILPlatform)
FSharp.Compiler.AbstractIL.IL+ILPlatform: Int32 CompareTo(System.Object)
FSharp.Compiler.AbstractIL.IL+ILPlatform: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.AbstractIL.IL+ILPlatform: Int32 GetHashCode()
FSharp.Compiler.AbstractIL.IL+ILPlatform: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILPlatform: System.String ToString()
FSharp.Compiler.AbstractIL.IL+ILPreTypeDef: ILTypeDef GetTypeDef()
FSharp.Compiler.AbstractIL.IL+ILPreTypeDef: Microsoft.FSharp.Collections.FSharpList`1[System.String] Namespace
FSharp.Compiler.AbstractIL.IL+ILPreTypeDef: Microsoft.FSharp.Collections.FSharpList`1[System.String] get_Namespace()
FSharp.Compiler.AbstractIL.IL+ILPreTypeDef: System.String Name
FSharp.Compiler.AbstractIL.IL+ILPreTypeDef: System.String get_Name()
FSharp.Compiler.AbstractIL.IL+ILPropertyDef: Boolean IsRTSpecialName
FSharp.Compiler.AbstractIL.IL+ILPropertyDef: Boolean IsSpecialName
FSharp.Compiler.AbstractIL.IL+ILPropertyDef: Boolean get_IsRTSpecialName()
FSharp.Compiler.AbstractIL.IL+ILPropertyDef: Boolean get_IsSpecialName()
FSharp.Compiler.AbstractIL.IL+ILPropertyDef: ILAttributes CustomAttrs
FSharp.Compiler.AbstractIL.IL+ILPropertyDef: ILAttributes get_CustomAttrs()
FSharp.Compiler.AbstractIL.IL+ILPropertyDef: ILThisConvention CallingConv
FSharp.Compiler.AbstractIL.IL+ILPropertyDef: ILThisConvention get_CallingConv()
FSharp.Compiler.AbstractIL.IL+ILPropertyDef: ILType PropertyType
FSharp.Compiler.AbstractIL.IL+ILPropertyDef: ILType get_PropertyType()
FSharp.Compiler.AbstractIL.IL+ILPropertyDef: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILType] Args
FSharp.Compiler.AbstractIL.IL+ILPropertyDef: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILType] get_Args()
FSharp.Compiler.AbstractIL.IL+ILPropertyDef: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILFieldInit] Init
FSharp.Compiler.AbstractIL.IL+ILPropertyDef: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILFieldInit] get_Init()
FSharp.Compiler.AbstractIL.IL+ILPropertyDef: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILMethodRef] GetMethod
FSharp.Compiler.AbstractIL.IL+ILPropertyDef: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILMethodRef] SetMethod
FSharp.Compiler.AbstractIL.IL+ILPropertyDef: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILMethodRef] get_GetMethod()
FSharp.Compiler.AbstractIL.IL+ILPropertyDef: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILMethodRef] get_SetMethod()
FSharp.Compiler.AbstractIL.IL+ILPropertyDef: System.Reflection.PropertyAttributes Attributes
FSharp.Compiler.AbstractIL.IL+ILPropertyDef: System.Reflection.PropertyAttributes get_Attributes()
FSharp.Compiler.AbstractIL.IL+ILPropertyDef: System.String Name
FSharp.Compiler.AbstractIL.IL+ILPropertyDef: System.String ToString()
FSharp.Compiler.AbstractIL.IL+ILPropertyDef: System.String get_Name()
FSharp.Compiler.AbstractIL.IL+ILPropertyDef: Void .ctor(System.String, System.Reflection.PropertyAttributes, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILMethodRef], Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILMethodRef], ILThisConvention, ILType, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILFieldInit], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILType], ILAttributes)
FSharp.Compiler.AbstractIL.IL+ILPropertyDefs: System.String ToString()
FSharp.Compiler.AbstractIL.IL+ILResources: System.String ToString()
FSharp.Compiler.AbstractIL.IL+ILReturn: ILAttributes CustomAttrs
FSharp.Compiler.AbstractIL.IL+ILReturn: ILAttributes get_CustomAttrs()
FSharp.Compiler.AbstractIL.IL+ILReturn: ILAttributesStored CustomAttrsStored
FSharp.Compiler.AbstractIL.IL+ILReturn: ILAttributesStored get_CustomAttrsStored()
FSharp.Compiler.AbstractIL.IL+ILReturn: ILReturn WithCustomAttrs(ILAttributes)
FSharp.Compiler.AbstractIL.IL+ILReturn: ILType Type
FSharp.Compiler.AbstractIL.IL+ILReturn: ILType get_Type()
FSharp.Compiler.AbstractIL.IL+ILReturn: Int32 MetadataIndex
FSharp.Compiler.AbstractIL.IL+ILReturn: Int32 get_MetadataIndex()
FSharp.Compiler.AbstractIL.IL+ILReturn: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILNativeType] Marshal
FSharp.Compiler.AbstractIL.IL+ILReturn: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILNativeType] get_Marshal()
FSharp.Compiler.AbstractIL.IL+ILReturn: System.String ToString()
FSharp.Compiler.AbstractIL.IL+ILReturn: Void .ctor(Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILNativeType], ILType, ILAttributesStored, Int32)
FSharp.Compiler.AbstractIL.IL+ILScopeRef+Assembly: ILAssemblyRef Item
FSharp.Compiler.AbstractIL.IL+ILScopeRef+Assembly: ILAssemblyRef get_Item()
FSharp.Compiler.AbstractIL.IL+ILScopeRef+Module: ILModuleRef Item
FSharp.Compiler.AbstractIL.IL+ILScopeRef+Module: ILModuleRef get_Item()
FSharp.Compiler.AbstractIL.IL+ILScopeRef+Tags: Int32 Assembly
FSharp.Compiler.AbstractIL.IL+ILScopeRef+Tags: Int32 Local
FSharp.Compiler.AbstractIL.IL+ILScopeRef+Tags: Int32 Module
FSharp.Compiler.AbstractIL.IL+ILScopeRef+Tags: Int32 PrimaryAssembly
FSharp.Compiler.AbstractIL.IL+ILScopeRef: Boolean Equals(ILScopeRef)
FSharp.Compiler.AbstractIL.IL+ILScopeRef: Boolean Equals(System.Object)
FSharp.Compiler.AbstractIL.IL+ILScopeRef: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILScopeRef: Boolean IsAssembly
FSharp.Compiler.AbstractIL.IL+ILScopeRef: Boolean IsLocal
FSharp.Compiler.AbstractIL.IL+ILScopeRef: Boolean IsLocalRef
FSharp.Compiler.AbstractIL.IL+ILScopeRef: Boolean IsModule
FSharp.Compiler.AbstractIL.IL+ILScopeRef: Boolean IsPrimaryAssembly
FSharp.Compiler.AbstractIL.IL+ILScopeRef: Boolean get_IsAssembly()
FSharp.Compiler.AbstractIL.IL+ILScopeRef: Boolean get_IsLocal()
FSharp.Compiler.AbstractIL.IL+ILScopeRef: Boolean get_IsLocalRef()
FSharp.Compiler.AbstractIL.IL+ILScopeRef: Boolean get_IsModule()
FSharp.Compiler.AbstractIL.IL+ILScopeRef: Boolean get_IsPrimaryAssembly()
FSharp.Compiler.AbstractIL.IL+ILScopeRef: FSharp.Compiler.AbstractIL.IL+ILScopeRef+Assembly
FSharp.Compiler.AbstractIL.IL+ILScopeRef: FSharp.Compiler.AbstractIL.IL+ILScopeRef+Module
FSharp.Compiler.AbstractIL.IL+ILScopeRef: FSharp.Compiler.AbstractIL.IL+ILScopeRef+Tags
FSharp.Compiler.AbstractIL.IL+ILScopeRef: ILScopeRef Local
FSharp.Compiler.AbstractIL.IL+ILScopeRef: ILScopeRef NewAssembly(ILAssemblyRef)
FSharp.Compiler.AbstractIL.IL+ILScopeRef: ILScopeRef NewModule(ILModuleRef)
FSharp.Compiler.AbstractIL.IL+ILScopeRef: ILScopeRef PrimaryAssembly
FSharp.Compiler.AbstractIL.IL+ILScopeRef: ILScopeRef get_Local()
FSharp.Compiler.AbstractIL.IL+ILScopeRef: ILScopeRef get_PrimaryAssembly()
FSharp.Compiler.AbstractIL.IL+ILScopeRef: Int32 CompareTo(ILScopeRef)
FSharp.Compiler.AbstractIL.IL+ILScopeRef: Int32 CompareTo(System.Object)
FSharp.Compiler.AbstractIL.IL+ILScopeRef: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.AbstractIL.IL+ILScopeRef: Int32 GetHashCode()
FSharp.Compiler.AbstractIL.IL+ILScopeRef: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILScopeRef: Int32 Tag
FSharp.Compiler.AbstractIL.IL+ILScopeRef: Int32 get_Tag()
FSharp.Compiler.AbstractIL.IL+ILScopeRef: System.String QualifiedName
FSharp.Compiler.AbstractIL.IL+ILScopeRef: System.String ToString()
FSharp.Compiler.AbstractIL.IL+ILScopeRef: System.String get_QualifiedName()
FSharp.Compiler.AbstractIL.IL+ILSecurityDeclsStored: System.String ToString()
FSharp.Compiler.AbstractIL.IL+ILSourceDocument: Boolean Equals(ILSourceDocument)
FSharp.Compiler.AbstractIL.IL+ILSourceDocument: Boolean Equals(System.Object)
FSharp.Compiler.AbstractIL.IL+ILSourceDocument: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILSourceDocument: ILSourceDocument Create(Microsoft.FSharp.Core.FSharpOption`1[System.Byte[]], Microsoft.FSharp.Core.FSharpOption`1[System.Byte[]], Microsoft.FSharp.Core.FSharpOption`1[System.Byte[]], System.String)
FSharp.Compiler.AbstractIL.IL+ILSourceDocument: Int32 CompareTo(ILSourceDocument)
FSharp.Compiler.AbstractIL.IL+ILSourceDocument: Int32 CompareTo(System.Object)
FSharp.Compiler.AbstractIL.IL+ILSourceDocument: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.AbstractIL.IL+ILSourceDocument: Int32 GetHashCode()
FSharp.Compiler.AbstractIL.IL+ILSourceDocument: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILSourceDocument: Microsoft.FSharp.Core.FSharpOption`1[System.Byte[]] DocumentType
FSharp.Compiler.AbstractIL.IL+ILSourceDocument: Microsoft.FSharp.Core.FSharpOption`1[System.Byte[]] Language
FSharp.Compiler.AbstractIL.IL+ILSourceDocument: Microsoft.FSharp.Core.FSharpOption`1[System.Byte[]] Vendor
FSharp.Compiler.AbstractIL.IL+ILSourceDocument: Microsoft.FSharp.Core.FSharpOption`1[System.Byte[]] get_DocumentType()
FSharp.Compiler.AbstractIL.IL+ILSourceDocument: Microsoft.FSharp.Core.FSharpOption`1[System.Byte[]] get_Language()
FSharp.Compiler.AbstractIL.IL+ILSourceDocument: Microsoft.FSharp.Core.FSharpOption`1[System.Byte[]] get_Vendor()
FSharp.Compiler.AbstractIL.IL+ILSourceDocument: System.String File
FSharp.Compiler.AbstractIL.IL+ILSourceDocument: System.String ToString()
FSharp.Compiler.AbstractIL.IL+ILSourceDocument: System.String get_File()
FSharp.Compiler.AbstractIL.IL+ILType+Array: ILArrayShape Item1
FSharp.Compiler.AbstractIL.IL+ILType+Array: ILArrayShape get_Item1()
FSharp.Compiler.AbstractIL.IL+ILType+Array: ILType Item2
FSharp.Compiler.AbstractIL.IL+ILType+Array: ILType get_Item2()
FSharp.Compiler.AbstractIL.IL+ILType+Boxed: ILTypeSpec Item
FSharp.Compiler.AbstractIL.IL+ILType+Boxed: ILTypeSpec get_Item()
FSharp.Compiler.AbstractIL.IL+ILType+Byref: ILType Item
FSharp.Compiler.AbstractIL.IL+ILType+Byref: ILType get_Item()
FSharp.Compiler.AbstractIL.IL+ILType+FunctionPointer: ILCallingSignature Item
FSharp.Compiler.AbstractIL.IL+ILType+FunctionPointer: ILCallingSignature get_Item()
FSharp.Compiler.AbstractIL.IL+ILType+Modified: Boolean Item1
FSharp.Compiler.AbstractIL.IL+ILType+Modified: Boolean get_Item1()
FSharp.Compiler.AbstractIL.IL+ILType+Modified: ILType Item3
FSharp.Compiler.AbstractIL.IL+ILType+Modified: ILType get_Item3()
FSharp.Compiler.AbstractIL.IL+ILType+Modified: ILTypeRef Item2
FSharp.Compiler.AbstractIL.IL+ILType+Modified: ILTypeRef get_Item2()
FSharp.Compiler.AbstractIL.IL+ILType+Ptr: ILType Item
FSharp.Compiler.AbstractIL.IL+ILType+Ptr: ILType get_Item()
FSharp.Compiler.AbstractIL.IL+ILType+Tags: Int32 Array
FSharp.Compiler.AbstractIL.IL+ILType+Tags: Int32 Boxed
FSharp.Compiler.AbstractIL.IL+ILType+Tags: Int32 Byref
FSharp.Compiler.AbstractIL.IL+ILType+Tags: Int32 FunctionPointer
FSharp.Compiler.AbstractIL.IL+ILType+Tags: Int32 Modified
FSharp.Compiler.AbstractIL.IL+ILType+Tags: Int32 Ptr
FSharp.Compiler.AbstractIL.IL+ILType+Tags: Int32 TypeVar
FSharp.Compiler.AbstractIL.IL+ILType+Tags: Int32 Value
FSharp.Compiler.AbstractIL.IL+ILType+Tags: Int32 Void
FSharp.Compiler.AbstractIL.IL+ILType+TypeVar: UInt16 Item
FSharp.Compiler.AbstractIL.IL+ILType+TypeVar: UInt16 get_Item()
FSharp.Compiler.AbstractIL.IL+ILType+Value: ILTypeSpec Item
FSharp.Compiler.AbstractIL.IL+ILType+Value: ILTypeSpec get_Item()
FSharp.Compiler.AbstractIL.IL+ILType: Boolean Equals(ILType)
FSharp.Compiler.AbstractIL.IL+ILType: Boolean Equals(System.Object)
FSharp.Compiler.AbstractIL.IL+ILType: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILType: Boolean IsArray
FSharp.Compiler.AbstractIL.IL+ILType: Boolean IsBoxed
FSharp.Compiler.AbstractIL.IL+ILType: Boolean IsByref
FSharp.Compiler.AbstractIL.IL+ILType: Boolean IsFunctionPointer
FSharp.Compiler.AbstractIL.IL+ILType: Boolean IsModified
FSharp.Compiler.AbstractIL.IL+ILType: Boolean IsNominal
FSharp.Compiler.AbstractIL.IL+ILType: Boolean IsPtr
FSharp.Compiler.AbstractIL.IL+ILType: Boolean IsTypeVar
FSharp.Compiler.AbstractIL.IL+ILType: Boolean IsTyvar
FSharp.Compiler.AbstractIL.IL+ILType: Boolean IsValue
FSharp.Compiler.AbstractIL.IL+ILType: Boolean IsVoid
FSharp.Compiler.AbstractIL.IL+ILType: Boolean get_IsArray()
FSharp.Compiler.AbstractIL.IL+ILType: Boolean get_IsBoxed()
FSharp.Compiler.AbstractIL.IL+ILType: Boolean get_IsByref()
FSharp.Compiler.AbstractIL.IL+ILType: Boolean get_IsFunctionPointer()
FSharp.Compiler.AbstractIL.IL+ILType: Boolean get_IsModified()
FSharp.Compiler.AbstractIL.IL+ILType: Boolean get_IsNominal()
FSharp.Compiler.AbstractIL.IL+ILType: Boolean get_IsPtr()
FSharp.Compiler.AbstractIL.IL+ILType: Boolean get_IsTypeVar()
FSharp.Compiler.AbstractIL.IL+ILType: Boolean get_IsTyvar()
FSharp.Compiler.AbstractIL.IL+ILType: Boolean get_IsValue()
FSharp.Compiler.AbstractIL.IL+ILType: Boolean get_IsVoid()
FSharp.Compiler.AbstractIL.IL+ILType: FSharp.Compiler.AbstractIL.IL+ILType+Array
FSharp.Compiler.AbstractIL.IL+ILType: FSharp.Compiler.AbstractIL.IL+ILType+Boxed
FSharp.Compiler.AbstractIL.IL+ILType: FSharp.Compiler.AbstractIL.IL+ILType+Byref
FSharp.Compiler.AbstractIL.IL+ILType: FSharp.Compiler.AbstractIL.IL+ILType+FunctionPointer
FSharp.Compiler.AbstractIL.IL+ILType: FSharp.Compiler.AbstractIL.IL+ILType+Modified
FSharp.Compiler.AbstractIL.IL+ILType: FSharp.Compiler.AbstractIL.IL+ILType+Ptr
FSharp.Compiler.AbstractIL.IL+ILType: FSharp.Compiler.AbstractIL.IL+ILType+Tags
FSharp.Compiler.AbstractIL.IL+ILType: FSharp.Compiler.AbstractIL.IL+ILType+TypeVar
FSharp.Compiler.AbstractIL.IL+ILType: FSharp.Compiler.AbstractIL.IL+ILType+Value
FSharp.Compiler.AbstractIL.IL+ILType: ILType NewArray(ILArrayShape, ILType)
FSharp.Compiler.AbstractIL.IL+ILType: ILType NewBoxed(ILTypeSpec)
FSharp.Compiler.AbstractIL.IL+ILType: ILType NewByref(ILType)
FSharp.Compiler.AbstractIL.IL+ILType: ILType NewFunctionPointer(ILCallingSignature)
FSharp.Compiler.AbstractIL.IL+ILType: ILType NewModified(Boolean, ILTypeRef, ILType)
FSharp.Compiler.AbstractIL.IL+ILType: ILType NewPtr(ILType)
FSharp.Compiler.AbstractIL.IL+ILType: ILType NewTypeVar(UInt16)
FSharp.Compiler.AbstractIL.IL+ILType: ILType NewValue(ILTypeSpec)
FSharp.Compiler.AbstractIL.IL+ILType: ILType Void
FSharp.Compiler.AbstractIL.IL+ILType: ILType get_Void()
FSharp.Compiler.AbstractIL.IL+ILType: ILTypeRef TypeRef
FSharp.Compiler.AbstractIL.IL+ILType: ILTypeRef get_TypeRef()
FSharp.Compiler.AbstractIL.IL+ILType: ILTypeSpec TypeSpec
FSharp.Compiler.AbstractIL.IL+ILType: ILTypeSpec get_TypeSpec()
FSharp.Compiler.AbstractIL.IL+ILType: Int32 CompareTo(ILType)
FSharp.Compiler.AbstractIL.IL+ILType: Int32 CompareTo(System.Object)
FSharp.Compiler.AbstractIL.IL+ILType: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.AbstractIL.IL+ILType: Int32 GetHashCode()
FSharp.Compiler.AbstractIL.IL+ILType: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILType: Int32 Tag
FSharp.Compiler.AbstractIL.IL+ILType: Int32 get_Tag()
FSharp.Compiler.AbstractIL.IL+ILType: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILType] GenericArgs
FSharp.Compiler.AbstractIL.IL+ILType: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILType] get_GenericArgs()
FSharp.Compiler.AbstractIL.IL+ILType: System.String BasicQualifiedName
FSharp.Compiler.AbstractIL.IL+ILType: System.String QualifiedName
FSharp.Compiler.AbstractIL.IL+ILType: System.String ToString()
FSharp.Compiler.AbstractIL.IL+ILType: System.String get_BasicQualifiedName()
FSharp.Compiler.AbstractIL.IL+ILType: System.String get_QualifiedName()
FSharp.Compiler.AbstractIL.IL+ILTypeDef: Boolean HasSecurity
FSharp.Compiler.AbstractIL.IL+ILTypeDef: Boolean IsAbstract
FSharp.Compiler.AbstractIL.IL+ILTypeDef: Boolean IsClass
FSharp.Compiler.AbstractIL.IL+ILTypeDef: Boolean IsComInterop
FSharp.Compiler.AbstractIL.IL+ILTypeDef: Boolean IsDelegate
FSharp.Compiler.AbstractIL.IL+ILTypeDef: Boolean IsEnum
FSharp.Compiler.AbstractIL.IL+ILTypeDef: Boolean IsInterface
FSharp.Compiler.AbstractIL.IL+ILTypeDef: Boolean IsSealed
FSharp.Compiler.AbstractIL.IL+ILTypeDef: Boolean IsSerializable
FSharp.Compiler.AbstractIL.IL+ILTypeDef: Boolean IsSpecialName
FSharp.Compiler.AbstractIL.IL+ILTypeDef: Boolean IsStruct
FSharp.Compiler.AbstractIL.IL+ILTypeDef: Boolean IsStructOrEnum
FSharp.Compiler.AbstractIL.IL+ILTypeDef: Boolean get_HasSecurity()
FSharp.Compiler.AbstractIL.IL+ILTypeDef: Boolean get_IsAbstract()
FSharp.Compiler.AbstractIL.IL+ILTypeDef: Boolean get_IsClass()
FSharp.Compiler.AbstractIL.IL+ILTypeDef: Boolean get_IsComInterop()
FSharp.Compiler.AbstractIL.IL+ILTypeDef: Boolean get_IsDelegate()
FSharp.Compiler.AbstractIL.IL+ILTypeDef: Boolean get_IsEnum()
FSharp.Compiler.AbstractIL.IL+ILTypeDef: Boolean get_IsInterface()
FSharp.Compiler.AbstractIL.IL+ILTypeDef: Boolean get_IsSealed()
FSharp.Compiler.AbstractIL.IL+ILTypeDef: Boolean get_IsSerializable()
FSharp.Compiler.AbstractIL.IL+ILTypeDef: Boolean get_IsSpecialName()
FSharp.Compiler.AbstractIL.IL+ILTypeDef: Boolean get_IsStruct()
FSharp.Compiler.AbstractIL.IL+ILTypeDef: Boolean get_IsStructOrEnum()
FSharp.Compiler.AbstractIL.IL+ILTypeDef: ILAttributes CustomAttrs
FSharp.Compiler.AbstractIL.IL+ILTypeDef: ILAttributes get_CustomAttrs()
FSharp.Compiler.AbstractIL.IL+ILTypeDef: ILDefaultPInvokeEncoding Encoding
FSharp.Compiler.AbstractIL.IL+ILTypeDef: ILDefaultPInvokeEncoding get_Encoding()
FSharp.Compiler.AbstractIL.IL+ILTypeDef: ILEventDefs Events
FSharp.Compiler.AbstractIL.IL+ILTypeDef: ILEventDefs get_Events()
FSharp.Compiler.AbstractIL.IL+ILTypeDef: ILFieldDefs Fields
FSharp.Compiler.AbstractIL.IL+ILTypeDef: ILFieldDefs get_Fields()
FSharp.Compiler.AbstractIL.IL+ILTypeDef: ILMethodDefs Methods
FSharp.Compiler.AbstractIL.IL+ILTypeDef: ILMethodDefs get_Methods()
FSharp.Compiler.AbstractIL.IL+ILTypeDef: ILMethodImplDefs MethodImpls
FSharp.Compiler.AbstractIL.IL+ILTypeDef: ILMethodImplDefs get_MethodImpls()
FSharp.Compiler.AbstractIL.IL+ILTypeDef: ILPropertyDefs Properties
FSharp.Compiler.AbstractIL.IL+ILTypeDef: ILPropertyDefs get_Properties()
FSharp.Compiler.AbstractIL.IL+ILTypeDef: ILSecurityDecls SecurityDecls
FSharp.Compiler.AbstractIL.IL+ILTypeDef: ILSecurityDecls get_SecurityDecls()
FSharp.Compiler.AbstractIL.IL+ILTypeDef: ILTypeDef With(Microsoft.FSharp.Core.FSharpOption`1[System.String], Microsoft.FSharp.Core.FSharpOption`1[System.Reflection.TypeAttributes], Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILTypeDefLayout], Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILType]], Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILGenericParameterDef]], Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILType]], Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILMethodDefs], Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILTypeDefs], Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILFieldDefs], Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILMethodImplDefs], Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILEventDefs], Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILPropertyDefs], Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILAttributes], Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILSecurityDecls])
FSharp.Compiler.AbstractIL.IL+ILTypeDef: ILTypeDefAccess Access
FSharp.Compiler.AbstractIL.IL+ILTypeDef: ILTypeDefAccess get_Access()
FSharp.Compiler.AbstractIL.IL+ILTypeDef: ILTypeDefLayout Layout
FSharp.Compiler.AbstractIL.IL+ILTypeDef: ILTypeDefLayout get_Layout()
FSharp.Compiler.AbstractIL.IL+ILTypeDef: ILTypeDefs NestedTypes
FSharp.Compiler.AbstractIL.IL+ILTypeDef: ILTypeDefs get_NestedTypes()
FSharp.Compiler.AbstractIL.IL+ILTypeDef: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILGenericParameterDef] GenericParams
FSharp.Compiler.AbstractIL.IL+ILTypeDef: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILGenericParameterDef] get_GenericParams()
FSharp.Compiler.AbstractIL.IL+ILTypeDef: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILType] Implements
FSharp.Compiler.AbstractIL.IL+ILTypeDef: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILType] get_Implements()
FSharp.Compiler.AbstractIL.IL+ILTypeDef: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILType] Extends
FSharp.Compiler.AbstractIL.IL+ILTypeDef: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILType] get_Extends()
FSharp.Compiler.AbstractIL.IL+ILTypeDef: System.Reflection.TypeAttributes Attributes
FSharp.Compiler.AbstractIL.IL+ILTypeDef: System.Reflection.TypeAttributes get_Attributes()
FSharp.Compiler.AbstractIL.IL+ILTypeDef: System.String Name
FSharp.Compiler.AbstractIL.IL+ILTypeDef: System.String get_Name()
FSharp.Compiler.AbstractIL.IL+ILTypeDef: Void .ctor(System.String, System.Reflection.TypeAttributes, ILTypeDefLayout, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILType], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILGenericParameterDef], Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.AbstractIL.IL+ILType], ILMethodDefs, ILTypeDefs, ILFieldDefs, ILMethodImplDefs, ILEventDefs, ILPropertyDefs, ILSecurityDecls, ILAttributes)
FSharp.Compiler.AbstractIL.IL+ILTypeDefAccess+Nested: ILMemberAccess Item
FSharp.Compiler.AbstractIL.IL+ILTypeDefAccess+Nested: ILMemberAccess get_Item()
FSharp.Compiler.AbstractIL.IL+ILTypeDefAccess+Tags: Int32 Nested
FSharp.Compiler.AbstractIL.IL+ILTypeDefAccess+Tags: Int32 Private
FSharp.Compiler.AbstractIL.IL+ILTypeDefAccess+Tags: Int32 Public
FSharp.Compiler.AbstractIL.IL+ILTypeDefAccess: Boolean Equals(ILTypeDefAccess)
FSharp.Compiler.AbstractIL.IL+ILTypeDefAccess: Boolean Equals(System.Object)
FSharp.Compiler.AbstractIL.IL+ILTypeDefAccess: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILTypeDefAccess: Boolean IsNested
FSharp.Compiler.AbstractIL.IL+ILTypeDefAccess: Boolean IsPrivate
FSharp.Compiler.AbstractIL.IL+ILTypeDefAccess: Boolean IsPublic
FSharp.Compiler.AbstractIL.IL+ILTypeDefAccess: Boolean get_IsNested()
FSharp.Compiler.AbstractIL.IL+ILTypeDefAccess: Boolean get_IsPrivate()
FSharp.Compiler.AbstractIL.IL+ILTypeDefAccess: Boolean get_IsPublic()
FSharp.Compiler.AbstractIL.IL+ILTypeDefAccess: FSharp.Compiler.AbstractIL.IL+ILTypeDefAccess+Nested
FSharp.Compiler.AbstractIL.IL+ILTypeDefAccess: FSharp.Compiler.AbstractIL.IL+ILTypeDefAccess+Tags
FSharp.Compiler.AbstractIL.IL+ILTypeDefAccess: ILTypeDefAccess NewNested(ILMemberAccess)
FSharp.Compiler.AbstractIL.IL+ILTypeDefAccess: ILTypeDefAccess Private
FSharp.Compiler.AbstractIL.IL+ILTypeDefAccess: ILTypeDefAccess Public
FSharp.Compiler.AbstractIL.IL+ILTypeDefAccess: ILTypeDefAccess get_Private()
FSharp.Compiler.AbstractIL.IL+ILTypeDefAccess: ILTypeDefAccess get_Public()
FSharp.Compiler.AbstractIL.IL+ILTypeDefAccess: Int32 CompareTo(ILTypeDefAccess)
FSharp.Compiler.AbstractIL.IL+ILTypeDefAccess: Int32 CompareTo(System.Object)
FSharp.Compiler.AbstractIL.IL+ILTypeDefAccess: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.AbstractIL.IL+ILTypeDefAccess: Int32 GetHashCode()
FSharp.Compiler.AbstractIL.IL+ILTypeDefAccess: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILTypeDefAccess: Int32 Tag
FSharp.Compiler.AbstractIL.IL+ILTypeDefAccess: Int32 get_Tag()
FSharp.Compiler.AbstractIL.IL+ILTypeDefAccess: System.String ToString()
FSharp.Compiler.AbstractIL.IL+ILTypeDefKind+Tags: Int32 Class
FSharp.Compiler.AbstractIL.IL+ILTypeDefKind+Tags: Int32 Delegate
FSharp.Compiler.AbstractIL.IL+ILTypeDefKind+Tags: Int32 Enum
FSharp.Compiler.AbstractIL.IL+ILTypeDefKind+Tags: Int32 Interface
FSharp.Compiler.AbstractIL.IL+ILTypeDefKind+Tags: Int32 ValueType
FSharp.Compiler.AbstractIL.IL+ILTypeDefKind: Boolean Equals(ILTypeDefKind)
FSharp.Compiler.AbstractIL.IL+ILTypeDefKind: Boolean Equals(System.Object)
FSharp.Compiler.AbstractIL.IL+ILTypeDefKind: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILTypeDefKind: Boolean IsClass
FSharp.Compiler.AbstractIL.IL+ILTypeDefKind: Boolean IsDelegate
FSharp.Compiler.AbstractIL.IL+ILTypeDefKind: Boolean IsEnum
FSharp.Compiler.AbstractIL.IL+ILTypeDefKind: Boolean IsInterface
FSharp.Compiler.AbstractIL.IL+ILTypeDefKind: Boolean IsValueType
FSharp.Compiler.AbstractIL.IL+ILTypeDefKind: Boolean get_IsClass()
FSharp.Compiler.AbstractIL.IL+ILTypeDefKind: Boolean get_IsDelegate()
FSharp.Compiler.AbstractIL.IL+ILTypeDefKind: Boolean get_IsEnum()
FSharp.Compiler.AbstractIL.IL+ILTypeDefKind: Boolean get_IsInterface()
FSharp.Compiler.AbstractIL.IL+ILTypeDefKind: Boolean get_IsValueType()
FSharp.Compiler.AbstractIL.IL+ILTypeDefKind: FSharp.Compiler.AbstractIL.IL+ILTypeDefKind+Tags
FSharp.Compiler.AbstractIL.IL+ILTypeDefKind: ILTypeDefKind Class
FSharp.Compiler.AbstractIL.IL+ILTypeDefKind: ILTypeDefKind Delegate
FSharp.Compiler.AbstractIL.IL+ILTypeDefKind: ILTypeDefKind Enum
FSharp.Compiler.AbstractIL.IL+ILTypeDefKind: ILTypeDefKind Interface
FSharp.Compiler.AbstractIL.IL+ILTypeDefKind: ILTypeDefKind ValueType
FSharp.Compiler.AbstractIL.IL+ILTypeDefKind: ILTypeDefKind get_Class()
FSharp.Compiler.AbstractIL.IL+ILTypeDefKind: ILTypeDefKind get_Delegate()
FSharp.Compiler.AbstractIL.IL+ILTypeDefKind: ILTypeDefKind get_Enum()
FSharp.Compiler.AbstractIL.IL+ILTypeDefKind: ILTypeDefKind get_Interface()
FSharp.Compiler.AbstractIL.IL+ILTypeDefKind: ILTypeDefKind get_ValueType()
FSharp.Compiler.AbstractIL.IL+ILTypeDefKind: Int32 CompareTo(ILTypeDefKind)
FSharp.Compiler.AbstractIL.IL+ILTypeDefKind: Int32 CompareTo(System.Object)
FSharp.Compiler.AbstractIL.IL+ILTypeDefKind: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.AbstractIL.IL+ILTypeDefKind: Int32 GetHashCode()
FSharp.Compiler.AbstractIL.IL+ILTypeDefKind: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILTypeDefKind: Int32 Tag
FSharp.Compiler.AbstractIL.IL+ILTypeDefKind: Int32 get_Tag()
FSharp.Compiler.AbstractIL.IL+ILTypeDefKind: System.String ToString()
FSharp.Compiler.AbstractIL.IL+ILTypeDefLayout: Boolean Equals(ILTypeDefLayout)
FSharp.Compiler.AbstractIL.IL+ILTypeDefLayout: Boolean Equals(System.Object)
FSharp.Compiler.AbstractIL.IL+ILTypeDefLayout: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILTypeDefLayout: Int32 CompareTo(ILTypeDefLayout)
FSharp.Compiler.AbstractIL.IL+ILTypeDefLayout: Int32 CompareTo(System.Object)
FSharp.Compiler.AbstractIL.IL+ILTypeDefLayout: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.AbstractIL.IL+ILTypeDefLayout: Int32 GetHashCode()
FSharp.Compiler.AbstractIL.IL+ILTypeDefLayout: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILTypeDefLayout: System.String ToString()
FSharp.Compiler.AbstractIL.IL+ILTypeInit+Tags: Int32 BeforeField
FSharp.Compiler.AbstractIL.IL+ILTypeInit+Tags: Int32 OnAny
FSharp.Compiler.AbstractIL.IL+ILTypeInit: Boolean Equals(ILTypeInit)
FSharp.Compiler.AbstractIL.IL+ILTypeInit: Boolean Equals(System.Object)
FSharp.Compiler.AbstractIL.IL+ILTypeInit: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILTypeInit: Boolean IsBeforeField
FSharp.Compiler.AbstractIL.IL+ILTypeInit: Boolean IsOnAny
FSharp.Compiler.AbstractIL.IL+ILTypeInit: Boolean get_IsBeforeField()
FSharp.Compiler.AbstractIL.IL+ILTypeInit: Boolean get_IsOnAny()
FSharp.Compiler.AbstractIL.IL+ILTypeInit: FSharp.Compiler.AbstractIL.IL+ILTypeInit+Tags
FSharp.Compiler.AbstractIL.IL+ILTypeInit: ILTypeInit BeforeField
FSharp.Compiler.AbstractIL.IL+ILTypeInit: ILTypeInit OnAny
FSharp.Compiler.AbstractIL.IL+ILTypeInit: ILTypeInit get_BeforeField()
FSharp.Compiler.AbstractIL.IL+ILTypeInit: ILTypeInit get_OnAny()
FSharp.Compiler.AbstractIL.IL+ILTypeInit: Int32 CompareTo(ILTypeInit)
FSharp.Compiler.AbstractIL.IL+ILTypeInit: Int32 CompareTo(System.Object)
FSharp.Compiler.AbstractIL.IL+ILTypeInit: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.AbstractIL.IL+ILTypeInit: Int32 GetHashCode()
FSharp.Compiler.AbstractIL.IL+ILTypeInit: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILTypeInit: Int32 Tag
FSharp.Compiler.AbstractIL.IL+ILTypeInit: Int32 get_Tag()
FSharp.Compiler.AbstractIL.IL+ILTypeInit: System.String ToString()
FSharp.Compiler.AbstractIL.IL+ILTypeRef: Boolean Equals(System.Object)
FSharp.Compiler.AbstractIL.IL+ILTypeRef: ILScopeRef Scope
FSharp.Compiler.AbstractIL.IL+ILTypeRef: ILScopeRef get_Scope()
FSharp.Compiler.AbstractIL.IL+ILTypeRef: ILTypeRef Create(ILScopeRef, Microsoft.FSharp.Collections.FSharpList`1[System.String], System.String)
FSharp.Compiler.AbstractIL.IL+ILTypeRef: Int32 GetHashCode()
FSharp.Compiler.AbstractIL.IL+ILTypeRef: Microsoft.FSharp.Collections.FSharpList`1[System.String] Enclosing
FSharp.Compiler.AbstractIL.IL+ILTypeRef: Microsoft.FSharp.Collections.FSharpList`1[System.String] get_Enclosing()
FSharp.Compiler.AbstractIL.IL+ILTypeRef: System.String BasicQualifiedName
FSharp.Compiler.AbstractIL.IL+ILTypeRef: System.String FullName
FSharp.Compiler.AbstractIL.IL+ILTypeRef: System.String Name
FSharp.Compiler.AbstractIL.IL+ILTypeRef: System.String QualifiedName
FSharp.Compiler.AbstractIL.IL+ILTypeRef: System.String ToString()
FSharp.Compiler.AbstractIL.IL+ILTypeRef: System.String get_BasicQualifiedName()
FSharp.Compiler.AbstractIL.IL+ILTypeRef: System.String get_FullName()
FSharp.Compiler.AbstractIL.IL+ILTypeRef: System.String get_Name()
FSharp.Compiler.AbstractIL.IL+ILTypeRef: System.String get_QualifiedName()
FSharp.Compiler.AbstractIL.IL+ILTypeSpec: Boolean Equals(ILTypeSpec)
FSharp.Compiler.AbstractIL.IL+ILTypeSpec: Boolean Equals(System.Object)
FSharp.Compiler.AbstractIL.IL+ILTypeSpec: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILTypeSpec: ILScopeRef Scope
FSharp.Compiler.AbstractIL.IL+ILTypeSpec: ILScopeRef get_Scope()
FSharp.Compiler.AbstractIL.IL+ILTypeSpec: ILTypeRef TypeRef
FSharp.Compiler.AbstractIL.IL+ILTypeSpec: ILTypeRef get_TypeRef()
FSharp.Compiler.AbstractIL.IL+ILTypeSpec: ILTypeSpec Create(ILTypeRef, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILType])
FSharp.Compiler.AbstractIL.IL+ILTypeSpec: Int32 CompareTo(ILTypeSpec)
FSharp.Compiler.AbstractIL.IL+ILTypeSpec: Int32 CompareTo(System.Object)
FSharp.Compiler.AbstractIL.IL+ILTypeSpec: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.AbstractIL.IL+ILTypeSpec: Int32 GetHashCode()
FSharp.Compiler.AbstractIL.IL+ILTypeSpec: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILTypeSpec: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILType] GenericArgs
FSharp.Compiler.AbstractIL.IL+ILTypeSpec: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILType] get_GenericArgs()
FSharp.Compiler.AbstractIL.IL+ILTypeSpec: Microsoft.FSharp.Collections.FSharpList`1[System.String] Enclosing
FSharp.Compiler.AbstractIL.IL+ILTypeSpec: Microsoft.FSharp.Collections.FSharpList`1[System.String] get_Enclosing()
FSharp.Compiler.AbstractIL.IL+ILTypeSpec: System.String FullName
FSharp.Compiler.AbstractIL.IL+ILTypeSpec: System.String Name
FSharp.Compiler.AbstractIL.IL+ILTypeSpec: System.String ToString()
FSharp.Compiler.AbstractIL.IL+ILTypeSpec: System.String get_FullName()
FSharp.Compiler.AbstractIL.IL+ILTypeSpec: System.String get_Name()
FSharp.Compiler.AbstractIL.IL+ILVersionInfo: Boolean Equals(ILVersionInfo)
FSharp.Compiler.AbstractIL.IL+ILVersionInfo: Boolean Equals(System.Object)
FSharp.Compiler.AbstractIL.IL+ILVersionInfo: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILVersionInfo: Int32 CompareTo(ILVersionInfo)
FSharp.Compiler.AbstractIL.IL+ILVersionInfo: Int32 CompareTo(System.Object)
FSharp.Compiler.AbstractIL.IL+ILVersionInfo: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.AbstractIL.IL+ILVersionInfo: Int32 GetHashCode()
FSharp.Compiler.AbstractIL.IL+ILVersionInfo: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+ILVersionInfo: System.String ToString()
FSharp.Compiler.AbstractIL.IL+ILVersionInfo: UInt16 Build
FSharp.Compiler.AbstractIL.IL+ILVersionInfo: UInt16 Major
FSharp.Compiler.AbstractIL.IL+ILVersionInfo: UInt16 Minor
FSharp.Compiler.AbstractIL.IL+ILVersionInfo: UInt16 Revision
FSharp.Compiler.AbstractIL.IL+ILVersionInfo: UInt16 get_Build()
FSharp.Compiler.AbstractIL.IL+ILVersionInfo: UInt16 get_Major()
FSharp.Compiler.AbstractIL.IL+ILVersionInfo: UInt16 get_Minor()
FSharp.Compiler.AbstractIL.IL+ILVersionInfo: UInt16 get_Revision()
FSharp.Compiler.AbstractIL.IL+ILVersionInfo: Void .ctor(UInt16, UInt16, UInt16, UInt16)
FSharp.Compiler.AbstractIL.IL+PublicKey+PublicKey: Byte[] Item
FSharp.Compiler.AbstractIL.IL+PublicKey+PublicKey: Byte[] get_Item()
FSharp.Compiler.AbstractIL.IL+PublicKey+PublicKeyToken: Byte[] Item
FSharp.Compiler.AbstractIL.IL+PublicKey+PublicKeyToken: Byte[] get_Item()
FSharp.Compiler.AbstractIL.IL+PublicKey+Tags: Int32 PublicKey
FSharp.Compiler.AbstractIL.IL+PublicKey+Tags: Int32 PublicKeyToken
FSharp.Compiler.AbstractIL.IL+PublicKey: Boolean Equals(PublicKey)
FSharp.Compiler.AbstractIL.IL+PublicKey: Boolean Equals(System.Object)
FSharp.Compiler.AbstractIL.IL+PublicKey: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+PublicKey: Boolean IsKey
FSharp.Compiler.AbstractIL.IL+PublicKey: Boolean IsKeyToken
FSharp.Compiler.AbstractIL.IL+PublicKey: Boolean IsPublicKey
FSharp.Compiler.AbstractIL.IL+PublicKey: Boolean IsPublicKeyToken
FSharp.Compiler.AbstractIL.IL+PublicKey: Boolean get_IsKey()
FSharp.Compiler.AbstractIL.IL+PublicKey: Boolean get_IsKeyToken()
FSharp.Compiler.AbstractIL.IL+PublicKey: Boolean get_IsPublicKey()
FSharp.Compiler.AbstractIL.IL+PublicKey: Boolean get_IsPublicKeyToken()
FSharp.Compiler.AbstractIL.IL+PublicKey: Byte[] Key
FSharp.Compiler.AbstractIL.IL+PublicKey: Byte[] KeyToken
FSharp.Compiler.AbstractIL.IL+PublicKey: Byte[] get_Key()
FSharp.Compiler.AbstractIL.IL+PublicKey: Byte[] get_KeyToken()
FSharp.Compiler.AbstractIL.IL+PublicKey: FSharp.Compiler.AbstractIL.IL+PublicKey+PublicKey
FSharp.Compiler.AbstractIL.IL+PublicKey: FSharp.Compiler.AbstractIL.IL+PublicKey+PublicKeyToken
FSharp.Compiler.AbstractIL.IL+PublicKey: FSharp.Compiler.AbstractIL.IL+PublicKey+Tags
FSharp.Compiler.AbstractIL.IL+PublicKey: Int32 CompareTo(PublicKey)
FSharp.Compiler.AbstractIL.IL+PublicKey: Int32 CompareTo(System.Object)
FSharp.Compiler.AbstractIL.IL+PublicKey: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.AbstractIL.IL+PublicKey: Int32 GetHashCode()
FSharp.Compiler.AbstractIL.IL+PublicKey: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.AbstractIL.IL+PublicKey: Int32 Tag
FSharp.Compiler.AbstractIL.IL+PublicKey: Int32 get_Tag()
FSharp.Compiler.AbstractIL.IL+PublicKey: PublicKey KeyAsToken(Byte[])
FSharp.Compiler.AbstractIL.IL+PublicKey: PublicKey NewPublicKey(Byte[])
FSharp.Compiler.AbstractIL.IL+PublicKey: PublicKey NewPublicKeyToken(Byte[])
FSharp.Compiler.AbstractIL.IL+PublicKey: System.String ToString()
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILArrayShape
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILAssemblyLongevity
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILAssemblyManifest
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILAssemblyRef
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILAttribElem
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILAttribute
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILAttributes
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILAttributesStored
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILCallingConv
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILCallingSignature
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILDefaultPInvokeEncoding
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILEventDef
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILEventDefs
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILExportedTypeOrForwarder
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILExportedTypesAndForwarders
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILFieldDef
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILFieldDefs
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILFieldRef
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILFieldSpec
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILGenericParameterDef
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILGenericVariance
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILMemberAccess
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILMethodDef
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILMethodDefs
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILMethodImplDef
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILMethodImplDefs
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILMethodRef
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILMethodSpec
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILModuleDef
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILModuleRef
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILNativeResource
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILNativeType
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILNestedExportedType
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILNestedExportedTypes
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILParameter
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILPlatform
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILPreTypeDef
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILPropertyDef
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILPropertyDefs
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILResources
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILReturn
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILScopeRef
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILSecurityDeclsStored
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILSourceDocument
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILType
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILTypeDef
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILTypeDefAccess
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILTypeDefKind
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILTypeDefLayout
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILTypeDefs
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILTypeInit
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILTypeRef
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILTypeSpec
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+ILVersionInfo
FSharp.Compiler.AbstractIL.IL: FSharp.Compiler.AbstractIL.IL+PublicKey
FSharp.Compiler.AbstractIL.IL: ILAttributes emptyILCustomAttrs
FSharp.Compiler.AbstractIL.IL: ILAttributes get_emptyILCustomAttrs()
FSharp.Compiler.AbstractIL.IL: ILAttributes mkILCustomAttrsFromArray(ILAttribute[])
FSharp.Compiler.AbstractIL.IL: ILExportedTypesAndForwarders mkILExportedTypes(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILExportedTypeOrForwarder])
FSharp.Compiler.AbstractIL.IL: ILNestedExportedTypes mkILNestedExportedTypes(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILNestedExportedType])
FSharp.Compiler.AbstractIL.IL: ILResources emptyILResources
FSharp.Compiler.AbstractIL.IL: ILResources get_emptyILResources()
FSharp.Compiler.AbstractIL.IL: ILSecurityDecls emptyILSecurityDecls
FSharp.Compiler.AbstractIL.IL: ILSecurityDecls get_emptyILSecurityDecls()
FSharp.Compiler.AbstractIL.IL: ILTypeDefs mkILTypeDefsComputed(Microsoft.FSharp.Core.FSharpFunc`2[Microsoft.FSharp.Core.Unit,FSharp.Compiler.AbstractIL.IL+ILPreTypeDef[]])
FSharp.Compiler.AbstractIL.ILBinaryReader
FSharp.Compiler.AbstractIL.ILBinaryReader+ILModuleReader: ILModuleDef ILModuleDef
FSharp.Compiler.AbstractIL.ILBinaryReader+ILModuleReader: ILModuleDef get_ILModuleDef()
FSharp.Compiler.AbstractIL.ILBinaryReader+ILModuleReader: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILAssemblyRef] ILAssemblyRefs
FSharp.Compiler.AbstractIL.ILBinaryReader+ILModuleReader: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILAssemblyRef] get_ILAssemblyRefs()
FSharp.Compiler.AbstractIL.ILBinaryReader+Shim+IAssemblyReader: ILModuleReader GetILModuleReader(System.String, ILReaderOptions)
FSharp.Compiler.AbstractIL.ILBinaryReader+Shim: FSharp.Compiler.AbstractIL.ILBinaryReader+Shim+IAssemblyReader
FSharp.Compiler.AbstractIL.ILBinaryReader+Shim: IAssemblyReader AssemblyReader
FSharp.Compiler.AbstractIL.ILBinaryReader+Shim: IAssemblyReader get_AssemblyReader()
FSharp.Compiler.AbstractIL.ILBinaryReader+Shim: Void set_AssemblyReader(IAssemblyReader)
FSharp.Compiler.AbstractIL.ILBinaryReader: FSharp.Compiler.AbstractIL.ILBinaryReader+ILModuleReader
FSharp.Compiler.AbstractIL.ILBinaryReader: FSharp.Compiler.AbstractIL.ILBinaryReader+Shim
FSharp.Compiler.CodeAnalysis.BasicPatterns
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpExpr] |AddressOf|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpExpr] |Quote|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue] |Value|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpType] |BaseValue|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpType] |DefaultValue|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpType] |ThisValue|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Int32] |WitnessArg|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.CodeAnalysis.FSharpExpr,FSharp.Compiler.CodeAnalysis.FSharpExpr]] |AddressSet|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.CodeAnalysis.FSharpExpr,FSharp.Compiler.CodeAnalysis.FSharpExpr]] |Sequential|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.CodeAnalysis.FSharpExpr,FSharp.Compiler.CodeAnalysis.FSharpExpr]] |TryFinally|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.CodeAnalysis.FSharpExpr,FSharp.Compiler.CodeAnalysis.FSharpExpr]] |WhileLoop|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.CodeAnalysis.FSharpExpr,FSharp.Compiler.CodeAnalysis.FSharpType]] |UnionCaseTag|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.CodeAnalysis.FSharpExpr,Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue],FSharp.Compiler.CodeAnalysis.FSharpExpr]]]] |DecisionTree|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue,FSharp.Compiler.CodeAnalysis.FSharpExpr]] |Lambda|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue,FSharp.Compiler.CodeAnalysis.FSharpExpr]] |ValueSet|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.CodeAnalysis.FSharpType,FSharp.Compiler.CodeAnalysis.FSharpExpr]] |Coerce|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.CodeAnalysis.FSharpType,FSharp.Compiler.CodeAnalysis.FSharpExpr]] |NewDelegate|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.CodeAnalysis.FSharpType,FSharp.Compiler.CodeAnalysis.FSharpExpr]] |TypeTest|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.CodeAnalysis.FSharpType,Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpExpr]]] |NewAnonRecord|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.CodeAnalysis.FSharpType,Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpExpr]]] |NewArray|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.CodeAnalysis.FSharpType,Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpExpr]]] |NewRecord|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.CodeAnalysis.FSharpType,Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpExpr]]] |NewTuple|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpGenericParameter],FSharp.Compiler.CodeAnalysis.FSharpExpr]] |TypeLambda|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue,FSharp.Compiler.CodeAnalysis.FSharpExpr]],FSharp.Compiler.CodeAnalysis.FSharpExpr]] |LetRec|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[System.Int32,Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpExpr]]] |DecisionTreeSuccess|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[System.Object,FSharp.Compiler.CodeAnalysis.FSharpType]] |Const|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[System.Tuple`2[FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue,FSharp.Compiler.CodeAnalysis.FSharpExpr],FSharp.Compiler.CodeAnalysis.FSharpExpr]] |Let|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`3[FSharp.Compiler.CodeAnalysis.FSharpExpr,FSharp.Compiler.CodeAnalysis.FSharpExpr,FSharp.Compiler.CodeAnalysis.FSharpExpr]] |IfThenElse|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`3[FSharp.Compiler.CodeAnalysis.FSharpExpr,FSharp.Compiler.CodeAnalysis.FSharpType,FSharp.Compiler.CodeAnalysis.FSharpUnionCase]] |UnionCaseTest|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`3[FSharp.Compiler.CodeAnalysis.FSharpExpr,FSharp.Compiler.CodeAnalysis.FSharpType,System.Int32]] |AnonRecordGet|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`3[FSharp.Compiler.CodeAnalysis.FSharpExpr,Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpType],Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpExpr]]] |Application|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`3[FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue,Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpType],Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpExpr]]] |NewObject|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`3[FSharp.Compiler.CodeAnalysis.FSharpType,FSharp.Compiler.CodeAnalysis.FSharpUnionCase,Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpExpr]]] |NewUnionCase|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`3[FSharp.Compiler.CodeAnalysis.FSharpType,System.Int32,FSharp.Compiler.CodeAnalysis.FSharpExpr]] |TupleGet|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`3[Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpExpr],FSharp.Compiler.CodeAnalysis.FSharpType,FSharp.Compiler.CodeAnalysis.FSharpField]] |FSharpFieldGet|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`3[Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpExpr],FSharp.Compiler.CodeAnalysis.FSharpType,System.String]] |ILFieldGet|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`3[System.String,Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpType],Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpExpr]]] |ILAsm|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`4[FSharp.Compiler.CodeAnalysis.FSharpExpr,FSharp.Compiler.CodeAnalysis.FSharpExpr,FSharp.Compiler.CodeAnalysis.FSharpExpr,System.Boolean]] |FastIntegerForLoop|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`4[FSharp.Compiler.CodeAnalysis.FSharpExpr,FSharp.Compiler.CodeAnalysis.FSharpType,FSharp.Compiler.CodeAnalysis.FSharpUnionCase,FSharp.Compiler.CodeAnalysis.FSharpField]] |UnionCaseGet|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`4[FSharp.Compiler.CodeAnalysis.FSharpType,FSharp.Compiler.CodeAnalysis.FSharpExpr,Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpObjectExprOverride],Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[FSharp.Compiler.CodeAnalysis.FSharpType,Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpObjectExprOverride]]]]] |ObjectExpr|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`4[Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpExpr],FSharp.Compiler.CodeAnalysis.FSharpType,FSharp.Compiler.CodeAnalysis.FSharpField,FSharp.Compiler.CodeAnalysis.FSharpExpr]] |FSharpFieldSet|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`4[Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpExpr],FSharp.Compiler.CodeAnalysis.FSharpType,System.String,FSharp.Compiler.CodeAnalysis.FSharpExpr]] |ILFieldSet|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`5[FSharp.Compiler.CodeAnalysis.FSharpExpr,FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue,FSharp.Compiler.CodeAnalysis.FSharpExpr,FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue,FSharp.Compiler.CodeAnalysis.FSharpExpr]] |TryWith|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`5[FSharp.Compiler.CodeAnalysis.FSharpExpr,FSharp.Compiler.CodeAnalysis.FSharpType,FSharp.Compiler.CodeAnalysis.FSharpUnionCase,FSharp.Compiler.CodeAnalysis.FSharpField,FSharp.Compiler.CodeAnalysis.FSharpExpr]] |UnionCaseSet|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`5[Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpExpr],FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue,Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpType],Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpType],Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpExpr]]] |Call|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`6[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpType],System.String,FSharp.Compiler.Syntax.MemberFlags,Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpType],Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpType],Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpExpr]]] |TraitCall|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`6[Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpExpr],FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue,Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpType],Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpType],Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpExpr],Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpExpr]]] |CallWithWitnesses|_|(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.CompilerEnvironment
FSharp.Compiler.CodeAnalysis.CompilerEnvironment: Microsoft.FSharp.Core.FSharpOption`1[System.String] BinFolderOfDefaultFSharpCompiler(Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.CodeAnalysis.CompilerEnvironmentModule
FSharp.Compiler.CodeAnalysis.CompilerEnvironmentModule: Boolean IsCheckerSupportedSubcategory(System.String)
FSharp.Compiler.CodeAnalysis.CompilerEnvironmentModule: Microsoft.FSharp.Collections.FSharpList`1[System.String] DefaultReferencesForOrphanSources(Boolean)
FSharp.Compiler.CodeAnalysis.CompilerEnvironmentModule: Microsoft.FSharp.Collections.FSharpList`1[System.String] GetCompilationDefinesForEditing(FSharp.Compiler.CodeAnalysis.FSharpParsingOptions)
FSharp.Compiler.CodeAnalysis.DebuggerEnvironment
FSharp.Compiler.CodeAnalysis.DebuggerEnvironment: System.Guid GetLanguageID()
FSharp.Compiler.CodeAnalysis.FSharpAbstractParameter
FSharp.Compiler.CodeAnalysis.FSharpAbstractParameter: Boolean IsInArg
FSharp.Compiler.CodeAnalysis.FSharpAbstractParameter: Boolean IsOptionalArg
FSharp.Compiler.CodeAnalysis.FSharpAbstractParameter: Boolean IsOutArg
FSharp.Compiler.CodeAnalysis.FSharpAbstractParameter: Boolean get_IsInArg()
FSharp.Compiler.CodeAnalysis.FSharpAbstractParameter: Boolean get_IsOptionalArg()
FSharp.Compiler.CodeAnalysis.FSharpAbstractParameter: Boolean get_IsOutArg()
FSharp.Compiler.CodeAnalysis.FSharpAbstractParameter: FSharp.Compiler.CodeAnalysis.FSharpType Type
FSharp.Compiler.CodeAnalysis.FSharpAbstractParameter: FSharp.Compiler.CodeAnalysis.FSharpType get_Type()
FSharp.Compiler.CodeAnalysis.FSharpAbstractParameter: Microsoft.FSharp.Core.FSharpOption`1[System.String] Name
FSharp.Compiler.CodeAnalysis.FSharpAbstractParameter: Microsoft.FSharp.Core.FSharpOption`1[System.String] get_Name()
FSharp.Compiler.CodeAnalysis.FSharpAbstractParameter: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpAttribute] Attributes
FSharp.Compiler.CodeAnalysis.FSharpAbstractParameter: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpAttribute] get_Attributes()
FSharp.Compiler.CodeAnalysis.FSharpAbstractSignature
FSharp.Compiler.CodeAnalysis.FSharpAbstractSignature: FSharp.Compiler.CodeAnalysis.FSharpType AbstractReturnType
FSharp.Compiler.CodeAnalysis.FSharpAbstractSignature: FSharp.Compiler.CodeAnalysis.FSharpType DeclaringType
FSharp.Compiler.CodeAnalysis.FSharpAbstractSignature: FSharp.Compiler.CodeAnalysis.FSharpType get_AbstractReturnType()
FSharp.Compiler.CodeAnalysis.FSharpAbstractSignature: FSharp.Compiler.CodeAnalysis.FSharpType get_DeclaringType()
FSharp.Compiler.CodeAnalysis.FSharpAbstractSignature: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpGenericParameter] DeclaringTypeGenericParameters
FSharp.Compiler.CodeAnalysis.FSharpAbstractSignature: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpGenericParameter] MethodGenericParameters
FSharp.Compiler.CodeAnalysis.FSharpAbstractSignature: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpGenericParameter] get_DeclaringTypeGenericParameters()
FSharp.Compiler.CodeAnalysis.FSharpAbstractSignature: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpGenericParameter] get_MethodGenericParameters()
FSharp.Compiler.CodeAnalysis.FSharpAbstractSignature: System.Collections.Generic.IList`1[System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpAbstractParameter]] AbstractArguments
FSharp.Compiler.CodeAnalysis.FSharpAbstractSignature: System.Collections.Generic.IList`1[System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpAbstractParameter]] get_AbstractArguments()
FSharp.Compiler.CodeAnalysis.FSharpAbstractSignature: System.String Name
FSharp.Compiler.CodeAnalysis.FSharpAbstractSignature: System.String get_Name()
FSharp.Compiler.CodeAnalysis.FSharpAccessibility
FSharp.Compiler.CodeAnalysis.FSharpAccessibility: Boolean IsInternal
FSharp.Compiler.CodeAnalysis.FSharpAccessibility: Boolean IsPrivate
FSharp.Compiler.CodeAnalysis.FSharpAccessibility: Boolean IsProtected
FSharp.Compiler.CodeAnalysis.FSharpAccessibility: Boolean IsPublic
FSharp.Compiler.CodeAnalysis.FSharpAccessibility: Boolean get_IsInternal()
FSharp.Compiler.CodeAnalysis.FSharpAccessibility: Boolean get_IsPrivate()
FSharp.Compiler.CodeAnalysis.FSharpAccessibility: Boolean get_IsProtected()
FSharp.Compiler.CodeAnalysis.FSharpAccessibility: Boolean get_IsPublic()
FSharp.Compiler.CodeAnalysis.FSharpAccessibility: System.String ToString()
FSharp.Compiler.CodeAnalysis.FSharpAccessibilityRights
FSharp.Compiler.CodeAnalysis.FSharpActivePatternCase
FSharp.Compiler.CodeAnalysis.FSharpActivePatternCase: FSharp.Compiler.CodeAnalysis.FSharpActivePatternGroup Group
FSharp.Compiler.CodeAnalysis.FSharpActivePatternCase: FSharp.Compiler.CodeAnalysis.FSharpActivePatternGroup get_Group()
FSharp.Compiler.CodeAnalysis.FSharpActivePatternCase: FSharp.Compiler.Text.Range DeclarationLocation
FSharp.Compiler.CodeAnalysis.FSharpActivePatternCase: FSharp.Compiler.Text.Range get_DeclarationLocation()
FSharp.Compiler.CodeAnalysis.FSharpActivePatternCase: Int32 Index
FSharp.Compiler.CodeAnalysis.FSharpActivePatternCase: Int32 get_Index()
FSharp.Compiler.CodeAnalysis.FSharpActivePatternCase: System.Collections.Generic.IList`1[System.String] ElaboratedXmlDoc
FSharp.Compiler.CodeAnalysis.FSharpActivePatternCase: System.Collections.Generic.IList`1[System.String] XmlDoc
FSharp.Compiler.CodeAnalysis.FSharpActivePatternCase: System.Collections.Generic.IList`1[System.String] get_ElaboratedXmlDoc()
FSharp.Compiler.CodeAnalysis.FSharpActivePatternCase: System.Collections.Generic.IList`1[System.String] get_XmlDoc()
FSharp.Compiler.CodeAnalysis.FSharpActivePatternCase: System.String Name
FSharp.Compiler.CodeAnalysis.FSharpActivePatternCase: System.String XmlDocSig
FSharp.Compiler.CodeAnalysis.FSharpActivePatternCase: System.String get_Name()
FSharp.Compiler.CodeAnalysis.FSharpActivePatternCase: System.String get_XmlDocSig()
FSharp.Compiler.CodeAnalysis.FSharpActivePatternGroup
FSharp.Compiler.CodeAnalysis.FSharpActivePatternGroup: Boolean IsTotal
FSharp.Compiler.CodeAnalysis.FSharpActivePatternGroup: Boolean get_IsTotal()
FSharp.Compiler.CodeAnalysis.FSharpActivePatternGroup: FSharp.Compiler.CodeAnalysis.FSharpType OverallType
FSharp.Compiler.CodeAnalysis.FSharpActivePatternGroup: FSharp.Compiler.CodeAnalysis.FSharpType get_OverallType()
FSharp.Compiler.CodeAnalysis.FSharpActivePatternGroup: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpEntity] DeclaringEntity
FSharp.Compiler.CodeAnalysis.FSharpActivePatternGroup: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpEntity] get_DeclaringEntity()
FSharp.Compiler.CodeAnalysis.FSharpActivePatternGroup: Microsoft.FSharp.Core.FSharpOption`1[System.String] Name
FSharp.Compiler.CodeAnalysis.FSharpActivePatternGroup: Microsoft.FSharp.Core.FSharpOption`1[System.String] get_Name()
FSharp.Compiler.CodeAnalysis.FSharpActivePatternGroup: System.Collections.Generic.IList`1[System.String] Names
FSharp.Compiler.CodeAnalysis.FSharpActivePatternGroup: System.Collections.Generic.IList`1[System.String] get_Names()
FSharp.Compiler.CodeAnalysis.FSharpAnonRecordTypeDetails
FSharp.Compiler.CodeAnalysis.FSharpAnonRecordTypeDetails: FSharp.Compiler.CodeAnalysis.FSharpAssembly Assembly
FSharp.Compiler.CodeAnalysis.FSharpAnonRecordTypeDetails: FSharp.Compiler.CodeAnalysis.FSharpAssembly get_Assembly()
FSharp.Compiler.CodeAnalysis.FSharpAnonRecordTypeDetails: Microsoft.FSharp.Collections.FSharpList`1[System.String] EnclosingCompiledTypeNames
FSharp.Compiler.CodeAnalysis.FSharpAnonRecordTypeDetails: Microsoft.FSharp.Collections.FSharpList`1[System.String] get_EnclosingCompiledTypeNames()
FSharp.Compiler.CodeAnalysis.FSharpAnonRecordTypeDetails: System.String CompiledName
FSharp.Compiler.CodeAnalysis.FSharpAnonRecordTypeDetails: System.String get_CompiledName()
FSharp.Compiler.CodeAnalysis.FSharpAnonRecordTypeDetails: System.String[] SortedFieldNames
FSharp.Compiler.CodeAnalysis.FSharpAnonRecordTypeDetails: System.String[] get_SortedFieldNames()
FSharp.Compiler.CodeAnalysis.FSharpAssembly
FSharp.Compiler.CodeAnalysis.FSharpAssembly: Boolean IsProviderGenerated
FSharp.Compiler.CodeAnalysis.FSharpAssembly: Boolean get_IsProviderGenerated()
FSharp.Compiler.CodeAnalysis.FSharpAssembly: FSharp.Compiler.CodeAnalysis.FSharpAssemblySignature Contents
FSharp.Compiler.CodeAnalysis.FSharpAssembly: FSharp.Compiler.CodeAnalysis.FSharpAssemblySignature get_Contents()
FSharp.Compiler.CodeAnalysis.FSharpAssembly: Microsoft.FSharp.Core.FSharpOption`1[System.String] FileName
FSharp.Compiler.CodeAnalysis.FSharpAssembly: Microsoft.FSharp.Core.FSharpOption`1[System.String] get_FileName()
FSharp.Compiler.CodeAnalysis.FSharpAssembly: System.String CodeLocation
FSharp.Compiler.CodeAnalysis.FSharpAssembly: System.String QualifiedName
FSharp.Compiler.CodeAnalysis.FSharpAssembly: System.String SimpleName
FSharp.Compiler.CodeAnalysis.FSharpAssembly: System.String ToString()
FSharp.Compiler.CodeAnalysis.FSharpAssembly: System.String get_CodeLocation()
FSharp.Compiler.CodeAnalysis.FSharpAssembly: System.String get_QualifiedName()
FSharp.Compiler.CodeAnalysis.FSharpAssembly: System.String get_SimpleName()
FSharp.Compiler.CodeAnalysis.FSharpAssemblyContents
FSharp.Compiler.CodeAnalysis.FSharpAssemblyContents: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpImplementationFileContents] ImplementationFiles
FSharp.Compiler.CodeAnalysis.FSharpAssemblyContents: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpImplementationFileContents] get_ImplementationFiles()
FSharp.Compiler.CodeAnalysis.FSharpAssemblySignature
FSharp.Compiler.CodeAnalysis.FSharpAssemblySignature: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpEntity] FindEntityByPath(Microsoft.FSharp.Collections.FSharpList`1[System.String])
FSharp.Compiler.CodeAnalysis.FSharpAssemblySignature: System.Collections.Generic.IEnumerable`1[FSharp.Compiler.CodeAnalysis.FSharpEntity] TryGetEntities()
FSharp.Compiler.CodeAnalysis.FSharpAssemblySignature: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpAttribute] Attributes
FSharp.Compiler.CodeAnalysis.FSharpAssemblySignature: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpAttribute] get_Attributes()
FSharp.Compiler.CodeAnalysis.FSharpAssemblySignature: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpEntity] Entities
FSharp.Compiler.CodeAnalysis.FSharpAssemblySignature: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpEntity] get_Entities()
FSharp.Compiler.CodeAnalysis.FSharpAssemblySignature: System.String ToString()
FSharp.Compiler.CodeAnalysis.FSharpAttribute
FSharp.Compiler.CodeAnalysis.FSharpAttribute: Boolean IsUnresolved
FSharp.Compiler.CodeAnalysis.FSharpAttribute: Boolean get_IsUnresolved()
FSharp.Compiler.CodeAnalysis.FSharpAttribute: FSharp.Compiler.CodeAnalysis.FSharpEntity AttributeType
FSharp.Compiler.CodeAnalysis.FSharpAttribute: FSharp.Compiler.CodeAnalysis.FSharpEntity get_AttributeType()
FSharp.Compiler.CodeAnalysis.FSharpAttribute: FSharp.Compiler.Text.Range Range
FSharp.Compiler.CodeAnalysis.FSharpAttribute: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.CodeAnalysis.FSharpAttribute: System.Collections.Generic.IList`1[System.Tuple`2[FSharp.Compiler.CodeAnalysis.FSharpType,System.Object]] ConstructorArguments
FSharp.Compiler.CodeAnalysis.FSharpAttribute: System.Collections.Generic.IList`1[System.Tuple`2[FSharp.Compiler.CodeAnalysis.FSharpType,System.Object]] get_ConstructorArguments()
FSharp.Compiler.CodeAnalysis.FSharpAttribute: System.Collections.Generic.IList`1[System.Tuple`4[FSharp.Compiler.CodeAnalysis.FSharpType,System.String,System.Boolean,System.Object]] NamedArguments
FSharp.Compiler.CodeAnalysis.FSharpAttribute: System.Collections.Generic.IList`1[System.Tuple`4[FSharp.Compiler.CodeAnalysis.FSharpType,System.String,System.Boolean,System.Object]] get_NamedArguments()
FSharp.Compiler.CodeAnalysis.FSharpAttribute: System.String Format(FSharp.Compiler.CodeAnalysis.FSharpDisplayContext)
FSharp.Compiler.CodeAnalysis.FSharpAttribute: System.String ToString()
FSharp.Compiler.CodeAnalysis.FSharpCheckFileAnswer
FSharp.Compiler.CodeAnalysis.FSharpCheckFileAnswer+Succeeded: FSharp.Compiler.CodeAnalysis.FSharpCheckFileResults Item
FSharp.Compiler.CodeAnalysis.FSharpCheckFileAnswer+Succeeded: FSharp.Compiler.CodeAnalysis.FSharpCheckFileResults get_Item()
FSharp.Compiler.CodeAnalysis.FSharpCheckFileAnswer+Tags: Int32 Aborted
FSharp.Compiler.CodeAnalysis.FSharpCheckFileAnswer+Tags: Int32 Succeeded
FSharp.Compiler.CodeAnalysis.FSharpCheckFileAnswer: Boolean Equals(FSharp.Compiler.CodeAnalysis.FSharpCheckFileAnswer)
FSharp.Compiler.CodeAnalysis.FSharpCheckFileAnswer: Boolean Equals(System.Object)
FSharp.Compiler.CodeAnalysis.FSharpCheckFileAnswer: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.CodeAnalysis.FSharpCheckFileAnswer: Boolean IsAborted
FSharp.Compiler.CodeAnalysis.FSharpCheckFileAnswer: Boolean IsSucceeded
FSharp.Compiler.CodeAnalysis.FSharpCheckFileAnswer: Boolean get_IsAborted()
FSharp.Compiler.CodeAnalysis.FSharpCheckFileAnswer: Boolean get_IsSucceeded()
FSharp.Compiler.CodeAnalysis.FSharpCheckFileAnswer: FSharp.Compiler.CodeAnalysis.FSharpCheckFileAnswer Aborted
FSharp.Compiler.CodeAnalysis.FSharpCheckFileAnswer: FSharp.Compiler.CodeAnalysis.FSharpCheckFileAnswer NewSucceeded(FSharp.Compiler.CodeAnalysis.FSharpCheckFileResults)
FSharp.Compiler.CodeAnalysis.FSharpCheckFileAnswer: FSharp.Compiler.CodeAnalysis.FSharpCheckFileAnswer get_Aborted()
FSharp.Compiler.CodeAnalysis.FSharpCheckFileAnswer: FSharp.Compiler.CodeAnalysis.FSharpCheckFileAnswer+Succeeded
FSharp.Compiler.CodeAnalysis.FSharpCheckFileAnswer: FSharp.Compiler.CodeAnalysis.FSharpCheckFileAnswer+Tags
FSharp.Compiler.CodeAnalysis.FSharpCheckFileAnswer: Int32 GetHashCode()
FSharp.Compiler.CodeAnalysis.FSharpCheckFileAnswer: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.CodeAnalysis.FSharpCheckFileAnswer: Int32 Tag
FSharp.Compiler.CodeAnalysis.FSharpCheckFileAnswer: Int32 get_Tag()
FSharp.Compiler.CodeAnalysis.FSharpCheckFileAnswer: System.String ToString()
FSharp.Compiler.CodeAnalysis.FSharpCheckFileResults
FSharp.Compiler.CodeAnalysis.FSharpCheckFileResults: Boolean HasFullTypeCheckInfo
FSharp.Compiler.CodeAnalysis.FSharpCheckFileResults: Boolean IsRelativeNameResolvableFromSymbol(FSharp.Compiler.Text.Position, Microsoft.FSharp.Collections.FSharpList`1[System.String], FSharp.Compiler.CodeAnalysis.FSharpSymbol)
FSharp.Compiler.CodeAnalysis.FSharpCheckFileResults: Boolean get_HasFullTypeCheckInfo()
FSharp.Compiler.CodeAnalysis.FSharpCheckFileResults: FSharp.Compiler.CodeAnalysis.FSharpAssemblySignature PartialAssemblySignature
FSharp.Compiler.CodeAnalysis.FSharpCheckFileResults: FSharp.Compiler.CodeAnalysis.FSharpAssemblySignature get_PartialAssemblySignature()
FSharp.Compiler.CodeAnalysis.FSharpCheckFileResults: FSharp.Compiler.CodeAnalysis.FindDeclResult GetDeclarationLocation(Int32, Int32, System.String, Microsoft.FSharp.Collections.FSharpList`1[System.String], Microsoft.FSharp.Core.FSharpOption`1[System.Boolean])
FSharp.Compiler.CodeAnalysis.FSharpCheckFileResults: FSharp.Compiler.CodeAnalysis.FSharpOpenDeclaration[] OpenDeclarations
FSharp.Compiler.CodeAnalysis.FSharpCheckFileResults: FSharp.Compiler.CodeAnalysis.FSharpOpenDeclaration[] get_OpenDeclarations()
FSharp.Compiler.CodeAnalysis.FSharpCheckFileResults: FSharp.Compiler.CodeAnalysis.FSharpProjectContext ProjectContext
FSharp.Compiler.CodeAnalysis.FSharpCheckFileResults: FSharp.Compiler.CodeAnalysis.FSharpProjectContext get_ProjectContext()
FSharp.Compiler.CodeAnalysis.FSharpCheckFileResults: FSharp.Compiler.CodeAnalysis.SemanticClassificationItem[] GetSemanticClassification(Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range])
FSharp.Compiler.CodeAnalysis.FSharpCheckFileResults: FSharp.Compiler.CodeAnalysis.FSharpSymbolUse[] GetUsesOfSymbolInFile(FSharp.Compiler.CodeAnalysis.FSharpSymbol, Microsoft.FSharp.Core.FSharpOption`1[System.Threading.CancellationToken])
FSharp.Compiler.CodeAnalysis.FSharpCheckFileResults: FSharp.Compiler.Diagnostics.FSharpDiagnostic[] Diagnostics
FSharp.Compiler.CodeAnalysis.FSharpCheckFileResults: FSharp.Compiler.Diagnostics.FSharpDiagnostic[] get_Diagnostics()
FSharp.Compiler.CodeAnalysis.FSharpCheckFileResults: FSharp.Compiler.EditorServices.DeclarationListInfo GetDeclarationListInfo(Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpParseFileResults], Int32, System.String, FSharp.Compiler.EditorServices.PartialLongName, Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.FSharpFunc`2[Microsoft.FSharp.Core.Unit,Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.EditorServices.AssemblySymbol]]])
FSharp.Compiler.CodeAnalysis.FSharpCheckFileResults: FSharp.Compiler.EditorServices.MethodGroup GetMethods(Int32, Int32, System.String, Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Collections.FSharpList`1[System.String]])
FSharp.Compiler.CodeAnalysis.FSharpCheckFileResults: FSharp.Compiler.EditorServices.FSharpToolTipText`1[FSharp.Compiler.TextLayout.Layout] GetStructuredToolTipText(Int32, Int32, System.String, Microsoft.FSharp.Collections.FSharpList`1[System.String], Int32)
FSharp.Compiler.CodeAnalysis.FSharpCheckFileResults: FSharp.Compiler.EditorServices.FSharpToolTipText`1[System.String] GetToolTipText(Int32, Int32, System.String, Microsoft.FSharp.Collections.FSharpList`1[System.String], Int32)
FSharp.Compiler.CodeAnalysis.FSharpCheckFileResults: FSharp.Compiler.Text.Range[] GetFormatSpecifierLocations()
FSharp.Compiler.CodeAnalysis.FSharpCheckFileResults: Microsoft.FSharp.Collections.FSharpList`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpSymbolUse]] GetDeclarationListSymbols(Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpParseFileResults], Int32, System.String, FSharp.Compiler.EditorServices.PartialLongName, Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.FSharpFunc`2[Microsoft.FSharp.Core.Unit,Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.EditorServices.AssemblySymbol]]])
FSharp.Compiler.CodeAnalysis.FSharpCheckFileResults: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpDisplayContext] GetDisplayContextForPos(FSharp.Compiler.Text.Position)
FSharp.Compiler.CodeAnalysis.FSharpCheckFileResults: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpImplementationFileContents] ImplementationFile
FSharp.Compiler.CodeAnalysis.FSharpCheckFileResults: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpImplementationFileContents] get_ImplementationFile()
FSharp.Compiler.CodeAnalysis.FSharpCheckFileResults: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpSymbolUse] GetSymbolUseAtLocation(Int32, Int32, System.String, Microsoft.FSharp.Collections.FSharpList`1[System.String])
FSharp.Compiler.CodeAnalysis.FSharpCheckFileResults: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpSymbolUse]] GetMethodsAsSymbols(Int32, Int32, System.String, Microsoft.FSharp.Collections.FSharpList`1[System.String])
FSharp.Compiler.CodeAnalysis.FSharpCheckFileResults: Microsoft.FSharp.Core.FSharpOption`1[System.String] GetF1Keyword(Int32, Int32, System.String, Microsoft.FSharp.Collections.FSharpList`1[System.String])
FSharp.Compiler.CodeAnalysis.FSharpCheckFileResults: System.Collections.Generic.IEnumerable`1[FSharp.Compiler.CodeAnalysis.FSharpSymbolUse] GetAllUsesOfAllSymbolsInFile(Microsoft.FSharp.Core.FSharpOption`1[System.Threading.CancellationToken])
FSharp.Compiler.CodeAnalysis.FSharpCheckFileResults: System.String ToString()
FSharp.Compiler.CodeAnalysis.FSharpCheckFileResults: System.String[] DependencyFiles
FSharp.Compiler.CodeAnalysis.FSharpCheckFileResults: System.String[] get_DependencyFiles()
FSharp.Compiler.CodeAnalysis.FSharpCheckFileResults: System.Tuple`2[FSharp.Compiler.Text.Range,System.Int32][] GetFormatSpecifierLocationsAndArity()
FSharp.Compiler.CodeAnalysis.FSharpCheckProjectResults
FSharp.Compiler.CodeAnalysis.FSharpCheckProjectResults: Boolean HasCriticalErrors
FSharp.Compiler.CodeAnalysis.FSharpCheckProjectResults: Boolean get_HasCriticalErrors()
FSharp.Compiler.CodeAnalysis.FSharpCheckProjectResults: FSharp.Compiler.CodeAnalysis.FSharpAssemblyContents AssemblyContents
FSharp.Compiler.CodeAnalysis.FSharpCheckProjectResults: FSharp.Compiler.CodeAnalysis.FSharpAssemblyContents GetOptimizedAssemblyContents()
FSharp.Compiler.CodeAnalysis.FSharpCheckProjectResults: FSharp.Compiler.CodeAnalysis.FSharpAssemblyContents get_AssemblyContents()
FSharp.Compiler.CodeAnalysis.FSharpCheckProjectResults: FSharp.Compiler.CodeAnalysis.FSharpAssemblySignature AssemblySignature
FSharp.Compiler.CodeAnalysis.FSharpCheckProjectResults: FSharp.Compiler.CodeAnalysis.FSharpAssemblySignature get_AssemblySignature()
FSharp.Compiler.CodeAnalysis.FSharpCheckProjectResults: FSharp.Compiler.CodeAnalysis.FSharpProjectContext ProjectContext
FSharp.Compiler.CodeAnalysis.FSharpCheckProjectResults: FSharp.Compiler.CodeAnalysis.FSharpProjectContext get_ProjectContext()
FSharp.Compiler.CodeAnalysis.FSharpCheckProjectResults: FSharp.Compiler.CodeAnalysis.FSharpSymbolUse[] GetAllUsesOfAllSymbols(Microsoft.FSharp.Core.FSharpOption`1[System.Threading.CancellationToken])
FSharp.Compiler.CodeAnalysis.FSharpCheckProjectResults: FSharp.Compiler.CodeAnalysis.FSharpSymbolUse[] GetUsesOfSymbol(FSharp.Compiler.CodeAnalysis.FSharpSymbol, Microsoft.FSharp.Core.FSharpOption`1[System.Threading.CancellationToken])
FSharp.Compiler.CodeAnalysis.FSharpCheckProjectResults: FSharp.Compiler.Diagnostics.FSharpDiagnostic[] Diagnostics
FSharp.Compiler.CodeAnalysis.FSharpCheckProjectResults: FSharp.Compiler.Diagnostics.FSharpDiagnostic[] get_Diagnostics()
FSharp.Compiler.CodeAnalysis.FSharpCheckProjectResults: System.String ToString()
FSharp.Compiler.CodeAnalysis.FSharpCheckProjectResults: System.String[] DependencyFiles
FSharp.Compiler.CodeAnalysis.FSharpCheckProjectResults: System.String[] get_DependencyFiles()
FSharp.Compiler.CodeAnalysis.FSharpChecker
FSharp.Compiler.CodeAnalysis.FSharpChecker: Boolean ImplicitlyStartBackgroundWork
FSharp.Compiler.CodeAnalysis.FSharpChecker: Boolean get_ImplicitlyStartBackgroundWork()
FSharp.Compiler.CodeAnalysis.FSharpChecker: FSharp.Compiler.CodeAnalysis.FSharpChecker Create(Microsoft.FSharp.Core.FSharpOption`1[System.Int32], Microsoft.FSharp.Core.FSharpOption`1[System.Boolean], Microsoft.FSharp.Core.FSharpOption`1[System.Boolean], Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.LegacyReferenceResolver], Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.FSharpFunc`2[System.Tuple`2[System.String,System.DateTime],Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`3[System.Object,System.IntPtr,System.Int32]]]], Microsoft.FSharp.Core.FSharpOption`1[System.Boolean], Microsoft.FSharp.Core.FSharpOption`1[System.Boolean], Microsoft.FSharp.Core.FSharpOption`1[System.Boolean], Microsoft.FSharp.Core.FSharpOption`1[System.Boolean])
FSharp.Compiler.CodeAnalysis.FSharpChecker: FSharp.Compiler.CodeAnalysis.FSharpChecker Instance
FSharp.Compiler.CodeAnalysis.FSharpChecker: FSharp.Compiler.CodeAnalysis.FSharpChecker get_Instance()
FSharp.Compiler.CodeAnalysis.FSharpChecker: FSharp.Compiler.CodeAnalysis.FSharpProjectOptions GetProjectOptionsFromCommandLineArgs(System.String, System.String[], Microsoft.FSharp.Core.FSharpOption`1[System.DateTime], Microsoft.FSharp.Core.FSharpOption`1[System.Object])
FSharp.Compiler.CodeAnalysis.FSharpChecker: FSharp.Compiler.EditorServices.FSharpTokenInfo[][] TokenizeFile(System.String)
FSharp.Compiler.CodeAnalysis.FSharpChecker: Int32 CurrentQueueLength
FSharp.Compiler.CodeAnalysis.FSharpChecker: Int32 GlobalForegroundParseCountStatistic
FSharp.Compiler.CodeAnalysis.FSharpChecker: Int32 GlobalForegroundTypeCheckCountStatistic
FSharp.Compiler.CodeAnalysis.FSharpChecker: Int32 MaxMemory
FSharp.Compiler.CodeAnalysis.FSharpChecker: Int32 PauseBeforeBackgroundWork
FSharp.Compiler.CodeAnalysis.FSharpChecker: Int32 get_CurrentQueueLength()
FSharp.Compiler.CodeAnalysis.FSharpChecker: Int32 get_GlobalForegroundParseCountStatistic()
FSharp.Compiler.CodeAnalysis.FSharpChecker: Int32 get_GlobalForegroundTypeCheckCountStatistic()
FSharp.Compiler.CodeAnalysis.FSharpChecker: Int32 get_MaxMemory()
FSharp.Compiler.CodeAnalysis.FSharpChecker: Int32 get_PauseBeforeBackgroundWork()
FSharp.Compiler.CodeAnalysis.FSharpChecker: Microsoft.FSharp.Control.FSharpAsync`1[FSharp.Compiler.CodeAnalysis.FSharpCheckFileAnswer] CheckFileInProject(FSharp.Compiler.CodeAnalysis.FSharpParseFileResults, System.String, Int32, FSharp.Compiler.Text.ISourceText, FSharp.Compiler.CodeAnalysis.FSharpProjectOptions, Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.CodeAnalysis.FSharpChecker: Microsoft.FSharp.Control.FSharpAsync`1[FSharp.Compiler.CodeAnalysis.FSharpCheckProjectResults] ParseAndCheckProject(FSharp.Compiler.CodeAnalysis.FSharpProjectOptions, Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.CodeAnalysis.FSharpChecker: Microsoft.FSharp.Control.FSharpAsync`1[FSharp.Compiler.CodeAnalysis.FSharpParseFileResults] GetBackgroundParseResultsForFileInProject(System.String, FSharp.Compiler.CodeAnalysis.FSharpProjectOptions, Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.CodeAnalysis.FSharpChecker: Microsoft.FSharp.Control.FSharpAsync`1[FSharp.Compiler.CodeAnalysis.FSharpParseFileResults] ParseFile(System.String, FSharp.Compiler.Text.ISourceText, FSharp.Compiler.CodeAnalysis.FSharpParsingOptions, Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.CodeAnalysis.FSharpChecker: Microsoft.FSharp.Control.FSharpAsync`1[FSharp.Compiler.CodeAnalysis.FSharpParseFileResults] ParseFileInProject(System.String, System.String, FSharp.Compiler.CodeAnalysis.FSharpProjectOptions, Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.CodeAnalysis.FSharpChecker: Microsoft.FSharp.Control.FSharpAsync`1[FSharp.Compiler.CodeAnalysis.FSharpParseFileResults] ParseFileNoCache(System.String, FSharp.Compiler.Text.ISourceText, FSharp.Compiler.CodeAnalysis.FSharpParsingOptions, Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.CodeAnalysis.FSharpChecker: Microsoft.FSharp.Control.FSharpAsync`1[Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpCheckFileAnswer]] CheckFileInProjectAllowingStaleCachedResults(FSharp.Compiler.CodeAnalysis.FSharpParseFileResults, System.String, Int32, System.String, FSharp.Compiler.CodeAnalysis.FSharpProjectOptions, Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.CodeAnalysis.FSharpChecker: Microsoft.FSharp.Control.FSharpAsync`1[Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.SemanticClassificationView]] GetBackgroundSemanticClassificationForFile(System.String, FSharp.Compiler.CodeAnalysis.FSharpProjectOptions, Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.CodeAnalysis.FSharpChecker: Microsoft.FSharp.Control.FSharpAsync`1[Microsoft.FSharp.Core.Unit] NotifyProjectCleaned(FSharp.Compiler.CodeAnalysis.FSharpProjectOptions, Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.CodeAnalysis.FSharpChecker: Microsoft.FSharp.Control.FSharpAsync`1[System.Collections.Generic.IEnumerable`1[FSharp.Compiler.Text.Range]] FindBackgroundReferencesInFile(System.String, FSharp.Compiler.CodeAnalysis.FSharpProjectOptions, FSharp.Compiler.CodeAnalysis.FSharpSymbol, Microsoft.FSharp.Core.FSharpOption`1[System.Boolean], Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.CodeAnalysis.FSharpChecker: Microsoft.FSharp.Control.FSharpAsync`1[System.Tuple`2[FSharp.Compiler.CodeAnalysis.FSharpParseFileResults,FSharp.Compiler.CodeAnalysis.FSharpCheckFileAnswer]] ParseAndCheckFileInProject(System.String, Int32, FSharp.Compiler.Text.ISourceText, FSharp.Compiler.CodeAnalysis.FSharpProjectOptions, Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.CodeAnalysis.FSharpChecker: Microsoft.FSharp.Control.FSharpAsync`1[System.Tuple`2[FSharp.Compiler.CodeAnalysis.FSharpParseFileResults,FSharp.Compiler.CodeAnalysis.FSharpCheckFileResults]] GetBackgroundCheckResultsForFileInProject(System.String, FSharp.Compiler.CodeAnalysis.FSharpProjectOptions, Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.CodeAnalysis.FSharpChecker: Microsoft.FSharp.Control.FSharpAsync`1[System.Tuple`2[FSharp.Compiler.CodeAnalysis.FSharpProjectOptions,Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Diagnostics.FSharpDiagnostic]]] GetProjectOptionsFromScript(System.String, FSharp.Compiler.Text.ISourceText, Microsoft.FSharp.Core.FSharpOption`1[System.Boolean], Microsoft.FSharp.Core.FSharpOption`1[System.DateTime], Microsoft.FSharp.Core.FSharpOption`1[System.String[]], Microsoft.FSharp.Core.FSharpOption`1[System.Boolean], Microsoft.FSharp.Core.FSharpOption`1[System.Boolean], Microsoft.FSharp.Core.FSharpOption`1[System.Boolean], Microsoft.FSharp.Core.FSharpOption`1[System.String], Microsoft.FSharp.Core.FSharpOption`1[System.Object], Microsoft.FSharp.Core.FSharpOption`1[System.Int64], Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.CodeAnalysis.FSharpChecker: Microsoft.FSharp.Control.FSharpAsync`1[System.Tuple`2[FSharp.Compiler.Diagnostics.FSharpDiagnostic[],System.Int32]] Compile(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.ParsedInput], System.String, System.String, Microsoft.FSharp.Collections.FSharpList`1[System.String], Microsoft.FSharp.Core.FSharpOption`1[System.String], Microsoft.FSharp.Core.FSharpOption`1[System.Boolean], Microsoft.FSharp.Core.FSharpOption`1[System.Boolean], Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.CodeAnalysis.FSharpChecker: Microsoft.FSharp.Control.FSharpAsync`1[System.Tuple`2[FSharp.Compiler.Diagnostics.FSharpDiagnostic[],System.Int32]] Compile(System.String[], Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.CodeAnalysis.FSharpChecker: Microsoft.FSharp.Control.FSharpAsync`1[System.Tuple`2[FSharp.Compiler.Text.Range,FSharp.Compiler.Text.Range][]] MatchBraces(System.String, FSharp.Compiler.Text.ISourceText, FSharp.Compiler.CodeAnalysis.FSharpParsingOptions, Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.CodeAnalysis.FSharpChecker: Microsoft.FSharp.Control.FSharpAsync`1[System.Tuple`2[FSharp.Compiler.Text.Range,FSharp.Compiler.Text.Range][]] MatchBraces(System.String, System.String, FSharp.Compiler.CodeAnalysis.FSharpProjectOptions, Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.CodeAnalysis.FSharpChecker: Microsoft.FSharp.Control.FSharpAsync`1[System.Tuple`3[FSharp.Compiler.Diagnostics.FSharpDiagnostic[],System.Int32,Microsoft.FSharp.Core.FSharpOption`1[System.Reflection.Assembly]]] CompileToDynamicAssembly(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.ParsedInput], System.String, Microsoft.FSharp.Collections.FSharpList`1[System.String], Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[System.IO.TextWriter,System.IO.TextWriter]], Microsoft.FSharp.Core.FSharpOption`1[System.Boolean], Microsoft.FSharp.Core.FSharpOption`1[System.Boolean], Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.CodeAnalysis.FSharpChecker: Microsoft.FSharp.Control.FSharpAsync`1[System.Tuple`3[FSharp.Compiler.Diagnostics.FSharpDiagnostic[],System.Int32,Microsoft.FSharp.Core.FSharpOption`1[System.Reflection.Assembly]]] CompileToDynamicAssembly(System.String[], Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[System.IO.TextWriter,System.IO.TextWriter]], Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.CodeAnalysis.FSharpChecker: Microsoft.FSharp.Control.IEvent`2[Microsoft.FSharp.Control.FSharpHandler`1[Microsoft.FSharp.Core.Unit],Microsoft.FSharp.Core.Unit] MaxMemoryReached
FSharp.Compiler.CodeAnalysis.FSharpChecker: Microsoft.FSharp.Control.IEvent`2[Microsoft.FSharp.Control.FSharpHandler`1[Microsoft.FSharp.Core.Unit],Microsoft.FSharp.Core.Unit] get_MaxMemoryReached()
FSharp.Compiler.CodeAnalysis.FSharpChecker: Microsoft.FSharp.Control.IEvent`2[Microsoft.FSharp.Control.FSharpHandler`1[System.Tuple`2[System.String,Microsoft.FSharp.Core.FSharpOption`1[System.Object]]],System.Tuple`2[System.String,Microsoft.FSharp.Core.FSharpOption`1[System.Object]]] BeforeBackgroundFileCheck
FSharp.Compiler.CodeAnalysis.FSharpChecker: Microsoft.FSharp.Control.IEvent`2[Microsoft.FSharp.Control.FSharpHandler`1[System.Tuple`2[System.String,Microsoft.FSharp.Core.FSharpOption`1[System.Object]]],System.Tuple`2[System.String,Microsoft.FSharp.Core.FSharpOption`1[System.Object]]] FileChecked
FSharp.Compiler.CodeAnalysis.FSharpChecker: Microsoft.FSharp.Control.IEvent`2[Microsoft.FSharp.Control.FSharpHandler`1[System.Tuple`2[System.String,Microsoft.FSharp.Core.FSharpOption`1[System.Object]]],System.Tuple`2[System.String,Microsoft.FSharp.Core.FSharpOption`1[System.Object]]] FileParsed
FSharp.Compiler.CodeAnalysis.FSharpChecker: Microsoft.FSharp.Control.IEvent`2[Microsoft.FSharp.Control.FSharpHandler`1[System.Tuple`2[System.String,Microsoft.FSharp.Core.FSharpOption`1[System.Object]]],System.Tuple`2[System.String,Microsoft.FSharp.Core.FSharpOption`1[System.Object]]] ProjectChecked
FSharp.Compiler.CodeAnalysis.FSharpChecker: Microsoft.FSharp.Control.IEvent`2[Microsoft.FSharp.Control.FSharpHandler`1[System.Tuple`2[System.String,Microsoft.FSharp.Core.FSharpOption`1[System.Object]]],System.Tuple`2[System.String,Microsoft.FSharp.Core.FSharpOption`1[System.Object]]] get_BeforeBackgroundFileCheck()
FSharp.Compiler.CodeAnalysis.FSharpChecker: Microsoft.FSharp.Control.IEvent`2[Microsoft.FSharp.Control.FSharpHandler`1[System.Tuple`2[System.String,Microsoft.FSharp.Core.FSharpOption`1[System.Object]]],System.Tuple`2[System.String,Microsoft.FSharp.Core.FSharpOption`1[System.Object]]] get_FileChecked()
FSharp.Compiler.CodeAnalysis.FSharpChecker: Microsoft.FSharp.Control.IEvent`2[Microsoft.FSharp.Control.FSharpHandler`1[System.Tuple`2[System.String,Microsoft.FSharp.Core.FSharpOption`1[System.Object]]],System.Tuple`2[System.String,Microsoft.FSharp.Core.FSharpOption`1[System.Object]]] get_FileParsed()
FSharp.Compiler.CodeAnalysis.FSharpChecker: Microsoft.FSharp.Control.IEvent`2[Microsoft.FSharp.Control.FSharpHandler`1[System.Tuple`2[System.String,Microsoft.FSharp.Core.FSharpOption`1[System.Object]]],System.Tuple`2[System.String,Microsoft.FSharp.Core.FSharpOption`1[System.Object]]] get_ProjectChecked()
FSharp.Compiler.CodeAnalysis.FSharpChecker: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`3[FSharp.Compiler.CodeAnalysis.FSharpParseFileResults,FSharp.Compiler.CodeAnalysis.FSharpCheckFileResults,System.Int32]] TryGetRecentCheckResultsForFile(System.String, FSharp.Compiler.CodeAnalysis.FSharpProjectOptions, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.ISourceText], Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.CodeAnalysis.FSharpChecker: System.Tuple`2[FSharp.Compiler.CodeAnalysis.FSharpParsingOptions,Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Diagnostics.FSharpDiagnostic]] GetParsingOptionsFromCommandLineArgs(Microsoft.FSharp.Collections.FSharpList`1[System.String], Microsoft.FSharp.Collections.FSharpList`1[System.String], Microsoft.FSharp.Core.FSharpOption`1[System.Boolean])
FSharp.Compiler.CodeAnalysis.FSharpChecker: System.Tuple`2[FSharp.Compiler.CodeAnalysis.FSharpParsingOptions,Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Diagnostics.FSharpDiagnostic]] GetParsingOptionsFromCommandLineArgs(Microsoft.FSharp.Collections.FSharpList`1[System.String], Microsoft.FSharp.Core.FSharpOption`1[System.Boolean])
FSharp.Compiler.CodeAnalysis.FSharpChecker: System.Tuple`2[FSharp.Compiler.CodeAnalysis.FSharpParsingOptions,Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Diagnostics.FSharpDiagnostic]] GetParsingOptionsFromProjectOptions(FSharp.Compiler.CodeAnalysis.FSharpProjectOptions)
FSharp.Compiler.CodeAnalysis.FSharpChecker: System.Tuple`2[FSharp.Compiler.EditorServices.FSharpTokenInfo[],FSharp.Compiler.EditorServices.FSharpTokenizerLexState] TokenizeLine(System.String, FSharp.Compiler.EditorServices.FSharpTokenizerLexState)
FSharp.Compiler.CodeAnalysis.FSharpChecker: Void CheckProjectInBackground(FSharp.Compiler.CodeAnalysis.FSharpProjectOptions, Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.CodeAnalysis.FSharpChecker: Void ClearCache(System.Collections.Generic.IEnumerable`1[FSharp.Compiler.CodeAnalysis.FSharpProjectOptions], Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.CodeAnalysis.FSharpChecker: Void ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()
FSharp.Compiler.CodeAnalysis.FSharpChecker: Void InvalidateAll()
FSharp.Compiler.CodeAnalysis.FSharpChecker: Void InvalidateConfiguration(FSharp.Compiler.CodeAnalysis.FSharpProjectOptions, Microsoft.FSharp.Core.FSharpOption`1[System.Boolean], Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.CodeAnalysis.FSharpChecker: Void StopBackgroundCompile()
FSharp.Compiler.CodeAnalysis.FSharpChecker: Void WaitForBackgroundCompile()
FSharp.Compiler.CodeAnalysis.FSharpChecker: Void set_ImplicitlyStartBackgroundWork(Boolean)
FSharp.Compiler.CodeAnalysis.FSharpChecker: Void set_MaxMemory(Int32)
FSharp.Compiler.CodeAnalysis.FSharpChecker: Void set_PauseBeforeBackgroundWork(Int32)
FSharp.Compiler.CodeAnalysis.FSharpDelegateSignature
FSharp.Compiler.CodeAnalysis.FSharpDelegateSignature: FSharp.Compiler.CodeAnalysis.FSharpType DelegateReturnType
FSharp.Compiler.CodeAnalysis.FSharpDelegateSignature: FSharp.Compiler.CodeAnalysis.FSharpType get_DelegateReturnType()
FSharp.Compiler.CodeAnalysis.FSharpDelegateSignature: System.Collections.Generic.IList`1[System.Tuple`2[Microsoft.FSharp.Core.FSharpOption`1[System.String],FSharp.Compiler.CodeAnalysis.FSharpType]] DelegateArguments
FSharp.Compiler.CodeAnalysis.FSharpDelegateSignature: System.Collections.Generic.IList`1[System.Tuple`2[Microsoft.FSharp.Core.FSharpOption`1[System.String],FSharp.Compiler.CodeAnalysis.FSharpType]] get_DelegateArguments()
FSharp.Compiler.CodeAnalysis.FSharpDelegateSignature: System.String ToString()
FSharp.Compiler.CodeAnalysis.FSharpDisplayContext
FSharp.Compiler.CodeAnalysis.FSharpDisplayContext: FSharp.Compiler.CodeAnalysis.FSharpDisplayContext Empty
FSharp.Compiler.CodeAnalysis.FSharpDisplayContext: FSharp.Compiler.CodeAnalysis.FSharpDisplayContext WithPrefixGenericParameters()
FSharp.Compiler.CodeAnalysis.FSharpDisplayContext: FSharp.Compiler.CodeAnalysis.FSharpDisplayContext WithShortTypeNames(Boolean)
FSharp.Compiler.CodeAnalysis.FSharpDisplayContext: FSharp.Compiler.CodeAnalysis.FSharpDisplayContext WithSuffixGenericParameters()
FSharp.Compiler.CodeAnalysis.FSharpDisplayContext: FSharp.Compiler.CodeAnalysis.FSharpDisplayContext get_Empty()
FSharp.Compiler.CodeAnalysis.FSharpEntity
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean Equals(System.Object)
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean HasAssemblyCodeRepresentation
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean HasFSharpModuleSuffix
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean IsAbstractClass
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean IsArrayType
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean IsAttributeType
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean IsByRef
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean IsClass
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean IsDelegate
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean IsEnum
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean IsFSharp
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean IsFSharpAbbreviation
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean IsFSharpExceptionDeclaration
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean IsFSharpModule
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean IsFSharpRecord
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean IsFSharpUnion
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean IsInterface
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean IsMeasure
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean IsNamespace
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean IsOpaque
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean IsProvided
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean IsProvidedAndErased
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean IsProvidedAndGenerated
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean IsStaticInstantiation
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean IsUnresolved
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean IsValueType
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean UsesPrefixDisplay
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean get_HasAssemblyCodeRepresentation()
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean get_HasFSharpModuleSuffix()
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean get_IsAbstractClass()
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean get_IsArrayType()
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean get_IsAttributeType()
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean get_IsByRef()
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean get_IsClass()
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean get_IsDelegate()
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean get_IsEnum()
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean get_IsFSharp()
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean get_IsFSharpAbbreviation()
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean get_IsFSharpExceptionDeclaration()
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean get_IsFSharpModule()
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean get_IsFSharpRecord()
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean get_IsFSharpUnion()
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean get_IsInterface()
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean get_IsMeasure()
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean get_IsNamespace()
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean get_IsOpaque()
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean get_IsProvided()
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean get_IsProvidedAndErased()
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean get_IsProvidedAndGenerated()
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean get_IsStaticInstantiation()
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean get_IsUnresolved()
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean get_IsValueType()
FSharp.Compiler.CodeAnalysis.FSharpEntity: Boolean get_UsesPrefixDisplay()
FSharp.Compiler.CodeAnalysis.FSharpEntity: FSharp.Compiler.CodeAnalysis.FSharpAccessibility Accessibility
FSharp.Compiler.CodeAnalysis.FSharpEntity: FSharp.Compiler.CodeAnalysis.FSharpAccessibility RepresentationAccessibility
FSharp.Compiler.CodeAnalysis.FSharpEntity: FSharp.Compiler.CodeAnalysis.FSharpAccessibility get_Accessibility()
FSharp.Compiler.CodeAnalysis.FSharpEntity: FSharp.Compiler.CodeAnalysis.FSharpAccessibility get_RepresentationAccessibility()
FSharp.Compiler.CodeAnalysis.FSharpEntity: FSharp.Compiler.CodeAnalysis.FSharpDelegateSignature FSharpDelegateSignature
FSharp.Compiler.CodeAnalysis.FSharpEntity: FSharp.Compiler.CodeAnalysis.FSharpDelegateSignature get_FSharpDelegateSignature()
FSharp.Compiler.CodeAnalysis.FSharpEntity: FSharp.Compiler.CodeAnalysis.FSharpType AbbreviatedType
FSharp.Compiler.CodeAnalysis.FSharpEntity: FSharp.Compiler.CodeAnalysis.FSharpType get_AbbreviatedType()
FSharp.Compiler.CodeAnalysis.FSharpEntity: FSharp.Compiler.Text.Range DeclarationLocation
FSharp.Compiler.CodeAnalysis.FSharpEntity: FSharp.Compiler.Text.Range get_DeclarationLocation()
FSharp.Compiler.CodeAnalysis.FSharpEntity: Int32 ArrayRank
FSharp.Compiler.CodeAnalysis.FSharpEntity: Int32 GetHashCode()
FSharp.Compiler.CodeAnalysis.FSharpEntity: Int32 get_ArrayRank()
FSharp.Compiler.CodeAnalysis.FSharpEntity: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpActivePatternCase] ActivePatternCases
FSharp.Compiler.CodeAnalysis.FSharpEntity: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpActivePatternCase] get_ActivePatternCases()
FSharp.Compiler.CodeAnalysis.FSharpEntity: Microsoft.FSharp.Collections.FSharpList`1[System.String] AllCompilationPaths
FSharp.Compiler.CodeAnalysis.FSharpEntity: Microsoft.FSharp.Collections.FSharpList`1[System.String] get_AllCompilationPaths()
FSharp.Compiler.CodeAnalysis.FSharpEntity: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpEntity] DeclaringEntity
FSharp.Compiler.CodeAnalysis.FSharpEntity: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpEntity] get_DeclaringEntity()
FSharp.Compiler.CodeAnalysis.FSharpEntity: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpType] BaseType
FSharp.Compiler.CodeAnalysis.FSharpEntity: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpType] get_BaseType()
FSharp.Compiler.CodeAnalysis.FSharpEntity: Microsoft.FSharp.Core.FSharpOption`1[System.String] Namespace
FSharp.Compiler.CodeAnalysis.FSharpEntity: Microsoft.FSharp.Core.FSharpOption`1[System.String] TryFullName
FSharp.Compiler.CodeAnalysis.FSharpEntity: Microsoft.FSharp.Core.FSharpOption`1[System.String] TryGetFullCompiledName()
FSharp.Compiler.CodeAnalysis.FSharpEntity: Microsoft.FSharp.Core.FSharpOption`1[System.String] TryGetFullDisplayName()
FSharp.Compiler.CodeAnalysis.FSharpEntity: Microsoft.FSharp.Core.FSharpOption`1[System.String] TryGetFullName()
FSharp.Compiler.CodeAnalysis.FSharpEntity: Microsoft.FSharp.Core.FSharpOption`1[System.String] get_Namespace()
FSharp.Compiler.CodeAnalysis.FSharpEntity: Microsoft.FSharp.Core.FSharpOption`1[System.String] get_TryFullName()
FSharp.Compiler.CodeAnalysis.FSharpEntity: System.Collections.Generic.IEnumerable`1[FSharp.Compiler.CodeAnalysis.FSharpEntity] GetPublicNestedEntities()
FSharp.Compiler.CodeAnalysis.FSharpEntity: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpAttribute] Attributes
FSharp.Compiler.CodeAnalysis.FSharpEntity: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpAttribute] get_Attributes()
FSharp.Compiler.CodeAnalysis.FSharpEntity: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpEntity] NestedEntities
FSharp.Compiler.CodeAnalysis.FSharpEntity: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpEntity] get_NestedEntities()
FSharp.Compiler.CodeAnalysis.FSharpEntity: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpField] FSharpFields
FSharp.Compiler.CodeAnalysis.FSharpEntity: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpField] RecordFields
FSharp.Compiler.CodeAnalysis.FSharpEntity: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpField] get_FSharpFields()
FSharp.Compiler.CodeAnalysis.FSharpEntity: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpField] get_RecordFields()
FSharp.Compiler.CodeAnalysis.FSharpEntity: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpGenericParameter] GenericParameters
FSharp.Compiler.CodeAnalysis.FSharpEntity: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpGenericParameter] get_GenericParameters()
FSharp.Compiler.CodeAnalysis.FSharpEntity: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue] MembersFunctionsAndValues
FSharp.Compiler.CodeAnalysis.FSharpEntity: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue] MembersOrValues
FSharp.Compiler.CodeAnalysis.FSharpEntity: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue] TryGetMembersFunctionsAndValues()
FSharp.Compiler.CodeAnalysis.FSharpEntity: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue] get_MembersFunctionsAndValues()
FSharp.Compiler.CodeAnalysis.FSharpEntity: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue] get_MembersOrValues()
FSharp.Compiler.CodeAnalysis.FSharpEntity: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpStaticParameter] StaticParameters
FSharp.Compiler.CodeAnalysis.FSharpEntity: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpStaticParameter] get_StaticParameters()
FSharp.Compiler.CodeAnalysis.FSharpEntity: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpType] AllInterfaces
FSharp.Compiler.CodeAnalysis.FSharpEntity: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpType] DeclaredInterfaces
FSharp.Compiler.CodeAnalysis.FSharpEntity: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpType] get_AllInterfaces()
FSharp.Compiler.CodeAnalysis.FSharpEntity: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpType] get_DeclaredInterfaces()
FSharp.Compiler.CodeAnalysis.FSharpEntity: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpUnionCase] UnionCases
FSharp.Compiler.CodeAnalysis.FSharpEntity: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpUnionCase] get_UnionCases()
FSharp.Compiler.CodeAnalysis.FSharpEntity: System.Collections.Generic.IList`1[System.String] ElaboratedXmlDoc
FSharp.Compiler.CodeAnalysis.FSharpEntity: System.Collections.Generic.IList`1[System.String] XmlDoc
FSharp.Compiler.CodeAnalysis.FSharpEntity: System.Collections.Generic.IList`1[System.String] get_ElaboratedXmlDoc()
FSharp.Compiler.CodeAnalysis.FSharpEntity: System.Collections.Generic.IList`1[System.String] get_XmlDoc()
FSharp.Compiler.CodeAnalysis.FSharpEntity: System.String AccessPath
FSharp.Compiler.CodeAnalysis.FSharpEntity: System.String CompiledName
FSharp.Compiler.CodeAnalysis.FSharpEntity: System.String DisplayName
FSharp.Compiler.CodeAnalysis.FSharpEntity: System.String FullName
FSharp.Compiler.CodeAnalysis.FSharpEntity: System.String LogicalName
FSharp.Compiler.CodeAnalysis.FSharpEntity: System.String QualifiedName
FSharp.Compiler.CodeAnalysis.FSharpEntity: System.String ToString()
FSharp.Compiler.CodeAnalysis.FSharpEntity: System.String XmlDocSig
FSharp.Compiler.CodeAnalysis.FSharpEntity: System.String get_AccessPath()
FSharp.Compiler.CodeAnalysis.FSharpEntity: System.String get_CompiledName()
FSharp.Compiler.CodeAnalysis.FSharpEntity: System.String get_DisplayName()
FSharp.Compiler.CodeAnalysis.FSharpEntity: System.String get_FullName()
FSharp.Compiler.CodeAnalysis.FSharpEntity: System.String get_LogicalName()
FSharp.Compiler.CodeAnalysis.FSharpEntity: System.String get_QualifiedName()
FSharp.Compiler.CodeAnalysis.FSharpEntity: System.String get_XmlDocSig()
FSharp.Compiler.CodeAnalysis.FSharpExpr
FSharp.Compiler.CodeAnalysis.FSharpExpr: FSharp.Compiler.CodeAnalysis.FSharpType Type
FSharp.Compiler.CodeAnalysis.FSharpExpr: FSharp.Compiler.CodeAnalysis.FSharpType get_Type()
FSharp.Compiler.CodeAnalysis.FSharpExpr: FSharp.Compiler.Text.Range Range
FSharp.Compiler.CodeAnalysis.FSharpExpr: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.CodeAnalysis.FSharpExpr: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpExpr] ImmediateSubExpressions
FSharp.Compiler.CodeAnalysis.FSharpExpr: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpExpr] get_ImmediateSubExpressions()
FSharp.Compiler.CodeAnalysis.FSharpExpr: System.String ToString()
FSharp.Compiler.CodeAnalysis.FindDeclExternalParam
FSharp.Compiler.CodeAnalysis.FindDeclExternalParam: Boolean Equals(FSharp.Compiler.CodeAnalysis.FindDeclExternalParam)
FSharp.Compiler.CodeAnalysis.FindDeclExternalParam: Boolean Equals(System.Object)
FSharp.Compiler.CodeAnalysis.FindDeclExternalParam: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.CodeAnalysis.FindDeclExternalParam: Boolean IsByRef
FSharp.Compiler.CodeAnalysis.FindDeclExternalParam: Boolean get_IsByRef()
FSharp.Compiler.CodeAnalysis.FindDeclExternalParam: FSharp.Compiler.CodeAnalysis.FindDeclExternalParam Create(FSharp.Compiler.CodeAnalysis.FindDeclExternalType, Boolean)
FSharp.Compiler.CodeAnalysis.FindDeclExternalParam: FSharp.Compiler.CodeAnalysis.FindDeclExternalType ParameterType
FSharp.Compiler.CodeAnalysis.FindDeclExternalParam: FSharp.Compiler.CodeAnalysis.FindDeclExternalType get_ParameterType()
FSharp.Compiler.CodeAnalysis.FindDeclExternalParam: Int32 CompareTo(FSharp.Compiler.CodeAnalysis.FindDeclExternalParam)
FSharp.Compiler.CodeAnalysis.FindDeclExternalParam: Int32 CompareTo(System.Object)
FSharp.Compiler.CodeAnalysis.FindDeclExternalParam: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.CodeAnalysis.FindDeclExternalParam: Int32 GetHashCode()
FSharp.Compiler.CodeAnalysis.FindDeclExternalParam: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.CodeAnalysis.FindDeclExternalParam: System.String ToString()
FSharp.Compiler.CodeAnalysis.FSharpExternalParamModule
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol+Constructor: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FindDeclExternalParam] args
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol+Constructor: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FindDeclExternalParam] get_args()
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol+Constructor: System.String get_typeName()
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol+Constructor: System.String typeName
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol+Event: System.String get_name()
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol+Event: System.String get_typeName()
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol+Event: System.String name
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol+Event: System.String typeName
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol+Field: System.String get_name()
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol+Field: System.String get_typeName()
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol+Field: System.String name
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol+Field: System.String typeName
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol+Method: Int32 genericArity
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol+Method: Int32 get_genericArity()
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol+Method: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FindDeclExternalParam] get_paramSyms()
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol+Method: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FindDeclExternalParam] paramSyms
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol+Method: System.String get_name()
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol+Method: System.String get_typeName()
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol+Method: System.String name
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol+Method: System.String typeName
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol+Property: System.String get_name()
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol+Property: System.String get_typeName()
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol+Property: System.String name
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol+Property: System.String typeName
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol+Tags: Int32 Constructor
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol+Tags: Int32 Event
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol+Tags: Int32 Field
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol+Tags: Int32 Method
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol+Tags: Int32 Property
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol+Tags: Int32 Type
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol+Type: System.String fullName
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol+Type: System.String get_fullName()
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol: Boolean Equals(FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol)
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol: Boolean Equals(System.Object)
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol: Boolean IsConstructor
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol: Boolean IsEvent
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol: Boolean IsField
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol: Boolean IsMethod
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol: Boolean IsProperty
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol: Boolean IsType
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol: Boolean get_IsConstructor()
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol: Boolean get_IsEvent()
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol: Boolean get_IsField()
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol: Boolean get_IsMethod()
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol: Boolean get_IsProperty()
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol: Boolean get_IsType()
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol: FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol NewConstructor(System.String, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FindDeclExternalParam])
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol: FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol NewEvent(System.String, System.String)
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol: FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol NewField(System.String, System.String)
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol: FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol NewMethod(System.String, System.String, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FindDeclExternalParam], Int32)
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol: FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol NewProperty(System.String, System.String)
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol: FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol NewType(System.String)
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol: FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol+Constructor
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol: FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol+Event
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol: FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol+Field
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol: FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol+Method
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol: FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol+Property
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol: FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol+Tags
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol: FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol+Type
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol: Int32 CompareTo(FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol)
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol: Int32 CompareTo(System.Object)
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol: Int32 GetHashCode()
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol: Int32 Tag
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol: Int32 get_Tag()
FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol: System.String ToString()
FSharp.Compiler.CodeAnalysis.FindDeclExternalType
FSharp.Compiler.CodeAnalysis.FindDeclExternalType+Array: FSharp.Compiler.CodeAnalysis.FindDeclExternalType get_inner()
FSharp.Compiler.CodeAnalysis.FindDeclExternalType+Array: FSharp.Compiler.CodeAnalysis.FindDeclExternalType inner
FSharp.Compiler.CodeAnalysis.FindDeclExternalType+Pointer: FSharp.Compiler.CodeAnalysis.FindDeclExternalType get_inner()
FSharp.Compiler.CodeAnalysis.FindDeclExternalType+Pointer: FSharp.Compiler.CodeAnalysis.FindDeclExternalType inner
FSharp.Compiler.CodeAnalysis.FindDeclExternalType+Tags: Int32 Array
FSharp.Compiler.CodeAnalysis.FindDeclExternalType+Tags: Int32 Pointer
FSharp.Compiler.CodeAnalysis.FindDeclExternalType+Tags: Int32 Type
FSharp.Compiler.CodeAnalysis.FindDeclExternalType+Tags: Int32 TypeVar
FSharp.Compiler.CodeAnalysis.FindDeclExternalType+Type: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FindDeclExternalType] genericArgs
FSharp.Compiler.CodeAnalysis.FindDeclExternalType+Type: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FindDeclExternalType] get_genericArgs()
FSharp.Compiler.CodeAnalysis.FindDeclExternalType+Type: System.String fullName
FSharp.Compiler.CodeAnalysis.FindDeclExternalType+Type: System.String get_fullName()
FSharp.Compiler.CodeAnalysis.FindDeclExternalType+TypeVar: System.String get_typeName()
FSharp.Compiler.CodeAnalysis.FindDeclExternalType+TypeVar: System.String typeName
FSharp.Compiler.CodeAnalysis.FindDeclExternalType: Boolean Equals(FSharp.Compiler.CodeAnalysis.FindDeclExternalType)
FSharp.Compiler.CodeAnalysis.FindDeclExternalType: Boolean Equals(System.Object)
FSharp.Compiler.CodeAnalysis.FindDeclExternalType: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.CodeAnalysis.FindDeclExternalType: Boolean IsArray
FSharp.Compiler.CodeAnalysis.FindDeclExternalType: Boolean IsPointer
FSharp.Compiler.CodeAnalysis.FindDeclExternalType: Boolean IsType
FSharp.Compiler.CodeAnalysis.FindDeclExternalType: Boolean IsTypeVar
FSharp.Compiler.CodeAnalysis.FindDeclExternalType: Boolean get_IsArray()
FSharp.Compiler.CodeAnalysis.FindDeclExternalType: Boolean get_IsPointer()
FSharp.Compiler.CodeAnalysis.FindDeclExternalType: Boolean get_IsType()
FSharp.Compiler.CodeAnalysis.FindDeclExternalType: Boolean get_IsTypeVar()
FSharp.Compiler.CodeAnalysis.FindDeclExternalType: FSharp.Compiler.CodeAnalysis.FindDeclExternalType NewArray(FSharp.Compiler.CodeAnalysis.FindDeclExternalType)
FSharp.Compiler.CodeAnalysis.FindDeclExternalType: FSharp.Compiler.CodeAnalysis.FindDeclExternalType NewPointer(FSharp.Compiler.CodeAnalysis.FindDeclExternalType)
FSharp.Compiler.CodeAnalysis.FindDeclExternalType: FSharp.Compiler.CodeAnalysis.FindDeclExternalType NewType(System.String, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FindDeclExternalType])
FSharp.Compiler.CodeAnalysis.FindDeclExternalType: FSharp.Compiler.CodeAnalysis.FindDeclExternalType NewTypeVar(System.String)
FSharp.Compiler.CodeAnalysis.FindDeclExternalType: FSharp.Compiler.CodeAnalysis.FindDeclExternalType+Array
FSharp.Compiler.CodeAnalysis.FindDeclExternalType: FSharp.Compiler.CodeAnalysis.FindDeclExternalType+Pointer
FSharp.Compiler.CodeAnalysis.FindDeclExternalType: FSharp.Compiler.CodeAnalysis.FindDeclExternalType+Tags
FSharp.Compiler.CodeAnalysis.FindDeclExternalType: FSharp.Compiler.CodeAnalysis.FindDeclExternalType+Type
FSharp.Compiler.CodeAnalysis.FindDeclExternalType: FSharp.Compiler.CodeAnalysis.FindDeclExternalType+TypeVar
FSharp.Compiler.CodeAnalysis.FindDeclExternalType: Int32 CompareTo(FSharp.Compiler.CodeAnalysis.FindDeclExternalType)
FSharp.Compiler.CodeAnalysis.FindDeclExternalType: Int32 CompareTo(System.Object)
FSharp.Compiler.CodeAnalysis.FindDeclExternalType: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.CodeAnalysis.FindDeclExternalType: Int32 GetHashCode()
FSharp.Compiler.CodeAnalysis.FindDeclExternalType: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.CodeAnalysis.FindDeclExternalType: Int32 Tag
FSharp.Compiler.CodeAnalysis.FindDeclExternalType: Int32 get_Tag()
FSharp.Compiler.CodeAnalysis.FindDeclExternalType: System.String ToString()
FSharp.Compiler.CodeAnalysis.FSharpExternalTypeModule
FSharp.Compiler.CodeAnalysis.FSharpField
FSharp.Compiler.CodeAnalysis.FSharpField: Boolean Equals(System.Object)
FSharp.Compiler.CodeAnalysis.FSharpField: Boolean IsAnonRecordField
FSharp.Compiler.CodeAnalysis.FSharpField: Boolean IsCompilerGenerated
FSharp.Compiler.CodeAnalysis.FSharpField: Boolean IsDefaultValue
FSharp.Compiler.CodeAnalysis.FSharpField: Boolean IsLiteral
FSharp.Compiler.CodeAnalysis.FSharpField: Boolean IsMutable
FSharp.Compiler.CodeAnalysis.FSharpField: Boolean IsNameGenerated
FSharp.Compiler.CodeAnalysis.FSharpField: Boolean IsStatic
FSharp.Compiler.CodeAnalysis.FSharpField: Boolean IsUnionCaseField
FSharp.Compiler.CodeAnalysis.FSharpField: Boolean IsUnresolved
FSharp.Compiler.CodeAnalysis.FSharpField: Boolean IsVolatile
FSharp.Compiler.CodeAnalysis.FSharpField: Boolean get_IsAnonRecordField()
FSharp.Compiler.CodeAnalysis.FSharpField: Boolean get_IsCompilerGenerated()
FSharp.Compiler.CodeAnalysis.FSharpField: Boolean get_IsDefaultValue()
FSharp.Compiler.CodeAnalysis.FSharpField: Boolean get_IsLiteral()
FSharp.Compiler.CodeAnalysis.FSharpField: Boolean get_IsMutable()
FSharp.Compiler.CodeAnalysis.FSharpField: Boolean get_IsNameGenerated()
FSharp.Compiler.CodeAnalysis.FSharpField: Boolean get_IsStatic()
FSharp.Compiler.CodeAnalysis.FSharpField: Boolean get_IsUnionCaseField()
FSharp.Compiler.CodeAnalysis.FSharpField: Boolean get_IsUnresolved()
FSharp.Compiler.CodeAnalysis.FSharpField: Boolean get_IsVolatile()
FSharp.Compiler.CodeAnalysis.FSharpField: FSharp.Compiler.CodeAnalysis.FSharpAccessibility Accessibility
FSharp.Compiler.CodeAnalysis.FSharpField: FSharp.Compiler.CodeAnalysis.FSharpAccessibility get_Accessibility()
FSharp.Compiler.CodeAnalysis.FSharpField: FSharp.Compiler.CodeAnalysis.FSharpType FieldType
FSharp.Compiler.CodeAnalysis.FSharpField: FSharp.Compiler.CodeAnalysis.FSharpType get_FieldType()
FSharp.Compiler.CodeAnalysis.FSharpField: FSharp.Compiler.Text.Range DeclarationLocation
FSharp.Compiler.CodeAnalysis.FSharpField: FSharp.Compiler.Text.Range get_DeclarationLocation()
FSharp.Compiler.CodeAnalysis.FSharpField: Int32 GetHashCode()
FSharp.Compiler.CodeAnalysis.FSharpField: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpEntity] DeclaringEntity
FSharp.Compiler.CodeAnalysis.FSharpField: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpEntity] get_DeclaringEntity()
FSharp.Compiler.CodeAnalysis.FSharpField: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpUnionCase] DeclaringUnionCase
FSharp.Compiler.CodeAnalysis.FSharpField: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpUnionCase] get_DeclaringUnionCase()
FSharp.Compiler.CodeAnalysis.FSharpField: Microsoft.FSharp.Core.FSharpOption`1[System.Object] LiteralValue
FSharp.Compiler.CodeAnalysis.FSharpField: Microsoft.FSharp.Core.FSharpOption`1[System.Object] get_LiteralValue()
FSharp.Compiler.CodeAnalysis.FSharpField: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpAttribute] FieldAttributes
FSharp.Compiler.CodeAnalysis.FSharpField: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpAttribute] PropertyAttributes
FSharp.Compiler.CodeAnalysis.FSharpField: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpAttribute] get_FieldAttributes()
FSharp.Compiler.CodeAnalysis.FSharpField: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpAttribute] get_PropertyAttributes()
FSharp.Compiler.CodeAnalysis.FSharpField: System.Collections.Generic.IList`1[System.String] ElaboratedXmlDoc
FSharp.Compiler.CodeAnalysis.FSharpField: System.Collections.Generic.IList`1[System.String] XmlDoc
FSharp.Compiler.CodeAnalysis.FSharpField: System.Collections.Generic.IList`1[System.String] get_ElaboratedXmlDoc()
FSharp.Compiler.CodeAnalysis.FSharpField: System.Collections.Generic.IList`1[System.String] get_XmlDoc()
FSharp.Compiler.CodeAnalysis.FSharpField: System.String Name
FSharp.Compiler.CodeAnalysis.FSharpField: System.String ToString()
FSharp.Compiler.CodeAnalysis.FSharpField: System.String XmlDocSig
FSharp.Compiler.CodeAnalysis.FSharpField: System.String get_Name()
FSharp.Compiler.CodeAnalysis.FSharpField: System.String get_XmlDocSig()
FSharp.Compiler.CodeAnalysis.FSharpField: System.Tuple`3[FSharp.Compiler.CodeAnalysis.FSharpAnonRecordTypeDetails,FSharp.Compiler.CodeAnalysis.FSharpType[],System.Int32] AnonRecordFieldDetails
FSharp.Compiler.CodeAnalysis.FSharpField: System.Tuple`3[FSharp.Compiler.CodeAnalysis.FSharpAnonRecordTypeDetails,FSharp.Compiler.CodeAnalysis.FSharpType[],System.Int32] get_AnonRecordFieldDetails()
FSharp.Compiler.CodeAnalysis.FSharpFileUtilities
FSharp.Compiler.CodeAnalysis.FSharpFileUtilities: Boolean isScriptFile(System.String)
FSharp.Compiler.CodeAnalysis.FindDeclFailureReason
FSharp.Compiler.CodeAnalysis.FindDeclFailureReason+ProvidedMember: System.String get_memberName()
FSharp.Compiler.CodeAnalysis.FindDeclFailureReason+ProvidedMember: System.String memberName
FSharp.Compiler.CodeAnalysis.FindDeclFailureReason+ProvidedType: System.String get_typeName()
FSharp.Compiler.CodeAnalysis.FindDeclFailureReason+ProvidedType: System.String typeName
FSharp.Compiler.CodeAnalysis.FindDeclFailureReason+Tags: Int32 NoSourceCode
FSharp.Compiler.CodeAnalysis.FindDeclFailureReason+Tags: Int32 ProvidedMember
FSharp.Compiler.CodeAnalysis.FindDeclFailureReason+Tags: Int32 ProvidedType
FSharp.Compiler.CodeAnalysis.FindDeclFailureReason+Tags: Int32 Unknown
FSharp.Compiler.CodeAnalysis.FindDeclFailureReason+Unknown: System.String get_message()
FSharp.Compiler.CodeAnalysis.FindDeclFailureReason+Unknown: System.String message
FSharp.Compiler.CodeAnalysis.FindDeclFailureReason: Boolean Equals(FSharp.Compiler.CodeAnalysis.FindDeclFailureReason)
FSharp.Compiler.CodeAnalysis.FindDeclFailureReason: Boolean Equals(System.Object)
FSharp.Compiler.CodeAnalysis.FindDeclFailureReason: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.CodeAnalysis.FindDeclFailureReason: Boolean IsNoSourceCode
FSharp.Compiler.CodeAnalysis.FindDeclFailureReason: Boolean IsProvidedMember
FSharp.Compiler.CodeAnalysis.FindDeclFailureReason: Boolean IsProvidedType
FSharp.Compiler.CodeAnalysis.FindDeclFailureReason: Boolean IsUnknown
FSharp.Compiler.CodeAnalysis.FindDeclFailureReason: Boolean get_IsNoSourceCode()
FSharp.Compiler.CodeAnalysis.FindDeclFailureReason: Boolean get_IsProvidedMember()
FSharp.Compiler.CodeAnalysis.FindDeclFailureReason: Boolean get_IsProvidedType()
FSharp.Compiler.CodeAnalysis.FindDeclFailureReason: Boolean get_IsUnknown()
FSharp.Compiler.CodeAnalysis.FindDeclFailureReason: FSharp.Compiler.CodeAnalysis.FindDeclFailureReason NewProvidedMember(System.String)
FSharp.Compiler.CodeAnalysis.FindDeclFailureReason: FSharp.Compiler.CodeAnalysis.FindDeclFailureReason NewProvidedType(System.String)
FSharp.Compiler.CodeAnalysis.FindDeclFailureReason: FSharp.Compiler.CodeAnalysis.FindDeclFailureReason NewUnknown(System.String)
FSharp.Compiler.CodeAnalysis.FindDeclFailureReason: FSharp.Compiler.CodeAnalysis.FindDeclFailureReason NoSourceCode
FSharp.Compiler.CodeAnalysis.FindDeclFailureReason: FSharp.Compiler.CodeAnalysis.FindDeclFailureReason get_NoSourceCode()
FSharp.Compiler.CodeAnalysis.FindDeclFailureReason: FSharp.Compiler.CodeAnalysis.FindDeclFailureReason+ProvidedMember
FSharp.Compiler.CodeAnalysis.FindDeclFailureReason: FSharp.Compiler.CodeAnalysis.FindDeclFailureReason+ProvidedType
FSharp.Compiler.CodeAnalysis.FindDeclFailureReason: FSharp.Compiler.CodeAnalysis.FindDeclFailureReason+Tags
FSharp.Compiler.CodeAnalysis.FindDeclFailureReason: FSharp.Compiler.CodeAnalysis.FindDeclFailureReason+Unknown
FSharp.Compiler.CodeAnalysis.FindDeclFailureReason: Int32 CompareTo(FSharp.Compiler.CodeAnalysis.FindDeclFailureReason)
FSharp.Compiler.CodeAnalysis.FindDeclFailureReason: Int32 CompareTo(System.Object)
FSharp.Compiler.CodeAnalysis.FindDeclFailureReason: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.CodeAnalysis.FindDeclFailureReason: Int32 GetHashCode()
FSharp.Compiler.CodeAnalysis.FindDeclFailureReason: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.CodeAnalysis.FindDeclFailureReason: Int32 Tag
FSharp.Compiler.CodeAnalysis.FindDeclFailureReason: Int32 get_Tag()
FSharp.Compiler.CodeAnalysis.FindDeclFailureReason: System.String ToString()
FSharp.Compiler.CodeAnalysis.FindDeclResult
FSharp.Compiler.CodeAnalysis.FindDeclResult+DeclFound: FSharp.Compiler.Text.Range get_location()
FSharp.Compiler.CodeAnalysis.FindDeclResult+DeclFound: FSharp.Compiler.Text.Range location
FSharp.Compiler.CodeAnalysis.FindDeclResult+DeclNotFound: FSharp.Compiler.CodeAnalysis.FindDeclFailureReason Item
FSharp.Compiler.CodeAnalysis.FindDeclResult+DeclNotFound: FSharp.Compiler.CodeAnalysis.FindDeclFailureReason get_Item()
FSharp.Compiler.CodeAnalysis.FindDeclResult+ExternalDecl: FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol externalSym
FSharp.Compiler.CodeAnalysis.FindDeclResult+ExternalDecl: FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol get_externalSym()
FSharp.Compiler.CodeAnalysis.FindDeclResult+ExternalDecl: System.String assembly
FSharp.Compiler.CodeAnalysis.FindDeclResult+ExternalDecl: System.String get_assembly()
FSharp.Compiler.CodeAnalysis.FindDeclResult+Tags: Int32 DeclFound
FSharp.Compiler.CodeAnalysis.FindDeclResult+Tags: Int32 DeclNotFound
FSharp.Compiler.CodeAnalysis.FindDeclResult+Tags: Int32 ExternalDecl
FSharp.Compiler.CodeAnalysis.FindDeclResult: Boolean Equals(FSharp.Compiler.CodeAnalysis.FindDeclResult)
FSharp.Compiler.CodeAnalysis.FindDeclResult: Boolean Equals(System.Object)
FSharp.Compiler.CodeAnalysis.FindDeclResult: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.CodeAnalysis.FindDeclResult: Boolean IsDeclFound
FSharp.Compiler.CodeAnalysis.FindDeclResult: Boolean IsDeclNotFound
FSharp.Compiler.CodeAnalysis.FindDeclResult: Boolean IsExternalDecl
FSharp.Compiler.CodeAnalysis.FindDeclResult: Boolean get_IsDeclFound()
FSharp.Compiler.CodeAnalysis.FindDeclResult: Boolean get_IsDeclNotFound()
FSharp.Compiler.CodeAnalysis.FindDeclResult: Boolean get_IsExternalDecl()
FSharp.Compiler.CodeAnalysis.FindDeclResult: FSharp.Compiler.CodeAnalysis.FindDeclResult NewDeclFound(FSharp.Compiler.Text.Range)
FSharp.Compiler.CodeAnalysis.FindDeclResult: FSharp.Compiler.CodeAnalysis.FindDeclResult NewDeclNotFound(FSharp.Compiler.CodeAnalysis.FindDeclFailureReason)
FSharp.Compiler.CodeAnalysis.FindDeclResult: FSharp.Compiler.CodeAnalysis.FindDeclResult NewExternalDecl(System.String, FSharp.Compiler.CodeAnalysis.FindDeclExternalSymbol)
FSharp.Compiler.CodeAnalysis.FindDeclResult: FSharp.Compiler.CodeAnalysis.FindDeclResult+DeclFound
FSharp.Compiler.CodeAnalysis.FindDeclResult: FSharp.Compiler.CodeAnalysis.FindDeclResult+DeclNotFound
FSharp.Compiler.CodeAnalysis.FindDeclResult: FSharp.Compiler.CodeAnalysis.FindDeclResult+ExternalDecl
FSharp.Compiler.CodeAnalysis.FindDeclResult: FSharp.Compiler.CodeAnalysis.FindDeclResult+Tags
FSharp.Compiler.CodeAnalysis.FindDeclResult: Int32 GetHashCode()
FSharp.Compiler.CodeAnalysis.FindDeclResult: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.CodeAnalysis.FindDeclResult: Int32 Tag
FSharp.Compiler.CodeAnalysis.FindDeclResult: Int32 get_Tag()
FSharp.Compiler.CodeAnalysis.FindDeclResult: System.String ToString()
FSharp.Compiler.CodeAnalysis.FSharpGenericParameter
FSharp.Compiler.CodeAnalysis.FSharpGenericParameter: Boolean Equals(System.Object)
FSharp.Compiler.CodeAnalysis.FSharpGenericParameter: Boolean IsCompilerGenerated
FSharp.Compiler.CodeAnalysis.FSharpGenericParameter: Boolean IsMeasure
FSharp.Compiler.CodeAnalysis.FSharpGenericParameter: Boolean IsSolveAtCompileTime
FSharp.Compiler.CodeAnalysis.FSharpGenericParameter: Boolean get_IsCompilerGenerated()
FSharp.Compiler.CodeAnalysis.FSharpGenericParameter: Boolean get_IsMeasure()
FSharp.Compiler.CodeAnalysis.FSharpGenericParameter: Boolean get_IsSolveAtCompileTime()
FSharp.Compiler.CodeAnalysis.FSharpGenericParameter: FSharp.Compiler.Text.Range DeclarationLocation
FSharp.Compiler.CodeAnalysis.FSharpGenericParameter: FSharp.Compiler.Text.Range get_DeclarationLocation()
FSharp.Compiler.CodeAnalysis.FSharpGenericParameter: Int32 GetHashCode()
FSharp.Compiler.CodeAnalysis.FSharpGenericParameter: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpAttribute] Attributes
FSharp.Compiler.CodeAnalysis.FSharpGenericParameter: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpAttribute] get_Attributes()
FSharp.Compiler.CodeAnalysis.FSharpGenericParameter: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpGenericParameterConstraint] Constraints
FSharp.Compiler.CodeAnalysis.FSharpGenericParameter: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpGenericParameterConstraint] get_Constraints()
FSharp.Compiler.CodeAnalysis.FSharpGenericParameter: System.Collections.Generic.IList`1[System.String] ElaboratedXmlDoc
FSharp.Compiler.CodeAnalysis.FSharpGenericParameter: System.Collections.Generic.IList`1[System.String] XmlDoc
FSharp.Compiler.CodeAnalysis.FSharpGenericParameter: System.Collections.Generic.IList`1[System.String] get_ElaboratedXmlDoc()
FSharp.Compiler.CodeAnalysis.FSharpGenericParameter: System.Collections.Generic.IList`1[System.String] get_XmlDoc()
FSharp.Compiler.CodeAnalysis.FSharpGenericParameter: System.String Name
FSharp.Compiler.CodeAnalysis.FSharpGenericParameter: System.String ToString()
FSharp.Compiler.CodeAnalysis.FSharpGenericParameter: System.String get_Name()
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterConstraint
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterConstraint: Boolean IsCoercesToConstraint
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterConstraint: Boolean IsComparisonConstraint
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterConstraint: Boolean IsDefaultsToConstraint
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterConstraint: Boolean IsDelegateConstraint
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterConstraint: Boolean IsEnumConstraint
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterConstraint: Boolean IsEqualityConstraint
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterConstraint: Boolean IsMemberConstraint
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterConstraint: Boolean IsNonNullableValueTypeConstraint
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterConstraint: Boolean IsReferenceTypeConstraint
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterConstraint: Boolean IsRequiresDefaultConstructorConstraint
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterConstraint: Boolean IsSimpleChoiceConstraint
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterConstraint: Boolean IsSupportsNullConstraint
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterConstraint: Boolean IsUnmanagedConstraint
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterConstraint: Boolean get_IsCoercesToConstraint()
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterConstraint: Boolean get_IsComparisonConstraint()
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterConstraint: Boolean get_IsDefaultsToConstraint()
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterConstraint: Boolean get_IsDelegateConstraint()
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterConstraint: Boolean get_IsEnumConstraint()
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterConstraint: Boolean get_IsEqualityConstraint()
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterConstraint: Boolean get_IsMemberConstraint()
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterConstraint: Boolean get_IsNonNullableValueTypeConstraint()
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterConstraint: Boolean get_IsReferenceTypeConstraint()
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterConstraint: Boolean get_IsRequiresDefaultConstructorConstraint()
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterConstraint: Boolean get_IsSimpleChoiceConstraint()
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterConstraint: Boolean get_IsSupportsNullConstraint()
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterConstraint: Boolean get_IsUnmanagedConstraint()
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterConstraint: FSharp.Compiler.CodeAnalysis.FSharpGenericParameterDefaultsToConstraint DefaultsToConstraintData
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterConstraint: FSharp.Compiler.CodeAnalysis.FSharpGenericParameterDefaultsToConstraint get_DefaultsToConstraintData()
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterConstraint: FSharp.Compiler.CodeAnalysis.FSharpGenericParameterDelegateConstraint DelegateConstraintData
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterConstraint: FSharp.Compiler.CodeAnalysis.FSharpGenericParameterDelegateConstraint get_DelegateConstraintData()
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterConstraint: FSharp.Compiler.CodeAnalysis.FSharpGenericParameterMemberConstraint MemberConstraintData
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterConstraint: FSharp.Compiler.CodeAnalysis.FSharpGenericParameterMemberConstraint get_MemberConstraintData()
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterConstraint: FSharp.Compiler.CodeAnalysis.FSharpType CoercesToTarget
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterConstraint: FSharp.Compiler.CodeAnalysis.FSharpType EnumConstraintTarget
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterConstraint: FSharp.Compiler.CodeAnalysis.FSharpType get_CoercesToTarget()
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterConstraint: FSharp.Compiler.CodeAnalysis.FSharpType get_EnumConstraintTarget()
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterConstraint: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpType] SimpleChoices
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterConstraint: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpType] get_SimpleChoices()
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterConstraint: System.String ToString()
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterDefaultsToConstraint
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterDefaultsToConstraint: FSharp.Compiler.CodeAnalysis.FSharpType DefaultsToTarget
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterDefaultsToConstraint: FSharp.Compiler.CodeAnalysis.FSharpType get_DefaultsToTarget()
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterDefaultsToConstraint: Int32 DefaultsToPriority
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterDefaultsToConstraint: Int32 get_DefaultsToPriority()
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterDefaultsToConstraint: System.String ToString()
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterDelegateConstraint
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterDelegateConstraint: FSharp.Compiler.CodeAnalysis.FSharpType DelegateReturnType
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterDelegateConstraint: FSharp.Compiler.CodeAnalysis.FSharpType DelegateTupledArgumentType
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterDelegateConstraint: FSharp.Compiler.CodeAnalysis.FSharpType get_DelegateReturnType()
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterDelegateConstraint: FSharp.Compiler.CodeAnalysis.FSharpType get_DelegateTupledArgumentType()
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterDelegateConstraint: System.String ToString()
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterMemberConstraint
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterMemberConstraint: Boolean MemberIsStatic
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterMemberConstraint: Boolean get_MemberIsStatic()
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterMemberConstraint: FSharp.Compiler.CodeAnalysis.FSharpType MemberReturnType
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterMemberConstraint: FSharp.Compiler.CodeAnalysis.FSharpType get_MemberReturnType()
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterMemberConstraint: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpType] MemberArgumentTypes
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterMemberConstraint: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpType] MemberSources
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterMemberConstraint: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpType] get_MemberArgumentTypes()
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterMemberConstraint: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpType] get_MemberSources()
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterMemberConstraint: System.String MemberName
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterMemberConstraint: System.String ToString()
FSharp.Compiler.CodeAnalysis.FSharpGenericParameterMemberConstraint: System.String get_MemberName()
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileContents
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileContents: Boolean HasExplicitEntryPoint
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileContents: Boolean IsScript
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileContents: Boolean get_HasExplicitEntryPoint()
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileContents: Boolean get_IsScript()
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileContents: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration] Declarations
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileContents: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration] get_Declarations()
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileContents: System.String FileName
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileContents: System.String QualifiedName
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileContents: System.String get_FileName()
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileContents: System.String get_QualifiedName()
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration+Entity: FSharp.Compiler.CodeAnalysis.FSharpEntity entity
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration+Entity: FSharp.Compiler.CodeAnalysis.FSharpEntity get_entity()
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration+Entity: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration] declarations
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration+Entity: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration] get_declarations()
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration+InitAction: FSharp.Compiler.CodeAnalysis.FSharpExpr action
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration+InitAction: FSharp.Compiler.CodeAnalysis.FSharpExpr get_action()
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration+MemberOrFunctionOrValue: FSharp.Compiler.CodeAnalysis.FSharpExpr body
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration+MemberOrFunctionOrValue: FSharp.Compiler.CodeAnalysis.FSharpExpr get_body()
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration+MemberOrFunctionOrValue: FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue get_value()
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration+MemberOrFunctionOrValue: FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue value
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration+MemberOrFunctionOrValue: Microsoft.FSharp.Collections.FSharpList`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue]] curriedArgs
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration+MemberOrFunctionOrValue: Microsoft.FSharp.Collections.FSharpList`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue]] get_curriedArgs()
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration+Tags: Int32 Entity
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration+Tags: Int32 InitAction
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration+Tags: Int32 MemberOrFunctionOrValue
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration: Boolean Equals(FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration)
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration: Boolean Equals(System.Object)
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration: Boolean IsEntity
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration: Boolean IsInitAction
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration: Boolean IsMemberOrFunctionOrValue
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration: Boolean get_IsEntity()
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration: Boolean get_IsInitAction()
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration: Boolean get_IsMemberOrFunctionOrValue()
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration: FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration NewEntity(FSharp.Compiler.CodeAnalysis.FSharpEntity, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration])
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration: FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration NewInitAction(FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration: FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration NewMemberOrFunctionOrValue(FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue, Microsoft.FSharp.Collections.FSharpList`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue]], FSharp.Compiler.CodeAnalysis.FSharpExpr)
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration: FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration+Entity
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration: FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration+InitAction
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration: FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration+MemberOrFunctionOrValue
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration: FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration+Tags
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration: Int32 GetHashCode()
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration: Int32 Tag
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration: Int32 get_Tag()
FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration: System.String ToString()
FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation
FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation+Tags: Int32 AggressiveInline
FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation+Tags: Int32 AlwaysInline
FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation+Tags: Int32 NeverInline
FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation+Tags: Int32 OptionalInline
FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation+Tags: Int32 PseudoValue
FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation: Boolean Equals(FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation)
FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation: Boolean Equals(System.Object)
FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation: Boolean IsAggressiveInline
FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation: Boolean IsAlwaysInline
FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation: Boolean IsNeverInline
FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation: Boolean IsOptionalInline
FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation: Boolean IsPseudoValue
FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation: Boolean get_IsAggressiveInline()
FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation: Boolean get_IsAlwaysInline()
FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation: Boolean get_IsNeverInline()
FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation: Boolean get_IsOptionalInline()
FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation: Boolean get_IsPseudoValue()
FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation: FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation AggressiveInline
FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation: FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation AlwaysInline
FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation: FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation NeverInline
FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation: FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation OptionalInline
FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation: FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation PseudoValue
FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation: FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation get_AggressiveInline()
FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation: FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation get_AlwaysInline()
FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation: FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation get_NeverInline()
FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation: FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation get_OptionalInline()
FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation: FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation get_PseudoValue()
FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation: FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation+Tags
FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation: Int32 CompareTo(FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation)
FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation: Int32 CompareTo(System.Object)
FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation: Int32 GetHashCode()
FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation: Int32 Tag
FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation: Int32 get_Tag()
FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation: System.String ToString()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean Equals(System.Object)
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean EventIsStandard
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean HasGetterMethod
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean HasSetterMethod
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean IsActivePattern
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean IsBaseValue
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean IsCompilerGenerated
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean IsConstructor
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean IsConstructorThisValue
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean IsDispatchSlot
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean IsEvent
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean IsEventAddMethod
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean IsEventRemoveMethod
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean IsExplicitInterfaceImplementation
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean IsExtensionMember
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean IsGetterMethod
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean IsImplicitConstructor
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean IsInstanceMember
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean IsInstanceMemberInCompiledCode
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean IsMember
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean IsMemberThisValue
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean IsModuleValueOrMember
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean IsMutable
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean IsOverrideOrExplicitInterfaceImplementation
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean IsOverrideOrExplicitMember
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean IsProperty
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean IsPropertyGetterMethod
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean IsPropertySetterMethod
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean IsSetterMethod
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean IsTypeFunction
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean IsUnresolved
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean IsValCompiledAsMethod
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean IsValue
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean get_EventIsStandard()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean get_HasGetterMethod()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean get_HasSetterMethod()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean get_IsActivePattern()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean get_IsBaseValue()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean get_IsCompilerGenerated()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean get_IsConstructor()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean get_IsConstructorThisValue()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean get_IsDispatchSlot()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean get_IsEvent()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean get_IsEventAddMethod()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean get_IsEventRemoveMethod()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean get_IsExplicitInterfaceImplementation()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean get_IsExtensionMember()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean get_IsGetterMethod()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean get_IsImplicitConstructor()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean get_IsInstanceMember()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean get_IsInstanceMemberInCompiledCode()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean get_IsMember()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean get_IsMemberThisValue()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean get_IsModuleValueOrMember()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean get_IsMutable()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean get_IsOverrideOrExplicitInterfaceImplementation()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean get_IsOverrideOrExplicitMember()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean get_IsProperty()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean get_IsPropertyGetterMethod()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean get_IsPropertySetterMethod()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean get_IsSetterMethod()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean get_IsTypeFunction()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean get_IsUnresolved()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean get_IsValCompiledAsMethod()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Boolean get_IsValue()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: FSharp.Compiler.CodeAnalysis.FSharpAccessibility Accessibility
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: FSharp.Compiler.CodeAnalysis.FSharpAccessibility get_Accessibility()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: FSharp.Compiler.CodeAnalysis.FSharpEntity ApparentEnclosingEntity
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: FSharp.Compiler.CodeAnalysis.FSharpEntity get_ApparentEnclosingEntity()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation InlineAnnotation
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: FSharp.Compiler.CodeAnalysis.FSharpInlineAnnotation get_InlineAnnotation()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue EventAddMethod
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue EventRemoveMethod
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue GetterMethod
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue SetterMethod
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue get_EventAddMethod()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue get_EventRemoveMethod()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue get_GetterMethod()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue get_SetterMethod()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: FSharp.Compiler.CodeAnalysis.FSharpParameter ReturnParameter
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: FSharp.Compiler.CodeAnalysis.FSharpParameter get_ReturnParameter()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: FSharp.Compiler.CodeAnalysis.FSharpType EventDelegateType
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: FSharp.Compiler.CodeAnalysis.FSharpType FullType
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: FSharp.Compiler.CodeAnalysis.FSharpType get_EventDelegateType()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: FSharp.Compiler.CodeAnalysis.FSharpType get_FullType()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: FSharp.Compiler.Text.Range DeclarationLocation
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: FSharp.Compiler.Text.Range get_DeclarationLocation()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: FSharp.Compiler.TextLayout.Layout FormatLayout(FSharp.Compiler.CodeAnalysis.FSharpDisplayContext)
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Int32 GetHashCode()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpEntity] DeclaringEntity
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpEntity] get_DeclaringEntity()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue] EventForFSharpProperty
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue] get_EventForFSharpProperty()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpType] FullTypeSafe
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpType] get_FullTypeSafe()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Microsoft.FSharp.Core.FSharpOption`1[System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue]] Overloads(Boolean)
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Microsoft.FSharp.Core.FSharpOption`1[System.Object] LiteralValue
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Microsoft.FSharp.Core.FSharpOption`1[System.Object] get_LiteralValue()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Microsoft.FSharp.Core.FSharpOption`1[System.String[]] TryGetFullCompiledOperatorNameIdents()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Microsoft.FSharp.Core.FSharpOption`1[System.String] TryGetFullDisplayName()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[System.String,System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpParameter]]] GetWitnessPassingInfo()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpAbstractSignature] ImplementedAbstractSignatures
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpAbstractSignature] get_ImplementedAbstractSignatures()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpAttribute] Attributes
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpAttribute] get_Attributes()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpGenericParameter] GenericParameters
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpGenericParameter] get_GenericParameters()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: System.Collections.Generic.IList`1[System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpParameter]] CurriedParameterGroups
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: System.Collections.Generic.IList`1[System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpParameter]] get_CurriedParameterGroups()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: System.Collections.Generic.IList`1[System.String] ElaboratedXmlDoc
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: System.Collections.Generic.IList`1[System.String] XmlDoc
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: System.Collections.Generic.IList`1[System.String] get_ElaboratedXmlDoc()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: System.Collections.Generic.IList`1[System.String] get_XmlDoc()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: System.String CompiledName
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: System.String DisplayName
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: System.String LogicalName
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: System.String ToString()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: System.String XmlDocSig
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: System.String get_CompiledName()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: System.String get_DisplayName()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: System.String get_LogicalName()
FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue: System.String get_XmlDocSig()
FSharp.Compiler.CodeAnalysis.FSharpObjectExprOverride
FSharp.Compiler.CodeAnalysis.FSharpObjectExprOverride: FSharp.Compiler.CodeAnalysis.FSharpAbstractSignature Signature
FSharp.Compiler.CodeAnalysis.FSharpObjectExprOverride: FSharp.Compiler.CodeAnalysis.FSharpAbstractSignature get_Signature()
FSharp.Compiler.CodeAnalysis.FSharpObjectExprOverride: FSharp.Compiler.CodeAnalysis.FSharpExpr Body
FSharp.Compiler.CodeAnalysis.FSharpObjectExprOverride: FSharp.Compiler.CodeAnalysis.FSharpExpr get_Body()
FSharp.Compiler.CodeAnalysis.FSharpObjectExprOverride: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpGenericParameter] GenericParameters
FSharp.Compiler.CodeAnalysis.FSharpObjectExprOverride: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpGenericParameter] get_GenericParameters()
FSharp.Compiler.CodeAnalysis.FSharpObjectExprOverride: Microsoft.FSharp.Collections.FSharpList`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue]] CurriedParameterGroups
FSharp.Compiler.CodeAnalysis.FSharpObjectExprOverride: Microsoft.FSharp.Collections.FSharpList`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue]] get_CurriedParameterGroups()
FSharp.Compiler.CodeAnalysis.FSharpOpenDeclaration
FSharp.Compiler.CodeAnalysis.FSharpOpenDeclaration: Boolean IsOwnNamespace
FSharp.Compiler.CodeAnalysis.FSharpOpenDeclaration: Boolean get_IsOwnNamespace()
FSharp.Compiler.CodeAnalysis.FSharpOpenDeclaration: FSharp.Compiler.Syntax.SynOpenDeclTarget Target
FSharp.Compiler.CodeAnalysis.FSharpOpenDeclaration: FSharp.Compiler.Syntax.SynOpenDeclTarget get_Target()
FSharp.Compiler.CodeAnalysis.FSharpOpenDeclaration: FSharp.Compiler.Text.Range AppliedScope
FSharp.Compiler.CodeAnalysis.FSharpOpenDeclaration: FSharp.Compiler.Text.Range get_AppliedScope()
FSharp.Compiler.CodeAnalysis.FSharpOpenDeclaration: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpEntity] Modules
FSharp.Compiler.CodeAnalysis.FSharpOpenDeclaration: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpEntity] get_Modules()
FSharp.Compiler.CodeAnalysis.FSharpOpenDeclaration: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpType] Types
FSharp.Compiler.CodeAnalysis.FSharpOpenDeclaration: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpType] get_Types()
FSharp.Compiler.CodeAnalysis.FSharpOpenDeclaration: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident] LongId
FSharp.Compiler.CodeAnalysis.FSharpOpenDeclaration: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident] get_LongId()
FSharp.Compiler.CodeAnalysis.FSharpOpenDeclaration: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] Range
FSharp.Compiler.CodeAnalysis.FSharpOpenDeclaration: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] get_Range()
FSharp.Compiler.CodeAnalysis.FSharpParameter
FSharp.Compiler.CodeAnalysis.FSharpParameter: Boolean Equals(System.Object)
FSharp.Compiler.CodeAnalysis.FSharpParameter: Boolean IsInArg
FSharp.Compiler.CodeAnalysis.FSharpParameter: Boolean IsOptionalArg
FSharp.Compiler.CodeAnalysis.FSharpParameter: Boolean IsOutArg
FSharp.Compiler.CodeAnalysis.FSharpParameter: Boolean IsParamArrayArg
FSharp.Compiler.CodeAnalysis.FSharpParameter: Boolean get_IsInArg()
FSharp.Compiler.CodeAnalysis.FSharpParameter: Boolean get_IsOptionalArg()
FSharp.Compiler.CodeAnalysis.FSharpParameter: Boolean get_IsOutArg()
FSharp.Compiler.CodeAnalysis.FSharpParameter: Boolean get_IsParamArrayArg()
FSharp.Compiler.CodeAnalysis.FSharpParameter: FSharp.Compiler.CodeAnalysis.FSharpType Type
FSharp.Compiler.CodeAnalysis.FSharpParameter: FSharp.Compiler.CodeAnalysis.FSharpType get_Type()
FSharp.Compiler.CodeAnalysis.FSharpParameter: FSharp.Compiler.Text.Range DeclarationLocation
FSharp.Compiler.CodeAnalysis.FSharpParameter: FSharp.Compiler.Text.Range get_DeclarationLocation()
FSharp.Compiler.CodeAnalysis.FSharpParameter: Int32 GetHashCode()
FSharp.Compiler.CodeAnalysis.FSharpParameter: Microsoft.FSharp.Core.FSharpOption`1[System.String] Name
FSharp.Compiler.CodeAnalysis.FSharpParameter: Microsoft.FSharp.Core.FSharpOption`1[System.String] get_Name()
FSharp.Compiler.CodeAnalysis.FSharpParameter: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpAttribute] Attributes
FSharp.Compiler.CodeAnalysis.FSharpParameter: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpAttribute] get_Attributes()
FSharp.Compiler.CodeAnalysis.FSharpParameter: System.String ToString()
FSharp.Compiler.CodeAnalysis.FSharpParseFileResults
FSharp.Compiler.CodeAnalysis.FSharpParseFileResults: Boolean IsPosContainedInApplication(FSharp.Compiler.Text.Position)
FSharp.Compiler.CodeAnalysis.FSharpParseFileResults: Boolean IsPositionContainedInACurriedParameter(FSharp.Compiler.Text.Position)
FSharp.Compiler.CodeAnalysis.FSharpParseFileResults: Boolean ParseHadErrors
FSharp.Compiler.CodeAnalysis.FSharpParseFileResults: Boolean get_ParseHadErrors()
FSharp.Compiler.CodeAnalysis.FSharpParseFileResults: FSharp.Compiler.Diagnostics.FSharpDiagnostic[] Diagnostics
FSharp.Compiler.CodeAnalysis.FSharpParseFileResults: FSharp.Compiler.Diagnostics.FSharpDiagnostic[] get_Diagnostics()
FSharp.Compiler.CodeAnalysis.FSharpParseFileResults: FSharp.Compiler.EditorServices.NavigationItems GetNavigationItems()
FSharp.Compiler.CodeAnalysis.FSharpParseFileResults: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.EditorServices.ParameterLocations] FindParameterLocations(FSharp.Compiler.Text.Position)
FSharp.Compiler.CodeAnalysis.FSharpParseFileResults: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.ParsedInput] ParseTree
FSharp.Compiler.CodeAnalysis.FSharpParseFileResults: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.ParsedInput] get_ParseTree()
FSharp.Compiler.CodeAnalysis.FSharpParseFileResults: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] TryRangeOfExprInYieldOrReturn(FSharp.Compiler.Text.Position)
FSharp.Compiler.CodeAnalysis.FSharpParseFileResults: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] TryRangeOfFunctionOrMethodBeingApplied(FSharp.Compiler.Text.Position)
FSharp.Compiler.CodeAnalysis.FSharpParseFileResults: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] TryRangeOfNameOfNearestOuterBindingContainingPos(FSharp.Compiler.Text.Position)
FSharp.Compiler.CodeAnalysis.FSharpParseFileResults: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] TryRangeOfRecordExpressionContainingPos(FSharp.Compiler.Text.Position)
FSharp.Compiler.CodeAnalysis.FSharpParseFileResults: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] TryRangeOfRefCellDereferenceContainingPos(FSharp.Compiler.Text.Position)
FSharp.Compiler.CodeAnalysis.FSharpParseFileResults: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] ValidateBreakpointLocation(FSharp.Compiler.Text.Position)
FSharp.Compiler.CodeAnalysis.FSharpParseFileResults: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Text.Range]] GetAllArgumentsForFunctionApplicationAtPostion(FSharp.Compiler.Text.Position)
FSharp.Compiler.CodeAnalysis.FSharpParseFileResults: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.Syntax.Ident,System.Int32]] TryIdentOfPipelineContainingPosAndNumArgsApplied(FSharp.Compiler.Text.Position)
FSharp.Compiler.CodeAnalysis.FSharpParseFileResults: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`3[FSharp.Compiler.Text.Range,FSharp.Compiler.Text.Range,FSharp.Compiler.Text.Range]] TryRangeOfParenEnclosingOpEqualsGreaterUsage(FSharp.Compiler.Text.Position)
FSharp.Compiler.CodeAnalysis.FSharpParseFileResults: System.String FileName
FSharp.Compiler.CodeAnalysis.FSharpParseFileResults: System.String get_FileName()
FSharp.Compiler.CodeAnalysis.FSharpParseFileResults: System.String[] DependencyFiles
FSharp.Compiler.CodeAnalysis.FSharpParseFileResults: System.String[] get_DependencyFiles()
FSharp.Compiler.CodeAnalysis.FSharpParsingOptions
FSharp.Compiler.CodeAnalysis.FSharpParsingOptions: Boolean CompilingFsLib
FSharp.Compiler.CodeAnalysis.FSharpParsingOptions: Boolean Equals(FSharp.Compiler.CodeAnalysis.FSharpParsingOptions)
FSharp.Compiler.CodeAnalysis.FSharpParsingOptions: Boolean Equals(System.Object)
FSharp.Compiler.CodeAnalysis.FSharpParsingOptions: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.CodeAnalysis.FSharpParsingOptions: Boolean IsExe
FSharp.Compiler.CodeAnalysis.FSharpParsingOptions: Boolean IsInteractive
FSharp.Compiler.CodeAnalysis.FSharpParsingOptions: Boolean get_CompilingFsLib()
FSharp.Compiler.CodeAnalysis.FSharpParsingOptions: Boolean get_IsExe()
FSharp.Compiler.CodeAnalysis.FSharpParsingOptions: Boolean get_IsInteractive()
FSharp.Compiler.CodeAnalysis.FSharpParsingOptions: FSharp.Compiler.CodeAnalysis.FSharpParsingOptions Default
FSharp.Compiler.CodeAnalysis.FSharpParsingOptions: FSharp.Compiler.CodeAnalysis.FSharpParsingOptions get_Default()
FSharp.Compiler.CodeAnalysis.FSharpParsingOptions: FSharp.Compiler.Diagnostics.FSharpDiagnosticOptions ErrorSeverityOptions
FSharp.Compiler.CodeAnalysis.FSharpParsingOptions: FSharp.Compiler.Diagnostics.FSharpDiagnosticOptions get_ErrorSeverityOptions()
FSharp.Compiler.CodeAnalysis.FSharpParsingOptions: Int32 CompareTo(FSharp.Compiler.CodeAnalysis.FSharpParsingOptions)
FSharp.Compiler.CodeAnalysis.FSharpParsingOptions: Int32 CompareTo(System.Object)
FSharp.Compiler.CodeAnalysis.FSharpParsingOptions: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.CodeAnalysis.FSharpParsingOptions: Int32 GetHashCode()
FSharp.Compiler.CodeAnalysis.FSharpParsingOptions: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.CodeAnalysis.FSharpParsingOptions: Microsoft.FSharp.Collections.FSharpList`1[System.String] ConditionalCompilationDefines
FSharp.Compiler.CodeAnalysis.FSharpParsingOptions: Microsoft.FSharp.Collections.FSharpList`1[System.String] get_ConditionalCompilationDefines()
FSharp.Compiler.CodeAnalysis.FSharpParsingOptions: Microsoft.FSharp.Core.FSharpOption`1[System.Boolean] LightSyntax
FSharp.Compiler.CodeAnalysis.FSharpParsingOptions: Microsoft.FSharp.Core.FSharpOption`1[System.Boolean] get_LightSyntax()
FSharp.Compiler.CodeAnalysis.FSharpParsingOptions: System.String ToString()
FSharp.Compiler.CodeAnalysis.FSharpParsingOptions: System.String[] SourceFiles
FSharp.Compiler.CodeAnalysis.FSharpParsingOptions: System.String[] get_SourceFiles()
FSharp.Compiler.CodeAnalysis.FSharpParsingOptions: Void .ctor(System.String[], Microsoft.FSharp.Collections.FSharpList`1[System.String], FSharp.Compiler.Diagnostics.FSharpDiagnosticOptions, Boolean, Microsoft.FSharp.Core.FSharpOption`1[System.Boolean], Boolean, Boolean)
FSharp.Compiler.CodeAnalysis.FSharpProjectContext
FSharp.Compiler.CodeAnalysis.FSharpProjectContext: FSharp.Compiler.CodeAnalysis.FSharpAccessibilityRights AccessibilityRights
FSharp.Compiler.CodeAnalysis.FSharpProjectContext: FSharp.Compiler.CodeAnalysis.FSharpAccessibilityRights get_AccessibilityRights()
FSharp.Compiler.CodeAnalysis.FSharpProjectContext: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpAssembly] GetReferencedAssemblies()
FSharp.Compiler.CodeAnalysis.FSharpProjectOptions
FSharp.Compiler.CodeAnalysis.FSharpProjectOptions: Boolean Equals(FSharp.Compiler.CodeAnalysis.FSharpProjectOptions)
FSharp.Compiler.CodeAnalysis.FSharpProjectOptions: Boolean Equals(System.Object)
FSharp.Compiler.CodeAnalysis.FSharpProjectOptions: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.CodeAnalysis.FSharpProjectOptions: Boolean IsIncompleteTypeCheckEnvironment
FSharp.Compiler.CodeAnalysis.FSharpProjectOptions: Boolean UseScriptResolutionRules
FSharp.Compiler.CodeAnalysis.FSharpProjectOptions: Boolean get_IsIncompleteTypeCheckEnvironment()
FSharp.Compiler.CodeAnalysis.FSharpProjectOptions: Boolean get_UseScriptResolutionRules()
FSharp.Compiler.CodeAnalysis.FSharpProjectOptions: Int32 GetHashCode()
FSharp.Compiler.CodeAnalysis.FSharpProjectOptions: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.CodeAnalysis.FSharpProjectOptions: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`3[FSharp.Compiler.Text.Range,System.String,System.String]] OriginalLoadReferences
FSharp.Compiler.CodeAnalysis.FSharpProjectOptions: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`3[FSharp.Compiler.Text.Range,System.String,System.String]] get_OriginalLoadReferences()
FSharp.Compiler.CodeAnalysis.FSharpProjectOptions: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpUnresolvedReferencesSet] UnresolvedReferences
FSharp.Compiler.CodeAnalysis.FSharpProjectOptions: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpUnresolvedReferencesSet] get_UnresolvedReferences()
FSharp.Compiler.CodeAnalysis.FSharpProjectOptions: Microsoft.FSharp.Core.FSharpOption`1[System.Int64] Stamp
FSharp.Compiler.CodeAnalysis.FSharpProjectOptions: Microsoft.FSharp.Core.FSharpOption`1[System.Int64] get_Stamp()
FSharp.Compiler.CodeAnalysis.FSharpProjectOptions: Microsoft.FSharp.Core.FSharpOption`1[System.Object] ExtraProjectInfo
FSharp.Compiler.CodeAnalysis.FSharpProjectOptions: Microsoft.FSharp.Core.FSharpOption`1[System.Object] get_ExtraProjectInfo()
FSharp.Compiler.CodeAnalysis.FSharpProjectOptions: Microsoft.FSharp.Core.FSharpOption`1[System.String] ProjectId
FSharp.Compiler.CodeAnalysis.FSharpProjectOptions: Microsoft.FSharp.Core.FSharpOption`1[System.String] get_ProjectId()
FSharp.Compiler.CodeAnalysis.FSharpProjectOptions: System.DateTime LoadTime
FSharp.Compiler.CodeAnalysis.FSharpProjectOptions: System.DateTime get_LoadTime()
FSharp.Compiler.CodeAnalysis.FSharpProjectOptions: System.String ProjectFileName
FSharp.Compiler.CodeAnalysis.FSharpProjectOptions: System.String ToString()
FSharp.Compiler.CodeAnalysis.FSharpProjectOptions: System.String get_ProjectFileName()
FSharp.Compiler.CodeAnalysis.FSharpProjectOptions: System.String[] OtherOptions
FSharp.Compiler.CodeAnalysis.FSharpProjectOptions: System.String[] SourceFiles
FSharp.Compiler.CodeAnalysis.FSharpProjectOptions: System.String[] get_OtherOptions()
FSharp.Compiler.CodeAnalysis.FSharpProjectOptions: System.String[] get_SourceFiles()
FSharp.Compiler.CodeAnalysis.FSharpProjectOptions: System.Tuple`2[System.String,FSharp.Compiler.CodeAnalysis.FSharpProjectOptions][] ReferencedProjects
FSharp.Compiler.CodeAnalysis.FSharpProjectOptions: System.Tuple`2[System.String,FSharp.Compiler.CodeAnalysis.FSharpProjectOptions][] get_ReferencedProjects()
FSharp.Compiler.CodeAnalysis.FSharpProjectOptions: Void .ctor(System.String, Microsoft.FSharp.Core.FSharpOption`1[System.String], System.String[], System.String[], System.Tuple`2[System.String,FSharp.Compiler.CodeAnalysis.FSharpProjectOptions][], Boolean, Boolean, System.DateTime, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpUnresolvedReferencesSet], Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`3[FSharp.Compiler.Text.Range,System.String,System.String]], Microsoft.FSharp.Core.FSharpOption`1[System.Object], Microsoft.FSharp.Core.FSharpOption`1[System.Int64])
FSharp.Compiler.CodeAnalysis.SemanticClassificationItem
FSharp.Compiler.CodeAnalysis.SemanticClassificationItem: Boolean Equals(FSharp.Compiler.CodeAnalysis.SemanticClassificationItem)
FSharp.Compiler.CodeAnalysis.SemanticClassificationItem: Boolean Equals(System.Object)
FSharp.Compiler.CodeAnalysis.SemanticClassificationItem: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.CodeAnalysis.SemanticClassificationItem: FSharp.Compiler.CodeAnalysis.SemanticClassificationType Type
FSharp.Compiler.CodeAnalysis.SemanticClassificationItem: FSharp.Compiler.CodeAnalysis.SemanticClassificationType get_Type()
FSharp.Compiler.CodeAnalysis.SemanticClassificationItem: FSharp.Compiler.Text.Range Range
FSharp.Compiler.CodeAnalysis.SemanticClassificationItem: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.CodeAnalysis.SemanticClassificationItem: Int32 GetHashCode()
FSharp.Compiler.CodeAnalysis.SemanticClassificationItem: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.CodeAnalysis.SemanticClassificationItem: Void .ctor(System.Tuple`2[FSharp.Compiler.Text.Range,FSharp.Compiler.CodeAnalysis.SemanticClassificationType])
FSharp.Compiler.CodeAnalysis.SemanticClassificationView
FSharp.Compiler.CodeAnalysis.SemanticClassificationView: Void ForEach(Microsoft.FSharp.Core.FSharpFunc`2[FSharp.Compiler.CodeAnalysis.SemanticClassificationItem,Microsoft.FSharp.Core.Unit])
FSharp.Compiler.CodeAnalysis.FSharpStaticParameter
FSharp.Compiler.CodeAnalysis.FSharpStaticParameter: Boolean Equals(System.Object)
FSharp.Compiler.CodeAnalysis.FSharpStaticParameter: Boolean HasDefaultValue
FSharp.Compiler.CodeAnalysis.FSharpStaticParameter: Boolean IsOptional
FSharp.Compiler.CodeAnalysis.FSharpStaticParameter: Boolean get_HasDefaultValue()
FSharp.Compiler.CodeAnalysis.FSharpStaticParameter: Boolean get_IsOptional()
FSharp.Compiler.CodeAnalysis.FSharpStaticParameter: FSharp.Compiler.CodeAnalysis.FSharpType Kind
FSharp.Compiler.CodeAnalysis.FSharpStaticParameter: FSharp.Compiler.CodeAnalysis.FSharpType get_Kind()
FSharp.Compiler.CodeAnalysis.FSharpStaticParameter: FSharp.Compiler.Text.Range DeclarationLocation
FSharp.Compiler.CodeAnalysis.FSharpStaticParameter: FSharp.Compiler.Text.Range Range
FSharp.Compiler.CodeAnalysis.FSharpStaticParameter: FSharp.Compiler.Text.Range get_DeclarationLocation()
FSharp.Compiler.CodeAnalysis.FSharpStaticParameter: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.CodeAnalysis.FSharpStaticParameter: Int32 GetHashCode()
FSharp.Compiler.CodeAnalysis.FSharpStaticParameter: System.Object DefaultValue
FSharp.Compiler.CodeAnalysis.FSharpStaticParameter: System.Object get_DefaultValue()
FSharp.Compiler.CodeAnalysis.FSharpStaticParameter: System.String Name
FSharp.Compiler.CodeAnalysis.FSharpStaticParameter: System.String ToString()
FSharp.Compiler.CodeAnalysis.FSharpStaticParameter: System.String get_Name()
FSharp.Compiler.CodeAnalysis.FSharpSymbol
FSharp.Compiler.CodeAnalysis.FSharpSymbol: Boolean Equals(System.Object)
FSharp.Compiler.CodeAnalysis.FSharpSymbol: Boolean IsAccessible(FSharp.Compiler.CodeAnalysis.FSharpAccessibilityRights)
FSharp.Compiler.CodeAnalysis.FSharpSymbol: Boolean IsEffectivelySameAs(FSharp.Compiler.CodeAnalysis.FSharpSymbol)
FSharp.Compiler.CodeAnalysis.FSharpSymbol: Boolean IsExplicitlySuppressed
FSharp.Compiler.CodeAnalysis.FSharpSymbol: Boolean get_IsExplicitlySuppressed()
FSharp.Compiler.CodeAnalysis.FSharpSymbol: FSharp.Compiler.CodeAnalysis.FSharpAssembly Assembly
FSharp.Compiler.CodeAnalysis.FSharpSymbol: FSharp.Compiler.CodeAnalysis.FSharpAssembly get_Assembly()
FSharp.Compiler.CodeAnalysis.FSharpSymbol: Int32 GetEffectivelySameAsHash()
FSharp.Compiler.CodeAnalysis.FSharpSymbol: Int32 GetHashCode()
FSharp.Compiler.CodeAnalysis.FSharpSymbol: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpAccessibility] GetAccessibility(FSharp.Compiler.CodeAnalysis.FSharpSymbol)
FSharp.Compiler.CodeAnalysis.FSharpSymbol: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] DeclarationLocation
FSharp.Compiler.CodeAnalysis.FSharpSymbol: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] ImplementationLocation
FSharp.Compiler.CodeAnalysis.FSharpSymbol: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] SignatureLocation
FSharp.Compiler.CodeAnalysis.FSharpSymbol: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] get_DeclarationLocation()
FSharp.Compiler.CodeAnalysis.FSharpSymbol: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] get_ImplementationLocation()
FSharp.Compiler.CodeAnalysis.FSharpSymbol: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] get_SignatureLocation()
FSharp.Compiler.CodeAnalysis.FSharpSymbol: System.String DisplayName
FSharp.Compiler.CodeAnalysis.FSharpSymbol: System.String FullName
FSharp.Compiler.CodeAnalysis.FSharpSymbol: System.String ToString()
FSharp.Compiler.CodeAnalysis.FSharpSymbol: System.String get_DisplayName()
FSharp.Compiler.CodeAnalysis.FSharpSymbol: System.String get_FullName()
FSharp.Compiler.CodeAnalysis.FSharpSymbolUse
FSharp.Compiler.CodeAnalysis.FSharpSymbolUse: Boolean IsFromAttribute
FSharp.Compiler.CodeAnalysis.FSharpSymbolUse: Boolean IsFromComputationExpression
FSharp.Compiler.CodeAnalysis.FSharpSymbolUse: Boolean IsFromDefinition
FSharp.Compiler.CodeAnalysis.FSharpSymbolUse: Boolean IsFromDispatchSlotImplementation
FSharp.Compiler.CodeAnalysis.FSharpSymbolUse: Boolean IsFromOpenStatement
FSharp.Compiler.CodeAnalysis.FSharpSymbolUse: Boolean IsFromPattern
FSharp.Compiler.CodeAnalysis.FSharpSymbolUse: Boolean IsFromType
FSharp.Compiler.CodeAnalysis.FSharpSymbolUse: Boolean IsPrivateToFile
FSharp.Compiler.CodeAnalysis.FSharpSymbolUse: Boolean get_IsFromAttribute()
FSharp.Compiler.CodeAnalysis.FSharpSymbolUse: Boolean get_IsFromComputationExpression()
FSharp.Compiler.CodeAnalysis.FSharpSymbolUse: Boolean get_IsFromDefinition()
FSharp.Compiler.CodeAnalysis.FSharpSymbolUse: Boolean get_IsFromDispatchSlotImplementation()
FSharp.Compiler.CodeAnalysis.FSharpSymbolUse: Boolean get_IsFromOpenStatement()
FSharp.Compiler.CodeAnalysis.FSharpSymbolUse: Boolean get_IsFromPattern()
FSharp.Compiler.CodeAnalysis.FSharpSymbolUse: Boolean get_IsFromType()
FSharp.Compiler.CodeAnalysis.FSharpSymbolUse: Boolean get_IsPrivateToFile()
FSharp.Compiler.CodeAnalysis.FSharpSymbolUse: FSharp.Compiler.CodeAnalysis.FSharpDisplayContext DisplayContext
FSharp.Compiler.CodeAnalysis.FSharpSymbolUse: FSharp.Compiler.CodeAnalysis.FSharpDisplayContext get_DisplayContext()
FSharp.Compiler.CodeAnalysis.FSharpSymbolUse: FSharp.Compiler.CodeAnalysis.FSharpSymbol Symbol
FSharp.Compiler.CodeAnalysis.FSharpSymbolUse: FSharp.Compiler.CodeAnalysis.FSharpSymbol get_Symbol()
FSharp.Compiler.CodeAnalysis.FSharpSymbolUse: FSharp.Compiler.Text.Range Range
FSharp.Compiler.CodeAnalysis.FSharpSymbolUse: FSharp.Compiler.Text.Range get_RangeAlternate()
FSharp.Compiler.CodeAnalysis.FSharpSymbolUse: System.String FileName
FSharp.Compiler.CodeAnalysis.FSharpSymbolUse: System.String ToString()
FSharp.Compiler.CodeAnalysis.FSharpSymbolUse: System.String get_FileName()
FSharp.Compiler.CodeAnalysis.FSharpType
FSharp.Compiler.CodeAnalysis.FSharpType: Boolean Equals(System.Object)
FSharp.Compiler.CodeAnalysis.FSharpType: Boolean HasTypeDefinition
FSharp.Compiler.CodeAnalysis.FSharpType: Boolean IsAbbreviation
FSharp.Compiler.CodeAnalysis.FSharpType: Boolean IsAnonRecordType
FSharp.Compiler.CodeAnalysis.FSharpType: Boolean IsFunctionType
FSharp.Compiler.CodeAnalysis.FSharpType: Boolean IsGenericParameter
FSharp.Compiler.CodeAnalysis.FSharpType: Boolean IsNamedType
FSharp.Compiler.CodeAnalysis.FSharpType: Boolean IsStructTupleType
FSharp.Compiler.CodeAnalysis.FSharpType: Boolean IsTupleType
FSharp.Compiler.CodeAnalysis.FSharpType: Boolean IsUnresolved
FSharp.Compiler.CodeAnalysis.FSharpType: Boolean get_HasTypeDefinition()
FSharp.Compiler.CodeAnalysis.FSharpType: Boolean get_IsAbbreviation()
FSharp.Compiler.CodeAnalysis.FSharpType: Boolean get_IsAnonRecordType()
FSharp.Compiler.CodeAnalysis.FSharpType: Boolean get_IsFunctionType()
FSharp.Compiler.CodeAnalysis.FSharpType: Boolean get_IsGenericParameter()
FSharp.Compiler.CodeAnalysis.FSharpType: Boolean get_IsNamedType()
FSharp.Compiler.CodeAnalysis.FSharpType: Boolean get_IsStructTupleType()
FSharp.Compiler.CodeAnalysis.FSharpType: Boolean get_IsTupleType()
FSharp.Compiler.CodeAnalysis.FSharpType: Boolean get_IsUnresolved()
FSharp.Compiler.CodeAnalysis.FSharpType: FSharp.Compiler.CodeAnalysis.FSharpAnonRecordTypeDetails AnonRecordTypeDetails
FSharp.Compiler.CodeAnalysis.FSharpType: FSharp.Compiler.CodeAnalysis.FSharpAnonRecordTypeDetails get_AnonRecordTypeDetails()
FSharp.Compiler.CodeAnalysis.FSharpType: FSharp.Compiler.CodeAnalysis.FSharpEntity NamedEntity
FSharp.Compiler.CodeAnalysis.FSharpType: FSharp.Compiler.CodeAnalysis.FSharpEntity TypeDefinition
FSharp.Compiler.CodeAnalysis.FSharpType: FSharp.Compiler.CodeAnalysis.FSharpEntity get_NamedEntity()
FSharp.Compiler.CodeAnalysis.FSharpType: FSharp.Compiler.CodeAnalysis.FSharpEntity get_TypeDefinition()
FSharp.Compiler.CodeAnalysis.FSharpType: FSharp.Compiler.CodeAnalysis.FSharpGenericParameter GenericParameter
FSharp.Compiler.CodeAnalysis.FSharpType: FSharp.Compiler.CodeAnalysis.FSharpGenericParameter get_GenericParameter()
FSharp.Compiler.CodeAnalysis.FSharpType: FSharp.Compiler.CodeAnalysis.FSharpParameter Prettify(FSharp.Compiler.CodeAnalysis.FSharpParameter)
FSharp.Compiler.CodeAnalysis.FSharpType: FSharp.Compiler.CodeAnalysis.FSharpType AbbreviatedType
FSharp.Compiler.CodeAnalysis.FSharpType: FSharp.Compiler.CodeAnalysis.FSharpType Instantiate(Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[FSharp.Compiler.CodeAnalysis.FSharpGenericParameter,FSharp.Compiler.CodeAnalysis.FSharpType]])
FSharp.Compiler.CodeAnalysis.FSharpType: FSharp.Compiler.CodeAnalysis.FSharpType Prettify(FSharp.Compiler.CodeAnalysis.FSharpType)
FSharp.Compiler.CodeAnalysis.FSharpType: FSharp.Compiler.CodeAnalysis.FSharpType get_AbbreviatedType()
FSharp.Compiler.CodeAnalysis.FSharpType: FSharp.Compiler.TextLayout.Layout FormatLayout(FSharp.Compiler.CodeAnalysis.FSharpDisplayContext)
FSharp.Compiler.CodeAnalysis.FSharpType: Int32 GetHashCode()
FSharp.Compiler.CodeAnalysis.FSharpType: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpType] BaseType
FSharp.Compiler.CodeAnalysis.FSharpType: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpType] get_BaseType()
FSharp.Compiler.CodeAnalysis.FSharpType: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpParameter] Prettify(System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpParameter])
FSharp.Compiler.CodeAnalysis.FSharpType: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpType] AllInterfaces
FSharp.Compiler.CodeAnalysis.FSharpType: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpType] GenericArguments
FSharp.Compiler.CodeAnalysis.FSharpType: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpType] Prettify(System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpType])
FSharp.Compiler.CodeAnalysis.FSharpType: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpType] get_AllInterfaces()
FSharp.Compiler.CodeAnalysis.FSharpType: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpType] get_GenericArguments()
FSharp.Compiler.CodeAnalysis.FSharpType: System.Collections.Generic.IList`1[System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpParameter]] Prettify(System.Collections.Generic.IList`1[System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpParameter]])
FSharp.Compiler.CodeAnalysis.FSharpType: System.String Format(FSharp.Compiler.CodeAnalysis.FSharpDisplayContext)
FSharp.Compiler.CodeAnalysis.FSharpType: System.String ToString()
FSharp.Compiler.CodeAnalysis.FSharpType: System.Tuple`2[System.Collections.Generic.IList`1[System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpParameter]],FSharp.Compiler.CodeAnalysis.FSharpParameter] Prettify(System.Collections.Generic.IList`1[System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpParameter]], FSharp.Compiler.CodeAnalysis.FSharpParameter)
FSharp.Compiler.CodeAnalysis.FSharpUnionCase
FSharp.Compiler.CodeAnalysis.FSharpUnionCase: Boolean Equals(System.Object)
FSharp.Compiler.CodeAnalysis.FSharpUnionCase: Boolean HasFields
FSharp.Compiler.CodeAnalysis.FSharpUnionCase: Boolean IsUnresolved
FSharp.Compiler.CodeAnalysis.FSharpUnionCase: Boolean get_HasFields()
FSharp.Compiler.CodeAnalysis.FSharpUnionCase: Boolean get_IsUnresolved()
FSharp.Compiler.CodeAnalysis.FSharpUnionCase: FSharp.Compiler.CodeAnalysis.FSharpAccessibility Accessibility
FSharp.Compiler.CodeAnalysis.FSharpUnionCase: FSharp.Compiler.CodeAnalysis.FSharpAccessibility get_Accessibility()
FSharp.Compiler.CodeAnalysis.FSharpUnionCase: FSharp.Compiler.CodeAnalysis.FSharpType ReturnType
FSharp.Compiler.CodeAnalysis.FSharpUnionCase: FSharp.Compiler.CodeAnalysis.FSharpType get_ReturnType()
FSharp.Compiler.CodeAnalysis.FSharpUnionCase: FSharp.Compiler.Text.Range DeclarationLocation
FSharp.Compiler.CodeAnalysis.FSharpUnionCase: FSharp.Compiler.Text.Range get_DeclarationLocation()
FSharp.Compiler.CodeAnalysis.FSharpUnionCase: Int32 GetHashCode()
FSharp.Compiler.CodeAnalysis.FSharpUnionCase: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpAttribute] Attributes
FSharp.Compiler.CodeAnalysis.FSharpUnionCase: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpAttribute] get_Attributes()
FSharp.Compiler.CodeAnalysis.FSharpUnionCase: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpField] Fields
FSharp.Compiler.CodeAnalysis.FSharpUnionCase: System.Collections.Generic.IList`1[FSharp.Compiler.CodeAnalysis.FSharpField] get_UnionCaseFields()
FSharp.Compiler.CodeAnalysis.FSharpUnionCase: System.Collections.Generic.IList`1[System.String] ElaboratedXmlDoc
FSharp.Compiler.CodeAnalysis.FSharpUnionCase: System.Collections.Generic.IList`1[System.String] XmlDoc
FSharp.Compiler.CodeAnalysis.FSharpUnionCase: System.Collections.Generic.IList`1[System.String] get_ElaboratedXmlDoc()
FSharp.Compiler.CodeAnalysis.FSharpUnionCase: System.Collections.Generic.IList`1[System.String] get_XmlDoc()
FSharp.Compiler.CodeAnalysis.FSharpUnionCase: System.String CompiledName
FSharp.Compiler.CodeAnalysis.FSharpUnionCase: System.String Name
FSharp.Compiler.CodeAnalysis.FSharpUnionCase: System.String ToString()
FSharp.Compiler.CodeAnalysis.FSharpUnionCase: System.String XmlDocSig
FSharp.Compiler.CodeAnalysis.FSharpUnionCase: System.String get_CompiledName()
FSharp.Compiler.CodeAnalysis.FSharpUnionCase: System.String get_Name()
FSharp.Compiler.CodeAnalysis.FSharpUnionCase: System.String get_XmlDocSig()
FSharp.Compiler.CodeAnalysis.FSharpUnresolvedReferencesSet
FSharp.Compiler.CodeAnalysis.FSharpUnresolvedReferencesSet: Boolean Equals(FSharp.Compiler.CodeAnalysis.FSharpUnresolvedReferencesSet)
FSharp.Compiler.CodeAnalysis.FSharpUnresolvedReferencesSet: Boolean Equals(System.Object)
FSharp.Compiler.CodeAnalysis.FSharpUnresolvedReferencesSet: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.CodeAnalysis.FSharpUnresolvedReferencesSet: Int32 GetHashCode()
FSharp.Compiler.CodeAnalysis.FSharpUnresolvedReferencesSet: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.CodeAnalysis.FSharpUnresolvedReferencesSet: System.String ToString()
FSharp.Compiler.CodeAnalysis.FSharpXmlDoc
FSharp.Compiler.CodeAnalysis.FSharpXmlDoc+Tags: Int32 None
FSharp.Compiler.CodeAnalysis.FSharpXmlDoc+Tags: Int32 Text
FSharp.Compiler.CodeAnalysis.FSharpXmlDoc+Tags: Int32 FromXmlFile
FSharp.Compiler.CodeAnalysis.FSharpXmlDoc+Text: System.String[] elaboratedXmlLines
FSharp.Compiler.CodeAnalysis.FSharpXmlDoc+Text: System.String[] get_elaboratedXmlLines()
FSharp.Compiler.CodeAnalysis.FSharpXmlDoc+Text: System.String[] get_unprocessedLines()
FSharp.Compiler.CodeAnalysis.FSharpXmlDoc+Text: System.String[] unprocessedLines
FSharp.Compiler.CodeAnalysis.FSharpXmlDoc+FromXmlFile: System.String file
FSharp.Compiler.CodeAnalysis.FSharpXmlDoc+FromXmlFile: System.String get_file()
FSharp.Compiler.CodeAnalysis.FSharpXmlDoc+FromXmlFile: System.String get_xmlSig()
FSharp.Compiler.CodeAnalysis.FSharpXmlDoc+FromXmlFile: System.String xmlSig
FSharp.Compiler.CodeAnalysis.FSharpXmlDoc: Boolean Equals(FSharp.Compiler.CodeAnalysis.FSharpXmlDoc)
FSharp.Compiler.CodeAnalysis.FSharpXmlDoc: Boolean Equals(System.Object)
FSharp.Compiler.CodeAnalysis.FSharpXmlDoc: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.CodeAnalysis.FSharpXmlDoc: Boolean IsNone
FSharp.Compiler.CodeAnalysis.FSharpXmlDoc: Boolean IsText
FSharp.Compiler.CodeAnalysis.FSharpXmlDoc: Boolean IsXmlDocFileSignature
FSharp.Compiler.CodeAnalysis.FSharpXmlDoc: Boolean get_IsNone()
FSharp.Compiler.CodeAnalysis.FSharpXmlDoc: Boolean get_IsText()
FSharp.Compiler.CodeAnalysis.FSharpXmlDoc: Boolean get_IsXmlDocFileSignature()
FSharp.Compiler.CodeAnalysis.FSharpXmlDoc: FSharp.Compiler.CodeAnalysis.FSharpXmlDoc NewText(System.String[], System.String[])
FSharp.Compiler.CodeAnalysis.FSharpXmlDoc: FSharp.Compiler.CodeAnalysis.FSharpXmlDoc NewXmlDocFileSignature(System.String, System.String)
FSharp.Compiler.CodeAnalysis.FSharpXmlDoc: FSharp.Compiler.CodeAnalysis.FSharpXmlDoc None
FSharp.Compiler.CodeAnalysis.FSharpXmlDoc: FSharp.Compiler.CodeAnalysis.FSharpXmlDoc get_None()
FSharp.Compiler.CodeAnalysis.FSharpXmlDoc: FSharp.Compiler.CodeAnalysis.FSharpXmlDoc+Tags
FSharp.Compiler.CodeAnalysis.FSharpXmlDoc: FSharp.Compiler.CodeAnalysis.FSharpXmlDoc+Text
FSharp.Compiler.CodeAnalysis.FSharpXmlDoc: FSharp.Compiler.CodeAnalysis.FSharpXmlDoc+FromXmlFile
FSharp.Compiler.CodeAnalysis.FSharpXmlDoc: Int32 CompareTo(FSharp.Compiler.CodeAnalysis.FSharpXmlDoc)
FSharp.Compiler.CodeAnalysis.FSharpXmlDoc: Int32 CompareTo(System.Object)
FSharp.Compiler.CodeAnalysis.FSharpXmlDoc: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.CodeAnalysis.FSharpXmlDoc: Int32 GetHashCode()
FSharp.Compiler.CodeAnalysis.FSharpXmlDoc: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.CodeAnalysis.FSharpXmlDoc: Int32 Tag
FSharp.Compiler.CodeAnalysis.FSharpXmlDoc: Int32 get_Tag()
FSharp.Compiler.CodeAnalysis.FSharpXmlDoc: System.String ToString()
FSharp.Compiler.CodeAnalysis.LegacyMSBuildReferenceResolver
FSharp.Compiler.CodeAnalysis.LegacyMSBuildReferenceResolver: FSharp.Compiler.CodeAnalysis.LegacyReferenceResolver getResolver()
FSharp.Compiler.CodeAnalysis.LegacyReferenceResolver
FSharp.Compiler.CodeAnalysis.SemanticClassificationType
FSharp.Compiler.CodeAnalysis.SemanticClassificationType: FSharp.Compiler.CodeAnalysis.SemanticClassificationType ComputationExpression
FSharp.Compiler.CodeAnalysis.SemanticClassificationType: FSharp.Compiler.CodeAnalysis.SemanticClassificationType ConstructorForReferenceType
FSharp.Compiler.CodeAnalysis.SemanticClassificationType: FSharp.Compiler.CodeAnalysis.SemanticClassificationType ConstructorForValueType
FSharp.Compiler.CodeAnalysis.SemanticClassificationType: FSharp.Compiler.CodeAnalysis.SemanticClassificationType Delegate
FSharp.Compiler.CodeAnalysis.SemanticClassificationType: FSharp.Compiler.CodeAnalysis.SemanticClassificationType DisposableLocalValue
FSharp.Compiler.CodeAnalysis.SemanticClassificationType: FSharp.Compiler.CodeAnalysis.SemanticClassificationType DisposableTopLevelValue
FSharp.Compiler.CodeAnalysis.SemanticClassificationType: FSharp.Compiler.CodeAnalysis.SemanticClassificationType DisposableType
FSharp.Compiler.CodeAnalysis.SemanticClassificationType: FSharp.Compiler.CodeAnalysis.SemanticClassificationType Enumeration
FSharp.Compiler.CodeAnalysis.SemanticClassificationType: FSharp.Compiler.CodeAnalysis.SemanticClassificationType Event
FSharp.Compiler.CodeAnalysis.SemanticClassificationType: FSharp.Compiler.CodeAnalysis.SemanticClassificationType Exception
FSharp.Compiler.CodeAnalysis.SemanticClassificationType: FSharp.Compiler.CodeAnalysis.SemanticClassificationType ExtensionMethod
FSharp.Compiler.CodeAnalysis.SemanticClassificationType: FSharp.Compiler.CodeAnalysis.SemanticClassificationType Field
FSharp.Compiler.CodeAnalysis.SemanticClassificationType: FSharp.Compiler.CodeAnalysis.SemanticClassificationType Function
FSharp.Compiler.CodeAnalysis.SemanticClassificationType: FSharp.Compiler.CodeAnalysis.SemanticClassificationType Interface
FSharp.Compiler.CodeAnalysis.SemanticClassificationType: FSharp.Compiler.CodeAnalysis.SemanticClassificationType IntrinsicFunction
FSharp.Compiler.CodeAnalysis.SemanticClassificationType: FSharp.Compiler.CodeAnalysis.SemanticClassificationType Literal
FSharp.Compiler.CodeAnalysis.SemanticClassificationType: FSharp.Compiler.CodeAnalysis.SemanticClassificationType LocalValue
FSharp.Compiler.CodeAnalysis.SemanticClassificationType: FSharp.Compiler.CodeAnalysis.SemanticClassificationType Method
FSharp.Compiler.CodeAnalysis.SemanticClassificationType: FSharp.Compiler.CodeAnalysis.SemanticClassificationType Module
FSharp.Compiler.CodeAnalysis.SemanticClassificationType: FSharp.Compiler.CodeAnalysis.SemanticClassificationType MutableRecordField
FSharp.Compiler.CodeAnalysis.SemanticClassificationType: FSharp.Compiler.CodeAnalysis.SemanticClassificationType MutableVar
FSharp.Compiler.CodeAnalysis.SemanticClassificationType: FSharp.Compiler.CodeAnalysis.SemanticClassificationType NamedArgument
FSharp.Compiler.CodeAnalysis.SemanticClassificationType: FSharp.Compiler.CodeAnalysis.SemanticClassificationType Namespace
FSharp.Compiler.CodeAnalysis.SemanticClassificationType: FSharp.Compiler.CodeAnalysis.SemanticClassificationType Operator
FSharp.Compiler.CodeAnalysis.SemanticClassificationType: FSharp.Compiler.CodeAnalysis.SemanticClassificationType Plaintext
FSharp.Compiler.CodeAnalysis.SemanticClassificationType: FSharp.Compiler.CodeAnalysis.SemanticClassificationType Printf
FSharp.Compiler.CodeAnalysis.SemanticClassificationType: FSharp.Compiler.CodeAnalysis.SemanticClassificationType Property
FSharp.Compiler.CodeAnalysis.SemanticClassificationType: FSharp.Compiler.CodeAnalysis.SemanticClassificationType RecordField
FSharp.Compiler.CodeAnalysis.SemanticClassificationType: FSharp.Compiler.CodeAnalysis.SemanticClassificationType RecordFieldAsFunction
FSharp.Compiler.CodeAnalysis.SemanticClassificationType: FSharp.Compiler.CodeAnalysis.SemanticClassificationType ReferenceType
FSharp.Compiler.CodeAnalysis.SemanticClassificationType: FSharp.Compiler.CodeAnalysis.SemanticClassificationType Type
FSharp.Compiler.CodeAnalysis.SemanticClassificationType: FSharp.Compiler.CodeAnalysis.SemanticClassificationType TypeArgument
FSharp.Compiler.CodeAnalysis.SemanticClassificationType: FSharp.Compiler.CodeAnalysis.SemanticClassificationType TypeDef
FSharp.Compiler.CodeAnalysis.SemanticClassificationType: FSharp.Compiler.CodeAnalysis.SemanticClassificationType UnionCase
FSharp.Compiler.CodeAnalysis.SemanticClassificationType: FSharp.Compiler.CodeAnalysis.SemanticClassificationType UnionCaseField
FSharp.Compiler.CodeAnalysis.SemanticClassificationType: FSharp.Compiler.CodeAnalysis.SemanticClassificationType Value
FSharp.Compiler.CodeAnalysis.SemanticClassificationType: FSharp.Compiler.CodeAnalysis.SemanticClassificationType ValueType
FSharp.Compiler.CodeAnalysis.SemanticClassificationType: Int32 value__
FSharp.Compiler.CodeAnalysis.Symbol
FSharp.Compiler.CodeAnalysis.Symbol: Boolean hasAttribute[T](System.Collections.Generic.IEnumerable`1[FSharp.Compiler.CodeAnalysis.FSharpAttribute])
FSharp.Compiler.CodeAnalysis.Symbol: Boolean hasModuleSuffixAttribute(FSharp.Compiler.CodeAnalysis.FSharpEntity)
FSharp.Compiler.CodeAnalysis.Symbol: Boolean isAttribute[T](FSharp.Compiler.CodeAnalysis.FSharpAttribute)
FSharp.Compiler.CodeAnalysis.Symbol: Boolean isOperator(System.String)
FSharp.Compiler.CodeAnalysis.Symbol: Boolean isUnnamedUnionCaseField(FSharp.Compiler.CodeAnalysis.FSharpField)
FSharp.Compiler.CodeAnalysis.Symbol: FSharp.Compiler.CodeAnalysis.FSharpType getAbbreviatedType(FSharp.Compiler.CodeAnalysis.FSharpType)
FSharp.Compiler.CodeAnalysis.Symbol: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpActivePatternCase] |ActivePatternCase|_|(FSharp.Compiler.CodeAnalysis.FSharpSymbol)
FSharp.Compiler.CodeAnalysis.Symbol: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpAttribute] tryGetAttribute[T](System.Collections.Generic.IEnumerable`1[FSharp.Compiler.CodeAnalysis.FSharpAttribute])
FSharp.Compiler.CodeAnalysis.Symbol: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpEntity] |Constructor|_|(FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue)
FSharp.Compiler.CodeAnalysis.Symbol: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpEntity] |TypeWithDefinition|_|(FSharp.Compiler.CodeAnalysis.FSharpType)
FSharp.Compiler.CodeAnalysis.Symbol: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpField] |RecordField|_|(FSharp.Compiler.CodeAnalysis.FSharpSymbol)
FSharp.Compiler.CodeAnalysis.Symbol: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue] |MemberFunctionOrValue|_|(FSharp.Compiler.CodeAnalysis.FSharpSymbol)
FSharp.Compiler.CodeAnalysis.Symbol: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpType] |AbbreviatedType|_|(FSharp.Compiler.CodeAnalysis.FSharpEntity)
FSharp.Compiler.CodeAnalysis.Symbol: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpUnionCase] |UnionCase|_|(FSharp.Compiler.CodeAnalysis.FSharpSymbol)
FSharp.Compiler.CodeAnalysis.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |AbstractClass|_|(FSharp.Compiler.CodeAnalysis.FSharpEntity)
FSharp.Compiler.CodeAnalysis.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |Array|_|(FSharp.Compiler.CodeAnalysis.FSharpEntity)
FSharp.Compiler.CodeAnalysis.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |Attribute|_|(FSharp.Compiler.CodeAnalysis.FSharpEntity)
FSharp.Compiler.CodeAnalysis.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |ByRef|_|(FSharp.Compiler.CodeAnalysis.FSharpEntity)
FSharp.Compiler.CodeAnalysis.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |Class|_|[a](FSharp.Compiler.CodeAnalysis.FSharpEntity, FSharp.Compiler.CodeAnalysis.FSharpEntity, a)
FSharp.Compiler.CodeAnalysis.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |Delegate|_|(FSharp.Compiler.CodeAnalysis.FSharpEntity)
FSharp.Compiler.CodeAnalysis.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |Enum|_|(FSharp.Compiler.CodeAnalysis.FSharpEntity)
FSharp.Compiler.CodeAnalysis.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |Event|_|(FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue)
FSharp.Compiler.CodeAnalysis.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |ExtensionMember|_|(FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue)
FSharp.Compiler.CodeAnalysis.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |FSharpException|_|(FSharp.Compiler.CodeAnalysis.FSharpEntity)
FSharp.Compiler.CodeAnalysis.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |FSharpModule|_|(FSharp.Compiler.CodeAnalysis.FSharpEntity)
FSharp.Compiler.CodeAnalysis.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |FSharpType|_|(FSharp.Compiler.CodeAnalysis.FSharpEntity)
FSharp.Compiler.CodeAnalysis.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |FunctionType|_|(FSharp.Compiler.CodeAnalysis.FSharpType)
FSharp.Compiler.CodeAnalysis.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |Function|_|(Boolean, FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue)
FSharp.Compiler.CodeAnalysis.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |Interface|_|(FSharp.Compiler.CodeAnalysis.FSharpEntity)
FSharp.Compiler.CodeAnalysis.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |MutableVar|_|(FSharp.Compiler.CodeAnalysis.FSharpSymbol)
FSharp.Compiler.CodeAnalysis.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |Namespace|_|(FSharp.Compiler.CodeAnalysis.FSharpEntity)
FSharp.Compiler.CodeAnalysis.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |Parameter|_|(FSharp.Compiler.CodeAnalysis.FSharpSymbol)
FSharp.Compiler.CodeAnalysis.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |Pattern|_|(FSharp.Compiler.CodeAnalysis.FSharpSymbol)
FSharp.Compiler.CodeAnalysis.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |ProvidedAndErasedType|_|(FSharp.Compiler.CodeAnalysis.FSharpEntity)
FSharp.Compiler.CodeAnalysis.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |ProvidedType|_|(FSharp.Compiler.CodeAnalysis.FSharpEntity)
FSharp.Compiler.CodeAnalysis.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |Record|_|(FSharp.Compiler.CodeAnalysis.FSharpEntity)
FSharp.Compiler.CodeAnalysis.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |RefCell|_|(FSharp.Compiler.CodeAnalysis.FSharpType)
FSharp.Compiler.CodeAnalysis.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |Tuple|_|(FSharp.Compiler.CodeAnalysis.FSharpType)
FSharp.Compiler.CodeAnalysis.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |UnionType|_|(FSharp.Compiler.CodeAnalysis.FSharpEntity)
FSharp.Compiler.CodeAnalysis.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |ValueType|_|(FSharp.Compiler.CodeAnalysis.FSharpEntity)
FSharp.Compiler.CodeAnalysis.Symbol: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.CodeAnalysis.FSharpField,FSharp.Compiler.CodeAnalysis.FSharpType]] |Field|_|(FSharp.Compiler.CodeAnalysis.FSharpSymbol)
FSharp.Compiler.CodeAnalysis.Symbol: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`3[FSharp.Compiler.CodeAnalysis.FSharpEntity,FSharp.Compiler.CodeAnalysis.FSharpEntity,Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpType]]] |FSharpEntity|_|(FSharp.Compiler.CodeAnalysis.FSharpSymbol)
FSharp.Compiler.CodeAnalysis.Symbol: System.Tuple`2[FSharp.Compiler.CodeAnalysis.FSharpEntity,Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpType]] getEntityAbbreviatedType(FSharp.Compiler.CodeAnalysis.FSharpEntity)
FSharp.Compiler.DependencyManager.AssemblyResolutionProbe
FSharp.Compiler.DependencyManager.AssemblyResolutionProbe: System.Collections.Generic.IEnumerable`1[System.String] EndInvoke(System.IAsyncResult)
FSharp.Compiler.DependencyManager.AssemblyResolutionProbe: System.Collections.Generic.IEnumerable`1[System.String] Invoke()
FSharp.Compiler.DependencyManager.AssemblyResolutionProbe: System.IAsyncResult BeginInvoke(System.AsyncCallback, System.Object)
FSharp.Compiler.DependencyManager.AssemblyResolutionProbe: Void .ctor(System.Object, IntPtr)
FSharp.Compiler.DependencyManager.AssemblyResolveHandler
FSharp.Compiler.DependencyManager.AssemblyResolveHandler: Void .ctor(FSharp.Compiler.DependencyManager.AssemblyResolutionProbe)
FSharp.Compiler.DependencyManager.DependencyProvider
FSharp.Compiler.DependencyManager.DependencyProvider: FSharp.Compiler.DependencyManager.IDependencyManagerProvider TryFindDependencyManagerByKey(System.Collections.Generic.IEnumerable`1[System.String], System.String, FSharp.Compiler.DependencyManager.ResolvingErrorReport, System.String)
FSharp.Compiler.DependencyManager.DependencyProvider: FSharp.Compiler.DependencyManager.IResolveDependenciesResult Resolve(FSharp.Compiler.DependencyManager.IDependencyManagerProvider, System.String, System.Collections.Generic.IEnumerable`1[System.Tuple`2[System.String,System.String]], FSharp.Compiler.DependencyManager.ResolvingErrorReport, System.String, System.String, System.String, System.String, System.String, Int32)
FSharp.Compiler.DependencyManager.DependencyProvider: System.String[] GetRegisteredDependencyManagerHelpText(System.Collections.Generic.IEnumerable`1[System.String], System.String, FSharp.Compiler.DependencyManager.ResolvingErrorReport)
FSharp.Compiler.DependencyManager.DependencyProvider: System.Tuple`2[System.Int32,System.String] CreatePackageManagerUnknownError(System.Collections.Generic.IEnumerable`1[System.String], System.String, System.String, FSharp.Compiler.DependencyManager.ResolvingErrorReport)
FSharp.Compiler.DependencyManager.DependencyProvider: System.Tuple`2[System.String,FSharp.Compiler.DependencyManager.IDependencyManagerProvider] TryFindDependencyManagerInPath(System.Collections.Generic.IEnumerable`1[System.String], System.String, FSharp.Compiler.DependencyManager.ResolvingErrorReport, System.String)
FSharp.Compiler.DependencyManager.DependencyProvider: Void .ctor()
FSharp.Compiler.DependencyManager.DependencyProvider: Void .ctor(FSharp.Compiler.DependencyManager.AssemblyResolutionProbe, FSharp.Compiler.DependencyManager.NativeResolutionProbe)
FSharp.Compiler.DependencyManager.DependencyProvider: Void .ctor(FSharp.Compiler.DependencyManager.NativeResolutionProbe)
FSharp.Compiler.DependencyManager.ErrorReportType
FSharp.Compiler.DependencyManager.ErrorReportType+Tags: Int32 Error
FSharp.Compiler.DependencyManager.ErrorReportType+Tags: Int32 Warning
FSharp.Compiler.DependencyManager.ErrorReportType: Boolean Equals(FSharp.Compiler.DependencyManager.ErrorReportType)
FSharp.Compiler.DependencyManager.ErrorReportType: Boolean Equals(System.Object)
FSharp.Compiler.DependencyManager.ErrorReportType: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.DependencyManager.ErrorReportType: Boolean IsError
FSharp.Compiler.DependencyManager.ErrorReportType: Boolean IsWarning
FSharp.Compiler.DependencyManager.ErrorReportType: Boolean get_IsError()
FSharp.Compiler.DependencyManager.ErrorReportType: Boolean get_IsWarning()
FSharp.Compiler.DependencyManager.ErrorReportType: FSharp.Compiler.DependencyManager.ErrorReportType Error
FSharp.Compiler.DependencyManager.ErrorReportType: FSharp.Compiler.DependencyManager.ErrorReportType Warning
FSharp.Compiler.DependencyManager.ErrorReportType: FSharp.Compiler.DependencyManager.ErrorReportType get_Error()
FSharp.Compiler.DependencyManager.ErrorReportType: FSharp.Compiler.DependencyManager.ErrorReportType get_Warning()
FSharp.Compiler.DependencyManager.ErrorReportType: FSharp.Compiler.DependencyManager.ErrorReportType+Tags
FSharp.Compiler.DependencyManager.ErrorReportType: Int32 CompareTo(FSharp.Compiler.DependencyManager.ErrorReportType)
FSharp.Compiler.DependencyManager.ErrorReportType: Int32 CompareTo(System.Object)
FSharp.Compiler.DependencyManager.ErrorReportType: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.DependencyManager.ErrorReportType: Int32 GetHashCode()
FSharp.Compiler.DependencyManager.ErrorReportType: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.DependencyManager.ErrorReportType: Int32 Tag
FSharp.Compiler.DependencyManager.ErrorReportType: Int32 get_Tag()
FSharp.Compiler.DependencyManager.ErrorReportType: System.String ToString()
FSharp.Compiler.DependencyManager.IDependencyManagerProvider
FSharp.Compiler.DependencyManager.IDependencyManagerProvider: FSharp.Compiler.DependencyManager.IResolveDependenciesResult ResolveDependencies(System.String, System.String, System.String, System.String, System.Collections.Generic.IEnumerable`1[System.Tuple`2[System.String,System.String]], System.String, System.String, Int32)
FSharp.Compiler.DependencyManager.IDependencyManagerProvider: System.String Key
FSharp.Compiler.DependencyManager.IDependencyManagerProvider: System.String Name
FSharp.Compiler.DependencyManager.IDependencyManagerProvider: System.String get_Key()
FSharp.Compiler.DependencyManager.IDependencyManagerProvider: System.String get_Name()
FSharp.Compiler.DependencyManager.IDependencyManagerProvider: System.String[] HelpMessages
FSharp.Compiler.DependencyManager.IDependencyManagerProvider: System.String[] get_HelpMessages()
FSharp.Compiler.DependencyManager.IResolveDependenciesResult
FSharp.Compiler.DependencyManager.IResolveDependenciesResult: Boolean Success
FSharp.Compiler.DependencyManager.IResolveDependenciesResult: Boolean get_Success()
FSharp.Compiler.DependencyManager.IResolveDependenciesResult: System.Collections.Generic.IEnumerable`1[System.String] Resolutions
FSharp.Compiler.DependencyManager.IResolveDependenciesResult: System.Collections.Generic.IEnumerable`1[System.String] Roots
FSharp.Compiler.DependencyManager.IResolveDependenciesResult: System.Collections.Generic.IEnumerable`1[System.String] SourceFiles
FSharp.Compiler.DependencyManager.IResolveDependenciesResult: System.Collections.Generic.IEnumerable`1[System.String] get_Resolutions()
FSharp.Compiler.DependencyManager.IResolveDependenciesResult: System.Collections.Generic.IEnumerable`1[System.String] get_Roots()
FSharp.Compiler.DependencyManager.IResolveDependenciesResult: System.Collections.Generic.IEnumerable`1[System.String] get_SourceFiles()
FSharp.Compiler.DependencyManager.IResolveDependenciesResult: System.String[] StdError
FSharp.Compiler.DependencyManager.IResolveDependenciesResult: System.String[] StdOut
FSharp.Compiler.DependencyManager.IResolveDependenciesResult: System.String[] get_StdError()
FSharp.Compiler.DependencyManager.IResolveDependenciesResult: System.String[] get_StdOut()
FSharp.Compiler.DependencyManager.NativeDllResolveHandler
FSharp.Compiler.DependencyManager.NativeDllResolveHandler: Void .ctor(FSharp.Compiler.DependencyManager.NativeResolutionProbe)
FSharp.Compiler.DependencyManager.NativeResolutionProbe
FSharp.Compiler.DependencyManager.NativeResolutionProbe: System.Collections.Generic.IEnumerable`1[System.String] EndInvoke(System.IAsyncResult)
FSharp.Compiler.DependencyManager.NativeResolutionProbe: System.Collections.Generic.IEnumerable`1[System.String] Invoke()
FSharp.Compiler.DependencyManager.NativeResolutionProbe: System.IAsyncResult BeginInvoke(System.AsyncCallback, System.Object)
FSharp.Compiler.DependencyManager.NativeResolutionProbe: Void .ctor(System.Object, IntPtr)
FSharp.Compiler.DependencyManager.ResolvingErrorReport
FSharp.Compiler.DependencyManager.ResolvingErrorReport: System.IAsyncResult BeginInvoke(FSharp.Compiler.DependencyManager.ErrorReportType, Int32, System.String, System.AsyncCallback, System.Object)
FSharp.Compiler.DependencyManager.ResolvingErrorReport: Void .ctor(System.Object, IntPtr)
FSharp.Compiler.DependencyManager.ResolvingErrorReport: Void EndInvoke(System.IAsyncResult)
FSharp.Compiler.DependencyManager.ResolvingErrorReport: Void Invoke(FSharp.Compiler.DependencyManager.ErrorReportType, Int32, System.String)
FSharp.Compiler.Diagnostics.CompilerDiagnostics
FSharp.Compiler.Diagnostics.CompilerDiagnostics: System.String GetErrorMessage(FSharp.Compiler.Diagnostics.FSharpDiagnosticKind)
FSharp.Compiler.Diagnostics.FSharpDiagnostic
FSharp.Compiler.Diagnostics.FSharpDiagnostic: FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity Severity
FSharp.Compiler.Diagnostics.FSharpDiagnostic: FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity get_Severity()
FSharp.Compiler.Diagnostics.FSharpDiagnostic: FSharp.Compiler.Text.Position End
FSharp.Compiler.Diagnostics.FSharpDiagnostic: FSharp.Compiler.Text.Position Start
FSharp.Compiler.Diagnostics.FSharpDiagnostic: FSharp.Compiler.Text.Position get_End()
FSharp.Compiler.Diagnostics.FSharpDiagnostic: FSharp.Compiler.Text.Position get_Start()
FSharp.Compiler.Diagnostics.FSharpDiagnostic: FSharp.Compiler.Text.Range Range
FSharp.Compiler.Diagnostics.FSharpDiagnostic: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.Diagnostics.FSharpDiagnostic: Int32 EndColumn
FSharp.Compiler.Diagnostics.FSharpDiagnostic: Int32 EndLine
FSharp.Compiler.Diagnostics.FSharpDiagnostic: Int32 ErrorNumber
FSharp.Compiler.Diagnostics.FSharpDiagnostic: Int32 StartColumn
FSharp.Compiler.Diagnostics.FSharpDiagnostic: Int32 StartLine
FSharp.Compiler.Diagnostics.FSharpDiagnostic: Int32 get_EndColumn()
FSharp.Compiler.Diagnostics.FSharpDiagnostic: Int32 get_EndLineAlternate()
FSharp.Compiler.Diagnostics.FSharpDiagnostic: Int32 get_ErrorNumber()
FSharp.Compiler.Diagnostics.FSharpDiagnostic: Int32 get_StartColumn()
FSharp.Compiler.Diagnostics.FSharpDiagnostic: Int32 get_StartLineAlternate()
FSharp.Compiler.Diagnostics.FSharpDiagnostic: System.String FileName
FSharp.Compiler.Diagnostics.FSharpDiagnostic: System.String Message
FSharp.Compiler.Diagnostics.FSharpDiagnostic: System.String NewlineifyErrorString(System.String)
FSharp.Compiler.Diagnostics.FSharpDiagnostic: System.String NormalizeErrorString(System.String)
FSharp.Compiler.Diagnostics.FSharpDiagnostic: System.String Subcategory
FSharp.Compiler.Diagnostics.FSharpDiagnostic: System.String ToString()
FSharp.Compiler.Diagnostics.FSharpDiagnostic: System.String get_FileName()
FSharp.Compiler.Diagnostics.FSharpDiagnostic: System.String get_Message()
FSharp.Compiler.Diagnostics.FSharpDiagnostic: System.String get_Subcategory()
FSharp.Compiler.Diagnostics.FSharpDiagnosticKind
FSharp.Compiler.Diagnostics.FSharpDiagnosticKind+ReplaceWithSuggestion: System.String get_suggestion()
FSharp.Compiler.Diagnostics.FSharpDiagnosticKind+ReplaceWithSuggestion: System.String suggestion
FSharp.Compiler.Diagnostics.FSharpDiagnosticKind+Tags: Int32 AddIndexerDot
FSharp.Compiler.Diagnostics.FSharpDiagnosticKind+Tags: Int32 ReplaceWithSuggestion
FSharp.Compiler.Diagnostics.FSharpDiagnosticKind: Boolean Equals(FSharp.Compiler.Diagnostics.FSharpDiagnosticKind)
FSharp.Compiler.Diagnostics.FSharpDiagnosticKind: Boolean Equals(System.Object)
FSharp.Compiler.Diagnostics.FSharpDiagnosticKind: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.Diagnostics.FSharpDiagnosticKind: Boolean IsAddIndexerDot
FSharp.Compiler.Diagnostics.FSharpDiagnosticKind: Boolean IsReplaceWithSuggestion
FSharp.Compiler.Diagnostics.FSharpDiagnosticKind: Boolean get_IsAddIndexerDot()
FSharp.Compiler.Diagnostics.FSharpDiagnosticKind: Boolean get_IsReplaceWithSuggestion()
FSharp.Compiler.Diagnostics.FSharpDiagnosticKind: FSharp.Compiler.Diagnostics.FSharpDiagnosticKind AddIndexerDot
FSharp.Compiler.Diagnostics.FSharpDiagnosticKind: FSharp.Compiler.Diagnostics.FSharpDiagnosticKind NewReplaceWithSuggestion(System.String)
FSharp.Compiler.Diagnostics.FSharpDiagnosticKind: FSharp.Compiler.Diagnostics.FSharpDiagnosticKind get_AddIndexerDot()
FSharp.Compiler.Diagnostics.FSharpDiagnosticKind: FSharp.Compiler.Diagnostics.FSharpDiagnosticKind+ReplaceWithSuggestion
FSharp.Compiler.Diagnostics.FSharpDiagnosticKind: FSharp.Compiler.Diagnostics.FSharpDiagnosticKind+Tags
FSharp.Compiler.Diagnostics.FSharpDiagnosticKind: Int32 CompareTo(FSharp.Compiler.Diagnostics.FSharpDiagnosticKind)
FSharp.Compiler.Diagnostics.FSharpDiagnosticKind: Int32 CompareTo(System.Object)
FSharp.Compiler.Diagnostics.FSharpDiagnosticKind: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.Diagnostics.FSharpDiagnosticKind: Int32 GetHashCode()
FSharp.Compiler.Diagnostics.FSharpDiagnosticKind: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.Diagnostics.FSharpDiagnosticKind: Int32 Tag
FSharp.Compiler.Diagnostics.FSharpDiagnosticKind: Int32 get_Tag()
FSharp.Compiler.Diagnostics.FSharpDiagnosticKind: System.String ToString()
FSharp.Compiler.Diagnostics.FSharpDiagnosticOptions
FSharp.Compiler.Diagnostics.FSharpDiagnosticOptions: Boolean Equals(FSharp.Compiler.Diagnostics.FSharpDiagnosticOptions)
FSharp.Compiler.Diagnostics.FSharpDiagnosticOptions: Boolean Equals(System.Object)
FSharp.Compiler.Diagnostics.FSharpDiagnosticOptions: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.Diagnostics.FSharpDiagnosticOptions: Boolean GlobalWarnAsError
FSharp.Compiler.Diagnostics.FSharpDiagnosticOptions: Boolean get_GlobalWarnAsError()
FSharp.Compiler.Diagnostics.FSharpDiagnosticOptions: FSharp.Compiler.Diagnostics.FSharpDiagnosticOptions Default
FSharp.Compiler.Diagnostics.FSharpDiagnosticOptions: FSharp.Compiler.Diagnostics.FSharpDiagnosticOptions get_Default()
FSharp.Compiler.Diagnostics.FSharpDiagnosticOptions: Int32 CompareTo(FSharp.Compiler.Diagnostics.FSharpDiagnosticOptions)
FSharp.Compiler.Diagnostics.FSharpDiagnosticOptions: Int32 CompareTo(System.Object)
FSharp.Compiler.Diagnostics.FSharpDiagnosticOptions: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.Diagnostics.FSharpDiagnosticOptions: Int32 GetHashCode()
FSharp.Compiler.Diagnostics.FSharpDiagnosticOptions: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.Diagnostics.FSharpDiagnosticOptions: Int32 WarnLevel
FSharp.Compiler.Diagnostics.FSharpDiagnosticOptions: Int32 get_WarnLevel()
FSharp.Compiler.Diagnostics.FSharpDiagnosticOptions: Microsoft.FSharp.Collections.FSharpList`1[System.Int32] WarnAsError
FSharp.Compiler.Diagnostics.FSharpDiagnosticOptions: Microsoft.FSharp.Collections.FSharpList`1[System.Int32] WarnAsWarn
FSharp.Compiler.Diagnostics.FSharpDiagnosticOptions: Microsoft.FSharp.Collections.FSharpList`1[System.Int32] WarnOff
FSharp.Compiler.Diagnostics.FSharpDiagnosticOptions: Microsoft.FSharp.Collections.FSharpList`1[System.Int32] WarnOn
FSharp.Compiler.Diagnostics.FSharpDiagnosticOptions: Microsoft.FSharp.Collections.FSharpList`1[System.Int32] get_WarnAsError()
FSharp.Compiler.Diagnostics.FSharpDiagnosticOptions: Microsoft.FSharp.Collections.FSharpList`1[System.Int32] get_WarnAsWarn()
FSharp.Compiler.Diagnostics.FSharpDiagnosticOptions: Microsoft.FSharp.Collections.FSharpList`1[System.Int32] get_WarnOff()
FSharp.Compiler.Diagnostics.FSharpDiagnosticOptions: Microsoft.FSharp.Collections.FSharpList`1[System.Int32] get_WarnOn()
FSharp.Compiler.Diagnostics.FSharpDiagnosticOptions: System.String ToString()
FSharp.Compiler.Diagnostics.FSharpDiagnosticOptions: Void .ctor(Int32, Boolean, Microsoft.FSharp.Collections.FSharpList`1[System.Int32], Microsoft.FSharp.Collections.FSharpList`1[System.Int32], Microsoft.FSharp.Collections.FSharpList`1[System.Int32], Microsoft.FSharp.Collections.FSharpList`1[System.Int32])
FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity
FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity+Tags: Int32 Error
FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity+Tags: Int32 Hidden
FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity+Tags: Int32 Info
FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity+Tags: Int32 Warning
FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity: Boolean Equals(FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity)
FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity: Boolean Equals(System.Object)
FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity: Boolean IsError
FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity: Boolean IsHidden
FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity: Boolean IsInfo
FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity: Boolean IsWarning
FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity: Boolean get_IsError()
FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity: Boolean get_IsHidden()
FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity: Boolean get_IsInfo()
FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity: Boolean get_IsWarning()
FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity: FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity Error
FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity: FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity Hidden
FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity: FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity Info
FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity: FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity Warning
FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity: FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity get_Error()
FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity: FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity get_Hidden()
FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity: FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity get_Info()
FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity: FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity get_Warning()
FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity: FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity+Tags
FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity: Int32 CompareTo(FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity)
FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity: Int32 CompareTo(System.Object)
FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity: Int32 GetHashCode()
FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity: Int32 Tag
FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity: Int32 get_Tag()
FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity: System.String ToString()
FSharp.Compiler.EditorServices.AssemblyContent
FSharp.Compiler.EditorServices.AssemblyContent: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.EditorServices.AssemblySymbol] GetAssemblyContent(Microsoft.FSharp.Core.FSharpFunc`2[Microsoft.FSharp.Core.FSharpFunc`2[FSharp.Compiler.EditorServices.IAssemblyContentCache,Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.EditorServices.AssemblySymbol]],Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.EditorServices.AssemblySymbol]], FSharp.Compiler.EditorServices.AssemblyContentType, Microsoft.FSharp.Core.FSharpOption`1[System.String], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.CodeAnalysis.FSharpAssembly])
FSharp.Compiler.EditorServices.AssemblyContent: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.EditorServices.AssemblySymbol] GetAssemblySignatureContent(FSharp.Compiler.EditorServices.AssemblyContentType, FSharp.Compiler.CodeAnalysis.FSharpAssemblySignature)
FSharp.Compiler.EditorServices.AssemblyContentType
FSharp.Compiler.EditorServices.AssemblyContentType+Tags: Int32 Full
FSharp.Compiler.EditorServices.AssemblyContentType+Tags: Int32 Public
FSharp.Compiler.EditorServices.AssemblyContentType: Boolean Equals(FSharp.Compiler.EditorServices.AssemblyContentType)
FSharp.Compiler.EditorServices.AssemblyContentType: Boolean Equals(System.Object)
FSharp.Compiler.EditorServices.AssemblyContentType: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.AssemblyContentType: Boolean IsFull
FSharp.Compiler.EditorServices.AssemblyContentType: Boolean IsPublic
FSharp.Compiler.EditorServices.AssemblyContentType: Boolean get_IsFull()
FSharp.Compiler.EditorServices.AssemblyContentType: Boolean get_IsPublic()
FSharp.Compiler.EditorServices.AssemblyContentType: FSharp.Compiler.EditorServices.AssemblyContentType Full
FSharp.Compiler.EditorServices.AssemblyContentType: FSharp.Compiler.EditorServices.AssemblyContentType Public
FSharp.Compiler.EditorServices.AssemblyContentType: FSharp.Compiler.EditorServices.AssemblyContentType get_Full()
FSharp.Compiler.EditorServices.AssemblyContentType: FSharp.Compiler.EditorServices.AssemblyContentType get_Public()
FSharp.Compiler.EditorServices.AssemblyContentType: FSharp.Compiler.EditorServices.AssemblyContentType+Tags
FSharp.Compiler.EditorServices.AssemblyContentType: Int32 CompareTo(FSharp.Compiler.EditorServices.AssemblyContentType)
FSharp.Compiler.EditorServices.AssemblyContentType: Int32 CompareTo(System.Object)
FSharp.Compiler.EditorServices.AssemblyContentType: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.EditorServices.AssemblyContentType: Int32 GetHashCode()
FSharp.Compiler.EditorServices.AssemblyContentType: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.AssemblyContentType: Int32 Tag
FSharp.Compiler.EditorServices.AssemblyContentType: Int32 get_Tag()
FSharp.Compiler.EditorServices.AssemblyContentType: System.String ToString()
FSharp.Compiler.EditorServices.AssemblySymbol
FSharp.Compiler.EditorServices.AssemblySymbol: FSharp.Compiler.CodeAnalysis.FSharpSymbol Symbol
FSharp.Compiler.EditorServices.AssemblySymbol: FSharp.Compiler.CodeAnalysis.FSharpSymbol get_Symbol()
FSharp.Compiler.EditorServices.AssemblySymbol: FSharp.Compiler.EditorServices.FSharpUnresolvedSymbol FSharpUnresolvedSymbol
FSharp.Compiler.EditorServices.AssemblySymbol: FSharp.Compiler.EditorServices.FSharpUnresolvedSymbol get_FSharpUnresolvedSymbol()
FSharp.Compiler.EditorServices.AssemblySymbol: Microsoft.FSharp.Core.FSharpFunc`2[FSharp.Compiler.EditorServices.LookupType,FSharp.Compiler.EditorServices.EntityKind] Kind
FSharp.Compiler.EditorServices.AssemblySymbol: Microsoft.FSharp.Core.FSharpFunc`2[FSharp.Compiler.EditorServices.LookupType,FSharp.Compiler.EditorServices.EntityKind] get_Kind()
FSharp.Compiler.EditorServices.AssemblySymbol: Microsoft.FSharp.Core.FSharpOption`1[System.String[]] AutoOpenParent
FSharp.Compiler.EditorServices.AssemblySymbol: Microsoft.FSharp.Core.FSharpOption`1[System.String[]] Namespace
FSharp.Compiler.EditorServices.AssemblySymbol: Microsoft.FSharp.Core.FSharpOption`1[System.String[]] NearestRequireQualifiedAccessParent
FSharp.Compiler.EditorServices.AssemblySymbol: Microsoft.FSharp.Core.FSharpOption`1[System.String[]] TopRequireQualifiedAccessParent
FSharp.Compiler.EditorServices.AssemblySymbol: Microsoft.FSharp.Core.FSharpOption`1[System.String[]] get_AutoOpenParent()
FSharp.Compiler.EditorServices.AssemblySymbol: Microsoft.FSharp.Core.FSharpOption`1[System.String[]] get_Namespace()
FSharp.Compiler.EditorServices.AssemblySymbol: Microsoft.FSharp.Core.FSharpOption`1[System.String[]] get_NearestRequireQualifiedAccessParent()
FSharp.Compiler.EditorServices.AssemblySymbol: Microsoft.FSharp.Core.FSharpOption`1[System.String[]] get_TopRequireQualifiedAccessParent()
FSharp.Compiler.EditorServices.AssemblySymbol: System.String FullName
FSharp.Compiler.EditorServices.AssemblySymbol: System.String ToString()
FSharp.Compiler.EditorServices.AssemblySymbol: System.String get_FullName()
FSharp.Compiler.EditorServices.AssemblySymbol: System.String[] CleanedIdents
FSharp.Compiler.EditorServices.AssemblySymbol: System.String[] get_CleanedIdents()
FSharp.Compiler.EditorServices.AssemblySymbol: Void .ctor(System.String, System.String[], Microsoft.FSharp.Core.FSharpOption`1[System.String[]], Microsoft.FSharp.Core.FSharpOption`1[System.String[]], Microsoft.FSharp.Core.FSharpOption`1[System.String[]], Microsoft.FSharp.Core.FSharpOption`1[System.String[]], FSharp.Compiler.CodeAnalysis.FSharpSymbol, Microsoft.FSharp.Core.FSharpFunc`2[FSharp.Compiler.EditorServices.LookupType,FSharp.Compiler.EditorServices.EntityKind], FSharp.Compiler.EditorServices.FSharpUnresolvedSymbol)
FSharp.Compiler.EditorServices.EntityCache
FSharp.Compiler.EditorServices.EntityCache: T Locking[T](Microsoft.FSharp.Core.FSharpFunc`2[FSharp.Compiler.EditorServices.IAssemblyContentCache,T])
FSharp.Compiler.EditorServices.EntityCache: Void .ctor()
FSharp.Compiler.EditorServices.EntityCache: Void Clear()
FSharp.Compiler.EditorServices.ErrorResolutionHints
FSharp.Compiler.EditorServices.ErrorResolutionHints: System.Collections.Generic.IEnumerable`1[System.String] GetSuggestedNames(Microsoft.FSharp.Core.FSharpFunc`2[Microsoft.FSharp.Core.FSharpFunc`2[System.String,Microsoft.FSharp.Core.Unit],Microsoft.FSharp.Core.Unit], System.String)
FSharp.Compiler.EditorServices.CompletionContext
FSharp.Compiler.EditorServices.CompletionContext+Inherit: FSharp.Compiler.EditorServices.InheritanceContext context
FSharp.Compiler.EditorServices.CompletionContext+Inherit: FSharp.Compiler.EditorServices.InheritanceContext get_context()
FSharp.Compiler.EditorServices.CompletionContext+Inherit: System.Tuple`2[Microsoft.FSharp.Collections.FSharpList`1[System.String],Microsoft.FSharp.Core.FSharpOption`1[System.String]] get_path()
FSharp.Compiler.EditorServices.CompletionContext+Inherit: System.Tuple`2[Microsoft.FSharp.Collections.FSharpList`1[System.String],Microsoft.FSharp.Core.FSharpOption`1[System.String]] path
FSharp.Compiler.EditorServices.CompletionContext+OpenDeclaration: Boolean get_isOpenType()
FSharp.Compiler.EditorServices.CompletionContext+OpenDeclaration: Boolean isOpenType
FSharp.Compiler.EditorServices.CompletionContext+ParameterList: FSharp.Compiler.Text.Position Item1
FSharp.Compiler.EditorServices.CompletionContext+ParameterList: FSharp.Compiler.Text.Position get_Item1()
FSharp.Compiler.EditorServices.CompletionContext+ParameterList: System.Collections.Generic.HashSet`1[System.String] Item2
FSharp.Compiler.EditorServices.CompletionContext+ParameterList: System.Collections.Generic.HashSet`1[System.String] get_Item2()
FSharp.Compiler.EditorServices.CompletionContext+RecordField: FSharp.Compiler.EditorServices.RecordContext context
FSharp.Compiler.EditorServices.CompletionContext+RecordField: FSharp.Compiler.EditorServices.RecordContext get_context()
FSharp.Compiler.EditorServices.CompletionContext+Tags: Int32 AttributeApplication
FSharp.Compiler.EditorServices.CompletionContext+Tags: Int32 Inherit
FSharp.Compiler.EditorServices.CompletionContext+Tags: Int32 Invalid
FSharp.Compiler.EditorServices.CompletionContext+Tags: Int32 OpenDeclaration
FSharp.Compiler.EditorServices.CompletionContext+Tags: Int32 ParameterList
FSharp.Compiler.EditorServices.CompletionContext+Tags: Int32 PatternType
FSharp.Compiler.EditorServices.CompletionContext+Tags: Int32 RangeOperator
FSharp.Compiler.EditorServices.CompletionContext+Tags: Int32 RecordField
FSharp.Compiler.EditorServices.CompletionContext: Boolean Equals(FSharp.Compiler.EditorServices.CompletionContext)
FSharp.Compiler.EditorServices.CompletionContext: Boolean Equals(System.Object)
FSharp.Compiler.EditorServices.CompletionContext: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.CompletionContext: Boolean IsAttributeApplication
FSharp.Compiler.EditorServices.CompletionContext: Boolean IsInherit
FSharp.Compiler.EditorServices.CompletionContext: Boolean IsInvalid
FSharp.Compiler.EditorServices.CompletionContext: Boolean IsOpenDeclaration
FSharp.Compiler.EditorServices.CompletionContext: Boolean IsParameterList
FSharp.Compiler.EditorServices.CompletionContext: Boolean IsPatternType
FSharp.Compiler.EditorServices.CompletionContext: Boolean IsRangeOperator
FSharp.Compiler.EditorServices.CompletionContext: Boolean IsRecordField
FSharp.Compiler.EditorServices.CompletionContext: Boolean get_IsAttributeApplication()
FSharp.Compiler.EditorServices.CompletionContext: Boolean get_IsInherit()
FSharp.Compiler.EditorServices.CompletionContext: Boolean get_IsInvalid()
FSharp.Compiler.EditorServices.CompletionContext: Boolean get_IsOpenDeclaration()
FSharp.Compiler.EditorServices.CompletionContext: Boolean get_IsParameterList()
FSharp.Compiler.EditorServices.CompletionContext: Boolean get_IsPatternType()
FSharp.Compiler.EditorServices.CompletionContext: Boolean get_IsRangeOperator()
FSharp.Compiler.EditorServices.CompletionContext: Boolean get_IsRecordField()
FSharp.Compiler.EditorServices.CompletionContext: FSharp.Compiler.EditorServices.CompletionContext AttributeApplication
FSharp.Compiler.EditorServices.CompletionContext: FSharp.Compiler.EditorServices.CompletionContext Invalid
FSharp.Compiler.EditorServices.CompletionContext: FSharp.Compiler.EditorServices.CompletionContext NewInherit(FSharp.Compiler.EditorServices.InheritanceContext, System.Tuple`2[Microsoft.FSharp.Collections.FSharpList`1[System.String],Microsoft.FSharp.Core.FSharpOption`1[System.String]])
FSharp.Compiler.EditorServices.CompletionContext: FSharp.Compiler.EditorServices.CompletionContext NewOpenDeclaration(Boolean)
FSharp.Compiler.EditorServices.CompletionContext: FSharp.Compiler.EditorServices.CompletionContext NewParameterList(FSharp.Compiler.Text.Position, System.Collections.Generic.HashSet`1[System.String])
FSharp.Compiler.EditorServices.CompletionContext: FSharp.Compiler.EditorServices.CompletionContext NewRecordField(FSharp.Compiler.EditorServices.RecordContext)
FSharp.Compiler.EditorServices.CompletionContext: FSharp.Compiler.EditorServices.CompletionContext PatternType
FSharp.Compiler.EditorServices.CompletionContext: FSharp.Compiler.EditorServices.CompletionContext RangeOperator
FSharp.Compiler.EditorServices.CompletionContext: FSharp.Compiler.EditorServices.CompletionContext get_AttributeApplication()
FSharp.Compiler.EditorServices.CompletionContext: FSharp.Compiler.EditorServices.CompletionContext get_Invalid()
FSharp.Compiler.EditorServices.CompletionContext: FSharp.Compiler.EditorServices.CompletionContext get_PatternType()
FSharp.Compiler.EditorServices.CompletionContext: FSharp.Compiler.EditorServices.CompletionContext get_RangeOperator()
FSharp.Compiler.EditorServices.CompletionContext: FSharp.Compiler.EditorServices.CompletionContext+Inherit
FSharp.Compiler.EditorServices.CompletionContext: FSharp.Compiler.EditorServices.CompletionContext+OpenDeclaration
FSharp.Compiler.EditorServices.CompletionContext: FSharp.Compiler.EditorServices.CompletionContext+ParameterList
FSharp.Compiler.EditorServices.CompletionContext: FSharp.Compiler.EditorServices.CompletionContext+RecordField
FSharp.Compiler.EditorServices.CompletionContext: FSharp.Compiler.EditorServices.CompletionContext+Tags
FSharp.Compiler.EditorServices.CompletionContext: Int32 GetHashCode()
FSharp.Compiler.EditorServices.CompletionContext: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.CompletionContext: Int32 Tag
FSharp.Compiler.EditorServices.CompletionContext: Int32 get_Tag()
FSharp.Compiler.EditorServices.CompletionContext: System.String ToString()
FSharp.Compiler.EditorServices.CompletionItemKind
FSharp.Compiler.EditorServices.CompletionItemKind+Method: Boolean get_isExtension()
FSharp.Compiler.EditorServices.CompletionItemKind+Method: Boolean isExtension
FSharp.Compiler.EditorServices.CompletionItemKind+Tags: Int32 Argument
FSharp.Compiler.EditorServices.CompletionItemKind+Tags: Int32 CustomOperation
FSharp.Compiler.EditorServices.CompletionItemKind+Tags: Int32 Event
FSharp.Compiler.EditorServices.CompletionItemKind+Tags: Int32 Field
FSharp.Compiler.EditorServices.CompletionItemKind+Tags: Int32 Method
FSharp.Compiler.EditorServices.CompletionItemKind+Tags: Int32 Other
FSharp.Compiler.EditorServices.CompletionItemKind+Tags: Int32 Property
FSharp.Compiler.EditorServices.CompletionItemKind: Boolean Equals(FSharp.Compiler.EditorServices.CompletionItemKind)
FSharp.Compiler.EditorServices.CompletionItemKind: Boolean Equals(System.Object)
FSharp.Compiler.EditorServices.CompletionItemKind: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.CompletionItemKind: Boolean IsArgument
FSharp.Compiler.EditorServices.CompletionItemKind: Boolean IsCustomOperation
FSharp.Compiler.EditorServices.CompletionItemKind: Boolean IsEvent
FSharp.Compiler.EditorServices.CompletionItemKind: Boolean IsField
FSharp.Compiler.EditorServices.CompletionItemKind: Boolean IsMethod
FSharp.Compiler.EditorServices.CompletionItemKind: Boolean IsOther
FSharp.Compiler.EditorServices.CompletionItemKind: Boolean IsProperty
FSharp.Compiler.EditorServices.CompletionItemKind: Boolean get_IsArgument()
FSharp.Compiler.EditorServices.CompletionItemKind: Boolean get_IsCustomOperation()
FSharp.Compiler.EditorServices.CompletionItemKind: Boolean get_IsEvent()
FSharp.Compiler.EditorServices.CompletionItemKind: Boolean get_IsField()
FSharp.Compiler.EditorServices.CompletionItemKind: Boolean get_IsMethod()
FSharp.Compiler.EditorServices.CompletionItemKind: Boolean get_IsOther()
FSharp.Compiler.EditorServices.CompletionItemKind: Boolean get_IsProperty()
FSharp.Compiler.EditorServices.CompletionItemKind: FSharp.Compiler.EditorServices.CompletionItemKind Argument
FSharp.Compiler.EditorServices.CompletionItemKind: FSharp.Compiler.EditorServices.CompletionItemKind CustomOperation
FSharp.Compiler.EditorServices.CompletionItemKind: FSharp.Compiler.EditorServices.CompletionItemKind Event
FSharp.Compiler.EditorServices.CompletionItemKind: FSharp.Compiler.EditorServices.CompletionItemKind Field
FSharp.Compiler.EditorServices.CompletionItemKind: FSharp.Compiler.EditorServices.CompletionItemKind NewMethod(Boolean)
FSharp.Compiler.EditorServices.CompletionItemKind: FSharp.Compiler.EditorServices.CompletionItemKind Other
FSharp.Compiler.EditorServices.CompletionItemKind: FSharp.Compiler.EditorServices.CompletionItemKind Property
FSharp.Compiler.EditorServices.CompletionItemKind: FSharp.Compiler.EditorServices.CompletionItemKind get_Argument()
FSharp.Compiler.EditorServices.CompletionItemKind: FSharp.Compiler.EditorServices.CompletionItemKind get_CustomOperation()
FSharp.Compiler.EditorServices.CompletionItemKind: FSharp.Compiler.EditorServices.CompletionItemKind get_Event()
FSharp.Compiler.EditorServices.CompletionItemKind: FSharp.Compiler.EditorServices.CompletionItemKind get_Field()
FSharp.Compiler.EditorServices.CompletionItemKind: FSharp.Compiler.EditorServices.CompletionItemKind get_Other()
FSharp.Compiler.EditorServices.CompletionItemKind: FSharp.Compiler.EditorServices.CompletionItemKind get_Property()
FSharp.Compiler.EditorServices.CompletionItemKind: FSharp.Compiler.EditorServices.CompletionItemKind+Method
FSharp.Compiler.EditorServices.CompletionItemKind: FSharp.Compiler.EditorServices.CompletionItemKind+Tags
FSharp.Compiler.EditorServices.CompletionItemKind: Int32 CompareTo(FSharp.Compiler.EditorServices.CompletionItemKind)
FSharp.Compiler.EditorServices.CompletionItemKind: Int32 CompareTo(System.Object)
FSharp.Compiler.EditorServices.CompletionItemKind: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.EditorServices.CompletionItemKind: Int32 GetHashCode()
FSharp.Compiler.EditorServices.CompletionItemKind: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.CompletionItemKind: Int32 Tag
FSharp.Compiler.EditorServices.CompletionItemKind: Int32 get_Tag()
FSharp.Compiler.EditorServices.CompletionItemKind: System.String ToString()
FSharp.Compiler.EditorServices.DeclarationListInfo
FSharp.Compiler.EditorServices.DeclarationListInfo: Boolean IsError
FSharp.Compiler.EditorServices.DeclarationListInfo: Boolean IsForType
FSharp.Compiler.EditorServices.DeclarationListInfo: Boolean get_IsError()
FSharp.Compiler.EditorServices.DeclarationListInfo: Boolean get_IsForType()
FSharp.Compiler.EditorServices.DeclarationListInfo: FSharp.Compiler.EditorServices.DeclarationListInfo Empty
FSharp.Compiler.EditorServices.DeclarationListInfo: FSharp.Compiler.EditorServices.DeclarationListInfo get_Empty()
FSharp.Compiler.EditorServices.DeclarationListInfo: FSharp.Compiler.EditorServices.DeclarationListItem[] Items
FSharp.Compiler.EditorServices.DeclarationListInfo: FSharp.Compiler.EditorServices.DeclarationListItem[] get_Items()
FSharp.Compiler.EditorServices.DeclarationListItem
FSharp.Compiler.EditorServices.DeclarationListItem: Boolean IsOwnMember
FSharp.Compiler.EditorServices.DeclarationListItem: Boolean IsResolved
FSharp.Compiler.EditorServices.DeclarationListItem: Boolean get_IsOwnMember()
FSharp.Compiler.EditorServices.DeclarationListItem: Boolean get_IsResolved()
FSharp.Compiler.EditorServices.DeclarationListItem: FSharp.Compiler.EditorServices.CompletionItemKind Kind
FSharp.Compiler.EditorServices.DeclarationListItem: FSharp.Compiler.EditorServices.CompletionItemKind get_Kind()
FSharp.Compiler.EditorServices.DeclarationListItem: FSharp.Compiler.EditorServices.FSharpGlyph Glyph
FSharp.Compiler.EditorServices.DeclarationListItem: FSharp.Compiler.EditorServices.FSharpGlyph get_Glyph()
FSharp.Compiler.EditorServices.DeclarationListItem: FSharp.Compiler.EditorServices.FSharpToolTipText`1[FSharp.Compiler.TextLayout.Layout] StructuredDescriptionText
FSharp.Compiler.EditorServices.DeclarationListItem: FSharp.Compiler.EditorServices.FSharpToolTipText`1[FSharp.Compiler.TextLayout.Layout] get_StructuredDescriptionText()
FSharp.Compiler.EditorServices.DeclarationListItem: FSharp.Compiler.EditorServices.FSharpToolTipText`1[System.String] DescriptionText
FSharp.Compiler.EditorServices.DeclarationListItem: FSharp.Compiler.EditorServices.FSharpToolTipText`1[System.String] get_DescriptionText()
FSharp.Compiler.EditorServices.DeclarationListItem: Int32 MinorPriority
FSharp.Compiler.EditorServices.DeclarationListItem: Int32 get_MinorPriority()
FSharp.Compiler.EditorServices.DeclarationListItem: Microsoft.FSharp.Control.FSharpAsync`1[FSharp.Compiler.EditorServices.FSharpToolTipText`1[FSharp.Compiler.TextLayout.Layout]] StructuredDescriptionTextAsync
FSharp.Compiler.EditorServices.DeclarationListItem: Microsoft.FSharp.Control.FSharpAsync`1[FSharp.Compiler.EditorServices.FSharpToolTipText`1[FSharp.Compiler.TextLayout.Layout]] get_StructuredDescriptionTextAsync()
FSharp.Compiler.EditorServices.DeclarationListItem: Microsoft.FSharp.Control.FSharpAsync`1[FSharp.Compiler.EditorServices.FSharpToolTipText`1[System.String]] DescriptionTextAsync
FSharp.Compiler.EditorServices.DeclarationListItem: Microsoft.FSharp.Control.FSharpAsync`1[FSharp.Compiler.EditorServices.FSharpToolTipText`1[System.String]] get_DescriptionTextAsync()
FSharp.Compiler.EditorServices.DeclarationListItem: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpAccessibility] Accessibility
FSharp.Compiler.EditorServices.DeclarationListItem: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpAccessibility] get_Accessibility()
FSharp.Compiler.EditorServices.DeclarationListItem: Microsoft.FSharp.Core.FSharpOption`1[System.String] NamespaceToOpen
FSharp.Compiler.EditorServices.DeclarationListItem: Microsoft.FSharp.Core.FSharpOption`1[System.String] get_NamespaceToOpen()
FSharp.Compiler.EditorServices.DeclarationListItem: System.String FullName
FSharp.Compiler.EditorServices.DeclarationListItem: System.String Name
FSharp.Compiler.EditorServices.DeclarationListItem: System.String NameInCode
FSharp.Compiler.EditorServices.DeclarationListItem: System.String get_FullName()
FSharp.Compiler.EditorServices.DeclarationListItem: System.String get_Name()
FSharp.Compiler.EditorServices.DeclarationListItem: System.String get_NameInCode()
FSharp.Compiler.EditorServices.NavigationEntityKind
FSharp.Compiler.EditorServices.NavigationEntityKind+Tags: Int32 Class
FSharp.Compiler.EditorServices.NavigationEntityKind+Tags: Int32 Enum
FSharp.Compiler.EditorServices.NavigationEntityKind+Tags: Int32 Exception
FSharp.Compiler.EditorServices.NavigationEntityKind+Tags: Int32 Interface
FSharp.Compiler.EditorServices.NavigationEntityKind+Tags: Int32 Module
FSharp.Compiler.EditorServices.NavigationEntityKind+Tags: Int32 Namespace
FSharp.Compiler.EditorServices.NavigationEntityKind+Tags: Int32 Record
FSharp.Compiler.EditorServices.NavigationEntityKind+Tags: Int32 Union
FSharp.Compiler.EditorServices.NavigationEntityKind: Boolean Equals(FSharp.Compiler.EditorServices.NavigationEntityKind)
FSharp.Compiler.EditorServices.NavigationEntityKind: Boolean Equals(System.Object)
FSharp.Compiler.EditorServices.NavigationEntityKind: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.NavigationEntityKind: Boolean IsClass
FSharp.Compiler.EditorServices.NavigationEntityKind: Boolean IsEnum
FSharp.Compiler.EditorServices.NavigationEntityKind: Boolean IsException
FSharp.Compiler.EditorServices.NavigationEntityKind: Boolean IsInterface
FSharp.Compiler.EditorServices.NavigationEntityKind: Boolean IsModule
FSharp.Compiler.EditorServices.NavigationEntityKind: Boolean IsNamespace
FSharp.Compiler.EditorServices.NavigationEntityKind: Boolean IsRecord
FSharp.Compiler.EditorServices.NavigationEntityKind: Boolean IsUnion
FSharp.Compiler.EditorServices.NavigationEntityKind: Boolean get_IsClass()
FSharp.Compiler.EditorServices.NavigationEntityKind: Boolean get_IsEnum()
FSharp.Compiler.EditorServices.NavigationEntityKind: Boolean get_IsException()
FSharp.Compiler.EditorServices.NavigationEntityKind: Boolean get_IsInterface()
FSharp.Compiler.EditorServices.NavigationEntityKind: Boolean get_IsModule()
FSharp.Compiler.EditorServices.NavigationEntityKind: Boolean get_IsNamespace()
FSharp.Compiler.EditorServices.NavigationEntityKind: Boolean get_IsRecord()
FSharp.Compiler.EditorServices.NavigationEntityKind: Boolean get_IsUnion()
FSharp.Compiler.EditorServices.NavigationEntityKind: FSharp.Compiler.EditorServices.NavigationEntityKind Class
FSharp.Compiler.EditorServices.NavigationEntityKind: FSharp.Compiler.EditorServices.NavigationEntityKind Enum
FSharp.Compiler.EditorServices.NavigationEntityKind: FSharp.Compiler.EditorServices.NavigationEntityKind Exception
FSharp.Compiler.EditorServices.NavigationEntityKind: FSharp.Compiler.EditorServices.NavigationEntityKind Interface
FSharp.Compiler.EditorServices.NavigationEntityKind: FSharp.Compiler.EditorServices.NavigationEntityKind Module
FSharp.Compiler.EditorServices.NavigationEntityKind: FSharp.Compiler.EditorServices.NavigationEntityKind Namespace
FSharp.Compiler.EditorServices.NavigationEntityKind: FSharp.Compiler.EditorServices.NavigationEntityKind Record
FSharp.Compiler.EditorServices.NavigationEntityKind: FSharp.Compiler.EditorServices.NavigationEntityKind Union
FSharp.Compiler.EditorServices.NavigationEntityKind: FSharp.Compiler.EditorServices.NavigationEntityKind get_Class()
FSharp.Compiler.EditorServices.NavigationEntityKind: FSharp.Compiler.EditorServices.NavigationEntityKind get_Enum()
FSharp.Compiler.EditorServices.NavigationEntityKind: FSharp.Compiler.EditorServices.NavigationEntityKind get_Exception()
FSharp.Compiler.EditorServices.NavigationEntityKind: FSharp.Compiler.EditorServices.NavigationEntityKind get_Interface()
FSharp.Compiler.EditorServices.NavigationEntityKind: FSharp.Compiler.EditorServices.NavigationEntityKind get_Module()
FSharp.Compiler.EditorServices.NavigationEntityKind: FSharp.Compiler.EditorServices.NavigationEntityKind get_Namespace()
FSharp.Compiler.EditorServices.NavigationEntityKind: FSharp.Compiler.EditorServices.NavigationEntityKind get_Record()
FSharp.Compiler.EditorServices.NavigationEntityKind: FSharp.Compiler.EditorServices.NavigationEntityKind get_Union()
FSharp.Compiler.EditorServices.NavigationEntityKind: FSharp.Compiler.EditorServices.NavigationEntityKind+Tags
FSharp.Compiler.EditorServices.NavigationEntityKind: Int32 CompareTo(FSharp.Compiler.EditorServices.NavigationEntityKind)
FSharp.Compiler.EditorServices.NavigationEntityKind: Int32 CompareTo(System.Object)
FSharp.Compiler.EditorServices.NavigationEntityKind: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.EditorServices.NavigationEntityKind: Int32 GetHashCode()
FSharp.Compiler.EditorServices.NavigationEntityKind: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.NavigationEntityKind: Int32 Tag
FSharp.Compiler.EditorServices.NavigationEntityKind: Int32 get_Tag()
FSharp.Compiler.EditorServices.NavigationEntityKind: System.String ToString()
FSharp.Compiler.EditorServices.EntityKind
FSharp.Compiler.EditorServices.EntityKind+FunctionOrValue: Boolean get_isActivePattern()
FSharp.Compiler.EditorServices.EntityKind+FunctionOrValue: Boolean isActivePattern
FSharp.Compiler.EditorServices.EntityKind+Module: FSharp.Compiler.EditorServices.ModuleKind Item
FSharp.Compiler.EditorServices.EntityKind+Module: FSharp.Compiler.EditorServices.ModuleKind get_Item()
FSharp.Compiler.EditorServices.EntityKind+Tags: Int32 Attribute
FSharp.Compiler.EditorServices.EntityKind+Tags: Int32 FunctionOrValue
FSharp.Compiler.EditorServices.EntityKind+Tags: Int32 Module
FSharp.Compiler.EditorServices.EntityKind+Tags: Int32 Type
FSharp.Compiler.EditorServices.EntityKind: Boolean Equals(FSharp.Compiler.EditorServices.EntityKind)
FSharp.Compiler.EditorServices.EntityKind: Boolean Equals(System.Object)
FSharp.Compiler.EditorServices.EntityKind: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.EntityKind: Boolean IsAttribute
FSharp.Compiler.EditorServices.EntityKind: Boolean IsFunctionOrValue
FSharp.Compiler.EditorServices.EntityKind: Boolean IsModule
FSharp.Compiler.EditorServices.EntityKind: Boolean IsType
FSharp.Compiler.EditorServices.EntityKind: Boolean get_IsAttribute()
FSharp.Compiler.EditorServices.EntityKind: Boolean get_IsFunctionOrValue()
FSharp.Compiler.EditorServices.EntityKind: Boolean get_IsModule()
FSharp.Compiler.EditorServices.EntityKind: Boolean get_IsType()
FSharp.Compiler.EditorServices.EntityKind: FSharp.Compiler.EditorServices.EntityKind Attribute
FSharp.Compiler.EditorServices.EntityKind: FSharp.Compiler.EditorServices.EntityKind NewFunctionOrValue(Boolean)
FSharp.Compiler.EditorServices.EntityKind: FSharp.Compiler.EditorServices.EntityKind NewModule(FSharp.Compiler.EditorServices.ModuleKind)
FSharp.Compiler.EditorServices.EntityKind: FSharp.Compiler.EditorServices.EntityKind Type
FSharp.Compiler.EditorServices.EntityKind: FSharp.Compiler.EditorServices.EntityKind get_Attribute()
FSharp.Compiler.EditorServices.EntityKind: FSharp.Compiler.EditorServices.EntityKind get_Type()
FSharp.Compiler.EditorServices.EntityKind: FSharp.Compiler.EditorServices.EntityKind+FunctionOrValue
FSharp.Compiler.EditorServices.EntityKind: FSharp.Compiler.EditorServices.EntityKind+Module
FSharp.Compiler.EditorServices.EntityKind: FSharp.Compiler.EditorServices.EntityKind+Tags
FSharp.Compiler.EditorServices.EntityKind: Int32 CompareTo(FSharp.Compiler.EditorServices.EntityKind)
FSharp.Compiler.EditorServices.EntityKind: Int32 CompareTo(System.Object)
FSharp.Compiler.EditorServices.EntityKind: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.EditorServices.EntityKind: Int32 GetHashCode()
FSharp.Compiler.EditorServices.EntityKind: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.EntityKind: Int32 Tag
FSharp.Compiler.EditorServices.EntityKind: Int32 get_Tag()
FSharp.Compiler.EditorServices.EntityKind: System.String ToString()
FSharp.Compiler.EditorServices.FSharpGlyph
FSharp.Compiler.EditorServices.FSharpGlyph+Tags: Int32 Class
FSharp.Compiler.EditorServices.FSharpGlyph+Tags: Int32 Constant
FSharp.Compiler.EditorServices.FSharpGlyph+Tags: Int32 Delegate
FSharp.Compiler.EditorServices.FSharpGlyph+Tags: Int32 Enum
FSharp.Compiler.EditorServices.FSharpGlyph+Tags: Int32 EnumMember
FSharp.Compiler.EditorServices.FSharpGlyph+Tags: Int32 Error
FSharp.Compiler.EditorServices.FSharpGlyph+Tags: Int32 Event
FSharp.Compiler.EditorServices.FSharpGlyph+Tags: Int32 Exception
FSharp.Compiler.EditorServices.FSharpGlyph+Tags: Int32 ExtensionMethod
FSharp.Compiler.EditorServices.FSharpGlyph+Tags: Int32 Field
FSharp.Compiler.EditorServices.FSharpGlyph+Tags: Int32 Interface
FSharp.Compiler.EditorServices.FSharpGlyph+Tags: Int32 Method
FSharp.Compiler.EditorServices.FSharpGlyph+Tags: Int32 Module
FSharp.Compiler.EditorServices.FSharpGlyph+Tags: Int32 NameSpace
FSharp.Compiler.EditorServices.FSharpGlyph+Tags: Int32 OverridenMethod
FSharp.Compiler.EditorServices.FSharpGlyph+Tags: Int32 Property
FSharp.Compiler.EditorServices.FSharpGlyph+Tags: Int32 Struct
FSharp.Compiler.EditorServices.FSharpGlyph+Tags: Int32 Type
FSharp.Compiler.EditorServices.FSharpGlyph+Tags: Int32 Typedef
FSharp.Compiler.EditorServices.FSharpGlyph+Tags: Int32 Union
FSharp.Compiler.EditorServices.FSharpGlyph+Tags: Int32 Variable
FSharp.Compiler.EditorServices.FSharpGlyph: Boolean Equals(FSharp.Compiler.EditorServices.FSharpGlyph)
FSharp.Compiler.EditorServices.FSharpGlyph: Boolean Equals(System.Object)
FSharp.Compiler.EditorServices.FSharpGlyph: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.FSharpGlyph: Boolean IsClass
FSharp.Compiler.EditorServices.FSharpGlyph: Boolean IsConstant
FSharp.Compiler.EditorServices.FSharpGlyph: Boolean IsDelegate
FSharp.Compiler.EditorServices.FSharpGlyph: Boolean IsEnum
FSharp.Compiler.EditorServices.FSharpGlyph: Boolean IsEnumMember
FSharp.Compiler.EditorServices.FSharpGlyph: Boolean IsError
FSharp.Compiler.EditorServices.FSharpGlyph: Boolean IsEvent
FSharp.Compiler.EditorServices.FSharpGlyph: Boolean IsException
FSharp.Compiler.EditorServices.FSharpGlyph: Boolean IsExtensionMethod
FSharp.Compiler.EditorServices.FSharpGlyph: Boolean IsField
FSharp.Compiler.EditorServices.FSharpGlyph: Boolean IsInterface
FSharp.Compiler.EditorServices.FSharpGlyph: Boolean IsMethod
FSharp.Compiler.EditorServices.FSharpGlyph: Boolean IsModule
FSharp.Compiler.EditorServices.FSharpGlyph: Boolean IsNameSpace
FSharp.Compiler.EditorServices.FSharpGlyph: Boolean IsOverridenMethod
FSharp.Compiler.EditorServices.FSharpGlyph: Boolean IsProperty
FSharp.Compiler.EditorServices.FSharpGlyph: Boolean IsStruct
FSharp.Compiler.EditorServices.FSharpGlyph: Boolean IsType
FSharp.Compiler.EditorServices.FSharpGlyph: Boolean IsTypedef
FSharp.Compiler.EditorServices.FSharpGlyph: Boolean IsUnion
FSharp.Compiler.EditorServices.FSharpGlyph: Boolean IsVariable
FSharp.Compiler.EditorServices.FSharpGlyph: Boolean get_IsClass()
FSharp.Compiler.EditorServices.FSharpGlyph: Boolean get_IsConstant()
FSharp.Compiler.EditorServices.FSharpGlyph: Boolean get_IsDelegate()
FSharp.Compiler.EditorServices.FSharpGlyph: Boolean get_IsEnum()
FSharp.Compiler.EditorServices.FSharpGlyph: Boolean get_IsEnumMember()
FSharp.Compiler.EditorServices.FSharpGlyph: Boolean get_IsError()
FSharp.Compiler.EditorServices.FSharpGlyph: Boolean get_IsEvent()
FSharp.Compiler.EditorServices.FSharpGlyph: Boolean get_IsException()
FSharp.Compiler.EditorServices.FSharpGlyph: Boolean get_IsExtensionMethod()
FSharp.Compiler.EditorServices.FSharpGlyph: Boolean get_IsField()
FSharp.Compiler.EditorServices.FSharpGlyph: Boolean get_IsInterface()
FSharp.Compiler.EditorServices.FSharpGlyph: Boolean get_IsMethod()
FSharp.Compiler.EditorServices.FSharpGlyph: Boolean get_IsModule()
FSharp.Compiler.EditorServices.FSharpGlyph: Boolean get_IsNameSpace()
FSharp.Compiler.EditorServices.FSharpGlyph: Boolean get_IsOverridenMethod()
FSharp.Compiler.EditorServices.FSharpGlyph: Boolean get_IsProperty()
FSharp.Compiler.EditorServices.FSharpGlyph: Boolean get_IsStruct()
FSharp.Compiler.EditorServices.FSharpGlyph: Boolean get_IsType()
FSharp.Compiler.EditorServices.FSharpGlyph: Boolean get_IsTypedef()
FSharp.Compiler.EditorServices.FSharpGlyph: Boolean get_IsUnion()
FSharp.Compiler.EditorServices.FSharpGlyph: Boolean get_IsVariable()
FSharp.Compiler.EditorServices.FSharpGlyph: FSharp.Compiler.EditorServices.FSharpGlyph Class
FSharp.Compiler.EditorServices.FSharpGlyph: FSharp.Compiler.EditorServices.FSharpGlyph Constant
FSharp.Compiler.EditorServices.FSharpGlyph: FSharp.Compiler.EditorServices.FSharpGlyph Delegate
FSharp.Compiler.EditorServices.FSharpGlyph: FSharp.Compiler.EditorServices.FSharpGlyph Enum
FSharp.Compiler.EditorServices.FSharpGlyph: FSharp.Compiler.EditorServices.FSharpGlyph EnumMember
FSharp.Compiler.EditorServices.FSharpGlyph: FSharp.Compiler.EditorServices.FSharpGlyph Error
FSharp.Compiler.EditorServices.FSharpGlyph: FSharp.Compiler.EditorServices.FSharpGlyph Event
FSharp.Compiler.EditorServices.FSharpGlyph: FSharp.Compiler.EditorServices.FSharpGlyph Exception
FSharp.Compiler.EditorServices.FSharpGlyph: FSharp.Compiler.EditorServices.FSharpGlyph ExtensionMethod
FSharp.Compiler.EditorServices.FSharpGlyph: FSharp.Compiler.EditorServices.FSharpGlyph Field
FSharp.Compiler.EditorServices.FSharpGlyph: FSharp.Compiler.EditorServices.FSharpGlyph Interface
FSharp.Compiler.EditorServices.FSharpGlyph: FSharp.Compiler.EditorServices.FSharpGlyph Method
FSharp.Compiler.EditorServices.FSharpGlyph: FSharp.Compiler.EditorServices.FSharpGlyph Module
FSharp.Compiler.EditorServices.FSharpGlyph: FSharp.Compiler.EditorServices.FSharpGlyph NameSpace
FSharp.Compiler.EditorServices.FSharpGlyph: FSharp.Compiler.EditorServices.FSharpGlyph OverridenMethod
FSharp.Compiler.EditorServices.FSharpGlyph: FSharp.Compiler.EditorServices.FSharpGlyph Property
FSharp.Compiler.EditorServices.FSharpGlyph: FSharp.Compiler.EditorServices.FSharpGlyph Struct
FSharp.Compiler.EditorServices.FSharpGlyph: FSharp.Compiler.EditorServices.FSharpGlyph Type
FSharp.Compiler.EditorServices.FSharpGlyph: FSharp.Compiler.EditorServices.FSharpGlyph Typedef
FSharp.Compiler.EditorServices.FSharpGlyph: FSharp.Compiler.EditorServices.FSharpGlyph Union
FSharp.Compiler.EditorServices.FSharpGlyph: FSharp.Compiler.EditorServices.FSharpGlyph Variable
FSharp.Compiler.EditorServices.FSharpGlyph: FSharp.Compiler.EditorServices.FSharpGlyph get_Class()
FSharp.Compiler.EditorServices.FSharpGlyph: FSharp.Compiler.EditorServices.FSharpGlyph get_Constant()
FSharp.Compiler.EditorServices.FSharpGlyph: FSharp.Compiler.EditorServices.FSharpGlyph get_Delegate()
FSharp.Compiler.EditorServices.FSharpGlyph: FSharp.Compiler.EditorServices.FSharpGlyph get_Enum()
FSharp.Compiler.EditorServices.FSharpGlyph: FSharp.Compiler.EditorServices.FSharpGlyph get_EnumMember()
FSharp.Compiler.EditorServices.FSharpGlyph: FSharp.Compiler.EditorServices.FSharpGlyph get_Error()
FSharp.Compiler.EditorServices.FSharpGlyph: FSharp.Compiler.EditorServices.FSharpGlyph get_Event()
FSharp.Compiler.EditorServices.FSharpGlyph: FSharp.Compiler.EditorServices.FSharpGlyph get_Exception()
FSharp.Compiler.EditorServices.FSharpGlyph: FSharp.Compiler.EditorServices.FSharpGlyph get_ExtensionMethod()
FSharp.Compiler.EditorServices.FSharpGlyph: FSharp.Compiler.EditorServices.FSharpGlyph get_Field()
FSharp.Compiler.EditorServices.FSharpGlyph: FSharp.Compiler.EditorServices.FSharpGlyph get_Interface()
FSharp.Compiler.EditorServices.FSharpGlyph: FSharp.Compiler.EditorServices.FSharpGlyph get_Method()
FSharp.Compiler.EditorServices.FSharpGlyph: FSharp.Compiler.EditorServices.FSharpGlyph get_Module()
FSharp.Compiler.EditorServices.FSharpGlyph: FSharp.Compiler.EditorServices.FSharpGlyph get_NameSpace()
FSharp.Compiler.EditorServices.FSharpGlyph: FSharp.Compiler.EditorServices.FSharpGlyph get_OverridenMethod()
FSharp.Compiler.EditorServices.FSharpGlyph: FSharp.Compiler.EditorServices.FSharpGlyph get_Property()
FSharp.Compiler.EditorServices.FSharpGlyph: FSharp.Compiler.EditorServices.FSharpGlyph get_Struct()
FSharp.Compiler.EditorServices.FSharpGlyph: FSharp.Compiler.EditorServices.FSharpGlyph get_Type()
FSharp.Compiler.EditorServices.FSharpGlyph: FSharp.Compiler.EditorServices.FSharpGlyph get_Typedef()
FSharp.Compiler.EditorServices.FSharpGlyph: FSharp.Compiler.EditorServices.FSharpGlyph get_Union()
FSharp.Compiler.EditorServices.FSharpGlyph: FSharp.Compiler.EditorServices.FSharpGlyph get_Variable()
FSharp.Compiler.EditorServices.FSharpGlyph: FSharp.Compiler.EditorServices.FSharpGlyph+Tags
FSharp.Compiler.EditorServices.FSharpGlyph: Int32 CompareTo(FSharp.Compiler.EditorServices.FSharpGlyph)
FSharp.Compiler.EditorServices.FSharpGlyph: Int32 CompareTo(System.Object)
FSharp.Compiler.EditorServices.FSharpGlyph: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.EditorServices.FSharpGlyph: Int32 GetHashCode()
FSharp.Compiler.EditorServices.FSharpGlyph: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.FSharpGlyph: Int32 Tag
FSharp.Compiler.EditorServices.FSharpGlyph: Int32 get_Tag()
FSharp.Compiler.EditorServices.FSharpGlyph: System.String ToString()
FSharp.Compiler.EditorServices.InheritanceContext
FSharp.Compiler.EditorServices.InheritanceContext+Tags: Int32 Class
FSharp.Compiler.EditorServices.InheritanceContext+Tags: Int32 Interface
FSharp.Compiler.EditorServices.InheritanceContext+Tags: Int32 Unknown
FSharp.Compiler.EditorServices.InheritanceContext: Boolean Equals(FSharp.Compiler.EditorServices.InheritanceContext)
FSharp.Compiler.EditorServices.InheritanceContext: Boolean Equals(System.Object)
FSharp.Compiler.EditorServices.InheritanceContext: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.InheritanceContext: Boolean IsClass
FSharp.Compiler.EditorServices.InheritanceContext: Boolean IsInterface
FSharp.Compiler.EditorServices.InheritanceContext: Boolean IsUnknown
FSharp.Compiler.EditorServices.InheritanceContext: Boolean get_IsClass()
FSharp.Compiler.EditorServices.InheritanceContext: Boolean get_IsInterface()
FSharp.Compiler.EditorServices.InheritanceContext: Boolean get_IsUnknown()
FSharp.Compiler.EditorServices.InheritanceContext: FSharp.Compiler.EditorServices.InheritanceContext Class
FSharp.Compiler.EditorServices.InheritanceContext: FSharp.Compiler.EditorServices.InheritanceContext Interface
FSharp.Compiler.EditorServices.InheritanceContext: FSharp.Compiler.EditorServices.InheritanceContext Unknown
FSharp.Compiler.EditorServices.InheritanceContext: FSharp.Compiler.EditorServices.InheritanceContext get_Class()
FSharp.Compiler.EditorServices.InheritanceContext: FSharp.Compiler.EditorServices.InheritanceContext get_Interface()
FSharp.Compiler.EditorServices.InheritanceContext: FSharp.Compiler.EditorServices.InheritanceContext get_Unknown()
FSharp.Compiler.EditorServices.InheritanceContext: FSharp.Compiler.EditorServices.InheritanceContext+Tags
FSharp.Compiler.EditorServices.InheritanceContext: Int32 CompareTo(FSharp.Compiler.EditorServices.InheritanceContext)
FSharp.Compiler.EditorServices.InheritanceContext: Int32 CompareTo(System.Object)
FSharp.Compiler.EditorServices.InheritanceContext: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.EditorServices.InheritanceContext: Int32 GetHashCode()
FSharp.Compiler.EditorServices.InheritanceContext: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.InheritanceContext: Int32 Tag
FSharp.Compiler.EditorServices.InheritanceContext: Int32 get_Tag()
FSharp.Compiler.EditorServices.InheritanceContext: System.String ToString()
FSharp.Compiler.EditorServices.InsertionContext
FSharp.Compiler.EditorServices.InsertionContext: Boolean Equals(FSharp.Compiler.EditorServices.InsertionContext)
FSharp.Compiler.EditorServices.InsertionContext: Boolean Equals(System.Object)
FSharp.Compiler.EditorServices.InsertionContext: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.InsertionContext: FSharp.Compiler.EditorServices.ScopeKind ScopeKind
FSharp.Compiler.EditorServices.InsertionContext: FSharp.Compiler.EditorServices.ScopeKind get_ScopeKind()
FSharp.Compiler.EditorServices.InsertionContext: FSharp.Compiler.Text.Position Pos
FSharp.Compiler.EditorServices.InsertionContext: FSharp.Compiler.Text.Position get_Pos()
FSharp.Compiler.EditorServices.InsertionContext: Int32 GetHashCode()
FSharp.Compiler.EditorServices.InsertionContext: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.InsertionContext: System.String ToString()
FSharp.Compiler.EditorServices.InsertionContext: Void .ctor(FSharp.Compiler.EditorServices.ScopeKind, FSharp.Compiler.Text.Position)
FSharp.Compiler.EditorServices.FSharpKeywords
FSharp.Compiler.EditorServices.FSharpKeywords: Boolean DoesIdentifierNeedQuotation(System.String)
FSharp.Compiler.EditorServices.FSharpKeywords: Microsoft.FSharp.Collections.FSharpList`1[System.String] KeywordNames
FSharp.Compiler.EditorServices.FSharpKeywords: Microsoft.FSharp.Collections.FSharpList`1[System.String] get_KeywordNames()
FSharp.Compiler.EditorServices.FSharpKeywords: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[System.String,System.String]] KeywordsWithDescription
FSharp.Compiler.EditorServices.FSharpKeywords: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[System.String,System.String]] get_KeywordsWithDescription()
FSharp.Compiler.EditorServices.FSharpKeywords: System.String NormalizeIdentifierBackticks(System.String)
FSharp.Compiler.EditorServices.FSharpKeywords: System.String QuoteIdentifierIfNeeded(System.String)
FSharp.Compiler.EditorServices.FSharpLexer
FSharp.Compiler.EditorServices.FSharpLexer: Void Lex(FSharp.Compiler.Text.ISourceText, Microsoft.FSharp.Core.FSharpFunc`2[FSharp.Compiler.EditorServices.FSharpToken,Microsoft.FSharp.Core.Unit], Microsoft.FSharp.Core.FSharpOption`1[System.String], Microsoft.FSharp.Core.FSharpOption`1[System.String], Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Collections.FSharpList`1[System.String]], Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.EditorServices.FSharpLexerFlags], Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Collections.FSharpMap`2[System.String,System.String]], Microsoft.FSharp.Core.FSharpOption`1[System.Threading.CancellationToken])
FSharp.Compiler.EditorServices.FSharpLexerFlags
FSharp.Compiler.EditorServices.FSharpLexerFlags: FSharp.Compiler.EditorServices.FSharpLexerFlags Compiling
FSharp.Compiler.EditorServices.FSharpLexerFlags: FSharp.Compiler.EditorServices.FSharpLexerFlags CompilingFSharpCore
FSharp.Compiler.EditorServices.FSharpLexerFlags: FSharp.Compiler.EditorServices.FSharpLexerFlags Default
FSharp.Compiler.EditorServices.FSharpLexerFlags: FSharp.Compiler.EditorServices.FSharpLexerFlags LightSyntaxOn
FSharp.Compiler.EditorServices.FSharpLexerFlags: FSharp.Compiler.EditorServices.FSharpLexerFlags SkipTrivia
FSharp.Compiler.EditorServices.FSharpLexerFlags: FSharp.Compiler.EditorServices.FSharpLexerFlags UseLexFilter
FSharp.Compiler.EditorServices.FSharpLexerFlags: Int32 value__
FSharp.Compiler.EditorServices.FSharpLineTokenizer
FSharp.Compiler.EditorServices.FSharpLineTokenizer: FSharp.Compiler.EditorServices.FSharpTokenizerColorState ColorStateOfLexState(FSharp.Compiler.EditorServices.FSharpTokenizerLexState)
FSharp.Compiler.EditorServices.FSharpLineTokenizer: FSharp.Compiler.EditorServices.FSharpTokenizerLexState LexStateOfColorState(FSharp.Compiler.EditorServices.FSharpTokenizerColorState)
FSharp.Compiler.EditorServices.FSharpLineTokenizer: System.Tuple`2[Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.EditorServices.FSharpTokenInfo],FSharp.Compiler.EditorServices.FSharpTokenizerLexState] ScanToken(FSharp.Compiler.EditorServices.FSharpTokenizerLexState)
FSharp.Compiler.EditorServices.MaybeUnresolvedIdent
FSharp.Compiler.EditorServices.MaybeUnresolvedIdent: Boolean Equals(FSharp.Compiler.EditorServices.MaybeUnresolvedIdent)
FSharp.Compiler.EditorServices.MaybeUnresolvedIdent: Boolean Equals(System.Object)
FSharp.Compiler.EditorServices.MaybeUnresolvedIdent: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.MaybeUnresolvedIdent: Boolean Resolved
FSharp.Compiler.EditorServices.MaybeUnresolvedIdent: Boolean get_Resolved()
FSharp.Compiler.EditorServices.MaybeUnresolvedIdent: Int32 CompareTo(FSharp.Compiler.EditorServices.MaybeUnresolvedIdent)
FSharp.Compiler.EditorServices.MaybeUnresolvedIdent: Int32 CompareTo(System.Object)
FSharp.Compiler.EditorServices.MaybeUnresolvedIdent: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.EditorServices.MaybeUnresolvedIdent: Int32 GetHashCode()
FSharp.Compiler.EditorServices.MaybeUnresolvedIdent: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.MaybeUnresolvedIdent: System.String Ident
FSharp.Compiler.EditorServices.MaybeUnresolvedIdent: System.String ToString()
FSharp.Compiler.EditorServices.MaybeUnresolvedIdent: System.String get_Ident()
FSharp.Compiler.EditorServices.MaybeUnresolvedIdent: Void .ctor(System.String, Boolean)
FSharp.Compiler.EditorServices.MethodGroup
FSharp.Compiler.EditorServices.MethodGroup: FSharp.Compiler.EditorServices.MethodGroupItem[] Methods
FSharp.Compiler.EditorServices.MethodGroup: FSharp.Compiler.EditorServices.MethodGroupItem[] get_Methods()
FSharp.Compiler.EditorServices.MethodGroup: System.String MethodName
FSharp.Compiler.EditorServices.MethodGroup: System.String get_MethodName()
FSharp.Compiler.EditorServices.MethodGroupItem
FSharp.Compiler.EditorServices.MethodGroupItem: Boolean HasParamArrayArg
FSharp.Compiler.EditorServices.MethodGroupItem: Boolean HasParameters
FSharp.Compiler.EditorServices.MethodGroupItem: Boolean get_HasParamArrayArg()
FSharp.Compiler.EditorServices.MethodGroupItem: Boolean get_HasParameters()
FSharp.Compiler.EditorServices.MethodGroupItem: FSharp.Compiler.CodeAnalysis.FSharpXmlDoc XmlDoc
FSharp.Compiler.EditorServices.MethodGroupItem: FSharp.Compiler.CodeAnalysis.FSharpXmlDoc get_XmlDoc()
FSharp.Compiler.EditorServices.MethodGroupItem: FSharp.Compiler.EditorServices.MethodGroupItemParameter[] Parameters
FSharp.Compiler.EditorServices.MethodGroupItem: FSharp.Compiler.EditorServices.MethodGroupItemParameter[] StaticParameters
FSharp.Compiler.EditorServices.MethodGroupItem: FSharp.Compiler.EditorServices.MethodGroupItemParameter[] get_Parameters()
FSharp.Compiler.EditorServices.MethodGroupItem: FSharp.Compiler.EditorServices.MethodGroupItemParameter[] get_StaticParameters()
FSharp.Compiler.EditorServices.MethodGroupItem: FSharp.Compiler.EditorServices.FSharpToolTipText`1[FSharp.Compiler.TextLayout.Layout] StructuredDescription
FSharp.Compiler.EditorServices.MethodGroupItem: FSharp.Compiler.EditorServices.FSharpToolTipText`1[FSharp.Compiler.TextLayout.Layout] get_StructuredDescription()
FSharp.Compiler.EditorServices.MethodGroupItem: FSharp.Compiler.EditorServices.FSharpToolTipText`1[System.String] Description
FSharp.Compiler.EditorServices.MethodGroupItem: FSharp.Compiler.EditorServices.FSharpToolTipText`1[System.String] get_Description()
FSharp.Compiler.EditorServices.MethodGroupItem: FSharp.Compiler.TextLayout.Layout StructuredReturnTypeText
FSharp.Compiler.EditorServices.MethodGroupItem: FSharp.Compiler.TextLayout.Layout get_StructuredReturnTypeText()
FSharp.Compiler.EditorServices.MethodGroupItem: System.String ReturnTypeText
FSharp.Compiler.EditorServices.MethodGroupItem: System.String get_ReturnTypeText()
FSharp.Compiler.EditorServices.MethodGroupItemParameter
FSharp.Compiler.EditorServices.MethodGroupItemParameter: Boolean IsOptional
FSharp.Compiler.EditorServices.MethodGroupItemParameter: Boolean get_IsOptional()
FSharp.Compiler.EditorServices.MethodGroupItemParameter: FSharp.Compiler.TextLayout.Layout StructuredDisplay
FSharp.Compiler.EditorServices.MethodGroupItemParameter: FSharp.Compiler.TextLayout.Layout get_StructuredDisplay()
FSharp.Compiler.EditorServices.MethodGroupItemParameter: System.String CanonicalTypeTextForSorting
FSharp.Compiler.EditorServices.MethodGroupItemParameter: System.String Display
FSharp.Compiler.EditorServices.MethodGroupItemParameter: System.String ParameterName
FSharp.Compiler.EditorServices.MethodGroupItemParameter: System.String get_CanonicalTypeTextForSorting()
FSharp.Compiler.EditorServices.MethodGroupItemParameter: System.String get_Display()
FSharp.Compiler.EditorServices.MethodGroupItemParameter: System.String get_ParameterName()
FSharp.Compiler.EditorServices.ModuleKind
FSharp.Compiler.EditorServices.ModuleKind: Boolean Equals(FSharp.Compiler.EditorServices.ModuleKind)
FSharp.Compiler.EditorServices.ModuleKind: Boolean Equals(System.Object)
FSharp.Compiler.EditorServices.ModuleKind: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.ModuleKind: Boolean HasModuleSuffix
FSharp.Compiler.EditorServices.ModuleKind: Boolean IsAutoOpen
FSharp.Compiler.EditorServices.ModuleKind: Boolean get_HasModuleSuffix()
FSharp.Compiler.EditorServices.ModuleKind: Boolean get_IsAutoOpen()
FSharp.Compiler.EditorServices.ModuleKind: Int32 CompareTo(FSharp.Compiler.EditorServices.ModuleKind)
FSharp.Compiler.EditorServices.ModuleKind: Int32 CompareTo(System.Object)
FSharp.Compiler.EditorServices.ModuleKind: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.EditorServices.ModuleKind: Int32 GetHashCode()
FSharp.Compiler.EditorServices.ModuleKind: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.ModuleKind: System.String ToString()
FSharp.Compiler.EditorServices.ModuleKind: Void .ctor(Boolean, Boolean)
FSharp.Compiler.EditorServices.Navigation
FSharp.Compiler.EditorServices.Navigation: FSharp.Compiler.EditorServices.NavigationItems getNavigation(FSharp.Compiler.Syntax.ParsedInput)
FSharp.Compiler.EditorServices.NavigationItem
FSharp.Compiler.EditorServices.NavigationItem: Boolean IsAbstract
FSharp.Compiler.EditorServices.NavigationItem: Boolean IsSingleTopLevel
FSharp.Compiler.EditorServices.NavigationItem: Boolean get_IsAbstract()
FSharp.Compiler.EditorServices.NavigationItem: Boolean get_IsSingleTopLevel()
FSharp.Compiler.EditorServices.NavigationItem: FSharp.Compiler.EditorServices.NavigationEntityKind EnclosingEntityKind
FSharp.Compiler.EditorServices.NavigationItem: FSharp.Compiler.EditorServices.NavigationEntityKind get_EnclosingEntityKind()
FSharp.Compiler.EditorServices.NavigationItem: FSharp.Compiler.EditorServices.FSharpGlyph Glyph
FSharp.Compiler.EditorServices.NavigationItem: FSharp.Compiler.EditorServices.FSharpGlyph get_Glyph()
FSharp.Compiler.EditorServices.NavigationItem: FSharp.Compiler.EditorServices.NavigationItemKind Kind
FSharp.Compiler.EditorServices.NavigationItem: FSharp.Compiler.EditorServices.NavigationItemKind get_Kind()
FSharp.Compiler.EditorServices.NavigationItem: FSharp.Compiler.Text.Range BodyRange
FSharp.Compiler.EditorServices.NavigationItem: FSharp.Compiler.Text.Range Range
FSharp.Compiler.EditorServices.NavigationItem: FSharp.Compiler.Text.Range get_BodyRange()
FSharp.Compiler.EditorServices.NavigationItem: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.EditorServices.NavigationItem: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess] Access
FSharp.Compiler.EditorServices.NavigationItem: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess] get_Access()
FSharp.Compiler.EditorServices.NavigationItem: System.String Name
FSharp.Compiler.EditorServices.NavigationItem: System.String UniqueName
FSharp.Compiler.EditorServices.NavigationItem: System.String get_Name()
FSharp.Compiler.EditorServices.NavigationItem: System.String get_UniqueName()
FSharp.Compiler.EditorServices.NavigationItemKind
FSharp.Compiler.EditorServices.NavigationItemKind+Tags: Int32 ExnDecl
FSharp.Compiler.EditorServices.NavigationItemKind+Tags: Int32 FieldDecl
FSharp.Compiler.EditorServices.NavigationItemKind+Tags: Int32 MethodDecl
FSharp.Compiler.EditorServices.NavigationItemKind+Tags: Int32 ModuleDecl
FSharp.Compiler.EditorServices.NavigationItemKind+Tags: Int32 ModuleFile
FSharp.Compiler.EditorServices.NavigationItemKind+Tags: Int32 Namespace
FSharp.Compiler.EditorServices.NavigationItemKind+Tags: Int32 OtherDecl
FSharp.Compiler.EditorServices.NavigationItemKind+Tags: Int32 PropertyDecl
FSharp.Compiler.EditorServices.NavigationItemKind+Tags: Int32 TypeDecl
FSharp.Compiler.EditorServices.NavigationItemKind: Boolean Equals(FSharp.Compiler.EditorServices.NavigationItemKind)
FSharp.Compiler.EditorServices.NavigationItemKind: Boolean Equals(System.Object)
FSharp.Compiler.EditorServices.NavigationItemKind: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.NavigationItemKind: Boolean IsExnDecl
FSharp.Compiler.EditorServices.NavigationItemKind: Boolean IsFieldDecl
FSharp.Compiler.EditorServices.NavigationItemKind: Boolean IsMethodDecl
FSharp.Compiler.EditorServices.NavigationItemKind: Boolean IsModuleDecl
FSharp.Compiler.EditorServices.NavigationItemKind: Boolean IsModuleFileDecl
FSharp.Compiler.EditorServices.NavigationItemKind: Boolean IsNamespaceDecl
FSharp.Compiler.EditorServices.NavigationItemKind: Boolean IsOtherDecl
FSharp.Compiler.EditorServices.NavigationItemKind: Boolean IsPropertyDecl
FSharp.Compiler.EditorServices.NavigationItemKind: Boolean IsTypeDecl
FSharp.Compiler.EditorServices.NavigationItemKind: Boolean get_IsExnDecl()
FSharp.Compiler.EditorServices.NavigationItemKind: Boolean get_IsFieldDecl()
FSharp.Compiler.EditorServices.NavigationItemKind: Boolean get_IsMethodDecl()
FSharp.Compiler.EditorServices.NavigationItemKind: Boolean get_IsModuleDecl()
FSharp.Compiler.EditorServices.NavigationItemKind: Boolean get_IsModuleFileDecl()
FSharp.Compiler.EditorServices.NavigationItemKind: Boolean get_IsNamespaceDecl()
FSharp.Compiler.EditorServices.NavigationItemKind: Boolean get_IsOtherDecl()
FSharp.Compiler.EditorServices.NavigationItemKind: Boolean get_IsPropertyDecl()
FSharp.Compiler.EditorServices.NavigationItemKind: Boolean get_IsTypeDecl()
FSharp.Compiler.EditorServices.NavigationItemKind: FSharp.Compiler.EditorServices.NavigationItemKind ExnDecl
FSharp.Compiler.EditorServices.NavigationItemKind: FSharp.Compiler.EditorServices.NavigationItemKind FieldDecl
FSharp.Compiler.EditorServices.NavigationItemKind: FSharp.Compiler.EditorServices.NavigationItemKind MethodDecl
FSharp.Compiler.EditorServices.NavigationItemKind: FSharp.Compiler.EditorServices.NavigationItemKind ModuleDecl
FSharp.Compiler.EditorServices.NavigationItemKind: FSharp.Compiler.EditorServices.NavigationItemKind ModuleFile
FSharp.Compiler.EditorServices.NavigationItemKind: FSharp.Compiler.EditorServices.NavigationItemKind Namespace
FSharp.Compiler.EditorServices.NavigationItemKind: FSharp.Compiler.EditorServices.NavigationItemKind OtherDecl
FSharp.Compiler.EditorServices.NavigationItemKind: FSharp.Compiler.EditorServices.NavigationItemKind PropertyDecl
FSharp.Compiler.EditorServices.NavigationItemKind: FSharp.Compiler.EditorServices.NavigationItemKind TypeDecl
FSharp.Compiler.EditorServices.NavigationItemKind: FSharp.Compiler.EditorServices.NavigationItemKind get_ExnDecl()
FSharp.Compiler.EditorServices.NavigationItemKind: FSharp.Compiler.EditorServices.NavigationItemKind get_FieldDecl()
FSharp.Compiler.EditorServices.NavigationItemKind: FSharp.Compiler.EditorServices.NavigationItemKind get_MethodDecl()
FSharp.Compiler.EditorServices.NavigationItemKind: FSharp.Compiler.EditorServices.NavigationItemKind get_ModuleDecl()
FSharp.Compiler.EditorServices.NavigationItemKind: FSharp.Compiler.EditorServices.NavigationItemKind get_ModuleFileDecl()
FSharp.Compiler.EditorServices.NavigationItemKind: FSharp.Compiler.EditorServices.NavigationItemKind get_NamespaceDecl()
FSharp.Compiler.EditorServices.NavigationItemKind: FSharp.Compiler.EditorServices.NavigationItemKind get_OtherDecl()
FSharp.Compiler.EditorServices.NavigationItemKind: FSharp.Compiler.EditorServices.NavigationItemKind get_PropertyDecl()
FSharp.Compiler.EditorServices.NavigationItemKind: FSharp.Compiler.EditorServices.NavigationItemKind get_TypeDecl()
FSharp.Compiler.EditorServices.NavigationItemKind: FSharp.Compiler.EditorServices.NavigationItemKind+Tags
FSharp.Compiler.EditorServices.NavigationItemKind: Int32 CompareTo(FSharp.Compiler.EditorServices.NavigationItemKind)
FSharp.Compiler.EditorServices.NavigationItemKind: Int32 CompareTo(System.Object)
FSharp.Compiler.EditorServices.NavigationItemKind: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.EditorServices.NavigationItemKind: Int32 GetHashCode()
FSharp.Compiler.EditorServices.NavigationItemKind: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.NavigationItemKind: Int32 Tag
FSharp.Compiler.EditorServices.NavigationItemKind: Int32 get_Tag()
FSharp.Compiler.EditorServices.NavigationItemKind: System.String ToString()
FSharp.Compiler.EditorServices.NavigationItems
FSharp.Compiler.EditorServices.NavigationItems: FSharp.Compiler.EditorServices.NavigationTopLevelDeclaration[] Declarations
FSharp.Compiler.EditorServices.NavigationItems: FSharp.Compiler.EditorServices.NavigationTopLevelDeclaration[] get_Declarations()
FSharp.Compiler.EditorServices.NavigationTopLevelDeclaration
FSharp.Compiler.EditorServices.NavigationTopLevelDeclaration: FSharp.Compiler.EditorServices.NavigationItem Declaration
FSharp.Compiler.EditorServices.NavigationTopLevelDeclaration: FSharp.Compiler.EditorServices.NavigationItem get_Declaration()
FSharp.Compiler.EditorServices.NavigationTopLevelDeclaration: FSharp.Compiler.EditorServices.NavigationItem[] Nested
FSharp.Compiler.EditorServices.NavigationTopLevelDeclaration: FSharp.Compiler.EditorServices.NavigationItem[] get_Nested()
FSharp.Compiler.EditorServices.NavigationTopLevelDeclaration: System.String ToString()
FSharp.Compiler.EditorServices.NavigationTopLevelDeclaration: Void .ctor(FSharp.Compiler.EditorServices.NavigationItem, FSharp.Compiler.EditorServices.NavigationItem[])
FSharp.Compiler.EditorServices.ParameterLocations
FSharp.Compiler.EditorServices.ParameterLocations: Boolean IsThereACloseParen
FSharp.Compiler.EditorServices.ParameterLocations: Boolean get_IsThereACloseParen()
FSharp.Compiler.EditorServices.ParameterLocations: FSharp.Compiler.Text.Position LongIdEndLocation
FSharp.Compiler.EditorServices.ParameterLocations: FSharp.Compiler.Text.Position LongIdStartLocation
FSharp.Compiler.EditorServices.ParameterLocations: FSharp.Compiler.Text.Position OpenParenLocation
FSharp.Compiler.EditorServices.ParameterLocations: FSharp.Compiler.Text.Position get_LongIdEndLocation()
FSharp.Compiler.EditorServices.ParameterLocations: FSharp.Compiler.Text.Position get_LongIdStartLocation()
FSharp.Compiler.EditorServices.ParameterLocations: FSharp.Compiler.Text.Position get_OpenParenLocation()
FSharp.Compiler.EditorServices.ParameterLocations: FSharp.Compiler.Text.Position[] TupleEndLocations
FSharp.Compiler.EditorServices.ParameterLocations: FSharp.Compiler.Text.Position[] get_TupleEndLocations()
FSharp.Compiler.EditorServices.ParameterLocations: Microsoft.FSharp.Collections.FSharpList`1[System.String] LongId
FSharp.Compiler.EditorServices.ParameterLocations: Microsoft.FSharp.Collections.FSharpList`1[System.String] get_LongId()
FSharp.Compiler.EditorServices.ParameterLocations: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.EditorServices.ParameterLocations] Find(FSharp.Compiler.Text.Position, FSharp.Compiler.Syntax.ParsedInput)
FSharp.Compiler.EditorServices.ParameterLocations: Microsoft.FSharp.Core.FSharpOption`1[System.String][] NamedParamNames
FSharp.Compiler.EditorServices.ParameterLocations: Microsoft.FSharp.Core.FSharpOption`1[System.String][] get_NamedParamNames()
FSharp.Compiler.EditorServices.OpenStatementInsertionPoint
FSharp.Compiler.EditorServices.OpenStatementInsertionPoint+Tags: Int32 Nearest
FSharp.Compiler.EditorServices.OpenStatementInsertionPoint+Tags: Int32 TopLevel
FSharp.Compiler.EditorServices.OpenStatementInsertionPoint: Boolean Equals(FSharp.Compiler.EditorServices.OpenStatementInsertionPoint)
FSharp.Compiler.EditorServices.OpenStatementInsertionPoint: Boolean Equals(System.Object)
FSharp.Compiler.EditorServices.OpenStatementInsertionPoint: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.OpenStatementInsertionPoint: Boolean IsNearest
FSharp.Compiler.EditorServices.OpenStatementInsertionPoint: Boolean IsTopLevel
FSharp.Compiler.EditorServices.OpenStatementInsertionPoint: Boolean get_IsNearest()
FSharp.Compiler.EditorServices.OpenStatementInsertionPoint: Boolean get_IsTopLevel()
FSharp.Compiler.EditorServices.OpenStatementInsertionPoint: FSharp.Compiler.EditorServices.OpenStatementInsertionPoint Nearest
FSharp.Compiler.EditorServices.OpenStatementInsertionPoint: FSharp.Compiler.EditorServices.OpenStatementInsertionPoint TopLevel
FSharp.Compiler.EditorServices.OpenStatementInsertionPoint: FSharp.Compiler.EditorServices.OpenStatementInsertionPoint get_Nearest()
FSharp.Compiler.EditorServices.OpenStatementInsertionPoint: FSharp.Compiler.EditorServices.OpenStatementInsertionPoint get_TopLevel()
FSharp.Compiler.EditorServices.OpenStatementInsertionPoint: FSharp.Compiler.EditorServices.OpenStatementInsertionPoint+Tags
FSharp.Compiler.EditorServices.OpenStatementInsertionPoint: Int32 CompareTo(FSharp.Compiler.EditorServices.OpenStatementInsertionPoint)
FSharp.Compiler.EditorServices.OpenStatementInsertionPoint: Int32 CompareTo(System.Object)
FSharp.Compiler.EditorServices.OpenStatementInsertionPoint: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.EditorServices.OpenStatementInsertionPoint: Int32 GetHashCode()
FSharp.Compiler.EditorServices.OpenStatementInsertionPoint: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.OpenStatementInsertionPoint: Int32 Tag
FSharp.Compiler.EditorServices.OpenStatementInsertionPoint: Int32 get_Tag()
FSharp.Compiler.EditorServices.OpenStatementInsertionPoint: System.String ToString()
FSharp.Compiler.EditorServices.InsertionContextEntity
FSharp.Compiler.EditorServices.InsertionContextEntity: Boolean Equals(FSharp.Compiler.EditorServices.InsertionContextEntity)
FSharp.Compiler.EditorServices.InsertionContextEntity: Boolean Equals(System.Object)
FSharp.Compiler.EditorServices.InsertionContextEntity: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.InsertionContextEntity: Int32 CompareTo(FSharp.Compiler.EditorServices.InsertionContextEntity)
FSharp.Compiler.EditorServices.InsertionContextEntity: Int32 CompareTo(System.Object)
FSharp.Compiler.EditorServices.InsertionContextEntity: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.EditorServices.InsertionContextEntity: Int32 GetHashCode()
FSharp.Compiler.EditorServices.InsertionContextEntity: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.InsertionContextEntity: Microsoft.FSharp.Core.FSharpOption`1[System.String] Namespace
FSharp.Compiler.EditorServices.InsertionContextEntity: Microsoft.FSharp.Core.FSharpOption`1[System.String] get_Namespace()
FSharp.Compiler.EditorServices.InsertionContextEntity: System.String FullDisplayName
FSharp.Compiler.EditorServices.InsertionContextEntity: System.String FullRelativeName
FSharp.Compiler.EditorServices.InsertionContextEntity: System.String LastIdent
FSharp.Compiler.EditorServices.InsertionContextEntity: System.String Qualifier
FSharp.Compiler.EditorServices.InsertionContextEntity: System.String ToString()
FSharp.Compiler.EditorServices.InsertionContextEntity: System.String get_FullDisplayName()
FSharp.Compiler.EditorServices.InsertionContextEntity: System.String get_FullRelativeName()
FSharp.Compiler.EditorServices.InsertionContextEntity: System.String get_LastIdent()
FSharp.Compiler.EditorServices.InsertionContextEntity: System.String get_Qualifier()
FSharp.Compiler.EditorServices.InsertionContextEntity: Void .ctor(System.String, System.String, Microsoft.FSharp.Core.FSharpOption`1[System.String], System.String, System.String)
FSharp.Compiler.EditorServices.RecordContext
FSharp.Compiler.EditorServices.RecordContext+Constructor: System.String get_typeName()
FSharp.Compiler.EditorServices.RecordContext+Constructor: System.String typeName
FSharp.Compiler.EditorServices.RecordContext+CopyOnUpdate: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.EditorServices.RecordContext+CopyOnUpdate: FSharp.Compiler.Text.Range range
FSharp.Compiler.EditorServices.RecordContext+CopyOnUpdate: System.Tuple`2[Microsoft.FSharp.Collections.FSharpList`1[System.String],Microsoft.FSharp.Core.FSharpOption`1[System.String]] get_path()
FSharp.Compiler.EditorServices.RecordContext+CopyOnUpdate: System.Tuple`2[Microsoft.FSharp.Collections.FSharpList`1[System.String],Microsoft.FSharp.Core.FSharpOption`1[System.String]] path
FSharp.Compiler.EditorServices.RecordContext+New: System.Tuple`2[Microsoft.FSharp.Collections.FSharpList`1[System.String],Microsoft.FSharp.Core.FSharpOption`1[System.String]] get_path()
FSharp.Compiler.EditorServices.RecordContext+New: System.Tuple`2[Microsoft.FSharp.Collections.FSharpList`1[System.String],Microsoft.FSharp.Core.FSharpOption`1[System.String]] path
FSharp.Compiler.EditorServices.RecordContext+Tags: Int32 Constructor
FSharp.Compiler.EditorServices.RecordContext+Tags: Int32 CopyOnUpdate
FSharp.Compiler.EditorServices.RecordContext+Tags: Int32 New
FSharp.Compiler.EditorServices.RecordContext: Boolean Equals(FSharp.Compiler.EditorServices.RecordContext)
FSharp.Compiler.EditorServices.RecordContext: Boolean Equals(System.Object)
FSharp.Compiler.EditorServices.RecordContext: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.RecordContext: Boolean IsConstructor
FSharp.Compiler.EditorServices.RecordContext: Boolean IsCopyOnUpdate
FSharp.Compiler.EditorServices.RecordContext: Boolean IsNew
FSharp.Compiler.EditorServices.RecordContext: Boolean get_IsConstructor()
FSharp.Compiler.EditorServices.RecordContext: Boolean get_IsCopyOnUpdate()
FSharp.Compiler.EditorServices.RecordContext: Boolean get_IsNew()
FSharp.Compiler.EditorServices.RecordContext: FSharp.Compiler.EditorServices.RecordContext NewConstructor(System.String)
FSharp.Compiler.EditorServices.RecordContext: FSharp.Compiler.EditorServices.RecordContext NewCopyOnUpdate(FSharp.Compiler.Text.Range, System.Tuple`2[Microsoft.FSharp.Collections.FSharpList`1[System.String],Microsoft.FSharp.Core.FSharpOption`1[System.String]])
FSharp.Compiler.EditorServices.RecordContext: FSharp.Compiler.EditorServices.RecordContext NewNew(System.Tuple`2[Microsoft.FSharp.Collections.FSharpList`1[System.String],Microsoft.FSharp.Core.FSharpOption`1[System.String]])
FSharp.Compiler.EditorServices.RecordContext: FSharp.Compiler.EditorServices.RecordContext+Constructor
FSharp.Compiler.EditorServices.RecordContext: FSharp.Compiler.EditorServices.RecordContext+CopyOnUpdate
FSharp.Compiler.EditorServices.RecordContext: FSharp.Compiler.EditorServices.RecordContext+New
FSharp.Compiler.EditorServices.RecordContext: FSharp.Compiler.EditorServices.RecordContext+Tags
FSharp.Compiler.EditorServices.RecordContext: Int32 GetHashCode()
FSharp.Compiler.EditorServices.RecordContext: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.RecordContext: Int32 Tag
FSharp.Compiler.EditorServices.RecordContext: Int32 get_Tag()
FSharp.Compiler.EditorServices.RecordContext: System.String ToString()
FSharp.Compiler.EditorServices.ScopeKind
FSharp.Compiler.EditorServices.ScopeKind+Tags: Int32 HashDirective
FSharp.Compiler.EditorServices.ScopeKind+Tags: Int32 Namespace
FSharp.Compiler.EditorServices.ScopeKind+Tags: Int32 NestedModule
FSharp.Compiler.EditorServices.ScopeKind+Tags: Int32 OpenDeclaration
FSharp.Compiler.EditorServices.ScopeKind+Tags: Int32 TopModule
FSharp.Compiler.EditorServices.ScopeKind: Boolean Equals(FSharp.Compiler.EditorServices.ScopeKind)
FSharp.Compiler.EditorServices.ScopeKind: Boolean Equals(System.Object)
FSharp.Compiler.EditorServices.ScopeKind: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.ScopeKind: Boolean IsHashDirective
FSharp.Compiler.EditorServices.ScopeKind: Boolean IsNamespace
FSharp.Compiler.EditorServices.ScopeKind: Boolean IsNestedModule
FSharp.Compiler.EditorServices.ScopeKind: Boolean IsOpenDeclaration
FSharp.Compiler.EditorServices.ScopeKind: Boolean IsTopModule
FSharp.Compiler.EditorServices.ScopeKind: Boolean get_IsHashDirective()
FSharp.Compiler.EditorServices.ScopeKind: Boolean get_IsNamespace()
FSharp.Compiler.EditorServices.ScopeKind: Boolean get_IsNestedModule()
FSharp.Compiler.EditorServices.ScopeKind: Boolean get_IsOpenDeclaration()
FSharp.Compiler.EditorServices.ScopeKind: Boolean get_IsTopModule()
FSharp.Compiler.EditorServices.ScopeKind: FSharp.Compiler.EditorServices.ScopeKind HashDirective
FSharp.Compiler.EditorServices.ScopeKind: FSharp.Compiler.EditorServices.ScopeKind Namespace
FSharp.Compiler.EditorServices.ScopeKind: FSharp.Compiler.EditorServices.ScopeKind NestedModule
FSharp.Compiler.EditorServices.ScopeKind: FSharp.Compiler.EditorServices.ScopeKind OpenDeclaration
FSharp.Compiler.EditorServices.ScopeKind: FSharp.Compiler.EditorServices.ScopeKind TopModule
FSharp.Compiler.EditorServices.ScopeKind: FSharp.Compiler.EditorServices.ScopeKind get_HashDirective()
FSharp.Compiler.EditorServices.ScopeKind: FSharp.Compiler.EditorServices.ScopeKind get_Namespace()
FSharp.Compiler.EditorServices.ScopeKind: FSharp.Compiler.EditorServices.ScopeKind get_NestedModule()
FSharp.Compiler.EditorServices.ScopeKind: FSharp.Compiler.EditorServices.ScopeKind get_OpenDeclaration()
FSharp.Compiler.EditorServices.ScopeKind: FSharp.Compiler.EditorServices.ScopeKind get_TopModule()
FSharp.Compiler.EditorServices.ScopeKind: FSharp.Compiler.EditorServices.ScopeKind+Tags
FSharp.Compiler.EditorServices.ScopeKind: Int32 CompareTo(FSharp.Compiler.EditorServices.ScopeKind)
FSharp.Compiler.EditorServices.ScopeKind: Int32 CompareTo(System.Object)
FSharp.Compiler.EditorServices.ScopeKind: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.EditorServices.ScopeKind: Int32 GetHashCode()
FSharp.Compiler.EditorServices.ScopeKind: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.ScopeKind: Int32 Tag
FSharp.Compiler.EditorServices.ScopeKind: Int32 get_Tag()
FSharp.Compiler.EditorServices.ScopeKind: System.String ToString()
FSharp.Compiler.EditorServices.FSharpSourceFile
FSharp.Compiler.EditorServices.FSharpSourceFile: Boolean IsCompilable(System.String)
FSharp.Compiler.EditorServices.FSharpSourceFile: Boolean MustBeSingleFileProject(System.String)
FSharp.Compiler.EditorServices.FSharpSourceTokenizer
FSharp.Compiler.EditorServices.FSharpSourceTokenizer: FSharp.Compiler.EditorServices.FSharpLineTokenizer CreateBufferTokenizer(Microsoft.FSharp.Core.FSharpFunc`2[System.Tuple`3[System.Char[],System.Int32,System.Int32],System.Int32])
FSharp.Compiler.EditorServices.FSharpSourceTokenizer: FSharp.Compiler.EditorServices.FSharpLineTokenizer CreateLineTokenizer(System.String)
FSharp.Compiler.EditorServices.FSharpSourceTokenizer: Void .ctor(Microsoft.FSharp.Collections.FSharpList`1[System.String], Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.EditorServices.FSharpToken
FSharp.Compiler.EditorServices.FSharpToken: Boolean IsCommentTrivia
FSharp.Compiler.EditorServices.FSharpToken: Boolean IsIdentifier
FSharp.Compiler.EditorServices.FSharpToken: Boolean IsKeyword
FSharp.Compiler.EditorServices.FSharpToken: Boolean IsNumericLiteral
FSharp.Compiler.EditorServices.FSharpToken: Boolean IsStringLiteral
FSharp.Compiler.EditorServices.FSharpToken: Boolean get_IsCommentTrivia()
FSharp.Compiler.EditorServices.FSharpToken: Boolean get_IsIdentifier()
FSharp.Compiler.EditorServices.FSharpToken: Boolean get_IsKeyword()
FSharp.Compiler.EditorServices.FSharpToken: Boolean get_IsNumericLiteral()
FSharp.Compiler.EditorServices.FSharpToken: Boolean get_IsStringLiteral()
FSharp.Compiler.EditorServices.FSharpToken: FSharp.Compiler.EditorServices.FSharpTokenKind Kind
FSharp.Compiler.EditorServices.FSharpToken: FSharp.Compiler.EditorServices.FSharpTokenKind get_Kind()
FSharp.Compiler.EditorServices.FSharpToken: FSharp.Compiler.Text.Range Range
FSharp.Compiler.EditorServices.FSharpToken: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.EditorServices.FSharpTokenCharKind
FSharp.Compiler.EditorServices.FSharpTokenCharKind: FSharp.Compiler.EditorServices.FSharpTokenCharKind Comment
FSharp.Compiler.EditorServices.FSharpTokenCharKind: FSharp.Compiler.EditorServices.FSharpTokenCharKind Default
FSharp.Compiler.EditorServices.FSharpTokenCharKind: FSharp.Compiler.EditorServices.FSharpTokenCharKind Delimiter
FSharp.Compiler.EditorServices.FSharpTokenCharKind: FSharp.Compiler.EditorServices.FSharpTokenCharKind Identifier
FSharp.Compiler.EditorServices.FSharpTokenCharKind: FSharp.Compiler.EditorServices.FSharpTokenCharKind Keyword
FSharp.Compiler.EditorServices.FSharpTokenCharKind: FSharp.Compiler.EditorServices.FSharpTokenCharKind LineComment
FSharp.Compiler.EditorServices.FSharpTokenCharKind: FSharp.Compiler.EditorServices.FSharpTokenCharKind Literal
FSharp.Compiler.EditorServices.FSharpTokenCharKind: FSharp.Compiler.EditorServices.FSharpTokenCharKind Operator
FSharp.Compiler.EditorServices.FSharpTokenCharKind: FSharp.Compiler.EditorServices.FSharpTokenCharKind String
FSharp.Compiler.EditorServices.FSharpTokenCharKind: FSharp.Compiler.EditorServices.FSharpTokenCharKind Text
FSharp.Compiler.EditorServices.FSharpTokenCharKind: FSharp.Compiler.EditorServices.FSharpTokenCharKind WhiteSpace
FSharp.Compiler.EditorServices.FSharpTokenCharKind: Int32 value__
FSharp.Compiler.EditorServices.FSharpTokenColorKind
FSharp.Compiler.EditorServices.FSharpTokenColorKind: FSharp.Compiler.EditorServices.FSharpTokenColorKind Comment
FSharp.Compiler.EditorServices.FSharpTokenColorKind: FSharp.Compiler.EditorServices.FSharpTokenColorKind Default
FSharp.Compiler.EditorServices.FSharpTokenColorKind: FSharp.Compiler.EditorServices.FSharpTokenColorKind Identifier
FSharp.Compiler.EditorServices.FSharpTokenColorKind: FSharp.Compiler.EditorServices.FSharpTokenColorKind InactiveCode
FSharp.Compiler.EditorServices.FSharpTokenColorKind: FSharp.Compiler.EditorServices.FSharpTokenColorKind Keyword
FSharp.Compiler.EditorServices.FSharpTokenColorKind: FSharp.Compiler.EditorServices.FSharpTokenColorKind Number
FSharp.Compiler.EditorServices.FSharpTokenColorKind: FSharp.Compiler.EditorServices.FSharpTokenColorKind Operator
FSharp.Compiler.EditorServices.FSharpTokenColorKind: FSharp.Compiler.EditorServices.FSharpTokenColorKind PreprocessorKeyword
FSharp.Compiler.EditorServices.FSharpTokenColorKind: FSharp.Compiler.EditorServices.FSharpTokenColorKind Punctuation
FSharp.Compiler.EditorServices.FSharpTokenColorKind: FSharp.Compiler.EditorServices.FSharpTokenColorKind String
FSharp.Compiler.EditorServices.FSharpTokenColorKind: FSharp.Compiler.EditorServices.FSharpTokenColorKind Text
FSharp.Compiler.EditorServices.FSharpTokenColorKind: FSharp.Compiler.EditorServices.FSharpTokenColorKind UpperIdentifier
FSharp.Compiler.EditorServices.FSharpTokenColorKind: Int32 value__
FSharp.Compiler.EditorServices.FSharpTokenInfo
FSharp.Compiler.EditorServices.FSharpTokenInfo: Boolean Equals(FSharp.Compiler.EditorServices.FSharpTokenInfo)
FSharp.Compiler.EditorServices.FSharpTokenInfo: Boolean Equals(System.Object)
FSharp.Compiler.EditorServices.FSharpTokenInfo: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.FSharpTokenInfo: FSharp.Compiler.EditorServices.FSharpTokenCharKind CharClass
FSharp.Compiler.EditorServices.FSharpTokenInfo: FSharp.Compiler.EditorServices.FSharpTokenCharKind get_CharClass()
FSharp.Compiler.EditorServices.FSharpTokenInfo: FSharp.Compiler.EditorServices.FSharpTokenColorKind ColorClass
FSharp.Compiler.EditorServices.FSharpTokenInfo: FSharp.Compiler.EditorServices.FSharpTokenColorKind get_ColorClass()
FSharp.Compiler.EditorServices.FSharpTokenInfo: FSharp.Compiler.EditorServices.FSharpTokenTriggerClass FSharpTokenTriggerClass
FSharp.Compiler.EditorServices.FSharpTokenInfo: FSharp.Compiler.EditorServices.FSharpTokenTriggerClass get_FSharpTokenTriggerClass()
FSharp.Compiler.EditorServices.FSharpTokenInfo: Int32 CompareTo(FSharp.Compiler.EditorServices.FSharpTokenInfo)
FSharp.Compiler.EditorServices.FSharpTokenInfo: Int32 CompareTo(System.Object)
FSharp.Compiler.EditorServices.FSharpTokenInfo: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.EditorServices.FSharpTokenInfo: Int32 FullMatchedLength
FSharp.Compiler.EditorServices.FSharpTokenInfo: Int32 GetHashCode()
FSharp.Compiler.EditorServices.FSharpTokenInfo: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.FSharpTokenInfo: Int32 LeftColumn
FSharp.Compiler.EditorServices.FSharpTokenInfo: Int32 RightColumn
FSharp.Compiler.EditorServices.FSharpTokenInfo: Int32 Tag
FSharp.Compiler.EditorServices.FSharpTokenInfo: Int32 get_FullMatchedLength()
FSharp.Compiler.EditorServices.FSharpTokenInfo: Int32 get_LeftColumn()
FSharp.Compiler.EditorServices.FSharpTokenInfo: Int32 get_RightColumn()
FSharp.Compiler.EditorServices.FSharpTokenInfo: Int32 get_Tag()
FSharp.Compiler.EditorServices.FSharpTokenInfo: System.String ToString()
FSharp.Compiler.EditorServices.FSharpTokenInfo: System.String TokenName
FSharp.Compiler.EditorServices.FSharpTokenInfo: System.String get_TokenName()
FSharp.Compiler.EditorServices.FSharpTokenInfo: Void .ctor(Int32, Int32, FSharp.Compiler.EditorServices.FSharpTokenColorKind, FSharp.Compiler.EditorServices.FSharpTokenCharKind, FSharp.Compiler.EditorServices.FSharpTokenTriggerClass, Int32, System.String, Int32)
FSharp.Compiler.EditorServices.FSharpTokenKind
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Abstract
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 AdjacentPrefixOperator
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Ampersand
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 AmpersandAmpersand
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 And
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 As
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Asr
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Assert
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Bar
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 BarBar
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 BarRightBrace
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 BarRightBracket
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Base
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Begin
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 BigNumber
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Binder
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 ByteArray
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Char
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Class
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Colon
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 ColonColon
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 ColonEquals
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 ColonGreater
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 ColonQuestionMark
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 ColonQuestionMarkGreater
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Comma
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 CommentTrivia
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Const
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Constraint
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Constructor
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Decimal
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Default
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Delegate
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Do
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 DoBang
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Dollar
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Done
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Dot
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 DotDot
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 DotDotHat
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 DownTo
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Downcast
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Elif
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Else
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 End
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Equals
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Exception
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Extern
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 False
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Finally
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Fixed
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 For
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Fun
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Function
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 FunkyOperatorName
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Global
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Greater
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 GreaterBarRightBracket
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 GreaterRightBracket
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Hash
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 HashElse
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 HashEndIf
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 HashIf
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 HashLight
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 HashLine
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 HighPrecedenceBracketApp
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 HighPrecedenceParenthesisApp
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 HighPrecedenceTypeApp
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Identifier
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Ieee32
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Ieee64
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 If
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 In
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 InactiveCode
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 InfixAmpersandOperator
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 InfixAsr
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 InfixAtHatOperator
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 InfixBarOperator
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 InfixCompareOperator
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 InfixLand
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 InfixLor
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 InfixLsl
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 InfixLsr
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 InfixLxor
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 InfixMod
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 InfixStarDivideModuloOperator
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 InfixStarStarOperator
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Inherit
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Inline
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Instance
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Int16
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Int32
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Int32DotDot
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Int64
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Int8
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Interface
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Internal
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 JoinIn
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 KeywordString
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Lazy
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 LeftArrow
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 LeftBrace
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 LeftBraceBar
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 LeftBracket
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 LeftBracketBar
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 LeftBracketLess
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 LeftParenthesis
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 LeftParenthesisStarRightParenthesis
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 LeftQuote
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Less
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Let
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 LineCommentTrivia
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Match
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 MatchBang
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Member
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Minus
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Module
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Mutable
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Namespace
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 NativeInt
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 New
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 None
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Null
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Of
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 OffsideAssert
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 OffsideBinder
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 OffsideBlockBegin
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 OffsideBlockEnd
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 OffsideBlockSep
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 OffsideDeclEnd
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 OffsideDo
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 OffsideDoBang
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 OffsideElse
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 OffsideEnd
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 OffsideFun
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 OffsideFunction
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 OffsideInterfaceMember
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 OffsideLazy
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 OffsideLet
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 OffsideReset
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 OffsideRightBlockEnd
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 OffsideThen
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 OffsideWith
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Open
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Or
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Override
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 PercentOperator
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 PlusMinusOperator
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 PrefixOperator
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Private
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Public
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 QuestionMark
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 QuestionMarkQuestionMark
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Quote
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Rec
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Reserved
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 RightArrow
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 RightBrace
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 RightBracket
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 RightParenthesis
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 RightQuote
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 RightQuoteDot
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Semicolon
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 SemicolonSemicolon
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Sig
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Star
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Static
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 String
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 StringText
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Struct
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Then
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 To
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 True
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Try
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Type
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 UInt16
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 UInt32
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 UInt64
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 UInt8
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 UNativeInt
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Underscore
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Upcast
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Val
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Void
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 When
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 While
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 WhitespaceTrivia
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 With
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 Yield
FSharp.Compiler.EditorServices.FSharpTokenKind+Tags: Int32 YieldBang
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean Equals(FSharp.Compiler.EditorServices.FSharpTokenKind)
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean Equals(System.Object)
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsAbstract
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsAdjacentPrefixOperator
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsAmpersand
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsAmpersandAmpersand
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsAnd
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsAs
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsAsr
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsAssert
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsBar
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsBarBar
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsBarRightBrace
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsBarRightBracket
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsBase
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsBegin
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsBigNumber
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsBinder
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsByteArray
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsChar
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsClass
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsColon
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsColonColon
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsColonEquals
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsColonGreater
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsColonQuestionMark
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsColonQuestionMarkGreater
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsComma
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsCommentTrivia
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsConst
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsConstraint
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsConstructor
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsDecimal
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsDefault
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsDelegate
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsDo
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsDoBang
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsDollar
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsDone
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsDot
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsDotDot
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsDotDotHat
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsDownTo
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsDowncast
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsElif
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsElse
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsEnd
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsEquals
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsException
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsExtern
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsFalse
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsFinally
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsFixed
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsFor
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsFun
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsFunction
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsFunkyOperatorName
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsGlobal
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsGreater
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsGreaterBarRightBracket
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsGreaterRightBracket
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsHash
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsHashElse
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsHashEndIf
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsHashIf
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsHashLight
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsHashLine
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsHighPrecedenceBracketApp
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsHighPrecedenceParenthesisApp
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsHighPrecedenceTypeApp
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsIdentifier
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsIeee32
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsIeee64
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsIf
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsIn
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsInactiveCode
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsInfixAmpersandOperator
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsInfixAsr
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsInfixAtHatOperator
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsInfixBarOperator
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsInfixCompareOperator
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsInfixLand
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsInfixLor
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsInfixLsl
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsInfixLsr
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsInfixLxor
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsInfixMod
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsInfixStarDivideModuloOperator
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsInfixStarStarOperator
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsInherit
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsInline
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsInstance
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsInt16
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsInt32
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsInt32DotDot
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsInt64
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsInt8
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsInterface
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsInternal
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsJoinIn
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsKeywordString
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsLazy
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsLeftArrow
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsLeftBrace
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsLeftBraceBar
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsLeftBracket
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsLeftBracketBar
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsLeftBracketLess
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsLeftParenthesis
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsLeftParenthesisStarRightParenthesis
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsLeftQuote
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsLess
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsLet
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsLineCommentTrivia
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsMatch
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsMatchBang
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsMember
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsMinus
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsModule
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsMutable
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsNamespace
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsNativeInt
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsNew
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsNone
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsNull
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsOf
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsOffsideAssert
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsOffsideBinder
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsOffsideBlockBegin
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsOffsideBlockEnd
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsOffsideBlockSep
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsOffsideDeclEnd
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsOffsideDo
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsOffsideDoBang
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsOffsideElse
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsOffsideEnd
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsOffsideFun
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsOffsideFunction
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsOffsideInterfaceMember
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsOffsideLazy
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsOffsideLet
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsOffsideReset
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsOffsideRightBlockEnd
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsOffsideThen
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsOffsideWith
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsOpen
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsOr
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsOverride
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsPercentOperator
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsPlusMinusOperator
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsPrefixOperator
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsPrivate
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsPublic
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsQuestionMark
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsQuestionMarkQuestionMark
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsQuote
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsRec
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsReserved
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsRightArrow
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsRightBrace
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsRightBracket
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsRightParenthesis
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsRightQuote
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsRightQuoteDot
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsSemicolon
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsSemicolonSemicolon
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsSig
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsStar
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsStatic
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsString
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsStringText
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsStruct
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsThen
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsTo
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsTrue
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsTry
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsType
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsUInt16
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsUInt32
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsUInt64
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsUInt8
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsUNativeInt
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsUnderscore
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsUpcast
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsVal
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsVoid
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsWhen
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsWhile
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsWhitespaceTrivia
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsWith
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsYield
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean IsYieldBang
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsAbstract()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsAdjacentPrefixOperator()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsAmpersand()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsAmpersandAmpersand()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsAnd()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsAs()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsAsr()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsAssert()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsBar()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsBarBar()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsBarRightBrace()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsBarRightBracket()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsBase()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsBegin()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsBigNumber()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsBinder()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsByteArray()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsChar()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsClass()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsColon()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsColonColon()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsColonEquals()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsColonGreater()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsColonQuestionMark()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsColonQuestionMarkGreater()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsComma()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsCommentTrivia()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsConst()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsConstraint()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsConstructor()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsDecimal()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsDefault()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsDelegate()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsDo()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsDoBang()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsDollar()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsDone()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsDot()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsDotDot()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsDotDotHat()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsDownTo()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsDowncast()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsElif()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsElse()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsEnd()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsEquals()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsException()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsExtern()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsFalse()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsFinally()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsFixed()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsFor()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsFun()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsFunction()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsFunkyOperatorName()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsGlobal()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsGreater()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsGreaterBarRightBracket()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsGreaterRightBracket()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsHash()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsHashElse()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsHashEndIf()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsHashIf()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsHashLight()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsHashLine()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsHighPrecedenceBracketApp()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsHighPrecedenceParenthesisApp()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsHighPrecedenceTypeApp()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsIdentifier()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsIeee32()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsIeee64()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsIf()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsIn()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsInactiveCode()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsInfixAmpersandOperator()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsInfixAsr()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsInfixAtHatOperator()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsInfixBarOperator()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsInfixCompareOperator()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsInfixLand()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsInfixLor()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsInfixLsl()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsInfixLsr()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsInfixLxor()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsInfixMod()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsInfixStarDivideModuloOperator()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsInfixStarStarOperator()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsInherit()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsInline()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsInstance()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsInt16()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsInt32()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsInt32DotDot()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsInt64()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsInt8()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsInterface()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsInternal()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsJoinIn()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsKeywordString()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsLazy()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsLeftArrow()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsLeftBrace()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsLeftBraceBar()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsLeftBracket()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsLeftBracketBar()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsLeftBracketLess()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsLeftParenthesis()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsLeftParenthesisStarRightParenthesis()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsLeftQuote()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsLess()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsLet()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsLineCommentTrivia()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsMatch()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsMatchBang()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsMember()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsMinus()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsModule()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsMutable()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsNamespace()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsNativeInt()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsNew()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsNone()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsNull()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsOf()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsOffsideAssert()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsOffsideBinder()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsOffsideBlockBegin()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsOffsideBlockEnd()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsOffsideBlockSep()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsOffsideDeclEnd()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsOffsideDo()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsOffsideDoBang()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsOffsideElse()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsOffsideEnd()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsOffsideFun()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsOffsideFunction()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsOffsideInterfaceMember()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsOffsideLazy()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsOffsideLet()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsOffsideReset()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsOffsideRightBlockEnd()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsOffsideThen()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsOffsideWith()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsOpen()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsOr()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsOverride()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsPercentOperator()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsPlusMinusOperator()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsPrefixOperator()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsPrivate()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsPublic()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsQuestionMark()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsQuestionMarkQuestionMark()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsQuote()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsRec()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsReserved()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsRightArrow()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsRightBrace()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsRightBracket()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsRightParenthesis()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsRightQuote()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsRightQuoteDot()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsSemicolon()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsSemicolonSemicolon()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsSig()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsStar()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsStatic()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsString()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsStringText()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsStruct()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsThen()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsTo()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsTrue()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsTry()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsType()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsUInt16()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsUInt32()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsUInt64()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsUInt8()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsUNativeInt()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsUnderscore()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsUpcast()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsVal()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsVoid()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsWhen()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsWhile()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsWhitespaceTrivia()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsWith()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsYield()
FSharp.Compiler.EditorServices.FSharpTokenKind: Boolean get_IsYieldBang()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Abstract
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind AdjacentPrefixOperator
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Ampersand
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind AmpersandAmpersand
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind And
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind As
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Asr
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Assert
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Bar
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind BarBar
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind BarRightBrace
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind BarRightBracket
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Base
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Begin
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind BigNumber
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Binder
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind ByteArray
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Char
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Class
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Colon
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind ColonColon
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind ColonEquals
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind ColonGreater
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind ColonQuestionMark
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind ColonQuestionMarkGreater
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Comma
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind CommentTrivia
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Const
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Constraint
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Constructor
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Decimal
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Default
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Delegate
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Do
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind DoBang
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Dollar
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Done
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Dot
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind DotDot
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind DotDotHat
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind DownTo
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Downcast
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Elif
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Else
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind End
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Equals
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Exception
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Extern
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind False
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Finally
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Fixed
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind For
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Fun
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Function
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind FunkyOperatorName
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Global
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Greater
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind GreaterBarRightBracket
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind GreaterRightBracket
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Hash
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind HashElse
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind HashEndIf
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind HashIf
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind HashLight
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind HashLine
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind HighPrecedenceBracketApp
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind HighPrecedenceParenthesisApp
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind HighPrecedenceTypeApp
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Identifier
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Ieee32
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Ieee64
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind If
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind In
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind InactiveCode
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind InfixAmpersandOperator
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind InfixAsr
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind InfixAtHatOperator
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind InfixBarOperator
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind InfixCompareOperator
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind InfixLand
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind InfixLor
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind InfixLsl
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind InfixLsr
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind InfixLxor
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind InfixMod
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind InfixStarDivideModuloOperator
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind InfixStarStarOperator
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Inherit
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Inline
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Instance
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Int16
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Int32
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Int32DotDot
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Int64
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Int8
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Interface
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Internal
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind JoinIn
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind KeywordString
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Lazy
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind LeftArrow
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind LeftBrace
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind LeftBraceBar
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind LeftBracket
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind LeftBracketBar
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind LeftBracketLess
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind LeftParenthesis
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind LeftParenthesisStarRightParenthesis
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind LeftQuote
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Less
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Let
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind LineCommentTrivia
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Match
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind MatchBang
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Member
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Minus
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Module
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Mutable
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Namespace
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind NativeInt
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind New
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind None
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Null
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Of
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind OffsideAssert
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind OffsideBinder
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind OffsideBlockBegin
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind OffsideBlockEnd
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind OffsideBlockSep
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind OffsideDeclEnd
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind OffsideDo
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind OffsideDoBang
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind OffsideElse
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind OffsideEnd
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind OffsideFun
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind OffsideFunction
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind OffsideInterfaceMember
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind OffsideLazy
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind OffsideLet
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind OffsideReset
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind OffsideRightBlockEnd
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind OffsideThen
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind OffsideWith
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Open
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Or
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Override
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind PercentOperator
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind PlusMinusOperator
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind PrefixOperator
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Private
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Public
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind QuestionMark
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind QuestionMarkQuestionMark
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Quote
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Rec
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Reserved
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind RightArrow
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind RightBrace
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind RightBracket
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind RightParenthesis
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind RightQuote
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind RightQuoteDot
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Semicolon
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind SemicolonSemicolon
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Sig
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Star
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Static
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind String
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind StringText
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Struct
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Then
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind To
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind True
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Try
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Type
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind UInt16
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind UInt32
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind UInt64
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind UInt8
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind UNativeInt
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Underscore
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Upcast
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Val
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Void
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind When
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind While
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind WhitespaceTrivia
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind With
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind Yield
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind YieldBang
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Abstract()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_AdjacentPrefixOperator()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Ampersand()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_AmpersandAmpersand()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_And()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_As()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Asr()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Assert()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Bar()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_BarBar()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_BarRightBrace()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_BarRightBracket()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Base()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Begin()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_BigNumber()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Binder()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_ByteArray()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Char()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Class()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Colon()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_ColonColon()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_ColonEquals()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_ColonGreater()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_ColonQuestionMark()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_ColonQuestionMarkGreater()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Comma()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_CommentTrivia()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Const()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Constraint()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Constructor()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Decimal()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Default()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Delegate()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Do()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_DoBang()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Dollar()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Done()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Dot()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_DotDot()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_DotDotHat()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_DownTo()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Downcast()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Elif()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Else()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_End()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Equals()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Exception()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Extern()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_False()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Finally()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Fixed()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_For()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Fun()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Function()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_FunkyOperatorName()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Global()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Greater()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_GreaterBarRightBracket()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_GreaterRightBracket()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Hash()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_HashElse()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_HashEndIf()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_HashIf()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_HashLight()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_HashLine()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_HighPrecedenceBracketApp()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_HighPrecedenceParenthesisApp()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_HighPrecedenceTypeApp()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Identifier()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Ieee32()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Ieee64()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_If()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_In()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_InactiveCode()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_InfixAmpersandOperator()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_InfixAsr()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_InfixAtHatOperator()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_InfixBarOperator()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_InfixCompareOperator()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_InfixLand()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_InfixLor()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_InfixLsl()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_InfixLsr()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_InfixLxor()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_InfixMod()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_InfixStarDivideModuloOperator()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_InfixStarStarOperator()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Inherit()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Inline()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Instance()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Int16()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Int32()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Int32DotDot()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Int64()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Int8()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Interface()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Internal()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_JoinIn()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_KeywordString()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Lazy()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_LeftArrow()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_LeftBrace()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_LeftBraceBar()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_LeftBracket()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_LeftBracketBar()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_LeftBracketLess()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_LeftParenthesis()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_LeftParenthesisStarRightParenthesis()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_LeftQuote()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Less()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Let()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_LineCommentTrivia()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Match()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_MatchBang()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Member()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Minus()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Module()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Mutable()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Namespace()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_NativeInt()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_New()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_None()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Null()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Of()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_OffsideAssert()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_OffsideBinder()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_OffsideBlockBegin()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_OffsideBlockEnd()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_OffsideBlockSep()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_OffsideDeclEnd()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_OffsideDo()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_OffsideDoBang()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_OffsideElse()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_OffsideEnd()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_OffsideFun()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_OffsideFunction()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_OffsideInterfaceMember()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_OffsideLazy()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_OffsideLet()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_OffsideReset()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_OffsideRightBlockEnd()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_OffsideThen()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_OffsideWith()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Open()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Or()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Override()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_PercentOperator()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_PlusMinusOperator()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_PrefixOperator()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Private()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Public()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_QuestionMark()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_QuestionMarkQuestionMark()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Quote()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Rec()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Reserved()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_RightArrow()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_RightBrace()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_RightBracket()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_RightParenthesis()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_RightQuote()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_RightQuoteDot()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Semicolon()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_SemicolonSemicolon()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Sig()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Star()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Static()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_String()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_StringText()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Struct()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Then()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_To()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_True()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Try()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Type()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_UInt16()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_UInt32()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_UInt64()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_UInt8()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_UNativeInt()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Underscore()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Upcast()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Val()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Void()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_When()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_While()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_WhitespaceTrivia()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_With()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_Yield()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind get_YieldBang()
FSharp.Compiler.EditorServices.FSharpTokenKind: FSharp.Compiler.EditorServices.FSharpTokenKind+Tags
FSharp.Compiler.EditorServices.FSharpTokenKind: Int32 CompareTo(FSharp.Compiler.EditorServices.FSharpTokenKind)
FSharp.Compiler.EditorServices.FSharpTokenKind: Int32 CompareTo(System.Object)
FSharp.Compiler.EditorServices.FSharpTokenKind: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.EditorServices.FSharpTokenKind: Int32 GetHashCode()
FSharp.Compiler.EditorServices.FSharpTokenKind: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.FSharpTokenKind: Int32 Tag
FSharp.Compiler.EditorServices.FSharpTokenKind: Int32 get_Tag()
FSharp.Compiler.EditorServices.FSharpTokenKind: System.String ToString()
FSharp.Compiler.EditorServices.FSharpTokenTag
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 AMP_AMP
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 BAR
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 BAR_BAR
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 BAR_RBRACK
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 BEGIN
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 CLASS
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 COLON
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 COLON_COLON
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 COLON_EQUALS
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 COLON_GREATER
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 COLON_QMARK
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 COLON_QMARK_GREATER
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 COMMA
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 COMMENT
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 DO
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 DOT
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 DOT_DOT
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 DOT_DOT_HAT
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 ELSE
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 EQUALS
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 FUNCTION
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 GREATER
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 GREATER_RBRACK
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 IDENT
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 INFIX_AT_HAT_OP
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 INFIX_BAR_OP
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 INFIX_COMPARE_OP
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 INFIX_STAR_DIV_MOD_OP
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 INT32_DOT_DOT
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 INTERP_STRING_BEGIN_END
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 INTERP_STRING_BEGIN_PART
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 INTERP_STRING_END
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 INTERP_STRING_PART
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 Identifier
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 LARROW
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 LBRACE
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 LBRACK
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 LBRACK_BAR
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 LBRACK_LESS
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 LESS
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 LINE_COMMENT
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 LPAREN
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 MINUS
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 NEW
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 OWITH
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 PERCENT_OP
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 PLUS_MINUS_OP
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 PREFIX_OP
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 QMARK
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 QUOTE
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 RARROW
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 RBRACE
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 RBRACK
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 RPAREN
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 SEMICOLON
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 STAR
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 STRING
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 STRUCT
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 String
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 THEN
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 TRY
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 UNDERSCORE
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 WHITESPACE
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 WITH
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_AMP_AMP()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_BAR()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_BAR_BAR()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_BAR_RBRACK()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_BEGIN()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_CLASS()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_COLON()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_COLON_COLON()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_COLON_EQUALS()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_COLON_GREATER()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_COLON_QMARK()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_COLON_QMARK_GREATER()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_COMMA()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_COMMENT()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_DO()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_DOT()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_DOT_DOT()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_DOT_DOT_HAT()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_ELSE()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_EQUALS()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_FUNCTION()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_GREATER()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_GREATER_RBRACK()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_IDENT()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_INFIX_AT_HAT_OP()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_INFIX_BAR_OP()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_INFIX_COMPARE_OP()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_INFIX_STAR_DIV_MOD_OP()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_INT32_DOT_DOT()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_INTERP_STRING_BEGIN_END()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_INTERP_STRING_BEGIN_PART()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_INTERP_STRING_END()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_INTERP_STRING_PART()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_Identifier()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_LARROW()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_LBRACE()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_LBRACK()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_LBRACK_BAR()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_LBRACK_LESS()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_LESS()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_LINE_COMMENT()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_LPAREN()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_MINUS()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_NEW()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_OWITH()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_PERCENT_OP()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_PLUS_MINUS_OP()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_PREFIX_OP()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_QMARK()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_QUOTE()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_RARROW()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_RBRACE()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_RBRACK()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_RPAREN()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_SEMICOLON()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_STAR()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_STRING()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_STRUCT()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_String()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_THEN()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_TRY()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_UNDERSCORE()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_WHITESPACE()
FSharp.Compiler.EditorServices.FSharpTokenTag: Int32 get_WITH()
FSharp.Compiler.EditorServices.FSharpTokenTriggerClass
FSharp.Compiler.EditorServices.FSharpTokenTriggerClass: FSharp.Compiler.EditorServices.FSharpTokenTriggerClass ChoiceSelect
FSharp.Compiler.EditorServices.FSharpTokenTriggerClass: FSharp.Compiler.EditorServices.FSharpTokenTriggerClass MatchBraces
FSharp.Compiler.EditorServices.FSharpTokenTriggerClass: FSharp.Compiler.EditorServices.FSharpTokenTriggerClass MemberSelect
FSharp.Compiler.EditorServices.FSharpTokenTriggerClass: FSharp.Compiler.EditorServices.FSharpTokenTriggerClass MethodTip
FSharp.Compiler.EditorServices.FSharpTokenTriggerClass: FSharp.Compiler.EditorServices.FSharpTokenTriggerClass None
FSharp.Compiler.EditorServices.FSharpTokenTriggerClass: FSharp.Compiler.EditorServices.FSharpTokenTriggerClass ParamEnd
FSharp.Compiler.EditorServices.FSharpTokenTriggerClass: FSharp.Compiler.EditorServices.FSharpTokenTriggerClass ParamNext
FSharp.Compiler.EditorServices.FSharpTokenTriggerClass: FSharp.Compiler.EditorServices.FSharpTokenTriggerClass ParamStart
FSharp.Compiler.EditorServices.FSharpTokenTriggerClass: Int32 value__
FSharp.Compiler.EditorServices.FSharpTokenizerColorState
FSharp.Compiler.EditorServices.FSharpTokenizerColorState: FSharp.Compiler.EditorServices.FSharpTokenizerColorState CamlOnly
FSharp.Compiler.EditorServices.FSharpTokenizerColorState: FSharp.Compiler.EditorServices.FSharpTokenizerColorState Comment
FSharp.Compiler.EditorServices.FSharpTokenizerColorState: FSharp.Compiler.EditorServices.FSharpTokenizerColorState EndLineThenSkip
FSharp.Compiler.EditorServices.FSharpTokenizerColorState: FSharp.Compiler.EditorServices.FSharpTokenizerColorState EndLineThenToken
FSharp.Compiler.EditorServices.FSharpTokenizerColorState: FSharp.Compiler.EditorServices.FSharpTokenizerColorState IfDefSkip
FSharp.Compiler.EditorServices.FSharpTokenizerColorState: FSharp.Compiler.EditorServices.FSharpTokenizerColorState InitialState
FSharp.Compiler.EditorServices.FSharpTokenizerColorState: FSharp.Compiler.EditorServices.FSharpTokenizerColorState SingleLineComment
FSharp.Compiler.EditorServices.FSharpTokenizerColorState: FSharp.Compiler.EditorServices.FSharpTokenizerColorState String
FSharp.Compiler.EditorServices.FSharpTokenizerColorState: FSharp.Compiler.EditorServices.FSharpTokenizerColorState StringInComment
FSharp.Compiler.EditorServices.FSharpTokenizerColorState: FSharp.Compiler.EditorServices.FSharpTokenizerColorState Token
FSharp.Compiler.EditorServices.FSharpTokenizerColorState: FSharp.Compiler.EditorServices.FSharpTokenizerColorState TripleQuoteString
FSharp.Compiler.EditorServices.FSharpTokenizerColorState: FSharp.Compiler.EditorServices.FSharpTokenizerColorState TripleQuoteStringInComment
FSharp.Compiler.EditorServices.FSharpTokenizerColorState: FSharp.Compiler.EditorServices.FSharpTokenizerColorState VerbatimString
FSharp.Compiler.EditorServices.FSharpTokenizerColorState: FSharp.Compiler.EditorServices.FSharpTokenizerColorState VerbatimStringInComment
FSharp.Compiler.EditorServices.FSharpTokenizerColorState: Int32 value__
FSharp.Compiler.EditorServices.FSharpTokenizerLexState
FSharp.Compiler.EditorServices.FSharpTokenizerLexState: Boolean Equals(FSharp.Compiler.EditorServices.FSharpTokenizerLexState)
FSharp.Compiler.EditorServices.FSharpTokenizerLexState: Boolean Equals(System.Object)
FSharp.Compiler.EditorServices.FSharpTokenizerLexState: FSharp.Compiler.EditorServices.FSharpTokenizerLexState Initial
FSharp.Compiler.EditorServices.FSharpTokenizerLexState: FSharp.Compiler.EditorServices.FSharpTokenizerLexState get_Initial()
FSharp.Compiler.EditorServices.FSharpTokenizerLexState: Int32 GetHashCode()
FSharp.Compiler.EditorServices.FSharpTokenizerLexState: Int64 OtherBits
FSharp.Compiler.EditorServices.FSharpTokenizerLexState: Int64 PosBits
FSharp.Compiler.EditorServices.FSharpTokenizerLexState: Int64 get_OtherBits()
FSharp.Compiler.EditorServices.FSharpTokenizerLexState: Int64 get_PosBits()
FSharp.Compiler.EditorServices.FSharpTokenizerLexState: System.String ToString()
FSharp.Compiler.EditorServices.FSharpTokenizerLexState: Void .ctor(Int64, Int64)
FSharp.Compiler.EditorServices.FSharpToolTip
FSharp.Compiler.EditorServices.FSharpToolTip: FSharp.Compiler.EditorServices.FSharpToolTipElement`1[System.String] ToFSharpToolTipElement(FSharp.Compiler.EditorServices.FSharpToolTipElement`1[FSharp.Compiler.TextLayout.Layout])
FSharp.Compiler.EditorServices.FSharpToolTip: FSharp.Compiler.EditorServices.FSharpToolTipText`1[System.String] ToFSharpToolTipText(FSharp.Compiler.EditorServices.FSharpToolTipText`1[FSharp.Compiler.TextLayout.Layout])
FSharp.Compiler.EditorServices.FSharpToolTipElementData`1[T]
FSharp.Compiler.EditorServices.FSharpToolTipElementData`1[T]: Boolean Equals(FSharp.Compiler.EditorServices.FSharpToolTipElementData`1[T])
FSharp.Compiler.EditorServices.FSharpToolTipElementData`1[T]: Boolean Equals(System.Object)
FSharp.Compiler.EditorServices.FSharpToolTipElementData`1[T]: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.FSharpToolTipElementData`1[T]: FSharp.Compiler.CodeAnalysis.FSharpXmlDoc XmlDoc
FSharp.Compiler.EditorServices.FSharpToolTipElementData`1[T]: FSharp.Compiler.CodeAnalysis.FSharpXmlDoc get_XmlDoc()
FSharp.Compiler.EditorServices.FSharpToolTipElementData`1[T]: Int32 CompareTo(FSharp.Compiler.EditorServices.FSharpToolTipElementData`1[T])
FSharp.Compiler.EditorServices.FSharpToolTipElementData`1[T]: Int32 CompareTo(System.Object)
FSharp.Compiler.EditorServices.FSharpToolTipElementData`1[T]: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.EditorServices.FSharpToolTipElementData`1[T]: Int32 GetHashCode()
FSharp.Compiler.EditorServices.FSharpToolTipElementData`1[T]: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.FSharpToolTipElementData`1[T]: Microsoft.FSharp.Collections.FSharpList`1[T] TypeMapping
FSharp.Compiler.EditorServices.FSharpToolTipElementData`1[T]: Microsoft.FSharp.Collections.FSharpList`1[T] get_TypeMapping()
FSharp.Compiler.EditorServices.FSharpToolTipElementData`1[T]: Microsoft.FSharp.Core.FSharpOption`1[System.String] ParamName
FSharp.Compiler.EditorServices.FSharpToolTipElementData`1[T]: Microsoft.FSharp.Core.FSharpOption`1[System.String] get_ParamName()
FSharp.Compiler.EditorServices.FSharpToolTipElementData`1[T]: Microsoft.FSharp.Core.FSharpOption`1[T] Remarks
FSharp.Compiler.EditorServices.FSharpToolTipElementData`1[T]: Microsoft.FSharp.Core.FSharpOption`1[T] get_Remarks()
FSharp.Compiler.EditorServices.FSharpToolTipElementData`1[T]: System.String ToString()
FSharp.Compiler.EditorServices.FSharpToolTipElementData`1[T]: T MainDescription
FSharp.Compiler.EditorServices.FSharpToolTipElementData`1[T]: T get_MainDescription()
FSharp.Compiler.EditorServices.FSharpToolTipElementData`1[T]: Void .ctor(T, FSharp.Compiler.CodeAnalysis.FSharpXmlDoc, Microsoft.FSharp.Collections.FSharpList`1[T], Microsoft.FSharp.Core.FSharpOption`1[T], Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.EditorServices.FSharpToolTipElement`1+CompositionError[T]: System.String errorText
FSharp.Compiler.EditorServices.FSharpToolTipElement`1+CompositionError[T]: System.String get_errorText()
FSharp.Compiler.EditorServices.FSharpToolTipElement`1+Group[T]: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.EditorServices.FSharpToolTipElementData`1[T]] elements
FSharp.Compiler.EditorServices.FSharpToolTipElement`1+Group[T]: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.EditorServices.FSharpToolTipElementData`1[T]] get_elements()
FSharp.Compiler.EditorServices.FSharpToolTipElement`1+Tags[T]: Int32 CompositionError
FSharp.Compiler.EditorServices.FSharpToolTipElement`1+Tags[T]: Int32 Group
FSharp.Compiler.EditorServices.FSharpToolTipElement`1+Tags[T]: Int32 None
FSharp.Compiler.EditorServices.FSharpToolTipElement`1[T]
FSharp.Compiler.EditorServices.FSharpToolTipElement`1[T]: Boolean Equals(FSharp.Compiler.EditorServices.FSharpToolTipElement`1[T])
FSharp.Compiler.EditorServices.FSharpToolTipElement`1[T]: Boolean Equals(System.Object)
FSharp.Compiler.EditorServices.FSharpToolTipElement`1[T]: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.FSharpToolTipElement`1[T]: Boolean IsCompositionError
FSharp.Compiler.EditorServices.FSharpToolTipElement`1[T]: Boolean IsGroup
FSharp.Compiler.EditorServices.FSharpToolTipElement`1[T]: Boolean IsNone
FSharp.Compiler.EditorServices.FSharpToolTipElement`1[T]: Boolean get_IsCompositionError()
FSharp.Compiler.EditorServices.FSharpToolTipElement`1[T]: Boolean get_IsGroup()
FSharp.Compiler.EditorServices.FSharpToolTipElement`1[T]: Boolean get_IsNone()
FSharp.Compiler.EditorServices.FSharpToolTipElement`1[T]: FSharp.Compiler.EditorServices.FSharpToolTipElement`1+CompositionError[T]
FSharp.Compiler.EditorServices.FSharpToolTipElement`1[T]: FSharp.Compiler.EditorServices.FSharpToolTipElement`1+Group[T]
FSharp.Compiler.EditorServices.FSharpToolTipElement`1[T]: FSharp.Compiler.EditorServices.FSharpToolTipElement`1+Tags[T]
FSharp.Compiler.EditorServices.FSharpToolTipElement`1[T]: FSharp.Compiler.EditorServices.FSharpToolTipElement`1[T] NewCompositionError(System.String)
FSharp.Compiler.EditorServices.FSharpToolTipElement`1[T]: FSharp.Compiler.EditorServices.FSharpToolTipElement`1[T] NewGroup(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.EditorServices.FSharpToolTipElementData`1[T]])
FSharp.Compiler.EditorServices.FSharpToolTipElement`1[T]: FSharp.Compiler.EditorServices.FSharpToolTipElement`1[T] None
FSharp.Compiler.EditorServices.FSharpToolTipElement`1[T]: FSharp.Compiler.EditorServices.FSharpToolTipElement`1[T] Single(T, FSharp.Compiler.CodeAnalysis.FSharpXmlDoc, Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Collections.FSharpList`1[T]], Microsoft.FSharp.Core.FSharpOption`1[System.String], Microsoft.FSharp.Core.FSharpOption`1[T])
FSharp.Compiler.EditorServices.FSharpToolTipElement`1[T]: FSharp.Compiler.EditorServices.FSharpToolTipElement`1[T] get_None()
FSharp.Compiler.EditorServices.FSharpToolTipElement`1[T]: Int32 CompareTo(FSharp.Compiler.EditorServices.FSharpToolTipElement`1[T])
FSharp.Compiler.EditorServices.FSharpToolTipElement`1[T]: Int32 CompareTo(System.Object)
FSharp.Compiler.EditorServices.FSharpToolTipElement`1[T]: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.EditorServices.FSharpToolTipElement`1[T]: Int32 GetHashCode()
FSharp.Compiler.EditorServices.FSharpToolTipElement`1[T]: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.FSharpToolTipElement`1[T]: Int32 Tag
FSharp.Compiler.EditorServices.FSharpToolTipElement`1[T]: Int32 get_Tag()
FSharp.Compiler.EditorServices.FSharpToolTipElement`1[T]: System.String ToString()
FSharp.Compiler.EditorServices.FSharpToolTipText`1[T]
FSharp.Compiler.EditorServices.FSharpToolTipText`1[T]: Boolean Equals(FSharp.Compiler.EditorServices.FSharpToolTipText`1[T])
FSharp.Compiler.EditorServices.FSharpToolTipText`1[T]: Boolean Equals(System.Object)
FSharp.Compiler.EditorServices.FSharpToolTipText`1[T]: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.FSharpToolTipText`1[T]: FSharp.Compiler.EditorServices.FSharpToolTipText`1[T] NewFSharpToolTipText(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.EditorServices.FSharpToolTipElement`1[T]])
FSharp.Compiler.EditorServices.FSharpToolTipText`1[T]: Int32 CompareTo(FSharp.Compiler.EditorServices.FSharpToolTipText`1[T])
FSharp.Compiler.EditorServices.FSharpToolTipText`1[T]: Int32 CompareTo(System.Object)
FSharp.Compiler.EditorServices.FSharpToolTipText`1[T]: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.EditorServices.FSharpToolTipText`1[T]: Int32 GetHashCode()
FSharp.Compiler.EditorServices.FSharpToolTipText`1[T]: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.FSharpToolTipText`1[T]: Int32 Tag
FSharp.Compiler.EditorServices.FSharpToolTipText`1[T]: Int32 get_Tag()
FSharp.Compiler.EditorServices.FSharpToolTipText`1[T]: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.EditorServices.FSharpToolTipElement`1[T]] Item
FSharp.Compiler.EditorServices.FSharpToolTipText`1[T]: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.EditorServices.FSharpToolTipElement`1[T]] get_Item()
FSharp.Compiler.EditorServices.FSharpToolTipText`1[T]: System.String ToString()
FSharp.Compiler.EditorServices.FSharpUnresolvedSymbol
FSharp.Compiler.EditorServices.FSharpUnresolvedSymbol: Boolean Equals(FSharp.Compiler.EditorServices.FSharpUnresolvedSymbol)
FSharp.Compiler.EditorServices.FSharpUnresolvedSymbol: Boolean Equals(System.Object)
FSharp.Compiler.EditorServices.FSharpUnresolvedSymbol: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.FSharpUnresolvedSymbol: Int32 CompareTo(FSharp.Compiler.EditorServices.FSharpUnresolvedSymbol)
FSharp.Compiler.EditorServices.FSharpUnresolvedSymbol: Int32 CompareTo(System.Object)
FSharp.Compiler.EditorServices.FSharpUnresolvedSymbol: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.EditorServices.FSharpUnresolvedSymbol: Int32 GetHashCode()
FSharp.Compiler.EditorServices.FSharpUnresolvedSymbol: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.FSharpUnresolvedSymbol: System.String DisplayName
FSharp.Compiler.EditorServices.FSharpUnresolvedSymbol: System.String FullName
FSharp.Compiler.EditorServices.FSharpUnresolvedSymbol: System.String ToString()
FSharp.Compiler.EditorServices.FSharpUnresolvedSymbol: System.String get_DisplayName()
FSharp.Compiler.EditorServices.FSharpUnresolvedSymbol: System.String get_FullName()
FSharp.Compiler.EditorServices.FSharpUnresolvedSymbol: System.String[] Namespace
FSharp.Compiler.EditorServices.FSharpUnresolvedSymbol: System.String[] get_Namespace()
FSharp.Compiler.EditorServices.FSharpUnresolvedSymbol: Void .ctor(System.String, System.String, System.String[])
FSharp.Compiler.EditorServices.IAssemblyContentCache
FSharp.Compiler.EditorServices.IAssemblyContentCache: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.EditorServices.AssemblyContentCacheEntry] TryGet(System.String)
FSharp.Compiler.EditorServices.IAssemblyContentCache: Void Set(System.String, FSharp.Compiler.EditorServices.AssemblyContentCacheEntry)
FSharp.Compiler.EditorServices.InterfaceData
FSharp.Compiler.EditorServices.InterfaceData+Interface: FSharp.Compiler.Syntax.SynType get_interfaceType()
FSharp.Compiler.EditorServices.InterfaceData+Interface: FSharp.Compiler.Syntax.SynType interfaceType
FSharp.Compiler.EditorServices.InterfaceData+Interface: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynMemberDefn]] get_memberDefns()
FSharp.Compiler.EditorServices.InterfaceData+Interface: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynMemberDefn]] memberDefns
FSharp.Compiler.EditorServices.InterfaceData+ObjExpr: FSharp.Compiler.Syntax.SynType get_objType()
FSharp.Compiler.EditorServices.InterfaceData+ObjExpr: FSharp.Compiler.Syntax.SynType objType
FSharp.Compiler.EditorServices.InterfaceData+ObjExpr: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynBinding] bindings
FSharp.Compiler.EditorServices.InterfaceData+ObjExpr: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynBinding] get_bindings()
FSharp.Compiler.EditorServices.InterfaceData+Tags: Int32 Interface
FSharp.Compiler.EditorServices.InterfaceData+Tags: Int32 ObjExpr
FSharp.Compiler.EditorServices.InterfaceData: Boolean IsInterface
FSharp.Compiler.EditorServices.InterfaceData: Boolean IsObjExpr
FSharp.Compiler.EditorServices.InterfaceData: Boolean get_IsInterface()
FSharp.Compiler.EditorServices.InterfaceData: Boolean get_IsObjExpr()
FSharp.Compiler.EditorServices.InterfaceData: FSharp.Compiler.EditorServices.InterfaceData NewInterface(FSharp.Compiler.Syntax.SynType, Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynMemberDefn]])
FSharp.Compiler.EditorServices.InterfaceData: FSharp.Compiler.EditorServices.InterfaceData NewObjExpr(FSharp.Compiler.Syntax.SynType, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynBinding])
FSharp.Compiler.EditorServices.InterfaceData: FSharp.Compiler.EditorServices.InterfaceData+Interface
FSharp.Compiler.EditorServices.InterfaceData: FSharp.Compiler.EditorServices.InterfaceData+ObjExpr
FSharp.Compiler.EditorServices.InterfaceData: FSharp.Compiler.EditorServices.InterfaceData+Tags
FSharp.Compiler.EditorServices.InterfaceData: FSharp.Compiler.Text.Range Range
FSharp.Compiler.EditorServices.InterfaceData: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.EditorServices.InterfaceData: Int32 Tag
FSharp.Compiler.EditorServices.InterfaceData: Int32 get_Tag()
FSharp.Compiler.EditorServices.InterfaceData: System.String ToString()
FSharp.Compiler.EditorServices.InterfaceData: System.String[] TypeParameters
FSharp.Compiler.EditorServices.InterfaceData: System.String[] get_TypeParameters()
FSharp.Compiler.EditorServices.InterfaceStubGenerator
FSharp.Compiler.EditorServices.InterfaceStubGenerator: Boolean HasNoInterfaceMember(FSharp.Compiler.CodeAnalysis.FSharpEntity)
FSharp.Compiler.EditorServices.InterfaceStubGenerator: Boolean IsInterface(FSharp.Compiler.CodeAnalysis.FSharpEntity)
FSharp.Compiler.EditorServices.InterfaceStubGenerator: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[System.String,FSharp.Compiler.Text.Range]] GetMemberNameAndRanges(FSharp.Compiler.EditorServices.InterfaceData)
FSharp.Compiler.EditorServices.InterfaceStubGenerator: Microsoft.FSharp.Control.FSharpAsync`1[Microsoft.FSharp.Collections.FSharpSet`1[System.String]] GetImplementedMemberSignatures(Microsoft.FSharp.Core.FSharpFunc`2[System.Tuple`2[System.String,FSharp.Compiler.Text.Range],Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.FSharpSymbolUse]], FSharp.Compiler.CodeAnalysis.FSharpDisplayContext, FSharp.Compiler.EditorServices.InterfaceData)
FSharp.Compiler.EditorServices.InterfaceStubGenerator: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.EditorServices.InterfaceData] TryFindInterfaceDeclaration(FSharp.Compiler.Text.Position, FSharp.Compiler.Syntax.ParsedInput)
FSharp.Compiler.EditorServices.InterfaceStubGenerator: System.Collections.Generic.IEnumerable`1[System.Tuple`2[FSharp.Compiler.CodeAnalysis.FSharpMemberOrFunctionOrValue,System.Collections.Generic.IEnumerable`1[System.Tuple`2[FSharp.Compiler.CodeAnalysis.FSharpGenericParameter,FSharp.Compiler.CodeAnalysis.FSharpType]]]] GetInterfaceMembers(FSharp.Compiler.CodeAnalysis.FSharpEntity)
FSharp.Compiler.EditorServices.InterfaceStubGenerator: System.String FormatInterface(Int32, Int32, System.String[], System.String, System.String, FSharp.Compiler.CodeAnalysis.FSharpDisplayContext, Microsoft.FSharp.Collections.FSharpSet`1[System.String], FSharp.Compiler.CodeAnalysis.FSharpEntity, Boolean)
FSharp.Compiler.EditorServices.LookupType
FSharp.Compiler.EditorServices.LookupType+Tags: Int32 Fuzzy
FSharp.Compiler.EditorServices.LookupType+Tags: Int32 Precise
FSharp.Compiler.EditorServices.LookupType: Boolean Equals(FSharp.Compiler.EditorServices.LookupType)
FSharp.Compiler.EditorServices.LookupType: Boolean Equals(System.Object)
FSharp.Compiler.EditorServices.LookupType: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.LookupType: Boolean IsFuzzy
FSharp.Compiler.EditorServices.LookupType: Boolean IsPrecise
FSharp.Compiler.EditorServices.LookupType: Boolean get_IsFuzzy()
FSharp.Compiler.EditorServices.LookupType: Boolean get_IsPrecise()
FSharp.Compiler.EditorServices.LookupType: FSharp.Compiler.EditorServices.LookupType Fuzzy
FSharp.Compiler.EditorServices.LookupType: FSharp.Compiler.EditorServices.LookupType Precise
FSharp.Compiler.EditorServices.LookupType: FSharp.Compiler.EditorServices.LookupType get_Fuzzy()
FSharp.Compiler.EditorServices.LookupType: FSharp.Compiler.EditorServices.LookupType get_Precise()
FSharp.Compiler.EditorServices.LookupType: FSharp.Compiler.EditorServices.LookupType+Tags
FSharp.Compiler.EditorServices.LookupType: Int32 CompareTo(FSharp.Compiler.EditorServices.LookupType)
FSharp.Compiler.EditorServices.LookupType: Int32 CompareTo(System.Object)
FSharp.Compiler.EditorServices.LookupType: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.EditorServices.LookupType: Int32 GetHashCode()
FSharp.Compiler.EditorServices.LookupType: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.LookupType: Int32 Tag
FSharp.Compiler.EditorServices.LookupType: Int32 get_Tag()
FSharp.Compiler.EditorServices.LookupType: System.String ToString()
FSharp.Compiler.EditorServices.NavigateTo
FSharp.Compiler.EditorServices.NavigateTo+Container: Boolean Equals(Container)
FSharp.Compiler.EditorServices.NavigateTo+Container: Boolean Equals(System.Object)
FSharp.Compiler.EditorServices.NavigateTo+Container: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.NavigateTo+Container: ContainerType Type
FSharp.Compiler.EditorServices.NavigateTo+Container: ContainerType get_Type()
FSharp.Compiler.EditorServices.NavigateTo+Container: Int32 CompareTo(Container)
FSharp.Compiler.EditorServices.NavigateTo+Container: Int32 CompareTo(System.Object)
FSharp.Compiler.EditorServices.NavigateTo+Container: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.EditorServices.NavigateTo+Container: Int32 GetHashCode()
FSharp.Compiler.EditorServices.NavigateTo+Container: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.NavigateTo+Container: System.String Name
FSharp.Compiler.EditorServices.NavigateTo+Container: System.String ToString()
FSharp.Compiler.EditorServices.NavigateTo+Container: System.String get_Name()
FSharp.Compiler.EditorServices.NavigateTo+Container: Void .ctor(ContainerType, System.String)
FSharp.Compiler.EditorServices.NavigateTo+ContainerType+Tags: Int32 Exception
FSharp.Compiler.EditorServices.NavigateTo+ContainerType+Tags: Int32 File
FSharp.Compiler.EditorServices.NavigateTo+ContainerType+Tags: Int32 Module
FSharp.Compiler.EditorServices.NavigateTo+ContainerType+Tags: Int32 Namespace
FSharp.Compiler.EditorServices.NavigateTo+ContainerType+Tags: Int32 Type
FSharp.Compiler.EditorServices.NavigateTo+ContainerType: Boolean Equals(ContainerType)
FSharp.Compiler.EditorServices.NavigateTo+ContainerType: Boolean Equals(System.Object)
FSharp.Compiler.EditorServices.NavigateTo+ContainerType: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.NavigateTo+ContainerType: Boolean IsException
FSharp.Compiler.EditorServices.NavigateTo+ContainerType: Boolean IsFile
FSharp.Compiler.EditorServices.NavigateTo+ContainerType: Boolean IsModule
FSharp.Compiler.EditorServices.NavigateTo+ContainerType: Boolean IsNamespace
FSharp.Compiler.EditorServices.NavigateTo+ContainerType: Boolean IsType
FSharp.Compiler.EditorServices.NavigateTo+ContainerType: Boolean get_IsException()
FSharp.Compiler.EditorServices.NavigateTo+ContainerType: Boolean get_IsFile()
FSharp.Compiler.EditorServices.NavigateTo+ContainerType: Boolean get_IsModule()
FSharp.Compiler.EditorServices.NavigateTo+ContainerType: Boolean get_IsNamespace()
FSharp.Compiler.EditorServices.NavigateTo+ContainerType: Boolean get_IsType()
FSharp.Compiler.EditorServices.NavigateTo+ContainerType: ContainerType Exception
FSharp.Compiler.EditorServices.NavigateTo+ContainerType: ContainerType File
FSharp.Compiler.EditorServices.NavigateTo+ContainerType: ContainerType Module
FSharp.Compiler.EditorServices.NavigateTo+ContainerType: ContainerType Namespace
FSharp.Compiler.EditorServices.NavigateTo+ContainerType: ContainerType Type
FSharp.Compiler.EditorServices.NavigateTo+ContainerType: ContainerType get_Exception()
FSharp.Compiler.EditorServices.NavigateTo+ContainerType: ContainerType get_File()
FSharp.Compiler.EditorServices.NavigateTo+ContainerType: ContainerType get_Module()
FSharp.Compiler.EditorServices.NavigateTo+ContainerType: ContainerType get_Namespace()
FSharp.Compiler.EditorServices.NavigateTo+ContainerType: ContainerType get_Type()
FSharp.Compiler.EditorServices.NavigateTo+ContainerType: FSharp.Compiler.EditorServices.NavigateTo+ContainerType+Tags
FSharp.Compiler.EditorServices.NavigateTo+ContainerType: Int32 CompareTo(ContainerType)
FSharp.Compiler.EditorServices.NavigateTo+ContainerType: Int32 CompareTo(System.Object)
FSharp.Compiler.EditorServices.NavigateTo+ContainerType: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.EditorServices.NavigateTo+ContainerType: Int32 GetHashCode()
FSharp.Compiler.EditorServices.NavigateTo+ContainerType: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.NavigateTo+ContainerType: Int32 Tag
FSharp.Compiler.EditorServices.NavigateTo+ContainerType: Int32 get_Tag()
FSharp.Compiler.EditorServices.NavigateTo+ContainerType: System.String ToString()
FSharp.Compiler.EditorServices.NavigateTo+NavigableItem: Boolean Equals(NavigableItem)
FSharp.Compiler.EditorServices.NavigateTo+NavigableItem: Boolean Equals(System.Object)
FSharp.Compiler.EditorServices.NavigateTo+NavigableItem: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.NavigateTo+NavigableItem: Boolean IsSignature
FSharp.Compiler.EditorServices.NavigateTo+NavigableItem: Boolean get_IsSignature()
FSharp.Compiler.EditorServices.NavigateTo+NavigableItem: Container Container
FSharp.Compiler.EditorServices.NavigateTo+NavigableItem: Container get_Container()
FSharp.Compiler.EditorServices.NavigateTo+NavigableItem: FSharp.Compiler.Text.Range Range
FSharp.Compiler.EditorServices.NavigateTo+NavigableItem: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.EditorServices.NavigateTo+NavigableItem: Int32 GetHashCode()
FSharp.Compiler.EditorServices.NavigateTo+NavigableItem: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.NavigateTo+NavigableItem: NavigableItemKind Kind
FSharp.Compiler.EditorServices.NavigateTo+NavigableItem: NavigableItemKind get_Kind()
FSharp.Compiler.EditorServices.NavigateTo+NavigableItem: System.String Name
FSharp.Compiler.EditorServices.NavigateTo+NavigableItem: System.String ToString()
FSharp.Compiler.EditorServices.NavigateTo+NavigableItem: System.String get_Name()
FSharp.Compiler.EditorServices.NavigateTo+NavigableItem: Void .ctor(System.String, FSharp.Compiler.Text.Range, Boolean, NavigableItemKind, Container)
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind+Tags: Int32 Constructor
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind+Tags: Int32 EnumCase
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind+Tags: Int32 Exception
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind+Tags: Int32 Field
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind+Tags: Int32 Member
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind+Tags: Int32 Module
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind+Tags: Int32 ModuleAbbreviation
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind+Tags: Int32 ModuleValue
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind+Tags: Int32 Property
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind+Tags: Int32 Type
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind+Tags: Int32 UnionCase
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: Boolean Equals(NavigableItemKind)
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: Boolean Equals(System.Object)
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: Boolean IsConstructor
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: Boolean IsEnumCase
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: Boolean IsException
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: Boolean IsField
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: Boolean IsMember
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: Boolean IsModule
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: Boolean IsModuleAbbreviation
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: Boolean IsModuleValue
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: Boolean IsProperty
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: Boolean IsType
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: Boolean IsUnionCase
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: Boolean get_IsConstructor()
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: Boolean get_IsEnumCase()
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: Boolean get_IsException()
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: Boolean get_IsField()
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: Boolean get_IsMember()
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: Boolean get_IsModule()
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: Boolean get_IsModuleAbbreviation()
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: Boolean get_IsModuleValue()
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: Boolean get_IsProperty()
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: Boolean get_IsType()
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: Boolean get_IsUnionCase()
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind+Tags
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: Int32 CompareTo(NavigableItemKind)
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: Int32 CompareTo(System.Object)
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: Int32 GetHashCode()
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: Int32 Tag
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: Int32 get_Tag()
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: NavigableItemKind Constructor
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: NavigableItemKind EnumCase
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: NavigableItemKind Exception
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: NavigableItemKind Field
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: NavigableItemKind Member
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: NavigableItemKind Module
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: NavigableItemKind ModuleAbbreviation
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: NavigableItemKind ModuleValue
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: NavigableItemKind Property
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: NavigableItemKind Type
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: NavigableItemKind UnionCase
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: NavigableItemKind get_Constructor()
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: NavigableItemKind get_EnumCase()
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: NavigableItemKind get_Exception()
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: NavigableItemKind get_Field()
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: NavigableItemKind get_Member()
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: NavigableItemKind get_Module()
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: NavigableItemKind get_ModuleAbbreviation()
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: NavigableItemKind get_ModuleValue()
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: NavigableItemKind get_Property()
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: NavigableItemKind get_Type()
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: NavigableItemKind get_UnionCase()
FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind: System.String ToString()
FSharp.Compiler.EditorServices.NavigateTo: FSharp.Compiler.EditorServices.NavigateTo+Container
FSharp.Compiler.EditorServices.NavigateTo: FSharp.Compiler.EditorServices.NavigateTo+ContainerType
FSharp.Compiler.EditorServices.NavigateTo: FSharp.Compiler.EditorServices.NavigateTo+NavigableItem
FSharp.Compiler.EditorServices.NavigateTo: FSharp.Compiler.EditorServices.NavigateTo+NavigableItemKind
FSharp.Compiler.EditorServices.NavigateTo: NavigableItem[] getNavigableItems(FSharp.Compiler.Syntax.ParsedInput)
FSharp.Compiler.EditorServices.ParsedInput
FSharp.Compiler.EditorServices.ParsedInput: FSharp.Compiler.EditorServices.InsertionContext FindNearestPointToInsertOpenDeclaration(Int32, FSharp.Compiler.Syntax.ParsedInput, System.String[], FSharp.Compiler.EditorServices.OpenStatementInsertionPoint)
FSharp.Compiler.EditorServices.ParsedInput: FSharp.Compiler.Text.Position AdjustInsertionPoint(Microsoft.FSharp.Core.FSharpFunc`2[System.Int32,System.String], FSharp.Compiler.EditorServices.InsertionContext)
FSharp.Compiler.EditorServices.ParsedInput: Microsoft.FSharp.Core.FSharpFunc`2[System.Tuple`4[Microsoft.FSharp.Core.FSharpOption`1[System.String[]],Microsoft.FSharp.Core.FSharpOption`1[System.String[]],Microsoft.FSharp.Core.FSharpOption`1[System.String[]],System.String[]],System.Tuple`2[FSharp.Compiler.EditorServices.InsertionContextEntity,FSharp.Compiler.EditorServices.InsertionContext][]] TryFindInsertionContext(Int32, FSharp.Compiler.Syntax.ParsedInput, FSharp.Compiler.EditorServices.MaybeUnresolvedIdent[], FSharp.Compiler.EditorServices.OpenStatementInsertionPoint)
FSharp.Compiler.EditorServices.ParsedInput: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.EditorServices.CompletionContext] TryGetCompletionContext(FSharp.Compiler.Text.Position, FSharp.Compiler.Syntax.ParsedInput, System.String)
FSharp.Compiler.EditorServices.ParsedInput: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.EditorServices.EntityKind] GetEntityKind(FSharp.Compiler.Text.Position, FSharp.Compiler.Syntax.ParsedInput)
FSharp.Compiler.EditorServices.ParsedInput: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] GetRangeOfExprLeftOfDot(FSharp.Compiler.Text.Position, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.ParsedInput])
FSharp.Compiler.EditorServices.ParsedInput: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident]] GetLongIdentAt(FSharp.Compiler.Syntax.ParsedInput, FSharp.Compiler.Text.Position)
FSharp.Compiler.EditorServices.ParsedInput: Microsoft.FSharp.Core.FSharpOption`1[System.String] TryFindExpressionIslandInPosition(FSharp.Compiler.Text.Position, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.ParsedInput])
FSharp.Compiler.EditorServices.ParsedInput: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.Text.Position,System.Boolean]] TryFindExpressionASTLeftOfDotLeftOfCursor(FSharp.Compiler.Text.Position, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.ParsedInput])
FSharp.Compiler.EditorServices.ParsedInput: System.String[] GetFullNameOfSmallestModuleOrNamespaceAtPoint(FSharp.Compiler.Syntax.ParsedInput, FSharp.Compiler.Text.Position)
FSharp.Compiler.EditorServices.PartialLongName
FSharp.Compiler.EditorServices.PartialLongName: Boolean Equals(FSharp.Compiler.EditorServices.PartialLongName)
FSharp.Compiler.EditorServices.PartialLongName: Boolean Equals(System.Object)
FSharp.Compiler.EditorServices.PartialLongName: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.PartialLongName: FSharp.Compiler.EditorServices.PartialLongName Empty(Int32)
FSharp.Compiler.EditorServices.PartialLongName: Int32 CompareTo(FSharp.Compiler.EditorServices.PartialLongName)
FSharp.Compiler.EditorServices.PartialLongName: Int32 CompareTo(System.Object)
FSharp.Compiler.EditorServices.PartialLongName: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.EditorServices.PartialLongName: Int32 EndColumn
FSharp.Compiler.EditorServices.PartialLongName: Int32 GetHashCode()
FSharp.Compiler.EditorServices.PartialLongName: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.PartialLongName: Int32 get_EndColumn()
FSharp.Compiler.EditorServices.PartialLongName: Microsoft.FSharp.Collections.FSharpList`1[System.String] QualifyingIdents
FSharp.Compiler.EditorServices.PartialLongName: Microsoft.FSharp.Collections.FSharpList`1[System.String] get_QualifyingIdents()
FSharp.Compiler.EditorServices.PartialLongName: Microsoft.FSharp.Core.FSharpOption`1[System.Int32] LastDotPos
FSharp.Compiler.EditorServices.PartialLongName: Microsoft.FSharp.Core.FSharpOption`1[System.Int32] get_LastDotPos()
FSharp.Compiler.EditorServices.PartialLongName: System.String PartialIdent
FSharp.Compiler.EditorServices.PartialLongName: System.String ToString()
FSharp.Compiler.EditorServices.PartialLongName: System.String get_PartialIdent()
FSharp.Compiler.EditorServices.PartialLongName: Void .ctor(Microsoft.FSharp.Collections.FSharpList`1[System.String], System.String, Int32, Microsoft.FSharp.Core.FSharpOption`1[System.Int32])
FSharp.Compiler.EditorServices.QuickParse
FSharp.Compiler.EditorServices.QuickParse: Boolean TestMemberOrOverrideDeclaration(FSharp.Compiler.EditorServices.FSharpTokenInfo[])
FSharp.Compiler.EditorServices.QuickParse: FSharp.Compiler.EditorServices.PartialLongName GetPartialLongNameEx(System.String, Int32)
FSharp.Compiler.EditorServices.QuickParse: Int32 CorrectIdentifierToken(System.String, Int32)
FSharp.Compiler.EditorServices.QuickParse: Int32 MagicalAdjustmentConstant
FSharp.Compiler.EditorServices.QuickParse: Int32 get_MagicalAdjustmentConstant()
FSharp.Compiler.EditorServices.QuickParse: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`3[System.String,System.Int32,System.Boolean]] GetCompleteIdentifierIsland(Boolean, System.String, Int32)
FSharp.Compiler.EditorServices.QuickParse: System.Tuple`2[Microsoft.FSharp.Collections.FSharpList`1[System.String],System.String] GetPartialLongName(System.String, Int32)
FSharp.Compiler.EditorServices.SimplifyNames
FSharp.Compiler.EditorServices.SimplifyNames+SimplifiableRange: Boolean Equals(SimplifiableRange)
FSharp.Compiler.EditorServices.SimplifyNames+SimplifiableRange: Boolean Equals(System.Object)
FSharp.Compiler.EditorServices.SimplifyNames+SimplifiableRange: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.SimplifyNames+SimplifiableRange: FSharp.Compiler.Text.Range Range
FSharp.Compiler.EditorServices.SimplifyNames+SimplifiableRange: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.EditorServices.SimplifyNames+SimplifiableRange: Int32 GetHashCode()
FSharp.Compiler.EditorServices.SimplifyNames+SimplifiableRange: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.SimplifyNames+SimplifiableRange: System.String RelativeName
FSharp.Compiler.EditorServices.SimplifyNames+SimplifiableRange: System.String ToString()
FSharp.Compiler.EditorServices.SimplifyNames+SimplifiableRange: System.String get_RelativeName()
FSharp.Compiler.EditorServices.SimplifyNames+SimplifiableRange: Void .ctor(FSharp.Compiler.Text.Range, System.String)
FSharp.Compiler.EditorServices.SimplifyNames: FSharp.Compiler.EditorServices.SimplifyNames+SimplifiableRange
FSharp.Compiler.EditorServices.SimplifyNames: Microsoft.FSharp.Control.FSharpAsync`1[System.Collections.Generic.IEnumerable`1[FSharp.Compiler.EditorServices.SimplifyNames+SimplifiableRange]] getSimplifiableNames(FSharp.Compiler.CodeAnalysis.FSharpCheckFileResults, Microsoft.FSharp.Core.FSharpFunc`2[System.Int32,System.String])
FSharp.Compiler.EditorServices.Structure
FSharp.Compiler.EditorServices.Structure+Collapse+Tags: Int32 Below
FSharp.Compiler.EditorServices.Structure+Collapse+Tags: Int32 Same
FSharp.Compiler.EditorServices.Structure+Collapse: Boolean Equals(Collapse)
FSharp.Compiler.EditorServices.Structure+Collapse: Boolean Equals(System.Object)
FSharp.Compiler.EditorServices.Structure+Collapse: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.Structure+Collapse: Boolean IsBelow
FSharp.Compiler.EditorServices.Structure+Collapse: Boolean IsSame
FSharp.Compiler.EditorServices.Structure+Collapse: Boolean get_IsBelow()
FSharp.Compiler.EditorServices.Structure+Collapse: Boolean get_IsSame()
FSharp.Compiler.EditorServices.Structure+Collapse: Collapse Below
FSharp.Compiler.EditorServices.Structure+Collapse: Collapse Same
FSharp.Compiler.EditorServices.Structure+Collapse: Collapse get_Below()
FSharp.Compiler.EditorServices.Structure+Collapse: Collapse get_Same()
FSharp.Compiler.EditorServices.Structure+Collapse: FSharp.Compiler.EditorServices.Structure+Collapse+Tags
FSharp.Compiler.EditorServices.Structure+Collapse: Int32 CompareTo(Collapse)
FSharp.Compiler.EditorServices.Structure+Collapse: Int32 CompareTo(System.Object)
FSharp.Compiler.EditorServices.Structure+Collapse: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.EditorServices.Structure+Collapse: Int32 GetHashCode()
FSharp.Compiler.EditorServices.Structure+Collapse: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.Structure+Collapse: Int32 Tag
FSharp.Compiler.EditorServices.Structure+Collapse: Int32 get_Tag()
FSharp.Compiler.EditorServices.Structure+Collapse: System.String ToString()
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 ArrayOrList
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 Attribute
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 Comment
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 CompExpr
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 CompExprInternal
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 Do
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 ElseInIfThenElse
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 EnumCase
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 FinallyInTryFinally
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 For
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 HashDirective
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 IfThenElse
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 Interface
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 Lambda
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 LetOrUse
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 LetOrUseBang
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 Match
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 MatchBang
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 MatchClause
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 MatchLambda
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 Member
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 Module
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 Namespace
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 New
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 ObjExpr
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 Open
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 Quote
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 Record
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 RecordDefn
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 RecordField
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 SpecialFunc
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 ThenInIfThenElse
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 TryFinally
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 TryInTryFinally
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 TryInTryWith
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 TryWith
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 Tuple
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 Type
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 TypeExtension
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 UnionCase
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 UnionDefn
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 Val
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 While
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 WithInTryWith
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 XmlDocComment
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 YieldOrReturn
FSharp.Compiler.EditorServices.Structure+Scope+Tags: Int32 YieldOrReturnBang
FSharp.Compiler.EditorServices.Structure+Scope: Boolean Equals(Scope)
FSharp.Compiler.EditorServices.Structure+Scope: Boolean Equals(System.Object)
FSharp.Compiler.EditorServices.Structure+Scope: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsArrayOrList
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsAttribute
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsComment
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsCompExpr
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsCompExprInternal
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsDo
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsElseInIfThenElse
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsEnumCase
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsFinallyInTryFinally
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsFor
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsHashDirective
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsIfThenElse
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsInterface
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsLambda
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsLetOrUse
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsLetOrUseBang
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsMatch
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsMatchBang
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsMatchClause
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsMatchLambda
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsMember
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsModule
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsNamespace
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsNew
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsObjExpr
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsOpen
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsQuote
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsRecord
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsRecordDefn
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsRecordField
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsSpecialFunc
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsThenInIfThenElse
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsTryFinally
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsTryInTryFinally
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsTryInTryWith
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsTryWith
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsTuple
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsType
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsTypeExtension
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsUnionCase
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsUnionDefn
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsVal
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsWhile
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsWithInTryWith
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsXmlDocComment
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsYieldOrReturn
FSharp.Compiler.EditorServices.Structure+Scope: Boolean IsYieldOrReturnBang
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsArrayOrList()
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsAttribute()
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsComment()
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsCompExpr()
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsCompExprInternal()
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsDo()
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsElseInIfThenElse()
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsEnumCase()
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsFinallyInTryFinally()
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsFor()
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsHashDirective()
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsIfThenElse()
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsInterface()
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsLambda()
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsLetOrUse()
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsLetOrUseBang()
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsMatch()
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsMatchBang()
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsMatchClause()
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsMatchLambda()
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsMember()
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsModule()
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsNamespace()
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsNew()
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsObjExpr()
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsOpen()
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsQuote()
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsRecord()
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsRecordDefn()
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsRecordField()
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsSpecialFunc()
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsThenInIfThenElse()
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsTryFinally()
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsTryInTryFinally()
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsTryInTryWith()
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsTryWith()
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsTuple()
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsType()
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsTypeExtension()
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsUnionCase()
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsUnionDefn()
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsVal()
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsWhile()
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsWithInTryWith()
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsXmlDocComment()
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsYieldOrReturn()
FSharp.Compiler.EditorServices.Structure+Scope: Boolean get_IsYieldOrReturnBang()
FSharp.Compiler.EditorServices.Structure+Scope: FSharp.Compiler.EditorServices.Structure+Scope+Tags
FSharp.Compiler.EditorServices.Structure+Scope: Int32 CompareTo(Scope)
FSharp.Compiler.EditorServices.Structure+Scope: Int32 CompareTo(System.Object)
FSharp.Compiler.EditorServices.Structure+Scope: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.EditorServices.Structure+Scope: Int32 GetHashCode()
FSharp.Compiler.EditorServices.Structure+Scope: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.Structure+Scope: Int32 Tag
FSharp.Compiler.EditorServices.Structure+Scope: Int32 get_Tag()
FSharp.Compiler.EditorServices.Structure+Scope: Scope ArrayOrList
FSharp.Compiler.EditorServices.Structure+Scope: Scope Attribute
FSharp.Compiler.EditorServices.Structure+Scope: Scope Comment
FSharp.Compiler.EditorServices.Structure+Scope: Scope CompExpr
FSharp.Compiler.EditorServices.Structure+Scope: Scope CompExprInternal
FSharp.Compiler.EditorServices.Structure+Scope: Scope Do
FSharp.Compiler.EditorServices.Structure+Scope: Scope ElseInIfThenElse
FSharp.Compiler.EditorServices.Structure+Scope: Scope EnumCase
FSharp.Compiler.EditorServices.Structure+Scope: Scope FinallyInTryFinally
FSharp.Compiler.EditorServices.Structure+Scope: Scope For
FSharp.Compiler.EditorServices.Structure+Scope: Scope HashDirective
FSharp.Compiler.EditorServices.Structure+Scope: Scope IfThenElse
FSharp.Compiler.EditorServices.Structure+Scope: Scope Interface
FSharp.Compiler.EditorServices.Structure+Scope: Scope Lambda
FSharp.Compiler.EditorServices.Structure+Scope: Scope LetOrUse
FSharp.Compiler.EditorServices.Structure+Scope: Scope LetOrUseBang
FSharp.Compiler.EditorServices.Structure+Scope: Scope Match
FSharp.Compiler.EditorServices.Structure+Scope: Scope MatchBang
FSharp.Compiler.EditorServices.Structure+Scope: Scope MatchClause
FSharp.Compiler.EditorServices.Structure+Scope: Scope MatchLambda
FSharp.Compiler.EditorServices.Structure+Scope: Scope Member
FSharp.Compiler.EditorServices.Structure+Scope: Scope Module
FSharp.Compiler.EditorServices.Structure+Scope: Scope Namespace
FSharp.Compiler.EditorServices.Structure+Scope: Scope New
FSharp.Compiler.EditorServices.Structure+Scope: Scope ObjExpr
FSharp.Compiler.EditorServices.Structure+Scope: Scope Open
FSharp.Compiler.EditorServices.Structure+Scope: Scope Quote
FSharp.Compiler.EditorServices.Structure+Scope: Scope Record
FSharp.Compiler.EditorServices.Structure+Scope: Scope RecordDefn
FSharp.Compiler.EditorServices.Structure+Scope: Scope RecordField
FSharp.Compiler.EditorServices.Structure+Scope: Scope SpecialFunc
FSharp.Compiler.EditorServices.Structure+Scope: Scope ThenInIfThenElse
FSharp.Compiler.EditorServices.Structure+Scope: Scope TryFinally
FSharp.Compiler.EditorServices.Structure+Scope: Scope TryInTryFinally
FSharp.Compiler.EditorServices.Structure+Scope: Scope TryInTryWith
FSharp.Compiler.EditorServices.Structure+Scope: Scope TryWith
FSharp.Compiler.EditorServices.Structure+Scope: Scope Tuple
FSharp.Compiler.EditorServices.Structure+Scope: Scope Type
FSharp.Compiler.EditorServices.Structure+Scope: Scope TypeExtension
FSharp.Compiler.EditorServices.Structure+Scope: Scope UnionCase
FSharp.Compiler.EditorServices.Structure+Scope: Scope UnionDefn
FSharp.Compiler.EditorServices.Structure+Scope: Scope Val
FSharp.Compiler.EditorServices.Structure+Scope: Scope While
FSharp.Compiler.EditorServices.Structure+Scope: Scope WithInTryWith
FSharp.Compiler.EditorServices.Structure+Scope: Scope XmlDocComment
FSharp.Compiler.EditorServices.Structure+Scope: Scope YieldOrReturn
FSharp.Compiler.EditorServices.Structure+Scope: Scope YieldOrReturnBang
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_ArrayOrList()
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_Attribute()
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_Comment()
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_CompExpr()
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_CompExprInternal()
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_Do()
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_ElseInIfThenElse()
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_EnumCase()
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_FinallyInTryFinally()
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_For()
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_HashDirective()
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_IfThenElse()
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_Interface()
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_Lambda()
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_LetOrUse()
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_LetOrUseBang()
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_Match()
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_MatchBang()
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_MatchClause()
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_MatchLambda()
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_Member()
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_Module()
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_Namespace()
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_New()
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_ObjExpr()
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_Open()
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_Quote()
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_Record()
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_RecordDefn()
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_RecordField()
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_SpecialFunc()
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_ThenInIfThenElse()
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_TryFinally()
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_TryInTryFinally()
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_TryInTryWith()
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_TryWith()
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_Tuple()
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_Type()
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_TypeExtension()
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_UnionCase()
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_UnionDefn()
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_Val()
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_While()
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_WithInTryWith()
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_XmlDocComment()
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_YieldOrReturn()
FSharp.Compiler.EditorServices.Structure+Scope: Scope get_YieldOrReturnBang()
FSharp.Compiler.EditorServices.Structure+Scope: System.String ToString()
FSharp.Compiler.EditorServices.Structure+ScopeRange: Boolean Equals(ScopeRange)
FSharp.Compiler.EditorServices.Structure+ScopeRange: Boolean Equals(System.Object)
FSharp.Compiler.EditorServices.Structure+ScopeRange: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.Structure+ScopeRange: Collapse Collapse
FSharp.Compiler.EditorServices.Structure+ScopeRange: Collapse get_Collapse()
FSharp.Compiler.EditorServices.Structure+ScopeRange: FSharp.Compiler.Text.Range CollapseRange
FSharp.Compiler.EditorServices.Structure+ScopeRange: FSharp.Compiler.Text.Range Range
FSharp.Compiler.EditorServices.Structure+ScopeRange: FSharp.Compiler.Text.Range get_CollapseRange()
FSharp.Compiler.EditorServices.Structure+ScopeRange: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.EditorServices.Structure+ScopeRange: Int32 GetHashCode()
FSharp.Compiler.EditorServices.Structure+ScopeRange: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.Structure+ScopeRange: Scope Scope
FSharp.Compiler.EditorServices.Structure+ScopeRange: Scope get_Scope()
FSharp.Compiler.EditorServices.Structure+ScopeRange: System.String ToString()
FSharp.Compiler.EditorServices.Structure+ScopeRange: Void .ctor(Scope, Collapse, FSharp.Compiler.Text.Range, FSharp.Compiler.Text.Range)
FSharp.Compiler.EditorServices.Structure: FSharp.Compiler.EditorServices.Structure+Collapse
FSharp.Compiler.EditorServices.Structure: FSharp.Compiler.EditorServices.Structure+Scope
FSharp.Compiler.EditorServices.Structure: FSharp.Compiler.EditorServices.Structure+ScopeRange
FSharp.Compiler.EditorServices.Structure: System.Collections.Generic.IEnumerable`1[FSharp.Compiler.EditorServices.Structure+ScopeRange] getOutliningRanges(System.String[], FSharp.Compiler.Syntax.ParsedInput)
FSharp.Compiler.EditorServices.UnusedDeclarations
FSharp.Compiler.EditorServices.UnusedDeclarations: Microsoft.FSharp.Control.FSharpAsync`1[System.Collections.Generic.IEnumerable`1[FSharp.Compiler.Text.Range]] getUnusedDeclarations(FSharp.Compiler.CodeAnalysis.FSharpCheckFileResults, Boolean)
FSharp.Compiler.EditorServices.UnusedOpens
FSharp.Compiler.EditorServices.UnusedOpens: Microsoft.FSharp.Control.FSharpAsync`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Text.Range]] getUnusedOpens(FSharp.Compiler.CodeAnalysis.FSharpCheckFileResults, Microsoft.FSharp.Core.FSharpFunc`2[System.Int32,System.String])
FSharp.Compiler.EditorServices.XmlDocComment
FSharp.Compiler.EditorServices.XmlDocComment: Microsoft.FSharp.Core.FSharpOption`1[System.Int32] isBlank(System.String)
FSharp.Compiler.EditorServices.XmlDocParser
FSharp.Compiler.EditorServices.XmlDocParser: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.EditorServices.XmlDocable] getXmlDocables(FSharp.Compiler.Text.ISourceText, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.ParsedInput])
FSharp.Compiler.EditorServices.XmlDocable
FSharp.Compiler.EditorServices.XmlDocable: Boolean Equals(FSharp.Compiler.EditorServices.XmlDocable)
FSharp.Compiler.EditorServices.XmlDocable: Boolean Equals(System.Object)
FSharp.Compiler.EditorServices.XmlDocable: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.XmlDocable: FSharp.Compiler.EditorServices.XmlDocable NewXmlDocable(Int32, Int32, Microsoft.FSharp.Collections.FSharpList`1[System.String])
FSharp.Compiler.EditorServices.XmlDocable: Int32 CompareTo(FSharp.Compiler.EditorServices.XmlDocable)
FSharp.Compiler.EditorServices.XmlDocable: Int32 CompareTo(System.Object)
FSharp.Compiler.EditorServices.XmlDocable: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.EditorServices.XmlDocable: Int32 GetHashCode()
FSharp.Compiler.EditorServices.XmlDocable: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.EditorServices.XmlDocable: Int32 Tag
FSharp.Compiler.EditorServices.XmlDocable: Int32 get_Tag()
FSharp.Compiler.EditorServices.XmlDocable: Int32 get_indent()
FSharp.Compiler.EditorServices.XmlDocable: Int32 get_line()
FSharp.Compiler.EditorServices.XmlDocable: Int32 indent
FSharp.Compiler.EditorServices.XmlDocable: Int32 line
FSharp.Compiler.EditorServices.XmlDocable: Microsoft.FSharp.Collections.FSharpList`1[System.String] get_paramNames()
FSharp.Compiler.EditorServices.XmlDocable: Microsoft.FSharp.Collections.FSharpList`1[System.String] paramNames
FSharp.Compiler.EditorServices.XmlDocable: System.String ToString()
FSharp.Compiler.IO.DefaultFileSystem
FSharp.Compiler.IO.DefaultFileSystem: Void .ctor()
FSharp.Compiler.IO.FileSystemAutoOpens
FSharp.Compiler.IO.FileSystemAutoOpens: FSharp.Compiler.IO.IFileSystem FileSystem
FSharp.Compiler.IO.FileSystemAutoOpens: FSharp.Compiler.IO.IFileSystem get_FileSystem()
FSharp.Compiler.IO.FileSystemAutoOpens: Void set_FileSystem(FSharp.Compiler.IO.IFileSystem)
FSharp.Compiler.IO.IFileSystem
FSharp.Compiler.IO.IFileSystem: Boolean IsInvalidPathShim(System.String)
FSharp.Compiler.IO.IFileSystem: Boolean IsPathRootedShim(System.String)
FSharp.Compiler.IO.IFileSystem: Boolean IsStableFileHeuristic(System.String)
FSharp.Compiler.IO.IFileSystem: Boolean SafeExists(System.String)
FSharp.Compiler.IO.IFileSystem: Byte[] ReadAllBytesShim(System.String)
FSharp.Compiler.IO.IFileSystem: System.DateTime GetLastWriteTimeShim(System.String)
FSharp.Compiler.IO.IFileSystem: System.IO.Stream FileStreamCreateShim(System.String)
FSharp.Compiler.IO.IFileSystem: System.IO.Stream FileStreamReadShim(System.String)
FSharp.Compiler.IO.IFileSystem: System.IO.Stream FileStreamWriteExistingShim(System.String)
FSharp.Compiler.IO.IFileSystem: System.Reflection.Assembly AssemblyLoad(System.Reflection.AssemblyName)
FSharp.Compiler.IO.IFileSystem: System.Reflection.Assembly AssemblyLoadFrom(System.String)
FSharp.Compiler.IO.IFileSystem: System.String GetFullPathShim(System.String)
FSharp.Compiler.IO.IFileSystem: System.String GetTempPathShim()
FSharp.Compiler.IO.IFileSystem: Void FileDelete(System.String)
FSharp.Compiler.Interactive.Shell
FSharp.Compiler.Interactive.Shell+CompilerInputStream: Boolean CanRead
FSharp.Compiler.Interactive.Shell+CompilerInputStream: Boolean CanSeek
FSharp.Compiler.Interactive.Shell+CompilerInputStream: Boolean CanWrite
FSharp.Compiler.Interactive.Shell+CompilerInputStream: Boolean get_CanRead()
FSharp.Compiler.Interactive.Shell+CompilerInputStream: Boolean get_CanSeek()
FSharp.Compiler.Interactive.Shell+CompilerInputStream: Boolean get_CanWrite()
FSharp.Compiler.Interactive.Shell+CompilerInputStream: Int32 Read(Byte[], Int32, Int32)
FSharp.Compiler.Interactive.Shell+CompilerInputStream: Int64 Length
FSharp.Compiler.Interactive.Shell+CompilerInputStream: Int64 Position
FSharp.Compiler.Interactive.Shell+CompilerInputStream: Int64 Seek(Int64, System.IO.SeekOrigin)
FSharp.Compiler.Interactive.Shell+CompilerInputStream: Int64 get_Length()
FSharp.Compiler.Interactive.Shell+CompilerInputStream: Int64 get_Position()
FSharp.Compiler.Interactive.Shell+CompilerInputStream: Void .ctor()
FSharp.Compiler.Interactive.Shell+CompilerInputStream: Void Add(System.String)
FSharp.Compiler.Interactive.Shell+CompilerInputStream: Void Flush()
FSharp.Compiler.Interactive.Shell+CompilerInputStream: Void SetLength(Int64)
FSharp.Compiler.Interactive.Shell+CompilerInputStream: Void Write(Byte[], Int32, Int32)
FSharp.Compiler.Interactive.Shell+CompilerInputStream: Void set_Position(Int64)
FSharp.Compiler.Interactive.Shell+CompilerOutputStream: Boolean CanRead
FSharp.Compiler.Interactive.Shell+CompilerOutputStream: Boolean CanSeek
FSharp.Compiler.Interactive.Shell+CompilerOutputStream: Boolean CanWrite
FSharp.Compiler.Interactive.Shell+CompilerOutputStream: Boolean get_CanRead()
FSharp.Compiler.Interactive.Shell+CompilerOutputStream: Boolean get_CanSeek()
FSharp.Compiler.Interactive.Shell+CompilerOutputStream: Boolean get_CanWrite()
FSharp.Compiler.Interactive.Shell+CompilerOutputStream: Int32 Read(Byte[], Int32, Int32)
FSharp.Compiler.Interactive.Shell+CompilerOutputStream: Int64 Length
FSharp.Compiler.Interactive.Shell+CompilerOutputStream: Int64 Position
FSharp.Compiler.Interactive.Shell+CompilerOutputStream: Int64 Seek(Int64, System.IO.SeekOrigin)
FSharp.Compiler.Interactive.Shell+CompilerOutputStream: Int64 get_Length()
FSharp.Compiler.Interactive.Shell+CompilerOutputStream: Int64 get_Position()
FSharp.Compiler.Interactive.Shell+CompilerOutputStream: System.String Read()
FSharp.Compiler.Interactive.Shell+CompilerOutputStream: Void .ctor()
FSharp.Compiler.Interactive.Shell+CompilerOutputStream: Void Flush()
FSharp.Compiler.Interactive.Shell+CompilerOutputStream: Void SetLength(Int64)
FSharp.Compiler.Interactive.Shell+CompilerOutputStream: Void Write(Byte[], Int32, Int32)
FSharp.Compiler.Interactive.Shell+CompilerOutputStream: Void set_Position(Int64)
FSharp.Compiler.Interactive.Shell+EvaluationEventArgs: FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration ImplementationDeclaration
FSharp.Compiler.Interactive.Shell+EvaluationEventArgs: FSharp.Compiler.CodeAnalysis.FSharpImplementationFileDeclaration get_ImplementationDeclaration()
FSharp.Compiler.Interactive.Shell+EvaluationEventArgs: FSharp.Compiler.CodeAnalysis.FSharpSymbol Symbol
FSharp.Compiler.Interactive.Shell+EvaluationEventArgs: FSharp.Compiler.CodeAnalysis.FSharpSymbol get_Symbol()
FSharp.Compiler.Interactive.Shell+EvaluationEventArgs: FSharp.Compiler.CodeAnalysis.FSharpSymbolUse SymbolUse
FSharp.Compiler.Interactive.Shell+EvaluationEventArgs: FSharp.Compiler.CodeAnalysis.FSharpSymbolUse get_SymbolUse()
FSharp.Compiler.Interactive.Shell+EvaluationEventArgs: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Interactive.Shell+FsiValue] FsiValue
FSharp.Compiler.Interactive.Shell+EvaluationEventArgs: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Interactive.Shell+FsiValue] get_FsiValue()
FSharp.Compiler.Interactive.Shell+EvaluationEventArgs: System.String Name
FSharp.Compiler.Interactive.Shell+EvaluationEventArgs: System.String get_Name()
FSharp.Compiler.Interactive.Shell+FsiBoundValue: FsiValue Value
FSharp.Compiler.Interactive.Shell+FsiBoundValue: FsiValue get_Value()
FSharp.Compiler.Interactive.Shell+FsiBoundValue: System.String Name
FSharp.Compiler.Interactive.Shell+FsiBoundValue: System.String get_Name()
FSharp.Compiler.Interactive.Shell+FsiCompilationException: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Diagnostics.FSharpDiagnostic[]] ErrorInfos
FSharp.Compiler.Interactive.Shell+FsiCompilationException: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Diagnostics.FSharpDiagnostic[]] get_ErrorInfos()
FSharp.Compiler.Interactive.Shell+FsiCompilationException: Void .ctor(System.String, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Diagnostics.FSharpDiagnostic[]])
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: Boolean IsGui
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: Boolean get_IsGui()
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: FSharp.Compiler.CodeAnalysis.FSharpAssemblySignature CurrentPartialAssemblySignature
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: FSharp.Compiler.CodeAnalysis.FSharpAssemblySignature get_CurrentPartialAssemblySignature()
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: FSharp.Compiler.CodeAnalysis.FSharpChecker InteractiveChecker
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: FSharp.Compiler.CodeAnalysis.FSharpChecker get_InteractiveChecker()
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: FsiEvaluationSession Create(FsiEvaluationSessionHostConfig, System.String[], System.IO.TextReader, System.IO.TextWriter, System.IO.TextWriter, Microsoft.FSharp.Core.FSharpOption`1[System.Boolean], Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.CodeAnalysis.LegacyReferenceResolver])
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: FsiEvaluationSessionHostConfig GetDefaultConfiguration()
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: FsiEvaluationSessionHostConfig GetDefaultConfiguration(System.Object)
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: FsiEvaluationSessionHostConfig GetDefaultConfiguration(System.Object, Boolean)
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Interactive.Shell+FsiBoundValue] GetBoundValues()
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: Microsoft.FSharp.Control.FSharpAsync`1[System.Tuple`3[FSharp.Compiler.CodeAnalysis.FSharpParseFileResults,FSharp.Compiler.CodeAnalysis.FSharpCheckFileResults,FSharp.Compiler.CodeAnalysis.FSharpCheckProjectResults]] ParseAndCheckInteraction(System.String)
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: Microsoft.FSharp.Control.IEvent`2[Microsoft.FSharp.Control.FSharpHandler`1[Microsoft.FSharp.Core.Unit],Microsoft.FSharp.Core.Unit] PartialAssemblySignatureUpdated
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: Microsoft.FSharp.Control.IEvent`2[Microsoft.FSharp.Control.FSharpHandler`1[Microsoft.FSharp.Core.Unit],Microsoft.FSharp.Core.Unit] get_PartialAssemblySignatureUpdated()
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: Microsoft.FSharp.Control.IEvent`2[Microsoft.FSharp.Control.FSharpHandler`1[System.Tuple`3[System.Object,System.Type,System.String]],System.Tuple`3[System.Object,System.Type,System.String]] ValueBound
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: Microsoft.FSharp.Control.IEvent`2[Microsoft.FSharp.Control.FSharpHandler`1[System.Tuple`3[System.Object,System.Type,System.String]],System.Tuple`3[System.Object,System.Type,System.String]] get_ValueBound()
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Interactive.Shell+FsiBoundValue] TryFindBoundValue(System.String)
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Interactive.Shell+FsiValue] EvalExpression(System.String)
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: Microsoft.FSharp.Core.FSharpOption`1[System.Int32] LCID
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: Microsoft.FSharp.Core.FSharpOption`1[System.Int32] get_LCID()
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: System.Collections.Generic.IEnumerable`1[System.String] GetCompletions(System.String)
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: System.Reflection.Assembly DynamicAssembly
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: System.Reflection.Assembly get_DynamicAssembly()
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: System.String FormatValue(System.Object, System.Type)
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: System.Tuple`2[Microsoft.FSharp.Core.FSharpChoice`2[Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Interactive.Shell+FsiValue],System.Exception],FSharp.Compiler.Diagnostics.FSharpDiagnostic[]] EvalExpressionNonThrowing(System.String)
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: System.Tuple`2[Microsoft.FSharp.Core.FSharpChoice`2[Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Interactive.Shell+FsiValue],System.Exception],FSharp.Compiler.Diagnostics.FSharpDiagnostic[]] EvalInteractionNonThrowing(System.String, Microsoft.FSharp.Core.FSharpOption`1[System.Threading.CancellationToken])
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: System.Tuple`2[Microsoft.FSharp.Core.FSharpChoice`2[Microsoft.FSharp.Core.Unit,System.Exception],FSharp.Compiler.Diagnostics.FSharpDiagnostic[]] EvalScriptNonThrowing(System.String)
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: Void AddBoundValue(System.String, System.Object)
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: Void EvalInteraction(System.String, Microsoft.FSharp.Core.FSharpOption`1[System.Threading.CancellationToken])
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: Void EvalScript(System.String)
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: Void Interrupt()
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: Void ReportUnhandledException(System.Exception)
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: Void Run()
FSharp.Compiler.Interactive.Shell+FsiEvaluationSessionHostConfig: Boolean EventLoopRun()
FSharp.Compiler.Interactive.Shell+FsiEvaluationSessionHostConfig: Boolean ShowDeclarationValues
FSharp.Compiler.Interactive.Shell+FsiEvaluationSessionHostConfig: Boolean ShowIEnumerable
FSharp.Compiler.Interactive.Shell+FsiEvaluationSessionHostConfig: Boolean ShowProperties
FSharp.Compiler.Interactive.Shell+FsiEvaluationSessionHostConfig: Boolean UseFsiAuxLib
FSharp.Compiler.Interactive.Shell+FsiEvaluationSessionHostConfig: Boolean get_ShowDeclarationValues()
FSharp.Compiler.Interactive.Shell+FsiEvaluationSessionHostConfig: Boolean get_ShowIEnumerable()
FSharp.Compiler.Interactive.Shell+FsiEvaluationSessionHostConfig: Boolean get_ShowProperties()
FSharp.Compiler.Interactive.Shell+FsiEvaluationSessionHostConfig: Boolean get_UseFsiAuxLib()
FSharp.Compiler.Interactive.Shell+FsiEvaluationSessionHostConfig: Int32 PrintDepth
FSharp.Compiler.Interactive.Shell+FsiEvaluationSessionHostConfig: Int32 PrintLength
FSharp.Compiler.Interactive.Shell+FsiEvaluationSessionHostConfig: Int32 PrintSize
FSharp.Compiler.Interactive.Shell+FsiEvaluationSessionHostConfig: Int32 PrintWidth
FSharp.Compiler.Interactive.Shell+FsiEvaluationSessionHostConfig: Int32 get_PrintDepth()
FSharp.Compiler.Interactive.Shell+FsiEvaluationSessionHostConfig: Int32 get_PrintLength()
FSharp.Compiler.Interactive.Shell+FsiEvaluationSessionHostConfig: Int32 get_PrintSize()
FSharp.Compiler.Interactive.Shell+FsiEvaluationSessionHostConfig: Int32 get_PrintWidth()
FSharp.Compiler.Interactive.Shell+FsiEvaluationSessionHostConfig: Microsoft.FSharp.Collections.FSharpList`1[Microsoft.FSharp.Core.FSharpChoice`2[System.Tuple`2[System.Type,Microsoft.FSharp.Core.FSharpFunc`2[System.Object,System.String]],System.Tuple`2[System.Type,Microsoft.FSharp.Core.FSharpFunc`2[System.Object,System.Object]]]] AddedPrinters
FSharp.Compiler.Interactive.Shell+FsiEvaluationSessionHostConfig: Microsoft.FSharp.Collections.FSharpList`1[Microsoft.FSharp.Core.FSharpChoice`2[System.Tuple`2[System.Type,Microsoft.FSharp.Core.FSharpFunc`2[System.Object,System.String]],System.Tuple`2[System.Type,Microsoft.FSharp.Core.FSharpFunc`2[System.Object,System.Object]]]] get_AddedPrinters()
FSharp.Compiler.Interactive.Shell+FsiEvaluationSessionHostConfig: Microsoft.FSharp.Control.IEvent`2[Microsoft.FSharp.Control.FSharpHandler`1[FSharp.Compiler.Interactive.Shell+EvaluationEventArgs],FSharp.Compiler.Interactive.Shell+EvaluationEventArgs] OnEvaluation
FSharp.Compiler.Interactive.Shell+FsiEvaluationSessionHostConfig: Microsoft.FSharp.Control.IEvent`2[Microsoft.FSharp.Control.FSharpHandler`1[FSharp.Compiler.Interactive.Shell+EvaluationEventArgs],FSharp.Compiler.Interactive.Shell+EvaluationEventArgs] get_OnEvaluation()
FSharp.Compiler.Interactive.Shell+FsiEvaluationSessionHostConfig: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.FSharpFunc`2[Microsoft.FSharp.Core.Unit,System.String]] GetOptionalConsoleReadLine(Boolean)
FSharp.Compiler.Interactive.Shell+FsiEvaluationSessionHostConfig: System.IFormatProvider FormatProvider
FSharp.Compiler.Interactive.Shell+FsiEvaluationSessionHostConfig: System.IFormatProvider get_FormatProvider()
FSharp.Compiler.Interactive.Shell+FsiEvaluationSessionHostConfig: System.String FloatingPointFormat
FSharp.Compiler.Interactive.Shell+FsiEvaluationSessionHostConfig: System.String get_FloatingPointFormat()
FSharp.Compiler.Interactive.Shell+FsiEvaluationSessionHostConfig: T EventLoopInvoke[T](Microsoft.FSharp.Core.FSharpFunc`2[Microsoft.FSharp.Core.Unit,T])
FSharp.Compiler.Interactive.Shell+FsiEvaluationSessionHostConfig: Void .ctor()
FSharp.Compiler.Interactive.Shell+FsiEvaluationSessionHostConfig: Void EventLoopScheduleRestart()
FSharp.Compiler.Interactive.Shell+FsiEvaluationSessionHostConfig: Void ReportUserCommandLineArgs(System.String[])
FSharp.Compiler.Interactive.Shell+FsiEvaluationSessionHostConfig: Void StartServer(System.String)
FSharp.Compiler.Interactive.Shell+FsiValue: System.Object ReflectionValue
FSharp.Compiler.Interactive.Shell+FsiValue: System.Object get_ReflectionValue()
FSharp.Compiler.Interactive.Shell+FsiValue: System.Type ReflectionType
FSharp.Compiler.Interactive.Shell+FsiValue: System.Type get_ReflectionType()
FSharp.Compiler.Interactive.Shell+Settings+IEventLoop: Boolean Run()
FSharp.Compiler.Interactive.Shell+Settings+IEventLoop: T Invoke[T](Microsoft.FSharp.Core.FSharpFunc`2[Microsoft.FSharp.Core.Unit,T])
FSharp.Compiler.Interactive.Shell+Settings+IEventLoop: Void ScheduleRestart()
FSharp.Compiler.Interactive.Shell+Settings+InteractiveSettings: Boolean ShowDeclarationValues
FSharp.Compiler.Interactive.Shell+Settings+InteractiveSettings: Boolean ShowIEnumerable
FSharp.Compiler.Interactive.Shell+Settings+InteractiveSettings: Boolean ShowProperties
FSharp.Compiler.Interactive.Shell+Settings+InteractiveSettings: Boolean get_ShowDeclarationValues()
FSharp.Compiler.Interactive.Shell+Settings+InteractiveSettings: Boolean get_ShowIEnumerable()
FSharp.Compiler.Interactive.Shell+Settings+InteractiveSettings: Boolean get_ShowProperties()
FSharp.Compiler.Interactive.Shell+Settings+InteractiveSettings: IEventLoop EventLoop
FSharp.Compiler.Interactive.Shell+Settings+InteractiveSettings: IEventLoop get_EventLoop()
FSharp.Compiler.Interactive.Shell+Settings+InteractiveSettings: Int32 PrintDepth
FSharp.Compiler.Interactive.Shell+Settings+InteractiveSettings: Int32 PrintLength
FSharp.Compiler.Interactive.Shell+Settings+InteractiveSettings: Int32 PrintSize
FSharp.Compiler.Interactive.Shell+Settings+InteractiveSettings: Int32 PrintWidth
FSharp.Compiler.Interactive.Shell+Settings+InteractiveSettings: Int32 get_PrintDepth()
FSharp.Compiler.Interactive.Shell+Settings+InteractiveSettings: Int32 get_PrintLength()
FSharp.Compiler.Interactive.Shell+Settings+InteractiveSettings: Int32 get_PrintSize()
FSharp.Compiler.Interactive.Shell+Settings+InteractiveSettings: Int32 get_PrintWidth()
FSharp.Compiler.Interactive.Shell+Settings+InteractiveSettings: System.IFormatProvider FormatProvider
FSharp.Compiler.Interactive.Shell+Settings+InteractiveSettings: System.IFormatProvider get_FormatProvider()
FSharp.Compiler.Interactive.Shell+Settings+InteractiveSettings: System.String FloatingPointFormat
FSharp.Compiler.Interactive.Shell+Settings+InteractiveSettings: System.String get_FloatingPointFormat()
FSharp.Compiler.Interactive.Shell+Settings+InteractiveSettings: System.String[] CommandLineArgs
FSharp.Compiler.Interactive.Shell+Settings+InteractiveSettings: System.String[] get_CommandLineArgs()
FSharp.Compiler.Interactive.Shell+Settings+InteractiveSettings: Void AddPrintTransformer[T](Microsoft.FSharp.Core.FSharpFunc`2[T,System.Object])
FSharp.Compiler.Interactive.Shell+Settings+InteractiveSettings: Void AddPrinter[T](Microsoft.FSharp.Core.FSharpFunc`2[T,System.String])
FSharp.Compiler.Interactive.Shell+Settings+InteractiveSettings: Void set_CommandLineArgs(System.String[])
FSharp.Compiler.Interactive.Shell+Settings+InteractiveSettings: Void set_EventLoop(IEventLoop)
FSharp.Compiler.Interactive.Shell+Settings+InteractiveSettings: Void set_FloatingPointFormat(System.String)
FSharp.Compiler.Interactive.Shell+Settings+InteractiveSettings: Void set_FormatProvider(System.IFormatProvider)
FSharp.Compiler.Interactive.Shell+Settings+InteractiveSettings: Void set_PrintDepth(Int32)
FSharp.Compiler.Interactive.Shell+Settings+InteractiveSettings: Void set_PrintLength(Int32)
FSharp.Compiler.Interactive.Shell+Settings+InteractiveSettings: Void set_PrintSize(Int32)
FSharp.Compiler.Interactive.Shell+Settings+InteractiveSettings: Void set_PrintWidth(Int32)
FSharp.Compiler.Interactive.Shell+Settings+InteractiveSettings: Void set_ShowDeclarationValues(Boolean)
FSharp.Compiler.Interactive.Shell+Settings+InteractiveSettings: Void set_ShowIEnumerable(Boolean)
FSharp.Compiler.Interactive.Shell+Settings+InteractiveSettings: Void set_ShowProperties(Boolean)
FSharp.Compiler.Interactive.Shell+Settings: FSharp.Compiler.Interactive.Shell+Settings+IEventLoop
FSharp.Compiler.Interactive.Shell+Settings: FSharp.Compiler.Interactive.Shell+Settings+InteractiveSettings
FSharp.Compiler.Interactive.Shell+Settings: InteractiveSettings fsi
FSharp.Compiler.Interactive.Shell+Settings: InteractiveSettings get_fsi()
FSharp.Compiler.Interactive.Shell: FSharp.Compiler.Interactive.Shell+CompilerInputStream
FSharp.Compiler.Interactive.Shell: FSharp.Compiler.Interactive.Shell+CompilerOutputStream
FSharp.Compiler.Interactive.Shell: FSharp.Compiler.Interactive.Shell+EvaluationEventArgs
FSharp.Compiler.Interactive.Shell: FSharp.Compiler.Interactive.Shell+FsiBoundValue
FSharp.Compiler.Interactive.Shell: FSharp.Compiler.Interactive.Shell+FsiCompilationException
FSharp.Compiler.Interactive.Shell: FSharp.Compiler.Interactive.Shell+FsiEvaluationSession
FSharp.Compiler.Interactive.Shell: FSharp.Compiler.Interactive.Shell+FsiEvaluationSessionHostConfig
FSharp.Compiler.Interactive.Shell: FSharp.Compiler.Interactive.Shell+FsiValue
FSharp.Compiler.Interactive.Shell: FSharp.Compiler.Interactive.Shell+Settings
FSharp.Compiler.Syntax.DebugPointAtFinally
FSharp.Compiler.Syntax.DebugPointAtFinally+Tags: Int32 No
FSharp.Compiler.Syntax.DebugPointAtFinally+Tags: Int32 Yes
FSharp.Compiler.Syntax.DebugPointAtFinally+Yes: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.DebugPointAtFinally+Yes: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.DebugPointAtFinally: Boolean Equals(FSharp.Compiler.Syntax.DebugPointAtFinally)
FSharp.Compiler.Syntax.DebugPointAtFinally: Boolean Equals(System.Object)
FSharp.Compiler.Syntax.DebugPointAtFinally: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.Syntax.DebugPointAtFinally: Boolean IsNo
FSharp.Compiler.Syntax.DebugPointAtFinally: Boolean IsYes
FSharp.Compiler.Syntax.DebugPointAtFinally: Boolean get_IsNo()
FSharp.Compiler.Syntax.DebugPointAtFinally: Boolean get_IsYes()
FSharp.Compiler.Syntax.DebugPointAtFinally: FSharp.Compiler.Syntax.DebugPointAtFinally NewYes(FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.DebugPointAtFinally: FSharp.Compiler.Syntax.DebugPointAtFinally No
FSharp.Compiler.Syntax.DebugPointAtFinally: FSharp.Compiler.Syntax.DebugPointAtFinally get_No()
FSharp.Compiler.Syntax.DebugPointAtFinally: FSharp.Compiler.Syntax.DebugPointAtFinally+Tags
FSharp.Compiler.Syntax.DebugPointAtFinally: FSharp.Compiler.Syntax.DebugPointAtFinally+Yes
FSharp.Compiler.Syntax.DebugPointAtFinally: Int32 GetHashCode()
FSharp.Compiler.Syntax.DebugPointAtFinally: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.Syntax.DebugPointAtFinally: Int32 Tag
FSharp.Compiler.Syntax.DebugPointAtFinally: Int32 get_Tag()
FSharp.Compiler.Syntax.DebugPointAtFinally: System.String ToString()
FSharp.Compiler.Syntax.DebugPointAtFor
FSharp.Compiler.Syntax.DebugPointAtFor+Tags: Int32 No
FSharp.Compiler.Syntax.DebugPointAtFor+Tags: Int32 Yes
FSharp.Compiler.Syntax.DebugPointAtFor+Yes: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.DebugPointAtFor+Yes: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.DebugPointAtFor: Boolean Equals(FSharp.Compiler.Syntax.DebugPointAtFor)
FSharp.Compiler.Syntax.DebugPointAtFor: Boolean Equals(System.Object)
FSharp.Compiler.Syntax.DebugPointAtFor: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.Syntax.DebugPointAtFor: Boolean IsNo
FSharp.Compiler.Syntax.DebugPointAtFor: Boolean IsYes
FSharp.Compiler.Syntax.DebugPointAtFor: Boolean get_IsNo()
FSharp.Compiler.Syntax.DebugPointAtFor: Boolean get_IsYes()
FSharp.Compiler.Syntax.DebugPointAtFor: FSharp.Compiler.Syntax.DebugPointAtFor NewYes(FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.DebugPointAtFor: FSharp.Compiler.Syntax.DebugPointAtFor No
FSharp.Compiler.Syntax.DebugPointAtFor: FSharp.Compiler.Syntax.DebugPointAtFor get_No()
FSharp.Compiler.Syntax.DebugPointAtFor: FSharp.Compiler.Syntax.DebugPointAtFor+Tags
FSharp.Compiler.Syntax.DebugPointAtFor: FSharp.Compiler.Syntax.DebugPointAtFor+Yes
FSharp.Compiler.Syntax.DebugPointAtFor: Int32 GetHashCode()
FSharp.Compiler.Syntax.DebugPointAtFor: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.Syntax.DebugPointAtFor: Int32 Tag
FSharp.Compiler.Syntax.DebugPointAtFor: Int32 get_Tag()
FSharp.Compiler.Syntax.DebugPointAtFor: System.String ToString()
FSharp.Compiler.Syntax.DebugPointAtSequential
FSharp.Compiler.Syntax.DebugPointAtSequential+Tags: Int32 Both
FSharp.Compiler.Syntax.DebugPointAtSequential+Tags: Int32 ExprOnly
FSharp.Compiler.Syntax.DebugPointAtSequential+Tags: Int32 StmtOnly
FSharp.Compiler.Syntax.DebugPointAtSequential: Boolean Equals(FSharp.Compiler.Syntax.DebugPointAtSequential)
FSharp.Compiler.Syntax.DebugPointAtSequential: Boolean Equals(System.Object)
FSharp.Compiler.Syntax.DebugPointAtSequential: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.Syntax.DebugPointAtSequential: Boolean IsBoth
FSharp.Compiler.Syntax.DebugPointAtSequential: Boolean IsExprOnly
FSharp.Compiler.Syntax.DebugPointAtSequential: Boolean IsStmtOnly
FSharp.Compiler.Syntax.DebugPointAtSequential: Boolean get_IsBoth()
FSharp.Compiler.Syntax.DebugPointAtSequential: Boolean get_IsExprOnly()
FSharp.Compiler.Syntax.DebugPointAtSequential: Boolean get_IsStmtOnly()
FSharp.Compiler.Syntax.DebugPointAtSequential: FSharp.Compiler.Syntax.DebugPointAtSequential Both
FSharp.Compiler.Syntax.DebugPointAtSequential: FSharp.Compiler.Syntax.DebugPointAtSequential ExprOnly
FSharp.Compiler.Syntax.DebugPointAtSequential: FSharp.Compiler.Syntax.DebugPointAtSequential StmtOnly
FSharp.Compiler.Syntax.DebugPointAtSequential: FSharp.Compiler.Syntax.DebugPointAtSequential get_Both()
FSharp.Compiler.Syntax.DebugPointAtSequential: FSharp.Compiler.Syntax.DebugPointAtSequential get_ExprOnly()
FSharp.Compiler.Syntax.DebugPointAtSequential: FSharp.Compiler.Syntax.DebugPointAtSequential get_StmtOnly()
FSharp.Compiler.Syntax.DebugPointAtSequential: FSharp.Compiler.Syntax.DebugPointAtSequential+Tags
FSharp.Compiler.Syntax.DebugPointAtSequential: Int32 CompareTo(FSharp.Compiler.Syntax.DebugPointAtSequential)
FSharp.Compiler.Syntax.DebugPointAtSequential: Int32 CompareTo(System.Object)
FSharp.Compiler.Syntax.DebugPointAtSequential: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.Syntax.DebugPointAtSequential: Int32 GetHashCode()
FSharp.Compiler.Syntax.DebugPointAtSequential: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.Syntax.DebugPointAtSequential: Int32 Tag
FSharp.Compiler.Syntax.DebugPointAtSequential: Int32 get_Tag()
FSharp.Compiler.Syntax.DebugPointAtSequential: System.String ToString()
FSharp.Compiler.Syntax.DebugPointAtTry
FSharp.Compiler.Syntax.DebugPointAtTry+Tags: Int32 Body
FSharp.Compiler.Syntax.DebugPointAtTry+Tags: Int32 No
FSharp.Compiler.Syntax.DebugPointAtTry+Tags: Int32 Yes
FSharp.Compiler.Syntax.DebugPointAtTry+Yes: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.DebugPointAtTry+Yes: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.DebugPointAtTry: Boolean Equals(FSharp.Compiler.Syntax.DebugPointAtTry)
FSharp.Compiler.Syntax.DebugPointAtTry: Boolean Equals(System.Object)
FSharp.Compiler.Syntax.DebugPointAtTry: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.Syntax.DebugPointAtTry: Boolean IsBody
FSharp.Compiler.Syntax.DebugPointAtTry: Boolean IsNo
FSharp.Compiler.Syntax.DebugPointAtTry: Boolean IsYes
FSharp.Compiler.Syntax.DebugPointAtTry: Boolean get_IsBody()
FSharp.Compiler.Syntax.DebugPointAtTry: Boolean get_IsNo()
FSharp.Compiler.Syntax.DebugPointAtTry: Boolean get_IsYes()
FSharp.Compiler.Syntax.DebugPointAtTry: FSharp.Compiler.Syntax.DebugPointAtTry Body
FSharp.Compiler.Syntax.DebugPointAtTry: FSharp.Compiler.Syntax.DebugPointAtTry NewYes(FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.DebugPointAtTry: FSharp.Compiler.Syntax.DebugPointAtTry No
FSharp.Compiler.Syntax.DebugPointAtTry: FSharp.Compiler.Syntax.DebugPointAtTry get_Body()
FSharp.Compiler.Syntax.DebugPointAtTry: FSharp.Compiler.Syntax.DebugPointAtTry get_No()
FSharp.Compiler.Syntax.DebugPointAtTry: FSharp.Compiler.Syntax.DebugPointAtTry+Tags
FSharp.Compiler.Syntax.DebugPointAtTry: FSharp.Compiler.Syntax.DebugPointAtTry+Yes
FSharp.Compiler.Syntax.DebugPointAtTry: Int32 GetHashCode()
FSharp.Compiler.Syntax.DebugPointAtTry: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.Syntax.DebugPointAtTry: Int32 Tag
FSharp.Compiler.Syntax.DebugPointAtTry: Int32 get_Tag()
FSharp.Compiler.Syntax.DebugPointAtTry: System.String ToString()
FSharp.Compiler.Syntax.DebugPointAtWhile
FSharp.Compiler.Syntax.DebugPointAtWhile+Tags: Int32 No
FSharp.Compiler.Syntax.DebugPointAtWhile+Tags: Int32 Yes
FSharp.Compiler.Syntax.DebugPointAtWhile+Yes: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.DebugPointAtWhile+Yes: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.DebugPointAtWhile: Boolean Equals(FSharp.Compiler.Syntax.DebugPointAtWhile)
FSharp.Compiler.Syntax.DebugPointAtWhile: Boolean Equals(System.Object)
FSharp.Compiler.Syntax.DebugPointAtWhile: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.Syntax.DebugPointAtWhile: Boolean IsNo
FSharp.Compiler.Syntax.DebugPointAtWhile: Boolean IsYes
FSharp.Compiler.Syntax.DebugPointAtWhile: Boolean get_IsNo()
FSharp.Compiler.Syntax.DebugPointAtWhile: Boolean get_IsYes()
FSharp.Compiler.Syntax.DebugPointAtWhile: FSharp.Compiler.Syntax.DebugPointAtWhile NewYes(FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.DebugPointAtWhile: FSharp.Compiler.Syntax.DebugPointAtWhile No
FSharp.Compiler.Syntax.DebugPointAtWhile: FSharp.Compiler.Syntax.DebugPointAtWhile get_No()
FSharp.Compiler.Syntax.DebugPointAtWhile: FSharp.Compiler.Syntax.DebugPointAtWhile+Tags
FSharp.Compiler.Syntax.DebugPointAtWhile: FSharp.Compiler.Syntax.DebugPointAtWhile+Yes
FSharp.Compiler.Syntax.DebugPointAtWhile: Int32 GetHashCode()
FSharp.Compiler.Syntax.DebugPointAtWhile: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.Syntax.DebugPointAtWhile: Int32 Tag
FSharp.Compiler.Syntax.DebugPointAtWhile: Int32 get_Tag()
FSharp.Compiler.Syntax.DebugPointAtWhile: System.String ToString()
FSharp.Compiler.Syntax.DebugPointAtWith
FSharp.Compiler.Syntax.DebugPointAtWith+Tags: Int32 No
FSharp.Compiler.Syntax.DebugPointAtWith+Tags: Int32 Yes
FSharp.Compiler.Syntax.DebugPointAtWith+Yes: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.DebugPointAtWith+Yes: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.DebugPointAtWith: Boolean Equals(FSharp.Compiler.Syntax.DebugPointAtWith)
FSharp.Compiler.Syntax.DebugPointAtWith: Boolean Equals(System.Object)
FSharp.Compiler.Syntax.DebugPointAtWith: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.Syntax.DebugPointAtWith: Boolean IsNo
FSharp.Compiler.Syntax.DebugPointAtWith: Boolean IsYes
FSharp.Compiler.Syntax.DebugPointAtWith: Boolean get_IsNo()
FSharp.Compiler.Syntax.DebugPointAtWith: Boolean get_IsYes()
FSharp.Compiler.Syntax.DebugPointAtWith: FSharp.Compiler.Syntax.DebugPointAtWith NewYes(FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.DebugPointAtWith: FSharp.Compiler.Syntax.DebugPointAtWith No
FSharp.Compiler.Syntax.DebugPointAtWith: FSharp.Compiler.Syntax.DebugPointAtWith get_No()
FSharp.Compiler.Syntax.DebugPointAtWith: FSharp.Compiler.Syntax.DebugPointAtWith+Tags
FSharp.Compiler.Syntax.DebugPointAtWith: FSharp.Compiler.Syntax.DebugPointAtWith+Yes
FSharp.Compiler.Syntax.DebugPointAtWith: Int32 GetHashCode()
FSharp.Compiler.Syntax.DebugPointAtWith: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.Syntax.DebugPointAtWith: Int32 Tag
FSharp.Compiler.Syntax.DebugPointAtWith: Int32 get_Tag()
FSharp.Compiler.Syntax.DebugPointAtWith: System.String ToString()
FSharp.Compiler.Syntax.DebugPointAtBinding
FSharp.Compiler.Syntax.DebugPointAtBinding+DebugPointAtBinding.Yes: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.DebugPointAtBinding+DebugPointAtBinding.Yes: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.DebugPointAtBinding+Tags: Int32 DebugPointAtBinding.Yes
FSharp.Compiler.Syntax.DebugPointAtBinding+Tags: Int32 DebugPointAtBinding.NoneAtDo
FSharp.Compiler.Syntax.DebugPointAtBinding+Tags: Int32 DebugPointAtBinding.NoneAtInvisible
FSharp.Compiler.Syntax.DebugPointAtBinding+Tags: Int32 DebugPointAtBinding.NoneAtLet
FSharp.Compiler.Syntax.DebugPointAtBinding+Tags: Int32 DebugPointAtBinding.NoneAtSticky
FSharp.Compiler.Syntax.DebugPointAtBinding: Boolean Equals(FSharp.Compiler.Syntax.DebugPointAtBinding)
FSharp.Compiler.Syntax.DebugPointAtBinding: Boolean Equals(System.Object)
FSharp.Compiler.Syntax.DebugPointAtBinding: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.Syntax.DebugPointAtBinding: Boolean IsDebugPointAtBinding
FSharp.Compiler.Syntax.DebugPointAtBinding: Boolean IsNoDebugPointAtDoBinding
FSharp.Compiler.Syntax.DebugPointAtBinding: Boolean IsNoDebugPointAtInvisibleBinding
FSharp.Compiler.Syntax.DebugPointAtBinding: Boolean IsNoDebugPointAtLetBinding
FSharp.Compiler.Syntax.DebugPointAtBinding: Boolean IsNoDebugPointAtStickyBinding
FSharp.Compiler.Syntax.DebugPointAtBinding: Boolean get_IsDebugPointAtBinding()
FSharp.Compiler.Syntax.DebugPointAtBinding: Boolean get_IsNoDebugPointAtDoBinding()
FSharp.Compiler.Syntax.DebugPointAtBinding: Boolean get_IsNoDebugPointAtInvisibleBinding()
FSharp.Compiler.Syntax.DebugPointAtBinding: Boolean get_IsNoDebugPointAtLetBinding()
FSharp.Compiler.Syntax.DebugPointAtBinding: Boolean get_IsNoDebugPointAtStickyBinding()
FSharp.Compiler.Syntax.DebugPointAtBinding: FSharp.Compiler.Syntax.DebugPointAtBinding Combine(FSharp.Compiler.Syntax.DebugPointAtBinding)
FSharp.Compiler.Syntax.DebugPointAtBinding: FSharp.Compiler.Syntax.DebugPointAtBinding NewDebugPointAtBinding(FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.DebugPointAtBinding: FSharp.Compiler.Syntax.DebugPointAtBinding DebugPointAtBinding.NoneAtDo
FSharp.Compiler.Syntax.DebugPointAtBinding: FSharp.Compiler.Syntax.DebugPointAtBinding DebugPointAtBinding.NoneAtInvisible
FSharp.Compiler.Syntax.DebugPointAtBinding: FSharp.Compiler.Syntax.DebugPointAtBinding DebugPointAtBinding.NoneAtLet
FSharp.Compiler.Syntax.DebugPointAtBinding: FSharp.Compiler.Syntax.DebugPointAtBinding DebugPointAtBinding.NoneAtSticky
FSharp.Compiler.Syntax.DebugPointAtBinding: FSharp.Compiler.Syntax.DebugPointAtBinding get_NoDebugPointAtDoBinding()
FSharp.Compiler.Syntax.DebugPointAtBinding: FSharp.Compiler.Syntax.DebugPointAtBinding get_NoDebugPointAtInvisibleBinding()
FSharp.Compiler.Syntax.DebugPointAtBinding: FSharp.Compiler.Syntax.DebugPointAtBinding get_NoDebugPointAtLetBinding()
FSharp.Compiler.Syntax.DebugPointAtBinding: FSharp.Compiler.Syntax.DebugPointAtBinding get_NoDebugPointAtStickyBinding()
FSharp.Compiler.Syntax.DebugPointAtBinding: FSharp.Compiler.Syntax.DebugPointAtBinding+DebugPointAtBinding.Yes
FSharp.Compiler.Syntax.DebugPointAtBinding: FSharp.Compiler.Syntax.DebugPointAtBinding+Tags
FSharp.Compiler.Syntax.DebugPointAtBinding: Int32 GetHashCode()
FSharp.Compiler.Syntax.DebugPointAtBinding: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.Syntax.DebugPointAtBinding: Int32 Tag
FSharp.Compiler.Syntax.DebugPointAtBinding: Int32 get_Tag()
FSharp.Compiler.Syntax.DebugPointAtBinding: System.String ToString()
FSharp.Compiler.Syntax.DebugPointForTarget
FSharp.Compiler.Syntax.DebugPointForTarget+Tags: Int32 No
FSharp.Compiler.Syntax.DebugPointForTarget+Tags: Int32 Yes
FSharp.Compiler.Syntax.DebugPointForTarget: Boolean Equals(FSharp.Compiler.Syntax.DebugPointForTarget)
FSharp.Compiler.Syntax.DebugPointForTarget: Boolean Equals(System.Object)
FSharp.Compiler.Syntax.DebugPointForTarget: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.Syntax.DebugPointForTarget: Boolean IsNo
FSharp.Compiler.Syntax.DebugPointForTarget: Boolean IsYes
FSharp.Compiler.Syntax.DebugPointForTarget: Boolean get_IsNo()
FSharp.Compiler.Syntax.DebugPointForTarget: Boolean get_IsYes()
FSharp.Compiler.Syntax.DebugPointForTarget: FSharp.Compiler.Syntax.DebugPointForTarget No
FSharp.Compiler.Syntax.DebugPointForTarget: FSharp.Compiler.Syntax.DebugPointForTarget Yes
FSharp.Compiler.Syntax.DebugPointForTarget: FSharp.Compiler.Syntax.DebugPointForTarget get_No()
FSharp.Compiler.Syntax.DebugPointForTarget: FSharp.Compiler.Syntax.DebugPointForTarget get_Yes()
FSharp.Compiler.Syntax.DebugPointForTarget: FSharp.Compiler.Syntax.DebugPointForTarget+Tags
FSharp.Compiler.Syntax.DebugPointForTarget: Int32 CompareTo(FSharp.Compiler.Syntax.DebugPointForTarget)
FSharp.Compiler.Syntax.DebugPointForTarget: Int32 CompareTo(System.Object)
FSharp.Compiler.Syntax.DebugPointForTarget: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.Syntax.DebugPointForTarget: Int32 GetHashCode()
FSharp.Compiler.Syntax.DebugPointForTarget: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.Syntax.DebugPointForTarget: Int32 Tag
FSharp.Compiler.Syntax.DebugPointForTarget: Int32 get_Tag()
FSharp.Compiler.Syntax.DebugPointForTarget: System.String ToString()
FSharp.Compiler.Syntax.ExprAtomicFlag
FSharp.Compiler.Syntax.ExprAtomicFlag: FSharp.Compiler.Syntax.ExprAtomicFlag Atomic
FSharp.Compiler.Syntax.ExprAtomicFlag: FSharp.Compiler.Syntax.ExprAtomicFlag NonAtomic
FSharp.Compiler.Syntax.ExprAtomicFlag: Int32 value__
FSharp.Compiler.Syntax.Ident
FSharp.Compiler.Syntax.Ident: FSharp.Compiler.Text.Range get_idRange()
FSharp.Compiler.Syntax.Ident: FSharp.Compiler.Text.Range idRange
FSharp.Compiler.Syntax.Ident: System.String ToString()
FSharp.Compiler.Syntax.Ident: System.String get_idText()
FSharp.Compiler.Syntax.Ident: System.String idText
FSharp.Compiler.Syntax.Ident: Void .ctor(System.String, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.LongIdentWithDots
FSharp.Compiler.Syntax.LongIdentWithDots: Boolean ThereIsAnExtraDotAtTheEnd
FSharp.Compiler.Syntax.LongIdentWithDots: Boolean get_ThereIsAnExtraDotAtTheEnd()
FSharp.Compiler.Syntax.LongIdentWithDots: FSharp.Compiler.Syntax.LongIdentWithDots NewLongIdentWithDots(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Text.Range])
FSharp.Compiler.Syntax.LongIdentWithDots: FSharp.Compiler.Text.Range Range
FSharp.Compiler.Syntax.LongIdentWithDots: FSharp.Compiler.Text.Range RangeSansAnyExtraDot
FSharp.Compiler.Syntax.LongIdentWithDots: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.Syntax.LongIdentWithDots: FSharp.Compiler.Text.Range get_RangeSansAnyExtraDot()
FSharp.Compiler.Syntax.LongIdentWithDots: Int32 Tag
FSharp.Compiler.Syntax.LongIdentWithDots: Int32 get_Tag()
FSharp.Compiler.Syntax.LongIdentWithDots: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident] Lid
FSharp.Compiler.Syntax.LongIdentWithDots: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident] get_Lid()
FSharp.Compiler.Syntax.LongIdentWithDots: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident] get_id()
FSharp.Compiler.Syntax.LongIdentWithDots: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident] id
FSharp.Compiler.Syntax.LongIdentWithDots: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Text.Range] dotms
FSharp.Compiler.Syntax.LongIdentWithDots: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Text.Range] get_dotms()
FSharp.Compiler.Syntax.LongIdentWithDots: System.String ToString()
FSharp.Compiler.Syntax.MemberFlags
FSharp.Compiler.Syntax.MemberFlags: Boolean Equals(FSharp.Compiler.Syntax.MemberFlags)
FSharp.Compiler.Syntax.MemberFlags: Boolean Equals(System.Object)
FSharp.Compiler.Syntax.MemberFlags: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.Syntax.MemberFlags: Boolean IsDispatchSlot
FSharp.Compiler.Syntax.MemberFlags: Boolean IsFinal
FSharp.Compiler.Syntax.MemberFlags: Boolean IsInstance
FSharp.Compiler.Syntax.MemberFlags: Boolean IsOverrideOrExplicitImpl
FSharp.Compiler.Syntax.MemberFlags: Boolean get_IsDispatchSlot()
FSharp.Compiler.Syntax.MemberFlags: Boolean get_IsFinal()
FSharp.Compiler.Syntax.MemberFlags: Boolean get_IsInstance()
FSharp.Compiler.Syntax.MemberFlags: Boolean get_IsOverrideOrExplicitImpl()
FSharp.Compiler.Syntax.MemberFlags: FSharp.Compiler.Syntax.MemberKind MemberKind
FSharp.Compiler.Syntax.MemberFlags: FSharp.Compiler.Syntax.MemberKind get_MemberKind()
FSharp.Compiler.Syntax.MemberFlags: Int32 GetHashCode()
FSharp.Compiler.Syntax.MemberFlags: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.Syntax.MemberFlags: System.String ToString()
FSharp.Compiler.Syntax.MemberFlags: Void .ctor(Boolean, Boolean, Boolean, Boolean, FSharp.Compiler.Syntax.MemberKind)
FSharp.Compiler.Syntax.MemberKind
FSharp.Compiler.Syntax.MemberKind+Tags: Int32 ClassConstructor
FSharp.Compiler.Syntax.MemberKind+Tags: Int32 Constructor
FSharp.Compiler.Syntax.MemberKind+Tags: Int32 Member
FSharp.Compiler.Syntax.MemberKind+Tags: Int32 PropertyGet
FSharp.Compiler.Syntax.MemberKind+Tags: Int32 PropertyGetSet
FSharp.Compiler.Syntax.MemberKind+Tags: Int32 PropertySet
FSharp.Compiler.Syntax.MemberKind: Boolean Equals(FSharp.Compiler.Syntax.MemberKind)
FSharp.Compiler.Syntax.MemberKind: Boolean Equals(System.Object)
FSharp.Compiler.Syntax.MemberKind: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.Syntax.MemberKind: Boolean IsClassConstructor
FSharp.Compiler.Syntax.MemberKind: Boolean IsConstructor
FSharp.Compiler.Syntax.MemberKind: Boolean IsMember
FSharp.Compiler.Syntax.MemberKind: Boolean IsPropertyGet
FSharp.Compiler.Syntax.MemberKind: Boolean IsPropertyGetSet
FSharp.Compiler.Syntax.MemberKind: Boolean IsPropertySet
FSharp.Compiler.Syntax.MemberKind: Boolean get_IsClassConstructor()
FSharp.Compiler.Syntax.MemberKind: Boolean get_IsConstructor()
FSharp.Compiler.Syntax.MemberKind: Boolean get_IsMember()
FSharp.Compiler.Syntax.MemberKind: Boolean get_IsPropertyGet()
FSharp.Compiler.Syntax.MemberKind: Boolean get_IsPropertyGetSet()
FSharp.Compiler.Syntax.MemberKind: Boolean get_IsPropertySet()
FSharp.Compiler.Syntax.MemberKind: FSharp.Compiler.Syntax.MemberKind ClassConstructor
FSharp.Compiler.Syntax.MemberKind: FSharp.Compiler.Syntax.MemberKind Constructor
FSharp.Compiler.Syntax.MemberKind: FSharp.Compiler.Syntax.MemberKind Member
FSharp.Compiler.Syntax.MemberKind: FSharp.Compiler.Syntax.MemberKind PropertyGet
FSharp.Compiler.Syntax.MemberKind: FSharp.Compiler.Syntax.MemberKind PropertyGetSet
FSharp.Compiler.Syntax.MemberKind: FSharp.Compiler.Syntax.MemberKind PropertySet
FSharp.Compiler.Syntax.MemberKind: FSharp.Compiler.Syntax.MemberKind get_ClassConstructor()
FSharp.Compiler.Syntax.MemberKind: FSharp.Compiler.Syntax.MemberKind get_Constructor()
FSharp.Compiler.Syntax.MemberKind: FSharp.Compiler.Syntax.MemberKind get_Member()
FSharp.Compiler.Syntax.MemberKind: FSharp.Compiler.Syntax.MemberKind get_PropertyGet()
FSharp.Compiler.Syntax.MemberKind: FSharp.Compiler.Syntax.MemberKind get_PropertyGetSet()
FSharp.Compiler.Syntax.MemberKind: FSharp.Compiler.Syntax.MemberKind get_PropertySet()
FSharp.Compiler.Syntax.MemberKind: FSharp.Compiler.Syntax.MemberKind+Tags
FSharp.Compiler.Syntax.MemberKind: Int32 GetHashCode()
FSharp.Compiler.Syntax.MemberKind: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.Syntax.MemberKind: Int32 Tag
FSharp.Compiler.Syntax.MemberKind: Int32 get_Tag()
FSharp.Compiler.Syntax.MemberKind: System.String ToString()
FSharp.Compiler.Syntax.ParsedFsiInteraction
FSharp.Compiler.Syntax.ParsedFsiInteraction+ParsedFsiInteraction.Definitions: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.ParsedFsiInteraction+ParsedFsiInteraction.Definitions: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.ParsedFsiInteraction+ParsedFsiInteraction.Definitions: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynModuleDecl] defns
FSharp.Compiler.Syntax.ParsedFsiInteraction+ParsedFsiInteraction.Definitions: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynModuleDecl] get_defns()
FSharp.Compiler.Syntax.ParsedFsiInteraction+ParsedFsiInteraction.HashDirective: FSharp.Compiler.Syntax.ParsedHashDirective get_hashDirective()
FSharp.Compiler.Syntax.ParsedFsiInteraction+ParsedFsiInteraction.HashDirective: FSharp.Compiler.Syntax.ParsedHashDirective hashDirective
FSharp.Compiler.Syntax.ParsedFsiInteraction+ParsedFsiInteraction.HashDirective: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.ParsedFsiInteraction+ParsedFsiInteraction.HashDirective: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.ParsedFsiInteraction+Tags: Int32 ParsedFsiInteraction.Definitions
FSharp.Compiler.Syntax.ParsedFsiInteraction+Tags: Int32 ParsedFsiInteraction.HashDirective
FSharp.Compiler.Syntax.ParsedFsiInteraction: Boolean IsIDefns
FSharp.Compiler.Syntax.ParsedFsiInteraction: Boolean IsIHash
FSharp.Compiler.Syntax.ParsedFsiInteraction: Boolean get_IsIDefns()
FSharp.Compiler.Syntax.ParsedFsiInteraction: Boolean get_IsIHash()
FSharp.Compiler.Syntax.ParsedFsiInteraction: FSharp.Compiler.Syntax.ParsedFsiInteraction NewIDefns(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynModuleDecl], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.ParsedFsiInteraction: FSharp.Compiler.Syntax.ParsedFsiInteraction NewIHash(FSharp.Compiler.Syntax.ParsedHashDirective, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.ParsedFsiInteraction: FSharp.Compiler.Syntax.ParsedFsiInteraction+ParsedFsiInteraction.Definitions
FSharp.Compiler.Syntax.ParsedFsiInteraction: FSharp.Compiler.Syntax.ParsedFsiInteraction+ParsedFsiInteraction.HashDirective
FSharp.Compiler.Syntax.ParsedFsiInteraction: FSharp.Compiler.Syntax.ParsedFsiInteraction+Tags
FSharp.Compiler.Syntax.ParsedFsiInteraction: Int32 Tag
FSharp.Compiler.Syntax.ParsedFsiInteraction: Int32 get_Tag()
FSharp.Compiler.Syntax.ParsedFsiInteraction: System.String ToString()
FSharp.Compiler.Syntax.ParsedHashDirective
FSharp.Compiler.Syntax.ParsedHashDirective: FSharp.Compiler.Syntax.ParsedHashDirective NewParsedHashDirective(System.String, Microsoft.FSharp.Collections.FSharpList`1[System.String], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.ParsedHashDirective: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.ParsedHashDirective: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.ParsedHashDirective: Int32 Tag
FSharp.Compiler.Syntax.ParsedHashDirective: Int32 get_Tag()
FSharp.Compiler.Syntax.ParsedHashDirective: Microsoft.FSharp.Collections.FSharpList`1[System.String] args
FSharp.Compiler.Syntax.ParsedHashDirective: Microsoft.FSharp.Collections.FSharpList`1[System.String] get_args()
FSharp.Compiler.Syntax.ParsedHashDirective: System.String ToString()
FSharp.Compiler.Syntax.ParsedHashDirective: System.String get_ident()
FSharp.Compiler.Syntax.ParsedHashDirective: System.String ident
FSharp.Compiler.Syntax.ParsedImplFile
FSharp.Compiler.Syntax.ParsedImplFile: FSharp.Compiler.Syntax.ParsedImplFile NewParsedImplFile(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.ParsedHashDirective], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.ParsedImplFileFragment])
FSharp.Compiler.Syntax.ParsedImplFile: Int32 Tag
FSharp.Compiler.Syntax.ParsedImplFile: Int32 get_Tag()
FSharp.Compiler.Syntax.ParsedImplFile: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.ParsedHashDirective] get_hashDirectives()
FSharp.Compiler.Syntax.ParsedImplFile: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.ParsedHashDirective] hashDirectives
FSharp.Compiler.Syntax.ParsedImplFile: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.ParsedImplFileFragment] fragments
FSharp.Compiler.Syntax.ParsedImplFile: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.ParsedImplFileFragment] get_fragments()
FSharp.Compiler.Syntax.ParsedImplFile: System.String ToString()
FSharp.Compiler.Syntax.ParsedImplFileFragment
FSharp.Compiler.Syntax.ParsedImplFileFragment+AnonModule: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.ParsedImplFileFragment+AnonModule: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.ParsedImplFileFragment+AnonModule: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynModuleDecl] decls
FSharp.Compiler.Syntax.ParsedImplFileFragment+AnonModule: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynModuleDecl] get_decls()
FSharp.Compiler.Syntax.ParsedImplFileFragment+NamedModule: FSharp.Compiler.Syntax.SynModuleOrNamespace get_namedModule()
FSharp.Compiler.Syntax.ParsedImplFileFragment+NamedModule: FSharp.Compiler.Syntax.SynModuleOrNamespace namedModule
FSharp.Compiler.Syntax.ParsedImplFileFragment+NamespaceFragment: Boolean get_isRecursive()
FSharp.Compiler.Syntax.ParsedImplFileFragment+NamespaceFragment: Boolean isRecursive
FSharp.Compiler.Syntax.ParsedImplFileFragment+NamespaceFragment: FSharp.Compiler.Syntax.PreXmlDoc get_xmlDoc()
FSharp.Compiler.Syntax.ParsedImplFileFragment+NamespaceFragment: FSharp.Compiler.Syntax.PreXmlDoc xmlDoc
FSharp.Compiler.Syntax.ParsedImplFileFragment+NamespaceFragment: FSharp.Compiler.Syntax.SynModuleOrNamespaceKind get_kind()
FSharp.Compiler.Syntax.ParsedImplFileFragment+NamespaceFragment: FSharp.Compiler.Syntax.SynModuleOrNamespaceKind kind
FSharp.Compiler.Syntax.ParsedImplFileFragment+NamespaceFragment: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.ParsedImplFileFragment+NamespaceFragment: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.ParsedImplFileFragment+NamespaceFragment: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident] get_longId()
FSharp.Compiler.Syntax.ParsedImplFileFragment+NamespaceFragment: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident] longId
FSharp.Compiler.Syntax.ParsedImplFileFragment+NamespaceFragment: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList] attributes
FSharp.Compiler.Syntax.ParsedImplFileFragment+NamespaceFragment: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList] get_attributes()
FSharp.Compiler.Syntax.ParsedImplFileFragment+NamespaceFragment: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynModuleDecl] decls
FSharp.Compiler.Syntax.ParsedImplFileFragment+NamespaceFragment: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynModuleDecl] get_decls()
FSharp.Compiler.Syntax.ParsedImplFileFragment+Tags: Int32 AnonModule
FSharp.Compiler.Syntax.ParsedImplFileFragment+Tags: Int32 NamedModule
FSharp.Compiler.Syntax.ParsedImplFileFragment+Tags: Int32 NamespaceFragment
FSharp.Compiler.Syntax.ParsedImplFileFragment: Boolean IsAnonModule
FSharp.Compiler.Syntax.ParsedImplFileFragment: Boolean IsNamedModule
FSharp.Compiler.Syntax.ParsedImplFileFragment: Boolean IsNamespaceFragment
FSharp.Compiler.Syntax.ParsedImplFileFragment: Boolean get_IsAnonModule()
FSharp.Compiler.Syntax.ParsedImplFileFragment: Boolean get_IsNamedModule()
FSharp.Compiler.Syntax.ParsedImplFileFragment: Boolean get_IsNamespaceFragment()
FSharp.Compiler.Syntax.ParsedImplFileFragment: FSharp.Compiler.Syntax.ParsedImplFileFragment NewAnonModule(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynModuleDecl], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.ParsedImplFileFragment: FSharp.Compiler.Syntax.ParsedImplFileFragment NewNamedModule(FSharp.Compiler.Syntax.SynModuleOrNamespace)
FSharp.Compiler.Syntax.ParsedImplFileFragment: FSharp.Compiler.Syntax.ParsedImplFileFragment NewNamespaceFragment(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident], Boolean, FSharp.Compiler.Syntax.SynModuleOrNamespaceKind, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynModuleDecl], FSharp.Compiler.Syntax.PreXmlDoc, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.ParsedImplFileFragment: FSharp.Compiler.Syntax.ParsedImplFileFragment+AnonModule
FSharp.Compiler.Syntax.ParsedImplFileFragment: FSharp.Compiler.Syntax.ParsedImplFileFragment+NamedModule
FSharp.Compiler.Syntax.ParsedImplFileFragment: FSharp.Compiler.Syntax.ParsedImplFileFragment+NamespaceFragment
FSharp.Compiler.Syntax.ParsedImplFileFragment: FSharp.Compiler.Syntax.ParsedImplFileFragment+Tags
FSharp.Compiler.Syntax.ParsedImplFileFragment: Int32 Tag
FSharp.Compiler.Syntax.ParsedImplFileFragment: Int32 get_Tag()
FSharp.Compiler.Syntax.ParsedImplFileFragment: System.String ToString()
FSharp.Compiler.Syntax.ParsedImplFileInput
FSharp.Compiler.Syntax.ParsedImplFileInput: Boolean get_isScript()
FSharp.Compiler.Syntax.ParsedImplFileInput: Boolean isScript
FSharp.Compiler.Syntax.ParsedImplFileInput: FSharp.Compiler.Syntax.ParsedImplFileInput NewParsedImplFileInput(System.String, Boolean, FSharp.Compiler.Syntax.QualifiedNameOfFile, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.ScopedPragma], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.ParsedHashDirective], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynModuleOrNamespace], System.Tuple`2[System.Boolean,System.Boolean])
FSharp.Compiler.Syntax.ParsedImplFileInput: FSharp.Compiler.Syntax.QualifiedNameOfFile get_qualifiedNameOfFile()
FSharp.Compiler.Syntax.ParsedImplFileInput: FSharp.Compiler.Syntax.QualifiedNameOfFile qualifiedNameOfFile
FSharp.Compiler.Syntax.ParsedImplFileInput: Int32 Tag
FSharp.Compiler.Syntax.ParsedImplFileInput: Int32 get_Tag()
FSharp.Compiler.Syntax.ParsedImplFileInput: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.ParsedHashDirective] get_hashDirectives()
FSharp.Compiler.Syntax.ParsedImplFileInput: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.ParsedHashDirective] hashDirectives
FSharp.Compiler.Syntax.ParsedImplFileInput: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.ScopedPragma] get_scopedPragmas()
FSharp.Compiler.Syntax.ParsedImplFileInput: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.ScopedPragma] scopedPragmas
FSharp.Compiler.Syntax.ParsedImplFileInput: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynModuleOrNamespace] get_modules()
FSharp.Compiler.Syntax.ParsedImplFileInput: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynModuleOrNamespace] modules
FSharp.Compiler.Syntax.ParsedImplFileInput: System.String ToString()
FSharp.Compiler.Syntax.ParsedImplFileInput: System.String fileName
FSharp.Compiler.Syntax.ParsedImplFileInput: System.String get_fileName()
FSharp.Compiler.Syntax.ParsedImplFileInput: System.Tuple`2[System.Boolean,System.Boolean] get_isLastCompiland()
FSharp.Compiler.Syntax.ParsedImplFileInput: System.Tuple`2[System.Boolean,System.Boolean] isLastCompiland
FSharp.Compiler.Syntax.ParsedInput
FSharp.Compiler.Syntax.ParsedInput+ImplFile: FSharp.Compiler.Syntax.ParsedImplFileInput Item
FSharp.Compiler.Syntax.ParsedInput+ImplFile: FSharp.Compiler.Syntax.ParsedImplFileInput get_Item()
FSharp.Compiler.Syntax.ParsedInput+SigFile: FSharp.Compiler.Syntax.ParsedSigFileInput Item
FSharp.Compiler.Syntax.ParsedInput+SigFile: FSharp.Compiler.Syntax.ParsedSigFileInput get_Item()
FSharp.Compiler.Syntax.ParsedInput+Tags: Int32 ImplFile
FSharp.Compiler.Syntax.ParsedInput+Tags: Int32 SigFile
FSharp.Compiler.Syntax.ParsedInput: Boolean IsImplFile
FSharp.Compiler.Syntax.ParsedInput: Boolean IsSigFile
FSharp.Compiler.Syntax.ParsedInput: Boolean get_IsImplFile()
FSharp.Compiler.Syntax.ParsedInput: Boolean get_IsSigFile()
FSharp.Compiler.Syntax.ParsedInput: FSharp.Compiler.Syntax.ParsedInput NewImplFile(FSharp.Compiler.Syntax.ParsedImplFileInput)
FSharp.Compiler.Syntax.ParsedInput: FSharp.Compiler.Syntax.ParsedInput NewSigFile(FSharp.Compiler.Syntax.ParsedSigFileInput)
FSharp.Compiler.Syntax.ParsedInput: FSharp.Compiler.Syntax.ParsedInput+ImplFile
FSharp.Compiler.Syntax.ParsedInput: FSharp.Compiler.Syntax.ParsedInput+SigFile
FSharp.Compiler.Syntax.ParsedInput: FSharp.Compiler.Syntax.ParsedInput+Tags
FSharp.Compiler.Syntax.ParsedInput: FSharp.Compiler.Text.Range Range
FSharp.Compiler.Syntax.ParsedInput: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.Syntax.ParsedInput: Int32 Tag
FSharp.Compiler.Syntax.ParsedInput: Int32 get_Tag()
FSharp.Compiler.Syntax.ParsedInput: System.String ToString()
FSharp.Compiler.Syntax.ParsedSigFile
FSharp.Compiler.Syntax.ParsedSigFile: FSharp.Compiler.Syntax.ParsedSigFile NewParsedSigFile(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.ParsedHashDirective], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.ParsedSigFileFragment])
FSharp.Compiler.Syntax.ParsedSigFile: Int32 Tag
FSharp.Compiler.Syntax.ParsedSigFile: Int32 get_Tag()
FSharp.Compiler.Syntax.ParsedSigFile: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.ParsedHashDirective] get_hashDirectives()
FSharp.Compiler.Syntax.ParsedSigFile: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.ParsedHashDirective] hashDirectives
FSharp.Compiler.Syntax.ParsedSigFile: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.ParsedSigFileFragment] fragments
FSharp.Compiler.Syntax.ParsedSigFile: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.ParsedSigFileFragment] get_fragments()
FSharp.Compiler.Syntax.ParsedSigFile: System.String ToString()
FSharp.Compiler.Syntax.ParsedSigFileFragment
FSharp.Compiler.Syntax.ParsedSigFileFragment+AnonModule: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.ParsedSigFileFragment+AnonModule: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.ParsedSigFileFragment+AnonModule: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynModuleSigDecl] decls
FSharp.Compiler.Syntax.ParsedSigFileFragment+AnonModule: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynModuleSigDecl] get_decls()
FSharp.Compiler.Syntax.ParsedSigFileFragment+NamedModule: FSharp.Compiler.Syntax.SynModuleOrNamespaceSig get_namedModule()
FSharp.Compiler.Syntax.ParsedSigFileFragment+NamedModule: FSharp.Compiler.Syntax.SynModuleOrNamespaceSig namedModule
FSharp.Compiler.Syntax.ParsedSigFileFragment+NamespaceFragment: Boolean get_isRecursive()
FSharp.Compiler.Syntax.ParsedSigFileFragment+NamespaceFragment: Boolean isRecursive
FSharp.Compiler.Syntax.ParsedSigFileFragment+NamespaceFragment: FSharp.Compiler.Syntax.PreXmlDoc get_xmlDoc()
FSharp.Compiler.Syntax.ParsedSigFileFragment+NamespaceFragment: FSharp.Compiler.Syntax.PreXmlDoc xmlDoc
FSharp.Compiler.Syntax.ParsedSigFileFragment+NamespaceFragment: FSharp.Compiler.Syntax.SynModuleOrNamespaceKind get_kind()
FSharp.Compiler.Syntax.ParsedSigFileFragment+NamespaceFragment: FSharp.Compiler.Syntax.SynModuleOrNamespaceKind kind
FSharp.Compiler.Syntax.ParsedSigFileFragment+NamespaceFragment: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.ParsedSigFileFragment+NamespaceFragment: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.ParsedSigFileFragment+NamespaceFragment: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident] get_longId()
FSharp.Compiler.Syntax.ParsedSigFileFragment+NamespaceFragment: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident] longId
FSharp.Compiler.Syntax.ParsedSigFileFragment+NamespaceFragment: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList] attributes
FSharp.Compiler.Syntax.ParsedSigFileFragment+NamespaceFragment: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList] get_attributes()
FSharp.Compiler.Syntax.ParsedSigFileFragment+NamespaceFragment: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynModuleSigDecl] decls
FSharp.Compiler.Syntax.ParsedSigFileFragment+NamespaceFragment: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynModuleSigDecl] get_decls()
FSharp.Compiler.Syntax.ParsedSigFileFragment+Tags: Int32 AnonModule
FSharp.Compiler.Syntax.ParsedSigFileFragment+Tags: Int32 NamedModule
FSharp.Compiler.Syntax.ParsedSigFileFragment+Tags: Int32 NamespaceFragment
FSharp.Compiler.Syntax.ParsedSigFileFragment: Boolean IsAnonModule
FSharp.Compiler.Syntax.ParsedSigFileFragment: Boolean IsNamedModule
FSharp.Compiler.Syntax.ParsedSigFileFragment: Boolean IsNamespaceFragment
FSharp.Compiler.Syntax.ParsedSigFileFragment: Boolean get_IsAnonModule()
FSharp.Compiler.Syntax.ParsedSigFileFragment: Boolean get_IsNamedModule()
FSharp.Compiler.Syntax.ParsedSigFileFragment: Boolean get_IsNamespaceFragment()
FSharp.Compiler.Syntax.ParsedSigFileFragment: FSharp.Compiler.Syntax.ParsedSigFileFragment NewAnonModule(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynModuleSigDecl], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.ParsedSigFileFragment: FSharp.Compiler.Syntax.ParsedSigFileFragment NewNamedModule(FSharp.Compiler.Syntax.SynModuleOrNamespaceSig)
FSharp.Compiler.Syntax.ParsedSigFileFragment: FSharp.Compiler.Syntax.ParsedSigFileFragment NewNamespaceFragment(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident], Boolean, FSharp.Compiler.Syntax.SynModuleOrNamespaceKind, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynModuleSigDecl], FSharp.Compiler.Syntax.PreXmlDoc, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.ParsedSigFileFragment: FSharp.Compiler.Syntax.ParsedSigFileFragment+AnonModule
FSharp.Compiler.Syntax.ParsedSigFileFragment: FSharp.Compiler.Syntax.ParsedSigFileFragment+NamedModule
FSharp.Compiler.Syntax.ParsedSigFileFragment: FSharp.Compiler.Syntax.ParsedSigFileFragment+NamespaceFragment
FSharp.Compiler.Syntax.ParsedSigFileFragment: FSharp.Compiler.Syntax.ParsedSigFileFragment+Tags
FSharp.Compiler.Syntax.ParsedSigFileFragment: Int32 Tag
FSharp.Compiler.Syntax.ParsedSigFileFragment: Int32 get_Tag()
FSharp.Compiler.Syntax.ParsedSigFileFragment: System.String ToString()
FSharp.Compiler.Syntax.ParsedSigFileInput
FSharp.Compiler.Syntax.ParsedSigFileInput: FSharp.Compiler.Syntax.ParsedSigFileInput NewParsedSigFileInput(System.String, FSharp.Compiler.Syntax.QualifiedNameOfFile, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.ScopedPragma], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.ParsedHashDirective], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynModuleOrNamespaceSig])
FSharp.Compiler.Syntax.ParsedSigFileInput: FSharp.Compiler.Syntax.QualifiedNameOfFile get_qualifiedNameOfFile()
FSharp.Compiler.Syntax.ParsedSigFileInput: FSharp.Compiler.Syntax.QualifiedNameOfFile qualifiedNameOfFile
FSharp.Compiler.Syntax.ParsedSigFileInput: Int32 Tag
FSharp.Compiler.Syntax.ParsedSigFileInput: Int32 get_Tag()
FSharp.Compiler.Syntax.ParsedSigFileInput: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.ParsedHashDirective] get_hashDirectives()
FSharp.Compiler.Syntax.ParsedSigFileInput: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.ParsedHashDirective] hashDirectives
FSharp.Compiler.Syntax.ParsedSigFileInput: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.ScopedPragma] get_scopedPragmas()
FSharp.Compiler.Syntax.ParsedSigFileInput: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.ScopedPragma] scopedPragmas
FSharp.Compiler.Syntax.ParsedSigFileInput: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynModuleOrNamespaceSig] get_modules()
FSharp.Compiler.Syntax.ParsedSigFileInput: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynModuleOrNamespaceSig] modules
FSharp.Compiler.Syntax.ParsedSigFileInput: System.String ToString()
FSharp.Compiler.Syntax.ParsedSigFileInput: System.String fileName
FSharp.Compiler.Syntax.ParsedSigFileInput: System.String get_fileName()
FSharp.Compiler.Syntax.ParserDetail
FSharp.Compiler.Syntax.ParserDetail+Tags: Int32 ErrorRecovery
FSharp.Compiler.Syntax.ParserDetail+Tags: Int32 Ok
FSharp.Compiler.Syntax.ParserDetail: Boolean Equals(FSharp.Compiler.Syntax.ParserDetail)
FSharp.Compiler.Syntax.ParserDetail: Boolean Equals(System.Object)
FSharp.Compiler.Syntax.ParserDetail: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.Syntax.ParserDetail: Boolean IsErrorRecovery
FSharp.Compiler.Syntax.ParserDetail: Boolean IsOk
FSharp.Compiler.Syntax.ParserDetail: Boolean get_IsErrorRecovery()
FSharp.Compiler.Syntax.ParserDetail: Boolean get_IsOk()
FSharp.Compiler.Syntax.ParserDetail: FSharp.Compiler.Syntax.ParserDetail ErrorRecovery
FSharp.Compiler.Syntax.ParserDetail: FSharp.Compiler.Syntax.ParserDetail Ok
FSharp.Compiler.Syntax.ParserDetail: FSharp.Compiler.Syntax.ParserDetail get_ErrorRecovery()
FSharp.Compiler.Syntax.ParserDetail: FSharp.Compiler.Syntax.ParserDetail get_Ok()
FSharp.Compiler.Syntax.ParserDetail: FSharp.Compiler.Syntax.ParserDetail+Tags
FSharp.Compiler.Syntax.ParserDetail: Int32 CompareTo(FSharp.Compiler.Syntax.ParserDetail)
FSharp.Compiler.Syntax.ParserDetail: Int32 CompareTo(System.Object)
FSharp.Compiler.Syntax.ParserDetail: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.Syntax.ParserDetail: Int32 GetHashCode()
FSharp.Compiler.Syntax.ParserDetail: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.Syntax.ParserDetail: Int32 Tag
FSharp.Compiler.Syntax.ParserDetail: Int32 get_Tag()
FSharp.Compiler.Syntax.ParserDetail: System.String ToString()
FSharp.Compiler.Syntax.PreXmlDoc
FSharp.Compiler.Syntax.PreXmlDoc: Boolean Equals(FSharp.Compiler.Syntax.PreXmlDoc)
FSharp.Compiler.Syntax.PreXmlDoc: Boolean Equals(System.Object)
FSharp.Compiler.Syntax.PreXmlDoc: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.Syntax.PreXmlDoc: FSharp.Compiler.Syntax.PreXmlDoc Create(System.String[], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.PreXmlDoc: FSharp.Compiler.Syntax.PreXmlDoc Empty
FSharp.Compiler.Syntax.PreXmlDoc: FSharp.Compiler.Syntax.PreXmlDoc Merge(FSharp.Compiler.Syntax.PreXmlDoc, FSharp.Compiler.Syntax.PreXmlDoc)
FSharp.Compiler.Syntax.PreXmlDoc: FSharp.Compiler.Syntax.PreXmlDoc get_Empty()
FSharp.Compiler.Syntax.PreXmlDoc: FSharp.Compiler.Syntax.XmlDoc ToXmlDoc(Boolean, Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Collections.FSharpList`1[System.String]])
FSharp.Compiler.Syntax.PreXmlDoc: Int32 GetHashCode()
FSharp.Compiler.Syntax.PreXmlDoc: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.Syntax.PreXmlDoc: System.String ToString()
FSharp.Compiler.Syntax.PrettyNaming
FSharp.Compiler.Syntax.PrettyNaming: Boolean IsActivePatternName(System.String)
FSharp.Compiler.Syntax.PrettyNaming: Boolean IsCompilerGeneratedName(System.String)
FSharp.Compiler.Syntax.PrettyNaming: Boolean IsIdentifierFirstCharacter(Char)
FSharp.Compiler.Syntax.PrettyNaming: Boolean IsIdentifierPartCharacter(Char)
FSharp.Compiler.Syntax.PrettyNaming: Boolean IsInfixOperator(System.String)
FSharp.Compiler.Syntax.PrettyNaming: Boolean IsLongIdentifierPartCharacter(Char)
FSharp.Compiler.Syntax.PrettyNaming: Boolean IsMangledOpName(System.String)
FSharp.Compiler.Syntax.PrettyNaming: Boolean IsOperatorName(System.String)
FSharp.Compiler.Syntax.PrettyNaming: Boolean IsOperatorOrBacktickedName(System.String)
FSharp.Compiler.Syntax.PrettyNaming: Boolean IsPrefixOperator(System.String)
FSharp.Compiler.Syntax.PrettyNaming: Boolean IsPunctuation(System.String)
FSharp.Compiler.Syntax.PrettyNaming: Boolean IsTernaryOperator(System.String)
FSharp.Compiler.Syntax.PrettyNaming: Microsoft.FSharp.Collections.FSharpList`1[System.String] GetLongNameFromString(System.String)
FSharp.Compiler.Syntax.PrettyNaming: Microsoft.FSharp.Core.FSharpOption`1[System.String] TryChopPropertyName(System.String)
FSharp.Compiler.Syntax.PrettyNaming: System.String CompileOpName(System.String)
FSharp.Compiler.Syntax.PrettyNaming: System.String DecompileOpName(System.String)
FSharp.Compiler.Syntax.PrettyNaming: System.String DemangleOperatorName(System.String)
FSharp.Compiler.Syntax.PrettyNaming: System.String FormatAndOtherOverloadsString(Int32)
FSharp.Compiler.Syntax.PrettyNaming: System.String FsiDynamicModulePrefix
FSharp.Compiler.Syntax.PrettyNaming: System.String get_FsiDynamicModulePrefix()
FSharp.Compiler.Syntax.QualifiedNameOfFile
FSharp.Compiler.Syntax.QualifiedNameOfFile: FSharp.Compiler.Syntax.Ident Id
FSharp.Compiler.Syntax.QualifiedNameOfFile: FSharp.Compiler.Syntax.Ident Item
FSharp.Compiler.Syntax.QualifiedNameOfFile: FSharp.Compiler.Syntax.Ident get_Id()
FSharp.Compiler.Syntax.QualifiedNameOfFile: FSharp.Compiler.Syntax.Ident get_Item()
FSharp.Compiler.Syntax.QualifiedNameOfFile: FSharp.Compiler.Syntax.QualifiedNameOfFile NewQualifiedNameOfFile(FSharp.Compiler.Syntax.Ident)
FSharp.Compiler.Syntax.QualifiedNameOfFile: FSharp.Compiler.Text.Range Range
FSharp.Compiler.Syntax.QualifiedNameOfFile: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.Syntax.QualifiedNameOfFile: Int32 Tag
FSharp.Compiler.Syntax.QualifiedNameOfFile: Int32 get_Tag()
FSharp.Compiler.Syntax.QualifiedNameOfFile: System.String Text
FSharp.Compiler.Syntax.QualifiedNameOfFile: System.String ToString()
FSharp.Compiler.Syntax.QualifiedNameOfFile: System.String get_Text()
FSharp.Compiler.Syntax.ScopedPragma
FSharp.Compiler.Syntax.ScopedPragma: Boolean Equals(FSharp.Compiler.Syntax.ScopedPragma)
FSharp.Compiler.Syntax.ScopedPragma: Boolean Equals(System.Object)
FSharp.Compiler.Syntax.ScopedPragma: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.Syntax.ScopedPragma: FSharp.Compiler.Syntax.ScopedPragma NewWarningOff(FSharp.Compiler.Text.Range, Int32)
FSharp.Compiler.Syntax.ScopedPragma: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.ScopedPragma: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.ScopedPragma: Int32 GetHashCode()
FSharp.Compiler.Syntax.ScopedPragma: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.Syntax.ScopedPragma: Int32 Tag
FSharp.Compiler.Syntax.ScopedPragma: Int32 get_Tag()
FSharp.Compiler.Syntax.ScopedPragma: Int32 get_warningNumber()
FSharp.Compiler.Syntax.ScopedPragma: Int32 warningNumber
FSharp.Compiler.Syntax.ScopedPragma: System.String ToString()
FSharp.Compiler.Syntax.SeqExprOnly
FSharp.Compiler.Syntax.SeqExprOnly: Boolean Equals(FSharp.Compiler.Syntax.SeqExprOnly)
FSharp.Compiler.Syntax.SeqExprOnly: Boolean Equals(System.Object)
FSharp.Compiler.Syntax.SeqExprOnly: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.Syntax.SeqExprOnly: Boolean Item
FSharp.Compiler.Syntax.SeqExprOnly: Boolean get_Item()
FSharp.Compiler.Syntax.SeqExprOnly: FSharp.Compiler.Syntax.SeqExprOnly NewSeqExprOnly(Boolean)
FSharp.Compiler.Syntax.SeqExprOnly: Int32 CompareTo(FSharp.Compiler.Syntax.SeqExprOnly)
FSharp.Compiler.Syntax.SeqExprOnly: Int32 CompareTo(System.Object)
FSharp.Compiler.Syntax.SeqExprOnly: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.Syntax.SeqExprOnly: Int32 GetHashCode()
FSharp.Compiler.Syntax.SeqExprOnly: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.Syntax.SeqExprOnly: Int32 Tag
FSharp.Compiler.Syntax.SeqExprOnly: Int32 get_Tag()
FSharp.Compiler.Syntax.SeqExprOnly: System.String ToString()
FSharp.Compiler.Syntax.SynAccess
FSharp.Compiler.Syntax.SynAccess+Tags: Int32 Internal
FSharp.Compiler.Syntax.SynAccess+Tags: Int32 Private
FSharp.Compiler.Syntax.SynAccess+Tags: Int32 Public
FSharp.Compiler.Syntax.SynAccess: Boolean Equals(FSharp.Compiler.Syntax.SynAccess)
FSharp.Compiler.Syntax.SynAccess: Boolean Equals(System.Object)
FSharp.Compiler.Syntax.SynAccess: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.Syntax.SynAccess: Boolean IsInternal
FSharp.Compiler.Syntax.SynAccess: Boolean IsPrivate
FSharp.Compiler.Syntax.SynAccess: Boolean IsPublic
FSharp.Compiler.Syntax.SynAccess: Boolean get_IsInternal()
FSharp.Compiler.Syntax.SynAccess: Boolean get_IsPrivate()
FSharp.Compiler.Syntax.SynAccess: Boolean get_IsPublic()
FSharp.Compiler.Syntax.SynAccess: FSharp.Compiler.Syntax.SynAccess Internal
FSharp.Compiler.Syntax.SynAccess: FSharp.Compiler.Syntax.SynAccess Private
FSharp.Compiler.Syntax.SynAccess: FSharp.Compiler.Syntax.SynAccess Public
FSharp.Compiler.Syntax.SynAccess: FSharp.Compiler.Syntax.SynAccess get_Internal()
FSharp.Compiler.Syntax.SynAccess: FSharp.Compiler.Syntax.SynAccess get_Private()
FSharp.Compiler.Syntax.SynAccess: FSharp.Compiler.Syntax.SynAccess get_Public()
FSharp.Compiler.Syntax.SynAccess: FSharp.Compiler.Syntax.SynAccess+Tags
FSharp.Compiler.Syntax.SynAccess: Int32 CompareTo(FSharp.Compiler.Syntax.SynAccess)
FSharp.Compiler.Syntax.SynAccess: Int32 CompareTo(System.Object)
FSharp.Compiler.Syntax.SynAccess: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.Syntax.SynAccess: Int32 GetHashCode()
FSharp.Compiler.Syntax.SynAccess: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.Syntax.SynAccess: Int32 Tag
FSharp.Compiler.Syntax.SynAccess: Int32 get_Tag()
FSharp.Compiler.Syntax.SynAccess: System.String ToString()
FSharp.Compiler.Syntax.SynArgInfo
FSharp.Compiler.Syntax.SynArgInfo: Boolean get_optional()
FSharp.Compiler.Syntax.SynArgInfo: Boolean optional
FSharp.Compiler.Syntax.SynArgInfo: FSharp.Compiler.Syntax.SynArgInfo NewSynArgInfo(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList], Boolean, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.Ident])
FSharp.Compiler.Syntax.SynArgInfo: Int32 Tag
FSharp.Compiler.Syntax.SynArgInfo: Int32 get_Tag()
FSharp.Compiler.Syntax.SynArgInfo: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList] attributes
FSharp.Compiler.Syntax.SynArgInfo: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList] get_attributes()
FSharp.Compiler.Syntax.SynArgInfo: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.Ident] Ident
FSharp.Compiler.Syntax.SynArgInfo: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.Ident] get_Ident()
FSharp.Compiler.Syntax.SynArgInfo: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.Ident] get_ident()
FSharp.Compiler.Syntax.SynArgInfo: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.Ident] ident
FSharp.Compiler.Syntax.SynArgInfo: System.String ToString()
FSharp.Compiler.Syntax.SynArgPats
FSharp.Compiler.Syntax.SynArgPats+NamePatPairs: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynArgPats+NamePatPairs: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynArgPats+NamePatPairs: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[FSharp.Compiler.Syntax.Ident,FSharp.Compiler.Syntax.SynPat]] get_pats()
FSharp.Compiler.Syntax.SynArgPats+NamePatPairs: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[FSharp.Compiler.Syntax.Ident,FSharp.Compiler.Syntax.SynPat]] pats
FSharp.Compiler.Syntax.SynArgPats+Pats: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynPat] get_pats()
FSharp.Compiler.Syntax.SynArgPats+Pats: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynPat] pats
FSharp.Compiler.Syntax.SynArgPats+Tags: Int32 NamePatPairs
FSharp.Compiler.Syntax.SynArgPats+Tags: Int32 Pats
FSharp.Compiler.Syntax.SynArgPats: Boolean IsNamePatPairs
FSharp.Compiler.Syntax.SynArgPats: Boolean IsPats
FSharp.Compiler.Syntax.SynArgPats: Boolean get_IsNamePatPairs()
FSharp.Compiler.Syntax.SynArgPats: Boolean get_IsPats()
FSharp.Compiler.Syntax.SynArgPats: FSharp.Compiler.Syntax.SynArgPats NewNamePatPairs(Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[FSharp.Compiler.Syntax.Ident,FSharp.Compiler.Syntax.SynPat]], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynArgPats: FSharp.Compiler.Syntax.SynArgPats NewPats(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynPat])
FSharp.Compiler.Syntax.SynArgPats: FSharp.Compiler.Syntax.SynArgPats+NamePatPairs
FSharp.Compiler.Syntax.SynArgPats: FSharp.Compiler.Syntax.SynArgPats+Pats
FSharp.Compiler.Syntax.SynArgPats: FSharp.Compiler.Syntax.SynArgPats+Tags
FSharp.Compiler.Syntax.SynArgPats: Int32 Tag
FSharp.Compiler.Syntax.SynArgPats: Int32 get_Tag()
FSharp.Compiler.Syntax.SynArgPats: System.String ToString()
FSharp.Compiler.Syntax.SynAttribute
FSharp.Compiler.Syntax.SynAttribute: Boolean AppliesToGetterAndSetter
FSharp.Compiler.Syntax.SynAttribute: Boolean get_AppliesToGetterAndSetter()
FSharp.Compiler.Syntax.SynAttribute: FSharp.Compiler.Syntax.LongIdentWithDots TypeName
FSharp.Compiler.Syntax.SynAttribute: FSharp.Compiler.Syntax.LongIdentWithDots get_TypeName()
FSharp.Compiler.Syntax.SynAttribute: FSharp.Compiler.Syntax.SynExpr ArgExpr
FSharp.Compiler.Syntax.SynAttribute: FSharp.Compiler.Syntax.SynExpr get_ArgExpr()
FSharp.Compiler.Syntax.SynAttribute: FSharp.Compiler.Text.Range Range
FSharp.Compiler.Syntax.SynAttribute: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.Syntax.SynAttribute: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.Ident] Target
FSharp.Compiler.Syntax.SynAttribute: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.Ident] get_Target()
FSharp.Compiler.Syntax.SynAttribute: System.String ToString()
FSharp.Compiler.Syntax.SynAttribute: Void .ctor(FSharp.Compiler.Syntax.LongIdentWithDots, FSharp.Compiler.Syntax.SynExpr, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.Ident], Boolean, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynAttributeList
FSharp.Compiler.Syntax.SynAttributeList: FSharp.Compiler.Text.Range Range
FSharp.Compiler.Syntax.SynAttributeList: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.Syntax.SynAttributeList: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttribute] Attributes
FSharp.Compiler.Syntax.SynAttributeList: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttribute] get_Attributes()
FSharp.Compiler.Syntax.SynAttributeList: System.String ToString()
FSharp.Compiler.Syntax.SynAttributeList: Void .ctor(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttribute], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynBinding
FSharp.Compiler.Syntax.SynBinding: Boolean get_isMutable()
FSharp.Compiler.Syntax.SynBinding: Boolean get_mustInline()
FSharp.Compiler.Syntax.SynBinding: Boolean isMutable
FSharp.Compiler.Syntax.SynBinding: Boolean mustInline
FSharp.Compiler.Syntax.SynBinding: FSharp.Compiler.Syntax.DebugPointAtBinding get_seqPoint()
FSharp.Compiler.Syntax.SynBinding: FSharp.Compiler.Syntax.DebugPointAtBinding seqPoint
FSharp.Compiler.Syntax.SynBinding: FSharp.Compiler.Syntax.PreXmlDoc get_xmlDoc()
FSharp.Compiler.Syntax.SynBinding: FSharp.Compiler.Syntax.PreXmlDoc xmlDoc
FSharp.Compiler.Syntax.SynBinding: FSharp.Compiler.Syntax.SynBinding NewBinding(Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess], FSharp.Compiler.Syntax.SynBindingKind, Boolean, Boolean, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList], FSharp.Compiler.Syntax.PreXmlDoc, FSharp.Compiler.Syntax.SynValData, FSharp.Compiler.Syntax.SynPat, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynBindingReturnInfo], FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Text.Range, FSharp.Compiler.Syntax.DebugPointAtBinding)
FSharp.Compiler.Syntax.SynBinding: FSharp.Compiler.Syntax.SynBindingKind get_kind()
FSharp.Compiler.Syntax.SynBinding: FSharp.Compiler.Syntax.SynBindingKind kind
FSharp.Compiler.Syntax.SynBinding: FSharp.Compiler.Syntax.SynExpr expr
FSharp.Compiler.Syntax.SynBinding: FSharp.Compiler.Syntax.SynExpr get_expr()
FSharp.Compiler.Syntax.SynBinding: FSharp.Compiler.Syntax.SynPat get_headPat()
FSharp.Compiler.Syntax.SynBinding: FSharp.Compiler.Syntax.SynPat headPat
FSharp.Compiler.Syntax.SynBinding: FSharp.Compiler.Syntax.SynValData get_valData()
FSharp.Compiler.Syntax.SynBinding: FSharp.Compiler.Syntax.SynValData valData
FSharp.Compiler.Syntax.SynBinding: FSharp.Compiler.Text.Range RangeOfBindingAndRhs
FSharp.Compiler.Syntax.SynBinding: FSharp.Compiler.Text.Range RangeOfBindingSansRhs
FSharp.Compiler.Syntax.SynBinding: FSharp.Compiler.Text.Range RangeOfHeadPat
FSharp.Compiler.Syntax.SynBinding: FSharp.Compiler.Text.Range get_RangeOfBindingAndRhs()
FSharp.Compiler.Syntax.SynBinding: FSharp.Compiler.Text.Range get_RangeOfBindingSansRhs()
FSharp.Compiler.Syntax.SynBinding: FSharp.Compiler.Text.Range get_RangeOfHeadPat()
FSharp.Compiler.Syntax.SynBinding: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynBinding: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynBinding: Int32 Tag
FSharp.Compiler.Syntax.SynBinding: Int32 get_Tag()
FSharp.Compiler.Syntax.SynBinding: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList] attributes
FSharp.Compiler.Syntax.SynBinding: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList] get_attributes()
FSharp.Compiler.Syntax.SynBinding: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess] accessibility
FSharp.Compiler.Syntax.SynBinding: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess] get_accessibility()
FSharp.Compiler.Syntax.SynBinding: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynBindingReturnInfo] get_returnInfo()
FSharp.Compiler.Syntax.SynBinding: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynBindingReturnInfo] returnInfo
FSharp.Compiler.Syntax.SynBinding: System.String ToString()
FSharp.Compiler.Syntax.SynBindingKind
FSharp.Compiler.Syntax.SynBindingKind+Tags: Int32 Do
FSharp.Compiler.Syntax.SynBindingKind+Tags: Int32 NormalBinding
FSharp.Compiler.Syntax.SynBindingKind+Tags: Int32 StandaloneExpression
FSharp.Compiler.Syntax.SynBindingKind: Boolean Equals(FSharp.Compiler.Syntax.SynBindingKind)
FSharp.Compiler.Syntax.SynBindingKind: Boolean Equals(System.Object)
FSharp.Compiler.Syntax.SynBindingKind: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.Syntax.SynBindingKind: Boolean IsDoBinding
FSharp.Compiler.Syntax.SynBindingKind: Boolean IsNormalBinding
FSharp.Compiler.Syntax.SynBindingKind: Boolean IsStandaloneExpression
FSharp.Compiler.Syntax.SynBindingKind: Boolean get_IsDoBinding()
FSharp.Compiler.Syntax.SynBindingKind: Boolean get_IsNormalBinding()
FSharp.Compiler.Syntax.SynBindingKind: Boolean get_IsStandaloneExpression()
FSharp.Compiler.Syntax.SynBindingKind: FSharp.Compiler.Syntax.SynBindingKind Do
FSharp.Compiler.Syntax.SynBindingKind: FSharp.Compiler.Syntax.SynBindingKind NormalBinding
FSharp.Compiler.Syntax.SynBindingKind: FSharp.Compiler.Syntax.SynBindingKind StandaloneExpression
FSharp.Compiler.Syntax.SynBindingKind: FSharp.Compiler.Syntax.SynBindingKind get_DoBinding()
FSharp.Compiler.Syntax.SynBindingKind: FSharp.Compiler.Syntax.SynBindingKind get_NormalBinding()
FSharp.Compiler.Syntax.SynBindingKind: FSharp.Compiler.Syntax.SynBindingKind get_StandaloneExpression()
FSharp.Compiler.Syntax.SynBindingKind: FSharp.Compiler.Syntax.SynBindingKind+Tags
FSharp.Compiler.Syntax.SynBindingKind: Int32 CompareTo(FSharp.Compiler.Syntax.SynBindingKind)
FSharp.Compiler.Syntax.SynBindingKind: Int32 CompareTo(System.Object)
FSharp.Compiler.Syntax.SynBindingKind: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.Syntax.SynBindingKind: Int32 GetHashCode()
FSharp.Compiler.Syntax.SynBindingKind: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.Syntax.SynBindingKind: Int32 Tag
FSharp.Compiler.Syntax.SynBindingKind: Int32 get_Tag()
FSharp.Compiler.Syntax.SynBindingKind: System.String ToString()
FSharp.Compiler.Syntax.SynBindingReturnInfo
FSharp.Compiler.Syntax.SynBindingReturnInfo: FSharp.Compiler.Syntax.SynBindingReturnInfo NewSynBindingReturnInfo(FSharp.Compiler.Syntax.SynType, FSharp.Compiler.Text.Range, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList])
FSharp.Compiler.Syntax.SynBindingReturnInfo: FSharp.Compiler.Syntax.SynType get_typeName()
FSharp.Compiler.Syntax.SynBindingReturnInfo: FSharp.Compiler.Syntax.SynType typeName
FSharp.Compiler.Syntax.SynBindingReturnInfo: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynBindingReturnInfo: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynBindingReturnInfo: Int32 Tag
FSharp.Compiler.Syntax.SynBindingReturnInfo: Int32 get_Tag()
FSharp.Compiler.Syntax.SynBindingReturnInfo: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList] attributes
FSharp.Compiler.Syntax.SynBindingReturnInfo: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList] get_attributes()
FSharp.Compiler.Syntax.SynBindingReturnInfo: System.String ToString()
FSharp.Compiler.Syntax.SynComponentInfo
FSharp.Compiler.Syntax.SynComponentInfo: Boolean get_preferPostfix()
FSharp.Compiler.Syntax.SynComponentInfo: Boolean preferPostfix
FSharp.Compiler.Syntax.SynComponentInfo: FSharp.Compiler.Syntax.PreXmlDoc get_xmlDoc()
FSharp.Compiler.Syntax.SynComponentInfo: FSharp.Compiler.Syntax.PreXmlDoc xmlDoc
FSharp.Compiler.Syntax.SynComponentInfo: FSharp.Compiler.Syntax.SynComponentInfo NewComponentInfo(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynTyparDecl], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynTypeConstraint], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident], FSharp.Compiler.Syntax.PreXmlDoc, Boolean, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynComponentInfo: FSharp.Compiler.Text.Range Range
FSharp.Compiler.Syntax.SynComponentInfo: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.Syntax.SynComponentInfo: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynComponentInfo: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynComponentInfo: Int32 Tag
FSharp.Compiler.Syntax.SynComponentInfo: Int32 get_Tag()
FSharp.Compiler.Syntax.SynComponentInfo: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident] get_longId()
FSharp.Compiler.Syntax.SynComponentInfo: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident] longId
FSharp.Compiler.Syntax.SynComponentInfo: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList] attributes
FSharp.Compiler.Syntax.SynComponentInfo: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList] get_attributes()
FSharp.Compiler.Syntax.SynComponentInfo: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynTyparDecl] get_typeParams()
FSharp.Compiler.Syntax.SynComponentInfo: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynTyparDecl] typeParams
FSharp.Compiler.Syntax.SynComponentInfo: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynTypeConstraint] constraints
FSharp.Compiler.Syntax.SynComponentInfo: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynTypeConstraint] get_constraints()
FSharp.Compiler.Syntax.SynComponentInfo: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess] accessibility
FSharp.Compiler.Syntax.SynComponentInfo: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess] get_accessibility()
FSharp.Compiler.Syntax.SynComponentInfo: System.String ToString()
FSharp.Compiler.Syntax.SynConst
FSharp.Compiler.Syntax.SynConst+Bool: Boolean Item
FSharp.Compiler.Syntax.SynConst+Bool: Boolean get_Item()
FSharp.Compiler.Syntax.SynConst+Byte: Byte Item
FSharp.Compiler.Syntax.SynConst+Byte: Byte get_Item()
FSharp.Compiler.Syntax.SynConst+Bytes: Byte[] bytes
FSharp.Compiler.Syntax.SynConst+Bytes: Byte[] get_bytes()
FSharp.Compiler.Syntax.SynConst+Bytes: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynConst+Bytes: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynConst+Char: Char Item
FSharp.Compiler.Syntax.SynConst+Char: Char get_Item()
FSharp.Compiler.Syntax.SynConst+Decimal: System.Decimal Item
FSharp.Compiler.Syntax.SynConst+Decimal: System.Decimal get_Item()
FSharp.Compiler.Syntax.SynConst+Double: Double Item
FSharp.Compiler.Syntax.SynConst+Double: Double get_Item()
FSharp.Compiler.Syntax.SynConst+Int16: Int16 Item
FSharp.Compiler.Syntax.SynConst+Int16: Int16 get_Item()
FSharp.Compiler.Syntax.SynConst+Int32: Int32 Item
FSharp.Compiler.Syntax.SynConst+Int32: Int32 get_Item()
FSharp.Compiler.Syntax.SynConst+Int64: Int64 Item
FSharp.Compiler.Syntax.SynConst+Int64: Int64 get_Item()
FSharp.Compiler.Syntax.SynConst+IntPtr: Int64 Item
FSharp.Compiler.Syntax.SynConst+IntPtr: Int64 get_Item()
FSharp.Compiler.Syntax.SynConst+Measure: FSharp.Compiler.Syntax.SynConst constant
FSharp.Compiler.Syntax.SynConst+Measure: FSharp.Compiler.Syntax.SynConst get_constant()
FSharp.Compiler.Syntax.SynConst+Measure: FSharp.Compiler.Syntax.SynMeasure Item2
FSharp.Compiler.Syntax.SynConst+Measure: FSharp.Compiler.Syntax.SynMeasure get_Item2()
FSharp.Compiler.Syntax.SynConst+SByte: SByte Item
FSharp.Compiler.Syntax.SynConst+SByte: SByte get_Item()
FSharp.Compiler.Syntax.SynConst+Single: Single Item
FSharp.Compiler.Syntax.SynConst+Single: Single get_Item()
FSharp.Compiler.Syntax.SynConst+String: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynConst+String: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynConst+String: System.String get_text()
FSharp.Compiler.Syntax.SynConst+String: System.String text
FSharp.Compiler.Syntax.SynConst+Tags: Int32 Bool
FSharp.Compiler.Syntax.SynConst+Tags: Int32 Byte
FSharp.Compiler.Syntax.SynConst+Tags: Int32 Bytes
FSharp.Compiler.Syntax.SynConst+Tags: Int32 Char
FSharp.Compiler.Syntax.SynConst+Tags: Int32 Decimal
FSharp.Compiler.Syntax.SynConst+Tags: Int32 Double
FSharp.Compiler.Syntax.SynConst+Tags: Int32 Int16
FSharp.Compiler.Syntax.SynConst+Tags: Int32 Int32
FSharp.Compiler.Syntax.SynConst+Tags: Int32 Int64
FSharp.Compiler.Syntax.SynConst+Tags: Int32 IntPtr
FSharp.Compiler.Syntax.SynConst+Tags: Int32 Measure
FSharp.Compiler.Syntax.SynConst+Tags: Int32 SByte
FSharp.Compiler.Syntax.SynConst+Tags: Int32 Single
FSharp.Compiler.Syntax.SynConst+Tags: Int32 String
FSharp.Compiler.Syntax.SynConst+Tags: Int32 UInt16
FSharp.Compiler.Syntax.SynConst+Tags: Int32 UInt16s
FSharp.Compiler.Syntax.SynConst+Tags: Int32 UInt32
FSharp.Compiler.Syntax.SynConst+Tags: Int32 UInt64
FSharp.Compiler.Syntax.SynConst+Tags: Int32 UIntPtr
FSharp.Compiler.Syntax.SynConst+Tags: Int32 Unit
FSharp.Compiler.Syntax.SynConst+Tags: Int32 UserNum
FSharp.Compiler.Syntax.SynConst+UInt16: UInt16 Item
FSharp.Compiler.Syntax.SynConst+UInt16: UInt16 get_Item()
FSharp.Compiler.Syntax.SynConst+UInt16s: UInt16[] Item
FSharp.Compiler.Syntax.SynConst+UInt16s: UInt16[] get_Item()
FSharp.Compiler.Syntax.SynConst+UInt32: UInt32 Item
FSharp.Compiler.Syntax.SynConst+UInt32: UInt32 get_Item()
FSharp.Compiler.Syntax.SynConst+UInt64: UInt64 Item
FSharp.Compiler.Syntax.SynConst+UInt64: UInt64 get_Item()
FSharp.Compiler.Syntax.SynConst+UIntPtr: UInt64 Item
FSharp.Compiler.Syntax.SynConst+UIntPtr: UInt64 get_Item()
FSharp.Compiler.Syntax.SynConst+UserNum: System.String get_suffix()
FSharp.Compiler.Syntax.SynConst+UserNum: System.String get_value()
FSharp.Compiler.Syntax.SynConst+UserNum: System.String suffix
FSharp.Compiler.Syntax.SynConst+UserNum: System.String value
FSharp.Compiler.Syntax.SynConst: Boolean IsBool
FSharp.Compiler.Syntax.SynConst: Boolean IsByte
FSharp.Compiler.Syntax.SynConst: Boolean IsBytes
FSharp.Compiler.Syntax.SynConst: Boolean IsChar
FSharp.Compiler.Syntax.SynConst: Boolean IsDecimal
FSharp.Compiler.Syntax.SynConst: Boolean IsDouble
FSharp.Compiler.Syntax.SynConst: Boolean IsInt16
FSharp.Compiler.Syntax.SynConst: Boolean IsInt32
FSharp.Compiler.Syntax.SynConst: Boolean IsInt64
FSharp.Compiler.Syntax.SynConst: Boolean IsIntPtr
FSharp.Compiler.Syntax.SynConst: Boolean IsMeasure
FSharp.Compiler.Syntax.SynConst: Boolean IsSByte
FSharp.Compiler.Syntax.SynConst: Boolean IsSingle
FSharp.Compiler.Syntax.SynConst: Boolean IsString
FSharp.Compiler.Syntax.SynConst: Boolean IsUInt16
FSharp.Compiler.Syntax.SynConst: Boolean IsUInt16s
FSharp.Compiler.Syntax.SynConst: Boolean IsUInt32
FSharp.Compiler.Syntax.SynConst: Boolean IsUInt64
FSharp.Compiler.Syntax.SynConst: Boolean IsUIntPtr
FSharp.Compiler.Syntax.SynConst: Boolean IsUnit
FSharp.Compiler.Syntax.SynConst: Boolean IsUserNum
FSharp.Compiler.Syntax.SynConst: Boolean get_IsBool()
FSharp.Compiler.Syntax.SynConst: Boolean get_IsByte()
FSharp.Compiler.Syntax.SynConst: Boolean get_IsBytes()
FSharp.Compiler.Syntax.SynConst: Boolean get_IsChar()
FSharp.Compiler.Syntax.SynConst: Boolean get_IsDecimal()
FSharp.Compiler.Syntax.SynConst: Boolean get_IsDouble()
FSharp.Compiler.Syntax.SynConst: Boolean get_IsInt16()
FSharp.Compiler.Syntax.SynConst: Boolean get_IsInt32()
FSharp.Compiler.Syntax.SynConst: Boolean get_IsInt64()
FSharp.Compiler.Syntax.SynConst: Boolean get_IsIntPtr()
FSharp.Compiler.Syntax.SynConst: Boolean get_IsMeasure()
FSharp.Compiler.Syntax.SynConst: Boolean get_IsSByte()
FSharp.Compiler.Syntax.SynConst: Boolean get_IsSingle()
FSharp.Compiler.Syntax.SynConst: Boolean get_IsString()
FSharp.Compiler.Syntax.SynConst: Boolean get_IsUInt16()
FSharp.Compiler.Syntax.SynConst: Boolean get_IsUInt16s()
FSharp.Compiler.Syntax.SynConst: Boolean get_IsUInt32()
FSharp.Compiler.Syntax.SynConst: Boolean get_IsUInt64()
FSharp.Compiler.Syntax.SynConst: Boolean get_IsUIntPtr()
FSharp.Compiler.Syntax.SynConst: Boolean get_IsUnit()
FSharp.Compiler.Syntax.SynConst: Boolean get_IsUserNum()
FSharp.Compiler.Syntax.SynConst: FSharp.Compiler.Syntax.SynConst NewBool(Boolean)
FSharp.Compiler.Syntax.SynConst: FSharp.Compiler.Syntax.SynConst NewByte(Byte)
FSharp.Compiler.Syntax.SynConst: FSharp.Compiler.Syntax.SynConst NewBytes(Byte[], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynConst: FSharp.Compiler.Syntax.SynConst NewChar(Char)
FSharp.Compiler.Syntax.SynConst: FSharp.Compiler.Syntax.SynConst NewDecimal(System.Decimal)
FSharp.Compiler.Syntax.SynConst: FSharp.Compiler.Syntax.SynConst NewDouble(Double)
FSharp.Compiler.Syntax.SynConst: FSharp.Compiler.Syntax.SynConst NewInt16(Int16)
FSharp.Compiler.Syntax.SynConst: FSharp.Compiler.Syntax.SynConst NewInt32(Int32)
FSharp.Compiler.Syntax.SynConst: FSharp.Compiler.Syntax.SynConst NewInt64(Int64)
FSharp.Compiler.Syntax.SynConst: FSharp.Compiler.Syntax.SynConst NewIntPtr(Int64)
FSharp.Compiler.Syntax.SynConst: FSharp.Compiler.Syntax.SynConst NewMeasure(FSharp.Compiler.Syntax.SynConst, FSharp.Compiler.Syntax.SynMeasure)
FSharp.Compiler.Syntax.SynConst: FSharp.Compiler.Syntax.SynConst NewSByte(SByte)
FSharp.Compiler.Syntax.SynConst: FSharp.Compiler.Syntax.SynConst NewSingle(Single)
FSharp.Compiler.Syntax.SynConst: FSharp.Compiler.Syntax.SynConst NewString(System.String, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynConst: FSharp.Compiler.Syntax.SynConst NewUInt16(UInt16)
FSharp.Compiler.Syntax.SynConst: FSharp.Compiler.Syntax.SynConst NewUInt16s(UInt16[])
FSharp.Compiler.Syntax.SynConst: FSharp.Compiler.Syntax.SynConst NewUInt32(UInt32)
FSharp.Compiler.Syntax.SynConst: FSharp.Compiler.Syntax.SynConst NewUInt64(UInt64)
FSharp.Compiler.Syntax.SynConst: FSharp.Compiler.Syntax.SynConst NewUIntPtr(UInt64)
FSharp.Compiler.Syntax.SynConst: FSharp.Compiler.Syntax.SynConst NewUserNum(System.String, System.String)
FSharp.Compiler.Syntax.SynConst: FSharp.Compiler.Syntax.SynConst Unit
FSharp.Compiler.Syntax.SynConst: FSharp.Compiler.Syntax.SynConst get_Unit()
FSharp.Compiler.Syntax.SynConst: FSharp.Compiler.Syntax.SynConst+Bool
FSharp.Compiler.Syntax.SynConst: FSharp.Compiler.Syntax.SynConst+Byte
FSharp.Compiler.Syntax.SynConst: FSharp.Compiler.Syntax.SynConst+Bytes
FSharp.Compiler.Syntax.SynConst: FSharp.Compiler.Syntax.SynConst+Char
FSharp.Compiler.Syntax.SynConst: FSharp.Compiler.Syntax.SynConst+Decimal
FSharp.Compiler.Syntax.SynConst: FSharp.Compiler.Syntax.SynConst+Double
FSharp.Compiler.Syntax.SynConst: FSharp.Compiler.Syntax.SynConst+Int16
FSharp.Compiler.Syntax.SynConst: FSharp.Compiler.Syntax.SynConst+Int32
FSharp.Compiler.Syntax.SynConst: FSharp.Compiler.Syntax.SynConst+Int64
FSharp.Compiler.Syntax.SynConst: FSharp.Compiler.Syntax.SynConst+IntPtr
FSharp.Compiler.Syntax.SynConst: FSharp.Compiler.Syntax.SynConst+Measure
FSharp.Compiler.Syntax.SynConst: FSharp.Compiler.Syntax.SynConst+SByte
FSharp.Compiler.Syntax.SynConst: FSharp.Compiler.Syntax.SynConst+Single
FSharp.Compiler.Syntax.SynConst: FSharp.Compiler.Syntax.SynConst+String
FSharp.Compiler.Syntax.SynConst: FSharp.Compiler.Syntax.SynConst+Tags
FSharp.Compiler.Syntax.SynConst: FSharp.Compiler.Syntax.SynConst+UInt16
FSharp.Compiler.Syntax.SynConst: FSharp.Compiler.Syntax.SynConst+UInt16s
FSharp.Compiler.Syntax.SynConst: FSharp.Compiler.Syntax.SynConst+UInt32
FSharp.Compiler.Syntax.SynConst: FSharp.Compiler.Syntax.SynConst+UInt64
FSharp.Compiler.Syntax.SynConst: FSharp.Compiler.Syntax.SynConst+UIntPtr
FSharp.Compiler.Syntax.SynConst: FSharp.Compiler.Syntax.SynConst+UserNum
FSharp.Compiler.Syntax.SynConst: FSharp.Compiler.Text.Range Range(FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynConst: Int32 Tag
FSharp.Compiler.Syntax.SynConst: Int32 get_Tag()
FSharp.Compiler.Syntax.SynConst: System.String ToString()
FSharp.Compiler.Syntax.SynEnumCase
FSharp.Compiler.Syntax.SynEnumCase: FSharp.Compiler.Syntax.Ident get_ident()
FSharp.Compiler.Syntax.SynEnumCase: FSharp.Compiler.Syntax.Ident ident
FSharp.Compiler.Syntax.SynEnumCase: FSharp.Compiler.Syntax.PreXmlDoc get_xmlDoc()
FSharp.Compiler.Syntax.SynEnumCase: FSharp.Compiler.Syntax.PreXmlDoc xmlDoc
FSharp.Compiler.Syntax.SynEnumCase: FSharp.Compiler.Syntax.SynConst get_value()
FSharp.Compiler.Syntax.SynEnumCase: FSharp.Compiler.Syntax.SynConst value
FSharp.Compiler.Syntax.SynEnumCase: FSharp.Compiler.Syntax.SynEnumCase NewEnumCase(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList], FSharp.Compiler.Syntax.Ident, FSharp.Compiler.Syntax.SynConst, FSharp.Compiler.Syntax.PreXmlDoc, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynEnumCase: FSharp.Compiler.Text.Range Range
FSharp.Compiler.Syntax.SynEnumCase: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.Syntax.SynEnumCase: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynEnumCase: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynEnumCase: Int32 Tag
FSharp.Compiler.Syntax.SynEnumCase: Int32 get_Tag()
FSharp.Compiler.Syntax.SynEnumCase: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList] attributes
FSharp.Compiler.Syntax.SynEnumCase: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList] get_attributes()
FSharp.Compiler.Syntax.SynEnumCase: System.String ToString()
FSharp.Compiler.Syntax.SynExceptionDefn
FSharp.Compiler.Syntax.SynExceptionDefn: FSharp.Compiler.Syntax.SynExceptionDefn NewSynExceptionDefn(FSharp.Compiler.Syntax.SynExceptionDefnRepr, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynMemberDefn], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExceptionDefn: FSharp.Compiler.Syntax.SynExceptionDefnRepr exnRepr
FSharp.Compiler.Syntax.SynExceptionDefn: FSharp.Compiler.Syntax.SynExceptionDefnRepr get_exnRepr()
FSharp.Compiler.Syntax.SynExceptionDefn: FSharp.Compiler.Text.Range Range
FSharp.Compiler.Syntax.SynExceptionDefn: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.Syntax.SynExceptionDefn: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExceptionDefn: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExceptionDefn: Int32 Tag
FSharp.Compiler.Syntax.SynExceptionDefn: Int32 get_Tag()
FSharp.Compiler.Syntax.SynExceptionDefn: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynMemberDefn] get_members()
FSharp.Compiler.Syntax.SynExceptionDefn: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynMemberDefn] members
FSharp.Compiler.Syntax.SynExceptionDefn: System.String ToString()
FSharp.Compiler.Syntax.SynExceptionDefnRepr
FSharp.Compiler.Syntax.SynExceptionDefnRepr: FSharp.Compiler.Syntax.PreXmlDoc get_xmlDoc()
FSharp.Compiler.Syntax.SynExceptionDefnRepr: FSharp.Compiler.Syntax.PreXmlDoc xmlDoc
FSharp.Compiler.Syntax.SynExceptionDefnRepr: FSharp.Compiler.Syntax.SynExceptionDefnRepr NewSynExceptionDefnRepr(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList], FSharp.Compiler.Syntax.SynUnionCase, Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident]], FSharp.Compiler.Syntax.PreXmlDoc, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExceptionDefnRepr: FSharp.Compiler.Syntax.SynUnionCase caseName
FSharp.Compiler.Syntax.SynExceptionDefnRepr: FSharp.Compiler.Syntax.SynUnionCase get_caseName()
FSharp.Compiler.Syntax.SynExceptionDefnRepr: FSharp.Compiler.Text.Range Range
FSharp.Compiler.Syntax.SynExceptionDefnRepr: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.Syntax.SynExceptionDefnRepr: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExceptionDefnRepr: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExceptionDefnRepr: Int32 Tag
FSharp.Compiler.Syntax.SynExceptionDefnRepr: Int32 get_Tag()
FSharp.Compiler.Syntax.SynExceptionDefnRepr: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList] attributes
FSharp.Compiler.Syntax.SynExceptionDefnRepr: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList] get_attributes()
FSharp.Compiler.Syntax.SynExceptionDefnRepr: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess] accessibility
FSharp.Compiler.Syntax.SynExceptionDefnRepr: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess] get_accessibility()
FSharp.Compiler.Syntax.SynExceptionDefnRepr: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident]] get_longId()
FSharp.Compiler.Syntax.SynExceptionDefnRepr: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident]] longId
FSharp.Compiler.Syntax.SynExceptionDefnRepr: System.String ToString()
FSharp.Compiler.Syntax.SynExceptionSig
FSharp.Compiler.Syntax.SynExceptionSig: FSharp.Compiler.Syntax.SynExceptionDefnRepr exnRepr
FSharp.Compiler.Syntax.SynExceptionSig: FSharp.Compiler.Syntax.SynExceptionDefnRepr get_exnRepr()
FSharp.Compiler.Syntax.SynExceptionSig: FSharp.Compiler.Syntax.SynExceptionSig NewSynExceptionSig(FSharp.Compiler.Syntax.SynExceptionDefnRepr, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynMemberSig], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExceptionSig: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExceptionSig: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExceptionSig: Int32 Tag
FSharp.Compiler.Syntax.SynExceptionSig: Int32 get_Tag()
FSharp.Compiler.Syntax.SynExceptionSig: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynMemberSig] get_members()
FSharp.Compiler.Syntax.SynExceptionSig: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynMemberSig] members
FSharp.Compiler.Syntax.SynExceptionSig: System.String ToString()
FSharp.Compiler.Syntax.SynExpr
FSharp.Compiler.Syntax.SynExpr+AddressOf: Boolean get_isByref()
FSharp.Compiler.Syntax.SynExpr+AddressOf: Boolean isByref
FSharp.Compiler.Syntax.SynExpr+AddressOf: FSharp.Compiler.Syntax.SynExpr expr
FSharp.Compiler.Syntax.SynExpr+AddressOf: FSharp.Compiler.Syntax.SynExpr get_expr()
FSharp.Compiler.Syntax.SynExpr+AddressOf: FSharp.Compiler.Text.Range get_opRange()
FSharp.Compiler.Syntax.SynExpr+AddressOf: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+AddressOf: FSharp.Compiler.Text.Range opRange
FSharp.Compiler.Syntax.SynExpr+AddressOf: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+AnonRecd: Boolean get_isStruct()
FSharp.Compiler.Syntax.SynExpr+AnonRecd: Boolean isStruct
FSharp.Compiler.Syntax.SynExpr+AnonRecd: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+AnonRecd: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+AnonRecd: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[FSharp.Compiler.Syntax.Ident,FSharp.Compiler.Syntax.SynExpr]] get_recordFields()
FSharp.Compiler.Syntax.SynExpr+AnonRecd: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[FSharp.Compiler.Syntax.Ident,FSharp.Compiler.Syntax.SynExpr]] recordFields
FSharp.Compiler.Syntax.SynExpr+AnonRecd: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.Syntax.SynExpr,System.Tuple`2[FSharp.Compiler.Text.Range,Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Position]]]] copyInfo
FSharp.Compiler.Syntax.SynExpr+AnonRecd: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.Syntax.SynExpr,System.Tuple`2[FSharp.Compiler.Text.Range,Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Position]]]] get_copyInfo()
FSharp.Compiler.Syntax.SynExpr+App: Boolean get_isInfix()
FSharp.Compiler.Syntax.SynExpr+App: Boolean isInfix
FSharp.Compiler.Syntax.SynExpr+App: FSharp.Compiler.Syntax.ExprAtomicFlag flag
FSharp.Compiler.Syntax.SynExpr+App: FSharp.Compiler.Syntax.ExprAtomicFlag get_flag()
FSharp.Compiler.Syntax.SynExpr+App: FSharp.Compiler.Syntax.SynExpr argExpr
FSharp.Compiler.Syntax.SynExpr+App: FSharp.Compiler.Syntax.SynExpr funcExpr
FSharp.Compiler.Syntax.SynExpr+App: FSharp.Compiler.Syntax.SynExpr get_argExpr()
FSharp.Compiler.Syntax.SynExpr+App: FSharp.Compiler.Syntax.SynExpr get_funcExpr()
FSharp.Compiler.Syntax.SynExpr+App: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+App: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+ArbitraryAfterError: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+ArbitraryAfterError: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+ArbitraryAfterError: System.String debugStr
FSharp.Compiler.Syntax.SynExpr+ArbitraryAfterError: System.String get_debugStr()
FSharp.Compiler.Syntax.SynExpr+ArrayOrList: Boolean get_isArray()
FSharp.Compiler.Syntax.SynExpr+ArrayOrList: Boolean isArray
FSharp.Compiler.Syntax.SynExpr+ArrayOrList: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+ArrayOrList: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+ArrayOrList: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynExpr] exprs
FSharp.Compiler.Syntax.SynExpr+ArrayOrList: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynExpr] get_exprs()
FSharp.Compiler.Syntax.SynExpr+ArrayOrListOfSeqExpr: Boolean get_isArray()
FSharp.Compiler.Syntax.SynExpr+ArrayOrListOfSeqExpr: Boolean isArray
FSharp.Compiler.Syntax.SynExpr+ArrayOrListOfSeqExpr: FSharp.Compiler.Syntax.SynExpr expr
FSharp.Compiler.Syntax.SynExpr+ArrayOrListOfSeqExpr: FSharp.Compiler.Syntax.SynExpr get_expr()
FSharp.Compiler.Syntax.SynExpr+ArrayOrListOfSeqExpr: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+ArrayOrListOfSeqExpr: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+Assert: FSharp.Compiler.Syntax.SynExpr expr
FSharp.Compiler.Syntax.SynExpr+Assert: FSharp.Compiler.Syntax.SynExpr get_expr()
FSharp.Compiler.Syntax.SynExpr+Assert: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+Assert: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+CompExpr: Boolean get_isArrayOrList()
FSharp.Compiler.Syntax.SynExpr+CompExpr: Boolean isArrayOrList
FSharp.Compiler.Syntax.SynExpr+CompExpr: FSharp.Compiler.Syntax.SynExpr expr
FSharp.Compiler.Syntax.SynExpr+CompExpr: FSharp.Compiler.Syntax.SynExpr get_expr()
FSharp.Compiler.Syntax.SynExpr+CompExpr: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+CompExpr: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+CompExpr: Microsoft.FSharp.Core.FSharpRef`1[System.Boolean] get_isNotNakedRefCell()
FSharp.Compiler.Syntax.SynExpr+CompExpr: Microsoft.FSharp.Core.FSharpRef`1[System.Boolean] isNotNakedRefCell
FSharp.Compiler.Syntax.SynExpr+Const: FSharp.Compiler.Syntax.SynConst constant
FSharp.Compiler.Syntax.SynExpr+Const: FSharp.Compiler.Syntax.SynConst get_constant()
FSharp.Compiler.Syntax.SynExpr+Const: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+Const: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+DiscardAfterMissingQualificationAfterDot: FSharp.Compiler.Syntax.SynExpr expr
FSharp.Compiler.Syntax.SynExpr+DiscardAfterMissingQualificationAfterDot: FSharp.Compiler.Syntax.SynExpr get_expr()
FSharp.Compiler.Syntax.SynExpr+DiscardAfterMissingQualificationAfterDot: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+DiscardAfterMissingQualificationAfterDot: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+Do: FSharp.Compiler.Syntax.SynExpr expr
FSharp.Compiler.Syntax.SynExpr+Do: FSharp.Compiler.Syntax.SynExpr get_expr()
FSharp.Compiler.Syntax.SynExpr+Do: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+Do: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+DoBang: FSharp.Compiler.Syntax.SynExpr expr
FSharp.Compiler.Syntax.SynExpr+DoBang: FSharp.Compiler.Syntax.SynExpr get_expr()
FSharp.Compiler.Syntax.SynExpr+DoBang: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+DoBang: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+DotGet: FSharp.Compiler.Syntax.LongIdentWithDots get_longDotId()
FSharp.Compiler.Syntax.SynExpr+DotGet: FSharp.Compiler.Syntax.LongIdentWithDots longDotId
FSharp.Compiler.Syntax.SynExpr+DotGet: FSharp.Compiler.Syntax.SynExpr expr
FSharp.Compiler.Syntax.SynExpr+DotGet: FSharp.Compiler.Syntax.SynExpr get_expr()
FSharp.Compiler.Syntax.SynExpr+DotGet: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+DotGet: FSharp.Compiler.Text.Range get_rangeOfDot()
FSharp.Compiler.Syntax.SynExpr+DotGet: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+DotGet: FSharp.Compiler.Text.Range rangeOfDot
FSharp.Compiler.Syntax.SynExpr+DotIndexedGet: FSharp.Compiler.Syntax.SynExpr get_objectExpr()
FSharp.Compiler.Syntax.SynExpr+DotIndexedGet: FSharp.Compiler.Syntax.SynExpr objectExpr
FSharp.Compiler.Syntax.SynExpr+DotIndexedGet: FSharp.Compiler.Text.Range dotRange
FSharp.Compiler.Syntax.SynExpr+DotIndexedGet: FSharp.Compiler.Text.Range get_dotRange()
FSharp.Compiler.Syntax.SynExpr+DotIndexedGet: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+DotIndexedGet: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+DotIndexedGet: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynIndexerArg] get_indexExprs()
FSharp.Compiler.Syntax.SynExpr+DotIndexedGet: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynIndexerArg] indexExprs
FSharp.Compiler.Syntax.SynExpr+DotIndexedSet: FSharp.Compiler.Syntax.SynExpr get_objectExpr()
FSharp.Compiler.Syntax.SynExpr+DotIndexedSet: FSharp.Compiler.Syntax.SynExpr get_valueExpr()
FSharp.Compiler.Syntax.SynExpr+DotIndexedSet: FSharp.Compiler.Syntax.SynExpr objectExpr
FSharp.Compiler.Syntax.SynExpr+DotIndexedSet: FSharp.Compiler.Syntax.SynExpr valueExpr
FSharp.Compiler.Syntax.SynExpr+DotIndexedSet: FSharp.Compiler.Text.Range dotRange
FSharp.Compiler.Syntax.SynExpr+DotIndexedSet: FSharp.Compiler.Text.Range get_dotRange()
FSharp.Compiler.Syntax.SynExpr+DotIndexedSet: FSharp.Compiler.Text.Range get_leftOfSetRange()
FSharp.Compiler.Syntax.SynExpr+DotIndexedSet: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+DotIndexedSet: FSharp.Compiler.Text.Range leftOfSetRange
FSharp.Compiler.Syntax.SynExpr+DotIndexedSet: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+DotIndexedSet: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynIndexerArg] get_indexExprs()
FSharp.Compiler.Syntax.SynExpr+DotIndexedSet: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynIndexerArg] indexExprs
FSharp.Compiler.Syntax.SynExpr+DotNamedIndexedPropertySet: FSharp.Compiler.Syntax.LongIdentWithDots get_longDotId()
FSharp.Compiler.Syntax.SynExpr+DotNamedIndexedPropertySet: FSharp.Compiler.Syntax.LongIdentWithDots longDotId
FSharp.Compiler.Syntax.SynExpr+DotNamedIndexedPropertySet: FSharp.Compiler.Syntax.SynExpr argExpr
FSharp.Compiler.Syntax.SynExpr+DotNamedIndexedPropertySet: FSharp.Compiler.Syntax.SynExpr get_argExpr()
FSharp.Compiler.Syntax.SynExpr+DotNamedIndexedPropertySet: FSharp.Compiler.Syntax.SynExpr get_rhsExpr()
FSharp.Compiler.Syntax.SynExpr+DotNamedIndexedPropertySet: FSharp.Compiler.Syntax.SynExpr get_targetExpr()
FSharp.Compiler.Syntax.SynExpr+DotNamedIndexedPropertySet: FSharp.Compiler.Syntax.SynExpr rhsExpr
FSharp.Compiler.Syntax.SynExpr+DotNamedIndexedPropertySet: FSharp.Compiler.Syntax.SynExpr targetExpr
FSharp.Compiler.Syntax.SynExpr+DotNamedIndexedPropertySet: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+DotNamedIndexedPropertySet: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+DotSet: FSharp.Compiler.Syntax.LongIdentWithDots get_longDotId()
FSharp.Compiler.Syntax.SynExpr+DotSet: FSharp.Compiler.Syntax.LongIdentWithDots longDotId
FSharp.Compiler.Syntax.SynExpr+DotSet: FSharp.Compiler.Syntax.SynExpr get_rhsExpr()
FSharp.Compiler.Syntax.SynExpr+DotSet: FSharp.Compiler.Syntax.SynExpr get_targetExpr()
FSharp.Compiler.Syntax.SynExpr+DotSet: FSharp.Compiler.Syntax.SynExpr rhsExpr
FSharp.Compiler.Syntax.SynExpr+DotSet: FSharp.Compiler.Syntax.SynExpr targetExpr
FSharp.Compiler.Syntax.SynExpr+DotSet: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+DotSet: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+Downcast: FSharp.Compiler.Syntax.SynExpr expr
FSharp.Compiler.Syntax.SynExpr+Downcast: FSharp.Compiler.Syntax.SynExpr get_expr()
FSharp.Compiler.Syntax.SynExpr+Downcast: FSharp.Compiler.Syntax.SynType get_targetType()
FSharp.Compiler.Syntax.SynExpr+Downcast: FSharp.Compiler.Syntax.SynType targetType
FSharp.Compiler.Syntax.SynExpr+Downcast: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+Downcast: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+Fixed: FSharp.Compiler.Syntax.SynExpr expr
FSharp.Compiler.Syntax.SynExpr+Fixed: FSharp.Compiler.Syntax.SynExpr get_expr()
FSharp.Compiler.Syntax.SynExpr+Fixed: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+Fixed: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+For: Boolean direction
FSharp.Compiler.Syntax.SynExpr+For: Boolean get_direction()
FSharp.Compiler.Syntax.SynExpr+For: FSharp.Compiler.Syntax.DebugPointAtFor forSeqPoint
FSharp.Compiler.Syntax.SynExpr+For: FSharp.Compiler.Syntax.DebugPointAtFor get_forSeqPoint()
FSharp.Compiler.Syntax.SynExpr+For: FSharp.Compiler.Syntax.Ident get_ident()
FSharp.Compiler.Syntax.SynExpr+For: FSharp.Compiler.Syntax.Ident ident
FSharp.Compiler.Syntax.SynExpr+For: FSharp.Compiler.Syntax.SynExpr doBody
FSharp.Compiler.Syntax.SynExpr+For: FSharp.Compiler.Syntax.SynExpr get_doBody()
FSharp.Compiler.Syntax.SynExpr+For: FSharp.Compiler.Syntax.SynExpr get_identBody()
FSharp.Compiler.Syntax.SynExpr+For: FSharp.Compiler.Syntax.SynExpr get_toBody()
FSharp.Compiler.Syntax.SynExpr+For: FSharp.Compiler.Syntax.SynExpr identBody
FSharp.Compiler.Syntax.SynExpr+For: FSharp.Compiler.Syntax.SynExpr toBody
FSharp.Compiler.Syntax.SynExpr+For: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+For: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+ForEach: Boolean get_isFromSource()
FSharp.Compiler.Syntax.SynExpr+ForEach: Boolean isFromSource
FSharp.Compiler.Syntax.SynExpr+ForEach: FSharp.Compiler.Syntax.DebugPointAtFor forSeqPoint
FSharp.Compiler.Syntax.SynExpr+ForEach: FSharp.Compiler.Syntax.DebugPointAtFor get_forSeqPoint()
FSharp.Compiler.Syntax.SynExpr+ForEach: FSharp.Compiler.Syntax.SeqExprOnly get_seqExprOnly()
FSharp.Compiler.Syntax.SynExpr+ForEach: FSharp.Compiler.Syntax.SeqExprOnly seqExprOnly
FSharp.Compiler.Syntax.SynExpr+ForEach: FSharp.Compiler.Syntax.SynExpr bodyExpr
FSharp.Compiler.Syntax.SynExpr+ForEach: FSharp.Compiler.Syntax.SynExpr enumExpr
FSharp.Compiler.Syntax.SynExpr+ForEach: FSharp.Compiler.Syntax.SynExpr get_bodyExpr()
FSharp.Compiler.Syntax.SynExpr+ForEach: FSharp.Compiler.Syntax.SynExpr get_enumExpr()
FSharp.Compiler.Syntax.SynExpr+ForEach: FSharp.Compiler.Syntax.SynPat get_pat()
FSharp.Compiler.Syntax.SynExpr+ForEach: FSharp.Compiler.Syntax.SynPat pat
FSharp.Compiler.Syntax.SynExpr+ForEach: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+ForEach: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+FromParseError: FSharp.Compiler.Syntax.SynExpr expr
FSharp.Compiler.Syntax.SynExpr+FromParseError: FSharp.Compiler.Syntax.SynExpr get_expr()
FSharp.Compiler.Syntax.SynExpr+FromParseError: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+FromParseError: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+Ident: FSharp.Compiler.Syntax.Ident get_ident()
FSharp.Compiler.Syntax.SynExpr+Ident: FSharp.Compiler.Syntax.Ident ident
FSharp.Compiler.Syntax.SynExpr+IfThenElse: Boolean get_isFromErrorRecovery()
FSharp.Compiler.Syntax.SynExpr+IfThenElse: Boolean isFromErrorRecovery
FSharp.Compiler.Syntax.SynExpr+IfThenElse: FSharp.Compiler.Syntax.DebugPointAtBinding get_spIfToThen()
FSharp.Compiler.Syntax.SynExpr+IfThenElse: FSharp.Compiler.Syntax.DebugPointAtBinding spIfToThen
FSharp.Compiler.Syntax.SynExpr+IfThenElse: FSharp.Compiler.Syntax.SynExpr get_ifExpr()
FSharp.Compiler.Syntax.SynExpr+IfThenElse: FSharp.Compiler.Syntax.SynExpr get_thenExpr()
FSharp.Compiler.Syntax.SynExpr+IfThenElse: FSharp.Compiler.Syntax.SynExpr ifExpr
FSharp.Compiler.Syntax.SynExpr+IfThenElse: FSharp.Compiler.Syntax.SynExpr thenExpr
FSharp.Compiler.Syntax.SynExpr+IfThenElse: FSharp.Compiler.Text.Range get_ifToThenRange()
FSharp.Compiler.Syntax.SynExpr+IfThenElse: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+IfThenElse: FSharp.Compiler.Text.Range ifToThenRange
FSharp.Compiler.Syntax.SynExpr+IfThenElse: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+IfThenElse: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynExpr] elseExpr
FSharp.Compiler.Syntax.SynExpr+IfThenElse: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynExpr] get_elseExpr()
FSharp.Compiler.Syntax.SynExpr+ImplicitZero: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+ImplicitZero: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+InferredDowncast: FSharp.Compiler.Syntax.SynExpr expr
FSharp.Compiler.Syntax.SynExpr+InferredDowncast: FSharp.Compiler.Syntax.SynExpr get_expr()
FSharp.Compiler.Syntax.SynExpr+InferredDowncast: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+InferredDowncast: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+InferredUpcast: FSharp.Compiler.Syntax.SynExpr expr
FSharp.Compiler.Syntax.SynExpr+InferredUpcast: FSharp.Compiler.Syntax.SynExpr get_expr()
FSharp.Compiler.Syntax.SynExpr+InferredUpcast: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+InferredUpcast: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+InterpolatedString: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+InterpolatedString: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+InterpolatedString: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynInterpolatedStringPart] contents
FSharp.Compiler.Syntax.SynExpr+InterpolatedString: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynInterpolatedStringPart] get_contents()
FSharp.Compiler.Syntax.SynExpr+JoinIn: FSharp.Compiler.Syntax.SynExpr get_lhsExpr()
FSharp.Compiler.Syntax.SynExpr+JoinIn: FSharp.Compiler.Syntax.SynExpr get_rhsExpr()
FSharp.Compiler.Syntax.SynExpr+JoinIn: FSharp.Compiler.Syntax.SynExpr lhsExpr
FSharp.Compiler.Syntax.SynExpr+JoinIn: FSharp.Compiler.Syntax.SynExpr rhsExpr
FSharp.Compiler.Syntax.SynExpr+JoinIn: FSharp.Compiler.Text.Range get_lhsRange()
FSharp.Compiler.Syntax.SynExpr+JoinIn: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+JoinIn: FSharp.Compiler.Text.Range lhsRange
FSharp.Compiler.Syntax.SynExpr+JoinIn: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+Lambda: Boolean fromMethod
FSharp.Compiler.Syntax.SynExpr+Lambda: Boolean get_fromMethod()
FSharp.Compiler.Syntax.SynExpr+Lambda: Boolean get_inLambdaSeq()
FSharp.Compiler.Syntax.SynExpr+Lambda: Boolean inLambdaSeq
FSharp.Compiler.Syntax.SynExpr+Lambda: FSharp.Compiler.Syntax.SynExpr body
FSharp.Compiler.Syntax.SynExpr+Lambda: FSharp.Compiler.Syntax.SynExpr get_body()
FSharp.Compiler.Syntax.SynExpr+Lambda: FSharp.Compiler.Syntax.SynSimplePats args
FSharp.Compiler.Syntax.SynExpr+Lambda: FSharp.Compiler.Syntax.SynSimplePats get_args()
FSharp.Compiler.Syntax.SynExpr+Lambda: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+Lambda: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+Lambda: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynPat],FSharp.Compiler.Syntax.SynExpr]] get_parsedData()
FSharp.Compiler.Syntax.SynExpr+Lambda: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynPat],FSharp.Compiler.Syntax.SynExpr]] parsedData
FSharp.Compiler.Syntax.SynExpr+Lazy: FSharp.Compiler.Syntax.SynExpr expr
FSharp.Compiler.Syntax.SynExpr+Lazy: FSharp.Compiler.Syntax.SynExpr get_expr()
FSharp.Compiler.Syntax.SynExpr+Lazy: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+Lazy: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+LetOrUse: Boolean get_isRecursive()
FSharp.Compiler.Syntax.SynExpr+LetOrUse: Boolean get_isUse()
FSharp.Compiler.Syntax.SynExpr+LetOrUse: Boolean isRecursive
FSharp.Compiler.Syntax.SynExpr+LetOrUse: Boolean isUse
FSharp.Compiler.Syntax.SynExpr+LetOrUse: FSharp.Compiler.Syntax.SynExpr body
FSharp.Compiler.Syntax.SynExpr+LetOrUse: FSharp.Compiler.Syntax.SynExpr get_body()
FSharp.Compiler.Syntax.SynExpr+LetOrUse: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+LetOrUse: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+LetOrUse: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynBinding] bindings
FSharp.Compiler.Syntax.SynExpr+LetOrUse: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynBinding] get_bindings()
FSharp.Compiler.Syntax.SynExpr+LetOrUseBang: Boolean get_isFromSource()
FSharp.Compiler.Syntax.SynExpr+LetOrUseBang: Boolean get_isUse()
FSharp.Compiler.Syntax.SynExpr+LetOrUseBang: Boolean isFromSource
FSharp.Compiler.Syntax.SynExpr+LetOrUseBang: Boolean isUse
FSharp.Compiler.Syntax.SynExpr+LetOrUseBang: FSharp.Compiler.Syntax.DebugPointAtBinding bindSeqPoint
FSharp.Compiler.Syntax.SynExpr+LetOrUseBang: FSharp.Compiler.Syntax.DebugPointAtBinding get_bindSeqPoint()
FSharp.Compiler.Syntax.SynExpr+LetOrUseBang: FSharp.Compiler.Syntax.SynExpr body
FSharp.Compiler.Syntax.SynExpr+LetOrUseBang: FSharp.Compiler.Syntax.SynExpr get_body()
FSharp.Compiler.Syntax.SynExpr+LetOrUseBang: FSharp.Compiler.Syntax.SynExpr get_rhs()
FSharp.Compiler.Syntax.SynExpr+LetOrUseBang: FSharp.Compiler.Syntax.SynExpr rhs
FSharp.Compiler.Syntax.SynExpr+LetOrUseBang: FSharp.Compiler.Syntax.SynPat get_pat()
FSharp.Compiler.Syntax.SynExpr+LetOrUseBang: FSharp.Compiler.Syntax.SynPat pat
FSharp.Compiler.Syntax.SynExpr+LetOrUseBang: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+LetOrUseBang: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+LetOrUseBang: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`6[FSharp.Compiler.Syntax.DebugPointAtBinding,System.Boolean,System.Boolean,FSharp.Compiler.Syntax.SynPat,FSharp.Compiler.Syntax.SynExpr,FSharp.Compiler.Text.Range]] andBangs
FSharp.Compiler.Syntax.SynExpr+LetOrUseBang: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`6[FSharp.Compiler.Syntax.DebugPointAtBinding,System.Boolean,System.Boolean,FSharp.Compiler.Syntax.SynPat,FSharp.Compiler.Syntax.SynExpr,FSharp.Compiler.Text.Range]] get_andBangs()
FSharp.Compiler.Syntax.SynExpr+LibraryOnlyILAssembly: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+LibraryOnlyILAssembly: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+LibraryOnlyILAssembly: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynExpr] args
FSharp.Compiler.Syntax.SynExpr+LibraryOnlyILAssembly: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynExpr] get_args()
FSharp.Compiler.Syntax.SynExpr+LibraryOnlyILAssembly: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynType] get_retTy()
FSharp.Compiler.Syntax.SynExpr+LibraryOnlyILAssembly: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynType] get_typeArgs()
FSharp.Compiler.Syntax.SynExpr+LibraryOnlyILAssembly: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynType] retTy
FSharp.Compiler.Syntax.SynExpr+LibraryOnlyILAssembly: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynType] typeArgs
FSharp.Compiler.Syntax.SynExpr+LibraryOnlyILAssembly: System.Object get_ilCode()
FSharp.Compiler.Syntax.SynExpr+LibraryOnlyILAssembly: System.Object ilCode
FSharp.Compiler.Syntax.SynExpr+LibraryOnlyStaticOptimization: FSharp.Compiler.Syntax.SynExpr expr
FSharp.Compiler.Syntax.SynExpr+LibraryOnlyStaticOptimization: FSharp.Compiler.Syntax.SynExpr get_expr()
FSharp.Compiler.Syntax.SynExpr+LibraryOnlyStaticOptimization: FSharp.Compiler.Syntax.SynExpr get_optimizedExpr()
FSharp.Compiler.Syntax.SynExpr+LibraryOnlyStaticOptimization: FSharp.Compiler.Syntax.SynExpr optimizedExpr
FSharp.Compiler.Syntax.SynExpr+LibraryOnlyStaticOptimization: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+LibraryOnlyStaticOptimization: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+LibraryOnlyStaticOptimization: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynStaticOptimizationConstraint] constraints
FSharp.Compiler.Syntax.SynExpr+LibraryOnlyStaticOptimization: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynStaticOptimizationConstraint] get_constraints()
FSharp.Compiler.Syntax.SynExpr+LibraryOnlyUnionCaseFieldGet: FSharp.Compiler.Syntax.SynExpr expr
FSharp.Compiler.Syntax.SynExpr+LibraryOnlyUnionCaseFieldGet: FSharp.Compiler.Syntax.SynExpr get_expr()
FSharp.Compiler.Syntax.SynExpr+LibraryOnlyUnionCaseFieldGet: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+LibraryOnlyUnionCaseFieldGet: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+LibraryOnlyUnionCaseFieldGet: Int32 fieldNum
FSharp.Compiler.Syntax.SynExpr+LibraryOnlyUnionCaseFieldGet: Int32 get_fieldNum()
FSharp.Compiler.Syntax.SynExpr+LibraryOnlyUnionCaseFieldGet: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident] get_longId()
FSharp.Compiler.Syntax.SynExpr+LibraryOnlyUnionCaseFieldGet: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident] longId
FSharp.Compiler.Syntax.SynExpr+LibraryOnlyUnionCaseFieldSet: FSharp.Compiler.Syntax.SynExpr expr
FSharp.Compiler.Syntax.SynExpr+LibraryOnlyUnionCaseFieldSet: FSharp.Compiler.Syntax.SynExpr get_expr()
FSharp.Compiler.Syntax.SynExpr+LibraryOnlyUnionCaseFieldSet: FSharp.Compiler.Syntax.SynExpr get_rhsExpr()
FSharp.Compiler.Syntax.SynExpr+LibraryOnlyUnionCaseFieldSet: FSharp.Compiler.Syntax.SynExpr rhsExpr
FSharp.Compiler.Syntax.SynExpr+LibraryOnlyUnionCaseFieldSet: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+LibraryOnlyUnionCaseFieldSet: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+LibraryOnlyUnionCaseFieldSet: Int32 fieldNum
FSharp.Compiler.Syntax.SynExpr+LibraryOnlyUnionCaseFieldSet: Int32 get_fieldNum()
FSharp.Compiler.Syntax.SynExpr+LibraryOnlyUnionCaseFieldSet: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident] get_longId()
FSharp.Compiler.Syntax.SynExpr+LibraryOnlyUnionCaseFieldSet: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident] longId
FSharp.Compiler.Syntax.SynExpr+LongIdent: Boolean get_isOptional()
FSharp.Compiler.Syntax.SynExpr+LongIdent: Boolean isOptional
FSharp.Compiler.Syntax.SynExpr+LongIdent: FSharp.Compiler.Syntax.LongIdentWithDots get_longDotId()
FSharp.Compiler.Syntax.SynExpr+LongIdent: FSharp.Compiler.Syntax.LongIdentWithDots longDotId
FSharp.Compiler.Syntax.SynExpr+LongIdent: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+LongIdent: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+LongIdent: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.FSharpRef`1[FSharp.Compiler.Syntax.SynSimplePatAlternativeIdInfo]] altNameRefCell
FSharp.Compiler.Syntax.SynExpr+LongIdent: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.FSharpRef`1[FSharp.Compiler.Syntax.SynSimplePatAlternativeIdInfo]] get_altNameRefCell()
FSharp.Compiler.Syntax.SynExpr+LongIdentSet: FSharp.Compiler.Syntax.LongIdentWithDots get_longDotId()
FSharp.Compiler.Syntax.SynExpr+LongIdentSet: FSharp.Compiler.Syntax.LongIdentWithDots longDotId
FSharp.Compiler.Syntax.SynExpr+LongIdentSet: FSharp.Compiler.Syntax.SynExpr expr
FSharp.Compiler.Syntax.SynExpr+LongIdentSet: FSharp.Compiler.Syntax.SynExpr get_expr()
FSharp.Compiler.Syntax.SynExpr+LongIdentSet: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+LongIdentSet: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+Match: FSharp.Compiler.Syntax.DebugPointAtBinding get_matchSeqPoint()
FSharp.Compiler.Syntax.SynExpr+Match: FSharp.Compiler.Syntax.DebugPointAtBinding matchSeqPoint
FSharp.Compiler.Syntax.SynExpr+Match: FSharp.Compiler.Syntax.SynExpr expr
FSharp.Compiler.Syntax.SynExpr+Match: FSharp.Compiler.Syntax.SynExpr get_expr()
FSharp.Compiler.Syntax.SynExpr+Match: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+Match: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+Match: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynMatchClause] clauses
FSharp.Compiler.Syntax.SynExpr+Match: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynMatchClause] get_clauses()
FSharp.Compiler.Syntax.SynExpr+MatchBang: FSharp.Compiler.Syntax.DebugPointAtBinding get_matchSeqPoint()
FSharp.Compiler.Syntax.SynExpr+MatchBang: FSharp.Compiler.Syntax.DebugPointAtBinding matchSeqPoint
FSharp.Compiler.Syntax.SynExpr+MatchBang: FSharp.Compiler.Syntax.SynExpr expr
FSharp.Compiler.Syntax.SynExpr+MatchBang: FSharp.Compiler.Syntax.SynExpr get_expr()
FSharp.Compiler.Syntax.SynExpr+MatchBang: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+MatchBang: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+MatchBang: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynMatchClause] clauses
FSharp.Compiler.Syntax.SynExpr+MatchBang: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynMatchClause] get_clauses()
FSharp.Compiler.Syntax.SynExpr+MatchLambda: Boolean get_isExnMatch()
FSharp.Compiler.Syntax.SynExpr+MatchLambda: Boolean isExnMatch
FSharp.Compiler.Syntax.SynExpr+MatchLambda: FSharp.Compiler.Syntax.DebugPointAtBinding get_matchSeqPoint()
FSharp.Compiler.Syntax.SynExpr+MatchLambda: FSharp.Compiler.Syntax.DebugPointAtBinding matchSeqPoint
FSharp.Compiler.Syntax.SynExpr+MatchLambda: FSharp.Compiler.Text.Range get_keywordRange()
FSharp.Compiler.Syntax.SynExpr+MatchLambda: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+MatchLambda: FSharp.Compiler.Text.Range keywordRange
FSharp.Compiler.Syntax.SynExpr+MatchLambda: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+MatchLambda: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynMatchClause] get_matchClauses()
FSharp.Compiler.Syntax.SynExpr+MatchLambda: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynMatchClause] matchClauses
FSharp.Compiler.Syntax.SynExpr+NamedIndexedPropertySet: FSharp.Compiler.Syntax.LongIdentWithDots get_longDotId()
FSharp.Compiler.Syntax.SynExpr+NamedIndexedPropertySet: FSharp.Compiler.Syntax.LongIdentWithDots longDotId
FSharp.Compiler.Syntax.SynExpr+NamedIndexedPropertySet: FSharp.Compiler.Syntax.SynExpr expr1
FSharp.Compiler.Syntax.SynExpr+NamedIndexedPropertySet: FSharp.Compiler.Syntax.SynExpr expr2
FSharp.Compiler.Syntax.SynExpr+NamedIndexedPropertySet: FSharp.Compiler.Syntax.SynExpr get_expr1()
FSharp.Compiler.Syntax.SynExpr+NamedIndexedPropertySet: FSharp.Compiler.Syntax.SynExpr get_expr2()
FSharp.Compiler.Syntax.SynExpr+NamedIndexedPropertySet: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+NamedIndexedPropertySet: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+New: Boolean get_isProtected()
FSharp.Compiler.Syntax.SynExpr+New: Boolean isProtected
FSharp.Compiler.Syntax.SynExpr+New: FSharp.Compiler.Syntax.SynExpr expr
FSharp.Compiler.Syntax.SynExpr+New: FSharp.Compiler.Syntax.SynExpr get_expr()
FSharp.Compiler.Syntax.SynExpr+New: FSharp.Compiler.Syntax.SynType get_targetType()
FSharp.Compiler.Syntax.SynExpr+New: FSharp.Compiler.Syntax.SynType targetType
FSharp.Compiler.Syntax.SynExpr+New: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+New: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+Null: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+Null: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+ObjExpr: FSharp.Compiler.Syntax.SynType get_objType()
FSharp.Compiler.Syntax.SynExpr+ObjExpr: FSharp.Compiler.Syntax.SynType objType
FSharp.Compiler.Syntax.SynExpr+ObjExpr: FSharp.Compiler.Text.Range get_newExprRange()
FSharp.Compiler.Syntax.SynExpr+ObjExpr: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+ObjExpr: FSharp.Compiler.Text.Range newExprRange
FSharp.Compiler.Syntax.SynExpr+ObjExpr: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+ObjExpr: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynBinding] bindings
FSharp.Compiler.Syntax.SynExpr+ObjExpr: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynBinding] get_bindings()
FSharp.Compiler.Syntax.SynExpr+ObjExpr: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynInterfaceImpl] extraImpls
FSharp.Compiler.Syntax.SynExpr+ObjExpr: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynInterfaceImpl] get_extraImpls()
FSharp.Compiler.Syntax.SynExpr+ObjExpr: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.Syntax.SynExpr,Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.Ident]]] argOptions
FSharp.Compiler.Syntax.SynExpr+ObjExpr: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.Syntax.SynExpr,Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.Ident]]] get_argOptions()
FSharp.Compiler.Syntax.SynExpr+Paren: FSharp.Compiler.Syntax.SynExpr expr
FSharp.Compiler.Syntax.SynExpr+Paren: FSharp.Compiler.Syntax.SynExpr get_expr()
FSharp.Compiler.Syntax.SynExpr+Paren: FSharp.Compiler.Text.Range get_leftParenRange()
FSharp.Compiler.Syntax.SynExpr+Paren: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+Paren: FSharp.Compiler.Text.Range leftParenRange
FSharp.Compiler.Syntax.SynExpr+Paren: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+Paren: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] get_rightParenRange()
FSharp.Compiler.Syntax.SynExpr+Paren: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] rightParenRange
FSharp.Compiler.Syntax.SynExpr+Quote: Boolean get_isFromQueryExpression()
FSharp.Compiler.Syntax.SynExpr+Quote: Boolean get_isRaw()
FSharp.Compiler.Syntax.SynExpr+Quote: Boolean isFromQueryExpression
FSharp.Compiler.Syntax.SynExpr+Quote: Boolean isRaw
FSharp.Compiler.Syntax.SynExpr+Quote: FSharp.Compiler.Syntax.SynExpr get_operator()
FSharp.Compiler.Syntax.SynExpr+Quote: FSharp.Compiler.Syntax.SynExpr get_quotedExpr()
FSharp.Compiler.Syntax.SynExpr+Quote: FSharp.Compiler.Syntax.SynExpr operator
FSharp.Compiler.Syntax.SynExpr+Quote: FSharp.Compiler.Syntax.SynExpr quotedExpr
FSharp.Compiler.Syntax.SynExpr+Quote: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+Quote: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+Record: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+Record: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+Record: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`3[System.Tuple`2[FSharp.Compiler.Syntax.LongIdentWithDots,System.Boolean],Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynExpr],Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.Text.Range,Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Position]]]]] get_recordFields()
FSharp.Compiler.Syntax.SynExpr+Record: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`3[System.Tuple`2[FSharp.Compiler.Syntax.LongIdentWithDots,System.Boolean],Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynExpr],Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.Text.Range,Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Position]]]]] recordFields
FSharp.Compiler.Syntax.SynExpr+Record: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.Syntax.SynExpr,System.Tuple`2[FSharp.Compiler.Text.Range,Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Position]]]] copyInfo
FSharp.Compiler.Syntax.SynExpr+Record: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.Syntax.SynExpr,System.Tuple`2[FSharp.Compiler.Text.Range,Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Position]]]] get_copyInfo()
FSharp.Compiler.Syntax.SynExpr+Record: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`5[FSharp.Compiler.Syntax.SynType,FSharp.Compiler.Syntax.SynExpr,FSharp.Compiler.Text.Range,Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.Text.Range,Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Position]]],FSharp.Compiler.Text.Range]] baseInfo
FSharp.Compiler.Syntax.SynExpr+Record: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`5[FSharp.Compiler.Syntax.SynType,FSharp.Compiler.Syntax.SynExpr,FSharp.Compiler.Text.Range,Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.Text.Range,Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Position]]],FSharp.Compiler.Text.Range]] get_baseInfo()
FSharp.Compiler.Syntax.SynExpr+Sequential: Boolean get_isTrueSeq()
FSharp.Compiler.Syntax.SynExpr+Sequential: Boolean isTrueSeq
FSharp.Compiler.Syntax.SynExpr+Sequential: FSharp.Compiler.Syntax.DebugPointAtSequential get_seqPoint()
FSharp.Compiler.Syntax.SynExpr+Sequential: FSharp.Compiler.Syntax.DebugPointAtSequential seqPoint
FSharp.Compiler.Syntax.SynExpr+Sequential: FSharp.Compiler.Syntax.SynExpr expr1
FSharp.Compiler.Syntax.SynExpr+Sequential: FSharp.Compiler.Syntax.SynExpr expr2
FSharp.Compiler.Syntax.SynExpr+Sequential: FSharp.Compiler.Syntax.SynExpr get_expr1()
FSharp.Compiler.Syntax.SynExpr+Sequential: FSharp.Compiler.Syntax.SynExpr get_expr2()
FSharp.Compiler.Syntax.SynExpr+Sequential: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+Sequential: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+SequentialOrImplicitYield: FSharp.Compiler.Syntax.DebugPointAtSequential get_seqPoint()
FSharp.Compiler.Syntax.SynExpr+SequentialOrImplicitYield: FSharp.Compiler.Syntax.DebugPointAtSequential seqPoint
FSharp.Compiler.Syntax.SynExpr+SequentialOrImplicitYield: FSharp.Compiler.Syntax.SynExpr expr1
FSharp.Compiler.Syntax.SynExpr+SequentialOrImplicitYield: FSharp.Compiler.Syntax.SynExpr expr2
FSharp.Compiler.Syntax.SynExpr+SequentialOrImplicitYield: FSharp.Compiler.Syntax.SynExpr get_expr1()
FSharp.Compiler.Syntax.SynExpr+SequentialOrImplicitYield: FSharp.Compiler.Syntax.SynExpr get_expr2()
FSharp.Compiler.Syntax.SynExpr+SequentialOrImplicitYield: FSharp.Compiler.Syntax.SynExpr get_ifNotStmt()
FSharp.Compiler.Syntax.SynExpr+SequentialOrImplicitYield: FSharp.Compiler.Syntax.SynExpr ifNotStmt
FSharp.Compiler.Syntax.SynExpr+SequentialOrImplicitYield: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+SequentialOrImplicitYield: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+Set: FSharp.Compiler.Syntax.SynExpr get_rhsExpr()
FSharp.Compiler.Syntax.SynExpr+Set: FSharp.Compiler.Syntax.SynExpr get_targetExpr()
FSharp.Compiler.Syntax.SynExpr+Set: FSharp.Compiler.Syntax.SynExpr rhsExpr
FSharp.Compiler.Syntax.SynExpr+Set: FSharp.Compiler.Syntax.SynExpr targetExpr
FSharp.Compiler.Syntax.SynExpr+Set: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+Set: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 AddressOf
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 AnonRecd
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 App
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 ArbitraryAfterError
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 ArrayOrList
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 ArrayOrListOfSeqExpr
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 Assert
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 CompExpr
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 Const
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 DiscardAfterMissingQualificationAfterDot
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 Do
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 DoBang
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 DotGet
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 DotIndexedGet
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 DotIndexedSet
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 DotNamedIndexedPropertySet
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 DotSet
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 Downcast
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 Fixed
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 For
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 ForEach
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 FromParseError
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 Ident
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 IfThenElse
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 ImplicitZero
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 InferredDowncast
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 InferredUpcast
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 InterpolatedString
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 JoinIn
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 Lambda
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 Lazy
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 LetOrUse
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 LetOrUseBang
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 LibraryOnlyILAssembly
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 LibraryOnlyStaticOptimization
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 LibraryOnlyUnionCaseFieldGet
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 LibraryOnlyUnionCaseFieldSet
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 LongIdent
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 LongIdentSet
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 Match
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 MatchBang
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 MatchLambda
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 NamedIndexedPropertySet
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 New
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 Null
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 ObjExpr
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 Paren
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 Quote
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 Record
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 Sequential
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 SequentialOrImplicitYield
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 Set
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 TraitCall
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 TryFinally
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 TryWith
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 Tuple
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 TypeApp
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 TypeTest
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 Typed
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 Upcast
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 While
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 YieldOrReturn
FSharp.Compiler.Syntax.SynExpr+Tags: Int32 YieldOrReturnFrom
FSharp.Compiler.Syntax.SynExpr+TraitCall: FSharp.Compiler.Syntax.SynExpr argExpr
FSharp.Compiler.Syntax.SynExpr+TraitCall: FSharp.Compiler.Syntax.SynExpr get_argExpr()
FSharp.Compiler.Syntax.SynExpr+TraitCall: FSharp.Compiler.Syntax.SynMemberSig get_traitSig()
FSharp.Compiler.Syntax.SynExpr+TraitCall: FSharp.Compiler.Syntax.SynMemberSig traitSig
FSharp.Compiler.Syntax.SynExpr+TraitCall: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+TraitCall: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+TraitCall: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynTypar] get_supportTys()
FSharp.Compiler.Syntax.SynExpr+TraitCall: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynTypar] supportTys
FSharp.Compiler.Syntax.SynExpr+TryFinally: FSharp.Compiler.Syntax.DebugPointAtFinally finallySeqPoint
FSharp.Compiler.Syntax.SynExpr+TryFinally: FSharp.Compiler.Syntax.DebugPointAtFinally get_finallySeqPoint()
FSharp.Compiler.Syntax.SynExpr+TryFinally: FSharp.Compiler.Syntax.DebugPointAtTry get_trySeqPoint()
FSharp.Compiler.Syntax.SynExpr+TryFinally: FSharp.Compiler.Syntax.DebugPointAtTry trySeqPoint
FSharp.Compiler.Syntax.SynExpr+TryFinally: FSharp.Compiler.Syntax.SynExpr finallyExpr
FSharp.Compiler.Syntax.SynExpr+TryFinally: FSharp.Compiler.Syntax.SynExpr get_finallyExpr()
FSharp.Compiler.Syntax.SynExpr+TryFinally: FSharp.Compiler.Syntax.SynExpr get_tryExpr()
FSharp.Compiler.Syntax.SynExpr+TryFinally: FSharp.Compiler.Syntax.SynExpr tryExpr
FSharp.Compiler.Syntax.SynExpr+TryFinally: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+TryFinally: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+TryWith: FSharp.Compiler.Syntax.DebugPointAtTry get_trySeqPoint()
FSharp.Compiler.Syntax.SynExpr+TryWith: FSharp.Compiler.Syntax.DebugPointAtTry trySeqPoint
FSharp.Compiler.Syntax.SynExpr+TryWith: FSharp.Compiler.Syntax.DebugPointAtWith get_withSeqPoint()
FSharp.Compiler.Syntax.SynExpr+TryWith: FSharp.Compiler.Syntax.DebugPointAtWith withSeqPoint
FSharp.Compiler.Syntax.SynExpr+TryWith: FSharp.Compiler.Syntax.SynExpr get_tryExpr()
FSharp.Compiler.Syntax.SynExpr+TryWith: FSharp.Compiler.Syntax.SynExpr tryExpr
FSharp.Compiler.Syntax.SynExpr+TryWith: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+TryWith: FSharp.Compiler.Text.Range get_tryRange()
FSharp.Compiler.Syntax.SynExpr+TryWith: FSharp.Compiler.Text.Range get_withRange()
FSharp.Compiler.Syntax.SynExpr+TryWith: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+TryWith: FSharp.Compiler.Text.Range tryRange
FSharp.Compiler.Syntax.SynExpr+TryWith: FSharp.Compiler.Text.Range withRange
FSharp.Compiler.Syntax.SynExpr+TryWith: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynMatchClause] get_withCases()
FSharp.Compiler.Syntax.SynExpr+TryWith: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynMatchClause] withCases
FSharp.Compiler.Syntax.SynExpr+Tuple: Boolean get_isStruct()
FSharp.Compiler.Syntax.SynExpr+Tuple: Boolean isStruct
FSharp.Compiler.Syntax.SynExpr+Tuple: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+Tuple: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+Tuple: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynExpr] exprs
FSharp.Compiler.Syntax.SynExpr+Tuple: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynExpr] get_exprs()
FSharp.Compiler.Syntax.SynExpr+Tuple: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Text.Range] commaRanges
FSharp.Compiler.Syntax.SynExpr+Tuple: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Text.Range] get_commaRanges()
FSharp.Compiler.Syntax.SynExpr+TypeApp: FSharp.Compiler.Syntax.SynExpr expr
FSharp.Compiler.Syntax.SynExpr+TypeApp: FSharp.Compiler.Syntax.SynExpr get_expr()
FSharp.Compiler.Syntax.SynExpr+TypeApp: FSharp.Compiler.Text.Range get_lessRange()
FSharp.Compiler.Syntax.SynExpr+TypeApp: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+TypeApp: FSharp.Compiler.Text.Range get_typeArgsRange()
FSharp.Compiler.Syntax.SynExpr+TypeApp: FSharp.Compiler.Text.Range lessRange
FSharp.Compiler.Syntax.SynExpr+TypeApp: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+TypeApp: FSharp.Compiler.Text.Range typeArgsRange
FSharp.Compiler.Syntax.SynExpr+TypeApp: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynType] get_typeArgs()
FSharp.Compiler.Syntax.SynExpr+TypeApp: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynType] typeArgs
FSharp.Compiler.Syntax.SynExpr+TypeApp: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Text.Range] commaRanges
FSharp.Compiler.Syntax.SynExpr+TypeApp: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Text.Range] get_commaRanges()
FSharp.Compiler.Syntax.SynExpr+TypeApp: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] get_greaterRange()
FSharp.Compiler.Syntax.SynExpr+TypeApp: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] greaterRange
FSharp.Compiler.Syntax.SynExpr+TypeTest: FSharp.Compiler.Syntax.SynExpr expr
FSharp.Compiler.Syntax.SynExpr+TypeTest: FSharp.Compiler.Syntax.SynExpr get_expr()
FSharp.Compiler.Syntax.SynExpr+TypeTest: FSharp.Compiler.Syntax.SynType get_targetType()
FSharp.Compiler.Syntax.SynExpr+TypeTest: FSharp.Compiler.Syntax.SynType targetType
FSharp.Compiler.Syntax.SynExpr+TypeTest: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+TypeTest: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+Typed: FSharp.Compiler.Syntax.SynExpr expr
FSharp.Compiler.Syntax.SynExpr+Typed: FSharp.Compiler.Syntax.SynExpr get_expr()
FSharp.Compiler.Syntax.SynExpr+Typed: FSharp.Compiler.Syntax.SynType get_targetType()
FSharp.Compiler.Syntax.SynExpr+Typed: FSharp.Compiler.Syntax.SynType targetType
FSharp.Compiler.Syntax.SynExpr+Typed: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+Typed: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+Upcast: FSharp.Compiler.Syntax.SynExpr expr
FSharp.Compiler.Syntax.SynExpr+Upcast: FSharp.Compiler.Syntax.SynExpr get_expr()
FSharp.Compiler.Syntax.SynExpr+Upcast: FSharp.Compiler.Syntax.SynType get_targetType()
FSharp.Compiler.Syntax.SynExpr+Upcast: FSharp.Compiler.Syntax.SynType targetType
FSharp.Compiler.Syntax.SynExpr+Upcast: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+Upcast: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+While: FSharp.Compiler.Syntax.DebugPointAtWhile get_whileSeqPoint()
FSharp.Compiler.Syntax.SynExpr+While: FSharp.Compiler.Syntax.DebugPointAtWhile whileSeqPoint
FSharp.Compiler.Syntax.SynExpr+While: FSharp.Compiler.Syntax.SynExpr doExpr
FSharp.Compiler.Syntax.SynExpr+While: FSharp.Compiler.Syntax.SynExpr get_doExpr()
FSharp.Compiler.Syntax.SynExpr+While: FSharp.Compiler.Syntax.SynExpr get_whileExpr()
FSharp.Compiler.Syntax.SynExpr+While: FSharp.Compiler.Syntax.SynExpr whileExpr
FSharp.Compiler.Syntax.SynExpr+While: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+While: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+YieldOrReturn: FSharp.Compiler.Syntax.SynExpr expr
FSharp.Compiler.Syntax.SynExpr+YieldOrReturn: FSharp.Compiler.Syntax.SynExpr get_expr()
FSharp.Compiler.Syntax.SynExpr+YieldOrReturn: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+YieldOrReturn: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+YieldOrReturn: System.Tuple`2[System.Boolean,System.Boolean] flags
FSharp.Compiler.Syntax.SynExpr+YieldOrReturn: System.Tuple`2[System.Boolean,System.Boolean] get_flags()
FSharp.Compiler.Syntax.SynExpr+YieldOrReturnFrom: FSharp.Compiler.Syntax.SynExpr expr
FSharp.Compiler.Syntax.SynExpr+YieldOrReturnFrom: FSharp.Compiler.Syntax.SynExpr get_expr()
FSharp.Compiler.Syntax.SynExpr+YieldOrReturnFrom: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynExpr+YieldOrReturnFrom: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynExpr+YieldOrReturnFrom: System.Tuple`2[System.Boolean,System.Boolean] flags
FSharp.Compiler.Syntax.SynExpr+YieldOrReturnFrom: System.Tuple`2[System.Boolean,System.Boolean] get_flags()
FSharp.Compiler.Syntax.SynExpr: Boolean IsAddressOf
FSharp.Compiler.Syntax.SynExpr: Boolean IsAnonRecd
FSharp.Compiler.Syntax.SynExpr: Boolean IsApp
FSharp.Compiler.Syntax.SynExpr: Boolean IsArbExprAndThusAlreadyReportedError
FSharp.Compiler.Syntax.SynExpr: Boolean IsArbitraryAfterError
FSharp.Compiler.Syntax.SynExpr: Boolean IsArrayOrList
FSharp.Compiler.Syntax.SynExpr: Boolean IsArrayOrListOfSeqExpr
FSharp.Compiler.Syntax.SynExpr: Boolean IsAssert
FSharp.Compiler.Syntax.SynExpr: Boolean IsCompExpr
FSharp.Compiler.Syntax.SynExpr: Boolean IsConst
FSharp.Compiler.Syntax.SynExpr: Boolean IsDiscardAfterMissingQualificationAfterDot
FSharp.Compiler.Syntax.SynExpr: Boolean IsDo
FSharp.Compiler.Syntax.SynExpr: Boolean IsDoBang
FSharp.Compiler.Syntax.SynExpr: Boolean IsDotGet
FSharp.Compiler.Syntax.SynExpr: Boolean IsDotIndexedGet
FSharp.Compiler.Syntax.SynExpr: Boolean IsDotIndexedSet
FSharp.Compiler.Syntax.SynExpr: Boolean IsDotNamedIndexedPropertySet
FSharp.Compiler.Syntax.SynExpr: Boolean IsDotSet
FSharp.Compiler.Syntax.SynExpr: Boolean IsDowncast
FSharp.Compiler.Syntax.SynExpr: Boolean IsFixed
FSharp.Compiler.Syntax.SynExpr: Boolean IsFor
FSharp.Compiler.Syntax.SynExpr: Boolean IsForEach
FSharp.Compiler.Syntax.SynExpr: Boolean IsFromParseError
FSharp.Compiler.Syntax.SynExpr: Boolean IsIdent
FSharp.Compiler.Syntax.SynExpr: Boolean IsIfThenElse
FSharp.Compiler.Syntax.SynExpr: Boolean IsImplicitZero
FSharp.Compiler.Syntax.SynExpr: Boolean IsInferredDowncast
FSharp.Compiler.Syntax.SynExpr: Boolean IsInferredUpcast
FSharp.Compiler.Syntax.SynExpr: Boolean IsInterpolatedString
FSharp.Compiler.Syntax.SynExpr: Boolean IsJoinIn
FSharp.Compiler.Syntax.SynExpr: Boolean IsLambda
FSharp.Compiler.Syntax.SynExpr: Boolean IsLazy
FSharp.Compiler.Syntax.SynExpr: Boolean IsLetOrUse
FSharp.Compiler.Syntax.SynExpr: Boolean IsLetOrUseBang
FSharp.Compiler.Syntax.SynExpr: Boolean IsLibraryOnlyILAssembly
FSharp.Compiler.Syntax.SynExpr: Boolean IsLibraryOnlyStaticOptimization
FSharp.Compiler.Syntax.SynExpr: Boolean IsLibraryOnlyUnionCaseFieldGet
FSharp.Compiler.Syntax.SynExpr: Boolean IsLibraryOnlyUnionCaseFieldSet
FSharp.Compiler.Syntax.SynExpr: Boolean IsLongIdent
FSharp.Compiler.Syntax.SynExpr: Boolean IsLongIdentSet
FSharp.Compiler.Syntax.SynExpr: Boolean IsMatch
FSharp.Compiler.Syntax.SynExpr: Boolean IsMatchBang
FSharp.Compiler.Syntax.SynExpr: Boolean IsMatchLambda
FSharp.Compiler.Syntax.SynExpr: Boolean IsNamedIndexedPropertySet
FSharp.Compiler.Syntax.SynExpr: Boolean IsNew
FSharp.Compiler.Syntax.SynExpr: Boolean IsNull
FSharp.Compiler.Syntax.SynExpr: Boolean IsObjExpr
FSharp.Compiler.Syntax.SynExpr: Boolean IsParen
FSharp.Compiler.Syntax.SynExpr: Boolean IsQuote
FSharp.Compiler.Syntax.SynExpr: Boolean IsRecord
FSharp.Compiler.Syntax.SynExpr: Boolean IsSequential
FSharp.Compiler.Syntax.SynExpr: Boolean IsSequentialOrImplicitYield
FSharp.Compiler.Syntax.SynExpr: Boolean IsSet
FSharp.Compiler.Syntax.SynExpr: Boolean IsTraitCall
FSharp.Compiler.Syntax.SynExpr: Boolean IsTryFinally
FSharp.Compiler.Syntax.SynExpr: Boolean IsTryWith
FSharp.Compiler.Syntax.SynExpr: Boolean IsTuple
FSharp.Compiler.Syntax.SynExpr: Boolean IsTypeApp
FSharp.Compiler.Syntax.SynExpr: Boolean IsTypeTest
FSharp.Compiler.Syntax.SynExpr: Boolean IsTyped
FSharp.Compiler.Syntax.SynExpr: Boolean IsUpcast
FSharp.Compiler.Syntax.SynExpr: Boolean IsWhile
FSharp.Compiler.Syntax.SynExpr: Boolean IsYieldOrReturn
FSharp.Compiler.Syntax.SynExpr: Boolean IsYieldOrReturnFrom
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsAddressOf()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsAnonRecd()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsApp()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsArbExprAndThusAlreadyReportedError()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsArbitraryAfterError()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsArrayOrList()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsArrayOrListOfSeqExpr()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsAssert()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsCompExpr()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsConst()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsDiscardAfterMissingQualificationAfterDot()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsDo()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsDoBang()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsDotGet()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsDotIndexedGet()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsDotIndexedSet()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsDotNamedIndexedPropertySet()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsDotSet()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsDowncast()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsFixed()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsFor()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsForEach()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsFromParseError()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsIdent()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsIfThenElse()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsImplicitZero()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsInferredDowncast()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsInferredUpcast()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsInterpolatedString()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsJoinIn()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsLambda()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsLazy()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsLetOrUse()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsLetOrUseBang()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsLibraryOnlyILAssembly()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsLibraryOnlyStaticOptimization()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsLibraryOnlyUnionCaseFieldGet()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsLibraryOnlyUnionCaseFieldSet()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsLongIdent()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsLongIdentSet()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsMatch()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsMatchBang()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsMatchLambda()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsNamedIndexedPropertySet()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsNew()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsNull()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsObjExpr()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsParen()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsQuote()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsRecord()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsSequential()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsSequentialOrImplicitYield()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsSet()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsTraitCall()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsTryFinally()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsTryWith()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsTuple()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsTypeApp()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsTypeTest()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsTyped()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsUpcast()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsWhile()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsYieldOrReturn()
FSharp.Compiler.Syntax.SynExpr: Boolean get_IsYieldOrReturnFrom()
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewAddressOf(Boolean, FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Text.Range, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewAnonRecd(Boolean, Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.Syntax.SynExpr,System.Tuple`2[FSharp.Compiler.Text.Range,Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Position]]]], Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[FSharp.Compiler.Syntax.Ident,FSharp.Compiler.Syntax.SynExpr]], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewApp(FSharp.Compiler.Syntax.ExprAtomicFlag, Boolean, FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewArbitraryAfterError(System.String, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewArrayOrList(Boolean, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynExpr], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewArrayOrListOfSeqExpr(Boolean, FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewAssert(FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewCompExpr(Boolean, Microsoft.FSharp.Core.FSharpRef`1[System.Boolean], FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewConst(FSharp.Compiler.Syntax.SynConst, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewDiscardAfterMissingQualificationAfterDot(FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewDo(FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewDoBang(FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewDotGet(FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Text.Range, FSharp.Compiler.Syntax.LongIdentWithDots, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewDotIndexedGet(FSharp.Compiler.Syntax.SynExpr, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynIndexerArg], FSharp.Compiler.Text.Range, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewDotIndexedSet(FSharp.Compiler.Syntax.SynExpr, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynIndexerArg], FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Text.Range, FSharp.Compiler.Text.Range, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewDotNamedIndexedPropertySet(FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Syntax.LongIdentWithDots, FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewDotSet(FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Syntax.LongIdentWithDots, FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewDowncast(FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Syntax.SynType, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewFixed(FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewFor(FSharp.Compiler.Syntax.DebugPointAtFor, FSharp.Compiler.Syntax.Ident, FSharp.Compiler.Syntax.SynExpr, Boolean, FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewForEach(FSharp.Compiler.Syntax.DebugPointAtFor, FSharp.Compiler.Syntax.SeqExprOnly, Boolean, FSharp.Compiler.Syntax.SynPat, FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewFromParseError(FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewIdent(FSharp.Compiler.Syntax.Ident)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewIfThenElse(FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Syntax.SynExpr, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynExpr], FSharp.Compiler.Syntax.DebugPointAtBinding, Boolean, FSharp.Compiler.Text.Range, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewImplicitZero(FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewInferredDowncast(FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewInferredUpcast(FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewInterpolatedString(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynInterpolatedStringPart], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewJoinIn(FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Text.Range, FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewLambda(Boolean, Boolean, FSharp.Compiler.Syntax.SynSimplePats, FSharp.Compiler.Syntax.SynExpr, Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynPat],FSharp.Compiler.Syntax.SynExpr]], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewLazy(FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewLetOrUse(Boolean, Boolean, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynBinding], FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewLetOrUseBang(FSharp.Compiler.Syntax.DebugPointAtBinding, Boolean, Boolean, FSharp.Compiler.Syntax.SynPat, FSharp.Compiler.Syntax.SynExpr, Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`6[FSharp.Compiler.Syntax.DebugPointAtBinding,System.Boolean,System.Boolean,FSharp.Compiler.Syntax.SynPat,FSharp.Compiler.Syntax.SynExpr,FSharp.Compiler.Text.Range]], FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewLibraryOnlyILAssembly(System.Object, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynType], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynExpr], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynType], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewLibraryOnlyStaticOptimization(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynStaticOptimizationConstraint], FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewLibraryOnlyUnionCaseFieldGet(FSharp.Compiler.Syntax.SynExpr, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident], Int32, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewLibraryOnlyUnionCaseFieldSet(FSharp.Compiler.Syntax.SynExpr, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident], Int32, FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewLongIdent(Boolean, FSharp.Compiler.Syntax.LongIdentWithDots, Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.FSharpRef`1[FSharp.Compiler.Syntax.SynSimplePatAlternativeIdInfo]], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewLongIdentSet(FSharp.Compiler.Syntax.LongIdentWithDots, FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewMatch(FSharp.Compiler.Syntax.DebugPointAtBinding, FSharp.Compiler.Syntax.SynExpr, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynMatchClause], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewMatchBang(FSharp.Compiler.Syntax.DebugPointAtBinding, FSharp.Compiler.Syntax.SynExpr, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynMatchClause], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewMatchLambda(Boolean, FSharp.Compiler.Text.Range, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynMatchClause], FSharp.Compiler.Syntax.DebugPointAtBinding, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewNamedIndexedPropertySet(FSharp.Compiler.Syntax.LongIdentWithDots, FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewNew(Boolean, FSharp.Compiler.Syntax.SynType, FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewNull(FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewObjExpr(FSharp.Compiler.Syntax.SynType, Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.Syntax.SynExpr,Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.Ident]]], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynBinding], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynInterfaceImpl], FSharp.Compiler.Text.Range, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewParen(FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Text.Range, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewQuote(FSharp.Compiler.Syntax.SynExpr, Boolean, FSharp.Compiler.Syntax.SynExpr, Boolean, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewRecord(Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`5[FSharp.Compiler.Syntax.SynType,FSharp.Compiler.Syntax.SynExpr,FSharp.Compiler.Text.Range,Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.Text.Range,Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Position]]],FSharp.Compiler.Text.Range]], Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.Syntax.SynExpr,System.Tuple`2[FSharp.Compiler.Text.Range,Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Position]]]], Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`3[System.Tuple`2[FSharp.Compiler.Syntax.LongIdentWithDots,System.Boolean],Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynExpr],Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.Text.Range,Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Position]]]]], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewSequential(FSharp.Compiler.Syntax.DebugPointAtSequential, Boolean, FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewSequentialOrImplicitYield(FSharp.Compiler.Syntax.DebugPointAtSequential, FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewSet(FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewTraitCall(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynTypar], FSharp.Compiler.Syntax.SynMemberSig, FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewTryFinally(FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Text.Range, FSharp.Compiler.Syntax.DebugPointAtTry, FSharp.Compiler.Syntax.DebugPointAtFinally)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewTryWith(FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Text.Range, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynMatchClause], FSharp.Compiler.Text.Range, FSharp.Compiler.Text.Range, FSharp.Compiler.Syntax.DebugPointAtTry, FSharp.Compiler.Syntax.DebugPointAtWith)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewTuple(Boolean, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynExpr], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Text.Range], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewTypeApp(FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Text.Range, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynType], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Text.Range], Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range], FSharp.Compiler.Text.Range, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewTypeTest(FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Syntax.SynType, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewTyped(FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Syntax.SynType, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewUpcast(FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Syntax.SynType, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewWhile(FSharp.Compiler.Syntax.DebugPointAtWhile, FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewYieldOrReturn(System.Tuple`2[System.Boolean,System.Boolean], FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr NewYieldOrReturnFrom(System.Tuple`2[System.Boolean,System.Boolean], FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+AddressOf
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+AnonRecd
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+App
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+ArbitraryAfterError
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+ArrayOrList
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+ArrayOrListOfSeqExpr
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+Assert
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+CompExpr
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+Const
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+DiscardAfterMissingQualificationAfterDot
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+Do
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+DoBang
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+DotGet
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+DotIndexedGet
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+DotIndexedSet
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+DotNamedIndexedPropertySet
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+DotSet
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+Downcast
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+Fixed
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+For
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+ForEach
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+FromParseError
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+Ident
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+IfThenElse
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+ImplicitZero
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+InferredDowncast
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+InferredUpcast
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+InterpolatedString
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+JoinIn
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+Lambda
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+Lazy
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+LetOrUse
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+LetOrUseBang
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+LibraryOnlyILAssembly
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+LibraryOnlyStaticOptimization
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+LibraryOnlyUnionCaseFieldGet
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+LibraryOnlyUnionCaseFieldSet
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+LongIdent
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+LongIdentSet
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+Match
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+MatchBang
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+MatchLambda
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+NamedIndexedPropertySet
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+New
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+Null
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+ObjExpr
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+Paren
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+Quote
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+Record
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+Sequential
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+SequentialOrImplicitYield
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+Set
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+Tags
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+TraitCall
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+TryFinally
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+TryWith
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+Tuple
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+TypeApp
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+TypeTest
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+Typed
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+Upcast
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+While
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+YieldOrReturn
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Syntax.SynExpr+YieldOrReturnFrom
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Text.Range Range
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Text.Range RangeOfFirstPortion
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Text.Range RangeSansAnyExtraDot
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Text.Range get_RangeOfFirstPortion()
FSharp.Compiler.Syntax.SynExpr: FSharp.Compiler.Text.Range get_RangeSansAnyExtraDot()
FSharp.Compiler.Syntax.SynExpr: Int32 Tag
FSharp.Compiler.Syntax.SynExpr: Int32 get_Tag()
FSharp.Compiler.Syntax.SynExpr: System.String ToString()
FSharp.Compiler.Syntax.SynField
FSharp.Compiler.Syntax.SynField: Boolean get_isMutable()
FSharp.Compiler.Syntax.SynField: Boolean get_isStatic()
FSharp.Compiler.Syntax.SynField: Boolean isMutable
FSharp.Compiler.Syntax.SynField: Boolean isStatic
FSharp.Compiler.Syntax.SynField: FSharp.Compiler.Syntax.PreXmlDoc get_xmlDoc()
FSharp.Compiler.Syntax.SynField: FSharp.Compiler.Syntax.PreXmlDoc xmlDoc
FSharp.Compiler.Syntax.SynField: FSharp.Compiler.Syntax.SynField NewField(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList], Boolean, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.Ident], FSharp.Compiler.Syntax.SynType, Boolean, FSharp.Compiler.Syntax.PreXmlDoc, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynField: FSharp.Compiler.Syntax.SynType fieldType
FSharp.Compiler.Syntax.SynField: FSharp.Compiler.Syntax.SynType get_fieldType()
FSharp.Compiler.Syntax.SynField: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynField: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynField: Int32 Tag
FSharp.Compiler.Syntax.SynField: Int32 get_Tag()
FSharp.Compiler.Syntax.SynField: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList] attributes
FSharp.Compiler.Syntax.SynField: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList] get_attributes()
FSharp.Compiler.Syntax.SynField: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.Ident] get_idOpt()
FSharp.Compiler.Syntax.SynField: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.Ident] idOpt
FSharp.Compiler.Syntax.SynField: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess] accessibility
FSharp.Compiler.Syntax.SynField: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess] get_accessibility()
FSharp.Compiler.Syntax.SynField: System.String ToString()
FSharp.Compiler.Syntax.SynIndexerArg
FSharp.Compiler.Syntax.SynIndexerArg+One: Boolean fromEnd
FSharp.Compiler.Syntax.SynIndexerArg+One: Boolean get_fromEnd()
FSharp.Compiler.Syntax.SynIndexerArg+One: FSharp.Compiler.Syntax.SynExpr expr
FSharp.Compiler.Syntax.SynIndexerArg+One: FSharp.Compiler.Syntax.SynExpr get_expr()
FSharp.Compiler.Syntax.SynIndexerArg+One: FSharp.Compiler.Text.Range Item3
FSharp.Compiler.Syntax.SynIndexerArg+One: FSharp.Compiler.Text.Range get_Item3()
FSharp.Compiler.Syntax.SynIndexerArg+Tags: Int32 One
FSharp.Compiler.Syntax.SynIndexerArg+Tags: Int32 Two
FSharp.Compiler.Syntax.SynIndexerArg+Two: Boolean fromEnd1
FSharp.Compiler.Syntax.SynIndexerArg+Two: Boolean fromEnd2
FSharp.Compiler.Syntax.SynIndexerArg+Two: Boolean get_fromEnd1()
FSharp.Compiler.Syntax.SynIndexerArg+Two: Boolean get_fromEnd2()
FSharp.Compiler.Syntax.SynIndexerArg+Two: FSharp.Compiler.Syntax.SynExpr expr1
FSharp.Compiler.Syntax.SynIndexerArg+Two: FSharp.Compiler.Syntax.SynExpr expr2
FSharp.Compiler.Syntax.SynIndexerArg+Two: FSharp.Compiler.Syntax.SynExpr get_expr1()
FSharp.Compiler.Syntax.SynIndexerArg+Two: FSharp.Compiler.Syntax.SynExpr get_expr2()
FSharp.Compiler.Syntax.SynIndexerArg+Two: FSharp.Compiler.Text.Range get_range1()
FSharp.Compiler.Syntax.SynIndexerArg+Two: FSharp.Compiler.Text.Range get_range2()
FSharp.Compiler.Syntax.SynIndexerArg+Two: FSharp.Compiler.Text.Range range1
FSharp.Compiler.Syntax.SynIndexerArg+Two: FSharp.Compiler.Text.Range range2
FSharp.Compiler.Syntax.SynIndexerArg: Boolean IsOne
FSharp.Compiler.Syntax.SynIndexerArg: Boolean IsTwo
FSharp.Compiler.Syntax.SynIndexerArg: Boolean get_IsOne()
FSharp.Compiler.Syntax.SynIndexerArg: Boolean get_IsTwo()
FSharp.Compiler.Syntax.SynIndexerArg: FSharp.Compiler.Syntax.SynIndexerArg NewOne(FSharp.Compiler.Syntax.SynExpr, Boolean, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynIndexerArg: FSharp.Compiler.Syntax.SynIndexerArg NewTwo(FSharp.Compiler.Syntax.SynExpr, Boolean, FSharp.Compiler.Syntax.SynExpr, Boolean, FSharp.Compiler.Text.Range, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynIndexerArg: FSharp.Compiler.Syntax.SynIndexerArg+One
FSharp.Compiler.Syntax.SynIndexerArg: FSharp.Compiler.Syntax.SynIndexerArg+Tags
FSharp.Compiler.Syntax.SynIndexerArg: FSharp.Compiler.Syntax.SynIndexerArg+Two
FSharp.Compiler.Syntax.SynIndexerArg: FSharp.Compiler.Text.Range Range
FSharp.Compiler.Syntax.SynIndexerArg: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.Syntax.SynIndexerArg: Int32 Tag
FSharp.Compiler.Syntax.SynIndexerArg: Int32 get_Tag()
FSharp.Compiler.Syntax.SynIndexerArg: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynExpr] Exprs
FSharp.Compiler.Syntax.SynIndexerArg: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynExpr] get_Exprs()
FSharp.Compiler.Syntax.SynIndexerArg: System.String ToString()
FSharp.Compiler.Syntax.SynInterfaceImpl
FSharp.Compiler.Syntax.SynInterfaceImpl: FSharp.Compiler.Syntax.SynInterfaceImpl NewInterfaceImpl(FSharp.Compiler.Syntax.SynType, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynBinding], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynInterfaceImpl: FSharp.Compiler.Syntax.SynType get_interfaceTy()
FSharp.Compiler.Syntax.SynInterfaceImpl: FSharp.Compiler.Syntax.SynType interfaceTy
FSharp.Compiler.Syntax.SynInterfaceImpl: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynInterfaceImpl: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynInterfaceImpl: Int32 Tag
FSharp.Compiler.Syntax.SynInterfaceImpl: Int32 get_Tag()
FSharp.Compiler.Syntax.SynInterfaceImpl: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynBinding] bindings
FSharp.Compiler.Syntax.SynInterfaceImpl: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynBinding] get_bindings()
FSharp.Compiler.Syntax.SynInterfaceImpl: System.String ToString()
FSharp.Compiler.Syntax.SynInterpolatedStringPart
FSharp.Compiler.Syntax.SynInterpolatedStringPart+FillExpr: FSharp.Compiler.Syntax.SynExpr fillExpr
FSharp.Compiler.Syntax.SynInterpolatedStringPart+FillExpr: FSharp.Compiler.Syntax.SynExpr get_fillExpr()
FSharp.Compiler.Syntax.SynInterpolatedStringPart+FillExpr: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.Ident] get_qualifiers()
FSharp.Compiler.Syntax.SynInterpolatedStringPart+FillExpr: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.Ident] qualifiers
FSharp.Compiler.Syntax.SynInterpolatedStringPart+String: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynInterpolatedStringPart+String: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynInterpolatedStringPart+String: System.String get_value()
FSharp.Compiler.Syntax.SynInterpolatedStringPart+String: System.String value
FSharp.Compiler.Syntax.SynInterpolatedStringPart+Tags: Int32 FillExpr
FSharp.Compiler.Syntax.SynInterpolatedStringPart+Tags: Int32 String
FSharp.Compiler.Syntax.SynInterpolatedStringPart: Boolean IsFillExpr
FSharp.Compiler.Syntax.SynInterpolatedStringPart: Boolean IsString
FSharp.Compiler.Syntax.SynInterpolatedStringPart: Boolean get_IsFillExpr()
FSharp.Compiler.Syntax.SynInterpolatedStringPart: Boolean get_IsString()
FSharp.Compiler.Syntax.SynInterpolatedStringPart: FSharp.Compiler.Syntax.SynInterpolatedStringPart NewFillExpr(FSharp.Compiler.Syntax.SynExpr, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.Ident])
FSharp.Compiler.Syntax.SynInterpolatedStringPart: FSharp.Compiler.Syntax.SynInterpolatedStringPart NewString(System.String, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynInterpolatedStringPart: FSharp.Compiler.Syntax.SynInterpolatedStringPart+FillExpr
FSharp.Compiler.Syntax.SynInterpolatedStringPart: FSharp.Compiler.Syntax.SynInterpolatedStringPart+String
FSharp.Compiler.Syntax.SynInterpolatedStringPart: FSharp.Compiler.Syntax.SynInterpolatedStringPart+Tags
FSharp.Compiler.Syntax.SynInterpolatedStringPart: Int32 Tag
FSharp.Compiler.Syntax.SynInterpolatedStringPart: Int32 get_Tag()
FSharp.Compiler.Syntax.SynInterpolatedStringPart: System.String ToString()
FSharp.Compiler.Syntax.SynMatchClause
FSharp.Compiler.Syntax.SynMatchClause: FSharp.Compiler.Syntax.DebugPointForTarget get_spInfo()
FSharp.Compiler.Syntax.SynMatchClause: FSharp.Compiler.Syntax.DebugPointForTarget spInfo
FSharp.Compiler.Syntax.SynMatchClause: FSharp.Compiler.Syntax.SynExpr get_resultExpr()
FSharp.Compiler.Syntax.SynMatchClause: FSharp.Compiler.Syntax.SynExpr resultExpr
FSharp.Compiler.Syntax.SynMatchClause: FSharp.Compiler.Syntax.SynMatchClause NewClause(FSharp.Compiler.Syntax.SynPat, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynExpr], FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Text.Range, FSharp.Compiler.Syntax.DebugPointForTarget)
FSharp.Compiler.Syntax.SynMatchClause: FSharp.Compiler.Syntax.SynPat get_pat()
FSharp.Compiler.Syntax.SynMatchClause: FSharp.Compiler.Syntax.SynPat pat
FSharp.Compiler.Syntax.SynMatchClause: FSharp.Compiler.Text.Range Range
FSharp.Compiler.Syntax.SynMatchClause: FSharp.Compiler.Text.Range RangeOfGuardAndRhs
FSharp.Compiler.Syntax.SynMatchClause: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.Syntax.SynMatchClause: FSharp.Compiler.Text.Range get_RangeOfGuardAndRhs()
FSharp.Compiler.Syntax.SynMatchClause: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynMatchClause: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynMatchClause: Int32 Tag
FSharp.Compiler.Syntax.SynMatchClause: Int32 get_Tag()
FSharp.Compiler.Syntax.SynMatchClause: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynExpr] get_whenExpr()
FSharp.Compiler.Syntax.SynMatchClause: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynExpr] whenExpr
FSharp.Compiler.Syntax.SynMatchClause: System.String ToString()
FSharp.Compiler.Syntax.SynMeasure
FSharp.Compiler.Syntax.SynMeasure+Anon: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynMeasure+Anon: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynMeasure+Divide: FSharp.Compiler.Syntax.SynMeasure get_measure1()
FSharp.Compiler.Syntax.SynMeasure+Divide: FSharp.Compiler.Syntax.SynMeasure get_measure2()
FSharp.Compiler.Syntax.SynMeasure+Divide: FSharp.Compiler.Syntax.SynMeasure measure1
FSharp.Compiler.Syntax.SynMeasure+Divide: FSharp.Compiler.Syntax.SynMeasure measure2
FSharp.Compiler.Syntax.SynMeasure+Divide: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynMeasure+Divide: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynMeasure+Named: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynMeasure+Named: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynMeasure+Named: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident] get_longId()
FSharp.Compiler.Syntax.SynMeasure+Named: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident] longId
FSharp.Compiler.Syntax.SynMeasure+Power: FSharp.Compiler.Syntax.SynMeasure get_measure()
FSharp.Compiler.Syntax.SynMeasure+Power: FSharp.Compiler.Syntax.SynMeasure measure
FSharp.Compiler.Syntax.SynMeasure+Power: FSharp.Compiler.Syntax.SynRationalConst get_power()
FSharp.Compiler.Syntax.SynMeasure+Power: FSharp.Compiler.Syntax.SynRationalConst power
FSharp.Compiler.Syntax.SynMeasure+Power: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynMeasure+Power: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynMeasure+Product: FSharp.Compiler.Syntax.SynMeasure get_measure1()
FSharp.Compiler.Syntax.SynMeasure+Product: FSharp.Compiler.Syntax.SynMeasure get_measure2()
FSharp.Compiler.Syntax.SynMeasure+Product: FSharp.Compiler.Syntax.SynMeasure measure1
FSharp.Compiler.Syntax.SynMeasure+Product: FSharp.Compiler.Syntax.SynMeasure measure2
FSharp.Compiler.Syntax.SynMeasure+Product: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynMeasure+Product: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynMeasure+Seq: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynMeasure+Seq: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynMeasure+Seq: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynMeasure] get_measures()
FSharp.Compiler.Syntax.SynMeasure+Seq: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynMeasure] measures
FSharp.Compiler.Syntax.SynMeasure+Tags: Int32 Anon
FSharp.Compiler.Syntax.SynMeasure+Tags: Int32 Divide
FSharp.Compiler.Syntax.SynMeasure+Tags: Int32 Named
FSharp.Compiler.Syntax.SynMeasure+Tags: Int32 One
FSharp.Compiler.Syntax.SynMeasure+Tags: Int32 Power
FSharp.Compiler.Syntax.SynMeasure+Tags: Int32 Product
FSharp.Compiler.Syntax.SynMeasure+Tags: Int32 Seq
FSharp.Compiler.Syntax.SynMeasure+Tags: Int32 Var
FSharp.Compiler.Syntax.SynMeasure+Var: FSharp.Compiler.Syntax.SynTypar get_typar()
FSharp.Compiler.Syntax.SynMeasure+Var: FSharp.Compiler.Syntax.SynTypar typar
FSharp.Compiler.Syntax.SynMeasure+Var: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynMeasure+Var: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynMeasure: Boolean IsAnon
FSharp.Compiler.Syntax.SynMeasure: Boolean IsDivide
FSharp.Compiler.Syntax.SynMeasure: Boolean IsNamed
FSharp.Compiler.Syntax.SynMeasure: Boolean IsOne
FSharp.Compiler.Syntax.SynMeasure: Boolean IsPower
FSharp.Compiler.Syntax.SynMeasure: Boolean IsProduct
FSharp.Compiler.Syntax.SynMeasure: Boolean IsSeq
FSharp.Compiler.Syntax.SynMeasure: Boolean IsVar
FSharp.Compiler.Syntax.SynMeasure: Boolean get_IsAnon()
FSharp.Compiler.Syntax.SynMeasure: Boolean get_IsDivide()
FSharp.Compiler.Syntax.SynMeasure: Boolean get_IsNamed()
FSharp.Compiler.Syntax.SynMeasure: Boolean get_IsOne()
FSharp.Compiler.Syntax.SynMeasure: Boolean get_IsPower()
FSharp.Compiler.Syntax.SynMeasure: Boolean get_IsProduct()
FSharp.Compiler.Syntax.SynMeasure: Boolean get_IsSeq()
FSharp.Compiler.Syntax.SynMeasure: Boolean get_IsVar()
FSharp.Compiler.Syntax.SynMeasure: FSharp.Compiler.Syntax.SynMeasure NewAnon(FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynMeasure: FSharp.Compiler.Syntax.SynMeasure NewDivide(FSharp.Compiler.Syntax.SynMeasure, FSharp.Compiler.Syntax.SynMeasure, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynMeasure: FSharp.Compiler.Syntax.SynMeasure NewNamed(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynMeasure: FSharp.Compiler.Syntax.SynMeasure NewPower(FSharp.Compiler.Syntax.SynMeasure, FSharp.Compiler.Syntax.SynRationalConst, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynMeasure: FSharp.Compiler.Syntax.SynMeasure NewProduct(FSharp.Compiler.Syntax.SynMeasure, FSharp.Compiler.Syntax.SynMeasure, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynMeasure: FSharp.Compiler.Syntax.SynMeasure NewSeq(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynMeasure], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynMeasure: FSharp.Compiler.Syntax.SynMeasure NewVar(FSharp.Compiler.Syntax.SynTypar, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynMeasure: FSharp.Compiler.Syntax.SynMeasure One
FSharp.Compiler.Syntax.SynMeasure: FSharp.Compiler.Syntax.SynMeasure get_One()
FSharp.Compiler.Syntax.SynMeasure: FSharp.Compiler.Syntax.SynMeasure+Anon
FSharp.Compiler.Syntax.SynMeasure: FSharp.Compiler.Syntax.SynMeasure+Divide
FSharp.Compiler.Syntax.SynMeasure: FSharp.Compiler.Syntax.SynMeasure+Named
FSharp.Compiler.Syntax.SynMeasure: FSharp.Compiler.Syntax.SynMeasure+Power
FSharp.Compiler.Syntax.SynMeasure: FSharp.Compiler.Syntax.SynMeasure+Product
FSharp.Compiler.Syntax.SynMeasure: FSharp.Compiler.Syntax.SynMeasure+Seq
FSharp.Compiler.Syntax.SynMeasure: FSharp.Compiler.Syntax.SynMeasure+Tags
FSharp.Compiler.Syntax.SynMeasure: FSharp.Compiler.Syntax.SynMeasure+Var
FSharp.Compiler.Syntax.SynMeasure: Int32 Tag
FSharp.Compiler.Syntax.SynMeasure: Int32 get_Tag()
FSharp.Compiler.Syntax.SynMeasure: System.String ToString()
FSharp.Compiler.Syntax.SynMemberDefn
FSharp.Compiler.Syntax.SynMemberDefn+AbstractSlot: FSharp.Compiler.Syntax.MemberFlags flags
FSharp.Compiler.Syntax.SynMemberDefn+AbstractSlot: FSharp.Compiler.Syntax.MemberFlags get_flags()
FSharp.Compiler.Syntax.SynMemberDefn+AbstractSlot: FSharp.Compiler.Syntax.SynValSig get_slotSig()
FSharp.Compiler.Syntax.SynMemberDefn+AbstractSlot: FSharp.Compiler.Syntax.SynValSig slotSig
FSharp.Compiler.Syntax.SynMemberDefn+AbstractSlot: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynMemberDefn+AbstractSlot: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynMemberDefn+AutoProperty: Boolean get_isStatic()
FSharp.Compiler.Syntax.SynMemberDefn+AutoProperty: Boolean isStatic
FSharp.Compiler.Syntax.SynMemberDefn+AutoProperty: FSharp.Compiler.Syntax.Ident get_ident()
FSharp.Compiler.Syntax.SynMemberDefn+AutoProperty: FSharp.Compiler.Syntax.Ident ident
FSharp.Compiler.Syntax.SynMemberDefn+AutoProperty: FSharp.Compiler.Syntax.MemberKind get_propKind()
FSharp.Compiler.Syntax.SynMemberDefn+AutoProperty: FSharp.Compiler.Syntax.MemberKind propKind
FSharp.Compiler.Syntax.SynMemberDefn+AutoProperty: FSharp.Compiler.Syntax.PreXmlDoc get_xmlDoc()
FSharp.Compiler.Syntax.SynMemberDefn+AutoProperty: FSharp.Compiler.Syntax.PreXmlDoc xmlDoc
FSharp.Compiler.Syntax.SynMemberDefn+AutoProperty: FSharp.Compiler.Syntax.SynExpr get_synExpr()
FSharp.Compiler.Syntax.SynMemberDefn+AutoProperty: FSharp.Compiler.Syntax.SynExpr synExpr
FSharp.Compiler.Syntax.SynMemberDefn+AutoProperty: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynMemberDefn+AutoProperty: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynMemberDefn+AutoProperty: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList] attributes
FSharp.Compiler.Syntax.SynMemberDefn+AutoProperty: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList] get_attributes()
FSharp.Compiler.Syntax.SynMemberDefn+AutoProperty: Microsoft.FSharp.Core.FSharpFunc`2[FSharp.Compiler.Syntax.MemberKind,FSharp.Compiler.Syntax.MemberFlags] get_memberFlags()
FSharp.Compiler.Syntax.SynMemberDefn+AutoProperty: Microsoft.FSharp.Core.FSharpFunc`2[FSharp.Compiler.Syntax.MemberKind,FSharp.Compiler.Syntax.MemberFlags] memberFlags
FSharp.Compiler.Syntax.SynMemberDefn+AutoProperty: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess] accessibility
FSharp.Compiler.Syntax.SynMemberDefn+AutoProperty: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess] get_accessibility()
FSharp.Compiler.Syntax.SynMemberDefn+AutoProperty: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynType] get_typeOpt()
FSharp.Compiler.Syntax.SynMemberDefn+AutoProperty: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynType] typeOpt
FSharp.Compiler.Syntax.SynMemberDefn+AutoProperty: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] getSetRange
FSharp.Compiler.Syntax.SynMemberDefn+AutoProperty: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] get_getSetRange()
FSharp.Compiler.Syntax.SynMemberDefn+ImplicitCtor: FSharp.Compiler.Syntax.PreXmlDoc get_xmlDoc()
FSharp.Compiler.Syntax.SynMemberDefn+ImplicitCtor: FSharp.Compiler.Syntax.PreXmlDoc xmlDoc
FSharp.Compiler.Syntax.SynMemberDefn+ImplicitCtor: FSharp.Compiler.Syntax.SynSimplePats ctorArgs
FSharp.Compiler.Syntax.SynMemberDefn+ImplicitCtor: FSharp.Compiler.Syntax.SynSimplePats get_ctorArgs()
FSharp.Compiler.Syntax.SynMemberDefn+ImplicitCtor: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynMemberDefn+ImplicitCtor: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynMemberDefn+ImplicitCtor: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList] attributes
FSharp.Compiler.Syntax.SynMemberDefn+ImplicitCtor: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList] get_attributes()
FSharp.Compiler.Syntax.SynMemberDefn+ImplicitCtor: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.Ident] get_selfIdentifier()
FSharp.Compiler.Syntax.SynMemberDefn+ImplicitCtor: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.Ident] selfIdentifier
FSharp.Compiler.Syntax.SynMemberDefn+ImplicitCtor: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess] accessibility
FSharp.Compiler.Syntax.SynMemberDefn+ImplicitCtor: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess] get_accessibility()
FSharp.Compiler.Syntax.SynMemberDefn+ImplicitInherit: FSharp.Compiler.Syntax.SynExpr get_inheritArgs()
FSharp.Compiler.Syntax.SynMemberDefn+ImplicitInherit: FSharp.Compiler.Syntax.SynExpr inheritArgs
FSharp.Compiler.Syntax.SynMemberDefn+ImplicitInherit: FSharp.Compiler.Syntax.SynType get_inheritType()
FSharp.Compiler.Syntax.SynMemberDefn+ImplicitInherit: FSharp.Compiler.Syntax.SynType inheritType
FSharp.Compiler.Syntax.SynMemberDefn+ImplicitInherit: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynMemberDefn+ImplicitInherit: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynMemberDefn+ImplicitInherit: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.Ident] get_inheritAlias()
FSharp.Compiler.Syntax.SynMemberDefn+ImplicitInherit: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.Ident] inheritAlias
FSharp.Compiler.Syntax.SynMemberDefn+Inherit: FSharp.Compiler.Syntax.SynType baseType
FSharp.Compiler.Syntax.SynMemberDefn+Inherit: FSharp.Compiler.Syntax.SynType get_baseType()
FSharp.Compiler.Syntax.SynMemberDefn+Inherit: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynMemberDefn+Inherit: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynMemberDefn+Inherit: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.Ident] asIdent
FSharp.Compiler.Syntax.SynMemberDefn+Inherit: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.Ident] get_asIdent()
FSharp.Compiler.Syntax.SynMemberDefn+Interface: FSharp.Compiler.Syntax.SynType get_interfaceType()
FSharp.Compiler.Syntax.SynMemberDefn+Interface: FSharp.Compiler.Syntax.SynType interfaceType
FSharp.Compiler.Syntax.SynMemberDefn+Interface: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynMemberDefn+Interface: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynMemberDefn+Interface: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynMemberDefn]] get_members()
FSharp.Compiler.Syntax.SynMemberDefn+Interface: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynMemberDefn]] members
FSharp.Compiler.Syntax.SynMemberDefn+LetBindings: Boolean get_isRecursive()
FSharp.Compiler.Syntax.SynMemberDefn+LetBindings: Boolean get_isStatic()
FSharp.Compiler.Syntax.SynMemberDefn+LetBindings: Boolean isRecursive
FSharp.Compiler.Syntax.SynMemberDefn+LetBindings: Boolean isStatic
FSharp.Compiler.Syntax.SynMemberDefn+LetBindings: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynMemberDefn+LetBindings: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynMemberDefn+LetBindings: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynBinding] bindings
FSharp.Compiler.Syntax.SynMemberDefn+LetBindings: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynBinding] get_bindings()
FSharp.Compiler.Syntax.SynMemberDefn+Member: FSharp.Compiler.Syntax.SynBinding get_memberDefn()
FSharp.Compiler.Syntax.SynMemberDefn+Member: FSharp.Compiler.Syntax.SynBinding memberDefn
FSharp.Compiler.Syntax.SynMemberDefn+Member: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynMemberDefn+Member: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynMemberDefn+NestedType: FSharp.Compiler.Syntax.SynTypeDefn get_typeDefn()
FSharp.Compiler.Syntax.SynMemberDefn+NestedType: FSharp.Compiler.Syntax.SynTypeDefn typeDefn
FSharp.Compiler.Syntax.SynMemberDefn+NestedType: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynMemberDefn+NestedType: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynMemberDefn+NestedType: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess] accessibility
FSharp.Compiler.Syntax.SynMemberDefn+NestedType: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess] get_accessibility()
FSharp.Compiler.Syntax.SynMemberDefn+Open: FSharp.Compiler.Syntax.SynOpenDeclTarget get_target()
FSharp.Compiler.Syntax.SynMemberDefn+Open: FSharp.Compiler.Syntax.SynOpenDeclTarget target
FSharp.Compiler.Syntax.SynMemberDefn+Open: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynMemberDefn+Open: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynMemberDefn+Tags: Int32 AbstractSlot
FSharp.Compiler.Syntax.SynMemberDefn+Tags: Int32 AutoProperty
FSharp.Compiler.Syntax.SynMemberDefn+Tags: Int32 ImplicitCtor
FSharp.Compiler.Syntax.SynMemberDefn+Tags: Int32 ImplicitInherit
FSharp.Compiler.Syntax.SynMemberDefn+Tags: Int32 Inherit
FSharp.Compiler.Syntax.SynMemberDefn+Tags: Int32 Interface
FSharp.Compiler.Syntax.SynMemberDefn+Tags: Int32 LetBindings
FSharp.Compiler.Syntax.SynMemberDefn+Tags: Int32 Member
FSharp.Compiler.Syntax.SynMemberDefn+Tags: Int32 NestedType
FSharp.Compiler.Syntax.SynMemberDefn+Tags: Int32 Open
FSharp.Compiler.Syntax.SynMemberDefn+Tags: Int32 ValField
FSharp.Compiler.Syntax.SynMemberDefn+ValField: FSharp.Compiler.Syntax.SynField fieldInfo
FSharp.Compiler.Syntax.SynMemberDefn+ValField: FSharp.Compiler.Syntax.SynField get_fieldInfo()
FSharp.Compiler.Syntax.SynMemberDefn+ValField: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynMemberDefn+ValField: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynMemberDefn: Boolean IsAbstractSlot
FSharp.Compiler.Syntax.SynMemberDefn: Boolean IsAutoProperty
FSharp.Compiler.Syntax.SynMemberDefn: Boolean IsImplicitCtor
FSharp.Compiler.Syntax.SynMemberDefn: Boolean IsImplicitInherit
FSharp.Compiler.Syntax.SynMemberDefn: Boolean IsInherit
FSharp.Compiler.Syntax.SynMemberDefn: Boolean IsInterface
FSharp.Compiler.Syntax.SynMemberDefn: Boolean IsLetBindings
FSharp.Compiler.Syntax.SynMemberDefn: Boolean IsMember
FSharp.Compiler.Syntax.SynMemberDefn: Boolean IsNestedType
FSharp.Compiler.Syntax.SynMemberDefn: Boolean IsOpen
FSharp.Compiler.Syntax.SynMemberDefn: Boolean IsValField
FSharp.Compiler.Syntax.SynMemberDefn: Boolean get_IsAbstractSlot()
FSharp.Compiler.Syntax.SynMemberDefn: Boolean get_IsAutoProperty()
FSharp.Compiler.Syntax.SynMemberDefn: Boolean get_IsImplicitCtor()
FSharp.Compiler.Syntax.SynMemberDefn: Boolean get_IsImplicitInherit()
FSharp.Compiler.Syntax.SynMemberDefn: Boolean get_IsInherit()
FSharp.Compiler.Syntax.SynMemberDefn: Boolean get_IsInterface()
FSharp.Compiler.Syntax.SynMemberDefn: Boolean get_IsLetBindings()
FSharp.Compiler.Syntax.SynMemberDefn: Boolean get_IsMember()
FSharp.Compiler.Syntax.SynMemberDefn: Boolean get_IsNestedType()
FSharp.Compiler.Syntax.SynMemberDefn: Boolean get_IsOpen()
FSharp.Compiler.Syntax.SynMemberDefn: Boolean get_IsValField()
FSharp.Compiler.Syntax.SynMemberDefn: FSharp.Compiler.Syntax.SynMemberDefn NewAbstractSlot(FSharp.Compiler.Syntax.SynValSig, FSharp.Compiler.Syntax.MemberFlags, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynMemberDefn: FSharp.Compiler.Syntax.SynMemberDefn NewAutoProperty(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList], Boolean, FSharp.Compiler.Syntax.Ident, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynType], FSharp.Compiler.Syntax.MemberKind, Microsoft.FSharp.Core.FSharpFunc`2[FSharp.Compiler.Syntax.MemberKind,FSharp.Compiler.Syntax.MemberFlags], FSharp.Compiler.Syntax.PreXmlDoc, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess], FSharp.Compiler.Syntax.SynExpr, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynMemberDefn: FSharp.Compiler.Syntax.SynMemberDefn NewImplicitCtor(Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList], FSharp.Compiler.Syntax.SynSimplePats, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.Ident], FSharp.Compiler.Syntax.PreXmlDoc, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynMemberDefn: FSharp.Compiler.Syntax.SynMemberDefn NewImplicitInherit(FSharp.Compiler.Syntax.SynType, FSharp.Compiler.Syntax.SynExpr, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.Ident], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynMemberDefn: FSharp.Compiler.Syntax.SynMemberDefn NewInherit(FSharp.Compiler.Syntax.SynType, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.Ident], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynMemberDefn: FSharp.Compiler.Syntax.SynMemberDefn NewInterface(FSharp.Compiler.Syntax.SynType, Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynMemberDefn]], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynMemberDefn: FSharp.Compiler.Syntax.SynMemberDefn NewLetBindings(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynBinding], Boolean, Boolean, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynMemberDefn: FSharp.Compiler.Syntax.SynMemberDefn NewMember(FSharp.Compiler.Syntax.SynBinding, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynMemberDefn: FSharp.Compiler.Syntax.SynMemberDefn NewNestedType(FSharp.Compiler.Syntax.SynTypeDefn, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynMemberDefn: FSharp.Compiler.Syntax.SynMemberDefn NewOpen(FSharp.Compiler.Syntax.SynOpenDeclTarget, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynMemberDefn: FSharp.Compiler.Syntax.SynMemberDefn NewValField(FSharp.Compiler.Syntax.SynField, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynMemberDefn: FSharp.Compiler.Syntax.SynMemberDefn+AbstractSlot
FSharp.Compiler.Syntax.SynMemberDefn: FSharp.Compiler.Syntax.SynMemberDefn+AutoProperty
FSharp.Compiler.Syntax.SynMemberDefn: FSharp.Compiler.Syntax.SynMemberDefn+ImplicitCtor
FSharp.Compiler.Syntax.SynMemberDefn: FSharp.Compiler.Syntax.SynMemberDefn+ImplicitInherit
FSharp.Compiler.Syntax.SynMemberDefn: FSharp.Compiler.Syntax.SynMemberDefn+Inherit
FSharp.Compiler.Syntax.SynMemberDefn: FSharp.Compiler.Syntax.SynMemberDefn+Interface
FSharp.Compiler.Syntax.SynMemberDefn: FSharp.Compiler.Syntax.SynMemberDefn+LetBindings
FSharp.Compiler.Syntax.SynMemberDefn: FSharp.Compiler.Syntax.SynMemberDefn+Member
FSharp.Compiler.Syntax.SynMemberDefn: FSharp.Compiler.Syntax.SynMemberDefn+NestedType
FSharp.Compiler.Syntax.SynMemberDefn: FSharp.Compiler.Syntax.SynMemberDefn+Open
FSharp.Compiler.Syntax.SynMemberDefn: FSharp.Compiler.Syntax.SynMemberDefn+Tags
FSharp.Compiler.Syntax.SynMemberDefn: FSharp.Compiler.Syntax.SynMemberDefn+ValField
FSharp.Compiler.Syntax.SynMemberDefn: FSharp.Compiler.Text.Range Range
FSharp.Compiler.Syntax.SynMemberDefn: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.Syntax.SynMemberDefn: Int32 Tag
FSharp.Compiler.Syntax.SynMemberDefn: Int32 get_Tag()
FSharp.Compiler.Syntax.SynMemberDefn: System.String ToString()
FSharp.Compiler.Syntax.SynMemberSig
FSharp.Compiler.Syntax.SynMemberSig+Inherit: FSharp.Compiler.Syntax.SynType get_inheritedType()
FSharp.Compiler.Syntax.SynMemberSig+Inherit: FSharp.Compiler.Syntax.SynType inheritedType
FSharp.Compiler.Syntax.SynMemberSig+Inherit: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynMemberSig+Inherit: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynMemberSig+Interface: FSharp.Compiler.Syntax.SynType get_interfaceType()
FSharp.Compiler.Syntax.SynMemberSig+Interface: FSharp.Compiler.Syntax.SynType interfaceType
FSharp.Compiler.Syntax.SynMemberSig+Interface: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynMemberSig+Interface: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynMemberSig+Member: FSharp.Compiler.Syntax.MemberFlags flags
FSharp.Compiler.Syntax.SynMemberSig+Member: FSharp.Compiler.Syntax.MemberFlags get_flags()
FSharp.Compiler.Syntax.SynMemberSig+Member: FSharp.Compiler.Syntax.SynValSig get_memberSig()
FSharp.Compiler.Syntax.SynMemberSig+Member: FSharp.Compiler.Syntax.SynValSig memberSig
FSharp.Compiler.Syntax.SynMemberSig+Member: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynMemberSig+Member: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynMemberSig+NestedType: FSharp.Compiler.Syntax.SynTypeDefnSig get_nestedType()
FSharp.Compiler.Syntax.SynMemberSig+NestedType: FSharp.Compiler.Syntax.SynTypeDefnSig nestedType
FSharp.Compiler.Syntax.SynMemberSig+NestedType: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynMemberSig+NestedType: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynMemberSig+Tags: Int32 Inherit
FSharp.Compiler.Syntax.SynMemberSig+Tags: Int32 Interface
FSharp.Compiler.Syntax.SynMemberSig+Tags: Int32 Member
FSharp.Compiler.Syntax.SynMemberSig+Tags: Int32 NestedType
FSharp.Compiler.Syntax.SynMemberSig+Tags: Int32 ValField
FSharp.Compiler.Syntax.SynMemberSig+ValField: FSharp.Compiler.Syntax.SynField field
FSharp.Compiler.Syntax.SynMemberSig+ValField: FSharp.Compiler.Syntax.SynField get_field()
FSharp.Compiler.Syntax.SynMemberSig+ValField: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynMemberSig+ValField: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynMemberSig: Boolean IsInherit
FSharp.Compiler.Syntax.SynMemberSig: Boolean IsInterface
FSharp.Compiler.Syntax.SynMemberSig: Boolean IsMember
FSharp.Compiler.Syntax.SynMemberSig: Boolean IsNestedType
FSharp.Compiler.Syntax.SynMemberSig: Boolean IsValField
FSharp.Compiler.Syntax.SynMemberSig: Boolean get_IsInherit()
FSharp.Compiler.Syntax.SynMemberSig: Boolean get_IsInterface()
FSharp.Compiler.Syntax.SynMemberSig: Boolean get_IsMember()
FSharp.Compiler.Syntax.SynMemberSig: Boolean get_IsNestedType()
FSharp.Compiler.Syntax.SynMemberSig: Boolean get_IsValField()
FSharp.Compiler.Syntax.SynMemberSig: FSharp.Compiler.Syntax.SynMemberSig NewInherit(FSharp.Compiler.Syntax.SynType, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynMemberSig: FSharp.Compiler.Syntax.SynMemberSig NewInterface(FSharp.Compiler.Syntax.SynType, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynMemberSig: FSharp.Compiler.Syntax.SynMemberSig NewMember(FSharp.Compiler.Syntax.SynValSig, FSharp.Compiler.Syntax.MemberFlags, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynMemberSig: FSharp.Compiler.Syntax.SynMemberSig NewNestedType(FSharp.Compiler.Syntax.SynTypeDefnSig, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynMemberSig: FSharp.Compiler.Syntax.SynMemberSig NewValField(FSharp.Compiler.Syntax.SynField, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynMemberSig: FSharp.Compiler.Syntax.SynMemberSig+Inherit
FSharp.Compiler.Syntax.SynMemberSig: FSharp.Compiler.Syntax.SynMemberSig+Interface
FSharp.Compiler.Syntax.SynMemberSig: FSharp.Compiler.Syntax.SynMemberSig+Member
FSharp.Compiler.Syntax.SynMemberSig: FSharp.Compiler.Syntax.SynMemberSig+NestedType
FSharp.Compiler.Syntax.SynMemberSig: FSharp.Compiler.Syntax.SynMemberSig+Tags
FSharp.Compiler.Syntax.SynMemberSig: FSharp.Compiler.Syntax.SynMemberSig+ValField
FSharp.Compiler.Syntax.SynMemberSig: Int32 Tag
FSharp.Compiler.Syntax.SynMemberSig: Int32 get_Tag()
FSharp.Compiler.Syntax.SynMemberSig: System.String ToString()
FSharp.Compiler.Syntax.SynModuleDecl
FSharp.Compiler.Syntax.SynModuleDecl+Attributes: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynModuleDecl+Attributes: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynModuleDecl+Attributes: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList] attributes
FSharp.Compiler.Syntax.SynModuleDecl+Attributes: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList] get_attributes()
FSharp.Compiler.Syntax.SynModuleDecl+DoExpr: FSharp.Compiler.Syntax.DebugPointAtBinding get_spInfo()
FSharp.Compiler.Syntax.SynModuleDecl+DoExpr: FSharp.Compiler.Syntax.DebugPointAtBinding spInfo
FSharp.Compiler.Syntax.SynModuleDecl+DoExpr: FSharp.Compiler.Syntax.SynExpr expr
FSharp.Compiler.Syntax.SynModuleDecl+DoExpr: FSharp.Compiler.Syntax.SynExpr get_expr()
FSharp.Compiler.Syntax.SynModuleDecl+DoExpr: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynModuleDecl+DoExpr: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynModuleDecl+Exception: FSharp.Compiler.Syntax.SynExceptionDefn exnDefn
FSharp.Compiler.Syntax.SynModuleDecl+Exception: FSharp.Compiler.Syntax.SynExceptionDefn get_exnDefn()
FSharp.Compiler.Syntax.SynModuleDecl+Exception: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynModuleDecl+Exception: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynModuleDecl+HashDirective: FSharp.Compiler.Syntax.ParsedHashDirective get_hashDirective()
FSharp.Compiler.Syntax.SynModuleDecl+HashDirective: FSharp.Compiler.Syntax.ParsedHashDirective hashDirective
FSharp.Compiler.Syntax.SynModuleDecl+HashDirective: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynModuleDecl+HashDirective: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynModuleDecl+Let: Boolean get_isRecursive()
FSharp.Compiler.Syntax.SynModuleDecl+Let: Boolean isRecursive
FSharp.Compiler.Syntax.SynModuleDecl+Let: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynModuleDecl+Let: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynModuleDecl+Let: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynBinding] bindings
FSharp.Compiler.Syntax.SynModuleDecl+Let: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynBinding] get_bindings()
FSharp.Compiler.Syntax.SynModuleDecl+ModuleAbbrev: FSharp.Compiler.Syntax.Ident get_ident()
FSharp.Compiler.Syntax.SynModuleDecl+ModuleAbbrev: FSharp.Compiler.Syntax.Ident ident
FSharp.Compiler.Syntax.SynModuleDecl+ModuleAbbrev: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynModuleDecl+ModuleAbbrev: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynModuleDecl+ModuleAbbrev: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident] get_longId()
FSharp.Compiler.Syntax.SynModuleDecl+ModuleAbbrev: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident] longId
FSharp.Compiler.Syntax.SynModuleDecl+NamespaceFragment: FSharp.Compiler.Syntax.SynModuleOrNamespace fragment
FSharp.Compiler.Syntax.SynModuleDecl+NamespaceFragment: FSharp.Compiler.Syntax.SynModuleOrNamespace get_fragment()
FSharp.Compiler.Syntax.SynModuleDecl+NestedModule: Boolean get_isContinuing()
FSharp.Compiler.Syntax.SynModuleDecl+NestedModule: Boolean get_isRecursive()
FSharp.Compiler.Syntax.SynModuleDecl+NestedModule: Boolean isContinuing
FSharp.Compiler.Syntax.SynModuleDecl+NestedModule: Boolean isRecursive
FSharp.Compiler.Syntax.SynModuleDecl+NestedModule: FSharp.Compiler.Syntax.SynComponentInfo get_moduleInfo()
FSharp.Compiler.Syntax.SynModuleDecl+NestedModule: FSharp.Compiler.Syntax.SynComponentInfo moduleInfo
FSharp.Compiler.Syntax.SynModuleDecl+NestedModule: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynModuleDecl+NestedModule: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynModuleDecl+NestedModule: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynModuleDecl] decls
FSharp.Compiler.Syntax.SynModuleDecl+NestedModule: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynModuleDecl] get_decls()
FSharp.Compiler.Syntax.SynModuleDecl+Open: FSharp.Compiler.Syntax.SynOpenDeclTarget get_target()
FSharp.Compiler.Syntax.SynModuleDecl+Open: FSharp.Compiler.Syntax.SynOpenDeclTarget target
FSharp.Compiler.Syntax.SynModuleDecl+Open: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynModuleDecl+Open: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynModuleDecl+Tags: Int32 Attributes
FSharp.Compiler.Syntax.SynModuleDecl+Tags: Int32 DoExpr
FSharp.Compiler.Syntax.SynModuleDecl+Tags: Int32 Exception
FSharp.Compiler.Syntax.SynModuleDecl+Tags: Int32 HashDirective
FSharp.Compiler.Syntax.SynModuleDecl+Tags: Int32 Let
FSharp.Compiler.Syntax.SynModuleDecl+Tags: Int32 ModuleAbbrev
FSharp.Compiler.Syntax.SynModuleDecl+Tags: Int32 NamespaceFragment
FSharp.Compiler.Syntax.SynModuleDecl+Tags: Int32 NestedModule
FSharp.Compiler.Syntax.SynModuleDecl+Tags: Int32 Open
FSharp.Compiler.Syntax.SynModuleDecl+Tags: Int32 Types
FSharp.Compiler.Syntax.SynModuleDecl+Types: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynModuleDecl+Types: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynModuleDecl+Types: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynTypeDefn] get_typeDefns()
FSharp.Compiler.Syntax.SynModuleDecl+Types: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynTypeDefn] typeDefns
FSharp.Compiler.Syntax.SynModuleDecl: Boolean IsAttributes
FSharp.Compiler.Syntax.SynModuleDecl: Boolean IsDoExpr
FSharp.Compiler.Syntax.SynModuleDecl: Boolean IsException
FSharp.Compiler.Syntax.SynModuleDecl: Boolean IsHashDirective
FSharp.Compiler.Syntax.SynModuleDecl: Boolean IsLet
FSharp.Compiler.Syntax.SynModuleDecl: Boolean IsModuleAbbrev
FSharp.Compiler.Syntax.SynModuleDecl: Boolean IsNamespaceFragment
FSharp.Compiler.Syntax.SynModuleDecl: Boolean IsNestedModule
FSharp.Compiler.Syntax.SynModuleDecl: Boolean IsOpen
FSharp.Compiler.Syntax.SynModuleDecl: Boolean IsTypes
FSharp.Compiler.Syntax.SynModuleDecl: Boolean get_IsAttributes()
FSharp.Compiler.Syntax.SynModuleDecl: Boolean get_IsDoExpr()
FSharp.Compiler.Syntax.SynModuleDecl: Boolean get_IsException()
FSharp.Compiler.Syntax.SynModuleDecl: Boolean get_IsHashDirective()
FSharp.Compiler.Syntax.SynModuleDecl: Boolean get_IsLet()
FSharp.Compiler.Syntax.SynModuleDecl: Boolean get_IsModuleAbbrev()
FSharp.Compiler.Syntax.SynModuleDecl: Boolean get_IsNamespaceFragment()
FSharp.Compiler.Syntax.SynModuleDecl: Boolean get_IsNestedModule()
FSharp.Compiler.Syntax.SynModuleDecl: Boolean get_IsOpen()
FSharp.Compiler.Syntax.SynModuleDecl: Boolean get_IsTypes()
FSharp.Compiler.Syntax.SynModuleDecl: FSharp.Compiler.Syntax.SynModuleDecl NewAttributes(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynModuleDecl: FSharp.Compiler.Syntax.SynModuleDecl NewDoExpr(FSharp.Compiler.Syntax.DebugPointAtBinding, FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynModuleDecl: FSharp.Compiler.Syntax.SynModuleDecl NewException(FSharp.Compiler.Syntax.SynExceptionDefn, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynModuleDecl: FSharp.Compiler.Syntax.SynModuleDecl NewHashDirective(FSharp.Compiler.Syntax.ParsedHashDirective, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynModuleDecl: FSharp.Compiler.Syntax.SynModuleDecl NewLet(Boolean, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynBinding], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynModuleDecl: FSharp.Compiler.Syntax.SynModuleDecl NewModuleAbbrev(FSharp.Compiler.Syntax.Ident, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynModuleDecl: FSharp.Compiler.Syntax.SynModuleDecl NewNamespaceFragment(FSharp.Compiler.Syntax.SynModuleOrNamespace)
FSharp.Compiler.Syntax.SynModuleDecl: FSharp.Compiler.Syntax.SynModuleDecl NewNestedModule(FSharp.Compiler.Syntax.SynComponentInfo, Boolean, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynModuleDecl], Boolean, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynModuleDecl: FSharp.Compiler.Syntax.SynModuleDecl NewOpen(FSharp.Compiler.Syntax.SynOpenDeclTarget, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynModuleDecl: FSharp.Compiler.Syntax.SynModuleDecl NewTypes(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynTypeDefn], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynModuleDecl: FSharp.Compiler.Syntax.SynModuleDecl+Attributes
FSharp.Compiler.Syntax.SynModuleDecl: FSharp.Compiler.Syntax.SynModuleDecl+DoExpr
FSharp.Compiler.Syntax.SynModuleDecl: FSharp.Compiler.Syntax.SynModuleDecl+Exception
FSharp.Compiler.Syntax.SynModuleDecl: FSharp.Compiler.Syntax.SynModuleDecl+HashDirective
FSharp.Compiler.Syntax.SynModuleDecl: FSharp.Compiler.Syntax.SynModuleDecl+Let
FSharp.Compiler.Syntax.SynModuleDecl: FSharp.Compiler.Syntax.SynModuleDecl+ModuleAbbrev
FSharp.Compiler.Syntax.SynModuleDecl: FSharp.Compiler.Syntax.SynModuleDecl+NamespaceFragment
FSharp.Compiler.Syntax.SynModuleDecl: FSharp.Compiler.Syntax.SynModuleDecl+NestedModule
FSharp.Compiler.Syntax.SynModuleDecl: FSharp.Compiler.Syntax.SynModuleDecl+Open
FSharp.Compiler.Syntax.SynModuleDecl: FSharp.Compiler.Syntax.SynModuleDecl+Tags
FSharp.Compiler.Syntax.SynModuleDecl: FSharp.Compiler.Syntax.SynModuleDecl+Types
FSharp.Compiler.Syntax.SynModuleDecl: FSharp.Compiler.Text.Range Range
FSharp.Compiler.Syntax.SynModuleDecl: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.Syntax.SynModuleDecl: Int32 Tag
FSharp.Compiler.Syntax.SynModuleDecl: Int32 get_Tag()
FSharp.Compiler.Syntax.SynModuleDecl: System.String ToString()
FSharp.Compiler.Syntax.SynModuleOrNamespace
FSharp.Compiler.Syntax.SynModuleOrNamespace: Boolean get_isRecursive()
FSharp.Compiler.Syntax.SynModuleOrNamespace: Boolean isRecursive
FSharp.Compiler.Syntax.SynModuleOrNamespace: FSharp.Compiler.Syntax.PreXmlDoc get_xmlDoc()
FSharp.Compiler.Syntax.SynModuleOrNamespace: FSharp.Compiler.Syntax.PreXmlDoc xmlDoc
FSharp.Compiler.Syntax.SynModuleOrNamespace: FSharp.Compiler.Syntax.SynModuleOrNamespace NewSynModuleOrNamespace(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident], Boolean, FSharp.Compiler.Syntax.SynModuleOrNamespaceKind, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynModuleDecl], FSharp.Compiler.Syntax.PreXmlDoc, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList], Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynModuleOrNamespace: FSharp.Compiler.Syntax.SynModuleOrNamespaceKind get_kind()
FSharp.Compiler.Syntax.SynModuleOrNamespace: FSharp.Compiler.Syntax.SynModuleOrNamespaceKind kind
FSharp.Compiler.Syntax.SynModuleOrNamespace: FSharp.Compiler.Text.Range Range
FSharp.Compiler.Syntax.SynModuleOrNamespace: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.Syntax.SynModuleOrNamespace: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynModuleOrNamespace: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynModuleOrNamespace: Int32 Tag
FSharp.Compiler.Syntax.SynModuleOrNamespace: Int32 get_Tag()
FSharp.Compiler.Syntax.SynModuleOrNamespace: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident] get_longId()
FSharp.Compiler.Syntax.SynModuleOrNamespace: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident] longId
FSharp.Compiler.Syntax.SynModuleOrNamespace: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList] attribs
FSharp.Compiler.Syntax.SynModuleOrNamespace: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList] get_attribs()
FSharp.Compiler.Syntax.SynModuleOrNamespace: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynModuleDecl] decls
FSharp.Compiler.Syntax.SynModuleOrNamespace: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynModuleDecl] get_decls()
FSharp.Compiler.Syntax.SynModuleOrNamespace: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess] accessibility
FSharp.Compiler.Syntax.SynModuleOrNamespace: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess] get_accessibility()
FSharp.Compiler.Syntax.SynModuleOrNamespace: System.String ToString()
FSharp.Compiler.Syntax.SynModuleOrNamespaceKind
FSharp.Compiler.Syntax.SynModuleOrNamespaceKind+Tags: Int32 AnonModule
FSharp.Compiler.Syntax.SynModuleOrNamespaceKind+Tags: Int32 DeclaredNamespace
FSharp.Compiler.Syntax.SynModuleOrNamespaceKind+Tags: Int32 GlobalNamespace
FSharp.Compiler.Syntax.SynModuleOrNamespaceKind+Tags: Int32 NamedModule
FSharp.Compiler.Syntax.SynModuleOrNamespaceKind: Boolean Equals(FSharp.Compiler.Syntax.SynModuleOrNamespaceKind)
FSharp.Compiler.Syntax.SynModuleOrNamespaceKind: Boolean Equals(System.Object)
FSharp.Compiler.Syntax.SynModuleOrNamespaceKind: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.Syntax.SynModuleOrNamespaceKind: Boolean IsAnonModule
FSharp.Compiler.Syntax.SynModuleOrNamespaceKind: Boolean IsDeclaredNamespace
FSharp.Compiler.Syntax.SynModuleOrNamespaceKind: Boolean IsGlobalNamespace
FSharp.Compiler.Syntax.SynModuleOrNamespaceKind: Boolean IsModule
FSharp.Compiler.Syntax.SynModuleOrNamespaceKind: Boolean IsNamedModule
FSharp.Compiler.Syntax.SynModuleOrNamespaceKind: Boolean get_IsAnonModule()
FSharp.Compiler.Syntax.SynModuleOrNamespaceKind: Boolean get_IsDeclaredNamespace()
FSharp.Compiler.Syntax.SynModuleOrNamespaceKind: Boolean get_IsGlobalNamespace()
FSharp.Compiler.Syntax.SynModuleOrNamespaceKind: Boolean get_IsModule()
FSharp.Compiler.Syntax.SynModuleOrNamespaceKind: Boolean get_IsNamedModule()
FSharp.Compiler.Syntax.SynModuleOrNamespaceKind: FSharp.Compiler.Syntax.SynModuleOrNamespaceKind AnonModule
FSharp.Compiler.Syntax.SynModuleOrNamespaceKind: FSharp.Compiler.Syntax.SynModuleOrNamespaceKind DeclaredNamespace
FSharp.Compiler.Syntax.SynModuleOrNamespaceKind: FSharp.Compiler.Syntax.SynModuleOrNamespaceKind GlobalNamespace
FSharp.Compiler.Syntax.SynModuleOrNamespaceKind: FSharp.Compiler.Syntax.SynModuleOrNamespaceKind NamedModule
FSharp.Compiler.Syntax.SynModuleOrNamespaceKind: FSharp.Compiler.Syntax.SynModuleOrNamespaceKind get_AnonModule()
FSharp.Compiler.Syntax.SynModuleOrNamespaceKind: FSharp.Compiler.Syntax.SynModuleOrNamespaceKind get_DeclaredNamespace()
FSharp.Compiler.Syntax.SynModuleOrNamespaceKind: FSharp.Compiler.Syntax.SynModuleOrNamespaceKind get_GlobalNamespace()
FSharp.Compiler.Syntax.SynModuleOrNamespaceKind: FSharp.Compiler.Syntax.SynModuleOrNamespaceKind get_NamedModule()
FSharp.Compiler.Syntax.SynModuleOrNamespaceKind: FSharp.Compiler.Syntax.SynModuleOrNamespaceKind+Tags
FSharp.Compiler.Syntax.SynModuleOrNamespaceKind: Int32 CompareTo(FSharp.Compiler.Syntax.SynModuleOrNamespaceKind)
FSharp.Compiler.Syntax.SynModuleOrNamespaceKind: Int32 CompareTo(System.Object)
FSharp.Compiler.Syntax.SynModuleOrNamespaceKind: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.Syntax.SynModuleOrNamespaceKind: Int32 GetHashCode()
FSharp.Compiler.Syntax.SynModuleOrNamespaceKind: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.Syntax.SynModuleOrNamespaceKind: Int32 Tag
FSharp.Compiler.Syntax.SynModuleOrNamespaceKind: Int32 get_Tag()
FSharp.Compiler.Syntax.SynModuleOrNamespaceKind: System.String ToString()
FSharp.Compiler.Syntax.SynModuleOrNamespaceSig
FSharp.Compiler.Syntax.SynModuleOrNamespaceSig: Boolean get_isRecursive()
FSharp.Compiler.Syntax.SynModuleOrNamespaceSig: Boolean isRecursive
FSharp.Compiler.Syntax.SynModuleOrNamespaceSig: FSharp.Compiler.Syntax.PreXmlDoc get_xmlDoc()
FSharp.Compiler.Syntax.SynModuleOrNamespaceSig: FSharp.Compiler.Syntax.PreXmlDoc xmlDoc
FSharp.Compiler.Syntax.SynModuleOrNamespaceSig: FSharp.Compiler.Syntax.SynModuleOrNamespaceKind get_kind()
FSharp.Compiler.Syntax.SynModuleOrNamespaceSig: FSharp.Compiler.Syntax.SynModuleOrNamespaceKind kind
FSharp.Compiler.Syntax.SynModuleOrNamespaceSig: FSharp.Compiler.Syntax.SynModuleOrNamespaceSig NewSynModuleOrNamespaceSig(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident], Boolean, FSharp.Compiler.Syntax.SynModuleOrNamespaceKind, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynModuleSigDecl], FSharp.Compiler.Syntax.PreXmlDoc, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList], Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynModuleOrNamespaceSig: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynModuleOrNamespaceSig: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynModuleOrNamespaceSig: Int32 Tag
FSharp.Compiler.Syntax.SynModuleOrNamespaceSig: Int32 get_Tag()
FSharp.Compiler.Syntax.SynModuleOrNamespaceSig: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident] get_longId()
FSharp.Compiler.Syntax.SynModuleOrNamespaceSig: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident] longId
FSharp.Compiler.Syntax.SynModuleOrNamespaceSig: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList] attribs
FSharp.Compiler.Syntax.SynModuleOrNamespaceSig: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList] get_attribs()
FSharp.Compiler.Syntax.SynModuleOrNamespaceSig: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynModuleSigDecl] decls
FSharp.Compiler.Syntax.SynModuleOrNamespaceSig: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynModuleSigDecl] get_decls()
FSharp.Compiler.Syntax.SynModuleOrNamespaceSig: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess] accessibility
FSharp.Compiler.Syntax.SynModuleOrNamespaceSig: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess] get_accessibility()
FSharp.Compiler.Syntax.SynModuleOrNamespaceSig: System.String ToString()
FSharp.Compiler.Syntax.SynModuleSigDecl
FSharp.Compiler.Syntax.SynModuleSigDecl+Exception: FSharp.Compiler.Syntax.SynExceptionSig exnSig
FSharp.Compiler.Syntax.SynModuleSigDecl+Exception: FSharp.Compiler.Syntax.SynExceptionSig get_exnSig()
FSharp.Compiler.Syntax.SynModuleSigDecl+Exception: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynModuleSigDecl+Exception: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynModuleSigDecl+HashDirective: FSharp.Compiler.Syntax.ParsedHashDirective get_hashDirective()
FSharp.Compiler.Syntax.SynModuleSigDecl+HashDirective: FSharp.Compiler.Syntax.ParsedHashDirective hashDirective
FSharp.Compiler.Syntax.SynModuleSigDecl+HashDirective: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynModuleSigDecl+HashDirective: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynModuleSigDecl+ModuleAbbrev: FSharp.Compiler.Syntax.Ident get_ident()
FSharp.Compiler.Syntax.SynModuleSigDecl+ModuleAbbrev: FSharp.Compiler.Syntax.Ident ident
FSharp.Compiler.Syntax.SynModuleSigDecl+ModuleAbbrev: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynModuleSigDecl+ModuleAbbrev: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynModuleSigDecl+ModuleAbbrev: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident] get_longId()
FSharp.Compiler.Syntax.SynModuleSigDecl+ModuleAbbrev: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident] longId
FSharp.Compiler.Syntax.SynModuleSigDecl+NamespaceFragment: FSharp.Compiler.Syntax.SynModuleOrNamespaceSig Item
FSharp.Compiler.Syntax.SynModuleSigDecl+NamespaceFragment: FSharp.Compiler.Syntax.SynModuleOrNamespaceSig get_Item()
FSharp.Compiler.Syntax.SynModuleSigDecl+NestedModule: Boolean get_isRecursive()
FSharp.Compiler.Syntax.SynModuleSigDecl+NestedModule: Boolean isRecursive
FSharp.Compiler.Syntax.SynModuleSigDecl+NestedModule: FSharp.Compiler.Syntax.SynComponentInfo get_moduleInfo()
FSharp.Compiler.Syntax.SynModuleSigDecl+NestedModule: FSharp.Compiler.Syntax.SynComponentInfo moduleInfo
FSharp.Compiler.Syntax.SynModuleSigDecl+NestedModule: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynModuleSigDecl+NestedModule: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynModuleSigDecl+NestedModule: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynModuleSigDecl] get_moduleDecls()
FSharp.Compiler.Syntax.SynModuleSigDecl+NestedModule: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynModuleSigDecl] moduleDecls
FSharp.Compiler.Syntax.SynModuleSigDecl+Open: FSharp.Compiler.Syntax.SynOpenDeclTarget get_target()
FSharp.Compiler.Syntax.SynModuleSigDecl+Open: FSharp.Compiler.Syntax.SynOpenDeclTarget target
FSharp.Compiler.Syntax.SynModuleSigDecl+Open: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynModuleSigDecl+Open: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynModuleSigDecl+Tags: Int32 Exception
FSharp.Compiler.Syntax.SynModuleSigDecl+Tags: Int32 HashDirective
FSharp.Compiler.Syntax.SynModuleSigDecl+Tags: Int32 ModuleAbbrev
FSharp.Compiler.Syntax.SynModuleSigDecl+Tags: Int32 NamespaceFragment
FSharp.Compiler.Syntax.SynModuleSigDecl+Tags: Int32 NestedModule
FSharp.Compiler.Syntax.SynModuleSigDecl+Tags: Int32 Open
FSharp.Compiler.Syntax.SynModuleSigDecl+Tags: Int32 Types
FSharp.Compiler.Syntax.SynModuleSigDecl+Tags: Int32 Val
FSharp.Compiler.Syntax.SynModuleSigDecl+Types: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynModuleSigDecl+Types: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynModuleSigDecl+Types: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynTypeDefnSig] get_types()
FSharp.Compiler.Syntax.SynModuleSigDecl+Types: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynTypeDefnSig] types
FSharp.Compiler.Syntax.SynModuleSigDecl+Val: FSharp.Compiler.Syntax.SynValSig get_valSig()
FSharp.Compiler.Syntax.SynModuleSigDecl+Val: FSharp.Compiler.Syntax.SynValSig valSig
FSharp.Compiler.Syntax.SynModuleSigDecl+Val: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynModuleSigDecl+Val: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynModuleSigDecl: Boolean IsException
FSharp.Compiler.Syntax.SynModuleSigDecl: Boolean IsHashDirective
FSharp.Compiler.Syntax.SynModuleSigDecl: Boolean IsModuleAbbrev
FSharp.Compiler.Syntax.SynModuleSigDecl: Boolean IsNamespaceFragment
FSharp.Compiler.Syntax.SynModuleSigDecl: Boolean IsNestedModule
FSharp.Compiler.Syntax.SynModuleSigDecl: Boolean IsOpen
FSharp.Compiler.Syntax.SynModuleSigDecl: Boolean IsTypes
FSharp.Compiler.Syntax.SynModuleSigDecl: Boolean IsVal
FSharp.Compiler.Syntax.SynModuleSigDecl: Boolean get_IsException()
FSharp.Compiler.Syntax.SynModuleSigDecl: Boolean get_IsHashDirective()
FSharp.Compiler.Syntax.SynModuleSigDecl: Boolean get_IsModuleAbbrev()
FSharp.Compiler.Syntax.SynModuleSigDecl: Boolean get_IsNamespaceFragment()
FSharp.Compiler.Syntax.SynModuleSigDecl: Boolean get_IsNestedModule()
FSharp.Compiler.Syntax.SynModuleSigDecl: Boolean get_IsOpen()
FSharp.Compiler.Syntax.SynModuleSigDecl: Boolean get_IsTypes()
FSharp.Compiler.Syntax.SynModuleSigDecl: Boolean get_IsVal()
FSharp.Compiler.Syntax.SynModuleSigDecl: FSharp.Compiler.Syntax.SynModuleSigDecl NewException(FSharp.Compiler.Syntax.SynExceptionSig, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynModuleSigDecl: FSharp.Compiler.Syntax.SynModuleSigDecl NewHashDirective(FSharp.Compiler.Syntax.ParsedHashDirective, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynModuleSigDecl: FSharp.Compiler.Syntax.SynModuleSigDecl NewModuleAbbrev(FSharp.Compiler.Syntax.Ident, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynModuleSigDecl: FSharp.Compiler.Syntax.SynModuleSigDecl NewNamespaceFragment(FSharp.Compiler.Syntax.SynModuleOrNamespaceSig)
FSharp.Compiler.Syntax.SynModuleSigDecl: FSharp.Compiler.Syntax.SynModuleSigDecl NewNestedModule(FSharp.Compiler.Syntax.SynComponentInfo, Boolean, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynModuleSigDecl], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynModuleSigDecl: FSharp.Compiler.Syntax.SynModuleSigDecl NewOpen(FSharp.Compiler.Syntax.SynOpenDeclTarget, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynModuleSigDecl: FSharp.Compiler.Syntax.SynModuleSigDecl NewTypes(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynTypeDefnSig], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynModuleSigDecl: FSharp.Compiler.Syntax.SynModuleSigDecl NewVal(FSharp.Compiler.Syntax.SynValSig, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynModuleSigDecl: FSharp.Compiler.Syntax.SynModuleSigDecl+Exception
FSharp.Compiler.Syntax.SynModuleSigDecl: FSharp.Compiler.Syntax.SynModuleSigDecl+HashDirective
FSharp.Compiler.Syntax.SynModuleSigDecl: FSharp.Compiler.Syntax.SynModuleSigDecl+ModuleAbbrev
FSharp.Compiler.Syntax.SynModuleSigDecl: FSharp.Compiler.Syntax.SynModuleSigDecl+NamespaceFragment
FSharp.Compiler.Syntax.SynModuleSigDecl: FSharp.Compiler.Syntax.SynModuleSigDecl+NestedModule
FSharp.Compiler.Syntax.SynModuleSigDecl: FSharp.Compiler.Syntax.SynModuleSigDecl+Open
FSharp.Compiler.Syntax.SynModuleSigDecl: FSharp.Compiler.Syntax.SynModuleSigDecl+Tags
FSharp.Compiler.Syntax.SynModuleSigDecl: FSharp.Compiler.Syntax.SynModuleSigDecl+Types
FSharp.Compiler.Syntax.SynModuleSigDecl: FSharp.Compiler.Syntax.SynModuleSigDecl+Val
FSharp.Compiler.Syntax.SynModuleSigDecl: FSharp.Compiler.Text.Range Range
FSharp.Compiler.Syntax.SynModuleSigDecl: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.Syntax.SynModuleSigDecl: Int32 Tag
FSharp.Compiler.Syntax.SynModuleSigDecl: Int32 get_Tag()
FSharp.Compiler.Syntax.SynModuleSigDecl: System.String ToString()
FSharp.Compiler.Syntax.SynOpenDeclTarget
FSharp.Compiler.Syntax.SynOpenDeclTarget+ModuleOrNamespace: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynOpenDeclTarget+ModuleOrNamespace: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynOpenDeclTarget+ModuleOrNamespace: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident] get_longId()
FSharp.Compiler.Syntax.SynOpenDeclTarget+ModuleOrNamespace: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident] longId
FSharp.Compiler.Syntax.SynOpenDeclTarget+Tags: Int32 ModuleOrNamespace
FSharp.Compiler.Syntax.SynOpenDeclTarget+Tags: Int32 Type
FSharp.Compiler.Syntax.SynOpenDeclTarget+Type: FSharp.Compiler.Syntax.SynType get_typeName()
FSharp.Compiler.Syntax.SynOpenDeclTarget+Type: FSharp.Compiler.Syntax.SynType typeName
FSharp.Compiler.Syntax.SynOpenDeclTarget+Type: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynOpenDeclTarget+Type: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynOpenDeclTarget: Boolean IsModuleOrNamespace
FSharp.Compiler.Syntax.SynOpenDeclTarget: Boolean IsType
FSharp.Compiler.Syntax.SynOpenDeclTarget: Boolean get_IsModuleOrNamespace()
FSharp.Compiler.Syntax.SynOpenDeclTarget: Boolean get_IsType()
FSharp.Compiler.Syntax.SynOpenDeclTarget: FSharp.Compiler.Syntax.SynOpenDeclTarget NewModuleOrNamespace(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynOpenDeclTarget: FSharp.Compiler.Syntax.SynOpenDeclTarget NewType(FSharp.Compiler.Syntax.SynType, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynOpenDeclTarget: FSharp.Compiler.Syntax.SynOpenDeclTarget+ModuleOrNamespace
FSharp.Compiler.Syntax.SynOpenDeclTarget: FSharp.Compiler.Syntax.SynOpenDeclTarget+Tags
FSharp.Compiler.Syntax.SynOpenDeclTarget: FSharp.Compiler.Syntax.SynOpenDeclTarget+Type
FSharp.Compiler.Syntax.SynOpenDeclTarget: FSharp.Compiler.Text.Range Range
FSharp.Compiler.Syntax.SynOpenDeclTarget: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.Syntax.SynOpenDeclTarget: Int32 Tag
FSharp.Compiler.Syntax.SynOpenDeclTarget: Int32 get_Tag()
FSharp.Compiler.Syntax.SynOpenDeclTarget: System.String ToString()
FSharp.Compiler.Syntax.SynPat
FSharp.Compiler.Syntax.SynPat+Ands: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynPat+Ands: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynPat+Ands: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynPat] get_pats()
FSharp.Compiler.Syntax.SynPat+Ands: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynPat] pats
FSharp.Compiler.Syntax.SynPat+ArrayOrList: Boolean get_isArray()
FSharp.Compiler.Syntax.SynPat+ArrayOrList: Boolean isArray
FSharp.Compiler.Syntax.SynPat+ArrayOrList: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynPat+ArrayOrList: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynPat+ArrayOrList: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynPat] elementPats
FSharp.Compiler.Syntax.SynPat+ArrayOrList: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynPat] get_elementPats()
FSharp.Compiler.Syntax.SynPat+Attrib: FSharp.Compiler.Syntax.SynPat get_pat()
FSharp.Compiler.Syntax.SynPat+Attrib: FSharp.Compiler.Syntax.SynPat pat
FSharp.Compiler.Syntax.SynPat+Attrib: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynPat+Attrib: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynPat+Attrib: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList] attributes
FSharp.Compiler.Syntax.SynPat+Attrib: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList] get_attributes()
FSharp.Compiler.Syntax.SynPat+Const: FSharp.Compiler.Syntax.SynConst constant
FSharp.Compiler.Syntax.SynPat+Const: FSharp.Compiler.Syntax.SynConst get_constant()
FSharp.Compiler.Syntax.SynPat+Const: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynPat+Const: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynPat+DeprecatedCharRange: Char endChar
FSharp.Compiler.Syntax.SynPat+DeprecatedCharRange: Char get_endChar()
FSharp.Compiler.Syntax.SynPat+DeprecatedCharRange: Char get_startChar()
FSharp.Compiler.Syntax.SynPat+DeprecatedCharRange: Char startChar
FSharp.Compiler.Syntax.SynPat+DeprecatedCharRange: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynPat+DeprecatedCharRange: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynPat+FromParseError: FSharp.Compiler.Syntax.SynPat get_pat()
FSharp.Compiler.Syntax.SynPat+FromParseError: FSharp.Compiler.Syntax.SynPat pat
FSharp.Compiler.Syntax.SynPat+FromParseError: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynPat+FromParseError: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynPat+InstanceMember: FSharp.Compiler.Syntax.Ident get_memberId()
FSharp.Compiler.Syntax.SynPat+InstanceMember: FSharp.Compiler.Syntax.Ident get_thisId()
FSharp.Compiler.Syntax.SynPat+InstanceMember: FSharp.Compiler.Syntax.Ident memberId
FSharp.Compiler.Syntax.SynPat+InstanceMember: FSharp.Compiler.Syntax.Ident thisId
FSharp.Compiler.Syntax.SynPat+InstanceMember: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynPat+InstanceMember: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynPat+InstanceMember: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.Ident] get_toolingId()
FSharp.Compiler.Syntax.SynPat+InstanceMember: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.Ident] toolingId
FSharp.Compiler.Syntax.SynPat+InstanceMember: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess] accessibility
FSharp.Compiler.Syntax.SynPat+InstanceMember: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess] get_accessibility()
FSharp.Compiler.Syntax.SynPat+IsInst: FSharp.Compiler.Syntax.SynType get_pat()
FSharp.Compiler.Syntax.SynPat+IsInst: FSharp.Compiler.Syntax.SynType pat
FSharp.Compiler.Syntax.SynPat+IsInst: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynPat+IsInst: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynPat+LongIdent: FSharp.Compiler.Syntax.LongIdentWithDots get_longDotId()
FSharp.Compiler.Syntax.SynPat+LongIdent: FSharp.Compiler.Syntax.LongIdentWithDots longDotId
FSharp.Compiler.Syntax.SynPat+LongIdent: FSharp.Compiler.Syntax.SynArgPats argPats
FSharp.Compiler.Syntax.SynPat+LongIdent: FSharp.Compiler.Syntax.SynArgPats get_argPats()
FSharp.Compiler.Syntax.SynPat+LongIdent: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynPat+LongIdent: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynPat+LongIdent: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.Ident] extraId
FSharp.Compiler.Syntax.SynPat+LongIdent: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.Ident] get_extraId()
FSharp.Compiler.Syntax.SynPat+LongIdent: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess] accessibility
FSharp.Compiler.Syntax.SynPat+LongIdent: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess] get_accessibility()
FSharp.Compiler.Syntax.SynPat+LongIdent: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynValTyparDecls] get_typarDecls()
FSharp.Compiler.Syntax.SynPat+LongIdent: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynValTyparDecls] typarDecls
FSharp.Compiler.Syntax.SynPat+Named: Boolean get_isSelfIdentifier()
FSharp.Compiler.Syntax.SynPat+Named: Boolean isSelfIdentifier
FSharp.Compiler.Syntax.SynPat+Named: FSharp.Compiler.Syntax.Ident get_ident()
FSharp.Compiler.Syntax.SynPat+Named: FSharp.Compiler.Syntax.Ident ident
FSharp.Compiler.Syntax.SynPat+Named: FSharp.Compiler.Syntax.SynPat get_pat()
FSharp.Compiler.Syntax.SynPat+Named: FSharp.Compiler.Syntax.SynPat pat
FSharp.Compiler.Syntax.SynPat+Named: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynPat+Named: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynPat+Named: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess] accessibility
FSharp.Compiler.Syntax.SynPat+Named: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess] get_accessibility()
FSharp.Compiler.Syntax.SynPat+Null: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynPat+Null: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynPat+OptionalVal: FSharp.Compiler.Syntax.Ident get_ident()
FSharp.Compiler.Syntax.SynPat+OptionalVal: FSharp.Compiler.Syntax.Ident ident
FSharp.Compiler.Syntax.SynPat+OptionalVal: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynPat+OptionalVal: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynPat+Or: FSharp.Compiler.Syntax.SynPat get_lhsPat()
FSharp.Compiler.Syntax.SynPat+Or: FSharp.Compiler.Syntax.SynPat get_rhsPat()
FSharp.Compiler.Syntax.SynPat+Or: FSharp.Compiler.Syntax.SynPat lhsPat
FSharp.Compiler.Syntax.SynPat+Or: FSharp.Compiler.Syntax.SynPat rhsPat
FSharp.Compiler.Syntax.SynPat+Or: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynPat+Or: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynPat+Paren: FSharp.Compiler.Syntax.SynPat get_pat()
FSharp.Compiler.Syntax.SynPat+Paren: FSharp.Compiler.Syntax.SynPat pat
FSharp.Compiler.Syntax.SynPat+Paren: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynPat+Paren: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynPat+QuoteExpr: FSharp.Compiler.Syntax.SynExpr expr
FSharp.Compiler.Syntax.SynPat+QuoteExpr: FSharp.Compiler.Syntax.SynExpr get_expr()
FSharp.Compiler.Syntax.SynPat+QuoteExpr: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynPat+QuoteExpr: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynPat+Record: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynPat+Record: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynPat+Record: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[System.Tuple`2[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident],FSharp.Compiler.Syntax.Ident],FSharp.Compiler.Syntax.SynPat]] fieldPats
FSharp.Compiler.Syntax.SynPat+Record: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[System.Tuple`2[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident],FSharp.Compiler.Syntax.Ident],FSharp.Compiler.Syntax.SynPat]] get_fieldPats()
FSharp.Compiler.Syntax.SynPat+Tags: Int32 Ands
FSharp.Compiler.Syntax.SynPat+Tags: Int32 ArrayOrList
FSharp.Compiler.Syntax.SynPat+Tags: Int32 Attrib
FSharp.Compiler.Syntax.SynPat+Tags: Int32 Const
FSharp.Compiler.Syntax.SynPat+Tags: Int32 DeprecatedCharRange
FSharp.Compiler.Syntax.SynPat+Tags: Int32 FromParseError
FSharp.Compiler.Syntax.SynPat+Tags: Int32 InstanceMember
FSharp.Compiler.Syntax.SynPat+Tags: Int32 IsInst
FSharp.Compiler.Syntax.SynPat+Tags: Int32 LongIdent
FSharp.Compiler.Syntax.SynPat+Tags: Int32 Named
FSharp.Compiler.Syntax.SynPat+Tags: Int32 Null
FSharp.Compiler.Syntax.SynPat+Tags: Int32 OptionalVal
FSharp.Compiler.Syntax.SynPat+Tags: Int32 Or
FSharp.Compiler.Syntax.SynPat+Tags: Int32 Paren
FSharp.Compiler.Syntax.SynPat+Tags: Int32 QuoteExpr
FSharp.Compiler.Syntax.SynPat+Tags: Int32 Record
FSharp.Compiler.Syntax.SynPat+Tags: Int32 Tuple
FSharp.Compiler.Syntax.SynPat+Tags: Int32 Typed
FSharp.Compiler.Syntax.SynPat+Tags: Int32 Wild
FSharp.Compiler.Syntax.SynPat+Tuple: Boolean get_isStruct()
FSharp.Compiler.Syntax.SynPat+Tuple: Boolean isStruct
FSharp.Compiler.Syntax.SynPat+Tuple: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynPat+Tuple: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynPat+Tuple: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynPat] elementPats
FSharp.Compiler.Syntax.SynPat+Tuple: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynPat] get_elementPats()
FSharp.Compiler.Syntax.SynPat+Typed: FSharp.Compiler.Syntax.SynPat get_pat()
FSharp.Compiler.Syntax.SynPat+Typed: FSharp.Compiler.Syntax.SynPat pat
FSharp.Compiler.Syntax.SynPat+Typed: FSharp.Compiler.Syntax.SynType get_targetType()
FSharp.Compiler.Syntax.SynPat+Typed: FSharp.Compiler.Syntax.SynType targetType
FSharp.Compiler.Syntax.SynPat+Typed: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynPat+Typed: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynPat+Wild: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynPat+Wild: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynPat: Boolean IsAnds
FSharp.Compiler.Syntax.SynPat: Boolean IsArrayOrList
FSharp.Compiler.Syntax.SynPat: Boolean IsAttrib
FSharp.Compiler.Syntax.SynPat: Boolean IsConst
FSharp.Compiler.Syntax.SynPat: Boolean IsDeprecatedCharRange
FSharp.Compiler.Syntax.SynPat: Boolean IsFromParseError
FSharp.Compiler.Syntax.SynPat: Boolean IsInstanceMember
FSharp.Compiler.Syntax.SynPat: Boolean IsIsInst
FSharp.Compiler.Syntax.SynPat: Boolean IsLongIdent
FSharp.Compiler.Syntax.SynPat: Boolean IsNamed
FSharp.Compiler.Syntax.SynPat: Boolean IsNull
FSharp.Compiler.Syntax.SynPat: Boolean IsOptionalVal
FSharp.Compiler.Syntax.SynPat: Boolean IsOr
FSharp.Compiler.Syntax.SynPat: Boolean IsParen
FSharp.Compiler.Syntax.SynPat: Boolean IsQuoteExpr
FSharp.Compiler.Syntax.SynPat: Boolean IsRecord
FSharp.Compiler.Syntax.SynPat: Boolean IsTuple
FSharp.Compiler.Syntax.SynPat: Boolean IsTyped
FSharp.Compiler.Syntax.SynPat: Boolean IsWild
FSharp.Compiler.Syntax.SynPat: Boolean get_IsAnds()
FSharp.Compiler.Syntax.SynPat: Boolean get_IsArrayOrList()
FSharp.Compiler.Syntax.SynPat: Boolean get_IsAttrib()
FSharp.Compiler.Syntax.SynPat: Boolean get_IsConst()
FSharp.Compiler.Syntax.SynPat: Boolean get_IsDeprecatedCharRange()
FSharp.Compiler.Syntax.SynPat: Boolean get_IsFromParseError()
FSharp.Compiler.Syntax.SynPat: Boolean get_IsInstanceMember()
FSharp.Compiler.Syntax.SynPat: Boolean get_IsIsInst()
FSharp.Compiler.Syntax.SynPat: Boolean get_IsLongIdent()
FSharp.Compiler.Syntax.SynPat: Boolean get_IsNamed()
FSharp.Compiler.Syntax.SynPat: Boolean get_IsNull()
FSharp.Compiler.Syntax.SynPat: Boolean get_IsOptionalVal()
FSharp.Compiler.Syntax.SynPat: Boolean get_IsOr()
FSharp.Compiler.Syntax.SynPat: Boolean get_IsParen()
FSharp.Compiler.Syntax.SynPat: Boolean get_IsQuoteExpr()
FSharp.Compiler.Syntax.SynPat: Boolean get_IsRecord()
FSharp.Compiler.Syntax.SynPat: Boolean get_IsTuple()
FSharp.Compiler.Syntax.SynPat: Boolean get_IsTyped()
FSharp.Compiler.Syntax.SynPat: Boolean get_IsWild()
FSharp.Compiler.Syntax.SynPat: FSharp.Compiler.Syntax.SynPat NewAnds(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynPat], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynPat: FSharp.Compiler.Syntax.SynPat NewArrayOrList(Boolean, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynPat], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynPat: FSharp.Compiler.Syntax.SynPat NewAttrib(FSharp.Compiler.Syntax.SynPat, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynPat: FSharp.Compiler.Syntax.SynPat NewConst(FSharp.Compiler.Syntax.SynConst, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynPat: FSharp.Compiler.Syntax.SynPat NewDeprecatedCharRange(Char, Char, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynPat: FSharp.Compiler.Syntax.SynPat NewFromParseError(FSharp.Compiler.Syntax.SynPat, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynPat: FSharp.Compiler.Syntax.SynPat NewInstanceMember(FSharp.Compiler.Syntax.Ident, FSharp.Compiler.Syntax.Ident, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.Ident], Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynPat: FSharp.Compiler.Syntax.SynPat NewIsInst(FSharp.Compiler.Syntax.SynType, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynPat: FSharp.Compiler.Syntax.SynPat NewLongIdent(FSharp.Compiler.Syntax.LongIdentWithDots, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.Ident], Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynValTyparDecls], FSharp.Compiler.Syntax.SynArgPats, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynPat: FSharp.Compiler.Syntax.SynPat NewNamed(FSharp.Compiler.Syntax.SynPat, FSharp.Compiler.Syntax.Ident, Boolean, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynPat: FSharp.Compiler.Syntax.SynPat NewNull(FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynPat: FSharp.Compiler.Syntax.SynPat NewOptionalVal(FSharp.Compiler.Syntax.Ident, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynPat: FSharp.Compiler.Syntax.SynPat NewOr(FSharp.Compiler.Syntax.SynPat, FSharp.Compiler.Syntax.SynPat, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynPat: FSharp.Compiler.Syntax.SynPat NewParen(FSharp.Compiler.Syntax.SynPat, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynPat: FSharp.Compiler.Syntax.SynPat NewQuoteExpr(FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynPat: FSharp.Compiler.Syntax.SynPat NewRecord(Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[System.Tuple`2[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.Ident],FSharp.Compiler.Syntax.Ident],FSharp.Compiler.Syntax.SynPat]], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynPat: FSharp.Compiler.Syntax.SynPat NewTuple(Boolean, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynPat], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynPat: FSharp.Compiler.Syntax.SynPat NewTyped(FSharp.Compiler.Syntax.SynPat, FSharp.Compiler.Syntax.SynType, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynPat: FSharp.Compiler.Syntax.SynPat NewWild(FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynPat: FSharp.Compiler.Syntax.SynPat+Ands
FSharp.Compiler.Syntax.SynPat: FSharp.Compiler.Syntax.SynPat+ArrayOrList
FSharp.Compiler.Syntax.SynPat: FSharp.Compiler.Syntax.SynPat+Attrib
FSharp.Compiler.Syntax.SynPat: FSharp.Compiler.Syntax.SynPat+Const
FSharp.Compiler.Syntax.SynPat: FSharp.Compiler.Syntax.SynPat+DeprecatedCharRange
FSharp.Compiler.Syntax.SynPat: FSharp.Compiler.Syntax.SynPat+FromParseError
FSharp.Compiler.Syntax.SynPat: FSharp.Compiler.Syntax.SynPat+InstanceMember
FSharp.Compiler.Syntax.SynPat: FSharp.Compiler.Syntax.SynPat+IsInst
FSharp.Compiler.Syntax.SynPat: FSharp.Compiler.Syntax.SynPat+LongIdent
FSharp.Compiler.Syntax.SynPat: FSharp.Compiler.Syntax.SynPat+Named
FSharp.Compiler.Syntax.SynPat: FSharp.Compiler.Syntax.SynPat+Null
FSharp.Compiler.Syntax.SynPat: FSharp.Compiler.Syntax.SynPat+OptionalVal
FSharp.Compiler.Syntax.SynPat: FSharp.Compiler.Syntax.SynPat+Or
FSharp.Compiler.Syntax.SynPat: FSharp.Compiler.Syntax.SynPat+Paren
FSharp.Compiler.Syntax.SynPat: FSharp.Compiler.Syntax.SynPat+QuoteExpr
FSharp.Compiler.Syntax.SynPat: FSharp.Compiler.Syntax.SynPat+Record
FSharp.Compiler.Syntax.SynPat: FSharp.Compiler.Syntax.SynPat+Tags
FSharp.Compiler.Syntax.SynPat: FSharp.Compiler.Syntax.SynPat+Tuple
FSharp.Compiler.Syntax.SynPat: FSharp.Compiler.Syntax.SynPat+Typed
FSharp.Compiler.Syntax.SynPat: FSharp.Compiler.Syntax.SynPat+Wild
FSharp.Compiler.Syntax.SynPat: FSharp.Compiler.Text.Range Range
FSharp.Compiler.Syntax.SynPat: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.Syntax.SynPat: Int32 Tag
FSharp.Compiler.Syntax.SynPat: Int32 get_Tag()
FSharp.Compiler.Syntax.SynPat: System.String ToString()
FSharp.Compiler.Syntax.SynRationalConst
FSharp.Compiler.Syntax.SynRationalConst+Integer: Int32 get_value()
FSharp.Compiler.Syntax.SynRationalConst+Integer: Int32 value
FSharp.Compiler.Syntax.SynRationalConst+Negate: FSharp.Compiler.Syntax.SynRationalConst Item
FSharp.Compiler.Syntax.SynRationalConst+Negate: FSharp.Compiler.Syntax.SynRationalConst get_Item()
FSharp.Compiler.Syntax.SynRationalConst+Rational: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynRationalConst+Rational: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynRationalConst+Rational: Int32 denominator
FSharp.Compiler.Syntax.SynRationalConst+Rational: Int32 get_denominator()
FSharp.Compiler.Syntax.SynRationalConst+Rational: Int32 get_numerator()
FSharp.Compiler.Syntax.SynRationalConst+Rational: Int32 numerator
FSharp.Compiler.Syntax.SynRationalConst+Tags: Int32 Integer
FSharp.Compiler.Syntax.SynRationalConst+Tags: Int32 Negate
FSharp.Compiler.Syntax.SynRationalConst+Tags: Int32 Rational
FSharp.Compiler.Syntax.SynRationalConst: Boolean IsInteger
FSharp.Compiler.Syntax.SynRationalConst: Boolean IsNegate
FSharp.Compiler.Syntax.SynRationalConst: Boolean IsRational
FSharp.Compiler.Syntax.SynRationalConst: Boolean get_IsInteger()
FSharp.Compiler.Syntax.SynRationalConst: Boolean get_IsNegate()
FSharp.Compiler.Syntax.SynRationalConst: Boolean get_IsRational()
FSharp.Compiler.Syntax.SynRationalConst: FSharp.Compiler.Syntax.SynRationalConst NewInteger(Int32)
FSharp.Compiler.Syntax.SynRationalConst: FSharp.Compiler.Syntax.SynRationalConst NewNegate(FSharp.Compiler.Syntax.SynRationalConst)
FSharp.Compiler.Syntax.SynRationalConst: FSharp.Compiler.Syntax.SynRationalConst NewRational(Int32, Int32, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynRationalConst: FSharp.Compiler.Syntax.SynRationalConst+Integer
FSharp.Compiler.Syntax.SynRationalConst: FSharp.Compiler.Syntax.SynRationalConst+Negate
FSharp.Compiler.Syntax.SynRationalConst: FSharp.Compiler.Syntax.SynRationalConst+Rational
FSharp.Compiler.Syntax.SynRationalConst: FSharp.Compiler.Syntax.SynRationalConst+Tags
FSharp.Compiler.Syntax.SynRationalConst: Int32 Tag
FSharp.Compiler.Syntax.SynRationalConst: Int32 get_Tag()
FSharp.Compiler.Syntax.SynRationalConst: System.String ToString()
FSharp.Compiler.Syntax.SynReturnInfo
FSharp.Compiler.Syntax.SynReturnInfo: FSharp.Compiler.Syntax.SynReturnInfo NewSynReturnInfo(System.Tuple`2[FSharp.Compiler.Syntax.SynType,FSharp.Compiler.Syntax.SynArgInfo], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynReturnInfo: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynReturnInfo: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynReturnInfo: Int32 Tag
FSharp.Compiler.Syntax.SynReturnInfo: Int32 get_Tag()
FSharp.Compiler.Syntax.SynReturnInfo: System.String ToString()
FSharp.Compiler.Syntax.SynReturnInfo: System.Tuple`2[FSharp.Compiler.Syntax.SynType,FSharp.Compiler.Syntax.SynArgInfo] get_returnType()
FSharp.Compiler.Syntax.SynReturnInfo: System.Tuple`2[FSharp.Compiler.Syntax.SynType,FSharp.Compiler.Syntax.SynArgInfo] returnType
FSharp.Compiler.Syntax.SynSimplePat
FSharp.Compiler.Syntax.SynSimplePat+Attrib: FSharp.Compiler.Syntax.SynSimplePat get_pat()
FSharp.Compiler.Syntax.SynSimplePat+Attrib: FSharp.Compiler.Syntax.SynSimplePat pat
FSharp.Compiler.Syntax.SynSimplePat+Attrib: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynSimplePat+Attrib: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynSimplePat+Attrib: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList] attributes
FSharp.Compiler.Syntax.SynSimplePat+Attrib: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList] get_attributes()
FSharp.Compiler.Syntax.SynSimplePat+Id: Boolean get_isCompilerGenerated()
FSharp.Compiler.Syntax.SynSimplePat+Id: Boolean get_isOptArg()
FSharp.Compiler.Syntax.SynSimplePat+Id: Boolean get_isThisVar()
FSharp.Compiler.Syntax.SynSimplePat+Id: Boolean isCompilerGenerated
FSharp.Compiler.Syntax.SynSimplePat+Id: Boolean isOptArg
FSharp.Compiler.Syntax.SynSimplePat+Id: Boolean isThisVar
FSharp.Compiler.Syntax.SynSimplePat+Id: FSharp.Compiler.Syntax.Ident get_ident()
FSharp.Compiler.Syntax.SynSimplePat+Id: FSharp.Compiler.Syntax.Ident ident
FSharp.Compiler.Syntax.SynSimplePat+Id: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynSimplePat+Id: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynSimplePat+Id: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.FSharpRef`1[FSharp.Compiler.Syntax.SynSimplePatAlternativeIdInfo]] altNameRefCell
FSharp.Compiler.Syntax.SynSimplePat+Id: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.FSharpRef`1[FSharp.Compiler.Syntax.SynSimplePatAlternativeIdInfo]] get_altNameRefCell()
FSharp.Compiler.Syntax.SynSimplePat+Tags: Int32 Attrib
FSharp.Compiler.Syntax.SynSimplePat+Tags: Int32 Id
FSharp.Compiler.Syntax.SynSimplePat+Tags: Int32 Typed
FSharp.Compiler.Syntax.SynSimplePat+Typed: FSharp.Compiler.Syntax.SynSimplePat get_pat()
FSharp.Compiler.Syntax.SynSimplePat+Typed: FSharp.Compiler.Syntax.SynSimplePat pat
FSharp.Compiler.Syntax.SynSimplePat+Typed: FSharp.Compiler.Syntax.SynType get_targetType()
FSharp.Compiler.Syntax.SynSimplePat+Typed: FSharp.Compiler.Syntax.SynType targetType
FSharp.Compiler.Syntax.SynSimplePat+Typed: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynSimplePat+Typed: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynSimplePat: Boolean IsAttrib
FSharp.Compiler.Syntax.SynSimplePat: Boolean IsId
FSharp.Compiler.Syntax.SynSimplePat: Boolean IsTyped
FSharp.Compiler.Syntax.SynSimplePat: Boolean get_IsAttrib()
FSharp.Compiler.Syntax.SynSimplePat: Boolean get_IsId()
FSharp.Compiler.Syntax.SynSimplePat: Boolean get_IsTyped()
FSharp.Compiler.Syntax.SynSimplePat: FSharp.Compiler.Syntax.SynSimplePat NewAttrib(FSharp.Compiler.Syntax.SynSimplePat, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynSimplePat: FSharp.Compiler.Syntax.SynSimplePat NewId(FSharp.Compiler.Syntax.Ident, Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.FSharpRef`1[FSharp.Compiler.Syntax.SynSimplePatAlternativeIdInfo]], Boolean, Boolean, Boolean, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynSimplePat: FSharp.Compiler.Syntax.SynSimplePat NewTyped(FSharp.Compiler.Syntax.SynSimplePat, FSharp.Compiler.Syntax.SynType, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynSimplePat: FSharp.Compiler.Syntax.SynSimplePat+Attrib
FSharp.Compiler.Syntax.SynSimplePat: FSharp.Compiler.Syntax.SynSimplePat+Id
FSharp.Compiler.Syntax.SynSimplePat: FSharp.Compiler.Syntax.SynSimplePat+Tags
FSharp.Compiler.Syntax.SynSimplePat: FSharp.Compiler.Syntax.SynSimplePat+Typed
FSharp.Compiler.Syntax.SynSimplePat: Int32 Tag
FSharp.Compiler.Syntax.SynSimplePat: Int32 get_Tag()
FSharp.Compiler.Syntax.SynSimplePat: System.String ToString()
FSharp.Compiler.Syntax.SynSimplePatAlternativeIdInfo
FSharp.Compiler.Syntax.SynSimplePatAlternativeIdInfo+Decided: FSharp.Compiler.Syntax.Ident Item
FSharp.Compiler.Syntax.SynSimplePatAlternativeIdInfo+Decided: FSharp.Compiler.Syntax.Ident get_Item()
FSharp.Compiler.Syntax.SynSimplePatAlternativeIdInfo+Tags: Int32 Decided
FSharp.Compiler.Syntax.SynSimplePatAlternativeIdInfo+Tags: Int32 Undecided
FSharp.Compiler.Syntax.SynSimplePatAlternativeIdInfo+Undecided: FSharp.Compiler.Syntax.Ident Item
FSharp.Compiler.Syntax.SynSimplePatAlternativeIdInfo+Undecided: FSharp.Compiler.Syntax.Ident get_Item()
FSharp.Compiler.Syntax.SynSimplePatAlternativeIdInfo: Boolean IsDecided
FSharp.Compiler.Syntax.SynSimplePatAlternativeIdInfo: Boolean IsUndecided
FSharp.Compiler.Syntax.SynSimplePatAlternativeIdInfo: Boolean get_IsDecided()
FSharp.Compiler.Syntax.SynSimplePatAlternativeIdInfo: Boolean get_IsUndecided()
FSharp.Compiler.Syntax.SynSimplePatAlternativeIdInfo: FSharp.Compiler.Syntax.SynSimplePatAlternativeIdInfo NewDecided(FSharp.Compiler.Syntax.Ident)
FSharp.Compiler.Syntax.SynSimplePatAlternativeIdInfo: FSharp.Compiler.Syntax.SynSimplePatAlternativeIdInfo NewUndecided(FSharp.Compiler.Syntax.Ident)
FSharp.Compiler.Syntax.SynSimplePatAlternativeIdInfo: FSharp.Compiler.Syntax.SynSimplePatAlternativeIdInfo+Decided
FSharp.Compiler.Syntax.SynSimplePatAlternativeIdInfo: FSharp.Compiler.Syntax.SynSimplePatAlternativeIdInfo+Tags
FSharp.Compiler.Syntax.SynSimplePatAlternativeIdInfo: FSharp.Compiler.Syntax.SynSimplePatAlternativeIdInfo+Undecided
FSharp.Compiler.Syntax.SynSimplePatAlternativeIdInfo: Int32 Tag
FSharp.Compiler.Syntax.SynSimplePatAlternativeIdInfo: Int32 get_Tag()
FSharp.Compiler.Syntax.SynSimplePatAlternativeIdInfo: System.String ToString()
FSharp.Compiler.Syntax.SynSimplePats
FSharp.Compiler.Syntax.SynSimplePats+SimplePats: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynSimplePats+SimplePats: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynSimplePats+SimplePats: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynSimplePat] get_pats()
FSharp.Compiler.Syntax.SynSimplePats+SimplePats: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynSimplePat] pats
FSharp.Compiler.Syntax.SynSimplePats+Tags: Int32 SimplePats
FSharp.Compiler.Syntax.SynSimplePats+Tags: Int32 Typed
FSharp.Compiler.Syntax.SynSimplePats+Typed: FSharp.Compiler.Syntax.SynSimplePats get_pats()
FSharp.Compiler.Syntax.SynSimplePats+Typed: FSharp.Compiler.Syntax.SynSimplePats pats
FSharp.Compiler.Syntax.SynSimplePats+Typed: FSharp.Compiler.Syntax.SynType get_targetType()
FSharp.Compiler.Syntax.SynSimplePats+Typed: FSharp.Compiler.Syntax.SynType targetType
FSharp.Compiler.Syntax.SynSimplePats+Typed: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynSimplePats+Typed: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynSimplePats: Boolean IsSimplePats
FSharp.Compiler.Syntax.SynSimplePats: Boolean IsTyped
FSharp.Compiler.Syntax.SynSimplePats: Boolean get_IsSimplePats()
FSharp.Compiler.Syntax.SynSimplePats: Boolean get_IsTyped()
FSharp.Compiler.Syntax.SynSimplePats: FSharp.Compiler.Syntax.SynSimplePats NewSimplePats(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynSimplePat], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynSimplePats: FSharp.Compiler.Syntax.SynSimplePats NewTyped(FSharp.Compiler.Syntax.SynSimplePats, FSharp.Compiler.Syntax.SynType, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynSimplePats: FSharp.Compiler.Syntax.SynSimplePats+SimplePats
FSharp.Compiler.Syntax.SynSimplePats: FSharp.Compiler.Syntax.SynSimplePats+Tags
FSharp.Compiler.Syntax.SynSimplePats: FSharp.Compiler.Syntax.SynSimplePats+Typed
FSharp.Compiler.Syntax.SynSimplePats: Int32 Tag
FSharp.Compiler.Syntax.SynSimplePats: Int32 get_Tag()
FSharp.Compiler.Syntax.SynSimplePats: System.String ToString()
FSharp.Compiler.Syntax.SynStaticOptimizationConstraint
FSharp.Compiler.Syntax.SynStaticOptimizationConstraint+Tags: Int32 WhenTyparIsStruct
FSharp.Compiler.Syntax.SynStaticOptimizationConstraint+Tags: Int32 WhenTyparTyconEqualsTycon
FSharp.Compiler.Syntax.SynStaticOptimizationConstraint+WhenTyparIsStruct: FSharp.Compiler.Syntax.SynTypar get_typar()
FSharp.Compiler.Syntax.SynStaticOptimizationConstraint+WhenTyparIsStruct: FSharp.Compiler.Syntax.SynTypar typar
FSharp.Compiler.Syntax.SynStaticOptimizationConstraint+WhenTyparIsStruct: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynStaticOptimizationConstraint+WhenTyparIsStruct: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynStaticOptimizationConstraint+WhenTyparTyconEqualsTycon: FSharp.Compiler.Syntax.SynTypar get_typar()
FSharp.Compiler.Syntax.SynStaticOptimizationConstraint+WhenTyparTyconEqualsTycon: FSharp.Compiler.Syntax.SynTypar typar
FSharp.Compiler.Syntax.SynStaticOptimizationConstraint+WhenTyparTyconEqualsTycon: FSharp.Compiler.Syntax.SynType get_rhsType()
FSharp.Compiler.Syntax.SynStaticOptimizationConstraint+WhenTyparTyconEqualsTycon: FSharp.Compiler.Syntax.SynType rhsType
FSharp.Compiler.Syntax.SynStaticOptimizationConstraint+WhenTyparTyconEqualsTycon: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynStaticOptimizationConstraint+WhenTyparTyconEqualsTycon: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynStaticOptimizationConstraint: Boolean IsWhenTyparIsStruct
FSharp.Compiler.Syntax.SynStaticOptimizationConstraint: Boolean IsWhenTyparTyconEqualsTycon
FSharp.Compiler.Syntax.SynStaticOptimizationConstraint: Boolean get_IsWhenTyparIsStruct()
FSharp.Compiler.Syntax.SynStaticOptimizationConstraint: Boolean get_IsWhenTyparTyconEqualsTycon()
FSharp.Compiler.Syntax.SynStaticOptimizationConstraint: FSharp.Compiler.Syntax.SynStaticOptimizationConstraint NewWhenTyparIsStruct(FSharp.Compiler.Syntax.SynTypar, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynStaticOptimizationConstraint: FSharp.Compiler.Syntax.SynStaticOptimizationConstraint NewWhenTyparTyconEqualsTycon(FSharp.Compiler.Syntax.SynTypar, FSharp.Compiler.Syntax.SynType, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynStaticOptimizationConstraint: FSharp.Compiler.Syntax.SynStaticOptimizationConstraint+Tags
FSharp.Compiler.Syntax.SynStaticOptimizationConstraint: FSharp.Compiler.Syntax.SynStaticOptimizationConstraint+WhenTyparIsStruct
FSharp.Compiler.Syntax.SynStaticOptimizationConstraint: FSharp.Compiler.Syntax.SynStaticOptimizationConstraint+WhenTyparTyconEqualsTycon
FSharp.Compiler.Syntax.SynStaticOptimizationConstraint: Int32 Tag
FSharp.Compiler.Syntax.SynStaticOptimizationConstraint: Int32 get_Tag()
FSharp.Compiler.Syntax.SynStaticOptimizationConstraint: System.String ToString()
FSharp.Compiler.Syntax.SynTypar
FSharp.Compiler.Syntax.SynTypar: Boolean get_isCompGen()
FSharp.Compiler.Syntax.SynTypar: Boolean isCompGen
FSharp.Compiler.Syntax.SynTypar: FSharp.Compiler.Syntax.Ident get_ident()
FSharp.Compiler.Syntax.SynTypar: FSharp.Compiler.Syntax.Ident ident
FSharp.Compiler.Syntax.SynTypar: FSharp.Compiler.Syntax.SynTypar NewTypar(FSharp.Compiler.Syntax.Ident, FSharp.Compiler.Syntax.TyparStaticReq, Boolean)
FSharp.Compiler.Syntax.SynTypar: FSharp.Compiler.Syntax.TyparStaticReq get_staticReq()
FSharp.Compiler.Syntax.SynTypar: FSharp.Compiler.Syntax.TyparStaticReq staticReq
FSharp.Compiler.Syntax.SynTypar: FSharp.Compiler.Text.Range Range
FSharp.Compiler.Syntax.SynTypar: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.Syntax.SynTypar: Int32 Tag
FSharp.Compiler.Syntax.SynTypar: Int32 get_Tag()
FSharp.Compiler.Syntax.SynTypar: System.String ToString()
FSharp.Compiler.Syntax.SynTyparDecl
FSharp.Compiler.Syntax.SynTyparDecl: FSharp.Compiler.Syntax.SynTypar Item2
FSharp.Compiler.Syntax.SynTyparDecl: FSharp.Compiler.Syntax.SynTypar get_Item2()
FSharp.Compiler.Syntax.SynTyparDecl: FSharp.Compiler.Syntax.SynTyparDecl NewTyparDecl(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList], FSharp.Compiler.Syntax.SynTypar)
FSharp.Compiler.Syntax.SynTyparDecl: Int32 Tag
FSharp.Compiler.Syntax.SynTyparDecl: Int32 get_Tag()
FSharp.Compiler.Syntax.SynTyparDecl: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList] attributes
FSharp.Compiler.Syntax.SynTyparDecl: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList] get_attributes()
FSharp.Compiler.Syntax.SynTyparDecl: System.String ToString()
FSharp.Compiler.Syntax.SynType
FSharp.Compiler.Syntax.SynType+Anon: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynType+Anon: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynType+AnonRecd: Boolean get_isStruct()
FSharp.Compiler.Syntax.SynType+AnonRecd: Boolean isStruct
FSharp.Compiler.Syntax.SynType+AnonRecd: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynType+AnonRecd: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynType+AnonRecd: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[FSharp.Compiler.Syntax.Ident,FSharp.Compiler.Syntax.SynType]] fields
FSharp.Compiler.Syntax.SynType+AnonRecd: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[FSharp.Compiler.Syntax.Ident,FSharp.Compiler.Syntax.SynType]] get_fields()
FSharp.Compiler.Syntax.SynType+App: Boolean get_isPostfix()
FSharp.Compiler.Syntax.SynType+App: Boolean isPostfix
FSharp.Compiler.Syntax.SynType+App: FSharp.Compiler.Syntax.SynType get_typeName()
FSharp.Compiler.Syntax.SynType+App: FSharp.Compiler.Syntax.SynType typeName
FSharp.Compiler.Syntax.SynType+App: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynType+App: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynType+App: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynType] get_typeArgs()
FSharp.Compiler.Syntax.SynType+App: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynType] typeArgs
FSharp.Compiler.Syntax.SynType+App: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Text.Range] commaRanges
FSharp.Compiler.Syntax.SynType+App: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Text.Range] get_commaRanges()
FSharp.Compiler.Syntax.SynType+App: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] get_greaterRange()
FSharp.Compiler.Syntax.SynType+App: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] get_lessRange()
FSharp.Compiler.Syntax.SynType+App: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] greaterRange
FSharp.Compiler.Syntax.SynType+App: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] lessRange
FSharp.Compiler.Syntax.SynType+Array: FSharp.Compiler.Syntax.SynType elementType
FSharp.Compiler.Syntax.SynType+Array: FSharp.Compiler.Syntax.SynType get_elementType()
FSharp.Compiler.Syntax.SynType+Array: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynType+Array: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynType+Array: Int32 get_rank()
FSharp.Compiler.Syntax.SynType+Array: Int32 rank
FSharp.Compiler.Syntax.SynType+Fun: FSharp.Compiler.Syntax.SynType argType
FSharp.Compiler.Syntax.SynType+Fun: FSharp.Compiler.Syntax.SynType get_argType()
FSharp.Compiler.Syntax.SynType+Fun: FSharp.Compiler.Syntax.SynType get_returnType()
FSharp.Compiler.Syntax.SynType+Fun: FSharp.Compiler.Syntax.SynType returnType
FSharp.Compiler.Syntax.SynType+Fun: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynType+Fun: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynType+HashConstraint: FSharp.Compiler.Syntax.SynType get_innerType()
FSharp.Compiler.Syntax.SynType+HashConstraint: FSharp.Compiler.Syntax.SynType innerType
FSharp.Compiler.Syntax.SynType+HashConstraint: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynType+HashConstraint: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynType+LongIdent: FSharp.Compiler.Syntax.LongIdentWithDots get_longDotId()
FSharp.Compiler.Syntax.SynType+LongIdent: FSharp.Compiler.Syntax.LongIdentWithDots longDotId
FSharp.Compiler.Syntax.SynType+LongIdentApp: FSharp.Compiler.Syntax.LongIdentWithDots get_longDotId()
FSharp.Compiler.Syntax.SynType+LongIdentApp: FSharp.Compiler.Syntax.LongIdentWithDots longDotId
FSharp.Compiler.Syntax.SynType+LongIdentApp: FSharp.Compiler.Syntax.SynType get_typeName()
FSharp.Compiler.Syntax.SynType+LongIdentApp: FSharp.Compiler.Syntax.SynType typeName
FSharp.Compiler.Syntax.SynType+LongIdentApp: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynType+LongIdentApp: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynType+LongIdentApp: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynType] get_typeArgs()
FSharp.Compiler.Syntax.SynType+LongIdentApp: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynType] typeArgs
FSharp.Compiler.Syntax.SynType+LongIdentApp: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Text.Range] commaRanges
FSharp.Compiler.Syntax.SynType+LongIdentApp: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Text.Range] get_commaRanges()
FSharp.Compiler.Syntax.SynType+LongIdentApp: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] get_greaterRange()
FSharp.Compiler.Syntax.SynType+LongIdentApp: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] get_lessRange()
FSharp.Compiler.Syntax.SynType+LongIdentApp: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] greaterRange
FSharp.Compiler.Syntax.SynType+LongIdentApp: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] lessRange
FSharp.Compiler.Syntax.SynType+MeasureDivide: FSharp.Compiler.Syntax.SynType dividend
FSharp.Compiler.Syntax.SynType+MeasureDivide: FSharp.Compiler.Syntax.SynType divisor
FSharp.Compiler.Syntax.SynType+MeasureDivide: FSharp.Compiler.Syntax.SynType get_dividend()
FSharp.Compiler.Syntax.SynType+MeasureDivide: FSharp.Compiler.Syntax.SynType get_divisor()
FSharp.Compiler.Syntax.SynType+MeasureDivide: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynType+MeasureDivide: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynType+MeasurePower: FSharp.Compiler.Syntax.SynRationalConst exponent
FSharp.Compiler.Syntax.SynType+MeasurePower: FSharp.Compiler.Syntax.SynRationalConst get_exponent()
FSharp.Compiler.Syntax.SynType+MeasurePower: FSharp.Compiler.Syntax.SynType baseMeasure
FSharp.Compiler.Syntax.SynType+MeasurePower: FSharp.Compiler.Syntax.SynType get_baseMeasure()
FSharp.Compiler.Syntax.SynType+MeasurePower: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynType+MeasurePower: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynType+Paren: FSharp.Compiler.Syntax.SynType get_innerType()
FSharp.Compiler.Syntax.SynType+Paren: FSharp.Compiler.Syntax.SynType innerType
FSharp.Compiler.Syntax.SynType+Paren: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynType+Paren: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynType+StaticConstant: FSharp.Compiler.Syntax.SynConst constant
FSharp.Compiler.Syntax.SynType+StaticConstant: FSharp.Compiler.Syntax.SynConst get_constant()
FSharp.Compiler.Syntax.SynType+StaticConstant: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynType+StaticConstant: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynType+StaticConstantExpr: FSharp.Compiler.Syntax.SynExpr expr
FSharp.Compiler.Syntax.SynType+StaticConstantExpr: FSharp.Compiler.Syntax.SynExpr get_expr()
FSharp.Compiler.Syntax.SynType+StaticConstantExpr: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynType+StaticConstantExpr: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynType+StaticConstantNamed: FSharp.Compiler.Syntax.SynType get_ident()
FSharp.Compiler.Syntax.SynType+StaticConstantNamed: FSharp.Compiler.Syntax.SynType get_value()
FSharp.Compiler.Syntax.SynType+StaticConstantNamed: FSharp.Compiler.Syntax.SynType ident
FSharp.Compiler.Syntax.SynType+StaticConstantNamed: FSharp.Compiler.Syntax.SynType value
FSharp.Compiler.Syntax.SynType+StaticConstantNamed: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynType+StaticConstantNamed: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynType+Tags: Int32 Anon
FSharp.Compiler.Syntax.SynType+Tags: Int32 AnonRecd
FSharp.Compiler.Syntax.SynType+Tags: Int32 App
FSharp.Compiler.Syntax.SynType+Tags: Int32 Array
FSharp.Compiler.Syntax.SynType+Tags: Int32 Fun
FSharp.Compiler.Syntax.SynType+Tags: Int32 HashConstraint
FSharp.Compiler.Syntax.SynType+Tags: Int32 LongIdent
FSharp.Compiler.Syntax.SynType+Tags: Int32 LongIdentApp
FSharp.Compiler.Syntax.SynType+Tags: Int32 MeasureDivide
FSharp.Compiler.Syntax.SynType+Tags: Int32 MeasurePower
FSharp.Compiler.Syntax.SynType+Tags: Int32 Paren
FSharp.Compiler.Syntax.SynType+Tags: Int32 StaticConstant
FSharp.Compiler.Syntax.SynType+Tags: Int32 StaticConstantExpr
FSharp.Compiler.Syntax.SynType+Tags: Int32 StaticConstantNamed
FSharp.Compiler.Syntax.SynType+Tags: Int32 Tuple
FSharp.Compiler.Syntax.SynType+Tags: Int32 Var
FSharp.Compiler.Syntax.SynType+Tags: Int32 WithGlobalConstraints
FSharp.Compiler.Syntax.SynType+Tuple: Boolean get_isStruct()
FSharp.Compiler.Syntax.SynType+Tuple: Boolean isStruct
FSharp.Compiler.Syntax.SynType+Tuple: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynType+Tuple: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynType+Tuple: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[System.Boolean,FSharp.Compiler.Syntax.SynType]] elementTypes
FSharp.Compiler.Syntax.SynType+Tuple: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[System.Boolean,FSharp.Compiler.Syntax.SynType]] get_elementTypes()
FSharp.Compiler.Syntax.SynType+Var: FSharp.Compiler.Syntax.SynTypar get_typar()
FSharp.Compiler.Syntax.SynType+Var: FSharp.Compiler.Syntax.SynTypar typar
FSharp.Compiler.Syntax.SynType+Var: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynType+Var: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynType+WithGlobalConstraints: FSharp.Compiler.Syntax.SynType get_typeName()
FSharp.Compiler.Syntax.SynType+WithGlobalConstraints: FSharp.Compiler.Syntax.SynType typeName
FSharp.Compiler.Syntax.SynType+WithGlobalConstraints: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynType+WithGlobalConstraints: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynType+WithGlobalConstraints: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynTypeConstraint] constraints
FSharp.Compiler.Syntax.SynType+WithGlobalConstraints: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynTypeConstraint] get_constraints()
FSharp.Compiler.Syntax.SynType: Boolean IsAnon
FSharp.Compiler.Syntax.SynType: Boolean IsAnonRecd
FSharp.Compiler.Syntax.SynType: Boolean IsApp
FSharp.Compiler.Syntax.SynType: Boolean IsArray
FSharp.Compiler.Syntax.SynType: Boolean IsFun
FSharp.Compiler.Syntax.SynType: Boolean IsHashConstraint
FSharp.Compiler.Syntax.SynType: Boolean IsLongIdent
FSharp.Compiler.Syntax.SynType: Boolean IsLongIdentApp
FSharp.Compiler.Syntax.SynType: Boolean IsMeasureDivide
FSharp.Compiler.Syntax.SynType: Boolean IsMeasurePower
FSharp.Compiler.Syntax.SynType: Boolean IsParen
FSharp.Compiler.Syntax.SynType: Boolean IsStaticConstant
FSharp.Compiler.Syntax.SynType: Boolean IsStaticConstantExpr
FSharp.Compiler.Syntax.SynType: Boolean IsStaticConstantNamed
FSharp.Compiler.Syntax.SynType: Boolean IsTuple
FSharp.Compiler.Syntax.SynType: Boolean IsVar
FSharp.Compiler.Syntax.SynType: Boolean IsWithGlobalConstraints
FSharp.Compiler.Syntax.SynType: Boolean get_IsAnon()
FSharp.Compiler.Syntax.SynType: Boolean get_IsAnonRecd()
FSharp.Compiler.Syntax.SynType: Boolean get_IsApp()
FSharp.Compiler.Syntax.SynType: Boolean get_IsArray()
FSharp.Compiler.Syntax.SynType: Boolean get_IsFun()
FSharp.Compiler.Syntax.SynType: Boolean get_IsHashConstraint()
FSharp.Compiler.Syntax.SynType: Boolean get_IsLongIdent()
FSharp.Compiler.Syntax.SynType: Boolean get_IsLongIdentApp()
FSharp.Compiler.Syntax.SynType: Boolean get_IsMeasureDivide()
FSharp.Compiler.Syntax.SynType: Boolean get_IsMeasurePower()
FSharp.Compiler.Syntax.SynType: Boolean get_IsParen()
FSharp.Compiler.Syntax.SynType: Boolean get_IsStaticConstant()
FSharp.Compiler.Syntax.SynType: Boolean get_IsStaticConstantExpr()
FSharp.Compiler.Syntax.SynType: Boolean get_IsStaticConstantNamed()
FSharp.Compiler.Syntax.SynType: Boolean get_IsTuple()
FSharp.Compiler.Syntax.SynType: Boolean get_IsVar()
FSharp.Compiler.Syntax.SynType: Boolean get_IsWithGlobalConstraints()
FSharp.Compiler.Syntax.SynType: FSharp.Compiler.Syntax.SynType NewAnon(FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynType: FSharp.Compiler.Syntax.SynType NewAnonRecd(Boolean, Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[FSharp.Compiler.Syntax.Ident,FSharp.Compiler.Syntax.SynType]], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynType: FSharp.Compiler.Syntax.SynType NewApp(FSharp.Compiler.Syntax.SynType, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynType], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Text.Range], Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range], Boolean, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynType: FSharp.Compiler.Syntax.SynType NewArray(Int32, FSharp.Compiler.Syntax.SynType, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynType: FSharp.Compiler.Syntax.SynType NewFun(FSharp.Compiler.Syntax.SynType, FSharp.Compiler.Syntax.SynType, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynType: FSharp.Compiler.Syntax.SynType NewHashConstraint(FSharp.Compiler.Syntax.SynType, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynType: FSharp.Compiler.Syntax.SynType NewLongIdent(FSharp.Compiler.Syntax.LongIdentWithDots)
FSharp.Compiler.Syntax.SynType: FSharp.Compiler.Syntax.SynType NewLongIdentApp(FSharp.Compiler.Syntax.SynType, FSharp.Compiler.Syntax.LongIdentWithDots, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynType], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Text.Range], Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynType: FSharp.Compiler.Syntax.SynType NewMeasureDivide(FSharp.Compiler.Syntax.SynType, FSharp.Compiler.Syntax.SynType, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynType: FSharp.Compiler.Syntax.SynType NewMeasurePower(FSharp.Compiler.Syntax.SynType, FSharp.Compiler.Syntax.SynRationalConst, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynType: FSharp.Compiler.Syntax.SynType NewParen(FSharp.Compiler.Syntax.SynType, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynType: FSharp.Compiler.Syntax.SynType NewStaticConstant(FSharp.Compiler.Syntax.SynConst, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynType: FSharp.Compiler.Syntax.SynType NewStaticConstantExpr(FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynType: FSharp.Compiler.Syntax.SynType NewStaticConstantNamed(FSharp.Compiler.Syntax.SynType, FSharp.Compiler.Syntax.SynType, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynType: FSharp.Compiler.Syntax.SynType NewTuple(Boolean, Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[System.Boolean,FSharp.Compiler.Syntax.SynType]], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynType: FSharp.Compiler.Syntax.SynType NewVar(FSharp.Compiler.Syntax.SynTypar, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynType: FSharp.Compiler.Syntax.SynType NewWithGlobalConstraints(FSharp.Compiler.Syntax.SynType, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynTypeConstraint], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynType: FSharp.Compiler.Syntax.SynType+Anon
FSharp.Compiler.Syntax.SynType: FSharp.Compiler.Syntax.SynType+AnonRecd
FSharp.Compiler.Syntax.SynType: FSharp.Compiler.Syntax.SynType+App
FSharp.Compiler.Syntax.SynType: FSharp.Compiler.Syntax.SynType+Array
FSharp.Compiler.Syntax.SynType: FSharp.Compiler.Syntax.SynType+Fun
FSharp.Compiler.Syntax.SynType: FSharp.Compiler.Syntax.SynType+HashConstraint
FSharp.Compiler.Syntax.SynType: FSharp.Compiler.Syntax.SynType+LongIdent
FSharp.Compiler.Syntax.SynType: FSharp.Compiler.Syntax.SynType+LongIdentApp
FSharp.Compiler.Syntax.SynType: FSharp.Compiler.Syntax.SynType+MeasureDivide
FSharp.Compiler.Syntax.SynType: FSharp.Compiler.Syntax.SynType+MeasurePower
FSharp.Compiler.Syntax.SynType: FSharp.Compiler.Syntax.SynType+Paren
FSharp.Compiler.Syntax.SynType: FSharp.Compiler.Syntax.SynType+StaticConstant
FSharp.Compiler.Syntax.SynType: FSharp.Compiler.Syntax.SynType+StaticConstantExpr
FSharp.Compiler.Syntax.SynType: FSharp.Compiler.Syntax.SynType+StaticConstantNamed
FSharp.Compiler.Syntax.SynType: FSharp.Compiler.Syntax.SynType+Tags
FSharp.Compiler.Syntax.SynType: FSharp.Compiler.Syntax.SynType+Tuple
FSharp.Compiler.Syntax.SynType: FSharp.Compiler.Syntax.SynType+Var
FSharp.Compiler.Syntax.SynType: FSharp.Compiler.Syntax.SynType+WithGlobalConstraints
FSharp.Compiler.Syntax.SynType: FSharp.Compiler.Text.Range Range
FSharp.Compiler.Syntax.SynType: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.Syntax.SynType: Int32 Tag
FSharp.Compiler.Syntax.SynType: Int32 get_Tag()
FSharp.Compiler.Syntax.SynType: System.String ToString()
FSharp.Compiler.Syntax.SynTypeConstraint
FSharp.Compiler.Syntax.SynTypeConstraint+Tags: Int32 WhereTyparDefaultsToType
FSharp.Compiler.Syntax.SynTypeConstraint+Tags: Int32 WhereTyparIsComparable
FSharp.Compiler.Syntax.SynTypeConstraint+Tags: Int32 WhereTyparIsDelegate
FSharp.Compiler.Syntax.SynTypeConstraint+Tags: Int32 WhereTyparIsEnum
FSharp.Compiler.Syntax.SynTypeConstraint+Tags: Int32 WhereTyparIsEquatable
FSharp.Compiler.Syntax.SynTypeConstraint+Tags: Int32 WhereTyparIsReferenceType
FSharp.Compiler.Syntax.SynTypeConstraint+Tags: Int32 WhereTyparIsUnmanaged
FSharp.Compiler.Syntax.SynTypeConstraint+Tags: Int32 WhereTyparIsValueType
FSharp.Compiler.Syntax.SynTypeConstraint+Tags: Int32 WhereTyparSubtypeOfType
FSharp.Compiler.Syntax.SynTypeConstraint+Tags: Int32 WhereTyparSupportsMember
FSharp.Compiler.Syntax.SynTypeConstraint+Tags: Int32 WhereTyparSupportsNull
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparDefaultsToType: FSharp.Compiler.Syntax.SynTypar get_typar()
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparDefaultsToType: FSharp.Compiler.Syntax.SynTypar typar
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparDefaultsToType: FSharp.Compiler.Syntax.SynType get_typeName()
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparDefaultsToType: FSharp.Compiler.Syntax.SynType typeName
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparDefaultsToType: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparDefaultsToType: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparIsComparable: FSharp.Compiler.Syntax.SynTypar get_typar()
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparIsComparable: FSharp.Compiler.Syntax.SynTypar typar
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparIsComparable: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparIsComparable: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparIsDelegate: FSharp.Compiler.Syntax.SynTypar get_typar()
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparIsDelegate: FSharp.Compiler.Syntax.SynTypar typar
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparIsDelegate: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparIsDelegate: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparIsDelegate: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynType] get_typeArgs()
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparIsDelegate: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynType] typeArgs
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparIsEnum: FSharp.Compiler.Syntax.SynTypar get_typar()
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparIsEnum: FSharp.Compiler.Syntax.SynTypar typar
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparIsEnum: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparIsEnum: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparIsEnum: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynType] get_typeArgs()
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparIsEnum: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynType] typeArgs
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparIsEquatable: FSharp.Compiler.Syntax.SynTypar get_typar()
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparIsEquatable: FSharp.Compiler.Syntax.SynTypar typar
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparIsEquatable: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparIsEquatable: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparIsReferenceType: FSharp.Compiler.Syntax.SynTypar get_typar()
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparIsReferenceType: FSharp.Compiler.Syntax.SynTypar typar
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparIsReferenceType: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparIsReferenceType: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparIsUnmanaged: FSharp.Compiler.Syntax.SynTypar get_typar()
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparIsUnmanaged: FSharp.Compiler.Syntax.SynTypar typar
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparIsUnmanaged: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparIsUnmanaged: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparIsValueType: FSharp.Compiler.Syntax.SynTypar get_typar()
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparIsValueType: FSharp.Compiler.Syntax.SynTypar typar
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparIsValueType: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparIsValueType: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparSubtypeOfType: FSharp.Compiler.Syntax.SynTypar get_typar()
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparSubtypeOfType: FSharp.Compiler.Syntax.SynTypar typar
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparSubtypeOfType: FSharp.Compiler.Syntax.SynType get_typeName()
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparSubtypeOfType: FSharp.Compiler.Syntax.SynType typeName
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparSubtypeOfType: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparSubtypeOfType: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparSupportsMember: FSharp.Compiler.Syntax.SynMemberSig get_memberSig()
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparSupportsMember: FSharp.Compiler.Syntax.SynMemberSig memberSig
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparSupportsMember: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparSupportsMember: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparSupportsMember: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynType] get_typars()
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparSupportsMember: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynType] typars
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparSupportsNull: FSharp.Compiler.Syntax.SynTypar get_typar()
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparSupportsNull: FSharp.Compiler.Syntax.SynTypar typar
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparSupportsNull: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparSupportsNull: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynTypeConstraint: Boolean IsWhereTyparDefaultsToType
FSharp.Compiler.Syntax.SynTypeConstraint: Boolean IsWhereTyparIsComparable
FSharp.Compiler.Syntax.SynTypeConstraint: Boolean IsWhereTyparIsDelegate
FSharp.Compiler.Syntax.SynTypeConstraint: Boolean IsWhereTyparIsEnum
FSharp.Compiler.Syntax.SynTypeConstraint: Boolean IsWhereTyparIsEquatable
FSharp.Compiler.Syntax.SynTypeConstraint: Boolean IsWhereTyparIsReferenceType
FSharp.Compiler.Syntax.SynTypeConstraint: Boolean IsWhereTyparIsUnmanaged
FSharp.Compiler.Syntax.SynTypeConstraint: Boolean IsWhereTyparIsValueType
FSharp.Compiler.Syntax.SynTypeConstraint: Boolean IsWhereTyparSubtypeOfType
FSharp.Compiler.Syntax.SynTypeConstraint: Boolean IsWhereTyparSupportsMember
FSharp.Compiler.Syntax.SynTypeConstraint: Boolean IsWhereTyparSupportsNull
FSharp.Compiler.Syntax.SynTypeConstraint: Boolean get_IsWhereTyparDefaultsToType()
FSharp.Compiler.Syntax.SynTypeConstraint: Boolean get_IsWhereTyparIsComparable()
FSharp.Compiler.Syntax.SynTypeConstraint: Boolean get_IsWhereTyparIsDelegate()
FSharp.Compiler.Syntax.SynTypeConstraint: Boolean get_IsWhereTyparIsEnum()
FSharp.Compiler.Syntax.SynTypeConstraint: Boolean get_IsWhereTyparIsEquatable()
FSharp.Compiler.Syntax.SynTypeConstraint: Boolean get_IsWhereTyparIsReferenceType()
FSharp.Compiler.Syntax.SynTypeConstraint: Boolean get_IsWhereTyparIsUnmanaged()
FSharp.Compiler.Syntax.SynTypeConstraint: Boolean get_IsWhereTyparIsValueType()
FSharp.Compiler.Syntax.SynTypeConstraint: Boolean get_IsWhereTyparSubtypeOfType()
FSharp.Compiler.Syntax.SynTypeConstraint: Boolean get_IsWhereTyparSupportsMember()
FSharp.Compiler.Syntax.SynTypeConstraint: Boolean get_IsWhereTyparSupportsNull()
FSharp.Compiler.Syntax.SynTypeConstraint: FSharp.Compiler.Syntax.SynTypeConstraint NewWhereTyparDefaultsToType(FSharp.Compiler.Syntax.SynTypar, FSharp.Compiler.Syntax.SynType, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynTypeConstraint: FSharp.Compiler.Syntax.SynTypeConstraint NewWhereTyparIsComparable(FSharp.Compiler.Syntax.SynTypar, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynTypeConstraint: FSharp.Compiler.Syntax.SynTypeConstraint NewWhereTyparIsDelegate(FSharp.Compiler.Syntax.SynTypar, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynType], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynTypeConstraint: FSharp.Compiler.Syntax.SynTypeConstraint NewWhereTyparIsEnum(FSharp.Compiler.Syntax.SynTypar, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynType], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynTypeConstraint: FSharp.Compiler.Syntax.SynTypeConstraint NewWhereTyparIsEquatable(FSharp.Compiler.Syntax.SynTypar, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynTypeConstraint: FSharp.Compiler.Syntax.SynTypeConstraint NewWhereTyparIsReferenceType(FSharp.Compiler.Syntax.SynTypar, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynTypeConstraint: FSharp.Compiler.Syntax.SynTypeConstraint NewWhereTyparIsUnmanaged(FSharp.Compiler.Syntax.SynTypar, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynTypeConstraint: FSharp.Compiler.Syntax.SynTypeConstraint NewWhereTyparIsValueType(FSharp.Compiler.Syntax.SynTypar, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynTypeConstraint: FSharp.Compiler.Syntax.SynTypeConstraint NewWhereTyparSubtypeOfType(FSharp.Compiler.Syntax.SynTypar, FSharp.Compiler.Syntax.SynType, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynTypeConstraint: FSharp.Compiler.Syntax.SynTypeConstraint NewWhereTyparSupportsMember(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynType], FSharp.Compiler.Syntax.SynMemberSig, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynTypeConstraint: FSharp.Compiler.Syntax.SynTypeConstraint NewWhereTyparSupportsNull(FSharp.Compiler.Syntax.SynTypar, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynTypeConstraint: FSharp.Compiler.Syntax.SynTypeConstraint+Tags
FSharp.Compiler.Syntax.SynTypeConstraint: FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparDefaultsToType
FSharp.Compiler.Syntax.SynTypeConstraint: FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparIsComparable
FSharp.Compiler.Syntax.SynTypeConstraint: FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparIsDelegate
FSharp.Compiler.Syntax.SynTypeConstraint: FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparIsEnum
FSharp.Compiler.Syntax.SynTypeConstraint: FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparIsEquatable
FSharp.Compiler.Syntax.SynTypeConstraint: FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparIsReferenceType
FSharp.Compiler.Syntax.SynTypeConstraint: FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparIsUnmanaged
FSharp.Compiler.Syntax.SynTypeConstraint: FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparIsValueType
FSharp.Compiler.Syntax.SynTypeConstraint: FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparSubtypeOfType
FSharp.Compiler.Syntax.SynTypeConstraint: FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparSupportsMember
FSharp.Compiler.Syntax.SynTypeConstraint: FSharp.Compiler.Syntax.SynTypeConstraint+WhereTyparSupportsNull
FSharp.Compiler.Syntax.SynTypeConstraint: Int32 Tag
FSharp.Compiler.Syntax.SynTypeConstraint: Int32 get_Tag()
FSharp.Compiler.Syntax.SynTypeConstraint: System.String ToString()
FSharp.Compiler.Syntax.SynTypeDefn
FSharp.Compiler.Syntax.SynTypeDefn: FSharp.Compiler.Syntax.SynComponentInfo get_typeInfo()
FSharp.Compiler.Syntax.SynTypeDefn: FSharp.Compiler.Syntax.SynComponentInfo typeInfo
FSharp.Compiler.Syntax.SynTypeDefn: FSharp.Compiler.Syntax.SynTypeDefn NewTypeDefn(FSharp.Compiler.Syntax.SynComponentInfo, FSharp.Compiler.Syntax.SynTypeDefnRepr, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynMemberDefn], Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynMemberDefn], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynTypeDefn: FSharp.Compiler.Syntax.SynTypeDefnRepr get_typeRepr()
FSharp.Compiler.Syntax.SynTypeDefn: FSharp.Compiler.Syntax.SynTypeDefnRepr typeRepr
FSharp.Compiler.Syntax.SynTypeDefn: FSharp.Compiler.Text.Range Range
FSharp.Compiler.Syntax.SynTypeDefn: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.Syntax.SynTypeDefn: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynTypeDefn: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynTypeDefn: Int32 Tag
FSharp.Compiler.Syntax.SynTypeDefn: Int32 get_Tag()
FSharp.Compiler.Syntax.SynTypeDefn: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynMemberDefn] get_members()
FSharp.Compiler.Syntax.SynTypeDefn: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynMemberDefn] members
FSharp.Compiler.Syntax.SynTypeDefn: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynMemberDefn] get_implicitConstructor()
FSharp.Compiler.Syntax.SynTypeDefn: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynMemberDefn] implicitConstructor
FSharp.Compiler.Syntax.SynTypeDefn: System.String ToString()
FSharp.Compiler.Syntax.SynTypeDefnKind
FSharp.Compiler.Syntax.SynTypeDefnKind+Tags: Int32 SynTypeDefnKind.Abbrev
FSharp.Compiler.Syntax.SynTypeDefnKind+Tags: Int32 SynTypeDefnKind.Augmentation
FSharp.Compiler.Syntax.SynTypeDefnKind+Tags: Int32 SynTypeDefnKind.Class
FSharp.Compiler.Syntax.SynTypeDefnKind+Tags: Int32 SynTypeDefnKind.Delegate
FSharp.Compiler.Syntax.SynTypeDefnKind+Tags: Int32 SynTypeDefnKind.Opaque
FSharp.Compiler.Syntax.SynTypeDefnKind+Tags: Int32 SynTypeDefnKind.IL
FSharp.Compiler.Syntax.SynTypeDefnKind+Tags: Int32 SynTypeDefnKind.Interface
FSharp.Compiler.Syntax.SynTypeDefnKind+Tags: Int32 SynTypeDefnKind.Record
FSharp.Compiler.Syntax.SynTypeDefnKind+Tags: Int32 SynTypeDefnKind.Struct
FSharp.Compiler.Syntax.SynTypeDefnKind+Tags: Int32 SynTypeDefnKind.Union
FSharp.Compiler.Syntax.SynTypeDefnKind+Tags: Int32 SynTypeDefnKind.Unspecified
FSharp.Compiler.Syntax.SynTypeDefnKind+SynTypeDefnKind.Delegate: FSharp.Compiler.Syntax.SynType get_signature()
FSharp.Compiler.Syntax.SynTypeDefnKind+SynTypeDefnKind.Delegate: FSharp.Compiler.Syntax.SynType signature
FSharp.Compiler.Syntax.SynTypeDefnKind+SynTypeDefnKind.Delegate: FSharp.Compiler.Syntax.SynValInfo get_signatureInfo()
FSharp.Compiler.Syntax.SynTypeDefnKind+SynTypeDefnKind.Delegate: FSharp.Compiler.Syntax.SynValInfo signatureInfo
FSharp.Compiler.Syntax.SynTypeDefnKind: Boolean IsTyconAbbrev
FSharp.Compiler.Syntax.SynTypeDefnKind: Boolean IsTyconAugmentation
FSharp.Compiler.Syntax.SynTypeDefnKind: Boolean IsTyconClass
FSharp.Compiler.Syntax.SynTypeDefnKind: Boolean IsTyconDelegate
FSharp.Compiler.Syntax.SynTypeDefnKind: Boolean IsTyconHiddenRepr
FSharp.Compiler.Syntax.SynTypeDefnKind: Boolean IsTyconILAssemblyCode
FSharp.Compiler.Syntax.SynTypeDefnKind: Boolean IsTyconInterface
FSharp.Compiler.Syntax.SynTypeDefnKind: Boolean IsTyconRecord
FSharp.Compiler.Syntax.SynTypeDefnKind: Boolean IsTyconStruct
FSharp.Compiler.Syntax.SynTypeDefnKind: Boolean IsTyconUnion
FSharp.Compiler.Syntax.SynTypeDefnKind: Boolean IsTyconUnspecified
FSharp.Compiler.Syntax.SynTypeDefnKind: Boolean get_IsTyconAbbrev()
FSharp.Compiler.Syntax.SynTypeDefnKind: Boolean get_IsTyconAugmentation()
FSharp.Compiler.Syntax.SynTypeDefnKind: Boolean get_IsTyconClass()
FSharp.Compiler.Syntax.SynTypeDefnKind: Boolean get_IsTyconDelegate()
FSharp.Compiler.Syntax.SynTypeDefnKind: Boolean get_IsTyconHiddenRepr()
FSharp.Compiler.Syntax.SynTypeDefnKind: Boolean get_IsTyconILAssemblyCode()
FSharp.Compiler.Syntax.SynTypeDefnKind: Boolean get_IsTyconInterface()
FSharp.Compiler.Syntax.SynTypeDefnKind: Boolean get_IsTyconRecord()
FSharp.Compiler.Syntax.SynTypeDefnKind: Boolean get_IsTyconStruct()
FSharp.Compiler.Syntax.SynTypeDefnKind: Boolean get_IsTyconUnion()
FSharp.Compiler.Syntax.SynTypeDefnKind: Boolean get_IsTyconUnspecified()
FSharp.Compiler.Syntax.SynTypeDefnKind: FSharp.Compiler.Syntax.SynTypeDefnKind NewTyconDelegate(FSharp.Compiler.Syntax.SynType, FSharp.Compiler.Syntax.SynValInfo)
FSharp.Compiler.Syntax.SynTypeDefnKind: FSharp.Compiler.Syntax.SynTypeDefnKind SynTypeDefnKind.Abbrev
FSharp.Compiler.Syntax.SynTypeDefnKind: FSharp.Compiler.Syntax.SynTypeDefnKind SynTypeDefnKind.Augmentation
FSharp.Compiler.Syntax.SynTypeDefnKind: FSharp.Compiler.Syntax.SynTypeDefnKind SynTypeDefnKind.Class
FSharp.Compiler.Syntax.SynTypeDefnKind: FSharp.Compiler.Syntax.SynTypeDefnKind SynTypeDefnKind.Opaque
FSharp.Compiler.Syntax.SynTypeDefnKind: FSharp.Compiler.Syntax.SynTypeDefnKind SynTypeDefnKind.IL
FSharp.Compiler.Syntax.SynTypeDefnKind: FSharp.Compiler.Syntax.SynTypeDefnKind SynTypeDefnKind.Interface
FSharp.Compiler.Syntax.SynTypeDefnKind: FSharp.Compiler.Syntax.SynTypeDefnKind SynTypeDefnKind.Record
FSharp.Compiler.Syntax.SynTypeDefnKind: FSharp.Compiler.Syntax.SynTypeDefnKind SynTypeDefnKind.Struct
FSharp.Compiler.Syntax.SynTypeDefnKind: FSharp.Compiler.Syntax.SynTypeDefnKind SynTypeDefnKind.Union
FSharp.Compiler.Syntax.SynTypeDefnKind: FSharp.Compiler.Syntax.SynTypeDefnKind SynTypeDefnKind.Unspecified
FSharp.Compiler.Syntax.SynTypeDefnKind: FSharp.Compiler.Syntax.SynTypeDefnKind get_TyconAbbrev()
FSharp.Compiler.Syntax.SynTypeDefnKind: FSharp.Compiler.Syntax.SynTypeDefnKind get_TyconAugmentation()
FSharp.Compiler.Syntax.SynTypeDefnKind: FSharp.Compiler.Syntax.SynTypeDefnKind get_TyconClass()
FSharp.Compiler.Syntax.SynTypeDefnKind: FSharp.Compiler.Syntax.SynTypeDefnKind get_TyconHiddenRepr()
FSharp.Compiler.Syntax.SynTypeDefnKind: FSharp.Compiler.Syntax.SynTypeDefnKind get_TyconILAssemblyCode()
FSharp.Compiler.Syntax.SynTypeDefnKind: FSharp.Compiler.Syntax.SynTypeDefnKind get_TyconInterface()
FSharp.Compiler.Syntax.SynTypeDefnKind: FSharp.Compiler.Syntax.SynTypeDefnKind get_TyconRecord()
FSharp.Compiler.Syntax.SynTypeDefnKind: FSharp.Compiler.Syntax.SynTypeDefnKind get_TyconStruct()
FSharp.Compiler.Syntax.SynTypeDefnKind: FSharp.Compiler.Syntax.SynTypeDefnKind get_TyconUnion()
FSharp.Compiler.Syntax.SynTypeDefnKind: FSharp.Compiler.Syntax.SynTypeDefnKind get_TyconUnspecified()
FSharp.Compiler.Syntax.SynTypeDefnKind: FSharp.Compiler.Syntax.SynTypeDefnKind+Tags
FSharp.Compiler.Syntax.SynTypeDefnKind: FSharp.Compiler.Syntax.SynTypeDefnKind+SynTypeDefnKind.Delegate
FSharp.Compiler.Syntax.SynTypeDefnKind: Int32 Tag
FSharp.Compiler.Syntax.SynTypeDefnKind: Int32 get_Tag()
FSharp.Compiler.Syntax.SynTypeDefnKind: System.String ToString()
FSharp.Compiler.Syntax.SynTypeDefnRepr
FSharp.Compiler.Syntax.SynTypeDefnRepr+Exception: FSharp.Compiler.Syntax.SynExceptionDefnRepr exnRepr
FSharp.Compiler.Syntax.SynTypeDefnRepr+Exception: FSharp.Compiler.Syntax.SynExceptionDefnRepr get_exnRepr()
FSharp.Compiler.Syntax.SynTypeDefnRepr+ObjectModel: FSharp.Compiler.Syntax.SynTypeDefnKind get_kind()
FSharp.Compiler.Syntax.SynTypeDefnRepr+ObjectModel: FSharp.Compiler.Syntax.SynTypeDefnKind kind
FSharp.Compiler.Syntax.SynTypeDefnRepr+ObjectModel: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynTypeDefnRepr+ObjectModel: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynTypeDefnRepr+ObjectModel: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynMemberDefn] get_members()
FSharp.Compiler.Syntax.SynTypeDefnRepr+ObjectModel: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynMemberDefn] members
FSharp.Compiler.Syntax.SynTypeDefnRepr+Simple: FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr get_simpleRepr()
FSharp.Compiler.Syntax.SynTypeDefnRepr+Simple: FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr simpleRepr
FSharp.Compiler.Syntax.SynTypeDefnRepr+Simple: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynTypeDefnRepr+Simple: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynTypeDefnRepr+Tags: Int32 Exception
FSharp.Compiler.Syntax.SynTypeDefnRepr+Tags: Int32 ObjectModel
FSharp.Compiler.Syntax.SynTypeDefnRepr+Tags: Int32 Simple
FSharp.Compiler.Syntax.SynTypeDefnRepr: Boolean IsException
FSharp.Compiler.Syntax.SynTypeDefnRepr: Boolean IsObjectModel
FSharp.Compiler.Syntax.SynTypeDefnRepr: Boolean IsSimple
FSharp.Compiler.Syntax.SynTypeDefnRepr: Boolean get_IsException()
FSharp.Compiler.Syntax.SynTypeDefnRepr: Boolean get_IsObjectModel()
FSharp.Compiler.Syntax.SynTypeDefnRepr: Boolean get_IsSimple()
FSharp.Compiler.Syntax.SynTypeDefnRepr: FSharp.Compiler.Syntax.SynTypeDefnRepr NewException(FSharp.Compiler.Syntax.SynExceptionDefnRepr)
FSharp.Compiler.Syntax.SynTypeDefnRepr: FSharp.Compiler.Syntax.SynTypeDefnRepr NewObjectModel(FSharp.Compiler.Syntax.SynTypeDefnKind, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynMemberDefn], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynTypeDefnRepr: FSharp.Compiler.Syntax.SynTypeDefnRepr NewSimple(FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynTypeDefnRepr: FSharp.Compiler.Syntax.SynTypeDefnRepr+Exception
FSharp.Compiler.Syntax.SynTypeDefnRepr: FSharp.Compiler.Syntax.SynTypeDefnRepr+ObjectModel
FSharp.Compiler.Syntax.SynTypeDefnRepr: FSharp.Compiler.Syntax.SynTypeDefnRepr+Simple
FSharp.Compiler.Syntax.SynTypeDefnRepr: FSharp.Compiler.Syntax.SynTypeDefnRepr+Tags
FSharp.Compiler.Syntax.SynTypeDefnRepr: FSharp.Compiler.Text.Range Range
FSharp.Compiler.Syntax.SynTypeDefnRepr: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.Syntax.SynTypeDefnRepr: Int32 Tag
FSharp.Compiler.Syntax.SynTypeDefnRepr: Int32 get_Tag()
FSharp.Compiler.Syntax.SynTypeDefnRepr: System.String ToString()
FSharp.Compiler.Syntax.SynTypeDefnSig
FSharp.Compiler.Syntax.SynTypeDefnSig: FSharp.Compiler.Syntax.SynComponentInfo get_typeInfo()
FSharp.Compiler.Syntax.SynTypeDefnSig: FSharp.Compiler.Syntax.SynComponentInfo typeInfo
FSharp.Compiler.Syntax.SynTypeDefnSig: FSharp.Compiler.Syntax.SynTypeDefnSig NewTypeDefnSig(FSharp.Compiler.Syntax.SynComponentInfo, FSharp.Compiler.Syntax.SynTypeDefnSigRepr, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynMemberSig], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynTypeDefnSig: FSharp.Compiler.Syntax.SynTypeDefnSigRepr get_typeRepr()
FSharp.Compiler.Syntax.SynTypeDefnSig: FSharp.Compiler.Syntax.SynTypeDefnSigRepr typeRepr
FSharp.Compiler.Syntax.SynTypeDefnSig: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynTypeDefnSig: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynTypeDefnSig: Int32 Tag
FSharp.Compiler.Syntax.SynTypeDefnSig: Int32 get_Tag()
FSharp.Compiler.Syntax.SynTypeDefnSig: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynMemberSig] get_members()
FSharp.Compiler.Syntax.SynTypeDefnSig: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynMemberSig] members
FSharp.Compiler.Syntax.SynTypeDefnSig: System.String ToString()
FSharp.Compiler.Syntax.SynTypeDefnSigRepr
FSharp.Compiler.Syntax.SynTypeDefnSigRepr+Exception: FSharp.Compiler.Syntax.SynExceptionDefnRepr get_repr()
FSharp.Compiler.Syntax.SynTypeDefnSigRepr+Exception: FSharp.Compiler.Syntax.SynExceptionDefnRepr repr
FSharp.Compiler.Syntax.SynTypeDefnSigRepr+ObjectModel: FSharp.Compiler.Syntax.SynTypeDefnKind get_kind()
FSharp.Compiler.Syntax.SynTypeDefnSigRepr+ObjectModel: FSharp.Compiler.Syntax.SynTypeDefnKind kind
FSharp.Compiler.Syntax.SynTypeDefnSigRepr+ObjectModel: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynTypeDefnSigRepr+ObjectModel: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynTypeDefnSigRepr+ObjectModel: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynMemberSig] get_memberSigs()
FSharp.Compiler.Syntax.SynTypeDefnSigRepr+ObjectModel: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynMemberSig] memberSigs
FSharp.Compiler.Syntax.SynTypeDefnSigRepr+Simple: FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr get_repr()
FSharp.Compiler.Syntax.SynTypeDefnSigRepr+Simple: FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr repr
FSharp.Compiler.Syntax.SynTypeDefnSigRepr+Simple: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynTypeDefnSigRepr+Simple: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynTypeDefnSigRepr+Tags: Int32 Exception
FSharp.Compiler.Syntax.SynTypeDefnSigRepr+Tags: Int32 ObjectModel
FSharp.Compiler.Syntax.SynTypeDefnSigRepr+Tags: Int32 Simple
FSharp.Compiler.Syntax.SynTypeDefnSigRepr: Boolean IsException
FSharp.Compiler.Syntax.SynTypeDefnSigRepr: Boolean IsObjectModel
FSharp.Compiler.Syntax.SynTypeDefnSigRepr: Boolean IsSimple
FSharp.Compiler.Syntax.SynTypeDefnSigRepr: Boolean get_IsException()
FSharp.Compiler.Syntax.SynTypeDefnSigRepr: Boolean get_IsObjectModel()
FSharp.Compiler.Syntax.SynTypeDefnSigRepr: Boolean get_IsSimple()
FSharp.Compiler.Syntax.SynTypeDefnSigRepr: FSharp.Compiler.Syntax.SynTypeDefnSigRepr NewException(FSharp.Compiler.Syntax.SynExceptionDefnRepr)
FSharp.Compiler.Syntax.SynTypeDefnSigRepr: FSharp.Compiler.Syntax.SynTypeDefnSigRepr NewObjectModel(FSharp.Compiler.Syntax.SynTypeDefnKind, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynMemberSig], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynTypeDefnSigRepr: FSharp.Compiler.Syntax.SynTypeDefnSigRepr NewSimple(FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynTypeDefnSigRepr: FSharp.Compiler.Syntax.SynTypeDefnSigRepr+Exception
FSharp.Compiler.Syntax.SynTypeDefnSigRepr: FSharp.Compiler.Syntax.SynTypeDefnSigRepr+ObjectModel
FSharp.Compiler.Syntax.SynTypeDefnSigRepr: FSharp.Compiler.Syntax.SynTypeDefnSigRepr+Simple
FSharp.Compiler.Syntax.SynTypeDefnSigRepr: FSharp.Compiler.Syntax.SynTypeDefnSigRepr+Tags
FSharp.Compiler.Syntax.SynTypeDefnSigRepr: FSharp.Compiler.Text.Range Range
FSharp.Compiler.Syntax.SynTypeDefnSigRepr: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.Syntax.SynTypeDefnSigRepr: Int32 Tag
FSharp.Compiler.Syntax.SynTypeDefnSigRepr: Int32 get_Tag()
FSharp.Compiler.Syntax.SynTypeDefnSigRepr: System.String ToString()
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+Enum: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+Enum: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+Enum: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynEnumCase] cases
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+Enum: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynEnumCase] get_cases()
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+Exception: FSharp.Compiler.Syntax.SynExceptionDefnRepr exnRepr
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+Exception: FSharp.Compiler.Syntax.SynExceptionDefnRepr get_exnRepr()
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+General: Boolean get_isConcrete()
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+General: Boolean get_isIncrClass()
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+General: Boolean isConcrete
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+General: Boolean isIncrClass
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+General: FSharp.Compiler.Syntax.SynTypeDefnKind get_kind()
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+General: FSharp.Compiler.Syntax.SynTypeDefnKind kind
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+General: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+General: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+General: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynField] fields
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+General: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynField] get_fields()
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+General: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[FSharp.Compiler.Syntax.SynValSig,FSharp.Compiler.Syntax.MemberFlags]] get_slotsigs()
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+General: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[FSharp.Compiler.Syntax.SynValSig,FSharp.Compiler.Syntax.MemberFlags]] slotsigs
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+General: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`3[FSharp.Compiler.Syntax.SynType,FSharp.Compiler.Text.Range,Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.Ident]]] get_inherits()
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+General: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`3[FSharp.Compiler.Syntax.SynType,FSharp.Compiler.Text.Range,Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.Ident]]] inherits
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+General: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynSimplePats] get_implicitCtorSynPats()
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+General: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynSimplePats] implicitCtorSynPats
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+LibraryOnlyILAssembly: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+LibraryOnlyILAssembly: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+LibraryOnlyILAssembly: System.Object get_ilType()
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+LibraryOnlyILAssembly: System.Object ilType
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+None: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+None: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+Record: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+Record: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+Record: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynField] get_recordFields()
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+Record: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynField] recordFields
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+Record: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess] accessibility
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+Record: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess] get_accessibility()
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+Tags: Int32 Enum
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+Tags: Int32 Exception
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+Tags: Int32 General
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+Tags: Int32 LibraryOnlyILAssembly
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+Tags: Int32 None
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+Tags: Int32 Record
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+Tags: Int32 TypeAbbrev
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+Tags: Int32 Union
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+TypeAbbrev: FSharp.Compiler.Syntax.ParserDetail detail
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+TypeAbbrev: FSharp.Compiler.Syntax.ParserDetail get_detail()
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+TypeAbbrev: FSharp.Compiler.Syntax.SynType get_rhsType()
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+TypeAbbrev: FSharp.Compiler.Syntax.SynType rhsType
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+TypeAbbrev: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+TypeAbbrev: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+Union: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+Union: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+Union: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynUnionCase] get_unionCases()
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+Union: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynUnionCase] unionCases
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+Union: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess] accessibility
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+Union: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess] get_accessibility()
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr: Boolean IsEnum
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr: Boolean IsException
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr: Boolean IsGeneral
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr: Boolean IsLibraryOnlyILAssembly
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr: Boolean IsNone
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr: Boolean IsRecord
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr: Boolean IsTypeAbbrev
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr: Boolean IsUnion
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr: Boolean get_IsEnum()
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr: Boolean get_IsException()
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr: Boolean get_IsGeneral()
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr: Boolean get_IsLibraryOnlyILAssembly()
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr: Boolean get_IsNone()
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr: Boolean get_IsRecord()
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr: Boolean get_IsTypeAbbrev()
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr: Boolean get_IsUnion()
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr: FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr NewEnum(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynEnumCase], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr: FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr NewException(FSharp.Compiler.Syntax.SynExceptionDefnRepr)
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr: FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr NewGeneral(FSharp.Compiler.Syntax.SynTypeDefnKind, Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`3[FSharp.Compiler.Syntax.SynType,FSharp.Compiler.Text.Range,Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.Ident]]], Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[FSharp.Compiler.Syntax.SynValSig,FSharp.Compiler.Syntax.MemberFlags]], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynField], Boolean, Boolean, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynSimplePats], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr: FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr NewLibraryOnlyILAssembly(System.Object, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr: FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr NewNone(FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr: FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr NewRecord(Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynField], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr: FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr NewTypeAbbrev(FSharp.Compiler.Syntax.ParserDetail, FSharp.Compiler.Syntax.SynType, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr: FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr NewUnion(Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynUnionCase], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr: FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+Enum
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr: FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+Exception
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr: FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+General
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr: FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+LibraryOnlyILAssembly
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr: FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+None
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr: FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+Record
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr: FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+Tags
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr: FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+TypeAbbrev
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr: FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr+Union
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr: FSharp.Compiler.Text.Range Range
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr: Int32 Tag
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr: Int32 get_Tag()
FSharp.Compiler.Syntax.SynTypeDefnSimpleRepr: System.String ToString()
FSharp.Compiler.Syntax.SynUnionCase
FSharp.Compiler.Syntax.SynUnionCase: FSharp.Compiler.Syntax.Ident get_ident()
FSharp.Compiler.Syntax.SynUnionCase: FSharp.Compiler.Syntax.Ident ident
FSharp.Compiler.Syntax.SynUnionCase: FSharp.Compiler.Syntax.PreXmlDoc get_xmlDoc()
FSharp.Compiler.Syntax.SynUnionCase: FSharp.Compiler.Syntax.PreXmlDoc xmlDoc
FSharp.Compiler.Syntax.SynUnionCase: FSharp.Compiler.Syntax.SynUnionCase NewUnionCase(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList], FSharp.Compiler.Syntax.Ident, FSharp.Compiler.Syntax.SynUnionCaseKind, FSharp.Compiler.Syntax.PreXmlDoc, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynUnionCase: FSharp.Compiler.Syntax.SynUnionCaseKind caseType
FSharp.Compiler.Syntax.SynUnionCase: FSharp.Compiler.Syntax.SynUnionCaseKind get_caseType()
FSharp.Compiler.Syntax.SynUnionCase: FSharp.Compiler.Text.Range Range
FSharp.Compiler.Syntax.SynUnionCase: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.Syntax.SynUnionCase: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynUnionCase: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynUnionCase: Int32 Tag
FSharp.Compiler.Syntax.SynUnionCase: Int32 get_Tag()
FSharp.Compiler.Syntax.SynUnionCase: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList] attributes
FSharp.Compiler.Syntax.SynUnionCase: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList] get_attributes()
FSharp.Compiler.Syntax.SynUnionCase: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess] accessibility
FSharp.Compiler.Syntax.SynUnionCase: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess] get_accessibility()
FSharp.Compiler.Syntax.SynUnionCase: System.String ToString()
FSharp.Compiler.Syntax.SynUnionCaseKind
FSharp.Compiler.Syntax.SynUnionCaseKind+Tags: Int32 Fields
FSharp.Compiler.Syntax.SynUnionCaseKind+Tags: Int32 FullType
FSharp.Compiler.Syntax.SynUnionCaseKind+Fields: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynField] cases
FSharp.Compiler.Syntax.SynUnionCaseKind+Fields: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynField] get_cases()
FSharp.Compiler.Syntax.SynUnionCaseKind+FullType: FSharp.Compiler.Syntax.SynType fullType
FSharp.Compiler.Syntax.SynUnionCaseKind+FullType: FSharp.Compiler.Syntax.SynType get_fullType()
FSharp.Compiler.Syntax.SynUnionCaseKind+FullType: FSharp.Compiler.Syntax.SynValInfo fullTypeInfo
FSharp.Compiler.Syntax.SynUnionCaseKind+FullType: FSharp.Compiler.Syntax.SynValInfo get_fullTypeInfo()
FSharp.Compiler.Syntax.SynUnionCaseKind: Boolean IsUnionCaseFields
FSharp.Compiler.Syntax.SynUnionCaseKind: Boolean IsUnionCaseFullType
FSharp.Compiler.Syntax.SynUnionCaseKind: Boolean get_IsUnionCaseFields()
FSharp.Compiler.Syntax.SynUnionCaseKind: Boolean get_IsUnionCaseFullType()
FSharp.Compiler.Syntax.SynUnionCaseKind: FSharp.Compiler.Syntax.SynUnionCaseKind NewUnionCaseFields(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynField])
FSharp.Compiler.Syntax.SynUnionCaseKind: FSharp.Compiler.Syntax.SynUnionCaseKind NewUnionCaseFullType(FSharp.Compiler.Syntax.SynType, FSharp.Compiler.Syntax.SynValInfo)
FSharp.Compiler.Syntax.SynUnionCaseKind: FSharp.Compiler.Syntax.SynUnionCaseKind+Tags
FSharp.Compiler.Syntax.SynUnionCaseKind: FSharp.Compiler.Syntax.SynUnionCaseKind+Fields
FSharp.Compiler.Syntax.SynUnionCaseKind: FSharp.Compiler.Syntax.SynUnionCaseKind+FullType
FSharp.Compiler.Syntax.SynUnionCaseKind: Int32 Tag
FSharp.Compiler.Syntax.SynUnionCaseKind: Int32 get_Tag()
FSharp.Compiler.Syntax.SynUnionCaseKind: System.String ToString()
FSharp.Compiler.Syntax.SynValData
FSharp.Compiler.Syntax.SynValData: FSharp.Compiler.Syntax.SynValData NewSynValData(Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.MemberFlags], FSharp.Compiler.Syntax.SynValInfo, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.Ident])
FSharp.Compiler.Syntax.SynValData: FSharp.Compiler.Syntax.SynValInfo SynValInfo
FSharp.Compiler.Syntax.SynValData: FSharp.Compiler.Syntax.SynValInfo get_SynValInfo()
FSharp.Compiler.Syntax.SynValData: FSharp.Compiler.Syntax.SynValInfo get_valInfo()
FSharp.Compiler.Syntax.SynValData: FSharp.Compiler.Syntax.SynValInfo valInfo
FSharp.Compiler.Syntax.SynValData: Int32 Tag
FSharp.Compiler.Syntax.SynValData: Int32 get_Tag()
FSharp.Compiler.Syntax.SynValData: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.Ident] get_thisIdOpt()
FSharp.Compiler.Syntax.SynValData: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.Ident] thisIdOpt
FSharp.Compiler.Syntax.SynValData: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.MemberFlags] get_memberFlags()
FSharp.Compiler.Syntax.SynValData: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.MemberFlags] memberFlags
FSharp.Compiler.Syntax.SynValData: System.String ToString()
FSharp.Compiler.Syntax.SynValInfo
FSharp.Compiler.Syntax.SynValInfo: FSharp.Compiler.Syntax.SynArgInfo get_returnInfo()
FSharp.Compiler.Syntax.SynValInfo: FSharp.Compiler.Syntax.SynArgInfo returnInfo
FSharp.Compiler.Syntax.SynValInfo: FSharp.Compiler.Syntax.SynValInfo NewSynValInfo(Microsoft.FSharp.Collections.FSharpList`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynArgInfo]], FSharp.Compiler.Syntax.SynArgInfo)
FSharp.Compiler.Syntax.SynValInfo: Int32 Tag
FSharp.Compiler.Syntax.SynValInfo: Int32 get_Tag()
FSharp.Compiler.Syntax.SynValInfo: Microsoft.FSharp.Collections.FSharpList`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynArgInfo]] CurriedArgInfos
FSharp.Compiler.Syntax.SynValInfo: Microsoft.FSharp.Collections.FSharpList`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynArgInfo]] curriedArgInfos
FSharp.Compiler.Syntax.SynValInfo: Microsoft.FSharp.Collections.FSharpList`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynArgInfo]] get_CurriedArgInfos()
FSharp.Compiler.Syntax.SynValInfo: Microsoft.FSharp.Collections.FSharpList`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynArgInfo]] get_curriedArgInfos()
FSharp.Compiler.Syntax.SynValInfo: Microsoft.FSharp.Collections.FSharpList`1[System.String] ArgNames
FSharp.Compiler.Syntax.SynValInfo: Microsoft.FSharp.Collections.FSharpList`1[System.String] get_ArgNames()
FSharp.Compiler.Syntax.SynValInfo: System.String ToString()
FSharp.Compiler.Syntax.SynValSig
FSharp.Compiler.Syntax.SynValSig: Boolean get_isInline()
FSharp.Compiler.Syntax.SynValSig: Boolean get_isMutable()
FSharp.Compiler.Syntax.SynValSig: Boolean isInline
FSharp.Compiler.Syntax.SynValSig: Boolean isMutable
FSharp.Compiler.Syntax.SynValSig: FSharp.Compiler.Syntax.Ident get_ident()
FSharp.Compiler.Syntax.SynValSig: FSharp.Compiler.Syntax.Ident ident
FSharp.Compiler.Syntax.SynValSig: FSharp.Compiler.Syntax.PreXmlDoc get_xmlDoc()
FSharp.Compiler.Syntax.SynValSig: FSharp.Compiler.Syntax.PreXmlDoc xmlDoc
FSharp.Compiler.Syntax.SynValSig: FSharp.Compiler.Syntax.SynType SynType
FSharp.Compiler.Syntax.SynValSig: FSharp.Compiler.Syntax.SynType get_SynType()
FSharp.Compiler.Syntax.SynValSig: FSharp.Compiler.Syntax.SynType get_synType()
FSharp.Compiler.Syntax.SynValSig: FSharp.Compiler.Syntax.SynType synType
FSharp.Compiler.Syntax.SynValSig: FSharp.Compiler.Syntax.SynValInfo SynInfo
FSharp.Compiler.Syntax.SynValSig: FSharp.Compiler.Syntax.SynValInfo arity
FSharp.Compiler.Syntax.SynValSig: FSharp.Compiler.Syntax.SynValInfo get_SynInfo()
FSharp.Compiler.Syntax.SynValSig: FSharp.Compiler.Syntax.SynValInfo get_arity()
FSharp.Compiler.Syntax.SynValSig: FSharp.Compiler.Syntax.SynValSig NewValSpfn(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList], FSharp.Compiler.Syntax.Ident, FSharp.Compiler.Syntax.SynValTyparDecls, FSharp.Compiler.Syntax.SynType, FSharp.Compiler.Syntax.SynValInfo, Boolean, Boolean, FSharp.Compiler.Syntax.PreXmlDoc, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess], Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynExpr], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SynValSig: FSharp.Compiler.Syntax.SynValTyparDecls explicitValDecls
FSharp.Compiler.Syntax.SynValSig: FSharp.Compiler.Syntax.SynValTyparDecls get_explicitValDecls()
FSharp.Compiler.Syntax.SynValSig: FSharp.Compiler.Text.Range RangeOfId
FSharp.Compiler.Syntax.SynValSig: FSharp.Compiler.Text.Range get_RangeOfId()
FSharp.Compiler.Syntax.SynValSig: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.Syntax.SynValSig: FSharp.Compiler.Text.Range range
FSharp.Compiler.Syntax.SynValSig: Int32 Tag
FSharp.Compiler.Syntax.SynValSig: Int32 get_Tag()
FSharp.Compiler.Syntax.SynValSig: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList] attributes
FSharp.Compiler.Syntax.SynValSig: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynAttributeList] get_attributes()
FSharp.Compiler.Syntax.SynValSig: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess] accessibility
FSharp.Compiler.Syntax.SynValSig: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynAccess] get_accessibility()
FSharp.Compiler.Syntax.SynValSig: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynExpr] get_synExpr()
FSharp.Compiler.Syntax.SynValSig: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynExpr] synExpr
FSharp.Compiler.Syntax.SynValSig: System.String ToString()
FSharp.Compiler.Syntax.SynValTyparDecls
FSharp.Compiler.Syntax.SynValTyparDecls: Boolean canInfer
FSharp.Compiler.Syntax.SynValTyparDecls: Boolean get_canInfer()
FSharp.Compiler.Syntax.SynValTyparDecls: FSharp.Compiler.Syntax.SynValTyparDecls NewSynValTyparDecls(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynTyparDecl], Boolean, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynTypeConstraint])
FSharp.Compiler.Syntax.SynValTyparDecls: Int32 Tag
FSharp.Compiler.Syntax.SynValTyparDecls: Int32 get_Tag()
FSharp.Compiler.Syntax.SynValTyparDecls: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynTyparDecl] get_typars()
FSharp.Compiler.Syntax.SynValTyparDecls: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynTyparDecl] typars
FSharp.Compiler.Syntax.SynValTyparDecls: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynTypeConstraint] constraints
FSharp.Compiler.Syntax.SynValTyparDecls: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynTypeConstraint] get_constraints()
FSharp.Compiler.Syntax.SynValTyparDecls: System.String ToString()
FSharp.Compiler.Syntax.SyntaxNode
FSharp.Compiler.Syntax.SyntaxNode+SynBinding: FSharp.Compiler.Syntax.SynBinding Item
FSharp.Compiler.Syntax.SyntaxNode+SynBinding: FSharp.Compiler.Syntax.SynBinding get_Item()
FSharp.Compiler.Syntax.SyntaxNode+SynExpr: FSharp.Compiler.Syntax.SynExpr Item
FSharp.Compiler.Syntax.SyntaxNode+SynExpr: FSharp.Compiler.Syntax.SynExpr get_Item()
FSharp.Compiler.Syntax.SyntaxNode+SynMatchClause: FSharp.Compiler.Syntax.SynMatchClause Item
FSharp.Compiler.Syntax.SyntaxNode+SynMatchClause: FSharp.Compiler.Syntax.SynMatchClause get_Item()
FSharp.Compiler.Syntax.SyntaxNode+SynMemberDefn: FSharp.Compiler.Syntax.SynMemberDefn Item
FSharp.Compiler.Syntax.SyntaxNode+SynMemberDefn: FSharp.Compiler.Syntax.SynMemberDefn get_Item()
FSharp.Compiler.Syntax.SyntaxNode+SynModule: FSharp.Compiler.Syntax.SynModuleDecl Item
FSharp.Compiler.Syntax.SyntaxNode+SynModule: FSharp.Compiler.Syntax.SynModuleDecl get_Item()
FSharp.Compiler.Syntax.SyntaxNode+SynModuleOrNamespace: FSharp.Compiler.Syntax.SynModuleOrNamespace Item
FSharp.Compiler.Syntax.SyntaxNode+SynModuleOrNamespace: FSharp.Compiler.Syntax.SynModuleOrNamespace get_Item()
FSharp.Compiler.Syntax.SyntaxNode+SynPat: FSharp.Compiler.Syntax.SynPat Item
FSharp.Compiler.Syntax.SyntaxNode+SynPat: FSharp.Compiler.Syntax.SynPat get_Item()
FSharp.Compiler.Syntax.SyntaxNode+SynType: FSharp.Compiler.Syntax.SynType Item
FSharp.Compiler.Syntax.SyntaxNode+SynType: FSharp.Compiler.Syntax.SynType get_Item()
FSharp.Compiler.Syntax.SyntaxNode+SynTypeDefn: FSharp.Compiler.Syntax.SynTypeDefn Item
FSharp.Compiler.Syntax.SyntaxNode+SynTypeDefn: FSharp.Compiler.Syntax.SynTypeDefn get_Item()
FSharp.Compiler.Syntax.SyntaxNode+Tags: Int32 SynBinding
FSharp.Compiler.Syntax.SyntaxNode+Tags: Int32 SynExpr
FSharp.Compiler.Syntax.SyntaxNode+Tags: Int32 SynMatchClause
FSharp.Compiler.Syntax.SyntaxNode+Tags: Int32 SynMemberDefn
FSharp.Compiler.Syntax.SyntaxNode+Tags: Int32 SynModule
FSharp.Compiler.Syntax.SyntaxNode+Tags: Int32 SynModuleOrNamespace
FSharp.Compiler.Syntax.SyntaxNode+Tags: Int32 SynPat
FSharp.Compiler.Syntax.SyntaxNode+Tags: Int32 SynType
FSharp.Compiler.Syntax.SyntaxNode+Tags: Int32 SynTypeDefn
FSharp.Compiler.Syntax.SyntaxNode: Boolean IsSynBinding
FSharp.Compiler.Syntax.SyntaxNode: Boolean IsSynExpr
FSharp.Compiler.Syntax.SyntaxNode: Boolean IsSynMatchClause
FSharp.Compiler.Syntax.SyntaxNode: Boolean IsSynMemberDefn
FSharp.Compiler.Syntax.SyntaxNode: Boolean IsSynModule
FSharp.Compiler.Syntax.SyntaxNode: Boolean IsSynModuleOrNamespace
FSharp.Compiler.Syntax.SyntaxNode: Boolean IsSynPat
FSharp.Compiler.Syntax.SyntaxNode: Boolean IsSynType
FSharp.Compiler.Syntax.SyntaxNode: Boolean IsSynTypeDefn
FSharp.Compiler.Syntax.SyntaxNode: Boolean get_IsSynBinding()
FSharp.Compiler.Syntax.SyntaxNode: Boolean get_IsSynExpr()
FSharp.Compiler.Syntax.SyntaxNode: Boolean get_IsSynMatchClause()
FSharp.Compiler.Syntax.SyntaxNode: Boolean get_IsSynMemberDefn()
FSharp.Compiler.Syntax.SyntaxNode: Boolean get_IsSynModule()
FSharp.Compiler.Syntax.SyntaxNode: Boolean get_IsSynModuleOrNamespace()
FSharp.Compiler.Syntax.SyntaxNode: Boolean get_IsSynPat()
FSharp.Compiler.Syntax.SyntaxNode: Boolean get_IsSynType()
FSharp.Compiler.Syntax.SyntaxNode: Boolean get_IsSynTypeDefn()
FSharp.Compiler.Syntax.SyntaxNode: FSharp.Compiler.Syntax.SyntaxNode NewSynBinding(FSharp.Compiler.Syntax.SynBinding)
FSharp.Compiler.Syntax.SyntaxNode: FSharp.Compiler.Syntax.SyntaxNode NewSynExpr(FSharp.Compiler.Syntax.SynExpr)
FSharp.Compiler.Syntax.SyntaxNode: FSharp.Compiler.Syntax.SyntaxNode NewSynMatchClause(FSharp.Compiler.Syntax.SynMatchClause)
FSharp.Compiler.Syntax.SyntaxNode: FSharp.Compiler.Syntax.SyntaxNode NewSynMemberDefn(FSharp.Compiler.Syntax.SynMemberDefn)
FSharp.Compiler.Syntax.SyntaxNode: FSharp.Compiler.Syntax.SyntaxNode NewSynModule(FSharp.Compiler.Syntax.SynModuleDecl)
FSharp.Compiler.Syntax.SyntaxNode: FSharp.Compiler.Syntax.SyntaxNode NewSynModuleOrNamespace(FSharp.Compiler.Syntax.SynModuleOrNamespace)
FSharp.Compiler.Syntax.SyntaxNode: FSharp.Compiler.Syntax.SyntaxNode NewSynPat(FSharp.Compiler.Syntax.SynPat)
FSharp.Compiler.Syntax.SyntaxNode: FSharp.Compiler.Syntax.SyntaxNode NewSynType(FSharp.Compiler.Syntax.SynType)
FSharp.Compiler.Syntax.SyntaxNode: FSharp.Compiler.Syntax.SyntaxNode NewSynTypeDefn(FSharp.Compiler.Syntax.SynTypeDefn)
FSharp.Compiler.Syntax.SyntaxNode: FSharp.Compiler.Syntax.SyntaxNode+SynBinding
FSharp.Compiler.Syntax.SyntaxNode: FSharp.Compiler.Syntax.SyntaxNode+SynExpr
FSharp.Compiler.Syntax.SyntaxNode: FSharp.Compiler.Syntax.SyntaxNode+SynMatchClause
FSharp.Compiler.Syntax.SyntaxNode: FSharp.Compiler.Syntax.SyntaxNode+SynMemberDefn
FSharp.Compiler.Syntax.SyntaxNode: FSharp.Compiler.Syntax.SyntaxNode+SynModule
FSharp.Compiler.Syntax.SyntaxNode: FSharp.Compiler.Syntax.SyntaxNode+SynModuleOrNamespace
FSharp.Compiler.Syntax.SyntaxNode: FSharp.Compiler.Syntax.SyntaxNode+SynPat
FSharp.Compiler.Syntax.SyntaxNode: FSharp.Compiler.Syntax.SyntaxNode+SynType
FSharp.Compiler.Syntax.SyntaxNode: FSharp.Compiler.Syntax.SyntaxNode+SynTypeDefn
FSharp.Compiler.Syntax.SyntaxNode: FSharp.Compiler.Syntax.SyntaxNode+Tags
FSharp.Compiler.Syntax.SyntaxNode: Int32 Tag
FSharp.Compiler.Syntax.SyntaxNode: Int32 get_Tag()
FSharp.Compiler.Syntax.SyntaxNode: System.String ToString()
FSharp.Compiler.Syntax.SyntaxTraversal
FSharp.Compiler.Syntax.SyntaxTraversal: Microsoft.FSharp.Core.FSharpOption`1[T] Traverse[T](FSharp.Compiler.Text.Position, FSharp.Compiler.Syntax.ParsedInput, FSharp.Compiler.Syntax.SyntaxVisitorBase`1[T])
FSharp.Compiler.Syntax.SyntaxVisitorBase`1[T]
FSharp.Compiler.Syntax.SyntaxVisitorBase`1[T]: Microsoft.FSharp.Core.FSharpOption`1[T] VisitBinding(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SyntaxNode], Microsoft.FSharp.Core.FSharpFunc`2[FSharp.Compiler.Syntax.SynBinding,Microsoft.FSharp.Core.FSharpOption`1[T]], FSharp.Compiler.Syntax.SynBinding)
FSharp.Compiler.Syntax.SyntaxVisitorBase`1[T]: Microsoft.FSharp.Core.FSharpOption`1[T] VisitComponentInfo(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SyntaxNode], FSharp.Compiler.Syntax.SynComponentInfo)
FSharp.Compiler.Syntax.SyntaxVisitorBase`1[T]: Microsoft.FSharp.Core.FSharpOption`1[T] VisitExpr(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SyntaxNode], Microsoft.FSharp.Core.FSharpFunc`2[FSharp.Compiler.Syntax.SynExpr,Microsoft.FSharp.Core.FSharpOption`1[T]], Microsoft.FSharp.Core.FSharpFunc`2[FSharp.Compiler.Syntax.SynExpr,Microsoft.FSharp.Core.FSharpOption`1[T]], FSharp.Compiler.Syntax.SynExpr)
FSharp.Compiler.Syntax.SyntaxVisitorBase`1[T]: Microsoft.FSharp.Core.FSharpOption`1[T] VisitHashDirective(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SyntaxNode], FSharp.Compiler.Syntax.ParsedHashDirective, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SyntaxVisitorBase`1[T]: Microsoft.FSharp.Core.FSharpOption`1[T] VisitImplicitInherit(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SyntaxNode], Microsoft.FSharp.Core.FSharpFunc`2[FSharp.Compiler.Syntax.SynExpr,Microsoft.FSharp.Core.FSharpOption`1[T]], FSharp.Compiler.Syntax.SynType, FSharp.Compiler.Syntax.SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SyntaxVisitorBase`1[T]: Microsoft.FSharp.Core.FSharpOption`1[T] VisitInheritSynMemberDefn(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SyntaxNode], FSharp.Compiler.Syntax.SynComponentInfo, FSharp.Compiler.Syntax.SynTypeDefnKind, FSharp.Compiler.Syntax.SynType, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynMemberDefn], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SyntaxVisitorBase`1[T]: Microsoft.FSharp.Core.FSharpOption`1[T] VisitInterfaceSynMemberDefnType(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SyntaxNode], FSharp.Compiler.Syntax.SynType)
FSharp.Compiler.Syntax.SyntaxVisitorBase`1[T]: Microsoft.FSharp.Core.FSharpOption`1[T] VisitLetOrUse(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SyntaxNode], Boolean, Microsoft.FSharp.Core.FSharpFunc`2[FSharp.Compiler.Syntax.SynBinding,Microsoft.FSharp.Core.FSharpOption`1[T]], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynBinding], FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SyntaxVisitorBase`1[T]: Microsoft.FSharp.Core.FSharpOption`1[T] VisitMatchClause(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SyntaxNode], Microsoft.FSharp.Core.FSharpFunc`2[FSharp.Compiler.Syntax.SynMatchClause,Microsoft.FSharp.Core.FSharpOption`1[T]], FSharp.Compiler.Syntax.SynMatchClause)
FSharp.Compiler.Syntax.SyntaxVisitorBase`1[T]: Microsoft.FSharp.Core.FSharpOption`1[T] VisitModuleDecl(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SyntaxNode], Microsoft.FSharp.Core.FSharpFunc`2[FSharp.Compiler.Syntax.SynModuleDecl,Microsoft.FSharp.Core.FSharpOption`1[T]], FSharp.Compiler.Syntax.SynModuleDecl)
FSharp.Compiler.Syntax.SyntaxVisitorBase`1[T]: Microsoft.FSharp.Core.FSharpOption`1[T] VisitModuleOrNamespace(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SyntaxNode], FSharp.Compiler.Syntax.SynModuleOrNamespace)
FSharp.Compiler.Syntax.SyntaxVisitorBase`1[T]: Microsoft.FSharp.Core.FSharpOption`1[T] VisitPat(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SyntaxNode], Microsoft.FSharp.Core.FSharpFunc`2[FSharp.Compiler.Syntax.SynPat,Microsoft.FSharp.Core.FSharpOption`1[T]], FSharp.Compiler.Syntax.SynPat)
FSharp.Compiler.Syntax.SyntaxVisitorBase`1[T]: Microsoft.FSharp.Core.FSharpOption`1[T] VisitRecordField(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SyntaxNode], Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.SynExpr], Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Syntax.LongIdentWithDots])
FSharp.Compiler.Syntax.SyntaxVisitorBase`1[T]: Microsoft.FSharp.Core.FSharpOption`1[T] VisitSimplePats(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SyntaxNode], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SynSimplePat])
FSharp.Compiler.Syntax.SyntaxVisitorBase`1[T]: Microsoft.FSharp.Core.FSharpOption`1[T] VisitType(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SyntaxNode], Microsoft.FSharp.Core.FSharpFunc`2[FSharp.Compiler.Syntax.SynType,Microsoft.FSharp.Core.FSharpOption`1[T]], FSharp.Compiler.Syntax.SynType)
FSharp.Compiler.Syntax.SyntaxVisitorBase`1[T]: Microsoft.FSharp.Core.FSharpOption`1[T] VisitTypeAbbrev(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Syntax.SyntaxNode], FSharp.Compiler.Syntax.SynType, FSharp.Compiler.Text.Range)
FSharp.Compiler.Syntax.SyntaxVisitorBase`1[T]: Void .ctor()
FSharp.Compiler.Syntax.TyparStaticReq
FSharp.Compiler.Syntax.TyparStaticReq+Tags: Int32 TyparStaticReq.HeadType
FSharp.Compiler.Syntax.TyparStaticReq+Tags: Int32 TyparStaticReq.None
FSharp.Compiler.Syntax.TyparStaticReq: Boolean Equals(FSharp.Compiler.Syntax.TyparStaticReq)
FSharp.Compiler.Syntax.TyparStaticReq: Boolean Equals(System.Object)
FSharp.Compiler.Syntax.TyparStaticReq: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.Syntax.TyparStaticReq: Boolean IsHeadTypeStaticReq
FSharp.Compiler.Syntax.TyparStaticReq: Boolean IsNoStaticReq
FSharp.Compiler.Syntax.TyparStaticReq: Boolean get_IsHeadTypeStaticReq()
FSharp.Compiler.Syntax.TyparStaticReq: Boolean get_IsNoStaticReq()
FSharp.Compiler.Syntax.TyparStaticReq: FSharp.Compiler.Syntax.TyparStaticReq TyparStaticReq.HeadType
FSharp.Compiler.Syntax.TyparStaticReq: FSharp.Compiler.Syntax.TyparStaticReq TyparStaticReq.None
FSharp.Compiler.Syntax.TyparStaticReq: FSharp.Compiler.Syntax.TyparStaticReq get_HeadTypeStaticReq()
FSharp.Compiler.Syntax.TyparStaticReq: FSharp.Compiler.Syntax.TyparStaticReq get_NoStaticReq()
FSharp.Compiler.Syntax.TyparStaticReq: FSharp.Compiler.Syntax.TyparStaticReq+Tags
FSharp.Compiler.Syntax.TyparStaticReq: Int32 CompareTo(FSharp.Compiler.Syntax.TyparStaticReq)
FSharp.Compiler.Syntax.TyparStaticReq: Int32 CompareTo(System.Object)
FSharp.Compiler.Syntax.TyparStaticReq: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.Syntax.TyparStaticReq: Int32 GetHashCode()
FSharp.Compiler.Syntax.TyparStaticReq: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.Syntax.TyparStaticReq: Int32 Tag
FSharp.Compiler.Syntax.TyparStaticReq: Int32 get_Tag()
FSharp.Compiler.Syntax.TyparStaticReq: System.String ToString()
FSharp.Compiler.Syntax.XmlDoc
FSharp.Compiler.Syntax.XmlDoc: Boolean IsEmpty
FSharp.Compiler.Syntax.XmlDoc: Boolean NonEmpty
FSharp.Compiler.Syntax.XmlDoc: Boolean get_IsEmpty()
FSharp.Compiler.Syntax.XmlDoc: Boolean get_NonEmpty()
FSharp.Compiler.Syntax.XmlDoc: FSharp.Compiler.Syntax.XmlDoc Empty
FSharp.Compiler.Syntax.XmlDoc: FSharp.Compiler.Syntax.XmlDoc Merge(FSharp.Compiler.Syntax.XmlDoc, FSharp.Compiler.Syntax.XmlDoc)
FSharp.Compiler.Syntax.XmlDoc: FSharp.Compiler.Syntax.XmlDoc get_Empty()
FSharp.Compiler.Syntax.XmlDoc: FSharp.Compiler.Text.Range Range
FSharp.Compiler.Syntax.XmlDoc: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.Syntax.XmlDoc: System.String GetXmlText()
FSharp.Compiler.Syntax.XmlDoc: System.String[] GetElaboratedXmlLines()
FSharp.Compiler.Syntax.XmlDoc: System.String[] UnprocessedLines
FSharp.Compiler.Syntax.XmlDoc: System.String[] get_UnprocessedLines()
FSharp.Compiler.Syntax.XmlDoc: Void .ctor(System.String[], FSharp.Compiler.Text.Range)
FSharp.Compiler.Text.ISourceText
FSharp.Compiler.Text.ISourceText: Boolean ContentEquals(FSharp.Compiler.Text.ISourceText)
FSharp.Compiler.Text.ISourceText: Boolean SubTextEquals(System.String, Int32)
FSharp.Compiler.Text.ISourceText: Char Item [Int32]
FSharp.Compiler.Text.ISourceText: Char get_Item(Int32)
FSharp.Compiler.Text.ISourceText: Int32 GetLineCount()
FSharp.Compiler.Text.ISourceText: Int32 Length
FSharp.Compiler.Text.ISourceText: Int32 get_Length()
FSharp.Compiler.Text.ISourceText: System.String GetLineString(Int32)
FSharp.Compiler.Text.ISourceText: System.String GetSubTextString(Int32, Int32)
FSharp.Compiler.Text.ISourceText: System.Tuple`2[System.Int32,System.Int32] GetLastCharacterPosition()
FSharp.Compiler.Text.ISourceText: Void CopyTo(Int32, Char[], Int32, Int32)
FSharp.Compiler.Text.Line
FSharp.Compiler.Text.Line: Int32 fromZ(Int32)
FSharp.Compiler.Text.Line: Int32 toZ(Int32)
FSharp.Compiler.Text.Position
FSharp.Compiler.Text.Position: Boolean Equals(System.Object)
FSharp.Compiler.Text.Position: FSharp.Compiler.Text.Position Decode(Int64)
FSharp.Compiler.Text.Position: Int32 Column
FSharp.Compiler.Text.Position: Int32 EncodingSize
FSharp.Compiler.Text.Position: Int32 GetHashCode()
FSharp.Compiler.Text.Position: Int32 Line
FSharp.Compiler.Text.Position: Int32 get_Column()
FSharp.Compiler.Text.Position: Int32 get_EncodingSize()
FSharp.Compiler.Text.Position: Int32 get_Line()
FSharp.Compiler.Text.Position: Int64 Encoding
FSharp.Compiler.Text.Position: Int64 get_Encoding()
FSharp.Compiler.Text.Position: System.String ToString()
FSharp.Compiler.Text.PosModule
FSharp.Compiler.Text.PosModule: Boolean posEq(FSharp.Compiler.Text.Position, FSharp.Compiler.Text.Position)
FSharp.Compiler.Text.PosModule: Boolean posGeq(FSharp.Compiler.Text.Position, FSharp.Compiler.Text.Position)
FSharp.Compiler.Text.PosModule: Boolean posGt(FSharp.Compiler.Text.Position, FSharp.Compiler.Text.Position)
FSharp.Compiler.Text.PosModule: Boolean posLt(FSharp.Compiler.Text.Position, FSharp.Compiler.Text.Position)
FSharp.Compiler.Text.PosModule: FSharp.Compiler.Text.Position fromZ(Int32, Int32)
FSharp.Compiler.Text.PosModule: FSharp.Compiler.Text.Position get_pos0()
FSharp.Compiler.Text.PosModule: FSharp.Compiler.Text.Position mkPos(Int32, Int32)
FSharp.Compiler.Text.PosModule: FSharp.Compiler.Text.Position pos0
FSharp.Compiler.Text.PosModule: System.String stringOfPos(FSharp.Compiler.Text.Position)
FSharp.Compiler.Text.PosModule: System.Tuple`2[System.Int32,System.Int32] toZ(FSharp.Compiler.Text.Position)
FSharp.Compiler.Text.PosModule: Void outputPos(System.IO.TextWriter, FSharp.Compiler.Text.Position)
FSharp.Compiler.Text.Range
FSharp.Compiler.Text.Range: Boolean Equals(System.Object)
FSharp.Compiler.Text.Range: Boolean IsSynthetic
FSharp.Compiler.Text.Range: Boolean get_IsSynthetic()
FSharp.Compiler.Text.Range: FSharp.Compiler.Text.Position End
FSharp.Compiler.Text.Range: FSharp.Compiler.Text.Position Start
FSharp.Compiler.Text.Range: FSharp.Compiler.Text.Position get_End()
FSharp.Compiler.Text.Range: FSharp.Compiler.Text.Position get_Start()
FSharp.Compiler.Text.Range: FSharp.Compiler.Text.Range EndRange
FSharp.Compiler.Text.Range: FSharp.Compiler.Text.Range MakeSynthetic()
FSharp.Compiler.Text.Range: FSharp.Compiler.Text.Range StartRange
FSharp.Compiler.Text.Range: FSharp.Compiler.Text.Range Zero
FSharp.Compiler.Text.Range: FSharp.Compiler.Text.Range get_EndRange()
FSharp.Compiler.Text.Range: FSharp.Compiler.Text.Range get_StartRange()
FSharp.Compiler.Text.Range: FSharp.Compiler.Text.Range get_Zero()
FSharp.Compiler.Text.Range: Int32 EndColumn
FSharp.Compiler.Text.Range: Int32 EndLine
FSharp.Compiler.Text.Range: Int32 FileIndex
FSharp.Compiler.Text.Range: Int32 GetHashCode()
FSharp.Compiler.Text.Range: Int32 StartColumn
FSharp.Compiler.Text.Range: Int32 StartLine
FSharp.Compiler.Text.Range: Int32 get_EndColumn()
FSharp.Compiler.Text.Range: Int32 get_EndLine()
FSharp.Compiler.Text.Range: Int32 get_FileIndex()
FSharp.Compiler.Text.Range: Int32 get_StartColumn()
FSharp.Compiler.Text.Range: Int32 get_StartLine()
FSharp.Compiler.Text.Range: System.String FileName
FSharp.Compiler.Text.Range: System.String ToShortString()
FSharp.Compiler.Text.Range: System.String ToString()
FSharp.Compiler.Text.Range: System.String get_FileName()
FSharp.Compiler.Text.RangeModule
FSharp.Compiler.Text.RangeModule: Boolean equals(FSharp.Compiler.Text.Range, FSharp.Compiler.Text.Range)
FSharp.Compiler.Text.RangeModule: Boolean rangeBeforePos(FSharp.Compiler.Text.Range, FSharp.Compiler.Text.Position)
FSharp.Compiler.Text.RangeModule: Boolean rangeContainsPos(FSharp.Compiler.Text.Range, FSharp.Compiler.Text.Position)
FSharp.Compiler.Text.RangeModule: Boolean rangeContainsRange(FSharp.Compiler.Text.Range, FSharp.Compiler.Text.Range)
FSharp.Compiler.Text.RangeModule: FSharp.Compiler.Text.Range get_range0()
FSharp.Compiler.Text.RangeModule: FSharp.Compiler.Text.Range get_rangeCmdArgs()
FSharp.Compiler.Text.RangeModule: FSharp.Compiler.Text.Range get_rangeStartup()
FSharp.Compiler.Text.RangeModule: FSharp.Compiler.Text.Range mkFileIndexRange(Int32, FSharp.Compiler.Text.Position, FSharp.Compiler.Text.Position)
FSharp.Compiler.Text.RangeModule: FSharp.Compiler.Text.Range mkFirstLineOfFile(System.String)
FSharp.Compiler.Text.RangeModule: FSharp.Compiler.Text.Range mkRange(System.String, FSharp.Compiler.Text.Position, FSharp.Compiler.Text.Position)
FSharp.Compiler.Text.RangeModule: FSharp.Compiler.Text.Range range0
FSharp.Compiler.Text.RangeModule: FSharp.Compiler.Text.Range rangeCmdArgs
FSharp.Compiler.Text.RangeModule: FSharp.Compiler.Text.Range rangeN(System.String, Int32)
FSharp.Compiler.Text.RangeModule: FSharp.Compiler.Text.Range rangeStartup
FSharp.Compiler.Text.RangeModule: FSharp.Compiler.Text.Range trimRangeToLine(FSharp.Compiler.Text.Range)
FSharp.Compiler.Text.RangeModule: FSharp.Compiler.Text.Range unionRanges(FSharp.Compiler.Text.Range, FSharp.Compiler.Text.Range)
FSharp.Compiler.Text.RangeModule: System.Collections.Generic.IComparer`1[FSharp.Compiler.Text.Position] get_posOrder()
FSharp.Compiler.Text.RangeModule: System.Collections.Generic.IComparer`1[FSharp.Compiler.Text.Position] posOrder
FSharp.Compiler.Text.RangeModule: System.Collections.Generic.IComparer`1[FSharp.Compiler.Text.Range] get_rangeOrder()
FSharp.Compiler.Text.RangeModule: System.Collections.Generic.IComparer`1[FSharp.Compiler.Text.Range] rangeOrder
FSharp.Compiler.Text.RangeModule: System.Collections.Generic.IEqualityComparer`1[FSharp.Compiler.Text.Range] comparer
FSharp.Compiler.Text.RangeModule: System.Collections.Generic.IEqualityComparer`1[FSharp.Compiler.Text.Range] get_comparer()
FSharp.Compiler.Text.RangeModule: System.String stringOfRange(FSharp.Compiler.Text.Range)
FSharp.Compiler.Text.RangeModule: System.Tuple`2[System.String,System.Tuple`2[System.Tuple`2[System.Int32,System.Int32],System.Tuple`2[System.Int32,System.Int32]]] toFileZ(FSharp.Compiler.Text.Range)
FSharp.Compiler.Text.RangeModule: System.Tuple`2[System.Tuple`2[System.Int32,System.Int32],System.Tuple`2[System.Int32,System.Int32]] toZ(FSharp.Compiler.Text.Range)
FSharp.Compiler.Text.RangeModule: Void outputRange(System.IO.TextWriter, FSharp.Compiler.Text.Range)
FSharp.Compiler.Text.SourceText
FSharp.Compiler.Text.SourceText: FSharp.Compiler.Text.ISourceText ofString(System.String)
FSharp.Compiler.TextLayout.Layout
FSharp.Compiler.TextLayout.Layout: System.String ToString()
FSharp.Compiler.TextLayout.LayoutModule
FSharp.Compiler.TextLayout.LayoutModule: Boolean isEmptyL(FSharp.Compiler.TextLayout.Layout)
FSharp.Compiler.TextLayout.LayoutModule: FSharp.Compiler.TextLayout.Layout aboveL(FSharp.Compiler.TextLayout.Layout, FSharp.Compiler.TextLayout.Layout)
FSharp.Compiler.TextLayout.LayoutModule: FSharp.Compiler.TextLayout.Layout aboveListL(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.TextLayout.Layout])
FSharp.Compiler.TextLayout.LayoutModule: FSharp.Compiler.TextLayout.Layout braceL(FSharp.Compiler.TextLayout.Layout)
FSharp.Compiler.TextLayout.LayoutModule: FSharp.Compiler.TextLayout.Layout bracketL(FSharp.Compiler.TextLayout.Layout)
FSharp.Compiler.TextLayout.LayoutModule: FSharp.Compiler.TextLayout.Layout commaListL(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.TextLayout.Layout])
FSharp.Compiler.TextLayout.LayoutModule: FSharp.Compiler.TextLayout.Layout emptyL
FSharp.Compiler.TextLayout.LayoutModule: FSharp.Compiler.TextLayout.Layout get_emptyL()
FSharp.Compiler.TextLayout.LayoutModule: FSharp.Compiler.TextLayout.Layout leftL(FSharp.Compiler.TextLayout.TaggedText)
FSharp.Compiler.TextLayout.LayoutModule: FSharp.Compiler.TextLayout.Layout listL[T](Microsoft.FSharp.Core.FSharpFunc`2[T,FSharp.Compiler.TextLayout.Layout], Microsoft.FSharp.Collections.FSharpList`1[T])
FSharp.Compiler.TextLayout.LayoutModule: FSharp.Compiler.TextLayout.Layout objL(System.Object)
FSharp.Compiler.TextLayout.LayoutModule: FSharp.Compiler.TextLayout.Layout op_AtAt(FSharp.Compiler.TextLayout.Layout, FSharp.Compiler.TextLayout.Layout)
FSharp.Compiler.TextLayout.LayoutModule: FSharp.Compiler.TextLayout.Layout op_AtAtMinus(FSharp.Compiler.TextLayout.Layout, FSharp.Compiler.TextLayout.Layout)
FSharp.Compiler.TextLayout.LayoutModule: FSharp.Compiler.TextLayout.Layout op_AtAtMinusMinus(FSharp.Compiler.TextLayout.Layout, FSharp.Compiler.TextLayout.Layout)
FSharp.Compiler.TextLayout.LayoutModule: FSharp.Compiler.TextLayout.Layout op_HatHat(FSharp.Compiler.TextLayout.Layout, FSharp.Compiler.TextLayout.Layout)
FSharp.Compiler.TextLayout.LayoutModule: FSharp.Compiler.TextLayout.Layout op_MinusMinus(FSharp.Compiler.TextLayout.Layout, FSharp.Compiler.TextLayout.Layout)
FSharp.Compiler.TextLayout.LayoutModule: FSharp.Compiler.TextLayout.Layout op_MinusMinusMinus(FSharp.Compiler.TextLayout.Layout, FSharp.Compiler.TextLayout.Layout)
FSharp.Compiler.TextLayout.LayoutModule: FSharp.Compiler.TextLayout.Layout op_PlusPlus(FSharp.Compiler.TextLayout.Layout, FSharp.Compiler.TextLayout.Layout)
FSharp.Compiler.TextLayout.LayoutModule: FSharp.Compiler.TextLayout.Layout optionL[T](Microsoft.FSharp.Core.FSharpFunc`2[T,FSharp.Compiler.TextLayout.Layout], Microsoft.FSharp.Core.FSharpOption`1[T])
FSharp.Compiler.TextLayout.LayoutModule: FSharp.Compiler.TextLayout.Layout rightL(FSharp.Compiler.TextLayout.TaggedText)
FSharp.Compiler.TextLayout.LayoutModule: FSharp.Compiler.TextLayout.Layout semiListL(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.TextLayout.Layout])
FSharp.Compiler.TextLayout.LayoutModule: FSharp.Compiler.TextLayout.Layout sepL(FSharp.Compiler.TextLayout.TaggedText)
FSharp.Compiler.TextLayout.LayoutModule: FSharp.Compiler.TextLayout.Layout sepListL(FSharp.Compiler.TextLayout.Layout, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.TextLayout.Layout])
FSharp.Compiler.TextLayout.LayoutModule: FSharp.Compiler.TextLayout.Layout spaceListL(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.TextLayout.Layout])
FSharp.Compiler.TextLayout.LayoutModule: FSharp.Compiler.TextLayout.Layout squareBracketL(FSharp.Compiler.TextLayout.Layout)
FSharp.Compiler.TextLayout.LayoutModule: FSharp.Compiler.TextLayout.Layout tagAttrL(System.String, Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[System.String,System.String]], FSharp.Compiler.TextLayout.Layout)
FSharp.Compiler.TextLayout.LayoutModule: FSharp.Compiler.TextLayout.Layout tupleL(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.TextLayout.Layout])
FSharp.Compiler.TextLayout.LayoutModule: FSharp.Compiler.TextLayout.Layout wordL(FSharp.Compiler.TextLayout.TaggedText)
FSharp.Compiler.TextLayout.LayoutModule: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.TextLayout.Layout] unfoldL[T,State](Microsoft.FSharp.Core.FSharpFunc`2[T,FSharp.Compiler.TextLayout.Layout], Microsoft.FSharp.Core.FSharpFunc`2[State,Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[T,State]]], State, Int32)
FSharp.Compiler.TextLayout.LayoutRender
FSharp.Compiler.TextLayout.LayoutRender: Void emitL(Microsoft.FSharp.Core.FSharpFunc`2[FSharp.Compiler.TextLayout.TaggedText,Microsoft.FSharp.Core.Unit], FSharp.Compiler.TextLayout.Layout)
FSharp.Compiler.TextLayout.LayoutTag
FSharp.Compiler.TextLayout.LayoutTag+Tags: Int32 ActivePatternCase
FSharp.Compiler.TextLayout.LayoutTag+Tags: Int32 ActivePatternResult
FSharp.Compiler.TextLayout.LayoutTag+Tags: Int32 Alias
FSharp.Compiler.TextLayout.LayoutTag+Tags: Int32 Class
FSharp.Compiler.TextLayout.LayoutTag+Tags: Int32 Delegate
FSharp.Compiler.TextLayout.LayoutTag+Tags: Int32 Enum
FSharp.Compiler.TextLayout.LayoutTag+Tags: Int32 Event
FSharp.Compiler.TextLayout.LayoutTag+Tags: Int32 Field
FSharp.Compiler.TextLayout.LayoutTag+Tags: Int32 Function
FSharp.Compiler.TextLayout.LayoutTag+Tags: Int32 Interface
FSharp.Compiler.TextLayout.LayoutTag+Tags: Int32 Keyword
FSharp.Compiler.TextLayout.LayoutTag+Tags: Int32 LineBreak
FSharp.Compiler.TextLayout.LayoutTag+Tags: Int32 Local
FSharp.Compiler.TextLayout.LayoutTag+Tags: Int32 Member
FSharp.Compiler.TextLayout.LayoutTag+Tags: Int32 Method
FSharp.Compiler.TextLayout.LayoutTag+Tags: Int32 Module
FSharp.Compiler.TextLayout.LayoutTag+Tags: Int32 ModuleBinding
FSharp.Compiler.TextLayout.LayoutTag+Tags: Int32 Namespace
FSharp.Compiler.TextLayout.LayoutTag+Tags: Int32 NumericLiteral
FSharp.Compiler.TextLayout.LayoutTag+Tags: Int32 Operator
FSharp.Compiler.TextLayout.LayoutTag+Tags: Int32 Parameter
FSharp.Compiler.TextLayout.LayoutTag+Tags: Int32 Property
FSharp.Compiler.TextLayout.LayoutTag+Tags: Int32 Punctuation
FSharp.Compiler.TextLayout.LayoutTag+Tags: Int32 Record
FSharp.Compiler.TextLayout.LayoutTag+Tags: Int32 RecordField
FSharp.Compiler.TextLayout.LayoutTag+Tags: Int32 Space
FSharp.Compiler.TextLayout.LayoutTag+Tags: Int32 StringLiteral
FSharp.Compiler.TextLayout.LayoutTag+Tags: Int32 Struct
FSharp.Compiler.TextLayout.LayoutTag+Tags: Int32 Text
FSharp.Compiler.TextLayout.LayoutTag+Tags: Int32 TypeParameter
FSharp.Compiler.TextLayout.LayoutTag+Tags: Int32 Union
FSharp.Compiler.TextLayout.LayoutTag+Tags: Int32 UnionCase
FSharp.Compiler.TextLayout.LayoutTag+Tags: Int32 UnknownEntity
FSharp.Compiler.TextLayout.LayoutTag+Tags: Int32 UnknownType
FSharp.Compiler.TextLayout.LayoutTag: Boolean Equals(FSharp.Compiler.TextLayout.LayoutTag)
FSharp.Compiler.TextLayout.LayoutTag: Boolean Equals(System.Object)
FSharp.Compiler.TextLayout.LayoutTag: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.TextLayout.LayoutTag: Boolean IsActivePatternCase
FSharp.Compiler.TextLayout.LayoutTag: Boolean IsActivePatternResult
FSharp.Compiler.TextLayout.LayoutTag: Boolean IsAlias
FSharp.Compiler.TextLayout.LayoutTag: Boolean IsClass
FSharp.Compiler.TextLayout.LayoutTag: Boolean IsDelegate
FSharp.Compiler.TextLayout.LayoutTag: Boolean IsEnum
FSharp.Compiler.TextLayout.LayoutTag: Boolean IsEvent
FSharp.Compiler.TextLayout.LayoutTag: Boolean IsField
FSharp.Compiler.TextLayout.LayoutTag: Boolean IsFunction
FSharp.Compiler.TextLayout.LayoutTag: Boolean IsInterface
FSharp.Compiler.TextLayout.LayoutTag: Boolean IsKeyword
FSharp.Compiler.TextLayout.LayoutTag: Boolean IsLineBreak
FSharp.Compiler.TextLayout.LayoutTag: Boolean IsLocal
FSharp.Compiler.TextLayout.LayoutTag: Boolean IsMember
FSharp.Compiler.TextLayout.LayoutTag: Boolean IsMethod
FSharp.Compiler.TextLayout.LayoutTag: Boolean IsModule
FSharp.Compiler.TextLayout.LayoutTag: Boolean IsModuleBinding
FSharp.Compiler.TextLayout.LayoutTag: Boolean IsNamespace
FSharp.Compiler.TextLayout.LayoutTag: Boolean IsNumericLiteral
FSharp.Compiler.TextLayout.LayoutTag: Boolean IsOperator
FSharp.Compiler.TextLayout.LayoutTag: Boolean IsParameter
FSharp.Compiler.TextLayout.LayoutTag: Boolean IsProperty
FSharp.Compiler.TextLayout.LayoutTag: Boolean IsPunctuation
FSharp.Compiler.TextLayout.LayoutTag: Boolean IsRecord
FSharp.Compiler.TextLayout.LayoutTag: Boolean IsRecordField
FSharp.Compiler.TextLayout.LayoutTag: Boolean IsSpace
FSharp.Compiler.TextLayout.LayoutTag: Boolean IsStringLiteral
FSharp.Compiler.TextLayout.LayoutTag: Boolean IsStruct
FSharp.Compiler.TextLayout.LayoutTag: Boolean IsText
FSharp.Compiler.TextLayout.LayoutTag: Boolean IsTypeParameter
FSharp.Compiler.TextLayout.LayoutTag: Boolean IsUnion
FSharp.Compiler.TextLayout.LayoutTag: Boolean IsUnionCase
FSharp.Compiler.TextLayout.LayoutTag: Boolean IsUnknownEntity
FSharp.Compiler.TextLayout.LayoutTag: Boolean IsUnknownType
FSharp.Compiler.TextLayout.LayoutTag: Boolean get_IsActivePatternCase()
FSharp.Compiler.TextLayout.LayoutTag: Boolean get_IsActivePatternResult()
FSharp.Compiler.TextLayout.LayoutTag: Boolean get_IsAlias()
FSharp.Compiler.TextLayout.LayoutTag: Boolean get_IsClass()
FSharp.Compiler.TextLayout.LayoutTag: Boolean get_IsDelegate()
FSharp.Compiler.TextLayout.LayoutTag: Boolean get_IsEnum()
FSharp.Compiler.TextLayout.LayoutTag: Boolean get_IsEvent()
FSharp.Compiler.TextLayout.LayoutTag: Boolean get_IsField()
FSharp.Compiler.TextLayout.LayoutTag: Boolean get_IsFunction()
FSharp.Compiler.TextLayout.LayoutTag: Boolean get_IsInterface()
FSharp.Compiler.TextLayout.LayoutTag: Boolean get_IsKeyword()
FSharp.Compiler.TextLayout.LayoutTag: Boolean get_IsLineBreak()
FSharp.Compiler.TextLayout.LayoutTag: Boolean get_IsLocal()
FSharp.Compiler.TextLayout.LayoutTag: Boolean get_IsMember()
FSharp.Compiler.TextLayout.LayoutTag: Boolean get_IsMethod()
FSharp.Compiler.TextLayout.LayoutTag: Boolean get_IsModule()
FSharp.Compiler.TextLayout.LayoutTag: Boolean get_IsModuleBinding()
FSharp.Compiler.TextLayout.LayoutTag: Boolean get_IsNamespace()
FSharp.Compiler.TextLayout.LayoutTag: Boolean get_IsNumericLiteral()
FSharp.Compiler.TextLayout.LayoutTag: Boolean get_IsOperator()
FSharp.Compiler.TextLayout.LayoutTag: Boolean get_IsParameter()
FSharp.Compiler.TextLayout.LayoutTag: Boolean get_IsProperty()
FSharp.Compiler.TextLayout.LayoutTag: Boolean get_IsPunctuation()
FSharp.Compiler.TextLayout.LayoutTag: Boolean get_IsRecord()
FSharp.Compiler.TextLayout.LayoutTag: Boolean get_IsRecordField()
FSharp.Compiler.TextLayout.LayoutTag: Boolean get_IsSpace()
FSharp.Compiler.TextLayout.LayoutTag: Boolean get_IsStringLiteral()
FSharp.Compiler.TextLayout.LayoutTag: Boolean get_IsStruct()
FSharp.Compiler.TextLayout.LayoutTag: Boolean get_IsText()
FSharp.Compiler.TextLayout.LayoutTag: Boolean get_IsTypeParameter()
FSharp.Compiler.TextLayout.LayoutTag: Boolean get_IsUnion()
FSharp.Compiler.TextLayout.LayoutTag: Boolean get_IsUnionCase()
FSharp.Compiler.TextLayout.LayoutTag: Boolean get_IsUnknownEntity()
FSharp.Compiler.TextLayout.LayoutTag: Boolean get_IsUnknownType()
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag ActivePatternCase
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag ActivePatternResult
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag Alias
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag Class
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag Delegate
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag Enum
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag Event
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag Field
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag Function
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag Interface
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag Keyword
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag LineBreak
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag Local
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag Member
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag Method
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag Module
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag ModuleBinding
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag Namespace
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag NumericLiteral
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag Operator
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag Parameter
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag Property
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag Punctuation
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag Record
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag RecordField
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag Space
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag StringLiteral
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag Struct
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag Text
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag TypeParameter
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag Union
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag UnionCase
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag UnknownEntity
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag UnknownType
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag get_ActivePatternCase()
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag get_ActivePatternResult()
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag get_Alias()
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag get_Class()
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag get_Delegate()
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag get_Enum()
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag get_Event()
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag get_Field()
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag get_Function()
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag get_Interface()
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag get_Keyword()
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag get_LineBreak()
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag get_Local()
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag get_Member()
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag get_Method()
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag get_Module()
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag get_ModuleBinding()
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag get_Namespace()
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag get_NumericLiteral()
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag get_Operator()
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag get_Parameter()
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag get_Property()
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag get_Punctuation()
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag get_Record()
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag get_RecordField()
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag get_Space()
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag get_StringLiteral()
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag get_Struct()
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag get_Text()
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag get_TypeParameter()
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag get_Union()
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag get_UnionCase()
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag get_UnknownEntity()
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag get_UnknownType()
FSharp.Compiler.TextLayout.LayoutTag: FSharp.Compiler.TextLayout.LayoutTag+Tags
FSharp.Compiler.TextLayout.LayoutTag: Int32 GetHashCode()
FSharp.Compiler.TextLayout.LayoutTag: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.TextLayout.LayoutTag: Int32 Tag
FSharp.Compiler.TextLayout.LayoutTag: Int32 get_Tag()
FSharp.Compiler.TextLayout.LayoutTag: System.String ToString()
FSharp.Compiler.TextLayout.NavigableTaggedText
FSharp.Compiler.TextLayout.NavigableTaggedText: FSharp.Compiler.Text.Range Range
FSharp.Compiler.TextLayout.NavigableTaggedText: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.TextLayout.TaggedText
FSharp.Compiler.TextLayout.TaggedText: FSharp.Compiler.TextLayout.LayoutTag Tag
FSharp.Compiler.TextLayout.TaggedText: FSharp.Compiler.TextLayout.LayoutTag get_Tag()
FSharp.Compiler.TextLayout.TaggedText: System.String Text
FSharp.Compiler.TextLayout.TaggedText: System.String ToString()
FSharp.Compiler.TextLayout.TaggedText: System.String get_Text()
FSharp.Compiler.TextLayout.TaggedText: Void .ctor(FSharp.Compiler.TextLayout.LayoutTag, System.String)
FSharp.Compiler.TextLayout.TaggedTextModule
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText arrow
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText bar
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText colon
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText comma
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText dot
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText equals
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_arrow()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_bar()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_colon()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_comma()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_dot()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_equals()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_keywordAbstract()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_keywordDelegate()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_keywordEnd()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_keywordEnum()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_keywordEvent()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_keywordFalse()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_keywordGet()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_keywordInherit()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_keywordInternal()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_keywordMember()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_keywordNested()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_keywordNew()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_keywordOf()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_keywordOverride()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_keywordPrivate()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_keywordSet()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_keywordStatic()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_keywordStruct()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_keywordTrue()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_keywordType()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_keywordTypedefof()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_keywordTypeof()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_keywordVal()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_keywordWith()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_leftAngle()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_leftBrace()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_leftBraceBar()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_leftBracket()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_leftBracketAngle()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_leftBracketBar()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_leftParen()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_lineBreak()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_minus()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_questionMark()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_rightAngle()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_rightBrace()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_rightBraceBar()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_rightBracket()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_rightBracketAngle()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_rightBracketBar()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_rightParen()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_semicolon()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_space()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_star()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText get_structUnit()
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText keywordAbstract
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText keywordDelegate
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText keywordEnd
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText keywordEnum
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText keywordEvent
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText keywordFalse
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText keywordGet
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText keywordInherit
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText keywordInternal
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText keywordMember
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText keywordNested
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText keywordNew
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText keywordOf
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText keywordOverride
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText keywordPrivate
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText keywordSet
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText keywordStatic
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText keywordStruct
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText keywordTrue
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText keywordType
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText keywordTypedefof
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText keywordTypeof
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText keywordVal
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText keywordWith
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText leftAngle
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText leftBrace
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText leftBraceBar
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText leftBracket
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText leftBracketAngle
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText leftBracketBar
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText leftParen
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText lineBreak
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText minus
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText mkTag(FSharp.Compiler.TextLayout.LayoutTag, System.String)
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText questionMark
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText rightAngle
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText rightBrace
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText rightBraceBar
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText rightBracket
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText rightBracketAngle
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText rightBracketBar
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText rightParen
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText semicolon
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText space
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText star
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText structUnit
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText tagActivePatternCase(System.String)
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText tagActivePatternResult(System.String)
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText tagAlias(System.String)
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText tagClass(System.String)
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText tagDelegate(System.String)
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText tagEnum(System.String)
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText tagEvent(System.String)
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText tagField(System.String)
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText tagFunction(System.String)
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText tagInterface(System.String)
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText tagKeyword(System.String)
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText tagLineBreak(System.String)
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText tagLocal(System.String)
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText tagMember(System.String)
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText tagMethod(System.String)
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText tagModule(System.String)
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText tagModuleBinding(System.String)
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText tagNamespace(System.String)
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText tagNumericLiteral(System.String)
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText tagOperator(System.String)
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText tagParameter(System.String)
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText tagProperty(System.String)
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText tagPunctuation(System.String)
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText tagRecord(System.String)
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText tagRecordField(System.String)
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText tagSpace(System.String)
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText tagStringLiteral(System.String)
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText tagStruct(System.String)
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText tagText(System.String)
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText tagTypeParameter(System.String)
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText tagUnion(System.String)
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText tagUnionCase(System.String)
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText tagUnknownEntity(System.String)
FSharp.Compiler.TextLayout.TaggedTextModule: FSharp.Compiler.TextLayout.TaggedText tagUnknownType(System.String)
FSharp.Compiler.TextLayout.TaggedTextModule: Microsoft.FSharp.Collections.FSharpSet`1[System.String] get_keywordFunctions()
FSharp.Compiler.TextLayout.TaggedTextModule: Microsoft.FSharp.Collections.FSharpSet`1[System.String] keywordFunctions"
        SurfaceArea.verify expected "netstandard" (System.IO.Path.Combine(__SOURCE_DIRECTORY__,__SOURCE_FILE__))