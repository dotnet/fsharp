// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.AbstractIL.BinaryConstants

open FSharp.Compiler.AbstractIL.IL
open Internal.Utilities.Library

[<Struct>]
type TableName(idx: int) =
    member x.Index = idx
    static member FromIndex n = TableName n

module TableNames =
    let Module = TableName 0
    let TypeRef = TableName 1
    let TypeDef = TableName 2
    let FieldPtr = TableName 3
    let Field = TableName 4
    let MethodPtr = TableName 5
    let Method = TableName 6
    let ParamPtr = TableName 7
    let Param = TableName 8
    let InterfaceImpl = TableName 9
    let MemberRef = TableName 10
    let Constant = TableName 11
    let CustomAttribute = TableName 12
    let FieldMarshal = TableName 13
    let Permission = TableName 14
    let ClassLayout = TableName 15
    let FieldLayout = TableName 16
    let StandAloneSig = TableName 17
    let EventMap = TableName 18
    let EventPtr = TableName 19
    let Event = TableName 20
    let PropertyMap = TableName 21
    let PropertyPtr = TableName 22
    let Property = TableName 23
    let MethodSemantics = TableName 24
    let MethodImpl = TableName 25
    let ModuleRef = TableName 26
    let TypeSpec = TableName 27
    let ImplMap = TableName 28
    let FieldRVA = TableName 29
    let ENCLog = TableName 30
    let ENCMap = TableName 31
    let Assembly = TableName 32
    let AssemblyProcessor = TableName 33
    let AssemblyOS = TableName 34
    let AssemblyRef = TableName 35
    let AssemblyRefProcessor = TableName 36
    let AssemblyRefOS = TableName 37
    let File = TableName 38
    let ExportedType = TableName 39
    let ManifestResource = TableName 40
    let Nested = TableName 41
    let GenericParam = TableName 42
    let MethodSpec = TableName 43
    let GenericParamConstraint = TableName 44

    let UserStrings =
        TableName 0x70 (* Special encoding of embedded UserString tokens - See 1.9 Partition III *)

/// Which tables are sorted and by which column.
//
// Sorted bit-vector as stored by CLR V1: 00fa 0133 0002 0000
// But what does this mean?  The ECMA spec does not say!
// Metainfo -schema reports sorting as shown below.
// But some sorting, e.g. EventMap does not seem to show
let sortedTableInfo =
    [
        (TableNames.InterfaceImpl, 0)
        (TableNames.Constant, 1)
        (TableNames.CustomAttribute, 0)
        (TableNames.FieldMarshal, 0)
        (TableNames.Permission, 1)
        (TableNames.ClassLayout, 2)
        (TableNames.FieldLayout, 1)
        (TableNames.MethodSemantics, 2)
        (TableNames.MethodImpl, 0)
        (TableNames.ImplMap, 1)
        (TableNames.FieldRVA, 1)
        (TableNames.Nested, 0)
        (TableNames.GenericParam, 2)
        (TableNames.GenericParamConstraint, 0)
    ]

[<Struct>]
type TypeDefOrRefTag(tag: int32) =
    member x.Tag = tag

let tdor_TypeDef = TypeDefOrRefTag 0x00
let tdor_TypeRef = TypeDefOrRefTag 0x01
let tdor_TypeSpec = TypeDefOrRefTag 0x2

let mkTypeDefOrRefOrSpecTag x =
    match x with
    | 0x00 -> tdor_TypeDef // nb. avoid reallocation
    | 0x01 -> tdor_TypeRef
    | 0x02 -> tdor_TypeSpec
    | _ -> invalidArg "x" "mkTypeDefOrRefOrSpecTag"

[<Struct>]
type HasConstantTag(tag: int32) =
    member x.Tag = tag

let hc_FieldDef = HasConstantTag 0x0
let hc_ParamDef = HasConstantTag 0x1
let hc_Property = HasConstantTag 0x2

let mkHasConstantTag x =
    match x with
    | 0x00 -> hc_FieldDef
    | 0x01 -> hc_ParamDef
    | 0x02 -> hc_Property
    | _ -> invalidArg "x" "mkHasConstantTag"

[<Struct>]
type HasCustomAttributeTag(tag: int32) =
    member x.Tag = tag

let hca_MethodDef = HasCustomAttributeTag 0x0
let hca_FieldDef = HasCustomAttributeTag 0x1
let hca_TypeRef = HasCustomAttributeTag 0x2
let hca_TypeDef = HasCustomAttributeTag 0x3
let hca_ParamDef = HasCustomAttributeTag 0x4
let hca_InterfaceImpl = HasCustomAttributeTag 0x5
let hca_MemberRef = HasCustomAttributeTag 0x6
let hca_Module = HasCustomAttributeTag 0x7
let hca_Permission = HasCustomAttributeTag 0x8
let hca_Property = HasCustomAttributeTag 0x9
let hca_Event = HasCustomAttributeTag 0xa
let hca_StandAloneSig = HasCustomAttributeTag 0xb
let hca_ModuleRef = HasCustomAttributeTag 0xc
let hca_TypeSpec = HasCustomAttributeTag 0xd
let hca_Assembly = HasCustomAttributeTag 0xe
let hca_AssemblyRef = HasCustomAttributeTag 0xf
let hca_File = HasCustomAttributeTag 0x10
let hca_ExportedType = HasCustomAttributeTag 0x11
let hca_ManifestResource = HasCustomAttributeTag 0x12
let hca_GenericParam = HasCustomAttributeTag 0x13
let hca_GenericParamConstraint = HasCustomAttributeTag 0x14
let hca_MethodSpec = HasCustomAttributeTag 0x15

