




.assembly extern runtime { }
.assembly extern FSharp.Core { }
.assembly extern netstandard
{
  .publickeytoken = (CC 7B 13 FF CD 2D DD 51 )                         
  .ver 2:1:0:0
}
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
  .field static assembly object o@19
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly bool lockTaken@1
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public specialname static object get_o() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     object assembly::o@19
    IL_0005:  ret
  } 

  .method assembly specialname static bool get_lockTaken@1() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     bool assembly::lockTaken@1
    IL_0005:  ret
  } 

  .method assembly specialname static void set_lockTaken@1(bool 'value') cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  stsfld     bool assembly::lockTaken@1
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

  .method assembly static void  staticInitialization@() cil managed
  {
    
    .maxstack  4
    IL_0000:  newobj     instance void [runtime]System.Object::.ctor()
    IL_0005:  stsfld     object assembly::o@19
    IL_000a:  ldc.i4.0
    IL_000b:  stsfld     bool assembly::lockTaken@1
    .try
    {
      IL_0010:  call       object assembly::get_o()
      IL_0015:  ldsflda    bool assembly::lockTaken@1
      IL_001a:  call       void [netstandard]System.Threading.Monitor::Enter(object,
                                                                             bool&)
      IL_001f:  leave.s    IL_0034

    }  
    finally
    {
      IL_0021:  call       bool assembly::get_lockTaken@1()
      IL_0026:  brfalse.s  IL_0033

      IL_0028:  call       object assembly::get_o()
      IL_002d:  call       void [netstandard]System.Threading.Monitor::Exit(object)
      IL_0032:  endfinally
      IL_0033:  endfinally
    }  
    IL_0034:  ret
  } 

  .property object o()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get object assembly::get_o()
  } 
  .property bool lockTaken@1()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .set void assembly::set_lockTaken@1(bool)
    .get bool assembly::get_lockTaken@1()
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






