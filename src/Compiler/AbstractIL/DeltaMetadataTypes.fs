module internal FSharp.Compiler.AbstractIL.DeltaMetadataTypes

open System
open System.Reflection
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.BinaryConstants
open FSharp.Compiler.AbstractIL.ILDeltaHandles

// ============================================================================
// Definition keys
// ============================================================================
// Stable, content-based identifiers for metadata definitions. These are used to
// correlate a definition across compiles/generations (e.g. baseline vs. fresh
// compile) independently of row-id churn. Lifted from the hot-reload baseline
// module: unlike the rest of that module (FSharpEmitBaseline, handle caches,
// token maps, TypeReferenceKey, ...), these records carry no session state and
// are pure structural identities over ILType/string data, so they belong beside
// the *RowInfo contract types below rather than with baseline bookkeeping.

/// <summary>Stable identifier for a method definition used when correlating baseline tokens.</summary>
type MethodDefinitionKey =
    {
        DeclaringType: string
        Name: string
        GenericArity: int
        ParameterTypes: ILType list
        ReturnType: ILType
    }

/// <summary>Stable identifier for a method parameter (sequence number within a method).</summary>
type ParameterDefinitionKey =
    {
        Method: MethodDefinitionKey
        SequenceNumber: int
    }

/// <summary>Stable identifier for a field definition in the baseline assembly.</summary>
type FieldDefinitionKey =
    {
        DeclaringType: string
        Name: string
        FieldType: ILType
    }

/// <summary>Stable identifier for a property definition (including indexer parameter shapes).</summary>
type PropertyDefinitionKey =
    {
        DeclaringType: string
        Name: string
        PropertyType: ILType
        IndexParameterTypes: ILType list
    }

/// <summary>Stable identifier for an event definition in the baseline assembly.</summary>
type EventDefinitionKey =
    {
        DeclaringType: string
        Name: string
        EventType: ILType option
    }

/// Identifies the property or event a MethodSemantics row (getter/setter/add/remove) is
/// associated with, plus the row id of that PropertyMap/EventMap-owned parent.
type MethodSemanticsAssociation =
    | PropertyAssociation of PropertyDefinitionKey * rowId: int
    | EventAssociation of EventDefinitionKey * rowId: int

/// Minimal shared types for hot-reload metadata tables.
type RowElementData =
    {
        Tag: int
        Value: int
        IsAbsolute: bool
    }

type MethodDefinitionRowInfo =
    {
        Key: MethodDefinitionKey
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
        CodeRva: int option
    }

type ParameterDefinitionRowInfo =
    {
        Key: ParameterDefinitionKey
        RowId: int
        IsAdded: bool
        Attributes: ParameterAttributes
        SequenceNumber: int
        Name: string option
        NameOffset: StringOffset option
    }

/// Row model for a Field table entry emitted into a delta (ECMA-335 II.22.15:
/// Flags, Name, Signature). Added fields additionally record the parent TypeDef
/// row so the EncLog can emit the Roslyn-style AddField parent entry.
type FieldDefinitionRowInfo =
    {
        Key: FieldDefinitionKey
        RowId: int
        IsAdded: bool
        /// Row id of the baseline TypeDef that receives the field; used for the
        /// EncLog (TypeDef, AddField) parent entry preceding the Field row.
        ParentTypeDefRowId: int
        Attributes: FieldAttributes
        Name: string
        NameOffset: StringOffset option
        Signature: byte[]
        SignatureOffset: BlobOffset option
    }

/// Row model for an ADDED TypeDef table entry emitted into a delta (ECMA-335
/// II.22.37: Flags, TypeName, TypeNamespace, Extends, FieldList, MethodList).
/// Roslyn parity (DeltaMetadataWriter.GetFirstFieldDefinitionHandle /
/// GetFirstMethodDefinitionHandle return default in EnC deltas): the
/// FieldList/MethodList columns are always written as 0 — members are linked
/// to the new type through the AddField/AddMethod EncLog parent entries.
type TypeDefinitionRowInfo =
    {
        /// Full name of the added type (namespace-qualified, '+'-nested), used as the
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
        EnclosingTypeDefRowId: int option
    }

