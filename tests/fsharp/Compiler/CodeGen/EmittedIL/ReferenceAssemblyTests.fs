// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests.CodeGen.EmittedIL

open System.IO
open System.Reflection
open FSharp.Test.Utilities
open FSharp.Test.Utilities.Compiler
open NUnit.Framework

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
    
    } 
    
    .class private abstract auto ansi sealed '<StartupCode$assembly>'.$ReferenceAssembly
           extends [runtime]System.Object
    {
    }"""
        ]
        |> ignore

    [<Test>]
    let ``Simple reference assembly should have expected IL with dummy typed impl file``() =
        let src =
            """
module ReferenceAssembly

open System

let test() =
    Console.WriteLine("Hello World!")
            """

        FSharp src
        |> withOptions ["--test:RefOnlyTestSigOfImpl"]
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
    
    } 
    
    .class private abstract auto ansi sealed '<StartupCode$assembly>'.$ReferenceAssembly
           extends [runtime]System.Object
    {
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
    
    } 
    
    .class private abstract auto ansi sealed '<StartupCode$assembly>'.$ReferenceAssembly
           extends [runtime]System.Object
    {
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
    
    } 
    
    .class private abstract auto ansi sealed '<StartupCode$assembly>'.$ReferenceAssembly
           extends [runtime]System.Object
    {
    }"""
        ]
        |> ignore

    [<Test>]
    let ``Simple reference assembly should have expected IL with anonymous record with dummy typed impl file``() =
        let src =
            """
module ReferenceAssembly

open System

let test(_x: {| a: int32 |}) =
    Console.WriteLine("Hello World!")
            """

        FSharp src
        |> withOptions ["--test:RefOnlyTestSigOfImpl"]
        |> compile
        |> shouldSucceed
        |> verifyIL [
            referenceAssemblyAttributeExpectedIL
            """.maxstack  8
        IL_0000:  ldnull
        IL_0001:  throw
      } 
    
    } 
    
    .class private abstract auto ansi sealed '<StartupCode$assembly>'.$ReferenceAssembly
           extends [runtime]System.Object
    {
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
    
    } 
    
    .class private abstract auto ansi sealed '<StartupCode$assembly>'.$ReferenceAssembly
            extends [runtime]System.Object
    {
    }"""
        ]
        |> ignore

    [<Test>]
    let ``Simple reference assembly with nested module should have expected IL with dummy typed impl``() =
        let src =
            """
module ReferenceAssembly

open System

module Nested =

    let test() =
        Console.WriteLine("Hello World!")
            """

        FSharp src
        |> withOptions ["--test:RefOnlyTestSigOfImpl"]
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
    
    } 
    
    .class private abstract auto ansi sealed '<StartupCode$assembly>'.$ReferenceAssembly
            extends [runtime]System.Object
    {
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

    let test(_x: Test) =
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
    
    } 
    
    .class private abstract auto ansi sealed '<StartupCode$assembly>'.$ReferenceAssembly
            extends [runtime]System.Object
    {
    }"""
        ]
        |> ignore
