




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





.class public abstract auto ansi sealed Program
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public DelegateImmediateInvoke4
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto ansi serializable sealed nested public Foo`1<T>
           extends [runtime]System.MulticastDelegate
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
      .method public hidebysig specialname rtspecialname instance void  .ctor(object 'object', native int 'method') runtime managed
      {
      } 

      .method public hidebysig strict virtual instance void  Invoke(!T A_1) runtime managed
      {
      } 

      .method public hidebysig strict virtual 
              instance class [runtime]System.IAsyncResult 
              BeginInvoke(!T A_1,
                          class [runtime]System.AsyncCallback callback,
                          object objects) runtime managed
      {
      } 

      .method public hidebysig strict virtual instance void  EndInvoke(class [runtime]System.IAsyncResult result) runtime managed
      {
      } 

    } 

    .method public static void  f() cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

  } 

  .class abstract auto ansi sealed nested public DelegateImmediateInvoke3
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto ansi serializable sealed nested public Foo`1<T>
           extends [runtime]System.MulticastDelegate
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
      .method public hidebysig specialname rtspecialname instance void  .ctor(object 'object', native int 'method') runtime managed
      {
      } 

      .method public hidebysig strict virtual instance void  Invoke(!T A_1) runtime managed
      {
      } 

      .method public hidebysig strict virtual 
              instance class [runtime]System.IAsyncResult 
              BeginInvoke(!T A_1,
                          class [runtime]System.AsyncCallback callback,
                          object objects) runtime managed
      {
      } 

      .method public hidebysig strict virtual instance void  EndInvoke(class [runtime]System.IAsyncResult result) runtime managed
      {
      } 

    } 

    .method public static void  f() cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

  } 

  .class abstract auto ansi sealed nested public DelegateImmediateInvoke2
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto ansi serializable sealed nested public Foo
           extends [runtime]System.MulticastDelegate
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
      .method public hidebysig specialname rtspecialname instance void  .ctor(object 'object', native int 'method') runtime managed
      {
      } 

      .method public hidebysig strict virtual instance void  Invoke() runtime managed
      {
      } 

      .method public hidebysig strict virtual 
              instance class [runtime]System.IAsyncResult 
              BeginInvoke(class [runtime]System.AsyncCallback callback,
                          object objects) runtime managed
      {
      } 

      .method public hidebysig strict virtual instance void  EndInvoke(class [runtime]System.IAsyncResult result) runtime managed
      {
      } 

    } 

    .class abstract auto autochar serializable sealed nested assembly beforefieldinit specialname 'f@13-1'
           extends [runtime]System.Object
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
      .method assembly static void  Invoke() cil managed
      {
        
        .maxstack  8
        IL_0000:  ret
      } 

    } 

    .method public static void  f() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldftn      void Program/DelegateImmediateInvoke2/'f@13-1'::Invoke()
      IL_0007:  newobj     instance void Program/DelegateImmediateInvoke2/Foo::.ctor(object,
                                                                                     native int)
      IL_000c:  tail.
      IL_000e:  callvirt   instance void Program/DelegateImmediateInvoke2/Foo::Invoke()
      IL_0013:  ret
    } 

  } 

  .class abstract auto ansi sealed nested public DelegateImmediateInvoke1
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto ansi serializable sealed nested public Foo
           extends [runtime]System.MulticastDelegate
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
      .method public hidebysig specialname rtspecialname instance void  .ctor(object 'object', native int 'method') runtime managed
      {
      } 

      .method public hidebysig strict virtual instance void  Invoke() runtime managed
      {
      } 

      .method public hidebysig strict virtual 
              instance class [runtime]System.IAsyncResult 
              BeginInvoke(class [runtime]System.AsyncCallback callback,
                          object objects) runtime managed
      {
      } 

      .method public hidebysig strict virtual instance void  EndInvoke(class [runtime]System.IAsyncResult result) runtime managed
      {
      } 

    } 

    .class abstract auto autochar serializable sealed nested assembly beforefieldinit specialname f@7
           extends [runtime]System.Object
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
      .method assembly static void  Invoke() cil managed
      {
        
        .maxstack  8
        IL_0000:  ret
      } 

    } 

    .method public static void  f() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldftn      void Program/DelegateImmediateInvoke1/f@7::Invoke()
      IL_0007:  newobj     instance void Program/DelegateImmediateInvoke1/Foo::.ctor(object,
                                                                                     native int)
      IL_000c:  tail.
      IL_000e:  callvirt   instance void Program/DelegateImmediateInvoke1/Foo::Invoke()
      IL_0013:  ret
    } 

  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$Program
       extends [runtime]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  8
    IL_0000:  ret
  } 

} 






