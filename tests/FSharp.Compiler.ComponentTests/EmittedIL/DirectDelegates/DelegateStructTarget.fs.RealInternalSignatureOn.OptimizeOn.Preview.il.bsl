




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
  .class sequential ansi serializable sealed nested public S
         extends [runtime]System.ValueType
         implements class [runtime]System.IEquatable`1<valuetype assembly/S>,
                    [runtime]System.Collections.IStructuralEquatable,
                    class [runtime]System.IComparable`1<valuetype assembly/S>,
                    [runtime]System.IComparable,
                    [runtime]System.Collections.IStructuralComparable
  {
    .pack 0
    .size 1
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.StructAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public hidebysig virtual final instance int32  CompareTo(valuetype assembly/S obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method public hidebysig virtual final instance int32  CompareTo(object obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  3
      .locals init (valuetype assembly/S V_0)
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  assembly/S
      IL_0006:  stloc.0
      IL_0007:  ldc.i4.0
      IL_0008:  ret
    } 

    .method public hidebysig virtual final instance int32  CompareTo(object obj, class [runtime]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  3
      .locals init (valuetype assembly/S V_0)
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  assembly/S
      IL_0006:  stloc.0
      IL_0007:  ldc.i4.0
      IL_0008:  ret
    } 

    .method public hidebysig virtual final instance int32  GetHashCode(class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method public hidebysig virtual final instance int32  GetHashCode() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0006:  call       instance int32 assembly/S::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
      IL_000b:  ret
    } 

    .method public hidebysig instance bool Equals(valuetype assembly/S obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldc.i4.1
      IL_0001:  ret
    } 

    .method public hidebysig virtual final instance bool  Equals(object obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  3
      .locals init (valuetype assembly/S V_0)
      IL_0000:  ldarg.1
      IL_0001:  isinst     assembly/S
      IL_0006:  brfalse.s  IL_0011

      IL_0008:  ldarg.1
      IL_0009:  unbox.any  assembly/S
      IL_000e:  stloc.0
      IL_000f:  ldc.i4.1
      IL_0010:  ret

      IL_0011:  ldc.i4.0
      IL_0012:  ret
    } 

    .method public hidebysig instance int32 Add(int32 x, int32 y) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.NoCompilerInliningAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.2
      IL_0002:  add
      IL_0003:  ret
    } 

    .method public hidebysig virtual final instance bool  Equals(valuetype assembly/S obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldc.i4.1
      IL_0001:  ret
    } 

    .method public hidebysig virtual final instance bool  Equals(object obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  3
      .locals init (valuetype assembly/S V_0)
      IL_0000:  ldarg.1
      IL_0001:  isinst     assembly/S
      IL_0006:  brfalse.s  IL_0011

      IL_0008:  ldarg.1
      IL_0009:  unbox.any  assembly/S
      IL_000e:  stloc.0
      IL_000f:  ldc.i4.1
      IL_0010:  ret

      IL_0011:  ldc.i4.0
      IL_0012:  ret
    } 

  } 

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname structInstanceNonEta@13
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public valuetype assembly/S s
    .method public specialname rtspecialname instance void  .ctor(valuetype assembly/S s) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      valuetype assembly/S assembly/structInstanceNonEta@13::s
      IL_0007:  ldarg.0
      IL_0008:  call       instance void [runtime]System.Object::.ctor()
      IL_000d:  ret
    } 

    .method assembly hidebysig instance int32 Invoke(int32 delegateArg0, int32 delegateArg1) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldflda     valuetype assembly/S assembly/structInstanceNonEta@13::s
      IL_0006:  ldarg.1
      IL_0007:  ldarg.2
      IL_0008:  call       instance int32 assembly/S::Add(int32,
                                                                      int32)
      IL_000d:  ret
    } 

  } 

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname structInstanceEta@15
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public valuetype assembly/S s
    .method public specialname rtspecialname instance void  .ctor(valuetype assembly/S s) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      valuetype assembly/S assembly/structInstanceEta@15::s
      IL_0007:  ldarg.0
      IL_0008:  call       instance void [runtime]System.Object::.ctor()
      IL_000d:  ret
    } 

    .method assembly hidebysig instance int32 Invoke(int32 a, int32 b) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldflda     valuetype assembly/S assembly/structInstanceEta@15::s
      IL_0006:  ldarg.1
      IL_0007:  ldarg.2
      IL_0008:  call       instance int32 assembly/S::Add(int32,
                                                                      int32)
      IL_000d:  ret
    } 

  } 

  .method public static class [runtime]System.Func`3<int32,int32,int32> structInstanceNonEta(valuetype assembly/S s) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void assembly/structInstanceNonEta@13::.ctor(valuetype assembly/S)
    IL_0006:  ldftn      instance int32 assembly/structInstanceNonEta@13::Invoke(int32,
                                                                                             int32)
    IL_000c:  newobj     instance void class [runtime]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_0011:  ret
  } 

  .method public static class [runtime]System.Func`3<int32,int32,int32> structInstanceEta(valuetype assembly/S s) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void assembly/structInstanceEta@15::.ctor(valuetype assembly/S)
    IL_0006:  ldftn      instance int32 assembly/structInstanceEta@15::Invoke(int32,
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






