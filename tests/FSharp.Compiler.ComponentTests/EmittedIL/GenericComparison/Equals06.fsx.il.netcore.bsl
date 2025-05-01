




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
    .class auto autochar serializable sealed nested public beforefieldinit GenericKey`1<a>
           extends [runtime]System.Object
           implements class [runtime]System.IEquatable`1<class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a>>,
                      [runtime]System.Collections.IStructuralEquatable,
                      class [runtime]System.IComparable`1<class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a>>,
                      [runtime]System.IComparable,
                      [runtime]System.Collections.IStructuralComparable
    {
      .custom instance void [runtime]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 7B 5F 5F 44 65 62 75 67 44 69 73 70 6C   
                                                                                                           61 79 28 29 2C 6E 71 7D 00 00 )                   
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 01 00 00 00 00 00 ) 
      .field assembly initonly !a item1
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field assembly initonly !a item2
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method public static class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a> NewGenericKey(!a item1, !a item2) cil managed
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32) = ( 01 00 08 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  newobj     instance void class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::.ctor(!0,
                                                                                                                        !0)
        IL_0007:  ret
      } 

      .method assembly specialname rtspecialname instance void  .ctor(!a item1, !a item2) cil managed
      {
        .custom instance void [runtime]System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype [runtime]System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                                                class [runtime]System.Type) = ( 01 00 60 06 00 00 3B 45 71 75 61 6C 73 30 36 2B   
                                                                                                                                                       45 71 75 61 6C 73 4D 69 63 72 6F 50 65 72 66 41   
                                                                                                                                                       6E 64 43 6F 64 65 47 65 6E 65 72 61 74 69 6F 6E   
                                                                                                                                                       54 65 73 74 73 2B 47 65 6E 65 72 69 63 4B 65 79   
                                                                                                                                                       60 31 00 00 )                                     
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [runtime]System.Object::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      !0 class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::item1
        IL_000d:  ldarg.0
        IL_000e:  ldarg.2
        IL_000f:  stfld      !0 class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::item2
        IL_0014:  ret
      } 

      .method public hidebysig instance !a get_Item1() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      !0 class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::item1
        IL_0006:  ret
      } 

      .method public hidebysig instance !a get_Item2() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      !0 class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::item2
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
        IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a>,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,string>::.ctor(string)
        IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a>,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
        IL_000f:  ldarg.0
        IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a>,string>::Invoke(!0)
        IL_0015:  ret
      } 

      .method public strict virtual instance string ToString() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldstr      "%+A"
        IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a>,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a>>::.ctor(string)
        IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a>,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
        IL_000f:  ldarg.0
        IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a>,string>::Invoke(!0)
        IL_0015:  ret
      } 

      .method public hidebysig virtual final instance int32  CompareTo(class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a> obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  5
        .locals init (class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a> V_0,
                 class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a> V_1,
                 int32 V_2,
                 class [runtime]System.Collections.IComparer V_3,
                 !a V_4,
                 !a V_5)
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_005e

        IL_0003:  ldarg.1
        IL_0004:  brfalse.s  IL_005c

        IL_0006:  ldarg.0
        IL_0007:  pop
        IL_0008:  ldarg.0
        IL_0009:  stloc.0
        IL_000a:  ldarg.1
        IL_000b:  stloc.1
        IL_000c:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_0011:  stloc.3
        IL_0012:  ldloc.0
        IL_0013:  ldfld      !0 class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::item1
        IL_0018:  stloc.s    V_4
        IL_001a:  ldloc.1
        IL_001b:  ldfld      !0 class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::item1
        IL_0020:  stloc.s    V_5
        IL_0022:  ldloc.3
        IL_0023:  ldloc.s    V_4
        IL_0025:  ldloc.s    V_5
        IL_0027:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericComparisonWithComparerIntrinsic<!a>(class [runtime]System.Collections.IComparer,
                                                                                                                                                 !!0,
                                                                                                                                                 !!0)
        IL_002c:  stloc.2
        IL_002d:  ldloc.2
        IL_002e:  ldc.i4.0
        IL_002f:  bge.s      IL_0033

        IL_0031:  ldloc.2
        IL_0032:  ret

        IL_0033:  ldloc.2
        IL_0034:  ldc.i4.0
        IL_0035:  ble.s      IL_0039

        IL_0037:  ldloc.2
        IL_0038:  ret

        IL_0039:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
        IL_003e:  stloc.3
        IL_003f:  ldloc.0
        IL_0040:  ldfld      !0 class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::item2
        IL_0045:  stloc.s    V_4
        IL_0047:  ldloc.1
        IL_0048:  ldfld      !0 class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::item2
        IL_004d:  stloc.s    V_5
        IL_004f:  ldloc.3
        IL_0050:  ldloc.s    V_4
        IL_0052:  ldloc.s    V_5
        IL_0054:  tail.
        IL_0056:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericComparisonWithComparerIntrinsic<!a>(class [runtime]System.Collections.IComparer,
                                                                                                                                                 !!0,
                                                                                                                                                 !!0)
        IL_005b:  ret

        IL_005c:  ldc.i4.1
        IL_005d:  ret

        IL_005e:  ldarg.1
        IL_005f:  brfalse.s  IL_0063

        IL_0061:  ldc.i4.m1
        IL_0062:  ret

        IL_0063:  ldc.i4.0
        IL_0064:  ret
      } 

      .method public hidebysig virtual final instance int32  CompareTo(object obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  unbox.any  class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a>
        IL_0007:  tail.
        IL_0009:  callvirt   instance int32 class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::CompareTo(class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!0>)
        IL_000e:  ret
      } 

      .method public hidebysig virtual final instance int32  CompareTo(object obj, class [runtime]System.Collections.IComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  5
        .locals init (class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a> V_0,
                 class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a> V_1,
                 class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a> V_2,
                 int32 V_3,
                 !a V_4,
                 !a V_5)
        IL_0000:  ldarg.1
        IL_0001:  unbox.any  class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a>
        IL_0006:  stloc.0
        IL_0007:  ldarg.0
        IL_0008:  brfalse.s  IL_005e

        IL_000a:  ldarg.1
        IL_000b:  unbox.any  class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a>
        IL_0010:  brfalse.s  IL_005c

        IL_0012:  ldarg.0
        IL_0013:  pop
        IL_0014:  ldarg.0
        IL_0015:  stloc.1
        IL_0016:  ldloc.0
        IL_0017:  stloc.2
        IL_0018:  ldloc.1
        IL_0019:  ldfld      !0 class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::item1
        IL_001e:  stloc.s    V_4
        IL_0020:  ldloc.2
        IL_0021:  ldfld      !0 class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::item1
        IL_0026:  stloc.s    V_5
        IL_0028:  ldarg.2
        IL_0029:  ldloc.s    V_4
        IL_002b:  ldloc.s    V_5
        IL_002d:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericComparisonWithComparerIntrinsic<!a>(class [runtime]System.Collections.IComparer,
                                                                                                                                                 !!0,
                                                                                                                                                 !!0)
        IL_0032:  stloc.3
        IL_0033:  ldloc.3
        IL_0034:  ldc.i4.0
        IL_0035:  bge.s      IL_0039

        IL_0037:  ldloc.3
        IL_0038:  ret

        IL_0039:  ldloc.3
        IL_003a:  ldc.i4.0
        IL_003b:  ble.s      IL_003f

        IL_003d:  ldloc.3
        IL_003e:  ret

        IL_003f:  ldloc.1
        IL_0040:  ldfld      !0 class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::item2
        IL_0045:  stloc.s    V_4
        IL_0047:  ldloc.2
        IL_0048:  ldfld      !0 class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::item2
        IL_004d:  stloc.s    V_5
        IL_004f:  ldarg.2
        IL_0050:  ldloc.s    V_4
        IL_0052:  ldloc.s    V_5
        IL_0054:  tail.
        IL_0056:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericComparisonWithComparerIntrinsic<!a>(class [runtime]System.Collections.IComparer,
                                                                                                                                                 !!0,
                                                                                                                                                 !!0)
        IL_005b:  ret

        IL_005c:  ldc.i4.1
        IL_005d:  ret

        IL_005e:  ldarg.1
        IL_005f:  unbox.any  class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a>
        IL_0064:  brfalse.s  IL_0068

        IL_0066:  ldc.i4.m1
        IL_0067:  ret

        IL_0068:  ldc.i4.0
        IL_0069:  ret
      } 

      .method public hidebysig virtual final instance int32  GetHashCode(class [runtime]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  7
        .locals init (int32 V_0,
                 class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a> V_1,
                 !a V_2)
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_0047

        IL_0003:  ldc.i4.0
        IL_0004:  stloc.0
        IL_0005:  ldarg.0
        IL_0006:  pop
        IL_0007:  ldarg.0
        IL_0008:  stloc.1
        IL_0009:  ldc.i4.0
        IL_000a:  stloc.0
        IL_000b:  ldc.i4     0x9e3779b9
        IL_0010:  ldloc.1
        IL_0011:  ldfld      !0 class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::item2
        IL_0016:  stloc.2
        IL_0017:  ldarg.1
        IL_0018:  ldloc.2
        IL_0019:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericHashWithComparerIntrinsic<!a>(class [runtime]System.Collections.IEqualityComparer,
                                                                                                                                           !!0)
        IL_001e:  ldloc.0
        IL_001f:  ldc.i4.6
        IL_0020:  shl
        IL_0021:  ldloc.0
        IL_0022:  ldc.i4.2
        IL_0023:  shr
        IL_0024:  add
        IL_0025:  add
        IL_0026:  add
        IL_0027:  stloc.0
        IL_0028:  ldc.i4     0x9e3779b9
        IL_002d:  ldloc.1
        IL_002e:  ldfld      !0 class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::item1
        IL_0033:  stloc.2
        IL_0034:  ldarg.1
        IL_0035:  ldloc.2
        IL_0036:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericHashWithComparerIntrinsic<!a>(class [runtime]System.Collections.IEqualityComparer,
                                                                                                                                           !!0)
        IL_003b:  ldloc.0
        IL_003c:  ldc.i4.6
        IL_003d:  shl
        IL_003e:  ldloc.0
        IL_003f:  ldc.i4.2
        IL_0040:  shr
        IL_0041:  add
        IL_0042:  add
        IL_0043:  add
        IL_0044:  stloc.0
        IL_0045:  ldloc.0
        IL_0046:  ret

        IL_0047:  ldc.i4.0
        IL_0048:  ret
      } 

      .method public hidebysig virtual final instance int32  GetHashCode() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
        IL_0006:  callvirt   instance int32 class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
        IL_000b:  ret
      } 

      .method public hidebysig instance bool Equals(class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a> obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  5
        .locals init (class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a> V_0,
                 class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a> V_1,
                 !a V_2,
                 !a V_3)
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_0041

        IL_0003:  ldarg.1
        IL_0004:  brfalse.s  IL_003f

        IL_0006:  ldarg.0
        IL_0007:  pop
        IL_0008:  ldarg.0
        IL_0009:  stloc.0
        IL_000a:  ldarg.1
        IL_000b:  stloc.1
        IL_000c:  ldloc.0
        IL_000d:  ldfld      !0 class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::item1
        IL_0012:  stloc.2
        IL_0013:  ldloc.1
        IL_0014:  ldfld      !0 class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::item1
        IL_0019:  stloc.3
        IL_001a:  ldarg.2
        IL_001b:  ldloc.2
        IL_001c:  ldloc.3
        IL_001d:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericEqualityWithComparerIntrinsic<!a>(class [runtime]System.Collections.IEqualityComparer,
                                                                                                                                              !!0,
                                                                                                                                              !!0)
        IL_0022:  brfalse.s  IL_003d

        IL_0024:  ldloc.0
        IL_0025:  ldfld      !0 class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::item2
        IL_002a:  stloc.2
        IL_002b:  ldloc.1
        IL_002c:  ldfld      !0 class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::item2
        IL_0031:  stloc.3
        IL_0032:  ldarg.2
        IL_0033:  ldloc.2
        IL_0034:  ldloc.3
        IL_0035:  tail.
        IL_0037:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericEqualityWithComparerIntrinsic<!a>(class [runtime]System.Collections.IEqualityComparer,
                                                                                                                                              !!0,
                                                                                                                                              !!0)
        IL_003c:  ret

        IL_003d:  ldc.i4.0
        IL_003e:  ret

        IL_003f:  ldc.i4.0
        IL_0040:  ret

        IL_0041:  ldarg.1
        IL_0042:  ldnull
        IL_0043:  cgt.un
        IL_0045:  ldc.i4.0
        IL_0046:  ceq
        IL_0048:  ret
      } 

      .method public hidebysig virtual final instance bool  Equals(object obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  5
        .locals init (class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a> V_0)
        IL_0000:  ldarg.1
        IL_0001:  isinst     class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a>
        IL_0006:  stloc.0
        IL_0007:  ldloc.0
        IL_0008:  brfalse.s  IL_0015

        IL_000a:  ldarg.0
        IL_000b:  ldloc.0
        IL_000c:  ldarg.2
        IL_000d:  tail.
        IL_000f:  callvirt   instance bool class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::Equals(class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!0>,
                                                                                                                         class [runtime]System.Collections.IEqualityComparer)
        IL_0014:  ret

        IL_0015:  ldc.i4.0
        IL_0016:  ret
      } 

      .method public hidebysig virtual final instance bool  Equals(class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a> obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  4
        .locals init (class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a> V_0,
                 class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a> V_1,
                 !a V_2,
                 !a V_3)
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_003f

        IL_0003:  ldarg.1
        IL_0004:  brfalse.s  IL_003d

        IL_0006:  ldarg.0
        IL_0007:  pop
        IL_0008:  ldarg.0
        IL_0009:  stloc.0
        IL_000a:  ldarg.1
        IL_000b:  stloc.1
        IL_000c:  ldloc.0
        IL_000d:  ldfld      !0 class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::item1
        IL_0012:  stloc.2
        IL_0013:  ldloc.1
        IL_0014:  ldfld      !0 class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::item1
        IL_0019:  stloc.3
        IL_001a:  ldloc.2
        IL_001b:  ldloc.3
        IL_001c:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericEqualityERIntrinsic<!a>(!!0,
                                                                                                                                    !!0)
        IL_0021:  brfalse.s  IL_003b

        IL_0023:  ldloc.0
        IL_0024:  ldfld      !0 class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::item2
        IL_0029:  stloc.2
        IL_002a:  ldloc.1
        IL_002b:  ldfld      !0 class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::item2
        IL_0030:  stloc.3
        IL_0031:  ldloc.2
        IL_0032:  ldloc.3
        IL_0033:  tail.
        IL_0035:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericEqualityERIntrinsic<!a>(!!0,
                                                                                                                                    !!0)
        IL_003a:  ret

        IL_003b:  ldc.i4.0
        IL_003c:  ret

        IL_003d:  ldc.i4.0
        IL_003e:  ret

        IL_003f:  ldarg.1
        IL_0040:  ldnull
        IL_0041:  cgt.un
        IL_0043:  ldc.i4.0
        IL_0044:  ceq
        IL_0046:  ret
      } 

      .method public hidebysig virtual final instance bool  Equals(object obj) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  4
        .locals init (class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a> V_0)
        IL_0000:  ldarg.1
        IL_0001:  isinst     class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a>
        IL_0006:  stloc.0
        IL_0007:  ldloc.0
        IL_0008:  brfalse.s  IL_0014

        IL_000a:  ldarg.0
        IL_000b:  ldloc.0
        IL_000c:  tail.
        IL_000e:  callvirt   instance bool class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!a>::Equals(class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!0>)
        IL_0013:  ret

        IL_0014:  ldc.i4.0
        IL_0015:  ret
      } 

      .property instance int32 Tag()
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
        .get instance int32 assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1::get_Tag()
      } 
      .property instance !a Item1()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance !a assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1::get_Item1()
      } 
      .property instance !a Item2()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 00 00 00 00 01 00 00 00 00 00 ) 
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance !a assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1::get_Item2()
      } 
    } 

    .method public static bool  f6() cil managed
    {
      
      .maxstack  5
      .locals init (bool V_0,
               class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<int32> V_1,
               class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<int32> V_2,
               int32 V_3)
      IL_0000:  ldc.i4.0
      IL_0001:  stloc.0
      IL_0002:  ldc.i4.1
      IL_0003:  ldc.i4.2
      IL_0004:  call       class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!0> class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<int32>::NewGenericKey(!0,
                                                                                                                                                                                         !0)
      IL_0009:  stloc.1
      IL_000a:  ldc.i4.1
      IL_000b:  ldc.i4.3
      IL_000c:  call       class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!0> class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<int32>::NewGenericKey(!0,
                                                                                                                                                                                         !0)
      IL_0011:  stloc.2
      IL_0012:  ldc.i4.0
      IL_0013:  stloc.3
      IL_0014:  br.s       IL_0027

      IL_0016:  ldloc.1
      IL_0017:  ldloc.2
      IL_0018:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_001d:  callvirt   instance bool class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<int32>::Equals(class assembly/EqualsMicroPerfAndCodeGenerationTests/GenericKey`1<!0>,
                                                                                                                          class [runtime]System.Collections.IEqualityComparer)
      IL_0022:  stloc.0
      IL_0023:  ldloc.3
      IL_0024:  ldc.i4.1
      IL_0025:  add
      IL_0026:  stloc.3
      IL_0027:  ldloc.3
      IL_0028:  ldc.i4     0x989681
      IL_002d:  blt.s      IL_0016

      IL_002f:  ldloc.0
      IL_0030:  ret
    } 

  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly$fsx
       extends [runtime]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  8
    IL_0000:  ret
  } 

} 






