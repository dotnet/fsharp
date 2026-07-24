




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
  .field static assembly class '<>f__AnonymousType3986374330`2'<class '<>f__AnonymousType3104616430`2'<string,string>,class '<>f__AnonymousType3104616430`2'<string,string>> actual@4
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class '<>f__AnonymousType3986374330`2'<class '<>f__AnonymousType3104616430`2'<string,string>,class '<>f__AnonymousType3104616430`2'<string,string>> bind@4
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class '<>f__AnonymousType1074009332`1'<class '<>f__AnonymousType3104616430`2'<string,string>> 'bind@4-1'
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class '<>f__AnonymousType3104616430`2'<string,string> inputRecord@4
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public static class '<>f__AnonymousType3986374330`2'<class '<>f__AnonymousType3104616430`2'<string,string>,class '<>f__AnonymousType3104616430`2'<string,string>> orig1() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldstr      "value1"
    IL_0005:  ldstr      "value1"
    IL_000a:  newobj     instance void class '<>f__AnonymousType3104616430`2'<string,string>::.ctor(!0,
                                                                                                    !1)
    IL_000f:  ldstr      "value2"
    IL_0014:  ldstr      "value2"
    IL_0019:  newobj     instance void class '<>f__AnonymousType3104616430`2'<string,string>::.ctor(!0,
                                                                                                    !1)
    IL_001e:  newobj     instance void class '<>f__AnonymousType3986374330`2'<class '<>f__AnonymousType3104616430`2'<string,string>,class '<>f__AnonymousType3104616430`2'<string,string>>::.ctor(!0,
                                                                                                                                                                                                  !1)
    IL_0023:  ret
  } 

  .method public static class '<>f__AnonymousType1074009332`1'<class '<>f__AnonymousType3104616430`2'<string,string>> orig2() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldstr      "value3"
    IL_0005:  ldstr      "value3"
    IL_000a:  newobj     instance void class '<>f__AnonymousType3104616430`2'<string,string>::.ctor(!0,
                                                                                                    !1)
    IL_000f:  newobj     instance void class '<>f__AnonymousType1074009332`1'<class '<>f__AnonymousType3104616430`2'<string,string>>::.ctor(!0)
    IL_0014:  ret
  } 

  .method public specialname static class '<>f__AnonymousType3986374330`2'<class '<>f__AnonymousType3104616430`2'<string,string>,class '<>f__AnonymousType3104616430`2'<string,string>> get_actual() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class '<>f__AnonymousType3986374330`2'<class '<>f__AnonymousType3104616430`2'<string,string>,class '<>f__AnonymousType3104616430`2'<string,string>> assembly::actual@4
    IL_0005:  ret
  } 

  .method assembly specialname static class '<>f__AnonymousType3986374330`2'<class '<>f__AnonymousType3104616430`2'<string,string>,class '<>f__AnonymousType3104616430`2'<string,string>> get_bind@4() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class '<>f__AnonymousType3986374330`2'<class '<>f__AnonymousType3104616430`2'<string,string>,class '<>f__AnonymousType3104616430`2'<string,string>> assembly::bind@4
    IL_0005:  ret
  } 

  .method assembly specialname static class '<>f__AnonymousType1074009332`1'<class '<>f__AnonymousType3104616430`2'<string,string>> 'get_bind@4-1'() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class '<>f__AnonymousType1074009332`1'<class '<>f__AnonymousType3104616430`2'<string,string>> assembly::'bind@4-1'
    IL_0005:  ret
  } 

  .method assembly specialname static class '<>f__AnonymousType3104616430`2'<string,string> get_inputRecord@4() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class '<>f__AnonymousType3104616430`2'<string,string> assembly::inputRecord@4
    IL_0005:  ret
  } 

  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$assembly::init@
    IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$assembly::init@
    IL_000b:  pop
    IL_000c:  ret
  } 

  .method assembly static void  staticInitialization@() cil managed
  {
    
    .maxstack  5
    IL_0000:  nop
    IL_0001:  ldstr      "value1"
    IL_0006:  ldstr      "value1"
    IL_000b:  newobj     instance void class '<>f__AnonymousType3104616430`2'<string,string>::.ctor(!0,
                                                                                                    !1)
    IL_0010:  ldstr      "value2"
    IL_0015:  ldstr      "value2"
    IL_001a:  newobj     instance void class '<>f__AnonymousType3104616430`2'<string,string>::.ctor(!0,
                                                                                                    !1)
    IL_001f:  newobj     instance void class '<>f__AnonymousType3986374330`2'<class '<>f__AnonymousType3104616430`2'<string,string>,class '<>f__AnonymousType3104616430`2'<string,string>>::.ctor(!0,
                                                                                                                                                                                                  !1)
    IL_0024:  stsfld     class '<>f__AnonymousType3986374330`2'<class '<>f__AnonymousType3104616430`2'<string,string>,class '<>f__AnonymousType3104616430`2'<string,string>> assembly::bind@4
    IL_0029:  ldstr      "value3"
    IL_002e:  ldstr      "value3"
    IL_0033:  newobj     instance void class '<>f__AnonymousType3104616430`2'<string,string>::.ctor(!0,
                                                                                                    !1)
    IL_0038:  newobj     instance void class '<>f__AnonymousType1074009332`1'<class '<>f__AnonymousType3104616430`2'<string,string>>::.ctor(!0)
    IL_003d:  stsfld     class '<>f__AnonymousType1074009332`1'<class '<>f__AnonymousType3104616430`2'<string,string>> assembly::'bind@4-1'
    IL_0042:  call       class '<>f__AnonymousType1074009332`1'<class '<>f__AnonymousType3104616430`2'<string,string>> assembly::'get_bind@4-1'()
    IL_0047:  call       instance !0 class '<>f__AnonymousType1074009332`1'<class '<>f__AnonymousType3104616430`2'<string,string>>::get_Nested()
    IL_004c:  call       class '<>f__AnonymousType3986374330`2'<class '<>f__AnonymousType3104616430`2'<string,string>,class '<>f__AnonymousType3104616430`2'<string,string>> assembly::get_bind@4()
    IL_0051:  call       instance !1 class '<>f__AnonymousType3986374330`2'<class '<>f__AnonymousType3104616430`2'<string,string>,class '<>f__AnonymousType3104616430`2'<string,string>>::get_Other()
    IL_0056:  stsfld     class '<>f__AnonymousType3104616430`2'<string,string> assembly::inputRecord@4
    IL_005b:  call       class '<>f__AnonymousType3104616430`2'<string,string> assembly::get_inputRecord@4()
    IL_0060:  call       instance !0 class '<>f__AnonymousType3104616430`2'<string,string>::get_A()
    IL_0065:  ldstr      "value5"
    IL_006a:  newobj     instance void class '<>f__AnonymousType3104616430`2'<string,string>::.ctor(!0,
                                                                                                    !1)
    IL_006f:  newobj     instance void class '<>f__AnonymousType3986374330`2'<class '<>f__AnonymousType3104616430`2'<string,string>,class '<>f__AnonymousType3104616430`2'<string,string>>::.ctor(!0,
                                                                                                                                                                                                  !1)
    IL_0074:  stsfld     class '<>f__AnonymousType3986374330`2'<class '<>f__AnonymousType3104616430`2'<string,string>,class '<>f__AnonymousType3104616430`2'<string,string>> assembly::actual@4
    IL_0079:  ret
  } 

  .property class '<>f__AnonymousType3986374330`2'<class '<>f__AnonymousType3104616430`2'<string,string>,class '<>f__AnonymousType3104616430`2'<string,string>>
          actual()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class '<>f__AnonymousType3986374330`2'<class '<>f__AnonymousType3104616430`2'<string,string>,class '<>f__AnonymousType3104616430`2'<string,string>> assembly::get_actual()
  } 
  .property class '<>f__AnonymousType3986374330`2'<class '<>f__AnonymousType3104616430`2'<string,string>,class '<>f__AnonymousType3104616430`2'<string,string>>
          bind@4()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class '<>f__AnonymousType3986374330`2'<class '<>f__AnonymousType3104616430`2'<string,string>,class '<>f__AnonymousType3104616430`2'<string,string>> assembly::get_bind@4()
  } 
  .property class '<>f__AnonymousType1074009332`1'<class '<>f__AnonymousType3104616430`2'<string,string>>
          'bind@4-1'()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class '<>f__AnonymousType1074009332`1'<class '<>f__AnonymousType3104616430`2'<string,string>> assembly::'get_bind@4-1'()
  } 
  .property class '<>f__AnonymousType3104616430`2'<string,string>
          inputRecord@4()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class '<>f__AnonymousType3104616430`2'<string,string> assembly::get_inputRecord@4()
  } 
} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly
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

