




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
  .method public specialname static uint32 get_c() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     uint32 '<StartupCode$assembly>'.$assembly::c@1
    IL_0005:  ret
  } 

  .method public specialname static void set_c(uint32 'value') cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  stsfld     uint32 '<StartupCode$assembly>'.$assembly::c@1
    IL_0006:  ret
  } 

  .method public static void  f0() cil managed
  {
    
    .maxstack  4
    .locals init (uint32 V_0,
             uint32 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.s   10
    IL_0004:  stloc.1
    IL_0005:  br.s       IL_0015

    IL_0007:  ldloc.1
    IL_0008:  call       void assembly::set_c(uint32)
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
    .locals init (uint32 V_0,
             uint32 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.s   10
    IL_0004:  stloc.1
    IL_0005:  br.s       IL_0015

    IL_0007:  ldloc.1
    IL_0008:  call       void assembly::set_c(uint32)
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
    .locals init (uint64 V_0,
             uint32 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stloc.0
    IL_0003:  ldc.i4.1
    IL_0004:  stloc.1
    IL_0005:  br.s       IL_0016

    IL_0007:  ldloc.1
    IL_0008:  call       void assembly::set_c(uint32)
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

  .method public static void  f2(uint32 start) cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             uint64 V_1,
             uint32 V_2)
    IL_0000:  ldc.i4.s   10
    IL_0002:  ldarg.0
    IL_0003:  bge.un.s   IL_000a

    IL_0005:  ldc.i4.0
    IL_0006:  conv.i8
    IL_0007:  nop
    IL_0008:  br.s       IL_0013

    IL_000a:  ldc.i4.s   10
    IL_000c:  ldarg.0
    IL_000d:  sub
    IL_000e:  conv.u8
    IL_000f:  ldc.i4.1
    IL_0010:  conv.i8
    IL_0011:  add
    IL_0012:  nop
    IL_0013:  stloc.0
    IL_0014:  ldc.i4.0
    IL_0015:  conv.i8
    IL_0016:  stloc.1
    IL_0017:  ldarg.0
    IL_0018:  stloc.2
    IL_0019:  br.s       IL_002a

    IL_001b:  ldloc.2
    IL_001c:  call       void assembly::set_c(uint32)
    IL_0021:  ldloc.2
    IL_0022:  ldc.i4.1
    IL_0023:  add
    IL_0024:  stloc.2
    IL_0025:  ldloc.1
    IL_0026:  ldc.i4.1
    IL_0027:  conv.i8
    IL_0028:  add
    IL_0029:  stloc.1
    IL_002a:  ldloc.1
    IL_002b:  ldloc.0
    IL_002c:  blt.un.s   IL_001b

    IL_002e:  ret
  } 

  .method public static void  f3(uint32 finish) cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             uint64 V_1,
             uint32 V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  bge.un.s   IL_0009

    IL_0004:  ldc.i4.0
    IL_0005:  conv.i8
    IL_0006:  nop
    IL_0007:  br.s       IL_0011

    IL_0009:  ldarg.0
    IL_000a:  ldc.i4.1
    IL_000b:  sub
    IL_000c:  conv.u8
    IL_000d:  ldc.i4.1
    IL_000e:  conv.i8
    IL_000f:  add
    IL_0010:  nop
    IL_0011:  stloc.0
    IL_0012:  ldc.i4.0
    IL_0013:  conv.i8
    IL_0014:  stloc.1
    IL_0015:  ldc.i4.1
    IL_0016:  stloc.2
    IL_0017:  br.s       IL_0028

    IL_0019:  ldloc.2
    IL_001a:  call       void assembly::set_c(uint32)
    IL_001f:  ldloc.2
    IL_0020:  ldc.i4.1
    IL_0021:  add
    IL_0022:  stloc.2
    IL_0023:  ldloc.1
    IL_0024:  ldc.i4.1
    IL_0025:  conv.i8
    IL_0026:  add
    IL_0027:  stloc.1
    IL_0028:  ldloc.1
    IL_0029:  ldloc.0
    IL_002a:  blt.un.s   IL_0019

    IL_002c:  ret
  } 

  .method public static void  f4(uint32 start,
                                 uint32 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  4
    .locals init (uint64 V_0,
             uint64 V_1,
             uint32 V_2)
    IL_0000:  ldarg.1
    IL_0001:  ldarg.0
    IL_0002:  bge.un.s   IL_0009

    IL_0004:  ldc.i4.0
    IL_0005:  conv.i8
    IL_0006:  nop
    IL_0007:  br.s       IL_0011

    IL_0009:  ldarg.1
    IL_000a:  ldarg.0
    IL_000b:  sub
    IL_000c:  conv.u8
    IL_000d:  ldc.i4.1
    IL_000e:  conv.i8
    IL_000f:  add
    IL_0010:  nop
    IL_0011:  stloc.0
    IL_0012:  ldc.i4.0
    IL_0013:  conv.i8
    IL_0014:  stloc.1
    IL_0015:  ldarg.0
    IL_0016:  stloc.2
    IL_0017:  br.s       IL_0028

    IL_0019:  ldloc.2
    IL_001a:  call       void assembly::set_c(uint32)
    IL_001f:  ldloc.2
    IL_0020:  ldc.i4.1
    IL_0021:  add
    IL_0022:  stloc.2
    IL_0023:  ldloc.1
    IL_0024:  ldc.i4.1
    IL_0025:  conv.i8
    IL_0026:  add
    IL_0027:  stloc.1
    IL_0028:  ldloc.1
    IL_0029:  ldloc.0
    IL_002a:  blt.un.s   IL_0019

    IL_002c:  ret
  } 

  .method public static void  f5() cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             uint32 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stloc.0
    IL_0003:  ldc.i4.1
    IL_0004:  stloc.1
    IL_0005:  br.s       IL_0016

    IL_0007:  ldloc.1
    IL_0008:  call       void assembly::set_c(uint32)
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
             uint32 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stloc.0
    IL_0003:  ldc.i4.1
    IL_0004:  stloc.1
    IL_0005:  br.s       IL_0016

    IL_0007:  ldloc.1
    IL_0008:  call       void assembly::set_c(uint32)
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

  .method public static void  f7(uint32 start) cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             uint64 V_1,
             uint32 V_2)
    IL_0000:  ldc.i4.s   10
    IL_0002:  ldarg.0
    IL_0003:  bge.un.s   IL_000a

    IL_0005:  ldc.i4.0
    IL_0006:  conv.i8
    IL_0007:  nop
    IL_0008:  br.s       IL_0015

    IL_000a:  ldc.i4.s   10
    IL_000c:  ldarg.0
    IL_000d:  sub
    IL_000e:  ldc.i4.2
    IL_000f:  div.un
    IL_0010:  conv.u8
    IL_0011:  ldc.i4.1
    IL_0012:  conv.i8
    IL_0013:  add
    IL_0014:  nop
    IL_0015:  stloc.0
    IL_0016:  ldc.i4.0
    IL_0017:  conv.i8
    IL_0018:  stloc.1
    IL_0019:  ldarg.0
    IL_001a:  stloc.2
    IL_001b:  br.s       IL_002c

    IL_001d:  ldloc.2
    IL_001e:  call       void assembly::set_c(uint32)
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

  .method public static void  f8(uint32 step) cil managed
  {
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             uint32 V_2)
    IL_0000:  ldarg.0
    IL_0001:  brtrue.s   IL_0010

    IL_0003:  ldc.i4.1
    IL_0004:  ldarg.0
    IL_0005:  ldc.i4.s   10
    IL_0007:  call       class [runtime]System.Collections.Generic.IEnumerable`1<uint32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeUInt32(uint32,
                                                                                                                                                                             uint32,
                                                                                                                                                                             uint32)
    IL_000c:  pop
    IL_000d:  nop
    IL_000e:  br.s       IL_0011

    IL_0010:  nop
    IL_0011:  ldc.i4.s   9
    IL_0013:  ldarg.0
    IL_0014:  div.un
    IL_0015:  conv.u8
    IL_0016:  ldc.i4.1
    IL_0017:  conv.i8
    IL_0018:  add
    IL_0019:  stloc.0
    IL_001a:  ldc.i4.0
    IL_001b:  conv.i8
    IL_001c:  stloc.1
    IL_001d:  ldc.i4.1
    IL_001e:  stloc.2
    IL_001f:  br.s       IL_0030

    IL_0021:  ldloc.2
    IL_0022:  call       void assembly::set_c(uint32)
    IL_0027:  ldloc.2
    IL_0028:  ldarg.0
    IL_0029:  add
    IL_002a:  stloc.2
    IL_002b:  ldloc.1
    IL_002c:  ldc.i4.1
    IL_002d:  conv.i8
    IL_002e:  add
    IL_002f:  stloc.1
    IL_0030:  ldloc.1
    IL_0031:  ldloc.0
    IL_0032:  blt.un.s   IL_0021

    IL_0034:  ret
  } 

  .method public static void  f9(uint32 finish) cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             uint64 V_1,
             uint32 V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  bge.un.s   IL_0009

    IL_0004:  ldc.i4.0
    IL_0005:  conv.i8
    IL_0006:  nop
    IL_0007:  br.s       IL_0013

    IL_0009:  ldarg.0
    IL_000a:  ldc.i4.1
    IL_000b:  sub
    IL_000c:  ldc.i4.2
    IL_000d:  div.un
    IL_000e:  conv.u8
    IL_000f:  ldc.i4.1
    IL_0010:  conv.i8
    IL_0011:  add
    IL_0012:  nop
    IL_0013:  stloc.0
    IL_0014:  ldc.i4.0
    IL_0015:  conv.i8
    IL_0016:  stloc.1
    IL_0017:  ldc.i4.1
    IL_0018:  stloc.2
    IL_0019:  br.s       IL_002a

    IL_001b:  ldloc.2
    IL_001c:  call       void assembly::set_c(uint32)
    IL_0021:  ldloc.2
    IL_0022:  ldc.i4.2
    IL_0023:  add
    IL_0024:  stloc.2
    IL_0025:  ldloc.1
    IL_0026:  ldc.i4.1
    IL_0027:  conv.i8
    IL_0028:  add
    IL_0029:  stloc.1
    IL_002a:  ldloc.1
    IL_002b:  ldloc.0
    IL_002c:  blt.un.s   IL_001b

    IL_002e:  ret
  } 

  .method public static void  f10(uint32 start,
                                  uint32 step,
                                  uint32 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  5
    .locals init (uint64 V_0,
             uint64 V_1,
             uint32 V_2)
    IL_0000:  ldarg.1
    IL_0001:  brtrue.s   IL_000f

    IL_0003:  ldarg.2
    IL_0004:  ldarg.1
    IL_0005:  ldarg.2
    IL_0006:  call       class [runtime]System.Collections.Generic.IEnumerable`1<uint32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeUInt32(uint32,
                                                                                                                                                                             uint32,
                                                                                                                                                                             uint32)
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
    IL_0017:  br.s       IL_0023

    IL_0019:  ldarg.2
    IL_001a:  ldarg.2
    IL_001b:  sub
    IL_001c:  ldarg.1
    IL_001d:  div.un
    IL_001e:  conv.u8
    IL_001f:  ldc.i4.1
    IL_0020:  conv.i8
    IL_0021:  add
    IL_0022:  nop
    IL_0023:  stloc.0
    IL_0024:  ldc.i4.0
    IL_0025:  conv.i8
    IL_0026:  stloc.1
    IL_0027:  ldarg.2
    IL_0028:  stloc.2
    IL_0029:  br.s       IL_003a

    IL_002b:  ldloc.2
    IL_002c:  call       void assembly::set_c(uint32)
    IL_0031:  ldloc.2
    IL_0032:  ldarg.1
    IL_0033:  add
    IL_0034:  stloc.2
    IL_0035:  ldloc.1
    IL_0036:  ldc.i4.1
    IL_0037:  conv.i8
    IL_0038:  add
    IL_0039:  stloc.1
    IL_003a:  ldloc.1
    IL_003b:  ldloc.0
    IL_003c:  blt.un.s   IL_002b

    IL_003e:  ret
  } 

  .method public static void  f11(uint32 start,
                                  uint32 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (uint32 V_0,
             uint32 V_1,
             uint32 V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  ldarg.1
    IL_0003:  call       class [runtime]System.Collections.Generic.IEnumerable`1<uint32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeUInt32(uint32,
                                                                                                                                                                             uint32,
                                                                                                                                                                             uint32)
    IL_0008:  pop
    IL_0009:  ldc.i4.0
    IL_000a:  stloc.0
    IL_000b:  ldc.i4.0
    IL_000c:  stloc.1
    IL_000d:  ldarg.0
    IL_000e:  stloc.2
    IL_000f:  br.s       IL_001f

    IL_0011:  ldloc.2
    IL_0012:  call       void assembly::set_c(uint32)
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
    .locals init (uint32 V_0,
             uint32 V_1,
             uint32 V_2)
    IL_0000:  ldc.i4.1
    IL_0001:  ldc.i4.0
    IL_0002:  ldc.i4.s   10
    IL_0004:  call       class [runtime]System.Collections.Generic.IEnumerable`1<uint32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeUInt32(uint32,
                                                                                                                                                                             uint32,
                                                                                                                                                                             uint32)
    IL_0009:  pop
    IL_000a:  ldc.i4.0
    IL_000b:  stloc.0
    IL_000c:  ldc.i4.0
    IL_000d:  stloc.1
    IL_000e:  ldc.i4.1
    IL_000f:  stloc.2
    IL_0010:  br.s       IL_0020

    IL_0012:  ldloc.2
    IL_0013:  call       void assembly::set_c(uint32)
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

  .property uint32 c()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .set void assembly::set_c(uint32)
    .get uint32 assembly::get_c()
  } 
} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly
       extends [runtime]System.Object
{
  .field static assembly uint32 c@1
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
    IL_0001:  stsfld     uint32 '<StartupCode$assembly>'.$assembly::c@1
    IL_0006:  ret
  } 

} 






