




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
  .class abstract auto ansi sealed nested public HashMicroPerfAndCodeGenerationTests
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .method public static void  f8() cil managed
    {
      
      .maxstack  5
      .locals init (int32[] V_0,
               int32[] V_1,
               int32 V_2,
               int32 V_3)
      IL_0000:  nop
      IL_0001:  ldc.i4.s   101
      IL_0003:  newarr     [runtime]System.Int32
      IL_0008:  stloc.1
      IL_0009:  ldc.i4.0
      IL_000a:  stloc.2
      IL_000b:  ldc.i4.0
      IL_000c:  stloc.3
      IL_000d:  br.s       IL_001b

      IL_000f:  ldloc.1
      IL_0010:  ldloc.2
      IL_0011:  ldloc.3
      IL_0012:  stelem.i4
      IL_0013:  ldloc.3
      IL_0014:  ldc.i4.1
      IL_0015:  add
      IL_0016:  stloc.3
      IL_0017:  ldloc.2
      IL_0018:  ldc.i4.1
      IL_0019:  add
      IL_001a:  stloc.2
      IL_001b:  ldloc.2
      IL_001c:  ldc.i4.s   101
      IL_001e:  blt.un.s   IL_000f

      IL_0020:  ldloc.1
      IL_0021:  stloc.0
      IL_0022:  ldc.i4.0
      IL_0023:  stloc.2
      IL_0024:  br.s       IL_0031

      IL_0026:  ldloc.0
      IL_0027:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericHashIntrinsic<int32[]>(!!0)
      IL_002c:  stloc.3
      IL_002d:  ldloc.2
      IL_002e:  ldc.i4.1
      IL_002f:  add
      IL_0030:  stloc.2
      IL_0031:  ldloc.2
      IL_0032:  ldc.i4     0x989681
      IL_0037:  blt.s      IL_0026

      IL_0039:  ret
    } 

  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly$fsx
       extends [runtime]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  8
    IL_0000:  ret
  } 

} 






