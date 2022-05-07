// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.EmittedIL

open Xunit
open FSharp.Test.Compiler

module TypeTestsInPatternMatching =

    [<Fact>]
    let ``Test codegen for one column of sealed types``() =
        FSharp """
module Test
let TestOneColumnOfTypeTestsWithSealedTypes(x: obj) =
    match x with
    | :? string -> 1
    | :? int -> 2
    | :? bool -> 3
    | :? float -> 4
    | :? char -> 5
    | _ -> 6
         """
         |> compile
         |> shouldSucceed
         |> verifyIL [
                      """
      .method public static int32  TestOneColumnOfTypeTestsWithSealedTypes(object x) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  isinst     [runtime]System.String
    IL_0006:  brtrue.s   IL_002a

    IL_0008:  ldarg.0
    IL_0009:  isinst     [runtime]System.Int32
    IL_000e:  brtrue.s   IL_002c

    IL_0010:  ldarg.0
    IL_0011:  isinst     [runtime]System.Boolean
    IL_0016:  brtrue.s   IL_002e

    IL_0018:  ldarg.0
    IL_0019:  isinst     [runtime]System.Double
    IL_001e:  brtrue.s   IL_0030

    IL_0020:  ldarg.0
    IL_0021:  isinst     [runtime]System.Char
    IL_0026:  brfalse.s  IL_0034

    IL_0028:  br.s       IL_0032

    IL_002a:  ldc.i4.1
    IL_002b:  ret

    IL_002c:  ldc.i4.2
    IL_002d:  ret

    IL_002e:  ldc.i4.3
    IL_002f:  ret

    IL_0030:  ldc.i4.4
    IL_0031:  ret

    IL_0032:  ldc.i4.5
    IL_0033:  ret

    IL_0034:  ldc.i4.6
    IL_0035:  ret
  } 
"""
       ]



    [<Fact>]
    let ``Test codegen for two columns of sealed types``() =
        FSharp """
module Test
let TestTwoColumnsOfTypeTestsWithSealedTypes(x: obj, y: obj) =
    match x, y with
    | :? string, :? string -> 1
    | :? int, :? int -> 2
    | :? bool, :? bool -> 3
    | :? float, :? float -> 4
    | :? char, :? char -> 5
    | _ -> 6
         """
         |> compile
         |> shouldSucceed
         |> verifyIL [
                      """
      .method public static int32  TestTwoColumnsOfTypeTestsWithSealedTypes(object x,
                                                                        object y) cil managed
  {
    
    .maxstack  3
    .locals init (string V_0)
    IL_0000:  ldarg.0
    IL_0001:  isinst     [runtime]System.String
    IL_0006:  brfalse.s  IL_0014

    IL_0008:  ldarg.1
    IL_0009:  isinst     [runtime]System.String
    IL_000e:  stloc.0
    IL_000f:  ldloc.0
    IL_0010:  brfalse.s  IL_005c

    IL_0012:  ldc.i4.1
    IL_0013:  ret

    IL_0014:  ldarg.0
    IL_0015:  isinst     [runtime]System.Int32
    IL_001a:  brfalse.s  IL_0026

    IL_001c:  ldarg.1
    IL_001d:  isinst     [runtime]System.Int32
    IL_0022:  brfalse.s  IL_005c

    IL_0024:  ldc.i4.2
    IL_0025:  ret

    IL_0026:  ldarg.0
    IL_0027:  isinst     [runtime]System.Boolean
    IL_002c:  brfalse.s  IL_0038

    IL_002e:  ldarg.1
    IL_002f:  isinst     [runtime]System.Boolean
    IL_0034:  brfalse.s  IL_005c

    IL_0036:  ldc.i4.3
    IL_0037:  ret

    IL_0038:  ldarg.0
    IL_0039:  isinst     [runtime]System.Double
    IL_003e:  brfalse.s  IL_004a

    IL_0040:  ldarg.1
    IL_0041:  isinst     [runtime]System.Double
    IL_0046:  brfalse.s  IL_005c

    IL_0048:  ldc.i4.4
    IL_0049:  ret

    IL_004a:  ldarg.0
    IL_004b:  isinst     [runtime]System.Char
    IL_0050:  brfalse.s  IL_005c

    IL_0052:  ldarg.1
    IL_0053:  isinst     [runtime]System.Char
    IL_0058:  brfalse.s  IL_005c

    IL_005a:  ldc.i4.5
    IL_005b:  ret

    IL_005c:  ldc.i4.6
    IL_005d:  ret
  } 
"""

       ]




    [<Fact>]
    let ``Test codegen for two columns of sealed types with bind``() =
        FSharp """
module Test
let TestTwoColumnsOfTypeTestsWithSealedTypes(x: obj, y: obj) =
    match x, y with
    | :? string as s1, (:? string as s2) -> s1.Length + s2.Length
    | :? int as i1, (:? int as i2) -> i1 + i2
    | :? bool as b1, (:? bool as b2) -> (if b1 then 1 else 0) + (if b2 then 1 else 0)
    | :? float as f1, (:? float as f2) -> int f2 + int f2
    | :? char as c1, (:? char as c2) -> int c1 + int c2
    | _ -> 6
         """
         |> compile
         |> shouldSucceed
         |> verifyIL [
                      """
      .method public static int32  TestTwoColumnsOfTypeTestsWithSealedTypes(object x,
                                                                        object y) cil managed
  {
    
    .maxstack  4
    .locals init (string V_0,
             string V_1,
             string V_2,
             int32 V_3,
             int32 V_4,
             bool V_5,
             bool V_6,
             float64 V_7,
             float64 V_8,
             char V_9,
             char V_10)
    IL_0000:  ldarg.0
    IL_0001:  isinst     [runtime]System.String
    IL_0006:  brfalse.s  IL_002c

    IL_0008:  ldarg.1
    IL_0009:  isinst     [runtime]System.String
    IL_000e:  stloc.0
    IL_000f:  ldloc.0
    IL_0010:  brfalse    IL_00d8

    IL_0015:  ldloc.0
    IL_0016:  stloc.1
    IL_0017:  ldarg.0
    IL_0018:  unbox.any  [runtime]System.String
    IL_001d:  stloc.2
    IL_001e:  ldloc.2
    IL_001f:  callvirt   instance int32 [runtime]System.String::get_Length()
    IL_0024:  ldloc.1
    IL_0025:  callvirt   instance int32 [runtime]System.String::get_Length()
    IL_002a:  add
    IL_002b:  ret

    IL_002c:  ldarg.0
    IL_002d:  isinst     [runtime]System.Int32
    IL_0032:  brfalse.s  IL_0053

    IL_0034:  ldarg.1
    IL_0035:  isinst     [runtime]System.Int32
    IL_003a:  brfalse    IL_00d8

    IL_003f:  ldarg.1
    IL_0040:  unbox.any  [runtime]System.Int32
    IL_0045:  stloc.3
    IL_0046:  ldarg.0
    IL_0047:  unbox.any  [runtime]System.Int32
    IL_004c:  stloc.s    V_4
    IL_004e:  ldloc.s    V_4
    IL_0050:  ldloc.3
    IL_0051:  add
    IL_0052:  ret

    IL_0053:  ldarg.0
    IL_0054:  isinst     [runtime]System.Boolean
    IL_0059:  brfalse.s  IL_0088

    IL_005b:  ldarg.1
    IL_005c:  isinst     [runtime]System.Boolean
    IL_0061:  brfalse    IL_00d8

    IL_0066:  ldarg.1
    IL_0067:  unbox.any  [runtime]System.Boolean
    IL_006c:  stloc.s    V_5
    IL_006e:  ldarg.0
    IL_006f:  unbox.any  [runtime]System.Boolean
    IL_0074:  stloc.s    V_6
    IL_0076:  ldloc.s    V_6
    IL_0078:  brfalse.s  IL_007d

    IL_007a:  ldc.i4.1
    IL_007b:  br.s       IL_007e

    IL_007d:  ldc.i4.0
    IL_007e:  ldloc.s    V_5
    IL_0080:  brfalse.s  IL_0085

    IL_0082:  ldc.i4.1
    IL_0083:  br.s       IL_0086

    IL_0085:  ldc.i4.0
    IL_0086:  add
    IL_0087:  ret

    IL_0088:  ldarg.0
    IL_0089:  isinst     [runtime]System.Double
    IL_008e:  brfalse.s  IL_00b0

    IL_0090:  ldarg.1
    IL_0091:  isinst     [runtime]System.Double
    IL_0096:  brfalse.s  IL_00d8

    IL_0098:  ldarg.1
    IL_0099:  unbox.any  [runtime]System.Double
    IL_009e:  stloc.s    V_7
    IL_00a0:  ldarg.0
    IL_00a1:  unbox.any  [runtime]System.Double
    IL_00a6:  stloc.s    V_8
    IL_00a8:  ldloc.s    V_7
    IL_00aa:  conv.i4
    IL_00ab:  ldloc.s    V_7
    IL_00ad:  conv.i4
    IL_00ae:  add
    IL_00af:  ret

    IL_00b0:  ldarg.0
    IL_00b1:  isinst     [runtime]System.Char
    IL_00b6:  brfalse.s  IL_00d8

    IL_00b8:  ldarg.1
    IL_00b9:  isinst     [runtime]System.Char
    IL_00be:  brfalse.s  IL_00d8

    IL_00c0:  ldarg.1
    IL_00c1:  unbox.any  [runtime]System.Char
    IL_00c6:  stloc.s    V_9
    IL_00c8:  ldarg.0
    IL_00c9:  unbox.any  [runtime]System.Char
    IL_00ce:  stloc.s    V_10
    IL_00d0:  ldloc.s    V_10
    IL_00d2:  conv.i4
    IL_00d3:  ldloc.s    V_9
    IL_00d5:  conv.i4
    IL_00d6:  add
    IL_00d7:  ret

    IL_00d8:  ldc.i4.6
    IL_00d9:  ret
  } 
"""
      ]