.class public auto ansi serializable sealed beforefieldinit '<>f__AnonymousType1074009332`1'<'<Nested>j__TPar'>
       extends [runtime]System.Object
       implements [runtime]System.Collections.IStructuralComparable,
                  [runtime]System.IComparable,
                  class [runtime]System.IComparable`1<class '<>f__AnonymousType1074009332`1'<!'<Nested>j__TPar'>>,
                  [runtime]System.Collections.IStructuralEquatable,
                  class [runtime]System.IEquatable`1<class '<>f__AnonymousType1074009332`1'<!'<Nested>j__TPar'>>
{
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 02 00 00 00 00 00 ) 
  .field private !'<Nested>j__TPar' Nested@
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public specialname rtspecialname instance void  .ctor(!'<Nested>j__TPar' Nested) cil managed
  {
    .custom instance void [runtime]System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype [runtime]System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                                            class [runtime]System.Type) = ( 01 00 60 06 00 00 1E 3C 3E 66 5F 5F 41 6E 6F 6E   
                                                                                                                                                   79 6D 6F 75 73 54 79 70 65 31 30 37 34 30 30 39   
                                                                                                                                                   33 33 32 60 31 00 00 )                            
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [runtime]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousType1074009332`1'<!'<Nested>j__TPar'>::Nested@
    IL_000d:  ret
  } 

  .method public hidebysig specialname instance !'<Nested>j__TPar' get_Nested() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType1074009332`1'<!'<Nested>j__TPar'>::Nested@
    IL_0006:  ret
  } 

  .method public strict virtual instance string ToString() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldstr      "%+A"
    IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class '<>f__AnonymousType1074009332`1'<!'<Nested>j__TPar'>,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class '<>f__AnonymousType1074009332`1'<!'<Nested>j__TPar'>>::.ctor(string)
    IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class '<>f__AnonymousType1074009332`1'<!'<Nested>j__TPar'>,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
    IL_000f:  ldarg.0
    IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class '<>f__AnonymousType1074009332`1'<!'<Nested>j__TPar'>,string>::Invoke(!0)
    IL_0015:  ret
  } 

  .method public hidebysig virtual final instance int32  CompareTo(class '<>f__AnonymousType1074009332`1'<!'<Nested>j__TPar'> obj) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_0021

    IL_0003:  ldarg.1
    IL_0004:  brfalse.s  IL_001f

    IL_0006:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
    IL_000b:  ldarg.0
    IL_000c:  ldfld      !0 class '<>f__AnonymousType1074009332`1'<!'<Nested>j__TPar'>::Nested@
    IL_0011:  ldarg.1
    IL_0012:  ldfld      !0 class '<>f__AnonymousType1074009332`1'<!'<Nested>j__TPar'>::Nested@
    IL_0017:  tail.
    IL_0019:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericComparisonWithComparer<!'<Nested>j__TPar'>(class [runtime]System.Collections.IComparer,
                                                                                                                                        !!0,
                                                                                                                                        !!0)
    IL_001e:  ret

    IL_001f:  ldc.i4.1
    IL_0020:  ret

    IL_0021:  ldarg.1
    IL_0022:  brfalse.s  IL_0026

    IL_0024:  ldc.i4.m1
    IL_0025:  ret

    IL_0026:  ldc.i4.0
    IL_0027:  ret
  } 

  .method public hidebysig virtual final instance int32  CompareTo(object obj) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  unbox.any  class '<>f__AnonymousType1074009332`1'<!'<Nested>j__TPar'>
    IL_0007:  tail.
    IL_0009:  callvirt   instance int32 class '<>f__AnonymousType1074009332`1'<!'<Nested>j__TPar'>::CompareTo(class '<>f__AnonymousType1074009332`1'<!0>)
    IL_000e:  ret
  } 

  .method public hidebysig virtual final instance int32  CompareTo(object obj, class [runtime]System.Collections.IComparer comp) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  5
    .locals init (class '<>f__AnonymousType1074009332`1'<!'<Nested>j__TPar'> V_0,
             class '<>f__AnonymousType1074009332`1'<!'<Nested>j__TPar'> V_1)
    IL_0000:  ldarg.1
    IL_0001:  unbox.any  class '<>f__AnonymousType1074009332`1'<!'<Nested>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  stloc.1
    IL_0009:  ldarg.0
    IL_000a:  brfalse.s  IL_002b

    IL_000c:  ldarg.1
    IL_000d:  unbox.any  class '<>f__AnonymousType1074009332`1'<!'<Nested>j__TPar'>
    IL_0012:  brfalse.s  IL_0029

    IL_0014:  ldarg.2
    IL_0015:  ldarg.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType1074009332`1'<!'<Nested>j__TPar'>::Nested@
    IL_001b:  ldloc.1
    IL_001c:  ldfld      !0 class '<>f__AnonymousType1074009332`1'<!'<Nested>j__TPar'>::Nested@
    IL_0021:  tail.
    IL_0023:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericComparisonWithComparer<!'<Nested>j__TPar'>(class [runtime]System.Collections.IComparer,
                                                                                                                                        !!0,
                                                                                                                                        !!0)
    IL_0028:  ret

    IL_0029:  ldc.i4.1
    IL_002a:  ret

    IL_002b:  ldarg.1
    IL_002c:  unbox.any  class '<>f__AnonymousType1074009332`1'<!'<Nested>j__TPar'>
    IL_0031:  brfalse.s  IL_0035

    IL_0033:  ldc.i4.m1
    IL_0034:  ret

    IL_0035:  ldc.i4.0
    IL_0036:  ret
  } 

  .method public hidebysig virtual final instance int32  GetHashCode(class [runtime]System.Collections.IEqualityComparer comp) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  7
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_0022

    IL_0003:  ldc.i4.0
    IL_0004:  stloc.0
    IL_0005:  ldc.i4     0x9e3779b9
    IL_000a:  ldarg.1
    IL_000b:  ldarg.0
    IL_000c:  ldfld      !0 class '<>f__AnonymousType1074009332`1'<!'<Nested>j__TPar'>::Nested@
    IL_0011:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericHashWithComparer<!'<Nested>j__TPar'>(class [runtime]System.Collections.IEqualityComparer,
                                                                                                                                  !!0)
    IL_0016:  ldloc.0
    IL_0017:  ldc.i4.6
    IL_0018:  shl
    IL_0019:  ldloc.0
    IL_001a:  ldc.i4.2
    IL_001b:  shr
    IL_001c:  add
    IL_001d:  add
    IL_001e:  add
    IL_001f:  stloc.0
    IL_0020:  ldloc.0
    IL_0021:  ret

    IL_0022:  ldc.i4.0
    IL_0023:  ret
  } 

  .method public hidebysig virtual final instance int32  GetHashCode() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
    IL_0006:  tail.
    IL_0008:  callvirt   instance int32 class '<>f__AnonymousType1074009332`1'<!'<Nested>j__TPar'>::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
    IL_000d:  ret
  } 

  .method public hidebysig instance bool Equals(class '<>f__AnonymousType1074009332`1'<!'<Nested>j__TPar'> obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  5
    .locals init (class '<>f__AnonymousType1074009332`1'<!'<Nested>j__TPar'> V_0)
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_001f

    IL_0003:  ldarg.1
    IL_0004:  brfalse.s  IL_001d

    IL_0006:  ldarg.1
    IL_0007:  stloc.0
    IL_0008:  ldarg.2
    IL_0009:  ldarg.0
    IL_000a:  ldfld      !0 class '<>f__AnonymousType1074009332`1'<!'<Nested>j__TPar'>::Nested@
    IL_000f:  ldloc.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType1074009332`1'<!'<Nested>j__TPar'>::Nested@
    IL_0015:  tail.
    IL_0017:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericEqualityWithComparer<!'<Nested>j__TPar'>(class [runtime]System.Collections.IEqualityComparer,
                                                                                                                                     !!0,
                                                                                                                                     !!0)
    IL_001c:  ret

    IL_001d:  ldc.i4.0
    IL_001e:  ret

    IL_001f:  ldarg.1
    IL_0020:  ldnull
    IL_0021:  cgt.un
    IL_0023:  ldc.i4.0
    IL_0024:  ceq
    IL_0026:  ret
  } 

  .method public hidebysig virtual final instance bool  Equals(object obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  5
    .locals init (class '<>f__AnonymousType1074009332`1'<!'<Nested>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType1074009332`1'<!'<Nested>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0015

    IL_000a:  ldarg.0
    IL_000b:  ldloc.0
    IL_000c:  ldarg.2
    IL_000d:  tail.
    IL_000f:  callvirt   instance bool class '<>f__AnonymousType1074009332`1'<!'<Nested>j__TPar'>::Equals(class '<>f__AnonymousType1074009332`1'<!0>,
                                                                                                          class [runtime]System.Collections.IEqualityComparer)
    IL_0014:  ret

    IL_0015:  ldc.i4.0
    IL_0016:  ret
  } 

  .method public hidebysig virtual final instance bool  Equals(class '<>f__AnonymousType1074009332`1'<!'<Nested>j__TPar'> obj) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_001c

    IL_0003:  ldarg.1
    IL_0004:  brfalse.s  IL_001a

    IL_0006:  ldarg.0
    IL_0007:  ldfld      !0 class '<>f__AnonymousType1074009332`1'<!'<Nested>j__TPar'>::Nested@
    IL_000c:  ldarg.1
    IL_000d:  ldfld      !0 class '<>f__AnonymousType1074009332`1'<!'<Nested>j__TPar'>::Nested@
    IL_0012:  tail.
    IL_0014:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericEqualityER<!'<Nested>j__TPar'>(!!0,
                                                                                                                           !!0)
    IL_0019:  ret

    IL_001a:  ldc.i4.0
    IL_001b:  ret

    IL_001c:  ldarg.1
    IL_001d:  ldnull
    IL_001e:  cgt.un
    IL_0020:  ldc.i4.0
    IL_0021:  ceq
    IL_0023:  ret
  } 

  .method public hidebysig virtual final instance bool  Equals(object obj) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  4
    .locals init (class '<>f__AnonymousType1074009332`1'<!'<Nested>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType1074009332`1'<!'<Nested>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0014

    IL_000a:  ldarg.0
    IL_000b:  ldloc.0
    IL_000c:  tail.
    IL_000e:  callvirt   instance bool class '<>f__AnonymousType1074009332`1'<!'<Nested>j__TPar'>::Equals(class '<>f__AnonymousType1074009332`1'<!0>)
    IL_0013:  ret

    IL_0014:  ldc.i4.0
    IL_0015:  ret
  } 

  .property instance !'<Nested>j__TPar' Nested()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
    .get instance !'<Nested>j__TPar' '<>f__AnonymousType1074009332`1'::get_Nested()
  } 
} 

