




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
    IL_0008:  br.s       IL_0014

    IL_000a:  ldc.i4.s   10
    IL_000c:  conv.i8
    IL_000d:  ldarg.0
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
    IL_0018:  ldarg.0
    IL_0019:  stloc.2
    IL_001a:  br.s       IL_002b

    IL_001c:  ldloc.2
    IL_001d:  call       void assembly::set_c(uint32)
    IL_0022:  ldloc.2
    IL_0023:  ldc.i4.1
    IL_0024:  add
    IL_0025:  stloc.2
    IL_0026:  ldloc.1
    IL_0027:  ldc.i4.1
    IL_0028:  conv.i8
    IL_0029:  add
    IL_002a:  stloc.1
    IL_002b:  ldloc.1
    IL_002c:  ldloc.0
    IL_002d:  blt.un.s   IL_001c

    IL_002f:  ret
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
    IL_0007:  br.s       IL_0012

    IL_0009:  ldarg.0
    IL_000a:  conv.i8
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
    IL_0017:  stloc.2
    IL_0018:  br.s       IL_0029

    IL_001a:  ldloc.2
    IL_001b:  call       void assembly::set_c(uint32)
    IL_0020:  ldloc.2
    IL_0021:  ldc.i4.1
    IL_0022:  add
    IL_0023:  stloc.2
    IL_0024:  ldloc.1
    IL_0025:  ldc.i4.1
    IL_0026:  conv.i8
    IL_0027:  add
    IL_0028:  stloc.1
    IL_0029:  ldloc.1
    IL_002a:  ldloc.0
    IL_002b:  blt.un.s   IL_001a

    IL_002d:  ret
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
    IL_0007:  br.s       IL_0012

    IL_0009:  ldarg.1
    IL_000a:  conv.i8
    IL_000b:  ldarg.0
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
    IL_0016:  ldarg.0
    IL_0017:  stloc.2
    IL_0018:  br.s       IL_0029

    IL_001a:  ldloc.2
    IL_001b:  call       void assembly::set_c(uint32)
    IL_0020:  ldloc.2
    IL_0021:  ldc.i4.1
    IL_0022:  add
    IL_0023:  stloc.2
    IL_0024:  ldloc.1
    IL_0025:  ldc.i4.1
    IL_0026:  conv.i8
    IL_0027:  add
    IL_0028:  stloc.1
    IL_0029:  ldloc.1
    IL_002a:  ldloc.0
    IL_002b:  blt.un.s   IL_001a

    IL_002d:  ret
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
    IL_0020:  call       void assembly::set_c(uint32)
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
    IL_0011:  ldc.i4.s   10
    IL_0013:  conv.i8
    IL_0014:  ldc.i4.1
    IL_0015:  conv.i8
    IL_0016:  sub
    IL_0017:  ldarg.0
    IL_0018:  conv.i8
    IL_0019:  div.un
    IL_001a:  ldc.i4.1
    IL_001b:  conv.i8
    IL_001c:  add.ovf.un
    IL_001d:  stloc.0
    IL_001e:  ldc.i4.0
    IL_001f:  conv.i8
    IL_0020:  stloc.1
    IL_0021:  ldc.i4.1
    IL_0022:  stloc.2
    IL_0023:  br.s       IL_0034

    IL_0025:  ldloc.2
    IL_0026:  call       void assembly::set_c(uint32)
    IL_002b:  ldloc.2
    IL_002c:  ldarg.0
    IL_002d:  add
    IL_002e:  stloc.2
    IL_002f:  ldloc.1
    IL_0030:  ldc.i4.1
    IL_0031:  conv.i8
    IL_0032:  add
    IL_0033:  stloc.1
    IL_0034:  ldloc.1
    IL_0035:  ldloc.0
    IL_0036:  blt.un.s   IL_0025

    IL_0038:  ret
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
    IL_0017:  br.s       IL_0025

    IL_0019:  ldarg.2
    IL_001a:  conv.i8
    IL_001b:  ldarg.2
    IL_001c:  conv.i8
    IL_001d:  sub
    IL_001e:  ldarg.1
    IL_001f:  conv.i8
    IL_0020:  div.un
    IL_0021:  ldc.i4.1
    IL_0022:  conv.i8
    IL_0023:  add.ovf.un
    IL_0024:  nop
    IL_0025:  stloc.0
    IL_0026:  ldc.i4.0
    IL_0027:  conv.i8
    IL_0028:  stloc.1
    IL_0029:  ldarg.2
    IL_002a:  stloc.2
    IL_002b:  br.s       IL_003c

    IL_002d:  ldloc.2
    IL_002e:  call       void assembly::set_c(uint32)
    IL_0033:  ldloc.2
    IL_0034:  ldarg.1
    IL_0035:  add
    IL_0036:  stloc.2
    IL_0037:  ldloc.1
    IL_0038:  ldc.i4.1
    IL_0039:  conv.i8
    IL_003a:  add
    IL_003b:  stloc.1
    IL_003c:  ldloc.1
    IL_003d:  ldloc.0
    IL_003e:  blt.un.s   IL_002d

    IL_0040:  ret
  } 

  .method public static void  f11(uint32 start,
                                  uint32 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  ldarg.1
    IL_0003:  call       class [runtime]System.Collections.Generic.IEnumerable`1<uint32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeUInt32(uint32,
                                                                                                                                                                             uint32,
                                                                                                                                                                             uint32)
    IL_0008:  pop
    IL_0009:  ret
  } 

  .method public static void  f12() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.1
    IL_0001:  ldc.i4.0
    IL_0002:  ldc.i4.s   10
    IL_0004:  call       class [runtime]System.Collections.Generic.IEnumerable`1<uint32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeUInt32(uint32,
                                                                                                                                                                             uint32,
                                                                                                                                                                             uint32)
    IL_0009:  pop
    IL_000a:  ret
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