let mkHasCustomAttributeTag x =
    match x with
    | 0x00 -> hca_MethodDef
    | 0x01 -> hca_FieldDef
    | 0x02 -> hca_TypeRef
    | 0x03 -> hca_TypeDef
    | 0x04 -> hca_ParamDef
    | 0x05 -> hca_InterfaceImpl
    | 0x06 -> hca_MemberRef
    | 0x07 -> hca_Module
    | 0x08 -> hca_Permission
    | 0x09 -> hca_Property
    | 0x0a -> hca_Event
    | 0x0b -> hca_StandAloneSig
    | 0x0c -> hca_ModuleRef
    | 0x0d -> hca_TypeSpec
    | 0x0e -> hca_Assembly
    | 0x0f -> hca_AssemblyRef
    | 0x10 -> hca_File
    | 0x11 -> hca_ExportedType
    | 0x12 -> hca_ManifestResource
    | 0x13 -> hca_GenericParam
    | 0x14 -> hca_GenericParamConstraint
    | 0x15 -> hca_MethodSpec
    | _ -> HasCustomAttributeTag x

[<Struct>]
type HasFieldMarshalTag(tag: int32) =
    member x.Tag = tag

let hfm_FieldDef = HasFieldMarshalTag 0x00
let hfm_ParamDef = HasFieldMarshalTag 0x01

let mkHasFieldMarshalTag x =
    match x with
    | 0x00 -> hfm_FieldDef
    | 0x01 -> hfm_ParamDef
    | _ -> HasFieldMarshalTag x

[<Struct>]
type HasDeclSecurityTag(tag: int32) =
    member x.Tag = tag

let hds_TypeDef = HasDeclSecurityTag 0x00
let hds_MethodDef = HasDeclSecurityTag 0x01
let hds_Assembly = HasDeclSecurityTag 0x02

let mkHasDeclSecurityTag x =
    match x with
    | 0x00 -> hds_TypeDef
    | 0x01 -> hds_MethodDef
    | 0x02 -> hds_Assembly
    | _ -> HasDeclSecurityTag x

[<Struct>]
type MemberRefParentTag(tag: int32) =
    member x.Tag = tag

let mrp_TypeRef = MemberRefParentTag 0x01
let mrp_ModuleRef = MemberRefParentTag 0x02
let mrp_MethodDef = MemberRefParentTag 0x03
let mrp_TypeSpec = MemberRefParentTag 0x04

let mkMemberRefParentTag x =
    match x with
    | 0x01 -> mrp_TypeRef
    | 0x02 -> mrp_ModuleRef
    | 0x03 -> mrp_MethodDef
    | 0x04 -> mrp_TypeSpec
    | _ -> MemberRefParentTag x

[<Struct>]
type HasSemanticsTag(tag: int32) =
    member x.Tag = tag

let hs_Event = HasSemanticsTag 0x00
let hs_Property = HasSemanticsTag 0x01

let mkHasSemanticsTag x =
    match x with
    | 0x00 -> hs_Event
    | 0x01 -> hs_Property
    | _ -> HasSemanticsTag x

[<Struct>]
type MethodDefOrRefTag(tag: int32) =
    member x.Tag = tag

let mdor_MethodDef = MethodDefOrRefTag 0x00
let mdor_MemberRef = MethodDefOrRefTag 0x01
let mdor_MethodSpec = MethodDefOrRefTag 0x02

let mkMethodDefOrRefTag x =
    match x with
    | 0x00 -> mdor_MethodDef
    | 0x01 -> mdor_MemberRef
    | 0x02 -> mdor_MethodSpec
    | _ -> MethodDefOrRefTag x

[<Struct>]
type MemberForwardedTag(tag: int32) =
    member x.Tag = tag

let mf_FieldDef = MemberForwardedTag 0x00
let mf_MethodDef = MemberForwardedTag 0x01

let mkMemberForwardedTag x =
    match x with
    | 0x00 -> mf_FieldDef
    | 0x01 -> mf_MethodDef
    | _ -> MemberForwardedTag x

[<Struct>]
type ImplementationTag(tag: int32) =
    member x.Tag = tag

let i_File = ImplementationTag 0x00
let i_AssemblyRef = ImplementationTag 0x01
let i_ExportedType = ImplementationTag 0x02

let mkImplementationTag x =
    match x with
    | 0x00 -> i_File
    | 0x01 -> i_AssemblyRef
    | 0x02 -> i_ExportedType
    | _ -> ImplementationTag x

[<Struct>]
type CustomAttributeTypeTag(tag: int32) =
    member x.Tag = tag

let cat_MethodDef = CustomAttributeTypeTag 0x02
let cat_MemberRef = CustomAttributeTypeTag 0x03

let mkILCustomAttributeTypeTag x =
    match x with
    | 0x02 -> cat_MethodDef
    | 0x03 -> cat_MemberRef
    | _ -> CustomAttributeTypeTag x

