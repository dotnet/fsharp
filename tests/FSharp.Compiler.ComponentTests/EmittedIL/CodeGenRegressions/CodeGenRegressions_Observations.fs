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


    // https://github.com/dotnet/fsharp/issues/19428
    [<Fact>]
    let ``Issue_19428_CompilationMappingOnGenericValue`` () =
        FSharp """
module GenericValueTest

let l = []
let empty<'T> = Seq.empty<'T>
"""
        |> compile
        |> shouldSucceed
        |> verifyIL [
            """
  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!a> l<a>() cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 )
"""
            """
  .method public static class [runtime]System.Collections.Generic.IEnumerable`1<!!T> empty<T>() cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 )
"""
        ]

    // https://github.com/dotnet/fsharp/issues/18128
    // Concrete-type Unchecked.defaultof bindings should be eliminated under optimization.
    // Pins both the absence of initobj and that no decimal local slot is allocated for
    // any of the three discarded bindings.
    [<Fact>]
    let ``Issue_18128_Unchecked_defaultof_concrete_eliminated`` () =
        FSharp """
module Test

open System

let f (n: float32) =
    Console.WriteLine n
    let _ = Unchecked.defaultof<decimal>
    let _ = Unchecked.defaultof<decimal>
    let _ = Unchecked.defaultof<decimal>
    let n' = n * 2.f
    Console.WriteLine n'
"""
        |> asLibrary
        |> withOptimize
        |> compile
        |> shouldSucceed
        |> verifyILNotPresent [ "initobj"; "valuetype [runtime]System.Decimal" ]
        |> ignore

    // https://github.com/dotnet/fsharp/issues/18128
    // The real-world FSharpPlus-style SRTP witness pattern from the issue. After elimination,
    // doWork reduces to a direct double-precision multiplication; the nil<PreOps> and nil<^b>
    // witness bindings are gone.
    [<Fact>]
    let ``Issue_18128_SRTP_witness_pattern_compiles_and_optimizes`` () =
        FSharp """
module Test

open System.ComponentModel
open FSharp.Core.LanguagePrimitives

[<AbstractClass; Sealed; EditorBrowsable(EditorBrowsableState.Never)>]
type PreOps =
    static member inline Double (n: float<'u>) : float<'u> = n * 2.
    static member inline Double (n: float32<'u>) : float32<'u> = n * 2.f

#nowarn "64"
module PreludeOperators =
    let inline private nil<'T> = Unchecked.defaultof<'T>
    let inline double (x: ^a) =
        let inline _call (_: ^M, input: ^I, _: ^R) = ((^M or ^I) : (static member Double : ^I -> ^R) input)
        _call (nil<PreOps>, x, nil< ^b >)

open PreludeOperators
let doWork (n: float) = double n
"""
        |> asLibrary
        |> withOptimize
        |> compile
        |> shouldSucceed
        |> verifyILNotPresent [ "initobj"; "ldnull" ]
        |> ignore

    // Soundness pin: optimizing away an unused `Unchecked.defaultof<T>` must not introduce
    // a new reference to T in the enclosing method. `defaultof` of a reference type lowers
    // to `ldnull` (not `newobj`), so the binding's removal cannot suppress an observable
    // cctor call - f's body should contain no reference to WithCctor at all.
    [<Fact>]
    let ``Issue_18128_eliminated_defaultof_leaves_no_reference_to_T_in_caller`` () =
        FSharp """
module Test

type WithCctor() =
    static do failwith "cctor must not run"

let f () =
    let _ = Unchecked.defaultof<WithCctor>
    42
"""
        |> asLibrary
        |> withOptimize
        |> compile
        |> shouldSucceed
        |> verifyIL [
            """.method public static int32  f() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.s   42
    IL_0002:  ret
  }"""
        ]
        |> ignore
