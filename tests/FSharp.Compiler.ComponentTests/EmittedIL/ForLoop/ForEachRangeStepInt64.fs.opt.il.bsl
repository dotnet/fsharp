




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
    IL_0009:  br.s       IL_0016

    IL_000b:  ldc.i4.s   10
    IL_000d:  conv.i8
    IL_000e:  conv.i8
    IL_000f:  ldarg.0
    IL_0010:  conv.i8
    IL_0011:  sub
    IL_0012:  ldc.i4.1
    IL_0013:  conv.i8
    IL_0014:  add.ovf.un
    IL_0015:  nop
    IL_0016:  stloc.0
    IL_0017:  ldc.i4.0
    IL_0018:  conv.i8
    IL_0019:  stloc.1
    IL_001a:  ldarg.0
    IL_001b:  stloc.2
    IL_001c:  br.s       IL_002e

    IL_001e:  ldloc.2
    IL_001f:  call       void assembly::set_c(int64)
    IL_0024:  ldloc.2
    IL_0025:  ldc.i4.1
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
    IL_0008:  br.s       IL_0014

    IL_000a:  ldarg.0
    IL_000b:  conv.i8
    IL_000c:  ldc.i4.1
    IL_000d:  conv.i8
    IL_000e:  conv.i8
    IL_000f:  sub
    IL_0010:  ldc.i4.1
    IL_0011:  conv.i8
    IL_0012:  add.ovf.un
    IL_0013:  nop
    IL_0014:  stloc.0
    IL_0015:  ldc.i4.0
    IL_0016:  conv.i8
    IL_0017:  stloc.1
    IL_0018:  ldc.i4.1
    IL_0019:  conv.i8
    IL_001a:  stloc.2
    IL_001b:  br.s       IL_002d

    IL_001d:  ldloc.2
    IL_001e:  call       void assembly::set_c(int64)
    IL_0023:  ldloc.2
    IL_0024:  ldc.i4.1
    IL_0025:  conv.i8
    IL_0026:  add
    IL_0027:  stloc.2
    IL_0028:  ldloc.1
    IL_0029:  ldc.i4.1
    IL_002a:  conv.i8
    IL_002b:  add
    IL_002c:  stloc.1
    IL_002d:  ldloc.1
    IL_002e:  ldloc.0
    IL_002f:  blt.un.s   IL_001d

    IL_0031:  ret
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
    IL_0007:  br.s       IL_000f

    IL_0009:  ldarg.1
    IL_000a:  conv.i8
    IL_000b:  ldarg.0
    IL_000c:  conv.i8
    IL_000d:  sub
    IL_000e:  nop
    IL_000f:  stloc.0
    IL_0010:  ldloc.0
    IL_0011:  ldc.i4.m1
    IL_0012:  conv.i8
    IL_0013:  bne.un.s   IL_0042

    IL_0015:  ldc.i4.0
    IL_0016:  conv.i8
    IL_0017:  stloc.1
    IL_0018:  ldarg.0
    IL_0019:  stloc.2
    IL_001a:  ldloc.2
    IL_001b:  call       void assembly::set_c(int64)
    IL_0020:  ldloc.2
    IL_0021:  ldc.i4.1
    IL_0022:  conv.i8
    IL_0023:  add
    IL_0024:  stloc.2
    IL_0025:  ldloc.1
    IL_0026:  ldc.i4.1
    IL_0027:  conv.i8
    IL_0028:  add
    IL_0029:  stloc.1
    IL_002a:  br.s       IL_003c

    IL_002c:  ldloc.2
    IL_002d:  call       void assembly::set_c(int64)
    IL_0032:  ldloc.2
    IL_0033:  ldc.i4.1
    IL_0034:  conv.i8
    IL_0035:  add
    IL_0036:  stloc.2
    IL_0037:  ldloc.1
    IL_0038:  ldc.i4.1
    IL_0039:  conv.i8
    IL_003a:  add
    IL_003b:  stloc.1
    IL_003c:  ldloc.1
    IL_003d:  ldc.i4.0
    IL_003e:  conv.i8
    IL_003f:  bgt.un.s   IL_002c

    IL_0041:  ret

    IL_0042:  ldloc.0
    IL_0043:  ldc.i4.1
    IL_0044:  conv.i8
    IL_0045:  add.ovf.un
    IL_0046:  stloc.1
    IL_0047:  ldc.i4.0
    IL_0048:  conv.i8
    IL_0049:  stloc.3
    IL_004a:  ldarg.0
    IL_004b:  stloc.2
    IL_004c:  br.s       IL_005e

    IL_004e:  ldloc.2
    IL_004f:  call       void assembly::set_c(int64)
    IL_0054:  ldloc.2
    IL_0055:  ldc.i4.1
    IL_0056:  conv.i8
    IL_0057:  add
    IL_0058:  stloc.2
    IL_0059:  ldloc.3
    IL_005a:  ldc.i4.1
    IL_005b:  conv.i8
    IL_005c:  add
    IL_005d:  stloc.3
    IL_005e:  ldloc.3
    IL_005f:  ldloc.1
    IL_0060:  blt.un.s   IL_004e

    IL_0062:  ret
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
    IL_0009:  br.s       IL_001a

    IL_000b:  ldc.i4.s   10
    IL_000d:  conv.i8
    IL_000e:  conv.i8
    IL_000f:  ldarg.0
    IL_0010:  conv.i8
    IL_0011:  sub
    IL_0012:  ldc.i4.2
    IL_0013:  conv.i8
    IL_0014:  conv.i8
    IL_0015:  div.un
    IL_0016:  ldc.i4.1
    IL_0017:  conv.i8
    IL_0018:  add.ovf.un
    IL_0019:  nop
    IL_001a:  stloc.0
    IL_001b:  ldc.i4.0
    IL_001c:  conv.i8
    IL_001d:  stloc.1
    IL_001e:  ldarg.0
    IL_001f:  stloc.2
    IL_0020:  br.s       IL_0032

    IL_0022:  ldloc.2
    IL_0023:  call       void assembly::set_c(int64)
    IL_0028:  ldloc.2
    IL_0029:  ldc.i4.2
    IL_002a:  conv.i8
    IL_002b:  add
    IL_002c:  stloc.2
    IL_002d:  ldloc.1
    IL_002e:  ldc.i4.1
    IL_002f:  conv.i8
    IL_0030:  add
    IL_0031:  stloc.1
    IL_0032:  ldloc.1
    IL_0033:  ldloc.0
    IL_0034:  blt.un.s   IL_0022

    IL_0036:  ret
  } 

  .method public static void  f8(int64 step) cil managed
  {
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             int64 V_2,
             uint64 V_3)
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
    IL_0016:  bge.s      IL_0026

    IL_0018:  ldc.i4.s   10
    IL_001a:  conv.i8
    IL_001b:  conv.i8
    IL_001c:  ldc.i4.1
    IL_001d:  conv.i8
    IL_001e:  conv.i8
    IL_001f:  sub
    IL_0020:  ldarg.0
    IL_0021:  conv.i8
    IL_0022:  div.un
    IL_0023:  nop
    IL_0024:  br.s       IL_0029

    IL_0026:  ldc.i4.0
    IL_0027:  conv.i8
    IL_0028:  nop
    IL_0029:  stloc.0
    IL_002a:  ldloc.0
    IL_002b:  ldc.i4.m1
    IL_002c:  conv.i8
    IL_002d:  bne.un.s   IL_005b

    IL_002f:  ldc.i4.0
    IL_0030:  conv.i8
    IL_0031:  stloc.1
    IL_0032:  ldc.i4.1
    IL_0033:  conv.i8
    IL_0034:  stloc.2
    IL_0035:  ldloc.2
    IL_0036:  call       void assembly::set_c(int64)
    IL_003b:  ldloc.2
    IL_003c:  ldarg.0
    IL_003d:  add
    IL_003e:  stloc.2
    IL_003f:  ldloc.1
    IL_0040:  ldc.i4.1
    IL_0041:  conv.i8
    IL_0042:  add
    IL_0043:  stloc.1
    IL_0044:  br.s       IL_0055

    IL_0046:  ldloc.2
    IL_0047:  call       void assembly::set_c(int64)
    IL_004c:  ldloc.2
    IL_004d:  ldarg.0
    IL_004e:  add
    IL_004f:  stloc.2
    IL_0050:  ldloc.1
    IL_0051:  ldc.i4.1
    IL_0052:  conv.i8
    IL_0053:  add
    IL_0054:  stloc.1
    IL_0055:  ldloc.1
    IL_0056:  ldc.i4.0
    IL_0057:  conv.i8
    IL_0058:  bgt.un.s   IL_0046

    IL_005a:  ret

    IL_005b:  ldloc.0
    IL_005c:  ldc.i4.1
    IL_005d:  conv.i8
    IL_005e:  add.ovf.un
    IL_005f:  stloc.1
    IL_0060:  ldc.i4.0
    IL_0061:  conv.i8
    IL_0062:  stloc.3
    IL_0063:  ldc.i4.1
    IL_0064:  conv.i8
    IL_0065:  stloc.2
    IL_0066:  br.s       IL_0077

    IL_0068:  ldloc.2
    IL_0069:  call       void assembly::set_c(int64)
    IL_006e:  ldloc.2
    IL_006f:  ldarg.0
    IL_0070:  add
    IL_0071:  stloc.2
    IL_0072:  ldloc.3
    IL_0073:  ldc.i4.1
    IL_0074:  conv.i8
    IL_0075:  add
    IL_0076:  stloc.3
    IL_0077:  ldloc.3
    IL_0078:  ldloc.1
    IL_0079:  blt.un.s   IL_0068

    IL_007b:  ret
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
    IL_0008:  br.s       IL_0018

    IL_000a:  ldarg.0
    IL_000b:  conv.i8
    IL_000c:  ldc.i4.1
    IL_000d:  conv.i8
    IL_000e:  conv.i8
    IL_000f:  sub
    IL_0010:  ldc.i4.2
    IL_0011:  conv.i8
    IL_0012:  conv.i8
    IL_0013:  div.un
    IL_0014:  ldc.i4.1
    IL_0015:  conv.i8
    IL_0016:  add.ovf.un
    IL_0017:  nop
    IL_0018:  stloc.0
    IL_0019:  ldc.i4.0
    IL_001a:  conv.i8
    IL_001b:  stloc.1
    IL_001c:  ldc.i4.1
    IL_001d:  conv.i8
    IL_001e:  stloc.2
    IL_001f:  br.s       IL_0031

    IL_0021:  ldloc.2
    IL_0022:  call       void assembly::set_c(int64)
    IL_0027:  ldloc.2
    IL_0028:  ldc.i4.2
    IL_0029:  conv.i8
    IL_002a:  add
    IL_002b:  stloc.2
    IL_002c:  ldloc.1
    IL_002d:  ldc.i4.1
    IL_002e:  conv.i8
    IL_002f:  add
    IL_0030:  stloc.1
    IL_0031:  ldloc.1
    IL_0032:  ldloc.0
    IL_0033:  blt.un.s   IL_0021

    IL_0035:  ret
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
    IL_0013:  bge.s      IL_0029

    IL_0015:  ldarg.2
    IL_0016:  ldarg.2
    IL_0017:  bge.s      IL_001e

    IL_0019:  ldc.i4.0
    IL_001a:  conv.i8
    IL_001b:  nop
    IL_001c:  br.s       IL_0040

    IL_001e:  ldarg.2
    IL_001f:  conv.i8
    IL_0020:  ldarg.2
    IL_0021:  conv.i8
    IL_0022:  sub
    IL_0023:  ldarg.1
    IL_0024:  conv.i8
    IL_0025:  div.un
    IL_0026:  nop
    IL_0027:  br.s       IL_0040

    IL_0029:  ldarg.2
    IL_002a:  ldarg.2
    IL_002b:  bge.s      IL_0032

    IL_002d:  ldc.i4.0
    IL_002e:  conv.i8
    IL_002f:  nop
    IL_0030:  br.s       IL_0040

    IL_0032:  ldarg.2
    IL_0033:  conv.i8
    IL_0034:  ldarg.2
    IL_0035:  conv.i8
    IL_0036:  sub
    IL_0037:  ldarg.1
    IL_0038:  not
    IL_0039:  conv.i8
    IL_003a:  ldc.i4.1
    IL_003b:  conv.i8
    IL_003c:  add
    IL_003d:  conv.i8
    IL_003e:  div.un
    IL_003f:  nop
    IL_0040:  stloc.0
    IL_0041:  ldloc.0
    IL_0042:  ldc.i4.m1
    IL_0043:  conv.i8
    IL_0044:  bne.un.s   IL_0071

    IL_0046:  ldc.i4.0
    IL_0047:  conv.i8
    IL_0048:  stloc.1
    IL_0049:  ldarg.2
    IL_004a:  stloc.2
    IL_004b:  ldloc.2
    IL_004c:  call       void assembly::set_c(int64)
    IL_0051:  ldloc.2
    IL_0052:  ldarg.1
    IL_0053:  add
    IL_0054:  stloc.2
    IL_0055:  ldloc.1
    IL_0056:  ldc.i4.1
    IL_0057:  conv.i8
    IL_0058:  add
    IL_0059:  stloc.1
    IL_005a:  br.s       IL_006b

    IL_005c:  ldloc.2
    IL_005d:  call       void assembly::set_c(int64)
    IL_0062:  ldloc.2
    IL_0063:  ldarg.1
    IL_0064:  add
    IL_0065:  stloc.2
    IL_0066:  ldloc.1
    IL_0067:  ldc.i4.1
    IL_0068:  conv.i8
    IL_0069:  add
    IL_006a:  stloc.1
    IL_006b:  ldloc.1
    IL_006c:  ldc.i4.0
    IL_006d:  conv.i8
    IL_006e:  bgt.un.s   IL_005c

    IL_0070:  ret

    IL_0071:  ldloc.0
    IL_0072:  ldc.i4.1
    IL_0073:  conv.i8
    IL_0074:  add.ovf.un
    IL_0075:  stloc.1
    IL_0076:  ldc.i4.0
    IL_0077:  conv.i8
    IL_0078:  stloc.3
    IL_0079:  ldarg.2
    IL_007a:  stloc.2
    IL_007b:  br.s       IL_008c

    IL_007d:  ldloc.2
    IL_007e:  call       void assembly::set_c(int64)
    IL_0083:  ldloc.2
    IL_0084:  ldarg.1
    IL_0085:  add
    IL_0086:  stloc.2
    IL_0087:  ldloc.3
    IL_0088:  ldc.i4.1
    IL_0089:  conv.i8
    IL_008a:  add
    IL_008b:  stloc.3
    IL_008c:  ldloc.3
    IL_008d:  ldloc.1
    IL_008e:  blt.un.s   IL_007d

    IL_0090:  ret
  } 

  .method public static void  f11(int64 start,
                                  int64 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  conv.i8
    IL_0003:  ldarg.1
    IL_0004:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int64> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt64(int64,
                                                                                                                                                                           int64,
                                                                                                                                                                           int64)
    IL_0009:  pop
    IL_000a:  ret
  } 

  .method public static void  f12() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.1
    IL_0001:  conv.i8
    IL_0002:  ldc.i4.0
    IL_0003:  conv.i8
    IL_0004:  ldc.i4.s   10
    IL_0006:  conv.i8
    IL_0007:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int64> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt64(int64,
                                                                                                                                                                           int64,
                                                                                                                                                                           int64)
    IL_000c:  pop
    IL_000d:  ret
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






