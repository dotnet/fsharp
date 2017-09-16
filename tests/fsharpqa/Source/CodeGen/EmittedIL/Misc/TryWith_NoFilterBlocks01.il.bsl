
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
  .ver 4:4:1:0
}
.assembly TryWith_NoFilterBlocks01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.TryWith_NoFilterBlocks01
{
  // Offset: 0x00000000 Length: 0x0000015D
}
.mresource public FSharpOptimizationData.TryWith_NoFilterBlocks01
{
  // Offset: 0x00000168 Length: 0x0000005F
}
.module TryWith_NoFilterBlocks01.exe
// MVID: {59B19213-3DEF-9A40-A745-03831392B159}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x017A0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed TryWith_NoFilterBlocks01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
} // end of class TryWith_NoFilterBlocks01

.class private abstract auto ansi sealed '<StartupCode$TryWith_NoFilterBlocks01>'.$TryWith_NoFilterBlocks01
       extends [mscorlib]System.Object
{
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       40 (0x28)
    .maxstack  4
    .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Core.Unit V_0,
             [1] class [mscorlib]System.Exception V_1,
             [2] class [mscorlib]System.Exception e,
             [3] class [mscorlib]System.Exception V_3)
    .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
    .line 4,4 : 3,5 'C:\\GitHub\\dsyme\\visualfsharp\\tests\\fsharpqa\\Source\\CodeGen\\EmittedIL\\Misc\\TryWith_NoFilterBlocks01.fs'
    .try
    {
      IL_0000:  ldnull
      IL_0001:  stloc.0
      IL_0002:  leave.s    IL_0025

      .line 5,5 : 2,6 ''
    }  // end .try
    catch [mscorlib]System.Object 
    {
      IL_0004:  castclass  [mscorlib]System.Exception
      IL_0009:  stloc.1
      IL_000a:  ldloc.1
      IL_000b:  stloc.2
      IL_000c:  ldloc.2
      IL_000d:  callvirt   instance int32 [mscorlib]System.Object::GetHashCode()
      IL_0012:  ldc.i4.0
      IL_0013:  ceq
      IL_0015:  brfalse.s  IL_0019

      IL_0017:  br.s       IL_001b

      IL_0019:  br.s       IL_0021

      IL_001b:  ldloc.1
      IL_001c:  stloc.3
      .line 6,6 : 35,37 ''
      IL_001d:  ldnull
      IL_001e:  stloc.0
      IL_001f:  leave.s    IL_0025

      .line 7,7 : 10,12 ''
      IL_0021:  ldnull
      IL_0022:  stloc.0
      IL_0023:  leave.s    IL_0025

      .line 100001,100001 : 0,0 ''
    }  // end handler
    IL_0025:  ldloc.0
    IL_0026:  pop
    IL_0027:  ret
  } // end of method $TryWith_NoFilterBlocks01::main@

} // end of class '<StartupCode$TryWith_NoFilterBlocks01>'.$TryWith_NoFilterBlocks01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
