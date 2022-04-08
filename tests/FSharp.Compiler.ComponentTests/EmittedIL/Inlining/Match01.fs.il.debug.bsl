
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
.assembly Match01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Match01
{
  // Offset: 0x00000000 Length: 0x00000722
  // WARNING: managed resource file FSharpSignatureData.Match01 created
}
.mresource public FSharpOptimizationData.Match01
{
  // Offset: 0x00000728 Length: 0x000003B7
  // WARNING: managed resource file FSharpOptimizationData.Match01 created
}
.module Match01.exe
// MVID: {624E9975-1470-80B4-A745-038375994E62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x04BD0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Match01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto autochar serializable nested public beforefieldinit Test1
         extends [mscorlib]System.Object
         implements class [mscorlib]System.IEquatable`1<class Match01/Test1>,
                    [mscorlib]System.Collections.IStructuralEquatable,
                    class [mscorlib]System.IComparable`1<class Match01/Test1>,
                    [mscorlib]System.IComparable,
                    [mscorlib]System.Collections.IStructuralComparable
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 7B 5F 5F 44 65 62 75 67 44 69 73 70 6C   // ...{__DebugDispl
                                                                                                   61 79 28 29 2C 6E 71 7D 00 00 )                   // ay(),nq}..
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 01 00 00 00 00 00 ) 
    .class abstract auto ansi sealed nested public Tags
           extends [mscorlib]System.Object
    {
      .field public static literal int32 X11 = int32(0x00000000)
      .field public static literal int32 X12 = int32(0x00000001)
      .field public static literal int32 X13 = int32(0x00000002)
      .field public static literal int32 X14 = int32(0x00000003)
    } // end of class Tags

    .class auto ansi serializable nested public beforefieldinit specialname X11
           extends Match01/Test1
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerTypeProxyAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 20 4D 61 74 63 68 30 31 2B 54 65 73 74 31   // .. Match01+Test1
                                                                                                                            2B 58 31 31 40 44 65 62 75 67 54 79 70 65 50 72   // +X11@DebugTypePr
                                                                                                                            6F 78 79 00 00 )                                  // oxy..
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
        // Code size       15 (0xf)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldc.i4.0
        IL_0002:  call       instance void Match01/Test1::.ctor(int32)
        IL_0007:  ldarg.0
        IL_0008:  ldarg.1
        IL_0009:  stfld      int32 Match01/Test1/X11::item
        IL_000e:  ret
      } // end of method X11::.ctor

      .method public hidebysig instance int32 
              get_Item() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 Match01/Test1/X11::item
        IL_0006:  ret
      } // end of method X11::get_Item

      .property instance int32 Item()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 Match01/Test1/X11::get_Item()
      } // end of property X11::Item
    } // end of class X11

    .class auto ansi serializable nested public beforefieldinit specialname X12
           extends Match01/Test1
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerTypeProxyAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 20 4D 61 74 63 68 30 31 2B 54 65 73 74 31   // .. Match01+Test1
                                                                                                                            2B 58 31 32 40 44 65 62 75 67 54 79 70 65 50 72   // +X12@DebugTypePr
                                                                                                                            6F 78 79 00 00 )                                  // oxy..
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
        // Code size       15 (0xf)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldc.i4.1
        IL_0002:  call       instance void Match01/Test1::.ctor(int32)
        IL_0007:  ldarg.0
        IL_0008:  ldarg.1
        IL_0009:  stfld      int32 Match01/Test1/X12::item
        IL_000e:  ret
      } // end of method X12::.ctor

      .method public hidebysig instance int32 
              get_Item() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 Match01/Test1/X12::item
        IL_0006:  ret
      } // end of method X12::get_Item

      .property instance int32 Item()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 01 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 Match01/Test1/X12::get_Item()
      } // end of property X12::Item
    } // end of class X12

    .class auto ansi serializable nested public beforefieldinit specialname X13
           extends Match01/Test1
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerTypeProxyAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 20 4D 61 74 63 68 30 31 2B 54 65 73 74 31   // .. Match01+Test1
                                                                                                                            2B 58 31 33 40 44 65 62 75 67 54 79 70 65 50 72   // +X13@DebugTypePr
                                                                                                                            6F 78 79 00 00 )                                  // oxy..
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
        // Code size       15 (0xf)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldc.i4.2
        IL_0002:  call       instance void Match01/Test1::.ctor(int32)
        IL_0007:  ldarg.0
        IL_0008:  ldarg.1
        IL_0009:  stfld      int32 Match01/Test1/X13::item
        IL_000e:  ret
      } // end of method X13::.ctor

      .method public hidebysig instance int32 
              get_Item() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 Match01/Test1/X13::item
        IL_0006:  ret
      } // end of method X13::get_Item

      .property instance int32 Item()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 02 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 Match01/Test1/X13::get_Item()
      } // end of property X13::Item
    } // end of class X13

    .class auto ansi serializable nested public beforefieldinit specialname X14
           extends Match01/Test1
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerTypeProxyAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 20 4D 61 74 63 68 30 31 2B 54 65 73 74 31   // .. Match01+Test1
                                                                                                                            2B 58 31 34 40 44 65 62 75 67 54 79 70 65 50 72   // +X14@DebugTypePr
                                                                                                                            6F 78 79 00 00 )                                  // oxy..
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
        // Code size       15 (0xf)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldc.i4.3
        IL_0002:  call       instance void Match01/Test1::.ctor(int32)
        IL_0007:  ldarg.0
        IL_0008:  ldarg.1
        IL_0009:  stfld      int32 Match01/Test1/X14::item
        IL_000e:  ret
      } // end of method X14::.ctor

      .method public hidebysig instance int32 
              get_Item() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 Match01/Test1/X14::item
        IL_0006:  ret
      } // end of method X14::get_Item

      .property instance int32 Item()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 03 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 Match01/Test1/X14::get_Item()
      } // end of property X14::Item
    } // end of class X14

    .class auto ansi nested assembly beforefieldinit specialname X11@DebugTypeProxy
           extends [mscorlib]System.Object
    {
      .field assembly class Match01/Test1/X11 _obj
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method public specialname rtspecialname 
              instance void  .ctor(class Match01/Test1/X11 obj) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       14 (0xe)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class Match01/Test1/X11 Match01/Test1/X11@DebugTypeProxy::_obj
        IL_000d:  ret
      } // end of method X11@DebugTypeProxy::.ctor

      .method public hidebysig instance int32 
              get_Item() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       12 (0xc)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class Match01/Test1/X11 Match01/Test1/X11@DebugTypeProxy::_obj
        IL_0006:  ldfld      int32 Match01/Test1/X11::item
        IL_000b:  ret
      } // end of method X11@DebugTypeProxy::get_Item

      .property instance int32 Item()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 Match01/Test1/X11@DebugTypeProxy::get_Item()
      } // end of property X11@DebugTypeProxy::Item
    } // end of class X11@DebugTypeProxy

    .class auto ansi nested assembly beforefieldinit specialname X12@DebugTypeProxy
           extends [mscorlib]System.Object
    {
      .field assembly class Match01/Test1/X12 _obj
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method public specialname rtspecialname 
              instance void  .ctor(class Match01/Test1/X12 obj) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       14 (0xe)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class Match01/Test1/X12 Match01/Test1/X12@DebugTypeProxy::_obj
        IL_000d:  ret
      } // end of method X12@DebugTypeProxy::.ctor

      .method public hidebysig instance int32 
              get_Item() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       12 (0xc)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class Match01/Test1/X12 Match01/Test1/X12@DebugTypeProxy::_obj
        IL_0006:  ldfld      int32 Match01/Test1/X12::item
        IL_000b:  ret
      } // end of method X12@DebugTypeProxy::get_Item

      .property instance int32 Item()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 01 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 Match01/Test1/X12@DebugTypeProxy::get_Item()
      } // end of property X12@DebugTypeProxy::Item
    } // end of class X12@DebugTypeProxy

    .class auto ansi nested assembly beforefieldinit specialname X13@DebugTypeProxy
           extends [mscorlib]System.Object
    {
      .field assembly class Match01/Test1/X13 _obj
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method public specialname rtspecialname 
              instance void  .ctor(class Match01/Test1/X13 obj) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       14 (0xe)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class Match01/Test1/X13 Match01/Test1/X13@DebugTypeProxy::_obj
        IL_000d:  ret
      } // end of method X13@DebugTypeProxy::.ctor

      .method public hidebysig instance int32 
              get_Item() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       12 (0xc)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class Match01/Test1/X13 Match01/Test1/X13@DebugTypeProxy::_obj
        IL_0006:  ldfld      int32 Match01/Test1/X13::item
        IL_000b:  ret
      } // end of method X13@DebugTypeProxy::get_Item

      .property instance int32 Item()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 02 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 Match01/Test1/X13@DebugTypeProxy::get_Item()
      } // end of property X13@DebugTypeProxy::Item
    } // end of class X13@DebugTypeProxy

    .class auto ansi nested assembly beforefieldinit specialname X14@DebugTypeProxy
           extends [mscorlib]System.Object
    {
      .field assembly class Match01/Test1/X14 _obj
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method public specialname rtspecialname 
              instance void  .ctor(class Match01/Test1/X14 obj) cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       14 (0xe)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class Match01/Test1/X14 Match01/Test1/X14@DebugTypeProxy::_obj
        IL_000d:  ret
      } // end of method X14@DebugTypeProxy::.ctor

      .method public hidebysig instance int32 
              get_Item() cil managed
      {
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        // Code size       12 (0xc)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class Match01/Test1/X14 Match01/Test1/X14@DebugTypeProxy::_obj
        IL_0006:  ldfld      int32 Match01/Test1/X14::item
        IL_000b:  ret
      } // end of method X14@DebugTypeProxy::get_Item

      .property instance int32 Item()
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                    int32,
                                                                                                    int32) = ( 01 00 04 00 00 00 03 00 00 00 00 00 00 00 00 00 ) 
        .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .get instance int32 Match01/Test1/X14@DebugTypeProxy::get_Item()
      } // end of property X14@DebugTypeProxy::Item
    } // end of class X14@DebugTypeProxy

    .field assembly initonly int32 _tag
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
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
      IL_0008:  stfld      int32 Match01/Test1::_tag
      IL_000d:  ret
    } // end of method Test1::.ctor

    .method public static class Match01/Test1 
            NewX11(int32 item) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 08 00 00 00 00 00 00 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  newobj     instance void Match01/Test1/X11::.ctor(int32)
      IL_0006:  ret
    } // end of method Test1::NewX11

    .method public hidebysig instance bool 
            get_IsX11() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       10 (0xa)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance int32 Match01/Test1::get_Tag()
      IL_0006:  ldc.i4.0
      IL_0007:  ceq
      IL_0009:  ret
    } // end of method Test1::get_IsX11

    .method public static class Match01/Test1 
            NewX12(int32 item) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 08 00 00 00 01 00 00 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  newobj     instance void Match01/Test1/X12::.ctor(int32)
      IL_0006:  ret
    } // end of method Test1::NewX12

    .method public hidebysig instance bool 
            get_IsX12() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       10 (0xa)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance int32 Match01/Test1::get_Tag()
      IL_0006:  ldc.i4.1
      IL_0007:  ceq
      IL_0009:  ret
    } // end of method Test1::get_IsX12

    .method public static class Match01/Test1 
            NewX13(int32 item) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 08 00 00 00 02 00 00 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  newobj     instance void Match01/Test1/X13::.ctor(int32)
      IL_0006:  ret
    } // end of method Test1::NewX13

    .method public hidebysig instance bool 
            get_IsX13() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       10 (0xa)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance int32 Match01/Test1::get_Tag()
      IL_0006:  ldc.i4.2
      IL_0007:  ceq
      IL_0009:  ret
    } // end of method Test1::get_IsX13

    .method public static class Match01/Test1 
            NewX14(int32 item) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 08 00 00 00 03 00 00 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  newobj     instance void Match01/Test1/X14::.ctor(int32)
      IL_0006:  ret
    } // end of method Test1::NewX14

    .method public hidebysig instance bool 
            get_IsX14() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       10 (0xa)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance int32 Match01/Test1::get_Tag()
      IL_0006:  ldc.i4.3
      IL_0007:  ceq
      IL_0009:  ret
    } // end of method Test1::get_IsX14

    .method public hidebysig instance int32 
            get_Tag() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 Match01/Test1::_tag
      IL_0006:  ret
    } // end of method Test1::get_Tag

    .method assembly hidebysig specialname 
            instance object  __DebugDisplay() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       22 (0x16)
      .maxstack  8
      IL_0000:  ldstr      "%+0.8A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Match01/Test1,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,string>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Match01/Test1,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Match01/Test1,string>::Invoke(!0)
      IL_0015:  ret
    } // end of method Test1::__DebugDisplay

    .method public strict virtual instance string 
            ToString() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       22 (0x16)
      .maxstack  8
      IL_0000:  ldstr      "%+A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Match01/Test1,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class Match01/Test1>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Match01/Test1,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class Match01/Test1,string>::Invoke(!0)
      IL_0015:  ret
    } // end of method Test1::ToString

    .method public hidebysig virtual final 
            instance int32  CompareTo(class Match01/Test1 obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       46 (0x2e)
      .maxstack  5
      .locals init (int32 V_0,
               int32 V_1)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0027

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_0025

      IL_0006:  ldarg.0
      IL_0007:  ldfld      int32 Match01/Test1::_tag
      IL_000c:  stloc.0
      IL_000d:  ldarg.1
      IL_000e:  ldfld      int32 Match01/Test1::_tag
      IL_0013:  stloc.1
      IL_0014:  ldloc.0
      IL_0015:  ldloc.1
      IL_0016:  bne.un.s   IL_0021

      IL_0018:  ldarg.0
      IL_0019:  ldarg.1
      IL_001a:  ldnull
      IL_001b:  call       int32 Match01::CompareTo$cont@4(class Match01/Test1,
                                                           class Match01/Test1,
                                                           class [FSharp.Core]Microsoft.FSharp.Core.Unit)
      IL_0020:  ret

      IL_0021:  ldloc.0
      IL_0022:  ldloc.1
      IL_0023:  sub
      IL_0024:  ret

      IL_0025:  ldc.i4.1
      IL_0026:  ret

      IL_0027:  ldarg.1
      IL_0028:  brfalse.s  IL_002c

      IL_002a:  ldc.i4.m1
      IL_002b:  ret

      IL_002c:  ldc.i4.0
      IL_002d:  ret
    } // end of method Test1::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  unbox.any  Match01/Test1
      IL_0007:  callvirt   instance int32 Match01/Test1::CompareTo(class Match01/Test1)
      IL_000c:  ret
    } // end of method Test1::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj,
                                      class [mscorlib]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       63 (0x3f)
      .maxstack  5
      .locals init (class Match01/Test1 V_0,
               int32 V_1,
               int32 V_2)
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  Match01/Test1
      IL_0006:  stloc.0
      IL_0007:  ldarg.0
      IL_0008:  brfalse.s  IL_0033

      IL_000a:  ldarg.1
      IL_000b:  unbox.any  Match01/Test1
      IL_0010:  brfalse.s  IL_0031

      IL_0012:  ldarg.0
      IL_0013:  ldfld      int32 Match01/Test1::_tag
      IL_0018:  stloc.1
      IL_0019:  ldloc.0
      IL_001a:  ldfld      int32 Match01/Test1::_tag
      IL_001f:  stloc.2
      IL_0020:  ldloc.1
      IL_0021:  ldloc.2
      IL_0022:  bne.un.s   IL_002d

      IL_0024:  ldarg.0
      IL_0025:  ldloc.0
      IL_0026:  ldnull
      IL_0027:  call       int32 Match01::'CompareTo$cont@4-1'(class Match01/Test1,
                                                               class Match01/Test1,
                                                               class [FSharp.Core]Microsoft.FSharp.Core.Unit)
      IL_002c:  ret

      IL_002d:  ldloc.1
      IL_002e:  ldloc.2
      IL_002f:  sub
      IL_0030:  ret

      IL_0031:  ldc.i4.1
      IL_0032:  ret

      IL_0033:  ldarg.1
      IL_0034:  unbox.any  Match01/Test1
      IL_0039:  brfalse.s  IL_003d

      IL_003b:  ldc.i4.m1
      IL_003c:  ret

      IL_003d:  ldc.i4.0
      IL_003e:  ret
    } // end of method Test1::CompareTo

    .method public hidebysig virtual final 
            instance int32  GetHashCode(class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       167 (0xa7)
      .maxstack  7
      .locals init (int32 V_0,
               class Match01/Test1/X11 V_1,
               class Match01/Test1/X12 V_2,
               class Match01/Test1/X13 V_3,
               class Match01/Test1/X14 V_4)
      IL_0000:  ldarg.0
      IL_0001:  brfalse    IL_00a5

      IL_0006:  ldc.i4.0
      IL_0007:  stloc.0
      IL_0008:  ldarg.0
      IL_0009:  call       instance int32 Match01/Test1::get_Tag()
      IL_000e:  switch     ( 
                            IL_0023,
                            IL_0043,
                            IL_0063,
                            IL_0083)
      IL_0023:  ldarg.0
      IL_0024:  castclass  Match01/Test1/X11
      IL_0029:  stloc.1
      IL_002a:  ldc.i4.0
      IL_002b:  stloc.0
      IL_002c:  ldc.i4     0x9e3779b9
      IL_0031:  ldloc.1
      IL_0032:  ldfld      int32 Match01/Test1/X11::item
      IL_0037:  ldloc.0
      IL_0038:  ldc.i4.6
      IL_0039:  shl
      IL_003a:  ldloc.0
      IL_003b:  ldc.i4.2
      IL_003c:  shr
      IL_003d:  add
      IL_003e:  add
      IL_003f:  add
      IL_0040:  stloc.0
      IL_0041:  ldloc.0
      IL_0042:  ret

      IL_0043:  ldarg.0
      IL_0044:  castclass  Match01/Test1/X12
      IL_0049:  stloc.2
      IL_004a:  ldc.i4.1
      IL_004b:  stloc.0
      IL_004c:  ldc.i4     0x9e3779b9
      IL_0051:  ldloc.2
      IL_0052:  ldfld      int32 Match01/Test1/X12::item
      IL_0057:  ldloc.0
      IL_0058:  ldc.i4.6
      IL_0059:  shl
      IL_005a:  ldloc.0
      IL_005b:  ldc.i4.2
      IL_005c:  shr
      IL_005d:  add
      IL_005e:  add
      IL_005f:  add
      IL_0060:  stloc.0
      IL_0061:  ldloc.0
      IL_0062:  ret

      IL_0063:  ldarg.0
      IL_0064:  castclass  Match01/Test1/X13
      IL_0069:  stloc.3
      IL_006a:  ldc.i4.2
      IL_006b:  stloc.0
      IL_006c:  ldc.i4     0x9e3779b9
      IL_0071:  ldloc.3
      IL_0072:  ldfld      int32 Match01/Test1/X13::item
      IL_0077:  ldloc.0
      IL_0078:  ldc.i4.6
      IL_0079:  shl
      IL_007a:  ldloc.0
      IL_007b:  ldc.i4.2
      IL_007c:  shr
      IL_007d:  add
      IL_007e:  add
      IL_007f:  add
      IL_0080:  stloc.0
      IL_0081:  ldloc.0
      IL_0082:  ret

      IL_0083:  ldarg.0
      IL_0084:  castclass  Match01/Test1/X14
      IL_0089:  stloc.s    V_4
      IL_008b:  ldc.i4.3
      IL_008c:  stloc.0
      IL_008d:  ldc.i4     0x9e3779b9
      IL_0092:  ldloc.s    V_4
      IL_0094:  ldfld      int32 Match01/Test1/X14::item
      IL_0099:  ldloc.0
      IL_009a:  ldc.i4.6
      IL_009b:  shl
      IL_009c:  ldloc.0
      IL_009d:  ldc.i4.2
      IL_009e:  shr
      IL_009f:  add
      IL_00a0:  add
      IL_00a1:  add
      IL_00a2:  stloc.0
      IL_00a3:  ldloc.0
      IL_00a4:  ret

      IL_00a5:  ldc.i4.0
      IL_00a6:  ret
    } // end of method Test1::GetHashCode

    .method public hidebysig virtual final 
            instance int32  GetHashCode() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       class [mscorlib]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0006:  callvirt   instance int32 Match01/Test1::GetHashCode(class [mscorlib]System.Collections.IEqualityComparer)
      IL_000b:  ret
    } // end of method Test1::GetHashCode

    .method public hidebysig virtual final 
            instance bool  Equals(object obj,
                                  class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       209 (0xd1)
      .maxstack  4
      .locals init (class Match01/Test1 V_0,
               int32 V_1,
               int32 V_2,
               class Match01/Test1/X11 V_3,
               class Match01/Test1/X11 V_4,
               class Match01/Test1/X12 V_5,
               class Match01/Test1/X12 V_6,
               class Match01/Test1/X13 V_7,
               class Match01/Test1/X13 V_8,
               class Match01/Test1/X14 V_9,
               class Match01/Test1/X14 V_10)
      IL_0000:  ldarg.0
      IL_0001:  brfalse    IL_00c9

      IL_0006:  ldarg.1
      IL_0007:  isinst     Match01/Test1
      IL_000c:  stloc.0
      IL_000d:  ldloc.0
      IL_000e:  brfalse    IL_00c7

      IL_0013:  ldarg.0
      IL_0014:  ldfld      int32 Match01/Test1::_tag
      IL_0019:  stloc.1
      IL_001a:  ldloc.0
      IL_001b:  ldfld      int32 Match01/Test1::_tag
      IL_0020:  stloc.2
      IL_0021:  ldloc.1
      IL_0022:  ldloc.2
      IL_0023:  bne.un     IL_00c5

      IL_0028:  ldarg.0
      IL_0029:  call       instance int32 Match01/Test1::get_Tag()
      IL_002e:  switch     ( 
                            IL_0043,
                            IL_0062,
                            IL_0083,
                            IL_00a4)
      IL_0043:  ldarg.0
      IL_0044:  castclass  Match01/Test1/X11
      IL_0049:  stloc.3
      IL_004a:  ldloc.0
      IL_004b:  castclass  Match01/Test1/X11
      IL_0050:  stloc.s    V_4
      IL_0052:  ldloc.3
      IL_0053:  ldfld      int32 Match01/Test1/X11::item
      IL_0058:  ldloc.s    V_4
      IL_005a:  ldfld      int32 Match01/Test1/X11::item
      IL_005f:  ceq
      IL_0061:  ret

      IL_0062:  ldarg.0
      IL_0063:  castclass  Match01/Test1/X12
      IL_0068:  stloc.s    V_5
      IL_006a:  ldloc.0
      IL_006b:  castclass  Match01/Test1/X12
      IL_0070:  stloc.s    V_6
      IL_0072:  ldloc.s    V_5
      IL_0074:  ldfld      int32 Match01/Test1/X12::item
      IL_0079:  ldloc.s    V_6
      IL_007b:  ldfld      int32 Match01/Test1/X12::item
      IL_0080:  ceq
      IL_0082:  ret

      IL_0083:  ldarg.0
      IL_0084:  castclass  Match01/Test1/X13
      IL_0089:  stloc.s    V_7
      IL_008b:  ldloc.0
      IL_008c:  castclass  Match01/Test1/X13
      IL_0091:  stloc.s    V_8
      IL_0093:  ldloc.s    V_7
      IL_0095:  ldfld      int32 Match01/Test1/X13::item
      IL_009a:  ldloc.s    V_8
      IL_009c:  ldfld      int32 Match01/Test1/X13::item
      IL_00a1:  ceq
      IL_00a3:  ret

      IL_00a4:  ldarg.0
      IL_00a5:  castclass  Match01/Test1/X14
      IL_00aa:  stloc.s    V_9
      IL_00ac:  ldloc.0
      IL_00ad:  castclass  Match01/Test1/X14
      IL_00b2:  stloc.s    V_10
      IL_00b4:  ldloc.s    V_9
      IL_00b6:  ldfld      int32 Match01/Test1/X14::item
      IL_00bb:  ldloc.s    V_10
      IL_00bd:  ldfld      int32 Match01/Test1/X14::item
      IL_00c2:  ceq
      IL_00c4:  ret

      IL_00c5:  ldc.i4.0
      IL_00c6:  ret

      IL_00c7:  ldc.i4.0
      IL_00c8:  ret

      IL_00c9:  ldarg.1
      IL_00ca:  ldnull
      IL_00cb:  cgt.un
      IL_00cd:  ldc.i4.0
      IL_00ce:  ceq
      IL_00d0:  ret
    } // end of method Test1::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(class Match01/Test1 obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       200 (0xc8)
      .maxstack  4
      .locals init (int32 V_0,
               int32 V_1,
               class Match01/Test1/X11 V_2,
               class Match01/Test1/X11 V_3,
               class Match01/Test1/X12 V_4,
               class Match01/Test1/X12 V_5,
               class Match01/Test1/X13 V_6,
               class Match01/Test1/X13 V_7,
               class Match01/Test1/X14 V_8,
               class Match01/Test1/X14 V_9)
      IL_0000:  ldarg.0
      IL_0001:  brfalse    IL_00c0

      IL_0006:  ldarg.1
      IL_0007:  brfalse    IL_00be

      IL_000c:  ldarg.0
      IL_000d:  ldfld      int32 Match01/Test1::_tag
      IL_0012:  stloc.0
      IL_0013:  ldarg.1
      IL_0014:  ldfld      int32 Match01/Test1::_tag
      IL_0019:  stloc.1
      IL_001a:  ldloc.0
      IL_001b:  ldloc.1
      IL_001c:  bne.un     IL_00bc

      IL_0021:  ldarg.0
      IL_0022:  call       instance int32 Match01/Test1::get_Tag()
      IL_0027:  switch     ( 
                            IL_003c,
                            IL_0059,
                            IL_007a,
                            IL_009b)
      IL_003c:  ldarg.0
      IL_003d:  castclass  Match01/Test1/X11
      IL_0042:  stloc.2
      IL_0043:  ldarg.1
      IL_0044:  castclass  Match01/Test1/X11
      IL_0049:  stloc.3
      IL_004a:  ldloc.2
      IL_004b:  ldfld      int32 Match01/Test1/X11::item
      IL_0050:  ldloc.3
      IL_0051:  ldfld      int32 Match01/Test1/X11::item
      IL_0056:  ceq
      IL_0058:  ret

      IL_0059:  ldarg.0
      IL_005a:  castclass  Match01/Test1/X12
      IL_005f:  stloc.s    V_4
      IL_0061:  ldarg.1
      IL_0062:  castclass  Match01/Test1/X12
      IL_0067:  stloc.s    V_5
      IL_0069:  ldloc.s    V_4
      IL_006b:  ldfld      int32 Match01/Test1/X12::item
      IL_0070:  ldloc.s    V_5
      IL_0072:  ldfld      int32 Match01/Test1/X12::item
      IL_0077:  ceq
      IL_0079:  ret

      IL_007a:  ldarg.0
      IL_007b:  castclass  Match01/Test1/X13
      IL_0080:  stloc.s    V_6
      IL_0082:  ldarg.1
      IL_0083:  castclass  Match01/Test1/X13
      IL_0088:  stloc.s    V_7
      IL_008a:  ldloc.s    V_6
      IL_008c:  ldfld      int32 Match01/Test1/X13::item
      IL_0091:  ldloc.s    V_7
      IL_0093:  ldfld      int32 Match01/Test1/X13::item
      IL_0098:  ceq
      IL_009a:  ret

      IL_009b:  ldarg.0
      IL_009c:  castclass  Match01/Test1/X14
      IL_00a1:  stloc.s    V_8
      IL_00a3:  ldarg.1
      IL_00a4:  castclass  Match01/Test1/X14
      IL_00a9:  stloc.s    V_9
      IL_00ab:  ldloc.s    V_8
      IL_00ad:  ldfld      int32 Match01/Test1/X14::item
      IL_00b2:  ldloc.s    V_9
      IL_00b4:  ldfld      int32 Match01/Test1/X14::item
      IL_00b9:  ceq
      IL_00bb:  ret

      IL_00bc:  ldc.i4.0
      IL_00bd:  ret

      IL_00be:  ldc.i4.0
      IL_00bf:  ret

      IL_00c0:  ldarg.1
      IL_00c1:  ldnull
      IL_00c2:  cgt.un
      IL_00c4:  ldc.i4.0
      IL_00c5:  ceq
      IL_00c7:  ret
    } // end of method Test1::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(object obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       20 (0x14)
      .maxstack  4
      .locals init (class Match01/Test1 V_0)
      IL_0000:  ldarg.1
      IL_0001:  isinst     Match01/Test1
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0012

      IL_000a:  ldarg.0
      IL_000b:  ldloc.0
      IL_000c:  callvirt   instance bool Match01/Test1::Equals(class Match01/Test1)
      IL_0011:  ret

      IL_0012:  ldc.i4.0
      IL_0013:  ret
    } // end of method Test1::Equals

    .property instance int32 Tag()
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance int32 Match01/Test1::get_Tag()
    } // end of property Test1::Tag
    .property instance bool IsX11()
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance bool Match01/Test1::get_IsX11()
    } // end of property Test1::IsX11
    .property instance bool IsX12()
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance bool Match01/Test1::get_IsX12()
    } // end of property Test1::IsX12
    .property instance bool IsX13()
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance bool Match01/Test1::get_IsX13()
    } // end of property Test1::IsX13
    .property instance bool IsX14()
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance bool Match01/Test1::get_IsX14()
    } // end of property Test1::IsX14
  } // end of class Test1

  .method assembly static int32  CompareTo$cont@4(class Match01/Test1 this,
                                                  class Match01/Test1 obj,
                                                  class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       231 (0xe7)
    .maxstack  4
    .locals init (class Match01/Test1/X11 V_0,
             class Match01/Test1/X11 V_1,
             class [mscorlib]System.Collections.IComparer V_2,
             int32 V_3,
             int32 V_4,
             class Match01/Test1/X12 V_5,
             class Match01/Test1/X12 V_6,
             class Match01/Test1/X13 V_7,
             class Match01/Test1/X13 V_8,
             class Match01/Test1/X14 V_9,
             class Match01/Test1/X14 V_10)
    IL_0000:  ldarg.0
    IL_0001:  call       instance int32 Match01/Test1::get_Tag()
    IL_0006:  switch     ( 
                          IL_001b,
                          IL_004b,
                          IL_007f,
                          IL_00b3)
    IL_001b:  ldarg.0
    IL_001c:  castclass  Match01/Test1/X11
    IL_0021:  stloc.0
    IL_0022:  ldarg.1
    IL_0023:  castclass  Match01/Test1/X11
    IL_0028:  stloc.1
    IL_0029:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
    IL_002e:  stloc.2
    IL_002f:  ldloc.0
    IL_0030:  ldfld      int32 Match01/Test1/X11::item
    IL_0035:  stloc.3
    IL_0036:  ldloc.1
    IL_0037:  ldfld      int32 Match01/Test1/X11::item
    IL_003c:  stloc.s    V_4
    IL_003e:  ldloc.3
    IL_003f:  ldloc.s    V_4
    IL_0041:  bge.s      IL_0045

    IL_0043:  ldc.i4.m1
    IL_0044:  ret

    IL_0045:  ldloc.3
    IL_0046:  ldloc.s    V_4
    IL_0048:  cgt
    IL_004a:  ret

    IL_004b:  ldarg.0
    IL_004c:  castclass  Match01/Test1/X12
    IL_0051:  stloc.s    V_5
    IL_0053:  ldarg.1
    IL_0054:  castclass  Match01/Test1/X12
    IL_0059:  stloc.s    V_6
    IL_005b:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
    IL_0060:  stloc.2
    IL_0061:  ldloc.s    V_5
    IL_0063:  ldfld      int32 Match01/Test1/X12::item
    IL_0068:  stloc.3
    IL_0069:  ldloc.s    V_6
    IL_006b:  ldfld      int32 Match01/Test1/X12::item
    IL_0070:  stloc.s    V_4
    IL_0072:  ldloc.3
    IL_0073:  ldloc.s    V_4
    IL_0075:  bge.s      IL_0079

    IL_0077:  ldc.i4.m1
    IL_0078:  ret

    IL_0079:  ldloc.3
    IL_007a:  ldloc.s    V_4
    IL_007c:  cgt
    IL_007e:  ret

    IL_007f:  ldarg.0
    IL_0080:  castclass  Match01/Test1/X13
    IL_0085:  stloc.s    V_7
    IL_0087:  ldarg.1
    IL_0088:  castclass  Match01/Test1/X13
    IL_008d:  stloc.s    V_8
    IL_008f:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
    IL_0094:  stloc.2
    IL_0095:  ldloc.s    V_7
    IL_0097:  ldfld      int32 Match01/Test1/X13::item
    IL_009c:  stloc.3
    IL_009d:  ldloc.s    V_8
    IL_009f:  ldfld      int32 Match01/Test1/X13::item
    IL_00a4:  stloc.s    V_4
    IL_00a6:  ldloc.3
    IL_00a7:  ldloc.s    V_4
    IL_00a9:  bge.s      IL_00ad

    IL_00ab:  ldc.i4.m1
    IL_00ac:  ret

    IL_00ad:  ldloc.3
    IL_00ae:  ldloc.s    V_4
    IL_00b0:  cgt
    IL_00b2:  ret

    IL_00b3:  ldarg.0
    IL_00b4:  castclass  Match01/Test1/X14
    IL_00b9:  stloc.s    V_9
    IL_00bb:  ldarg.1
    IL_00bc:  castclass  Match01/Test1/X14
    IL_00c1:  stloc.s    V_10
    IL_00c3:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
    IL_00c8:  stloc.2
    IL_00c9:  ldloc.s    V_9
    IL_00cb:  ldfld      int32 Match01/Test1/X14::item
    IL_00d0:  stloc.3
    IL_00d1:  ldloc.s    V_10
    IL_00d3:  ldfld      int32 Match01/Test1/X14::item
    IL_00d8:  stloc.s    V_4
    IL_00da:  ldloc.3
    IL_00db:  ldloc.s    V_4
    IL_00dd:  bge.s      IL_00e1

    IL_00df:  ldc.i4.m1
    IL_00e0:  ret

    IL_00e1:  ldloc.3
    IL_00e2:  ldloc.s    V_4
    IL_00e4:  cgt
    IL_00e6:  ret
  } // end of method Match01::CompareTo$cont@4

  .method assembly static int32  'CompareTo$cont@4-1'(class Match01/Test1 this,
                                                      class Match01/Test1 objTemp,
                                                      class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       195 (0xc3)
    .maxstack  4
    .locals init (class Match01/Test1/X11 V_0,
             class Match01/Test1/X11 V_1,
             int32 V_2,
             int32 V_3,
             class Match01/Test1/X12 V_4,
             class Match01/Test1/X12 V_5,
             class Match01/Test1/X13 V_6,
             class Match01/Test1/X13 V_7,
             class Match01/Test1/X14 V_8,
             class Match01/Test1/X14 V_9)
    IL_0000:  ldarg.0
    IL_0001:  call       instance int32 Match01/Test1::get_Tag()
    IL_0006:  switch     ( 
                          IL_001b,
                          IL_0042,
                          IL_006d,
                          IL_0098)
    IL_001b:  ldarg.0
    IL_001c:  castclass  Match01/Test1/X11
    IL_0021:  stloc.0
    IL_0022:  ldarg.1
    IL_0023:  castclass  Match01/Test1/X11
    IL_0028:  stloc.1
    IL_0029:  ldloc.0
    IL_002a:  ldfld      int32 Match01/Test1/X11::item
    IL_002f:  stloc.2
    IL_0030:  ldloc.1
    IL_0031:  ldfld      int32 Match01/Test1/X11::item
    IL_0036:  stloc.3
    IL_0037:  ldloc.2
    IL_0038:  ldloc.3
    IL_0039:  bge.s      IL_003d

    IL_003b:  ldc.i4.m1
    IL_003c:  ret

    IL_003d:  ldloc.2
    IL_003e:  ldloc.3
    IL_003f:  cgt
    IL_0041:  ret

    IL_0042:  ldarg.0
    IL_0043:  castclass  Match01/Test1/X12
    IL_0048:  stloc.s    V_4
    IL_004a:  ldarg.1
    IL_004b:  castclass  Match01/Test1/X12
    IL_0050:  stloc.s    V_5
    IL_0052:  ldloc.s    V_4
    IL_0054:  ldfld      int32 Match01/Test1/X12::item
    IL_0059:  stloc.2
    IL_005a:  ldloc.s    V_5
    IL_005c:  ldfld      int32 Match01/Test1/X12::item
    IL_0061:  stloc.3
    IL_0062:  ldloc.2
    IL_0063:  ldloc.3
    IL_0064:  bge.s      IL_0068

    IL_0066:  ldc.i4.m1
    IL_0067:  ret

    IL_0068:  ldloc.2
    IL_0069:  ldloc.3
    IL_006a:  cgt
    IL_006c:  ret

    IL_006d:  ldarg.0
    IL_006e:  castclass  Match01/Test1/X13
    IL_0073:  stloc.s    V_6
    IL_0075:  ldarg.1
    IL_0076:  castclass  Match01/Test1/X13
    IL_007b:  stloc.s    V_7
    IL_007d:  ldloc.s    V_6
    IL_007f:  ldfld      int32 Match01/Test1/X13::item
    IL_0084:  stloc.2
    IL_0085:  ldloc.s    V_7
    IL_0087:  ldfld      int32 Match01/Test1/X13::item
    IL_008c:  stloc.3
    IL_008d:  ldloc.2
    IL_008e:  ldloc.3
    IL_008f:  bge.s      IL_0093

    IL_0091:  ldc.i4.m1
    IL_0092:  ret

    IL_0093:  ldloc.2
    IL_0094:  ldloc.3
    IL_0095:  cgt
    IL_0097:  ret

    IL_0098:  ldarg.0
    IL_0099:  castclass  Match01/Test1/X14
    IL_009e:  stloc.s    V_8
    IL_00a0:  ldarg.1
    IL_00a1:  castclass  Match01/Test1/X14
    IL_00a6:  stloc.s    V_9
    IL_00a8:  ldloc.s    V_8
    IL_00aa:  ldfld      int32 Match01/Test1/X14::item
    IL_00af:  stloc.2
    IL_00b0:  ldloc.s    V_9
    IL_00b2:  ldfld      int32 Match01/Test1/X14::item
    IL_00b7:  stloc.3
    IL_00b8:  ldloc.2
    IL_00b9:  ldloc.3
    IL_00ba:  bge.s      IL_00be

    IL_00bc:  ldc.i4.m1
    IL_00bd:  ret

    IL_00be:  ldloc.2
    IL_00bf:  ldloc.3
    IL_00c0:  cgt
    IL_00c2:  ret
  } // end of method Match01::'CompareTo$cont@4-1'

  .method public static int32  select1(class Match01/Test1 x) cil managed
  {
    // Code size       46 (0x2e)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  call       instance int32 Match01/Test1::get_Tag()
    IL_0007:  switch     ( 
                          IL_001c,
                          IL_0028,
                          IL_002a,
                          IL_002c)
    IL_001c:  ldarg.0
    IL_001d:  castclass  Match01/Test1/X11
    IL_0022:  ldfld      int32 Match01/Test1/X11::item
    IL_0027:  ret

    IL_0028:  ldc.i4.2
    IL_0029:  ret

    IL_002a:  ldc.i4.3
    IL_002b:  ret

    IL_002c:  ldc.i4.4
    IL_002d:  ret
  } // end of method Match01::select1

  .method public static int32  fm(class Match01/Test1 y) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       int32 Match01::select1(class Match01/Test1)
    IL_0006:  ret
  } // end of method Match01::fm

} // end of class Match01

.class private abstract auto ansi sealed '<StartupCode$Match01>'.$Match01
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $Match01::main@

} // end of class '<StartupCode$Match01>'.$Match01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file C:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net472\tests\EmittedIL\Inlining\Match01_fs\Match01.res
