




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
      .locals init (char V_0,
               char V_1)
      IL_0000:  ldc.i4.0
      IL_0001:  stloc.0
      IL_0002:  ldc.i4.s   122
      IL_0004:  stloc.1
      IL_0005:  br.s       IL_0015

      IL_0007:  ldloc.1
      IL_0008:  call       void assembly::set_c(char)
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
      .locals init (uint32 V_0,
               char V_1)
      IL_0000:  ldc.i4.0
      IL_0001:  stloc.0
      IL_0002:  ldc.i4.s   97
      IL_0004:  stloc.1
      IL_0005:  br.s       IL_0015

      IL_0007:  ldloc.1
      IL_0008:  call       void assembly::set_c(char)
      IL_000d:  ldloc.1
      IL_000e:  ldc.i4.1
      IL_000f:  add
      IL_0010:  stloc.1
      IL_0011:  ldloc.0
      IL_0012:  ldc.i4.1
      IL_0013:  add
      IL_0014:  stloc.0
      IL_0015:  ldloc.0
      IL_0016:  ldc.i4.s   26
      IL_0018:  blt.un.s   IL_0007

      IL_001a:  ret
    } 

    .method public static void  constFinish(char start) cil managed
    {
      
      .maxstack  4
      .locals init (uint32 V_0,
               uint32 V_1,
               char V_2)
      IL_0000:  ldc.i4.s   122
      IL_0002:  ldarg.0
      IL_0003:  bge.un.s   IL_0009

      IL_0005:  ldc.i4.0
      IL_0006:  nop
      IL_0007:  br.s       IL_0011

      IL_0009:  ldc.i4.s   122
      IL_000b:  ldarg.0
      IL_000c:  sub
      IL_000d:  conv.u4
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
      IL_0019:  call       void assembly::set_c(char)
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

    .method public static void  constStart(char finish) cil managed
    {
      
      .maxstack  4
      .locals init (uint32 V_0,
               uint32 V_1,
               char V_2)
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.s   97
      IL_0003:  bge.un.s   IL_0009

      IL_0005:  ldc.i4.0
      IL_0006:  nop
      IL_0007:  br.s       IL_0011

      IL_0009:  ldarg.0
      IL_000a:  ldc.i4.s   97
      IL_000c:  sub
      IL_000d:  conv.u4
      IL_000e:  ldc.i4.1
      IL_000f:  add
      IL_0010:  nop
      IL_0011:  stloc.0
      IL_0012:  ldc.i4.0
      IL_0013:  stloc.1
      IL_0014:  ldc.i4.s   97
      IL_0016:  stloc.2
      IL_0017:  br.s       IL_0027

      IL_0019:  ldloc.2
      IL_001a:  call       void assembly::set_c(char)
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

    .method public static void  annotatedStart(char start,
                                               char finish) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
      
      .maxstack  4
      .locals init (uint32 V_0,
               uint32 V_1,
               char V_2)
      IL_0000:  ldarg.1
      IL_0001:  ldarg.0
      IL_0002:  bge.un.s   IL_0008

      IL_0004:  ldc.i4.0
      IL_0005:  nop
      IL_0006:  br.s       IL_000f

      IL_0008:  ldarg.1
      IL_0009:  ldarg.0
      IL_000a:  sub
      IL_000b:  conv.u4
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
      IL_0017:  call       void assembly::set_c(char)
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

    .method public static void  annotatedFinish(char start,
                                                char finish) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
      
      .maxstack  4
      .locals init (uint32 V_0,
               uint32 V_1,
               char V_2)
      IL_0000:  ldarg.1
      IL_0001:  ldarg.0
      IL_0002:  bge.un.s   IL_0008

      IL_0004:  ldc.i4.0
      IL_0005:  nop
      IL_0006:  br.s       IL_000f

      IL_0008:  ldarg.1
      IL_0009:  ldarg.0
      IL_000a:  sub
      IL_000b:  conv.u4
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
      IL_0017:  call       void assembly::set_c(char)
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

    .method public static void  inferredStartAndFinish(char start,
                                                       char finish) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
      
      .maxstack  4
      .locals init (uint32 V_0,
               uint32 V_1,
               char V_2)
      IL_0000:  ldarg.1
      IL_0001:  ldarg.0
      IL_0002:  bge.un.s   IL_0008

      IL_0004:  ldc.i4.0
      IL_0005:  nop
      IL_0006:  br.s       IL_000f

      IL_0008:  ldarg.1
      IL_0009:  ldarg.0
      IL_000a:  sub
      IL_000b:  conv.u4
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
      IL_0017:  call       void assembly::set_c(char)
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

  } 

  .field static assembly char c@1
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public specialname static char get_c() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     char assembly::c@1
    IL_0005:  ret
  } 

  .method public specialname static void set_c(char 'value') cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  stsfld     char assembly::c@1
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
    IL_0001:  stsfld     char assembly::c@1
    IL_0006:  ret
  } 

  .property char c()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .set void assembly::set_c(char)
    .get char assembly::get_c()
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






