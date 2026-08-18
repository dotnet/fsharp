




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
             class [runtime]System.Collections.Generic.IEnumerable`1<int32> V_2,
             int32 V_3,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_4,
             class [runtime]System.IDisposable V_5)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_001e

      IL_000a:  ldloc.1
      IL_000b:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0010:  stloc.3
      IL_0011:  ldloca.s   V_0
      IL_0013:  stloc.s    V_4
      IL_0015:  ldloc.s    V_4
      IL_0017:  ldloc.3
      IL_0018:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_001d:  nop
      IL_001e:  ldloc.1
      IL_001f:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0024:  brtrue.s   IL_000a

      IL_0026:  ldnull
      IL_0027:  stloc.2
      IL_0028:  leave.s    IL_003f

    }  
    finally
    {
      IL_002a:  ldloc.1
      IL_002b:  isinst     [runtime]System.IDisposable
      IL_0030:  stloc.s    V_5
      IL_0032:  ldloc.s    V_5
      IL_0034:  brfalse.s  IL_003e

      IL_0036:  ldloc.s    V_5
      IL_0038:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_003d:  endfinally
      IL_003e:  endfinally
    }  
    IL_003f:  ldloc.2
    IL_0040:  pop
    IL_0041:  ldloca.s   V_0
    IL_0043:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0048:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f00(class [runtime]System.Collections.Generic.IEnumerable`1<int32> seq) cil managed
  {
    
    .maxstack  4
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             class [runtime]System.Collections.Generic.IEnumerator`1<int32> V_1,
             class [runtime]System.Collections.Generic.IEnumerable`1<int32> V_2,
             int32 V_3,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_4,
             class [runtime]System.IDisposable V_5)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_001e

      IL_000a:  ldloc.1
      IL_000b:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0010:  stloc.3
      IL_0011:  ldloca.s   V_0
      IL_0013:  stloc.s    V_4
      IL_0015:  ldloc.s    V_4
      IL_0017:  ldloc.3
      IL_0018:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_001d:  nop
      IL_001e:  ldloc.1
      IL_001f:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0024:  brtrue.s   IL_000a

      IL_0026:  ldnull
      IL_0027:  stloc.2
      IL_0028:  leave.s    IL_003f

    }  
    finally
    {
      IL_002a:  ldloc.1
      IL_002b:  isinst     [runtime]System.IDisposable
      IL_0030:  stloc.s    V_5
      IL_0032:  ldloc.s    V_5
      IL_0034:  brfalse.s  IL_003e

      IL_0036:  ldloc.s    V_5
      IL_0038:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_003d:  endfinally
      IL_003e:  endfinally
    }  
    IL_003f:  ldloc.2
    IL_0040:  pop
    IL_0041:  ldloca.s   V_0
    IL_0043:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0048:  ret
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
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_4,
             class [runtime]System.IDisposable V_5)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_001e

      IL_000a:  ldloc.1
      IL_000b:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0010:  stloc.3
      IL_0011:  ldloca.s   V_0
      IL_0013:  stloc.s    V_4
      IL_0015:  ldloc.s    V_4
      IL_0017:  ldloc.3
      IL_0018:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_001d:  nop
      IL_001e:  ldloc.1
      IL_001f:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0024:  brtrue.s   IL_000a

      IL_0026:  ldnull
      IL_0027:  stloc.2
      IL_0028:  leave.s    IL_003f

    }  
    finally
    {
      IL_002a:  ldloc.1
      IL_002b:  isinst     [runtime]System.IDisposable
      IL_0030:  stloc.s    V_5
      IL_0032:  ldloc.s    V_5
      IL_0034:  brfalse.s  IL_003e

      IL_0036:  ldloc.s    V_5
      IL_0038:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_003d:  endfinally
      IL_003e:  endfinally
    }  
    IL_003f:  ldloc.2
    IL_0040:  pop
    IL_0041:  ldloca.s   V_0
    IL_0043:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0048:  ret
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
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_6,
             class [runtime]System.IDisposable V_7)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_002e

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
      IL_001d:  stloc.s    V_6
      IL_001f:  ldloc.s    V_6
      IL_0021:  ldloc.3
      IL_0022:  ldloc.s    V_4
      IL_0024:  add
      IL_0025:  ldloc.s    V_5
      IL_0027:  add
      IL_0028:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_002d:  nop
      IL_002e:  ldloc.1
      IL_002f:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0034:  brtrue.s   IL_000a

      IL_0036:  ldnull
      IL_0037:  stloc.2
      IL_0038:  leave.s    IL_004f

    }  
    finally
    {
      IL_003a:  ldloc.1
      IL_003b:  isinst     [runtime]System.IDisposable
      IL_0040:  stloc.s    V_7
      IL_0042:  ldloc.s    V_7
      IL_0044:  brfalse.s  IL_004e

      IL_0046:  ldloc.s    V_7
      IL_0048:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_004d:  endfinally
      IL_004e:  endfinally
    }  
    IL_004f:  ldloc.2
    IL_0050:  pop
    IL_0051:  ldloca.s   V_0
    IL_0053:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0058:  ret
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
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_6,
             class [runtime]System.IDisposable V_7)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_0036

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
      IL_0025:  stloc.s    V_6
      IL_0027:  ldloc.s    V_6
      IL_0029:  ldloc.3
      IL_002a:  ldloc.s    V_4
      IL_002c:  add
      IL_002d:  ldloc.s    V_5
      IL_002f:  add
      IL_0030:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0035:  nop
      IL_0036:  ldloc.1
      IL_0037:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_003c:  brtrue.s   IL_000a

      IL_003e:  ldnull
      IL_003f:  stloc.2
      IL_0040:  leave.s    IL_0057

    }  
    finally
    {
      IL_0042:  ldloc.1
      IL_0043:  isinst     [runtime]System.IDisposable
      IL_0048:  stloc.s    V_7
      IL_004a:  ldloc.s    V_7
      IL_004c:  brfalse.s  IL_0056

      IL_004e:  ldloc.s    V_7
      IL_0050:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_0055:  endfinally
      IL_0056:  endfinally
    }  
    IL_0057:  ldloc.2
    IL_0058:  pop
    IL_0059:  ldloca.s   V_0
    IL_005b:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0060:  ret
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
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_6,
             class [runtime]System.IDisposable V_7)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_0036

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
      IL_0025:  stloc.s    V_6
      IL_0027:  ldloc.s    V_6
      IL_0029:  ldloc.3
      IL_002a:  ldloc.s    V_4
      IL_002c:  add
      IL_002d:  ldloc.s    V_5
      IL_002f:  add
      IL_0030:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0035:  nop
      IL_0036:  ldloc.1
      IL_0037:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_003c:  brtrue.s   IL_000a

      IL_003e:  ldnull
      IL_003f:  stloc.2
      IL_0040:  leave.s    IL_0057

    }  
    finally
    {
      IL_0042:  ldloc.1
      IL_0043:  isinst     [runtime]System.IDisposable
      IL_0048:  stloc.s    V_7
      IL_004a:  ldloc.s    V_7
      IL_004c:  brfalse.s  IL_0056

      IL_004e:  ldloc.s    V_7
      IL_0050:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_0055:  endfinally
      IL_0056:  endfinally
    }  
    IL_0057:  ldloc.2
    IL_0058:  pop
    IL_0059:  ldloca.s   V_0
    IL_005b:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0060:  ret
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
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_6,
             class [runtime]System.IDisposable V_7)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_0036

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
      IL_0025:  stloc.s    V_6
      IL_0027:  ldloc.s    V_6
      IL_0029:  ldloc.3
      IL_002a:  ldloc.s    V_4
      IL_002c:  add
      IL_002d:  ldloc.s    V_5
      IL_002f:  add
      IL_0030:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0035:  nop
      IL_0036:  ldloc.1
      IL_0037:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_003c:  brtrue.s   IL_000a

      IL_003e:  ldnull
      IL_003f:  stloc.2
      IL_0040:  leave.s    IL_0057

    }  
    finally
    {
      IL_0042:  ldloc.1
      IL_0043:  isinst     [runtime]System.IDisposable
      IL_0048:  stloc.s    V_7
      IL_004a:  ldloc.s    V_7
      IL_004c:  brfalse.s  IL_0056

      IL_004e:  ldloc.s    V_7
      IL_0050:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_0055:  endfinally
      IL_0056:  endfinally
    }  
    IL_0057:  ldloc.2
    IL_0058:  pop
    IL_0059:  ldloca.s   V_0
    IL_005b:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0060:  ret
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
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_6,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_7,
             class [runtime]System.IDisposable V_8)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_0041

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
      IL_001d:  stloc.s    V_6
      IL_001f:  ldloc.s    V_6
      IL_0021:  ldarg.1
      IL_0022:  ldnull
      IL_0023:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
      IL_0028:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_002d:  nop
      IL_002e:  ldloca.s   V_0
      IL_0030:  stloc.s    V_7
      IL_0032:  ldloc.s    V_7
      IL_0034:  ldloc.3
      IL_0035:  ldloc.s    V_4
      IL_0037:  add
      IL_0038:  ldloc.s    V_5
      IL_003a:  add
      IL_003b:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0040:  nop
      IL_0041:  ldloc.1
      IL_0042:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0047:  brtrue.s   IL_000a

      IL_0049:  ldnull
      IL_004a:  stloc.2
      IL_004b:  leave.s    IL_0062

    }  
    finally
    {
      IL_004d:  ldloc.1
      IL_004e:  isinst     [runtime]System.IDisposable
      IL_0053:  stloc.s    V_8
      IL_0055:  ldloc.s    V_8
      IL_0057:  brfalse.s  IL_0061

      IL_0059:  ldloc.s    V_8
      IL_005b:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_0060:  endfinally
      IL_0061:  endfinally
    }  
    IL_0062:  ldloc.2
    IL_0063:  pop
    IL_0064:  ldloca.s   V_0
    IL_0066:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_006b:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f1(class [runtime]System.Collections.Generic.IEnumerable`1<int32> seq) cil managed
  {
    
    .maxstack  4
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             class [runtime]System.Collections.Generic.IEnumerator`1<int32> V_1,
             class [runtime]System.Collections.Generic.IEnumerable`1<int32> V_2,
             int32 V_3,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_4,
             class [runtime]System.IDisposable V_5)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_001e

      IL_000a:  ldloc.1
      IL_000b:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0010:  stloc.3
      IL_0011:  ldloca.s   V_0
      IL_0013:  stloc.s    V_4
      IL_0015:  ldloc.s    V_4
      IL_0017:  ldloc.3
      IL_0018:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_001d:  nop
      IL_001e:  ldloc.1
      IL_001f:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0024:  brtrue.s   IL_000a

      IL_0026:  ldnull
      IL_0027:  stloc.2
      IL_0028:  leave.s    IL_003f

    }  
    finally
    {
      IL_002a:  ldloc.1
      IL_002b:  isinst     [runtime]System.IDisposable
      IL_0030:  stloc.s    V_5
      IL_0032:  ldloc.s    V_5
      IL_0034:  brfalse.s  IL_003e

      IL_0036:  ldloc.s    V_5
      IL_0038:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_003d:  endfinally
      IL_003e:  endfinally
    }  
    IL_003f:  ldloc.2
    IL_0040:  pop
    IL_0041:  ldloca.s   V_0
    IL_0043:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0048:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!a> f2<a>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,!!a> f, class [runtime]System.Collections.Generic.IEnumerable`1<int32> seq) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<!!a> V_0,
             class [runtime]System.Collections.Generic.IEnumerator`1<int32> V_1,
             class [runtime]System.Collections.Generic.IEnumerable`1<!!a> V_2,
             int32 V_3,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<!!a>& V_4,
             class [runtime]System.IDisposable V_5)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_0024

      IL_000a:  ldloc.1
      IL_000b:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0010:  stloc.3
      IL_0011:  ldloca.s   V_0
      IL_0013:  stloc.s    V_4
      IL_0015:  ldloc.s    V_4
      IL_0017:  ldarg.0
      IL_0018:  ldloc.3
      IL_0019:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,!!a>::Invoke(!0)
      IL_001e:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<!!a>::Add(!0)
      IL_0023:  nop
      IL_0024:  ldloc.1
      IL_0025:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_002a:  brtrue.s   IL_000a

      IL_002c:  ldnull
      IL_002d:  stloc.2
      IL_002e:  leave.s    IL_0045

    }  
    finally
    {
      IL_0030:  ldloc.1
      IL_0031:  isinst     [runtime]System.IDisposable
      IL_0036:  stloc.s    V_5
      IL_0038:  ldloc.s    V_5
      IL_003a:  brfalse.s  IL_0044

      IL_003c:  ldloc.s    V_5
      IL_003e:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_0043:  endfinally
      IL_0044:  endfinally
    }  
    IL_0045:  ldloc.2
    IL_0046:  pop
    IL_0047:  ldloca.s   V_0
    IL_0049:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<!!a>::Close()
    IL_004e:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f3(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> f, class [runtime]System.Collections.Generic.IEnumerable`1<int32> seq) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             class [runtime]System.Collections.Generic.IEnumerator`1<int32> V_1,
             class [runtime]System.Collections.Generic.IEnumerable`1<int32> V_2,
             int32 V_3,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_4,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_5,
             class [runtime]System.IDisposable V_6)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_002a

      IL_000a:  ldloc.1
      IL_000b:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0010:  stloc.3
      IL_0011:  ldloca.s   V_0
      IL_0013:  stloc.s    V_4
      IL_0015:  ldloc.s    V_4
      IL_0017:  ldarg.0
      IL_0018:  ldnull
      IL_0019:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
      IL_001e:  pop
      IL_001f:  stloc.s    V_5
      IL_0021:  ldloc.s    V_5
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
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_4,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_5,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_6,
             class [runtime]System.IDisposable V_7)
    IL_0000:  nop
    IL_0001:  ldarg.2
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_0036

      IL_000a:  ldloc.1
      IL_000b:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0010:  stloc.3
      IL_0011:  ldloca.s   V_0
      IL_0013:  stloc.s    V_4
      IL_0015:  ldloc.s    V_4
      IL_0017:  ldarg.0
      IL_0018:  ldnull
      IL_0019:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
      IL_001e:  pop
      IL_001f:  stloc.s    V_5
      IL_0021:  ldloc.s    V_5
      IL_0023:  ldarg.1
      IL_0024:  ldnull
      IL_0025:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
      IL_002a:  pop
      IL_002b:  stloc.s    V_6
      IL_002d:  ldloc.s    V_6
      IL_002f:  ldloc.3
      IL_0030:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0035:  nop
      IL_0036:  ldloc.1
      IL_0037:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_003c:  brtrue.s   IL_000a

      IL_003e:  ldnull
      IL_003f:  stloc.2
      IL_0040:  leave.s    IL_0057

    }  
    finally
    {
      IL_0042:  ldloc.1
      IL_0043:  isinst     [runtime]System.IDisposable
      IL_0048:  stloc.s    V_7
      IL_004a:  ldloc.s    V_7
      IL_004c:  brfalse.s  IL_0056

      IL_004e:  ldloc.s    V_7
      IL_0050:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_0055:  endfinally
      IL_0056:  endfinally
    }  
    IL_0057:  ldloc.2
    IL_0058:  pop
    IL_0059:  ldloca.s   V_0
    IL_005b:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0060:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f5(class [runtime]System.Collections.Generic.IEnumerable`1<int32> seq) cil managed
  {
    
    .maxstack  4
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             class [runtime]System.Collections.Generic.IEnumerator`1<int32> V_1,
             class [runtime]System.Collections.Generic.IEnumerable`1<int32> V_2,
             int32 V_3,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_4,
             class [runtime]System.IDisposable V_5)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_001e

      IL_000a:  ldloc.1
      IL_000b:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0010:  stloc.3
      IL_0011:  ldloca.s   V_0
      IL_0013:  stloc.s    V_4
      IL_0015:  ldloc.s    V_4
      IL_0017:  ldloc.3
      IL_0018:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_001d:  nop
      IL_001e:  ldloc.1
      IL_001f:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0024:  brtrue.s   IL_000a

      IL_0026:  ldnull
      IL_0027:  stloc.2
      IL_0028:  leave.s    IL_003f

    }  
    finally
    {
      IL_002a:  ldloc.1
      IL_002b:  isinst     [runtime]System.IDisposable
      IL_0030:  stloc.s    V_5
      IL_0032:  ldloc.s    V_5
      IL_0034:  brfalse.s  IL_003e

      IL_0036:  ldloc.s    V_5
      IL_0038:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_003d:  endfinally
      IL_003e:  endfinally
    }  
    IL_003f:  ldloc.2
    IL_0040:  pop
    IL_0041:  ldloca.s   V_0
    IL_0043:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0048:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f6(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> f, class [runtime]System.Collections.Generic.IEnumerable`1<int32> seq) cil managed
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
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_6,
             class [runtime]System.IDisposable V_7)
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
      IL_0021:  stloc.s    V_5
      IL_0023:  ldloca.s   V_0
      IL_0025:  stloc.s    V_6
      IL_0027:  ldloc.s    V_6
      IL_0029:  ldloc.s    V_5
      IL_002b:  ldloc.1
      IL_002c:  add
      IL_002d:  ldloc.2
      IL_002e:  add
      IL_002f:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0034:  nop
      IL_0035:  ldloc.3
      IL_0036:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_003b:  brtrue.s   IL_001b

      IL_003d:  ldnull
      IL_003e:  stloc.s    V_4
      IL_0040:  leave.s    IL_0057

    }  
    finally
    {
      IL_0042:  ldloc.3
      IL_0043:  isinst     [runtime]System.IDisposable
      IL_0048:  stloc.s    V_7
      IL_004a:  ldloc.s    V_7
      IL_004c:  brfalse.s  IL_0056

      IL_004e:  ldloc.s    V_7
      IL_0050:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_0055:  endfinally
      IL_0056:  endfinally
    }  
    IL_0057:  ldloc.s    V_4
    IL_0059:  pop
    IL_005a:  ldloca.s   V_0
    IL_005c:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0061:  ret
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
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_5,
             class [runtime]System.IDisposable V_6)
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
      IL_0019:  br.s       IL_0033

      IL_001b:  ldloc.2
      IL_001c:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0021:  stloc.s    V_4
      IL_0023:  ldloca.s   V_0
      IL_0025:  stloc.s    V_5
      IL_0027:  ldloc.s    V_5
      IL_0029:  ldloc.s    V_4
      IL_002b:  ldloc.1
      IL_002c:  add
      IL_002d:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0032:  nop
      IL_0033:  ldloc.2
      IL_0034:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0039:  brtrue.s   IL_001b

      IL_003b:  ldnull
      IL_003c:  stloc.3
      IL_003d:  leave.s    IL_0054

    }  
    finally
    {
      IL_003f:  ldloc.2
      IL_0040:  isinst     [runtime]System.IDisposable
      IL_0045:  stloc.s    V_6
      IL_0047:  ldloc.s    V_6
      IL_0049:  brfalse.s  IL_0053

      IL_004b:  ldloc.s    V_6
      IL_004d:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_0052:  endfinally
      IL_0053:  endfinally
    }  
    IL_0054:  ldloc.3
    IL_0055:  pop
    IL_0056:  ldloca.s   V_0
    IL_0058:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_005d:  ret
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
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_4,
             class [runtime]System.IDisposable V_5)
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
      IL_0019:  br.s       IL_002f

      IL_001b:  ldloc.1
      IL_001c:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0021:  stloc.3
      IL_0022:  ldloca.s   V_0
      IL_0024:  stloc.s    V_4
      IL_0026:  ldloc.s    V_4
      IL_0028:  ldloc.3
      IL_0029:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_002e:  nop
      IL_002f:  ldloc.1
      IL_0030:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0035:  brtrue.s   IL_001b

      IL_0037:  ldnull
      IL_0038:  stloc.2
      IL_0039:  leave.s    IL_0050

    }  
    finally
    {
      IL_003b:  ldloc.1
      IL_003c:  isinst     [runtime]System.IDisposable
      IL_0041:  stloc.s    V_5
      IL_0043:  ldloc.s    V_5
      IL_0045:  brfalse.s  IL_004f

      IL_0047:  ldloc.s    V_5
      IL_0049:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_004e:  endfinally
      IL_004f:  endfinally
    }  
    IL_0050:  ldloc.2
    IL_0051:  pop
    IL_0052:  ldloca.s   V_0
    IL_0054:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0059:  ret
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
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_5,
             class [runtime]System.IDisposable V_6)
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
      IL_0019:  br.s       IL_0033

      IL_001b:  ldloc.2
      IL_001c:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0021:  stloc.s    V_4
      IL_0023:  ldloca.s   V_0
      IL_0025:  stloc.s    V_5
      IL_0027:  ldloc.s    V_5
      IL_0029:  ldloc.s    V_4
      IL_002b:  ldloc.1
      IL_002c:  add
      IL_002d:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0032:  nop
      IL_0033:  ldloc.2
      IL_0034:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0039:  brtrue.s   IL_001b

      IL_003b:  ldnull
      IL_003c:  stloc.3
      IL_003d:  leave.s    IL_0054

    }  
    finally
    {
      IL_003f:  ldloc.2
      IL_0040:  isinst     [runtime]System.IDisposable
      IL_0045:  stloc.s    V_6
      IL_0047:  ldloc.s    V_6
      IL_0049:  brfalse.s  IL_0053

      IL_004b:  ldloc.s    V_6
      IL_004d:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_0052:  endfinally
      IL_0053:  endfinally
    }  
    IL_0054:  ldloc.3
    IL_0055:  pop
    IL_0056:  ldloca.s   V_0
    IL_0058:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_005d:  ret
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






