




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
  .method public static int32[]  loop(int32 n) cil managed
  {
    
    .maxstack  5
    .locals init (int32[] V_0,
             int32 V_1,
             int32 V_2,
             int32 V_3)
    IL_0000:  ldarg.0
    IL_0001:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::ZeroCreate<int32>(int32)
    IL_0006:  stloc.0
    IL_0007:  ldc.i4.m1
    IL_0008:  stloc.1
    IL_0009:  ldc.i4.1
    IL_000a:  stloc.3
    IL_000b:  ldarg.0
    IL_000c:  stloc.2
    IL_000d:  ldloc.2
    IL_000e:  ldloc.3
    IL_000f:  blt.s      IL_0027

    IL_0011:  ldloc.1
    IL_0012:  ldc.i4.1
    IL_0013:  add
    IL_0014:  stloc.1
    IL_0015:  ldloc.0
    IL_0016:  ldloc.1
    IL_0017:  ldloc.3
    IL_0018:  stelem     [runtime]System.Int32
    IL_001d:  ldloc.3
    IL_001e:  ldc.i4.1
    IL_001f:  add
    IL_0020:  stloc.3
    IL_0021:  ldloc.3
    IL_0022:  ldloc.2
    IL_0023:  ldc.i4.1
    IL_0024:  add
    IL_0025:  bne.un.s   IL_0011

    IL_0027:  ldloc.0
    IL_0028:  ret
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






