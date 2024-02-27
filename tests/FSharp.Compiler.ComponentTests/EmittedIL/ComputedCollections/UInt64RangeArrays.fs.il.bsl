




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
    IL_003a:  brfalse.s  IL_0078

    IL_003c:  ldc.i4.0
    IL_003d:  conv.i8
    IL_003e:  stloc.s    V_4
    IL_0040:  ldarg.0
    IL_0041:  stloc.s    V_5
    IL_0043:  ldloc.3
    IL_0044:  ldloc.s    V_4
    IL_0046:  conv.i
    IL_0047:  ldloc.s    V_5
    IL_0049:  stelem.i8
    IL_004a:  ldloc.s    V_5
    IL_004c:  ldc.i4.1
    IL_004d:  conv.i8
    IL_004e:  add
    IL_004f:  stloc.s    V_5
    IL_0051:  ldloc.s    V_4
    IL_0053:  ldc.i4.1
    IL_0054:  conv.i8
    IL_0055:  add
    IL_0056:  stloc.s    V_4
    IL_0058:  br.s       IL_006f

    IL_005a:  ldloc.3
    IL_005b:  ldloc.s    V_4
    IL_005d:  conv.i
    IL_005e:  ldloc.s    V_5
    IL_0060:  stelem.i8
    IL_0061:  ldloc.s    V_5
    IL_0063:  ldc.i4.1
    IL_0064:  conv.i8
    IL_0065:  add
    IL_0066:  stloc.s    V_5
    IL_0068:  ldloc.s    V_4
    IL_006a:  ldc.i4.1
    IL_006b:  conv.i8
    IL_006c:  add
    IL_006d:  stloc.s    V_4
    IL_006f:  ldloc.s    V_4
    IL_0071:  ldc.i4.0
    IL_0072:  conv.i8
    IL_0073:  bgt.un.s   IL_005a

    IL_0075:  nop
    IL_0076:  br.s       IL_00af

    IL_0078:  ldarg.1
    IL_0079:  ldarg.0
    IL_007a:  bge.un.s   IL_0081

    IL_007c:  ldc.i4.0
    IL_007d:  conv.i8
    IL_007e:  nop
    IL_007f:  br.s       IL_0088

    IL_0081:  ldarg.1
    IL_0082:  ldarg.0
    IL_0083:  sub
    IL_0084:  ldc.i4.1
    IL_0085:  conv.i8
    IL_0086:  add.ovf.un
    IL_0087:  nop
    IL_0088:  stloc.s    V_4
    IL_008a:  ldc.i4.0
    IL_008b:  conv.i8
    IL_008c:  stloc.s    V_5
    IL_008e:  ldarg.0
    IL_008f:  stloc.s    V_6
    IL_0091:  br.s       IL_00a8

    IL_0093:  ldloc.3
    IL_0094:  ldloc.s    V_5
    IL_0096:  conv.i
    IL_0097:  ldloc.s    V_6
    IL_0099:  stelem.i8
    IL_009a:  ldloc.s    V_6
    IL_009c:  ldc.i4.1
    IL_009d:  conv.i8
    IL_009e:  add
    IL_009f:  stloc.s    V_6
    IL_00a1:  ldloc.s    V_5
    IL_00a3:  ldc.i4.1
    IL_00a4:  conv.i8
    IL_00a5:  add
    IL_00a6:  stloc.s    V_5
    IL_00a8:  ldloc.s    V_5
    IL_00aa:  ldloc.s    V_4
    IL_00ac:  blt.un.s   IL_0093

    IL_00ae:  nop
    IL_00af:  ldloc.3
    IL_00b0:  ret
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
    IL_001e:  br.s       IL_002d

    IL_0020:  ldc.i4.s   10
    IL_0022:  conv.i8
    IL_0023:  ldc.i4.1
    IL_0024:  conv.i8
    IL_0025:  sub
    IL_0026:  ldarg.0
    IL_0027:  conv.i8
    IL_0028:  div.un
    IL_0029:  ldc.i4.1
    IL_002a:  conv.i8
    IL_002b:  add.ovf.un
    IL_002c:  nop
    IL_002d:  stloc.0
    IL_002e:  ldloc.0
    IL_002f:  stloc.1
    IL_0030:  ldloc.1
    IL_0031:  ldc.i4.1
    IL_0032:  conv.i8
    IL_0033:  bge.un.s   IL_003b

    IL_0035:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_003a:  ret

    IL_003b:  ldloc.1
    IL_003c:  conv.ovf.i.un
    IL_003d:  newarr     [runtime]System.UInt64
    IL_0042:  stloc.2
    IL_0043:  ldc.i4.0
    IL_0044:  conv.i8
    IL_0045:  stloc.3
    IL_0046:  ldc.i4.1
    IL_0047:  conv.i8
    IL_0048:  stloc.s    V_4
    IL_004a:  br.s       IL_005d

    IL_004c:  ldloc.2
    IL_004d:  ldloc.3
    IL_004e:  conv.i
    IL_004f:  ldloc.s    V_4
    IL_0051:  stelem.i8
    IL_0052:  ldloc.s    V_4
    IL_0054:  ldarg.0
    IL_0055:  add
    IL_0056:  stloc.s    V_4
    IL_0058:  ldloc.3
    IL_0059:  ldc.i4.1
    IL_005a:  conv.i8
    IL_005b:  add
    IL_005c:  stloc.3
    IL_005d:  ldloc.3
    IL_005e:  ldloc.0
    IL_005f:  blt.un.s   IL_004c

    IL_0061:  ldloc.2
    IL_0062:  ret
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
    IL_001c:  br.s       IL_002a

    IL_001e:  ldc.i4.s   10
    IL_0020:  conv.i8
    IL_0021:  ldarg.0
    IL_0022:  sub
    IL_0023:  ldarg.1
    IL_0024:  conv.i8
    IL_0025:  div.un
    IL_0026:  ldc.i4.1
    IL_0027:  conv.i8
    IL_0028:  add.ovf.un
    IL_0029:  nop
    IL_002a:  stloc.0
    IL_002b:  ldloc.0
    IL_002c:  stloc.1
    IL_002d:  ldloc.1
    IL_002e:  ldc.i4.1
    IL_002f:  conv.i8
    IL_0030:  bge.un.s   IL_0038

    IL_0032:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_0037:  ret

    IL_0038:  ldloc.1
    IL_0039:  conv.ovf.i.un
    IL_003a:  newarr     [runtime]System.UInt64
    IL_003f:  stloc.2
    IL_0040:  ldc.i4.0
    IL_0041:  conv.i8
    IL_0042:  stloc.3
    IL_0043:  ldarg.0
    IL_0044:  stloc.s    V_4
    IL_0046:  br.s       IL_0059

    IL_0048:  ldloc.2
    IL_0049:  ldloc.3
    IL_004a:  conv.i
    IL_004b:  ldloc.s    V_4
    IL_004d:  stelem.i8
    IL_004e:  ldloc.s    V_4
    IL_0050:  ldarg.1
    IL_0051:  add
    IL_0052:  stloc.s    V_4
    IL_0054:  ldloc.3
    IL_0055:  ldc.i4.1
    IL_0056:  conv.i8
    IL_0057:  add
    IL_0058:  stloc.3
    IL_0059:  ldloc.3
    IL_005a:  ldloc.0
    IL_005b:  blt.un.s   IL_0048

    IL_005d:  ldloc.2
    IL_005e:  ret
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
    IL_003a:  brfalse.s  IL_0078

    IL_003c:  ldc.i4.0
    IL_003d:  conv.i8
    IL_003e:  stloc.s    V_4
    IL_0040:  ldarg.0
    IL_0041:  stloc.s    V_5
    IL_0043:  ldloc.3
    IL_0044:  ldloc.s    V_4
    IL_0046:  conv.i
    IL_0047:  ldloc.s    V_5
    IL_0049:  stelem.i8
    IL_004a:  ldloc.s    V_5
    IL_004c:  ldc.i4.1
    IL_004d:  conv.i8
    IL_004e:  add
    IL_004f:  stloc.s    V_5
    IL_0051:  ldloc.s    V_4
    IL_0053:  ldc.i4.1
    IL_0054:  conv.i8
    IL_0055:  add
    IL_0056:  stloc.s    V_4
    IL_0058:  br.s       IL_006f

    IL_005a:  ldloc.3
    IL_005b:  ldloc.s    V_4
    IL_005d:  conv.i
    IL_005e:  ldloc.s    V_5
    IL_0060:  stelem.i8
    IL_0061:  ldloc.s    V_5
    IL_0063:  ldc.i4.1
    IL_0064:  conv.i8
    IL_0065:  add
    IL_0066:  stloc.s    V_5
    IL_0068:  ldloc.s    V_4
    IL_006a:  ldc.i4.1
    IL_006b:  conv.i8
    IL_006c:  add
    IL_006d:  stloc.s    V_4
    IL_006f:  ldloc.s    V_4
    IL_0071:  ldc.i4.0
    IL_0072:  conv.i8
    IL_0073:  bgt.un.s   IL_005a

    IL_0075:  nop
    IL_0076:  br.s       IL_00af

    IL_0078:  ldarg.1
    IL_0079:  ldarg.0
    IL_007a:  bge.un.s   IL_0081

    IL_007c:  ldc.i4.0
    IL_007d:  conv.i8
    IL_007e:  nop
    IL_007f:  br.s       IL_0088

    IL_0081:  ldarg.1
    IL_0082:  ldarg.0
    IL_0083:  sub
    IL_0084:  ldc.i4.1
    IL_0085:  conv.i8
    IL_0086:  add.ovf.un
    IL_0087:  nop
    IL_0088:  stloc.s    V_4
    IL_008a:  ldc.i4.0
    IL_008b:  conv.i8
    IL_008c:  stloc.s    V_5
    IL_008e:  ldarg.0
    IL_008f:  stloc.s    V_6
    IL_0091:  br.s       IL_00a8

    IL_0093:  ldloc.3
    IL_0094:  ldloc.s    V_5
    IL_0096:  conv.i
    IL_0097:  ldloc.s    V_6
    IL_0099:  stelem.i8
    IL_009a:  ldloc.s    V_6
    IL_009c:  ldc.i4.1
    IL_009d:  conv.i8
    IL_009e:  add
    IL_009f:  stloc.s    V_6
    IL_00a1:  ldloc.s    V_5
    IL_00a3:  ldc.i4.1
    IL_00a4:  conv.i8
    IL_00a5:  add
    IL_00a6:  stloc.s    V_5
    IL_00a8:  ldloc.s    V_5
    IL_00aa:  ldloc.s    V_4
    IL_00ac:  blt.un.s   IL_0093

    IL_00ae:  nop
    IL_00af:  ldloc.3
    IL_00b0:  ret
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
    IL_001a:  br.s       IL_0027

    IL_001c:  ldarg.1
    IL_001d:  ldc.i4.1
    IL_001e:  conv.i8
    IL_001f:  sub
    IL_0020:  ldarg.0
    IL_0021:  conv.i8
    IL_0022:  div.un
    IL_0023:  ldc.i4.1
    IL_0024:  conv.i8
    IL_0025:  add.ovf.un
    IL_0026:  nop
    IL_0027:  stloc.0
    IL_0028:  ldloc.0
    IL_0029:  stloc.1
    IL_002a:  ldloc.1
    IL_002b:  ldc.i4.1
    IL_002c:  conv.i8
    IL_002d:  bge.un.s   IL_0035

    IL_002f:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_0034:  ret

    IL_0035:  ldloc.1
    IL_0036:  conv.ovf.i.un
    IL_0037:  newarr     [runtime]System.UInt64
    IL_003c:  stloc.2
    IL_003d:  ldc.i4.0
    IL_003e:  conv.i8
    IL_003f:  stloc.3
    IL_0040:  ldc.i4.1
    IL_0041:  conv.i8
    IL_0042:  stloc.s    V_4
    IL_0044:  br.s       IL_0057

    IL_0046:  ldloc.2
    IL_0047:  ldloc.3
    IL_0048:  conv.i
    IL_0049:  ldloc.s    V_4
    IL_004b:  stelem.i8
    IL_004c:  ldloc.s    V_4
    IL_004e:  ldarg.0
    IL_004f:  add
    IL_0050:  stloc.s    V_4
    IL_0052:  ldloc.3
    IL_0053:  ldc.i4.1
    IL_0054:  conv.i8
    IL_0055:  add
    IL_0056:  stloc.3
    IL_0057:  ldloc.3
    IL_0058:  ldloc.0
    IL_0059:  blt.un.s   IL_0046

    IL_005b:  ldloc.2
    IL_005c:  ret
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
    IL_0028:  ldarg.1
    IL_0029:  brtrue.s   IL_0037

    IL_002b:  ldarg.0
    IL_002c:  ldarg.1
    IL_002d:  ldarg.2
    IL_002e:  call       class [runtime]System.Collections.Generic.IEnumerable`1<uint64> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeUInt64(uint64,
                                                                                                                                                                             uint64,
                                                                                                                                                                             uint64)
    IL_0033:  pop
    IL_0034:  nop
    IL_0035:  br.s       IL_0038

    IL_0037:  nop
    IL_0038:  ldarg.2
    IL_0039:  ldarg.0
    IL_003a:  bge.un.s   IL_0041

    IL_003c:  ldc.i4.0
    IL_003d:  conv.i8
    IL_003e:  nop
    IL_003f:  br.s       IL_004b

    IL_0041:  ldarg.2
    IL_0042:  ldarg.0
    IL_0043:  sub
    IL_0044:  ldarg.1
    IL_0045:  conv.i8
    IL_0046:  div.un
    IL_0047:  ldc.i4.1
    IL_0048:  conv.i8
    IL_0049:  add.ovf.un
    IL_004a:  nop
    IL_004b:  stloc.2
    IL_004c:  ldloc.2
    IL_004d:  ldc.i4.1
    IL_004e:  conv.i8
    IL_004f:  bge.un.s   IL_0057

    IL_0051:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_0056:  ret

    IL_0057:  ldloc.2
    IL_0058:  conv.ovf.i.un
    IL_0059:  newarr     [runtime]System.UInt64
    IL_005e:  stloc.3
    IL_005f:  ldloc.1
    IL_0060:  brfalse.s  IL_009c

    IL_0062:  ldc.i4.0
    IL_0063:  conv.i8
    IL_0064:  stloc.s    V_4
    IL_0066:  ldarg.0
    IL_0067:  stloc.s    V_5
    IL_0069:  ldloc.3
    IL_006a:  ldloc.s    V_4
    IL_006c:  conv.i
    IL_006d:  ldloc.s    V_5
    IL_006f:  stelem.i8
    IL_0070:  ldloc.s    V_5
    IL_0072:  ldarg.1
    IL_0073:  add
    IL_0074:  stloc.s    V_5
    IL_0076:  ldloc.s    V_4
    IL_0078:  ldc.i4.1
    IL_0079:  conv.i8
    IL_007a:  add
    IL_007b:  stloc.s    V_4
    IL_007d:  br.s       IL_0093

    IL_007f:  ldloc.3
    IL_0080:  ldloc.s    V_4
    IL_0082:  conv.i
    IL_0083:  ldloc.s    V_5
    IL_0085:  stelem.i8
    IL_0086:  ldloc.s    V_5
    IL_0088:  ldarg.1
    IL_0089:  add
    IL_008a:  stloc.s    V_5
    IL_008c:  ldloc.s    V_4
    IL_008e:  ldc.i4.1
    IL_008f:  conv.i8
    IL_0090:  add
    IL_0091:  stloc.s    V_4
    IL_0093:  ldloc.s    V_4
    IL_0095:  ldc.i4.0
    IL_0096:  conv.i8
    IL_0097:  bgt.un.s   IL_007f

    IL_0099:  nop
    IL_009a:  br.s       IL_00e5

    IL_009c:  ldarg.1
    IL_009d:  brtrue.s   IL_00ab

    IL_009f:  ldarg.0
    IL_00a0:  ldarg.1
    IL_00a1:  ldarg.2
    IL_00a2:  call       class [runtime]System.Collections.Generic.IEnumerable`1<uint64> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeUInt64(uint64,
                                                                                                                                                                             uint64,
                                                                                                                                                                             uint64)
    IL_00a7:  pop
    IL_00a8:  nop
    IL_00a9:  br.s       IL_00ac

    IL_00ab:  nop
    IL_00ac:  ldarg.2
    IL_00ad:  ldarg.0
    IL_00ae:  bge.un.s   IL_00b5

    IL_00b0:  ldc.i4.0
    IL_00b1:  conv.i8
    IL_00b2:  nop
    IL_00b3:  br.s       IL_00bf

    IL_00b5:  ldarg.2
    IL_00b6:  ldarg.0
    IL_00b7:  sub
    IL_00b8:  ldarg.1
    IL_00b9:  conv.i8
    IL_00ba:  div.un
    IL_00bb:  ldc.i4.1
    IL_00bc:  conv.i8
    IL_00bd:  add.ovf.un
    IL_00be:  nop
    IL_00bf:  stloc.s    V_4
    IL_00c1:  ldc.i4.0
    IL_00c2:  conv.i8
    IL_00c3:  stloc.s    V_5
    IL_00c5:  ldarg.0
    IL_00c6:  stloc.s    V_6
    IL_00c8:  br.s       IL_00de

    IL_00ca:  ldloc.3
    IL_00cb:  ldloc.s    V_5
    IL_00cd:  conv.i
    IL_00ce:  ldloc.s    V_6
    IL_00d0:  stelem.i8
    IL_00d1:  ldloc.s    V_6
    IL_00d3:  ldarg.1
    IL_00d4:  add
    IL_00d5:  stloc.s    V_6
    IL_00d7:  ldloc.s    V_5
    IL_00d9:  ldc.i4.1
    IL_00da:  conv.i8
    IL_00db:  add
    IL_00dc:  stloc.s    V_5
    IL_00de:  ldloc.s    V_5
    IL_00e0:  ldloc.s    V_4
    IL_00e2:  blt.un.s   IL_00ca

    IL_00e4:  nop
    IL_00e5:  ldloc.3
    IL_00e6:  ret
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
    IL_004d:  brfalse.s  IL_008d

    IL_004f:  ldc.i4.0
    IL_0050:  conv.i8
    IL_0051:  stloc.s    V_6
    IL_0053:  ldloc.0
    IL_0054:  stloc.s    V_7
    IL_0056:  ldloc.s    V_5
    IL_0058:  ldloc.s    V_6
    IL_005a:  conv.i
    IL_005b:  ldloc.s    V_7
    IL_005d:  stelem.i8
    IL_005e:  ldloc.s    V_7
    IL_0060:  ldc.i4.1
    IL_0061:  conv.i8
    IL_0062:  add
    IL_0063:  stloc.s    V_7
    IL_0065:  ldloc.s    V_6
    IL_0067:  ldc.i4.1
    IL_0068:  conv.i8
    IL_0069:  add
    IL_006a:  stloc.s    V_6
    IL_006c:  br.s       IL_0084

    IL_006e:  ldloc.s    V_5
    IL_0070:  ldloc.s    V_6
    IL_0072:  conv.i
    IL_0073:  ldloc.s    V_7
    IL_0075:  stelem.i8
    IL_0076:  ldloc.s    V_7
    IL_0078:  ldc.i4.1
    IL_0079:  conv.i8
    IL_007a:  add
    IL_007b:  stloc.s    V_7
    IL_007d:  ldloc.s    V_6
    IL_007f:  ldc.i4.1
    IL_0080:  conv.i8
    IL_0081:  add
    IL_0082:  stloc.s    V_6
    IL_0084:  ldloc.s    V_6
    IL_0086:  ldc.i4.0
    IL_0087:  conv.i8
    IL_0088:  bgt.un.s   IL_006e

    IL_008a:  nop
    IL_008b:  br.s       IL_00c5

    IL_008d:  ldloc.1
    IL_008e:  ldloc.0
    IL_008f:  bge.un.s   IL_0096

    IL_0091:  ldc.i4.0
    IL_0092:  conv.i8
    IL_0093:  nop
    IL_0094:  br.s       IL_009d

    IL_0096:  ldloc.1
    IL_0097:  ldloc.0
    IL_0098:  sub
    IL_0099:  ldc.i4.1
    IL_009a:  conv.i8
    IL_009b:  add.ovf.un
    IL_009c:  nop
    IL_009d:  stloc.s    V_6
    IL_009f:  ldc.i4.0
    IL_00a0:  conv.i8
    IL_00a1:  stloc.s    V_7
    IL_00a3:  ldloc.0
    IL_00a4:  stloc.s    V_8
    IL_00a6:  br.s       IL_00be

    IL_00a8:  ldloc.s    V_5
    IL_00aa:  ldloc.s    V_7
    IL_00ac:  conv.i
    IL_00ad:  ldloc.s    V_8
    IL_00af:  stelem.i8
    IL_00b0:  ldloc.s    V_8
    IL_00b2:  ldc.i4.1
    IL_00b3:  conv.i8
    IL_00b4:  add
    IL_00b5:  stloc.s    V_8
    IL_00b7:  ldloc.s    V_7
    IL_00b9:  ldc.i4.1
    IL_00ba:  conv.i8
    IL_00bb:  add
    IL_00bc:  stloc.s    V_7
    IL_00be:  ldloc.s    V_7
    IL_00c0:  ldloc.s    V_6
    IL_00c2:  blt.un.s   IL_00a8

    IL_00c4:  nop
    IL_00c5:  ldloc.s    V_5
    IL_00c7:  ret
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
    IL_0025:  br.s       IL_0034

    IL_0027:  ldc.i4.s   10
    IL_0029:  conv.i8
    IL_002a:  ldc.i4.1
    IL_002b:  conv.i8
    IL_002c:  sub
    IL_002d:  ldloc.0
    IL_002e:  conv.i8
    IL_002f:  div.un
    IL_0030:  ldc.i4.1
    IL_0031:  conv.i8
    IL_0032:  add.ovf.un
    IL_0033:  nop
    IL_0034:  stloc.1
    IL_0035:  ldloc.1
    IL_0036:  stloc.2
    IL_0037:  ldloc.2
    IL_0038:  ldc.i4.1
    IL_0039:  conv.i8
    IL_003a:  bge.un.s   IL_0042

    IL_003c:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_0041:  ret

    IL_0042:  ldloc.2
    IL_0043:  conv.ovf.i.un
    IL_0044:  newarr     [runtime]System.UInt64
    IL_0049:  stloc.3
    IL_004a:  ldc.i4.0
    IL_004b:  conv.i8
    IL_004c:  stloc.s    V_4
    IL_004e:  ldc.i4.1
    IL_004f:  conv.i8
    IL_0050:  stloc.s    V_5
    IL_0052:  br.s       IL_0068

    IL_0054:  ldloc.3
    IL_0055:  ldloc.s    V_4
    IL_0057:  conv.i
    IL_0058:  ldloc.s    V_5
    IL_005a:  stelem.i8
    IL_005b:  ldloc.s    V_5
    IL_005d:  ldloc.0
    IL_005e:  add
    IL_005f:  stloc.s    V_5
    IL_0061:  ldloc.s    V_4
    IL_0063:  ldc.i4.1
    IL_0064:  conv.i8
    IL_0065:  add
    IL_0066:  stloc.s    V_4
    IL_0068:  ldloc.s    V_4
    IL_006a:  ldloc.1
    IL_006b:  blt.un.s   IL_0054

    IL_006d:  ldloc.3
    IL_006e:  ret
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
    IL_0040:  ldloc.1
    IL_0041:  brtrue.s   IL_004f

    IL_0043:  ldloc.0
    IL_0044:  ldloc.1
    IL_0045:  ldloc.2
    IL_0046:  call       class [runtime]System.Collections.Generic.IEnumerable`1<uint64> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeUInt64(uint64,
                                                                                                                                                                             uint64,
                                                                                                                                                                             uint64)
    IL_004b:  pop
    IL_004c:  nop
    IL_004d:  br.s       IL_0050

    IL_004f:  nop
    IL_0050:  ldloc.2
    IL_0051:  ldloc.0
    IL_0052:  bge.un.s   IL_0059

    IL_0054:  ldc.i4.0
    IL_0055:  conv.i8
    IL_0056:  nop
    IL_0057:  br.s       IL_0063

    IL_0059:  ldloc.2
    IL_005a:  ldloc.0
    IL_005b:  sub
    IL_005c:  ldloc.1
    IL_005d:  conv.i8
    IL_005e:  div.un
    IL_005f:  ldc.i4.1
    IL_0060:  conv.i8
    IL_0061:  add.ovf.un
    IL_0062:  nop
    IL_0063:  stloc.s    V_5
    IL_0065:  ldloc.s    V_5
    IL_0067:  ldc.i4.1
    IL_0068:  conv.i8
    IL_0069:  bge.un.s   IL_0071

    IL_006b:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_0070:  ret

    IL_0071:  ldloc.s    V_5
    IL_0073:  conv.ovf.i.un
    IL_0074:  newarr     [runtime]System.UInt64
    IL_0079:  stloc.s    V_6
    IL_007b:  ldloc.s    V_4
    IL_007d:  brfalse.s  IL_00bb

    IL_007f:  ldc.i4.0
    IL_0080:  conv.i8
    IL_0081:  stloc.s    V_7
    IL_0083:  ldloc.0
    IL_0084:  stloc.s    V_8
    IL_0086:  ldloc.s    V_6
    IL_0088:  ldloc.s    V_7
    IL_008a:  conv.i
    IL_008b:  ldloc.s    V_8
    IL_008d:  stelem.i8
    IL_008e:  ldloc.s    V_8
    IL_0090:  ldloc.1
    IL_0091:  add
    IL_0092:  stloc.s    V_8
    IL_0094:  ldloc.s    V_7
    IL_0096:  ldc.i4.1
    IL_0097:  conv.i8
    IL_0098:  add
    IL_0099:  stloc.s    V_7
    IL_009b:  br.s       IL_00b2

    IL_009d:  ldloc.s    V_6
    IL_009f:  ldloc.s    V_7
    IL_00a1:  conv.i
    IL_00a2:  ldloc.s    V_8
    IL_00a4:  stelem.i8
    IL_00a5:  ldloc.s    V_8
    IL_00a7:  ldloc.1
    IL_00a8:  add
    IL_00a9:  stloc.s    V_8
    IL_00ab:  ldloc.s    V_7
    IL_00ad:  ldc.i4.1
    IL_00ae:  conv.i8
    IL_00af:  add
    IL_00b0:  stloc.s    V_7
    IL_00b2:  ldloc.s    V_7
    IL_00b4:  ldc.i4.0
    IL_00b5:  conv.i8
    IL_00b6:  bgt.un.s   IL_009d

    IL_00b8:  nop
    IL_00b9:  br.s       IL_0105

    IL_00bb:  ldloc.1
    IL_00bc:  brtrue.s   IL_00ca

    IL_00be:  ldloc.0
    IL_00bf:  ldloc.1
    IL_00c0:  ldloc.2
    IL_00c1:  call       class [runtime]System.Collections.Generic.IEnumerable`1<uint64> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeUInt64(uint64,
                                                                                                                                                                             uint64,
                                                                                                                                                                             uint64)
    IL_00c6:  pop
    IL_00c7:  nop
    IL_00c8:  br.s       IL_00cb

    IL_00ca:  nop
    IL_00cb:  ldloc.2
    IL_00cc:  ldloc.0
    IL_00cd:  bge.un.s   IL_00d4

    IL_00cf:  ldc.i4.0
    IL_00d0:  conv.i8
    IL_00d1:  nop
    IL_00d2:  br.s       IL_00de

    IL_00d4:  ldloc.2
    IL_00d5:  ldloc.0
    IL_00d6:  sub
    IL_00d7:  ldloc.1
    IL_00d8:  conv.i8
    IL_00d9:  div.un
    IL_00da:  ldc.i4.1
    IL_00db:  conv.i8
    IL_00dc:  add.ovf.un
    IL_00dd:  nop
    IL_00de:  stloc.s    V_7
    IL_00e0:  ldc.i4.0
    IL_00e1:  conv.i8
    IL_00e2:  stloc.s    V_8
    IL_00e4:  ldloc.0
    IL_00e5:  stloc.s    V_9
    IL_00e7:  br.s       IL_00fe

    IL_00e9:  ldloc.s    V_6
    IL_00eb:  ldloc.s    V_8
    IL_00ed:  conv.i
    IL_00ee:  ldloc.s    V_9
    IL_00f0:  stelem.i8
    IL_00f1:  ldloc.s    V_9
    IL_00f3:  ldloc.1
    IL_00f4:  add
    IL_00f5:  stloc.s    V_9
    IL_00f7:  ldloc.s    V_8
    IL_00f9:  ldc.i4.1
    IL_00fa:  conv.i8
    IL_00fb:  add
    IL_00fc:  stloc.s    V_8
    IL_00fe:  ldloc.s    V_8
    IL_0100:  ldloc.s    V_7
    IL_0102:  blt.un.s   IL_00e9

    IL_0104:  nop
    IL_0105:  ldloc.s    V_6
    IL_0107:  ret
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






