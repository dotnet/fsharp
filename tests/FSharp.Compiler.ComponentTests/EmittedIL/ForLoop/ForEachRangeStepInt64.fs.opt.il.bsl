




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
  .method public specialname static int64 get_c() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int64 '<StartupCode$assembly>'.$assembly::c@1
    IL_0005:  ret
  } 

  .method public specialname static void set_c(int64 'value') cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  stsfld     int64 '<StartupCode$assembly>'.$assembly::c@1
    IL_0006:  ret
  } 

  .method public static void  f0() cil managed
  {
    
    .maxstack  4
    .locals init (int64 V_0,
             int64 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stloc.0
    IL_0003:  ldc.i4.s   10
    IL_0005:  conv.i8
    IL_0006:  stloc.1
    IL_0007:  br.s       IL_0019

    IL_0009:  ldloc.1
    IL_000a:  call       void assembly::set_c(int64)
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
    .locals init (int64 V_0,
             int64 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stloc.0
    IL_0003:  ldc.i4.s   10
    IL_0005:  conv.i8
    IL_0006:  stloc.1
    IL_0007:  br.s       IL_0019

    IL_0009:  ldloc.1
    IL_000a:  call       void assembly::set_c(int64)
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
             int64 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stloc.0
    IL_0003:  ldc.i4.1
    IL_0004:  conv.i8
    IL_0005:  stloc.1
    IL_0006:  br.s       IL_0018

    IL_0008:  ldloc.1
    IL_0009:  call       void assembly::set_c(int64)
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

  .method public static void  f2(int64 start) cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             uint64 V_1,
             int64 V_2)
    IL_0000:  ldc.i4.s   10
    IL_0002:  conv.i8
    IL_0003:  ldarg.0
    IL_0004:  bge.s      IL_000b

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
    IL_001d:  call       void assembly::set_c(int64)
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

  .method public static void  f3(int64 finish) cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             uint64 V_1,
             int64 V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  conv.i8
    IL_0003:  bge.s      IL_000a

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
    IL_001c:  call       void assembly::set_c(int64)
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

  .method public static void  f4(int64 start,
                                 int64 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  4
    .locals init (uint64 V_0,
             uint64 V_1,
             int64 V_2,
             uint64 V_3)
    IL_0000:  ldarg.1
    IL_0001:  ldarg.0
    IL_0002:  bge.s      IL_0009

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
    IL_0019:  call       void assembly::set_c(int64)
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
    IL_002b:  call       void assembly::set_c(int64)
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
    IL_0042:  bge.s      IL_0049

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
    IL_0053:  stloc.3
    IL_0054:  ldarg.0
    IL_0055:  stloc.2
    IL_0056:  br.s       IL_0068

    IL_0058:  ldloc.2
    IL_0059:  call       void assembly::set_c(int64)
    IL_005e:  ldloc.2
    IL_005f:  ldc.i4.1
    IL_0060:  conv.i8
    IL_0061:  add
    IL_0062:  stloc.2
    IL_0063:  ldloc.3
    IL_0064:  ldc.i4.1
    IL_0065:  conv.i8
    IL_0066:  add
    IL_0067:  stloc.3
    IL_0068:  ldloc.3
    IL_0069:  ldloc.1
    IL_006a:  blt.un.s   IL_0058

    IL_006c:  ret
  } 

  .method public static void  f5() cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             int64 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stloc.0
    IL_0003:  ldc.i4.1
    IL_0004:  conv.i8
    IL_0005:  stloc.1
    IL_0006:  br.s       IL_0018

    IL_0008:  ldloc.1
    IL_0009:  call       void assembly::set_c(int64)
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
             int64 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stloc.0
    IL_0003:  ldc.i4.1
    IL_0004:  conv.i8
    IL_0005:  stloc.1
    IL_0006:  br.s       IL_0018

    IL_0008:  ldloc.1
    IL_0009:  call       void assembly::set_c(int64)
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

  .method public static void  f7(int64 start) cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             uint64 V_1,
             int64 V_2)
    IL_0000:  ldc.i4.s   10
    IL_0002:  conv.i8
    IL_0003:  ldarg.0
    IL_0004:  bge.s      IL_000b

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
    IL_0020:  call       void assembly::set_c(int64)
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

  .method public static void  f8(int64 step) cil managed
  {
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             int64 V_2)
    IL_0000:  ldarg.0
    IL_0001:  brtrue.s   IL_0012

    IL_0003:  ldc.i4.1
    IL_0004:  conv.i8
    IL_0005:  ldarg.0
    IL_0006:  ldc.i4.s   10
    IL_0008:  conv.i8
    IL_0009:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int64> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt64(int64,
                                                                                                                                                                           int64,
                                                                                                                                                                           int64)
    IL_000e:  pop
    IL_000f:  nop
    IL_0010:  br.s       IL_0013

    IL_0012:  nop
    IL_0013:  ldc.i4.0
    IL_0014:  conv.i8
    IL_0015:  ldarg.0
    IL_0016:  bge.s      IL_0023

    IL_0018:  ldc.i4.s   9
    IL_001a:  conv.i8
    IL_001b:  ldarg.0
    IL_001c:  div.un
    IL_001d:  ldc.i4.1
    IL_001e:  conv.i8
    IL_001f:  add.ovf.un
    IL_0020:  nop
    IL_0021:  br.s       IL_0026

    IL_0023:  ldc.i4.0
    IL_0024:  conv.i8
    IL_0025:  nop
    IL_0026:  stloc.0
    IL_0027:  ldc.i4.0
    IL_0028:  conv.i8
    IL_0029:  stloc.1
    IL_002a:  ldc.i4.1
    IL_002b:  conv.i8
    IL_002c:  stloc.2
    IL_002d:  br.s       IL_003e

    IL_002f:  ldloc.2
    IL_0030:  call       void assembly::set_c(int64)
    IL_0035:  ldloc.2
    IL_0036:  ldarg.0
    IL_0037:  add
    IL_0038:  stloc.2
    IL_0039:  ldloc.1
    IL_003a:  ldc.i4.1
    IL_003b:  conv.i8
    IL_003c:  add
    IL_003d:  stloc.1
    IL_003e:  ldloc.1
    IL_003f:  ldloc.0
    IL_0040:  blt.un.s   IL_002f

    IL_0042:  ret
  } 

  .method public static void  f9(int64 finish) cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             uint64 V_1,
             int64 V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  conv.i8
    IL_0003:  bge.s      IL_000a

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
    IL_001f:  call       void assembly::set_c(int64)
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

  .method public static void  f10(int64 start,
                                  int64 step,
                                  int64 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             int64 V_2,
             uint64 V_3)
    IL_0000:  ldarg.1
    IL_0001:  brtrue.s   IL_000f

    IL_0003:  ldarg.2
    IL_0004:  ldarg.1
    IL_0005:  ldarg.2
    IL_0006:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int64> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt64(int64,
                                                                                                                                                                           int64,
                                                                                                                                                                           int64)
    IL_000b:  pop
    IL_000c:  nop
    IL_000d:  br.s       IL_0010

    IL_000f:  nop
    IL_0010:  ldc.i4.0
    IL_0011:  conv.i8
    IL_0012:  ldarg.1
    IL_0013:  bge.s      IL_0026

    IL_0015:  ldarg.2
    IL_0016:  ldarg.2
    IL_0017:  bge.s      IL_001e

    IL_0019:  ldc.i4.0
    IL_001a:  conv.i8
    IL_001b:  nop
    IL_001c:  br.s       IL_0039

    IL_001e:  ldarg.2
    IL_001f:  ldarg.2
    IL_0020:  sub
    IL_0021:  ldarg.1
    IL_0022:  div.un
    IL_0023:  nop
    IL_0024:  br.s       IL_0039

    IL_0026:  ldarg.2
    IL_0027:  ldarg.2
    IL_0028:  bge.s      IL_002f

    IL_002a:  ldc.i4.0
    IL_002b:  conv.i8
    IL_002c:  nop
    IL_002d:  br.s       IL_0039

    IL_002f:  ldarg.2
    IL_0030:  ldarg.2
    IL_0031:  sub
    IL_0032:  ldarg.1
    IL_0033:  not
    IL_0034:  ldc.i4.1
    IL_0035:  conv.i8
    IL_0036:  add
    IL_0037:  div.un
    IL_0038:  nop
    IL_0039:  stloc.0
    IL_003a:  ldloc.0
    IL_003b:  ldc.i4.m1
    IL_003c:  conv.i8
    IL_003d:  bne.un.s   IL_006a

    IL_003f:  ldc.i4.0
    IL_0040:  conv.i8
    IL_0041:  stloc.1
    IL_0042:  ldarg.2
    IL_0043:  stloc.2
    IL_0044:  ldloc.2
    IL_0045:  call       void assembly::set_c(int64)
    IL_004a:  ldloc.2
    IL_004b:  ldarg.1
    IL_004c:  add
    IL_004d:  stloc.2
    IL_004e:  ldloc.1
    IL_004f:  ldc.i4.1
    IL_0050:  conv.i8
    IL_0051:  add
    IL_0052:  stloc.1
    IL_0053:  br.s       IL_0064

    IL_0055:  ldloc.2
    IL_0056:  call       void assembly::set_c(int64)
    IL_005b:  ldloc.2
    IL_005c:  ldarg.1
    IL_005d:  add
    IL_005e:  stloc.2
    IL_005f:  ldloc.1
    IL_0060:  ldc.i4.1
    IL_0061:  conv.i8
    IL_0062:  add
    IL_0063:  stloc.1
    IL_0064:  ldloc.1
    IL_0065:  ldc.i4.0
    IL_0066:  conv.i8
    IL_0067:  bgt.un.s   IL_0055

    IL_0069:  ret

    IL_006a:  ldarg.1
    IL_006b:  brtrue.s   IL_0079

    IL_006d:  ldarg.2
    IL_006e:  ldarg.1
    IL_006f:  ldarg.2
    IL_0070:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int64> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt64(int64,
                                                                                                                                                                           int64,
                                                                                                                                                                           int64)
    IL_0075:  pop
    IL_0076:  nop
    IL_0077:  br.s       IL_007a

    IL_0079:  nop
    IL_007a:  ldc.i4.0
    IL_007b:  conv.i8
    IL_007c:  ldarg.1
    IL_007d:  bge.s      IL_0093

    IL_007f:  ldarg.2
    IL_0080:  ldarg.2
    IL_0081:  bge.s      IL_0088

    IL_0083:  ldc.i4.0
    IL_0084:  conv.i8
    IL_0085:  nop
    IL_0086:  br.s       IL_00a9

    IL_0088:  ldarg.2
    IL_0089:  ldarg.2
    IL_008a:  sub
    IL_008b:  ldarg.1
    IL_008c:  div.un
    IL_008d:  ldc.i4.1
    IL_008e:  conv.i8
    IL_008f:  add.ovf.un
    IL_0090:  nop
    IL_0091:  br.s       IL_00a9

    IL_0093:  ldarg.2
    IL_0094:  ldarg.2
    IL_0095:  bge.s      IL_009c

    IL_0097:  ldc.i4.0
    IL_0098:  conv.i8
    IL_0099:  nop
    IL_009a:  br.s       IL_00a9

    IL_009c:  ldarg.2
    IL_009d:  ldarg.2
    IL_009e:  sub
    IL_009f:  ldarg.1
    IL_00a0:  not
    IL_00a1:  ldc.i4.1
    IL_00a2:  conv.i8
    IL_00a3:  add
    IL_00a4:  div.un
    IL_00a5:  ldc.i4.1
    IL_00a6:  conv.i8
    IL_00a7:  add.ovf.un
    IL_00a8:  nop
    IL_00a9:  stloc.1
    IL_00aa:  ldc.i4.0
    IL_00ab:  conv.i8
    IL_00ac:  stloc.3
    IL_00ad:  ldarg.2
    IL_00ae:  stloc.2
    IL_00af:  br.s       IL_00c0

    IL_00b1:  ldloc.2
    IL_00b2:  call       void assembly::set_c(int64)
    IL_00b7:  ldloc.2
    IL_00b8:  ldarg.1
    IL_00b9:  add
    IL_00ba:  stloc.2
    IL_00bb:  ldloc.3
    IL_00bc:  ldc.i4.1
    IL_00bd:  conv.i8
    IL_00be:  add
    IL_00bf:  stloc.3
    IL_00c0:  ldloc.3
    IL_00c1:  ldloc.1
    IL_00c2:  blt.un.s   IL_00b1

    IL_00c4:  ret
  } 

  .method public static void  f11(int64 start,
                                  int64 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  6
    .locals init (int32 V_0,
             int64 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.0
    IL_0003:  stloc.1
    IL_0004:  br.s       IL_0015

    IL_0006:  ldloc.1
    IL_0007:  call       void assembly::set_c(int64)
    IL_000c:  ldloc.1
    IL_000d:  ldc.i4.0
    IL_000e:  conv.i8
    IL_000f:  add
    IL_0010:  stloc.1
    IL_0011:  ldloc.0
    IL_0012:  ldc.i4.1
    IL_0013:  add
    IL_0014:  stloc.0
    IL_0015:  ldloc.0
    IL_0016:  ldarg.0
    IL_0017:  ldc.i4.0
    IL_0018:  conv.i8
    IL_0019:  ldarg.1
    IL_001a:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int64> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt64(int64,
                                                                                                                                                                           int64,
                                                                                                                                                                           int64)
    IL_001f:  pop
    IL_0020:  ldc.i4.m1
    IL_0021:  blt.un.s   IL_0006

    IL_0023:  ret
  } 

  .method public static void  f12() cil managed
  {
    
    .maxstack  6
    .locals init (int32 V_0,
             int64 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.1
    IL_0003:  conv.i8
    IL_0004:  stloc.1
    IL_0005:  br.s       IL_0016

    IL_0007:  ldloc.1
    IL_0008:  call       void assembly::set_c(int64)
    IL_000d:  ldloc.1
    IL_000e:  ldc.i4.0
    IL_000f:  conv.i8
    IL_0010:  add
    IL_0011:  stloc.1
    IL_0012:  ldloc.0
    IL_0013:  ldc.i4.1
    IL_0014:  add
    IL_0015:  stloc.0
    IL_0016:  ldloc.0
    IL_0017:  ldc.i4.1
    IL_0018:  conv.i8
    IL_0019:  ldc.i4.0
    IL_001a:  conv.i8
    IL_001b:  ldc.i4.s   10
    IL_001d:  conv.i8
    IL_001e:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int64> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt64(int64,
                                                                                                                                                                           int64,
                                                                                                                                                                           int64)
    IL_0023:  pop
    IL_0024:  ldc.i4.m1
    IL_0025:  blt.un.s   IL_0007

    IL_0027:  ret
  } 

  .method public static void  f13() cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             int64 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stloc.0
    IL_0003:  ldc.i4.s   10
    IL_0005:  conv.i8
    IL_0006:  stloc.1
    IL_0007:  br.s       IL_0019

    IL_0009:  ldloc.1
    IL_000a:  call       void assembly::set_c(int64)
    IL_000f:  ldloc.1
    IL_0010:  ldc.i4.m1
    IL_0011:  conv.i8
    IL_0012:  add
    IL_0013:  stloc.1
    IL_0014:  ldloc.0
    IL_0015:  ldc.i4.1
    IL_0016:  conv.i8
    IL_0017:  add
    IL_0018:  stloc.0
    IL_0019:  ldloc.0
    IL_001a:  ldc.i4.s   10
    IL_001c:  conv.i8
    IL_001d:  blt.un.s   IL_0009

    IL_001f:  ret
  } 

  .method public static void  f14() cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             int64 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stloc.0
    IL_0003:  ldc.i4.s   10
    IL_0005:  conv.i8
    IL_0006:  stloc.1
    IL_0007:  br.s       IL_001a

    IL_0009:  ldloc.1
    IL_000a:  call       void assembly::set_c(int64)
    IL_000f:  ldloc.1
    IL_0010:  ldc.i4.s   -2
    IL_0012:  conv.i8
    IL_0013:  add
    IL_0014:  stloc.1
    IL_0015:  ldloc.0
    IL_0016:  ldc.i4.1
    IL_0017:  conv.i8
    IL_0018:  add
    IL_0019:  stloc.0
    IL_001a:  ldloc.0
    IL_001b:  ldc.i4.5
    IL_001c:  conv.i8
    IL_001d:  blt.un.s   IL_0009

    IL_001f:  ret
  } 

  .property int64 c()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .set void assembly::set_c(int64)
    .get int64 assembly::get_c()
  } 
} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly
       extends [runtime]System.Object
{
  .field static assembly int64 c@1
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stsfld     int64 '<StartupCode$assembly>'.$assembly::c@1
    IL_0007:  ret
  } 

} 