[<Struct>]
type ResolutionScopeTag(tag: int32) =
    member x.Tag = tag

let rs_Module = ResolutionScopeTag 0x00
let rs_ModuleRef = ResolutionScopeTag 0x01
let rs_AssemblyRef = ResolutionScopeTag 0x02
let rs_TypeRef = ResolutionScopeTag 0x03

let mkResolutionScopeTag x =
    match x with
    | 0x00 -> rs_Module
    | 0x01 -> rs_ModuleRef
    | 0x02 -> rs_AssemblyRef
    | 0x03 -> rs_TypeRef
    | _ -> ResolutionScopeTag x

[<Struct>]
type TypeOrMethodDefTag(tag: int32) =
    member x.Tag = tag

let tomd_TypeDef = TypeOrMethodDefTag 0x00
let tomd_MethodDef = TypeOrMethodDefTag 0x01

let mkTypeOrMethodDefTag x =
    match x with
    | 0x00 -> tomd_TypeDef
    | 0x01 -> tomd_MethodDef
    | _ -> TypeOrMethodDefTag x

let et_END = 0x00uy
let et_VOID = 0x01uy
let et_BOOLEAN = 0x02uy
let et_CHAR = 0x03uy
let et_I1 = 0x04uy
let et_U1 = 0x05uy
let et_I2 = 0x06uy
let et_U2 = 0x07uy
let et_I4 = 0x08uy
let et_U4 = 0x09uy
let et_I8 = 0x0Auy
let et_U8 = 0x0Buy
let et_R4 = 0x0Cuy
let et_R8 = 0x0Duy
let et_STRING = 0x0Euy
let et_PTR = 0x0Fuy
let et_BYREF = 0x10uy
let et_VALUETYPE = 0x11uy
let et_CLASS = 0x12uy
let et_VAR = 0x13uy
let et_ARRAY = 0x14uy
let et_WITH = 0x15uy
let et_TYPEDBYREF = 0x16uy
let et_I = 0x18uy
let et_U = 0x19uy
let et_FNPTR = 0x1Buy
let et_OBJECT = 0x1Cuy
let et_SZARRAY = 0x1Duy
let et_MVAR = 0x1euy
let et_CMOD_REQD = 0x1Fuy
let et_CMOD_OPT = 0x20uy

let et_SENTINEL = 0x41uy // sentinel for varargs
let et_PINNED = 0x45uy

