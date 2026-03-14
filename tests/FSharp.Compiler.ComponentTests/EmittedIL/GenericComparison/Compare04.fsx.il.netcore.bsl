




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
    .method public static int32  f4_tuple5() cil managed
    {
      
      .maxstack  5
      .locals init (int32 V_0,
               int32 V_1,
               int32 V_2)
      IL_0000:  ldc.i4.1
      IL_0001:  stloc.0
      IL_0002:  nop
      IL_0003:  nop
      IL_0004:  ldc.i4.0
      IL_0005:  stloc.1
      IL_0006:  br         IL_0097

      IL_000b:  ldstr      "5"
      IL_0010:  ldstr      "5"
      IL_0015:  call       int32 [netstandard]System.String::CompareOrdinal(string,
                                                                            string)
      IL_001a:  stloc.2
      IL_001b:  ldloc.2
      IL_001c:  brfalse.s  IL_0025

      IL_001e:  ldloc.2
      IL_001f:  nop
      IL_0020:  br         IL_0092

      IL_0025:  ldc.r8     6.0999999999999996
      IL_002e:  ldc.r8     7.0999999999999996
      IL_0037:  clt
      IL_0039:  brfalse.s  IL_003f

      IL_003b:  ldc.i4.m1
      IL_003c:  nop
      IL_003d:  br.s       IL_0092

      IL_003f:  ldc.r8     6.0999999999999996
      IL_0048:  ldc.r8     7.0999999999999996
      IL_0051:  cgt
      IL_0053:  brfalse.s  IL_0059

      IL_0055:  ldc.i4.1
      IL_0056:  nop
      IL_0057:  br.s       IL_0092

      IL_0059:  ldc.r8     6.0999999999999996
      IL_0062:  ldc.r8     7.0999999999999996
      IL_006b:  ceq
      IL_006d:  brfalse.s  IL_0073

      IL_006f:  ldc.i4.0
      IL_0070:  nop
      IL_0071:  br.s       IL_0092

      IL_0073:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_0078:  ldc.r8     6.0999999999999996
      IL_0081:  ldc.r8     7.0999999999999996
      IL_008a:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericComparisonWithComparerIntrinsic<float64>(class [runtime]System.Collections.IComparer,
                                                                                                                                                    !!0,
                                                                                                                                                    !!0)
      IL_008f:  nop
      IL_0090:  br.s       IL_0092

      IL_0092:  stloc.0
      IL_0093:  ldloc.1
      IL_0094:  ldc.i4.1
      IL_0095:  add
      IL_0096:  stloc.1
      IL_0097:  ldloc.1
      IL_0098:  ldc.i4     0x989681
      IL_009d:  blt        IL_000b

      IL_00a2:  ldloc.0
      IL_00a3:  ret
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





