




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
  .class abstract auto ansi sealed nested public HashMicroPerfAndCodeGenerationTests
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .method public static void  f8() cil managed
    {
      
      .maxstack  5
      .locals init (int32[] V_0,
               int32[] V_1,
               uint64 V_2,
               int32 V_3,
               int32 V_4)
      IL_0000:  nop
      IL_0001:  ldc.i4.s   101
      IL_0003:  conv.i8
      IL_0004:  conv.ovf.i.un
      IL_0005:  newarr     [runtime]System.Int32
      IL_000a:  stloc.1
      IL_000b:  ldc.i4.0
      IL_000c:  conv.i8
      IL_000d:  stloc.2
      IL_000e:  ldc.i4.0
      IL_000f:  stloc.3
      IL_0010:  br.s       IL_0020

      IL_0012:  ldloc.1
      IL_0013:  ldloc.2
      IL_0014:  conv.i
      IL_0015:  ldloc.3
      IL_0016:  stelem.i4
      IL_0017:  ldloc.3
      IL_0018:  ldc.i4.1
      IL_0019:  add
      IL_001a:  stloc.3
      IL_001b:  ldloc.2
      IL_001c:  ldc.i4.1
      IL_001d:  conv.i8
      IL_001e:  add
      IL_001f:  stloc.2
      IL_0020:  ldloc.2
      IL_0021:  ldc.i4.s   101
      IL_0023:  conv.i8
      IL_0024:  blt.un.s   IL_0012

      IL_0026:  ldloc.1
      IL_0027:  stloc.0
      IL_0028:  ldc.i4.0
      IL_0029:  stloc.3
      IL_002a:  br.s       IL_0038

      IL_002c:  ldloc.0
      IL_002d:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericHashIntrinsic<int32[]>(!!0)
      IL_0032:  stloc.s    V_4
      IL_0034:  ldloc.3
      IL_0035:  ldc.i4.1
      IL_0036:  add
      IL_0037:  stloc.3
      IL_0038:  ldloc.3
      IL_0039:  ldc.i4     0x989681
      IL_003e:  blt.s      IL_002c

      IL_0040:  ret
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






