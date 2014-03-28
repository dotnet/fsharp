//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.18020
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
.assembly SteppingMatch09
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.SteppingMatch09
{
  // Offset: 0x00000000 Length: 0x000001ED
}
.mresource public FSharpOptimizationData.SteppingMatch09
{
  // Offset: 0x000001F8 Length: 0x0000007A
}
.module SteppingMatch09.dll
// MVID: {515BDBE9-4935-D6AC-A745-0383E9DB5B51}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00E10000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed SteppingMatch09
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<int32> 
          funcA(int32 n) cil managed
  {
    // Code size       41 (0x29)
    .maxstack  8
    .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
    .line 5,5 : 9,21 
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.1
    IL_0003:  sub
    IL_0004:  switch     ( 
                          IL_0013,
                          IL_0015)
    IL_0011:  br.s       IL_0021

    IL_0013:  br.s       IL_0017

    IL_0015:  br.s       IL_001f

    .line 7,7 : 13,21 
    IL_0017:  ldc.i4.s   10
    IL_0019:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<!0> class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<int32>::Some(!0)
    IL_001e:  ret

    .line 9,9 : 13,17 
    IL_001f:  ldnull
    IL_0020:  ret

    .line 11,11 : 20,34 
    IL_0021:  ldc.i4.s   22
    IL_0023:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<!0> class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<int32>::Some(!0)
    IL_0028:  ret
  } // end of method SteppingMatch09::funcA

} // end of class SteppingMatch09

.class private abstract auto ansi sealed '<StartupCode$SteppingMatch09>'.$SteppingMatch09
       extends [mscorlib]System.Object
{
} // end of class '<StartupCode$SteppingMatch09>'.$SteppingMatch09


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
