




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
  .class auto ansi serializable nested public G`1<U>
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public static void  SMc<T>(!!T x,
                                       !!T y) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method public static void  SMt<T>(!!T x,
                                       !!T y) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

  } 

  .class abstract auto autochar serializable sealed nested assembly beforefieldinit specialname case7_nonEta@10
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .method assembly static void  Invoke(int32 x,
                                         int32 y) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

  } 

  .class abstract auto autochar serializable sealed nested assembly beforefieldinit specialname case8_etaCurried@13
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

  .class abstract auto autochar serializable sealed nested assembly beforefieldinit specialname case9_etaTupled@16
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

  .method public static class [runtime]System.Action`2<int32,int32> case7_nonEta() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ldftn      void assembly/case7_nonEta@10::Invoke(int32,
                                                                                  int32)
    IL_0007:  newobj     instance void class [runtime]System.Action`2<int32,int32>::.ctor(object,
                                                                                                 native int)
    IL_000c:  ret
  } 

  .method public static class [runtime]System.Action`2<int32,int32> case8_etaCurried() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ldftn      void assembly/case8_etaCurried@13::Invoke(int32,
                                                                                      int32)
    IL_0007:  newobj     instance void class [runtime]System.Action`2<int32,int32>::.ctor(object,
                                                                                                 native int)
    IL_000c:  ret
  } 

  .method public static class [runtime]System.Action`2<int32,int32> case9_etaTupled() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ldftn      void assembly/case9_etaTupled@16::Invoke(int32,
                                                                                     int32)
    IL_0007:  newobj     instance void class [runtime]System.Action`2<int32,int32>::.ctor(object,
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






