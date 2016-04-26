
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.33440
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
.assembly TupleElimination
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.TupleElimination
{
  // Offset: 0x00000000 Length: 0x0000022E
}
.mresource public FSharpOptimizationData.TupleElimination
{
  // Offset: 0x00000238 Length: 0x0000007B
}
.module TupleElimination.exe
// MVID: {551307BF-DFDD-92DF-A745-0383BF071355}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00460000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed TupleElimination
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method assembly static void  p@5<a>(!!a v) cil managed
  {
    // Code size       31 (0x1f)
    .maxstack  4
    .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!a,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_0)
    .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
    .line 5,5 : 15,27 ''
    IL_0000:  nop
    IL_0001:  ldstr      "%A"
    IL_0006:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!a,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,!!a>::.ctor(string)
    IL_000b:  stloc.0
    IL_000c:  call       class [mscorlib]System.IO.TextWriter [mscorlib]System.Console::get_Out()
    IL_0011:  ldloc.0
    IL_0012:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatLineToTextWriter<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [mscorlib]System.IO.TextWriter,
                                                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0017:  ldarg.0
    IL_0018:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!a,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_001d:  pop
    IL_001e:  ret
  } // end of method TupleElimination::p@5

  .method public static int32  main(string[] argv) cil managed
  {
    .entrypoint
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.EntryPointAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       101 (0x65)
    .maxstack  5
    .locals init ([0] class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32> dic,
             [1] int32 V_1,
             [2] bool V_2,
             [3] int32 V_3,
             [4] int64 V_4,
             [5] bool V_5,
             [6] int64 V_6,
             [7] class [mscorlib]System.Tuple`2<bool,int64> t)
    .line 7,7 : 5,64 ''
    IL_0000:  nop
    IL_0001:  newobj     instance void class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>::.ctor()
    IL_0006:  stloc.0
    .line 9,9 : 31,48 ''
    IL_0007:  ldc.i4.0
    IL_0008:  stloc.1
    IL_0009:  ldloc.0
    IL_000a:  ldc.i4.1
    IL_000b:  ldloca.s   V_1
    IL_000d:  callvirt   instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>::TryGetValue(!0,
                                                                                                                         !1&)
    IL_0012:  stloc.2
    IL_0013:  ldloc.1
    IL_0014:  stloc.3
    .line 10,10 : 5,6 ''
    IL_0015:  ldloc.2
    IL_0016:  call       void TupleElimination::p@5<bool>(!!0)
    IL_001b:  nop
    .line 11,11 : 5,6 ''
    IL_001c:  ldloc.3
    IL_001d:  call       void TupleElimination::p@5<int32>(!!0)
    IL_0022:  nop
    .line 14,14 : 38,65 ''
    IL_0023:  ldc.i8     0x0
    IL_002c:  stloc.s    V_4
    IL_002e:  ldstr      "123"
    IL_0033:  ldloca.s   V_4
    IL_0035:  call       bool [mscorlib]System.Int64::TryParse(string,
                                                               int64&)
    IL_003a:  stloc.s    V_5
    IL_003c:  ldloc.s    V_4
    IL_003e:  stloc.s    V_6
    .line 14,14 : 5,65 ''
    IL_0040:  ldloc.s    V_5
    IL_0042:  ldloc.s    V_6
    IL_0044:  newobj     instance void class [mscorlib]System.Tuple`2<bool,int64>::.ctor(!0,
                                                                                         !1)
    IL_0049:  stloc.s    t
    .line 15,15 : 5,6 ''
    IL_004b:  ldloc.s    V_5
    IL_004d:  call       void TupleElimination::p@5<bool>(!!0)
    IL_0052:  nop
    .line 16,16 : 5,6 ''
    IL_0053:  ldloc.s    V_6
    IL_0055:  call       void TupleElimination::p@5<int64>(!!0)
    IL_005a:  nop
    .line 21,21 : 5,6 ''
    IL_005b:  ldloc.s    t
    IL_005d:  call       void TupleElimination::p@5<class [mscorlib]System.Tuple`2<bool,int64>>(!!0)
    IL_0062:  nop
    .line 23,23 : 5,6 ''
    IL_0063:  ldc.i4.0
    IL_0064:  ret
  } // end of method TupleElimination::main

} // end of class TupleElimination

.class private abstract auto ansi sealed '<StartupCode$TupleElimination>'.$TupleElimination
       extends [mscorlib]System.Object
{
} // end of class '<StartupCode$TupleElimination>'.$TupleElimination


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