/// Row model for a NestedClass table entry (ECMA-335 II.22.32: NestedClass,
/// EnclosingClass — both TypeDef row indices). Emitted for added nested types;
/// logged as a plain Default EncLog entry (Roslyn parity).
type NestedClassRowInfo =
    {
        RowId: int
        NestedTypeDefRowId: int
        EnclosingTypeDefRowId: int
    }

/// Row model for an InterfaceImpl table entry (ECMA-335 II.22.23: Class — a TypeDef row
/// index — and Interface — a TypeDefOrRef coded index). Emitted for the interfaces
/// implemented by ADDED types (records/unions implement IComparable/IEquatable and
/// friends); logged as a plain Default EncLog entry trailing the log and listed in
/// EncMap as an add (C# 'new_class' reference template: InterfaceImpl 0x09000001 trails
/// the generation-1 log of a new class implementing IDisposable).
type InterfaceImplRowInfo =
    {
        RowId: int
        ClassTypeDefRowId: int
        Interface: TypeDefOrRef
    }

/// Row model for a MethodImpl table entry (ECMA-335 II.22.27: Class — a TypeDef row
/// index — MethodBody and MethodDeclaration — MethodDefOrRef coded indexes). Emitted
/// for the explicit interface implementations of ADDED types (F# classes implement
/// interfaces explicitly, so unlike C#'s implicit public mapping every implemented
/// interface slot carries a MethodImpl row).
type MethodImplRowInfo =
    {
        RowId: int
        ClassTypeDefRowId: int
        MethodBody: MethodDefOrRef
        MethodDeclaration: MethodDefOrRef
    }

/// Row model for a Constant table entry (ECMA-335 II.22.9: Type — a 1-byte
/// ELEMENT_TYPE code followed by a zero padding byte — Parent — a HasConstant coded
/// index — and Value — a #Blob offset). Emitted for the literal (HasDefault) fields
/// of ADDED types and members: enum members, union Tags holder constants, [<Literal>]
/// module values. Logged as plain Default EncLog entries trailing the log and listed
/// in EncMap as adds (C# 'new_enum' reference template: the three Constant rows of an
/// added enum trail the generation-1 log, parents are the new Field rows, value blobs
/// live in the delta #Blob heap).
type ConstantRowInfo =
    {
        RowId: int
        /// ELEMENT_TYPE constant type code (ECMA-335 II.23.1.16, e.g. 0x08 = I4).
        TypeCode: byte
        Parent: HasConstant
        Value: byte[]
    }

type TypeReferenceRowInfo =
    {
        RowId: int
        ResolutionScope: ResolutionScope
        Name: string
        NameOffset: StringOffset option
        Namespace: string
        NamespaceOffset: StringOffset option
    }

type MemberReferenceRowInfo =
    {
        RowId: int
        Parent: MemberRefParent
        Name: string
        NameOffset: StringOffset option
        Signature: byte[]
        SignatureOffset: BlobOffset option
    }

type MethodSpecificationRowInfo =
    {
        RowId: int
        Method: MethodDefOrRef
        Signature: byte[]
        SignatureOffset: BlobOffset option
    }

/// Row model for a TypeSpec table entry (ECMA-335 II.22.39: a single #Blob signature
/// column carrying a bare Type, II.23.2.14). Appended with a plain Default EncLog entry
/// (C# reference template parity) when an edit references a generic instantiation that
/// has no matching baseline row — e.g. an added lambda whose closure class extends a
/// brand-new FSharpFunc<A,B> instantiation.
type TypeSpecificationRowInfo =
    {
        RowId: int
        Signature: byte[]
        SignatureOffset: BlobOffset option
    }

/// Row model for a GenericParam table entry (ECMA-335 II.22.20: Number (u2),
/// Flags (u2), Owner (TypeOrMethodDef coded index), Name (#Strings)). Emitted for
/// the generic parameters of ADDED generic methods (and added generic types).
/// Logged as a plain Default EncLog entry and listed in EncMap as an add — the
/// recorded C# reference template (csharp_enc_reference 'generic_method_add')
/// shows 'GenericParam 0x2a000001 Default' trailing the AddMethod/AddParameter
/// pairs, with the row present in EncMap. GenericParam rows of UPDATED methods
/// are baseline rows and are never re-emitted.
type GenericParamRowInfo =
    {
        RowId: int
        /// Zero-based ordinal of the generic parameter within its owner.
        Number: int
        Attributes: GenericParameterAttributes
        Owner: TypeOrMethodDef
        Name: string
        NameOffset: StringOffset option
    }

