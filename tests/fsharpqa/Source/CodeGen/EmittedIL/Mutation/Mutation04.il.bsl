
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
.assembly Mutation04
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Mutation04
{
  // Offset: 0x00000000 Length: 0x000001D1
}
.mresource public FSharpOptimizationData.Mutation04
{
  // Offset: 0x000001D8 Length: 0x0000006C
}
.module Mutation04.exe
// MVID: {4DAC143B-8C6A-2E43-A745-03833B14AC4D}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x0000000000780000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Mutation04
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public specialname static valuetype [mscorlib]System.Decimal 
          get_x() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     valuetype [mscorlib]System.Decimal '<StartupCode$Mutation04>'.$Mutation04::x@4
    IL_0005:  ret
  } // end of method Mutation04::get_x

  .property valuetype [mscorlib]System.Decimal
          x()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get valuetype [mscorlib]System.Decimal Mutation04::get_x()
  } // end of property Mutation04::x
} // end of class Mutation04

.class private abstract auto ansi sealed '<StartupCode$Mutation04>'.$Mutation04
       extends [mscorlib]System.Object
{
  .field static assembly valuetype [mscorlib]System.Decimal x@4
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       34 (0x22)
    .maxstack  4
    .locals init ([0] valuetype [mscorlib]System.Decimal x,
             [1] valuetype [mscorlib]System.Decimal V_1)
    .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
    .line 4,4 : 1,32 
    IL_0000:  nop
    IL_0001:  ldsfld     valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::MaxValue
    IL_0006:  dup
    IL_0007:  stsfld     valuetype [mscorlib]System.Decimal '<StartupCode$Mutation04>'.$Mutation04::x@4
    IL_000c:  stloc.0
    .line 5,5 : 1,13 
    IL_000d:  call       valuetype [mscorlib]System.Decimal Mutation04::get_x()
    IL_0012:  stloc.1
    IL_0013:  ldloca.s   V_1
    IL_0015:  constrained. [mscorlib]System.Decimal
    IL_001b:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0020:  pop
    IL_0021:  ret
  } // end of method $Mutation04::main@

} // end of class '<StartupCode$Mutation04>'.$Mutation04


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
