// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.EmittedIL

open Xunit
open FSharp.Test.Utilities.Compiler

module ``TupleElimination`` =

    [<Fact>]
    let ``Sequence expressions with potential side effects do not prevent tuple elimination``() =
        FSharp """
module TupleElimination
open System.Runtime.CompilerServices

[<MethodImpl(MethodImplOptions.NoInlining)>]
let f () = 3

[<MethodImpl(MethodImplOptions.NoInlining)>]
let sideEffect () = ()

type Test =
    [<MethodImpl(MethodImplOptions.NoInlining)>]
    static member test(x: int32 * int32) = x

let v () =
    let a, b =
        "".ToString () |> ignore
        System.DateTime.Now |> ignore
        "3".ToString () |> ignore
        2L, f ()
    System.DateTime.Now |> ignore
    a, b

let w () =
    let (a, b) as t =
        "".ToString () |> ignore
        System.DateTime.Now |> ignore
        "3".ToString () |> ignore
        2, f ()
    System.DateTime.Now |> ignore
    let _ = Test.test(t)
    a + b

let x () =
    let a, b =
        "".ToString () |> ignore
        System.DateTime.Now |> ignore
        "3".ToString () |> ignore
        2, f ()
    System.DateTime.Now |> ignore
    a + b

let y () =
    let a, b, c =
        let a = f ()
        sideEffect ()
        a, f (), f ()
    a + b + c

let z () =
    let a, b, c =
        let u, v = 3, 4
        sideEffect ()
        f (), f () + u, f () + v
    a + b + c
         """
         |> compile
         |> shouldSucceed
         |> verifyIL [

// public static Tuple<long, int> v()
// {
//     string text = "".ToString();
//     DateTime now = DateTime.Now;
//     text = "3".ToString();
//     int item = TupleElimination.f();
//     now = DateTime.Now;
//     return new Tuple<long, int>(2L, item);
// }
                      """
.method public static class [runtime]System.Tuple`2<int64,int32> 
        v() cil managed
{
  
  .maxstack  4
  .locals init (string V_0,
           valuetype [runtime]System.DateTime V_1,
           int32 V_2)
  IL_0000:  ldstr      ""
  IL_0005:  callvirt   instance string [runtime]System.Object::ToString()
  IL_000a:  stloc.0
  IL_000b:  call       valuetype [runtime]System.DateTime [runtime]System.DateTime::get_Now()
  IL_0010:  stloc.1
  IL_0011:  ldstr      "3"
  IL_0016:  callvirt   instance string [runtime]System.Object::ToString()
  IL_001b:  stloc.0
  IL_001c:  call       int32 TupleElimination::f()
  IL_0021:  stloc.2
  IL_0022:  call       valuetype [runtime]System.DateTime [runtime]System.DateTime::get_Now()
  IL_0027:  stloc.1
  IL_0028:  ldc.i4.2
  IL_0029:  conv.i8
  IL_002a:  ldloc.2
  IL_002b:  newobj     instance void class [runtime]System.Tuple`2<int64,int32>::.ctor(!0,
                                                                                              !1)
  IL_0030:  ret
}
"""

// public static int w()
// {
//     string text = "".ToString();
//     DateTime now = DateTime.Now;
//     text = "3".ToString();
//     int num = TupleElimination.f();
//     Tuple<int, int> x = new Tuple<int, int>(2, num);
//     now = DateTime.Now;
//     TupleElimination.Test.test(x);
//     return 2 + num;
// }
                      """
.method public static int32  w() cil managed
{
  
  .maxstack  4
  .locals init (string V_0,
           valuetype [runtime]System.DateTime V_1,
           int32 V_2,
           class [runtime]System.Tuple`2<int32,int32> V_3)
  IL_0000:  ldstr      ""
  IL_0005:  callvirt   instance string [runtime]System.Object::ToString()
  IL_000a:  stloc.0
  IL_000b:  call       valuetype [runtime]System.DateTime [runtime]System.DateTime::get_Now()
  IL_0010:  stloc.1
  IL_0011:  ldstr      "3"
  IL_0016:  callvirt   instance string [runtime]System.Object::ToString()
  IL_001b:  stloc.0
  IL_001c:  call       int32 TupleElimination::f()
  IL_0021:  stloc.2
  IL_0022:  ldc.i4.2
  IL_0023:  ldloc.2
  IL_0024:  newobj     instance void class [runtime]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                              !1)
  IL_0029:  stloc.3
  IL_002a:  call       valuetype [runtime]System.DateTime [runtime]System.DateTime::get_Now()
  IL_002f:  stloc.1
  IL_0030:  ldloc.3
  IL_0031:  call       class [runtime]System.Tuple`2<int32,int32> TupleElimination/Test::test(class [runtime]System.Tuple`2<int32,int32>)
  IL_0036:  pop
  IL_0037:  ldc.i4.2
  IL_0038:  ldloc.2
  IL_0039:  add
  IL_003a:  ret
}
"""
            
// public static int x()
// {
//     string text = "".ToString();
//     DateTime now = DateTime.Now;
//     text = "3".ToString();
//     int num = TupleElimination.f();
//     now = DateTime.Now;
//     return 2 + num;
// }          
                      """
.method public static int32  x() cil managed
{
  
  .maxstack  4
  .locals init (string V_0,
           valuetype [runtime]System.DateTime V_1,
           int32 V_2)
  IL_0000:  ldstr      ""
  IL_0005:  callvirt   instance string [runtime]System.Object::ToString()
  IL_000a:  stloc.0
  IL_000b:  call       valuetype [runtime]System.DateTime [runtime]System.DateTime::get_Now()
  IL_0010:  stloc.1
  IL_0011:  ldstr      "3"
  IL_0016:  callvirt   instance string [runtime]System.Object::ToString()
  IL_001b:  stloc.0
  IL_001c:  call       int32 TupleElimination::f()
  IL_0021:  stloc.2
  IL_0022:  call       valuetype [runtime]System.DateTime [runtime]System.DateTime::get_Now()
  IL_0027:  stloc.1
  IL_0028:  ldc.i4.2
  IL_0029:  ldloc.2
  IL_002a:  add
  IL_002b:  ret
}
"""

// public static int y()
// {
//     int num = TupleElimination.f();
//     TupleElimination.sideEffect();
//     return num + TupleElimination.f() + TupleElimination.f();
// }
                      """
.method public static int32  y() cil managed
{
  
  .maxstack  4
  .locals init (int32 V_0)
  IL_0000:  call       int32 TupleElimination::f()
  IL_0005:  stloc.0
  IL_0006:  call       void TupleElimination::sideEffect()
  IL_000b:  ldloc.0
  IL_000c:  call       int32 TupleElimination::f()
  IL_0011:  add
  IL_0012:  call       int32 TupleElimination::f()
  IL_0017:  add
  IL_0018:  ret
}
"""

// public static int z()
// {
//     TupleElimination.sideEffect();
//     return TupleElimination.f() + (TupleElimination.f() + 3) + (TupleElimination.f() + 4);
// }
                      """
.method public static int32  z() cil managed
{
  
  .maxstack  8
  IL_0000:  call       void TupleElimination::sideEffect()
  IL_0005:  call       int32 TupleElimination::f()
  IL_000a:  call       int32 TupleElimination::f()
  IL_000f:  ldc.i4.3
  IL_0010:  add
  IL_0011:  add
  IL_0012:  call       int32 TupleElimination::f()
  IL_0017:  ldc.i4.4
  IL_0018:  add
  IL_0019:  add
  IL_001a:  ret
}
""" ]

    [<Fact>]
    let ``First class use of tuple prevents elimination``() =
        FSharp """
module TupleElimination
open System.Runtime.CompilerServices

[<MethodImpl(MethodImplOptions.NoInlining)>]
let f () = 3

type Test =
    [<MethodImpl(MethodImplOptions.NoInlining)>]
    static member test(x: int32 * int32) = x

let z () =
    let a, b =
        "".ToString () |> ignore
        System.DateTime.Now |> ignore
        "3".ToString () |> ignore
        Test.test(2, f ())
    System.DateTime.Now |> ignore
    a + b
         """
         |> compile
         |> shouldSucceed
         |> verifyIL [

// public static int z()
// {
//     string text = "".ToString();
//     DateTime now = DateTime.Now;
//     text = "3".ToString();
//     Tuple<int, int> tuple = TupleElimination.Test.test(new Tuple<int, int>(2, TupleElimination.f()));
//     int item = tuple.Item2;
//     int item2 = tuple.Item1;
//     now = DateTime.Now;
//     return item2 + item;
// }
                      """
.method public static int32  z() cil managed
{
  
  .maxstack  4
  .locals init (class [runtime]System.Tuple`2<int32,int32> V_0,
           string V_1,
           valuetype [runtime]System.DateTime V_2,
           int32 V_3,
           int32 V_4)
  IL_0000:  ldstr      ""
  IL_0005:  callvirt   instance string [runtime]System.Object::ToString()
  IL_000a:  stloc.1
  IL_000b:  call       valuetype [runtime]System.DateTime [runtime]System.DateTime::get_Now()
  IL_0010:  stloc.2
  IL_0011:  ldstr      "3"
  IL_0016:  callvirt   instance string [runtime]System.Object::ToString()
  IL_001b:  stloc.1
  IL_001c:  ldc.i4.2
  IL_001d:  call       int32 TupleElimination::f()
  IL_0022:  newobj     instance void class [runtime]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                              !1)
  IL_0027:  call       class [runtime]System.Tuple`2<int32,int32> TupleElimination/Test::test(class [runtime]System.Tuple`2<int32,int32>)
  IL_002c:  stloc.0
  IL_002d:  ldloc.0
  IL_002e:  call       instance !1 class [runtime]System.Tuple`2<int32,int32>::get_Item2()
  IL_0033:  stloc.3
  IL_0034:  ldloc.0
  IL_0035:  call       instance !0 class [runtime]System.Tuple`2<int32,int32>::get_Item1()
  IL_003a:  stloc.s    V_4
  IL_003c:  call       valuetype [runtime]System.DateTime [runtime]System.DateTime::get_Now()
  IL_0041:  stloc.2
  IL_0042:  ldloc.s    V_4
  IL_0044:  ldloc.3
  IL_0045:  add
  IL_0046:  ret
}""" ]
