




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
.mresource public FSharpSignatureCompressedData.assembly
{
  
  
}
.mresource public FSharpOptimizationCompressedData.assembly
{
  
  
}
.module assembly.exe

.imagebase {value}
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       
.corflags 0x00000001    





.class public abstract auto ansi sealed assembly
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f0(class [runtime]System.Collections.Generic.IEnumerable`1<int32> seq) cil managed
  {
    
    .maxstack  4
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             class [runtime]System.Collections.Generic.IEnumerator`1<int32> V_1,
             class [runtime]System.Collections.Generic.IEnumerable`1<int32> V_2,
             int32 V_3,
             class [runtime]System.IDisposable V_4)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_001a

      IL_000a:  ldloc.1
      IL_000b:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0010:  stloc.3
      IL_0011:  ldloca.s   V_0
      IL_0013:  ldloc.3
      IL_0014:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0019:  nop
      IL_001a:  ldloc.1
      IL_001b:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0020:  brtrue.s   IL_000a

      IL_0022:  ldnull
      IL_0023:  stloc.2
      IL_0024:  leave.s    IL_003b

    }  
    finally
    {
      IL_0026:  ldloc.1
      IL_0027:  isinst     [runtime]System.IDisposable
      IL_002c:  stloc.s    V_4
      IL_002e:  ldloc.s    V_4
      IL_0030:  brfalse.s  IL_003a

      IL_0032:  ldloc.s    V_4
      IL_0034:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_0039:  endfinally
      IL_003a:  endfinally
    }  
    IL_003b:  ldloc.2
    IL_003c:  pop
    IL_003d:  ldloca.s   V_0
    IL_003f:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0044:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f00(class [runtime]System.Collections.Generic.IEnumerable`1<int32> seq) cil managed
  {
    
    .maxstack  4
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             class [runtime]System.Collections.Generic.IEnumerator`1<int32> V_1,
             class [runtime]System.Collections.Generic.IEnumerable`1<int32> V_2,
             int32 V_3,
             class [runtime]System.IDisposable V_4)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_001a

      IL_000a:  ldloc.1
      IL_000b:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0010:  stloc.3
      IL_0011:  ldloca.s   V_0
      IL_0013:  ldloc.3
      IL_0014:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0019:  nop
      IL_001a:  ldloc.1
      IL_001b:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0020:  brtrue.s   IL_000a

      IL_0022:  ldnull
      IL_0023:  stloc.2
      IL_0024:  leave.s    IL_003b

    }  
    finally
    {
      IL_0026:  ldloc.1
      IL_0027:  isinst     [runtime]System.IDisposable
      IL_002c:  stloc.s    V_4
      IL_002e:  ldloc.s    V_4
      IL_0030:  brfalse.s  IL_003a

      IL_0032:  ldloc.s    V_4
      IL_0034:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_0039:  endfinally
      IL_003a:  endfinally
    }  
    IL_003b:  ldloc.2
    IL_003c:  pop
    IL_003d:  ldloca.s   V_0
    IL_003f:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0044:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f000(class [runtime]System.Collections.Generic.IEnumerable`1<int32> seq) cil managed
  {
    
    .maxstack  4
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             class [runtime]System.Collections.Generic.IEnumerator`1<int32> V_1,
             class [runtime]System.Collections.Generic.IEnumerable`1<int32> V_2,
             int32 V_3,
             class [runtime]System.IDisposable V_4)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_001b

      IL_000a:  ldloc.1
      IL_000b:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0010:  stloc.3
      IL_0011:  nop
      IL_0012:  ldloca.s   V_0
      IL_0014:  ldloc.3
      IL_0015:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_001a:  nop
      IL_001b:  ldloc.1
      IL_001c:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0021:  brtrue.s   IL_000a

      IL_0023:  ldnull
      IL_0024:  stloc.2
      IL_0025:  leave.s    IL_003c

    }  
    finally
    {
      IL_0027:  ldloc.1
      IL_0028:  isinst     [runtime]System.IDisposable
      IL_002d:  stloc.s    V_4
      IL_002f:  ldloc.s    V_4
      IL_0031:  brfalse.s  IL_003b

      IL_0033:  ldloc.s    V_4
      IL_0035:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_003a:  endfinally
      IL_003b:  endfinally
    }  
    IL_003c:  ldloc.2
    IL_003d:  pop
    IL_003e:  ldloca.s   V_0
    IL_0040:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0045:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f0000(class [runtime]System.Collections.Generic.IEnumerable`1<int32> seq) cil managed
  {
    
    .maxstack  4
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             class [runtime]System.Collections.Generic.IEnumerator`1<int32> V_1,
             class [runtime]System.Collections.Generic.IEnumerable`1<int32> V_2,
             int32 V_3,
             class [runtime]System.IDisposable V_4)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_001a

      IL_000a:  ldloc.1
      IL_000b:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0010:  stloc.3
      IL_0011:  ldloca.s   V_0
      IL_0013:  ldloc.3
      IL_0014:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0019:  nop
      IL_001a:  ldloc.1
      IL_001b:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0020:  brtrue.s   IL_000a

      IL_0022:  ldnull
      IL_0023:  stloc.2
      IL_0024:  leave.s    IL_003b

    }  
    finally
    {
      IL_0026:  ldloc.1
      IL_0027:  isinst     [runtime]System.IDisposable
      IL_002c:  stloc.s    V_4
      IL_002e:  ldloc.s    V_4
      IL_0030:  brfalse.s  IL_003a

      IL_0032:  ldloc.s    V_4
      IL_0034:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_0039:  endfinally
      IL_003a:  endfinally
    }  
    IL_003b:  ldloc.2
    IL_003c:  pop
    IL_003d:  ldloca.s   V_0
    IL_003f:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0044:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          f00000(class [runtime]System.Collections.Generic.IEnumerable`1<int32> seq,
                 int32 x,
                 int32 y) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  5
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             class [runtime]System.Collections.Generic.IEnumerator`1<int32> V_1,
             class [runtime]System.Collections.Generic.IEnumerable`1<int32> V_2,
             int32 V_3,
             int32 V_4,
             int32 V_5,
             class [runtime]System.IDisposable V_6)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_002a

      IL_000a:  ldloc.1
      IL_000b:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0010:  stloc.3
      IL_0011:  ldloc.3
      IL_0012:  ldarg.1
      IL_0013:  add
      IL_0014:  stloc.s    V_4
      IL_0016:  ldloc.3
      IL_0017:  ldarg.2
      IL_0018:  add
      IL_0019:  stloc.s    V_5
      IL_001b:  ldloca.s   V_0
      IL_001d:  ldloc.3
      IL_001e:  ldloc.s    V_4
      IL_0020:  add
      IL_0021:  ldloc.s    V_5
      IL_0023:  add
      IL_0024:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0029:  nop
      IL_002a:  ldloc.1
      IL_002b:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0030:  brtrue.s   IL_000a

      IL_0032:  ldnull
      IL_0033:  stloc.2
      IL_0034:  leave.s    IL_004b

    }  
    finally
    {
      IL_0036:  ldloc.1
      IL_0037:  isinst     [runtime]System.IDisposable
      IL_003c:  stloc.s    V_6
      IL_003e:  ldloc.s    V_6
      IL_0040:  brfalse.s  IL_004a

      IL_0042:  ldloc.s    V_6
      IL_0044:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_0049:  endfinally
      IL_004a:  endfinally
    }  
    IL_004b:  ldloc.2
    IL_004c:  pop
    IL_004d:  ldloca.s   V_0
    IL_004f:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0054:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          f000000(class [runtime]System.Collections.Generic.IEnumerable`1<int32> seq,
                  int32 x,
                  int32 y) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  5
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             class [runtime]System.Collections.Generic.IEnumerator`1<int32> V_1,
             class [runtime]System.Collections.Generic.IEnumerable`1<int32> V_2,
             int32 V_3,
             int32 V_4,
             int32 V_5,
             class [runtime]System.IDisposable V_6)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_002a

      IL_000a:  ldloc.1
      IL_000b:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0010:  stloc.3
      IL_0011:  ldloc.3
      IL_0012:  ldarg.1
      IL_0013:  add
      IL_0014:  stloc.s    V_4
      IL_0016:  ldloc.3
      IL_0017:  ldarg.2
      IL_0018:  add
      IL_0019:  stloc.s    V_5
      IL_001b:  ldloca.s   V_0
      IL_001d:  ldloc.3
      IL_001e:  ldloc.s    V_4
      IL_0020:  add
      IL_0021:  ldloc.s    V_5
      IL_0023:  add
      IL_0024:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0029:  nop
      IL_002a:  ldloc.1
      IL_002b:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0030:  brtrue.s   IL_000a

      IL_0032:  ldnull
      IL_0033:  stloc.2
      IL_0034:  leave.s    IL_004b

    }  
    finally
    {
      IL_0036:  ldloc.1
      IL_0037:  isinst     [runtime]System.IDisposable
      IL_003c:  stloc.s    V_6
      IL_003e:  ldloc.s    V_6
      IL_0040:  brfalse.s  IL_004a

      IL_0042:  ldloc.s    V_6
      IL_0044:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_0049:  endfinally
      IL_004a:  endfinally
    }  
    IL_004b:  ldloc.2
    IL_004c:  pop
    IL_004d:  ldloca.s   V_0
    IL_004f:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0054:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          f0000000(class [runtime]System.Collections.Generic.IEnumerable`1<int32> seq,
                   class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> f,
                   int32 x,
                   int32 y) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 04 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             class [runtime]System.Collections.Generic.IEnumerator`1<int32> V_1,
             class [runtime]System.Collections.Generic.IEnumerable`1<int32> V_2,
             int32 V_3,
             int32 V_4,
             int32 V_5,
             class [runtime]System.IDisposable V_6)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_0032

      IL_000a:  ldloc.1
      IL_000b:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0010:  stloc.3
      IL_0011:  ldarg.1
      IL_0012:  ldnull
      IL_0013:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
      IL_0018:  pop
      IL_0019:  ldloc.3
      IL_001a:  ldarg.2
      IL_001b:  add
      IL_001c:  stloc.s    V_4
      IL_001e:  ldloc.3
      IL_001f:  ldarg.3
      IL_0020:  add
      IL_0021:  stloc.s    V_5
      IL_0023:  ldloca.s   V_0
      IL_0025:  ldloc.3
      IL_0026:  ldloc.s    V_4
      IL_0028:  add
      IL_0029:  ldloc.s    V_5
      IL_002b:  add
      IL_002c:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0031:  nop
      IL_0032:  ldloc.1
      IL_0033:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0038:  brtrue.s   IL_000a

      IL_003a:  ldnull
      IL_003b:  stloc.2
      IL_003c:  leave.s    IL_0053

    }  
    finally
    {
      IL_003e:  ldloc.1
      IL_003f:  isinst     [runtime]System.IDisposable
      IL_0044:  stloc.s    V_6
      IL_0046:  ldloc.s    V_6
      IL_0048:  brfalse.s  IL_0052

      IL_004a:  ldloc.s    V_6
      IL_004c:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_0051:  endfinally
      IL_0052:  endfinally
    }  
    IL_0053:  ldloc.2
    IL_0054:  pop
    IL_0055:  ldloca.s   V_0
    IL_0057:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_005c:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          f00000000(class [runtime]System.Collections.Generic.IEnumerable`1<int32> seq,
                    class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> f,
                    int32 x,
                    int32 y) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 04 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             class [runtime]System.Collections.Generic.IEnumerator`1<int32> V_1,
             class [runtime]System.Collections.Generic.IEnumerable`1<int32> V_2,
             int32 V_3,
             int32 V_4,
             int32 V_5,
             class [runtime]System.IDisposable V_6)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_0032

      IL_000a:  ldloc.1
      IL_000b:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0010:  stloc.3
      IL_0011:  ldloc.3
      IL_0012:  ldarg.2
      IL_0013:  add
      IL_0014:  stloc.s    V_4
      IL_0016:  ldarg.1
      IL_0017:  ldnull
      IL_0018:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
      IL_001d:  pop
      IL_001e:  ldloc.3
      IL_001f:  ldarg.3
      IL_0020:  add
      IL_0021:  stloc.s    V_5
      IL_0023:  ldloca.s   V_0
      IL_0025:  ldloc.3
      IL_0026:  ldloc.s    V_4
      IL_0028:  add
      IL_0029:  ldloc.s    V_5
      IL_002b:  add
      IL_002c:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0031:  nop
      IL_0032:  ldloc.1
      IL_0033:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0038:  brtrue.s   IL_000a

      IL_003a:  ldnull
      IL_003b:  stloc.2
      IL_003c:  leave.s    IL_0053

    }  
    finally
    {
      IL_003e:  ldloc.1
      IL_003f:  isinst     [runtime]System.IDisposable
      IL_0044:  stloc.s    V_6
      IL_0046:  ldloc.s    V_6
      IL_0048:  brfalse.s  IL_0052

      IL_004a:  ldloc.s    V_6
      IL_004c:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_0051:  endfinally
      IL_0052:  endfinally
    }  
    IL_0053:  ldloc.2
    IL_0054:  pop
    IL_0055:  ldloca.s   V_0
    IL_0057:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_005c:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          f000000000(class [runtime]System.Collections.Generic.IEnumerable`1<int32> seq,
                     class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> f,
                     int32 x,
                     int32 y) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 04 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             class [runtime]System.Collections.Generic.IEnumerator`1<int32> V_1,
             class [runtime]System.Collections.Generic.IEnumerable`1<int32> V_2,
             int32 V_3,
             int32 V_4,
             int32 V_5,
             class [runtime]System.IDisposable V_6)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_0032

      IL_000a:  ldloc.1
      IL_000b:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0010:  stloc.3
      IL_0011:  ldloc.3
      IL_0012:  ldarg.2
      IL_0013:  add
      IL_0014:  stloc.s    V_4
      IL_0016:  ldloc.3
      IL_0017:  ldarg.3
      IL_0018:  add
      IL_0019:  stloc.s    V_5
      IL_001b:  ldarg.1
      IL_001c:  ldnull
      IL_001d:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
      IL_0022:  pop
      IL_0023:  ldloca.s   V_0
      IL_0025:  ldloc.3
      IL_0026:  ldloc.s    V_4
      IL_0028:  add
      IL_0029:  ldloc.s    V_5
      IL_002b:  add
      IL_002c:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0031:  nop
      IL_0032:  ldloc.1
      IL_0033:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0038:  brtrue.s   IL_000a

      IL_003a:  ldnull
      IL_003b:  stloc.2
      IL_003c:  leave.s    IL_0053

    }  
    finally
    {
      IL_003e:  ldloc.1
      IL_003f:  isinst     [runtime]System.IDisposable
      IL_0044:  stloc.s    V_6
      IL_0046:  ldloc.s    V_6
      IL_0048:  brfalse.s  IL_0052

      IL_004a:  ldloc.s    V_6
      IL_004c:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_0051:  endfinally
      IL_0052:  endfinally
    }  
    IL_0053:  ldloc.2
    IL_0054:  pop
    IL_0055:  ldloca.s   V_0
    IL_0057:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_005c:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          f0000000000(class [runtime]System.Collections.Generic.IEnumerable`1<int32> seq,
                      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> f,
                      int32 x,
                      int32 y) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 04 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             class [runtime]System.Collections.Generic.IEnumerator`1<int32> V_1,
             class [runtime]System.Collections.Generic.IEnumerable`1<int32> V_2,
             int32 V_3,
             int32 V_4,
             int32 V_5,
             class [runtime]System.IDisposable V_6)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_0039

      IL_000a:  ldloc.1
      IL_000b:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0010:  stloc.3
      IL_0011:  ldloc.3
      IL_0012:  ldarg.2
      IL_0013:  add
      IL_0014:  stloc.s    V_4
      IL_0016:  ldloc.3
      IL_0017:  ldarg.3
      IL_0018:  add
      IL_0019:  stloc.s    V_5
      IL_001b:  ldloca.s   V_0
      IL_001d:  ldarg.1
      IL_001e:  ldnull
      IL_001f:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
      IL_0024:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0029:  nop
      IL_002a:  ldloca.s   V_0
      IL_002c:  ldloc.3
      IL_002d:  ldloc.s    V_4
      IL_002f:  add
      IL_0030:  ldloc.s    V_5
      IL_0032:  add
      IL_0033:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0038:  nop
      IL_0039:  ldloc.1
      IL_003a:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_003f:  brtrue.s   IL_000a

      IL_0041:  ldnull
      IL_0042:  stloc.2
      IL_0043:  leave.s    IL_005a

    }  
    finally
    {
      IL_0045:  ldloc.1
      IL_0046:  isinst     [runtime]System.IDisposable
      IL_004b:  stloc.s    V_6
      IL_004d:  ldloc.s    V_6
      IL_004f:  brfalse.s  IL_0059

      IL_0051:  ldloc.s    V_6
      IL_0053:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_0058:  endfinally
      IL_0059:  endfinally
    }  
    IL_005a:  ldloc.2
    IL_005b:  pop
    IL_005c:  ldloca.s   V_0
    IL_005e:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0063:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f1(class [runtime]System.Collections.Generic.IEnumerable`1<int32> seq) cil managed
  {
    
    .maxstack  4
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             class [runtime]System.Collections.Generic.IEnumerator`1<int32> V_1,
             class [runtime]System.Collections.Generic.IEnumerable`1<int32> V_2,
             int32 V_3,
             class [runtime]System.IDisposable V_4)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_001a

      IL_000a:  ldloc.1
      IL_000b:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0010:  stloc.3
      IL_0011:  ldloca.s   V_0
      IL_0013:  ldloc.3
      IL_0014:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0019:  nop
      IL_001a:  ldloc.1
      IL_001b:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0020:  brtrue.s   IL_000a

      IL_0022:  ldnull
      IL_0023:  stloc.2
      IL_0024:  leave.s    IL_003b

    }  
    finally
    {
      IL_0026:  ldloc.1
      IL_0027:  isinst     [runtime]System.IDisposable
      IL_002c:  stloc.s    V_4
      IL_002e:  ldloc.s    V_4
      IL_0030:  brfalse.s  IL_003a

      IL_0032:  ldloc.s    V_4
      IL_0034:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_0039:  endfinally
      IL_003a:  endfinally
    }  
    IL_003b:  ldloc.2
    IL_003c:  pop
    IL_003d:  ldloca.s   V_0
    IL_003f:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0044:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!a> 
          f2<a>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,!!a> f,
                class [runtime]System.Collections.Generic.IEnumerable`1<int32> seq) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<!!a> V_0,
             class [runtime]System.Collections.Generic.IEnumerator`1<int32> V_1,
             class [runtime]System.Collections.Generic.IEnumerable`1<!!a> V_2,
             int32 V_3,
             class [runtime]System.IDisposable V_4)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_0020

      IL_000a:  ldloc.1
      IL_000b:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0010:  stloc.3
      IL_0011:  ldloca.s   V_0
      IL_0013:  ldarg.0
      IL_0014:  ldloc.3
      IL_0015:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,!!a>::Invoke(!0)
      IL_001a:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<!!a>::Add(!0)
      IL_001f:  nop
      IL_0020:  ldloc.1
      IL_0021:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0026:  brtrue.s   IL_000a

      IL_0028:  ldnull
      IL_0029:  stloc.2
      IL_002a:  leave.s    IL_0041

    }  
    finally
    {
      IL_002c:  ldloc.1
      IL_002d:  isinst     [runtime]System.IDisposable
      IL_0032:  stloc.s    V_4
      IL_0034:  ldloc.s    V_4
      IL_0036:  brfalse.s  IL_0040

      IL_0038:  ldloc.s    V_4
      IL_003a:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_003f:  endfinally
      IL_0040:  endfinally
    }  
    IL_0041:  ldloc.2
    IL_0042:  pop
    IL_0043:  ldloca.s   V_0
    IL_0045:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<!!a>::Close()
    IL_004a:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          f3(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> f,
             class [runtime]System.Collections.Generic.IEnumerable`1<int32> seq) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             class [runtime]System.Collections.Generic.IEnumerator`1<int32> V_1,
             class [runtime]System.Collections.Generic.IEnumerable`1<int32> V_2,
             int32 V_3,
             class [runtime]System.IDisposable V_4)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_0022

      IL_000a:  ldloc.1
      IL_000b:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0010:  stloc.3
      IL_0011:  ldloca.s   V_0
      IL_0013:  ldarg.0
      IL_0014:  ldnull
      IL_0015:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
      IL_001a:  pop
      IL_001b:  ldloc.3
      IL_001c:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0021:  nop
      IL_0022:  ldloc.1
      IL_0023:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0028:  brtrue.s   IL_000a

      IL_002a:  ldnull
      IL_002b:  stloc.2
      IL_002c:  leave.s    IL_0043

    }  
    finally
    {
      IL_002e:  ldloc.1
      IL_002f:  isinst     [runtime]System.IDisposable
      IL_0034:  stloc.s    V_4
      IL_0036:  ldloc.s    V_4
      IL_0038:  brfalse.s  IL_0042

      IL_003a:  ldloc.s    V_4
      IL_003c:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_0041:  endfinally
      IL_0042:  endfinally
    }  
    IL_0043:  ldloc.2
    IL_0044:  pop
    IL_0045:  ldloca.s   V_0
    IL_0047:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_004c:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          f4(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> f,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> g,
             class [runtime]System.Collections.Generic.IEnumerable`1<int32> seq) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  5
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             class [runtime]System.Collections.Generic.IEnumerator`1<int32> V_1,
             class [runtime]System.Collections.Generic.IEnumerable`1<int32> V_2,
             int32 V_3,
             class [runtime]System.IDisposable V_4)
    IL_0000:  nop
    IL_0001:  ldarg.2
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_002a

      IL_000a:  ldloc.1
      IL_000b:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0010:  stloc.3
      IL_0011:  ldloca.s   V_0
      IL_0013:  ldarg.0
      IL_0014:  ldnull
      IL_0015:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
      IL_001a:  pop
      IL_001b:  ldarg.1
      IL_001c:  ldnull
      IL_001d:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
      IL_0022:  pop
      IL_0023:  ldloc.3
      IL_0024:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0029:  nop
      IL_002a:  ldloc.1
      IL_002b:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0030:  brtrue.s   IL_000a

      IL_0032:  ldnull
      IL_0033:  stloc.2
      IL_0034:  leave.s    IL_004b

    }  
    finally
    {
      IL_0036:  ldloc.1
      IL_0037:  isinst     [runtime]System.IDisposable
      IL_003c:  stloc.s    V_4
      IL_003e:  ldloc.s    V_4
      IL_0040:  brfalse.s  IL_004a

      IL_0042:  ldloc.s    V_4
      IL_0044:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_0049:  endfinally
      IL_004a:  endfinally
    }  
    IL_004b:  ldloc.2
    IL_004c:  pop
    IL_004d:  ldloca.s   V_0
    IL_004f:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0054:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f5(class [runtime]System.Collections.Generic.IEnumerable`1<int32> seq) cil managed
  {
    
    .maxstack  4
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             class [runtime]System.Collections.Generic.IEnumerator`1<int32> V_1,
             class [runtime]System.Collections.Generic.IEnumerable`1<int32> V_2,
             int32 V_3,
             class [runtime]System.IDisposable V_4)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_001a

      IL_000a:  ldloc.1
      IL_000b:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0010:  stloc.3
      IL_0011:  ldloca.s   V_0
      IL_0013:  ldloc.3
      IL_0014:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0019:  nop
      IL_001a:  ldloc.1
      IL_001b:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0020:  brtrue.s   IL_000a

      IL_0022:  ldnull
      IL_0023:  stloc.2
      IL_0024:  leave.s    IL_003b

    }  
    finally
    {
      IL_0026:  ldloc.1
      IL_0027:  isinst     [runtime]System.IDisposable
      IL_002c:  stloc.s    V_4
      IL_002e:  ldloc.s    V_4
      IL_0030:  brfalse.s  IL_003a

      IL_0032:  ldloc.s    V_4
      IL_0034:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_0039:  endfinally
      IL_003a:  endfinally
    }  
    IL_003b:  ldloc.2
    IL_003c:  pop
    IL_003d:  ldloca.s   V_0
    IL_003f:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0044:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          f6(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> f,
             class [runtime]System.Collections.Generic.IEnumerable`1<int32> seq) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  4
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             class [runtime]System.Collections.Generic.IEnumerator`1<int32> V_1,
             class [runtime]System.Collections.Generic.IEnumerable`1<int32> V_2,
             int32 V_3,
             class [runtime]System.IDisposable V_4)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_0022

      IL_000a:  ldloc.1
      IL_000b:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0010:  stloc.3
      IL_0011:  ldarg.0
      IL_0012:  ldnull
      IL_0013:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
      IL_0018:  pop
      IL_0019:  ldloca.s   V_0
      IL_001b:  ldloc.3
      IL_001c:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0021:  nop
      IL_0022:  ldloc.1
      IL_0023:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0028:  brtrue.s   IL_000a

      IL_002a:  ldnull
      IL_002b:  stloc.2
      IL_002c:  leave.s    IL_0043

    }  
    finally
    {
      IL_002e:  ldloc.1
      IL_002f:  isinst     [runtime]System.IDisposable
      IL_0034:  stloc.s    V_4
      IL_0036:  ldloc.s    V_4
      IL_0038:  brfalse.s  IL_0042

      IL_003a:  ldloc.s    V_4
      IL_003c:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_0041:  endfinally
      IL_0042:  endfinally
    }  
    IL_0043:  ldloc.2
    IL_0044:  pop
    IL_0045:  ldloca.s   V_0
    IL_0047:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_004c:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          f7(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> f,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> g,
             class [runtime]System.Collections.Generic.IEnumerable`1<int32> seq) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  4
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             class [runtime]System.Collections.Generic.IEnumerator`1<int32> V_1,
             class [runtime]System.Collections.Generic.IEnumerable`1<int32> V_2,
             int32 V_3,
             class [runtime]System.IDisposable V_4)
    IL_0000:  nop
    IL_0001:  ldarg.2
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_002a

      IL_000a:  ldloc.1
      IL_000b:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0010:  stloc.3
      IL_0011:  ldarg.0
      IL_0012:  ldnull
      IL_0013:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
      IL_0018:  pop
      IL_0019:  ldarg.1
      IL_001a:  ldnull
      IL_001b:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
      IL_0020:  pop
      IL_0021:  ldloca.s   V_0
      IL_0023:  ldloc.3
      IL_0024:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0029:  nop
      IL_002a:  ldloc.1
      IL_002b:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0030:  brtrue.s   IL_000a

      IL_0032:  ldnull
      IL_0033:  stloc.2
      IL_0034:  leave.s    IL_004b

    }  
    finally
    {
      IL_0036:  ldloc.1
      IL_0037:  isinst     [runtime]System.IDisposable
      IL_003c:  stloc.s    V_4
      IL_003e:  ldloc.s    V_4
      IL_0040:  brfalse.s  IL_004a

      IL_0042:  ldloc.s    V_4
      IL_0044:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_0049:  endfinally
      IL_004a:  endfinally
    }  
    IL_004b:  ldloc.2
    IL_004c:  pop
    IL_004d:  ldloca.s   V_0
    IL_004f:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0054:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          f8(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> f,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> g,
             class [runtime]System.Collections.Generic.IEnumerable`1<int32> seq) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  5
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             int32 V_1,
             int32 V_2,
             class [runtime]System.Collections.Generic.IEnumerator`1<int32> V_3,
             class [runtime]System.Collections.Generic.IEnumerable`1<int32> V_4,
             int32 V_5,
             class [runtime]System.IDisposable V_6)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldnull
    IL_0003:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
    IL_0008:  stloc.1
    IL_0009:  ldarg.1
    IL_000a:  ldnull
    IL_000b:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
    IL_0010:  stloc.2
    IL_0011:  nop
    IL_0012:  ldarg.2
    IL_0013:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0018:  stloc.3
    .try
    {
      IL_0019:  br.s       IL_0031

      IL_001b:  ldloc.3
      IL_001c:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0021:  stloc.s    V_5
      IL_0023:  ldloca.s   V_0
      IL_0025:  ldloc.s    V_5
      IL_0027:  ldloc.1
      IL_0028:  add
      IL_0029:  ldloc.2
      IL_002a:  add
      IL_002b:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0030:  nop
      IL_0031:  ldloc.3
      IL_0032:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0037:  brtrue.s   IL_001b

      IL_0039:  ldnull
      IL_003a:  stloc.s    V_4
      IL_003c:  leave.s    IL_0053

    }  
    finally
    {
      IL_003e:  ldloc.3
      IL_003f:  isinst     [runtime]System.IDisposable
      IL_0044:  stloc.s    V_6
      IL_0046:  ldloc.s    V_6
      IL_0048:  brfalse.s  IL_0052

      IL_004a:  ldloc.s    V_6
      IL_004c:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_0051:  endfinally
      IL_0052:  endfinally
    }  
    IL_0053:  ldloc.s    V_4
    IL_0055:  pop
    IL_0056:  ldloca.s   V_0
    IL_0058:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_005d:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          f9(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> f,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> g,
             class [runtime]System.Collections.Generic.IEnumerable`1<int32> seq) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  5
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             int32 V_1,
             class [runtime]System.Collections.Generic.IEnumerator`1<int32> V_2,
             class [runtime]System.Collections.Generic.IEnumerable`1<int32> V_3,
             int32 V_4,
             class [runtime]System.IDisposable V_5)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldnull
    IL_0003:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
    IL_0008:  stloc.1
    IL_0009:  ldarg.1
    IL_000a:  ldnull
    IL_000b:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0010:  pop
    IL_0011:  nop
    IL_0012:  ldarg.2
    IL_0013:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0018:  stloc.2
    .try
    {
      IL_0019:  br.s       IL_002f

      IL_001b:  ldloc.2
      IL_001c:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0021:  stloc.s    V_4
      IL_0023:  ldloca.s   V_0
      IL_0025:  ldloc.s    V_4
      IL_0027:  ldloc.1
      IL_0028:  add
      IL_0029:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_002e:  nop
      IL_002f:  ldloc.2
      IL_0030:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0035:  brtrue.s   IL_001b

      IL_0037:  ldnull
      IL_0038:  stloc.3
      IL_0039:  leave.s    IL_0050

    }  
    finally
    {
      IL_003b:  ldloc.2
      IL_003c:  isinst     [runtime]System.IDisposable
      IL_0041:  stloc.s    V_5
      IL_0043:  ldloc.s    V_5
      IL_0045:  brfalse.s  IL_004f

      IL_0047:  ldloc.s    V_5
      IL_0049:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_004e:  endfinally
      IL_004f:  endfinally
    }  
    IL_0050:  ldloc.3
    IL_0051:  pop
    IL_0052:  ldloca.s   V_0
    IL_0054:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0059:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          f10(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> f,
              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> g,
              class [runtime]System.Collections.Generic.IEnumerable`1<int32> seq) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  4
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             class [runtime]System.Collections.Generic.IEnumerator`1<int32> V_1,
             class [runtime]System.Collections.Generic.IEnumerable`1<int32> V_2,
             int32 V_3,
             class [runtime]System.IDisposable V_4)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldnull
    IL_0003:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0008:  pop
    IL_0009:  ldarg.1
    IL_000a:  ldnull
    IL_000b:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0010:  pop
    IL_0011:  nop
    IL_0012:  ldarg.2
    IL_0013:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0018:  stloc.1
    .try
    {
      IL_0019:  br.s       IL_002b

      IL_001b:  ldloc.1
      IL_001c:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0021:  stloc.3
      IL_0022:  ldloca.s   V_0
      IL_0024:  ldloc.3
      IL_0025:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_002a:  nop
      IL_002b:  ldloc.1
      IL_002c:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0031:  brtrue.s   IL_001b

      IL_0033:  ldnull
      IL_0034:  stloc.2
      IL_0035:  leave.s    IL_004c

    }  
    finally
    {
      IL_0037:  ldloc.1
      IL_0038:  isinst     [runtime]System.IDisposable
      IL_003d:  stloc.s    V_4
      IL_003f:  ldloc.s    V_4
      IL_0041:  brfalse.s  IL_004b

      IL_0043:  ldloc.s    V_4
      IL_0045:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_004a:  endfinally
      IL_004b:  endfinally
    }  
    IL_004c:  ldloc.2
    IL_004d:  pop
    IL_004e:  ldloca.s   V_0
    IL_0050:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0055:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          f11(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> f,
              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> g,
              class [runtime]System.Collections.Generic.IEnumerable`1<int32> seq) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  5
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             int32 V_1,
             class [runtime]System.Collections.Generic.IEnumerator`1<int32> V_2,
             class [runtime]System.Collections.Generic.IEnumerable`1<int32> V_3,
             int32 V_4,
             class [runtime]System.IDisposable V_5)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldnull
    IL_0003:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0008:  pop
    IL_0009:  ldarg.1
    IL_000a:  ldnull
    IL_000b:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
    IL_0010:  stloc.1
    IL_0011:  nop
    IL_0012:  ldarg.2
    IL_0013:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0018:  stloc.2
    .try
    {
      IL_0019:  br.s       IL_002f

      IL_001b:  ldloc.2
      IL_001c:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0021:  stloc.s    V_4
      IL_0023:  ldloca.s   V_0
      IL_0025:  ldloc.s    V_4
      IL_0027:  ldloc.1
      IL_0028:  add
      IL_0029:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_002e:  nop
      IL_002f:  ldloc.2
      IL_0030:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0035:  brtrue.s   IL_001b

      IL_0037:  ldnull
      IL_0038:  stloc.3
      IL_0039:  leave.s    IL_0050

    }  
    finally
    {
      IL_003b:  ldloc.2
      IL_003c:  isinst     [runtime]System.IDisposable
      IL_0041:  stloc.s    V_5
      IL_0043:  ldloc.s    V_5
      IL_0045:  brfalse.s  IL_004f

      IL_0047:  ldloc.s    V_5
      IL_0049:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_004e:  endfinally
      IL_004f:  endfinally
    }  
    IL_0050:  ldloc.3
    IL_0051:  pop
    IL_0052:  ldloca.s   V_0
    IL_0054:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0059:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly
       extends [runtime]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  8
    IL_0000:  ret
  } 

} 






