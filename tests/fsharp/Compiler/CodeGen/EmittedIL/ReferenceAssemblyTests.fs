// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests.CodeGen.EmittedIL

open System.IO
open System.Reflection
open FSharp.Test.Utilities
open FSharp.Test.Utilities.Compiler
open NUnit.Framework

[<TestFixture>]
module ReferenceAssemblyTests =

    [<Test>]
    let ``Simple reference assembly``() =
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
            """.class public abstract auto ansi sealed ReferenceAssembly
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
    
    } 
    
    .class private abstract auto ansi sealed '<StartupCode$assembly>'.$ReferenceAssembly
           extends [runtime]System.Object
    {
    }"""
        ]
        |> ignore


