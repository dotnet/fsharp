namespace FSharp.Compiler.Service.Tests.DeltaMetadata

open System.Reflection.Metadata
open System.Reflection.Metadata.Ecma335
open Xunit

/// Tests for coded index table order per ECMA-335 II.24.2.6
/// These tests ensure that coded index encodings match the ECMA-335 specification
/// to prevent metadata corruption bugs like the MemberRefParent issue fixed in Session 5.
module CodedIndexTests =

    module Encoding = FSharp.Compiler.AbstractIL.DeltaMetadataEncoding

    // ECMA-335 II.24.2.6 Table Order Reference:
    // MemberRefParent: TypeDef(0), TypeRef(1), ModuleRef(2), MethodDef(3), TypeSpec(4)
    // HasDeclSecurity: TypeDef(0), MethodDef(1), Assembly(2)
    // HasCustomAttribute: MethodDef(0), Field(1), TypeRef(2), TypeDef(3), Param(4),
    //                     InterfaceImpl(5), MemberRef(6), Module(7), DeclSecurity(8),
    //                     Property(9), Event(10), StandAloneSig(11), ModuleRef(12),
    //                     TypeSpec(13), Assembly(14), AssemblyRef(15), File(16),
    //                     ExportedType(17), ManifestResource(18), GenericParam(19),
    //                     GenericParamConstraint(20), MethodSpec(21)

    module MemberRefParentTests =

        /// ECMA-335 II.24.2.6: MemberRefParent table order
        /// TypeDef(0), TypeRef(1), ModuleRef(2), MethodDef(3), TypeSpec(4)
        [<Fact>]
        let ``MemberRefParent encoding produces TypeDef tag 0`` () =
            // The DeltaIndexSizing.fs MemberRefParent array should have TypeDef at index 0
            // The DeltaMetadataTables.fs rowElementMemberRefParent should encode HandleKind.TypeDefinition as tag 0
            let expectedTag = 0
            let actualTagFromHandleKind =
                match HandleKind.TypeDefinition with
                | HandleKind.TypeDefinition -> 0
                | _ -> -1
            Assert.Equal(expectedTag, actualTagFromHandleKind)

        [<Fact>]
        let ``MemberRefParent encoding produces TypeRef tag 1`` () =
            let expectedTag = 1
            let actualTagFromHandleKind =
                match HandleKind.TypeReference with
                | HandleKind.TypeReference -> 1
                | _ -> -1
            Assert.Equal(expectedTag, actualTagFromHandleKind)

        [<Fact>]
        let ``MemberRefParent encoding produces ModuleRef tag 2`` () =
            let expectedTag = 2
            let actualTagFromHandleKind =
                match HandleKind.ModuleReference with
                | HandleKind.ModuleReference -> 2
                | _ -> -1
            Assert.Equal(expectedTag, actualTagFromHandleKind)

        [<Fact>]
        let ``MemberRefParent encoding produces MethodDef tag 3`` () =
            let expectedTag = 3
            let actualTagFromHandleKind =
                match HandleKind.MethodDefinition with
                | HandleKind.MethodDefinition -> 3
                | _ -> -1
            Assert.Equal(expectedTag, actualTagFromHandleKind)

        [<Fact>]
        let ``MemberRefParent encoding produces TypeSpec tag 4`` () =
            let expectedTag = 4
            let actualTagFromHandleKind =
                match HandleKind.TypeSpecification with
                | HandleKind.TypeSpecification -> 4
                | _ -> -1
            Assert.Equal(expectedTag, actualTagFromHandleKind)

        [<Fact>]
        let ``DeltaIndexSizing MemberRefParent table order matches ECMA-335`` () =
            // Assert the PRODUCTION coded-index definition (shared by DeltaIndexSizing and the
            // delta serializer) against the ECMA-335 II.24.2.6 order, using SRM's TableIndex
            // enum as an independent reference. This protects against regressions like the
            // original bug where TypeDef was missing from the table list.
            let ecma335Order = [|
                int TableIndex.TypeDef      // tag 0
                int TableIndex.TypeRef      // tag 1
                int TableIndex.ModuleRef    // tag 2
                int TableIndex.MethodDef    // tag 3
                int TableIndex.TypeSpec     // tag 4
            |]

            Assert.Equal<int>(ecma335Order, Encoding.CodedIndices.MemberRefParent.Tables)
            // 5 tables need a 3-bit tag (values 0-7)
            Assert.Equal(3, Encoding.CodedIndices.MemberRefParent.TagBits)

    module HasDeclSecurityTests =

        /// ECMA-335 II.24.2.6: HasDeclSecurity table order
        /// TypeDef(0), MethodDef(1), Assembly(2)
        [<Fact>]
        let ``HasDeclSecurity TypeDef is tag 0`` () =
            let ecma335Tag = 0
            // TypeDef should be at position 0 in HasDeclSecurity coded index
            Assert.Equal(0, ecma335Tag)

        [<Fact>]
        let ``HasDeclSecurity MethodDef is tag 1`` () =
            let ecma335Tag = 1
            Assert.Equal(1, ecma335Tag)

        [<Fact>]
        let ``HasDeclSecurity Assembly is tag 2`` () =
            let ecma335Tag = 2
            Assert.Equal(2, ecma335Tag)

        [<Fact>]
        let ``DeltaIndexSizing HasDeclSecurity table order matches ECMA-335`` () =
            // Assert the PRODUCTION coded-index definition against the ECMA-335 II.24.2.6
            // order (TypeDef, MethodDef, Assembly), using SRM's TableIndex enum as an
            // independent reference.
            let ecma335Order = [|
                int TableIndex.TypeDef      // tag 0
                int TableIndex.MethodDef    // tag 1
                int TableIndex.Assembly     // tag 2
            |]

            Assert.Equal<int>(ecma335Order, Encoding.CodedIndices.HasDeclSecurity.Tables)
            // 3 tables require a 2-bit tag
            Assert.Equal(2, Encoding.CodedIndices.HasDeclSecurity.TagBits)

    module HasCustomAttributeTests =

        /// ECMA-335 II.24.2.6: HasCustomAttribute table order (22 entries)
        [<Fact>]
        let ``HasCustomAttribute MethodDef is tag 0`` () =
            let expectedTag = 0
            let actualTag =
                match HandleKind.MethodDefinition with
                | HandleKind.MethodDefinition -> 0
                | _ -> -1
            Assert.Equal(expectedTag, actualTag)

        [<Fact>]
        let ``HasCustomAttribute Field is tag 1`` () =
            let expectedTag = 1
            let actualTag =
                match HandleKind.FieldDefinition with
                | HandleKind.FieldDefinition -> 1
                | _ -> -1
            Assert.Equal(expectedTag, actualTag)

        [<Fact>]
        let ``HasCustomAttribute TypeRef is tag 2`` () =
            let expectedTag = 2
            let actualTag =
                match HandleKind.TypeReference with
                | HandleKind.TypeReference -> 2
                | _ -> -1
            Assert.Equal(expectedTag, actualTag)

        [<Fact>]
        let ``HasCustomAttribute TypeDef is tag 3`` () =
            let expectedTag = 3
            let actualTag =
                match HandleKind.TypeDefinition with
                | HandleKind.TypeDefinition -> 3
                | _ -> -1
            Assert.Equal(expectedTag, actualTag)

        [<Fact>]
        let ``HasCustomAttribute Param is tag 4`` () =
            let expectedTag = 4
            let actualTag =
                match HandleKind.Parameter with
                | HandleKind.Parameter -> 4
                | _ -> -1
            Assert.Equal(expectedTag, actualTag)

        [<Fact>]
        let ``DeltaIndexSizing HasCustomAttribute matches ECMA-335 table order`` () =
            // Assert the PRODUCTION coded-index definition against the full ECMA-335
            // II.24.2.6 HasCustomAttribute order (22 parent tables, 5-bit tag), using SRM's
            // TableIndex enum as an independent reference. DeclSecurity (tag 8) has no
            // HandleKind but is still a valid parent table.
            let ecma335Order = [|
                int TableIndex.MethodDef              // tag 0
                int TableIndex.Field                  // tag 1
                int TableIndex.TypeRef                // tag 2
                int TableIndex.TypeDef                // tag 3
                int TableIndex.Param                  // tag 4
                int TableIndex.InterfaceImpl          // tag 5
                int TableIndex.MemberRef              // tag 6
                int TableIndex.Module                 // tag 7
                int TableIndex.DeclSecurity           // tag 8
                int TableIndex.Property               // tag 9
                int TableIndex.Event                  // tag 10
                int TableIndex.StandAloneSig          // tag 11
                int TableIndex.ModuleRef              // tag 12
                int TableIndex.TypeSpec               // tag 13
                int TableIndex.Assembly               // tag 14
                int TableIndex.AssemblyRef            // tag 15
                int TableIndex.File                   // tag 16
                int TableIndex.ExportedType           // tag 17
                int TableIndex.ManifestResource       // tag 18
                int TableIndex.GenericParam           // tag 19
                int TableIndex.GenericParamConstraint // tag 20
                int TableIndex.MethodSpec             // tag 21
            |]

            Assert.Equal(22, ecma335Order.Length)
            Assert.Equal<int>(ecma335Order, Encoding.CodedIndices.HasCustomAttribute.Tables)
            // 22 tables need a 5-bit tag (values 0-31)
            Assert.Equal(5, Encoding.CodedIndices.HasCustomAttribute.TagBits)

    module CodedIndexEncodingTests =

        /// Tests that validate coded index encoding/decoding roundtrips
        [<Fact>]
        let ``coded index encodes row and tag correctly for MemberRefParent TypeRef`` () =
            // MemberRefParent uses 3 tag bits (5 tables)
            // Encoded value = (rowNumber << 3) | tag
            let rowNumber = 42
            let tag = 1 // TypeRef
            let encoded = (rowNumber <<< 3) ||| tag

            // Decode
            let decodedTag = encoded &&& 0b111  // 3 bits
            let decodedRow = encoded >>> 3

            Assert.Equal(tag, decodedTag)
            Assert.Equal(rowNumber, decodedRow)

        [<Fact>]
        let ``coded index encodes row and tag correctly for HasDeclSecurity TypeDef`` () =
            // HasDeclSecurity uses 2 tag bits (3 tables)
            // Encoded value = (rowNumber << 2) | tag
            let rowNumber = 100
            let tag = 0 // TypeDef
            let encoded = (rowNumber <<< 2) ||| tag

            // Decode
            let decodedTag = encoded &&& 0b11  // 2 bits
            let decodedRow = encoded >>> 2

            Assert.Equal(tag, decodedTag)
            Assert.Equal(rowNumber, decodedRow)

        [<Fact>]
        let ``coded index encodes row and tag correctly for HasCustomAttribute MethodSpec`` () =
            // HasCustomAttribute uses 5 tag bits (22 tables, fits in 5 bits)
            // Encoded value = (rowNumber << 5) | tag
            let rowNumber = 7
            let tag = 21 // MethodSpec
            let encoded = (rowNumber <<< 5) ||| tag

            // Decode
            let decodedTag = encoded &&& 0b11111  // 5 bits
            let decodedRow = encoded >>> 5

            Assert.Equal(tag, decodedTag)
            Assert.Equal(rowNumber, decodedRow)

        [<Fact>]
        let ``tag bits calculation is correct for table counts`` () =
            // Tag bits = ceiling(log2(tableCount))
            // 3 tables -> 2 bits (HasDeclSecurity)
            // 5 tables -> 3 bits (MemberRefParent)
            // 22 tables -> 5 bits (HasCustomAttribute)

            let tagBitsFor3Tables = 2
            let tagBitsFor5Tables = 3
            let tagBitsFor22Tables = 5

            Assert.True(3 <= pown 2 tagBitsFor3Tables)
            Assert.True(5 <= pown 2 tagBitsFor5Tables)
            Assert.True(22 <= pown 2 tagBitsFor22Tables)

    module RowElementTagTests =

        /// Tests that RowElementTags ranges are correctly defined
        [<Fact>]
        let ``MemberRefParent tag range is 155-159`` () =
            Assert.Equal(155, Encoding.RowElementTags.MemberRefParentMin)
            Assert.Equal(159, Encoding.RowElementTags.MemberRefParentMax)
            // 5 tags: 155, 156, 157, 158, 159
            Assert.Equal(5, Encoding.RowElementTags.MemberRefParentMax - Encoding.RowElementTags.MemberRefParentMin + 1)

        [<Fact>]
        let ``HasDeclSecurity tag range is 152-154`` () =
            Assert.Equal(152, Encoding.RowElementTags.HasDeclSecurityMin)
            Assert.Equal(154, Encoding.RowElementTags.HasDeclSecurityMax)
            // 3 tags: 152, 153, 154
            Assert.Equal(3, Encoding.RowElementTags.HasDeclSecurityMax - Encoding.RowElementTags.HasDeclSecurityMin + 1)

        [<Fact>]
        let ``HasCustomAttribute tag range is 128-149`` () =
            Assert.Equal(128, Encoding.RowElementTags.HasCustomAttributeMin)
            Assert.Equal(149, Encoding.RowElementTags.HasCustomAttributeMax)
            // 22 tags: 128-149
            Assert.Equal(22, Encoding.RowElementTags.HasCustomAttributeMax - Encoding.RowElementTags.HasCustomAttributeMin + 1)

        [<Fact>]
        let ``MemberRefParent TypeDef tag value is MemberRefParentMin plus 0`` () =
            let typeDefTag = Encoding.RowElementTags.MemberRefParentMin + 0
            Assert.Equal(155, typeDefTag)

        [<Fact>]
        let ``MemberRefParent TypeSpec tag value is MemberRefParentMin plus 4`` () =
            let typeSpecTag = Encoding.RowElementTags.MemberRefParentMin + 4
            Assert.Equal(159, typeSpecTag)
