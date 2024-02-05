




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
               int32 V_3,
               uint8 V_4,
               uint8[] V_5)
      IL_0000:  ldc.i4.1
      IL_0001:  stloc.0
      IL_0002:  ldc.i4.s   101
      IL_0004:  newarr     [runtime]System.Byte
      IL_0009:  stloc.2
      IL_000a:  ldc.i4.0
      IL_000b:  stloc.3
      IL_000c:  ldc.i4.0
      IL_000d:  stloc.s    V_4
      IL_000f:  br.s       IL_0024

      IL_0011:  ldloc.2
      IL_0012:  ldloc.3
      IL_0013:  ldloc.s    V_4
      IL_0015:  stelem     [runtime]System.Byte
      IL_001a:  ldloc.s    V_4
      IL_001c:  ldc.i4.1
      IL_001d:  add
      IL_001e:  stloc.s    V_4
      IL_0020:  ldloc.3
      IL_0021:  ldc.i4.1
      IL_0022:  add
      IL_0023:  stloc.3
      IL_0024:  ldloc.3
      IL_0025:  ldloc.2
      IL_0026:  ldlen
      IL_0027:  conv.i4
      IL_0028:  blt.s      IL_0011

      IL_002a:  ldloc.2
      IL_002b:  stloc.1
      IL_002c:  ldc.i4.s   101
      IL_002e:  newarr     [runtime]System.Byte
      IL_0033:  stloc.s    V_5
      IL_0035:  ldc.i4.0
      IL_0036:  stloc.3
      IL_0037:  ldc.i4.0
      IL_0038:  stloc.s    V_4
      IL_003a:  br.s       IL_0050

      IL_003c:  ldloc.s    V_5
      IL_003e:  ldloc.3
      IL_003f:  ldloc.s    V_4
      IL_0041:  stelem     [runtime]System.Byte
      IL_0046:  ldloc.s    V_4
      IL_0048:  ldc.i4.1
      IL_0049:  add
      IL_004a:  stloc.s    V_4
      IL_004c:  ldloc.3
      IL_004d:  ldc.i4.1
      IL_004e:  add
      IL_004f:  stloc.3
      IL_0050:  ldloc.3
      IL_0051:  ldloc.s    V_5
      IL_0053:  ldlen
      IL_0054:  conv.i4
      IL_0055:  blt.s      IL_003c

      IL_0057:  ldloc.s    V_5
      IL_0059:  stloc.2
      IL_005a:  ldc.i4.0
      IL_005b:  stloc.3
      IL_005c:  br.s       IL_006a

      IL_005e:  ldloc.1
      IL_005f:  ldloc.2
      IL_0060:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericComparisonIntrinsic<uint8[]>(!!0,
                                                                                                                                        !!0)
      IL_0065:  stloc.0
      IL_0066:  ldloc.3
      IL_0067:  ldc.i4.1
      IL_0068:  add
      IL_0069:  stloc.3
      IL_006a:  ldloc.3
      IL_006b:  ldc.i4     0x989681
      IL_0070:  blt.s      IL_005e

      IL_0072:  ldloc.0
      IL_0073:  ret
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






