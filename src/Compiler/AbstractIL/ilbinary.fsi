// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Compiler use only.  Code and constants shared between binary reader/writer.
module internal FSharp.Compiler.AbstractIL.BinaryConstants

open FSharp.Compiler.AbstractIL.IL

[<Struct>]
type TableName =
    member Index: int
    static member FromIndex: int -> TableName

module TableNames =
    val Module: TableName
    val TypeRef: TableName
    val TypeDef: TableName
    val FieldPtr: TableName
    val Field: TableName
    val MethodPtr: TableName
    val Method: TableName
    val ParamPtr: TableName
    val Param: TableName
    val InterfaceImpl: TableName
    val MemberRef: TableName
    val Constant: TableName
    val CustomAttribute: TableName
    val FieldMarshal: TableName
    val Permission: TableName
    val ClassLayout: TableName
    val FieldLayout: TableName
    val StandAloneSig: TableName
    val EventMap: TableName
    val EventPtr: TableName
    val Event: TableName
    val PropertyMap: TableName
    val PropertyPtr: TableName
    val Property: TableName
    val MethodSemantics: TableName
    val MethodImpl: TableName
    val ModuleRef: TableName
    val TypeSpec: TableName
    val ImplMap: TableName
    val FieldRVA: TableName
    val ENCLog: TableName
    val ENCMap: TableName
    val Assembly: TableName
    val AssemblyProcessor: TableName
    val AssemblyOS: TableName
    val AssemblyRef: TableName
    val AssemblyRefProcessor: TableName
    val AssemblyRefOS: TableName
    val File: TableName
    val ExportedType: TableName
    val ManifestResource: TableName
    val Nested: TableName
    val GenericParam: TableName
    val GenericParamConstraint: TableName
    val MethodSpec: TableName
    val UserStrings: TableName

val sortedTableInfo: (TableName * int) list

[<Struct>]
type TypeDefOrRefTag =
    member Tag: int32

val tdor_TypeDef: TypeDefOrRefTag
val tdor_TypeRef: TypeDefOrRefTag
val tdor_TypeSpec: TypeDefOrRefTag

[<Struct>]
type HasConstantTag =
    member Tag: int32

val hc_FieldDef: HasConstantTag
val hc_ParamDef: HasConstantTag
val hc_Property: HasConstantTag

[<Struct>]
type HasCustomAttributeTag =
    member Tag: int32

val hca_MethodDef: HasCustomAttributeTag
val hca_FieldDef: HasCustomAttributeTag
val hca_TypeRef: HasCustomAttributeTag
val hca_TypeDef: HasCustomAttributeTag
val hca_ParamDef: HasCustomAttributeTag
val hca_InterfaceImpl: HasCustomAttributeTag
val hca_MemberRef: HasCustomAttributeTag
val hca_Module: HasCustomAttributeTag
val hca_Permission: HasCustomAttributeTag
val hca_Property: HasCustomAttributeTag
val hca_GenericParam: HasCustomAttributeTag
val hca_Event: HasCustomAttributeTag
val hca_StandAloneSig: HasCustomAttributeTag
val hca_ModuleRef: HasCustomAttributeTag
val hca_TypeSpec: HasCustomAttributeTag
val hca_Assembly: HasCustomAttributeTag
val hca_AssemblyRef: HasCustomAttributeTag
val hca_File: HasCustomAttributeTag
val hca_ExportedType: HasCustomAttributeTag
val hca_ManifestResource: HasCustomAttributeTag

[<Struct>]
type HasFieldMarshalTag =
    member Tag: int32

val hfm_FieldDef: HasFieldMarshalTag
val hfm_ParamDef: HasFieldMarshalTag


[<Struct>]
type HasDeclSecurityTag =
    member Tag: int32

val hds_TypeDef: HasDeclSecurityTag
val hds_MethodDef: HasDeclSecurityTag
val hds_Assembly: HasDeclSecurityTag


[<Struct>]
type MemberRefParentTag =
    member Tag: int32

val mrp_TypeRef: MemberRefParentTag
val mrp_ModuleRef: MemberRefParentTag
val mrp_MethodDef: MemberRefParentTag
val mrp_TypeSpec: MemberRefParentTag


[<Struct>]
type HasSemanticsTag =
    member Tag: int32

val hs_Event: HasSemanticsTag
val hs_Property: HasSemanticsTag


[<Struct>]
type MethodDefOrRefTag =
    member Tag: int32

val mdor_MethodDef: MethodDefOrRefTag
val mdor_MemberRef: MethodDefOrRefTag


[<Struct>]
type MemberForwardedTag =
    member Tag: int32

val mf_FieldDef: MemberForwardedTag
val mf_MethodDef: MemberForwardedTag


