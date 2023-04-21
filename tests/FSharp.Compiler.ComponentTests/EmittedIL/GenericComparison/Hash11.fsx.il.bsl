




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
               int32 V_1,
               int32 V_2)
      IL_0000:  nop
      IL_0001:  ldc.i4.0
      IL_0002:  ldc.i4.1
      IL_0003:  ldc.i4.s   100
      IL_0005:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                             int32,
                                                                                                                                                                             int32)
      IL_000a:  call       class [runtime]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::CreateSequence<int32>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_000f:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<int32>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0014:  stloc.0
      IL_0015:  ldc.i4.0
      IL_0016:  stloc.1
      IL_0017:  br.s       IL_0024

      IL_0019:  ldloc.0
      IL_001a:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericHashIntrinsic<int32[]>(!!0)
      IL_001f:  stloc.2
      IL_0020:  ldloc.1
      IL_0021:  ldc.i4.1
      IL_0022:  add
      IL_0023:  stloc.1
      IL_0024:  ldloc.1
      IL_0025:  ldc.i4     0x989681
      IL_002a:  blt.s      IL_0019

      IL_002c:  ret
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






