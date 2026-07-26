




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
  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<uint64> f1() cil managed
  {
    
    .maxstack  4
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64> V_0,
             uint64 V_1,
             uint64 V_2)
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stloc.1
    IL_0003:  ldc.i4.1
    IL_0004:  conv.i8
    IL_0005:  stloc.2
    IL_0006:  br.s       IL_001b

    IL_0008:  ldloca.s   V_0
    IL_000a:  ldloc.2
    IL_000b:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_0010:  nop
    IL_0011:  ldloc.2
    IL_0012:  ldc.i4.1
    IL_0013:  conv.i8
    IL_0014:  add
    IL_0015:  stloc.2
    IL_0016:  ldloc.1
    IL_0017:  ldc.i4.1
    IL_0018:  conv.i8
    IL_0019:  add
    IL_001a:  stloc.1
    IL_001b:  ldloc.1
    IL_001c:  ldc.i4.s   10
    IL_001e:  conv.i8
    IL_001f:  blt.un.s   IL_0008

    IL_0021:  ldloca.s   V_0
    IL_0023:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Close()
    IL_0028:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<uint64> f2() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<uint64>::get_Empty()
    IL_0005:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<uint64> f3() cil managed
  {
    
    .maxstack  4
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64> V_0,
             uint64 V_1,
             uint64 V_2)
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stloc.1
    IL_0003:  ldc.i4.1
    IL_0004:  conv.i8
    IL_0005:  stloc.2
    IL_0006:  br.s       IL_001b

    IL_0008:  ldloca.s   V_0
    IL_000a:  ldloc.2
    IL_000b:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_0010:  nop
    IL_0011:  ldloc.2
    IL_0012:  ldc.i4.1
    IL_0013:  conv.i8
    IL_0014:  add
    IL_0015:  stloc.2
    IL_0016:  ldloc.1
    IL_0017:  ldc.i4.1
    IL_0018:  conv.i8
    IL_0019:  add
    IL_001a:  stloc.1
    IL_001b:  ldloc.1
    IL_001c:  ldc.i4.s   10
    IL_001e:  conv.i8
    IL_001f:  blt.un.s   IL_0008

    IL_0021:  ldloca.s   V_0
    IL_0023:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Close()
    IL_0028:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<uint64> f4() cil managed
  {
    
    .maxstack  4
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64> V_0,
             uint64 V_1,
             uint64 V_2)
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stloc.1
    IL_0003:  ldc.i4.1
    IL_0004:  conv.i8
    IL_0005:  stloc.2
    IL_0006:  br.s       IL_001b

    IL_0008:  ldloca.s   V_0
    IL_000a:  ldloc.2
    IL_000b:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_0010:  nop
    IL_0011:  ldloc.2
    IL_0012:  ldc.i4.2
    IL_0013:  conv.i8
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
    IL_0022:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Close()
    IL_0027:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<uint64> f5() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<uint64>::get_Empty()
    IL_0005:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<uint64> f6(uint64 start) cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64> V_1,
             uint64 V_2,
             uint64 V_3)
    IL_0000:  ldc.i4.s   10
    IL_0002:  conv.i8
    IL_0003:  ldarg.0
    IL_0004:  bge.un.s   IL_000b

    IL_0006:  ldc.i4.0
    IL_0007:  conv.i8
    IL_0008:  nop
    IL_0009:  br.s       IL_0014

    IL_000b:  ldc.i4.s   10
    IL_000d:  conv.i8
    IL_000e:  ldarg.0
    IL_000f:  sub
    IL_0010:  ldc.i4.1
    IL_0011:  conv.i8
    IL_0012:  add.ovf.un
    IL_0013:  nop
    IL_0014:  stloc.0
    IL_0015:  ldc.i4.0
    IL_0016:  conv.i8
    IL_0017:  stloc.2
    IL_0018:  ldarg.0
    IL_0019:  stloc.3
    IL_001a:  br.s       IL_002f

    IL_001c:  ldloca.s   V_1
    IL_001e:  ldloc.3
    IL_001f:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_0024:  nop
    IL_0025:  ldloc.3
    IL_0026:  ldc.i4.1
    IL_0027:  conv.i8
    IL_0028:  add
    IL_0029:  stloc.3
    IL_002a:  ldloc.2
    IL_002b:  ldc.i4.1
    IL_002c:  conv.i8
    IL_002d:  add
    IL_002e:  stloc.2
    IL_002f:  ldloc.2
    IL_0030:  ldloc.0
    IL_0031:  blt.un.s   IL_001c

    IL_0033:  ldloca.s   V_1
    IL_0035:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Close()
    IL_003a:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<uint64> f7(uint64 finish) cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64> V_1,
             uint64 V_2,
             uint64 V_3)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  conv.i8
    IL_0003:  bge.un.s   IL_000a

    IL_0005:  ldc.i4.0
    IL_0006:  conv.i8
    IL_0007:  nop
    IL_0008:  br.s       IL_0012

    IL_000a:  ldarg.0
    IL_000b:  ldc.i4.1
    IL_000c:  conv.i8
    IL_000d:  sub
    IL_000e:  ldc.i4.1
    IL_000f:  conv.i8
    IL_0010:  add.ovf.un
    IL_0011:  nop
    IL_0012:  stloc.0
    IL_0013:  ldc.i4.0
    IL_0014:  conv.i8
    IL_0015:  stloc.2
    IL_0016:  ldc.i4.1
    IL_0017:  conv.i8
    IL_0018:  stloc.3
    IL_0019:  br.s       IL_002e

    IL_001b:  ldloca.s   V_1
    IL_001d:  ldloc.3
    IL_001e:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_0023:  nop
    IL_0024:  ldloc.3
    IL_0025:  ldc.i4.1
    IL_0026:  conv.i8
    IL_0027:  add
    IL_0028:  stloc.3
    IL_0029:  ldloc.2
    IL_002a:  ldc.i4.1
    IL_002b:  conv.i8
    IL_002c:  add
    IL_002d:  stloc.2
    IL_002e:  ldloc.2
    IL_002f:  ldloc.0
    IL_0030:  blt.un.s   IL_001b

    IL_0032:  ldloca.s   V_1
    IL_0034:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Close()
    IL_0039:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<uint64> f8(uint64 start, uint64 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  4
    .locals init (uint64 V_0,
             bool V_1,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64> V_2,
             bool V_3,
             uint64 V_4,
             uint64 V_5,
             uint64 V_6)
    IL_0000:  ldarg.1
    IL_0001:  ldarg.0
    IL_0002:  bge.un.s   IL_0009

    IL_0004:  ldc.i4.0
    IL_0005:  conv.i8
    IL_0006:  nop
    IL_0007:  br.s       IL_000d

    IL_0009:  ldarg.1
    IL_000a:  ldarg.0
    IL_000b:  sub
    IL_000c:  nop
    IL_000d:  stloc.0
    IL_000e:  ldloc.0
    IL_000f:  ldc.i4.m1
    IL_0010:  conv.i8
    IL_0011:  ceq
    IL_0013:  stloc.1
    IL_0014:  ldloc.1
    IL_0015:  brfalse.s  IL_0047

    IL_0017:  ldc.i4.1
    IL_0018:  stloc.3
    IL_0019:  ldc.i4.0
    IL_001a:  conv.i8
    IL_001b:  stloc.s    V_4
    IL_001d:  ldarg.0
    IL_001e:  stloc.s    V_5
    IL_0020:  br.s       IL_0041

    IL_0022:  ldloca.s   V_2
    IL_0024:  ldloc.s    V_5
    IL_0026:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_002b:  nop
    IL_002c:  ldloc.s    V_5
    IL_002e:  ldc.i4.1
    IL_002f:  conv.i8
    IL_0030:  add
    IL_0031:  stloc.s    V_5
    IL_0033:  ldloc.s    V_4
    IL_0035:  ldc.i4.1
    IL_0036:  conv.i8
    IL_0037:  add
    IL_0038:  stloc.s    V_4
    IL_003a:  ldloc.s    V_4
    IL_003c:  ldc.i4.0
    IL_003d:  conv.i8
    IL_003e:  cgt.un
    IL_0040:  stloc.3
    IL_0041:  ldloc.3
    IL_0042:  brtrue.s   IL_0022

    IL_0044:  nop
    IL_0045:  br.s       IL_0081

    IL_0047:  ldarg.1
    IL_0048:  ldarg.0
    IL_0049:  bge.un.s   IL_0050

    IL_004b:  ldc.i4.0
    IL_004c:  conv.i8
    IL_004d:  nop
    IL_004e:  br.s       IL_0057

    IL_0050:  ldarg.1
    IL_0051:  ldarg.0
    IL_0052:  sub
    IL_0053:  ldc.i4.1
    IL_0054:  conv.i8
    IL_0055:  add.ovf.un
    IL_0056:  nop
    IL_0057:  stloc.s    V_4
    IL_0059:  ldc.i4.0
    IL_005a:  conv.i8
    IL_005b:  stloc.s    V_5
    IL_005d:  ldarg.0
    IL_005e:  stloc.s    V_6
    IL_0060:  br.s       IL_007a

    IL_0062:  ldloca.s   V_2
    IL_0064:  ldloc.s    V_6
    IL_0066:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_006b:  nop
    IL_006c:  ldloc.s    V_6
    IL_006e:  ldc.i4.1
    IL_006f:  conv.i8
    IL_0070:  add
    IL_0071:  stloc.s    V_6
    IL_0073:  ldloc.s    V_5
    IL_0075:  ldc.i4.1
    IL_0076:  conv.i8
    IL_0077:  add
    IL_0078:  stloc.s    V_5
    IL_007a:  ldloc.s    V_5
    IL_007c:  ldloc.s    V_4
    IL_007e:  blt.un.s   IL_0062

    IL_0080:  nop
    IL_0081:  ldloca.s   V_2
    IL_0083:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Close()
    IL_0088:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<uint64> f9(uint64 start) cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64> V_1,
             uint64 V_2,
             uint64 V_3)
    IL_0000:  ldc.i4.s   10
    IL_0002:  conv.i8
    IL_0003:  ldarg.0
    IL_0004:  bge.un.s   IL_000b

    IL_0006:  ldc.i4.0
    IL_0007:  conv.i8
    IL_0008:  nop
    IL_0009:  br.s       IL_0014

    IL_000b:  ldc.i4.s   10
    IL_000d:  conv.i8
    IL_000e:  ldarg.0
    IL_000f:  sub
    IL_0010:  ldc.i4.1
    IL_0011:  conv.i8
    IL_0012:  add.ovf.un
    IL_0013:  nop
    IL_0014:  stloc.0
    IL_0015:  ldc.i4.0
    IL_0016:  conv.i8
    IL_0017:  stloc.2
    IL_0018:  ldarg.0
    IL_0019:  stloc.3
    IL_001a:  br.s       IL_002f

    IL_001c:  ldloca.s   V_1
    IL_001e:  ldloc.3
    IL_001f:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_0024:  nop
    IL_0025:  ldloc.3
    IL_0026:  ldc.i4.1
    IL_0027:  conv.i8
    IL_0028:  add
    IL_0029:  stloc.3
    IL_002a:  ldloc.2
    IL_002b:  ldc.i4.1
    IL_002c:  conv.i8
    IL_002d:  add
    IL_002e:  stloc.2
    IL_002f:  ldloc.2
    IL_0030:  ldloc.0
    IL_0031:  blt.un.s   IL_001c

    IL_0033:  ldloca.s   V_1
    IL_0035:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Close()
    IL_003a:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<uint64> f10(uint64 step) cil managed
  {
    
    .maxstack  5
    .locals init (uint64 V_0,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64> V_1,
             uint64 V_2,
             uint64 V_3)
    IL_0000:  ldarg.0
    IL_0001:  brtrue.s   IL_0012

    IL_0003:  ldc.i4.1
    IL_0004:  conv.i8
    IL_0005:  ldarg.0
    IL_0006:  ldc.i4.s   10
    IL_0008:  conv.i8
    IL_0009:  call       class [runtime]System.Collections.Generic.IEnumerable`1<uint64> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeUInt64(uint64,
                                                                                                                                                                             uint64,
                                                                                                                                                                             uint64)
    IL_000e:  pop
    IL_000f:  nop
    IL_0010:  br.s       IL_0013

    IL_0012:  nop
    IL_0013:  ldc.i4.s   10
    IL_0015:  conv.i8
    IL_0016:  ldc.i4.1
    IL_0017:  conv.i8
    IL_0018:  bge.un.s   IL_001f

    IL_001a:  ldc.i4.0
    IL_001b:  conv.i8
    IL_001c:  nop
    IL_001d:  br.s       IL_002b

    IL_001f:  ldc.i4.s   10
    IL_0021:  conv.i8
    IL_0022:  ldc.i4.1
    IL_0023:  conv.i8
    IL_0024:  sub
    IL_0025:  ldarg.0
    IL_0026:  div.un
    IL_0027:  ldc.i4.1
    IL_0028:  conv.i8
    IL_0029:  add.ovf.un
    IL_002a:  nop
    IL_002b:  stloc.0
    IL_002c:  ldc.i4.0
    IL_002d:  conv.i8
    IL_002e:  stloc.2
    IL_002f:  ldc.i4.1
    IL_0030:  conv.i8
    IL_0031:  stloc.3
    IL_0032:  br.s       IL_0046

    IL_0034:  ldloca.s   V_1
    IL_0036:  ldloc.3
    IL_0037:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_003c:  nop
    IL_003d:  ldloc.3
    IL_003e:  ldarg.0
    IL_003f:  add
    IL_0040:  stloc.3
    IL_0041:  ldloc.2
    IL_0042:  ldc.i4.1
    IL_0043:  conv.i8
    IL_0044:  add
    IL_0045:  stloc.2
    IL_0046:  ldloc.2
    IL_0047:  ldloc.0
    IL_0048:  blt.un.s   IL_0034

    IL_004a:  ldloca.s   V_1
    IL_004c:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Close()
    IL_0051:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<uint64> f11(uint64 finish) cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64> V_1,
             uint64 V_2,
             uint64 V_3)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  conv.i8
    IL_0003:  bge.un.s   IL_000a

    IL_0005:  ldc.i4.0
    IL_0006:  conv.i8
    IL_0007:  nop
    IL_0008:  br.s       IL_0012

    IL_000a:  ldarg.0
    IL_000b:  ldc.i4.1
    IL_000c:  conv.i8
    IL_000d:  sub
    IL_000e:  ldc.i4.1
    IL_000f:  conv.i8
    IL_0010:  add.ovf.un
    IL_0011:  nop
    IL_0012:  stloc.0
    IL_0013:  ldc.i4.0
    IL_0014:  conv.i8
    IL_0015:  stloc.2
    IL_0016:  ldc.i4.1
    IL_0017:  conv.i8
    IL_0018:  stloc.3
    IL_0019:  br.s       IL_002e

    IL_001b:  ldloca.s   V_1
    IL_001d:  ldloc.3
    IL_001e:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_0023:  nop
    IL_0024:  ldloc.3
    IL_0025:  ldc.i4.1
    IL_0026:  conv.i8
    IL_0027:  add
    IL_0028:  stloc.3
    IL_0029:  ldloc.2
    IL_002a:  ldc.i4.1
    IL_002b:  conv.i8
    IL_002c:  add
    IL_002d:  stloc.2
    IL_002e:  ldloc.2
    IL_002f:  ldloc.0
    IL_0030:  blt.un.s   IL_001b

    IL_0032:  ldloca.s   V_1
    IL_0034:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Close()
    IL_0039:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<uint64> f12(uint64 start, uint64 step) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (uint64 V_0,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64> V_1,
             uint64 V_2,
             uint64 V_3)
    IL_0000:  ldarg.1
    IL_0001:  brtrue.s   IL_0011

    IL_0003:  ldarg.0
    IL_0004:  ldarg.1
    IL_0005:  ldc.i4.s   10
    IL_0007:  conv.i8
    IL_0008:  call       class [runtime]System.Collections.Generic.IEnumerable`1<uint64> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeUInt64(uint64,
                                                                                                                                                                             uint64,
                                                                                                                                                                             uint64)
    IL_000d:  pop
    IL_000e:  nop
    IL_000f:  br.s       IL_0012

    IL_0011:  nop
    IL_0012:  ldc.i4.s   10
    IL_0014:  conv.i8
    IL_0015:  ldarg.0
    IL_0016:  bge.un.s   IL_001d

    IL_0018:  ldc.i4.0
    IL_0019:  conv.i8
    IL_001a:  nop
    IL_001b:  br.s       IL_0028

    IL_001d:  ldc.i4.s   10
    IL_001f:  conv.i8
    IL_0020:  ldarg.0
    IL_0021:  sub
    IL_0022:  ldarg.1
    IL_0023:  div.un
    IL_0024:  ldc.i4.1
    IL_0025:  conv.i8
    IL_0026:  add.ovf.un
    IL_0027:  nop
    IL_0028:  stloc.0
    IL_0029:  ldc.i4.0
    IL_002a:  conv.i8
    IL_002b:  stloc.2
    IL_002c:  ldarg.0
    IL_002d:  stloc.3
    IL_002e:  br.s       IL_0042

    IL_0030:  ldloca.s   V_1
    IL_0032:  ldloc.3
    IL_0033:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_0038:  nop
    IL_0039:  ldloc.3
    IL_003a:  ldarg.1
    IL_003b:  add
    IL_003c:  stloc.3
    IL_003d:  ldloc.2
    IL_003e:  ldc.i4.1
    IL_003f:  conv.i8
    IL_0040:  add
    IL_0041:  stloc.2
    IL_0042:  ldloc.2
    IL_0043:  ldloc.0
    IL_0044:  blt.un.s   IL_0030

    IL_0046:  ldloca.s   V_1
    IL_0048:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Close()
    IL_004d:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<uint64> f13(uint64 start, uint64 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  4
    .locals init (uint64 V_0,
             bool V_1,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64> V_2,
             bool V_3,
             uint64 V_4,
             uint64 V_5,
             uint64 V_6)
    IL_0000:  ldarg.1
    IL_0001:  ldarg.0
    IL_0002:  bge.un.s   IL_0009

    IL_0004:  ldc.i4.0
    IL_0005:  conv.i8
    IL_0006:  nop
    IL_0007:  br.s       IL_000d

    IL_0009:  ldarg.1
    IL_000a:  ldarg.0
    IL_000b:  sub
    IL_000c:  nop
    IL_000d:  stloc.0
    IL_000e:  ldloc.0
    IL_000f:  ldc.i4.m1
    IL_0010:  conv.i8
    IL_0011:  ceq
    IL_0013:  stloc.1
    IL_0014:  ldloc.1
    IL_0015:  brfalse.s  IL_0047

    IL_0017:  ldc.i4.1
    IL_0018:  stloc.3
    IL_0019:  ldc.i4.0
    IL_001a:  conv.i8
    IL_001b:  stloc.s    V_4
    IL_001d:  ldarg.0
    IL_001e:  stloc.s    V_5
    IL_0020:  br.s       IL_0041

    IL_0022:  ldloca.s   V_2
    IL_0024:  ldloc.s    V_5
    IL_0026:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_002b:  nop
    IL_002c:  ldloc.s    V_5
    IL_002e:  ldc.i4.1
    IL_002f:  conv.i8
    IL_0030:  add
    IL_0031:  stloc.s    V_5
    IL_0033:  ldloc.s    V_4
    IL_0035:  ldc.i4.1
    IL_0036:  conv.i8
    IL_0037:  add
    IL_0038:  stloc.s    V_4
    IL_003a:  ldloc.s    V_4
    IL_003c:  ldc.i4.0
    IL_003d:  conv.i8
    IL_003e:  cgt.un
    IL_0040:  stloc.3
    IL_0041:  ldloc.3
    IL_0042:  brtrue.s   IL_0022

    IL_0044:  nop
    IL_0045:  br.s       IL_0081

    IL_0047:  ldarg.1
    IL_0048:  ldarg.0
    IL_0049:  bge.un.s   IL_0050

    IL_004b:  ldc.i4.0
    IL_004c:  conv.i8
    IL_004d:  nop
    IL_004e:  br.s       IL_0057

    IL_0050:  ldarg.1
    IL_0051:  ldarg.0
    IL_0052:  sub
    IL_0053:  ldc.i4.1
    IL_0054:  conv.i8
    IL_0055:  add.ovf.un
    IL_0056:  nop
    IL_0057:  stloc.s    V_4
    IL_0059:  ldc.i4.0
    IL_005a:  conv.i8
    IL_005b:  stloc.s    V_5
    IL_005d:  ldarg.0
    IL_005e:  stloc.s    V_6
    IL_0060:  br.s       IL_007a

    IL_0062:  ldloca.s   V_2
    IL_0064:  ldloc.s    V_6
    IL_0066:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_006b:  nop
    IL_006c:  ldloc.s    V_6
    IL_006e:  ldc.i4.1
    IL_006f:  conv.i8
    IL_0070:  add
    IL_0071:  stloc.s    V_6
    IL_0073:  ldloc.s    V_5
    IL_0075:  ldc.i4.1
    IL_0076:  conv.i8
    IL_0077:  add
    IL_0078:  stloc.s    V_5
    IL_007a:  ldloc.s    V_5
    IL_007c:  ldloc.s    V_4
    IL_007e:  blt.un.s   IL_0062

    IL_0080:  nop
    IL_0081:  ldloca.s   V_2
    IL_0083:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Close()
    IL_0088:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<uint64> f14(uint64 step, uint64 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (uint64 V_0,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64> V_1,
             uint64 V_2,
             uint64 V_3)
    IL_0000:  ldarg.0
    IL_0001:  brtrue.s   IL_0010

    IL_0003:  ldc.i4.1
    IL_0004:  conv.i8
    IL_0005:  ldarg.0
    IL_0006:  ldarg.1
    IL_0007:  call       class [runtime]System.Collections.Generic.IEnumerable`1<uint64> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeUInt64(uint64,
                                                                                                                                                                             uint64,
                                                                                                                                                                             uint64)
    IL_000c:  pop
    IL_000d:  nop
    IL_000e:  br.s       IL_0011

    IL_0010:  nop
    IL_0011:  ldarg.1
    IL_0012:  ldc.i4.1
    IL_0013:  conv.i8
    IL_0014:  bge.un.s   IL_001b

    IL_0016:  ldc.i4.0
    IL_0017:  conv.i8
    IL_0018:  nop
    IL_0019:  br.s       IL_0025

    IL_001b:  ldarg.1
    IL_001c:  ldc.i4.1
    IL_001d:  conv.i8
    IL_001e:  sub
    IL_001f:  ldarg.0
    IL_0020:  div.un
    IL_0021:  ldc.i4.1
    IL_0022:  conv.i8
    IL_0023:  add.ovf.un
    IL_0024:  nop
    IL_0025:  stloc.0
    IL_0026:  ldc.i4.0
    IL_0027:  conv.i8
    IL_0028:  stloc.2
    IL_0029:  ldc.i4.1
    IL_002a:  conv.i8
    IL_002b:  stloc.3
    IL_002c:  br.s       IL_0040

    IL_002e:  ldloca.s   V_1
    IL_0030:  ldloc.3
    IL_0031:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_0036:  nop
    IL_0037:  ldloc.3
    IL_0038:  ldarg.0
    IL_0039:  add
    IL_003a:  stloc.3
    IL_003b:  ldloc.2
    IL_003c:  ldc.i4.1
    IL_003d:  conv.i8
    IL_003e:  add
    IL_003f:  stloc.2
    IL_0040:  ldloc.2
    IL_0041:  ldloc.0
    IL_0042:  blt.un.s   IL_002e

    IL_0044:  ldloca.s   V_1
    IL_0046:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Close()
    IL_004b:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<uint64> 
          f15(uint64 start,
              uint64 step,
              uint64 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  5
    .locals init (uint64 V_0,
             bool V_1,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64> V_2,
             bool V_3,
             uint64 V_4,
             uint64 V_5,
             uint64 V_6)
    IL_0000:  ldarg.1
    IL_0001:  brtrue.s   IL_000f

    IL_0003:  ldarg.0
    IL_0004:  ldarg.1
    IL_0005:  ldarg.2
    IL_0006:  call       class [runtime]System.Collections.Generic.IEnumerable`1<uint64> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeUInt64(uint64,
                                                                                                                                                                             uint64,
                                                                                                                                                                             uint64)
    IL_000b:  pop
    IL_000c:  nop
    IL_000d:  br.s       IL_0010

    IL_000f:  nop
    IL_0010:  ldarg.2
    IL_0011:  ldarg.0
    IL_0012:  bge.un.s   IL_0019

    IL_0014:  ldc.i4.0
    IL_0015:  conv.i8
    IL_0016:  nop
    IL_0017:  br.s       IL_001f

    IL_0019:  ldarg.2
    IL_001a:  ldarg.0
    IL_001b:  sub
    IL_001c:  ldarg.1
    IL_001d:  div.un
    IL_001e:  nop
    IL_001f:  stloc.0
    IL_0020:  ldloc.0
    IL_0021:  ldc.i4.m1
    IL_0022:  conv.i8
    IL_0023:  ceq
    IL_0025:  stloc.1
    IL_0026:  ldloc.1
    IL_0027:  brfalse.s  IL_0058

    IL_0029:  ldc.i4.1
    IL_002a:  stloc.3
    IL_002b:  ldc.i4.0
    IL_002c:  conv.i8
    IL_002d:  stloc.s    V_4
    IL_002f:  ldarg.0
    IL_0030:  stloc.s    V_5
    IL_0032:  br.s       IL_0052

    IL_0034:  ldloca.s   V_2
    IL_0036:  ldloc.s    V_5
    IL_0038:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_003d:  nop
    IL_003e:  ldloc.s    V_5
    IL_0040:  ldarg.1
    IL_0041:  add
    IL_0042:  stloc.s    V_5
    IL_0044:  ldloc.s    V_4
    IL_0046:  ldc.i4.1
    IL_0047:  conv.i8
    IL_0048:  add
    IL_0049:  stloc.s    V_4
    IL_004b:  ldloc.s    V_4
    IL_004d:  ldc.i4.0
    IL_004e:  conv.i8
    IL_004f:  cgt.un
    IL_0051:  stloc.3
    IL_0052:  ldloc.3
    IL_0053:  brtrue.s   IL_0034

    IL_0055:  nop
    IL_0056:  br.s       IL_0093

    IL_0058:  ldarg.2
    IL_0059:  ldarg.0
    IL_005a:  bge.un.s   IL_0061

    IL_005c:  ldc.i4.0
    IL_005d:  conv.i8
    IL_005e:  nop
    IL_005f:  br.s       IL_006a

    IL_0061:  ldarg.2
    IL_0062:  ldarg.0
    IL_0063:  sub
    IL_0064:  ldarg.1
    IL_0065:  div.un
    IL_0066:  ldc.i4.1
    IL_0067:  conv.i8
    IL_0068:  add.ovf.un
    IL_0069:  nop
    IL_006a:  stloc.s    V_4
    IL_006c:  ldc.i4.0
    IL_006d:  conv.i8
    IL_006e:  stloc.s    V_5
    IL_0070:  ldarg.0
    IL_0071:  stloc.s    V_6
    IL_0073:  br.s       IL_008c

    IL_0075:  ldloca.s   V_2
    IL_0077:  ldloc.s    V_6
    IL_0079:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_007e:  nop
    IL_007f:  ldloc.s    V_6
    IL_0081:  ldarg.1
    IL_0082:  add
    IL_0083:  stloc.s    V_6
    IL_0085:  ldloc.s    V_5
    IL_0087:  ldc.i4.1
    IL_0088:  conv.i8
    IL_0089:  add
    IL_008a:  stloc.s    V_5
    IL_008c:  ldloc.s    V_5
    IL_008e:  ldloc.s    V_4
    IL_0090:  blt.un.s   IL_0075

    IL_0092:  nop
    IL_0093:  ldloca.s   V_2
    IL_0095:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Close()
    IL_009a:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<uint64> f16(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,uint64> f) cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             uint64 V_1,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64> V_2,
             uint64 V_3,
             uint64 V_4)
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,uint64>::Invoke(!0)
    IL_0007:  stloc.0
    IL_0008:  ldc.i4.s   10
    IL_000a:  conv.i8
    IL_000b:  ldloc.0
    IL_000c:  bge.un.s   IL_0013

    IL_000e:  ldc.i4.0
    IL_000f:  conv.i8
    IL_0010:  nop
    IL_0011:  br.s       IL_001c

    IL_0013:  ldc.i4.s   10
    IL_0015:  conv.i8
    IL_0016:  ldloc.0
    IL_0017:  sub
    IL_0018:  ldc.i4.1
    IL_0019:  conv.i8
    IL_001a:  add.ovf.un
    IL_001b:  nop
    IL_001c:  stloc.1
    IL_001d:  ldc.i4.0
    IL_001e:  conv.i8
    IL_001f:  stloc.3
    IL_0020:  ldloc.0
    IL_0021:  stloc.s    V_4
    IL_0023:  br.s       IL_003b

    IL_0025:  ldloca.s   V_2
    IL_0027:  ldloc.s    V_4
    IL_0029:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_002e:  nop
    IL_002f:  ldloc.s    V_4
    IL_0031:  ldc.i4.1
    IL_0032:  conv.i8
    IL_0033:  add
    IL_0034:  stloc.s    V_4
    IL_0036:  ldloc.3
    IL_0037:  ldc.i4.1
    IL_0038:  conv.i8
    IL_0039:  add
    IL_003a:  stloc.3
    IL_003b:  ldloc.3
    IL_003c:  ldloc.1
    IL_003d:  blt.un.s   IL_0025

    IL_003f:  ldloca.s   V_2
    IL_0041:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Close()
    IL_0046:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<uint64> f17(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,uint64> f) cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             uint64 V_1,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64> V_2,
             uint64 V_3,
             uint64 V_4)
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,uint64>::Invoke(!0)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldc.i4.1
    IL_000a:  conv.i8
    IL_000b:  bge.un.s   IL_0012

    IL_000d:  ldc.i4.0
    IL_000e:  conv.i8
    IL_000f:  nop
    IL_0010:  br.s       IL_001a

    IL_0012:  ldloc.0
    IL_0013:  ldc.i4.1
    IL_0014:  conv.i8
    IL_0015:  sub
    IL_0016:  ldc.i4.1
    IL_0017:  conv.i8
    IL_0018:  add.ovf.un
    IL_0019:  nop
    IL_001a:  stloc.1
    IL_001b:  ldc.i4.0
    IL_001c:  conv.i8
    IL_001d:  stloc.3
    IL_001e:  ldc.i4.1
    IL_001f:  conv.i8
    IL_0020:  stloc.s    V_4
    IL_0022:  br.s       IL_003a

    IL_0024:  ldloca.s   V_2
    IL_0026:  ldloc.s    V_4
    IL_0028:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_002d:  nop
    IL_002e:  ldloc.s    V_4
    IL_0030:  ldc.i4.1
    IL_0031:  conv.i8
    IL_0032:  add
    IL_0033:  stloc.s    V_4
    IL_0035:  ldloc.3
    IL_0036:  ldc.i4.1
    IL_0037:  conv.i8
    IL_0038:  add
    IL_0039:  stloc.3
    IL_003a:  ldloc.3
    IL_003b:  ldloc.1
    IL_003c:  blt.un.s   IL_0024

    IL_003e:  ldloca.s   V_2
    IL_0040:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Close()
    IL_0045:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<uint64> f18(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,uint64> f, class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,uint64> g) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  4
    .locals init (uint64 V_0,
             uint64 V_1,
             uint64 V_2,
             bool V_3,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64> V_4,
             bool V_5,
             uint64 V_6,
             uint64 V_7,
             uint64 V_8)
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,uint64>::Invoke(!0)
    IL_0007:  stloc.0
    IL_0008:  ldarg.1
    IL_0009:  ldnull
    IL_000a:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,uint64>::Invoke(!0)
    IL_000f:  stloc.1
    IL_0010:  ldloc.1
    IL_0011:  ldloc.0
    IL_0012:  bge.un.s   IL_0019

    IL_0014:  ldc.i4.0
    IL_0015:  conv.i8
    IL_0016:  nop
    IL_0017:  br.s       IL_001d

    IL_0019:  ldloc.1
    IL_001a:  ldloc.0
    IL_001b:  sub
    IL_001c:  nop
    IL_001d:  stloc.2
    IL_001e:  ldloc.2
    IL_001f:  ldc.i4.m1
    IL_0020:  conv.i8
    IL_0021:  ceq
    IL_0023:  stloc.3
    IL_0024:  ldloc.3
    IL_0025:  brfalse.s  IL_005a

    IL_0027:  ldc.i4.1
    IL_0028:  stloc.s    V_5
    IL_002a:  ldc.i4.0
    IL_002b:  conv.i8
    IL_002c:  stloc.s    V_6
    IL_002e:  ldloc.0
    IL_002f:  stloc.s    V_7
    IL_0031:  br.s       IL_0053

    IL_0033:  ldloca.s   V_4
    IL_0035:  ldloc.s    V_7
    IL_0037:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_003c:  nop
    IL_003d:  ldloc.s    V_7
    IL_003f:  ldc.i4.1
    IL_0040:  conv.i8
    IL_0041:  add
    IL_0042:  stloc.s    V_7
    IL_0044:  ldloc.s    V_6
    IL_0046:  ldc.i4.1
    IL_0047:  conv.i8
    IL_0048:  add
    IL_0049:  stloc.s    V_6
    IL_004b:  ldloc.s    V_6
    IL_004d:  ldc.i4.0
    IL_004e:  conv.i8
    IL_004f:  cgt.un
    IL_0051:  stloc.s    V_5
    IL_0053:  ldloc.s    V_5
    IL_0055:  brtrue.s   IL_0033

    IL_0057:  nop
    IL_0058:  br.s       IL_0094

    IL_005a:  ldloc.1
    IL_005b:  ldloc.0
    IL_005c:  bge.un.s   IL_0063

    IL_005e:  ldc.i4.0
    IL_005f:  conv.i8
    IL_0060:  nop
    IL_0061:  br.s       IL_006a

    IL_0063:  ldloc.1
    IL_0064:  ldloc.0
    IL_0065:  sub
    IL_0066:  ldc.i4.1
    IL_0067:  conv.i8
    IL_0068:  add.ovf.un
    IL_0069:  nop
    IL_006a:  stloc.s    V_6
    IL_006c:  ldc.i4.0
    IL_006d:  conv.i8
    IL_006e:  stloc.s    V_7
    IL_0070:  ldloc.0
    IL_0071:  stloc.s    V_8
    IL_0073:  br.s       IL_008d

    IL_0075:  ldloca.s   V_4
    IL_0077:  ldloc.s    V_8
    IL_0079:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_007e:  nop
    IL_007f:  ldloc.s    V_8
    IL_0081:  ldc.i4.1
    IL_0082:  conv.i8
    IL_0083:  add
    IL_0084:  stloc.s    V_8
    IL_0086:  ldloc.s    V_7
    IL_0088:  ldc.i4.1
    IL_0089:  conv.i8
    IL_008a:  add
    IL_008b:  stloc.s    V_7
    IL_008d:  ldloc.s    V_7
    IL_008f:  ldloc.s    V_6
    IL_0091:  blt.un.s   IL_0075

    IL_0093:  nop
    IL_0094:  ldloca.s   V_4
    IL_0096:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Close()
    IL_009b:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<uint64> f19(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,uint64> f) cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             uint64 V_1,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64> V_2,
             uint64 V_3,
             uint64 V_4)
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,uint64>::Invoke(!0)
    IL_0007:  stloc.0
    IL_0008:  ldc.i4.s   10
    IL_000a:  conv.i8
    IL_000b:  ldloc.0
    IL_000c:  bge.un.s   IL_0013

    IL_000e:  ldc.i4.0
    IL_000f:  conv.i8
    IL_0010:  nop
    IL_0011:  br.s       IL_001c

    IL_0013:  ldc.i4.s   10
    IL_0015:  conv.i8
    IL_0016:  ldloc.0
    IL_0017:  sub
    IL_0018:  ldc.i4.1
    IL_0019:  conv.i8
    IL_001a:  add.ovf.un
    IL_001b:  nop
    IL_001c:  stloc.1
    IL_001d:  ldc.i4.0
    IL_001e:  conv.i8
    IL_001f:  stloc.3
    IL_0020:  ldloc.0
    IL_0021:  stloc.s    V_4
    IL_0023:  br.s       IL_003b

    IL_0025:  ldloca.s   V_2
    IL_0027:  ldloc.s    V_4
    IL_0029:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_002e:  nop
    IL_002f:  ldloc.s    V_4
    IL_0031:  ldc.i4.1
    IL_0032:  conv.i8
    IL_0033:  add
    IL_0034:  stloc.s    V_4
    IL_0036:  ldloc.3
    IL_0037:  ldc.i4.1
    IL_0038:  conv.i8
    IL_0039:  add
    IL_003a:  stloc.3
    IL_003b:  ldloc.3
    IL_003c:  ldloc.1
    IL_003d:  blt.un.s   IL_0025

    IL_003f:  ldloca.s   V_2
    IL_0041:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Close()
    IL_0046:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<uint64> f20(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,uint64> f) cil managed
  {
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64> V_2,
             uint64 V_3,
             uint64 V_4)
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,uint64>::Invoke(!0)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  brtrue.s   IL_001a

    IL_000b:  ldc.i4.1
    IL_000c:  conv.i8
    IL_000d:  ldloc.0
    IL_000e:  ldc.i4.s   10
    IL_0010:  conv.i8
    IL_0011:  call       class [runtime]System.Collections.Generic.IEnumerable`1<uint64> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeUInt64(uint64,
                                                                                                                                                                             uint64,
                                                                                                                                                                             uint64)
    IL_0016:  pop
    IL_0017:  nop
    IL_0018:  br.s       IL_001b

    IL_001a:  nop
    IL_001b:  ldc.i4.s   10
    IL_001d:  conv.i8
    IL_001e:  ldc.i4.1
    IL_001f:  conv.i8
    IL_0020:  bge.un.s   IL_0027

    IL_0022:  ldc.i4.0
    IL_0023:  conv.i8
    IL_0024:  nop
    IL_0025:  br.s       IL_0033

    IL_0027:  ldc.i4.s   10
    IL_0029:  conv.i8
    IL_002a:  ldc.i4.1
    IL_002b:  conv.i8
    IL_002c:  sub
    IL_002d:  ldloc.0
    IL_002e:  div.un
    IL_002f:  ldc.i4.1
    IL_0030:  conv.i8
    IL_0031:  add.ovf.un
    IL_0032:  nop
    IL_0033:  stloc.1
    IL_0034:  ldc.i4.0
    IL_0035:  conv.i8
    IL_0036:  stloc.3
    IL_0037:  ldc.i4.1
    IL_0038:  conv.i8
    IL_0039:  stloc.s    V_4
    IL_003b:  br.s       IL_0052

    IL_003d:  ldloca.s   V_2
    IL_003f:  ldloc.s    V_4
    IL_0041:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_0046:  nop
    IL_0047:  ldloc.s    V_4
    IL_0049:  ldloc.0
    IL_004a:  add
    IL_004b:  stloc.s    V_4
    IL_004d:  ldloc.3
    IL_004e:  ldc.i4.1
    IL_004f:  conv.i8
    IL_0050:  add
    IL_0051:  stloc.3
    IL_0052:  ldloc.3
    IL_0053:  ldloc.1
    IL_0054:  blt.un.s   IL_003d

    IL_0056:  ldloca.s   V_2
    IL_0058:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Close()
    IL_005d:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<uint64> f21(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,uint64> f) cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             uint64 V_1,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64> V_2,
             uint64 V_3,
             uint64 V_4)
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,uint64>::Invoke(!0)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldc.i4.1
    IL_000a:  conv.i8
    IL_000b:  bge.un.s   IL_0012

    IL_000d:  ldc.i4.0
    IL_000e:  conv.i8
    IL_000f:  nop
    IL_0010:  br.s       IL_001a

    IL_0012:  ldloc.0
    IL_0013:  ldc.i4.1
    IL_0014:  conv.i8
    IL_0015:  sub
    IL_0016:  ldc.i4.1
    IL_0017:  conv.i8
    IL_0018:  add.ovf.un
    IL_0019:  nop
    IL_001a:  stloc.1
    IL_001b:  ldc.i4.0
    IL_001c:  conv.i8
    IL_001d:  stloc.3
    IL_001e:  ldc.i4.1
    IL_001f:  conv.i8
    IL_0020:  stloc.s    V_4
    IL_0022:  br.s       IL_003a

    IL_0024:  ldloca.s   V_2
    IL_0026:  ldloc.s    V_4
    IL_0028:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_002d:  nop
    IL_002e:  ldloc.s    V_4
    IL_0030:  ldc.i4.1
    IL_0031:  conv.i8
    IL_0032:  add
    IL_0033:  stloc.s    V_4
    IL_0035:  ldloc.3
    IL_0036:  ldc.i4.1
    IL_0037:  conv.i8
    IL_0038:  add
    IL_0039:  stloc.3
    IL_003a:  ldloc.3
    IL_003b:  ldloc.1
    IL_003c:  blt.un.s   IL_0024

    IL_003e:  ldloca.s   V_2
    IL_0040:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Close()
    IL_0045:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<uint64> 
          f22(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,uint64> f,
              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,uint64> g,
              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,uint64> h) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             uint64 V_2,
             uint64 V_3,
             bool V_4,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64> V_5,
             bool V_6,
             uint64 V_7,
             uint64 V_8,
             uint64 V_9)
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,uint64>::Invoke(!0)
    IL_0007:  stloc.0
    IL_0008:  ldarg.1
    IL_0009:  ldnull
    IL_000a:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,uint64>::Invoke(!0)
    IL_000f:  stloc.1
    IL_0010:  ldarg.2
    IL_0011:  ldnull
    IL_0012:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,uint64>::Invoke(!0)
    IL_0017:  stloc.2
    IL_0018:  ldloc.1
    IL_0019:  brtrue.s   IL_0027

    IL_001b:  ldloc.0
    IL_001c:  ldloc.1
    IL_001d:  ldloc.2
    IL_001e:  call       class [runtime]System.Collections.Generic.IEnumerable`1<uint64> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeUInt64(uint64,
                                                                                                                                                                             uint64,
                                                                                                                                                                             uint64)
    IL_0023:  pop
    IL_0024:  nop
    IL_0025:  br.s       IL_0028

    IL_0027:  nop
    IL_0028:  ldloc.2
    IL_0029:  ldloc.0
    IL_002a:  bge.un.s   IL_0031

    IL_002c:  ldc.i4.0
    IL_002d:  conv.i8
    IL_002e:  nop
    IL_002f:  br.s       IL_0037

    IL_0031:  ldloc.2
    IL_0032:  ldloc.0
    IL_0033:  sub
    IL_0034:  ldloc.1
    IL_0035:  div.un
    IL_0036:  nop
    IL_0037:  stloc.3
    IL_0038:  ldloc.3
    IL_0039:  ldc.i4.m1
    IL_003a:  conv.i8
    IL_003b:  ceq
    IL_003d:  stloc.s    V_4
    IL_003f:  ldloc.s    V_4
    IL_0041:  brfalse.s  IL_0075

    IL_0043:  ldc.i4.1
    IL_0044:  stloc.s    V_6
    IL_0046:  ldc.i4.0
    IL_0047:  conv.i8
    IL_0048:  stloc.s    V_7
    IL_004a:  ldloc.0
    IL_004b:  stloc.s    V_8
    IL_004d:  br.s       IL_006e

    IL_004f:  ldloca.s   V_5
    IL_0051:  ldloc.s    V_8
    IL_0053:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_0058:  nop
    IL_0059:  ldloc.s    V_8
    IL_005b:  ldloc.1
    IL_005c:  add
    IL_005d:  stloc.s    V_8
    IL_005f:  ldloc.s    V_7
    IL_0061:  ldc.i4.1
    IL_0062:  conv.i8
    IL_0063:  add
    IL_0064:  stloc.s    V_7
    IL_0066:  ldloc.s    V_7
    IL_0068:  ldc.i4.0
    IL_0069:  conv.i8
    IL_006a:  cgt.un
    IL_006c:  stloc.s    V_6
    IL_006e:  ldloc.s    V_6
    IL_0070:  brtrue.s   IL_004f

    IL_0072:  nop
    IL_0073:  br.s       IL_00b0

    IL_0075:  ldloc.2
    IL_0076:  ldloc.0
    IL_0077:  bge.un.s   IL_007e

    IL_0079:  ldc.i4.0
    IL_007a:  conv.i8
    IL_007b:  nop
    IL_007c:  br.s       IL_0087

    IL_007e:  ldloc.2
    IL_007f:  ldloc.0
    IL_0080:  sub
    IL_0081:  ldloc.1
    IL_0082:  div.un
    IL_0083:  ldc.i4.1
    IL_0084:  conv.i8
    IL_0085:  add.ovf.un
    IL_0086:  nop
    IL_0087:  stloc.s    V_7
    IL_0089:  ldc.i4.0
    IL_008a:  conv.i8
    IL_008b:  stloc.s    V_8
    IL_008d:  ldloc.0
    IL_008e:  stloc.s    V_9
    IL_0090:  br.s       IL_00a9

    IL_0092:  ldloca.s   V_5
    IL_0094:  ldloc.s    V_9
    IL_0096:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_009b:  nop
    IL_009c:  ldloc.s    V_9
    IL_009e:  ldloc.1
    IL_009f:  add
    IL_00a0:  stloc.s    V_9
    IL_00a2:  ldloc.s    V_8
    IL_00a4:  ldc.i4.1
    IL_00a5:  conv.i8
    IL_00a6:  add
    IL_00a7:  stloc.s    V_8
    IL_00a9:  ldloc.s    V_8
    IL_00ab:  ldloc.s    V_7
    IL_00ad:  blt.un.s   IL_0092

    IL_00af:  nop
    IL_00b0:  ldloca.s   V_5
    IL_00b2:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Close()
    IL_00b7:  ret
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






