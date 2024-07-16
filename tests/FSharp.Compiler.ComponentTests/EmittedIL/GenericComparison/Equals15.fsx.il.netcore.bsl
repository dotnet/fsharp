




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
.mresource public FSharpSignatureCompressedData.assembly
{
  
  
}
.mresource public FSharpOptimizationCompressedData.assembly
{
  
  
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
  .class abstract auto ansi sealed nested public EqualsMicroPerfAndCodeGenerationTests
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class sequential ansi serializable sealed nested public SomeGenericRecord`1<T>
           extends [runtime]System.ValueType
           implements class [runtime]System.IEquatable`1<valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!T>>,
                      [runtime]System.Collections.IStructuralEquatable,
                      class [runtime]System.IComparable`1<valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!T>>,
                      [runtime]System.IComparable,
                      [runtime]System.Collections.IStructuralComparable
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.StructAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 02 00 00 00 00 00 ) 
      .field assembly !T V@
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .field assembly int32 U@
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .method public hidebysig specialname instance !T  get_V() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      !0 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!T>::V@
        IL_0006:  ret
      } 

      .method public hidebysig specialname instance int32  get_U() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!T>::U@
        IL_0006:  ret
      } 

      .method public specialname rtspecialname 
              instance void  .ctor(!T v,
                                   int32 u) cil managed
      {
        .custom instance void [runtime]System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype [runtime]System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                                                class [runtime]System.Type) = ( 01 00 60 06 00 00 42 45 71 75 61 6C 73 31 35 2B   
                                                                                                                                                       45 71 75 61 6C 73 4D 69 63 72 6F 50 65 72 66 41   
                                                                                                                                                       6E 64 43 6F 64 65 47 65 6E 65 72 61 74 69 6F 6E   
                                                                                                                                                       54 65 73 74 73 2B 53 6F 6D 65 47 65 6E 65 72 69   
                                                                                                                                                       63 52 65 63 6F 72 64 60 31 00 00 )                
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  stfld      !0 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!T>::V@
        IL_0007:  ldarg.0
        IL_0008:  ldarg.2
        IL_0009:  stfld      int32 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!T>::U@
        IL_000e:  ret
      } 

      .method public strict virtual instance string ToString() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldstr      "%+A"
        IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!T>,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!T>>::.ctor(string)
        IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!T>,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
        IL_000f:  ldarg.0
        IL_0010:  ldobj      valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!T>
        IL_0015:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!T>,string>::Invoke(!0)
        IL_001a:  ret
      } 

      .method public hidebysig virtual final instance int32  CompareTo(valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!T> obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  5
        .locals init (int32 V_0,
                 class [runtime]System.Collections.IComparer V_1,
                 !T V_2,
                 !T V_3,
                 int32 V_4,
                 int32 V_5)
        IL_0000:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_0005:  stloc.1
        IL_0006:  ldarg.0
        IL_0007:  ldfld      !0 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!T>::V@
        IL_000c:  stloc.2
        IL_000d:  ldarga.s   obj
        IL_000f:  ldfld      !0 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!T>::V@
        IL_0014:  stloc.3
        IL_0015:  ldloc.1
        IL_0016:  ldloc.2
        IL_0017:  ldloc.3
        IL_0018:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericComparisonWithComparerIntrinsic<!T>(class [runtime]System.Collections.IComparer,
                                                                                                                                                 !!0,
                                                                                                                                                 !!0)
        IL_001d:  stloc.0
        IL_001e:  ldloc.0
        IL_001f:  ldc.i4.0
        IL_0020:  bge.s      IL_0024

        IL_0022:  ldloc.0
        IL_0023:  ret

        IL_0024:  ldloc.0
        IL_0025:  ldc.i4.0
        IL_0026:  ble.s      IL_002a

        IL_0028:  ldloc.0
        IL_0029:  ret

        IL_002a:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_002f:  stloc.1
        IL_0030:  ldarg.0
        IL_0031:  ldfld      int32 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!T>::U@
        IL_0036:  stloc.s    V_4
        IL_0038:  ldarga.s   obj
        IL_003a:  ldfld      int32 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!T>::U@
        IL_003f:  stloc.s    V_5
        IL_0041:  ldloc.s    V_4
        IL_0043:  ldloc.s    V_5
        IL_0045:  cgt
        IL_0047:  ldloc.s    V_4
        IL_0049:  ldloc.s    V_5
        IL_004b:  clt
        IL_004d:  sub
        IL_004e:  ret
      } 

      .method public hidebysig virtual final instance int32  CompareTo(object obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  unbox.any  valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!T>
        IL_0007:  call       instance int32 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!T>::CompareTo(valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!0>)
        IL_000c:  ret
      } 

      .method public hidebysig virtual final 
              instance int32  CompareTo(object obj,
                                        class [runtime]System.Collections.IComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  5
        .locals init (valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!T> V_0,
                 int32 V_1,
                 !T V_2,
                 !T V_3,
                 int32 V_4,
                 int32 V_5)
        IL_0000:  ldarg.1
        IL_0001:  unbox.any  valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!T>
        IL_0006:  stloc.0
        IL_0007:  ldarg.0
        IL_0008:  ldfld      !0 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!T>::V@
        IL_000d:  stloc.2
        IL_000e:  ldloca.s   V_0
        IL_0010:  ldfld      !0 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!T>::V@
        IL_0015:  stloc.3
        IL_0016:  ldarg.2
        IL_0017:  ldloc.2
        IL_0018:  ldloc.3
        IL_0019:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericComparisonWithComparerIntrinsic<!T>(class [runtime]System.Collections.IComparer,
                                                                                                                                                 !!0,
                                                                                                                                                 !!0)
        IL_001e:  stloc.1
        IL_001f:  ldloc.1
        IL_0020:  ldc.i4.0
        IL_0021:  bge.s      IL_0025

        IL_0023:  ldloc.1
        IL_0024:  ret

        IL_0025:  ldloc.1
        IL_0026:  ldc.i4.0
        IL_0027:  ble.s      IL_002b

        IL_0029:  ldloc.1
        IL_002a:  ret

        IL_002b:  ldarg.0
        IL_002c:  ldfld      int32 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!T>::U@
        IL_0031:  stloc.s    V_4
        IL_0033:  ldloca.s   V_0
        IL_0035:  ldfld      int32 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!T>::U@
        IL_003a:  stloc.s    V_5
        IL_003c:  ldloc.s    V_4
        IL_003e:  ldloc.s    V_5
        IL_0040:  cgt
        IL_0042:  ldloc.s    V_4
        IL_0044:  ldloc.s    V_5
        IL_0046:  clt
        IL_0048:  sub
        IL_0049:  ret
      } 

      .method public hidebysig virtual final instance int32  GetHashCode(class [runtime]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  7
        .locals init (int32 V_0,
                 !T V_1)
        IL_0000:  ldc.i4.0
        IL_0001:  stloc.0
        IL_0002:  ldc.i4     0x9e3779b9
        IL_0007:  ldarg.0
        IL_0008:  ldfld      int32 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!T>::U@
        IL_000d:  ldloc.0
        IL_000e:  ldc.i4.6
        IL_000f:  shl
        IL_0010:  ldloc.0
        IL_0011:  ldc.i4.2
        IL_0012:  shr
        IL_0013:  add
        IL_0014:  add
        IL_0015:  add
        IL_0016:  stloc.0
        IL_0017:  ldc.i4     0x9e3779b9
        IL_001c:  ldarg.0
        IL_001d:  ldfld      !0 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!T>::V@
        IL_0022:  stloc.1
        IL_0023:  ldarg.1
        IL_0024:  ldloc.1
        IL_0025:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericHashWithComparerIntrinsic<!T>(class [runtime]System.Collections.IEqualityComparer,
                                                                                                                                           !!0)
        IL_002a:  ldloc.0
        IL_002b:  ldc.i4.6
        IL_002c:  shl
        IL_002d:  ldloc.0
        IL_002e:  ldc.i4.2
        IL_002f:  shr
        IL_0030:  add
        IL_0031:  add
        IL_0032:  add
        IL_0033:  stloc.0
        IL_0034:  ldloc.0
        IL_0035:  ret
      } 

      .method public hidebysig virtual final instance int32  GetHashCode() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
        IL_0006:  call       instance int32 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!T>::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
        IL_000b:  ret
      } 

      .method public hidebysig instance bool 
              Equals(valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!T> obj,
                     class [runtime]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  5
        .locals init (!T V_0,
                 !T V_1)
        IL_0000:  ldarg.0
        IL_0001:  ldfld      !0 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!T>::V@
        IL_0006:  stloc.0
        IL_0007:  ldarga.s   obj
        IL_0009:  ldfld      !0 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!T>::V@
        IL_000e:  stloc.1
        IL_000f:  ldarg.2
        IL_0010:  ldloc.0
        IL_0011:  ldloc.1
        IL_0012:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericEqualityWithComparerIntrinsic<!T>(class [runtime]System.Collections.IEqualityComparer,
                                                                                                                                              !!0,
                                                                                                                                              !!0)
        IL_0017:  brfalse.s  IL_0029

        IL_0019:  ldarg.0
        IL_001a:  ldfld      int32 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!T>::U@
        IL_001f:  ldarga.s   obj
        IL_0021:  ldfld      int32 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!T>::U@
        IL_0026:  ceq
        IL_0028:  ret

        IL_0029:  ldc.i4.0
        IL_002a:  ret
      } 

      .method public hidebysig virtual final 
              instance bool  Equals(object obj,
                                    class [runtime]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  5
        .locals init (valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!T> V_0)
        IL_0000:  ldarg.1
        IL_0001:  isinst     valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!T>
        IL_0006:  brfalse.s  IL_0018

        IL_0008:  ldarg.1
        IL_0009:  unbox.any  valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!T>
        IL_000e:  stloc.0
        IL_000f:  ldarg.0
        IL_0010:  ldloc.0
        IL_0011:  ldarg.2
        IL_0012:  call       instance bool valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!T>::Equals(valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!0>,
                                                                                                                                    class [runtime]System.Collections.IEqualityComparer)
        IL_0017:  ret

        IL_0018:  ldc.i4.0
        IL_0019:  ret
      } 

      .method public hidebysig virtual final instance bool  Equals(valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!T> obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  4
        .locals init (!T V_0,
                 !T V_1)
        IL_0000:  ldarg.0
        IL_0001:  ldfld      !0 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!T>::V@
        IL_0006:  stloc.0
        IL_0007:  ldarga.s   obj
        IL_0009:  ldfld      !0 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!T>::V@
        IL_000e:  stloc.1
        IL_000f:  ldloc.0
        IL_0010:  ldloc.1
        IL_0011:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericEqualityERIntrinsic<!T>(!!0,
                                                                                                                                    !!0)
        IL_0016:  brfalse.s  IL_0028

        IL_0018:  ldarg.0
        IL_0019:  ldfld      int32 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!T>::U@
        IL_001e:  ldarga.s   obj
        IL_0020:  ldfld      int32 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!T>::U@
        IL_0025:  ceq
        IL_0027:  ret

        IL_0028:  ldc.i4.0
        IL_0029:  ret
      } 

      .method public hidebysig virtual final instance bool  Equals(object obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.1
        IL_0001:  isinst     valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!T>
        IL_0006:  brfalse.s  IL_0015

        IL_0008:  ldarg.0
        IL_0009:  ldarg.1
        IL_000a:  unbox.any  valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!T>
        IL_000f:  call       instance bool valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!T>::Equals(valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!0>)
        IL_0014:  ret

        IL_0015:  ldc.i4.0
        IL_0016:  ret
      } 

      .property instance !T V()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
        .get instance !T assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1::get_V()
      } 
      .property instance int32 U()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32) = ( 01 00 04 00 00 00 01 00 00 00 00 00 ) 
        .get instance int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1::get_U()
      } 
    } 

    .field static assembly bool arg@1
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<int32> x@1
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<int32> y@1
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .method assembly specialname static bool get_arg@1() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     bool assembly/EqualsMicroPerfAndCodeGenerationTests::arg@1
      IL_0005:  ret
    } 

    .method assembly specialname static valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<int32> get_x@1() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<int32> assembly/EqualsMicroPerfAndCodeGenerationTests::x@1
      IL_0005:  ret
    } 

    .method assembly specialname static valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<int32> get_y@1() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<int32> assembly/EqualsMicroPerfAndCodeGenerationTests::y@1
      IL_0005:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$assembly$fsx::init@
      IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$assembly$fsx::init@
      IL_000b:  pop
      IL_000c:  ret
    } 

    .method assembly specialname static void staticInitialization@() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.1
      IL_0001:  ldc.i4.2
      IL_0002:  newobj     instance void valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<int32>::.ctor(!0,
                                                                                                                                    int32)
      IL_0007:  stsfld     valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<int32> assembly/EqualsMicroPerfAndCodeGenerationTests::x@1
      IL_000c:  ldc.i4.2
      IL_000d:  ldc.i4.3
      IL_000e:  newobj     instance void valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<int32>::.ctor(!0,
                                                                                                                                    int32)
      IL_0013:  stsfld     valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<int32> assembly/EqualsMicroPerfAndCodeGenerationTests::y@1
      IL_0018:  ldsflda    valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<int32> assembly/EqualsMicroPerfAndCodeGenerationTests::x@1
      IL_001d:  call       valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<int32> assembly/EqualsMicroPerfAndCodeGenerationTests::get_y@1()
      IL_0022:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0027:  call       instance bool valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<int32>::Equals(valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<!0>,
                                                                                                                                     class [runtime]System.Collections.IEqualityComparer)
      IL_002c:  stsfld     bool assembly/EqualsMicroPerfAndCodeGenerationTests::arg@1
      IL_0031:  ret
    } 

    .property bool arg@1()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get bool assembly/EqualsMicroPerfAndCodeGenerationTests::get_arg@1()
    } 
    .property valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<int32>
            x@1()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<int32> assembly/EqualsMicroPerfAndCodeGenerationTests::get_x@1()
    } 
    .property valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<int32>
            y@1()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericRecord`1<int32> assembly/EqualsMicroPerfAndCodeGenerationTests::get_y@1()
    } 
  } 

  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$assembly$fsx::init@
    IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$assembly$fsx::init@
    IL_000b:  pop
    IL_000c:  ret
  } 

  .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       void assembly/EqualsMicroPerfAndCodeGenerationTests::staticInitialization@()
    IL_0005:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly$fsx
       extends [runtime]System.Object
{
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  8
    IL_0000:  call       void assembly::staticInitialization@()
    IL_0005:  ret
  } 

} 






