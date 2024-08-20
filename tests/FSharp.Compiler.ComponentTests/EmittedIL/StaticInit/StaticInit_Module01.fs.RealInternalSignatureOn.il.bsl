




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
  .class abstract auto ansi sealed nested public M
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class abstract auto ansi sealed nested public N
           extends [runtime]System.Object
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
      .field static assembly int32 y@7
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .field static assembly int32 z@8
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .method public specialname static int32 get_y() cil managed
      {
        
        .maxstack  8
        IL_0000:  ldsfld     int32 assembly/M/N::y@7
        IL_0005:  ret
      } 

      .method public specialname static int32 get_z() cil managed
      {
        
        .maxstack  8
        IL_0000:  ldsfld     int32 assembly/M/N::z@8
        IL_0005:  ret
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

      .method assembly specialname static void  staticInitialization@() cil managed
      {
        
        .maxstack  8
        IL_0000:  call       int32 assembly/M::get_x()
        IL_0005:  ldstr      "2"
        IL_000a:  callvirt   instance int32 [runtime]System.String::get_Length()
        IL_000f:  add
        IL_0010:  stsfld     int32 assembly/M/N::y@7
        IL_0015:  call       int32 assembly/M/N::get_y()
        IL_001a:  ldstr      "3"
        IL_001f:  callvirt   instance int32 [runtime]System.String::get_Length()
        IL_0024:  add
        IL_0025:  stsfld     int32 assembly/M/N::z@8
        IL_002a:  ret
      } 

      .property int32 y()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
        .get int32 assembly/M/N::get_y()
      } 
      .property int32 z()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
        .get int32 assembly/M/N::get_z()
      } 
    } 

    .field static assembly int32 x@5
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .method public specialname static int32 get_x() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     int32 assembly/M::x@5
      IL_0005:  ret
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
      IL_0000:  ldstr      "1"
      IL_0005:  callvirt   instance int32 [runtime]System.String::get_Length()
      IL_000a:  stsfld     int32 assembly/M::x@5
      IL_000f:  call       void assembly/M/N::staticInitialization@()
      IL_0014:  ret
    } 

    .property int32 x()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get int32 assembly/M::get_x()
    } 
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
    IL_0000:  call       void assembly/M::staticInitialization@()
    IL_0005:  ret
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






