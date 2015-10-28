
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.17014
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
.assembly NoIEnumerable03
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 )

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 00 00 00 00 00 )

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.NoIEnumerable03
{
  // Offset: 0x00000000 Length: 0x00000203
}
.mresource public FSharpOptimizationData.NoIEnumerable03
{
  // Offset: 0x00000208 Length: 0x0000006C
}
.module NoIEnumerable03.dll
// MVID: {4E1B9C7F-7903-6020-A745-03837F9C1B4E}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x0000000000860000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed M
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 )
  .method public static void  loop3(int32 a,
                                    int32 N) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 )
    // Code size       43 (0x2b)
    .maxstack  5
    .locals init ([0] int32 V_0,
             [1] int32 i,
             [2] class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_2)
    .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
    .line 7,7 : 4,22
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.1
    IL_0003:  ldarg.1
    IL_0004:  stloc.0
    IL_0005:  ldloc.0
    IL_0006:  ldloc.1
    IL_0007:  blt.s      IL_002a

    .line 8,8 : 7,20
    IL_0009:  ldstr      "aaa"
    IL_000e:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
    IL_0013:  stloc.2
    IL_0014:  call       class [mscorlib]System.IO.TextWriter [mscorlib]System.Console::get_Out()
    IL_0019:  ldloc.2
    IL_001a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatLineToTextWriter<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [mscorlib]System.IO.TextWriter,
                                                                                                                                                         class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_001f:  pop
    IL_0020:  ldloc.1
    IL_0021:  ldc.i4.1
    IL_0022:  add
    IL_0023:  stloc.1
    .line 7,7 : 18,19
    IL_0024:  ldloc.1
    IL_0025:  ldloc.0
    IL_0026:  ldc.i4.1
    IL_0027:  add
    IL_0028:  bne.un.s   IL_0009

    IL_002a:  ret
  } // end of method M::loop3

} // end of class M

.class private abstract auto ansi sealed '<StartupCode$NoIEnumerable03>'.$M$fsx
       extends [mscorlib]System.Object
{
} // end of class '<StartupCode$NoIEnumerable03>'.$M$fsx


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
