




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
    .method public static void  f7() cil managed
    {
      
      .maxstack  5
      .locals init (uint8[] V_0,
               uint8[] V_1,
               int32 V_2,
               uint8 V_3,
               int32 V_4)
      IL_0000:  nop
      IL_0001:  ldc.i4.s   101
      IL_0003:  newarr     [runtime]System.Byte
      IL_0008:  stloc.1
      IL_0009:  ldc.i4.0
      IL_000a:  stloc.2
      IL_000b:  ldc.i4.0
      IL_000c:  stloc.3
      IL_000d:  br.s       IL_001f

      IL_000f:  ldloc.1
      IL_0010:  ldloc.2
      IL_0011:  ldloc.3
      IL_0012:  stelem     [runtime]System.Byte
      IL_0017:  ldloc.3
      IL_0018:  ldc.i4.1
      IL_0019:  add
      IL_001a:  stloc.3
      IL_001b:  ldloc.2
      IL_001c:  ldc.i4.1
      IL_001d:  add
      IL_001e:  stloc.2
      IL_001f:  ldloc.2
      IL_0020:  ldloc.1
      IL_0021:  ldlen
      IL_0022:  conv.i4
      IL_0023:  blt.s      IL_000f

      IL_0025:  ldloc.1
      IL_0026:  stloc.0
      IL_0027:  ldc.i4.0
      IL_0028:  stloc.2
      IL_0029:  br.s       IL_0037

      IL_002b:  ldloc.0
      IL_002c:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericHashIntrinsic<uint8[]>(!!0)
      IL_0031:  stloc.s    V_4
      IL_0033:  ldloc.2
      IL_0034:  ldc.i4.1
      IL_0035:  add
      IL_0036:  stloc.2
      IL_0037:  ldloc.2
      IL_0038:  ldc.i4     0x989681
      IL_003d:  blt.s      IL_002b

      IL_003f:  ret
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






