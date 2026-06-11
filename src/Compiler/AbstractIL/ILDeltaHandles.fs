// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// F# types and utilities for hot reload delta metadata emission.
///
/// These handles/coded-index unions are intentionally delta-owned to keep the
/// hot-reload pipeline isolated from broad mainline signature churn.
/// The core IL writer keeps its own row models; adapters below convert between
/// delta-owned and core-owned representations when boundary crossings are needed.
module internal FSharp.Compiler.AbstractIL.ILDeltaHandles

open System
open FSharp.Compiler.AbstractIL.BinaryConstants

// ============================================================================
// Entity Token
// ============================================================================
// Generic token representation for EncLog/EncMap entries

/// Represents a metadata token as table index and row ID
/// Used for EncLog and EncMap entries
[<Struct>]
type EntityToken =
    { TableIndex: int
      RowId: int }

    /// Creates a token from table index and row ID
    static member Create(tableIndex: int, rowId: int) = { TableIndex = tableIndex; RowId = rowId }

    /// Gets the full 32-bit token value (table << 24 | rowId)
    member this.Token = (this.TableIndex <<< 24) ||| (this.RowId &&& 0x00FFFFFF)


// ============================================================================
// Typed handles and coded indices used by delta metadata code
// ============================================================================

[<Struct>]
type ModuleHandle = ModuleHandle of rowId: int with member this.RowId = let (ModuleHandle v) = this in v

[<Struct>]
type TypeRefHandle = TypeRefHandle of rowId: int with member this.RowId = let (TypeRefHandle v) = this in v

[<Struct>]
type TypeDefHandle = TypeDefHandle of rowId: int with member this.RowId = let (TypeDefHandle v) = this in v

[<Struct>]
type FieldHandle = FieldHandle of rowId: int with member this.RowId = let (FieldHandle v) = this in v

[<Struct>]
type MethodDefHandle = MethodDefHandle of rowId: int with member this.RowId = let (MethodDefHandle v) = this in v

[<Struct>]
type ParamHandle = ParamHandle of rowId: int with member this.RowId = let (ParamHandle v) = this in v

[<Struct>]
type InterfaceImplHandle = InterfaceImplHandle of rowId: int with member this.RowId = let (InterfaceImplHandle v) = this in v

[<Struct>]
type MemberRefHandle = MemberRefHandle of rowId: int with member this.RowId = let (MemberRefHandle v) = this in v

[<Struct>]
type DeclSecurityHandle = DeclSecurityHandle of rowId: int with member this.RowId = let (DeclSecurityHandle v) = this in v

[<Struct>]
type StandAloneSigHandle = StandAloneSigHandle of rowId: int with member this.RowId = let (StandAloneSigHandle v) = this in v

[<Struct>]
type EventHandle = EventHandle of rowId: int with member this.RowId = let (EventHandle v) = this in v

[<Struct>]
type PropertyHandle = PropertyHandle of rowId: int with member this.RowId = let (PropertyHandle v) = this in v

[<Struct>]
type ModuleRefHandle = ModuleRefHandle of rowId: int with member this.RowId = let (ModuleRefHandle v) = this in v

[<Struct>]
type TypeSpecHandle = TypeSpecHandle of rowId: int with member this.RowId = let (TypeSpecHandle v) = this in v

[<Struct>]
type AssemblyHandle = AssemblyHandle of rowId: int with member this.RowId = let (AssemblyHandle v) = this in v

[<Struct>]
type AssemblyRefHandle = AssemblyRefHandle of rowId: int with member this.RowId = let (AssemblyRefHandle v) = this in v

[<Struct>]
type FileHandle = FileHandle of rowId: int with member this.RowId = let (FileHandle v) = this in v

[<Struct>]
type ExportedTypeHandle = ExportedTypeHandle of rowId: int with member this.RowId = let (ExportedTypeHandle v) = this in v

[<Struct>]
type ManifestResourceHandle = ManifestResourceHandle of rowId: int with member this.RowId = let (ManifestResourceHandle v) = this in v

