




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
      
      .maxstack  3
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/C::k
      IL_0006:  stloc.0
      IL_0007:  ret
    } 

    .method public hidebysig instance void AddT(int32 x, int32 y) cil managed
    {
      
      .maxstack  3
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/C::k
      IL_0006:  stloc.0
      IL_0007:  ret
    } 

    .method public hidebysig virtual instance void V(int32 x, int32 y) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
      
      .maxstack  3
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/C::k
      IL_0006:  stloc.0
      IL_0007:  ret
    } 

  } 

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname case20_nonEta@12
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class assembly/C o
    .method public specialname rtspecialname instance void  .ctor(class assembly/C o) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class assembly/C assembly/case20_nonEta@12::o
      IL_0007:  ldarg.0
      IL_0008:  call       instance void [runtime]System.Object::.ctor()
      IL_000d:  ret
    } 

    .method assembly hidebysig instance void Invoke(int32 delegateArg0, int32 delegateArg1) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class assembly/C assembly/case20_nonEta@12::o
      IL_0006:  ldarg.1
      IL_0007:  ldarg.2
      IL_0008:  callvirt   instance void assembly/C::AddC(int32,
                                                                        int32)
      IL_000d:  nop
      IL_000e:  ret
    } 

  } 

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname case4_etaCurried@15
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class assembly/C o
    .method public specialname rtspecialname instance void  .ctor(class assembly/C o) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class assembly/C assembly/case4_etaCurried@15::o
      IL_0007:  ldarg.0
      IL_0008:  call       instance void [runtime]System.Object::.ctor()
      IL_000d:  ret
    } 

    .method assembly hidebysig instance void Invoke(int32 a, int32 b) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class assembly/C assembly/case4_etaCurried@15::o
      IL_0006:  ldarg.1
      IL_0007:  ldarg.2
      IL_0008:  callvirt   instance void assembly/C::AddC(int32,
                                                                        int32)
      IL_000d:  nop
      IL_000e:  ret
    } 

  } 

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname case34_etaTupled@18
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class assembly/C o
    .method public specialname rtspecialname instance void  .ctor(class assembly/C o) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class assembly/C assembly/case34_etaTupled@18::o
      IL_0007:  ldarg.0
      IL_0008:  call       instance void [runtime]System.Object::.ctor()
      IL_000d:  ret
    } 

    .method assembly hidebysig instance void Invoke(int32 a, int32 b) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class assembly/C assembly/case34_etaTupled@18::o
      IL_0006:  ldarg.1
      IL_0007:  ldarg.2
      IL_0008:  callvirt   instance void assembly/C::AddT(int32,
                                                                        int32)
      IL_000d:  nop
      IL_000e:  ret
    } 

  } 

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname case21_virtual@21
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class assembly/C o
    .method public specialname rtspecialname instance void  .ctor(class assembly/C o) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class assembly/C assembly/case21_virtual@21::o
      IL_0007:  ldarg.0
      IL_0008:  call       instance void [runtime]System.Object::.ctor()
      IL_000d:  ret
    } 

    .method assembly hidebysig instance void Invoke(int32 delegateArg0, int32 delegateArg1) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class assembly/C assembly/case21_virtual@21::o
      IL_0006:  ldarg.1
      IL_0007:  ldarg.2
      IL_0008:  tail.
      IL_000a:  callvirt   instance void assembly/C::V(int32,
                                                                     int32)
      IL_000f:  ret
    } 

  } 

  .method public static class [runtime]System.Action`2<int32,int32> case20_nonEta(class assembly/C o) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void assembly/case20_nonEta@12::.ctor(class assembly/C)
    IL_0006:  ldftn      instance void assembly/case20_nonEta@12::Invoke(int32,
                                                                                       int32)
    IL_000c:  newobj     instance void class [runtime]System.Action`2<int32,int32>::.ctor(object,
                                                                                                 native int)
    IL_0011:  ret
  } 

  .method public static class [runtime]System.Action`2<int32,int32> case4_etaCurried(class assembly/C o) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void assembly/case4_etaCurried@15::.ctor(class assembly/C)
    IL_0006:  ldftn      instance void assembly/case4_etaCurried@15::Invoke(int32,
                                                                                          int32)
    IL_000c:  newobj     instance void class [runtime]System.Action`2<int32,int32>::.ctor(object,
                                                                                                 native int)
    IL_0011:  ret
  } 

  .method public static class [runtime]System.Action`2<int32,int32> case34_etaTupled(class assembly/C o) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void assembly/case34_etaTupled@18::.ctor(class assembly/C)
    IL_0006:  ldftn      instance void assembly/case34_etaTupled@18::Invoke(int32,
                                                                                          int32)
    IL_000c:  newobj     instance void class [runtime]System.Action`2<int32,int32>::.ctor(object,
                                                                                                 native int)
    IL_0011:  ret
  } 

  .method public static class [runtime]System.Action`2<int32,int32> case21_virtual(class assembly/C o) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void assembly/case21_virtual@21::.ctor(class assembly/C)
    IL_0006:  ldftn      instance void assembly/case21_virtual@21::Invoke(int32,
                                                                                        int32)
    IL_000c:  newobj     instance void class [runtime]System.Action`2<int32,int32>::.ctor(object,
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






