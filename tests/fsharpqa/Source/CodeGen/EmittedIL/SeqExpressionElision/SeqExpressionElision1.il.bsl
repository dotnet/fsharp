
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.6.1055.0
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly extern FSharp.Core
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 4:4:3:0
}
.assembly SeqExpressionElision1
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.SeqExpressionElision1
{
  // Offset: 0x00000000 Length: 0x00000763
}
.mresource public FSharpOptimizationData.SeqExpressionElision1
{
  // Offset: 0x00000768 Length: 0x000004A4
}
.module SeqExpressionElision1.exe
// MVID: {5B16780F-39C3-BE99-A745-03830F78165B}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00F20000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed SeqExpressionElision1
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto autochar serializable sealed nested public beforefieldinit Foo
         extends [mscorlib]System.Object
         implements class [mscorlib]System.IEquatable`1<class SeqExpressionElision1/Foo>,
                    [mscorlib]System.Collections.IStructuralEquatable,
                    class [mscorlib]System.IComparable`1<class SeqExpressionElision1/Foo>,
                    [mscorlib]System.IComparable,
                    [mscorlib]System.Collections.IStructuralComparable
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 7B 5F 5F 44 65 62 75 67 44 69 73 70 6C   // ...{__DebugDispl
                                                                                                   61 79 28 29 2C 6E 71 7D 00 00 )                   // ay(),nq}..
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 01 00 00 00 00 00 ) 
    .class abstract auto ansi sealed nested public Tags
           extends [mscorlib]System.Object
    {
      .field public static literal int32 Foo = int32(0x00000000)
      .field public static literal int32 Bar = int32(0x00000001)
    } // end of class Tags

    .field assembly initonly int32 _tag
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field static assembly initonly class SeqExpressionElision1/Foo _unique_Foo
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field static assembly initonly class SeqExpressionElision1/Foo _unique_Bar
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       23 (0x17)
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  newobj     instance void SeqExpressionElision1/Foo::.ctor(int32)
      IL_0006:  stsfld     class SeqExpressionElision1/Foo SeqExpressionElision1/Foo::_unique_Foo
      IL_000b:  ldc.i4.1
      IL_000c:  newobj     instance void SeqExpressionElision1/Foo::.ctor(int32)
      IL_0011:  stsfld     class SeqExpressionElision1/Foo SeqExpressionElision1/Foo::_unique_Bar
      IL_0016:  ret
    } // end of method Foo::.cctor

    .method assembly specialname rtspecialname 
            instance void  .ctor(int32 _tag) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32 SeqExpressionElision1/Foo::_tag
      IL_000d:  ret
    } // end of method Foo::.ctor

    .method public static class SeqExpressionElision1/Foo 
            get_Foo() cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 08 00 00 00 00 00 00 00 00 00 ) 
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  ldsfld     class SeqExpressionElision1/Foo SeqExpressionElision1/Foo::_unique_Foo
      IL_0005:  ret
    } // end of method Foo::get_Foo

    .method public hidebysig instance bool 
            get_IsFoo() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       10 (0xa)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance int32 SeqExpressionElision1/Foo::get_Tag()
      IL_0006:  ldc.i4.0
      IL_0007:  ceq
      IL_0009:  ret
    } // end of method Foo::get_IsFoo

    .method public static class SeqExpressionElision1/Foo 
            get_Bar() cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 08 00 00 00 01 00 00 00 00 00 ) 
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  ldsfld     class SeqExpressionElision1/Foo SeqExpressionElision1/Foo::_unique_Bar
      IL_0005:  ret
    } // end of method Foo::get_Bar

    .method public hidebysig instance bool 
            get_IsBar() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       10 (0xa)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance int32 SeqExpressionElision1/Foo::get_Tag()
      IL_0006:  ldc.i4.1
      IL_0007:  ceq
      IL_0009:  ret
    } // end of method Foo::get_IsBar

    .method public hidebysig instance int32 
            get_Tag() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 SeqExpressionElision1/Foo::_tag
      IL_0006:  ret
    } // end of method Foo::get_Tag

    .method assembly hidebysig specialname 
            instance object  __DebugDisplay() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       22 (0x16)
      .maxstack  8
      IL_0000:  ldstr      "%+0.8A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class SeqExpressionElision1/Foo,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,string>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class SeqExpressionElision1/Foo,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class SeqExpressionElision1/Foo,string>::Invoke(!0)
      IL_0015:  ret
    } // end of method Foo::__DebugDisplay

    .method public strict virtual instance string 
            ToString() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       22 (0x16)
      .maxstack  8
      IL_0000:  ldstr      "%+A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class SeqExpressionElision1/Foo,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class SeqExpressionElision1/Foo>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class SeqExpressionElision1/Foo,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class SeqExpressionElision1/Foo,string>::Invoke(!0)
      IL_0015:  ret
    } // end of method Foo::ToString

    .method public hidebysig virtual final 
            instance int32  CompareTo(class SeqExpressionElision1/Foo obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       48 (0x30)
      .maxstack  4
      .locals init (int32 V_0,
               int32 V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldnull
      IL_0002:  cgt.un
      IL_0004:  brfalse.s  IL_0026

      IL_0006:  ldarg.1
      IL_0007:  ldnull
      IL_0008:  cgt.un
      IL_000a:  brfalse.s  IL_0024

      IL_000c:  ldarg.0
      IL_000d:  ldfld      int32 SeqExpressionElision1/Foo::_tag
      IL_0012:  stloc.0
      IL_0013:  ldarg.1
      IL_0014:  ldfld      int32 SeqExpressionElision1/Foo::_tag
      IL_0019:  stloc.1
      IL_001a:  ldloc.0
      IL_001b:  ldloc.1
      IL_001c:  bne.un.s   IL_0020

      IL_001e:  ldc.i4.0
      IL_001f:  ret

      IL_0020:  ldloc.0
      IL_0021:  ldloc.1
      IL_0022:  sub
      IL_0023:  ret

      IL_0024:  ldc.i4.1
      IL_0025:  ret

      IL_0026:  ldarg.1
      IL_0027:  ldnull
      IL_0028:  cgt.un
      IL_002a:  brfalse.s  IL_002e

      IL_002c:  ldc.i4.m1
      IL_002d:  ret

      IL_002e:  ldc.i4.0
      IL_002f:  ret
    } // end of method Foo::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  unbox.any  SeqExpressionElision1/Foo
      IL_0007:  callvirt   instance int32 SeqExpressionElision1/Foo::CompareTo(class SeqExpressionElision1/Foo)
      IL_000c:  ret
    } // end of method Foo::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj,
                                      class [mscorlib]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       65 (0x41)
      .maxstack  4
      .locals init (class SeqExpressionElision1/Foo V_0,
               int32 V_1,
               int32 V_2)
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  SeqExpressionElision1/Foo
      IL_0006:  stloc.0
      IL_0007:  ldarg.0
      IL_0008:  ldnull
      IL_0009:  cgt.un
      IL_000b:  brfalse.s  IL_0032

      IL_000d:  ldarg.1
      IL_000e:  unbox.any  SeqExpressionElision1/Foo
      IL_0013:  ldnull
      IL_0014:  cgt.un
      IL_0016:  brfalse.s  IL_0030

      IL_0018:  ldarg.0
      IL_0019:  ldfld      int32 SeqExpressionElision1/Foo::_tag
      IL_001e:  stloc.1
      IL_001f:  ldloc.0
      IL_0020:  ldfld      int32 SeqExpressionElision1/Foo::_tag
      IL_0025:  stloc.2
      IL_0026:  ldloc.1
      IL_0027:  ldloc.2
      IL_0028:  bne.un.s   IL_002c

      IL_002a:  ldc.i4.0
      IL_002b:  ret

      IL_002c:  ldloc.1
      IL_002d:  ldloc.2
      IL_002e:  sub
      IL_002f:  ret

      IL_0030:  ldc.i4.1
      IL_0031:  ret

      IL_0032:  ldarg.1
      IL_0033:  unbox.any  SeqExpressionElision1/Foo
      IL_0038:  ldnull
      IL_0039:  cgt.un
      IL_003b:  brfalse.s  IL_003f

      IL_003d:  ldc.i4.m1
      IL_003e:  ret

      IL_003f:  ldc.i4.0
      IL_0040:  ret
    } // end of method Foo::CompareTo

    .method public hidebysig virtual final 
            instance int32  GetHashCode(class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       15 (0xf)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldnull
      IL_0002:  cgt.un
      IL_0004:  brfalse.s  IL_000d

      IL_0006:  ldarg.0
      IL_0007:  ldfld      int32 SeqExpressionElision1/Foo::_tag
      IL_000c:  ret

      IL_000d:  ldc.i4.0
      IL_000e:  ret
    } // end of method Foo::GetHashCode

    .method public hidebysig virtual final 
            instance int32  GetHashCode() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       class [mscorlib]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0006:  callvirt   instance int32 SeqExpressionElision1/Foo::GetHashCode(class [mscorlib]System.Collections.IEqualityComparer)
      IL_000b:  ret
    } // end of method Foo::GetHashCode

    .method public hidebysig virtual final 
            instance bool  Equals(object obj,
                                  class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       45 (0x2d)
      .maxstack  4
      .locals init (class SeqExpressionElision1/Foo V_0,
               int32 V_1,
               int32 V_2)
      IL_0000:  ldarg.0
      IL_0001:  ldnull
      IL_0002:  cgt.un
      IL_0004:  brfalse.s  IL_0025

      IL_0006:  ldarg.1
      IL_0007:  isinst     SeqExpressionElision1/Foo
      IL_000c:  stloc.0
      IL_000d:  ldloc.0
      IL_000e:  brfalse.s  IL_0023

      IL_0010:  ldarg.0
      IL_0011:  ldfld      int32 SeqExpressionElision1/Foo::_tag
      IL_0016:  stloc.1
      IL_0017:  ldloc.0
      IL_0018:  ldfld      int32 SeqExpressionElision1/Foo::_tag
      IL_001d:  stloc.2
      IL_001e:  ldloc.1
      IL_001f:  ldloc.2
      IL_0020:  ceq
      IL_0022:  ret

      IL_0023:  ldc.i4.0
      IL_0024:  ret

      IL_0025:  ldarg.1
      IL_0026:  ldnull
      IL_0027:  cgt.un
      IL_0029:  ldc.i4.0
      IL_002a:  ceq
      IL_002c:  ret
    } // end of method Foo::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(class SeqExpressionElision1/Foo obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       41 (0x29)
      .maxstack  4
      .locals init (int32 V_0,
               int32 V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldnull
      IL_0002:  cgt.un
      IL_0004:  brfalse.s  IL_0021

      IL_0006:  ldarg.1
      IL_0007:  ldnull
      IL_0008:  cgt.un
      IL_000a:  brfalse.s  IL_001f

      IL_000c:  ldarg.0
      IL_000d:  ldfld      int32 SeqExpressionElision1/Foo::_tag
      IL_0012:  stloc.0
      IL_0013:  ldarg.1
      IL_0014:  ldfld      int32 SeqExpressionElision1/Foo::_tag
      IL_0019:  stloc.1
      IL_001a:  ldloc.0
      IL_001b:  ldloc.1
      IL_001c:  ceq
      IL_001e:  ret

      IL_001f:  ldc.i4.0
      IL_0020:  ret

      IL_0021:  ldarg.1
      IL_0022:  ldnull
      IL_0023:  cgt.un
      IL_0025:  ldc.i4.0
      IL_0026:  ceq
      IL_0028:  ret
    } // end of method Foo::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(object obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       20 (0x14)
      .maxstack  4
      .locals init (class SeqExpressionElision1/Foo V_0)
      IL_0000:  ldarg.1
      IL_0001:  isinst     SeqExpressionElision1/Foo
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0012

      IL_000a:  ldarg.0
      IL_000b:  ldloc.0
      IL_000c:  callvirt   instance bool SeqExpressionElision1/Foo::Equals(class SeqExpressionElision1/Foo)
      IL_0011:  ret

      IL_0012:  ldc.i4.0
      IL_0013:  ret
    } // end of method Foo::Equals

    .property instance int32 Tag()
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance int32 SeqExpressionElision1/Foo::get_Tag()
    } // end of property Foo::Tag
    .property class SeqExpressionElision1/Foo
            Foo()
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get class SeqExpressionElision1/Foo SeqExpressionElision1/Foo::get_Foo()
    } // end of property Foo::Foo
    .property instance bool IsFoo()
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance bool SeqExpressionElision1/Foo::get_IsFoo()
    } // end of property Foo::IsFoo
    .property class SeqExpressionElision1/Foo
            Bar()
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get class SeqExpressionElision1/Foo SeqExpressionElision1/Foo::get_Bar()
    } // end of property Foo::Bar
    .property instance bool IsBar()
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance bool SeqExpressionElision1/Foo::get_IsBar()
    } // end of property Foo::IsBar
  } // end of class Foo

  .class auto ansi serializable sealed nested assembly beforefieldinit seq1@6
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>>
  {
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>>::.ctor()
      IL_0006:  ret
    } // end of method seq1@6::.ctor

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
            Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
    {
      // Code size       40 (0x28)
      .maxstack  7
      .locals init (class [mscorlib]System.Tuple`2<int32,int32> V_0,
               int32 V_1,
               int32 V_2)
      IL_0000:  ldc.i4.1
      IL_0001:  ldc.i4.2
      IL_0002:  newobj     instance void class [mscorlib]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                            !1)
      IL_0007:  stloc.0
      IL_0008:  ldloc.0
      IL_0009:  call       instance !1 class [mscorlib]System.Tuple`2<int32,int32>::get_Item2()
      IL_000e:  stloc.1
      IL_000f:  ldloc.0
      IL_0010:  call       instance !0 class [mscorlib]System.Tuple`2<int32,int32>::get_Item1()
      IL_0015:  stloc.2
      IL_0016:  ldloc.2
      IL_0017:  ldloc.1
      IL_0018:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
      IL_001d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
      IL_0022:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
      IL_0027:  ret
    } // end of method seq1@6::Invoke

  } // end of class seq1@6

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname seq2@11
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [mscorlib]System.Tuple`2<int32,int32> c
    .field public int32 b
    .field public int32 a
    .field public int32 pc
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 current
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [mscorlib]System.Tuple`2<int32,int32> c,
                                 int32 b,
                                 int32 a,
                                 int32 pc,
                                 int32 current) cil managed
    {
      // Code size       44 (0x2c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [mscorlib]System.Tuple`2<int32,int32> SeqExpressionElision1/seq2@11::c
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 SeqExpressionElision1/seq2@11::b
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 SeqExpressionElision1/seq2@11::a
      IL_0015:  ldarg.0
      IL_0016:  ldarg.s    pc
      IL_0018:  stfld      int32 SeqExpressionElision1/seq2@11::pc
      IL_001d:  ldarg.0
      IL_001e:  ldarg.s    current
      IL_0020:  stfld      int32 SeqExpressionElision1/seq2@11::current
      IL_0025:  ldarg.0
      IL_0026:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
      IL_002b:  ret
    } // end of method seq2@11::.ctor

    .method public strict virtual instance int32 
            GenerateNext(class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
    {
      // Code size       153 (0x99)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 SeqExpressionElision1/seq2@11::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_0017,
                            IL_0019)
      IL_0015:  br.s       IL_001b

      IL_0017:  br.s       IL_0074

      IL_0019:  br.s       IL_0090

      IL_001b:  ldarg.0
      IL_001c:  ldc.i4.1
      IL_001d:  ldc.i4.2
      IL_001e:  newobj     instance void class [mscorlib]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                            !1)
      IL_0023:  stfld      class [mscorlib]System.Tuple`2<int32,int32> SeqExpressionElision1/seq2@11::c
      IL_0028:  ldarg.0
      IL_0029:  ldarg.0
      IL_002a:  ldfld      class [mscorlib]System.Tuple`2<int32,int32> SeqExpressionElision1/seq2@11::c
      IL_002f:  call       instance !1 class [mscorlib]System.Tuple`2<int32,int32>::get_Item2()
      IL_0034:  stfld      int32 SeqExpressionElision1/seq2@11::b
      IL_0039:  ldarg.0
      IL_003a:  ldarg.0
      IL_003b:  ldfld      class [mscorlib]System.Tuple`2<int32,int32> SeqExpressionElision1/seq2@11::c
      IL_0040:  call       instance !0 class [mscorlib]System.Tuple`2<int32,int32>::get_Item1()
      IL_0045:  stfld      int32 SeqExpressionElision1/seq2@11::a
      IL_004a:  ldarg.0
      IL_004b:  ldc.i4.1
      IL_004c:  stfld      int32 SeqExpressionElision1/seq2@11::pc
      IL_0051:  ldarg.1
      IL_0052:  ldarg.0
      IL_0053:  ldfld      int32 SeqExpressionElision1/seq2@11::a
      IL_0058:  ldarg.0
      IL_0059:  ldfld      int32 SeqExpressionElision1/seq2@11::b
      IL_005e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
      IL_0063:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
      IL_0068:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
      IL_006d:  stobj      class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>
      IL_0072:  ldc.i4.2
      IL_0073:  ret

      IL_0074:  ldarg.0
      IL_0075:  ldc.i4.0
      IL_0076:  stfld      int32 SeqExpressionElision1/seq2@11::a
      IL_007b:  ldarg.0
      IL_007c:  ldc.i4.0
      IL_007d:  stfld      int32 SeqExpressionElision1/seq2@11::b
      IL_0082:  ldarg.0
      IL_0083:  ldnull
      IL_0084:  stfld      class [mscorlib]System.Tuple`2<int32,int32> SeqExpressionElision1/seq2@11::c
      IL_0089:  ldarg.0
      IL_008a:  ldc.i4.2
      IL_008b:  stfld      int32 SeqExpressionElision1/seq2@11::pc
      IL_0090:  ldarg.0
      IL_0091:  ldc.i4.0
      IL_0092:  stfld      int32 SeqExpressionElision1/seq2@11::current
      IL_0097:  ldc.i4.0
      IL_0098:  ret
    } // end of method seq2@11::GenerateNext

    .method public strict virtual instance void 
            Close() cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.2
      IL_0002:  stfld      int32 SeqExpressionElision1/seq2@11::pc
      IL_0007:  ret
    } // end of method seq2@11::Close

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      // Code size       35 (0x23)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 SeqExpressionElision1/seq2@11::pc
      IL_0006:  switch     ( 
                            IL_0019,
                            IL_001b,
                            IL_001d)
      IL_0017:  br.s       IL_001f

      IL_0019:  br.s       IL_0021

      IL_001b:  br.s       IL_001f

      IL_001d:  br.s       IL_0021

      IL_001f:  ldc.i4.0
      IL_0020:  ret

      IL_0021:  ldc.i4.0
      IL_0022:  ret
    } // end of method seq2@11::get_CheckClose

    .method public strict virtual instance int32 
            get_LastGenerated() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 SeqExpressionElision1/seq2@11::current
      IL_0006:  ret
    } // end of method seq2@11::get_LastGenerated

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       11 (0xb)
      .maxstack  9
      IL_0000:  ldnull
      IL_0001:  ldc.i4.0
      IL_0002:  ldc.i4.0
      IL_0003:  ldc.i4.0
      IL_0004:  ldc.i4.0
      IL_0005:  newobj     instance void SeqExpressionElision1/seq2@11::.ctor(class [mscorlib]System.Tuple`2<int32,int32>,
                                                                              int32,
                                                                              int32,
                                                                              int32,
                                                                              int32)
      IL_000a:  ret
    } // end of method seq2@11::GetFreshEnumerator

  } // end of class seq2@11

  .class auto ansi serializable sealed nested assembly beforefieldinit seq3@17
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>>
  {
    .field public class SeqExpressionElision1/Foo foo
    .method assembly specialname rtspecialname 
            instance void  .ctor(class SeqExpressionElision1/Foo foo) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class SeqExpressionElision1/Foo SeqExpressionElision1/seq3@17::foo
      IL_000d:  ret
    } // end of method seq3@17::.ctor

    .method public strict virtual instance class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
            Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
    {
      // Code size       140 (0x8c)
      .maxstack  7
      .locals init (class [mscorlib]System.Tuple`2<int32,int32> V_0,
               int32 V_1,
               int32 V_2)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class SeqExpressionElision1/Foo SeqExpressionElision1/seq3@17::foo
      IL_0006:  call       instance int32 SeqExpressionElision1/Foo::get_Tag()
      IL_000b:  ldc.i4.1
      IL_000c:  bne.un.s   IL_004d

      IL_000e:  ldarg.0
      IL_000f:  ldfld      class SeqExpressionElision1/Foo SeqExpressionElision1/seq3@17::foo
      IL_0014:  call       instance int32 SeqExpressionElision1/Foo::get_Tag()
      IL_0019:  ldc.i4.1
      IL_001a:  bne.un.s   IL_0025

      IL_001c:  ldc.i4.3
      IL_001d:  ldc.i4.4
      IL_001e:  newobj     instance void class [mscorlib]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                            !1)
      IL_0023:  br.s       IL_002c

      IL_0025:  ldc.i4.1
      IL_0026:  ldc.i4.2
      IL_0027:  newobj     instance void class [mscorlib]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                            !1)
      IL_002c:  stloc.0
      IL_002d:  ldloc.0
      IL_002e:  call       instance !1 class [mscorlib]System.Tuple`2<int32,int32>::get_Item2()
      IL_0033:  stloc.1
      IL_0034:  ldloc.0
      IL_0035:  call       instance !0 class [mscorlib]System.Tuple`2<int32,int32>::get_Item1()
      IL_003a:  stloc.2
      IL_003b:  ldloc.2
      IL_003c:  ldloc.1
      IL_003d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
      IL_0042:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
      IL_0047:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
      IL_004c:  ret

      IL_004d:  ldarg.0
      IL_004e:  ldfld      class SeqExpressionElision1/Foo SeqExpressionElision1/seq3@17::foo
      IL_0053:  call       instance int32 SeqExpressionElision1/Foo::get_Tag()
      IL_0058:  ldc.i4.1
      IL_0059:  bne.un.s   IL_0064

      IL_005b:  ldc.i4.3
      IL_005c:  ldc.i4.4
      IL_005d:  newobj     instance void class [mscorlib]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                            !1)
      IL_0062:  br.s       IL_006b

      IL_0064:  ldc.i4.1
      IL_0065:  ldc.i4.2
      IL_0066:  newobj     instance void class [mscorlib]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                            !1)
      IL_006b:  stloc.0
      IL_006c:  ldloc.0
      IL_006d:  call       instance !1 class [mscorlib]System.Tuple`2<int32,int32>::get_Item2()
      IL_0072:  stloc.1
      IL_0073:  ldloc.0
      IL_0074:  call       instance !0 class [mscorlib]System.Tuple`2<int32,int32>::get_Item1()
      IL_0079:  stloc.2
      IL_007a:  ldloc.2
      IL_007b:  ldloc.1
      IL_007c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
      IL_0081:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
      IL_0086:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
      IL_008b:  ret
    } // end of method seq3@17::Invoke

  } // end of class seq3@17

  .method public static class [mscorlib]System.Tuple`2<int32,int32> 
          getTuple(class SeqExpressionElision1/Foo foo) cil managed
  {
    // Code size       25 (0x19)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance int32 SeqExpressionElision1/Foo::get_Tag()
    IL_0006:  ldc.i4.1
    IL_0007:  bne.un.s   IL_0011

    IL_0009:  ldc.i4.3
    IL_000a:  ldc.i4.4
    IL_000b:  newobj     instance void class [mscorlib]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                          !1)
    IL_0010:  ret

    IL_0011:  ldc.i4.1
    IL_0012:  ldc.i4.2
    IL_0013:  newobj     instance void class [mscorlib]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                          !1)
    IL_0018:  ret
  } // end of method SeqExpressionElision1::getTuple

  .method public static class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          seq1() cil managed
  {
    // Code size       18 (0x12)
    .maxstack  8
    IL_0000:  newobj     instance void SeqExpressionElision1/seq1@6::.ctor()
    IL_0005:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Delay<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>>)
    IL_000a:  tail.
    IL_000c:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::CreateSequence<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0011:  ret
  } // end of method SeqExpressionElision1::seq1

  .method public static class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          seq2() cil managed
  {
    // Code size       11 (0xb)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ldc.i4.0
    IL_0002:  ldc.i4.0
    IL_0003:  ldc.i4.0
    IL_0004:  ldc.i4.0
    IL_0005:  newobj     instance void SeqExpressionElision1/seq2@11::.ctor(class [mscorlib]System.Tuple`2<int32,int32>,
                                                                            int32,
                                                                            int32,
                                                                            int32,
                                                                            int32)
    IL_000a:  ret
  } // end of method SeqExpressionElision1::seq2

  .method public static class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> 
          seq3(class SeqExpressionElision1/Foo foo) cil managed
  {
    // Code size       19 (0x13)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void SeqExpressionElision1/seq3@17::.ctor(class SeqExpressionElision1/Foo)
    IL_0006:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Delay<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>>)
    IL_000b:  tail.
    IL_000d:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::CreateSequence<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0012:  ret
  } // end of method SeqExpressionElision1::seq3

} // end of class SeqExpressionElision1

.class private abstract auto ansi sealed '<StartupCode$SeqExpressionElision1>'.$SeqExpressionElision1
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $SeqExpressionElision1::main@

} // end of class '<StartupCode$SeqExpressionElision1>'.$SeqExpressionElision1


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
