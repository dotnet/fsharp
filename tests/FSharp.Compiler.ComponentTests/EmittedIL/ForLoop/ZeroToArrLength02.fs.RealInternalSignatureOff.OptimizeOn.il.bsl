




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
  .method public static void  f1(int32[] arr) cil managed
  {
    
    .maxstack  5
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.1
    IL_0002:  ldarg.0
    IL_0003:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Length<int32>(!!0[])
    IL_0008:  ldc.i4.1
    IL_0009:  sub
    IL_000a:  stloc.0
    IL_000b:  ldloc.0
    IL_000c:  ldloc.1
    IL_000d:  blt.s      IL_001d

    IL_000f:  ldarg.0
    IL_0010:  ldloc.1
    IL_0011:  ldloc.1
    IL_0012:  stelem.i4
    IL_0013:  ldloc.1
    IL_0014:  ldc.i4.1
    IL_0015:  add
    IL_0016:  stloc.1
    IL_0017:  ldloc.1
    IL_0018:  ldloc.0
    IL_0019:  ldc.i4.1
    IL_001a:  add
    IL_001b:  bne.un.s   IL_000f

    IL_001d:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly
       extends [runtime]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  8
    IL_0000:  ret
  } 

} 