let i_nop = 0x00
let i_break = 0x01
let i_ldarg_0 = 0x02
let i_ldarg_1 = 0x03
let i_ldarg_2 = 0x04
let i_ldarg_3 = 0x05
let i_ldloc_0 = 0x06
let i_ldloc_1 = 0x07
let i_ldloc_2 = 0x08
let i_ldloc_3 = 0x09
let i_stloc_0 = 0x0a
let i_stloc_1 = 0x0b
let i_stloc_2 = 0x0c
let i_stloc_3 = 0x0d
let i_ldarg_s = 0x0e
let i_ldarga_s = 0x0f
let i_starg_s = 0x10
let i_ldloc_s = 0x11
let i_ldloca_s = 0x12
let i_stloc_s = 0x13
let i_ldnull = 0x14
let i_ldc_i4_m1 = 0x15
let i_ldc_i4_0 = 0x16
let i_ldc_i4_1 = 0x17
let i_ldc_i4_2 = 0x18
let i_ldc_i4_3 = 0x19
let i_ldc_i4_4 = 0x1a
let i_ldc_i4_5 = 0x1b
let i_ldc_i4_6 = 0x1c
let i_ldc_i4_7 = 0x1d
let i_ldc_i4_8 = 0x1e
let i_ldc_i4_s = 0x1f
let i_ldc_i4 = 0x20
let i_ldc_i8 = 0x21
let i_ldc_r4 = 0x22
let i_ldc_r8 = 0x23
let i_dup = 0x25
let i_pop = 0x26
let i_jmp = 0x27
let i_call = 0x28
let i_calli = 0x29
let i_ret = 0x2a
let i_br_s = 0x2b
let i_brfalse_s = 0x2c
let i_brtrue_s = 0x2d
let i_beq_s = 0x2e
let i_bge_s = 0x2f
let i_bgt_s = 0x30
let i_ble_s = 0x31
let i_blt_s = 0x32
let i_bne_un_s = 0x33
let i_bge_un_s = 0x34
let i_bgt_un_s = 0x35
let i_ble_un_s = 0x36
let i_blt_un_s = 0x37
let i_br = 0x38
let i_brfalse = 0x39
let i_brtrue = 0x3a
let i_beq = 0x3b
let i_bge = 0x3c
let i_bgt = 0x3d
let i_ble = 0x3e
let i_blt = 0x3f
let i_bne_un = 0x40
let i_bge_un = 0x41
let i_bgt_un = 0x42
let i_ble_un = 0x43
let i_blt_un = 0x44
let i_switch = 0x45
let i_ldind_i1 = 0x46
let i_ldind_u1 = 0x47
let i_ldind_i2 = 0x48
let i_ldind_u2 = 0x49
let i_ldind_i4 = 0x4a
let i_ldind_u4 = 0x4b
let i_ldind_i8 = 0x4c
let i_ldind_i = 0x4d
let i_ldind_r4 = 0x4e
let i_ldind_r8 = 0x4f
let i_ldind_ref = 0x50
let i_stind_ref = 0x51
let i_stind_i1 = 0x52
let i_stind_i2 = 0x53
let i_stind_i4 = 0x54
let i_stind_i8 = 0x55
let i_stind_r4 = 0x56
let i_stind_r8 = 0x57
let i_add = 0x58
let i_sub = 0x59
let i_mul = 0x5a
let i_div = 0x5b
let i_div_un = 0x5c
let i_rem = 0x5d
let i_rem_un = 0x5e
let i_and = 0x5f
let i_or = 0x60
let i_xor = 0x61
let i_shl = 0x62
let i_shr = 0x63
let i_shr_un = 0x64
let i_neg = 0x65
let i_not = 0x66
let i_conv_i1 = 0x67
let i_conv_i2 = 0x68
let i_conv_i4 = 0x69
let i_conv_i8 = 0x6a
let i_conv_r4 = 0x6b
let i_conv_r8 = 0x6c
let i_conv_u4 = 0x6d
let i_conv_u8 = 0x6e
let i_callvirt = 0x6f
let i_cpobj = 0x70
let i_ldobj = 0x71
let i_ldstr = 0x72
let i_newobj = 0x73
let i_castclass = 0x74
let i_isinst = 0x75
let i_conv_r_un = 0x76
let i_unbox = 0x79
let i_throw = 0x7a
let i_ldfld = 0x7b
let i_ldflda = 0x7c
let i_stfld = 0x7d
let i_ldsfld = 0x7e
let i_ldsflda = 0x7f
let i_stsfld = 0x80
let i_stobj = 0x81
let i_conv_ovf_i1_un = 0x82
let i_conv_ovf_i2_un = 0x83
let i_conv_ovf_i4_un = 0x84
let i_conv_ovf_i8_un = 0x85
let i_conv_ovf_u1_un = 0x86
let i_conv_ovf_u2_un = 0x87
let i_conv_ovf_u4_un = 0x88
let i_conv_ovf_u8_un = 0x89
let i_conv_ovf_i_un = 0x8a
let i_conv_ovf_u_un = 0x8b
let i_box = 0x8c
let i_newarr = 0x8d
let i_ldlen = 0x8e
let i_ldelema = 0x8f
let i_ldelem_i1 = 0x90
let i_ldelem_u1 = 0x91
let i_ldelem_i2 = 0x92
let i_ldelem_u2 = 0x93
let i_ldelem_i4 = 0x94
let i_ldelem_u4 = 0x95
let i_ldelem_i8 = 0x96
let i_ldelem_i = 0x97
let i_ldelem_r4 = 0x98
let i_ldelem_r8 = 0x99
let i_ldelem_ref = 0x9a
let i_stelem_i = 0x9b
let i_stelem_i1 = 0x9c
let i_stelem_i2 = 0x9d
let i_stelem_i4 = 0x9e
let i_stelem_i8 = 0x9f
let i_stelem_r4 = 0xa0
let i_stelem_r8 = 0xa1
let i_stelem_ref = 0xa2
let i_conv_ovf_i1 = 0xb3
let i_conv_ovf_u1 = 0xb4
let i_conv_ovf_i2 = 0xb5
let i_conv_ovf_u2 = 0xb6
let i_conv_ovf_i4 = 0xb7
let i_conv_ovf_u4 = 0xb8
let i_conv_ovf_i8 = 0xb9
let i_conv_ovf_u8 = 0xba
let i_refanyval = 0xc2
let i_ckfinite = 0xc3
let i_mkrefany = 0xc6
let i_ldtoken = 0xd0
let i_conv_u2 = 0xd1
let i_conv_u1 = 0xd2
let i_conv_i = 0xd3
let i_conv_ovf_i = 0xd4
let i_conv_ovf_u = 0xd5
let i_add_ovf = 0xd6
let i_add_ovf_un = 0xd7
let i_mul_ovf = 0xd8
let i_mul_ovf_un = 0xd9
let i_sub_ovf = 0xda
let i_sub_ovf_un = 0xdb
let i_endfinally = 0xdc
let i_leave = 0xdd
let i_leave_s = 0xde
let i_stind_i = 0xdf
let i_conv_u = 0xe0
let i_arglist = 0xfe00
let i_ceq = 0xfe01
let i_cgt = 0xfe02
let i_cgt_un = 0xfe03
let i_clt = 0xfe04
let i_clt_un = 0xfe05
let i_ldftn = 0xfe06
let i_ldvirtftn = 0xfe07
let i_ldarg = 0xfe09
let i_ldarga = 0xfe0a
let i_starg = 0xfe0b
let i_ldloc = 0xfe0c
let i_ldloca = 0xfe0d
let i_stloc = 0xfe0e
let i_localloc = 0xfe0f
let i_endfilter = 0xfe11
let i_unaligned = 0xfe12
let i_volatile = 0xfe13
let i_constrained = 0xfe16
let i_readonly = 0xfe1e
let i_tail = 0xfe14
let i_initobj = 0xfe15
let i_cpblk = 0xfe17
let i_initblk = 0xfe18
let i_rethrow = 0xfe1a
let i_sizeof = 0xfe1c
let i_refanytype = 0xfe1d
let i_ldelem_any = 0xa3
let i_stelem_any = 0xa4
let i_unbox_any = 0xa5

