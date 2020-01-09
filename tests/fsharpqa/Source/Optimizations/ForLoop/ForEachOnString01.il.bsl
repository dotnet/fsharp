
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.7.3081.0
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
  .ver 4:7:0:0
}
.assembly ForEachOnString01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.ForEachOnString01
{
  // Offset: 0x00000000 Length: 0x00000350
}
.mresource public FSharpSignatureDataB.ForEachOnString01
{
  // Offset: 0x00000358 Length: 0x0000001B
}
.mresource public FSharpOptimizationData.ForEachOnString01
{
  // Offset: 0x00000378 Length: 0x000000FF
}
.module ForEachOnString01.dll
// MVID: {5E171CA0-105C-852B-A745-0383A01C175E}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x05AC0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed ForEachOnString01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable sealed nested assembly beforefieldinit test8@54
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,char>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,char>::.ctor()
      IL_0006:  ret
    } // end of method test8@54::.ctor

    .method public strict virtual instance char 
            Invoke(char x) cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 55,55 : 21,39 'C:\\GitHub\\dsyme\\fsharp\\tests\\fsharpqa\\source\\Optimizations\\ForLoop\\ForEachOnString01.fs'
      IL_0000:  ldarg.1
      IL_0001:  conv.i4
      IL_0002:  ldc.i4.1
      IL_0003:  add
      IL_0004:  conv.u2
      IL_0005:  ret
    } // end of method test8@54::Invoke

  } // end of class test8@54

  .class auto ansi serializable sealed nested assembly beforefieldinit test9@63
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,char>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,char>::.ctor()
      IL_0006:  ret
    } // end of method test9@63::.ctor

    .method public strict virtual instance char 
            Invoke(char x) cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      .line 64,64 : 21,39 ''
      IL_0000:  ldarg.1
      IL_0001:  conv.i4
      IL_0002:  ldc.i4.1
      IL_0003:  add
      IL_0004:  conv.u2
      IL_0005:  ret
    } // end of method test9@63::Invoke

  } // end of class test9@63

  .method public static void  test1(string str) cil managed
  {
    // Code size       41 (0x29)
    .maxstack  5
    .locals init ([0] int32 z,
             [1] int32 V_1,
             [2] int32 V_2,
             [3] char x)
    .line 8,8 : 6,23 ''
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    .line 9,9 : 6,21 ''
    IL_0002:  ldc.i4.0
    IL_0003:  stloc.2
    IL_0004:  ldarg.0
    IL_0005:  callvirt   instance int32 [mscorlib]System.String::get_Length()
    IL_000a:  ldc.i4.1
    IL_000b:  sub
    IL_000c:  stloc.1
    IL_000d:  ldloc.1
    IL_000e:  ldloc.2
    IL_000f:  blt.s      IL_0028

    .line 9,9 : 6,21 ''
    IL_0011:  ldarg.0
    IL_0012:  ldloc.2
    IL_0013:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0018:  stloc.3
    IL_0019:  ldloc.0
    IL_001a:  ldloc.3
    IL_001b:  conv.i4
    IL_001c:  add
    IL_001d:  stloc.0
    IL_001e:  ldloc.2
    IL_001f:  ldc.i4.1
    IL_0020:  add
    IL_0021:  stloc.2
    .line 9,9 : 6,21 ''
    IL_0022:  ldloc.2
    IL_0023:  ldloc.1
    IL_0024:  ldc.i4.1
    IL_0025:  add
    IL_0026:  bne.un.s   IL_0011

    IL_0028:  ret
  } // end of method ForEachOnString01::test1

  .method public static void  test2() cil managed
  {
    // Code size       49 (0x31)
    .maxstack  5
    .locals init ([0] int32 z,
             [1] int32 V_1,
             [2] int32 V_2,
             [3] char x)
    .line 13,13 : 6,23 ''
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    .line 14,14 : 6,23 ''
    IL_0002:  ldc.i4.0
    IL_0003:  stloc.2
    IL_0004:  ldstr      "123"
    IL_0009:  callvirt   instance int32 [mscorlib]System.String::get_Length()
    IL_000e:  ldc.i4.1
    IL_000f:  sub
    IL_0010:  stloc.1
    IL_0011:  ldloc.1
    IL_0012:  ldloc.2
    IL_0013:  blt.s      IL_0030

    .line 14,14 : 6,23 ''
    IL_0015:  ldstr      "123"
    IL_001a:  ldloc.2
    IL_001b:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0020:  stloc.3
    IL_0021:  ldloc.0
    IL_0022:  ldloc.3
    IL_0023:  conv.i4
    IL_0024:  add
    IL_0025:  stloc.0
    IL_0026:  ldloc.2
    IL_0027:  ldc.i4.1
    IL_0028:  add
    IL_0029:  stloc.2
    .line 14,14 : 6,23 ''
    IL_002a:  ldloc.2
    IL_002b:  ldloc.1
    IL_002c:  ldc.i4.1
    IL_002d:  add
    IL_002e:  bne.un.s   IL_0015

    IL_0030:  ret
  } // end of method ForEachOnString01::test2

  .method public static void  test3() cil managed
  {
    // Code size       49 (0x31)
    .maxstack  5
    .locals init ([0] int32 z,
             [1] int32 V_1,
             [2] int32 V_2,
             [3] char x)
    .line 19,19 : 6,23 ''
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    .line 20,20 : 6,20 ''
    IL_0002:  ldc.i4.0
    IL_0003:  stloc.2
    IL_0004:  ldstr      "123"
    IL_0009:  callvirt   instance int32 [mscorlib]System.String::get_Length()
    IL_000e:  ldc.i4.1
    IL_000f:  sub
    IL_0010:  stloc.1
    IL_0011:  ldloc.1
    IL_0012:  ldloc.2
    IL_0013:  blt.s      IL_0030

    .line 20,20 : 6,20 ''
    IL_0015:  ldstr      "123"
    IL_001a:  ldloc.2
    IL_001b:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0020:  stloc.3
    IL_0021:  ldloc.0
    IL_0022:  ldloc.3
    IL_0023:  conv.i4
    IL_0024:  add
    IL_0025:  stloc.0
    IL_0026:  ldloc.2
    IL_0027:  ldc.i4.1
    IL_0028:  add
    IL_0029:  stloc.2
    .line 20,20 : 6,20 ''
    IL_002a:  ldloc.2
    IL_002b:  ldloc.1
    IL_002c:  ldc.i4.1
    IL_002d:  add
    IL_002e:  bne.un.s   IL_0015

    IL_0030:  ret
  } // end of method ForEachOnString01::test3

  .method public static void  test4() cil managed
  {
    // Code size       49 (0x31)
    .maxstack  5
    .locals init ([0] int32 z,
             [1] int32 V_1,
             [2] int32 V_2,
             [3] char x)
    .line 24,24 : 6,23 ''
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    .line 26,26 : 6,20 ''
    IL_0002:  ldc.i4.0
    IL_0003:  stloc.2
    IL_0004:  ldstr      "123"
    IL_0009:  callvirt   instance int32 [mscorlib]System.String::get_Length()
    IL_000e:  ldc.i4.1
    IL_000f:  sub
    IL_0010:  stloc.1
    IL_0011:  ldloc.1
    IL_0012:  ldloc.2
    IL_0013:  blt.s      IL_0030

    .line 26,26 : 6,20 ''
    IL_0015:  ldstr      "123"
    IL_001a:  ldloc.2
    IL_001b:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0020:  stloc.3
    IL_0021:  ldloc.0
    IL_0022:  ldloc.3
    IL_0023:  conv.i4
    IL_0024:  add
    IL_0025:  stloc.0
    IL_0026:  ldloc.2
    IL_0027:  ldc.i4.1
    IL_0028:  add
    IL_0029:  stloc.2
    .line 26,26 : 6,20 ''
    IL_002a:  ldloc.2
    IL_002b:  ldloc.1
    IL_002c:  ldc.i4.1
    IL_002d:  add
    IL_002e:  bne.un.s   IL_0015

    IL_0030:  ret
  } // end of method ForEachOnString01::test4

  .method public static void  test5() cil managed
  {
    // Code size       64 (0x40)
    .maxstack  5
    .locals init ([0] int32 V_0,
             [1] int32 V_1,
             [2] char x)
    .line 31,31 : 6,20 ''
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.1
    IL_0002:  ldstr      "123"
    IL_0007:  callvirt   instance int32 [mscorlib]System.String::get_Length()
    IL_000c:  ldc.i4.1
    IL_000d:  sub
    IL_000e:  stloc.0
    IL_000f:  ldloc.0
    IL_0010:  ldloc.1
    IL_0011:  blt.s      IL_003f

    .line 31,31 : 6,20 ''
    IL_0013:  ldstr      "123"
    IL_0018:  ldloc.1
    IL_0019:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_001e:  stloc.2
    IL_001f:  ldstr      "%A"
    IL_0024:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,char>::.ctor(string)
    IL_0029:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_002e:  ldloc.2
    IL_002f:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0034:  pop
    IL_0035:  ldloc.1
    IL_0036:  ldc.i4.1
    IL_0037:  add
    IL_0038:  stloc.1
    .line 31,31 : 6,20 ''
    IL_0039:  ldloc.1
    IL_003a:  ldloc.0
    IL_003b:  ldc.i4.1
    IL_003c:  add
    IL_003d:  bne.un.s   IL_0013

    IL_003f:  ret
  } // end of method ForEachOnString01::test5

  .method public static void  test6(string str) cil managed
  {
    // Code size       41 (0x29)
    .maxstack  5
    .locals init ([0] int32 z,
             [1] int32 V_1,
             [2] int32 V_2,
             [3] char x)
    .line 40,40 : 6,23 ''
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    .line 41,41 : 6,21 ''
    IL_0002:  ldc.i4.0
    IL_0003:  stloc.2
    IL_0004:  ldarg.0
    IL_0005:  callvirt   instance int32 [mscorlib]System.String::get_Length()
    IL_000a:  ldc.i4.1
    IL_000b:  sub
    IL_000c:  stloc.1
    IL_000d:  ldloc.1
    IL_000e:  ldloc.2
    IL_000f:  blt.s      IL_0028

    .line 41,41 : 6,21 ''
    IL_0011:  ldarg.0
    IL_0012:  ldloc.2
    IL_0013:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0018:  stloc.3
    IL_0019:  ldloc.0
    IL_001a:  ldloc.3
    IL_001b:  conv.i4
    IL_001c:  add
    IL_001d:  stloc.0
    IL_001e:  ldloc.2
    IL_001f:  ldc.i4.1
    IL_0020:  add
    IL_0021:  stloc.2
    .line 41,41 : 6,21 ''
    IL_0022:  ldloc.2
    IL_0023:  ldloc.1
    IL_0024:  ldc.i4.1
    IL_0025:  add
    IL_0026:  bne.un.s   IL_0011

    IL_0028:  ret
  } // end of method ForEachOnString01::test6

  .method public static void  test7() cil managed
  {
    // Code size       49 (0x31)
    .maxstack  5
    .locals init ([0] int32 z,
             [1] int32 V_1,
             [2] int32 V_2,
             [3] char x)
    .line 46,46 : 6,23 ''
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    .line 47,47 : 6,20 ''
    IL_0002:  ldc.i4.0
    IL_0003:  stloc.2
    IL_0004:  ldstr      "123"
    IL_0009:  callvirt   instance int32 [mscorlib]System.String::get_Length()
    IL_000e:  ldc.i4.1
    IL_000f:  sub
    IL_0010:  stloc.1
    IL_0011:  ldloc.1
    IL_0012:  ldloc.2
    IL_0013:  blt.s      IL_0030

    .line 47,47 : 6,20 ''
    IL_0015:  ldstr      "123"
    IL_001a:  ldloc.2
    IL_001b:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0020:  stloc.3
    IL_0021:  ldloc.0
    IL_0022:  ldloc.3
    IL_0023:  conv.i4
    IL_0024:  add
    IL_0025:  stloc.0
    IL_0026:  ldloc.2
    IL_0027:  ldc.i4.1
    IL_0028:  add
    IL_0029:  stloc.2
    .line 47,47 : 6,20 ''
    IL_002a:  ldloc.2
    IL_002b:  ldloc.1
    IL_002c:  ldc.i4.1
    IL_002d:  add
    IL_002e:  bne.un.s   IL_0015

    IL_0030:  ret
  } // end of method ForEachOnString01::test7

  .method public static void  test8() cil managed
  {
    // Code size       72 (0x48)
    .maxstack  5
    .locals init ([0] string V_0,
             [1] int32 V_1,
             [2] int32 V_2,
             [3] char i)
    .line 53,55 : 17,40 ''
    IL_0000:  newobj     instance void ForEachOnString01/test8@54::.ctor()
    IL_0005:  ldstr      "1234"
    IL_000a:  call       string [FSharp.Core]Microsoft.FSharp.Core.StringModule::Map(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,char>,
                                                                                     string)
    IL_000f:  stloc.0
    .line 52,56 : 5,21 ''
    IL_0010:  ldc.i4.0
    IL_0011:  stloc.2
    IL_0012:  ldloc.0
    IL_0013:  callvirt   instance int32 [mscorlib]System.String::get_Length()
    IL_0018:  ldc.i4.1
    IL_0019:  sub
    IL_001a:  stloc.1
    IL_001b:  ldloc.1
    IL_001c:  ldloc.2
    IL_001d:  blt.s      IL_0047

    .line 52,56 : 5,21 ''
    IL_001f:  ldloc.0
    IL_0020:  ldloc.2
    IL_0021:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0026:  stloc.3
    IL_0027:  ldstr      "%O"
    IL_002c:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,char>::.ctor(string)
    IL_0031:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0036:  ldloc.3
    IL_0037:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_003c:  pop
    IL_003d:  ldloc.2
    IL_003e:  ldc.i4.1
    IL_003f:  add
    IL_0040:  stloc.2
    .line 52,56 : 5,21 ''
    IL_0041:  ldloc.2
    IL_0042:  ldloc.1
    IL_0043:  ldc.i4.1
    IL_0044:  add
    IL_0045:  bne.un.s   IL_001f

    IL_0047:  ret
  } // end of method ForEachOnString01::test8

  .method public static void  test9() cil managed
  {
    // Code size       91 (0x5b)
    .maxstack  5
    .locals init ([0] string V_0,
             [1] int32 V_1,
             [2] int32 V_2,
             [3] char i,
             [4] string tmp)
    .line 62,64 : 17,40 ''
    IL_0000:  newobj     instance void ForEachOnString01/test9@63::.ctor()
    IL_0005:  ldstr      "1234"
    IL_000a:  call       string [FSharp.Core]Microsoft.FSharp.Core.StringModule::Map(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,char>,
                                                                                     string)
    IL_000f:  stloc.0
    .line 61,65 : 5,21 ''
    IL_0010:  ldc.i4.0
    IL_0011:  stloc.2
    IL_0012:  ldloc.0
    IL_0013:  callvirt   instance int32 [mscorlib]System.String::get_Length()
    IL_0018:  ldc.i4.1
    IL_0019:  sub
    IL_001a:  stloc.1
    IL_001b:  ldloc.1
    IL_001c:  ldloc.2
    IL_001d:  blt.s      IL_005a

    .line 61,65 : 5,21 ''
    IL_001f:  ldloc.0
    IL_0020:  ldloc.2
    IL_0021:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0026:  stloc.3
    .line 66,66 : 9,53 ''
    IL_0027:  ldstr      "{0} foo"
    IL_002c:  ldloc.3
    IL_002d:  box        [mscorlib]System.Char
    IL_0032:  call       string [mscorlib]System.String::Format(string,
                                                                object)
    IL_0037:  stloc.s    tmp
    .line 67,67 : 9,21 ''
    IL_0039:  ldstr      "%O"
    IL_003e:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string>::.ctor(string)
    IL_0043:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0048:  ldloc.s    tmp
    IL_004a:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_004f:  pop
    IL_0050:  ldloc.2
    IL_0051:  ldc.i4.1
    IL_0052:  add
    IL_0053:  stloc.2
    .line 61,65 : 5,21 ''
    IL_0054:  ldloc.2
    IL_0055:  ldloc.1
    IL_0056:  ldc.i4.1
    IL_0057:  add
    IL_0058:  bne.un.s   IL_001f

    IL_005a:  ret
  } // end of method ForEachOnString01::test9

} // end of class ForEachOnString01

.class private abstract auto ansi sealed '<StartupCode$ForEachOnString01>'.$ForEachOnString01
       extends [mscorlib]System.Object
{
} // end of class '<StartupCode$ForEachOnString01>'.$ForEachOnString01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
