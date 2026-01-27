




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
    
    .maxstack  4
    .locals init (class [runtime]System.Exception V_0,
             object V_1,
             class [runtime]System.Exception V_2,
             class [runtime]System.Exception V_3)
    .try
    {
      IL_0000:  nop
      IL_0001:  leave.s    IL_0028

    }  
    catch [runtime]System.Object 
    {
      IL_0003:  stloc.1
      IL_0004:  ldloc.1
      IL_0005:  isinst     [runtime]System.Exception
      IL_000a:  dup
      IL_000b:  brtrue.s   IL_0014

      IL_000d:  pop
      IL_000e:  ldloc.1
      IL_000f:  newobj     instance void [runtime]System.Runtime.CompilerServices.RuntimeWrappedException::.ctor(object)
      IL_0014:  stloc.0
      IL_0015:  ldloc.0
      IL_0016:  stloc.2
      IL_0017:  ldloc.2
      IL_0018:  callvirt   instance int32 [runtime]System.Object::GetHashCode()
      IL_001d:  ldc.i4.0
      IL_001e:  ceq
      IL_0020:  brfalse.s  IL_0026

      IL_0022:  ldloc.0
      IL_0023:  stloc.3
      IL_0024:  leave.s    IL_0028

      IL_0026:  leave.s    IL_0028

    }  
    IL_0028:  ret
  } 

} 