.class public auto ansi serializable sealed beforefieldinit '<>f__AnonymousType3104616430`2'<'<A>j__TPar','<B>j__TPar'>
       extends [runtime]System.Object
       implements [runtime]System.Collections.IStructuralComparable,
                  [runtime]System.IComparable,
                  class [runtime]System.IComparable`1<class '<>f__AnonymousType3104616430`2'<!'<A>j__TPar',!'<B>j__TPar'>>,
                  [runtime]System.Collections.IStructuralEquatable,
                  class [runtime]System.IEquatable`1<class '<>f__AnonymousType3104616430`2'<!'<A>j__TPar',!'<B>j__TPar'>>
{
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 02 00 00 00 00 00 ) 
  .field private !'<A>j__TPar' A@
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private !'<B>j__TPar' B@
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public specialname rtspecialname instance void  .ctor(!'<A>j__TPar' A, !'<B>j__TPar' B) cil managed
  {
    .custom instance void [runtime]System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype [runtime]System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                                            class [runtime]System.Type) = ( 01 00 60 06 00 00 1E 3C 3E 66 5F 5F 41 6E 6F 6E   
                                                                                                                                                   79 6D 6F 75 73 54 79 70 65 33 31 30 34 36 31 36   
                                                                                                                                                   34 33 30 60 32 00 00 )                            
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [runtime]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousType3104616430`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      !1 class '<>f__AnonymousType3104616430`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0014:  ret
  } 

  .method public hidebysig specialname instance !'<A>j__TPar' get_A() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType3104616430`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0006:  ret
  } 

  .method public hidebysig specialname instance !'<B>j__TPar' get_B() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType3104616430`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0006:  ret
  } 

  .method public strict virtual instance string ToString() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldstr      "%+A"
    IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class '<>f__AnonymousType3104616430`2'<!'<A>j__TPar',!'<B>j__TPar'>,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class '<>f__AnonymousType3104616430`2'<!'<A>j__TPar',!'<B>j__TPar'>>::.ctor(string)
    IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class '<>f__AnonymousType3104616430`2'<!'<A>j__TPar',!'<B>j__TPar'>,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
    IL_000f:  ldarg.0
    IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class '<>f__AnonymousType3104616430`2'<!'<A>j__TPar',!'<B>j__TPar'>,string>::Invoke(!0)
    IL_0015:  ret
  } 

  .method public hidebysig virtual final instance int32  CompareTo(class '<>f__AnonymousType3104616430`2'<!'<A>j__TPar',!'<B>j__TPar'> obj) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  5
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_0044

    IL_0003:  ldarg.1
    IL_0004:  brfalse.s  IL_0042

    IL_0006:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
    IL_000b:  ldarg.0
    IL_000c:  ldfld      !0 class '<>f__AnonymousType3104616430`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0011:  ldarg.1
    IL_0012:  ldfld      !0 class '<>f__AnonymousType3104616430`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0017:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericComparisonWithComparer<!'<A>j__TPar'>(class [runtime]System.Collections.IComparer,
                                                                                                                                   !!0,
                                                                                                                                   !!0)
    IL_001c:  stloc.0
    IL_001d:  ldloc.0
    IL_001e:  ldc.i4.0
    IL_001f:  bge.s      IL_0023

    IL_0021:  ldloc.0
    IL_0022:  ret

    IL_0023:  ldloc.0
    IL_0024:  ldc.i4.0
    IL_0025:  ble.s      IL_0029

    IL_0027:  ldloc.0
    IL_0028:  ret

    IL_0029:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
    IL_002e:  ldarg.0
    IL_002f:  ldfld      !1 class '<>f__AnonymousType3104616430`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0034:  ldarg.1
    IL_0035:  ldfld      !1 class '<>f__AnonymousType3104616430`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_003a:  tail.
    IL_003c:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericComparisonWithComparer<!'<B>j__TPar'>(class [runtime]System.Collections.IComparer,
                                                                                                                                   !!0,
                                                                                                                                   !!0)
    IL_0041:  ret

    IL_0042:  ldc.i4.1
    IL_0043:  ret

    IL_0044:  ldarg.1
    IL_0045:  brfalse.s  IL_0049

    IL_0047:  ldc.i4.m1
    IL_0048:  ret

    IL_0049:  ldc.i4.0
    IL_004a:  ret
  } 

  .method public hidebysig virtual final instance int32  CompareTo(object obj) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  unbox.any  class '<>f__AnonymousType3104616430`2'<!'<A>j__TPar',!'<B>j__TPar'>
    IL_0007:  tail.
    IL_0009:  callvirt   instance int32 class '<>f__AnonymousType3104616430`2'<!'<A>j__TPar',!'<B>j__TPar'>::CompareTo(class '<>f__AnonymousType3104616430`2'<!0,!1>)
    IL_000e:  ret
  } 

  .method public hidebysig virtual final instance int32  CompareTo(object obj, class [runtime]System.Collections.IComparer comp) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  5
    .locals init (class '<>f__AnonymousType3104616430`2'<!'<A>j__TPar',!'<B>j__TPar'> V_0,
             class '<>f__AnonymousType3104616430`2'<!'<A>j__TPar',!'<B>j__TPar'> V_1,
             int32 V_2)
    IL_0000:  ldarg.1
    IL_0001:  unbox.any  class '<>f__AnonymousType3104616430`2'<!'<A>j__TPar',!'<B>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  stloc.1
    IL_0009:  ldarg.0
    IL_000a:  brfalse.s  IL_004a

    IL_000c:  ldarg.1
    IL_000d:  unbox.any  class '<>f__AnonymousType3104616430`2'<!'<A>j__TPar',!'<B>j__TPar'>
    IL_0012:  brfalse.s  IL_0048

    IL_0014:  ldarg.2
    IL_0015:  ldarg.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType3104616430`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_001b:  ldloc.1
    IL_001c:  ldfld      !0 class '<>f__AnonymousType3104616430`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0021:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericComparisonWithComparer<!'<A>j__TPar'>(class [runtime]System.Collections.IComparer,
                                                                                                                                   !!0,
                                                                                                                                   !!0)
    IL_0026:  stloc.2
    IL_0027:  ldloc.2
    IL_0028:  ldc.i4.0
    IL_0029:  bge.s      IL_002d

    IL_002b:  ldloc.2
    IL_002c:  ret

    IL_002d:  ldloc.2
    IL_002e:  ldc.i4.0
    IL_002f:  ble.s      IL_0033

    IL_0031:  ldloc.2
    IL_0032:  ret

    IL_0033:  ldarg.2
    IL_0034:  ldarg.0
    IL_0035:  ldfld      !1 class '<>f__AnonymousType3104616430`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_003a:  ldloc.1
    IL_003b:  ldfld      !1 class '<>f__AnonymousType3104616430`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0040:  tail.
    IL_0042:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericComparisonWithComparer<!'<B>j__TPar'>(class [runtime]System.Collections.IComparer,
                                                                                                                                   !!0,
                                                                                                                                   !!0)
    IL_0047:  ret

    IL_0048:  ldc.i4.1
    IL_0049:  ret

    IL_004a:  ldarg.1
    IL_004b:  unbox.any  class '<>f__AnonymousType3104616430`2'<!'<A>j__TPar',!'<B>j__TPar'>
    IL_0050:  brfalse.s  IL_0054

    IL_0052:  ldc.i4.m1
    IL_0053:  ret

    IL_0054:  ldc.i4.0
    IL_0055:  ret
  } 

  .method public hidebysig virtual final instance int32  GetHashCode(class [runtime]System.Collections.IEqualityComparer comp) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  7
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_003d

    IL_0003:  ldc.i4.0
    IL_0004:  stloc.0
    IL_0005:  ldc.i4     0x9e3779b9
    IL_000a:  ldarg.1
    IL_000b:  ldarg.0
    IL_000c:  ldfld      !1 class '<>f__AnonymousType3104616430`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0011:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericHashWithComparer<!'<B>j__TPar'>(class [runtime]System.Collections.IEqualityComparer,
                                                                                                                             !!0)
    IL_0016:  ldloc.0
    IL_0017:  ldc.i4.6
    IL_0018:  shl
    IL_0019:  ldloc.0
    IL_001a:  ldc.i4.2
    IL_001b:  shr
    IL_001c:  add
    IL_001d:  add
    IL_001e:  add
    IL_001f:  stloc.0
    IL_0020:  ldc.i4     0x9e3779b9
    IL_0025:  ldarg.1
    IL_0026:  ldarg.0
    IL_0027:  ldfld      !0 class '<>f__AnonymousType3104616430`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_002c:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericHashWithComparer<!'<A>j__TPar'>(class [runtime]System.Collections.IEqualityComparer,
                                                                                                                             !!0)
    IL_0031:  ldloc.0
    IL_0032:  ldc.i4.6
    IL_0033:  shl
    IL_0034:  ldloc.0
    IL_0035:  ldc.i4.2
    IL_0036:  shr
    IL_0037:  add
    IL_0038:  add
    IL_0039:  add
    IL_003a:  stloc.0
    IL_003b:  ldloc.0
    IL_003c:  ret

    IL_003d:  ldc.i4.0
    IL_003e:  ret
  } 

  .method public hidebysig virtual final instance int32  GetHashCode() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
    IL_0006:  tail.
    IL_0008:  callvirt   instance int32 class '<>f__AnonymousType3104616430`2'<!'<A>j__TPar',!'<B>j__TPar'>::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
    IL_000d:  ret
  } 

  .method public hidebysig instance bool Equals(class '<>f__AnonymousType3104616430`2'<!'<A>j__TPar',!'<B>j__TPar'> obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  5
    .locals init (class '<>f__AnonymousType3104616430`2'<!'<A>j__TPar',!'<B>j__TPar'> V_0)
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_0035

    IL_0003:  ldarg.1
    IL_0004:  brfalse.s  IL_0033

    IL_0006:  ldarg.1
    IL_0007:  stloc.0
    IL_0008:  ldarg.2
    IL_0009:  ldarg.0
    IL_000a:  ldfld      !0 class '<>f__AnonymousType3104616430`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_000f:  ldloc.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType3104616430`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0015:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericEqualityWithComparer<!'<A>j__TPar'>(class [runtime]System.Collections.IEqualityComparer,
                                                                                                                                !!0,
                                                                                                                                !!0)
    IL_001a:  brfalse.s  IL_0031

    IL_001c:  ldarg.2
    IL_001d:  ldarg.0
    IL_001e:  ldfld      !1 class '<>f__AnonymousType3104616430`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0023:  ldloc.0
    IL_0024:  ldfld      !1 class '<>f__AnonymousType3104616430`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0029:  tail.
    IL_002b:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericEqualityWithComparer<!'<B>j__TPar'>(class [runtime]System.Collections.IEqualityComparer,
                                                                                                                                !!0,
                                                                                                                                !!0)
    IL_0030:  ret

    IL_0031:  ldc.i4.0
    IL_0032:  ret

    IL_0033:  ldc.i4.0
    IL_0034:  ret

    IL_0035:  ldarg.1
    IL_0036:  ldnull
    IL_0037:  cgt.un
    IL_0039:  ldc.i4.0
    IL_003a:  ceq
    IL_003c:  ret
  } 

  .method public hidebysig virtual final instance bool  Equals(object obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  5
    .locals init (class '<>f__AnonymousType3104616430`2'<!'<A>j__TPar',!'<B>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType3104616430`2'<!'<A>j__TPar',!'<B>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0015

    IL_000a:  ldarg.0
    IL_000b:  ldloc.0
    IL_000c:  ldarg.2
    IL_000d:  tail.
    IL_000f:  callvirt   instance bool class '<>f__AnonymousType3104616430`2'<!'<A>j__TPar',!'<B>j__TPar'>::Equals(class '<>f__AnonymousType3104616430`2'<!0,!1>,
                                                                                                                   class [runtime]System.Collections.IEqualityComparer)
    IL_0014:  ret

    IL_0015:  ldc.i4.0
    IL_0016:  ret
  } 

  .method public hidebysig virtual final instance bool  Equals(class '<>f__AnonymousType3104616430`2'<!'<A>j__TPar',!'<B>j__TPar'> obj) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_0031

    IL_0003:  ldarg.1
    IL_0004:  brfalse.s  IL_002f

    IL_0006:  ldarg.0
    IL_0007:  ldfld      !0 class '<>f__AnonymousType3104616430`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_000c:  ldarg.1
    IL_000d:  ldfld      !0 class '<>f__AnonymousType3104616430`2'<!'<A>j__TPar',!'<B>j__TPar'>::A@
    IL_0012:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericEqualityER<!'<A>j__TPar'>(!!0,
                                                                                                                      !!0)
    IL_0017:  brfalse.s  IL_002d

    IL_0019:  ldarg.0
    IL_001a:  ldfld      !1 class '<>f__AnonymousType3104616430`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_001f:  ldarg.1
    IL_0020:  ldfld      !1 class '<>f__AnonymousType3104616430`2'<!'<A>j__TPar',!'<B>j__TPar'>::B@
    IL_0025:  tail.
    IL_0027:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericEqualityER<!'<B>j__TPar'>(!!0,
                                                                                                                      !!0)
    IL_002c:  ret

    IL_002d:  ldc.i4.0
    IL_002e:  ret

    IL_002f:  ldc.i4.0
    IL_0030:  ret

    IL_0031:  ldarg.1
    IL_0032:  ldnull
    IL_0033:  cgt.un
    IL_0035:  ldc.i4.0
    IL_0036:  ceq
    IL_0038:  ret
  } 

  .method public hidebysig virtual final instance bool  Equals(object obj) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  4
    .locals init (class '<>f__AnonymousType3104616430`2'<!'<A>j__TPar',!'<B>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType3104616430`2'<!'<A>j__TPar',!'<B>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0014

    IL_000a:  ldarg.0
    IL_000b:  ldloc.0
    IL_000c:  tail.
    IL_000e:  callvirt   instance bool class '<>f__AnonymousType3104616430`2'<!'<A>j__TPar',!'<B>j__TPar'>::Equals(class '<>f__AnonymousType3104616430`2'<!0,!1>)
    IL_0013:  ret

    IL_0014:  ldc.i4.0
    IL_0015:  ret
  } 

  .property instance !'<A>j__TPar' A()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
    .get instance !'<A>j__TPar' '<>f__AnonymousType3104616430`2'::get_A()
  } 
  .property instance !'<B>j__TPar' B()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                int32) = ( 01 00 04 00 00 00 01 00 00 00 00 00 ) 
    .get instance !'<B>j__TPar' '<>f__AnonymousType3104616430`2'::get_B()
  } 
} 

