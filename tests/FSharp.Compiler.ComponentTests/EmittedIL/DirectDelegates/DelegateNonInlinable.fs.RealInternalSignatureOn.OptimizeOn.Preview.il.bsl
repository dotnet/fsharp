




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
  .class auto ansi serializable nested public S
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public static int32  AccS(int32 x,
                                      int32 y) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.NoCompilerInliningAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  add
      IL_0003:  ret
    } 

  } 

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

    .method public hidebysig instance int32 AccC(int32 x, int32 y) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.NoCompilerInliningAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.2
      IL_0002:  add
      IL_0003:  ldarg.0
      IL_0004:  ldfld      int32 assembly/C::k
      IL_0009:  add
      IL_000a:  ret
    } 

    .method public hidebysig instance !!T GPick<T>(!!T x, !!T y) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.NoCompilerInliningAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } 

  } 

  .class abstract auto autochar serializable sealed nested assembly beforefieldinit specialname niEtaTupled@26
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .method assembly static int32  Invoke(int32 a,
                                          int32 b) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  call       int32 assembly::accTupled(int32,
                                                                 int32)
      IL_0007:  ret
    } 

  } 

  .class abstract auto autochar serializable sealed nested assembly beforefieldinit specialname niTrivialNonEta@63
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .method assembly static int32  Invoke(int32 x,
                                          int32 y) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  add
      IL_0003:  ret
    } 

  } 

  .class abstract auto autochar serializable sealed nested assembly beforefieldinit specialname niTrivialEta@65
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .method assembly static int32  Invoke(int32 a,
                                          int32 b) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  add
      IL_0003:  ret
    } 

  } 

  .method public static int32  accCurried(int32 x,
                                          int32 y) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.NoCompilerInliningAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  add
    IL_0003:  ret
  } 

  .method public static int32  accTupled(int32 x,
                                         int32 y) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.NoCompilerInliningAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  add
    IL_0003:  ret
  } 

  .method public static class [runtime]System.Func`3<int32,int32,int32> niNonEta() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ldftn      int32 assembly::accCurried(int32,
                                                                int32)
    IL_0007:  newobj     instance void class [runtime]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_000c:  ret
  } 

  .method public static class [runtime]System.Func`3<int32,int32,int32> niEtaCurried() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ldftn      int32 assembly::accCurried(int32,
                                                                int32)
    IL_0007:  newobj     instance void class [runtime]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_000c:  ret
  } 

  .method public static class [runtime]System.Func`3<int32,int32,int32> niEtaTupled() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ldftn      int32 assembly/niEtaTupled@26::Invoke(int32,
                                                                           int32)
    IL_0007:  newobj     instance void class [runtime]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_000c:  ret
  } 

  .method public static class [runtime]System.Func`3<int32,int32,int32> niStaticNonEta() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ldftn      int32 assembly/S::AccS(int32,
                                                            int32)
    IL_0007:  newobj     instance void class [runtime]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_000c:  ret
  } 

  .method public static class [runtime]System.Func`3<int32,int32,int32> niStaticEta() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ldftn      int32 assembly/S::AccS(int32,
                                                            int32)
    IL_0007:  newobj     instance void class [runtime]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_000c:  ret
  } 

  .method public static class [runtime]System.Func`3<int32,int32,int32> niInstanceNonEta(class assembly/C o) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldftn      instance int32 assembly/C::AccC(int32,
                                                                     int32)
    IL_0007:  newobj     instance void class [runtime]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_000c:  ret
  } 

  .method public static class [runtime]System.Func`3<int32,int32,int32> niInstanceEta(class assembly/C o) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldftn      instance int32 assembly/C::AccC(int32,
                                                                     int32)
    IL_0007:  newobj     instance void class [runtime]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_000c:  ret
  } 

  .method public static class [runtime]System.Func`3<int32,int32,int32> niGenericInstanceNonEta(class assembly/C o) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldftn      instance !!0 assembly/C::GPick<int32>(!!0,
                                                                           !!0)
    IL_0007:  newobj     instance void class [runtime]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_000c:  ret
  } 

  .method public static class [runtime]System.Func`3<int32,int32,int32> niGenericInstanceEta(class assembly/C o) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldftn      instance !!0 assembly/C::GPick<int32>(!!0,
                                                                           !!0)
    IL_0007:  newobj     instance void class [runtime]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_000c:  ret
  } 

  .method public static int32  trivial(int32 x,
                                       int32 y) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  add
    IL_0003:  ret
  } 

  .method public static class [runtime]System.Func`3<int32,int32,int32> niTrivialNonEta() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ldftn      int32 assembly/niTrivialNonEta@63::Invoke(int32,
                                                                               int32)
    IL_0007:  newobj     instance void class [runtime]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_000c:  ret
  } 

  .method public static class [runtime]System.Func`3<int32,int32,int32> niTrivialEta() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ldftn      int32 assembly/niTrivialEta@65::Invoke(int32,
                                                                            int32)
    IL_0007:  newobj     instance void class [runtime]System.Func`3<int32,int32,int32>::.ctor(object,
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






