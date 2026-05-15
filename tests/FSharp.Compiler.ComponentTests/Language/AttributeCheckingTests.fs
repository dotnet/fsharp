// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Language

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module AttributeCheckingTests =

    [<Fact>]
    let ``attributes check inherited AllowMultiple`` () =
        Fsx """
open System

[<AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)>]
type HttpMethodAttribute() = inherit Attribute()
type HttpGetAttribute() = inherit HttpMethodAttribute()

[<HttpGet; HttpGet>] // this shouldn't error like 
[<HttpMethod; HttpMethod>] // this doesn't
type C() =
    member _.M() = ()
        """
         |> ignoreWarnings
         |> compile
         |> shouldSucceed

    [<Fact>]
    let ``AllowMultiple=false allows adding attribute to both property and getter/setter`` () =
        Fsx """
open System

[<AttributeUsage(AttributeTargets.Property ||| AttributeTargets.Method, AllowMultiple = false)>]
type FooAttribute() = inherit Attribute()

type C() =
    [<Foo>]
    member _.Foo
        with [<Foo>] get () = "bar"
         and [<Foo>] set (v: string) = ()
        """
         |> ignoreWarnings
         |> compile
         |> shouldSucceed

    // Regression test for https://github.com/dotnet/fsharp/issues/19020
    [<Fact>]
    let ``[<return: X>] prefix attributes are emitted on class members`` () =
        FSharp """
module TestModule
open System

[<AttributeUsage(AttributeTargets.ReturnValue)>]
type DescAttribute() = inherit Attribute()

module Mod =
    [<return: Desc>]
    let func a = a + 1

type T() =
    [<return: Desc>]
    member _.InstMember a = a + 1

    [<return: Desc>]
    static member StatMember a = a + 1

[<EntryPoint>]
let main _ =
    let asm = System.Reflection.Assembly.GetExecutingAssembly()
    let modType = asm.GetType("TestModule+Mod")
    let modAttrs = modType.GetMethod("func").ReturnParameter.GetCustomAttributes(typeof<DescAttribute>, false).Length
    let inst = typeof<T>.GetMethod("InstMember").ReturnParameter.GetCustomAttributes(typeof<DescAttribute>, false).Length
    let stat = typeof<T>.GetMethod("StatMember").ReturnParameter.GetCustomAttributes(typeof<DescAttribute>, false).Length
    if modAttrs <> 1 then failwithf "module func: expected 1, got %d" modAttrs
    if inst <> 1 then failwithf "InstMember: expected 1 (bug #19020 — attribute dropped on instance members), got %d" inst
    if stat <> 1 then failwithf "StatMember: expected 1 (bug #19020 — attribute dropped on static members), got %d" stat
    0
        """
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed

    // Regression test for https://github.com/dotnet/fsharp/issues/17904
    [<Fact>]
    let ``AllowMultiple=false allows the same attribute on method and on its return value`` () =
        Fsx """
open System

[<AttributeUsage(AttributeTargets.All)>]
type AttrAttribute(m: string) =
    inherit Attribute()

type Test() =
    [<Attr("on method")>]
    [<return: Attr("on return")>]
    member _.Foo() = ()
        """
         |> ignoreWarnings
         |> compile
         |> shouldSucceed

    // Regression test for https://github.com/dotnet/fsharp/issues/17904 — short-form + explicit-same-target
    // mix must still be flagged as a duplicate because both apply to the same metadata target.
    [<Fact>]
    let ``AllowMultiple=false still errors when short-form and method-targeted attributes are mixed`` () =
        Fsx """
open System

[<AttributeUsage(AttributeTargets.All)>]
type AttrAttribute() =
    inherit Attribute()

type Test() =
    [<Attr>]
    [<method: Attr>]
    member _.Foo() = ()
        """
         |> ignoreWarnings
         |> compile
         |> shouldFail
         |> withSingleDiagnostic (Error 429, Line 10, Col 7, Line 10, Col 19, "The attribute type 'AttrAttribute' has 'AllowMultiple=false'. Multiple instances of this attribute cannot be attached to a single language element.")

    // Regression test for https://github.com/dotnet/fsharp/issues/17904
    [<Fact>]
    let ``AllowMultiple=false still errors when the same attribute is applied twice to the same target`` () =
        Fsx """
open System

[<AttributeUsage(AttributeTargets.All)>]
type AttrAttribute() =
    inherit Attribute()

type Test() =
    [<return: Attr>]
    [<return: Attr>]
    member _.Foo() = ()
        """
         |> ignoreWarnings
         |> compile
         |> shouldFail
         |> withSingleDiagnostic (Error 429, Line 10, Col 7, Line 10, Col 19, "The attribute type 'AttrAttribute' has 'AllowMultiple=false'. Multiple instances of this attribute cannot be attached to a single language element.")


    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Regression: typechecker does not fail when attribute is on type variable (https://github.com/dotnet/fsharp/issues/13525)`` () =
        let csharpBaseClass = 
            CSharp """
        using System.Diagnostics.CodeAnalysis;
        
        namespace CSharp
        {
        
            public interface ITreeNode
            {
            }
        
            public static class Extensions
            {
                public static TNode Copy<TNode>([NotNull] this TNode node, ITreeNode context1 = null) where TNode : ITreeNode =>
                    node;
            }
        }""" |> withName "csLib"
        
        let fsharpSource =
            """
    module FooBar
    open type CSharp.Extensions
    
    let replaceWithCopy oldChild newChild =
        let newChildCopy = newChild.Copy()
        ignore newChildCopy
    """
        FSharp fsharpSource
        |> withLangVersionPreview
        |> withReferences [csharpBaseClass]
        |> compile
        |> shouldSucceed

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``C# attribute subclass inherits AllowMultiple true from base`` () =
        let csharpLib =
            CSharp """
            using System;

            [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
            public class BaseAttribute : Attribute { }
            public class ChildAttribute : BaseAttribute { }
            """ |> withName "csAttrLib"

        FSharp """
module Test

[<Child; Child>]
type C() = class end
        """
        |> withReferences [csharpLib]
        |> compile
        |> shouldSucceed

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``C# attribute subclass inherits AllowMultiple false from base`` () =
        let csharpLib =
            CSharp """
            using System;

            [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
            public class BaseAttribute : Attribute { }
            public class ChildAttribute : BaseAttribute { }
            """ |> withName "csAttrLib"

        FSharp """
module Test

[<Child; Child>]
type C() = class end
        """
        |> withReferences [csharpLib]
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Error 429, Line 4, Col 10, Line 4, Col 15, "The attribute type 'ChildAttribute' has 'AllowMultiple=false'. Multiple instances of this attribute cannot be attached to a single language element.")

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``C# attribute multi-level inheritance inherits AllowMultiple true`` () =
        let csharpLib =
            CSharp """
            using System;

            [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
            public class BaseAttribute : Attribute { }
            public class MiddleAttribute : BaseAttribute { }
            public class LeafAttribute : MiddleAttribute { }
            """ |> withName "csAttrLib"

        FSharp """
module Test

[<Leaf; Leaf>]
type C() = class end
        """
        |> withReferences [csharpLib]
        |> compile
        |> shouldSucceed

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``C# attribute subclass with own AttributeUsage overrides base AllowMultiple`` () =
        let csharpLib =
            CSharp """
            using System;

            [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
            public class BaseAttribute : Attribute { }

            [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
            public class ChildAttribute : BaseAttribute { }
            """ |> withName "csAttrLib"

        FSharp """
module Test

[<Child; Child>]
type C() = class end
        """
        |> withReferences [csharpLib]
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Error 429, Line 4, Col 10, Line 4, Col 15, "The attribute type 'ChildAttribute' has 'AllowMultiple=false'. Multiple instances of this attribute cannot be attached to a single language element.")

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``F# attribute subclass of C# base inherits AllowMultiple true`` () =
        let csharpLib =
            CSharp """
            using System;

            [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
            public class BaseAttribute : Attribute { }
            """ |> withName "csAttrLib"

        FSharp """
module Test

type ChildAttribute() = inherit BaseAttribute()

[<Child; Child>]
type C() = class end
        """
        |> withReferences [csharpLib]
        |> compile
        |> shouldSucceed

    // https://github.com/dotnet/fsharp/issues/12796
    // Empty arrays of user-defined class types in custom attributes should compile successfully.
    // Previously this caused FS0192 internal error in encodeCustomAttrElemType.
    [<Fact>]
    let ``Issue 12796 - Empty array of class type in attribute compiles, verifies IL, and runs`` () =
        FSharp
            """
module TestModule

open System
open System.ComponentModel
open System.Reflection

[<AllowNullLiteral>]
type A() = class end

[<DefaultValue([||] : A[])>]
type B() = class end

[<EntryPoint>]
let main _ =
    let attr = typeof<B>.GetCustomAttribute<DefaultValueAttribute>()
    let arr = attr.Value :?> obj[]
    if arr.Length <> 0 then failwith "Expected empty array"
    printfn "class: len=%d" arr.Length
    0
            """
        |> asExe
        |> compile
        |> shouldSucceed
        |> verifyPEFileWithSystemDlls
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> withStdOutContains "class: len=0"

    // https://github.com/dotnet/fsharp/issues/12796
    [<Fact>]
    let ``Issue 12796 - Empty array of record type in attribute compiles, verifies IL, and runs`` () =
        FSharp
            """
module TestModule

open System
open System.ComponentModel
open System.Reflection

type A = { AField: string }

[<DefaultValue([||] : A[])>]
type B() = class end

[<EntryPoint>]
let main _ =
    let attr = typeof<B>.GetCustomAttribute<DefaultValueAttribute>()
    let arr = attr.Value :?> obj[]
    if arr.Length <> 0 then failwith "Expected empty array"
    printfn "record: len=%d" arr.Length
    0
            """
        |> asExe
        |> compile
        |> shouldSucceed
        |> verifyPEFileWithSystemDlls
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> withStdOutContains "record: len=0"

    // https://github.com/dotnet/fsharp/issues/12796
    [<Fact>]
    let ``Issue 12796 - Empty array of enum type in attribute compiles, verifies IL, and runs`` () =
        FSharp
            """
module TestModule

open System
open System.ComponentModel
open System.Reflection

type MyEnum = A = 0 | B = 1

[<DefaultValue([||] : MyEnum[])>]
type T() = class end

[<EntryPoint>]
let main _ =
    let attr = typeof<T>.GetCustomAttribute<DefaultValueAttribute>()
    let arr = attr.Value :?> MyEnum[]
    if arr.Length <> 0 then failwith "Expected empty array"
    printfn "enum: len=%d" arr.Length
    0
            """
        |> asExe
        |> compile
        |> shouldSucceed
        |> verifyPEFileWithSystemDlls
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> withStdOutContains "enum: len=0"

    // https://github.com/dotnet/fsharp/issues/12796
    [<Fact>]
    let ``Issue 12796 - Empty array of primitive type in attribute compiles, verifies IL, and runs`` () =
        FSharp
            """
module TestModule

open System
open System.ComponentModel
open System.Reflection

[<DefaultValue([||] : int[])>]
type T() = class end

[<EntryPoint>]
let main _ =
    let attr = typeof<T>.GetCustomAttribute<DefaultValueAttribute>()
    let arr = attr.Value :?> int[]
    if arr.Length <> 0 then failwith "Expected empty array"
    printfn "int: len=%d" arr.Length
    0
            """
        |> asExe
        |> compile
        |> shouldSucceed
        |> verifyPEFileWithSystemDlls
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> withStdOutContains "int: len=0"

    // https://github.com/dotnet/fsharp/issues/12796
    // Non-empty arrays of user-defined types should produce FS3887 diagnostic instead of FS0192 internal error.
    [<Fact>]
    let ``Issue 12796 - Non-empty array of user-defined type in attribute gives proper error`` () =
        FSharp
            """
module TestModule

open System.ComponentModel

[<AllowNullLiteral>]
type A() = class end

[<DefaultValue([| (null : A) |])>]
type B() = class end
            """
        |> compile
        |> shouldFail
        |> withErrorCode 3887
        |> withDiagnosticMessageMatches "not a valid custom attribute argument type"

    // https://github.com/dotnet/fsharp/issues/12796
    [<Fact>]
    let ``Issue 12796 - Non-empty array of struct type in attribute gives proper error`` () =
        FSharp
            """
module TestModule

open System.ComponentModel

[<Struct>]
type MyStruct = { X: int }

[<DefaultValue([| { X = 1 } |] : MyStruct[])>]
type B() = class end
            """
        |> compile
        |> shouldFail
        |> withErrorCode 267

    // https://github.com/dotnet/fsharp/issues/12796
    [<Fact>]
    let ``Issue 12796 - Empty array of struct type in attribute compiles, verifies IL, and runs`` () =
        FSharp
            """
module TestModule

open System
open System.ComponentModel
open System.Reflection

[<Struct>]
type MyStruct = { X: int; Y: int }

[<DefaultValue([||] : MyStruct[])>]
type T() = class end

[<EntryPoint>]
let main _ =
    let attr = typeof<T>.GetCustomAttribute<DefaultValueAttribute>()
    let arr = attr.Value :?> obj[]
    if arr.Length <> 0 then failwith "Expected empty array"
    printfn "struct: len=%d" arr.Length
    0
            """
        |> asExe
        |> compile
        |> shouldSucceed
        |> verifyPEFileWithSystemDlls
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> withStdOutContains "struct: len=0"

    // https://github.com/dotnet/fsharp/issues/12796
    [<Fact>]
    let ``Issue 12796 - Empty array of discriminated union type in attribute compiles, verifies IL, and runs`` () =
        FSharp
            """
module TestModule

open System
open System.ComponentModel
open System.Reflection

type MyDU = Case1 | Case2 of int

[<DefaultValue([||] : MyDU[])>]
type T() = class end

[<EntryPoint>]
let main _ =
    let attr = typeof<T>.GetCustomAttribute<DefaultValueAttribute>()
    let arr = attr.Value :?> obj[]
    if arr.Length <> 0 then failwith "Expected empty array"
    printfn "du: len=%d" arr.Length
    0
            """
        |> asExe
        |> compile
        |> shouldSucceed
        |> verifyPEFileWithSystemDlls
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> withStdOutContains "du: len=0"

    // https://github.com/dotnet/fsharp/issues/12796
    [<Fact>]
    let ``Issue 12796 - Empty array of interface type in attribute compiles, verifies IL, and runs`` () =
        FSharp
            """
module TestModule

open System
open System.ComponentModel
open System.Reflection

type IFoo =
    abstract member Bar : unit -> unit

[<DefaultValue([||] : IFoo[])>]
type T() = class end

[<EntryPoint>]
let main _ =
    let attr = typeof<T>.GetCustomAttribute<DefaultValueAttribute>()
    let arr = attr.Value :?> obj[]
    if arr.Length <> 0 then failwith "Expected empty array"
    printfn "interface: len=%d" arr.Length
    0
            """
        |> asExe
        |> compile
        |> shouldSucceed
        |> verifyPEFileWithSystemDlls
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> withStdOutContains "interface: len=0"

    // https://github.com/dotnet/fsharp/issues/12796
    // Verify encodable array types still work correctly through the new code path.
    [<Fact>]
    let ``Issue 12796 - Empty array of obj type in attribute compiles, verifies IL, and runs`` () =
        FSharp
            """
module TestModule

open System
open System.ComponentModel
open System.Reflection

[<DefaultValue([||] : obj[])>]
type T() = class end

[<EntryPoint>]
let main _ =
    let attr = typeof<T>.GetCustomAttribute<DefaultValueAttribute>()
    let arr = attr.Value :?> obj[]
    if arr.Length <> 0 then failwith "Expected empty array"
    printfn "obj: len=%d" arr.Length
    0
            """
        |> asExe
        |> compile
        |> shouldSucceed
        |> verifyPEFileWithSystemDlls
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> withStdOutContains "obj: len=0"

    // https://github.com/dotnet/fsharp/issues/12796
    [<Fact>]
    let ``Issue 12796 - Empty array of string type in attribute compiles, verifies IL, and runs`` () =
        FSharp
            """
module TestModule

open System
open System.ComponentModel
open System.Reflection

[<DefaultValue([||] : string[])>]
type T() = class end

[<EntryPoint>]
let main _ =
    let attr = typeof<T>.GetCustomAttribute<DefaultValueAttribute>()
    let arr = attr.Value :?> string[]
    if arr.Length <> 0 then failwith "Expected empty array"
    printfn "string: len=%d" arr.Length
    0
            """
        |> asExe
        |> compile
        |> shouldSucceed
        |> verifyPEFileWithSystemDlls
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> withStdOutContains "string: len=0"

    // https://github.com/dotnet/fsharp/issues/12796
    [<Fact>]
    let ``Issue 12796 - Non-empty array of string type in attribute compiles and runs`` () =
        FSharp
            """
module TestModule

open System
open System.ComponentModel
open System.Reflection

[<DefaultValue([| "hello"; "world" |])>]
type T() = class end

[<EntryPoint>]
let main _ =
    let attr = typeof<T>.GetCustomAttribute<DefaultValueAttribute>()
    let arr = attr.Value :?> string[]
    if arr.Length <> 2 then failwith "Expected 2 elements"
    if arr.[0] <> "hello" then failwith "Expected 'hello'"
    printfn "string-nonempty: len=%d" arr.Length
    0
            """
        |> asExe
        |> compile
        |> shouldSucceed
        |> verifyPEFileWithSystemDlls
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> withStdOutContains "string-nonempty: len=2"
