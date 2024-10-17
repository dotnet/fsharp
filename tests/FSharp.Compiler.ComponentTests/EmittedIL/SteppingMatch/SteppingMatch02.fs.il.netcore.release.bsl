




.assembly extern runtime { }
.assembly extern FSharp.Core { }
.assembly extern runtime { }
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
  .method public static void  funcB<a,b>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`2<!!a,!!b> n) cil managed
  {
    
    .maxstack  3
    .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`2<!!a,!!b> V_0,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`2/Choice1Of2<!!a,!!b> V_1,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`2/Choice2Of2<!!a,!!b> V_2)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  isinst     class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`2/Choice2Of2<!!a,!!b>
    IL_0008:  brfalse.s  IL_000c

    IL_000a:  br.s       IL_001e

    IL_000c:  ldloc.0
    IL_000d:  castclass  class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`2/Choice1Of2<!!a,!!b>
    IL_0012:  stloc.1
    IL_0013:  ldstr      "B"
    IL_0018:  call       void [runtime]System.Console::WriteLine(string)
    IL_001d:  ret

    IL_001e:  ldloc.0
    IL_001f:  castclass  class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`2/Choice2Of2<!!a,!!b>
    IL_0024:  stloc.2
    IL_0025:  ldstr      "A"
    IL_002a:  call       void [runtime]System.Console::WriteLine(string)
    IL_002f:  ret
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






