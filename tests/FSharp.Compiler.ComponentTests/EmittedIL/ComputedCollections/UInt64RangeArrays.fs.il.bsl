




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
  .method public static uint64[]  f1() cil managed
  {
    
    .maxstack  5
    .locals init (uint64[] V_0,
             uint64 V_1,
             uint64 V_2)
    IL_0000:  ldc.i4.s   10
    IL_0002:  conv.i8
    IL_0003:  conv.ovf.i.un
    IL_0004:  newarr     [runtime]System.UInt64
    IL_0009:  stloc.0
    IL_000a:  ldc.i4.0
    IL_000b:  conv.i8
    IL_000c:  stloc.1
    IL_000d:  ldc.i4.1
    IL_000e:  conv.i8
    IL_000f:  stloc.2
    IL_0010:  br.s       IL_0021

    IL_0012:  ldloc.0
    IL_0013:  ldloc.1
    IL_0014:  conv.i
    IL_0015:  ldloc.2
    IL_0016:  stelem.i8
    IL_0017:  ldloc.2
    IL_0018:  ldc.i4.1
    IL_0019:  conv.i8
    IL_001a:  add
    IL_001b:  stloc.2
    IL_001c:  ldloc.1
    IL_001d:  ldc.i4.1
    IL_001e:  conv.i8
    IL_001f:  add
    IL_0020:  stloc.1
    IL_0021:  ldloc.1
    IL_0022:  ldc.i4.s   10
    IL_0024:  conv.i8
    IL_0025:  blt.un.s   IL_0012

    IL_0027:  ldloc.0
    IL_0028:  ret
  } 

  .method public static uint64[]  f2() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_0005:  ret
  } 

  .method public static uint64[]  f3() cil managed
  {
    
    .maxstack  5
    .locals init (uint64[] V_0,
             uint64 V_1,
             uint64 V_2)
    IL_0000:  ldc.i4.s   10
    IL_0002:  conv.i8
    IL_0003:  conv.ovf.i.un
    IL_0004:  newarr     [runtime]System.UInt64
    IL_0009:  stloc.0
    IL_000a:  ldc.i4.0
    IL_000b:  conv.i8
    IL_000c:  stloc.1
    IL_000d:  ldc.i4.1
    IL_000e:  conv.i8
    IL_000f:  stloc.2
    IL_0010:  br.s       IL_0021

    IL_0012:  ldloc.0
    IL_0013:  ldloc.1
    IL_0014:  conv.i
    IL_0015:  ldloc.2
    IL_0016:  stelem.i8
    IL_0017:  ldloc.2
    IL_0018:  ldc.i4.1
    IL_0019:  conv.i8
    IL_001a:  add
    IL_001b:  stloc.2
    IL_001c:  ldloc.1
    IL_001d:  ldc.i4.1
    IL_001e:  conv.i8
    IL_001f:  add
    IL_0020:  stloc.1
    IL_0021:  ldloc.1
    IL_0022:  ldc.i4.s   10
    IL_0024:  conv.i8
    IL_0025:  blt.un.s   IL_0012

    IL_0027:  ldloc.0
    IL_0028:  ret
  } 

  .method public static uint64[]  f4() cil managed
  {
    
    .maxstack  5
    .locals init (uint64[] V_0,
             uint64 V_1,
             uint64 V_2)
    IL_0000:  ldc.i4.5
    IL_0001:  conv.i8
    IL_0002:  conv.ovf.i.un
    IL_0003:  newarr     [runtime]System.UInt64
    IL_0008:  stloc.0
    IL_0009:  ldc.i4.0
    IL_000a:  conv.i8
    IL_000b:  stloc.1
    IL_000c:  ldc.i4.1
    IL_000d:  conv.i8
    IL_000e:  stloc.2
    IL_000f:  br.s       IL_0020

    IL_0011:  ldloc.0
    IL_0012:  ldloc.1
    IL_0013:  conv.i
    IL_0014:  ldloc.2
    IL_0015:  stelem.i8
    IL_0016:  ldloc.2
    IL_0017:  ldc.i4.2
    IL_0018:  conv.i8
    IL_0019:  add
    IL_001a:  stloc.2
    IL_001b:  ldloc.1
    IL_001c:  ldc.i4.1
    IL_001d:  conv.i8
    IL_001e:  add
    IL_001f:  stloc.1
    IL_0020:  ldloc.1
    IL_0021:  ldc.i4.5
    IL_0022:  conv.i8
    IL_0023:  blt.un.s   IL_0011

    IL_0025:  ldloc.0
    IL_0026:  ret
  } 

  .method public static uint64[]  f5() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_0005:  ret
  } 

  .method public static uint64[]  f6(uint64 start) cil managed
  {
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             uint64[] V_2,
             uint64 V_3,
             uint64 V_4)
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
    IL_0016:  ldloc.0
    IL_0017:  stloc.1
    IL_0018:  ldloc.1
    IL_0019:  ldc.i4.1
    IL_001a:  conv.i8
    IL_001b:  bge.un.s   IL_0023

    IL_001d:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_0022:  ret

    IL_0023:  ldloc.1
    IL_0024:  conv.ovf.i.un
    IL_0025:  newarr     [runtime]System.UInt64
    IL_002a:  stloc.2
    IL_002b:  ldc.i4.0
    IL_002c:  conv.i8
    IL_002d:  stloc.3
    IL_002e:  ldarg.0
    IL_002f:  stloc.s    V_4
    IL_0031:  br.s       IL_0045

    IL_0033:  ldloc.2
    IL_0034:  ldloc.3
    IL_0035:  conv.i
    IL_0036:  ldloc.s    V_4
    IL_0038:  stelem.i8
    IL_0039:  ldloc.s    V_4
    IL_003b:  ldc.i4.1
    IL_003c:  conv.i8
    IL_003d:  add
    IL_003e:  stloc.s    V_4
    IL_0040:  ldloc.3
    IL_0041:  ldc.i4.1
    IL_0042:  conv.i8
    IL_0043:  add
    IL_0044:  stloc.3
    IL_0045:  ldloc.3
    IL_0046:  ldloc.0
    IL_0047:  blt.un.s   IL_0033

    IL_0049:  ldloc.2
    IL_004a:  ret
  } 

  .method public static uint64[]  f7(uint64 finish) cil managed
  {
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             uint64[] V_2,
             uint64 V_3,
             uint64 V_4)
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
    IL_0014:  ldloc.0
    IL_0015:  stloc.1
    IL_0016:  ldloc.1
    IL_0017:  ldc.i4.1
    IL_0018:  conv.i8
    IL_0019:  bge.un.s   IL_0021

    IL_001b:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_0020:  ret

    IL_0021:  ldloc.1
    IL_0022:  conv.ovf.i.un
    IL_0023:  newarr     [runtime]System.UInt64
    IL_0028:  stloc.2
    IL_0029:  ldc.i4.0
    IL_002a:  conv.i8
    IL_002b:  stloc.3
    IL_002c:  ldc.i4.1
    IL_002d:  conv.i8
    IL_002e:  stloc.s    V_4
    IL_0030:  br.s       IL_0044

    IL_0032:  ldloc.2
    IL_0033:  ldloc.3
    IL_0034:  conv.i
    IL_0035:  ldloc.s    V_4
    IL_0037:  stelem.i8
    IL_0038:  ldloc.s    V_4
    IL_003a:  ldc.i4.1
    IL_003b:  conv.i8
    IL_003c:  add
    IL_003d:  stloc.s    V_4
    IL_003f:  ldloc.3
    IL_0040:  ldc.i4.1
    IL_0041:  conv.i8
    IL_0042:  add
    IL_0043:  stloc.3
    IL_0044:  ldloc.3
    IL_0045:  ldloc.0
    IL_0046:  blt.un.s   IL_0032

    IL_0048:  ldloc.2
    IL_0049:  ret
  } 

  .method public static uint64[]  f8(uint64 start,
                                     uint64 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (uint64 V_0,
             bool V_1,
             uint64 V_2,
             uint64[] V_3,
             bool V_4,
             uint64 V_5,
             uint64 V_6,
             uint64 V_7)
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
    IL_0015:  ldarg.1
    IL_0016:  ldarg.0
    IL_0017:  bge.un.s   IL_001e

    IL_0019:  ldc.i4.0
    IL_001a:  conv.i8
    IL_001b:  nop
    IL_001c:  br.s       IL_0025

    IL_001e:  ldarg.1
    IL_001f:  ldarg.0
    IL_0020:  sub
    IL_0021:  ldc.i4.1
    IL_0022:  conv.i8
    IL_0023:  add.ovf.un
    IL_0024:  nop
    IL_0025:  stloc.2
    IL_0026:  ldloc.2
    IL_0027:  ldc.i4.1
    IL_0028:  conv.i8
    IL_0029:  bge.un.s   IL_0031

    IL_002b:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_0030:  ret

    IL_0031:  ldloc.2
    IL_0032:  conv.ovf.i.un
    IL_0033:  newarr     [runtime]System.UInt64
    IL_0038:  stloc.3
    IL_0039:  ldloc.1
    IL_003a:  brfalse.s  IL_006c

    IL_003c:  ldc.i4.1
    IL_003d:  stloc.s    V_4
    IL_003f:  ldc.i4.0
    IL_0040:  conv.i8
    IL_0041:  stloc.s    V_5
    IL_0043:  ldarg.0
    IL_0044:  stloc.s    V_6
    IL_0046:  br.s       IL_0065

    IL_0048:  ldloc.3
    IL_0049:  ldloc.s    V_5
    IL_004b:  conv.i
    IL_004c:  ldloc.s    V_6
    IL_004e:  stelem.i8
    IL_004f:  ldloc.s    V_6
    IL_0051:  ldc.i4.1
    IL_0052:  conv.i8
    IL_0053:  add
    IL_0054:  stloc.s    V_6
    IL_0056:  ldloc.s    V_5
    IL_0058:  ldc.i4.1
    IL_0059:  conv.i8
    IL_005a:  add
    IL_005b:  stloc.s    V_5
    IL_005d:  ldloc.s    V_5
    IL_005f:  ldc.i4.0
    IL_0060:  conv.i8
    IL_0061:  cgt.un
    IL_0063:  stloc.s    V_4
    IL_0065:  ldloc.s    V_4
    IL_0067:  brtrue.s   IL_0048

    IL_0069:  nop
    IL_006a:  br.s       IL_00a3

    IL_006c:  ldarg.1
    IL_006d:  ldarg.0
    IL_006e:  bge.un.s   IL_0075

    IL_0070:  ldc.i4.0
    IL_0071:  conv.i8
    IL_0072:  nop
    IL_0073:  br.s       IL_007c

    IL_0075:  ldarg.1
    IL_0076:  ldarg.0
    IL_0077:  sub
    IL_0078:  ldc.i4.1
    IL_0079:  conv.i8
    IL_007a:  add.ovf.un
    IL_007b:  nop
    IL_007c:  stloc.s    V_5
    IL_007e:  ldc.i4.0
    IL_007f:  conv.i8
    IL_0080:  stloc.s    V_6
    IL_0082:  ldarg.0
    IL_0083:  stloc.s    V_7
    IL_0085:  br.s       IL_009c

    IL_0087:  ldloc.3
    IL_0088:  ldloc.s    V_6
    IL_008a:  conv.i
    IL_008b:  ldloc.s    V_7
    IL_008d:  stelem.i8
    IL_008e:  ldloc.s    V_7
    IL_0090:  ldc.i4.1
    IL_0091:  conv.i8
    IL_0092:  add
    IL_0093:  stloc.s    V_7
    IL_0095:  ldloc.s    V_6
    IL_0097:  ldc.i4.1
    IL_0098:  conv.i8
    IL_0099:  add
    IL_009a:  stloc.s    V_6
    IL_009c:  ldloc.s    V_6
    IL_009e:  ldloc.s    V_5
    IL_00a0:  blt.un.s   IL_0087

    IL_00a2:  nop
    IL_00a3:  ldloc.3
    IL_00a4:  ret
  } 

  .method public static uint64[]  f9(uint64 start) cil managed
  {
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             uint64[] V_2,
             uint64 V_3,
             uint64 V_4)
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
    IL_0016:  ldloc.0
    IL_0017:  stloc.1
    IL_0018:  ldloc.1
    IL_0019:  ldc.i4.1
    IL_001a:  conv.i8
    IL_001b:  bge.un.s   IL_0023

    IL_001d:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_0022:  ret

    IL_0023:  ldloc.1
    IL_0024:  conv.ovf.i.un
    IL_0025:  newarr     [runtime]System.UInt64
    IL_002a:  stloc.2
    IL_002b:  ldc.i4.0
    IL_002c:  conv.i8
    IL_002d:  stloc.3
    IL_002e:  ldarg.0
    IL_002f:  stloc.s    V_4
    IL_0031:  br.s       IL_0045

    IL_0033:  ldloc.2
    IL_0034:  ldloc.3
    IL_0035:  conv.i
    IL_0036:  ldloc.s    V_4
    IL_0038:  stelem.i8
    IL_0039:  ldloc.s    V_4
    IL_003b:  ldc.i4.1
    IL_003c:  conv.i8
    IL_003d:  add
    IL_003e:  stloc.s    V_4
    IL_0040:  ldloc.3
    IL_0041:  ldc.i4.1
    IL_0042:  conv.i8
    IL_0043:  add
    IL_0044:  stloc.3
    IL_0045:  ldloc.3
    IL_0046:  ldloc.0
    IL_0047:  blt.un.s   IL_0033

    IL_0049:  ldloc.2
    IL_004a:  ret
  } 

  .method public static uint64[]  f10(uint64 step) cil managed
  {
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             uint64[] V_2,
             uint64 V_3,
             uint64 V_4)
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
    IL_002d:  ldloc.0
    IL_002e:  stloc.1
    IL_002f:  ldloc.1
    IL_0030:  ldc.i4.1
    IL_0031:  conv.i8
    IL_0032:  bge.un.s   IL_003a

    IL_0034:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_0039:  ret

    IL_003a:  ldloc.1
    IL_003b:  conv.ovf.i.un
    IL_003c:  newarr     [runtime]System.UInt64
    IL_0041:  stloc.2
    IL_0042:  ldc.i4.0
    IL_0043:  conv.i8
    IL_0044:  stloc.3
    IL_0045:  ldc.i4.1
    IL_0046:  conv.i8
    IL_0047:  stloc.s    V_4
    IL_0049:  br.s       IL_005c

    IL_004b:  ldloc.2
    IL_004c:  ldloc.3
    IL_004d:  conv.i
    IL_004e:  ldloc.s    V_4
    IL_0050:  stelem.i8
    IL_0051:  ldloc.s    V_4
    IL_0053:  ldarg.0
    IL_0054:  add
    IL_0055:  stloc.s    V_4
    IL_0057:  ldloc.3
    IL_0058:  ldc.i4.1
    IL_0059:  conv.i8
    IL_005a:  add
    IL_005b:  stloc.3
    IL_005c:  ldloc.3
    IL_005d:  ldloc.0
    IL_005e:  blt.un.s   IL_004b

    IL_0060:  ldloc.2
    IL_0061:  ret
  } 

  .method public static uint64[]  f11(uint64 finish) cil managed
  {
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             uint64[] V_2,
             uint64 V_3,
             uint64 V_4)
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
    IL_0014:  ldloc.0
    IL_0015:  stloc.1
    IL_0016:  ldloc.1
    IL_0017:  ldc.i4.1
    IL_0018:  conv.i8
    IL_0019:  bge.un.s   IL_0021

    IL_001b:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_0020:  ret

    IL_0021:  ldloc.1
    IL_0022:  conv.ovf.i.un
    IL_0023:  newarr     [runtime]System.UInt64
    IL_0028:  stloc.2
    IL_0029:  ldc.i4.0
    IL_002a:  conv.i8
    IL_002b:  stloc.3
    IL_002c:  ldc.i4.1
    IL_002d:  conv.i8
    IL_002e:  stloc.s    V_4
    IL_0030:  br.s       IL_0044

    IL_0032:  ldloc.2
    IL_0033:  ldloc.3
    IL_0034:  conv.i
    IL_0035:  ldloc.s    V_4
    IL_0037:  stelem.i8
    IL_0038:  ldloc.s    V_4
    IL_003a:  ldc.i4.1
    IL_003b:  conv.i8
    IL_003c:  add
    IL_003d:  stloc.s    V_4
    IL_003f:  ldloc.3
    IL_0040:  ldc.i4.1
    IL_0041:  conv.i8
    IL_0042:  add
    IL_0043:  stloc.3
    IL_0044:  ldloc.3
    IL_0045:  ldloc.0
    IL_0046:  blt.un.s   IL_0032

    IL_0048:  ldloc.2
    IL_0049:  ret
  } 

  .method public static uint64[]  f12(uint64 start,
                                      uint64 step) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             uint64[] V_2,
             uint64 V_3,
             uint64 V_4)
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
    IL_002a:  ldloc.0
    IL_002b:  stloc.1
    IL_002c:  ldloc.1
    IL_002d:  ldc.i4.1
    IL_002e:  conv.i8
    IL_002f:  bge.un.s   IL_0037

    IL_0031:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_0036:  ret

    IL_0037:  ldloc.1
    IL_0038:  conv.ovf.i.un
    IL_0039:  newarr     [runtime]System.UInt64
    IL_003e:  stloc.2
    IL_003f:  ldc.i4.0
    IL_0040:  conv.i8
    IL_0041:  stloc.3
    IL_0042:  ldarg.0
    IL_0043:  stloc.s    V_4
    IL_0045:  br.s       IL_0058

    IL_0047:  ldloc.2
    IL_0048:  ldloc.3
    IL_0049:  conv.i
    IL_004a:  ldloc.s    V_4
    IL_004c:  stelem.i8
    IL_004d:  ldloc.s    V_4
    IL_004f:  ldarg.1
    IL_0050:  add
    IL_0051:  stloc.s    V_4
    IL_0053:  ldloc.3
    IL_0054:  ldc.i4.1
    IL_0055:  conv.i8
    IL_0056:  add
    IL_0057:  stloc.3
    IL_0058:  ldloc.3
    IL_0059:  ldloc.0
    IL_005a:  blt.un.s   IL_0047

    IL_005c:  ldloc.2
    IL_005d:  ret
  } 

  .method public static uint64[]  f13(uint64 start,
                                      uint64 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (uint64 V_0,
             bool V_1,
             uint64 V_2,
             uint64[] V_3,
             bool V_4,
             uint64 V_5,
             uint64 V_6,
             uint64 V_7)
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
    IL_0015:  ldarg.1
    IL_0016:  ldarg.0
    IL_0017:  bge.un.s   IL_001e

    IL_0019:  ldc.i4.0
    IL_001a:  conv.i8
    IL_001b:  nop
    IL_001c:  br.s       IL_0025

    IL_001e:  ldarg.1
    IL_001f:  ldarg.0
    IL_0020:  sub
    IL_0021:  ldc.i4.1
    IL_0022:  conv.i8
    IL_0023:  add.ovf.un
    IL_0024:  nop
    IL_0025:  stloc.2
    IL_0026:  ldloc.2
    IL_0027:  ldc.i4.1
    IL_0028:  conv.i8
    IL_0029:  bge.un.s   IL_0031

    IL_002b:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_0030:  ret

    IL_0031:  ldloc.2
    IL_0032:  conv.ovf.i.un
    IL_0033:  newarr     [runtime]System.UInt64
    IL_0038:  stloc.3
    IL_0039:  ldloc.1
    IL_003a:  brfalse.s  IL_006c

    IL_003c:  ldc.i4.1
    IL_003d:  stloc.s    V_4
    IL_003f:  ldc.i4.0
    IL_0040:  conv.i8
    IL_0041:  stloc.s    V_5
    IL_0043:  ldarg.0
    IL_0044:  stloc.s    V_6
    IL_0046:  br.s       IL_0065

    IL_0048:  ldloc.3
    IL_0049:  ldloc.s    V_5
    IL_004b:  conv.i
    IL_004c:  ldloc.s    V_6
    IL_004e:  stelem.i8
    IL_004f:  ldloc.s    V_6
    IL_0051:  ldc.i4.1
    IL_0052:  conv.i8
    IL_0053:  add
    IL_0054:  stloc.s    V_6
    IL_0056:  ldloc.s    V_5
    IL_0058:  ldc.i4.1
    IL_0059:  conv.i8
    IL_005a:  add
    IL_005b:  stloc.s    V_5
    IL_005d:  ldloc.s    V_5
    IL_005f:  ldc.i4.0
    IL_0060:  conv.i8
    IL_0061:  cgt.un
    IL_0063:  stloc.s    V_4
    IL_0065:  ldloc.s    V_4
    IL_0067:  brtrue.s   IL_0048

    IL_0069:  nop
    IL_006a:  br.s       IL_00a3

    IL_006c:  ldarg.1
    IL_006d:  ldarg.0
    IL_006e:  bge.un.s   IL_0075

    IL_0070:  ldc.i4.0
    IL_0071:  conv.i8
    IL_0072:  nop
    IL_0073:  br.s       IL_007c

    IL_0075:  ldarg.1
    IL_0076:  ldarg.0
    IL_0077:  sub
    IL_0078:  ldc.i4.1
    IL_0079:  conv.i8
    IL_007a:  add.ovf.un
    IL_007b:  nop
    IL_007c:  stloc.s    V_5
    IL_007e:  ldc.i4.0
    IL_007f:  conv.i8
    IL_0080:  stloc.s    V_6
    IL_0082:  ldarg.0
    IL_0083:  stloc.s    V_7
    IL_0085:  br.s       IL_009c

    IL_0087:  ldloc.3
    IL_0088:  ldloc.s    V_6
    IL_008a:  conv.i
    IL_008b:  ldloc.s    V_7
    IL_008d:  stelem.i8
    IL_008e:  ldloc.s    V_7
    IL_0090:  ldc.i4.1
    IL_0091:  conv.i8
    IL_0092:  add
    IL_0093:  stloc.s    V_7
    IL_0095:  ldloc.s    V_6
    IL_0097:  ldc.i4.1
    IL_0098:  conv.i8
    IL_0099:  add
    IL_009a:  stloc.s    V_6
    IL_009c:  ldloc.s    V_6
    IL_009e:  ldloc.s    V_5
    IL_00a0:  blt.un.s   IL_0087

    IL_00a2:  nop
    IL_00a3:  ldloc.3
    IL_00a4:  ret
  } 

  .method public static uint64[]  f14(uint64 step,
                                      uint64 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             uint64[] V_2,
             uint64 V_3,
             uint64 V_4)
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
    IL_0027:  ldloc.0
    IL_0028:  stloc.1
    IL_0029:  ldloc.1
    IL_002a:  ldc.i4.1
    IL_002b:  conv.i8
    IL_002c:  bge.un.s   IL_0034

    IL_002e:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_0033:  ret

    IL_0034:  ldloc.1
    IL_0035:  conv.ovf.i.un
    IL_0036:  newarr     [runtime]System.UInt64
    IL_003b:  stloc.2
    IL_003c:  ldc.i4.0
    IL_003d:  conv.i8
    IL_003e:  stloc.3
    IL_003f:  ldc.i4.1
    IL_0040:  conv.i8
    IL_0041:  stloc.s    V_4
    IL_0043:  br.s       IL_0056

    IL_0045:  ldloc.2
    IL_0046:  ldloc.3
    IL_0047:  conv.i
    IL_0048:  ldloc.s    V_4
    IL_004a:  stelem.i8
    IL_004b:  ldloc.s    V_4
    IL_004d:  ldarg.0
    IL_004e:  add
    IL_004f:  stloc.s    V_4
    IL_0051:  ldloc.3
    IL_0052:  ldc.i4.1
    IL_0053:  conv.i8
    IL_0054:  add
    IL_0055:  stloc.3
    IL_0056:  ldloc.3
    IL_0057:  ldloc.0
    IL_0058:  blt.un.s   IL_0045

    IL_005a:  ldloc.2
    IL_005b:  ret
  } 

  .method public static uint64[]  f15(uint64 start,
                                      uint64 step,
                                      uint64 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  5
    .locals init (uint64 V_0,
             bool V_1,
             uint64 V_2,
             uint64[] V_3,
             bool V_4,
             uint64 V_5,
             uint64 V_6,
             uint64 V_7)
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
    IL_0027:  ldarg.2
    IL_0028:  ldarg.0
    IL_0029:  bge.un.s   IL_0030

    IL_002b:  ldc.i4.0
    IL_002c:  conv.i8
    IL_002d:  nop
    IL_002e:  br.s       IL_0039

    IL_0030:  ldarg.2
    IL_0031:  ldarg.0
    IL_0032:  sub
    IL_0033:  ldarg.1
    IL_0034:  div.un
    IL_0035:  ldc.i4.1
    IL_0036:  conv.i8
    IL_0037:  add.ovf.un
    IL_0038:  nop
    IL_0039:  stloc.2
    IL_003a:  ldloc.2
    IL_003b:  ldc.i4.1
    IL_003c:  conv.i8
    IL_003d:  bge.un.s   IL_0045

    IL_003f:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_0044:  ret

    IL_0045:  ldloc.2
    IL_0046:  conv.ovf.i.un
    IL_0047:  newarr     [runtime]System.UInt64
    IL_004c:  stloc.3
    IL_004d:  ldloc.1
    IL_004e:  brfalse.s  IL_007f

    IL_0050:  ldc.i4.1
    IL_0051:  stloc.s    V_4
    IL_0053:  ldc.i4.0
    IL_0054:  conv.i8
    IL_0055:  stloc.s    V_5
    IL_0057:  ldarg.0
    IL_0058:  stloc.s    V_6
    IL_005a:  br.s       IL_0078

    IL_005c:  ldloc.3
    IL_005d:  ldloc.s    V_5
    IL_005f:  conv.i
    IL_0060:  ldloc.s    V_6
    IL_0062:  stelem.i8
    IL_0063:  ldloc.s    V_6
    IL_0065:  ldarg.1
    IL_0066:  add
    IL_0067:  stloc.s    V_6
    IL_0069:  ldloc.s    V_5
    IL_006b:  ldc.i4.1
    IL_006c:  conv.i8
    IL_006d:  add
    IL_006e:  stloc.s    V_5
    IL_0070:  ldloc.s    V_5
    IL_0072:  ldc.i4.0
    IL_0073:  conv.i8
    IL_0074:  cgt.un
    IL_0076:  stloc.s    V_4
    IL_0078:  ldloc.s    V_4
    IL_007a:  brtrue.s   IL_005c

    IL_007c:  nop
    IL_007d:  br.s       IL_00b7

    IL_007f:  ldarg.2
    IL_0080:  ldarg.0
    IL_0081:  bge.un.s   IL_0088

    IL_0083:  ldc.i4.0
    IL_0084:  conv.i8
    IL_0085:  nop
    IL_0086:  br.s       IL_0091

    IL_0088:  ldarg.2
    IL_0089:  ldarg.0
    IL_008a:  sub
    IL_008b:  ldarg.1
    IL_008c:  div.un
    IL_008d:  ldc.i4.1
    IL_008e:  conv.i8
    IL_008f:  add.ovf.un
    IL_0090:  nop
    IL_0091:  stloc.s    V_5
    IL_0093:  ldc.i4.0
    IL_0094:  conv.i8
    IL_0095:  stloc.s    V_6
    IL_0097:  ldarg.0
    IL_0098:  stloc.s    V_7
    IL_009a:  br.s       IL_00b0

    IL_009c:  ldloc.3
    IL_009d:  ldloc.s    V_6
    IL_009f:  conv.i
    IL_00a0:  ldloc.s    V_7
    IL_00a2:  stelem.i8
    IL_00a3:  ldloc.s    V_7
    IL_00a5:  ldarg.1
    IL_00a6:  add
    IL_00a7:  stloc.s    V_7
    IL_00a9:  ldloc.s    V_6
    IL_00ab:  ldc.i4.1
    IL_00ac:  conv.i8
    IL_00ad:  add
    IL_00ae:  stloc.s    V_6
    IL_00b0:  ldloc.s    V_6
    IL_00b2:  ldloc.s    V_5
    IL_00b4:  blt.un.s   IL_009c

    IL_00b6:  nop
    IL_00b7:  ldloc.3
    IL_00b8:  ret
  } 

  .method public static uint64[]  f16(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,uint64> f) cil managed
  {
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             uint64 V_2,
             uint64[] V_3,
             uint64 V_4,
             uint64 V_5)
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
    IL_001d:  ldloc.1
    IL_001e:  stloc.2
    IL_001f:  ldloc.2
    IL_0020:  ldc.i4.1
    IL_0021:  conv.i8
    IL_0022:  bge.un.s   IL_002a

    IL_0024:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_0029:  ret

    IL_002a:  ldloc.2
    IL_002b:  conv.ovf.i.un
    IL_002c:  newarr     [runtime]System.UInt64
    IL_0031:  stloc.3
    IL_0032:  ldc.i4.0
    IL_0033:  conv.i8
    IL_0034:  stloc.s    V_4
    IL_0036:  ldloc.0
    IL_0037:  stloc.s    V_5
    IL_0039:  br.s       IL_0050

    IL_003b:  ldloc.3
    IL_003c:  ldloc.s    V_4
    IL_003e:  conv.i
    IL_003f:  ldloc.s    V_5
    IL_0041:  stelem.i8
    IL_0042:  ldloc.s    V_5
    IL_0044:  ldc.i4.1
    IL_0045:  conv.i8
    IL_0046:  add
    IL_0047:  stloc.s    V_5
    IL_0049:  ldloc.s    V_4
    IL_004b:  ldc.i4.1
    IL_004c:  conv.i8
    IL_004d:  add
    IL_004e:  stloc.s    V_4
    IL_0050:  ldloc.s    V_4
    IL_0052:  ldloc.1
    IL_0053:  blt.un.s   IL_003b

    IL_0055:  ldloc.3
    IL_0056:  ret
  } 

  .method public static uint64[]  f17(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,uint64> f) cil managed
  {
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             uint64 V_2,
             uint64[] V_3,
             uint64 V_4,
             uint64 V_5)
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
    IL_001b:  ldloc.1
    IL_001c:  stloc.2
    IL_001d:  ldloc.2
    IL_001e:  ldc.i4.1
    IL_001f:  conv.i8
    IL_0020:  bge.un.s   IL_0028

    IL_0022:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_0027:  ret

    IL_0028:  ldloc.2
    IL_0029:  conv.ovf.i.un
    IL_002a:  newarr     [runtime]System.UInt64
    IL_002f:  stloc.3
    IL_0030:  ldc.i4.0
    IL_0031:  conv.i8
    IL_0032:  stloc.s    V_4
    IL_0034:  ldc.i4.1
    IL_0035:  conv.i8
    IL_0036:  stloc.s    V_5
    IL_0038:  br.s       IL_004f

    IL_003a:  ldloc.3
    IL_003b:  ldloc.s    V_4
    IL_003d:  conv.i
    IL_003e:  ldloc.s    V_5
    IL_0040:  stelem.i8
    IL_0041:  ldloc.s    V_5
    IL_0043:  ldc.i4.1
    IL_0044:  conv.i8
    IL_0045:  add
    IL_0046:  stloc.s    V_5
    IL_0048:  ldloc.s    V_4
    IL_004a:  ldc.i4.1
    IL_004b:  conv.i8
    IL_004c:  add
    IL_004d:  stloc.s    V_4
    IL_004f:  ldloc.s    V_4
    IL_0051:  ldloc.1
    IL_0052:  blt.un.s   IL_003a

    IL_0054:  ldloc.3
    IL_0055:  ret
  } 

  .method public static uint64[]  f18(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,uint64> f,
                                      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,uint64> g) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             uint64 V_2,
             bool V_3,
             uint64 V_4,
             uint64[] V_5,
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
    IL_0024:  ldloc.1
    IL_0025:  ldloc.0
    IL_0026:  bge.un.s   IL_002d

    IL_0028:  ldc.i4.0
    IL_0029:  conv.i8
    IL_002a:  nop
    IL_002b:  br.s       IL_0034

    IL_002d:  ldloc.1
    IL_002e:  ldloc.0
    IL_002f:  sub
    IL_0030:  ldc.i4.1
    IL_0031:  conv.i8
    IL_0032:  add.ovf.un
    IL_0033:  nop
    IL_0034:  stloc.s    V_4
    IL_0036:  ldloc.s    V_4
    IL_0038:  ldc.i4.1
    IL_0039:  conv.i8
    IL_003a:  bge.un.s   IL_0042

    IL_003c:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_0041:  ret

    IL_0042:  ldloc.s    V_4
    IL_0044:  conv.ovf.i.un
    IL_0045:  newarr     [runtime]System.UInt64
    IL_004a:  stloc.s    V_5
    IL_004c:  ldloc.3
    IL_004d:  brfalse.s  IL_0080

    IL_004f:  ldc.i4.1
    IL_0050:  stloc.s    V_6
    IL_0052:  ldc.i4.0
    IL_0053:  conv.i8
    IL_0054:  stloc.s    V_7
    IL_0056:  ldloc.0
    IL_0057:  stloc.s    V_8
    IL_0059:  br.s       IL_0079

    IL_005b:  ldloc.s    V_5
    IL_005d:  ldloc.s    V_7
    IL_005f:  conv.i
    IL_0060:  ldloc.s    V_8
    IL_0062:  stelem.i8
    IL_0063:  ldloc.s    V_8
    IL_0065:  ldc.i4.1
    IL_0066:  conv.i8
    IL_0067:  add
    IL_0068:  stloc.s    V_8
    IL_006a:  ldloc.s    V_7
    IL_006c:  ldc.i4.1
    IL_006d:  conv.i8
    IL_006e:  add
    IL_006f:  stloc.s    V_7
    IL_0071:  ldloc.s    V_7
    IL_0073:  ldc.i4.0
    IL_0074:  conv.i8
    IL_0075:  cgt.un
    IL_0077:  stloc.s    V_6
    IL_0079:  ldloc.s    V_6
    IL_007b:  brtrue.s   IL_005b

    IL_007d:  nop
    IL_007e:  br.s       IL_00b8

    IL_0080:  ldloc.1
    IL_0081:  ldloc.0
    IL_0082:  bge.un.s   IL_0089

    IL_0084:  ldc.i4.0
    IL_0085:  conv.i8
    IL_0086:  nop
    IL_0087:  br.s       IL_0090

    IL_0089:  ldloc.1
    IL_008a:  ldloc.0
    IL_008b:  sub
    IL_008c:  ldc.i4.1
    IL_008d:  conv.i8
    IL_008e:  add.ovf.un
    IL_008f:  nop
    IL_0090:  stloc.s    V_7
    IL_0092:  ldc.i4.0
    IL_0093:  conv.i8
    IL_0094:  stloc.s    V_8
    IL_0096:  ldloc.0
    IL_0097:  stloc.s    V_9
    IL_0099:  br.s       IL_00b1

    IL_009b:  ldloc.s    V_5
    IL_009d:  ldloc.s    V_8
    IL_009f:  conv.i
    IL_00a0:  ldloc.s    V_9
    IL_00a2:  stelem.i8
    IL_00a3:  ldloc.s    V_9
    IL_00a5:  ldc.i4.1
    IL_00a6:  conv.i8
    IL_00a7:  add
    IL_00a8:  stloc.s    V_9
    IL_00aa:  ldloc.s    V_8
    IL_00ac:  ldc.i4.1
    IL_00ad:  conv.i8
    IL_00ae:  add
    IL_00af:  stloc.s    V_8
    IL_00b1:  ldloc.s    V_8
    IL_00b3:  ldloc.s    V_7
    IL_00b5:  blt.un.s   IL_009b

    IL_00b7:  nop
    IL_00b8:  ldloc.s    V_5
    IL_00ba:  ret
  } 

  .method public static uint64[]  f19(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,uint64> f) cil managed
  {
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             uint64 V_2,
             uint64[] V_3,
             uint64 V_4,
             uint64 V_5)
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
    IL_001d:  ldloc.1
    IL_001e:  stloc.2
    IL_001f:  ldloc.2
    IL_0020:  ldc.i4.1
    IL_0021:  conv.i8
    IL_0022:  bge.un.s   IL_002a

    IL_0024:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_0029:  ret

    IL_002a:  ldloc.2
    IL_002b:  conv.ovf.i.un
    IL_002c:  newarr     [runtime]System.UInt64
    IL_0031:  stloc.3
    IL_0032:  ldc.i4.0
    IL_0033:  conv.i8
    IL_0034:  stloc.s    V_4
    IL_0036:  ldloc.0
    IL_0037:  stloc.s    V_5
    IL_0039:  br.s       IL_0050

    IL_003b:  ldloc.3
    IL_003c:  ldloc.s    V_4
    IL_003e:  conv.i
    IL_003f:  ldloc.s    V_5
    IL_0041:  stelem.i8
    IL_0042:  ldloc.s    V_5
    IL_0044:  ldc.i4.1
    IL_0045:  conv.i8
    IL_0046:  add
    IL_0047:  stloc.s    V_5
    IL_0049:  ldloc.s    V_4
    IL_004b:  ldc.i4.1
    IL_004c:  conv.i8
    IL_004d:  add
    IL_004e:  stloc.s    V_4
    IL_0050:  ldloc.s    V_4
    IL_0052:  ldloc.1
    IL_0053:  blt.un.s   IL_003b

    IL_0055:  ldloc.3
    IL_0056:  ret
  } 

  .method public static uint64[]  f20(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,uint64> f) cil managed
  {
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             uint64 V_2,
             uint64[] V_3,
             uint64 V_4,
             uint64 V_5)
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
    IL_0034:  ldloc.1
    IL_0035:  stloc.2
    IL_0036:  ldloc.2
    IL_0037:  ldc.i4.1
    IL_0038:  conv.i8
    IL_0039:  bge.un.s   IL_0041

    IL_003b:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_0040:  ret

    IL_0041:  ldloc.2
    IL_0042:  conv.ovf.i.un
    IL_0043:  newarr     [runtime]System.UInt64
    IL_0048:  stloc.3
    IL_0049:  ldc.i4.0
    IL_004a:  conv.i8
    IL_004b:  stloc.s    V_4
    IL_004d:  ldc.i4.1
    IL_004e:  conv.i8
    IL_004f:  stloc.s    V_5
    IL_0051:  br.s       IL_0067

    IL_0053:  ldloc.3
    IL_0054:  ldloc.s    V_4
    IL_0056:  conv.i
    IL_0057:  ldloc.s    V_5
    IL_0059:  stelem.i8
    IL_005a:  ldloc.s    V_5
    IL_005c:  ldloc.0
    IL_005d:  add
    IL_005e:  stloc.s    V_5
    IL_0060:  ldloc.s    V_4
    IL_0062:  ldc.i4.1
    IL_0063:  conv.i8
    IL_0064:  add
    IL_0065:  stloc.s    V_4
    IL_0067:  ldloc.s    V_4
    IL_0069:  ldloc.1
    IL_006a:  blt.un.s   IL_0053

    IL_006c:  ldloc.3
    IL_006d:  ret
  } 

  .method public static uint64[]  f21(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,uint64> f) cil managed
  {
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             uint64 V_2,
             uint64[] V_3,
             uint64 V_4,
             uint64 V_5)
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
    IL_001b:  ldloc.1
    IL_001c:  stloc.2
    IL_001d:  ldloc.2
    IL_001e:  ldc.i4.1
    IL_001f:  conv.i8
    IL_0020:  bge.un.s   IL_0028

    IL_0022:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_0027:  ret

    IL_0028:  ldloc.2
    IL_0029:  conv.ovf.i.un
    IL_002a:  newarr     [runtime]System.UInt64
    IL_002f:  stloc.3
    IL_0030:  ldc.i4.0
    IL_0031:  conv.i8
    IL_0032:  stloc.s    V_4
    IL_0034:  ldc.i4.1
    IL_0035:  conv.i8
    IL_0036:  stloc.s    V_5
    IL_0038:  br.s       IL_004f

    IL_003a:  ldloc.3
    IL_003b:  ldloc.s    V_4
    IL_003d:  conv.i
    IL_003e:  ldloc.s    V_5
    IL_0040:  stelem.i8
    IL_0041:  ldloc.s    V_5
    IL_0043:  ldc.i4.1
    IL_0044:  conv.i8
    IL_0045:  add
    IL_0046:  stloc.s    V_5
    IL_0048:  ldloc.s    V_4
    IL_004a:  ldc.i4.1
    IL_004b:  conv.i8
    IL_004c:  add
    IL_004d:  stloc.s    V_4
    IL_004f:  ldloc.s    V_4
    IL_0051:  ldloc.1
    IL_0052:  blt.un.s   IL_003a

    IL_0054:  ldloc.3
    IL_0055:  ret
  } 

  .method public static uint64[]  f22(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,uint64> f,
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
             uint64 V_5,
             uint64[] V_6,
             bool V_7,
             uint64 V_8,
             uint64 V_9,
             uint64 V_10)
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
    IL_003f:  ldloc.2
    IL_0040:  ldloc.0
    IL_0041:  bge.un.s   IL_0048

    IL_0043:  ldc.i4.0
    IL_0044:  conv.i8
    IL_0045:  nop
    IL_0046:  br.s       IL_0051

    IL_0048:  ldloc.2
    IL_0049:  ldloc.0
    IL_004a:  sub
    IL_004b:  ldloc.1
    IL_004c:  div.un
    IL_004d:  ldc.i4.1
    IL_004e:  conv.i8
    IL_004f:  add.ovf.un
    IL_0050:  nop
    IL_0051:  stloc.s    V_5
    IL_0053:  ldloc.s    V_5
    IL_0055:  ldc.i4.1
    IL_0056:  conv.i8
    IL_0057:  bge.un.s   IL_005f

    IL_0059:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_005e:  ret

    IL_005f:  ldloc.s    V_5
    IL_0061:  conv.ovf.i.un
    IL_0062:  newarr     [runtime]System.UInt64
    IL_0067:  stloc.s    V_6
    IL_0069:  ldloc.s    V_4
    IL_006b:  brfalse.s  IL_009d

    IL_006d:  ldc.i4.1
    IL_006e:  stloc.s    V_7
    IL_0070:  ldc.i4.0
    IL_0071:  conv.i8
    IL_0072:  stloc.s    V_8
    IL_0074:  ldloc.0
    IL_0075:  stloc.s    V_9
    IL_0077:  br.s       IL_0096

    IL_0079:  ldloc.s    V_6
    IL_007b:  ldloc.s    V_8
    IL_007d:  conv.i
    IL_007e:  ldloc.s    V_9
    IL_0080:  stelem.i8
    IL_0081:  ldloc.s    V_9
    IL_0083:  ldloc.1
    IL_0084:  add
    IL_0085:  stloc.s    V_9
    IL_0087:  ldloc.s    V_8
    IL_0089:  ldc.i4.1
    IL_008a:  conv.i8
    IL_008b:  add
    IL_008c:  stloc.s    V_8
    IL_008e:  ldloc.s    V_8
    IL_0090:  ldc.i4.0
    IL_0091:  conv.i8
    IL_0092:  cgt.un
    IL_0094:  stloc.s    V_7
    IL_0096:  ldloc.s    V_7
    IL_0098:  brtrue.s   IL_0079

    IL_009a:  nop
    IL_009b:  br.s       IL_00d6

    IL_009d:  ldloc.2
    IL_009e:  ldloc.0
    IL_009f:  bge.un.s   IL_00a6

    IL_00a1:  ldc.i4.0
    IL_00a2:  conv.i8
    IL_00a3:  nop
    IL_00a4:  br.s       IL_00af

    IL_00a6:  ldloc.2
    IL_00a7:  ldloc.0
    IL_00a8:  sub
    IL_00a9:  ldloc.1
    IL_00aa:  div.un
    IL_00ab:  ldc.i4.1
    IL_00ac:  conv.i8
    IL_00ad:  add.ovf.un
    IL_00ae:  nop
    IL_00af:  stloc.s    V_8
    IL_00b1:  ldc.i4.0
    IL_00b2:  conv.i8
    IL_00b3:  stloc.s    V_9
    IL_00b5:  ldloc.0
    IL_00b6:  stloc.s    V_10
    IL_00b8:  br.s       IL_00cf

    IL_00ba:  ldloc.s    V_6
    IL_00bc:  ldloc.s    V_9
    IL_00be:  conv.i
    IL_00bf:  ldloc.s    V_10
    IL_00c1:  stelem.i8
    IL_00c2:  ldloc.s    V_10
    IL_00c4:  ldloc.1
    IL_00c5:  add
    IL_00c6:  stloc.s    V_10
    IL_00c8:  ldloc.s    V_9
    IL_00ca:  ldc.i4.1
    IL_00cb:  conv.i8
    IL_00cc:  add
    IL_00cd:  stloc.s    V_9
    IL_00cf:  ldloc.s    V_9
    IL_00d1:  ldloc.s    V_8
    IL_00d3:  blt.un.s   IL_00ba

    IL_00d5:  nop
    IL_00d6:  ldloc.s    V_6
    IL_00d8:  ret
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






