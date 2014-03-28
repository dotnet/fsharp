
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
.assembly SteppingMatch05
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.SteppingMatch05
{
  // Offset: 0x00000000 Length: 0x00000256
}
.mresource public FSharpOptimizationData.SteppingMatch05
{
  // Offset: 0x00000260 Length: 0x0000007B
}
.module SteppingMatch05.dll
// MVID: {4DAC14D5-30E9-4ADA-A745-0383D514AC4D}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x0000000000270000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed SteppingMatch05
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static void  funcC3<a,b,c>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`3<!!a,!!b,!!c> n) cil managed
  {
    // Code size       84 (0x54)
    .maxstack  3
    .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`3<!!a,!!b,!!c> V_0,
             [1] class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`3/Choice3Of3<!!a,!!b,!!c> V_1,
             [2] class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`3/Choice2Of3<!!a,!!b,!!c> V_2,
             [3] class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`3/Choice1Of3<!!a,!!b,!!c> V_3,
             [4] class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`3<!!a,!!b,!!c> V_4)
    .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
    .line 5,5 : 9,21 
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldloc.0
    IL_0004:  stloc.s    V_4
    IL_0006:  ldloc.s    V_4
    IL_0008:  isinst     class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`3/Choice2Of3<!!a,!!b,!!c>
    IL_000d:  brtrue.s   IL_001a

    IL_000f:  ldloc.s    V_4
    IL_0011:  isinst     class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`3/Choice1Of3<!!a,!!b,!!c>
    IL_0016:  brtrue.s   IL_001c

    IL_0018:  br.s       IL_001e

    IL_001a:  br.s       IL_0030

    IL_001c:  br.s       IL_0042

    IL_001e:  ldloc.0
    IL_001f:  castclass  class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`3/Choice3Of3<!!a,!!b,!!c>
    IL_0024:  stloc.1
    .line 7,7 : 13,35 
    IL_0025:  ldstr      "C"
    IL_002a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_002f:  ret

    .line 5,5 : 9,21 
    IL_0030:  ldloc.0
    IL_0031:  castclass  class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`3/Choice2Of3<!!a,!!b,!!c>
    IL_0036:  stloc.2
    .line 9,9 : 13,35 
    IL_0037:  ldstr      "B"
    IL_003c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0041:  ret

    .line 5,5 : 9,21 
    IL_0042:  ldloc.0
    IL_0043:  castclass  class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`3/Choice1Of3<!!a,!!b,!!c>
    IL_0048:  stloc.3
    .line 11,11 : 13,35 
    IL_0049:  ldstr      "A"
    IL_004e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0053:  ret
  } // end of method SteppingMatch05::funcC3

} // end of class SteppingMatch05

.class private abstract auto ansi sealed '<StartupCode$SteppingMatch05>'.$SteppingMatch05
       extends [mscorlib]System.Object
{
} // end of class '<StartupCode$SteppingMatch05>'.$SteppingMatch05


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
