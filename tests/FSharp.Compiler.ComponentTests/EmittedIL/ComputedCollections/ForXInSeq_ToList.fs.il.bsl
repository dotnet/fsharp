




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
             int32 V_2,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_3,
             class [runtime]System.IDisposable V_4)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_001c

      IL_000a:  ldloc.1
      IL_000b:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0010:  stloc.2
      IL_0011:  ldloca.s   V_0
      IL_0013:  stloc.3
      IL_0014:  ldloc.3
      IL_0015:  ldloc.2
      IL_0016:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_001b:  nop
      IL_001c:  ldloc.1
      IL_001d:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0022:  brtrue.s   IL_000a

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
    IL_003b:  ldloca.s   V_0
    IL_003d:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0042:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f00(class [runtime]System.Collections.Generic.IEnumerable`1<int32> seq) cil managed
  {
    
    .maxstack  4
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             class [runtime]System.Collections.Generic.IEnumerator`1<int32> V_1,
             int32 V_2,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_3,
             class [runtime]System.IDisposable V_4)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_001c

      IL_000a:  ldloc.1
      IL_000b:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0010:  stloc.2
      IL_0011:  ldloca.s   V_0
      IL_0013:  stloc.3
      IL_0014:  ldloc.3
      IL_0015:  ldloc.2
      IL_0016:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_001b:  nop
      IL_001c:  ldloc.1
      IL_001d:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0022:  brtrue.s   IL_000a

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
    IL_003b:  ldloca.s   V_0
    IL_003d:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0042:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f000(class [runtime]System.Collections.Generic.IEnumerable`1<int32> seq) cil managed
  {
    
    .maxstack  4
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             class [runtime]System.Collections.Generic.IEnumerator`1<int32> V_1,
             int32 V_2,
             class [runtime]System.IDisposable V_3)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_001b

      IL_000a:  ldloc.1
      IL_000b:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0010:  stloc.2
      IL_0011:  nop
      IL_0012:  ldloca.s   V_0
      IL_0014:  ldloc.2
      IL_0015:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_001a:  nop
      IL_001b:  ldloc.1
      IL_001c:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0021:  brtrue.s   IL_000a

      IL_0023:  leave.s    IL_0037

    }  
    finally
    {
      IL_0025:  ldloc.1
      IL_0026:  isinst     [runtime]System.IDisposable
      IL_002b:  stloc.3
      IL_002c:  ldloc.3
      IL_002d:  brfalse.s  IL_0036

      IL_002f:  ldloc.3
      IL_0030:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_0035:  endfinally
      IL_0036:  endfinally
    }  
    IL_0037:  ldloca.s   V_0
    IL_0039:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_003e:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f0000(class [runtime]System.Collections.Generic.IEnumerable`1<int32> seq) cil managed
  {
    
    .maxstack  4
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             class [runtime]System.Collections.Generic.IEnumerator`1<int32> V_1,
             int32 V_2,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_3,
             class [runtime]System.IDisposable V_4)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_001c

      IL_000a:  ldloc.1
      IL_000b:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0010:  stloc.2
      IL_0011:  ldloca.s   V_0
      IL_0013:  stloc.3
      IL_0014:  ldloc.3
      IL_0015:  ldloc.2
      IL_0016:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_001b:  nop
      IL_001c:  ldloc.1
      IL_001d:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0022:  brtrue.s   IL_000a

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
    IL_003b:  ldloca.s   V_0
    IL_003d:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0042:  ret
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
             int32 V_2,
             int32 V_3,
             int32 V_4,
             class [runtime]System.IDisposable V_5)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_0028

      IL_000a:  ldloc.1
      IL_000b:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0010:  stloc.2
      IL_0011:  ldloc.2
      IL_0012:  ldarg.1
      IL_0013:  add
      IL_0014:  stloc.3
      IL_0015:  ldloc.2
      IL_0016:  ldarg.2
      IL_0017:  add
      IL_0018:  stloc.s    V_4
      IL_001a:  ldloca.s   V_0
      IL_001c:  ldloc.2
      IL_001d:  ldloc.3
      IL_001e:  add
      IL_001f:  ldloc.s    V_4
      IL_0021:  add
      IL_0022:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0027:  nop
      IL_0028:  ldloc.1
      IL_0029:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_002e:  brtrue.s   IL_000a

      IL_0030:  leave.s    IL_0047

    }  
    finally
    {
      IL_0032:  ldloc.1
      IL_0033:  isinst     [runtime]System.IDisposable
      IL_0038:  stloc.s    V_5
      IL_003a:  ldloc.s    V_5
      IL_003c:  brfalse.s  IL_0046

      IL_003e:  ldloc.s    V_5
      IL_0040:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_0045:  endfinally
      IL_0046:  endfinally
    }  
    IL_0047:  ldloca.s   V_0
    IL_0049:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_004e:  ret
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
             int32 V_2,
             int32 V_3,
             int32 V_4,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_5,
             class [runtime]System.IDisposable V_6)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_002c

      IL_000a:  ldloc.1
      IL_000b:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0010:  stloc.2
      IL_0011:  ldloc.2
      IL_0012:  ldarg.1
      IL_0013:  add
      IL_0014:  stloc.3
      IL_0015:  ldloc.2
      IL_0016:  ldarg.2
      IL_0017:  add
      IL_0018:  stloc.s    V_4
      IL_001a:  ldloca.s   V_0
      IL_001c:  stloc.s    V_5
      IL_001e:  ldloc.s    V_5
      IL_0020:  ldloc.2
      IL_0021:  ldloc.3
      IL_0022:  add
      IL_0023:  ldloc.s    V_4
      IL_0025:  add
      IL_0026:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_002b:  nop
      IL_002c:  ldloc.1
      IL_002d:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0032:  brtrue.s   IL_000a

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
    IL_004b:  ldloca.s   V_0
    IL_004d:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0052:  ret
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
             int32 V_2,
             int32 V_3,
             int32 V_4,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_5,
             class [runtime]System.IDisposable V_6)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_0034

      IL_000a:  ldloc.1
      IL_000b:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0010:  stloc.2
      IL_0011:  ldarg.1
      IL_0012:  ldnull
      IL_0013:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
      IL_0018:  pop
      IL_0019:  ldloc.2
      IL_001a:  ldarg.2
      IL_001b:  add
      IL_001c:  stloc.3
      IL_001d:  ldloc.2
      IL_001e:  ldarg.3
      IL_001f:  add
      IL_0020:  stloc.s    V_4
      IL_0022:  ldloca.s   V_0
      IL_0024:  stloc.s    V_5
      IL_0026:  ldloc.s    V_5
      IL_0028:  ldloc.2
      IL_0029:  ldloc.3
      IL_002a:  add
      IL_002b:  ldloc.s    V_4
      IL_002d:  add
      IL_002e:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0033:  nop
      IL_0034:  ldloc.1
      IL_0035:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_003a:  brtrue.s   IL_000a

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
    IL_0053:  ldloca.s   V_0
    IL_0055:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_005a:  ret
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
             int32 V_2,
             int32 V_3,
             int32 V_4,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_5,
             class [runtime]System.IDisposable V_6)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_0034

      IL_000a:  ldloc.1
      IL_000b:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0010:  stloc.2
      IL_0011:  ldloc.2
      IL_0012:  ldarg.2
      IL_0013:  add
      IL_0014:  stloc.3
      IL_0015:  ldarg.1
      IL_0016:  ldnull
      IL_0017:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
      IL_001c:  pop
      IL_001d:  ldloc.2
      IL_001e:  ldarg.3
      IL_001f:  add
      IL_0020:  stloc.s    V_4
      IL_0022:  ldloca.s   V_0
      IL_0024:  stloc.s    V_5
      IL_0026:  ldloc.s    V_5
      IL_0028:  ldloc.2
      IL_0029:  ldloc.3
      IL_002a:  add
      IL_002b:  ldloc.s    V_4
      IL_002d:  add
      IL_002e:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0033:  nop
      IL_0034:  ldloc.1
      IL_0035:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_003a:  brtrue.s   IL_000a

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
    IL_0053:  ldloca.s   V_0
    IL_0055:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_005a:  ret
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
             int32 V_2,
             int32 V_3,
             int32 V_4,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_5,
             class [runtime]System.IDisposable V_6)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_0034

      IL_000a:  ldloc.1
      IL_000b:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0010:  stloc.2
      IL_0011:  ldloc.2
      IL_0012:  ldarg.2
      IL_0013:  add
      IL_0014:  stloc.3
      IL_0015:  ldloc.2
      IL_0016:  ldarg.3
      IL_0017:  add
      IL_0018:  stloc.s    V_4
      IL_001a:  ldarg.1
      IL_001b:  ldnull
      IL_001c:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
      IL_0021:  pop
      IL_0022:  ldloca.s   V_0
      IL_0024:  stloc.s    V_5
      IL_0026:  ldloc.s    V_5
      IL_0028:  ldloc.2
      IL_0029:  ldloc.3
      IL_002a:  add
      IL_002b:  ldloc.s    V_4
      IL_002d:  add
      IL_002e:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0033:  nop
      IL_0034:  ldloc.1
      IL_0035:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_003a:  brtrue.s   IL_000a

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
    IL_0053:  ldloca.s   V_0
    IL_0055:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_005a:  ret
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
             int32 V_2,
             int32 V_3,
             int32 V_4,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_5,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_6,
             class [runtime]System.IDisposable V_7)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_003f

      IL_000a:  ldloc.1
      IL_000b:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0010:  stloc.2
      IL_0011:  ldloc.2
      IL_0012:  ldarg.2
      IL_0013:  add
      IL_0014:  stloc.3
      IL_0015:  ldloc.2
      IL_0016:  ldarg.3
      IL_0017:  add
      IL_0018:  stloc.s    V_4
      IL_001a:  ldloca.s   V_0
      IL_001c:  stloc.s    V_5
      IL_001e:  ldloc.s    V_5
      IL_0020:  ldarg.1
      IL_0021:  ldnull
      IL_0022:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
      IL_0027:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_002c:  nop
      IL_002d:  ldloca.s   V_0
      IL_002f:  stloc.s    V_6
      IL_0031:  ldloc.s    V_6
      IL_0033:  ldloc.2
      IL_0034:  ldloc.3
      IL_0035:  add
      IL_0036:  ldloc.s    V_4
      IL_0038:  add
      IL_0039:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_003e:  nop
      IL_003f:  ldloc.1
      IL_0040:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0045:  brtrue.s   IL_000a

      IL_0047:  leave.s    IL_005e

    }  
    finally
    {
      IL_0049:  ldloc.1
      IL_004a:  isinst     [runtime]System.IDisposable
      IL_004f:  stloc.s    V_7
      IL_0051:  ldloc.s    V_7
      IL_0053:  brfalse.s  IL_005d

      IL_0055:  ldloc.s    V_7
      IL_0057:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_005c:  endfinally
      IL_005d:  endfinally
    }  
    IL_005e:  ldloca.s   V_0
    IL_0060:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0065:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f1(class [runtime]System.Collections.Generic.IEnumerable`1<int32> seq) cil managed
  {
    
    .maxstack  4
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             class [runtime]System.Collections.Generic.IEnumerator`1<int32> V_1,
             int32 V_2,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_3,
             class [runtime]System.IDisposable V_4)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_001c

      IL_000a:  ldloc.1
      IL_000b:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0010:  stloc.2
      IL_0011:  ldloca.s   V_0
      IL_0013:  stloc.3
      IL_0014:  ldloc.3
      IL_0015:  ldloc.2
      IL_0016:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_001b:  nop
      IL_001c:  ldloc.1
      IL_001d:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0022:  brtrue.s   IL_000a

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
    IL_003b:  ldloca.s   V_0
    IL_003d:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0042:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!a> f2<a>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,!!a> f, class [runtime]System.Collections.Generic.IEnumerable`1<int32> seq) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<!!a> V_0,
             class [runtime]System.Collections.Generic.IEnumerator`1<int32> V_1,
             int32 V_2,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<!!a>& V_3,
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
      IL_0010:  stloc.2
      IL_0011:  ldloca.s   V_0
      IL_0013:  stloc.3
      IL_0014:  ldloc.3
      IL_0015:  ldarg.0
      IL_0016:  ldloc.2
      IL_0017:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,!!a>::Invoke(!0)
      IL_001c:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<!!a>::Add(!0)
      IL_0021:  nop
      IL_0022:  ldloc.1
      IL_0023:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0028:  brtrue.s   IL_000a

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
    IL_0041:  ldloca.s   V_0
    IL_0043:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<!!a>::Close()
    IL_0048:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f3(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> f, class [runtime]System.Collections.Generic.IEnumerable`1<int32> seq) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             class [runtime]System.Collections.Generic.IEnumerator`1<int32> V_1,
             int32 V_2,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_3,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_4,
             class [runtime]System.IDisposable V_5)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_0028

      IL_000a:  ldloc.1
      IL_000b:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0010:  stloc.2
      IL_0011:  ldloca.s   V_0
      IL_0013:  stloc.3
      IL_0014:  ldloc.3
      IL_0015:  ldarg.0
      IL_0016:  ldnull
      IL_0017:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
      IL_001c:  pop
      IL_001d:  stloc.s    V_4
      IL_001f:  ldloc.s    V_4
      IL_0021:  ldloc.2
      IL_0022:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0027:  nop
      IL_0028:  ldloc.1
      IL_0029:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_002e:  brtrue.s   IL_000a

      IL_0030:  leave.s    IL_0047

    }  
    finally
    {
      IL_0032:  ldloc.1
      IL_0033:  isinst     [runtime]System.IDisposable
      IL_0038:  stloc.s    V_5
      IL_003a:  ldloc.s    V_5
      IL_003c:  brfalse.s  IL_0046

      IL_003e:  ldloc.s    V_5
      IL_0040:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_0045:  endfinally
      IL_0046:  endfinally
    }  
    IL_0047:  ldloca.s   V_0
    IL_0049:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_004e:  ret
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
             int32 V_2,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_3,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_4,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_5,
             class [runtime]System.IDisposable V_6)
    IL_0000:  nop
    IL_0001:  ldarg.2
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_0034

      IL_000a:  ldloc.1
      IL_000b:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0010:  stloc.2
      IL_0011:  ldloca.s   V_0
      IL_0013:  stloc.3
      IL_0014:  ldloc.3
      IL_0015:  ldarg.0
      IL_0016:  ldnull
      IL_0017:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
      IL_001c:  pop
      IL_001d:  stloc.s    V_4
      IL_001f:  ldloc.s    V_4
      IL_0021:  ldarg.1
      IL_0022:  ldnull
      IL_0023:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
      IL_0028:  pop
      IL_0029:  stloc.s    V_5
      IL_002b:  ldloc.s    V_5
      IL_002d:  ldloc.2
      IL_002e:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0033:  nop
      IL_0034:  ldloc.1
      IL_0035:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_003a:  brtrue.s   IL_000a

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
    IL_0053:  ldloca.s   V_0
    IL_0055:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_005a:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f5(class [runtime]System.Collections.Generic.IEnumerable`1<int32> seq) cil managed
  {
    
    .maxstack  4
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             class [runtime]System.Collections.Generic.IEnumerator`1<int32> V_1,
             int32 V_2,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_3,
             class [runtime]System.IDisposable V_4)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_001c

      IL_000a:  ldloc.1
      IL_000b:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0010:  stloc.2
      IL_0011:  ldloca.s   V_0
      IL_0013:  stloc.3
      IL_0014:  ldloc.3
      IL_0015:  ldloc.2
      IL_0016:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_001b:  nop
      IL_001c:  ldloc.1
      IL_001d:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0022:  brtrue.s   IL_000a

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
    IL_003b:  ldloca.s   V_0
    IL_003d:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0042:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f6(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> f, class [runtime]System.Collections.Generic.IEnumerable`1<int32> seq) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  4
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             class [runtime]System.Collections.Generic.IEnumerator`1<int32> V_1,
             int32 V_2,
             class [runtime]System.IDisposable V_3)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_0022

      IL_000a:  ldloc.1
      IL_000b:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0010:  stloc.2
      IL_0011:  ldarg.0
      IL_0012:  ldnull
      IL_0013:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
      IL_0018:  pop
      IL_0019:  ldloca.s   V_0
      IL_001b:  ldloc.2
      IL_001c:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0021:  nop
      IL_0022:  ldloc.1
      IL_0023:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0028:  brtrue.s   IL_000a

      IL_002a:  leave.s    IL_003e

    }  
    finally
    {
      IL_002c:  ldloc.1
      IL_002d:  isinst     [runtime]System.IDisposable
      IL_0032:  stloc.3
      IL_0033:  ldloc.3
      IL_0034:  brfalse.s  IL_003d

      IL_0036:  ldloc.3
      IL_0037:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_003c:  endfinally
      IL_003d:  endfinally
    }  
    IL_003e:  ldloca.s   V_0
    IL_0040:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0045:  ret
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
             int32 V_2,
             class [runtime]System.IDisposable V_3)
    IL_0000:  nop
    IL_0001:  ldarg.2
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_002a

      IL_000a:  ldloc.1
      IL_000b:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0010:  stloc.2
      IL_0011:  ldarg.0
      IL_0012:  ldnull
      IL_0013:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
      IL_0018:  pop
      IL_0019:  ldarg.1
      IL_001a:  ldnull
      IL_001b:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
      IL_0020:  pop
      IL_0021:  ldloca.s   V_0
      IL_0023:  ldloc.2
      IL_0024:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0029:  nop
      IL_002a:  ldloc.1
      IL_002b:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0030:  brtrue.s   IL_000a

      IL_0032:  leave.s    IL_0046

    }  
    finally
    {
      IL_0034:  ldloc.1
      IL_0035:  isinst     [runtime]System.IDisposable
      IL_003a:  stloc.3
      IL_003b:  ldloc.3
      IL_003c:  brfalse.s  IL_0045

      IL_003e:  ldloc.3
      IL_003f:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_0044:  endfinally
      IL_0045:  endfinally
    }  
    IL_0046:  ldloca.s   V_0
    IL_0048:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_004d:  ret
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
             int32 V_4,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_5,
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
      IL_0019:  br.s       IL_0035

      IL_001b:  ldloc.3
      IL_001c:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0021:  stloc.s    V_4
      IL_0023:  ldloca.s   V_0
      IL_0025:  stloc.s    V_5
      IL_0027:  ldloc.s    V_5
      IL_0029:  ldloc.s    V_4
      IL_002b:  ldloc.1
      IL_002c:  add
      IL_002d:  ldloc.2
      IL_002e:  add
      IL_002f:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0034:  nop
      IL_0035:  ldloc.3
      IL_0036:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_003b:  brtrue.s   IL_001b

      IL_003d:  leave.s    IL_0054

    }  
    finally
    {
      IL_003f:  ldloc.3
      IL_0040:  isinst     [runtime]System.IDisposable
      IL_0045:  stloc.s    V_6
      IL_0047:  ldloc.s    V_6
      IL_0049:  brfalse.s  IL_0053

      IL_004b:  ldloc.s    V_6
      IL_004d:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_0052:  endfinally
      IL_0053:  endfinally
    }  
    IL_0054:  ldloca.s   V_0
    IL_0056:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_005b:  ret
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
             int32 V_3,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_4,
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
      IL_0019:  br.s       IL_0031

      IL_001b:  ldloc.2
      IL_001c:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0021:  stloc.3
      IL_0022:  ldloca.s   V_0
      IL_0024:  stloc.s    V_4
      IL_0026:  ldloc.s    V_4
      IL_0028:  ldloc.3
      IL_0029:  ldloc.1
      IL_002a:  add
      IL_002b:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0030:  nop
      IL_0031:  ldloc.2
      IL_0032:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0037:  brtrue.s   IL_001b

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
    IL_0050:  ldloca.s   V_0
    IL_0052:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0057:  ret
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
             int32 V_2,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_3,
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
      IL_0019:  br.s       IL_002d

      IL_001b:  ldloc.1
      IL_001c:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0021:  stloc.2
      IL_0022:  ldloca.s   V_0
      IL_0024:  stloc.3
      IL_0025:  ldloc.3
      IL_0026:  ldloc.2
      IL_0027:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_002c:  nop
      IL_002d:  ldloc.1
      IL_002e:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0033:  brtrue.s   IL_001b

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
    IL_004c:  ldloca.s   V_0
    IL_004e:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0053:  ret
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
             int32 V_3,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_4,
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
      IL_0019:  br.s       IL_0031

      IL_001b:  ldloc.2
      IL_001c:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0021:  stloc.3
      IL_0022:  ldloca.s   V_0
      IL_0024:  stloc.s    V_4
      IL_0026:  ldloc.s    V_4
      IL_0028:  ldloc.3
      IL_0029:  ldloc.1
      IL_002a:  add
      IL_002b:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0030:  nop
      IL_0031:  ldloc.2
      IL_0032:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0037:  brtrue.s   IL_001b

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
    IL_0050:  ldloca.s   V_0
    IL_0052:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0057:  ret
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