let mk_ldc i = mkLdcInt32 i

let noArgInstrs =
    lazy
        [
            i_ldc_i4_0, mk_ldc 0
            i_ldc_i4_1, mk_ldc 1
            i_ldc_i4_2, mk_ldc 2
            i_ldc_i4_3, mk_ldc 3
            i_ldc_i4_4, mk_ldc 4
            i_ldc_i4_5, mk_ldc 5
            i_ldc_i4_6, mk_ldc 6
            i_ldc_i4_7, mk_ldc 7
            i_ldc_i4_8, mk_ldc 8
            i_ldc_i4_m1, mk_ldc -1
            0x0a, mkStloc 0us
            0x0b, mkStloc 1us
            0x0c, mkStloc 2us
            0x0d, mkStloc 3us
            0x06, mkLdloc 0us
            0x07, mkLdloc 1us
            0x08, mkLdloc 2us
            0x09, mkLdloc 3us
            0x02, mkLdarg 0us
            0x03, mkLdarg 1us
            0x04, mkLdarg 2us
            0x05, mkLdarg 3us
            0x2a, I_ret
            0x58, AI_add
            0xd6, AI_add_ovf
            0xd7, AI_add_ovf_un
            0x5f, AI_and
            0x5b, AI_div
            0x5c, AI_div_un
            0xfe01, AI_ceq
            0xfe02, AI_cgt
            0xfe03, AI_cgt_un
            0xfe04, AI_clt
            0xfe05, AI_clt_un
            0x67, AI_conv DT_I1
            0x68, AI_conv DT_I2
            0x69, AI_conv DT_I4
            0x6a, AI_conv DT_I8
            0xd3, AI_conv DT_I
            0x6b, AI_conv DT_R4
            0x6c, AI_conv DT_R8
            0xd2, AI_conv DT_U1
            0xd1, AI_conv DT_U2
            0x6d, AI_conv DT_U4
            0x6e, AI_conv DT_U8
            0xe0, AI_conv DT_U
            0x76, AI_conv DT_R
            0xb3, AI_conv_ovf DT_I1
            0xb5, AI_conv_ovf DT_I2
            0xb7, AI_conv_ovf DT_I4
            0xb9, AI_conv_ovf DT_I8
            0xd4, AI_conv_ovf DT_I
            0xb4, AI_conv_ovf DT_U1
            0xb6, AI_conv_ovf DT_U2
            0xb8, AI_conv_ovf DT_U4
            0xba, AI_conv_ovf DT_U8
            0xd5, AI_conv_ovf DT_U
            0x82, AI_conv_ovf_un DT_I1
            0x83, AI_conv_ovf_un DT_I2
            0x84, AI_conv_ovf_un DT_I4
            0x85, AI_conv_ovf_un DT_I8
            0x8a, AI_conv_ovf_un DT_I
            0x86, AI_conv_ovf_un DT_U1
            0x87, AI_conv_ovf_un DT_U2
            0x88, AI_conv_ovf_un DT_U4
            0x89, AI_conv_ovf_un DT_U8
            0x8b, AI_conv_ovf_un DT_U
            0x9c, I_stelem DT_I1
            0x9d, I_stelem DT_I2
            0x9e, I_stelem DT_I4
            0x9f, I_stelem DT_I8
            0xa0, I_stelem DT_R4
            0xa1, I_stelem DT_R8
            0x9b, I_stelem DT_I
            0xa2, I_stelem DT_REF
            0x90, I_ldelem DT_I1
            0x92, I_ldelem DT_I2
            0x94, I_ldelem DT_I4
            0x96, I_ldelem DT_I8
            0x91, I_ldelem DT_U1
            0x93, I_ldelem DT_U2
            0x95, I_ldelem DT_U4
            0x98, I_ldelem DT_R4
            0x99, I_ldelem DT_R8
            0x97, I_ldelem DT_I
            0x9a, I_ldelem DT_REF
            0x5a, AI_mul
            0xd8, AI_mul_ovf
            0xd9, AI_mul_ovf_un
            0x5d, AI_rem
            0x5e, AI_rem_un
            0x62, AI_shl
            0x63, AI_shr
            0x64, AI_shr_un
            0x59, AI_sub
            0xda, AI_sub_ovf
            0xdb, AI_sub_ovf_un
            0x61, AI_xor
            0x60, AI_or
            0x65, AI_neg
            0x66, AI_not
            i_ldnull, AI_ldnull
            i_dup, AI_dup
            i_pop, AI_pop
            i_ckfinite, AI_ckfinite
            i_nop, AI_nop
            i_break, I_break
            i_arglist, I_arglist
            i_endfilter, I_endfilter
            i_endfinally, I_endfinally
            i_refanytype, I_refanytype
            i_localloc, I_localloc
            i_throw, I_throw
            i_ldlen, I_ldlen
            i_rethrow, I_rethrow
        ]

