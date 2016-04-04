
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.1
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
  .ver 4:4:1:0
}
.assembly ForEachOnString01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // 
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.ForEachOnString01
{
  // Offset: 0x00000000 Length: 0x0000036E
}
.mresource public FSharpOptimizationData.ForEachOnString01
{
  // Offset: 0x00000378 Length: 0x000000FF
}
.module ForEachOnString01.dll
// MVID: {547FB1E9-105C-852B-A745-0383E9B17F54}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x029A0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed ForEachOnString01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable nested assembly beforefieldinit test8@54
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,char>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // 
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,char>::.ctor()
      IL_0006:  ret
    } // end of method test8@54::.ctor

    .method public strict virtual instance char 
            Invoke(char x) cil managed
    {
      // 
      .maxstack  8
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 55,55 : 21,39 'C:\\Users\\latkin\\Source\\Repos\\visualfsharp\\tests\\fsharpqa\\Source\\Optimizations\\ForLoop\\ForEachOnString01.fs'
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  conv.i4
      IL_0003:  ldc.i4.1
      IL_0004:  add
      IL_0005:  conv.u2
      IL_0006:  ret
    } // end of method test8@54::Invoke

  } // end of class test8@54

  .class auto ansi serializable nested assembly beforefieldinit test9@63
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,char>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // 
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,char>::.ctor()
      IL_0006:  ret
    } // end of method test9@63::.ctor

    .method public strict virtual instance char 
            Invoke(char x) cil managed
    {
      // 
      .maxstack  8
      .line 64,64 : 21,39 ''
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  conv.i4
      IL_0003:  ldc.i4.1
      IL_0004:  add
      IL_0005:  conv.u2
      IL_0006:  ret
    } // end of method test9@63::Invoke

  } // end of class test9@63

  .method public static void  test1(string str) cil managed
  {
    // 
    .maxstack  5
    .locals init ([0] int32 z,
             [1] int32 V_1,
             [2] int32 V_2,
             [3] char x)
    .line 8,8 : 6,23 ''
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    .line 9,9 : 6,21 ''
    IL_0003:  ldc.i4.0
    IL_0004:  stloc.2
    IL_0005:  ldarg.0
    IL_0006:  callvirt   instance int32 [mscorlib]System.String::get_Length()
    IL_000b:  ldc.i4.1
    IL_000c:  sub
    IL_000d:  stloc.1
    IL_000e:  ldloc.1
    IL_000f:  ldloc.2
    IL_0010:  blt.s      IL_0029

    .line 10,10 : 10,26 ''
    IL_0012:  ldarg.0
    IL_0013:  ldloc.2
    IL_0014:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0019:  stloc.3
    IL_001a:  ldloc.0
    IL_001b:  ldloc.3
    IL_001c:  conv.i4
    IL_001d:  add
    IL_001e:  stloc.0
    IL_001f:  ldloc.2
    IL_0020:  ldc.i4.1
    IL_0021:  add
    IL_0022:  stloc.2
    .line 9,9 : 6,21 ''
    IL_0023:  ldloc.2
    IL_0024:  ldloc.1
    IL_0025:  ldc.i4.1
    IL_0026:  add
    IL_0027:  bne.un.s   IL_0012

    IL_0029:  ret
  } // end of method ForEachOnString01::test1

  .method public static void  test2() cil managed
  {
    // 
    .maxstack  5
    .locals init ([0] int32 z,
             [1] int32 V_1,
             [2] int32 V_2,
             [3] char x)
    .line 13,13 : 6,23 ''
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    .line 14,14 : 6,23 ''
    IL_0003:  ldc.i4.0
    IL_0004:  stloc.2
    IL_0005:  ldstr      "123"
    IL_000a:  callvirt   instance int32 [mscorlib]System.String::get_Length()
    IL_000f:  ldc.i4.1
    IL_0010:  sub
    IL_0011:  stloc.1
    IL_0012:  ldloc.1
    IL_0013:  ldloc.2
    IL_0014:  blt.s      IL_0031

    .line 15,15 : 10,26 ''
    IL_0016:  ldstr      "123"
    IL_001b:  ldloc.2
    IL_001c:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0021:  stloc.3
    IL_0022:  ldloc.0
    IL_0023:  ldloc.3
    IL_0024:  conv.i4
    IL_0025:  add
    IL_0026:  stloc.0
    IL_0027:  ldloc.2
    IL_0028:  ldc.i4.1
    IL_0029:  add
    IL_002a:  stloc.2
    .line 14,14 : 6,23 ''
    IL_002b:  ldloc.2
    IL_002c:  ldloc.1
    IL_002d:  ldc.i4.1
    IL_002e:  add
    IL_002f:  bne.un.s   IL_0016

    IL_0031:  ret
  } // end of method ForEachOnString01::test2

  .method public static void  test3() cil managed
  {
    // 
    .maxstack  5
    .locals init ([0] int32 z,
             [1] int32 V_1,
             [2] int32 V_2,
             [3] char x)
    .line 19,19 : 6,23 ''
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    .line 20,20 : 6,20 ''
    IL_0003:  ldc.i4.0
    IL_0004:  stloc.2
    IL_0005:  ldstr      "123"
    IL_000a:  callvirt   instance int32 [mscorlib]System.String::get_Length()
    IL_000f:  ldc.i4.1
    IL_0010:  sub
    IL_0011:  stloc.1
    IL_0012:  ldloc.1
    IL_0013:  ldloc.2
    IL_0014:  blt.s      IL_0031

    .line 21,21 : 10,26 ''
    IL_0016:  ldstr      "123"
    IL_001b:  ldloc.2
    IL_001c:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0021:  stloc.3
    IL_0022:  ldloc.0
    IL_0023:  ldloc.3
    IL_0024:  conv.i4
    IL_0025:  add
    IL_0026:  stloc.0
    IL_0027:  ldloc.2
    IL_0028:  ldc.i4.1
    IL_0029:  add
    IL_002a:  stloc.2
    .line 20,20 : 6,20 ''
    IL_002b:  ldloc.2
    IL_002c:  ldloc.1
    IL_002d:  ldc.i4.1
    IL_002e:  add
    IL_002f:  bne.un.s   IL_0016

    IL_0031:  ret
  } // end of method ForEachOnString01::test3

  .method public static void  test4() cil managed
  {
    // 
    .maxstack  5
    .locals init ([0] int32 z,
             [1] int32 V_1,
             [2] int32 V_2,
             [3] char x)
    .line 24,24 : 6,23 ''
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    .line 26,26 : 6,20 ''
    IL_0003:  ldc.i4.0
    IL_0004:  stloc.2
    IL_0005:  ldstr      "123"
    IL_000a:  callvirt   instance int32 [mscorlib]System.String::get_Length()
    IL_000f:  ldc.i4.1
    IL_0010:  sub
    IL_0011:  stloc.1
    IL_0012:  ldloc.1
    IL_0013:  ldloc.2
    IL_0014:  blt.s      IL_0031

    .line 27,27 : 10,26 ''
    IL_0016:  ldstr      "123"
    IL_001b:  ldloc.2
    IL_001c:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0021:  stloc.3
    IL_0022:  ldloc.0
    IL_0023:  ldloc.3
    IL_0024:  conv.i4
    IL_0025:  add
    IL_0026:  stloc.0
    IL_0027:  ldloc.2
    IL_0028:  ldc.i4.1
    IL_0029:  add
    IL_002a:  stloc.2
    .line 26,26 : 6,20 ''
    IL_002b:  ldloc.2
    IL_002c:  ldloc.1
    IL_002d:  ldc.i4.1
    IL_002e:  add
    IL_002f:  bne.un.s   IL_0016

    IL_0031:  ret
  } // end of method ForEachOnString01::test4

  .method public static void  test5() cil managed
  {
    // 
    .maxstack  5
    .locals init ([0] int32 V_0,
             [1] int32 V_1,
             [2] char x,
             [3] class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_3)
    .line 31,31 : 6,20 ''
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.1
    IL_0003:  ldstr      "123"
    IL_0008:  callvirt   instance int32 [mscorlib]System.String::get_Length()
    IL_000d:  ldc.i4.1
    IL_000e:  sub
    IL_000f:  stloc.0
    IL_0010:  ldloc.0
    IL_0011:  ldloc.1
    IL_0012:  blt.s      IL_0047

    .line 32,32 : 10,24 ''
    IL_0014:  ldstr      "123"
    IL_0019:  ldloc.1
    IL_001a:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_001f:  stloc.2
    IL_0020:  ldstr      "%A"
    IL_0025:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,char>::.ctor(string)
    IL_002a:  stloc.3
    IL_002b:  call       class [mscorlib]System.IO.TextWriter [mscorlib]System.Console::get_Out()
    IL_0030:  ldloc.3
    IL_0031:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatLineToTextWriter<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [mscorlib]System.IO.TextWriter,
                                                                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0036:  ldloc.2
    IL_0037:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_003c:  pop
    IL_003d:  ldloc.1
    IL_003e:  ldc.i4.1
    IL_003f:  add
    IL_0040:  stloc.1
    .line 31,31 : 6,20 ''
    IL_0041:  ldloc.1
    IL_0042:  ldloc.0
    IL_0043:  ldc.i4.1
    IL_0044:  add
    IL_0045:  bne.un.s   IL_0014

    IL_0047:  ret
  } // end of method ForEachOnString01::test5

  .method public static void  test6(string str) cil managed
  {
    // 
    .maxstack  5
    .locals init ([0] int32 z,
             [1] int32 V_1,
             [2] int32 V_2,
             [3] char x)
    .line 40,40 : 6,23 ''
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    .line 41,41 : 6,21 ''
    IL_0003:  ldc.i4.0
    IL_0004:  stloc.2
    IL_0005:  ldarg.0
    IL_0006:  callvirt   instance int32 [mscorlib]System.String::get_Length()
    IL_000b:  ldc.i4.1
    IL_000c:  sub
    IL_000d:  stloc.1
    IL_000e:  ldloc.1
    IL_000f:  ldloc.2
    IL_0010:  blt.s      IL_0029

    .line 42,42 : 10,26 ''
    IL_0012:  ldarg.0
    IL_0013:  ldloc.2
    IL_0014:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0019:  stloc.3
    IL_001a:  ldloc.0
    IL_001b:  ldloc.3
    IL_001c:  conv.i4
    IL_001d:  add
    IL_001e:  stloc.0
    IL_001f:  ldloc.2
    IL_0020:  ldc.i4.1
    IL_0021:  add
    IL_0022:  stloc.2
    .line 41,41 : 6,21 ''
    IL_0023:  ldloc.2
    IL_0024:  ldloc.1
    IL_0025:  ldc.i4.1
    IL_0026:  add
    IL_0027:  bne.un.s   IL_0012

    IL_0029:  ret
  } // end of method ForEachOnString01::test6

  .method public static void  test7() cil managed
  {
    // 
    .maxstack  5
    .locals init ([0] int32 z,
             [1] int32 V_1,
             [2] int32 V_2,
             [3] char x)
    .line 46,46 : 6,23 ''
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    .line 47,47 : 6,20 ''
    IL_0003:  ldc.i4.0
    IL_0004:  stloc.2
    IL_0005:  ldstr      "123"
    IL_000a:  callvirt   instance int32 [mscorlib]System.String::get_Length()
    IL_000f:  ldc.i4.1
    IL_0010:  sub
    IL_0011:  stloc.1
    IL_0012:  ldloc.1
    IL_0013:  ldloc.2
    IL_0014:  blt.s      IL_0031

    .line 48,48 : 10,26 ''
    IL_0016:  ldstr      "123"
    IL_001b:  ldloc.2
    IL_001c:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0021:  stloc.3
    IL_0022:  ldloc.0
    IL_0023:  ldloc.3
    IL_0024:  conv.i4
    IL_0025:  add
    IL_0026:  stloc.0
    IL_0027:  ldloc.2
    IL_0028:  ldc.i4.1
    IL_0029:  add
    IL_002a:  stloc.2
    .line 47,47 : 6,20 ''
    IL_002b:  ldloc.2
    IL_002c:  ldloc.1
    IL_002d:  ldc.i4.1
    IL_002e:  add
    IL_002f:  bne.un.s   IL_0016

    IL_0031:  ret
  } // end of method ForEachOnString01::test7

  .method public static void  test8() cil managed
  {
    // 
    .maxstack  5
    .locals init ([0] string V_0,
             [1] int32 V_1,
             [2] int32 V_2,
             [3] char i,
             [4] class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_4)
    .line 53,55 : 17,40 ''
    IL_0000:  nop
    IL_0001:  newobj     instance void ForEachOnString01/test8@54::.ctor()
    IL_0006:  ldstr      "1234"
    IL_000b:  call       string [FSharp.Core]Microsoft.FSharp.Core.StringModule::Map(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,char>,
                                                                                     string)
    IL_0010:  stloc.0
    .line 52,56 : 5,21 ''
    IL_0011:  ldc.i4.0
    IL_0012:  stloc.2
    IL_0013:  ldloc.0
    IL_0014:  callvirt   instance int32 [mscorlib]System.String::get_Length()
    IL_0019:  ldc.i4.1
    IL_001a:  sub
    IL_001b:  stloc.1
    IL_001c:  ldloc.1
    IL_001d:  ldloc.2
    IL_001e:  blt.s      IL_0051

    .line 57,57 : 9,23 ''
    IL_0020:  ldloc.0
    IL_0021:  ldloc.2
    IL_0022:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0027:  stloc.3
    IL_0028:  ldstr      "%O"
    IL_002d:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,char>::.ctor(string)
    IL_0032:  stloc.s    V_4
    IL_0034:  call       class [mscorlib]System.IO.TextWriter [mscorlib]System.Console::get_Out()
    IL_0039:  ldloc.s    V_4
    IL_003b:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatLineToTextWriter<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [mscorlib]System.IO.TextWriter,
                                                                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0040:  ldloc.3
    IL_0041:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0046:  pop
    IL_0047:  ldloc.2
    IL_0048:  ldc.i4.1
    IL_0049:  add
    IL_004a:  stloc.2
    .line 52,56 : 5,21 ''
    IL_004b:  ldloc.2
    IL_004c:  ldloc.1
    IL_004d:  ldc.i4.1
    IL_004e:  add
    IL_004f:  bne.un.s   IL_0020

    IL_0051:  ret
  } // end of method ForEachOnString01::test8

  .method public static void  test9() cil managed
  {
    // 
    .maxstack  5
    .locals init ([0] string V_0,
             [1] int32 V_1,
             [2] int32 V_2,
             [3] char i,
             [4] string tmp,
             [5] class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_5)
    .line 62,64 : 17,40 ''
    IL_0000:  nop
    IL_0001:  newobj     instance void ForEachOnString01/test9@63::.ctor()
    IL_0006:  ldstr      "1234"
    IL_000b:  call       string [FSharp.Core]Microsoft.FSharp.Core.StringModule::Map(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,char>,
                                                                                     string)
    IL_0010:  stloc.0
    .line 61,65 : 5,21 ''
    IL_0011:  ldc.i4.0
    IL_0012:  stloc.2
    IL_0013:  ldloc.0
    IL_0014:  callvirt   instance int32 [mscorlib]System.String::get_Length()
    IL_0019:  ldc.i4.1
    IL_001a:  sub
    IL_001b:  stloc.1
    IL_001c:  ldloc.1
    IL_001d:  ldloc.2
    IL_001e:  blt.s      IL_0064

    .line 66,66 : 13,16 ''
    IL_0020:  ldloc.0
    IL_0021:  ldloc.2
    IL_0022:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0027:  stloc.3
    .line 66,66 : 9,53 ''
    IL_0028:  ldstr      "{0} foo"
    IL_002d:  ldloc.3
    IL_002e:  box        [mscorlib]System.Char
    IL_0033:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_0038:  stloc.s    tmp
    .line 67,67 : 9,21 ''
    IL_003a:  ldstr      "%O"
    IL_003f:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string>::.ctor(string)
    IL_0044:  stloc.s    V_5
    IL_0046:  call       class [mscorlib]System.IO.TextWriter [mscorlib]System.Console::get_Out()
    IL_004b:  ldloc.s    V_5
    IL_004d:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatLineToTextWriter<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [mscorlib]System.IO.TextWriter,
                                                                                                                                                                                                                       class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0052:  ldloc.s    tmp
    IL_0054:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0059:  pop
    IL_005a:  ldloc.2
    IL_005b:  ldc.i4.1
    IL_005c:  add
    IL_005d:  stloc.2
    .line 61,65 : 5,21 ''
    IL_005e:  ldloc.2
    IL_005f:  ldloc.1
    IL_0060:  ldc.i4.1
    IL_0061:  add
    IL_0062:  bne.un.s   IL_0020

    IL_0064:  ret
  } // end of method ForEachOnString01::test9

} // end of class ForEachOnString01

.class private abstract auto ansi sealed '<StartupCode$ForEachOnString01>'.$ForEachOnString01
       extends [mscorlib]System.Object
{
} // end of class '<StartupCode$ForEachOnString01>'.$ForEachOnString01


// =============================================================

// 
