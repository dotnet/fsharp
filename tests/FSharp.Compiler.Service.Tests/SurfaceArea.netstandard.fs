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
FSharp.Compiler.AbstractIL.IL+ILMethodDef: ILLazyMethodBody Body
FSharp.Compiler.AbstractIL.IL+ILMethodDef: ILLazyMethodBody get_Body()
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
FSharp.Compiler.AbstractIL.IL+ILMethodDef: Void .ctor(System.String, System.Reflection.MethodAttributes, System.Reflection.MethodImplAttributes, ILCallingConv, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILParameter], ILReturn, ILLazyMethodBody, Boolean, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.AbstractIL.IL+ILGenericParameterDef], ILSecurityDecls, ILAttributes)
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
FSharp.Compiler.Interactive.Shell+EvaluationEventArgs: FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration ImplementationDeclaration
FSharp.Compiler.Interactive.Shell+EvaluationEventArgs: FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration get_ImplementationDeclaration()
FSharp.Compiler.Interactive.Shell+EvaluationEventArgs: FSharp.Compiler.SourceCodeServices.FSharpSymbol Symbol
FSharp.Compiler.Interactive.Shell+EvaluationEventArgs: FSharp.Compiler.SourceCodeServices.FSharpSymbol get_Symbol()
FSharp.Compiler.Interactive.Shell+EvaluationEventArgs: FSharp.Compiler.SourceCodeServices.FSharpSymbolUse SymbolUse
FSharp.Compiler.Interactive.Shell+EvaluationEventArgs: FSharp.Compiler.SourceCodeServices.FSharpSymbolUse get_SymbolUse()
FSharp.Compiler.Interactive.Shell+EvaluationEventArgs: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Interactive.Shell+FsiValue] FsiValue
FSharp.Compiler.Interactive.Shell+EvaluationEventArgs: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Interactive.Shell+FsiValue] get_FsiValue()
FSharp.Compiler.Interactive.Shell+EvaluationEventArgs: System.String Name
FSharp.Compiler.Interactive.Shell+EvaluationEventArgs: System.String get_Name()
FSharp.Compiler.Interactive.Shell+FsiBoundValue: FsiValue Value
FSharp.Compiler.Interactive.Shell+FsiBoundValue: FsiValue get_Value()
FSharp.Compiler.Interactive.Shell+FsiBoundValue: System.String Name
FSharp.Compiler.Interactive.Shell+FsiBoundValue: System.String get_Name()
FSharp.Compiler.Interactive.Shell+FsiCompilationException: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpDiagnostic[]] ErrorInfos
FSharp.Compiler.Interactive.Shell+FsiCompilationException: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpDiagnostic[]] get_ErrorInfos()
FSharp.Compiler.Interactive.Shell+FsiCompilationException: Void .ctor(System.String, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpDiagnostic[]])
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: Boolean IsGui
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: Boolean get_IsGui()
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: FSharp.Compiler.SourceCodeServices.FSharpAssemblySignature CurrentPartialAssemblySignature
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: FSharp.Compiler.SourceCodeServices.FSharpAssemblySignature get_CurrentPartialAssemblySignature()
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: FSharp.Compiler.SourceCodeServices.FSharpChecker InteractiveChecker
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: FSharp.Compiler.SourceCodeServices.FSharpChecker get_InteractiveChecker()
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: FsiEvaluationSession Create(FsiEvaluationSessionHostConfig, System.String[], System.IO.TextReader, System.IO.TextWriter, System.IO.TextWriter, Microsoft.FSharp.Core.FSharpOption`1[System.Boolean], Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.LegacyReferenceResolver])
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: FsiEvaluationSessionHostConfig GetDefaultConfiguration()
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: FsiEvaluationSessionHostConfig GetDefaultConfiguration(System.Object)
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: FsiEvaluationSessionHostConfig GetDefaultConfiguration(System.Object, Boolean)
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Interactive.Shell+FsiBoundValue] GetBoundValues()
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: Microsoft.FSharp.Control.FSharpAsync`1[System.Tuple`3[FSharp.Compiler.SourceCodeServices.FSharpParseFileResults,FSharp.Compiler.SourceCodeServices.FSharpCheckFileResults,FSharp.Compiler.SourceCodeServices.FSharpCheckProjectResults]] ParseAndCheckInteraction(System.String)
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
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: System.Tuple`2[Microsoft.FSharp.Core.FSharpChoice`2[Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Interactive.Shell+FsiValue],System.Exception],FSharp.Compiler.SourceCodeServices.FSharpDiagnostic[]] EvalExpressionNonThrowing(System.String)
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: System.Tuple`2[Microsoft.FSharp.Core.FSharpChoice`2[Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Interactive.Shell+FsiValue],System.Exception],FSharp.Compiler.SourceCodeServices.FSharpDiagnostic[]] EvalInteractionNonThrowing(System.String, Microsoft.FSharp.Core.FSharpOption`1[System.Threading.CancellationToken])
FSharp.Compiler.Interactive.Shell+FsiEvaluationSession: System.Tuple`2[Microsoft.FSharp.Core.FSharpChoice`2[Microsoft.FSharp.Core.Unit,System.Exception],FSharp.Compiler.SourceCodeServices.FSharpDiagnostic[]] EvalScriptNonThrowing(System.String)
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
FSharp.Compiler.LegacyReferenceResolver
FSharp.Compiler.SourceCodeServices.AssemblyContentProvider
FSharp.Compiler.SourceCodeServices.AssemblyContentProvider: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.AssemblySymbol] getAssemblyContent(Microsoft.FSharp.Core.FSharpFunc`2[Microsoft.FSharp.Core.FSharpFunc`2[FSharp.Compiler.SourceCodeServices.IAssemblyContentCache,Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.AssemblySymbol]],Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.AssemblySymbol]], FSharp.Compiler.SourceCodeServices.AssemblyContentType, Microsoft.FSharp.Core.FSharpOption`1[System.String], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpAssembly])
FSharp.Compiler.SourceCodeServices.AssemblyContentProvider: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.AssemblySymbol] getAssemblySignatureContent(FSharp.Compiler.SourceCodeServices.AssemblyContentType, FSharp.Compiler.SourceCodeServices.FSharpAssemblySignature)
FSharp.Compiler.SourceCodeServices.AssemblyContentType
FSharp.Compiler.SourceCodeServices.AssemblyContentType+Tags: Int32 Full
FSharp.Compiler.SourceCodeServices.AssemblyContentType+Tags: Int32 Public
FSharp.Compiler.SourceCodeServices.AssemblyContentType: Boolean Equals(FSharp.Compiler.SourceCodeServices.AssemblyContentType)
FSharp.Compiler.SourceCodeServices.AssemblyContentType: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.AssemblyContentType: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.AssemblyContentType: Boolean IsFull
FSharp.Compiler.SourceCodeServices.AssemblyContentType: Boolean IsPublic
FSharp.Compiler.SourceCodeServices.AssemblyContentType: Boolean get_IsFull()
FSharp.Compiler.SourceCodeServices.AssemblyContentType: Boolean get_IsPublic()
FSharp.Compiler.SourceCodeServices.AssemblyContentType: FSharp.Compiler.SourceCodeServices.AssemblyContentType Full
FSharp.Compiler.SourceCodeServices.AssemblyContentType: FSharp.Compiler.SourceCodeServices.AssemblyContentType Public
FSharp.Compiler.SourceCodeServices.AssemblyContentType: FSharp.Compiler.SourceCodeServices.AssemblyContentType get_Full()
FSharp.Compiler.SourceCodeServices.AssemblyContentType: FSharp.Compiler.SourceCodeServices.AssemblyContentType get_Public()
FSharp.Compiler.SourceCodeServices.AssemblyContentType: FSharp.Compiler.SourceCodeServices.AssemblyContentType+Tags
FSharp.Compiler.SourceCodeServices.AssemblyContentType: Int32 CompareTo(FSharp.Compiler.SourceCodeServices.AssemblyContentType)
FSharp.Compiler.SourceCodeServices.AssemblyContentType: Int32 CompareTo(System.Object)
FSharp.Compiler.SourceCodeServices.AssemblyContentType: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.SourceCodeServices.AssemblyContentType: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.AssemblyContentType: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.AssemblyContentType: Int32 Tag
FSharp.Compiler.SourceCodeServices.AssemblyContentType: Int32 get_Tag()
FSharp.Compiler.SourceCodeServices.AssemblyContentType: System.String ToString()
FSharp.Compiler.SourceCodeServices.AssemblySymbol
FSharp.Compiler.SourceCodeServices.AssemblySymbol: FSharp.Compiler.SourceCodeServices.FSharpSymbol Symbol
FSharp.Compiler.SourceCodeServices.AssemblySymbol: FSharp.Compiler.SourceCodeServices.FSharpSymbol get_Symbol()
FSharp.Compiler.SourceCodeServices.AssemblySymbol: FSharp.Compiler.SourceCodeServices.FSharpUnresolvedSymbol FSharpUnresolvedSymbol
FSharp.Compiler.SourceCodeServices.AssemblySymbol: FSharp.Compiler.SourceCodeServices.FSharpUnresolvedSymbol get_FSharpUnresolvedSymbol()
FSharp.Compiler.SourceCodeServices.AssemblySymbol: Microsoft.FSharp.Core.FSharpFunc`2[FSharp.Compiler.SourceCodeServices.LookupType,FSharp.Compiler.SourceCodeServices.EntityKind] Kind
FSharp.Compiler.SourceCodeServices.AssemblySymbol: Microsoft.FSharp.Core.FSharpFunc`2[FSharp.Compiler.SourceCodeServices.LookupType,FSharp.Compiler.SourceCodeServices.EntityKind] get_Kind()
FSharp.Compiler.SourceCodeServices.AssemblySymbol: Microsoft.FSharp.Core.FSharpOption`1[System.String[]] AutoOpenParent
FSharp.Compiler.SourceCodeServices.AssemblySymbol: Microsoft.FSharp.Core.FSharpOption`1[System.String[]] Namespace
FSharp.Compiler.SourceCodeServices.AssemblySymbol: Microsoft.FSharp.Core.FSharpOption`1[System.String[]] NearestRequireQualifiedAccessParent
FSharp.Compiler.SourceCodeServices.AssemblySymbol: Microsoft.FSharp.Core.FSharpOption`1[System.String[]] TopRequireQualifiedAccessParent
FSharp.Compiler.SourceCodeServices.AssemblySymbol: Microsoft.FSharp.Core.FSharpOption`1[System.String[]] get_AutoOpenParent()
FSharp.Compiler.SourceCodeServices.AssemblySymbol: Microsoft.FSharp.Core.FSharpOption`1[System.String[]] get_Namespace()
FSharp.Compiler.SourceCodeServices.AssemblySymbol: Microsoft.FSharp.Core.FSharpOption`1[System.String[]] get_NearestRequireQualifiedAccessParent()
FSharp.Compiler.SourceCodeServices.AssemblySymbol: Microsoft.FSharp.Core.FSharpOption`1[System.String[]] get_TopRequireQualifiedAccessParent()
FSharp.Compiler.SourceCodeServices.AssemblySymbol: System.String FullName
FSharp.Compiler.SourceCodeServices.AssemblySymbol: System.String ToString()
FSharp.Compiler.SourceCodeServices.AssemblySymbol: System.String get_FullName()
FSharp.Compiler.SourceCodeServices.AssemblySymbol: System.String[] CleanedIdents
FSharp.Compiler.SourceCodeServices.AssemblySymbol: System.String[] get_CleanedIdents()
FSharp.Compiler.SourceCodeServices.AssemblySymbol: Void .ctor(System.String, System.String[], Microsoft.FSharp.Core.FSharpOption`1[System.String[]], Microsoft.FSharp.Core.FSharpOption`1[System.String[]], Microsoft.FSharp.Core.FSharpOption`1[System.String[]], Microsoft.FSharp.Core.FSharpOption`1[System.String[]], FSharp.Compiler.SourceCodeServices.FSharpSymbol, Microsoft.FSharp.Core.FSharpFunc`2[FSharp.Compiler.SourceCodeServices.LookupType,FSharp.Compiler.SourceCodeServices.EntityKind], FSharp.Compiler.SourceCodeServices.FSharpUnresolvedSymbol)
FSharp.Compiler.SourceCodeServices.AstTraversal
FSharp.Compiler.SourceCodeServices.AstTraversal+AstVisitorBase`1[T]: Microsoft.FSharp.Core.FSharpOption`1[T] VisitBinding(Microsoft.FSharp.Core.FSharpFunc`2[FSharp.Compiler.SyntaxTree+SynBinding,Microsoft.FSharp.Core.FSharpOption`1[T]], SynBinding)
FSharp.Compiler.SourceCodeServices.AstTraversal+AstVisitorBase`1[T]: Microsoft.FSharp.Core.FSharpOption`1[T] VisitComponentInfo(SynComponentInfo)
FSharp.Compiler.SourceCodeServices.AstTraversal+AstVisitorBase`1[T]: Microsoft.FSharp.Core.FSharpOption`1[T] VisitExpr(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep], Microsoft.FSharp.Core.FSharpFunc`2[FSharp.Compiler.SyntaxTree+SynExpr,Microsoft.FSharp.Core.FSharpOption`1[T]], Microsoft.FSharp.Core.FSharpFunc`2[FSharp.Compiler.SyntaxTree+SynExpr,Microsoft.FSharp.Core.FSharpOption`1[T]], SynExpr)
FSharp.Compiler.SourceCodeServices.AstTraversal+AstVisitorBase`1[T]: Microsoft.FSharp.Core.FSharpOption`1[T] VisitHashDirective(FSharp.Compiler.Text.Range)
FSharp.Compiler.SourceCodeServices.AstTraversal+AstVisitorBase`1[T]: Microsoft.FSharp.Core.FSharpOption`1[T] VisitImplicitInherit(Microsoft.FSharp.Core.FSharpFunc`2[FSharp.Compiler.SyntaxTree+SynExpr,Microsoft.FSharp.Core.FSharpOption`1[T]], SynType, SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.SourceCodeServices.AstTraversal+AstVisitorBase`1[T]: Microsoft.FSharp.Core.FSharpOption`1[T] VisitInheritSynMemberDefn(SynComponentInfo, SynTypeDefnKind, SynType, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynMemberDefn], FSharp.Compiler.Text.Range)
FSharp.Compiler.SourceCodeServices.AstTraversal+AstVisitorBase`1[T]: Microsoft.FSharp.Core.FSharpOption`1[T] VisitInterfaceSynMemberDefnType(SynType)
FSharp.Compiler.SourceCodeServices.AstTraversal+AstVisitorBase`1[T]: Microsoft.FSharp.Core.FSharpOption`1[T] VisitLetOrUse(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep], Microsoft.FSharp.Core.FSharpFunc`2[FSharp.Compiler.SyntaxTree+SynBinding,Microsoft.FSharp.Core.FSharpOption`1[T]], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynBinding], FSharp.Compiler.Text.Range)
FSharp.Compiler.SourceCodeServices.AstTraversal+AstVisitorBase`1[T]: Microsoft.FSharp.Core.FSharpOption`1[T] VisitMatchClause(Microsoft.FSharp.Core.FSharpFunc`2[FSharp.Compiler.SyntaxTree+SynMatchClause,Microsoft.FSharp.Core.FSharpOption`1[T]], SynMatchClause)
FSharp.Compiler.SourceCodeServices.AstTraversal+AstVisitorBase`1[T]: Microsoft.FSharp.Core.FSharpOption`1[T] VisitModuleDecl(Microsoft.FSharp.Core.FSharpFunc`2[FSharp.Compiler.SyntaxTree+SynModuleDecl,Microsoft.FSharp.Core.FSharpOption`1[T]], SynModuleDecl)
FSharp.Compiler.SourceCodeServices.AstTraversal+AstVisitorBase`1[T]: Microsoft.FSharp.Core.FSharpOption`1[T] VisitModuleOrNamespace(SynModuleOrNamespace)
FSharp.Compiler.SourceCodeServices.AstTraversal+AstVisitorBase`1[T]: Microsoft.FSharp.Core.FSharpOption`1[T] VisitPat(Microsoft.FSharp.Core.FSharpFunc`2[FSharp.Compiler.SyntaxTree+SynPat,Microsoft.FSharp.Core.FSharpOption`1[T]], SynPat)
FSharp.Compiler.SourceCodeServices.AstTraversal+AstVisitorBase`1[T]: Microsoft.FSharp.Core.FSharpOption`1[T] VisitRecordField(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep], Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynExpr], Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+LongIdentWithDots])
FSharp.Compiler.SourceCodeServices.AstTraversal+AstVisitorBase`1[T]: Microsoft.FSharp.Core.FSharpOption`1[T] VisitSimplePats(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynSimplePat])
FSharp.Compiler.SourceCodeServices.AstTraversal+AstVisitorBase`1[T]: Microsoft.FSharp.Core.FSharpOption`1[T] VisitType(Microsoft.FSharp.Core.FSharpFunc`2[FSharp.Compiler.SyntaxTree+SynType,Microsoft.FSharp.Core.FSharpOption`1[T]], SynType)
FSharp.Compiler.SourceCodeServices.AstTraversal+AstVisitorBase`1[T]: Microsoft.FSharp.Core.FSharpOption`1[T] VisitTypeAbbrev(SynType, FSharp.Compiler.Text.Range)
FSharp.Compiler.SourceCodeServices.AstTraversal+AstVisitorBase`1[T]: Void .ctor()
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep+Binding: SynBinding Item
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep+Binding: SynBinding get_Item()
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep+Expr: SynExpr Item
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep+Expr: SynExpr get_Item()
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep+MatchClause: SynMatchClause Item
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep+MatchClause: SynMatchClause get_Item()
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep+MemberDefn: SynMemberDefn Item
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep+MemberDefn: SynMemberDefn get_Item()
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep+Module: SynModuleDecl Item
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep+Module: SynModuleDecl get_Item()
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep+ModuleOrNamespace: SynModuleOrNamespace Item
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep+ModuleOrNamespace: SynModuleOrNamespace get_Item()
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep+Tags: Int32 Binding
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep+Tags: Int32 Expr
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep+Tags: Int32 MatchClause
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep+Tags: Int32 MemberDefn
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep+Tags: Int32 Module
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep+Tags: Int32 ModuleOrNamespace
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep+Tags: Int32 TypeDefn
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep+TypeDefn: SynTypeDefn Item
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep+TypeDefn: SynTypeDefn get_Item()
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep: Boolean IsBinding
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep: Boolean IsExpr
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep: Boolean IsMatchClause
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep: Boolean IsMemberDefn
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep: Boolean IsModule
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep: Boolean IsModuleOrNamespace
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep: Boolean IsTypeDefn
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep: Boolean get_IsBinding()
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep: Boolean get_IsExpr()
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep: Boolean get_IsMatchClause()
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep: Boolean get_IsMemberDefn()
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep: Boolean get_IsModule()
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep: Boolean get_IsModuleOrNamespace()
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep: Boolean get_IsTypeDefn()
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep: FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep+Binding
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep: FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep+Expr
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep: FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep+MatchClause
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep: FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep+MemberDefn
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep: FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep+Module
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep: FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep+ModuleOrNamespace
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep: FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep+Tags
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep: FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep+TypeDefn
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep: Int32 Tag
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep: Int32 get_Tag()
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep: System.String ToString()
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep: TraverseStep NewBinding(SynBinding)
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep: TraverseStep NewExpr(SynExpr)
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep: TraverseStep NewMatchClause(SynMatchClause)
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep: TraverseStep NewMemberDefn(SynMemberDefn)
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep: TraverseStep NewModule(SynModuleDecl)
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep: TraverseStep NewModuleOrNamespace(SynModuleOrNamespace)
FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep: TraverseStep NewTypeDefn(SynTypeDefn)
FSharp.Compiler.SourceCodeServices.AstTraversal: Boolean rangeContainsPosEdgesExclusive(FSharp.Compiler.Text.Range, FSharp.Compiler.Text.Pos)
FSharp.Compiler.SourceCodeServices.AstTraversal: Boolean rangeContainsPosLeftEdgeExclusiveAndRightEdgeInclusive(FSharp.Compiler.Text.Range, FSharp.Compiler.Text.Pos)
FSharp.Compiler.SourceCodeServices.AstTraversal: Boolean rangeContainsPosLeftEdgeInclusive(FSharp.Compiler.Text.Range, FSharp.Compiler.Text.Pos)
FSharp.Compiler.SourceCodeServices.AstTraversal: FSharp.Compiler.SourceCodeServices.AstTraversal+AstVisitorBase`1[T]
FSharp.Compiler.SourceCodeServices.AstTraversal: FSharp.Compiler.SourceCodeServices.AstTraversal+TraverseStep
FSharp.Compiler.SourceCodeServices.AstTraversal: Microsoft.FSharp.Core.FSharpOption`1[T] Traverse[T](FSharp.Compiler.Text.Pos, ParsedInput, AstVisitorBase`1)
FSharp.Compiler.SourceCodeServices.AstTraversal: Microsoft.FSharp.Core.FSharpOption`1[a] pick[a](FSharp.Compiler.Text.Pos, FSharp.Compiler.Text.Range, System.Object, Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[FSharp.Compiler.Text.Range,Microsoft.FSharp.Core.FSharpFunc`2[Microsoft.FSharp.Core.Unit,Microsoft.FSharp.Core.FSharpOption`1[a]]]])
FSharp.Compiler.SourceCodeServices.AstTraversal: System.Tuple`2[b,Microsoft.FSharp.Core.FSharpFunc`2[Microsoft.FSharp.Core.Unit,c]] dive[a,b,c](a, b, Microsoft.FSharp.Core.FSharpFunc`2[a,c])
FSharp.Compiler.SourceCodeServices.BasicPatterns
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpExpr] |AddressOf|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpExpr] |Quote|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue] |Value|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpType] |BaseValue|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpType] |DefaultValue|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpType] |ThisValue|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Int32] |WitnessArg|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.SourceCodeServices.FSharpExpr,FSharp.Compiler.SourceCodeServices.FSharpExpr]] |AddressSet|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.SourceCodeServices.FSharpExpr,FSharp.Compiler.SourceCodeServices.FSharpExpr]] |Sequential|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.SourceCodeServices.FSharpExpr,FSharp.Compiler.SourceCodeServices.FSharpExpr]] |TryFinally|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.SourceCodeServices.FSharpExpr,FSharp.Compiler.SourceCodeServices.FSharpExpr]] |WhileLoop|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.SourceCodeServices.FSharpExpr,FSharp.Compiler.SourceCodeServices.FSharpType]] |UnionCaseTag|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.SourceCodeServices.FSharpExpr,Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue],FSharp.Compiler.SourceCodeServices.FSharpExpr]]]] |DecisionTree|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue,FSharp.Compiler.SourceCodeServices.FSharpExpr]] |Lambda|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue,FSharp.Compiler.SourceCodeServices.FSharpExpr]] |ValueSet|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.SourceCodeServices.FSharpType,FSharp.Compiler.SourceCodeServices.FSharpExpr]] |Coerce|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.SourceCodeServices.FSharpType,FSharp.Compiler.SourceCodeServices.FSharpExpr]] |NewDelegate|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.SourceCodeServices.FSharpType,FSharp.Compiler.SourceCodeServices.FSharpExpr]] |TypeTest|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.SourceCodeServices.FSharpType,Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpExpr]]] |NewAnonRecord|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.SourceCodeServices.FSharpType,Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpExpr]]] |NewArray|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.SourceCodeServices.FSharpType,Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpExpr]]] |NewRecord|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.SourceCodeServices.FSharpType,Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpExpr]]] |NewTuple|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpGenericParameter],FSharp.Compiler.SourceCodeServices.FSharpExpr]] |TypeLambda|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue,FSharp.Compiler.SourceCodeServices.FSharpExpr]],FSharp.Compiler.SourceCodeServices.FSharpExpr]] |LetRec|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[System.Int32,Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpExpr]]] |DecisionTreeSuccess|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[System.Object,FSharp.Compiler.SourceCodeServices.FSharpType]] |Const|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[System.Tuple`2[FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue,FSharp.Compiler.SourceCodeServices.FSharpExpr],FSharp.Compiler.SourceCodeServices.FSharpExpr]] |Let|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`3[FSharp.Compiler.SourceCodeServices.FSharpExpr,FSharp.Compiler.SourceCodeServices.FSharpExpr,FSharp.Compiler.SourceCodeServices.FSharpExpr]] |IfThenElse|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`3[FSharp.Compiler.SourceCodeServices.FSharpExpr,FSharp.Compiler.SourceCodeServices.FSharpType,FSharp.Compiler.SourceCodeServices.FSharpUnionCase]] |UnionCaseTest|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`3[FSharp.Compiler.SourceCodeServices.FSharpExpr,FSharp.Compiler.SourceCodeServices.FSharpType,System.Int32]] |AnonRecordGet|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`3[FSharp.Compiler.SourceCodeServices.FSharpExpr,Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpType],Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpExpr]]] |Application|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`3[FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue,Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpType],Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpExpr]]] |NewObject|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`3[FSharp.Compiler.SourceCodeServices.FSharpType,FSharp.Compiler.SourceCodeServices.FSharpUnionCase,Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpExpr]]] |NewUnionCase|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`3[FSharp.Compiler.SourceCodeServices.FSharpType,System.Int32,FSharp.Compiler.SourceCodeServices.FSharpExpr]] |TupleGet|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`3[Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpExpr],FSharp.Compiler.SourceCodeServices.FSharpType,FSharp.Compiler.SourceCodeServices.FSharpField]] |FSharpFieldGet|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`3[Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpExpr],FSharp.Compiler.SourceCodeServices.FSharpType,System.String]] |ILFieldGet|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`3[System.String,Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpType],Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpExpr]]] |ILAsm|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`4[FSharp.Compiler.SourceCodeServices.FSharpExpr,FSharp.Compiler.SourceCodeServices.FSharpExpr,FSharp.Compiler.SourceCodeServices.FSharpExpr,System.Boolean]] |FastIntegerForLoop|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`4[FSharp.Compiler.SourceCodeServices.FSharpExpr,FSharp.Compiler.SourceCodeServices.FSharpType,FSharp.Compiler.SourceCodeServices.FSharpUnionCase,FSharp.Compiler.SourceCodeServices.FSharpField]] |UnionCaseGet|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`4[FSharp.Compiler.SourceCodeServices.FSharpType,FSharp.Compiler.SourceCodeServices.FSharpExpr,Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpObjectExprOverride],Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[FSharp.Compiler.SourceCodeServices.FSharpType,Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpObjectExprOverride]]]]] |ObjectExpr|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`4[Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpExpr],FSharp.Compiler.SourceCodeServices.FSharpType,FSharp.Compiler.SourceCodeServices.FSharpField,FSharp.Compiler.SourceCodeServices.FSharpExpr]] |FSharpFieldSet|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`4[Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpExpr],FSharp.Compiler.SourceCodeServices.FSharpType,System.String,FSharp.Compiler.SourceCodeServices.FSharpExpr]] |ILFieldSet|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`5[FSharp.Compiler.SourceCodeServices.FSharpExpr,FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue,FSharp.Compiler.SourceCodeServices.FSharpExpr,FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue,FSharp.Compiler.SourceCodeServices.FSharpExpr]] |TryWith|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`5[FSharp.Compiler.SourceCodeServices.FSharpExpr,FSharp.Compiler.SourceCodeServices.FSharpType,FSharp.Compiler.SourceCodeServices.FSharpUnionCase,FSharp.Compiler.SourceCodeServices.FSharpField,FSharp.Compiler.SourceCodeServices.FSharpExpr]] |UnionCaseSet|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`5[Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpExpr],FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue,Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpType],Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpType],Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpExpr]]] |Call|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`6[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpType],System.String,FSharp.Compiler.SyntaxTree+MemberFlags,Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpType],Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpType],Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpExpr]]] |TraitCall|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.BasicPatterns: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`6[Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpExpr],FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue,Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpType],Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpType],Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpExpr],Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpExpr]]] |CallWithWitnesses|_|(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.CompilerDiagnostics
FSharp.Compiler.SourceCodeServices.CompilerDiagnostics: System.String getErrorMessage(FSharp.Compiler.SourceCodeServices.DiagnosticKind)
FSharp.Compiler.SourceCodeServices.CompilerEnvironment
FSharp.Compiler.SourceCodeServices.CompilerEnvironment: Microsoft.FSharp.Core.FSharpOption`1[System.String] BinFolderOfDefaultFSharpCompiler(Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.SourceCodeServices.CompilerEnvironmentModule
FSharp.Compiler.SourceCodeServices.CompilerEnvironmentModule: Boolean IsCheckerSupportedSubcategory(System.String)
FSharp.Compiler.SourceCodeServices.CompilerEnvironmentModule: Microsoft.FSharp.Collections.FSharpList`1[System.String] DefaultReferencesForOrphanSources(Boolean)
FSharp.Compiler.SourceCodeServices.CompilerEnvironmentModule: Microsoft.FSharp.Collections.FSharpList`1[System.String] GetCompilationDefinesForEditing(FSharp.Compiler.SourceCodeServices.FSharpParsingOptions)
FSharp.Compiler.SourceCodeServices.CompletionContext
FSharp.Compiler.SourceCodeServices.CompletionContext+Inherit: FSharp.Compiler.SourceCodeServices.InheritanceContext context
FSharp.Compiler.SourceCodeServices.CompletionContext+Inherit: FSharp.Compiler.SourceCodeServices.InheritanceContext get_context()
FSharp.Compiler.SourceCodeServices.CompletionContext+Inherit: System.Tuple`2[Microsoft.FSharp.Collections.FSharpList`1[System.String],Microsoft.FSharp.Core.FSharpOption`1[System.String]] get_path()
FSharp.Compiler.SourceCodeServices.CompletionContext+Inherit: System.Tuple`2[Microsoft.FSharp.Collections.FSharpList`1[System.String],Microsoft.FSharp.Core.FSharpOption`1[System.String]] path
FSharp.Compiler.SourceCodeServices.CompletionContext+OpenDeclaration: Boolean get_isOpenType()
FSharp.Compiler.SourceCodeServices.CompletionContext+OpenDeclaration: Boolean isOpenType
FSharp.Compiler.SourceCodeServices.CompletionContext+ParameterList: FSharp.Compiler.Text.Pos Item1
FSharp.Compiler.SourceCodeServices.CompletionContext+ParameterList: FSharp.Compiler.Text.Pos get_Item1()
FSharp.Compiler.SourceCodeServices.CompletionContext+ParameterList: System.Collections.Generic.HashSet`1[System.String] Item2
FSharp.Compiler.SourceCodeServices.CompletionContext+ParameterList: System.Collections.Generic.HashSet`1[System.String] get_Item2()
FSharp.Compiler.SourceCodeServices.CompletionContext+RecordField: FSharp.Compiler.SourceCodeServices.RecordContext context
FSharp.Compiler.SourceCodeServices.CompletionContext+RecordField: FSharp.Compiler.SourceCodeServices.RecordContext get_context()
FSharp.Compiler.SourceCodeServices.CompletionContext+Tags: Int32 AttributeApplication
FSharp.Compiler.SourceCodeServices.CompletionContext+Tags: Int32 Inherit
FSharp.Compiler.SourceCodeServices.CompletionContext+Tags: Int32 Invalid
FSharp.Compiler.SourceCodeServices.CompletionContext+Tags: Int32 OpenDeclaration
FSharp.Compiler.SourceCodeServices.CompletionContext+Tags: Int32 ParameterList
FSharp.Compiler.SourceCodeServices.CompletionContext+Tags: Int32 PatternType
FSharp.Compiler.SourceCodeServices.CompletionContext+Tags: Int32 RangeOperator
FSharp.Compiler.SourceCodeServices.CompletionContext+Tags: Int32 RecordField
FSharp.Compiler.SourceCodeServices.CompletionContext: Boolean Equals(FSharp.Compiler.SourceCodeServices.CompletionContext)
FSharp.Compiler.SourceCodeServices.CompletionContext: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.CompletionContext: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.CompletionContext: Boolean IsAttributeApplication
FSharp.Compiler.SourceCodeServices.CompletionContext: Boolean IsInherit
FSharp.Compiler.SourceCodeServices.CompletionContext: Boolean IsInvalid
FSharp.Compiler.SourceCodeServices.CompletionContext: Boolean IsOpenDeclaration
FSharp.Compiler.SourceCodeServices.CompletionContext: Boolean IsParameterList
FSharp.Compiler.SourceCodeServices.CompletionContext: Boolean IsPatternType
FSharp.Compiler.SourceCodeServices.CompletionContext: Boolean IsRangeOperator
FSharp.Compiler.SourceCodeServices.CompletionContext: Boolean IsRecordField
FSharp.Compiler.SourceCodeServices.CompletionContext: Boolean get_IsAttributeApplication()
FSharp.Compiler.SourceCodeServices.CompletionContext: Boolean get_IsInherit()
FSharp.Compiler.SourceCodeServices.CompletionContext: Boolean get_IsInvalid()
FSharp.Compiler.SourceCodeServices.CompletionContext: Boolean get_IsOpenDeclaration()
FSharp.Compiler.SourceCodeServices.CompletionContext: Boolean get_IsParameterList()
FSharp.Compiler.SourceCodeServices.CompletionContext: Boolean get_IsPatternType()
FSharp.Compiler.SourceCodeServices.CompletionContext: Boolean get_IsRangeOperator()
FSharp.Compiler.SourceCodeServices.CompletionContext: Boolean get_IsRecordField()
FSharp.Compiler.SourceCodeServices.CompletionContext: FSharp.Compiler.SourceCodeServices.CompletionContext AttributeApplication
FSharp.Compiler.SourceCodeServices.CompletionContext: FSharp.Compiler.SourceCodeServices.CompletionContext Invalid
FSharp.Compiler.SourceCodeServices.CompletionContext: FSharp.Compiler.SourceCodeServices.CompletionContext NewInherit(FSharp.Compiler.SourceCodeServices.InheritanceContext, System.Tuple`2[Microsoft.FSharp.Collections.FSharpList`1[System.String],Microsoft.FSharp.Core.FSharpOption`1[System.String]])
FSharp.Compiler.SourceCodeServices.CompletionContext: FSharp.Compiler.SourceCodeServices.CompletionContext NewOpenDeclaration(Boolean)
FSharp.Compiler.SourceCodeServices.CompletionContext: FSharp.Compiler.SourceCodeServices.CompletionContext NewParameterList(FSharp.Compiler.Text.Pos, System.Collections.Generic.HashSet`1[System.String])
FSharp.Compiler.SourceCodeServices.CompletionContext: FSharp.Compiler.SourceCodeServices.CompletionContext NewRecordField(FSharp.Compiler.SourceCodeServices.RecordContext)
FSharp.Compiler.SourceCodeServices.CompletionContext: FSharp.Compiler.SourceCodeServices.CompletionContext PatternType
FSharp.Compiler.SourceCodeServices.CompletionContext: FSharp.Compiler.SourceCodeServices.CompletionContext RangeOperator
FSharp.Compiler.SourceCodeServices.CompletionContext: FSharp.Compiler.SourceCodeServices.CompletionContext get_AttributeApplication()
FSharp.Compiler.SourceCodeServices.CompletionContext: FSharp.Compiler.SourceCodeServices.CompletionContext get_Invalid()
FSharp.Compiler.SourceCodeServices.CompletionContext: FSharp.Compiler.SourceCodeServices.CompletionContext get_PatternType()
FSharp.Compiler.SourceCodeServices.CompletionContext: FSharp.Compiler.SourceCodeServices.CompletionContext get_RangeOperator()
FSharp.Compiler.SourceCodeServices.CompletionContext: FSharp.Compiler.SourceCodeServices.CompletionContext+Inherit
FSharp.Compiler.SourceCodeServices.CompletionContext: FSharp.Compiler.SourceCodeServices.CompletionContext+OpenDeclaration
FSharp.Compiler.SourceCodeServices.CompletionContext: FSharp.Compiler.SourceCodeServices.CompletionContext+ParameterList
FSharp.Compiler.SourceCodeServices.CompletionContext: FSharp.Compiler.SourceCodeServices.CompletionContext+RecordField
FSharp.Compiler.SourceCodeServices.CompletionContext: FSharp.Compiler.SourceCodeServices.CompletionContext+Tags
FSharp.Compiler.SourceCodeServices.CompletionContext: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.CompletionContext: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.CompletionContext: Int32 Tag
FSharp.Compiler.SourceCodeServices.CompletionContext: Int32 get_Tag()
FSharp.Compiler.SourceCodeServices.CompletionContext: System.String ToString()
FSharp.Compiler.SourceCodeServices.DebuggerEnvironment
FSharp.Compiler.SourceCodeServices.DebuggerEnvironment: System.Guid GetLanguageID()
FSharp.Compiler.SourceCodeServices.DefaultFileSystem
FSharp.Compiler.SourceCodeServices.DefaultFileSystem: Void .ctor()
FSharp.Compiler.SourceCodeServices.DiagnosticKind
FSharp.Compiler.SourceCodeServices.DiagnosticKind+ReplaceWithSuggestion: System.String get_suggestion()
FSharp.Compiler.SourceCodeServices.DiagnosticKind+ReplaceWithSuggestion: System.String suggestion
FSharp.Compiler.SourceCodeServices.DiagnosticKind+Tags: Int32 AddIndexerDot
FSharp.Compiler.SourceCodeServices.DiagnosticKind+Tags: Int32 ReplaceWithSuggestion
FSharp.Compiler.SourceCodeServices.DiagnosticKind: Boolean Equals(FSharp.Compiler.SourceCodeServices.DiagnosticKind)
FSharp.Compiler.SourceCodeServices.DiagnosticKind: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.DiagnosticKind: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.DiagnosticKind: Boolean IsAddIndexerDot
FSharp.Compiler.SourceCodeServices.DiagnosticKind: Boolean IsReplaceWithSuggestion
FSharp.Compiler.SourceCodeServices.DiagnosticKind: Boolean get_IsAddIndexerDot()
FSharp.Compiler.SourceCodeServices.DiagnosticKind: Boolean get_IsReplaceWithSuggestion()
FSharp.Compiler.SourceCodeServices.DiagnosticKind: FSharp.Compiler.SourceCodeServices.DiagnosticKind AddIndexerDot
FSharp.Compiler.SourceCodeServices.DiagnosticKind: FSharp.Compiler.SourceCodeServices.DiagnosticKind NewReplaceWithSuggestion(System.String)
FSharp.Compiler.SourceCodeServices.DiagnosticKind: FSharp.Compiler.SourceCodeServices.DiagnosticKind get_AddIndexerDot()
FSharp.Compiler.SourceCodeServices.DiagnosticKind: FSharp.Compiler.SourceCodeServices.DiagnosticKind+ReplaceWithSuggestion
FSharp.Compiler.SourceCodeServices.DiagnosticKind: FSharp.Compiler.SourceCodeServices.DiagnosticKind+Tags
FSharp.Compiler.SourceCodeServices.DiagnosticKind: Int32 CompareTo(FSharp.Compiler.SourceCodeServices.DiagnosticKind)
FSharp.Compiler.SourceCodeServices.DiagnosticKind: Int32 CompareTo(System.Object)
FSharp.Compiler.SourceCodeServices.DiagnosticKind: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.SourceCodeServices.DiagnosticKind: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.DiagnosticKind: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.DiagnosticKind: Int32 Tag
FSharp.Compiler.SourceCodeServices.DiagnosticKind: Int32 get_Tag()
FSharp.Compiler.SourceCodeServices.DiagnosticKind: System.String ToString()
FSharp.Compiler.SourceCodeServices.Entity
FSharp.Compiler.SourceCodeServices.Entity: Boolean Equals(FSharp.Compiler.SourceCodeServices.Entity)
FSharp.Compiler.SourceCodeServices.Entity: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.Entity: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.Entity: Int32 CompareTo(FSharp.Compiler.SourceCodeServices.Entity)
FSharp.Compiler.SourceCodeServices.Entity: Int32 CompareTo(System.Object)
FSharp.Compiler.SourceCodeServices.Entity: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.SourceCodeServices.Entity: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.Entity: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.Entity: Microsoft.FSharp.Core.FSharpOption`1[System.String] Namespace
FSharp.Compiler.SourceCodeServices.Entity: Microsoft.FSharp.Core.FSharpOption`1[System.String] get_Namespace()
FSharp.Compiler.SourceCodeServices.Entity: System.String FullRelativeName
FSharp.Compiler.SourceCodeServices.Entity: System.String LastIdent
FSharp.Compiler.SourceCodeServices.Entity: System.String Name
FSharp.Compiler.SourceCodeServices.Entity: System.String Qualifier
FSharp.Compiler.SourceCodeServices.Entity: System.String ToString()
FSharp.Compiler.SourceCodeServices.Entity: System.String get_FullRelativeName()
FSharp.Compiler.SourceCodeServices.Entity: System.String get_LastIdent()
FSharp.Compiler.SourceCodeServices.Entity: System.String get_Name()
FSharp.Compiler.SourceCodeServices.Entity: System.String get_Qualifier()
FSharp.Compiler.SourceCodeServices.Entity: Void .ctor(System.String, System.String, Microsoft.FSharp.Core.FSharpOption`1[System.String], System.String, System.String)
FSharp.Compiler.SourceCodeServices.EntityCache
FSharp.Compiler.SourceCodeServices.EntityCache: T Locking[T](Microsoft.FSharp.Core.FSharpFunc`2[FSharp.Compiler.SourceCodeServices.IAssemblyContentCache,T])
FSharp.Compiler.SourceCodeServices.EntityCache: Void .ctor()
FSharp.Compiler.SourceCodeServices.EntityCache: Void Clear()
FSharp.Compiler.SourceCodeServices.EntityKind
FSharp.Compiler.SourceCodeServices.EntityKind+FunctionOrValue: Boolean get_isActivePattern()
FSharp.Compiler.SourceCodeServices.EntityKind+FunctionOrValue: Boolean isActivePattern
FSharp.Compiler.SourceCodeServices.EntityKind+Module: FSharp.Compiler.SourceCodeServices.ModuleKind Item
FSharp.Compiler.SourceCodeServices.EntityKind+Module: FSharp.Compiler.SourceCodeServices.ModuleKind get_Item()
FSharp.Compiler.SourceCodeServices.EntityKind+Tags: Int32 Attribute
FSharp.Compiler.SourceCodeServices.EntityKind+Tags: Int32 FunctionOrValue
FSharp.Compiler.SourceCodeServices.EntityKind+Tags: Int32 Module
FSharp.Compiler.SourceCodeServices.EntityKind+Tags: Int32 Type
FSharp.Compiler.SourceCodeServices.EntityKind: Boolean Equals(FSharp.Compiler.SourceCodeServices.EntityKind)
FSharp.Compiler.SourceCodeServices.EntityKind: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.EntityKind: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.EntityKind: Boolean IsAttribute
FSharp.Compiler.SourceCodeServices.EntityKind: Boolean IsFunctionOrValue
FSharp.Compiler.SourceCodeServices.EntityKind: Boolean IsModule
FSharp.Compiler.SourceCodeServices.EntityKind: Boolean IsType
FSharp.Compiler.SourceCodeServices.EntityKind: Boolean get_IsAttribute()
FSharp.Compiler.SourceCodeServices.EntityKind: Boolean get_IsFunctionOrValue()
FSharp.Compiler.SourceCodeServices.EntityKind: Boolean get_IsModule()
FSharp.Compiler.SourceCodeServices.EntityKind: Boolean get_IsType()
FSharp.Compiler.SourceCodeServices.EntityKind: FSharp.Compiler.SourceCodeServices.EntityKind Attribute
FSharp.Compiler.SourceCodeServices.EntityKind: FSharp.Compiler.SourceCodeServices.EntityKind NewFunctionOrValue(Boolean)
FSharp.Compiler.SourceCodeServices.EntityKind: FSharp.Compiler.SourceCodeServices.EntityKind NewModule(FSharp.Compiler.SourceCodeServices.ModuleKind)
FSharp.Compiler.SourceCodeServices.EntityKind: FSharp.Compiler.SourceCodeServices.EntityKind Type
FSharp.Compiler.SourceCodeServices.EntityKind: FSharp.Compiler.SourceCodeServices.EntityKind get_Attribute()
FSharp.Compiler.SourceCodeServices.EntityKind: FSharp.Compiler.SourceCodeServices.EntityKind get_Type()
FSharp.Compiler.SourceCodeServices.EntityKind: FSharp.Compiler.SourceCodeServices.EntityKind+FunctionOrValue
FSharp.Compiler.SourceCodeServices.EntityKind: FSharp.Compiler.SourceCodeServices.EntityKind+Module
FSharp.Compiler.SourceCodeServices.EntityKind: FSharp.Compiler.SourceCodeServices.EntityKind+Tags
FSharp.Compiler.SourceCodeServices.EntityKind: Int32 CompareTo(FSharp.Compiler.SourceCodeServices.EntityKind)
FSharp.Compiler.SourceCodeServices.EntityKind: Int32 CompareTo(System.Object)
FSharp.Compiler.SourceCodeServices.EntityKind: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.SourceCodeServices.EntityKind: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.EntityKind: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.EntityKind: Int32 Tag
FSharp.Compiler.SourceCodeServices.EntityKind: Int32 get_Tag()
FSharp.Compiler.SourceCodeServices.EntityKind: System.String ToString()
FSharp.Compiler.SourceCodeServices.ErrorResolutionHints
FSharp.Compiler.SourceCodeServices.ErrorResolutionHints: System.Collections.Generic.IEnumerable`1[System.String] getSuggestedNames(Microsoft.FSharp.Core.FSharpFunc`2[Microsoft.FSharp.Core.FSharpFunc`2[System.String,Microsoft.FSharp.Core.Unit],Microsoft.FSharp.Core.Unit], System.String)
FSharp.Compiler.SourceCodeServices.ExternalType
FSharp.Compiler.SourceCodeServices.ExternalType+Array: FSharp.Compiler.SourceCodeServices.ExternalType get_inner()
FSharp.Compiler.SourceCodeServices.ExternalType+Array: FSharp.Compiler.SourceCodeServices.ExternalType inner
FSharp.Compiler.SourceCodeServices.ExternalType+Pointer: FSharp.Compiler.SourceCodeServices.ExternalType get_inner()
FSharp.Compiler.SourceCodeServices.ExternalType+Pointer: FSharp.Compiler.SourceCodeServices.ExternalType inner
FSharp.Compiler.SourceCodeServices.ExternalType+Tags: Int32 Array
FSharp.Compiler.SourceCodeServices.ExternalType+Tags: Int32 Pointer
FSharp.Compiler.SourceCodeServices.ExternalType+Tags: Int32 Type
FSharp.Compiler.SourceCodeServices.ExternalType+Tags: Int32 TypeVar
FSharp.Compiler.SourceCodeServices.ExternalType+Type: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.ExternalType] genericArgs
FSharp.Compiler.SourceCodeServices.ExternalType+Type: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.ExternalType] get_genericArgs()
FSharp.Compiler.SourceCodeServices.ExternalType+Type: System.String fullName
FSharp.Compiler.SourceCodeServices.ExternalType+Type: System.String get_fullName()
FSharp.Compiler.SourceCodeServices.ExternalType+TypeVar: System.String get_typeName()
FSharp.Compiler.SourceCodeServices.ExternalType+TypeVar: System.String typeName
FSharp.Compiler.SourceCodeServices.ExternalType: Boolean Equals(FSharp.Compiler.SourceCodeServices.ExternalType)
FSharp.Compiler.SourceCodeServices.ExternalType: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.ExternalType: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.ExternalType: Boolean IsArray
FSharp.Compiler.SourceCodeServices.ExternalType: Boolean IsPointer
FSharp.Compiler.SourceCodeServices.ExternalType: Boolean IsType
FSharp.Compiler.SourceCodeServices.ExternalType: Boolean IsTypeVar
FSharp.Compiler.SourceCodeServices.ExternalType: Boolean get_IsArray()
FSharp.Compiler.SourceCodeServices.ExternalType: Boolean get_IsPointer()
FSharp.Compiler.SourceCodeServices.ExternalType: Boolean get_IsType()
FSharp.Compiler.SourceCodeServices.ExternalType: Boolean get_IsTypeVar()
FSharp.Compiler.SourceCodeServices.ExternalType: FSharp.Compiler.SourceCodeServices.ExternalType NewArray(FSharp.Compiler.SourceCodeServices.ExternalType)
FSharp.Compiler.SourceCodeServices.ExternalType: FSharp.Compiler.SourceCodeServices.ExternalType NewPointer(FSharp.Compiler.SourceCodeServices.ExternalType)
FSharp.Compiler.SourceCodeServices.ExternalType: FSharp.Compiler.SourceCodeServices.ExternalType NewType(System.String, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.ExternalType])
FSharp.Compiler.SourceCodeServices.ExternalType: FSharp.Compiler.SourceCodeServices.ExternalType NewTypeVar(System.String)
FSharp.Compiler.SourceCodeServices.ExternalType: FSharp.Compiler.SourceCodeServices.ExternalType+Array
FSharp.Compiler.SourceCodeServices.ExternalType: FSharp.Compiler.SourceCodeServices.ExternalType+Pointer
FSharp.Compiler.SourceCodeServices.ExternalType: FSharp.Compiler.SourceCodeServices.ExternalType+Tags
FSharp.Compiler.SourceCodeServices.ExternalType: FSharp.Compiler.SourceCodeServices.ExternalType+Type
FSharp.Compiler.SourceCodeServices.ExternalType: FSharp.Compiler.SourceCodeServices.ExternalType+TypeVar
FSharp.Compiler.SourceCodeServices.ExternalType: Int32 CompareTo(FSharp.Compiler.SourceCodeServices.ExternalType)
FSharp.Compiler.SourceCodeServices.ExternalType: Int32 CompareTo(System.Object)
FSharp.Compiler.SourceCodeServices.ExternalType: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.SourceCodeServices.ExternalType: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.ExternalType: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.ExternalType: Int32 Tag
FSharp.Compiler.SourceCodeServices.ExternalType: Int32 get_Tag()
FSharp.Compiler.SourceCodeServices.ExternalType: System.String ToString()
FSharp.Compiler.SourceCodeServices.ExternalTypeModule
FSharp.Compiler.SourceCodeServices.FSharpAbstractParameter
FSharp.Compiler.SourceCodeServices.FSharpAbstractParameter: Boolean IsInArg
FSharp.Compiler.SourceCodeServices.FSharpAbstractParameter: Boolean IsOptionalArg
FSharp.Compiler.SourceCodeServices.FSharpAbstractParameter: Boolean IsOutArg
FSharp.Compiler.SourceCodeServices.FSharpAbstractParameter: Boolean get_IsInArg()
FSharp.Compiler.SourceCodeServices.FSharpAbstractParameter: Boolean get_IsOptionalArg()
FSharp.Compiler.SourceCodeServices.FSharpAbstractParameter: Boolean get_IsOutArg()
FSharp.Compiler.SourceCodeServices.FSharpAbstractParameter: FSharp.Compiler.SourceCodeServices.FSharpType Type
FSharp.Compiler.SourceCodeServices.FSharpAbstractParameter: FSharp.Compiler.SourceCodeServices.FSharpType get_Type()
FSharp.Compiler.SourceCodeServices.FSharpAbstractParameter: Microsoft.FSharp.Core.FSharpOption`1[System.String] Name
FSharp.Compiler.SourceCodeServices.FSharpAbstractParameter: Microsoft.FSharp.Core.FSharpOption`1[System.String] get_Name()
FSharp.Compiler.SourceCodeServices.FSharpAbstractParameter: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpAttribute] Attributes
FSharp.Compiler.SourceCodeServices.FSharpAbstractParameter: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpAttribute] get_Attributes()
FSharp.Compiler.SourceCodeServices.FSharpAbstractSignature
FSharp.Compiler.SourceCodeServices.FSharpAbstractSignature: FSharp.Compiler.SourceCodeServices.FSharpType AbstractReturnType
FSharp.Compiler.SourceCodeServices.FSharpAbstractSignature: FSharp.Compiler.SourceCodeServices.FSharpType DeclaringType
FSharp.Compiler.SourceCodeServices.FSharpAbstractSignature: FSharp.Compiler.SourceCodeServices.FSharpType get_AbstractReturnType()
FSharp.Compiler.SourceCodeServices.FSharpAbstractSignature: FSharp.Compiler.SourceCodeServices.FSharpType get_DeclaringType()
FSharp.Compiler.SourceCodeServices.FSharpAbstractSignature: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpGenericParameter] DeclaringTypeGenericParameters
FSharp.Compiler.SourceCodeServices.FSharpAbstractSignature: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpGenericParameter] MethodGenericParameters
FSharp.Compiler.SourceCodeServices.FSharpAbstractSignature: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpGenericParameter] get_DeclaringTypeGenericParameters()
FSharp.Compiler.SourceCodeServices.FSharpAbstractSignature: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpGenericParameter] get_MethodGenericParameters()
FSharp.Compiler.SourceCodeServices.FSharpAbstractSignature: System.Collections.Generic.IList`1[System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpAbstractParameter]] AbstractArguments
FSharp.Compiler.SourceCodeServices.FSharpAbstractSignature: System.Collections.Generic.IList`1[System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpAbstractParameter]] get_AbstractArguments()
FSharp.Compiler.SourceCodeServices.FSharpAbstractSignature: System.String Name
FSharp.Compiler.SourceCodeServices.FSharpAbstractSignature: System.String get_Name()
FSharp.Compiler.SourceCodeServices.FSharpAccessibility
FSharp.Compiler.SourceCodeServices.FSharpAccessibility: Boolean IsInternal
FSharp.Compiler.SourceCodeServices.FSharpAccessibility: Boolean IsPrivate
FSharp.Compiler.SourceCodeServices.FSharpAccessibility: Boolean IsProtected
FSharp.Compiler.SourceCodeServices.FSharpAccessibility: Boolean IsPublic
FSharp.Compiler.SourceCodeServices.FSharpAccessibility: Boolean get_IsInternal()
FSharp.Compiler.SourceCodeServices.FSharpAccessibility: Boolean get_IsPrivate()
FSharp.Compiler.SourceCodeServices.FSharpAccessibility: Boolean get_IsProtected()
FSharp.Compiler.SourceCodeServices.FSharpAccessibility: Boolean get_IsPublic()
FSharp.Compiler.SourceCodeServices.FSharpAccessibility: System.String ToString()
FSharp.Compiler.SourceCodeServices.FSharpAccessibilityRights
FSharp.Compiler.SourceCodeServices.FSharpActivePatternCase
FSharp.Compiler.SourceCodeServices.FSharpActivePatternCase: FSharp.Compiler.SourceCodeServices.FSharpActivePatternGroup Group
FSharp.Compiler.SourceCodeServices.FSharpActivePatternCase: FSharp.Compiler.SourceCodeServices.FSharpActivePatternGroup get_Group()
FSharp.Compiler.SourceCodeServices.FSharpActivePatternCase: FSharp.Compiler.Text.Range DeclarationLocation
FSharp.Compiler.SourceCodeServices.FSharpActivePatternCase: FSharp.Compiler.Text.Range get_DeclarationLocation()
FSharp.Compiler.SourceCodeServices.FSharpActivePatternCase: Int32 Index
FSharp.Compiler.SourceCodeServices.FSharpActivePatternCase: Int32 get_Index()
FSharp.Compiler.SourceCodeServices.FSharpActivePatternCase: System.Collections.Generic.IList`1[System.String] ElaboratedXmlDoc
FSharp.Compiler.SourceCodeServices.FSharpActivePatternCase: System.Collections.Generic.IList`1[System.String] XmlDoc
FSharp.Compiler.SourceCodeServices.FSharpActivePatternCase: System.Collections.Generic.IList`1[System.String] get_ElaboratedXmlDoc()
FSharp.Compiler.SourceCodeServices.FSharpActivePatternCase: System.Collections.Generic.IList`1[System.String] get_XmlDoc()
FSharp.Compiler.SourceCodeServices.FSharpActivePatternCase: System.String Name
FSharp.Compiler.SourceCodeServices.FSharpActivePatternCase: System.String XmlDocSig
FSharp.Compiler.SourceCodeServices.FSharpActivePatternCase: System.String get_Name()
FSharp.Compiler.SourceCodeServices.FSharpActivePatternCase: System.String get_XmlDocSig()
FSharp.Compiler.SourceCodeServices.FSharpActivePatternGroup
FSharp.Compiler.SourceCodeServices.FSharpActivePatternGroup: Boolean IsTotal
FSharp.Compiler.SourceCodeServices.FSharpActivePatternGroup: Boolean get_IsTotal()
FSharp.Compiler.SourceCodeServices.FSharpActivePatternGroup: FSharp.Compiler.SourceCodeServices.FSharpType OverallType
FSharp.Compiler.SourceCodeServices.FSharpActivePatternGroup: FSharp.Compiler.SourceCodeServices.FSharpType get_OverallType()
FSharp.Compiler.SourceCodeServices.FSharpActivePatternGroup: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpEntity] DeclaringEntity
FSharp.Compiler.SourceCodeServices.FSharpActivePatternGroup: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpEntity] get_DeclaringEntity()
FSharp.Compiler.SourceCodeServices.FSharpActivePatternGroup: Microsoft.FSharp.Core.FSharpOption`1[System.String] Name
FSharp.Compiler.SourceCodeServices.FSharpActivePatternGroup: Microsoft.FSharp.Core.FSharpOption`1[System.String] get_Name()
FSharp.Compiler.SourceCodeServices.FSharpActivePatternGroup: System.Collections.Generic.IList`1[System.String] Names
FSharp.Compiler.SourceCodeServices.FSharpActivePatternGroup: System.Collections.Generic.IList`1[System.String] get_Names()
FSharp.Compiler.SourceCodeServices.FSharpAnonRecordTypeDetails
FSharp.Compiler.SourceCodeServices.FSharpAnonRecordTypeDetails: FSharp.Compiler.SourceCodeServices.FSharpAssembly Assembly
FSharp.Compiler.SourceCodeServices.FSharpAnonRecordTypeDetails: FSharp.Compiler.SourceCodeServices.FSharpAssembly get_Assembly()
FSharp.Compiler.SourceCodeServices.FSharpAnonRecordTypeDetails: Microsoft.FSharp.Collections.FSharpList`1[System.String] EnclosingCompiledTypeNames
FSharp.Compiler.SourceCodeServices.FSharpAnonRecordTypeDetails: Microsoft.FSharp.Collections.FSharpList`1[System.String] get_EnclosingCompiledTypeNames()
FSharp.Compiler.SourceCodeServices.FSharpAnonRecordTypeDetails: System.String CompiledName
FSharp.Compiler.SourceCodeServices.FSharpAnonRecordTypeDetails: System.String get_CompiledName()
FSharp.Compiler.SourceCodeServices.FSharpAnonRecordTypeDetails: System.String[] SortedFieldNames
FSharp.Compiler.SourceCodeServices.FSharpAnonRecordTypeDetails: System.String[] get_SortedFieldNames()
FSharp.Compiler.SourceCodeServices.FSharpAssembly
FSharp.Compiler.SourceCodeServices.FSharpAssembly: Boolean IsProviderGenerated
FSharp.Compiler.SourceCodeServices.FSharpAssembly: Boolean get_IsProviderGenerated()
FSharp.Compiler.SourceCodeServices.FSharpAssembly: FSharp.Compiler.SourceCodeServices.FSharpAssemblySignature Contents
FSharp.Compiler.SourceCodeServices.FSharpAssembly: FSharp.Compiler.SourceCodeServices.FSharpAssemblySignature get_Contents()
FSharp.Compiler.SourceCodeServices.FSharpAssembly: Microsoft.FSharp.Core.FSharpOption`1[System.String] FileName
FSharp.Compiler.SourceCodeServices.FSharpAssembly: Microsoft.FSharp.Core.FSharpOption`1[System.String] get_FileName()
FSharp.Compiler.SourceCodeServices.FSharpAssembly: System.String CodeLocation
FSharp.Compiler.SourceCodeServices.FSharpAssembly: System.String QualifiedName
FSharp.Compiler.SourceCodeServices.FSharpAssembly: System.String SimpleName
FSharp.Compiler.SourceCodeServices.FSharpAssembly: System.String ToString()
FSharp.Compiler.SourceCodeServices.FSharpAssembly: System.String get_CodeLocation()
FSharp.Compiler.SourceCodeServices.FSharpAssembly: System.String get_QualifiedName()
FSharp.Compiler.SourceCodeServices.FSharpAssembly: System.String get_SimpleName()
FSharp.Compiler.SourceCodeServices.FSharpAssemblyContents
FSharp.Compiler.SourceCodeServices.FSharpAssemblyContents: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpImplementationFileContents] ImplementationFiles
FSharp.Compiler.SourceCodeServices.FSharpAssemblyContents: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpImplementationFileContents] get_ImplementationFiles()
FSharp.Compiler.SourceCodeServices.FSharpAssemblySignature
FSharp.Compiler.SourceCodeServices.FSharpAssemblySignature: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpEntity] FindEntityByPath(Microsoft.FSharp.Collections.FSharpList`1[System.String])
FSharp.Compiler.SourceCodeServices.FSharpAssemblySignature: System.Collections.Generic.IEnumerable`1[FSharp.Compiler.SourceCodeServices.FSharpEntity] TryGetEntities()
FSharp.Compiler.SourceCodeServices.FSharpAssemblySignature: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpAttribute] Attributes
FSharp.Compiler.SourceCodeServices.FSharpAssemblySignature: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpAttribute] get_Attributes()
FSharp.Compiler.SourceCodeServices.FSharpAssemblySignature: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpEntity] Entities
FSharp.Compiler.SourceCodeServices.FSharpAssemblySignature: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpEntity] get_Entities()
FSharp.Compiler.SourceCodeServices.FSharpAssemblySignature: System.String ToString()
FSharp.Compiler.SourceCodeServices.FSharpAttribute
FSharp.Compiler.SourceCodeServices.FSharpAttribute: Boolean IsUnresolved
FSharp.Compiler.SourceCodeServices.FSharpAttribute: Boolean get_IsUnresolved()
FSharp.Compiler.SourceCodeServices.FSharpAttribute: FSharp.Compiler.SourceCodeServices.FSharpEntity AttributeType
FSharp.Compiler.SourceCodeServices.FSharpAttribute: FSharp.Compiler.SourceCodeServices.FSharpEntity get_AttributeType()
FSharp.Compiler.SourceCodeServices.FSharpAttribute: FSharp.Compiler.Text.Range Range
FSharp.Compiler.SourceCodeServices.FSharpAttribute: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.SourceCodeServices.FSharpAttribute: System.Collections.Generic.IList`1[System.Tuple`2[FSharp.Compiler.SourceCodeServices.FSharpType,System.Object]] ConstructorArguments
FSharp.Compiler.SourceCodeServices.FSharpAttribute: System.Collections.Generic.IList`1[System.Tuple`2[FSharp.Compiler.SourceCodeServices.FSharpType,System.Object]] get_ConstructorArguments()
FSharp.Compiler.SourceCodeServices.FSharpAttribute: System.Collections.Generic.IList`1[System.Tuple`4[FSharp.Compiler.SourceCodeServices.FSharpType,System.String,System.Boolean,System.Object]] NamedArguments
FSharp.Compiler.SourceCodeServices.FSharpAttribute: System.Collections.Generic.IList`1[System.Tuple`4[FSharp.Compiler.SourceCodeServices.FSharpType,System.String,System.Boolean,System.Object]] get_NamedArguments()
FSharp.Compiler.SourceCodeServices.FSharpAttribute: System.String Format(FSharp.Compiler.SourceCodeServices.FSharpDisplayContext)
FSharp.Compiler.SourceCodeServices.FSharpAttribute: System.String ToString()
FSharp.Compiler.SourceCodeServices.FSharpCheckFileAnswer
FSharp.Compiler.SourceCodeServices.FSharpCheckFileAnswer+Succeeded: FSharp.Compiler.SourceCodeServices.FSharpCheckFileResults Item
FSharp.Compiler.SourceCodeServices.FSharpCheckFileAnswer+Succeeded: FSharp.Compiler.SourceCodeServices.FSharpCheckFileResults get_Item()
FSharp.Compiler.SourceCodeServices.FSharpCheckFileAnswer+Tags: Int32 Aborted
FSharp.Compiler.SourceCodeServices.FSharpCheckFileAnswer+Tags: Int32 Succeeded
FSharp.Compiler.SourceCodeServices.FSharpCheckFileAnswer: Boolean Equals(FSharp.Compiler.SourceCodeServices.FSharpCheckFileAnswer)
FSharp.Compiler.SourceCodeServices.FSharpCheckFileAnswer: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpCheckFileAnswer: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.FSharpCheckFileAnswer: Boolean IsAborted
FSharp.Compiler.SourceCodeServices.FSharpCheckFileAnswer: Boolean IsSucceeded
FSharp.Compiler.SourceCodeServices.FSharpCheckFileAnswer: Boolean get_IsAborted()
FSharp.Compiler.SourceCodeServices.FSharpCheckFileAnswer: Boolean get_IsSucceeded()
FSharp.Compiler.SourceCodeServices.FSharpCheckFileAnswer: FSharp.Compiler.SourceCodeServices.FSharpCheckFileAnswer Aborted
FSharp.Compiler.SourceCodeServices.FSharpCheckFileAnswer: FSharp.Compiler.SourceCodeServices.FSharpCheckFileAnswer NewSucceeded(FSharp.Compiler.SourceCodeServices.FSharpCheckFileResults)
FSharp.Compiler.SourceCodeServices.FSharpCheckFileAnswer: FSharp.Compiler.SourceCodeServices.FSharpCheckFileAnswer get_Aborted()
FSharp.Compiler.SourceCodeServices.FSharpCheckFileAnswer: FSharp.Compiler.SourceCodeServices.FSharpCheckFileAnswer+Succeeded
FSharp.Compiler.SourceCodeServices.FSharpCheckFileAnswer: FSharp.Compiler.SourceCodeServices.FSharpCheckFileAnswer+Tags
FSharp.Compiler.SourceCodeServices.FSharpCheckFileAnswer: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.FSharpCheckFileAnswer: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.FSharpCheckFileAnswer: Int32 Tag
FSharp.Compiler.SourceCodeServices.FSharpCheckFileAnswer: Int32 get_Tag()
FSharp.Compiler.SourceCodeServices.FSharpCheckFileAnswer: System.String ToString()
FSharp.Compiler.SourceCodeServices.FSharpCheckFileResults
FSharp.Compiler.SourceCodeServices.FSharpCheckFileResults: Boolean HasFullTypeCheckInfo
FSharp.Compiler.SourceCodeServices.FSharpCheckFileResults: Boolean IsRelativeNameResolvableFromSymbol(FSharp.Compiler.Text.Pos, Microsoft.FSharp.Collections.FSharpList`1[System.String], FSharp.Compiler.SourceCodeServices.FSharpSymbol)
FSharp.Compiler.SourceCodeServices.FSharpCheckFileResults: Boolean get_HasFullTypeCheckInfo()
FSharp.Compiler.SourceCodeServices.FSharpCheckFileResults: FSharp.Compiler.SourceCodeServices.FSharpAssemblySignature PartialAssemblySignature
FSharp.Compiler.SourceCodeServices.FSharpCheckFileResults: FSharp.Compiler.SourceCodeServices.FSharpAssemblySignature get_PartialAssemblySignature()
FSharp.Compiler.SourceCodeServices.FSharpCheckFileResults: FSharp.Compiler.SourceCodeServices.FSharpDeclarationListInfo GetDeclarationListInfo(Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpParseFileResults], Int32, System.String, FSharp.Compiler.SourceCodeServices.PartialLongName, Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.FSharpFunc`2[Microsoft.FSharp.Core.Unit,Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.AssemblySymbol]]])
FSharp.Compiler.SourceCodeServices.FSharpCheckFileResults: FSharp.Compiler.SourceCodeServices.FSharpDiagnostic[] Errors
FSharp.Compiler.SourceCodeServices.FSharpCheckFileResults: FSharp.Compiler.SourceCodeServices.FSharpDiagnostic[] get_Errors()
FSharp.Compiler.SourceCodeServices.FSharpCheckFileResults: FSharp.Compiler.SourceCodeServices.FSharpFindDeclResult GetDeclarationLocation(Int32, Int32, System.String, Microsoft.FSharp.Collections.FSharpList`1[System.String], Microsoft.FSharp.Core.FSharpOption`1[System.Boolean])
FSharp.Compiler.SourceCodeServices.FSharpCheckFileResults: FSharp.Compiler.SourceCodeServices.FSharpMethodGroup GetMethods(Int32, Int32, System.String, Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Collections.FSharpList`1[System.String]])
FSharp.Compiler.SourceCodeServices.FSharpCheckFileResults: FSharp.Compiler.SourceCodeServices.FSharpOpenDeclaration[] OpenDeclarations
FSharp.Compiler.SourceCodeServices.FSharpCheckFileResults: FSharp.Compiler.SourceCodeServices.FSharpOpenDeclaration[] get_OpenDeclarations()
FSharp.Compiler.SourceCodeServices.FSharpCheckFileResults: FSharp.Compiler.SourceCodeServices.FSharpProjectContext ProjectContext
FSharp.Compiler.SourceCodeServices.FSharpCheckFileResults: FSharp.Compiler.SourceCodeServices.FSharpProjectContext get_ProjectContext()
FSharp.Compiler.SourceCodeServices.FSharpCheckFileResults: FSharp.Compiler.SourceCodeServices.FSharpSymbolUse[] GetUsesOfSymbolInFile(FSharp.Compiler.SourceCodeServices.FSharpSymbol, Microsoft.FSharp.Core.FSharpOption`1[System.Threading.CancellationToken])
FSharp.Compiler.SourceCodeServices.FSharpCheckFileResults: FSharp.Compiler.SourceCodeServices.FSharpToolTipText`1[FSharp.Compiler.TextLayout.Layout] GetStructuredToolTipText(Int32, Int32, System.String, Microsoft.FSharp.Collections.FSharpList`1[System.String], Int32)
FSharp.Compiler.SourceCodeServices.FSharpCheckFileResults: FSharp.Compiler.SourceCodeServices.FSharpToolTipText`1[System.String] GetToolTipText(Int32, Int32, System.String, Microsoft.FSharp.Collections.FSharpList`1[System.String], Int32)
FSharp.Compiler.SourceCodeServices.FSharpCheckFileResults: FSharp.Compiler.Text.Range[] GetFormatSpecifierLocations()
FSharp.Compiler.SourceCodeServices.FSharpCheckFileResults: Microsoft.FSharp.Collections.FSharpList`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpSymbolUse]] GetDeclarationListSymbols(Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpParseFileResults], Int32, System.String, FSharp.Compiler.SourceCodeServices.PartialLongName, Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.FSharpFunc`2[Microsoft.FSharp.Core.Unit,Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.AssemblySymbol]]])
FSharp.Compiler.SourceCodeServices.FSharpCheckFileResults: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpDisplayContext] GetDisplayContextForPos(FSharp.Compiler.Text.Pos)
FSharp.Compiler.SourceCodeServices.FSharpCheckFileResults: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpImplementationFileContents] ImplementationFile
FSharp.Compiler.SourceCodeServices.FSharpCheckFileResults: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpImplementationFileContents] get_ImplementationFile()
FSharp.Compiler.SourceCodeServices.FSharpCheckFileResults: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpSymbolUse] GetSymbolUseAtLocation(Int32, Int32, System.String, Microsoft.FSharp.Collections.FSharpList`1[System.String])
FSharp.Compiler.SourceCodeServices.FSharpCheckFileResults: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpSymbolUse]] GetMethodsAsSymbols(Int32, Int32, System.String, Microsoft.FSharp.Collections.FSharpList`1[System.String])
FSharp.Compiler.SourceCodeServices.FSharpCheckFileResults: Microsoft.FSharp.Core.FSharpOption`1[System.String] GetF1Keyword(Int32, Int32, System.String, Microsoft.FSharp.Collections.FSharpList`1[System.String])
FSharp.Compiler.SourceCodeServices.FSharpCheckFileResults: System.Collections.Generic.IEnumerable`1[FSharp.Compiler.SourceCodeServices.FSharpSymbolUse] GetAllUsesOfAllSymbolsInFile(Microsoft.FSharp.Core.FSharpOption`1[System.Threading.CancellationToken])
FSharp.Compiler.SourceCodeServices.FSharpCheckFileResults: System.String ToString()
FSharp.Compiler.SourceCodeServices.FSharpCheckFileResults: System.String[] DependencyFiles
FSharp.Compiler.SourceCodeServices.FSharpCheckFileResults: System.String[] get_DependencyFiles()
FSharp.Compiler.SourceCodeServices.FSharpCheckFileResults: System.Tuple`2[FSharp.Compiler.Text.Range,System.Int32][] GetFormatSpecifierLocationsAndArity()
FSharp.Compiler.SourceCodeServices.FSharpCheckFileResults: System.ValueTuple`2[FSharp.Compiler.Text.Range,FSharp.Compiler.SourceCodeServices.SemanticClassificationType][] GetSemanticClassification(Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range])
FSharp.Compiler.SourceCodeServices.FSharpCheckProjectResults
FSharp.Compiler.SourceCodeServices.FSharpCheckProjectResults: Boolean HasCriticalErrors
FSharp.Compiler.SourceCodeServices.FSharpCheckProjectResults: Boolean get_HasCriticalErrors()
FSharp.Compiler.SourceCodeServices.FSharpCheckProjectResults: FSharp.Compiler.SourceCodeServices.FSharpAssemblyContents AssemblyContents
FSharp.Compiler.SourceCodeServices.FSharpCheckProjectResults: FSharp.Compiler.SourceCodeServices.FSharpAssemblyContents GetOptimizedAssemblyContents()
FSharp.Compiler.SourceCodeServices.FSharpCheckProjectResults: FSharp.Compiler.SourceCodeServices.FSharpAssemblyContents get_AssemblyContents()
FSharp.Compiler.SourceCodeServices.FSharpCheckProjectResults: FSharp.Compiler.SourceCodeServices.FSharpAssemblySignature AssemblySignature
FSharp.Compiler.SourceCodeServices.FSharpCheckProjectResults: FSharp.Compiler.SourceCodeServices.FSharpAssemblySignature get_AssemblySignature()
FSharp.Compiler.SourceCodeServices.FSharpCheckProjectResults: FSharp.Compiler.SourceCodeServices.FSharpDiagnostic[] Errors
FSharp.Compiler.SourceCodeServices.FSharpCheckProjectResults: FSharp.Compiler.SourceCodeServices.FSharpDiagnostic[] get_Errors()
FSharp.Compiler.SourceCodeServices.FSharpCheckProjectResults: FSharp.Compiler.SourceCodeServices.FSharpProjectContext ProjectContext
FSharp.Compiler.SourceCodeServices.FSharpCheckProjectResults: FSharp.Compiler.SourceCodeServices.FSharpProjectContext get_ProjectContext()
FSharp.Compiler.SourceCodeServices.FSharpCheckProjectResults: FSharp.Compiler.SourceCodeServices.FSharpSymbolUse[] GetAllUsesOfAllSymbols(Microsoft.FSharp.Core.FSharpOption`1[System.Threading.CancellationToken])
FSharp.Compiler.SourceCodeServices.FSharpCheckProjectResults: FSharp.Compiler.SourceCodeServices.FSharpSymbolUse[] GetUsesOfSymbol(FSharp.Compiler.SourceCodeServices.FSharpSymbol, Microsoft.FSharp.Core.FSharpOption`1[System.Threading.CancellationToken])
FSharp.Compiler.SourceCodeServices.FSharpCheckProjectResults: System.String ToString()
FSharp.Compiler.SourceCodeServices.FSharpCheckProjectResults: System.String[] DependencyFiles
FSharp.Compiler.SourceCodeServices.FSharpCheckProjectResults: System.String[] get_DependencyFiles()
FSharp.Compiler.SourceCodeServices.FSharpChecker
FSharp.Compiler.SourceCodeServices.FSharpChecker: Boolean ImplicitlyStartBackgroundWork
FSharp.Compiler.SourceCodeServices.FSharpChecker: Boolean get_ImplicitlyStartBackgroundWork()
FSharp.Compiler.SourceCodeServices.FSharpChecker: FSharp.Compiler.SourceCodeServices.FSharpChecker Create(Microsoft.FSharp.Core.FSharpOption`1[System.Int32], Microsoft.FSharp.Core.FSharpOption`1[System.Boolean], Microsoft.FSharp.Core.FSharpOption`1[System.Boolean], Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.LegacyReferenceResolver], Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.FSharpFunc`2[System.Tuple`2[System.String,System.DateTime],Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`3[System.Object,System.IntPtr,System.Int32]]]], Microsoft.FSharp.Core.FSharpOption`1[System.Boolean], Microsoft.FSharp.Core.FSharpOption`1[System.Boolean], Microsoft.FSharp.Core.FSharpOption`1[System.Boolean], Microsoft.FSharp.Core.FSharpOption`1[System.Boolean])
FSharp.Compiler.SourceCodeServices.FSharpChecker: FSharp.Compiler.SourceCodeServices.FSharpChecker Instance
FSharp.Compiler.SourceCodeServices.FSharpChecker: FSharp.Compiler.SourceCodeServices.FSharpChecker get_Instance()
FSharp.Compiler.SourceCodeServices.FSharpChecker: FSharp.Compiler.SourceCodeServices.FSharpProjectOptions GetProjectOptionsFromCommandLineArgs(System.String, System.String[], Microsoft.FSharp.Core.FSharpOption`1[System.DateTime], Microsoft.FSharp.Core.FSharpOption`1[System.Object])
FSharp.Compiler.SourceCodeServices.FSharpChecker: FSharp.Compiler.SourceCodeServices.FSharpTokenInfo[][] TokenizeFile(System.String)
FSharp.Compiler.SourceCodeServices.FSharpChecker: Int32 CurrentQueueLength
FSharp.Compiler.SourceCodeServices.FSharpChecker: Int32 GlobalForegroundParseCountStatistic
FSharp.Compiler.SourceCodeServices.FSharpChecker: Int32 GlobalForegroundTypeCheckCountStatistic
FSharp.Compiler.SourceCodeServices.FSharpChecker: Int32 MaxMemory
FSharp.Compiler.SourceCodeServices.FSharpChecker: Int32 PauseBeforeBackgroundWork
FSharp.Compiler.SourceCodeServices.FSharpChecker: Int32 get_CurrentQueueLength()
FSharp.Compiler.SourceCodeServices.FSharpChecker: Int32 get_GlobalForegroundParseCountStatistic()
FSharp.Compiler.SourceCodeServices.FSharpChecker: Int32 get_GlobalForegroundTypeCheckCountStatistic()
FSharp.Compiler.SourceCodeServices.FSharpChecker: Int32 get_MaxMemory()
FSharp.Compiler.SourceCodeServices.FSharpChecker: Int32 get_PauseBeforeBackgroundWork()
FSharp.Compiler.SourceCodeServices.FSharpChecker: Microsoft.FSharp.Control.FSharpAsync`1[FSharp.Compiler.SourceCodeServices.FSharpCheckFileAnswer] CheckFileInProject(FSharp.Compiler.SourceCodeServices.FSharpParseFileResults, System.String, Int32, FSharp.Compiler.Text.ISourceText, FSharp.Compiler.SourceCodeServices.FSharpProjectOptions, Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.SourceCodeServices.FSharpChecker: Microsoft.FSharp.Control.FSharpAsync`1[FSharp.Compiler.SourceCodeServices.FSharpCheckProjectResults] ParseAndCheckProject(FSharp.Compiler.SourceCodeServices.FSharpProjectOptions, Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.SourceCodeServices.FSharpChecker: Microsoft.FSharp.Control.FSharpAsync`1[FSharp.Compiler.SourceCodeServices.FSharpParseFileResults] GetBackgroundParseResultsForFileInProject(System.String, FSharp.Compiler.SourceCodeServices.FSharpProjectOptions, Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.SourceCodeServices.FSharpChecker: Microsoft.FSharp.Control.FSharpAsync`1[FSharp.Compiler.SourceCodeServices.FSharpParseFileResults] ParseFile(System.String, FSharp.Compiler.Text.ISourceText, FSharp.Compiler.SourceCodeServices.FSharpParsingOptions, Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.SourceCodeServices.FSharpChecker: Microsoft.FSharp.Control.FSharpAsync`1[FSharp.Compiler.SourceCodeServices.FSharpParseFileResults] ParseFileInProject(System.String, System.String, FSharp.Compiler.SourceCodeServices.FSharpProjectOptions, Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.SourceCodeServices.FSharpChecker: Microsoft.FSharp.Control.FSharpAsync`1[FSharp.Compiler.SourceCodeServices.FSharpParseFileResults] ParseFileNoCache(System.String, FSharp.Compiler.Text.ISourceText, FSharp.Compiler.SourceCodeServices.FSharpParsingOptions, Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.SourceCodeServices.FSharpChecker: Microsoft.FSharp.Control.FSharpAsync`1[Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpCheckFileAnswer]] CheckFileInProjectAllowingStaleCachedResults(FSharp.Compiler.SourceCodeServices.FSharpParseFileResults, System.String, Int32, System.String, FSharp.Compiler.SourceCodeServices.FSharpProjectOptions, Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.SourceCodeServices.FSharpChecker: Microsoft.FSharp.Control.FSharpAsync`1[Microsoft.FSharp.Core.Unit] NotifyProjectCleaned(FSharp.Compiler.SourceCodeServices.FSharpProjectOptions, Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.SourceCodeServices.FSharpChecker: Microsoft.FSharp.Control.FSharpAsync`1[System.Collections.Generic.IEnumerable`1[FSharp.Compiler.Text.Range]] FindBackgroundReferencesInFile(System.String, FSharp.Compiler.SourceCodeServices.FSharpProjectOptions, FSharp.Compiler.SourceCodeServices.FSharpSymbol, Microsoft.FSharp.Core.FSharpOption`1[System.Boolean], Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.SourceCodeServices.FSharpChecker: Microsoft.FSharp.Control.FSharpAsync`1[System.Tuple`2[FSharp.Compiler.SourceCodeServices.FSharpDiagnostic[],System.Int32]] Compile(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+ParsedInput], System.String, System.String, Microsoft.FSharp.Collections.FSharpList`1[System.String], Microsoft.FSharp.Core.FSharpOption`1[System.String], Microsoft.FSharp.Core.FSharpOption`1[System.Boolean], Microsoft.FSharp.Core.FSharpOption`1[System.Boolean], Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.SourceCodeServices.FSharpChecker: Microsoft.FSharp.Control.FSharpAsync`1[System.Tuple`2[FSharp.Compiler.SourceCodeServices.FSharpDiagnostic[],System.Int32]] Compile(System.String[], Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.SourceCodeServices.FSharpChecker: Microsoft.FSharp.Control.FSharpAsync`1[System.Tuple`2[FSharp.Compiler.SourceCodeServices.FSharpParseFileResults,FSharp.Compiler.SourceCodeServices.FSharpCheckFileAnswer]] ParseAndCheckFileInProject(System.String, Int32, FSharp.Compiler.Text.ISourceText, FSharp.Compiler.SourceCodeServices.FSharpProjectOptions, Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.SourceCodeServices.FSharpChecker: Microsoft.FSharp.Control.FSharpAsync`1[System.Tuple`2[FSharp.Compiler.SourceCodeServices.FSharpParseFileResults,FSharp.Compiler.SourceCodeServices.FSharpCheckFileResults]] GetBackgroundCheckResultsForFileInProject(System.String, FSharp.Compiler.SourceCodeServices.FSharpProjectOptions, Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.SourceCodeServices.FSharpChecker: Microsoft.FSharp.Control.FSharpAsync`1[System.Tuple`2[FSharp.Compiler.SourceCodeServices.FSharpProjectOptions,Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpDiagnostic]]] GetProjectOptionsFromScript(System.String, FSharp.Compiler.Text.ISourceText, Microsoft.FSharp.Core.FSharpOption`1[System.Boolean], Microsoft.FSharp.Core.FSharpOption`1[System.DateTime], Microsoft.FSharp.Core.FSharpOption`1[System.String[]], Microsoft.FSharp.Core.FSharpOption`1[System.Boolean], Microsoft.FSharp.Core.FSharpOption`1[System.Boolean], Microsoft.FSharp.Core.FSharpOption`1[System.Boolean], Microsoft.FSharp.Core.FSharpOption`1[System.String], Microsoft.FSharp.Core.FSharpOption`1[System.Object], Microsoft.FSharp.Core.FSharpOption`1[System.Int64], Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.SourceCodeServices.FSharpChecker: Microsoft.FSharp.Control.FSharpAsync`1[System.Tuple`2[FSharp.Compiler.Text.Range,FSharp.Compiler.Text.Range][]] MatchBraces(System.String, FSharp.Compiler.Text.ISourceText, FSharp.Compiler.SourceCodeServices.FSharpParsingOptions, Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.SourceCodeServices.FSharpChecker: Microsoft.FSharp.Control.FSharpAsync`1[System.Tuple`2[FSharp.Compiler.Text.Range,FSharp.Compiler.Text.Range][]] MatchBraces(System.String, System.String, FSharp.Compiler.SourceCodeServices.FSharpProjectOptions, Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.SourceCodeServices.FSharpChecker: Microsoft.FSharp.Control.FSharpAsync`1[System.Tuple`3[FSharp.Compiler.SourceCodeServices.FSharpDiagnostic[],System.Int32,Microsoft.FSharp.Core.FSharpOption`1[System.Reflection.Assembly]]] CompileToDynamicAssembly(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+ParsedInput], System.String, Microsoft.FSharp.Collections.FSharpList`1[System.String], Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[System.IO.TextWriter,System.IO.TextWriter]], Microsoft.FSharp.Core.FSharpOption`1[System.Boolean], Microsoft.FSharp.Core.FSharpOption`1[System.Boolean], Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.SourceCodeServices.FSharpChecker: Microsoft.FSharp.Control.FSharpAsync`1[System.Tuple`3[FSharp.Compiler.SourceCodeServices.FSharpDiagnostic[],System.Int32,Microsoft.FSharp.Core.FSharpOption`1[System.Reflection.Assembly]]] CompileToDynamicAssembly(System.String[], Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[System.IO.TextWriter,System.IO.TextWriter]], Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.SourceCodeServices.FSharpChecker: Microsoft.FSharp.Control.FSharpAsync`1[System.ValueTuple`2[FSharp.Compiler.Text.Range,FSharp.Compiler.SourceCodeServices.SemanticClassificationType][]] GetBackgroundSemanticClassificationForFile(System.String, FSharp.Compiler.SourceCodeServices.FSharpProjectOptions, Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.SourceCodeServices.FSharpChecker: Microsoft.FSharp.Control.IEvent`2[Microsoft.FSharp.Control.FSharpHandler`1[Microsoft.FSharp.Core.Unit],Microsoft.FSharp.Core.Unit] MaxMemoryReached
FSharp.Compiler.SourceCodeServices.FSharpChecker: Microsoft.FSharp.Control.IEvent`2[Microsoft.FSharp.Control.FSharpHandler`1[Microsoft.FSharp.Core.Unit],Microsoft.FSharp.Core.Unit] get_MaxMemoryReached()
FSharp.Compiler.SourceCodeServices.FSharpChecker: Microsoft.FSharp.Control.IEvent`2[Microsoft.FSharp.Control.FSharpHandler`1[System.Tuple`2[System.String,Microsoft.FSharp.Core.FSharpOption`1[System.Object]]],System.Tuple`2[System.String,Microsoft.FSharp.Core.FSharpOption`1[System.Object]]] BeforeBackgroundFileCheck
FSharp.Compiler.SourceCodeServices.FSharpChecker: Microsoft.FSharp.Control.IEvent`2[Microsoft.FSharp.Control.FSharpHandler`1[System.Tuple`2[System.String,Microsoft.FSharp.Core.FSharpOption`1[System.Object]]],System.Tuple`2[System.String,Microsoft.FSharp.Core.FSharpOption`1[System.Object]]] FileChecked
FSharp.Compiler.SourceCodeServices.FSharpChecker: Microsoft.FSharp.Control.IEvent`2[Microsoft.FSharp.Control.FSharpHandler`1[System.Tuple`2[System.String,Microsoft.FSharp.Core.FSharpOption`1[System.Object]]],System.Tuple`2[System.String,Microsoft.FSharp.Core.FSharpOption`1[System.Object]]] FileParsed
FSharp.Compiler.SourceCodeServices.FSharpChecker: Microsoft.FSharp.Control.IEvent`2[Microsoft.FSharp.Control.FSharpHandler`1[System.Tuple`2[System.String,Microsoft.FSharp.Core.FSharpOption`1[System.Object]]],System.Tuple`2[System.String,Microsoft.FSharp.Core.FSharpOption`1[System.Object]]] ProjectChecked
FSharp.Compiler.SourceCodeServices.FSharpChecker: Microsoft.FSharp.Control.IEvent`2[Microsoft.FSharp.Control.FSharpHandler`1[System.Tuple`2[System.String,Microsoft.FSharp.Core.FSharpOption`1[System.Object]]],System.Tuple`2[System.String,Microsoft.FSharp.Core.FSharpOption`1[System.Object]]] get_BeforeBackgroundFileCheck()
FSharp.Compiler.SourceCodeServices.FSharpChecker: Microsoft.FSharp.Control.IEvent`2[Microsoft.FSharp.Control.FSharpHandler`1[System.Tuple`2[System.String,Microsoft.FSharp.Core.FSharpOption`1[System.Object]]],System.Tuple`2[System.String,Microsoft.FSharp.Core.FSharpOption`1[System.Object]]] get_FileChecked()
FSharp.Compiler.SourceCodeServices.FSharpChecker: Microsoft.FSharp.Control.IEvent`2[Microsoft.FSharp.Control.FSharpHandler`1[System.Tuple`2[System.String,Microsoft.FSharp.Core.FSharpOption`1[System.Object]]],System.Tuple`2[System.String,Microsoft.FSharp.Core.FSharpOption`1[System.Object]]] get_FileParsed()
FSharp.Compiler.SourceCodeServices.FSharpChecker: Microsoft.FSharp.Control.IEvent`2[Microsoft.FSharp.Control.FSharpHandler`1[System.Tuple`2[System.String,Microsoft.FSharp.Core.FSharpOption`1[System.Object]]],System.Tuple`2[System.String,Microsoft.FSharp.Core.FSharpOption`1[System.Object]]] get_ProjectChecked()
FSharp.Compiler.SourceCodeServices.FSharpChecker: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`3[FSharp.Compiler.SourceCodeServices.FSharpParseFileResults,FSharp.Compiler.SourceCodeServices.FSharpCheckFileResults,System.Int32]] TryGetRecentCheckResultsForFile(System.String, FSharp.Compiler.SourceCodeServices.FSharpProjectOptions, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.ISourceText], Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.SourceCodeServices.FSharpChecker: System.Tuple`2[FSharp.Compiler.SourceCodeServices.FSharpParsingOptions,Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpDiagnostic]] GetParsingOptionsFromCommandLineArgs(Microsoft.FSharp.Collections.FSharpList`1[System.String], Microsoft.FSharp.Collections.FSharpList`1[System.String], Microsoft.FSharp.Core.FSharpOption`1[System.Boolean])
FSharp.Compiler.SourceCodeServices.FSharpChecker: System.Tuple`2[FSharp.Compiler.SourceCodeServices.FSharpParsingOptions,Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpDiagnostic]] GetParsingOptionsFromCommandLineArgs(Microsoft.FSharp.Collections.FSharpList`1[System.String], Microsoft.FSharp.Core.FSharpOption`1[System.Boolean])
FSharp.Compiler.SourceCodeServices.FSharpChecker: System.Tuple`2[FSharp.Compiler.SourceCodeServices.FSharpParsingOptions,Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpDiagnostic]] GetParsingOptionsFromProjectOptions(FSharp.Compiler.SourceCodeServices.FSharpProjectOptions)
FSharp.Compiler.SourceCodeServices.FSharpChecker: System.Tuple`2[FSharp.Compiler.SourceCodeServices.FSharpTokenInfo[],FSharp.Compiler.SourceCodeServices.FSharpTokenizerLexState] TokenizeLine(System.String, FSharp.Compiler.SourceCodeServices.FSharpTokenizerLexState)
FSharp.Compiler.SourceCodeServices.FSharpChecker: Void CheckProjectInBackground(FSharp.Compiler.SourceCodeServices.FSharpProjectOptions, Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.SourceCodeServices.FSharpChecker: Void ClearCache(System.Collections.Generic.IEnumerable`1[FSharp.Compiler.SourceCodeServices.FSharpProjectOptions], Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.SourceCodeServices.FSharpChecker: Void ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()
FSharp.Compiler.SourceCodeServices.FSharpChecker: Void InvalidateAll()
FSharp.Compiler.SourceCodeServices.FSharpChecker: Void InvalidateConfiguration(FSharp.Compiler.SourceCodeServices.FSharpProjectOptions, Microsoft.FSharp.Core.FSharpOption`1[System.Boolean], Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.SourceCodeServices.FSharpChecker: Void StopBackgroundCompile()
FSharp.Compiler.SourceCodeServices.FSharpChecker: Void WaitForBackgroundCompile()
FSharp.Compiler.SourceCodeServices.FSharpChecker: Void set_ImplicitlyStartBackgroundWork(Boolean)
FSharp.Compiler.SourceCodeServices.FSharpChecker: Void set_MaxMemory(Int32)
FSharp.Compiler.SourceCodeServices.FSharpChecker: Void set_PauseBeforeBackgroundWork(Int32)
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind+Method: Boolean get_isExtension()
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind+Method: Boolean isExtension
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind+Tags: Int32 Argument
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind+Tags: Int32 CustomOperation
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind+Tags: Int32 Event
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind+Tags: Int32 Field
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind+Tags: Int32 Method
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind+Tags: Int32 Other
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind+Tags: Int32 Property
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind: Boolean Equals(FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind)
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind: Boolean IsArgument
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind: Boolean IsCustomOperation
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind: Boolean IsEvent
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind: Boolean IsField
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind: Boolean IsMethod
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind: Boolean IsOther
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind: Boolean IsProperty
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind: Boolean get_IsArgument()
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind: Boolean get_IsCustomOperation()
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind: Boolean get_IsEvent()
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind: Boolean get_IsField()
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind: Boolean get_IsMethod()
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind: Boolean get_IsOther()
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind: Boolean get_IsProperty()
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind: FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind Argument
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind: FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind CustomOperation
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind: FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind Event
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind: FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind Field
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind: FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind NewMethod(Boolean)
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind: FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind Other
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind: FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind Property
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind: FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind get_Argument()
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind: FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind get_CustomOperation()
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind: FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind get_Event()
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind: FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind get_Field()
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind: FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind get_Other()
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind: FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind get_Property()
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind: FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind+Method
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind: FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind+Tags
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind: Int32 CompareTo(FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind)
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind: Int32 CompareTo(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind: Int32 Tag
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind: Int32 get_Tag()
FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind: System.String ToString()
FSharp.Compiler.SourceCodeServices.FSharpDeclarationListInfo
FSharp.Compiler.SourceCodeServices.FSharpDeclarationListInfo: Boolean IsError
FSharp.Compiler.SourceCodeServices.FSharpDeclarationListInfo: Boolean IsForType
FSharp.Compiler.SourceCodeServices.FSharpDeclarationListInfo: Boolean get_IsError()
FSharp.Compiler.SourceCodeServices.FSharpDeclarationListInfo: Boolean get_IsForType()
FSharp.Compiler.SourceCodeServices.FSharpDeclarationListInfo: FSharp.Compiler.SourceCodeServices.FSharpDeclarationListInfo Empty
FSharp.Compiler.SourceCodeServices.FSharpDeclarationListInfo: FSharp.Compiler.SourceCodeServices.FSharpDeclarationListInfo get_Empty()
FSharp.Compiler.SourceCodeServices.FSharpDeclarationListInfo: FSharp.Compiler.SourceCodeServices.FSharpDeclarationListItem[] Items
FSharp.Compiler.SourceCodeServices.FSharpDeclarationListInfo: FSharp.Compiler.SourceCodeServices.FSharpDeclarationListItem[] get_Items()
FSharp.Compiler.SourceCodeServices.FSharpDeclarationListItem
FSharp.Compiler.SourceCodeServices.FSharpDeclarationListItem: Boolean IsOwnMember
FSharp.Compiler.SourceCodeServices.FSharpDeclarationListItem: Boolean IsResolved
FSharp.Compiler.SourceCodeServices.FSharpDeclarationListItem: Boolean get_IsOwnMember()
FSharp.Compiler.SourceCodeServices.FSharpDeclarationListItem: Boolean get_IsResolved()
FSharp.Compiler.SourceCodeServices.FSharpDeclarationListItem: FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind Kind
FSharp.Compiler.SourceCodeServices.FSharpDeclarationListItem: FSharp.Compiler.SourceCodeServices.FSharpCompletionItemKind get_Kind()
FSharp.Compiler.SourceCodeServices.FSharpDeclarationListItem: FSharp.Compiler.SourceCodeServices.FSharpGlyph Glyph
FSharp.Compiler.SourceCodeServices.FSharpDeclarationListItem: FSharp.Compiler.SourceCodeServices.FSharpGlyph get_Glyph()
FSharp.Compiler.SourceCodeServices.FSharpDeclarationListItem: FSharp.Compiler.SourceCodeServices.FSharpToolTipText`1[FSharp.Compiler.TextLayout.Layout] StructuredDescriptionText
FSharp.Compiler.SourceCodeServices.FSharpDeclarationListItem: FSharp.Compiler.SourceCodeServices.FSharpToolTipText`1[FSharp.Compiler.TextLayout.Layout] get_StructuredDescriptionText()
FSharp.Compiler.SourceCodeServices.FSharpDeclarationListItem: FSharp.Compiler.SourceCodeServices.FSharpToolTipText`1[System.String] DescriptionText
FSharp.Compiler.SourceCodeServices.FSharpDeclarationListItem: FSharp.Compiler.SourceCodeServices.FSharpToolTipText`1[System.String] get_DescriptionText()
FSharp.Compiler.SourceCodeServices.FSharpDeclarationListItem: Int32 MinorPriority
FSharp.Compiler.SourceCodeServices.FSharpDeclarationListItem: Int32 get_MinorPriority()
FSharp.Compiler.SourceCodeServices.FSharpDeclarationListItem: Microsoft.FSharp.Control.FSharpAsync`1[FSharp.Compiler.SourceCodeServices.FSharpToolTipText`1[FSharp.Compiler.TextLayout.Layout]] StructuredDescriptionTextAsync
FSharp.Compiler.SourceCodeServices.FSharpDeclarationListItem: Microsoft.FSharp.Control.FSharpAsync`1[FSharp.Compiler.SourceCodeServices.FSharpToolTipText`1[FSharp.Compiler.TextLayout.Layout]] get_StructuredDescriptionTextAsync()
FSharp.Compiler.SourceCodeServices.FSharpDeclarationListItem: Microsoft.FSharp.Control.FSharpAsync`1[FSharp.Compiler.SourceCodeServices.FSharpToolTipText`1[System.String]] DescriptionTextAsync
FSharp.Compiler.SourceCodeServices.FSharpDeclarationListItem: Microsoft.FSharp.Control.FSharpAsync`1[FSharp.Compiler.SourceCodeServices.FSharpToolTipText`1[System.String]] get_DescriptionTextAsync()
FSharp.Compiler.SourceCodeServices.FSharpDeclarationListItem: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpAccessibility] Accessibility
FSharp.Compiler.SourceCodeServices.FSharpDeclarationListItem: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpAccessibility] get_Accessibility()
FSharp.Compiler.SourceCodeServices.FSharpDeclarationListItem: Microsoft.FSharp.Core.FSharpOption`1[System.String] NamespaceToOpen
FSharp.Compiler.SourceCodeServices.FSharpDeclarationListItem: Microsoft.FSharp.Core.FSharpOption`1[System.String] get_NamespaceToOpen()
FSharp.Compiler.SourceCodeServices.FSharpDeclarationListItem: System.String FullName
FSharp.Compiler.SourceCodeServices.FSharpDeclarationListItem: System.String Name
FSharp.Compiler.SourceCodeServices.FSharpDeclarationListItem: System.String NameInCode
FSharp.Compiler.SourceCodeServices.FSharpDeclarationListItem: System.String get_FullName()
FSharp.Compiler.SourceCodeServices.FSharpDeclarationListItem: System.String get_Name()
FSharp.Compiler.SourceCodeServices.FSharpDeclarationListItem: System.String get_NameInCode()
FSharp.Compiler.SourceCodeServices.FSharpDelegateSignature
FSharp.Compiler.SourceCodeServices.FSharpDelegateSignature: FSharp.Compiler.SourceCodeServices.FSharpType DelegateReturnType
FSharp.Compiler.SourceCodeServices.FSharpDelegateSignature: FSharp.Compiler.SourceCodeServices.FSharpType get_DelegateReturnType()
FSharp.Compiler.SourceCodeServices.FSharpDelegateSignature: System.Collections.Generic.IList`1[System.Tuple`2[Microsoft.FSharp.Core.FSharpOption`1[System.String],FSharp.Compiler.SourceCodeServices.FSharpType]] DelegateArguments
FSharp.Compiler.SourceCodeServices.FSharpDelegateSignature: System.Collections.Generic.IList`1[System.Tuple`2[Microsoft.FSharp.Core.FSharpOption`1[System.String],FSharp.Compiler.SourceCodeServices.FSharpType]] get_DelegateArguments()
FSharp.Compiler.SourceCodeServices.FSharpDelegateSignature: System.String ToString()
FSharp.Compiler.SourceCodeServices.FSharpDiagnostic
FSharp.Compiler.SourceCodeServices.FSharpDiagnostic: FSharp.Compiler.SourceCodeServices.FSharpDiagnosticSeverity Severity
FSharp.Compiler.SourceCodeServices.FSharpDiagnostic: FSharp.Compiler.SourceCodeServices.FSharpDiagnosticSeverity get_Severity()
FSharp.Compiler.SourceCodeServices.FSharpDiagnostic: FSharp.Compiler.Text.Pos End
FSharp.Compiler.SourceCodeServices.FSharpDiagnostic: FSharp.Compiler.Text.Pos Start
FSharp.Compiler.SourceCodeServices.FSharpDiagnostic: FSharp.Compiler.Text.Pos get_End()
FSharp.Compiler.SourceCodeServices.FSharpDiagnostic: FSharp.Compiler.Text.Pos get_Start()
FSharp.Compiler.SourceCodeServices.FSharpDiagnostic: FSharp.Compiler.Text.Range Range
FSharp.Compiler.SourceCodeServices.FSharpDiagnostic: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.SourceCodeServices.FSharpDiagnostic: Int32 EndColumn
FSharp.Compiler.SourceCodeServices.FSharpDiagnostic: Int32 EndLineAlternate
FSharp.Compiler.SourceCodeServices.FSharpDiagnostic: Int32 ErrorNumber
FSharp.Compiler.SourceCodeServices.FSharpDiagnostic: Int32 StartColumn
FSharp.Compiler.SourceCodeServices.FSharpDiagnostic: Int32 StartLineAlternate
FSharp.Compiler.SourceCodeServices.FSharpDiagnostic: Int32 get_EndColumn()
FSharp.Compiler.SourceCodeServices.FSharpDiagnostic: Int32 get_EndLineAlternate()
FSharp.Compiler.SourceCodeServices.FSharpDiagnostic: Int32 get_ErrorNumber()
FSharp.Compiler.SourceCodeServices.FSharpDiagnostic: Int32 get_StartColumn()
FSharp.Compiler.SourceCodeServices.FSharpDiagnostic: Int32 get_StartLineAlternate()
FSharp.Compiler.SourceCodeServices.FSharpDiagnostic: System.String FileName
FSharp.Compiler.SourceCodeServices.FSharpDiagnostic: System.String Message
FSharp.Compiler.SourceCodeServices.FSharpDiagnostic: System.String NewlineifyErrorString(System.String)
FSharp.Compiler.SourceCodeServices.FSharpDiagnostic: System.String NormalizeErrorString(System.String)
FSharp.Compiler.SourceCodeServices.FSharpDiagnostic: System.String Subcategory
FSharp.Compiler.SourceCodeServices.FSharpDiagnostic: System.String ToString()
FSharp.Compiler.SourceCodeServices.FSharpDiagnostic: System.String get_FileName()
FSharp.Compiler.SourceCodeServices.FSharpDiagnostic: System.String get_Message()
FSharp.Compiler.SourceCodeServices.FSharpDiagnostic: System.String get_Subcategory()
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticOptions
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticOptions: Boolean Equals(FSharp.Compiler.SourceCodeServices.FSharpDiagnosticOptions)
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticOptions: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticOptions: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticOptions: Boolean GlobalWarnAsError
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticOptions: Boolean get_GlobalWarnAsError()
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticOptions: FSharp.Compiler.SourceCodeServices.FSharpDiagnosticOptions Default
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticOptions: FSharp.Compiler.SourceCodeServices.FSharpDiagnosticOptions get_Default()
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticOptions: Int32 CompareTo(FSharp.Compiler.SourceCodeServices.FSharpDiagnosticOptions)
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticOptions: Int32 CompareTo(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticOptions: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticOptions: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticOptions: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticOptions: Int32 WarnLevel
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticOptions: Int32 get_WarnLevel()
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticOptions: Microsoft.FSharp.Collections.FSharpList`1[System.Int32] WarnAsError
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticOptions: Microsoft.FSharp.Collections.FSharpList`1[System.Int32] WarnAsWarn
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticOptions: Microsoft.FSharp.Collections.FSharpList`1[System.Int32] WarnOff
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticOptions: Microsoft.FSharp.Collections.FSharpList`1[System.Int32] WarnOn
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticOptions: Microsoft.FSharp.Collections.FSharpList`1[System.Int32] get_WarnAsError()
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticOptions: Microsoft.FSharp.Collections.FSharpList`1[System.Int32] get_WarnAsWarn()
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticOptions: Microsoft.FSharp.Collections.FSharpList`1[System.Int32] get_WarnOff()
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticOptions: Microsoft.FSharp.Collections.FSharpList`1[System.Int32] get_WarnOn()
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticOptions: System.String ToString()
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticOptions: Void .ctor(Int32, Boolean, Microsoft.FSharp.Collections.FSharpList`1[System.Int32], Microsoft.FSharp.Collections.FSharpList`1[System.Int32], Microsoft.FSharp.Collections.FSharpList`1[System.Int32], Microsoft.FSharp.Collections.FSharpList`1[System.Int32])
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticSeverity
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticSeverity+Tags: Int32 Error
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticSeverity+Tags: Int32 Hidden
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticSeverity+Tags: Int32 Info
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticSeverity+Tags: Int32 Warning
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticSeverity: Boolean Equals(FSharp.Compiler.SourceCodeServices.FSharpDiagnosticSeverity)
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticSeverity: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticSeverity: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticSeverity: Boolean IsError
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticSeverity: Boolean IsHidden
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticSeverity: Boolean IsInfo
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticSeverity: Boolean IsWarning
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticSeverity: Boolean get_IsError()
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticSeverity: Boolean get_IsHidden()
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticSeverity: Boolean get_IsInfo()
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticSeverity: Boolean get_IsWarning()
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticSeverity: FSharp.Compiler.SourceCodeServices.FSharpDiagnosticSeverity Error
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticSeverity: FSharp.Compiler.SourceCodeServices.FSharpDiagnosticSeverity Hidden
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticSeverity: FSharp.Compiler.SourceCodeServices.FSharpDiagnosticSeverity Info
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticSeverity: FSharp.Compiler.SourceCodeServices.FSharpDiagnosticSeverity Warning
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticSeverity: FSharp.Compiler.SourceCodeServices.FSharpDiagnosticSeverity get_Error()
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticSeverity: FSharp.Compiler.SourceCodeServices.FSharpDiagnosticSeverity get_Hidden()
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticSeverity: FSharp.Compiler.SourceCodeServices.FSharpDiagnosticSeverity get_Info()
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticSeverity: FSharp.Compiler.SourceCodeServices.FSharpDiagnosticSeverity get_Warning()
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticSeverity: FSharp.Compiler.SourceCodeServices.FSharpDiagnosticSeverity+Tags
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticSeverity: Int32 CompareTo(FSharp.Compiler.SourceCodeServices.FSharpDiagnosticSeverity)
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticSeverity: Int32 CompareTo(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticSeverity: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticSeverity: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticSeverity: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticSeverity: Int32 Tag
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticSeverity: Int32 get_Tag()
FSharp.Compiler.SourceCodeServices.FSharpDiagnosticSeverity: System.String ToString()
FSharp.Compiler.SourceCodeServices.FSharpDisplayContext
FSharp.Compiler.SourceCodeServices.FSharpDisplayContext: FSharp.Compiler.SourceCodeServices.FSharpDisplayContext Empty
FSharp.Compiler.SourceCodeServices.FSharpDisplayContext: FSharp.Compiler.SourceCodeServices.FSharpDisplayContext WithPrefixGenericParameters()
FSharp.Compiler.SourceCodeServices.FSharpDisplayContext: FSharp.Compiler.SourceCodeServices.FSharpDisplayContext WithShortTypeNames(Boolean)
FSharp.Compiler.SourceCodeServices.FSharpDisplayContext: FSharp.Compiler.SourceCodeServices.FSharpDisplayContext WithSuffixGenericParameters()
FSharp.Compiler.SourceCodeServices.FSharpDisplayContext: FSharp.Compiler.SourceCodeServices.FSharpDisplayContext get_Empty()
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind+Tags: Int32 Class
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind+Tags: Int32 DU
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind+Tags: Int32 Enum
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind+Tags: Int32 Exception
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind+Tags: Int32 Interface
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind+Tags: Int32 Module
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind+Tags: Int32 Namespace
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind+Tags: Int32 Record
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind: Boolean Equals(FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind)
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind: Boolean IsClass
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind: Boolean IsDU
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind: Boolean IsEnum
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind: Boolean IsException
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind: Boolean IsInterface
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind: Boolean IsModule
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind: Boolean IsNamespace
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind: Boolean IsRecord
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind: Boolean get_IsClass()
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind: Boolean get_IsDU()
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind: Boolean get_IsEnum()
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind: Boolean get_IsException()
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind: Boolean get_IsInterface()
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind: Boolean get_IsModule()
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind: Boolean get_IsNamespace()
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind: Boolean get_IsRecord()
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind: FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind Class
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind: FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind DU
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind: FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind Enum
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind: FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind Exception
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind: FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind Interface
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind: FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind Module
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind: FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind Namespace
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind: FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind Record
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind: FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind get_Class()
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind: FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind get_DU()
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind: FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind get_Enum()
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind: FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind get_Exception()
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind: FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind get_Interface()
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind: FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind get_Module()
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind: FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind get_Namespace()
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind: FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind get_Record()
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind: FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind+Tags
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind: Int32 CompareTo(FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind)
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind: Int32 CompareTo(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind: Int32 Tag
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind: Int32 get_Tag()
FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind: System.String ToString()
FSharp.Compiler.SourceCodeServices.FSharpEntity
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean HasAssemblyCodeRepresentation
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean HasFSharpModuleSuffix
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean IsAbstractClass
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean IsArrayType
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean IsAttributeType
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean IsByRef
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean IsClass
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean IsDelegate
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean IsEnum
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean IsFSharp
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean IsFSharpAbbreviation
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean IsFSharpExceptionDeclaration
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean IsFSharpModule
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean IsFSharpRecord
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean IsFSharpUnion
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean IsInterface
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean IsMeasure
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean IsNamespace
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean IsOpaque
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean IsProvided
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean IsProvidedAndErased
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean IsProvidedAndGenerated
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean IsStaticInstantiation
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean IsUnresolved
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean IsValueType
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean UsesPrefixDisplay
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean get_HasAssemblyCodeRepresentation()
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean get_HasFSharpModuleSuffix()
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean get_IsAbstractClass()
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean get_IsArrayType()
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean get_IsAttributeType()
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean get_IsByRef()
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean get_IsClass()
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean get_IsDelegate()
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean get_IsEnum()
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean get_IsFSharp()
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean get_IsFSharpAbbreviation()
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean get_IsFSharpExceptionDeclaration()
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean get_IsFSharpModule()
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean get_IsFSharpRecord()
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean get_IsFSharpUnion()
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean get_IsInterface()
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean get_IsMeasure()
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean get_IsNamespace()
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean get_IsOpaque()
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean get_IsProvided()
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean get_IsProvidedAndErased()
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean get_IsProvidedAndGenerated()
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean get_IsStaticInstantiation()
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean get_IsUnresolved()
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean get_IsValueType()
FSharp.Compiler.SourceCodeServices.FSharpEntity: Boolean get_UsesPrefixDisplay()
FSharp.Compiler.SourceCodeServices.FSharpEntity: FSharp.Compiler.SourceCodeServices.FSharpAccessibility Accessibility
FSharp.Compiler.SourceCodeServices.FSharpEntity: FSharp.Compiler.SourceCodeServices.FSharpAccessibility RepresentationAccessibility
FSharp.Compiler.SourceCodeServices.FSharpEntity: FSharp.Compiler.SourceCodeServices.FSharpAccessibility get_Accessibility()
FSharp.Compiler.SourceCodeServices.FSharpEntity: FSharp.Compiler.SourceCodeServices.FSharpAccessibility get_RepresentationAccessibility()
FSharp.Compiler.SourceCodeServices.FSharpEntity: FSharp.Compiler.SourceCodeServices.FSharpDelegateSignature FSharpDelegateSignature
FSharp.Compiler.SourceCodeServices.FSharpEntity: FSharp.Compiler.SourceCodeServices.FSharpDelegateSignature get_FSharpDelegateSignature()
FSharp.Compiler.SourceCodeServices.FSharpEntity: FSharp.Compiler.SourceCodeServices.FSharpType AbbreviatedType
FSharp.Compiler.SourceCodeServices.FSharpEntity: FSharp.Compiler.SourceCodeServices.FSharpType get_AbbreviatedType()
FSharp.Compiler.SourceCodeServices.FSharpEntity: FSharp.Compiler.Text.Range DeclarationLocation
FSharp.Compiler.SourceCodeServices.FSharpEntity: FSharp.Compiler.Text.Range get_DeclarationLocation()
FSharp.Compiler.SourceCodeServices.FSharpEntity: Int32 ArrayRank
FSharp.Compiler.SourceCodeServices.FSharpEntity: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.FSharpEntity: Int32 get_ArrayRank()
FSharp.Compiler.SourceCodeServices.FSharpEntity: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpActivePatternCase] ActivePatternCases
FSharp.Compiler.SourceCodeServices.FSharpEntity: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpActivePatternCase] get_ActivePatternCases()
FSharp.Compiler.SourceCodeServices.FSharpEntity: Microsoft.FSharp.Collections.FSharpList`1[System.String] AllCompilationPaths
FSharp.Compiler.SourceCodeServices.FSharpEntity: Microsoft.FSharp.Collections.FSharpList`1[System.String] get_AllCompilationPaths()
FSharp.Compiler.SourceCodeServices.FSharpEntity: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpEntity] DeclaringEntity
FSharp.Compiler.SourceCodeServices.FSharpEntity: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpEntity] get_DeclaringEntity()
FSharp.Compiler.SourceCodeServices.FSharpEntity: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpType] BaseType
FSharp.Compiler.SourceCodeServices.FSharpEntity: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpType] get_BaseType()
FSharp.Compiler.SourceCodeServices.FSharpEntity: Microsoft.FSharp.Core.FSharpOption`1[System.String] Namespace
FSharp.Compiler.SourceCodeServices.FSharpEntity: Microsoft.FSharp.Core.FSharpOption`1[System.String] TryFullName
FSharp.Compiler.SourceCodeServices.FSharpEntity: Microsoft.FSharp.Core.FSharpOption`1[System.String] TryGetFullCompiledName()
FSharp.Compiler.SourceCodeServices.FSharpEntity: Microsoft.FSharp.Core.FSharpOption`1[System.String] TryGetFullDisplayName()
FSharp.Compiler.SourceCodeServices.FSharpEntity: Microsoft.FSharp.Core.FSharpOption`1[System.String] TryGetFullName()
FSharp.Compiler.SourceCodeServices.FSharpEntity: Microsoft.FSharp.Core.FSharpOption`1[System.String] get_Namespace()
FSharp.Compiler.SourceCodeServices.FSharpEntity: Microsoft.FSharp.Core.FSharpOption`1[System.String] get_TryFullName()
FSharp.Compiler.SourceCodeServices.FSharpEntity: System.Collections.Generic.IEnumerable`1[FSharp.Compiler.SourceCodeServices.FSharpEntity] GetPublicNestedEntities()
FSharp.Compiler.SourceCodeServices.FSharpEntity: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpAttribute] Attributes
FSharp.Compiler.SourceCodeServices.FSharpEntity: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpAttribute] get_Attributes()
FSharp.Compiler.SourceCodeServices.FSharpEntity: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpEntity] NestedEntities
FSharp.Compiler.SourceCodeServices.FSharpEntity: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpEntity] get_NestedEntities()
FSharp.Compiler.SourceCodeServices.FSharpEntity: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpField] FSharpFields
FSharp.Compiler.SourceCodeServices.FSharpEntity: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpField] RecordFields
FSharp.Compiler.SourceCodeServices.FSharpEntity: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpField] get_FSharpFields()
FSharp.Compiler.SourceCodeServices.FSharpEntity: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpField] get_RecordFields()
FSharp.Compiler.SourceCodeServices.FSharpEntity: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpGenericParameter] GenericParameters
FSharp.Compiler.SourceCodeServices.FSharpEntity: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpGenericParameter] get_GenericParameters()
FSharp.Compiler.SourceCodeServices.FSharpEntity: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue] MembersFunctionsAndValues
FSharp.Compiler.SourceCodeServices.FSharpEntity: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue] MembersOrValues
FSharp.Compiler.SourceCodeServices.FSharpEntity: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue] TryGetMembersFunctionsAndValues()
FSharp.Compiler.SourceCodeServices.FSharpEntity: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue] get_MembersFunctionsAndValues()
FSharp.Compiler.SourceCodeServices.FSharpEntity: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue] get_MembersOrValues()
FSharp.Compiler.SourceCodeServices.FSharpEntity: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpStaticParameter] StaticParameters
FSharp.Compiler.SourceCodeServices.FSharpEntity: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpStaticParameter] get_StaticParameters()
FSharp.Compiler.SourceCodeServices.FSharpEntity: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpType] AllInterfaces
FSharp.Compiler.SourceCodeServices.FSharpEntity: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpType] DeclaredInterfaces
FSharp.Compiler.SourceCodeServices.FSharpEntity: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpType] get_AllInterfaces()
FSharp.Compiler.SourceCodeServices.FSharpEntity: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpType] get_DeclaredInterfaces()
FSharp.Compiler.SourceCodeServices.FSharpEntity: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpUnionCase] UnionCases
FSharp.Compiler.SourceCodeServices.FSharpEntity: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpUnionCase] get_UnionCases()
FSharp.Compiler.SourceCodeServices.FSharpEntity: System.Collections.Generic.IList`1[System.String] ElaboratedXmlDoc
FSharp.Compiler.SourceCodeServices.FSharpEntity: System.Collections.Generic.IList`1[System.String] XmlDoc
FSharp.Compiler.SourceCodeServices.FSharpEntity: System.Collections.Generic.IList`1[System.String] get_ElaboratedXmlDoc()
FSharp.Compiler.SourceCodeServices.FSharpEntity: System.Collections.Generic.IList`1[System.String] get_XmlDoc()
FSharp.Compiler.SourceCodeServices.FSharpEntity: System.String AccessPath
FSharp.Compiler.SourceCodeServices.FSharpEntity: System.String CompiledName
FSharp.Compiler.SourceCodeServices.FSharpEntity: System.String DisplayName
FSharp.Compiler.SourceCodeServices.FSharpEntity: System.String FullName
FSharp.Compiler.SourceCodeServices.FSharpEntity: System.String LogicalName
FSharp.Compiler.SourceCodeServices.FSharpEntity: System.String QualifiedName
FSharp.Compiler.SourceCodeServices.FSharpEntity: System.String ToString()
FSharp.Compiler.SourceCodeServices.FSharpEntity: System.String XmlDocSig
FSharp.Compiler.SourceCodeServices.FSharpEntity: System.String get_AccessPath()
FSharp.Compiler.SourceCodeServices.FSharpEntity: System.String get_CompiledName()
FSharp.Compiler.SourceCodeServices.FSharpEntity: System.String get_DisplayName()
FSharp.Compiler.SourceCodeServices.FSharpEntity: System.String get_FullName()
FSharp.Compiler.SourceCodeServices.FSharpEntity: System.String get_LogicalName()
FSharp.Compiler.SourceCodeServices.FSharpEntity: System.String get_QualifiedName()
FSharp.Compiler.SourceCodeServices.FSharpEntity: System.String get_XmlDocSig()
FSharp.Compiler.SourceCodeServices.FSharpExpr
FSharp.Compiler.SourceCodeServices.FSharpExpr: FSharp.Compiler.SourceCodeServices.FSharpType Type
FSharp.Compiler.SourceCodeServices.FSharpExpr: FSharp.Compiler.SourceCodeServices.FSharpType get_Type()
FSharp.Compiler.SourceCodeServices.FSharpExpr: FSharp.Compiler.Text.Range Range
FSharp.Compiler.SourceCodeServices.FSharpExpr: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.SourceCodeServices.FSharpExpr: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpExpr] ImmediateSubExpressions
FSharp.Compiler.SourceCodeServices.FSharpExpr: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpExpr] get_ImmediateSubExpressions()
FSharp.Compiler.SourceCodeServices.FSharpExpr: System.String ToString()
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol+Constructor: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.ParamTypeSymbol] args
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol+Constructor: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.ParamTypeSymbol] get_args()
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol+Constructor: System.String get_typeName()
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol+Constructor: System.String typeName
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol+Event: System.String get_name()
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol+Event: System.String get_typeName()
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol+Event: System.String name
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol+Event: System.String typeName
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol+Field: System.String get_name()
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol+Field: System.String get_typeName()
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol+Field: System.String name
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol+Field: System.String typeName
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol+Method: Int32 genericArity
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol+Method: Int32 get_genericArity()
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol+Method: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.ParamTypeSymbol] get_paramSyms()
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol+Method: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.ParamTypeSymbol] paramSyms
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol+Method: System.String get_name()
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol+Method: System.String get_typeName()
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol+Method: System.String name
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol+Method: System.String typeName
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol+Property: System.String get_name()
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol+Property: System.String get_typeName()
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol+Property: System.String name
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol+Property: System.String typeName
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol+Tags: Int32 Constructor
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol+Tags: Int32 Event
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol+Tags: Int32 Field
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol+Tags: Int32 Method
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol+Tags: Int32 Property
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol+Tags: Int32 Type
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol+Type: System.String fullName
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol+Type: System.String get_fullName()
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol: Boolean Equals(FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol)
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol: Boolean IsConstructor
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol: Boolean IsEvent
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol: Boolean IsField
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol: Boolean IsMethod
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol: Boolean IsProperty
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol: Boolean IsType
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol: Boolean get_IsConstructor()
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol: Boolean get_IsEvent()
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol: Boolean get_IsField()
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol: Boolean get_IsMethod()
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol: Boolean get_IsProperty()
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol: Boolean get_IsType()
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol: FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol NewConstructor(System.String, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.ParamTypeSymbol])
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol: FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol NewEvent(System.String, System.String)
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol: FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol NewField(System.String, System.String)
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol: FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol NewMethod(System.String, System.String, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.ParamTypeSymbol], Int32)
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol: FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol NewProperty(System.String, System.String)
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol: FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol NewType(System.String)
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol: FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol+Constructor
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol: FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol+Event
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol: FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol+Field
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol: FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol+Method
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol: FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol+Property
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol: FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol+Tags
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol: FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol+Type
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol: Int32 CompareTo(FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol)
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol: Int32 CompareTo(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol: Int32 Tag
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol: Int32 get_Tag()
FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol: System.String ToString()
FSharp.Compiler.SourceCodeServices.FSharpField
FSharp.Compiler.SourceCodeServices.FSharpField: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpField: Boolean IsAnonRecordField
FSharp.Compiler.SourceCodeServices.FSharpField: Boolean IsCompilerGenerated
FSharp.Compiler.SourceCodeServices.FSharpField: Boolean IsDefaultValue
FSharp.Compiler.SourceCodeServices.FSharpField: Boolean IsLiteral
FSharp.Compiler.SourceCodeServices.FSharpField: Boolean IsMutable
FSharp.Compiler.SourceCodeServices.FSharpField: Boolean IsNameGenerated
FSharp.Compiler.SourceCodeServices.FSharpField: Boolean IsStatic
FSharp.Compiler.SourceCodeServices.FSharpField: Boolean IsUnionCaseField
FSharp.Compiler.SourceCodeServices.FSharpField: Boolean IsUnresolved
FSharp.Compiler.SourceCodeServices.FSharpField: Boolean IsVolatile
FSharp.Compiler.SourceCodeServices.FSharpField: Boolean get_IsAnonRecordField()
FSharp.Compiler.SourceCodeServices.FSharpField: Boolean get_IsCompilerGenerated()
FSharp.Compiler.SourceCodeServices.FSharpField: Boolean get_IsDefaultValue()
FSharp.Compiler.SourceCodeServices.FSharpField: Boolean get_IsLiteral()
FSharp.Compiler.SourceCodeServices.FSharpField: Boolean get_IsMutable()
FSharp.Compiler.SourceCodeServices.FSharpField: Boolean get_IsNameGenerated()
FSharp.Compiler.SourceCodeServices.FSharpField: Boolean get_IsStatic()
FSharp.Compiler.SourceCodeServices.FSharpField: Boolean get_IsUnionCaseField()
FSharp.Compiler.SourceCodeServices.FSharpField: Boolean get_IsUnresolved()
FSharp.Compiler.SourceCodeServices.FSharpField: Boolean get_IsVolatile()
FSharp.Compiler.SourceCodeServices.FSharpField: FSharp.Compiler.SourceCodeServices.FSharpAccessibility Accessibility
FSharp.Compiler.SourceCodeServices.FSharpField: FSharp.Compiler.SourceCodeServices.FSharpAccessibility get_Accessibility()
FSharp.Compiler.SourceCodeServices.FSharpField: FSharp.Compiler.SourceCodeServices.FSharpType FieldType
FSharp.Compiler.SourceCodeServices.FSharpField: FSharp.Compiler.SourceCodeServices.FSharpType get_FieldType()
FSharp.Compiler.SourceCodeServices.FSharpField: FSharp.Compiler.Text.Range DeclarationLocation
FSharp.Compiler.SourceCodeServices.FSharpField: FSharp.Compiler.Text.Range get_DeclarationLocation()
FSharp.Compiler.SourceCodeServices.FSharpField: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.FSharpField: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpEntity] DeclaringEntity
FSharp.Compiler.SourceCodeServices.FSharpField: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpEntity] get_DeclaringEntity()
FSharp.Compiler.SourceCodeServices.FSharpField: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpUnionCase] DeclaringUnionCase
FSharp.Compiler.SourceCodeServices.FSharpField: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpUnionCase] get_DeclaringUnionCase()
FSharp.Compiler.SourceCodeServices.FSharpField: Microsoft.FSharp.Core.FSharpOption`1[System.Object] LiteralValue
FSharp.Compiler.SourceCodeServices.FSharpField: Microsoft.FSharp.Core.FSharpOption`1[System.Object] get_LiteralValue()
FSharp.Compiler.SourceCodeServices.FSharpField: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpAttribute] FieldAttributes
FSharp.Compiler.SourceCodeServices.FSharpField: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpAttribute] PropertyAttributes
FSharp.Compiler.SourceCodeServices.FSharpField: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpAttribute] get_FieldAttributes()
FSharp.Compiler.SourceCodeServices.FSharpField: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpAttribute] get_PropertyAttributes()
FSharp.Compiler.SourceCodeServices.FSharpField: System.Collections.Generic.IList`1[System.String] ElaboratedXmlDoc
FSharp.Compiler.SourceCodeServices.FSharpField: System.Collections.Generic.IList`1[System.String] XmlDoc
FSharp.Compiler.SourceCodeServices.FSharpField: System.Collections.Generic.IList`1[System.String] get_ElaboratedXmlDoc()
FSharp.Compiler.SourceCodeServices.FSharpField: System.Collections.Generic.IList`1[System.String] get_XmlDoc()
FSharp.Compiler.SourceCodeServices.FSharpField: System.String Name
FSharp.Compiler.SourceCodeServices.FSharpField: System.String ToString()
FSharp.Compiler.SourceCodeServices.FSharpField: System.String XmlDocSig
FSharp.Compiler.SourceCodeServices.FSharpField: System.String get_Name()
FSharp.Compiler.SourceCodeServices.FSharpField: System.String get_XmlDocSig()
FSharp.Compiler.SourceCodeServices.FSharpField: System.Tuple`3[FSharp.Compiler.SourceCodeServices.FSharpAnonRecordTypeDetails,FSharp.Compiler.SourceCodeServices.FSharpType[],System.Int32] AnonRecordFieldDetails
FSharp.Compiler.SourceCodeServices.FSharpField: System.Tuple`3[FSharp.Compiler.SourceCodeServices.FSharpAnonRecordTypeDetails,FSharp.Compiler.SourceCodeServices.FSharpType[],System.Int32] get_AnonRecordFieldDetails()
FSharp.Compiler.SourceCodeServices.FSharpFileUtilities
FSharp.Compiler.SourceCodeServices.FSharpFileUtilities: Boolean isScriptFile(System.String)
FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason
FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason+ProvidedMember: System.String get_memberName()
FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason+ProvidedMember: System.String memberName
FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason+ProvidedType: System.String get_typeName()
FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason+ProvidedType: System.String typeName
FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason+Tags: Int32 NoSourceCode
FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason+Tags: Int32 ProvidedMember
FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason+Tags: Int32 ProvidedType
FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason+Tags: Int32 Unknown
FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason+Unknown: System.String get_message()
FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason+Unknown: System.String message
FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason: Boolean Equals(FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason)
FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason: Boolean IsNoSourceCode
FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason: Boolean IsProvidedMember
FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason: Boolean IsProvidedType
FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason: Boolean IsUnknown
FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason: Boolean get_IsNoSourceCode()
FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason: Boolean get_IsProvidedMember()
FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason: Boolean get_IsProvidedType()
FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason: Boolean get_IsUnknown()
FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason: FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason NewProvidedMember(System.String)
FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason: FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason NewProvidedType(System.String)
FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason: FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason NewUnknown(System.String)
FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason: FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason NoSourceCode
FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason: FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason get_NoSourceCode()
FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason: FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason+ProvidedMember
FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason: FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason+ProvidedType
FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason: FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason+Tags
FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason: FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason+Unknown
FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason: Int32 CompareTo(FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason)
FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason: Int32 CompareTo(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason: Int32 Tag
FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason: Int32 get_Tag()
FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason: System.String ToString()
FSharp.Compiler.SourceCodeServices.FSharpFindDeclResult
FSharp.Compiler.SourceCodeServices.FSharpFindDeclResult+DeclFound: FSharp.Compiler.Text.Range get_location()
FSharp.Compiler.SourceCodeServices.FSharpFindDeclResult+DeclFound: FSharp.Compiler.Text.Range location
FSharp.Compiler.SourceCodeServices.FSharpFindDeclResult+DeclNotFound: FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason Item
FSharp.Compiler.SourceCodeServices.FSharpFindDeclResult+DeclNotFound: FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason get_Item()
FSharp.Compiler.SourceCodeServices.FSharpFindDeclResult+ExternalDecl: FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol externalSym
FSharp.Compiler.SourceCodeServices.FSharpFindDeclResult+ExternalDecl: FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol get_externalSym()
FSharp.Compiler.SourceCodeServices.FSharpFindDeclResult+ExternalDecl: System.String assembly
FSharp.Compiler.SourceCodeServices.FSharpFindDeclResult+ExternalDecl: System.String get_assembly()
FSharp.Compiler.SourceCodeServices.FSharpFindDeclResult+Tags: Int32 DeclFound
FSharp.Compiler.SourceCodeServices.FSharpFindDeclResult+Tags: Int32 DeclNotFound
FSharp.Compiler.SourceCodeServices.FSharpFindDeclResult+Tags: Int32 ExternalDecl
FSharp.Compiler.SourceCodeServices.FSharpFindDeclResult: Boolean Equals(FSharp.Compiler.SourceCodeServices.FSharpFindDeclResult)
FSharp.Compiler.SourceCodeServices.FSharpFindDeclResult: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpFindDeclResult: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.FSharpFindDeclResult: Boolean IsDeclFound
FSharp.Compiler.SourceCodeServices.FSharpFindDeclResult: Boolean IsDeclNotFound
FSharp.Compiler.SourceCodeServices.FSharpFindDeclResult: Boolean IsExternalDecl
FSharp.Compiler.SourceCodeServices.FSharpFindDeclResult: Boolean get_IsDeclFound()
FSharp.Compiler.SourceCodeServices.FSharpFindDeclResult: Boolean get_IsDeclNotFound()
FSharp.Compiler.SourceCodeServices.FSharpFindDeclResult: Boolean get_IsExternalDecl()
FSharp.Compiler.SourceCodeServices.FSharpFindDeclResult: FSharp.Compiler.SourceCodeServices.FSharpFindDeclResult NewDeclFound(FSharp.Compiler.Text.Range)
FSharp.Compiler.SourceCodeServices.FSharpFindDeclResult: FSharp.Compiler.SourceCodeServices.FSharpFindDeclResult NewDeclNotFound(FSharp.Compiler.SourceCodeServices.FSharpFindDeclFailureReason)
FSharp.Compiler.SourceCodeServices.FSharpFindDeclResult: FSharp.Compiler.SourceCodeServices.FSharpFindDeclResult NewExternalDecl(System.String, FSharp.Compiler.SourceCodeServices.FSharpExternalSymbol)
FSharp.Compiler.SourceCodeServices.FSharpFindDeclResult: FSharp.Compiler.SourceCodeServices.FSharpFindDeclResult+DeclFound
FSharp.Compiler.SourceCodeServices.FSharpFindDeclResult: FSharp.Compiler.SourceCodeServices.FSharpFindDeclResult+DeclNotFound
FSharp.Compiler.SourceCodeServices.FSharpFindDeclResult: FSharp.Compiler.SourceCodeServices.FSharpFindDeclResult+ExternalDecl
FSharp.Compiler.SourceCodeServices.FSharpFindDeclResult: FSharp.Compiler.SourceCodeServices.FSharpFindDeclResult+Tags
FSharp.Compiler.SourceCodeServices.FSharpFindDeclResult: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.FSharpFindDeclResult: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.FSharpFindDeclResult: Int32 Tag
FSharp.Compiler.SourceCodeServices.FSharpFindDeclResult: Int32 get_Tag()
FSharp.Compiler.SourceCodeServices.FSharpFindDeclResult: System.String ToString()
FSharp.Compiler.SourceCodeServices.FSharpGenericParameter
FSharp.Compiler.SourceCodeServices.FSharpGenericParameter: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpGenericParameter: Boolean IsCompilerGenerated
FSharp.Compiler.SourceCodeServices.FSharpGenericParameter: Boolean IsMeasure
FSharp.Compiler.SourceCodeServices.FSharpGenericParameter: Boolean IsSolveAtCompileTime
FSharp.Compiler.SourceCodeServices.FSharpGenericParameter: Boolean get_IsCompilerGenerated()
FSharp.Compiler.SourceCodeServices.FSharpGenericParameter: Boolean get_IsMeasure()
FSharp.Compiler.SourceCodeServices.FSharpGenericParameter: Boolean get_IsSolveAtCompileTime()
FSharp.Compiler.SourceCodeServices.FSharpGenericParameter: FSharp.Compiler.Text.Range DeclarationLocation
FSharp.Compiler.SourceCodeServices.FSharpGenericParameter: FSharp.Compiler.Text.Range get_DeclarationLocation()
FSharp.Compiler.SourceCodeServices.FSharpGenericParameter: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.FSharpGenericParameter: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpAttribute] Attributes
FSharp.Compiler.SourceCodeServices.FSharpGenericParameter: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpAttribute] get_Attributes()
FSharp.Compiler.SourceCodeServices.FSharpGenericParameter: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpGenericParameterConstraint] Constraints
FSharp.Compiler.SourceCodeServices.FSharpGenericParameter: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpGenericParameterConstraint] get_Constraints()
FSharp.Compiler.SourceCodeServices.FSharpGenericParameter: System.Collections.Generic.IList`1[System.String] ElaboratedXmlDoc
FSharp.Compiler.SourceCodeServices.FSharpGenericParameter: System.Collections.Generic.IList`1[System.String] XmlDoc
FSharp.Compiler.SourceCodeServices.FSharpGenericParameter: System.Collections.Generic.IList`1[System.String] get_ElaboratedXmlDoc()
FSharp.Compiler.SourceCodeServices.FSharpGenericParameter: System.Collections.Generic.IList`1[System.String] get_XmlDoc()
FSharp.Compiler.SourceCodeServices.FSharpGenericParameter: System.String Name
FSharp.Compiler.SourceCodeServices.FSharpGenericParameter: System.String ToString()
FSharp.Compiler.SourceCodeServices.FSharpGenericParameter: System.String get_Name()
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterConstraint
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterConstraint: Boolean IsCoercesToConstraint
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterConstraint: Boolean IsComparisonConstraint
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterConstraint: Boolean IsDefaultsToConstraint
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterConstraint: Boolean IsDelegateConstraint
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterConstraint: Boolean IsEnumConstraint
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterConstraint: Boolean IsEqualityConstraint
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterConstraint: Boolean IsMemberConstraint
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterConstraint: Boolean IsNonNullableValueTypeConstraint
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterConstraint: Boolean IsReferenceTypeConstraint
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterConstraint: Boolean IsRequiresDefaultConstructorConstraint
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterConstraint: Boolean IsSimpleChoiceConstraint
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterConstraint: Boolean IsSupportsNullConstraint
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterConstraint: Boolean IsUnmanagedConstraint
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterConstraint: Boolean get_IsCoercesToConstraint()
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterConstraint: Boolean get_IsComparisonConstraint()
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterConstraint: Boolean get_IsDefaultsToConstraint()
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterConstraint: Boolean get_IsDelegateConstraint()
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterConstraint: Boolean get_IsEnumConstraint()
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterConstraint: Boolean get_IsEqualityConstraint()
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterConstraint: Boolean get_IsMemberConstraint()
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterConstraint: Boolean get_IsNonNullableValueTypeConstraint()
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterConstraint: Boolean get_IsReferenceTypeConstraint()
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterConstraint: Boolean get_IsRequiresDefaultConstructorConstraint()
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterConstraint: Boolean get_IsSimpleChoiceConstraint()
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterConstraint: Boolean get_IsSupportsNullConstraint()
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterConstraint: Boolean get_IsUnmanagedConstraint()
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterConstraint: FSharp.Compiler.SourceCodeServices.FSharpGenericParameterDefaultsToConstraint DefaultsToConstraintData
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterConstraint: FSharp.Compiler.SourceCodeServices.FSharpGenericParameterDefaultsToConstraint get_DefaultsToConstraintData()
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterConstraint: FSharp.Compiler.SourceCodeServices.FSharpGenericParameterDelegateConstraint DelegateConstraintData
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterConstraint: FSharp.Compiler.SourceCodeServices.FSharpGenericParameterDelegateConstraint get_DelegateConstraintData()
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterConstraint: FSharp.Compiler.SourceCodeServices.FSharpGenericParameterMemberConstraint MemberConstraintData
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterConstraint: FSharp.Compiler.SourceCodeServices.FSharpGenericParameterMemberConstraint get_MemberConstraintData()
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterConstraint: FSharp.Compiler.SourceCodeServices.FSharpType CoercesToTarget
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterConstraint: FSharp.Compiler.SourceCodeServices.FSharpType EnumConstraintTarget
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterConstraint: FSharp.Compiler.SourceCodeServices.FSharpType get_CoercesToTarget()
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterConstraint: FSharp.Compiler.SourceCodeServices.FSharpType get_EnumConstraintTarget()
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterConstraint: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpType] SimpleChoices
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterConstraint: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpType] get_SimpleChoices()
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterConstraint: System.String ToString()
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterDefaultsToConstraint
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterDefaultsToConstraint: FSharp.Compiler.SourceCodeServices.FSharpType DefaultsToTarget
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterDefaultsToConstraint: FSharp.Compiler.SourceCodeServices.FSharpType get_DefaultsToTarget()
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterDefaultsToConstraint: Int32 DefaultsToPriority
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterDefaultsToConstraint: Int32 get_DefaultsToPriority()
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterDefaultsToConstraint: System.String ToString()
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterDelegateConstraint
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterDelegateConstraint: FSharp.Compiler.SourceCodeServices.FSharpType DelegateReturnType
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterDelegateConstraint: FSharp.Compiler.SourceCodeServices.FSharpType DelegateTupledArgumentType
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterDelegateConstraint: FSharp.Compiler.SourceCodeServices.FSharpType get_DelegateReturnType()
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterDelegateConstraint: FSharp.Compiler.SourceCodeServices.FSharpType get_DelegateTupledArgumentType()
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterDelegateConstraint: System.String ToString()
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterMemberConstraint
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterMemberConstraint: Boolean MemberIsStatic
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterMemberConstraint: Boolean get_MemberIsStatic()
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterMemberConstraint: FSharp.Compiler.SourceCodeServices.FSharpType MemberReturnType
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterMemberConstraint: FSharp.Compiler.SourceCodeServices.FSharpType get_MemberReturnType()
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterMemberConstraint: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpType] MemberArgumentTypes
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterMemberConstraint: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpType] MemberSources
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterMemberConstraint: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpType] get_MemberArgumentTypes()
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterMemberConstraint: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpType] get_MemberSources()
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterMemberConstraint: System.String MemberName
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterMemberConstraint: System.String ToString()
FSharp.Compiler.SourceCodeServices.FSharpGenericParameterMemberConstraint: System.String get_MemberName()
FSharp.Compiler.SourceCodeServices.FSharpGlyph
FSharp.Compiler.SourceCodeServices.FSharpGlyph+Tags: Int32 Class
FSharp.Compiler.SourceCodeServices.FSharpGlyph+Tags: Int32 Constant
FSharp.Compiler.SourceCodeServices.FSharpGlyph+Tags: Int32 Delegate
FSharp.Compiler.SourceCodeServices.FSharpGlyph+Tags: Int32 Enum
FSharp.Compiler.SourceCodeServices.FSharpGlyph+Tags: Int32 EnumMember
FSharp.Compiler.SourceCodeServices.FSharpGlyph+Tags: Int32 Error
FSharp.Compiler.SourceCodeServices.FSharpGlyph+Tags: Int32 Event
FSharp.Compiler.SourceCodeServices.FSharpGlyph+Tags: Int32 Exception
FSharp.Compiler.SourceCodeServices.FSharpGlyph+Tags: Int32 ExtensionMethod
FSharp.Compiler.SourceCodeServices.FSharpGlyph+Tags: Int32 Field
FSharp.Compiler.SourceCodeServices.FSharpGlyph+Tags: Int32 Interface
FSharp.Compiler.SourceCodeServices.FSharpGlyph+Tags: Int32 Method
FSharp.Compiler.SourceCodeServices.FSharpGlyph+Tags: Int32 Module
FSharp.Compiler.SourceCodeServices.FSharpGlyph+Tags: Int32 NameSpace
FSharp.Compiler.SourceCodeServices.FSharpGlyph+Tags: Int32 OverridenMethod
FSharp.Compiler.SourceCodeServices.FSharpGlyph+Tags: Int32 Property
FSharp.Compiler.SourceCodeServices.FSharpGlyph+Tags: Int32 Struct
FSharp.Compiler.SourceCodeServices.FSharpGlyph+Tags: Int32 Type
FSharp.Compiler.SourceCodeServices.FSharpGlyph+Tags: Int32 Typedef
FSharp.Compiler.SourceCodeServices.FSharpGlyph+Tags: Int32 Union
FSharp.Compiler.SourceCodeServices.FSharpGlyph+Tags: Int32 Variable
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Boolean Equals(FSharp.Compiler.SourceCodeServices.FSharpGlyph)
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Boolean IsClass
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Boolean IsConstant
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Boolean IsDelegate
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Boolean IsEnum
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Boolean IsEnumMember
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Boolean IsError
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Boolean IsEvent
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Boolean IsException
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Boolean IsExtensionMethod
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Boolean IsField
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Boolean IsInterface
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Boolean IsMethod
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Boolean IsModule
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Boolean IsNameSpace
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Boolean IsOverridenMethod
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Boolean IsProperty
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Boolean IsStruct
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Boolean IsType
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Boolean IsTypedef
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Boolean IsUnion
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Boolean IsVariable
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Boolean get_IsClass()
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Boolean get_IsConstant()
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Boolean get_IsDelegate()
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Boolean get_IsEnum()
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Boolean get_IsEnumMember()
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Boolean get_IsError()
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Boolean get_IsEvent()
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Boolean get_IsException()
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Boolean get_IsExtensionMethod()
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Boolean get_IsField()
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Boolean get_IsInterface()
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Boolean get_IsMethod()
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Boolean get_IsModule()
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Boolean get_IsNameSpace()
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Boolean get_IsOverridenMethod()
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Boolean get_IsProperty()
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Boolean get_IsStruct()
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Boolean get_IsType()
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Boolean get_IsTypedef()
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Boolean get_IsUnion()
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Boolean get_IsVariable()
FSharp.Compiler.SourceCodeServices.FSharpGlyph: FSharp.Compiler.SourceCodeServices.FSharpGlyph Class
FSharp.Compiler.SourceCodeServices.FSharpGlyph: FSharp.Compiler.SourceCodeServices.FSharpGlyph Constant
FSharp.Compiler.SourceCodeServices.FSharpGlyph: FSharp.Compiler.SourceCodeServices.FSharpGlyph Delegate
FSharp.Compiler.SourceCodeServices.FSharpGlyph: FSharp.Compiler.SourceCodeServices.FSharpGlyph Enum
FSharp.Compiler.SourceCodeServices.FSharpGlyph: FSharp.Compiler.SourceCodeServices.FSharpGlyph EnumMember
FSharp.Compiler.SourceCodeServices.FSharpGlyph: FSharp.Compiler.SourceCodeServices.FSharpGlyph Error
FSharp.Compiler.SourceCodeServices.FSharpGlyph: FSharp.Compiler.SourceCodeServices.FSharpGlyph Event
FSharp.Compiler.SourceCodeServices.FSharpGlyph: FSharp.Compiler.SourceCodeServices.FSharpGlyph Exception
FSharp.Compiler.SourceCodeServices.FSharpGlyph: FSharp.Compiler.SourceCodeServices.FSharpGlyph ExtensionMethod
FSharp.Compiler.SourceCodeServices.FSharpGlyph: FSharp.Compiler.SourceCodeServices.FSharpGlyph Field
FSharp.Compiler.SourceCodeServices.FSharpGlyph: FSharp.Compiler.SourceCodeServices.FSharpGlyph Interface
FSharp.Compiler.SourceCodeServices.FSharpGlyph: FSharp.Compiler.SourceCodeServices.FSharpGlyph Method
FSharp.Compiler.SourceCodeServices.FSharpGlyph: FSharp.Compiler.SourceCodeServices.FSharpGlyph Module
FSharp.Compiler.SourceCodeServices.FSharpGlyph: FSharp.Compiler.SourceCodeServices.FSharpGlyph NameSpace
FSharp.Compiler.SourceCodeServices.FSharpGlyph: FSharp.Compiler.SourceCodeServices.FSharpGlyph OverridenMethod
FSharp.Compiler.SourceCodeServices.FSharpGlyph: FSharp.Compiler.SourceCodeServices.FSharpGlyph Property
FSharp.Compiler.SourceCodeServices.FSharpGlyph: FSharp.Compiler.SourceCodeServices.FSharpGlyph Struct
FSharp.Compiler.SourceCodeServices.FSharpGlyph: FSharp.Compiler.SourceCodeServices.FSharpGlyph Type
FSharp.Compiler.SourceCodeServices.FSharpGlyph: FSharp.Compiler.SourceCodeServices.FSharpGlyph Typedef
FSharp.Compiler.SourceCodeServices.FSharpGlyph: FSharp.Compiler.SourceCodeServices.FSharpGlyph Union
FSharp.Compiler.SourceCodeServices.FSharpGlyph: FSharp.Compiler.SourceCodeServices.FSharpGlyph Variable
FSharp.Compiler.SourceCodeServices.FSharpGlyph: FSharp.Compiler.SourceCodeServices.FSharpGlyph get_Class()
FSharp.Compiler.SourceCodeServices.FSharpGlyph: FSharp.Compiler.SourceCodeServices.FSharpGlyph get_Constant()
FSharp.Compiler.SourceCodeServices.FSharpGlyph: FSharp.Compiler.SourceCodeServices.FSharpGlyph get_Delegate()
FSharp.Compiler.SourceCodeServices.FSharpGlyph: FSharp.Compiler.SourceCodeServices.FSharpGlyph get_Enum()
FSharp.Compiler.SourceCodeServices.FSharpGlyph: FSharp.Compiler.SourceCodeServices.FSharpGlyph get_EnumMember()
FSharp.Compiler.SourceCodeServices.FSharpGlyph: FSharp.Compiler.SourceCodeServices.FSharpGlyph get_Error()
FSharp.Compiler.SourceCodeServices.FSharpGlyph: FSharp.Compiler.SourceCodeServices.FSharpGlyph get_Event()
FSharp.Compiler.SourceCodeServices.FSharpGlyph: FSharp.Compiler.SourceCodeServices.FSharpGlyph get_Exception()
FSharp.Compiler.SourceCodeServices.FSharpGlyph: FSharp.Compiler.SourceCodeServices.FSharpGlyph get_ExtensionMethod()
FSharp.Compiler.SourceCodeServices.FSharpGlyph: FSharp.Compiler.SourceCodeServices.FSharpGlyph get_Field()
FSharp.Compiler.SourceCodeServices.FSharpGlyph: FSharp.Compiler.SourceCodeServices.FSharpGlyph get_Interface()
FSharp.Compiler.SourceCodeServices.FSharpGlyph: FSharp.Compiler.SourceCodeServices.FSharpGlyph get_Method()
FSharp.Compiler.SourceCodeServices.FSharpGlyph: FSharp.Compiler.SourceCodeServices.FSharpGlyph get_Module()
FSharp.Compiler.SourceCodeServices.FSharpGlyph: FSharp.Compiler.SourceCodeServices.FSharpGlyph get_NameSpace()
FSharp.Compiler.SourceCodeServices.FSharpGlyph: FSharp.Compiler.SourceCodeServices.FSharpGlyph get_OverridenMethod()
FSharp.Compiler.SourceCodeServices.FSharpGlyph: FSharp.Compiler.SourceCodeServices.FSharpGlyph get_Property()
FSharp.Compiler.SourceCodeServices.FSharpGlyph: FSharp.Compiler.SourceCodeServices.FSharpGlyph get_Struct()
FSharp.Compiler.SourceCodeServices.FSharpGlyph: FSharp.Compiler.SourceCodeServices.FSharpGlyph get_Type()
FSharp.Compiler.SourceCodeServices.FSharpGlyph: FSharp.Compiler.SourceCodeServices.FSharpGlyph get_Typedef()
FSharp.Compiler.SourceCodeServices.FSharpGlyph: FSharp.Compiler.SourceCodeServices.FSharpGlyph get_Union()
FSharp.Compiler.SourceCodeServices.FSharpGlyph: FSharp.Compiler.SourceCodeServices.FSharpGlyph get_Variable()
FSharp.Compiler.SourceCodeServices.FSharpGlyph: FSharp.Compiler.SourceCodeServices.FSharpGlyph+Tags
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Int32 CompareTo(FSharp.Compiler.SourceCodeServices.FSharpGlyph)
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Int32 CompareTo(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Int32 Tag
FSharp.Compiler.SourceCodeServices.FSharpGlyph: Int32 get_Tag()
FSharp.Compiler.SourceCodeServices.FSharpGlyph: System.String ToString()
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileContents
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileContents: Boolean HasExplicitEntryPoint
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileContents: Boolean IsScript
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileContents: Boolean get_HasExplicitEntryPoint()
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileContents: Boolean get_IsScript()
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileContents: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration] Declarations
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileContents: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration] get_Declarations()
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileContents: System.String FileName
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileContents: System.String QualifiedName
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileContents: System.String get_FileName()
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileContents: System.String get_QualifiedName()
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration+Entity: FSharp.Compiler.SourceCodeServices.FSharpEntity entity
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration+Entity: FSharp.Compiler.SourceCodeServices.FSharpEntity get_entity()
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration+Entity: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration] declarations
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration+Entity: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration] get_declarations()
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration+InitAction: FSharp.Compiler.SourceCodeServices.FSharpExpr action
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration+InitAction: FSharp.Compiler.SourceCodeServices.FSharpExpr get_action()
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration+MemberOrFunctionOrValue: FSharp.Compiler.SourceCodeServices.FSharpExpr body
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration+MemberOrFunctionOrValue: FSharp.Compiler.SourceCodeServices.FSharpExpr get_body()
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration+MemberOrFunctionOrValue: FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue get_value()
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration+MemberOrFunctionOrValue: FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue value
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration+MemberOrFunctionOrValue: Microsoft.FSharp.Collections.FSharpList`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue]] curriedArgs
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration+MemberOrFunctionOrValue: Microsoft.FSharp.Collections.FSharpList`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue]] get_curriedArgs()
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration+Tags: Int32 Entity
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration+Tags: Int32 InitAction
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration+Tags: Int32 MemberOrFunctionOrValue
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration: Boolean Equals(FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration)
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration: Boolean IsEntity
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration: Boolean IsInitAction
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration: Boolean IsMemberOrFunctionOrValue
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration: Boolean get_IsEntity()
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration: Boolean get_IsInitAction()
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration: Boolean get_IsMemberOrFunctionOrValue()
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration: FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration NewEntity(FSharp.Compiler.SourceCodeServices.FSharpEntity, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration])
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration: FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration NewInitAction(FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration: FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration NewMemberOrFunctionOrValue(FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue, Microsoft.FSharp.Collections.FSharpList`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue]], FSharp.Compiler.SourceCodeServices.FSharpExpr)
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration: FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration+Entity
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration: FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration+InitAction
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration: FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration+MemberOrFunctionOrValue
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration: FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration+Tags
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration: Int32 Tag
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration: Int32 get_Tag()
FSharp.Compiler.SourceCodeServices.FSharpImplementationFileDeclaration: System.String ToString()
FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation
FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation+Tags: Int32 AggressiveInline
FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation+Tags: Int32 AlwaysInline
FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation+Tags: Int32 NeverInline
FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation+Tags: Int32 OptionalInline
FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation+Tags: Int32 PseudoValue
FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation: Boolean Equals(FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation)
FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation: Boolean IsAggressiveInline
FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation: Boolean IsAlwaysInline
FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation: Boolean IsNeverInline
FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation: Boolean IsOptionalInline
FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation: Boolean IsPseudoValue
FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation: Boolean get_IsAggressiveInline()
FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation: Boolean get_IsAlwaysInline()
FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation: Boolean get_IsNeverInline()
FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation: Boolean get_IsOptionalInline()
FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation: Boolean get_IsPseudoValue()
FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation: FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation AggressiveInline
FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation: FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation AlwaysInline
FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation: FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation NeverInline
FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation: FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation OptionalInline
FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation: FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation PseudoValue
FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation: FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation get_AggressiveInline()
FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation: FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation get_AlwaysInline()
FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation: FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation get_NeverInline()
FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation: FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation get_OptionalInline()
FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation: FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation get_PseudoValue()
FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation: FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation+Tags
FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation: Int32 CompareTo(FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation)
FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation: Int32 CompareTo(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation: Int32 Tag
FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation: Int32 get_Tag()
FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation: System.String ToString()
FSharp.Compiler.SourceCodeServices.FSharpKeywords
FSharp.Compiler.SourceCodeServices.FSharpKeywords: Boolean DoesIdentifierNeedQuotation(System.String)
FSharp.Compiler.SourceCodeServices.FSharpKeywords: Microsoft.FSharp.Collections.FSharpList`1[System.String] KeywordNames
FSharp.Compiler.SourceCodeServices.FSharpKeywords: Microsoft.FSharp.Collections.FSharpList`1[System.String] get_KeywordNames()
FSharp.Compiler.SourceCodeServices.FSharpKeywords: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[System.String,System.String]] KeywordsWithDescription
FSharp.Compiler.SourceCodeServices.FSharpKeywords: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[System.String,System.String]] get_KeywordsWithDescription()
FSharp.Compiler.SourceCodeServices.FSharpKeywords: System.String NormalizeIdentifierBackticks(System.String)
FSharp.Compiler.SourceCodeServices.FSharpKeywords: System.String QuoteIdentifierIfNeeded(System.String)
FSharp.Compiler.SourceCodeServices.FSharpLexer
FSharp.Compiler.SourceCodeServices.FSharpLexer: Void Lex(FSharp.Compiler.Text.ISourceText, Microsoft.FSharp.Core.FSharpFunc`2[FSharp.Compiler.SourceCodeServices.FSharpToken,Microsoft.FSharp.Core.Unit], Microsoft.FSharp.Core.FSharpOption`1[System.String], Microsoft.FSharp.Core.FSharpOption`1[System.String], Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Collections.FSharpList`1[System.String]], Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpLexerFlags], Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Collections.FSharpMap`2[System.String,System.String]], Microsoft.FSharp.Core.FSharpOption`1[System.Threading.CancellationToken])
FSharp.Compiler.SourceCodeServices.FSharpLexerFlags
FSharp.Compiler.SourceCodeServices.FSharpLexerFlags: FSharp.Compiler.SourceCodeServices.FSharpLexerFlags Compiling
FSharp.Compiler.SourceCodeServices.FSharpLexerFlags: FSharp.Compiler.SourceCodeServices.FSharpLexerFlags CompilingFSharpCore
FSharp.Compiler.SourceCodeServices.FSharpLexerFlags: FSharp.Compiler.SourceCodeServices.FSharpLexerFlags Default
FSharp.Compiler.SourceCodeServices.FSharpLexerFlags: FSharp.Compiler.SourceCodeServices.FSharpLexerFlags LightSyntaxOn
FSharp.Compiler.SourceCodeServices.FSharpLexerFlags: FSharp.Compiler.SourceCodeServices.FSharpLexerFlags SkipTrivia
FSharp.Compiler.SourceCodeServices.FSharpLexerFlags: FSharp.Compiler.SourceCodeServices.FSharpLexerFlags UseLexFilter
FSharp.Compiler.SourceCodeServices.FSharpLexerFlags: Int32 value__
FSharp.Compiler.SourceCodeServices.FSharpLineTokenizer
FSharp.Compiler.SourceCodeServices.FSharpLineTokenizer: FSharp.Compiler.SourceCodeServices.FSharpTokenizerColorState ColorStateOfLexState(FSharp.Compiler.SourceCodeServices.FSharpTokenizerLexState)
FSharp.Compiler.SourceCodeServices.FSharpLineTokenizer: FSharp.Compiler.SourceCodeServices.FSharpTokenizerLexState LexStateOfColorState(FSharp.Compiler.SourceCodeServices.FSharpTokenizerColorState)
FSharp.Compiler.SourceCodeServices.FSharpLineTokenizer: System.Tuple`2[Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpTokenInfo],FSharp.Compiler.SourceCodeServices.FSharpTokenizerLexState] ScanToken(FSharp.Compiler.SourceCodeServices.FSharpTokenizerLexState)
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean EventIsStandard
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean HasGetterMethod
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean HasSetterMethod
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean IsActivePattern
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean IsBaseValue
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean IsCompilerGenerated
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean IsConstructor
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean IsConstructorThisValue
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean IsDispatchSlot
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean IsEvent
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean IsEventAddMethod
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean IsEventRemoveMethod
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean IsExplicitInterfaceImplementation
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean IsExtensionMember
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean IsGetterMethod
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean IsImplicitConstructor
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean IsInstanceMember
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean IsInstanceMemberInCompiledCode
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean IsMember
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean IsMemberThisValue
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean IsModuleValueOrMember
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean IsMutable
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean IsOverrideOrExplicitInterfaceImplementation
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean IsOverrideOrExplicitMember
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean IsProperty
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean IsPropertyGetterMethod
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean IsPropertySetterMethod
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean IsSetterMethod
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean IsTypeFunction
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean IsUnresolved
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean IsValCompiledAsMethod
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean IsValue
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean get_EventIsStandard()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean get_HasGetterMethod()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean get_HasSetterMethod()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean get_IsActivePattern()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean get_IsBaseValue()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean get_IsCompilerGenerated()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean get_IsConstructor()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean get_IsConstructorThisValue()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean get_IsDispatchSlot()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean get_IsEvent()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean get_IsEventAddMethod()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean get_IsEventRemoveMethod()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean get_IsExplicitInterfaceImplementation()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean get_IsExtensionMember()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean get_IsGetterMethod()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean get_IsImplicitConstructor()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean get_IsInstanceMember()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean get_IsInstanceMemberInCompiledCode()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean get_IsMember()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean get_IsMemberThisValue()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean get_IsModuleValueOrMember()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean get_IsMutable()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean get_IsOverrideOrExplicitInterfaceImplementation()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean get_IsOverrideOrExplicitMember()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean get_IsProperty()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean get_IsPropertyGetterMethod()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean get_IsPropertySetterMethod()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean get_IsSetterMethod()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean get_IsTypeFunction()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean get_IsUnresolved()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean get_IsValCompiledAsMethod()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Boolean get_IsValue()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: FSharp.Compiler.SourceCodeServices.FSharpAccessibility Accessibility
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: FSharp.Compiler.SourceCodeServices.FSharpAccessibility get_Accessibility()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: FSharp.Compiler.SourceCodeServices.FSharpEntity ApparentEnclosingEntity
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: FSharp.Compiler.SourceCodeServices.FSharpEntity get_ApparentEnclosingEntity()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation InlineAnnotation
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: FSharp.Compiler.SourceCodeServices.FSharpInlineAnnotation get_InlineAnnotation()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue EventAddMethod
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue EventRemoveMethod
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue GetterMethod
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue SetterMethod
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue get_EventAddMethod()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue get_EventRemoveMethod()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue get_GetterMethod()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue get_SetterMethod()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: FSharp.Compiler.SourceCodeServices.FSharpParameter ReturnParameter
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: FSharp.Compiler.SourceCodeServices.FSharpParameter get_ReturnParameter()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: FSharp.Compiler.SourceCodeServices.FSharpType EventDelegateType
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: FSharp.Compiler.SourceCodeServices.FSharpType FullType
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: FSharp.Compiler.SourceCodeServices.FSharpType get_EventDelegateType()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: FSharp.Compiler.SourceCodeServices.FSharpType get_FullType()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: FSharp.Compiler.Text.Range DeclarationLocation
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: FSharp.Compiler.Text.Range get_DeclarationLocation()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: FSharp.Compiler.TextLayout.Layout FormatLayout(FSharp.Compiler.SourceCodeServices.FSharpDisplayContext)
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpEntity] DeclaringEntity
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpEntity] get_DeclaringEntity()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue] EventForFSharpProperty
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue] get_EventForFSharpProperty()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpType] FullTypeSafe
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpType] get_FullTypeSafe()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Microsoft.FSharp.Core.FSharpOption`1[System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue]] Overloads(Boolean)
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Microsoft.FSharp.Core.FSharpOption`1[System.Object] LiteralValue
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Microsoft.FSharp.Core.FSharpOption`1[System.Object] get_LiteralValue()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Microsoft.FSharp.Core.FSharpOption`1[System.String[]] TryGetFullCompiledOperatorNameIdents()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Microsoft.FSharp.Core.FSharpOption`1[System.String] TryGetFullDisplayName()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[System.String,System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpParameter]]] GetWitnessPassingInfo()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpAbstractSignature] ImplementedAbstractSignatures
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpAbstractSignature] get_ImplementedAbstractSignatures()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpAttribute] Attributes
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpAttribute] get_Attributes()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpGenericParameter] GenericParameters
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpGenericParameter] get_GenericParameters()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: System.Collections.Generic.IList`1[System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpParameter]] CurriedParameterGroups
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: System.Collections.Generic.IList`1[System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpParameter]] get_CurriedParameterGroups()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: System.Collections.Generic.IList`1[System.String] ElaboratedXmlDoc
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: System.Collections.Generic.IList`1[System.String] XmlDoc
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: System.Collections.Generic.IList`1[System.String] get_ElaboratedXmlDoc()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: System.Collections.Generic.IList`1[System.String] get_XmlDoc()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: System.String CompiledName
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: System.String DisplayName
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: System.String LogicalName
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: System.String ToString()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: System.String XmlDocSig
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: System.String get_CompiledName()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: System.String get_DisplayName()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: System.String get_LogicalName()
FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue: System.String get_XmlDocSig()
FSharp.Compiler.SourceCodeServices.FSharpMethodGroup
FSharp.Compiler.SourceCodeServices.FSharpMethodGroup: FSharp.Compiler.SourceCodeServices.FSharpMethodGroupItem[] Methods
FSharp.Compiler.SourceCodeServices.FSharpMethodGroup: FSharp.Compiler.SourceCodeServices.FSharpMethodGroupItem[] get_Methods()
FSharp.Compiler.SourceCodeServices.FSharpMethodGroup: System.String MethodName
FSharp.Compiler.SourceCodeServices.FSharpMethodGroup: System.String get_MethodName()
FSharp.Compiler.SourceCodeServices.FSharpMethodGroupItem
FSharp.Compiler.SourceCodeServices.FSharpMethodGroupItem: Boolean HasParamArrayArg
FSharp.Compiler.SourceCodeServices.FSharpMethodGroupItem: Boolean HasParameters
FSharp.Compiler.SourceCodeServices.FSharpMethodGroupItem: Boolean get_HasParamArrayArg()
FSharp.Compiler.SourceCodeServices.FSharpMethodGroupItem: Boolean get_HasParameters()
FSharp.Compiler.SourceCodeServices.FSharpMethodGroupItem: FSharp.Compiler.SourceCodeServices.FSharpMethodGroupItemParameter[] Parameters
FSharp.Compiler.SourceCodeServices.FSharpMethodGroupItem: FSharp.Compiler.SourceCodeServices.FSharpMethodGroupItemParameter[] StaticParameters
FSharp.Compiler.SourceCodeServices.FSharpMethodGroupItem: FSharp.Compiler.SourceCodeServices.FSharpMethodGroupItemParameter[] get_Parameters()
FSharp.Compiler.SourceCodeServices.FSharpMethodGroupItem: FSharp.Compiler.SourceCodeServices.FSharpMethodGroupItemParameter[] get_StaticParameters()
FSharp.Compiler.SourceCodeServices.FSharpMethodGroupItem: FSharp.Compiler.SourceCodeServices.FSharpToolTipText`1[FSharp.Compiler.TextLayout.Layout] StructuredDescription
FSharp.Compiler.SourceCodeServices.FSharpMethodGroupItem: FSharp.Compiler.SourceCodeServices.FSharpToolTipText`1[FSharp.Compiler.TextLayout.Layout] get_StructuredDescription()
FSharp.Compiler.SourceCodeServices.FSharpMethodGroupItem: FSharp.Compiler.SourceCodeServices.FSharpToolTipText`1[System.String] Description
FSharp.Compiler.SourceCodeServices.FSharpMethodGroupItem: FSharp.Compiler.SourceCodeServices.FSharpToolTipText`1[System.String] get_Description()
FSharp.Compiler.SourceCodeServices.FSharpMethodGroupItem: FSharp.Compiler.SourceCodeServices.FSharpXmlDoc XmlDoc
FSharp.Compiler.SourceCodeServices.FSharpMethodGroupItem: FSharp.Compiler.SourceCodeServices.FSharpXmlDoc get_XmlDoc()
FSharp.Compiler.SourceCodeServices.FSharpMethodGroupItem: FSharp.Compiler.TextLayout.Layout StructuredReturnTypeText
FSharp.Compiler.SourceCodeServices.FSharpMethodGroupItem: FSharp.Compiler.TextLayout.Layout get_StructuredReturnTypeText()
FSharp.Compiler.SourceCodeServices.FSharpMethodGroupItem: System.String ReturnTypeText
FSharp.Compiler.SourceCodeServices.FSharpMethodGroupItem: System.String get_ReturnTypeText()
FSharp.Compiler.SourceCodeServices.FSharpMethodGroupItemParameter
FSharp.Compiler.SourceCodeServices.FSharpMethodGroupItemParameter: Boolean IsOptional
FSharp.Compiler.SourceCodeServices.FSharpMethodGroupItemParameter: Boolean get_IsOptional()
FSharp.Compiler.SourceCodeServices.FSharpMethodGroupItemParameter: FSharp.Compiler.TextLayout.Layout StructuredDisplay
FSharp.Compiler.SourceCodeServices.FSharpMethodGroupItemParameter: FSharp.Compiler.TextLayout.Layout get_StructuredDisplay()
FSharp.Compiler.SourceCodeServices.FSharpMethodGroupItemParameter: System.String CanonicalTypeTextForSorting
FSharp.Compiler.SourceCodeServices.FSharpMethodGroupItemParameter: System.String Display
FSharp.Compiler.SourceCodeServices.FSharpMethodGroupItemParameter: System.String ParameterName
FSharp.Compiler.SourceCodeServices.FSharpMethodGroupItemParameter: System.String get_CanonicalTypeTextForSorting()
FSharp.Compiler.SourceCodeServices.FSharpMethodGroupItemParameter: System.String get_Display()
FSharp.Compiler.SourceCodeServices.FSharpMethodGroupItemParameter: System.String get_ParameterName()
FSharp.Compiler.SourceCodeServices.FSharpNavigation
FSharp.Compiler.SourceCodeServices.FSharpNavigation: FSharp.Compiler.SourceCodeServices.FSharpNavigationItems getNavigation(ParsedInput)
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItem
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItem: Boolean IsAbstract
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItem: Boolean IsSingleTopLevel
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItem: Boolean get_IsAbstract()
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItem: Boolean get_IsSingleTopLevel()
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItem: FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind EnclosingEntityKind
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItem: FSharp.Compiler.SourceCodeServices.FSharpEnclosingEntityKind get_EnclosingEntityKind()
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItem: FSharp.Compiler.SourceCodeServices.FSharpGlyph Glyph
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItem: FSharp.Compiler.SourceCodeServices.FSharpGlyph get_Glyph()
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItem: FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind Kind
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItem: FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind get_Kind()
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItem: FSharp.Compiler.Text.Range BodyRange
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItem: FSharp.Compiler.Text.Range Range
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItem: FSharp.Compiler.Text.Range get_BodyRange()
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItem: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItem: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess] Access
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItem: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess] get_Access()
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItem: System.String Name
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItem: System.String UniqueName
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItem: System.String get_Name()
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItem: System.String get_UniqueName()
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind+Tags: Int32 ExnDecl
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind+Tags: Int32 FieldDecl
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind+Tags: Int32 MethodDecl
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind+Tags: Int32 ModuleDecl
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind+Tags: Int32 ModuleFileDecl
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind+Tags: Int32 NamespaceDecl
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind+Tags: Int32 OtherDecl
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind+Tags: Int32 PropertyDecl
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind+Tags: Int32 TypeDecl
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: Boolean Equals(FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind)
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: Boolean IsExnDecl
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: Boolean IsFieldDecl
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: Boolean IsMethodDecl
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: Boolean IsModuleDecl
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: Boolean IsModuleFileDecl
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: Boolean IsNamespaceDecl
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: Boolean IsOtherDecl
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: Boolean IsPropertyDecl
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: Boolean IsTypeDecl
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: Boolean get_IsExnDecl()
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: Boolean get_IsFieldDecl()
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: Boolean get_IsMethodDecl()
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: Boolean get_IsModuleDecl()
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: Boolean get_IsModuleFileDecl()
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: Boolean get_IsNamespaceDecl()
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: Boolean get_IsOtherDecl()
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: Boolean get_IsPropertyDecl()
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: Boolean get_IsTypeDecl()
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind ExnDecl
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind FieldDecl
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind MethodDecl
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind ModuleDecl
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind ModuleFileDecl
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind NamespaceDecl
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind OtherDecl
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind PropertyDecl
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind TypeDecl
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind get_ExnDecl()
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind get_FieldDecl()
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind get_MethodDecl()
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind get_ModuleDecl()
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind get_ModuleFileDecl()
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind get_NamespaceDecl()
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind get_OtherDecl()
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind get_PropertyDecl()
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind get_TypeDecl()
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind+Tags
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: Int32 CompareTo(FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind)
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: Int32 CompareTo(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: Int32 Tag
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: Int32 get_Tag()
FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItemKind: System.String ToString()
FSharp.Compiler.SourceCodeServices.FSharpNavigationItems
FSharp.Compiler.SourceCodeServices.FSharpNavigationItems: FSharp.Compiler.SourceCodeServices.FSharpNavigationTopLevelDeclaration[] Declarations
FSharp.Compiler.SourceCodeServices.FSharpNavigationItems: FSharp.Compiler.SourceCodeServices.FSharpNavigationTopLevelDeclaration[] get_Declarations()
FSharp.Compiler.SourceCodeServices.FSharpNavigationTopLevelDeclaration
FSharp.Compiler.SourceCodeServices.FSharpNavigationTopLevelDeclaration: FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItem Declaration
FSharp.Compiler.SourceCodeServices.FSharpNavigationTopLevelDeclaration: FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItem get_Declaration()
FSharp.Compiler.SourceCodeServices.FSharpNavigationTopLevelDeclaration: FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItem[] Nested
FSharp.Compiler.SourceCodeServices.FSharpNavigationTopLevelDeclaration: FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItem[] get_Nested()
FSharp.Compiler.SourceCodeServices.FSharpNavigationTopLevelDeclaration: System.String ToString()
FSharp.Compiler.SourceCodeServices.FSharpNavigationTopLevelDeclaration: Void .ctor(FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItem, FSharp.Compiler.SourceCodeServices.FSharpNavigationDeclarationItem[])
FSharp.Compiler.SourceCodeServices.FSharpNoteworthyParamInfoLocations
FSharp.Compiler.SourceCodeServices.FSharpNoteworthyParamInfoLocations: Boolean IsThereACloseParen
FSharp.Compiler.SourceCodeServices.FSharpNoteworthyParamInfoLocations: Boolean get_IsThereACloseParen()
FSharp.Compiler.SourceCodeServices.FSharpNoteworthyParamInfoLocations: FSharp.Compiler.Text.Pos LongIdEndLocation
FSharp.Compiler.SourceCodeServices.FSharpNoteworthyParamInfoLocations: FSharp.Compiler.Text.Pos LongIdStartLocation
FSharp.Compiler.SourceCodeServices.FSharpNoteworthyParamInfoLocations: FSharp.Compiler.Text.Pos OpenParenLocation
FSharp.Compiler.SourceCodeServices.FSharpNoteworthyParamInfoLocations: FSharp.Compiler.Text.Pos get_LongIdEndLocation()
FSharp.Compiler.SourceCodeServices.FSharpNoteworthyParamInfoLocations: FSharp.Compiler.Text.Pos get_LongIdStartLocation()
FSharp.Compiler.SourceCodeServices.FSharpNoteworthyParamInfoLocations: FSharp.Compiler.Text.Pos get_OpenParenLocation()
FSharp.Compiler.SourceCodeServices.FSharpNoteworthyParamInfoLocations: FSharp.Compiler.Text.Pos[] TupleEndLocations
FSharp.Compiler.SourceCodeServices.FSharpNoteworthyParamInfoLocations: FSharp.Compiler.Text.Pos[] get_TupleEndLocations()
FSharp.Compiler.SourceCodeServices.FSharpNoteworthyParamInfoLocations: Microsoft.FSharp.Collections.FSharpList`1[System.String] LongId
FSharp.Compiler.SourceCodeServices.FSharpNoteworthyParamInfoLocations: Microsoft.FSharp.Collections.FSharpList`1[System.String] get_LongId()
FSharp.Compiler.SourceCodeServices.FSharpNoteworthyParamInfoLocations: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpNoteworthyParamInfoLocations] Find(FSharp.Compiler.Text.Pos, ParsedInput)
FSharp.Compiler.SourceCodeServices.FSharpNoteworthyParamInfoLocations: Microsoft.FSharp.Core.FSharpOption`1[System.String][] NamedParamNames
FSharp.Compiler.SourceCodeServices.FSharpNoteworthyParamInfoLocations: Microsoft.FSharp.Core.FSharpOption`1[System.String][] get_NamedParamNames()
FSharp.Compiler.SourceCodeServices.FSharpObjectExprOverride
FSharp.Compiler.SourceCodeServices.FSharpObjectExprOverride: FSharp.Compiler.SourceCodeServices.FSharpAbstractSignature Signature
FSharp.Compiler.SourceCodeServices.FSharpObjectExprOverride: FSharp.Compiler.SourceCodeServices.FSharpAbstractSignature get_Signature()
FSharp.Compiler.SourceCodeServices.FSharpObjectExprOverride: FSharp.Compiler.SourceCodeServices.FSharpExpr Body
FSharp.Compiler.SourceCodeServices.FSharpObjectExprOverride: FSharp.Compiler.SourceCodeServices.FSharpExpr get_Body()
FSharp.Compiler.SourceCodeServices.FSharpObjectExprOverride: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpGenericParameter] GenericParameters
FSharp.Compiler.SourceCodeServices.FSharpObjectExprOverride: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpGenericParameter] get_GenericParameters()
FSharp.Compiler.SourceCodeServices.FSharpObjectExprOverride: Microsoft.FSharp.Collections.FSharpList`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue]] CurriedParameterGroups
FSharp.Compiler.SourceCodeServices.FSharpObjectExprOverride: Microsoft.FSharp.Collections.FSharpList`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue]] get_CurriedParameterGroups()
FSharp.Compiler.SourceCodeServices.FSharpOpenDeclaration
FSharp.Compiler.SourceCodeServices.FSharpOpenDeclaration: Boolean IsOwnNamespace
FSharp.Compiler.SourceCodeServices.FSharpOpenDeclaration: Boolean get_IsOwnNamespace()
FSharp.Compiler.SourceCodeServices.FSharpOpenDeclaration: FSharp.Compiler.Text.Range AppliedScope
FSharp.Compiler.SourceCodeServices.FSharpOpenDeclaration: FSharp.Compiler.Text.Range get_AppliedScope()
FSharp.Compiler.SourceCodeServices.FSharpOpenDeclaration: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpEntity] Modules
FSharp.Compiler.SourceCodeServices.FSharpOpenDeclaration: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpEntity] get_Modules()
FSharp.Compiler.SourceCodeServices.FSharpOpenDeclaration: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpType] Types
FSharp.Compiler.SourceCodeServices.FSharpOpenDeclaration: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpType] get_Types()
FSharp.Compiler.SourceCodeServices.FSharpOpenDeclaration: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident] LongId
FSharp.Compiler.SourceCodeServices.FSharpOpenDeclaration: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident] get_LongId()
FSharp.Compiler.SourceCodeServices.FSharpOpenDeclaration: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] Range
FSharp.Compiler.SourceCodeServices.FSharpOpenDeclaration: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] get_Range()
FSharp.Compiler.SourceCodeServices.FSharpOpenDeclaration: SynOpenDeclTarget Target
FSharp.Compiler.SourceCodeServices.FSharpOpenDeclaration: SynOpenDeclTarget get_Target()
FSharp.Compiler.SourceCodeServices.FSharpParameter
FSharp.Compiler.SourceCodeServices.FSharpParameter: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpParameter: Boolean IsInArg
FSharp.Compiler.SourceCodeServices.FSharpParameter: Boolean IsOptionalArg
FSharp.Compiler.SourceCodeServices.FSharpParameter: Boolean IsOutArg
FSharp.Compiler.SourceCodeServices.FSharpParameter: Boolean IsParamArrayArg
FSharp.Compiler.SourceCodeServices.FSharpParameter: Boolean get_IsInArg()
FSharp.Compiler.SourceCodeServices.FSharpParameter: Boolean get_IsOptionalArg()
FSharp.Compiler.SourceCodeServices.FSharpParameter: Boolean get_IsOutArg()
FSharp.Compiler.SourceCodeServices.FSharpParameter: Boolean get_IsParamArrayArg()
FSharp.Compiler.SourceCodeServices.FSharpParameter: FSharp.Compiler.SourceCodeServices.FSharpType Type
FSharp.Compiler.SourceCodeServices.FSharpParameter: FSharp.Compiler.SourceCodeServices.FSharpType get_Type()
FSharp.Compiler.SourceCodeServices.FSharpParameter: FSharp.Compiler.Text.Range DeclarationLocation
FSharp.Compiler.SourceCodeServices.FSharpParameter: FSharp.Compiler.Text.Range get_DeclarationLocation()
FSharp.Compiler.SourceCodeServices.FSharpParameter: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.FSharpParameter: Microsoft.FSharp.Core.FSharpOption`1[System.String] Name
FSharp.Compiler.SourceCodeServices.FSharpParameter: Microsoft.FSharp.Core.FSharpOption`1[System.String] get_Name()
FSharp.Compiler.SourceCodeServices.FSharpParameter: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpAttribute] Attributes
FSharp.Compiler.SourceCodeServices.FSharpParameter: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpAttribute] get_Attributes()
FSharp.Compiler.SourceCodeServices.FSharpParameter: System.String ToString()
FSharp.Compiler.SourceCodeServices.FSharpParseFileResults
FSharp.Compiler.SourceCodeServices.FSharpParseFileResults: Boolean IsPosContainedInApplication(FSharp.Compiler.Text.Pos)
FSharp.Compiler.SourceCodeServices.FSharpParseFileResults: Boolean IsPositionContainedInACurriedParameter(FSharp.Compiler.Text.Pos)
FSharp.Compiler.SourceCodeServices.FSharpParseFileResults: Boolean ParseHadErrors
FSharp.Compiler.SourceCodeServices.FSharpParseFileResults: Boolean get_ParseHadErrors()
FSharp.Compiler.SourceCodeServices.FSharpParseFileResults: FSharp.Compiler.SourceCodeServices.FSharpDiagnostic[] Errors
FSharp.Compiler.SourceCodeServices.FSharpParseFileResults: FSharp.Compiler.SourceCodeServices.FSharpDiagnostic[] get_Errors()
FSharp.Compiler.SourceCodeServices.FSharpParseFileResults: FSharp.Compiler.SourceCodeServices.FSharpNavigationItems GetNavigationItems()
FSharp.Compiler.SourceCodeServices.FSharpParseFileResults: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpNoteworthyParamInfoLocations] FindNoteworthyParamInfoLocations(FSharp.Compiler.Text.Pos)
FSharp.Compiler.SourceCodeServices.FSharpParseFileResults: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+ParsedInput] ParseTree
FSharp.Compiler.SourceCodeServices.FSharpParseFileResults: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+ParsedInput] get_ParseTree()
FSharp.Compiler.SourceCodeServices.FSharpParseFileResults: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] TryRangeOfExprInYieldOrReturn(FSharp.Compiler.Text.Pos)
FSharp.Compiler.SourceCodeServices.FSharpParseFileResults: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] TryRangeOfFunctionOrMethodBeingApplied(FSharp.Compiler.Text.Pos)
FSharp.Compiler.SourceCodeServices.FSharpParseFileResults: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] TryRangeOfNameOfNearestOuterBindingContainingPos(FSharp.Compiler.Text.Pos)
FSharp.Compiler.SourceCodeServices.FSharpParseFileResults: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] TryRangeOfRecordExpressionContainingPos(FSharp.Compiler.Text.Pos)
FSharp.Compiler.SourceCodeServices.FSharpParseFileResults: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] TryRangeOfRefCellDereferenceContainingPos(FSharp.Compiler.Text.Pos)
FSharp.Compiler.SourceCodeServices.FSharpParseFileResults: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] ValidateBreakpointLocation(FSharp.Compiler.Text.Pos)
FSharp.Compiler.SourceCodeServices.FSharpParseFileResults: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Text.Range]] GetAllArgumentsForFunctionApplicationAtPostion(FSharp.Compiler.Text.Pos)
FSharp.Compiler.SourceCodeServices.FSharpParseFileResults: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.SyntaxTree+Ident,System.Int32]] TryIdentOfPipelineContainingPosAndNumArgsApplied(FSharp.Compiler.Text.Pos)
FSharp.Compiler.SourceCodeServices.FSharpParseFileResults: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`3[FSharp.Compiler.Text.Range,FSharp.Compiler.Text.Range,FSharp.Compiler.Text.Range]] TryRangeOfParenEnclosingOpEqualsGreaterUsage(FSharp.Compiler.Text.Pos)
FSharp.Compiler.SourceCodeServices.FSharpParseFileResults: System.String FileName
FSharp.Compiler.SourceCodeServices.FSharpParseFileResults: System.String get_FileName()
FSharp.Compiler.SourceCodeServices.FSharpParseFileResults: System.String[] DependencyFiles
FSharp.Compiler.SourceCodeServices.FSharpParseFileResults: System.String[] get_DependencyFiles()
FSharp.Compiler.SourceCodeServices.FSharpParsingOptions
FSharp.Compiler.SourceCodeServices.FSharpParsingOptions: Boolean CompilingFsLib
FSharp.Compiler.SourceCodeServices.FSharpParsingOptions: Boolean Equals(FSharp.Compiler.SourceCodeServices.FSharpParsingOptions)
FSharp.Compiler.SourceCodeServices.FSharpParsingOptions: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpParsingOptions: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.FSharpParsingOptions: Boolean IsExe
FSharp.Compiler.SourceCodeServices.FSharpParsingOptions: Boolean IsInteractive
FSharp.Compiler.SourceCodeServices.FSharpParsingOptions: Boolean get_CompilingFsLib()
FSharp.Compiler.SourceCodeServices.FSharpParsingOptions: Boolean get_IsExe()
FSharp.Compiler.SourceCodeServices.FSharpParsingOptions: Boolean get_IsInteractive()
FSharp.Compiler.SourceCodeServices.FSharpParsingOptions: FSharp.Compiler.SourceCodeServices.FSharpDiagnosticOptions ErrorSeverityOptions
FSharp.Compiler.SourceCodeServices.FSharpParsingOptions: FSharp.Compiler.SourceCodeServices.FSharpDiagnosticOptions get_ErrorSeverityOptions()
FSharp.Compiler.SourceCodeServices.FSharpParsingOptions: FSharp.Compiler.SourceCodeServices.FSharpParsingOptions Default
FSharp.Compiler.SourceCodeServices.FSharpParsingOptions: FSharp.Compiler.SourceCodeServices.FSharpParsingOptions get_Default()
FSharp.Compiler.SourceCodeServices.FSharpParsingOptions: Int32 CompareTo(FSharp.Compiler.SourceCodeServices.FSharpParsingOptions)
FSharp.Compiler.SourceCodeServices.FSharpParsingOptions: Int32 CompareTo(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpParsingOptions: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.SourceCodeServices.FSharpParsingOptions: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.FSharpParsingOptions: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.FSharpParsingOptions: Microsoft.FSharp.Collections.FSharpList`1[System.String] ConditionalCompilationDefines
FSharp.Compiler.SourceCodeServices.FSharpParsingOptions: Microsoft.FSharp.Collections.FSharpList`1[System.String] get_ConditionalCompilationDefines()
FSharp.Compiler.SourceCodeServices.FSharpParsingOptions: Microsoft.FSharp.Core.FSharpOption`1[System.Boolean] LightSyntax
FSharp.Compiler.SourceCodeServices.FSharpParsingOptions: Microsoft.FSharp.Core.FSharpOption`1[System.Boolean] get_LightSyntax()
FSharp.Compiler.SourceCodeServices.FSharpParsingOptions: System.String ToString()
FSharp.Compiler.SourceCodeServices.FSharpParsingOptions: System.String[] SourceFiles
FSharp.Compiler.SourceCodeServices.FSharpParsingOptions: System.String[] get_SourceFiles()
FSharp.Compiler.SourceCodeServices.FSharpParsingOptions: Void .ctor(System.String[], Microsoft.FSharp.Collections.FSharpList`1[System.String], FSharp.Compiler.SourceCodeServices.FSharpDiagnosticOptions, Boolean, Microsoft.FSharp.Core.FSharpOption`1[System.Boolean], Boolean, Boolean)
FSharp.Compiler.SourceCodeServices.FSharpProjectContext
FSharp.Compiler.SourceCodeServices.FSharpProjectContext: FSharp.Compiler.SourceCodeServices.FSharpAccessibilityRights AccessibilityRights
FSharp.Compiler.SourceCodeServices.FSharpProjectContext: FSharp.Compiler.SourceCodeServices.FSharpAccessibilityRights get_AccessibilityRights()
FSharp.Compiler.SourceCodeServices.FSharpProjectContext: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpAssembly] GetReferencedAssemblies()
FSharp.Compiler.SourceCodeServices.FSharpProjectOptions
FSharp.Compiler.SourceCodeServices.FSharpProjectOptions: Boolean Equals(FSharp.Compiler.SourceCodeServices.FSharpProjectOptions)
FSharp.Compiler.SourceCodeServices.FSharpProjectOptions: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpProjectOptions: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.FSharpProjectOptions: Boolean IsIncompleteTypeCheckEnvironment
FSharp.Compiler.SourceCodeServices.FSharpProjectOptions: Boolean UseScriptResolutionRules
FSharp.Compiler.SourceCodeServices.FSharpProjectOptions: Boolean get_IsIncompleteTypeCheckEnvironment()
FSharp.Compiler.SourceCodeServices.FSharpProjectOptions: Boolean get_UseScriptResolutionRules()
FSharp.Compiler.SourceCodeServices.FSharpProjectOptions: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.FSharpProjectOptions: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.FSharpProjectOptions: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`3[FSharp.Compiler.Text.Range,System.String,System.String]] OriginalLoadReferences
FSharp.Compiler.SourceCodeServices.FSharpProjectOptions: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`3[FSharp.Compiler.Text.Range,System.String,System.String]] get_OriginalLoadReferences()
FSharp.Compiler.SourceCodeServices.FSharpProjectOptions: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpUnresolvedReferencesSet] UnresolvedReferences
FSharp.Compiler.SourceCodeServices.FSharpProjectOptions: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpUnresolvedReferencesSet] get_UnresolvedReferences()
FSharp.Compiler.SourceCodeServices.FSharpProjectOptions: Microsoft.FSharp.Core.FSharpOption`1[System.Int64] Stamp
FSharp.Compiler.SourceCodeServices.FSharpProjectOptions: Microsoft.FSharp.Core.FSharpOption`1[System.Int64] get_Stamp()
FSharp.Compiler.SourceCodeServices.FSharpProjectOptions: Microsoft.FSharp.Core.FSharpOption`1[System.Object] ExtraProjectInfo
FSharp.Compiler.SourceCodeServices.FSharpProjectOptions: Microsoft.FSharp.Core.FSharpOption`1[System.Object] get_ExtraProjectInfo()
FSharp.Compiler.SourceCodeServices.FSharpProjectOptions: Microsoft.FSharp.Core.FSharpOption`1[System.String] ProjectId
FSharp.Compiler.SourceCodeServices.FSharpProjectOptions: Microsoft.FSharp.Core.FSharpOption`1[System.String] get_ProjectId()
FSharp.Compiler.SourceCodeServices.FSharpProjectOptions: System.DateTime LoadTime
FSharp.Compiler.SourceCodeServices.FSharpProjectOptions: System.DateTime get_LoadTime()
FSharp.Compiler.SourceCodeServices.FSharpProjectOptions: System.String ProjectFileName
FSharp.Compiler.SourceCodeServices.FSharpProjectOptions: System.String ToString()
FSharp.Compiler.SourceCodeServices.FSharpProjectOptions: System.String get_ProjectFileName()
FSharp.Compiler.SourceCodeServices.FSharpProjectOptions: System.String[] OtherOptions
FSharp.Compiler.SourceCodeServices.FSharpProjectOptions: System.String[] SourceFiles
FSharp.Compiler.SourceCodeServices.FSharpProjectOptions: System.String[] get_OtherOptions()
FSharp.Compiler.SourceCodeServices.FSharpProjectOptions: System.String[] get_SourceFiles()
FSharp.Compiler.SourceCodeServices.FSharpProjectOptions: System.Tuple`2[System.String,FSharp.Compiler.SourceCodeServices.FSharpProjectOptions][] ReferencedProjects
FSharp.Compiler.SourceCodeServices.FSharpProjectOptions: System.Tuple`2[System.String,FSharp.Compiler.SourceCodeServices.FSharpProjectOptions][] get_ReferencedProjects()
FSharp.Compiler.SourceCodeServices.FSharpProjectOptions: Void .ctor(System.String, Microsoft.FSharp.Core.FSharpOption`1[System.String], System.String[], System.String[], System.Tuple`2[System.String,FSharp.Compiler.SourceCodeServices.FSharpProjectOptions][], Boolean, Boolean, System.DateTime, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpUnresolvedReferencesSet], Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`3[FSharp.Compiler.Text.Range,System.String,System.String]], Microsoft.FSharp.Core.FSharpOption`1[System.Object], Microsoft.FSharp.Core.FSharpOption`1[System.Int64])
FSharp.Compiler.SourceCodeServices.FSharpSourceTokenizer
FSharp.Compiler.SourceCodeServices.FSharpSourceTokenizer: FSharp.Compiler.SourceCodeServices.FSharpLineTokenizer CreateBufferTokenizer(Microsoft.FSharp.Core.FSharpFunc`2[System.Tuple`3[System.Char[],System.Int32,System.Int32],System.Int32])
FSharp.Compiler.SourceCodeServices.FSharpSourceTokenizer: FSharp.Compiler.SourceCodeServices.FSharpLineTokenizer CreateLineTokenizer(System.String)
FSharp.Compiler.SourceCodeServices.FSharpSourceTokenizer: Void .ctor(Microsoft.FSharp.Collections.FSharpList`1[System.String], Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.SourceCodeServices.FSharpStaticParameter
FSharp.Compiler.SourceCodeServices.FSharpStaticParameter: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpStaticParameter: Boolean HasDefaultValue
FSharp.Compiler.SourceCodeServices.FSharpStaticParameter: Boolean IsOptional
FSharp.Compiler.SourceCodeServices.FSharpStaticParameter: Boolean get_HasDefaultValue()
FSharp.Compiler.SourceCodeServices.FSharpStaticParameter: Boolean get_IsOptional()
FSharp.Compiler.SourceCodeServices.FSharpStaticParameter: FSharp.Compiler.SourceCodeServices.FSharpType Kind
FSharp.Compiler.SourceCodeServices.FSharpStaticParameter: FSharp.Compiler.SourceCodeServices.FSharpType get_Kind()
FSharp.Compiler.SourceCodeServices.FSharpStaticParameter: FSharp.Compiler.Text.Range DeclarationLocation
FSharp.Compiler.SourceCodeServices.FSharpStaticParameter: FSharp.Compiler.Text.Range Range
FSharp.Compiler.SourceCodeServices.FSharpStaticParameter: FSharp.Compiler.Text.Range get_DeclarationLocation()
FSharp.Compiler.SourceCodeServices.FSharpStaticParameter: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.SourceCodeServices.FSharpStaticParameter: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.FSharpStaticParameter: System.Object DefaultValue
FSharp.Compiler.SourceCodeServices.FSharpStaticParameter: System.Object get_DefaultValue()
FSharp.Compiler.SourceCodeServices.FSharpStaticParameter: System.String Name
FSharp.Compiler.SourceCodeServices.FSharpStaticParameter: System.String ToString()
FSharp.Compiler.SourceCodeServices.FSharpStaticParameter: System.String get_Name()
FSharp.Compiler.SourceCodeServices.FSharpSymbol
FSharp.Compiler.SourceCodeServices.FSharpSymbol: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpSymbol: Boolean IsAccessible(FSharp.Compiler.SourceCodeServices.FSharpAccessibilityRights)
FSharp.Compiler.SourceCodeServices.FSharpSymbol: Boolean IsEffectivelySameAs(FSharp.Compiler.SourceCodeServices.FSharpSymbol)
FSharp.Compiler.SourceCodeServices.FSharpSymbol: Boolean IsExplicitlySuppressed
FSharp.Compiler.SourceCodeServices.FSharpSymbol: Boolean get_IsExplicitlySuppressed()
FSharp.Compiler.SourceCodeServices.FSharpSymbol: FSharp.Compiler.SourceCodeServices.FSharpAssembly Assembly
FSharp.Compiler.SourceCodeServices.FSharpSymbol: FSharp.Compiler.SourceCodeServices.FSharpAssembly get_Assembly()
FSharp.Compiler.SourceCodeServices.FSharpSymbol: Int32 GetEffectivelySameAsHash()
FSharp.Compiler.SourceCodeServices.FSharpSymbol: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.FSharpSymbol: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpAccessibility] GetAccessibility(FSharp.Compiler.SourceCodeServices.FSharpSymbol)
FSharp.Compiler.SourceCodeServices.FSharpSymbol: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] DeclarationLocation
FSharp.Compiler.SourceCodeServices.FSharpSymbol: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] ImplementationLocation
FSharp.Compiler.SourceCodeServices.FSharpSymbol: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] SignatureLocation
FSharp.Compiler.SourceCodeServices.FSharpSymbol: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] get_DeclarationLocation()
FSharp.Compiler.SourceCodeServices.FSharpSymbol: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] get_ImplementationLocation()
FSharp.Compiler.SourceCodeServices.FSharpSymbol: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] get_SignatureLocation()
FSharp.Compiler.SourceCodeServices.FSharpSymbol: System.String DisplayName
FSharp.Compiler.SourceCodeServices.FSharpSymbol: System.String FullName
FSharp.Compiler.SourceCodeServices.FSharpSymbol: System.String ToString()
FSharp.Compiler.SourceCodeServices.FSharpSymbol: System.String get_DisplayName()
FSharp.Compiler.SourceCodeServices.FSharpSymbol: System.String get_FullName()
FSharp.Compiler.SourceCodeServices.FSharpSymbolUse
FSharp.Compiler.SourceCodeServices.FSharpSymbolUse: Boolean IsFromAttribute
FSharp.Compiler.SourceCodeServices.FSharpSymbolUse: Boolean IsFromComputationExpression
FSharp.Compiler.SourceCodeServices.FSharpSymbolUse: Boolean IsFromDefinition
FSharp.Compiler.SourceCodeServices.FSharpSymbolUse: Boolean IsFromDispatchSlotImplementation
FSharp.Compiler.SourceCodeServices.FSharpSymbolUse: Boolean IsFromOpenStatement
FSharp.Compiler.SourceCodeServices.FSharpSymbolUse: Boolean IsFromPattern
FSharp.Compiler.SourceCodeServices.FSharpSymbolUse: Boolean IsFromType
FSharp.Compiler.SourceCodeServices.FSharpSymbolUse: Boolean IsPrivateToFile
FSharp.Compiler.SourceCodeServices.FSharpSymbolUse: Boolean get_IsFromAttribute()
FSharp.Compiler.SourceCodeServices.FSharpSymbolUse: Boolean get_IsFromComputationExpression()
FSharp.Compiler.SourceCodeServices.FSharpSymbolUse: Boolean get_IsFromDefinition()
FSharp.Compiler.SourceCodeServices.FSharpSymbolUse: Boolean get_IsFromDispatchSlotImplementation()
FSharp.Compiler.SourceCodeServices.FSharpSymbolUse: Boolean get_IsFromOpenStatement()
FSharp.Compiler.SourceCodeServices.FSharpSymbolUse: Boolean get_IsFromPattern()
FSharp.Compiler.SourceCodeServices.FSharpSymbolUse: Boolean get_IsFromType()
FSharp.Compiler.SourceCodeServices.FSharpSymbolUse: Boolean get_IsPrivateToFile()
FSharp.Compiler.SourceCodeServices.FSharpSymbolUse: FSharp.Compiler.SourceCodeServices.FSharpDisplayContext DisplayContext
FSharp.Compiler.SourceCodeServices.FSharpSymbolUse: FSharp.Compiler.SourceCodeServices.FSharpDisplayContext get_DisplayContext()
FSharp.Compiler.SourceCodeServices.FSharpSymbolUse: FSharp.Compiler.SourceCodeServices.FSharpSymbol Symbol
FSharp.Compiler.SourceCodeServices.FSharpSymbolUse: FSharp.Compiler.SourceCodeServices.FSharpSymbol get_Symbol()
FSharp.Compiler.SourceCodeServices.FSharpSymbolUse: FSharp.Compiler.Text.Range RangeAlternate
FSharp.Compiler.SourceCodeServices.FSharpSymbolUse: FSharp.Compiler.Text.Range get_RangeAlternate()
FSharp.Compiler.SourceCodeServices.FSharpSymbolUse: System.String FileName
FSharp.Compiler.SourceCodeServices.FSharpSymbolUse: System.String ToString()
FSharp.Compiler.SourceCodeServices.FSharpSymbolUse: System.String get_FileName()
FSharp.Compiler.SourceCodeServices.FSharpToken
FSharp.Compiler.SourceCodeServices.FSharpToken: Boolean IsCommentTrivia
FSharp.Compiler.SourceCodeServices.FSharpToken: Boolean IsIdentifier
FSharp.Compiler.SourceCodeServices.FSharpToken: Boolean IsKeyword
FSharp.Compiler.SourceCodeServices.FSharpToken: Boolean IsNumericLiteral
FSharp.Compiler.SourceCodeServices.FSharpToken: Boolean IsStringLiteral
FSharp.Compiler.SourceCodeServices.FSharpToken: Boolean get_IsCommentTrivia()
FSharp.Compiler.SourceCodeServices.FSharpToken: Boolean get_IsIdentifier()
FSharp.Compiler.SourceCodeServices.FSharpToken: Boolean get_IsKeyword()
FSharp.Compiler.SourceCodeServices.FSharpToken: Boolean get_IsNumericLiteral()
FSharp.Compiler.SourceCodeServices.FSharpToken: Boolean get_IsStringLiteral()
FSharp.Compiler.SourceCodeServices.FSharpToken: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Kind
FSharp.Compiler.SourceCodeServices.FSharpToken: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Kind()
FSharp.Compiler.SourceCodeServices.FSharpToken: FSharp.Compiler.Text.Range Range
FSharp.Compiler.SourceCodeServices.FSharpToken: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.SourceCodeServices.FSharpTokenCharKind
FSharp.Compiler.SourceCodeServices.FSharpTokenCharKind: FSharp.Compiler.SourceCodeServices.FSharpTokenCharKind Comment
FSharp.Compiler.SourceCodeServices.FSharpTokenCharKind: FSharp.Compiler.SourceCodeServices.FSharpTokenCharKind Default
FSharp.Compiler.SourceCodeServices.FSharpTokenCharKind: FSharp.Compiler.SourceCodeServices.FSharpTokenCharKind Delimiter
FSharp.Compiler.SourceCodeServices.FSharpTokenCharKind: FSharp.Compiler.SourceCodeServices.FSharpTokenCharKind Identifier
FSharp.Compiler.SourceCodeServices.FSharpTokenCharKind: FSharp.Compiler.SourceCodeServices.FSharpTokenCharKind Keyword
FSharp.Compiler.SourceCodeServices.FSharpTokenCharKind: FSharp.Compiler.SourceCodeServices.FSharpTokenCharKind LineComment
FSharp.Compiler.SourceCodeServices.FSharpTokenCharKind: FSharp.Compiler.SourceCodeServices.FSharpTokenCharKind Literal
FSharp.Compiler.SourceCodeServices.FSharpTokenCharKind: FSharp.Compiler.SourceCodeServices.FSharpTokenCharKind Operator
FSharp.Compiler.SourceCodeServices.FSharpTokenCharKind: FSharp.Compiler.SourceCodeServices.FSharpTokenCharKind String
FSharp.Compiler.SourceCodeServices.FSharpTokenCharKind: FSharp.Compiler.SourceCodeServices.FSharpTokenCharKind Text
FSharp.Compiler.SourceCodeServices.FSharpTokenCharKind: FSharp.Compiler.SourceCodeServices.FSharpTokenCharKind WhiteSpace
FSharp.Compiler.SourceCodeServices.FSharpTokenCharKind: Int32 value__
FSharp.Compiler.SourceCodeServices.FSharpTokenColorKind
FSharp.Compiler.SourceCodeServices.FSharpTokenColorKind: FSharp.Compiler.SourceCodeServices.FSharpTokenColorKind Comment
FSharp.Compiler.SourceCodeServices.FSharpTokenColorKind: FSharp.Compiler.SourceCodeServices.FSharpTokenColorKind Default
FSharp.Compiler.SourceCodeServices.FSharpTokenColorKind: FSharp.Compiler.SourceCodeServices.FSharpTokenColorKind Identifier
FSharp.Compiler.SourceCodeServices.FSharpTokenColorKind: FSharp.Compiler.SourceCodeServices.FSharpTokenColorKind InactiveCode
FSharp.Compiler.SourceCodeServices.FSharpTokenColorKind: FSharp.Compiler.SourceCodeServices.FSharpTokenColorKind Keyword
FSharp.Compiler.SourceCodeServices.FSharpTokenColorKind: FSharp.Compiler.SourceCodeServices.FSharpTokenColorKind Number
FSharp.Compiler.SourceCodeServices.FSharpTokenColorKind: FSharp.Compiler.SourceCodeServices.FSharpTokenColorKind Operator
FSharp.Compiler.SourceCodeServices.FSharpTokenColorKind: FSharp.Compiler.SourceCodeServices.FSharpTokenColorKind PreprocessorKeyword
FSharp.Compiler.SourceCodeServices.FSharpTokenColorKind: FSharp.Compiler.SourceCodeServices.FSharpTokenColorKind Punctuation
FSharp.Compiler.SourceCodeServices.FSharpTokenColorKind: FSharp.Compiler.SourceCodeServices.FSharpTokenColorKind String
FSharp.Compiler.SourceCodeServices.FSharpTokenColorKind: FSharp.Compiler.SourceCodeServices.FSharpTokenColorKind Text
FSharp.Compiler.SourceCodeServices.FSharpTokenColorKind: FSharp.Compiler.SourceCodeServices.FSharpTokenColorKind UpperIdentifier
FSharp.Compiler.SourceCodeServices.FSharpTokenColorKind: Int32 value__
FSharp.Compiler.SourceCodeServices.FSharpTokenInfo
FSharp.Compiler.SourceCodeServices.FSharpTokenInfo: Boolean Equals(FSharp.Compiler.SourceCodeServices.FSharpTokenInfo)
FSharp.Compiler.SourceCodeServices.FSharpTokenInfo: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpTokenInfo: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.FSharpTokenInfo: FSharp.Compiler.SourceCodeServices.FSharpTokenCharKind CharClass
FSharp.Compiler.SourceCodeServices.FSharpTokenInfo: FSharp.Compiler.SourceCodeServices.FSharpTokenCharKind get_CharClass()
FSharp.Compiler.SourceCodeServices.FSharpTokenInfo: FSharp.Compiler.SourceCodeServices.FSharpTokenColorKind ColorClass
FSharp.Compiler.SourceCodeServices.FSharpTokenInfo: FSharp.Compiler.SourceCodeServices.FSharpTokenColorKind get_ColorClass()
FSharp.Compiler.SourceCodeServices.FSharpTokenInfo: FSharp.Compiler.SourceCodeServices.FSharpTokenTriggerClass FSharpTokenTriggerClass
FSharp.Compiler.SourceCodeServices.FSharpTokenInfo: FSharp.Compiler.SourceCodeServices.FSharpTokenTriggerClass get_FSharpTokenTriggerClass()
FSharp.Compiler.SourceCodeServices.FSharpTokenInfo: Int32 CompareTo(FSharp.Compiler.SourceCodeServices.FSharpTokenInfo)
FSharp.Compiler.SourceCodeServices.FSharpTokenInfo: Int32 CompareTo(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpTokenInfo: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.SourceCodeServices.FSharpTokenInfo: Int32 FullMatchedLength
FSharp.Compiler.SourceCodeServices.FSharpTokenInfo: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.FSharpTokenInfo: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.FSharpTokenInfo: Int32 LeftColumn
FSharp.Compiler.SourceCodeServices.FSharpTokenInfo: Int32 RightColumn
FSharp.Compiler.SourceCodeServices.FSharpTokenInfo: Int32 Tag
FSharp.Compiler.SourceCodeServices.FSharpTokenInfo: Int32 get_FullMatchedLength()
FSharp.Compiler.SourceCodeServices.FSharpTokenInfo: Int32 get_LeftColumn()
FSharp.Compiler.SourceCodeServices.FSharpTokenInfo: Int32 get_RightColumn()
FSharp.Compiler.SourceCodeServices.FSharpTokenInfo: Int32 get_Tag()
FSharp.Compiler.SourceCodeServices.FSharpTokenInfo: System.String ToString()
FSharp.Compiler.SourceCodeServices.FSharpTokenInfo: System.String TokenName
FSharp.Compiler.SourceCodeServices.FSharpTokenInfo: System.String get_TokenName()
FSharp.Compiler.SourceCodeServices.FSharpTokenInfo: Void .ctor(Int32, Int32, FSharp.Compiler.SourceCodeServices.FSharpTokenColorKind, FSharp.Compiler.SourceCodeServices.FSharpTokenCharKind, FSharp.Compiler.SourceCodeServices.FSharpTokenTriggerClass, Int32, System.String, Int32)
FSharp.Compiler.SourceCodeServices.FSharpTokenKind
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Abstract
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 AdjacentPrefixOperator
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Ampersand
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 AmpersandAmpersand
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 And
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 As
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Asr
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Assert
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Bar
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 BarBar
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 BarRightBrace
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 BarRightBracket
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Base
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Begin
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 BigNumber
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Binder
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 ByteArray
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Char
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Class
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Colon
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 ColonColon
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 ColonEquals
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 ColonGreater
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 ColonQuestionMark
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 ColonQuestionMarkGreater
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Comma
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 CommentTrivia
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Const
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Constraint
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Constructor
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Decimal
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Default
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Delegate
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Do
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 DoBang
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Dollar
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Done
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Dot
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 DotDot
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 DotDotHat
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 DownTo
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Downcast
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Elif
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Else
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 End
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Equals
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Exception
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Extern
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 False
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Finally
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Fixed
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 For
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Fun
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Function
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 FunkyOperatorName
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Global
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Greater
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 GreaterBarRightBracket
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 GreaterRightBracket
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Hash
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 HashElse
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 HashEndIf
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 HashIf
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 HashLight
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 HashLine
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 HighPrecedenceBracketApp
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 HighPrecedenceParenthesisApp
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 HighPrecedenceTypeApp
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Identifier
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Ieee32
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Ieee64
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 If
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 In
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 InactiveCode
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 InfixAmpersandOperator
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 InfixAsr
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 InfixAtHatOperator
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 InfixBarOperator
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 InfixCompareOperator
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 InfixLand
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 InfixLor
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 InfixLsl
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 InfixLsr
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 InfixLxor
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 InfixMod
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 InfixStarDivideModuloOperator
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 InfixStarStarOperator
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Inherit
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Inline
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Instance
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Int16
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Int32
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Int32DotDot
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Int64
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Int8
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Interface
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Internal
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 JoinIn
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 KeywordString
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Lazy
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 LeftArrow
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 LeftBrace
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 LeftBraceBar
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 LeftBracket
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 LeftBracketBar
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 LeftBracketLess
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 LeftParenthesis
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 LeftParenthesisStarRightParenthesis
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 LeftQuote
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Less
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Let
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 LineCommentTrivia
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Match
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 MatchBang
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Member
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Minus
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Module
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Mutable
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Namespace
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 NativeInt
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 New
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 None
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Null
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Of
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 OffsideAssert
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 OffsideBinder
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 OffsideBlockBegin
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 OffsideBlockEnd
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 OffsideBlockSep
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 OffsideDeclEnd
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 OffsideDo
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 OffsideDoBang
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 OffsideElse
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 OffsideEnd
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 OffsideFun
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 OffsideFunction
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 OffsideInterfaceMember
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 OffsideLazy
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 OffsideLet
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 OffsideReset
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 OffsideRightBlockEnd
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 OffsideThen
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 OffsideWith
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Open
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Or
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Override
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 PercentOperator
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 PlusMinusOperator
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 PrefixOperator
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Private
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Public
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 QuestionMark
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 QuestionMarkQuestionMark
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Quote
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Rec
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Reserved
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 RightArrow
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 RightBrace
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 RightBracket
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 RightParenthesis
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 RightQuote
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 RightQuoteDot
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Semicolon
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 SemicolonSemicolon
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Sig
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Star
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Static
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 String
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 StringText
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Struct
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Then
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 To
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 True
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Try
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Type
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 UInt16
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 UInt32
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 UInt64
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 UInt8
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 UNativeInt
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Underscore
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Upcast
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Val
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Void
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 When
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 While
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 WhitespaceTrivia
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 With
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 Yield
FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags: Int32 YieldBang
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean Equals(FSharp.Compiler.SourceCodeServices.FSharpTokenKind)
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsAbstract
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsAdjacentPrefixOperator
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsAmpersand
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsAmpersandAmpersand
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsAnd
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsAs
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsAsr
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsAssert
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsBar
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsBarBar
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsBarRightBrace
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsBarRightBracket
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsBase
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsBegin
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsBigNumber
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsBinder
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsByteArray
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsChar
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsClass
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsColon
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsColonColon
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsColonEquals
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsColonGreater
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsColonQuestionMark
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsColonQuestionMarkGreater
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsComma
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsCommentTrivia
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsConst
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsConstraint
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsConstructor
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsDecimal
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsDefault
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsDelegate
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsDo
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsDoBang
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsDollar
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsDone
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsDot
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsDotDot
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsDotDotHat
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsDownTo
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsDowncast
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsElif
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsElse
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsEnd
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsEquals
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsException
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsExtern
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsFalse
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsFinally
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsFixed
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsFor
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsFun
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsFunction
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsFunkyOperatorName
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsGlobal
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsGreater
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsGreaterBarRightBracket
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsGreaterRightBracket
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsHash
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsHashElse
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsHashEndIf
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsHashIf
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsHashLight
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsHashLine
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsHighPrecedenceBracketApp
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsHighPrecedenceParenthesisApp
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsHighPrecedenceTypeApp
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsIdentifier
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsIeee32
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsIeee64
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsIf
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsIn
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsInactiveCode
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsInfixAmpersandOperator
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsInfixAsr
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsInfixAtHatOperator
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsInfixBarOperator
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsInfixCompareOperator
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsInfixLand
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsInfixLor
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsInfixLsl
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsInfixLsr
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsInfixLxor
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsInfixMod
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsInfixStarDivideModuloOperator
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsInfixStarStarOperator
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsInherit
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsInline
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsInstance
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsInt16
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsInt32
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsInt32DotDot
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsInt64
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsInt8
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsInterface
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsInternal
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsJoinIn
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsKeywordString
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsLazy
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsLeftArrow
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsLeftBrace
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsLeftBraceBar
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsLeftBracket
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsLeftBracketBar
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsLeftBracketLess
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsLeftParenthesis
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsLeftParenthesisStarRightParenthesis
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsLeftQuote
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsLess
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsLet
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsLineCommentTrivia
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsMatch
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsMatchBang
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsMember
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsMinus
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsModule
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsMutable
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsNamespace
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsNativeInt
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsNew
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsNone
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsNull
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsOf
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsOffsideAssert
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsOffsideBinder
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsOffsideBlockBegin
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsOffsideBlockEnd
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsOffsideBlockSep
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsOffsideDeclEnd
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsOffsideDo
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsOffsideDoBang
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsOffsideElse
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsOffsideEnd
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsOffsideFun
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsOffsideFunction
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsOffsideInterfaceMember
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsOffsideLazy
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsOffsideLet
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsOffsideReset
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsOffsideRightBlockEnd
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsOffsideThen
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsOffsideWith
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsOpen
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsOr
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsOverride
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsPercentOperator
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsPlusMinusOperator
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsPrefixOperator
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsPrivate
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsPublic
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsQuestionMark
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsQuestionMarkQuestionMark
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsQuote
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsRec
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsReserved
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsRightArrow
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsRightBrace
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsRightBracket
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsRightParenthesis
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsRightQuote
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsRightQuoteDot
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsSemicolon
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsSemicolonSemicolon
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsSig
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsStar
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsStatic
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsString
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsStringText
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsStruct
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsThen
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsTo
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsTrue
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsTry
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsType
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsUInt16
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsUInt32
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsUInt64
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsUInt8
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsUNativeInt
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsUnderscore
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsUpcast
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsVal
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsVoid
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsWhen
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsWhile
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsWhitespaceTrivia
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsWith
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsYield
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean IsYieldBang
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsAbstract()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsAdjacentPrefixOperator()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsAmpersand()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsAmpersandAmpersand()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsAnd()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsAs()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsAsr()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsAssert()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsBar()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsBarBar()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsBarRightBrace()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsBarRightBracket()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsBase()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsBegin()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsBigNumber()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsBinder()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsByteArray()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsChar()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsClass()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsColon()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsColonColon()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsColonEquals()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsColonGreater()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsColonQuestionMark()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsColonQuestionMarkGreater()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsComma()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsCommentTrivia()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsConst()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsConstraint()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsConstructor()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsDecimal()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsDefault()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsDelegate()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsDo()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsDoBang()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsDollar()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsDone()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsDot()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsDotDot()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsDotDotHat()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsDownTo()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsDowncast()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsElif()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsElse()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsEnd()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsEquals()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsException()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsExtern()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsFalse()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsFinally()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsFixed()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsFor()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsFun()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsFunction()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsFunkyOperatorName()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsGlobal()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsGreater()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsGreaterBarRightBracket()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsGreaterRightBracket()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsHash()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsHashElse()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsHashEndIf()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsHashIf()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsHashLight()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsHashLine()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsHighPrecedenceBracketApp()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsHighPrecedenceParenthesisApp()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsHighPrecedenceTypeApp()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsIdentifier()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsIeee32()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsIeee64()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsIf()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsIn()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsInactiveCode()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsInfixAmpersandOperator()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsInfixAsr()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsInfixAtHatOperator()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsInfixBarOperator()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsInfixCompareOperator()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsInfixLand()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsInfixLor()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsInfixLsl()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsInfixLsr()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsInfixLxor()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsInfixMod()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsInfixStarDivideModuloOperator()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsInfixStarStarOperator()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsInherit()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsInline()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsInstance()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsInt16()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsInt32()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsInt32DotDot()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsInt64()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsInt8()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsInterface()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsInternal()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsJoinIn()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsKeywordString()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsLazy()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsLeftArrow()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsLeftBrace()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsLeftBraceBar()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsLeftBracket()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsLeftBracketBar()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsLeftBracketLess()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsLeftParenthesis()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsLeftParenthesisStarRightParenthesis()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsLeftQuote()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsLess()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsLet()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsLineCommentTrivia()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsMatch()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsMatchBang()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsMember()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsMinus()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsModule()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsMutable()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsNamespace()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsNativeInt()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsNew()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsNone()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsNull()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsOf()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsOffsideAssert()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsOffsideBinder()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsOffsideBlockBegin()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsOffsideBlockEnd()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsOffsideBlockSep()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsOffsideDeclEnd()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsOffsideDo()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsOffsideDoBang()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsOffsideElse()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsOffsideEnd()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsOffsideFun()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsOffsideFunction()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsOffsideInterfaceMember()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsOffsideLazy()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsOffsideLet()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsOffsideReset()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsOffsideRightBlockEnd()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsOffsideThen()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsOffsideWith()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsOpen()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsOr()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsOverride()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsPercentOperator()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsPlusMinusOperator()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsPrefixOperator()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsPrivate()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsPublic()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsQuestionMark()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsQuestionMarkQuestionMark()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsQuote()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsRec()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsReserved()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsRightArrow()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsRightBrace()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsRightBracket()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsRightParenthesis()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsRightQuote()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsRightQuoteDot()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsSemicolon()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsSemicolonSemicolon()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsSig()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsStar()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsStatic()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsString()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsStringText()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsStruct()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsThen()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsTo()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsTrue()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsTry()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsType()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsUInt16()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsUInt32()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsUInt64()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsUInt8()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsUNativeInt()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsUnderscore()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsUpcast()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsVal()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsVoid()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsWhen()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsWhile()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsWhitespaceTrivia()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsWith()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsYield()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Boolean get_IsYieldBang()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Abstract
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind AdjacentPrefixOperator
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Ampersand
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind AmpersandAmpersand
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind And
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind As
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Asr
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Assert
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Bar
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind BarBar
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind BarRightBrace
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind BarRightBracket
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Base
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Begin
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind BigNumber
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Binder
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind ByteArray
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Char
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Class
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Colon
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind ColonColon
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind ColonEquals
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind ColonGreater
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind ColonQuestionMark
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind ColonQuestionMarkGreater
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Comma
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind CommentTrivia
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Const
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Constraint
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Constructor
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Decimal
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Default
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Delegate
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Do
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind DoBang
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Dollar
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Done
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Dot
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind DotDot
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind DotDotHat
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind DownTo
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Downcast
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Elif
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Else
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind End
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Equals
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Exception
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Extern
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind False
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Finally
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Fixed
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind For
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Fun
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Function
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind FunkyOperatorName
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Global
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Greater
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind GreaterBarRightBracket
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind GreaterRightBracket
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Hash
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind HashElse
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind HashEndIf
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind HashIf
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind HashLight
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind HashLine
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind HighPrecedenceBracketApp
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind HighPrecedenceParenthesisApp
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind HighPrecedenceTypeApp
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Identifier
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Ieee32
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Ieee64
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind If
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind In
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind InactiveCode
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind InfixAmpersandOperator
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind InfixAsr
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind InfixAtHatOperator
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind InfixBarOperator
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind InfixCompareOperator
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind InfixLand
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind InfixLor
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind InfixLsl
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind InfixLsr
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind InfixLxor
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind InfixMod
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind InfixStarDivideModuloOperator
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind InfixStarStarOperator
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Inherit
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Inline
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Instance
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Int16
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Int32
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Int32DotDot
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Int64
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Int8
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Interface
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Internal
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind JoinIn
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind KeywordString
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Lazy
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind LeftArrow
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind LeftBrace
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind LeftBraceBar
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind LeftBracket
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind LeftBracketBar
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind LeftBracketLess
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind LeftParenthesis
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind LeftParenthesisStarRightParenthesis
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind LeftQuote
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Less
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Let
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind LineCommentTrivia
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Match
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind MatchBang
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Member
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Minus
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Module
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Mutable
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Namespace
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind NativeInt
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind New
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind None
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Null
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Of
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind OffsideAssert
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind OffsideBinder
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind OffsideBlockBegin
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind OffsideBlockEnd
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind OffsideBlockSep
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind OffsideDeclEnd
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind OffsideDo
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind OffsideDoBang
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind OffsideElse
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind OffsideEnd
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind OffsideFun
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind OffsideFunction
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind OffsideInterfaceMember
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind OffsideLazy
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind OffsideLet
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind OffsideReset
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind OffsideRightBlockEnd
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind OffsideThen
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind OffsideWith
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Open
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Or
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Override
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind PercentOperator
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind PlusMinusOperator
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind PrefixOperator
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Private
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Public
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind QuestionMark
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind QuestionMarkQuestionMark
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Quote
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Rec
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Reserved
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind RightArrow
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind RightBrace
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind RightBracket
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind RightParenthesis
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind RightQuote
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind RightQuoteDot
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Semicolon
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind SemicolonSemicolon
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Sig
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Star
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Static
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind String
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind StringText
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Struct
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Then
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind To
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind True
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Try
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Type
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind UInt16
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind UInt32
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind UInt64
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind UInt8
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind UNativeInt
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Underscore
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Upcast
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Val
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Void
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind When
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind While
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind WhitespaceTrivia
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind With
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind Yield
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind YieldBang
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Abstract()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_AdjacentPrefixOperator()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Ampersand()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_AmpersandAmpersand()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_And()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_As()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Asr()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Assert()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Bar()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_BarBar()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_BarRightBrace()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_BarRightBracket()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Base()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Begin()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_BigNumber()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Binder()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_ByteArray()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Char()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Class()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Colon()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_ColonColon()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_ColonEquals()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_ColonGreater()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_ColonQuestionMark()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_ColonQuestionMarkGreater()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Comma()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_CommentTrivia()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Const()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Constraint()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Constructor()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Decimal()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Default()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Delegate()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Do()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_DoBang()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Dollar()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Done()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Dot()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_DotDot()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_DotDotHat()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_DownTo()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Downcast()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Elif()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Else()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_End()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Equals()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Exception()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Extern()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_False()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Finally()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Fixed()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_For()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Fun()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Function()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_FunkyOperatorName()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Global()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Greater()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_GreaterBarRightBracket()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_GreaterRightBracket()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Hash()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_HashElse()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_HashEndIf()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_HashIf()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_HashLight()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_HashLine()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_HighPrecedenceBracketApp()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_HighPrecedenceParenthesisApp()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_HighPrecedenceTypeApp()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Identifier()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Ieee32()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Ieee64()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_If()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_In()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_InactiveCode()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_InfixAmpersandOperator()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_InfixAsr()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_InfixAtHatOperator()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_InfixBarOperator()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_InfixCompareOperator()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_InfixLand()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_InfixLor()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_InfixLsl()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_InfixLsr()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_InfixLxor()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_InfixMod()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_InfixStarDivideModuloOperator()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_InfixStarStarOperator()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Inherit()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Inline()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Instance()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Int16()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Int32()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Int32DotDot()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Int64()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Int8()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Interface()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Internal()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_JoinIn()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_KeywordString()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Lazy()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_LeftArrow()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_LeftBrace()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_LeftBraceBar()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_LeftBracket()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_LeftBracketBar()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_LeftBracketLess()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_LeftParenthesis()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_LeftParenthesisStarRightParenthesis()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_LeftQuote()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Less()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Let()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_LineCommentTrivia()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Match()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_MatchBang()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Member()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Minus()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Module()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Mutable()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Namespace()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_NativeInt()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_New()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_None()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Null()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Of()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_OffsideAssert()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_OffsideBinder()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_OffsideBlockBegin()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_OffsideBlockEnd()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_OffsideBlockSep()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_OffsideDeclEnd()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_OffsideDo()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_OffsideDoBang()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_OffsideElse()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_OffsideEnd()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_OffsideFun()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_OffsideFunction()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_OffsideInterfaceMember()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_OffsideLazy()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_OffsideLet()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_OffsideReset()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_OffsideRightBlockEnd()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_OffsideThen()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_OffsideWith()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Open()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Or()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Override()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_PercentOperator()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_PlusMinusOperator()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_PrefixOperator()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Private()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Public()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_QuestionMark()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_QuestionMarkQuestionMark()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Quote()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Rec()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Reserved()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_RightArrow()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_RightBrace()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_RightBracket()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_RightParenthesis()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_RightQuote()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_RightQuoteDot()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Semicolon()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_SemicolonSemicolon()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Sig()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Star()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Static()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_String()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_StringText()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Struct()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Then()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_To()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_True()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Try()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Type()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_UInt16()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_UInt32()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_UInt64()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_UInt8()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_UNativeInt()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Underscore()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Upcast()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Val()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Void()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_When()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_While()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_WhitespaceTrivia()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_With()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_Yield()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind get_YieldBang()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: FSharp.Compiler.SourceCodeServices.FSharpTokenKind+Tags
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Int32 CompareTo(FSharp.Compiler.SourceCodeServices.FSharpTokenKind)
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Int32 CompareTo(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Int32 Tag
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: Int32 get_Tag()
FSharp.Compiler.SourceCodeServices.FSharpTokenKind: System.String ToString()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 AMP_AMP
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 BAR
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 BAR_BAR
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 BAR_RBRACK
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 BEGIN
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 CLASS
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 COLON
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 COLON_COLON
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 COLON_EQUALS
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 COLON_GREATER
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 COLON_QMARK
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 COLON_QMARK_GREATER
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 COMMA
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 COMMENT
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 DO
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 DOT
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 DOT_DOT
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 DOT_DOT_HAT
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 ELSE
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 EQUALS
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 FUNCTION
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 GREATER
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 GREATER_RBRACK
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 IDENT
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 INFIX_AT_HAT_OP
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 INFIX_BAR_OP
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 INFIX_COMPARE_OP
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 INFIX_STAR_DIV_MOD_OP
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 INT32_DOT_DOT
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 INTERP_STRING_BEGIN_END
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 INTERP_STRING_BEGIN_PART
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 INTERP_STRING_END
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 INTERP_STRING_PART
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 Identifier
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 LARROW
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 LBRACE
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 LBRACK
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 LBRACK_BAR
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 LBRACK_LESS
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 LESS
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 LINE_COMMENT
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 LPAREN
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 MINUS
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 NEW
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 OWITH
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 PERCENT_OP
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 PLUS_MINUS_OP
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 PREFIX_OP
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 QMARK
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 QUOTE
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 RARROW
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 RBRACE
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 RBRACK
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 RPAREN
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 SEMICOLON
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 STAR
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 STRING
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 STRUCT
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 String
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 THEN
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 TRY
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 UNDERSCORE
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 WHITESPACE
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 WITH
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_AMP_AMP()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_BAR()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_BAR_BAR()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_BAR_RBRACK()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_BEGIN()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_CLASS()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_COLON()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_COLON_COLON()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_COLON_EQUALS()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_COLON_GREATER()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_COLON_QMARK()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_COLON_QMARK_GREATER()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_COMMA()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_COMMENT()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_DO()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_DOT()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_DOT_DOT()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_DOT_DOT_HAT()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_ELSE()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_EQUALS()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_FUNCTION()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_GREATER()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_GREATER_RBRACK()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_IDENT()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_INFIX_AT_HAT_OP()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_INFIX_BAR_OP()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_INFIX_COMPARE_OP()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_INFIX_STAR_DIV_MOD_OP()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_INT32_DOT_DOT()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_INTERP_STRING_BEGIN_END()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_INTERP_STRING_BEGIN_PART()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_INTERP_STRING_END()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_INTERP_STRING_PART()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_Identifier()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_LARROW()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_LBRACE()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_LBRACK()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_LBRACK_BAR()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_LBRACK_LESS()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_LESS()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_LINE_COMMENT()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_LPAREN()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_MINUS()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_NEW()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_OWITH()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_PERCENT_OP()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_PLUS_MINUS_OP()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_PREFIX_OP()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_QMARK()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_QUOTE()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_RARROW()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_RBRACE()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_RBRACK()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_RPAREN()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_SEMICOLON()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_STAR()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_STRING()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_STRUCT()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_String()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_THEN()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_TRY()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_UNDERSCORE()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_WHITESPACE()
FSharp.Compiler.SourceCodeServices.FSharpTokenTag: Int32 get_WITH()
FSharp.Compiler.SourceCodeServices.FSharpTokenTriggerClass
FSharp.Compiler.SourceCodeServices.FSharpTokenTriggerClass: FSharp.Compiler.SourceCodeServices.FSharpTokenTriggerClass ChoiceSelect
FSharp.Compiler.SourceCodeServices.FSharpTokenTriggerClass: FSharp.Compiler.SourceCodeServices.FSharpTokenTriggerClass MatchBraces
FSharp.Compiler.SourceCodeServices.FSharpTokenTriggerClass: FSharp.Compiler.SourceCodeServices.FSharpTokenTriggerClass MemberSelect
FSharp.Compiler.SourceCodeServices.FSharpTokenTriggerClass: FSharp.Compiler.SourceCodeServices.FSharpTokenTriggerClass MethodTip
FSharp.Compiler.SourceCodeServices.FSharpTokenTriggerClass: FSharp.Compiler.SourceCodeServices.FSharpTokenTriggerClass None
FSharp.Compiler.SourceCodeServices.FSharpTokenTriggerClass: FSharp.Compiler.SourceCodeServices.FSharpTokenTriggerClass ParamEnd
FSharp.Compiler.SourceCodeServices.FSharpTokenTriggerClass: FSharp.Compiler.SourceCodeServices.FSharpTokenTriggerClass ParamNext
FSharp.Compiler.SourceCodeServices.FSharpTokenTriggerClass: FSharp.Compiler.SourceCodeServices.FSharpTokenTriggerClass ParamStart
FSharp.Compiler.SourceCodeServices.FSharpTokenTriggerClass: Int32 value__
FSharp.Compiler.SourceCodeServices.FSharpTokenizerColorState
FSharp.Compiler.SourceCodeServices.FSharpTokenizerColorState: FSharp.Compiler.SourceCodeServices.FSharpTokenizerColorState CamlOnly
FSharp.Compiler.SourceCodeServices.FSharpTokenizerColorState: FSharp.Compiler.SourceCodeServices.FSharpTokenizerColorState Comment
FSharp.Compiler.SourceCodeServices.FSharpTokenizerColorState: FSharp.Compiler.SourceCodeServices.FSharpTokenizerColorState EndLineThenSkip
FSharp.Compiler.SourceCodeServices.FSharpTokenizerColorState: FSharp.Compiler.SourceCodeServices.FSharpTokenizerColorState EndLineThenToken
FSharp.Compiler.SourceCodeServices.FSharpTokenizerColorState: FSharp.Compiler.SourceCodeServices.FSharpTokenizerColorState IfDefSkip
FSharp.Compiler.SourceCodeServices.FSharpTokenizerColorState: FSharp.Compiler.SourceCodeServices.FSharpTokenizerColorState InitialState
FSharp.Compiler.SourceCodeServices.FSharpTokenizerColorState: FSharp.Compiler.SourceCodeServices.FSharpTokenizerColorState SingleLineComment
FSharp.Compiler.SourceCodeServices.FSharpTokenizerColorState: FSharp.Compiler.SourceCodeServices.FSharpTokenizerColorState String
FSharp.Compiler.SourceCodeServices.FSharpTokenizerColorState: FSharp.Compiler.SourceCodeServices.FSharpTokenizerColorState StringInComment
FSharp.Compiler.SourceCodeServices.FSharpTokenizerColorState: FSharp.Compiler.SourceCodeServices.FSharpTokenizerColorState Token
FSharp.Compiler.SourceCodeServices.FSharpTokenizerColorState: FSharp.Compiler.SourceCodeServices.FSharpTokenizerColorState TripleQuoteString
FSharp.Compiler.SourceCodeServices.FSharpTokenizerColorState: FSharp.Compiler.SourceCodeServices.FSharpTokenizerColorState TripleQuoteStringInComment
FSharp.Compiler.SourceCodeServices.FSharpTokenizerColorState: FSharp.Compiler.SourceCodeServices.FSharpTokenizerColorState VerbatimString
FSharp.Compiler.SourceCodeServices.FSharpTokenizerColorState: FSharp.Compiler.SourceCodeServices.FSharpTokenizerColorState VerbatimStringInComment
FSharp.Compiler.SourceCodeServices.FSharpTokenizerColorState: Int32 value__
FSharp.Compiler.SourceCodeServices.FSharpTokenizerLexState
FSharp.Compiler.SourceCodeServices.FSharpTokenizerLexState: Boolean Equals(FSharp.Compiler.SourceCodeServices.FSharpTokenizerLexState)
FSharp.Compiler.SourceCodeServices.FSharpTokenizerLexState: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpTokenizerLexState: FSharp.Compiler.SourceCodeServices.FSharpTokenizerLexState Initial
FSharp.Compiler.SourceCodeServices.FSharpTokenizerLexState: FSharp.Compiler.SourceCodeServices.FSharpTokenizerLexState get_Initial()
FSharp.Compiler.SourceCodeServices.FSharpTokenizerLexState: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.FSharpTokenizerLexState: Int64 OtherBits
FSharp.Compiler.SourceCodeServices.FSharpTokenizerLexState: Int64 PosBits
FSharp.Compiler.SourceCodeServices.FSharpTokenizerLexState: Int64 get_OtherBits()
FSharp.Compiler.SourceCodeServices.FSharpTokenizerLexState: Int64 get_PosBits()
FSharp.Compiler.SourceCodeServices.FSharpTokenizerLexState: System.String ToString()
FSharp.Compiler.SourceCodeServices.FSharpTokenizerLexState: Void .ctor(Int64, Int64)
FSharp.Compiler.SourceCodeServices.FSharpToolTip
FSharp.Compiler.SourceCodeServices.FSharpToolTip: FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1[System.String] ToFSharpToolTipElement(FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1[FSharp.Compiler.TextLayout.Layout])
FSharp.Compiler.SourceCodeServices.FSharpToolTip: FSharp.Compiler.SourceCodeServices.FSharpToolTipText`1[System.String] ToFSharpToolTipText(FSharp.Compiler.SourceCodeServices.FSharpToolTipText`1[FSharp.Compiler.TextLayout.Layout])
FSharp.Compiler.SourceCodeServices.FSharpToolTipElementData`1[T]
FSharp.Compiler.SourceCodeServices.FSharpToolTipElementData`1[T]: Boolean Equals(FSharp.Compiler.SourceCodeServices.FSharpToolTipElementData`1[T])
FSharp.Compiler.SourceCodeServices.FSharpToolTipElementData`1[T]: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpToolTipElementData`1[T]: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.FSharpToolTipElementData`1[T]: FSharp.Compiler.SourceCodeServices.FSharpXmlDoc XmlDoc
FSharp.Compiler.SourceCodeServices.FSharpToolTipElementData`1[T]: FSharp.Compiler.SourceCodeServices.FSharpXmlDoc get_XmlDoc()
FSharp.Compiler.SourceCodeServices.FSharpToolTipElementData`1[T]: Int32 CompareTo(FSharp.Compiler.SourceCodeServices.FSharpToolTipElementData`1[T])
FSharp.Compiler.SourceCodeServices.FSharpToolTipElementData`1[T]: Int32 CompareTo(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpToolTipElementData`1[T]: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.SourceCodeServices.FSharpToolTipElementData`1[T]: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.FSharpToolTipElementData`1[T]: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.FSharpToolTipElementData`1[T]: Microsoft.FSharp.Collections.FSharpList`1[T] TypeMapping
FSharp.Compiler.SourceCodeServices.FSharpToolTipElementData`1[T]: Microsoft.FSharp.Collections.FSharpList`1[T] get_TypeMapping()
FSharp.Compiler.SourceCodeServices.FSharpToolTipElementData`1[T]: Microsoft.FSharp.Core.FSharpOption`1[System.String] ParamName
FSharp.Compiler.SourceCodeServices.FSharpToolTipElementData`1[T]: Microsoft.FSharp.Core.FSharpOption`1[System.String] get_ParamName()
FSharp.Compiler.SourceCodeServices.FSharpToolTipElementData`1[T]: Microsoft.FSharp.Core.FSharpOption`1[T] Remarks
FSharp.Compiler.SourceCodeServices.FSharpToolTipElementData`1[T]: Microsoft.FSharp.Core.FSharpOption`1[T] get_Remarks()
FSharp.Compiler.SourceCodeServices.FSharpToolTipElementData`1[T]: System.String ToString()
FSharp.Compiler.SourceCodeServices.FSharpToolTipElementData`1[T]: T MainDescription
FSharp.Compiler.SourceCodeServices.FSharpToolTipElementData`1[T]: T get_MainDescription()
FSharp.Compiler.SourceCodeServices.FSharpToolTipElementData`1[T]: Void .ctor(T, FSharp.Compiler.SourceCodeServices.FSharpXmlDoc, Microsoft.FSharp.Collections.FSharpList`1[T], Microsoft.FSharp.Core.FSharpOption`1[T], Microsoft.FSharp.Core.FSharpOption`1[System.String])
FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1+CompositionError[T]: System.String errorText
FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1+CompositionError[T]: System.String get_errorText()
FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1+Group[T]: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpToolTipElementData`1[T]] elements
FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1+Group[T]: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpToolTipElementData`1[T]] get_elements()
FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1+Tags[T]: Int32 CompositionError
FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1+Tags[T]: Int32 Group
FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1+Tags[T]: Int32 None
FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1[T]
FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1[T]: Boolean Equals(FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1[T])
FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1[T]: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1[T]: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1[T]: Boolean IsCompositionError
FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1[T]: Boolean IsGroup
FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1[T]: Boolean IsNone
FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1[T]: Boolean get_IsCompositionError()
FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1[T]: Boolean get_IsGroup()
FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1[T]: Boolean get_IsNone()
FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1[T]: FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1+CompositionError[T]
FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1[T]: FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1+Group[T]
FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1[T]: FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1+Tags[T]
FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1[T]: FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1[T] NewCompositionError(System.String)
FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1[T]: FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1[T] NewGroup(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpToolTipElementData`1[T]])
FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1[T]: FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1[T] None
FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1[T]: FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1[T] Single(T, FSharp.Compiler.SourceCodeServices.FSharpXmlDoc, Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Collections.FSharpList`1[T]], Microsoft.FSharp.Core.FSharpOption`1[System.String], Microsoft.FSharp.Core.FSharpOption`1[T])
FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1[T]: FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1[T] get_None()
FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1[T]: Int32 CompareTo(FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1[T])
FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1[T]: Int32 CompareTo(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1[T]: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1[T]: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1[T]: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1[T]: Int32 Tag
FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1[T]: Int32 get_Tag()
FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1[T]: System.String ToString()
FSharp.Compiler.SourceCodeServices.FSharpToolTipText`1[T]
FSharp.Compiler.SourceCodeServices.FSharpToolTipText`1[T]: Boolean Equals(FSharp.Compiler.SourceCodeServices.FSharpToolTipText`1[T])
FSharp.Compiler.SourceCodeServices.FSharpToolTipText`1[T]: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpToolTipText`1[T]: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.FSharpToolTipText`1[T]: FSharp.Compiler.SourceCodeServices.FSharpToolTipText`1[T] NewFSharpToolTipText(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1[T]])
FSharp.Compiler.SourceCodeServices.FSharpToolTipText`1[T]: Int32 CompareTo(FSharp.Compiler.SourceCodeServices.FSharpToolTipText`1[T])
FSharp.Compiler.SourceCodeServices.FSharpToolTipText`1[T]: Int32 CompareTo(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpToolTipText`1[T]: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.SourceCodeServices.FSharpToolTipText`1[T]: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.FSharpToolTipText`1[T]: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.FSharpToolTipText`1[T]: Int32 Tag
FSharp.Compiler.SourceCodeServices.FSharpToolTipText`1[T]: Int32 get_Tag()
FSharp.Compiler.SourceCodeServices.FSharpToolTipText`1[T]: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1[T]] Item
FSharp.Compiler.SourceCodeServices.FSharpToolTipText`1[T]: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.FSharpToolTipElement`1[T]] get_Item()
FSharp.Compiler.SourceCodeServices.FSharpToolTipText`1[T]: System.String ToString()
FSharp.Compiler.SourceCodeServices.FSharpType
FSharp.Compiler.SourceCodeServices.FSharpType: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpType: Boolean HasTypeDefinition
FSharp.Compiler.SourceCodeServices.FSharpType: Boolean IsAbbreviation
FSharp.Compiler.SourceCodeServices.FSharpType: Boolean IsAnonRecordType
FSharp.Compiler.SourceCodeServices.FSharpType: Boolean IsFunctionType
FSharp.Compiler.SourceCodeServices.FSharpType: Boolean IsGenericParameter
FSharp.Compiler.SourceCodeServices.FSharpType: Boolean IsNamedType
FSharp.Compiler.SourceCodeServices.FSharpType: Boolean IsStructTupleType
FSharp.Compiler.SourceCodeServices.FSharpType: Boolean IsTupleType
FSharp.Compiler.SourceCodeServices.FSharpType: Boolean IsUnresolved
FSharp.Compiler.SourceCodeServices.FSharpType: Boolean get_HasTypeDefinition()
FSharp.Compiler.SourceCodeServices.FSharpType: Boolean get_IsAbbreviation()
FSharp.Compiler.SourceCodeServices.FSharpType: Boolean get_IsAnonRecordType()
FSharp.Compiler.SourceCodeServices.FSharpType: Boolean get_IsFunctionType()
FSharp.Compiler.SourceCodeServices.FSharpType: Boolean get_IsGenericParameter()
FSharp.Compiler.SourceCodeServices.FSharpType: Boolean get_IsNamedType()
FSharp.Compiler.SourceCodeServices.FSharpType: Boolean get_IsStructTupleType()
FSharp.Compiler.SourceCodeServices.FSharpType: Boolean get_IsTupleType()
FSharp.Compiler.SourceCodeServices.FSharpType: Boolean get_IsUnresolved()
FSharp.Compiler.SourceCodeServices.FSharpType: FSharp.Compiler.SourceCodeServices.FSharpAnonRecordTypeDetails AnonRecordTypeDetails
FSharp.Compiler.SourceCodeServices.FSharpType: FSharp.Compiler.SourceCodeServices.FSharpAnonRecordTypeDetails get_AnonRecordTypeDetails()
FSharp.Compiler.SourceCodeServices.FSharpType: FSharp.Compiler.SourceCodeServices.FSharpEntity NamedEntity
FSharp.Compiler.SourceCodeServices.FSharpType: FSharp.Compiler.SourceCodeServices.FSharpEntity TypeDefinition
FSharp.Compiler.SourceCodeServices.FSharpType: FSharp.Compiler.SourceCodeServices.FSharpEntity get_NamedEntity()
FSharp.Compiler.SourceCodeServices.FSharpType: FSharp.Compiler.SourceCodeServices.FSharpEntity get_TypeDefinition()
FSharp.Compiler.SourceCodeServices.FSharpType: FSharp.Compiler.SourceCodeServices.FSharpGenericParameter GenericParameter
FSharp.Compiler.SourceCodeServices.FSharpType: FSharp.Compiler.SourceCodeServices.FSharpGenericParameter get_GenericParameter()
FSharp.Compiler.SourceCodeServices.FSharpType: FSharp.Compiler.SourceCodeServices.FSharpParameter Prettify(FSharp.Compiler.SourceCodeServices.FSharpParameter)
FSharp.Compiler.SourceCodeServices.FSharpType: FSharp.Compiler.SourceCodeServices.FSharpType AbbreviatedType
FSharp.Compiler.SourceCodeServices.FSharpType: FSharp.Compiler.SourceCodeServices.FSharpType Instantiate(Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[FSharp.Compiler.SourceCodeServices.FSharpGenericParameter,FSharp.Compiler.SourceCodeServices.FSharpType]])
FSharp.Compiler.SourceCodeServices.FSharpType: FSharp.Compiler.SourceCodeServices.FSharpType Prettify(FSharp.Compiler.SourceCodeServices.FSharpType)
FSharp.Compiler.SourceCodeServices.FSharpType: FSharp.Compiler.SourceCodeServices.FSharpType get_AbbreviatedType()
FSharp.Compiler.SourceCodeServices.FSharpType: FSharp.Compiler.TextLayout.Layout FormatLayout(FSharp.Compiler.SourceCodeServices.FSharpDisplayContext)
FSharp.Compiler.SourceCodeServices.FSharpType: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.FSharpType: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpType] BaseType
FSharp.Compiler.SourceCodeServices.FSharpType: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpType] get_BaseType()
FSharp.Compiler.SourceCodeServices.FSharpType: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpParameter] Prettify(System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpParameter])
FSharp.Compiler.SourceCodeServices.FSharpType: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpType] AllInterfaces
FSharp.Compiler.SourceCodeServices.FSharpType: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpType] GenericArguments
FSharp.Compiler.SourceCodeServices.FSharpType: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpType] Prettify(System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpType])
FSharp.Compiler.SourceCodeServices.FSharpType: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpType] get_AllInterfaces()
FSharp.Compiler.SourceCodeServices.FSharpType: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpType] get_GenericArguments()
FSharp.Compiler.SourceCodeServices.FSharpType: System.Collections.Generic.IList`1[System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpParameter]] Prettify(System.Collections.Generic.IList`1[System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpParameter]])
FSharp.Compiler.SourceCodeServices.FSharpType: System.String Format(FSharp.Compiler.SourceCodeServices.FSharpDisplayContext)
FSharp.Compiler.SourceCodeServices.FSharpType: System.String ToString()
FSharp.Compiler.SourceCodeServices.FSharpType: System.Tuple`2[System.Collections.Generic.IList`1[System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpParameter]],FSharp.Compiler.SourceCodeServices.FSharpParameter] Prettify(System.Collections.Generic.IList`1[System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpParameter]], FSharp.Compiler.SourceCodeServices.FSharpParameter)
FSharp.Compiler.SourceCodeServices.FSharpUnionCase
FSharp.Compiler.SourceCodeServices.FSharpUnionCase: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpUnionCase: Boolean HasFields
FSharp.Compiler.SourceCodeServices.FSharpUnionCase: Boolean IsUnresolved
FSharp.Compiler.SourceCodeServices.FSharpUnionCase: Boolean get_HasFields()
FSharp.Compiler.SourceCodeServices.FSharpUnionCase: Boolean get_IsUnresolved()
FSharp.Compiler.SourceCodeServices.FSharpUnionCase: FSharp.Compiler.SourceCodeServices.FSharpAccessibility Accessibility
FSharp.Compiler.SourceCodeServices.FSharpUnionCase: FSharp.Compiler.SourceCodeServices.FSharpAccessibility get_Accessibility()
FSharp.Compiler.SourceCodeServices.FSharpUnionCase: FSharp.Compiler.SourceCodeServices.FSharpType ReturnType
FSharp.Compiler.SourceCodeServices.FSharpUnionCase: FSharp.Compiler.SourceCodeServices.FSharpType get_ReturnType()
FSharp.Compiler.SourceCodeServices.FSharpUnionCase: FSharp.Compiler.Text.Range DeclarationLocation
FSharp.Compiler.SourceCodeServices.FSharpUnionCase: FSharp.Compiler.Text.Range get_DeclarationLocation()
FSharp.Compiler.SourceCodeServices.FSharpUnionCase: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.FSharpUnionCase: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpAttribute] Attributes
FSharp.Compiler.SourceCodeServices.FSharpUnionCase: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpAttribute] get_Attributes()
FSharp.Compiler.SourceCodeServices.FSharpUnionCase: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpField] UnionCaseFields
FSharp.Compiler.SourceCodeServices.FSharpUnionCase: System.Collections.Generic.IList`1[FSharp.Compiler.SourceCodeServices.FSharpField] get_UnionCaseFields()
FSharp.Compiler.SourceCodeServices.FSharpUnionCase: System.Collections.Generic.IList`1[System.String] ElaboratedXmlDoc
FSharp.Compiler.SourceCodeServices.FSharpUnionCase: System.Collections.Generic.IList`1[System.String] XmlDoc
FSharp.Compiler.SourceCodeServices.FSharpUnionCase: System.Collections.Generic.IList`1[System.String] get_ElaboratedXmlDoc()
FSharp.Compiler.SourceCodeServices.FSharpUnionCase: System.Collections.Generic.IList`1[System.String] get_XmlDoc()
FSharp.Compiler.SourceCodeServices.FSharpUnionCase: System.String CompiledName
FSharp.Compiler.SourceCodeServices.FSharpUnionCase: System.String Name
FSharp.Compiler.SourceCodeServices.FSharpUnionCase: System.String ToString()
FSharp.Compiler.SourceCodeServices.FSharpUnionCase: System.String XmlDocSig
FSharp.Compiler.SourceCodeServices.FSharpUnionCase: System.String get_CompiledName()
FSharp.Compiler.SourceCodeServices.FSharpUnionCase: System.String get_Name()
FSharp.Compiler.SourceCodeServices.FSharpUnionCase: System.String get_XmlDocSig()
FSharp.Compiler.SourceCodeServices.FSharpUnresolvedReferencesSet
FSharp.Compiler.SourceCodeServices.FSharpUnresolvedReferencesSet: Boolean Equals(FSharp.Compiler.SourceCodeServices.FSharpUnresolvedReferencesSet)
FSharp.Compiler.SourceCodeServices.FSharpUnresolvedReferencesSet: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpUnresolvedReferencesSet: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.FSharpUnresolvedReferencesSet: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.FSharpUnresolvedReferencesSet: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.FSharpUnresolvedReferencesSet: System.String ToString()
FSharp.Compiler.SourceCodeServices.FSharpUnresolvedSymbol
FSharp.Compiler.SourceCodeServices.FSharpUnresolvedSymbol: Boolean Equals(FSharp.Compiler.SourceCodeServices.FSharpUnresolvedSymbol)
FSharp.Compiler.SourceCodeServices.FSharpUnresolvedSymbol: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpUnresolvedSymbol: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.FSharpUnresolvedSymbol: Int32 CompareTo(FSharp.Compiler.SourceCodeServices.FSharpUnresolvedSymbol)
FSharp.Compiler.SourceCodeServices.FSharpUnresolvedSymbol: Int32 CompareTo(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpUnresolvedSymbol: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.SourceCodeServices.FSharpUnresolvedSymbol: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.FSharpUnresolvedSymbol: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.FSharpUnresolvedSymbol: System.String DisplayName
FSharp.Compiler.SourceCodeServices.FSharpUnresolvedSymbol: System.String FullName
FSharp.Compiler.SourceCodeServices.FSharpUnresolvedSymbol: System.String ToString()
FSharp.Compiler.SourceCodeServices.FSharpUnresolvedSymbol: System.String get_DisplayName()
FSharp.Compiler.SourceCodeServices.FSharpUnresolvedSymbol: System.String get_FullName()
FSharp.Compiler.SourceCodeServices.FSharpUnresolvedSymbol: System.String[] Namespace
FSharp.Compiler.SourceCodeServices.FSharpUnresolvedSymbol: System.String[] get_Namespace()
FSharp.Compiler.SourceCodeServices.FSharpUnresolvedSymbol: Void .ctor(System.String, System.String, System.String[])
FSharp.Compiler.SourceCodeServices.FSharpXmlDoc
FSharp.Compiler.SourceCodeServices.FSharpXmlDoc+Tags: Int32 None
FSharp.Compiler.SourceCodeServices.FSharpXmlDoc+Tags: Int32 Text
FSharp.Compiler.SourceCodeServices.FSharpXmlDoc+Tags: Int32 XmlDocFileSignature
FSharp.Compiler.SourceCodeServices.FSharpXmlDoc+Text: System.String[] elaboratedXmlLines
FSharp.Compiler.SourceCodeServices.FSharpXmlDoc+Text: System.String[] get_elaboratedXmlLines()
FSharp.Compiler.SourceCodeServices.FSharpXmlDoc+Text: System.String[] get_unprocessedLines()
FSharp.Compiler.SourceCodeServices.FSharpXmlDoc+Text: System.String[] unprocessedLines
FSharp.Compiler.SourceCodeServices.FSharpXmlDoc+XmlDocFileSignature: System.String file
FSharp.Compiler.SourceCodeServices.FSharpXmlDoc+XmlDocFileSignature: System.String get_file()
FSharp.Compiler.SourceCodeServices.FSharpXmlDoc+XmlDocFileSignature: System.String get_xmlSig()
FSharp.Compiler.SourceCodeServices.FSharpXmlDoc+XmlDocFileSignature: System.String xmlSig
FSharp.Compiler.SourceCodeServices.FSharpXmlDoc: Boolean Equals(FSharp.Compiler.SourceCodeServices.FSharpXmlDoc)
FSharp.Compiler.SourceCodeServices.FSharpXmlDoc: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpXmlDoc: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.FSharpXmlDoc: Boolean IsNone
FSharp.Compiler.SourceCodeServices.FSharpXmlDoc: Boolean IsText
FSharp.Compiler.SourceCodeServices.FSharpXmlDoc: Boolean IsXmlDocFileSignature
FSharp.Compiler.SourceCodeServices.FSharpXmlDoc: Boolean get_IsNone()
FSharp.Compiler.SourceCodeServices.FSharpXmlDoc: Boolean get_IsText()
FSharp.Compiler.SourceCodeServices.FSharpXmlDoc: Boolean get_IsXmlDocFileSignature()
FSharp.Compiler.SourceCodeServices.FSharpXmlDoc: FSharp.Compiler.SourceCodeServices.FSharpXmlDoc NewText(System.String[], System.String[])
FSharp.Compiler.SourceCodeServices.FSharpXmlDoc: FSharp.Compiler.SourceCodeServices.FSharpXmlDoc NewXmlDocFileSignature(System.String, System.String)
FSharp.Compiler.SourceCodeServices.FSharpXmlDoc: FSharp.Compiler.SourceCodeServices.FSharpXmlDoc None
FSharp.Compiler.SourceCodeServices.FSharpXmlDoc: FSharp.Compiler.SourceCodeServices.FSharpXmlDoc get_None()
FSharp.Compiler.SourceCodeServices.FSharpXmlDoc: FSharp.Compiler.SourceCodeServices.FSharpXmlDoc+Tags
FSharp.Compiler.SourceCodeServices.FSharpXmlDoc: FSharp.Compiler.SourceCodeServices.FSharpXmlDoc+Text
FSharp.Compiler.SourceCodeServices.FSharpXmlDoc: FSharp.Compiler.SourceCodeServices.FSharpXmlDoc+XmlDocFileSignature
FSharp.Compiler.SourceCodeServices.FSharpXmlDoc: Int32 CompareTo(FSharp.Compiler.SourceCodeServices.FSharpXmlDoc)
FSharp.Compiler.SourceCodeServices.FSharpXmlDoc: Int32 CompareTo(System.Object)
FSharp.Compiler.SourceCodeServices.FSharpXmlDoc: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.SourceCodeServices.FSharpXmlDoc: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.FSharpXmlDoc: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.FSharpXmlDoc: Int32 Tag
FSharp.Compiler.SourceCodeServices.FSharpXmlDoc: Int32 get_Tag()
FSharp.Compiler.SourceCodeServices.FSharpXmlDoc: System.String ToString()
FSharp.Compiler.SourceCodeServices.FileSystemAutoOpens
FSharp.Compiler.SourceCodeServices.FileSystemAutoOpens: FSharp.Compiler.SourceCodeServices.IFileSystem FileSystem
FSharp.Compiler.SourceCodeServices.FileSystemAutoOpens: FSharp.Compiler.SourceCodeServices.IFileSystem get_FileSystem()
FSharp.Compiler.SourceCodeServices.FileSystemAutoOpens: Void set_FileSystem(FSharp.Compiler.SourceCodeServices.IFileSystem)
FSharp.Compiler.SourceCodeServices.IAssemblyContentCache
FSharp.Compiler.SourceCodeServices.IAssemblyContentCache: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.AssemblyContentCacheEntry] TryGet(System.String)
FSharp.Compiler.SourceCodeServices.IAssemblyContentCache: Void Set(System.String, FSharp.Compiler.SourceCodeServices.AssemblyContentCacheEntry)
FSharp.Compiler.SourceCodeServices.IFileSystem
FSharp.Compiler.SourceCodeServices.IFileSystem: Boolean IsInvalidPathShim(System.String)
FSharp.Compiler.SourceCodeServices.IFileSystem: Boolean IsPathRootedShim(System.String)
FSharp.Compiler.SourceCodeServices.IFileSystem: Boolean IsStableFileHeuristic(System.String)
FSharp.Compiler.SourceCodeServices.IFileSystem: Boolean SafeExists(System.String)
FSharp.Compiler.SourceCodeServices.IFileSystem: Byte[] ReadAllBytesShim(System.String)
FSharp.Compiler.SourceCodeServices.IFileSystem: System.DateTime GetLastWriteTimeShim(System.String)
FSharp.Compiler.SourceCodeServices.IFileSystem: System.IO.Stream FileStreamCreateShim(System.String)
FSharp.Compiler.SourceCodeServices.IFileSystem: System.IO.Stream FileStreamReadShim(System.String)
FSharp.Compiler.SourceCodeServices.IFileSystem: System.IO.Stream FileStreamWriteExistingShim(System.String)
FSharp.Compiler.SourceCodeServices.IFileSystem: System.Reflection.Assembly AssemblyLoad(System.Reflection.AssemblyName)
FSharp.Compiler.SourceCodeServices.IFileSystem: System.Reflection.Assembly AssemblyLoadFrom(System.String)
FSharp.Compiler.SourceCodeServices.IFileSystem: System.String GetFullPathShim(System.String)
FSharp.Compiler.SourceCodeServices.IFileSystem: System.String GetTempPathShim()
FSharp.Compiler.SourceCodeServices.IFileSystem: Void FileDelete(System.String)
FSharp.Compiler.SourceCodeServices.InheritanceContext
FSharp.Compiler.SourceCodeServices.InheritanceContext+Tags: Int32 Class
FSharp.Compiler.SourceCodeServices.InheritanceContext+Tags: Int32 Interface
FSharp.Compiler.SourceCodeServices.InheritanceContext+Tags: Int32 Unknown
FSharp.Compiler.SourceCodeServices.InheritanceContext: Boolean Equals(FSharp.Compiler.SourceCodeServices.InheritanceContext)
FSharp.Compiler.SourceCodeServices.InheritanceContext: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.InheritanceContext: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.InheritanceContext: Boolean IsClass
FSharp.Compiler.SourceCodeServices.InheritanceContext: Boolean IsInterface
FSharp.Compiler.SourceCodeServices.InheritanceContext: Boolean IsUnknown
FSharp.Compiler.SourceCodeServices.InheritanceContext: Boolean get_IsClass()
FSharp.Compiler.SourceCodeServices.InheritanceContext: Boolean get_IsInterface()
FSharp.Compiler.SourceCodeServices.InheritanceContext: Boolean get_IsUnknown()
FSharp.Compiler.SourceCodeServices.InheritanceContext: FSharp.Compiler.SourceCodeServices.InheritanceContext Class
FSharp.Compiler.SourceCodeServices.InheritanceContext: FSharp.Compiler.SourceCodeServices.InheritanceContext Interface
FSharp.Compiler.SourceCodeServices.InheritanceContext: FSharp.Compiler.SourceCodeServices.InheritanceContext Unknown
FSharp.Compiler.SourceCodeServices.InheritanceContext: FSharp.Compiler.SourceCodeServices.InheritanceContext get_Class()
FSharp.Compiler.SourceCodeServices.InheritanceContext: FSharp.Compiler.SourceCodeServices.InheritanceContext get_Interface()
FSharp.Compiler.SourceCodeServices.InheritanceContext: FSharp.Compiler.SourceCodeServices.InheritanceContext get_Unknown()
FSharp.Compiler.SourceCodeServices.InheritanceContext: FSharp.Compiler.SourceCodeServices.InheritanceContext+Tags
FSharp.Compiler.SourceCodeServices.InheritanceContext: Int32 CompareTo(FSharp.Compiler.SourceCodeServices.InheritanceContext)
FSharp.Compiler.SourceCodeServices.InheritanceContext: Int32 CompareTo(System.Object)
FSharp.Compiler.SourceCodeServices.InheritanceContext: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.SourceCodeServices.InheritanceContext: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.InheritanceContext: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.InheritanceContext: Int32 Tag
FSharp.Compiler.SourceCodeServices.InheritanceContext: Int32 get_Tag()
FSharp.Compiler.SourceCodeServices.InheritanceContext: System.String ToString()
FSharp.Compiler.SourceCodeServices.InsertContext
FSharp.Compiler.SourceCodeServices.InsertContext: Boolean Equals(FSharp.Compiler.SourceCodeServices.InsertContext)
FSharp.Compiler.SourceCodeServices.InsertContext: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.InsertContext: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.InsertContext: FSharp.Compiler.SourceCodeServices.ScopeKind ScopeKind
FSharp.Compiler.SourceCodeServices.InsertContext: FSharp.Compiler.SourceCodeServices.ScopeKind get_ScopeKind()
FSharp.Compiler.SourceCodeServices.InsertContext: FSharp.Compiler.Text.Pos Pos
FSharp.Compiler.SourceCodeServices.InsertContext: FSharp.Compiler.Text.Pos get_Pos()
FSharp.Compiler.SourceCodeServices.InsertContext: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.InsertContext: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.InsertContext: System.String ToString()
FSharp.Compiler.SourceCodeServices.InsertContext: Void .ctor(FSharp.Compiler.SourceCodeServices.ScopeKind, FSharp.Compiler.Text.Pos)
FSharp.Compiler.SourceCodeServices.LookupType
FSharp.Compiler.SourceCodeServices.LookupType+Tags: Int32 Fuzzy
FSharp.Compiler.SourceCodeServices.LookupType+Tags: Int32 Precise
FSharp.Compiler.SourceCodeServices.LookupType: Boolean Equals(FSharp.Compiler.SourceCodeServices.LookupType)
FSharp.Compiler.SourceCodeServices.LookupType: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.LookupType: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.LookupType: Boolean IsFuzzy
FSharp.Compiler.SourceCodeServices.LookupType: Boolean IsPrecise
FSharp.Compiler.SourceCodeServices.LookupType: Boolean get_IsFuzzy()
FSharp.Compiler.SourceCodeServices.LookupType: Boolean get_IsPrecise()
FSharp.Compiler.SourceCodeServices.LookupType: FSharp.Compiler.SourceCodeServices.LookupType Fuzzy
FSharp.Compiler.SourceCodeServices.LookupType: FSharp.Compiler.SourceCodeServices.LookupType Precise
FSharp.Compiler.SourceCodeServices.LookupType: FSharp.Compiler.SourceCodeServices.LookupType get_Fuzzy()
FSharp.Compiler.SourceCodeServices.LookupType: FSharp.Compiler.SourceCodeServices.LookupType get_Precise()
FSharp.Compiler.SourceCodeServices.LookupType: FSharp.Compiler.SourceCodeServices.LookupType+Tags
FSharp.Compiler.SourceCodeServices.LookupType: Int32 CompareTo(FSharp.Compiler.SourceCodeServices.LookupType)
FSharp.Compiler.SourceCodeServices.LookupType: Int32 CompareTo(System.Object)
FSharp.Compiler.SourceCodeServices.LookupType: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.SourceCodeServices.LookupType: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.LookupType: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.LookupType: Int32 Tag
FSharp.Compiler.SourceCodeServices.LookupType: Int32 get_Tag()
FSharp.Compiler.SourceCodeServices.LookupType: System.String ToString()
FSharp.Compiler.SourceCodeServices.MaybeUnresolvedIdent
FSharp.Compiler.SourceCodeServices.MaybeUnresolvedIdent: Boolean Equals(FSharp.Compiler.SourceCodeServices.MaybeUnresolvedIdent)
FSharp.Compiler.SourceCodeServices.MaybeUnresolvedIdent: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.MaybeUnresolvedIdent: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.MaybeUnresolvedIdent: Boolean Resolved
FSharp.Compiler.SourceCodeServices.MaybeUnresolvedIdent: Boolean get_Resolved()
FSharp.Compiler.SourceCodeServices.MaybeUnresolvedIdent: Int32 CompareTo(FSharp.Compiler.SourceCodeServices.MaybeUnresolvedIdent)
FSharp.Compiler.SourceCodeServices.MaybeUnresolvedIdent: Int32 CompareTo(System.Object)
FSharp.Compiler.SourceCodeServices.MaybeUnresolvedIdent: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.SourceCodeServices.MaybeUnresolvedIdent: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.MaybeUnresolvedIdent: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.MaybeUnresolvedIdent: System.String Ident
FSharp.Compiler.SourceCodeServices.MaybeUnresolvedIdent: System.String ToString()
FSharp.Compiler.SourceCodeServices.MaybeUnresolvedIdent: System.String get_Ident()
FSharp.Compiler.SourceCodeServices.MaybeUnresolvedIdent: Void .ctor(System.String, Boolean)
FSharp.Compiler.SourceCodeServices.ModuleKind
FSharp.Compiler.SourceCodeServices.ModuleKind: Boolean Equals(FSharp.Compiler.SourceCodeServices.ModuleKind)
FSharp.Compiler.SourceCodeServices.ModuleKind: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.ModuleKind: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.ModuleKind: Boolean HasModuleSuffix
FSharp.Compiler.SourceCodeServices.ModuleKind: Boolean IsAutoOpen
FSharp.Compiler.SourceCodeServices.ModuleKind: Boolean get_HasModuleSuffix()
FSharp.Compiler.SourceCodeServices.ModuleKind: Boolean get_IsAutoOpen()
FSharp.Compiler.SourceCodeServices.ModuleKind: Int32 CompareTo(FSharp.Compiler.SourceCodeServices.ModuleKind)
FSharp.Compiler.SourceCodeServices.ModuleKind: Int32 CompareTo(System.Object)
FSharp.Compiler.SourceCodeServices.ModuleKind: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.SourceCodeServices.ModuleKind: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.ModuleKind: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.ModuleKind: System.String ToString()
FSharp.Compiler.SourceCodeServices.ModuleKind: Void .ctor(Boolean, Boolean)
FSharp.Compiler.SourceCodeServices.NavigateTo
FSharp.Compiler.SourceCodeServices.NavigateTo+Container: Boolean Equals(Container)
FSharp.Compiler.SourceCodeServices.NavigateTo+Container: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.NavigateTo+Container: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.NavigateTo+Container: ContainerType Type
FSharp.Compiler.SourceCodeServices.NavigateTo+Container: ContainerType get_Type()
FSharp.Compiler.SourceCodeServices.NavigateTo+Container: Int32 CompareTo(Container)
FSharp.Compiler.SourceCodeServices.NavigateTo+Container: Int32 CompareTo(System.Object)
FSharp.Compiler.SourceCodeServices.NavigateTo+Container: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.SourceCodeServices.NavigateTo+Container: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.NavigateTo+Container: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.NavigateTo+Container: System.String Name
FSharp.Compiler.SourceCodeServices.NavigateTo+Container: System.String ToString()
FSharp.Compiler.SourceCodeServices.NavigateTo+Container: System.String get_Name()
FSharp.Compiler.SourceCodeServices.NavigateTo+Container: Void .ctor(ContainerType, System.String)
FSharp.Compiler.SourceCodeServices.NavigateTo+ContainerType+Tags: Int32 Exception
FSharp.Compiler.SourceCodeServices.NavigateTo+ContainerType+Tags: Int32 File
FSharp.Compiler.SourceCodeServices.NavigateTo+ContainerType+Tags: Int32 Module
FSharp.Compiler.SourceCodeServices.NavigateTo+ContainerType+Tags: Int32 Namespace
FSharp.Compiler.SourceCodeServices.NavigateTo+ContainerType+Tags: Int32 Type
FSharp.Compiler.SourceCodeServices.NavigateTo+ContainerType: Boolean Equals(ContainerType)
FSharp.Compiler.SourceCodeServices.NavigateTo+ContainerType: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.NavigateTo+ContainerType: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.NavigateTo+ContainerType: Boolean IsException
FSharp.Compiler.SourceCodeServices.NavigateTo+ContainerType: Boolean IsFile
FSharp.Compiler.SourceCodeServices.NavigateTo+ContainerType: Boolean IsModule
FSharp.Compiler.SourceCodeServices.NavigateTo+ContainerType: Boolean IsNamespace
FSharp.Compiler.SourceCodeServices.NavigateTo+ContainerType: Boolean IsType
FSharp.Compiler.SourceCodeServices.NavigateTo+ContainerType: Boolean get_IsException()
FSharp.Compiler.SourceCodeServices.NavigateTo+ContainerType: Boolean get_IsFile()
FSharp.Compiler.SourceCodeServices.NavigateTo+ContainerType: Boolean get_IsModule()
FSharp.Compiler.SourceCodeServices.NavigateTo+ContainerType: Boolean get_IsNamespace()
FSharp.Compiler.SourceCodeServices.NavigateTo+ContainerType: Boolean get_IsType()
FSharp.Compiler.SourceCodeServices.NavigateTo+ContainerType: ContainerType Exception
FSharp.Compiler.SourceCodeServices.NavigateTo+ContainerType: ContainerType File
FSharp.Compiler.SourceCodeServices.NavigateTo+ContainerType: ContainerType Module
FSharp.Compiler.SourceCodeServices.NavigateTo+ContainerType: ContainerType Namespace
FSharp.Compiler.SourceCodeServices.NavigateTo+ContainerType: ContainerType Type
FSharp.Compiler.SourceCodeServices.NavigateTo+ContainerType: ContainerType get_Exception()
FSharp.Compiler.SourceCodeServices.NavigateTo+ContainerType: ContainerType get_File()
FSharp.Compiler.SourceCodeServices.NavigateTo+ContainerType: ContainerType get_Module()
FSharp.Compiler.SourceCodeServices.NavigateTo+ContainerType: ContainerType get_Namespace()
FSharp.Compiler.SourceCodeServices.NavigateTo+ContainerType: ContainerType get_Type()
FSharp.Compiler.SourceCodeServices.NavigateTo+ContainerType: FSharp.Compiler.SourceCodeServices.NavigateTo+ContainerType+Tags
FSharp.Compiler.SourceCodeServices.NavigateTo+ContainerType: Int32 CompareTo(ContainerType)
FSharp.Compiler.SourceCodeServices.NavigateTo+ContainerType: Int32 CompareTo(System.Object)
FSharp.Compiler.SourceCodeServices.NavigateTo+ContainerType: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.SourceCodeServices.NavigateTo+ContainerType: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.NavigateTo+ContainerType: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.NavigateTo+ContainerType: Int32 Tag
FSharp.Compiler.SourceCodeServices.NavigateTo+ContainerType: Int32 get_Tag()
FSharp.Compiler.SourceCodeServices.NavigateTo+ContainerType: System.String ToString()
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItem: Boolean Equals(NavigableItem)
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItem: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItem: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItem: Boolean IsSignature
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItem: Boolean get_IsSignature()
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItem: Container Container
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItem: Container get_Container()
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItem: FSharp.Compiler.Text.Range Range
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItem: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItem: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItem: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItem: NavigableItemKind Kind
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItem: NavigableItemKind get_Kind()
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItem: System.String Name
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItem: System.String ToString()
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItem: System.String get_Name()
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItem: Void .ctor(System.String, FSharp.Compiler.Text.Range, Boolean, NavigableItemKind, Container)
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind+Tags: Int32 Constructor
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind+Tags: Int32 EnumCase
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind+Tags: Int32 Exception
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind+Tags: Int32 Field
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind+Tags: Int32 Member
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind+Tags: Int32 Module
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind+Tags: Int32 ModuleAbbreviation
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind+Tags: Int32 ModuleValue
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind+Tags: Int32 Property
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind+Tags: Int32 Type
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind+Tags: Int32 UnionCase
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: Boolean Equals(NavigableItemKind)
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: Boolean IsConstructor
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: Boolean IsEnumCase
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: Boolean IsException
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: Boolean IsField
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: Boolean IsMember
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: Boolean IsModule
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: Boolean IsModuleAbbreviation
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: Boolean IsModuleValue
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: Boolean IsProperty
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: Boolean IsType
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: Boolean IsUnionCase
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: Boolean get_IsConstructor()
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: Boolean get_IsEnumCase()
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: Boolean get_IsException()
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: Boolean get_IsField()
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: Boolean get_IsMember()
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: Boolean get_IsModule()
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: Boolean get_IsModuleAbbreviation()
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: Boolean get_IsModuleValue()
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: Boolean get_IsProperty()
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: Boolean get_IsType()
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: Boolean get_IsUnionCase()
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind+Tags
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: Int32 CompareTo(NavigableItemKind)
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: Int32 CompareTo(System.Object)
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: Int32 Tag
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: Int32 get_Tag()
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: NavigableItemKind Constructor
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: NavigableItemKind EnumCase
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: NavigableItemKind Exception
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: NavigableItemKind Field
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: NavigableItemKind Member
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: NavigableItemKind Module
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: NavigableItemKind ModuleAbbreviation
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: NavigableItemKind ModuleValue
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: NavigableItemKind Property
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: NavigableItemKind Type
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: NavigableItemKind UnionCase
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: NavigableItemKind get_Constructor()
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: NavigableItemKind get_EnumCase()
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: NavigableItemKind get_Exception()
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: NavigableItemKind get_Field()
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: NavigableItemKind get_Member()
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: NavigableItemKind get_Module()
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: NavigableItemKind get_ModuleAbbreviation()
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: NavigableItemKind get_ModuleValue()
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: NavigableItemKind get_Property()
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: NavigableItemKind get_Type()
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: NavigableItemKind get_UnionCase()
FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind: System.String ToString()
FSharp.Compiler.SourceCodeServices.NavigateTo: FSharp.Compiler.SourceCodeServices.NavigateTo+Container
FSharp.Compiler.SourceCodeServices.NavigateTo: FSharp.Compiler.SourceCodeServices.NavigateTo+ContainerType
FSharp.Compiler.SourceCodeServices.NavigateTo: FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItem
FSharp.Compiler.SourceCodeServices.NavigateTo: FSharp.Compiler.SourceCodeServices.NavigateTo+NavigableItemKind
FSharp.Compiler.SourceCodeServices.NavigateTo: NavigableItem[] getNavigableItems(ParsedInput)
FSharp.Compiler.SourceCodeServices.OpenStatementInsertionPoint
FSharp.Compiler.SourceCodeServices.OpenStatementInsertionPoint+Tags: Int32 Nearest
FSharp.Compiler.SourceCodeServices.OpenStatementInsertionPoint+Tags: Int32 TopLevel
FSharp.Compiler.SourceCodeServices.OpenStatementInsertionPoint: Boolean Equals(FSharp.Compiler.SourceCodeServices.OpenStatementInsertionPoint)
FSharp.Compiler.SourceCodeServices.OpenStatementInsertionPoint: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.OpenStatementInsertionPoint: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.OpenStatementInsertionPoint: Boolean IsNearest
FSharp.Compiler.SourceCodeServices.OpenStatementInsertionPoint: Boolean IsTopLevel
FSharp.Compiler.SourceCodeServices.OpenStatementInsertionPoint: Boolean get_IsNearest()
FSharp.Compiler.SourceCodeServices.OpenStatementInsertionPoint: Boolean get_IsTopLevel()
FSharp.Compiler.SourceCodeServices.OpenStatementInsertionPoint: FSharp.Compiler.SourceCodeServices.OpenStatementInsertionPoint Nearest
FSharp.Compiler.SourceCodeServices.OpenStatementInsertionPoint: FSharp.Compiler.SourceCodeServices.OpenStatementInsertionPoint TopLevel
FSharp.Compiler.SourceCodeServices.OpenStatementInsertionPoint: FSharp.Compiler.SourceCodeServices.OpenStatementInsertionPoint get_Nearest()
FSharp.Compiler.SourceCodeServices.OpenStatementInsertionPoint: FSharp.Compiler.SourceCodeServices.OpenStatementInsertionPoint get_TopLevel()
FSharp.Compiler.SourceCodeServices.OpenStatementInsertionPoint: FSharp.Compiler.SourceCodeServices.OpenStatementInsertionPoint+Tags
FSharp.Compiler.SourceCodeServices.OpenStatementInsertionPoint: Int32 CompareTo(FSharp.Compiler.SourceCodeServices.OpenStatementInsertionPoint)
FSharp.Compiler.SourceCodeServices.OpenStatementInsertionPoint: Int32 CompareTo(System.Object)
FSharp.Compiler.SourceCodeServices.OpenStatementInsertionPoint: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.SourceCodeServices.OpenStatementInsertionPoint: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.OpenStatementInsertionPoint: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.OpenStatementInsertionPoint: Int32 Tag
FSharp.Compiler.SourceCodeServices.OpenStatementInsertionPoint: Int32 get_Tag()
FSharp.Compiler.SourceCodeServices.OpenStatementInsertionPoint: System.String ToString()
FSharp.Compiler.SourceCodeServices.ParamTypeSymbol
FSharp.Compiler.SourceCodeServices.ParamTypeSymbol+Byref: FSharp.Compiler.SourceCodeServices.ExternalType get_parameterType()
FSharp.Compiler.SourceCodeServices.ParamTypeSymbol+Byref: FSharp.Compiler.SourceCodeServices.ExternalType parameterType
FSharp.Compiler.SourceCodeServices.ParamTypeSymbol+Param: FSharp.Compiler.SourceCodeServices.ExternalType get_parameterType()
FSharp.Compiler.SourceCodeServices.ParamTypeSymbol+Param: FSharp.Compiler.SourceCodeServices.ExternalType parameterType
FSharp.Compiler.SourceCodeServices.ParamTypeSymbol+Tags: Int32 Byref
FSharp.Compiler.SourceCodeServices.ParamTypeSymbol+Tags: Int32 Param
FSharp.Compiler.SourceCodeServices.ParamTypeSymbol: Boolean Equals(FSharp.Compiler.SourceCodeServices.ParamTypeSymbol)
FSharp.Compiler.SourceCodeServices.ParamTypeSymbol: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.ParamTypeSymbol: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.ParamTypeSymbol: Boolean IsByref
FSharp.Compiler.SourceCodeServices.ParamTypeSymbol: Boolean IsParam
FSharp.Compiler.SourceCodeServices.ParamTypeSymbol: Boolean get_IsByref()
FSharp.Compiler.SourceCodeServices.ParamTypeSymbol: Boolean get_IsParam()
FSharp.Compiler.SourceCodeServices.ParamTypeSymbol: FSharp.Compiler.SourceCodeServices.ParamTypeSymbol NewByref(FSharp.Compiler.SourceCodeServices.ExternalType)
FSharp.Compiler.SourceCodeServices.ParamTypeSymbol: FSharp.Compiler.SourceCodeServices.ParamTypeSymbol NewParam(FSharp.Compiler.SourceCodeServices.ExternalType)
FSharp.Compiler.SourceCodeServices.ParamTypeSymbol: FSharp.Compiler.SourceCodeServices.ParamTypeSymbol+Byref
FSharp.Compiler.SourceCodeServices.ParamTypeSymbol: FSharp.Compiler.SourceCodeServices.ParamTypeSymbol+Param
FSharp.Compiler.SourceCodeServices.ParamTypeSymbol: FSharp.Compiler.SourceCodeServices.ParamTypeSymbol+Tags
FSharp.Compiler.SourceCodeServices.ParamTypeSymbol: Int32 CompareTo(FSharp.Compiler.SourceCodeServices.ParamTypeSymbol)
FSharp.Compiler.SourceCodeServices.ParamTypeSymbol: Int32 CompareTo(System.Object)
FSharp.Compiler.SourceCodeServices.ParamTypeSymbol: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.SourceCodeServices.ParamTypeSymbol: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.ParamTypeSymbol: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.ParamTypeSymbol: Int32 Tag
FSharp.Compiler.SourceCodeServices.ParamTypeSymbol: Int32 get_Tag()
FSharp.Compiler.SourceCodeServices.ParamTypeSymbol: System.String ToString()
FSharp.Compiler.SourceCodeServices.ParamTypeSymbolModule
FSharp.Compiler.SourceCodeServices.ParsedInput
FSharp.Compiler.SourceCodeServices.ParsedInput: FSharp.Compiler.SourceCodeServices.InsertContext findNearestPointToInsertOpenDeclaration(Int32, ParsedInput, System.String[], FSharp.Compiler.SourceCodeServices.OpenStatementInsertionPoint)
FSharp.Compiler.SourceCodeServices.ParsedInput: FSharp.Compiler.Text.Pos adjustInsertionPoint(Microsoft.FSharp.Core.FSharpFunc`2[System.Int32,System.String], FSharp.Compiler.SourceCodeServices.InsertContext)
FSharp.Compiler.SourceCodeServices.ParsedInput: Microsoft.FSharp.Core.FSharpFunc`2[System.Tuple`4[Microsoft.FSharp.Core.FSharpOption`1[System.String[]],Microsoft.FSharp.Core.FSharpOption`1[System.String[]],Microsoft.FSharp.Core.FSharpOption`1[System.String[]],System.String[]],System.Tuple`2[FSharp.Compiler.SourceCodeServices.Entity,FSharp.Compiler.SourceCodeServices.InsertContext][]] tryFindInsertionContext(Int32, ParsedInput, FSharp.Compiler.SourceCodeServices.MaybeUnresolvedIdent[], FSharp.Compiler.SourceCodeServices.OpenStatementInsertionPoint)
FSharp.Compiler.SourceCodeServices.ParsedInput: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident]] getLongIdentAt(ParsedInput, FSharp.Compiler.Text.Pos)
FSharp.Compiler.SourceCodeServices.PartialLongName
FSharp.Compiler.SourceCodeServices.PartialLongName: Boolean Equals(FSharp.Compiler.SourceCodeServices.PartialLongName)
FSharp.Compiler.SourceCodeServices.PartialLongName: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.PartialLongName: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.PartialLongName: FSharp.Compiler.SourceCodeServices.PartialLongName Empty(Int32)
FSharp.Compiler.SourceCodeServices.PartialLongName: Int32 CompareTo(FSharp.Compiler.SourceCodeServices.PartialLongName)
FSharp.Compiler.SourceCodeServices.PartialLongName: Int32 CompareTo(System.Object)
FSharp.Compiler.SourceCodeServices.PartialLongName: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.SourceCodeServices.PartialLongName: Int32 EndColumn
FSharp.Compiler.SourceCodeServices.PartialLongName: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.PartialLongName: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.PartialLongName: Int32 get_EndColumn()
FSharp.Compiler.SourceCodeServices.PartialLongName: Microsoft.FSharp.Collections.FSharpList`1[System.String] QualifyingIdents
FSharp.Compiler.SourceCodeServices.PartialLongName: Microsoft.FSharp.Collections.FSharpList`1[System.String] get_QualifyingIdents()
FSharp.Compiler.SourceCodeServices.PartialLongName: Microsoft.FSharp.Core.FSharpOption`1[System.Int32] LastDotPos
FSharp.Compiler.SourceCodeServices.PartialLongName: Microsoft.FSharp.Core.FSharpOption`1[System.Int32] get_LastDotPos()
FSharp.Compiler.SourceCodeServices.PartialLongName: System.String PartialIdent
FSharp.Compiler.SourceCodeServices.PartialLongName: System.String ToString()
FSharp.Compiler.SourceCodeServices.PartialLongName: System.String get_PartialIdent()
FSharp.Compiler.SourceCodeServices.PartialLongName: Void .ctor(Microsoft.FSharp.Collections.FSharpList`1[System.String], System.String, Int32, Microsoft.FSharp.Core.FSharpOption`1[System.Int32])
FSharp.Compiler.SourceCodeServices.PrettyNaming
FSharp.Compiler.SourceCodeServices.PrettyNaming: Boolean IsActivePatternName(System.String)
FSharp.Compiler.SourceCodeServices.PrettyNaming: Boolean IsCompilerGeneratedName(System.String)
FSharp.Compiler.SourceCodeServices.PrettyNaming: Boolean IsIdentifierFirstCharacter(Char)
FSharp.Compiler.SourceCodeServices.PrettyNaming: Boolean IsIdentifierPartCharacter(Char)
FSharp.Compiler.SourceCodeServices.PrettyNaming: Boolean IsInfixOperator(System.String)
FSharp.Compiler.SourceCodeServices.PrettyNaming: Boolean IsLongIdentifierPartCharacter(Char)
FSharp.Compiler.SourceCodeServices.PrettyNaming: Boolean IsMangledOpName(System.String)
FSharp.Compiler.SourceCodeServices.PrettyNaming: Boolean IsOperatorName(System.String)
FSharp.Compiler.SourceCodeServices.PrettyNaming: Boolean IsOperatorOrBacktickedName(System.String)
FSharp.Compiler.SourceCodeServices.PrettyNaming: Boolean IsPrefixOperator(System.String)
FSharp.Compiler.SourceCodeServices.PrettyNaming: Boolean IsPunctuation(System.String)
FSharp.Compiler.SourceCodeServices.PrettyNaming: Boolean IsTernaryOperator(System.String)
FSharp.Compiler.SourceCodeServices.PrettyNaming: Microsoft.FSharp.Collections.FSharpList`1[System.String] GetLongNameFromString(System.String)
FSharp.Compiler.SourceCodeServices.PrettyNaming: Microsoft.FSharp.Core.FSharpOption`1[System.String] TryChopPropertyName(System.String)
FSharp.Compiler.SourceCodeServices.PrettyNaming: System.String CompileOpName(System.String)
FSharp.Compiler.SourceCodeServices.PrettyNaming: System.String DecompileOpName(System.String)
FSharp.Compiler.SourceCodeServices.PrettyNaming: System.String DemangleOperatorName(System.String)
FSharp.Compiler.SourceCodeServices.PrettyNaming: System.String FormatAndOtherOverloadsString(Int32)
FSharp.Compiler.SourceCodeServices.PrettyNaming: System.String FsiDynamicModulePrefix
FSharp.Compiler.SourceCodeServices.PrettyNaming: System.String get_FsiDynamicModulePrefix()
FSharp.Compiler.SourceCodeServices.QuickParse
FSharp.Compiler.SourceCodeServices.QuickParse: Boolean TestMemberOrOverrideDeclaration(FSharp.Compiler.SourceCodeServices.FSharpTokenInfo[])
FSharp.Compiler.SourceCodeServices.QuickParse: FSharp.Compiler.SourceCodeServices.PartialLongName GetPartialLongNameEx(System.String, Int32)
FSharp.Compiler.SourceCodeServices.QuickParse: Int32 CorrectIdentifierToken(System.String, Int32)
FSharp.Compiler.SourceCodeServices.QuickParse: Int32 MagicalAdjustmentConstant
FSharp.Compiler.SourceCodeServices.QuickParse: Int32 get_MagicalAdjustmentConstant()
FSharp.Compiler.SourceCodeServices.QuickParse: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`3[System.String,System.Int32,System.Boolean]] GetCompleteIdentifierIsland(Boolean, System.String, Int32)
FSharp.Compiler.SourceCodeServices.QuickParse: System.Tuple`2[Microsoft.FSharp.Collections.FSharpList`1[System.String],System.String] GetPartialLongName(System.String, Int32)
FSharp.Compiler.SourceCodeServices.RecordContext
FSharp.Compiler.SourceCodeServices.RecordContext+Constructor: System.String get_typeName()
FSharp.Compiler.SourceCodeServices.RecordContext+Constructor: System.String typeName
FSharp.Compiler.SourceCodeServices.RecordContext+CopyOnUpdate: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SourceCodeServices.RecordContext+CopyOnUpdate: FSharp.Compiler.Text.Range range
FSharp.Compiler.SourceCodeServices.RecordContext+CopyOnUpdate: System.Tuple`2[Microsoft.FSharp.Collections.FSharpList`1[System.String],Microsoft.FSharp.Core.FSharpOption`1[System.String]] get_path()
FSharp.Compiler.SourceCodeServices.RecordContext+CopyOnUpdate: System.Tuple`2[Microsoft.FSharp.Collections.FSharpList`1[System.String],Microsoft.FSharp.Core.FSharpOption`1[System.String]] path
FSharp.Compiler.SourceCodeServices.RecordContext+New: System.Tuple`2[Microsoft.FSharp.Collections.FSharpList`1[System.String],Microsoft.FSharp.Core.FSharpOption`1[System.String]] get_path()
FSharp.Compiler.SourceCodeServices.RecordContext+New: System.Tuple`2[Microsoft.FSharp.Collections.FSharpList`1[System.String],Microsoft.FSharp.Core.FSharpOption`1[System.String]] path
FSharp.Compiler.SourceCodeServices.RecordContext+Tags: Int32 Constructor
FSharp.Compiler.SourceCodeServices.RecordContext+Tags: Int32 CopyOnUpdate
FSharp.Compiler.SourceCodeServices.RecordContext+Tags: Int32 New
FSharp.Compiler.SourceCodeServices.RecordContext: Boolean Equals(FSharp.Compiler.SourceCodeServices.RecordContext)
FSharp.Compiler.SourceCodeServices.RecordContext: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.RecordContext: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.RecordContext: Boolean IsConstructor
FSharp.Compiler.SourceCodeServices.RecordContext: Boolean IsCopyOnUpdate
FSharp.Compiler.SourceCodeServices.RecordContext: Boolean IsNew
FSharp.Compiler.SourceCodeServices.RecordContext: Boolean get_IsConstructor()
FSharp.Compiler.SourceCodeServices.RecordContext: Boolean get_IsCopyOnUpdate()
FSharp.Compiler.SourceCodeServices.RecordContext: Boolean get_IsNew()
FSharp.Compiler.SourceCodeServices.RecordContext: FSharp.Compiler.SourceCodeServices.RecordContext NewConstructor(System.String)
FSharp.Compiler.SourceCodeServices.RecordContext: FSharp.Compiler.SourceCodeServices.RecordContext NewCopyOnUpdate(FSharp.Compiler.Text.Range, System.Tuple`2[Microsoft.FSharp.Collections.FSharpList`1[System.String],Microsoft.FSharp.Core.FSharpOption`1[System.String]])
FSharp.Compiler.SourceCodeServices.RecordContext: FSharp.Compiler.SourceCodeServices.RecordContext NewNew(System.Tuple`2[Microsoft.FSharp.Collections.FSharpList`1[System.String],Microsoft.FSharp.Core.FSharpOption`1[System.String]])
FSharp.Compiler.SourceCodeServices.RecordContext: FSharp.Compiler.SourceCodeServices.RecordContext+Constructor
FSharp.Compiler.SourceCodeServices.RecordContext: FSharp.Compiler.SourceCodeServices.RecordContext+CopyOnUpdate
FSharp.Compiler.SourceCodeServices.RecordContext: FSharp.Compiler.SourceCodeServices.RecordContext+New
FSharp.Compiler.SourceCodeServices.RecordContext: FSharp.Compiler.SourceCodeServices.RecordContext+Tags
FSharp.Compiler.SourceCodeServices.RecordContext: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.RecordContext: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.RecordContext: Int32 Tag
FSharp.Compiler.SourceCodeServices.RecordContext: Int32 get_Tag()
FSharp.Compiler.SourceCodeServices.RecordContext: System.String ToString()
FSharp.Compiler.SourceCodeServices.ScopeKind
FSharp.Compiler.SourceCodeServices.ScopeKind+Tags: Int32 HashDirective
FSharp.Compiler.SourceCodeServices.ScopeKind+Tags: Int32 Namespace
FSharp.Compiler.SourceCodeServices.ScopeKind+Tags: Int32 NestedModule
FSharp.Compiler.SourceCodeServices.ScopeKind+Tags: Int32 OpenDeclaration
FSharp.Compiler.SourceCodeServices.ScopeKind+Tags: Int32 TopModule
FSharp.Compiler.SourceCodeServices.ScopeKind: Boolean Equals(FSharp.Compiler.SourceCodeServices.ScopeKind)
FSharp.Compiler.SourceCodeServices.ScopeKind: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.ScopeKind: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.ScopeKind: Boolean IsHashDirective
FSharp.Compiler.SourceCodeServices.ScopeKind: Boolean IsNamespace
FSharp.Compiler.SourceCodeServices.ScopeKind: Boolean IsNestedModule
FSharp.Compiler.SourceCodeServices.ScopeKind: Boolean IsOpenDeclaration
FSharp.Compiler.SourceCodeServices.ScopeKind: Boolean IsTopModule
FSharp.Compiler.SourceCodeServices.ScopeKind: Boolean get_IsHashDirective()
FSharp.Compiler.SourceCodeServices.ScopeKind: Boolean get_IsNamespace()
FSharp.Compiler.SourceCodeServices.ScopeKind: Boolean get_IsNestedModule()
FSharp.Compiler.SourceCodeServices.ScopeKind: Boolean get_IsOpenDeclaration()
FSharp.Compiler.SourceCodeServices.ScopeKind: Boolean get_IsTopModule()
FSharp.Compiler.SourceCodeServices.ScopeKind: FSharp.Compiler.SourceCodeServices.ScopeKind HashDirective
FSharp.Compiler.SourceCodeServices.ScopeKind: FSharp.Compiler.SourceCodeServices.ScopeKind Namespace
FSharp.Compiler.SourceCodeServices.ScopeKind: FSharp.Compiler.SourceCodeServices.ScopeKind NestedModule
FSharp.Compiler.SourceCodeServices.ScopeKind: FSharp.Compiler.SourceCodeServices.ScopeKind OpenDeclaration
FSharp.Compiler.SourceCodeServices.ScopeKind: FSharp.Compiler.SourceCodeServices.ScopeKind TopModule
FSharp.Compiler.SourceCodeServices.ScopeKind: FSharp.Compiler.SourceCodeServices.ScopeKind get_HashDirective()
FSharp.Compiler.SourceCodeServices.ScopeKind: FSharp.Compiler.SourceCodeServices.ScopeKind get_Namespace()
FSharp.Compiler.SourceCodeServices.ScopeKind: FSharp.Compiler.SourceCodeServices.ScopeKind get_NestedModule()
FSharp.Compiler.SourceCodeServices.ScopeKind: FSharp.Compiler.SourceCodeServices.ScopeKind get_OpenDeclaration()
FSharp.Compiler.SourceCodeServices.ScopeKind: FSharp.Compiler.SourceCodeServices.ScopeKind get_TopModule()
FSharp.Compiler.SourceCodeServices.ScopeKind: FSharp.Compiler.SourceCodeServices.ScopeKind+Tags
FSharp.Compiler.SourceCodeServices.ScopeKind: Int32 CompareTo(FSharp.Compiler.SourceCodeServices.ScopeKind)
FSharp.Compiler.SourceCodeServices.ScopeKind: Int32 CompareTo(System.Object)
FSharp.Compiler.SourceCodeServices.ScopeKind: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.SourceCodeServices.ScopeKind: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.ScopeKind: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.ScopeKind: Int32 Tag
FSharp.Compiler.SourceCodeServices.ScopeKind: Int32 get_Tag()
FSharp.Compiler.SourceCodeServices.ScopeKind: System.String ToString()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType
FSharp.Compiler.SourceCodeServices.SemanticClassificationType+Tags: Int32 ComputationExpression
FSharp.Compiler.SourceCodeServices.SemanticClassificationType+Tags: Int32 ConstructorForReferenceType
FSharp.Compiler.SourceCodeServices.SemanticClassificationType+Tags: Int32 ConstructorForValueType
FSharp.Compiler.SourceCodeServices.SemanticClassificationType+Tags: Int32 Delegate
FSharp.Compiler.SourceCodeServices.SemanticClassificationType+Tags: Int32 DisposableLocalValue
FSharp.Compiler.SourceCodeServices.SemanticClassificationType+Tags: Int32 DisposableTopLevelValue
FSharp.Compiler.SourceCodeServices.SemanticClassificationType+Tags: Int32 DisposableType
FSharp.Compiler.SourceCodeServices.SemanticClassificationType+Tags: Int32 Enumeration
FSharp.Compiler.SourceCodeServices.SemanticClassificationType+Tags: Int32 Event
FSharp.Compiler.SourceCodeServices.SemanticClassificationType+Tags: Int32 Exception
FSharp.Compiler.SourceCodeServices.SemanticClassificationType+Tags: Int32 ExtensionMethod
FSharp.Compiler.SourceCodeServices.SemanticClassificationType+Tags: Int32 Field
FSharp.Compiler.SourceCodeServices.SemanticClassificationType+Tags: Int32 Function
FSharp.Compiler.SourceCodeServices.SemanticClassificationType+Tags: Int32 Interface
FSharp.Compiler.SourceCodeServices.SemanticClassificationType+Tags: Int32 IntrinsicFunction
FSharp.Compiler.SourceCodeServices.SemanticClassificationType+Tags: Int32 Literal
FSharp.Compiler.SourceCodeServices.SemanticClassificationType+Tags: Int32 LocalValue
FSharp.Compiler.SourceCodeServices.SemanticClassificationType+Tags: Int32 Method
FSharp.Compiler.SourceCodeServices.SemanticClassificationType+Tags: Int32 Module
FSharp.Compiler.SourceCodeServices.SemanticClassificationType+Tags: Int32 MutableRecordField
FSharp.Compiler.SourceCodeServices.SemanticClassificationType+Tags: Int32 MutableVar
FSharp.Compiler.SourceCodeServices.SemanticClassificationType+Tags: Int32 NamedArgument
FSharp.Compiler.SourceCodeServices.SemanticClassificationType+Tags: Int32 Namespace
FSharp.Compiler.SourceCodeServices.SemanticClassificationType+Tags: Int32 Operator
FSharp.Compiler.SourceCodeServices.SemanticClassificationType+Tags: Int32 Plaintext
FSharp.Compiler.SourceCodeServices.SemanticClassificationType+Tags: Int32 Printf
FSharp.Compiler.SourceCodeServices.SemanticClassificationType+Tags: Int32 Property
FSharp.Compiler.SourceCodeServices.SemanticClassificationType+Tags: Int32 RecordField
FSharp.Compiler.SourceCodeServices.SemanticClassificationType+Tags: Int32 RecordFieldAsFunction
FSharp.Compiler.SourceCodeServices.SemanticClassificationType+Tags: Int32 ReferenceType
FSharp.Compiler.SourceCodeServices.SemanticClassificationType+Tags: Int32 Type
FSharp.Compiler.SourceCodeServices.SemanticClassificationType+Tags: Int32 TypeArgument
FSharp.Compiler.SourceCodeServices.SemanticClassificationType+Tags: Int32 TypeDef
FSharp.Compiler.SourceCodeServices.SemanticClassificationType+Tags: Int32 UnionCase
FSharp.Compiler.SourceCodeServices.SemanticClassificationType+Tags: Int32 UnionCaseField
FSharp.Compiler.SourceCodeServices.SemanticClassificationType+Tags: Int32 Value
FSharp.Compiler.SourceCodeServices.SemanticClassificationType+Tags: Int32 ValueType
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean Equals(FSharp.Compiler.SourceCodeServices.SemanticClassificationType)
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean IsComputationExpression
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean IsConstructorForReferenceType
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean IsConstructorForValueType
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean IsDelegate
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean IsDisposableLocalValue
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean IsDisposableTopLevelValue
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean IsDisposableType
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean IsEnumeration
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean IsEvent
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean IsException
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean IsExtensionMethod
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean IsField
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean IsFunction
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean IsInterface
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean IsIntrinsicFunction
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean IsLiteral
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean IsLocalValue
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean IsMethod
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean IsModule
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean IsMutableRecordField
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean IsMutableVar
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean IsNamedArgument
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean IsNamespace
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean IsOperator
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean IsPlaintext
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean IsPrintf
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean IsProperty
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean IsRecordField
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean IsRecordFieldAsFunction
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean IsReferenceType
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean IsType
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean IsTypeArgument
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean IsTypeDef
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean IsUnionCase
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean IsUnionCaseField
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean IsValue
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean IsValueType
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean get_IsComputationExpression()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean get_IsConstructorForReferenceType()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean get_IsConstructorForValueType()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean get_IsDelegate()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean get_IsDisposableLocalValue()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean get_IsDisposableTopLevelValue()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean get_IsDisposableType()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean get_IsEnumeration()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean get_IsEvent()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean get_IsException()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean get_IsExtensionMethod()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean get_IsField()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean get_IsFunction()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean get_IsInterface()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean get_IsIntrinsicFunction()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean get_IsLiteral()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean get_IsLocalValue()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean get_IsMethod()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean get_IsModule()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean get_IsMutableRecordField()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean get_IsMutableVar()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean get_IsNamedArgument()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean get_IsNamespace()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean get_IsOperator()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean get_IsPlaintext()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean get_IsPrintf()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean get_IsProperty()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean get_IsRecordField()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean get_IsRecordFieldAsFunction()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean get_IsReferenceType()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean get_IsType()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean get_IsTypeArgument()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean get_IsTypeDef()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean get_IsUnionCase()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean get_IsUnionCaseField()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean get_IsValue()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Boolean get_IsValueType()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType ComputationExpression
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType ConstructorForReferenceType
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType ConstructorForValueType
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType Delegate
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType DisposableLocalValue
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType DisposableTopLevelValue
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType DisposableType
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType Enumeration
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType Event
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType Exception
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType ExtensionMethod
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType Field
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType Function
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType Interface
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType IntrinsicFunction
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType Literal
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType LocalValue
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType Method
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType Module
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType MutableRecordField
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType MutableVar
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType NamedArgument
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType Namespace
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType Operator
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType Plaintext
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType Printf
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType Property
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType RecordField
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType RecordFieldAsFunction
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType ReferenceType
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType Type
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType TypeArgument
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType TypeDef
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType UnionCase
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType UnionCaseField
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType Value
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType ValueType
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType get_ComputationExpression()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType get_ConstructorForReferenceType()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType get_ConstructorForValueType()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType get_Delegate()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType get_DisposableLocalValue()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType get_DisposableTopLevelValue()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType get_DisposableType()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType get_Enumeration()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType get_Event()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType get_Exception()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType get_ExtensionMethod()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType get_Field()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType get_Function()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType get_Interface()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType get_IntrinsicFunction()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType get_Literal()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType get_LocalValue()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType get_Method()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType get_Module()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType get_MutableRecordField()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType get_MutableVar()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType get_NamedArgument()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType get_Namespace()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType get_Operator()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType get_Plaintext()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType get_Printf()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType get_Property()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType get_RecordField()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType get_RecordFieldAsFunction()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType get_ReferenceType()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType get_Type()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType get_TypeArgument()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType get_TypeDef()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType get_UnionCase()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType get_UnionCaseField()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType get_Value()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType get_ValueType()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: FSharp.Compiler.SourceCodeServices.SemanticClassificationType+Tags
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Int32 CompareTo(FSharp.Compiler.SourceCodeServices.SemanticClassificationType)
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Int32 CompareTo(System.Object)
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Int32 Tag
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: Int32 get_Tag()
FSharp.Compiler.SourceCodeServices.SemanticClassificationType: System.String ToString()
FSharp.Compiler.SourceCodeServices.SimplifyNames
FSharp.Compiler.SourceCodeServices.SimplifyNames+SimplifiableRange: Boolean Equals(SimplifiableRange)
FSharp.Compiler.SourceCodeServices.SimplifyNames+SimplifiableRange: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.SimplifyNames+SimplifiableRange: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.SimplifyNames+SimplifiableRange: FSharp.Compiler.Text.Range Range
FSharp.Compiler.SourceCodeServices.SimplifyNames+SimplifiableRange: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.SourceCodeServices.SimplifyNames+SimplifiableRange: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.SimplifyNames+SimplifiableRange: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.SimplifyNames+SimplifiableRange: System.String RelativeName
FSharp.Compiler.SourceCodeServices.SimplifyNames+SimplifiableRange: System.String ToString()
FSharp.Compiler.SourceCodeServices.SimplifyNames+SimplifiableRange: System.String get_RelativeName()
FSharp.Compiler.SourceCodeServices.SimplifyNames+SimplifiableRange: Void .ctor(FSharp.Compiler.Text.Range, System.String)
FSharp.Compiler.SourceCodeServices.SimplifyNames: FSharp.Compiler.SourceCodeServices.SimplifyNames+SimplifiableRange
FSharp.Compiler.SourceCodeServices.SimplifyNames: Microsoft.FSharp.Control.FSharpAsync`1[System.Collections.Generic.IEnumerable`1[FSharp.Compiler.SourceCodeServices.SimplifyNames+SimplifiableRange]] getSimplifiableNames(FSharp.Compiler.SourceCodeServices.FSharpCheckFileResults, Microsoft.FSharp.Core.FSharpFunc`2[System.Int32,System.String])
FSharp.Compiler.SourceCodeServices.SourceFile
FSharp.Compiler.SourceCodeServices.SourceFile: Boolean IsCompilable(System.String)
FSharp.Compiler.SourceCodeServices.SourceFile: Boolean MustBeSingleFileProject(System.String)
FSharp.Compiler.SourceCodeServices.Structure
FSharp.Compiler.SourceCodeServices.Structure+Collapse+Tags: Int32 Below
FSharp.Compiler.SourceCodeServices.Structure+Collapse+Tags: Int32 Same
FSharp.Compiler.SourceCodeServices.Structure+Collapse: Boolean Equals(Collapse)
FSharp.Compiler.SourceCodeServices.Structure+Collapse: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.Structure+Collapse: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.Structure+Collapse: Boolean IsBelow
FSharp.Compiler.SourceCodeServices.Structure+Collapse: Boolean IsSame
FSharp.Compiler.SourceCodeServices.Structure+Collapse: Boolean get_IsBelow()
FSharp.Compiler.SourceCodeServices.Structure+Collapse: Boolean get_IsSame()
FSharp.Compiler.SourceCodeServices.Structure+Collapse: Collapse Below
FSharp.Compiler.SourceCodeServices.Structure+Collapse: Collapse Same
FSharp.Compiler.SourceCodeServices.Structure+Collapse: Collapse get_Below()
FSharp.Compiler.SourceCodeServices.Structure+Collapse: Collapse get_Same()
FSharp.Compiler.SourceCodeServices.Structure+Collapse: FSharp.Compiler.SourceCodeServices.Structure+Collapse+Tags
FSharp.Compiler.SourceCodeServices.Structure+Collapse: Int32 CompareTo(Collapse)
FSharp.Compiler.SourceCodeServices.Structure+Collapse: Int32 CompareTo(System.Object)
FSharp.Compiler.SourceCodeServices.Structure+Collapse: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.SourceCodeServices.Structure+Collapse: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.Structure+Collapse: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.Structure+Collapse: Int32 Tag
FSharp.Compiler.SourceCodeServices.Structure+Collapse: Int32 get_Tag()
FSharp.Compiler.SourceCodeServices.Structure+Collapse: System.String ToString()
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 ArrayOrList
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 Attribute
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 Comment
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 CompExpr
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 CompExprInternal
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 Do
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 ElseInIfThenElse
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 EnumCase
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 FinallyInTryFinally
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 For
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 HashDirective
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 IfThenElse
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 Interface
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 Lambda
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 LetOrUse
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 LetOrUseBang
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 Match
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 MatchBang
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 MatchClause
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 MatchLambda
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 Member
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 Module
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 Namespace
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 New
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 ObjExpr
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 Open
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 Quote
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 Record
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 RecordDefn
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 RecordField
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 SpecialFunc
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 ThenInIfThenElse
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 TryFinally
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 TryInTryFinally
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 TryInTryWith
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 TryWith
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 Tuple
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 Type
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 TypeExtension
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 UnionCase
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 UnionDefn
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 Val
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 While
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 WithInTryWith
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 XmlDocComment
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 YieldOrReturn
FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags: Int32 YieldOrReturnBang
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean Equals(Scope)
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsArrayOrList
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsAttribute
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsComment
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsCompExpr
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsCompExprInternal
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsDo
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsElseInIfThenElse
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsEnumCase
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsFinallyInTryFinally
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsFor
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsHashDirective
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsIfThenElse
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsInterface
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsLambda
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsLetOrUse
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsLetOrUseBang
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsMatch
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsMatchBang
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsMatchClause
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsMatchLambda
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsMember
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsModule
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsNamespace
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsNew
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsObjExpr
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsOpen
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsQuote
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsRecord
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsRecordDefn
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsRecordField
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsSpecialFunc
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsThenInIfThenElse
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsTryFinally
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsTryInTryFinally
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsTryInTryWith
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsTryWith
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsTuple
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsType
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsTypeExtension
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsUnionCase
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsUnionDefn
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsVal
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsWhile
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsWithInTryWith
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsXmlDocComment
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsYieldOrReturn
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean IsYieldOrReturnBang
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsArrayOrList()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsAttribute()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsComment()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsCompExpr()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsCompExprInternal()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsDo()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsElseInIfThenElse()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsEnumCase()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsFinallyInTryFinally()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsFor()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsHashDirective()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsIfThenElse()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsInterface()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsLambda()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsLetOrUse()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsLetOrUseBang()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsMatch()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsMatchBang()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsMatchClause()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsMatchLambda()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsMember()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsModule()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsNamespace()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsNew()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsObjExpr()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsOpen()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsQuote()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsRecord()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsRecordDefn()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsRecordField()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsSpecialFunc()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsThenInIfThenElse()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsTryFinally()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsTryInTryFinally()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsTryInTryWith()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsTryWith()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsTuple()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsType()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsTypeExtension()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsUnionCase()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsUnionDefn()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsVal()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsWhile()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsWithInTryWith()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsXmlDocComment()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsYieldOrReturn()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Boolean get_IsYieldOrReturnBang()
FSharp.Compiler.SourceCodeServices.Structure+Scope: FSharp.Compiler.SourceCodeServices.Structure+Scope+Tags
FSharp.Compiler.SourceCodeServices.Structure+Scope: Int32 CompareTo(Scope)
FSharp.Compiler.SourceCodeServices.Structure+Scope: Int32 CompareTo(System.Object)
FSharp.Compiler.SourceCodeServices.Structure+Scope: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.SourceCodeServices.Structure+Scope: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.Structure+Scope: Int32 Tag
FSharp.Compiler.SourceCodeServices.Structure+Scope: Int32 get_Tag()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope ArrayOrList
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope Attribute
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope Comment
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope CompExpr
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope CompExprInternal
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope Do
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope ElseInIfThenElse
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope EnumCase
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope FinallyInTryFinally
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope For
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope HashDirective
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope IfThenElse
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope Interface
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope Lambda
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope LetOrUse
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope LetOrUseBang
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope Match
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope MatchBang
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope MatchClause
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope MatchLambda
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope Member
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope Module
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope Namespace
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope New
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope ObjExpr
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope Open
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope Quote
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope Record
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope RecordDefn
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope RecordField
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope SpecialFunc
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope ThenInIfThenElse
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope TryFinally
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope TryInTryFinally
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope TryInTryWith
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope TryWith
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope Tuple
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope Type
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope TypeExtension
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope UnionCase
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope UnionDefn
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope Val
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope While
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope WithInTryWith
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope XmlDocComment
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope YieldOrReturn
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope YieldOrReturnBang
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_ArrayOrList()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_Attribute()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_Comment()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_CompExpr()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_CompExprInternal()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_Do()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_ElseInIfThenElse()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_EnumCase()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_FinallyInTryFinally()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_For()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_HashDirective()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_IfThenElse()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_Interface()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_Lambda()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_LetOrUse()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_LetOrUseBang()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_Match()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_MatchBang()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_MatchClause()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_MatchLambda()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_Member()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_Module()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_Namespace()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_New()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_ObjExpr()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_Open()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_Quote()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_Record()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_RecordDefn()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_RecordField()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_SpecialFunc()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_ThenInIfThenElse()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_TryFinally()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_TryInTryFinally()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_TryInTryWith()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_TryWith()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_Tuple()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_Type()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_TypeExtension()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_UnionCase()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_UnionDefn()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_Val()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_While()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_WithInTryWith()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_XmlDocComment()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_YieldOrReturn()
FSharp.Compiler.SourceCodeServices.Structure+Scope: Scope get_YieldOrReturnBang()
FSharp.Compiler.SourceCodeServices.Structure+Scope: System.String ToString()
FSharp.Compiler.SourceCodeServices.Structure+ScopeRange: Boolean Equals(ScopeRange)
FSharp.Compiler.SourceCodeServices.Structure+ScopeRange: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.Structure+ScopeRange: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.Structure+ScopeRange: Collapse Collapse
FSharp.Compiler.SourceCodeServices.Structure+ScopeRange: Collapse get_Collapse()
FSharp.Compiler.SourceCodeServices.Structure+ScopeRange: FSharp.Compiler.Text.Range CollapseRange
FSharp.Compiler.SourceCodeServices.Structure+ScopeRange: FSharp.Compiler.Text.Range Range
FSharp.Compiler.SourceCodeServices.Structure+ScopeRange: FSharp.Compiler.Text.Range get_CollapseRange()
FSharp.Compiler.SourceCodeServices.Structure+ScopeRange: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.SourceCodeServices.Structure+ScopeRange: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.Structure+ScopeRange: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.Structure+ScopeRange: Scope Scope
FSharp.Compiler.SourceCodeServices.Structure+ScopeRange: Scope get_Scope()
FSharp.Compiler.SourceCodeServices.Structure+ScopeRange: System.String ToString()
FSharp.Compiler.SourceCodeServices.Structure+ScopeRange: Void .ctor(Scope, Collapse, FSharp.Compiler.Text.Range, FSharp.Compiler.Text.Range)
FSharp.Compiler.SourceCodeServices.Structure: FSharp.Compiler.SourceCodeServices.Structure+Collapse
FSharp.Compiler.SourceCodeServices.Structure: FSharp.Compiler.SourceCodeServices.Structure+Scope
FSharp.Compiler.SourceCodeServices.Structure: FSharp.Compiler.SourceCodeServices.Structure+ScopeRange
FSharp.Compiler.SourceCodeServices.Structure: System.Collections.Generic.IEnumerable`1[FSharp.Compiler.SourceCodeServices.Structure+ScopeRange] getOutliningRanges(System.String[], ParsedInput)
FSharp.Compiler.SourceCodeServices.Symbol
FSharp.Compiler.SourceCodeServices.Symbol: Boolean hasAttribute[T](System.Collections.Generic.IEnumerable`1[FSharp.Compiler.SourceCodeServices.FSharpAttribute])
FSharp.Compiler.SourceCodeServices.Symbol: Boolean hasModuleSuffixAttribute(FSharp.Compiler.SourceCodeServices.FSharpEntity)
FSharp.Compiler.SourceCodeServices.Symbol: Boolean isAttribute[T](FSharp.Compiler.SourceCodeServices.FSharpAttribute)
FSharp.Compiler.SourceCodeServices.Symbol: Boolean isOperator(System.String)
FSharp.Compiler.SourceCodeServices.Symbol: Boolean isUnnamedUnionCaseField(FSharp.Compiler.SourceCodeServices.FSharpField)
FSharp.Compiler.SourceCodeServices.Symbol: FSharp.Compiler.SourceCodeServices.FSharpType getAbbreviatedType(FSharp.Compiler.SourceCodeServices.FSharpType)
FSharp.Compiler.SourceCodeServices.Symbol: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpActivePatternCase] |ActivePatternCase|_|(FSharp.Compiler.SourceCodeServices.FSharpSymbol)
FSharp.Compiler.SourceCodeServices.Symbol: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpAttribute] tryGetAttribute[T](System.Collections.Generic.IEnumerable`1[FSharp.Compiler.SourceCodeServices.FSharpAttribute])
FSharp.Compiler.SourceCodeServices.Symbol: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpEntity] |Constructor|_|(FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue)
FSharp.Compiler.SourceCodeServices.Symbol: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpEntity] |TypeWithDefinition|_|(FSharp.Compiler.SourceCodeServices.FSharpType)
FSharp.Compiler.SourceCodeServices.Symbol: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpField] |RecordField|_|(FSharp.Compiler.SourceCodeServices.FSharpSymbol)
FSharp.Compiler.SourceCodeServices.Symbol: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue] |MemberFunctionOrValue|_|(FSharp.Compiler.SourceCodeServices.FSharpSymbol)
FSharp.Compiler.SourceCodeServices.Symbol: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpType] |AbbreviatedType|_|(FSharp.Compiler.SourceCodeServices.FSharpEntity)
FSharp.Compiler.SourceCodeServices.Symbol: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpUnionCase] |UnionCase|_|(FSharp.Compiler.SourceCodeServices.FSharpSymbol)
FSharp.Compiler.SourceCodeServices.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |AbstractClass|_|(FSharp.Compiler.SourceCodeServices.FSharpEntity)
FSharp.Compiler.SourceCodeServices.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |Array|_|(FSharp.Compiler.SourceCodeServices.FSharpEntity)
FSharp.Compiler.SourceCodeServices.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |Attribute|_|(FSharp.Compiler.SourceCodeServices.FSharpEntity)
FSharp.Compiler.SourceCodeServices.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |ByRef|_|(FSharp.Compiler.SourceCodeServices.FSharpEntity)
FSharp.Compiler.SourceCodeServices.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |Class|_|[a](FSharp.Compiler.SourceCodeServices.FSharpEntity, FSharp.Compiler.SourceCodeServices.FSharpEntity, a)
FSharp.Compiler.SourceCodeServices.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |Delegate|_|(FSharp.Compiler.SourceCodeServices.FSharpEntity)
FSharp.Compiler.SourceCodeServices.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |Enum|_|(FSharp.Compiler.SourceCodeServices.FSharpEntity)
FSharp.Compiler.SourceCodeServices.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |Event|_|(FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue)
FSharp.Compiler.SourceCodeServices.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |ExtensionMember|_|(FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue)
FSharp.Compiler.SourceCodeServices.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |FSharpException|_|(FSharp.Compiler.SourceCodeServices.FSharpEntity)
FSharp.Compiler.SourceCodeServices.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |FSharpModule|_|(FSharp.Compiler.SourceCodeServices.FSharpEntity)
FSharp.Compiler.SourceCodeServices.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |FSharpType|_|(FSharp.Compiler.SourceCodeServices.FSharpEntity)
FSharp.Compiler.SourceCodeServices.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |FunctionType|_|(FSharp.Compiler.SourceCodeServices.FSharpType)
FSharp.Compiler.SourceCodeServices.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |Function|_|(Boolean, FSharp.Compiler.SourceCodeServices.FSharpMemberOrFunctionOrValue)
FSharp.Compiler.SourceCodeServices.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |Interface|_|(FSharp.Compiler.SourceCodeServices.FSharpEntity)
FSharp.Compiler.SourceCodeServices.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |MutableVar|_|(FSharp.Compiler.SourceCodeServices.FSharpSymbol)
FSharp.Compiler.SourceCodeServices.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |Namespace|_|(FSharp.Compiler.SourceCodeServices.FSharpEntity)
FSharp.Compiler.SourceCodeServices.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |Parameter|_|(FSharp.Compiler.SourceCodeServices.FSharpSymbol)
FSharp.Compiler.SourceCodeServices.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |Pattern|_|(FSharp.Compiler.SourceCodeServices.FSharpSymbol)
FSharp.Compiler.SourceCodeServices.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |ProvidedAndErasedType|_|(FSharp.Compiler.SourceCodeServices.FSharpEntity)
FSharp.Compiler.SourceCodeServices.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |ProvidedType|_|(FSharp.Compiler.SourceCodeServices.FSharpEntity)
FSharp.Compiler.SourceCodeServices.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |Record|_|(FSharp.Compiler.SourceCodeServices.FSharpEntity)
FSharp.Compiler.SourceCodeServices.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |RefCell|_|(FSharp.Compiler.SourceCodeServices.FSharpType)
FSharp.Compiler.SourceCodeServices.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |Tuple|_|(FSharp.Compiler.SourceCodeServices.FSharpType)
FSharp.Compiler.SourceCodeServices.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |UnionType|_|(FSharp.Compiler.SourceCodeServices.FSharpEntity)
FSharp.Compiler.SourceCodeServices.Symbol: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.Unit] |ValueType|_|(FSharp.Compiler.SourceCodeServices.FSharpEntity)
FSharp.Compiler.SourceCodeServices.Symbol: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.SourceCodeServices.FSharpField,FSharp.Compiler.SourceCodeServices.FSharpType]] |Field|_|(FSharp.Compiler.SourceCodeServices.FSharpSymbol)
FSharp.Compiler.SourceCodeServices.Symbol: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`3[FSharp.Compiler.SourceCodeServices.FSharpEntity,FSharp.Compiler.SourceCodeServices.FSharpEntity,Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpType]]] |FSharpEntity|_|(FSharp.Compiler.SourceCodeServices.FSharpSymbol)
FSharp.Compiler.SourceCodeServices.Symbol: System.Tuple`2[FSharp.Compiler.SourceCodeServices.FSharpEntity,Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.FSharpType]] getEntityAbbreviatedType(FSharp.Compiler.SourceCodeServices.FSharpEntity)
FSharp.Compiler.SourceCodeServices.UntypedParseImpl
FSharp.Compiler.SourceCodeServices.UntypedParseImpl: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.CompletionContext] TryGetCompletionContext(FSharp.Compiler.Text.Pos, ParsedInput, System.String)
FSharp.Compiler.SourceCodeServices.UntypedParseImpl: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SourceCodeServices.EntityKind] GetEntityKind(FSharp.Compiler.Text.Pos, ParsedInput)
FSharp.Compiler.SourceCodeServices.UntypedParseImpl: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] GetRangeOfExprLeftOfDot(FSharp.Compiler.Text.Pos, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+ParsedInput])
FSharp.Compiler.SourceCodeServices.UntypedParseImpl: Microsoft.FSharp.Core.FSharpOption`1[System.String] TryFindExpressionIslandInPosition(FSharp.Compiler.Text.Pos, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+ParsedInput])
FSharp.Compiler.SourceCodeServices.UntypedParseImpl: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.Text.Pos,System.Boolean]] TryFindExpressionASTLeftOfDotLeftOfCursor(FSharp.Compiler.Text.Pos, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+ParsedInput])
FSharp.Compiler.SourceCodeServices.UntypedParseImpl: System.String[] GetFullNameOfSmallestModuleOrNamespaceAtPoint(ParsedInput, FSharp.Compiler.Text.Pos)
FSharp.Compiler.SourceCodeServices.UnusedDeclarations
FSharp.Compiler.SourceCodeServices.UnusedDeclarations: Microsoft.FSharp.Control.FSharpAsync`1[System.Collections.Generic.IEnumerable`1[FSharp.Compiler.Text.Range]] getUnusedDeclarations(FSharp.Compiler.SourceCodeServices.FSharpCheckFileResults, Boolean)
FSharp.Compiler.SourceCodeServices.UnusedOpens
FSharp.Compiler.SourceCodeServices.UnusedOpens: Microsoft.FSharp.Control.FSharpAsync`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Text.Range]] getUnusedOpens(FSharp.Compiler.SourceCodeServices.FSharpCheckFileResults, Microsoft.FSharp.Core.FSharpFunc`2[System.Int32,System.String])
FSharp.Compiler.SourceCodeServices.XmlDocComment
FSharp.Compiler.SourceCodeServices.XmlDocComment: Microsoft.FSharp.Core.FSharpOption`1[System.Int32] isBlank(System.String)
FSharp.Compiler.SourceCodeServices.XmlDocParser
FSharp.Compiler.SourceCodeServices.XmlDocParser: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SourceCodeServices.XmlDocable] getXmlDocables(FSharp.Compiler.Text.ISourceText, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+ParsedInput])
FSharp.Compiler.SourceCodeServices.XmlDocable
FSharp.Compiler.SourceCodeServices.XmlDocable: Boolean Equals(FSharp.Compiler.SourceCodeServices.XmlDocable)
FSharp.Compiler.SourceCodeServices.XmlDocable: Boolean Equals(System.Object)
FSharp.Compiler.SourceCodeServices.XmlDocable: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.XmlDocable: FSharp.Compiler.SourceCodeServices.XmlDocable NewXmlDocable(Int32, Int32, Microsoft.FSharp.Collections.FSharpList`1[System.String])
FSharp.Compiler.SourceCodeServices.XmlDocable: Int32 CompareTo(FSharp.Compiler.SourceCodeServices.XmlDocable)
FSharp.Compiler.SourceCodeServices.XmlDocable: Int32 CompareTo(System.Object)
FSharp.Compiler.SourceCodeServices.XmlDocable: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.SourceCodeServices.XmlDocable: Int32 GetHashCode()
FSharp.Compiler.SourceCodeServices.XmlDocable: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SourceCodeServices.XmlDocable: Int32 Tag
FSharp.Compiler.SourceCodeServices.XmlDocable: Int32 get_Tag()
FSharp.Compiler.SourceCodeServices.XmlDocable: Int32 get_indent()
FSharp.Compiler.SourceCodeServices.XmlDocable: Int32 get_line()
FSharp.Compiler.SourceCodeServices.XmlDocable: Int32 indent
FSharp.Compiler.SourceCodeServices.XmlDocable: Int32 line
FSharp.Compiler.SourceCodeServices.XmlDocable: Microsoft.FSharp.Collections.FSharpList`1[System.String] get_paramNames()
FSharp.Compiler.SourceCodeServices.XmlDocable: Microsoft.FSharp.Collections.FSharpList`1[System.String] paramNames
FSharp.Compiler.SourceCodeServices.XmlDocable: System.String ToString()
FSharp.Compiler.SyntaxTree
FSharp.Compiler.SyntaxTree+DebugPointAtFinally+Tags: Int32 No
FSharp.Compiler.SyntaxTree+DebugPointAtFinally+Tags: Int32 Yes
FSharp.Compiler.SyntaxTree+DebugPointAtFinally+Yes: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+DebugPointAtFinally+Yes: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+DebugPointAtFinally: Boolean Equals(DebugPointAtFinally)
FSharp.Compiler.SyntaxTree+DebugPointAtFinally: Boolean Equals(System.Object)
FSharp.Compiler.SyntaxTree+DebugPointAtFinally: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SyntaxTree+DebugPointAtFinally: Boolean IsNo
FSharp.Compiler.SyntaxTree+DebugPointAtFinally: Boolean IsYes
FSharp.Compiler.SyntaxTree+DebugPointAtFinally: Boolean get_IsNo()
FSharp.Compiler.SyntaxTree+DebugPointAtFinally: Boolean get_IsYes()
FSharp.Compiler.SyntaxTree+DebugPointAtFinally: DebugPointAtFinally NewYes(FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+DebugPointAtFinally: DebugPointAtFinally No
FSharp.Compiler.SyntaxTree+DebugPointAtFinally: DebugPointAtFinally get_No()
FSharp.Compiler.SyntaxTree+DebugPointAtFinally: FSharp.Compiler.SyntaxTree+DebugPointAtFinally+Tags
FSharp.Compiler.SyntaxTree+DebugPointAtFinally: FSharp.Compiler.SyntaxTree+DebugPointAtFinally+Yes
FSharp.Compiler.SyntaxTree+DebugPointAtFinally: Int32 GetHashCode()
FSharp.Compiler.SyntaxTree+DebugPointAtFinally: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SyntaxTree+DebugPointAtFinally: Int32 Tag
FSharp.Compiler.SyntaxTree+DebugPointAtFinally: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+DebugPointAtFinally: System.String ToString()
FSharp.Compiler.SyntaxTree+DebugPointAtFor+Tags: Int32 No
FSharp.Compiler.SyntaxTree+DebugPointAtFor+Tags: Int32 Yes
FSharp.Compiler.SyntaxTree+DebugPointAtFor+Yes: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+DebugPointAtFor+Yes: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+DebugPointAtFor: Boolean Equals(DebugPointAtFor)
FSharp.Compiler.SyntaxTree+DebugPointAtFor: Boolean Equals(System.Object)
FSharp.Compiler.SyntaxTree+DebugPointAtFor: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SyntaxTree+DebugPointAtFor: Boolean IsNo
FSharp.Compiler.SyntaxTree+DebugPointAtFor: Boolean IsYes
FSharp.Compiler.SyntaxTree+DebugPointAtFor: Boolean get_IsNo()
FSharp.Compiler.SyntaxTree+DebugPointAtFor: Boolean get_IsYes()
FSharp.Compiler.SyntaxTree+DebugPointAtFor: DebugPointAtFor NewYes(FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+DebugPointAtFor: DebugPointAtFor No
FSharp.Compiler.SyntaxTree+DebugPointAtFor: DebugPointAtFor get_No()
FSharp.Compiler.SyntaxTree+DebugPointAtFor: FSharp.Compiler.SyntaxTree+DebugPointAtFor+Tags
FSharp.Compiler.SyntaxTree+DebugPointAtFor: FSharp.Compiler.SyntaxTree+DebugPointAtFor+Yes
FSharp.Compiler.SyntaxTree+DebugPointAtFor: Int32 GetHashCode()
FSharp.Compiler.SyntaxTree+DebugPointAtFor: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SyntaxTree+DebugPointAtFor: Int32 Tag
FSharp.Compiler.SyntaxTree+DebugPointAtFor: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+DebugPointAtFor: System.String ToString()
FSharp.Compiler.SyntaxTree+DebugPointAtSequential+Tags: Int32 Both
FSharp.Compiler.SyntaxTree+DebugPointAtSequential+Tags: Int32 ExprOnly
FSharp.Compiler.SyntaxTree+DebugPointAtSequential+Tags: Int32 StmtOnly
FSharp.Compiler.SyntaxTree+DebugPointAtSequential: Boolean Equals(DebugPointAtSequential)
FSharp.Compiler.SyntaxTree+DebugPointAtSequential: Boolean Equals(System.Object)
FSharp.Compiler.SyntaxTree+DebugPointAtSequential: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SyntaxTree+DebugPointAtSequential: Boolean IsBoth
FSharp.Compiler.SyntaxTree+DebugPointAtSequential: Boolean IsExprOnly
FSharp.Compiler.SyntaxTree+DebugPointAtSequential: Boolean IsStmtOnly
FSharp.Compiler.SyntaxTree+DebugPointAtSequential: Boolean get_IsBoth()
FSharp.Compiler.SyntaxTree+DebugPointAtSequential: Boolean get_IsExprOnly()
FSharp.Compiler.SyntaxTree+DebugPointAtSequential: Boolean get_IsStmtOnly()
FSharp.Compiler.SyntaxTree+DebugPointAtSequential: DebugPointAtSequential Both
FSharp.Compiler.SyntaxTree+DebugPointAtSequential: DebugPointAtSequential ExprOnly
FSharp.Compiler.SyntaxTree+DebugPointAtSequential: DebugPointAtSequential StmtOnly
FSharp.Compiler.SyntaxTree+DebugPointAtSequential: DebugPointAtSequential get_Both()
FSharp.Compiler.SyntaxTree+DebugPointAtSequential: DebugPointAtSequential get_ExprOnly()
FSharp.Compiler.SyntaxTree+DebugPointAtSequential: DebugPointAtSequential get_StmtOnly()
FSharp.Compiler.SyntaxTree+DebugPointAtSequential: FSharp.Compiler.SyntaxTree+DebugPointAtSequential+Tags
FSharp.Compiler.SyntaxTree+DebugPointAtSequential: Int32 CompareTo(DebugPointAtSequential)
FSharp.Compiler.SyntaxTree+DebugPointAtSequential: Int32 CompareTo(System.Object)
FSharp.Compiler.SyntaxTree+DebugPointAtSequential: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.SyntaxTree+DebugPointAtSequential: Int32 GetHashCode()
FSharp.Compiler.SyntaxTree+DebugPointAtSequential: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SyntaxTree+DebugPointAtSequential: Int32 Tag
FSharp.Compiler.SyntaxTree+DebugPointAtSequential: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+DebugPointAtSequential: System.String ToString()
FSharp.Compiler.SyntaxTree+DebugPointAtTry+Tags: Int32 Body
FSharp.Compiler.SyntaxTree+DebugPointAtTry+Tags: Int32 No
FSharp.Compiler.SyntaxTree+DebugPointAtTry+Tags: Int32 Yes
FSharp.Compiler.SyntaxTree+DebugPointAtTry+Yes: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+DebugPointAtTry+Yes: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+DebugPointAtTry: Boolean Equals(DebugPointAtTry)
FSharp.Compiler.SyntaxTree+DebugPointAtTry: Boolean Equals(System.Object)
FSharp.Compiler.SyntaxTree+DebugPointAtTry: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SyntaxTree+DebugPointAtTry: Boolean IsBody
FSharp.Compiler.SyntaxTree+DebugPointAtTry: Boolean IsNo
FSharp.Compiler.SyntaxTree+DebugPointAtTry: Boolean IsYes
FSharp.Compiler.SyntaxTree+DebugPointAtTry: Boolean get_IsBody()
FSharp.Compiler.SyntaxTree+DebugPointAtTry: Boolean get_IsNo()
FSharp.Compiler.SyntaxTree+DebugPointAtTry: Boolean get_IsYes()
FSharp.Compiler.SyntaxTree+DebugPointAtTry: DebugPointAtTry Body
FSharp.Compiler.SyntaxTree+DebugPointAtTry: DebugPointAtTry NewYes(FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+DebugPointAtTry: DebugPointAtTry No
FSharp.Compiler.SyntaxTree+DebugPointAtTry: DebugPointAtTry get_Body()
FSharp.Compiler.SyntaxTree+DebugPointAtTry: DebugPointAtTry get_No()
FSharp.Compiler.SyntaxTree+DebugPointAtTry: FSharp.Compiler.SyntaxTree+DebugPointAtTry+Tags
FSharp.Compiler.SyntaxTree+DebugPointAtTry: FSharp.Compiler.SyntaxTree+DebugPointAtTry+Yes
FSharp.Compiler.SyntaxTree+DebugPointAtTry: Int32 GetHashCode()
FSharp.Compiler.SyntaxTree+DebugPointAtTry: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SyntaxTree+DebugPointAtTry: Int32 Tag
FSharp.Compiler.SyntaxTree+DebugPointAtTry: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+DebugPointAtTry: System.String ToString()
FSharp.Compiler.SyntaxTree+DebugPointAtWhile+Tags: Int32 No
FSharp.Compiler.SyntaxTree+DebugPointAtWhile+Tags: Int32 Yes
FSharp.Compiler.SyntaxTree+DebugPointAtWhile+Yes: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+DebugPointAtWhile+Yes: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+DebugPointAtWhile: Boolean Equals(DebugPointAtWhile)
FSharp.Compiler.SyntaxTree+DebugPointAtWhile: Boolean Equals(System.Object)
FSharp.Compiler.SyntaxTree+DebugPointAtWhile: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SyntaxTree+DebugPointAtWhile: Boolean IsNo
FSharp.Compiler.SyntaxTree+DebugPointAtWhile: Boolean IsYes
FSharp.Compiler.SyntaxTree+DebugPointAtWhile: Boolean get_IsNo()
FSharp.Compiler.SyntaxTree+DebugPointAtWhile: Boolean get_IsYes()
FSharp.Compiler.SyntaxTree+DebugPointAtWhile: DebugPointAtWhile NewYes(FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+DebugPointAtWhile: DebugPointAtWhile No
FSharp.Compiler.SyntaxTree+DebugPointAtWhile: DebugPointAtWhile get_No()
FSharp.Compiler.SyntaxTree+DebugPointAtWhile: FSharp.Compiler.SyntaxTree+DebugPointAtWhile+Tags
FSharp.Compiler.SyntaxTree+DebugPointAtWhile: FSharp.Compiler.SyntaxTree+DebugPointAtWhile+Yes
FSharp.Compiler.SyntaxTree+DebugPointAtWhile: Int32 GetHashCode()
FSharp.Compiler.SyntaxTree+DebugPointAtWhile: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SyntaxTree+DebugPointAtWhile: Int32 Tag
FSharp.Compiler.SyntaxTree+DebugPointAtWhile: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+DebugPointAtWhile: System.String ToString()
FSharp.Compiler.SyntaxTree+DebugPointAtWith+Tags: Int32 No
FSharp.Compiler.SyntaxTree+DebugPointAtWith+Tags: Int32 Yes
FSharp.Compiler.SyntaxTree+DebugPointAtWith+Yes: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+DebugPointAtWith+Yes: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+DebugPointAtWith: Boolean Equals(DebugPointAtWith)
FSharp.Compiler.SyntaxTree+DebugPointAtWith: Boolean Equals(System.Object)
FSharp.Compiler.SyntaxTree+DebugPointAtWith: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SyntaxTree+DebugPointAtWith: Boolean IsNo
FSharp.Compiler.SyntaxTree+DebugPointAtWith: Boolean IsYes
FSharp.Compiler.SyntaxTree+DebugPointAtWith: Boolean get_IsNo()
FSharp.Compiler.SyntaxTree+DebugPointAtWith: Boolean get_IsYes()
FSharp.Compiler.SyntaxTree+DebugPointAtWith: DebugPointAtWith NewYes(FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+DebugPointAtWith: DebugPointAtWith No
FSharp.Compiler.SyntaxTree+DebugPointAtWith: DebugPointAtWith get_No()
FSharp.Compiler.SyntaxTree+DebugPointAtWith: FSharp.Compiler.SyntaxTree+DebugPointAtWith+Tags
FSharp.Compiler.SyntaxTree+DebugPointAtWith: FSharp.Compiler.SyntaxTree+DebugPointAtWith+Yes
FSharp.Compiler.SyntaxTree+DebugPointAtWith: Int32 GetHashCode()
FSharp.Compiler.SyntaxTree+DebugPointAtWith: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SyntaxTree+DebugPointAtWith: Int32 Tag
FSharp.Compiler.SyntaxTree+DebugPointAtWith: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+DebugPointAtWith: System.String ToString()
FSharp.Compiler.SyntaxTree+DebugPointForBinding+DebugPointAtBinding: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+DebugPointForBinding+DebugPointAtBinding: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+DebugPointForBinding+Tags: Int32 DebugPointAtBinding
FSharp.Compiler.SyntaxTree+DebugPointForBinding+Tags: Int32 NoDebugPointAtDoBinding
FSharp.Compiler.SyntaxTree+DebugPointForBinding+Tags: Int32 NoDebugPointAtInvisibleBinding
FSharp.Compiler.SyntaxTree+DebugPointForBinding+Tags: Int32 NoDebugPointAtLetBinding
FSharp.Compiler.SyntaxTree+DebugPointForBinding+Tags: Int32 NoDebugPointAtStickyBinding
FSharp.Compiler.SyntaxTree+DebugPointForBinding: Boolean Equals(DebugPointForBinding)
FSharp.Compiler.SyntaxTree+DebugPointForBinding: Boolean Equals(System.Object)
FSharp.Compiler.SyntaxTree+DebugPointForBinding: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SyntaxTree+DebugPointForBinding: Boolean IsDebugPointAtBinding
FSharp.Compiler.SyntaxTree+DebugPointForBinding: Boolean IsNoDebugPointAtDoBinding
FSharp.Compiler.SyntaxTree+DebugPointForBinding: Boolean IsNoDebugPointAtInvisibleBinding
FSharp.Compiler.SyntaxTree+DebugPointForBinding: Boolean IsNoDebugPointAtLetBinding
FSharp.Compiler.SyntaxTree+DebugPointForBinding: Boolean IsNoDebugPointAtStickyBinding
FSharp.Compiler.SyntaxTree+DebugPointForBinding: Boolean get_IsDebugPointAtBinding()
FSharp.Compiler.SyntaxTree+DebugPointForBinding: Boolean get_IsNoDebugPointAtDoBinding()
FSharp.Compiler.SyntaxTree+DebugPointForBinding: Boolean get_IsNoDebugPointAtInvisibleBinding()
FSharp.Compiler.SyntaxTree+DebugPointForBinding: Boolean get_IsNoDebugPointAtLetBinding()
FSharp.Compiler.SyntaxTree+DebugPointForBinding: Boolean get_IsNoDebugPointAtStickyBinding()
FSharp.Compiler.SyntaxTree+DebugPointForBinding: DebugPointForBinding Combine(DebugPointForBinding)
FSharp.Compiler.SyntaxTree+DebugPointForBinding: DebugPointForBinding NewDebugPointAtBinding(FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+DebugPointForBinding: DebugPointForBinding NoDebugPointAtDoBinding
FSharp.Compiler.SyntaxTree+DebugPointForBinding: DebugPointForBinding NoDebugPointAtInvisibleBinding
FSharp.Compiler.SyntaxTree+DebugPointForBinding: DebugPointForBinding NoDebugPointAtLetBinding
FSharp.Compiler.SyntaxTree+DebugPointForBinding: DebugPointForBinding NoDebugPointAtStickyBinding
FSharp.Compiler.SyntaxTree+DebugPointForBinding: DebugPointForBinding get_NoDebugPointAtDoBinding()
FSharp.Compiler.SyntaxTree+DebugPointForBinding: DebugPointForBinding get_NoDebugPointAtInvisibleBinding()
FSharp.Compiler.SyntaxTree+DebugPointForBinding: DebugPointForBinding get_NoDebugPointAtLetBinding()
FSharp.Compiler.SyntaxTree+DebugPointForBinding: DebugPointForBinding get_NoDebugPointAtStickyBinding()
FSharp.Compiler.SyntaxTree+DebugPointForBinding: FSharp.Compiler.SyntaxTree+DebugPointForBinding+DebugPointAtBinding
FSharp.Compiler.SyntaxTree+DebugPointForBinding: FSharp.Compiler.SyntaxTree+DebugPointForBinding+Tags
FSharp.Compiler.SyntaxTree+DebugPointForBinding: Int32 GetHashCode()
FSharp.Compiler.SyntaxTree+DebugPointForBinding: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SyntaxTree+DebugPointForBinding: Int32 Tag
FSharp.Compiler.SyntaxTree+DebugPointForBinding: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+DebugPointForBinding: System.String ToString()
FSharp.Compiler.SyntaxTree+DebugPointForTarget+Tags: Int32 No
FSharp.Compiler.SyntaxTree+DebugPointForTarget+Tags: Int32 Yes
FSharp.Compiler.SyntaxTree+DebugPointForTarget: Boolean Equals(DebugPointForTarget)
FSharp.Compiler.SyntaxTree+DebugPointForTarget: Boolean Equals(System.Object)
FSharp.Compiler.SyntaxTree+DebugPointForTarget: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SyntaxTree+DebugPointForTarget: Boolean IsNo
FSharp.Compiler.SyntaxTree+DebugPointForTarget: Boolean IsYes
FSharp.Compiler.SyntaxTree+DebugPointForTarget: Boolean get_IsNo()
FSharp.Compiler.SyntaxTree+DebugPointForTarget: Boolean get_IsYes()
FSharp.Compiler.SyntaxTree+DebugPointForTarget: DebugPointForTarget No
FSharp.Compiler.SyntaxTree+DebugPointForTarget: DebugPointForTarget Yes
FSharp.Compiler.SyntaxTree+DebugPointForTarget: DebugPointForTarget get_No()
FSharp.Compiler.SyntaxTree+DebugPointForTarget: DebugPointForTarget get_Yes()
FSharp.Compiler.SyntaxTree+DebugPointForTarget: FSharp.Compiler.SyntaxTree+DebugPointForTarget+Tags
FSharp.Compiler.SyntaxTree+DebugPointForTarget: Int32 CompareTo(DebugPointForTarget)
FSharp.Compiler.SyntaxTree+DebugPointForTarget: Int32 CompareTo(System.Object)
FSharp.Compiler.SyntaxTree+DebugPointForTarget: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.SyntaxTree+DebugPointForTarget: Int32 GetHashCode()
FSharp.Compiler.SyntaxTree+DebugPointForTarget: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SyntaxTree+DebugPointForTarget: Int32 Tag
FSharp.Compiler.SyntaxTree+DebugPointForTarget: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+DebugPointForTarget: System.String ToString()
FSharp.Compiler.SyntaxTree+ExprAtomicFlag: ExprAtomicFlag Atomic
FSharp.Compiler.SyntaxTree+ExprAtomicFlag: ExprAtomicFlag NonAtomic
FSharp.Compiler.SyntaxTree+ExprAtomicFlag: Int32 value__
FSharp.Compiler.SyntaxTree+Ident: FSharp.Compiler.Text.Range get_idRange()
FSharp.Compiler.SyntaxTree+Ident: FSharp.Compiler.Text.Range idRange
FSharp.Compiler.SyntaxTree+Ident: System.String ToString()
FSharp.Compiler.SyntaxTree+Ident: System.String get_idText()
FSharp.Compiler.SyntaxTree+Ident: System.String idText
FSharp.Compiler.SyntaxTree+Ident: Void .ctor(System.String, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+LongIdentWithDots: Boolean ThereIsAnExtraDotAtTheEnd
FSharp.Compiler.SyntaxTree+LongIdentWithDots: Boolean get_ThereIsAnExtraDotAtTheEnd()
FSharp.Compiler.SyntaxTree+LongIdentWithDots: FSharp.Compiler.Text.Range Range
FSharp.Compiler.SyntaxTree+LongIdentWithDots: FSharp.Compiler.Text.Range RangeSansAnyExtraDot
FSharp.Compiler.SyntaxTree+LongIdentWithDots: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.SyntaxTree+LongIdentWithDots: FSharp.Compiler.Text.Range get_RangeSansAnyExtraDot()
FSharp.Compiler.SyntaxTree+LongIdentWithDots: Int32 Tag
FSharp.Compiler.SyntaxTree+LongIdentWithDots: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+LongIdentWithDots: LongIdentWithDots NewLongIdentWithDots(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Text.Range])
FSharp.Compiler.SyntaxTree+LongIdentWithDots: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident] Lid
FSharp.Compiler.SyntaxTree+LongIdentWithDots: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident] get_Lid()
FSharp.Compiler.SyntaxTree+LongIdentWithDots: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident] get_id()
FSharp.Compiler.SyntaxTree+LongIdentWithDots: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident] id
FSharp.Compiler.SyntaxTree+LongIdentWithDots: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Text.Range] dotms
FSharp.Compiler.SyntaxTree+LongIdentWithDots: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Text.Range] get_dotms()
FSharp.Compiler.SyntaxTree+LongIdentWithDots: System.String ToString()
FSharp.Compiler.SyntaxTree+MemberFlags: Boolean Equals(MemberFlags)
FSharp.Compiler.SyntaxTree+MemberFlags: Boolean Equals(System.Object)
FSharp.Compiler.SyntaxTree+MemberFlags: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SyntaxTree+MemberFlags: Boolean IsDispatchSlot
FSharp.Compiler.SyntaxTree+MemberFlags: Boolean IsFinal
FSharp.Compiler.SyntaxTree+MemberFlags: Boolean IsInstance
FSharp.Compiler.SyntaxTree+MemberFlags: Boolean IsOverrideOrExplicitImpl
FSharp.Compiler.SyntaxTree+MemberFlags: Boolean get_IsDispatchSlot()
FSharp.Compiler.SyntaxTree+MemberFlags: Boolean get_IsFinal()
FSharp.Compiler.SyntaxTree+MemberFlags: Boolean get_IsInstance()
FSharp.Compiler.SyntaxTree+MemberFlags: Boolean get_IsOverrideOrExplicitImpl()
FSharp.Compiler.SyntaxTree+MemberFlags: Int32 GetHashCode()
FSharp.Compiler.SyntaxTree+MemberFlags: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SyntaxTree+MemberFlags: MemberKind MemberKind
FSharp.Compiler.SyntaxTree+MemberFlags: MemberKind get_MemberKind()
FSharp.Compiler.SyntaxTree+MemberFlags: System.String ToString()
FSharp.Compiler.SyntaxTree+MemberFlags: Void .ctor(Boolean, Boolean, Boolean, Boolean, MemberKind)
FSharp.Compiler.SyntaxTree+MemberKind+Tags: Int32 ClassConstructor
FSharp.Compiler.SyntaxTree+MemberKind+Tags: Int32 Constructor
FSharp.Compiler.SyntaxTree+MemberKind+Tags: Int32 Member
FSharp.Compiler.SyntaxTree+MemberKind+Tags: Int32 PropertyGet
FSharp.Compiler.SyntaxTree+MemberKind+Tags: Int32 PropertyGetSet
FSharp.Compiler.SyntaxTree+MemberKind+Tags: Int32 PropertySet
FSharp.Compiler.SyntaxTree+MemberKind: Boolean Equals(MemberKind)
FSharp.Compiler.SyntaxTree+MemberKind: Boolean Equals(System.Object)
FSharp.Compiler.SyntaxTree+MemberKind: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SyntaxTree+MemberKind: Boolean IsClassConstructor
FSharp.Compiler.SyntaxTree+MemberKind: Boolean IsConstructor
FSharp.Compiler.SyntaxTree+MemberKind: Boolean IsMember
FSharp.Compiler.SyntaxTree+MemberKind: Boolean IsPropertyGet
FSharp.Compiler.SyntaxTree+MemberKind: Boolean IsPropertyGetSet
FSharp.Compiler.SyntaxTree+MemberKind: Boolean IsPropertySet
FSharp.Compiler.SyntaxTree+MemberKind: Boolean get_IsClassConstructor()
FSharp.Compiler.SyntaxTree+MemberKind: Boolean get_IsConstructor()
FSharp.Compiler.SyntaxTree+MemberKind: Boolean get_IsMember()
FSharp.Compiler.SyntaxTree+MemberKind: Boolean get_IsPropertyGet()
FSharp.Compiler.SyntaxTree+MemberKind: Boolean get_IsPropertyGetSet()
FSharp.Compiler.SyntaxTree+MemberKind: Boolean get_IsPropertySet()
FSharp.Compiler.SyntaxTree+MemberKind: FSharp.Compiler.SyntaxTree+MemberKind+Tags
FSharp.Compiler.SyntaxTree+MemberKind: Int32 GetHashCode()
FSharp.Compiler.SyntaxTree+MemberKind: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SyntaxTree+MemberKind: Int32 Tag
FSharp.Compiler.SyntaxTree+MemberKind: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+MemberKind: MemberKind ClassConstructor
FSharp.Compiler.SyntaxTree+MemberKind: MemberKind Constructor
FSharp.Compiler.SyntaxTree+MemberKind: MemberKind Member
FSharp.Compiler.SyntaxTree+MemberKind: MemberKind PropertyGet
FSharp.Compiler.SyntaxTree+MemberKind: MemberKind PropertyGetSet
FSharp.Compiler.SyntaxTree+MemberKind: MemberKind PropertySet
FSharp.Compiler.SyntaxTree+MemberKind: MemberKind get_ClassConstructor()
FSharp.Compiler.SyntaxTree+MemberKind: MemberKind get_Constructor()
FSharp.Compiler.SyntaxTree+MemberKind: MemberKind get_Member()
FSharp.Compiler.SyntaxTree+MemberKind: MemberKind get_PropertyGet()
FSharp.Compiler.SyntaxTree+MemberKind: MemberKind get_PropertyGetSet()
FSharp.Compiler.SyntaxTree+MemberKind: MemberKind get_PropertySet()
FSharp.Compiler.SyntaxTree+MemberKind: System.String ToString()
FSharp.Compiler.SyntaxTree+ParsedFsiInteraction+IDefns: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+ParsedFsiInteraction+IDefns: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+ParsedFsiInteraction+IDefns: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynModuleDecl] defns
FSharp.Compiler.SyntaxTree+ParsedFsiInteraction+IDefns: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynModuleDecl] get_defns()
FSharp.Compiler.SyntaxTree+ParsedFsiInteraction+IHash: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+ParsedFsiInteraction+IHash: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+ParsedFsiInteraction+IHash: ParsedHashDirective get_hashDirective()
FSharp.Compiler.SyntaxTree+ParsedFsiInteraction+IHash: ParsedHashDirective hashDirective
FSharp.Compiler.SyntaxTree+ParsedFsiInteraction+Tags: Int32 IDefns
FSharp.Compiler.SyntaxTree+ParsedFsiInteraction+Tags: Int32 IHash
FSharp.Compiler.SyntaxTree+ParsedFsiInteraction: Boolean IsIDefns
FSharp.Compiler.SyntaxTree+ParsedFsiInteraction: Boolean IsIHash
FSharp.Compiler.SyntaxTree+ParsedFsiInteraction: Boolean get_IsIDefns()
FSharp.Compiler.SyntaxTree+ParsedFsiInteraction: Boolean get_IsIHash()
FSharp.Compiler.SyntaxTree+ParsedFsiInteraction: FSharp.Compiler.SyntaxTree+ParsedFsiInteraction+IDefns
FSharp.Compiler.SyntaxTree+ParsedFsiInteraction: FSharp.Compiler.SyntaxTree+ParsedFsiInteraction+IHash
FSharp.Compiler.SyntaxTree+ParsedFsiInteraction: FSharp.Compiler.SyntaxTree+ParsedFsiInteraction+Tags
FSharp.Compiler.SyntaxTree+ParsedFsiInteraction: Int32 Tag
FSharp.Compiler.SyntaxTree+ParsedFsiInteraction: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+ParsedFsiInteraction: ParsedFsiInteraction NewIDefns(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynModuleDecl], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+ParsedFsiInteraction: ParsedFsiInteraction NewIHash(ParsedHashDirective, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+ParsedFsiInteraction: System.String ToString()
FSharp.Compiler.SyntaxTree+ParsedHashDirective: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+ParsedHashDirective: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+ParsedHashDirective: Int32 Tag
FSharp.Compiler.SyntaxTree+ParsedHashDirective: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+ParsedHashDirective: Microsoft.FSharp.Collections.FSharpList`1[System.String] args
FSharp.Compiler.SyntaxTree+ParsedHashDirective: Microsoft.FSharp.Collections.FSharpList`1[System.String] get_args()
FSharp.Compiler.SyntaxTree+ParsedHashDirective: ParsedHashDirective NewParsedHashDirective(System.String, Microsoft.FSharp.Collections.FSharpList`1[System.String], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+ParsedHashDirective: System.String ToString()
FSharp.Compiler.SyntaxTree+ParsedHashDirective: System.String get_ident()
FSharp.Compiler.SyntaxTree+ParsedHashDirective: System.String ident
FSharp.Compiler.SyntaxTree+ParsedImplFile: Int32 Tag
FSharp.Compiler.SyntaxTree+ParsedImplFile: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+ParsedImplFile: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+ParsedHashDirective] get_hashDirectives()
FSharp.Compiler.SyntaxTree+ParsedImplFile: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+ParsedHashDirective] hashDirectives
FSharp.Compiler.SyntaxTree+ParsedImplFile: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+ParsedImplFileFragment] fragments
FSharp.Compiler.SyntaxTree+ParsedImplFile: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+ParsedImplFileFragment] get_fragments()
FSharp.Compiler.SyntaxTree+ParsedImplFile: ParsedImplFile NewParsedImplFile(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+ParsedHashDirective], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+ParsedImplFileFragment])
FSharp.Compiler.SyntaxTree+ParsedImplFile: System.String ToString()
FSharp.Compiler.SyntaxTree+ParsedImplFileFragment+AnonModule: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+ParsedImplFileFragment+AnonModule: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+ParsedImplFileFragment+AnonModule: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynModuleDecl] decls
FSharp.Compiler.SyntaxTree+ParsedImplFileFragment+AnonModule: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynModuleDecl] get_decls()
FSharp.Compiler.SyntaxTree+ParsedImplFileFragment+NamedModule: SynModuleOrNamespace get_namedModule()
FSharp.Compiler.SyntaxTree+ParsedImplFileFragment+NamedModule: SynModuleOrNamespace namedModule
FSharp.Compiler.SyntaxTree+ParsedImplFileFragment+NamespaceFragment: Boolean get_isRecursive()
FSharp.Compiler.SyntaxTree+ParsedImplFileFragment+NamespaceFragment: Boolean isRecursive
FSharp.Compiler.SyntaxTree+ParsedImplFileFragment+NamespaceFragment: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+ParsedImplFileFragment+NamespaceFragment: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+ParsedImplFileFragment+NamespaceFragment: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident] get_longId()
FSharp.Compiler.SyntaxTree+ParsedImplFileFragment+NamespaceFragment: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident] longId
FSharp.Compiler.SyntaxTree+ParsedImplFileFragment+NamespaceFragment: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList] attributes
FSharp.Compiler.SyntaxTree+ParsedImplFileFragment+NamespaceFragment: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList] get_attributes()
FSharp.Compiler.SyntaxTree+ParsedImplFileFragment+NamespaceFragment: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynModuleDecl] decls
FSharp.Compiler.SyntaxTree+ParsedImplFileFragment+NamespaceFragment: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynModuleDecl] get_decls()
FSharp.Compiler.SyntaxTree+ParsedImplFileFragment+NamespaceFragment: PreXmlDoc get_xmlDoc()
FSharp.Compiler.SyntaxTree+ParsedImplFileFragment+NamespaceFragment: PreXmlDoc xmlDoc
FSharp.Compiler.SyntaxTree+ParsedImplFileFragment+NamespaceFragment: SynModuleOrNamespaceKind get_kind()
FSharp.Compiler.SyntaxTree+ParsedImplFileFragment+NamespaceFragment: SynModuleOrNamespaceKind kind
FSharp.Compiler.SyntaxTree+ParsedImplFileFragment+Tags: Int32 AnonModule
FSharp.Compiler.SyntaxTree+ParsedImplFileFragment+Tags: Int32 NamedModule
FSharp.Compiler.SyntaxTree+ParsedImplFileFragment+Tags: Int32 NamespaceFragment
FSharp.Compiler.SyntaxTree+ParsedImplFileFragment: Boolean IsAnonModule
FSharp.Compiler.SyntaxTree+ParsedImplFileFragment: Boolean IsNamedModule
FSharp.Compiler.SyntaxTree+ParsedImplFileFragment: Boolean IsNamespaceFragment
FSharp.Compiler.SyntaxTree+ParsedImplFileFragment: Boolean get_IsAnonModule()
FSharp.Compiler.SyntaxTree+ParsedImplFileFragment: Boolean get_IsNamedModule()
FSharp.Compiler.SyntaxTree+ParsedImplFileFragment: Boolean get_IsNamespaceFragment()
FSharp.Compiler.SyntaxTree+ParsedImplFileFragment: FSharp.Compiler.SyntaxTree+ParsedImplFileFragment+AnonModule
FSharp.Compiler.SyntaxTree+ParsedImplFileFragment: FSharp.Compiler.SyntaxTree+ParsedImplFileFragment+NamedModule
FSharp.Compiler.SyntaxTree+ParsedImplFileFragment: FSharp.Compiler.SyntaxTree+ParsedImplFileFragment+NamespaceFragment
FSharp.Compiler.SyntaxTree+ParsedImplFileFragment: FSharp.Compiler.SyntaxTree+ParsedImplFileFragment+Tags
FSharp.Compiler.SyntaxTree+ParsedImplFileFragment: Int32 Tag
FSharp.Compiler.SyntaxTree+ParsedImplFileFragment: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+ParsedImplFileFragment: ParsedImplFileFragment NewAnonModule(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynModuleDecl], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+ParsedImplFileFragment: ParsedImplFileFragment NewNamedModule(SynModuleOrNamespace)
FSharp.Compiler.SyntaxTree+ParsedImplFileFragment: ParsedImplFileFragment NewNamespaceFragment(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident], Boolean, SynModuleOrNamespaceKind, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynModuleDecl], PreXmlDoc, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+ParsedImplFileFragment: System.String ToString()
FSharp.Compiler.SyntaxTree+ParsedImplFileInput: Boolean get_isScript()
FSharp.Compiler.SyntaxTree+ParsedImplFileInput: Boolean isScript
FSharp.Compiler.SyntaxTree+ParsedImplFileInput: Int32 Tag
FSharp.Compiler.SyntaxTree+ParsedImplFileInput: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+ParsedImplFileInput: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+ParsedHashDirective] get_hashDirectives()
FSharp.Compiler.SyntaxTree+ParsedImplFileInput: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+ParsedHashDirective] hashDirectives
FSharp.Compiler.SyntaxTree+ParsedImplFileInput: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+ScopedPragma] get_scopedPragmas()
FSharp.Compiler.SyntaxTree+ParsedImplFileInput: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+ScopedPragma] scopedPragmas
FSharp.Compiler.SyntaxTree+ParsedImplFileInput: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynModuleOrNamespace] get_modules()
FSharp.Compiler.SyntaxTree+ParsedImplFileInput: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynModuleOrNamespace] modules
FSharp.Compiler.SyntaxTree+ParsedImplFileInput: ParsedImplFileInput NewParsedImplFileInput(System.String, Boolean, QualifiedNameOfFile, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+ScopedPragma], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+ParsedHashDirective], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynModuleOrNamespace], System.Tuple`2[System.Boolean,System.Boolean])
FSharp.Compiler.SyntaxTree+ParsedImplFileInput: QualifiedNameOfFile get_qualifiedNameOfFile()
FSharp.Compiler.SyntaxTree+ParsedImplFileInput: QualifiedNameOfFile qualifiedNameOfFile
FSharp.Compiler.SyntaxTree+ParsedImplFileInput: System.String ToString()
FSharp.Compiler.SyntaxTree+ParsedImplFileInput: System.String fileName
FSharp.Compiler.SyntaxTree+ParsedImplFileInput: System.String get_fileName()
FSharp.Compiler.SyntaxTree+ParsedImplFileInput: System.Tuple`2[System.Boolean,System.Boolean] get_isLastCompiland()
FSharp.Compiler.SyntaxTree+ParsedImplFileInput: System.Tuple`2[System.Boolean,System.Boolean] isLastCompiland
FSharp.Compiler.SyntaxTree+ParsedInput+ImplFile: ParsedImplFileInput Item
FSharp.Compiler.SyntaxTree+ParsedInput+ImplFile: ParsedImplFileInput get_Item()
FSharp.Compiler.SyntaxTree+ParsedInput+SigFile: ParsedSigFileInput Item
FSharp.Compiler.SyntaxTree+ParsedInput+SigFile: ParsedSigFileInput get_Item()
FSharp.Compiler.SyntaxTree+ParsedInput+Tags: Int32 ImplFile
FSharp.Compiler.SyntaxTree+ParsedInput+Tags: Int32 SigFile
FSharp.Compiler.SyntaxTree+ParsedInput: Boolean IsImplFile
FSharp.Compiler.SyntaxTree+ParsedInput: Boolean IsSigFile
FSharp.Compiler.SyntaxTree+ParsedInput: Boolean get_IsImplFile()
FSharp.Compiler.SyntaxTree+ParsedInput: Boolean get_IsSigFile()
FSharp.Compiler.SyntaxTree+ParsedInput: FSharp.Compiler.SyntaxTree+ParsedInput+ImplFile
FSharp.Compiler.SyntaxTree+ParsedInput: FSharp.Compiler.SyntaxTree+ParsedInput+SigFile
FSharp.Compiler.SyntaxTree+ParsedInput: FSharp.Compiler.SyntaxTree+ParsedInput+Tags
FSharp.Compiler.SyntaxTree+ParsedInput: FSharp.Compiler.Text.Range Range
FSharp.Compiler.SyntaxTree+ParsedInput: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.SyntaxTree+ParsedInput: Int32 Tag
FSharp.Compiler.SyntaxTree+ParsedInput: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+ParsedInput: ParsedInput NewImplFile(ParsedImplFileInput)
FSharp.Compiler.SyntaxTree+ParsedInput: ParsedInput NewSigFile(ParsedSigFileInput)
FSharp.Compiler.SyntaxTree+ParsedInput: System.String ToString()
FSharp.Compiler.SyntaxTree+ParsedSigFile: Int32 Tag
FSharp.Compiler.SyntaxTree+ParsedSigFile: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+ParsedSigFile: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+ParsedHashDirective] get_hashDirectives()
FSharp.Compiler.SyntaxTree+ParsedSigFile: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+ParsedHashDirective] hashDirectives
FSharp.Compiler.SyntaxTree+ParsedSigFile: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+ParsedSigFileFragment] fragments
FSharp.Compiler.SyntaxTree+ParsedSigFile: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+ParsedSigFileFragment] get_fragments()
FSharp.Compiler.SyntaxTree+ParsedSigFile: ParsedSigFile NewParsedSigFile(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+ParsedHashDirective], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+ParsedSigFileFragment])
FSharp.Compiler.SyntaxTree+ParsedSigFile: System.String ToString()
FSharp.Compiler.SyntaxTree+ParsedSigFileFragment+AnonModule: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+ParsedSigFileFragment+AnonModule: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+ParsedSigFileFragment+AnonModule: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynModuleSigDecl] decls
FSharp.Compiler.SyntaxTree+ParsedSigFileFragment+AnonModule: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynModuleSigDecl] get_decls()
FSharp.Compiler.SyntaxTree+ParsedSigFileFragment+NamedModule: SynModuleOrNamespaceSig get_namedModule()
FSharp.Compiler.SyntaxTree+ParsedSigFileFragment+NamedModule: SynModuleOrNamespaceSig namedModule
FSharp.Compiler.SyntaxTree+ParsedSigFileFragment+NamespaceFragment: Boolean get_isRecursive()
FSharp.Compiler.SyntaxTree+ParsedSigFileFragment+NamespaceFragment: Boolean isRecursive
FSharp.Compiler.SyntaxTree+ParsedSigFileFragment+NamespaceFragment: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+ParsedSigFileFragment+NamespaceFragment: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+ParsedSigFileFragment+NamespaceFragment: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident] get_longId()
FSharp.Compiler.SyntaxTree+ParsedSigFileFragment+NamespaceFragment: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident] longId
FSharp.Compiler.SyntaxTree+ParsedSigFileFragment+NamespaceFragment: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList] attributes
FSharp.Compiler.SyntaxTree+ParsedSigFileFragment+NamespaceFragment: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList] get_attributes()
FSharp.Compiler.SyntaxTree+ParsedSigFileFragment+NamespaceFragment: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynModuleSigDecl] decls
FSharp.Compiler.SyntaxTree+ParsedSigFileFragment+NamespaceFragment: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynModuleSigDecl] get_decls()
FSharp.Compiler.SyntaxTree+ParsedSigFileFragment+NamespaceFragment: PreXmlDoc get_xmlDoc()
FSharp.Compiler.SyntaxTree+ParsedSigFileFragment+NamespaceFragment: PreXmlDoc xmlDoc
FSharp.Compiler.SyntaxTree+ParsedSigFileFragment+NamespaceFragment: SynModuleOrNamespaceKind get_kind()
FSharp.Compiler.SyntaxTree+ParsedSigFileFragment+NamespaceFragment: SynModuleOrNamespaceKind kind
FSharp.Compiler.SyntaxTree+ParsedSigFileFragment+Tags: Int32 AnonModule
FSharp.Compiler.SyntaxTree+ParsedSigFileFragment+Tags: Int32 NamedModule
FSharp.Compiler.SyntaxTree+ParsedSigFileFragment+Tags: Int32 NamespaceFragment
FSharp.Compiler.SyntaxTree+ParsedSigFileFragment: Boolean IsAnonModule
FSharp.Compiler.SyntaxTree+ParsedSigFileFragment: Boolean IsNamedModule
FSharp.Compiler.SyntaxTree+ParsedSigFileFragment: Boolean IsNamespaceFragment
FSharp.Compiler.SyntaxTree+ParsedSigFileFragment: Boolean get_IsAnonModule()
FSharp.Compiler.SyntaxTree+ParsedSigFileFragment: Boolean get_IsNamedModule()
FSharp.Compiler.SyntaxTree+ParsedSigFileFragment: Boolean get_IsNamespaceFragment()
FSharp.Compiler.SyntaxTree+ParsedSigFileFragment: FSharp.Compiler.SyntaxTree+ParsedSigFileFragment+AnonModule
FSharp.Compiler.SyntaxTree+ParsedSigFileFragment: FSharp.Compiler.SyntaxTree+ParsedSigFileFragment+NamedModule
FSharp.Compiler.SyntaxTree+ParsedSigFileFragment: FSharp.Compiler.SyntaxTree+ParsedSigFileFragment+NamespaceFragment
FSharp.Compiler.SyntaxTree+ParsedSigFileFragment: FSharp.Compiler.SyntaxTree+ParsedSigFileFragment+Tags
FSharp.Compiler.SyntaxTree+ParsedSigFileFragment: Int32 Tag
FSharp.Compiler.SyntaxTree+ParsedSigFileFragment: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+ParsedSigFileFragment: ParsedSigFileFragment NewAnonModule(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynModuleSigDecl], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+ParsedSigFileFragment: ParsedSigFileFragment NewNamedModule(SynModuleOrNamespaceSig)
FSharp.Compiler.SyntaxTree+ParsedSigFileFragment: ParsedSigFileFragment NewNamespaceFragment(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident], Boolean, SynModuleOrNamespaceKind, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynModuleSigDecl], PreXmlDoc, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+ParsedSigFileFragment: System.String ToString()
FSharp.Compiler.SyntaxTree+ParsedSigFileInput: Int32 Tag
FSharp.Compiler.SyntaxTree+ParsedSigFileInput: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+ParsedSigFileInput: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+ParsedHashDirective] get_hashDirectives()
FSharp.Compiler.SyntaxTree+ParsedSigFileInput: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+ParsedHashDirective] hashDirectives
FSharp.Compiler.SyntaxTree+ParsedSigFileInput: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+ScopedPragma] get_scopedPragmas()
FSharp.Compiler.SyntaxTree+ParsedSigFileInput: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+ScopedPragma] scopedPragmas
FSharp.Compiler.SyntaxTree+ParsedSigFileInput: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceSig] get_modules()
FSharp.Compiler.SyntaxTree+ParsedSigFileInput: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceSig] modules
FSharp.Compiler.SyntaxTree+ParsedSigFileInput: ParsedSigFileInput NewParsedSigFileInput(System.String, QualifiedNameOfFile, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+ScopedPragma], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+ParsedHashDirective], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceSig])
FSharp.Compiler.SyntaxTree+ParsedSigFileInput: QualifiedNameOfFile get_qualifiedNameOfFile()
FSharp.Compiler.SyntaxTree+ParsedSigFileInput: QualifiedNameOfFile qualifiedNameOfFile
FSharp.Compiler.SyntaxTree+ParsedSigFileInput: System.String ToString()
FSharp.Compiler.SyntaxTree+ParsedSigFileInput: System.String fileName
FSharp.Compiler.SyntaxTree+ParsedSigFileInput: System.String get_fileName()
FSharp.Compiler.SyntaxTree+ParserDetail+Tags: Int32 ErrorRecovery
FSharp.Compiler.SyntaxTree+ParserDetail+Tags: Int32 Ok
FSharp.Compiler.SyntaxTree+ParserDetail: Boolean Equals(ParserDetail)
FSharp.Compiler.SyntaxTree+ParserDetail: Boolean Equals(System.Object)
FSharp.Compiler.SyntaxTree+ParserDetail: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SyntaxTree+ParserDetail: Boolean IsErrorRecovery
FSharp.Compiler.SyntaxTree+ParserDetail: Boolean IsOk
FSharp.Compiler.SyntaxTree+ParserDetail: Boolean get_IsErrorRecovery()
FSharp.Compiler.SyntaxTree+ParserDetail: Boolean get_IsOk()
FSharp.Compiler.SyntaxTree+ParserDetail: FSharp.Compiler.SyntaxTree+ParserDetail+Tags
FSharp.Compiler.SyntaxTree+ParserDetail: Int32 CompareTo(ParserDetail)
FSharp.Compiler.SyntaxTree+ParserDetail: Int32 CompareTo(System.Object)
FSharp.Compiler.SyntaxTree+ParserDetail: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.SyntaxTree+ParserDetail: Int32 GetHashCode()
FSharp.Compiler.SyntaxTree+ParserDetail: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SyntaxTree+ParserDetail: Int32 Tag
FSharp.Compiler.SyntaxTree+ParserDetail: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+ParserDetail: ParserDetail ErrorRecovery
FSharp.Compiler.SyntaxTree+ParserDetail: ParserDetail Ok
FSharp.Compiler.SyntaxTree+ParserDetail: ParserDetail get_ErrorRecovery()
FSharp.Compiler.SyntaxTree+ParserDetail: ParserDetail get_Ok()
FSharp.Compiler.SyntaxTree+ParserDetail: System.String ToString()
FSharp.Compiler.SyntaxTree+QualifiedNameOfFile: FSharp.Compiler.Text.Range Range
FSharp.Compiler.SyntaxTree+QualifiedNameOfFile: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.SyntaxTree+QualifiedNameOfFile: Ident Id
FSharp.Compiler.SyntaxTree+QualifiedNameOfFile: Ident Item
FSharp.Compiler.SyntaxTree+QualifiedNameOfFile: Ident get_Id()
FSharp.Compiler.SyntaxTree+QualifiedNameOfFile: Ident get_Item()
FSharp.Compiler.SyntaxTree+QualifiedNameOfFile: Int32 Tag
FSharp.Compiler.SyntaxTree+QualifiedNameOfFile: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+QualifiedNameOfFile: QualifiedNameOfFile NewQualifiedNameOfFile(Ident)
FSharp.Compiler.SyntaxTree+QualifiedNameOfFile: System.String Text
FSharp.Compiler.SyntaxTree+QualifiedNameOfFile: System.String ToString()
FSharp.Compiler.SyntaxTree+QualifiedNameOfFile: System.String get_Text()
FSharp.Compiler.SyntaxTree+ScopedPragma: Boolean Equals(ScopedPragma)
FSharp.Compiler.SyntaxTree+ScopedPragma: Boolean Equals(System.Object)
FSharp.Compiler.SyntaxTree+ScopedPragma: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SyntaxTree+ScopedPragma: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+ScopedPragma: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+ScopedPragma: Int32 GetHashCode()
FSharp.Compiler.SyntaxTree+ScopedPragma: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SyntaxTree+ScopedPragma: Int32 Tag
FSharp.Compiler.SyntaxTree+ScopedPragma: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+ScopedPragma: Int32 get_warningNumber()
FSharp.Compiler.SyntaxTree+ScopedPragma: Int32 warningNumber
FSharp.Compiler.SyntaxTree+ScopedPragma: ScopedPragma NewWarningOff(FSharp.Compiler.Text.Range, Int32)
FSharp.Compiler.SyntaxTree+ScopedPragma: System.String ToString()
FSharp.Compiler.SyntaxTree+SeqExprOnly: Boolean Equals(SeqExprOnly)
FSharp.Compiler.SyntaxTree+SeqExprOnly: Boolean Equals(System.Object)
FSharp.Compiler.SyntaxTree+SeqExprOnly: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SyntaxTree+SeqExprOnly: Boolean Item
FSharp.Compiler.SyntaxTree+SeqExprOnly: Boolean get_Item()
FSharp.Compiler.SyntaxTree+SeqExprOnly: Int32 CompareTo(SeqExprOnly)
FSharp.Compiler.SyntaxTree+SeqExprOnly: Int32 CompareTo(System.Object)
FSharp.Compiler.SyntaxTree+SeqExprOnly: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.SyntaxTree+SeqExprOnly: Int32 GetHashCode()
FSharp.Compiler.SyntaxTree+SeqExprOnly: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SyntaxTree+SeqExprOnly: Int32 Tag
FSharp.Compiler.SyntaxTree+SeqExprOnly: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SeqExprOnly: SeqExprOnly NewSeqExprOnly(Boolean)
FSharp.Compiler.SyntaxTree+SeqExprOnly: System.String ToString()
FSharp.Compiler.SyntaxTree+SynAccess+Tags: Int32 Internal
FSharp.Compiler.SyntaxTree+SynAccess+Tags: Int32 Private
FSharp.Compiler.SyntaxTree+SynAccess+Tags: Int32 Public
FSharp.Compiler.SyntaxTree+SynAccess: Boolean Equals(SynAccess)
FSharp.Compiler.SyntaxTree+SynAccess: Boolean Equals(System.Object)
FSharp.Compiler.SyntaxTree+SynAccess: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SyntaxTree+SynAccess: Boolean IsInternal
FSharp.Compiler.SyntaxTree+SynAccess: Boolean IsPrivate
FSharp.Compiler.SyntaxTree+SynAccess: Boolean IsPublic
FSharp.Compiler.SyntaxTree+SynAccess: Boolean get_IsInternal()
FSharp.Compiler.SyntaxTree+SynAccess: Boolean get_IsPrivate()
FSharp.Compiler.SyntaxTree+SynAccess: Boolean get_IsPublic()
FSharp.Compiler.SyntaxTree+SynAccess: FSharp.Compiler.SyntaxTree+SynAccess+Tags
FSharp.Compiler.SyntaxTree+SynAccess: Int32 CompareTo(SynAccess)
FSharp.Compiler.SyntaxTree+SynAccess: Int32 CompareTo(System.Object)
FSharp.Compiler.SyntaxTree+SynAccess: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.SyntaxTree+SynAccess: Int32 GetHashCode()
FSharp.Compiler.SyntaxTree+SynAccess: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SyntaxTree+SynAccess: Int32 Tag
FSharp.Compiler.SyntaxTree+SynAccess: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynAccess: SynAccess Internal
FSharp.Compiler.SyntaxTree+SynAccess: SynAccess Private
FSharp.Compiler.SyntaxTree+SynAccess: SynAccess Public
FSharp.Compiler.SyntaxTree+SynAccess: SynAccess get_Internal()
FSharp.Compiler.SyntaxTree+SynAccess: SynAccess get_Private()
FSharp.Compiler.SyntaxTree+SynAccess: SynAccess get_Public()
FSharp.Compiler.SyntaxTree+SynAccess: System.String ToString()
FSharp.Compiler.SyntaxTree+SynArgInfo: Boolean get_optional()
FSharp.Compiler.SyntaxTree+SynArgInfo: Boolean optional
FSharp.Compiler.SyntaxTree+SynArgInfo: Int32 Tag
FSharp.Compiler.SyntaxTree+SynArgInfo: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynArgInfo: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList] attributes
FSharp.Compiler.SyntaxTree+SynArgInfo: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList] get_attributes()
FSharp.Compiler.SyntaxTree+SynArgInfo: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+Ident] Ident
FSharp.Compiler.SyntaxTree+SynArgInfo: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+Ident] get_Ident()
FSharp.Compiler.SyntaxTree+SynArgInfo: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+Ident] get_ident()
FSharp.Compiler.SyntaxTree+SynArgInfo: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+Ident] ident
FSharp.Compiler.SyntaxTree+SynArgInfo: SynArgInfo NewSynArgInfo(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList], Boolean, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+Ident])
FSharp.Compiler.SyntaxTree+SynArgInfo: System.String ToString()
FSharp.Compiler.SyntaxTree+SynArgPats+NamePatPairs: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynArgPats+NamePatPairs: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynArgPats+NamePatPairs: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[FSharp.Compiler.SyntaxTree+Ident,FSharp.Compiler.SyntaxTree+SynPat]] get_pats()
FSharp.Compiler.SyntaxTree+SynArgPats+NamePatPairs: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[FSharp.Compiler.SyntaxTree+Ident,FSharp.Compiler.SyntaxTree+SynPat]] pats
FSharp.Compiler.SyntaxTree+SynArgPats+Pats: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynPat] get_pats()
FSharp.Compiler.SyntaxTree+SynArgPats+Pats: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynPat] pats
FSharp.Compiler.SyntaxTree+SynArgPats+Tags: Int32 NamePatPairs
FSharp.Compiler.SyntaxTree+SynArgPats+Tags: Int32 Pats
FSharp.Compiler.SyntaxTree+SynArgPats: Boolean IsNamePatPairs
FSharp.Compiler.SyntaxTree+SynArgPats: Boolean IsPats
FSharp.Compiler.SyntaxTree+SynArgPats: Boolean get_IsNamePatPairs()
FSharp.Compiler.SyntaxTree+SynArgPats: Boolean get_IsPats()
FSharp.Compiler.SyntaxTree+SynArgPats: FSharp.Compiler.SyntaxTree+SynArgPats+NamePatPairs
FSharp.Compiler.SyntaxTree+SynArgPats: FSharp.Compiler.SyntaxTree+SynArgPats+Pats
FSharp.Compiler.SyntaxTree+SynArgPats: FSharp.Compiler.SyntaxTree+SynArgPats+Tags
FSharp.Compiler.SyntaxTree+SynArgPats: Int32 Tag
FSharp.Compiler.SyntaxTree+SynArgPats: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynArgPats: SynArgPats NewNamePatPairs(Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[FSharp.Compiler.SyntaxTree+Ident,FSharp.Compiler.SyntaxTree+SynPat]], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynArgPats: SynArgPats NewPats(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynPat])
FSharp.Compiler.SyntaxTree+SynArgPats: System.String ToString()
FSharp.Compiler.SyntaxTree+SynAttribute: Boolean AppliesToGetterAndSetter
FSharp.Compiler.SyntaxTree+SynAttribute: Boolean get_AppliesToGetterAndSetter()
FSharp.Compiler.SyntaxTree+SynAttribute: FSharp.Compiler.Text.Range Range
FSharp.Compiler.SyntaxTree+SynAttribute: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.SyntaxTree+SynAttribute: LongIdentWithDots TypeName
FSharp.Compiler.SyntaxTree+SynAttribute: LongIdentWithDots get_TypeName()
FSharp.Compiler.SyntaxTree+SynAttribute: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+Ident] Target
FSharp.Compiler.SyntaxTree+SynAttribute: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+Ident] get_Target()
FSharp.Compiler.SyntaxTree+SynAttribute: SynExpr ArgExpr
FSharp.Compiler.SyntaxTree+SynAttribute: SynExpr get_ArgExpr()
FSharp.Compiler.SyntaxTree+SynAttribute: System.String ToString()
FSharp.Compiler.SyntaxTree+SynAttribute: Void .ctor(LongIdentWithDots, SynExpr, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+Ident], Boolean, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynAttributeList: FSharp.Compiler.Text.Range Range
FSharp.Compiler.SyntaxTree+SynAttributeList: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.SyntaxTree+SynAttributeList: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttribute] Attributes
FSharp.Compiler.SyntaxTree+SynAttributeList: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttribute] get_Attributes()
FSharp.Compiler.SyntaxTree+SynAttributeList: System.String ToString()
FSharp.Compiler.SyntaxTree+SynAttributeList: Void .ctor(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttribute], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynBinding: Boolean get_isMutable()
FSharp.Compiler.SyntaxTree+SynBinding: Boolean get_mustInline()
FSharp.Compiler.SyntaxTree+SynBinding: Boolean isMutable
FSharp.Compiler.SyntaxTree+SynBinding: Boolean mustInline
FSharp.Compiler.SyntaxTree+SynBinding: DebugPointForBinding get_seqPoint()
FSharp.Compiler.SyntaxTree+SynBinding: DebugPointForBinding seqPoint
FSharp.Compiler.SyntaxTree+SynBinding: FSharp.Compiler.Text.Range RangeOfBindingAndRhs
FSharp.Compiler.SyntaxTree+SynBinding: FSharp.Compiler.Text.Range RangeOfBindingSansRhs
FSharp.Compiler.SyntaxTree+SynBinding: FSharp.Compiler.Text.Range RangeOfHeadPat
FSharp.Compiler.SyntaxTree+SynBinding: FSharp.Compiler.Text.Range get_RangeOfBindingAndRhs()
FSharp.Compiler.SyntaxTree+SynBinding: FSharp.Compiler.Text.Range get_RangeOfBindingSansRhs()
FSharp.Compiler.SyntaxTree+SynBinding: FSharp.Compiler.Text.Range get_RangeOfHeadPat()
FSharp.Compiler.SyntaxTree+SynBinding: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynBinding: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynBinding: Int32 Tag
FSharp.Compiler.SyntaxTree+SynBinding: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynBinding: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList] attributes
FSharp.Compiler.SyntaxTree+SynBinding: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList] get_attributes()
FSharp.Compiler.SyntaxTree+SynBinding: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess] accessibility
FSharp.Compiler.SyntaxTree+SynBinding: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess] get_accessibility()
FSharp.Compiler.SyntaxTree+SynBinding: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynBindingReturnInfo] get_returnInfo()
FSharp.Compiler.SyntaxTree+SynBinding: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynBindingReturnInfo] returnInfo
FSharp.Compiler.SyntaxTree+SynBinding: PreXmlDoc get_xmlDoc()
FSharp.Compiler.SyntaxTree+SynBinding: PreXmlDoc xmlDoc
FSharp.Compiler.SyntaxTree+SynBinding: SynBinding NewBinding(Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess], SynBindingKind, Boolean, Boolean, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList], PreXmlDoc, SynValData, SynPat, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynBindingReturnInfo], SynExpr, FSharp.Compiler.Text.Range, DebugPointForBinding)
FSharp.Compiler.SyntaxTree+SynBinding: SynBindingKind get_kind()
FSharp.Compiler.SyntaxTree+SynBinding: SynBindingKind kind
FSharp.Compiler.SyntaxTree+SynBinding: SynExpr expr
FSharp.Compiler.SyntaxTree+SynBinding: SynExpr get_expr()
FSharp.Compiler.SyntaxTree+SynBinding: SynPat get_headPat()
FSharp.Compiler.SyntaxTree+SynBinding: SynPat headPat
FSharp.Compiler.SyntaxTree+SynBinding: SynValData get_valData()
FSharp.Compiler.SyntaxTree+SynBinding: SynValData valData
FSharp.Compiler.SyntaxTree+SynBinding: System.String ToString()
FSharp.Compiler.SyntaxTree+SynBindingKind+Tags: Int32 DoBinding
FSharp.Compiler.SyntaxTree+SynBindingKind+Tags: Int32 NormalBinding
FSharp.Compiler.SyntaxTree+SynBindingKind+Tags: Int32 StandaloneExpression
FSharp.Compiler.SyntaxTree+SynBindingKind: Boolean Equals(SynBindingKind)
FSharp.Compiler.SyntaxTree+SynBindingKind: Boolean Equals(System.Object)
FSharp.Compiler.SyntaxTree+SynBindingKind: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SyntaxTree+SynBindingKind: Boolean IsDoBinding
FSharp.Compiler.SyntaxTree+SynBindingKind: Boolean IsNormalBinding
FSharp.Compiler.SyntaxTree+SynBindingKind: Boolean IsStandaloneExpression
FSharp.Compiler.SyntaxTree+SynBindingKind: Boolean get_IsDoBinding()
FSharp.Compiler.SyntaxTree+SynBindingKind: Boolean get_IsNormalBinding()
FSharp.Compiler.SyntaxTree+SynBindingKind: Boolean get_IsStandaloneExpression()
FSharp.Compiler.SyntaxTree+SynBindingKind: FSharp.Compiler.SyntaxTree+SynBindingKind+Tags
FSharp.Compiler.SyntaxTree+SynBindingKind: Int32 CompareTo(SynBindingKind)
FSharp.Compiler.SyntaxTree+SynBindingKind: Int32 CompareTo(System.Object)
FSharp.Compiler.SyntaxTree+SynBindingKind: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.SyntaxTree+SynBindingKind: Int32 GetHashCode()
FSharp.Compiler.SyntaxTree+SynBindingKind: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SyntaxTree+SynBindingKind: Int32 Tag
FSharp.Compiler.SyntaxTree+SynBindingKind: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynBindingKind: SynBindingKind DoBinding
FSharp.Compiler.SyntaxTree+SynBindingKind: SynBindingKind NormalBinding
FSharp.Compiler.SyntaxTree+SynBindingKind: SynBindingKind StandaloneExpression
FSharp.Compiler.SyntaxTree+SynBindingKind: SynBindingKind get_DoBinding()
FSharp.Compiler.SyntaxTree+SynBindingKind: SynBindingKind get_NormalBinding()
FSharp.Compiler.SyntaxTree+SynBindingKind: SynBindingKind get_StandaloneExpression()
FSharp.Compiler.SyntaxTree+SynBindingKind: System.String ToString()
FSharp.Compiler.SyntaxTree+SynBindingReturnInfo: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynBindingReturnInfo: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynBindingReturnInfo: Int32 Tag
FSharp.Compiler.SyntaxTree+SynBindingReturnInfo: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynBindingReturnInfo: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList] attributes
FSharp.Compiler.SyntaxTree+SynBindingReturnInfo: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList] get_attributes()
FSharp.Compiler.SyntaxTree+SynBindingReturnInfo: SynBindingReturnInfo NewSynBindingReturnInfo(SynType, FSharp.Compiler.Text.Range, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList])
FSharp.Compiler.SyntaxTree+SynBindingReturnInfo: SynType get_typeName()
FSharp.Compiler.SyntaxTree+SynBindingReturnInfo: SynType typeName
FSharp.Compiler.SyntaxTree+SynBindingReturnInfo: System.String ToString()
FSharp.Compiler.SyntaxTree+SynComponentInfo: Boolean get_preferPostfix()
FSharp.Compiler.SyntaxTree+SynComponentInfo: Boolean preferPostfix
FSharp.Compiler.SyntaxTree+SynComponentInfo: FSharp.Compiler.Text.Range Range
FSharp.Compiler.SyntaxTree+SynComponentInfo: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.SyntaxTree+SynComponentInfo: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynComponentInfo: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynComponentInfo: Int32 Tag
FSharp.Compiler.SyntaxTree+SynComponentInfo: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynComponentInfo: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident] get_longId()
FSharp.Compiler.SyntaxTree+SynComponentInfo: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident] longId
FSharp.Compiler.SyntaxTree+SynComponentInfo: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList] attributes
FSharp.Compiler.SyntaxTree+SynComponentInfo: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList] get_attributes()
FSharp.Compiler.SyntaxTree+SynComponentInfo: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynTyparDecl] get_typeParams()
FSharp.Compiler.SyntaxTree+SynComponentInfo: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynTyparDecl] typeParams
FSharp.Compiler.SyntaxTree+SynComponentInfo: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynTypeConstraint] constraints
FSharp.Compiler.SyntaxTree+SynComponentInfo: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynTypeConstraint] get_constraints()
FSharp.Compiler.SyntaxTree+SynComponentInfo: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess] accessibility
FSharp.Compiler.SyntaxTree+SynComponentInfo: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess] get_accessibility()
FSharp.Compiler.SyntaxTree+SynComponentInfo: PreXmlDoc get_xmlDoc()
FSharp.Compiler.SyntaxTree+SynComponentInfo: PreXmlDoc xmlDoc
FSharp.Compiler.SyntaxTree+SynComponentInfo: SynComponentInfo NewComponentInfo(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynTyparDecl], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynTypeConstraint], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident], PreXmlDoc, Boolean, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynComponentInfo: System.String ToString()
FSharp.Compiler.SyntaxTree+SynConst+Bool: Boolean Item
FSharp.Compiler.SyntaxTree+SynConst+Bool: Boolean get_Item()
FSharp.Compiler.SyntaxTree+SynConst+Byte: Byte Item
FSharp.Compiler.SyntaxTree+SynConst+Byte: Byte get_Item()
FSharp.Compiler.SyntaxTree+SynConst+Bytes: Byte[] bytes
FSharp.Compiler.SyntaxTree+SynConst+Bytes: Byte[] get_bytes()
FSharp.Compiler.SyntaxTree+SynConst+Bytes: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynConst+Bytes: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynConst+Char: Char Item
FSharp.Compiler.SyntaxTree+SynConst+Char: Char get_Item()
FSharp.Compiler.SyntaxTree+SynConst+Decimal: System.Decimal Item
FSharp.Compiler.SyntaxTree+SynConst+Decimal: System.Decimal get_Item()
FSharp.Compiler.SyntaxTree+SynConst+Double: Double Item
FSharp.Compiler.SyntaxTree+SynConst+Double: Double get_Item()
FSharp.Compiler.SyntaxTree+SynConst+Int16: Int16 Item
FSharp.Compiler.SyntaxTree+SynConst+Int16: Int16 get_Item()
FSharp.Compiler.SyntaxTree+SynConst+Int32: Int32 Item
FSharp.Compiler.SyntaxTree+SynConst+Int32: Int32 get_Item()
FSharp.Compiler.SyntaxTree+SynConst+Int64: Int64 Item
FSharp.Compiler.SyntaxTree+SynConst+Int64: Int64 get_Item()
FSharp.Compiler.SyntaxTree+SynConst+IntPtr: Int64 Item
FSharp.Compiler.SyntaxTree+SynConst+IntPtr: Int64 get_Item()
FSharp.Compiler.SyntaxTree+SynConst+Measure: SynConst constant
FSharp.Compiler.SyntaxTree+SynConst+Measure: SynConst get_constant()
FSharp.Compiler.SyntaxTree+SynConst+Measure: SynMeasure Item2
FSharp.Compiler.SyntaxTree+SynConst+Measure: SynMeasure get_Item2()
FSharp.Compiler.SyntaxTree+SynConst+SByte: SByte Item
FSharp.Compiler.SyntaxTree+SynConst+SByte: SByte get_Item()
FSharp.Compiler.SyntaxTree+SynConst+Single: Single Item
FSharp.Compiler.SyntaxTree+SynConst+Single: Single get_Item()
FSharp.Compiler.SyntaxTree+SynConst+String: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynConst+String: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynConst+String: System.String get_text()
FSharp.Compiler.SyntaxTree+SynConst+String: System.String text
FSharp.Compiler.SyntaxTree+SynConst+Tags: Int32 Bool
FSharp.Compiler.SyntaxTree+SynConst+Tags: Int32 Byte
FSharp.Compiler.SyntaxTree+SynConst+Tags: Int32 Bytes
FSharp.Compiler.SyntaxTree+SynConst+Tags: Int32 Char
FSharp.Compiler.SyntaxTree+SynConst+Tags: Int32 Decimal
FSharp.Compiler.SyntaxTree+SynConst+Tags: Int32 Double
FSharp.Compiler.SyntaxTree+SynConst+Tags: Int32 Int16
FSharp.Compiler.SyntaxTree+SynConst+Tags: Int32 Int32
FSharp.Compiler.SyntaxTree+SynConst+Tags: Int32 Int64
FSharp.Compiler.SyntaxTree+SynConst+Tags: Int32 IntPtr
FSharp.Compiler.SyntaxTree+SynConst+Tags: Int32 Measure
FSharp.Compiler.SyntaxTree+SynConst+Tags: Int32 SByte
FSharp.Compiler.SyntaxTree+SynConst+Tags: Int32 Single
FSharp.Compiler.SyntaxTree+SynConst+Tags: Int32 String
FSharp.Compiler.SyntaxTree+SynConst+Tags: Int32 UInt16
FSharp.Compiler.SyntaxTree+SynConst+Tags: Int32 UInt16s
FSharp.Compiler.SyntaxTree+SynConst+Tags: Int32 UInt32
FSharp.Compiler.SyntaxTree+SynConst+Tags: Int32 UInt64
FSharp.Compiler.SyntaxTree+SynConst+Tags: Int32 UIntPtr
FSharp.Compiler.SyntaxTree+SynConst+Tags: Int32 Unit
FSharp.Compiler.SyntaxTree+SynConst+Tags: Int32 UserNum
FSharp.Compiler.SyntaxTree+SynConst+UInt16: UInt16 Item
FSharp.Compiler.SyntaxTree+SynConst+UInt16: UInt16 get_Item()
FSharp.Compiler.SyntaxTree+SynConst+UInt16s: UInt16[] Item
FSharp.Compiler.SyntaxTree+SynConst+UInt16s: UInt16[] get_Item()
FSharp.Compiler.SyntaxTree+SynConst+UInt32: UInt32 Item
FSharp.Compiler.SyntaxTree+SynConst+UInt32: UInt32 get_Item()
FSharp.Compiler.SyntaxTree+SynConst+UInt64: UInt64 Item
FSharp.Compiler.SyntaxTree+SynConst+UInt64: UInt64 get_Item()
FSharp.Compiler.SyntaxTree+SynConst+UIntPtr: UInt64 Item
FSharp.Compiler.SyntaxTree+SynConst+UIntPtr: UInt64 get_Item()
FSharp.Compiler.SyntaxTree+SynConst+UserNum: System.String get_suffix()
FSharp.Compiler.SyntaxTree+SynConst+UserNum: System.String get_value()
FSharp.Compiler.SyntaxTree+SynConst+UserNum: System.String suffix
FSharp.Compiler.SyntaxTree+SynConst+UserNum: System.String value
FSharp.Compiler.SyntaxTree+SynConst: Boolean IsBool
FSharp.Compiler.SyntaxTree+SynConst: Boolean IsByte
FSharp.Compiler.SyntaxTree+SynConst: Boolean IsBytes
FSharp.Compiler.SyntaxTree+SynConst: Boolean IsChar
FSharp.Compiler.SyntaxTree+SynConst: Boolean IsDecimal
FSharp.Compiler.SyntaxTree+SynConst: Boolean IsDouble
FSharp.Compiler.SyntaxTree+SynConst: Boolean IsInt16
FSharp.Compiler.SyntaxTree+SynConst: Boolean IsInt32
FSharp.Compiler.SyntaxTree+SynConst: Boolean IsInt64
FSharp.Compiler.SyntaxTree+SynConst: Boolean IsIntPtr
FSharp.Compiler.SyntaxTree+SynConst: Boolean IsMeasure
FSharp.Compiler.SyntaxTree+SynConst: Boolean IsSByte
FSharp.Compiler.SyntaxTree+SynConst: Boolean IsSingle
FSharp.Compiler.SyntaxTree+SynConst: Boolean IsString
FSharp.Compiler.SyntaxTree+SynConst: Boolean IsUInt16
FSharp.Compiler.SyntaxTree+SynConst: Boolean IsUInt16s
FSharp.Compiler.SyntaxTree+SynConst: Boolean IsUInt32
FSharp.Compiler.SyntaxTree+SynConst: Boolean IsUInt64
FSharp.Compiler.SyntaxTree+SynConst: Boolean IsUIntPtr
FSharp.Compiler.SyntaxTree+SynConst: Boolean IsUnit
FSharp.Compiler.SyntaxTree+SynConst: Boolean IsUserNum
FSharp.Compiler.SyntaxTree+SynConst: Boolean get_IsBool()
FSharp.Compiler.SyntaxTree+SynConst: Boolean get_IsByte()
FSharp.Compiler.SyntaxTree+SynConst: Boolean get_IsBytes()
FSharp.Compiler.SyntaxTree+SynConst: Boolean get_IsChar()
FSharp.Compiler.SyntaxTree+SynConst: Boolean get_IsDecimal()
FSharp.Compiler.SyntaxTree+SynConst: Boolean get_IsDouble()
FSharp.Compiler.SyntaxTree+SynConst: Boolean get_IsInt16()
FSharp.Compiler.SyntaxTree+SynConst: Boolean get_IsInt32()
FSharp.Compiler.SyntaxTree+SynConst: Boolean get_IsInt64()
FSharp.Compiler.SyntaxTree+SynConst: Boolean get_IsIntPtr()
FSharp.Compiler.SyntaxTree+SynConst: Boolean get_IsMeasure()
FSharp.Compiler.SyntaxTree+SynConst: Boolean get_IsSByte()
FSharp.Compiler.SyntaxTree+SynConst: Boolean get_IsSingle()
FSharp.Compiler.SyntaxTree+SynConst: Boolean get_IsString()
FSharp.Compiler.SyntaxTree+SynConst: Boolean get_IsUInt16()
FSharp.Compiler.SyntaxTree+SynConst: Boolean get_IsUInt16s()
FSharp.Compiler.SyntaxTree+SynConst: Boolean get_IsUInt32()
FSharp.Compiler.SyntaxTree+SynConst: Boolean get_IsUInt64()
FSharp.Compiler.SyntaxTree+SynConst: Boolean get_IsUIntPtr()
FSharp.Compiler.SyntaxTree+SynConst: Boolean get_IsUnit()
FSharp.Compiler.SyntaxTree+SynConst: Boolean get_IsUserNum()
FSharp.Compiler.SyntaxTree+SynConst: FSharp.Compiler.SyntaxTree+SynConst+Bool
FSharp.Compiler.SyntaxTree+SynConst: FSharp.Compiler.SyntaxTree+SynConst+Byte
FSharp.Compiler.SyntaxTree+SynConst: FSharp.Compiler.SyntaxTree+SynConst+Bytes
FSharp.Compiler.SyntaxTree+SynConst: FSharp.Compiler.SyntaxTree+SynConst+Char
FSharp.Compiler.SyntaxTree+SynConst: FSharp.Compiler.SyntaxTree+SynConst+Decimal
FSharp.Compiler.SyntaxTree+SynConst: FSharp.Compiler.SyntaxTree+SynConst+Double
FSharp.Compiler.SyntaxTree+SynConst: FSharp.Compiler.SyntaxTree+SynConst+Int16
FSharp.Compiler.SyntaxTree+SynConst: FSharp.Compiler.SyntaxTree+SynConst+Int32
FSharp.Compiler.SyntaxTree+SynConst: FSharp.Compiler.SyntaxTree+SynConst+Int64
FSharp.Compiler.SyntaxTree+SynConst: FSharp.Compiler.SyntaxTree+SynConst+IntPtr
FSharp.Compiler.SyntaxTree+SynConst: FSharp.Compiler.SyntaxTree+SynConst+Measure
FSharp.Compiler.SyntaxTree+SynConst: FSharp.Compiler.SyntaxTree+SynConst+SByte
FSharp.Compiler.SyntaxTree+SynConst: FSharp.Compiler.SyntaxTree+SynConst+Single
FSharp.Compiler.SyntaxTree+SynConst: FSharp.Compiler.SyntaxTree+SynConst+String
FSharp.Compiler.SyntaxTree+SynConst: FSharp.Compiler.SyntaxTree+SynConst+Tags
FSharp.Compiler.SyntaxTree+SynConst: FSharp.Compiler.SyntaxTree+SynConst+UInt16
FSharp.Compiler.SyntaxTree+SynConst: FSharp.Compiler.SyntaxTree+SynConst+UInt16s
FSharp.Compiler.SyntaxTree+SynConst: FSharp.Compiler.SyntaxTree+SynConst+UInt32
FSharp.Compiler.SyntaxTree+SynConst: FSharp.Compiler.SyntaxTree+SynConst+UInt64
FSharp.Compiler.SyntaxTree+SynConst: FSharp.Compiler.SyntaxTree+SynConst+UIntPtr
FSharp.Compiler.SyntaxTree+SynConst: FSharp.Compiler.SyntaxTree+SynConst+UserNum
FSharp.Compiler.SyntaxTree+SynConst: FSharp.Compiler.Text.Range Range(FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynConst: Int32 Tag
FSharp.Compiler.SyntaxTree+SynConst: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynConst: SynConst NewBool(Boolean)
FSharp.Compiler.SyntaxTree+SynConst: SynConst NewByte(Byte)
FSharp.Compiler.SyntaxTree+SynConst: SynConst NewBytes(Byte[], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynConst: SynConst NewChar(Char)
FSharp.Compiler.SyntaxTree+SynConst: SynConst NewDecimal(System.Decimal)
FSharp.Compiler.SyntaxTree+SynConst: SynConst NewDouble(Double)
FSharp.Compiler.SyntaxTree+SynConst: SynConst NewInt16(Int16)
FSharp.Compiler.SyntaxTree+SynConst: SynConst NewInt32(Int32)
FSharp.Compiler.SyntaxTree+SynConst: SynConst NewInt64(Int64)
FSharp.Compiler.SyntaxTree+SynConst: SynConst NewIntPtr(Int64)
FSharp.Compiler.SyntaxTree+SynConst: SynConst NewMeasure(SynConst, SynMeasure)
FSharp.Compiler.SyntaxTree+SynConst: SynConst NewSByte(SByte)
FSharp.Compiler.SyntaxTree+SynConst: SynConst NewSingle(Single)
FSharp.Compiler.SyntaxTree+SynConst: SynConst NewString(System.String, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynConst: SynConst NewUInt16(UInt16)
FSharp.Compiler.SyntaxTree+SynConst: SynConst NewUInt16s(UInt16[])
FSharp.Compiler.SyntaxTree+SynConst: SynConst NewUInt32(UInt32)
FSharp.Compiler.SyntaxTree+SynConst: SynConst NewUInt64(UInt64)
FSharp.Compiler.SyntaxTree+SynConst: SynConst NewUIntPtr(UInt64)
FSharp.Compiler.SyntaxTree+SynConst: SynConst NewUserNum(System.String, System.String)
FSharp.Compiler.SyntaxTree+SynConst: SynConst Unit
FSharp.Compiler.SyntaxTree+SynConst: SynConst get_Unit()
FSharp.Compiler.SyntaxTree+SynConst: System.String ToString()
FSharp.Compiler.SyntaxTree+SynEnumCase: FSharp.Compiler.Text.Range Range
FSharp.Compiler.SyntaxTree+SynEnumCase: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.SyntaxTree+SynEnumCase: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynEnumCase: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynEnumCase: Ident get_ident()
FSharp.Compiler.SyntaxTree+SynEnumCase: Ident ident
FSharp.Compiler.SyntaxTree+SynEnumCase: Int32 Tag
FSharp.Compiler.SyntaxTree+SynEnumCase: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynEnumCase: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList] attributes
FSharp.Compiler.SyntaxTree+SynEnumCase: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList] get_attributes()
FSharp.Compiler.SyntaxTree+SynEnumCase: PreXmlDoc get_xmlDoc()
FSharp.Compiler.SyntaxTree+SynEnumCase: PreXmlDoc xmlDoc
FSharp.Compiler.SyntaxTree+SynEnumCase: SynConst get_value()
FSharp.Compiler.SyntaxTree+SynEnumCase: SynConst value
FSharp.Compiler.SyntaxTree+SynEnumCase: SynEnumCase NewEnumCase(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList], Ident, SynConst, PreXmlDoc, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynEnumCase: System.String ToString()
FSharp.Compiler.SyntaxTree+SynExceptionDefn: FSharp.Compiler.Text.Range Range
FSharp.Compiler.SyntaxTree+SynExceptionDefn: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.SyntaxTree+SynExceptionDefn: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExceptionDefn: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExceptionDefn: Int32 Tag
FSharp.Compiler.SyntaxTree+SynExceptionDefn: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynExceptionDefn: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynMemberDefn] get_members()
FSharp.Compiler.SyntaxTree+SynExceptionDefn: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynMemberDefn] members
FSharp.Compiler.SyntaxTree+SynExceptionDefn: SynExceptionDefn NewSynExceptionDefn(SynExceptionDefnRepr, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynMemberDefn], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExceptionDefn: SynExceptionDefnRepr exnRepr
FSharp.Compiler.SyntaxTree+SynExceptionDefn: SynExceptionDefnRepr get_exnRepr()
FSharp.Compiler.SyntaxTree+SynExceptionDefn: System.String ToString()
FSharp.Compiler.SyntaxTree+SynExceptionDefnRepr: FSharp.Compiler.Text.Range Range
FSharp.Compiler.SyntaxTree+SynExceptionDefnRepr: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.SyntaxTree+SynExceptionDefnRepr: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExceptionDefnRepr: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExceptionDefnRepr: Int32 Tag
FSharp.Compiler.SyntaxTree+SynExceptionDefnRepr: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynExceptionDefnRepr: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList] attributes
FSharp.Compiler.SyntaxTree+SynExceptionDefnRepr: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList] get_attributes()
FSharp.Compiler.SyntaxTree+SynExceptionDefnRepr: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess] accessibility
FSharp.Compiler.SyntaxTree+SynExceptionDefnRepr: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess] get_accessibility()
FSharp.Compiler.SyntaxTree+SynExceptionDefnRepr: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident]] get_longId()
FSharp.Compiler.SyntaxTree+SynExceptionDefnRepr: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident]] longId
FSharp.Compiler.SyntaxTree+SynExceptionDefnRepr: PreXmlDoc get_xmlDoc()
FSharp.Compiler.SyntaxTree+SynExceptionDefnRepr: PreXmlDoc xmlDoc
FSharp.Compiler.SyntaxTree+SynExceptionDefnRepr: SynExceptionDefnRepr NewSynExceptionDefnRepr(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList], SynUnionCase, Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident]], PreXmlDoc, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExceptionDefnRepr: SynUnionCase caseName
FSharp.Compiler.SyntaxTree+SynExceptionDefnRepr: SynUnionCase get_caseName()
FSharp.Compiler.SyntaxTree+SynExceptionDefnRepr: System.String ToString()
FSharp.Compiler.SyntaxTree+SynExceptionSig: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExceptionSig: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExceptionSig: Int32 Tag
FSharp.Compiler.SyntaxTree+SynExceptionSig: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynExceptionSig: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynMemberSig] get_members()
FSharp.Compiler.SyntaxTree+SynExceptionSig: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynMemberSig] members
FSharp.Compiler.SyntaxTree+SynExceptionSig: SynExceptionDefnRepr exnRepr
FSharp.Compiler.SyntaxTree+SynExceptionSig: SynExceptionDefnRepr get_exnRepr()
FSharp.Compiler.SyntaxTree+SynExceptionSig: SynExceptionSig NewSynExceptionSig(SynExceptionDefnRepr, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynMemberSig], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExceptionSig: System.String ToString()
FSharp.Compiler.SyntaxTree+SynExpr+AddressOf: Boolean get_isByref()
FSharp.Compiler.SyntaxTree+SynExpr+AddressOf: Boolean isByref
FSharp.Compiler.SyntaxTree+SynExpr+AddressOf: FSharp.Compiler.Text.Range get_opRange()
FSharp.Compiler.SyntaxTree+SynExpr+AddressOf: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+AddressOf: FSharp.Compiler.Text.Range opRange
FSharp.Compiler.SyntaxTree+SynExpr+AddressOf: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+AddressOf: SynExpr expr
FSharp.Compiler.SyntaxTree+SynExpr+AddressOf: SynExpr get_expr()
FSharp.Compiler.SyntaxTree+SynExpr+AnonRecd: Boolean get_isStruct()
FSharp.Compiler.SyntaxTree+SynExpr+AnonRecd: Boolean isStruct
FSharp.Compiler.SyntaxTree+SynExpr+AnonRecd: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+AnonRecd: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+AnonRecd: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[FSharp.Compiler.SyntaxTree+Ident,FSharp.Compiler.SyntaxTree+SynExpr]] get_recordFields()
FSharp.Compiler.SyntaxTree+SynExpr+AnonRecd: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[FSharp.Compiler.SyntaxTree+Ident,FSharp.Compiler.SyntaxTree+SynExpr]] recordFields
FSharp.Compiler.SyntaxTree+SynExpr+AnonRecd: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.SyntaxTree+SynExpr,System.Tuple`2[FSharp.Compiler.Text.Range,Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Pos]]]] copyInfo
FSharp.Compiler.SyntaxTree+SynExpr+AnonRecd: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.SyntaxTree+SynExpr,System.Tuple`2[FSharp.Compiler.Text.Range,Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Pos]]]] get_copyInfo()
FSharp.Compiler.SyntaxTree+SynExpr+App: Boolean get_isInfix()
FSharp.Compiler.SyntaxTree+SynExpr+App: Boolean isInfix
FSharp.Compiler.SyntaxTree+SynExpr+App: ExprAtomicFlag flag
FSharp.Compiler.SyntaxTree+SynExpr+App: ExprAtomicFlag get_flag()
FSharp.Compiler.SyntaxTree+SynExpr+App: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+App: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+App: SynExpr argExpr
FSharp.Compiler.SyntaxTree+SynExpr+App: SynExpr funcExpr
FSharp.Compiler.SyntaxTree+SynExpr+App: SynExpr get_argExpr()
FSharp.Compiler.SyntaxTree+SynExpr+App: SynExpr get_funcExpr()
FSharp.Compiler.SyntaxTree+SynExpr+ArbitraryAfterError: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+ArbitraryAfterError: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+ArbitraryAfterError: System.String debugStr
FSharp.Compiler.SyntaxTree+SynExpr+ArbitraryAfterError: System.String get_debugStr()
FSharp.Compiler.SyntaxTree+SynExpr+ArrayOrList: Boolean get_isList()
FSharp.Compiler.SyntaxTree+SynExpr+ArrayOrList: Boolean isList
FSharp.Compiler.SyntaxTree+SynExpr+ArrayOrList: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+ArrayOrList: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+ArrayOrList: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynExpr] exprs
FSharp.Compiler.SyntaxTree+SynExpr+ArrayOrList: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynExpr] get_exprs()
FSharp.Compiler.SyntaxTree+SynExpr+ArrayOrListOfSeqExpr: Boolean get_isArray()
FSharp.Compiler.SyntaxTree+SynExpr+ArrayOrListOfSeqExpr: Boolean isArray
FSharp.Compiler.SyntaxTree+SynExpr+ArrayOrListOfSeqExpr: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+ArrayOrListOfSeqExpr: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+ArrayOrListOfSeqExpr: SynExpr expr
FSharp.Compiler.SyntaxTree+SynExpr+ArrayOrListOfSeqExpr: SynExpr get_expr()
FSharp.Compiler.SyntaxTree+SynExpr+Assert: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+Assert: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+Assert: SynExpr expr
FSharp.Compiler.SyntaxTree+SynExpr+Assert: SynExpr get_expr()
FSharp.Compiler.SyntaxTree+SynExpr+CompExpr: Boolean get_isArrayOrList()
FSharp.Compiler.SyntaxTree+SynExpr+CompExpr: Boolean isArrayOrList
FSharp.Compiler.SyntaxTree+SynExpr+CompExpr: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+CompExpr: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+CompExpr: Microsoft.FSharp.Core.FSharpRef`1[System.Boolean] get_isNotNakedRefCell()
FSharp.Compiler.SyntaxTree+SynExpr+CompExpr: Microsoft.FSharp.Core.FSharpRef`1[System.Boolean] isNotNakedRefCell
FSharp.Compiler.SyntaxTree+SynExpr+CompExpr: SynExpr expr
FSharp.Compiler.SyntaxTree+SynExpr+CompExpr: SynExpr get_expr()
FSharp.Compiler.SyntaxTree+SynExpr+Const: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+Const: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+Const: SynConst constant
FSharp.Compiler.SyntaxTree+SynExpr+Const: SynConst get_constant()
FSharp.Compiler.SyntaxTree+SynExpr+DiscardAfterMissingQualificationAfterDot: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+DiscardAfterMissingQualificationAfterDot: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+DiscardAfterMissingQualificationAfterDot: SynExpr expr
FSharp.Compiler.SyntaxTree+SynExpr+DiscardAfterMissingQualificationAfterDot: SynExpr get_expr()
FSharp.Compiler.SyntaxTree+SynExpr+Do: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+Do: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+Do: SynExpr expr
FSharp.Compiler.SyntaxTree+SynExpr+Do: SynExpr get_expr()
FSharp.Compiler.SyntaxTree+SynExpr+DoBang: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+DoBang: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+DoBang: SynExpr expr
FSharp.Compiler.SyntaxTree+SynExpr+DoBang: SynExpr get_expr()
FSharp.Compiler.SyntaxTree+SynExpr+DotGet: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+DotGet: FSharp.Compiler.Text.Range get_rangeOfDot()
FSharp.Compiler.SyntaxTree+SynExpr+DotGet: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+DotGet: FSharp.Compiler.Text.Range rangeOfDot
FSharp.Compiler.SyntaxTree+SynExpr+DotGet: LongIdentWithDots get_longDotId()
FSharp.Compiler.SyntaxTree+SynExpr+DotGet: LongIdentWithDots longDotId
FSharp.Compiler.SyntaxTree+SynExpr+DotGet: SynExpr expr
FSharp.Compiler.SyntaxTree+SynExpr+DotGet: SynExpr get_expr()
FSharp.Compiler.SyntaxTree+SynExpr+DotIndexedGet: FSharp.Compiler.Text.Range dotRange
FSharp.Compiler.SyntaxTree+SynExpr+DotIndexedGet: FSharp.Compiler.Text.Range get_dotRange()
FSharp.Compiler.SyntaxTree+SynExpr+DotIndexedGet: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+DotIndexedGet: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+DotIndexedGet: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynIndexerArg] get_indexExprs()
FSharp.Compiler.SyntaxTree+SynExpr+DotIndexedGet: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynIndexerArg] indexExprs
FSharp.Compiler.SyntaxTree+SynExpr+DotIndexedGet: SynExpr get_objectExpr()
FSharp.Compiler.SyntaxTree+SynExpr+DotIndexedGet: SynExpr objectExpr
FSharp.Compiler.SyntaxTree+SynExpr+DotIndexedSet: FSharp.Compiler.Text.Range dotRange
FSharp.Compiler.SyntaxTree+SynExpr+DotIndexedSet: FSharp.Compiler.Text.Range get_dotRange()
FSharp.Compiler.SyntaxTree+SynExpr+DotIndexedSet: FSharp.Compiler.Text.Range get_leftOfSetRange()
FSharp.Compiler.SyntaxTree+SynExpr+DotIndexedSet: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+DotIndexedSet: FSharp.Compiler.Text.Range leftOfSetRange
FSharp.Compiler.SyntaxTree+SynExpr+DotIndexedSet: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+DotIndexedSet: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynIndexerArg] get_indexExprs()
FSharp.Compiler.SyntaxTree+SynExpr+DotIndexedSet: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynIndexerArg] indexExprs
FSharp.Compiler.SyntaxTree+SynExpr+DotIndexedSet: SynExpr get_objectExpr()
FSharp.Compiler.SyntaxTree+SynExpr+DotIndexedSet: SynExpr get_valueExpr()
FSharp.Compiler.SyntaxTree+SynExpr+DotIndexedSet: SynExpr objectExpr
FSharp.Compiler.SyntaxTree+SynExpr+DotIndexedSet: SynExpr valueExpr
FSharp.Compiler.SyntaxTree+SynExpr+DotNamedIndexedPropertySet: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+DotNamedIndexedPropertySet: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+DotNamedIndexedPropertySet: LongIdentWithDots get_longDotId()
FSharp.Compiler.SyntaxTree+SynExpr+DotNamedIndexedPropertySet: LongIdentWithDots longDotId
FSharp.Compiler.SyntaxTree+SynExpr+DotNamedIndexedPropertySet: SynExpr argExpr
FSharp.Compiler.SyntaxTree+SynExpr+DotNamedIndexedPropertySet: SynExpr get_argExpr()
FSharp.Compiler.SyntaxTree+SynExpr+DotNamedIndexedPropertySet: SynExpr get_rhsExpr()
FSharp.Compiler.SyntaxTree+SynExpr+DotNamedIndexedPropertySet: SynExpr get_targetExpr()
FSharp.Compiler.SyntaxTree+SynExpr+DotNamedIndexedPropertySet: SynExpr rhsExpr
FSharp.Compiler.SyntaxTree+SynExpr+DotNamedIndexedPropertySet: SynExpr targetExpr
FSharp.Compiler.SyntaxTree+SynExpr+DotSet: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+DotSet: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+DotSet: LongIdentWithDots get_longDotId()
FSharp.Compiler.SyntaxTree+SynExpr+DotSet: LongIdentWithDots longDotId
FSharp.Compiler.SyntaxTree+SynExpr+DotSet: SynExpr get_rhsExpr()
FSharp.Compiler.SyntaxTree+SynExpr+DotSet: SynExpr get_targetExpr()
FSharp.Compiler.SyntaxTree+SynExpr+DotSet: SynExpr rhsExpr
FSharp.Compiler.SyntaxTree+SynExpr+DotSet: SynExpr targetExpr
FSharp.Compiler.SyntaxTree+SynExpr+Downcast: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+Downcast: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+Downcast: SynExpr expr
FSharp.Compiler.SyntaxTree+SynExpr+Downcast: SynExpr get_expr()
FSharp.Compiler.SyntaxTree+SynExpr+Downcast: SynType get_targetType()
FSharp.Compiler.SyntaxTree+SynExpr+Downcast: SynType targetType
FSharp.Compiler.SyntaxTree+SynExpr+Fixed: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+Fixed: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+Fixed: SynExpr expr
FSharp.Compiler.SyntaxTree+SynExpr+Fixed: SynExpr get_expr()
FSharp.Compiler.SyntaxTree+SynExpr+For: Boolean direction
FSharp.Compiler.SyntaxTree+SynExpr+For: Boolean get_direction()
FSharp.Compiler.SyntaxTree+SynExpr+For: DebugPointAtFor forSeqPoint
FSharp.Compiler.SyntaxTree+SynExpr+For: DebugPointAtFor get_forSeqPoint()
FSharp.Compiler.SyntaxTree+SynExpr+For: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+For: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+For: Ident get_ident()
FSharp.Compiler.SyntaxTree+SynExpr+For: Ident ident
FSharp.Compiler.SyntaxTree+SynExpr+For: SynExpr doBody
FSharp.Compiler.SyntaxTree+SynExpr+For: SynExpr get_doBody()
FSharp.Compiler.SyntaxTree+SynExpr+For: SynExpr get_identBody()
FSharp.Compiler.SyntaxTree+SynExpr+For: SynExpr get_toBody()
FSharp.Compiler.SyntaxTree+SynExpr+For: SynExpr identBody
FSharp.Compiler.SyntaxTree+SynExpr+For: SynExpr toBody
FSharp.Compiler.SyntaxTree+SynExpr+ForEach: Boolean get_isFromSource()
FSharp.Compiler.SyntaxTree+SynExpr+ForEach: Boolean isFromSource
FSharp.Compiler.SyntaxTree+SynExpr+ForEach: DebugPointAtFor forSeqPoint
FSharp.Compiler.SyntaxTree+SynExpr+ForEach: DebugPointAtFor get_forSeqPoint()
FSharp.Compiler.SyntaxTree+SynExpr+ForEach: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+ForEach: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+ForEach: SeqExprOnly get_seqExprOnly()
FSharp.Compiler.SyntaxTree+SynExpr+ForEach: SeqExprOnly seqExprOnly
FSharp.Compiler.SyntaxTree+SynExpr+ForEach: SynExpr bodyExpr
FSharp.Compiler.SyntaxTree+SynExpr+ForEach: SynExpr enumExpr
FSharp.Compiler.SyntaxTree+SynExpr+ForEach: SynExpr get_bodyExpr()
FSharp.Compiler.SyntaxTree+SynExpr+ForEach: SynExpr get_enumExpr()
FSharp.Compiler.SyntaxTree+SynExpr+ForEach: SynPat get_pat()
FSharp.Compiler.SyntaxTree+SynExpr+ForEach: SynPat pat
FSharp.Compiler.SyntaxTree+SynExpr+FromParseError: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+FromParseError: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+FromParseError: SynExpr expr
FSharp.Compiler.SyntaxTree+SynExpr+FromParseError: SynExpr get_expr()
FSharp.Compiler.SyntaxTree+SynExpr+Ident: Ident get_ident()
FSharp.Compiler.SyntaxTree+SynExpr+Ident: Ident ident
FSharp.Compiler.SyntaxTree+SynExpr+IfThenElse: Boolean get_isFromErrorRecovery()
FSharp.Compiler.SyntaxTree+SynExpr+IfThenElse: Boolean isFromErrorRecovery
FSharp.Compiler.SyntaxTree+SynExpr+IfThenElse: DebugPointForBinding get_spIfToThen()
FSharp.Compiler.SyntaxTree+SynExpr+IfThenElse: DebugPointForBinding spIfToThen
FSharp.Compiler.SyntaxTree+SynExpr+IfThenElse: FSharp.Compiler.Text.Range get_ifToThenRange()
FSharp.Compiler.SyntaxTree+SynExpr+IfThenElse: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+IfThenElse: FSharp.Compiler.Text.Range ifToThenRange
FSharp.Compiler.SyntaxTree+SynExpr+IfThenElse: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+IfThenElse: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynExpr] elseExpr
FSharp.Compiler.SyntaxTree+SynExpr+IfThenElse: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynExpr] get_elseExpr()
FSharp.Compiler.SyntaxTree+SynExpr+IfThenElse: SynExpr get_ifExpr()
FSharp.Compiler.SyntaxTree+SynExpr+IfThenElse: SynExpr get_thenExpr()
FSharp.Compiler.SyntaxTree+SynExpr+IfThenElse: SynExpr ifExpr
FSharp.Compiler.SyntaxTree+SynExpr+IfThenElse: SynExpr thenExpr
FSharp.Compiler.SyntaxTree+SynExpr+ImplicitZero: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+ImplicitZero: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+InferredDowncast: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+InferredDowncast: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+InferredDowncast: SynExpr expr
FSharp.Compiler.SyntaxTree+SynExpr+InferredDowncast: SynExpr get_expr()
FSharp.Compiler.SyntaxTree+SynExpr+InferredUpcast: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+InferredUpcast: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+InferredUpcast: SynExpr expr
FSharp.Compiler.SyntaxTree+SynExpr+InferredUpcast: SynExpr get_expr()
FSharp.Compiler.SyntaxTree+SynExpr+InterpolatedString: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+InterpolatedString: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+InterpolatedString: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynInterpolatedStringPart] contents
FSharp.Compiler.SyntaxTree+SynExpr+InterpolatedString: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynInterpolatedStringPart] get_contents()
FSharp.Compiler.SyntaxTree+SynExpr+JoinIn: FSharp.Compiler.Text.Range get_lhsRange()
FSharp.Compiler.SyntaxTree+SynExpr+JoinIn: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+JoinIn: FSharp.Compiler.Text.Range lhsRange
FSharp.Compiler.SyntaxTree+SynExpr+JoinIn: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+JoinIn: SynExpr get_lhsExpr()
FSharp.Compiler.SyntaxTree+SynExpr+JoinIn: SynExpr get_rhsExpr()
FSharp.Compiler.SyntaxTree+SynExpr+JoinIn: SynExpr lhsExpr
FSharp.Compiler.SyntaxTree+SynExpr+JoinIn: SynExpr rhsExpr
FSharp.Compiler.SyntaxTree+SynExpr+Lambda: Boolean fromMethod
FSharp.Compiler.SyntaxTree+SynExpr+Lambda: Boolean get_fromMethod()
FSharp.Compiler.SyntaxTree+SynExpr+Lambda: Boolean get_inLambdaSeq()
FSharp.Compiler.SyntaxTree+SynExpr+Lambda: Boolean inLambdaSeq
FSharp.Compiler.SyntaxTree+SynExpr+Lambda: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+Lambda: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+Lambda: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynPat],FSharp.Compiler.SyntaxTree+SynExpr]] get_parsedData()
FSharp.Compiler.SyntaxTree+SynExpr+Lambda: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynPat],FSharp.Compiler.SyntaxTree+SynExpr]] parsedData
FSharp.Compiler.SyntaxTree+SynExpr+Lambda: SynExpr body
FSharp.Compiler.SyntaxTree+SynExpr+Lambda: SynExpr get_body()
FSharp.Compiler.SyntaxTree+SynExpr+Lambda: SynSimplePats args
FSharp.Compiler.SyntaxTree+SynExpr+Lambda: SynSimplePats get_args()
FSharp.Compiler.SyntaxTree+SynExpr+Lazy: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+Lazy: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+Lazy: SynExpr expr
FSharp.Compiler.SyntaxTree+SynExpr+Lazy: SynExpr get_expr()
FSharp.Compiler.SyntaxTree+SynExpr+LetOrUse: Boolean get_isRecursive()
FSharp.Compiler.SyntaxTree+SynExpr+LetOrUse: Boolean get_isUse()
FSharp.Compiler.SyntaxTree+SynExpr+LetOrUse: Boolean isRecursive
FSharp.Compiler.SyntaxTree+SynExpr+LetOrUse: Boolean isUse
FSharp.Compiler.SyntaxTree+SynExpr+LetOrUse: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+LetOrUse: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+LetOrUse: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynBinding] bindings
FSharp.Compiler.SyntaxTree+SynExpr+LetOrUse: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynBinding] get_bindings()
FSharp.Compiler.SyntaxTree+SynExpr+LetOrUse: SynExpr body
FSharp.Compiler.SyntaxTree+SynExpr+LetOrUse: SynExpr get_body()
FSharp.Compiler.SyntaxTree+SynExpr+LetOrUseBang: Boolean get_isFromSource()
FSharp.Compiler.SyntaxTree+SynExpr+LetOrUseBang: Boolean get_isUse()
FSharp.Compiler.SyntaxTree+SynExpr+LetOrUseBang: Boolean isFromSource
FSharp.Compiler.SyntaxTree+SynExpr+LetOrUseBang: Boolean isUse
FSharp.Compiler.SyntaxTree+SynExpr+LetOrUseBang: DebugPointForBinding bindSeqPoint
FSharp.Compiler.SyntaxTree+SynExpr+LetOrUseBang: DebugPointForBinding get_bindSeqPoint()
FSharp.Compiler.SyntaxTree+SynExpr+LetOrUseBang: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+LetOrUseBang: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+LetOrUseBang: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`6[FSharp.Compiler.SyntaxTree+DebugPointForBinding,System.Boolean,System.Boolean,FSharp.Compiler.SyntaxTree+SynPat,FSharp.Compiler.SyntaxTree+SynExpr,FSharp.Compiler.Text.Range]] andBangs
FSharp.Compiler.SyntaxTree+SynExpr+LetOrUseBang: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`6[FSharp.Compiler.SyntaxTree+DebugPointForBinding,System.Boolean,System.Boolean,FSharp.Compiler.SyntaxTree+SynPat,FSharp.Compiler.SyntaxTree+SynExpr,FSharp.Compiler.Text.Range]] get_andBangs()
FSharp.Compiler.SyntaxTree+SynExpr+LetOrUseBang: SynExpr body
FSharp.Compiler.SyntaxTree+SynExpr+LetOrUseBang: SynExpr get_body()
FSharp.Compiler.SyntaxTree+SynExpr+LetOrUseBang: SynExpr get_rhs()
FSharp.Compiler.SyntaxTree+SynExpr+LetOrUseBang: SynExpr rhs
FSharp.Compiler.SyntaxTree+SynExpr+LetOrUseBang: SynPat get_pat()
FSharp.Compiler.SyntaxTree+SynExpr+LetOrUseBang: SynPat pat
FSharp.Compiler.SyntaxTree+SynExpr+LibraryOnlyILAssembly: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+LibraryOnlyILAssembly: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+LibraryOnlyILAssembly: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynExpr] args
FSharp.Compiler.SyntaxTree+SynExpr+LibraryOnlyILAssembly: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynExpr] get_args()
FSharp.Compiler.SyntaxTree+SynExpr+LibraryOnlyILAssembly: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynType] get_retTy()
FSharp.Compiler.SyntaxTree+SynExpr+LibraryOnlyILAssembly: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynType] get_typeArgs()
FSharp.Compiler.SyntaxTree+SynExpr+LibraryOnlyILAssembly: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynType] retTy
FSharp.Compiler.SyntaxTree+SynExpr+LibraryOnlyILAssembly: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynType] typeArgs
FSharp.Compiler.SyntaxTree+SynExpr+LibraryOnlyILAssembly: System.Object get_ilCode()
FSharp.Compiler.SyntaxTree+SynExpr+LibraryOnlyILAssembly: System.Object ilCode
FSharp.Compiler.SyntaxTree+SynExpr+LibraryOnlyStaticOptimization: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+LibraryOnlyStaticOptimization: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+LibraryOnlyStaticOptimization: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynStaticOptimizationConstraint] constraints
FSharp.Compiler.SyntaxTree+SynExpr+LibraryOnlyStaticOptimization: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynStaticOptimizationConstraint] get_constraints()
FSharp.Compiler.SyntaxTree+SynExpr+LibraryOnlyStaticOptimization: SynExpr expr
FSharp.Compiler.SyntaxTree+SynExpr+LibraryOnlyStaticOptimization: SynExpr get_expr()
FSharp.Compiler.SyntaxTree+SynExpr+LibraryOnlyStaticOptimization: SynExpr get_optimizedExpr()
FSharp.Compiler.SyntaxTree+SynExpr+LibraryOnlyStaticOptimization: SynExpr optimizedExpr
FSharp.Compiler.SyntaxTree+SynExpr+LibraryOnlyUnionCaseFieldGet: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+LibraryOnlyUnionCaseFieldGet: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+LibraryOnlyUnionCaseFieldGet: Int32 fieldNum
FSharp.Compiler.SyntaxTree+SynExpr+LibraryOnlyUnionCaseFieldGet: Int32 get_fieldNum()
FSharp.Compiler.SyntaxTree+SynExpr+LibraryOnlyUnionCaseFieldGet: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident] get_longId()
FSharp.Compiler.SyntaxTree+SynExpr+LibraryOnlyUnionCaseFieldGet: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident] longId
FSharp.Compiler.SyntaxTree+SynExpr+LibraryOnlyUnionCaseFieldGet: SynExpr expr
FSharp.Compiler.SyntaxTree+SynExpr+LibraryOnlyUnionCaseFieldGet: SynExpr get_expr()
FSharp.Compiler.SyntaxTree+SynExpr+LibraryOnlyUnionCaseFieldSet: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+LibraryOnlyUnionCaseFieldSet: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+LibraryOnlyUnionCaseFieldSet: Int32 fieldNum
FSharp.Compiler.SyntaxTree+SynExpr+LibraryOnlyUnionCaseFieldSet: Int32 get_fieldNum()
FSharp.Compiler.SyntaxTree+SynExpr+LibraryOnlyUnionCaseFieldSet: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident] get_longId()
FSharp.Compiler.SyntaxTree+SynExpr+LibraryOnlyUnionCaseFieldSet: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident] longId
FSharp.Compiler.SyntaxTree+SynExpr+LibraryOnlyUnionCaseFieldSet: SynExpr expr
FSharp.Compiler.SyntaxTree+SynExpr+LibraryOnlyUnionCaseFieldSet: SynExpr get_expr()
FSharp.Compiler.SyntaxTree+SynExpr+LibraryOnlyUnionCaseFieldSet: SynExpr get_rhsExpr()
FSharp.Compiler.SyntaxTree+SynExpr+LibraryOnlyUnionCaseFieldSet: SynExpr rhsExpr
FSharp.Compiler.SyntaxTree+SynExpr+LongIdent: Boolean get_isOptional()
FSharp.Compiler.SyntaxTree+SynExpr+LongIdent: Boolean isOptional
FSharp.Compiler.SyntaxTree+SynExpr+LongIdent: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+LongIdent: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+LongIdent: LongIdentWithDots get_longDotId()
FSharp.Compiler.SyntaxTree+SynExpr+LongIdent: LongIdentWithDots longDotId
FSharp.Compiler.SyntaxTree+SynExpr+LongIdent: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.FSharpRef`1[FSharp.Compiler.SyntaxTree+SynSimplePatAlternativeIdInfo]] altNameRefCell
FSharp.Compiler.SyntaxTree+SynExpr+LongIdent: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.FSharpRef`1[FSharp.Compiler.SyntaxTree+SynSimplePatAlternativeIdInfo]] get_altNameRefCell()
FSharp.Compiler.SyntaxTree+SynExpr+LongIdentSet: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+LongIdentSet: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+LongIdentSet: LongIdentWithDots get_longDotId()
FSharp.Compiler.SyntaxTree+SynExpr+LongIdentSet: LongIdentWithDots longDotId
FSharp.Compiler.SyntaxTree+SynExpr+LongIdentSet: SynExpr expr
FSharp.Compiler.SyntaxTree+SynExpr+LongIdentSet: SynExpr get_expr()
FSharp.Compiler.SyntaxTree+SynExpr+Match: DebugPointForBinding get_matchSeqPoint()
FSharp.Compiler.SyntaxTree+SynExpr+Match: DebugPointForBinding matchSeqPoint
FSharp.Compiler.SyntaxTree+SynExpr+Match: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+Match: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+Match: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynMatchClause] clauses
FSharp.Compiler.SyntaxTree+SynExpr+Match: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynMatchClause] get_clauses()
FSharp.Compiler.SyntaxTree+SynExpr+Match: SynExpr expr
FSharp.Compiler.SyntaxTree+SynExpr+Match: SynExpr get_expr()
FSharp.Compiler.SyntaxTree+SynExpr+MatchBang: DebugPointForBinding get_matchSeqPoint()
FSharp.Compiler.SyntaxTree+SynExpr+MatchBang: DebugPointForBinding matchSeqPoint
FSharp.Compiler.SyntaxTree+SynExpr+MatchBang: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+MatchBang: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+MatchBang: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynMatchClause] clauses
FSharp.Compiler.SyntaxTree+SynExpr+MatchBang: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynMatchClause] get_clauses()
FSharp.Compiler.SyntaxTree+SynExpr+MatchBang: SynExpr expr
FSharp.Compiler.SyntaxTree+SynExpr+MatchBang: SynExpr get_expr()
FSharp.Compiler.SyntaxTree+SynExpr+MatchLambda: Boolean get_isExnMatch()
FSharp.Compiler.SyntaxTree+SynExpr+MatchLambda: Boolean isExnMatch
FSharp.Compiler.SyntaxTree+SynExpr+MatchLambda: DebugPointForBinding get_matchSeqPoint()
FSharp.Compiler.SyntaxTree+SynExpr+MatchLambda: DebugPointForBinding matchSeqPoint
FSharp.Compiler.SyntaxTree+SynExpr+MatchLambda: FSharp.Compiler.Text.Range get_keywordRange()
FSharp.Compiler.SyntaxTree+SynExpr+MatchLambda: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+MatchLambda: FSharp.Compiler.Text.Range keywordRange
FSharp.Compiler.SyntaxTree+SynExpr+MatchLambda: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+MatchLambda: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynMatchClause] get_matchClauses()
FSharp.Compiler.SyntaxTree+SynExpr+MatchLambda: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynMatchClause] matchClauses
FSharp.Compiler.SyntaxTree+SynExpr+NamedIndexedPropertySet: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+NamedIndexedPropertySet: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+NamedIndexedPropertySet: LongIdentWithDots get_longDotId()
FSharp.Compiler.SyntaxTree+SynExpr+NamedIndexedPropertySet: LongIdentWithDots longDotId
FSharp.Compiler.SyntaxTree+SynExpr+NamedIndexedPropertySet: SynExpr expr1
FSharp.Compiler.SyntaxTree+SynExpr+NamedIndexedPropertySet: SynExpr expr2
FSharp.Compiler.SyntaxTree+SynExpr+NamedIndexedPropertySet: SynExpr get_expr1()
FSharp.Compiler.SyntaxTree+SynExpr+NamedIndexedPropertySet: SynExpr get_expr2()
FSharp.Compiler.SyntaxTree+SynExpr+New: Boolean get_isProtected()
FSharp.Compiler.SyntaxTree+SynExpr+New: Boolean isProtected
FSharp.Compiler.SyntaxTree+SynExpr+New: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+New: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+New: SynExpr expr
FSharp.Compiler.SyntaxTree+SynExpr+New: SynExpr get_expr()
FSharp.Compiler.SyntaxTree+SynExpr+New: SynType get_targetType()
FSharp.Compiler.SyntaxTree+SynExpr+New: SynType targetType
FSharp.Compiler.SyntaxTree+SynExpr+Null: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+Null: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+ObjExpr: FSharp.Compiler.Text.Range get_newExprRange()
FSharp.Compiler.SyntaxTree+SynExpr+ObjExpr: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+ObjExpr: FSharp.Compiler.Text.Range newExprRange
FSharp.Compiler.SyntaxTree+SynExpr+ObjExpr: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+ObjExpr: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynBinding] bindings
FSharp.Compiler.SyntaxTree+SynExpr+ObjExpr: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynBinding] get_bindings()
FSharp.Compiler.SyntaxTree+SynExpr+ObjExpr: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynInterfaceImpl] extraImpls
FSharp.Compiler.SyntaxTree+SynExpr+ObjExpr: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynInterfaceImpl] get_extraImpls()
FSharp.Compiler.SyntaxTree+SynExpr+ObjExpr: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.SyntaxTree+SynExpr,Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+Ident]]] argOptions
FSharp.Compiler.SyntaxTree+SynExpr+ObjExpr: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.SyntaxTree+SynExpr,Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+Ident]]] get_argOptions()
FSharp.Compiler.SyntaxTree+SynExpr+ObjExpr: SynType get_objType()
FSharp.Compiler.SyntaxTree+SynExpr+ObjExpr: SynType objType
FSharp.Compiler.SyntaxTree+SynExpr+Paren: FSharp.Compiler.Text.Range get_leftParenRange()
FSharp.Compiler.SyntaxTree+SynExpr+Paren: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+Paren: FSharp.Compiler.Text.Range leftParenRange
FSharp.Compiler.SyntaxTree+SynExpr+Paren: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+Paren: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] get_rightParenRange()
FSharp.Compiler.SyntaxTree+SynExpr+Paren: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] rightParenRange
FSharp.Compiler.SyntaxTree+SynExpr+Paren: SynExpr expr
FSharp.Compiler.SyntaxTree+SynExpr+Paren: SynExpr get_expr()
FSharp.Compiler.SyntaxTree+SynExpr+Quote: Boolean get_isFromQueryExpression()
FSharp.Compiler.SyntaxTree+SynExpr+Quote: Boolean get_isRaw()
FSharp.Compiler.SyntaxTree+SynExpr+Quote: Boolean isFromQueryExpression
FSharp.Compiler.SyntaxTree+SynExpr+Quote: Boolean isRaw
FSharp.Compiler.SyntaxTree+SynExpr+Quote: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+Quote: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+Quote: SynExpr get_operator()
FSharp.Compiler.SyntaxTree+SynExpr+Quote: SynExpr get_quotedExpr()
FSharp.Compiler.SyntaxTree+SynExpr+Quote: SynExpr operator
FSharp.Compiler.SyntaxTree+SynExpr+Quote: SynExpr quotedExpr
FSharp.Compiler.SyntaxTree+SynExpr+Record: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+Record: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+Record: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`3[System.Tuple`2[FSharp.Compiler.SyntaxTree+LongIdentWithDots,System.Boolean],Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynExpr],Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.Text.Range,Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Pos]]]]] get_recordFields()
FSharp.Compiler.SyntaxTree+SynExpr+Record: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`3[System.Tuple`2[FSharp.Compiler.SyntaxTree+LongIdentWithDots,System.Boolean],Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynExpr],Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.Text.Range,Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Pos]]]]] recordFields
FSharp.Compiler.SyntaxTree+SynExpr+Record: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.SyntaxTree+SynExpr,System.Tuple`2[FSharp.Compiler.Text.Range,Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Pos]]]] copyInfo
FSharp.Compiler.SyntaxTree+SynExpr+Record: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.SyntaxTree+SynExpr,System.Tuple`2[FSharp.Compiler.Text.Range,Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Pos]]]] get_copyInfo()
FSharp.Compiler.SyntaxTree+SynExpr+Record: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`5[FSharp.Compiler.SyntaxTree+SynType,FSharp.Compiler.SyntaxTree+SynExpr,FSharp.Compiler.Text.Range,Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.Text.Range,Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Pos]]],FSharp.Compiler.Text.Range]] baseInfo
FSharp.Compiler.SyntaxTree+SynExpr+Record: Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`5[FSharp.Compiler.SyntaxTree+SynType,FSharp.Compiler.SyntaxTree+SynExpr,FSharp.Compiler.Text.Range,Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.Text.Range,Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Pos]]],FSharp.Compiler.Text.Range]] get_baseInfo()
FSharp.Compiler.SyntaxTree+SynExpr+Sequential: Boolean get_isTrueSeq()
FSharp.Compiler.SyntaxTree+SynExpr+Sequential: Boolean isTrueSeq
FSharp.Compiler.SyntaxTree+SynExpr+Sequential: DebugPointAtSequential get_seqPoint()
FSharp.Compiler.SyntaxTree+SynExpr+Sequential: DebugPointAtSequential seqPoint
FSharp.Compiler.SyntaxTree+SynExpr+Sequential: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+Sequential: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+Sequential: SynExpr expr1
FSharp.Compiler.SyntaxTree+SynExpr+Sequential: SynExpr expr2
FSharp.Compiler.SyntaxTree+SynExpr+Sequential: SynExpr get_expr1()
FSharp.Compiler.SyntaxTree+SynExpr+Sequential: SynExpr get_expr2()
FSharp.Compiler.SyntaxTree+SynExpr+SequentialOrImplicitYield: DebugPointAtSequential get_seqPoint()
FSharp.Compiler.SyntaxTree+SynExpr+SequentialOrImplicitYield: DebugPointAtSequential seqPoint
FSharp.Compiler.SyntaxTree+SynExpr+SequentialOrImplicitYield: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+SequentialOrImplicitYield: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+SequentialOrImplicitYield: SynExpr expr1
FSharp.Compiler.SyntaxTree+SynExpr+SequentialOrImplicitYield: SynExpr expr2
FSharp.Compiler.SyntaxTree+SynExpr+SequentialOrImplicitYield: SynExpr get_expr1()
FSharp.Compiler.SyntaxTree+SynExpr+SequentialOrImplicitYield: SynExpr get_expr2()
FSharp.Compiler.SyntaxTree+SynExpr+SequentialOrImplicitYield: SynExpr get_ifNotStmt()
FSharp.Compiler.SyntaxTree+SynExpr+SequentialOrImplicitYield: SynExpr ifNotStmt
FSharp.Compiler.SyntaxTree+SynExpr+Set: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+Set: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+Set: SynExpr get_rhsExpr()
FSharp.Compiler.SyntaxTree+SynExpr+Set: SynExpr get_targetExpr()
FSharp.Compiler.SyntaxTree+SynExpr+Set: SynExpr rhsExpr
FSharp.Compiler.SyntaxTree+SynExpr+Set: SynExpr targetExpr
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 AddressOf
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 AnonRecd
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 App
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 ArbitraryAfterError
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 ArrayOrList
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 ArrayOrListOfSeqExpr
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 Assert
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 CompExpr
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 Const
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 DiscardAfterMissingQualificationAfterDot
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 Do
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 DoBang
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 DotGet
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 DotIndexedGet
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 DotIndexedSet
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 DotNamedIndexedPropertySet
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 DotSet
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 Downcast
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 Fixed
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 For
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 ForEach
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 FromParseError
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 Ident
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 IfThenElse
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 ImplicitZero
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 InferredDowncast
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 InferredUpcast
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 InterpolatedString
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 JoinIn
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 Lambda
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 Lazy
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 LetOrUse
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 LetOrUseBang
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 LibraryOnlyILAssembly
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 LibraryOnlyStaticOptimization
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 LibraryOnlyUnionCaseFieldGet
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 LibraryOnlyUnionCaseFieldSet
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 LongIdent
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 LongIdentSet
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 Match
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 MatchBang
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 MatchLambda
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 NamedIndexedPropertySet
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 New
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 Null
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 ObjExpr
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 Paren
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 Quote
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 Record
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 Sequential
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 SequentialOrImplicitYield
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 Set
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 TraitCall
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 TryFinally
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 TryWith
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 Tuple
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 TypeApp
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 TypeTest
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 Typed
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 Upcast
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 While
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 YieldOrReturn
FSharp.Compiler.SyntaxTree+SynExpr+Tags: Int32 YieldOrReturnFrom
FSharp.Compiler.SyntaxTree+SynExpr+TraitCall: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+TraitCall: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+TraitCall: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynTypar] get_supportTys()
FSharp.Compiler.SyntaxTree+SynExpr+TraitCall: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynTypar] supportTys
FSharp.Compiler.SyntaxTree+SynExpr+TraitCall: SynExpr argExpr
FSharp.Compiler.SyntaxTree+SynExpr+TraitCall: SynExpr get_argExpr()
FSharp.Compiler.SyntaxTree+SynExpr+TraitCall: SynMemberSig get_traitSig()
FSharp.Compiler.SyntaxTree+SynExpr+TraitCall: SynMemberSig traitSig
FSharp.Compiler.SyntaxTree+SynExpr+TryFinally: DebugPointAtFinally finallySeqPoint
FSharp.Compiler.SyntaxTree+SynExpr+TryFinally: DebugPointAtFinally get_finallySeqPoint()
FSharp.Compiler.SyntaxTree+SynExpr+TryFinally: DebugPointAtTry get_trySeqPoint()
FSharp.Compiler.SyntaxTree+SynExpr+TryFinally: DebugPointAtTry trySeqPoint
FSharp.Compiler.SyntaxTree+SynExpr+TryFinally: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+TryFinally: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+TryFinally: SynExpr finallyExpr
FSharp.Compiler.SyntaxTree+SynExpr+TryFinally: SynExpr get_finallyExpr()
FSharp.Compiler.SyntaxTree+SynExpr+TryFinally: SynExpr get_tryExpr()
FSharp.Compiler.SyntaxTree+SynExpr+TryFinally: SynExpr tryExpr
FSharp.Compiler.SyntaxTree+SynExpr+TryWith: DebugPointAtTry get_trySeqPoint()
FSharp.Compiler.SyntaxTree+SynExpr+TryWith: DebugPointAtTry trySeqPoint
FSharp.Compiler.SyntaxTree+SynExpr+TryWith: DebugPointAtWith get_withSeqPoint()
FSharp.Compiler.SyntaxTree+SynExpr+TryWith: DebugPointAtWith withSeqPoint
FSharp.Compiler.SyntaxTree+SynExpr+TryWith: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+TryWith: FSharp.Compiler.Text.Range get_tryRange()
FSharp.Compiler.SyntaxTree+SynExpr+TryWith: FSharp.Compiler.Text.Range get_withRange()
FSharp.Compiler.SyntaxTree+SynExpr+TryWith: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+TryWith: FSharp.Compiler.Text.Range tryRange
FSharp.Compiler.SyntaxTree+SynExpr+TryWith: FSharp.Compiler.Text.Range withRange
FSharp.Compiler.SyntaxTree+SynExpr+TryWith: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynMatchClause] get_withCases()
FSharp.Compiler.SyntaxTree+SynExpr+TryWith: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynMatchClause] withCases
FSharp.Compiler.SyntaxTree+SynExpr+TryWith: SynExpr get_tryExpr()
FSharp.Compiler.SyntaxTree+SynExpr+TryWith: SynExpr tryExpr
FSharp.Compiler.SyntaxTree+SynExpr+Tuple: Boolean get_isStruct()
FSharp.Compiler.SyntaxTree+SynExpr+Tuple: Boolean isStruct
FSharp.Compiler.SyntaxTree+SynExpr+Tuple: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+Tuple: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+Tuple: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynExpr] exprs
FSharp.Compiler.SyntaxTree+SynExpr+Tuple: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynExpr] get_exprs()
FSharp.Compiler.SyntaxTree+SynExpr+Tuple: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Text.Range] commaRanges
FSharp.Compiler.SyntaxTree+SynExpr+Tuple: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Text.Range] get_commaRanges()
FSharp.Compiler.SyntaxTree+SynExpr+TypeApp: FSharp.Compiler.Text.Range get_lessRange()
FSharp.Compiler.SyntaxTree+SynExpr+TypeApp: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+TypeApp: FSharp.Compiler.Text.Range get_typeArgsRange()
FSharp.Compiler.SyntaxTree+SynExpr+TypeApp: FSharp.Compiler.Text.Range lessRange
FSharp.Compiler.SyntaxTree+SynExpr+TypeApp: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+TypeApp: FSharp.Compiler.Text.Range typeArgsRange
FSharp.Compiler.SyntaxTree+SynExpr+TypeApp: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynType] get_typeArgs()
FSharp.Compiler.SyntaxTree+SynExpr+TypeApp: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynType] typeArgs
FSharp.Compiler.SyntaxTree+SynExpr+TypeApp: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Text.Range] commaRanges
FSharp.Compiler.SyntaxTree+SynExpr+TypeApp: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Text.Range] get_commaRanges()
FSharp.Compiler.SyntaxTree+SynExpr+TypeApp: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] get_greaterRange()
FSharp.Compiler.SyntaxTree+SynExpr+TypeApp: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] greaterRange
FSharp.Compiler.SyntaxTree+SynExpr+TypeApp: SynExpr expr
FSharp.Compiler.SyntaxTree+SynExpr+TypeApp: SynExpr get_expr()
FSharp.Compiler.SyntaxTree+SynExpr+TypeTest: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+TypeTest: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+TypeTest: SynExpr expr
FSharp.Compiler.SyntaxTree+SynExpr+TypeTest: SynExpr get_expr()
FSharp.Compiler.SyntaxTree+SynExpr+TypeTest: SynType get_targetType()
FSharp.Compiler.SyntaxTree+SynExpr+TypeTest: SynType targetType
FSharp.Compiler.SyntaxTree+SynExpr+Typed: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+Typed: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+Typed: SynExpr expr
FSharp.Compiler.SyntaxTree+SynExpr+Typed: SynExpr get_expr()
FSharp.Compiler.SyntaxTree+SynExpr+Typed: SynType get_targetType()
FSharp.Compiler.SyntaxTree+SynExpr+Typed: SynType targetType
FSharp.Compiler.SyntaxTree+SynExpr+Upcast: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+Upcast: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+Upcast: SynExpr expr
FSharp.Compiler.SyntaxTree+SynExpr+Upcast: SynExpr get_expr()
FSharp.Compiler.SyntaxTree+SynExpr+Upcast: SynType get_targetType()
FSharp.Compiler.SyntaxTree+SynExpr+Upcast: SynType targetType
FSharp.Compiler.SyntaxTree+SynExpr+While: DebugPointAtWhile get_whileSeqPoint()
FSharp.Compiler.SyntaxTree+SynExpr+While: DebugPointAtWhile whileSeqPoint
FSharp.Compiler.SyntaxTree+SynExpr+While: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+While: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+While: SynExpr doExpr
FSharp.Compiler.SyntaxTree+SynExpr+While: SynExpr get_doExpr()
FSharp.Compiler.SyntaxTree+SynExpr+While: SynExpr get_whileExpr()
FSharp.Compiler.SyntaxTree+SynExpr+While: SynExpr whileExpr
FSharp.Compiler.SyntaxTree+SynExpr+YieldOrReturn: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+YieldOrReturn: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+YieldOrReturn: SynExpr expr
FSharp.Compiler.SyntaxTree+SynExpr+YieldOrReturn: SynExpr get_expr()
FSharp.Compiler.SyntaxTree+SynExpr+YieldOrReturn: System.Tuple`2[System.Boolean,System.Boolean] flags
FSharp.Compiler.SyntaxTree+SynExpr+YieldOrReturn: System.Tuple`2[System.Boolean,System.Boolean] get_flags()
FSharp.Compiler.SyntaxTree+SynExpr+YieldOrReturnFrom: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynExpr+YieldOrReturnFrom: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynExpr+YieldOrReturnFrom: SynExpr expr
FSharp.Compiler.SyntaxTree+SynExpr+YieldOrReturnFrom: SynExpr get_expr()
FSharp.Compiler.SyntaxTree+SynExpr+YieldOrReturnFrom: System.Tuple`2[System.Boolean,System.Boolean] flags
FSharp.Compiler.SyntaxTree+SynExpr+YieldOrReturnFrom: System.Tuple`2[System.Boolean,System.Boolean] get_flags()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsAddressOf
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsAnonRecd
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsApp
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsArbExprAndThusAlreadyReportedError
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsArbitraryAfterError
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsArrayOrList
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsArrayOrListOfSeqExpr
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsAssert
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsCompExpr
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsConst
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsDiscardAfterMissingQualificationAfterDot
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsDo
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsDoBang
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsDotGet
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsDotIndexedGet
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsDotIndexedSet
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsDotNamedIndexedPropertySet
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsDotSet
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsDowncast
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsFixed
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsFor
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsForEach
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsFromParseError
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsIdent
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsIfThenElse
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsImplicitZero
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsInferredDowncast
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsInferredUpcast
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsInterpolatedString
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsJoinIn
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsLambda
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsLazy
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsLetOrUse
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsLetOrUseBang
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsLibraryOnlyILAssembly
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsLibraryOnlyStaticOptimization
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsLibraryOnlyUnionCaseFieldGet
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsLibraryOnlyUnionCaseFieldSet
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsLongIdent
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsLongIdentSet
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsMatch
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsMatchBang
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsMatchLambda
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsNamedIndexedPropertySet
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsNew
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsNull
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsObjExpr
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsParen
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsQuote
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsRecord
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsSequential
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsSequentialOrImplicitYield
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsSet
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsTraitCall
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsTryFinally
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsTryWith
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsTuple
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsTypeApp
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsTypeTest
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsTyped
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsUpcast
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsWhile
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsYieldOrReturn
FSharp.Compiler.SyntaxTree+SynExpr: Boolean IsYieldOrReturnFrom
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsAddressOf()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsAnonRecd()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsApp()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsArbExprAndThusAlreadyReportedError()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsArbitraryAfterError()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsArrayOrList()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsArrayOrListOfSeqExpr()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsAssert()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsCompExpr()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsConst()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsDiscardAfterMissingQualificationAfterDot()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsDo()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsDoBang()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsDotGet()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsDotIndexedGet()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsDotIndexedSet()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsDotNamedIndexedPropertySet()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsDotSet()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsDowncast()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsFixed()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsFor()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsForEach()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsFromParseError()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsIdent()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsIfThenElse()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsImplicitZero()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsInferredDowncast()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsInferredUpcast()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsInterpolatedString()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsJoinIn()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsLambda()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsLazy()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsLetOrUse()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsLetOrUseBang()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsLibraryOnlyILAssembly()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsLibraryOnlyStaticOptimization()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsLibraryOnlyUnionCaseFieldGet()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsLibraryOnlyUnionCaseFieldSet()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsLongIdent()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsLongIdentSet()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsMatch()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsMatchBang()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsMatchLambda()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsNamedIndexedPropertySet()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsNew()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsNull()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsObjExpr()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsParen()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsQuote()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsRecord()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsSequential()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsSequentialOrImplicitYield()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsSet()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsTraitCall()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsTryFinally()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsTryWith()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsTuple()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsTypeApp()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsTypeTest()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsTyped()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsUpcast()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsWhile()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsYieldOrReturn()
FSharp.Compiler.SyntaxTree+SynExpr: Boolean get_IsYieldOrReturnFrom()
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+AddressOf
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+AnonRecd
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+App
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+ArbitraryAfterError
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+ArrayOrList
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+ArrayOrListOfSeqExpr
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+Assert
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+CompExpr
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+Const
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+DiscardAfterMissingQualificationAfterDot
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+Do
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+DoBang
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+DotGet
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+DotIndexedGet
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+DotIndexedSet
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+DotNamedIndexedPropertySet
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+DotSet
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+Downcast
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+Fixed
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+For
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+ForEach
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+FromParseError
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+Ident
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+IfThenElse
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+ImplicitZero
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+InferredDowncast
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+InferredUpcast
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+InterpolatedString
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+JoinIn
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+Lambda
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+Lazy
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+LetOrUse
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+LetOrUseBang
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+LibraryOnlyILAssembly
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+LibraryOnlyStaticOptimization
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+LibraryOnlyUnionCaseFieldGet
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+LibraryOnlyUnionCaseFieldSet
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+LongIdent
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+LongIdentSet
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+Match
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+MatchBang
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+MatchLambda
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+NamedIndexedPropertySet
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+New
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+Null
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+ObjExpr
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+Paren
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+Quote
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+Record
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+Sequential
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+SequentialOrImplicitYield
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+Set
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+Tags
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+TraitCall
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+TryFinally
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+TryWith
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+Tuple
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+TypeApp
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+TypeTest
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+Typed
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+Upcast
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+While
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+YieldOrReturn
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.SyntaxTree+SynExpr+YieldOrReturnFrom
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.Text.Range Range
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.Text.Range RangeOfFirstPortion
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.Text.Range RangeSansAnyExtraDot
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.Text.Range get_RangeOfFirstPortion()
FSharp.Compiler.SyntaxTree+SynExpr: FSharp.Compiler.Text.Range get_RangeSansAnyExtraDot()
FSharp.Compiler.SyntaxTree+SynExpr: Int32 Tag
FSharp.Compiler.SyntaxTree+SynExpr: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewAddressOf(Boolean, SynExpr, FSharp.Compiler.Text.Range, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewAnonRecd(Boolean, Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.SyntaxTree+SynExpr,System.Tuple`2[FSharp.Compiler.Text.Range,Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Pos]]]], Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[FSharp.Compiler.SyntaxTree+Ident,FSharp.Compiler.SyntaxTree+SynExpr]], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewApp(ExprAtomicFlag, Boolean, SynExpr, SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewArbitraryAfterError(System.String, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewArrayOrList(Boolean, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynExpr], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewArrayOrListOfSeqExpr(Boolean, SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewAssert(SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewCompExpr(Boolean, Microsoft.FSharp.Core.FSharpRef`1[System.Boolean], SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewConst(SynConst, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewDiscardAfterMissingQualificationAfterDot(SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewDo(SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewDoBang(SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewDotGet(SynExpr, FSharp.Compiler.Text.Range, LongIdentWithDots, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewDotIndexedGet(SynExpr, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynIndexerArg], FSharp.Compiler.Text.Range, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewDotIndexedSet(SynExpr, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynIndexerArg], SynExpr, FSharp.Compiler.Text.Range, FSharp.Compiler.Text.Range, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewDotNamedIndexedPropertySet(SynExpr, LongIdentWithDots, SynExpr, SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewDotSet(SynExpr, LongIdentWithDots, SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewDowncast(SynExpr, SynType, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewFixed(SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewFor(DebugPointAtFor, Ident, SynExpr, Boolean, SynExpr, SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewForEach(DebugPointAtFor, SeqExprOnly, Boolean, SynPat, SynExpr, SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewFromParseError(SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewIdent(Ident)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewIfThenElse(SynExpr, SynExpr, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynExpr], DebugPointForBinding, Boolean, FSharp.Compiler.Text.Range, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewImplicitZero(FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewInferredDowncast(SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewInferredUpcast(SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewInterpolatedString(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynInterpolatedStringPart], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewJoinIn(SynExpr, FSharp.Compiler.Text.Range, SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewLambda(Boolean, Boolean, SynSimplePats, SynExpr, Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynPat],FSharp.Compiler.SyntaxTree+SynExpr]], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewLazy(SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewLetOrUse(Boolean, Boolean, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynBinding], SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewLetOrUseBang(DebugPointForBinding, Boolean, Boolean, SynPat, SynExpr, Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`6[FSharp.Compiler.SyntaxTree+DebugPointForBinding,System.Boolean,System.Boolean,FSharp.Compiler.SyntaxTree+SynPat,FSharp.Compiler.SyntaxTree+SynExpr,FSharp.Compiler.Text.Range]], SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewLibraryOnlyILAssembly(System.Object, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynType], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynExpr], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynType], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewLibraryOnlyStaticOptimization(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynStaticOptimizationConstraint], SynExpr, SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewLibraryOnlyUnionCaseFieldGet(SynExpr, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident], Int32, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewLibraryOnlyUnionCaseFieldSet(SynExpr, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident], Int32, SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewLongIdent(Boolean, LongIdentWithDots, Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.FSharpRef`1[FSharp.Compiler.SyntaxTree+SynSimplePatAlternativeIdInfo]], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewLongIdentSet(LongIdentWithDots, SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewMatch(DebugPointForBinding, SynExpr, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynMatchClause], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewMatchBang(DebugPointForBinding, SynExpr, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynMatchClause], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewMatchLambda(Boolean, FSharp.Compiler.Text.Range, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynMatchClause], DebugPointForBinding, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewNamedIndexedPropertySet(LongIdentWithDots, SynExpr, SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewNew(Boolean, SynType, SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewNull(FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewObjExpr(SynType, Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.SyntaxTree+SynExpr,Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+Ident]]], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynBinding], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynInterfaceImpl], FSharp.Compiler.Text.Range, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewParen(SynExpr, FSharp.Compiler.Text.Range, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewQuote(SynExpr, Boolean, SynExpr, Boolean, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewRecord(Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`5[FSharp.Compiler.SyntaxTree+SynType,FSharp.Compiler.SyntaxTree+SynExpr,FSharp.Compiler.Text.Range,Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.Text.Range,Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Pos]]],FSharp.Compiler.Text.Range]], Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.SyntaxTree+SynExpr,System.Tuple`2[FSharp.Compiler.Text.Range,Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Pos]]]], Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`3[System.Tuple`2[FSharp.Compiler.SyntaxTree+LongIdentWithDots,System.Boolean],Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynExpr],Microsoft.FSharp.Core.FSharpOption`1[System.Tuple`2[FSharp.Compiler.Text.Range,Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Pos]]]]], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewSequential(DebugPointAtSequential, Boolean, SynExpr, SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewSequentialOrImplicitYield(DebugPointAtSequential, SynExpr, SynExpr, SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewSet(SynExpr, SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewTraitCall(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynTypar], SynMemberSig, SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewTryFinally(SynExpr, SynExpr, FSharp.Compiler.Text.Range, DebugPointAtTry, DebugPointAtFinally)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewTryWith(SynExpr, FSharp.Compiler.Text.Range, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynMatchClause], FSharp.Compiler.Text.Range, FSharp.Compiler.Text.Range, DebugPointAtTry, DebugPointAtWith)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewTuple(Boolean, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynExpr], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Text.Range], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewTypeApp(SynExpr, FSharp.Compiler.Text.Range, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynType], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Text.Range], Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range], FSharp.Compiler.Text.Range, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewTypeTest(SynExpr, SynType, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewTyped(SynExpr, SynType, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewUpcast(SynExpr, SynType, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewWhile(DebugPointAtWhile, SynExpr, SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewYieldOrReturn(System.Tuple`2[System.Boolean,System.Boolean], SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: SynExpr NewYieldOrReturnFrom(System.Tuple`2[System.Boolean,System.Boolean], SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynExpr: System.String ToString()
FSharp.Compiler.SyntaxTree+SynField: Boolean get_isMutable()
FSharp.Compiler.SyntaxTree+SynField: Boolean get_isStatic()
FSharp.Compiler.SyntaxTree+SynField: Boolean isMutable
FSharp.Compiler.SyntaxTree+SynField: Boolean isStatic
FSharp.Compiler.SyntaxTree+SynField: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynField: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynField: Int32 Tag
FSharp.Compiler.SyntaxTree+SynField: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynField: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList] attributes
FSharp.Compiler.SyntaxTree+SynField: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList] get_attributes()
FSharp.Compiler.SyntaxTree+SynField: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+Ident] get_idOpt()
FSharp.Compiler.SyntaxTree+SynField: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+Ident] idOpt
FSharp.Compiler.SyntaxTree+SynField: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess] accessibility
FSharp.Compiler.SyntaxTree+SynField: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess] get_accessibility()
FSharp.Compiler.SyntaxTree+SynField: PreXmlDoc get_xmlDoc()
FSharp.Compiler.SyntaxTree+SynField: PreXmlDoc xmlDoc
FSharp.Compiler.SyntaxTree+SynField: SynField NewField(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList], Boolean, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+Ident], SynType, Boolean, PreXmlDoc, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynField: SynType fieldType
FSharp.Compiler.SyntaxTree+SynField: SynType get_fieldType()
FSharp.Compiler.SyntaxTree+SynField: System.String ToString()
FSharp.Compiler.SyntaxTree+SynIndexerArg+One: Boolean fromEnd
FSharp.Compiler.SyntaxTree+SynIndexerArg+One: Boolean get_fromEnd()
FSharp.Compiler.SyntaxTree+SynIndexerArg+One: FSharp.Compiler.Text.Range Item3
FSharp.Compiler.SyntaxTree+SynIndexerArg+One: FSharp.Compiler.Text.Range get_Item3()
FSharp.Compiler.SyntaxTree+SynIndexerArg+One: SynExpr expr
FSharp.Compiler.SyntaxTree+SynIndexerArg+One: SynExpr get_expr()
FSharp.Compiler.SyntaxTree+SynIndexerArg+Tags: Int32 One
FSharp.Compiler.SyntaxTree+SynIndexerArg+Tags: Int32 Two
FSharp.Compiler.SyntaxTree+SynIndexerArg+Two: Boolean fromEnd1
FSharp.Compiler.SyntaxTree+SynIndexerArg+Two: Boolean fromEnd2
FSharp.Compiler.SyntaxTree+SynIndexerArg+Two: Boolean get_fromEnd1()
FSharp.Compiler.SyntaxTree+SynIndexerArg+Two: Boolean get_fromEnd2()
FSharp.Compiler.SyntaxTree+SynIndexerArg+Two: FSharp.Compiler.Text.Range get_range1()
FSharp.Compiler.SyntaxTree+SynIndexerArg+Two: FSharp.Compiler.Text.Range get_range2()
FSharp.Compiler.SyntaxTree+SynIndexerArg+Two: FSharp.Compiler.Text.Range range1
FSharp.Compiler.SyntaxTree+SynIndexerArg+Two: FSharp.Compiler.Text.Range range2
FSharp.Compiler.SyntaxTree+SynIndexerArg+Two: SynExpr expr1
FSharp.Compiler.SyntaxTree+SynIndexerArg+Two: SynExpr expr2
FSharp.Compiler.SyntaxTree+SynIndexerArg+Two: SynExpr get_expr1()
FSharp.Compiler.SyntaxTree+SynIndexerArg+Two: SynExpr get_expr2()
FSharp.Compiler.SyntaxTree+SynIndexerArg: Boolean IsOne
FSharp.Compiler.SyntaxTree+SynIndexerArg: Boolean IsTwo
FSharp.Compiler.SyntaxTree+SynIndexerArg: Boolean get_IsOne()
FSharp.Compiler.SyntaxTree+SynIndexerArg: Boolean get_IsTwo()
FSharp.Compiler.SyntaxTree+SynIndexerArg: FSharp.Compiler.SyntaxTree+SynIndexerArg+One
FSharp.Compiler.SyntaxTree+SynIndexerArg: FSharp.Compiler.SyntaxTree+SynIndexerArg+Tags
FSharp.Compiler.SyntaxTree+SynIndexerArg: FSharp.Compiler.SyntaxTree+SynIndexerArg+Two
FSharp.Compiler.SyntaxTree+SynIndexerArg: FSharp.Compiler.Text.Range Range
FSharp.Compiler.SyntaxTree+SynIndexerArg: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.SyntaxTree+SynIndexerArg: Int32 Tag
FSharp.Compiler.SyntaxTree+SynIndexerArg: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynIndexerArg: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynExpr] Exprs
FSharp.Compiler.SyntaxTree+SynIndexerArg: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynExpr] get_Exprs()
FSharp.Compiler.SyntaxTree+SynIndexerArg: SynIndexerArg NewOne(SynExpr, Boolean, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynIndexerArg: SynIndexerArg NewTwo(SynExpr, Boolean, SynExpr, Boolean, FSharp.Compiler.Text.Range, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynIndexerArg: System.String ToString()
FSharp.Compiler.SyntaxTree+SynInterfaceImpl: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynInterfaceImpl: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynInterfaceImpl: Int32 Tag
FSharp.Compiler.SyntaxTree+SynInterfaceImpl: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynInterfaceImpl: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynBinding] bindings
FSharp.Compiler.SyntaxTree+SynInterfaceImpl: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynBinding] get_bindings()
FSharp.Compiler.SyntaxTree+SynInterfaceImpl: SynInterfaceImpl NewInterfaceImpl(SynType, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynBinding], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynInterfaceImpl: SynType get_interfaceTy()
FSharp.Compiler.SyntaxTree+SynInterfaceImpl: SynType interfaceTy
FSharp.Compiler.SyntaxTree+SynInterfaceImpl: System.String ToString()
FSharp.Compiler.SyntaxTree+SynInterpolatedStringPart+FillExpr: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+Ident] get_qualifiers()
FSharp.Compiler.SyntaxTree+SynInterpolatedStringPart+FillExpr: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+Ident] qualifiers
FSharp.Compiler.SyntaxTree+SynInterpolatedStringPart+FillExpr: SynExpr fillExpr
FSharp.Compiler.SyntaxTree+SynInterpolatedStringPart+FillExpr: SynExpr get_fillExpr()
FSharp.Compiler.SyntaxTree+SynInterpolatedStringPart+String: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynInterpolatedStringPart+String: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynInterpolatedStringPart+String: System.String get_value()
FSharp.Compiler.SyntaxTree+SynInterpolatedStringPart+String: System.String value
FSharp.Compiler.SyntaxTree+SynInterpolatedStringPart+Tags: Int32 FillExpr
FSharp.Compiler.SyntaxTree+SynInterpolatedStringPart+Tags: Int32 String
FSharp.Compiler.SyntaxTree+SynInterpolatedStringPart: Boolean IsFillExpr
FSharp.Compiler.SyntaxTree+SynInterpolatedStringPart: Boolean IsString
FSharp.Compiler.SyntaxTree+SynInterpolatedStringPart: Boolean get_IsFillExpr()
FSharp.Compiler.SyntaxTree+SynInterpolatedStringPart: Boolean get_IsString()
FSharp.Compiler.SyntaxTree+SynInterpolatedStringPart: FSharp.Compiler.SyntaxTree+SynInterpolatedStringPart+FillExpr
FSharp.Compiler.SyntaxTree+SynInterpolatedStringPart: FSharp.Compiler.SyntaxTree+SynInterpolatedStringPart+String
FSharp.Compiler.SyntaxTree+SynInterpolatedStringPart: FSharp.Compiler.SyntaxTree+SynInterpolatedStringPart+Tags
FSharp.Compiler.SyntaxTree+SynInterpolatedStringPart: Int32 Tag
FSharp.Compiler.SyntaxTree+SynInterpolatedStringPart: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynInterpolatedStringPart: SynInterpolatedStringPart NewFillExpr(SynExpr, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+Ident])
FSharp.Compiler.SyntaxTree+SynInterpolatedStringPart: SynInterpolatedStringPart NewString(System.String, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynInterpolatedStringPart: System.String ToString()
FSharp.Compiler.SyntaxTree+SynMatchClause: DebugPointForTarget get_spInfo()
FSharp.Compiler.SyntaxTree+SynMatchClause: DebugPointForTarget spInfo
FSharp.Compiler.SyntaxTree+SynMatchClause: FSharp.Compiler.Text.Range Range
FSharp.Compiler.SyntaxTree+SynMatchClause: FSharp.Compiler.Text.Range RangeOfGuardAndRhs
FSharp.Compiler.SyntaxTree+SynMatchClause: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.SyntaxTree+SynMatchClause: FSharp.Compiler.Text.Range get_RangeOfGuardAndRhs()
FSharp.Compiler.SyntaxTree+SynMatchClause: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynMatchClause: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynMatchClause: Int32 Tag
FSharp.Compiler.SyntaxTree+SynMatchClause: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynMatchClause: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynExpr] get_whenExpr()
FSharp.Compiler.SyntaxTree+SynMatchClause: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynExpr] whenExpr
FSharp.Compiler.SyntaxTree+SynMatchClause: SynExpr get_resultExpr()
FSharp.Compiler.SyntaxTree+SynMatchClause: SynExpr resultExpr
FSharp.Compiler.SyntaxTree+SynMatchClause: SynMatchClause NewClause(SynPat, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynExpr], SynExpr, FSharp.Compiler.Text.Range, DebugPointForTarget)
FSharp.Compiler.SyntaxTree+SynMatchClause: SynPat get_pat()
FSharp.Compiler.SyntaxTree+SynMatchClause: SynPat pat
FSharp.Compiler.SyntaxTree+SynMatchClause: System.String ToString()
FSharp.Compiler.SyntaxTree+SynMeasure+Anon: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynMeasure+Anon: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynMeasure+Divide: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynMeasure+Divide: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynMeasure+Divide: SynMeasure get_measure1()
FSharp.Compiler.SyntaxTree+SynMeasure+Divide: SynMeasure get_measure2()
FSharp.Compiler.SyntaxTree+SynMeasure+Divide: SynMeasure measure1
FSharp.Compiler.SyntaxTree+SynMeasure+Divide: SynMeasure measure2
FSharp.Compiler.SyntaxTree+SynMeasure+Named: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynMeasure+Named: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynMeasure+Named: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident] get_longId()
FSharp.Compiler.SyntaxTree+SynMeasure+Named: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident] longId
FSharp.Compiler.SyntaxTree+SynMeasure+Power: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynMeasure+Power: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynMeasure+Power: SynMeasure get_measure()
FSharp.Compiler.SyntaxTree+SynMeasure+Power: SynMeasure measure
FSharp.Compiler.SyntaxTree+SynMeasure+Power: SynRationalConst get_power()
FSharp.Compiler.SyntaxTree+SynMeasure+Power: SynRationalConst power
FSharp.Compiler.SyntaxTree+SynMeasure+Product: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynMeasure+Product: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynMeasure+Product: SynMeasure get_measure1()
FSharp.Compiler.SyntaxTree+SynMeasure+Product: SynMeasure get_measure2()
FSharp.Compiler.SyntaxTree+SynMeasure+Product: SynMeasure measure1
FSharp.Compiler.SyntaxTree+SynMeasure+Product: SynMeasure measure2
FSharp.Compiler.SyntaxTree+SynMeasure+Seq: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynMeasure+Seq: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynMeasure+Seq: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynMeasure] get_measures()
FSharp.Compiler.SyntaxTree+SynMeasure+Seq: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynMeasure] measures
FSharp.Compiler.SyntaxTree+SynMeasure+Tags: Int32 Anon
FSharp.Compiler.SyntaxTree+SynMeasure+Tags: Int32 Divide
FSharp.Compiler.SyntaxTree+SynMeasure+Tags: Int32 Named
FSharp.Compiler.SyntaxTree+SynMeasure+Tags: Int32 One
FSharp.Compiler.SyntaxTree+SynMeasure+Tags: Int32 Power
FSharp.Compiler.SyntaxTree+SynMeasure+Tags: Int32 Product
FSharp.Compiler.SyntaxTree+SynMeasure+Tags: Int32 Seq
FSharp.Compiler.SyntaxTree+SynMeasure+Tags: Int32 Var
FSharp.Compiler.SyntaxTree+SynMeasure+Var: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynMeasure+Var: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynMeasure+Var: SynTypar get_typar()
FSharp.Compiler.SyntaxTree+SynMeasure+Var: SynTypar typar
FSharp.Compiler.SyntaxTree+SynMeasure: Boolean IsAnon
FSharp.Compiler.SyntaxTree+SynMeasure: Boolean IsDivide
FSharp.Compiler.SyntaxTree+SynMeasure: Boolean IsNamed
FSharp.Compiler.SyntaxTree+SynMeasure: Boolean IsOne
FSharp.Compiler.SyntaxTree+SynMeasure: Boolean IsPower
FSharp.Compiler.SyntaxTree+SynMeasure: Boolean IsProduct
FSharp.Compiler.SyntaxTree+SynMeasure: Boolean IsSeq
FSharp.Compiler.SyntaxTree+SynMeasure: Boolean IsVar
FSharp.Compiler.SyntaxTree+SynMeasure: Boolean get_IsAnon()
FSharp.Compiler.SyntaxTree+SynMeasure: Boolean get_IsDivide()
FSharp.Compiler.SyntaxTree+SynMeasure: Boolean get_IsNamed()
FSharp.Compiler.SyntaxTree+SynMeasure: Boolean get_IsOne()
FSharp.Compiler.SyntaxTree+SynMeasure: Boolean get_IsPower()
FSharp.Compiler.SyntaxTree+SynMeasure: Boolean get_IsProduct()
FSharp.Compiler.SyntaxTree+SynMeasure: Boolean get_IsSeq()
FSharp.Compiler.SyntaxTree+SynMeasure: Boolean get_IsVar()
FSharp.Compiler.SyntaxTree+SynMeasure: FSharp.Compiler.SyntaxTree+SynMeasure+Anon
FSharp.Compiler.SyntaxTree+SynMeasure: FSharp.Compiler.SyntaxTree+SynMeasure+Divide
FSharp.Compiler.SyntaxTree+SynMeasure: FSharp.Compiler.SyntaxTree+SynMeasure+Named
FSharp.Compiler.SyntaxTree+SynMeasure: FSharp.Compiler.SyntaxTree+SynMeasure+Power
FSharp.Compiler.SyntaxTree+SynMeasure: FSharp.Compiler.SyntaxTree+SynMeasure+Product
FSharp.Compiler.SyntaxTree+SynMeasure: FSharp.Compiler.SyntaxTree+SynMeasure+Seq
FSharp.Compiler.SyntaxTree+SynMeasure: FSharp.Compiler.SyntaxTree+SynMeasure+Tags
FSharp.Compiler.SyntaxTree+SynMeasure: FSharp.Compiler.SyntaxTree+SynMeasure+Var
FSharp.Compiler.SyntaxTree+SynMeasure: Int32 Tag
FSharp.Compiler.SyntaxTree+SynMeasure: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynMeasure: SynMeasure NewAnon(FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynMeasure: SynMeasure NewDivide(SynMeasure, SynMeasure, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynMeasure: SynMeasure NewNamed(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynMeasure: SynMeasure NewPower(SynMeasure, SynRationalConst, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynMeasure: SynMeasure NewProduct(SynMeasure, SynMeasure, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynMeasure: SynMeasure NewSeq(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynMeasure], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynMeasure: SynMeasure NewVar(SynTypar, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynMeasure: SynMeasure One
FSharp.Compiler.SyntaxTree+SynMeasure: SynMeasure get_One()
FSharp.Compiler.SyntaxTree+SynMeasure: System.String ToString()
FSharp.Compiler.SyntaxTree+SynMemberDefn+AbstractSlot: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynMemberDefn+AbstractSlot: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynMemberDefn+AbstractSlot: MemberFlags flags
FSharp.Compiler.SyntaxTree+SynMemberDefn+AbstractSlot: MemberFlags get_flags()
FSharp.Compiler.SyntaxTree+SynMemberDefn+AbstractSlot: SynValSig get_slotSig()
FSharp.Compiler.SyntaxTree+SynMemberDefn+AbstractSlot: SynValSig slotSig
FSharp.Compiler.SyntaxTree+SynMemberDefn+AutoProperty: Boolean get_isStatic()
FSharp.Compiler.SyntaxTree+SynMemberDefn+AutoProperty: Boolean isStatic
FSharp.Compiler.SyntaxTree+SynMemberDefn+AutoProperty: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynMemberDefn+AutoProperty: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynMemberDefn+AutoProperty: Ident get_ident()
FSharp.Compiler.SyntaxTree+SynMemberDefn+AutoProperty: Ident ident
FSharp.Compiler.SyntaxTree+SynMemberDefn+AutoProperty: MemberKind get_propKind()
FSharp.Compiler.SyntaxTree+SynMemberDefn+AutoProperty: MemberKind propKind
FSharp.Compiler.SyntaxTree+SynMemberDefn+AutoProperty: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList] attributes
FSharp.Compiler.SyntaxTree+SynMemberDefn+AutoProperty: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList] get_attributes()
FSharp.Compiler.SyntaxTree+SynMemberDefn+AutoProperty: Microsoft.FSharp.Core.FSharpFunc`2[FSharp.Compiler.SyntaxTree+MemberKind,FSharp.Compiler.SyntaxTree+MemberFlags] get_memberFlags()
FSharp.Compiler.SyntaxTree+SynMemberDefn+AutoProperty: Microsoft.FSharp.Core.FSharpFunc`2[FSharp.Compiler.SyntaxTree+MemberKind,FSharp.Compiler.SyntaxTree+MemberFlags] memberFlags
FSharp.Compiler.SyntaxTree+SynMemberDefn+AutoProperty: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess] accessibility
FSharp.Compiler.SyntaxTree+SynMemberDefn+AutoProperty: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess] get_accessibility()
FSharp.Compiler.SyntaxTree+SynMemberDefn+AutoProperty: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynType] get_typeOpt()
FSharp.Compiler.SyntaxTree+SynMemberDefn+AutoProperty: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynType] typeOpt
FSharp.Compiler.SyntaxTree+SynMemberDefn+AutoProperty: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] getSetRange
FSharp.Compiler.SyntaxTree+SynMemberDefn+AutoProperty: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] get_getSetRange()
FSharp.Compiler.SyntaxTree+SynMemberDefn+AutoProperty: PreXmlDoc get_xmlDoc()
FSharp.Compiler.SyntaxTree+SynMemberDefn+AutoProperty: PreXmlDoc xmlDoc
FSharp.Compiler.SyntaxTree+SynMemberDefn+AutoProperty: SynExpr get_synExpr()
FSharp.Compiler.SyntaxTree+SynMemberDefn+AutoProperty: SynExpr synExpr
FSharp.Compiler.SyntaxTree+SynMemberDefn+ImplicitCtor: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynMemberDefn+ImplicitCtor: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynMemberDefn+ImplicitCtor: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList] attributes
FSharp.Compiler.SyntaxTree+SynMemberDefn+ImplicitCtor: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList] get_attributes()
FSharp.Compiler.SyntaxTree+SynMemberDefn+ImplicitCtor: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+Ident] get_selfIdentifier()
FSharp.Compiler.SyntaxTree+SynMemberDefn+ImplicitCtor: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+Ident] selfIdentifier
FSharp.Compiler.SyntaxTree+SynMemberDefn+ImplicitCtor: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess] accessibility
FSharp.Compiler.SyntaxTree+SynMemberDefn+ImplicitCtor: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess] get_accessibility()
FSharp.Compiler.SyntaxTree+SynMemberDefn+ImplicitCtor: PreXmlDoc get_xmlDoc()
FSharp.Compiler.SyntaxTree+SynMemberDefn+ImplicitCtor: PreXmlDoc xmlDoc
FSharp.Compiler.SyntaxTree+SynMemberDefn+ImplicitCtor: SynSimplePats ctorArgs
FSharp.Compiler.SyntaxTree+SynMemberDefn+ImplicitCtor: SynSimplePats get_ctorArgs()
FSharp.Compiler.SyntaxTree+SynMemberDefn+ImplicitInherit: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynMemberDefn+ImplicitInherit: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynMemberDefn+ImplicitInherit: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+Ident] get_inheritAlias()
FSharp.Compiler.SyntaxTree+SynMemberDefn+ImplicitInherit: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+Ident] inheritAlias
FSharp.Compiler.SyntaxTree+SynMemberDefn+ImplicitInherit: SynExpr get_inheritArgs()
FSharp.Compiler.SyntaxTree+SynMemberDefn+ImplicitInherit: SynExpr inheritArgs
FSharp.Compiler.SyntaxTree+SynMemberDefn+ImplicitInherit: SynType get_inheritType()
FSharp.Compiler.SyntaxTree+SynMemberDefn+ImplicitInherit: SynType inheritType
FSharp.Compiler.SyntaxTree+SynMemberDefn+Inherit: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynMemberDefn+Inherit: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynMemberDefn+Inherit: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+Ident] asIdent
FSharp.Compiler.SyntaxTree+SynMemberDefn+Inherit: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+Ident] get_asIdent()
FSharp.Compiler.SyntaxTree+SynMemberDefn+Inherit: SynType baseType
FSharp.Compiler.SyntaxTree+SynMemberDefn+Inherit: SynType get_baseType()
FSharp.Compiler.SyntaxTree+SynMemberDefn+Interface: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynMemberDefn+Interface: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynMemberDefn+Interface: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynMemberDefn]] get_members()
FSharp.Compiler.SyntaxTree+SynMemberDefn+Interface: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynMemberDefn]] members
FSharp.Compiler.SyntaxTree+SynMemberDefn+Interface: SynType get_interfaceType()
FSharp.Compiler.SyntaxTree+SynMemberDefn+Interface: SynType interfaceType
FSharp.Compiler.SyntaxTree+SynMemberDefn+LetBindings: Boolean get_isRecursive()
FSharp.Compiler.SyntaxTree+SynMemberDefn+LetBindings: Boolean get_isStatic()
FSharp.Compiler.SyntaxTree+SynMemberDefn+LetBindings: Boolean isRecursive
FSharp.Compiler.SyntaxTree+SynMemberDefn+LetBindings: Boolean isStatic
FSharp.Compiler.SyntaxTree+SynMemberDefn+LetBindings: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynMemberDefn+LetBindings: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynMemberDefn+LetBindings: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynBinding] bindings
FSharp.Compiler.SyntaxTree+SynMemberDefn+LetBindings: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynBinding] get_bindings()
FSharp.Compiler.SyntaxTree+SynMemberDefn+Member: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynMemberDefn+Member: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynMemberDefn+Member: SynBinding get_memberDefn()
FSharp.Compiler.SyntaxTree+SynMemberDefn+Member: SynBinding memberDefn
FSharp.Compiler.SyntaxTree+SynMemberDefn+NestedType: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynMemberDefn+NestedType: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynMemberDefn+NestedType: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess] accessibility
FSharp.Compiler.SyntaxTree+SynMemberDefn+NestedType: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess] get_accessibility()
FSharp.Compiler.SyntaxTree+SynMemberDefn+NestedType: SynTypeDefn get_typeDefn()
FSharp.Compiler.SyntaxTree+SynMemberDefn+NestedType: SynTypeDefn typeDefn
FSharp.Compiler.SyntaxTree+SynMemberDefn+Open: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynMemberDefn+Open: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynMemberDefn+Open: SynOpenDeclTarget get_target()
FSharp.Compiler.SyntaxTree+SynMemberDefn+Open: SynOpenDeclTarget target
FSharp.Compiler.SyntaxTree+SynMemberDefn+Tags: Int32 AbstractSlot
FSharp.Compiler.SyntaxTree+SynMemberDefn+Tags: Int32 AutoProperty
FSharp.Compiler.SyntaxTree+SynMemberDefn+Tags: Int32 ImplicitCtor
FSharp.Compiler.SyntaxTree+SynMemberDefn+Tags: Int32 ImplicitInherit
FSharp.Compiler.SyntaxTree+SynMemberDefn+Tags: Int32 Inherit
FSharp.Compiler.SyntaxTree+SynMemberDefn+Tags: Int32 Interface
FSharp.Compiler.SyntaxTree+SynMemberDefn+Tags: Int32 LetBindings
FSharp.Compiler.SyntaxTree+SynMemberDefn+Tags: Int32 Member
FSharp.Compiler.SyntaxTree+SynMemberDefn+Tags: Int32 NestedType
FSharp.Compiler.SyntaxTree+SynMemberDefn+Tags: Int32 Open
FSharp.Compiler.SyntaxTree+SynMemberDefn+Tags: Int32 ValField
FSharp.Compiler.SyntaxTree+SynMemberDefn+ValField: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynMemberDefn+ValField: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynMemberDefn+ValField: SynField fieldInfo
FSharp.Compiler.SyntaxTree+SynMemberDefn+ValField: SynField get_fieldInfo()
FSharp.Compiler.SyntaxTree+SynMemberDefn: Boolean IsAbstractSlot
FSharp.Compiler.SyntaxTree+SynMemberDefn: Boolean IsAutoProperty
FSharp.Compiler.SyntaxTree+SynMemberDefn: Boolean IsImplicitCtor
FSharp.Compiler.SyntaxTree+SynMemberDefn: Boolean IsImplicitInherit
FSharp.Compiler.SyntaxTree+SynMemberDefn: Boolean IsInherit
FSharp.Compiler.SyntaxTree+SynMemberDefn: Boolean IsInterface
FSharp.Compiler.SyntaxTree+SynMemberDefn: Boolean IsLetBindings
FSharp.Compiler.SyntaxTree+SynMemberDefn: Boolean IsMember
FSharp.Compiler.SyntaxTree+SynMemberDefn: Boolean IsNestedType
FSharp.Compiler.SyntaxTree+SynMemberDefn: Boolean IsOpen
FSharp.Compiler.SyntaxTree+SynMemberDefn: Boolean IsValField
FSharp.Compiler.SyntaxTree+SynMemberDefn: Boolean get_IsAbstractSlot()
FSharp.Compiler.SyntaxTree+SynMemberDefn: Boolean get_IsAutoProperty()
FSharp.Compiler.SyntaxTree+SynMemberDefn: Boolean get_IsImplicitCtor()
FSharp.Compiler.SyntaxTree+SynMemberDefn: Boolean get_IsImplicitInherit()
FSharp.Compiler.SyntaxTree+SynMemberDefn: Boolean get_IsInherit()
FSharp.Compiler.SyntaxTree+SynMemberDefn: Boolean get_IsInterface()
FSharp.Compiler.SyntaxTree+SynMemberDefn: Boolean get_IsLetBindings()
FSharp.Compiler.SyntaxTree+SynMemberDefn: Boolean get_IsMember()
FSharp.Compiler.SyntaxTree+SynMemberDefn: Boolean get_IsNestedType()
FSharp.Compiler.SyntaxTree+SynMemberDefn: Boolean get_IsOpen()
FSharp.Compiler.SyntaxTree+SynMemberDefn: Boolean get_IsValField()
FSharp.Compiler.SyntaxTree+SynMemberDefn: FSharp.Compiler.SyntaxTree+SynMemberDefn+AbstractSlot
FSharp.Compiler.SyntaxTree+SynMemberDefn: FSharp.Compiler.SyntaxTree+SynMemberDefn+AutoProperty
FSharp.Compiler.SyntaxTree+SynMemberDefn: FSharp.Compiler.SyntaxTree+SynMemberDefn+ImplicitCtor
FSharp.Compiler.SyntaxTree+SynMemberDefn: FSharp.Compiler.SyntaxTree+SynMemberDefn+ImplicitInherit
FSharp.Compiler.SyntaxTree+SynMemberDefn: FSharp.Compiler.SyntaxTree+SynMemberDefn+Inherit
FSharp.Compiler.SyntaxTree+SynMemberDefn: FSharp.Compiler.SyntaxTree+SynMemberDefn+Interface
FSharp.Compiler.SyntaxTree+SynMemberDefn: FSharp.Compiler.SyntaxTree+SynMemberDefn+LetBindings
FSharp.Compiler.SyntaxTree+SynMemberDefn: FSharp.Compiler.SyntaxTree+SynMemberDefn+Member
FSharp.Compiler.SyntaxTree+SynMemberDefn: FSharp.Compiler.SyntaxTree+SynMemberDefn+NestedType
FSharp.Compiler.SyntaxTree+SynMemberDefn: FSharp.Compiler.SyntaxTree+SynMemberDefn+Open
FSharp.Compiler.SyntaxTree+SynMemberDefn: FSharp.Compiler.SyntaxTree+SynMemberDefn+Tags
FSharp.Compiler.SyntaxTree+SynMemberDefn: FSharp.Compiler.SyntaxTree+SynMemberDefn+ValField
FSharp.Compiler.SyntaxTree+SynMemberDefn: FSharp.Compiler.Text.Range Range
FSharp.Compiler.SyntaxTree+SynMemberDefn: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.SyntaxTree+SynMemberDefn: Int32 Tag
FSharp.Compiler.SyntaxTree+SynMemberDefn: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynMemberDefn: SynMemberDefn NewAbstractSlot(SynValSig, MemberFlags, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynMemberDefn: SynMemberDefn NewAutoProperty(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList], Boolean, Ident, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynType], MemberKind, Microsoft.FSharp.Core.FSharpFunc`2[FSharp.Compiler.SyntaxTree+MemberKind,FSharp.Compiler.SyntaxTree+MemberFlags], PreXmlDoc, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess], SynExpr, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynMemberDefn: SynMemberDefn NewImplicitCtor(Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList], SynSimplePats, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+Ident], PreXmlDoc, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynMemberDefn: SynMemberDefn NewImplicitInherit(SynType, SynExpr, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+Ident], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynMemberDefn: SynMemberDefn NewInherit(SynType, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+Ident], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynMemberDefn: SynMemberDefn NewInterface(SynType, Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynMemberDefn]], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynMemberDefn: SynMemberDefn NewLetBindings(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynBinding], Boolean, Boolean, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynMemberDefn: SynMemberDefn NewMember(SynBinding, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynMemberDefn: SynMemberDefn NewNestedType(SynTypeDefn, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynMemberDefn: SynMemberDefn NewOpen(SynOpenDeclTarget, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynMemberDefn: SynMemberDefn NewValField(SynField, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynMemberDefn: System.String ToString()
FSharp.Compiler.SyntaxTree+SynMemberSig+Inherit: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynMemberSig+Inherit: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynMemberSig+Inherit: SynType get_inheritedType()
FSharp.Compiler.SyntaxTree+SynMemberSig+Inherit: SynType inheritedType
FSharp.Compiler.SyntaxTree+SynMemberSig+Interface: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynMemberSig+Interface: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynMemberSig+Interface: SynType get_interfaceType()
FSharp.Compiler.SyntaxTree+SynMemberSig+Interface: SynType interfaceType
FSharp.Compiler.SyntaxTree+SynMemberSig+Member: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynMemberSig+Member: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynMemberSig+Member: MemberFlags flags
FSharp.Compiler.SyntaxTree+SynMemberSig+Member: MemberFlags get_flags()
FSharp.Compiler.SyntaxTree+SynMemberSig+Member: SynValSig get_memberSig()
FSharp.Compiler.SyntaxTree+SynMemberSig+Member: SynValSig memberSig
FSharp.Compiler.SyntaxTree+SynMemberSig+NestedType: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynMemberSig+NestedType: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynMemberSig+NestedType: SynTypeDefnSig get_nestedType()
FSharp.Compiler.SyntaxTree+SynMemberSig+NestedType: SynTypeDefnSig nestedType
FSharp.Compiler.SyntaxTree+SynMemberSig+Tags: Int32 Inherit
FSharp.Compiler.SyntaxTree+SynMemberSig+Tags: Int32 Interface
FSharp.Compiler.SyntaxTree+SynMemberSig+Tags: Int32 Member
FSharp.Compiler.SyntaxTree+SynMemberSig+Tags: Int32 NestedType
FSharp.Compiler.SyntaxTree+SynMemberSig+Tags: Int32 ValField
FSharp.Compiler.SyntaxTree+SynMemberSig+ValField: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynMemberSig+ValField: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynMemberSig+ValField: SynField field
FSharp.Compiler.SyntaxTree+SynMemberSig+ValField: SynField get_field()
FSharp.Compiler.SyntaxTree+SynMemberSig: Boolean IsInherit
FSharp.Compiler.SyntaxTree+SynMemberSig: Boolean IsInterface
FSharp.Compiler.SyntaxTree+SynMemberSig: Boolean IsMember
FSharp.Compiler.SyntaxTree+SynMemberSig: Boolean IsNestedType
FSharp.Compiler.SyntaxTree+SynMemberSig: Boolean IsValField
FSharp.Compiler.SyntaxTree+SynMemberSig: Boolean get_IsInherit()
FSharp.Compiler.SyntaxTree+SynMemberSig: Boolean get_IsInterface()
FSharp.Compiler.SyntaxTree+SynMemberSig: Boolean get_IsMember()
FSharp.Compiler.SyntaxTree+SynMemberSig: Boolean get_IsNestedType()
FSharp.Compiler.SyntaxTree+SynMemberSig: Boolean get_IsValField()
FSharp.Compiler.SyntaxTree+SynMemberSig: FSharp.Compiler.SyntaxTree+SynMemberSig+Inherit
FSharp.Compiler.SyntaxTree+SynMemberSig: FSharp.Compiler.SyntaxTree+SynMemberSig+Interface
FSharp.Compiler.SyntaxTree+SynMemberSig: FSharp.Compiler.SyntaxTree+SynMemberSig+Member
FSharp.Compiler.SyntaxTree+SynMemberSig: FSharp.Compiler.SyntaxTree+SynMemberSig+NestedType
FSharp.Compiler.SyntaxTree+SynMemberSig: FSharp.Compiler.SyntaxTree+SynMemberSig+Tags
FSharp.Compiler.SyntaxTree+SynMemberSig: FSharp.Compiler.SyntaxTree+SynMemberSig+ValField
FSharp.Compiler.SyntaxTree+SynMemberSig: Int32 Tag
FSharp.Compiler.SyntaxTree+SynMemberSig: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynMemberSig: SynMemberSig NewInherit(SynType, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynMemberSig: SynMemberSig NewInterface(SynType, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynMemberSig: SynMemberSig NewMember(SynValSig, MemberFlags, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynMemberSig: SynMemberSig NewNestedType(SynTypeDefnSig, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynMemberSig: SynMemberSig NewValField(SynField, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynMemberSig: System.String ToString()
FSharp.Compiler.SyntaxTree+SynModuleDecl+Attributes: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynModuleDecl+Attributes: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynModuleDecl+Attributes: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList] attributes
FSharp.Compiler.SyntaxTree+SynModuleDecl+Attributes: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList] get_attributes()
FSharp.Compiler.SyntaxTree+SynModuleDecl+DoExpr: DebugPointForBinding get_spInfo()
FSharp.Compiler.SyntaxTree+SynModuleDecl+DoExpr: DebugPointForBinding spInfo
FSharp.Compiler.SyntaxTree+SynModuleDecl+DoExpr: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynModuleDecl+DoExpr: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynModuleDecl+DoExpr: SynExpr expr
FSharp.Compiler.SyntaxTree+SynModuleDecl+DoExpr: SynExpr get_expr()
FSharp.Compiler.SyntaxTree+SynModuleDecl+Exception: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynModuleDecl+Exception: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynModuleDecl+Exception: SynExceptionDefn exnDefn
FSharp.Compiler.SyntaxTree+SynModuleDecl+Exception: SynExceptionDefn get_exnDefn()
FSharp.Compiler.SyntaxTree+SynModuleDecl+HashDirective: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynModuleDecl+HashDirective: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynModuleDecl+HashDirective: ParsedHashDirective get_hashDirective()
FSharp.Compiler.SyntaxTree+SynModuleDecl+HashDirective: ParsedHashDirective hashDirective
FSharp.Compiler.SyntaxTree+SynModuleDecl+Let: Boolean get_isRecursive()
FSharp.Compiler.SyntaxTree+SynModuleDecl+Let: Boolean isRecursive
FSharp.Compiler.SyntaxTree+SynModuleDecl+Let: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynModuleDecl+Let: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynModuleDecl+Let: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynBinding] bindings
FSharp.Compiler.SyntaxTree+SynModuleDecl+Let: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynBinding] get_bindings()
FSharp.Compiler.SyntaxTree+SynModuleDecl+ModuleAbbrev: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynModuleDecl+ModuleAbbrev: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynModuleDecl+ModuleAbbrev: Ident get_ident()
FSharp.Compiler.SyntaxTree+SynModuleDecl+ModuleAbbrev: Ident ident
FSharp.Compiler.SyntaxTree+SynModuleDecl+ModuleAbbrev: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident] get_longId()
FSharp.Compiler.SyntaxTree+SynModuleDecl+ModuleAbbrev: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident] longId
FSharp.Compiler.SyntaxTree+SynModuleDecl+NamespaceFragment: SynModuleOrNamespace fragment
FSharp.Compiler.SyntaxTree+SynModuleDecl+NamespaceFragment: SynModuleOrNamespace get_fragment()
FSharp.Compiler.SyntaxTree+SynModuleDecl+NestedModule: Boolean get_isContinuing()
FSharp.Compiler.SyntaxTree+SynModuleDecl+NestedModule: Boolean get_isRecursive()
FSharp.Compiler.SyntaxTree+SynModuleDecl+NestedModule: Boolean isContinuing
FSharp.Compiler.SyntaxTree+SynModuleDecl+NestedModule: Boolean isRecursive
FSharp.Compiler.SyntaxTree+SynModuleDecl+NestedModule: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynModuleDecl+NestedModule: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynModuleDecl+NestedModule: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynModuleDecl] decls
FSharp.Compiler.SyntaxTree+SynModuleDecl+NestedModule: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynModuleDecl] get_decls()
FSharp.Compiler.SyntaxTree+SynModuleDecl+NestedModule: SynComponentInfo get_moduleInfo()
FSharp.Compiler.SyntaxTree+SynModuleDecl+NestedModule: SynComponentInfo moduleInfo
FSharp.Compiler.SyntaxTree+SynModuleDecl+Open: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynModuleDecl+Open: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynModuleDecl+Open: SynOpenDeclTarget get_target()
FSharp.Compiler.SyntaxTree+SynModuleDecl+Open: SynOpenDeclTarget target
FSharp.Compiler.SyntaxTree+SynModuleDecl+Tags: Int32 Attributes
FSharp.Compiler.SyntaxTree+SynModuleDecl+Tags: Int32 DoExpr
FSharp.Compiler.SyntaxTree+SynModuleDecl+Tags: Int32 Exception
FSharp.Compiler.SyntaxTree+SynModuleDecl+Tags: Int32 HashDirective
FSharp.Compiler.SyntaxTree+SynModuleDecl+Tags: Int32 Let
FSharp.Compiler.SyntaxTree+SynModuleDecl+Tags: Int32 ModuleAbbrev
FSharp.Compiler.SyntaxTree+SynModuleDecl+Tags: Int32 NamespaceFragment
FSharp.Compiler.SyntaxTree+SynModuleDecl+Tags: Int32 NestedModule
FSharp.Compiler.SyntaxTree+SynModuleDecl+Tags: Int32 Open
FSharp.Compiler.SyntaxTree+SynModuleDecl+Tags: Int32 Types
FSharp.Compiler.SyntaxTree+SynModuleDecl+Types: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynModuleDecl+Types: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynModuleDecl+Types: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynTypeDefn] get_typeDefns()
FSharp.Compiler.SyntaxTree+SynModuleDecl+Types: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynTypeDefn] typeDefns
FSharp.Compiler.SyntaxTree+SynModuleDecl: Boolean IsAttributes
FSharp.Compiler.SyntaxTree+SynModuleDecl: Boolean IsDoExpr
FSharp.Compiler.SyntaxTree+SynModuleDecl: Boolean IsException
FSharp.Compiler.SyntaxTree+SynModuleDecl: Boolean IsHashDirective
FSharp.Compiler.SyntaxTree+SynModuleDecl: Boolean IsLet
FSharp.Compiler.SyntaxTree+SynModuleDecl: Boolean IsModuleAbbrev
FSharp.Compiler.SyntaxTree+SynModuleDecl: Boolean IsNamespaceFragment
FSharp.Compiler.SyntaxTree+SynModuleDecl: Boolean IsNestedModule
FSharp.Compiler.SyntaxTree+SynModuleDecl: Boolean IsOpen
FSharp.Compiler.SyntaxTree+SynModuleDecl: Boolean IsTypes
FSharp.Compiler.SyntaxTree+SynModuleDecl: Boolean get_IsAttributes()
FSharp.Compiler.SyntaxTree+SynModuleDecl: Boolean get_IsDoExpr()
FSharp.Compiler.SyntaxTree+SynModuleDecl: Boolean get_IsException()
FSharp.Compiler.SyntaxTree+SynModuleDecl: Boolean get_IsHashDirective()
FSharp.Compiler.SyntaxTree+SynModuleDecl: Boolean get_IsLet()
FSharp.Compiler.SyntaxTree+SynModuleDecl: Boolean get_IsModuleAbbrev()
FSharp.Compiler.SyntaxTree+SynModuleDecl: Boolean get_IsNamespaceFragment()
FSharp.Compiler.SyntaxTree+SynModuleDecl: Boolean get_IsNestedModule()
FSharp.Compiler.SyntaxTree+SynModuleDecl: Boolean get_IsOpen()
FSharp.Compiler.SyntaxTree+SynModuleDecl: Boolean get_IsTypes()
FSharp.Compiler.SyntaxTree+SynModuleDecl: FSharp.Compiler.SyntaxTree+SynModuleDecl+Attributes
FSharp.Compiler.SyntaxTree+SynModuleDecl: FSharp.Compiler.SyntaxTree+SynModuleDecl+DoExpr
FSharp.Compiler.SyntaxTree+SynModuleDecl: FSharp.Compiler.SyntaxTree+SynModuleDecl+Exception
FSharp.Compiler.SyntaxTree+SynModuleDecl: FSharp.Compiler.SyntaxTree+SynModuleDecl+HashDirective
FSharp.Compiler.SyntaxTree+SynModuleDecl: FSharp.Compiler.SyntaxTree+SynModuleDecl+Let
FSharp.Compiler.SyntaxTree+SynModuleDecl: FSharp.Compiler.SyntaxTree+SynModuleDecl+ModuleAbbrev
FSharp.Compiler.SyntaxTree+SynModuleDecl: FSharp.Compiler.SyntaxTree+SynModuleDecl+NamespaceFragment
FSharp.Compiler.SyntaxTree+SynModuleDecl: FSharp.Compiler.SyntaxTree+SynModuleDecl+NestedModule
FSharp.Compiler.SyntaxTree+SynModuleDecl: FSharp.Compiler.SyntaxTree+SynModuleDecl+Open
FSharp.Compiler.SyntaxTree+SynModuleDecl: FSharp.Compiler.SyntaxTree+SynModuleDecl+Tags
FSharp.Compiler.SyntaxTree+SynModuleDecl: FSharp.Compiler.SyntaxTree+SynModuleDecl+Types
FSharp.Compiler.SyntaxTree+SynModuleDecl: FSharp.Compiler.Text.Range Range
FSharp.Compiler.SyntaxTree+SynModuleDecl: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.SyntaxTree+SynModuleDecl: Int32 Tag
FSharp.Compiler.SyntaxTree+SynModuleDecl: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynModuleDecl: SynModuleDecl NewAttributes(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynModuleDecl: SynModuleDecl NewDoExpr(DebugPointForBinding, SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynModuleDecl: SynModuleDecl NewException(SynExceptionDefn, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynModuleDecl: SynModuleDecl NewHashDirective(ParsedHashDirective, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynModuleDecl: SynModuleDecl NewLet(Boolean, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynBinding], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynModuleDecl: SynModuleDecl NewModuleAbbrev(Ident, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynModuleDecl: SynModuleDecl NewNamespaceFragment(SynModuleOrNamespace)
FSharp.Compiler.SyntaxTree+SynModuleDecl: SynModuleDecl NewNestedModule(SynComponentInfo, Boolean, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynModuleDecl], Boolean, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynModuleDecl: SynModuleDecl NewOpen(SynOpenDeclTarget, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynModuleDecl: SynModuleDecl NewTypes(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynTypeDefn], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynModuleDecl: System.String ToString()
FSharp.Compiler.SyntaxTree+SynModuleOrNamespace: Boolean get_isRecursive()
FSharp.Compiler.SyntaxTree+SynModuleOrNamespace: Boolean isRecursive
FSharp.Compiler.SyntaxTree+SynModuleOrNamespace: FSharp.Compiler.Text.Range Range
FSharp.Compiler.SyntaxTree+SynModuleOrNamespace: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.SyntaxTree+SynModuleOrNamespace: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynModuleOrNamespace: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynModuleOrNamespace: Int32 Tag
FSharp.Compiler.SyntaxTree+SynModuleOrNamespace: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynModuleOrNamespace: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident] get_longId()
FSharp.Compiler.SyntaxTree+SynModuleOrNamespace: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident] longId
FSharp.Compiler.SyntaxTree+SynModuleOrNamespace: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList] attribs
FSharp.Compiler.SyntaxTree+SynModuleOrNamespace: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList] get_attribs()
FSharp.Compiler.SyntaxTree+SynModuleOrNamespace: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynModuleDecl] decls
FSharp.Compiler.SyntaxTree+SynModuleOrNamespace: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynModuleDecl] get_decls()
FSharp.Compiler.SyntaxTree+SynModuleOrNamespace: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess] accessibility
FSharp.Compiler.SyntaxTree+SynModuleOrNamespace: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess] get_accessibility()
FSharp.Compiler.SyntaxTree+SynModuleOrNamespace: PreXmlDoc get_xmlDoc()
FSharp.Compiler.SyntaxTree+SynModuleOrNamespace: PreXmlDoc xmlDoc
FSharp.Compiler.SyntaxTree+SynModuleOrNamespace: SynModuleOrNamespace NewSynModuleOrNamespace(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident], Boolean, SynModuleOrNamespaceKind, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynModuleDecl], PreXmlDoc, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList], Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynModuleOrNamespace: SynModuleOrNamespaceKind get_kind()
FSharp.Compiler.SyntaxTree+SynModuleOrNamespace: SynModuleOrNamespaceKind kind
FSharp.Compiler.SyntaxTree+SynModuleOrNamespace: System.String ToString()
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceKind+Tags: Int32 AnonModule
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceKind+Tags: Int32 DeclaredNamespace
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceKind+Tags: Int32 GlobalNamespace
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceKind+Tags: Int32 NamedModule
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceKind: Boolean Equals(SynModuleOrNamespaceKind)
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceKind: Boolean Equals(System.Object)
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceKind: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceKind: Boolean IsAnonModule
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceKind: Boolean IsDeclaredNamespace
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceKind: Boolean IsGlobalNamespace
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceKind: Boolean IsModule
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceKind: Boolean IsNamedModule
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceKind: Boolean get_IsAnonModule()
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceKind: Boolean get_IsDeclaredNamespace()
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceKind: Boolean get_IsGlobalNamespace()
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceKind: Boolean get_IsModule()
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceKind: Boolean get_IsNamedModule()
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceKind: FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceKind+Tags
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceKind: Int32 CompareTo(SynModuleOrNamespaceKind)
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceKind: Int32 CompareTo(System.Object)
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceKind: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceKind: Int32 GetHashCode()
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceKind: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceKind: Int32 Tag
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceKind: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceKind: SynModuleOrNamespaceKind AnonModule
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceKind: SynModuleOrNamespaceKind DeclaredNamespace
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceKind: SynModuleOrNamespaceKind GlobalNamespace
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceKind: SynModuleOrNamespaceKind NamedModule
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceKind: SynModuleOrNamespaceKind get_AnonModule()
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceKind: SynModuleOrNamespaceKind get_DeclaredNamespace()
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceKind: SynModuleOrNamespaceKind get_GlobalNamespace()
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceKind: SynModuleOrNamespaceKind get_NamedModule()
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceKind: System.String ToString()
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceSig: Boolean get_isRecursive()
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceSig: Boolean isRecursive
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceSig: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceSig: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceSig: Int32 Tag
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceSig: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceSig: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident] get_longId()
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceSig: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident] longId
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceSig: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList] attribs
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceSig: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList] get_attribs()
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceSig: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynModuleSigDecl] decls
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceSig: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynModuleSigDecl] get_decls()
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceSig: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess] accessibility
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceSig: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess] get_accessibility()
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceSig: PreXmlDoc get_xmlDoc()
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceSig: PreXmlDoc xmlDoc
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceSig: SynModuleOrNamespaceKind get_kind()
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceSig: SynModuleOrNamespaceKind kind
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceSig: SynModuleOrNamespaceSig NewSynModuleOrNamespaceSig(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident], Boolean, SynModuleOrNamespaceKind, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynModuleSigDecl], PreXmlDoc, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList], Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceSig: System.String ToString()
FSharp.Compiler.SyntaxTree+SynModuleSigDecl+Exception: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynModuleSigDecl+Exception: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynModuleSigDecl+Exception: SynExceptionSig exnSig
FSharp.Compiler.SyntaxTree+SynModuleSigDecl+Exception: SynExceptionSig get_exnSig()
FSharp.Compiler.SyntaxTree+SynModuleSigDecl+HashDirective: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynModuleSigDecl+HashDirective: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynModuleSigDecl+HashDirective: ParsedHashDirective get_hashDirective()
FSharp.Compiler.SyntaxTree+SynModuleSigDecl+HashDirective: ParsedHashDirective hashDirective
FSharp.Compiler.SyntaxTree+SynModuleSigDecl+ModuleAbbrev: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynModuleSigDecl+ModuleAbbrev: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynModuleSigDecl+ModuleAbbrev: Ident get_ident()
FSharp.Compiler.SyntaxTree+SynModuleSigDecl+ModuleAbbrev: Ident ident
FSharp.Compiler.SyntaxTree+SynModuleSigDecl+ModuleAbbrev: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident] get_longId()
FSharp.Compiler.SyntaxTree+SynModuleSigDecl+ModuleAbbrev: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident] longId
FSharp.Compiler.SyntaxTree+SynModuleSigDecl+NamespaceFragment: SynModuleOrNamespaceSig Item
FSharp.Compiler.SyntaxTree+SynModuleSigDecl+NamespaceFragment: SynModuleOrNamespaceSig get_Item()
FSharp.Compiler.SyntaxTree+SynModuleSigDecl+NestedModule: Boolean get_isRecursive()
FSharp.Compiler.SyntaxTree+SynModuleSigDecl+NestedModule: Boolean isRecursive
FSharp.Compiler.SyntaxTree+SynModuleSigDecl+NestedModule: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynModuleSigDecl+NestedModule: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynModuleSigDecl+NestedModule: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynModuleSigDecl] get_moduleDecls()
FSharp.Compiler.SyntaxTree+SynModuleSigDecl+NestedModule: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynModuleSigDecl] moduleDecls
FSharp.Compiler.SyntaxTree+SynModuleSigDecl+NestedModule: SynComponentInfo get_moduleInfo()
FSharp.Compiler.SyntaxTree+SynModuleSigDecl+NestedModule: SynComponentInfo moduleInfo
FSharp.Compiler.SyntaxTree+SynModuleSigDecl+Open: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynModuleSigDecl+Open: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynModuleSigDecl+Open: SynOpenDeclTarget get_target()
FSharp.Compiler.SyntaxTree+SynModuleSigDecl+Open: SynOpenDeclTarget target
FSharp.Compiler.SyntaxTree+SynModuleSigDecl+Tags: Int32 Exception
FSharp.Compiler.SyntaxTree+SynModuleSigDecl+Tags: Int32 HashDirective
FSharp.Compiler.SyntaxTree+SynModuleSigDecl+Tags: Int32 ModuleAbbrev
FSharp.Compiler.SyntaxTree+SynModuleSigDecl+Tags: Int32 NamespaceFragment
FSharp.Compiler.SyntaxTree+SynModuleSigDecl+Tags: Int32 NestedModule
FSharp.Compiler.SyntaxTree+SynModuleSigDecl+Tags: Int32 Open
FSharp.Compiler.SyntaxTree+SynModuleSigDecl+Tags: Int32 Types
FSharp.Compiler.SyntaxTree+SynModuleSigDecl+Tags: Int32 Val
FSharp.Compiler.SyntaxTree+SynModuleSigDecl+Types: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynModuleSigDecl+Types: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynModuleSigDecl+Types: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynTypeDefnSig] get_types()
FSharp.Compiler.SyntaxTree+SynModuleSigDecl+Types: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynTypeDefnSig] types
FSharp.Compiler.SyntaxTree+SynModuleSigDecl+Val: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynModuleSigDecl+Val: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynModuleSigDecl+Val: SynValSig get_valSig()
FSharp.Compiler.SyntaxTree+SynModuleSigDecl+Val: SynValSig valSig
FSharp.Compiler.SyntaxTree+SynModuleSigDecl: Boolean IsException
FSharp.Compiler.SyntaxTree+SynModuleSigDecl: Boolean IsHashDirective
FSharp.Compiler.SyntaxTree+SynModuleSigDecl: Boolean IsModuleAbbrev
FSharp.Compiler.SyntaxTree+SynModuleSigDecl: Boolean IsNamespaceFragment
FSharp.Compiler.SyntaxTree+SynModuleSigDecl: Boolean IsNestedModule
FSharp.Compiler.SyntaxTree+SynModuleSigDecl: Boolean IsOpen
FSharp.Compiler.SyntaxTree+SynModuleSigDecl: Boolean IsTypes
FSharp.Compiler.SyntaxTree+SynModuleSigDecl: Boolean IsVal
FSharp.Compiler.SyntaxTree+SynModuleSigDecl: Boolean get_IsException()
FSharp.Compiler.SyntaxTree+SynModuleSigDecl: Boolean get_IsHashDirective()
FSharp.Compiler.SyntaxTree+SynModuleSigDecl: Boolean get_IsModuleAbbrev()
FSharp.Compiler.SyntaxTree+SynModuleSigDecl: Boolean get_IsNamespaceFragment()
FSharp.Compiler.SyntaxTree+SynModuleSigDecl: Boolean get_IsNestedModule()
FSharp.Compiler.SyntaxTree+SynModuleSigDecl: Boolean get_IsOpen()
FSharp.Compiler.SyntaxTree+SynModuleSigDecl: Boolean get_IsTypes()
FSharp.Compiler.SyntaxTree+SynModuleSigDecl: Boolean get_IsVal()
FSharp.Compiler.SyntaxTree+SynModuleSigDecl: FSharp.Compiler.SyntaxTree+SynModuleSigDecl+Exception
FSharp.Compiler.SyntaxTree+SynModuleSigDecl: FSharp.Compiler.SyntaxTree+SynModuleSigDecl+HashDirective
FSharp.Compiler.SyntaxTree+SynModuleSigDecl: FSharp.Compiler.SyntaxTree+SynModuleSigDecl+ModuleAbbrev
FSharp.Compiler.SyntaxTree+SynModuleSigDecl: FSharp.Compiler.SyntaxTree+SynModuleSigDecl+NamespaceFragment
FSharp.Compiler.SyntaxTree+SynModuleSigDecl: FSharp.Compiler.SyntaxTree+SynModuleSigDecl+NestedModule
FSharp.Compiler.SyntaxTree+SynModuleSigDecl: FSharp.Compiler.SyntaxTree+SynModuleSigDecl+Open
FSharp.Compiler.SyntaxTree+SynModuleSigDecl: FSharp.Compiler.SyntaxTree+SynModuleSigDecl+Tags
FSharp.Compiler.SyntaxTree+SynModuleSigDecl: FSharp.Compiler.SyntaxTree+SynModuleSigDecl+Types
FSharp.Compiler.SyntaxTree+SynModuleSigDecl: FSharp.Compiler.SyntaxTree+SynModuleSigDecl+Val
FSharp.Compiler.SyntaxTree+SynModuleSigDecl: FSharp.Compiler.Text.Range Range
FSharp.Compiler.SyntaxTree+SynModuleSigDecl: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.SyntaxTree+SynModuleSigDecl: Int32 Tag
FSharp.Compiler.SyntaxTree+SynModuleSigDecl: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynModuleSigDecl: SynModuleSigDecl NewException(SynExceptionSig, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynModuleSigDecl: SynModuleSigDecl NewHashDirective(ParsedHashDirective, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynModuleSigDecl: SynModuleSigDecl NewModuleAbbrev(Ident, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynModuleSigDecl: SynModuleSigDecl NewNamespaceFragment(SynModuleOrNamespaceSig)
FSharp.Compiler.SyntaxTree+SynModuleSigDecl: SynModuleSigDecl NewNestedModule(SynComponentInfo, Boolean, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynModuleSigDecl], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynModuleSigDecl: SynModuleSigDecl NewOpen(SynOpenDeclTarget, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynModuleSigDecl: SynModuleSigDecl NewTypes(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynTypeDefnSig], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynModuleSigDecl: SynModuleSigDecl NewVal(SynValSig, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynModuleSigDecl: System.String ToString()
FSharp.Compiler.SyntaxTree+SynOpenDeclTarget+ModuleOrNamespace: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynOpenDeclTarget+ModuleOrNamespace: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynOpenDeclTarget+ModuleOrNamespace: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident] get_longId()
FSharp.Compiler.SyntaxTree+SynOpenDeclTarget+ModuleOrNamespace: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident] longId
FSharp.Compiler.SyntaxTree+SynOpenDeclTarget+Tags: Int32 ModuleOrNamespace
FSharp.Compiler.SyntaxTree+SynOpenDeclTarget+Tags: Int32 Type
FSharp.Compiler.SyntaxTree+SynOpenDeclTarget+Type: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynOpenDeclTarget+Type: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynOpenDeclTarget+Type: SynType get_typeName()
FSharp.Compiler.SyntaxTree+SynOpenDeclTarget+Type: SynType typeName
FSharp.Compiler.SyntaxTree+SynOpenDeclTarget: Boolean IsModuleOrNamespace
FSharp.Compiler.SyntaxTree+SynOpenDeclTarget: Boolean IsType
FSharp.Compiler.SyntaxTree+SynOpenDeclTarget: Boolean get_IsModuleOrNamespace()
FSharp.Compiler.SyntaxTree+SynOpenDeclTarget: Boolean get_IsType()
FSharp.Compiler.SyntaxTree+SynOpenDeclTarget: FSharp.Compiler.SyntaxTree+SynOpenDeclTarget+ModuleOrNamespace
FSharp.Compiler.SyntaxTree+SynOpenDeclTarget: FSharp.Compiler.SyntaxTree+SynOpenDeclTarget+Tags
FSharp.Compiler.SyntaxTree+SynOpenDeclTarget: FSharp.Compiler.SyntaxTree+SynOpenDeclTarget+Type
FSharp.Compiler.SyntaxTree+SynOpenDeclTarget: FSharp.Compiler.Text.Range Range
FSharp.Compiler.SyntaxTree+SynOpenDeclTarget: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.SyntaxTree+SynOpenDeclTarget: Int32 Tag
FSharp.Compiler.SyntaxTree+SynOpenDeclTarget: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynOpenDeclTarget: SynOpenDeclTarget NewModuleOrNamespace(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynOpenDeclTarget: SynOpenDeclTarget NewType(SynType, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynOpenDeclTarget: System.String ToString()
FSharp.Compiler.SyntaxTree+SynPat+Ands: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynPat+Ands: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynPat+Ands: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynPat] get_pats()
FSharp.Compiler.SyntaxTree+SynPat+Ands: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynPat] pats
FSharp.Compiler.SyntaxTree+SynPat+ArrayOrList: Boolean get_isArray()
FSharp.Compiler.SyntaxTree+SynPat+ArrayOrList: Boolean isArray
FSharp.Compiler.SyntaxTree+SynPat+ArrayOrList: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynPat+ArrayOrList: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynPat+ArrayOrList: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynPat] elementPats
FSharp.Compiler.SyntaxTree+SynPat+ArrayOrList: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynPat] get_elementPats()
FSharp.Compiler.SyntaxTree+SynPat+Attrib: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynPat+Attrib: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynPat+Attrib: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList] attributes
FSharp.Compiler.SyntaxTree+SynPat+Attrib: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList] get_attributes()
FSharp.Compiler.SyntaxTree+SynPat+Attrib: SynPat get_pat()
FSharp.Compiler.SyntaxTree+SynPat+Attrib: SynPat pat
FSharp.Compiler.SyntaxTree+SynPat+Const: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynPat+Const: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynPat+Const: SynConst constant
FSharp.Compiler.SyntaxTree+SynPat+Const: SynConst get_constant()
FSharp.Compiler.SyntaxTree+SynPat+DeprecatedCharRange: Char endChar
FSharp.Compiler.SyntaxTree+SynPat+DeprecatedCharRange: Char get_endChar()
FSharp.Compiler.SyntaxTree+SynPat+DeprecatedCharRange: Char get_startChar()
FSharp.Compiler.SyntaxTree+SynPat+DeprecatedCharRange: Char startChar
FSharp.Compiler.SyntaxTree+SynPat+DeprecatedCharRange: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynPat+DeprecatedCharRange: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynPat+FromParseError: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynPat+FromParseError: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynPat+FromParseError: SynPat get_pat()
FSharp.Compiler.SyntaxTree+SynPat+FromParseError: SynPat pat
FSharp.Compiler.SyntaxTree+SynPat+InstanceMember: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynPat+InstanceMember: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynPat+InstanceMember: Ident get_memberId()
FSharp.Compiler.SyntaxTree+SynPat+InstanceMember: Ident get_thisId()
FSharp.Compiler.SyntaxTree+SynPat+InstanceMember: Ident memberId
FSharp.Compiler.SyntaxTree+SynPat+InstanceMember: Ident thisId
FSharp.Compiler.SyntaxTree+SynPat+InstanceMember: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+Ident] get_toolingId()
FSharp.Compiler.SyntaxTree+SynPat+InstanceMember: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+Ident] toolingId
FSharp.Compiler.SyntaxTree+SynPat+InstanceMember: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess] accessibility
FSharp.Compiler.SyntaxTree+SynPat+InstanceMember: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess] get_accessibility()
FSharp.Compiler.SyntaxTree+SynPat+IsInst: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynPat+IsInst: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynPat+IsInst: SynType get_pat()
FSharp.Compiler.SyntaxTree+SynPat+IsInst: SynType pat
FSharp.Compiler.SyntaxTree+SynPat+LongIdent: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynPat+LongIdent: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynPat+LongIdent: LongIdentWithDots get_longDotId()
FSharp.Compiler.SyntaxTree+SynPat+LongIdent: LongIdentWithDots longDotId
FSharp.Compiler.SyntaxTree+SynPat+LongIdent: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+Ident] extraId
FSharp.Compiler.SyntaxTree+SynPat+LongIdent: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+Ident] get_extraId()
FSharp.Compiler.SyntaxTree+SynPat+LongIdent: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess] accessibility
FSharp.Compiler.SyntaxTree+SynPat+LongIdent: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess] get_accessibility()
FSharp.Compiler.SyntaxTree+SynPat+LongIdent: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynValTyparDecls] get_typarDecls()
FSharp.Compiler.SyntaxTree+SynPat+LongIdent: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynValTyparDecls] typarDecls
FSharp.Compiler.SyntaxTree+SynPat+LongIdent: SynArgPats argPats
FSharp.Compiler.SyntaxTree+SynPat+LongIdent: SynArgPats get_argPats()
FSharp.Compiler.SyntaxTree+SynPat+Named: Boolean get_isSelfIdentifier()
FSharp.Compiler.SyntaxTree+SynPat+Named: Boolean isSelfIdentifier
FSharp.Compiler.SyntaxTree+SynPat+Named: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynPat+Named: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynPat+Named: Ident get_ident()
FSharp.Compiler.SyntaxTree+SynPat+Named: Ident ident
FSharp.Compiler.SyntaxTree+SynPat+Named: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess] accessibility
FSharp.Compiler.SyntaxTree+SynPat+Named: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess] get_accessibility()
FSharp.Compiler.SyntaxTree+SynPat+Named: SynPat get_pat()
FSharp.Compiler.SyntaxTree+SynPat+Named: SynPat pat
FSharp.Compiler.SyntaxTree+SynPat+Null: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynPat+Null: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynPat+OptionalVal: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynPat+OptionalVal: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynPat+OptionalVal: Ident get_ident()
FSharp.Compiler.SyntaxTree+SynPat+OptionalVal: Ident ident
FSharp.Compiler.SyntaxTree+SynPat+Or: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynPat+Or: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynPat+Or: SynPat get_lhsPat()
FSharp.Compiler.SyntaxTree+SynPat+Or: SynPat get_rhsPat()
FSharp.Compiler.SyntaxTree+SynPat+Or: SynPat lhsPat
FSharp.Compiler.SyntaxTree+SynPat+Or: SynPat rhsPat
FSharp.Compiler.SyntaxTree+SynPat+Paren: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynPat+Paren: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynPat+Paren: SynPat get_pat()
FSharp.Compiler.SyntaxTree+SynPat+Paren: SynPat pat
FSharp.Compiler.SyntaxTree+SynPat+QuoteExpr: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynPat+QuoteExpr: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynPat+QuoteExpr: SynExpr expr
FSharp.Compiler.SyntaxTree+SynPat+QuoteExpr: SynExpr get_expr()
FSharp.Compiler.SyntaxTree+SynPat+Record: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynPat+Record: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynPat+Record: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[System.Tuple`2[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident],FSharp.Compiler.SyntaxTree+Ident],FSharp.Compiler.SyntaxTree+SynPat]] fieldPats
FSharp.Compiler.SyntaxTree+SynPat+Record: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[System.Tuple`2[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident],FSharp.Compiler.SyntaxTree+Ident],FSharp.Compiler.SyntaxTree+SynPat]] get_fieldPats()
FSharp.Compiler.SyntaxTree+SynPat+Tags: Int32 Ands
FSharp.Compiler.SyntaxTree+SynPat+Tags: Int32 ArrayOrList
FSharp.Compiler.SyntaxTree+SynPat+Tags: Int32 Attrib
FSharp.Compiler.SyntaxTree+SynPat+Tags: Int32 Const
FSharp.Compiler.SyntaxTree+SynPat+Tags: Int32 DeprecatedCharRange
FSharp.Compiler.SyntaxTree+SynPat+Tags: Int32 FromParseError
FSharp.Compiler.SyntaxTree+SynPat+Tags: Int32 InstanceMember
FSharp.Compiler.SyntaxTree+SynPat+Tags: Int32 IsInst
FSharp.Compiler.SyntaxTree+SynPat+Tags: Int32 LongIdent
FSharp.Compiler.SyntaxTree+SynPat+Tags: Int32 Named
FSharp.Compiler.SyntaxTree+SynPat+Tags: Int32 Null
FSharp.Compiler.SyntaxTree+SynPat+Tags: Int32 OptionalVal
FSharp.Compiler.SyntaxTree+SynPat+Tags: Int32 Or
FSharp.Compiler.SyntaxTree+SynPat+Tags: Int32 Paren
FSharp.Compiler.SyntaxTree+SynPat+Tags: Int32 QuoteExpr
FSharp.Compiler.SyntaxTree+SynPat+Tags: Int32 Record
FSharp.Compiler.SyntaxTree+SynPat+Tags: Int32 Tuple
FSharp.Compiler.SyntaxTree+SynPat+Tags: Int32 Typed
FSharp.Compiler.SyntaxTree+SynPat+Tags: Int32 Wild
FSharp.Compiler.SyntaxTree+SynPat+Tuple: Boolean get_isStruct()
FSharp.Compiler.SyntaxTree+SynPat+Tuple: Boolean isStruct
FSharp.Compiler.SyntaxTree+SynPat+Tuple: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynPat+Tuple: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynPat+Tuple: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynPat] elementPats
FSharp.Compiler.SyntaxTree+SynPat+Tuple: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynPat] get_elementPats()
FSharp.Compiler.SyntaxTree+SynPat+Typed: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynPat+Typed: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynPat+Typed: SynPat get_pat()
FSharp.Compiler.SyntaxTree+SynPat+Typed: SynPat pat
FSharp.Compiler.SyntaxTree+SynPat+Typed: SynType get_targetType()
FSharp.Compiler.SyntaxTree+SynPat+Typed: SynType targetType
FSharp.Compiler.SyntaxTree+SynPat+Wild: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynPat+Wild: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynPat: Boolean IsAnds
FSharp.Compiler.SyntaxTree+SynPat: Boolean IsArrayOrList
FSharp.Compiler.SyntaxTree+SynPat: Boolean IsAttrib
FSharp.Compiler.SyntaxTree+SynPat: Boolean IsConst
FSharp.Compiler.SyntaxTree+SynPat: Boolean IsDeprecatedCharRange
FSharp.Compiler.SyntaxTree+SynPat: Boolean IsFromParseError
FSharp.Compiler.SyntaxTree+SynPat: Boolean IsInstanceMember
FSharp.Compiler.SyntaxTree+SynPat: Boolean IsIsInst
FSharp.Compiler.SyntaxTree+SynPat: Boolean IsLongIdent
FSharp.Compiler.SyntaxTree+SynPat: Boolean IsNamed
FSharp.Compiler.SyntaxTree+SynPat: Boolean IsNull
FSharp.Compiler.SyntaxTree+SynPat: Boolean IsOptionalVal
FSharp.Compiler.SyntaxTree+SynPat: Boolean IsOr
FSharp.Compiler.SyntaxTree+SynPat: Boolean IsParen
FSharp.Compiler.SyntaxTree+SynPat: Boolean IsQuoteExpr
FSharp.Compiler.SyntaxTree+SynPat: Boolean IsRecord
FSharp.Compiler.SyntaxTree+SynPat: Boolean IsTuple
FSharp.Compiler.SyntaxTree+SynPat: Boolean IsTyped
FSharp.Compiler.SyntaxTree+SynPat: Boolean IsWild
FSharp.Compiler.SyntaxTree+SynPat: Boolean get_IsAnds()
FSharp.Compiler.SyntaxTree+SynPat: Boolean get_IsArrayOrList()
FSharp.Compiler.SyntaxTree+SynPat: Boolean get_IsAttrib()
FSharp.Compiler.SyntaxTree+SynPat: Boolean get_IsConst()
FSharp.Compiler.SyntaxTree+SynPat: Boolean get_IsDeprecatedCharRange()
FSharp.Compiler.SyntaxTree+SynPat: Boolean get_IsFromParseError()
FSharp.Compiler.SyntaxTree+SynPat: Boolean get_IsInstanceMember()
FSharp.Compiler.SyntaxTree+SynPat: Boolean get_IsIsInst()
FSharp.Compiler.SyntaxTree+SynPat: Boolean get_IsLongIdent()
FSharp.Compiler.SyntaxTree+SynPat: Boolean get_IsNamed()
FSharp.Compiler.SyntaxTree+SynPat: Boolean get_IsNull()
FSharp.Compiler.SyntaxTree+SynPat: Boolean get_IsOptionalVal()
FSharp.Compiler.SyntaxTree+SynPat: Boolean get_IsOr()
FSharp.Compiler.SyntaxTree+SynPat: Boolean get_IsParen()
FSharp.Compiler.SyntaxTree+SynPat: Boolean get_IsQuoteExpr()
FSharp.Compiler.SyntaxTree+SynPat: Boolean get_IsRecord()
FSharp.Compiler.SyntaxTree+SynPat: Boolean get_IsTuple()
FSharp.Compiler.SyntaxTree+SynPat: Boolean get_IsTyped()
FSharp.Compiler.SyntaxTree+SynPat: Boolean get_IsWild()
FSharp.Compiler.SyntaxTree+SynPat: FSharp.Compiler.SyntaxTree+SynPat+Ands
FSharp.Compiler.SyntaxTree+SynPat: FSharp.Compiler.SyntaxTree+SynPat+ArrayOrList
FSharp.Compiler.SyntaxTree+SynPat: FSharp.Compiler.SyntaxTree+SynPat+Attrib
FSharp.Compiler.SyntaxTree+SynPat: FSharp.Compiler.SyntaxTree+SynPat+Const
FSharp.Compiler.SyntaxTree+SynPat: FSharp.Compiler.SyntaxTree+SynPat+DeprecatedCharRange
FSharp.Compiler.SyntaxTree+SynPat: FSharp.Compiler.SyntaxTree+SynPat+FromParseError
FSharp.Compiler.SyntaxTree+SynPat: FSharp.Compiler.SyntaxTree+SynPat+InstanceMember
FSharp.Compiler.SyntaxTree+SynPat: FSharp.Compiler.SyntaxTree+SynPat+IsInst
FSharp.Compiler.SyntaxTree+SynPat: FSharp.Compiler.SyntaxTree+SynPat+LongIdent
FSharp.Compiler.SyntaxTree+SynPat: FSharp.Compiler.SyntaxTree+SynPat+Named
FSharp.Compiler.SyntaxTree+SynPat: FSharp.Compiler.SyntaxTree+SynPat+Null
FSharp.Compiler.SyntaxTree+SynPat: FSharp.Compiler.SyntaxTree+SynPat+OptionalVal
FSharp.Compiler.SyntaxTree+SynPat: FSharp.Compiler.SyntaxTree+SynPat+Or
FSharp.Compiler.SyntaxTree+SynPat: FSharp.Compiler.SyntaxTree+SynPat+Paren
FSharp.Compiler.SyntaxTree+SynPat: FSharp.Compiler.SyntaxTree+SynPat+QuoteExpr
FSharp.Compiler.SyntaxTree+SynPat: FSharp.Compiler.SyntaxTree+SynPat+Record
FSharp.Compiler.SyntaxTree+SynPat: FSharp.Compiler.SyntaxTree+SynPat+Tags
FSharp.Compiler.SyntaxTree+SynPat: FSharp.Compiler.SyntaxTree+SynPat+Tuple
FSharp.Compiler.SyntaxTree+SynPat: FSharp.Compiler.SyntaxTree+SynPat+Typed
FSharp.Compiler.SyntaxTree+SynPat: FSharp.Compiler.SyntaxTree+SynPat+Wild
FSharp.Compiler.SyntaxTree+SynPat: FSharp.Compiler.Text.Range Range
FSharp.Compiler.SyntaxTree+SynPat: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.SyntaxTree+SynPat: Int32 Tag
FSharp.Compiler.SyntaxTree+SynPat: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynPat: SynPat NewAnds(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynPat], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynPat: SynPat NewArrayOrList(Boolean, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynPat], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynPat: SynPat NewAttrib(SynPat, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynPat: SynPat NewConst(SynConst, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynPat: SynPat NewDeprecatedCharRange(Char, Char, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynPat: SynPat NewFromParseError(SynPat, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynPat: SynPat NewInstanceMember(Ident, Ident, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+Ident], Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynPat: SynPat NewIsInst(SynType, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynPat: SynPat NewLongIdent(LongIdentWithDots, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+Ident], Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynValTyparDecls], SynArgPats, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynPat: SynPat NewNamed(SynPat, Ident, Boolean, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynPat: SynPat NewNull(FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynPat: SynPat NewOptionalVal(Ident, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynPat: SynPat NewOr(SynPat, SynPat, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynPat: SynPat NewParen(SynPat, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynPat: SynPat NewQuoteExpr(SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynPat: SynPat NewRecord(Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[System.Tuple`2[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+Ident],FSharp.Compiler.SyntaxTree+Ident],FSharp.Compiler.SyntaxTree+SynPat]], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynPat: SynPat NewTuple(Boolean, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynPat], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynPat: SynPat NewTyped(SynPat, SynType, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynPat: SynPat NewWild(FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynPat: System.String ToString()
FSharp.Compiler.SyntaxTree+SynRationalConst+Integer: Int32 get_value()
FSharp.Compiler.SyntaxTree+SynRationalConst+Integer: Int32 value
FSharp.Compiler.SyntaxTree+SynRationalConst+Negate: SynRationalConst Item
FSharp.Compiler.SyntaxTree+SynRationalConst+Negate: SynRationalConst get_Item()
FSharp.Compiler.SyntaxTree+SynRationalConst+Rational: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynRationalConst+Rational: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynRationalConst+Rational: Int32 denominator
FSharp.Compiler.SyntaxTree+SynRationalConst+Rational: Int32 get_denominator()
FSharp.Compiler.SyntaxTree+SynRationalConst+Rational: Int32 get_numerator()
FSharp.Compiler.SyntaxTree+SynRationalConst+Rational: Int32 numerator
FSharp.Compiler.SyntaxTree+SynRationalConst+Tags: Int32 Integer
FSharp.Compiler.SyntaxTree+SynRationalConst+Tags: Int32 Negate
FSharp.Compiler.SyntaxTree+SynRationalConst+Tags: Int32 Rational
FSharp.Compiler.SyntaxTree+SynRationalConst: Boolean IsInteger
FSharp.Compiler.SyntaxTree+SynRationalConst: Boolean IsNegate
FSharp.Compiler.SyntaxTree+SynRationalConst: Boolean IsRational
FSharp.Compiler.SyntaxTree+SynRationalConst: Boolean get_IsInteger()
FSharp.Compiler.SyntaxTree+SynRationalConst: Boolean get_IsNegate()
FSharp.Compiler.SyntaxTree+SynRationalConst: Boolean get_IsRational()
FSharp.Compiler.SyntaxTree+SynRationalConst: FSharp.Compiler.SyntaxTree+SynRationalConst+Integer
FSharp.Compiler.SyntaxTree+SynRationalConst: FSharp.Compiler.SyntaxTree+SynRationalConst+Negate
FSharp.Compiler.SyntaxTree+SynRationalConst: FSharp.Compiler.SyntaxTree+SynRationalConst+Rational
FSharp.Compiler.SyntaxTree+SynRationalConst: FSharp.Compiler.SyntaxTree+SynRationalConst+Tags
FSharp.Compiler.SyntaxTree+SynRationalConst: Int32 Tag
FSharp.Compiler.SyntaxTree+SynRationalConst: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynRationalConst: SynRationalConst NewInteger(Int32)
FSharp.Compiler.SyntaxTree+SynRationalConst: SynRationalConst NewNegate(SynRationalConst)
FSharp.Compiler.SyntaxTree+SynRationalConst: SynRationalConst NewRational(Int32, Int32, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynRationalConst: System.String ToString()
FSharp.Compiler.SyntaxTree+SynReturnInfo: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynReturnInfo: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynReturnInfo: Int32 Tag
FSharp.Compiler.SyntaxTree+SynReturnInfo: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynReturnInfo: SynReturnInfo NewSynReturnInfo(System.Tuple`2[FSharp.Compiler.SyntaxTree+SynType,FSharp.Compiler.SyntaxTree+SynArgInfo], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynReturnInfo: System.String ToString()
FSharp.Compiler.SyntaxTree+SynReturnInfo: System.Tuple`2[FSharp.Compiler.SyntaxTree+SynType,FSharp.Compiler.SyntaxTree+SynArgInfo] get_returnType()
FSharp.Compiler.SyntaxTree+SynReturnInfo: System.Tuple`2[FSharp.Compiler.SyntaxTree+SynType,FSharp.Compiler.SyntaxTree+SynArgInfo] returnType
FSharp.Compiler.SyntaxTree+SynSimplePat+Attrib: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynSimplePat+Attrib: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynSimplePat+Attrib: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList] attributes
FSharp.Compiler.SyntaxTree+SynSimplePat+Attrib: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList] get_attributes()
FSharp.Compiler.SyntaxTree+SynSimplePat+Attrib: SynSimplePat get_pat()
FSharp.Compiler.SyntaxTree+SynSimplePat+Attrib: SynSimplePat pat
FSharp.Compiler.SyntaxTree+SynSimplePat+Id: Boolean get_isCompilerGenerated()
FSharp.Compiler.SyntaxTree+SynSimplePat+Id: Boolean get_isOptArg()
FSharp.Compiler.SyntaxTree+SynSimplePat+Id: Boolean get_isThisVar()
FSharp.Compiler.SyntaxTree+SynSimplePat+Id: Boolean isCompilerGenerated
FSharp.Compiler.SyntaxTree+SynSimplePat+Id: Boolean isOptArg
FSharp.Compiler.SyntaxTree+SynSimplePat+Id: Boolean isThisVar
FSharp.Compiler.SyntaxTree+SynSimplePat+Id: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynSimplePat+Id: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynSimplePat+Id: Ident get_ident()
FSharp.Compiler.SyntaxTree+SynSimplePat+Id: Ident ident
FSharp.Compiler.SyntaxTree+SynSimplePat+Id: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.FSharpRef`1[FSharp.Compiler.SyntaxTree+SynSimplePatAlternativeIdInfo]] altNameRefCell
FSharp.Compiler.SyntaxTree+SynSimplePat+Id: Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.FSharpRef`1[FSharp.Compiler.SyntaxTree+SynSimplePatAlternativeIdInfo]] get_altNameRefCell()
FSharp.Compiler.SyntaxTree+SynSimplePat+Tags: Int32 Attrib
FSharp.Compiler.SyntaxTree+SynSimplePat+Tags: Int32 Id
FSharp.Compiler.SyntaxTree+SynSimplePat+Tags: Int32 Typed
FSharp.Compiler.SyntaxTree+SynSimplePat+Typed: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynSimplePat+Typed: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynSimplePat+Typed: SynSimplePat get_pat()
FSharp.Compiler.SyntaxTree+SynSimplePat+Typed: SynSimplePat pat
FSharp.Compiler.SyntaxTree+SynSimplePat+Typed: SynType get_targetType()
FSharp.Compiler.SyntaxTree+SynSimplePat+Typed: SynType targetType
FSharp.Compiler.SyntaxTree+SynSimplePat: Boolean IsAttrib
FSharp.Compiler.SyntaxTree+SynSimplePat: Boolean IsId
FSharp.Compiler.SyntaxTree+SynSimplePat: Boolean IsTyped
FSharp.Compiler.SyntaxTree+SynSimplePat: Boolean get_IsAttrib()
FSharp.Compiler.SyntaxTree+SynSimplePat: Boolean get_IsId()
FSharp.Compiler.SyntaxTree+SynSimplePat: Boolean get_IsTyped()
FSharp.Compiler.SyntaxTree+SynSimplePat: FSharp.Compiler.SyntaxTree+SynSimplePat+Attrib
FSharp.Compiler.SyntaxTree+SynSimplePat: FSharp.Compiler.SyntaxTree+SynSimplePat+Id
FSharp.Compiler.SyntaxTree+SynSimplePat: FSharp.Compiler.SyntaxTree+SynSimplePat+Tags
FSharp.Compiler.SyntaxTree+SynSimplePat: FSharp.Compiler.SyntaxTree+SynSimplePat+Typed
FSharp.Compiler.SyntaxTree+SynSimplePat: Int32 Tag
FSharp.Compiler.SyntaxTree+SynSimplePat: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynSimplePat: SynSimplePat NewAttrib(SynSimplePat, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynSimplePat: SynSimplePat NewId(Ident, Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Core.FSharpRef`1[FSharp.Compiler.SyntaxTree+SynSimplePatAlternativeIdInfo]], Boolean, Boolean, Boolean, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynSimplePat: SynSimplePat NewTyped(SynSimplePat, SynType, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynSimplePat: System.String ToString()
FSharp.Compiler.SyntaxTree+SynSimplePatAlternativeIdInfo+Decided: Ident Item
FSharp.Compiler.SyntaxTree+SynSimplePatAlternativeIdInfo+Decided: Ident get_Item()
FSharp.Compiler.SyntaxTree+SynSimplePatAlternativeIdInfo+Tags: Int32 Decided
FSharp.Compiler.SyntaxTree+SynSimplePatAlternativeIdInfo+Tags: Int32 Undecided
FSharp.Compiler.SyntaxTree+SynSimplePatAlternativeIdInfo+Undecided: Ident Item
FSharp.Compiler.SyntaxTree+SynSimplePatAlternativeIdInfo+Undecided: Ident get_Item()
FSharp.Compiler.SyntaxTree+SynSimplePatAlternativeIdInfo: Boolean IsDecided
FSharp.Compiler.SyntaxTree+SynSimplePatAlternativeIdInfo: Boolean IsUndecided
FSharp.Compiler.SyntaxTree+SynSimplePatAlternativeIdInfo: Boolean get_IsDecided()
FSharp.Compiler.SyntaxTree+SynSimplePatAlternativeIdInfo: Boolean get_IsUndecided()
FSharp.Compiler.SyntaxTree+SynSimplePatAlternativeIdInfo: FSharp.Compiler.SyntaxTree+SynSimplePatAlternativeIdInfo+Decided
FSharp.Compiler.SyntaxTree+SynSimplePatAlternativeIdInfo: FSharp.Compiler.SyntaxTree+SynSimplePatAlternativeIdInfo+Tags
FSharp.Compiler.SyntaxTree+SynSimplePatAlternativeIdInfo: FSharp.Compiler.SyntaxTree+SynSimplePatAlternativeIdInfo+Undecided
FSharp.Compiler.SyntaxTree+SynSimplePatAlternativeIdInfo: Int32 Tag
FSharp.Compiler.SyntaxTree+SynSimplePatAlternativeIdInfo: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynSimplePatAlternativeIdInfo: SynSimplePatAlternativeIdInfo NewDecided(Ident)
FSharp.Compiler.SyntaxTree+SynSimplePatAlternativeIdInfo: SynSimplePatAlternativeIdInfo NewUndecided(Ident)
FSharp.Compiler.SyntaxTree+SynSimplePatAlternativeIdInfo: System.String ToString()
FSharp.Compiler.SyntaxTree+SynSimplePats+SimplePats: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynSimplePats+SimplePats: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynSimplePats+SimplePats: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynSimplePat] get_pats()
FSharp.Compiler.SyntaxTree+SynSimplePats+SimplePats: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynSimplePat] pats
FSharp.Compiler.SyntaxTree+SynSimplePats+Tags: Int32 SimplePats
FSharp.Compiler.SyntaxTree+SynSimplePats+Tags: Int32 Typed
FSharp.Compiler.SyntaxTree+SynSimplePats+Typed: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynSimplePats+Typed: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynSimplePats+Typed: SynSimplePats get_pats()
FSharp.Compiler.SyntaxTree+SynSimplePats+Typed: SynSimplePats pats
FSharp.Compiler.SyntaxTree+SynSimplePats+Typed: SynType get_targetType()
FSharp.Compiler.SyntaxTree+SynSimplePats+Typed: SynType targetType
FSharp.Compiler.SyntaxTree+SynSimplePats: Boolean IsSimplePats
FSharp.Compiler.SyntaxTree+SynSimplePats: Boolean IsTyped
FSharp.Compiler.SyntaxTree+SynSimplePats: Boolean get_IsSimplePats()
FSharp.Compiler.SyntaxTree+SynSimplePats: Boolean get_IsTyped()
FSharp.Compiler.SyntaxTree+SynSimplePats: FSharp.Compiler.SyntaxTree+SynSimplePats+SimplePats
FSharp.Compiler.SyntaxTree+SynSimplePats: FSharp.Compiler.SyntaxTree+SynSimplePats+Tags
FSharp.Compiler.SyntaxTree+SynSimplePats: FSharp.Compiler.SyntaxTree+SynSimplePats+Typed
FSharp.Compiler.SyntaxTree+SynSimplePats: Int32 Tag
FSharp.Compiler.SyntaxTree+SynSimplePats: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynSimplePats: SynSimplePats NewSimplePats(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynSimplePat], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynSimplePats: SynSimplePats NewTyped(SynSimplePats, SynType, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynSimplePats: System.String ToString()
FSharp.Compiler.SyntaxTree+SynStaticOptimizationConstraint+Tags: Int32 WhenTyparIsStruct
FSharp.Compiler.SyntaxTree+SynStaticOptimizationConstraint+Tags: Int32 WhenTyparTyconEqualsTycon
FSharp.Compiler.SyntaxTree+SynStaticOptimizationConstraint+WhenTyparIsStruct: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynStaticOptimizationConstraint+WhenTyparIsStruct: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynStaticOptimizationConstraint+WhenTyparIsStruct: SynTypar get_typar()
FSharp.Compiler.SyntaxTree+SynStaticOptimizationConstraint+WhenTyparIsStruct: SynTypar typar
FSharp.Compiler.SyntaxTree+SynStaticOptimizationConstraint+WhenTyparTyconEqualsTycon: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynStaticOptimizationConstraint+WhenTyparTyconEqualsTycon: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynStaticOptimizationConstraint+WhenTyparTyconEqualsTycon: SynTypar get_typar()
FSharp.Compiler.SyntaxTree+SynStaticOptimizationConstraint+WhenTyparTyconEqualsTycon: SynTypar typar
FSharp.Compiler.SyntaxTree+SynStaticOptimizationConstraint+WhenTyparTyconEqualsTycon: SynType get_rhsType()
FSharp.Compiler.SyntaxTree+SynStaticOptimizationConstraint+WhenTyparTyconEqualsTycon: SynType rhsType
FSharp.Compiler.SyntaxTree+SynStaticOptimizationConstraint: Boolean IsWhenTyparIsStruct
FSharp.Compiler.SyntaxTree+SynStaticOptimizationConstraint: Boolean IsWhenTyparTyconEqualsTycon
FSharp.Compiler.SyntaxTree+SynStaticOptimizationConstraint: Boolean get_IsWhenTyparIsStruct()
FSharp.Compiler.SyntaxTree+SynStaticOptimizationConstraint: Boolean get_IsWhenTyparTyconEqualsTycon()
FSharp.Compiler.SyntaxTree+SynStaticOptimizationConstraint: FSharp.Compiler.SyntaxTree+SynStaticOptimizationConstraint+Tags
FSharp.Compiler.SyntaxTree+SynStaticOptimizationConstraint: FSharp.Compiler.SyntaxTree+SynStaticOptimizationConstraint+WhenTyparIsStruct
FSharp.Compiler.SyntaxTree+SynStaticOptimizationConstraint: FSharp.Compiler.SyntaxTree+SynStaticOptimizationConstraint+WhenTyparTyconEqualsTycon
FSharp.Compiler.SyntaxTree+SynStaticOptimizationConstraint: Int32 Tag
FSharp.Compiler.SyntaxTree+SynStaticOptimizationConstraint: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynStaticOptimizationConstraint: SynStaticOptimizationConstraint NewWhenTyparIsStruct(SynTypar, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynStaticOptimizationConstraint: SynStaticOptimizationConstraint NewWhenTyparTyconEqualsTycon(SynTypar, SynType, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynStaticOptimizationConstraint: System.String ToString()
FSharp.Compiler.SyntaxTree+SynTypar: Boolean get_isCompGen()
FSharp.Compiler.SyntaxTree+SynTypar: Boolean isCompGen
FSharp.Compiler.SyntaxTree+SynTypar: FSharp.Compiler.Text.Range Range
FSharp.Compiler.SyntaxTree+SynTypar: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.SyntaxTree+SynTypar: Ident get_ident()
FSharp.Compiler.SyntaxTree+SynTypar: Ident ident
FSharp.Compiler.SyntaxTree+SynTypar: Int32 Tag
FSharp.Compiler.SyntaxTree+SynTypar: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynTypar: SynTypar NewTypar(Ident, TyparStaticReq, Boolean)
FSharp.Compiler.SyntaxTree+SynTypar: System.String ToString()
FSharp.Compiler.SyntaxTree+SynTypar: TyparStaticReq get_staticReq()
FSharp.Compiler.SyntaxTree+SynTypar: TyparStaticReq staticReq
FSharp.Compiler.SyntaxTree+SynTyparDecl: Int32 Tag
FSharp.Compiler.SyntaxTree+SynTyparDecl: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynTyparDecl: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList] attributes
FSharp.Compiler.SyntaxTree+SynTyparDecl: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList] get_attributes()
FSharp.Compiler.SyntaxTree+SynTyparDecl: SynTypar Item2
FSharp.Compiler.SyntaxTree+SynTyparDecl: SynTypar get_Item2()
FSharp.Compiler.SyntaxTree+SynTyparDecl: SynTyparDecl NewTyparDecl(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList], SynTypar)
FSharp.Compiler.SyntaxTree+SynTyparDecl: System.String ToString()
FSharp.Compiler.SyntaxTree+SynType+Anon: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynType+Anon: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynType+AnonRecd: Boolean get_isStruct()
FSharp.Compiler.SyntaxTree+SynType+AnonRecd: Boolean isStruct
FSharp.Compiler.SyntaxTree+SynType+AnonRecd: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynType+AnonRecd: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynType+AnonRecd: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[FSharp.Compiler.SyntaxTree+Ident,FSharp.Compiler.SyntaxTree+SynType]] fields
FSharp.Compiler.SyntaxTree+SynType+AnonRecd: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[FSharp.Compiler.SyntaxTree+Ident,FSharp.Compiler.SyntaxTree+SynType]] get_fields()
FSharp.Compiler.SyntaxTree+SynType+App: Boolean get_isPostfix()
FSharp.Compiler.SyntaxTree+SynType+App: Boolean isPostfix
FSharp.Compiler.SyntaxTree+SynType+App: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynType+App: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynType+App: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynType] get_typeArgs()
FSharp.Compiler.SyntaxTree+SynType+App: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynType] typeArgs
FSharp.Compiler.SyntaxTree+SynType+App: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Text.Range] commaRanges
FSharp.Compiler.SyntaxTree+SynType+App: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Text.Range] get_commaRanges()
FSharp.Compiler.SyntaxTree+SynType+App: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] get_greaterRange()
FSharp.Compiler.SyntaxTree+SynType+App: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] get_lessRange()
FSharp.Compiler.SyntaxTree+SynType+App: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] greaterRange
FSharp.Compiler.SyntaxTree+SynType+App: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] lessRange
FSharp.Compiler.SyntaxTree+SynType+App: SynType get_typeName()
FSharp.Compiler.SyntaxTree+SynType+App: SynType typeName
FSharp.Compiler.SyntaxTree+SynType+Array: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynType+Array: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynType+Array: Int32 get_rank()
FSharp.Compiler.SyntaxTree+SynType+Array: Int32 rank
FSharp.Compiler.SyntaxTree+SynType+Array: SynType elementType
FSharp.Compiler.SyntaxTree+SynType+Array: SynType get_elementType()
FSharp.Compiler.SyntaxTree+SynType+Fun: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynType+Fun: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynType+Fun: SynType argType
FSharp.Compiler.SyntaxTree+SynType+Fun: SynType get_argType()
FSharp.Compiler.SyntaxTree+SynType+Fun: SynType get_returnType()
FSharp.Compiler.SyntaxTree+SynType+Fun: SynType returnType
FSharp.Compiler.SyntaxTree+SynType+HashConstraint: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynType+HashConstraint: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynType+HashConstraint: SynType get_innerType()
FSharp.Compiler.SyntaxTree+SynType+HashConstraint: SynType innerType
FSharp.Compiler.SyntaxTree+SynType+LongIdent: LongIdentWithDots get_longDotId()
FSharp.Compiler.SyntaxTree+SynType+LongIdent: LongIdentWithDots longDotId
FSharp.Compiler.SyntaxTree+SynType+LongIdentApp: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynType+LongIdentApp: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynType+LongIdentApp: LongIdentWithDots get_longDotId()
FSharp.Compiler.SyntaxTree+SynType+LongIdentApp: LongIdentWithDots longDotId
FSharp.Compiler.SyntaxTree+SynType+LongIdentApp: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynType] get_typeArgs()
FSharp.Compiler.SyntaxTree+SynType+LongIdentApp: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynType] typeArgs
FSharp.Compiler.SyntaxTree+SynType+LongIdentApp: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Text.Range] commaRanges
FSharp.Compiler.SyntaxTree+SynType+LongIdentApp: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Text.Range] get_commaRanges()
FSharp.Compiler.SyntaxTree+SynType+LongIdentApp: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] get_greaterRange()
FSharp.Compiler.SyntaxTree+SynType+LongIdentApp: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] get_lessRange()
FSharp.Compiler.SyntaxTree+SynType+LongIdentApp: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] greaterRange
FSharp.Compiler.SyntaxTree+SynType+LongIdentApp: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range] lessRange
FSharp.Compiler.SyntaxTree+SynType+LongIdentApp: SynType get_typeName()
FSharp.Compiler.SyntaxTree+SynType+LongIdentApp: SynType typeName
FSharp.Compiler.SyntaxTree+SynType+MeasureDivide: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynType+MeasureDivide: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynType+MeasureDivide: SynType dividend
FSharp.Compiler.SyntaxTree+SynType+MeasureDivide: SynType divisor
FSharp.Compiler.SyntaxTree+SynType+MeasureDivide: SynType get_dividend()
FSharp.Compiler.SyntaxTree+SynType+MeasureDivide: SynType get_divisor()
FSharp.Compiler.SyntaxTree+SynType+MeasurePower: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynType+MeasurePower: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynType+MeasurePower: SynRationalConst exponent
FSharp.Compiler.SyntaxTree+SynType+MeasurePower: SynRationalConst get_exponent()
FSharp.Compiler.SyntaxTree+SynType+MeasurePower: SynType baseMeasure
FSharp.Compiler.SyntaxTree+SynType+MeasurePower: SynType get_baseMeasure()
FSharp.Compiler.SyntaxTree+SynType+Paren: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynType+Paren: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynType+Paren: SynType get_innerType()
FSharp.Compiler.SyntaxTree+SynType+Paren: SynType innerType
FSharp.Compiler.SyntaxTree+SynType+StaticConstant: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynType+StaticConstant: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynType+StaticConstant: SynConst constant
FSharp.Compiler.SyntaxTree+SynType+StaticConstant: SynConst get_constant()
FSharp.Compiler.SyntaxTree+SynType+StaticConstantExpr: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynType+StaticConstantExpr: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynType+StaticConstantExpr: SynExpr expr
FSharp.Compiler.SyntaxTree+SynType+StaticConstantExpr: SynExpr get_expr()
FSharp.Compiler.SyntaxTree+SynType+StaticConstantNamed: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynType+StaticConstantNamed: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynType+StaticConstantNamed: SynType get_ident()
FSharp.Compiler.SyntaxTree+SynType+StaticConstantNamed: SynType get_value()
FSharp.Compiler.SyntaxTree+SynType+StaticConstantNamed: SynType ident
FSharp.Compiler.SyntaxTree+SynType+StaticConstantNamed: SynType value
FSharp.Compiler.SyntaxTree+SynType+Tags: Int32 Anon
FSharp.Compiler.SyntaxTree+SynType+Tags: Int32 AnonRecd
FSharp.Compiler.SyntaxTree+SynType+Tags: Int32 App
FSharp.Compiler.SyntaxTree+SynType+Tags: Int32 Array
FSharp.Compiler.SyntaxTree+SynType+Tags: Int32 Fun
FSharp.Compiler.SyntaxTree+SynType+Tags: Int32 HashConstraint
FSharp.Compiler.SyntaxTree+SynType+Tags: Int32 LongIdent
FSharp.Compiler.SyntaxTree+SynType+Tags: Int32 LongIdentApp
FSharp.Compiler.SyntaxTree+SynType+Tags: Int32 MeasureDivide
FSharp.Compiler.SyntaxTree+SynType+Tags: Int32 MeasurePower
FSharp.Compiler.SyntaxTree+SynType+Tags: Int32 Paren
FSharp.Compiler.SyntaxTree+SynType+Tags: Int32 StaticConstant
FSharp.Compiler.SyntaxTree+SynType+Tags: Int32 StaticConstantExpr
FSharp.Compiler.SyntaxTree+SynType+Tags: Int32 StaticConstantNamed
FSharp.Compiler.SyntaxTree+SynType+Tags: Int32 Tuple
FSharp.Compiler.SyntaxTree+SynType+Tags: Int32 Var
FSharp.Compiler.SyntaxTree+SynType+Tags: Int32 WithGlobalConstraints
FSharp.Compiler.SyntaxTree+SynType+Tuple: Boolean get_isStruct()
FSharp.Compiler.SyntaxTree+SynType+Tuple: Boolean isStruct
FSharp.Compiler.SyntaxTree+SynType+Tuple: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynType+Tuple: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynType+Tuple: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[System.Boolean,FSharp.Compiler.SyntaxTree+SynType]] elementTypes
FSharp.Compiler.SyntaxTree+SynType+Tuple: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[System.Boolean,FSharp.Compiler.SyntaxTree+SynType]] get_elementTypes()
FSharp.Compiler.SyntaxTree+SynType+Var: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynType+Var: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynType+Var: SynTypar get_typar()
FSharp.Compiler.SyntaxTree+SynType+Var: SynTypar typar
FSharp.Compiler.SyntaxTree+SynType+WithGlobalConstraints: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynType+WithGlobalConstraints: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynType+WithGlobalConstraints: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynTypeConstraint] constraints
FSharp.Compiler.SyntaxTree+SynType+WithGlobalConstraints: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynTypeConstraint] get_constraints()
FSharp.Compiler.SyntaxTree+SynType+WithGlobalConstraints: SynType get_typeName()
FSharp.Compiler.SyntaxTree+SynType+WithGlobalConstraints: SynType typeName
FSharp.Compiler.SyntaxTree+SynType: Boolean IsAnon
FSharp.Compiler.SyntaxTree+SynType: Boolean IsAnonRecd
FSharp.Compiler.SyntaxTree+SynType: Boolean IsApp
FSharp.Compiler.SyntaxTree+SynType: Boolean IsArray
FSharp.Compiler.SyntaxTree+SynType: Boolean IsFun
FSharp.Compiler.SyntaxTree+SynType: Boolean IsHashConstraint
FSharp.Compiler.SyntaxTree+SynType: Boolean IsLongIdent
FSharp.Compiler.SyntaxTree+SynType: Boolean IsLongIdentApp
FSharp.Compiler.SyntaxTree+SynType: Boolean IsMeasureDivide
FSharp.Compiler.SyntaxTree+SynType: Boolean IsMeasurePower
FSharp.Compiler.SyntaxTree+SynType: Boolean IsParen
FSharp.Compiler.SyntaxTree+SynType: Boolean IsStaticConstant
FSharp.Compiler.SyntaxTree+SynType: Boolean IsStaticConstantExpr
FSharp.Compiler.SyntaxTree+SynType: Boolean IsStaticConstantNamed
FSharp.Compiler.SyntaxTree+SynType: Boolean IsTuple
FSharp.Compiler.SyntaxTree+SynType: Boolean IsVar
FSharp.Compiler.SyntaxTree+SynType: Boolean IsWithGlobalConstraints
FSharp.Compiler.SyntaxTree+SynType: Boolean get_IsAnon()
FSharp.Compiler.SyntaxTree+SynType: Boolean get_IsAnonRecd()
FSharp.Compiler.SyntaxTree+SynType: Boolean get_IsApp()
FSharp.Compiler.SyntaxTree+SynType: Boolean get_IsArray()
FSharp.Compiler.SyntaxTree+SynType: Boolean get_IsFun()
FSharp.Compiler.SyntaxTree+SynType: Boolean get_IsHashConstraint()
FSharp.Compiler.SyntaxTree+SynType: Boolean get_IsLongIdent()
FSharp.Compiler.SyntaxTree+SynType: Boolean get_IsLongIdentApp()
FSharp.Compiler.SyntaxTree+SynType: Boolean get_IsMeasureDivide()
FSharp.Compiler.SyntaxTree+SynType: Boolean get_IsMeasurePower()
FSharp.Compiler.SyntaxTree+SynType: Boolean get_IsParen()
FSharp.Compiler.SyntaxTree+SynType: Boolean get_IsStaticConstant()
FSharp.Compiler.SyntaxTree+SynType: Boolean get_IsStaticConstantExpr()
FSharp.Compiler.SyntaxTree+SynType: Boolean get_IsStaticConstantNamed()
FSharp.Compiler.SyntaxTree+SynType: Boolean get_IsTuple()
FSharp.Compiler.SyntaxTree+SynType: Boolean get_IsVar()
FSharp.Compiler.SyntaxTree+SynType: Boolean get_IsWithGlobalConstraints()
FSharp.Compiler.SyntaxTree+SynType: FSharp.Compiler.SyntaxTree+SynType+Anon
FSharp.Compiler.SyntaxTree+SynType: FSharp.Compiler.SyntaxTree+SynType+AnonRecd
FSharp.Compiler.SyntaxTree+SynType: FSharp.Compiler.SyntaxTree+SynType+App
FSharp.Compiler.SyntaxTree+SynType: FSharp.Compiler.SyntaxTree+SynType+Array
FSharp.Compiler.SyntaxTree+SynType: FSharp.Compiler.SyntaxTree+SynType+Fun
FSharp.Compiler.SyntaxTree+SynType: FSharp.Compiler.SyntaxTree+SynType+HashConstraint
FSharp.Compiler.SyntaxTree+SynType: FSharp.Compiler.SyntaxTree+SynType+LongIdent
FSharp.Compiler.SyntaxTree+SynType: FSharp.Compiler.SyntaxTree+SynType+LongIdentApp
FSharp.Compiler.SyntaxTree+SynType: FSharp.Compiler.SyntaxTree+SynType+MeasureDivide
FSharp.Compiler.SyntaxTree+SynType: FSharp.Compiler.SyntaxTree+SynType+MeasurePower
FSharp.Compiler.SyntaxTree+SynType: FSharp.Compiler.SyntaxTree+SynType+Paren
FSharp.Compiler.SyntaxTree+SynType: FSharp.Compiler.SyntaxTree+SynType+StaticConstant
FSharp.Compiler.SyntaxTree+SynType: FSharp.Compiler.SyntaxTree+SynType+StaticConstantExpr
FSharp.Compiler.SyntaxTree+SynType: FSharp.Compiler.SyntaxTree+SynType+StaticConstantNamed
FSharp.Compiler.SyntaxTree+SynType: FSharp.Compiler.SyntaxTree+SynType+Tags
FSharp.Compiler.SyntaxTree+SynType: FSharp.Compiler.SyntaxTree+SynType+Tuple
FSharp.Compiler.SyntaxTree+SynType: FSharp.Compiler.SyntaxTree+SynType+Var
FSharp.Compiler.SyntaxTree+SynType: FSharp.Compiler.SyntaxTree+SynType+WithGlobalConstraints
FSharp.Compiler.SyntaxTree+SynType: FSharp.Compiler.Text.Range Range
FSharp.Compiler.SyntaxTree+SynType: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.SyntaxTree+SynType: Int32 Tag
FSharp.Compiler.SyntaxTree+SynType: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynType: SynType NewAnon(FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynType: SynType NewAnonRecd(Boolean, Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[FSharp.Compiler.SyntaxTree+Ident,FSharp.Compiler.SyntaxTree+SynType]], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynType: SynType NewApp(SynType, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynType], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Text.Range], Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range], Boolean, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynType: SynType NewArray(Int32, SynType, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynType: SynType NewFun(SynType, SynType, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynType: SynType NewHashConstraint(SynType, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynType: SynType NewLongIdent(LongIdentWithDots)
FSharp.Compiler.SyntaxTree+SynType: SynType NewLongIdentApp(SynType, LongIdentWithDots, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynType], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.Text.Range], Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.Text.Range], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynType: SynType NewMeasureDivide(SynType, SynType, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynType: SynType NewMeasurePower(SynType, SynRationalConst, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynType: SynType NewParen(SynType, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynType: SynType NewStaticConstant(SynConst, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynType: SynType NewStaticConstantExpr(SynExpr, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynType: SynType NewStaticConstantNamed(SynType, SynType, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynType: SynType NewTuple(Boolean, Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[System.Boolean,FSharp.Compiler.SyntaxTree+SynType]], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynType: SynType NewVar(SynTypar, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynType: SynType NewWithGlobalConstraints(SynType, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynTypeConstraint], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynType: System.String ToString()
FSharp.Compiler.SyntaxTree+SynTypeConstraint+Tags: Int32 WhereTyparDefaultsToType
FSharp.Compiler.SyntaxTree+SynTypeConstraint+Tags: Int32 WhereTyparIsComparable
FSharp.Compiler.SyntaxTree+SynTypeConstraint+Tags: Int32 WhereTyparIsDelegate
FSharp.Compiler.SyntaxTree+SynTypeConstraint+Tags: Int32 WhereTyparIsEnum
FSharp.Compiler.SyntaxTree+SynTypeConstraint+Tags: Int32 WhereTyparIsEquatable
FSharp.Compiler.SyntaxTree+SynTypeConstraint+Tags: Int32 WhereTyparIsReferenceType
FSharp.Compiler.SyntaxTree+SynTypeConstraint+Tags: Int32 WhereTyparIsUnmanaged
FSharp.Compiler.SyntaxTree+SynTypeConstraint+Tags: Int32 WhereTyparIsValueType
FSharp.Compiler.SyntaxTree+SynTypeConstraint+Tags: Int32 WhereTyparSubtypeOfType
FSharp.Compiler.SyntaxTree+SynTypeConstraint+Tags: Int32 WhereTyparSupportsMember
FSharp.Compiler.SyntaxTree+SynTypeConstraint+Tags: Int32 WhereTyparSupportsNull
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparDefaultsToType: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparDefaultsToType: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparDefaultsToType: SynTypar get_typar()
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparDefaultsToType: SynTypar typar
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparDefaultsToType: SynType get_typeName()
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparDefaultsToType: SynType typeName
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparIsComparable: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparIsComparable: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparIsComparable: SynTypar get_typar()
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparIsComparable: SynTypar typar
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparIsDelegate: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparIsDelegate: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparIsDelegate: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynType] get_typeArgs()
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparIsDelegate: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynType] typeArgs
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparIsDelegate: SynTypar get_typar()
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparIsDelegate: SynTypar typar
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparIsEnum: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparIsEnum: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparIsEnum: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynType] get_typeArgs()
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparIsEnum: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynType] typeArgs
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparIsEnum: SynTypar get_typar()
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparIsEnum: SynTypar typar
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparIsEquatable: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparIsEquatable: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparIsEquatable: SynTypar get_typar()
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparIsEquatable: SynTypar typar
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparIsReferenceType: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparIsReferenceType: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparIsReferenceType: SynTypar get_typar()
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparIsReferenceType: SynTypar typar
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparIsUnmanaged: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparIsUnmanaged: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparIsUnmanaged: SynTypar get_typar()
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparIsUnmanaged: SynTypar typar
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparIsValueType: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparIsValueType: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparIsValueType: SynTypar get_typar()
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparIsValueType: SynTypar typar
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparSubtypeOfType: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparSubtypeOfType: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparSubtypeOfType: SynTypar get_typar()
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparSubtypeOfType: SynTypar typar
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparSubtypeOfType: SynType get_typeName()
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparSubtypeOfType: SynType typeName
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparSupportsMember: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparSupportsMember: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparSupportsMember: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynType] get_typars()
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparSupportsMember: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynType] typars
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparSupportsMember: SynMemberSig get_memberSig()
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparSupportsMember: SynMemberSig memberSig
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparSupportsNull: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparSupportsNull: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparSupportsNull: SynTypar get_typar()
FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparSupportsNull: SynTypar typar
FSharp.Compiler.SyntaxTree+SynTypeConstraint: Boolean IsWhereTyparDefaultsToType
FSharp.Compiler.SyntaxTree+SynTypeConstraint: Boolean IsWhereTyparIsComparable
FSharp.Compiler.SyntaxTree+SynTypeConstraint: Boolean IsWhereTyparIsDelegate
FSharp.Compiler.SyntaxTree+SynTypeConstraint: Boolean IsWhereTyparIsEnum
FSharp.Compiler.SyntaxTree+SynTypeConstraint: Boolean IsWhereTyparIsEquatable
FSharp.Compiler.SyntaxTree+SynTypeConstraint: Boolean IsWhereTyparIsReferenceType
FSharp.Compiler.SyntaxTree+SynTypeConstraint: Boolean IsWhereTyparIsUnmanaged
FSharp.Compiler.SyntaxTree+SynTypeConstraint: Boolean IsWhereTyparIsValueType
FSharp.Compiler.SyntaxTree+SynTypeConstraint: Boolean IsWhereTyparSubtypeOfType
FSharp.Compiler.SyntaxTree+SynTypeConstraint: Boolean IsWhereTyparSupportsMember
FSharp.Compiler.SyntaxTree+SynTypeConstraint: Boolean IsWhereTyparSupportsNull
FSharp.Compiler.SyntaxTree+SynTypeConstraint: Boolean get_IsWhereTyparDefaultsToType()
FSharp.Compiler.SyntaxTree+SynTypeConstraint: Boolean get_IsWhereTyparIsComparable()
FSharp.Compiler.SyntaxTree+SynTypeConstraint: Boolean get_IsWhereTyparIsDelegate()
FSharp.Compiler.SyntaxTree+SynTypeConstraint: Boolean get_IsWhereTyparIsEnum()
FSharp.Compiler.SyntaxTree+SynTypeConstraint: Boolean get_IsWhereTyparIsEquatable()
FSharp.Compiler.SyntaxTree+SynTypeConstraint: Boolean get_IsWhereTyparIsReferenceType()
FSharp.Compiler.SyntaxTree+SynTypeConstraint: Boolean get_IsWhereTyparIsUnmanaged()
FSharp.Compiler.SyntaxTree+SynTypeConstraint: Boolean get_IsWhereTyparIsValueType()
FSharp.Compiler.SyntaxTree+SynTypeConstraint: Boolean get_IsWhereTyparSubtypeOfType()
FSharp.Compiler.SyntaxTree+SynTypeConstraint: Boolean get_IsWhereTyparSupportsMember()
FSharp.Compiler.SyntaxTree+SynTypeConstraint: Boolean get_IsWhereTyparSupportsNull()
FSharp.Compiler.SyntaxTree+SynTypeConstraint: FSharp.Compiler.SyntaxTree+SynTypeConstraint+Tags
FSharp.Compiler.SyntaxTree+SynTypeConstraint: FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparDefaultsToType
FSharp.Compiler.SyntaxTree+SynTypeConstraint: FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparIsComparable
FSharp.Compiler.SyntaxTree+SynTypeConstraint: FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparIsDelegate
FSharp.Compiler.SyntaxTree+SynTypeConstraint: FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparIsEnum
FSharp.Compiler.SyntaxTree+SynTypeConstraint: FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparIsEquatable
FSharp.Compiler.SyntaxTree+SynTypeConstraint: FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparIsReferenceType
FSharp.Compiler.SyntaxTree+SynTypeConstraint: FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparIsUnmanaged
FSharp.Compiler.SyntaxTree+SynTypeConstraint: FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparIsValueType
FSharp.Compiler.SyntaxTree+SynTypeConstraint: FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparSubtypeOfType
FSharp.Compiler.SyntaxTree+SynTypeConstraint: FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparSupportsMember
FSharp.Compiler.SyntaxTree+SynTypeConstraint: FSharp.Compiler.SyntaxTree+SynTypeConstraint+WhereTyparSupportsNull
FSharp.Compiler.SyntaxTree+SynTypeConstraint: Int32 Tag
FSharp.Compiler.SyntaxTree+SynTypeConstraint: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynTypeConstraint: SynTypeConstraint NewWhereTyparDefaultsToType(SynTypar, SynType, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynTypeConstraint: SynTypeConstraint NewWhereTyparIsComparable(SynTypar, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynTypeConstraint: SynTypeConstraint NewWhereTyparIsDelegate(SynTypar, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynType], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynTypeConstraint: SynTypeConstraint NewWhereTyparIsEnum(SynTypar, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynType], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynTypeConstraint: SynTypeConstraint NewWhereTyparIsEquatable(SynTypar, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynTypeConstraint: SynTypeConstraint NewWhereTyparIsReferenceType(SynTypar, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynTypeConstraint: SynTypeConstraint NewWhereTyparIsUnmanaged(SynTypar, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynTypeConstraint: SynTypeConstraint NewWhereTyparIsValueType(SynTypar, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynTypeConstraint: SynTypeConstraint NewWhereTyparSubtypeOfType(SynTypar, SynType, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynTypeConstraint: SynTypeConstraint NewWhereTyparSupportsMember(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynType], SynMemberSig, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynTypeConstraint: SynTypeConstraint NewWhereTyparSupportsNull(SynTypar, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynTypeConstraint: System.String ToString()
FSharp.Compiler.SyntaxTree+SynTypeDefn: FSharp.Compiler.Text.Range Range
FSharp.Compiler.SyntaxTree+SynTypeDefn: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.SyntaxTree+SynTypeDefn: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynTypeDefn: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynTypeDefn: Int32 Tag
FSharp.Compiler.SyntaxTree+SynTypeDefn: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynTypeDefn: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynMemberDefn] get_members()
FSharp.Compiler.SyntaxTree+SynTypeDefn: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynMemberDefn] members
FSharp.Compiler.SyntaxTree+SynTypeDefn: SynComponentInfo get_typeInfo()
FSharp.Compiler.SyntaxTree+SynTypeDefn: SynComponentInfo typeInfo
FSharp.Compiler.SyntaxTree+SynTypeDefn: SynTypeDefn NewTypeDefn(SynComponentInfo, SynTypeDefnRepr, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynMemberDefn], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynTypeDefn: SynTypeDefnRepr get_typeRepr()
FSharp.Compiler.SyntaxTree+SynTypeDefn: SynTypeDefnRepr typeRepr
FSharp.Compiler.SyntaxTree+SynTypeDefn: System.String ToString()
FSharp.Compiler.SyntaxTree+SynTypeDefnKind+Tags: Int32 TyconAbbrev
FSharp.Compiler.SyntaxTree+SynTypeDefnKind+Tags: Int32 TyconAugmentation
FSharp.Compiler.SyntaxTree+SynTypeDefnKind+Tags: Int32 TyconClass
FSharp.Compiler.SyntaxTree+SynTypeDefnKind+Tags: Int32 TyconDelegate
FSharp.Compiler.SyntaxTree+SynTypeDefnKind+Tags: Int32 TyconHiddenRepr
FSharp.Compiler.SyntaxTree+SynTypeDefnKind+Tags: Int32 TyconILAssemblyCode
FSharp.Compiler.SyntaxTree+SynTypeDefnKind+Tags: Int32 TyconInterface
FSharp.Compiler.SyntaxTree+SynTypeDefnKind+Tags: Int32 TyconRecord
FSharp.Compiler.SyntaxTree+SynTypeDefnKind+Tags: Int32 TyconStruct
FSharp.Compiler.SyntaxTree+SynTypeDefnKind+Tags: Int32 TyconUnion
FSharp.Compiler.SyntaxTree+SynTypeDefnKind+Tags: Int32 TyconUnspecified
FSharp.Compiler.SyntaxTree+SynTypeDefnKind+TyconDelegate: SynType get_signature()
FSharp.Compiler.SyntaxTree+SynTypeDefnKind+TyconDelegate: SynType signature
FSharp.Compiler.SyntaxTree+SynTypeDefnKind+TyconDelegate: SynValInfo get_signatureInfo()
FSharp.Compiler.SyntaxTree+SynTypeDefnKind+TyconDelegate: SynValInfo signatureInfo
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: Boolean IsTyconAbbrev
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: Boolean IsTyconAugmentation
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: Boolean IsTyconClass
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: Boolean IsTyconDelegate
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: Boolean IsTyconHiddenRepr
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: Boolean IsTyconILAssemblyCode
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: Boolean IsTyconInterface
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: Boolean IsTyconRecord
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: Boolean IsTyconStruct
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: Boolean IsTyconUnion
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: Boolean IsTyconUnspecified
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: Boolean get_IsTyconAbbrev()
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: Boolean get_IsTyconAugmentation()
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: Boolean get_IsTyconClass()
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: Boolean get_IsTyconDelegate()
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: Boolean get_IsTyconHiddenRepr()
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: Boolean get_IsTyconILAssemblyCode()
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: Boolean get_IsTyconInterface()
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: Boolean get_IsTyconRecord()
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: Boolean get_IsTyconStruct()
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: Boolean get_IsTyconUnion()
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: Boolean get_IsTyconUnspecified()
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: FSharp.Compiler.SyntaxTree+SynTypeDefnKind+Tags
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: FSharp.Compiler.SyntaxTree+SynTypeDefnKind+TyconDelegate
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: Int32 Tag
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: SynTypeDefnKind NewTyconDelegate(SynType, SynValInfo)
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: SynTypeDefnKind TyconAbbrev
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: SynTypeDefnKind TyconAugmentation
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: SynTypeDefnKind TyconClass
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: SynTypeDefnKind TyconHiddenRepr
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: SynTypeDefnKind TyconILAssemblyCode
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: SynTypeDefnKind TyconInterface
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: SynTypeDefnKind TyconRecord
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: SynTypeDefnKind TyconStruct
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: SynTypeDefnKind TyconUnion
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: SynTypeDefnKind TyconUnspecified
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: SynTypeDefnKind get_TyconAbbrev()
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: SynTypeDefnKind get_TyconAugmentation()
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: SynTypeDefnKind get_TyconClass()
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: SynTypeDefnKind get_TyconHiddenRepr()
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: SynTypeDefnKind get_TyconILAssemblyCode()
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: SynTypeDefnKind get_TyconInterface()
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: SynTypeDefnKind get_TyconRecord()
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: SynTypeDefnKind get_TyconStruct()
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: SynTypeDefnKind get_TyconUnion()
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: SynTypeDefnKind get_TyconUnspecified()
FSharp.Compiler.SyntaxTree+SynTypeDefnKind: System.String ToString()
FSharp.Compiler.SyntaxTree+SynTypeDefnRepr+Exception: SynExceptionDefnRepr exnRepr
FSharp.Compiler.SyntaxTree+SynTypeDefnRepr+Exception: SynExceptionDefnRepr get_exnRepr()
FSharp.Compiler.SyntaxTree+SynTypeDefnRepr+ObjectModel: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynTypeDefnRepr+ObjectModel: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynTypeDefnRepr+ObjectModel: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynMemberDefn] get_members()
FSharp.Compiler.SyntaxTree+SynTypeDefnRepr+ObjectModel: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynMemberDefn] members
FSharp.Compiler.SyntaxTree+SynTypeDefnRepr+ObjectModel: SynTypeDefnKind get_kind()
FSharp.Compiler.SyntaxTree+SynTypeDefnRepr+ObjectModel: SynTypeDefnKind kind
FSharp.Compiler.SyntaxTree+SynTypeDefnRepr+Simple: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynTypeDefnRepr+Simple: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynTypeDefnRepr+Simple: SynTypeDefnSimpleRepr get_simpleRepr()
FSharp.Compiler.SyntaxTree+SynTypeDefnRepr+Simple: SynTypeDefnSimpleRepr simpleRepr
FSharp.Compiler.SyntaxTree+SynTypeDefnRepr+Tags: Int32 Exception
FSharp.Compiler.SyntaxTree+SynTypeDefnRepr+Tags: Int32 ObjectModel
FSharp.Compiler.SyntaxTree+SynTypeDefnRepr+Tags: Int32 Simple
FSharp.Compiler.SyntaxTree+SynTypeDefnRepr: Boolean IsException
FSharp.Compiler.SyntaxTree+SynTypeDefnRepr: Boolean IsObjectModel
FSharp.Compiler.SyntaxTree+SynTypeDefnRepr: Boolean IsSimple
FSharp.Compiler.SyntaxTree+SynTypeDefnRepr: Boolean get_IsException()
FSharp.Compiler.SyntaxTree+SynTypeDefnRepr: Boolean get_IsObjectModel()
FSharp.Compiler.SyntaxTree+SynTypeDefnRepr: Boolean get_IsSimple()
FSharp.Compiler.SyntaxTree+SynTypeDefnRepr: FSharp.Compiler.SyntaxTree+SynTypeDefnRepr+Exception
FSharp.Compiler.SyntaxTree+SynTypeDefnRepr: FSharp.Compiler.SyntaxTree+SynTypeDefnRepr+ObjectModel
FSharp.Compiler.SyntaxTree+SynTypeDefnRepr: FSharp.Compiler.SyntaxTree+SynTypeDefnRepr+Simple
FSharp.Compiler.SyntaxTree+SynTypeDefnRepr: FSharp.Compiler.SyntaxTree+SynTypeDefnRepr+Tags
FSharp.Compiler.SyntaxTree+SynTypeDefnRepr: FSharp.Compiler.Text.Range Range
FSharp.Compiler.SyntaxTree+SynTypeDefnRepr: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.SyntaxTree+SynTypeDefnRepr: Int32 Tag
FSharp.Compiler.SyntaxTree+SynTypeDefnRepr: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynTypeDefnRepr: SynTypeDefnRepr NewException(SynExceptionDefnRepr)
FSharp.Compiler.SyntaxTree+SynTypeDefnRepr: SynTypeDefnRepr NewObjectModel(SynTypeDefnKind, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynMemberDefn], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynTypeDefnRepr: SynTypeDefnRepr NewSimple(SynTypeDefnSimpleRepr, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynTypeDefnRepr: System.String ToString()
FSharp.Compiler.SyntaxTree+SynTypeDefnSig: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynTypeDefnSig: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynTypeDefnSig: Int32 Tag
FSharp.Compiler.SyntaxTree+SynTypeDefnSig: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynTypeDefnSig: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynMemberSig] get_members()
FSharp.Compiler.SyntaxTree+SynTypeDefnSig: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynMemberSig] members
FSharp.Compiler.SyntaxTree+SynTypeDefnSig: SynComponentInfo get_typeInfo()
FSharp.Compiler.SyntaxTree+SynTypeDefnSig: SynComponentInfo typeInfo
FSharp.Compiler.SyntaxTree+SynTypeDefnSig: SynTypeDefnSig NewTypeDefnSig(SynComponentInfo, SynTypeDefnSigRepr, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynMemberSig], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynTypeDefnSig: SynTypeDefnSigRepr get_typeRepr()
FSharp.Compiler.SyntaxTree+SynTypeDefnSig: SynTypeDefnSigRepr typeRepr
FSharp.Compiler.SyntaxTree+SynTypeDefnSig: System.String ToString()
FSharp.Compiler.SyntaxTree+SynTypeDefnSigRepr+Exception: SynExceptionDefnRepr Item
FSharp.Compiler.SyntaxTree+SynTypeDefnSigRepr+Exception: SynExceptionDefnRepr get_Item()
FSharp.Compiler.SyntaxTree+SynTypeDefnSigRepr+ObjectModel: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynTypeDefnSigRepr+ObjectModel: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynTypeDefnSigRepr+ObjectModel: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynMemberSig] get_memberSigs()
FSharp.Compiler.SyntaxTree+SynTypeDefnSigRepr+ObjectModel: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynMemberSig] memberSigs
FSharp.Compiler.SyntaxTree+SynTypeDefnSigRepr+ObjectModel: SynTypeDefnKind get_kind()
FSharp.Compiler.SyntaxTree+SynTypeDefnSigRepr+ObjectModel: SynTypeDefnKind kind
FSharp.Compiler.SyntaxTree+SynTypeDefnSigRepr+Simple: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynTypeDefnSigRepr+Simple: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynTypeDefnSigRepr+Simple: SynTypeDefnSimpleRepr get_repr()
FSharp.Compiler.SyntaxTree+SynTypeDefnSigRepr+Simple: SynTypeDefnSimpleRepr repr
FSharp.Compiler.SyntaxTree+SynTypeDefnSigRepr+Tags: Int32 Exception
FSharp.Compiler.SyntaxTree+SynTypeDefnSigRepr+Tags: Int32 ObjectModel
FSharp.Compiler.SyntaxTree+SynTypeDefnSigRepr+Tags: Int32 Simple
FSharp.Compiler.SyntaxTree+SynTypeDefnSigRepr: Boolean IsException
FSharp.Compiler.SyntaxTree+SynTypeDefnSigRepr: Boolean IsObjectModel
FSharp.Compiler.SyntaxTree+SynTypeDefnSigRepr: Boolean IsSimple
FSharp.Compiler.SyntaxTree+SynTypeDefnSigRepr: Boolean get_IsException()
FSharp.Compiler.SyntaxTree+SynTypeDefnSigRepr: Boolean get_IsObjectModel()
FSharp.Compiler.SyntaxTree+SynTypeDefnSigRepr: Boolean get_IsSimple()
FSharp.Compiler.SyntaxTree+SynTypeDefnSigRepr: FSharp.Compiler.SyntaxTree+SynTypeDefnSigRepr+Exception
FSharp.Compiler.SyntaxTree+SynTypeDefnSigRepr: FSharp.Compiler.SyntaxTree+SynTypeDefnSigRepr+ObjectModel
FSharp.Compiler.SyntaxTree+SynTypeDefnSigRepr: FSharp.Compiler.SyntaxTree+SynTypeDefnSigRepr+Simple
FSharp.Compiler.SyntaxTree+SynTypeDefnSigRepr: FSharp.Compiler.SyntaxTree+SynTypeDefnSigRepr+Tags
FSharp.Compiler.SyntaxTree+SynTypeDefnSigRepr: FSharp.Compiler.Text.Range Range
FSharp.Compiler.SyntaxTree+SynTypeDefnSigRepr: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.SyntaxTree+SynTypeDefnSigRepr: Int32 Tag
FSharp.Compiler.SyntaxTree+SynTypeDefnSigRepr: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynTypeDefnSigRepr: SynTypeDefnSigRepr NewException(SynExceptionDefnRepr)
FSharp.Compiler.SyntaxTree+SynTypeDefnSigRepr: SynTypeDefnSigRepr NewObjectModel(SynTypeDefnKind, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynMemberSig], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynTypeDefnSigRepr: SynTypeDefnSigRepr NewSimple(SynTypeDefnSimpleRepr, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynTypeDefnSigRepr: System.String ToString()
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+Enum: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+Enum: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+Enum: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynEnumCase] cases
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+Enum: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynEnumCase] get_cases()
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+Exception: SynExceptionDefnRepr exnRepr
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+Exception: SynExceptionDefnRepr get_exnRepr()
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+General: Boolean get_isConcrete()
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+General: Boolean get_isIncrClass()
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+General: Boolean isConcrete
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+General: Boolean isIncrClass
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+General: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+General: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+General: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynField] fields
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+General: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynField] get_fields()
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+General: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[FSharp.Compiler.SyntaxTree+SynValSig,FSharp.Compiler.SyntaxTree+MemberFlags]] get_slotsigs()
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+General: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[FSharp.Compiler.SyntaxTree+SynValSig,FSharp.Compiler.SyntaxTree+MemberFlags]] slotsigs
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+General: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`3[FSharp.Compiler.SyntaxTree+SynType,FSharp.Compiler.Text.Range,Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+Ident]]] get_inherits()
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+General: Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`3[FSharp.Compiler.SyntaxTree+SynType,FSharp.Compiler.Text.Range,Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+Ident]]] inherits
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+General: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynSimplePats] get_implicitCtorSynPats()
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+General: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynSimplePats] implicitCtorSynPats
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+General: SynTypeDefnKind get_kind()
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+General: SynTypeDefnKind kind
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+LibraryOnlyILAssembly: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+LibraryOnlyILAssembly: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+LibraryOnlyILAssembly: System.Object get_ilType()
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+LibraryOnlyILAssembly: System.Object ilType
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+None: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+None: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+Record: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+Record: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+Record: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynField] get_recordFields()
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+Record: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynField] recordFields
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+Record: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess] accessibility
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+Record: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess] get_accessibility()
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+Tags: Int32 Enum
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+Tags: Int32 Exception
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+Tags: Int32 General
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+Tags: Int32 LibraryOnlyILAssembly
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+Tags: Int32 None
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+Tags: Int32 Record
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+Tags: Int32 TypeAbbrev
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+Tags: Int32 Union
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+TypeAbbrev: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+TypeAbbrev: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+TypeAbbrev: ParserDetail detail
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+TypeAbbrev: ParserDetail get_detail()
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+TypeAbbrev: SynType get_rhsType()
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+TypeAbbrev: SynType rhsType
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+Union: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+Union: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+Union: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynUnionCase] get_unionCases()
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+Union: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynUnionCase] unionCases
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+Union: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess] accessibility
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+Union: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess] get_accessibility()
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr: Boolean IsEnum
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr: Boolean IsException
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr: Boolean IsGeneral
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr: Boolean IsLibraryOnlyILAssembly
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr: Boolean IsNone
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr: Boolean IsRecord
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr: Boolean IsTypeAbbrev
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr: Boolean IsUnion
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr: Boolean get_IsEnum()
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr: Boolean get_IsException()
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr: Boolean get_IsGeneral()
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr: Boolean get_IsLibraryOnlyILAssembly()
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr: Boolean get_IsNone()
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr: Boolean get_IsRecord()
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr: Boolean get_IsTypeAbbrev()
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr: Boolean get_IsUnion()
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr: FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+Enum
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr: FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+Exception
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr: FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+General
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr: FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+LibraryOnlyILAssembly
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr: FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+None
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr: FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+Record
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr: FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+Tags
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr: FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+TypeAbbrev
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr: FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr+Union
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr: FSharp.Compiler.Text.Range Range
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr: Int32 Tag
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr: SynTypeDefnSimpleRepr NewEnum(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynEnumCase], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr: SynTypeDefnSimpleRepr NewException(SynExceptionDefnRepr)
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr: SynTypeDefnSimpleRepr NewGeneral(SynTypeDefnKind, Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`3[FSharp.Compiler.SyntaxTree+SynType,FSharp.Compiler.Text.Range,Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+Ident]]], Microsoft.FSharp.Collections.FSharpList`1[System.Tuple`2[FSharp.Compiler.SyntaxTree+SynValSig,FSharp.Compiler.SyntaxTree+MemberFlags]], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynField], Boolean, Boolean, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynSimplePats], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr: SynTypeDefnSimpleRepr NewLibraryOnlyILAssembly(System.Object, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr: SynTypeDefnSimpleRepr NewNone(FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr: SynTypeDefnSimpleRepr NewRecord(Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynField], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr: SynTypeDefnSimpleRepr NewTypeAbbrev(ParserDetail, SynType, FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr: SynTypeDefnSimpleRepr NewUnion(Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess], Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynUnionCase], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr: System.String ToString()
FSharp.Compiler.SyntaxTree+SynUnionCase: FSharp.Compiler.Text.Range Range
FSharp.Compiler.SyntaxTree+SynUnionCase: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.SyntaxTree+SynUnionCase: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynUnionCase: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynUnionCase: Ident get_ident()
FSharp.Compiler.SyntaxTree+SynUnionCase: Ident ident
FSharp.Compiler.SyntaxTree+SynUnionCase: Int32 Tag
FSharp.Compiler.SyntaxTree+SynUnionCase: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynUnionCase: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList] attributes
FSharp.Compiler.SyntaxTree+SynUnionCase: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList] get_attributes()
FSharp.Compiler.SyntaxTree+SynUnionCase: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess] accessibility
FSharp.Compiler.SyntaxTree+SynUnionCase: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess] get_accessibility()
FSharp.Compiler.SyntaxTree+SynUnionCase: PreXmlDoc get_xmlDoc()
FSharp.Compiler.SyntaxTree+SynUnionCase: PreXmlDoc xmlDoc
FSharp.Compiler.SyntaxTree+SynUnionCase: SynUnionCase NewUnionCase(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList], Ident, SynUnionCaseType, PreXmlDoc, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynUnionCase: SynUnionCaseType caseType
FSharp.Compiler.SyntaxTree+SynUnionCase: SynUnionCaseType get_caseType()
FSharp.Compiler.SyntaxTree+SynUnionCase: System.String ToString()
FSharp.Compiler.SyntaxTree+SynUnionCaseType+Tags: Int32 UnionCaseFields
FSharp.Compiler.SyntaxTree+SynUnionCaseType+Tags: Int32 UnionCaseFullType
FSharp.Compiler.SyntaxTree+SynUnionCaseType+UnionCaseFields: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynField] cases
FSharp.Compiler.SyntaxTree+SynUnionCaseType+UnionCaseFields: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynField] get_cases()
FSharp.Compiler.SyntaxTree+SynUnionCaseType+UnionCaseFullType: SynType fullType
FSharp.Compiler.SyntaxTree+SynUnionCaseType+UnionCaseFullType: SynType get_fullType()
FSharp.Compiler.SyntaxTree+SynUnionCaseType+UnionCaseFullType: SynValInfo fullTypeInfo
FSharp.Compiler.SyntaxTree+SynUnionCaseType+UnionCaseFullType: SynValInfo get_fullTypeInfo()
FSharp.Compiler.SyntaxTree+SynUnionCaseType: Boolean IsUnionCaseFields
FSharp.Compiler.SyntaxTree+SynUnionCaseType: Boolean IsUnionCaseFullType
FSharp.Compiler.SyntaxTree+SynUnionCaseType: Boolean get_IsUnionCaseFields()
FSharp.Compiler.SyntaxTree+SynUnionCaseType: Boolean get_IsUnionCaseFullType()
FSharp.Compiler.SyntaxTree+SynUnionCaseType: FSharp.Compiler.SyntaxTree+SynUnionCaseType+Tags
FSharp.Compiler.SyntaxTree+SynUnionCaseType: FSharp.Compiler.SyntaxTree+SynUnionCaseType+UnionCaseFields
FSharp.Compiler.SyntaxTree+SynUnionCaseType: FSharp.Compiler.SyntaxTree+SynUnionCaseType+UnionCaseFullType
FSharp.Compiler.SyntaxTree+SynUnionCaseType: Int32 Tag
FSharp.Compiler.SyntaxTree+SynUnionCaseType: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynUnionCaseType: SynUnionCaseType NewUnionCaseFields(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynField])
FSharp.Compiler.SyntaxTree+SynUnionCaseType: SynUnionCaseType NewUnionCaseFullType(SynType, SynValInfo)
FSharp.Compiler.SyntaxTree+SynUnionCaseType: System.String ToString()
FSharp.Compiler.SyntaxTree+SynValData: Int32 Tag
FSharp.Compiler.SyntaxTree+SynValData: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynValData: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+Ident] get_thisIdOpt()
FSharp.Compiler.SyntaxTree+SynValData: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+Ident] thisIdOpt
FSharp.Compiler.SyntaxTree+SynValData: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+MemberFlags] get_memberFlags()
FSharp.Compiler.SyntaxTree+SynValData: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+MemberFlags] memberFlags
FSharp.Compiler.SyntaxTree+SynValData: SynValData NewSynValData(Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+MemberFlags], SynValInfo, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+Ident])
FSharp.Compiler.SyntaxTree+SynValData: SynValInfo SynValInfo
FSharp.Compiler.SyntaxTree+SynValData: SynValInfo get_SynValInfo()
FSharp.Compiler.SyntaxTree+SynValData: SynValInfo get_valInfo()
FSharp.Compiler.SyntaxTree+SynValData: SynValInfo valInfo
FSharp.Compiler.SyntaxTree+SynValData: System.String ToString()
FSharp.Compiler.SyntaxTree+SynValInfo: Int32 Tag
FSharp.Compiler.SyntaxTree+SynValInfo: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynValInfo: Microsoft.FSharp.Collections.FSharpList`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynArgInfo]] CurriedArgInfos
FSharp.Compiler.SyntaxTree+SynValInfo: Microsoft.FSharp.Collections.FSharpList`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynArgInfo]] curriedArgInfos
FSharp.Compiler.SyntaxTree+SynValInfo: Microsoft.FSharp.Collections.FSharpList`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynArgInfo]] get_CurriedArgInfos()
FSharp.Compiler.SyntaxTree+SynValInfo: Microsoft.FSharp.Collections.FSharpList`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynArgInfo]] get_curriedArgInfos()
FSharp.Compiler.SyntaxTree+SynValInfo: Microsoft.FSharp.Collections.FSharpList`1[System.String] ArgNames
FSharp.Compiler.SyntaxTree+SynValInfo: Microsoft.FSharp.Collections.FSharpList`1[System.String] get_ArgNames()
FSharp.Compiler.SyntaxTree+SynValInfo: SynArgInfo get_returnInfo()
FSharp.Compiler.SyntaxTree+SynValInfo: SynArgInfo returnInfo
FSharp.Compiler.SyntaxTree+SynValInfo: SynValInfo NewSynValInfo(Microsoft.FSharp.Collections.FSharpList`1[Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynArgInfo]], SynArgInfo)
FSharp.Compiler.SyntaxTree+SynValInfo: System.String ToString()
FSharp.Compiler.SyntaxTree+SynValSig: Boolean get_isInline()
FSharp.Compiler.SyntaxTree+SynValSig: Boolean get_isMutable()
FSharp.Compiler.SyntaxTree+SynValSig: Boolean isInline
FSharp.Compiler.SyntaxTree+SynValSig: Boolean isMutable
FSharp.Compiler.SyntaxTree+SynValSig: FSharp.Compiler.Text.Range RangeOfId
FSharp.Compiler.SyntaxTree+SynValSig: FSharp.Compiler.Text.Range get_RangeOfId()
FSharp.Compiler.SyntaxTree+SynValSig: FSharp.Compiler.Text.Range get_range()
FSharp.Compiler.SyntaxTree+SynValSig: FSharp.Compiler.Text.Range range
FSharp.Compiler.SyntaxTree+SynValSig: Ident get_ident()
FSharp.Compiler.SyntaxTree+SynValSig: Ident ident
FSharp.Compiler.SyntaxTree+SynValSig: Int32 Tag
FSharp.Compiler.SyntaxTree+SynValSig: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynValSig: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList] attributes
FSharp.Compiler.SyntaxTree+SynValSig: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList] get_attributes()
FSharp.Compiler.SyntaxTree+SynValSig: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess] accessibility
FSharp.Compiler.SyntaxTree+SynValSig: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess] get_accessibility()
FSharp.Compiler.SyntaxTree+SynValSig: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynExpr] get_synExpr()
FSharp.Compiler.SyntaxTree+SynValSig: Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynExpr] synExpr
FSharp.Compiler.SyntaxTree+SynValSig: PreXmlDoc get_xmlDoc()
FSharp.Compiler.SyntaxTree+SynValSig: PreXmlDoc xmlDoc
FSharp.Compiler.SyntaxTree+SynValSig: SynType SynType
FSharp.Compiler.SyntaxTree+SynValSig: SynType get_SynType()
FSharp.Compiler.SyntaxTree+SynValSig: SynType get_synType()
FSharp.Compiler.SyntaxTree+SynValSig: SynType synType
FSharp.Compiler.SyntaxTree+SynValSig: SynValInfo SynInfo
FSharp.Compiler.SyntaxTree+SynValSig: SynValInfo arity
FSharp.Compiler.SyntaxTree+SynValSig: SynValInfo get_SynInfo()
FSharp.Compiler.SyntaxTree+SynValSig: SynValInfo get_arity()
FSharp.Compiler.SyntaxTree+SynValSig: SynValSig NewValSpfn(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynAttributeList], Ident, SynValTyparDecls, SynType, SynValInfo, Boolean, Boolean, PreXmlDoc, Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynAccess], Microsoft.FSharp.Core.FSharpOption`1[FSharp.Compiler.SyntaxTree+SynExpr], FSharp.Compiler.Text.Range)
FSharp.Compiler.SyntaxTree+SynValSig: SynValTyparDecls explicitValDecls
FSharp.Compiler.SyntaxTree+SynValSig: SynValTyparDecls get_explicitValDecls()
FSharp.Compiler.SyntaxTree+SynValSig: System.String ToString()
FSharp.Compiler.SyntaxTree+SynValTyparDecls: Boolean canInfer
FSharp.Compiler.SyntaxTree+SynValTyparDecls: Boolean get_canInfer()
FSharp.Compiler.SyntaxTree+SynValTyparDecls: Int32 Tag
FSharp.Compiler.SyntaxTree+SynValTyparDecls: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+SynValTyparDecls: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynTyparDecl] get_typars()
FSharp.Compiler.SyntaxTree+SynValTyparDecls: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynTyparDecl] typars
FSharp.Compiler.SyntaxTree+SynValTyparDecls: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynTypeConstraint] constraints
FSharp.Compiler.SyntaxTree+SynValTyparDecls: Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynTypeConstraint] get_constraints()
FSharp.Compiler.SyntaxTree+SynValTyparDecls: SynValTyparDecls NewSynValTyparDecls(Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynTyparDecl], Boolean, Microsoft.FSharp.Collections.FSharpList`1[FSharp.Compiler.SyntaxTree+SynTypeConstraint])
FSharp.Compiler.SyntaxTree+SynValTyparDecls: System.String ToString()
FSharp.Compiler.SyntaxTree+TyparStaticReq+Tags: Int32 HeadTypeStaticReq
FSharp.Compiler.SyntaxTree+TyparStaticReq+Tags: Int32 NoStaticReq
FSharp.Compiler.SyntaxTree+TyparStaticReq: Boolean Equals(System.Object)
FSharp.Compiler.SyntaxTree+TyparStaticReq: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.SyntaxTree+TyparStaticReq: Boolean Equals(TyparStaticReq)
FSharp.Compiler.SyntaxTree+TyparStaticReq: Boolean IsHeadTypeStaticReq
FSharp.Compiler.SyntaxTree+TyparStaticReq: Boolean IsNoStaticReq
FSharp.Compiler.SyntaxTree+TyparStaticReq: Boolean get_IsHeadTypeStaticReq()
FSharp.Compiler.SyntaxTree+TyparStaticReq: Boolean get_IsNoStaticReq()
FSharp.Compiler.SyntaxTree+TyparStaticReq: FSharp.Compiler.SyntaxTree+TyparStaticReq+Tags
FSharp.Compiler.SyntaxTree+TyparStaticReq: Int32 CompareTo(System.Object)
FSharp.Compiler.SyntaxTree+TyparStaticReq: Int32 CompareTo(System.Object, System.Collections.IComparer)
FSharp.Compiler.SyntaxTree+TyparStaticReq: Int32 CompareTo(TyparStaticReq)
FSharp.Compiler.SyntaxTree+TyparStaticReq: Int32 GetHashCode()
FSharp.Compiler.SyntaxTree+TyparStaticReq: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.SyntaxTree+TyparStaticReq: Int32 Tag
FSharp.Compiler.SyntaxTree+TyparStaticReq: Int32 get_Tag()
FSharp.Compiler.SyntaxTree+TyparStaticReq: System.String ToString()
FSharp.Compiler.SyntaxTree+TyparStaticReq: TyparStaticReq HeadTypeStaticReq
FSharp.Compiler.SyntaxTree+TyparStaticReq: TyparStaticReq NoStaticReq
FSharp.Compiler.SyntaxTree+TyparStaticReq: TyparStaticReq get_HeadTypeStaticReq()
FSharp.Compiler.SyntaxTree+TyparStaticReq: TyparStaticReq get_NoStaticReq()
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+DebugPointAtFinally
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+DebugPointAtFor
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+DebugPointAtSequential
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+DebugPointAtTry
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+DebugPointAtWhile
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+DebugPointAtWith
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+DebugPointForBinding
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+DebugPointForTarget
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+ExprAtomicFlag
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+Ident
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+LongIdentWithDots
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+MemberFlags
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+MemberKind
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+ParsedFsiInteraction
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+ParsedHashDirective
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+ParsedImplFile
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+ParsedImplFileFragment
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+ParsedImplFileInput
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+ParsedInput
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+ParsedSigFile
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+ParsedSigFileFragment
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+ParsedSigFileInput
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+ParserDetail
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+QualifiedNameOfFile
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+ScopedPragma
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SeqExprOnly
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynAccess
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynArgInfo
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynArgPats
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynAttribute
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynAttributeList
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynBinding
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynBindingKind
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynBindingReturnInfo
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynComponentInfo
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynConst
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynEnumCase
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynExceptionDefn
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynExceptionDefnRepr
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynExceptionSig
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynExpr
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynField
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynIndexerArg
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynInterfaceImpl
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynInterpolatedStringPart
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynMatchClause
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynMeasure
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynMemberDefn
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynMemberSig
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynModuleDecl
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynModuleOrNamespace
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceKind
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynModuleOrNamespaceSig
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynModuleSigDecl
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynOpenDeclTarget
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynPat
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynRationalConst
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynReturnInfo
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynSimplePat
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynSimplePatAlternativeIdInfo
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynSimplePats
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynStaticOptimizationConstraint
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynTypar
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynTyparDecl
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynType
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynTypeConstraint
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynTypeDefn
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynTypeDefnKind
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynTypeDefnRepr
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynTypeDefnSig
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynTypeDefnSigRepr
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynTypeDefnSimpleRepr
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynUnionCase
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynUnionCaseType
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynValData
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynValInfo
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynValSig
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+SynValTyparDecls
FSharp.Compiler.SyntaxTree: FSharp.Compiler.SyntaxTree+TyparStaticReq
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
FSharp.Compiler.Text.Pos
FSharp.Compiler.Text.Pos: Boolean Equals(System.Object)
FSharp.Compiler.Text.Pos: FSharp.Compiler.Text.Pos Decode(Int64)
FSharp.Compiler.Text.Pos: Int32 Column
FSharp.Compiler.Text.Pos: Int32 EncodingSize
FSharp.Compiler.Text.Pos: Int32 GetHashCode()
FSharp.Compiler.Text.Pos: Int32 Line
FSharp.Compiler.Text.Pos: Int32 get_Column()
FSharp.Compiler.Text.Pos: Int32 get_EncodingSize()
FSharp.Compiler.Text.Pos: Int32 get_Line()
FSharp.Compiler.Text.Pos: Int64 Encoding
FSharp.Compiler.Text.Pos: Int64 get_Encoding()
FSharp.Compiler.Text.Pos: System.String ToString()
FSharp.Compiler.Text.PosModule
FSharp.Compiler.Text.PosModule: Boolean posEq(FSharp.Compiler.Text.Pos, FSharp.Compiler.Text.Pos)
FSharp.Compiler.Text.PosModule: Boolean posGeq(FSharp.Compiler.Text.Pos, FSharp.Compiler.Text.Pos)
FSharp.Compiler.Text.PosModule: Boolean posGt(FSharp.Compiler.Text.Pos, FSharp.Compiler.Text.Pos)
FSharp.Compiler.Text.PosModule: Boolean posLt(FSharp.Compiler.Text.Pos, FSharp.Compiler.Text.Pos)
FSharp.Compiler.Text.PosModule: FSharp.Compiler.Text.Pos fromZ(Int32, Int32)
FSharp.Compiler.Text.PosModule: FSharp.Compiler.Text.Pos get_pos0()
FSharp.Compiler.Text.PosModule: FSharp.Compiler.Text.Pos mkPos(Int32, Int32)
FSharp.Compiler.Text.PosModule: FSharp.Compiler.Text.Pos pos0
FSharp.Compiler.Text.PosModule: System.String stringOfPos(FSharp.Compiler.Text.Pos)
FSharp.Compiler.Text.PosModule: System.Tuple`2[System.Int32,System.Int32] toZ(FSharp.Compiler.Text.Pos)
FSharp.Compiler.Text.PosModule: Void outputPos(System.IO.TextWriter, FSharp.Compiler.Text.Pos)
FSharp.Compiler.Text.Range
FSharp.Compiler.Text.Range: Boolean Equals(System.Object)
FSharp.Compiler.Text.Range: Boolean IsSynthetic
FSharp.Compiler.Text.Range: Boolean get_IsSynthetic()
FSharp.Compiler.Text.Range: FSharp.Compiler.Text.Pos End
FSharp.Compiler.Text.Range: FSharp.Compiler.Text.Pos Start
FSharp.Compiler.Text.Range: FSharp.Compiler.Text.Pos get_End()
FSharp.Compiler.Text.Range: FSharp.Compiler.Text.Pos get_Start()
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
FSharp.Compiler.Text.RangeModule: Boolean rangeBeforePos(FSharp.Compiler.Text.Range, FSharp.Compiler.Text.Pos)
FSharp.Compiler.Text.RangeModule: Boolean rangeContainsPos(FSharp.Compiler.Text.Range, FSharp.Compiler.Text.Pos)
FSharp.Compiler.Text.RangeModule: Boolean rangeContainsRange(FSharp.Compiler.Text.Range, FSharp.Compiler.Text.Range)
FSharp.Compiler.Text.RangeModule: FSharp.Compiler.Text.Range get_range0()
FSharp.Compiler.Text.RangeModule: FSharp.Compiler.Text.Range get_rangeCmdArgs()
FSharp.Compiler.Text.RangeModule: FSharp.Compiler.Text.Range get_rangeStartup()
FSharp.Compiler.Text.RangeModule: FSharp.Compiler.Text.Range mkFileIndexRange(Int32, FSharp.Compiler.Text.Pos, FSharp.Compiler.Text.Pos)
FSharp.Compiler.Text.RangeModule: FSharp.Compiler.Text.Range mkFirstLineOfFile(System.String)
FSharp.Compiler.Text.RangeModule: FSharp.Compiler.Text.Range mkRange(System.String, FSharp.Compiler.Text.Pos, FSharp.Compiler.Text.Pos)
FSharp.Compiler.Text.RangeModule: FSharp.Compiler.Text.Range range0
FSharp.Compiler.Text.RangeModule: FSharp.Compiler.Text.Range rangeCmdArgs
FSharp.Compiler.Text.RangeModule: FSharp.Compiler.Text.Range rangeN(System.String, Int32)
FSharp.Compiler.Text.RangeModule: FSharp.Compiler.Text.Range rangeStartup
FSharp.Compiler.Text.RangeModule: FSharp.Compiler.Text.Range trimRangeToLine(FSharp.Compiler.Text.Range)
FSharp.Compiler.Text.RangeModule: FSharp.Compiler.Text.Range unionRanges(FSharp.Compiler.Text.Range, FSharp.Compiler.Text.Range)
FSharp.Compiler.Text.RangeModule: System.Collections.Generic.IComparer`1[FSharp.Compiler.Text.Pos] get_posOrder()
FSharp.Compiler.Text.RangeModule: System.Collections.Generic.IComparer`1[FSharp.Compiler.Text.Pos] posOrder
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
FSharp.Compiler.TextLayout.TaggedTextModule: Microsoft.FSharp.Collections.FSharpSet`1[System.String] keywordFunctions
FSharp.Compiler.XmlDoc
FSharp.Compiler.XmlDoc+PreXmlDoc: Boolean Equals(PreXmlDoc)
FSharp.Compiler.XmlDoc+PreXmlDoc: Boolean Equals(System.Object)
FSharp.Compiler.XmlDoc+PreXmlDoc: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
FSharp.Compiler.XmlDoc+PreXmlDoc: Int32 GetHashCode()
FSharp.Compiler.XmlDoc+PreXmlDoc: Int32 GetHashCode(System.Collections.IEqualityComparer)
FSharp.Compiler.XmlDoc+PreXmlDoc: PreXmlDoc Create(System.String[], FSharp.Compiler.Text.Range)
FSharp.Compiler.XmlDoc+PreXmlDoc: PreXmlDoc Empty
FSharp.Compiler.XmlDoc+PreXmlDoc: PreXmlDoc Merge(PreXmlDoc, PreXmlDoc)
FSharp.Compiler.XmlDoc+PreXmlDoc: PreXmlDoc get_Empty()
FSharp.Compiler.XmlDoc+PreXmlDoc: System.String ToString()
FSharp.Compiler.XmlDoc+PreXmlDoc: XmlDoc ToXmlDoc(Boolean, Microsoft.FSharp.Core.FSharpOption`1[Microsoft.FSharp.Collections.FSharpList`1[System.String]])
FSharp.Compiler.XmlDoc+XmlDoc: Boolean IsEmpty
FSharp.Compiler.XmlDoc+XmlDoc: Boolean NonEmpty
FSharp.Compiler.XmlDoc+XmlDoc: Boolean get_IsEmpty()
FSharp.Compiler.XmlDoc+XmlDoc: Boolean get_NonEmpty()
FSharp.Compiler.XmlDoc+XmlDoc: FSharp.Compiler.Text.Range Range
FSharp.Compiler.XmlDoc+XmlDoc: FSharp.Compiler.Text.Range get_Range()
FSharp.Compiler.XmlDoc+XmlDoc: System.String GetXmlText()
FSharp.Compiler.XmlDoc+XmlDoc: System.String[] GetElaboratedXmlLines()
FSharp.Compiler.XmlDoc+XmlDoc: System.String[] UnprocessedLines
FSharp.Compiler.XmlDoc+XmlDoc: System.String[] get_UnprocessedLines()
FSharp.Compiler.XmlDoc+XmlDoc: Void .ctor(System.String[], FSharp.Compiler.Text.Range)
FSharp.Compiler.XmlDoc+XmlDoc: XmlDoc Empty
FSharp.Compiler.XmlDoc+XmlDoc: XmlDoc Merge(XmlDoc, XmlDoc)
FSharp.Compiler.XmlDoc+XmlDoc: XmlDoc get_Empty()
FSharp.Compiler.XmlDoc: FSharp.Compiler.XmlDoc+PreXmlDoc
FSharp.Compiler.XmlDoc: FSharp.Compiler.XmlDoc+XmlDoc
Microsoft.DotNet.DependencyManager.AssemblyResolutionProbe
Microsoft.DotNet.DependencyManager.AssemblyResolutionProbe: System.Collections.Generic.IEnumerable`1[System.String] EndInvoke(System.IAsyncResult)
Microsoft.DotNet.DependencyManager.AssemblyResolutionProbe: System.Collections.Generic.IEnumerable`1[System.String] Invoke()
Microsoft.DotNet.DependencyManager.AssemblyResolutionProbe: System.IAsyncResult BeginInvoke(System.AsyncCallback, System.Object)
Microsoft.DotNet.DependencyManager.AssemblyResolutionProbe: Void .ctor(System.Object, IntPtr)
Microsoft.DotNet.DependencyManager.AssemblyResolveHandler
Microsoft.DotNet.DependencyManager.AssemblyResolveHandler: Void .ctor(Microsoft.DotNet.DependencyManager.AssemblyResolutionProbe)
Microsoft.DotNet.DependencyManager.DependencyProvider
Microsoft.DotNet.DependencyManager.DependencyProvider: Microsoft.DotNet.DependencyManager.IDependencyManagerProvider TryFindDependencyManagerByKey(System.Collections.Generic.IEnumerable`1[System.String], System.String, Microsoft.DotNet.DependencyManager.ResolvingErrorReport, System.String)
Microsoft.DotNet.DependencyManager.DependencyProvider: Microsoft.DotNet.DependencyManager.IResolveDependenciesResult Resolve(Microsoft.DotNet.DependencyManager.IDependencyManagerProvider, System.String, System.Collections.Generic.IEnumerable`1[System.Tuple`2[System.String,System.String]], Microsoft.DotNet.DependencyManager.ResolvingErrorReport, System.String, System.String, System.String, System.String, System.String, Int32)
Microsoft.DotNet.DependencyManager.DependencyProvider: System.String[] GetRegisteredDependencyManagerHelpText(System.Collections.Generic.IEnumerable`1[System.String], System.String, Microsoft.DotNet.DependencyManager.ResolvingErrorReport)
Microsoft.DotNet.DependencyManager.DependencyProvider: System.Tuple`2[System.Int32,System.String] CreatePackageManagerUnknownError(System.Collections.Generic.IEnumerable`1[System.String], System.String, System.String, Microsoft.DotNet.DependencyManager.ResolvingErrorReport)
Microsoft.DotNet.DependencyManager.DependencyProvider: System.Tuple`2[System.String,Microsoft.DotNet.DependencyManager.IDependencyManagerProvider] TryFindDependencyManagerInPath(System.Collections.Generic.IEnumerable`1[System.String], System.String, Microsoft.DotNet.DependencyManager.ResolvingErrorReport, System.String)
Microsoft.DotNet.DependencyManager.DependencyProvider: Void .ctor()
Microsoft.DotNet.DependencyManager.DependencyProvider: Void .ctor(Microsoft.DotNet.DependencyManager.AssemblyResolutionProbe, Microsoft.DotNet.DependencyManager.NativeResolutionProbe)
Microsoft.DotNet.DependencyManager.DependencyProvider: Void .ctor(Microsoft.DotNet.DependencyManager.NativeResolutionProbe)
Microsoft.DotNet.DependencyManager.ErrorReportType
Microsoft.DotNet.DependencyManager.ErrorReportType+Tags: Int32 Error
Microsoft.DotNet.DependencyManager.ErrorReportType+Tags: Int32 Warning
Microsoft.DotNet.DependencyManager.ErrorReportType: Boolean Equals(Microsoft.DotNet.DependencyManager.ErrorReportType)
Microsoft.DotNet.DependencyManager.ErrorReportType: Boolean Equals(System.Object)
Microsoft.DotNet.DependencyManager.ErrorReportType: Boolean Equals(System.Object, System.Collections.IEqualityComparer)
Microsoft.DotNet.DependencyManager.ErrorReportType: Boolean IsError
Microsoft.DotNet.DependencyManager.ErrorReportType: Boolean IsWarning
Microsoft.DotNet.DependencyManager.ErrorReportType: Boolean get_IsError()
Microsoft.DotNet.DependencyManager.ErrorReportType: Boolean get_IsWarning()
Microsoft.DotNet.DependencyManager.ErrorReportType: Int32 CompareTo(Microsoft.DotNet.DependencyManager.ErrorReportType)
Microsoft.DotNet.DependencyManager.ErrorReportType: Int32 CompareTo(System.Object)
Microsoft.DotNet.DependencyManager.ErrorReportType: Int32 CompareTo(System.Object, System.Collections.IComparer)
Microsoft.DotNet.DependencyManager.ErrorReportType: Int32 GetHashCode()
Microsoft.DotNet.DependencyManager.ErrorReportType: Int32 GetHashCode(System.Collections.IEqualityComparer)
Microsoft.DotNet.DependencyManager.ErrorReportType: Int32 Tag
Microsoft.DotNet.DependencyManager.ErrorReportType: Int32 get_Tag()
Microsoft.DotNet.DependencyManager.ErrorReportType: Microsoft.DotNet.DependencyManager.ErrorReportType Error
Microsoft.DotNet.DependencyManager.ErrorReportType: Microsoft.DotNet.DependencyManager.ErrorReportType Warning
Microsoft.DotNet.DependencyManager.ErrorReportType: Microsoft.DotNet.DependencyManager.ErrorReportType get_Error()
Microsoft.DotNet.DependencyManager.ErrorReportType: Microsoft.DotNet.DependencyManager.ErrorReportType get_Warning()
Microsoft.DotNet.DependencyManager.ErrorReportType: Microsoft.DotNet.DependencyManager.ErrorReportType+Tags
Microsoft.DotNet.DependencyManager.ErrorReportType: System.String ToString()
Microsoft.DotNet.DependencyManager.IDependencyManagerProvider
Microsoft.DotNet.DependencyManager.IDependencyManagerProvider: Microsoft.DotNet.DependencyManager.IResolveDependenciesResult ResolveDependencies(System.String, System.String, System.String, System.String, System.Collections.Generic.IEnumerable`1[System.Tuple`2[System.String,System.String]], System.String, System.String, Int32)
Microsoft.DotNet.DependencyManager.IDependencyManagerProvider: System.String Key
Microsoft.DotNet.DependencyManager.IDependencyManagerProvider: System.String Name
Microsoft.DotNet.DependencyManager.IDependencyManagerProvider: System.String get_Key()
Microsoft.DotNet.DependencyManager.IDependencyManagerProvider: System.String get_Name()
Microsoft.DotNet.DependencyManager.IDependencyManagerProvider: System.String[] HelpMessages
Microsoft.DotNet.DependencyManager.IDependencyManagerProvider: System.String[] get_HelpMessages()
Microsoft.DotNet.DependencyManager.IResolveDependenciesResult
Microsoft.DotNet.DependencyManager.IResolveDependenciesResult: Boolean Success
Microsoft.DotNet.DependencyManager.IResolveDependenciesResult: Boolean get_Success()
Microsoft.DotNet.DependencyManager.IResolveDependenciesResult: System.Collections.Generic.IEnumerable`1[System.String] Resolutions
Microsoft.DotNet.DependencyManager.IResolveDependenciesResult: System.Collections.Generic.IEnumerable`1[System.String] Roots
Microsoft.DotNet.DependencyManager.IResolveDependenciesResult: System.Collections.Generic.IEnumerable`1[System.String] SourceFiles
Microsoft.DotNet.DependencyManager.IResolveDependenciesResult: System.Collections.Generic.IEnumerable`1[System.String] get_Resolutions()
Microsoft.DotNet.DependencyManager.IResolveDependenciesResult: System.Collections.Generic.IEnumerable`1[System.String] get_Roots()
Microsoft.DotNet.DependencyManager.IResolveDependenciesResult: System.Collections.Generic.IEnumerable`1[System.String] get_SourceFiles()
Microsoft.DotNet.DependencyManager.IResolveDependenciesResult: System.String[] StdError
Microsoft.DotNet.DependencyManager.IResolveDependenciesResult: System.String[] StdOut
Microsoft.DotNet.DependencyManager.IResolveDependenciesResult: System.String[] get_StdError()
Microsoft.DotNet.DependencyManager.IResolveDependenciesResult: System.String[] get_StdOut()
Microsoft.DotNet.DependencyManager.NativeDllResolveHandler
Microsoft.DotNet.DependencyManager.NativeDllResolveHandler: Void .ctor(Microsoft.DotNet.DependencyManager.NativeResolutionProbe)
Microsoft.DotNet.DependencyManager.NativeResolutionProbe
Microsoft.DotNet.DependencyManager.NativeResolutionProbe: System.Collections.Generic.IEnumerable`1[System.String] EndInvoke(System.IAsyncResult)
Microsoft.DotNet.DependencyManager.NativeResolutionProbe: System.Collections.Generic.IEnumerable`1[System.String] Invoke()
Microsoft.DotNet.DependencyManager.NativeResolutionProbe: System.IAsyncResult BeginInvoke(System.AsyncCallback, System.Object)
Microsoft.DotNet.DependencyManager.NativeResolutionProbe: Void .ctor(System.Object, IntPtr)
Microsoft.DotNet.DependencyManager.ResolvingErrorReport
Microsoft.DotNet.DependencyManager.ResolvingErrorReport: System.IAsyncResult BeginInvoke(Microsoft.DotNet.DependencyManager.ErrorReportType, Int32, System.String, System.AsyncCallback, System.Object)
Microsoft.DotNet.DependencyManager.ResolvingErrorReport: Void .ctor(System.Object, IntPtr)
Microsoft.DotNet.DependencyManager.ResolvingErrorReport: Void EndInvoke(System.IAsyncResult)
Microsoft.DotNet.DependencyManager.ResolvingErrorReport: Void Invoke(Microsoft.DotNet.DependencyManager.ErrorReportType, Int32, System.String)"
        SurfaceArea.verify expected "netstandard" (System.IO.Path.Combine(__SOURCE_DIRECTORY__,__SOURCE_FILE__))