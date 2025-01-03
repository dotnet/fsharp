




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
  .field static assembly int64 c@3
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public specialname static int64 get_c() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int64 assembly::c@3
    IL_0005:  ret
  } 

  .method public specialname static void set_c(int64 'value') cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  stsfld     int64 assembly::c@3
    IL_0006:  ret
  } 

  .method public static void  f1() cil managed
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

  .method public static void  f2() cil managed
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

  .method public static void  f3() cil managed
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

  .method public static void  f4(int64 start) cil managed
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

  .method public static void  f5(int64 step) cil managed
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

  .method public static void  f6(int64 finish) cil managed
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

  .method public static void  f7(int64 start,
                                 int64 step,
                                 int64 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  5
    .locals init (uint64 V_0,
             bool V_1,
             uint64 V_2,
             int64 V_3,
             uint64 V_4)
    IL_0000:  ldarg.1
    IL_0001:  brtrue.s   IL_000f

    IL_0003:  ldarg.0
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
    IL_0016:  ldarg.0
    IL_0017:  bge.s      IL_001e

    IL_0019:  ldc.i4.0
    IL_001a:  conv.i8
    IL_001b:  nop
    IL_001c:  br.s       IL_0039

    IL_001e:  ldarg.2
    IL_001f:  ldarg.0
    IL_0020:  sub
    IL_0021:  ldarg.1
    IL_0022:  div.un
    IL_0023:  nop
    IL_0024:  br.s       IL_0039

    IL_0026:  ldarg.0
    IL_0027:  ldarg.2
    IL_0028:  bge.s      IL_002f

    IL_002a:  ldc.i4.0
    IL_002b:  conv.i8
    IL_002c:  nop
    IL_002d:  br.s       IL_0039

    IL_002f:  ldarg.0
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
    IL_003d:  bne.un.s   IL_0061

    IL_003f:  ldc.i4.1
    IL_0040:  stloc.1
    IL_0041:  ldc.i4.0
    IL_0042:  conv.i8
    IL_0043:  stloc.2
    IL_0044:  ldarg.0
    IL_0045:  stloc.3
    IL_0046:  br.s       IL_005d

    IL_0048:  ldloc.3
    IL_0049:  call       void assembly::set_c(int64)
    IL_004e:  ldloc.3
    IL_004f:  ldarg.1
    IL_0050:  add
    IL_0051:  stloc.3
    IL_0052:  ldloc.2
    IL_0053:  ldc.i4.1
    IL_0054:  conv.i8
    IL_0055:  add
    IL_0056:  stloc.2
    IL_0057:  ldloc.2
    IL_0058:  ldc.i4.0
    IL_0059:  conv.i8
    IL_005a:  cgt.un
    IL_005c:  stloc.1
    IL_005d:  ldloc.1
    IL_005e:  brtrue.s   IL_0048

    IL_0060:  ret

    IL_0061:  ldc.i4.0
    IL_0062:  conv.i8
    IL_0063:  ldarg.1
    IL_0064:  bge.s      IL_007a

    IL_0066:  ldarg.2
    IL_0067:  ldarg.0
    IL_0068:  bge.s      IL_006f

    IL_006a:  ldc.i4.0
    IL_006b:  conv.i8
    IL_006c:  nop
    IL_006d:  br.s       IL_0090

    IL_006f:  ldarg.2
    IL_0070:  ldarg.0
    IL_0071:  sub
    IL_0072:  ldarg.1
    IL_0073:  div.un
    IL_0074:  ldc.i4.1
    IL_0075:  conv.i8
    IL_0076:  add.ovf.un
    IL_0077:  nop
    IL_0078:  br.s       IL_0090

    IL_007a:  ldarg.0
    IL_007b:  ldarg.2
    IL_007c:  bge.s      IL_0083

    IL_007e:  ldc.i4.0
    IL_007f:  conv.i8
    IL_0080:  nop
    IL_0081:  br.s       IL_0090

    IL_0083:  ldarg.0
    IL_0084:  ldarg.2
    IL_0085:  sub
    IL_0086:  ldarg.1
    IL_0087:  not
    IL_0088:  ldc.i4.1
    IL_0089:  conv.i8
    IL_008a:  add
    IL_008b:  div.un
    IL_008c:  ldc.i4.1
    IL_008d:  conv.i8
    IL_008e:  add.ovf.un
    IL_008f:  nop
    IL_0090:  stloc.2
    IL_0091:  ldc.i4.0
    IL_0092:  conv.i8
    IL_0093:  stloc.s    V_4
    IL_0095:  ldarg.0
    IL_0096:  stloc.3
    IL_0097:  br.s       IL_00aa

    IL_0099:  ldloc.3
    IL_009a:  call       void assembly::set_c(int64)
    IL_009f:  ldloc.3
    IL_00a0:  ldarg.1
    IL_00a1:  add
    IL_00a2:  stloc.3
    IL_00a3:  ldloc.s    V_4
    IL_00a5:  ldc.i4.1
    IL_00a6:  conv.i8
    IL_00a7:  add
    IL_00a8:  stloc.s    V_4
    IL_00aa:  ldloc.s    V_4
    IL_00ac:  ldloc.2
    IL_00ad:  blt.un.s   IL_0099

    IL_00af:  ret
  } 

  .method public static void  f8(int64 start,
                                 int64 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (int64 V_0,
             int64 V_1,
             int64 V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  conv.i8
    IL_0003:  ldarg.1
    IL_0004:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int64> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt64(int64,
                                                                                                                                                                           int64,
                                                                                                                                                                           int64)
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
    IL_0015:  call       void assembly::set_c(int64)
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

  .method public static void  f9() cil managed
  {
    
    .maxstack  5
    .locals init (int64 V_0,
             int64 V_1,
             int64 V_2)
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
    IL_0019:  call       void assembly::set_c(int64)
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

  .method public static void  f10() cil managed
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

  .method public static void  f11() cil managed
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
    IL_0002:  stsfld     int64 assembly::c@3
    IL_0007:  ret
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






