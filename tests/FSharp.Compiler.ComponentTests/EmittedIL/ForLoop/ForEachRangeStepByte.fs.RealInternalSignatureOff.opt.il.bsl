




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

  .method public static void  f2(uint8 start) cil managed
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
    IL_0007:  br.s       IL_0011

    IL_0009:  ldc.i4.s   10
    IL_000b:  ldarg.0
    IL_000c:  sub
    IL_000d:  conv.u2
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
    IL_0019:  call       void assembly::set_c(uint8)
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

  .method public static void  f3(uint8 finish) cil managed
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
    IL_0006:  br.s       IL_000f

    IL_0008:  ldarg.0
    IL_0009:  ldc.i4.1
    IL_000a:  sub
    IL_000b:  conv.u2
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
    IL_0017:  call       void assembly::set_c(uint8)
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

  .method public static void  f4(uint8 start,
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
    IL_0006:  br.s       IL_000f

    IL_0008:  ldarg.1
    IL_0009:  ldarg.0
    IL_000a:  sub
    IL_000b:  conv.u2
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
    IL_0017:  call       void assembly::set_c(uint8)
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
    IL_0007:  br.s       IL_0013

    IL_0009:  ldc.i4.s   10
    IL_000b:  ldarg.0
    IL_000c:  sub
    IL_000d:  ldc.i4.2
    IL_000e:  div.un
    IL_000f:  conv.u2
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
    IL_001b:  call       void assembly::set_c(uint8)
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

  .method public static void  f8(uint8 step) cil managed
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
    IL_0011:  ldc.i4.s   9
    IL_0013:  ldarg.0
    IL_0014:  div.un
    IL_0015:  conv.u2
    IL_0016:  ldc.i4.1
    IL_0017:  add
    IL_0018:  stloc.0
    IL_0019:  ldc.i4.0
    IL_001a:  stloc.1
    IL_001b:  ldc.i4.1
    IL_001c:  stloc.2
    IL_001d:  br.s       IL_002d

    IL_001f:  ldloc.2
    IL_0020:  call       void assembly::set_c(uint8)
    IL_0025:  ldloc.2
    IL_0026:  ldarg.0
    IL_0027:  add
    IL_0028:  stloc.2
    IL_0029:  ldloc.1
    IL_002a:  ldc.i4.1
    IL_002b:  add
    IL_002c:  stloc.1
    IL_002d:  ldloc.1
    IL_002e:  ldloc.0
    IL_002f:  blt.un.s   IL_001f

    IL_0031:  ret
  } 

  .method public static void  f9(uint8 finish) cil managed
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
    IL_0006:  br.s       IL_0011

    IL_0008:  ldarg.0
    IL_0009:  ldc.i4.1
    IL_000a:  sub
    IL_000b:  ldc.i4.2
    IL_000c:  div.un
    IL_000d:  conv.u2
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
    IL_0019:  call       void assembly::set_c(uint8)
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

  .method public static void  f10(uint8 start,
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
    IL_0016:  br.s       IL_0021

    IL_0018:  ldarg.2
    IL_0019:  ldarg.2
    IL_001a:  sub
    IL_001b:  ldarg.1
    IL_001c:  div.un
    IL_001d:  conv.u2
    IL_001e:  ldc.i4.1
    IL_001f:  add
    IL_0020:  nop
    IL_0021:  stloc.0
    IL_0022:  ldc.i4.0
    IL_0023:  stloc.1
    IL_0024:  ldarg.2
    IL_0025:  stloc.2
    IL_0026:  br.s       IL_0036

    IL_0028:  ldloc.2
    IL_0029:  call       void assembly::set_c(uint8)
    IL_002e:  ldloc.2
    IL_002f:  ldarg.1
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

  .method public static void  f11(uint8 start,
                                  uint8 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (uint8 V_0,
             uint8 V_1,
             uint8 V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  ldarg.1
    IL_0003:  call       class [runtime]System.Collections.Generic.IEnumerable`1<uint8> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeByte(uint8,
                                                                                                                                                                          uint8,
                                                                                                                                                                          uint8)
    IL_0008:  pop
    IL_0009:  ldc.i4.0
    IL_000a:  stloc.0
    IL_000b:  ldc.i4.0
    IL_000c:  stloc.1
    IL_000d:  ldarg.0
    IL_000e:  stloc.2
    IL_000f:  br.s       IL_001f

    IL_0011:  ldloc.2
    IL_0012:  call       void assembly::set_c(uint8)
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
    .locals init (uint8 V_0,
             uint8 V_1,
             uint8 V_2)
    IL_0000:  ldc.i4.1
    IL_0001:  ldc.i4.0
    IL_0002:  ldc.i4.s   10
    IL_0004:  call       class [runtime]System.Collections.Generic.IEnumerable`1<uint8> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeByte(uint8,
                                                                                                                                                                          uint8,
                                                                                                                                                                          uint8)
    IL_0009:  pop
    IL_000a:  ldc.i4.0
    IL_000b:  stloc.0
    IL_000c:  ldc.i4.0
    IL_000d:  stloc.1
    IL_000e:  ldc.i4.1
    IL_000f:  stloc.2
    IL_0010:  br.s       IL_0020

    IL_0012:  ldloc.2
    IL_0013:  call       void assembly::set_c(uint8)
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