[<Struct>]
type GenericParamHandle = GenericParamHandle of rowId: int with member this.RowId = let (GenericParamHandle v) = this in v

[<Struct>]
type MethodSpecHandle = MethodSpecHandle of rowId: int with member this.RowId = let (MethodSpecHandle v) = this in v

[<Struct>]
type GenericParamConstraintHandle = GenericParamConstraintHandle of rowId: int with member this.RowId = let (GenericParamConstraintHandle v) = this in v

[<Struct>]
type StringOffset = StringOffset of offset: int with
    member this.Value = let (StringOffset v) = this in v
    static member Zero = StringOffset 0

[<Struct>]
type BlobOffset = BlobOffset of offset: int with
    member this.Value = let (BlobOffset v) = this in v
    static member Zero = BlobOffset 0

[<Struct>]
type GuidIndex = GuidIndex of index: int with
    member this.Value = let (GuidIndex v) = this in v
    static member Zero = GuidIndex 0

[<Struct>]
type UserStringOffset = UserStringOffset of offset: int with
    member this.Value = let (UserStringOffset v) = this in v
    static member Zero = UserStringOffset 0

/// TypeDefOrRef coded index (ECMA-335 II.24.2.6)
type TypeDefOrRef =
    | TDR_TypeDef of TypeDefHandle
    | TDR_TypeRef of TypeRefHandle
    | TDR_TypeSpec of TypeSpecHandle

    member this.CodedTag =
        match this with
        | TDR_TypeDef _ -> tdor_TypeDef.Tag
        | TDR_TypeRef _ -> tdor_TypeRef.Tag
        | TDR_TypeSpec _ -> tdor_TypeSpec.Tag

    member this.RowId =
        match this with
        | TDR_TypeDef h -> h.RowId
        | TDR_TypeRef h -> h.RowId
        | TDR_TypeSpec h -> h.RowId

/// HasCustomAttribute coded index (ECMA-335 II.24.2.6)
type HasCustomAttribute =
    | HCA_MethodDef of MethodDefHandle
    | HCA_Field of FieldHandle
    | HCA_TypeRef of TypeRefHandle
    | HCA_TypeDef of TypeDefHandle
    | HCA_Param of ParamHandle
    | HCA_InterfaceImpl of InterfaceImplHandle
    | HCA_MemberRef of MemberRefHandle
    | HCA_Module of ModuleHandle
    | HCA_DeclSecurity of DeclSecurityHandle
    | HCA_Property of PropertyHandle
    | HCA_Event of EventHandle
    | HCA_StandAloneSig of StandAloneSigHandle
    | HCA_ModuleRef of ModuleRefHandle
    | HCA_TypeSpec of TypeSpecHandle
    | HCA_Assembly of AssemblyHandle
    | HCA_AssemblyRef of AssemblyRefHandle
    | HCA_File of FileHandle
    | HCA_ExportedType of ExportedTypeHandle
    | HCA_ManifestResource of ManifestResourceHandle
    | HCA_GenericParam of GenericParamHandle
    | HCA_GenericParamConstraint of GenericParamConstraintHandle
    | HCA_MethodSpec of MethodSpecHandle

    member this.CodedTag =
        match this with
        | HCA_MethodDef _ -> hca_MethodDef.Tag
        | HCA_Field _ -> hca_FieldDef.Tag
        | HCA_TypeRef _ -> hca_TypeRef.Tag
        | HCA_TypeDef _ -> hca_TypeDef.Tag
        | HCA_Param _ -> hca_ParamDef.Tag
        | HCA_InterfaceImpl _ -> hca_InterfaceImpl.Tag
        | HCA_MemberRef _ -> hca_MemberRef.Tag
        | HCA_Module _ -> hca_Module.Tag
        | HCA_DeclSecurity _ -> hca_Permission.Tag
        | HCA_Property _ -> hca_Property.Tag
        | HCA_Event _ -> hca_Event.Tag
        | HCA_StandAloneSig _ -> hca_StandAloneSig.Tag
        | HCA_ModuleRef _ -> hca_ModuleRef.Tag
        | HCA_TypeSpec _ -> hca_TypeSpec.Tag
        | HCA_Assembly _ -> hca_Assembly.Tag
        | HCA_AssemblyRef _ -> hca_AssemblyRef.Tag
        | HCA_File _ -> hca_File.Tag
        | HCA_ExportedType _ -> hca_ExportedType.Tag
        | HCA_ManifestResource _ -> hca_ManifestResource.Tag
        | HCA_GenericParam _ -> hca_GenericParam.Tag
        // BinaryConstants does not expose these two HCA tags on main; keep the ECMA tag ids explicit here.
        | HCA_GenericParamConstraint _ -> 20
        | HCA_MethodSpec _ -> 21

    member this.RowId =
        match this with
        | HCA_MethodDef h -> h.RowId
        | HCA_Field h -> h.RowId
        | HCA_TypeRef h -> h.RowId
        | HCA_TypeDef h -> h.RowId
        | HCA_Param h -> h.RowId
        | HCA_InterfaceImpl h -> h.RowId
        | HCA_MemberRef h -> h.RowId
        | HCA_Module h -> h.RowId
        | HCA_DeclSecurity h -> h.RowId
        | HCA_Property h -> h.RowId
        | HCA_Event h -> h.RowId
        | HCA_StandAloneSig h -> h.RowId
        | HCA_ModuleRef h -> h.RowId
        | HCA_TypeSpec h -> h.RowId
        | HCA_Assembly h -> h.RowId
        | HCA_AssemblyRef h -> h.RowId
        | HCA_File h -> h.RowId
        | HCA_ExportedType h -> h.RowId
        | HCA_ManifestResource h -> h.RowId
        | HCA_GenericParam h -> h.RowId
        | HCA_GenericParamConstraint h -> h.RowId
        | HCA_MethodSpec h -> h.RowId

