module internal FSharp.Compiler.CodeGen.DeltaMetadataEncoding

open FSharp.Compiler.AbstractIL.BinaryConstants

/// Encodes row-element tags for delta table rows.
/// This stays hot-reload-owned so delta serialization can evolve without expanding ilwrite.fsi.
module RowElementTags =
    [<Literal>]
    let UShort = 0

    [<Literal>]
    let ULong = 1

    [<Literal>]
    let Data = 2

    [<Literal>]
    let DataResources = 3

    [<Literal>]
    let Guid = 4

    [<Literal>]
    let Blob = 5

    [<Literal>]
    let String = 6

    [<Literal>]
    let SimpleIndexMin = 7

    [<Literal>]
    let SimpleIndexMax = 119

    let SimpleIndex (table: TableName) = SimpleIndexMin + table.Index

    [<Literal>]
    let TypeDefOrRefOrSpecMin = 120

    [<Literal>]
    let TypeDefOrRefOrSpecMax = 122

    let TypeDefOrRefOrSpec (tag: TypeDefOrRefTag) = TypeDefOrRefOrSpecMin + int tag.Tag

    [<Literal>]
    let TypeOrMethodDefMin = 123

    [<Literal>]
    let TypeOrMethodDefMax = 124

    let TypeOrMethodDef (tag: TypeOrMethodDefTag) = TypeOrMethodDefMin + int tag.Tag

    [<Literal>]
    let HasConstantMin = 125

    [<Literal>]
    let HasConstantMax = 127

    let HasConstant (tag: HasConstantTag) = HasConstantMin + int tag.Tag

    [<Literal>]
    let HasCustomAttributeMin = 128

    [<Literal>]
    let HasCustomAttributeMax = 149

    let HasCustomAttribute (tag: HasCustomAttributeTag) = HasCustomAttributeMin + int tag.Tag

    [<Literal>]
    let HasFieldMarshalMin = 150

    [<Literal>]
    let HasFieldMarshalMax = 151

    let HasFieldMarshal (tag: HasFieldMarshalTag) = HasFieldMarshalMin + int tag.Tag

    [<Literal>]
    let HasDeclSecurityMin = 152

    [<Literal>]
    let HasDeclSecurityMax = 154

    let HasDeclSecurity (tag: HasDeclSecurityTag) = HasDeclSecurityMin + int tag.Tag

    [<Literal>]
    let MemberRefParentMin = 155

    [<Literal>]
    let MemberRefParentMax = 159

    let MemberRefParent (tag: MemberRefParentTag) = MemberRefParentMin + int tag.Tag

    [<Literal>]
    let HasSemanticsMin = 160

    [<Literal>]
    let HasSemanticsMax = 161

    let HasSemantics (tag: HasSemanticsTag) = HasSemanticsMin + int tag.Tag

    [<Literal>]
    let MethodDefOrRefMin = 162

    [<Literal>]
    let MethodDefOrRefMax = 164

    let MethodDefOrRef (tag: MethodDefOrRefTag) = MethodDefOrRefMin + int tag.Tag

    [<Literal>]
    let MemberForwardedMin = 165

    [<Literal>]
    let MemberForwardedMax = 166

    let MemberForwarded (tag: MemberForwardedTag) = MemberForwardedMin + int tag.Tag

    [<Literal>]
    let ImplementationMin = 167

    [<Literal>]
    let ImplementationMax = 169

    let Implementation (tag: ImplementationTag) = ImplementationMin + int tag.Tag

    [<Literal>]
    let CustomAttributeTypeMin = 170

    [<Literal>]
    let CustomAttributeTypeMax = 173

    let CustomAttributeType (tag: CustomAttributeTypeTag) = CustomAttributeTypeMin + int tag.Tag

    [<Literal>]
    let ResolutionScopeMin = 174

    [<Literal>]
    let ResolutionScopeMax = 178

    let ResolutionScope (tag: ResolutionScopeTag) = ResolutionScopeMin + int tag.Tag

type CodedIndexDefinition =
    { TagBits: int
      Tables: int[] }

