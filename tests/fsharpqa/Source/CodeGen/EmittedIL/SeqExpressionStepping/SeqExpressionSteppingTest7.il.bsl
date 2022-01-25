
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
.assembly SeqExpressionSteppingTest7
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.SeqExpressionSteppingTest7
{
  // Offset: 0x00000000 Length: 0x0000084F
}
.mresource public FSharpOptimizationData.SeqExpressionSteppingTest7
{
  // Offset: 0x00000858 Length: 0x000003BA
}
.module SeqExpressionSteppingTest7.exe
// MVID: {61EFEC61-2432-93C3-A745-038361ECEF61}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x06830000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed SeqExpressionSteppingTest7
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> 
          get_r() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> '<StartupCode$SeqExpressionSteppingTest7>'.$SeqExpressionSteppingTest7::r@4
    IL_0005:  ret
  } // end of method SeqExpressionSteppingTest7::get_r

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!a> 
          f<a>() cil managed
  {
    // Code size       60 (0x3c)
    .maxstack  5
    .locals init ([0] valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<!!a> V_0,
             [1] string V_1)
    .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
    .line 5,5 : 12,57 'C:\\GitHub\\dsyme\\fsharp\\tests\\fsharpqa\\source\\CodeGen\\EmittedIL\\SeqExpressionStepping\\SeqExpressionSteppingTest7.fs'
    IL_0000:  nop
    .line 5,5 : 14,36 ''
    IL_0001:  nop
    .line 5,5 : 18,24 ''
    IL_0002:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest7::get_r()
    IL_0007:  call       void [FSharp.Core]Microsoft.FSharp.Core.Operators::Increment(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>)
    IL_000c:  nop
    .line 5,5 : 26,30 ''
    IL_000d:  ldc.i4.1
    IL_000e:  brfalse.s  IL_0033

    .line 5,5 : 44,55 ''
    IL_0010:  ldstr      ""
    IL_0015:  stloc.1
    IL_0016:  ldloca.s   V_0
    IL_0018:  ldc.i4.0
    IL_0019:  brfalse.s  IL_0023

    IL_001b:  ldnull
    IL_001c:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<!!a>
    IL_0021:  br.s       IL_002a

    IL_0023:  ldloc.1
    IL_0024:  call       class [mscorlib]System.Exception [FSharp.Core]Microsoft.FSharp.Core.Operators::Failure(string)
    IL_0029:  throw

    IL_002a:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<!!a>::AddMany(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_002f:  nop
    .line 100001,100001 : 0,0 ''
    IL_0030:  nop
    IL_0031:  br.s       IL_0034

    .line 100001,100001 : 0,0 ''
    IL_0033:  nop
    IL_0034:  ldloca.s   V_0
    IL_0036:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<!!a>::Close()
    IL_003b:  ret
  } // end of method SeqExpressionSteppingTest7::f

  .method public static void  testSimpleForEachSeqLoopWithOneStatement(class [mscorlib]System.Collections.Generic.IEnumerable`1<object[]> inp) cil managed
  {
    // Code size       59 (0x3b)
    .maxstack  4
    .locals init ([0] class [mscorlib]System.Collections.Generic.IEnumerable`1<object[]> V_0,
             [1] class [mscorlib]System.Collections.Generic.IEnumerator`1<object[]> V_1,
             [2] object[] x,
             [3] class [mscorlib]System.IDisposable V_3)
    .line 10,10 : 5,8 ''
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    .line 10,10 : 14,17 ''
    IL_0002:  ldloc.0
    IL_0003:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<object[]>::GetEnumerator()
    IL_0008:  stloc.1
    .line 10,10 : 11,13 ''
    .try
    {
      IL_0009:  ldloc.1
      IL_000a:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_000f:  brfalse.s  IL_0026

      IL_0011:  ldloc.1
      IL_0012:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<object[]>::get_Current()
      IL_0017:  stloc.2
      .line 11,11 : 8,42 ''
      IL_0018:  ldstr      "{0}"
      IL_001d:  ldloc.2
      IL_001e:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                    object[])
      .line 100001,100001 : 0,0 ''
      IL_0023:  nop
      IL_0024:  br.s       IL_0009

      IL_0026:  leave.s    IL_003a

    }  // end .try
    finally
    {
      IL_0028:  ldloc.1
      IL_0029:  isinst     [mscorlib]System.IDisposable
      IL_002e:  stloc.3
      .line 100001,100001 : 0,0 ''
      IL_002f:  ldloc.3
      IL_0030:  brfalse.s  IL_0039

      .line 100001,100001 : 0,0 ''
      IL_0032:  ldloc.3
      IL_0033:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0038:  endfinally
      .line 100001,100001 : 0,0 ''
      IL_0039:  endfinally
      .line 100001,100001 : 0,0 ''
    }  // end handler
    IL_003a:  ret
  } // end of method SeqExpressionSteppingTest7::testSimpleForEachSeqLoopWithOneStatement

  .method public static void  testSimpleForEachSeqLoopWithTwoStatements(class [mscorlib]System.Collections.Generic.IEnumerable`1<object[]> inp) cil managed
  {
    // Code size       70 (0x46)
    .maxstack  4
    .locals init ([0] class [mscorlib]System.Collections.Generic.IEnumerable`1<object[]> V_0,
             [1] class [mscorlib]System.Collections.Generic.IEnumerator`1<object[]> V_1,
             [2] object[] x,
             [3] class [mscorlib]System.IDisposable V_3)
    .line 14,14 : 5,8 ''
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    .line 14,14 : 14,17 ''
    IL_0002:  ldloc.0
    IL_0003:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<object[]>::GetEnumerator()
    IL_0008:  stloc.1
    .line 14,14 : 11,13 ''
    .try
    {
      IL_0009:  ldloc.1
      IL_000a:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_000f:  brfalse.s  IL_0031

      IL_0011:  ldloc.1
      IL_0012:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<object[]>::get_Current()
      IL_0017:  stloc.2
      .line 15,15 : 8,42 ''
      IL_0018:  ldstr      "{0}"
      IL_001d:  ldloc.2
      IL_001e:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                    object[])
      .line 16,16 : 8,42 ''
      IL_0023:  ldstr      "{0}"
      IL_0028:  ldloc.2
      IL_0029:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                    object[])
      .line 100001,100001 : 0,0 ''
      IL_002e:  nop
      IL_002f:  br.s       IL_0009

      IL_0031:  leave.s    IL_0045

    }  // end .try
    finally
    {
      IL_0033:  ldloc.1
      IL_0034:  isinst     [mscorlib]System.IDisposable
      IL_0039:  stloc.3
      .line 100001,100001 : 0,0 ''
      IL_003a:  ldloc.3
      IL_003b:  brfalse.s  IL_0044

      .line 100001,100001 : 0,0 ''
      IL_003d:  ldloc.3
      IL_003e:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0043:  endfinally
      .line 100001,100001 : 0,0 ''
      IL_0044:  endfinally
      .line 100001,100001 : 0,0 ''
    }  // end handler
    IL_0045:  ret
  } // end of method SeqExpressionSteppingTest7::testSimpleForEachSeqLoopWithTwoStatements

  .method public static void  testSimpleForEachArrayLoopWithOneStatement(int32[] inp) cil managed
  {
    // Code size       41 (0x29)
    .maxstack  4
    .locals init ([0] int32[] V_0,
             [1] int32 V_1,
             [2] int32 x)
    .line 19,19 : 5,8 ''
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.0
    IL_0003:  stloc.1
    IL_0004:  br.s       IL_0022

    .line 100001,100001 : 0,0 ''
    IL_0006:  ldloc.0
    IL_0007:  ldloc.1
    IL_0008:  ldelem     [mscorlib]System.Int32
    IL_000d:  stloc.2
    .line 20,20 : 8,42 ''
    IL_000e:  ldstr      "{0}"
    IL_0013:  ldloc.2
    IL_0014:  box        [mscorlib]System.Int32
    IL_0019:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object)
    IL_001e:  ldloc.1
    IL_001f:  ldc.i4.1
    IL_0020:  add
    IL_0021:  stloc.1
    .line 19,19 : 11,13 ''
    IL_0022:  ldloc.1
    IL_0023:  ldloc.0
    IL_0024:  ldlen
    IL_0025:  conv.i4
    IL_0026:  blt.s      IL_0006

    IL_0028:  ret
  } // end of method SeqExpressionSteppingTest7::testSimpleForEachArrayLoopWithOneStatement

  .method public static void  testSimpleForEachArrayLoopWithTwoStatements(int32[] inp) cil managed
  {
    // Code size       57 (0x39)
    .maxstack  4
    .locals init ([0] int32[] V_0,
             [1] int32 V_1,
             [2] int32 x)
    .line 23,23 : 5,8 ''
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.0
    IL_0003:  stloc.1
    IL_0004:  br.s       IL_0032

    .line 100001,100001 : 0,0 ''
    IL_0006:  ldloc.0
    IL_0007:  ldloc.1
    IL_0008:  ldelem     [mscorlib]System.Int32
    IL_000d:  stloc.2
    .line 24,24 : 8,42 ''
    IL_000e:  ldstr      "{0}"
    IL_0013:  ldloc.2
    IL_0014:  box        [mscorlib]System.Int32
    IL_0019:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object)
    .line 25,25 : 8,42 ''
    IL_001e:  ldstr      "{0}"
    IL_0023:  ldloc.2
    IL_0024:  box        [mscorlib]System.Int32
    IL_0029:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object)
    IL_002e:  ldloc.1
    IL_002f:  ldc.i4.1
    IL_0030:  add
    IL_0031:  stloc.1
    .line 23,23 : 11,13 ''
    IL_0032:  ldloc.1
    IL_0033:  ldloc.0
    IL_0034:  ldlen
    IL_0035:  conv.i4
    IL_0036:  blt.s      IL_0006

    IL_0038:  ret
  } // end of method SeqExpressionSteppingTest7::testSimpleForEachArrayLoopWithTwoStatements

  .method public static void  testSimpleForEachListLoopWithOneStatement(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> inp) cil managed
  {
    // Code size       51 (0x33)
    .maxstack  4
    .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_0,
             [1] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_1,
             [2] int32 x)
    .line 28,28 : 5,8 ''
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0008:  stloc.1
    .line 28,28 : 11,13 ''
    IL_0009:  ldloc.1
    IL_000a:  ldnull
    IL_000b:  cgt.un
    IL_000d:  brfalse.s  IL_0032

    IL_000f:  ldloc.0
    IL_0010:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_0015:  stloc.2
    .line 29,29 : 8,42 ''
    IL_0016:  ldstr      "{0}"
    IL_001b:  ldloc.2
    IL_001c:  box        [mscorlib]System.Int32
    IL_0021:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object)
    IL_0026:  ldloc.1
    IL_0027:  stloc.0
    IL_0028:  ldloc.0
    IL_0029:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_002e:  stloc.1
    .line 100001,100001 : 0,0 ''
    IL_002f:  nop
    IL_0030:  br.s       IL_0009

    IL_0032:  ret
  } // end of method SeqExpressionSteppingTest7::testSimpleForEachListLoopWithOneStatement

  .method public static void  testSimpleForEachListLoopWithTwoStatements(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> inp) cil managed
  {
    // Code size       67 (0x43)
    .maxstack  4
    .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_0,
             [1] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_1,
             [2] int32 x)
    .line 32,32 : 5,8 ''
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0008:  stloc.1
    .line 32,32 : 11,13 ''
    IL_0009:  ldloc.1
    IL_000a:  ldnull
    IL_000b:  cgt.un
    IL_000d:  brfalse.s  IL_0042

    IL_000f:  ldloc.0
    IL_0010:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_0015:  stloc.2
    .line 33,33 : 8,42 ''
    IL_0016:  ldstr      "{0}"
    IL_001b:  ldloc.2
    IL_001c:  box        [mscorlib]System.Int32
    IL_0021:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object)
    .line 34,34 : 8,42 ''
    IL_0026:  ldstr      "{0}"
    IL_002b:  ldloc.2
    IL_002c:  box        [mscorlib]System.Int32
    IL_0031:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object)
    IL_0036:  ldloc.1
    IL_0037:  stloc.0
    IL_0038:  ldloc.0
    IL_0039:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_003e:  stloc.1
    .line 100001,100001 : 0,0 ''
    IL_003f:  nop
    IL_0040:  br.s       IL_0009

    IL_0042:  ret
  } // end of method SeqExpressionSteppingTest7::testSimpleForEachListLoopWithTwoStatements

  .method public static void  testSimpleForEachIntRangeLoopWithOneStatement(int32 start,
                                                                            int32 stop) cil managed
  {
    // Code size       35 (0x23)
    .maxstack  5
    .locals init ([0] int32 V_0,
             [1] int32 x)
    .line 37,37 : 5,8 ''
    IL_0000:  ldarg.0
    IL_0001:  stloc.1
    IL_0002:  ldarg.1
    IL_0003:  stloc.0
    IL_0004:  ldloc.0
    IL_0005:  ldloc.1
    IL_0006:  blt.s      IL_0022

    .line 38,38 : 8,42 ''
    IL_0008:  ldstr      "{0}"
    IL_000d:  ldloc.1
    IL_000e:  box        [mscorlib]System.Int32
    IL_0013:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object)
    IL_0018:  ldloc.1
    IL_0019:  ldc.i4.1
    IL_001a:  add
    IL_001b:  stloc.1
    .line 37,37 : 11,13 ''
    IL_001c:  ldloc.1
    IL_001d:  ldloc.0
    IL_001e:  ldc.i4.1
    IL_001f:  add
    IL_0020:  bne.un.s   IL_0008

    IL_0022:  ret
  } // end of method SeqExpressionSteppingTest7::testSimpleForEachIntRangeLoopWithOneStatement

  .method public static void  testSimpleForEachIntRangeLoopWithTwoStatements(int32 start,
                                                                             int32 stop) cil managed
  {
    // Code size       51 (0x33)
    .maxstack  5
    .locals init ([0] int32 V_0,
             [1] int32 x)
    .line 41,41 : 5,8 ''
    IL_0000:  ldarg.0
    IL_0001:  stloc.1
    IL_0002:  ldarg.1
    IL_0003:  stloc.0
    IL_0004:  ldloc.0
    IL_0005:  ldloc.1
    IL_0006:  blt.s      IL_0032

    .line 42,42 : 8,42 ''
    IL_0008:  ldstr      "{0}"
    IL_000d:  ldloc.1
    IL_000e:  box        [mscorlib]System.Int32
    IL_0013:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object)
    .line 43,43 : 8,42 ''
    IL_0018:  ldstr      "{0}"
    IL_001d:  ldloc.1
    IL_001e:  box        [mscorlib]System.Int32
    IL_0023:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object)
    IL_0028:  ldloc.1
    IL_0029:  ldc.i4.1
    IL_002a:  add
    IL_002b:  stloc.1
    .line 41,41 : 11,13 ''
    IL_002c:  ldloc.1
    IL_002d:  ldloc.0
    IL_002e:  ldc.i4.1
    IL_002f:  add
    IL_0030:  bne.un.s   IL_0008

    IL_0032:  ret
  } // end of method SeqExpressionSteppingTest7::testSimpleForEachIntRangeLoopWithTwoStatements

  .method public static void  testSimpleForEachIntRangeLoopDownWithOneStatement(int32 start,
                                                                                int32 stop) cil managed
  {
    // Code size       71 (0x47)
    .maxstack  5
    .locals init ([0] class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> V_0,
             [1] class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_1,
             [2] int32 x,
             [3] class [mscorlib]System.IDisposable V_3)
    .line 46,46 : 5,8 ''
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.m1
    IL_0002:  ldarg.1
    IL_0003:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                     int32,
                                                                                                                                                                     int32)
    IL_0008:  stloc.0
    .line 46,46 : 14,33 ''
    IL_0009:  ldloc.0
    IL_000a:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_000f:  stloc.1
    .line 46,46 : 11,13 ''
    .try
    {
      IL_0010:  ldloc.1
      IL_0011:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0016:  brfalse.s  IL_0032

      IL_0018:  ldloc.1
      IL_0019:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_001e:  stloc.2
      .line 47,47 : 8,42 ''
      IL_001f:  ldstr      "{0}"
      IL_0024:  ldloc.2
      IL_0025:  box        [mscorlib]System.Int32
      IL_002a:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                    object)
      .line 100001,100001 : 0,0 ''
      IL_002f:  nop
      IL_0030:  br.s       IL_0010

      IL_0032:  leave.s    IL_0046

    }  // end .try
    finally
    {
      IL_0034:  ldloc.1
      IL_0035:  isinst     [mscorlib]System.IDisposable
      IL_003a:  stloc.3
      .line 100001,100001 : 0,0 ''
      IL_003b:  ldloc.3
      IL_003c:  brfalse.s  IL_0045

      .line 100001,100001 : 0,0 ''
      IL_003e:  ldloc.3
      IL_003f:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0044:  endfinally
      .line 100001,100001 : 0,0 ''
      IL_0045:  endfinally
      .line 100001,100001 : 0,0 ''
    }  // end handler
    IL_0046:  ret
  } // end of method SeqExpressionSteppingTest7::testSimpleForEachIntRangeLoopDownWithOneStatement

  .method public static void  testSimpleForEachIntRangeLoopDownWithTwoStatements(int32 start,
                                                                                 int32 stop) cil managed
  {
    // Code size       87 (0x57)
    .maxstack  5
    .locals init ([0] class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> V_0,
             [1] class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_1,
             [2] int32 x,
             [3] class [mscorlib]System.IDisposable V_3)
    .line 50,50 : 5,8 ''
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.m1
    IL_0002:  ldarg.1
    IL_0003:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                     int32,
                                                                                                                                                                     int32)
    IL_0008:  stloc.0
    .line 50,50 : 14,33 ''
    IL_0009:  ldloc.0
    IL_000a:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_000f:  stloc.1
    .line 50,50 : 11,13 ''
    .try
    {
      IL_0010:  ldloc.1
      IL_0011:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0016:  brfalse.s  IL_0042

      IL_0018:  ldloc.1
      IL_0019:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_001e:  stloc.2
      .line 51,51 : 8,42 ''
      IL_001f:  ldstr      "{0}"
      IL_0024:  ldloc.2
      IL_0025:  box        [mscorlib]System.Int32
      IL_002a:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                    object)
      .line 52,52 : 8,42 ''
      IL_002f:  ldstr      "{0}"
      IL_0034:  ldloc.2
      IL_0035:  box        [mscorlib]System.Int32
      IL_003a:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                    object)
      .line 100001,100001 : 0,0 ''
      IL_003f:  nop
      IL_0040:  br.s       IL_0010

      IL_0042:  leave.s    IL_0056

    }  // end .try
    finally
    {
      IL_0044:  ldloc.1
      IL_0045:  isinst     [mscorlib]System.IDisposable
      IL_004a:  stloc.3
      .line 100001,100001 : 0,0 ''
      IL_004b:  ldloc.3
      IL_004c:  brfalse.s  IL_0055

      .line 100001,100001 : 0,0 ''
      IL_004e:  ldloc.3
      IL_004f:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0054:  endfinally
      .line 100001,100001 : 0,0 ''
      IL_0055:  endfinally
      .line 100001,100001 : 0,0 ''
    }  // end handler
    IL_0056:  ret
  } // end of method SeqExpressionSteppingTest7::testSimpleForEachIntRangeLoopDownWithTwoStatements

  .method public static void  testSimpleForEachIntLoopWithOneStatement(int32 start,
                                                                       int32 stop) cil managed
  {
    // Code size       35 (0x23)
    .maxstack  5
    .locals init ([0] int32 V_0,
             [1] int32 x)
    .line 55,55 : 5,8 ''
    IL_0000:  ldarg.0
    IL_0001:  stloc.1
    IL_0002:  ldarg.1
    IL_0003:  stloc.0
    IL_0004:  ldloc.0
    IL_0005:  ldloc.1
    IL_0006:  blt.s      IL_0022

    .line 56,56 : 8,42 ''
    IL_0008:  ldstr      "{0}"
    IL_000d:  ldloc.1
    IL_000e:  box        [mscorlib]System.Int32
    IL_0013:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object)
    IL_0018:  ldloc.1
    IL_0019:  ldc.i4.1
    IL_001a:  add
    IL_001b:  stloc.1
    .line 55,55 : 19,21 ''
    IL_001c:  ldloc.1
    IL_001d:  ldloc.0
    IL_001e:  ldc.i4.1
    IL_001f:  add
    IL_0020:  bne.un.s   IL_0008

    IL_0022:  ret
  } // end of method SeqExpressionSteppingTest7::testSimpleForEachIntLoopWithOneStatement

  .method public static void  testSimpleForEachIntLoopWithTwoStatements(int32 start,
                                                                        int32 stop) cil managed
  {
    // Code size       51 (0x33)
    .maxstack  5
    .locals init ([0] int32 V_0,
             [1] int32 x)
    .line 59,59 : 5,8 ''
    IL_0000:  ldarg.0
    IL_0001:  stloc.1
    IL_0002:  ldarg.1
    IL_0003:  stloc.0
    IL_0004:  ldloc.0
    IL_0005:  ldloc.1
    IL_0006:  blt.s      IL_0032

    .line 60,60 : 8,42 ''
    IL_0008:  ldstr      "{0}"
    IL_000d:  ldloc.1
    IL_000e:  box        [mscorlib]System.Int32
    IL_0013:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object)
    .line 61,61 : 8,42 ''
    IL_0018:  ldstr      "{0}"
    IL_001d:  ldloc.1
    IL_001e:  box        [mscorlib]System.Int32
    IL_0023:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object)
    IL_0028:  ldloc.1
    IL_0029:  ldc.i4.1
    IL_002a:  add
    IL_002b:  stloc.1
    .line 59,59 : 19,21 ''
    IL_002c:  ldloc.1
    IL_002d:  ldloc.0
    IL_002e:  ldc.i4.1
    IL_002f:  add
    IL_0030:  bne.un.s   IL_0008

    IL_0032:  ret
  } // end of method SeqExpressionSteppingTest7::testSimpleForEachIntLoopWithTwoStatements

  .method public static void  testSimpleForEachIntLoopDownWithOneStatement(int32 start,
                                                                           int32 stop) cil managed
  {
    // Code size       35 (0x23)
    .maxstack  5
    .locals init ([0] int32 V_0,
             [1] int32 x)
    .line 64,64 : 5,8 ''
    IL_0000:  ldarg.1
    IL_0001:  stloc.1
    IL_0002:  ldarg.0
    IL_0003:  stloc.0
    IL_0004:  ldloc.0
    IL_0005:  ldloc.1
    IL_0006:  bgt.s      IL_0022

    .line 65,65 : 8,42 ''
    IL_0008:  ldstr      "{0}"
    IL_000d:  ldloc.1
    IL_000e:  box        [mscorlib]System.Int32
    IL_0013:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object)
    IL_0018:  ldloc.1
    IL_0019:  ldc.i4.1
    IL_001a:  sub
    IL_001b:  stloc.1
    .line 64,64 : 18,24 ''
    IL_001c:  ldloc.1
    IL_001d:  ldloc.0
    IL_001e:  ldc.i4.1
    IL_001f:  sub
    IL_0020:  bne.un.s   IL_0008

    IL_0022:  ret
  } // end of method SeqExpressionSteppingTest7::testSimpleForEachIntLoopDownWithOneStatement

  .method public static void  testSimpleForEachIntLoopDownWithTwoStatements(int32 start,
                                                                            int32 stop) cil managed
  {
    // Code size       51 (0x33)
    .maxstack  5
    .locals init ([0] int32 V_0,
             [1] int32 x)
    .line 68,68 : 5,8 ''
    IL_0000:  ldarg.1
    IL_0001:  stloc.1
    IL_0002:  ldarg.0
    IL_0003:  stloc.0
    IL_0004:  ldloc.0
    IL_0005:  ldloc.1
    IL_0006:  bgt.s      IL_0032

    .line 69,69 : 8,42 ''
    IL_0008:  ldstr      "{0}"
    IL_000d:  ldloc.1
    IL_000e:  box        [mscorlib]System.Int32
    IL_0013:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object)
    .line 70,70 : 8,42 ''
    IL_0018:  ldstr      "{0}"
    IL_001d:  ldloc.1
    IL_001e:  box        [mscorlib]System.Int32
    IL_0023:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object)
    IL_0028:  ldloc.1
    IL_0029:  ldc.i4.1
    IL_002a:  sub
    IL_002b:  stloc.1
    .line 68,68 : 18,24 ''
    IL_002c:  ldloc.1
    IL_002d:  ldloc.0
    IL_002e:  ldc.i4.1
    IL_002f:  sub
    IL_0030:  bne.un.s   IL_0008

    IL_0032:  ret
  } // end of method SeqExpressionSteppingTest7::testSimpleForEachIntLoopDownWithTwoStatements

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          ListExpressionSteppingTest7() cil managed
  {
    // Code size       93 (0x5d)
    .maxstack  5
    .locals init ([0] valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             [1] class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_1,
             [2] class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> V_2,
             [3] int32 x,
             [4] class [mscorlib]System.IDisposable V_4)
    .line 73,75 : 5,22 ''
    IL_0000:  nop
    .line 73,73 : 7,10 ''
    IL_0001:  ldc.i4.1
    IL_0002:  ldc.i4.1
    IL_0003:  ldc.i4.4
    IL_0004:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                     int32,
                                                                                                                                                                     int32)
    IL_0009:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_000e:  stloc.1
    .line 73,73 : 13,15 ''
    .try
    {
      IL_000f:  ldloc.1
      IL_0010:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0015:  brfalse.s  IL_003a

      IL_0017:  ldloc.1
      IL_0018:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_001d:  stloc.3
      .line 74,74 : 13,28 ''
      IL_001e:  ldstr      "hello"
      IL_0023:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
      IL_0028:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
      IL_002d:  pop
      IL_002e:  ldloca.s   V_0
      .line 75,75 : 19,20 ''
      IL_0030:  ldloc.3
      IL_0031:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0036:  nop
      .line 100001,100001 : 0,0 ''
      IL_0037:  nop
      IL_0038:  br.s       IL_000f

      IL_003a:  ldnull
      IL_003b:  stloc.2
      IL_003c:  leave.s    IL_0053

    }  // end .try
    finally
    {
      IL_003e:  ldloc.1
      IL_003f:  isinst     [mscorlib]System.IDisposable
      IL_0044:  stloc.s    V_4
      .line 100001,100001 : 0,0 ''
      IL_0046:  ldloc.s    V_4
      IL_0048:  brfalse.s  IL_0052

      .line 100001,100001 : 0,0 ''
      IL_004a:  ldloc.s    V_4
      IL_004c:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0051:  endfinally
      .line 100001,100001 : 0,0 ''
      IL_0052:  endfinally
      .line 100001,100001 : 0,0 ''
    }  // end handler
    IL_0053:  ldloc.2
    IL_0054:  pop
    IL_0055:  ldloca.s   V_0
    IL_0057:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_005c:  ret
  } // end of method SeqExpressionSteppingTest7::ListExpressionSteppingTest7

  .property class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>
          r()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest7::get_r()
  } // end of property SeqExpressionSteppingTest7::r
} // end of class SeqExpressionSteppingTest7