/// MemberRefParent coded index (ECMA-335 II.24.2.6)
type MemberRefParent =
    | MRP_TypeDef of TypeDefHandle
    | MRP_TypeRef of TypeRefHandle
    | MRP_ModuleRef of ModuleRefHandle
    | MRP_MethodDef of MethodDefHandle
    | MRP_TypeSpec of TypeSpecHandle

    member this.CodedTag =
        match this with
        // BinaryConstants does not expose this tag on main; keep the ECMA tag id explicit here.
        | MRP_TypeDef _ -> 0
        | MRP_TypeRef _ -> mrp_TypeRef.Tag
        | MRP_ModuleRef _ -> mrp_ModuleRef.Tag
        | MRP_MethodDef _ -> mrp_MethodDef.Tag
        | MRP_TypeSpec _ -> mrp_TypeSpec.Tag

    member this.RowId =
        match this with
        | MRP_TypeDef h -> h.RowId
        | MRP_TypeRef h -> h.RowId
        | MRP_ModuleRef h -> h.RowId
        | MRP_MethodDef h -> h.RowId
        | MRP_TypeSpec h -> h.RowId

/// HasSemantics coded index (ECMA-335 II.24.2.6)
type HasSemantics =
    | HS_Event of EventHandle
    | HS_Property of PropertyHandle

    member this.CodedTag =
        match this with
        | HS_Event _ -> hs_Event.Tag
        | HS_Property _ -> hs_Property.Tag

    member this.RowId =
        match this with
        | HS_Event h -> h.RowId
        | HS_Property h -> h.RowId

/// CustomAttributeType coded index (ECMA-335 II.24.2.6)
type CustomAttributeType =
    | CAT_MethodDef of MethodDefHandle
    | CAT_MemberRef of MemberRefHandle

    member this.CodedTag =
        match this with
        | CAT_MethodDef _ -> cat_MethodDef.Tag
        | CAT_MemberRef _ -> cat_MemberRef.Tag

    member this.RowId =
        match this with
        | CAT_MethodDef h -> h.RowId
        | CAT_MemberRef h -> h.RowId

