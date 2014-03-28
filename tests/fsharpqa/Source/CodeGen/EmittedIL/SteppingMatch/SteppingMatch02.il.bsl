
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.16774
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
  .ver 4:0:0:0
}
.assembly SteppingMatch02
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.SteppingMatch02
{
  // Offset: 0x00000000 Length: 0x00000240
}
.mresource public FSharpOptimizationData.SteppingMatch02
{
  // Offset: 0x00000248 Length: 0x0000007A
}
.module SteppingMatch02.dll
// MVID: {4DAC14CE-CAC2-C63D-A745-0383CE14AC4D}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x0000000000270000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed SteppingMatch02
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static void  funcB<a,b>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`2<!!a,!!b> n) cil managed
  {
    // Code size       49 (0x31)
    .maxstack  3
    .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`2<!!a,!!b> V_0,
             [1] class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`2/Choice1Of2<!!a,!!b> V_1,
             [2] class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`2/Choice2Of2<!!a,!!b> V_2)
    .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
    .line 5,5 : 9,21 
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldloc.0
    IL_0004:  isinst     class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`2/Choice2Of2<!!a,!!b>
    IL_0009:  brfalse.s  IL_000d

    IL_000b:  br.s       IL_001f

    IL_000d:  ldloc.0
    IL_000e:  castclass  class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`2/Choice1Of2<!!a,!!b>
    IL_0013:  stloc.1
    .line 7,7 : 13,35 
    IL_0014:  ldstr      "B"
    IL_0019:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_001e:  ret

    .line 5,5 : 9,21 
    IL_001f:  ldloc.0
    IL_0020:  castclass  class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`2/Choice2Of2<!!a,!!b>
    IL_0025:  stloc.2
    .line 9,9 : 13,35 
    IL_0026:  ldstr      "A"
    IL_002b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0030:  ret
  } // end of method SteppingMatch02::funcB

} // end of class SteppingMatch02

.class private abstract auto ansi sealed '<StartupCode$SteppingMatch02>'.$SteppingMatch02
       extends [mscorlib]System.Object
{
} // end of class '<StartupCode$SteppingMatch02>'.$SteppingMatch02


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
