
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.17014
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
  .ver 4:3:0:0
}
.assembly TailCall05
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.TailCall05
{
  // Offset: 0x00000000 Length: 0x00000263
}
.mresource public FSharpOptimizationData.TailCall05
{
  // Offset: 0x00000268 Length: 0x0000007C
}
.module TailCall05.exe
// MVID: {4E2CF067-7D8F-CFC6-A745-038367F02C4E}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x0000000000450000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed TailCall05
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static void  foo<a>(int32& x,
                                     int32& y,
                                     !!a z) cil managed
  {
    // Code size       40 (0x28)
    .maxstack  4
    .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_0,
             [1] int32 V_1)
    .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
    .line 3,3 : 40,58 
    IL_0000:  ldstr      "%d"
    IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::.ctor(string)
    IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_000f:  stloc.0
    IL_0010:  nop
    IL_0011:  ldarg.0
    IL_0012:  ldobj      [mscorlib]System.Int32
    IL_0017:  ldarg.1
    IL_0018:  ldobj      [mscorlib]System.Int32
    IL_001d:  add
    IL_001e:  stloc.1
    IL_001f:  ldloc.0
    IL_0020:  ldloc.1
    IL_0021:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0026:  pop
    IL_0027:  ret
  } // end of method TailCall05::foo

  .method public static void  run() cil managed
  {
    // Code size       15 (0xf)
    .maxstack  5
    .locals init ([0] int32 x)
    .line 4,4 : 13,30 
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    .line 4,4 : 34,46 
    IL_0003:  ldloca.s   x
    IL_0005:  ldloca.s   x
    IL_0007:  ldc.i4.5
    IL_0008:  call       void TailCall05::foo<int32>(int32&,
                                                     int32&,
                                                     !!0)
    IL_000d:  nop
    IL_000e:  ret
  } // end of method TailCall05::run

} // end of class TailCall05

.class private abstract auto ansi sealed '<StartupCode$TailCall05>'.$TailCall05
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method $TailCall05::main@

} // end of class '<StartupCode$TailCall05>'.$TailCall05


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
