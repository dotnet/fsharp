
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
.assembly TestFunction3b
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.TestFunction3b
{
  // Offset: 0x00000000 Length: 0x000001E6
}
.mresource public FSharpOptimizationData.TestFunction3b
{
  // Offset: 0x000001F0 Length: 0x0000008A
}
.module TestFunction3b.exe
// MVID: {550EFC51-A662-4FC9-A745-038351FC0E55}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00210000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed TestFunction3b
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static int32  TestFunction1() cil managed
  {
    // Code size       37 (0x25)
    .maxstack  8
    .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
    .line 5,5 : 5,20 'C:\\visualfsharp\\tests\\fsharpqa\\Source\\CodeGen\\EmittedIL\\TestFunctions\\TestFunction3b.fs'
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
  } // end of method TestFunction3b::TestFunction1

  .method public static void  TestFunction3b() cil managed
  {
    // Code size       96 (0x60)
    .maxstack  3
    .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Core.Unit V_0,
             [1] int32 x,
             [2] class [mscorlib]System.Exception V_2,
             [3] class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string> V_3,
             [4] class [mscorlib]System.Exception V_4,
             [5] class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string> V_5)
    .line 10,10 : 5,8 ''
    IL_0000:  nop
    .try
    {
      IL_0001:  nop
      .line 11,11 : 8,31 ''
      IL_0002:  call       int32 TestFunction3b::TestFunction1()
      IL_0007:  stloc.1
      .line 12,12 : 8,24 ''
      IL_0008:  ldstr      "hello"
      IL_000d:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::FailWith<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(string)
      IL_0012:  stloc.0
      IL_0013:  leave.s    IL_005d

      .line 13,13 : 5,9 ''
    }  // end .try
    filter
    {
      IL_0015:  castclass  [mscorlib]System.Exception
      IL_001a:  stloc.2
      IL_001b:  ldloc.2
      IL_001c:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string> [FSharp.Core]Microsoft.FSharp.Core.Operators::FailurePattern(class [mscorlib]System.Exception)
      IL_0021:  stloc.3
      IL_0022:  ldloc.3
      IL_0023:  brfalse.s  IL_0027

      IL_0025:  br.s       IL_0029

      IL_0027:  br.s       IL_002d

      .line 100001,100001 : 0,0 ''
      IL_0029:  ldc.i4.1
      .line 100001,100001 : 0,0 ''
      IL_002a:  nop
      IL_002b:  br.s       IL_002f

      .line 100001,100001 : 0,0 ''
      IL_002d:  ldc.i4.0
      .line 100001,100001 : 0,0 ''
      IL_002e:  nop
      IL_002f:  endfilter
    }  // end filter
    {  // handler
      IL_0031:  castclass  [mscorlib]System.Exception
      IL_0036:  stloc.s    V_4
      IL_0038:  ldloc.s    V_4
      IL_003a:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string> [FSharp.Core]Microsoft.FSharp.Core.Operators::FailurePattern(class [mscorlib]System.Exception)
      IL_003f:  stloc.s    V_5
      IL_0041:  ldloc.s    V_5
      IL_0043:  brfalse.s  IL_0047

      IL_0045:  br.s       IL_0049

      IL_0047:  br.s       IL_005b

      .line 14,14 : 8,23 ''
      IL_0049:  ldstr      "World"
      IL_004e:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
      IL_0053:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
      IL_0058:  stloc.0
      IL_0059:  leave.s    IL_005d

      .line 100001,100001 : 0,0 ''
      IL_005b:  rethrow
      .line 100001,100001 : 0,0 ''
    }  // end handler
    IL_005d:  ldloc.0
    IL_005e:  pop
    IL_005f:  ret
  } // end of method TestFunction3b::TestFunction3b

} // end of class TestFunction3b

.class private abstract auto ansi sealed '<StartupCode$TestFunction3b>'.$TestFunction3b
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method $TestFunction3b::main@

} // end of class '<StartupCode$TestFunction3b>'.$TestFunction3b


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
