




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
  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f1() cil managed
  {
    
    .maxstack  4
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             uint64 V_1,
             int32 V_2)
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stloc.1
    IL_0003:  ldc.i4.1
    IL_0004:  stloc.2
    IL_0005:  br.s       IL_0019

    IL_0007:  ldloca.s   V_0
    IL_0009:  ldloc.2
    IL_000a:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_000f:  nop
    IL_0010:  ldloc.2
    IL_0011:  ldc.i4.1
    IL_0012:  add
    IL_0013:  stloc.2
    IL_0014:  ldloc.1
    IL_0015:  ldc.i4.1
    IL_0016:  conv.i8
    IL_0017:  add
    IL_0018:  stloc.1
    IL_0019:  ldloc.1
    IL_001a:  ldc.i4.s   10
    IL_001c:  conv.i8
    IL_001d:  blt.un.s   IL_0007

    IL_001f:  ldloca.s   V_0
    IL_0021:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0026:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f2() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_0005:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f3() cil managed
  {
    
    .maxstack  4
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             uint64 V_1,
             int32 V_2)
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stloc.1
    IL_0003:  ldc.i4.1
    IL_0004:  stloc.2
    IL_0005:  br.s       IL_0019

    IL_0007:  ldloca.s   V_0
    IL_0009:  ldloc.2
    IL_000a:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_000f:  nop
    IL_0010:  ldloc.2
    IL_0011:  ldc.i4.1
    IL_0012:  add
    IL_0013:  stloc.2
    IL_0014:  ldloc.1
    IL_0015:  ldc.i4.1
    IL_0016:  conv.i8
    IL_0017:  add
    IL_0018:  stloc.1
    IL_0019:  ldloc.1
    IL_001a:  ldc.i4.s   10
    IL_001c:  conv.i8
    IL_001d:  blt.un.s   IL_0007

    IL_001f:  ldloca.s   V_0
    IL_0021:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0026:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f4() cil managed
  {
    
    .maxstack  4
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             uint64 V_1,
             int32 V_2)
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stloc.1
    IL_0003:  ldc.i4.1
    IL_0004:  stloc.2
    IL_0005:  br.s       IL_0019

    IL_0007:  ldloca.s   V_0
    IL_0009:  ldloc.2
    IL_000a:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_000f:  nop
    IL_0010:  ldloc.2
    IL_0011:  ldc.i4.2
    IL_0012:  add
    IL_0013:  stloc.2
    IL_0014:  ldloc.1
    IL_0015:  ldc.i4.1
    IL_0016:  conv.i8
    IL_0017:  add
    IL_0018:  stloc.1
    IL_0019:  ldloc.1
    IL_001a:  ldc.i4.5
    IL_001b:  conv.i8
    IL_001c:  blt.un.s   IL_0007

    IL_001e:  ldloca.s   V_0
    IL_0020:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0025:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f5() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_0005:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f6() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_0005:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f7() cil managed
  {
    
    .maxstack  4
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             uint64 V_1,
             int32 V_2)
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stloc.1
    IL_0003:  ldc.i4.s   10
    IL_0005:  stloc.2
    IL_0006:  br.s       IL_001a

    IL_0008:  ldloca.s   V_0
    IL_000a:  ldloc.2
    IL_000b:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0010:  nop
    IL_0011:  ldloc.2
    IL_0012:  ldc.i4.m1
    IL_0013:  add
    IL_0014:  stloc.2
    IL_0015:  ldloc.1
    IL_0016:  ldc.i4.1
    IL_0017:  conv.i8
    IL_0018:  add
    IL_0019:  stloc.1
    IL_001a:  ldloc.1
    IL_001b:  ldc.i4.s   10
    IL_001d:  conv.i8
    IL_001e:  blt.un.s   IL_0008

    IL_0020:  ldloca.s   V_0
    IL_0022:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0027:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f8() cil managed
  {
    
    .maxstack  4
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             uint64 V_1,
             int32 V_2)
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stloc.1
    IL_0003:  ldc.i4.s   10
    IL_0005:  stloc.2
    IL_0006:  br.s       IL_001b

    IL_0008:  ldloca.s   V_0
    IL_000a:  ldloc.2
    IL_000b:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0010:  nop
    IL_0011:  ldloc.2
    IL_0012:  ldc.i4.s   -2
    IL_0014:  add
    IL_0015:  stloc.2
    IL_0016:  ldloc.1
    IL_0017:  ldc.i4.1
    IL_0018:  conv.i8
    IL_0019:  add
    IL_001a:  stloc.1
    IL_001b:  ldloc.1
    IL_001c:  ldc.i4.5
    IL_001d:  conv.i8
    IL_001e:  blt.un.s   IL_0008

    IL_0020:  ldloca.s   V_0
    IL_0022:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0027:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f9(int32 start) cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_1,
             uint64 V_2,
             int32 V_3)
    IL_0000:  nop
    IL_0001:  ldc.i4.s   10
    IL_0003:  ldarg.0
    IL_0004:  bge.s      IL_000b

    IL_0006:  ldc.i4.0
    IL_0007:  conv.i8
    IL_0008:  nop
    IL_0009:  br.s       IL_0014

    IL_000b:  ldc.i4.s   10
    IL_000d:  ldarg.0
    IL_000e:  sub
    IL_000f:  conv.i8
    IL_0010:  ldc.i4.1
    IL_0011:  conv.i8
    IL_0012:  add
    IL_0013:  nop
    IL_0014:  stloc.0
    IL_0015:  ldc.i4.0
    IL_0016:  conv.i8
    IL_0017:  stloc.2
    IL_0018:  ldarg.0
    IL_0019:  stloc.3
    IL_001a:  br.s       IL_002e

    IL_001c:  ldloca.s   V_1
    IL_001e:  ldloc.3
    IL_001f:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0024:  nop
    IL_0025:  ldloc.3
    IL_0026:  ldc.i4.1
    IL_0027:  add
    IL_0028:  stloc.3
    IL_0029:  ldloc.2
    IL_002a:  ldc.i4.1
    IL_002b:  conv.i8
    IL_002c:  add
    IL_002d:  stloc.2
    IL_002e:  ldloc.2
    IL_002f:  ldloc.0
    IL_0030:  blt.un.s   IL_001c

    IL_0032:  ldloca.s   V_1
    IL_0034:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0039:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f10(int32 finish) cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_1,
             uint64 V_2,
             int32 V_3)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.1
    IL_0003:  bge.s      IL_000a

    IL_0005:  ldc.i4.0
    IL_0006:  conv.i8
    IL_0007:  nop
    IL_0008:  br.s       IL_0012

    IL_000a:  ldarg.0
    IL_000b:  ldc.i4.1
    IL_000c:  sub
    IL_000d:  conv.i8
    IL_000e:  ldc.i4.1
    IL_000f:  conv.i8
    IL_0010:  add
    IL_0011:  nop
    IL_0012:  stloc.0
    IL_0013:  ldc.i4.0
    IL_0014:  conv.i8
    IL_0015:  stloc.2
    IL_0016:  ldc.i4.1
    IL_0017:  stloc.3
    IL_0018:  br.s       IL_002c

    IL_001a:  ldloca.s   V_1
    IL_001c:  ldloc.3
    IL_001d:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0022:  nop
    IL_0023:  ldloc.3
    IL_0024:  ldc.i4.1
    IL_0025:  add
    IL_0026:  stloc.3
    IL_0027:  ldloc.2
    IL_0028:  ldc.i4.1
    IL_0029:  conv.i8
    IL_002a:  add
    IL_002b:  stloc.2
    IL_002c:  ldloc.2
    IL_002d:  ldloc.0
    IL_002e:  blt.un.s   IL_001a

    IL_0030:  ldloca.s   V_1
    IL_0032:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0037:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          f11(int32 start,
              int32 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  4
    .locals init (uint64 V_0,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_1,
             uint64 V_2,
             int32 V_3)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldarg.0
    IL_0003:  bge.s      IL_000a

    IL_0005:  ldc.i4.0
    IL_0006:  conv.i8
    IL_0007:  nop
    IL_0008:  br.s       IL_0012

    IL_000a:  ldarg.1
    IL_000b:  ldarg.0
    IL_000c:  sub
    IL_000d:  conv.i8
    IL_000e:  ldc.i4.1
    IL_000f:  conv.i8
    IL_0010:  add
    IL_0011:  nop
    IL_0012:  stloc.0
    IL_0013:  ldc.i4.0
    IL_0014:  conv.i8
    IL_0015:  stloc.2
    IL_0016:  ldarg.0
    IL_0017:  stloc.3
    IL_0018:  br.s       IL_002c

    IL_001a:  ldloca.s   V_1
    IL_001c:  ldloc.3
    IL_001d:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0022:  nop
    IL_0023:  ldloc.3
    IL_0024:  ldc.i4.1
    IL_0025:  add
    IL_0026:  stloc.3
    IL_0027:  ldloc.2
    IL_0028:  ldc.i4.1
    IL_0029:  conv.i8
    IL_002a:  add
    IL_002b:  stloc.2
    IL_002c:  ldloc.2
    IL_002d:  ldloc.0
    IL_002e:  blt.un.s   IL_001a

    IL_0030:  ldloca.s   V_1
    IL_0032:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0037:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f12(int32 start) cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_1,
             uint64 V_2,
             int32 V_3)
    IL_0000:  nop
    IL_0001:  ldc.i4.s   10
    IL_0003:  ldarg.0
    IL_0004:  bge.s      IL_000b

    IL_0006:  ldc.i4.0
    IL_0007:  conv.i8
    IL_0008:  nop
    IL_0009:  br.s       IL_0014

    IL_000b:  ldc.i4.s   10
    IL_000d:  ldarg.0
    IL_000e:  sub
    IL_000f:  conv.i8
    IL_0010:  ldc.i4.1
    IL_0011:  conv.i8
    IL_0012:  add
    IL_0013:  nop
    IL_0014:  stloc.0
    IL_0015:  ldc.i4.0
    IL_0016:  conv.i8
    IL_0017:  stloc.2
    IL_0018:  ldarg.0
    IL_0019:  stloc.3
    IL_001a:  br.s       IL_002e

    IL_001c:  ldloca.s   V_1
    IL_001e:  ldloc.3
    IL_001f:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0024:  nop
    IL_0025:  ldloc.3
    IL_0026:  ldc.i4.1
    IL_0027:  add
    IL_0028:  stloc.3
    IL_0029:  ldloc.2
    IL_002a:  ldc.i4.1
    IL_002b:  conv.i8
    IL_002c:  add
    IL_002d:  stloc.2
    IL_002e:  ldloc.2
    IL_002f:  ldloc.0
    IL_0030:  blt.un.s   IL_001c

    IL_0032:  ldloca.s   V_1
    IL_0034:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0039:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f13(int32 step) cil managed
  {
    
    .maxstack  5
    .locals init (uint64 V_0,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_1,
             uint64 V_2,
             int32 V_3)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  brtrue.s   IL_0011

    IL_0004:  ldc.i4.1
    IL_0005:  ldarg.0
    IL_0006:  ldc.i4.s   10
    IL_0008:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                           int32,
                                                                                                                                                                           int32)
    IL_000d:  pop
    IL_000e:  nop
    IL_000f:  br.s       IL_0012

    IL_0011:  nop
    IL_0012:  ldc.i4.0
    IL_0013:  ldarg.0
    IL_0014:  bge.s      IL_002d

    IL_0016:  ldc.i4.s   10
    IL_0018:  ldc.i4.1
    IL_0019:  bge.s      IL_0020

    IL_001b:  ldc.i4.0
    IL_001c:  conv.i8
    IL_001d:  nop
    IL_001e:  br.s       IL_0045

    IL_0020:  ldc.i4.s   10
    IL_0022:  ldc.i4.1
    IL_0023:  sub
    IL_0024:  ldarg.0
    IL_0025:  div.un
    IL_0026:  conv.i8
    IL_0027:  ldc.i4.1
    IL_0028:  conv.i8
    IL_0029:  add
    IL_002a:  nop
    IL_002b:  br.s       IL_0045

    IL_002d:  ldc.i4.1
    IL_002e:  ldc.i4.s   10
    IL_0030:  bge.s      IL_0037

    IL_0032:  ldc.i4.0
    IL_0033:  conv.i8
    IL_0034:  nop
    IL_0035:  br.s       IL_0045

    IL_0037:  ldc.i4.1
    IL_0038:  ldc.i4.s   10
    IL_003a:  sub
    IL_003b:  ldarg.0
    IL_003c:  not
    IL_003d:  ldc.i4.1
    IL_003e:  add
    IL_003f:  div.un
    IL_0040:  conv.i8
    IL_0041:  ldc.i4.1
    IL_0042:  conv.i8
    IL_0043:  add
    IL_0044:  nop
    IL_0045:  stloc.0
    IL_0046:  ldc.i4.0
    IL_0047:  conv.i8
    IL_0048:  stloc.2
    IL_0049:  ldc.i4.1
    IL_004a:  stloc.3
    IL_004b:  br.s       IL_005f

    IL_004d:  ldloca.s   V_1
    IL_004f:  ldloc.3
    IL_0050:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0055:  nop
    IL_0056:  ldloc.3
    IL_0057:  ldarg.0
    IL_0058:  add
    IL_0059:  stloc.3
    IL_005a:  ldloc.2
    IL_005b:  ldc.i4.1
    IL_005c:  conv.i8
    IL_005d:  add
    IL_005e:  stloc.2
    IL_005f:  ldloc.2
    IL_0060:  ldloc.0
    IL_0061:  blt.un.s   IL_004d

    IL_0063:  ldloca.s   V_1
    IL_0065:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_006a:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f14(int32 finish) cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_1,
             uint64 V_2,
             int32 V_3)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.1
    IL_0003:  bge.s      IL_000a

    IL_0005:  ldc.i4.0
    IL_0006:  conv.i8
    IL_0007:  nop
    IL_0008:  br.s       IL_0012

    IL_000a:  ldarg.0
    IL_000b:  ldc.i4.1
    IL_000c:  sub
    IL_000d:  conv.i8
    IL_000e:  ldc.i4.1
    IL_000f:  conv.i8
    IL_0010:  add
    IL_0011:  nop
    IL_0012:  stloc.0
    IL_0013:  ldc.i4.0
    IL_0014:  conv.i8
    IL_0015:  stloc.2
    IL_0016:  ldc.i4.1
    IL_0017:  stloc.3
    IL_0018:  br.s       IL_002c

    IL_001a:  ldloca.s   V_1
    IL_001c:  ldloc.3
    IL_001d:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0022:  nop
    IL_0023:  ldloc.3
    IL_0024:  ldc.i4.1
    IL_0025:  add
    IL_0026:  stloc.3
    IL_0027:  ldloc.2
    IL_0028:  ldc.i4.1
    IL_0029:  conv.i8
    IL_002a:  add
    IL_002b:  stloc.2
    IL_002c:  ldloc.2
    IL_002d:  ldloc.0
    IL_002e:  blt.un.s   IL_001a

    IL_0030:  ldloca.s   V_1
    IL_0032:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0037:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          f15(int32 start,
              int32 step) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (uint64 V_0,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_1,
             uint64 V_2,
             int32 V_3)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  brtrue.s   IL_0011

    IL_0004:  ldarg.0
    IL_0005:  ldarg.1
    IL_0006:  ldc.i4.s   10
    IL_0008:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                           int32,
                                                                                                                                                                           int32)
    IL_000d:  pop
    IL_000e:  nop
    IL_000f:  br.s       IL_0012

    IL_0011:  nop
    IL_0012:  ldc.i4.0
    IL_0013:  ldarg.1
    IL_0014:  bge.s      IL_002d

    IL_0016:  ldc.i4.s   10
    IL_0018:  ldarg.0
    IL_0019:  bge.s      IL_0020

    IL_001b:  ldc.i4.0
    IL_001c:  conv.i8
    IL_001d:  nop
    IL_001e:  br.s       IL_0045

    IL_0020:  ldc.i4.s   10
    IL_0022:  ldarg.0
    IL_0023:  sub
    IL_0024:  ldarg.1
    IL_0025:  div.un
    IL_0026:  conv.i8
    IL_0027:  ldc.i4.1
    IL_0028:  conv.i8
    IL_0029:  add
    IL_002a:  nop
    IL_002b:  br.s       IL_0045

    IL_002d:  ldarg.0
    IL_002e:  ldc.i4.s   10
    IL_0030:  bge.s      IL_0037

    IL_0032:  ldc.i4.0
    IL_0033:  conv.i8
    IL_0034:  nop
    IL_0035:  br.s       IL_0045

    IL_0037:  ldarg.0
    IL_0038:  ldc.i4.s   10
    IL_003a:  sub
    IL_003b:  ldarg.1
    IL_003c:  not
    IL_003d:  ldc.i4.1
    IL_003e:  add
    IL_003f:  div.un
    IL_0040:  conv.i8
    IL_0041:  ldc.i4.1
    IL_0042:  conv.i8
    IL_0043:  add
    IL_0044:  nop
    IL_0045:  stloc.0
    IL_0046:  ldc.i4.0
    IL_0047:  conv.i8
    IL_0048:  stloc.2
    IL_0049:  ldarg.0
    IL_004a:  stloc.3
    IL_004b:  br.s       IL_005f

    IL_004d:  ldloca.s   V_1
    IL_004f:  ldloc.3
    IL_0050:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0055:  nop
    IL_0056:  ldloc.3
    IL_0057:  ldarg.1
    IL_0058:  add
    IL_0059:  stloc.3
    IL_005a:  ldloc.2
    IL_005b:  ldc.i4.1
    IL_005c:  conv.i8
    IL_005d:  add
    IL_005e:  stloc.2
    IL_005f:  ldloc.2
    IL_0060:  ldloc.0
    IL_0061:  blt.un.s   IL_004d

    IL_0063:  ldloca.s   V_1
    IL_0065:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_006a:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          f16(int32 start,
              int32 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  4
    .locals init (uint64 V_0,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_1,
             uint64 V_2,
             int32 V_3)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldarg.0
    IL_0003:  bge.s      IL_000a

    IL_0005:  ldc.i4.0
    IL_0006:  conv.i8
    IL_0007:  nop
    IL_0008:  br.s       IL_0012

    IL_000a:  ldarg.1
    IL_000b:  ldarg.0
    IL_000c:  sub
    IL_000d:  conv.i8
    IL_000e:  ldc.i4.1
    IL_000f:  conv.i8
    IL_0010:  add
    IL_0011:  nop
    IL_0012:  stloc.0
    IL_0013:  ldc.i4.0
    IL_0014:  conv.i8
    IL_0015:  stloc.2
    IL_0016:  ldarg.0
    IL_0017:  stloc.3
    IL_0018:  br.s       IL_002c

    IL_001a:  ldloca.s   V_1
    IL_001c:  ldloc.3
    IL_001d:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0022:  nop
    IL_0023:  ldloc.3
    IL_0024:  ldc.i4.1
    IL_0025:  add
    IL_0026:  stloc.3
    IL_0027:  ldloc.2
    IL_0028:  ldc.i4.1
    IL_0029:  conv.i8
    IL_002a:  add
    IL_002b:  stloc.2
    IL_002c:  ldloc.2
    IL_002d:  ldloc.0
    IL_002e:  blt.un.s   IL_001a

    IL_0030:  ldloca.s   V_1
    IL_0032:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0037:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          f17(int32 step,
              int32 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (uint64 V_0,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_1,
             uint64 V_2,
             int32 V_3)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  brtrue.s   IL_0010

    IL_0004:  ldc.i4.1
    IL_0005:  ldarg.0
    IL_0006:  ldarg.1
    IL_0007:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                           int32,
                                                                                                                                                                           int32)
    IL_000c:  pop
    IL_000d:  nop
    IL_000e:  br.s       IL_0011

    IL_0010:  nop
    IL_0011:  ldc.i4.0
    IL_0012:  ldarg.0
    IL_0013:  bge.s      IL_002a

    IL_0015:  ldarg.1
    IL_0016:  ldc.i4.1
    IL_0017:  bge.s      IL_001e

    IL_0019:  ldc.i4.0
    IL_001a:  conv.i8
    IL_001b:  nop
    IL_001c:  br.s       IL_0040

    IL_001e:  ldarg.1
    IL_001f:  ldc.i4.1
    IL_0020:  sub
    IL_0021:  ldarg.0
    IL_0022:  div.un
    IL_0023:  conv.i8
    IL_0024:  ldc.i4.1
    IL_0025:  conv.i8
    IL_0026:  add
    IL_0027:  nop
    IL_0028:  br.s       IL_0040

    IL_002a:  ldc.i4.1
    IL_002b:  ldarg.1
    IL_002c:  bge.s      IL_0033

    IL_002e:  ldc.i4.0
    IL_002f:  conv.i8
    IL_0030:  nop
    IL_0031:  br.s       IL_0040

    IL_0033:  ldc.i4.1
    IL_0034:  ldarg.1
    IL_0035:  sub
    IL_0036:  ldarg.0
    IL_0037:  not
    IL_0038:  ldc.i4.1
    IL_0039:  add
    IL_003a:  div.un
    IL_003b:  conv.i8
    IL_003c:  ldc.i4.1
    IL_003d:  conv.i8
    IL_003e:  add
    IL_003f:  nop
    IL_0040:  stloc.0
    IL_0041:  ldc.i4.0
    IL_0042:  conv.i8
    IL_0043:  stloc.2
    IL_0044:  ldc.i4.1
    IL_0045:  stloc.3
    IL_0046:  br.s       IL_005a

    IL_0048:  ldloca.s   V_1
    IL_004a:  ldloc.3
    IL_004b:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0050:  nop
    IL_0051:  ldloc.3
    IL_0052:  ldarg.0
    IL_0053:  add
    IL_0054:  stloc.3
    IL_0055:  ldloc.2
    IL_0056:  ldc.i4.1
    IL_0057:  conv.i8
    IL_0058:  add
    IL_0059:  stloc.2
    IL_005a:  ldloc.2
    IL_005b:  ldloc.0
    IL_005c:  blt.un.s   IL_0048

    IL_005e:  ldloca.s   V_1
    IL_0060:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0065:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          f18(int32 start,
              int32 step,
              int32 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  5
    .locals init (uint64 V_0,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_1,
             uint64 V_2,
             int32 V_3)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  brtrue.s   IL_0010

    IL_0004:  ldarg.0
    IL_0005:  ldarg.1
    IL_0006:  ldarg.2
    IL_0007:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                           int32,
                                                                                                                                                                           int32)
    IL_000c:  pop
    IL_000d:  nop
    IL_000e:  br.s       IL_0011

    IL_0010:  nop
    IL_0011:  ldc.i4.0
    IL_0012:  ldarg.1
    IL_0013:  bge.s      IL_002a

    IL_0015:  ldarg.2
    IL_0016:  ldarg.0
    IL_0017:  bge.s      IL_001e

    IL_0019:  ldc.i4.0
    IL_001a:  conv.i8
    IL_001b:  nop
    IL_001c:  br.s       IL_0040

    IL_001e:  ldarg.2
    IL_001f:  ldarg.0
    IL_0020:  sub
    IL_0021:  ldarg.1
    IL_0022:  div.un
    IL_0023:  conv.i8
    IL_0024:  ldc.i4.1
    IL_0025:  conv.i8
    IL_0026:  add
    IL_0027:  nop
    IL_0028:  br.s       IL_0040

    IL_002a:  ldarg.0
    IL_002b:  ldarg.2
    IL_002c:  bge.s      IL_0033

    IL_002e:  ldc.i4.0
    IL_002f:  conv.i8
    IL_0030:  nop
    IL_0031:  br.s       IL_0040

    IL_0033:  ldarg.0
    IL_0034:  ldarg.2
    IL_0035:  sub
    IL_0036:  ldarg.1
    IL_0037:  not
    IL_0038:  ldc.i4.1
    IL_0039:  add
    IL_003a:  div.un
    IL_003b:  conv.i8
    IL_003c:  ldc.i4.1
    IL_003d:  conv.i8
    IL_003e:  add
    IL_003f:  nop
    IL_0040:  stloc.0
    IL_0041:  ldc.i4.0
    IL_0042:  conv.i8
    IL_0043:  stloc.2
    IL_0044:  ldarg.0
    IL_0045:  stloc.3
    IL_0046:  br.s       IL_005a

    IL_0048:  ldloca.s   V_1
    IL_004a:  ldloc.3
    IL_004b:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0050:  nop
    IL_0051:  ldloc.3
    IL_0052:  ldarg.1
    IL_0053:  add
    IL_0054:  stloc.3
    IL_0055:  ldloc.2
    IL_0056:  ldc.i4.1
    IL_0057:  conv.i8
    IL_0058:  add
    IL_0059:  stloc.2
    IL_005a:  ldloc.2
    IL_005b:  ldloc.0
    IL_005c:  blt.un.s   IL_0048

    IL_005e:  ldloca.s   V_1
    IL_0060:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0065:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f19(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> f) cil managed
  {
    
    .maxstack  4
    .locals init (int32 V_0,
             uint64 V_1,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_2,
             uint64 V_3,
             int32 V_4)
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
    IL_0007:  stloc.0
    IL_0008:  ldc.i4.s   10
    IL_000a:  ldloc.0
    IL_000b:  bge.s      IL_0012

    IL_000d:  ldc.i4.0
    IL_000e:  conv.i8
    IL_000f:  nop
    IL_0010:  br.s       IL_001b

    IL_0012:  ldc.i4.s   10
    IL_0014:  ldloc.0
    IL_0015:  sub
    IL_0016:  conv.i8
    IL_0017:  ldc.i4.1
    IL_0018:  conv.i8
    IL_0019:  add
    IL_001a:  nop
    IL_001b:  stloc.1
    IL_001c:  ldc.i4.0
    IL_001d:  conv.i8
    IL_001e:  stloc.3
    IL_001f:  ldloc.0
    IL_0020:  stloc.s    V_4
    IL_0022:  br.s       IL_0039

    IL_0024:  ldloca.s   V_2
    IL_0026:  ldloc.s    V_4
    IL_0028:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_002d:  nop
    IL_002e:  ldloc.s    V_4
    IL_0030:  ldc.i4.1
    IL_0031:  add
    IL_0032:  stloc.s    V_4
    IL_0034:  ldloc.3
    IL_0035:  ldc.i4.1
    IL_0036:  conv.i8
    IL_0037:  add
    IL_0038:  stloc.3
    IL_0039:  ldloc.3
    IL_003a:  ldloc.1
    IL_003b:  blt.un.s   IL_0024

    IL_003d:  ldloca.s   V_2
    IL_003f:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0044:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f20(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> f) cil managed
  {
    
    .maxstack  4
    .locals init (int32 V_0,
             uint64 V_1,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_2,
             uint64 V_3,
             int32 V_4)
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldc.i4.1
    IL_000a:  bge.s      IL_0011

    IL_000c:  ldc.i4.0
    IL_000d:  conv.i8
    IL_000e:  nop
    IL_000f:  br.s       IL_0019

    IL_0011:  ldloc.0
    IL_0012:  ldc.i4.1
    IL_0013:  sub
    IL_0014:  conv.i8
    IL_0015:  ldc.i4.1
    IL_0016:  conv.i8
    IL_0017:  add
    IL_0018:  nop
    IL_0019:  stloc.1
    IL_001a:  ldc.i4.0
    IL_001b:  conv.i8
    IL_001c:  stloc.3
    IL_001d:  ldc.i4.1
    IL_001e:  stloc.s    V_4
    IL_0020:  br.s       IL_0037

    IL_0022:  ldloca.s   V_2
    IL_0024:  ldloc.s    V_4
    IL_0026:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_002b:  nop
    IL_002c:  ldloc.s    V_4
    IL_002e:  ldc.i4.1
    IL_002f:  add
    IL_0030:  stloc.s    V_4
    IL_0032:  ldloc.3
    IL_0033:  ldc.i4.1
    IL_0034:  conv.i8
    IL_0035:  add
    IL_0036:  stloc.3
    IL_0037:  ldloc.3
    IL_0038:  ldloc.1
    IL_0039:  blt.un.s   IL_0022

    IL_003b:  ldloca.s   V_2
    IL_003d:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0042:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          f21(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> f,
              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> g) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  4
    .locals init (int32 V_0,
             int32 V_1,
             uint64 V_2,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_3,
             uint64 V_4,
             int32 V_5)
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
    IL_0007:  stloc.0
    IL_0008:  ldarg.1
    IL_0009:  ldnull
    IL_000a:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
    IL_000f:  stloc.1
    IL_0010:  ldloc.1
    IL_0011:  ldloc.0
    IL_0012:  bge.s      IL_0019

    IL_0014:  ldc.i4.0
    IL_0015:  conv.i8
    IL_0016:  nop
    IL_0017:  br.s       IL_0021

    IL_0019:  ldloc.1
    IL_001a:  ldloc.0
    IL_001b:  sub
    IL_001c:  conv.i8
    IL_001d:  ldc.i4.1
    IL_001e:  conv.i8
    IL_001f:  add
    IL_0020:  nop
    IL_0021:  stloc.2
    IL_0022:  ldc.i4.0
    IL_0023:  conv.i8
    IL_0024:  stloc.s    V_4
    IL_0026:  ldloc.0
    IL_0027:  stloc.s    V_5
    IL_0029:  br.s       IL_0042

    IL_002b:  ldloca.s   V_3
    IL_002d:  ldloc.s    V_5
    IL_002f:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0034:  nop
    IL_0035:  ldloc.s    V_5
    IL_0037:  ldc.i4.1
    IL_0038:  add
    IL_0039:  stloc.s    V_5
    IL_003b:  ldloc.s    V_4
    IL_003d:  ldc.i4.1
    IL_003e:  conv.i8
    IL_003f:  add
    IL_0040:  stloc.s    V_4
    IL_0042:  ldloc.s    V_4
    IL_0044:  ldloc.2
    IL_0045:  blt.un.s   IL_002b

    IL_0047:  ldloca.s   V_3
    IL_0049:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_004e:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f22(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> f) cil managed
  {
    
    .maxstack  4
    .locals init (int32 V_0,
             uint64 V_1,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_2,
             uint64 V_3,
             int32 V_4)
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
    IL_0007:  stloc.0
    IL_0008:  ldc.i4.s   10
    IL_000a:  ldloc.0
    IL_000b:  bge.s      IL_0012

    IL_000d:  ldc.i4.0
    IL_000e:  conv.i8
    IL_000f:  nop
    IL_0010:  br.s       IL_001b

    IL_0012:  ldc.i4.s   10
    IL_0014:  ldloc.0
    IL_0015:  sub
    IL_0016:  conv.i8
    IL_0017:  ldc.i4.1
    IL_0018:  conv.i8
    IL_0019:  add
    IL_001a:  nop
    IL_001b:  stloc.1
    IL_001c:  ldc.i4.0
    IL_001d:  conv.i8
    IL_001e:  stloc.3
    IL_001f:  ldloc.0
    IL_0020:  stloc.s    V_4
    IL_0022:  br.s       IL_0039

    IL_0024:  ldloca.s   V_2
    IL_0026:  ldloc.s    V_4
    IL_0028:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_002d:  nop
    IL_002e:  ldloc.s    V_4
    IL_0030:  ldc.i4.1
    IL_0031:  add
    IL_0032:  stloc.s    V_4
    IL_0034:  ldloc.3
    IL_0035:  ldc.i4.1
    IL_0036:  conv.i8
    IL_0037:  add
    IL_0038:  stloc.3
    IL_0039:  ldloc.3
    IL_003a:  ldloc.1
    IL_003b:  blt.un.s   IL_0024

    IL_003d:  ldloca.s   V_2
    IL_003f:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0044:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f23(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> f) cil managed
  {
    
    .maxstack  5
    .locals init (int32 V_0,
             uint64 V_1,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_2,
             uint64 V_3,
             int32 V_4)
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  brtrue.s   IL_0018

    IL_000b:  ldc.i4.1
    IL_000c:  ldloc.0
    IL_000d:  ldc.i4.s   10
    IL_000f:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                           int32,
                                                                                                                                                                           int32)
    IL_0014:  pop
    IL_0015:  nop
    IL_0016:  br.s       IL_0019

    IL_0018:  nop
    IL_0019:  ldc.i4.0
    IL_001a:  ldloc.0
    IL_001b:  bge.s      IL_0034

    IL_001d:  ldc.i4.s   10
    IL_001f:  ldc.i4.1
    IL_0020:  bge.s      IL_0027

    IL_0022:  ldc.i4.0
    IL_0023:  conv.i8
    IL_0024:  nop
    IL_0025:  br.s       IL_004c

    IL_0027:  ldc.i4.s   10
    IL_0029:  ldc.i4.1
    IL_002a:  sub
    IL_002b:  ldloc.0
    IL_002c:  div.un
    IL_002d:  conv.i8
    IL_002e:  ldc.i4.1
    IL_002f:  conv.i8
    IL_0030:  add
    IL_0031:  nop
    IL_0032:  br.s       IL_004c

    IL_0034:  ldc.i4.1
    IL_0035:  ldc.i4.s   10
    IL_0037:  bge.s      IL_003e

    IL_0039:  ldc.i4.0
    IL_003a:  conv.i8
    IL_003b:  nop
    IL_003c:  br.s       IL_004c

    IL_003e:  ldc.i4.1
    IL_003f:  ldc.i4.s   10
    IL_0041:  sub
    IL_0042:  ldloc.0
    IL_0043:  not
    IL_0044:  ldc.i4.1
    IL_0045:  add
    IL_0046:  div.un
    IL_0047:  conv.i8
    IL_0048:  ldc.i4.1
    IL_0049:  conv.i8
    IL_004a:  add
    IL_004b:  nop
    IL_004c:  stloc.1
    IL_004d:  ldc.i4.0
    IL_004e:  conv.i8
    IL_004f:  stloc.3
    IL_0050:  ldc.i4.1
    IL_0051:  stloc.s    V_4
    IL_0053:  br.s       IL_006a

    IL_0055:  ldloca.s   V_2
    IL_0057:  ldloc.s    V_4
    IL_0059:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_005e:  nop
    IL_005f:  ldloc.s    V_4
    IL_0061:  ldloc.0
    IL_0062:  add
    IL_0063:  stloc.s    V_4
    IL_0065:  ldloc.3
    IL_0066:  ldc.i4.1
    IL_0067:  conv.i8
    IL_0068:  add
    IL_0069:  stloc.3
    IL_006a:  ldloc.3
    IL_006b:  ldloc.1
    IL_006c:  blt.un.s   IL_0055

    IL_006e:  ldloca.s   V_2
    IL_0070:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0075:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f24(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> f) cil managed
  {
    
    .maxstack  4
    .locals init (int32 V_0,
             uint64 V_1,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_2,
             uint64 V_3,
             int32 V_4)
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldc.i4.1
    IL_000a:  bge.s      IL_0011

    IL_000c:  ldc.i4.0
    IL_000d:  conv.i8
    IL_000e:  nop
    IL_000f:  br.s       IL_0019

    IL_0011:  ldloc.0
    IL_0012:  ldc.i4.1
    IL_0013:  sub
    IL_0014:  conv.i8
    IL_0015:  ldc.i4.1
    IL_0016:  conv.i8
    IL_0017:  add
    IL_0018:  nop
    IL_0019:  stloc.1
    IL_001a:  ldc.i4.0
    IL_001b:  conv.i8
    IL_001c:  stloc.3
    IL_001d:  ldc.i4.1
    IL_001e:  stloc.s    V_4
    IL_0020:  br.s       IL_0037

    IL_0022:  ldloca.s   V_2
    IL_0024:  ldloc.s    V_4
    IL_0026:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_002b:  nop
    IL_002c:  ldloc.s    V_4
    IL_002e:  ldc.i4.1
    IL_002f:  add
    IL_0030:  stloc.s    V_4
    IL_0032:  ldloc.3
    IL_0033:  ldc.i4.1
    IL_0034:  conv.i8
    IL_0035:  add
    IL_0036:  stloc.3
    IL_0037:  ldloc.3
    IL_0038:  ldloc.1
    IL_0039:  blt.un.s   IL_0022

    IL_003b:  ldloca.s   V_2
    IL_003d:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0042:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          f25(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> f,
              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> g,
              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> h) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  5
    .locals init (int32 V_0,
             int32 V_1,
             int32 V_2,
             uint64 V_3,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_4,
             uint64 V_5,
             int32 V_6)
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
    IL_0007:  stloc.0
    IL_0008:  ldarg.1
    IL_0009:  ldnull
    IL_000a:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
    IL_000f:  stloc.1
    IL_0010:  ldarg.2
    IL_0011:  ldnull
    IL_0012:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
    IL_0017:  stloc.2
    IL_0018:  ldloc.1
    IL_0019:  brtrue.s   IL_0027

    IL_001b:  ldloc.0
    IL_001c:  ldloc.1
    IL_001d:  ldloc.2
    IL_001e:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                           int32,
                                                                                                                                                                           int32)
    IL_0023:  pop
    IL_0024:  nop
    IL_0025:  br.s       IL_0028

    IL_0027:  nop
    IL_0028:  ldc.i4.0
    IL_0029:  ldloc.1
    IL_002a:  bge.s      IL_0041

    IL_002c:  ldloc.2
    IL_002d:  ldloc.0
    IL_002e:  bge.s      IL_0035

    IL_0030:  ldc.i4.0
    IL_0031:  conv.i8
    IL_0032:  nop
    IL_0033:  br.s       IL_0057

    IL_0035:  ldloc.2
    IL_0036:  ldloc.0
    IL_0037:  sub
    IL_0038:  ldloc.1
    IL_0039:  div.un
    IL_003a:  conv.i8
    IL_003b:  ldc.i4.1
    IL_003c:  conv.i8
    IL_003d:  add
    IL_003e:  nop
    IL_003f:  br.s       IL_0057

    IL_0041:  ldloc.0
    IL_0042:  ldloc.2
    IL_0043:  bge.s      IL_004a

    IL_0045:  ldc.i4.0
    IL_0046:  conv.i8
    IL_0047:  nop
    IL_0048:  br.s       IL_0057

    IL_004a:  ldloc.0
    IL_004b:  ldloc.2
    IL_004c:  sub
    IL_004d:  ldloc.1
    IL_004e:  not
    IL_004f:  ldc.i4.1
    IL_0050:  add
    IL_0051:  div.un
    IL_0052:  conv.i8
    IL_0053:  ldc.i4.1
    IL_0054:  conv.i8
    IL_0055:  add
    IL_0056:  nop
    IL_0057:  stloc.3
    IL_0058:  ldc.i4.0
    IL_0059:  conv.i8
    IL_005a:  stloc.s    V_5
    IL_005c:  ldloc.0
    IL_005d:  stloc.s    V_6
    IL_005f:  br.s       IL_0078

    IL_0061:  ldloca.s   V_4
    IL_0063:  ldloc.s    V_6
    IL_0065:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_006a:  nop
    IL_006b:  ldloc.s    V_6
    IL_006d:  ldloc.1
    IL_006e:  add
    IL_006f:  stloc.s    V_6
    IL_0071:  ldloc.s    V_5
    IL_0073:  ldc.i4.1
    IL_0074:  conv.i8
    IL_0075:  add
    IL_0076:  stloc.s    V_5
    IL_0078:  ldloc.s    V_5
    IL_007a:  ldloc.3
    IL_007b:  blt.un.s   IL_0061

    IL_007d:  ldloca.s   V_4
    IL_007f:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0084:  ret
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






