
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.6.1055.0
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
  .ver 4:5:0:0
}
.assembly Testfunction22f
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Testfunction22f
{
  // Offset: 0x00000000 Length: 0x00000165
}
.mresource public FSharpSignatureDataB.Testfunction22f
{
  // Offset: 0x00000170 Length: 0x00000000
}
.mresource public FSharpOptimizationData.Testfunction22f
{
  // Offset: 0x00000178 Length: 0x00000056
}
.mresource public FSharpOptimizationDataB.Testfunction22f
{
  // Offset: 0x000001D8 Length: 0x00000000
}
.module Testfunction22f.exe
// MVID: {5C6C932B-634A-F08B-A745-03832B936C5C}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00F20000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Testfunction22f
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
} // end of class Testfunction22f

.class private abstract auto ansi sealed '<StartupCode$Testfunction22f>'.$Testfunction22f
       extends [mscorlib]System.Object
{
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       38 (0x26)
    .maxstack  4
    .locals init ([0] string V_0)
    .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
    .line 3,3 : 1,15 'C:\\GitHub\\dsyme\\visualfsharp\\tests\\fsharpqa\\Source\\CodeGen\\EmittedIL\\TestFunctions\\Testfunction22f.fs'
    IL_0000:  ldstr      "A"
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldstr      "A"
    IL_000c:  call       bool [mscorlib]System.String::Equals(string,
                                                              string)
    IL_0011:  brfalse.s  IL_0015

    IL_0013:  br.s       IL_0017

    IL_0015:  br.s       IL_001f

    .line 4,4 : 12,38 ''
    IL_0017:  call       void [mscorlib]System.Console::WriteLine()
    .line 100001,100001 : 0,0 ''
    IL_001c:  nop
    IL_001d:  br.s       IL_0025

    .line 5,5 : 10,36 ''
    IL_001f:  call       void [mscorlib]System.Console::WriteLine()
    .line 100001,100001 : 0,0 ''
    IL_0024:  nop
    IL_0025:  ret
  } // end of method $Testfunction22f::main@

} // end of class '<StartupCode$Testfunction22f>'.$Testfunction22f


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