/// ResolutionScope coded index (ECMA-335 II.24.2.6)
type ResolutionScope =
    | RS_Module of ModuleHandle
    | RS_ModuleRef of ModuleRefHandle
    | RS_AssemblyRef of AssemblyRefHandle
    | RS_TypeRef of TypeRefHandle

    member this.CodedTag =
        match this with
        | RS_Module _ -> rs_Module.Tag
        | RS_ModuleRef _ -> rs_ModuleRef.Tag
        | RS_AssemblyRef _ -> rs_AssemblyRef.Tag
        | RS_TypeRef _ -> rs_TypeRef.Tag

    member this.RowId =
        match this with
        | RS_Module h -> h.RowId
        | RS_ModuleRef h -> h.RowId
        | RS_AssemblyRef h -> h.RowId
        | RS_TypeRef h -> h.RowId

/// MethodDefOrRef coded index (ECMA-335 II.24.2.6)
type MethodDefOrRef =
    | MDOR_MethodDef of MethodDefHandle
    | MDOR_MemberRef of MemberRefHandle

    member this.CodedTag =
        match this with
        | MDOR_MethodDef _ -> mdor_MethodDef.Tag
        | MDOR_MemberRef _ -> mdor_MemberRef.Tag

    member this.RowId =
        match this with
        | MDOR_MethodDef h -> h.RowId
        | MDOR_MemberRef h -> h.RowId

// ----------------------------------------------------------------------------
// Adapters from delta-owned coded indices to boundary-safe primitives.
// ilbinary.fsi intentionally hides core handle/coded-index unions; by using
// primitives at boundaries we keep hot-reload isolated without widening core APIs.
// ----------------------------------------------------------------------------
module CoreTypeAdapters =
    let moduleRowId (ModuleHandle rowId) = rowId
    let typeRefRowId (TypeRefHandle rowId) = rowId
    let typeDefRowId (TypeDefHandle rowId) = rowId
    let memberRefRowId (MemberRefHandle rowId) = rowId
    let methodDefRowId (MethodDefHandle rowId) = rowId
    let typeSpecRowId (TypeSpecHandle rowId) = rowId
    let moduleRefRowId (ModuleRefHandle rowId) = rowId
    let assemblyRefRowId (AssemblyRefHandle rowId) = rowId

    /// Returns (coded tag, row id) for TypeDefOrRef.
    let typeDefOrRefParts (value: TypeDefOrRef) = value.CodedTag, value.RowId

    /// Returns (coded tag, row id) for MemberRefParent.
    let memberRefParentParts (value: MemberRefParent) = value.CodedTag, value.RowId

    /// Returns (coded tag, row id) for MethodDefOrRef.
    let methodDefOrRefParts (value: MethodDefOrRef) = value.CodedTag, value.RowId

    /// Returns (coded tag, row id) for ResolutionScope.
    let resolutionScopeParts (value: ResolutionScope) = value.CodedTag, value.RowId

// ============================================================================
// Additional Coded Index Types (less frequently used)
// ============================================================================
// These are defined here rather than in BinaryConstants because they are
// primarily used by delta code and not needed for baseline IL writing.

/// HasConstant coded index (2-bit tag)
/// Tag: Field=0, Param=1, Property=2
type HasConstant =
    | HC_Field of FieldHandle
    | HC_Param of ParamHandle
    | HC_Property of PropertyHandle

    member this.TableIndex =
        match this with
        | HC_Field _ -> 0x04
        | HC_Param _ -> 0x08
        | HC_Property _ -> 0x17

    member this.RowId =
        match this with
        | HC_Field(FieldHandle rid) -> rid
        | HC_Param(ParamHandle rid) -> rid
        | HC_Property(PropertyHandle rid) -> rid

