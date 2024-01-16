




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
  .class abstract auto ansi sealed nested public EqualsMicroPerfAndCodeGenerationTests
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .method public static bool  f7() cil managed
    {
      
      .maxstack  5
      .locals init (bool V_0,
               uint8[] V_1,
               uint8[] V_2,
               int32 V_3)
      IL_0000:  ldc.i4.0
      IL_0001:  stloc.0
      IL_0002:  ldc.i4.0
      IL_0003:  ldc.i4.1
      IL_0004:  ldc.i4.s   100
      IL_0006:  call       class [runtime]System.Collections.Generic.IEnumerable`1<uint8> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeByte(uint8,
                                                                                                                                                                            uint8,
                                                                                                                                                                            uint8)
      IL_000b:  call       class [runtime]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::CreateSequence<uint8>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0010:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<uint8>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0015:  stloc.1
      IL_0016:  ldc.i4.0
      IL_0017:  ldc.i4.1
      IL_0018:  ldc.i4.s   100
      IL_001a:  call       class [runtime]System.Collections.Generic.IEnumerable`1<uint8> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeByte(uint8,
                                                                                                                                                                            uint8,
                                                                                                                                                                            uint8)
      IL_001f:  call       class [runtime]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::CreateSequence<uint8>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0024:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<uint8>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0029:  stloc.2
      IL_002a:  ldc.i4.0
      IL_002b:  stloc.3
      IL_002c:  br.s       IL_003a

      IL_002e:  ldloc.1
      IL_002f:  ldloc.2
      IL_0030:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericEqualityIntrinsic<uint8[]>(!!0,
                                                                                                                                     !!0)
      IL_0035:  stloc.0
      IL_0036:  ldloc.3
      IL_0037:  ldc.i4.1
      IL_0038:  add
      IL_0039:  stloc.3
      IL_003a:  ldloc.3
      IL_003b:  ldc.i4     0x989681
      IL_0040:  blt.s      IL_002e

      IL_0042:  ldloc.0
      IL_0043:  ret
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






