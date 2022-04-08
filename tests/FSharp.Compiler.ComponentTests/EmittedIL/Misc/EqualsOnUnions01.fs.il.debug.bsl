
//  Microsoft (R) .NET IL Disassembler.  Version 5.0.0-preview.7.20364.11



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly extern FSharp.Core
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 6:0:0:0
}
.assembly EqualsOnUnions01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.EqualsOnUnions01
{
  // Offset: 0x00000000 Length: 0x0000067D
  // WARNING: managed resource file FSharpSignatureData.EqualsOnUnions01 created
}
.mresource public FSharpOptimizationData.EqualsOnUnions01
{
  // Offset: 0x00000688 Length: 0x000001C7
  // WARNING: managed resource file FSharpOptimizationData.EqualsOnUnions01 created
}
.module EqualsOnUnions01.exe
// MVID: {624E3948-BBFB-14A0-A745-038348394E62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x036B0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed EqualsOnUnions01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto autochar serializable nested public beforefieldinit U
         extends [mscorlib]System.Object
         implements class [mscorlib]System.IEquatable`1<class EqualsOnUnions01/U>,
                    [mscorlib]System.Collections.IStructuralEquatable,
                    class [mscorlib]System.IComparable`1<class EqualsOnUnions01/U>,
                    [mscorlib]System.IComparable,
                    [mscorlib]System.Collections.IStructuralComparable
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 7B 5F 5F 44 65 62 75 67 44 69 73 70 6C   // ...{__DebugDispl
                                                                                                   61 79 28 29 2C 6E 71 7D 00 00 )                   // ay(),nq}..
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 01 00 00 00 00 00 ) 
    .class abstract auto ansi sealed nested public Tags
           extends [mscorlib]System.Object
    {
      .field public static literal int32 A = int32(0x00000000)
      .field public static literal int32 B = int32(0x00000001)
    } // end of class Tags

    .class auto ansi serializable nested assembly beforefieldinit specialname _A
           extends EqualsOnUnions01/U
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerTypeProxyAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 24 45 71 75 61 6C 73 4F 6E 55 6E 69 6F 6E   // ..$EqualsOnUnion
                                                                                                                            73 30 31 2B 55 2B 5F 41 40 44 65 62 75 67 54 79   // s01+U+_A@DebugTy
                                                                                                                            70 65 50 72 6F 78 79 00 00 )                      // peProxy..
      .custom instance void [mscorlib]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 7B 5F 5F 44 65 62 75 67 44 69 73 70 6C   // ...{__DebugDispl
                                                                                                     61 79 28 29 2C 6E 71 7D 00 00 )                   // ay(),nq}..
      .method assembly specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void EqualsOnUnions01/U::.ctor()
        IL_0006:  ret
      } // end of method _A::.ctor

    } // end of class _A

    .class auto ansi serializable nested public beforefieldinit specialname B
           extends EqualsOnUnions01/U
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerTypeProxyAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 23 45 71 75 61 6C 73 4F 6E 55 6E 69 6F 6E   // ..#EqualsOnUnion
                                                                                                                            73 30 31 2B 55 2B 42 40 44 65 62 75 67 54 79 70   // s01+U+B@DebugTyp
                                                                                                                            65 50 72 6F 78 79 00 00 )                         // eProxy..
      .custom instance void [mscorlib]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 7B 5F 5F 44 65 62 75 67 44 69 73 70 6C   // ...{__DebugDispl
                                                                                                     61 79 28 29 2C 6E 71 7D 00 00 )                   // ay(),nq}..
      .field assembly initonly int32 item
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method assembly specialname rtspecialname 
              instance void  .ctor(int32 item) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       14 (0xe)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void EqualsOnUnions01/U::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      int32 EqualsOnUnions01/U/B::item
        IL_000d:  ret
      } // end of method B::.ctor

      .method public hidebysig instance int32 
              get_Item() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 EqualsOnUnions01/U/B::item
        IL_0006:  ret
      } // end of method B::get_Item

      .property instance int32 Item()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 01 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 EqualsOnUnions01/U/B::get_Item()
      } // end of property B::Item
    } // end of class B

    .class auto ansi nested assembly beforefieldinit specialname _A@DebugTypeProxy
           extends [mscorlib]System.Object
    {
      .field assembly class EqualsOnUnions01/U/_A _obj
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method public specialname rtspecialname 
              instance void  .ctor(class EqualsOnUnions01/U/_A obj) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       14 (0xe)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class EqualsOnUnions01/U/_A EqualsOnUnions01/U/_A@DebugTypeProxy::_obj
        IL_000d:  ret
      } // end of method _A@DebugTypeProxy::.ctor

    } // end of class _A@DebugTypeProxy

    .class auto ansi nested assembly beforefieldinit specialname B@DebugTypeProxy
           extends [mscorlib]System.Object
    {
      .field assembly class EqualsOnUnions01/U/B _obj
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method public specialname rtspecialname 
              instance void  .ctor(class EqualsOnUnions01/U/B obj) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       14 (0xe)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class EqualsOnUnions01/U/B EqualsOnUnions01/U/B@DebugTypeProxy::_obj
        IL_000d:  ret
      } // end of method B@DebugTypeProxy::.ctor

      .method public hidebysig instance int32 
              get_Item() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       12 (0xc)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class EqualsOnUnions01/U/B EqualsOnUnions01/U/B@DebugTypeProxy::_obj
        IL_0006:  ldfld      int32 EqualsOnUnions01/U/B::item
        IL_000b:  ret
      } // end of method B@DebugTypeProxy::get_Item

      .property instance int32 Item()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 01 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 EqualsOnUnions01/U/B@DebugTypeProxy::get_Item()
      } // end of property B@DebugTypeProxy::Item
    } // end of class B@DebugTypeProxy

    .field static assembly initonly class EqualsOnUnions01/U _unique_A
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  8
      IL_0000:  newobj     instance void EqualsOnUnions01/U/_A::.ctor()
      IL_0005:  stsfld     class EqualsOnUnions01/U EqualsOnUnions01/U::_unique_A
      IL_000a:  ret
    } // end of method U::.cctor

    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method U::.ctor

    .method public static class EqualsOnUnions01/U 
            get_A() cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 08 00 00 00 00 00 00 00 00 00 ) 
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  ldsfld     class EqualsOnUnions01/U EqualsOnUnions01/U::_unique_A
      IL_0005:  ret
    } // end of method U::get_A

    .method public hidebysig instance bool 
            get_IsA() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       10 (0xa)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  isinst     EqualsOnUnions01/U/_A
      IL_0006:  ldnull
      IL_0007:  cgt.un
      IL_0009:  ret
    } // end of method U::get_IsA

    .method public static class EqualsOnUnions01/U 
            NewB(int32 item) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 08 00 00 00 01 00 00 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  newobj     instance void EqualsOnUnions01/U/B::.ctor(int32)
      IL_0006:  ret
    } // end of method U::NewB

    .method public hidebysig instance bool 
            get_IsB() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       10 (0xa)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  isinst     EqualsOnUnions01/U/B
      IL_0006:  ldnull
      IL_0007:  cgt.un
      IL_0009:  ret
    } // end of method U::get_IsB

    .method public hidebysig instance int32 
            get_Tag() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  isinst     EqualsOnUnions01/U/B
      IL_0006:  brfalse.s  IL_000b

      IL_0008:  ldc.i4.1
      IL_0009:  br.s       IL_000c

      IL_000b:  ldc.i4.0
      IL_000c:  ret
    } // end of method U::get_Tag

    .method assembly hidebysig specialname 
            instance object  __DebugDisplay() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       22 (0x16)
      .maxstack  8
      IL_0000:  ldstr      "%+0.8A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class EqualsOnUnions01/U,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,string>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class EqualsOnUnions01/U,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class EqualsOnUnions01/U,string>::Invoke(!0)
      IL_0015:  ret
    } // end of method U::__DebugDisplay

    .method public strict virtual instance string 
            ToString() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       22 (0x16)
      .maxstack  8
      IL_0000:  ldstr      "%+A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class EqualsOnUnions01/U,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class EqualsOnUnions01/U>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class EqualsOnUnions01/U,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class EqualsOnUnions01/U,string>::Invoke(!0)
      IL_0015:  ret
    } // end of method U::ToString

    .method public hidebysig virtual final 
            instance int32  CompareTo(class EqualsOnUnions01/U obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       137 (0x89)
      .maxstack  4
      .locals init (int32 V_0,
               class EqualsOnUnions01/U V_1,
               int32 V_2,
               class EqualsOnUnions01/U V_3,
               class EqualsOnUnions01/U/B V_4,
               class EqualsOnUnions01/U/B V_5,
               class [mscorlib]System.Collections.IComparer V_6,
               int32 V_7,
               int32 V_8,
               class [mscorlib]System.Collections.IComparer V_9,
               int32 V_10,
               int32 V_11)
      IL_0000:  ldarg.0
      IL_0001:  brfalse    IL_0082

      IL_0006:  ldarg.1
      IL_0007:  brfalse    IL_0080

      IL_000c:  ldarg.0
      IL_000d:  stloc.1
      IL_000e:  ldloc.1
      IL_000f:  isinst     EqualsOnUnions01/U/B
      IL_0014:  brfalse.s  IL_0019

      IL_0016:  ldc.i4.1
      IL_0017:  br.s       IL_001a

      IL_0019:  ldc.i4.0
      IL_001a:  stloc.0
      IL_001b:  ldarg.1
      IL_001c:  stloc.3
      IL_001d:  ldloc.3
      IL_001e:  isinst     EqualsOnUnions01/U/B
      IL_0023:  brfalse.s  IL_0028

      IL_0025:  ldc.i4.1
      IL_0026:  br.s       IL_0029

      IL_0028:  ldc.i4.0
      IL_0029:  stloc.2
      IL_002a:  ldloc.0
      IL_002b:  ldloc.2
      IL_002c:  bne.un.s   IL_007c

      IL_002e:  ldarg.0
      IL_002f:  isinst     EqualsOnUnions01/U/B
      IL_0034:  brfalse.s  IL_007a

      IL_0036:  ldarg.0
      IL_0037:  castclass  EqualsOnUnions01/U/B
      IL_003c:  stloc.s    V_4
      IL_003e:  ldarg.1
      IL_003f:  castclass  EqualsOnUnions01/U/B
      IL_0044:  stloc.s    V_5
      IL_0046:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_004b:  stloc.s    V_6
      IL_004d:  ldloc.s    V_4
      IL_004f:  ldfld      int32 EqualsOnUnions01/U/B::item
      IL_0054:  stloc.s    V_7
      IL_0056:  ldloc.s    V_5
      IL_0058:  ldfld      int32 EqualsOnUnions01/U/B::item
      IL_005d:  stloc.s    V_8
      IL_005f:  ldloc.s    V_6
      IL_0061:  stloc.s    V_9
      IL_0063:  ldloc.s    V_7
      IL_0065:  stloc.s    V_10
      IL_0067:  ldloc.s    V_8
      IL_0069:  stloc.s    V_11
      IL_006b:  ldloc.s    V_10
      IL_006d:  ldloc.s    V_11
      IL_006f:  bge.s      IL_0073

      IL_0071:  ldc.i4.m1
      IL_0072:  ret

      IL_0073:  ldloc.s    V_10
      IL_0075:  ldloc.s    V_11
      IL_0077:  cgt
      IL_0079:  ret

      IL_007a:  ldc.i4.0
      IL_007b:  ret

      IL_007c:  ldloc.0
      IL_007d:  ldloc.2
      IL_007e:  sub
      IL_007f:  ret

      IL_0080:  ldc.i4.1
      IL_0081:  ret

      IL_0082:  ldarg.1
      IL_0083:  brfalse.s  IL_0087

      IL_0085:  ldc.i4.m1
      IL_0086:  ret

      IL_0087:  ldc.i4.0
      IL_0088:  ret
    } // end of method U::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  unbox.any  EqualsOnUnions01/U
      IL_0007:  callvirt   instance int32 EqualsOnUnions01/U::CompareTo(class EqualsOnUnions01/U)
      IL_000c:  ret
    } // end of method U::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj,
                                      class [mscorlib]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       152 (0x98)
      .maxstack  4
      .locals init (class EqualsOnUnions01/U V_0,
               int32 V_1,
               class EqualsOnUnions01/U V_2,
               int32 V_3,
               class EqualsOnUnions01/U V_4,
               class EqualsOnUnions01/U/B V_5,
               class EqualsOnUnions01/U/B V_6,
               class [mscorlib]System.Collections.IComparer V_7,
               int32 V_8,
               int32 V_9,
               class [mscorlib]System.Collections.IComparer V_10,
               int32 V_11,
               int32 V_12)
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  EqualsOnUnions01/U
      IL_0006:  stloc.0
      IL_0007:  ldarg.0
      IL_0008:  brfalse    IL_008c

      IL_000d:  ldarg.1
      IL_000e:  unbox.any  EqualsOnUnions01/U
      IL_0013:  brfalse    IL_008a

      IL_0018:  ldarg.0
      IL_0019:  stloc.2
      IL_001a:  ldloc.2
      IL_001b:  isinst     EqualsOnUnions01/U/B
      IL_0020:  brfalse.s  IL_0025

      IL_0022:  ldc.i4.1
      IL_0023:  br.s       IL_0026

      IL_0025:  ldc.i4.0
      IL_0026:  stloc.1
      IL_0027:  ldloc.0
      IL_0028:  stloc.s    V_4
      IL_002a:  ldloc.s    V_4
      IL_002c:  isinst     EqualsOnUnions01/U/B
      IL_0031:  brfalse.s  IL_0036

      IL_0033:  ldc.i4.1
      IL_0034:  br.s       IL_0037

      IL_0036:  ldc.i4.0
      IL_0037:  stloc.3
      IL_0038:  ldloc.1
      IL_0039:  ldloc.3
      IL_003a:  bne.un.s   IL_0086

      IL_003c:  ldarg.0
      IL_003d:  isinst     EqualsOnUnions01/U/B
      IL_0042:  brfalse.s  IL_0084

      IL_0044:  ldarg.0
      IL_0045:  castclass  EqualsOnUnions01/U/B
      IL_004a:  stloc.s    V_5
      IL_004c:  ldloc.0
      IL_004d:  castclass  EqualsOnUnions01/U/B
      IL_0052:  stloc.s    V_6
      IL_0054:  ldarg.2
      IL_0055:  stloc.s    V_7
      IL_0057:  ldloc.s    V_5
      IL_0059:  ldfld      int32 EqualsOnUnions01/U/B::item
      IL_005e:  stloc.s    V_8
      IL_0060:  ldloc.s    V_6
      IL_0062:  ldfld      int32 EqualsOnUnions01/U/B::item
      IL_0067:  stloc.s    V_9
      IL_0069:  ldloc.s    V_7
      IL_006b:  stloc.s    V_10
      IL_006d:  ldloc.s    V_8
      IL_006f:  stloc.s    V_11
      IL_0071:  ldloc.s    V_9
      IL_0073:  stloc.s    V_12
      IL_0075:  ldloc.s    V_11
      IL_0077:  ldloc.s    V_12
      IL_0079:  bge.s      IL_007d

      IL_007b:  ldc.i4.m1
      IL_007c:  ret

      IL_007d:  ldloc.s    V_11
      IL_007f:  ldloc.s    V_12
      IL_0081:  cgt
      IL_0083:  ret

      IL_0084:  ldc.i4.0
      IL_0085:  ret

      IL_0086:  ldloc.1
      IL_0087:  ldloc.3
      IL_0088:  sub
      IL_0089:  ret

      IL_008a:  ldc.i4.1
      IL_008b:  ret

      IL_008c:  ldarg.1
      IL_008d:  unbox.any  EqualsOnUnions01/U
      IL_0092:  brfalse.s  IL_0096

      IL_0094:  ldc.i4.m1
      IL_0095:  ret

      IL_0096:  ldc.i4.0
      IL_0097:  ret
    } // end of method U::CompareTo

    .method public hidebysig virtual final 
            instance int32  GetHashCode(class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       71 (0x47)
      .maxstack  7
      .locals init (int32 V_0,
               class EqualsOnUnions01/U/B V_1,
               class [mscorlib]System.Collections.IEqualityComparer V_2,
               int32 V_3,
               class [mscorlib]System.Collections.IEqualityComparer V_4,
               class EqualsOnUnions01/U V_5)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0045

      IL_0003:  ldc.i4.0
      IL_0004:  stloc.0
      IL_0005:  ldarg.0
      IL_0006:  isinst     EqualsOnUnions01/U/B
      IL_000b:  brfalse.s  IL_0034

      IL_000d:  ldarg.0
      IL_000e:  castclass  EqualsOnUnions01/U/B
      IL_0013:  stloc.1
      IL_0014:  ldc.i4.1
      IL_0015:  stloc.0
      IL_0016:  ldc.i4     0x9e3779b9
      IL_001b:  ldarg.1
      IL_001c:  stloc.2
      IL_001d:  ldloc.1
      IL_001e:  ldfld      int32 EqualsOnUnions01/U/B::item
      IL_0023:  stloc.3
      IL_0024:  ldloc.2
      IL_0025:  stloc.s    V_4
      IL_0027:  ldloc.3
      IL_0028:  ldloc.0
      IL_0029:  ldc.i4.6
      IL_002a:  shl
      IL_002b:  ldloc.0
      IL_002c:  ldc.i4.2
      IL_002d:  shr
      IL_002e:  add
      IL_002f:  add
      IL_0030:  add
      IL_0031:  stloc.0
      IL_0032:  ldloc.0
      IL_0033:  ret

      IL_0034:  ldarg.0
      IL_0035:  stloc.s    V_5
      IL_0037:  ldloc.s    V_5
      IL_0039:  isinst     EqualsOnUnions01/U/B
      IL_003e:  brfalse.s  IL_0043

      IL_0040:  ldc.i4.1
      IL_0041:  br.s       IL_0044

      IL_0043:  ldc.i4.0
      IL_0044:  ret

      IL_0045:  ldc.i4.0
      IL_0046:  ret
    } // end of method U::GetHashCode

    .method public hidebysig virtual final 
            instance int32  GetHashCode() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       class [mscorlib]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0006:  callvirt   instance int32 EqualsOnUnions01/U::GetHashCode(class [mscorlib]System.Collections.IEqualityComparer)
      IL_000b:  ret
    } // end of method U::GetHashCode

    .method public hidebysig virtual final 
            instance bool  Equals(object obj,
                                  class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       126 (0x7e)
      .maxstack  4
      .locals init (class EqualsOnUnions01/U V_0,
               class EqualsOnUnions01/U V_1,
               int32 V_2,
               class EqualsOnUnions01/U V_3,
               int32 V_4,
               class EqualsOnUnions01/U V_5,
               class EqualsOnUnions01/U/B V_6,
               class EqualsOnUnions01/U/B V_7,
               class [mscorlib]System.Collections.IEqualityComparer V_8,
               int32 V_9,
               int32 V_10,
               class [mscorlib]System.Collections.IEqualityComparer V_11)
      IL_0000:  ldarg.0
      IL_0001:  brfalse    IL_0076

      IL_0006:  ldarg.1
      IL_0007:  isinst     EqualsOnUnions01/U
      IL_000c:  stloc.0
      IL_000d:  ldloc.0
      IL_000e:  brfalse.s  IL_0074

      IL_0010:  ldloc.0
      IL_0011:  stloc.1
      IL_0012:  ldarg.0
      IL_0013:  stloc.3
      IL_0014:  ldloc.3
      IL_0015:  isinst     EqualsOnUnions01/U/B
      IL_001a:  brfalse.s  IL_001f

      IL_001c:  ldc.i4.1
      IL_001d:  br.s       IL_0020

      IL_001f:  ldc.i4.0
      IL_0020:  stloc.2
      IL_0021:  ldloc.1
      IL_0022:  stloc.s    V_5
      IL_0024:  ldloc.s    V_5
      IL_0026:  isinst     EqualsOnUnions01/U/B
      IL_002b:  brfalse.s  IL_0030

      IL_002d:  ldc.i4.1
      IL_002e:  br.s       IL_0031

      IL_0030:  ldc.i4.0
      IL_0031:  stloc.s    V_4
      IL_0033:  ldloc.2
      IL_0034:  ldloc.s    V_4
      IL_0036:  bne.un.s   IL_0072

      IL_0038:  ldarg.0
      IL_0039:  isinst     EqualsOnUnions01/U/B
      IL_003e:  brfalse.s  IL_0070

      IL_0040:  ldarg.0
      IL_0041:  castclass  EqualsOnUnions01/U/B
      IL_0046:  stloc.s    V_6
      IL_0048:  ldloc.1
      IL_0049:  castclass  EqualsOnUnions01/U/B
      IL_004e:  stloc.s    V_7
      IL_0050:  ldarg.2
      IL_0051:  stloc.s    V_8
      IL_0053:  ldloc.s    V_6
      IL_0055:  ldfld      int32 EqualsOnUnions01/U/B::item
      IL_005a:  stloc.s    V_9
      IL_005c:  ldloc.s    V_7
      IL_005e:  ldfld      int32 EqualsOnUnions01/U/B::item
      IL_0063:  stloc.s    V_10
      IL_0065:  ldloc.s    V_8
      IL_0067:  stloc.s    V_11
      IL_0069:  ldloc.s    V_9
      IL_006b:  ldloc.s    V_10
      IL_006d:  ceq
      IL_006f:  ret

      IL_0070:  ldc.i4.1
      IL_0071:  ret

      IL_0072:  ldc.i4.0
      IL_0073:  ret

      IL_0074:  ldc.i4.0
      IL_0075:  ret

      IL_0076:  ldarg.1
      IL_0077:  ldnull
      IL_0078:  cgt.un
      IL_007a:  ldc.i4.0
      IL_007b:  ceq
      IL_007d:  ret
    } // end of method U::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(class EqualsOnUnions01/U obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       95 (0x5f)
      .maxstack  4
      .locals init (int32 V_0,
               class EqualsOnUnions01/U V_1,
               int32 V_2,
               class EqualsOnUnions01/U V_3,
               class EqualsOnUnions01/U/B V_4,
               class EqualsOnUnions01/U/B V_5)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0057

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_0055

      IL_0006:  ldarg.0
      IL_0007:  stloc.1
      IL_0008:  ldloc.1
      IL_0009:  isinst     EqualsOnUnions01/U/B
      IL_000e:  brfalse.s  IL_0013

      IL_0010:  ldc.i4.1
      IL_0011:  br.s       IL_0014

      IL_0013:  ldc.i4.0
      IL_0014:  stloc.0
      IL_0015:  ldarg.1
      IL_0016:  stloc.3
      IL_0017:  ldloc.3
      IL_0018:  isinst     EqualsOnUnions01/U/B
      IL_001d:  brfalse.s  IL_0022

      IL_001f:  ldc.i4.1
      IL_0020:  br.s       IL_0023

      IL_0022:  ldc.i4.0
      IL_0023:  stloc.2
      IL_0024:  ldloc.0
      IL_0025:  ldloc.2
      IL_0026:  bne.un.s   IL_0053

      IL_0028:  ldarg.0
      IL_0029:  isinst     EqualsOnUnions01/U/B
      IL_002e:  brfalse.s  IL_0051

      IL_0030:  ldarg.0
      IL_0031:  castclass  EqualsOnUnions01/U/B
      IL_0036:  stloc.s    V_4
      IL_0038:  ldarg.1
      IL_0039:  castclass  EqualsOnUnions01/U/B
      IL_003e:  stloc.s    V_5
      IL_0040:  ldloc.s    V_4
      IL_0042:  ldfld      int32 EqualsOnUnions01/U/B::item
      IL_0047:  ldloc.s    V_5
      IL_0049:  ldfld      int32 EqualsOnUnions01/U/B::item
      IL_004e:  ceq
      IL_0050:  ret

      IL_0051:  ldc.i4.1
      IL_0052:  ret

      IL_0053:  ldc.i4.0
      IL_0054:  ret

      IL_0055:  ldc.i4.0
      IL_0056:  ret

      IL_0057:  ldarg.1
      IL_0058:  ldnull
      IL_0059:  cgt.un
      IL_005b:  ldc.i4.0
      IL_005c:  ceq
      IL_005e:  ret
    } // end of method U::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(object obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       20 (0x14)
      .maxstack  4
      .locals init (class EqualsOnUnions01/U V_0)
      IL_0000:  ldarg.1
      IL_0001:  isinst     EqualsOnUnions01/U
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0012

      IL_000a:  ldarg.0
      IL_000b:  ldloc.0
      IL_000c:  callvirt   instance bool EqualsOnUnions01/U::Equals(class EqualsOnUnions01/U)
      IL_0011:  ret

      IL_0012:  ldc.i4.0
      IL_0013:  ret
    } // end of method U::Equals

    .property instance int32 Tag()
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance int32 EqualsOnUnions01/U::get_Tag()
    } // end of property U::Tag
    .property class EqualsOnUnions01/U A()
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get class EqualsOnUnions01/U EqualsOnUnions01/U::get_A()
    } // end of property U::A
    .property instance bool IsA()
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance bool EqualsOnUnions01/U::get_IsA()
    } // end of property U::IsA
    .property instance bool IsB()
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance bool EqualsOnUnions01/U::get_IsB()
    } // end of property U::IsB
  } // end of class U

} // end of class EqualsOnUnions01

.class private abstract auto ansi sealed '<StartupCode$EqualsOnUnions01>'.$EqualsOnUnions01
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $EqualsOnUnions01::main@

} // end of class '<StartupCode$EqualsOnUnions01>'.$EqualsOnUnions01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file C:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net472\tests\EmittedIL\Misc\EqualsOnUnions01_fs\EqualsOnUnions01.res
