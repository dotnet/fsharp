




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
.mresource public FSharpSignatureData.assembly
{
  
  
}
.mresource public FSharpOptimizationData.assembly
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
  .class auto autochar serializable sealed nested public beforefieldinit Discr
         extends [runtime]System.Object
         implements class [runtime]System.IEquatable`1<class assembly/Discr>,
                    [runtime]System.Collections.IStructuralEquatable,
                    class [runtime]System.IComparable`1<class assembly/Discr>,
                    [runtime]System.IComparable,
                    [runtime]System.Collections.IStructuralComparable
  {
    .custom instance void [runtime]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 7B 5F 5F 44 65 62 75 67 44 69 73 70 6C   
                                                                                                   61 79 28 29 2C 6E 71 7D 00 00 )                   
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 01 00 00 00 00 00 ) 
    .class abstract auto ansi sealed nested public Tags
           extends [runtime]System.Object
    {
      .field public static literal int32 CaseA = int32(0x00000000)
      .field public static literal int32 CaseB = int32(0x00000001)
    } 

    .field assembly initonly int32 _tag
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field static assembly initonly class assembly/Discr _unique_CaseA
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field static assembly initonly class assembly/Discr _unique_CaseB
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  newobj     instance void assembly/Discr::.ctor(int32)
      IL_0006:  stsfld     class assembly/Discr assembly/Discr::_unique_CaseA
      IL_000b:  ldc.i4.1
      IL_000c:  newobj     instance void assembly/Discr::.ctor(int32)
      IL_0011:  stsfld     class assembly/Discr assembly/Discr::_unique_CaseB
      IL_0016:  ret
    } 

    .method assembly specialname rtspecialname 
            instance void  .ctor(int32 _tag) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32 assembly/Discr::_tag
      IL_000d:  ret
    } 

    .method public static class assembly/Discr 
            get_CaseA() cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 08 00 00 00 00 00 00 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldsfld     class assembly/Discr assembly/Discr::_unique_CaseA
      IL_0005:  ret
    } 

    .method public hidebysig instance bool 
            get_IsCaseA() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance int32 assembly/Discr::get_Tag()
      IL_0006:  ldc.i4.0
      IL_0007:  ceq
      IL_0009:  ret
    } 

    .method public static class assembly/Discr 
            get_CaseB() cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 08 00 00 00 01 00 00 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldsfld     class assembly/Discr assembly/Discr::_unique_CaseB
      IL_0005:  ret
    } 

    .method public hidebysig instance bool 
            get_IsCaseB() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance int32 assembly/Discr::get_Tag()
      IL_0006:  ldc.i4.1
      IL_0007:  ceq
      IL_0009:  ret
    } 

    .method public hidebysig instance int32 
            get_Tag() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/Discr::_tag
      IL_0006:  ret
    } 

    .method assembly hidebysig specialname 
            instance object  __DebugDisplay() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldstr      "%+0.8A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/Discr,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,string>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/Discr,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/Discr,string>::Invoke(!0)
      IL_0015:  ret
    } 

    .method public strict virtual instance string 
            ToString() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldstr      "%+A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/Discr,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class assembly/Discr>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/Discr,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/Discr,string>::Invoke(!0)
      IL_0015:  ret
    } 

    .method public hidebysig virtual final 
            instance int32  CompareTo(class assembly/Discr obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (int32 V_0,
               int32 V_1)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0020

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_001e

      IL_0006:  ldarg.0
      IL_0007:  ldfld      int32 assembly/Discr::_tag
      IL_000c:  stloc.0
      IL_000d:  ldarg.1
      IL_000e:  ldfld      int32 assembly/Discr::_tag
      IL_0013:  stloc.1
      IL_0014:  ldloc.0
      IL_0015:  ldloc.1
      IL_0016:  bne.un.s   IL_001a

      IL_0018:  ldc.i4.0
      IL_0019:  ret

      IL_001a:  ldloc.0
      IL_001b:  ldloc.1
      IL_001c:  sub
      IL_001d:  ret

      IL_001e:  ldc.i4.1
      IL_001f:  ret

      IL_0020:  ldarg.1
      IL_0021:  brfalse.s  IL_0025

      IL_0023:  ldc.i4.m1
      IL_0024:  ret

      IL_0025:  ldc.i4.0
      IL_0026:  ret
    } 

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  unbox.any  assembly/Discr
      IL_0007:  callvirt   instance int32 assembly/Discr::CompareTo(class assembly/Discr)
      IL_000c:  ret
    } 

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj,
                                      class [runtime]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (class assembly/Discr V_0,
               int32 V_1,
               int32 V_2)
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  assembly/Discr
      IL_0006:  stloc.0
      IL_0007:  ldarg.0
      IL_0008:  brfalse.s  IL_002c

      IL_000a:  ldarg.1
      IL_000b:  unbox.any  assembly/Discr
      IL_0010:  brfalse.s  IL_002a

      IL_0012:  ldarg.0
      IL_0013:  ldfld      int32 assembly/Discr::_tag
      IL_0018:  stloc.1
      IL_0019:  ldloc.0
      IL_001a:  ldfld      int32 assembly/Discr::_tag
      IL_001f:  stloc.2
      IL_0020:  ldloc.1
      IL_0021:  ldloc.2
      IL_0022:  bne.un.s   IL_0026

      IL_0024:  ldc.i4.0
      IL_0025:  ret

      IL_0026:  ldloc.1
      IL_0027:  ldloc.2
      IL_0028:  sub
      IL_0029:  ret

      IL_002a:  ldc.i4.1
      IL_002b:  ret

      IL_002c:  ldarg.1
      IL_002d:  unbox.any  assembly/Discr
      IL_0032:  brfalse.s  IL_0036

      IL_0034:  ldc.i4.m1
      IL_0035:  ret

      IL_0036:  ldc.i4.0
      IL_0037:  ret
    } 

    .method public hidebysig virtual final 
            instance int32  GetHashCode(class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  3
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_000c

      IL_0003:  ldc.i4.0
      IL_0004:  stloc.0
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 assembly/Discr::_tag
      IL_000b:  ret

      IL_000c:  ldc.i4.0
      IL_000d:  ret
    } 

    .method public hidebysig virtual final 
            instance int32  GetHashCode() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0006:  callvirt   instance int32 assembly/Discr::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
      IL_000b:  ret
    } 

    .method public hidebysig virtual final 
            instance bool  Equals(object obj,
                                  class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (class assembly/Discr V_0,
               class assembly/Discr V_1,
               int32 V_2,
               int32 V_3)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0024

      IL_0003:  ldarg.1
      IL_0004:  isinst     assembly/Discr
      IL_0009:  stloc.0
      IL_000a:  ldloc.0
      IL_000b:  brfalse.s  IL_0022

      IL_000d:  ldloc.0
      IL_000e:  stloc.1
      IL_000f:  ldarg.0
      IL_0010:  ldfld      int32 assembly/Discr::_tag
      IL_0015:  stloc.2
      IL_0016:  ldloc.1
      IL_0017:  ldfld      int32 assembly/Discr::_tag
      IL_001c:  stloc.3
      IL_001d:  ldloc.2
      IL_001e:  ldloc.3
      IL_001f:  ceq
      IL_0021:  ret

      IL_0022:  ldc.i4.0
      IL_0023:  ret

      IL_0024:  ldarg.1
      IL_0025:  ldnull
      IL_0026:  cgt.un
      IL_0028:  ldc.i4.0
      IL_0029:  ceq
      IL_002b:  ret
    } 

    .method public hidebysig virtual final 
            instance bool  Equals(class assembly/Discr obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (int32 V_0,
               int32 V_1)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_001b

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_0019

      IL_0006:  ldarg.0
      IL_0007:  ldfld      int32 assembly/Discr::_tag
      IL_000c:  stloc.0
      IL_000d:  ldarg.1
      IL_000e:  ldfld      int32 assembly/Discr::_tag
      IL_0013:  stloc.1
      IL_0014:  ldloc.0
      IL_0015:  ldloc.1
      IL_0016:  ceq
      IL_0018:  ret

      IL_0019:  ldc.i4.0
      IL_001a:  ret

      IL_001b:  ldarg.1
      IL_001c:  ldnull
      IL_001d:  cgt.un
      IL_001f:  ldc.i4.0
      IL_0020:  ceq
      IL_0022:  ret
    } 

    .method public hidebysig virtual final 
            instance bool  Equals(object obj) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (class assembly/Discr V_0)
      IL_0000:  ldarg.1
      IL_0001:  isinst     assembly/Discr
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0012

      IL_000a:  ldarg.0
      IL_000b:  ldloc.0
      IL_000c:  callvirt   instance bool assembly/Discr::Equals(class assembly/Discr)
      IL_0011:  ret

      IL_0012:  ldc.i4.0
      IL_0013:  ret
    } 

    .property instance int32 Tag()
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance int32 assembly/Discr::get_Tag()
    } 
    .property class assembly/Discr
            CaseA()
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get class assembly/Discr assembly/Discr::get_CaseA()
    } 
    .property instance bool IsCaseA()
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance bool assembly/Discr::get_IsCaseA()
    } 
    .property class assembly/Discr
            CaseB()
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get class assembly/Discr assembly/Discr::get_CaseB()
    } 
    .property instance bool IsCaseB()
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance bool assembly/Discr::get_IsCaseB()
    } 
  } 

  .method public static void  funcD(class assembly/Discr n) cil managed
  {
    
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  call       instance int32 assembly/Discr::get_Tag()
    IL_0007:  ldc.i4.0
    IL_0008:  bne.un.s   IL_000c

    IL_000a:  br.s       IL_0017

    IL_000c:  ldstr      "B"
    IL_0011:  call       void [runtime]System.Console::WriteLine(string)
    IL_0016:  ret

    IL_0017:  ldstr      "A"
    IL_001c:  call       void [runtime]System.Console::WriteLine(string)
    IL_0021:  ret
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






