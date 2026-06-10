module internal FSharp.Compiler.CodeGen.DeltaMetadataTypes

open System
open System.Reflection
open FSharp.Compiler.AbstractIL.BinaryConstants
open FSharp.Compiler.AbstractIL.ILDeltaHandles
open FSharp.Compiler.HotReloadBaseline

/// Minimal shared types for hot-reload metadata tables.
type RowElementData =
    { Tag: int
      Value: int
      IsAbsolute: bool }

type MethodDefinitionRowInfo =
    { Key: MethodDefinitionKey
      RowId: int
      IsAdded: bool
      Attributes: MethodAttributes
      ImplAttributes: MethodImplAttributes
      Name: string
      NameOffset: StringOffset option
      Signature: byte[]
      SignatureOffset: BlobOffset option
      FirstParameterRowId: int option
      CodeRva: int option }

type ParameterDefinitionRowInfo =
    { Key: ParameterDefinitionKey
      RowId: int
      IsAdded: bool
      Attributes: ParameterAttributes
      SequenceNumber: int
      Name: string option
      NameOffset: StringOffset option }

/// Row model for a Field table entry emitted into a delta (ECMA-335 II.22.15:
/// Flags, Name, Signature). Added fields additionally record the parent TypeDef
/// row so the EncLog can emit the Roslyn-style AddField parent entry.
type FieldDefinitionRowInfo =
    { Key: FieldDefinitionKey
      RowId: int
      IsAdded: bool
      /// Row id of the baseline TypeDef that receives the field; used for the
      /// EncLog (TypeDef, AddField) parent entry preceding the Field row.
      ParentTypeDefRowId: int
      Attributes: FieldAttributes
      Name: string
      NameOffset: StringOffset option
      Signature: byte[]
      SignatureOffset: BlobOffset option }

type TypeReferenceRowInfo =
    { RowId: int
      ResolutionScope: ResolutionScope
      Name: string
      NameOffset: StringOffset option
      Namespace: string
      NamespaceOffset: StringOffset option }

type MemberReferenceRowInfo =
    { RowId: int
      Parent: MemberRefParent
      Name: string
      NameOffset: StringOffset option
      Signature: byte[]
      SignatureOffset: BlobOffset option }

type MethodSpecificationRowInfo =
    { RowId: int
      Method: MethodDefOrRef
      Signature: byte[]
      SignatureOffset: BlobOffset option }

type AssemblyReferenceRowInfo =
    { RowId: int
      Version: Version
      Flags: AssemblyFlags
      PublicKeyOrToken: byte[]
      PublicKeyOrTokenOffset: BlobOffset option
      Name: string
      NameOffset: StringOffset option
      Culture: string option
      CultureOffset: StringOffset option
      HashValue: byte[]
      HashValueOffset: BlobOffset option }

type CustomAttributeRowInfo =
    { RowId: int
      Parent: HasCustomAttribute
      Constructor: CustomAttributeType
      Value: byte[]
      ValueOffset: BlobOffset option }

type PropertyDefinitionRowInfo =
    { Key: PropertyDefinitionKey
      RowId: int
      IsAdded: bool
      Name: string
      NameOffset: StringOffset option
      Signature: byte[]
      SignatureOffset: BlobOffset option
      Attributes: PropertyAttributes }

type EventDefinitionRowInfo =
    { Key: EventDefinitionKey
      RowId: int
      IsAdded: bool
      Name: string
      NameOffset: StringOffset option
      Attributes: EventAttributes
      EventType: TypeDefOrRef }

type PropertyMapRowInfo =
    { DeclaringType: string
      RowId: int
      TypeDefRowId: int
      FirstPropertyRowId: int option
      IsAdded: bool }

type EventMapRowInfo =
    { DeclaringType: string
      RowId: int
      TypeDefRowId: int
      FirstEventRowId: int option
      IsAdded: bool }

type MethodSemanticsMetadataUpdate =
    { RowId: int
      MethodToken: int
      Attributes: MethodSemanticsAttributes
      IsAdded: bool
      /// Association info is required - provides property/event key and rowId
      AssociationInfo: MethodSemanticsAssociation }

type TableRows =
    { Module: RowElementData[][]
      Field: RowElementData[][]
      MethodDef: RowElementData[][]
      Param: RowElementData[][]
      TypeRef: RowElementData[][]
      MemberRef: RowElementData[][]
      MethodSpec: RowElementData[][]
      AssemblyRef: RowElementData[][]
      StandAloneSig: RowElementData[][]
      CustomAttribute: RowElementData[][]
      Property: RowElementData[][]
      Event: RowElementData[][]
      PropertyMap: RowElementData[][]
      EventMap: RowElementData[][]
      MethodSemantics: RowElementData[][]
      EncLog: RowElementData[][]
      EncMap: RowElementData[][] }
