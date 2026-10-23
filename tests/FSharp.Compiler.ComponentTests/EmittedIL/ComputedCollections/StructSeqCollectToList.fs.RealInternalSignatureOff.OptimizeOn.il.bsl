




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
  .class sequential ansi serializable sealed nested public StructSeq
         extends [runtime]System.ValueType
         implements class [runtime]System.IEquatable`1<valuetype assembly/StructSeq>,
                    [runtime]System.Collections.IStructuralEquatable,
                    class [runtime]System.IComparable`1<valuetype assembly/StructSeq>,
                    [runtime]System.IComparable,
                    [runtime]System.Collections.IStructuralComparable,
                    [runtime]System.Collections.IEnumerable,
                    class [runtime]System.Collections.Generic.IEnumerable`1<int32>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.StructAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .field assembly int32[] items
    .method public hidebysig virtual final instance int32  CompareTo(valuetype assembly/StructSeq obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32[] assembly/StructSeq::items
      IL_000b:  ldarga.s   obj
      IL_000d:  ldfld      int32[] assembly/StructSeq::items
      IL_0012:  tail.
      IL_0014:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericComparisonWithComparerIntrinsic<int32[]>(class [runtime]System.Collections.IComparer,
                                                                                                                                                    !!0,
                                                                                                                                                    !!0)
      IL_0019:  ret
    } 

    .method public hidebysig virtual final instance int32  CompareTo(object obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  unbox.any  assembly/StructSeq
      IL_0007:  call       instance int32 assembly/StructSeq::CompareTo(valuetype assembly/StructSeq)
      IL_000c:  ret
    } 

    .method public hidebysig virtual final instance int32  CompareTo(object obj, class [runtime]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  5
      .locals init (valuetype assembly/StructSeq V_0)
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  assembly/StructSeq
      IL_0006:  stloc.0
      IL_0007:  ldarg.2
      IL_0008:  ldarg.0
      IL_0009:  ldfld      int32[] assembly/StructSeq::items
      IL_000e:  ldloca.s   V_0
      IL_0010:  ldfld      int32[] assembly/StructSeq::items
      IL_0015:  tail.
      IL_0017:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericComparisonWithComparerIntrinsic<int32[]>(class [runtime]System.Collections.IComparer,
                                                                                                                                                    !!0,
                                                                                                                                                    !!0)
      IL_001c:  ret
    } 

    .method public hidebysig virtual final instance int32  GetHashCode(class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  7
      .locals init (int32 V_0)
      IL_0000:  ldc.i4.0
      IL_0001:  stloc.0
      IL_0002:  ldc.i4     0x9e3779b9
      IL_0007:  ldarg.1
      IL_0008:  ldarg.0
      IL_0009:  ldfld      int32[] assembly/StructSeq::items
      IL_000e:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericHashWithComparerIntrinsic<int32[]>(class [runtime]System.Collections.IEqualityComparer,
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

    .method public hidebysig virtual final instance int32  GetHashCode() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0006:  call       instance int32 assembly/StructSeq::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
      IL_000b:  ret
    } 

    .method public hidebysig instance bool Equals(valuetype assembly/StructSeq obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.2
      IL_0001:  ldarg.0
      IL_0002:  ldfld      int32[] assembly/StructSeq::items
      IL_0007:  ldarga.s   obj
      IL_0009:  ldfld      int32[] assembly/StructSeq::items
      IL_000e:  tail.
      IL_0010:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericEqualityWithComparerIntrinsic<int32[]>(class [runtime]System.Collections.IEqualityComparer,
                                                                                                                                                 !!0,
                                                                                                                                                 !!0)
      IL_0015:  ret
    } 

    .method public hidebysig virtual final instance bool  Equals(object obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  5
      .locals init (valuetype assembly/StructSeq V_0)
      IL_0000:  ldarg.1
      IL_0001:  isinst     assembly/StructSeq
      IL_0006:  brfalse.s  IL_0018

      IL_0008:  ldarg.1
      IL_0009:  unbox.any  assembly/StructSeq
      IL_000e:  stloc.0
      IL_000f:  ldarg.0
      IL_0010:  ldloc.0
      IL_0011:  ldarg.2
      IL_0012:  call       instance bool assembly/StructSeq::Equals(valuetype assembly/StructSeq,
                                                                                  class [runtime]System.Collections.IEqualityComparer)
      IL_0017:  ret

      IL_0018:  ldc.i4.0
      IL_0019:  ret
    } 

    .method public specialname rtspecialname instance void  .ctor(int32[] items) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32[] assembly/StructSeq::items
      IL_0007:  ret
    } 

    .method private hidebysig newslot virtual instance class [runtime]System.Collections.Generic.IEnumerator`1<int32>  'System.Collections.Generic.IEnumerable<System.Int32>.GetEnumerator'() cil managed
    {
      .override  method instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32[] assembly/StructSeq::items
      IL_0006:  tail.
      IL_0008:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_000d:  ret
    } 

    .method private hidebysig newslot virtual instance class [runtime]System.Collections.IEnumerator  System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .override [runtime]System.Collections.IEnumerable::GetEnumerator
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32[] assembly/StructSeq::items
      IL_0006:  tail.
      IL_0008:  callvirt   instance class [runtime]System.Collections.IEnumerator [runtime]System.Array::GetEnumerator()
      IL_000d:  ret
    } 

    .method public hidebysig virtual final instance bool  Equals(valuetype assembly/StructSeq obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32[] assembly/StructSeq::items
      IL_0006:  ldarga.s   obj
      IL_0008:  ldfld      int32[] assembly/StructSeq::items
      IL_000d:  tail.
      IL_000f:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericEqualityERIntrinsic<int32[]>(!!0,
                                                                                                                                       !!0)
      IL_0014:  ret
    } 

    .method public hidebysig virtual final instance bool  Equals(object obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  isinst     assembly/StructSeq
      IL_0006:  brfalse.s  IL_0015

      IL_0008:  ldarg.0
      IL_0009:  ldarg.1
      IL_000a:  unbox.any  assembly/StructSeq
      IL_000f:  call       instance bool assembly/StructSeq::Equals(valuetype assembly/StructSeq)
      IL_0014:  ret

      IL_0015:  ldc.i4.0
      IL_0016:  ret
    } 

  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> collectToList(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<valuetype assembly/StructSeq> xs) cil managed
  {
    
    .maxstack  4
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             class [runtime]System.Collections.Generic.IEnumerator`1<valuetype assembly/StructSeq> V_1,
             valuetype assembly/StructSeq V_2,
             class [runtime]System.IDisposable V_3)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<valuetype assembly/StructSeq>::GetEnumerator()
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  br.s       IL_0024

      IL_000a:  ldloc.1
      IL_000b:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<valuetype assembly/StructSeq>::get_Current()
      IL_0010:  stloc.2
      IL_0011:  ldloca.s   V_0
      IL_0013:  ldloc.2
      IL_0014:  box        assembly/StructSeq
      IL_0019:  unbox.any  class [runtime]System.Collections.Generic.IEnumerable`1<int32>
      IL_001e:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::AddMany(class [runtime]System.Collections.Generic.IEnumerable`1<!0>)
      IL_0023:  nop
      IL_0024:  ldloc.1
      IL_0025:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_002a:  brtrue.s   IL_000a

      IL_002c:  leave.s    IL_0040

    }  
    finally
    {
      IL_002e:  ldloc.1
      IL_002f:  isinst     [runtime]System.IDisposable
      IL_0034:  stloc.3
      IL_0035:  ldloc.3
      IL_0036:  brfalse.s  IL_003f

      IL_0038:  ldloc.3
      IL_0039:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_003e:  endfinally
      IL_003f:  endfinally
    }  
    IL_0040:  ldloca.s   V_0
    IL_0042:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0047:  ret
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