/// HasFieldMarshal coded index (1-bit tag)
/// Tag: Field=0, Param=1
type HasFieldMarshal =
    | HFM_Field of FieldHandle
    | HFM_Param of ParamHandle

    member this.TableIndex =
        match this with
        | HFM_Field _ -> 0x04
        | HFM_Param _ -> 0x08

    member this.RowId =
        match this with
        | HFM_Field(FieldHandle rid) -> rid
        | HFM_Param(ParamHandle rid) -> rid

/// HasDeclSecurity coded index (2-bit tag)
/// Tag: TypeDef=0, MethodDef=1, Assembly=2
type HasDeclSecurity =
    | HDS_TypeDef of TypeDefHandle
    | HDS_MethodDef of MethodDefHandle
    | HDS_Assembly of AssemblyHandle

    member this.TableIndex =
        match this with
        | HDS_TypeDef _ -> 0x02
        | HDS_MethodDef _ -> 0x06
        | HDS_Assembly _ -> 0x20

    member this.RowId =
        match this with
        | HDS_TypeDef(TypeDefHandle rid) -> rid
        | HDS_MethodDef(MethodDefHandle rid) -> rid
        | HDS_Assembly(AssemblyHandle rid) -> rid

/// MemberForwarded coded index (1-bit tag)
/// Tag: Field=0, MethodDef=1
type MemberForwarded =
    | MF_Field of FieldHandle
    | MF_MethodDef of MethodDefHandle

    member this.TableIndex =
        match this with
        | MF_Field _ -> 0x04
        | MF_MethodDef _ -> 0x06

    member this.RowId =
        match this with
        | MF_Field(FieldHandle rid) -> rid
        | MF_MethodDef(MethodDefHandle rid) -> rid

/// Implementation coded index (2-bit tag)
/// Tag: File=0, AssemblyRef=1, ExportedType=2
type Implementation =
    | IMP_File of FileHandle
    | IMP_AssemblyRef of AssemblyRefHandle
    | IMP_ExportedType of ExportedTypeHandle

    member this.TableIndex =
        match this with
        | IMP_File _ -> 0x26
        | IMP_AssemblyRef _ -> 0x23
        | IMP_ExportedType _ -> 0x27

    member this.RowId =
        match this with
        | IMP_File(FileHandle rid) -> rid
        | IMP_AssemblyRef(AssemblyRefHandle rid) -> rid
        | IMP_ExportedType(ExportedTypeHandle rid) -> rid

/// TypeOrMethodDef coded index (1-bit tag)
/// Tag: TypeDef=0, MethodDef=1
type TypeOrMethodDef =
    | TOMD_TypeDef of TypeDefHandle
    | TOMD_MethodDef of MethodDefHandle

    member this.TableIndex =
        match this with
        | TOMD_TypeDef _ -> 0x02
        | TOMD_MethodDef _ -> 0x06

    member this.CodedTag =
        match this with
        | TOMD_TypeDef _ -> tomd_TypeDef.Tag
        | TOMD_MethodDef _ -> tomd_MethodDef.Tag

    member this.RowId =
        match this with
        | TOMD_TypeDef(TypeDefHandle rid) -> rid
        | TOMD_MethodDef(MethodDefHandle rid) -> rid

// ============================================================================
// DeltaTokens Module
// ============================================================================
// Utilities for metadata token manipulation, replacing MetadataTokens static methods.

