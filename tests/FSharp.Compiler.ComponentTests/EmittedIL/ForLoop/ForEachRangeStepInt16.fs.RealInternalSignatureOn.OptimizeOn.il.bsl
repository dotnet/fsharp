




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
  .field static assembly int16 c@1
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public specialname static int16 get_c() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int16 assembly::c@1
    IL_0005:  ret
  } 

  .method public specialname static void set_c(int16 'value') cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  stsfld     int16 assembly::c@1
    IL_0006:  ret
  } 

  .method public static void  f0() cil managed
  {
    
    .maxstack  4
    .locals init (int16 V_0,
             int16 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.s   10
    IL_0004:  stloc.1
    IL_0005:  br.s       IL_0015

    IL_0007:  ldloc.1
    IL_0008:  call       void assembly::set_c(int16)
    IL_000d:  ldloc.1
    IL_000e:  ldc.i4.1
    IL_000f:  add
    IL_0010:  stloc.1
    IL_0011:  ldloc.0
    IL_0012:  ldc.i4.1
    IL_0013:  add
    IL_0014:  stloc.0
    IL_0015:  ldloc.0
    IL_0016:  ldc.i4.0
    IL_0017:  blt.un.s   IL_0007

    IL_0019:  ret
  } 

  .method public static void  f00() cil managed
  {
    
    .maxstack  4
    .locals init (int16 V_0,
             int16 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.s   10
    IL_0004:  stloc.1
    IL_0005:  br.s       IL_0015

    IL_0007:  ldloc.1
    IL_0008:  call       void assembly::set_c(int16)
    IL_000d:  ldloc.1
    IL_000e:  ldc.i4.1
    IL_000f:  add
    IL_0010:  stloc.1
    IL_0011:  ldloc.0
    IL_0012:  ldc.i4.1
    IL_0013:  add
    IL_0014:  stloc.0
    IL_0015:  ldloc.0
    IL_0016:  ldc.i4.0
    IL_0017:  blt.un.s   IL_0007

    IL_0019:  ret
  } 

  .method public static void  f1() cil managed
  {
    
    .maxstack  4
    .locals init (uint32 V_0,
             int16 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.1
    IL_0003:  stloc.1
    IL_0004:  br.s       IL_0014

    IL_0006:  ldloc.1
    IL_0007:  call       void assembly::set_c(int16)
    IL_000c:  ldloc.1
    IL_000d:  ldc.i4.1
    IL_000e:  add
    IL_000f:  stloc.1
    IL_0010:  ldloc.0
    IL_0011:  ldc.i4.1
    IL_0012:  add
    IL_0013:  stloc.0
    IL_0014:  ldloc.0
    IL_0015:  ldc.i4.s   10
    IL_0017:  blt.un.s   IL_0006

    IL_0019:  ret
  } 

  .method public static void  f2(int16 start) cil managed
  {
    
    .maxstack  4
    .locals init (uint32 V_0,
             uint32 V_1,
             int16 V_2)
    IL_0000:  ldc.i4.s   10
    IL_0002:  ldarg.0
    IL_0003:  bge.s      IL_0009

    IL_0005:  ldc.i4.0
    IL_0006:  nop
    IL_0007:  br.s       IL_0011

    IL_0009:  ldc.i4.s   10
    IL_000b:  ldarg.0
    IL_000c:  sub
    IL_000d:  conv.i4
    IL_000e:  ldc.i4.1
    IL_000f:  add
    IL_0010:  nop
    IL_0011:  stloc.0
    IL_0012:  ldc.i4.0
    IL_0013:  stloc.1
    IL_0014:  ldarg.0
    IL_0015:  stloc.2
    IL_0016:  br.s       IL_0026

    IL_0018:  ldloc.2
    IL_0019:  call       void assembly::set_c(int16)
    IL_001e:  ldloc.2
    IL_001f:  ldc.i4.1
    IL_0020:  add
    IL_0021:  stloc.2
    IL_0022:  ldloc.1
    IL_0023:  ldc.i4.1
    IL_0024:  add
    IL_0025:  stloc.1
    IL_0026:  ldloc.1
    IL_0027:  ldloc.0
    IL_0028:  blt.un.s   IL_0018

    IL_002a:  ret
  } 

  .method public static void  f3(int16 finish) cil managed
  {
    
    .maxstack  4
    .locals init (uint32 V_0,
             uint32 V_1,
             int16 V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  bge.s      IL_0008

    IL_0004:  ldc.i4.0
    IL_0005:  nop
    IL_0006:  br.s       IL_000f

    IL_0008:  ldarg.0
    IL_0009:  ldc.i4.1
    IL_000a:  sub
    IL_000b:  conv.i4
    IL_000c:  ldc.i4.1
    IL_000d:  add
    IL_000e:  nop
    IL_000f:  stloc.0
    IL_0010:  ldc.i4.0
    IL_0011:  stloc.1
    IL_0012:  ldc.i4.1
    IL_0013:  stloc.2
    IL_0014:  br.s       IL_0024

    IL_0016:  ldloc.2
    IL_0017:  call       void assembly::set_c(int16)
    IL_001c:  ldloc.2
    IL_001d:  ldc.i4.1
    IL_001e:  add
    IL_001f:  stloc.2
    IL_0020:  ldloc.1
    IL_0021:  ldc.i4.1
    IL_0022:  add
    IL_0023:  stloc.1
    IL_0024:  ldloc.1
    IL_0025:  ldloc.0
    IL_0026:  blt.un.s   IL_0016

    IL_0028:  ret
  } 

  .method public static void  f4(int16 start,
                                 int16 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  4
    .locals init (uint32 V_0,
             uint32 V_1,
             int16 V_2)
    IL_0000:  ldarg.1
    IL_0001:  ldarg.0
    IL_0002:  bge.s      IL_0008

    IL_0004:  ldc.i4.0
    IL_0005:  nop
    IL_0006:  br.s       IL_000f

    IL_0008:  ldarg.1
    IL_0009:  ldarg.0
    IL_000a:  sub
    IL_000b:  conv.i4
    IL_000c:  ldc.i4.1
    IL_000d:  add
    IL_000e:  nop
    IL_000f:  stloc.0
    IL_0010:  ldc.i4.0
    IL_0011:  stloc.1
    IL_0012:  ldarg.0
    IL_0013:  stloc.2
    IL_0014:  br.s       IL_0024

    IL_0016:  ldloc.2
    IL_0017:  call       void assembly::set_c(int16)
    IL_001c:  ldloc.2
    IL_001d:  ldc.i4.1
    IL_001e:  add
    IL_001f:  stloc.2
    IL_0020:  ldloc.1
    IL_0021:  ldc.i4.1
    IL_0022:  add
    IL_0023:  stloc.1
    IL_0024:  ldloc.1
    IL_0025:  ldloc.0
    IL_0026:  blt.un.s   IL_0016

    IL_0028:  ret
  } 

  .method public static void  f5() cil managed
  {
    
    .maxstack  4
    .locals init (uint32 V_0,
             int16 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.1
    IL_0003:  stloc.1
    IL_0004:  br.s       IL_0014

    IL_0006:  ldloc.1
    IL_0007:  call       void assembly::set_c(int16)
    IL_000c:  ldloc.1
    IL_000d:  ldc.i4.1
    IL_000e:  add
    IL_000f:  stloc.1
    IL_0010:  ldloc.0
    IL_0011:  ldc.i4.1
    IL_0012:  add
    IL_0013:  stloc.0
    IL_0014:  ldloc.0
    IL_0015:  ldc.i4.s   10
    IL_0017:  blt.un.s   IL_0006

    IL_0019:  ret
  } 

  .method public static void  f6() cil managed
  {
    
    .maxstack  4
    .locals init (uint32 V_0,
             int16 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.1
    IL_0003:  stloc.1
    IL_0004:  br.s       IL_0014

    IL_0006:  ldloc.1
    IL_0007:  call       void assembly::set_c(int16)
    IL_000c:  ldloc.1
    IL_000d:  ldc.i4.2
    IL_000e:  add
    IL_000f:  stloc.1
    IL_0010:  ldloc.0
    IL_0011:  ldc.i4.1
    IL_0012:  add
    IL_0013:  stloc.0
    IL_0014:  ldloc.0
    IL_0015:  ldc.i4.5
    IL_0016:  blt.un.s   IL_0006

    IL_0018:  ret
  } 

  .method public static void  f7(int16 start) cil managed
  {
    
    .maxstack  4
    .locals init (uint32 V_0,
             uint32 V_1,
             int16 V_2)
    IL_0000:  ldc.i4.s   10
    IL_0002:  ldarg.0
    IL_0003:  bge.s      IL_0009

    IL_0005:  ldc.i4.0
    IL_0006:  nop
    IL_0007:  br.s       IL_0013

    IL_0009:  ldc.i4.s   10
    IL_000b:  ldarg.0
    IL_000c:  sub
    IL_000d:  ldc.i4.2
    IL_000e:  div.un
    IL_000f:  conv.i4
    IL_0010:  ldc.i4.1
    IL_0011:  add
    IL_0012:  nop
    IL_0013:  stloc.0
    IL_0014:  ldc.i4.0
    IL_0015:  stloc.1
    IL_0016:  ldarg.0
    IL_0017:  stloc.2
    IL_0018:  br.s       IL_0028

    IL_001a:  ldloc.2
    IL_001b:  call       void assembly::set_c(int16)
    IL_0020:  ldloc.2
    IL_0021:  ldc.i4.2
    IL_0022:  add
    IL_0023:  stloc.2
    IL_0024:  ldloc.1
    IL_0025:  ldc.i4.1
    IL_0026:  add
    IL_0027:  stloc.1
    IL_0028:  ldloc.1
    IL_0029:  ldloc.0
    IL_002a:  blt.un.s   IL_001a

    IL_002c:  ret
  } 

  .method public static void  f8(int16 step) cil managed
  {
    
    .maxstack  5
    .locals init (uint32 V_0,
             uint32 V_1,
             int16 V_2)
    IL_0000:  ldarg.0
    IL_0001:  brtrue.s   IL_0010

    IL_0003:  ldc.i4.1
    IL_0004:  ldarg.0
    IL_0005:  ldc.i4.s   10
    IL_0007:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int16> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt16(int16,
                                                                                                                                                                           int16,
                                                                                                                                                                           int16)
    IL_000c:  pop
    IL_000d:  nop
    IL_000e:  br.s       IL_0011

    IL_0010:  nop
    IL_0011:  ldc.i4.0
    IL_0012:  ldarg.0
    IL_0013:  bge.s      IL_001f

    IL_0015:  ldc.i4.s   9
    IL_0017:  ldarg.0
    IL_0018:  div.un
    IL_0019:  conv.i4
    IL_001a:  ldc.i4.1
    IL_001b:  add
    IL_001c:  nop
    IL_001d:  br.s       IL_0021

    IL_001f:  ldc.i4.0
    IL_0020:  nop
    IL_0021:  stloc.0
    IL_0022:  ldc.i4.0
    IL_0023:  stloc.1
    IL_0024:  ldc.i4.1
    IL_0025:  stloc.2
    IL_0026:  br.s       IL_0036

    IL_0028:  ldloc.2
    IL_0029:  call       void assembly::set_c(int16)
    IL_002e:  ldloc.2
    IL_002f:  ldarg.0
    IL_0030:  add
    IL_0031:  stloc.2
    IL_0032:  ldloc.1
    IL_0033:  ldc.i4.1
    IL_0034:  add
    IL_0035:  stloc.1
    IL_0036:  ldloc.1
    IL_0037:  ldloc.0
    IL_0038:  blt.un.s   IL_0028

    IL_003a:  ret
  } 

  .method public static void  f9(int16 finish) cil managed
  {
    
    .maxstack  4
    .locals init (uint32 V_0,
             uint32 V_1,
             int16 V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  bge.s      IL_0008

    IL_0004:  ldc.i4.0
    IL_0005:  nop
    IL_0006:  br.s       IL_0011

    IL_0008:  ldarg.0
    IL_0009:  ldc.i4.1
    IL_000a:  sub
    IL_000b:  ldc.i4.2
    IL_000c:  div.un
    IL_000d:  conv.i4
    IL_000e:  ldc.i4.1
    IL_000f:  add
    IL_0010:  nop
    IL_0011:  stloc.0
    IL_0012:  ldc.i4.0
    IL_0013:  stloc.1
    IL_0014:  ldc.i4.1
    IL_0015:  stloc.2
    IL_0016:  br.s       IL_0026

    IL_0018:  ldloc.2
    IL_0019:  call       void assembly::set_c(int16)
    IL_001e:  ldloc.2
    IL_001f:  ldc.i4.2
    IL_0020:  add
    IL_0021:  stloc.2
    IL_0022:  ldloc.1
    IL_0023:  ldc.i4.1
    IL_0024:  add
    IL_0025:  stloc.1
    IL_0026:  ldloc.1
    IL_0027:  ldloc.0
    IL_0028:  blt.un.s   IL_0018

    IL_002a:  ret
  } 

  .method public static void  f10(int16 start,
                                  int16 step,
                                  int16 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  5
    .locals init (uint32 V_0,
             uint32 V_1,
             int16 V_2)
    IL_0000:  ldarg.1
    IL_0001:  brtrue.s   IL_000f

    IL_0003:  ldarg.2
    IL_0004:  ldarg.1
    IL_0005:  ldarg.2
    IL_0006:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int16> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt16(int16,
                                                                                                                                                                           int16,
                                                                                                                                                                           int16)
    IL_000b:  pop
    IL_000c:  nop
    IL_000d:  br.s       IL_0010

    IL_000f:  nop
    IL_0010:  ldc.i4.0
    IL_0011:  ldarg.1
    IL_0012:  bge.s      IL_0027

    IL_0014:  ldarg.2
    IL_0015:  ldarg.2
    IL_0016:  bge.s      IL_001c

    IL_0018:  ldc.i4.0
    IL_0019:  nop
    IL_001a:  br.s       IL_003b

    IL_001c:  ldarg.2
    IL_001d:  ldarg.2
    IL_001e:  sub
    IL_001f:  ldarg.1
    IL_0020:  div.un
    IL_0021:  conv.i4
    IL_0022:  ldc.i4.1
    IL_0023:  add
    IL_0024:  nop
    IL_0025:  br.s       IL_003b

    IL_0027:  ldarg.2
    IL_0028:  ldarg.2
    IL_0029:  bge.s      IL_002f

    IL_002b:  ldc.i4.0
    IL_002c:  nop
    IL_002d:  br.s       IL_003b

    IL_002f:  ldarg.2
    IL_0030:  ldarg.2
    IL_0031:  sub
    IL_0032:  ldarg.1
    IL_0033:  not
    IL_0034:  ldc.i4.1
    IL_0035:  add
    IL_0036:  div.un
    IL_0037:  conv.i4
    IL_0038:  ldc.i4.1
    IL_0039:  add
    IL_003a:  nop
    IL_003b:  stloc.0
    IL_003c:  ldc.i4.0
    IL_003d:  stloc.1
    IL_003e:  ldarg.2
    IL_003f:  stloc.2
    IL_0040:  br.s       IL_0050

    IL_0042:  ldloc.2
    IL_0043:  call       void assembly::set_c(int16)
    IL_0048:  ldloc.2
    IL_0049:  ldarg.1
    IL_004a:  add
    IL_004b:  stloc.2
    IL_004c:  ldloc.1
    IL_004d:  ldc.i4.1
    IL_004e:  add
    IL_004f:  stloc.1
    IL_0050:  ldloc.1
    IL_0051:  ldloc.0
    IL_0052:  blt.un.s   IL_0042

    IL_0054:  ret
  } 

  .method public static void  f11(int16 start,
                                  int16 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (int16 V_0,
             int16 V_1,
             int16 V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  ldarg.1
    IL_0003:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int16> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt16(int16,
                                                                                                                                                                           int16,
                                                                                                                                                                           int16)
    IL_0008:  pop
    IL_0009:  ldc.i4.0
    IL_000a:  stloc.0
    IL_000b:  ldc.i4.0
    IL_000c:  stloc.1
    IL_000d:  ldarg.0
    IL_000e:  stloc.2
    IL_000f:  br.s       IL_001f

    IL_0011:  ldloc.2
    IL_0012:  call       void assembly::set_c(int16)
    IL_0017:  ldloc.2
    IL_0018:  ldc.i4.0
    IL_0019:  add
    IL_001a:  stloc.2
    IL_001b:  ldloc.1
    IL_001c:  ldc.i4.1
    IL_001d:  add
    IL_001e:  stloc.1
    IL_001f:  ldloc.1
    IL_0020:  ldloc.0
    IL_0021:  blt.un.s   IL_0011

    IL_0023:  ret
  } 

  .method public static void  f12() cil managed
  {
    
    .maxstack  5
    .locals init (int16 V_0,
             int16 V_1,
             int16 V_2)
    IL_0000:  ldc.i4.1
    IL_0001:  ldc.i4.0
    IL_0002:  ldc.i4.s   10
    IL_0004:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int16> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt16(int16,
                                                                                                                                                                           int16,
                                                                                                                                                                           int16)
    IL_0009:  pop
    IL_000a:  ldc.i4.0
    IL_000b:  stloc.0
    IL_000c:  ldc.i4.0
    IL_000d:  stloc.1
    IL_000e:  ldc.i4.1
    IL_000f:  stloc.2
    IL_0010:  br.s       IL_0020

    IL_0012:  ldloc.2
    IL_0013:  call       void assembly::set_c(int16)
    IL_0018:  ldloc.2
    IL_0019:  ldc.i4.0
    IL_001a:  add
    IL_001b:  stloc.2
    IL_001c:  ldloc.1
    IL_001d:  ldc.i4.1
    IL_001e:  add
    IL_001f:  stloc.1
    IL_0020:  ldloc.1
    IL_0021:  ldloc.0
    IL_0022:  blt.un.s   IL_0012

    IL_0024:  ret
  } 

  .method public static void  f13() cil managed
  {
    
    .maxstack  4
    .locals init (uint32 V_0,
             int16 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.s   10
    IL_0004:  stloc.1
    IL_0005:  br.s       IL_0015

    IL_0007:  ldloc.1
    IL_0008:  call       void assembly::set_c(int16)
    IL_000d:  ldloc.1
    IL_000e:  ldc.i4.m1
    IL_000f:  add
    IL_0010:  stloc.1
    IL_0011:  ldloc.0
    IL_0012:  ldc.i4.1
    IL_0013:  add
    IL_0014:  stloc.0
    IL_0015:  ldloc.0
    IL_0016:  ldc.i4.s   10
    IL_0018:  blt.un.s   IL_0007

    IL_001a:  ret
  } 

  .method public static void  f14() cil managed
  {
    
    .maxstack  4
    .locals init (uint32 V_0,
             int16 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.s   10
    IL_0004:  stloc.1
    IL_0005:  br.s       IL_0016

    IL_0007:  ldloc.1
    IL_0008:  call       void assembly::set_c(int16)
    IL_000d:  ldloc.1
    IL_000e:  ldc.i4.s   -2
    IL_0010:  add
    IL_0011:  stloc.1
    IL_0012:  ldloc.0
    IL_0013:  ldc.i4.1
    IL_0014:  add
    IL_0015:  stloc.0
    IL_0016:  ldloc.0
    IL_0017:  ldc.i4.5
    IL_0018:  blt.un.s   IL_0007

    IL_001a:  ret
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
    IL_0001:  stsfld     int16 assembly::c@1
    IL_0006:  ret
  } 

  .property int16 c()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .set void assembly::set_c(int16)
    .get int16 assembly::get_c()
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






