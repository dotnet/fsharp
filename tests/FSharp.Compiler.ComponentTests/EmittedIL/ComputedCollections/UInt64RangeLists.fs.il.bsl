




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
             uint64 V_3,
             uint64 V_4,
             uint64 V_5)
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
    IL_0016:  brfalse.s  IL_0054

    IL_0018:  ldc.i4.0
    IL_0019:  conv.i8
    IL_001a:  stloc.3
    IL_001b:  ldarg.0
    IL_001c:  stloc.s    V_4
    IL_001e:  ldloca.s   V_2
    IL_0020:  ldloc.s    V_4
    IL_0022:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_0027:  nop
    IL_0028:  ldloc.s    V_4
    IL_002a:  ldc.i4.1
    IL_002b:  conv.i8
    IL_002c:  add
    IL_002d:  stloc.s    V_4
    IL_002f:  ldloc.3
    IL_0030:  ldc.i4.1
    IL_0031:  conv.i8
    IL_0032:  add
    IL_0033:  stloc.3
    IL_0034:  br.s       IL_004c

    IL_0036:  ldloca.s   V_2
    IL_0038:  ldloc.s    V_4
    IL_003a:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_003f:  nop
    IL_0040:  ldloc.s    V_4
    IL_0042:  ldc.i4.1
    IL_0043:  conv.i8
    IL_0044:  add
    IL_0045:  stloc.s    V_4
    IL_0047:  ldloc.3
    IL_0048:  ldc.i4.1
    IL_0049:  conv.i8
    IL_004a:  add
    IL_004b:  stloc.3
    IL_004c:  ldloc.3
    IL_004d:  ldc.i4.0
    IL_004e:  conv.i8
    IL_004f:  bgt.un.s   IL_0036

    IL_0051:  nop
    IL_0052:  br.s       IL_0080

    IL_0054:  ldloc.0
    IL_0055:  ldc.i4.1
    IL_0056:  conv.i8
    IL_0057:  add.ovf.un
    IL_0058:  stloc.3
    IL_0059:  ldc.i4.0
    IL_005a:  conv.i8
    IL_005b:  stloc.s    V_4
    IL_005d:  ldarg.0
    IL_005e:  stloc.s    V_5
    IL_0060:  br.s       IL_007a

    IL_0062:  ldloca.s   V_2
    IL_0064:  ldloc.s    V_5
    IL_0066:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_006b:  nop
    IL_006c:  ldloc.s    V_5
    IL_006e:  ldc.i4.1
    IL_006f:  conv.i8
    IL_0070:  add
    IL_0071:  stloc.s    V_5
    IL_0073:  ldloc.s    V_4
    IL_0075:  ldc.i4.1
    IL_0076:  conv.i8
    IL_0077:  add
    IL_0078:  stloc.s    V_4
    IL_007a:  ldloc.s    V_4
    IL_007c:  ldloc.3
    IL_007d:  blt.un.s   IL_0062

    IL_007f:  nop
    IL_0080:  ldloca.s   V_2
    IL_0082:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Close()
    IL_0087:  ret
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
             bool V_1,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64> V_2,
             uint64 V_3,
             uint64 V_4,
             uint64 V_5)
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
    IL_001e:  br.s       IL_002a

    IL_0020:  ldc.i4.s   10
    IL_0022:  conv.i8
    IL_0023:  ldc.i4.1
    IL_0024:  conv.i8
    IL_0025:  sub
    IL_0026:  ldarg.0
    IL_0027:  conv.i8
    IL_0028:  div.un
    IL_0029:  nop
    IL_002a:  stloc.0
    IL_002b:  ldloc.0
    IL_002c:  ldc.i4.m1
    IL_002d:  conv.i8
    IL_002e:  ceq
    IL_0030:  stloc.1
    IL_0031:  ldloc.1
    IL_0032:  brfalse.s  IL_006f

    IL_0034:  ldc.i4.0
    IL_0035:  conv.i8
    IL_0036:  stloc.3
    IL_0037:  ldc.i4.1
    IL_0038:  conv.i8
    IL_0039:  stloc.s    V_4
    IL_003b:  ldloca.s   V_2
    IL_003d:  ldloc.s    V_4
    IL_003f:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_0044:  nop
    IL_0045:  ldloc.s    V_4
    IL_0047:  ldarg.0
    IL_0048:  add
    IL_0049:  stloc.s    V_4
    IL_004b:  ldloc.3
    IL_004c:  ldc.i4.1
    IL_004d:  conv.i8
    IL_004e:  add
    IL_004f:  stloc.3
    IL_0050:  br.s       IL_0067

    IL_0052:  ldloca.s   V_2
    IL_0054:  ldloc.s    V_4
    IL_0056:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_005b:  nop
    IL_005c:  ldloc.s    V_4
    IL_005e:  ldarg.0
    IL_005f:  add
    IL_0060:  stloc.s    V_4
    IL_0062:  ldloc.3
    IL_0063:  ldc.i4.1
    IL_0064:  conv.i8
    IL_0065:  add
    IL_0066:  stloc.3
    IL_0067:  ldloc.3
    IL_0068:  ldc.i4.0
    IL_0069:  conv.i8
    IL_006a:  bgt.un.s   IL_0052

    IL_006c:  nop
    IL_006d:  br.s       IL_009b

    IL_006f:  ldloc.0
    IL_0070:  ldc.i4.1
    IL_0071:  conv.i8
    IL_0072:  add.ovf.un
    IL_0073:  stloc.3
    IL_0074:  ldc.i4.0
    IL_0075:  conv.i8
    IL_0076:  stloc.s    V_4
    IL_0078:  ldc.i4.1
    IL_0079:  conv.i8
    IL_007a:  stloc.s    V_5
    IL_007c:  br.s       IL_0095

    IL_007e:  ldloca.s   V_2
    IL_0080:  ldloc.s    V_5
    IL_0082:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_0087:  nop
    IL_0088:  ldloc.s    V_5
    IL_008a:  ldarg.0
    IL_008b:  add
    IL_008c:  stloc.s    V_5
    IL_008e:  ldloc.s    V_4
    IL_0090:  ldc.i4.1
    IL_0091:  conv.i8
    IL_0092:  add
    IL_0093:  stloc.s    V_4
    IL_0095:  ldloc.s    V_4
    IL_0097:  ldloc.3
    IL_0098:  blt.un.s   IL_007e

    IL_009a:  nop
    IL_009b:  ldloca.s   V_2
    IL_009d:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Close()
    IL_00a2:  ret
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
             bool V_1,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64> V_2,
             uint64 V_3,
             uint64 V_4,
             uint64 V_5)
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
    IL_001c:  br.s       IL_0027

    IL_001e:  ldc.i4.s   10
    IL_0020:  conv.i8
    IL_0021:  ldarg.0
    IL_0022:  sub
    IL_0023:  ldarg.1
    IL_0024:  conv.i8
    IL_0025:  div.un
    IL_0026:  nop
    IL_0027:  stloc.0
    IL_0028:  ldloc.0
    IL_0029:  ldc.i4.m1
    IL_002a:  conv.i8
    IL_002b:  ceq
    IL_002d:  stloc.1
    IL_002e:  ldloc.1
    IL_002f:  brfalse.s  IL_006b

    IL_0031:  ldc.i4.0
    IL_0032:  conv.i8
    IL_0033:  stloc.3
    IL_0034:  ldarg.0
    IL_0035:  stloc.s    V_4
    IL_0037:  ldloca.s   V_2
    IL_0039:  ldloc.s    V_4
    IL_003b:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_0040:  nop
    IL_0041:  ldloc.s    V_4
    IL_0043:  ldarg.1
    IL_0044:  add
    IL_0045:  stloc.s    V_4
    IL_0047:  ldloc.3
    IL_0048:  ldc.i4.1
    IL_0049:  conv.i8
    IL_004a:  add
    IL_004b:  stloc.3
    IL_004c:  br.s       IL_0063

    IL_004e:  ldloca.s   V_2
    IL_0050:  ldloc.s    V_4
    IL_0052:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_0057:  nop
    IL_0058:  ldloc.s    V_4
    IL_005a:  ldarg.1
    IL_005b:  add
    IL_005c:  stloc.s    V_4
    IL_005e:  ldloc.3
    IL_005f:  ldc.i4.1
    IL_0060:  conv.i8
    IL_0061:  add
    IL_0062:  stloc.3
    IL_0063:  ldloc.3
    IL_0064:  ldc.i4.0
    IL_0065:  conv.i8
    IL_0066:  bgt.un.s   IL_004e

    IL_0068:  nop
    IL_0069:  br.s       IL_0096

    IL_006b:  ldloc.0
    IL_006c:  ldc.i4.1
    IL_006d:  conv.i8
    IL_006e:  add.ovf.un
    IL_006f:  stloc.3
    IL_0070:  ldc.i4.0
    IL_0071:  conv.i8
    IL_0072:  stloc.s    V_4
    IL_0074:  ldarg.0
    IL_0075:  stloc.s    V_5
    IL_0077:  br.s       IL_0090

    IL_0079:  ldloca.s   V_2
    IL_007b:  ldloc.s    V_5
    IL_007d:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_0082:  nop
    IL_0083:  ldloc.s    V_5
    IL_0085:  ldarg.1
    IL_0086:  add
    IL_0087:  stloc.s    V_5
    IL_0089:  ldloc.s    V_4
    IL_008b:  ldc.i4.1
    IL_008c:  conv.i8
    IL_008d:  add
    IL_008e:  stloc.s    V_4
    IL_0090:  ldloc.s    V_4
    IL_0092:  ldloc.3
    IL_0093:  blt.un.s   IL_0079

    IL_0095:  nop
    IL_0096:  ldloca.s   V_2
    IL_0098:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Close()
    IL_009d:  ret
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
             uint64 V_3,
             uint64 V_4,
             uint64 V_5)
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
    IL_0016:  brfalse.s  IL_0054

    IL_0018:  ldc.i4.0
    IL_0019:  conv.i8
    IL_001a:  stloc.3
    IL_001b:  ldarg.0
    IL_001c:  stloc.s    V_4
    IL_001e:  ldloca.s   V_2
    IL_0020:  ldloc.s    V_4
    IL_0022:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_0027:  nop
    IL_0028:  ldloc.s    V_4
    IL_002a:  ldc.i4.1
    IL_002b:  conv.i8
    IL_002c:  add
    IL_002d:  stloc.s    V_4
    IL_002f:  ldloc.3
    IL_0030:  ldc.i4.1
    IL_0031:  conv.i8
    IL_0032:  add
    IL_0033:  stloc.3
    IL_0034:  br.s       IL_004c

    IL_0036:  ldloca.s   V_2
    IL_0038:  ldloc.s    V_4
    IL_003a:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_003f:  nop
    IL_0040:  ldloc.s    V_4
    IL_0042:  ldc.i4.1
    IL_0043:  conv.i8
    IL_0044:  add
    IL_0045:  stloc.s    V_4
    IL_0047:  ldloc.3
    IL_0048:  ldc.i4.1
    IL_0049:  conv.i8
    IL_004a:  add
    IL_004b:  stloc.3
    IL_004c:  ldloc.3
    IL_004d:  ldc.i4.0
    IL_004e:  conv.i8
    IL_004f:  bgt.un.s   IL_0036

    IL_0051:  nop
    IL_0052:  br.s       IL_0080

    IL_0054:  ldloc.0
    IL_0055:  ldc.i4.1
    IL_0056:  conv.i8
    IL_0057:  add.ovf.un
    IL_0058:  stloc.3
    IL_0059:  ldc.i4.0
    IL_005a:  conv.i8
    IL_005b:  stloc.s    V_4
    IL_005d:  ldarg.0
    IL_005e:  stloc.s    V_5
    IL_0060:  br.s       IL_007a

    IL_0062:  ldloca.s   V_2
    IL_0064:  ldloc.s    V_5
    IL_0066:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_006b:  nop
    IL_006c:  ldloc.s    V_5
    IL_006e:  ldc.i4.1
    IL_006f:  conv.i8
    IL_0070:  add
    IL_0071:  stloc.s    V_5
    IL_0073:  ldloc.s    V_4
    IL_0075:  ldc.i4.1
    IL_0076:  conv.i8
    IL_0077:  add
    IL_0078:  stloc.s    V_4
    IL_007a:  ldloc.s    V_4
    IL_007c:  ldloc.3
    IL_007d:  blt.un.s   IL_0062

    IL_007f:  nop
    IL_0080:  ldloca.s   V_2
    IL_0082:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Close()
    IL_0087:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<uint64> 
          f14(uint64 step,
              uint64 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (uint64 V_0,
             bool V_1,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64> V_2,
             uint64 V_3,
             uint64 V_4,
             uint64 V_5)
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
    IL_001a:  br.s       IL_0024

    IL_001c:  ldarg.1
    IL_001d:  ldc.i4.1
    IL_001e:  conv.i8
    IL_001f:  sub
    IL_0020:  ldarg.0
    IL_0021:  conv.i8
    IL_0022:  div.un
    IL_0023:  nop
    IL_0024:  stloc.0
    IL_0025:  ldloc.0
    IL_0026:  ldc.i4.m1
    IL_0027:  conv.i8
    IL_0028:  ceq
    IL_002a:  stloc.1
    IL_002b:  ldloc.1
    IL_002c:  brfalse.s  IL_0069

    IL_002e:  ldc.i4.0
    IL_002f:  conv.i8
    IL_0030:  stloc.3
    IL_0031:  ldc.i4.1
    IL_0032:  conv.i8
    IL_0033:  stloc.s    V_4
    IL_0035:  ldloca.s   V_2
    IL_0037:  ldloc.s    V_4
    IL_0039:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_003e:  nop
    IL_003f:  ldloc.s    V_4
    IL_0041:  ldarg.0
    IL_0042:  add
    IL_0043:  stloc.s    V_4
    IL_0045:  ldloc.3
    IL_0046:  ldc.i4.1
    IL_0047:  conv.i8
    IL_0048:  add
    IL_0049:  stloc.3
    IL_004a:  br.s       IL_0061

    IL_004c:  ldloca.s   V_2
    IL_004e:  ldloc.s    V_4
    IL_0050:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_0055:  nop
    IL_0056:  ldloc.s    V_4
    IL_0058:  ldarg.0
    IL_0059:  add
    IL_005a:  stloc.s    V_4
    IL_005c:  ldloc.3
    IL_005d:  ldc.i4.1
    IL_005e:  conv.i8
    IL_005f:  add
    IL_0060:  stloc.3
    IL_0061:  ldloc.3
    IL_0062:  ldc.i4.0
    IL_0063:  conv.i8
    IL_0064:  bgt.un.s   IL_004c

    IL_0066:  nop
    IL_0067:  br.s       IL_0095

    IL_0069:  ldloc.0
    IL_006a:  ldc.i4.1
    IL_006b:  conv.i8
    IL_006c:  add.ovf.un
    IL_006d:  stloc.3
    IL_006e:  ldc.i4.0
    IL_006f:  conv.i8
    IL_0070:  stloc.s    V_4
    IL_0072:  ldc.i4.1
    IL_0073:  conv.i8
    IL_0074:  stloc.s    V_5
    IL_0076:  br.s       IL_008f

    IL_0078:  ldloca.s   V_2
    IL_007a:  ldloc.s    V_5
    IL_007c:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_0081:  nop
    IL_0082:  ldloc.s    V_5
    IL_0084:  ldarg.0
    IL_0085:  add
    IL_0086:  stloc.s    V_5
    IL_0088:  ldloc.s    V_4
    IL_008a:  ldc.i4.1
    IL_008b:  conv.i8
    IL_008c:  add
    IL_008d:  stloc.s    V_4
    IL_008f:  ldloc.s    V_4
    IL_0091:  ldloc.3
    IL_0092:  blt.un.s   IL_0078

    IL_0094:  nop
    IL_0095:  ldloca.s   V_2
    IL_0097:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Close()
    IL_009c:  ret
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
             uint64 V_3,
             uint64 V_4,
             uint64 V_5)
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
    IL_0018:  br.s       IL_0021

    IL_001a:  ldarg.2
    IL_001b:  ldarg.0
    IL_001c:  sub
    IL_001d:  ldarg.1
    IL_001e:  conv.i8
    IL_001f:  div.un
    IL_0020:  nop
    IL_0021:  stloc.0
    IL_0022:  ldloc.0
    IL_0023:  ldc.i4.m1
    IL_0024:  conv.i8
    IL_0025:  ceq
    IL_0027:  stloc.1
    IL_0028:  ldloc.1
    IL_0029:  brfalse.s  IL_0065

    IL_002b:  ldc.i4.0
    IL_002c:  conv.i8
    IL_002d:  stloc.3
    IL_002e:  ldarg.0
    IL_002f:  stloc.s    V_4
    IL_0031:  ldloca.s   V_2
    IL_0033:  ldloc.s    V_4
    IL_0035:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_003a:  nop
    IL_003b:  ldloc.s    V_4
    IL_003d:  ldarg.1
    IL_003e:  add
    IL_003f:  stloc.s    V_4
    IL_0041:  ldloc.3
    IL_0042:  ldc.i4.1
    IL_0043:  conv.i8
    IL_0044:  add
    IL_0045:  stloc.3
    IL_0046:  br.s       IL_005d

    IL_0048:  ldloca.s   V_2
    IL_004a:  ldloc.s    V_4
    IL_004c:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_0051:  nop
    IL_0052:  ldloc.s    V_4
    IL_0054:  ldarg.1
    IL_0055:  add
    IL_0056:  stloc.s    V_4
    IL_0058:  ldloc.3
    IL_0059:  ldc.i4.1
    IL_005a:  conv.i8
    IL_005b:  add
    IL_005c:  stloc.3
    IL_005d:  ldloc.3
    IL_005e:  ldc.i4.0
    IL_005f:  conv.i8
    IL_0060:  bgt.un.s   IL_0048

    IL_0062:  nop
    IL_0063:  br.s       IL_0090

    IL_0065:  ldloc.0
    IL_0066:  ldc.i4.1
    IL_0067:  conv.i8
    IL_0068:  add.ovf.un
    IL_0069:  stloc.3
    IL_006a:  ldc.i4.0
    IL_006b:  conv.i8
    IL_006c:  stloc.s    V_4
    IL_006e:  ldarg.0
    IL_006f:  stloc.s    V_5
    IL_0071:  br.s       IL_008a

    IL_0073:  ldloca.s   V_2
    IL_0075:  ldloc.s    V_5
    IL_0077:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_007c:  nop
    IL_007d:  ldloc.s    V_5
    IL_007f:  ldarg.1
    IL_0080:  add
    IL_0081:  stloc.s    V_5
    IL_0083:  ldloc.s    V_4
    IL_0085:  ldc.i4.1
    IL_0086:  conv.i8
    IL_0087:  add
    IL_0088:  stloc.s    V_4
    IL_008a:  ldloc.s    V_4
    IL_008c:  ldloc.3
    IL_008d:  blt.un.s   IL_0073

    IL_008f:  nop
    IL_0090:  ldloca.s   V_2
    IL_0092:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Close()
    IL_0097:  ret
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
             uint64 V_5,
             uint64 V_6,
             uint64 V_7)
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
    IL_0025:  brfalse.s  IL_0069

    IL_0027:  ldc.i4.0
    IL_0028:  conv.i8
    IL_0029:  stloc.s    V_5
    IL_002b:  ldloc.0
    IL_002c:  stloc.s    V_6
    IL_002e:  ldloca.s   V_4
    IL_0030:  ldloc.s    V_6
    IL_0032:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_0037:  nop
    IL_0038:  ldloc.s    V_6
    IL_003a:  ldc.i4.1
    IL_003b:  conv.i8
    IL_003c:  add
    IL_003d:  stloc.s    V_6
    IL_003f:  ldloc.s    V_5
    IL_0041:  ldc.i4.1
    IL_0042:  conv.i8
    IL_0043:  add
    IL_0044:  stloc.s    V_5
    IL_0046:  br.s       IL_0060

    IL_0048:  ldloca.s   V_4
    IL_004a:  ldloc.s    V_6
    IL_004c:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_0051:  nop
    IL_0052:  ldloc.s    V_6
    IL_0054:  ldc.i4.1
    IL_0055:  conv.i8
    IL_0056:  add
    IL_0057:  stloc.s    V_6
    IL_0059:  ldloc.s    V_5
    IL_005b:  ldc.i4.1
    IL_005c:  conv.i8
    IL_005d:  add
    IL_005e:  stloc.s    V_5
    IL_0060:  ldloc.s    V_5
    IL_0062:  ldc.i4.0
    IL_0063:  conv.i8
    IL_0064:  bgt.un.s   IL_0048

    IL_0066:  nop
    IL_0067:  br.s       IL_0097

    IL_0069:  ldloc.2
    IL_006a:  ldc.i4.1
    IL_006b:  conv.i8
    IL_006c:  add.ovf.un
    IL_006d:  stloc.s    V_5
    IL_006f:  ldc.i4.0
    IL_0070:  conv.i8
    IL_0071:  stloc.s    V_6
    IL_0073:  ldloc.0
    IL_0074:  stloc.s    V_7
    IL_0076:  br.s       IL_0090

    IL_0078:  ldloca.s   V_4
    IL_007a:  ldloc.s    V_7
    IL_007c:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_0081:  nop
    IL_0082:  ldloc.s    V_7
    IL_0084:  ldc.i4.1
    IL_0085:  conv.i8
    IL_0086:  add
    IL_0087:  stloc.s    V_7
    IL_0089:  ldloc.s    V_6
    IL_008b:  ldc.i4.1
    IL_008c:  conv.i8
    IL_008d:  add
    IL_008e:  stloc.s    V_6
    IL_0090:  ldloc.s    V_6
    IL_0092:  ldloc.s    V_5
    IL_0094:  blt.un.s   IL_0078

    IL_0096:  nop
    IL_0097:  ldloca.s   V_4
    IL_0099:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Close()
    IL_009e:  ret
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
             bool V_2,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64> V_3,
             uint64 V_4,
             uint64 V_5,
             uint64 V_6)
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
    IL_0025:  br.s       IL_0031

    IL_0027:  ldc.i4.s   10
    IL_0029:  conv.i8
    IL_002a:  ldc.i4.1
    IL_002b:  conv.i8
    IL_002c:  sub
    IL_002d:  ldloc.0
    IL_002e:  conv.i8
    IL_002f:  div.un
    IL_0030:  nop
    IL_0031:  stloc.1
    IL_0032:  ldloc.1
    IL_0033:  ldc.i4.m1
    IL_0034:  conv.i8
    IL_0035:  ceq
    IL_0037:  stloc.2
    IL_0038:  ldloc.2
    IL_0039:  brfalse.s  IL_007c

    IL_003b:  ldc.i4.0
    IL_003c:  conv.i8
    IL_003d:  stloc.s    V_4
    IL_003f:  ldc.i4.1
    IL_0040:  conv.i8
    IL_0041:  stloc.s    V_5
    IL_0043:  ldloca.s   V_3
    IL_0045:  ldloc.s    V_5
    IL_0047:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_004c:  nop
    IL_004d:  ldloc.s    V_5
    IL_004f:  ldloc.0
    IL_0050:  add
    IL_0051:  stloc.s    V_5
    IL_0053:  ldloc.s    V_4
    IL_0055:  ldc.i4.1
    IL_0056:  conv.i8
    IL_0057:  add
    IL_0058:  stloc.s    V_4
    IL_005a:  br.s       IL_0073

    IL_005c:  ldloca.s   V_3
    IL_005e:  ldloc.s    V_5
    IL_0060:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_0065:  nop
    IL_0066:  ldloc.s    V_5
    IL_0068:  ldloc.0
    IL_0069:  add
    IL_006a:  stloc.s    V_5
    IL_006c:  ldloc.s    V_4
    IL_006e:  ldc.i4.1
    IL_006f:  conv.i8
    IL_0070:  add
    IL_0071:  stloc.s    V_4
    IL_0073:  ldloc.s    V_4
    IL_0075:  ldc.i4.0
    IL_0076:  conv.i8
    IL_0077:  bgt.un.s   IL_005c

    IL_0079:  nop
    IL_007a:  br.s       IL_00aa

    IL_007c:  ldloc.1
    IL_007d:  ldc.i4.1
    IL_007e:  conv.i8
    IL_007f:  add.ovf.un
    IL_0080:  stloc.s    V_4
    IL_0082:  ldc.i4.0
    IL_0083:  conv.i8
    IL_0084:  stloc.s    V_5
    IL_0086:  ldc.i4.1
    IL_0087:  conv.i8
    IL_0088:  stloc.s    V_6
    IL_008a:  br.s       IL_00a3

    IL_008c:  ldloca.s   V_3
    IL_008e:  ldloc.s    V_6
    IL_0090:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_0095:  nop
    IL_0096:  ldloc.s    V_6
    IL_0098:  ldloc.0
    IL_0099:  add
    IL_009a:  stloc.s    V_6
    IL_009c:  ldloc.s    V_5
    IL_009e:  ldc.i4.1
    IL_009f:  conv.i8
    IL_00a0:  add
    IL_00a1:  stloc.s    V_5
    IL_00a3:  ldloc.s    V_5
    IL_00a5:  ldloc.s    V_4
    IL_00a7:  blt.un.s   IL_008c

    IL_00a9:  nop
    IL_00aa:  ldloca.s   V_3
    IL_00ac:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Close()
    IL_00b1:  ret
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
    IL_002f:  br.s       IL_0038

    IL_0031:  ldloc.2
    IL_0032:  ldloc.0
    IL_0033:  sub
    IL_0034:  ldloc.1
    IL_0035:  conv.i8
    IL_0036:  div.un
    IL_0037:  nop
    IL_0038:  stloc.3
    IL_0039:  ldloc.3
    IL_003a:  ldc.i4.m1
    IL_003b:  conv.i8
    IL_003c:  ceq
    IL_003e:  stloc.s    V_4
    IL_0040:  ldloc.s    V_4
    IL_0042:  brfalse.s  IL_0084

    IL_0044:  ldc.i4.0
    IL_0045:  conv.i8
    IL_0046:  stloc.s    V_6
    IL_0048:  ldloc.0
    IL_0049:  stloc.s    V_7
    IL_004b:  ldloca.s   V_5
    IL_004d:  ldloc.s    V_7
    IL_004f:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_0054:  nop
    IL_0055:  ldloc.s    V_7
    IL_0057:  ldloc.1
    IL_0058:  add
    IL_0059:  stloc.s    V_7
    IL_005b:  ldloc.s    V_6
    IL_005d:  ldc.i4.1
    IL_005e:  conv.i8
    IL_005f:  add
    IL_0060:  stloc.s    V_6
    IL_0062:  br.s       IL_007b

    IL_0064:  ldloca.s   V_5
    IL_0066:  ldloc.s    V_7
    IL_0068:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_006d:  nop
    IL_006e:  ldloc.s    V_7
    IL_0070:  ldloc.1
    IL_0071:  add
    IL_0072:  stloc.s    V_7
    IL_0074:  ldloc.s    V_6
    IL_0076:  ldc.i4.1
    IL_0077:  conv.i8
    IL_0078:  add
    IL_0079:  stloc.s    V_6
    IL_007b:  ldloc.s    V_6
    IL_007d:  ldc.i4.0
    IL_007e:  conv.i8
    IL_007f:  bgt.un.s   IL_0064

    IL_0081:  nop
    IL_0082:  br.s       IL_00b1

    IL_0084:  ldloc.3
    IL_0085:  ldc.i4.1
    IL_0086:  conv.i8
    IL_0087:  add.ovf.un
    IL_0088:  stloc.s    V_6
    IL_008a:  ldc.i4.0
    IL_008b:  conv.i8
    IL_008c:  stloc.s    V_7
    IL_008e:  ldloc.0
    IL_008f:  stloc.s    V_8
    IL_0091:  br.s       IL_00aa

    IL_0093:  ldloca.s   V_5
    IL_0095:  ldloc.s    V_8
    IL_0097:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Add(!0)
    IL_009c:  nop
    IL_009d:  ldloc.s    V_8
    IL_009f:  ldloc.1
    IL_00a0:  add
    IL_00a1:  stloc.s    V_8
    IL_00a3:  ldloc.s    V_7
    IL_00a5:  ldc.i4.1
    IL_00a6:  conv.i8
    IL_00a7:  add
    IL_00a8:  stloc.s    V_7
    IL_00aa:  ldloc.s    V_7
    IL_00ac:  ldloc.s    V_6
    IL_00ae:  blt.un.s   IL_0093

    IL_00b0:  nop
    IL_00b1:  ldloca.s   V_5
    IL_00b3:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<uint64>::Close()
    IL_00b8:  ret
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






