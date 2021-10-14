// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.EmittedIL

open Xunit
open FSharp.Test.Compiler

#if NETCOREAPP
module ``SkipLocalsInit`` =
    [<Fact>]
    let ``Init in function and closure not emitted when applied on function``() =
        FSharp """
module SkipLocalsInit

[<System.Runtime.CompilerServices.SkipLocalsInitAttribute>]
let x () =
    [||] |> Array.filter (fun x -> let y = "".Length in y + y = x) |> ignore
"""
         |> compile
         |> shouldSucceed
         |> verifyIL ["""
.method public static void  x() cil managed
{
  .custom instance void [runtime]System.Runtime.CompilerServices.SkipLocalsInitAttribute::.ctor() = ( 01 00 00 00 )

  .maxstack  4
  .locals (int32[] V_0)
"""

                      """
.method public strict virtual instance bool 
        Invoke(int32 x) cil managed
{
          
  .maxstack  6
  .locals (int32 V_0)"""]

    [<Fact>]
    let ``Init in static method not emitted when applied on class``() =
        FSharp """
module SkipLocalsInit

[<System.Runtime.CompilerServices.SkipLocalsInitAttribute>]
type X () =
    static member Y () =
        let x = "ssa".Length
        x + x
         """
         |> compile
         |> shouldSucceed
         |> verifyIL ["""
.custom instance void [runtime]System.Runtime.CompilerServices.SkipLocalsInitAttribute::.ctor() = ( 01 00 00 00 ) 
.custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
"""

                      """
.method public static int32  Y() cil managed
{
  
  .maxstack  4
  .locals (int32 V_0)"""]

    [<Fact>]
    let ``Init in static method and function not emitted when applied on module``() =
        FSharp """
[<System.Runtime.CompilerServices.SkipLocalsInitAttribute>]
module SkipLocalsInit

let x () =
    let x = "ssa".Length
    x + x

type X () =
    static member Y () =
        let x = "ssa".Length
        x + x
         """
         |> compile
         |> shouldSucceed
         |> verifyIL ["""
.custom instance void [runtime]System.Runtime.CompilerServices.SkipLocalsInitAttribute::.ctor() = ( 01 00 00 00 ) 
.custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
"""

                      """
.method public static int32  x() cil managed
{

  .maxstack  4
  .locals (int32 V_0)
"""

                      """
.method public static int32  Y() cil managed
{
  
  .maxstack  4
  .locals (int32 V_0)"""]

    [<Fact>]
    let ``Init in method and closure not emitted when applied on method``() =
        FSharp """
module SkipLocalsInit

type X () =
    [<System.Runtime.CompilerServices.SkipLocalsInitAttribute>]
    member _.Y () =
        [||] |> Array.filter (fun x -> let y = "".Length in y + y = x) |> ignore
        let x = "ssa".Length
        x + x
         """
         |> compile
         |> shouldSucceed
         |> verifyIL ["""
.method public hidebysig instance int32 
        Y() cil managed
{
  .custom instance void [runtime]System.Runtime.CompilerServices.SkipLocalsInitAttribute::.ctor() = ( 01 00 00 00 ) 
  
  .maxstack  4
  .locals (int32[] V_0,
           int32 V_1)
"""
           
                      """
.method public strict virtual instance bool 
        Invoke(int32 x) cil managed
{
  
  .maxstack  6
  .locals (int32 V_0)
""" ]

    [<Fact>]
    let ``Zero init performed to get defaults despite the attribute``() =
        FSharp """
module SkipLocalsInit
open System

[<System.Runtime.CompilerServices.SkipLocalsInit>]
let z () =
    let mutable a = Unchecked.defaultof<System.DateTime>
    a

[<System.Runtime.CompilerServices.SkipLocalsInitAttribute>]
let x f =
    let a = if 1 / 1 = 1 then Nullable () else Nullable 5L
    f a |> ignore
        """
        |> compile
        |> shouldSucceed
        |> verifyIL ["""
.locals (valuetype [runtime]System.DateTime V_0)
IL_0000:  ldloca.s   V_0
IL_0002:  initobj    [runtime]System.DateTime
IL_0008:  ldloc.0
IL_0009:  ret
        """
        
                     """
.locals (valuetype [runtime]System.Nullable`1<int64> V_0,
         valuetype [runtime]System.Nullable`1<int64> V_1,
         !!a V_2)
IL_0000:  ldc.i4.1
IL_0001:  ldc.i4.1
IL_0002:  div
IL_0003:  ldc.i4.1
IL_0004:  bne.un.s   IL_0011

IL_0006:  ldloca.s   V_1
IL_0008:  initobj    valuetype [runtime]System.Nullable`1<int64>
IL_000e:  ldloc.1
IL_000f:  br.s       IL_0018

IL_0011:  ldc.i4.5
IL_0012:  conv.i8
IL_0013:  newobj     instance void valuetype [runtime]System.Nullable`1<int64>::.ctor(!0)
IL_0018:  stloc.0
IL_0019:  ldarg.0
IL_001a:  ldloc.0
IL_001b:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype [runtime]System.Nullable`1<int64>,!!a>::Invoke(!0)
IL_0020:  stloc.2
IL_0021:  ret"""]
#endif