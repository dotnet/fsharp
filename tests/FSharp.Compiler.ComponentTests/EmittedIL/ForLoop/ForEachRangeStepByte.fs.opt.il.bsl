




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
  .method public specialname static uint8 get_c() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     uint8 '<StartupCode$assembly>'.$assembly::c@1
    IL_0005:  ret
  } 

  .method public specialname static void set_c(uint8 'value') cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  stsfld     uint8 '<StartupCode$assembly>'.$assembly::c@1
    IL_0006:  ret
  } 

  .method public static void  f0() cil managed
  {
    
    .maxstack  4
    .locals init (uint8 V_0,
             uint8 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.s   10
    IL_0004:  stloc.1
    IL_0005:  br.s       IL_0015

    IL_0007:  ldloc.1
    IL_0008:  call       void assembly::set_c(uint8)
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
    .locals init (uint8 V_0,
             uint8 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.s   10
    IL_0004:  stloc.1
    IL_0005:  br.s       IL_0015

    IL_0007:  ldloc.1
    IL_0008:  call       void assembly::set_c(uint8)
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
    .locals init (uint16 V_0,
             uint8 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.1
    IL_0003:  stloc.1
    IL_0004:  br.s       IL_0014

    IL_0006:  ldloc.1
    IL_0007:  call       void assembly::set_c(uint8)
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

  .method public static void  f2uy(uint8 start) cil managed
  {
    
    .maxstack  4
    .locals init (uint16 V_0,
             uint16 V_1,
             uint8 V_2)
    IL_0000:  ldc.i4.s   10
    IL_0002:  ldarg.0
    IL_0003:  bge.un.s   IL_0009

    IL_0005:  ldc.i4.0
    IL_0006:  nop
    IL_0007:  br.s       IL_0012

    IL_0009:  ldc.i4.s   10
    IL_000b:  conv.i2
    IL_000c:  ldarg.0
    IL_000d:  conv.i2
    IL_000e:  sub
    IL_000f:  ldc.i4.1
    IL_0010:  add
    IL_0011:  nop
    IL_0012:  stloc.0
    IL_0013:  ldc.i4.0
    IL_0014:  stloc.1
    IL_0015:  ldarg.0
    IL_0016:  stloc.2
    IL_0017:  br.s       IL_0027

    IL_0019:  ldloc.2
    IL_001a:  call       void assembly::set_c(uint8)
    IL_001f:  ldloc.2
    IL_0020:  ldc.i4.1
    IL_0021:  add
    IL_0022:  stloc.2
    IL_0023:  ldloc.1
    IL_0024:  ldc.i4.1
    IL_0025:  add
    IL_0026:  stloc.1
    IL_0027:  ldloc.1
    IL_0028:  ldloc.0
    IL_0029:  blt.un.s   IL_0019

    IL_002b:  ret
  } 

  .method public static void  f3uy(uint8 finish) cil managed
  {
    
    .maxstack  4
    .locals init (uint16 V_0,
             uint16 V_1,
             uint8 V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  bge.un.s   IL_0008

    IL_0004:  ldc.i4.0
    IL_0005:  nop
    IL_0006:  br.s       IL_0010

    IL_0008:  ldarg.0
    IL_0009:  conv.i2
    IL_000a:  ldc.i4.1
    IL_000b:  conv.i2
    IL_000c:  sub
    IL_000d:  ldc.i4.1
    IL_000e:  add
    IL_000f:  nop
    IL_0010:  stloc.0
    IL_0011:  ldc.i4.0
    IL_0012:  stloc.1
    IL_0013:  ldc.i4.1
    IL_0014:  stloc.2
    IL_0015:  br.s       IL_0025

    IL_0017:  ldloc.2
    IL_0018:  call       void assembly::set_c(uint8)
    IL_001d:  ldloc.2
    IL_001e:  ldc.i4.1
    IL_001f:  add
    IL_0020:  stloc.2
    IL_0021:  ldloc.1
    IL_0022:  ldc.i4.1
    IL_0023:  add
    IL_0024:  stloc.1
    IL_0025:  ldloc.1
    IL_0026:  ldloc.0
    IL_0027:  blt.un.s   IL_0017

    IL_0029:  ret
  } 

  .method public static void  f4uy(uint8 start,
                                   uint8 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  4
    .locals init (uint16 V_0,
             uint16 V_1,
             uint8 V_2)
    IL_0000:  ldarg.1
    IL_0001:  ldarg.0
    IL_0002:  bge.un.s   IL_0008

    IL_0004:  ldc.i4.0
    IL_0005:  nop
    IL_0006:  br.s       IL_0010

    IL_0008:  ldarg.1
    IL_0009:  conv.i2
    IL_000a:  ldarg.0
    IL_000b:  conv.i2
    IL_000c:  sub
    IL_000d:  ldc.i4.1
    IL_000e:  add
    IL_000f:  nop
    IL_0010:  stloc.0
    IL_0011:  ldc.i4.0
    IL_0012:  stloc.1
    IL_0013:  ldarg.0
    IL_0014:  stloc.2
    IL_0015:  br.s       IL_0025

    IL_0017:  ldloc.2
    IL_0018:  call       void assembly::set_c(uint8)
    IL_001d:  ldloc.2
    IL_001e:  ldc.i4.1
    IL_001f:  add
    IL_0020:  stloc.2
    IL_0021:  ldloc.1
    IL_0022:  ldc.i4.1
    IL_0023:  add
    IL_0024:  stloc.1
    IL_0025:  ldloc.1
    IL_0026:  ldloc.0
    IL_0027:  blt.un.s   IL_0017

    IL_0029:  ret
  } 

  .method public static void  f5() cil managed
  {
    
    .maxstack  4
    .locals init (uint16 V_0,
             uint8 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.1
    IL_0003:  stloc.1
    IL_0004:  br.s       IL_0014

    IL_0006:  ldloc.1
    IL_0007:  call       void assembly::set_c(uint8)
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
    .locals init (uint16 V_0,
             uint8 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.1
    IL_0003:  stloc.1
    IL_0004:  br.s       IL_0014

    IL_0006:  ldloc.1
    IL_0007:  call       void assembly::set_c(uint8)
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

  .method public static void  f7uy(uint8 start) cil managed
  {
    
    .maxstack  4
    .locals init (uint16 V_0,
             uint16 V_1,
             uint8 V_2)
    IL_0000:  ldc.i4.s   10
    IL_0002:  ldarg.0
    IL_0003:  bge.un.s   IL_0009

    IL_0005:  ldc.i4.0
    IL_0006:  nop
    IL_0007:  br.s       IL_0014

    IL_0009:  ldc.i4.s   10
    IL_000b:  conv.i2
    IL_000c:  ldarg.0
    IL_000d:  conv.i2
    IL_000e:  sub
    IL_000f:  ldc.i4.2
    IL_0010:  div.un
    IL_0011:  ldc.i4.1
    IL_0012:  add
    IL_0013:  nop
    IL_0014:  stloc.0
    IL_0015:  ldc.i4.0
    IL_0016:  stloc.1
    IL_0017:  ldarg.0
    IL_0018:  stloc.2
    IL_0019:  br.s       IL_0029

    IL_001b:  ldloc.2
    IL_001c:  call       void assembly::set_c(uint8)
    IL_0021:  ldloc.2
    IL_0022:  ldc.i4.2
    IL_0023:  add
    IL_0024:  stloc.2
    IL_0025:  ldloc.1
    IL_0026:  ldc.i4.1
    IL_0027:  add
    IL_0028:  stloc.1
    IL_0029:  ldloc.1
    IL_002a:  ldloc.0
    IL_002b:  blt.un.s   IL_001b

    IL_002d:  ret
  } 

  .method public static void  f8uy(uint8 step) cil managed
  {
    
    .maxstack  5
    .locals init (uint16 V_0,
             uint16 V_1,
             uint8 V_2)
    IL_0000:  ldarg.0
    IL_0001:  brtrue.s   IL_0010

    IL_0003:  ldc.i4.1
    IL_0004:  ldarg.0
    IL_0005:  ldc.i4.s   10
    IL_0007:  call       class [runtime]System.Collections.Generic.IEnumerable`1<uint8> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeByte(uint8,
                                                                                                                                                                          uint8,
                                                                                                                                                                          uint8)
    IL_000c:  pop
    IL_000d:  nop
    IL_000e:  br.s       IL_0011

    IL_0010:  nop
    IL_0011:  ldc.i4.s   10
    IL_0013:  conv.i2
    IL_0014:  ldc.i4.1
    IL_0015:  conv.i2
    IL_0016:  sub
    IL_0017:  ldarg.0
    IL_0018:  div.un
    IL_0019:  ldc.i4.1
    IL_001a:  add
    IL_001b:  stloc.0
    IL_001c:  ldc.i4.0
    IL_001d:  stloc.1
    IL_001e:  ldc.i4.1
    IL_001f:  stloc.2
    IL_0020:  br.s       IL_0030

    IL_0022:  ldloc.2
    IL_0023:  call       void assembly::set_c(uint8)
    IL_0028:  ldloc.2
    IL_0029:  ldarg.0
    IL_002a:  add
    IL_002b:  stloc.2
    IL_002c:  ldloc.1
    IL_002d:  ldc.i4.1
    IL_002e:  add
    IL_002f:  stloc.1
    IL_0030:  ldloc.1
    IL_0031:  ldloc.0
    IL_0032:  blt.un.s   IL_0022

    IL_0034:  ret
  } 

  .method public static void  f9uy(uint8 finish) cil managed
  {
    
    .maxstack  4
    .locals init (uint16 V_0,
             uint16 V_1,
             uint8 V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  bge.un.s   IL_0008

    IL_0004:  ldc.i4.0
    IL_0005:  nop
    IL_0006:  br.s       IL_0012

    IL_0008:  ldarg.0
    IL_0009:  conv.i2
    IL_000a:  ldc.i4.1
    IL_000b:  conv.i2
    IL_000c:  sub
    IL_000d:  ldc.i4.2
    IL_000e:  div.un
    IL_000f:  ldc.i4.1
    IL_0010:  add
    IL_0011:  nop
    IL_0012:  stloc.0
    IL_0013:  ldc.i4.0
    IL_0014:  stloc.1
    IL_0015:  ldc.i4.1
    IL_0016:  stloc.2
    IL_0017:  br.s       IL_0027

    IL_0019:  ldloc.2
    IL_001a:  call       void assembly::set_c(uint8)
    IL_001f:  ldloc.2
    IL_0020:  ldc.i4.2
    IL_0021:  add
    IL_0022:  stloc.2
    IL_0023:  ldloc.1
    IL_0024:  ldc.i4.1
    IL_0025:  add
    IL_0026:  stloc.1
    IL_0027:  ldloc.1
    IL_0028:  ldloc.0
    IL_0029:  blt.un.s   IL_0019

    IL_002b:  ret
  } 

  .method public static void  f10uy(uint8 start,
                                    uint8 step,
                                    uint8 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  5
    .locals init (uint16 V_0,
             uint16 V_1,
             uint8 V_2)
    IL_0000:  ldarg.1
    IL_0001:  brtrue.s   IL_000f

    IL_0003:  ldarg.2
    IL_0004:  ldarg.1
    IL_0005:  ldarg.2
    IL_0006:  call       class [runtime]System.Collections.Generic.IEnumerable`1<uint8> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeByte(uint8,
                                                                                                                                                                          uint8,
                                                                                                                                                                          uint8)
    IL_000b:  pop
    IL_000c:  nop
    IL_000d:  br.s       IL_0010

    IL_000f:  nop
    IL_0010:  ldarg.2
    IL_0011:  ldarg.2
    IL_0012:  bge.un.s   IL_0018

    IL_0014:  ldc.i4.0
    IL_0015:  nop
    IL_0016:  br.s       IL_0022

    IL_0018:  ldarg.2
    IL_0019:  conv.i2
    IL_001a:  ldarg.2
    IL_001b:  conv.i2
    IL_001c:  sub
    IL_001d:  ldarg.1
    IL_001e:  div.un
    IL_001f:  ldc.i4.1
    IL_0020:  add
    IL_0021:  nop
    IL_0022:  stloc.0
    IL_0023:  ldc.i4.0
    IL_0024:  stloc.1
    IL_0025:  ldarg.2
    IL_0026:  stloc.2
    IL_0027:  br.s       IL_0037

    IL_0029:  ldloc.2
    IL_002a:  call       void assembly::set_c(uint8)
    IL_002f:  ldloc.2
    IL_0030:  ldarg.1
    IL_0031:  add
    IL_0032:  stloc.2
    IL_0033:  ldloc.1
    IL_0034:  ldc.i4.1
    IL_0035:  add
    IL_0036:  stloc.1
    IL_0037:  ldloc.1
    IL_0038:  ldloc.0
    IL_0039:  blt.un.s   IL_0029

    IL_003b:  ret
  } 

  .method public static void  f11uy(uint8 start,
                                    uint8 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  6
    .locals init (int32 V_0,
             uint8 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.0
    IL_0003:  stloc.1
    IL_0004:  br.s       IL_0014

    IL_0006:  ldloc.1
    IL_0007:  call       void assembly::set_c(uint8)
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
    IL_0018:  call       class [runtime]System.Collections.Generic.IEnumerable`1<uint8> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeByte(uint8,
                                                                                                                                                                          uint8,
                                                                                                                                                                          uint8)
    IL_001d:  pop
    IL_001e:  ldc.i4.m1
    IL_001f:  blt.un.s   IL_0006

    IL_0021:  ret
  } 

  .method public static void  f12() cil managed
  {
    
    .maxstack  6
    .locals init (int32 V_0,
             uint8 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.1
    IL_0003:  stloc.1
    IL_0004:  br.s       IL_0014

    IL_0006:  ldloc.1
    IL_0007:  call       void assembly::set_c(uint8)
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
    IL_0019:  call       class [runtime]System.Collections.Generic.IEnumerable`1<uint8> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeByte(uint8,
                                                                                                                                                                          uint8,
                                                                                                                                                                          uint8)
    IL_001e:  pop
    IL_001f:  ldc.i4.m1
    IL_0020:  blt.un.s   IL_0006

    IL_0022:  ret
  } 

  .property uint8 c()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .set void assembly::set_c(uint8)
    .get uint8 assembly::get_c()
  } 
} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly
       extends [runtime]System.Object
{
  .field static assembly uint8 c@1
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
    IL_0001:  stsfld     uint8 '<StartupCode$assembly>'.$assembly::c@1
    IL_0006:  ret
  } 

} 