.class private abstract auto ansi sealed '<StartupCode$SeqExpressionSteppingTest7>'.$SeqExpressionSteppingTest7
       extends [mscorlib]System.Object
{
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> r@4
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       98 (0x62)
    .maxstack  4
    .locals init ([0] class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> r,
             [1] class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_1,
             [2] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_2,
             [3] class [mscorlib]System.Exception V_3,
             [4] class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string> V_4)
    .line 4,4 : 1,14 ''
    IL_0000:  ldc.i4.0
    IL_0001:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::Ref<int32>(!!0)
    IL_0006:  dup
    IL_0007:  stsfld     class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> '<StartupCode$SeqExpressionSteppingTest7>'.$SeqExpressionSteppingTest7::r@4
    IL_000c:  stloc.0
    .line 6,6 : 1,53 ''
    IL_000d:  ldstr      "res = %A"
    IL_0012:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>>::.ctor(string)
    IL_0017:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    .line 6,6 : 21,24 ''
    IL_001c:  stloc.1
    .line 6,6 : 25,29 ''
    .try
    {
      IL_001d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> SeqExpressionSteppingTest7::f<int32>()
      IL_0022:  stloc.2
      IL_0023:  leave.s    IL_0059

      .line 6,6 : 30,34 ''
    }  // end .try
    catch [mscorlib]System.Object 
    {
      IL_0025:  castclass  [mscorlib]System.Exception
      IL_002a:  stloc.3
      IL_002b:  ldloc.3
      IL_002c:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string> [FSharp.Core]Microsoft.FSharp.Core.Operators::FailurePattern(class [mscorlib]System.Exception)
      IL_0031:  stloc.s    V_4
      .line 100001,100001 : 0,0 ''
      IL_0033:  ldloc.s    V_4
      IL_0035:  brfalse.s  IL_004e

      .line 6,6 : 48,52 ''
      IL_0037:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest7::get_r()
      IL_003c:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::op_Dereference<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<!!0>)
      IL_0041:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
      IL_0046:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
      IL_004b:  stloc.2
      IL_004c:  leave.s    IL_0059

      .line 100001,100001 : 0,0 ''
      IL_004e:  rethrow
      IL_0050:  ldnull
      IL_0051:  unbox.any  class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
      IL_0056:  stloc.2
      IL_0057:  leave.s    IL_0059

      .line 100001,100001 : 0,0 ''
    }  // end handler
    IL_0059:  ldloc.1
    IL_005a:  ldloc.2
    IL_005b:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0060:  pop
    IL_0061:  ret
  } // end of method $SeqExpressionSteppingTest7::main@

} // end of class '<StartupCode$SeqExpressionSteppingTest7>'.$SeqExpressionSteppingTest7


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
