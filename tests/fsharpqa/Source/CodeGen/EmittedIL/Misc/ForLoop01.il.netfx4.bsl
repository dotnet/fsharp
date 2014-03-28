
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
.assembly ForLoop01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.ForLoop01
{
  // Offset: 0x00000000 Length: 0x00000163
}
.mresource public FSharpOptimizationData.ForLoop01
{
  // Offset: 0x00000168 Length: 0x00000050
}
.module ForLoop01.exe
// MVID: {4DAC0DD7-1795-791C-A745-0383D70DAC4D}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00000000001F0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed ForLoop01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
} // end of class ForLoop01

.class private abstract auto ansi sealed '<StartupCode$ForLoop01>'.$ForLoop01
       extends [mscorlib]System.Object
{
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       116 (0x74)
    .maxstack  5
    .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_0,
             [1] class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_1,
             [2] class [FSharp.Core]Microsoft.FSharp.Core.Unit V_2,
             [3] int32 wi,
             [4] class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_4,
             [5] int32 V_5,
             [6] class [mscorlib]System.IDisposable V_6)
    .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
    .line 5,5 : 11,21 
    IL_0000:  nop
    IL_0001:  ldc.i4.1
    IL_0002:  ldc.i4.1
    IL_0003:  ldc.i4.3
    IL_0004:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                     int32,
                                                                                                                                                                     int32)
    IL_0009:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::CreateSequence<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_000e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0013:  stloc.0
    .line 5,5 : 1,24 
    IL_0014:  ldloc.0
    IL_0015:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>
    IL_001a:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_001f:  stloc.1
    .try
    {
      IL_0020:  ldloc.1
      IL_0021:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0026:  brfalse.s  IL_0050

      .line 6,6 : 4,19 
      IL_0028:  ldloc.1
      IL_0029:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_002e:  stloc.3
      IL_002f:  ldstr      "%A"
      IL_0034:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::.ctor(string)
      IL_0039:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
      IL_003e:  stloc.s    V_4
      IL_0040:  ldloc.3
      IL_0041:  stloc.s    V_5
      IL_0043:  ldloc.s    V_4
      IL_0045:  ldloc.s    V_5
      IL_0047:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
      IL_004c:  pop
      .line 100001,100001 : 0,0 
      IL_004d:  nop
      IL_004e:  br.s       IL_0020

      IL_0050:  ldnull
      IL_0051:  stloc.2
      IL_0052:  leave.s    IL_0071

    }  // end .try
    finally
    {
      IL_0054:  ldloc.1
      IL_0055:  isinst     [mscorlib]System.IDisposable
      IL_005a:  stloc.s    V_6
      IL_005c:  ldloc.s    V_6
      IL_005e:  brfalse.s  IL_0062

      IL_0060:  br.s       IL_0064

      IL_0062:  br.s       IL_006e

      .line 100001,100001 : 0,0 
      IL_0064:  ldloc.s    V_6
      IL_0066:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_006b:  ldnull
      IL_006c:  pop
      IL_006d:  endfinally
      .line 100001,100001 : 0,0 
      IL_006e:  ldnull
      IL_006f:  pop
      IL_0070:  endfinally
      .line 100001,100001 : 0,0 
    }  // end handler
    IL_0071:  ldloc.2
    IL_0072:  pop
    IL_0073:  ret
  } // end of method $ForLoop01::main@

} // end of class '<StartupCode$ForLoop01>'.$ForLoop01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
