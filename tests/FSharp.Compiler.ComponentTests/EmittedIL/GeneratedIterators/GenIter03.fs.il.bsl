




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
  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> squaresOfOneToTenC() cil managed
  {
    
    .maxstack  5
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             uint64 V_1,
             int32 V_2,
             int32 V_3)
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stloc.1
    IL_0003:  ldc.i4.0
    IL_0004:  stloc.2
    IL_0005:  br.s       IL_001d

    IL_0007:  ldloca.s   V_0
    IL_0009:  ldloc.2
    IL_000a:  stloc.3
    IL_000b:  ldloc.3
    IL_000c:  ldloc.3
    IL_000d:  mul
    IL_000e:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0013:  nop
    IL_0014:  ldloc.2
    IL_0015:  ldc.i4.1
    IL_0016:  add
    IL_0017:  stloc.2
    IL_0018:  ldloc.1
    IL_0019:  ldc.i4.1
    IL_001a:  conv.i8
    IL_001b:  add
    IL_001c:  stloc.1
    IL_001d:  ldloc.1
    IL_001e:  ldc.i4.s   11
    IL_0020:  conv.i8
    IL_0021:  blt.un.s   IL_0007

    IL_0023:  ldloca.s   V_0
    IL_0025:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_002a:  ret
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






