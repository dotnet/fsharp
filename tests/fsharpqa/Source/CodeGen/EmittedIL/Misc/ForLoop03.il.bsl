
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
.assembly ForLoop03
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.ForLoop03
{
  // Offset: 0x00000000 Length: 0x0000021E
}
.mresource public FSharpOptimizationData.ForLoop03
{
  // Offset: 0x00000228 Length: 0x0000007B
}
.module ForLoop03.exe
// MVID: {4DAC0DDB-1757-791C-A745-0383DB0DAC4D}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00000000002F0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed ForLoop03
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public specialname static class [mscorlib]System.Collections.Generic.List`1<int32> 
          get_ra() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [mscorlib]System.Collections.Generic.List`1<int32> '<StartupCode$ForLoop03>'.$ForLoop03::ra@5
    IL_0005:  ret
  } // end of method ForLoop03::get_ra

  .method public static void  test1() cil managed
  {
    // Code size       114 (0x72)
    .maxstack  5
    .locals init ([0] int32 z,
             [1] int32 i,
             [2] class [mscorlib]System.Collections.Generic.List`1<int32> V_2,
             [3] valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<int32> V_3,
             [4] class [FSharp.Core]Microsoft.FSharp.Core.Unit V_4,
             [5] int32 x,
             [6] class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_6,
             [7] int32 V_7)
    .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
    .line 10,10 : 4,21 
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    .line 11,11 : 4,28 
    IL_0003:  ldc.i4.0
    IL_0004:  stloc.1
    IL_0005:  br.s       IL_0049

    .line 12,12 : 15,17 
    IL_0007:  call       class [mscorlib]System.Collections.Generic.List`1<int32> ForLoop03::get_ra()
    IL_000c:  stloc.2
    .line 12,12 : 6,20 
    IL_000d:  ldloc.2
    IL_000e:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<int32>::GetEnumerator()
    IL_0013:  stloc.3
    .try
    {
      IL_0014:  ldloca.s   V_3
      IL_0016:  call       instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<int32>::MoveNext()
      IL_001b:  brfalse.s  IL_002d

      .line 13,13 : 10,20 
      IL_001d:  ldloca.s   V_3
      IL_001f:  call       instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<int32>::get_Current()
      IL_0024:  stloc.s    x
      IL_0026:  ldloc.0
      IL_0027:  ldc.i4.1
      IL_0028:  add
      IL_0029:  stloc.0
      .line 100001,100001 : 0,0 
      IL_002a:  nop
      IL_002b:  br.s       IL_0014

      IL_002d:  ldnull
      IL_002e:  stloc.s    V_4
      IL_0030:  leave.s    IL_0042

    }  // end .try
    finally
    {
      IL_0032:  ldloca.s   V_3
      IL_0034:  constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<int32>
      IL_003a:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_003f:  ldnull
      IL_0040:  pop
      IL_0041:  endfinally
      .line 100001,100001 : 0,0 
    }  // end handler
    IL_0042:  ldloc.s    V_4
    IL_0044:  pop
    IL_0045:  ldloc.1
    IL_0046:  ldc.i4.1
    IL_0047:  add
    IL_0048:  stloc.1
    .line 11,11 : 17,25 
    IL_0049:  ldloc.1
    IL_004a:  ldc.i4.1
    IL_004b:  ldc.i4     0x989680
    IL_0050:  add
    IL_0051:  blt.s      IL_0007

    IL_0053:  ldstr      "z = %d"
    IL_0058:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::.ctor(string)
    IL_005d:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0062:  stloc.s    V_6
    .line 14,14 : 4,22 
    IL_0064:  ldloc.0
    IL_0065:  stloc.s    V_7
    IL_0067:  ldloc.s    V_6
    IL_0069:  ldloc.s    V_7
    IL_006b:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0070:  pop
    IL_0071:  ret
  } // end of method ForLoop03::test1

  .property class [mscorlib]System.Collections.Generic.List`1<int32>
          ra()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [mscorlib]System.Collections.Generic.List`1<int32> ForLoop03::get_ra()
  } // end of property ForLoop03::ra
} // end of class ForLoop03

.class private abstract auto ansi sealed '<StartupCode$ForLoop03>'.$ForLoop03
       extends [mscorlib]System.Object
{
  .field static assembly class [mscorlib]System.Collections.Generic.List`1<int32> ra@5
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       42 (0x2a)
    .maxstack  5
    .locals init ([0] class [mscorlib]System.Collections.Generic.List`1<int32> ra,
             [1] int32 i)
    .line 5,5 : 1,35 
    IL_0000:  nop
    IL_0001:  ldc.i4.s   100
    IL_0003:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<int32>::.ctor(int32)
    IL_0008:  dup
    IL_0009:  stsfld     class [mscorlib]System.Collections.Generic.List`1<int32> '<StartupCode$ForLoop03>'.$ForLoop03::ra@5
    IL_000e:  stloc.0
    .line 6,6 : 1,20 
    IL_000f:  ldc.i4.0
    IL_0010:  stloc.1
    IL_0011:  br.s       IL_0022

    .line 6,6 : 21,30 
    IL_0013:  call       class [mscorlib]System.Collections.Generic.List`1<int32> ForLoop03::get_ra()
    IL_0018:  ldloc.1
    IL_0019:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_001e:  ldloc.1
    IL_001f:  ldc.i4.1
    IL_0020:  add
    IL_0021:  stloc.1
    .line 6,6 : 14,17 
    IL_0022:  ldloc.1
    IL_0023:  ldc.i4.1
    IL_0024:  ldc.i4.s   100
    IL_0026:  add
    IL_0027:  blt.s      IL_0013

    IL_0029:  ret
  } // end of method $ForLoop03::main@

} // end of class '<StartupCode$ForLoop03>'.$ForLoop03


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
