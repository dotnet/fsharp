




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

  .class abstract auto autochar serializable sealed nested assembly beforefieldinit specialname caseUnitNonEta@11
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .method assembly static void  Invoke() cil managed
    {
      
      .maxstack  8
      IL_0000:  call       void assembly::'handler'()
      IL_0005:  nop
      IL_0006:  ret
    } 

  } 

  .class abstract auto autochar serializable sealed nested assembly beforefieldinit specialname caseUnitEta@14
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .method assembly static void  Invoke() cil managed
    {
      
      .maxstack  8
      IL_0000:  call       void assembly::'handler'()
      IL_0005:  nop
      IL_0006:  ret
    } 

  } 

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname caseUnitInstanceNonEta@17
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class assembly/C c
    .method public specialname rtspecialname instance void  .ctor(class assembly/C c) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class assembly/C assembly/caseUnitInstanceNonEta@17::c
      IL_0007:  ldarg.0
      IL_0008:  call       instance void [runtime]System.Object::.ctor()
      IL_000d:  ret
    } 

    .method assembly hidebysig instance void Invoke() cil managed
    {
      
      .maxstack  5
      .locals init (class assembly/C V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class assembly/C assembly/caseUnitInstanceNonEta@17::c
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  callvirt   instance void assembly/C::M()
      IL_000d:  nop
      IL_000e:  ret
    } 

  } 

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname caseUnitInstanceEta@20
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class assembly/C c
    .method public specialname rtspecialname instance void  .ctor(class assembly/C c) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class assembly/C assembly/caseUnitInstanceEta@20::c
      IL_0007:  ldarg.0
      IL_0008:  call       instance void [runtime]System.Object::.ctor()
      IL_000d:  ret
    } 

    .method assembly hidebysig instance void Invoke() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class assembly/C assembly/caseUnitInstanceEta@20::c
      IL_0006:  callvirt   instance void assembly/C::M()
      IL_000b:  nop
      IL_000c:  ret
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
    IL_0001:  ldftn      void assembly/caseUnitNonEta@11::Invoke()
    IL_0007:  newobj     instance void [runtime]System.Action::.ctor(object,
                                                                            native int)
    IL_000c:  ret
  } 

  .method public static class [runtime]System.Action caseUnitEta() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ldftn      void assembly/caseUnitEta@14::Invoke()
    IL_0007:  newobj     instance void [runtime]System.Action::.ctor(object,
                                                                            native int)
    IL_000c:  ret
  } 

  .method public static class [runtime]System.Action caseUnitInstanceNonEta(class assembly/C c) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void assembly/caseUnitInstanceNonEta@17::.ctor(class assembly/C)
    IL_0006:  ldftn      instance void assembly/caseUnitInstanceNonEta@17::Invoke()
    IL_000c:  newobj     instance void [runtime]System.Action::.ctor(object,
                                                                            native int)
    IL_0011:  ret
  } 

  .method public static class [runtime]System.Action caseUnitInstanceEta(class assembly/C c) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void assembly/caseUnitInstanceEta@20::.ctor(class assembly/C)
    IL_0006:  ldftn      instance void assembly/caseUnitInstanceEta@20::Invoke()
    IL_000c:  newobj     instance void [runtime]System.Action::.ctor(object,
                                                                            native int)
    IL_0011:  ret
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