let isNoArgInstr i =
    match i with
    | AI_ldc (DT_I4, ILConst.I4 n) when -1 <= n && n <= 8 -> true
    | I_stloc n
    | I_ldloc n
    | I_ldarg n when n <= 3us -> true
    | I_ret
    | AI_add
    | AI_add_ovf
    | AI_add_ovf_un
    | AI_and
    | AI_div
    | AI_div_un
    | AI_ceq
    | AI_cgt
    | AI_cgt_un
    | AI_clt
    | AI_clt_un
    | AI_conv DT_I1
    | AI_conv DT_I2
    | AI_conv DT_I4
    | AI_conv DT_I8
    | AI_conv DT_I
    | AI_conv DT_R4
    | AI_conv DT_R8
    | AI_conv DT_U1
    | AI_conv DT_U2
    | AI_conv DT_U4
    | AI_conv DT_U8
    | AI_conv DT_U
    | AI_conv DT_R
    | AI_conv_ovf DT_I1
    | AI_conv_ovf DT_I2
    | AI_conv_ovf DT_I4
    | AI_conv_ovf DT_I8
    | AI_conv_ovf DT_I
    | AI_conv_ovf DT_U1
    | AI_conv_ovf DT_U2
    | AI_conv_ovf DT_U4
    | AI_conv_ovf DT_U8
    | AI_conv_ovf DT_U
    | AI_conv_ovf_un DT_I1
    | AI_conv_ovf_un DT_I2
    | AI_conv_ovf_un DT_I4
    | AI_conv_ovf_un DT_I8
    | AI_conv_ovf_un DT_I
    | AI_conv_ovf_un DT_U1
    | AI_conv_ovf_un DT_U2
    | AI_conv_ovf_un DT_U4
    | AI_conv_ovf_un DT_U8
    | AI_conv_ovf_un DT_U
    | I_stelem DT_I1
    | I_stelem DT_I2
    | I_stelem DT_I4
    | I_stelem DT_I8
    | I_stelem DT_R4
    | I_stelem DT_R8
    | I_stelem DT_I
    | I_stelem DT_REF
    | I_ldelem DT_I1
    | I_ldelem DT_I2
    | I_ldelem DT_I4
    | I_ldelem DT_I8
    | I_ldelem DT_U1
    | I_ldelem DT_U2
    | I_ldelem DT_U4
    | I_ldelem DT_R4
    | I_ldelem DT_R8
    | I_ldelem DT_I
    | I_ldelem DT_REF
    | AI_mul
    | AI_mul_ovf
    | AI_mul_ovf_un
    | AI_rem
    | AI_rem_un
    | AI_shl
    | AI_shr
    | AI_shr_un
    | AI_sub
    | AI_sub_ovf
    | AI_sub_ovf_un
    | AI_xor
    | AI_or
    | AI_neg
    | AI_not
    | AI_ldnull
    | AI_dup
    | AI_pop
    | AI_ckfinite
    | AI_nop
    | I_break
    | I_arglist
    | I_endfilter
    | I_endfinally
    | I_refanytype
    | I_localloc
    | I_throw
    | I_ldlen
    | I_rethrow -> true
    | _ -> false

let ILCmpInstrMap =
    lazy
        (let dict = Dictionary.newWithSize 12
         dict.Add(BI_beq, i_beq)
         dict.Add(BI_bgt, i_bgt)
         dict.Add(BI_bgt_un, i_bgt_un)
         dict.Add(BI_bge, i_bge)
         dict.Add(BI_bge_un, i_bge_un)
         dict.Add(BI_ble, i_ble)
         dict.Add(BI_ble_un, i_ble_un)
         dict.Add(BI_blt, i_blt)
         dict.Add(BI_blt_un, i_blt_un)
         dict.Add(BI_bne_un, i_bne_un)
         dict.Add(BI_brfalse, i_brfalse)
         dict.Add(BI_brtrue, i_brtrue)
         dict)

let ILCmpInstrRevMap =
    lazy
        (let dict = Dictionary.newWithSize 12
         dict.Add(BI_beq, i_beq_s)
         dict.Add(BI_bgt, i_bgt_s)
         dict.Add(BI_bgt_un, i_bgt_un_s)
         dict.Add(BI_bge, i_bge_s)
         dict.Add(BI_bge_un, i_bge_un_s)
         dict.Add(BI_ble, i_ble_s)
         dict.Add(BI_ble_un, i_ble_un_s)
         dict.Add(BI_blt, i_blt_s)
         dict.Add(BI_blt_un, i_blt_un_s)
         dict.Add(BI_bne_un, i_bne_un_s)
         dict.Add(BI_brfalse, i_brfalse_s)
         dict.Add(BI_brtrue, i_brtrue_s)
         dict)

// From corhdr.h

