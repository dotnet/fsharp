




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
  .method public specialname static object 
          get_o() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     object '<StartupCode$assembly>'.$assembly::o@19
    IL_0005:  ret
  } 

  .property object o()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get object assembly::get_o()
  } 
} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly
       extends [runtime]System.Object
{
  .field static assembly object o@19
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  4
    .locals init (object V_0,
             object V_1,
             bool V_2)
    IL_0000:  newobj     instance void [runtime]System.Object::.ctor()
    IL_0005:  dup
    IL_0006:  stsfld     object '<StartupCode$assembly>'.$assembly::o@19
    IL_000b:  stloc.0
    IL_000c:  call       object assembly::get_o()
    IL_0011:  stloc.1
    IL_0012:  ldc.i4.0
    IL_0013:  stloc.2
    .try
    {
      IL_0014:  ldloc.1
      IL_0015:  ldloca.s   V_2
      IL_0017:  call       void [netstandard]System.Threading.Monitor::Enter(object,
                                                                             bool&)
      IL_001c:  leave.s    IL_0029

    }  
    finally
    {
      IL_001e:  ldloc.2
      IL_001f:  brfalse.s  IL_0028

      IL_0021:  ldloc.1
      IL_0022:  call       void [netstandard]System.Threading.Monitor::Exit(object)
      IL_0027:  endfinally
      IL_0028:  endfinally
    }  
    IL_0029:  ret
  } 

} 






