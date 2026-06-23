




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
  .method public static int32[]  f1() cil managed
  {
    
    .maxstack  5
    .locals init (int32[] V_0,
             uint64 V_1,
             int32 V_2)
    IL_0000:  ldc.i4.s   10
    IL_0002:  conv.i8
    IL_0003:  conv.ovf.i.un
    IL_0004:  newarr     [runtime]System.Int32
    IL_0009:  stloc.0
    IL_000a:  ldc.i4.0
    IL_000b:  conv.i8
    IL_000c:  stloc.1
    IL_000d:  ldc.i4.1
    IL_000e:  stloc.2
    IL_000f:  br.s       IL_001f

    IL_0011:  ldloc.0
    IL_0012:  ldloc.1
    IL_0013:  conv.i
    IL_0014:  ldloc.2
    IL_0015:  stelem.i4
    IL_0016:  ldloc.2
    IL_0017:  ldc.i4.1
    IL_0018:  add
    IL_0019:  stloc.2
    IL_001a:  ldloc.1
    IL_001b:  ldc.i4.1
    IL_001c:  conv.i8
    IL_001d:  add
    IL_001e:  stloc.1
    IL_001f:  ldloc.1
    IL_0020:  ldc.i4.s   10
    IL_0022:  conv.i8
    IL_0023:  blt.un.s   IL_0011

    IL_0025:  ldloc.0
    IL_0026:  ret
  } 

  .method public static int32[]  f2() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       !!0[] [runtime]System.Array::Empty<int32>()
    IL_0005:  ret
  } 

  .method public static int32[]  f3() cil managed
  {
    
    .maxstack  5
    .locals init (int32[] V_0,
             uint64 V_1,
             int32 V_2)
    IL_0000:  ldc.i4.s   10
    IL_0002:  conv.i8
    IL_0003:  conv.ovf.i.un
    IL_0004:  newarr     [runtime]System.Int32
    IL_0009:  stloc.0
    IL_000a:  ldc.i4.0
    IL_000b:  conv.i8
    IL_000c:  stloc.1
    IL_000d:  ldc.i4.1
    IL_000e:  stloc.2
    IL_000f:  br.s       IL_001f

    IL_0011:  ldloc.0
    IL_0012:  ldloc.1
    IL_0013:  conv.i
    IL_0014:  ldloc.2
    IL_0015:  stelem.i4
    IL_0016:  ldloc.2
    IL_0017:  ldc.i4.1
    IL_0018:  add
    IL_0019:  stloc.2
    IL_001a:  ldloc.1
    IL_001b:  ldc.i4.1
    IL_001c:  conv.i8
    IL_001d:  add
    IL_001e:  stloc.1
    IL_001f:  ldloc.1
    IL_0020:  ldc.i4.s   10
    IL_0022:  conv.i8
    IL_0023:  blt.un.s   IL_0011

    IL_0025:  ldloc.0
    IL_0026:  ret
  } 

  .method public static int32[]  f4() cil managed
  {
    
    .maxstack  5
    .locals init (int32[] V_0,
             uint64 V_1,
             int32 V_2)
    IL_0000:  ldc.i4.5
    IL_0001:  conv.i8
    IL_0002:  conv.ovf.i.un
    IL_0003:  newarr     [runtime]System.Int32
    IL_0008:  stloc.0
    IL_0009:  ldc.i4.0
    IL_000a:  conv.i8
    IL_000b:  stloc.1
    IL_000c:  ldc.i4.1
    IL_000d:  stloc.2
    IL_000e:  br.s       IL_001e

    IL_0010:  ldloc.0
    IL_0011:  ldloc.1
    IL_0012:  conv.i
    IL_0013:  ldloc.2
    IL_0014:  stelem.i4
    IL_0015:  ldloc.2
    IL_0016:  ldc.i4.2
    IL_0017:  add
    IL_0018:  stloc.2
    IL_0019:  ldloc.1
    IL_001a:  ldc.i4.1
    IL_001b:  conv.i8
    IL_001c:  add
    IL_001d:  stloc.1
    IL_001e:  ldloc.1
    IL_001f:  ldc.i4.5
    IL_0020:  conv.i8
    IL_0021:  blt.un.s   IL_0010

    IL_0023:  ldloc.0
    IL_0024:  ret
  } 

  .method public static int32[]  f5() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       !!0[] [runtime]System.Array::Empty<int32>()
    IL_0005:  ret
  } 

  .method public static int32[]  f6() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       !!0[] [runtime]System.Array::Empty<int32>()
    IL_0005:  ret
  } 

  .method public static int32[]  f7() cil managed
  {
    
    .maxstack  5
    .locals init (int32[] V_0,
             uint64 V_1,
             int32 V_2)
    IL_0000:  ldc.i4.s   10
    IL_0002:  conv.i8
    IL_0003:  conv.ovf.i.un
    IL_0004:  newarr     [runtime]System.Int32
    IL_0009:  stloc.0
    IL_000a:  ldc.i4.0
    IL_000b:  conv.i8
    IL_000c:  stloc.1
    IL_000d:  ldc.i4.s   10
    IL_000f:  stloc.2
    IL_0010:  br.s       IL_0020

    IL_0012:  ldloc.0
    IL_0013:  ldloc.1
    IL_0014:  conv.i
    IL_0015:  ldloc.2
    IL_0016:  stelem.i4
    IL_0017:  ldloc.2
    IL_0018:  ldc.i4.m1
    IL_0019:  add
    IL_001a:  stloc.2
    IL_001b:  ldloc.1
    IL_001c:  ldc.i4.1
    IL_001d:  conv.i8
    IL_001e:  add
    IL_001f:  stloc.1
    IL_0020:  ldloc.1
    IL_0021:  ldc.i4.s   10
    IL_0023:  conv.i8
    IL_0024:  blt.un.s   IL_0012

    IL_0026:  ldloc.0
    IL_0027:  ret
  } 

  .method public static int32[]  f8() cil managed
  {
    
    .maxstack  5
    .locals init (int32[] V_0,
             uint64 V_1,
             int32 V_2)
    IL_0000:  ldc.i4.5
    IL_0001:  conv.i8
    IL_0002:  conv.ovf.i.un
    IL_0003:  newarr     [runtime]System.Int32
    IL_0008:  stloc.0
    IL_0009:  ldc.i4.0
    IL_000a:  conv.i8
    IL_000b:  stloc.1
    IL_000c:  ldc.i4.s   10
    IL_000e:  stloc.2
    IL_000f:  br.s       IL_0020

    IL_0011:  ldloc.0
    IL_0012:  ldloc.1
    IL_0013:  conv.i
    IL_0014:  ldloc.2
    IL_0015:  stelem.i4
    IL_0016:  ldloc.2
    IL_0017:  ldc.i4.s   -2
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

  .method public static int32[]  f9(int32 start) cil managed
  {
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             int32[] V_2,
             uint64 V_3,
             int32 V_4)
    IL_0000:  ldc.i4.s   10
    IL_0002:  ldarg.0
    IL_0003:  bge.s      IL_000a

    IL_0005:  ldc.i4.0
    IL_0006:  conv.i8
    IL_0007:  nop
    IL_0008:  br.s       IL_0013

    IL_000a:  ldc.i4.s   10
    IL_000c:  ldarg.0
    IL_000d:  sub
    IL_000e:  conv.i8
    IL_000f:  ldc.i4.1
    IL_0010:  conv.i8
    IL_0011:  add
    IL_0012:  nop
    IL_0013:  stloc.0
    IL_0014:  ldloc.0
    IL_0015:  stloc.1
    IL_0016:  ldloc.1
    IL_0017:  brtrue.s   IL_001f

    IL_0019:  call       !!0[] [runtime]System.Array::Empty<int32>()
    IL_001e:  ret

    IL_001f:  ldloc.1
    IL_0020:  conv.ovf.i.un
    IL_0021:  newarr     [runtime]System.Int32
    IL_0026:  stloc.2
    IL_0027:  ldc.i4.0
    IL_0028:  conv.i8
    IL_0029:  stloc.3
    IL_002a:  ldarg.0
    IL_002b:  stloc.s    V_4
    IL_002d:  br.s       IL_0040

    IL_002f:  ldloc.2
    IL_0030:  ldloc.3
    IL_0031:  conv.i
    IL_0032:  ldloc.s    V_4
    IL_0034:  stelem.i4
    IL_0035:  ldloc.s    V_4
    IL_0037:  ldc.i4.1
    IL_0038:  add
    IL_0039:  stloc.s    V_4
    IL_003b:  ldloc.3
    IL_003c:  ldc.i4.1
    IL_003d:  conv.i8
    IL_003e:  add
    IL_003f:  stloc.3
    IL_0040:  ldloc.3
    IL_0041:  ldloc.0
    IL_0042:  blt.un.s   IL_002f

    IL_0044:  ldloc.2
    IL_0045:  ret
  } 

  .method public static int32[]  f10(int32 finish) cil managed
  {
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             int32[] V_2,
             uint64 V_3,
             int32 V_4)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  bge.s      IL_0009

    IL_0004:  ldc.i4.0
    IL_0005:  conv.i8
    IL_0006:  nop
    IL_0007:  br.s       IL_0011

    IL_0009:  ldarg.0
    IL_000a:  ldc.i4.1
    IL_000b:  sub
    IL_000c:  conv.i8
    IL_000d:  ldc.i4.1
    IL_000e:  conv.i8
    IL_000f:  add
    IL_0010:  nop
    IL_0011:  stloc.0
    IL_0012:  ldloc.0
    IL_0013:  stloc.1
    IL_0014:  ldloc.1
    IL_0015:  brtrue.s   IL_001d

    IL_0017:  call       !!0[] [runtime]System.Array::Empty<int32>()
    IL_001c:  ret

    IL_001d:  ldloc.1
    IL_001e:  conv.ovf.i.un
    IL_001f:  newarr     [runtime]System.Int32
    IL_0024:  stloc.2
    IL_0025:  ldc.i4.0
    IL_0026:  conv.i8
    IL_0027:  stloc.3
    IL_0028:  ldc.i4.1
    IL_0029:  stloc.s    V_4
    IL_002b:  br.s       IL_003e

    IL_002d:  ldloc.2
    IL_002e:  ldloc.3
    IL_002f:  conv.i
    IL_0030:  ldloc.s    V_4
    IL_0032:  stelem.i4
    IL_0033:  ldloc.s    V_4
    IL_0035:  ldc.i4.1
    IL_0036:  add
    IL_0037:  stloc.s    V_4
    IL_0039:  ldloc.3
    IL_003a:  ldc.i4.1
    IL_003b:  conv.i8
    IL_003c:  add
    IL_003d:  stloc.3
    IL_003e:  ldloc.3
    IL_003f:  ldloc.0
    IL_0040:  blt.un.s   IL_002d

    IL_0042:  ldloc.2
    IL_0043:  ret
  } 

  .method public static int32[]  f11(int32 start,
                                     int32 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             int32[] V_2,
             uint64 V_3,
             int32 V_4)
    IL_0000:  ldarg.1
    IL_0001:  ldarg.0
    IL_0002:  bge.s      IL_0009

    IL_0004:  ldc.i4.0
    IL_0005:  conv.i8
    IL_0006:  nop
    IL_0007:  br.s       IL_0011

    IL_0009:  ldarg.1
    IL_000a:  ldarg.0
    IL_000b:  sub
    IL_000c:  conv.i8
    IL_000d:  ldc.i4.1
    IL_000e:  conv.i8
    IL_000f:  add
    IL_0010:  nop
    IL_0011:  stloc.0
    IL_0012:  ldloc.0
    IL_0013:  stloc.1
    IL_0014:  ldloc.1
    IL_0015:  brtrue.s   IL_001d

    IL_0017:  call       !!0[] [runtime]System.Array::Empty<int32>()
    IL_001c:  ret

    IL_001d:  ldloc.1
    IL_001e:  conv.ovf.i.un
    IL_001f:  newarr     [runtime]System.Int32
    IL_0024:  stloc.2
    IL_0025:  ldc.i4.0
    IL_0026:  conv.i8
    IL_0027:  stloc.3
    IL_0028:  ldarg.0
    IL_0029:  stloc.s    V_4
    IL_002b:  br.s       IL_003e

    IL_002d:  ldloc.2
    IL_002e:  ldloc.3
    IL_002f:  conv.i
    IL_0030:  ldloc.s    V_4
    IL_0032:  stelem.i4
    IL_0033:  ldloc.s    V_4
    IL_0035:  ldc.i4.1
    IL_0036:  add
    IL_0037:  stloc.s    V_4
    IL_0039:  ldloc.3
    IL_003a:  ldc.i4.1
    IL_003b:  conv.i8
    IL_003c:  add
    IL_003d:  stloc.3
    IL_003e:  ldloc.3
    IL_003f:  ldloc.0
    IL_0040:  blt.un.s   IL_002d

    IL_0042:  ldloc.2
    IL_0043:  ret
  } 

  .method public static int32[]  f12(int32 start) cil managed
  {
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             int32[] V_2,
             uint64 V_3,
             int32 V_4)
    IL_0000:  ldc.i4.s   10
    IL_0002:  ldarg.0
    IL_0003:  bge.s      IL_000a

    IL_0005:  ldc.i4.0
    IL_0006:  conv.i8
    IL_0007:  nop
    IL_0008:  br.s       IL_0013

    IL_000a:  ldc.i4.s   10
    IL_000c:  ldarg.0
    IL_000d:  sub
    IL_000e:  conv.i8
    IL_000f:  ldc.i4.1
    IL_0010:  conv.i8
    IL_0011:  add
    IL_0012:  nop
    IL_0013:  stloc.0
    IL_0014:  ldloc.0
    IL_0015:  stloc.1
    IL_0016:  ldloc.1
    IL_0017:  brtrue.s   IL_001f

    IL_0019:  call       !!0[] [runtime]System.Array::Empty<int32>()
    IL_001e:  ret

    IL_001f:  ldloc.1
    IL_0020:  conv.ovf.i.un
    IL_0021:  newarr     [runtime]System.Int32
    IL_0026:  stloc.2
    IL_0027:  ldc.i4.0
    IL_0028:  conv.i8
    IL_0029:  stloc.3
    IL_002a:  ldarg.0
    IL_002b:  stloc.s    V_4
    IL_002d:  br.s       IL_0040

    IL_002f:  ldloc.2
    IL_0030:  ldloc.3
    IL_0031:  conv.i
    IL_0032:  ldloc.s    V_4
    IL_0034:  stelem.i4
    IL_0035:  ldloc.s    V_4
    IL_0037:  ldc.i4.1
    IL_0038:  add
    IL_0039:  stloc.s    V_4
    IL_003b:  ldloc.3
    IL_003c:  ldc.i4.1
    IL_003d:  conv.i8
    IL_003e:  add
    IL_003f:  stloc.3
    IL_0040:  ldloc.3
    IL_0041:  ldloc.0
    IL_0042:  blt.un.s   IL_002f

    IL_0044:  ldloc.2
    IL_0045:  ret
  } 

  .method public static int32[]  f13(int32 step) cil managed
  {
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             int32[] V_2,
             uint64 V_3,
             int32 V_4)
    IL_0000:  ldarg.0
    IL_0001:  brtrue.s   IL_0010

    IL_0003:  ldc.i4.1
    IL_0004:  ldarg.0
    IL_0005:  ldc.i4.s   10
    IL_0007:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                           int32,
                                                                                                                                                                           int32)
    IL_000c:  pop
    IL_000d:  nop
    IL_000e:  br.s       IL_0011

    IL_0010:  nop
    IL_0011:  ldc.i4.0
    IL_0012:  ldarg.0
    IL_0013:  bge.s      IL_002c

    IL_0015:  ldc.i4.s   10
    IL_0017:  ldc.i4.1
    IL_0018:  bge.s      IL_001f

    IL_001a:  ldc.i4.0
    IL_001b:  conv.i8
    IL_001c:  nop
    IL_001d:  br.s       IL_0044

    IL_001f:  ldc.i4.s   10
    IL_0021:  ldc.i4.1
    IL_0022:  sub
    IL_0023:  ldarg.0
    IL_0024:  div.un
    IL_0025:  conv.i8
    IL_0026:  ldc.i4.1
    IL_0027:  conv.i8
    IL_0028:  add
    IL_0029:  nop
    IL_002a:  br.s       IL_0044

    IL_002c:  ldc.i4.1
    IL_002d:  ldc.i4.s   10
    IL_002f:  bge.s      IL_0036

    IL_0031:  ldc.i4.0
    IL_0032:  conv.i8
    IL_0033:  nop
    IL_0034:  br.s       IL_0044

    IL_0036:  ldc.i4.1
    IL_0037:  ldc.i4.s   10
    IL_0039:  sub
    IL_003a:  ldarg.0
    IL_003b:  not
    IL_003c:  ldc.i4.1
    IL_003d:  add
    IL_003e:  div.un
    IL_003f:  conv.i8
    IL_0040:  ldc.i4.1
    IL_0041:  conv.i8
    IL_0042:  add
    IL_0043:  nop
    IL_0044:  stloc.0
    IL_0045:  ldloc.0
    IL_0046:  stloc.1
    IL_0047:  ldloc.1
    IL_0048:  brtrue.s   IL_0050

    IL_004a:  call       !!0[] [runtime]System.Array::Empty<int32>()
    IL_004f:  ret

    IL_0050:  ldloc.1
    IL_0051:  conv.ovf.i.un
    IL_0052:  newarr     [runtime]System.Int32
    IL_0057:  stloc.2
    IL_0058:  ldc.i4.0
    IL_0059:  conv.i8
    IL_005a:  stloc.3
    IL_005b:  ldc.i4.1
    IL_005c:  stloc.s    V_4
    IL_005e:  br.s       IL_0071

    IL_0060:  ldloc.2
    IL_0061:  ldloc.3
    IL_0062:  conv.i
    IL_0063:  ldloc.s    V_4
    IL_0065:  stelem.i4
    IL_0066:  ldloc.s    V_4
    IL_0068:  ldarg.0
    IL_0069:  add
    IL_006a:  stloc.s    V_4
    IL_006c:  ldloc.3
    IL_006d:  ldc.i4.1
    IL_006e:  conv.i8
    IL_006f:  add
    IL_0070:  stloc.3
    IL_0071:  ldloc.3
    IL_0072:  ldloc.0
    IL_0073:  blt.un.s   IL_0060

    IL_0075:  ldloc.2
    IL_0076:  ret
  } 

  .method public static int32[]  f14(int32 finish) cil managed
  {
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             int32[] V_2,
             uint64 V_3,
             int32 V_4)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  bge.s      IL_0009

    IL_0004:  ldc.i4.0
    IL_0005:  conv.i8
    IL_0006:  nop
    IL_0007:  br.s       IL_0011

    IL_0009:  ldarg.0
    IL_000a:  ldc.i4.1
    IL_000b:  sub
    IL_000c:  conv.i8
    IL_000d:  ldc.i4.1
    IL_000e:  conv.i8
    IL_000f:  add
    IL_0010:  nop
    IL_0011:  stloc.0
    IL_0012:  ldloc.0
    IL_0013:  stloc.1
    IL_0014:  ldloc.1
    IL_0015:  brtrue.s   IL_001d

    IL_0017:  call       !!0[] [runtime]System.Array::Empty<int32>()
    IL_001c:  ret

    IL_001d:  ldloc.1
    IL_001e:  conv.ovf.i.un
    IL_001f:  newarr     [runtime]System.Int32
    IL_0024:  stloc.2
    IL_0025:  ldc.i4.0
    IL_0026:  conv.i8
    IL_0027:  stloc.3
    IL_0028:  ldc.i4.1
    IL_0029:  stloc.s    V_4
    IL_002b:  br.s       IL_003e

    IL_002d:  ldloc.2
    IL_002e:  ldloc.3
    IL_002f:  conv.i
    IL_0030:  ldloc.s    V_4
    IL_0032:  stelem.i4
    IL_0033:  ldloc.s    V_4
    IL_0035:  ldc.i4.1
    IL_0036:  add
    IL_0037:  stloc.s    V_4
    IL_0039:  ldloc.3
    IL_003a:  ldc.i4.1
    IL_003b:  conv.i8
    IL_003c:  add
    IL_003d:  stloc.3
    IL_003e:  ldloc.3
    IL_003f:  ldloc.0
    IL_0040:  blt.un.s   IL_002d

    IL_0042:  ldloc.2
    IL_0043:  ret
  } 

  .method public static int32[]  f15(int32 start,
                                     int32 step) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             int32[] V_2,
             uint64 V_3,
             int32 V_4)
    IL_0000:  ldarg.1
    IL_0001:  brtrue.s   IL_0010

    IL_0003:  ldarg.0
    IL_0004:  ldarg.1
    IL_0005:  ldc.i4.s   10
    IL_0007:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                           int32,
                                                                                                                                                                           int32)
    IL_000c:  pop
    IL_000d:  nop
    IL_000e:  br.s       IL_0011

    IL_0010:  nop
    IL_0011:  ldc.i4.0
    IL_0012:  ldarg.1
    IL_0013:  bge.s      IL_002c

    IL_0015:  ldc.i4.s   10
    IL_0017:  ldarg.0
    IL_0018:  bge.s      IL_001f

    IL_001a:  ldc.i4.0
    IL_001b:  conv.i8
    IL_001c:  nop
    IL_001d:  br.s       IL_0044

    IL_001f:  ldc.i4.s   10
    IL_0021:  ldarg.0
    IL_0022:  sub
    IL_0023:  ldarg.1
    IL_0024:  div.un
    IL_0025:  conv.i8
    IL_0026:  ldc.i4.1
    IL_0027:  conv.i8
    IL_0028:  add
    IL_0029:  nop
    IL_002a:  br.s       IL_0044

    IL_002c:  ldarg.0
    IL_002d:  ldc.i4.s   10
    IL_002f:  bge.s      IL_0036

    IL_0031:  ldc.i4.0
    IL_0032:  conv.i8
    IL_0033:  nop
    IL_0034:  br.s       IL_0044

    IL_0036:  ldarg.0
    IL_0037:  ldc.i4.s   10
    IL_0039:  sub
    IL_003a:  ldarg.1
    IL_003b:  not
    IL_003c:  ldc.i4.1
    IL_003d:  add
    IL_003e:  div.un
    IL_003f:  conv.i8
    IL_0040:  ldc.i4.1
    IL_0041:  conv.i8
    IL_0042:  add
    IL_0043:  nop
    IL_0044:  stloc.0
    IL_0045:  ldloc.0
    IL_0046:  stloc.1
    IL_0047:  ldloc.1
    IL_0048:  brtrue.s   IL_0050

    IL_004a:  call       !!0[] [runtime]System.Array::Empty<int32>()
    IL_004f:  ret

    IL_0050:  ldloc.1
    IL_0051:  conv.ovf.i.un
    IL_0052:  newarr     [runtime]System.Int32
    IL_0057:  stloc.2
    IL_0058:  ldc.i4.0
    IL_0059:  conv.i8
    IL_005a:  stloc.3
    IL_005b:  ldarg.0
    IL_005c:  stloc.s    V_4
    IL_005e:  br.s       IL_0071

    IL_0060:  ldloc.2
    IL_0061:  ldloc.3
    IL_0062:  conv.i
    IL_0063:  ldloc.s    V_4
    IL_0065:  stelem.i4
    IL_0066:  ldloc.s    V_4
    IL_0068:  ldarg.1
    IL_0069:  add
    IL_006a:  stloc.s    V_4
    IL_006c:  ldloc.3
    IL_006d:  ldc.i4.1
    IL_006e:  conv.i8
    IL_006f:  add
    IL_0070:  stloc.3
    IL_0071:  ldloc.3
    IL_0072:  ldloc.0
    IL_0073:  blt.un.s   IL_0060

    IL_0075:  ldloc.2
    IL_0076:  ret
  } 

  .method public static int32[]  f16(int32 start,
                                     int32 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             int32[] V_2,
             uint64 V_3,
             int32 V_4)
    IL_0000:  ldarg.1
    IL_0001:  ldarg.0
    IL_0002:  bge.s      IL_0009

    IL_0004:  ldc.i4.0
    IL_0005:  conv.i8
    IL_0006:  nop
    IL_0007:  br.s       IL_0011

    IL_0009:  ldarg.1
    IL_000a:  ldarg.0
    IL_000b:  sub
    IL_000c:  conv.i8
    IL_000d:  ldc.i4.1
    IL_000e:  conv.i8
    IL_000f:  add
    IL_0010:  nop
    IL_0011:  stloc.0
    IL_0012:  ldloc.0
    IL_0013:  stloc.1
    IL_0014:  ldloc.1
    IL_0015:  brtrue.s   IL_001d

    IL_0017:  call       !!0[] [runtime]System.Array::Empty<int32>()
    IL_001c:  ret

    IL_001d:  ldloc.1
    IL_001e:  conv.ovf.i.un
    IL_001f:  newarr     [runtime]System.Int32
    IL_0024:  stloc.2
    IL_0025:  ldc.i4.0
    IL_0026:  conv.i8
    IL_0027:  stloc.3
    IL_0028:  ldarg.0
    IL_0029:  stloc.s    V_4
    IL_002b:  br.s       IL_003e

    IL_002d:  ldloc.2
    IL_002e:  ldloc.3
    IL_002f:  conv.i
    IL_0030:  ldloc.s    V_4
    IL_0032:  stelem.i4
    IL_0033:  ldloc.s    V_4
    IL_0035:  ldc.i4.1
    IL_0036:  add
    IL_0037:  stloc.s    V_4
    IL_0039:  ldloc.3
    IL_003a:  ldc.i4.1
    IL_003b:  conv.i8
    IL_003c:  add
    IL_003d:  stloc.3
    IL_003e:  ldloc.3
    IL_003f:  ldloc.0
    IL_0040:  blt.un.s   IL_002d

    IL_0042:  ldloc.2
    IL_0043:  ret
  } 

  .method public static int32[]  f17(int32 step,
                                     int32 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             int32[] V_2,
             uint64 V_3,
             int32 V_4)
    IL_0000:  ldarg.0
    IL_0001:  brtrue.s   IL_000f

    IL_0003:  ldc.i4.1
    IL_0004:  ldarg.0
    IL_0005:  ldarg.1
    IL_0006:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                           int32,
                                                                                                                                                                           int32)
    IL_000b:  pop
    IL_000c:  nop
    IL_000d:  br.s       IL_0010

    IL_000f:  nop
    IL_0010:  ldc.i4.0
    IL_0011:  ldarg.0
    IL_0012:  bge.s      IL_0029

    IL_0014:  ldarg.1
    IL_0015:  ldc.i4.1
    IL_0016:  bge.s      IL_001d

    IL_0018:  ldc.i4.0
    IL_0019:  conv.i8
    IL_001a:  nop
    IL_001b:  br.s       IL_003f

    IL_001d:  ldarg.1
    IL_001e:  ldc.i4.1
    IL_001f:  sub
    IL_0020:  ldarg.0
    IL_0021:  div.un
    IL_0022:  conv.i8
    IL_0023:  ldc.i4.1
    IL_0024:  conv.i8
    IL_0025:  add
    IL_0026:  nop
    IL_0027:  br.s       IL_003f

    IL_0029:  ldc.i4.1
    IL_002a:  ldarg.1
    IL_002b:  bge.s      IL_0032

    IL_002d:  ldc.i4.0
    IL_002e:  conv.i8
    IL_002f:  nop
    IL_0030:  br.s       IL_003f

    IL_0032:  ldc.i4.1
    IL_0033:  ldarg.1
    IL_0034:  sub
    IL_0035:  ldarg.0
    IL_0036:  not
    IL_0037:  ldc.i4.1
    IL_0038:  add
    IL_0039:  div.un
    IL_003a:  conv.i8
    IL_003b:  ldc.i4.1
    IL_003c:  conv.i8
    IL_003d:  add
    IL_003e:  nop
    IL_003f:  stloc.0
    IL_0040:  ldloc.0
    IL_0041:  stloc.1
    IL_0042:  ldloc.1
    IL_0043:  brtrue.s   IL_004b

    IL_0045:  call       !!0[] [runtime]System.Array::Empty<int32>()
    IL_004a:  ret

    IL_004b:  ldloc.1
    IL_004c:  conv.ovf.i.un
    IL_004d:  newarr     [runtime]System.Int32
    IL_0052:  stloc.2
    IL_0053:  ldc.i4.0
    IL_0054:  conv.i8
    IL_0055:  stloc.3
    IL_0056:  ldc.i4.1
    IL_0057:  stloc.s    V_4
    IL_0059:  br.s       IL_006c

    IL_005b:  ldloc.2
    IL_005c:  ldloc.3
    IL_005d:  conv.i
    IL_005e:  ldloc.s    V_4
    IL_0060:  stelem.i4
    IL_0061:  ldloc.s    V_4
    IL_0063:  ldarg.0
    IL_0064:  add
    IL_0065:  stloc.s    V_4
    IL_0067:  ldloc.3
    IL_0068:  ldc.i4.1
    IL_0069:  conv.i8
    IL_006a:  add
    IL_006b:  stloc.3
    IL_006c:  ldloc.3
    IL_006d:  ldloc.0
    IL_006e:  blt.un.s   IL_005b

    IL_0070:  ldloc.2
    IL_0071:  ret
  } 

  .method public static int32[]  f18(int32 start,
                                     int32 step,
                                     int32 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             int32[] V_2,
             uint64 V_3,
             int32 V_4)
    IL_0000:  ldarg.1
    IL_0001:  brtrue.s   IL_000f

    IL_0003:  ldarg.0
    IL_0004:  ldarg.1
    IL_0005:  ldarg.2
    IL_0006:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                           int32,
                                                                                                                                                                           int32)
    IL_000b:  pop
    IL_000c:  nop
    IL_000d:  br.s       IL_0010

    IL_000f:  nop
    IL_0010:  ldc.i4.0
    IL_0011:  ldarg.1
    IL_0012:  bge.s      IL_0029

    IL_0014:  ldarg.2
    IL_0015:  ldarg.0
    IL_0016:  bge.s      IL_001d

    IL_0018:  ldc.i4.0
    IL_0019:  conv.i8
    IL_001a:  nop
    IL_001b:  br.s       IL_003f

    IL_001d:  ldarg.2
    IL_001e:  ldarg.0
    IL_001f:  sub
    IL_0020:  ldarg.1
    IL_0021:  div.un
    IL_0022:  conv.i8
    IL_0023:  ldc.i4.1
    IL_0024:  conv.i8
    IL_0025:  add
    IL_0026:  nop
    IL_0027:  br.s       IL_003f

    IL_0029:  ldarg.0
    IL_002a:  ldarg.2
    IL_002b:  bge.s      IL_0032

    IL_002d:  ldc.i4.0
    IL_002e:  conv.i8
    IL_002f:  nop
    IL_0030:  br.s       IL_003f

    IL_0032:  ldarg.0
    IL_0033:  ldarg.2
    IL_0034:  sub
    IL_0035:  ldarg.1
    IL_0036:  not
    IL_0037:  ldc.i4.1
    IL_0038:  add
    IL_0039:  div.un
    IL_003a:  conv.i8
    IL_003b:  ldc.i4.1
    IL_003c:  conv.i8
    IL_003d:  add
    IL_003e:  nop
    IL_003f:  stloc.0
    IL_0040:  ldloc.0
    IL_0041:  stloc.1
    IL_0042:  ldloc.1
    IL_0043:  brtrue.s   IL_004b

    IL_0045:  call       !!0[] [runtime]System.Array::Empty<int32>()
    IL_004a:  ret

    IL_004b:  ldloc.1
    IL_004c:  conv.ovf.i.un
    IL_004d:  newarr     [runtime]System.Int32
    IL_0052:  stloc.2
    IL_0053:  ldc.i4.0
    IL_0054:  conv.i8
    IL_0055:  stloc.3
    IL_0056:  ldarg.0
    IL_0057:  stloc.s    V_4
    IL_0059:  br.s       IL_006c

    IL_005b:  ldloc.2
    IL_005c:  ldloc.3
    IL_005d:  conv.i
    IL_005e:  ldloc.s    V_4
    IL_0060:  stelem.i4
    IL_0061:  ldloc.s    V_4
    IL_0063:  ldarg.1
    IL_0064:  add
    IL_0065:  stloc.s    V_4
    IL_0067:  ldloc.3
    IL_0068:  ldc.i4.1
    IL_0069:  conv.i8
    IL_006a:  add
    IL_006b:  stloc.3
    IL_006c:  ldloc.3
    IL_006d:  ldloc.0
    IL_006e:  blt.un.s   IL_005b

    IL_0070:  ldloc.2
    IL_0071:  ret
  } 

  .method public static int32[]  f19(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> f) cil managed
  {
    
    .maxstack  5
    .locals init (int32 V_0,
             uint64 V_1,
             uint64 V_2,
             int32[] V_3,
             uint64 V_4,
             int32 V_5)
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
    IL_001c:  ldloc.1
    IL_001d:  stloc.2
    IL_001e:  ldloc.2
    IL_001f:  brtrue.s   IL_0027

    IL_0021:  call       !!0[] [runtime]System.Array::Empty<int32>()
    IL_0026:  ret

    IL_0027:  ldloc.2
    IL_0028:  conv.ovf.i.un
    IL_0029:  newarr     [runtime]System.Int32
    IL_002e:  stloc.3
    IL_002f:  ldc.i4.0
    IL_0030:  conv.i8
    IL_0031:  stloc.s    V_4
    IL_0033:  ldloc.0
    IL_0034:  stloc.s    V_5
    IL_0036:  br.s       IL_004c

    IL_0038:  ldloc.3
    IL_0039:  ldloc.s    V_4
    IL_003b:  conv.i
    IL_003c:  ldloc.s    V_5
    IL_003e:  stelem.i4
    IL_003f:  ldloc.s    V_5
    IL_0041:  ldc.i4.1
    IL_0042:  add
    IL_0043:  stloc.s    V_5
    IL_0045:  ldloc.s    V_4
    IL_0047:  ldc.i4.1
    IL_0048:  conv.i8
    IL_0049:  add
    IL_004a:  stloc.s    V_4
    IL_004c:  ldloc.s    V_4
    IL_004e:  ldloc.1
    IL_004f:  blt.un.s   IL_0038

    IL_0051:  ldloc.3
    IL_0052:  ret
  } 

  .method public static int32[]  f20(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> f) cil managed
  {
    
    .maxstack  5
    .locals init (int32 V_0,
             uint64 V_1,
             uint64 V_2,
             int32[] V_3,
             uint64 V_4,
             int32 V_5)
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
    IL_001a:  ldloc.1
    IL_001b:  stloc.2
    IL_001c:  ldloc.2
    IL_001d:  brtrue.s   IL_0025

    IL_001f:  call       !!0[] [runtime]System.Array::Empty<int32>()
    IL_0024:  ret

    IL_0025:  ldloc.2
    IL_0026:  conv.ovf.i.un
    IL_0027:  newarr     [runtime]System.Int32
    IL_002c:  stloc.3
    IL_002d:  ldc.i4.0
    IL_002e:  conv.i8
    IL_002f:  stloc.s    V_4
    IL_0031:  ldc.i4.1
    IL_0032:  stloc.s    V_5
    IL_0034:  br.s       IL_004a

    IL_0036:  ldloc.3
    IL_0037:  ldloc.s    V_4
    IL_0039:  conv.i
    IL_003a:  ldloc.s    V_5
    IL_003c:  stelem.i4
    IL_003d:  ldloc.s    V_5
    IL_003f:  ldc.i4.1
    IL_0040:  add
    IL_0041:  stloc.s    V_5
    IL_0043:  ldloc.s    V_4
    IL_0045:  ldc.i4.1
    IL_0046:  conv.i8
    IL_0047:  add
    IL_0048:  stloc.s    V_4
    IL_004a:  ldloc.s    V_4
    IL_004c:  ldloc.1
    IL_004d:  blt.un.s   IL_0036

    IL_004f:  ldloc.3
    IL_0050:  ret
  } 

  .method public static int32[]  f21(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> f,
                                     class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> g) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (int32 V_0,
             int32 V_1,
             uint64 V_2,
             uint64 V_3,
             int32[] V_4,
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
    IL_0022:  ldloc.2
    IL_0023:  stloc.3
    IL_0024:  ldloc.3
    IL_0025:  brtrue.s   IL_002d

    IL_0027:  call       !!0[] [runtime]System.Array::Empty<int32>()
    IL_002c:  ret

    IL_002d:  ldloc.3
    IL_002e:  conv.ovf.i.un
    IL_002f:  newarr     [runtime]System.Int32
    IL_0034:  stloc.s    V_4
    IL_0036:  ldc.i4.0
    IL_0037:  conv.i8
    IL_0038:  stloc.s    V_5
    IL_003a:  ldloc.0
    IL_003b:  stloc.s    V_6
    IL_003d:  br.s       IL_0054

    IL_003f:  ldloc.s    V_4
    IL_0041:  ldloc.s    V_5
    IL_0043:  conv.i
    IL_0044:  ldloc.s    V_6
    IL_0046:  stelem.i4
    IL_0047:  ldloc.s    V_6
    IL_0049:  ldc.i4.1
    IL_004a:  add
    IL_004b:  stloc.s    V_6
    IL_004d:  ldloc.s    V_5
    IL_004f:  ldc.i4.1
    IL_0050:  conv.i8
    IL_0051:  add
    IL_0052:  stloc.s    V_5
    IL_0054:  ldloc.s    V_5
    IL_0056:  ldloc.2
    IL_0057:  blt.un.s   IL_003f

    IL_0059:  ldloc.s    V_4
    IL_005b:  ret
  } 

  .method public static int32[]  f22(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> f) cil managed
  {
    
    .maxstack  5
    .locals init (int32 V_0,
             uint64 V_1,
             uint64 V_2,
             int32[] V_3,
             uint64 V_4,
             int32 V_5)
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
    IL_001c:  ldloc.1
    IL_001d:  stloc.2
    IL_001e:  ldloc.2
    IL_001f:  brtrue.s   IL_0027

    IL_0021:  call       !!0[] [runtime]System.Array::Empty<int32>()
    IL_0026:  ret

    IL_0027:  ldloc.2
    IL_0028:  conv.ovf.i.un
    IL_0029:  newarr     [runtime]System.Int32
    IL_002e:  stloc.3
    IL_002f:  ldc.i4.0
    IL_0030:  conv.i8
    IL_0031:  stloc.s    V_4
    IL_0033:  ldloc.0
    IL_0034:  stloc.s    V_5
    IL_0036:  br.s       IL_004c

    IL_0038:  ldloc.3
    IL_0039:  ldloc.s    V_4
    IL_003b:  conv.i
    IL_003c:  ldloc.s    V_5
    IL_003e:  stelem.i4
    IL_003f:  ldloc.s    V_5
    IL_0041:  ldc.i4.1
    IL_0042:  add
    IL_0043:  stloc.s    V_5
    IL_0045:  ldloc.s    V_4
    IL_0047:  ldc.i4.1
    IL_0048:  conv.i8
    IL_0049:  add
    IL_004a:  stloc.s    V_4
    IL_004c:  ldloc.s    V_4
    IL_004e:  ldloc.1
    IL_004f:  blt.un.s   IL_0038

    IL_0051:  ldloc.3
    IL_0052:  ret
  } 

  .method public static int32[]  f23(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> f) cil managed
  {
    
    .maxstack  5
    .locals init (int32 V_0,
             uint64 V_1,
             uint64 V_2,
             int32[] V_3,
             uint64 V_4,
             int32 V_5)
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
    IL_004d:  ldloc.1
    IL_004e:  stloc.2
    IL_004f:  ldloc.2
    IL_0050:  brtrue.s   IL_0058

    IL_0052:  call       !!0[] [runtime]System.Array::Empty<int32>()
    IL_0057:  ret

    IL_0058:  ldloc.2
    IL_0059:  conv.ovf.i.un
    IL_005a:  newarr     [runtime]System.Int32
    IL_005f:  stloc.3
    IL_0060:  ldc.i4.0
    IL_0061:  conv.i8
    IL_0062:  stloc.s    V_4
    IL_0064:  ldc.i4.1
    IL_0065:  stloc.s    V_5
    IL_0067:  br.s       IL_007d

    IL_0069:  ldloc.3
    IL_006a:  ldloc.s    V_4
    IL_006c:  conv.i
    IL_006d:  ldloc.s    V_5
    IL_006f:  stelem.i4
    IL_0070:  ldloc.s    V_5
    IL_0072:  ldloc.0
    IL_0073:  add
    IL_0074:  stloc.s    V_5
    IL_0076:  ldloc.s    V_4
    IL_0078:  ldc.i4.1
    IL_0079:  conv.i8
    IL_007a:  add
    IL_007b:  stloc.s    V_4
    IL_007d:  ldloc.s    V_4
    IL_007f:  ldloc.1
    IL_0080:  blt.un.s   IL_0069

    IL_0082:  ldloc.3
    IL_0083:  ret
  } 

  .method public static int32[]  f24(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> f) cil managed
  {
    
    .maxstack  5
    .locals init (int32 V_0,
             uint64 V_1,
             uint64 V_2,
             int32[] V_3,
             uint64 V_4,
             int32 V_5)
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
    IL_001a:  ldloc.1
    IL_001b:  stloc.2
    IL_001c:  ldloc.2
    IL_001d:  brtrue.s   IL_0025

    IL_001f:  call       !!0[] [runtime]System.Array::Empty<int32>()
    IL_0024:  ret

    IL_0025:  ldloc.2
    IL_0026:  conv.ovf.i.un
    IL_0027:  newarr     [runtime]System.Int32
    IL_002c:  stloc.3
    IL_002d:  ldc.i4.0
    IL_002e:  conv.i8
    IL_002f:  stloc.s    V_4
    IL_0031:  ldc.i4.1
    IL_0032:  stloc.s    V_5
    IL_0034:  br.s       IL_004a

    IL_0036:  ldloc.3
    IL_0037:  ldloc.s    V_4
    IL_0039:  conv.i
    IL_003a:  ldloc.s    V_5
    IL_003c:  stelem.i4
    IL_003d:  ldloc.s    V_5
    IL_003f:  ldc.i4.1
    IL_0040:  add
    IL_0041:  stloc.s    V_5
    IL_0043:  ldloc.s    V_4
    IL_0045:  ldc.i4.1
    IL_0046:  conv.i8
    IL_0047:  add
    IL_0048:  stloc.s    V_4
    IL_004a:  ldloc.s    V_4
    IL_004c:  ldloc.1
    IL_004d:  blt.un.s   IL_0036

    IL_004f:  ldloc.3
    IL_0050:  ret
  } 

  .method public static int32[]  f25(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> f,
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
             uint64 V_4,
             int32[] V_5,
             uint64 V_6,
             int32 V_7)
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
    IL_0058:  ldloc.3
    IL_0059:  stloc.s    V_4
    IL_005b:  ldloc.s    V_4
    IL_005d:  brtrue.s   IL_0065

    IL_005f:  call       !!0[] [runtime]System.Array::Empty<int32>()
    IL_0064:  ret

    IL_0065:  ldloc.s    V_4
    IL_0067:  conv.ovf.i.un
    IL_0068:  newarr     [runtime]System.Int32
    IL_006d:  stloc.s    V_5
    IL_006f:  ldc.i4.0
    IL_0070:  conv.i8
    IL_0071:  stloc.s    V_6
    IL_0073:  ldloc.0
    IL_0074:  stloc.s    V_7
    IL_0076:  br.s       IL_008d

    IL_0078:  ldloc.s    V_5
    IL_007a:  ldloc.s    V_6
    IL_007c:  conv.i
    IL_007d:  ldloc.s    V_7
    IL_007f:  stelem.i4
    IL_0080:  ldloc.s    V_7
    IL_0082:  ldloc.1
    IL_0083:  add
    IL_0084:  stloc.s    V_7
    IL_0086:  ldloc.s    V_6
    IL_0088:  ldc.i4.1
    IL_0089:  conv.i8
    IL_008a:  add
    IL_008b:  stloc.s    V_6
    IL_008d:  ldloc.s    V_6
    IL_008f:  ldloc.3
    IL_0090:  blt.un.s   IL_0078

    IL_0092:  ldloc.s    V_5
    IL_0094:  ret
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





