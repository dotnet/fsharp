
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
.assembly extern netstandard
{
  .publickeytoken = (CC 7B 13 FF CD 2D DD 51 )                         // .{...-.Q
  .ver 2:0:0:0
}
.assembly TupleElimination
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.TupleElimination
{
  // Offset: 0x00000000 Length: 0x0000022A
}
.mresource public FSharpOptimizationData.TupleElimination
{
  // Offset: 0x00000230 Length: 0x0000007B
}
.module TupleElimination.exe
// MVID: {608C0510-DFDD-92DF-A745-038310058C60}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x06D10000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed TupleElimination
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static int32  main(string[] argv) cil managed
  {
    .entrypoint
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.EntryPointAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       197 (0xc5)
    .maxstack  5
    .locals init ([0] class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32> dic,
             [1] int32 V_1,
             [2] bool V_2,
             [3] class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_3,
             [4] class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_4,
             [5] int64 V_5,
             [6] bool V_6,
             [7] class [mscorlib]System.Tuple`2<bool,int64> t,
             [8] class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_8,
             [9] class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<bool,int64>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_9)
    .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
    .line 7,7 : 5,64 'C:\\GitHub\\dsyme\\fsharp\\tests\\fsharpqa\\source\\CodeGen\\EmittedIL\\Tuples\\TupleElimination.fs'
    IL_0000:  newobj     instance void class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>::.ctor()
    IL_0005:  stloc.0
    .line 9,9 : 31,48 ''
    IL_0006:  ldloc.0
    IL_0007:  ldc.i4.1
    IL_0008:  ldloca.s   V_1
    IL_000a:  callvirt   instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>::TryGetValue(!0,
                                                                                                                         !1&)
    IL_000f:  stloc.2
    .line 10,10 : 5,6 ''
    IL_0010:  ldstr      "%A"
    IL_0015:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,bool>::.ctor(string)
    IL_001a:  stloc.3
    IL_001b:  call       class [netstandard]System.IO.TextWriter [netstandard]System.Console::get_Out()
    IL_0020:  ldloc.3
    IL_0021:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatLineToTextWriter<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [mscorlib]System.IO.TextWriter,
                                                                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0026:  ldloc.2
    IL_0027:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_002c:  pop
    .line 11,11 : 5,6 ''
    IL_002d:  ldstr      "%A"
    IL_0032:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::.ctor(string)
    IL_0037:  stloc.s    V_4
    IL_0039:  call       class [netstandard]System.IO.TextWriter [netstandard]System.Console::get_Out()
    IL_003e:  ldloc.s    V_4
    IL_0040:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatLineToTextWriter<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [mscorlib]System.IO.TextWriter,
                                                                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0045:  ldloc.1
    IL_0046:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_004b:  pop
    .line 14,14 : 38,65 ''
    IL_004c:  ldstr      "123"
    IL_0051:  ldloca.s   V_5
    IL_0053:  call       bool [mscorlib]System.Int64::TryParse(string,
                                                               int64&)
    IL_0058:  stloc.s    V_6
    .line 14,14 : 5,65 ''
    IL_005a:  ldloc.s    V_6
    IL_005c:  ldloc.s    V_5
    IL_005e:  newobj     instance void class [mscorlib]System.Tuple`2<bool,int64>::.ctor(!0,
                                                                                         !1)
    IL_0063:  stloc.s    t
    .line 15,15 : 5,6 ''
    IL_0065:  ldstr      "%A"
    IL_006a:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,bool>::.ctor(string)
    IL_006f:  stloc.3
    IL_0070:  call       class [netstandard]System.IO.TextWriter [netstandard]System.Console::get_Out()
    IL_0075:  ldloc.3
    IL_0076:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatLineToTextWriter<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [mscorlib]System.IO.TextWriter,
                                                                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_007b:  ldloc.s    V_6
    IL_007d:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<bool,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0082:  pop
    .line 16,16 : 5,6 ''
    IL_0083:  ldstr      "%A"
    IL_0088:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,int64>::.ctor(string)
    IL_008d:  stloc.s    V_8
    IL_008f:  call       class [netstandard]System.IO.TextWriter [netstandard]System.Console::get_Out()
    IL_0094:  ldloc.s    V_8
    IL_0096:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatLineToTextWriter<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [mscorlib]System.IO.TextWriter,
                                                                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_009b:  ldloc.s    V_5
    IL_009d:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int64,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_00a2:  pop
    .line 21,21 : 5,6 ''
    IL_00a3:  ldstr      "%A"
    IL_00a8:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<bool,int64>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.Tuple`2<bool,int64>>::.ctor(string)
    IL_00ad:  stloc.s    V_9
    IL_00af:  call       class [netstandard]System.IO.TextWriter [netstandard]System.Console::get_Out()
    IL_00b4:  ldloc.s    V_9
    IL_00b6:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatLineToTextWriter<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<bool,int64>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [mscorlib]System.IO.TextWriter,
                                                                                                                                                                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_00bb:  ldloc.s    t
    IL_00bd:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [mscorlib]System.Tuple`2<bool,int64>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_00c2:  pop
    .line 23,23 : 5,6 ''
    IL_00c3:  ldc.i4.0
    IL_00c4:  ret
  } // end of method TupleElimination::main

} // end of class TupleElimination

.class private abstract auto ansi sealed '<StartupCode$TupleElimination>'.$TupleElimination
       extends [mscorlib]System.Object
{
} // end of class '<StartupCode$TupleElimination>'.$TupleElimination


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