/// Canonical coded-index table orders for hot reload metadata sizing and serialization.
module CodedIndices =
    /// TypeDef(0), TypeRef(1), TypeSpec(2)
    let TypeDefOrRef =
        { TagBits = 2
          Tables =
            [| TableNames.TypeDef.Index
               TableNames.TypeRef.Index
               TableNames.TypeSpec.Index |] }

    /// TypeDef(0), MethodDef(1)
    let TypeOrMethodDef =
        { TagBits = 1
          Tables =
            [| TableNames.TypeDef.Index
               TableNames.Method.Index |] }

    /// Field(0), Param(1), Property(2)
    let HasConstant =
        { TagBits = 2
          Tables =
            [| TableNames.Field.Index
               TableNames.Param.Index
               TableNames.Property.Index |] }

    /// MethodDef(0), Field(1), TypeRef(2), TypeDef(3), Param(4), InterfaceImpl(5),
    /// MemberRef(6), Module(7), DeclSecurity(8), Property(9), Event(10), StandAloneSig(11),
    /// ModuleRef(12), TypeSpec(13), Assembly(14), AssemblyRef(15), File(16),
    /// ExportedType(17), ManifestResource(18), GenericParam(19), GenericParamConstraint(20), MethodSpec(21)
    let HasCustomAttribute =
        { TagBits = 5
          Tables =
            [| TableNames.Method.Index
               TableNames.Field.Index
               TableNames.TypeRef.Index
               TableNames.TypeDef.Index
               TableNames.Param.Index
               TableNames.InterfaceImpl.Index
               TableNames.MemberRef.Index
               TableNames.Module.Index
               TableNames.Permission.Index
               TableNames.Property.Index
               TableNames.Event.Index
               TableNames.StandAloneSig.Index
               TableNames.ModuleRef.Index
               TableNames.TypeSpec.Index
               TableNames.Assembly.Index
               TableNames.AssemblyRef.Index
               TableNames.File.Index
               TableNames.ExportedType.Index
               TableNames.ManifestResource.Index
               TableNames.GenericParam.Index
               TableNames.GenericParamConstraint.Index
               TableNames.MethodSpec.Index |] }

    /// Field(0), Param(1)
    let HasFieldMarshal =
        { TagBits = 1
          Tables =
            [| TableNames.Field.Index
               TableNames.Param.Index |] }

    /// TypeDef(0), MethodDef(1), Assembly(2)
    let HasDeclSecurity =
        { TagBits = 2
          Tables =
            [| TableNames.TypeDef.Index
               TableNames.Method.Index
               TableNames.Assembly.Index |] }

    /// TypeDef(0), TypeRef(1), ModuleRef(2), MethodDef(3), TypeSpec(4)
    let MemberRefParent =
        { TagBits = 3
          Tables =
            [| TableNames.TypeDef.Index
               TableNames.TypeRef.Index
               TableNames.ModuleRef.Index
               TableNames.Method.Index
               TableNames.TypeSpec.Index |] }

    /// Event(0), Property(1)
    let HasSemantics =
        { TagBits = 1
          Tables =
            [| TableNames.Event.Index
               TableNames.Property.Index |] }

    /// MethodDef(0), MemberRef(1)
    let MethodDefOrRef =
        { TagBits = 1
          Tables =
            [| TableNames.Method.Index
               TableNames.MemberRef.Index |] }

    /// Field(0), MethodDef(1)
    let MemberForwarded =
        { TagBits = 1
          Tables =
            [| TableNames.Field.Index
               TableNames.Method.Index |] }

    /// File(0), AssemblyRef(1), ExportedType(2)
    let Implementation =
        { TagBits = 2
          Tables =
            [| TableNames.File.Index
               TableNames.AssemblyRef.Index
               TableNames.ExportedType.Index |] }

    /// MethodDef(2), MemberRef(3)
    let CustomAttributeType =
        { TagBits = 3
          Tables =
            [| TableNames.Method.Index
               TableNames.MemberRef.Index |] }

    /// Module(0), ModuleRef(1), AssemblyRef(2), TypeRef(3)
    let ResolutionScope =
        { TagBits = 2
          Tables =
            [| TableNames.Module.Index
               TableNames.ModuleRef.Index
               TableNames.AssemblyRef.Index
               TableNames.TypeRef.Index |] }
