




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
  .class abstract auto ansi sealed nested public M
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class abstract auto ansi sealed nested public N
           extends [runtime]System.Object
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
      .method public specialname static int32 
              get_y() cil managed
      {
        
        .maxstack  8
        IL_0000:  ldsfld     int32 '<StartupCode$assembly>'.$assembly::y@7
        IL_0005:  ret
      } 

      .method public specialname static int32 
              get_z() cil managed
      {
        
        .maxstack  8
        IL_0000:  ldsfld     int32 '<StartupCode$assembly>'.$assembly::z@8
        IL_0005:  ret
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

    .method public specialname static int32 
            get_x() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     int32 '<StartupCode$assembly>'.$assembly::x@5
      IL_0005:  ret
    } 

    .property int32 x()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get int32 assembly/M::get_x()
    } 
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly
       extends [runtime]System.Object
{
  .field static assembly int32 x@5
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 y@7
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 z@8
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  4
    .locals init (int32 V_0,
             int32 V_1,
             int32 V_2)
    IL_0000:  ldstr      "1"
    IL_0005:  callvirt   instance int32 [runtime]System.String::get_Length()
    IL_000a:  dup
    IL_000b:  stsfld     int32 '<StartupCode$assembly>'.$assembly::x@5
    IL_0010:  stloc.0
    IL_0011:  call       int32 assembly/M::get_x()
    IL_0016:  ldstr      "2"
    IL_001b:  callvirt   instance int32 [runtime]System.String::get_Length()
    IL_0020:  add
    IL_0021:  dup
    IL_0022:  stsfld     int32 '<StartupCode$assembly>'.$assembly::y@7
    IL_0027:  stloc.1
    IL_0028:  call       int32 assembly/M/N::get_y()
    IL_002d:  ldstr      "3"
    IL_0032:  callvirt   instance int32 [runtime]System.String::get_Length()
    IL_0037:  add
    IL_0038:  dup
    IL_0039:  stsfld     int32 '<StartupCode$assembly>'.$assembly::z@8
    IL_003e:  stloc.2
    IL_003f:  ret
  } 

} 






