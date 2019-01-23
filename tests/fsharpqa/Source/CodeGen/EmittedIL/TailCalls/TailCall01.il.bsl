
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
.assembly TailCall01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.TailCall01
{
  // Offset: 0x00000000 Length: 0x0000021D
}
.mresource public FSharpSignatureDataB.TailCall01
{
  // Offset: 0x00000228 Length: 0x00000007
}
.mresource public FSharpOptimizationData.TailCall01
{
  // Offset: 0x00000238 Length: 0x0000007C
}
.mresource public FSharpOptimizationDataB.TailCall01
{
  // Offset: 0x000002B8 Length: 0x00000000
}
.module TailCall01.exe
// MVID: {5BF2DEA9-7D8F-CF4A-A745-0383A9DEF25B}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00D00000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed TailCall01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static void  foo<a>(int32 x,
                                     !!a y) cil managed
  {
    // Code size       27 (0x1b)
    .maxstack  4
    .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_0,
             [1] int32 V_1)
    .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
    .line 3,3 : 21,33 'C:\\GitHub\\dsyme\\visualfsharp\\tests\\fsharpqa\\Source\\CodeGen\\EmittedIL\\TailCalls\\TailCall01.fs'
    IL_0000:  ldstr      "%d"
    IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::.ctor(string)
    IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_000f:  stloc.0
    IL_0010:  ldarg.0
    IL_0011:  stloc.1
    IL_0012:  ldloc.0
    IL_0013:  ldloc.1
    IL_0014:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0019:  pop
    IL_001a:  ret
  } // end of method TailCall01::foo

  .method public static void  run() cil managed
  {
    // Code size       12 (0xc)
    .maxstack  4
    .locals init ([0] int32 x)
    .line 4,4 : 13,22 ''
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    .line 4,4 : 26,34 ''
    IL_0002:  ldloc.0
    IL_0003:  ldc.i4.5
    IL_0004:  tail.
    IL_0006:  call       void TailCall01::foo<int32>(int32,
                                                     !!0)
    IL_000b:  ret
  } // end of method TailCall01::run

} // end of class TailCall01

.class private abstract auto ansi sealed '<StartupCode$TailCall01>'.$TailCall01
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $TailCall01::main@

} // end of class '<StartupCode$TailCall01>'.$TailCall01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
