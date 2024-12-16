




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
  .class abstract auto ansi sealed nested public EqualsMicroPerfAndCodeGenerationTests
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class sequential autochar serializable sealed nested public beforefieldinit SomeGenericUnion`1<T>
           extends [runtime]System.ValueType
           implements class [runtime]System.IEquatable`1<valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T>>,
                      [runtime]System.Collections.IStructuralEquatable,
                      class [runtime]System.IComparable`1<valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T>>,
                      [runtime]System.IComparable,
                      [runtime]System.Collections.IStructuralComparable
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.StructAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 7B 5F 5F 44 65 62 75 67 44 69 73 70 6C   
                                                                                                     61 79 28 29 2C 6E 71 7D 00 00 )                   
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 01 00 00 00 00 00 ) 
      .field assembly !T item1
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field assembly int32 item2
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method public static valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T> NewSomeGenericUnion(!T item1, int32 item2) cil managed
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32) = ( 01 00 08 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  3
        .locals init (valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T> V_0)
        IL_0000:  ldloca.s   V_0
        IL_0002:  initobj    valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T>
        IL_0008:  ldloca.s   V_0
        IL_000a:  ldarg.0
        IL_000b:  stfld      !0 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T>::item1
        IL_0010:  ldloca.s   V_0
        IL_0012:  ldarg.1
        IL_0013:  stfld      int32 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T>::item2
        IL_0018:  ldloc.0
        IL_0019:  ret
      } 

      .method public hidebysig instance !T get_Item1() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      !0 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T>::item1
        IL_0006:  ret
      } 

      .method public hidebysig instance int32 get_Item2() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T>::item2
        IL_0006:  ret
      } 

      .method public hidebysig instance int32 get_Tag() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  pop
        IL_0002:  ldc.i4.0
        IL_0003:  ret
      } 

      .method assembly hidebysig specialname instance object  __DebugDisplay() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldstr      "%+0.8A"
        IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T>,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,string>::.ctor(string)
        IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T>,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
        IL_000f:  ldarg.0
        IL_0010:  ldobj      valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T>
        IL_0015:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T>,string>::Invoke(!0)
        IL_001a:  ret
      } 

      .method public strict virtual instance string ToString() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldstr      "%+A"
        IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T>,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T>>::.ctor(string)
        IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T>,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
        IL_000f:  ldarg.0
        IL_0010:  ldobj      valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T>
        IL_0015:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T>,string>::Invoke(!0)
        IL_001a:  ret
      } 

      .method public hidebysig virtual final instance int32  CompareTo(valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T> obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  5
        .locals init (int32 V_0,
                 class [runtime]System.Collections.IComparer V_1,
                 !T V_2,
                 !T V_3,
                 int32 V_4,
                 int32 V_5)
        IL_0000:  ldarg.0
        IL_0001:  pop
        IL_0002:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_0007:  stloc.1
        IL_0008:  ldarg.0
        IL_0009:  ldfld      !0 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T>::item1
        IL_000e:  stloc.2
        IL_000f:  ldarga.s   obj
        IL_0011:  ldfld      !0 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T>::item1
        IL_0016:  stloc.3
        IL_0017:  ldloc.1
        IL_0018:  ldloc.2
        IL_0019:  ldloc.3
        IL_001a:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericComparisonWithComparerIntrinsic<!T>(class [runtime]System.Collections.IComparer,
                                                                                                                                                 !!0,
                                                                                                                                                 !!0)
        IL_001f:  stloc.0
        IL_0020:  ldloc.0
        IL_0021:  ldc.i4.0
        IL_0022:  bge.s      IL_0026

        IL_0024:  ldloc.0
        IL_0025:  ret

        IL_0026:  ldloc.0
        IL_0027:  ldc.i4.0
        IL_0028:  ble.s      IL_002c

        IL_002a:  ldloc.0
        IL_002b:  ret

        IL_002c:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_0031:  stloc.1
        IL_0032:  ldarg.0
        IL_0033:  ldfld      int32 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T>::item2
        IL_0038:  stloc.s    V_4
        IL_003a:  ldarga.s   obj
        IL_003c:  ldfld      int32 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T>::item2
        IL_0041:  stloc.s    V_5
        IL_0043:  ldloc.s    V_4
        IL_0045:  ldloc.s    V_5
        IL_0047:  cgt
        IL_0049:  ldloc.s    V_4
        IL_004b:  ldloc.s    V_5
        IL_004d:  clt
        IL_004f:  sub
        IL_0050:  ret
      } 

      .method public hidebysig virtual final instance int32  CompareTo(object obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  unbox.any  valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T>
        IL_0007:  call       instance int32 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T>::CompareTo(valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!0>)
        IL_000c:  ret
      } 

      .method public hidebysig virtual final instance int32  CompareTo(object obj, class [runtime]System.Collections.IComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  5
        .locals init (valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T> V_0,
                 int32 V_1,
                 !T V_2,
                 !T V_3,
                 int32 V_4,
                 int32 V_5)
        IL_0000:  ldarg.1
        IL_0001:  unbox.any  valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T>
        IL_0006:  stloc.0
        IL_0007:  ldarg.0
        IL_0008:  pop
        IL_0009:  ldarg.0
        IL_000a:  ldfld      !0 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T>::item1
        IL_000f:  stloc.2
        IL_0010:  ldloca.s   V_0
        IL_0012:  ldfld      !0 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T>::item1
        IL_0017:  stloc.3
        IL_0018:  ldarg.2
        IL_0019:  ldloc.2
        IL_001a:  ldloc.3
        IL_001b:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericComparisonWithComparerIntrinsic<!T>(class [runtime]System.Collections.IComparer,
                                                                                                                                                 !!0,
                                                                                                                                                 !!0)
        IL_0020:  stloc.1
        IL_0021:  ldloc.1
        IL_0022:  ldc.i4.0
        IL_0023:  bge.s      IL_0027

        IL_0025:  ldloc.1
        IL_0026:  ret

        IL_0027:  ldloc.1
        IL_0028:  ldc.i4.0
        IL_0029:  ble.s      IL_002d

        IL_002b:  ldloc.1
        IL_002c:  ret

        IL_002d:  ldarg.0
        IL_002e:  ldfld      int32 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T>::item2
        IL_0033:  stloc.s    V_4
        IL_0035:  ldloca.s   V_0
        IL_0037:  ldfld      int32 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T>::item2
        IL_003c:  stloc.s    V_5
        IL_003e:  ldloc.s    V_4
        IL_0040:  ldloc.s    V_5
        IL_0042:  cgt
        IL_0044:  ldloc.s    V_4
        IL_0046:  ldloc.s    V_5
        IL_0048:  clt
        IL_004a:  sub
        IL_004b:  ret
      } 

      .method public hidebysig virtual final instance int32  GetHashCode(class [runtime]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  7
        .locals init (int32 V_0,
                 !T V_1)
        IL_0000:  ldc.i4.0
        IL_0001:  stloc.0
        IL_0002:  ldarg.0
        IL_0003:  pop
        IL_0004:  ldc.i4.0
        IL_0005:  stloc.0
        IL_0006:  ldc.i4     0x9e3779b9
        IL_000b:  ldarg.0
        IL_000c:  ldfld      int32 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T>::item2
        IL_0011:  ldloc.0
        IL_0012:  ldc.i4.6
        IL_0013:  shl
        IL_0014:  ldloc.0
        IL_0015:  ldc.i4.2
        IL_0016:  shr
        IL_0017:  add
        IL_0018:  add
        IL_0019:  add
        IL_001a:  stloc.0
        IL_001b:  ldc.i4     0x9e3779b9
        IL_0020:  ldarg.0
        IL_0021:  ldfld      !0 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T>::item1
        IL_0026:  stloc.1
        IL_0027:  ldarg.1
        IL_0028:  ldloc.1
        IL_0029:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericHashWithComparerIntrinsic<!T>(class [runtime]System.Collections.IEqualityComparer,
                                                                                                                                           !!0)
        IL_002e:  ldloc.0
        IL_002f:  ldc.i4.6
        IL_0030:  shl
        IL_0031:  ldloc.0
        IL_0032:  ldc.i4.2
        IL_0033:  shr
        IL_0034:  add
        IL_0035:  add
        IL_0036:  add
        IL_0037:  stloc.0
        IL_0038:  ldloc.0
        IL_0039:  ret
      } 

      .method public hidebysig virtual final instance int32  GetHashCode() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
        IL_0006:  call       instance int32 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T>::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
        IL_000b:  ret
      } 

      .method public hidebysig instance bool Equals(valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T> obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  5
        .locals init (!T V_0,
                 !T V_1)
        IL_0000:  ldarg.0
        IL_0001:  pop
        IL_0002:  ldarg.0
        IL_0003:  ldfld      !0 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T>::item1
        IL_0008:  stloc.0
        IL_0009:  ldarga.s   obj
        IL_000b:  ldfld      !0 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T>::item1
        IL_0010:  stloc.1
        IL_0011:  ldarg.2
        IL_0012:  ldloc.0
        IL_0013:  ldloc.1
        IL_0014:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericEqualityWithComparerIntrinsic<!T>(class [runtime]System.Collections.IEqualityComparer,
                                                                                                                                              !!0,
                                                                                                                                              !!0)
        IL_0019:  brfalse.s  IL_002b

        IL_001b:  ldarg.0
        IL_001c:  ldfld      int32 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T>::item2
        IL_0021:  ldarga.s   obj
        IL_0023:  ldfld      int32 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T>::item2
        IL_0028:  ceq
        IL_002a:  ret

        IL_002b:  ldc.i4.0
        IL_002c:  ret
      } 

      .method public hidebysig virtual final instance bool  Equals(object obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  5
        .locals init (valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T> V_0)
        IL_0000:  ldarg.1
        IL_0001:  isinst     valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T>
        IL_0006:  brfalse.s  IL_0018

        IL_0008:  ldarg.1
        IL_0009:  unbox.any  valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T>
        IL_000e:  stloc.0
        IL_000f:  ldarg.0
        IL_0010:  ldloc.0
        IL_0011:  ldarg.2
        IL_0012:  call       instance bool valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T>::Equals(valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!0>,
                                                                                                                                   class [runtime]System.Collections.IEqualityComparer)
        IL_0017:  ret

        IL_0018:  ldc.i4.0
        IL_0019:  ret
      } 

      .method public hidebysig virtual final instance bool  Equals(valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T> obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  4
        .locals init (!T V_0,
                 !T V_1)
        IL_0000:  ldarg.0
        IL_0001:  pop
        IL_0002:  ldarg.0
        IL_0003:  ldfld      !0 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T>::item1
        IL_0008:  stloc.0
        IL_0009:  ldarga.s   obj
        IL_000b:  ldfld      !0 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T>::item1
        IL_0010:  stloc.1
        IL_0011:  ldloc.0
        IL_0012:  ldloc.1
        IL_0013:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericEqualityERIntrinsic<!T>(!!0,
                                                                                                                                    !!0)
        IL_0018:  brfalse.s  IL_002a

        IL_001a:  ldarg.0
        IL_001b:  ldfld      int32 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T>::item2
        IL_0020:  ldarga.s   obj
        IL_0022:  ldfld      int32 valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T>::item2
        IL_0027:  ceq
        IL_0029:  ret

        IL_002a:  ldc.i4.0
        IL_002b:  ret
      } 

      .method public hidebysig virtual final instance bool  Equals(object obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.1
        IL_0001:  isinst     valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T>
        IL_0006:  brfalse.s  IL_0015

        IL_0008:  ldarg.0
        IL_0009:  ldarg.1
        IL_000a:  unbox.any  valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T>
        IL_000f:  call       instance bool valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!T>::Equals(valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!0>)
        IL_0014:  ret

        IL_0015:  ldc.i4.0
        IL_0016:  ret
      } 

      .property instance int32 Tag()
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
        .get instance int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1::get_Tag()
      } 
      .property instance !T Item1()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance !T assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1::get_Item1()
      } 
      .property instance int32 Item2()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 00 00 00 00 01 00 00 00 00 00 ) 
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1::get_Item2()
      } 
    } 

    .field static assembly bool arg@1
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<int32> x@1
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .field static assembly valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<int32> y@1
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .method assembly specialname static bool get_arg@1() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     bool assembly/EqualsMicroPerfAndCodeGenerationTests::arg@1
      IL_0005:  ret
    } 

    .method assembly specialname static valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<int32> get_x@1() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<int32> assembly/EqualsMicroPerfAndCodeGenerationTests::x@1
      IL_0005:  ret
    } 

    .method assembly specialname static valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<int32> get_y@1() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<int32> assembly/EqualsMicroPerfAndCodeGenerationTests::y@1
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
      IL_0002:  call       valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!0> valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<int32>::NewSomeGenericUnion(!0,
                                                                                                                                                                                                                   int32)
      IL_0007:  stsfld     valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<int32> assembly/EqualsMicroPerfAndCodeGenerationTests::x@1
      IL_000c:  ldc.i4.2
      IL_000d:  ldc.i4.3
      IL_000e:  call       valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!0> valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<int32>::NewSomeGenericUnion(!0,
                                                                                                                                                                                                                   int32)
      IL_0013:  stsfld     valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<int32> assembly/EqualsMicroPerfAndCodeGenerationTests::y@1
      IL_0018:  ldsflda    valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<int32> assembly/EqualsMicroPerfAndCodeGenerationTests::x@1
      IL_001d:  call       valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<int32> assembly/EqualsMicroPerfAndCodeGenerationTests::get_y@1()
      IL_0022:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0027:  call       instance bool valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<int32>::Equals(valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<!0>,
                                                                                                                                    class [runtime]System.Collections.IEqualityComparer)
      IL_002c:  stsfld     bool assembly/EqualsMicroPerfAndCodeGenerationTests::arg@1
      IL_0031:  ret
    } 

    .property bool arg@1()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get bool assembly/EqualsMicroPerfAndCodeGenerationTests::get_arg@1()
    } 
    .property valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<int32>
            x@1()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<int32> assembly/EqualsMicroPerfAndCodeGenerationTests::get_x@1()
    } 
    .property valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<int32>
            y@1()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get valuetype assembly/EqualsMicroPerfAndCodeGenerationTests/SomeGenericUnion`1<int32> assembly/EqualsMicroPerfAndCodeGenerationTests::get_y@1()
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






