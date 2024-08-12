




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
  .class abstract auto ansi sealed nested public CompareMicroPerfAndCodeGenerationTests
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .method public static int32  f7() cil managed
    {
      
      .maxstack  5
      .locals init (int32 V_0,
               uint8[] V_1,
               uint8[] V_2,
               uint16 V_3,
               uint8 V_4,
               uint8[] V_5,
               int32 V_6)
      IL_0000:  ldc.i4.1
      IL_0001:  stloc.0
      IL_0002:  ldc.i4.s   101
      IL_0004:  newarr     [runtime]System.Byte
      IL_0009:  stloc.2
      IL_000a:  ldc.i4.0
      IL_000b:  stloc.3
      IL_000c:  ldc.i4.0
      IL_000d:  stloc.s    V_4
      IL_000f:  br.s       IL_0020

      IL_0011:  ldloc.2
      IL_0012:  ldloc.3
      IL_0013:  ldloc.s    V_4
      IL_0015:  stelem.i1
      IL_0016:  ldloc.s    V_4
      IL_0018:  ldc.i4.1
      IL_0019:  add
      IL_001a:  stloc.s    V_4
      IL_001c:  ldloc.3
      IL_001d:  ldc.i4.1
      IL_001e:  add
      IL_001f:  stloc.3
      IL_0020:  ldloc.3
      IL_0021:  ldc.i4.s   101
      IL_0023:  blt.un.s   IL_0011

      IL_0025:  ldloc.2
      IL_0026:  stloc.1
      IL_0027:  ldc.i4.s   101
      IL_0029:  newarr     [runtime]System.Byte
      IL_002e:  stloc.s    V_5
      IL_0030:  ldc.i4.0
      IL_0031:  stloc.3
      IL_0032:  ldc.i4.0
      IL_0033:  stloc.s    V_4
      IL_0035:  br.s       IL_0047

      IL_0037:  ldloc.s    V_5
      IL_0039:  ldloc.3
      IL_003a:  ldloc.s    V_4
      IL_003c:  stelem.i1
      IL_003d:  ldloc.s    V_4
      IL_003f:  ldc.i4.1
      IL_0040:  add
      IL_0041:  stloc.s    V_4
      IL_0043:  ldloc.3
      IL_0044:  ldc.i4.1
      IL_0045:  add
      IL_0046:  stloc.3
      IL_0047:  ldloc.3
      IL_0048:  ldc.i4.s   101
      IL_004a:  blt.un.s   IL_0037

      IL_004c:  ldloc.s    V_5
      IL_004e:  stloc.2
      IL_004f:  ldc.i4.0
      IL_0050:  stloc.s    V_6
      IL_0052:  br.s       IL_0062

      IL_0054:  ldloc.1
      IL_0055:  ldloc.2
      IL_0056:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericComparisonIntrinsic<uint8[]>(!!0,
                                                                                                                                        !!0)
      IL_005b:  stloc.0
      IL_005c:  ldloc.s    V_6
      IL_005e:  ldc.i4.1
      IL_005f:  add
      IL_0060:  stloc.s    V_6
      IL_0062:  ldloc.s    V_6
      IL_0064:  ldc.i4     0x989681
      IL_0069:  blt.s      IL_0054

      IL_006b:  ldloc.0
      IL_006c:  ret
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






