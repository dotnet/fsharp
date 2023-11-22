




.assembly extern runtime { }
.assembly extern FSharp.Core { }
.assembly extern runtime { }
.assembly assembly
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.assembly
{
  
  
}
.mresource public FSharpOptimizationData.assembly
{
  
  
}
.module assembly.dll

.imagebase {value}
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       
.corflags 0x00000001    





.class public abstract auto ansi sealed Static_init_field
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable nested public beforefieldinit S`1<T>
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .field static assembly class Static_init_field/S`1<!T> empty
    .field static assembly int32 init@5
    .method public specialname rtspecialname 
            instance void  .ctor(!T[] x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ret
    } 

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  newobj     instance void class Static_init_field/S`1<!T>::.ctor(!0[])
      IL_0006:  stsfld     class Static_init_field/S`1<!0> class Static_init_field/S`1<!T>::empty
      IL_000b:  ldc.i4.1
      IL_000c:  volatile.
      IL_000e:  stsfld     int32 class Static_init_field/S`1<!T>::init@5
      IL_0013:  ret
    } 

    .method public specialname static class Static_init_field/S`1<!T> 
            get_Empty() cil managed
    {
      
      .maxstack  8
      IL_0000:  volatile.
      IL_0002:  ldsfld     int32 class Static_init_field/S`1<!T>::init@5
      IL_0007:  ldc.i4.1
      IL_0008:  bge.s      IL_0011

      IL_000a:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::FailStaticInit()
      IL_000f:  br.s       IL_0011

      IL_0011:  ldsfld     class Static_init_field/S`1<!0> class Static_init_field/S`1<!T>::empty
      IL_0016:  ret
    } 

    .property class Static_init_field/S`1<!T>
            Empty()
    {
      .get class Static_init_field/S`1<!T> Static_init_field/S`1::get_Empty()
    } 
  } 

  .class sequential ansi serializable sealed nested public beforefieldinit S2`1<T>
         extends [runtime]System.ValueType
         implements class [runtime]System.IEquatable`1<valuetype Static_init_field/S2`1<!T>>,
                    [runtime]System.Collections.IStructuralEquatable,
                    class [runtime]System.IComparable`1<valuetype Static_init_field/S2`1<!T>>,
                    [runtime]System.IComparable,
                    [runtime]System.Collections.IStructuralComparable
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.StructAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .field static assembly object empty
    .field static assembly int32 'init@10-1'
    .field assembly !T[] x
    .method public hidebysig virtual final 
            instance int32  CompareTo(valuetype Static_init_field/S2`1<!T> obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      !0[] valuetype Static_init_field/S2`1<!T>::x
      IL_000b:  ldarga.s   obj
      IL_000d:  ldfld      !0[] valuetype Static_init_field/S2`1<!T>::x
      IL_0012:  tail.
      IL_0014:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericComparisonWithComparerIntrinsic<!T[]>(class [runtime]System.Collections.IComparer,
                                                                                                                                                 !!0,
                                                                                                                                                 !!0)
      IL_0019:  ret
    } 

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  5
      .locals init (valuetype Static_init_field/S2`1<!T> V_0)
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  valuetype Static_init_field/S2`1<!T>
      IL_0006:  stloc.0
      IL_0007:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_000c:  ldarg.0
      IL_000d:  ldfld      !0[] valuetype Static_init_field/S2`1<!T>::x
      IL_0012:  ldloca.s   V_0
      IL_0014:  ldfld      !0[] valuetype Static_init_field/S2`1<!T>::x
      IL_0019:  tail.
      IL_001b:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericComparisonWithComparerIntrinsic<!T[]>(class [runtime]System.Collections.IComparer,
                                                                                                                                                 !!0,
                                                                                                                                                 !!0)
      IL_0020:  ret
    } 

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj,
                                      class [runtime]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  5
      .locals init (valuetype Static_init_field/S2`1<!T> V_0)
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  valuetype Static_init_field/S2`1<!T>
      IL_0006:  stloc.0
      IL_0007:  ldarg.2
      IL_0008:  ldarg.0
      IL_0009:  ldfld      !0[] valuetype Static_init_field/S2`1<!T>::x
      IL_000e:  ldloca.s   V_0
      IL_0010:  ldfld      !0[] valuetype Static_init_field/S2`1<!T>::x
      IL_0015:  tail.
      IL_0017:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericComparisonWithComparerIntrinsic<!T[]>(class [runtime]System.Collections.IComparer,
                                                                                                                                                 !!0,
                                                                                                                                                 !!0)
      IL_001c:  ret
    } 

    .method public hidebysig virtual final 
            instance int32  GetHashCode(class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  7
      .locals init (int32 V_0)
      IL_0000:  ldc.i4.0
      IL_0001:  stloc.0
      IL_0002:  ldc.i4     0x9e3779b9
      IL_0007:  ldarg.1
      IL_0008:  ldarg.0
      IL_0009:  ldfld      !0[] valuetype Static_init_field/S2`1<!T>::x
      IL_000e:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericHashWithComparerIntrinsic<!T[]>(class [runtime]System.Collections.IEqualityComparer,
                                                                                                                                           !!0)
      IL_0013:  ldloc.0
      IL_0014:  ldc.i4.6
      IL_0015:  shl
      IL_0016:  ldloc.0
      IL_0017:  ldc.i4.2
      IL_0018:  shr
      IL_0019:  add
      IL_001a:  add
      IL_001b:  add
      IL_001c:  stloc.0
      IL_001d:  ldloc.0
      IL_001e:  ret
    } 

    .method public hidebysig virtual final 
            instance int32  GetHashCode() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0006:  call       instance int32 valuetype Static_init_field/S2`1<!T>::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
      IL_000b:  ret
    } 

    .method public hidebysig virtual final 
            instance bool  Equals(object obj,
                                  class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  5
      .locals init (valuetype Static_init_field/S2`1<!T> V_0)
      IL_0000:  ldarg.1
      IL_0001:  isinst     valuetype Static_init_field/S2`1<!T>
      IL_0006:  brfalse.s  IL_0025

      IL_0008:  ldarg.1
      IL_0009:  unbox.any  valuetype Static_init_field/S2`1<!T>
      IL_000e:  stloc.0
      IL_000f:  ldarg.2
      IL_0010:  ldarg.0
      IL_0011:  ldfld      !0[] valuetype Static_init_field/S2`1<!T>::x
      IL_0016:  ldloca.s   V_0
      IL_0018:  ldfld      !0[] valuetype Static_init_field/S2`1<!T>::x
      IL_001d:  tail.
      IL_001f:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericEqualityWithComparerIntrinsic<!T[]>(class [runtime]System.Collections.IEqualityComparer,
                                                                                                                                              !!0,
                                                                                                                                              !!0)
      IL_0024:  ret

      IL_0025:  ldc.i4.0
      IL_0026:  ret
    } 

    .method public specialname rtspecialname 
            instance void  .ctor(!T[] x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      !0[] valuetype Static_init_field/S2`1<!T>::x
      IL_0007:  ret
    } 

    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  call       object valuetype Static_init_field/S2`1<!T>::M()
      IL_0005:  stsfld     object valuetype Static_init_field/S2`1<!T>::empty
      IL_000a:  ldc.i4.1
      IL_000b:  volatile.
      IL_000d:  stsfld     int32 valuetype Static_init_field/S2`1<!T>::'init@10-1'
      IL_0012:  ret
    } 

    .method public static object  M() cil managed
    {
      
      .maxstack  8
      IL_0000:  volatile.
      IL_0002:  ldsfld     int32 valuetype Static_init_field/S2`1<!T>::'init@10-1'
      IL_0007:  ldc.i4.1
      IL_0008:  bge.s      IL_0011

      IL_000a:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::FailStaticInit()
      IL_000f:  br.s       IL_0011

      IL_0011:  ldsfld     object valuetype Static_init_field/S2`1<!T>::empty
      IL_0016:  ret
    } 

    .method public hidebysig virtual final 
            instance bool  Equals(valuetype Static_init_field/S2`1<!T> obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0[] valuetype Static_init_field/S2`1<!T>::x
      IL_0006:  ldarga.s   obj
      IL_0008:  ldfld      !0[] valuetype Static_init_field/S2`1<!T>::x
      IL_000d:  tail.
      IL_000f:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericEqualityERIntrinsic<!T[]>(!!0,
                                                                                                                                    !!0)
      IL_0014:  ret
    } 

    .method public hidebysig virtual final 
            instance bool  Equals(object obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (valuetype Static_init_field/S2`1<!T> V_0,
               valuetype Static_init_field/S2`1<!T> V_1)
      IL_0000:  ldarg.1
      IL_0001:  isinst     valuetype Static_init_field/S2`1<!T>
      IL_0006:  brfalse.s  IL_0026

      IL_0008:  ldarg.1
      IL_0009:  unbox.any  valuetype Static_init_field/S2`1<!T>
      IL_000e:  stloc.0
      IL_000f:  ldloc.0
      IL_0010:  stloc.1
      IL_0011:  ldarg.0
      IL_0012:  ldfld      !0[] valuetype Static_init_field/S2`1<!T>::x
      IL_0017:  ldloca.s   V_1
      IL_0019:  ldfld      !0[] valuetype Static_init_field/S2`1<!T>::x
      IL_001e:  tail.
      IL_0020:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericEqualityERIntrinsic<!T[]>(!!0,
                                                                                                                                    !!0)
      IL_0025:  ret

      IL_0026:  ldc.i4.0
      IL_0027:  ret
    } 

  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$Static_init_field$fsx
       extends [runtime]System.Object
{
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method private specialname rtspecialname static 
          void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       void [runtime]System.Console::WriteLine()
    IL_0005:  ret
  } 

} 





