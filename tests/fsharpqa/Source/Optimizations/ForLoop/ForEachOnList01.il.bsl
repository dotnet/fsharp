
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.8.3928.0
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly extern FSharp.Core
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 6:0:0:0
}
.assembly extern netstandard
{
  .publickeytoken = (CC 7B 13 FF CD 2D DD 51 )                         // .{...-.Q
  .ver 2:0:0:0
}
.assembly ForEachOnList01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.ForEachOnList01
{
  // Offset: 0x00000000 Length: 0x000002E9
}
.mresource public FSharpOptimizationData.ForEachOnList01
{
  // Offset: 0x000002F0 Length: 0x000000DB
}
.module ForEachOnList01.dll
// MVID: {6220E4F8-56DF-F74F-A745-0383F8E42062}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x06C70000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed ForEachOnList01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable sealed nested assembly beforefieldinit test6@38
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>
  {
    .field static assembly initonly class ForEachOnList01/test6@38 @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::.ctor()
      IL_0006:  ret
    } // end of method test6@38::.ctor

    .method public strict virtual instance int32 
            Invoke(int32 x) cil managed
    {
      // Code size       4 (0x4)
      .maxstack  8
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 39,39 : 21,26 'C:\\GitHub\\dsyme\\fsharp\\tests\\fsharpqa\\source\\Optimizations\\ForLoop\\ForEachOnList01.fs'
      IL_0000:  ldarg.1
      IL_0001:  ldc.i4.1
      IL_0002:  add
      IL_0003:  ret
    } // end of method test6@38::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void ForEachOnList01/test6@38::.ctor()
      IL_0005:  stsfld     class ForEachOnList01/test6@38 ForEachOnList01/test6@38::@_instance
      IL_000a:  ret
    } // end of method test6@38::.cctor

  } // end of class test6@38

  .class auto ansi serializable sealed nested assembly beforefieldinit test7@47
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>
  {
    .field static assembly initonly class ForEachOnList01/test7@47 @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::.ctor()
      IL_0006:  ret
    } // end of method test7@47::.ctor

    .method public strict virtual instance int32 
            Invoke(int32 x) cil managed
    {
      // Code size       4 (0x4)
      .maxstack  8
      .line 48,48 : 21,26 ''
      IL_0000:  ldarg.1
      IL_0001:  ldc.i4.1
      IL_0002:  add
      IL_0003:  ret
    } // end of method test7@47::Invoke

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  10
      IL_0000:  newobj     instance void ForEachOnList01/test7@47::.ctor()
      IL_0005:  stsfld     class ForEachOnList01/test7@47 ForEachOnList01/test7@47::@_instance
      IL_000a:  ret
    } // end of method test7@47::.cctor

  } // end of class test7@47

  .method public static void  test1(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> lst) cil managed
  {
    // Code size       38 (0x26)
    .maxstack  4
    .locals init ([0] int32 z,
             [1] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_1,
             [2] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_2,
             [3] int32 x)
    .line 8,8 : 6,23 ''
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    .line 9,9 : 6,9 ''
    IL_0002:  ldarg.0
    IL_0003:  stloc.1
    IL_0004:  ldloc.1
    IL_0005:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_000a:  stloc.2
    .line 9,9 : 12,14 ''
    IL_000b:  ldloc.2
    IL_000c:  brfalse.s  IL_0025

    IL_000e:  ldloc.1
    IL_000f:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_0014:  stloc.3
    .line 10,10 : 10,20 ''
    IL_0015:  ldloc.0
    IL_0016:  ldloc.3
    IL_0017:  add
    IL_0018:  stloc.0
    IL_0019:  ldloc.2
    IL_001a:  stloc.1
    IL_001b:  ldloc.1
    IL_001c:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0021:  stloc.2
    .line 16707566,16707566 : 0,0 ''
    IL_0022:  nop
    IL_0023:  br.s       IL_000b

    IL_0025:  ret
  } // end of method ForEachOnList01::test1

  .method public static void  test2() cil managed
  {
    // Code size       60 (0x3c)
    .maxstack  6
    .locals init ([0] int32 z,
             [1] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_1,
             [2] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_2,
             [3] int32 x)
    .line 13,13 : 6,23 ''
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    .line 14,14 : 6,9 ''
    IL_0002:  ldc.i4.1
    IL_0003:  ldc.i4.2
    IL_0004:  ldc.i4.3
    IL_0005:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_000a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_000f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0014:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0019:  stloc.1
    IL_001a:  ldloc.1
    IL_001b:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0020:  stloc.2
    .line 14,14 : 12,14 ''
    IL_0021:  ldloc.2
    IL_0022:  brfalse.s  IL_003b

    IL_0024:  ldloc.1
    IL_0025:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_002a:  stloc.3
    .line 15,15 : 10,20 ''
    IL_002b:  ldloc.0
    IL_002c:  ldloc.3
    IL_002d:  add
    IL_002e:  stloc.0
    IL_002f:  ldloc.2
    IL_0030:  stloc.1
    IL_0031:  ldloc.1
    IL_0032:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0037:  stloc.2
    .line 16707566,16707566 : 0,0 ''
    IL_0038:  nop
    IL_0039:  br.s       IL_0021

    IL_003b:  ret
  } // end of method ForEachOnList01::test2

  .method public static void  test3() cil managed
  {
    // Code size       64 (0x40)
    .maxstack  6
    .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> xs,
             [1] int32 z,
             [2] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_2,
             [3] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_3,
             [4] int32 x)
    .line 18,18 : 6,22 ''
    IL_0000:  ldc.i4.1
    IL_0001:  ldc.i4.2
    IL_0002:  ldc.i4.3
    IL_0003:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_0008:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_000d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0012:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0017:  stloc.0
    .line 19,19 : 6,23 ''
    IL_0018:  ldc.i4.0
    IL_0019:  stloc.1
    .line 20,20 : 6,9 ''
    IL_001a:  ldloc.0
    IL_001b:  stloc.2
    IL_001c:  ldloc.2
    IL_001d:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0022:  stloc.3
    .line 20,20 : 12,14 ''
    IL_0023:  ldloc.3
    IL_0024:  brfalse.s  IL_003f

    IL_0026:  ldloc.2
    IL_0027:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_002c:  stloc.s    x
    .line 21,21 : 10,20 ''
    IL_002e:  ldloc.1
    IL_002f:  ldloc.s    x
    IL_0031:  add
    IL_0032:  stloc.1
    IL_0033:  ldloc.3
    IL_0034:  stloc.2
    IL_0035:  ldloc.2
    IL_0036:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_003b:  stloc.3
    .line 16707566,16707566 : 0,0 ''
    IL_003c:  nop
    IL_003d:  br.s       IL_0023

    IL_003f:  ret
  } // end of method ForEachOnList01::test3

  .method public static void  test4() cil managed
  {
    // Code size       64 (0x40)
    .maxstack  6
    .locals init ([0] int32 z,
             [1] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> xs,
             [2] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_2,
             [3] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_3,
             [4] int32 x)
    .line 24,24 : 6,23 ''
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    .line 25,25 : 6,22 ''
    IL_0002:  ldc.i4.1
    IL_0003:  ldc.i4.2
    IL_0004:  ldc.i4.3
    IL_0005:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_000a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_000f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0014:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0019:  stloc.1
    .line 26,26 : 6,9 ''
    IL_001a:  ldloc.1
    IL_001b:  stloc.2
    IL_001c:  ldloc.2
    IL_001d:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0022:  stloc.3
    .line 26,26 : 12,14 ''
    IL_0023:  ldloc.3
    IL_0024:  brfalse.s  IL_003f

    IL_0026:  ldloc.2
    IL_0027:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_002c:  stloc.s    x
    .line 27,27 : 10,20 ''
    IL_002e:  ldloc.0
    IL_002f:  ldloc.s    x
    IL_0031:  add
    IL_0032:  stloc.0
    IL_0033:  ldloc.3
    IL_0034:  stloc.2
    IL_0035:  ldloc.2
    IL_0036:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_003b:  stloc.3
    .line 16707566,16707566 : 0,0 ''
    IL_003c:  nop
    IL_003d:  br.s       IL_0023

    IL_003f:  ret
  } // end of method ForEachOnList01::test4

  .method public static void  test5() cil managed
  {
    // Code size       87 (0x57)
    .maxstack  6
    .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> xs,
             [1] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_1,
             [2] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_2,
             [3] int32 x,
             [4] class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_4)
    .line 30,30 : 6,22 ''
    IL_0000:  ldc.i4.1
    IL_0001:  ldc.i4.2
    IL_0002:  ldc.i4.3
    IL_0003:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_0008:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_000d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0012:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0017:  stloc.0
    .line 31,31 : 6,9 ''
    IL_0018:  ldloc.0
    IL_0019:  stloc.1
    IL_001a:  ldloc.1
    IL_001b:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0020:  stloc.2
    .line 31,31 : 12,14 ''
    IL_0021:  ldloc.2
    IL_0022:  brfalse.s  IL_0056

    IL_0024:  ldloc.1
    IL_0025:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_002a:  stloc.3
    .line 32,32 : 10,24 ''
    IL_002b:  ldstr      "%A"
    IL_0030:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::.ctor(string)
    IL_0035:  stloc.s    V_4
    IL_0037:  call       class [netstandard]System.IO.TextWriter [netstandard]System.Console::get_Out()
    IL_003c:  ldloc.s    V_4
    IL_003e:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatLineToTextWriter<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [mscorlib]System.IO.TextWriter,
                                                                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0043:  ldloc.3
    IL_0044:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0049:  pop
    IL_004a:  ldloc.2
    IL_004b:  stloc.1
    IL_004c:  ldloc.1
    IL_004d:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0052:  stloc.2
    .line 16707566,16707566 : 0,0 ''
    IL_0053:  nop
    IL_0054:  br.s       IL_0021

    IL_0056:  ret
  } // end of method ForEachOnList01::test5

  .method public static void  test6() cil managed
  {
    // Code size       99 (0x63)
    .maxstack  8
    .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_0,
             [1] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_1,
             [2] int32 i,
             [3] class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_3)
    .line 36,36 : 5,8 ''
    IL_0000:  ldsfld     class ForEachOnList01/test6@38 ForEachOnList01/test6@38::@_instance
    IL_0005:  ldc.i4.1
    IL_0006:  ldc.i4.2
    IL_0007:  ldc.i4.3
    IL_0008:  ldc.i4.4
    IL_0009:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_000e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0013:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0018:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_001d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0022:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!1> [FSharp.Core]Microsoft.FSharp.Collections.ListModule::Map<int32,int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0>)
    IL_0027:  stloc.0
    IL_0028:  ldloc.0
    IL_0029:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_002e:  stloc.1
    .line 36,36 : 11,13 ''
    IL_002f:  ldloc.1
    IL_0030:  brfalse.s  IL_0062

    IL_0032:  ldloc.0
    IL_0033:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_0038:  stloc.2
    .line 41,41 : 9,23 ''
    IL_0039:  ldstr      "%O"
    IL_003e:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::.ctor(string)
    IL_0043:  stloc.3
    IL_0044:  call       class [netstandard]System.IO.TextWriter [netstandard]System.Console::get_Out()
    IL_0049:  ldloc.3
    IL_004a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatLineToTextWriter<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [mscorlib]System.IO.TextWriter,
                                                                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_004f:  ldloc.2
    IL_0050:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0055:  pop
    IL_0056:  ldloc.1
    IL_0057:  stloc.0
    IL_0058:  ldloc.0
    IL_0059:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_005e:  stloc.1
    .line 16707566,16707566 : 0,0 ''
    IL_005f:  nop
    IL_0060:  br.s       IL_002f

    IL_0062:  ret
  } // end of method ForEachOnList01::test6

  .method public static void  test7() cil managed
  {
    // Code size       105 (0x69)
    .maxstack  8
    .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_0,
             [1] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_1,
             [2] int32 i,
             [3] int32 tmp,
             [4] class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_4)
    .line 45,45 : 5,8 ''
    IL_0000:  ldsfld     class ForEachOnList01/test7@47 ForEachOnList01/test7@47::@_instance
    IL_0005:  ldc.i4.1
    IL_0006:  ldc.i4.2
    IL_0007:  ldc.i4.3
    IL_0008:  ldc.i4.4
    IL_0009:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_000e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0013:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0018:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_001d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0022:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!1> [FSharp.Core]Microsoft.FSharp.Collections.ListModule::Map<int32,int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0>)
    IL_0027:  stloc.0
    IL_0028:  ldloc.0
    IL_0029:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_002e:  stloc.1
    .line 45,45 : 11,13 ''
    IL_002f:  ldloc.1
    IL_0030:  brfalse.s  IL_0068

    IL_0032:  ldloc.0
    IL_0033:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_0038:  stloc.2
    .line 50,50 : 9,24 ''
    IL_0039:  ldloc.2
    IL_003a:  ldc.i4.1
    IL_003b:  add
    IL_003c:  stloc.3
    .line 51,51 : 9,25 ''
    IL_003d:  ldstr      "%O"
    IL_0042:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::.ctor(string)
    IL_0047:  stloc.s    V_4
    IL_0049:  call       class [netstandard]System.IO.TextWriter [netstandard]System.Console::get_Out()
    IL_004e:  ldloc.s    V_4
    IL_0050:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatLineToTextWriter<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [mscorlib]System.IO.TextWriter,
                                                                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0055:  ldloc.3
    IL_0056:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_005b:  pop
    IL_005c:  ldloc.1
    IL_005d:  stloc.0
    IL_005e:  ldloc.0
    IL_005f:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0064:  stloc.1
    .line 16707566,16707566 : 0,0 ''
    IL_0065:  nop
    IL_0066:  br.s       IL_002f

    IL_0068:  ret
  } // end of method ForEachOnList01::test7

} // end of class ForEachOnList01

.class private abstract auto ansi sealed '<StartupCode$ForEachOnList01>'.$ForEachOnList01
       extends [mscorlib]System.Object
{
} // end of class '<StartupCode$ForEachOnList01>'.$ForEachOnList01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
