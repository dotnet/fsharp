
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
.assembly LetIfThenElse01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 00 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.LetIfThenElse01
{
  // Offset: 0x00000000 Length: 0x000001E5
}
.mresource public FSharpOptimizationData.LetIfThenElse01
{
  // Offset: 0x000001F0 Length: 0x00000076
}
.module LetIfThenElse01.exe
// MVID: {5775B177-BE5A-D8FD-A745-038377B17557}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x02660000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed LetIfThenElse01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static class [mscorlib]System.Tuple`4<int32,int32,int32,int32> 
          F<a>(!!a y) cil managed
  {
    // Code size       141 (0x8d)
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
    .line 6,6 : 12,51 'C:\\GitHub\\dsyme\\visualfsharp\\tests\\fsharpqa\\Source\\CodeGen\\EmittedIL\\Misc\\LetIfThenElse01.fs'
    IL_0000:  nop
    IL_0001:  call       valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::get_Now()
    IL_0006:  stloc.1
    IL_0007:  ldloca.s   V_1
    IL_0009:  call       instance int32 [mscorlib]System.DateTime::get_Year()
    IL_000e:  ldc.i4     0x7d0
    IL_0013:  ble.s      IL_0017

    IL_0015:  br.s       IL_0019

    IL_0017:  br.s       IL_001d

    .line 6,6 : 52,53
    IL_0019:  ldc.i4.1
    .line 100001,100001 : 0,0
    IL_001a:  nop
    IL_001b:  br.s       IL_001f

    .line 6,6 : 59,60
    IL_001d:  ldc.i4.2
    .line 100001,100001 : 0,0
    IL_001e:  nop
    .line 100001,100001 : 0,0
    IL_001f:  stloc.0
    .line 7,7 : 12,51
    IL_0020:  call       valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::get_Now()
    IL_0025:  stloc.3
    IL_0026:  ldloca.s   V_3
    IL_0028:  call       instance int32 [mscorlib]System.DateTime::get_Year()
    IL_002d:  ldc.i4     0x7d0
    IL_0032:  ble.s      IL_0036

    IL_0034:  br.s       IL_0038

    IL_0036:  br.s       IL_003c

    .line 7,7 : 52,53
    IL_0038:  ldc.i4.1
    .line 100001,100001 : 0,0
    IL_0039:  nop
    IL_003a:  br.s       IL_003e

    .line 7,7 : 59,60
    IL_003c:  ldc.i4.2
    .line 100001,100001 : 0,0
    IL_003d:  nop
    .line 100001,100001 : 0,0
    IL_003e:  stloc.2
    .line 8,8 : 12,51
    IL_003f:  call       valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::get_Now()
    IL_0044:  stloc.s    V_5
    IL_0046:  ldloca.s   V_5
    IL_0048:  call       instance int32 [mscorlib]System.DateTime::get_Year()
    IL_004d:  ldc.i4     0x7d0
    IL_0052:  bge.s      IL_0056

    IL_0054:  br.s       IL_0058

    IL_0056:  br.s       IL_005c

    .line 8,8 : 52,53
    IL_0058:  ldc.i4.1
    .line 100001,100001 : 0,0
    IL_0059:  nop
    IL_005a:  br.s       IL_005e

    .line 8,8 : 59,60
    IL_005c:  ldc.i4.2
    .line 100001,100001 : 0,0
    IL_005d:  nop
    .line 100001,100001 : 0,0
    IL_005e:  stloc.s    x2
    .line 9,9 : 12,51
    IL_0060:  call       valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::get_Now()
    IL_0065:  stloc.s    V_7
    IL_0067:  ldloca.s   V_7
    IL_0069:  call       instance int32 [mscorlib]System.DateTime::get_Year()
    IL_006e:  ldc.i4     0x7d0
    IL_0073:  bge.s      IL_0077

    IL_0075:  br.s       IL_0079

    IL_0077:  br.s       IL_007d

    .line 9,9 : 52,53
    IL_0079:  ldc.i4.1
    .line 100001,100001 : 0,0
    IL_007a:  nop
    IL_007b:  br.s       IL_007f

    .line 9,9 : 59,60
    IL_007d:  ldc.i4.2
    .line 100001,100001 : 0,0
    IL_007e:  nop
    .line 100001,100001 : 0,0
    IL_007f:  stloc.s    y2
    .line 10,10 : 3,14
    IL_0081:  ldloc.0
    IL_0082:  ldloc.2
    IL_0083:  ldloc.s    x2
    IL_0085:  ldloc.s    y2
    IL_0087:  newobj     instance void class [mscorlib]System.Tuple`4<int32,int32,int32,int32>::.ctor(!0,
                                                                                                      !1,
                                                                                                      !2,
                                                                                                      !3)
    IL_008c:  ret
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
    // Code size       54 (0x36)
    .maxstack  6
    .locals init ([0] class [mscorlib]System.Tuple`4<int32,int32,int32,int32> V_0,
             [1] class [mscorlib]System.Tuple`4<int32,int32,int32,int32> V_1,
             [2] int32 V_2,
             [3] int32 V_3,
             [4] int32 V_4,
             [5] int32 V_5,
             [6] class [mscorlib]System.Tuple`4<int32,int32,int32,int32> V_6)
    .line 12,12 : 1,15
    IL_0000:  nop
    IL_0001:  ldc.i4.1
    IL_0002:  call       class [mscorlib]System.Tuple`4<int32,int32,int32,int32> LetIfThenElse01::F<int32>(!!0)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  stloc.1
    IL_000a:  ldloc.1
    IL_000b:  call       instance !0 class [mscorlib]System.Tuple`4<int32,int32,int32,int32>::get_Item1()
    IL_0010:  stloc.2
    IL_0011:  ldloc.1
    IL_0012:  call       instance !1 class [mscorlib]System.Tuple`4<int32,int32,int32,int32>::get_Item2()
    IL_0017:  stloc.3
    IL_0018:  ldloc.1
    IL_0019:  call       instance !2 class [mscorlib]System.Tuple`4<int32,int32,int32,int32>::get_Item3()
    IL_001e:  stloc.s    V_4
    IL_0020:  ldloc.1
    IL_0021:  call       instance !3 class [mscorlib]System.Tuple`4<int32,int32,int32,int32>::get_Item4()
    IL_0026:  stloc.s    V_5
    IL_0028:  ldloc.2
    IL_0029:  ldloc.3
    IL_002a:  ldloc.s    V_4
    IL_002c:  ldloc.s    V_5
    IL_002e:  newobj     instance void class [mscorlib]System.Tuple`4<int32,int32,int32,int32>::.ctor(!0,
                                                                                                      !1,
                                                                                                      !2,
                                                                                                      !3)
    IL_0033:  stloc.s    V_6
    IL_0035:  ret
  } // end of method $LetIfThenElse01::main@

} // end of class '<StartupCode$LetIfThenElse01>'.$LetIfThenElse01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
