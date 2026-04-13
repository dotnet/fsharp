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

    [<Fact(Skip = "Blocked on compiler fix: duplicate .cctor in generic DU with static member val and nullary cases")>]
    let ``Discriminated union with generic statics generates single cctor calling renamed methods``() =
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
          void  .cctor() cil managed
  {
    // Code size
    IL_0000:  call       void DuplicateCctorFix/TestUnion`1::cctor_renamed_0()
    IL_0005:  call       void DuplicateCctorFix/TestUnion`1::cctor_renamed_1()
    IL_000a:  ret
  } // end of method TestUnion`1::.cctor

  .method private static void cctor_renamed_0() cil managed
  .method private static void cctor_renamed_1() cil managed"""]
