




.assembly extern runtime { }
.assembly extern FSharp.Core { }
.assembly extern netstandard
{
  .publickeytoken = (CC 7B 13 FF CD 2D DD 51 )                         
  .ver 2:0:0:0
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
  .class abstract auto ansi sealed nested public EqualsMicroPerfAndCodeGenerationTests
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .method public static bool  f4_tuple5() cil managed
    {
      
      .maxstack  4
      .locals init (bool V_0,
               int32 V_1)
      IL_0000:  ldc.i4.0
      IL_0001:  stloc.0
      IL_0002:  nop
      IL_0003:  nop
      IL_0004:  ldc.i4.0
      IL_0005:  stloc.1
      IL_0006:  br.s       IL_0038

      IL_0008:  nop
      IL_0009:  ldstr      "5"
      IL_000e:  ldstr      "5"
      IL_0013:  call       bool [netstandard]System.String::Equals(string,
                                                                   string)
      IL_0018:  brfalse.s  IL_0031

      IL_001a:  ldc.r8     6.0999999999999996
      IL_0023:  ldc.r8     7.0999999999999996
      IL_002c:  ceq
      IL_002e:  nop
      IL_002f:  br.s       IL_0033

      IL_0031:  ldc.i4.0
      IL_0032:  nop
      IL_0033:  stloc.0
      IL_0034:  ldloc.1
      IL_0035:  ldc.i4.1
      IL_0036:  add
      IL_0037:  stloc.1
      IL_0038:  ldloc.1
      IL_0039:  ldc.i4     0x989681
      IL_003e:  blt.s      IL_0008

      IL_0040:  ldloc.0
      IL_0041:  ret
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






