﻿// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace EmittedIL

open Xunit
open FSharp.Test.Compiler

module ArgumentNames =

    [<Fact>]
    let ``Implied argument names are taken from method or constructor``() =
        FSharp """
module ArgumentNames

type M (name: string, count: int) =
    [<System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)>]
    static member Open (fileName: string) = ()

let test1 = M
let test2 = M.Open
        """
        |> withLangVersion80
        |> compile
        |> shouldSucceed
        |> verifyIL ["""
.method public static class ArgumentNames/M 
        test1(string name,
              int32 count) cil managed
{

  .maxstack  8
  IL_0000:  ldarg.0
  IL_0001:  ldarg.1
  IL_0002:  newobj     instance void ArgumentNames/M::.ctor(string,
                                                            int32)
  IL_0007:  ret
} 

.method public static void  test2(string fileName) cil managed
{

  .maxstack  8
  IL_0000:  ldarg.0
  IL_0001:  call       void ArgumentNames/M::Open(string)
  IL_0006:  ret
} """ ]

    [<Fact>]
    let ``Implied argument names are taken from curried method``() =
        FSharp """
module ArgumentNames

type M =
    [<System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)>]
    static member Write (fileName: string, offset: int) (data: byte[]) = ()

let test1 = M.Write
        """
        |> withLangVersion80
        |> compile
        |> shouldSucceed
        |> verifyIL ["""
.method public static void  test1(string fileName,
                                  int32 offset,
                                  uint8[] data) cil managed
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 02 00 00 00 01 00 00 00 00 00 ) 

  .maxstack  8
  IL_0000:  ldarg.0
  IL_0001:  ldarg.1
  IL_0002:  ldarg.2
  IL_0003:  call       void ArgumentNames/M::Write(string,
                                                   int32,
                                                   uint8[])
  IL_0008:  ret
} """ ]

    [<Fact>]
    let ``Implied argument names are taken from C#-style extension method``() =
        FSharp """
module ArgumentNames

open System.Runtime.CompilerServices

[<Extension>]
type Ext =
    [<System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)>]
    [<Extension>]
    static member Print(x: int, yy: string) = printfn "%d%s" x yy

let test1 = (3).Print
        """
        |> withLangVersion80
        |> compile
        |> shouldSucceed
        |> verifyIL ["""
        Invoke(string yy) cil managed
{
  
  .maxstack  8
  IL_0000:  ldc.i4.3
  IL_0001:  ldarg.1
  IL_0002:  call       void ArgumentNames/Ext::Print(int32,
                                                     string)
  IL_0007:  ldnull
  IL_0008:  ret
} """ ]

    [<Fact>]
    let ``Implied argument names are taken from DU case constructor or exception``() =
        FSharp """
module ArgumentNames

exception X of code: int * string

type DU =
    | Case1 of code: int * string

let test1 = X
let test2 = Case1
        """
        |> withLangVersion80
        |> compile
        |> shouldSucceed
        |> verifyIL ["""
.method public static class [runtime]System.Exception 
        test1(int32 code,
              string Data1) cil managed
{

  .maxstack  8
  IL_0000:  ldarg.0
  IL_0001:  ldarg.1
  IL_0002:  newobj     instance void ArgumentNames/X::.ctor(int32,
                                                            string)
  IL_0007:  ret
}

.method public static class ArgumentNames/DU 
        test2(int32 code,
              string Item2) cil managed
{

  .maxstack  8
  IL_0000:  ldarg.0
  IL_0001:  ldarg.1
  IL_0002:  call       class ArgumentNames/DU ArgumentNames/DU::NewCase1(int32,
                                                                         string)
  IL_0007:  ret
} """ ]

    [<Fact>]
    let ``Implied argument names are taken from function and used in delegate Invoke``() =
        FSharp """
module ArgumentNames

[<System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)>]
let add num1 num2 = printfn "%d" (num1 + num2)

let test1 = System.Action<_, _>(add)
        """
        |> withLangVersion80
        |> compile
        |> shouldSucceed
        |> verifyIL ["""
.method assembly static void  Invoke(int32 num1,
                                     int32 num2) cil managed
{
  
  .maxstack  8
  IL_0000:  ldarg.0
  IL_0001:  ldarg.1
  IL_0002:  tail.
  IL_0004:  call       void ArgumentNames::'add'(int32,
                                                 int32)
  IL_0009:  ret
} """ ]
