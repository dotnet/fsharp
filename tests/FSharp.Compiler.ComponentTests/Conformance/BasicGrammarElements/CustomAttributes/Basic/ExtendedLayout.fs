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

    [<Fact>]
    let ``ExtendedLayout on a struct emits the 0x18 layout flag and preserves the attribute`` () =
        let output =
            FSharp """
namespace Test

open System.Runtime.InteropServices

[<ExtendedLayout(ExtendedLayoutKind.CStruct)>]
type CStructLike =
    struct
        val mutable X: int
        val mutable Y: int
    end
"""
            |> asLibrary
            |> compile
            |> shouldSucceed
            |> getOutputPath

        use stream = File.OpenRead output
        use peReader = new PEReader(stream)
        let reader = peReader.GetMetadataReader()
        let typeDef = findType reader "CStructLike"

        // The extended layout is encoded as TypeAttributes value 0x18 (both the sequential and explicit layout bits set).
        let layout = typeDef.Attributes &&& TypeAttributes.LayoutMask
        Assert.Equal(0x18, int layout)

        // ExtendedLayoutAttribute is a real user-written attribute and must be preserved on the emitted type.
        let preserved =
            typeDef.GetCustomAttributes()
            |> Seq.map reader.GetCustomAttribute
            |> Seq.exists (fun ca -> customAttributeTypeName reader ca = "System.Runtime.InteropServices.ExtendedLayoutAttribute")
        Assert.True(preserved, "ExtendedLayoutAttribute should be preserved on the emitted type.")

    [<Fact>]
    let ``ExtendedLayout on a struct record emits the 0x18 layout flag and preserves the attribute`` () =
        let output =
            FSharp """
namespace Test

open System.Runtime.InteropServices

[<Struct; ExtendedLayout(ExtendedLayoutKind.CStruct)>]
type StructRecord = { X: int; Y: int }
"""
            |> asLibrary
            |> compile
            |> shouldSucceed
            |> getOutputPath

        use stream = File.OpenRead output
        use peReader = new PEReader(stream)
        let reader = peReader.GetMetadataReader()
        let typeDef = findType reader "StructRecord"

        let layout = typeDef.Attributes &&& TypeAttributes.LayoutMask
        Assert.Equal(0x18, int layout)

        let preserved =
            typeDef.GetCustomAttributes()
            |> Seq.map reader.GetCustomAttribute
            |> Seq.exists (fun ca -> customAttributeTypeName reader ca = "System.Runtime.InteropServices.ExtendedLayoutAttribute")
        Assert.True(preserved, "ExtendedLayoutAttribute should be preserved on the emitted struct record.")

    [<Fact>]
    let ``ExtendedLayout and StructLayout cannot be combined on a struct record`` () =
        FSharp """
namespace Test

open System.Runtime.InteropServices

[<Struct; ExtendedLayout(ExtendedLayoutKind.CStruct); StructLayout(LayoutKind.Sequential)>]
type BothOnRecord = { X: int }
"""
        |> asLibrary
        |> compile
        |> shouldFail
        |> withErrorCode 3910

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
        FSharp """
namespace Test

open System.Runtime.InteropServices

[<ExtendedLayout(ExtendedLayoutKind.CStruct)>]
type NotAStruct() =
    member _.X = 1
"""
        |> asLibrary
        |> compile
        |> shouldFail
        |> withErrorCode 3911
        |> withErrorMessage "Only structs may be given the 'ExtendedLayoutAttribute'"

    [<Fact>]
    let ``ExtendedLayout on an interface is rejected`` () =
        FSharp """
namespace Test

open System.Runtime.InteropServices

[<ExtendedLayout(ExtendedLayoutKind.CStruct)>]
type IExtended =
    abstract M: unit -> int
"""
        |> asLibrary
        |> compile
        |> shouldFail
        |> withErrorCode 3911

    [<Fact>]
    let ``ExtendedLayout on a reference record is rejected`` () =
        FSharp """
namespace Test

open System.Runtime.InteropServices

[<ExtendedLayout(ExtendedLayoutKind.CStruct)>]
type R = { X: int }
"""
        |> asLibrary
        |> compile
        |> shouldFail
        |> withErrorCode 3911

    [<Fact>]
    let ``ExtendedLayout on a union is rejected`` () =
        FSharp """
namespace Test

open System.Runtime.InteropServices

[<ExtendedLayout(ExtendedLayoutKind.CStruct)>]
type U = A | B
"""
        |> asLibrary
        |> compile
        |> shouldFail
        |> withErrorCode 3911

    [<Fact>]
    let ``ExtendedLayout on a struct union is rejected`` () =
        FSharp """
namespace Test

open System.Runtime.InteropServices

[<Struct; ExtendedLayout(ExtendedLayoutKind.CStruct)>]
type U = A of x: int | B of y: int
"""
        |> asLibrary
        |> compile
        |> shouldFail
        |> withErrorCode 3911

    [<Fact>]
    let ``ExtendedLayout on an enum is rejected`` () =
        FSharp """
namespace Test

open System.Runtime.InteropServices

[<ExtendedLayout(ExtendedLayoutKind.CStruct)>]
type E =
    | A = 0
    | B = 1
"""
        |> asLibrary
        |> compile
        |> shouldFail
        |> withErrorCode 3911

    [<Fact>]
    let ``ExtendedLayout on a delegate is rejected`` () =
        FSharp """
namespace Test

open System.Runtime.InteropServices

[<ExtendedLayout(ExtendedLayoutKind.CStruct)>]
type D = delegate of int -> int
"""
        |> asLibrary
        |> compile
        |> shouldFail
        |> withErrorCode 3911

    [<Fact>]
    let ``FieldOffset is not allowed on an ExtendedLayout struct`` () =
        FSharp """
namespace Test

open System.Runtime.InteropServices

[<ExtendedLayout(ExtendedLayoutKind.CStruct)>]
type WithOffset =
    struct
        [<FieldOffset(0)>] val mutable X: int
    end
"""
        |> asLibrary
        |> compile
        |> shouldFail
        |> withErrorCode 1211
#endif
