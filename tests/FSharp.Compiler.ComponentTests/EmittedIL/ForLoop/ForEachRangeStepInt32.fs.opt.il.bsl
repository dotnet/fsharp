




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
  .method public specialname static int32 get_c() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32 '<StartupCode$assembly>'.$assembly::c@1
    IL_0005:  ret
  } 

  .method public specialname static void set_c(int32 'value') cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$assembly::c@1
    IL_0006:  ret
  } 

  .method public static void  f0() cil managed
  {
    
    .maxstack  4
    .locals init (int32 V_0)
    IL_0000:  ldc.i4.s   10
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_000f

    IL_0005:  ldloc.0
    IL_0006:  call       void assembly::set_c(int32)
    IL_000b:  ldloc.0
    IL_000c:  ldc.i4.1
    IL_000d:  add
    IL_000e:  stloc.0
    IL_000f:  ldloc.0
    IL_0010:  ldc.i4.2
    IL_0011:  blt.s      IL_0005

    IL_0013:  ret
  } 

  .method public static void  f00() cil managed
  {
    
    .maxstack  4
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.s   10
    IL_0004:  stloc.1
    IL_0005:  br.s       IL_0015

    IL_0007:  ldloc.1
    IL_0008:  call       void assembly::set_c(int32)
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
    .locals init (int32 V_0)
    IL_0000:  ldc.i4.1
    IL_0001:  stloc.0
    IL_0002:  br.s       IL_000e

    IL_0004:  ldloc.0
    IL_0005:  call       void assembly::set_c(int32)
    IL_000a:  ldloc.0
    IL_000b:  ldc.i4.1
    IL_000c:  add
    IL_000d:  stloc.0
    IL_000e:  ldloc.0
    IL_000f:  ldc.i4.s   11
    IL_0011:  blt.s      IL_0004

    IL_0013:  ret
  } 

  .method public static void  f2(int32 start) cil managed
  {
    
    .maxstack  4
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  br.s       IL_000e

    IL_0004:  ldloc.0
    IL_0005:  call       void assembly::set_c(int32)
    IL_000a:  ldloc.0
    IL_000b:  ldc.i4.1
    IL_000c:  add
    IL_000d:  stloc.0
    IL_000e:  ldloc.0
    IL_000f:  ldc.i4.s   11
    IL_0011:  blt.s      IL_0004

    IL_0013:  ret
  } 

  .method public static void  f3(int32 finish) cil managed
  {
    
    .maxstack  5
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  ldc.i4.1
    IL_0001:  stloc.1
    IL_0002:  ldarg.0
    IL_0003:  stloc.0
    IL_0004:  ldloc.0
    IL_0005:  ldloc.1
    IL_0006:  blt.s      IL_0018

    IL_0008:  ldloc.1
    IL_0009:  call       void assembly::set_c(int32)
    IL_000e:  ldloc.1
    IL_000f:  ldc.i4.1
    IL_0010:  add
    IL_0011:  stloc.1
    IL_0012:  ldloc.1
    IL_0013:  ldloc.0
    IL_0014:  ldc.i4.1
    IL_0015:  add
    IL_0016:  bne.un.s   IL_0008

    IL_0018:  ret
  } 

  .method public static void  f4(int32 start,
                                 int32 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  ldarg.0
    IL_0001:  stloc.1
    IL_0002:  ldarg.1
    IL_0003:  stloc.0
    IL_0004:  ldloc.0
    IL_0005:  ldloc.1
    IL_0006:  blt.s      IL_0018

    IL_0008:  ldloc.1
    IL_0009:  call       void assembly::set_c(int32)
    IL_000e:  ldloc.1
    IL_000f:  ldc.i4.1
    IL_0010:  add
    IL_0011:  stloc.1
    IL_0012:  ldloc.1
    IL_0013:  ldloc.0
    IL_0014:  ldc.i4.1
    IL_0015:  add
    IL_0016:  bne.un.s   IL_0008

    IL_0018:  ret
  } 

  .method public static void  f5() cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             int32 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stloc.0
    IL_0003:  ldc.i4.1
    IL_0004:  stloc.1
    IL_0005:  br.s       IL_0016

    IL_0007:  ldloc.1
    IL_0008:  call       void assembly::set_c(int32)
    IL_000d:  ldloc.1
    IL_000e:  ldc.i4.1
    IL_000f:  add
    IL_0010:  stloc.1
    IL_0011:  ldloc.0
    IL_0012:  ldc.i4.1
    IL_0013:  conv.i8
    IL_0014:  add
    IL_0015:  stloc.0
    IL_0016:  ldloc.0
    IL_0017:  ldc.i4.s   10
    IL_0019:  conv.i8
    IL_001a:  blt.un.s   IL_0007

    IL_001c:  ret
  } 

  .method public static void  f6() cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             int32 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stloc.0
    IL_0003:  ldc.i4.1
    IL_0004:  stloc.1
    IL_0005:  br.s       IL_0016

    IL_0007:  ldloc.1
    IL_0008:  call       void assembly::set_c(int32)
    IL_000d:  ldloc.1
    IL_000e:  ldc.i4.2
    IL_000f:  add
    IL_0010:  stloc.1
    IL_0011:  ldloc.0
    IL_0012:  ldc.i4.1
    IL_0013:  conv.i8
    IL_0014:  add
    IL_0015:  stloc.0
    IL_0016:  ldloc.0
    IL_0017:  ldc.i4.5
    IL_0018:  conv.i8
    IL_0019:  blt.un.s   IL_0007

    IL_001b:  ret
  } 

  .method public static void  f7(int32 start) cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             uint64 V_1,
             int32 V_2)
    IL_0000:  ldc.i4.s   10
    IL_0002:  ldarg.0
    IL_0003:  bge.s      IL_000a

    IL_0005:  ldc.i4.0
    IL_0006:  conv.i8
    IL_0007:  nop
    IL_0008:  br.s       IL_0017

    IL_000a:  ldc.i4.s   10
    IL_000c:  conv.i8
    IL_000d:  ldarg.0
    IL_000e:  conv.i8
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
    IL_001d:  br.s       IL_002e

    IL_001f:  ldloc.2
    IL_0020:  call       void assembly::set_c(int32)
    IL_0025:  ldloc.2
    IL_0026:  ldc.i4.2
    IL_0027:  add
    IL_0028:  stloc.2
    IL_0029:  ldloc.1
    IL_002a:  ldc.i4.1
    IL_002b:  conv.i8
    IL_002c:  add
    IL_002d:  stloc.1
    IL_002e:  ldloc.1
    IL_002f:  ldloc.0
    IL_0030:  blt.un.s   IL_001f

    IL_0032:  ret
  } 

  .method public static void  f8(int32 step) cil managed
  {
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             int32 V_2)
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
    IL_0013:  bge.s      IL_0024

    IL_0015:  ldc.i4.s   10
    IL_0017:  conv.i8
    IL_0018:  ldc.i4.1
    IL_0019:  conv.i8
    IL_001a:  sub
    IL_001b:  ldarg.0
    IL_001c:  conv.i8
    IL_001d:  div.un
    IL_001e:  ldc.i4.1
    IL_001f:  conv.i8
    IL_0020:  add.ovf.un
    IL_0021:  nop
    IL_0022:  br.s       IL_0027

    IL_0024:  ldc.i4.0
    IL_0025:  conv.i8
    IL_0026:  nop
    IL_0027:  stloc.0
    IL_0028:  ldc.i4.0
    IL_0029:  conv.i8
    IL_002a:  stloc.1
    IL_002b:  ldc.i4.1
    IL_002c:  stloc.2
    IL_002d:  br.s       IL_003e

    IL_002f:  ldloc.2
    IL_0030:  call       void assembly::set_c(int32)
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

  .method public static void  f9(int32 finish) cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             uint64 V_1,
             int32 V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  bge.s      IL_0009

    IL_0004:  ldc.i4.0
    IL_0005:  conv.i8
    IL_0006:  nop
    IL_0007:  br.s       IL_0015

    IL_0009:  ldarg.0
    IL_000a:  conv.i8
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
    IL_001a:  stloc.2
    IL_001b:  br.s       IL_002c

    IL_001d:  ldloc.2
    IL_001e:  call       void assembly::set_c(int32)
    IL_0023:  ldloc.2
    IL_0024:  ldc.i4.2
    IL_0025:  add
    IL_0026:  stloc.2
    IL_0027:  ldloc.1
    IL_0028:  ldc.i4.1
    IL_0029:  conv.i8
    IL_002a:  add
    IL_002b:  stloc.1
    IL_002c:  ldloc.1
    IL_002d:  ldloc.0
    IL_002e:  blt.un.s   IL_001d

    IL_0030:  ret
  } 

  .method public static void  f10<a>(!!a start,
                                     int32 step,
                                     int32 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             int32 V_2)
    IL_0000:  ldarg.1
    IL_0001:  brtrue.s   IL_000f

    IL_0003:  ldarg.2
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
    IL_0012:  bge.s      IL_002b

    IL_0014:  ldarg.2
    IL_0015:  ldarg.2
    IL_0016:  bge.s      IL_001d

    IL_0018:  ldc.i4.0
    IL_0019:  conv.i8
    IL_001a:  nop
    IL_001b:  br.s       IL_0045

    IL_001d:  ldarg.2
    IL_001e:  conv.i8
    IL_001f:  ldarg.2
    IL_0020:  conv.i8
    IL_0021:  sub
    IL_0022:  ldarg.1
    IL_0023:  conv.i8
    IL_0024:  div.un
    IL_0025:  ldc.i4.1
    IL_0026:  conv.i8
    IL_0027:  add.ovf.un
    IL_0028:  nop
    IL_0029:  br.s       IL_0045

    IL_002b:  ldarg.2
    IL_002c:  ldarg.2
    IL_002d:  bge.s      IL_0034

    IL_002f:  ldc.i4.0
    IL_0030:  conv.i8
    IL_0031:  nop
    IL_0032:  br.s       IL_0045

    IL_0034:  ldarg.2
    IL_0035:  conv.i8
    IL_0036:  ldarg.2
    IL_0037:  conv.i8
    IL_0038:  sub
    IL_0039:  ldarg.1
    IL_003a:  not
    IL_003b:  conv.i8
    IL_003c:  ldc.i4.1
    IL_003d:  conv.i8
    IL_003e:  add
    IL_003f:  conv.i8
    IL_0040:  div.un
    IL_0041:  ldc.i4.1
    IL_0042:  conv.i8
    IL_0043:  add.ovf.un
    IL_0044:  nop
    IL_0045:  stloc.0
    IL_0046:  ldc.i4.0
    IL_0047:  conv.i8
    IL_0048:  stloc.1
    IL_0049:  ldarg.2
    IL_004a:  stloc.2
    IL_004b:  br.s       IL_005c

    IL_004d:  ldloc.2
    IL_004e:  call       void assembly::set_c(int32)
    IL_0053:  ldloc.2
    IL_0054:  ldarg.1
    IL_0055:  add
    IL_0056:  stloc.2
    IL_0057:  ldloc.1
    IL_0058:  ldc.i4.1
    IL_0059:  conv.i8
    IL_005a:  add
    IL_005b:  stloc.1
    IL_005c:  ldloc.1
    IL_005d:  ldloc.0
    IL_005e:  blt.un.s   IL_004d

    IL_0060:  ret
  } 

  .method public static void  f11(int32 start,
                                  int32 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  6
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.0
    IL_0003:  stloc.1
    IL_0004:  br.s       IL_0014

    IL_0006:  ldloc.1
    IL_0007:  call       void assembly::set_c(int32)
    IL_000c:  ldloc.1
    IL_000d:  ldc.i4.0
    IL_000e:  add
    IL_000f:  stloc.1
    IL_0010:  ldloc.0
    IL_0011:  ldc.i4.1
    IL_0012:  add
    IL_0013:  stloc.0
    IL_0014:  ldloc.0
    IL_0015:  ldarg.0
    IL_0016:  ldc.i4.0
    IL_0017:  ldarg.1
    IL_0018:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                           int32,
                                                                                                                                                                           int32)
    IL_001d:  pop
    IL_001e:  ldc.i4.m1
    IL_001f:  blt.un.s   IL_0006

    IL_0021:  ret
  } 

  .method public static void  f12() cil managed
  {
    
    .maxstack  6
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.1
    IL_0003:  stloc.1
    IL_0004:  br.s       IL_0014

    IL_0006:  ldloc.1
    IL_0007:  call       void assembly::set_c(int32)
    IL_000c:  ldloc.1
    IL_000d:  ldc.i4.0
    IL_000e:  add
    IL_000f:  stloc.1
    IL_0010:  ldloc.0
    IL_0011:  ldc.i4.1
    IL_0012:  add
    IL_0013:  stloc.0
    IL_0014:  ldloc.0
    IL_0015:  ldc.i4.1
    IL_0016:  ldc.i4.0
    IL_0017:  ldc.i4.s   10
    IL_0019:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                           int32,
                                                                                                                                                                           int32)
    IL_001e:  pop
    IL_001f:  ldc.i4.m1
    IL_0020:  blt.un.s   IL_0006

    IL_0022:  ret
  } 

  .method public static void  f13() cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             int32 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stloc.0
    IL_0003:  ldc.i4.s   10
    IL_0005:  stloc.1
    IL_0006:  br.s       IL_0017

    IL_0008:  ldloc.1
    IL_0009:  call       void assembly::set_c(int32)
    IL_000e:  ldloc.1
    IL_000f:  ldc.i4.m1
    IL_0010:  add
    IL_0011:  stloc.1
    IL_0012:  ldloc.0
    IL_0013:  ldc.i4.1
    IL_0014:  conv.i8
    IL_0015:  add
    IL_0016:  stloc.0
    IL_0017:  ldloc.0
    IL_0018:  ldc.i4.s   10
    IL_001a:  conv.i8
    IL_001b:  blt.un.s   IL_0008

    IL_001d:  ret
  } 

  .method public static void  f14() cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             int32 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stloc.0
    IL_0003:  ldc.i4.s   10
    IL_0005:  stloc.1
    IL_0006:  br.s       IL_0018

    IL_0008:  ldloc.1
    IL_0009:  call       void assembly::set_c(int32)
    IL_000e:  ldloc.1
    IL_000f:  ldc.i4.s   -2
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

  .property int32 c()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .set void assembly::set_c(int32)
    .get int32 assembly::get_c()
  } 
} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly
       extends [runtime]System.Object
{
  .field static assembly int32 c@1
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
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$assembly::c@1
    IL_0006:  ret
  } 

} 






