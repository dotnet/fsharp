
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.8.3928.0
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
  .ver 5:0:0:0
}
.assembly LetIfThenElse01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.LetIfThenElse01
{
  // Offset: 0x00000000 Length: 0x000001D9
}
.mresource public FSharpOptimizationData.LetIfThenElse01
{
  // Offset: 0x000001E0 Length: 0x00000076
}
.module LetIfThenElse01.exe
// MVID: {611C4D7C-BE5A-D8FD-A745-03837C4D1C61}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x06920000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed LetIfThenElse01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static class [mscorlib]System.Tuple`4<int32,int32,int32,int32> 
          F<a>(!!a y) cil managed
  {
    // Code size       128 (0x80)
    .maxstack  6
    .locals init ([0] int32 x1,
             [1] valuetype [mscorlib]System.DateTime V_1,
             [2] int32 y1,
             [3] valuetype [mscorlib]System.DateTime V_3,
             [4] int32 x2,
             [5] valuetype [mscorlib]System.DateTime V_5,
             [6] int32 y2,
             [7] valuetype [mscorlib]System.DateTime V_7)
    .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
    .line 6,6 : 12,51 'C:\\GitHub\\dsyme\\fsharp\\tests\\fsharpqa\\source\\CodeGen\\EmittedIL\\Misc\\LetIfThenElse01.fs'
    IL_0000:  nop
    .line 100001,100001 : 0,0 ''
    IL_0001:  call       valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::get_Now()
    IL_0006:  stloc.1
    IL_0007:  ldloca.s   V_1
    IL_0009:  call       instance int32 [mscorlib]System.DateTime::get_Year()
    IL_000e:  ldc.i4     0x7d0
    IL_0013:  ble.s      IL_0019

    .line 6,6 : 52,53 ''
    IL_0015:  ldc.i4.1
    .line 100001,100001 : 0,0 ''
    IL_0016:  nop
    IL_0017:  br.s       IL_001b

    .line 6,6 : 59,60 ''
    IL_0019:  ldc.i4.2
    .line 100001,100001 : 0,0 ''
    IL_001a:  nop
    .line 100001,100001 : 0,0 ''
    IL_001b:  stloc.0
    .line 7,7 : 12,51 ''
    IL_001c:  nop
    .line 100001,100001 : 0,0 ''
    IL_001d:  call       valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::get_Now()
    IL_0022:  stloc.3
    IL_0023:  ldloca.s   V_3
    IL_0025:  call       instance int32 [mscorlib]System.DateTime::get_Year()
    IL_002a:  ldc.i4     0x7d0
    IL_002f:  ble.s      IL_0035

    .line 7,7 : 52,53 ''
    IL_0031:  ldc.i4.1
    .line 100001,100001 : 0,0 ''
    IL_0032:  nop
    IL_0033:  br.s       IL_0037

    .line 7,7 : 59,60 ''
    IL_0035:  ldc.i4.2
    .line 100001,100001 : 0,0 ''
    IL_0036:  nop
    .line 100001,100001 : 0,0 ''
    IL_0037:  stloc.2
    .line 8,8 : 12,51 ''
    IL_0038:  nop
    .line 100001,100001 : 0,0 ''
    IL_0039:  call       valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::get_Now()
    IL_003e:  stloc.s    V_5
    IL_0040:  ldloca.s   V_5
    IL_0042:  call       instance int32 [mscorlib]System.DateTime::get_Year()
    IL_0047:  ldc.i4     0x7d0
    IL_004c:  bge.s      IL_0052

    .line 8,8 : 52,53 ''
    IL_004e:  ldc.i4.1
    .line 100001,100001 : 0,0 ''
    IL_004f:  nop
    IL_0050:  br.s       IL_0054

    .line 8,8 : 59,60 ''
    IL_0052:  ldc.i4.2
    .line 100001,100001 : 0,0 ''
    IL_0053:  nop
    .line 100001,100001 : 0,0 ''
    IL_0054:  stloc.s    x2
    .line 9,9 : 12,51 ''
    IL_0056:  nop
    .line 100001,100001 : 0,0 ''
    IL_0057:  call       valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::get_Now()
    IL_005c:  stloc.s    V_7
    IL_005e:  ldloca.s   V_7
    IL_0060:  call       instance int32 [mscorlib]System.DateTime::get_Year()
    IL_0065:  ldc.i4     0x7d0
    IL_006a:  bge.s      IL_0070

    .line 9,9 : 52,53 ''
    IL_006c:  ldc.i4.1
    .line 100001,100001 : 0,0 ''
    IL_006d:  nop
    IL_006e:  br.s       IL_0072

    .line 9,9 : 59,60 ''
    IL_0070:  ldc.i4.2
    .line 100001,100001 : 0,0 ''
    IL_0071:  nop
    .line 100001,100001 : 0,0 ''
    IL_0072:  stloc.s    y2
    .line 10,10 : 3,14 ''
    IL_0074:  ldloc.0
    IL_0075:  ldloc.2
    IL_0076:  ldloc.s    x2
    IL_0078:  ldloc.s    y2
    IL_007a:  newobj     instance void class [mscorlib]System.Tuple`4<int32,int32,int32,int32>::.ctor(!0,
                                                                                                      !1,
                                                                                                      !2,
                                                                                                      !3)
    IL_007f:  ret
  } // end of method LetIfThenElse01::F

} // end of class LetIfThenElse01

.class private abstract auto ansi sealed '<StartupCode$LetIfThenElse01>'.$LetIfThenElse01
       extends [mscorlib]System.Object
{
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       10 (0xa)
    .maxstack  3
    .locals init ([0] class [mscorlib]System.Tuple`4<int32,int32,int32,int32> 'Pipe #1 input at line 12',
             [1] class [mscorlib]System.Tuple`4<int32,int32,int32,int32> V_1)
    .line 12,12 : 1,5 ''
    IL_0000:  ldc.i4.1
    IL_0001:  call       class [mscorlib]System.Tuple`4<int32,int32,int32,int32> LetIfThenElse01::F<int32>(!!0)
    IL_0006:  stloc.0
    .line 12,12 : 9,15 ''
    IL_0007:  ldloc.0
    IL_0008:  stloc.1
    IL_0009:  ret
  } // end of method $LetIfThenElse01::main@

} // end of class '<StartupCode$LetIfThenElse01>'.$LetIfThenElse01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
