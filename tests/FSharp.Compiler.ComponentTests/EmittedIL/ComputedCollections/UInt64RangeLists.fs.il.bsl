




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
    IL_0000:  nop
    IL_0001:  ldc.i4.s   10
    IL_0003:  conv.i8
    IL_0004:  ldarg.0
    IL_0005:  bge.un.s   IL_000c

    IL_0007:  ldc.i4.0
    IL_0008:  conv.i8
    IL_0009:  nop
    IL_000a:  br.s       IL_0015

    IL_000c:  ldc.i4.s   10
    IL_000e:  conv.i8
    IL_000f:  ldarg.0
    IL_0010:  sub
    IL_0011:  ldc.i4.1
    IL_0012:  conv.i8
    IL_0013:  add.ovf.un
    IL_0014:  nop
    IL_0015:  stloc.0
    IL_0016:  ldc.i4.0
    IL_0017:  conv.i8
    IL_0018:  stloc.2
    IL_0019:  ldarg.0
    IL_001a:  stloc.3
    IL_001b:  br.s       IL_0030

    IL_001d:  ldloca.s   V_1
    IL_001f:  ldloc.3
    IL_0020:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_0025:  nop
    IL_0026:  ldloc.3
    IL_0027:  ldc.i4.1
    IL_0028:  conv.i8
    IL_0029:  add
    IL_002a:  stloc.3
    IL_002b:  ldloc.2
    IL_002c:  ldc.i4.1
    IL_002d:  conv.i8
    IL_002e:  add
    IL_002f:  stloc.2
    IL_0030:  ldloc.2
    IL_0031:  ldloc.0
    IL_0032:  blt.un.s   IL_001d

    IL_0034:  ldloca.s   V_1
    IL_0036:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Close()
    IL_003b:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<uint64> f7(uint64 finish) cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64> V_1,
             uint64 V_2,
             uint64 V_3)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.1
    IL_0003:  conv.i8
    IL_0004:  bge.un.s   IL_000b

    IL_0006:  ldc.i4.0
    IL_0007:  conv.i8
    IL_0008:  nop
    IL_0009:  br.s       IL_0013

    IL_000b:  ldarg.0
    IL_000c:  ldc.i4.1
    IL_000d:  conv.i8
    IL_000e:  sub
    IL_000f:  ldc.i4.1
    IL_0010:  conv.i8
    IL_0011:  add.ovf.un
    IL_0012:  nop
    IL_0013:  stloc.0
    IL_0014:  ldc.i4.0
    IL_0015:  conv.i8
    IL_0016:  stloc.2
    IL_0017:  ldc.i4.1
    IL_0018:  conv.i8
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

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<uint64> 
          f8(uint64 start,
             uint64 finish) cil managed
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
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldarg.0
    IL_0003:  bge.un.s   IL_000a

    IL_0005:  ldc.i4.0
    IL_0006:  conv.i8
    IL_0007:  nop
    IL_0008:  br.s       IL_000e

    IL_000a:  ldarg.1
    IL_000b:  ldarg.0
    IL_000c:  sub
    IL_000d:  nop
    IL_000e:  stloc.0
    IL_000f:  ldloc.0
    IL_0010:  ldc.i4.m1
    IL_0011:  conv.i8
    IL_0012:  ceq
    IL_0014:  stloc.1
    IL_0015:  ldloc.1
    IL_0016:  brfalse.s  IL_0048

    IL_0018:  ldc.i4.1
    IL_0019:  stloc.3
    IL_001a:  ldc.i4.0
    IL_001b:  conv.i8
    IL_001c:  stloc.s    V_4
    IL_001e:  ldarg.0
    IL_001f:  stloc.s    V_5
    IL_0021:  br.s       IL_0042

    IL_0023:  ldloca.s   V_2
    IL_0025:  ldloc.s    V_5
    IL_0027:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_002c:  nop
    IL_002d:  ldloc.s    V_5
    IL_002f:  ldc.i4.1
    IL_0030:  conv.i8
    IL_0031:  add
    IL_0032:  stloc.s    V_5
    IL_0034:  ldloc.s    V_4
    IL_0036:  ldc.i4.1
    IL_0037:  conv.i8
    IL_0038:  add
    IL_0039:  stloc.s    V_4
    IL_003b:  ldloc.s    V_4
    IL_003d:  ldc.i4.0
    IL_003e:  conv.i8
    IL_003f:  cgt.un
    IL_0041:  stloc.3
    IL_0042:  ldloc.3
    IL_0043:  brtrue.s   IL_0023

    IL_0045:  nop
    IL_0046:  br.s       IL_0082

    IL_0048:  ldarg.1
    IL_0049:  ldarg.0
    IL_004a:  bge.un.s   IL_0051

    IL_004c:  ldc.i4.0
    IL_004d:  conv.i8
    IL_004e:  nop
    IL_004f:  br.s       IL_0058

    IL_0051:  ldarg.1
    IL_0052:  ldarg.0
    IL_0053:  sub
    IL_0054:  ldc.i4.1
    IL_0055:  conv.i8
    IL_0056:  add.ovf.un
    IL_0057:  nop
    IL_0058:  stloc.s    V_4
    IL_005a:  ldc.i4.0
    IL_005b:  conv.i8
    IL_005c:  stloc.s    V_5
    IL_005e:  ldarg.0
    IL_005f:  stloc.s    V_6
    IL_0061:  br.s       IL_007b

    IL_0063:  ldloca.s   V_2
    IL_0065:  ldloc.s    V_6
    IL_0067:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_006c:  nop
    IL_006d:  ldloc.s    V_6
    IL_006f:  ldc.i4.1
    IL_0070:  conv.i8
    IL_0071:  add
    IL_0072:  stloc.s    V_6
    IL_0074:  ldloc.s    V_5
    IL_0076:  ldc.i4.1
    IL_0077:  conv.i8
    IL_0078:  add
    IL_0079:  stloc.s    V_5
    IL_007b:  ldloc.s    V_5
    IL_007d:  ldloc.s    V_4
    IL_007f:  blt.un.s   IL_0063

    IL_0081:  nop
    IL_0082:  ldloca.s   V_2
    IL_0084:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Close()
    IL_0089:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<uint64> f9(uint64 start) cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64> V_1,
             uint64 V_2,
             uint64 V_3)
    IL_0000:  nop
    IL_0001:  ldc.i4.s   10
    IL_0003:  conv.i8
    IL_0004:  ldarg.0
    IL_0005:  bge.un.s   IL_000c

    IL_0007:  ldc.i4.0
    IL_0008:  conv.i8
    IL_0009:  nop
    IL_000a:  br.s       IL_0015

    IL_000c:  ldc.i4.s   10
    IL_000e:  conv.i8
    IL_000f:  ldarg.0
    IL_0010:  sub
    IL_0011:  ldc.i4.1
    IL_0012:  conv.i8
    IL_0013:  add.ovf.un
    IL_0014:  nop
    IL_0015:  stloc.0
    IL_0016:  ldc.i4.0
    IL_0017:  conv.i8
    IL_0018:  stloc.2
    IL_0019:  ldarg.0
    IL_001a:  stloc.3
    IL_001b:  br.s       IL_0030

    IL_001d:  ldloca.s   V_1
    IL_001f:  ldloc.3
    IL_0020:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_0025:  nop
    IL_0026:  ldloc.3
    IL_0027:  ldc.i4.1
    IL_0028:  conv.i8
    IL_0029:  add
    IL_002a:  stloc.3
    IL_002b:  ldloc.2
    IL_002c:  ldc.i4.1
    IL_002d:  conv.i8
    IL_002e:  add
    IL_002f:  stloc.2
    IL_0030:  ldloc.2
    IL_0031:  ldloc.0
    IL_0032:  blt.un.s   IL_001d

    IL_0034:  ldloca.s   V_1
    IL_0036:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Close()
    IL_003b:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<uint64> f10(uint64 step) cil managed
  {
    
    .maxstack  5
    .locals init (uint64 V_0,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64> V_1,
             uint64 V_2,
             uint64 V_3)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  brtrue.s   IL_0013

    IL_0004:  ldc.i4.1
    IL_0005:  conv.i8
    IL_0006:  ldarg.0
    IL_0007:  ldc.i4.s   10
    IL_0009:  conv.i8
    IL_000a:  call       class [runtime]System.Collections.Generic.IEnumerable`1<uint64> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeUInt64(uint64,
                                                                                                                                                                             uint64,
                                                                                                                                                                             uint64)
    IL_000f:  pop
    IL_0010:  nop
    IL_0011:  br.s       IL_0014

    IL_0013:  nop
    IL_0014:  ldc.i4.s   10
    IL_0016:  conv.i8
    IL_0017:  ldc.i4.1
    IL_0018:  conv.i8
    IL_0019:  bge.un.s   IL_0020

    IL_001b:  ldc.i4.0
    IL_001c:  conv.i8
    IL_001d:  nop
    IL_001e:  br.s       IL_002c

    IL_0020:  ldc.i4.s   10
    IL_0022:  conv.i8
    IL_0023:  ldc.i4.1
    IL_0024:  conv.i8
    IL_0025:  sub
    IL_0026:  ldarg.0
    IL_0027:  div.un
    IL_0028:  ldc.i4.1
    IL_0029:  conv.i8
    IL_002a:  add.ovf.un
    IL_002b:  nop
    IL_002c:  stloc.0
    IL_002d:  ldc.i4.0
    IL_002e:  conv.i8
    IL_002f:  stloc.2
    IL_0030:  ldc.i4.1
    IL_0031:  conv.i8
    IL_0032:  stloc.3
    IL_0033:  br.s       IL_0047

    IL_0035:  ldloca.s   V_1
    IL_0037:  ldloc.3
    IL_0038:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_003d:  nop
    IL_003e:  ldloc.3
    IL_003f:  ldarg.0
    IL_0040:  add
    IL_0041:  stloc.3
    IL_0042:  ldloc.2
    IL_0043:  ldc.i4.1
    IL_0044:  conv.i8
    IL_0045:  add
    IL_0046:  stloc.2
    IL_0047:  ldloc.2
    IL_0048:  ldloc.0
    IL_0049:  blt.un.s   IL_0035

    IL_004b:  ldloca.s   V_1
    IL_004d:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Close()
    IL_0052:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<uint64> f11(uint64 finish) cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64> V_1,
             uint64 V_2,
             uint64 V_3)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.1
    IL_0003:  conv.i8
    IL_0004:  bge.un.s   IL_000b

    IL_0006:  ldc.i4.0
    IL_0007:  conv.i8
    IL_0008:  nop
    IL_0009:  br.s       IL_0013

    IL_000b:  ldarg.0
    IL_000c:  ldc.i4.1
    IL_000d:  conv.i8
    IL_000e:  sub
    IL_000f:  ldc.i4.1
    IL_0010:  conv.i8
    IL_0011:  add.ovf.un
    IL_0012:  nop
    IL_0013:  stloc.0
    IL_0014:  ldc.i4.0
    IL_0015:  conv.i8
    IL_0016:  stloc.2
    IL_0017:  ldc.i4.1
    IL_0018:  conv.i8
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

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<uint64> 
          f12(uint64 start,
              uint64 step) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (uint64 V_0,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64> V_1,
             uint64 V_2,
             uint64 V_3)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  brtrue.s   IL_0012

    IL_0004:  ldarg.0
    IL_0005:  ldarg.1
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
    IL_0016:  ldarg.0
    IL_0017:  bge.un.s   IL_001e

    IL_0019:  ldc.i4.0
    IL_001a:  conv.i8
    IL_001b:  nop
    IL_001c:  br.s       IL_0029

    IL_001e:  ldc.i4.s   10
    IL_0020:  conv.i8
    IL_0021:  ldarg.0
    IL_0022:  sub
    IL_0023:  ldarg.1
    IL_0024:  div.un
    IL_0025:  ldc.i4.1
    IL_0026:  conv.i8
    IL_0027:  add.ovf.un
    IL_0028:  nop
    IL_0029:  stloc.0
    IL_002a:  ldc.i4.0
    IL_002b:  conv.i8
    IL_002c:  stloc.2
    IL_002d:  ldarg.0
    IL_002e:  stloc.3
    IL_002f:  br.s       IL_0043

    IL_0031:  ldloca.s   V_1
    IL_0033:  ldloc.3
    IL_0034:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_0039:  nop
    IL_003a:  ldloc.3
    IL_003b:  ldarg.1
    IL_003c:  add
    IL_003d:  stloc.3
    IL_003e:  ldloc.2
    IL_003f:  ldc.i4.1
    IL_0040:  conv.i8
    IL_0041:  add
    IL_0042:  stloc.2
    IL_0043:  ldloc.2
    IL_0044:  ldloc.0
    IL_0045:  blt.un.s   IL_0031

    IL_0047:  ldloca.s   V_1
    IL_0049:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Close()
    IL_004e:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<uint64> 
          f13(uint64 start,
              uint64 finish) cil managed
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
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldarg.0
    IL_0003:  bge.un.s   IL_000a

    IL_0005:  ldc.i4.0
    IL_0006:  conv.i8
    IL_0007:  nop
    IL_0008:  br.s       IL_000e

    IL_000a:  ldarg.1
    IL_000b:  ldarg.0
    IL_000c:  sub
    IL_000d:  nop
    IL_000e:  stloc.0
    IL_000f:  ldloc.0
    IL_0010:  ldc.i4.m1
    IL_0011:  conv.i8
    IL_0012:  ceq
    IL_0014:  stloc.1
    IL_0015:  ldloc.1
    IL_0016:  brfalse.s  IL_0048

    IL_0018:  ldc.i4.1
    IL_0019:  stloc.3
    IL_001a:  ldc.i4.0
    IL_001b:  conv.i8
    IL_001c:  stloc.s    V_4
    IL_001e:  ldarg.0
    IL_001f:  stloc.s    V_5
    IL_0021:  br.s       IL_0042

    IL_0023:  ldloca.s   V_2
    IL_0025:  ldloc.s    V_5
    IL_0027:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_002c:  nop
    IL_002d:  ldloc.s    V_5
    IL_002f:  ldc.i4.1
    IL_0030:  conv.i8
    IL_0031:  add
    IL_0032:  stloc.s    V_5
    IL_0034:  ldloc.s    V_4
    IL_0036:  ldc.i4.1
    IL_0037:  conv.i8
    IL_0038:  add
    IL_0039:  stloc.s    V_4
    IL_003b:  ldloc.s    V_4
    IL_003d:  ldc.i4.0
    IL_003e:  conv.i8
    IL_003f:  cgt.un
    IL_0041:  stloc.3
    IL_0042:  ldloc.3
    IL_0043:  brtrue.s   IL_0023

    IL_0045:  nop
    IL_0046:  br.s       IL_0082

    IL_0048:  ldarg.1
    IL_0049:  ldarg.0
    IL_004a:  bge.un.s   IL_0051

    IL_004c:  ldc.i4.0
    IL_004d:  conv.i8
    IL_004e:  nop
    IL_004f:  br.s       IL_0058

    IL_0051:  ldarg.1
    IL_0052:  ldarg.0
    IL_0053:  sub
    IL_0054:  ldc.i4.1
    IL_0055:  conv.i8
    IL_0056:  add.ovf.un
    IL_0057:  nop
    IL_0058:  stloc.s    V_4
    IL_005a:  ldc.i4.0
    IL_005b:  conv.i8
    IL_005c:  stloc.s    V_5
    IL_005e:  ldarg.0
    IL_005f:  stloc.s    V_6
    IL_0061:  br.s       IL_007b

    IL_0063:  ldloca.s   V_2
    IL_0065:  ldloc.s    V_6
    IL_0067:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_006c:  nop
    IL_006d:  ldloc.s    V_6
    IL_006f:  ldc.i4.1
    IL_0070:  conv.i8
    IL_0071:  add
    IL_0072:  stloc.s    V_6
    IL_0074:  ldloc.s    V_5
    IL_0076:  ldc.i4.1
    IL_0077:  conv.i8
    IL_0078:  add
    IL_0079:  stloc.s    V_5
    IL_007b:  ldloc.s    V_5
    IL_007d:  ldloc.s    V_4
    IL_007f:  blt.un.s   IL_0063

    IL_0081:  nop
    IL_0082:  ldloca.s   V_2
    IL_0084:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Close()
    IL_0089:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<uint64> 
          f14(uint64 step,
              uint64 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (uint64 V_0,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64> V_1,
             uint64 V_2,
             uint64 V_3)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  brtrue.s   IL_0011

    IL_0004:  ldc.i4.1
    IL_0005:  conv.i8
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  call       class [runtime]System.Collections.Generic.IEnumerable`1<uint64> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeUInt64(uint64,
                                                                                                                                                                             uint64,
                                                                                                                                                                             uint64)
    IL_000d:  pop
    IL_000e:  nop
    IL_000f:  br.s       IL_0012

    IL_0011:  nop
    IL_0012:  ldarg.1
    IL_0013:  ldc.i4.1
    IL_0014:  conv.i8
    IL_0015:  bge.un.s   IL_001c

    IL_0017:  ldc.i4.0
    IL_0018:  conv.i8
    IL_0019:  nop
    IL_001a:  br.s       IL_0026

    IL_001c:  ldarg.1
    IL_001d:  ldc.i4.1
    IL_001e:  conv.i8
    IL_001f:  sub
    IL_0020:  ldarg.0
    IL_0021:  div.un
    IL_0022:  ldc.i4.1
    IL_0023:  conv.i8
    IL_0024:  add.ovf.un
    IL_0025:  nop
    IL_0026:  stloc.0
    IL_0027:  ldc.i4.0
    IL_0028:  conv.i8
    IL_0029:  stloc.2
    IL_002a:  ldc.i4.1
    IL_002b:  conv.i8
    IL_002c:  stloc.3
    IL_002d:  br.s       IL_0041

    IL_002f:  ldloca.s   V_1
    IL_0031:  ldloc.3
    IL_0032:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_0037:  nop
    IL_0038:  ldloc.3
    IL_0039:  ldarg.0
    IL_003a:  add
    IL_003b:  stloc.3
    IL_003c:  ldloc.2
    IL_003d:  ldc.i4.1
    IL_003e:  conv.i8
    IL_003f:  add
    IL_0040:  stloc.2
    IL_0041:  ldloc.2
    IL_0042:  ldloc.0
    IL_0043:  blt.un.s   IL_002f

    IL_0045:  ldloca.s   V_1
    IL_0047:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Close()
    IL_004c:  ret
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
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  brtrue.s   IL_0010

    IL_0004:  ldarg.0
    IL_0005:  ldarg.1
    IL_0006:  ldarg.2
    IL_0007:  call       class [runtime]System.Collections.Generic.IEnumerable`1<uint64> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeUInt64(uint64,
                                                                                                                                                                             uint64,
                                                                                                                                                                             uint64)
    IL_000c:  pop
    IL_000d:  nop
    IL_000e:  br.s       IL_0011

    IL_0010:  nop
    IL_0011:  ldarg.2
    IL_0012:  ldarg.0
    IL_0013:  bge.un.s   IL_001a

    IL_0015:  ldc.i4.0
    IL_0016:  conv.i8
    IL_0017:  nop
    IL_0018:  br.s       IL_0020

    IL_001a:  ldarg.2
    IL_001b:  ldarg.0
    IL_001c:  sub
    IL_001d:  ldarg.1
    IL_001e:  div.un
    IL_001f:  nop
    IL_0020:  stloc.0
    IL_0021:  ldloc.0
    IL_0022:  ldc.i4.m1
    IL_0023:  conv.i8
    IL_0024:  ceq
    IL_0026:  stloc.1
    IL_0027:  ldloc.1
    IL_0028:  brfalse.s  IL_0059

    IL_002a:  ldc.i4.1
    IL_002b:  stloc.3
    IL_002c:  ldc.i4.0
    IL_002d:  conv.i8
    IL_002e:  stloc.s    V_4
    IL_0030:  ldarg.0
    IL_0031:  stloc.s    V_5
    IL_0033:  br.s       IL_0053

    IL_0035:  ldloca.s   V_2
    IL_0037:  ldloc.s    V_5
    IL_0039:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_003e:  nop
    IL_003f:  ldloc.s    V_5
    IL_0041:  ldarg.1
    IL_0042:  add
    IL_0043:  stloc.s    V_5
    IL_0045:  ldloc.s    V_4
    IL_0047:  ldc.i4.1
    IL_0048:  conv.i8
    IL_0049:  add
    IL_004a:  stloc.s    V_4
    IL_004c:  ldloc.s    V_4
    IL_004e:  ldc.i4.0
    IL_004f:  conv.i8
    IL_0050:  cgt.un
    IL_0052:  stloc.3
    IL_0053:  ldloc.3
    IL_0054:  brtrue.s   IL_0035

    IL_0056:  nop
    IL_0057:  br.s       IL_0094

    IL_0059:  ldarg.2
    IL_005a:  ldarg.0
    IL_005b:  bge.un.s   IL_0062

    IL_005d:  ldc.i4.0
    IL_005e:  conv.i8
    IL_005f:  nop
    IL_0060:  br.s       IL_006b

    IL_0062:  ldarg.2
    IL_0063:  ldarg.0
    IL_0064:  sub
    IL_0065:  ldarg.1
    IL_0066:  div.un
    IL_0067:  ldc.i4.1
    IL_0068:  conv.i8
    IL_0069:  add.ovf.un
    IL_006a:  nop
    IL_006b:  stloc.s    V_4
    IL_006d:  ldc.i4.0
    IL_006e:  conv.i8
    IL_006f:  stloc.s    V_5
    IL_0071:  ldarg.0
    IL_0072:  stloc.s    V_6
    IL_0074:  br.s       IL_008d

    IL_0076:  ldloca.s   V_2
    IL_0078:  ldloc.s    V_6
    IL_007a:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_007f:  nop
    IL_0080:  ldloc.s    V_6
    IL_0082:  ldarg.1
    IL_0083:  add
    IL_0084:  stloc.s    V_6
    IL_0086:  ldloc.s    V_5
    IL_0088:  ldc.i4.1
    IL_0089:  conv.i8
    IL_008a:  add
    IL_008b:  stloc.s    V_5
    IL_008d:  ldloc.s    V_5
    IL_008f:  ldloc.s    V_4
    IL_0091:  blt.un.s   IL_0076

    IL_0093:  nop
    IL_0094:  ldloca.s   V_2
    IL_0096:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Close()
    IL_009b:  ret
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

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<uint64> 
          f18(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,uint64> f,
              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,uint64> g) cil managed
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






