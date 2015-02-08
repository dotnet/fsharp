
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
.assembly TryWith_FilterBlocks01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.TryWith_FilterBlocks01
{
  // Offset: 0x00000000 Length: 0x00000122
}
.mresource public FSharpOptimizationData.TryWith_FilterBlocks01
{
  // Offset: 0x00000128 Length: 0x0000005D
}
.module TryWith_FilterBlocks01.exe
// MVID: {54D7D809-3732-DEEC-A745-038309D8D754}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x005C0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed TryWith_FilterBlocks01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
} // end of class TryWith_FilterBlocks01

.class private abstract auto ansi sealed '<StartupCode$TryWith_FilterBlocks01>'.$TryWith_FilterBlocks01
       extends [mscorlib]System.Object
{
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       81 (0x51)
    .maxstack  4
    .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Core.Unit V_0,
             [1] class [mscorlib]System.Exception V_1,
             [2] class [mscorlib]System.Exception e,
             [3] class [mscorlib]System.Exception V_3,
             [4] class [mscorlib]System.Exception V_4,
             [5] class [mscorlib]System.Exception V_5,
             [6] class [mscorlib]System.Exception V_6)
    .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
    .line 3,3 : 1,4 
    IL_0000:  nop
    .try
    {
      IL_0001:  nop
      .line 4,4 : 3,5 ''
      IL_0002:  ldnull
      IL_0003:  stloc.0
      IL_0004:  leave.s    IL_004e

      .line 5,5 : 2,6 ''
    }  // end .try
    filter
    {
      IL_0006:  castclass  [mscorlib]System.Exception
      IL_000b:  stloc.1
      IL_000c:  ldloc.1
      IL_000d:  stloc.2
      IL_000e:  ldloc.2
      IL_000f:  callvirt   instance int32 [mscorlib]System.Object::GetHashCode()
      IL_0014:  ldc.i4.0
      IL_0015:  ceq
      IL_0017:  brfalse.s  IL_001b

      IL_0019:  br.s       IL_001d

      IL_001b:  br.s       IL_0023

      .line 100001,100001 : 0,0 ''
      IL_001d:  ldloc.1
      IL_001e:  stloc.3
      IL_001f:  ldc.i4.1
      .line 100001,100001 : 0,0 ''
      IL_0020:  nop
      IL_0021:  br.s       IL_0025

      .line 7,7 : 5,6 ''
      IL_0023:  ldc.i4.1
      .line 100001,100001 : 0,0 ''
      IL_0024:  nop
      IL_0025:  endfilter
    }  // end filter
    {  // handler
      IL_0027:  castclass  [mscorlib]System.Exception
      IL_002c:  stloc.s    V_4
      IL_002e:  ldloc.s    V_4
      IL_0030:  stloc.s    V_5
      IL_0032:  ldloc.s    V_5
      IL_0034:  callvirt   instance int32 [mscorlib]System.Object::GetHashCode()
      IL_0039:  ldc.i4.0
      IL_003a:  ceq
      IL_003c:  brfalse.s  IL_0040

      IL_003e:  br.s       IL_0042

      IL_0040:  br.s       IL_004a

      IL_0042:  ldloc.s    V_4
      IL_0044:  stloc.s    V_6
      .line 6,6 : 35,37 ''
      IL_0046:  ldnull
      IL_0047:  stloc.0
      IL_0048:  leave.s    IL_004e

      .line 7,7 : 10,12 ''
      IL_004a:  ldnull
      IL_004b:  stloc.0
      IL_004c:  leave.s    IL_004e

      .line 100001,100001 : 0,0 ''
    }  // end handler
    IL_004e:  ldloc.0
    IL_004f:  pop
    IL_0050:  ret
  } // end of method $TryWith_FilterBlocks01::main@

} // end of class '<StartupCode$TryWith_FilterBlocks01>'.$TryWith_FilterBlocks01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
