
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
  .ver 6:0:0:0
}
.assembly GenIter02
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.GenIter02
{
  // Offset: 0x00000000 Length: 0x000001F4
}
.mresource public FSharpOptimizationData.GenIter02
{
  // Offset: 0x000001F8 Length: 0x0000007B
}
.module GenIter02.exe
// MVID: {61E07031-F857-DC98-A745-03833170E061}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x06D10000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed GenIter02
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          squaresOfOneToTenB() cil managed
  {
    // Code size       95 (0x5f)
    .maxstack  5
    .locals init ([0] valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             [1] class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_1,
             [2] class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> V_2,
             [3] int32 x,
             [4] class [mscorlib]System.IDisposable V_4)
    .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
    .line 5,7 : 5,25 'C:\\GitHub\\dsyme\\fsharp\\tests\\fsharpqa\\source\\CodeGen\\EmittedIL\\GeneratedIterators\\GenIter02.fs'
    IL_0000:  nop
    .line 5,5 : 7,10 ''
    IL_0001:  ldc.i4.0
    IL_0002:  ldc.i4.1
    IL_0003:  ldc.i4.2
    IL_0004:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                     int32,
                                                                                                                                                                     int32)
    IL_0009:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_000e:  stloc.1
    .line 5,5 : 13,15 ''
    .try
    {
      IL_000f:  ldloc.1
      IL_0010:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0015:  brfalse.s  IL_003c

      IL_0017:  ldloc.1
      IL_0018:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_001d:  stloc.3
      .line 6,6 : 12,27 ''
      IL_001e:  ldstr      "hello"
      IL_0023:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
      IL_0028:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
      IL_002d:  pop
      IL_002e:  ldloca.s   V_0
      .line 7,7 : 18,23 ''
      IL_0030:  ldloc.3
      IL_0031:  ldloc.3
      IL_0032:  mul
      IL_0033:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0038:  nop
      .line 100001,100001 : 0,0 ''
      IL_0039:  nop
      IL_003a:  br.s       IL_000f

      IL_003c:  ldnull
      IL_003d:  stloc.2
      IL_003e:  leave.s    IL_0055

    }  // end .try
    finally
    {
      IL_0040:  ldloc.1
      IL_0041:  isinst     [mscorlib]System.IDisposable
      IL_0046:  stloc.s    V_4
      .line 100001,100001 : 0,0 ''
      IL_0048:  ldloc.s    V_4
      IL_004a:  brfalse.s  IL_0054

      .line 100001,100001 : 0,0 ''
      IL_004c:  ldloc.s    V_4
      IL_004e:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0053:  endfinally
      .line 100001,100001 : 0,0 ''
      IL_0054:  endfinally
      .line 100001,100001 : 0,0 ''
    }  // end handler
    IL_0055:  ldloc.2
    IL_0056:  pop
    IL_0057:  ldloca.s   V_0
    IL_0059:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_005e:  ret
  } // end of method GenIter02::squaresOfOneToTenB

} // end of class GenIter02

.class private abstract auto ansi sealed '<StartupCode$GenIter02>'.$GenIter02
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $GenIter02::main@

} // end of class '<StartupCode$GenIter02>'.$GenIter02


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