let nt_VOID = 0x1uy
let nt_BOOLEAN = 0x2uy
let nt_I1 = 0x3uy
let nt_U1 = 0x4uy
let nt_I2 = 0x5uy
let nt_U2 = 0x6uy
let nt_I4 = 0x7uy
let nt_U4 = 0x8uy
let nt_I8 = 0x9uy
let nt_U8 = 0xAuy
let nt_R4 = 0xBuy
let nt_R8 = 0xCuy
let nt_SYSCHAR = 0xDuy
let nt_VARIANT = 0xEuy
let nt_CURRENCY = 0xFuy
let nt_PTR = 0x10uy
let nt_DECIMAL = 0x11uy
let nt_DATE = 0x12uy
let nt_BSTR = 0x13uy
let nt_LPSTR = 0x14uy
let nt_LPWSTR = 0x15uy
let nt_LPTSTR = 0x16uy
let nt_FIXEDSYSSTRING = 0x17uy
let nt_OBJECTREF = 0x18uy
let nt_IUNKNOWN = 0x19uy
let nt_IDISPATCH = 0x1Auy
let nt_STRUCT = 0x1Buy
let nt_INTF = 0x1Cuy
let nt_SAFEARRAY = 0x1Duy
let nt_FIXEDARRAY = 0x1Euy
let nt_INT = 0x1Fuy
let nt_UINT = 0x20uy
let nt_NESTEDSTRUCT = 0x21uy
let nt_BYVALSTR = 0x22uy
let nt_ANSIBSTR = 0x23uy
let nt_TBSTR = 0x24uy
let nt_VARIANTBOOL = 0x25uy
let nt_FUNC = 0x26uy
let nt_ASANY = 0x28uy
let nt_ARRAY = 0x2Auy
let nt_LPSTRUCT = 0x2Buy
let nt_CUSTOMMARSHALER = 0x2Cuy
let nt_ERROR = 0x2Duy
let nt_LPUTF8STR = 0x30uy
let nt_MAX = 0x50uy

// From c:/clrenv.i386/Crt/Inc/i386/hs.h

let vt_EMPTY = 0
let vt_NULL = 1
let vt_I2 = 2
let vt_I4 = 3
let vt_R4 = 4
let vt_R8 = 5
let vt_CY = 6
let vt_DATE = 7
let vt_BSTR = 8
let vt_DISPATCH = 9
let vt_ERROR = 10
let vt_BOOL = 11
let vt_VARIANT = 12
let vt_UNKNOWN = 13
let vt_DECIMAL = 14
let vt_I1 = 16
let vt_UI1 = 17
let vt_UI2 = 18
let vt_UI4 = 19
let vt_I8 = 20
let vt_UI8 = 21
let vt_INT = 22
let vt_UINT = 23
let vt_VOID = 24
let vt_HRESULT = 25
let vt_PTR = 26
let vt_SAFEARRAY = 27
let vt_CARRAY = 28
let vt_USERDEFINED = 29
let vt_LPSTR = 30
let vt_LPWSTR = 31
let vt_RECORD = 36
let vt_FILETIME = 64
let vt_BLOB = 65
let vt_STREAM = 66
let vt_STORAGE = 67
let vt_STREAMED_OBJECT = 68
let vt_STORED_OBJECT = 69
let vt_BLOB_OBJECT = 70
let vt_CF = 71
let vt_CLSID = 72
let vt_VECTOR = 0x1000
let vt_ARRAY = 0x2000
let vt_BYREF = 0x4000

let ILNativeTypeMap =
    lazy
        [
            nt_CURRENCY, ILNativeType.Currency
            nt_BSTR (* COM interop *) , ILNativeType.BSTR
            nt_LPSTR, ILNativeType.LPSTR
            nt_LPWSTR, ILNativeType.LPWSTR
            nt_LPTSTR, ILNativeType.LPTSTR
            nt_LPUTF8STR, ILNativeType.LPUTF8STR
            nt_IUNKNOWN (* COM interop *) , ILNativeType.IUnknown
            nt_IDISPATCH (* COM interop *) , ILNativeType.IDispatch
            nt_BYVALSTR, ILNativeType.ByValStr
            nt_TBSTR, ILNativeType.TBSTR
            nt_LPSTRUCT, ILNativeType.LPSTRUCT
            nt_INTF (* COM interop *) , ILNativeType.Interface
            nt_STRUCT, ILNativeType.Struct
            nt_ERROR (* COM interop *) , ILNativeType.Error
            nt_VOID, ILNativeType.Void
            nt_BOOLEAN, ILNativeType.Bool
            nt_I1, ILNativeType.Int8
            nt_I2, ILNativeType.Int16
            nt_I4, ILNativeType.Int32
            nt_I8, ILNativeType.Int64
            nt_R4, ILNativeType.Single
            nt_R8, ILNativeType.Double
            nt_U1, ILNativeType.Byte
            nt_U2, ILNativeType.UInt16
            nt_U4, ILNativeType.UInt32
            nt_U8, ILNativeType.UInt64
            nt_INT, ILNativeType.Int
            nt_UINT, ILNativeType.UInt
            nt_ANSIBSTR (* COM interop *) , ILNativeType.ANSIBSTR
            nt_VARIANTBOOL (* COM interop *) , ILNativeType.VariantBool
            nt_FUNC, ILNativeType.Method
            nt_ASANY, ILNativeType.AsAny
        ]

let ILNativeTypeRevMap =
    lazy (List.map (fun (x, y) -> (y, x)) (Lazy.force ILNativeTypeMap))

