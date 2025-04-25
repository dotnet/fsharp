




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
  .class abstract auto ansi sealed nested public Up
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .method public static void  constEmpty() cil managed
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

    .method public static void  constNonEmpty() cil managed
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

    .method public static void  constFinish(uint32 start) cil managed
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

    .method public static void  constStart(uint32 finish) cil managed
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

    .method public static void  annotatedStart(uint32 start,
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

    .method public static void  annotatedFinish(uint32 start,
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

    .method public static void  inferredStartAndFinish(uint32 start,
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

  } 

  .field static assembly uint32 c@1
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public specialname static uint32 get_c() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     uint32 assembly::c@1
    IL_0005:  ret
  } 

  .method public specialname static void set_c(uint32 'value') cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  stsfld     uint32 assembly::c@1
    IL_0006:  ret
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
    IL_0001:  stsfld     uint32 assembly::c@1
    IL_0006:  ret
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






