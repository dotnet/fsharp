




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
    .method public static int32  f8() cil managed
    {
      
      .maxstack  5
      .locals init (int32 V_0,
               int32[] V_1,
               int32[] V_2,
               uint64 V_3,
               int32 V_4,
               int32[] V_5)
      IL_0000:  ldc.i4.1
      IL_0001:  stloc.0
      IL_0002:  ldc.i4.s   101
      IL_0004:  conv.i8
      IL_0005:  conv.ovf.i.un
      IL_0006:  newarr     [runtime]System.Int32
      IL_000b:  stloc.2
      IL_000c:  ldc.i4.0
      IL_000d:  conv.i8
      IL_000e:  stloc.3
      IL_000f:  ldc.i4.0
      IL_0010:  stloc.s    V_4
      IL_0012:  br.s       IL_0025

      IL_0014:  ldloc.2
      IL_0015:  ldloc.3
      IL_0016:  conv.i
      IL_0017:  ldloc.s    V_4
      IL_0019:  stelem.i4
      IL_001a:  ldloc.s    V_4
      IL_001c:  ldc.i4.1
      IL_001d:  add
      IL_001e:  stloc.s    V_4
      IL_0020:  ldloc.3
      IL_0021:  ldc.i4.1
      IL_0022:  conv.i8
      IL_0023:  add
      IL_0024:  stloc.3
      IL_0025:  ldloc.3
      IL_0026:  ldc.i4.s   101
      IL_0028:  conv.i8
      IL_0029:  blt.un.s   IL_0014

      IL_002b:  ldloc.2
      IL_002c:  stloc.1
      IL_002d:  ldc.i4.s   101
      IL_002f:  conv.i8
      IL_0030:  conv.ovf.i.un
      IL_0031:  newarr     [runtime]System.Int32
      IL_0036:  stloc.s    V_5
      IL_0038:  ldc.i4.0
      IL_0039:  conv.i8
      IL_003a:  stloc.3
      IL_003b:  ldc.i4.0
      IL_003c:  stloc.s    V_4
      IL_003e:  br.s       IL_0052

      IL_0040:  ldloc.s    V_5
      IL_0042:  ldloc.3
      IL_0043:  conv.i
      IL_0044:  ldloc.s    V_4
      IL_0046:  stelem.i4
      IL_0047:  ldloc.s    V_4
      IL_0049:  ldc.i4.1
      IL_004a:  add
      IL_004b:  stloc.s    V_4
      IL_004d:  ldloc.3
      IL_004e:  ldc.i4.1
      IL_004f:  conv.i8
      IL_0050:  add
      IL_0051:  stloc.3
      IL_0052:  ldloc.3
      IL_0053:  ldc.i4.s   101
      IL_0055:  conv.i8
      IL_0056:  blt.un.s   IL_0040

      IL_0058:  ldloc.s    V_5
      IL_005a:  stloc.2
      IL_005b:  ldc.i4.0
      IL_005c:  stloc.s    V_4
      IL_005e:  br.s       IL_006e

      IL_0060:  ldloc.1
      IL_0061:  ldloc.2
      IL_0062:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericComparisonIntrinsic<int32[]>(!!0,
                                                                                                                                        !!0)
      IL_0067:  stloc.0
      IL_0068:  ldloc.s    V_4
      IL_006a:  ldc.i4.1
      IL_006b:  add
      IL_006c:  stloc.s    V_4
      IL_006e:  ldloc.s    V_4
      IL_0070:  ldc.i4     0x186a1
      IL_0075:  blt.s      IL_0060

      IL_0077:  ldloc.0
      IL_0078:  ret
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






