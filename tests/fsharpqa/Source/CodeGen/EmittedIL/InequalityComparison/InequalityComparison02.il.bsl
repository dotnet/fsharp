
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
  .ver 4:7:0:0
}
.assembly InequalityComparison02
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.InequalityComparison02
{
  // Offset: 0x00000000 Length: 0x00000224
}
.mresource public FSharpOptimizationData.InequalityComparison02
{
  // Offset: 0x00000228 Length: 0x00000085
}
.module InequalityComparison02.exe
// MVID: {5EE40368-263A-E72C-A745-03836803E45E}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00B80000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed InequalityComparison02
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static bool  f2(int32 x,
                                 int32 y) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    // Code size       24 (0x18)
    .maxstack  4
    .locals init ([0] int32 V_0,
             [1] int32 V_1,
             [2] int32 V_2,
             [3] int32 V_3,
             [4] int32 V_4,
             [5] int32 V_5)
    .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
    .line 3,3 : 27,33 'C:\\Users\\phcart\\source\\repos\\fsharp\\tests\\fsharpqa\\source\\CodeGen\\EmittedIL\\InequalityComparison\\InequalityComparison02.fs'
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.1
    IL_0003:  stloc.1
    IL_0004:  ldloc.0
    IL_0005:  stloc.2
    IL_0006:  ldloc.1
    IL_0007:  stloc.3
    IL_0008:  ldloc.2
    IL_0009:  stloc.s    V_4
    IL_000b:  ldloc.3
    IL_000c:  stloc.s    V_5
    IL_000e:  ldloc.s    V_4
    IL_0010:  ldloc.s    V_5
    IL_0012:  clt
    IL_0014:  ldc.i4.0
    IL_0015:  ceq
    IL_0017:  ret
  } // end of method InequalityComparison02::f2

} // end of class InequalityComparison02

.class private abstract auto ansi sealed '<StartupCode$InequalityComparison02>'.$InequalityComparison02
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $InequalityComparison02::main@

} // end of class '<StartupCode$InequalityComparison02>'.$InequalityComparison02


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