/// Row model for a GenericParamConstraint table entry (ECMA-335 II.22.21: Owner — a
/// GenericParam row index — and Constraint — a TypeDefOrRef coded index). Emitted for
/// the IL constraints of ADDED generic definitions' type parameters; logged as a plain
/// Default EncLog entry after the GenericParam entries and listed in EncMap as an add
/// (C# reference template 'generic_constraint_add': GenericParamConstraint 0x2c000001
/// Default trailing the GenericParam entry).
type GenericParamConstraintRowInfo =
    {
        RowId: int
        OwnerGenericParamRowId: int
        Constraint: TypeDefOrRef
    }

type AssemblyReferenceRowInfo =
    {
        RowId: int
        Version: Version
        Flags: AssemblyFlags
        PublicKeyOrToken: byte[]
        PublicKeyOrTokenOffset: BlobOffset option
        Name: string
        NameOffset: StringOffset option
        Culture: string option
        CultureOffset: StringOffset option
        HashValue: byte[]
        HashValueOffset: BlobOffset option
    }

type CustomAttributeRowInfo =
    {
        RowId: int
        Parent: HasCustomAttribute
        Constructor: CustomAttributeType
        Value: byte[]
        ValueOffset: BlobOffset option
    }

type PropertyDefinitionRowInfo =
    {
        Key: PropertyDefinitionKey
        RowId: int
        IsAdded: bool
        /// PropertyMap row id owning an ADDED property; the AddProperty EncLog entry must
        /// carry the parent PropertyMap token (CLR links via AddPropertyToPropertyMap).
        ParentPropertyMapRowId: int option
        Name: string
        NameOffset: StringOffset option
        Signature: byte[]
        SignatureOffset: BlobOffset option
        Attributes: PropertyAttributes
    }

type EventDefinitionRowInfo =
    {
        Key: EventDefinitionKey
        RowId: int
        IsAdded: bool
        /// EventMap row id owning an ADDED event; the AddEvent EncLog entry must carry the
        /// parent EventMap token (CLR links via AddEventToEventMap).
        ParentEventMapRowId: int option
        Name: string
        NameOffset: StringOffset option
        Attributes: EventAttributes
        EventType: TypeDefOrRef
    }

type PropertyMapRowInfo =
    {
        DeclaringType: string
        RowId: int
        TypeDefRowId: int
        FirstPropertyRowId: int option
        IsAdded: bool
    }

type EventMapRowInfo =
    {
        DeclaringType: string
        RowId: int
        TypeDefRowId: int
        FirstEventRowId: int option
        IsAdded: bool
    }

type MethodSemanticsMetadataUpdate =
    {
        RowId: int
        MethodToken: int
        Attributes: MethodSemanticsAttributes
        IsAdded: bool
        /// Association info is required - provides property/event key and rowId
        AssociationInfo: MethodSemanticsAssociation
    }

type TableRows =
    {
        Module: RowElementData[][]
        TypeDef: RowElementData[][]
        NestedClass: RowElementData[][]
        InterfaceImpl: RowElementData[][]
        Constant: RowElementData[][]
        MethodImpl: RowElementData[][]
        Field: RowElementData[][]
        MethodDef: RowElementData[][]
        Param: RowElementData[][]
        TypeRef: RowElementData[][]
        MemberRef: RowElementData[][]
        MethodSpec: RowElementData[][]
        TypeSpec: RowElementData[][]
        GenericParam: RowElementData[][]
        GenericParamConstraint: RowElementData[][]
        AssemblyRef: RowElementData[][]
        StandAloneSig: RowElementData[][]
        CustomAttribute: RowElementData[][]
        Property: RowElementData[][]
        Event: RowElementData[][]
        PropertyMap: RowElementData[][]
        EventMap: RowElementData[][]
        MethodSemantics: RowElementData[][]
        EncLog: RowElementData[][]
        EncMap: RowElementData[][]
    }
