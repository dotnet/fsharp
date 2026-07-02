




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
  .class auto ansi serializable nested public C
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public specialname rtspecialname instance void  .ctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ret
    } 

    .method public hidebysig instance void M() cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

  } 

  .method public static void  'handler'() cil managed
  {
    
    .maxstack  8
    IL_0000:  ret
  } 

  .method public static class [runtime]System.Action caseUnitNonEta() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ldftn      void assembly::'handler'()
    IL_0007:  newobj     instance void [runtime]System.Action::.ctor(object,
                                                                            native int)
    IL_000c:  ret
  } 

  .method public static class [runtime]System.Action caseUnitEta() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ldftn      void assembly::'handler'()
    IL_0007:  newobj     instance void [runtime]System.Action::.ctor(object,
                                                                            native int)
    IL_000c:  ret
  } 

  .method public static class [runtime]System.Action caseUnitInstanceNonEta(class assembly/C c) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldftn      instance void assembly/C::M()
    IL_0007:  newobj     instance void [runtime]System.Action::.ctor(object,
                                                                            native int)
    IL_000c:  ret
  } 

  .method public static class [runtime]System.Action caseUnitInstanceEta(class assembly/C c) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldftn      instance void assembly/C::M()
    IL_0007:  newobj     instance void [runtime]System.Action::.ctor(object,
                                                                            native int)
    IL_000c:  ret
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






