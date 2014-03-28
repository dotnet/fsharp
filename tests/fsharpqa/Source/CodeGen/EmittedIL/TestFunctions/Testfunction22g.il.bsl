
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
.assembly Testfunction22g
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Testfunction22g
{
  // Offset: 0x00000000 Length: 0x00000181
}
.mresource public FSharpOptimizationData.Testfunction22g
{
  // Offset: 0x00000188 Length: 0x00000056
}
.module Testfunction22g.exe
// MVID: {4DAC30FD-CA89-74DB-A745-0383FD30AC4D}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x0000000000180000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Testfunction22g
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
} // end of class Testfunction22g

.class private abstract auto ansi sealed '<StartupCode$Testfunction22g>'.$Testfunction22g
       extends [mscorlib]System.Object
{
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       23 (0x17)
    .maxstack  8
    .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
    .line 3,3 : 1,13 
    IL_0000:  nop
    IL_0001:  ldc.i4.1
    IL_0002:  brfalse.s  IL_0006

    IL_0004:  br.s       IL_0008

    IL_0006:  br.s       IL_0010

    .line 3,3 : 14,40 
    IL_0008:  call       void [mscorlib]System.Console::WriteLine()
    .line 100001,100001 : 0,0 
    IL_000d:  nop
    IL_000e:  br.s       IL_0016

    .line 3,3 : 46,72 
    IL_0010:  call       void [mscorlib]System.Console::WriteLine()
    .line 100001,100001 : 0,0 
    IL_0015:  nop
    IL_0016:  ret
  } // end of method $Testfunction22g::main@

} // end of class '<StartupCode$Testfunction22g>'.$Testfunction22g


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
