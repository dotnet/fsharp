// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.BasicGrammarElements

open Xunit
open FSharp.Test.Compiler

#if NETCOREAPP
open System.IO
open System.Reflection
open System.Reflection.Metadata
open System.Reflection.PortableExecutable
#endif

module CustomAttributes_ExtendedLayout =

#if !NETCOREAPP
    // Every test in this module requires System.Runtime.InteropServices.ExtendedLayoutAttribute,
    // which only exists starting with .NET 11. On other target frameworks (e.g. net472) the module
    // would otherwise be empty, which is not a valid module declaration, so keep one placeholder.
    let private _requiresNetCore = ()
#endif

#if NETCOREAPP
    // System.Runtime.InteropServices.ExtendedLayoutAttribute / ExtendedLayoutKind only exist starting with .NET 11,
    // so these tests are gated to the .NET (Core) test flavor where that BCL is referenced.

    let private getOutputPath result =
        match result with
        | CompilationResult.Success success ->
            match success.OutputPath with
            | Some path -> path
            | None -> failwith "Compilation succeeded but produced no output path."
        | CompilationResult.Failure failure ->
            failwithf "Compilation was expected to succeed, but failed with: %A" failure.Diagnostics

    let private findType (reader: MetadataReader) name =
        reader.TypeDefinitions
        |> Seq.map reader.GetTypeDefinition
        |> Seq.find (fun td -> reader.GetString td.Name = name)

    let private customAttributeTypeName (reader: MetadataReader) (ca: CustomAttribute) =
        match ca.Constructor.Kind with
        | HandleKind.MemberReference ->
            let mref = reader.GetMemberReference(MemberReferenceHandle.op_Explicit ca.Constructor)
            match mref.Parent.Kind with
            | HandleKind.TypeReference ->
                let tref = reader.GetTypeReference(TypeReferenceHandle.op_Explicit mref.Parent)
                reader.GetString tref.Namespace + "." + reader.GetString tref.Name
            | _ -> ""
        | _ -> ""

    /// Compile the source, then assert the named type carries the 0x18 extended layout flag
    /// and still emits a real ExtendedLayoutAttribute (unlike StructLayout, which is stripped).
    let private assertExtendedLayoutEmitted typeName source =
        let output =
            FSharp source
            |> asLibrary
            |> compile
            |> shouldSucceed
            |> getOutputPath

        use stream = File.OpenRead output
        use peReader = new PEReader(stream)
        let reader = peReader.GetMetadataReader()
        let typeDef = findType reader typeName

        // Extended layout is encoded as TypeAttributes value 0x18 (both sequential and explicit layout bits set).
        let layout = typeDef.Attributes &&& TypeAttributes.LayoutMask
        Assert.Equal(0x18, int layout)

        let preserved =
            typeDef.GetCustomAttributes()
            |> Seq.map reader.GetCustomAttribute
            |> Seq.exists (fun ca -> customAttributeTypeName reader ca = "System.Runtime.InteropServices.ExtendedLayoutAttribute")
        Assert.True(preserved, "ExtendedLayoutAttribute should be preserved on the emitted type.")

    let private expectRejected errorCode source =
        FSharp source
        |> asLibrary
        |> compile
        |> shouldFail
        |> withErrorCode errorCode

    let private expectRejectedWithMessage errorCode message source =
        source
        |> expectRejected errorCode
        |> withErrorMessage message

    [<Fact>]
    let ``ExtendedLayout on a struct emits the 0x18 layout flag and preserves the attribute`` () =
        assertExtendedLayoutEmitted "CStructLike" """
namespace Test

open System.Runtime.InteropServices

[<ExtendedLayout(ExtendedLayoutKind.CStruct)>]
type CStructLike =
    struct
        val mutable X: int
        val mutable Y: int
    end
"""

    [<Fact>]
    let ``ExtendedLayout on a struct record emits the 0x18 layout flag and preserves the attribute`` () =
        assertExtendedLayoutEmitted "StructRecord" """
namespace Test

open System.Runtime.InteropServices

[<Struct; ExtendedLayout(ExtendedLayoutKind.CStruct)>]
type StructRecord = { X: int; Y: int }
"""

    [<Fact>]
    let ``ExtendedLayout and StructLayout cannot be combined on a struct record`` () =
        expectRejected 3910 """
namespace Test

open System.Runtime.InteropServices

[<Struct; ExtendedLayout(ExtendedLayoutKind.CStruct); StructLayout(LayoutKind.Sequential)>]
type BothOnRecord = { X: int }
"""

    [<Fact>]
    let ``ExtendedLayout and StructLayout cannot be combined on the same type`` () =
        FSharp """
namespace Test

open System.Runtime.InteropServices

[<ExtendedLayout(ExtendedLayoutKind.CStruct)>]
[<StructLayout(LayoutKind.Sequential)>]
type BothAttrs =
    struct
        val mutable X: int
    end
"""
        |> asLibrary
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Error 3910, Line 8, Col 6, Line 8, Col 15, "The attributes 'StructLayoutAttribute' and 'ExtendedLayoutAttribute' cannot be used together on the same type")

    [<Fact>]
    let ``ExtendedLayout on a class is rejected`` () =
        expectRejectedWithMessage 3911 "Only structs may be given the 'ExtendedLayoutAttribute'" """
namespace Test

open System.Runtime.InteropServices

[<ExtendedLayout(ExtendedLayoutKind.CStruct)>]
type NotAStruct() =
    member _.X = 1
"""

    [<Fact>]
    let ``ExtendedLayout on an interface is rejected`` () =
        expectRejected 3911 """
namespace Test

open System.Runtime.InteropServices

[<ExtendedLayout(ExtendedLayoutKind.CStruct)>]
type IExtended =
    abstract M: unit -> int
"""

    [<Fact>]
    let ``ExtendedLayout on a reference record is rejected`` () =
        expectRejected 3911 """
namespace Test

open System.Runtime.InteropServices

[<ExtendedLayout(ExtendedLayoutKind.CStruct)>]
type R = { X: int }
"""

    [<Fact>]
    let ``ExtendedLayout on a union is rejected`` () =
        expectRejectedWithMessage 3913 "The 'ExtendedLayoutAttribute' cannot be applied to discriminated unions" """
namespace Test

open System.Runtime.InteropServices

[<ExtendedLayout(ExtendedLayoutKind.CStruct)>]
type U = A | B
"""

    [<Fact>]
    let ``ExtendedLayout on a struct union is rejected`` () =
        expectRejectedWithMessage 3913 "The 'ExtendedLayoutAttribute' cannot be applied to discriminated unions" """
namespace Test

open System.Runtime.InteropServices

[<Struct; ExtendedLayout(ExtendedLayoutKind.CStruct)>]
type U = A of x: int | B of y: int
"""

    [<Fact>]
    let ``ExtendedLayout on an enum is rejected`` () =
        expectRejected 3911 """
namespace Test

open System.Runtime.InteropServices

[<ExtendedLayout(ExtendedLayoutKind.CStruct)>]
type E =
    | A = 0
    | B = 1
"""

    [<Fact>]
    let ``ExtendedLayout on a delegate is rejected`` () =
        expectRejected 3911 """
namespace Test

open System.Runtime.InteropServices

[<ExtendedLayout(ExtendedLayoutKind.CStruct)>]
type D = delegate of int -> int
"""

    [<Fact>]
    let ``FieldOffset is not allowed on an ExtendedLayout struct`` () =
        expectRejected 1211 """
namespace Test

open System.Runtime.InteropServices

[<ExtendedLayout(ExtendedLayoutKind.CStruct)>]
type WithOffset =
    struct
        [<FieldOffset(0)>] val mutable X: int
    end
"""

    [<Fact>]
    let ``StructLayout with the reserved extended-layout value on a non-struct reports the struct-only error`` () =
        expectRejected 937 """
namespace Test

open System.Runtime.InteropServices

[<StructLayout(enum<LayoutKind>(1))>]
type U = A | B
"""

    [<Fact>]
    let ``ExtendedLayout on a generic struct emits the 0x18 layout flag and preserves the attribute`` () =
        // The IL type name of a generic type carries the backtick arity suffix.
        assertExtendedLayoutEmitted "GenericStruct`1" """
namespace Test

open System.Runtime.InteropServices

[<ExtendedLayout(ExtendedLayoutKind.CStruct)>]
type GenericStruct<'T> =
    struct
        val mutable Value: 'T
    end
"""

    // Reflection.Emit (used by FSI) has its own type-def path (ilreflect.fs), separate from the static
    // metadata writer, so it is exercised directly here under both single- and multi-emit modes.
    [<Theory>]
    [<InlineData("--multiemit-")>]
    [<InlineData("--multiemit+")>]
    let ``FSI preserves the extended layout and attribute for a dynamically emitted struct`` (multiEmit: string) =
        Fsx """
open System
open System.Reflection
open System.Runtime.InteropServices

[<ExtendedLayout(ExtendedLayoutKind.CUnion)>]
type CUnionLike =
    struct
        val mutable X: int
        val mutable Y: int
    end

if int (typeof<CUnionLike>.Attributes &&& TypeAttributes.LayoutMask) <> 0x18 then
    failwith "Expected the 0x18 extended layout flag on the dynamically emitted type."

let arg =
    CustomAttributeData.GetCustomAttributes(typeof<CUnionLike>)
    |> Seq.find (fun a -> a.AttributeType = typeof<ExtendedLayoutAttribute>)
    |> fun a -> a.ConstructorArguments.[0]

if Convert.ToInt32 arg.Value <> int ExtendedLayoutKind.CUnion then
    failwith "Expected the CUnion extended layout kind to be preserved as the attribute argument."
"""
        |> withOptions [multiEmit]
        |> eval
        |> shouldSucceed

    [<Fact>]
    let ``An extended layout struct can be consumed from a referenced F# assembly`` () =
        // Consuming the type forces the metadata reader (ilread.fs) to decode the 0x18 layout flag back to Extended.
        let producer =
            FSharp """
namespace Producer

open System.Runtime.InteropServices

[<Struct; ExtendedLayout(ExtendedLayoutKind.CStruct)>]
type Point = { X: int; Y: int }
"""
            |> asLibrary
            |> withName "ExtendedLayoutProducer"

        FSharp """
module Consumer

open Producer

let sum (p: Point) = p.X + p.Y
"""
        |> asLibrary
        |> withReferences [producer]
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``ExtendedLayout on a struct with no instance fields is rejected`` () =
        // The runtime derives an extended-layout type's size from its fields, so an empty one would
        // emit invalid metadata that fails to load. Reject it at compile time instead.
        expectRejectedWithMessage 3914 "A struct with the 'ExtendedLayoutAttribute' must have at least one instance field" """
namespace Test

open System.Runtime.InteropServices

[<ExtendedLayout(ExtendedLayoutKind.CStruct)>]
type Empty =
    struct
    end
"""

    [<Fact>]
    let ``ExtendedLayout on a struct with only static fields is rejected`` () =
        // Static fields do not participate in the type's layout, so a struct with only static fields
        // is empty as far as extended layout is concerned and must also be rejected.
        expectRejectedWithMessage 3914 "A struct with the 'ExtendedLayoutAttribute' must have at least one instance field" """
namespace Test

open System.Runtime.InteropServices

[<ExtendedLayout(ExtendedLayoutKind.CStruct)>]
type OnlyStatic =
    struct
        [<DefaultValue>] static val mutable private X: int
    end
"""
#endif
