
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.6.81.0
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
.assembly SteppingMatch03
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 00 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.SteppingMatch03
{
  // Offset: 0x00000000 Length: 0x00000231
}
.mresource public FSharpOptimizationData.SteppingMatch03
{
  // Offset: 0x00000238 Length: 0x0000007A
}
.module SteppingMatch03.dll
// MVID: {57570D0A-4E87-D110-A745-03830A0D5757}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x02A70000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed SteppingMatch03
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static void  funcC<a,b,c>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`3<!!a,!!b,!!c> n) cil managed
  {
    // Code size       82 (0x52)
    .maxstack  3
    .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`3<!!a,!!b,!!c> V_0,
             [1] class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`3<!!a,!!b,!!c> V_1,
             [2] class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`3/Choice1Of3<!!a,!!b,!!c> V_2,
             [3] class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`3/Choice2Of3<!!a,!!b,!!c> V_3,
             [4] class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`3/Choice3Of3<!!a,!!b,!!c> V_4)
    .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
    .line 5,5 : 9,21 'C:\\GitHub\\dsyme\\visualfsharp\\tests\\fsharpqa\\Source\\CodeGen\\EmittedIL\\SteppingMatch\\SteppingMatch03.fs'
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldloc.0
    IL_0004:  stloc.1
    IL_0005:  ldloc.1
    IL_0006:  isinst     class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`3/Choice2Of3<!!a,!!b,!!c>
    IL_000b:  brtrue.s   IL_0017

    IL_000d:  ldloc.1
    IL_000e:  isinst     class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`3/Choice3Of3<!!a,!!b,!!c>
    IL_0013:  brtrue.s   IL_0019

    IL_0015:  br.s       IL_001b

    IL_0017:  br.s       IL_002d

    IL_0019:  br.s       IL_003f

    IL_001b:  ldloc.0
    IL_001c:  castclass  class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`3/Choice1Of3<!!a,!!b,!!c>
    IL_0021:  stloc.2
    .line 7,7 : 13,35 ''
    IL_0022:  ldstr      "A"
    IL_0027:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_002c:  ret

    .line 5,5 : 9,21 ''
    IL_002d:  ldloc.0
    IL_002e:  castclass  class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`3/Choice2Of3<!!a,!!b,!!c>
    IL_0033:  stloc.3
    .line 9,9 : 13,35 ''
    IL_0034:  ldstr      "B"
    IL_0039:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_003e:  ret

    .line 5,5 : 9,21 ''
    IL_003f:  ldloc.0
    IL_0040:  castclass  class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`3/Choice3Of3<!!a,!!b,!!c>
    IL_0045:  stloc.s    V_4
    .line 11,11 : 13,35 ''
    IL_0047:  ldstr      "C"
    IL_004c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0051:  ret
  } // end of method SteppingMatch03::funcC

} // end of class SteppingMatch03

.class private abstract auto ansi sealed '<StartupCode$SteppingMatch03>'.$SteppingMatch03
       extends [mscorlib]System.Object
{
} // end of class '<StartupCode$SteppingMatch03>'.$SteppingMatch03


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
