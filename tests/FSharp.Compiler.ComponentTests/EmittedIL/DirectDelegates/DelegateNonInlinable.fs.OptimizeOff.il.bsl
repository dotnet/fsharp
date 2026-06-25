




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

  .class abstract auto autochar serializable sealed nested assembly beforefieldinit specialname niNonEta@12
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .method assembly static int32  Invoke(int32 x,
                                          int32 y) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  call       int32 assembly::accCurried(int32,
                                                                  int32)
      IL_0007:  ret
    } 

  } 

  .class abstract auto autochar serializable sealed nested assembly beforefieldinit specialname niEtaCurried@15
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .method assembly static int32  Invoke(int32 a,
                                          int32 b) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  call       int32 assembly::accCurried(int32,
                                                                  int32)
      IL_0007:  ret
    } 

  } 

  .class abstract auto autochar serializable sealed nested assembly beforefieldinit specialname niEtaTupled@18
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

  .class abstract auto autochar serializable sealed nested assembly beforefieldinit specialname niStaticNonEta@25
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .method assembly static int32  Invoke(int32 x,
                                          int32 y) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  call       int32 assembly/S::AccS(int32,
                                                              int32)
      IL_0007:  ret
    } 

  } 

  .class abstract auto autochar serializable sealed nested assembly beforefieldinit specialname niStaticEta@28
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .method assembly static int32  Invoke(int32 a,
                                          int32 b) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  call       int32 assembly/S::AccS(int32,
                                                              int32)
      IL_0007:  ret
    } 

  } 

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname niInstanceNonEta@38
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class assembly/C o
    .method public specialname rtspecialname instance void  .ctor(class assembly/C o) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class assembly/C assembly/niInstanceNonEta@38::o
      IL_0007:  ldarg.0
      IL_0008:  call       instance void [runtime]System.Object::.ctor()
      IL_000d:  ret
    } 

    .method assembly hidebysig instance int32 Invoke(int32 delegateArg0, int32 delegateArg1) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class assembly/C assembly/niInstanceNonEta@38::o
      IL_0006:  ldarg.1
      IL_0007:  ldarg.2
      IL_0008:  tail.
      IL_000a:  callvirt   instance int32 assembly/C::AccC(int32,
                                                                       int32)
      IL_000f:  ret
    } 

  } 

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname niInstanceEta@41
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class assembly/C o
    .method public specialname rtspecialname instance void  .ctor(class assembly/C o) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class assembly/C assembly/niInstanceEta@41::o
      IL_0007:  ldarg.0
      IL_0008:  call       instance void [runtime]System.Object::.ctor()
      IL_000d:  ret
    } 

    .method assembly hidebysig instance int32 Invoke(int32 a, int32 b) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class assembly/C assembly/niInstanceEta@41::o
      IL_0006:  ldarg.1
      IL_0007:  ldarg.2
      IL_0008:  tail.
      IL_000a:  callvirt   instance int32 assembly/C::AccC(int32,
                                                                       int32)
      IL_000f:  ret
    } 

  } 

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname niGenericInstanceNonEta@44
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class assembly/C o
    .method public specialname rtspecialname instance void  .ctor(class assembly/C o) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class assembly/C assembly/niGenericInstanceNonEta@44::o
      IL_0007:  ldarg.0
      IL_0008:  call       instance void [runtime]System.Object::.ctor()
      IL_000d:  ret
    } 

    .method assembly hidebysig instance int32 Invoke(int32 delegateArg0, int32 delegateArg1) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class assembly/C assembly/niGenericInstanceNonEta@44::o
      IL_0006:  ldarg.1
      IL_0007:  ldarg.2
      IL_0008:  callvirt   instance !!0 assembly/C::GPick<int32>(!!0,
                                                                             !!0)
      IL_000d:  ret
    } 

  } 

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname niGenericInstanceEta@47
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class assembly/C o
    .method public specialname rtspecialname instance void  .ctor(class assembly/C o) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class assembly/C assembly/niGenericInstanceEta@47::o
      IL_0007:  ldarg.0
      IL_0008:  call       instance void [runtime]System.Object::.ctor()
      IL_000d:  ret
    } 

    .method assembly hidebysig instance int32 Invoke(int32 a, int32 b) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class assembly/C assembly/niGenericInstanceEta@47::o
      IL_0006:  ldarg.1
      IL_0007:  ldarg.2
      IL_0008:  callvirt   instance !!0 assembly/C::GPick<int32>(!!0,
                                                                             !!0)
      IL_000d:  ret
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
    IL_0001:  ldftn      int32 assembly/niNonEta@12::Invoke(int32,
                                                                        int32)
    IL_0007:  newobj     instance void class [runtime]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_000c:  ret
  } 

  .method public static class [runtime]System.Func`3<int32,int32,int32> niEtaCurried() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ldftn      int32 assembly/niEtaCurried@15::Invoke(int32,
                                                                            int32)
    IL_0007:  newobj     instance void class [runtime]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_000c:  ret
  } 

  .method public static class [runtime]System.Func`3<int32,int32,int32> niEtaTupled() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ldftn      int32 assembly/niEtaTupled@18::Invoke(int32,
                                                                           int32)
    IL_0007:  newobj     instance void class [runtime]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_000c:  ret
  } 

  .method public static class [runtime]System.Func`3<int32,int32,int32> niStaticNonEta() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ldftn      int32 assembly/niStaticNonEta@25::Invoke(int32,
                                                                              int32)
    IL_0007:  newobj     instance void class [runtime]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_000c:  ret
  } 

  .method public static class [runtime]System.Func`3<int32,int32,int32> niStaticEta() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ldftn      int32 assembly/niStaticEta@28::Invoke(int32,
                                                                           int32)
    IL_0007:  newobj     instance void class [runtime]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_000c:  ret
  } 

  .method public static class [runtime]System.Func`3<int32,int32,int32> niInstanceNonEta(class assembly/C o) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void assembly/niInstanceNonEta@38::.ctor(class assembly/C)
    IL_0006:  ldftn      instance int32 assembly/niInstanceNonEta@38::Invoke(int32,
                                                                                         int32)
    IL_000c:  newobj     instance void class [runtime]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_0011:  ret
  } 

  .method public static class [runtime]System.Func`3<int32,int32,int32> niInstanceEta(class assembly/C o) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void assembly/niInstanceEta@41::.ctor(class assembly/C)
    IL_0006:  ldftn      instance int32 assembly/niInstanceEta@41::Invoke(int32,
                                                                                      int32)
    IL_000c:  newobj     instance void class [runtime]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_0011:  ret
  } 

  .method public static class [runtime]System.Func`3<int32,int32,int32> niGenericInstanceNonEta(class assembly/C o) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void assembly/niGenericInstanceNonEta@44::.ctor(class assembly/C)
    IL_0006:  ldftn      instance int32 assembly/niGenericInstanceNonEta@44::Invoke(int32,
                                                                                                int32)
    IL_000c:  newobj     instance void class [runtime]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_0011:  ret
  } 

  .method public static class [runtime]System.Func`3<int32,int32,int32> niGenericInstanceEta(class assembly/C o) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void assembly/niGenericInstanceEta@47::.ctor(class assembly/C)
    IL_0006:  ldftn      instance int32 assembly/niGenericInstanceEta@47::Invoke(int32,
                                                                                             int32)
    IL_000c:  newobj     instance void class [runtime]System.Func`3<int32,int32,int32>::.ctor(object,
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






