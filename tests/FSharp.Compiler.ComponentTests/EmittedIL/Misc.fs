// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.EmittedIL

open Xunit
open FSharp.Test.Utilities.Compiler

module ``Misc`` =
    let private compileForNetCore opts =
        opts |> ignoreWarnings |> withOptions ["-g"; "--optimize+"; "--targetprofile:netcore"] |> compile

    [<Fact>]
    let ``Empty array construct compiles to System.Array.Empty<_>()``() =
        FSharp """
module Misc

let zInt (): int[] = [||]

let zString (): string[] = [||]

let zGeneric<'a> (): 'a[] = [||]
         """
         |> compileForNetCore
         |> shouldSucceed
         |> verifyIL ["""
.method public static int32[] zInt() cil managed
{

  .maxstack  8
  IL_0000:  call       !!0[] [runtime]System.Array::Empty<int32>()
  IL_0005:  ret
} 

.method public static string[] zString() cil managed
{

  .maxstack  8
  IL_0000:  call       !!0[] [runtime]System.Array::Empty<string>()
  IL_0005:  ret
}

.method public static !!a[]  zGeneric<a>() cil managed
{

  .maxstack  8
  IL_0000:  call       !!0[] [runtime]System.Array::Empty<!!0>()
  IL_0005:  ret
}"""]
