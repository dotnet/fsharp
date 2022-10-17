﻿// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests.CodeGen.EmittedIL

open FSharp.Test.Compiler
open NUnit.Framework
open FSharp.Compiler.IO

[<TestFixture>]
module ReferenceAssemblyTests =

    let referenceAssemblyAttributeExpectedIL =
        """.custom instance void [runtime]System.Runtime.CompilerServices.ReferenceAssemblyAttribute::.ctor() = ( 01 00 00 00 )"""

    [<Test>]
    let ``Simple reference assembly should have expected IL``() =
        let src =
            """
module ReferenceAssembly

open System

let test() =
    Console.WriteLine("Hello World!")
            """

        FSharp src
        |> withOptions ["--refonly"]
        |> compile
        |> shouldSucceed
        |> verifyIL [
            referenceAssemblyAttributeExpectedIL
            """.class public abstract auto ansi sealed ReferenceAssembly
           extends [runtime]System.Object
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 )
      .method public static void  test() cil managed
      {

        .maxstack  8
        IL_0000:  ldnull
        IL_0001:  throw
      }

    }"""
        ]
        |> ignore

    [<Test>]
    let ``Simple reference assembly should have expected IL without a private function``() =
        let src =
            """
module ReferenceAssembly

open System

let private privTest() =
    Console.WriteLine("Private Hello World!")

let test() =
    privTest()
    Console.WriteLine("Hello World!")
            """

        FSharp src
        |> withOptions ["--refonly"]
        |> compile
        |> shouldSucceed
        |> verifyIL [
            referenceAssemblyAttributeExpectedIL
            """.class public abstract auto ansi sealed ReferenceAssembly
           extends [runtime]System.Object
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 )
      .method public static void  test() cil managed
      {

        .maxstack  8
        IL_0000:  ldnull
        IL_0001:  throw
      }

    }"""
        ]
        |> ignore

    [<Test>]
    let ``Simple reference assembly should have expected IL with anonymous record``() =
        let src =
            """
module ReferenceAssembly

open System

let test(_x: {| a: int32 |}) =
    Console.WriteLine("Hello World!")
            """

        FSharp src
        |> withOptions ["--refonly"]
        |> compile
        |> shouldSucceed
        |> verifyIL [
            referenceAssemblyAttributeExpectedIL
            """.maxstack  8
        IL_0000:  ldnull
        IL_0001:  throw
      }

    }"""
        ]
        |> ignore

    [<Test>]
    let ``Simple reference assembly with nested module should have expected IL``() =
        let src =
            """
module ReferenceAssembly

open System

module Nested =

    let test() =
        Console.WriteLine("Hello World!")
            """

        FSharp src
        |> withOptions ["--refonly"]
        |> compile
        |> shouldSucceed
        |> verifyIL [
            referenceAssemblyAttributeExpectedIL
            """.class public abstract auto ansi sealed ReferenceAssembly
            extends [runtime]System.Object
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 )
      .class abstract auto ansi sealed nested public Nested
              extends [runtime]System.Object
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 )
        .method public static void  test() cil managed
        {

          .maxstack  8
          IL_0000:  ldnull
          IL_0001:  throw
        }

      }

    }"""
        ]
        |> ignore

    [<Test>]
    let ``Simple reference assembly with nested module with type should have expected IL``() =
        let src =
            """
module ReferenceAssembly

open System

module Nested =

    type Test = { x: int }

    let private foo () = ()

    let test(_x: Test) =
        foo ()
        Console.WriteLine("Hello World!")
            """

        FSharp src
        |> withOptions ["--refonly"]
        |> compile
        |> shouldSucceed
        |> verifyIL [
            referenceAssemblyAttributeExpectedIL
            """.class abstract auto ansi sealed nested public Nested
            extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto ansi serializable sealed nested public Test
              extends [runtime]System.Object
              implements class [runtime]System.IEquatable`1<class ReferenceAssembly/Nested/Test>,
                         [runtime]System.Collections.IStructuralEquatable,
                         class [runtime]System.IComparable`1<class ReferenceAssembly/Nested/Test>,
                         [runtime]System.IComparable,
                         [runtime]System.Collections.IStructuralComparable
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 02 00 00 00 00 00 ) 
      .method public hidebysig specialname 
                 instance int32  get_x() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 )

        .maxstack  8
        IL_0000:  ldnull
        IL_0001:  throw
      } 

      .method public specialname rtspecialname 
                 instance void  .ctor(int32 x) cil managed
      {
        
        .maxstack  8
        IL_0000:  ldnull
        IL_0001:  throw
      } 

      .method public strict virtual instance string 
                 ToString() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldnull
        IL_0001:  throw
      } 

      .method public hidebysig virtual final 
                 instance int32  CompareTo(class ReferenceAssembly/Nested/Test obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 )
        
        .maxstack  8
        IL_0000:  ldnull
        IL_0001:  throw
      } 

      .method public hidebysig virtual final 
                 instance int32  CompareTo(object obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 )
        
        .maxstack  8
        IL_0000:  ldnull
        IL_0001:  throw
      } 

      .method public hidebysig virtual final 
                 instance int32  CompareTo(object obj,
                                           class [runtime]System.Collections.IComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 )
        
        .maxstack  8
        IL_0000:  ldnull
        IL_0001:  throw
      } 

      .method public hidebysig virtual final 
                 instance int32  GetHashCode(class [runtime]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 )
        
        .maxstack  8
        IL_0000:  ldnull
        IL_0001:  throw
      } 

      .method public hidebysig virtual final 
                 instance int32  GetHashCode() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 )
        
        .maxstack  8
        IL_0000:  ldnull
        IL_0001:  throw
      } 

      .method public hidebysig virtual final 
                 instance bool  Equals(object obj,
                                       class [runtime]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 )
        
        .maxstack  8
        IL_0000:  ldnull
        IL_0001:  throw
      } 

      .method public hidebysig virtual final 
                 instance bool  Equals(class ReferenceAssembly/Nested/Test obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 )
        
        .maxstack  8
        IL_0000:  ldnull
        IL_0001:  throw
      } 

      .method public hidebysig virtual final 
                 instance bool  Equals(object obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 )
        
        .maxstack  8
        IL_0000:  ldnull
        IL_0001:  throw
      } 

      .property instance int32 x()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                       int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
        .get instance int32 ReferenceAssembly/Nested/Test::get_x()
      } 
    } 

    .method public static void  test(class ReferenceAssembly/Nested/Test _x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  throw
    } 

  } """
        ]
        |> ignore

    [<Test>]
    let ``--refout should produce both normal and reference assemblies``() =
        // TODO: We probably want a built-in test framework functionality which will be taking care of comparing/verifying refout.
        let refoutDllPath = FileSystem.GetTempPathShim() + "Test.ref.dll"
        let src =
            """
module ReferenceAssembly

open System

let test() =
    Console.WriteLine("Hello World!")
            """

        // This will produce normal assembly as well as ref in {refoutPath}
        let result =
            FSharp src
            |> withOptions [$"--refout:{refoutDllPath}"]
            |> compile

        // Should build successfully.
        result |> shouldSucceed
        // Verify that normal assembly has been produced.
        |> verifyIL [""".class public abstract auto ansi sealed ReferenceAssembly
        extends [runtime]System.Object
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 )
      .method public static void  test() cil managed
      {

        .maxstack  8
        IL_0000:  ldstr      "Hello World!"
        IL_0005:  call       void [runtime]System.Console::WriteLine(string)
        IL_000a:  ret
      }

    }"""
        ]
        |> ignore

        // Verify that ref assembly in custom path was produced.
        if not (FileSystem.FileExistsShim refoutDllPath) then
            failwith $"Can't find reference assembly {refoutDllPath}"

        refoutDllPath
        |> verifyILBinary [
            referenceAssemblyAttributeExpectedIL
            """.class public abstract auto ansi sealed ReferenceAssembly
           extends [runtime]System.Object
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 )
      .method public static void  test() cil managed
      {

        .maxstack  8
        IL_0000:  ldnull
        IL_0001:  throw
      }

    }"""
        ]

    [<Test>]
    let ``Can't use both --refonly and --staticlink``() =
        let src =
            """
module ReferenceAssembly

open System

let test() =
    Console.WriteLine("Hello World!")
            """

        FSharp src
        |> withOptions ["--staticlink:foo"; "--refonly"]
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Error 2030, Line 0, Col 1, Line 0, Col 1, "Invalid use of emitting a reference assembly, do not use '--staticlink', or '--refonly' and '--refout' together.")
        |> ignore

    [<Test>]
    let ``Can't use both --refoout and --staticlink``() =
        let src =
            """
module ReferenceAssembly

open System

let test() =
    Console.WriteLine("Hello World!")
            """

        FSharp src
        |> withOptions ["--staticlink:foo"; "--refout:foo"]
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Error 2030, Line 0, Col 1, Line 0, Col 1, "Invalid use of emitting a reference assembly, do not use '--staticlink', or '--refonly' and '--refout' together.")
        |> ignore

    [<Test>]
    let ``Internal DU type doesn't generate any properties/methods without IVT`` () =
        FSharp """
module ReferenceAssembly
[<NoComparison;NoEquality>]
type internal RingState<'item> = | Writable of 'item
        """
        |> withOptions ["--refonly"]
        |> compile
        |> shouldSucceed
        |> verifyIL [
            referenceAssemblyAttributeExpectedIL
            """
.class auto autochar serializable sealed nested assembly beforefieldinit RingState`1<item>
extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.NoComparisonAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.NoEqualityAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 7B 5F 5F 44 65 62 75 67 44 69 73 70 6C   
                                                                                          61 79 28 29 2C 6E 71 7D 00 00 )                   
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 21 00 00 00 00 00 )                         
    .method public strict virtual instance string 
   ToString() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  throw
    } 

  } """]

    [<Test>]
    let ``Types with internal-only properties and methods don't generate anything without IVT`` () =
        FSharp """
module ReferenceAssembly
[<NoComparison;NoEquality>]
type MyType() =
    let mutable myInternalValue = 1
    member internal this.MyReadOnlyProperty = myInternalValue
    // A write-only property.
    member internal this.MyWriteOnlyProperty with set (value) = myInternalValue <- value
    // A read-write property.
    member internal this.MyReadWriteProperty
        with get () = myInternalValue
        and set (value) = myInternalValue <- value"""
        |> withOptions ["--refonly"]
        |> compile
        |> shouldSucceed
        |> verifyIL [
            referenceAssemblyAttributeExpectedIL
            """
.class auto ansi serializable nested public MyType
extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.NoComparisonAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.NoEqualityAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public specialname rtspecialname 
   instance void  .ctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  throw
    } 

  }"""]

    [<Test>]
    let ``Properties, getters, setters are emitted for internal properties`` () =
        FSharp """
module ReferenceAssembly

[<System.AttributeUsage(System.AttributeTargets.All)>]
type MyAttribute() =
    inherit System.Attribute()
    member val internal Prop1 : int = 0 with get, set

[<System.AttributeUsage(System.AttributeTargets.All)>]
type MySecondaryAttribute() =
    inherit MyAttribute()
    member val internal Prop1 : int = 0 with get, set
        """
        |> withOptions ["--refonly"]
        |> compile
        |> shouldSucceed
        |> verifyIL [
            referenceAssemblyAttributeExpectedIL
            """.class auto ansi serializable nested public MyAttribute
            extends [runtime]System.Attribute
  {
    .custom instance void [runtime]System.AttributeUsageAttribute::.ctor(valuetype [runtime]System.AttributeTargets) = ( 01 00 FF 7F 00 00 00 00 ) 
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .field assembly int32 Prop1@
    .method public specialname rtspecialname 
               instance void  .ctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  throw
    } 

    .method assembly hidebysig specialname 
               instance int32  get_Prop1() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 )

      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  throw
    } 

    .method assembly hidebysig specialname 
               instance void  set_Prop1(int32 v) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 )

      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  throw
    } 

  } 

  .class auto ansi serializable nested public MySecondaryAttribute
            extends ReferenceAssembly/MyAttribute
  {
    .custom instance void [runtime]System.AttributeUsageAttribute::.ctor(valuetype [runtime]System.AttributeTargets) = ( 01 00 FF 7F 00 00 00 00 ) 
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .field assembly int32 Prop1@
    .method public specialname rtspecialname 
               instance void  .ctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  throw
    } 

    .method assembly hidebysig specialname 
               instance int32  get_Prop1() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 )

      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  throw
    } 

    .method assembly hidebysig specialname 
               instance void  set_Prop1(int32 v) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 )

      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  throw
    } 

    .property instance int32 Prop1()
    {
      .set instance void ReferenceAssembly/MyAttribute::set_Prop1(int32)
      .get instance int32 ReferenceAssembly/MyAttribute::get_Prop1()
    } 
    .property instance int32 Prop1()
    {
      .set instance void ReferenceAssembly/MySecondaryAttribute::set_Prop1(int32)
      .get instance int32 ReferenceAssembly/MySecondaryAttribute::get_Prop1()
    } 
  } """
        ]
    
    [<Test>]
    let ``Internal and private fields are emitted for structs`` () =
        FSharp """
module ReferenceAssembly

[<NoEquality; NoComparison>]
type AStruct =
    struct
        [<DefaultValue>] val mutable internal myInt : int
        [<DefaultValue>] val mutable private myInt2 : int
    end
    """
        |> withOptions ["--refonly"]
        |> compile
        |> shouldSucceed
        |> verifyIL [
            referenceAssemblyAttributeExpectedIL
            """.class sequential ansi serializable sealed nested public AStruct
            extends [runtime]System.ValueType
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.NoEqualityAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.NoComparisonAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .field assembly int32 myInt
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.DefaultValueAttribute::.ctor() = ( 01 00 00 00 ) 
    .field assembly int32 myInt2
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.DefaultValueAttribute::.ctor() = ( 01 00 00 00 ) 
  }"""
        ]
    [<Test>]
    let ``Only public properties are emitted on non-IVT assemblies`` () =
        FSharp """
module ReferenceAssembly

type NotAnAttribute() =
    member val internal Prop1 : int = 0 with get, set

type MType() =
    member val public PubProp1 : int = 0 with get, set
    member val internal IntProp1 : int = 0 with get, set
    member val private PrivProp1 : int = 0 with get, set
    """
        |> withOptions ["--refonly"]
        |> compile
        |> shouldSucceed
        |> verifyIL [
            referenceAssemblyAttributeExpectedIL
            """.class auto ansi serializable nested public NotAnAttribute
            extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public specialname rtspecialname 
               instance void  .ctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  throw
    } 

  } 

  .class auto ansi serializable nested public MType
            extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public specialname rtspecialname 
               instance void  .ctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  throw
    } 

    .method public hidebysig specialname 
               instance int32  get_PubProp1() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 )

      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  throw
    } 

    .method public hidebysig specialname 
               instance void  set_PubProp1(int32 v) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 )

      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  throw
    } 

    .property instance int32 PubProp1()
    {
      .set instance void ReferenceAssembly/MType::set_PubProp1(int32)
      .get instance int32 ReferenceAssembly/MType::get_PubProp1()
    } 
  } """
        ]
    [<Test>]
    let ``Only public events are emitted for non-IVT assembly`` () =
        FSharp """
module ReferenceAssembly

type MType() =
    let event1 = new Event<_>()
    
    [<CLIEvent>]
    member private _.Event1 = event1.Publish
    """
        |> withOptions ["--refonly"]
        |> compile
        |> shouldSucceed
        |> verifyIL [
            referenceAssemblyAttributeExpectedIL
            """.class auto ansi serializable nested public MType
            extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public specialname rtspecialname 
               instance void  .ctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  throw
    } 

  } """
        ]

    [<Test>]
    let ``Internal constructor is emitted for attribute`` () =
        FSharp """
module ReferenceAssembly

open System
[<AttributeUsage(AttributeTargets.Method ||| AttributeTargets.Property, AllowMultiple=false)>]
[<Sealed>]
type CustomAttribute(smth: bool) =
    inherit Attribute()
    internal new () = CustomAttribute(false)
    member _.Something = smth

type Person(name : string, age : int) =
    [<Custom(true)>]
    member val Name = name with get, set
    [<Custom>]
    member val Age = age with get, set
    """
        |> withOptions ["--refonly"]
        |> compile
        |> shouldSucceed
        |> verifyIL [
            referenceAssemblyAttributeExpectedIL
            """.class auto ansi serializable sealed nested public CustomAttribute
            extends [runtime]System.Attribute
  {
    .custom instance void [runtime]System.AttributeUsageAttribute::.ctor(valuetype [runtime]System.AttributeTargets) = ( 01 00 C0 00 00 00 01 00 54 02 0D 41 6C 6C 6F 77
                                                                                                                                          4D 75 6C 74 69 70 6C 65 00 )
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.SealedAttribute::.ctor() = ( 01 00 00 00 )
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 )
    .field assembly bool smth
    .method public specialname rtspecialname
               instance void  .ctor(bool smth) cil managed
    {

      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  throw
    }

    .method assembly specialname rtspecialname
               instance void  .ctor() cil managed
    {

      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  throw
    }

    .method public hidebysig specialname
               instance bool  get_Something() cil managed
    {

      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  throw
    }

  }

  .class auto ansi serializable nested public Person
            extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 )
    .method public specialname rtspecialname
               instance void  .ctor(string name,
                                    int32 age) cil managed
    {

      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  throw
    }

    .method public hidebysig specialname
               instance string  get_Name() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 )

      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  throw
    }

    .method public hidebysig specialname
               instance void  set_Name(string v) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 )

      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  throw
    }

    .method public hidebysig specialname
               instance int32  get_Age() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 )

      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  throw
    }

    .method public hidebysig specialname
               instance void  set_Age(int32 v) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 )

      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  throw
    }

    .property instance bool Something()
    {
      .get instance bool ReferenceAssembly/CustomAttribute::get_Something()
    }
    .property instance string Name()
    {
      .custom instance void ReferenceAssembly/CustomAttribute::.ctor(bool) = ( 01 00 01 00 00 )
      .set instance void ReferenceAssembly/Person::set_Name(string)
      .get instance string ReferenceAssembly/Person::get_Name()
    }
    .property instance int32 Age()
    {
      .custom instance void ReferenceAssembly/CustomAttribute::.ctor() = ( 01 00 00 00 )
      .set instance void ReferenceAssembly/Person::set_Age(int32)
      .get instance int32 ReferenceAssembly/Person::get_Age()
    }
  }"""
        ]

    [<Test>]
    let ``Internal constructor is emitted for attribute (with fsi)`` () =
        let fsSig =
            Fsi """
namespace Microsoft.FSharp.Core
    open System
    [<AttributeUsage (AttributeTargets.Method ||| AttributeTargets.Property,AllowMultiple=false)>]  
    [<Sealed>]
    type NoDynamicInvocationAttribute =
        inherit Attribute
        new: unit -> NoDynamicInvocationAttribute
        internal new: isLegacy: bool -> NoDynamicInvocationAttribute

    module Operators = 
        [<CompiledName("GetId")>]
        val inline id: value: 'T -> 'T
            """

        let fsSource =
            FsSource """
namespace Microsoft.FSharp.Core
    open System
    [<AttributeUsage(AttributeTargets.Method ||| AttributeTargets.Property, AllowMultiple=false)>]
    [<Sealed>]
    type NoDynamicInvocationAttribute(isLegacy: bool) =
        inherit Attribute()
        new () = NoDynamicInvocationAttribute(false)
        member _.IsLegacy = isLegacy

    module Operators =
        [<NoDynamicInvocation(isLegacy=true)>]
        [<CompiledName("GetId")>]
        let inline id (value: 'T) = value
            """
        fsSig
        |> withOptions ["--refonly"]
        |> withAdditionalSourceFile fsSource
        |> compile
        |> shouldSucceed
        |> verifyIL [
            referenceAssemblyAttributeExpectedIL
            """
.class public auto ansi serializable sealed Microsoft.FSharp.Core.NoDynamicInvocationAttribute
extends [runtime]System.Attribute
{
  .custom instance void [runtime]System.AttributeUsageAttribute::.ctor(valuetype [runtime]System.AttributeTargets) = ( 01 00 C0 00 00 00 01 00 54 02 0D 41 6C 6C 6F 77   
                                                                                                                              4D 75 6C 74 69 70 6C 65 00 )                      
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.SealedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
  .field assembly bool isLegacy
  .method assembly specialname rtspecialname 
   instance void  .ctor(bool isLegacy) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  throw
  } 

  .method public specialname rtspecialname 
   instance void  .ctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  throw
  } 

  .method assembly hidebysig specialname 
   instance bool  get_IsLegacy() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  throw
  } 

  .property instance bool IsLegacy()
  {
    .get instance bool Microsoft.FSharp.Core.NoDynamicInvocationAttribute::get_IsLegacy()
  } 
} 

.class public abstract auto ansi sealed Microsoft.FSharp.Core.Operators
extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static !!T  GetId<T>(!!T 'value') cil managed
  {
    .custom instance void Microsoft.FSharp.Core.NoDynamicInvocationAttribute::.ctor(bool) = ( 01 00 01 00 00 ) 
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationSourceNameAttribute::.ctor(string) = ( 01 00 02 69 64 00 00 )                            
    
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  throw
  } 

} """ ]

    [<Test>]
    let ``Build .exe with --refonly ensure it produces a main in the ref assembly`` () =
        FSharp """module ReferenceAssembly
open System

Console.WriteLine("Hello World!")"""
        |> withOptions ["--refonly"]
        |> withName "HasMainCheck"
        |> asExe
        |> compile
        |> shouldSucceed
        |> verifyIL [
            referenceAssemblyAttributeExpectedIL
            """.class private abstract auto ansi sealed '<StartupCode$HasMainCheck>'.$ReferenceAssembly
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  throw
  } // end of method $ReferenceAssembly::main@

} // end of class '<StartupCode$HasMainCheck>'.$ReferenceAssembly
"""
        ]
        |> ignore

    // TODO: Add tests for internal functions, types, interfaces, abstract types (with and without IVTs), (private, internal, public) fields, properties (+ different visibility for getters and setters), events.