let ILVariantTypeMap =
    lazy
        [
            ILNativeVariant.Empty, vt_EMPTY
            ILNativeVariant.Null, vt_NULL
            ILNativeVariant.Variant, vt_VARIANT
            ILNativeVariant.Currency, vt_CY
            ILNativeVariant.Decimal, vt_DECIMAL
            ILNativeVariant.Date, vt_DATE
            ILNativeVariant.BSTR, vt_BSTR
            ILNativeVariant.LPSTR, vt_LPSTR
            ILNativeVariant.LPWSTR, vt_LPWSTR
            ILNativeVariant.IUnknown, vt_UNKNOWN
            ILNativeVariant.IDispatch, vt_DISPATCH
            ILNativeVariant.SafeArray, vt_SAFEARRAY
            ILNativeVariant.Error, vt_ERROR
            ILNativeVariant.HRESULT, vt_HRESULT
            ILNativeVariant.CArray, vt_CARRAY
            ILNativeVariant.UserDefined, vt_USERDEFINED
            ILNativeVariant.Record, vt_RECORD
            ILNativeVariant.FileTime, vt_FILETIME
            ILNativeVariant.Blob, vt_BLOB
            ILNativeVariant.Stream, vt_STREAM
            ILNativeVariant.Storage, vt_STORAGE
            ILNativeVariant.StreamedObject, vt_STREAMED_OBJECT
            ILNativeVariant.StoredObject, vt_STORED_OBJECT
            ILNativeVariant.BlobObject, vt_BLOB_OBJECT
            ILNativeVariant.CF, vt_CF
            ILNativeVariant.CLSID, vt_CLSID
            ILNativeVariant.Void, vt_VOID
            ILNativeVariant.Bool, vt_BOOL
            ILNativeVariant.Int8, vt_I1
            ILNativeVariant.Int16, vt_I2
            ILNativeVariant.Int32, vt_I4
            ILNativeVariant.Int64, vt_I8
            ILNativeVariant.Single, vt_R4
            ILNativeVariant.Double, vt_R8
            ILNativeVariant.UInt8, vt_UI1
            ILNativeVariant.UInt16, vt_UI2
            ILNativeVariant.UInt32, vt_UI4
            ILNativeVariant.UInt64, vt_UI8
            ILNativeVariant.PTR, vt_PTR
            ILNativeVariant.Int, vt_INT
            ILNativeVariant.UInt, vt_UINT
        ]

let ILVariantTypeRevMap =
    lazy (List.map (fun (x, y) -> (y, x)) (Lazy.force ILVariantTypeMap))

let ILSecurityActionMap =
    lazy
        [
            ILSecurityAction.Request, 0x0001
            ILSecurityAction.Demand, 0x0002
            ILSecurityAction.Assert, 0x0003
            ILSecurityAction.Deny, 0x0004
            ILSecurityAction.PermitOnly, 0x0005
            ILSecurityAction.LinkCheck, 0x0006
            ILSecurityAction.InheritCheck, 0x0007
            ILSecurityAction.ReqMin, 0x0008
            ILSecurityAction.ReqOpt, 0x0009
            ILSecurityAction.ReqRefuse, 0x000a
            ILSecurityAction.PreJitGrant, 0x000b
            ILSecurityAction.PreJitDeny, 0x000c
            ILSecurityAction.NonCasDemand, 0x000d
            ILSecurityAction.NonCasLinkDemand, 0x000e
            ILSecurityAction.NonCasInheritance, 0x000f
            ILSecurityAction.LinkDemandChoice, 0x0010
            ILSecurityAction.InheritanceDemandChoice, 0x0011
            ILSecurityAction.DemandChoice, 0x0012
        ]

let ILSecurityActionRevMap =
    lazy (List.map (fun (x, y) -> (y, x)) (Lazy.force ILSecurityActionMap))

let e_CorILMethod_TinyFormat = 0x02uy
let e_CorILMethod_FatFormat = 0x03uy
let e_CorILMethod_FormatMask = 0x03uy
let e_CorILMethod_MoreSects = 0x08uy
let e_CorILMethod_InitLocals = 0x10uy

let e_CorILMethod_Sect_EHTable = 0x1uy
let e_CorILMethod_Sect_FatFormat = 0x40uy
let e_CorILMethod_Sect_MoreSects = 0x80uy

let e_COR_ILEXCEPTION_CLAUSE_EXCEPTION = 0x0
let e_COR_ILEXCEPTION_CLAUSE_FILTER = 0x1
let e_COR_ILEXCEPTION_CLAUSE_FINALLY = 0x2
let e_COR_ILEXCEPTION_CLAUSE_FAULT = 0x4

let e_IMAGE_CEE_CS_CALLCONV_FASTCALL = 0x04uy
let e_IMAGE_CEE_CS_CALLCONV_STDCALL = 0x02uy
let e_IMAGE_CEE_CS_CALLCONV_THISCALL = 0x03uy
let e_IMAGE_CEE_CS_CALLCONV_CDECL = 0x01uy
let e_IMAGE_CEE_CS_CALLCONV_VARARG = 0x05uy
let e_IMAGE_CEE_CS_CALLCONV_FIELD = 0x06uy
let e_IMAGE_CEE_CS_CALLCONV_LOCAL_SIG = 0x07uy
let e_IMAGE_CEE_CS_CALLCONV_PROPERTY = 0x08uy

let e_IMAGE_CEE_CS_CALLCONV_GENERICINST = 0x0auy
let e_IMAGE_CEE_CS_CALLCONV_GENERIC = 0x10uy
let e_IMAGE_CEE_CS_CALLCONV_INSTANCE = 0x20uy
let e_IMAGE_CEE_CS_CALLCONV_INSTANCE_EXPLICIT = 0x40uy
