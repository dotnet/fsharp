




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
  .class auto ansi serializable nested public StackGuard
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

    .method public hidebysig instance !!T RunOnNewStack<T>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,!!T> f) cil managed noinlining
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldnull
      IL_0002:  tail.
      IL_0004:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,!!T>::Invoke(!0)
      IL_0009:  ret
    } 

    .method public hidebysig instance !!T Guard<T>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,!!T> f) cil managed
    {
      .param [1]
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.InlineIfLambdaAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  call       bool [runtime]System.Runtime.CompilerServices.RuntimeHelpers::TryEnsureSufficientExecutionStack()
      IL_0005:  brfalse.s  IL_0011

      IL_0007:  ldarg.1
      IL_0008:  ldnull
      IL_0009:  tail.
      IL_000b:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,!!T>::Invoke(!0)
      IL_0010:  ret

      IL_0011:  ldarg.0
      IL_0012:  ldarg.1
      IL_0013:  tail.
      IL_0015:  callvirt   instance !!0 assembly/StackGuard::RunOnNewStack<!!0>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,!!0>)
      IL_001a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit callDirect@26
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>
  {
    .field public int32 env
    .method assembly specialname rtspecialname instance void  .ctor(int32 env) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32 assembly/callDirect@26::env
      IL_000d:  ret
    } 

    .method public strict virtual instance int32 Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar0) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/callDirect@26::env
      IL_0006:  ldc.i4.1
      IL_0007:  add
      IL_0008:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit callPiped@28
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>
  {
    .field public int32 env
    .method assembly specialname rtspecialname instance void  .ctor(int32 env) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32 assembly/callPiped@28::env
      IL_000d:  ret
    } 

    .method public strict virtual instance int32 Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar0) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/callPiped@28::env
      IL_0006:  ldc.i4.1
      IL_0007:  add
      IL_0008:  ret
    } 

  } 

  .method public static int32  callDirect(class assembly/StackGuard sg,
                                          int32 env) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  call       bool [runtime]System.Runtime.CompilerServices.RuntimeHelpers::TryEnsureSufficientExecutionStack()
    IL_0005:  brfalse.s  IL_000b

    IL_0007:  ldarg.1
    IL_0008:  ldc.i4.1
    IL_0009:  add
    IL_000a:  ret

    IL_000b:  ldarg.0
    IL_000c:  ldarg.1
    IL_000d:  newobj     instance void assembly/callDirect@26::.ctor(int32)
    IL_0012:  tail.
    IL_0014:  callvirt   instance !!0 assembly/StackGuard::RunOnNewStack<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,!!0>)
    IL_0019:  ret
  } 

  .method public static int32  callPiped(class assembly/StackGuard sg,
                                         int32 env) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  4
    .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> V_0)
    IL_0000:  ldarg.1
    IL_0001:  newobj     instance void assembly/callPiped@28::.ctor(int32)
    IL_0006:  stloc.0
    IL_0007:  call       bool [runtime]System.Runtime.CompilerServices.RuntimeHelpers::TryEnsureSufficientExecutionStack()
    IL_000c:  brfalse.s  IL_0012

    IL_000e:  ldarg.1
    IL_000f:  ldc.i4.1
    IL_0010:  add
    IL_0011:  ret

    IL_0012:  ldarg.0
    IL_0013:  ldloc.0
    IL_0014:  tail.
    IL_0016:  callvirt   instance !!0 assembly/StackGuard::RunOnNewStack<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,!!0>)
    IL_001b:  ret
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





