




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
             uint64[] V_1,
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
    IL_0016:  ldloc.0
    IL_0017:  ldc.i4.1
    IL_0018:  conv.i8
    IL_0019:  bge.un.s   IL_0021

    IL_001b:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_0020:  ret

    IL_0021:  ldloc.0
    IL_0022:  conv.ovf.i.un
    IL_0023:  newarr     [runtime]System.UInt64
    IL_0028:  stloc.1
    IL_0029:  ldc.i4.0
    IL_002a:  conv.i8
    IL_002b:  stloc.2
    IL_002c:  ldarg.0
    IL_002d:  stloc.3
    IL_002e:  br.s       IL_003f

    IL_0030:  ldloc.1
    IL_0031:  ldloc.2
    IL_0032:  conv.i
    IL_0033:  ldloc.3
    IL_0034:  stelem.i8
    IL_0035:  ldloc.3
    IL_0036:  ldc.i4.1
    IL_0037:  conv.i8
    IL_0038:  add
    IL_0039:  stloc.3
    IL_003a:  ldloc.2
    IL_003b:  ldc.i4.1
    IL_003c:  conv.i8
    IL_003d:  add
    IL_003e:  stloc.2
    IL_003f:  ldloc.2
    IL_0040:  ldloc.0
    IL_0041:  blt.un.s   IL_0030

    IL_0043:  ldloc.1
    IL_0044:  ret
  } 

  .method public static uint64[]  f7(uint64 finish) cil managed
  {
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64[] V_1,
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
    IL_0014:  ldloc.0
    IL_0015:  ldc.i4.1
    IL_0016:  conv.i8
    IL_0017:  bge.un.s   IL_001f

    IL_0019:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_001e:  ret

    IL_001f:  ldloc.0
    IL_0020:  conv.ovf.i.un
    IL_0021:  newarr     [runtime]System.UInt64
    IL_0026:  stloc.1
    IL_0027:  ldc.i4.0
    IL_0028:  conv.i8
    IL_0029:  stloc.2
    IL_002a:  ldc.i4.1
    IL_002b:  conv.i8
    IL_002c:  stloc.3
    IL_002d:  br.s       IL_003e

    IL_002f:  ldloc.1
    IL_0030:  ldloc.2
    IL_0031:  conv.i
    IL_0032:  ldloc.3
    IL_0033:  stelem.i8
    IL_0034:  ldloc.3
    IL_0035:  ldc.i4.1
    IL_0036:  conv.i8
    IL_0037:  add
    IL_0038:  stloc.3
    IL_0039:  ldloc.2
    IL_003a:  ldc.i4.1
    IL_003b:  conv.i8
    IL_003c:  add
    IL_003d:  stloc.2
    IL_003e:  ldloc.2
    IL_003f:  ldloc.0
    IL_0040:  blt.un.s   IL_002f

    IL_0042:  ldloc.1
    IL_0043:  ret
  } 

  .method public static uint64[]  f8(uint64 start,
                                     uint64 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (uint64 V_0,
             bool V_1,
             uint64[] V_2,
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
    IL_0015:  ldloc.0
    IL_0016:  ldc.i4.1
    IL_0017:  conv.i8
    IL_0018:  add.ovf.un
    IL_0019:  ldc.i4.1
    IL_001a:  conv.i8
    IL_001b:  bge.un.s   IL_0023

    IL_001d:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_0022:  ret

    IL_0023:  ldloc.0
    IL_0024:  ldc.i4.1
    IL_0025:  conv.i8
    IL_0026:  add.ovf.un
    IL_0027:  conv.ovf.i.un
    IL_0028:  newarr     [runtime]System.UInt64
    IL_002d:  stloc.2
    IL_002e:  ldloc.1
    IL_002f:  brfalse.s  IL_0065

    IL_0031:  ldc.i4.0
    IL_0032:  conv.i8
    IL_0033:  stloc.3
    IL_0034:  ldarg.0
    IL_0035:  stloc.s    V_4
    IL_0037:  ldloc.2
    IL_0038:  ldloc.3
    IL_0039:  conv.i
    IL_003a:  ldloc.s    V_4
    IL_003c:  stelem.i8
    IL_003d:  ldloc.s    V_4
    IL_003f:  ldc.i4.1
    IL_0040:  conv.i8
    IL_0041:  add
    IL_0042:  stloc.s    V_4
    IL_0044:  ldloc.3
    IL_0045:  ldc.i4.1
    IL_0046:  conv.i8
    IL_0047:  add
    IL_0048:  stloc.3
    IL_0049:  br.s       IL_005d

    IL_004b:  ldloc.2
    IL_004c:  ldloc.3
    IL_004d:  conv.i
    IL_004e:  ldloc.s    V_4
    IL_0050:  stelem.i8
    IL_0051:  ldloc.s    V_4
    IL_0053:  ldc.i4.1
    IL_0054:  conv.i8
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
    IL_0060:  bgt.un.s   IL_004b

    IL_0062:  nop
    IL_0063:  br.s       IL_008e

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
    IL_0071:  br.s       IL_0088

    IL_0073:  ldloc.2
    IL_0074:  ldloc.s    V_4
    IL_0076:  conv.i
    IL_0077:  ldloc.s    V_5
    IL_0079:  stelem.i8
    IL_007a:  ldloc.s    V_5
    IL_007c:  ldc.i4.1
    IL_007d:  conv.i8
    IL_007e:  add
    IL_007f:  stloc.s    V_5
    IL_0081:  ldloc.s    V_4
    IL_0083:  ldc.i4.1
    IL_0084:  conv.i8
    IL_0085:  add
    IL_0086:  stloc.s    V_4
    IL_0088:  ldloc.s    V_4
    IL_008a:  ldloc.3
    IL_008b:  blt.un.s   IL_0073

    IL_008d:  nop
    IL_008e:  ldloc.2
    IL_008f:  ret
  } 

  .method public static uint64[]  f9(uint64 start) cil managed
  {
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64[] V_1,
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
    IL_0016:  ldloc.0
    IL_0017:  ldc.i4.1
    IL_0018:  conv.i8
    IL_0019:  bge.un.s   IL_0021

    IL_001b:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_0020:  ret

    IL_0021:  ldloc.0
    IL_0022:  conv.ovf.i.un
    IL_0023:  newarr     [runtime]System.UInt64
    IL_0028:  stloc.1
    IL_0029:  ldc.i4.0
    IL_002a:  conv.i8
    IL_002b:  stloc.2
    IL_002c:  ldarg.0
    IL_002d:  stloc.3
    IL_002e:  br.s       IL_003f

    IL_0030:  ldloc.1
    IL_0031:  ldloc.2
    IL_0032:  conv.i
    IL_0033:  ldloc.3
    IL_0034:  stelem.i8
    IL_0035:  ldloc.3
    IL_0036:  ldc.i4.1
    IL_0037:  conv.i8
    IL_0038:  add
    IL_0039:  stloc.3
    IL_003a:  ldloc.2
    IL_003b:  ldc.i4.1
    IL_003c:  conv.i8
    IL_003d:  add
    IL_003e:  stloc.2
    IL_003f:  ldloc.2
    IL_0040:  ldloc.0
    IL_0041:  blt.un.s   IL_0030

    IL_0043:  ldloc.1
    IL_0044:  ret
  } 

  .method public static uint64[]  f10(uint64 step) cil managed
  {
    
    .maxstack  5
    .locals init (uint64 V_0,
             bool V_1,
             uint64[] V_2,
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
    IL_0031:  ldloc.0
    IL_0032:  ldc.i4.1
    IL_0033:  conv.i8
    IL_0034:  add.ovf.un
    IL_0035:  ldc.i4.1
    IL_0036:  conv.i8
    IL_0037:  bge.un.s   IL_003f

    IL_0039:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_003e:  ret

    IL_003f:  ldloc.0
    IL_0040:  ldc.i4.1
    IL_0041:  conv.i8
    IL_0042:  add.ovf.un
    IL_0043:  conv.ovf.i.un
    IL_0044:  newarr     [runtime]System.UInt64
    IL_0049:  stloc.2
    IL_004a:  ldloc.1
    IL_004b:  brfalse.s  IL_0080

    IL_004d:  ldc.i4.0
    IL_004e:  conv.i8
    IL_004f:  stloc.3
    IL_0050:  ldc.i4.1
    IL_0051:  conv.i8
    IL_0052:  stloc.s    V_4
    IL_0054:  ldloc.2
    IL_0055:  ldloc.3
    IL_0056:  conv.i
    IL_0057:  ldloc.s    V_4
    IL_0059:  stelem.i8
    IL_005a:  ldloc.s    V_4
    IL_005c:  ldarg.0
    IL_005d:  add
    IL_005e:  stloc.s    V_4
    IL_0060:  ldloc.3
    IL_0061:  ldc.i4.1
    IL_0062:  conv.i8
    IL_0063:  add
    IL_0064:  stloc.3
    IL_0065:  br.s       IL_0078

    IL_0067:  ldloc.2
    IL_0068:  ldloc.3
    IL_0069:  conv.i
    IL_006a:  ldloc.s    V_4
    IL_006c:  stelem.i8
    IL_006d:  ldloc.s    V_4
    IL_006f:  ldarg.0
    IL_0070:  add
    IL_0071:  stloc.s    V_4
    IL_0073:  ldloc.3
    IL_0074:  ldc.i4.1
    IL_0075:  conv.i8
    IL_0076:  add
    IL_0077:  stloc.3
    IL_0078:  ldloc.3
    IL_0079:  ldc.i4.0
    IL_007a:  conv.i8
    IL_007b:  bgt.un.s   IL_0067

    IL_007d:  nop
    IL_007e:  br.s       IL_00a9

    IL_0080:  ldloc.0
    IL_0081:  ldc.i4.1
    IL_0082:  conv.i8
    IL_0083:  add.ovf.un
    IL_0084:  stloc.3
    IL_0085:  ldc.i4.0
    IL_0086:  conv.i8
    IL_0087:  stloc.s    V_4
    IL_0089:  ldc.i4.1
    IL_008a:  conv.i8
    IL_008b:  stloc.s    V_5
    IL_008d:  br.s       IL_00a3

    IL_008f:  ldloc.2
    IL_0090:  ldloc.s    V_4
    IL_0092:  conv.i
    IL_0093:  ldloc.s    V_5
    IL_0095:  stelem.i8
    IL_0096:  ldloc.s    V_5
    IL_0098:  ldarg.0
    IL_0099:  add
    IL_009a:  stloc.s    V_5
    IL_009c:  ldloc.s    V_4
    IL_009e:  ldc.i4.1
    IL_009f:  conv.i8
    IL_00a0:  add
    IL_00a1:  stloc.s    V_4
    IL_00a3:  ldloc.s    V_4
    IL_00a5:  ldloc.3
    IL_00a6:  blt.un.s   IL_008f

    IL_00a8:  nop
    IL_00a9:  ldloc.2
    IL_00aa:  ret
  } 

  .method public static uint64[]  f11(uint64 finish) cil managed
  {
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64[] V_1,
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
    IL_0014:  ldloc.0
    IL_0015:  ldc.i4.1
    IL_0016:  conv.i8
    IL_0017:  bge.un.s   IL_001f

    IL_0019:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_001e:  ret

    IL_001f:  ldloc.0
    IL_0020:  conv.ovf.i.un
    IL_0021:  newarr     [runtime]System.UInt64
    IL_0026:  stloc.1
    IL_0027:  ldc.i4.0
    IL_0028:  conv.i8
    IL_0029:  stloc.2
    IL_002a:  ldc.i4.1
    IL_002b:  conv.i8
    IL_002c:  stloc.3
    IL_002d:  br.s       IL_003e

    IL_002f:  ldloc.1
    IL_0030:  ldloc.2
    IL_0031:  conv.i
    IL_0032:  ldloc.3
    IL_0033:  stelem.i8
    IL_0034:  ldloc.3
    IL_0035:  ldc.i4.1
    IL_0036:  conv.i8
    IL_0037:  add
    IL_0038:  stloc.3
    IL_0039:  ldloc.2
    IL_003a:  ldc.i4.1
    IL_003b:  conv.i8
    IL_003c:  add
    IL_003d:  stloc.2
    IL_003e:  ldloc.2
    IL_003f:  ldloc.0
    IL_0040:  blt.un.s   IL_002f

    IL_0042:  ldloc.1
    IL_0043:  ret
  } 

  .method public static uint64[]  f12(uint64 start,
                                      uint64 step) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (uint64 V_0,
             bool V_1,
             uint64[] V_2,
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
    IL_002e:  ldloc.0
    IL_002f:  ldc.i4.1
    IL_0030:  conv.i8
    IL_0031:  add.ovf.un
    IL_0032:  ldc.i4.1
    IL_0033:  conv.i8
    IL_0034:  bge.un.s   IL_003c

    IL_0036:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_003b:  ret

    IL_003c:  ldloc.0
    IL_003d:  ldc.i4.1
    IL_003e:  conv.i8
    IL_003f:  add.ovf.un
    IL_0040:  conv.ovf.i.un
    IL_0041:  newarr     [runtime]System.UInt64
    IL_0046:  stloc.2
    IL_0047:  ldloc.1
    IL_0048:  brfalse.s  IL_007c

    IL_004a:  ldc.i4.0
    IL_004b:  conv.i8
    IL_004c:  stloc.3
    IL_004d:  ldarg.0
    IL_004e:  stloc.s    V_4
    IL_0050:  ldloc.2
    IL_0051:  ldloc.3
    IL_0052:  conv.i
    IL_0053:  ldloc.s    V_4
    IL_0055:  stelem.i8
    IL_0056:  ldloc.s    V_4
    IL_0058:  ldarg.1
    IL_0059:  add
    IL_005a:  stloc.s    V_4
    IL_005c:  ldloc.3
    IL_005d:  ldc.i4.1
    IL_005e:  conv.i8
    IL_005f:  add
    IL_0060:  stloc.3
    IL_0061:  br.s       IL_0074

    IL_0063:  ldloc.2
    IL_0064:  ldloc.3
    IL_0065:  conv.i
    IL_0066:  ldloc.s    V_4
    IL_0068:  stelem.i8
    IL_0069:  ldloc.s    V_4
    IL_006b:  ldarg.1
    IL_006c:  add
    IL_006d:  stloc.s    V_4
    IL_006f:  ldloc.3
    IL_0070:  ldc.i4.1
    IL_0071:  conv.i8
    IL_0072:  add
    IL_0073:  stloc.3
    IL_0074:  ldloc.3
    IL_0075:  ldc.i4.0
    IL_0076:  conv.i8
    IL_0077:  bgt.un.s   IL_0063

    IL_0079:  nop
    IL_007a:  br.s       IL_00a4

    IL_007c:  ldloc.0
    IL_007d:  ldc.i4.1
    IL_007e:  conv.i8
    IL_007f:  add.ovf.un
    IL_0080:  stloc.3
    IL_0081:  ldc.i4.0
    IL_0082:  conv.i8
    IL_0083:  stloc.s    V_4
    IL_0085:  ldarg.0
    IL_0086:  stloc.s    V_5
    IL_0088:  br.s       IL_009e

    IL_008a:  ldloc.2
    IL_008b:  ldloc.s    V_4
    IL_008d:  conv.i
    IL_008e:  ldloc.s    V_5
    IL_0090:  stelem.i8
    IL_0091:  ldloc.s    V_5
    IL_0093:  ldarg.1
    IL_0094:  add
    IL_0095:  stloc.s    V_5
    IL_0097:  ldloc.s    V_4
    IL_0099:  ldc.i4.1
    IL_009a:  conv.i8
    IL_009b:  add
    IL_009c:  stloc.s    V_4
    IL_009e:  ldloc.s    V_4
    IL_00a0:  ldloc.3
    IL_00a1:  blt.un.s   IL_008a

    IL_00a3:  nop
    IL_00a4:  ldloc.2
    IL_00a5:  ret
  } 

  .method public static uint64[]  f13(uint64 start,
                                      uint64 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (uint64 V_0,
             bool V_1,
             uint64[] V_2,
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
    IL_0015:  ldloc.0
    IL_0016:  ldc.i4.1
    IL_0017:  conv.i8
    IL_0018:  add.ovf.un
    IL_0019:  ldc.i4.1
    IL_001a:  conv.i8
    IL_001b:  bge.un.s   IL_0023

    IL_001d:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_0022:  ret

    IL_0023:  ldloc.0
    IL_0024:  ldc.i4.1
    IL_0025:  conv.i8
    IL_0026:  add.ovf.un
    IL_0027:  conv.ovf.i.un
    IL_0028:  newarr     [runtime]System.UInt64
    IL_002d:  stloc.2
    IL_002e:  ldloc.1
    IL_002f:  brfalse.s  IL_0065

    IL_0031:  ldc.i4.0
    IL_0032:  conv.i8
    IL_0033:  stloc.3
    IL_0034:  ldarg.0
    IL_0035:  stloc.s    V_4
    IL_0037:  ldloc.2
    IL_0038:  ldloc.3
    IL_0039:  conv.i
    IL_003a:  ldloc.s    V_4
    IL_003c:  stelem.i8
    IL_003d:  ldloc.s    V_4
    IL_003f:  ldc.i4.1
    IL_0040:  conv.i8
    IL_0041:  add
    IL_0042:  stloc.s    V_4
    IL_0044:  ldloc.3
    IL_0045:  ldc.i4.1
    IL_0046:  conv.i8
    IL_0047:  add
    IL_0048:  stloc.3
    IL_0049:  br.s       IL_005d

    IL_004b:  ldloc.2
    IL_004c:  ldloc.3
    IL_004d:  conv.i
    IL_004e:  ldloc.s    V_4
    IL_0050:  stelem.i8
    IL_0051:  ldloc.s    V_4
    IL_0053:  ldc.i4.1
    IL_0054:  conv.i8
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
    IL_0060:  bgt.un.s   IL_004b

    IL_0062:  nop
    IL_0063:  br.s       IL_008e

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
    IL_0071:  br.s       IL_0088

    IL_0073:  ldloc.2
    IL_0074:  ldloc.s    V_4
    IL_0076:  conv.i
    IL_0077:  ldloc.s    V_5
    IL_0079:  stelem.i8
    IL_007a:  ldloc.s    V_5
    IL_007c:  ldc.i4.1
    IL_007d:  conv.i8
    IL_007e:  add
    IL_007f:  stloc.s    V_5
    IL_0081:  ldloc.s    V_4
    IL_0083:  ldc.i4.1
    IL_0084:  conv.i8
    IL_0085:  add
    IL_0086:  stloc.s    V_4
    IL_0088:  ldloc.s    V_4
    IL_008a:  ldloc.3
    IL_008b:  blt.un.s   IL_0073

    IL_008d:  nop
    IL_008e:  ldloc.2
    IL_008f:  ret
  } 

  .method public static uint64[]  f14(uint64 step,
                                      uint64 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (uint64 V_0,
             bool V_1,
             uint64[] V_2,
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
    IL_002b:  ldloc.0
    IL_002c:  ldc.i4.1
    IL_002d:  conv.i8
    IL_002e:  add.ovf.un
    IL_002f:  ldc.i4.1
    IL_0030:  conv.i8
    IL_0031:  bge.un.s   IL_0039

    IL_0033:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_0038:  ret

    IL_0039:  ldloc.0
    IL_003a:  ldc.i4.1
    IL_003b:  conv.i8
    IL_003c:  add.ovf.un
    IL_003d:  conv.ovf.i.un
    IL_003e:  newarr     [runtime]System.UInt64
    IL_0043:  stloc.2
    IL_0044:  ldloc.1
    IL_0045:  brfalse.s  IL_007a

    IL_0047:  ldc.i4.0
    IL_0048:  conv.i8
    IL_0049:  stloc.3
    IL_004a:  ldc.i4.1
    IL_004b:  conv.i8
    IL_004c:  stloc.s    V_4
    IL_004e:  ldloc.2
    IL_004f:  ldloc.3
    IL_0050:  conv.i
    IL_0051:  ldloc.s    V_4
    IL_0053:  stelem.i8
    IL_0054:  ldloc.s    V_4
    IL_0056:  ldarg.0
    IL_0057:  add
    IL_0058:  stloc.s    V_4
    IL_005a:  ldloc.3
    IL_005b:  ldc.i4.1
    IL_005c:  conv.i8
    IL_005d:  add
    IL_005e:  stloc.3
    IL_005f:  br.s       IL_0072

    IL_0061:  ldloc.2
    IL_0062:  ldloc.3
    IL_0063:  conv.i
    IL_0064:  ldloc.s    V_4
    IL_0066:  stelem.i8
    IL_0067:  ldloc.s    V_4
    IL_0069:  ldarg.0
    IL_006a:  add
    IL_006b:  stloc.s    V_4
    IL_006d:  ldloc.3
    IL_006e:  ldc.i4.1
    IL_006f:  conv.i8
    IL_0070:  add
    IL_0071:  stloc.3
    IL_0072:  ldloc.3
    IL_0073:  ldc.i4.0
    IL_0074:  conv.i8
    IL_0075:  bgt.un.s   IL_0061

    IL_0077:  nop
    IL_0078:  br.s       IL_00a3

    IL_007a:  ldloc.0
    IL_007b:  ldc.i4.1
    IL_007c:  conv.i8
    IL_007d:  add.ovf.un
    IL_007e:  stloc.3
    IL_007f:  ldc.i4.0
    IL_0080:  conv.i8
    IL_0081:  stloc.s    V_4
    IL_0083:  ldc.i4.1
    IL_0084:  conv.i8
    IL_0085:  stloc.s    V_5
    IL_0087:  br.s       IL_009d

    IL_0089:  ldloc.2
    IL_008a:  ldloc.s    V_4
    IL_008c:  conv.i
    IL_008d:  ldloc.s    V_5
    IL_008f:  stelem.i8
    IL_0090:  ldloc.s    V_5
    IL_0092:  ldarg.0
    IL_0093:  add
    IL_0094:  stloc.s    V_5
    IL_0096:  ldloc.s    V_4
    IL_0098:  ldc.i4.1
    IL_0099:  conv.i8
    IL_009a:  add
    IL_009b:  stloc.s    V_4
    IL_009d:  ldloc.s    V_4
    IL_009f:  ldloc.3
    IL_00a0:  blt.un.s   IL_0089

    IL_00a2:  nop
    IL_00a3:  ldloc.2
    IL_00a4:  ret
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
             uint64[] V_2,
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
    IL_0028:  ldloc.0
    IL_0029:  ldc.i4.1
    IL_002a:  conv.i8
    IL_002b:  add.ovf.un
    IL_002c:  ldc.i4.1
    IL_002d:  conv.i8
    IL_002e:  bge.un.s   IL_0036

    IL_0030:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_0035:  ret

    IL_0036:  ldloc.0
    IL_0037:  ldc.i4.1
    IL_0038:  conv.i8
    IL_0039:  add.ovf.un
    IL_003a:  conv.ovf.i.un
    IL_003b:  newarr     [runtime]System.UInt64
    IL_0040:  stloc.2
    IL_0041:  ldloc.1
    IL_0042:  brfalse.s  IL_0076

    IL_0044:  ldc.i4.0
    IL_0045:  conv.i8
    IL_0046:  stloc.3
    IL_0047:  ldarg.0
    IL_0048:  stloc.s    V_4
    IL_004a:  ldloc.2
    IL_004b:  ldloc.3
    IL_004c:  conv.i
    IL_004d:  ldloc.s    V_4
    IL_004f:  stelem.i8
    IL_0050:  ldloc.s    V_4
    IL_0052:  ldarg.1
    IL_0053:  add
    IL_0054:  stloc.s    V_4
    IL_0056:  ldloc.3
    IL_0057:  ldc.i4.1
    IL_0058:  conv.i8
    IL_0059:  add
    IL_005a:  stloc.3
    IL_005b:  br.s       IL_006e

    IL_005d:  ldloc.2
    IL_005e:  ldloc.3
    IL_005f:  conv.i
    IL_0060:  ldloc.s    V_4
    IL_0062:  stelem.i8
    IL_0063:  ldloc.s    V_4
    IL_0065:  ldarg.1
    IL_0066:  add
    IL_0067:  stloc.s    V_4
    IL_0069:  ldloc.3
    IL_006a:  ldc.i4.1
    IL_006b:  conv.i8
    IL_006c:  add
    IL_006d:  stloc.3
    IL_006e:  ldloc.3
    IL_006f:  ldc.i4.0
    IL_0070:  conv.i8
    IL_0071:  bgt.un.s   IL_005d

    IL_0073:  nop
    IL_0074:  br.s       IL_009e

    IL_0076:  ldloc.0
    IL_0077:  ldc.i4.1
    IL_0078:  conv.i8
    IL_0079:  add.ovf.un
    IL_007a:  stloc.3
    IL_007b:  ldc.i4.0
    IL_007c:  conv.i8
    IL_007d:  stloc.s    V_4
    IL_007f:  ldarg.0
    IL_0080:  stloc.s    V_5
    IL_0082:  br.s       IL_0098

    IL_0084:  ldloc.2
    IL_0085:  ldloc.s    V_4
    IL_0087:  conv.i
    IL_0088:  ldloc.s    V_5
    IL_008a:  stelem.i8
    IL_008b:  ldloc.s    V_5
    IL_008d:  ldarg.1
    IL_008e:  add
    IL_008f:  stloc.s    V_5
    IL_0091:  ldloc.s    V_4
    IL_0093:  ldc.i4.1
    IL_0094:  conv.i8
    IL_0095:  add
    IL_0096:  stloc.s    V_4
    IL_0098:  ldloc.s    V_4
    IL_009a:  ldloc.3
    IL_009b:  blt.un.s   IL_0084

    IL_009d:  nop
    IL_009e:  ldloc.2
    IL_009f:  ret
  } 

  .method public static uint64[]  f16(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,uint64> f) cil managed
  {
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             uint64[] V_2,
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
    IL_001d:  ldloc.1
    IL_001e:  ldc.i4.1
    IL_001f:  conv.i8
    IL_0020:  bge.un.s   IL_0028

    IL_0022:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_0027:  ret

    IL_0028:  ldloc.1
    IL_0029:  conv.ovf.i.un
    IL_002a:  newarr     [runtime]System.UInt64
    IL_002f:  stloc.2
    IL_0030:  ldc.i4.0
    IL_0031:  conv.i8
    IL_0032:  stloc.3
    IL_0033:  ldloc.0
    IL_0034:  stloc.s    V_4
    IL_0036:  br.s       IL_004a

    IL_0038:  ldloc.2
    IL_0039:  ldloc.3
    IL_003a:  conv.i
    IL_003b:  ldloc.s    V_4
    IL_003d:  stelem.i8
    IL_003e:  ldloc.s    V_4
    IL_0040:  ldc.i4.1
    IL_0041:  conv.i8
    IL_0042:  add
    IL_0043:  stloc.s    V_4
    IL_0045:  ldloc.3
    IL_0046:  ldc.i4.1
    IL_0047:  conv.i8
    IL_0048:  add
    IL_0049:  stloc.3
    IL_004a:  ldloc.3
    IL_004b:  ldloc.1
    IL_004c:  blt.un.s   IL_0038

    IL_004e:  ldloc.2
    IL_004f:  ret
  } 

  .method public static uint64[]  f17(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,uint64> f) cil managed
  {
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             uint64[] V_2,
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
    IL_001b:  ldloc.1
    IL_001c:  ldc.i4.1
    IL_001d:  conv.i8
    IL_001e:  bge.un.s   IL_0026

    IL_0020:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_0025:  ret

    IL_0026:  ldloc.1
    IL_0027:  conv.ovf.i.un
    IL_0028:  newarr     [runtime]System.UInt64
    IL_002d:  stloc.2
    IL_002e:  ldc.i4.0
    IL_002f:  conv.i8
    IL_0030:  stloc.3
    IL_0031:  ldc.i4.1
    IL_0032:  conv.i8
    IL_0033:  stloc.s    V_4
    IL_0035:  br.s       IL_0049

    IL_0037:  ldloc.2
    IL_0038:  ldloc.3
    IL_0039:  conv.i
    IL_003a:  ldloc.s    V_4
    IL_003c:  stelem.i8
    IL_003d:  ldloc.s    V_4
    IL_003f:  ldc.i4.1
    IL_0040:  conv.i8
    IL_0041:  add
    IL_0042:  stloc.s    V_4
    IL_0044:  ldloc.3
    IL_0045:  ldc.i4.1
    IL_0046:  conv.i8
    IL_0047:  add
    IL_0048:  stloc.3
    IL_0049:  ldloc.3
    IL_004a:  ldloc.1
    IL_004b:  blt.un.s   IL_0037

    IL_004d:  ldloc.2
    IL_004e:  ret
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
             uint64[] V_4,
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
    IL_0024:  ldloc.2
    IL_0025:  ldc.i4.1
    IL_0026:  conv.i8
    IL_0027:  add.ovf.un
    IL_0028:  ldc.i4.1
    IL_0029:  conv.i8
    IL_002a:  bge.un.s   IL_0032

    IL_002c:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_0031:  ret

    IL_0032:  ldloc.2
    IL_0033:  ldc.i4.1
    IL_0034:  conv.i8
    IL_0035:  add.ovf.un
    IL_0036:  conv.ovf.i.un
    IL_0037:  newarr     [runtime]System.UInt64
    IL_003c:  stloc.s    V_4
    IL_003e:  ldloc.3
    IL_003f:  brfalse.s  IL_007f

    IL_0041:  ldc.i4.0
    IL_0042:  conv.i8
    IL_0043:  stloc.s    V_5
    IL_0045:  ldloc.0
    IL_0046:  stloc.s    V_6
    IL_0048:  ldloc.s    V_4
    IL_004a:  ldloc.s    V_5
    IL_004c:  conv.i
    IL_004d:  ldloc.s    V_6
    IL_004f:  stelem.i8
    IL_0050:  ldloc.s    V_6
    IL_0052:  ldc.i4.1
    IL_0053:  conv.i8
    IL_0054:  add
    IL_0055:  stloc.s    V_6
    IL_0057:  ldloc.s    V_5
    IL_0059:  ldc.i4.1
    IL_005a:  conv.i8
    IL_005b:  add
    IL_005c:  stloc.s    V_5
    IL_005e:  br.s       IL_0076

    IL_0060:  ldloc.s    V_4
    IL_0062:  ldloc.s    V_5
    IL_0064:  conv.i
    IL_0065:  ldloc.s    V_6
    IL_0067:  stelem.i8
    IL_0068:  ldloc.s    V_6
    IL_006a:  ldc.i4.1
    IL_006b:  conv.i8
    IL_006c:  add
    IL_006d:  stloc.s    V_6
    IL_006f:  ldloc.s    V_5
    IL_0071:  ldc.i4.1
    IL_0072:  conv.i8
    IL_0073:  add
    IL_0074:  stloc.s    V_5
    IL_0076:  ldloc.s    V_5
    IL_0078:  ldc.i4.0
    IL_0079:  conv.i8
    IL_007a:  bgt.un.s   IL_0060

    IL_007c:  nop
    IL_007d:  br.s       IL_00ab

    IL_007f:  ldloc.2
    IL_0080:  ldc.i4.1
    IL_0081:  conv.i8
    IL_0082:  add.ovf.un
    IL_0083:  stloc.s    V_5
    IL_0085:  ldc.i4.0
    IL_0086:  conv.i8
    IL_0087:  stloc.s    V_6
    IL_0089:  ldloc.0
    IL_008a:  stloc.s    V_7
    IL_008c:  br.s       IL_00a4

    IL_008e:  ldloc.s    V_4
    IL_0090:  ldloc.s    V_6
    IL_0092:  conv.i
    IL_0093:  ldloc.s    V_7
    IL_0095:  stelem.i8
    IL_0096:  ldloc.s    V_7
    IL_0098:  ldc.i4.1
    IL_0099:  conv.i8
    IL_009a:  add
    IL_009b:  stloc.s    V_7
    IL_009d:  ldloc.s    V_6
    IL_009f:  ldc.i4.1
    IL_00a0:  conv.i8
    IL_00a1:  add
    IL_00a2:  stloc.s    V_6
    IL_00a4:  ldloc.s    V_6
    IL_00a6:  ldloc.s    V_5
    IL_00a8:  blt.un.s   IL_008e

    IL_00aa:  nop
    IL_00ab:  ldloc.s    V_4
    IL_00ad:  ret
  } 

  .method public static uint64[]  f19(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,uint64> f) cil managed
  {
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             uint64[] V_2,
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
    IL_001d:  ldloc.1
    IL_001e:  ldc.i4.1
    IL_001f:  conv.i8
    IL_0020:  bge.un.s   IL_0028

    IL_0022:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_0027:  ret

    IL_0028:  ldloc.1
    IL_0029:  conv.ovf.i.un
    IL_002a:  newarr     [runtime]System.UInt64
    IL_002f:  stloc.2
    IL_0030:  ldc.i4.0
    IL_0031:  conv.i8
    IL_0032:  stloc.3
    IL_0033:  ldloc.0
    IL_0034:  stloc.s    V_4
    IL_0036:  br.s       IL_004a

    IL_0038:  ldloc.2
    IL_0039:  ldloc.3
    IL_003a:  conv.i
    IL_003b:  ldloc.s    V_4
    IL_003d:  stelem.i8
    IL_003e:  ldloc.s    V_4
    IL_0040:  ldc.i4.1
    IL_0041:  conv.i8
    IL_0042:  add
    IL_0043:  stloc.s    V_4
    IL_0045:  ldloc.3
    IL_0046:  ldc.i4.1
    IL_0047:  conv.i8
    IL_0048:  add
    IL_0049:  stloc.3
    IL_004a:  ldloc.3
    IL_004b:  ldloc.1
    IL_004c:  blt.un.s   IL_0038

    IL_004e:  ldloc.2
    IL_004f:  ret
  } 

  .method public static uint64[]  f20(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,uint64> f) cil managed
  {
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             bool V_2,
             uint64[] V_3,
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
    IL_0038:  ldloc.1
    IL_0039:  ldc.i4.1
    IL_003a:  conv.i8
    IL_003b:  add.ovf.un
    IL_003c:  ldc.i4.1
    IL_003d:  conv.i8
    IL_003e:  bge.un.s   IL_0046

    IL_0040:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_0045:  ret

    IL_0046:  ldloc.1
    IL_0047:  ldc.i4.1
    IL_0048:  conv.i8
    IL_0049:  add.ovf.un
    IL_004a:  conv.ovf.i.un
    IL_004b:  newarr     [runtime]System.UInt64
    IL_0050:  stloc.3
    IL_0051:  ldloc.2
    IL_0052:  brfalse.s  IL_008f

    IL_0054:  ldc.i4.0
    IL_0055:  conv.i8
    IL_0056:  stloc.s    V_4
    IL_0058:  ldc.i4.1
    IL_0059:  conv.i8
    IL_005a:  stloc.s    V_5
    IL_005c:  ldloc.3
    IL_005d:  ldloc.s    V_4
    IL_005f:  conv.i
    IL_0060:  ldloc.s    V_5
    IL_0062:  stelem.i8
    IL_0063:  ldloc.s    V_5
    IL_0065:  ldloc.0
    IL_0066:  add
    IL_0067:  stloc.s    V_5
    IL_0069:  ldloc.s    V_4
    IL_006b:  ldc.i4.1
    IL_006c:  conv.i8
    IL_006d:  add
    IL_006e:  stloc.s    V_4
    IL_0070:  br.s       IL_0086

    IL_0072:  ldloc.3
    IL_0073:  ldloc.s    V_4
    IL_0075:  conv.i
    IL_0076:  ldloc.s    V_5
    IL_0078:  stelem.i8
    IL_0079:  ldloc.s    V_5
    IL_007b:  ldloc.0
    IL_007c:  add
    IL_007d:  stloc.s    V_5
    IL_007f:  ldloc.s    V_4
    IL_0081:  ldc.i4.1
    IL_0082:  conv.i8
    IL_0083:  add
    IL_0084:  stloc.s    V_4
    IL_0086:  ldloc.s    V_4
    IL_0088:  ldc.i4.0
    IL_0089:  conv.i8
    IL_008a:  bgt.un.s   IL_0072

    IL_008c:  nop
    IL_008d:  br.s       IL_00ba

    IL_008f:  ldloc.1
    IL_0090:  ldc.i4.1
    IL_0091:  conv.i8
    IL_0092:  add.ovf.un
    IL_0093:  stloc.s    V_4
    IL_0095:  ldc.i4.0
    IL_0096:  conv.i8
    IL_0097:  stloc.s    V_5
    IL_0099:  ldc.i4.1
    IL_009a:  conv.i8
    IL_009b:  stloc.s    V_6
    IL_009d:  br.s       IL_00b3

    IL_009f:  ldloc.3
    IL_00a0:  ldloc.s    V_5
    IL_00a2:  conv.i
    IL_00a3:  ldloc.s    V_6
    IL_00a5:  stelem.i8
    IL_00a6:  ldloc.s    V_6
    IL_00a8:  ldloc.0
    IL_00a9:  add
    IL_00aa:  stloc.s    V_6
    IL_00ac:  ldloc.s    V_5
    IL_00ae:  ldc.i4.1
    IL_00af:  conv.i8
    IL_00b0:  add
    IL_00b1:  stloc.s    V_5
    IL_00b3:  ldloc.s    V_5
    IL_00b5:  ldloc.s    V_4
    IL_00b7:  blt.un.s   IL_009f

    IL_00b9:  nop
    IL_00ba:  ldloc.3
    IL_00bb:  ret
  } 

  .method public static uint64[]  f21(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,uint64> f) cil managed
  {
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             uint64[] V_2,
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
    IL_001b:  ldloc.1
    IL_001c:  ldc.i4.1
    IL_001d:  conv.i8
    IL_001e:  bge.un.s   IL_0026

    IL_0020:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_0025:  ret

    IL_0026:  ldloc.1
    IL_0027:  conv.ovf.i.un
    IL_0028:  newarr     [runtime]System.UInt64
    IL_002d:  stloc.2
    IL_002e:  ldc.i4.0
    IL_002f:  conv.i8
    IL_0030:  stloc.3
    IL_0031:  ldc.i4.1
    IL_0032:  conv.i8
    IL_0033:  stloc.s    V_4
    IL_0035:  br.s       IL_0049

    IL_0037:  ldloc.2
    IL_0038:  ldloc.3
    IL_0039:  conv.i
    IL_003a:  ldloc.s    V_4
    IL_003c:  stelem.i8
    IL_003d:  ldloc.s    V_4
    IL_003f:  ldc.i4.1
    IL_0040:  conv.i8
    IL_0041:  add
    IL_0042:  stloc.s    V_4
    IL_0044:  ldloc.3
    IL_0045:  ldc.i4.1
    IL_0046:  conv.i8
    IL_0047:  add
    IL_0048:  stloc.3
    IL_0049:  ldloc.3
    IL_004a:  ldloc.1
    IL_004b:  blt.un.s   IL_0037

    IL_004d:  ldloc.2
    IL_004e:  ret
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
    IL_0040:  ldloc.3
    IL_0041:  ldc.i4.1
    IL_0042:  conv.i8
    IL_0043:  add.ovf.un
    IL_0044:  ldc.i4.1
    IL_0045:  conv.i8
    IL_0046:  bge.un.s   IL_004e

    IL_0048:  call       !!0[] [runtime]System.Array::Empty<uint64>()
    IL_004d:  ret

    IL_004e:  ldloc.3
    IL_004f:  ldc.i4.1
    IL_0050:  conv.i8
    IL_0051:  add.ovf.un
    IL_0052:  conv.ovf.i.un
    IL_0053:  newarr     [runtime]System.UInt64
    IL_0058:  stloc.s    V_5
    IL_005a:  ldloc.s    V_4
    IL_005c:  brfalse.s  IL_009a

    IL_005e:  ldc.i4.0
    IL_005f:  conv.i8
    IL_0060:  stloc.s    V_6
    IL_0062:  ldloc.0
    IL_0063:  stloc.s    V_7
    IL_0065:  ldloc.s    V_5
    IL_0067:  ldloc.s    V_6
    IL_0069:  conv.i
    IL_006a:  ldloc.s    V_7
    IL_006c:  stelem.i8
    IL_006d:  ldloc.s    V_7
    IL_006f:  ldloc.1
    IL_0070:  add
    IL_0071:  stloc.s    V_7
    IL_0073:  ldloc.s    V_6
    IL_0075:  ldc.i4.1
    IL_0076:  conv.i8
    IL_0077:  add
    IL_0078:  stloc.s    V_6
    IL_007a:  br.s       IL_0091

    IL_007c:  ldloc.s    V_5
    IL_007e:  ldloc.s    V_6
    IL_0080:  conv.i
    IL_0081:  ldloc.s    V_7
    IL_0083:  stelem.i8
    IL_0084:  ldloc.s    V_7
    IL_0086:  ldloc.1
    IL_0087:  add
    IL_0088:  stloc.s    V_7
    IL_008a:  ldloc.s    V_6
    IL_008c:  ldc.i4.1
    IL_008d:  conv.i8
    IL_008e:  add
    IL_008f:  stloc.s    V_6
    IL_0091:  ldloc.s    V_6
    IL_0093:  ldc.i4.0
    IL_0094:  conv.i8
    IL_0095:  bgt.un.s   IL_007c

    IL_0097:  nop
    IL_0098:  br.s       IL_00c5

    IL_009a:  ldloc.3
    IL_009b:  ldc.i4.1
    IL_009c:  conv.i8
    IL_009d:  add.ovf.un
    IL_009e:  stloc.s    V_6
    IL_00a0:  ldc.i4.0
    IL_00a1:  conv.i8
    IL_00a2:  stloc.s    V_7
    IL_00a4:  ldloc.0
    IL_00a5:  stloc.s    V_8
    IL_00a7:  br.s       IL_00be

    IL_00a9:  ldloc.s    V_5
    IL_00ab:  ldloc.s    V_7
    IL_00ad:  conv.i
    IL_00ae:  ldloc.s    V_8
    IL_00b0:  stelem.i8
    IL_00b1:  ldloc.s    V_8
    IL_00b3:  ldloc.1
    IL_00b4:  add
    IL_00b5:  stloc.s    V_8
    IL_00b7:  ldloc.s    V_7
    IL_00b9:  ldc.i4.1
    IL_00ba:  conv.i8
    IL_00bb:  add
    IL_00bc:  stloc.s    V_7
    IL_00be:  ldloc.s    V_7
    IL_00c0:  ldloc.s    V_6
    IL_00c2:  blt.un.s   IL_00a9

    IL_00c4:  nop
    IL_00c5:  ldloc.s    V_5
    IL_00c7:  ret
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






