




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
  .class auto ansi serializable sealed nested public DTupled
         extends [runtime]System.MulticastDelegate
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public hidebysig specialname rtspecialname instance void  .ctor(object 'object', native int 'method') runtime managed
    {
    } 

    .method public hidebysig strict virtual instance int32  Invoke(int32 A_1, int32 A_2) runtime managed
    {
    } 

    .method public hidebysig strict virtual 
            instance class [runtime]System.IAsyncResult 
            BeginInvoke(int32 A_1,
                        int32 A_2,
                        class [runtime]System.AsyncCallback callback,
                        object objects) runtime managed
    {
    } 

    .method public hidebysig strict virtual instance int32  EndInvoke(class [runtime]System.IAsyncResult result) runtime managed
    {
    } 

  } 

  .class auto ansi serializable sealed nested public DGen`1<T>
         extends [runtime]System.MulticastDelegate
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public hidebysig specialname rtspecialname instance void  .ctor(object 'object', native int 'method') runtime managed
    {
    } 

    .method public hidebysig strict virtual instance !T  Invoke(!T A_1) runtime managed
    {
    } 

    .method public hidebysig strict virtual 
            instance class [runtime]System.IAsyncResult 
            BeginInvoke(!T A_1,
                        class [runtime]System.AsyncCallback callback,
                        object objects) runtime managed
    {
    } 

    .method public hidebysig strict virtual instance !T  EndInvoke(class [runtime]System.IAsyncResult result) runtime managed
    {
    } 

  } 

  .class auto ansi serializable sealed nested public DByref
         extends [runtime]System.MulticastDelegate
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public hidebysig specialname rtspecialname instance void  .ctor(object 'object', native int 'method') runtime managed
    {
    } 

    .method public hidebysig strict virtual instance void  Invoke(int32& A_1) runtime managed
    {
    } 

    .method public hidebysig strict virtual 
            instance class [runtime]System.IAsyncResult 
            BeginInvoke(int32& A_1,
                        class [runtime]System.AsyncCallback callback,
                        object objects) runtime managed
    {
    } 

    .method public hidebysig strict virtual instance void  EndInvoke(class [runtime]System.IAsyncResult result) runtime managed
    {
    } 

  } 

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

    .method public hidebysig instance int32 M(int32 x, int32 y) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.NoCompilerInliningAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.2
      IL_0002:  mul
      IL_0003:  ret
    } 

  } 

  .class abstract auto autochar serializable sealed nested assembly beforefieldinit specialname tupledNonEta@28
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .method assembly static int32  Invoke(int32 x,
                                          int32 y) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  call       int32 assembly::acc(int32,
                                                         int32)
      IL_0007:  ret
    } 

  } 

  .class abstract auto autochar serializable sealed nested assembly beforefieldinit specialname tupledEta@30
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .method assembly static int32  Invoke(int32 a,
                                          int32 b) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  call       int32 assembly::acc(int32,
                                                         int32)
      IL_0007:  ret
    } 

  } 

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname instanceNonEta@34
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class assembly/C c
    .method public specialname rtspecialname instance void  .ctor(class assembly/C c) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class assembly/C assembly/instanceNonEta@34::c
      IL_0007:  ldarg.0
      IL_0008:  call       instance void [runtime]System.Object::.ctor()
      IL_000d:  ret
    } 

    .method assembly hidebysig instance int32 Invoke(int32 delegateArg0, int32 delegateArg1) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class assembly/C assembly/instanceNonEta@34::c
      IL_0006:  ldarg.1
      IL_0007:  ldarg.2
      IL_0008:  callvirt   instance int32 assembly/C::M(int32,
                                                                  int32)
      IL_000d:  ret
    } 

  } 

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname instanceEta@36
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class assembly/C c
    .method public specialname rtspecialname instance void  .ctor(class assembly/C c) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class assembly/C assembly/instanceEta@36::c
      IL_0007:  ldarg.0
      IL_0008:  call       instance void [runtime]System.Object::.ctor()
      IL_000d:  ret
    } 

    .method assembly hidebysig instance int32 Invoke(int32 a, int32 b) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class assembly/C assembly/instanceEta@36::c
      IL_0006:  ldarg.1
      IL_0007:  ldarg.2
      IL_0008:  callvirt   instance int32 assembly/C::M(int32,
                                                                  int32)
      IL_000d:  ret
    } 

  } 

  .class abstract auto autochar serializable sealed nested assembly beforefieldinit specialname genNonEta@40
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .method assembly static int32  Invoke(int32 delegateArg0) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       !!0 assembly::ident<int32>(!!0)
      IL_0006:  ret
    } 

  } 

  .class abstract auto autochar serializable sealed nested assembly beforefieldinit specialname genEta@42
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .method assembly static int32  Invoke(int32 x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       !!0 assembly::ident<int32>(!!0)
      IL_0006:  ret
    } 

  } 

  .class abstract auto autochar serializable sealed nested assembly beforefieldinit specialname byrefMutate@47
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .method assembly static void  Invoke(int32& x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.0
      IL_0002:  ldobj      [runtime]System.Int32
      IL_0007:  ldc.i4.1
      IL_0008:  add
      IL_0009:  stobj      [runtime]System.Int32
      IL_000e:  ret
    } 

  } 

  .method public static int32  acc(int32 x,
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

  .method public static !!T  ident<T>(!!T x) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.NoCompilerInliningAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ret
  } 

  .method public static class assembly/DTupled tupledNonEta() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ldftn      int32 assembly/tupledNonEta@28::Invoke(int32,
                                                                          int32)
    IL_0007:  newobj     instance void assembly/DTupled::.ctor(object,
                                                                         native int)
    IL_000c:  ret
  } 

  .method public static class assembly/DTupled tupledEta() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ldftn      int32 assembly/tupledEta@30::Invoke(int32,
                                                                       int32)
    IL_0007:  newobj     instance void assembly/DTupled::.ctor(object,
                                                                         native int)
    IL_000c:  ret
  } 

  .method public static class assembly/DTupled instanceNonEta(class assembly/C c) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void assembly/instanceNonEta@34::.ctor(class assembly/C)
    IL_0006:  ldftn      instance int32 assembly/instanceNonEta@34::Invoke(int32,
                                                                                     int32)
    IL_000c:  newobj     instance void assembly/DTupled::.ctor(object,
                                                                         native int)
    IL_0011:  ret
  } 

  .method public static class assembly/DTupled instanceEta(class assembly/C c) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void assembly/instanceEta@36::.ctor(class assembly/C)
    IL_0006:  ldftn      instance int32 assembly/instanceEta@36::Invoke(int32,
                                                                                  int32)
    IL_000c:  newobj     instance void assembly/DTupled::.ctor(object,
                                                                         native int)
    IL_0011:  ret
  } 

  .method public static class assembly/DGen`1<int32> genNonEta() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ldftn      int32 assembly/genNonEta@40::Invoke(int32)
    IL_0007:  newobj     instance void class assembly/DGen`1<int32>::.ctor(object,
                                                                                     native int)
    IL_000c:  ret
  } 

  .method public static class assembly/DGen`1<int32> genEta() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ldftn      int32 assembly/genEta@42::Invoke(int32)
    IL_0007:  newobj     instance void class assembly/DGen`1<int32>::.ctor(object,
                                                                                     native int)
    IL_000c:  ret
  } 

  .method public static class assembly/DByref byrefMutate() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ldftn      void assembly/byrefMutate@47::Invoke(int32&)
    IL_0007:  newobj     instance void assembly/DByref::.ctor(object,
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






