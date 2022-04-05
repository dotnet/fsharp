
//  Microsoft (R) .NET IL Disassembler.  Version 5.0.0-preview.7.20364.11



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
.assembly SeqExpressionSteppingTest07
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.SeqExpressionSteppingTest07
{
  // Offset: 0x00000000 Length: 0x0000089B
  // WARNING: managed resource file FSharpSignatureData.SeqExpressionSteppingTest07 created
}
.mresource public FSharpOptimizationData.SeqExpressionSteppingTest07
{
  // Offset: 0x000008A0 Length: 0x000003BB
  // WARNING: managed resource file FSharpOptimizationData.SeqExpressionSteppingTest07 created
}
.module SeqExpressionSteppingTest07.exe
// MVID: {624BD7A9-D4FB-3709-A745-0383A9D74B62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x03900000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed SeqExpressionSteppingTest7
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public specialname static int32 
          get_r() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     int32 '<StartupCode$SeqExpressionSteppingTest07>'.$SeqExpressionSteppingTest7::r@4
    IL_0005:  ret
  } // end of method SeqExpressionSteppingTest7::get_r

  .method public specialname static void 
          set_r(int32 'value') cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  stsfld     int32 '<StartupCode$SeqExpressionSteppingTest07>'.$SeqExpressionSteppingTest7::r@4
    IL_0006:  ret
  } // end of method SeqExpressionSteppingTest7::set_r

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!a> 
          f<a>() cil managed
  {
    // Code size       61 (0x3d)
    .maxstack  5
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<!!a> V_0,
             string V_1)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  call       int32 SeqExpressionSteppingTest7::get_r()
    IL_0007:  ldc.i4.1
    IL_0008:  add
    IL_0009:  call       void SeqExpressionSteppingTest7::set_r(int32)
    IL_000e:  ldc.i4.1
    IL_000f:  brfalse.s  IL_0034

    IL_0011:  ldstr      ""
    IL_0016:  stloc.1
    IL_0017:  ldloca.s   V_0
    IL_0019:  ldc.i4.0
    IL_001a:  brfalse.s  IL_0024

    IL_001c:  ldnull
    IL_001d:  unbox.any  class [mscorlib]System.Collections.Generic.IEnumerable`1<!!a>
    IL_0022:  br.s       IL_002b

    IL_0024:  ldloc.1
    IL_0025:  call       class [mscorlib]System.Exception [FSharp.Core]Microsoft.FSharp.Core.Operators::Failure(string)
    IL_002a:  throw

    IL_002b:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<!!a>::AddMany(class [mscorlib]System.Collections.Generic.IEnumerable`1<!0>)
    IL_0030:  nop
    IL_0031:  nop
    IL_0032:  br.s       IL_0035

    IL_0034:  nop
    IL_0035:  ldloca.s   V_0
    IL_0037:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<!!a>::Close()
    IL_003c:  ret
  } // end of method SeqExpressionSteppingTest7::f

  .method public static void  testSimpleForEachSeqLoopWithOneStatement(class [mscorlib]System.Collections.Generic.IEnumerable`1<object[]> inp) cil managed
  {
    // Code size       59 (0x3b)
    .maxstack  4
    .locals init (class [mscorlib]System.Collections.Generic.IEnumerable`1<object[]> V_0,
             class [mscorlib]System.Collections.Generic.IEnumerator`1<object[]> V_1,
             object[] V_2,
             class [mscorlib]System.IDisposable V_3)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<object[]>::GetEnumerator()
    IL_0008:  stloc.1
    .try
    {
      IL_0009:  ldloc.1
      IL_000a:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_000f:  brfalse.s  IL_0026

      IL_0011:  ldloc.1
      IL_0012:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<object[]>::get_Current()
      IL_0017:  stloc.2
      IL_0018:  ldstr      "{0}"
      IL_001d:  ldloc.2
      IL_001e:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                    object[])
      IL_0023:  nop
      IL_0024:  br.s       IL_0009

      IL_0026:  leave.s    IL_003a

    }  // end .try
    finally
    {
      IL_0028:  ldloc.1
      IL_0029:  isinst     [mscorlib]System.IDisposable
      IL_002e:  stloc.3
      IL_002f:  ldloc.3
      IL_0030:  brfalse.s  IL_0039

      IL_0032:  ldloc.3
      IL_0033:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0038:  endfinally
      IL_0039:  endfinally
    }  // end handler
    IL_003a:  ret
  } // end of method SeqExpressionSteppingTest7::testSimpleForEachSeqLoopWithOneStatement

  .method public static void  testSimpleForEachSeqLoopWithTwoStatements(class [mscorlib]System.Collections.Generic.IEnumerable`1<object[]> inp) cil managed
  {
    // Code size       70 (0x46)
    .maxstack  4
    .locals init (class [mscorlib]System.Collections.Generic.IEnumerable`1<object[]> V_0,
             class [mscorlib]System.Collections.Generic.IEnumerator`1<object[]> V_1,
             object[] V_2,
             class [mscorlib]System.IDisposable V_3)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<object[]>::GetEnumerator()
    IL_0008:  stloc.1
    .try
    {
      IL_0009:  ldloc.1
      IL_000a:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_000f:  brfalse.s  IL_0031

      IL_0011:  ldloc.1
      IL_0012:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<object[]>::get_Current()
      IL_0017:  stloc.2
      IL_0018:  ldstr      "{0}"
      IL_001d:  ldloc.2
      IL_001e:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                    object[])
      IL_0023:  ldstr      "{0}"
      IL_0028:  ldloc.2
      IL_0029:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                    object[])
      IL_002e:  nop
      IL_002f:  br.s       IL_0009

      IL_0031:  leave.s    IL_0045

    }  // end .try
    finally
    {
      IL_0033:  ldloc.1
      IL_0034:  isinst     [mscorlib]System.IDisposable
      IL_0039:  stloc.3
      IL_003a:  ldloc.3
      IL_003b:  brfalse.s  IL_0044

      IL_003d:  ldloc.3
      IL_003e:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0043:  endfinally
      IL_0044:  endfinally
    }  // end handler
    IL_0045:  ret
  } // end of method SeqExpressionSteppingTest7::testSimpleForEachSeqLoopWithTwoStatements

  .method public static void  testSimpleForEachArrayLoopWithOneStatement(int32[] inp) cil managed
  {
    // Code size       41 (0x29)
    .maxstack  4
    .locals init (int32[] V_0,
             int32 V_1,
             int32 V_2)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.0
    IL_0003:  stloc.1
    IL_0004:  br.s       IL_0022

    IL_0006:  ldloc.0
    IL_0007:  ldloc.1
    IL_0008:  ldelem     [mscorlib]System.Int32
    IL_000d:  stloc.2
    IL_000e:  ldstr      "{0}"
    IL_0013:  ldloc.2
    IL_0014:  box        [mscorlib]System.Int32
    IL_0019:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object)
    IL_001e:  ldloc.1
    IL_001f:  ldc.i4.1
    IL_0020:  add
    IL_0021:  stloc.1
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
    .locals init (int32[] V_0,
             int32 V_1,
             int32 V_2)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.0
    IL_0003:  stloc.1
    IL_0004:  br.s       IL_0032

    IL_0006:  ldloc.0
    IL_0007:  ldloc.1
    IL_0008:  ldelem     [mscorlib]System.Int32
    IL_000d:  stloc.2
    IL_000e:  ldstr      "{0}"
    IL_0013:  ldloc.2
    IL_0014:  box        [mscorlib]System.Int32
    IL_0019:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object)
    IL_001e:  ldstr      "{0}"
    IL_0023:  ldloc.2
    IL_0024:  box        [mscorlib]System.Int32
    IL_0029:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object)
    IL_002e:  ldloc.1
    IL_002f:  ldc.i4.1
    IL_0030:  add
    IL_0031:  stloc.1
    IL_0032:  ldloc.1
    IL_0033:  ldloc.0
    IL_0034:  ldlen
    IL_0035:  conv.i4
    IL_0036:  blt.s      IL_0006

    IL_0038:  ret
  } // end of method SeqExpressionSteppingTest7::testSimpleForEachArrayLoopWithTwoStatements

  .method public static void  testSimpleForEachListLoopWithOneStatement(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> inp) cil managed
  {
    // Code size       48 (0x30)
    .maxstack  4
    .locals init (class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_0,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_1,
             int32 V_2)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0008:  stloc.1
    IL_0009:  ldloc.1
    IL_000a:  brfalse.s  IL_002f

    IL_000c:  ldloc.0
    IL_000d:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_0012:  stloc.2
    IL_0013:  ldstr      "{0}"
    IL_0018:  ldloc.2
    IL_0019:  box        [mscorlib]System.Int32
    IL_001e:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object)
    IL_0023:  ldloc.1
    IL_0024:  stloc.0
    IL_0025:  ldloc.0
    IL_0026:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_002b:  stloc.1
    IL_002c:  nop
    IL_002d:  br.s       IL_0009

    IL_002f:  ret
  } // end of method SeqExpressionSteppingTest7::testSimpleForEachListLoopWithOneStatement

  .method public static void  testSimpleForEachListLoopWithTwoStatements(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> inp) cil managed
  {
    // Code size       64 (0x40)
    .maxstack  4
    .locals init (class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_0,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_1,
             int32 V_2)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0008:  stloc.1
    IL_0009:  ldloc.1
    IL_000a:  brfalse.s  IL_003f

    IL_000c:  ldloc.0
    IL_000d:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_0012:  stloc.2
    IL_0013:  ldstr      "{0}"
    IL_0018:  ldloc.2
    IL_0019:  box        [mscorlib]System.Int32
    IL_001e:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object)
    IL_0023:  ldstr      "{0}"
    IL_0028:  ldloc.2
    IL_0029:  box        [mscorlib]System.Int32
    IL_002e:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object)
    IL_0033:  ldloc.1
    IL_0034:  stloc.0
    IL_0035:  ldloc.0
    IL_0036:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_003b:  stloc.1
    IL_003c:  nop
    IL_003d:  br.s       IL_0009

    IL_003f:  ret
  } // end of method SeqExpressionSteppingTest7::testSimpleForEachListLoopWithTwoStatements

  .method public static void  testSimpleForEachIntRangeLoopWithOneStatement(int32 start,
                                                                            int32 stop) cil managed
  {
    // Code size       35 (0x23)
    .maxstack  5
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  ldarg.0
    IL_0001:  stloc.1
    IL_0002:  ldarg.1
    IL_0003:  stloc.0
    IL_0004:  ldloc.0
    IL_0005:  ldloc.1
    IL_0006:  blt.s      IL_0022

    IL_0008:  ldstr      "{0}"
    IL_000d:  ldloc.1
    IL_000e:  box        [mscorlib]System.Int32
    IL_0013:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object)
    IL_0018:  ldloc.1
    IL_0019:  ldc.i4.1
    IL_001a:  add
    IL_001b:  stloc.1
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
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  ldarg.0
    IL_0001:  stloc.1
    IL_0002:  ldarg.1
    IL_0003:  stloc.0
    IL_0004:  ldloc.0
    IL_0005:  ldloc.1
    IL_0006:  blt.s      IL_0032

    IL_0008:  ldstr      "{0}"
    IL_000d:  ldloc.1
    IL_000e:  box        [mscorlib]System.Int32
    IL_0013:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object)
    IL_0018:  ldstr      "{0}"
    IL_001d:  ldloc.1
    IL_001e:  box        [mscorlib]System.Int32
    IL_0023:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object)
    IL_0028:  ldloc.1
    IL_0029:  ldc.i4.1
    IL_002a:  add
    IL_002b:  stloc.1
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
    .locals init (class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> V_0,
             class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_1,
             int32 V_2,
             class [mscorlib]System.IDisposable V_3)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.m1
    IL_0002:  ldarg.1
    IL_0003:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                     int32,
                                                                                                                                                                     int32)
    IL_0008:  stloc.0
    IL_0009:  ldloc.0
    IL_000a:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_000f:  stloc.1
    .try
    {
      IL_0010:  ldloc.1
      IL_0011:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0016:  brfalse.s  IL_0032

      IL_0018:  ldloc.1
      IL_0019:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_001e:  stloc.2
      IL_001f:  ldstr      "{0}"
      IL_0024:  ldloc.2
      IL_0025:  box        [mscorlib]System.Int32
      IL_002a:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                    object)
      IL_002f:  nop
      IL_0030:  br.s       IL_0010

      IL_0032:  leave.s    IL_0046

    }  // end .try
    finally
    {
      IL_0034:  ldloc.1
      IL_0035:  isinst     [mscorlib]System.IDisposable
      IL_003a:  stloc.3
      IL_003b:  ldloc.3
      IL_003c:  brfalse.s  IL_0045

      IL_003e:  ldloc.3
      IL_003f:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0044:  endfinally
      IL_0045:  endfinally
    }  // end handler
    IL_0046:  ret
  } // end of method SeqExpressionSteppingTest7::testSimpleForEachIntRangeLoopDownWithOneStatement

  .method public static void  testSimpleForEachIntRangeLoopDownWithTwoStatements(int32 start,
                                                                                 int32 stop) cil managed
  {
    // Code size       87 (0x57)
    .maxstack  5
    .locals init (class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> V_0,
             class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_1,
             int32 V_2,
             class [mscorlib]System.IDisposable V_3)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.m1
    IL_0002:  ldarg.1
    IL_0003:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                     int32,
                                                                                                                                                                     int32)
    IL_0008:  stloc.0
    IL_0009:  ldloc.0
    IL_000a:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_000f:  stloc.1
    .try
    {
      IL_0010:  ldloc.1
      IL_0011:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0016:  brfalse.s  IL_0042

      IL_0018:  ldloc.1
      IL_0019:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_001e:  stloc.2
      IL_001f:  ldstr      "{0}"
      IL_0024:  ldloc.2
      IL_0025:  box        [mscorlib]System.Int32
      IL_002a:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                    object)
      IL_002f:  ldstr      "{0}"
      IL_0034:  ldloc.2
      IL_0035:  box        [mscorlib]System.Int32
      IL_003a:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                    object)
      IL_003f:  nop
      IL_0040:  br.s       IL_0010

      IL_0042:  leave.s    IL_0056

    }  // end .try
    finally
    {
      IL_0044:  ldloc.1
      IL_0045:  isinst     [mscorlib]System.IDisposable
      IL_004a:  stloc.3
      IL_004b:  ldloc.3
      IL_004c:  brfalse.s  IL_0055

      IL_004e:  ldloc.3
      IL_004f:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0054:  endfinally
      IL_0055:  endfinally
    }  // end handler
    IL_0056:  ret
  } // end of method SeqExpressionSteppingTest7::testSimpleForEachIntRangeLoopDownWithTwoStatements

  .method public static void  testSimpleForEachIntLoopWithOneStatement(int32 start,
                                                                       int32 stop) cil managed
  {
    // Code size       35 (0x23)
    .maxstack  5
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  ldarg.0
    IL_0001:  stloc.1
    IL_0002:  ldarg.1
    IL_0003:  stloc.0
    IL_0004:  ldloc.0
    IL_0005:  ldloc.1
    IL_0006:  blt.s      IL_0022

    IL_0008:  ldstr      "{0}"
    IL_000d:  ldloc.1
    IL_000e:  box        [mscorlib]System.Int32
    IL_0013:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object)
    IL_0018:  ldloc.1
    IL_0019:  ldc.i4.1
    IL_001a:  add
    IL_001b:  stloc.1
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
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  ldarg.0
    IL_0001:  stloc.1
    IL_0002:  ldarg.1
    IL_0003:  stloc.0
    IL_0004:  ldloc.0
    IL_0005:  ldloc.1
    IL_0006:  blt.s      IL_0032

    IL_0008:  ldstr      "{0}"
    IL_000d:  ldloc.1
    IL_000e:  box        [mscorlib]System.Int32
    IL_0013:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object)
    IL_0018:  ldstr      "{0}"
    IL_001d:  ldloc.1
    IL_001e:  box        [mscorlib]System.Int32
    IL_0023:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object)
    IL_0028:  ldloc.1
    IL_0029:  ldc.i4.1
    IL_002a:  add
    IL_002b:  stloc.1
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
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  ldarg.1
    IL_0001:  stloc.1
    IL_0002:  ldarg.0
    IL_0003:  stloc.0
    IL_0004:  ldloc.0
    IL_0005:  ldloc.1
    IL_0006:  bgt.s      IL_0022

    IL_0008:  ldstr      "{0}"
    IL_000d:  ldloc.1
    IL_000e:  box        [mscorlib]System.Int32
    IL_0013:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object)
    IL_0018:  ldloc.1
    IL_0019:  ldc.i4.1
    IL_001a:  sub
    IL_001b:  stloc.1
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
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  ldarg.1
    IL_0001:  stloc.1
    IL_0002:  ldarg.0
    IL_0003:  stloc.0
    IL_0004:  ldloc.0
    IL_0005:  ldloc.1
    IL_0006:  bgt.s      IL_0032

    IL_0008:  ldstr      "{0}"
    IL_000d:  ldloc.1
    IL_000e:  box        [mscorlib]System.Int32
    IL_0013:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object)
    IL_0018:  ldstr      "{0}"
    IL_001d:  ldloc.1
    IL_001e:  box        [mscorlib]System.Int32
    IL_0023:  call       void [mscorlib]System.Console::WriteLine(string,
                                                                  object)
    IL_0028:  ldloc.1
    IL_0029:  ldc.i4.1
    IL_002a:  sub
    IL_002b:  stloc.1
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
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_1,
             class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> V_2,
             int32 V_3,
             class [mscorlib]System.IDisposable V_4)
    IL_0000:  nop
    IL_0001:  ldc.i4.1
    IL_0002:  ldc.i4.1
    IL_0003:  ldc.i4.4
    IL_0004:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                     int32,
                                                                                                                                                                     int32)
    IL_0009:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_000e:  stloc.1
    .try
    {
      IL_000f:  ldloc.1
      IL_0010:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0015:  brfalse.s  IL_003a

      IL_0017:  ldloc.1
      IL_0018:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_001d:  stloc.3
      IL_001e:  ldstr      "hello"
      IL_0023:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
      IL_0028:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
      IL_002d:  pop
      IL_002e:  ldloca.s   V_0
      IL_0030:  ldloc.3
      IL_0031:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0036:  nop
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
      IL_0046:  ldloc.s    V_4
      IL_0048:  brfalse.s  IL_0052

      IL_004a:  ldloc.s    V_4
      IL_004c:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0051:  endfinally
      IL_0052:  endfinally
    }  // end handler
    IL_0053:  ldloc.2
    IL_0054:  pop
    IL_0055:  ldloca.s   V_0
    IL_0057:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_005c:  ret
  } // end of method SeqExpressionSteppingTest7::ListExpressionSteppingTest7

  .property int32 r()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .set void SeqExpressionSteppingTest7::set_r(int32)
    .get int32 SeqExpressionSteppingTest7::get_r()
  } // end of property SeqExpressionSteppingTest7::r
} // end of class SeqExpressionSteppingTest7