/// Token arithmetic utilities (replaces System.Reflection.Metadata.Ecma335.MetadataTokens)
module DeltaTokens =

    /// Number of metadata tables defined in ECMA-335 (includes reserved slots)
    let TableCount = 64

    /// Extract the row number (lower 24 bits) from a metadata token
    let getRowNumber (token: int) = token &&& 0x00FFFFFF

    /// Extract the table index (upper 8 bits) from a metadata token
    let getTableIndex (token: int) = (token >>> 24) &&& 0xFF

    /// Create a metadata token from a TableName and row number.
    /// Token format: [table index : 8 bits][row number : 24 bits]
    /// Internal: TableName is from BinaryConstants which is internal.
    let internal makeToken (table: TableName) (rowNumber: int) =
        (table.Index <<< 24) ||| (rowNumber &&& 0x00FFFFFF)

    /// Create a metadata token from a raw table index (int) and row number.
    /// Use this for PDB tables which don't have TableName definitions,
    /// or when calling from outside the compiler assembly.
    let makeTokenFromIndex (tableIndex: int) (rowNumber: int) =
        (tableIndex <<< 24) ||| (rowNumber &&& 0x00FFFFFF)

    /// Create an EntityToken from a raw token value
    let toEntityToken (token: int) : EntityToken =
        { TableIndex = getTableIndex token
          RowId = getRowNumber token }

    /// Convert an EntityToken to a raw token value
    let fromEntityToken (entity: EntityToken) : int = entity.Token

    // -------------------------------------------------------------------------
    // Portable PDB Table Indices (not part of ECMA-335, defined in Portable PDB spec)
    // -------------------------------------------------------------------------
    // These tables are used for debug information in Portable PDB format.
    // They start at index 0x30 to avoid collision with ECMA-335 tables.
    // Reference: https://github.com/dotnet/runtime/blob/main/docs/design/specs/PortablePdb-Metadata.md

    let tableDocument = 0x30
    let tableMethodDebugInformation = 0x31
    let tableLocalScope = 0x32
    let tableLocalVariable = 0x33
    let tableLocalConstant = 0x34
    let tableImportScope = 0x35
    let tableStateMachineMethod = 0x36
    let tableCustomDebugInformation = 0x37

// ============================================================================
// Conversion Helpers
// ============================================================================
// Functions to convert between F# handles and raw values

module HandleConversions =
    /// Create a HasCustomAttribute from table index and row ID
    /// Returns None for invalid table indices
    let tryMakeHasCustomAttribute (tableIndex: int) (rowId: int) : HasCustomAttribute option =
        match tableIndex with
        | 0x06 -> Some(HCA_MethodDef(MethodDefHandle rowId))
        | 0x04 -> Some(HCA_Field(FieldHandle rowId))
        | 0x01 -> Some(HCA_TypeRef(TypeRefHandle rowId))
        | 0x02 -> Some(HCA_TypeDef(TypeDefHandle rowId))
        | 0x08 -> Some(HCA_Param(ParamHandle rowId))
        | 0x09 -> Some(HCA_InterfaceImpl(InterfaceImplHandle rowId))
        | 0x0A -> Some(HCA_MemberRef(MemberRefHandle rowId))
        | 0x00 -> Some(HCA_Module(ModuleHandle rowId))
        | 0x0E -> Some(HCA_DeclSecurity(DeclSecurityHandle rowId))
        | 0x17 -> Some(HCA_Property(PropertyHandle rowId))
        | 0x14 -> Some(HCA_Event(EventHandle rowId))
        | 0x11 -> Some(HCA_StandAloneSig(StandAloneSigHandle rowId))
        | 0x1A -> Some(HCA_ModuleRef(ModuleRefHandle rowId))
        | 0x1B -> Some(HCA_TypeSpec(TypeSpecHandle rowId))
        | 0x20 -> Some(HCA_Assembly(AssemblyHandle rowId))
        | 0x23 -> Some(HCA_AssemblyRef(AssemblyRefHandle rowId))
        | 0x26 -> Some(HCA_File(FileHandle rowId))
        | 0x27 -> Some(HCA_ExportedType(ExportedTypeHandle rowId))
        | 0x28 -> Some(HCA_ManifestResource(ManifestResourceHandle rowId))
        | 0x2A -> Some(HCA_GenericParam(GenericParamHandle rowId))
        | 0x2C -> Some(HCA_GenericParamConstraint(GenericParamConstraintHandle rowId))
        | 0x2B -> Some(HCA_MethodSpec(MethodSpecHandle rowId))
        | _ -> None

    /// Create a ResolutionScope from table index and row ID
    let tryMakeResolutionScope (tableIndex: int) (rowId: int) : ResolutionScope option =
        match tableIndex with
        | 0x00 -> Some(RS_Module(ModuleHandle rowId))
        | 0x1A -> Some(RS_ModuleRef(ModuleRefHandle rowId))
        | 0x23 -> Some(RS_AssemblyRef(AssemblyRefHandle rowId))
        | 0x01 -> Some(RS_TypeRef(TypeRefHandle rowId))
        | _ -> None

    /// Create a MemberRefParent from table index and row ID
    let tryMakeMemberRefParent (tableIndex: int) (rowId: int) : MemberRefParent option =
        match tableIndex with
        | 0x02 -> Some(MRP_TypeDef(TypeDefHandle rowId))
        | 0x01 -> Some(MRP_TypeRef(TypeRefHandle rowId))
        | 0x1A -> Some(MRP_ModuleRef(ModuleRefHandle rowId))
        | 0x06 -> Some(MRP_MethodDef(MethodDefHandle rowId))
        | 0x1B -> Some(MRP_TypeSpec(TypeSpecHandle rowId))
        | _ -> None

    /// Create a CustomAttributeType from table index and row ID
    let tryMakeCustomAttributeType (tableIndex: int) (rowId: int) : CustomAttributeType option =
        match tableIndex with
        | 0x06 -> Some(CAT_MethodDef(MethodDefHandle rowId))
        | 0x0A -> Some(CAT_MemberRef(MemberRefHandle rowId))
        | _ -> None

    /// Create a TypeDefOrRef from table index and row ID
    let tryMakeTypeDefOrRef (tableIndex: int) (rowId: int) : TypeDefOrRef option =
        match tableIndex with
        | 0x02 -> Some(TDR_TypeDef(TypeDefHandle rowId))
        | 0x01 -> Some(TDR_TypeRef(TypeRefHandle rowId))
        | 0x1B -> Some(TDR_TypeSpec(TypeSpecHandle rowId))
        | _ -> None

