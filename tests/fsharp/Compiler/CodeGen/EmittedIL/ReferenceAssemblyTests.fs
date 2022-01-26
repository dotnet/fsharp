// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

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
            """.class public abstract auto ansi sealed ReferenceAssembly
            extends [runtime]System.Object
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 )
      .class abstract auto ansi sealed nested public Nested
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
          .field assembly int32 x@
          .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 )
          .method public hidebysig specialname
                   instance int32  get_x() cil managed
          {

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

             .maxstack  8
             IL_0000:  ldnull
             IL_0001:  throw
          }

          .method public hidebysig virtual final
                   instance int32  CompareTo(object obj) cil managed
          {
             .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )

             .maxstack  8
             IL_0000:  ldnull
             IL_0001:  throw
          }

          .method public hidebysig virtual final
                   instance int32  CompareTo(object obj,
                                             class [runtime]System.Collections.IComparer comp) cil managed
          {
             .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )

             .maxstack  8
             IL_0000:  ldnull
             IL_0001:  throw
          }

          .method public hidebysig virtual final
                   instance int32  GetHashCode(class [runtime]System.Collections.IEqualityComparer comp) cil managed
          {
             .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )

             .maxstack  8
             IL_0000:  ldnull
             IL_0001:  throw
          }

          .method public hidebysig virtual final
                   instance int32  GetHashCode() cil managed
          {
             .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )

             .maxstack  8
             IL_0000:  ldnull
             IL_0001:  throw
          }

          .method public hidebysig virtual final
                   instance bool  Equals(object obj,
                                         class [runtime]System.Collections.IEqualityComparer comp) cil managed
          {
             .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )

             .maxstack  8
             IL_0000:  ldnull
             IL_0001:  throw
          }

          .method public hidebysig virtual final
                   instance bool  Equals(class ReferenceAssembly/Nested/Test obj) cil managed
          {
             .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )

             .maxstack  8
             IL_0000:  ldnull
             IL_0001:  throw
          }

          .method public hidebysig virtual final
                   instance bool  Equals(object obj) cil managed
          {
             .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )

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

      }

    }"""
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
        |> withSingleDiagnostic (Error 2030, Line 0, Col 1, Line 0, Col 1, "Invalid use of emitting a reference assembly. Check the compiler options to not specify static linking, or using '--refonly' and '--refout' together.")
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
        |> withSingleDiagnostic (Error 2030, Line 0, Col 1, Line 0, Col 1, "Invalid use of emitting a reference assembly. Check the compiler options to not specify static linking, or using '--refonly' and '--refout' together.")
        |> ignore

    [<Test>]
    let ``Internal DU type doesn't generate anything without IVT`` () =
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
.class public abstract auto ansi sealed ReferenceAssembly
extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
}"""]
    // TODO: Add tests for Internal types (+IVT), (private, internal, public) fields, properties, events + different combinations.