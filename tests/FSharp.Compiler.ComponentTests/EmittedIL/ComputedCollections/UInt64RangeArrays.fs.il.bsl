




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
    IL_0019:  brtrue.s   IL_0021

    IL_001b:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_0020:  ret

    IL_0021:  ldloc.1
    IL_0022:  conv.ovf.i.un
    IL_0023:  newarr     [runtime]System.UInt64
    IL_0028:  stloc.2
    IL_0029:  ldc.i4.0
    IL_002a:  conv.i8
    IL_002b:  stloc.3
    IL_002c:  ldarg.0
    IL_002d:  stloc.s    V_4
    IL_002f:  br.s       IL_0043

    IL_0031:  ldloc.2
    IL_0032:  ldloc.3
    IL_0033:  conv.i
    IL_0034:  ldloc.s    V_4
    IL_0036:  stelem.i8
    IL_0037:  ldloc.s    V_4
    IL_0039:  ldc.i4.1
    IL_003a:  conv.i8
    IL_003b:  add
    IL_003c:  stloc.s    V_4
    IL_003e:  ldloc.3
    IL_003f:  ldc.i4.1
    IL_0040:  conv.i8
    IL_0041:  add
    IL_0042:  stloc.3
    IL_0043:  ldloc.3
    IL_0044:  ldloc.0
    IL_0045:  blt.un.s   IL_0031

    IL_0047:  ldloc.2
    IL_0048:  ret
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
    IL_0017:  brtrue.s   IL_001f

    IL_0019:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_001e:  ret

    IL_001f:  ldloc.1
    IL_0020:  conv.ovf.i.un
    IL_0021:  newarr     [runtime]System.UInt64
    IL_0026:  stloc.2
    IL_0027:  ldc.i4.0
    IL_0028:  conv.i8
    IL_0029:  stloc.3
    IL_002a:  ldc.i4.1
    IL_002b:  conv.i8
    IL_002c:  stloc.s    V_4
    IL_002e:  br.s       IL_0042

    IL_0030:  ldloc.2
    IL_0031:  ldloc.3
    IL_0032:  conv.i
    IL_0033:  ldloc.s    V_4
    IL_0035:  stelem.i8
    IL_0036:  ldloc.s    V_4
    IL_0038:  ldc.i4.1
    IL_0039:  conv.i8
    IL_003a:  add
    IL_003b:  stloc.s    V_4
    IL_003d:  ldloc.3
    IL_003e:  ldc.i4.1
    IL_003f:  conv.i8
    IL_0040:  add
    IL_0041:  stloc.3
    IL_0042:  ldloc.3
    IL_0043:  ldloc.0
    IL_0044:  blt.un.s   IL_0030

    IL_0046:  ldloc.2
    IL_0047:  ret
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
    IL_0027:  brtrue.s   IL_002f

    IL_0029:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_002e:  ret

    IL_002f:  ldloc.2
    IL_0030:  conv.ovf.i.un
    IL_0031:  newarr     [runtime]System.UInt64
    IL_0036:  stloc.3
    IL_0037:  ldloc.1
    IL_0038:  brfalse.s  IL_006a

    IL_003a:  ldc.i4.1
    IL_003b:  stloc.s    V_4
    IL_003d:  ldc.i4.0
    IL_003e:  conv.i8
    IL_003f:  stloc.s    V_5
    IL_0041:  ldarg.0
    IL_0042:  stloc.s    V_6
    IL_0044:  br.s       IL_0063

    IL_0046:  ldloc.3
    IL_0047:  ldloc.s    V_5
    IL_0049:  conv.i
    IL_004a:  ldloc.s    V_6
    IL_004c:  stelem.i8
    IL_004d:  ldloc.s    V_6
    IL_004f:  ldc.i4.1
    IL_0050:  conv.i8
    IL_0051:  add
    IL_0052:  stloc.s    V_6
    IL_0054:  ldloc.s    V_5
    IL_0056:  ldc.i4.1
    IL_0057:  conv.i8
    IL_0058:  add
    IL_0059:  stloc.s    V_5
    IL_005b:  ldloc.s    V_5
    IL_005d:  ldc.i4.0
    IL_005e:  conv.i8
    IL_005f:  cgt.un
    IL_0061:  stloc.s    V_4
    IL_0063:  ldloc.s    V_4
    IL_0065:  brtrue.s   IL_0046

    IL_0067:  nop
    IL_0068:  br.s       IL_00a1

    IL_006a:  ldarg.1
    IL_006b:  ldarg.0
    IL_006c:  bge.un.s   IL_0073

    IL_006e:  ldc.i4.0
    IL_006f:  conv.i8
    IL_0070:  nop
    IL_0071:  br.s       IL_007a

    IL_0073:  ldarg.1
    IL_0074:  ldarg.0
    IL_0075:  sub
    IL_0076:  ldc.i4.1
    IL_0077:  conv.i8
    IL_0078:  add.ovf.un
    IL_0079:  nop
    IL_007a:  stloc.s    V_5
    IL_007c:  ldc.i4.0
    IL_007d:  conv.i8
    IL_007e:  stloc.s    V_6
    IL_0080:  ldarg.0
    IL_0081:  stloc.s    V_7
    IL_0083:  br.s       IL_009a

    IL_0085:  ldloc.3
    IL_0086:  ldloc.s    V_6
    IL_0088:  conv.i
    IL_0089:  ldloc.s    V_7
    IL_008b:  stelem.i8
    IL_008c:  ldloc.s    V_7
    IL_008e:  ldc.i4.1
    IL_008f:  conv.i8
    IL_0090:  add
    IL_0091:  stloc.s    V_7
    IL_0093:  ldloc.s    V_6
    IL_0095:  ldc.i4.1
    IL_0096:  conv.i8
    IL_0097:  add
    IL_0098:  stloc.s    V_6
    IL_009a:  ldloc.s    V_6
    IL_009c:  ldloc.s    V_5
    IL_009e:  blt.un.s   IL_0085

    IL_00a0:  nop
    IL_00a1:  ldloc.3
    IL_00a2:  ret
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
    IL_0019:  brtrue.s   IL_0021

    IL_001b:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_0020:  ret

    IL_0021:  ldloc.1
    IL_0022:  conv.ovf.i.un
    IL_0023:  newarr     [runtime]System.UInt64
    IL_0028:  stloc.2
    IL_0029:  ldc.i4.0
    IL_002a:  conv.i8
    IL_002b:  stloc.3
    IL_002c:  ldarg.0
    IL_002d:  stloc.s    V_4
    IL_002f:  br.s       IL_0043

    IL_0031:  ldloc.2
    IL_0032:  ldloc.3
    IL_0033:  conv.i
    IL_0034:  ldloc.s    V_4
    IL_0036:  stelem.i8
    IL_0037:  ldloc.s    V_4
    IL_0039:  ldc.i4.1
    IL_003a:  conv.i8
    IL_003b:  add
    IL_003c:  stloc.s    V_4
    IL_003e:  ldloc.3
    IL_003f:  ldc.i4.1
    IL_0040:  conv.i8
    IL_0041:  add
    IL_0042:  stloc.3
    IL_0043:  ldloc.3
    IL_0044:  ldloc.0
    IL_0045:  blt.un.s   IL_0031

    IL_0047:  ldloc.2
    IL_0048:  ret
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
    IL_0030:  brtrue.s   IL_0038

    IL_0032:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_0037:  ret

    IL_0038:  ldloc.1
    IL_0039:  conv.ovf.i.un
    IL_003a:  newarr     [runtime]System.UInt64
    IL_003f:  stloc.2
    IL_0040:  ldc.i4.0
    IL_0041:  conv.i8
    IL_0042:  stloc.3
    IL_0043:  ldc.i4.1
    IL_0044:  conv.i8
    IL_0045:  stloc.s    V_4
    IL_0047:  br.s       IL_005a

    IL_0049:  ldloc.2
    IL_004a:  ldloc.3
    IL_004b:  conv.i
    IL_004c:  ldloc.s    V_4
    IL_004e:  stelem.i8
    IL_004f:  ldloc.s    V_4
    IL_0051:  ldarg.0
    IL_0052:  add
    IL_0053:  stloc.s    V_4
    IL_0055:  ldloc.3
    IL_0056:  ldc.i4.1
    IL_0057:  conv.i8
    IL_0058:  add
    IL_0059:  stloc.3
    IL_005a:  ldloc.3
    IL_005b:  ldloc.0
    IL_005c:  blt.un.s   IL_0049

    IL_005e:  ldloc.2
    IL_005f:  ret
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
    IL_0017:  brtrue.s   IL_001f

    IL_0019:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_001e:  ret

    IL_001f:  ldloc.1
    IL_0020:  conv.ovf.i.un
    IL_0021:  newarr     [runtime]System.UInt64
    IL_0026:  stloc.2
    IL_0027:  ldc.i4.0
    IL_0028:  conv.i8
    IL_0029:  stloc.3
    IL_002a:  ldc.i4.1
    IL_002b:  conv.i8
    IL_002c:  stloc.s    V_4
    IL_002e:  br.s       IL_0042

    IL_0030:  ldloc.2
    IL_0031:  ldloc.3
    IL_0032:  conv.i
    IL_0033:  ldloc.s    V_4
    IL_0035:  stelem.i8
    IL_0036:  ldloc.s    V_4
    IL_0038:  ldc.i4.1
    IL_0039:  conv.i8
    IL_003a:  add
    IL_003b:  stloc.s    V_4
    IL_003d:  ldloc.3
    IL_003e:  ldc.i4.1
    IL_003f:  conv.i8
    IL_0040:  add
    IL_0041:  stloc.3
    IL_0042:  ldloc.3
    IL_0043:  ldloc.0
    IL_0044:  blt.un.s   IL_0030

    IL_0046:  ldloc.2
    IL_0047:  ret
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
    IL_002d:  brtrue.s   IL_0035

    IL_002f:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_0034:  ret

    IL_0035:  ldloc.1
    IL_0036:  conv.ovf.i.un
    IL_0037:  newarr     [runtime]System.UInt64
    IL_003c:  stloc.2
    IL_003d:  ldc.i4.0
    IL_003e:  conv.i8
    IL_003f:  stloc.3
    IL_0040:  ldarg.0
    IL_0041:  stloc.s    V_4
    IL_0043:  br.s       IL_0056

    IL_0045:  ldloc.2
    IL_0046:  ldloc.3
    IL_0047:  conv.i
    IL_0048:  ldloc.s    V_4
    IL_004a:  stelem.i8
    IL_004b:  ldloc.s    V_4
    IL_004d:  ldarg.1
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
    IL_0027:  brtrue.s   IL_002f

    IL_0029:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_002e:  ret

    IL_002f:  ldloc.2
    IL_0030:  conv.ovf.i.un
    IL_0031:  newarr     [runtime]System.UInt64
    IL_0036:  stloc.3
    IL_0037:  ldloc.1
    IL_0038:  brfalse.s  IL_006a

    IL_003a:  ldc.i4.1
    IL_003b:  stloc.s    V_4
    IL_003d:  ldc.i4.0
    IL_003e:  conv.i8
    IL_003f:  stloc.s    V_5
    IL_0041:  ldarg.0
    IL_0042:  stloc.s    V_6
    IL_0044:  br.s       IL_0063

    IL_0046:  ldloc.3
    IL_0047:  ldloc.s    V_5
    IL_0049:  conv.i
    IL_004a:  ldloc.s    V_6
    IL_004c:  stelem.i8
    IL_004d:  ldloc.s    V_6
    IL_004f:  ldc.i4.1
    IL_0050:  conv.i8
    IL_0051:  add
    IL_0052:  stloc.s    V_6
    IL_0054:  ldloc.s    V_5
    IL_0056:  ldc.i4.1
    IL_0057:  conv.i8
    IL_0058:  add
    IL_0059:  stloc.s    V_5
    IL_005b:  ldloc.s    V_5
    IL_005d:  ldc.i4.0
    IL_005e:  conv.i8
    IL_005f:  cgt.un
    IL_0061:  stloc.s    V_4
    IL_0063:  ldloc.s    V_4
    IL_0065:  brtrue.s   IL_0046

    IL_0067:  nop
    IL_0068:  br.s       IL_00a1

    IL_006a:  ldarg.1
    IL_006b:  ldarg.0
    IL_006c:  bge.un.s   IL_0073

    IL_006e:  ldc.i4.0
    IL_006f:  conv.i8
    IL_0070:  nop
    IL_0071:  br.s       IL_007a

    IL_0073:  ldarg.1
    IL_0074:  ldarg.0
    IL_0075:  sub
    IL_0076:  ldc.i4.1
    IL_0077:  conv.i8
    IL_0078:  add.ovf.un
    IL_0079:  nop
    IL_007a:  stloc.s    V_5
    IL_007c:  ldc.i4.0
    IL_007d:  conv.i8
    IL_007e:  stloc.s    V_6
    IL_0080:  ldarg.0
    IL_0081:  stloc.s    V_7
    IL_0083:  br.s       IL_009a

    IL_0085:  ldloc.3
    IL_0086:  ldloc.s    V_6
    IL_0088:  conv.i
    IL_0089:  ldloc.s    V_7
    IL_008b:  stelem.i8
    IL_008c:  ldloc.s    V_7
    IL_008e:  ldc.i4.1
    IL_008f:  conv.i8
    IL_0090:  add
    IL_0091:  stloc.s    V_7
    IL_0093:  ldloc.s    V_6
    IL_0095:  ldc.i4.1
    IL_0096:  conv.i8
    IL_0097:  add
    IL_0098:  stloc.s    V_6
    IL_009a:  ldloc.s    V_6
    IL_009c:  ldloc.s    V_5
    IL_009e:  blt.un.s   IL_0085

    IL_00a0:  nop
    IL_00a1:  ldloc.3
    IL_00a2:  ret
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
    IL_002a:  brtrue.s   IL_0032

    IL_002c:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_0031:  ret

    IL_0032:  ldloc.1
    IL_0033:  conv.ovf.i.un
    IL_0034:  newarr     [runtime]System.UInt64
    IL_0039:  stloc.2
    IL_003a:  ldc.i4.0
    IL_003b:  conv.i8
    IL_003c:  stloc.3
    IL_003d:  ldc.i4.1
    IL_003e:  conv.i8
    IL_003f:  stloc.s    V_4
    IL_0041:  br.s       IL_0054

    IL_0043:  ldloc.2
    IL_0044:  ldloc.3
    IL_0045:  conv.i
    IL_0046:  ldloc.s    V_4
    IL_0048:  stelem.i8
    IL_0049:  ldloc.s    V_4
    IL_004b:  ldarg.0
    IL_004c:  add
    IL_004d:  stloc.s    V_4
    IL_004f:  ldloc.3
    IL_0050:  ldc.i4.1
    IL_0051:  conv.i8
    IL_0052:  add
    IL_0053:  stloc.3
    IL_0054:  ldloc.3
    IL_0055:  ldloc.0
    IL_0056:  blt.un.s   IL_0043

    IL_0058:  ldloc.2
    IL_0059:  ret
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
    IL_003b:  brtrue.s   IL_0043

    IL_003d:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_0042:  ret

    IL_0043:  ldloc.2
    IL_0044:  conv.ovf.i.un
    IL_0045:  newarr     [runtime]System.UInt64
    IL_004a:  stloc.3
    IL_004b:  ldloc.1
    IL_004c:  brfalse.s  IL_007d

    IL_004e:  ldc.i4.1
    IL_004f:  stloc.s    V_4
    IL_0051:  ldc.i4.0
    IL_0052:  conv.i8
    IL_0053:  stloc.s    V_5
    IL_0055:  ldarg.0
    IL_0056:  stloc.s    V_6
    IL_0058:  br.s       IL_0076

    IL_005a:  ldloc.3
    IL_005b:  ldloc.s    V_5
    IL_005d:  conv.i
    IL_005e:  ldloc.s    V_6
    IL_0060:  stelem.i8
    IL_0061:  ldloc.s    V_6
    IL_0063:  ldarg.1
    IL_0064:  add
    IL_0065:  stloc.s    V_6
    IL_0067:  ldloc.s    V_5
    IL_0069:  ldc.i4.1
    IL_006a:  conv.i8
    IL_006b:  add
    IL_006c:  stloc.s    V_5
    IL_006e:  ldloc.s    V_5
    IL_0070:  ldc.i4.0
    IL_0071:  conv.i8
    IL_0072:  cgt.un
    IL_0074:  stloc.s    V_4
    IL_0076:  ldloc.s    V_4
    IL_0078:  brtrue.s   IL_005a

    IL_007a:  nop
    IL_007b:  br.s       IL_00b5

    IL_007d:  ldarg.2
    IL_007e:  ldarg.0
    IL_007f:  bge.un.s   IL_0086

    IL_0081:  ldc.i4.0
    IL_0082:  conv.i8
    IL_0083:  nop
    IL_0084:  br.s       IL_008f

    IL_0086:  ldarg.2
    IL_0087:  ldarg.0
    IL_0088:  sub
    IL_0089:  ldarg.1
    IL_008a:  div.un
    IL_008b:  ldc.i4.1
    IL_008c:  conv.i8
    IL_008d:  add.ovf.un
    IL_008e:  nop
    IL_008f:  stloc.s    V_5
    IL_0091:  ldc.i4.0
    IL_0092:  conv.i8
    IL_0093:  stloc.s    V_6
    IL_0095:  ldarg.0
    IL_0096:  stloc.s    V_7
    IL_0098:  br.s       IL_00ae

    IL_009a:  ldloc.3
    IL_009b:  ldloc.s    V_6
    IL_009d:  conv.i
    IL_009e:  ldloc.s    V_7
    IL_00a0:  stelem.i8
    IL_00a1:  ldloc.s    V_7
    IL_00a3:  ldarg.1
    IL_00a4:  add
    IL_00a5:  stloc.s    V_7
    IL_00a7:  ldloc.s    V_6
    IL_00a9:  ldc.i4.1
    IL_00aa:  conv.i8
    IL_00ab:  add
    IL_00ac:  stloc.s    V_6
    IL_00ae:  ldloc.s    V_6
    IL_00b0:  ldloc.s    V_5
    IL_00b2:  blt.un.s   IL_009a

    IL_00b4:  nop
    IL_00b5:  ldloc.3
    IL_00b6:  ret
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
    IL_0020:  brtrue.s   IL_0028

    IL_0022:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_0027:  ret

    IL_0028:  ldloc.2
    IL_0029:  conv.ovf.i.un
    IL_002a:  newarr     [runtime]System.UInt64
    IL_002f:  stloc.3
    IL_0030:  ldc.i4.0
    IL_0031:  conv.i8
    IL_0032:  stloc.s    V_4
    IL_0034:  ldloc.0
    IL_0035:  stloc.s    V_5
    IL_0037:  br.s       IL_004e

    IL_0039:  ldloc.3
    IL_003a:  ldloc.s    V_4
    IL_003c:  conv.i
    IL_003d:  ldloc.s    V_5
    IL_003f:  stelem.i8
    IL_0040:  ldloc.s    V_5
    IL_0042:  ldc.i4.1
    IL_0043:  conv.i8
    IL_0044:  add
    IL_0045:  stloc.s    V_5
    IL_0047:  ldloc.s    V_4
    IL_0049:  ldc.i4.1
    IL_004a:  conv.i8
    IL_004b:  add
    IL_004c:  stloc.s    V_4
    IL_004e:  ldloc.s    V_4
    IL_0050:  ldloc.1
    IL_0051:  blt.un.s   IL_0039

    IL_0053:  ldloc.3
    IL_0054:  ret
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
    IL_001e:  brtrue.s   IL_0026

    IL_0020:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_0025:  ret

    IL_0026:  ldloc.2
    IL_0027:  conv.ovf.i.un
    IL_0028:  newarr     [runtime]System.UInt64
    IL_002d:  stloc.3
    IL_002e:  ldc.i4.0
    IL_002f:  conv.i8
    IL_0030:  stloc.s    V_4
    IL_0032:  ldc.i4.1
    IL_0033:  conv.i8
    IL_0034:  stloc.s    V_5
    IL_0036:  br.s       IL_004d

    IL_0038:  ldloc.3
    IL_0039:  ldloc.s    V_4
    IL_003b:  conv.i
    IL_003c:  ldloc.s    V_5
    IL_003e:  stelem.i8
    IL_003f:  ldloc.s    V_5
    IL_0041:  ldc.i4.1
    IL_0042:  conv.i8
    IL_0043:  add
    IL_0044:  stloc.s    V_5
    IL_0046:  ldloc.s    V_4
    IL_0048:  ldc.i4.1
    IL_0049:  conv.i8
    IL_004a:  add
    IL_004b:  stloc.s    V_4
    IL_004d:  ldloc.s    V_4
    IL_004f:  ldloc.1
    IL_0050:  blt.un.s   IL_0038

    IL_0052:  ldloc.3
    IL_0053:  ret
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
    IL_0038:  brtrue.s   IL_0040

    IL_003a:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_003f:  ret

    IL_0040:  ldloc.s    V_4
    IL_0042:  conv.ovf.i.un
    IL_0043:  newarr     [runtime]System.UInt64
    IL_0048:  stloc.s    V_5
    IL_004a:  ldloc.3
    IL_004b:  brfalse.s  IL_007e

    IL_004d:  ldc.i4.1
    IL_004e:  stloc.s    V_6
    IL_0050:  ldc.i4.0
    IL_0051:  conv.i8
    IL_0052:  stloc.s    V_7
    IL_0054:  ldloc.0
    IL_0055:  stloc.s    V_8
    IL_0057:  br.s       IL_0077

    IL_0059:  ldloc.s    V_5
    IL_005b:  ldloc.s    V_7
    IL_005d:  conv.i
    IL_005e:  ldloc.s    V_8
    IL_0060:  stelem.i8
    IL_0061:  ldloc.s    V_8
    IL_0063:  ldc.i4.1
    IL_0064:  conv.i8
    IL_0065:  add
    IL_0066:  stloc.s    V_8
    IL_0068:  ldloc.s    V_7
    IL_006a:  ldc.i4.1
    IL_006b:  conv.i8
    IL_006c:  add
    IL_006d:  stloc.s    V_7
    IL_006f:  ldloc.s    V_7
    IL_0071:  ldc.i4.0
    IL_0072:  conv.i8
    IL_0073:  cgt.un
    IL_0075:  stloc.s    V_6
    IL_0077:  ldloc.s    V_6
    IL_0079:  brtrue.s   IL_0059

    IL_007b:  nop
    IL_007c:  br.s       IL_00b6

    IL_007e:  ldloc.1
    IL_007f:  ldloc.0
    IL_0080:  bge.un.s   IL_0087

    IL_0082:  ldc.i4.0
    IL_0083:  conv.i8
    IL_0084:  nop
    IL_0085:  br.s       IL_008e

    IL_0087:  ldloc.1
    IL_0088:  ldloc.0
    IL_0089:  sub
    IL_008a:  ldc.i4.1
    IL_008b:  conv.i8
    IL_008c:  add.ovf.un
    IL_008d:  nop
    IL_008e:  stloc.s    V_7
    IL_0090:  ldc.i4.0
    IL_0091:  conv.i8
    IL_0092:  stloc.s    V_8
    IL_0094:  ldloc.0
    IL_0095:  stloc.s    V_9
    IL_0097:  br.s       IL_00af

    IL_0099:  ldloc.s    V_5
    IL_009b:  ldloc.s    V_8
    IL_009d:  conv.i
    IL_009e:  ldloc.s    V_9
    IL_00a0:  stelem.i8
    IL_00a1:  ldloc.s    V_9
    IL_00a3:  ldc.i4.1
    IL_00a4:  conv.i8
    IL_00a5:  add
    IL_00a6:  stloc.s    V_9
    IL_00a8:  ldloc.s    V_8
    IL_00aa:  ldc.i4.1
    IL_00ab:  conv.i8
    IL_00ac:  add
    IL_00ad:  stloc.s    V_8
    IL_00af:  ldloc.s    V_8
    IL_00b1:  ldloc.s    V_7
    IL_00b3:  blt.un.s   IL_0099

    IL_00b5:  nop
    IL_00b6:  ldloc.s    V_5
    IL_00b8:  ret
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
    IL_0020:  brtrue.s   IL_0028

    IL_0022:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_0027:  ret

    IL_0028:  ldloc.2
    IL_0029:  conv.ovf.i.un
    IL_002a:  newarr     [runtime]System.UInt64
    IL_002f:  stloc.3
    IL_0030:  ldc.i4.0
    IL_0031:  conv.i8
    IL_0032:  stloc.s    V_4
    IL_0034:  ldloc.0
    IL_0035:  stloc.s    V_5
    IL_0037:  br.s       IL_004e

    IL_0039:  ldloc.3
    IL_003a:  ldloc.s    V_4
    IL_003c:  conv.i
    IL_003d:  ldloc.s    V_5
    IL_003f:  stelem.i8
    IL_0040:  ldloc.s    V_5
    IL_0042:  ldc.i4.1
    IL_0043:  conv.i8
    IL_0044:  add
    IL_0045:  stloc.s    V_5
    IL_0047:  ldloc.s    V_4
    IL_0049:  ldc.i4.1
    IL_004a:  conv.i8
    IL_004b:  add
    IL_004c:  stloc.s    V_4
    IL_004e:  ldloc.s    V_4
    IL_0050:  ldloc.1
    IL_0051:  blt.un.s   IL_0039

    IL_0053:  ldloc.3
    IL_0054:  ret
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
    IL_0037:  brtrue.s   IL_003f

    IL_0039:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_003e:  ret

    IL_003f:  ldloc.2
    IL_0040:  conv.ovf.i.un
    IL_0041:  newarr     [runtime]System.UInt64
    IL_0046:  stloc.3
    IL_0047:  ldc.i4.0
    IL_0048:  conv.i8
    IL_0049:  stloc.s    V_4
    IL_004b:  ldc.i4.1
    IL_004c:  conv.i8
    IL_004d:  stloc.s    V_5
    IL_004f:  br.s       IL_0065

    IL_0051:  ldloc.3
    IL_0052:  ldloc.s    V_4
    IL_0054:  conv.i
    IL_0055:  ldloc.s    V_5
    IL_0057:  stelem.i8
    IL_0058:  ldloc.s    V_5
    IL_005a:  ldloc.0
    IL_005b:  add
    IL_005c:  stloc.s    V_5
    IL_005e:  ldloc.s    V_4
    IL_0060:  ldc.i4.1
    IL_0061:  conv.i8
    IL_0062:  add
    IL_0063:  stloc.s    V_4
    IL_0065:  ldloc.s    V_4
    IL_0067:  ldloc.1
    IL_0068:  blt.un.s   IL_0051

    IL_006a:  ldloc.3
    IL_006b:  ret
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
    IL_001e:  brtrue.s   IL_0026

    IL_0020:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_0025:  ret

    IL_0026:  ldloc.2
    IL_0027:  conv.ovf.i.un
    IL_0028:  newarr     [runtime]System.UInt64
    IL_002d:  stloc.3
    IL_002e:  ldc.i4.0
    IL_002f:  conv.i8
    IL_0030:  stloc.s    V_4
    IL_0032:  ldc.i4.1
    IL_0033:  conv.i8
    IL_0034:  stloc.s    V_5
    IL_0036:  br.s       IL_004d

    IL_0038:  ldloc.3
    IL_0039:  ldloc.s    V_4
    IL_003b:  conv.i
    IL_003c:  ldloc.s    V_5
    IL_003e:  stelem.i8
    IL_003f:  ldloc.s    V_5
    IL_0041:  ldc.i4.1
    IL_0042:  conv.i8
    IL_0043:  add
    IL_0044:  stloc.s    V_5
    IL_0046:  ldloc.s    V_4
    IL_0048:  ldc.i4.1
    IL_0049:  conv.i8
    IL_004a:  add
    IL_004b:  stloc.s    V_4
    IL_004d:  ldloc.s    V_4
    IL_004f:  ldloc.1
    IL_0050:  blt.un.s   IL_0038

    IL_0052:  ldloc.3
    IL_0053:  ret
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
    IL_0055:  brtrue.s   IL_005d

    IL_0057:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_005c:  ret

    IL_005d:  ldloc.s    V_5
    IL_005f:  conv.ovf.i.un
    IL_0060:  newarr     [runtime]System.UInt64
    IL_0065:  stloc.s    V_6
    IL_0067:  ldloc.s    V_4
    IL_0069:  brfalse.s  IL_009b

    IL_006b:  ldc.i4.1
    IL_006c:  stloc.s    V_7
    IL_006e:  ldc.i4.0
    IL_006f:  conv.i8
    IL_0070:  stloc.s    V_8
    IL_0072:  ldloc.0
    IL_0073:  stloc.s    V_9
    IL_0075:  br.s       IL_0094

    IL_0077:  ldloc.s    V_6
    IL_0079:  ldloc.s    V_8
    IL_007b:  conv.i
    IL_007c:  ldloc.s    V_9
    IL_007e:  stelem.i8
    IL_007f:  ldloc.s    V_9
    IL_0081:  ldloc.1
    IL_0082:  add
    IL_0083:  stloc.s    V_9
    IL_0085:  ldloc.s    V_8
    IL_0087:  ldc.i4.1
    IL_0088:  conv.i8
    IL_0089:  add
    IL_008a:  stloc.s    V_8
    IL_008c:  ldloc.s    V_8
    IL_008e:  ldc.i4.0
    IL_008f:  conv.i8
    IL_0090:  cgt.un
    IL_0092:  stloc.s    V_7
    IL_0094:  ldloc.s    V_7
    IL_0096:  brtrue.s   IL_0077

    IL_0098:  nop
    IL_0099:  br.s       IL_00d4

    IL_009b:  ldloc.2
    IL_009c:  ldloc.0
    IL_009d:  bge.un.s   IL_00a4

    IL_009f:  ldc.i4.0
    IL_00a0:  conv.i8
    IL_00a1:  nop
    IL_00a2:  br.s       IL_00ad

    IL_00a4:  ldloc.2
    IL_00a5:  ldloc.0
    IL_00a6:  sub
    IL_00a7:  ldloc.1
    IL_00a8:  div.un
    IL_00a9:  ldc.i4.1
    IL_00aa:  conv.i8
    IL_00ab:  add.ovf.un
    IL_00ac:  nop
    IL_00ad:  stloc.s    V_8
    IL_00af:  ldc.i4.0
    IL_00b0:  conv.i8
    IL_00b1:  stloc.s    V_9
    IL_00b3:  ldloc.0
    IL_00b4:  stloc.s    V_10
    IL_00b6:  br.s       IL_00cd

    IL_00b8:  ldloc.s    V_6
    IL_00ba:  ldloc.s    V_9
    IL_00bc:  conv.i
    IL_00bd:  ldloc.s    V_10
    IL_00bf:  stelem.i8
    IL_00c0:  ldloc.s    V_10
    IL_00c2:  ldloc.1
    IL_00c3:  add
    IL_00c4:  stloc.s    V_10
    IL_00c6:  ldloc.s    V_9
    IL_00c8:  ldc.i4.1
    IL_00c9:  conv.i8
    IL_00ca:  add
    IL_00cb:  stloc.s    V_9
    IL_00cd:  ldloc.s    V_9
    IL_00cf:  ldloc.s    V_8
    IL_00d1:  blt.un.s   IL_00b8

    IL_00d3:  nop
    IL_00d4:  ldloc.s    V_6
    IL_00d6:  ret
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






