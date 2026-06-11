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
      /// Row id of the baseline TypeDef that receives an ADDED method. Required for added
      /// rows: the CLR EnC applier (CMiniMdRW::ApplyDelta) reads the parent TypeDef from
      /// the AddMethod EncLog entry and links the new method into that type's member list.
      ParentTypeDefRowId: int option
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

/// Row model for an ADDED TypeDef table entry emitted into a delta (ECMA-335
/// II.22.37: Flags, TypeName, TypeNamespace, Extends, FieldList, MethodList).
/// Roslyn parity (DeltaMetadataWriter.GetFirstFieldDefinitionHandle /
/// GetFirstMethodDefinitionHandle return default in EnC deltas): the
/// FieldList/MethodList columns are always written as 0 — members are linked
/// to the new type through the AddField/AddMethod EncLog parent entries.
type TypeDefinitionRowInfo =
    { /// Full name of the added type (namespace-qualified, '+'-nested), used as the
      /// baseline TypeTokens key when chaining the next-generation baseline.
      FullName: string
      RowId: int
      Attributes: TypeAttributes
      Name: string
      NameOffset: StringOffset option
      Namespace: string
      NamespaceOffset: StringOffset option
      /// Base type, remapped to baseline/delta rows. None encodes the nil
      /// TypeDefOrRef (interfaces / <Module>).
      Extends: TypeDefOrRef option
      /// Row id of the enclosing TypeDef when the added type is nested; drives the
      /// NestedClass row the writer emits alongside the TypeDef row.
      EnclosingTypeDefRowId: int option }

/// Row model for a NestedClass table entry (ECMA-335 II.22.32: NestedClass,
/// EnclosingClass — both TypeDef row indices). Emitted for added nested types;
/// logged as a plain Default EncLog entry (Roslyn parity).
type NestedClassRowInfo =
    { RowId: int
      NestedTypeDefRowId: int
      EnclosingTypeDefRowId: int }

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

/// Row model for a TypeSpec table entry (ECMA-335 II.22.39: a single #Blob signature
/// column carrying a bare Type, II.23.2.14). Appended with a plain Default EncLog entry
/// (C# reference template parity) when an edit references a generic instantiation that
/// has no matching baseline row — e.g. an added lambda whose closure class extends a
/// brand-new FSharpFunc<A,B> instantiation.
type TypeSpecificationRowInfo =
    { RowId: int
      Signature: byte[]
      SignatureOffset: BlobOffset option }

/// Row model for a GenericParam table entry (ECMA-335 II.22.20: Number (u2),
/// Flags (u2), Owner (TypeOrMethodDef coded index), Name (#Strings)). Emitted for
/// the generic parameters of ADDED generic methods (and added generic types).
/// Logged as a plain Default EncLog entry and listed in EncMap as an add — the
/// recorded C# reference template (csharp_enc_reference 'generic_method_add')
/// shows 'GenericParam 0x2a000001 Default' trailing the AddMethod/AddParameter
/// pairs, with the row present in EncMap. GenericParam rows of UPDATED methods
/// are baseline rows and are never re-emitted.
type GenericParamRowInfo =
    { RowId: int
      /// Zero-based ordinal of the generic parameter within its owner.
      Number: int
      Attributes: GenericParameterAttributes
      Owner: TypeOrMethodDef
      Name: string
      NameOffset: StringOffset option }

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
      /// PropertyMap row id owning an ADDED property; the AddProperty EncLog entry must
      /// carry the parent PropertyMap token (CLR links via AddPropertyToPropertyMap).
      ParentPropertyMapRowId: int option
      Name: string
      NameOffset: StringOffset option
      Signature: byte[]
      SignatureOffset: BlobOffset option
      Attributes: PropertyAttributes }

type EventDefinitionRowInfo =
    { Key: EventDefinitionKey
      RowId: int
      IsAdded: bool
      /// EventMap row id owning an ADDED event; the AddEvent EncLog entry must carry the
      /// parent EventMap token (CLR links via AddEventToEventMap).
      ParentEventMapRowId: int option
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
      TypeDef: RowElementData[][]
      NestedClass: RowElementData[][]
      Field: RowElementData[][]
      MethodDef: RowElementData[][]
      Param: RowElementData[][]
      TypeRef: RowElementData[][]
      MemberRef: RowElementData[][]
      MethodSpec: RowElementData[][]
      TypeSpec: RowElementData[][]
      GenericParam: RowElementData[][]
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