.class public auto ansi serializable sealed beforefieldinit '<>f__AnonymousType3986374330`2'<'<Nested>j__TPar','<Other>j__TPar'>
       extends [runtime]System.Object
       implements [runtime]System.Collections.IStructuralComparable,
                  [runtime]System.IComparable,
                  class [runtime]System.IComparable`1<class '<>f__AnonymousType3986374330`2'<!'<Nested>j__TPar',!'<Other>j__TPar'>>,
                  [runtime]System.Collections.IStructuralEquatable,
                  class [runtime]System.IEquatable`1<class '<>f__AnonymousType3986374330`2'<!'<Nested>j__TPar',!'<Other>j__TPar'>>
{
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 02 00 00 00 00 00 ) 
  .field private !'<Nested>j__TPar' Nested@
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private !'<Other>j__TPar' Other@
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public specialname rtspecialname instance void  .ctor(!'<Nested>j__TPar' Nested, !'<Other>j__TPar' Other) cil managed
  {
    .custom instance void [runtime]System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype [runtime]System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                                            class [runtime]System.Type) = ( 01 00 60 06 00 00 1E 3C 3E 66 5F 5F 41 6E 6F 6E   
                                                                                                                                                   79 6D 6F 75 73 54 79 70 65 33 39 38 36 33 37 34   
                                                                                                                                                   33 33 30 60 32 00 00 )                            
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [runtime]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousType3986374330`2'<!'<Nested>j__TPar',!'<Other>j__TPar'>::Nested@
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      !1 class '<>f__AnonymousType3986374330`2'<!'<Nested>j__TPar',!'<Other>j__TPar'>::Other@
    IL_0014:  ret
  } 

  .method public hidebysig specialname instance !'<Nested>j__TPar' get_Nested() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType3986374330`2'<!'<Nested>j__TPar',!'<Other>j__TPar'>::Nested@
    IL_0006:  ret
  } 

  .method public hidebysig specialname instance !'<Other>j__TPar' get_Other() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType3986374330`2'<!'<Nested>j__TPar',!'<Other>j__TPar'>::Other@
    IL_0006:  ret
  } 

  .method public strict virtual instance string ToString() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldstr      "%+A"
    IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class '<>f__AnonymousType3986374330`2'<!'<Nested>j__TPar',!'<Other>j__TPar'>,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class '<>f__AnonymousType3986374330`2'<!'<Nested>j__TPar',!'<Other>j__TPar'>>::.ctor(string)
    IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class '<>f__AnonymousType3986374330`2'<!'<Nested>j__TPar',!'<Other>j__TPar'>,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
    IL_000f:  ldarg.0
    IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class '<>f__AnonymousType3986374330`2'<!'<Nested>j__TPar',!'<Other>j__TPar'>,string>::Invoke(!0)
    IL_0015:  ret
  } 

  .method public hidebysig virtual final instance int32  CompareTo(class '<>f__AnonymousType3986374330`2'<!'<Nested>j__TPar',!'<Other>j__TPar'> obj) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  5
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_0044

    IL_0003:  ldarg.1
    IL_0004:  brfalse.s  IL_0042

    IL_0006:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
    IL_000b:  ldarg.0
    IL_000c:  ldfld      !0 class '<>f__AnonymousType3986374330`2'<!'<Nested>j__TPar',!'<Other>j__TPar'>::Nested@
    IL_0011:  ldarg.1
    IL_0012:  ldfld      !0 class '<>f__AnonymousType3986374330`2'<!'<Nested>j__TPar',!'<Other>j__TPar'>::Nested@
    IL_0017:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericComparisonWithComparer<!'<Nested>j__TPar'>(class [runtime]System.Collections.IComparer,
                                                                                                                                        !!0,
                                                                                                                                        !!0)
    IL_001c:  stloc.0
    IL_001d:  ldloc.0
    IL_001e:  ldc.i4.0
    IL_001f:  bge.s      IL_0023

    IL_0021:  ldloc.0
    IL_0022:  ret

    IL_0023:  ldloc.0
    IL_0024:  ldc.i4.0
    IL_0025:  ble.s      IL_0029

    IL_0027:  ldloc.0
    IL_0028:  ret

    IL_0029:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
    IL_002e:  ldarg.0
    IL_002f:  ldfld      !1 class '<>f__AnonymousType3986374330`2'<!'<Nested>j__TPar',!'<Other>j__TPar'>::Other@
    IL_0034:  ldarg.1
    IL_0035:  ldfld      !1 class '<>f__AnonymousType3986374330`2'<!'<Nested>j__TPar',!'<Other>j__TPar'>::Other@
    IL_003a:  tail.
    IL_003c:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericComparisonWithComparer<!'<Other>j__TPar'>(class [runtime]System.Collections.IComparer,
                                                                                                                                       !!0,
                                                                                                                                       !!0)
    IL_0041:  ret

    IL_0042:  ldc.i4.1
    IL_0043:  ret

    IL_0044:  ldarg.1
    IL_0045:  brfalse.s  IL_0049

    IL_0047:  ldc.i4.m1
    IL_0048:  ret

    IL_0049:  ldc.i4.0
    IL_004a:  ret
  } 

  .method public hidebysig virtual final instance int32  CompareTo(object obj) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  unbox.any  class '<>f__AnonymousType3986374330`2'<!'<Nested>j__TPar',!'<Other>j__TPar'>
    IL_0007:  tail.
    IL_0009:  callvirt   instance int32 class '<>f__AnonymousType3986374330`2'<!'<Nested>j__TPar',!'<Other>j__TPar'>::CompareTo(class '<>f__AnonymousType3986374330`2'<!0,!1>)
    IL_000e:  ret
  } 

  .method public hidebysig virtual final instance int32  CompareTo(object obj, class [runtime]System.Collections.IComparer comp) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  5
    .locals init (class '<>f__AnonymousType3986374330`2'<!'<Nested>j__TPar',!'<Other>j__TPar'> V_0,
             class '<>f__AnonymousType3986374330`2'<!'<Nested>j__TPar',!'<Other>j__TPar'> V_1,
             int32 V_2)
    IL_0000:  ldarg.1
    IL_0001:  unbox.any  class '<>f__AnonymousType3986374330`2'<!'<Nested>j__TPar',!'<Other>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  stloc.1
    IL_0009:  ldarg.0
    IL_000a:  brfalse.s  IL_004a

    IL_000c:  ldarg.1
    IL_000d:  unbox.any  class '<>f__AnonymousType3986374330`2'<!'<Nested>j__TPar',!'<Other>j__TPar'>
    IL_0012:  brfalse.s  IL_0048

    IL_0014:  ldarg.2
    IL_0015:  ldarg.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType3986374330`2'<!'<Nested>j__TPar',!'<Other>j__TPar'>::Nested@
    IL_001b:  ldloc.1
    IL_001c:  ldfld      !0 class '<>f__AnonymousType3986374330`2'<!'<Nested>j__TPar',!'<Other>j__TPar'>::Nested@
    IL_0021:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericComparisonWithComparer<!'<Nested>j__TPar'>(class [runtime]System.Collections.IComparer,
                                                                                                                                        !!0,
                                                                                                                                        !!0)
    IL_0026:  stloc.2
    IL_0027:  ldloc.2
    IL_0028:  ldc.i4.0
    IL_0029:  bge.s      IL_002d

    IL_002b:  ldloc.2
    IL_002c:  ret

    IL_002d:  ldloc.2
    IL_002e:  ldc.i4.0
    IL_002f:  ble.s      IL_0033

    IL_0031:  ldloc.2
    IL_0032:  ret

    IL_0033:  ldarg.2
    IL_0034:  ldarg.0
    IL_0035:  ldfld      !1 class '<>f__AnonymousType3986374330`2'<!'<Nested>j__TPar',!'<Other>j__TPar'>::Other@
    IL_003a:  ldloc.1
    IL_003b:  ldfld      !1 class '<>f__AnonymousType3986374330`2'<!'<Nested>j__TPar',!'<Other>j__TPar'>::Other@
    IL_0040:  tail.
    IL_0042:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericComparisonWithComparer<!'<Other>j__TPar'>(class [runtime]System.Collections.IComparer,
                                                                                                                                       !!0,
                                                                                                                                       !!0)
    IL_0047:  ret

    IL_0048:  ldc.i4.1
    IL_0049:  ret

    IL_004a:  ldarg.1
    IL_004b:  unbox.any  class '<>f__AnonymousType3986374330`2'<!'<Nested>j__TPar',!'<Other>j__TPar'>
    IL_0050:  brfalse.s  IL_0054

    IL_0052:  ldc.i4.m1
    IL_0053:  ret

    IL_0054:  ldc.i4.0
    IL_0055:  ret
  } 

  .method public hidebysig virtual final instance int32  GetHashCode(class [runtime]System.Collections.IEqualityComparer comp) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  7
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_003d

    IL_0003:  ldc.i4.0
    IL_0004:  stloc.0
    IL_0005:  ldc.i4     0x9e3779b9
    IL_000a:  ldarg.1
    IL_000b:  ldarg.0
    IL_000c:  ldfld      !1 class '<>f__AnonymousType3986374330`2'<!'<Nested>j__TPar',!'<Other>j__TPar'>::Other@
    IL_0011:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericHashWithComparer<!'<Other>j__TPar'>(class [runtime]System.Collections.IEqualityComparer,
                                                                                                                                 !!0)
    IL_0016:  ldloc.0
    IL_0017:  ldc.i4.6
    IL_0018:  shl
    IL_0019:  ldloc.0
    IL_001a:  ldc.i4.2
    IL_001b:  shr
    IL_001c:  add
    IL_001d:  add
    IL_001e:  add
    IL_001f:  stloc.0
    IL_0020:  ldc.i4     0x9e3779b9
    IL_0025:  ldarg.1
    IL_0026:  ldarg.0
    IL_0027:  ldfld      !0 class '<>f__AnonymousType3986374330`2'<!'<Nested>j__TPar',!'<Other>j__TPar'>::Nested@
    IL_002c:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericHashWithComparer<!'<Nested>j__TPar'>(class [runtime]System.Collections.IEqualityComparer,
                                                                                                                                  !!0)
    IL_0031:  ldloc.0
    IL_0032:  ldc.i4.6
    IL_0033:  shl
    IL_0034:  ldloc.0
    IL_0035:  ldc.i4.2
    IL_0036:  shr
    IL_0037:  add
    IL_0038:  add
    IL_0039:  add
    IL_003a:  stloc.0
    IL_003b:  ldloc.0
    IL_003c:  ret

    IL_003d:  ldc.i4.0
    IL_003e:  ret
  } 

  .method public hidebysig virtual final instance int32  GetHashCode() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
    IL_0006:  tail.
    IL_0008:  callvirt   instance int32 class '<>f__AnonymousType3986374330`2'<!'<Nested>j__TPar',!'<Other>j__TPar'>::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
    IL_000d:  ret
  } 

  .method public hidebysig instance bool Equals(class '<>f__AnonymousType3986374330`2'<!'<Nested>j__TPar',!'<Other>j__TPar'> obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  5
    .locals init (class '<>f__AnonymousType3986374330`2'<!'<Nested>j__TPar',!'<Other>j__TPar'> V_0)
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_0035

    IL_0003:  ldarg.1
    IL_0004:  brfalse.s  IL_0033

    IL_0006:  ldarg.1
    IL_0007:  stloc.0
    IL_0008:  ldarg.2
    IL_0009:  ldarg.0
    IL_000a:  ldfld      !0 class '<>f__AnonymousType3986374330`2'<!'<Nested>j__TPar',!'<Other>j__TPar'>::Nested@
    IL_000f:  ldloc.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType3986374330`2'<!'<Nested>j__TPar',!'<Other>j__TPar'>::Nested@
    IL_0015:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericEqualityWithComparer<!'<Nested>j__TPar'>(class [runtime]System.Collections.IEqualityComparer,
                                                                                                                                     !!0,
                                                                                                                                     !!0)
    IL_001a:  brfalse.s  IL_0031

    IL_001c:  ldarg.2
    IL_001d:  ldarg.0
    IL_001e:  ldfld      !1 class '<>f__AnonymousType3986374330`2'<!'<Nested>j__TPar',!'<Other>j__TPar'>::Other@
    IL_0023:  ldloc.0
    IL_0024:  ldfld      !1 class '<>f__AnonymousType3986374330`2'<!'<Nested>j__TPar',!'<Other>j__TPar'>::Other@
    IL_0029:  tail.
    IL_002b:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericEqualityWithComparer<!'<Other>j__TPar'>(class [runtime]System.Collections.IEqualityComparer,
                                                                                                                                    !!0,
                                                                                                                                    !!0)
    IL_0030:  ret

    IL_0031:  ldc.i4.0
    IL_0032:  ret

    IL_0033:  ldc.i4.0
    IL_0034:  ret

    IL_0035:  ldarg.1
    IL_0036:  ldnull
    IL_0037:  cgt.un
    IL_0039:  ldc.i4.0
    IL_003a:  ceq
    IL_003c:  ret
  } 

  .method public hidebysig virtual final instance bool  Equals(object obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  5
    .locals init (class '<>f__AnonymousType3986374330`2'<!'<Nested>j__TPar',!'<Other>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType3986374330`2'<!'<Nested>j__TPar',!'<Other>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0015

    IL_000a:  ldarg.0
    IL_000b:  ldloc.0
    IL_000c:  ldarg.2
    IL_000d:  tail.
    IL_000f:  callvirt   instance bool class '<>f__AnonymousType3986374330`2'<!'<Nested>j__TPar',!'<Other>j__TPar'>::Equals(class '<>f__AnonymousType3986374330`2'<!0,!1>,
                                                                                                                            class [runtime]System.Collections.IEqualityComparer)
    IL_0014:  ret

    IL_0015:  ldc.i4.0
    IL_0016:  ret
  } 

  .method public hidebysig virtual final instance bool  Equals(class '<>f__AnonymousType3986374330`2'<!'<Nested>j__TPar',!'<Other>j__TPar'> obj) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_0031

    IL_0003:  ldarg.1
    IL_0004:  brfalse.s  IL_002f

    IL_0006:  ldarg.0
    IL_0007:  ldfld      !0 class '<>f__AnonymousType3986374330`2'<!'<Nested>j__TPar',!'<Other>j__TPar'>::Nested@
    IL_000c:  ldarg.1
    IL_000d:  ldfld      !0 class '<>f__AnonymousType3986374330`2'<!'<Nested>j__TPar',!'<Other>j__TPar'>::Nested@
    IL_0012:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericEqualityER<!'<Nested>j__TPar'>(!!0,
                                                                                                                           !!0)
    IL_0017:  brfalse.s  IL_002d

    IL_0019:  ldarg.0
    IL_001a:  ldfld      !1 class '<>f__AnonymousType3986374330`2'<!'<Nested>j__TPar',!'<Other>j__TPar'>::Other@
    IL_001f:  ldarg.1
    IL_0020:  ldfld      !1 class '<>f__AnonymousType3986374330`2'<!'<Nested>j__TPar',!'<Other>j__TPar'>::Other@
    IL_0025:  tail.
    IL_0027:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::GenericEqualityER<!'<Other>j__TPar'>(!!0,
                                                                                                                          !!0)
    IL_002c:  ret

    IL_002d:  ldc.i4.0
    IL_002e:  ret

    IL_002f:  ldc.i4.0
    IL_0030:  ret

    IL_0031:  ldarg.1
    IL_0032:  ldnull
    IL_0033:  cgt.un
    IL_0035:  ldc.i4.0
    IL_0036:  ceq
    IL_0038:  ret
  } 

  .method public hidebysig virtual final instance bool  Equals(object obj) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  4
    .locals init (class '<>f__AnonymousType3986374330`2'<!'<Nested>j__TPar',!'<Other>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType3986374330`2'<!'<Nested>j__TPar',!'<Other>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0014

    IL_000a:  ldarg.0
    IL_000b:  ldloc.0
    IL_000c:  tail.
    IL_000e:  callvirt   instance bool class '<>f__AnonymousType3986374330`2'<!'<Nested>j__TPar',!'<Other>j__TPar'>::Equals(class '<>f__AnonymousType3986374330`2'<!0,!1>)
    IL_0013:  ret

    IL_0014:  ldc.i4.0
    IL_0015:  ret
  } 

  .property instance !'<Nested>j__TPar' Nested()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 ) 
    .get instance !'<Nested>j__TPar' '<>f__AnonymousType3986374330`2'::get_Nested()
  } 
  .property instance !'<Other>j__TPar' Other()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                int32) = ( 01 00 04 00 00 00 01 00 00 00 00 00 ) 
    .get instance !'<Other>j__TPar' '<>f__AnonymousType3986374330`2'::get_Other()
  } 
} 






