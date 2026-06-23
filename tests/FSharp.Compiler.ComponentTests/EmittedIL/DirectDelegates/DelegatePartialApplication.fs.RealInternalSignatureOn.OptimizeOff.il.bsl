




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
    .method public static void  Add3(int32 x,
                                     int32 y,
                                     int32 z) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                      00 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ret
    } 

  } 

  .class auto ansi serializable nested public I
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
      IL_000a:  stfld      int32 assembly/I::k
      IL_000f:  ret
    } 

    .method public hidebysig instance void 
            Add3(int32 x,
                 int32 y,
                 int32 z) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                      00 00 00 00 ) 
      
      .maxstack  3
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/I::k
      IL_0006:  stloc.0
      IL_0007:  ret
    } 

  } 

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname papKnownVar@21
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public int32 n
    .method public specialname rtspecialname instance void  .ctor(int32 n) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 assembly/papKnownVar@21::n
      IL_0007:  ldarg.0
      IL_0008:  call       instance void [runtime]System.Object::.ctor()
      IL_000d:  ret
    } 

    .method assembly hidebysig instance void Invoke(int32 delegateArg0, int32 delegateArg1) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/papKnownVar@21::n
      IL_0006:  ldarg.1
      IL_0007:  ldarg.2
      IL_0008:  call       void assembly::handler3(int32,
                                                                     int32,
                                                                     int32)
      IL_000d:  nop
      IL_000e:  ret
    } 

  } 

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname papStaticVar@24
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public int32 n
    .method public specialname rtspecialname instance void  .ctor(int32 n) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 assembly/papStaticVar@24::n
      IL_0007:  ldarg.0
      IL_0008:  call       instance void [runtime]System.Object::.ctor()
      IL_000d:  ret
    } 

    .method assembly hidebysig instance void Invoke(int32 delegateArg0, int32 delegateArg1) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/papStaticVar@24::n
      IL_0006:  ldarg.1
      IL_0007:  ldarg.2
      IL_0008:  call       void assembly/C::Add3(int32,
                                                                   int32,
                                                                   int32)
      IL_000d:  nop
      IL_000e:  ret
    } 

  } 

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname papInstanceVar@27
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class assembly/I o
    .field public int32 n
    .method public specialname rtspecialname instance void  .ctor(class assembly/I o, int32 n) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class assembly/I assembly/papInstanceVar@27::o
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 assembly/papInstanceVar@27::n
      IL_000e:  ldarg.0
      IL_000f:  call       instance void [runtime]System.Object::.ctor()
      IL_0014:  ret
    } 

    .method assembly hidebysig instance void Invoke(int32 delegateArg0, int32 delegateArg1) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class assembly/I assembly/papInstanceVar@27::o
      IL_0006:  ldarg.0
      IL_0007:  ldfld      int32 assembly/papInstanceVar@27::n
      IL_000c:  ldarg.1
      IL_000d:  ldarg.2
      IL_000e:  callvirt   instance void assembly/I::Add3(int32,
                                                                            int32,
                                                                            int32)
      IL_0013:  nop
      IL_0014:  ret
    } 

  } 

  .method public static void  handler3(int32 x,
                                       int32 y,
                                       int32 z) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ret
  } 

  .method public static class [runtime]System.Action`2<int32,int32> papKnownVar(int32 n) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void assembly/papKnownVar@21::.ctor(int32)
    IL_0006:  ldftn      instance void assembly/papKnownVar@21::Invoke(int32,
                                                                                         int32)
    IL_000c:  newobj     instance void class [runtime]System.Action`2<int32,int32>::.ctor(object,
                                                                                           native int)
    IL_0011:  ret
  } 

  .method public static class [runtime]System.Action`2<int32,int32> papStaticVar(int32 n) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void assembly/papStaticVar@24::.ctor(int32)
    IL_0006:  ldftn      instance void assembly/papStaticVar@24::Invoke(int32,
                                                                                          int32)
    IL_000c:  newobj     instance void class [runtime]System.Action`2<int32,int32>::.ctor(object,
                                                                                           native int)
    IL_0011:  ret
  } 

  .method public static class [runtime]System.Action`2<int32,int32> papInstanceVar(class assembly/I o, int32 n) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  newobj     instance void assembly/papInstanceVar@27::.ctor(class assembly/I,
                                                                                           int32)
    IL_0007:  ldftn      instance void assembly/papInstanceVar@27::Invoke(int32,
                                                                                            int32)
    IL_000d:  newobj     instance void class [runtime]System.Action`2<int32,int32>::.ctor(object,
                                                                                           native int)
    IL_0012:  ret
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






