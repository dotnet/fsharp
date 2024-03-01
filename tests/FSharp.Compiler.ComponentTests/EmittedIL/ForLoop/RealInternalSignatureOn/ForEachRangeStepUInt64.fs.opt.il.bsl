




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
  .field static assembly uint64 c@1
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public specialname static uint64 get_c() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     uint64 assembly::c@1
    IL_0005:  ret
  } 

  .method public specialname static void set_c(uint64 'value') cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  stsfld     uint64 assembly::c@1
    IL_0006:  ret
  } 

  .method public static void  f0() cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             uint64 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stloc.0
    IL_0003:  ldc.i4.s   10
    IL_0005:  conv.i8
    IL_0006:  stloc.1
    IL_0007:  br.s       IL_0019

    IL_0009:  ldloc.1
    IL_000a:  call       void assembly::set_c(uint64)
    IL_000f:  ldloc.1
    IL_0010:  ldc.i4.1
    IL_0011:  conv.i8
    IL_0012:  add
    IL_0013:  stloc.1
    IL_0014:  ldloc.0
    IL_0015:  ldc.i4.1
    IL_0016:  conv.i8
    IL_0017:  add
    IL_0018:  stloc.0
    IL_0019:  ldloc.0
    IL_001a:  ldc.i4.0
    IL_001b:  conv.i8
    IL_001c:  blt.un.s   IL_0009

    IL_001e:  ret
  } 

  .method public static void  f00() cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             uint64 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stloc.0
    IL_0003:  ldc.i4.s   10
    IL_0005:  conv.i8
    IL_0006:  stloc.1
    IL_0007:  br.s       IL_0019

    IL_0009:  ldloc.1
    IL_000a:  call       void assembly::set_c(uint64)
    IL_000f:  ldloc.1
    IL_0010:  ldc.i4.1
    IL_0011:  conv.i8
    IL_0012:  add
    IL_0013:  stloc.1
    IL_0014:  ldloc.0
    IL_0015:  ldc.i4.1
    IL_0016:  conv.i8
    IL_0017:  add
    IL_0018:  stloc.0
    IL_0019:  ldloc.0
    IL_001a:  ldc.i4.0
    IL_001b:  conv.i8
    IL_001c:  blt.un.s   IL_0009

    IL_001e:  ret
  } 

  .method public static void  f1() cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             uint64 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stloc.0
    IL_0003:  ldc.i4.1
    IL_0004:  conv.i8
    IL_0005:  stloc.1
    IL_0006:  br.s       IL_0018

    IL_0008:  ldloc.1
    IL_0009:  call       void assembly::set_c(uint64)
    IL_000e:  ldloc.1
    IL_000f:  ldc.i4.1
    IL_0010:  conv.i8
    IL_0011:  add
    IL_0012:  stloc.1
    IL_0013:  ldloc.0
    IL_0014:  ldc.i4.1
    IL_0015:  conv.i8
    IL_0016:  add
    IL_0017:  stloc.0
    IL_0018:  ldloc.0
    IL_0019:  ldc.i4.s   10
    IL_001b:  conv.i8
    IL_001c:  blt.un.s   IL_0008

    IL_001e:  ret
  } 

  .method public static void  f2(uint64 start) cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             uint64 V_1,
             uint64 V_2)
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
    IL_0017:  stloc.1
    IL_0018:  ldarg.0
    IL_0019:  stloc.2
    IL_001a:  br.s       IL_002c

    IL_001c:  ldloc.2
    IL_001d:  call       void assembly::set_c(uint64)
    IL_0022:  ldloc.2
    IL_0023:  ldc.i4.1
    IL_0024:  conv.i8
    IL_0025:  add
    IL_0026:  stloc.2
    IL_0027:  ldloc.1
    IL_0028:  ldc.i4.1
    IL_0029:  conv.i8
    IL_002a:  add
    IL_002b:  stloc.1
    IL_002c:  ldloc.1
    IL_002d:  ldloc.0
    IL_002e:  blt.un.s   IL_001c

    IL_0030:  ret
  } 

  .method public static void  f3(uint64 finish) cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             uint64 V_1,
             uint64 V_2)
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
    IL_0015:  stloc.1
    IL_0016:  ldc.i4.1
    IL_0017:  conv.i8
    IL_0018:  stloc.2
    IL_0019:  br.s       IL_002b

    IL_001b:  ldloc.2
    IL_001c:  call       void assembly::set_c(uint64)
    IL_0021:  ldloc.2
    IL_0022:  ldc.i4.1
    IL_0023:  conv.i8
    IL_0024:  add
    IL_0025:  stloc.2
    IL_0026:  ldloc.1
    IL_0027:  ldc.i4.1
    IL_0028:  conv.i8
    IL_0029:  add
    IL_002a:  stloc.1
    IL_002b:  ldloc.1
    IL_002c:  ldloc.0
    IL_002d:  blt.un.s   IL_001b

    IL_002f:  ret
  } 

  .method public static void  f4(uint64 start,
                                 uint64 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  4
    .locals init (uint64 V_0,
             uint64 V_1,
             uint64 V_2,
             uint64 V_3)
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
    IL_0011:  bne.un.s   IL_0040

    IL_0013:  ldc.i4.0
    IL_0014:  conv.i8
    IL_0015:  stloc.1
    IL_0016:  ldarg.0
    IL_0017:  stloc.2
    IL_0018:  ldloc.2
    IL_0019:  call       void assembly::set_c(uint64)
    IL_001e:  ldloc.2
    IL_001f:  ldc.i4.1
    IL_0020:  conv.i8
    IL_0021:  add
    IL_0022:  stloc.2
    IL_0023:  ldloc.1
    IL_0024:  ldc.i4.1
    IL_0025:  conv.i8
    IL_0026:  add
    IL_0027:  stloc.1
    IL_0028:  br.s       IL_003a

    IL_002a:  ldloc.2
    IL_002b:  call       void assembly::set_c(uint64)
    IL_0030:  ldloc.2
    IL_0031:  ldc.i4.1
    IL_0032:  conv.i8
    IL_0033:  add
    IL_0034:  stloc.2
    IL_0035:  ldloc.1
    IL_0036:  ldc.i4.1
    IL_0037:  conv.i8
    IL_0038:  add
    IL_0039:  stloc.1
    IL_003a:  ldloc.1
    IL_003b:  ldc.i4.0
    IL_003c:  conv.i8
    IL_003d:  bgt.un.s   IL_002a

    IL_003f:  ret

    IL_0040:  ldarg.1
    IL_0041:  ldarg.0
    IL_0042:  bge.un.s   IL_0049

    IL_0044:  ldc.i4.0
    IL_0045:  conv.i8
    IL_0046:  nop
    IL_0047:  br.s       IL_0050

    IL_0049:  ldarg.1
    IL_004a:  ldarg.0
    IL_004b:  sub
    IL_004c:  ldc.i4.1
    IL_004d:  conv.i8
    IL_004e:  add.ovf.un
    IL_004f:  nop
    IL_0050:  stloc.1
    IL_0051:  ldc.i4.0
    IL_0052:  conv.i8
    IL_0053:  stloc.2
    IL_0054:  ldarg.0
    IL_0055:  stloc.3
    IL_0056:  br.s       IL_0068

    IL_0058:  ldloc.3
    IL_0059:  call       void assembly::set_c(uint64)
    IL_005e:  ldloc.3
    IL_005f:  ldc.i4.1
    IL_0060:  conv.i8
    IL_0061:  add
    IL_0062:  stloc.3
    IL_0063:  ldloc.2
    IL_0064:  ldc.i4.1
    IL_0065:  conv.i8
    IL_0066:  add
    IL_0067:  stloc.2
    IL_0068:  ldloc.2
    IL_0069:  ldloc.1
    IL_006a:  blt.un.s   IL_0058

    IL_006c:  ret
  } 

  .method public static void  f5() cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             uint64 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stloc.0
    IL_0003:  ldc.i4.1
    IL_0004:  conv.i8
    IL_0005:  stloc.1
    IL_0006:  br.s       IL_0018

    IL_0008:  ldloc.1
    IL_0009:  call       void assembly::set_c(uint64)
    IL_000e:  ldloc.1
    IL_000f:  ldc.i4.1
    IL_0010:  conv.i8
    IL_0011:  add
    IL_0012:  stloc.1
    IL_0013:  ldloc.0
    IL_0014:  ldc.i4.1
    IL_0015:  conv.i8
    IL_0016:  add
    IL_0017:  stloc.0
    IL_0018:  ldloc.0
    IL_0019:  ldc.i4.s   10
    IL_001b:  conv.i8
    IL_001c:  blt.un.s   IL_0008

    IL_001e:  ret
  } 

  .method public static void  f6() cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             uint64 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stloc.0
    IL_0003:  ldc.i4.1
    IL_0004:  conv.i8
    IL_0005:  stloc.1
    IL_0006:  br.s       IL_0018

    IL_0008:  ldloc.1
    IL_0009:  call       void assembly::set_c(uint64)
    IL_000e:  ldloc.1
    IL_000f:  ldc.i4.2
    IL_0010:  conv.i8
    IL_0011:  add
    IL_0012:  stloc.1
    IL_0013:  ldloc.0
    IL_0014:  ldc.i4.1
    IL_0015:  conv.i8
    IL_0016:  add
    IL_0017:  stloc.0
    IL_0018:  ldloc.0
    IL_0019:  ldc.i4.5
    IL_001a:  conv.i8
    IL_001b:  blt.un.s   IL_0008

    IL_001d:  ret
  } 

  .method public static void  f7(uint64 start) cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             uint64 V_1,
             uint64 V_2)
    IL_0000:  ldc.i4.s   10
    IL_0002:  conv.i8
    IL_0003:  ldarg.0
    IL_0004:  bge.un.s   IL_000b

    IL_0006:  ldc.i4.0
    IL_0007:  conv.i8
    IL_0008:  nop
    IL_0009:  br.s       IL_0017

    IL_000b:  ldc.i4.s   10
    IL_000d:  conv.i8
    IL_000e:  ldarg.0
    IL_000f:  sub
    IL_0010:  ldc.i4.2
    IL_0011:  conv.i8
    IL_0012:  div.un
    IL_0013:  ldc.i4.1
    IL_0014:  conv.i8
    IL_0015:  add.ovf.un
    IL_0016:  nop
    IL_0017:  stloc.0
    IL_0018:  ldc.i4.0
    IL_0019:  conv.i8
    IL_001a:  stloc.1
    IL_001b:  ldarg.0
    IL_001c:  stloc.2
    IL_001d:  br.s       IL_002f

    IL_001f:  ldloc.2
    IL_0020:  call       void assembly::set_c(uint64)
    IL_0025:  ldloc.2
    IL_0026:  ldc.i4.2
    IL_0027:  conv.i8
    IL_0028:  add
    IL_0029:  stloc.2
    IL_002a:  ldloc.1
    IL_002b:  ldc.i4.1
    IL_002c:  conv.i8
    IL_002d:  add
    IL_002e:  stloc.1
    IL_002f:  ldloc.1
    IL_0030:  ldloc.0
    IL_0031:  blt.un.s   IL_001f

    IL_0033:  ret
  } 

  .method public static void  f8(uint64 step) cil managed
  {
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             uint64 V_2)
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
    IL_0013:  ldc.i4.s   9
    IL_0015:  conv.i8
    IL_0016:  ldarg.0
    IL_0017:  div.un
    IL_0018:  ldc.i4.1
    IL_0019:  conv.i8
    IL_001a:  add.ovf.un
    IL_001b:  stloc.0
    IL_001c:  ldc.i4.0
    IL_001d:  conv.i8
    IL_001e:  stloc.1
    IL_001f:  ldc.i4.1
    IL_0020:  conv.i8
    IL_0021:  stloc.2
    IL_0022:  br.s       IL_0033

    IL_0024:  ldloc.2
    IL_0025:  call       void assembly::set_c(uint64)
    IL_002a:  ldloc.2
    IL_002b:  ldarg.0
    IL_002c:  add
    IL_002d:  stloc.2
    IL_002e:  ldloc.1
    IL_002f:  ldc.i4.1
    IL_0030:  conv.i8
    IL_0031:  add
    IL_0032:  stloc.1
    IL_0033:  ldloc.1
    IL_0034:  ldloc.0
    IL_0035:  blt.un.s   IL_0024

    IL_0037:  ret
  } 

  .method public static void  f9(uint64 finish) cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             uint64 V_1,
             uint64 V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  conv.i8
    IL_0003:  bge.un.s   IL_000a

    IL_0005:  ldc.i4.0
    IL_0006:  conv.i8
    IL_0007:  nop
    IL_0008:  br.s       IL_0015

    IL_000a:  ldarg.0
    IL_000b:  ldc.i4.1
    IL_000c:  conv.i8
    IL_000d:  sub
    IL_000e:  ldc.i4.2
    IL_000f:  conv.i8
    IL_0010:  div.un
    IL_0011:  ldc.i4.1
    IL_0012:  conv.i8
    IL_0013:  add.ovf.un
    IL_0014:  nop
    IL_0015:  stloc.0
    IL_0016:  ldc.i4.0
    IL_0017:  conv.i8
    IL_0018:  stloc.1
    IL_0019:  ldc.i4.1
    IL_001a:  conv.i8
    IL_001b:  stloc.2
    IL_001c:  br.s       IL_002e

    IL_001e:  ldloc.2
    IL_001f:  call       void assembly::set_c(uint64)
    IL_0024:  ldloc.2
    IL_0025:  ldc.i4.2
    IL_0026:  conv.i8
    IL_0027:  add
    IL_0028:  stloc.2
    IL_0029:  ldloc.1
    IL_002a:  ldc.i4.1
    IL_002b:  conv.i8
    IL_002c:  add
    IL_002d:  stloc.1
    IL_002e:  ldloc.1
    IL_002f:  ldloc.0
    IL_0030:  blt.un.s   IL_001e

    IL_0032:  ret
  } 

  .method public static void  f10(uint64 start,
                                  uint64 step,
                                  uint64 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             uint64 V_2,
             uint64 V_3)
    IL_0000:  ldarg.1
    IL_0001:  brtrue.s   IL_000f

    IL_0003:  ldarg.2
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
    IL_0011:  ldarg.2
    IL_0012:  bge.un.s   IL_0019

    IL_0014:  ldc.i4.0
    IL_0015:  conv.i8
    IL_0016:  nop
    IL_0017:  br.s       IL_001f

    IL_0019:  ldarg.2
    IL_001a:  ldarg.2
    IL_001b:  sub
    IL_001c:  ldarg.1
    IL_001d:  div.un
    IL_001e:  nop
    IL_001f:  stloc.0
    IL_0020:  ldloc.0
    IL_0021:  ldc.i4.m1
    IL_0022:  conv.i8
    IL_0023:  bne.un.s   IL_0050

    IL_0025:  ldc.i4.0
    IL_0026:  conv.i8
    IL_0027:  stloc.1
    IL_0028:  ldarg.2
    IL_0029:  stloc.2
    IL_002a:  ldloc.2
    IL_002b:  call       void assembly::set_c(uint64)
    IL_0030:  ldloc.2
    IL_0031:  ldarg.1
    IL_0032:  add
    IL_0033:  stloc.2
    IL_0034:  ldloc.1
    IL_0035:  ldc.i4.1
    IL_0036:  conv.i8
    IL_0037:  add
    IL_0038:  stloc.1
    IL_0039:  br.s       IL_004a

    IL_003b:  ldloc.2
    IL_003c:  call       void assembly::set_c(uint64)
    IL_0041:  ldloc.2
    IL_0042:  ldarg.1
    IL_0043:  add
    IL_0044:  stloc.2
    IL_0045:  ldloc.1
    IL_0046:  ldc.i4.1
    IL_0047:  conv.i8
    IL_0048:  add
    IL_0049:  stloc.1
    IL_004a:  ldloc.1
    IL_004b:  ldc.i4.0
    IL_004c:  conv.i8
    IL_004d:  bgt.un.s   IL_003b

    IL_004f:  ret

    IL_0050:  ldarg.2
    IL_0051:  ldarg.2
    IL_0052:  bge.un.s   IL_0059

    IL_0054:  ldc.i4.0
    IL_0055:  conv.i8
    IL_0056:  nop
    IL_0057:  br.s       IL_0062

    IL_0059:  ldarg.2
    IL_005a:  ldarg.2
    IL_005b:  sub
    IL_005c:  ldarg.1
    IL_005d:  div.un
    IL_005e:  ldc.i4.1
    IL_005f:  conv.i8
    IL_0060:  add.ovf.un
    IL_0061:  nop
    IL_0062:  stloc.1
    IL_0063:  ldc.i4.0
    IL_0064:  conv.i8
    IL_0065:  stloc.2
    IL_0066:  ldarg.2
    IL_0067:  stloc.3
    IL_0068:  br.s       IL_0079

    IL_006a:  ldloc.3
    IL_006b:  call       void assembly::set_c(uint64)
    IL_0070:  ldloc.3
    IL_0071:  ldarg.1
    IL_0072:  add
    IL_0073:  stloc.3
    IL_0074:  ldloc.2
    IL_0075:  ldc.i4.1
    IL_0076:  conv.i8
    IL_0077:  add
    IL_0078:  stloc.2
    IL_0079:  ldloc.2
    IL_007a:  ldloc.1
    IL_007b:  blt.un.s   IL_006a

    IL_007d:  ret
  } 

  .method public static void  f11(uint64 start,
                                  uint64 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             uint64 V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  conv.i8
    IL_0003:  ldarg.1
    IL_0004:  call       class [runtime]System.Collections.Generic.IEnumerable`1<uint64> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeUInt64(uint64,
                                                                                                                                                                             uint64,
                                                                                                                                                                             uint64)
    IL_0009:  pop
    IL_000a:  ldc.i4.0
    IL_000b:  conv.i8
    IL_000c:  stloc.0
    IL_000d:  ldc.i4.0
    IL_000e:  conv.i8
    IL_000f:  stloc.1
    IL_0010:  ldarg.0
    IL_0011:  stloc.2
    IL_0012:  br.s       IL_0024

    IL_0014:  ldloc.2
    IL_0015:  call       void assembly::set_c(uint64)
    IL_001a:  ldloc.2
    IL_001b:  ldc.i4.0
    IL_001c:  conv.i8
    IL_001d:  add
    IL_001e:  stloc.2
    IL_001f:  ldloc.1
    IL_0020:  ldc.i4.1
    IL_0021:  conv.i8
    IL_0022:  add
    IL_0023:  stloc.1
    IL_0024:  ldloc.1
    IL_0025:  ldloc.0
    IL_0026:  blt.un.s   IL_0014

    IL_0028:  ret
  } 

  .method public static void  f12() cil managed
  {
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             uint64 V_2)
    IL_0000:  ldc.i4.1
    IL_0001:  conv.i8
    IL_0002:  ldc.i4.0
    IL_0003:  conv.i8
    IL_0004:  ldc.i4.s   10
    IL_0006:  conv.i8
    IL_0007:  call       class [runtime]System.Collections.Generic.IEnumerable`1<uint64> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeUInt64(uint64,
                                                                                                                                                                             uint64,
                                                                                                                                                                             uint64)
    IL_000c:  pop
    IL_000d:  ldc.i4.0
    IL_000e:  conv.i8
    IL_000f:  stloc.0
    IL_0010:  ldc.i4.0
    IL_0011:  conv.i8
    IL_0012:  stloc.1
    IL_0013:  ldc.i4.1
    IL_0014:  conv.i8
    IL_0015:  stloc.2
    IL_0016:  br.s       IL_0028

    IL_0018:  ldloc.2
    IL_0019:  call       void assembly::set_c(uint64)
    IL_001e:  ldloc.2
    IL_001f:  ldc.i4.0
    IL_0020:  conv.i8
    IL_0021:  add
    IL_0022:  stloc.2
    IL_0023:  ldloc.1
    IL_0024:  ldc.i4.1
    IL_0025:  conv.i8
    IL_0026:  add
    IL_0027:  stloc.1
    IL_0028:  ldloc.1
    IL_0029:  ldloc.0
    IL_002a:  blt.un.s   IL_0018

    IL_002c:  ret
  } 

  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$assembly::init@
    IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$assembly::init@
    IL_000b:  pop
    IL_000c:  ret
  } 

  .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stsfld     uint64 assembly::c@1
    IL_0007:  ret
  } 

  .property uint64 c()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .set void assembly::set_c(uint64)
    .get uint64 assembly::get_c()
  } 
} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly
       extends [runtime]System.Object
{
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  8
    IL_0000:  call       void assembly::staticInitialization@()
    IL_0005:  ret
  } 

} 






