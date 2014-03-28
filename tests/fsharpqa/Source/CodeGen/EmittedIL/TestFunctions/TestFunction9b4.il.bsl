
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
.assembly TestFunction9b4
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.TestFunction9b4
{
  // Offset: 0x00000000 Length: 0x00000270
}
.mresource public FSharpOptimizationData.TestFunction9b4
{
  // Offset: 0x00000278 Length: 0x00000085
}
.module TestFunction9b4.exe
// MVID: {4DAC30E3-A091-56C1-A745-0383E330AC4D}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x0000000000470000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed TestFunction9b4
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static !!a  Null<class a>() cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.LiteralAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       11 (0xb)
    .maxstack  3
    .locals init ([0] !!a V_0)
    .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
    .line 8,8 : 12,16 
    IL_0000:  nop
    IL_0001:  ldloca.s   V_0
    IL_0003:  initobj    !!a
    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method TestFunction9b4::Null

  .method public specialname static int32 
          get_x() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       3 (0x3)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldc.i4.5
    IL_0002:  ret
  } // end of method TestFunction9b4::get_x

  .property int32 x()
  {
    .get int32 TestFunction9b4::get_x()
  } // end of property TestFunction9b4::x
} // end of class TestFunction9b4

.class private abstract auto ansi sealed '<StartupCode$TestFunction9b4>'.$TestFunction9b4
       extends [mscorlib]System.Object
{
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       42 (0x2a)
    .maxstack  3
    .locals init ([0] int32 x)
    .line 10,10 : 1,10 
    IL_0000:  nop
    IL_0001:  call       int32 TestFunction9b4::get_x()
    IL_0006:  stloc.0
    .line 12,12 : 1,17 
    IL_0007:  call       int32 TestFunction9b4::get_x()
    IL_000c:  box        [mscorlib]System.Int32
    IL_0011:  brfalse.s  IL_0015

    IL_0013:  br.s       IL_0028

    .line 13,13 : 13,30 
    IL_0015:  ldstr      "Is null"
    IL_001a:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
    IL_001f:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0024:  pop
    .line 100001,100001 : 0,0 
    IL_0025:  nop
    IL_0026:  br.s       IL_0029

    .line 14,14 : 10,12 
    .line 100001,100001 : 0,0 
    IL_0028:  nop
    IL_0029:  ret
  } // end of method $TestFunction9b4::main@

} // end of class '<StartupCode$TestFunction9b4>'.$TestFunction9b4


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