[<Struct>]
type ImplementationTag =
    member Tag: int32

val i_File: ImplementationTag
val i_AssemblyRef: ImplementationTag
val i_ExportedType: ImplementationTag

[<Struct>]
type CustomAttributeTypeTag =
    member Tag: int32

val cat_MethodDef: CustomAttributeTypeTag
val cat_MemberRef: CustomAttributeTypeTag

[<Struct>]
type ResolutionScopeTag =
    member Tag: int32

val rs_Module: ResolutionScopeTag
val rs_ModuleRef: ResolutionScopeTag
val rs_AssemblyRef: ResolutionScopeTag
val rs_TypeRef: ResolutionScopeTag

[<Struct>]
type TypeOrMethodDefTag =
    member Tag: int32

val tomd_TypeDef: TypeOrMethodDefTag
val tomd_MethodDef: TypeOrMethodDefTag

val mkTypeDefOrRefOrSpecTag: int32 -> TypeDefOrRefTag
val mkHasConstantTag: int32 -> HasConstantTag
val mkHasCustomAttributeTag: int32 -> HasCustomAttributeTag
val mkHasFieldMarshalTag: int32 -> HasFieldMarshalTag
val mkHasDeclSecurityTag: int32 -> HasDeclSecurityTag
val mkMemberRefParentTag: int32 -> MemberRefParentTag
val mkHasSemanticsTag: int32 -> HasSemanticsTag
val mkMethodDefOrRefTag: int32 -> MethodDefOrRefTag
val mkMemberForwardedTag: int32 -> MemberForwardedTag
val mkImplementationTag: int32 -> ImplementationTag
val mkILCustomAttributeTypeTag: int32 -> CustomAttributeTypeTag
val mkResolutionScopeTag: int32 -> ResolutionScopeTag
val mkTypeOrMethodDefTag: int32 -> TypeOrMethodDefTag

