// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.EmittedIL

open Xunit
open FSharp.Test.Compiler

module ArgumentNames =

    [<Fact>]
    let ``Implied argument names are taken from the called method or constructor``() =
        FSharp """
module ArgumentNames

type M (name: string, count: int) =
    [<System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)>]
    static member Open (fileName: string) = ()

let test1 = M
let test2 = M.Open
        """
        |> withLangVersionPreview
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
    let ``Implied argument names are taken from the called DU case constructor or exception``() =
        FSharp """
module ArgumentNames

exception X of code: int * string

type DU =
    | Case1 of code: int * string

let test1 = X
let test2 = Case1
        """
        |> withLangVersionPreview
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
        |> withLangVersionPreview
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
