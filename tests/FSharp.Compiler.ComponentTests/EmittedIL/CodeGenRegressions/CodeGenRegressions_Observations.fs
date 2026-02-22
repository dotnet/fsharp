// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace EmittedIL

open Xunit
open FSharp.Test
open FSharp.Test.Compiler
open FSharp.Test.Utilities

module CodeGenRegressions_Observations =
    // https://github.com/dotnet/fsharp/issues/13100
    [<Fact>]
    let ``Issue_13100_PlatformCharacteristic`` () =
        let source = """
module PlatformTest

[<EntryPoint>]
let main _ = 0
"""
        FSharp source
        |> asExe
        |> withPlatform ExecutionPlatform.X64
        |> compile
        |> shouldSucceed
        |> withPeReader (fun rdr -> 
            let characteristics = rdr.PEHeaders.CoffHeader.Characteristics
            if not (characteristics.HasFlag(System.Reflection.PortableExecutable.Characteristics.LargeAddressAware)) then
                failwith $"x64 binary should have LargeAddressAware flag. Found: {characteristics}"
            if characteristics.HasFlag(System.Reflection.PortableExecutable.Characteristics.Bit32Machine) then
                failwith $"x64 binary should NOT have Bit32Machine flag. Found: {characteristics}")
        |> ignore


    // https://github.com/dotnet/fsharp/issues/11935
    [<Fact>]
    let ``Issue_11935_UnmanagedConstraintInterop`` () =
        let source = """
module Test

let test<'T when 'T : unmanaged> (x: 'T) = x
"""
        FSharp source
        |> asLibrary
        |> withLangVersion10
        |> compile
        |> shouldSucceed
        |> verifyILContains [
            ".method public static !!T  test<valuetype (class [runtime]System.ValueType modreq([runtime]System.Runtime.InteropServices.UnmanagedType)) T>(!!T x) cil managed"
        ]
        |> shouldSucceed


    [<Fact>]
    let ``Issue_11935_UnmanagedConstraintInterop_ClassType`` () =
        let source = """
module Test

type Container<'T when 'T : unmanaged>() =
    member _.GetDefault() : 'T = Unchecked.defaultof<'T>
"""
        FSharp source
        |> asLibrary
        |> withLangVersion10
        |> compile
        |> shouldSucceed
        |> verifyILContains [
            "Container`1<valuetype (class [runtime]System.ValueType modreq([runtime]System.Runtime.InteropServices.UnmanagedType)) T>"
        ]
        |> shouldSucceed


    [<Fact>]
    let ``Issue_11935_UnmanagedConstraintInterop_StructType`` () =
        let source = """
module Test

[<Struct>]
type StructContainer<'T when 'T : unmanaged> =
    val Value : 'T
    new(v) = { Value = v }
"""
        FSharp source
        |> asLibrary
        |> withLangVersion10
        |> compile
        |> shouldSucceed
        |> verifyILContains [
            "StructContainer`1<valuetype (class [runtime]System.ValueType modreq([runtime]System.Runtime.InteropServices.UnmanagedType)) T>"
        ]
        |> shouldSucceed


    [<Fact>]
    let ``Issue_11935_UnmanagedConstraintInterop_InstanceMethod`` () =
        let source = """
module Test

type Processor() =
    member _.Process<'T when 'T : unmanaged>(x: 'T) = x
"""
        FSharp source
        |> asLibrary
        |> withLangVersion10
        |> compile
        |> shouldSucceed
        |> verifyILContains [
            ".method public hidebysig instance !!T Process<valuetype (class [runtime]System.ValueType modreq([runtime]System.Runtime.InteropServices.UnmanagedType)) T>(!!T x) cil managed"
        ]
        |> shouldSucceed


    [<Fact>]
    let ``Issue_11935_UnmanagedConstraintInterop_MultipleTypeParams`` () =
        let source = """
module Test

let combine<'T, 'U when 'T : unmanaged and 'U : unmanaged> (x: 'T) (y: 'U) = struct(x, y)
"""
        FSharp source
        |> asLibrary
        |> withLangVersion10
        |> compile
        |> shouldSucceed
        |> verifyILContains [
            "combine<valuetype (class [runtime]System.ValueType modreq([runtime]System.Runtime.InteropServices.UnmanagedType)) T,valuetype (class [runtime]System.ValueType modreq([runtime]System.Runtime.InteropServices.UnmanagedType)) U>(!!T x, !!U y) cil managed"
        ]
        |> shouldSucceed


    // https://github.com/dotnet/fsharp/issues/7861
    [<Fact>]
    let ``Issue_7861_AttributeTypeReference`` () =
        let source = """
module Test

open System

type TypedAttribute(t: Type) =
    inherit Attribute()
    member _.TargetType = t

[<Typed(typeof<System.Xml.XmlDocument>)>]
type MyClass() = class end
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> verifyAssemblyReference "System.Xml"
        |> ignore


    [<Fact>]
    let ``Issue_7861_NamedAttributeArgument`` () =
        let source = """
module Test

open System

type TypePropertyAttribute() =
    inherit Attribute()
    member val TargetType : Type = null with get, set

[<TypeProperty(TargetType = typeof<System.Xml.XmlDocument>)>]
type MyClass() = class end
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> verifyAssemblyReference "System.Xml"
        |> ignore


    [<Fact>]
    let ``Issue_7861_AttributeOnMethod`` () =
        let source = """
module Test

open System

type TypedAttribute(t: Type) =
    inherit Attribute()
    member _.TargetType = t

type MyClass() =
    [<Typed(typeof<System.Xml.XmlDocument>)>]
    member _.MyMethod() = ()
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> verifyAssemblyReference "System.Xml"
        |> ignore
