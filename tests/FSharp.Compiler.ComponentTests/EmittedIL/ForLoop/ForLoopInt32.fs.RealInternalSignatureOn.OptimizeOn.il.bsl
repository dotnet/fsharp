




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
  .class abstract auto ansi sealed nested public Down
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .method public static void  constEmpty() cil managed
    {
      
      .maxstack  5
      .locals init (int32 V_0,
               int32 V_1)
      IL_0000:  ldc.i4.1
      IL_0001:  stloc.1
      IL_0002:  ldc.i4.s   10
      IL_0004:  stloc.0
      IL_0005:  ldloc.0
      IL_0006:  ldloc.1
      IL_0007:  bgt.s      IL_0019

      IL_0009:  ldloc.1
      IL_000a:  call       void assembly::set_c(int32)
      IL_000f:  ldloc.1
      IL_0010:  ldc.i4.1
      IL_0011:  sub
      IL_0012:  stloc.1
      IL_0013:  ldloc.1
      IL_0014:  ldloc.0
      IL_0015:  ldc.i4.1
      IL_0016:  sub
      IL_0017:  bne.un.s   IL_0009

      IL_0019:  ret
    } 

    .method public static void  constNonEmpty() cil managed
    {
      
      .maxstack  5
      .locals init (int32 V_0,
               int32 V_1)
      IL_0000:  ldc.i4.s   10
      IL_0002:  stloc.1
      IL_0003:  ldc.i4.1
      IL_0004:  stloc.0
      IL_0005:  ldloc.0
      IL_0006:  ldloc.1
      IL_0007:  bgt.s      IL_0019

      IL_0009:  ldloc.1
      IL_000a:  call       void assembly::set_c(int32)
      IL_000f:  ldloc.1
      IL_0010:  ldc.i4.1
      IL_0011:  sub
      IL_0012:  stloc.1
      IL_0013:  ldloc.1
      IL_0014:  ldloc.0
      IL_0015:  ldc.i4.1
      IL_0016:  sub
      IL_0017:  bne.un.s   IL_0009

      IL_0019:  ret
    } 

    .method public static void  constFinish(int32 start) cil managed
    {
      
      .maxstack  5
      .locals init (int32 V_0,
               int32 V_1)
      IL_0000:  ldarg.0
      IL_0001:  stloc.1
      IL_0002:  ldc.i4.1
      IL_0003:  stloc.0
      IL_0004:  ldloc.0
      IL_0005:  ldloc.1
      IL_0006:  bgt.s      IL_0018

      IL_0008:  ldloc.1
      IL_0009:  call       void assembly::set_c(int32)
      IL_000e:  ldloc.1
      IL_000f:  ldc.i4.1
      IL_0010:  sub
      IL_0011:  stloc.1
      IL_0012:  ldloc.1
      IL_0013:  ldloc.0
      IL_0014:  ldc.i4.1
      IL_0015:  sub
      IL_0016:  bne.un.s   IL_0008

      IL_0018:  ret
    } 

    .method public static void  constStart(int32 finish) cil managed
    {
      
      .maxstack  5
      .locals init (int32 V_0,
               int32 V_1)
      IL_0000:  ldc.i4.s   10
      IL_0002:  stloc.1
      IL_0003:  ldarg.0
      IL_0004:  stloc.0
      IL_0005:  ldloc.0
      IL_0006:  ldloc.1
      IL_0007:  bgt.s      IL_0019

      IL_0009:  ldloc.1
      IL_000a:  call       void assembly::set_c(int32)
      IL_000f:  ldloc.1
      IL_0010:  ldc.i4.1
      IL_0011:  sub
      IL_0012:  stloc.1
      IL_0013:  ldloc.1
      IL_0014:  ldloc.0
      IL_0015:  ldc.i4.1
      IL_0016:  sub
      IL_0017:  bne.un.s   IL_0009

      IL_0019:  ret
    } 

    .method public static void  annotatedStart(int32 start,
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
      IL_0006:  bgt.s      IL_0018

      IL_0008:  ldloc.1
      IL_0009:  call       void assembly::set_c(int32)
      IL_000e:  ldloc.1
      IL_000f:  ldc.i4.1
      IL_0010:  sub
      IL_0011:  stloc.1
      IL_0012:  ldloc.1
      IL_0013:  ldloc.0
      IL_0014:  ldc.i4.1
      IL_0015:  sub
      IL_0016:  bne.un.s   IL_0008

      IL_0018:  ret
    } 

    .method public static void  annotatedFinish(int32 start,
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
      IL_0006:  bgt.s      IL_0018

      IL_0008:  ldloc.1
      IL_0009:  call       void assembly::set_c(int32)
      IL_000e:  ldloc.1
      IL_000f:  ldc.i4.1
      IL_0010:  sub
      IL_0011:  stloc.1
      IL_0012:  ldloc.1
      IL_0013:  ldloc.0
      IL_0014:  ldc.i4.1
      IL_0015:  sub
      IL_0016:  bne.un.s   IL_0008

      IL_0018:  ret
    } 

    .method public static void  inferredStartAndFinish(int32 start,
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
      IL_0006:  bgt.s      IL_0018

      IL_0008:  ldloc.1
      IL_0009:  call       void assembly::set_c(int32)
      IL_000e:  ldloc.1
      IL_000f:  ldc.i4.1
      IL_0010:  sub
      IL_0011:  stloc.1
      IL_0012:  ldloc.1
      IL_0013:  ldloc.0
      IL_0014:  ldc.i4.1
      IL_0015:  sub
      IL_0016:  bne.un.s   IL_0008

      IL_0018:  ret
    } 

    .method public static void  unconstrainedStartAndFinish(int32 start,
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
      IL_0006:  bgt.s      IL_0019

      IL_0008:  ldloc.1
      IL_0009:  call       void assembly::f<int32>(!!0)
      IL_000e:  nop
      IL_000f:  ldloc.1
      IL_0010:  ldc.i4.1
      IL_0011:  sub
      IL_0012:  stloc.1
      IL_0013:  ldloc.1
      IL_0014:  ldloc.0
      IL_0015:  ldc.i4.1
      IL_0016:  sub
      IL_0017:  bne.un.s   IL_0008

      IL_0019:  ret
    } 

    .method public static void  boxedLoopVar(int32 start,
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
      IL_0006:  bgt.s      IL_001e

      IL_0008:  ldloc.1
      IL_0009:  box        [runtime]System.Int32
      IL_000e:  call       void assembly::forceBoxing(object)
      IL_0013:  nop
      IL_0014:  ldloc.1
      IL_0015:  ldc.i4.1
      IL_0016:  sub
      IL_0017:  stloc.1
      IL_0018:  ldloc.1
      IL_0019:  ldloc.0
      IL_001a:  ldc.i4.1
      IL_001b:  sub
      IL_001c:  bne.un.s   IL_0008

      IL_001e:  ret
    } 

  } 

  .class abstract auto ansi sealed nested public Up
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .method public static void  constEmpty() cil managed
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

    .method public static void  constNonEmpty() cil managed
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

    .method public static void  constFinish(int32 start) cil managed
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

    .method public static void  constStart(int32 finish) cil managed
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

    .method public static void  annotatedStart(int32 start,
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

    .method public static void  annotatedFinish(int32 start,
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

    .method public static void  inferredStartAndFinish(int32 start,
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

    .method public static void  unconstrainedStartAndFinish(int32 start,
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
      IL_0006:  blt.s      IL_0019

      IL_0008:  ldloc.1
      IL_0009:  call       void assembly::f<int32>(!!0)
      IL_000e:  nop
      IL_000f:  ldloc.1
      IL_0010:  ldc.i4.1
      IL_0011:  add
      IL_0012:  stloc.1
      IL_0013:  ldloc.1
      IL_0014:  ldloc.0
      IL_0015:  ldc.i4.1
      IL_0016:  add
      IL_0017:  bne.un.s   IL_0008

      IL_0019:  ret
    } 

    .method public static void  boxedLoopVar(int32 start,
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
      IL_0006:  blt.s      IL_001e

      IL_0008:  ldloc.1
      IL_0009:  box        [runtime]System.Int32
      IL_000e:  call       void assembly::forceBoxing(object)
      IL_0013:  nop
      IL_0014:  ldloc.1
      IL_0015:  ldc.i4.1
      IL_0016:  add
      IL_0017:  stloc.1
      IL_0018:  ldloc.1
      IL_0019:  ldloc.0
      IL_001a:  ldc.i4.1
      IL_001b:  add
      IL_001c:  bne.un.s   IL_0008

      IL_001e:  ret
    } 

  } 

  .field static assembly int32 c@3
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public specialname static int32 get_c() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     int32 assembly::c@3
    IL_0005:  ret
  } 

  .method public specialname static void set_c(int32 'value') cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  stsfld     int32 assembly::c@3
    IL_0006:  ret
  } 

  .method public static void  f<a>(!!a x) cil managed noinlining
  {
    
    .maxstack  8
    IL_0000:  ret
  } 

  .method public static void  forceBoxing(object x) cil managed noinlining
  {
    
    .maxstack  8
    IL_0000:  ret
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
    IL_0001:  stsfld     int32 assembly::c@3
    IL_0006:  ret
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