val et_END: byte
val et_VOID: byte
val et_BOOLEAN: byte
val et_CHAR: byte
val et_I1: byte
val et_U1: byte
val et_I2: byte
val et_U2: byte
val et_I4: byte
val et_U4: byte
val et_I8: byte
val et_U8: byte
val et_R4: byte
val et_R8: byte
val et_STRING: byte
val et_PTR: byte
val et_BYREF: byte
val et_VALUETYPE: byte
val et_CLASS: byte
val et_VAR: byte
val et_ARRAY: byte
val et_WITH: byte
val et_TYPEDBYREF: byte
val et_I: byte
val et_U: byte
val et_FNPTR: byte
val et_OBJECT: byte
val et_SZARRAY: byte
val et_MVAR: byte
val et_CMOD_REQD: byte
val et_CMOD_OPT: byte
val et_SENTINEL: byte
val et_PINNED: byte
val i_nop: int
val i_break: int
val i_ldarg_0: int
val i_ldarg_1: int
val i_ldarg_2: int
val i_ldarg_3: int
val i_ldloc_0: int
val i_ldloc_1: int
val i_ldloc_2: int
val i_ldloc_3: int
val i_stloc_0: int
val i_stloc_1: int
val i_stloc_2: int
val i_stloc_3: int
val i_ldarg_s: int
val i_ldarga_s: int
val i_starg_s: int
val i_ldloc_s: int
val i_ldloca_s: int
val i_stloc_s: int
val i_ldnull: int
val i_ldc_i4_m1: int
val i_ldc_i4_0: int
val i_ldc_i4_1: int
val i_ldc_i4_2: int
val i_ldc_i4_3: int
val i_ldc_i4_4: int
val i_ldc_i4_5: int
val i_ldc_i4_6: int
val i_ldc_i4_7: int
val i_ldc_i4_8: int
val i_ldc_i4_s: int
val i_ldc_i4: int
val i_ldc_i8: int
val i_ldc_r4: int
val i_ldc_r8: int
val i_dup: int
val i_pop: int
val i_jmp: int
val i_call: int
val i_calli: int
val i_ret: int
val i_br_s: int
val i_brfalse_s: int
val i_brtrue_s: int
val i_beq_s: int
val i_bge_s: int
val i_bgt_s: int
val i_ble_s: int
val i_blt_s: int
val i_bne_un_s: int
val i_bge_un_s: int
val i_bgt_un_s: int
val i_ble_un_s: int
val i_blt_un_s: int
val i_br: int
val i_brfalse: int
val i_brtrue: int
val i_beq: int
val i_bge: int
val i_bgt: int
val i_ble: int
val i_blt: int
val i_bne_un: int
val i_bge_un: int
val i_bgt_un: int
val i_ble_un: int
val i_blt_un: int
val i_switch: int
val i_ldind_i1: int
val i_ldind_u1: int
val i_ldind_i2: int
val i_ldind_u2: int
val i_ldind_i4: int
val i_ldind_u4: int
val i_ldind_i8: int
val i_ldind_i: int
val i_ldind_r4: int
val i_ldind_r8: int
val i_ldind_ref: int
val i_stind_ref: int
val i_stind_i1: int
val i_stind_i2: int
val i_stind_i4: int
val i_stind_i8: int
val i_stind_r4: int
val i_stind_r8: int
val i_add: int
val i_sub: int
val i_mul: int
val i_div: int
val i_div_un: int
val i_rem: int
val i_rem_un: int
val i_and: int
val i_or: int
val i_xor: int
val i_shl: int
val i_shr: int
val i_shr_un: int
val i_neg: int
val i_not: int
val i_conv_i1: int
val i_conv_i2: int
val i_conv_i4: int
val i_conv_i8: int
val i_conv_r4: int
val i_conv_r8: int
val i_conv_u4: int
val i_conv_u8: int
val i_callvirt: int
val i_cpobj: int
val i_ldobj: int
val i_ldstr: int
val i_newobj: int
val i_castclass: int
val i_isinst: int
val i_conv_r_un: int
val i_unbox: int
val i_throw: int
val i_ldfld: int
val i_ldflda: int
val i_stfld: int
val i_ldsfld: int
val i_ldsflda: int
val i_stsfld: int
val i_stobj: int
val i_conv_ovf_i1_un: int
val i_conv_ovf_i2_un: int
val i_conv_ovf_i4_un: int
val i_conv_ovf_i8_un: int
val i_conv_ovf_u1_un: int
val i_conv_ovf_u2_un: int
val i_conv_ovf_u4_un: int
val i_conv_ovf_u8_un: int
val i_conv_ovf_i_un: int
val i_conv_ovf_u_un: int
val i_box: int
val i_newarr: int
val i_ldlen: int
val i_ldelema: int
val i_ldelem_i1: int
val i_ldelem_u1: int
val i_ldelem_i2: int
val i_ldelem_u2: int
val i_ldelem_i4: int
val i_ldelem_u4: int
val i_ldelem_i8: int
val i_ldelem_i: int
val i_ldelem_r4: int
val i_ldelem_r8: int
val i_ldelem_ref: int
val i_stelem_i: int
val i_stelem_i1: int
val i_stelem_i2: int
val i_stelem_i4: int
val i_stelem_i8: int
val i_stelem_r4: int
val i_stelem_r8: int
val i_stelem_ref: int
val i_conv_ovf_i1: int
val i_conv_ovf_u1: int
val i_conv_ovf_i2: int
val i_conv_ovf_u2: int
val i_conv_ovf_i4: int
val i_conv_ovf_u4: int
val i_conv_ovf_i8: int
val i_conv_ovf_u8: int
val i_refanyval: int
val i_ckfinite: int
val i_mkrefany: int
val i_ldtoken: int
val i_conv_u2: int
val i_conv_u1: int
val i_conv_i: int
val i_conv_ovf_i: int
val i_conv_ovf_u: int
val i_add_ovf: int
val i_add_ovf_un: int
val i_mul_ovf: int
val i_mul_ovf_un: int
val i_sub_ovf: int
val i_sub_ovf_un: int
val i_endfinally: int
val i_leave: int
val i_leave_s: int
val i_stind_i: int
val i_conv_u: int
val i_arglist: int
val i_ceq: int
val i_cgt: int
val i_cgt_un: int
val i_clt: int
val i_clt_un: int
val i_ldftn: int
val i_ldvirtftn: int
val i_ldarg: int
val i_ldarga: int
val i_starg: int
val i_ldloc: int
val i_ldloca: int
val i_stloc: int
val i_localloc: int
val i_endfilter: int
val i_unaligned: int
val i_volatile: int
val i_constrained: int
val i_readonly: int
val i_tail: int
val i_initobj: int
val i_cpblk: int
val i_initblk: int
val i_rethrow: int
val i_sizeof: int
val i_refanytype: int
val i_ldelem_any: int
val i_stelem_any: int
val i_unbox_any: int
val noArgInstrs: Lazy<(int * ILInstr) list>
val isNoArgInstr: ILInstr -> bool
val ILCmpInstrMap: Lazy<System.Collections.Generic.Dictionary<ILComparisonInstr, int>>
val ILCmpInstrRevMap: Lazy<System.Collections.Generic.Dictionary<ILComparisonInstr, int>>
val nt_VOID: byte
val nt_BOOLEAN: byte
val nt_I1: byte
val nt_U1: byte
val nt_I2: byte
val nt_U2: byte
val nt_I4: byte
val nt_U4: byte
val nt_I8: byte
val nt_U8: byte
val nt_R4: byte
val nt_R8: byte
val nt_SYSCHAR: byte
val nt_VARIANT: byte
val nt_CURRENCY: byte
val nt_PTR: byte
val nt_DECIMAL: byte
val nt_DATE: byte
val nt_BSTR: byte
val nt_LPSTR: byte
val nt_LPWSTR: byte
val nt_LPTSTR: byte
val nt_FIXEDSYSSTRING: byte
val nt_OBJECTREF: byte
val nt_IUNKNOWN: byte
val nt_IDISPATCH: byte
val nt_STRUCT: byte
val nt_INTF: byte
val nt_SAFEARRAY: byte
val nt_FIXEDARRAY: byte
val nt_INT: byte
val nt_UINT: byte
val nt_NESTEDSTRUCT: byte
val nt_BYVALSTR: byte
val nt_ANSIBSTR: byte
val nt_TBSTR: byte
val nt_VARIANTBOOL: byte
val nt_FUNC: byte
val nt_ASANY: byte
val nt_ARRAY: byte
val nt_LPSTRUCT: byte
val nt_CUSTOMMARSHALER: byte
val nt_ERROR: byte
val nt_MAX: byte
val vt_EMPTY: int32
val vt_NULL: int32
val vt_I2: int32
val vt_I4: int32
val vt_R4: int32
val vt_R8: int32
val vt_CY: int32
val vt_DATE: int32
val vt_BSTR: int32
val vt_DISPATCH: int32
val vt_ERROR: int32
val vt_BOOL: int32
val vt_VARIANT: int32
val vt_UNKNOWN: int32
val vt_DECIMAL: int32
val vt_I1: int32
val vt_UI1: int32
val vt_UI2: int32
val vt_UI4: int32
val vt_I8: int32
val vt_UI8: int32
val vt_INT: int32
val vt_UINT: int32
val vt_VOID: int32
val vt_HRESULT: int32
val vt_PTR: int32
val vt_SAFEARRAY: int32
val vt_CARRAY: int32
val vt_USERDEFINED: int32
val vt_LPSTR: int32
val vt_LPWSTR: int32
val vt_RECORD: int32
val vt_FILETIME: int32
val vt_BLOB: int32
val vt_STREAM: int32
val vt_STORAGE: int32
val vt_STREAMED_OBJECT: int32
val vt_STORED_OBJECT: int32
val vt_BLOB_OBJECT: int32
val vt_CF: int32
val vt_CLSID: int32
val vt_VECTOR: int32
val vt_ARRAY: int32
val vt_BYREF: int32
val ILNativeTypeMap: Lazy<(byte * ILNativeType) list>
val ILNativeTypeRevMap: Lazy<(ILNativeType * byte) list>
val ILVariantTypeMap: Lazy<(ILNativeVariant * int32) list>
val ILVariantTypeRevMap: Lazy<(int32 * ILNativeVariant) list>
val ILSecurityActionMap: Lazy<(ILSecurityAction * int) list>
val ILSecurityActionRevMap: Lazy<(int * ILSecurityAction) list>
val e_CorILMethod_TinyFormat: byte
val e_CorILMethod_FatFormat: byte
val e_CorILMethod_FormatMask: byte
val e_CorILMethod_MoreSects: byte
val e_CorILMethod_InitLocals: byte
val e_CorILMethod_Sect_EHTable: byte
val e_CorILMethod_Sect_FatFormat: byte
val e_CorILMethod_Sect_MoreSects: byte
val e_COR_ILEXCEPTION_CLAUSE_EXCEPTION: int
val e_COR_ILEXCEPTION_CLAUSE_FILTER: int
val e_COR_ILEXCEPTION_CLAUSE_FINALLY: int
val e_COR_ILEXCEPTION_CLAUSE_FAULT: int

val e_IMAGE_CEE_CS_CALLCONV_FASTCALL: byte
val e_IMAGE_CEE_CS_CALLCONV_STDCALL: byte
val e_IMAGE_CEE_CS_CALLCONV_THISCALL: byte
val e_IMAGE_CEE_CS_CALLCONV_CDECL: byte
val e_IMAGE_CEE_CS_CALLCONV_VARARG: byte

val e_IMAGE_CEE_CS_CALLCONV_FIELD: byte
val e_IMAGE_CEE_CS_CALLCONV_LOCAL_SIG: byte
val e_IMAGE_CEE_CS_CALLCONV_GENERICINST: byte
val e_IMAGE_CEE_CS_CALLCONV_PROPERTY: byte

val e_IMAGE_CEE_CS_CALLCONV_INSTANCE: byte
val e_IMAGE_CEE_CS_CALLCONV_INSTANCE_EXPLICIT: byte
val e_IMAGE_CEE_CS_CALLCONV_GENERIC: byte