.class private abstract auto ansi sealed '<StartupCode$SeqExpressionSteppingTest07>'.$SeqExpressionSteppingTest7
       extends [mscorlib]System.Object
{
  .field static assembly int32 r@4
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       84 (0x54)
    .maxstack  4
    .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_0,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_1,
             class [mscorlib]System.Exception V_2,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string> V_3)
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 '<StartupCode$SeqExpressionSteppingTest07>'.$SeqExpressionSteppingTest7::r@4
    IL_0006:  ldstr      "res = %A"
    IL_000b:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>>::.ctor(string)
    IL_0010:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [mscorlib]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0015:  stloc.0
    .try
    {
      IL_0016:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> SeqExpressionSteppingTest7::f<int32>()
      IL_001b:  stloc.1
      IL_001c:  leave.s    IL_004b

    }  // end .try
    catch [mscorlib]System.Object 
    {
      IL_001e:  castclass  [mscorlib]System.Exception
      IL_0023:  stloc.2
      IL_0024:  ldloc.2
      IL_0025:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string> [FSharp.Core]Microsoft.FSharp.Core.Operators::FailurePattern(class [mscorlib]System.Exception)
      IL_002a:  stloc.3
      IL_002b:  ldloc.3
      IL_002c:  brfalse.s  IL_0040

      IL_002e:  call       int32 SeqExpressionSteppingTest7::get_r()
      IL_0033:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
      IL_0038:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
      IL_003d:  stloc.1
      IL_003e:  leave.s    IL_004b

      IL_0040:  rethrow
      IL_0042:  ldnull
      IL_0043:  unbox.any  class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
      IL_0048:  stloc.1
      IL_0049:  leave.s    IL_004b

    }  // end handler
    IL_004b:  ldloc.0
    IL_004c:  ldloc.1
    IL_004d:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0052:  pop
    IL_0053:  ret
  } // end of method $SeqExpressionSteppingTest7::main@

} // end of class '<StartupCode$SeqExpressionSteppingTest07>'.$SeqExpressionSteppingTest7


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file C:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net472\tests\EmittedIL\SeqExpressionStepping\SeqExpressionSteppingTest07_fs\SeqExpressionSteppingTest07.res
