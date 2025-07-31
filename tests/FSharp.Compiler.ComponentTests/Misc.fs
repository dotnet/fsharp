// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace EmittedIL

open Xunit
open FSharp.Test.Compiler

module ``Misc`` =
    [<Fact>]
    let ``Empty array construct compiles to System.Array.Empty<_>()``() =
        FSharp """
module Misc

let zInt (): int[] = [||]

let zString (): string[] = [||]

let zGeneric<'a> (): 'a[] = [||]
         """
         |> compile
         |> shouldSucceed
         |> verifyIL ["""
IL_0000:  call       !!0[] [runtime]System.Array::Empty<int32>()
IL_0005:  ret"""
                      """

IL_0000:  call       !!0[] [runtime]System.Array::Empty<string>()
IL_0005:  ret"""

                      """
IL_0000:  call       !!0[] [runtime]System.Array::Empty<!!0>()
IL_0005:  ret""" ]

    [<Fact>]
    let ``Discriminated union with generic statics generates single merged cctor``() =
        FSharp """
module DuplicateCctorFix

type TestUnion<'T when 'T: comparison> =
    | A of 'T
    | B of string  
    | C // nullary case that triggers union erasure .cctor for constant field initialization
    
    // Static member that triggers incremental class .cctor generation
    static member val StaticProperty = "test" with get, set
    
    // Another static member to ensure .cctor has meaningful initialization
    static member CompareStuff x y = compare x y
         """
         |> compile
         |> shouldSucceed
         |> verifyIL [""".method private specialname rtspecialname static 
          void  .cctor() cil managed"""]
