




.assembly extern runtime { }
.assembly extern FSharp.Core { }
.assembly extern netstandard
{
  .publickeytoken = (CC 7B 13 FF CD 2D DD 51 )                         
  .ver 2:1:0:0
}
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
    .method public static void  f4_tuple5() cil managed
    {
      
      .maxstack  5
      .locals init (int32 V_0,
               int32 V_1)
      IL_0000:  ldc.i4.1
      IL_0001:  stloc.0
      IL_0002:  nop
      IL_0003:  ldc.i4.0
      IL_0004:  stloc.1
      IL_0005:  br.s       IL_001f

      IL_0007:  ldc.i4     0x483
      IL_000c:  ldc.i4.s   99
      IL_000e:  ldstr      "5"
      IL_0013:  callvirt   instance int32 [netstandard]System.Object::GetHashCode()
      IL_0018:  xor
      IL_0019:  xor
      IL_001a:  stloc.0
      IL_001b:  ldloc.1
      IL_001c:  ldc.i4.1
      IL_001d:  add
      IL_001e:  stloc.1
      IL_001f:  ldloc.1
      IL_0020:  ldc.i4     0x989681
      IL_0025:  blt.s      IL_0007

      IL_0027:  ret
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






