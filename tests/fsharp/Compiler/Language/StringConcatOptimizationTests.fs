// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open System
open NUnit.Framework

#if !NETCOREAPP
[<TestFixture>]
module StringConcatOptimizationTests =

    // helper methods in this test only run on the full framework
    [<Test>]
    let Optimizations () =
        let baseSource = """
module Test

open System

let arr = ResizeArray()

let inline ss (x: int) =
    arr.Add(x)
    "_" + x.ToString() + "_"
"""

        let test1Source = """
let test1 () =
    ss 1 + ss 2 + ss 3
"""
        let test1IL = """.method public static string  test1() cil managed
  {
    
    .maxstack  7
    .locals init (int32 V_0)
    IL_0000:  ldc.i4.1
    IL_0001:  stloc.0
    IL_0002:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test::get_arr()
    IL_0007:  ldc.i4.1
    IL_0008:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_000d:  ldstr      "_"
    IL_0012:  ldloca.s   V_0
    IL_0014:  constrained. [mscorlib]System.Int32
    IL_001a:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_001f:  ldstr      "_"
    IL_0024:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0029:  ldc.i4.2
    IL_002a:  stloc.0
    IL_002b:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test::get_arr()
    IL_0030:  ldc.i4.2
    IL_0031:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0036:  ldstr      "_"
    IL_003b:  ldloca.s   V_0
    IL_003d:  constrained. [mscorlib]System.Int32
    IL_0043:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0048:  ldstr      "_"
    IL_004d:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0052:  ldc.i4.3
    IL_0053:  stloc.0
    IL_0054:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test::get_arr()
    IL_0059:  ldc.i4.3
    IL_005a:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_005f:  ldstr      "_"
    IL_0064:  ldloca.s   V_0
    IL_0066:  constrained. [mscorlib]System.Int32
    IL_006c:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0071:  ldstr      "_"
    IL_0076:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_007b:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0080:  ret
  }"""

        let test2Source = """
let test2 () =
    ss 1 + ss 2 + ss 3 + ss 4
"""
        let test2IL = """.method public static string  test2() cil managed
  {
    
    .maxstack  8
    .locals init (int32 V_0)
    IL_0000:  ldc.i4.1
    IL_0001:  stloc.0
    IL_0002:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test::get_arr()
    IL_0007:  ldc.i4.1
    IL_0008:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_000d:  ldstr      "_"
    IL_0012:  ldloca.s   V_0
    IL_0014:  constrained. [mscorlib]System.Int32
    IL_001a:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_001f:  ldstr      "_"
    IL_0024:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0029:  ldc.i4.2
    IL_002a:  stloc.0
    IL_002b:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test::get_arr()
    IL_0030:  ldc.i4.2
    IL_0031:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0036:  ldstr      "_"
    IL_003b:  ldloca.s   V_0
    IL_003d:  constrained. [mscorlib]System.Int32
    IL_0043:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0048:  ldstr      "_"
    IL_004d:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0052:  ldc.i4.3
    IL_0053:  stloc.0
    IL_0054:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test::get_arr()
    IL_0059:  ldc.i4.3
    IL_005a:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_005f:  ldstr      "_"
    IL_0064:  ldloca.s   V_0
    IL_0066:  constrained. [mscorlib]System.Int32
    IL_006c:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0071:  ldstr      "_"
    IL_0076:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_007b:  ldc.i4.4
    IL_007c:  stloc.0
    IL_007d:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test::get_arr()
    IL_0082:  ldc.i4.4
    IL_0083:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0088:  ldstr      "_"
    IL_008d:  ldloca.s   V_0
    IL_008f:  constrained. [mscorlib]System.Int32
    IL_0095:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_009a:  ldstr      "_"
    IL_009f:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_00a4:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string,
                                                                string)
    IL_00a9:  ret
  }"""

        let test3Source = """
let test3 () =
    ss 1 + ss 2 + ss 3 + ss 4 + ss 5
"""
        let test3IL = """.method public static string  test3() cil managed
  {
    
    .maxstack  8
    .locals init (int32 V_0)
    IL_0000:  ldc.i4.5
    IL_0001:  newarr     [mscorlib]System.String
    IL_0006:  dup
    IL_0007:  ldc.i4.0
    IL_0008:  ldc.i4.1
    IL_0009:  stloc.0
    IL_000a:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test::get_arr()
    IL_000f:  ldc.i4.1
    IL_0010:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0015:  ldstr      "_"
    IL_001a:  ldloca.s   V_0
    IL_001c:  constrained. [mscorlib]System.Int32
    IL_0022:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0027:  ldstr      "_"
    IL_002c:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0031:  stelem     [mscorlib]System.String
    IL_0036:  dup
    IL_0037:  ldc.i4.1
    IL_0038:  ldc.i4.2
    IL_0039:  stloc.0
    IL_003a:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test::get_arr()
    IL_003f:  ldc.i4.2
    IL_0040:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0045:  ldstr      "_"
    IL_004a:  ldloca.s   V_0
    IL_004c:  constrained. [mscorlib]System.Int32
    IL_0052:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0057:  ldstr      "_"
    IL_005c:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0061:  stelem     [mscorlib]System.String
    IL_0066:  dup
    IL_0067:  ldc.i4.2
    IL_0068:  ldc.i4.3
    IL_0069:  stloc.0
    IL_006a:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test::get_arr()
    IL_006f:  ldc.i4.3
    IL_0070:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0075:  ldstr      "_"
    IL_007a:  ldloca.s   V_0
    IL_007c:  constrained. [mscorlib]System.Int32
    IL_0082:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0087:  ldstr      "_"
    IL_008c:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0091:  stelem     [mscorlib]System.String
    IL_0096:  dup
    IL_0097:  ldc.i4.3
    IL_0098:  ldc.i4.4
    IL_0099:  stloc.0
    IL_009a:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test::get_arr()
    IL_009f:  ldc.i4.4
    IL_00a0:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_00a5:  ldstr      "_"
    IL_00aa:  ldloca.s   V_0
    IL_00ac:  constrained. [mscorlib]System.Int32
    IL_00b2:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_00b7:  ldstr      "_"
    IL_00bc:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_00c1:  stelem     [mscorlib]System.String
    IL_00c6:  dup
    IL_00c7:  ldc.i4.4
    IL_00c8:  ldc.i4.5
    IL_00c9:  stloc.0
    IL_00ca:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test::get_arr()
    IL_00cf:  ldc.i4.5
    IL_00d0:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_00d5:  ldstr      "_"
    IL_00da:  ldloca.s   V_0
    IL_00dc:  constrained. [mscorlib]System.Int32
    IL_00e2:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_00e7:  ldstr      "_"
    IL_00ec:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_00f1:  stelem     [mscorlib]System.String
    IL_00f6:  call       string [mscorlib]System.String::Concat(string[])
    IL_00fb:  ret
  }"""
  
        let test4Source = """
let test4 () =
    ss 5 + ss 6 + ss 7 + String.Concat(ss 8, ss 9) + ss 10 + "_50_" + "_60_" + String.Concat(ss 100, String.Concat(ss 101, ss 102), ss 103) + String.Concat([|"_104_";"_105_"|]) + ss 106
"""
        let test4IL = """.method public static string  test4() cil managed
  {
    
    .maxstack  8
    .locals init (int32 V_0)
    IL_0000:  ldc.i4.s   13
    IL_0002:  newarr     [mscorlib]System.String
    IL_0007:  dup
    IL_0008:  ldc.i4.0
    IL_0009:  ldc.i4.5
    IL_000a:  stloc.0
    IL_000b:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test::get_arr()
    IL_0010:  ldc.i4.5
    IL_0011:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0016:  ldstr      "_"
    IL_001b:  ldloca.s   V_0
    IL_001d:  constrained. [mscorlib]System.Int32
    IL_0023:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0028:  ldstr      "_"
    IL_002d:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0032:  stelem     [mscorlib]System.String
    IL_0037:  dup
    IL_0038:  ldc.i4.1
    IL_0039:  ldc.i4.6
    IL_003a:  stloc.0
    IL_003b:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test::get_arr()
    IL_0040:  ldc.i4.6
    IL_0041:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0046:  ldstr      "_"
    IL_004b:  ldloca.s   V_0
    IL_004d:  constrained. [mscorlib]System.Int32
    IL_0053:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0058:  ldstr      "_"
    IL_005d:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0062:  stelem     [mscorlib]System.String
    IL_0067:  dup
    IL_0068:  ldc.i4.2
    IL_0069:  ldc.i4.7
    IL_006a:  stloc.0
    IL_006b:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test::get_arr()
    IL_0070:  ldc.i4.7
    IL_0071:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0076:  ldstr      "_"
    IL_007b:  ldloca.s   V_0
    IL_007d:  constrained. [mscorlib]System.Int32
    IL_0083:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0088:  ldstr      "_"
    IL_008d:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0092:  stelem     [mscorlib]System.String
    IL_0097:  dup
    IL_0098:  ldc.i4.3
    IL_0099:  ldc.i4.8
    IL_009a:  stloc.0
    IL_009b:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test::get_arr()
    IL_00a0:  ldc.i4.8
    IL_00a1:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_00a6:  ldstr      "_"
    IL_00ab:  ldloca.s   V_0
    IL_00ad:  constrained. [mscorlib]System.Int32
    IL_00b3:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_00b8:  ldstr      "_"
    IL_00bd:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_00c2:  stelem     [mscorlib]System.String
    IL_00c7:  dup
    IL_00c8:  ldc.i4.4
    IL_00c9:  ldc.i4.s   9
    IL_00cb:  stloc.0
    IL_00cc:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test::get_arr()
    IL_00d1:  ldc.i4.s   9
    IL_00d3:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_00d8:  ldstr      "_"
    IL_00dd:  ldloca.s   V_0
    IL_00df:  constrained. [mscorlib]System.Int32
    IL_00e5:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_00ea:  ldstr      "_"
    IL_00ef:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_00f4:  stelem     [mscorlib]System.String
    IL_00f9:  dup
    IL_00fa:  ldc.i4.5
    IL_00fb:  ldc.i4.s   10
    IL_00fd:  stloc.0
    IL_00fe:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test::get_arr()
    IL_0103:  ldc.i4.s   10
    IL_0105:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_010a:  ldstr      "_"
    IL_010f:  ldloca.s   V_0
    IL_0111:  constrained. [mscorlib]System.Int32
    IL_0117:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_011c:  ldstr      "_"
    IL_0121:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0126:  stelem     [mscorlib]System.String
    IL_012b:  dup
    IL_012c:  ldc.i4.6
    IL_012d:  ldstr      "_50__60_"
    IL_0132:  stelem     [mscorlib]System.String
    IL_0137:  dup
    IL_0138:  ldc.i4.7
    IL_0139:  ldc.i4.s   100
    IL_013b:  stloc.0
    IL_013c:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test::get_arr()
    IL_0141:  ldc.i4.s   100
    IL_0143:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0148:  ldstr      "_"
    IL_014d:  ldloca.s   V_0
    IL_014f:  constrained. [mscorlib]System.Int32
    IL_0155:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_015a:  ldstr      "_"
    IL_015f:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0164:  stelem     [mscorlib]System.String
    IL_0169:  dup
    IL_016a:  ldc.i4.8
    IL_016b:  ldc.i4.s   101
    IL_016d:  stloc.0
    IL_016e:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test::get_arr()
    IL_0173:  ldc.i4.s   101
    IL_0175:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_017a:  ldstr      "_"
    IL_017f:  ldloca.s   V_0
    IL_0181:  constrained. [mscorlib]System.Int32
    IL_0187:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_018c:  ldstr      "_"
    IL_0191:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0196:  stelem     [mscorlib]System.String
    IL_019b:  dup
    IL_019c:  ldc.i4.s   9
    IL_019e:  ldc.i4.s   102
    IL_01a0:  stloc.0
    IL_01a1:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test::get_arr()
    IL_01a6:  ldc.i4.s   102
    IL_01a8:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_01ad:  ldstr      "_"
    IL_01b2:  ldloca.s   V_0
    IL_01b4:  constrained. [mscorlib]System.Int32
    IL_01ba:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_01bf:  ldstr      "_"
    IL_01c4:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_01c9:  stelem     [mscorlib]System.String
    IL_01ce:  dup
    IL_01cf:  ldc.i4.s   10
    IL_01d1:  ldc.i4.s   103
    IL_01d3:  stloc.0
    IL_01d4:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test::get_arr()
    IL_01d9:  ldc.i4.s   103
    IL_01db:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_01e0:  ldstr      "_"
    IL_01e5:  ldloca.s   V_0
    IL_01e7:  constrained. [mscorlib]System.Int32
    IL_01ed:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_01f2:  ldstr      "_"
    IL_01f7:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_01fc:  stelem     [mscorlib]System.String
    IL_0201:  dup
    IL_0202:  ldc.i4.s   11
    IL_0204:  ldstr      "_104__105_"
    IL_0209:  stelem     [mscorlib]System.String
    IL_020e:  dup
    IL_020f:  ldc.i4.s   12
    IL_0211:  ldc.i4.s   106
    IL_0213:  stloc.0
    IL_0214:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test::get_arr()
    IL_0219:  ldc.i4.s   106
    IL_021b:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0220:  ldstr      "_"
    IL_0225:  ldloca.s   V_0
    IL_0227:  constrained. [mscorlib]System.Int32
    IL_022d:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0232:  ldstr      "_"
    IL_0237:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_023c:  stelem     [mscorlib]System.String
    IL_0241:  call       string [mscorlib]System.String::Concat(string[])
    IL_0246:  ret
  }"""
  
        let test5Source = """
let test5 () =
    ss 5 + ss 6 + ss 7 + String.Concat(ss 8, ss 9) + ss 10 + "_50_" + "_60_" + String.Concat(ss 100, (let x = String.Concat(ss 101, ss 102) in Console.WriteLine(x);x), ss 103) + String.Concat([|"_104_";"_105_"|]) + ss 106
"""
        let test5IL = """.method public static string  test5() cil managed
  {
    
    .maxstack  9
    .locals init (int32 V_0,
             string V_1)
    IL_0000:  ldc.i4.s   12
    IL_0002:  newarr     [mscorlib]System.String
    IL_0007:  dup
    IL_0008:  ldc.i4.0
    IL_0009:  ldc.i4.5
    IL_000a:  stloc.0
    IL_000b:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test::get_arr()
    IL_0010:  ldc.i4.5
    IL_0011:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0016:  ldstr      "_"
    IL_001b:  ldloca.s   V_0
    IL_001d:  constrained. [mscorlib]System.Int32
    IL_0023:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0028:  ldstr      "_"
    IL_002d:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0032:  stelem     [mscorlib]System.String
    IL_0037:  dup
    IL_0038:  ldc.i4.1
    IL_0039:  ldc.i4.6
    IL_003a:  stloc.0
    IL_003b:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test::get_arr()
    IL_0040:  ldc.i4.6
    IL_0041:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0046:  ldstr      "_"
    IL_004b:  ldloca.s   V_0
    IL_004d:  constrained. [mscorlib]System.Int32
    IL_0053:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0058:  ldstr      "_"
    IL_005d:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0062:  stelem     [mscorlib]System.String
    IL_0067:  dup
    IL_0068:  ldc.i4.2
    IL_0069:  ldc.i4.7
    IL_006a:  stloc.0
    IL_006b:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test::get_arr()
    IL_0070:  ldc.i4.7
    IL_0071:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0076:  ldstr      "_"
    IL_007b:  ldloca.s   V_0
    IL_007d:  constrained. [mscorlib]System.Int32
    IL_0083:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0088:  ldstr      "_"
    IL_008d:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0092:  stelem     [mscorlib]System.String
    IL_0097:  dup
    IL_0098:  ldc.i4.3
    IL_0099:  ldc.i4.8
    IL_009a:  stloc.0
    IL_009b:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test::get_arr()
    IL_00a0:  ldc.i4.8
    IL_00a1:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_00a6:  ldstr      "_"
    IL_00ab:  ldloca.s   V_0
    IL_00ad:  constrained. [mscorlib]System.Int32
    IL_00b3:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_00b8:  ldstr      "_"
    IL_00bd:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_00c2:  stelem     [mscorlib]System.String
    IL_00c7:  dup
    IL_00c8:  ldc.i4.4
    IL_00c9:  ldc.i4.s   9
    IL_00cb:  stloc.0
    IL_00cc:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test::get_arr()
    IL_00d1:  ldc.i4.s   9
    IL_00d3:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_00d8:  ldstr      "_"
    IL_00dd:  ldloca.s   V_0
    IL_00df:  constrained. [mscorlib]System.Int32
    IL_00e5:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_00ea:  ldstr      "_"
    IL_00ef:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_00f4:  stelem     [mscorlib]System.String
    IL_00f9:  dup
    IL_00fa:  ldc.i4.5
    IL_00fb:  ldc.i4.s   10
    IL_00fd:  stloc.0
    IL_00fe:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test::get_arr()
    IL_0103:  ldc.i4.s   10
    IL_0105:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_010a:  ldstr      "_"
    IL_010f:  ldloca.s   V_0
    IL_0111:  constrained. [mscorlib]System.Int32
    IL_0117:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_011c:  ldstr      "_"
    IL_0121:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0126:  stelem     [mscorlib]System.String
    IL_012b:  dup
    IL_012c:  ldc.i4.6
    IL_012d:  ldstr      "_50__60_"
    IL_0132:  stelem     [mscorlib]System.String
    IL_0137:  dup
    IL_0138:  ldc.i4.7
    IL_0139:  ldc.i4.s   100
    IL_013b:  stloc.0
    IL_013c:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test::get_arr()
    IL_0141:  ldc.i4.s   100
    IL_0143:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0148:  ldstr      "_"
    IL_014d:  ldloca.s   V_0
    IL_014f:  constrained. [mscorlib]System.Int32
    IL_0155:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_015a:  ldstr      "_"
    IL_015f:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0164:  stelem     [mscorlib]System.String
    IL_0169:  dup
    IL_016a:  ldc.i4.8
    IL_016b:  ldc.i4.s   101
    IL_016d:  stloc.0
    IL_016e:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test::get_arr()
    IL_0173:  ldc.i4.s   101
    IL_0175:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_017a:  ldstr      "_"
    IL_017f:  ldloca.s   V_0
    IL_0181:  constrained. [mscorlib]System.Int32
    IL_0187:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_018c:  ldstr      "_"
    IL_0191:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0196:  ldc.i4.s   102
    IL_0198:  stloc.0
    IL_0199:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test::get_arr()
    IL_019e:  ldc.i4.s   102
    IL_01a0:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_01a5:  ldstr      "_"
    IL_01aa:  ldloca.s   V_0
    IL_01ac:  constrained. [mscorlib]System.Int32
    IL_01b2:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_01b7:  ldstr      "_"
    IL_01bc:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_01c1:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_01c6:  stloc.1
    IL_01c7:  ldloc.1
    IL_01c8:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_01cd:  ldloc.1
    IL_01ce:  stelem     [mscorlib]System.String
    IL_01d3:  dup
    IL_01d4:  ldc.i4.s   9
    IL_01d6:  ldc.i4.s   103
    IL_01d8:  stloc.0
    IL_01d9:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test::get_arr()
    IL_01de:  ldc.i4.s   103
    IL_01e0:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_01e5:  ldstr      "_"
    IL_01ea:  ldloca.s   V_0
    IL_01ec:  constrained. [mscorlib]System.Int32
    IL_01f2:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_01f7:  ldstr      "_"
    IL_01fc:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0201:  stelem     [mscorlib]System.String
    IL_0206:  dup
    IL_0207:  ldc.i4.s   10
    IL_0209:  ldstr      "_104__105_"
    IL_020e:  stelem     [mscorlib]System.String
    IL_0213:  dup
    IL_0214:  ldc.i4.s   11
    IL_0216:  ldc.i4.s   106
    IL_0218:  stloc.0
    IL_0219:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test::get_arr()
    IL_021e:  ldc.i4.s   106
    IL_0220:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0225:  ldstr      "_"
    IL_022a:  ldloca.s   V_0
    IL_022c:  constrained. [mscorlib]System.Int32
    IL_0232:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0237:  ldstr      "_"
    IL_023c:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0241:  stelem     [mscorlib]System.String
    IL_0246:  call       string [mscorlib]System.String::Concat(string[])
    IL_024b:  ret
  }"""

        let test6Source = """
let inline inlineStringConcat str1 str2 = str1 + str2

let test6 () =
    inlineStringConcat (inlineStringConcat (ss 1) (ss 2)) (ss 3) + ss 4
"""
        let test6IL = """.method public static string  test6() cil managed
  {
    
    .maxstack  8
    .locals init (int32 V_0)
    IL_0000:  ldc.i4.1
    IL_0001:  stloc.0
    IL_0002:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test::get_arr()
    IL_0007:  ldc.i4.1
    IL_0008:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_000d:  ldstr      "_"
    IL_0012:  ldloca.s   V_0
    IL_0014:  constrained. [mscorlib]System.Int32
    IL_001a:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_001f:  ldstr      "_"
    IL_0024:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0029:  ldc.i4.2
    IL_002a:  stloc.0
    IL_002b:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test::get_arr()
    IL_0030:  ldc.i4.2
    IL_0031:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0036:  ldstr      "_"
    IL_003b:  ldloca.s   V_0
    IL_003d:  constrained. [mscorlib]System.Int32
    IL_0043:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0048:  ldstr      "_"
    IL_004d:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0052:  ldc.i4.3
    IL_0053:  stloc.0
    IL_0054:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test::get_arr()
    IL_0059:  ldc.i4.3
    IL_005a:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_005f:  ldstr      "_"
    IL_0064:  ldloca.s   V_0
    IL_0066:  constrained. [mscorlib]System.Int32
    IL_006c:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0071:  ldstr      "_"
    IL_0076:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_007b:  ldc.i4.4
    IL_007c:  stloc.0
    IL_007d:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test::get_arr()
    IL_0082:  ldc.i4.4
    IL_0083:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0088:  ldstr      "_"
    IL_008d:  ldloca.s   V_0
    IL_008f:  constrained. [mscorlib]System.Int32
    IL_0095:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_009a:  ldstr      "_"
    IL_009f:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_00a4:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string,
                                                                string)
    IL_00a9:  ret
  }"""

        let test7Source = """
let test7 () =
    let x = 1
    x.ToString() + x.ToString() + x.ToString()
"""
        let test7IL = """.method public static string  test7() cil managed
  {
    
    .maxstack  5
    .locals init (int32 V_0)
    IL_0000:  ldc.i4.1
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_0
    IL_0004:  constrained. [mscorlib]System.Int32
    IL_000a:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_000f:  ldloca.s   V_0
    IL_0011:  constrained. [mscorlib]System.Int32
    IL_0017:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_001c:  ldloca.s   V_0
    IL_001e:  constrained. [mscorlib]System.Int32
    IL_0024:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0029:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_002e:  ret
  }"""

        let test8Source = """
let test8 () =
    let x = 1
    x.ToString() + x.ToString() + x.ToString() + x.ToString()
"""
        let test8IL = """.method public static string  test8() cil managed
  {
    
    .maxstack  6
    .locals init (int32 V_0)
    IL_0000:  ldc.i4.1
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_0
    IL_0004:  constrained. [mscorlib]System.Int32
    IL_000a:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_000f:  ldloca.s   V_0
    IL_0011:  constrained. [mscorlib]System.Int32
    IL_0017:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_001c:  ldloca.s   V_0
    IL_001e:  constrained. [mscorlib]System.Int32
    IL_0024:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0029:  ldloca.s   V_0
    IL_002b:  constrained. [mscorlib]System.Int32
    IL_0031:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0036:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string,
                                                                string)
    IL_003b:  ret
  }"""

        let test9Source = """
let test9 () =
    let x = 1
    x.ToString() + x.ToString() + x.ToString() + x.ToString() + x.ToString()
"""
        let test9IL = """.method public static string  test9() cil managed
  {
    
    .maxstack  6
    .locals init (int32 V_0)
    IL_0000:  ldc.i4.1
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.5
    IL_0003:  newarr     [mscorlib]System.String
    IL_0008:  dup
    IL_0009:  ldc.i4.0
    IL_000a:  ldloca.s   V_0
    IL_000c:  constrained. [mscorlib]System.Int32
    IL_0012:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0017:  stelem     [mscorlib]System.String
    IL_001c:  dup
    IL_001d:  ldc.i4.1
    IL_001e:  ldloca.s   V_0
    IL_0020:  constrained. [mscorlib]System.Int32
    IL_0026:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_002b:  stelem     [mscorlib]System.String
    IL_0030:  dup
    IL_0031:  ldc.i4.2
    IL_0032:  ldloca.s   V_0
    IL_0034:  constrained. [mscorlib]System.Int32
    IL_003a:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_003f:  stelem     [mscorlib]System.String
    IL_0044:  dup
    IL_0045:  ldc.i4.3
    IL_0046:  ldloca.s   V_0
    IL_0048:  constrained. [mscorlib]System.Int32
    IL_004e:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0053:  stelem     [mscorlib]System.String
    IL_0058:  dup
    IL_0059:  ldc.i4.4
    IL_005a:  ldloca.s   V_0
    IL_005c:  constrained. [mscorlib]System.Int32
    IL_0062:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0067:  stelem     [mscorlib]System.String
    IL_006c:  call       string [mscorlib]System.String::Concat(string[])
    IL_0071:  ret
  }"""

        let sources = 
            [
                baseSource
                test1Source
                test2Source
                test3Source
                test4Source
                test5Source
                test6Source
                test7Source
                test8Source
                test9Source
            ]
        let source = String.Join("", sources)
        ILChecker.check source
            [
                test1IL
                test2IL
                test3IL
                test4IL
                test5IL
                test6IL
                test7IL
                test8IL
                test9IL
            ]
#endif