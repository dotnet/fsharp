




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
    .field assembly int32 k
    .method public specialname rtspecialname instance void  .ctor(int32 k) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ldarg.0
      IL_0009:  ldarg.1
      IL_000a:  stfld      int32 assembly/C::k
      IL_000f:  ret
    } 

    .method public hidebysig instance void AddC(int32 x, int32 y) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method public hidebysig instance void AddT(int32 x, int32 y) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method public hidebysig virtual instance void V(int32 x, int32 y) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ret
    } 

  } 

  .class abstract auto autochar serializable sealed nested assembly beforefieldinit specialname case20_nonEta@12
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .method assembly static void  Invoke(int32 delegateArg0,
                                         int32 delegateArg1) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

  } 

  .class abstract auto autochar serializable sealed nested assembly beforefieldinit specialname case4_etaCurried@15
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .method assembly static void  Invoke(int32 a,
                                         int32 b) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

  } 

  .class abstract auto autochar serializable sealed nested assembly beforefieldinit specialname case34_etaTupled@18
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .method assembly static void  Invoke(int32 a,
                                         int32 b) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

  } 

  .method public static class [runtime]System.Action`2<int32,int32> case20_nonEta(class assembly/C o) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ldftn      void assembly/case20_nonEta@12::Invoke(int32,
                                                                              int32)
    IL_0007:  newobj     instance void class [runtime]System.Action`2<int32,int32>::.ctor(object,
                                                                                           native int)
    IL_000c:  ret
  } 

  .method public static class [runtime]System.Action`2<int32,int32> case4_etaCurried(class assembly/C o) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ldftn      void assembly/case4_etaCurried@15::Invoke(int32,
                                                                                 int32)
    IL_0007:  newobj     instance void class [runtime]System.Action`2<int32,int32>::.ctor(object,
                                                                                           native int)
    IL_000c:  ret
  } 

  .method public static class [runtime]System.Action`2<int32,int32> case34_etaTupled(class assembly/C o) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ldftn      void assembly/case34_etaTupled@18::Invoke(int32,
                                                                                 int32)
    IL_0007:  newobj     instance void class [runtime]System.Action`2<int32,int32>::.ctor(object,
                                                                                           native int)
    IL_000c:  ret
  } 

  .method public static class [runtime]System.Action`2<int32,int32> case21_virtual(class assembly/C o) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  dup
    IL_0002:  ldvirtftn  instance void assembly/C::V(int32,
                                                                   int32)
    IL_0008:  newobj     instance void class [runtime]System.Action`2<int32,int32>::.ctor(object,
                                                                                           native int)
    IL_000d:  ret
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