// ============================================================================
// Edit-and-Continue Operation Codes
// ============================================================================
// F# native enum for EncLog operation codes.
// Replaces System.Reflection.Metadata.Ecma335.EditAndContinueOperation.

/// Operation code for EncLog entries per ECMA-335.
/// Indicates whether a row is new (AddXxx) or an update (Default).
[<Struct; CustomEquality; NoComparison>]
type EditAndContinueOperation =
    | Default
    | AddMethod
    | AddField
    | AddParameter
    | AddProperty
    | AddEvent

    /// Get the numeric value for serialization.
    /// Values match the CLR EnC operation codes (and SRM's
    /// System.Reflection.Metadata.Ecma335.EditAndContinueOperation):
    /// Default=0, AddMethod=1, AddField=2, AddParameter=3, AddProperty=4, AddEvent=5.
    member this.Value =
        match this with
        | Default -> 0
        | AddMethod -> 1
        | AddField -> 2
        | AddParameter -> 3
        | AddProperty -> 4
        | AddEvent -> 5

    override this.GetHashCode() = this.Value
    override this.Equals obj =
        match obj with
        | :? EditAndContinueOperation as other -> this.Value = other.Value
        | _ -> false

    interface IEquatable<EditAndContinueOperation> with
        member this.Equals other = this.Value = other.Value

// ============================================================================
// IL Exception Region Types
// ============================================================================
// These replace System.Reflection.Metadata.ExceptionRegion and ExceptionRegionKind

/// Kind of exception handling region in IL method body
type IlExceptionRegionKind =
    | Catch = 0
    | Filter = 1
    | Finally = 2
    | Fault = 4

/// Exception handling region in IL method body.
/// Replaces System.Reflection.Metadata.ExceptionRegion for delta emission.
[<Struct>]
type IlExceptionRegion =
    {
        Kind: IlExceptionRegionKind
        TryOffset: int
        TryLength: int
        HandlerOffset: int
        HandlerLength: int
        /// For Catch: the catch type token; for others: 0
        CatchTypeToken: int
        /// For Filter: the filter offset; for others: 0
        FilterOffset: int
    }
