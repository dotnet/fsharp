




.assembly extern runtime { }
.assembly extern FSharp.Core { }
.assembly assembly
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  
  

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.assembly
{
  
  
}
.mresource public FSharpOptimizationData.assembly
{
  
  
}
.module assembly.exe

.imagebase {value}
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       
.corflags 0x00000001    





.class public abstract auto ansi sealed SeqExpressionSteppingTest7
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .field static assembly int32 r@4
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public specialname static int32 get_r() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32 SeqExpressionSteppingTest7::r@4
    IL_0005:  ret
  } 

  .method public specialname static void set_r(int32 'value') cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  stsfld     int32 SeqExpressionSteppingTest7::r@4
    IL_0006:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!a> f<a>() cil managed
  {
    
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
    IL_001d:  unbox.any  class [runtime]System.Collections.Generic.IEnumerable`1<!!a>
    IL_0022:  br.s       IL_002b

    IL_0024:  ldloc.1
    IL_0025:  call       class [runtime]System.Exception [FSharp.Core]Microsoft.FSharp.Core.Operators::Failure(string)
    IL_002a:  throw

    IL_002b:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<!!a>::AddMany(class [runtime]System.Collections.Generic.IEnumerable`1<!0>)
    IL_0030:  nop
    IL_0031:  nop
    IL_0032:  br.s       IL_0035

    IL_0034:  nop
    IL_0035:  ldloca.s   V_0
    IL_0037:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<!!a>::Close()
    IL_003c:  ret
  } 

  .method public static void  testSimpleForEachSeqLoopWithOneStatement(class [runtime]System.Collections.Generic.IEnumerable`1<object[]> inp) cil managed
  {
    
    .maxstack  4
    .locals init (class [runtime]System.Collections.Generic.IEnumerable`1<object[]> V_0,
             class [runtime]System.Collections.Generic.IEnumerator`1<object[]> V_1,
             object[] V_2,
             class [runtime]System.IDisposable V_3)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<object[]>::GetEnumerator()
    IL_0008:  stloc.1
    .try
    {
      IL_0009:  br.s       IL_001d

      IL_000b:  ldloc.1
      IL_000c:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<object[]>::get_Current()
      IL_0011:  stloc.2
      IL_0012:  ldstr      "{0}"
      IL_0017:  ldloc.2
      IL_0018:  call       void [runtime]System.Console::WriteLine(string,
                                                                    object[])
      IL_001d:  ldloc.1
      IL_001e:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0023:  brtrue.s   IL_000b

      IL_0025:  leave.s    IL_0039

    }  
    finally
    {
      IL_0027:  ldloc.1
      IL_0028:  isinst     [runtime]System.IDisposable
      IL_002d:  stloc.3
      IL_002e:  ldloc.3
      IL_002f:  brfalse.s  IL_0038

      IL_0031:  ldloc.3
      IL_0032:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_0037:  endfinally
      IL_0038:  endfinally
    }  
    IL_0039:  ret
  } 

  .method public static void  testSimpleForEachSeqLoopWithTwoStatements(class [runtime]System.Collections.Generic.IEnumerable`1<object[]> inp) cil managed
  {
    
    .maxstack  4
    .locals init (class [runtime]System.Collections.Generic.IEnumerable`1<object[]> V_0,
             class [runtime]System.Collections.Generic.IEnumerator`1<object[]> V_1,
             object[] V_2,
             class [runtime]System.IDisposable V_3)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<object[]>::GetEnumerator()
    IL_0008:  stloc.1
    .try
    {
      IL_0009:  br.s       IL_0028

      IL_000b:  ldloc.1
      IL_000c:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<object[]>::get_Current()
      IL_0011:  stloc.2
      IL_0012:  ldstr      "{0}"
      IL_0017:  ldloc.2
      IL_0018:  call       void [runtime]System.Console::WriteLine(string,
                                                                    object[])
      IL_001d:  ldstr      "{0}"
      IL_0022:  ldloc.2
      IL_0023:  call       void [runtime]System.Console::WriteLine(string,
                                                                    object[])
      IL_0028:  ldloc.1
      IL_0029:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_002e:  brtrue.s   IL_000b

      IL_0030:  leave.s    IL_0044

    }  
    finally
    {
      IL_0032:  ldloc.1
      IL_0033:  isinst     [runtime]System.IDisposable
      IL_0038:  stloc.3
      IL_0039:  ldloc.3
      IL_003a:  brfalse.s  IL_0043

      IL_003c:  ldloc.3
      IL_003d:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_0042:  endfinally
      IL_0043:  endfinally
    }  
    IL_0044:  ret
  } 

  .method public static void  testSimpleForEachArrayLoopWithOneStatement(int32[] inp) cil managed
  {
    
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
    IL_0008:  ldelem     [runtime]System.Int32
    IL_000d:  stloc.2
    IL_000e:  ldstr      "{0}"
    IL_0013:  ldloc.2
    IL_0014:  box        [runtime]System.Int32
    IL_0019:  call       void [runtime]System.Console::WriteLine(string,
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
  } 

  .method public static void  testSimpleForEachArrayLoopWithTwoStatements(int32[] inp) cil managed
  {
    
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
    IL_0008:  ldelem     [runtime]System.Int32
    IL_000d:  stloc.2
    IL_000e:  ldstr      "{0}"
    IL_0013:  ldloc.2
    IL_0014:  box        [runtime]System.Int32
    IL_0019:  call       void [runtime]System.Console::WriteLine(string,
                                                                  object)
    IL_001e:  ldstr      "{0}"
    IL_0023:  ldloc.2
    IL_0024:  box        [runtime]System.Int32
    IL_0029:  call       void [runtime]System.Console::WriteLine(string,
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
  } 

  .method public static void  testSimpleForEachListLoopWithOneStatement(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> inp) cil managed
  {
    
    .maxstack  4
    .locals init (class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_0,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_1,
             int32 V_2)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0008:  stloc.1
    IL_0009:  br.s       IL_002b

    IL_000b:  ldloc.0
    IL_000c:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_0011:  stloc.2
    IL_0012:  ldstr      "{0}"
    IL_0017:  ldloc.2
    IL_0018:  box        [runtime]System.Int32
    IL_001d:  call       void [runtime]System.Console::WriteLine(string,
                                                                  object)
    IL_0022:  ldloc.1
    IL_0023:  stloc.0
    IL_0024:  ldloc.0
    IL_0025:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_002a:  stloc.1
    IL_002b:  ldloc.1
    IL_002c:  brtrue.s   IL_000b

    IL_002e:  ret
  } 

  .method public static void  testSimpleForEachListLoopWithTwoStatements(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> inp) cil managed
  {
    
    .maxstack  4
    .locals init (class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_0,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_1,
             int32 V_2)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_0008:  stloc.1
    IL_0009:  br.s       IL_003b

    IL_000b:  ldloc.0
    IL_000c:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
    IL_0011:  stloc.2
    IL_0012:  ldstr      "{0}"
    IL_0017:  ldloc.2
    IL_0018:  box        [runtime]System.Int32
    IL_001d:  call       void [runtime]System.Console::WriteLine(string,
                                                                  object)
    IL_0022:  ldstr      "{0}"
    IL_0027:  ldloc.2
    IL_0028:  box        [runtime]System.Int32
    IL_002d:  call       void [runtime]System.Console::WriteLine(string,
                                                                  object)
    IL_0032:  ldloc.1
    IL_0033:  stloc.0
    IL_0034:  ldloc.0
    IL_0035:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
    IL_003a:  stloc.1
    IL_003b:  ldloc.1
    IL_003c:  brtrue.s   IL_000b

    IL_003e:  ret
  } 

  .method public static void  testSimpleForEachIntRangeLoopWithOneStatement(int32 start,
                                                                            int32 stop) cil managed
  {
    
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
    IL_000e:  box        [runtime]System.Int32
    IL_0013:  call       void [runtime]System.Console::WriteLine(string,
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
  } 

  .method public static void  testSimpleForEachIntRangeLoopWithTwoStatements(int32 start,
                                                                             int32 stop) cil managed
  {
    
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
    IL_000e:  box        [runtime]System.Int32
    IL_0013:  call       void [runtime]System.Console::WriteLine(string,
                                                                  object)
    IL_0018:  ldstr      "{0}"
    IL_001d:  ldloc.1
    IL_001e:  box        [runtime]System.Int32
    IL_0023:  call       void [runtime]System.Console::WriteLine(string,
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
  } 

  .method public static void  testSimpleForEachIntRangeLoopDownWithOneStatement(int32 start,
                                                                                int32 stop) cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             uint64 V_1,
             int32 V_2,
             int32 V_3)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  bge.s      IL_0009

    IL_0004:  ldc.i4.0
    IL_0005:  conv.i8
    IL_0006:  nop
    IL_0007:  br.s       IL_0011

    IL_0009:  ldarg.0
    IL_000a:  ldarg.1
    IL_000b:  sub
    IL_000c:  conv.i8
    IL_000d:  ldc.i4.1
    IL_000e:  conv.i8
    IL_000f:  add
    IL_0010:  nop
    IL_0011:  stloc.0
    IL_0012:  ldc.i4.0
    IL_0013:  conv.i8
    IL_0014:  stloc.1
    IL_0015:  ldarg.0
    IL_0016:  stloc.2
    IL_0017:  br.s       IL_0034

    IL_0019:  ldloc.2
    IL_001a:  stloc.3
    IL_001b:  ldstr      "{0}"
    IL_0020:  ldloc.3
    IL_0021:  box        [runtime]System.Int32
    IL_0026:  call       void [runtime]System.Console::WriteLine(string,
                                                                  object)
    IL_002b:  ldloc.2
    IL_002c:  ldc.i4.m1
    IL_002d:  add
    IL_002e:  stloc.2
    IL_002f:  ldloc.1
    IL_0030:  ldc.i4.1
    IL_0031:  conv.i8
    IL_0032:  add
    IL_0033:  stloc.1
    IL_0034:  ldloc.1
    IL_0035:  ldloc.0
    IL_0036:  blt.un.s   IL_0019

    IL_0038:  ret
  } 

  .method public static void  testSimpleForEachIntRangeLoopDownWithTwoStatements(int32 start,
                                                                                 int32 stop) cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             uint64 V_1,
             int32 V_2,
             int32 V_3)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  bge.s      IL_0009

    IL_0004:  ldc.i4.0
    IL_0005:  conv.i8
    IL_0006:  nop
    IL_0007:  br.s       IL_0011

    IL_0009:  ldarg.0
    IL_000a:  ldarg.1
    IL_000b:  sub
    IL_000c:  conv.i8
    IL_000d:  ldc.i4.1
    IL_000e:  conv.i8
    IL_000f:  add
    IL_0010:  nop
    IL_0011:  stloc.0
    IL_0012:  ldc.i4.0
    IL_0013:  conv.i8
    IL_0014:  stloc.1
    IL_0015:  ldarg.0
    IL_0016:  stloc.2
    IL_0017:  br.s       IL_0044

    IL_0019:  ldloc.2
    IL_001a:  stloc.3
    IL_001b:  ldstr      "{0}"
    IL_0020:  ldloc.3
    IL_0021:  box        [runtime]System.Int32
    IL_0026:  call       void [runtime]System.Console::WriteLine(string,
                                                                  object)
    IL_002b:  ldstr      "{0}"
    IL_0030:  ldloc.3
    IL_0031:  box        [runtime]System.Int32
    IL_0036:  call       void [runtime]System.Console::WriteLine(string,
                                                                  object)
    IL_003b:  ldloc.2
    IL_003c:  ldc.i4.m1
    IL_003d:  add
    IL_003e:  stloc.2
    IL_003f:  ldloc.1
    IL_0040:  ldc.i4.1
    IL_0041:  conv.i8
    IL_0042:  add
    IL_0043:  stloc.1
    IL_0044:  ldloc.1
    IL_0045:  ldloc.0
    IL_0046:  blt.un.s   IL_0019

    IL_0048:  ret
  } 

  .method public static void  testSimpleForEachIntLoopWithOneStatement(int32 start,
                                                                       int32 stop) cil managed
  {
    
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
    IL_000e:  box        [runtime]System.Int32
    IL_0013:  call       void [runtime]System.Console::WriteLine(string,
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
  } 

  .method public static void  testSimpleForEachIntLoopWithTwoStatements(int32 start,
                                                                        int32 stop) cil managed
  {
    
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
    IL_000e:  box        [runtime]System.Int32
    IL_0013:  call       void [runtime]System.Console::WriteLine(string,
                                                                  object)
    IL_0018:  ldstr      "{0}"
    IL_001d:  ldloc.1
    IL_001e:  box        [runtime]System.Int32
    IL_0023:  call       void [runtime]System.Console::WriteLine(string,
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
  } 

  .method public static void  testSimpleForEachIntLoopDownWithOneStatement(int32 start,
                                                                           int32 stop) cil managed
  {
    
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
    IL_000e:  box        [runtime]System.Int32
    IL_0013:  call       void [runtime]System.Console::WriteLine(string,
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
  } 

  .method public static void  testSimpleForEachIntLoopDownWithTwoStatements(int32 start,
                                                                            int32 stop) cil managed
  {
    
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
    IL_000e:  box        [runtime]System.Int32
    IL_0013:  call       void [runtime]System.Console::WriteLine(string,
                                                                  object)
    IL_0018:  ldstr      "{0}"
    IL_001d:  ldloc.1
    IL_001e:  box        [runtime]System.Int32
    IL_0023:  call       void [runtime]System.Console::WriteLine(string,
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
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> ListExpressionSteppingTest7() cil managed
  {
    
    .maxstack  4
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             uint64 V_1,
             int32 V_2,
             int32 V_3)
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stloc.1
    IL_0003:  ldc.i4.1
    IL_0004:  stloc.2
    IL_0005:  br.s       IL_002b

    IL_0007:  ldloca.s   V_0
    IL_0009:  ldloc.2
    IL_000a:  stloc.3
    IL_000b:  ldstr      "hello"
    IL_0010:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
    IL_0015:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_001a:  pop
    IL_001b:  ldloc.3
    IL_001c:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0021:  nop
    IL_0022:  ldloc.2
    IL_0023:  ldc.i4.1
    IL_0024:  add
    IL_0025:  stloc.2
    IL_0026:  ldloc.1
    IL_0027:  ldc.i4.1
    IL_0028:  conv.i8
    IL_0029:  add
    IL_002a:  stloc.1
    IL_002b:  ldloc.1
    IL_002c:  ldc.i4.4
    IL_002d:  conv.i8
    IL_002e:  blt.un.s   IL_0007

    IL_0030:  ldloca.s   V_0
    IL_0032:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0037:  ret
  } 

  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$SeqExpressionSteppingTest7::init@
    IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$SeqExpressionSteppingTest7::init@
    IL_000b:  pop
    IL_000c:  ret
  } 

  .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  4
    .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_0,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_1,
             class [runtime]System.Exception V_2,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string> V_3)
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 SeqExpressionSteppingTest7::r@4
    IL_0006:  ldstr      "res = %A"
    IL_000b:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>>::.ctor(string)
    IL_0010:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0015:  stloc.0
    .try
    {
      IL_0016:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> SeqExpressionSteppingTest7::f<int32>()
      IL_001b:  stloc.1
      IL_001c:  leave.s    IL_004b

    }  
    catch [runtime]System.Object 
    {
      IL_001e:  castclass  [runtime]System.Exception
      IL_0023:  stloc.2
      IL_0024:  ldloc.2
      IL_0025:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string> [FSharp.Core]Microsoft.FSharp.Core.Operators::FailurePattern(class [runtime]System.Exception)
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

    }  
    IL_004b:  ldloc.0
    IL_004c:  ldloc.1
    IL_004d:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0052:  pop
    IL_0053:  ret
  } 

  .property int32 r()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .set void SeqExpressionSteppingTest7::set_r(int32)
    .get int32 SeqExpressionSteppingTest7::get_r()
  } 
} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$SeqExpressionSteppingTest7
       extends [runtime]System.Object
{
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  8
    IL_0000:  call       void SeqExpressionSteppingTest7::staticInitialization@()
    IL_0005:  ret
  } 

} 






