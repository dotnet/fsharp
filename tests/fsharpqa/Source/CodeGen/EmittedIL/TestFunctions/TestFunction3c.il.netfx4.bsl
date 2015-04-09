
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.33440
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
  .ver 4:4:0:9055
}
.assembly TestFunction3c
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.TestFunction3c
{
  // Offset: 0x00000000 Length: 0x000001E6
}
.mresource public FSharpOptimizationData.TestFunction3c
{
  // Offset: 0x000001F0 Length: 0x0000008A
}
.module TestFunction3c.exe
// MVID: {550EFC51-A662-4FAC-A745-038351FC0E55}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00930000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed TestFunction3c
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static int32  TestFunction1() cil managed
  {
    // Code size       37 (0x25)
    .maxstack  8
    .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
    .line 5,5 : 5,20 'C:\\visualfsharp\\tests\\fsharpqa\\Source\\CodeGen\\EmittedIL\\TestFunctions\\TestFunction3c.fs'
    IL_0000:  nop
    IL_0001:  ldstr      "Hello"
    IL_0006:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
    IL_000b:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0010:  pop
    .line 6,6 : 5,20 ''
    IL_0011:  ldstr      "World"
    IL_0016:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
    IL_001b:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0020:  pop
    .line 7,7 : 5,8 ''
    IL_0021:  ldc.i4.3
    IL_0022:  ldc.i4.4
    IL_0023:  add
    IL_0024:  ret
  } // end of method TestFunction3c::TestFunction1

  .method public static void  TestFunction3c() cil managed
  {
    // Code size       161 (0xa1)
    .maxstack  4
    .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Core.Unit V_0,
             [1] int32 x,
             [2] class [mscorlib]System.Exception V_2,
             [3] class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string> V_3,
             [4] string msg,
             [5] string V_5,
             [6] class [mscorlib]System.Exception V_6,
             [7] class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string> V_7,
             [8] string V_8,
             [9] string V_9)
    .line 10,10 : 5,8 ''
    IL_0000:  nop
    .try
    {
      IL_0001:  nop
      .line 11,11 : 8,31 ''
      IL_0002:  call       int32 TestFunction3c::TestFunction1()
      IL_0007:  stloc.1
      .line 12,12 : 8,24 ''
      IL_0008:  ldstr      "hello"
      IL_000d:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::FailWith<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(string)
      IL_0012:  stloc.0
      IL_0013:  leave      IL_009e

      .line 13,13 : 5,9 ''
    }  // end .try
    filter
    {
      IL_0018:  castclass  [mscorlib]System.Exception
      IL_001d:  stloc.2
      IL_001e:  ldloc.2
      IL_001f:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string> [FSharp.Core]Microsoft.FSharp.Core.Operators::FailurePattern(class [mscorlib]System.Exception)
      IL_0024:  stloc.3
      IL_0025:  ldloc.3
      IL_0026:  brfalse.s  IL_004e

      IL_0028:  ldloc.3
      IL_0029:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string>::get_Value()
      IL_002e:  stloc.s    msg
      IL_0030:  ldloc.s    msg
      IL_0032:  ldstr      "hello"
      IL_0037:  call       bool [mscorlib]System.String::Equals(string,
                                                                string)
      IL_003c:  brfalse.s  IL_0040

      IL_003e:  br.s       IL_0042

      IL_0040:  br.s       IL_004e

      .line 100001,100001 : 0,0 ''
      IL_0042:  ldloc.3
      IL_0043:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string>::get_Value()
      IL_0048:  stloc.s    V_5
      IL_004a:  ldc.i4.1
      .line 100001,100001 : 0,0 ''
      IL_004b:  nop
      IL_004c:  br.s       IL_0050

      .line 100001,100001 : 0,0 ''
      IL_004e:  ldc.i4.0
      .line 100001,100001 : 0,0 ''
      IL_004f:  nop
      IL_0050:  endfilter
    }  // end filter
    {  // handler
      IL_0052:  castclass  [mscorlib]System.Exception
      IL_0057:  stloc.s    V_6
      IL_0059:  ldloc.s    V_6
      IL_005b:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string> [FSharp.Core]Microsoft.FSharp.Core.Operators::FailurePattern(class [mscorlib]System.Exception)
      IL_0060:  stloc.s    V_7
      IL_0062:  ldloc.s    V_7
      IL_0064:  brfalse.s  IL_009c

      IL_0066:  ldloc.s    V_7
      IL_0068:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string>::get_Value()
      IL_006d:  stloc.s    V_8
      IL_006f:  ldloc.s    V_8
      IL_0071:  ldstr      "hello"
      IL_0076:  call       bool [mscorlib]System.String::Equals(string,
                                                                string)
      IL_007b:  brfalse.s  IL_007f

      IL_007d:  br.s       IL_0081

      IL_007f:  br.s       IL_009c

      IL_0081:  ldloc.s    V_7
      IL_0083:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string>::get_Value()
      IL_0088:  stloc.s    V_9
      .line 14,14 : 8,23 ''
      IL_008a:  ldstr      "World"
      IL_008f:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
      IL_0094:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
      IL_0099:  stloc.0
      IL_009a:  leave.s    IL_009e

      .line 100001,100001 : 0,0 ''
      IL_009c:  rethrow
      .line 100001,100001 : 0,0 ''
    }  // end handler
    IL_009e:  ldloc.0
    IL_009f:  pop
    IL_00a0:  ret
  } // end of method TestFunction3c::TestFunction3c

} // end of class TestFunction3c

.class private abstract auto ansi sealed '<StartupCode$TestFunction3c>'.$TestFunction3c
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method $TestFunction3c::main@

} // end of class '<StartupCode$TestFunction3c>'.$TestFunction3c


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
